using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GzipApp
{
    public interface IReader
    {
        void Read(int block_size, SafeQueue<Block> queue);

        void ReadCompressed(int block_size, SafeQueue<Block> queue);
    }
}
