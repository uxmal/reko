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

        public void LoadArchitectureFromHeader()
        {
            Architecture = CreateArchitecture(endianness);
        }

        protected virtual IProcessorArchitecture CreateArchitecture(byte endianness)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var options = new Dictionary<string, object>();
            string arch;
            string stackRegName = null;
            switch (this.machine)
            {
            case ElfMachine.EM_NONE: return null; // No machine
            case ElfMachine.EM_SPARC:
            case ElfMachine.EM_SPARC32PLUS:
            case ElfMachine.EM_SPARCV9:
                arch = "sparc32"; break;
            case ElfMachine.EM_386: arch = "x86-protected-32"; break;
            case ElfMachine.EM_X86_64: arch = "x86-protected-64"; break;
            case ElfMachine.EM_68K: arch = "m68k"; break;
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
            case ElfMachine.EM_NANOMIPS:
                if (endianness == ELFDATA2LSB)
                {
                    arch = "mips-le-32";
                }
                else
                {
                    arch = "mips-be-32";
                }
                options["decoder"] = "nano";
                break;
            case ElfMachine.EM_BLACKFIN:
                arch = "blackfin";
                break;
            default:
                throw new NotSupportedException(string.Format("Processor format {0} is not supported.", machine));
            }
            var a = cfgSvc.GetArchitecture(arch);
            a.LoadUserOptions(options);
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
                ? platform.MakeAddressFromLinear(sym.Value, true)
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
            else if ((int) sym.Size == Architecture.PointerType.Size)
            {
                return PrimitiveType.CreateWord(DataType.BitsPerByte * (int) sym.Size);
            }
            else
            {
                return new UnknownType((int) sym.Size);
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
            throw new NotSupportedException($"Relocator for architecture {machine} not implemented yet.");
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
                case ElfSection.SHN_ABS: return "SHN_ABS";
                case ElfSection.SHN_COMMON: return "SHN_COMMON";
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


}
