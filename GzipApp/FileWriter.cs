using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GzipApp
{
    public class FileWriter : IWriter
    {
        private readonly string file_path;

        public FileWriter(string file_path)
        {
            this.file_path = file_path; 
        }

        public void WriteCompressed(OrderedBuffer input)
        {
            int block_number = 0;

            using (FileStream output_stream = new FileStream(file_path, FileMode.Append))
            {
                Block block;

                while (input.TryGet(block_number, out block))
                {
                    byte[] compressed_length_buffer = BitConverter.GetBytes(block.CompressedData.Length);
                    output_stream.Write(compressed_length_buffer, 0, compressed_length_buffer.Length);

                    byte[] original_length_buffer = BitConverter.GetBytes(block.OriginalDataLength);
                    output_stream.Write(original_length_buffer, 0, original_length_buffer.Length);

                    output_stream.Write(block.CompressedData, 0, block.CompressedData.Length);
                    block_number++;
                }
            }
        }

        public void WriteDecompressed(OrderedBuffer input)
        {
            int block_number = 0;

            using (FileStream output_stream = new FileStream(file_path, FileMode.Append))
            {
                Block block;

                while (input.TryGet(block_number, out block))
                {
                    output_stream.Write(block.OriginalData, 0, block.OriginalData.Length);
                    block_number++;
                }
            }
        }
    }
}
