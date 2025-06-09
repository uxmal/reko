#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.MachO
{
    // http://newosxbook.com/articles/DYLD.html
    // http://www.m4b.io/reverse/engineering/mach/binaries/2015/03/29/mach-binaries.html
    public class MachOLoader : ProgramImageLoader
    {
        const uint MH_MAGIC = 0xFEEDFACE;
        const uint MH_MAGIC_64 = 0xFEEDFACF;
        const uint MH_MAGIC_32_LE = 0xCEFAEDFE; // 0xFEEDFACE;
        const uint MH_MAGIC_64_LE = 0xCFFAEDFE; // 0xFEEDFACF;

        internal static readonly TraceSwitch trace = new TraceSwitch(nameof(MachOLoader), "Trace loading of MachO binaries") { Level = TraceLevel.Warning };

        private Parser? parser;
        internal List<MachOSection> sections;
        internal Dictionary<string, MachOSection> sectionsByName;
        internal Dictionary<MachOSection, ImageSegment> imageSections;
        internal List<MachOSymbol> machoSymbols;
        internal SortedList<Address, ImageSymbol> imageSymbols;
        internal List<ImageSymbol> entryPoints;
        internal Program? program;

        public MachOLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawImg)
            : base(services, imageLocation, rawImg)
        {
            this.sections = new List<MachOSection>();
            this.sectionsByName = new Dictionary<string, MachOSection>();
            this.imageSections = new Dictionary<MachOSection, ImageSegment>();
            this.imageSymbols = new SortedList<Address, ImageSymbol>();
            this.machoSymbols = new List<MachOSymbol>();
            this.entryPoints = new List<ImageSymbol>();
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr64(0x0100000); }
            set { throw new NotImplementedException(); }
        }

        public override Program LoadProgram(Address? a)
        {
            var addrLoad = a ?? PreferredBaseAddress;
            this.program = new Program();
            parser = CreateParser();
            var (hdr, specific) = parser.ParseHeader(addrLoad);
            this.program = parser.ParseLoadCommands(hdr, specific.Architecture, addrLoad);

            CollectSymbolStubs(parser, machoSymbols, imageSymbols);
            foreach (var de in imageSymbols)
            {
                if (program.SegmentMap.IsValidAddress(de.Key))
                    program.ImageSymbols.Add(de.Key, de.Value);
            }
            foreach (var ep in entryPoints)
            {
                program.EntryPoints[ep.Address] = ep;
            }
            return this.program;
        }

        public Parser CreateParser()
        {
            if (!ByteMemoryArea.TryReadBeUInt32(RawImage, 0, out uint magic))
                throw new BadImageFormatException("Invalid Mach-O header.");
            switch (magic)
            {
            case MH_MAGIC: return new Loader32(this, new BeImageReader(new ByteMemoryArea(Address.Ptr32(0), RawImage), 0));
            case MH_MAGIC_64: return new Loader64(this, new BeImageReader(new ByteMemoryArea(Address.Ptr32(0), RawImage), 0));
            case MH_MAGIC_32_LE: return new Loader32(this, new LeImageReader(new ByteMemoryArea(Address.Ptr32(0), RawImage), 0));
            case MH_MAGIC_64_LE: return new Loader64(this, new LeImageReader(new ByteMemoryArea(Address.Ptr32(0), RawImage), 0));
            }
            throw new BadImageFormatException("Invalid Mach-O header.");
        }

        /// <summary>
        /// Find the symbol stubs and add them to the <paramref name="imageSymbols"/> collection.
        /// </summary>
        /// <param name="machoSymbols"></param>
        /// <param name="imageSymbols"></param>
        private void CollectSymbolStubs(
            Parser parser,
            List<MachOSymbol> machoSymbols,
            SortedList<Address, ImageSymbol> imageSymbols)
        {
            var msec = this.sections.FirstOrDefault(s => (s.Flags & SectionFlags.SECTION_TYPE) == SectionFlags.S_SYMBOL_STUBS);
            if (msec is null)
                return;
            if (parser.dysymtab is null)
                return;
            var indirectSymRdr = program!.Architecture.Endianness.CreateImageReader(RawImage, parser.dysymtab.indirectsymoff);
            var sec = this.imageSections[msec];
            trace.Inform("MachO: Found {0} import stubs", sec.Size / msec.Reserved2);
            for (uint off = 0; off < sec.Size; off += msec.Reserved2)
            {
                var addrStub = sec.Address + off;
                if (!indirectSymRdr.TryReadInt32(out int isym))
                    break;
                var sym = machoSymbols[isym];
                trace.Verbose("   Stub at {0}: {1:X8}", addrStub, sym.Name);
                var stubSym = ImageSymbol.ExternalProcedure(program.Architecture, addrStub, sym.Name);
                imageSymbols.Add(addrStub, stubSym);
            }
        }

        public enum RelocationInfoType
        {
            GENERIC_RELOC_VANILLA = 0,
            GENERIC_RELOC_PAIR = 1,
            GENERIC_RELOC_SECTDIFF = 2,
            GENERIC_RELOC_PB_LA_PTR = 3,
            GENERIC_RELOC_LOCAL_SECTDIFF = 4,
            GENERIC_RELOC_TLV = 5,

            PPC_RELOC_VANILLA = GENERIC_RELOC_VANILLA,
            PPC_RELOC_PAIR = GENERIC_RELOC_PAIR,
            PPC_RELOC_BR14 = 2,
            PPC_RELOC_BR24 = 3,
            PPC_RELOC_HI16 = 4,
            PPC_RELOC_LO16 = 5,
            PPC_RELOC_HA16 = 6, PPC_RELOC_LO14 = 7,
            PPC_RELOC_SECTDIFF = 8,
            PPC_RELOC_PB_LA_PTR = 9,
            PPC_RELOC_HI16_SECTDIFF = 10,
            PPC_RELOC_LO16_SECTDIFF = 11,
            PPC_RELOC_HA16_SECTDIFF = 12,
            PPC_RELOC_JBSR = 13,
            PPC_RELOC_LO14_SECTDIFF = 14,
            PPC_RELOC_LOCAL_SECTDIFF = 15,

            ARM_RELOC_VANILLA = GENERIC_RELOC_VANILLA,
            ARM_RELOC_PAIR = GENERIC_RELOC_PAIR,
            ARM_RELOC_SECTDIFF = GENERIC_RELOC_SECTDIFF,
            ARM_RELOC_LOCAL_SECTDIFF = 3,
            ARM_RELOC_PB_LA_PTR = 4,
            ARM_RELOC_BR24 = 5,
            ARM_THUMB_RELOC_BR22 = 6,
            ARM_THUMB_32BIT_BRANCH = 7,
            ARM_RELOC_HALF = 8,
            ARM_RELOC_HALF_SECTDIFF = 9,

            ARM64_RELOC_UNSIGNED = 0,
            ARM64_RELOC_SUBTRACTOR = 1,
            ARM64_RELOC_BRANCH26 = 2,
            ARM64_RELOC_PAGE21 = 3,
            ARM64_RELOC_PAGEOFF12 = 4,
            ARM64_RELOC_GOT_LOAD_PAGE21 = 5,
            ARM64_RELOC_GOT_LOAD_PAGEOFF12 = 6,
            ARM64_RELOC_POINTER_TO_GOT = 7,
            ARM64_RELOC_TLVP_LOAD_PAGE21 = 8,
            ARM64_RELOC_TLVP_LOAD_PAGEOFF12 = 9,
            ARM64_RELOC_ADDEND = 10,

            X86_64_RELOC_UNSIGNED = 0,
            X86_64_RELOC_SIGNED = 1,
            X86_64_RELOC_BRANCH = 2,
            X86_64_RELOC_GOT_LOAD = 3,
            X86_64_RELOC_GOT = 4,
            X86_64_RELOC_SUBTRACTOR = 5,
            X86_64_RELOC_SIGNED_1 = 6,
            X86_64_RELOC_SIGNED_2 = 7,
            X86_64_RELOC_SIGNED_4 = 8,
            X86_64_RELOC_TLV = 9
        }


#if NYI

 const uint SECTION_TYPE = 0x000000ff;
 const uint SECTION_ATTRIBUTES = 0xffffff00;
 const uint S_REGULAR = 0;
 const uint S_ZEROFILL = 1;
 const uint S_ATTR_SOME_INSTRUCTIONS = 0x00000400;
 const uint S_ATTR_PURE_INSTRUCTIONS = 0x80000000;


static const byte N_TYPE = 0x0e;
static const byte N_UNDF = 0x0;
static const byte N_ABS = 0x2;
static const byte N_SECT = 0xe;
static const byte N_INDR = 0xa;

static const byte NO_SECT = 0;
#endif

    }
    /// <summary>
    /// Contains MachO specific information that is only used at load time
    /// and which doesn't fit into the ImageSegment class (which is generic)
    /// </summary>
    public class MachOSection
    {
        public readonly string Name;
        public readonly Address Address;
        public readonly SectionFlags Flags;
        public readonly uint Reserved1;
        public readonly uint Reserved2;

        public MachOSection(string name, Address addr, SectionFlags flags, uint reserved1, uint reserved2)
        {
            this.Name = name;
            this.Address = addr;
            this.Flags = flags;
            this.Reserved1 = reserved1;
            this.Reserved2 = reserved2;
        }
    }

    public class MachOSymbol
    {
        public string Name;
        public byte n_type;
        public byte n_sect;
        public ushort n_desc;
        public ulong n_value;

        public MachOSymbol(string name, byte n_type, byte n_sect, ushort n_desc, ulong n_value)
        {
            this.Name = name;
            this.n_type = n_type;
            this.n_sect = n_sect;
            this.n_desc = n_desc;
            this.n_value = n_value;
        }
    }

    [Flags]
    public enum SectionFlags : UInt32
    {
        SECTION_TYPE = 0xFF,

        S_REGULAR = 0x00u,                              // S_REGULAR - Regular section.
        S_ZEROFILL = 0x01u,                             // Zero fill on demand section.
        S_CSTRING_LITERALS = 0x02u,                     // Section with literal C strings.
        S_4BYTE_LITERALS = 0x03u,                       // Section with 4 byte literals.
        S_8BYTE_LITERALS = 0x04u,                       // Section with 8 byte literals.
        S_LITERAL_POINTERS = 0x05u,                     // Section with pointers to literals.
        S_NON_LAZY_SYMBOL_POINTERS = 0x06u,             // Section with non-lazy symbol pointers.
        S_LAZY_SYMBOL_POINTERS = 0x07u,                 // Section with lazy symbol pointers.
        S_SYMBOL_STUBS = 0x08u,                         // Section with symbol stubs, byte size of stub in the Reserved2 field.
        S_MOD_INIT_FUNC_POINTERS = 0x09u,               // Section with only function pointers for initialization.
        S_MOD_TERM_FUNC_POINTERS = 0x0au,               // Section with only function pointers for termination.
        S_COALESCED = 0x0bu,                            // Section contains symbols that are to be coalesced.
        S_GB_ZEROFILL = 0x0cu,                          // Zero fill on demand section (that can be larger than 4 gigabytes).
        S_INTERPOSING = 0x0du,                          // Section with only pairs of function pointers for interposing.
        S_16BYTE_LITERALS = 0x0eu,                      // Section with only 16 byte literals.
        S_DTRACE_DOF = 0x0fu,                           // Section contains DTrace Object Format.
        S_LAZY_DYLIB_SYMBOL_POINTERS = 0x10u,           // Section with lazy symbol pointers to lazy loaded dylibs.
        S_THREAD_LOCAL_REGULAR = 0x11u,                 // Thread local data section.
        S_THREAD_LOCAL_ZEROFILL = 0x12u,                // Thread local zerofill section.
        S_THREAD_LOCAL_VARIABLES = 0x13u,               // Section with thread local variable structure data.
        S_THREAD_LOCAL_VARIABLE_POINTERS = 0x14u,       // Section with pointers to thread local structures.
        S_THREAD_LOCAL_INIT_FUNCTION_POINTERS = 0x15u,  // Section with thread local variable initialization pointers to functions.

        LAST_KNOWN_SECTION_TYPE = S_THREAD_LOCAL_INIT_FUNCTION_POINTERS
    }
}