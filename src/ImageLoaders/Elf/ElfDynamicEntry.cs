#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Elf
{
    public class ElfDynamicEntry
    {
        public ElfDynamicEntry(long tag, ulong value)
        {
            this.Tag = (int)tag;
            this.UValue = value;
        }

        public ulong UValue { get; }
        public long SValue => (long)UValue;
        public int Tag { get; }

        public TagInfo GetTagInfo(ElfMachine machine)
        {
            if (TagInfos.TryGetValue(Tag, out var info))
                return info;
            if (MachineSpecificInfos.TryGetValue(machine, out var machineInfos))
            {
                if (machineInfos.TryGetValue(Tag, out info))
                    return info;
            }
            return null;
        }


        public class TagInfo
        {
            public string Name;
            public DtFormat Format;
        }


        public static Dictionary<long, TagInfo> TagInfos = new Dictionary<long, TagInfo>
        {
            { ElfLoader.DT_DEBUG,   new TagInfo { Name = "DT_DEBUG", Format= DtFormat.Hexadecimal } },
            { ElfLoader.DT_FINI,    new TagInfo { Name="DT_DEBUG", Format = DtFormat.Address } },
            { ElfLoader.DT_HASH,    new TagInfo { Name = "DT_HASH", Format = DtFormat.Address} },
            { ElfLoader.DT_INIT,    new TagInfo { Name = "DT_INIT", Format = DtFormat.Address} },
            { ElfLoader.DT_RELA,    new TagInfo { Name = "DT_RELA", Format = DtFormat.Address} },
            { ElfLoader.DT_RELASZ,  new TagInfo { Name = "DT_RELASZ", Format = DtFormat.Decimal } },
            { ElfLoader.DT_RELAENT, new TagInfo { Name = "DT_RELAENT", Format = DtFormat.Decimal } },
            { ElfLoader.DT_PLTGOT,  new TagInfo { Name = "DT_PLTGOT", Format = DtFormat.Address} },
            { ElfLoader.DT_PLTREL,  new TagInfo { Name = "DT_PLTREL", Format = DtFormat.Hexadecimal } },
            { ElfLoader.DT_PLTRELSZ, new TagInfo { Name = "DT_PLTRELSZ", Format = DtFormat.Decimal } },

            { ElfLoader.DT_REL,     new TagInfo { Name="DT_REL", Format = DtFormat.Address } },
            { ElfLoader.DT_RELSZ,   new TagInfo { Name="DT_RELSZ", Format = DtFormat.Decimal } },
            { ElfLoader.DT_RELENT,  new TagInfo { Name="DT_RELENT", Format = DtFormat.Decimal } },

            { ElfLoader.DT_JMPREL,  new TagInfo { Name = "DT_JMPREL", Format = DtFormat.Address} },
            { ElfLoader.DT_NEEDED,  new TagInfo { Name ="DT_NEEDED",  Format = DtFormat.String } },
            { ElfLoader.DT_STRSZ,   new TagInfo { Name = "DT_STRSZ", Format= DtFormat.Hexadecimal } },
            { ElfLoader.DT_STRTAB,  new TagInfo { Name = "DT_STRTAB", Format = DtFormat.Address} },
            { ElfLoader.DT_SYMENT,  new TagInfo { Name = "DT_SYMENT", Format = DtFormat.Decimal } },
            { ElfLoader.DT_SYMTAB,  new TagInfo { Name = "DT_SYMTAB", Format = DtFormat.Address} },
            { ElfLoader.DT_INIT_ARRAY,  new TagInfo { Name = "DT_INIT_ARRAY", Format = DtFormat.Address} },
            { ElfLoader.DT_FINI_ARRAY,  new TagInfo { Name = "DT_FINI_ARRAY", Format = DtFormat.Address} },
            { ElfLoader.DT_INIT_ARRAYSZ,  new TagInfo { Name = "DT_INIT_ARRAYSZ", Format = DtFormat.Hexadecimal} },
            { ElfLoader.DT_FINI_ARRAYSZ,  new TagInfo { Name = "DT_FINI_ARRAYSZ", Format = DtFormat.Hexadecimal} },
        };

        public static Dictionary<ElfMachine, Dictionary<long, TagInfo>> MachineSpecificInfos = new Dictionary<ElfMachine, Dictionary<long, TagInfo>>
        {
            {
                ElfMachine.EM_MIPS,
                new Dictionary<long, TagInfo>
                {
                    { Mips.DT_MIPS_GOTSYM, new TagInfo { Name = "DT_MIPS_GOTSYM", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_RLD_MAP, new TagInfo { Name = "DT_MIPS_RLD_MAP", Format= DtFormat.Address } },
                    { Mips.DT_MIPS_RLD_VERSION, new TagInfo { Name = "DT_MIPS_RLD_VERSION", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_FLAGS, new TagInfo { Name = "DT_MIPS_FLAGS", Format= DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_BASE_ADDRESS, new TagInfo { Name = "DT_MIPS_BASE_ADDRESS", Format= DtFormat.Address } },
                    { Mips.DT_MIPS_LOCAL_GOTNO, new TagInfo { Name = "DT_MIPS_LOCAL_GOTNO", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_SYMTABNO, new TagInfo { Name = "DT_MIPS_SYMTABNO", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_UNREFEXTNO, new TagInfo { Name = "DT_MIPS_UNREFEXTNO", Format= DtFormat.Decimal } },
                }
            }
        };

        public static class Mips
        {
            // Mips specific dynamic table entry tags.
            public const int DT_MIPS_RLD_VERSION = 0x70000001; // 32 bit version number for runtime linker interface.
            public const int DT_MIPS_TIME_STAMP = 0x70000002; // Time stamp.
            public const int DT_MIPS_ICHECKSUM = 0x70000003; // Checksum of external strings and common sizes.
            public const int DT_MIPS_IVERSION = 0x70000004; // Index of version string in string table.
            public const int DT_MIPS_FLAGS = 0x70000005; // 32 bits of flags.
            public const int DT_MIPS_BASE_ADDRESS = 0x70000006; // Base address of the segment.
            public const int DT_MIPS_MSYM = 0x70000007; // Address of .msym section.
            public const int DT_MIPS_CONFLICT = 0x70000008; // Address of .conflict section.
            public const int DT_MIPS_LIBLIST = 0x70000009; // Address of .liblist section.
            public const int DT_MIPS_LOCAL_GOTNO = 0x7000000a; // Number of local global offset table entries.
            public const int DT_MIPS_CONFLICTNO = 0x7000000b; // Number of entries in the .conflict section.
            public const int DT_MIPS_LIBLISTNO = 0x70000010; // Number of entries in the .liblist section.
            public const int DT_MIPS_SYMTABNO = 0x70000011; // Number of entries in the .dynsym section.
            public const int DT_MIPS_UNREFEXTNO = 0x70000012; // Index of first external dynamic symbol not referenced locally.
            public const int DT_MIPS_GOTSYM = 0x70000013; // Index of first dynamic symbol in global offset table.
            public const int DT_MIPS_HIPAGENO = 0x70000014; // Number of page table entries in global offset table.
            public const int DT_MIPS_RLD_MAP = 0x70000016; // Address of run time loader map; used for debugging.
            public const int DT_MIPS_DELTA_CLASS = 0x70000017; // Delta C++ class definition.
            public const int DT_MIPS_DELTA_CLASS_NO = 0x70000018; // Number of entries in        public const int DT_MIPS_DELTA_CLASS.
            public const int DT_MIPS_DELTA_INSTANCE = 0x70000019; // Delta C++ class instances.
            public const int DT_MIPS_DELTA_INSTANCE_NO = 0x7000001A; // Number of entries in        public const int DT_MIPS_DELTA_INSTANCE.
            public const int DT_MIPS_DELTA_RELOC = 0x7000001B; // Delta relocations.
            public const int DT_MIPS_DELTA_RELOC_NO = 0x7000001C; // Number of entries in        public const int DT_MIPS_DELTA_RELOC.
            public const int DT_MIPS_DELTA_SYM = 0x7000001D; // Delta symbols that Delta relocations refer to.
            public const int DT_MIPS_DELTA_SYM_NO = 0x7000001E; // Number of entries in        public const int DT_MIPS_DELTA_SYM.
            public const int DT_MIPS_DELTA_CLASSSYM = 0x70000020; // Delta symbols that hold class declarations.
            public const int DT_MIPS_DELTA_CLASSSYM_NO = 0x70000021; // Number of entries in        public const int DT_MIPS_DELTA_CLASSSYM.
            public const int DT_MIPS_CXX_FLAGS = 0x70000022; // Flags indicating information about C++ flavor.
            public const int DT_MIPS_PIXIE_INIT = 0x70000023; // Pixie information.
            public const int DT_MIPS_SYMBOL_LIB = 0x70000024; // Address of .MIPS.symlib
            public const int DT_MIPS_LOCALPAGE_GOTIDX = 0x70000025; // The GOT index of the first PTE for a segment
            public const int DT_MIPS_LOCAL_GOTIDX = 0x70000026; // The GOT index of the first PTE for a local symbol
            public const int DT_MIPS_HIDDEN_GOTIDX = 0x70000027; // The GOT index of the first PTE for a hidden symbol
            public const int DT_MIPS_PROTECTED_GOTIDX = 0x70000028; // The GOT index of the first PTE for a protected symbol
            public const int DT_MIPS_OPTIONS = 0x70000029; // Address of `.MIPS.options'.
            public const int DT_MIPS_INTERFACE = 0x7000002A; // Address of `.interface'.
            public const int DT_MIPS_DYNSTR_ALIGN = 0x7000002B; // Unknown.
            public const int DT_MIPS_INTERFACE_SIZE = 0x7000002C; // Size of the .interface section.
            public const int DT_MIPS_RLD_TEXT_RESOLVE_ADDR = 0x7000002D; // Size of rld_text_resolve function stored in the GOT.
            public const int DT_MIPS_PERF_SUFFIX = 0x7000002E; // Default suffix of DSO to be added by rld on dlopen() calls.
            public const int DT_MIPS_COMPACT_SIZE = 0x7000002F; // Size of compact relocation section (O32).
            public const int DT_MIPS_GP_VALUE = 0x70000030; // GP value for auxiliary GOTs.
            public const int DT_MIPS_AUX_DYNAMIC = 0x70000031; // Address of auxiliary .dynamic.
            public const int DT_MIPS_PLTGOT = 0x70000032; // Address of the base of the PLTGOT.
            public const int DT_MIPS_RWPLT = 0x70000034; // Points to the base of a writable PLT.
            public const int DT_MIPS_RLD_MAP_REL = 0x70000035; // Relative offset of run time loader map, used for debugging.
        }

    }
}
