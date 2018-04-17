
using System;
using System.IO;

namespace GzipApp
{
    public class FileReader : IReader
    {
        private readonly string file_path;

        public FileReader(string file_path)
        {
            this.file_path = file_path;
        }
    
        public void Read(int block_size, SafeQueue<Block> queue)
        {
            int block_number = 0;
            using (var file = new FileStream(file_path, FileMode.Open))
            {
                int bytes_read = 0;
                byte[] buffer = new byte[block_size];

                while ((bytes_read = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var original_data = new byte[bytes_read];
                    Array.Copy(buffer, original_data, bytes_read);

                    queue.Enqueue(new Block(block_number, original_data, null));
                    block_number++;
                }
            }

            queue.Close();
        }

        public void ReadCompressed(int block_size, SafeQueue<Block> queue)
        {
            int block_number = 0;
            using (var file = new FileStream(file_path, FileMode.Open))
            {
                while (file.Position < file.Length)
                {
                    byte[] compressed_length_buffer = new byte[4];
                    file.Read(compressed_length_buffer, 0, compressed_length_buffer.Length);
                    int compressed_length = BitConverter.ToInt32(compressed_length_buffer, 0);

                    byte[] original_length_buffer = new byte[4];
                    file.Read(original_length_buffer, 0, original_length_buffer.Length);
                    int original_length = BitConverter.ToInt32(original_length_buffer, 0);

                    byte[] compressed_data = new byte[compressed_length];
                    file.Read(compressed_data, 0, compressed_data.Length);
                    
                    queue.Enqueue(new Block(block_number, new byte[original_length], compressed_data));
                    block_number++;
                }
            }

            queue.Close();
        }

    }
}
