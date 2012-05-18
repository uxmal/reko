using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public enum ProgramHeaderType
    {
        PT_NULL = 0,
        PT_LOAD = 1,
        PT_DYNAMIC = 2,
        PT_INTERP = 3,
        PT_NOTE = 4,
        PT_SHLIB = 5,
        PT_PHDR = 6,
        PT_LOPROC = 0x70000000,
        PT_HIPROC = 0x7FFFFFFF,
        PT_GNU_STACK = 0x6474E551
    }
}
