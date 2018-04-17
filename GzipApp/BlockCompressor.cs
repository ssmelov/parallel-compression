using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GzipApp
{
    public class BlockCompressor
    {
        public void Compress(Block block)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var gzipstream = new GZipStream(ms, CompressionMode.Compress))
                    gzipstream.Write(block.OriginalData, 0, block.OriginalData.Length);

                block.CompressedData = ms.ToArray();
            }
        }

        public void Decompress(Block block)
        {
            using (MemoryStream ms = new MemoryStream(block.CompressedData))
            {
                using (var gzipstream = new GZipStream(ms, CompressionMode.Decompress))
                    gzipstream.Read(block.OriginalData, 0, block.OriginalData.Length);
            }
        }
    }
}
