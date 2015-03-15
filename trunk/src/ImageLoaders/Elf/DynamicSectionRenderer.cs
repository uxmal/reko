using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class DynamicSectionRenderer
    {
        public static DynamicSectionRenderer Load(Elf32_SHdr section, ImageReader rdr)
        {
            throw new NotImplementedException();
        }
//00 00 00 01 00 00 00 01   needed      ........
//00 00 00 01 00 00 02 DE   "
//00 00 00 01 00 00 02 EE   "
//00 00 00 01 00 00 03 06   "
//00 00 00 01 00 00 03 10   "
//00 00 00 01 00 00 03 6B   "
//00 00 00 01 00 00 03 91   "
//00 00 00 0C 10 00 29 CC   Init
//00 00 00 0D 10 03 81 B0   Fini
//00 00 00 04 10 00 01 A8   Hash
//6F FF FE F5 10 00 07 E0   ?
//00 00 00 05 10 00 14 AC   strtab
//00 00 00 06 10 00 08 3C   symtab
//00 00 00 0A 00 00 09 90   strsz
//00 00 00 0B 00 00 00 10   syment
//00 00 00 15 00 00 00 00   debug
//00 00 00 03 10 06 60 00   pltgot   -> first byte in linkage table
//00 00 00 02 00 00 08 D0   pltretsz
//00 00 00 14 00 00 00 07   pltrel
//00 00 00 17 10 00 20 FC   jmprel
//70 00 00 00 10 06 5F F4   loproc
//00 00 00 07 10 00 20 CC   rela
//00 00 00 08 00 00 09 00   relasz
//00 00 00 09 00 00 00 0C   relaent
//6F FF FF FE 10 00 1F CC  ?
//6F FF FF FF 00 00 00 04   ?
//6F FF FF F0 10 00 1E 3C  ?
    }
}
