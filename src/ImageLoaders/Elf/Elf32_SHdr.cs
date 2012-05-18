using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_SHdr
    {
        public uint sh_name;
        public SectionHeaderType sh_type;
        public uint sh_flags;
        public uint sh_addr;        // Address
        public uint sh_offset;
        public uint sh_size;
        public uint sh_link;
        public uint sh_info;
        public uint sh_addralign;
        public uint sh_entsize;
    }
}