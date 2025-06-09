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
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
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
        public const int HEADER_OFFSET = 0x0010;



        // Object file type
        public const ushort ET_NONE = 0;             // No file type
        public const ushort ET_REL = 1;             // Relocatable file
        public const ushort ET_EXEC = 2;             // Executable file
        public const ushort ET_DYN = 3;             // Shared object file
        public const ushort ET_CORE = 4;             // Core file

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

        protected ulong flags;
        protected IPlatform? platform;
        protected byte[] rawImage;
        private SegmentMap? segmentMap;

        protected ElfLoader(
            IServiceProvider services,
            ElfBinaryImage binary,
            byte[] rawImage) 
        {
            this.Services = services;
            this.BinaryImage = binary;
            this.flags = binary.Header.Flags;
            this.Endianness = binary.Endianness;
            this.rawImage = rawImage;
        }

        public ElfBinaryImage BinaryImage { get; }
        public IServiceProvider Services { get; }
        public abstract Address DefaultAddress { get; }
        
        public EndianServices Endianness { get; }

        public bool IsExecutableFile => this.BinaryImage.Header.BinaryFileType != BinaryFileType.ObjectFile;
        public bool IsRelocatableFile => this.BinaryImage.Header.BinaryFileType == BinaryFileType.ObjectFile;


        public bool IsBigendian => Endianness == EndianServices.Big;

        public abstract ulong AddressToFileOffset(ulong addr);

        public abstract Address ComputeBaseAddress(IPlatform platform);

        public abstract Address CreateAddress(ulong uAddr);

        public abstract ElfObjectLinker CreateLinker(IProcessorArchitecture arch);

        public abstract SegmentMap LoadImageBytes(IPlatform platform, byte[] rawImage, Address addrPreferred);

        public abstract void LoadFileHeader();

        //$TODO: modifies the BinaryImage as a side effect; don't do that.
        public abstract IReadOnlyList<ElfSegment> LoadSegments();

        //$TODO: modifies the BinaryImage as a side effect; don't do that.
        public abstract List<ElfSection> LoadSectionHeaders();

        public abstract ElfRelocation LoadRelEntry(EndianImageReader rdr, IDictionary<int, ElfSymbol> symbols);

        public abstract ElfRelocation LoadRelaEntry(EndianImageReader rdr, IDictionary<int, ElfSymbol> symbols);

        public abstract ElfSymbol? LoadSymbol(ulong offsetSymtab, ulong symbolIndex, ulong entrySize, ulong offsetStringTable);

        public abstract Dictionary<int, ElfSymbol> LoadSymbolsSection(ElfSection symSection);

        public abstract Address? ReadAddress(EndianImageReader rdr);

        protected abstract int GetSectionNameOffset(List<ElfSection> sections, uint idxString);

        public abstract void Dump(Address addrLoad, TextWriter writer);

        public abstract (Address?, ProcessorState?) GetEntryPointAddress(Address addrBase, Program prgram);

        public IEnumerable<ElfDynamicEntry> GetDynamicEntries(ulong offsetDynamic)
        {
            var rdr = Endianness!.CreateImageReader(this.rawImage!, (long) offsetDynamic);
            return GetDynamicEntries(rdr);
        }

        public abstract IEnumerable<ElfDynamicEntry> GetDynamicEntries(EndianImageReader rdr);

        public static AccessMode AccessModeOf(ulong sh_flags)
        {
            AccessMode mode = AccessMode.Read;
            if ((sh_flags & SHF_WRITE) != 0)
                mode |= AccessMode.Write;
            if ((sh_flags & SHF_EXECINSTR) != 0)
                mode |= AccessMode.Execute;
            return mode;
        }

        public ElfSegment? GetSegmentByAddress(ulong uAddr)
        {
            return BinaryImage
                .Segments
                .Cast<ElfSegment>()
                .FirstOrDefault(s => s.IsValidAddress(uAddr));
        }

        public static SortedList<Address, ByteMemoryArea> AllocateMemoryAreas(IEnumerable<(Address, uint)> segments)
        {
            var mems = new SortedList<Address, ByteMemoryArea>();
            Address? addr = null;
            Address? addrEnd = null;
            foreach (var pair in segments)
            {
                if (addr is null)
                {
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else if (addrEnd!.Value < pair.Item1)
                {
                    var size = (uint) (addrEnd! - addr);
                    mems.Add(addr.Value, new ByteMemoryArea(addr.Value, new byte[size]));
                    addr = pair.Item1;
                    addrEnd = pair.Item1 + pair.Item2;
                }
                else
                {
                    addrEnd = Address.Max(addrEnd!.Value, pair.Item1 + pair.Item2);
                }
            }
            if (addr is not null)
            {
                var size = (uint) (addrEnd! - addr);
                mems.Add(addr.Value, new ByteMemoryArea(addr.Value, new byte[size]));
            }
            return mems;
        }

        public virtual IProcessorArchitecture CreateArchitecture(ElfMachine elfMachine, EndianServices endianness)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var options = new Dictionary<string, object>();
            string arch;
            options[ProcessorOption.Endianness] = endianness == EndianServices.Little ? "le" : "be";

            string? stackRegName = null;
            switch (elfMachine)
            {
            case ElfMachine.EM_SPARC:
            case ElfMachine.EM_SPARC32PLUS:
                arch = "sparc32"; break;
            case ElfMachine.EM_SPARCV9:
                arch = "sparc64"; break;
            case ElfMachine.EM_386: arch = "x86-protected-32"; break;
            case ElfMachine.EM_X86_64: arch = "x86-protected-64"; break;
            case ElfMachine.EM_68K: arch = "m68k"; break;
            case ElfMachine.EM_PPC: arch = "ppc-be-32"; break;
            case ElfMachine.EM_ARM: arch = "arm"; break;
            case ElfMachine.EM_AARCH64: arch = "arm-64"; break;
            case ElfMachine.EM_XTENSA: arch = "xtensa"; break;
            case ElfMachine.EM_AVR: arch = "avr8"; break;
            case ElfMachine.EM_MSP430: arch = "msp430"; break;
            case ElfMachine.EM_SH:
                arch = "superH";
                if (superHModels.TryGetValue((SuperHFlags)this.flags & SuperHFlags.EF_MODEL_MASK, out var model))
                {
                    options[ProcessorOption.Model] = model;
                }
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
            case ElfMachine.EM_NANOMIPS:
                arch = endianness == EndianServices.Little ? "mips-le-32" : "mips-be-32";
                options[ProcessorOption.InstructionSet] = "nano";
                break;
            case ElfMachine.EM_BLACKFIN: arch = "blackfin"; break;
            case ElfMachine.EM_MORPHOS_PPC: arch = "ppc-be-32"; break;
            case ElfMachine.EM_PARISC:
                arch = "paRisc";
                options[ProcessorOption.WordSize] = "32";
                break;
            case ElfMachine.EM_AVR32:
            case ElfMachine.EM_AVR32a:
                arch = "avr32";
                break;
            case ElfMachine.EM_HEXAGON:
                arch = "hexagon";
                break;
            case ElfMachine.EM_VAX:
                arch = "vax";
                break;
            case ElfMachine.EM_ALTERA_NIOS2:
                arch = "nios2";
                break;
            case ElfMachine.EM_TRICORE:
                arch = "tricore";
                break;
            case ElfMachine.EM_LOONGARCH:
                arch = "loongaArch";
                break;
            case ElfMachine.EM_AEON:
                arch = "aeon";
                break;
            case ElfMachine.EM_TC32:
                arch = "tc32";
                break;
            case ElfMachine.EM_BA:
                arch = "beyond";
                break;
            default:
                throw new NotImplementedException($"ELF machine type {elfMachine} is not implemented yet.");
            }
            var a = cfgSvc.GetArchitecture(arch, options);
            if (a is null)
                throw new InvalidOperationException($"Unable to load architecture '{arch}'.");
            if (stackRegName is not null)
            {
                var sp = a.GetRegister(stackRegName);
                if (sp is null)
                    throw new ApplicationException($"Register '{stackRegName}' is not a stack register for architecture '{arch}'.");
                a.StackRegister = sp;
            }
            return a;
        }

        private static readonly Dictionary<ElfSymbolType, SymbolType> mpSymbolType = new Dictionary<ElfSymbolType, SymbolType>
        {
            { ElfSymbolType.STT_FUNC, SymbolType.Procedure },
            { ElfSymbolType.STT_OBJECT, SymbolType.Data },
            { ElfSymbolType.STT_NOTYPE, SymbolType.Unknown },
        };

        public ImageSymbol? CreateImageSymbol(ElfSymbol sym, IProcessorArchitecture arch, bool isExecutable)
        {
            if (!isExecutable && sym.SectionIndex > 0 && sym.SectionIndex >= BinaryImage.Sections.Count)
                return null;
            if (sym.SectionIndex == ElfSection.SHN_ABS)
                return null;
            SymbolType? st = GetSymbolType(sym);
            if (st is null || st.Value == SymbolType.Unknown)
                return null;
            // If this is a relocatable file, the symbol value is 
            // an offset from the section's virtual address. 
            // If this is an executable file, the symbol value is
            // the virtual address.
            var addr = isExecutable || sym.SectionIndex == 0
                ? platform!.MakeAddressFromLinear(sym.Value, true)
                : BinaryImage.Sections[(int) sym.SectionIndex].VirtualAddress! + sym.Value;

            var dt = GetSymbolDataType(sym, arch);
            var imgSym = ImageSymbol.Create(
                st.Value,
                arch,
                addr,
                RemoveModuleSuffix(sym.Name),
                dt);
            imgSym.ProcessorState = arch.CreateProcessorState();
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

        private DataType GetSymbolDataType(ElfSymbol sym, IProcessorArchitecture arch)
        {
            if (sym.Type == ElfSymbolType.STT_FUNC)
            {
                return new FunctionType();
            }
            else if ((int) sym.Size == arch.PointerType.Size)
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
            if (seg is null)
                return 0;

            var addrEnd = 0ul;
            foreach (var de in BinaryImage.DynamicEntries.Values)
            {
                if (de.UValue <= addrStart)
                    continue;
                var tagInfo = de.GetTagInfo(BinaryImage.Header.Machine);
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
            string envName = string.Empty;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var options = new Dictionary<string, object>();
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
                envName = "elf-neutral";
                options["osabi"] = "linux";
                break;
            case ELFOSABI_OPENVMS:
                envName = "openVMS";
                break;
            default:
                var eventListener = Services.GetService<IEventListener>();
                eventListener?.Warn("Unsupported ELF ABI 0x{0:X2}.", osAbi);
                break;
            }
            var env = cfgSvc.GetEnvironment(envName);
            this.platform = env.Load(Services, arch);
            this.platform.LoadUserOptions(options);
            return platform;
        }

        public virtual ElfRelocator CreateRelocator(
            ElfMachine machine,
            IProcessorArchitecture arch,
            SortedList<Address, ImageSymbol> symbols,
            Dictionary<ElfSymbol, Address> plt)
        {
            throw new NotSupportedException($"Relocator for architecture {machine} not implemented yet.");
        }

        public Program LoadImage(IPlatform platform, byte[] rawImage)
        {
            Debug.Assert(platform is not null);
            this.platform = platform;
            this.rawImage = rawImage;
            var addrPreferred = ComputeBaseAddress(platform);
            Dump(addrPreferred);
            this.segmentMap = LoadImageBytes(platform, rawImage, addrPreferred);
            var program = new Program(new ByteProgramMemory(segmentMap), platform.Architecture, platform);
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
            var dynSeg = BinaryImage.Segments.FirstOrDefault(p => p.p_type == ProgramHeaderType.PT_DYNAMIC);
            if (dynSeg is null)
                return;
            var rdr = this.Endianness!.CreateImageReader(rawImage!, (long) dynSeg.FileOffset);
            var (deps, entries) = LoadDynamicSegment(rdr);
            BinaryImage.Dependencies.AddRange(deps);
            foreach (var de in entries)
            {
                BinaryImage.DynamicEntries[de.Tag] = de;
            }
        }

        public (List<string>, List<ElfDynamicEntry>) LoadDynamicSegment(EndianImageReader rdr)
        {
            var dynEntries = GetDynamicEntries(rdr).ToList();
            var deStrTab = GetDynamicStringTableFileOffset(dynEntries);
            if (deStrTab is null)
            {
                //$REVIEW: is missing a string table worth a warning?
                return (new List<string>(), new List<ElfDynamicEntry>());
            }
            var offStrtab = AddressToFileOffset(deStrTab.Value);
            var dependencies = new List<string>();
            var dynamicEntries = new List<ElfDynamicEntry>();
            foreach (var de in dynEntries)
            {
                if (de.Tag == ElfDynamicEntry.DT_NEEDED)
                {
                    dependencies.Add(ReadAsciiString(offStrtab + de.UValue));
                }
                else
                {
                    dynamicEntries.Add(de);
                }
            }
            return (dependencies, dynamicEntries);
        }

        private ulong? GetDynamicStringTableFileOffset(List<ElfDynamicEntry> dynEntries)
        {
            var deStrTab = dynEntries.FirstOrDefault(de => de.Tag == ElfDynamicEntry.DT_STRTAB);
            if (deStrTab is not null)
                return deStrTab.UValue;
            var dynstr = this.GetSectionInfoByName(".dynstr");
            if (dynstr is not null)
                return dynstr.FileOffset;
            return null;
        }

        public SortedList<Address, ImageSymbol> CreateSymbolDictionaries(IProcessorArchitecture arch, bool isExecutable)
        {
            var imgSymbols = new SortedList<Address, ImageSymbol>();
            foreach (var sym in BinaryImage.SymbolsByFileOffset.Values.SelectMany(seg => seg.Values).OrderBy(s => s.Value))
            {
                var imgSym = CreateImageSymbol(sym, arch, isExecutable);
                if (imgSym is null || imgSym.Address!.ToLinear() == 0)
                    continue;
                imgSymbols[imgSym.Address] = imgSym;
            }
            return imgSymbols;
        }

        public EndianImageReader CreateReader(ulong fileOffset)
        {
            return Endianness.CreateImageReader(rawImage, (long) fileOffset);
        }

        public ImageWriter CreateWriter(uint fileOffset)
        {
            return Endianness.CreateImageWriter(rawImage, fileOffset);
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
            if (got is not null)
            {
                var gotStart = got.Address;
                var gotEnd = got.EndAddress;
                int cEntries = ConstructGotEntries(program, symbols, gotStart, gotEnd, false);
                if (cEntries > 0)
                    return;
            }
            // Ain't got no GOT!
            // Reconstruct GOT pointers from the dynamic relocations
            ReconstructGotPointersFromDynamicRelocations(program, symbols);
        }

        int ReconstructGotPointersFromDynamicRelocations(
            Program program,
            SortedList<Address, ImageSymbol> symbols)
        {
            int cSymbols = 0;
            foreach (var reloc in BinaryImage.DynamicRelocations)
            {
                var symbol = reloc.Symbol;
                var addrGot = CreateAddress(reloc.Offset);
                if (symbol is not null && 
                    (symbol.Type == ElfSymbolType.STT_FUNC))
                    // || symbol.Type == ElfSymbolType.STT_OBJECT))
                {
                    var name = symbol.Name;
                    ImageSymbol gotSym = CreateGotSymbol(program.Architecture, addrGot, name);
                    symbols[addrGot] = gotSym;
                    var symType = symbol.Type == ElfSymbolType.STT_FUNC
                        ? SymbolType.ExternalProcedure
                        : SymbolType.Data;
                    if (symbol.SectionIndex == 0) // An external thing
                    {
                        ++cSymbols;
                        if (!program.ImportReferences.TryGetValue(addrGot, out var oldImpRef))
                        {
                            program.ImportReferences.Add(
                                addrGot,
                                new NamedImportReference(
                                    addrGot,
                                    null,
                                    name,
                                    symType));
                        }
                    }
                }
            }
            return cSymbols;
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
        public int ConstructGotEntries(Program program, SortedList<Address, ImageSymbol> symbols, Address gotStart, Address gotEnd, bool makeGlobals)
        {
            ElfImageLoader.trace.Verbose("== Constructing GOT from .got section entries ==");
            if (!program.TryCreateImageReader(program.Architecture, gotStart, out var rdr))
                return 0;
            int cEntries = 0;
            var arch = program.Architecture;
            while (rdr.Address < gotEnd)
            {
                var addrGot = rdr.Address;
                var addrSym = ReadAddress(rdr);
                if (addrSym is null)
                    break;
                if (symbols.TryGetValue(addrSym.Value, out ImageSymbol? symbol))
                {
                    // This GOT entry is a known symbol!
                    if (symbol.Type == SymbolType.Procedure || symbol.Type == SymbolType.ExternalProcedure)
                    {
                        ++cEntries;
                        var name = symbol.Name!;
                        ImageSymbol gotSym = CreateGotSymbol(arch, addrGot, name);
                        symbols[addrGot] = gotSym;
                        ElfImageLoader.trace.Verbose("{0}+{1:X4}: Found GOT entry {2}, referring to symbol at {3}",
                            gotStart, addrGot - gotStart, gotSym, symbol);
                        if (symbol.Type == SymbolType.ExternalProcedure)
                        {
                            program.ImportReferences.TryGetValue(addrGot, out var oldImpRef);
                            if (oldImpRef is null)
                                program.ImportReferences.Add(
                                    addrGot,
                                    new NamedImportReference(
                                        addrGot,
                                        null,
                                        name,
                                        symbol.Type));
                        }
                    }
                }
                else if (makeGlobals)
                {
                    // This GOT entry has no corresponding symbol. It's likely a global
                    // variable with no name.
                    ImageSymbol gotDataSym = ImageSymbol.Create(SymbolType.Data, program.Architecture, addrGot);
                    ElfImageLoader.trace.Verbose("{0}+{1:X4}: GOT entry with no symbol, assuming local data {2}",
                        gotStart, addrGot - gotStart, addrGot);
                    ++cEntries;
                    program.ImportReferences.Add(
                        addrGot,
                        new NamedImportReference(
                            addrGot,
                            null,
                            null!,
                            gotDataSym.Type));
                }
            }
            return cEntries;
        }

        public ImageSymbol CreateGotSymbol(IProcessorArchitecture arch, Address addrGot, string name)
        {
            //$TODO: look up function signature.
            int size = arch.PointerType.Size;
            int bitSize = arch.PointerType.BitSize;
            return ImageSymbol.DataObject(arch, addrGot, name + "_GOT", new Pointer(new CodeType(), bitSize));
        }

        public IEnumerable<ElfSymbol> GetAllSymbols()
        {
            return BinaryImage.SymbolsByFileOffset.Values.SelectMany(s => s.Values);
        }

        public ElfSection? GetSectionByIndex(List<ElfSection> sections, uint shidx)
        {
            if (0 <= shidx && shidx < sections.Count)
            {
                return sections[(int) shidx];
            }
            else
            {
                return null;
            }
        }

        public ElfSection? GetSectionInfoByName(string sectionName)
        {
            return BinaryImage
                .Sections
                .Cast<ElfSection>()
                .FirstOrDefault(s => s.Name == sectionName);
        }

        protected string ReadSectionName(List<ElfSection> sections, uint idxString)
        {
            ulong offset = (ulong) GetSectionNameOffset(sections, idxString);
            return ReadAsciiString(offset);
        }

        protected string GetBindingName(ElfSymbolBinding binding)
        {
            return binding switch
            {
                ElfSymbolBinding.STB_GLOBAL => "glbl",
                ElfSymbolBinding.STB_LOCAL => "locl",
                ElfSymbolBinding.STB_WEAK => "weak",
                ElfSymbolBinding.STB_GNU_UNIQUE => "uniq",
                _ => ((int) binding).ToString("X4")
            };
        }

        public string GetSectionName(ushort st_shndx)
        {
            if (st_shndx == ElfSection.SHN_UNDEF)
            {
                return "SHN_UNDEF";
            }
            if (st_shndx < 0xFF00)
            {
                if (st_shndx < BinaryImage.Sections.Count)
                    return BinaryImage.Sections[st_shndx].Name!;
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
            if (strSection is null)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentNullException(nameof(strSection));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(strSection.FileOffset + offset);
        }

        [Conditional("DEBUG")]
        public void Dump(Address addrLoad)
        {
            if (ElfImageLoader.trace.TraceVerbose)
            {
                var sw = new StringWriter();
                Dump(addrLoad, sw);
                Debug.Print(sw.ToString());
            }
        }

        protected string DumpShFlags(ulong shf)
        {
            return string.Format("{0}{1}{2}",
                ((shf & SHF_EXECINSTR) != 0) ? "x" : " ",
                ((shf & SHF_ALLOC) != 0) ? "a" : " ",
                ((shf & SHF_WRITE) != 0) ? "w" : " ");
        }


        protected ImageSymbol EnsureEntryPoint(
            IProcessorArchitecture arch,
            List<ImageSymbol> entryPoints,
            SortedList<Address, ImageSymbol> symbols,
            Address addr,
            ProcessorState? estate)
        {
            if (!symbols.TryGetValue(addr, out ImageSymbol? ep))
            {
                ep = ImageSymbol.Procedure(arch, addr, state: estate ?? arch.CreateProcessorState());
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
        public ElfSymbol? EnsureSymbol(ulong offSymtab, int i, ulong symentrysize, ulong offStrtab)
        {
            if (!BinaryImage.SymbolsByFileOffset.TryGetValue(offSymtab, out var symList))
            {
                symList = new Dictionary<int, ElfSymbol>();
                BinaryImage.SymbolsByFileOffset.Add(offSymtab, symList);
            }
            if (!symList.TryGetValue(i, out var sym))
            {
                sym = LoadSymbol(offSymtab, (ulong) i, symentrysize, offStrtab);
                if (sym is null)
                {
                    ElfImageLoader.trace.Warn("Unable to load ELF image symbol {0} (0x{0:X}).", i);
                }
                else
                {
                    symList.Add(i, sym);
                }
            }
            return sym;
        }

        protected static bool IsLoadable(ulong p_pmemsz, ProgramHeaderType p_type)
        {
            if (p_pmemsz == 0)
                return false;
            return (p_type == ProgramHeaderType.PT_LOAD ||
                    p_type == ProgramHeaderType.PT_DYNAMIC);
        }

        public void LoadSymbolsFromSections()
        {
            var symbolSections = BinaryImage.Sections.Where(s =>
                s.Type == SectionHeaderType.SHT_SYMTAB ||
                s.Type == SectionHeaderType.SHT_DYNSYM)
                .ToList();
            foreach (var section in symbolSections)
            {
                ElfImageLoader.trace.Inform("== Loading ELF symbols from section {0} (at offset {1:X})", section.Name!, section.FileOffset);
                var symtab = LoadSymbolsSection(section);
                BinaryImage.SymbolsByFileOffset[section.FileOffset] = symtab;
            }
        }

        public string ReadAsciiString(ulong fileOffset)
        {
            var bytes = this.rawImage!;
            if (fileOffset >= (ulong) bytes.Length)
                return "";
            int u = (int) fileOffset;
            while (bytes[u] != 0)
            {
                ++u;
            }
            return Encoding.ASCII.GetString(bytes, (int) fileOffset, u - (int) fileOffset);
        }

        public void Relocate(ElfMachine elfMachine, Program program, Address addrLoad, Dictionary<ElfSymbol, Address> plt)
        {
            var arch = program.Architecture;
            var symbols = CreateSymbolDictionaries(arch, IsExecutableFile);
            var relocator = CreateRelocator(elfMachine, arch, symbols, plt);
            relocator.Relocate(program, addrLoad, plt);
            relocator.LocateGotPointers(program, symbols);
            symbols = symbols.Values.Select(relocator.AdjustImageSymbol).ToSortedList(s => s.Address!);
            var entryPoints = new List<ImageSymbol>();
            var (ae, estate) = GetEntryPointAddress(addrLoad, program);
            if (ae is not null)
            {
                var addrEntry = relocator.AdjustAddress(ae.Value);
                var symEntry = EnsureEntryPoint(arch, entryPoints, symbols, addrEntry, estate);
                var addrMain = relocator.FindMainFunction(program, addrEntry);
                if (addrMain is not null)
                {
                    EnsureEntryPoint(arch, entryPoints, symbols, addrMain.Value, estate);
                }
                var addrGlobalPtr = program.Platform.FindGlobalPointerValue(program, addrEntry);
                if (addrGlobalPtr is not null)
                {
                    program.GlobalRegisterValue = addrGlobalPtr;
                }
            }
            foreach (var ep in entryPoints.Select(relocator.AdjustImageSymbol))
            {
                program.EntryPoints[ep.Address] = ep;
            }
            foreach (var symbol in symbols)
            {
                program.ImageSymbols[symbol.Key] = symbol.Value;
            }
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

        protected string rwx(ulong flags)
        {
            var sb = new StringBuilder();
            sb.Append((flags & 4) != 0 ? 'r' : '-');
            sb.Append((flags & 2) != 0 ? 'w' : '-');
            sb.Append((flags & 1) != 0 ? 'x' : '-');
            return sb.ToString();
        }

        public BinaryFileType FileTypeOf(ushort e_type)
        {
            return e_type switch
            {
                ET_REL => BinaryFileType.ObjectFile,
                ET_EXEC => BinaryFileType.Executable,
                ET_DYN => BinaryFileType.SharedLibrary,
                _ => BinaryFileType.Unknown,
            };
        }


        public void LoadRelocations()
        {
            foreach (var relSection in this.BinaryImage.Sections)
            {
                var referringSection = relSection.RelocatedSection;
                if (relSection.Type == SectionHeaderType.SHT_REL)
                {
                    if (!BinaryImage.SymbolsByFileOffset.TryGetValue(relSection.LinkedSection!.FileOffset, out var sectionSymbols))
                        throw new NotImplementedException();    //$TODO
                    var fileOffset = relSection.FileOffset;
                    var cEntries = relSection.EntryCount();

                    var relocations = LoadRelEntries(sectionSymbols, fileOffset, cEntries);
                    var vaddr = referringSection?.VirtualAddress;
                    if (vaddr is not null && vaddr.Value.Offset != 0 && this.IsRelocatableFile)
                    {
                        BinaryImage.AddRelocations(referringSection?.Index ?? 0, relocations);
                    }
                    else
                    {
                        BinaryImage.AddDynamicRelocations(relocations);
                    }
                }
                else if (relSection.Type == SectionHeaderType.SHT_RELA)
                {
                    if (!BinaryImage.SymbolsByFileOffset.TryGetValue(relSection.LinkedSection!.FileOffset, out var sectionSymbols))
                        throw new NotImplementedException();    //$TODO
                    var fileOffset = relSection.FileOffset;
                    var cEntries = relSection.EntryCount();

                    var relocations = LoadRelaEntries(sectionSymbols, fileOffset, cEntries);
                    if (referringSection?.VirtualAddress is not null &&
                        this.IsRelocatableFile)
                    {
                        BinaryImage.AddRelocations(referringSection?.Index ?? 0, relocations);
                    }
                    else
                    {
                        BinaryImage.AddDynamicRelocations(relocations);
                    }
                }
            }
        }

        private List<ElfRelocation> LoadRelaEntries(
            Dictionary<int, ElfSymbol> sectionSymbols,
            ulong fileOffset, 
            uint cEntries)
        {
            var rdr = CreateReader(fileOffset);
            var relocations = new List<ElfRelocation>();
            for (uint i = 0; i < cEntries; ++i)
            {
                var rela = LoadRelaEntry(rdr, sectionSymbols);
                relocations.Add(rela);
            }
            return relocations;
        }

        private List<ElfRelocation> LoadRelEntries(
            Dictionary<int, ElfSymbol> sectionSymbols,
            ulong fileOffset,
            uint cEntries)
        {
            var rdr = CreateReader(fileOffset);
            var relocations = new List<ElfRelocation>();
            for (uint i = 0; i < cEntries; ++i)
            {
                var rel = LoadRelEntry(rdr, sectionSymbols);
                relocations.Add(rel);
            }
            return relocations;
        }

        public IReadOnlyCollection<ElfRelocation> LoadDynamicRelocations()
        {
            var symbols = new List<ElfSymbol>();
            var result = new List<ElfRelocation>();
            bool alreadySeen = false;
            foreach (var dynSeg in EnumerateDynamicSegments())
            {
                if (alreadySeen)
                {
                    var eventListener = Services.GetService<IEventListener>();
                    eventListener?.Warn("Multiple dynamic ELF sections detected. Results may be unpredictable.");
                    return result;
                }
                alreadySeen = true;

                var dynEntries = this.BinaryImage.DynamicEntries;
                dynEntries.TryGetValue(ElfDynamicEntry.DT_STRTAB, out var strtab);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_SYMTAB, out var symtab);

                dynEntries.TryGetValue(ElfDynamicEntry.DT_RELA, out var rela);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_RELASZ, out var relasz);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_RELAENT, out var relaent);

                dynEntries.TryGetValue(ElfDynamicEntry.DT_REL, out var rel);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_RELSZ, out var relsz);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_RELENT, out var relent);

                dynEntries.TryGetValue(ElfDynamicEntry.DT_SYMENT, out var syment);
                if (symtab is null)
                    continue;
                if (strtab is null)
                    throw new BadImageFormatException("ELF dynamic segment lacks a string table.");
                if (syment is null)
                    throw new BadImageFormatException("ELF dynamic segment lacks the size of symbol table entries.");


                var offStrtab = AddressToFileOffset(strtab.UValue);
                var offSymtab = AddressToFileOffset(symtab.UValue);

                var syms = LoadSymbolsFromDynamicSegment(dynSeg, symtab, syment, offStrtab, offSymtab)
                        .Select((s, i) => (s, i))
                        .ToDictionary(x => x.i, x => x.s);
                BinaryImage.AddDynamicSymbols(syms);

                if (rela is { })
                {
                    if (relasz is null || relasz.UValue == 0)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relaent is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    var fileOffset = this.AddressToFileOffset(rela.UValue);
                    var relocations = LoadRelaEntries(syms, fileOffset, (uint)(relaent.UValue / relasz.UValue));
                    result.AddRange(relocations);
                }
                else if (rel is { })
                {
                    if (relsz is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of the relocation table.");
                    if (relent is null)
                        throw new BadImageFormatException("ELF dynamic segment lacks the size of relocation table entries.");
                    var fileOffset = this.AddressToFileOffset(rel.UValue);
                    var relocations = LoadRelEntries(syms, fileOffset, (uint) (relsz.UValue / relent.UValue));
                    result.AddRange(relocations);
                }

                // Relocate the DT_JMPREL table.
                dynEntries.TryGetValue(ElfDynamicEntry.DT_JMPREL, out var jmprel);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_PLTRELSZ, out var pltrelsz);
                dynEntries.TryGetValue(ElfDynamicEntry.DT_PLTREL, out var pltrel);
                if (jmprel is { } && pltrelsz is { } && pltrel is { })
                {
                    /*
                    if (pltrel.SValue == ElfDynamicEntry.DT_RELA) // entries are in RELA format.
                    {
                        ReadRelocations(rela.UValue, rela.UValue, )
                        var relocations = LoadRelaEntries(syms, rela.UValue, )
                        relTable = new RelaTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else if (pltrel.SValue == ElfDynamicEntry.DT_REL) // entries are in REL format
                    {
                        relTable = new RelTable(this, jmprel.UValue, pltrelsz.UValue);
                    }
                    else
                    {
                        var listener = Loader.Services.RequireService<IEventListener>();
                        listener.Warn("Invalid value for DT_PLTREL: {0}", pltrel.UValue);
                        continue;
                    }

                    ElfImageLoader.trace.Inform("Relocating entries in DT_JMPREL:");
                    foreach (var (addrImport, elfSym, extraSym) in relTable.RelocateEntries(program, offStrtab, offSymtab, syment.UValue))
                    {
                        symbols.Add(elfSym);
                        if (extraSym is { })
                            symbols.Add(extraSym);
                        GenerateImageSymbol(program, addrImport, elfSym, extraSym);
                    }
                    */
                }
            }
            return result;
        }

        /*
        public List<(Address, ElfSymbol, ElfSymbol?)> ReadRelocations(
            ulong vaddr,
            ulong uAddrRelocation,
            ulong TableSize,
            ulong offSymtab,
            ulong symentrysize)
        {
            var offRela = AddressToFileOffset(vaddr);
            var rdrRela = CreateReader(offRela);
            var offRelaEnd = (long) (offRela + TableSize);

            var symbols = new List<(Address, ElfSymbol, ElfSymbol?)>();
            while (rdrRela.Offset < offRelaEnd)
            {
                var relocation = ReadRelocation(rdrRela);
                var elfSym = EnsureSymbol(offSymtab, relocation.SymbolIndex, symEntrySize, offStrtab);
                if (elfSym is null)
                    continue;
                if (!ctx.Update(relocation, elfSym))
                    continue;

                ElfImageLoader.trace.Verbose("  {0}: symbol {1} type: {2} addend: {3}", relocation, elfSym, relocator.RelocationTypeToString((byte) relocation.Info) ?? "?", relocation.Addend.HasValue ? relocation.Addend.Value.ToString("X") : "-None-");
                var (addrRelocation, newSym) = relocator.RelocateEntry(ctx, relocation, elfSym);
                if (addrRelocation is not null)
                {
                    symbols.Add((addrRelocation, elfSym, newSym));
                }
            }
            return symbols;
        }
        */


        private List<ElfSymbol> LoadSymbolsFromDynamicSegment(
            ElfSegment dynSeg,
            ElfDynamicEntry symtab,
            ElfDynamicEntry syment,
            ulong offStrtab,
            ulong offSymtab)
        {
            var result = new List<ElfSymbol>();

            // Sadly, the ELF format has no way to locate the end of the symbols in a DT_DYNAMIC segment.
            // We guess instead...
            var addrEnd = GuessAreaEnd(symtab.UValue, dynSeg);
            if (addrEnd != 0)
            {
                // We have found some symbols to ensure.
                ElfImageLoader.trace.Verbose("== Symbols in the DT_DYNAMIC segment");
                int i = 0;
                for (ulong uSymAddr = symtab.UValue; uSymAddr < addrEnd; uSymAddr += syment.UValue)
                {
                    var elfSym = EnsureSymbol(offSymtab, i, syment.UValue, offStrtab);
                    ++i;
                    if (elfSym is null)
                        continue;
                    result.Add(elfSym);
                }
            }
            return result;
        }

        public IEnumerable<ElfSegment> EnumerateDynamicSegments()
        {
            return BinaryImage.Segments
                .Where(p => p.p_type == ProgramHeaderType.PT_DYNAMIC);
        }

        private static readonly Dictionary<Relocators.SuperHFlags, string> superHModels = new()
        {
            { Relocators.SuperHFlags.EF_SH1, "sh1" },
            { Relocators.SuperHFlags.EF_SH2, "sh2" },
            { Relocators.SuperHFlags.EF_SH3, "sh3" },
            { Relocators.SuperHFlags.EF_SH_DSP, "sh_dsp" },
            { Relocators.SuperHFlags.EF_SH3_DSP, "sh3_dsp" },
            { Relocators.SuperHFlags.EF_SH4AL_DSP, "sh4_dsp" },
            { Relocators.SuperHFlags.EF_SH3E, "sh3e" },
            { Relocators.SuperHFlags.EF_SH4, "sh4" },
            { Relocators.SuperHFlags.EF_SH2E, "sh2e" },
            { Relocators.SuperHFlags.EF_SH4A, "sh4a" },
            { Relocators.SuperHFlags.EF_SH2A, "sh2a" },
        };
    }
}
