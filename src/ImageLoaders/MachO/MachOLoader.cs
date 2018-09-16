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
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reko.ImageLoaders.MachO
{
    public class MachOLoader : ImageLoader
    {
        const uint MH_MAGIC = 0xfeedface;
        const uint MH_MAGIC_64 = 0xfeedfacf;
        const uint MH_MAGIC_32_LE = 0xCEFAEDFE; //0xfeedface;
        const uint MH_MAGIC_64_LE = 0xCFFAEDFE; // 0xfeedfacf;

        // http://www.opensource.apple.com/source/xnu/xnu-792.13.8/osfmk/mach/machine.h

        public const uint CPU_ARCH_MASK	=0xff000000;		/* mask for architecture bits */
        public const uint CPU_ARCH_ABI64 = 0x01000000;		/* 64 bit ABI */

        public const uint CPU_TYPE_ANY		= 0xFFFFFFFF;
        public const uint CPU_TYPE_VAX	= 1;
        public const uint CPU_TYPE_MC680x0	= 6;
        public const uint CPU_TYPE_X86		= 7;
        public const uint CPU_TYPE_I386	=	CPU_TYPE_X86;
        public const uint CPU_TYPE_X86_64	=	(CPU_TYPE_X86 | CPU_ARCH_ABI64);
        public const uint CPU_TYPE_MIPS	=	8;
        public const uint CPU_TYPE_MC98000	 = 11;
        public const uint CPU_TYPE_ARM		= 12;
        public const uint CPU_TYPE_MC88000	= 13;
        public const uint CPU_TYPE_SPARC	= 14;
        public const uint CPU_TYPE_I860	= 15;
        public const uint CPU_TYPE_ALPHA	=16;
        public const uint CPU_TYPE_POWERPC	= 18;
        public const uint CPU_TYPE_POWERPC64 = (CPU_TYPE_POWERPC | CPU_ARCH_ABI64);

        public const uint CPU_SUBTYPE_MC68030 = 1;
        public const uint CPU_SUBTYPE_MC68040 = 2;
        
        private Parser ldr;

        public MachOLoader(IServiceProvider services, string filename, byte[] rawImg)
            : base(services, filename, rawImg)
        {

        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr64(0x0100000); } 
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            ldr = CreateParser();
            uint ncmds = ldr.ParseHeader(addrLoad);
            SegmentMap segmentMap = ldr.ParseLoadCommands(ncmds, addrLoad);
            var image = new MemoryArea(addrLoad, RawImage);
            return new Program(
                segmentMap,
                ldr.arch,
                new DefaultPlatform(Services, ldr.arch));
        }

        public class mach_header_32
        {
            public uint magic;
            public uint cputype;
            public uint cpusubtype;
            public uint filetype;
            public uint ncmds;
            public uint sizeofcmds;
            public uint flags;
        }

        public class mach_header_64
        {
            public uint magic;
            public uint cputype;
            public uint cpusubtype;
            public uint filetype;
            public uint ncmds;
            public uint sizeofcmds;
            public uint flags;
            public uint reserved;
        }

        public Parser CreateParser()
        {
            uint magic;
            if (!MemoryArea.TryReadBeUInt32(RawImage, 0, out magic))
                throw new BadImageFormatException("Invalid Mach-O header.");
            switch (magic)
            {
            case MH_MAGIC:
                return new Loader32(this, new BeImageReader(RawImage, 0));
            case MH_MAGIC_64:
                return new Loader64(this, new BeImageReader(RawImage, 0));
            case MH_MAGIC_32_LE:
                return new Loader32(this, new LeImageReader(RawImage, 0));
            case MH_MAGIC_64_LE:
                return new Loader64(this, new LeImageReader(RawImage, 0));
            }
            throw new BadImageFormatException("Invalid Mach-O header.");
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }

        public abstract class Parser
        {
            private MachOLoader ldr;
            protected EndianImageReader rdr;
            public IProcessorArchitecture arch;

            protected Parser(MachOLoader ldr, EndianImageReader rdr)
            {
                this.ldr = ldr;
                this.rdr = rdr;
            }

            public IProcessorArchitecture CreateArchitecture(uint cputype)
            {
                var cfgSvc = ldr.Services.RequireService<IConfigurationService>();
                string arch;
                switch (cputype)
                {
                case CPU_TYPE_ARM: arch = "arm"; break;
                case CPU_TYPE_I386: arch = "x86-protected-32"; break;
                case CPU_TYPE_X86_64: arch = "x86-protected-64"; break;
                case CPU_TYPE_MIPS: arch = "mips"; break;
                case CPU_TYPE_POWERPC: arch = "ppc-be-32"; break;
                default:
                    throw new NotSupportedException(string.Format("Processor format {0} is not supported.", cputype));
                }
                return cfgSvc.GetArchitecture(arch);
            }

            /// <summary>
            /// Parse the Mach-O image header
            /// </summary>
            /// <param name="addrLoad"></param>
            /// <returns>The number of load commands in the header.</returns>
            public abstract uint ParseHeader(Address addrLoad);

            const uint LC_REQ_DYLD = 0x80000000;

            // http://llvm.org/docs/doxygen/html/Support_2MachO_8h_source.html
            // http://opensource.apple.com//source/clang/clang-503.0.38/src/tools/macho-dump/macho-dump.cpp
            public const uint LC_SEGMENT              = 0x00000001u;
            public const uint LC_SYMTAB               = 0x00000002u;
            public const uint LC_SYMSEG               = 0x00000003u;
            public const uint LC_THREAD               = 0x00000004u;
            public const uint LC_UNIXTHREAD           = 0x00000005u;
            public const uint LC_LOADFVMLIB           = 0x00000006u;
            public const uint LC_IDFVMLIB             = 0x00000007u;
            public const uint LC_IDENT                = 0x00000008u;
            public const uint LC_FVMFILE              = 0x00000009u;
            public const uint LC_PREPAGE              = 0x0000000Au;
            public const uint LC_DYSYMTAB             = 0x0000000Bu;
            public const uint LC_LOAD_DYLIB           = 0x0000000Cu;
            public const uint LC_ID_DYLIB             = 0x0000000Du;
            public const uint LC_LOAD_DYLINKER        = 0x0000000Eu;
            public const uint LC_ID_DYLINKER          = 0x0000000Fu;
            public const uint LC_PREBOUND_DYLIB       = 0x00000010u;
            public const uint LC_ROUTINES             = 0x00000011u;
            public const uint LC_SUB_FRAMEWORK        = 0x00000012u;
            public const uint LC_SUB_UMBRELLA         = 0x00000013u;
            public const uint LC_SUB_CLIENT           = 0x00000014u;
            public const uint LC_SUB_LIBRARY          = 0x00000015u;
            public const uint LC_TWOLEVEL_HINTS       = 0x00000016u;
            public const uint LC_PREBIND_CKSUM        = 0x00000017u;
            public const uint LC_LOAD_WEAK_DYLIB      = 0x80000018u;
            public const uint LC_SEGMENT_64           = 0x00000019u;
            public const uint LC_ROUTINES_64          = 0x0000001Au;
            public const uint LC_UUID                 = 0x0000001Bu;
            public const uint LC_RPATH                = 0x8000001Cu;
            public const uint LC_CODE_SIGNATURE       = 0x0000001Du;
            public const uint LC_SEGMENT_SPLIT_INFO   = 0x0000001Eu;
            public const uint LC_REEXPORT_DYLIB       = 0x8000001Fu;
            public const uint LC_LAZY_LOAD_DYLIB      = 0x00000020u;
            public const uint LC_ENCRYPTION_INFO      = 0x00000021u;
            public const uint LC_DYLD_INFO            = 0x00000022u;
            public const uint LC_DYLD_INFO_ONLY       = 0x80000022u;
            public const uint LC_LOAD_UPWARD_DYLIB    = 0x80000023u;
            public const uint LC_VERSION_MIN_MACOSX   = 0x00000024u;
            public const uint LC_VERSION_MIN_IPHONEOS = 0x00000025u;
            public const uint LC_FUNCTION_STARTS      = 0x00000026u;
            public const uint LC_DYLD_ENVIRONMENT     = 0x00000027u;
            public const uint LC_MAIN                 = 0x80000028u;
            public const uint LC_DATA_IN_CODE         = 0x00000029u;
            public const uint LC_SOURCE_VERSION       = 0x0000002Au;
            public const uint LC_DYLIB_CODE_SIGN_DRS  = 0x0000002Bu;
            public const uint LC_ENCRYPTION_INFO_64   = 0x0000002Cu;
            public const uint LC_LINKER_OPTION        = 0x0000002Du;
            public const uint LC_LINKER_OPTIMIZATION_HINT = 0x0000002Eu;
            public const uint LC_VERSION_MIN_TVOS     = 0x0000002Fu;
            public const uint LC_VERSION_MIN_WATCHOS  = 0x00000030u;

            public SegmentMap ParseLoadCommands(uint ncmds, Address addrLoad)
            {
                var imageMap = new SegmentMap(addrLoad);
                Debug.Print("Parsing load commands, {0} of them.", ncmds);

                var lookup = GetType()
                    .GetFields(
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.StartsWith("LC_"))
                    .ToDictionary(fi => (uint)fi.GetValue(null), fi => fi.Name);

                for (uint i = 0; i < ncmds; ++i)
                {
                    var pos = rdr.Offset;
                    uint cmd;
                    uint cmdsize;
                    if (!rdr.TryReadUInt32(out cmd) ||
                        !rdr.TryReadUInt32(out cmdsize))
                    {
                        throw new BadImageFormatException(string.Format(
                            "Unable to read Mach-O command ({0:X}).",
                            rdr.Offset));
                    }
                    Debug.Print("{0,3}: Read load command 0x{1:X} {2} of size {3}.", i, cmd, lookup.ContainsKey(cmd) ? lookup[cmd] : "", cmdsize);

                    switch (cmd & ~LC_REQ_DYLD)
                    {
                    case LC_SEGMENT:
                        parseSegmentCommand32(imageMap);
                        break;
                    case LC_SEGMENT_64:
                        parseSegmentCommand64(imageMap);
                        break;
                    //case LC_SYMTAB:
                    //    parseSymtabCommand<Mach>();
                    //    break;
                    case LC_FUNCTION_STARTS:
                        parseFunctionStarts(rdr.Clone());
                        break;
                    }
                    rdr.Offset = pos + cmdsize;
                }
                return imageMap;
            }

            private static string ReadSectionName(EndianImageReader rdr, int maxSize)
            {
                byte[] bytes = rdr.ReadBytes(maxSize);
                Encoding asc = Encoding.ASCII;
                char[] chars = asc.GetChars(bytes);
                int i;
                for (i = chars.Length - 1; i >= 0; --i)
                {
                    if (chars[i] != 0)
                    {
                        ++i;
                        break;
                    }
                }
                return new String(chars, 0, i);
            }

            void parseSegmentCommand32(SegmentMap imageMap)
            {
                var segname = ReadSegmentName();
                uint vmaddr;
                uint vmsize;
                uint fileoff;
                uint filesize;
                uint maxprot;
                uint initprot;
                uint nsects;
                uint flags;
                if (!rdr.TryReadUInt32(out vmaddr) ||
                    !rdr.TryReadUInt32(out vmsize) ||
                    !rdr.TryReadUInt32(out fileoff) ||
                    !rdr.TryReadUInt32(out filesize) ||
                    !rdr.TryReadUInt32(out maxprot) ||
                    !rdr.TryReadUInt32(out initprot) ||
                    !rdr.TryReadUInt32(out nsects) ||
                    !rdr.TryReadUInt32(out flags))
                {
                    throw new BadImageFormatException("Could not read segment command.");
                }
                Debug.Print("Found segment '{0}' with {1} sections.", segname, nsects);

                for (uint i = 0; i < nsects; ++i)
                {
                    Debug.Print("Parsing section number {0}.", i);
                    parseSection32(initprot, imageMap);
                }
            }

            void parseSegmentCommand64(SegmentMap imageMap)
            {
                string segname = ReadSegmentName();
                ulong vmaddr;
                ulong vmsize;
                ulong fileoff;
                ulong filesize;
                uint maxprot;
                uint initprot;
                uint nsects;
                uint flags;
                if (!rdr.TryReadUInt64(out vmaddr) ||
                    !rdr.TryReadUInt64(out vmsize) ||
                    !rdr.TryReadUInt64(out fileoff) ||
                    !rdr.TryReadUInt64(out filesize) ||
                    !rdr.TryReadUInt32(out maxprot) ||
                    !rdr.TryReadUInt32(out initprot) ||
                    !rdr.TryReadUInt32(out nsects) ||
                    !rdr.TryReadUInt32(out flags))
                {
                    throw new BadImageFormatException("Could not read segment command.");
                }
                Debug.Print("Found segment '{0}' with {1} sections.", segname, nsects);

                for (uint i = 0; i < nsects; ++i)
                {
                    Debug.Print("Parsing section number {0}.", i);
                    parseSection64(initprot, imageMap);
                }
            }

            private string ReadSegmentName()
            {
                var abSegname = rdr.ReadBytes(16);
                var cChars = Array.IndexOf<byte>(abSegname, 0);
                if (cChars == -1)
                    cChars = 16;
                string segname = Encoding.ASCII.GetString(abSegname, 0, cChars);
                return segname;
            }

            void parseFunctionStarts(EndianImageReader rdr)
            {
                uint dataoff;
                uint datasize;
                if (!rdr.TryReadUInt32(out dataoff) ||
                    !rdr.TryReadUInt32(out datasize))
                {
                    throw new BadImageFormatException("Couldn't read LC_FUNCTIONSTARTS command");
                }
                Debug.Print(" LC_FUNCTIONSTARTS {0:X8} {1:X8}", dataoff, datasize);
                rdr.Offset = dataoff;
                var endoff = dataoff + datasize;
                while (rdr.Offset < endoff)
                {
                    uint fn = rdr.ReadUInt32();
                    Debug.Print("  fn: {0:X}", fn);
                }
            }

            private string GetAsciizString(byte[] bytes)
            {
                int c = Array.IndexOf<byte>(bytes, 0);
                if (c == -1)
                    c = bytes.Length;
                return Encoding.ASCII.GetString(bytes, 0, c);
            }

            const uint VM_PROT_READ = 0x01;
            const uint VM_PROT_WRITE = 0x02;
            const uint VM_PROT_EXECUTE = 0x04;

            void parseSection32(uint protection, SegmentMap segmentMap)
            {
                var abSectname = rdr.ReadBytes(16);
                var abSegname = rdr.ReadBytes(16);

                uint addr;
                uint size;
                uint offset;
                uint align;
                uint reloff;
                uint nreloc;
                uint flags;
                uint reserved1;
                uint reserved2;

                if (!rdr.TryReadUInt32(out addr) ||
                    !rdr.TryReadUInt32(out size) ||
                    !rdr.TryReadUInt32(out offset) ||
                    !rdr.TryReadUInt32(out align) ||
                    !rdr.TryReadUInt32(out reloff) ||
                    !rdr.TryReadUInt32(out nreloc) ||
                    !rdr.TryReadUInt32(out flags) ||
                    !rdr.TryReadUInt32(out reserved1) ||
                    !rdr.TryReadUInt32(out reserved2))
                {
                    throw new BadImageFormatException("Could not read Mach-O section.");
                }

                var sectionName = GetAsciizString(abSectname);
                var segmentName = GetAsciizString(abSegname);

                Debug.Print("Found section '{0}' in segment '{1}, addr = 0x{2:X}, size = 0x{3:X}.",
                        sectionName,
                        segmentName,
                        addr,
                        size);

                AccessMode am = 0;
                if ((protection & VM_PROT_READ) != 0)
                    am |= AccessMode.Read;
                if ((protection & VM_PROT_WRITE) != 0)
                    am |= AccessMode.Write;
                if ((protection & VM_PROT_EXECUTE) != 0)
                    am |= AccessMode.Execute;

                var bytes = rdr.CreateNew(this.ldr.RawImage, offset);
                var mem = new MemoryArea(
                    Address.Ptr32(addr),
                    bytes.ReadBytes((uint)size));
                var imageSection = new ImageSegment(
                    string.Format("{0},{1}", segmentName, sectionName),
                    mem,
                    am);

                //imageSection.setBss((section.flags & SECTION_TYPE) == S_ZEROFILL);

                //if (!imageSection.isBss()) {
                //    auto pos = source_->pos();
                //    if (!source_->seek(section.offset)) {
                //        throw ParseError("Could not seek to the beginning of the section's content.");
                //    }
                //    auto bytes = source_->read(section.size);
                //    if (checked_cast<uint>(bytes.size()) != section.size) {
                //        log_.warning("Could not read all the section's content.");
                //    } else {
                //        imageSection->setContent(std::move(bytes));
                //    }
                //    source_->seek(pos);
                //}

                segmentMap.AddSegment(imageSection);
            }

            void parseSection64(uint protection, SegmentMap segmentMap)
            {
                var abSectname = rdr.ReadBytes(16);
                var abSegname = rdr.ReadBytes(16);
                ulong addr;
                ulong size;
                uint offset;
                uint align;
                uint reloff;
                uint nreloc;
                uint flags;
                uint reserved1;
                uint reserved2;
                uint reserved3;

                if (!rdr.TryReadUInt64(out addr) ||
                    !rdr.TryReadUInt64(out size) ||
                    !rdr.TryReadUInt32(out offset) ||
                    !rdr.TryReadUInt32(out align) ||
                    !rdr.TryReadUInt32(out reloff) ||
                    !rdr.TryReadUInt32(out nreloc) ||
                    !rdr.TryReadUInt32(out flags) ||
                    !rdr.TryReadUInt32(out reserved1) ||
                    !rdr.TryReadUInt32(out reserved2) ||
                    !rdr.TryReadUInt32(out reserved3))
                {
                    throw new BadImageFormatException("Could not read Mach-O section.");
                }

                var sectionName = GetAsciizString(abSectname);
                var segmentName = GetAsciizString(abSegname);

                Debug.Print("Found section '{0}' in segment '{1}, addr = 0x{2:X}, size = 0x{3:X}.",
                        sectionName,
                        segmentName,
                        addr,
                        size);

                AccessMode am = 0;
                if ((protection & VM_PROT_READ) != 0)
                    am |= AccessMode.Read;
                if ((protection & VM_PROT_WRITE) != 0)
                    am |= AccessMode.Write;
                if ((protection & VM_PROT_EXECUTE) != 0)
                    am |= AccessMode.Execute;

                var bytes = rdr.CreateNew(this.ldr.RawImage, offset);
                var mem = new MemoryArea(
                    Address.Ptr64(addr),
                    bytes.ReadBytes((uint)size));
                var imageSection = new ImageSegment(
                    string.Format("{0},{1}", segmentName, sectionName),
                    mem,
                    am);

                //imageSection.setBss((section.flags & SECTION_TYPE) == S_ZEROFILL);

                //if (!imageSection.isBss()) {
                //    auto pos = source_->pos();
                //    if (!source_->seek(section.offset)) {
                //        throw ParseError("Could not seek to the beginning of the section's content.");
                //    }
                //    auto bytes = source_->read(section.size);
                //    if (checked_cast<uint>(bytes.size()) != section.size) {
                //        log_.warning("Could not read all the section's content.");
                //    } else {
                //        imageSection->setContent(std::move(bytes));
                //    }
                //    source_->seek(pos);
                //}

                //sections_.push_back(imageSection.get());
                //image_->addSection(std::move(imageSection));
                segmentMap.AddSegment(imageSection);
            }
        }

        public enum RelocationInfoType {
  GENERIC_RELOC_VANILLA = 0, GENERIC_RELOC_PAIR = 1, GENERIC_RELOC_SECTDIFF = 2, GENERIC_RELOC_PB_LA_PTR = 3,
  GENERIC_RELOC_LOCAL_SECTDIFF = 4, GENERIC_RELOC_TLV = 5, PPC_RELOC_VANILLA = GENERIC_RELOC_VANILLA, PPC_RELOC_PAIR = GENERIC_RELOC_PAIR,
  PPC_RELOC_BR14 = 2, PPC_RELOC_BR24 = 3, PPC_RELOC_HI16 = 4, PPC_RELOC_LO16 = 5,
  PPC_RELOC_HA16 = 6, PPC_RELOC_LO14 = 7, PPC_RELOC_SECTDIFF = 8, PPC_RELOC_PB_LA_PTR = 9,
  PPC_RELOC_HI16_SECTDIFF = 10, PPC_RELOC_LO16_SECTDIFF = 11, PPC_RELOC_HA16_SECTDIFF = 12, PPC_RELOC_JBSR = 13,
  PPC_RELOC_LO14_SECTDIFF = 14, PPC_RELOC_LOCAL_SECTDIFF = 15, ARM_RELOC_VANILLA = GENERIC_RELOC_VANILLA, ARM_RELOC_PAIR = GENERIC_RELOC_PAIR,
  ARM_RELOC_SECTDIFF = GENERIC_RELOC_SECTDIFF, ARM_RELOC_LOCAL_SECTDIFF = 3, ARM_RELOC_PB_LA_PTR = 4, ARM_RELOC_BR24 = 5,
  ARM_THUMB_RELOC_BR22 = 6, ARM_THUMB_32BIT_BRANCH = 7, ARM_RELOC_HALF = 8, ARM_RELOC_HALF_SECTDIFF = 9,
  ARM64_RELOC_UNSIGNED = 0, ARM64_RELOC_SUBTRACTOR = 1, ARM64_RELOC_BRANCH26 = 2, ARM64_RELOC_PAGE21 = 3,
  ARM64_RELOC_PAGEOFF12 = 4, ARM64_RELOC_GOT_LOAD_PAGE21 = 5, ARM64_RELOC_GOT_LOAD_PAGEOFF12 = 6, ARM64_RELOC_POINTER_TO_GOT = 7,
  ARM64_RELOC_TLVP_LOAD_PAGE21 = 8, ARM64_RELOC_TLVP_LOAD_PAGEOFF12 = 9, ARM64_RELOC_ADDEND = 10, X86_64_RELOC_UNSIGNED = 0,
  X86_64_RELOC_SIGNED = 1, X86_64_RELOC_BRANCH = 2, X86_64_RELOC_GOT_LOAD = 3, X86_64_RELOC_GOT = 4,
  X86_64_RELOC_SUBTRACTOR = 5, X86_64_RELOC_SIGNED_1 = 6, X86_64_RELOC_SIGNED_2 = 7, X86_64_RELOC_SIGNED_4 = 8,
  X86_64_RELOC_TLV = 9
}
    public class Loader32 : Parser
        {
            public Loader32(MachOLoader ldr, EndianImageReader rdr) : base(ldr, rdr)
            {
            }

            public override uint ParseHeader(Address addrLoad)
            {
                uint magic;
                uint cputype;
                uint cpusubtype;
                uint filetype;
                uint ncmds;
                uint sizeofcmds;
                uint flags;
                if (rdr.TryReadUInt32(out magic) &&
                    rdr.TryReadUInt32(out cputype) &&
                    rdr.TryReadUInt32(out cpusubtype) &&
                    rdr.TryReadUInt32(out filetype) &&
                    rdr.TryReadUInt32(out ncmds) &&
                    rdr.TryReadUInt32(out sizeofcmds) &&
                    rdr.TryReadUInt32(out flags))
                {
                    arch = CreateArchitecture(cputype);
                    return ncmds;
                }

                throw new BadImageFormatException("Invalid Mach-O header.");
            }
        }

        public class Loader64 : Parser
        {
            public Loader64(MachOLoader ldr, EndianImageReader rdr)
                : base(ldr, rdr)
            {
            }

            public override uint ParseHeader(Address addrLoad)
            {
                uint magic;
                uint cputype;
                uint cpusubtype;
                uint filetype;
                uint ncmds;
                uint sizeofcmds;
                uint flags;
                uint reserved;
                if (rdr.TryReadUInt32(out magic) &&
                  rdr.TryReadUInt32(out cputype) &&
                  rdr.TryReadUInt32(out cpusubtype) &&
                  rdr.TryReadUInt32(out filetype) &&
                  rdr.TryReadUInt32(out ncmds) &&
                  rdr.TryReadUInt32(out sizeofcmds) &&
                  rdr.TryReadUInt32(out flags) &&
                  rdr.TryReadUInt32(out reserved))
                {
                    arch = CreateArchitecture(cputype);
                    return ncmds;
                }
                throw new BadImageFormatException("Invalid Mach-O header.");
            }
        }

#if NYI
        public enum ByteOrder {
            BigEndian,
            LittleEndian,
        }

        static const ByteOrder [] byteOrders = {
        ByteOrder.BigEndian,
        ByteOrder.LittleEndian
        };

Tuple<int, ByteOrder> getBitnessAndByteOrder(uint magic) {
    throw new NotImplementedException();

    //foreach (var byteOrder in byteOrders) {
    //    auto m = magic;
    //    byteOrder.convertFrom(m);

    //    if (m == MH_MAGIC) {
    //        return std::make_pair(32, byteOrder);
    //    } else if (m == MH_MAGIC_64) {
    //        return std::make_pair(64, byteOrder);
    //    }
    //}
    //return null;
}

    EndianImageReader source_;
    //core::image::Image *image_;
    //const LogToken &log_;

    ByteOrder byteOrder_;
    List<ImageMapSegment> sections_;




    private EndianImageReader CreateImageReader(byte[] source)
    {
 	    throw new NotImplementedException();
    }

    void parse<Mach>() {
        source.seek(0);

        Mach.MachHeader header;
        if (!read(source_, header)) {
            throw new BadImageFormatException("Could not read Mach-O header.");
        }

        var bitnessAndByteOrder = getBitnessAndByteOrder(header.magic);
        if (!bitnessAndByteOrder) {
            throw new BadImageFormatException("Mach-O magic does not match.");
        }

        byteOrder_ = bitnessAndByteOrder->second;

        
        byteOrder_.convertFrom(header.magic);
        byteOrder_.convertFrom(header.cputype);
        byteOrder_.convertFrom(header.ncmds);

        if (header.magic != Mach::magic) {
            throw ParseError(tr("The instantiation of the method does not match the Mach-O class."));
        }

        switch (header.cputype) {
            case CPU_TYPE_I386:
                image_.setArchitecture(QLatin1String("i386"));
                break;
            case CPU_TYPE_X86_64:
                image_.setArchitecture(QLatin1String("x86-64"));
                break;
            case CPU_TYPE_ARM:
                image_.setArchitecture(QLatin1String(byteOrder_ == ByteOrder::LittleEndian ? "arm-le" : "arm-be"));
                break;
            default:
                throw  new BadImageFormatException("Unknown CPU type: %1.").arg(header.cputype);
        }

        parseLoadCommands<Mach>(header.ncmds);
    }

private

    void parseSegmentCommand<SegmentCommand, Section>() {
        SegmentCommand command;
        if (!read(source_, command)) {
            throw BadImageFormatException("Could not read segment command.");
        }
        byteOrder_.convertFrom(command.nsects);
        byteOrder_.convertFrom(command.initprot);

        Debug.Print(tr("Found segment '%1' with %2 sections.").arg(getAsciizString(command.segname)).arg(command.nsects));

        for (uint i = 0; i < command.nsects; ++i) {
            Debug.Print(tr("Parsing section number %1.").arg(i));
            parseSection<Section>(command.initprot);
        }
    }

    const uint VM_PROT_READ = 0x01;
const uint VM_PROT_WRITE = 0x02;
const uint VM_PROT_EXECUTE = 0x04;

    void parseSection<Section>(uint protection) {
        Section section;
        if (!read(source_, section)) {
            throw ParseError(tr("Could not read section."));
        }
        byteOrder_.convertFrom(section.addr);
        byteOrder_.convertFrom(section.size);
        byteOrder_.convertFrom(section.offset);
        byteOrder_.convertFrom(section.flags);

        auto sectionName = getAsciizString(section.sectname);
        auto segmentName = getAsciizString(section.segname);

        Debug.Print(tr("Found section '%1' in segment '%2', addr = 0x%3, size = 0x%4.")
                       .arg(sectionName)
                       .arg(segmentName)
                       .arg(section.addr, 0, 16)
                       .arg(section.size, 0, 16));

        var imageSection = std::make_unique<core::image::Section>(tr("%1,%2").arg(segmentName).arg(sectionName),
                                                                   section.addr, section.size);

        imageSection.setAllocated(protection);
        AccessMode am = 0;
        if ((protection & VM_PROT_READ) != 0)
            am |= AccessMode.Read;
        if ((protection & VM_PROT_WRITE) != 0)
            am |= AccessMode.Write;
        if ((protection & VM_PROT_EXECUTE) != 0)
            am |= AccessMode.Execute;
                   
        imageSection.setCode(section.flags & (S_ATTR_SOME_INSTRUCTIONS | S_ATTR_PURE_INSTRUCTIONS));
        imageSection.setData(!imageSection->isCode());
        imageSection.setBss((section.flags & SECTION_TYPE) == S_ZEROFILL);

        if (!imageSection.isBss()) {
            auto pos = source_->pos();
            if (!source_->seek(section.offset)) {
                throw ParseError("Could not seek to the beginning of the section's content.");
            }
            auto bytes = source_->read(section.size);
            if (checked_cast<uint>(bytes.size()) != section.size) {
                log_.warning("Could not read all the section's content.");
            } else {
                imageSection->setContent(std::move(bytes));
            }
            source_->seek(pos);
        }

        sections_.push_back(imageSection.get());
        image_->addSection(std::move(imageSection));
    }

    void parseSymtabCommand<Mach>() {
        symtab_command command;
        if (!read(source_, command)) {
            throw ParseError(tr("Could not read symtab command."));
        }
        byteOrder_.convertFrom(command.symoff);
        byteOrder_.convertFrom(command.nsyms);
        byteOrder_.convertFrom(command.stroff);
        byteOrder_.convertFrom(command.strsize);

        Debug.Print(tr("Found a symbol table with %1 entries.").arg(command.nsyms));

        if (!source_->seek(command.stroff)) {
            throw ParseError(tr("Could not seek to the string table."));
        }

        auto stringTable = source_->read(command.strsize);
        if (checked_cast<uint>(stringTable.size()) != command.strsize) {
            throw ParseError(tr("Could not read string table."));
        }

        if (!source_->seek(command.symoff)) {
            throw ParseError(tr("Could not seek to the symbol table."));
        }

        for (uint i = 0; i < command.nsyms; ++i) {
            using core::image::Symbol;
            using core::image::SymbolType;

            typename Mach::Nlist symbol;
            if (!read(source_, symbol)) {
                throw ParseError(tr("Could not read symbol number %1.").arg(i));
            }
            byteOrder_.convertFrom(symbol.n_strx);
            byteOrder_.convertFrom(symbol.n_type);
            byteOrder_.convertFrom(symbol.n_sect);
            byteOrder_.convertFrom(symbol.n_value);

            QString name = getAsciizString(stringTable, symbol.n_strx);

            boost::optional<ConstantValue> value;
            if ((symbol.n_type & N_TYPE) != N_UNDF) {
                value = symbol.n_value;
            }

            ImageMapSegment section = null;
            if (symbol.n_sect != NO_SECT && symbol.n_sect <= sections_.size()) {
                section = sections_[symbol.n_sect - 1];
            }

            /* Mach-O does not tell us the type. Let's do some guessing. */
            auto type = SymbolType.NOTYPE;
            if (section) {
                if (section->isCode()) {
                    type = SymbolType.FUNCTION;
                } else if (section->isData()) {
                    type = SymbolType.OBJECT;
                }
            }

            image_->addSymbol(std::make_unique<core::image::Symbol>(type, name, value, section));
        }
    }
};


bool doCanParse(QIODevice *source) const {
    uint magic;
    return read(source, magic) && getBitnessAndByteOrder(magic);
}

void doParse(QIODevice *source, core::image::Image *image, const LogToken &log) const {
    uint magic;
    if (!read(source, magic)) {
        throw ParseError(tr("Could not read Mach-O magic."));
    }

    auto bitnessAndByteOrder = getBitnessAndByteOrder(magic);
    if (!bitnessAndByteOrder) {
        throw ParseError(tr("Mach-O magic does not match."));
    }

    switch (bitnessAndByteOrder->first) {
        case 32:
            MachOParserImpl(source, image, log).parse<MachO32>();
            break;
        case 64:
            MachOParserImpl(source, image, log).parse<MachO64>();
            break;
        default:
            unreachable();
    }
}

//typedef uint cpu_type_t;
//typedef uint cpu_subtype_t;

struct mach_header {
    uint magic;
    uint cputype;
    uint cpusubtype;
    uint filetype;
    uint ncmds;
    uint sizeofcmds;
    uint flags;
};

struct mach_header_64 {
    uint magic;
    uint cputype;
    uint cpusubtype;
    uint filetype;
    uint ncmds;
    uint sizeofcmds;
    uint flags;
    uint reserved;
};


 
struct load_command {
    public uint cmd;
    public uint cmdsize;
}



typedef uint vm_prot_t;

struct segment_command {
    uint cmd;
    uint cmdsize;
    string segname; // char segname[16];
    uint vmaddr;
    uint vmsize;
    uint fileoff;
    uint filesize;
    vm_prot_t maxprot;
    vm_prot_t initprot;
    uint nsects;
    uint flags;
};

struct segment_command_64 {
    uint cmd;
    uint cmdsize;
    char segname[16];
    ulong vmaddr;
    ulong vmsize;
    ulong fileoff;
    ulong filesize;
    vm_prot_t maxprot;
    vm_prot_t initprot;
    uint nsects;
    uint flags;
}


struct section {
    char sectname[16];
    char segname[16];
    uint addr;
    uint size;
    uint offset;
    uint align;
    uint reloff;
    uint nreloc;
    uint flags;
    uint reserved1;
    uint reserved2;
};

struct section_64 {
    char sectname[16];
    char segname[16];
    ulong addr;
    ulong size;
    uint offset;
    uint align;
    uint reloff;
    uint nreloc;
    uint flags;
    uint reserved1;
    uint reserved2;
};

 const uint SECTION_TYPE = 0x000000ff;
 const uint SECTION_ATTRIBUTES = 0xffffff00;
 const uint S_REGULAR = 0;
 const uint S_ZEROFILL = 1;
 const uint S_ATTR_SOME_INSTRUCTIONS = 0x00000400;
 const uint S_ATTR_PURE_INSTRUCTIONS = 0x80000000;

struct symtab_command {
    public uint cmd;
    public uint cmdsize;
    public uint symoff;
    public uint nsyms;
    public uint stroff;
    public uint strsize;
};

struct nlist {
    uint n_strx;
    byte n_type;
    byte n_sect;
    int16_t n_desc;
    uint n_value;
};

struct nlist_64 {
    uint n_strx;
    byte n_type;
    byte n_sect;
    ushort n_desc;
    ulong n_value;
};

static const byte N_TYPE = 0x0e;
static const byte N_UNDF = 0x0;
static const byte N_ABS = 0x2;
static const byte N_SECT = 0xe;
static const byte N_INDR = 0xa;

static const byte NO_SECT = 0;

/* vim:set et sts=4 sw=4: */


}
#endif
    }
}