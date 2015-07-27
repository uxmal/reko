using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    public class MODULEENTRY32
    {
        public ulong modBaseAddr;
        public ulong modBaseSize;
        public string szModule;
    }
}
