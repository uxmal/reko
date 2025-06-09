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
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.ImageLoaders.MachO.Arch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.ImageLoaders.MachO
{
    using static Reko.ImageLoaders.MachO.Parser.Command;
    using static Reko.ImageLoaders.MachO.SectionFlags;

    public abstract class Parser
    {
        // https://opensource.apple.com/source/xnu/xnu-792.13.8/osfmk/mach/machine.h
        // https://opensource.apple.com/source/xnu/xnu-2050.18.24/EXTERNAL_HEADERS/mach-o/loader.h

        public const uint CPU_ARCH_MASK = 0xff000000;		// mask for architecture bits
        public const uint CPU_ARCH_ABI64 = 0x01000000;		// 64 bit ABI

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

        protected MachOLoader ldr;
        private IConfigurationService cfgSvc;
        protected EndianByteImageReader rdr;
        protected Dictionary<uint, uint> mpCputypeToUnixthreadPc;
        protected Dictionary<string, ulong> segments;

        protected Parser(MachOLoader ldr, EndianByteImageReader rdr)
        {
            this.ldr = ldr;
            this.cfgSvc = ldr.Services.RequireService<IConfigurationService>();
            this.rdr = rdr;
            this.mpCputypeToUnixthreadPc = new Dictionary<uint, uint>
            {
                { CPU_TYPE_POWERPC, 0x0000 },
                { CPU_TYPE_POWERPC64, 0x0010 },
                { CPU_TYPE_X86, 0x38 },
                { CPU_TYPE_X86_64, 0x80 },
                { CPU_TYPE_ARM, 0x4C },
                { CPU_TYPE_ARM64, 0x110 },
                { CPU_TYPE_MC680x0, 0x44 }
            };
            this.segments = new Dictionary<string, ulong>();
        }

        public DySymtab? dysymtab { get; private set; }


        public ArchSpecific CreateArchitecture(uint cputype)
        {
            switch (cputype)
            {
            case CPU_TYPE_ARM: return MakeSpecific("arm32", a => new ArmSpecific(a));
            case CPU_TYPE_I386: return MakeSpecific("x86-protected-32", a => new X86Specific(a));
            case CPU_TYPE_X86_64: return MakeSpecific("x86-protected-64", a => new X86Specific(a));
            case CPU_TYPE_MIPS: return MakeSpecific("mips-be-32", a => new MipsSpecific(a));
            case CPU_TYPE_POWERPC: return MakeSpecific("ppc-be-32", a => new PowerPCSpecific(a));
            case CPU_TYPE_MC680x0: return MakeSpecific("m68k", a=> new M68kSpecific(a));
            case CPU_TYPE_ARM64: return MakeSpecific("arm-64", a => new Arm64Specific(a));
            default:
                throw new NotSupportedException(string.Format("Processor format {0} is not supported.", cputype));
            }
        }

        private ArchSpecific MakeSpecific(string archLabel, Func<IProcessorArchitecture, ArchSpecific> ctor)
        {
            var arch = cfgSvc.GetArchitecture(archLabel);
            return ctor(arch!);
        }

        /// <summary>
        /// Parse the Mach-O image header.
        /// </summary>
        /// <param name="addrLoad"></param>
        /// <returns>The number of load commands in the header.</returns>
        public abstract (mach_header_64, ArchSpecific) ParseHeader(Address? addrLoad);

        // http://www.m4b.io/reverse/engineering/mach/binaries/2015/03/29/mach-binaries.html
        // http://llvm.org/docs/doxygen/html/Support_2MachO_8h_source.html
        // http://opensource.apple.com//source/clang/clang-503.0.38/src/tools/macho-dump/macho-dump.cpp
        // https://github.com/opensource-apple/cctools/blob/master/libstuff/ofile.c
        // https://github.com/aidansteele/osx-abi-macho-file-format-reference
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

        public Program ParseLoadCommands(mach_header_64 hdr, IProcessorArchitecture arch, Address addrLoad)
        {
            var segmentMap = new SegmentMap(addrLoad);
            Debug.Print("Parsing {0} load commands.", hdr.ncmds);
            string? platformName = null;
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
                MachOLoader.trace.Inform("{0,2}: Read MachO load command 0x{1:X} {2} of size {3}.", i, cmd, (Command) cmd, cmdsize);

                switch ((Command) (cmd & ~(uint) LC_REQ_DYLD))
                {
                case LC_SEGMENT:
                    ParseSegmentCommand32(segmentMap);
                    break;
                case LC_SEGMENT_64:
                    //$REVIEW: are there any other platforms using 64-bit machO? iOS?
                    platformName = "macOsX";
                    ParseSegmentCommand64(segmentMap);
                    break;
                case LC_SYMTAB:
                    ParseSymtabCommand(arch);
                    break;
                case LC_DYSYMTAB:
                    ParseDysymtabCommand(arch);
                    break;
                case LC_FUNCTION_STARTS:
                    ParseFunctionStarts(rdr.Clone());
                    break;
                case LC_UNIXTHREAD:
                    ParseUnixThread(hdr.cputype, arch);
                    break;
                case LC_VERSION_MIN_MACOSX:
                    platformName = "macOsX";
                    break;
                case LC_MAIN & ~LC_REQ_DYLD:
                    ParseMain(arch);
                    break;
                }
                rdr.Offset = pos + cmdsize;
            }
            ldr.program!.Architecture = arch;
            ldr.program.Memory = new ByteProgramMemory(segmentMap);
            ldr.program.SegmentMap = segmentMap;
            if (!string.IsNullOrEmpty(platformName))
            {
                var env = cfgSvc.GetEnvironment(platformName!);
                ldr.program.Platform = env.Load(ldr.Services, arch);
            }
            else
            {
                ldr.program.Platform = new DefaultPlatform(ldr.Services, arch);
            }
            return ldr.program;
        }

        protected MachOSection? FindSectionByType(SectionFlags type)
        {
            foreach (var kv in ldr.imageSections)
            {
                if ((kv.Key.Flags & SECTION_TYPE) == type)
                    return kv.Key;
            }
            return null;
        }

        protected abstract void ParseUnixThread(uint cputype, IProcessorArchitecture arch);

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
            Debug.Print("  Found segment '{0}' at address {1:X} of length {2:X} with {3} sections.", segname, vmaddr, vmsize, nsects);
            this.segments[segname] = vmaddr;

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
            Debug.Print("Found segment '{0}' at address {1} with {2} sections.", segname, vmaddr, nsects);
            this.segments[segname] = vmaddr;
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
                Debug.Print("  fn start: {0:X}", fn);
            }
        }

        [Conditional("DEBUG")]
        protected static void DumpData(ulong offset, byte[] data)
        {
            var sb = new StringBuilder();
            var offsetLo = offset & ~0xFul; 
            while (offsetLo < offset + (uint) data.Length)
            {
                sb.AppendFormat("{0:X8}", offsetLo);
                for (int i = 0; i < 16; ++i, ++offsetLo)
                {
                    if (offsetLo < offset)
                        sb.Append("   ");
                    else
                    {
                        int j = (int) (offsetLo - offset);
                        if (j >= data.Length)
                            break;
                        sb.AppendFormat(" {0:X2}", data[j]);
                    }
                }
                sb.AppendLine();
            }
            Debug.Write(sb.ToString());
        }

        public string GetAsciizString(byte[] bytes, int iStart)
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
            Debug.Print("      reloff:  {0:X8} nreloc: {1:X} flags {2}", reloff, nreloc, RenderSectionFlags(flags));
            Debug.Print("      reserv1: {0:X8} reserv2: {1:X8}", reserved1, reserved2);


            var addr = Address.Ptr32(uAddr);
            var bytes = rdr.CreateNew(this.ldr.RawImage, offset);
            var name = string.Format("{0},{1}", segmentName, sectionName);

            AddSection(segmentMap, size, flags, reserved1, reserved2, protection, addr, bytes, name);
        }

        private string RenderSectionFlags(uint flags)
        {
            return $"{flags:X8}";
        }

        private void AddSection(SegmentMap segmentMap, uint size, uint flags, uint reserved1, uint reserved2, uint protection, Address addr, EndianImageReader bytes, string name)
        {
            AccessMode am = 0;
            if ((protection & VM_PROT_READ) != 0)
                am |= AccessMode.Read;
            if ((protection & VM_PROT_WRITE) != 0)
                am |= AccessMode.Write;
            if ((protection & VM_PROT_EXECUTE) != 0)
                am |= AccessMode.Execute;

            var mem = new ByteMemoryArea(addr, bytes.ReadBytes((uint) size));
            var machoSection = new MachOSection(name, addr, (SectionFlags) flags, reserved1, reserved2);
            var imageSection = new ImageSegment(name, mem, am);
            ldr.imageSections.Add(machoSection, imageSection);

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
            if (!rdr.TryReadUInt64(out ulong uAddr) ||
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

            var addr = Address.Ptr64(uAddr);
            var bytes = rdr.CreateNew(this.ldr.RawImage, offset);
            var name = string.Format("{0},{1}", segmentName, sectionName);
            AddSection(segmentMap, (uint)size, flags, reserved1, reserved2, protection, addr, bytes, name);
        }

        protected abstract void ParseMain(IProcessorArchitecture arch);

        void ParseSymtabCommand(IProcessorArchitecture arch)
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
                    var (msym, a) = ReadSymbol(strBytes, syms, i);
                    if (msym is not null)
                    {
                        var addr = a!.Value;
                        ldr.machoSymbols.Add(msym);
                        if (addr.ToLinear() != 0 &&
                            msym.Name != "radr://5614542")      // Ignore Apple's hack for radar bug 5614542
                        {
                            ldr.imageSymbols[addr] = ImageSymbol.Procedure(arch, addr, msym.Name);
                        }
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

        protected static string AdjustSymbolName(string name)
        {
            //$REVIEW: macho symbols seem to have '_' prefixes. Remove these if present.
            // Root cause of github issue #1130
            if (!string.IsNullOrEmpty(name) && name[0] == '_')
                name = name.Substring(1);
            return name;
        }

        protected MachOSymbol? GetSymbol(int i, List<uint> indirects)
        {
            var indsym = indirects[i];
            if (indsym < ldr.machoSymbols.Count)
            {
                return ldr.machoSymbols[(int)indsym];
            }
            else
                return null;
        }

        public abstract (MachOSymbol?, Address?) ReadSymbol(byte[] strBytes, EndianImageReader syms, uint i);

        void ParseDysymtabCommand(IProcessorArchitecture arch)
        {
            if (dysymtab is not null)
            {
                var listener = ldr.Services.RequireService<Reko.Core.Services.IEventListener>();
                listener.Warn("Multiple dysymtab commands found.");
            }
            this.dysymtab = new DySymtab();
            if (rdr.TryReadUInt32(out this.dysymtab.ilocalsym) &&
                rdr.TryReadUInt32(out this.dysymtab.nlocalsym) &&
                rdr.TryReadUInt32(out this.dysymtab.iextdefsym) &&
                rdr.TryReadUInt32(out this.dysymtab.nextdefsym) &&

                rdr.TryReadUInt32(out this.dysymtab.iundefsym) &&
                rdr.TryReadUInt32(out this.dysymtab.nundefsym) &&
                rdr.TryReadUInt32(out this.dysymtab.tocoff) &&
                rdr.TryReadUInt32(out this.dysymtab.ntoc) &&

                rdr.TryReadUInt32(out this.dysymtab.modtaboff) &&
                rdr.TryReadUInt32(out this.dysymtab.nmodtab) &&
                rdr.TryReadUInt32(out this.dysymtab.extrefsymoff) &&
                rdr.TryReadUInt32(out this.dysymtab.nextrefsyms) &&

                rdr.TryReadUInt32(out this.dysymtab.indirectsymoff) &&
                rdr.TryReadUInt32(out this.dysymtab.nindirectsyms) &&
                rdr.TryReadUInt32(out this.dysymtab.extreloff) &&
                rdr.TryReadUInt32(out this.dysymtab.nextrel) &&
                rdr.TryReadUInt32(out this.dysymtab.locreloff) &&
                rdr.TryReadUInt32(out this.dysymtab.nlocrel))
            {
                if (dysymtab.nundefsym > 0 && dysymtab.nindirectsyms > 0)
                {
                    var rdrIndirect = rdr.CreateNew(ldr.RawImage, dysymtab.indirectsymoff);
                    var indirects = LoadIndirectSymbols(rdrIndirect, dysymtab.nindirectsyms);
                    var lazySection = FindSectionByType(S_LAZY_SYMBOL_POINTERS);
                    if (lazySection is not null)
                    {
                        LoadImports(lazySection, indirects, arch);
                    }
                    var nonLazySection = FindSectionByType(S_NON_LAZY_SYMBOL_POINTERS);
                    if (nonLazySection is not null)
                    {
                        LoadImports(nonLazySection, indirects, arch);
                    }
                }
            }
            else
            {
                throw new BadImageFormatException("Failed to parse LC_DYSYMTAB section.");
            }
        }

        protected abstract void LoadImports(MachOSection msec, List<uint> indirects, IProcessorArchitecture arch);

        private List<uint> LoadIndirectSymbols(EndianImageReader rdr, uint nindirectsyms)
        {
            Debug.Print("    Loading indirect symbols");
            var indirects = new List<uint>();

            for (uint i = 0; i < nindirectsyms; ++i)
            {
                if (!rdr.TryReadUInt32(out uint indir))
                    return indirects;
                indirects.Add(indir);
                if ((int)indir < 0 || (int)indir >= ldr.machoSymbols.Count)
                {
                    Debug.Print(" *** Invalid indirect symbol {0:X}", indir);
                }
                else
                {
                    var sym = ldr.machoSymbols[(int) indir];
                    Debug.Print("      {0}: {1} {2:X2} {3:X2} {4:X8} {5}", i, indir, sym.n_type, sym.n_desc, sym.n_value, sym.Name);
                }
            }
            return indirects;
        }
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

    public class Loader32 : Parser
    {
        public Loader32(MachOLoader ldr, EndianByteImageReader rdr) : base(ldr, rdr)
        {
        }

        public override (mach_header_64, ArchSpecific) ParseHeader(Address? addrLoad)
        {
            if (rdr.TryReadUInt32(out uint magic) &&
                rdr.TryReadUInt32(out uint cputype) &&
                rdr.TryReadUInt32(out uint cpusubtype) &&
                rdr.TryReadUInt32(out uint filetype) &&
                rdr.TryReadUInt32(out uint ncmds) &&
                rdr.TryReadUInt32(out uint sizeofcmds) &&
                rdr.TryReadUInt32(out uint flags))
            {
                var specific = CreateArchitecture(cputype);
                var hdr = new mach_header_64
                {
                    magic = magic,
                    cputype = cputype,
                    cpusubtype = cpusubtype,
                    filetype = filetype,
                    ncmds = ncmds,
                    sizeofcmds = sizeofcmds,
                    flags = flags
                };
                return (hdr, specific);
            }
            throw new BadImageFormatException("Invalid Mach-O header.");
        }

        protected override void ParseMain(IProcessorArchitecture arch)
        {
            throw new NotImplementedException();
        }

        protected override void ParseUnixThread(uint cputype, IProcessorArchitecture arch)
        {
            if (rdr.TryReadUInt32(out uint flavor) &&
                rdr.TryReadUInt32(out uint count))
            {
                var data = rdr.ReadBytes(count * 4);

                DumpData(0, data);
                if (!mpCputypeToUnixthreadPc.TryGetValue(cputype, out uint uOffAddrStart))
                    throw new BadImageFormatException($"LC_PARSEUNIXTHREAD for CPU type {cputype} has not been implemented.");
                var rdrPtr = arch.Endianness.CreateImageReader(data, uOffAddrStart);
                if (rdrPtr.TryReadUInt32(out uint ep))
                {
                    base.ldr.entryPoints.Add(ImageSymbol.Procedure(arch, Address.Ptr32(ep)));
                }
            }
        }

        protected override void LoadImports(MachOSection section, List<uint> indirects, IProcessorArchitecture arch)
        {
            Debug.Print("    Loading imports from section {0} / res: {1:X8}", section.Name, section.Reserved1);
            var iseg = ldr.imageSections[section];
            var rdr = iseg.CreateImageReader(arch);
            var addr = rdr.Address;
            var tableIndex = section.Reserved1;
            int i = (int) tableIndex;
            while (rdr.TryReadUInt32(out uint uImport))
            {
                var msym = GetSymbol(i, indirects);
                if (msym is not null)
                {
                    Debug.Print("      {0}: {1:X8} {2}", addr, uImport, msym.Name);
                    var addrImport = Address.Ptr32(uImport);
                    var ptr = new Pointer(new CodeType(), arch.PointerType.BitSize);
                    var impSymbol = ImageSymbol.DataObject(
                        arch,
                        addr,
                        "__imp__" + msym.Name,
                        ptr);
                    ldr.imageSymbols[addr] = impSymbol;
                    ldr.program!.ImportReferences.Add(addr, new NamedImportReference(addrImport, "", msym.Name, SymbolType.ExternalProcedure));
                }
                addr = rdr.Address;
                ++i;
            }
        }

        public override (MachOSymbol?, Address?) ReadSymbol(byte[] strBytes, EndianImageReader syms, uint i)
        {
            if (syms.TryReadUInt32(out uint n_strx) &&
                syms.TryReadByte(out byte n_type) &&
                syms.TryReadByte(out byte n_sect) &&
                syms.TryReadUInt16(out ushort n_desc) &&
                syms.TryReadUInt32(out uint n_value))
            {
                var str = GetAsciizString(strBytes, (int) n_strx);
                str = Parser.AdjustSymbolName(str);
                var secName = n_sect < ldr.sections.Count ? ldr.sections[n_sect].Name : n_sect.ToString();
                MachOLoader.trace.Verbose("      {0,2}: {2,-8} {3}({4}) {5:X4} {6:X8} {7}",
                    i, n_strx, (Stab) n_type, secName,
                    n_sect, n_desc, n_value, str);
                var msym = new MachOSymbol(str, n_type, n_sect, n_desc, n_value);
                var addr = Address.Ptr32(n_value);
                return (msym, addr);
            }
            else
                return (null, null);
        }
    }

    public class Loader64 : Parser
    {
        public Loader64(MachOLoader ldr, EndianByteImageReader rdr)
            : base(ldr, rdr)
        {
        }

        public override (mach_header_64, ArchSpecific) ParseHeader(Address? addrLoad)
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
                var specific = CreateArchitecture(cputype);
                var header = new mach_header_64
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
                return (header, specific);
            }
            throw new BadImageFormatException("Invalid Mach-O header.");
        }

        protected override void ParseMain(IProcessorArchitecture arch)
        {
            var entryOffset = rdr.ReadUInt64();
            var addrEntry = base.segments["__TEXT"] + entryOffset;
            base.ldr.entryPoints.Add(ImageSymbol.Procedure(arch, Address.Ptr64(addrEntry)));
        }

        protected override void ParseUnixThread(uint cputype, IProcessorArchitecture arch)
        {
            if (rdr.TryReadUInt32(out uint flavor) &&
                rdr.TryReadUInt32(out uint count))
            {
                var data = rdr.ReadBytes(count * 4);

                DumpData(0, data);
                if (!mpCputypeToUnixthreadPc.TryGetValue(cputype, out uint uOffAddrStart))
                    throw new BadImageFormatException($"LC_PARSEUNIXTHREAD for CPU type {cputype} has not been implemented.");
                var rdrPtr = arch.Endianness.CreateImageReader(data, uOffAddrStart);
                if (rdrPtr.TryReadUInt64(out ulong ep))
                {
                    base.ldr.entryPoints.Add(ImageSymbol.Procedure(arch, Address.Ptr64(ep)));
                }
            }
        }

        protected override void LoadImports(MachOSection section, List<uint> indirects, IProcessorArchitecture arch)
        {
            Debug.Print("    Loading imports from section {0} / res: {1:X8}", section.Name, section.Reserved1);
            var iseg = ldr.imageSections[section];
            var rdr = iseg.CreateImageReader(arch);
            var addr = rdr.Address;
            var tableIndex = section.Reserved1;
            int i = (int) tableIndex;
            while (rdr.TryReadUInt64(out ulong uImport))
            {
                var msym = GetSymbol(i, indirects);
                if (msym is not null)
                {
                    Debug.Print("      {0}: {1:X8} {2}", addr, uImport, msym.Name);
                    var addrImport = Address.Ptr64(uImport);
                    var ptr = new Pointer(new CodeType(), arch.PointerType.BitSize);
                    var impSymbol = ImageSymbol.DataObject(
                        arch,
                        addr,
                        "__imp__" + msym.Name,
                        ptr);
                    ldr.imageSymbols[addr] = impSymbol;
                    ldr.program!.ImportReferences.Add(addr, new NamedImportReference(addrImport, "", msym.Name, SymbolType.ExternalProcedure));
                }
                addr = rdr.Address;
                ++i;
            }
        }

        public override (MachOSymbol?, Address?) ReadSymbol(byte[] strBytes, EndianImageReader syms, uint i)
        {
            if (syms.TryReadUInt32(out uint n_strx) &&
                syms.TryReadByte(out byte n_type) &&
                syms.TryReadByte(out byte n_sect) &&
                syms.TryReadUInt16(out ushort n_desc) &&
                syms.TryReadUInt64(out ulong n_value))
            {
                var str = GetAsciizString(strBytes, (int) n_strx);
                str = Parser.AdjustSymbolName(str);
                var secName = n_sect < ldr.sections.Count ? ldr.sections[n_sect].Name : n_sect.ToString();

                MachOLoader.trace.Verbose("      {0,2}: {2,-8} {3}({4}) {5:X4} {6:X16} {7}",
                    i, n_strx, (Stab) n_type, secName,
                    n_sect, n_desc, n_value, str);
                var msym = new MachOSymbol(str, n_type, n_sect, n_desc, n_value);
                var addr = Address.Ptr64(n_value);
                return (msym, addr);
            }
            else
                return (null, null);
        }
    }

    public enum Stab
    {
        UNDF = 0x0,
        ABS = 0x2,
        INDR  = 0xA,    // The symbol is defined to be the same as another symbol. The n_value field is an index into the string table specifying the name of the other symbol. When that symbol is linked, both this and the other symbol point to the same defined type and value.
        PBUD = 0xC,     // The symbol is undefined and the image is using a prebound value for the symbol. Set the n_sect field to NO_SECT.
        SECT = 0xE,     // The symbol is defined in the section number given in n_sect

        GSYM = 0x20,    // global symbol: name,,NO_SECT,type,0 
        FNAME = 0x22,   // procedure name (f77 kludge): name,,NO_SECT,0,0
        FUN	 = 0x24,    // procedure: name,,n_sect,linenumber,address */
        STSYM = 0x26,	//  static symbol: name,,n_sect,type,address 
        LCSYM = 0x28,	//  .lcomm symbol: name,,n_sect,type,address 
        MAIN = 0x2a,
        BNSYM  = 0x2e,	// begin nsect sym: 0,,n_sect,0,address 
        PC = 0x30,
        AST = 0x32,
        MAC_UNDEF = 0x3a,
        OPT = 0x3c,	    // emitted with gcc2_compiled and in gcc source 
        RSYM = 0x40,	// register sym: name,,NO_SECT,type,register 
        SLINE = 0x44,	// src line: 0,,n_sect,linenumber,address 
        DSLINE = 0x46,
        BSLINE = 0x48,
        ENSYM = 0x4e,	// end nsect sym: 0,,n_sect,0,address 
        SSYM = 0x60,	// structure elt: name,,NO_SECT,type,struct_offset 
        SO = 0x64,	    // source file name: name,,n_sect,0,address 
        OSO = 0x66,	    // object file name: name,,0,0,st_mtime 
        LSYM = 0x80,	// local sym: name,,NO_SECT,type,offset 
        BINCL = 0x82,	// include file beginning: name,,NO_SECT,0,sum 
        SOL = 0x84,	    // #included file name: name,,n_sect,0,address 
        PARAMS = 0x86,	// compiler parameters: name,,NO_SECT,0,0 
        VERSION= 0x88,	// compiler version: name,,NO_SECT,0,0 
        OLEVEL = 0x8A,	// compiler -O level: name,,NO_SECT,0,0 
        PSYM = 0xa0,	// parameter: name,,NO_SECT,type,offset 
        EINCL = 0xa2,	// include file end: name,,NO_SECT,0,0 
        ENTRY = 0xa4,	// alternate entry: name,,n_sect,linenumber,address 
        LBRAC = 0xc0,	// left bracket: 0,,NO_SECT,nesting level,address 
        EXCL = 0xc2,	// deleted include file: name,,NO_SECT,0,sum 
        RBRAC = 0xe0,	// right bracket: 0,,NO_SECT,nesting level,address 
        BCOMM = 0xe2,	// begin common: name,,NO_SECT,0,0 
        ECOMM = 0xe4,	// end common: name,,n_sect,0,0 
        ECOML = 0xe8,	// end common (local name): 0,,n_sect,0,address 
        LENG = 0xfe,	// second stab entry with length information 
    }

    // https://www.apriorit.com/dev-blog/225-dynamic-linking-mach-o
    public class DySymtab
    {
        public uint ilocalsym;
        public uint nlocalsym;
        public uint iextdefsym;
        public uint nextdefsym;

        public uint iundefsym;
        public uint nundefsym;
        public uint tocoff;
        public uint ntoc;

        public uint modtaboff;
        public uint nmodtab;
        public uint extrefsymoff;
        public uint nextrefsyms;

        public uint indirectsymoff;
        public uint nindirectsyms;
        public uint extreloff;
        public uint nextrel;
        public uint locreloff;
        public uint nlocrel;
    }
}
