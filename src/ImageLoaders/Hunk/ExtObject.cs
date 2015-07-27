using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    public class ExtObject
    {
        public ExtType type;
        public string name;
        public uint common_size;
        public uint def;
        public List<uint> refs;
    }
}
