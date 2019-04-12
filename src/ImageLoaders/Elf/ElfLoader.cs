#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.ImageLoaders.Elf.Relocators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    public abstract class ElfLoader
    {
        public const int ELFOSABI_NONE = 0x00;      // No specific ABI specified.
        public const int ELFOSABI_HPUX = 1;         // Hewlett-Packard HP-UX 
        public const int ELFOSABI_NETBSD = 2;       // NetBSD 
        public const int ELFOSABI_GNU = 3;          // GNU 
        public const int ELFOSABI_LINUX = 3;        // Linux  historical - alias for ELFOSABI_GNU 
        public const int ELFOSABI_SOLARIS = 6;      // Sun Solaris 
        public const int ELFOSABI_AIX = 7;          // AIX 
        public const int ELFOSABI_IRIX = 8;         // IRIX 
        public const int ELFOSABI_FREEBSD = 9;      // FreeBSD 
        public const int ELFOSABI_TRU64 = 10;       // Compaq TRU64 UNIX 
        public const int ELFOSABI_MODESTO = 11;     // Novell Modesto 
        public const int ELFOSABI_OPENBSD = 12;     // Open BSD 
        public const int ELFOSABI_OPENVMS = 13;     // Open VMS 
        public const int ELFOSABI_NSK = 14;         // Hewlett-Packard Non-Stop Kernel 
        public const int ELFOSABI_AROS = 15;        // Amiga Research OS 
        public const int ELFOSABI_FENIXOS = 16;     // The FenixOS highly scalable multi-core OS 
        public const int ELFOSABI_CLOUDABI = 17;    // Nuxi CloudABI
        public const int ELFOSABI_OPENVOS = 18;     // Stratus Technologies OpenVOS

        // Architecture-specific ABI's
        public const int ELFOSABI_ARM = 0x61;
        public const int ELFOSABI_CELL_LV2 = 0x66;     // PS/3 has this in its files
        public const int ELFOSABI_STANDALONE = 0xFF;   // A GNU extension for the MSP.

        // Endianness
        public const byte ELFDATA2LSB = 1;
        public const byte ELFDATA2MSB = 2;

        public const uint SHF_WRITE = 0x1;
        public const uint SHF_ALLOC = 0x2;
        public const uint SHF_EXECINSTR = 0x4;
        public const uint SHF_REKOCOMMON = 0x08000000;  // A hack until we determine what should happen with SHN_COMMON symbols

        public const int STT_NOTYPE = 0;			// Symbol table type: none
        public const int STT_FUNC = 2;				// Symbol table type: function
        public const int STT_SECTION = 3;
        public const int STT_FILE = 4;
        public const int STB_GLOBAL = 1;
        public const int STB_WEAK = 2;

        public const uint PF_R = 4;
        public const uint PF_W = 2;
        public const uint PF_X = 1;

        protected ElfMachine machine;
        protected ElfImageLoader imgLoader;
        protected byte endianness;
        protected IPlatform platform;
        protected byte[] rawImage;
        private SegmentMap segmentMap;

        protected ElfLoader(ElfImageLoader imgLoader, ushort machine, byte endianness) : this()
        {
            this.imgLoader = imgLoader;
            this.machine = (ElfMachine) machine;
            this.endianness = endianness;
            this.Architecture = CreateArchitecture(machine, endianness);
        }

        protected ElfLoader()
        {
            this.Symbols = new Dictionary<ulong, Dictionary<int, ElfSymbol>>();
            this.DynamicSymbols = new Dictionary<int, ElfSymbol>();
            this.Sections = new List<ElfSection>();
            this.Segments = new List<ElfSegment>();
            this.DynamicEntries = new Dictionary<long, ElfDynamicEntry>();
            this.Dependencies = new List<string>();
        }

        public IProcessorArchitecture Architecture { get; private set; }
        public IServiceProvider Services { get { return imgLoader.Services; } }
        public abstract Address DefaultAddress { get; }
        public abstract bool IsExecutableFile { get; }
        public List<ElfSection> Sections { get; private set; }
        public List<ElfSegment> Segments { get; private set; }
        public Dictionary<ulong, Dictionary<int, ElfSymbol>> Symbols { get; private set; }
        public Dictionary<int, ElfSymbol> DynamicSymbols { get; private set; }
        public Dictionary<long, ElfDynamicEntry> DynamicEntries { get; private set; }
        public List<string> Dependencies { get; private set; }

        public abstract ulong AddressToFileOffset(ulong addr);

        public abstract Address ComputeBaseAddress(IPlatform platform);

        public abstract Address CreateAddress(ulong uAddr);

        public abstract ElfObjectLinker CreateLinker();

        public abstract SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred);

        public abstract int LoadSegments();

        public abstract void LoadSectionHeaders();

        public abstract ElfRelocation LoadRelEntry(EndianImageReader rdr);

        public abstract ElfRelocation LoadRelaEntry(EndianImageReader rdr);

        public abstract ElfSymbol LoadSymbol(ulong offsetSymtab, ulong symbolIndex, ulong entrySize, ulong offsetStringTable);

        public abstract Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection);

        public abstract Address ReadAddress(EndianImageReader rdr);

        protected abstract int GetSectionNameOffset(uint idxString);

        public abstract void Dump(TextWriter writer);

        public abstract Address GetEntryPointAddress(Address addrBase);

        public abstract IEnumerable<ElfDynamicEntry> GetDynamicEntries(ulong offsetDynamic);

        /// <summary>
        /// Find the names of all shared objects this image depends on.
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetDependencyList(byte[] rawImage);


        public static AccessMode AccessModeOf(ulong sh_flags)
        {
            AccessMode mode = AccessMode.Read;
            if ((sh_flags & SHF_WRITE) != 0)
                mode |= AccessMode.Write;
            if ((sh_flags & SHF_EXECINSTR) != 0)
                mode |= AccessMode.Execute;
            return mode;
        }

        public ElfSegment GetSegmentByAddress(ulong uAddr)
        {
            return Segments.FirstOrDefault(s => s.IsValidAddress(uAddr));
        }

        public static SortedList<Address, MemoryArea> AllocateMemoryAreas(IEnumerable<Tuple<Address, uint>> segments)
        {
            var mems = new SortedList<Address, MemoryArea>();
            Address addr = null;
            Address addrEnd = null;
            foreach (var pair in segments)
            {
                if (addr == null)
                {
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else if (addrEnd < pair.Item1)
                {
                    var size = (uint)(addrEnd - addr);
                    mems.Add(addr, new MemoryArea(addr, new byte[size]));
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else
                {
                    addrEnd = Address.Max(addrEnd, pair.Item1 + pair.Item2);
                }
            }
            if (addr != null)
            {
                var size = (uint)(addrEnd - addr);
                mems.Add(addr, new MemoryArea(addr, new byte[size]));
            }
            return mems;
        }

        protected virtual IProcessorArchitecture CreateArchitecture(ushort machineType, byte endianness)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            string arch;
            string stackRegName = null;
            switch ((ElfMachine)machineType)
            {
            case ElfMachine.EM_NONE: return null; // No machine
            case ElfMachine.EM_SPARC:
            case ElfMachine.EM_SPARC32PLUS:
            case ElfMachine.EM_SPARCV9:
                arch = "sparc32"; break;
            case ElfMachine.EM_386: arch = "x86-protected-32"; break;
            case ElfMachine.EM_X86_64: arch = "x86-protected-64"; break;
            case ElfMachine.EM_68K: arch = "m68k"; break;
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-32";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = "mips-be-32";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The MIPS architecture does not support ELF endianness value {0}", endianness));
                }
                break;
            case ElfMachine.EM_PPC: arch = "ppc-be-32"; break;
            case ElfMachine.EM_PPC64: arch = "ppc-be-64"; break;
            case ElfMachine.EM_ARM: arch = "arm"; break;
            case ElfMachine.EM_AARCH64: arch = "arm-64"; break;
            case ElfMachine.EM_XTENSA: arch = "xtensa"; break;
            case ElfMachine.EM_AVR: arch = "avr8"; break;
            case ElfMachine.EM_RISCV: arch = "risc-v"; break;
            case ElfMachine.EM_MSP430: arch = "msp430"; break;
            case ElfMachine.EM_SH:
                arch = endianness == ELFDATA2LSB ? "superH-le" : "superH-be";
                // SuperH stack pointer is not defined by the architecture,
                // but by the application itself. It appears r15 has been
                // chosen by at least the NetBSD folks.
                stackRegName = "r15";
                break;
            case ElfMachine.EM_ALPHA:
                arch = "alpha";
                // Alpha has no architecture-defined stack pointer. 
                // Alpha-Linux uses r30.
                stackRegName = "r30";
                break;
            case ElfMachine.EM_S370:
            case ElfMachine.EM_S390: //$REVIEW: any pertinent differences?
                arch = "zSeries";
                break;

            default:
                throw new NotSupportedException(string.Format("Processor format {0} is not supported.", machineType));
            }
            var a = cfgSvc.GetArchitecture(arch);
            if (stackRegName != null)
            {
                a.StackRegister = a.GetRegister(stackRegName);
            }
            return a;
        }

 

        private static Dictionary<ElfSymbolType, SymbolType> mpSymbolType = new Dictionary<ElfSymbolType, SymbolType>
        {
            { ElfSymbolType.STT_FUNC, SymbolType.Procedure },
            { ElfSymbolType.STT_OBJECT, SymbolType.Data },
            { ElfSymbolType.STT_NOTYPE, SymbolType.Unknown },
        };

        public ImageSymbol CreateImageSymbol(ElfSymbol sym, bool isExecutable)
        {
            if (!isExecutable && sym.SectionIndex > 0 && sym.SectionIndex >= Sections.Count)
                return null;
            SymbolType? st = GetSymbolType(sym);
            if (st == null || st.Value == SymbolType.Unknown)
                return null;
            // If this is a relocatable file, the symbol value is 
            // an offset from the section's virtual address. 
            // If this is an executable file, the symbol value is
            // the virtual address.
            var addr = isExecutable
                ? platform.MakeAddressFromLinear(sym.Value)
                : Sections[(int) sym.SectionIndex].Address + sym.Value;

            var dt = GetSymbolDataType(sym);
            var imgSym = ImageSymbol.Create(
                st.Value,
                this.Architecture,
                addr,
                sym.Name,
                dt);
            imgSym.ProcessorState = Architecture.CreateProcessorState();
            return imgSym;
        }

        public static SymbolType? GetSymbolType(ElfSymbol sym)
        {
            if (!mpSymbolType.TryGetValue(sym.Type, out var st))
                return null;
            if (sym.SectionIndex == 0)
            {
                if (st != SymbolType.Procedure && st != SymbolType.Unknown)
                    return null;
                st = SymbolType.ExternalProcedure;
            }
            return st;
        }

        private DataType GetSymbolDataType(ElfSymbol sym)
        {
            if (sym.Type == ElfSymbolType.STT_FUNC)
            {
                return new FunctionType();
            }
            else if (sym.Size == 0)
            {
                return new UnknownType();
        }
            else
            {
                return PrimitiveType.CreateWord(DataType.BitsPerByte * (int)sym.Size);
            }
        }

        /// <summary>
        /// Guess the size of an area by scanning the dynamic records and using the ones that
        /// look like pointers. This is not 100% safe, but the worst that can happen is that
        /// we don't get all the area.
        /// </summary>
        /// <remarks>
        /// The ELF format sadly is missing a DT_SYMSZ, whi
        /// </remarks>
        /// <param name="addrStart"></param>
        /// <param name="dynSeg">The dynamic segment.</param>
        /// <returns></returns>
        public ulong GuessAreaEnd(ulong addrStart, ElfSegment dynSeg)
        {
            var seg = GetSegmentByAddress(addrStart);
            if (seg == null)
                return 0;

            var addrEnd = 0ul;
            foreach (var de in DynamicEntries.Values)
            {
                if (de.UValue <= addrStart)
                    continue;
                var tagInfo = de.GetTagInfo(machine);
                if (tagInfo?.Format == DtFormat.Address)
                {
                    // This might be a pointer.
                    addrEnd = addrEnd == 0 ? de.UValue : Math.Min(addrEnd, de.UValue);
                }
            }
            return addrEnd;
        }


        public IPlatform LoadPlatform(byte osAbi, IProcessorArchitecture arch)
        {
            string envName;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            switch (osAbi)
            {
            case ELFOSABI_NONE: // Unspecified ABI
            case ELFOSABI_ARM:
            case ELFOSABI_STANDALONE:
                envName = "elf-neutral";
                break;
            case ELFOSABI_CELL_LV2: // PS/3
                envName = "elf-cell-lv2";
                break;
            case ELFOSABI_LINUX:
                envName = "linux";      //$TODO: create a linux platform
                break;
            default:
                throw new NotSupportedException(string.Format("Unsupported ELF ABI 0x{0:X2}.", osAbi));
            }
            var env = cfgSvc.GetEnvironment(envName);
            this.platform = env.Load(Services, arch);
            return platform;
        }

        public virtual ElfRelocator CreateRelocator(ElfMachine machine, SortedList<Address, ImageSymbol> symbols)
        {
            throw new NotSupportedException(
                string.Format("Relocator for architecture {0} not implemented yet.",
                machine));
        }

        public Program LoadImage(IPlatform platform, byte[] rawImage)
        {
            Debug.Assert(platform != null);
            this.platform = platform;
            this.rawImage = rawImage;
            var addrPreferred = ComputeBaseAddress(platform);
            Dump();
            this.segmentMap = LoadImageBytes(platform, rawImage, addrPreferred);
            LoadDynamicSegment();
            var program = new Program(segmentMap, platform.Architecture, platform);
            return program;
        }

        /// <summary>
        /// Loads the dynamic segment of the executable.
        /// </summary>
        /// <remarks>
        /// The ELF standard specifies that there will be at most 1 dynamic segment
        /// in an executable binary.
        /// </remarks>
        public void LoadDynamicSegment()
        {
            var dynSeg = Segments.FirstOrDefault(p => p.p_type == ProgramHeaderType.PT_DYNAMIC);
            if (dynSeg == null)
                return;
            var dynEntries = GetDynamicEntries(dynSeg.p_offset).ToList();
            var deStrTab = dynEntries.FirstOrDefault(de => de.Tag == ElfDynamicEntry.DT_STRTAB);
            if (deStrTab == null)
            {
                //$REVIEW: is missing a string table worth a warning?
                return;
            }
            var offStrtab = AddressToFileOffset(deStrTab.UValue);
            foreach (var de in dynEntries)
            {
                if (de.Tag == ElfDynamicEntry.DT_NEEDED)
                {
                    Dependencies.Add(ReadAsciiString(offStrtab + de.UValue));
                }
                else
                {
                    DynamicEntries[de.Tag] = de;
                }
            }
        }

        public SortedList<Address, ImageSymbol> CreateSymbolDictionaries(bool isExecutable)
        {
            var imgSymbols = new SortedList<Address, ImageSymbol>();
            foreach (var sym in Symbols.Values.SelectMany(seg => seg.Values).OrderBy(s => s.Value))
            {
                var imgSym = CreateImageSymbol(sym, isExecutable);
                if (imgSym == null || imgSym.Address.ToLinear() == 0)
                    continue;
                    imgSymbols[imgSym.Address] = imgSym;
                }
            return imgSymbols;
        }

        public EndianImageReader CreateReader(ulong fileOffset)
        {
            return imgLoader.CreateReader(fileOffset);
        }

        public ImageWriter CreateWriter(uint fileOffset)
        {
            return imgLoader.CreateWriter(fileOffset);
        }


        /// <summary>
        /// The GOT table contains an array of pointers. Some of these
        /// pointers may be pointing to the symbols in the symbol table(s).
        /// </summary>
        /// <remarks>
        /// Assumes that the binary has a valid .got section present. If the 
        /// .got sections has been stripped away, we will not recover any 
        /// GOT entries.
        /// <para>
        /// Because of this assumption, we use it as a fall back. If the 
        /// ELF specification for a particular processor specifies how
        /// to obtain GOT pointers in a safe way, then override the 
        /// ElfRelocatior.LocateGotPointers and do the right thing there.
        /// </para>
        /// </remarks>
        public void LocateGotPointers(Program program, SortedList<Address, ImageSymbol> symbols)
        {
            // Locate the GOT. It's fully possible that the binary doesn't have a 
            // .got section.
            var got = program.SegmentMap.Segments.Values.FirstOrDefault(s => s.Name == ".got");
            if (got == null)
                return;

            var gotStart = got.Address;
            var gotEnd = got.EndAddress;
            ConstructGotEntries(program, symbols, gotStart, gotEnd, false);
        }

        /// <summary>
        /// Scans the addresses between <paramref name="gotStart"/> and <paramref name="gotEnd"/>, 
        /// in the GOT, reading successive pointers. If a pointer coincides with the address of 
        /// a symbol, generate a GOT symbol and an import reference.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="symbols"></param>
        /// <param name="gotStart"></param>
        /// <param name="gotEnd"></param>
        public void ConstructGotEntries(Program program, SortedList<Address, ImageSymbol> symbols, Address gotStart, Address gotEnd, bool makeGlobals)
        {
            DebugEx.Verbose(ElfImageLoader.trace, "== Constructing GOT entries ==");
            var rdr = program.CreateImageReader(program.Architecture, gotStart);
            while (rdr.Address < gotEnd)
            {
                var addrGot = rdr.Address;
                var addrSym = ReadAddress(rdr);
                if (addrSym == null)
                    break;
                if (symbols.TryGetValue(addrSym, out ImageSymbol symbol))
                {
                    // This GOT entry is a known symbol!
                    if (symbol.Type == SymbolType.Procedure || symbol.Type == SymbolType.ExternalProcedure)
                    {
                        ImageSymbol gotSym = CreateGotSymbol(addrGot, symbol.Name);
                        symbols[addrGot] = gotSym;
                        DebugEx.Verbose(ElfImageLoader.trace, "{0}+{1:X4}: Found GOT entry {2}, referring to symbol at {3}",
                            gotStart, addrGot - gotStart, gotSym, symbol);
                        if (symbol.Type == SymbolType.ExternalProcedure)
                        {
                            program.ImportReferences.Add(
                                addrGot,
                                new NamedImportReference(
                                    addrGot,
                                    null,
                                    symbol.Name,
                                    symbol.Type));
                        }
                    }
                }
                else if (makeGlobals)
                {
                    // This GOT entry has no corresponding symbol. It's likely a global
                    // variable with no name.
                    ImageSymbol gotDataSym = ImageSymbol.Create(SymbolType.Data, this.Architecture, addrGot);
                    DebugEx.Verbose(ElfImageLoader.trace, "{0}+{1:X4}: GOT entry with no symbol, assuming local data {2}",
                        gotStart, addrGot - gotStart, addrGot);
                    program.ImportReferences.Add(
                        addrGot,
                        new NamedImportReference(
                            addrGot,
                            null,
                            null,
                            gotDataSym.Type));
                }
            }
        }

        public ImageSymbol CreateGotSymbol(Address addrGot, string name)
        {
            //$TODO: look up function signature.
            int size = Architecture.PointerType.Size;
            int bitSize = Architecture.PointerType.BitSize;
            return ImageSymbol.DataObject(Architecture, addrGot, name + "_GOT", new Pointer(new CodeType(), bitSize));
        }

        public IEnumerable<ElfSymbol> GetAllSymbols()
        {
            return Symbols.Values.SelectMany(s => s.Values);
        }

        public ElfSection GetSectionByIndex(uint shidx)
        {
            if (0 <= shidx && shidx < Sections.Count)
            {
                return Sections[(int)shidx];
            }
            else
            {
                return null;
            }
        }

        public ElfSection GetSectionInfoByName(string sectionName)
        {
            return Sections.FirstOrDefault(s => s.Name == sectionName);
        }

        protected string ReadSectionName(uint idxString)
        {
            ulong offset = (ulong)GetSectionNameOffset(idxString);
            return ReadAsciiString(offset);
        }

        public string GetSectionName(ushort st_shndx)
        {
            Debug.Assert(Sections != null);
            if (st_shndx < 0xFF00)
            {
                if (st_shndx < Sections.Count)
                return Sections[st_shndx].Name;
                else
                    return $"?section{st_shndx}?";
            }
            else
            {
                switch (st_shndx)
                {
                case 0xFFF1: return "SHN_ABS";
                case 0xFFF2: return "SHN_COMMON";
                default: return st_shndx.ToString("X4");
                }
            }
        }

        public string GetStrPtr(ElfSection strSection, ulong offset)
        {
            if (strSection == null)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentNullException(nameof(strSection));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(strSection.FileOffset + offset);
        }

        public void Dump()
        {
            var sw = new StringWriter();
            Dump(sw);
            Debug.Print(sw.ToString());
        }


        protected string DumpShFlags(ulong shf)
        {
            return string.Format("{0}{1}{2}",
                ((shf & SHF_EXECINSTR) != 0) ? "x" : " ",
                ((shf & SHF_ALLOC) != 0) ? "a" : " ",
                ((shf & SHF_WRITE) != 0) ? "w" : " ");
        }


        protected ImageSymbol EnsureEntryPoint(List<ImageSymbol> entryPoints, SortedList<Address, ImageSymbol> symbols, Address addr)
        {
            if (addr == null)
                return null;
            if (!symbols.TryGetValue(addr, out ImageSymbol ep))
            {
                ep = ImageSymbol.Procedure(this.Architecture, addr);
                ep.ProcessorState = Architecture.CreateProcessorState();
                symbols.Add(addr, ep);
            }
            entryPoints.Add(ep);
            return ep;
        }

        /// <summary>
        /// Fetches the <paramref name="i"/>'th symbol from the symbol table at 
        /// file offset <paramref name="offSymtab" />, whose elements are of size
        /// <paramref name="symentrysize"/> and whose strings are located in the
        /// string table at file offset <paramref name="offStrtab"/>.
        /// </summary>
        /// <remarks>
        /// This method caches symbol lookups in the Symbols property.
        /// </remarks>
        /// <param name="offSymtab">File offset of the symbol table</param>
        /// <param name="i">Index of the symbol</param>
        /// <param name="symentrysize"></param>
        /// <param name="offStrtab"></param>
        /// <returns>The cached symbol.</returns>
        public ElfSymbol EnsureSymbol(ulong offSymtab, int i, ulong symentrysize, ulong offStrtab)
        {
            if (!Symbols.TryGetValue(offSymtab, out var symList))
            {
                symList = new Dictionary<int, ElfSymbol>();
                Symbols.Add(offSymtab, symList);
            }
            if (!symList.TryGetValue(i, out var sym))
            {
                sym = LoadSymbol(offSymtab, (ulong)i, symentrysize, offStrtab);
                symList.Add(i, sym);
            }
            return sym;
            }

        protected bool IsLoadable(ulong p_pmemsz, ProgramHeaderType p_type)
        {
            if (p_pmemsz == 0)
                return false;
            return (p_type == ProgramHeaderType.PT_LOAD ||
                    p_type == ProgramHeaderType.PT_DYNAMIC);
        }

        public void LoadSymbolsFromSections()
        {
            var symbolSections = Sections.Where(s =>
                s.Type == SectionHeaderType.SHT_SYMTAB ||
                s.Type == SectionHeaderType.SHT_DYNSYM)
                .ToList();
                foreach (var section in symbolSections)
            {
                var symtab = LoadSymbolsSection(section);
                Symbols[section.FileOffset] = symtab;
                if (section.Type == SectionHeaderType.SHT_DYNSYM)
                {
                    this.DynamicSymbols = symtab;
            }
        }
        }

        public string ReadAsciiString(ulong fileOffset)
        {
            var bytes = this.rawImage;
            if (fileOffset >= (ulong)bytes.Length)
                return "";
            int u = (int)fileOffset;
            while (bytes[u] != 0)
            {
                ++u;
        }
            return Encoding.ASCII.GetString(bytes, (int) fileOffset, u - (int) fileOffset);
        }


        public RelocationResults Relocate(Program program, Address addrLoad)
        {
            var symbols = CreateSymbolDictionaries(IsExecutableFile);
            var relocator = CreateRelocator(this.machine, symbols);
            relocator.Relocate(program);
            relocator.LocateGotPointers(program, symbols);
            var entryPoints = new List<ImageSymbol>();
            var addrEntry = GetEntryPointAddress(addrLoad);
            var symEntry = EnsureEntryPoint(entryPoints, symbols, addrEntry);
            var addrMain = relocator.FindMainFunction(program, addrEntry);
            var symMain = EnsureEntryPoint(entryPoints, symbols, addrMain);
            symbols = symbols.Values.Select(relocator.AdjustImageSymbol).ToSortedList(s => s.Address);
            entryPoints = entryPoints.Select(relocator.AdjustImageSymbol).ToList();
            return new RelocationResults(entryPoints, symbols);
        }

        /// <summary>
        /// Hack off the @@GLIBC_... suffixes from symbols. 
        /// They might become useful at some later stage, but for now
        /// they just mangle the names unnecessarily.
        /// </summary>
        public static string RemoveModuleSuffix(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            int i = s.IndexOf("@@");
            if (i < 0)
                return s;
            return s.Remove(i);
        }

        protected string rwx(uint flags)
        {
            var sb = new StringBuilder();
            sb.Append((flags & 4) != 0 ? 'r' : '-');
            sb.Append((flags & 2) != 0 ? 'w' : '-');
            sb.Append((flags & 1) != 0 ? 'x' : '-');
            return sb.ToString();
    }
    }

    public class ElfLoader64 : ElfLoader
    {
        private readonly byte osAbi;

        public ElfLoader64(ElfImageLoader imgLoader, Elf64_EHdr elfHeader, byte[] rawImage, byte osAbi, byte endianness)
            : base(imgLoader, elfHeader.e_machine, endianness)
        {
            this.Header64 = elfHeader;
            this.osAbi = osAbi;
            base.rawImage = rawImage;
        }

        public Elf64_EHdr Header64 { get; set; }

        public override Address DefaultAddress { get { return Address.Ptr64(0x8048000); } }

        public override bool IsExecutableFile { get { return Header64.e_type != ElfImageLoader.ET_REL; } }

        public override ulong AddressToFileOffset(ulong addr)
        {
            foreach (var ph in Segments)
            {
                if (ph.p_vaddr <= addr && addr < ph.p_vaddr + ph.p_filesz)
                    return (addr - ph.p_vaddr) + ph.p_offset;
            }
            return ~0ul;
        }

        public override Address ComputeBaseAddress(IPlatform platform)
        {
            ulong uBaseAddr = Segments
                .Where(ph => ph.p_vaddr > 0 && ph.p_filesz > 0)
                .Min(ph => ph.p_vaddr);
            return platform.MakeAddressFromLinear(uBaseAddr);
        }

        public override Address CreateAddress(ulong uAddr)
        {
            return Address.Ptr64(uAddr);
        }

        protected override IProcessorArchitecture CreateArchitecture(ushort machineType, byte endianness)
        {
            string arch;
            switch ((ElfMachine)machineType)
            {
            case ElfMachine.EM_MIPS:
                //$TODO: detect release 6 of the MIPS architecture. 
                // would be great to get our sweaty little hands on
                // such a binary.
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-64";
                }
                else if (endianness == ELFDATA2MSB)
                {
                    arch = "mips-be-64";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The MIPS architecture does not support ELF endianness value {0}", endianness));
                }
                break;
            default:
                return base.CreateArchitecture(machineType, endianness);
            }
            var cfgSvc = Services.RequireService<IConfigurationService>();
            return cfgSvc.GetArchitecture(arch);
        }

        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker64(this, Architecture, rawImage);
        }

        private ImageSegmentRenderer CreateRenderer64(ElfSection shdr)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer64(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer64(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer64(this, shdr);
            default: return null;
            }
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine, SortedList<Address, ImageSymbol> symbols)
        {
            switch (machine)
            {
            case ElfMachine.EM_AARCH64: return new Arm64Relocator(this, symbols);
            case ElfMachine.EM_X86_64: return new x86_64Relocator(this, symbols);
            case ElfMachine.EM_PPC64: return new PpcRelocator64(this, symbols);
            case ElfMachine.EM_MIPS: return new MipsRelocator64(this, symbols);
            case ElfMachine.EM_RISCV: return new RiscVRelocator64(this, symbols);
            case ElfMachine.EM_ALPHA: return new AlphaRelocator(this, symbols);
            case ElfMachine.EM_S390: return new zSeriesRelocator(this, symbols);
            }
            return base.CreateRelocator(machine, symbols);
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header64.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.Address,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection != null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection != null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in Segments)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6} {7:X8} p_align:{8:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    rwx(ph.p_flags & 7),
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress(platform));
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList(imgLoader.RawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                // DumpRela(sh);
            }
        }

        public override List<string> GetDependencyList(byte[] rawImage)
        {
            return Dependencies;
        }

        public override IEnumerable<ElfDynamicEntry> GetDynamicEntries(ulong offsetDynamic)
            {
            var rdr = imgLoader.CreateReader(offsetDynamic);
            for (; ; )
            {
                var dyn = new Elf64_Dyn();
                if (!rdr.TryReadInt64(out dyn.d_tag))
                    break;
                if (dyn.d_tag == ElfDynamicEntry.DT_NULL)
                    break;
                if (!rdr.TryReadInt64(out long val))
                    break;
                dyn.d_val = val;
                yield return new ElfDynamicEntry(dyn.d_tag, dyn.d_ptr); ;
            }
        }


        public override Address GetEntryPointAddress(Address addrBase)
        {
            Address addr = null;
            //$REVIEW: should really have a subclassed "Ps3ElfLoader"
            if (osAbi == ElfLoader.ELFOSABI_CELL_LV2)
            {
                // The Header64.e_entry field actually points to a 
                // "function descriptor" consisiting of two 32-bit 
                // pointers.
                var rdr = imgLoader.CreateReader(Header64.e_entry - addrBase.ToLinear());
                if (rdr.TryReadUInt32(out uint uAddr))
                    addr = Address.Ptr32(uAddr);
            }
            else
            {
                addr = Address.Ptr64(Header64.e_entry);
            }
            return addr;
        }

        internal ElfSection GetSectionInfoByAddr64(ulong r_offset)
        {
            return
                (from sh in this.Sections
                 let addr = sh.Address != null ? sh.Address.ToLinear() : 0
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(uint idxString)
        {
            return (int)(Sections[Header64.e_shstrndx].FileOffset + idxString);
        }

        public string GetSymbol64(int iSymbolSection, ulong symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbol64(iSymbolSection, symbolNo);
        }

        public string GetSymbol64(ElfSection symSection, ulong symbolNo)
        {
            var strSection = symSection.LinkedSection;
            ulong offset = symSection.FileOffset + symbolNo * symSection.EntrySize;
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt64(out offset);
            return GetStrPtr(symSection.LinkedSection, (uint)offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                Segments
                    .Where(p => IsLoadable(p.p_pmemsz, p.p_type))
                    .Select(p => Tuple.Create(
                        platform.MakeAddressFromLinear(p.p_vaddr),
                        (uint)p.p_pmemsz)));
            foreach (var ph in Segments)
            {
                DebugEx.Inform(ElfImageLoader.trace, "ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_pmemsz, ph.p_type))
                    continue;
                var vaddr = platform.MakeAddressFromLinear(ph.p_vaddr);
                segMap.TryGetLowerBound(vaddr, out var mem);
                if (ph.p_filesz > 0)
                    Array.Copy(
                        rawImage,
                        (long)ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long)ph.p_filesz);
            }
            var segmentMap = new SegmentMap(addrPreferred);
            if (Sections.Count > 0)
            {
                foreach (var section in Sections)
                {
                    if (section.Name == null || section.Address == null)
                        continue;
                    if (segMap.TryGetLowerBound(section.Address, out var mem) &&
                        section.Address < mem.EndAddress)
                    {
                        AccessMode mode = AccessModeOf(section.Flags);
                        var seg = segmentMap.AddSegment(new ImageSegment(
                            section.Name,
                            section.Address,
                            mem, mode)
                        {
                            Size = (uint)section.Size
                        });
                        seg.Designer = CreateRenderer64(section);
                    }
                    else
                    {
                        //$TODO: warn
                    }
                }
            }
            else
            {
                // There are stripped ELF binaries with 0 sections. If we have one
                // create a pseudo-section from the segMap.
                foreach (var segment in segMap)
                {
                    var imgSegment = new ImageSegment(
                        segment.Value.BaseAddress.GenerateName("seg", ""),
                        segment.Value,
                        AccessMode.ReadExecute)        //$TODO: writeable segments.
                    {
                        Size = (uint)segment.Value.Length,
                    };
                    segmentMap.AddSegment(imgSegment);
                }
            }
            segmentMap.DumpSections();
            return segmentMap;
        }

        public override int LoadSegments()
        {
            var rdr = imgLoader.CreateReader(Header64.e_phoff);
            for (int i = 0; i < Header64.e_phnum; ++i)
            {
                var sSeg = Elf64_PHdr.Load(rdr);
                Segments.Add(new ElfSegment
                {
                    p_type = sSeg.p_type,
                    p_offset = sSeg.p_offset,
                    p_vaddr = sSeg.p_vaddr,
                    p_paddr = sSeg.p_paddr,
                    p_filesz = sSeg.p_filesz,
                    p_pmemsz = sSeg.p_pmemsz,
                    p_flags = sSeg.p_flags,
                    p_align = sSeg.p_align,
                });
            }
            return Segments.Count;
        }

        public override ElfRelocation LoadRelEntry(EndianImageReader rdr)
        {
            throw new NotImplementedException();
        }

        public override ElfRelocation LoadRelaEntry(EndianImageReader rdr)
        {
            var rela = Elf64_Rela.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                Addend = rela.r_addend,
                SymbolIndex = (int)(rela.r_info >> 32),
            };
        }


        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header64.e_shoff);
            for (uint i = 0; i < Header64.e_shnum; ++i)
            {
                var shdr = Elf64_SHdr.Load(rdr);
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = shdr.sh_addr != 0
                        ? platform.MakeAddressFromLinear(shdr.sh_addr)
                        : null,
                    FileOffset = shdr.sh_offset,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                Sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < Sections.Count; ++i)
            {
                var section = Sections[i];
                section.Name = ReadSectionName(inames[i]);

                ElfSection linkSection = null;
                ElfSection relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(links[i]);
                    relSection = GetSectionByIndex(infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
        }

        public override ElfSymbol LoadSymbol(ulong offsetSymtab,  ulong symbolIndex, ulong entrySize, ulong offsetStringTable)
        {
            var rdr = CreateReader(offsetSymtab + entrySize * symbolIndex);
            var sym = Elf64_Sym.Load(rdr);
            return new ElfSymbol
            {
                Name = RemoveModuleSuffix(ReadAsciiString(offsetStringTable + sym.st_name)),
                Type = (ElfSymbolType)(sym.st_info & 0xF),
                Bind = sym.st_info >> 4,
                SectionIndex = sym.st_shndx,
                Value = sym.st_value,
                Size = sym.st_size,
            };
        }

        public override Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            //Debug.Print("Symbols");
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new Dictionary<int,ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf64_Sym.Load(rdr);
                //Debug.Print("  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                //    i,
                //    RemoveGlibcSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name)),
                //    (ElfSymbolType)(sym.st_info & 0xF),
                //    sym.st_shndx,
                //    GetSectionName(sym.st_shndx),
                //    sym.st_value,
                //    sym.st_size);
                symbols.Add(
                    (int)i,
                    new ElfSymbol
                {
                    Name = RemoveModuleSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name)),
                    Type = (ElfSymbolType)(sym.st_info & 0xF),
                    SectionIndex = sym.st_shndx,
                    Value = sym.st_value,
                    Size = sym.st_size,
                });
            }
            return symbols;
        }

        public override Address ReadAddress(EndianImageReader rdr)
        {
            if (!rdr.TryReadUInt64(out ulong uAddrSym))
                return null;

            var addr = Address.Ptr64(uAddrSym);
            return addr;
        }

    }

    public class ElfLoader32 : ElfLoader
    {
        public ElfLoader32(ElfImageLoader imgLoader, Elf32_EHdr header32, byte[] rawImage, byte endianness)
            : base(imgLoader, header32.e_machine, endianness)
        {
            this.Header = header32 ?? throw new ArgumentNullException(nameof(header32));
            this.rawImage = rawImage;
        }

        public ElfLoader32() : base() { }

        public Elf32_EHdr Header { get; private set; }
        public override Address DefaultAddress { get { return Address.Ptr32(0x8048000); } }

        public override bool IsExecutableFile { get { return Header.e_type != ElfImageLoader.ET_REL; } }

        public static int ELF32_R_SYM(int info) { return ((info) >> 8); }
        public static int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        public static int ELF32_ST_TYPE(int i) { return ((i) & 0x0F); }
        public static byte ELF32_ST_INFO(int b, ElfSymbolType t) { return (byte)(((b) << 4) + ((byte)t & 0xF)); }

        public override ulong AddressToFileOffset(ulong addr)
        {
            foreach (var ph in Segments)
            {
                if (ph.p_vaddr <= addr && addr < ph.p_vaddr + ph.p_filesz)
                    return addr - ph.p_vaddr;
            }
            return ~0ul;
            }

        public override Address ComputeBaseAddress(IPlatform platform)
                {
            if (Segments.Count == 0)
                return Address.Ptr32(0);

            return Address.Ptr32(
                Segments
                .Where(ph => ph.p_filesz > 0)
                .Min(ph => (uint)ph.p_vaddr));
                }

        public override Address CreateAddress(ulong uAddr)
            {
            return Address.Ptr32((uint)uAddr);
            }


        public override ElfObjectLinker CreateLinker()
        {
            return new ElfObjectLinker32(this, Architecture, rawImage);
        }

        public override ElfRelocator CreateRelocator(ElfMachine machine, SortedList<Address, ImageSymbol> imageSymbols)
        {
            switch (machine)
            {
            case ElfMachine.EM_386: return new x86Relocator(this, imageSymbols);
            case ElfMachine.EM_ARM: return new ArmRelocator(this, imageSymbols);
            case ElfMachine.EM_MIPS: return new MipsRelocator(this, imageSymbols);
            case ElfMachine.EM_MSP430: return new Msp430Relocator(this, imageSymbols);
            case ElfMachine.EM_PPC: return new PpcRelocator(this, imageSymbols);
            case ElfMachine.EM_SPARC32PLUS:
            case ElfMachine.EM_SPARC: return new SparcRelocator(this, imageSymbols);
            case ElfMachine.EM_XTENSA: return new XtensaRelocator(this, imageSymbols);
            case ElfMachine.EM_68K: return new M68kRelocator(this, imageSymbols);
            case ElfMachine.EM_AVR: return new AvrRelocator(this, imageSymbols);
            case ElfMachine.EM_SH: return new SuperHRelocator(this, imageSymbols);
            }
            return base.CreateRelocator(machine, imageSymbols);
        }

        public ImageSegmentRenderer CreateRenderer(ElfSection shdr, ElfMachine machine)
        {
            switch (shdr.Type)
            {
            case SectionHeaderType.SHT_DYNAMIC:
                return new DynamicSectionRenderer32(this, shdr, machine);
            case SectionHeaderType.SHT_REL:
                return new RelSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_RELA:
                return new RelaSegmentRenderer(this, shdr);
            case SectionHeaderType.SHT_SYMTAB:
            case SectionHeaderType.SHT_DYNSYM:
                return new SymtabSegmentRenderer32(this, shdr);
            default: return null;
            }
        }

        public override void Dump(TextWriter writer)
        {
            writer.WriteLine("Entry: {0:X}", Header.e_entry);
            writer.WriteLine("Sections:");
            foreach (var sh in Sections)
            {
                writer.WriteLine("{0,-18} sh_type: {1,-12} sh_flags: {2,-4} sh_addr; {3:X8} sh_offset: {4:X8} sh_size: {5:X8} sh_link: {6,-18} sh_info: {7,-18} sh_addralign: {8:X8} sh_entsize: {9:X8}",
                    sh.Name,
                    sh.Type,
                    DumpShFlags(sh.Flags),
                    sh.Address,
                    sh.FileOffset,
                    sh.Size,
                    sh.LinkedSection != null ? sh.LinkedSection.Name : "",
                    sh.RelocatedSection != null ? sh.RelocatedSection.Name : "",
                    sh.Alignment,
                    sh.EntrySize);
            }
            writer.WriteLine();
            writer.WriteLine("Program headers:");
            foreach (var ph in Segments)
            {
                writer.WriteLine("p_type:{0,-12} p_offset: {1:X8} p_vaddr:{2:X8} p_paddr:{3:X8} p_filesz:{4:X8} p_pmemsz:{5:X8} p_flags:{6} {7:X8} p_align:{8:X8}",
                    ph.p_type,
                    ph.p_offset,
                    ph.p_vaddr,
                    ph.p_paddr,
                    ph.p_filesz,
                    ph.p_pmemsz,
                    rwx(ph.p_flags & 7),
                    ph.p_flags,
                    ph.p_align);
            }
            writer.WriteLine("Base address: {0:X8}", ComputeBaseAddress(platform));
            writer.WriteLine();
            writer.WriteLine("Dependencies");
            foreach (var dep in GetDependencyList(rawImage))
            {
                writer.WriteLine("  {0}", dep);
            }

            writer.WriteLine();
            writer.WriteLine("Relocations");
            foreach (var sh in Sections.Where(sh => sh.Type == SectionHeaderType.SHT_RELA))
            {
                DumpRela(sh);
            }
        }

        private void DumpRela(ElfSection sh)
        {
            var entries = sh.Size / sh.EntrySize;
            var symtab = sh.LinkedSection;
            var rdr = imgLoader.CreateReader(sh.FileOffset);
            for (ulong i = 0; i < entries; ++i)
            {
                if (!rdr.TryReadUInt32(out uint offset))
                    return;
                if (!rdr.TryReadUInt32(out uint info))
                    return;
                if (!rdr.TryReadInt32(out int addend))
                    return;

                uint sym = info >> 8;
                string symStr = GetStrPtr(symtab, sym);
                DebugEx.Verbose(ElfImageLoader.trace, "  RELA {0:X8} {1,3} {2:X8} {3:X8} {4}", offset, info & 0xFF, sym, addend, symStr);
            }
        }

        public override List<string> GetDependencyList(byte[] rawImage)
        {
            return Dependencies;
            }

        public override IEnumerable<ElfDynamicEntry> GetDynamicEntries(ulong offsetDynamic)
            {
            var rdr = imgLoader.CreateReader(offsetDynamic);
            for (; ; )
            {
                var dyn = new Elf32_Dyn();
                if (!rdr.TryReadInt32(out dyn.d_tag))
                    break;
                if (dyn.d_tag == ElfDynamicEntry.DT_NULL)
                    break;
                if (!rdr.TryReadInt32(out int val))
                    break;
                dyn.d_val = val;
                yield return new ElfDynamicEntry(dyn.d_tag, dyn.d_ptr);
            }
        }


        public override Address GetEntryPointAddress(Address addrBase)
        {
            if (Header.e_entry == 0)
                return null;
            else
                return Address.Ptr32(Header.e_entry);
        }

        internal ElfSection GetSectionInfoByAddr(uint r_offset)
        {
            return
                (from sh in this.Sections
                 let addr = sh.Address != null ? sh.Address.ToLinear() : 0
                 where
                    r_offset != 0 &&
                    addr <= r_offset && r_offset < addr + sh.Size
                 select sh)
                .FirstOrDefault();
        }

        protected override int GetSectionNameOffset(uint idxString)
        {
            return (int)(Sections[Header.e_shstrndx].FileOffset + idxString);
        }

        public string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(Sections[idx].FileOffset + offset);
        }

        public string GetSymbolName(int iSymbolSection, uint symbolNo)
        {
            var symSection = Sections[iSymbolSection];
            return GetSymbolName(symSection, symbolNo);
        }

        public string GetSymbolName(ElfSection symSection, uint symbolNo)
        {
            var strSection = symSection.LinkedSection;
            if (strSection == null)
                return string.Format("null:{0:X8}", symbolNo);
            uint offset = (uint)(symSection.FileOffset + symbolNo * symSection.EntrySize);
            var rdr = imgLoader.CreateReader(offset);
            rdr.TryReadUInt32(out offset);
            return GetStrPtr(strSection, offset);
        }

        public override SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred)
        {
            var segMap = AllocateMemoryAreas(
                Segments
                    .Where(p => IsLoadable(p.p_pmemsz, p.p_type))
                    .Select(p => Tuple.Create(
                        Address.Ptr32((uint)p.p_vaddr),
                        (uint)p.p_pmemsz)));

            foreach (var ph in Segments)
            {
                DebugEx.Inform(ElfImageLoader.trace, "ph: addr {0:X8} filesize {0:X8} memsize {0:X8}", ph.p_vaddr, ph.p_filesz, ph.p_pmemsz);
                if (!IsLoadable(ph.p_pmemsz, ph.p_type))
                    continue;
                var vaddr = Address.Ptr32((uint)ph.p_vaddr);
                segMap.TryGetLowerBound(vaddr, out var mem);
                if (ph.p_filesz > 0)
                {
                    Array.Copy(
                        rawImage,
                        (long)ph.p_offset, mem.Bytes,
                        vaddr - mem.BaseAddress, (long)ph.p_filesz);
            }
            }
            var segmentMap = new SegmentMap(addrPreferred);
            if (Sections.Count > 0)
            {
                foreach (var section in Sections)
                {
                    if (string.IsNullOrEmpty(section.Name) || section.Address == null)
                        continue;

                    if (segMap.TryGetLowerBound(section.Address, out var mem) &&
                        section.Address < mem.EndAddress)
                    {
                        AccessMode mode = AccessModeOf(section.Flags);
                        var seg = segmentMap.AddSegment(new ImageSegment(
                            section.Name,
                            section.Address,
                            mem, mode)
                        {
                            Size = (uint)section.Size
                        });
                        seg.Designer = CreateRenderer(section, machine);
                    }
                    else
                    {
                        //$TODO: warn
                    }
                }
            }
            else
            {
                // There are stripped ELF binaries with 0 sections. If we have one
                // create a pseudo-section from the segMap.
                foreach (var segment in segMap)
                {
                    var imgSegment = new ImageSegment(
                        segment.Value.BaseAddress.GenerateName("seg", ""),
                        segment.Value,
                        AccessMode.ReadExecute)        //$TODO: writeable segments.
                    {
                        Size = (uint)segment.Value.Length,
                    };
                    segmentMap.AddSegment(imgSegment);
                }
            }
            segmentMap.DumpSections();
            return segmentMap;
        }

        public override int LoadSegments()
        {
            var rdr = imgLoader.CreateReader(Header.e_phoff);
            for (int i = 0; i < Header.e_phnum; ++i)
            {
                var sSeg = Elf32_PHdr.Load(rdr);
                Segments.Add(new ElfSegment
                {
                    p_type = sSeg.p_type,
                    p_offset = sSeg.p_offset,
                    p_vaddr = sSeg.p_vaddr,
                    p_paddr = sSeg.p_paddr,
                    p_filesz = sSeg.p_filesz,
                    p_pmemsz = sSeg.p_pmemsz,
                    p_flags = sSeg.p_flags,
                    p_align = sSeg.p_align,
                });
            }
            return Segments.Count;
        }


        public override ElfRelocation LoadRelEntry(EndianImageReader rdr)
        {
            var rela = Elf32_Rel.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                SymbolIndex = (int)(rela.r_info >> 8)
            };
        }

        public override ElfRelocation LoadRelaEntry(EndianImageReader rdr)
        {
            var rela = Elf32_Rela.Read(rdr);
            return new ElfRelocation
            {
                Offset = rela.r_offset,
                Info = rela.r_info,
                Addend = rela.r_addend,
                SymbolIndex = (int)(rela.r_info >> 8)
            };
        }

        public override void LoadSectionHeaders()
        {
            // Create the sections.
            var inames = new List<uint>();
            var links = new List<uint>();
            var infos = new List<uint>();
            var rdr = imgLoader.CreateReader(Header.e_shoff);
            for (uint i = 0; i < Header.e_shnum; ++i)
            {
                var shdr = Elf32_SHdr.Load(rdr);
                if (shdr == null)
                    break;
                var section = new ElfSection
                {
                    Number = i,
                    Type = shdr.sh_type,
                    Flags = shdr.sh_flags,
                    Address = Address.Ptr32(shdr.sh_addr),
                    FileOffset = shdr.sh_offset,
                    Size = shdr.sh_size,
                    Alignment = shdr.sh_addralign,
                    EntrySize = shdr.sh_entsize,
                };
                Sections.Add(section);
                inames.Add(shdr.sh_name);
                links.Add(shdr.sh_link);
                infos.Add(shdr.sh_info);
            }

            // Get section names and crosslink sections.

            for (int i = 0; i < Sections.Count; ++i)
            {
                var section = Sections[i];
                section.Name = ReadSectionName(inames[i]);

                ElfSection linkSection = null;
                ElfSection relSection = null;
                switch (section.Type)
                {
                case SectionHeaderType.SHT_REL:
                case SectionHeaderType.SHT_RELA:
                    linkSection = GetSectionByIndex(links[i]);
                    relSection = GetSectionByIndex(infos[i]);
                    break;
                case SectionHeaderType.SHT_DYNAMIC:
                case SectionHeaderType.SHT_HASH:
                case SectionHeaderType.SHT_SYMTAB:
                case SectionHeaderType.SHT_DYNSYM:
                    linkSection = GetSectionByIndex(links[i]);
                    break;
                }
                section.LinkedSection = linkSection;
                section.RelocatedSection = relSection;
            }
        }

        public override ElfSymbol LoadSymbol(ulong offsetSymtab, ulong symbolIndex, ulong entrySize, ulong offsetStringTable)
        {
            var rdr = CreateReader(offsetSymtab + entrySize * symbolIndex);
            var sym = Elf32_Sym.Load(rdr);
            return new ElfSymbol
            {
                Name = RemoveModuleSuffix(ReadAsciiString(offsetStringTable + sym.st_name)),
                Type = (ElfSymbolType)(sym.st_info & 0xF),
                Bind = sym.st_info >> 4,
                SectionIndex = sym.st_shndx,
                Value = sym.st_value,
                Size = sym.st_size,
            };
        }

        public override Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection)
        {
            DebugEx.PrintIf(ElfImageLoader.trace.TraceInfo , "== Symbols from {0} ==", symSection.Name);
            var stringtableSection = symSection.LinkedSection;
            var rdr = CreateReader(symSection.FileOffset);
            var symbols = new Dictionary<int, ElfSymbol>();
            for (ulong i = 0; i < symSection.Size / symSection.EntrySize; ++i)
            {
                var sym = Elf32_Sym.Load(rdr);
                var symName = RemoveModuleSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name));
                DebugEx.PrintIf(ElfImageLoader.trace.TraceVerbose, "  {0,3} {1,-25} {2,-12} {3,6} {4,-15} {5:X8} {6,9}",
                    i,
                    string.IsNullOrWhiteSpace(symName) ? "<empty>" : symName,
                    (ElfSymbolType)(sym.st_info & 0xF),
                    sym.st_shndx,
                    GetSectionName(sym.st_shndx),
                    sym.st_value,
                    sym.st_size);
                symbols.Add((int) i, new ElfSymbol
                {
                    Name = RemoveModuleSuffix(ReadAsciiString(stringtableSection.FileOffset + sym.st_name)),
                    Type = (ElfSymbolType)(sym.st_info & 0xF),
                    SectionIndex = sym.st_shndx,
                    Value = sym.st_value,
                    Size = sym.st_size,
                });
            }
            return symbols;
        }

        public override Address ReadAddress(EndianImageReader rdr)
        {
            if (!rdr.TryReadUInt32(out uint uAddr))
                return null;
            var addr = Address.Ptr32(uAddr);
            return addr; 
        }
    }
}
