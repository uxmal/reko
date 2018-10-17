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
    using Reko.Core.Types;
    using static Reko.ImageLoaders.MachO.MachOLoader.SectionFlags;
    using static Reko.ImageLoaders.MachO.MachOLoader.Parser.Command;

    // http://newosxbook.com/articles/DYLD.html
    public class MachOLoader : ImageLoader
    {
        const uint MH_MAGIC = 0xFEEDFACE;
        const uint MH_MAGIC_64 = 0xFEEDFACF;
        const uint MH_MAGIC_32_LE = 0xCEFAEDFE; //0xfeedface;
        const uint MH_MAGIC_64_LE = 0xCFFAEDFE; // 0xfeedfacf;

        // http://www.opensource.apple.com/source/xnu/xnu-792.13.8/osfmk/mach/machine.h

        public const uint CPU_ARCH_MASK = 0xff000000;		/* mask for architecture bits */
        public const uint CPU_ARCH_ABI64 = 0x01000000;		/* 64 bit ABI */

        public const uint CPU_TYPE_ANY = 0xFFFFFFFF;
        public const uint CPU_TYPE_VAX = 1;
        public const uint CPU_TYPE_MC680x0 = 6;
        public const uint CPU_TYPE_X86 = 7;
        public const uint CPU_TYPE_I386 = CPU_TYPE_X86;
        public const uint CPU_TYPE_X86_64 = (CPU_TYPE_X86 | CPU_ARCH_ABI64);
        public const uint CPU_TYPE_MIPS = 8;
        public const uint CPU_TYPE_MC98000 = 11;
        public const uint CPU_TYPE_ARM = 12;
        public const uint CPU_TYPE_ARM64 = (CPU_TYPE_ARM | CPU_ARCH_ABI64);
        public const uint CPU_TYPE_MC88000 = 13;
        public const uint CPU_TYPE_SPARC = 14;
        public const uint CPU_TYPE_I860 = 15;
        public const uint CPU_TYPE_ALPHA = 16;
        public const uint CPU_TYPE_POWERPC = 18;
        public const uint CPU_TYPE_POWERPC64 = (CPU_TYPE_POWERPC | CPU_ARCH_ABI64);

        public const uint CPU_SUBTYPE_MC68030 = 1;
        public const uint CPU_SUBTYPE_MC68040 = 2;

        private Parser parser;
        private List<MachOSection> sections;
        private Dictionary<string, MachOSection> sectionsByName;
        private Dictionary<MachOSection, ImageSegment> imageSections;
        private List<MachOSymbol> machoSymbols;
        private SortedList<Address, ImageSymbol> imageSymbols;
        private List<ImageSymbol> entryPoints;
        private Program program;

        public MachOLoader(IServiceProvider services, string filename, byte[] rawImg)
            : base(services, filename, rawImg)
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

        public override Program Load(Address addrLoad)
        {
            this.program = new Program();
            parser = CreateParser();
            var hdr = parser.ParseHeader(addrLoad);
            this.program.SegmentMap = parser.ParseLoadCommands(hdr, addrLoad);
            this.program.Architecture = parser.arch;
            this.program.Platform = new DefaultPlatform(Services, parser.arch);
            return this.program;
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
            if (!MemoryArea.TryReadBeUInt32(RawImage, 0, out uint magic))
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
            return new RelocationResults(entryPoints, imageSymbols);
        }

        public abstract class Parser
        {
            protected MachOLoader ldr;
            protected EndianImageReader rdr;
            public IProcessorArchitecture arch;
            protected Dictionary<uint, uint> mpCputypeToUnixthreadPc;

            protected Parser(MachOLoader ldr, EndianImageReader rdr)
            {
                this.ldr = ldr;
                this.rdr = rdr;
                this.mpCputypeToUnixthreadPc = new Dictionary<uint, uint>
                {
                    { CPU_TYPE_POWERPC, 0x0010 },
                    { CPU_TYPE_POWERPC64, 0x0010 },
                    { CPU_TYPE_X86, 0x38 },
                    { CPU_TYPE_X86_64, 0x90 },
                    { CPU_TYPE_ARM, 0x4C },
                    { CPU_TYPE_ARM64, 0x110 },
                    { CPU_TYPE_MC680x0, 0x44 }
                };
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
                case CPU_TYPE_MC680x0: arch = "m68k"; break;
                default:
                    throw new NotSupportedException(string.Format("Processor format {0} is not supported.", cputype));
                }
                return cfgSvc.GetArchitecture(arch);
            }

            /// <summary>
            /// Parse the Mach-O image header.
            /// </summary>
            /// <param name="addrLoad"></param>
            /// <returns>The number of load commands in the header.</returns>
            public abstract mach_header_64 ParseHeader(Address addrLoad);


            // http://llvm.org/docs/doxygen/html/Support_2MachO_8h_source.html
            // http://opensource.apple.com//source/clang/clang-503.0.38/src/tools/macho-dump/macho-dump.cpp
            // https://github.com/opensource-apple/cctools/blob/master/libstuff/ofile.c
            [Flags]
            public enum Command : uint
            {
                LC_REQ_DYLD = 0x80000000,
                LC_SEGMENT = 0x00000001u,
                LC_SYMTAB = 0x00000002u,
                LC_SYMSEG = 0x00000003u,
                LC_THREAD = 0x00000004u,
                LC_UNIXTHREAD = 0x00000005u,
                LC_LOADFVMLIB = 0x00000006u,
                LC_IDFVMLIB = 0x00000007u,
                LC_IDENT = 0x00000008u,
                LC_FVMFILE = 0x00000009u,
                LC_PREPAGE = 0x0000000Au,
                LC_DYSYMTAB = 0x0000000Bu,
                LC_LOAD_DYLIB = 0x0000000Cu,
                LC_ID_DYLIB = 0x0000000Du,
                LC_LOAD_DYLINKER = 0x0000000Eu,
                LC_ID_DYLINKER = 0x0000000Fu,
                LC_PREBOUND_DYLIB = 0x00000010u,
                LC_ROUTINES = 0x00000011u,
                LC_SUB_FRAMEWORK = 0x00000012u,
                LC_SUB_UMBRELLA = 0x00000013u,
                LC_SUB_CLIENT = 0x00000014u,
                LC_SUB_LIBRARY = 0x00000015u,
                LC_TWOLEVEL_HINTS = 0x00000016u,
                LC_PREBIND_CKSUM = 0x00000017u,
                LC_LOAD_WEAK_DYLIB = 0x80000018u,
                LC_SEGMENT_64 = 0x00000019u,
                LC_ROUTINES_64 = 0x0000001Au,
                LC_UUID = 0x0000001Bu,
                LC_RPATH = 0x8000001Cu,
                LC_CODE_SIGNATURE = 0x0000001Du,
                LC_SEGMENT_SPLIT_INFO = 0x0000001Eu,
                LC_REEXPORT_DYLIB = 0x8000001Fu,
                LC_LAZY_LOAD_DYLIB = 0x00000020u,
                LC_ENCRYPTION_INFO = 0x00000021u,
                LC_DYLD_INFO = 0x00000022u,
                LC_DYLD_INFO_ONLY = 0x80000022u,
                LC_LOAD_UPWARD_DYLIB = 0x80000023u,
                LC_VERSION_MIN_MACOSX = 0x00000024u,
                LC_VERSION_MIN_IPHONEOS = 0x00000025u,
                LC_FUNCTION_STARTS = 0x00000026u,
                LC_DYLD_ENVIRONMENT = 0x00000027u,
                LC_MAIN = 0x80000028u,
                LC_DATA_IN_CODE = 0x00000029u,
                LC_SOURCE_VERSION = 0x0000002Au,
                LC_DYLIB_CODE_SIGN_DRS = 0x0000002Bu,
                LC_ENCRYPTION_INFO_64 = 0x0000002Cu,
                LC_LINKER_OPTION = 0x0000002Du,
                LC_LINKER_OPTIMIZATION_HINT = 0x0000002Eu,
                LC_VERSION_MIN_TVOS = 0x0000002Fu,
                LC_VERSION_MIN_WATCHOS = 0x00000030u,
            }


            public SegmentMap ParseLoadCommands(mach_header_64 hdr, Address addrLoad)
            {
                var imageMap = new SegmentMap(addrLoad);
                Debug.Print("Parsing {0} load commands.", hdr.ncmds);

                for (uint i = 0; i < hdr.ncmds; ++i)
                {
                    var pos = rdr.Offset;
                    if (!rdr.TryReadUInt32(out uint cmd) ||
                        !rdr.TryReadUInt32(out uint cmdsize))
                    {
                        throw new BadImageFormatException(string.Format(
                            "Unable to read Mach-O command ({0:X}).",
                            rdr.Offset));
                    }
                    Debug.Print("{0,2}: Read MachO load command 0x{1:X} {2} of size {3}.", i, (int)cmd, cmd, cmdsize);

                    switch ((Command)(cmd & ~(uint)LC_REQ_DYLD))
                    {
                    case LC_SEGMENT:
                        ParseSegmentCommand32(imageMap);
                        break;
                    case LC_SEGMENT_64:
                        ParseSegmentCommand64(imageMap);
                        break;
                    case LC_SYMTAB:
                        ParseSymtabCommand();
                        break;
                    case LC_DYSYMTAB:
                        ParseDysymtabCommand();
                        break;
                    case LC_FUNCTION_STARTS:
                        ParseFunctionStarts(rdr.Clone());
                        break;
                    case LC_UNIXTHREAD:
                        ParseUnixThread(hdr.cputype);
                        break;
                    }
                    rdr.Offset = pos + cmdsize;
                }
                return imageMap;
            }

            protected MachOSection FindSectionByType(SectionFlags type)
            {
                foreach (var kv in ldr.imageSections)
                {
                    if ((kv.Key.Flags & SECTION_TYPE) == type)
                        return kv.Key;
                }
                return null;
            }

            protected abstract void ParseUnixThread(uint cputype);

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

            void ParseSegmentCommand32(SegmentMap imageMap)
            {
                var segname = ReadSegmentName();
                if (!rdr.TryReadUInt32(out uint vmaddr) ||
                    !rdr.TryReadUInt32(out uint vmsize) ||
                    !rdr.TryReadUInt32(out uint fileoff) ||
                    !rdr.TryReadUInt32(out uint filesize) ||
                    !rdr.TryReadUInt32(out uint maxprot) ||
                    !rdr.TryReadUInt32(out uint initprot) ||
                    !rdr.TryReadUInt32(out uint nsects) ||
                    !rdr.TryReadUInt32(out uint flags))
                {
                    throw new BadImageFormatException("Could not read segment command.");
                }
                Debug.Print("  Found segment '{0}' with {1} sections.", segname, nsects);

                for (uint i = 0; i < nsects; ++i)
                {
                    Debug.Print("    Parsing section number {0}.", i);
                    ParseSection32(i, initprot, imageMap);
                }
            }

            void ParseSegmentCommand64(SegmentMap imageMap)
            {
                string segname = ReadSegmentName();
                if (!rdr.TryReadUInt64(out ulong vmaddr) ||
                    !rdr.TryReadUInt64(out ulong vmsize) ||
                    !rdr.TryReadUInt64(out ulong fileoff) ||
                    !rdr.TryReadUInt64(out ulong filesize) ||
                    !rdr.TryReadUInt32(out uint maxprot) ||
                    !rdr.TryReadUInt32(out uint initprot) ||
                    !rdr.TryReadUInt32(out uint nsects) ||
                    !rdr.TryReadUInt32(out uint flags))
                {
                    throw new BadImageFormatException("Could not read segment command.");
                }
                Debug.Print("Found segment '{0}' with {1} sections.", segname, nsects);

                for (uint i = 0; i < nsects; ++i)
                {
                    Debug.Print("Parsing section number {0}.", i);
                    ParseSection64(initprot, imageMap);
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

            void ParseFunctionStarts(EndianImageReader rdr)
            {
                if (!rdr.TryReadUInt32(out uint dataoff) ||
                    !rdr.TryReadUInt32(out uint datasize))
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

            private string GetAsciizString(byte[] bytes, int iStart)
            {
                int iEnd = Array.IndexOf<byte>(bytes, 0, iStart);
                if (iEnd == -1)
                    iEnd = bytes.Length;
                return Encoding.ASCII.GetString(bytes, iStart, iEnd - iStart);
            }

            const uint VM_PROT_READ = 0x01;
            const uint VM_PROT_WRITE = 0x02;
            const uint VM_PROT_EXECUTE = 0x04;

            void ParseSection32(uint iSection, uint protection, SegmentMap segmentMap)
            {
                var abSectname = rdr.ReadBytes(16);
                var abSegname = rdr.ReadBytes(16);

                if (!rdr.TryReadUInt32(out uint uAddr) ||
                    !rdr.TryReadUInt32(out uint size) ||
                    !rdr.TryReadUInt32(out uint offset) ||
                    !rdr.TryReadUInt32(out uint align) ||
                    !rdr.TryReadUInt32(out uint reloff) ||
                    !rdr.TryReadUInt32(out uint nreloc) ||
                    !rdr.TryReadUInt32(out uint flags) ||
                    !rdr.TryReadUInt32(out uint reserved1) ||
                    !rdr.TryReadUInt32(out uint reserved2))
                {
                    throw new BadImageFormatException("Could not read Mach-O section.");
                }

                var sectionName = GetAsciizString(abSectname, 0);
                var segmentName = GetAsciizString(abSegname, 0);

                Debug.Print("    Found section '{0}' in segment '{1}, addr = 0x{2:X}, size = 0x{3:X}.",
                        sectionName,
                        segmentName,
                        uAddr,
                        size);
                Debug.Print("      reloff: {0:X} nreloc: {1:X} flags {2:X}", reloff, nreloc, flags);

                AccessMode am = 0;
                if ((protection & VM_PROT_READ) != 0)
                    am |= AccessMode.Read;
                if ((protection & VM_PROT_WRITE) != 0)
                    am |= AccessMode.Write;
                if ((protection & VM_PROT_EXECUTE) != 0)
                    am |= AccessMode.Execute;

                var addr = Address.Ptr32(uAddr);
                var bytes = rdr.CreateNew(this.ldr.RawImage, offset);
                var name = string.Format("{0},{1}", segmentName, sectionName);

                var mem = new MemoryArea(addr, bytes.ReadBytes((uint) size));
                var machoSection = new MachOSection(name, addr, (SectionFlags) flags, reserved1, reserved2);
                var imageSection = new ImageSegment(name, mem, am);
                ldr.imageSections.Add(machoSection, imageSection);

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
                this.ldr.sections.Add(machoSection);
                this.ldr.sectionsByName.Add(imageSection.Name, machoSection);
                if (imageSection.Size > 0)
                {
                    segmentMap.AddSegment(imageSection);
                }
            }

            void ParseSection64(uint protection, SegmentMap segmentMap)
            {
                var abSectname = rdr.ReadBytes(16);
                var abSegname = rdr.ReadBytes(16);
                if (!rdr.TryReadUInt64(out ulong addr) ||
                    !rdr.TryReadUInt64(out ulong size) ||
                    !rdr.TryReadUInt32(out uint offset) ||
                    !rdr.TryReadUInt32(out uint align) ||
                    !rdr.TryReadUInt32(out uint reloff) ||
                    !rdr.TryReadUInt32(out uint nreloc) ||
                    !rdr.TryReadUInt32(out uint flags) ||
                    !rdr.TryReadUInt32(out uint reserved1) ||
                    !rdr.TryReadUInt32(out uint reserved2) ||
                    !rdr.TryReadUInt32(out uint reserved3))
                {
                    throw new BadImageFormatException("Could not read Mach-O section.");
                }

                var sectionName = GetAsciizString(abSectname, 0);
                var segmentName = GetAsciizString(abSegname, 0);

                Debug.Print("    Found section '{0}' in segment '{1}, addr = 0x{2:X}, size = 0x{3:X}.",
                        sectionName,
                        segmentName,
                        addr,
                        size);
                Debug.Print("      reloff: {0:X} nreloc: {1:X} flags {2:X}", reloff, nreloc, flags);
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
                    bytes.ReadBytes((uint) size));
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


            void ParseSymtabCommand()
            {
                if (rdr.TryReadUInt32(out uint symoff) &&
                    rdr.TryReadUInt32(out uint nsyms) &&
                    rdr.TryReadUInt32(out uint stroff) &&
                    rdr.TryReadUInt32(out uint strsize))
                {
                    Debug.Print("    Found a symbol table with {0} entries at {1:X8}.", nsyms, symoff);
                    var strs = rdr.CreateNew(this.ldr.RawImage, stroff);
                    var strBytes = strs.ReadBytes(strsize);
                    var syms = rdr.CreateNew(this.ldr.RawImage, symoff);
                    for (uint i = 0; i < nsyms; ++i)
                    {
                        if (syms.TryReadUInt32(out uint n_strx) &&
                            syms.TryReadByte(out byte n_type) &&
                            syms.TryReadByte(out byte n_sect) &&
                            syms.TryReadUInt16(out ushort n_desc) &&
                            syms.TryReadUInt32(out uint n_value))
                        {
                            var str = GetAsciizString(strBytes, (int) n_strx);
                            Debug.Print("      {0,2}: {1:X8} {2:X8} {3}({4}) {5:X4} {6:X8} {7}",
                                i, n_strx, n_type, ldr.sections[n_sect].Name,
                                n_sect, n_desc, n_value, str);
                            var addr = Address.Ptr32(n_value);
                            ldr.machoSymbols.Add(new MachOSymbol(str, n_type, ldr.sections[n_sect], n_desc, n_value));
                            ldr.imageSymbols[addr] = new ImageSymbol(addr);
                        }
                    }
                }
                else
                {
                    throw new BadImageFormatException("Could not read symtab command.");
                }

                /*
                auto stringTable = source_->read(command.strsize);
                if (checked_cast<uint>(stringTable.size()) != command.strsize)
                {
                    throw ParseError(tr("Could not read string table."));
                }

                    throw ParseError(tr("Could not seek to the symbol table."));
                }

                for (uint i = 0; i < command.nsyms; ++i)
                {
                    using core::image::Symbol;
                    using core::image::SymbolType;

                    typename Mach::Nlist symbol;
                    if (!read(source_, symbol))
                    {
                        throw ParseError(tr("Could not read symbol number %1.").arg(i));
                    }
                    byteOrder_.convertFrom(symbol.n_strx);
                    byteOrder_.convertFrom(symbol.n_type);
                    byteOrder_.convertFrom(symbol.n_sect);
                    byteOrder_.convertFrom(symbol.n_value);

                    QString name = getAsciizString(stringTable, symbol.n_strx);

                    boost::optional<ConstantValue> value;
                    if ((symbol.n_type & N_TYPE) != N_UNDF)
                    {
                        value = symbol.n_value;
                    }

                    ImageMapSegment section = null;
                    if (symbol.n_sect != NO_SECT && symbol.n_sect <= sections_.size())
                    {
                        section = sections_[symbol.n_sect - 1];
                    }

                    // Mach-O does not tell us the type. Let's do some guessing.
                    
                    auto type = SymbolType.NOTYPE;
                    if (section)
                    {
                        if (section->isCode())
                        {
                            type = SymbolType.FUNCTION;
                        }
                        else if (section->isData())
                        {
                            type = SymbolType.OBJECT;
                        }
                    }

                    image_->addSymbol(std::make_unique<core::image::Symbol>(type, name, value, section));
                    */
            }

            void ParseDysymtabCommand()
            {
                if (rdr.TryReadUInt32(out uint ilocalsym) &&
                    rdr.TryReadUInt32(out uint nlocalsym) &&
                    rdr.TryReadUInt32(out uint iextdefsym) &&
                    rdr.TryReadUInt32(out uint nextdefsym) &&
                    rdr.TryReadUInt32(out uint iundefsym) &&
                    rdr.TryReadUInt32(out uint nundefsym) &&
                    rdr.TryReadUInt32(out uint tocoff) &&
                    rdr.TryReadUInt32(out uint ntoc) &&
                    rdr.TryReadUInt32(out uint modtaboff) &&
                    rdr.TryReadUInt32(out uint nmodtab) &&
                    rdr.TryReadUInt32(out uint extrefsymoff) &&
                    rdr.TryReadUInt32(out uint nextrefsyms) &&
                    rdr.TryReadUInt32(out uint indirectsymoff) &&
                    rdr.TryReadUInt32(out uint nindirectsyms) &&
                    rdr.TryReadUInt32(out uint extreloff) &&
                    rdr.TryReadUInt32(out uint nextrel) &&
                    rdr.TryReadUInt32(out uint locreloff) &&
                    rdr.TryReadUInt32(out uint nlocrel))
                {
                    if (nundefsym > 0 && nindirectsyms > 0)
                    {
                        var rdrIndirect = rdr.CreateNew(ldr.RawImage, indirectsymoff);
                        var indirects = LoadIndirectSymbols(rdrIndirect, nindirectsyms);
                        var lazySection = FindSectionByType(S_LAZY_SYMBOL_POINTERS);
                        if (lazySection != null)
                            LoadImports(lazySection, indirects);
                        var nonLazySection = FindSectionByType(S_NON_LAZY_SYMBOL_POINTERS);
                        if (nonLazySection != null)
                            LoadImports(nonLazySection, indirects);
                    }
                    nextrel.ToString();
                }
                else
                {
                    throw new BadImageFormatException("Failed to parse LC_DYSYMTAB section.");
                }
            }

            protected abstract void LoadImports(MachOSection msec, List<uint> indirects);

            private List<uint> LoadIndirectSymbols(EndianImageReader rdr, uint nindirectsyms)
            {
                Debug.Print("    Loading indirect symbols");
                var indirects = new List<uint>();
                for (uint i = 0; i < nindirectsyms; ++i)
                {
                    if (!rdr.TryReadUInt32(out uint indir))
                        return indirects;
                    indirects.Add(indir);
                    var sym = ldr.machoSymbols[(int) indir];
                    Debug.Print("      {0}: {1} {2:X2} {3:X2} {4:X8} {5}", i, indir, sym.n_type, sym.n_desc, sym.n_value, sym.Name);  
                }
                return indirects;
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

        public class Loader32 : Parser
        {
            public Loader32(MachOLoader ldr, EndianImageReader rdr) : base(ldr, rdr)
            {
            }

            public override mach_header_64 ParseHeader(Address addrLoad)
            {
                if (rdr.TryReadUInt32(out uint magic) &&
                    rdr.TryReadUInt32(out uint cputype) &&
                    rdr.TryReadUInt32(out uint cpusubtype) &&
                    rdr.TryReadUInt32(out uint filetype) &&
                    rdr.TryReadUInt32(out uint ncmds) &&
                    rdr.TryReadUInt32(out uint sizeofcmds) &&
                    rdr.TryReadUInt32(out uint flags))
                {
                    arch = CreateArchitecture(cputype);
                    return new mach_header_64
                    {
                        magic = magic,
                        cputype = cputype,
                        cpusubtype = cpusubtype,
                        filetype = filetype,
                        ncmds = ncmds,
                        sizeofcmds = sizeofcmds,
                        flags = flags
                    };
                }
                throw new BadImageFormatException("Invalid Mach-O header.");
            }

            protected override void ParseUnixThread(uint cputype)
            {
                if (rdr.TryReadUInt32(out uint flavor) &&
                    rdr.TryReadUInt32(out uint count))
                {
                    var data = rdr.ReadBytes(count * 4);
                    if (!mpCputypeToUnixthreadPc.TryGetValue(cputype, out uint uOffAddrStart))
                        throw new BadImageFormatException($"LC_PARSEUNIXTHREAD for CPU type {cputype} has not been implemented.");
                    var ep = MemoryArea.ReadBeUInt32(data, uOffAddrStart);
                    base.ldr.entryPoints.Add(new ImageSymbol(Address.Ptr32(ep)));
                }
            }

            protected override void LoadImports(MachOSection section, List<uint> indirects)
            {
                Debug.Print("    Loading imports from section {0} / res: {1:X8}", section.Name, section.Reserved1);
                var iseg = ldr.imageSections[section];
                var rdr = iseg.CreateImageReader(arch);
                var addr = rdr.Address;
                var tableIndex = section.Reserved1;
                int i = (int) tableIndex;
                while (rdr.TryReadUInt32(out uint uImport))
                {
                    var msym = ldr.machoSymbols[(int)indirects[i]];
                    Debug.Print("      {0}: {1:X8} {2}", addr, uImport, msym.Name);
                    var addrImport = Address.Ptr32(uImport);
                    var ptr = new Pointer(new CodeType(), arch.PointerType.BitSize);
                    var impSymbol = new ImageSymbol(addr)
                    {
                        Name = "__imp__" + msym.Name,
                        Type = SymbolType.Data,
                        DataType = ptr,
                        Size = (uint) ptr.Size
                    };
                    ldr.imageSymbols[addr] = impSymbol;
                    ldr.program.ImportReferences.Add(addr, new NamedImportReference(addrImport, "", msym.Name));
                    addr = rdr.Address;
                    ++i;
                }
            }
        }

        public class Loader64 : Parser
        {
            public Loader64(MachOLoader ldr, EndianImageReader rdr)
                : base(ldr, rdr)
            {
            }

            public override mach_header_64 ParseHeader(Address addrLoad)
            {
                if (rdr.TryReadUInt32(out uint magic) &&
                  rdr.TryReadUInt32(out uint cputype) &&
                  rdr.TryReadUInt32(out uint cpusubtype) &&
                  rdr.TryReadUInt32(out uint filetype) &&
                  rdr.TryReadUInt32(out uint ncmds) &&
                  rdr.TryReadUInt32(out uint sizeofcmds) &&
                  rdr.TryReadUInt32(out uint flags) &&
                  rdr.TryReadUInt32(out uint reserved))
                {
                    arch = CreateArchitecture(cputype);
                    return new mach_header_64
                    {
                        magic = magic,
                        cputype = cputype,
                        cpusubtype = cpusubtype,
                        filetype = filetype,
                        ncmds = ncmds,
                        sizeofcmds = sizeofcmds,
                        flags = flags,
                        reserved = reserved,
                    };
                }
                throw new BadImageFormatException("Invalid Mach-O header.");
            }

            protected override void ParseUnixThread(uint cputype)
            {
                throw new NotImplementedException();
            }

            protected override void LoadImports(MachOSection section, List<uint> indirects)
            {
                throw new NotImplementedException();
            }
        }

#if NYI
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
            public MachOSection msec;
            public ushort n_desc;
            public uint n_value;

            public MachOSymbol(string name, byte n_type, MachOSection msec, ushort n_desc, uint n_value)
            {
                this.Name = name;
                this.n_type = n_type;
                this.msec = msec;
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
}