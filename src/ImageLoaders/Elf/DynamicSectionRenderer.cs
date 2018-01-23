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

using Reko.Core;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{

    public abstract class DynamicSectionRenderer : ImageSegmentRenderer
    {
        //            Executable Shared object
        public const int DT_NULL = 0; // ignored    mandatory  mandatory
        public const int DT_NEEDED = 1; // d_val    optional optional
        public const int DT_PLTRELSZ = 2; // d_val  optional optional
        public const int DT_PLTGOT = 3; // d_ptr optional   optional
        public const int DT_HASH = 4; // d_ptr mandatory mandatory
        public const int DT_STRTAB = 5; // d_ptr mandatory mandatory
        public const int DT_SYMTAB = 6; // d_ptr mandatory mandatory
        public const int DT_RELA = 7; //    d_ptr mandatory optional
        public const int DT_RELASZ = 8; // d_val mandatory optional
        public const int DT_RELAENT = 9; // d_val mandatory optional
        public const int DT_STRSZ = 10; // d_val mandatory mandatory
        public const int DT_SYMENT = 11; // d_val mandatory mandatory
        public const int DT_INIT = 12; // d_ptr optional optional
        public const int DT_FINI = 13; // d_ptr optional optional
        public const int DT_SONAME = 14; // d_val ignored optional
        public const int DT_RPATH = 15; // d_val optional ignored
        public const int DT_SYMBOLIC = 16; // ignored ignored optional
        public const int DT_REL = 17;      // d_ptr mandatory optional
        public const int DT_RELSZ = 18;// d_val     mandatory optional
        public const int DT_RELENT = 19; // d_val   mandatory optional
        public const int DT_PLTREL = 20;    // d_val ignored ignored
        public const int DT_DEBUG = 21;
        public const int DT_JMPREL = 23;
        public const int DT_INIT_ARRAY = 25; // d_ptr 	O 	O
        public const int DT_FINI_ARRAY = 26; // d_ptr 	O 	O
        public const int DT_INIT_ARRAYSZ = 27; // d_val 	O 	O
        public const int DT_FINI_ARRAYSZ = 28; // d_val 	O 	O 

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

        protected static Dictionary<long, Entry> entries = new Dictionary<long, Entry>
        {
            { DT_DEBUG,   new Entry { Name = "DT_DEBUG", Format= DtFormat.Hexadecimal } },
            { DT_FINI,    new Entry { Name="DT_DEBUG", Format = DtFormat.Address } },
            { DT_HASH,    new Entry { Name = "DT_HASH", Format = DtFormat.Address} },
            { DT_INIT,    new Entry { Name = "DT_INIT", Format = DtFormat.Address} },
            { DT_RELA,    new Entry { Name = "DT_RELA", Format = DtFormat.Address} },
            { DT_RELASZ,  new Entry { Name = "DT_RELASZ", Format = DtFormat.Decimal } },
            { DT_RELAENT, new Entry { Name = "DT_RELAENT", Format = DtFormat.Decimal } },
            { DT_PLTGOT,  new Entry { Name = "DT_PLTGOT", Format = DtFormat.Address} },
            { DT_PLTREL,  new Entry { Name = "DT_PLTREL", Format = DtFormat.Hexadecimal } },
            { DT_PLTRELSZ, new Entry { Name = "DT_PLTRELSZ", Format = DtFormat.Decimal } },

            { DT_REL,     new Entry { Name="DT_REL", Format = DtFormat.Address } },
            { DT_RELSZ,   new Entry { Name="DT_RELSZ", Format = DtFormat.Decimal } },
            { DT_RELENT,  new Entry { Name="DT_RELENT", Format = DtFormat.Decimal } },

            { DT_JMPREL,  new Entry { Name = "DT_JMPREL", Format = DtFormat.Address} },
            { DT_NEEDED,  new Entry { Name ="DT_NEEDED",  Format = DtFormat.String } },
            { DT_STRSZ,   new Entry { Name = "DT_STRSZ", Format= DtFormat.Hexadecimal } },
            { DT_STRTAB,  new Entry { Name = "DT_STRTAB", Format = DtFormat.Address} },
            { DT_SYMENT,  new Entry { Name = "DT_SYMENT", Format = DtFormat.Decimal } },
            { DT_SYMTAB,  new Entry { Name = "DT_SYMTAB", Format = DtFormat.Address} },
            { DT_INIT_ARRAY,  new Entry { Name = "DT_INIT_ARRAY", Format = DtFormat.Address} },
            { DT_FINI_ARRAY,  new Entry { Name = "DT_FINI_ARRAY", Format = DtFormat.Address} },
            { DT_INIT_ARRAYSZ,  new Entry { Name = "DT_INIT_ARRAYSZ", Format = DtFormat.Hexadecimal} },
            { DT_FINI_ARRAYSZ,  new Entry { Name = "DT_FINI_ARRAYSZ", Format = DtFormat.Hexadecimal} },
        };

        protected static Dictionary<ElfMachine, Dictionary<long, Entry>> machineSpecificEntries = new Dictionary<ElfMachine, Dictionary<long, Entry>>
        {
            {
                ElfMachine.EM_MIPS,
                new Dictionary<long, Entry>
                {
                    { Mips.DT_MIPS_GOTSYM, new Entry { Name = "DT_MIPS_GOTSYM", Format = DtFormat.Decimal } },
                    { Mips.DT_MIPS_RLD_MAP, new Entry { Name = "DT_MIPS_RLD_MAP", Format= DtFormat.Address } },
                    { Mips.DT_MIPS_RLD_VERSION, new Entry { Name = "DT_MIPS_RLD_VERSION", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_FLAGS, new Entry { Name = "DT_MIPS_FLAGS", Format= DtFormat.Hexadecimal } },
                    { Mips.DT_MIPS_BASE_ADDRESS, new Entry { Name = "DT_MIPS_BASE_ADDRESS", Format= DtFormat.Address } },
                    { Mips.DT_MIPS_LOCAL_GOTNO, new Entry { Name = "DT_MIPS_LOCAL_GOTNO", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_SYMTABNO, new Entry { Name = "DT_MIPS_SYMTABNO", Format= DtFormat.Decimal } },
                    { Mips.DT_MIPS_UNREFEXTNO, new Entry { Name = "DT_MIPS_UNREFEXTNO", Format= DtFormat.Decimal } },
                }
            }
        };

        public class Entry
        {
            public string Name;
            public DtFormat Format;
        }
    }

    

    public enum DtFormat
    {
        None,
        Address,
        String,
        Decimal,
        Hexadecimal,
    }

    public class DynamicSectionRenderer32 : DynamicSectionRenderer
    {
        private ElfLoader32 loader;
        private ElfSection shdr;
        private ElfSection strtabSection;
        private Dictionary<long, Entry> machineSpecific;

        public DynamicSectionRenderer32(ElfLoader32 loader, ElfSection shdr, ElfMachine machine)
        {
            this.loader = loader;
            this.shdr = shdr;
            if (!machineSpecificEntries.TryGetValue(machine, out machineSpecific))
            {
                machineSpecific = new Dictionary<long, Entry>();
            }
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            // Get the entry that has the segment# for the string table.
            var dynStrtab = loader.GetDynEntries(shdr.FileOffset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
            if (dynStrtab == null)
                return;
            this.strtabSection = loader.GetSectionInfoByAddr(dynStrtab.d_ptr);
            foreach (var entry in loader.GetDynEntries(shdr.FileOffset))
            {
                DtFormat fmt;
                string entryName;
                Entry dser;
                if (!machineSpecific.TryGetValue(entry.d_tag, out dser) &&
                    !entries.TryGetValue(entry.d_tag, out dser))
                {
                    entryName = string.Format("{0:X8}    ", entry.d_tag);
                    fmt = DtFormat.Hexadecimal;
                }
                else
                {
                    entryName = dser.Name;
                    fmt = dser.Format;
                }
                RenderEntry(entryName, fmt, entry, formatter);
                formatter.WriteLine();
            }
        }

        protected virtual void RenderEntry(string name, DtFormat format, Elf32_Dyn entry, Formatter formatter)
        {
            formatter.Write("{0,-20} ", name);
            switch (format)
            {
            default:
            case DtFormat.Hexadecimal:
                formatter.Write("{0:X8}", entry.d_val);
                break;
            case DtFormat.Decimal:
                formatter.Write("{0,8}", entry.d_val);
                break;
            case DtFormat.Address:
                formatter.WriteHyperlink(string.Format("{0:X8}", entry.d_ptr), Address.Ptr32(entry.d_ptr));
                break;
            case DtFormat.String:
                formatter.Write(loader.ReadAsciiString(strtabSection.FileOffset + entry.d_ptr));
                break;
            }
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

    public class DynamicSectionRenderer64 : DynamicSectionRenderer
    {
        private ElfLoader64 loader;
        private ElfSection shdr;
        private ElfSection strtabSection;

        public DynamicSectionRenderer64(ElfLoader64 loader, ElfSection shdr)
        {
            this.loader = loader;
            this.shdr = shdr;
        }

        public override void Render(ImageSegment segment, Program program, Formatter formatter)
        {
            // Get the entry that has the segment# for the string table.
            var dynStrtab = loader.GetDynEntries64(shdr.FileOffset).Where(d => d.d_tag == DT_STRTAB).FirstOrDefault();
            if (dynStrtab == null)
                return;
            this.strtabSection = loader.GetSectionInfoByAddr64(dynStrtab.d_ptr);
            foreach (var entry in loader.GetDynEntries64(shdr.FileOffset))
            {
                DtFormat fmt;
                string entryName;
                Entry dser;
                if (!entries.TryGetValue(entry.d_tag, out dser))
                {
                    entryName = string.Format("{0:X8}    ", entry.d_tag);
                    fmt = DtFormat.Hexadecimal;
                }
                else
                {
                    entryName = dser.Name;
                    fmt = dser.Format;
                }
                RenderEntry(entryName, fmt, entry, formatter);
                formatter.WriteLine();
            }
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


        protected virtual void RenderEntry(string name, DtFormat format, Elf64_Dyn entry, Formatter formatter)
        {
            formatter.Write("{0,-15} ", name);
            switch (format)
            {
            default:
            case DtFormat.Hexadecimal:
                formatter.Write("{0:X16}", entry.d_val);
                break;
            case DtFormat.Decimal:
                formatter.Write("{0,16}", entry.d_val);
                break;
            case DtFormat.Address:
                formatter.WriteHyperlink(string.Format("{0:X16}", entry.d_ptr), Address.Ptr64(entry.d_ptr));
                break;
            case DtFormat.String:
                formatter.Write(loader.ReadAsciiString(strtabSection.FileOffset + entry.d_ptr));
                break;
            }
        }
    }
}