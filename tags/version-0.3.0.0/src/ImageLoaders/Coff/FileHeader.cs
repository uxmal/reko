using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Coff
{
    public class FileHeader
    {
        public ushort f_magic;         // magic number
        public ushort f_nscns;         // number of sections
        public uint f_timdat;          // time & date stamp
        public uint f_symptr;          // file pointer to symtab
        public uint f_nsyms;           // number of symtab entries
        public ushort f_opthdr;        // sizeof(optional hdr)
        public ushort f_flags;         // flags
    }
}
