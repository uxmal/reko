using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    public class Reference
    {
        public int bits;
        public string name;
    }
    public class Definition
    {
        public string name;
        public short value;
        public int type;
        public string memf;
    }
}
