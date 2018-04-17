using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GzipApp
{
    public interface IWriter
    {
        void WriteCompressed(OrderedBuffer input);

        void WriteDecompressed(OrderedBuffer input);
    }
}
