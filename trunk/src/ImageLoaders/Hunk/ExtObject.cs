using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    class ExtObject
    {
        public string type_name;
        public int common_size;
        public int type;
        public string name;
        public int def;
        public List<uint> refs;
    }
}
