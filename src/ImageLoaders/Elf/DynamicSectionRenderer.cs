#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        public const int DT_NULL = 0; // ignored mandatory mandatory
        public const int DT_NEEDED = 1; // d_val   optional optional
        public const int DT_PLTRELSZ = 2; // d_val optional optional
        public const int DT_PLTGOT = 3; // d_ptr optional optional
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
        public const int DT_PLTREL = 20;    // d_val ignored ignored
        public const int DT_DEBUG = 21;
        public const int DT_JMPREL = 23;
        public const int DT_INIT_ARRAY = 25; // d_ptr 	O 	O
        public const int DT_FINI_ARRAY = 26; // d_ptr 	O 	O
        public const int DT_INIT_ARRAYSZ = 27; // d_val 	O 	O
        public const int DT_FINI_ARRAYSZ = 28; // d_val 	O 	O 

        protected static Dictionary<long, DynamicSectionEntryRenderer> entries = new Dictionary<long, DynamicSectionEntryRenderer>
        {
            { DT_DEBUG,   new DynamicSectionEntryRenderer { Name = "DT_DEBUG", Format= DtFormat.Hexadecimal } },
            { DT_FINI,    new DynamicSectionEntryRenderer { Name="DT_DEBUG", Format = DtFormat.Address } },
            { DT_HASH,    new DynamicSectionEntryRenderer { Name = "DT_HASH", Format = DtFormat.Address} },
            { DT_INIT,    new DynamicSectionEntryRenderer { Name = "DT_INIT", Format = DtFormat.Address} },
            { DT_RELA,    new DynamicSectionEntryRenderer { Name = "DT_RELA", Format = DtFormat.Address} },
            { DT_RELASZ,  new DynamicSectionEntryRenderer { Name = "DT_RELASZ", Format = DtFormat.Decimal } },
            { DT_RELAENT, new DynamicSectionEntryRenderer { Name = "DT_RELAENT", Format = DtFormat.Decimal } },
            { DT_PLTGOT,  new DynamicSectionEntryRenderer { Name = "DT_PLTGOT", Format = DtFormat.Address} },
            { DT_PLTREL,  new DynamicSectionEntryRenderer { Name = "DT_PLTREL", Format = DtFormat.Hexadecimal } },
            { DT_PLTRELSZ, new DynamicSectionEntryRenderer { Name = "DT_PLTRELSZ", Format = DtFormat.Decimal } },
            { DT_JMPREL,  new DynamicSectionEntryRenderer { Name = "DT_JMPREL", Format = DtFormat.Address} },
            { DT_NEEDED,  new DynamicSectionEntryRenderer { Name ="DT_NEEDED",  Format = DtFormat.String } },
            { DT_STRSZ,   new DynamicSectionEntryRenderer { Name = "DT_STRSZ", Format= DtFormat.Hexadecimal } },
            { DT_STRTAB,  new DynamicSectionEntryRenderer { Name = "DT_STRTAB", Format = DtFormat.Address} },
            { DT_SYMENT,  new DynamicSectionEntryRenderer { Name = "DT_SYMENT", Format = DtFormat.Decimal } },
            { DT_SYMTAB,  new DynamicSectionEntryRenderer { Name = "DT_SYMTAB", Format = DtFormat.Address} },
            { DT_INIT_ARRAY,  new DynamicSectionEntryRenderer { Name = "DT_INIT_ARRAY", Format = DtFormat.Address} },
            { DT_FINI_ARRAY,  new DynamicSectionEntryRenderer { Name = "DT_FINI_ARRAY", Format = DtFormat.Address} },
            { DT_INIT_ARRAYSZ,  new DynamicSectionEntryRenderer { Name = "DT_INIT_ARRAYSZ", Format = DtFormat.Hexadecimal} },
            { DT_FINI_ARRAYSZ,  new DynamicSectionEntryRenderer { Name = "DT_FINI_ARRAYSZ", Format = DtFormat.Hexadecimal} },
        };
    }

    public class DynamicSectionEntryRenderer
    {
        public string Name;
        public DtFormat Format;
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

        public DynamicSectionRenderer32(ElfLoader32 loader, ElfSection shdr)
        {
            this.loader = loader;
            this.shdr = shdr;
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
                DynamicSectionEntryRenderer dser;
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

        protected virtual void RenderEntry(string name, DtFormat format, Elf32_Dyn entry, Formatter formatter)
        {
            formatter.Write("{0,-15} ", name);
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
                DynamicSectionEntryRenderer dser;
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
