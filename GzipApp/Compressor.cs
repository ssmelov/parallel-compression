using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GzipApp
{
    public class Compressor
    {
        private SafeQueue<Block> compress_queue = new SafeQueue<Block>(100);
        private OrderedBuffer ordered_buffer = new OrderedBuffer(100);
        private List<string> error_messages = new List<string>();

        private int compress_threads_counter = 0;
        private const int block_size = 1024 * 1024;
        private int num_cpu = Environment.ProcessorCount;
        private Thread[] compress_threads;
       
        private readonly IReader reader;
        private readonly IWriter writer;

        public List<string> ErrorMessages
        {
            get { return error_messages; } 
        }

        public Compressor(IReader reader, IWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void Compress()
        {
            compress_threads = new Thread[num_cpu];

            for (int i = 0; i < num_cpu; i++)
            {
                compress_threads[i] = new Thread(() =>
                {
                    try
                    {
                        compress_data();
                    }
                    catch (Exception e)
                    {
                        error_messages.Add($"An error occurred: {e.Message}");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref compress_threads_counter);
                        if (compress_threads_counter == 0)
                            ordered_buffer.Close();
                    }
                });

                compress_threads[i].Start();
                Interlocked.Increment(ref compress_threads_counter);
            }

            Thread writer_thread = new Thread(() =>
            {
                try
                {
                    writer.WriteCompressed(ordered_buffer);
                }
                catch (Exception e)
                {
                    error_messages.Add($"An error occurred: {e.Message}");
                }
            });

            writer_thread.Start();

            reader.Read(block_size, compress_queue);

            writer_thread.Join();
        }

        public void Decompress()
        { 
            compress_threads = new Thread[num_cpu];

            for (int i = 0; i < num_cpu; i++)
            {
                compress_threads[i] = new Thread(() =>
                {
                    try
                    {
                        decompress_data();
                    }
                    catch (Exception e)
                    {
                        error_messages.Add($"An error occurred: {e.Message}");
                    }
                    finally
                    {
                        Interlocked.Decrement(ref compress_threads_counter);
                        if (compress_threads_counter == 0)
                            ordered_buffer.Close();
                    }
                });

                compress_threads[i].Start();
                Interlocked.Increment(ref compress_threads_counter);
            }

            Thread writer_thread = new Thread(() =>
            {
                try
                {
                    writer.WriteDecompressed(ordered_buffer);
                }
                catch (Exception e)
                {
                    error_messages.Add($"An error occurred: {e.Message}");
                }
            });

            writer_thread.Start();

            reader.ReadCompressed(block_size, compress_queue);

            writer_thread.Join();            
        }

        private void compress_data()
        {
            Block block;
            BlockCompressor block_compressor = new BlockCompressor();

            while (compress_queue.TryDequeue(out block))
            {
                block_compressor.Compress(block);
                block.OriginalData = null;

                ordered_buffer.Add(block.BlockNumber, block);
            }
        }

        private void decompress_data()
        {
            Block block;
            BlockCompressor block_compressor = new BlockCompressor();

            while (compress_queue.TryDequeue(out block))
            {
                block_compressor.Decompress(block);
                block.CompressedData = null;

                ordered_buffer.Add(block.BlockNumber, block);
            }
        }
    }
}
