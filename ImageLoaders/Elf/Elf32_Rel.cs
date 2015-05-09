using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_Rel
    {
        public uint r_offset;
        public int r_info;
    }

    public class Elf32_Rela
    {
        public uint r_offset;
        public uint r_info;
        public int r_addend;
    }
}
