using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.OdbgScript
{
    public class DEBUG_EVENT
    {
        public const int CREATE_PROCESS_DEBUG_EVENT = 1;
        public const int EXCEPTION_DEBUG_EVENT = 2;
        public int dwDebugEventCode;
    }
}
