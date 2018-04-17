
using System;

namespace GzipApp
{
    public class Block
    {
        public int BlockNumber { get; set; }
        public byte[] OriginalData { get; set; }
        public byte[] CompressedData { get; set; }

        public int OriginalDataLength { get; set; }

        public Block(int block_number, byte[] original_data, byte[] compressed_data)
        {
            this.BlockNumber = block_number;
            this.OriginalData = original_data;
            this.CompressedData = compressed_data;
            this.OriginalDataLength = original_data.Length;
        }
    }
}
