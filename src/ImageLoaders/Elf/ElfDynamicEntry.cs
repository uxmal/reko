#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        public const int DT_NULL = 0;
        public const int DT_NEEDED = 1;
        public const int DT_PLTRELSZ = 2;
        public const int DT_PLTGOT = 3;
        public const int DT_HASH = 4;
        public const int DT_STRTAB = 5;
        public const int DT_SYMTAB = 6;
        public const int DT_RELA = 7;
        public const int DT_RELASZ = 8;
        public const int DT_RELAENT = 9;
        public const int DT_STRSZ = 10;
        public const int DT_SYMENT = 11;
        public const int DT_INIT = 12;
        public const int DT_FINI = 13;
        public const int DT_SONAME = 14;
        public const int DT_RPATH = 15;
        public const int DT_SYMBOLIC = 16;
        public const int DT_REL = 17;
        public const int DT_RELSZ = 18;
        public const int DT_RELENT = 19;
        public const int DT_PLTREL = 20;
        public const int DT_DEBUG = 21;
        public const int DT_TEXTREL = 22;
        public const int DT_JMPREL = 23;
        public const int DT_BIND_NOW = 24;
        public const int DT_INIT_ARRAY = 25;
        public const int DT_FINI_ARRAY = 26;
        public const int DT_INIT_ARRAYSZ = 27;
        public const int DT_FINI_ARRAYSZ = 28;
        public const int DT_RUNPATH = 29;
        public const int DT_FLAGS = 30;
        public const int DT_ENCODING = 32;
        public const int DT_PREINIT_ARRAY = 32;
        public const int DT_PREINIT_ARRAYSZ = 33;
        public const int DT_MAXPOSTAGS = 34;

        public const int DT_GNU_HASH = 0x6FFFFEF5;
        public const int DT_RELACOUNT = 0x6FFFFFF9;
        public const int DT_RELCOUNT = 0x6FFFFFFA;
        public const int DT_FLAGS_1 = 0x6FFFFFFB;
        public const int DT_VERSYM = 0x6FFFFFF0;
        public const int DT_VERDEF = 0x6FFFFFFC;
        public const int DT_VERDEFNUM = 0x6FFFFFFD;
        public const int DT_VERNEED = 0x6FFFFFFE;
        public const int DT_VERNEEDNUM = 0x6FFFFFFF;


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
            { DT_DEBUG,   new TagInfo { Name = "DT_DEBUG", Format= DtFormat.Hexadecimal } },
            { DT_FINI,    new TagInfo { Name="DT_DEBUG", Format = DtFormat.Address } },
            { DT_HASH,    new TagInfo { Name = "DT_HASH", Format = DtFormat.Address} },
            { DT_INIT,    new TagInfo { Name = "DT_INIT", Format = DtFormat.Address} },
            { DT_RELA,    new TagInfo { Name = "DT_RELA", Format = DtFormat.Address} },
            { DT_RELASZ,  new TagInfo { Name = "DT_RELASZ", Format = DtFormat.Decimal } },
            { DT_RELAENT, new TagInfo { Name = "DT_RELAENT", Format = DtFormat.Decimal } },
            { DT_PLTGOT,  new TagInfo { Name = "DT_PLTGOT", Format = DtFormat.Address} },
            { DT_PLTREL,  new TagInfo { Name = "DT_PLTREL", Format = DtFormat.Hexadecimal } },
            { DT_PLTRELSZ, new TagInfo { Name = "DT_PLTRELSZ", Format = DtFormat.Decimal } },

            { DT_REL,     new TagInfo { Name="DT_REL", Format = DtFormat.Address } },
            { DT_RELSZ,   new TagInfo { Name="DT_RELSZ", Format = DtFormat.Decimal } },
            { DT_RELENT,  new TagInfo { Name="DT_RELENT", Format = DtFormat.Decimal } },

            { DT_RPATH,   new TagInfo { Name="DT_RPATH", Format = DtFormat.Hexadecimal } },
            { DT_JMPREL,  new TagInfo { Name = "DT_JMPREL", Format = DtFormat.Address} },
            { DT_NEEDED,  new TagInfo { Name ="DT_NEEDED",  Format = DtFormat.String } },
            { DT_STRSZ,   new TagInfo { Name = "DT_STRSZ", Format= DtFormat.Hexadecimal } },
            { DT_STRTAB,  new TagInfo { Name = "DT_STRTAB", Format = DtFormat.Address} },
            { DT_SYMENT,  new TagInfo { Name = "DT_SYMENT", Format = DtFormat.Decimal } },
            { DT_SYMTAB,  new TagInfo { Name = "DT_SYMTAB", Format = DtFormat.Address} },
            { DT_INIT_ARRAY,  new TagInfo { Name = "DT_INIT_ARRAY", Format = DtFormat.Address} },
            { DT_FINI_ARRAY,  new TagInfo { Name = "DT_FINI_ARRAY", Format = DtFormat.Address} },
            { DT_INIT_ARRAYSZ,  new TagInfo { Name = "DT_INIT_ARRAYSZ", Format = DtFormat.Hexadecimal} },
            { DT_FINI_ARRAYSZ,  new TagInfo { Name = "DT_FINI_ARRAYSZ", Format = DtFormat.Hexadecimal} },
            { DT_SONAME, new TagInfo { Name  = "DT_SONAME", Format =DtFormat.String } },
            { DT_FLAGS, new TagInfo { Name  = "DT_FLAGS", Format = DtFormat.Hexadecimal } },

            { DT_GNU_HASH,   new TagInfo { Name = "DT_GNU_HASH", Format = DtFormat.Hexadecimal} },
            { DT_RELACOUNT,   new TagInfo { Name = "DT_RELACOUNT", Format = DtFormat.Decimal} },
            { DT_RELCOUNT,   new TagInfo { Name = "DT_RELCOUNT", Format = DtFormat.Decimal} },
            { DT_FLAGS_1,   new TagInfo { Name = "DT_FLAGS_1", Format = DtFormat.Hexadecimal } },
            { DT_VERSYM,   new TagInfo { Name = "DT_VERSYM", Format = DtFormat.Hexadecimal } },
            { DT_VERDEF,   new TagInfo { Name = "DT_VERDEF", Format = DtFormat.Address } },
            { DT_VERDEFNUM,   new TagInfo { Name = "DT_VERDEFNUM", Format = DtFormat.Decimal } },
            { DT_VERNEED,   new TagInfo { Name = "DT_VERNEED", Format = DtFormat.Address } },
            { DT_VERNEEDNUM,   new TagInfo { Name = "DT_VERNEEDNUM", Format = DtFormat.Decimal } },
        };

        public static Dictionary<ElfMachine, Dictionary<long, TagInfo>> MachineSpecificInfos = new Dictionary<ElfMachine, Dictionary<long, TagInfo>>
        {
            {
                ElfMachine.EM_MIPS,
                new Dictionary<long, TagInfo>
                {
                    { Mips.DT_MIPS_RLD_VERSION, new TagInfo { Name = "DT_MIPS_RLD_VERSION", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_TIME_STAMP, new TagInfo { Name = "DT_MIPS_TIME_STAMP", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_ICHECKSUM, new TagInfo { Name = "DT_MIPS_ICHECKSUM", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_IVERSION, new TagInfo { Name = "DT_MIPS_IVERSION", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_FLAGS, new TagInfo { Name = "DT_MIPS_FLAGS", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_BASE_ADDRESS, new TagInfo { Name = "DT_MIPS_BASE_ADDRESS", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_MSYM, new TagInfo { Name = "DT_MIPS_MSYM", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_CONFLICT, new TagInfo { Name = "DT_MIPS_CONFLICT", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_LIBLIST, new TagInfo { Name = "DT_MIPS_LIBLIST", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_LOCAL_GOTNO, new TagInfo { Name = "DT_MIPS_LOCAL_GOTNO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_CONFLICTNO, new TagInfo { Name = "DT_MIPS_CONFLICTNO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_LIBLISTNO, new TagInfo { Name = "DT_MIPS_LIBLISTNO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_SYMTABNO, new TagInfo { Name = "DT_MIPS_SYMTABNO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_UNREFEXTNO, new TagInfo { Name = "DT_MIPS_UNREFEXTNO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_GOTSYM, new TagInfo { Name = "DT_MIPS_GOTSYM", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_HIPAGENO, new TagInfo { Name = "DT_MIPS_HIPAGENO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_RLD_MAP, new TagInfo { Name = "DT_MIPS_RLD_MAP", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_DELTA_CLASS, new TagInfo { Name = "DT_MIPS_DELTA_CLASS", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_DELTA_CLASS_NO, new TagInfo { Name = "DT_MIPS_DELTA_CLASS_NO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_DELTA_INSTANCE, new TagInfo { Name = "DT_MIPS_DELTA_INSTANCE", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_DELTA_INSTANCE_NO, new TagInfo { Name = "DT_MIPS_DELTA_INSTANCE_NO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_DELTA_RELOC, new TagInfo { Name = "DT_MIPS_DELTA_RELOC", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_DELTA_RELOC_NO, new TagInfo { Name = "DT_MIPS_DELTA_RELOC_NO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_DELTA_SYM, new TagInfo { Name = "DT_MIPS_DELTA_SYM", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_DELTA_SYM_NO, new TagInfo { Name = "DT_MIPS_DELTA_SYM_NO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_DELTA_CLASSSYM, new TagInfo { Name = "DT_MIPS_DELTA_CLASSSYM", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_DELTA_CLASSSYM_NO, new TagInfo { Name = "DT_MIPS_DELTA_CLASSSYM_NO", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_CXX_FLAGS, new TagInfo { Name = "DT_MIPS_CXX_FLAGS", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_PIXIE_INIT, new TagInfo { Name = "DT_MIPS_PIXIE_INIT", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_SYMBOL_LIB, new TagInfo { Name = "DT_MIPS_SYMBOL_LIB", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_LOCALPAGE_GOTIDX, new TagInfo { Name = "DT_MIPS_LOCALPAGE_GOTIDX", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_LOCAL_GOTIDX, new TagInfo { Name = "DT_MIPS_LOCAL_GOTIDX", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_HIDDEN_GOTIDX, new TagInfo { Name = "DT_MIPS_HIDDEN_GOTIDX", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_PROTECTED_GOTIDX, new TagInfo { Name = "DT_MIPS_PROTECTED_GOTIDX", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_OPTIONS, new TagInfo { Name = "DT_MIPS_OPTIONS", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_INTERFACE, new TagInfo { Name = "DT_MIPS_INTERFACE", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_DYNSTR_ALIGN, new TagInfo { Name = "DT_MIPS_DYNSTR_ALIGN", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_INTERFACE_SIZE, new TagInfo { Name = "DT_MIPS_INTERFACE_SIZE", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_RLD_TEXT_RESOLVE_ADDR, new TagInfo { Name = "DT_MIPS_RLD_TEXT_RESOLVE_ADDR", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_PERF_SUFFIX, new TagInfo { Name = "DT_MIPS_PERF_SUFFIX", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_COMPACT_SIZE, new TagInfo { Name = "DT_MIPS_COMPACT_SIZE", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_GP_VALUE, new TagInfo { Name = "DT_MIPS_GP_VALUE", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_AUX_DYNAMIC, new TagInfo { Name = "DT_MIPS_AUX_DYNAMIC", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_PLTGOT, new TagInfo { Name = "DT_MIPS_PLTGOT", Format = DtFormat.Address } },
                    { Mips.DT_MIPS_RWPLT, new TagInfo { Name = "DT_MIPS_RWPLT", Format = DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_RLD_MAP_REL, new TagInfo { Name = "DT_MIPS_RLD_MAP_REL", Format = DtFormat.Hexadecimal } },
                }
            }
        };

        public static class Mips
        {
            // Mips specific dynamic table entry tags.
            public const int DT_MIPS_RLD_VERSION = 0x70000001;              // 32 bit version number for runtime linker interface.
            public const int DT_MIPS_TIME_STAMP = 0x70000002;               // Time stamp.
            public const int DT_MIPS_ICHECKSUM = 0x70000003;                // Checksum of external strings and common sizes.
            public const int DT_MIPS_IVERSION = 0x70000004;                 // Index of version string in string table.
            public const int DT_MIPS_FLAGS = 0x70000005;                    // 32 bits of flags.
            public const int DT_MIPS_BASE_ADDRESS = 0x70000006;             // Base address of the segment.
            public const int DT_MIPS_MSYM = 0x70000007;                     // Address of .msym section.
            public const int DT_MIPS_CONFLICT = 0x70000008;                 // Address of .conflict section.
            public const int DT_MIPS_LIBLIST = 0x70000009;                  // Address of .liblist section.
            public const int DT_MIPS_LOCAL_GOTNO = 0x7000000a;              // Number of local global offset table entries.
            public const int DT_MIPS_CONFLICTNO = 0x7000000b;               // Number of entries in the .conflict section.
            public const int DT_MIPS_LIBLISTNO = 0x70000010;                // Number of entries in the .liblist section.
            public const int DT_MIPS_SYMTABNO = 0x70000011;                 // Number of entries in the .dynsym section.
            public const int DT_MIPS_UNREFEXTNO = 0x70000012;               // Index of first external dynamic symbol not referenced locally.
            public const int DT_MIPS_GOTSYM = 0x70000013;                   // Index of first dynamic symbol in global offset table.
            public const int DT_MIPS_HIPAGENO = 0x70000014;                 // Number of page table entries in global offset table.
            public const int DT_MIPS_RLD_MAP = 0x70000016;                  // Address of run time loader map; used for debugging.
            public const int DT_MIPS_DELTA_CLASS = 0x70000017;              // Delta C++ class definition.
            public const int DT_MIPS_DELTA_CLASS_NO = 0x70000018;           // Number of entries in        public const int DT_MIPS_DELTA_CLASS.
            public const int DT_MIPS_DELTA_INSTANCE = 0x70000019;           // Delta C++ class instances.
            public const int DT_MIPS_DELTA_INSTANCE_NO = 0x7000001A;        // Number of entries in        public const int DT_MIPS_DELTA_INSTANCE.
            public const int DT_MIPS_DELTA_RELOC = 0x7000001B;              // Delta relocations.
            public const int DT_MIPS_DELTA_RELOC_NO = 0x7000001C;           // Number of entries in        public const int DT_MIPS_DELTA_RELOC.
            public const int DT_MIPS_DELTA_SYM = 0x7000001D;                // Delta symbols that Delta relocations refer to.
            public const int DT_MIPS_DELTA_SYM_NO = 0x7000001E;             // Number of entries in        public const int DT_MIPS_DELTA_SYM.
            public const int DT_MIPS_DELTA_CLASSSYM = 0x70000020;           // Delta symbols that hold class declarations.
            public const int DT_MIPS_DELTA_CLASSSYM_NO = 0x70000021;        // Number of entries in        public const int DT_MIPS_DELTA_CLASSSYM.
            public const int DT_MIPS_CXX_FLAGS = 0x70000022;                // Flags indicating information about C++ flavor.
            public const int DT_MIPS_PIXIE_INIT = 0x70000023;               // Pixie information.
            public const int DT_MIPS_SYMBOL_LIB = 0x70000024;               // Address of .MIPS.symlib
            public const int DT_MIPS_LOCALPAGE_GOTIDX = 0x70000025;         // The GOT index of the first PTE for a segment
            public const int DT_MIPS_LOCAL_GOTIDX = 0x70000026;             // The GOT index of the first PTE for a local symbol
            public const int DT_MIPS_HIDDEN_GOTIDX = 0x70000027;            // The GOT index of the first PTE for a hidden symbol
            public const int DT_MIPS_PROTECTED_GOTIDX = 0x70000028;         // The GOT index of the first PTE for a protected symbol
            public const int DT_MIPS_OPTIONS = 0x70000029;                  // Address of `.MIPS.options'.
            public const int DT_MIPS_INTERFACE = 0x7000002A;                // Address of `.interface'.
            public const int DT_MIPS_DYNSTR_ALIGN = 0x7000002B;             // Unknown.
            public const int DT_MIPS_INTERFACE_SIZE = 0x7000002C;           // Size of the .interface section.
            public const int DT_MIPS_RLD_TEXT_RESOLVE_ADDR = 0x7000002D;    // Size of rld_text_resolve function stored in the GOT.
            public const int DT_MIPS_PERF_SUFFIX = 0x7000002E;              // Default suffix of DSO to be added by rld on dlopen() calls.
            public const int DT_MIPS_COMPACT_SIZE = 0x7000002F;             // Size of compact relocation section (O32).
            public const int DT_MIPS_GP_VALUE = 0x70000030;                 // GP value for auxiliary GOTs.
            public const int DT_MIPS_AUX_DYNAMIC = 0x70000031;              // Address of auxiliary .dynamic.
            public const int DT_MIPS_PLTGOT = 0x70000032;                   // Address of the base of the PLTGOT.
            public const int DT_MIPS_RWPLT = 0x70000034;                    // Points to the base of a writable PLT.
            public const int DT_MIPS_RLD_MAP_REL = 0x70000035;              // Relative offset of run time loader map, used for debugging.
        }
    }
}
