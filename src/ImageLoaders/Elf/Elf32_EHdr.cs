using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_EHdr
    {
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;
        public uint e_entry;            // Entry address
        public uint e_phoff;            // Program header offset
        public uint e_shoff;            // Section table offset
        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;      // Program header entry size
        public ushort e_phnum;          // Number of program header entries.
        public ushort e_shentsize;      // Section header size
        public ushort e_shnum;          // Number of section header entries.
        public ushort e_shstrndx;       // section name string table index
    }
}
