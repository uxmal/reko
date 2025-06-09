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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.ImageLoaders.MzExe.Msvc;
using Reko.ImageLoaders.MzExe.Pdb;
using Reko.ImageLoaders.MzExe.Pe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Loads Windows NT PE images.
    /// http://geos.icc.ru:8080/scripts/wwwbinv.dll/ShowR?coff.rfi
    /// </summary>
	public class PeImageLoader : AbstractPeLoader
	{
        internal static readonly TraceSwitch trace = new TraceSwitch(nameof(PeImageLoader), "Traces the progress of loading PE binary images") { Level = TraceLevel.Verbose};

        private const short ImageFileRelocationsStripped = 0x0001;
        private const short ImageFileExecutable = 0x0002;
        private const short ImageFileDll = 0x2000;
        public const uint DID_RvaBased = 1;
        private const uint CoffSymbolSize = 18;

        private IProcessorArchitecture arch;
        private IPlatform platform;
        private SizeSpecificLoader innerLoader;
        private Program program;
        private IEventListener listener;

        private ushort machine;
		private short optionalHeaderSize;
        private ushort fileFlags;
		private int sections;
        private uint offsetCoffSymbols;
        private uint cCoffSymbols;
        private uint offsetCoffStrings;

        private uint rvaSectionTable;
		private ByteMemoryArea imgLoaded;
        private PeBinaryImage binaryImage;
		private uint rvaImportTable;
        private uint rvaDelayImportDescriptor;
        private uint rvaExceptionTable;
        private uint sizeExceptionTable;
        private uint rvaBaseRelocationTable;
        private uint sizeBaseRelocationTable;
        private uint rvaDebug;
        private uint cbDebug;

        private uint rvaResources;
        private Relocator relocator;

#nullable disable
        public PeImageLoader(IServiceProvider services, ImageLocation imageLocation, byte [] img, uint peOffset) 
            : base(services, imageLocation, img)
		{
            listener = services.RequireService<IEventListener>();
            EndianImageReader rdr = new LeImageReader(RawImage, peOffset);
			if (rdr.ReadByte() != 'P' ||
				rdr.ReadByte() != 'E' ||
				rdr.ReadByte() != 0x0 ||
				rdr.ReadByte() != 0x0)
			{
				throw new BadImageFormatException("Not a valid PE header.");
			}
            ImageSymbols = new SortedList<Address, ImageSymbol>();
            var hdr = new PeHeader();
            this.binaryImage = new PeBinaryImage(imageLocation, hdr);
            short expectedMagic = ReadCoffHeader(rdr, hdr);
            binaryImage.AddSections(LoadSections(rvaSectionTable, sections));
            ReadOptionalHeader(rdr, expectedMagic, hdr);
        }
#nullable enable

        public SortedList<Address, ImageSymbol> ImageSymbols { get; private set; }
        public SegmentMap SegmentMap { get; private set; }
        public uint RvaStartAddress { get; private set; }		// unrelocated start address of the image.

        protected override SizeSpecificLoader Create32BitLoader(AbstractPeLoader outer) => new Pe32Loader(this);
        protected override SizeSpecificLoader Create64BitLoader(AbstractPeLoader outer) => new Pe64Loader(this);

        private void LoadExports(uint rvaExportTable, uint sizeExportTable)
        {
            var offsetExports = this.RvaToFileOffset(rvaExportTable);
            if (offsetExports is null)
                return;
            EndianImageReader rdr = new LeImageReader(
                this.RawImage,
                offsetExports.Value);
            uint characteristics = rdr.ReadLeUInt32();
            uint timestamp = rdr.ReadLeUInt32();
            uint version = rdr.ReadLeUInt32();
            uint binaryNameAddr = rdr.ReadLeUInt32();
            int baseOrdinal = rdr.ReadLeInt32();

            int nExports = rdr.ReadLeInt32();
            int nNames = rdr.ReadLeInt32();
            uint rvaApfn = rdr.ReadLeUInt32();
            uint rvaNames = rdr.ReadLeUInt32();

            var rdrAddrs = CreateLeReaderFromRva(rvaApfn);
            if (rdrAddrs is null)
                return;

            EndianImageReader? rdrNames = nNames != 0
                ? CreateLeReaderFromRva(rvaNames)
                : null;
            trace.Verbose("== Exports");
            for (int i = 0; i < nExports; ++i)
            {
                binaryImage.Exports.Add(LoadExport(rdrAddrs, i < nNames ? rdrNames : null));
            }
        }

        private EndianImageReader? CreateLeReaderFromRva(uint rva)
        {
            if (rva == 0)
                return null;
            var offset = this.RvaToFileOffset(rva);
            if (offset is null)
                return null;
            EndianImageReader rdr = new LeImageReader(
                RawImage,
                offset.Value);
            return rdr;
        }

        private void AddExportedEntryPoints(Address addrLoad, SegmentMap imageMap, IDictionary<Address, ImageSymbol> entryPoints)
		{
            trace.Verbose("== Exports");
            for (int i = 0; i < binaryImage.Exports.Count; ++i)
			{
                var exp = binaryImage.Exports[i];
                ImageSymbol ep = LoadEntryPoint(addrLoad, exp);
				if (imageMap.IsExecutableAddress(ep.Address))
				{
                    ep.Ordinal = binaryImage.ExportBaseOrdinal + i;
                    ImageSymbols[ep.Address] = ep;
                    trace.Verbose("  {0,-8} {1} {2}", ep.Ordinal, ep.Address, ep.Name!);
					entryPoints[ep.Address] = ep;
				}
			}
		}

        private List<ImageSymbol> LoadCoffSymbols(Address addrLoad)
        {
            var result = new List<ImageSymbol>();
            var rdr = new LeImageReader(RawImage, this.offsetCoffSymbols, this.offsetCoffStrings);
            while (TryLoadCoffSymbol(rdr, addrLoad, out var symbol))
            {
                if (symbol is not null)
                    result.Add(symbol);
            }
            return result;
        }

        private bool TryLoadCoffSymbol(LeImageReader rdr, Address addrLoad, out ImageSymbol? symbol)
        {
            symbol = null;
            if (!rdr.TryReadLeUInt32(out uint uName))
                return false;
            string? name;
            if (uName == 0)
            {
                // If first 4 bytes of name are 0, it is a long name.
                if (!rdr.TryReadLeUInt32(out uName))
                    return false;
                name = ReadUtf8String(new ByteMemoryArea(PreferredBaseAddress, RawImage), offsetCoffStrings + uName, 256);
            }
            else
            {
                rdr.Offset -= 4;
                var bytes = rdr.ReadBytes(8);
                name = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            }
            if (!rdr.TryReadLeUInt32(out uint value))
                return false;
            if (!rdr.TryReadLeInt16(out short sectionNumber))
                return false;
            if (!rdr.TryReadLeUInt16(out ushort type))
                return false;
            if (!rdr.TryReadByte(out byte storageClass))
                return false;
            if (!rdr.TryReadByte(out byte cAuxSymbols))
                return false;

            if (cAuxSymbols != 0)
                rdr.Offset += cAuxSymbols * CoffSymbolSize;

            string sectionName;
            PeSection? section;
            switch (sectionNumber)
            {
            default:
                section = binaryImage.Sections[sectionNumber];
                sectionName = section.Name ?? "(null)";
                break;
            case 0:
                section = null;
                sectionName = "";
                break;
            case -1:
                section = null;
                sectionName = "(abs)";
                break;
            case -2:
                section = null;
                sectionName = "(debug)";
                break;
            }
            trace.Verbose("Name: {0,-52} Value: {1:X8} Section:{2,-10} ({3,4}) Type: {4:X4} Class: {5:X2} Aux: {6:X2}",
                name ?? "(null)", value, sectionName, sectionNumber, type, storageClass, cAuxSymbols);

            // Ignore section names.
            if (storageClass == IMAGE_SYM_CLASS_STATIC ||
                storageClass == IMAGE_SYM_CLASS_FILE)
                return true;
            if (sectionNumber <= 0)
                return true;

            if (section is not null)
            {
                var addr = addrLoad + section.VirtualAddress + value;
                symbol = ImageSymbol.Create(SymbolType.Unknown, this.arch, addr, name);
            }
            else
            {
                symbol = null;
            }
            return true;
        }

        private PeExport LoadExport(EndianImageReader rdrAddrs, EndianImageReader? rdrNames)
        {
            uint rvaAddr = rdrAddrs.ReadLeUInt32();
            string? name = null;
            if (rdrNames is not null)
            {
                uint rvaName = rdrNames.ReadLeUInt32();
                var rdrName = CreateLeReaderFromRva(rvaName);
                if (rdrName is not null)
                    name = rdrName.ReadNulTerminatedString(PrimitiveType.Byte, Encoding.ASCII);
            }
            return new PeExport(rvaAddr, name);
        }

        private ImageSymbol LoadEntryPoint(Address addrLoad, PeExport exp)
        {
            uint rvaAddr = exp.RvaAddress;
            string? name = exp.Name;
            return ImageSymbol.Procedure(
                arch,
                addrLoad + rvaAddr,
                name,
                new FunctionType(),
                state: arch.CreateProcessorState());
        }

        private short GetExpectedMagic(ushort peMachineType)
        {
            switch (peMachineType)
            {
            case MACHINE_ALPHA:
            case MACHINE_ARM:
            case MACHINE_ARMNT:
            case MACHINE_I386:
			case MACHINE_m68k:
            case MACHINE_R4000:
            case MACHINE_POWERPC:
            case MACHINE_POWERPC_BE:
            case MACHINE_XBOX360:
                return 0x010B;
            case MACHINE_x86_64:
            case MACHINE_ARM64:
                return 0x020B;
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
        }

        public override Program LoadProgram(Address? a)
        {
            var addrLoad = a ?? PreferredBaseAddress;
            SegmentMap = new SegmentMap(addrLoad);
            if (sections > 0)
            {
                imgLoaded = LoadSectionBytes(addrLoad, binaryImage.Sections);
                AddSectionsToSegmentMap(addrLoad, SegmentMap);
            }
            if (offsetCoffSymbols > 0 && cCoffSymbols > 0)
            {
                var syms = LoadCoffSymbols(addrLoad);
                foreach (var sym in syms)
                {
                    ImageSymbols[sym.Address] = sym;
                }
            }
            this.program = new Program(new ByteProgramMemory(SegmentMap), arch, platform, ImageSymbols, new())
            {
                Name = this.ImageLocation.GetFilename()
            };
            this.importReferences = program.ImportReferences;

            var rsrcLoader = new ResourceLoader(this.imgLoaded, rvaResources);
            List<ProgramResource> items = rsrcLoader.Load();
            if (items.Count > 0)
            {
                program.Resources.AddRange(items);
            }
            Relocate(program, addrLoad);
            LoadDebugInformation();
            CollectRttiInformation();
            return program;
        }

        private void LoadDebugInformation()
        {
            if (rvaDebug == 0)
                return;
            var rdr = imgLoaded.CreateLeReader(rvaDebug, rvaDebug + cbDebug);
            while (rdr.IsValid)
            {
                if (!rdr.TryReadUInt32(out var characteristics))
                    break;
                if (characteristics != 0)   // Should be 0 according to PE spec.
                    break;
                if (!rdr.TryReadUInt32(out var timestamp))
                    break;
                if (!rdr.TryReadUInt32(out var major_minor_version))
                    break;
                if (!rdr.TryReadUInt32(out var debugInfoType))
                    break;
                if (!rdr.TryReadUInt32(out var sizeOfData))
                    break;
                if (!rdr.TryReadUInt32(out var uAddrRawData))
                    break;
                if (!rdr.TryReadUInt32(out var rvaRawData))
                    break;

                switch (debugInfoType)
                {
                case IMAGE_DEBUG_TYPE_CODEVIEW:
                    var rdrCodeview = new LeImageReader(RawImage, rvaRawData, rvaRawData + sizeOfData);
                    var pdbref = PdbFileReference.Load(rdrCodeview);
                    if (pdbref is null)
                        listener.Warn("Unable to read debug information.");
                    else
                    {
                        var pdbResolver = new PdbFileResolver(Services.RequireService<IFileSystemService>(), listener);
                        var symsrc = pdbResolver.Load(pdbref);
                    }
                    break;
                default:
                    listener.Info($"PE Debug type {debugInfoType} not supported yet.");
                    break;
                }
            }
        }

        private void CollectRttiInformation()
        {
            var rtti = new RttiScanner(program, listener);
            var results = rtti.Scan();
            foreach (var symbol in results.Symbols)
            {
                program.ImageSymbols.TryAdd(symbol.Address, symbol);
            }
            foreach (var pfn in results.VFTables.Values.SelectMany(t => t))
            {
                var fnSym = ImageSymbol.Procedure(program.Architecture, pfn);
                program.ImageSymbols.TryAdd(pfn, fnSym);
            }
        }

		/// <summary>
		/// Loads the sections
		/// </summary>
		/// <param name="rvaSectionTable"></param>
		/// <returns></returns>
		private List<PeSection> LoadSections(uint rvaSectionTable, int sections)
        {
            var sectionMap = new List<PeSection>();
			EndianImageReader rdr = new LeImageReader(RawImage, rvaSectionTable);

            PeSection? minSection = null;
            trace.Inform("          Name        Raw data Raw size VAddress Ld size  Flags");
			for (int i = 0; i != sections; ++i)
			{
				PeSection section = ReadSection(i, rdr);
				sectionMap.Add(section);
				if (minSection is null || section.VirtualAddress < minSection.VirtualAddress)
					minSection = section;
                trace.Inform("  Section: {0,-10} {1:X8} {2:X8} {3:X8} {4:X8} {5:X8}", section.Name ?? "(null)", section.OffsetRawData, section.SizeRawData, section.VirtualAddress, section.VirtualSize, section.Flags);
			}

            // Map the area between addrLoad and the lowest section.
            if (minSection is not null && 0 < minSection.VirtualAddress)
            {
                sectionMap.Insert(0, new PeSection(-1)
                {
                    Name = "(PE header)",
                    OffsetRawData= 0,
                    SizeRawData = minSection.OffsetRawData,
                    VirtualAddress = 0,
                    VirtualSize = minSection.OffsetRawData,
                    IsHidden = true,
                });
            }
            return sectionMap;
		}

        public ByteMemoryArea LoadSectionBytes(Address addrLoad, IReadOnlyList<PeSection> sections)
        {
            var vaMax = sections.Max(s => s.VirtualAddress);
            var sectionMax = sections.Where(s => s.VirtualAddress == vaMax).First();
            var imgLoaded = new ByteMemoryArea(addrLoad, new byte[sectionMax.VirtualAddress + Math.Max(sectionMax.VirtualSize, sectionMax.SizeRawData)]);
            foreach (PeSection s in binaryImage.Sections)
            {
                Array.Copy(RawImage, s.OffsetRawData, imgLoaded.Bytes, s.VirtualAddress, s.SizeRawData);
            }
            return imgLoaded;
        }

		public override Address PreferredBaseAddress
		{
			get { return this.binaryImage.Header.preferredBaseOfImage; }
            set { throw new NotImplementedException(); }
        }

		public short ReadCoffHeader(EndianImageReader rdr, PeHeader hdr)
		{
			this.machine = rdr.ReadLeUInt16();
            short expectedMagic = GetExpectedMagic(machine);
            arch = CreateArchitecture(machine);
			platform = CreatePlatform(machine, Services, arch);
            innerLoader = CreateInnerLoader(machine);

			sections = rdr.ReadLeInt16();
			rdr.ReadLeUInt32();		// timestamp.
			offsetCoffSymbols = rdr.ReadLeUInt32();		// COFF symbol table.
            cCoffSymbols = rdr.ReadLeUInt32();		// #of symbols.
            offsetCoffStrings = offsetCoffSymbols + cCoffSymbols * CoffSymbolSize;  // COFF string table starts after symbol table.
			optionalHeaderSize = rdr.ReadLeInt16();
			this.fileFlags = rdr.ReadLeUInt16();
			rvaSectionTable = (uint) ((int)rdr.Offset + optionalHeaderSize);
            return expectedMagic;
		}

		public void ReadOptionalHeader(EndianImageReader rdr, short expectedMagic, PeHeader hdr)
		{
			if (optionalHeaderSize <= 0)
				throw new BadImageFormatException("Optional header size should be larger than 0 in a PE executable image file.");

			short magic = rdr.ReadLeInt16();
			if (magic != expectedMagic) 
				throw new BadImageFormatException("Not a valid PE Header.");

			rdr.ReadByte();		// Linker major version
			rdr.ReadByte();		// Linker minor version
			rdr.ReadLeUInt32();		// code size (== .text section size)
			rdr.ReadLeUInt32();		// size of initialized data
			rdr.ReadLeUInt32();		// size of uninitialized data
			RvaStartAddress = rdr.ReadLeUInt32();
			uint rvaBaseOfCode = rdr.ReadLeUInt32();
            hdr.preferredBaseOfImage = innerLoader.ReadPreferredImageBase(rdr);
			rdr.ReadLeUInt32();		// section alignment
			rdr.ReadLeUInt32();		// file alignment
			rdr.ReadLeUInt16();		// OS major version
			rdr.ReadLeUInt16();		// OS minor version
			rdr.ReadLeUInt16();		// Image major version
			rdr.ReadLeUInt16();		// Image minor version
			rdr.ReadLeUInt16();		// Subsystem major version
			rdr.ReadLeUInt16();		// Subsystem minor version
			rdr.ReadLeUInt32();		// reserved
			uint sizeOfImage = rdr.ReadLeUInt32();
			uint sizeOfHeaders = rdr.ReadLeUInt32();
			uint checksum = rdr.ReadLeUInt32();
			ushort subsystem = rdr.ReadLeUInt16();
			ushort dllFlags = rdr.ReadLeUInt16();
            var stackReserve = innerLoader.ReadWord(rdr);
            var stackCommit = innerLoader.ReadWord(rdr);
            var heapReserve = innerLoader.ReadWord(rdr);
            var heapCommit = innerLoader.ReadWord(rdr);
			rdr.ReadLeUInt32();			// loader flags
			uint directoryCount = rdr.ReadLeUInt32();

            // Export directory
            if (directoryCount == 0) return;
            var rvaExportTable = rdr.ReadLeUInt32();
            var sizeExportTable = rdr.ReadLeUInt32();
            LoadExports(rvaExportTable, sizeExportTable);

            // Import directory
            if (--directoryCount == 0) return;
            this.rvaImportTable = rdr.ReadLeUInt32();
			uint importTableSize = rdr.ReadLeUInt32();

            // Resource directory
            if (--directoryCount == 0) return;
            this.rvaResources = rdr.ReadLeUInt32();			// resource address
			rdr.ReadLeUInt32();			// resource size

            // Exception table
            if (--directoryCount == 0) return;
			this.rvaExceptionTable = rdr.ReadLeUInt32();            // exception address
            this.sizeExceptionTable = rdr.ReadLeUInt32();			// exception size

            // Certificate table
            if (--directoryCount == 0) return;
			rdr.ReadLeUInt32();			// certificate address
			rdr.ReadLeUInt32();			// certificate size

            // Base relocation table (.reloc)
            if (--directoryCount == 0) return;
            this.rvaBaseRelocationTable = rdr.ReadLeUInt32();
            this.sizeBaseRelocationTable = rdr.ReadLeUInt32();

            // Debug data dictionary
            if (--directoryCount == 0) return;
            this.rvaDebug = rdr.ReadLeUInt32();
            this.cbDebug = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaArchitecture = rdr.ReadLeUInt32();
            uint cbArchitecture = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaGlobalPointer = rdr.ReadLeUInt32();
            uint cbGlobalPointer = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaTls = rdr.ReadLeUInt32();
            uint cbTls = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaLoadConfig = rdr.ReadLeUInt32();
            uint cbLoadConfig = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaBoundImport = rdr.ReadLeUInt32();
            uint cbBoundImport = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            uint rvaIat = rdr.ReadLeUInt32();
            uint cbIat = rdr.ReadLeUInt32();

            if (--directoryCount == 0) return;
            this.rvaDelayImportDescriptor = rdr.ReadLeUInt32();
            uint cbDelayImportDescriptor = rdr.ReadLeUInt32();
		}



        // MIPS relocation types.
        // http://www.docjar.com/docs/api/sun/jvm/hotspot/debugger/win32/coff/TypeIndicators.html
        private const ushort IMAGE_REL_MIPS_ABSOLUTE       = 0x0000;  // Reference is absolute, no relocation is necessary
        private const ushort IMAGE_REL_MIPS_REFHALF        = 0x0001;
        private const ushort IMAGE_REL_MIPS_REFWORD        = 0x0002;
        private const ushort IMAGE_REL_MIPS_JMPADDR        = 0x0003;
        private const ushort IMAGE_REL_MIPS_REFHI          = 0x0004;
        private const ushort IMAGE_REL_MIPS_REFLO          = 0x0005;
        private const ushort IMAGE_REL_MIPS_GPREL          = 0x0006;
        private const ushort IMAGE_REL_MIPS_LITERAL        = 0x0007;
        private const ushort IMAGE_REL_MIPS_SECTION        = 0x000A;
        private const ushort IMAGE_REL_MIPS_SECREL         = 0x000B;
        private const ushort IMAGE_REL_MIPS_SECRELLO       = 0x000C;  // Low 16-bit section relative referemce (used for >32k TLS)
        private const ushort IMAGE_REL_MIPS_SECRELHI       = 0x000D;  // High 16-bit section relative reference (used for >32k TLS)
        private const ushort IMAGE_REL_MIPS_TOKEN          = 0x000E;  // clr token
        private const ushort IMAGE_REL_MIPS_JMPADDR16      = 0x0010;
        private const ushort IMAGE_REL_MIPS_REFWORDNB      = 0x0022;
        private const ushort IMAGE_REL_MIPS_PAIR = 0x0025;

        // ARM relocation types
        private const ushort IMAGE_REL_ARM_ABSOLUTE        = 0x0000; // No relocation required
        private const ushort IMAGE_REL_ARM_ADDR32          = 0x0001; // 32 bit address
        private const ushort IMAGE_REL_ARM_ADDR32NB        = 0x0002; // 32 bit address w/o image base
        private const ushort IMAGE_REL_ARM_BRANCH24        = 0x0003; // 24 bit offset << 2 & sign ext.
        private const ushort IMAGE_REL_ARM_BRANCH11        = 0x0004; // Thumb: 2 11 bit offsets
        private const ushort IMAGE_REL_ARM_TOKEN           = 0x0005; // clr token
        private const ushort IMAGE_REL_ARM_GPREL12         = 0x0006; // GP-relative addressing (ARM)
        private const ushort IMAGE_REL_ARM_GPREL7          = 0x0007; // GP-relative addressing (Thumb)
        private const ushort IMAGE_REL_ARM_BLX24           = 0x0008;
        private const ushort IMAGE_REL_ARM_BLX11           = 0x0009;
        private const ushort IMAGE_REL_ARM_SECTION         = 0x000E; // Section table index
        private const ushort IMAGE_REL_ARM_SECREL = 0x000F; // Offset within section

        public void Relocate(Program program, Address addrLoad)
		{
            relocator = CreateRelocator(machine, program);
            var relocations = imgLoaded.Relocations;
			PeSection? relocSection;
            if (rvaBaseRelocationTable != 0 &&
                (relocSection = binaryImage.FindSection(section => 
                    this.rvaBaseRelocationTable >= section.VirtualAddress &&
                    this.rvaBaseRelocationTable < section.VirtualAddress + section.VirtualSize)) is not null)
			{
                ApplyRelocations(relocSection.OffsetRawData, relocSection.SizeRawData, addrLoad, relocations);
			} 
            var addrEp = platform.AdjustProcedureAddress(addrLoad + RvaStartAddress);
            var entrySym = CreateMainEntryPoint(
                    (this.fileFlags & ImageFileDll) != 0,
                    addrEp,
                    platform);
            ImageSymbols[entrySym.Address] = entrySym;
            program.EntryPoints[entrySym.Address] = entrySym;
            ReadExceptionRecords(addrLoad, rvaExceptionTable, sizeExceptionTable, ImageSymbols);
            AddExportedEntryPoints(addrLoad, SegmentMap, program.EntryPoints);
            ReadImportDescriptors(addrLoad);
            ReadDeferredLoadDescriptors(addrLoad);
		}

        /// <summary>
        /// All PE executables have a principal entry point, the WinMain or DllMain.
        /// </summary>
        /// <param name="isDll"></param>
        /// <param name="addrEp"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public ImageSymbol CreateMainEntryPoint(bool isDll, Address addrEp, IPlatform platform)
        {
            var s = platform.FindMainProcedure(this.program, addrEp);
            if (s is not null)
                return s;

            string? name;
            SerializedSignature? ssig;

            static Argument_v1 Arg(string? n, string t) => new Argument_v1
            {
                Name = n,
                Type = new TypeReference_v1 { TypeName = t }
            };
            if (isDll)
            {
                name = "DllMain";   //$TODO: ensure users can override this name
                ssig = new SerializedSignature
                {
                    Convention = "stdapi",
                    Arguments = new Argument_v1[]
                    {
                        Arg("hModule", "HANDLE"),
                        Arg("dwReason", "DWORD"),
                        Arg("lpReserved", "LPVOID")
                    },
                    ReturnValue = Arg(null, "BOOL")
                };
            }
            else
            {
                name = "Win32CrtStartup";
                ssig = new SerializedSignature
                {
                    Convention = "__cdecl",
                    ReturnValue = Arg(null, "DWORD")
                };
            }
            var entrySymbol = ImageSymbol.Procedure(
                arch,
                addrEp,
                name,
                state: arch.CreateProcessorState());
            entrySymbol.Signature = ssig;
            return entrySymbol;
        }

        public void AddSectionsToSegmentMap(Address addrLoad, SegmentMap segmentMap)
        {
            foreach (PeSection s in binaryImage.Sections)
            {
                var acc = s.AccessMode;
                bool isBss = 
                    (s.Flags & IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0 &&
                    s.Name == ".bss";
                var seg = segmentMap.AddSegment(new ImageSegment(
                    s.Name!,
                    addrLoad + s.VirtualAddress,
                    imgLoaded,
                    acc)
                {
                    Size = s.VirtualSize,
                    IsHidden = s.IsHidden,
                    IsBss = isBss,
                });
                seg.IsDiscardable = s.IsDiscardable;
            }
        }



#if I386
            //
// I386 relocation types.
//
const static final ushort IMAGE_REL_I386_ABSOLUTE        = 0x0000;  // Reference is absolute, no relocation is necessary
const static final ushort IMAGE_REL_I386_DIR16           = 0x0001;  // Direct 16-bit reference to the symbols virtual address
const static final ushort IMAGE_REL_I386_REL16           = 0x0002;  // PC-relative 16-bit reference to the symbols virtual address
const static final ushort IMAGE_REL_I386_DIR32           = 0x0006;  // Direct 32-bit reference to the symbols virtual address
const static final ushort IMAGE_REL_I386_DIR32NB         = 0x0007;  // Direct 32-bit reference to the symbols virtual address, base not included
const static final ushort IMAGE_REL_I386_SEG12           = 0x0009;  // Direct 16-bit reference to the segment-selector bits of a 32-bit virtual address
const static final ushort IMAGE_REL_I386_SECTION         = 0x000A;
const static final ushort IMAGE_REL_I386_SECREL          = 0x000B;
const static final ushort IMAGE_REL_I386_TOKEN           = 0x000C;  // clr token
const static final ushort IMAGE_REL_I386_SECREL7         = 0x000D;  // 7 bit offset from base of section containing target
const static final ushort IMAGE_REL_I386_REL32           = 0x0014;  // PC-relative 32-bit reference to the symbols virtual address
#endif

#if ARM
public static final  short 	IMAGE_REL_ARM_ABSOLUTE    	This relocation is ignored. 
public static final  short 	IMAGE_REL_ARM_ADDR32    	The target's 32-bit virtual address. 
public static final  short 	IMAGE_REL_ARM_ADDR32NB    	The target's 32-bit relative virtual address. 
public static final  short 	IMAGE_REL_ARM_BRANCH24    	The 24-bit relative displacement to the target. 
public static final  short 	IMAGE_REL_ARM_BRANCH11    	Reference to a subroutine call, consisting of two 16-bit instructions with 11-bit offsets. 
public static final  short 	IMAGE_REL_ARM_SECTION    	The 16-bit section index of the section containing the target. This is used to support debugging information. 
public static final  short 	IMAGE_REL_ARM_SECREL    	The 32-bit offset of the target from the beginning of its section. This is used to support debugging information as well as static thread local storage. 
#endif

#if NYI
        static void add16(uint8_t* P, int16_t V) { write16le(P, read16le(P) + V); }
        static void add32(uint8_t* P, int32_t V) { write32le(P, read32le(P) + V); }
        static void add64(uint8_t* P, int64_t V) { write64le(P, read64le(P) + V); }
        static void or16(uint8_t* P, uint16_t V) { write16le(P, read16le(P) | V); }

        void SectionChunk::applyRelX64(uint8_t* Off, uint16_t Type, Defined* Sym,
                               uint64_t P)  {
  uint64_t S = Sym->getRVA();
  switch (Type) {
  case IMAGE_REL_AMD64_ADDR32:   add32(Off, S + Config->ImageBase); break;
  case IMAGE_REL_AMD64_ADDR64:   add64(Off, S + Config->ImageBase); break;
  case IMAGE_REL_AMD64_ADDR32NB: add32(Off, S); break;
  case IMAGE_REL_AMD64_REL32:    add32(Off, S - P - 4); break;
  case IMAGE_REL_AMD64_REL32_1:  add32(Off, S - P - 5); break;
  case IMAGE_REL_AMD64_REL32_2:  add32(Off, S - P - 6); break;
  case IMAGE_REL_AMD64_REL32_3:  add32(Off, S - P - 7); break;
  case IMAGE_REL_AMD64_REL32_4:  add32(Off, S - P - 8); break;
  case IMAGE_REL_AMD64_REL32_5:  add32(Off, S - P - 9); break;
  case IMAGE_REL_AMD64_SECTION:  add16(Off, Sym->getSectionIndex()); break;
  case IMAGE_REL_AMD64_SECREL:   add32(Off, Sym->getSecrel()); break;
  default:
    error("Unsupported relocation type");
    }
}

void applyRelX86(uint8_t* Off, uint16_t Type, Defined* Sym,
                               uint64_t P)  {
  uint64_t S = Sym->getRVA();
  switch (Type) {
  case IMAGE_REL_I386_ABSOLUTE: break;
  case IMAGE_REL_I386_DIR32:    add32(Off, S + Config->ImageBase); break;
  case IMAGE_REL_I386_DIR32NB:  add32(Off, S); break;
  case IMAGE_REL_I386_REL32:    add32(Off, S - P - 4); break;
  case IMAGE_REL_I386_SECTION:  add16(Off, Sym->getSectionIndex()); break;
  case IMAGE_REL_I386_SECREL:   add32(Off, Sym->getSecrel()); break;
  default:
    error("Unsupported relocation type");
  }
}


        static void applyMOV(uint8_t* Off, uint16_t V)
        {
            or16(Off, ((V & 0x800) >> 1) | ((V >> 12) & 0xf));
            or16(Off + 2, ((V & 0x700) << 4) | (V & 0xff));
        }

        static void applyMOV32T(uint8_t* Off, uint32_t V)
        {
            applyMOV(Off, V);           // set MOVW operand
            applyMOV(Off + 4, V >> 16); // set MOVT operand
        }

        static void applyBranch20T(uint8_t* Off, int32_t V)
        {
            uint32_t S = V < 0 ? 1 : 0;
            uint32_t J1 = (V >> 19) & 1;
            uint32_t J2 = (V >> 18) & 1;
            or16(Off, (S << 10) | ((V >> 12) & 0x3f));
            or16(Off + 2, (J1 << 13) | (J2 << 11) | ((V >> 1) & 0x7ff));
        }

        static void applyBranch24T(uint8_t* Off, int32_t V)
        {
            uint32_t S = V < 0 ? 1 : 0;
            uint32_t J1 = ((~V >> 23) & 1) ^ S;
            uint32_t J2 = ((~V >> 22) & 1) ^ S;
            or16(Off, (S << 10) | ((V >> 12) & 0x3ff));
            or16(Off + 2, (J1 << 13) | (J2 << 11) | ((V >> 1) & 0x7ff));
        }
        void ApplyArmRelocation(uint8_t* Off, uint16_t Type, Defined* Sym,
                               uint64_t P)  {
  uint64_t S = Sym->getRVA();
  // Pointer to thumb code must have the LSB set.
  if (Sym->isExecutable())
    S |= 1;
  switch (Type) {
  case IMAGE_REL_ARM_ADDR32:    add32(Off, S + Config->ImageBase); break;
  case IMAGE_REL_ARM_ADDR32NB:  add32(Off, S); break;
  case IMAGE_REL_ARM_MOV32T:    applyMOV32T(Off, S + Config->ImageBase); break;
  case IMAGE_REL_ARM_BRANCH20T: applyBranch20T(Off, S - P - 4); break;
  case IMAGE_REL_ARM_BRANCH24T: applyBranch24T(Off, S - P - 4); break;
  case IMAGE_REL_ARM_BLX23T:    applyBranch24T(Off, S - P - 4); break;
  default:
    error("Unsupported relocation type");
    }
}

#endif
        public void ApplyRelocations(uint rvaReloc, uint size, Address baseOfImage, RelocationDictionary relocations)
		{
            trace.Inform("PELdr: applying relocations {0:X8}", rvaReloc);
			EndianImageReader rdr = new LeImageReader(RawImage, rvaReloc);
			uint rvaStop = rvaReloc + size;
			while (rdr.Offset < rvaStop)
			{
				// Read fixup block header.

				uint page = rdr.ReadLeUInt32();
				int cbBlock = rdr.ReadLeInt32();
                if (page == 0 || cbBlock == 0)
                    break;
				uint offBlockEnd = (uint)((int)rdr.Offset + cbBlock - 8);
				while (rdr.IsValidOffset(rdr.Offset) && rdr.Offset < offBlockEnd)
				{
					relocator.ApplyRelocation(baseOfImage, page, rdr, relocations);
				}
			}
		}

        /// <summary>
        /// Loads the import directory entry for one particular DLL.
        /// </summary>
        /// <remarks>
        /// The goal of this method is to discover the imported DLL's and the names
        /// of all imported methods. This is made difficult by the way different
        /// compilers and linkers build the import directory entries. Sometimes,
        /// the RVA to the Import lookup table (ILT) is null, so we have to use
        /// a last resort and walk the Import Address table (IAT).</remarks>
        /// <param name="rdr"></param>
        /// <param name="addrLoad"></param>
        /// <returns>True if there were entries in the import descriptor, otherwise 
        /// false.</returns>
        public bool ReadImportDescriptor(EndianImageReader rdr, Address addrLoad)
        {
            var rvaILT = rdr.ReadLeUInt32();            // Import lookup table
            rdr.ReadLeUInt32();		                    // Ignore datestamp...
            rdr.ReadLeUInt32();		                    // ...and forwarder chain
            var dllName = ReadUtf8String(imgLoaded, rdr.ReadLeUInt32(), 0);		// DLL name
            var rvaIAT = rdr.ReadLeUInt32();		    // Import address table 
            if (rvaILT == 0 && dllName is null)
                return false;

            var iatOffset = rvaILT != 0 ? rvaILT : rvaIAT;
            EndianImageReader rdrIlt = imgLoaded.CreateLeReader(iatOffset);
            if (!rdrIlt.IsValidOffset(iatOffset))
            {
                return false;
            }
            EndianImageReader rdrIat = imgLoaded.CreateLeReader(rvaIAT);
            while (true)
            {
                var addrIat = rdrIat.Address;
                var addrIlt = rdrIlt.Address;
                var (impRef, bitSize) = innerLoader.ResolveImportDescriptorEntry(imgLoaded, dllName!, rdrIlt, rdrIat);
                if (impRef is null)
                    break;
                ImageSymbols[addrIat] = ImageSymbol.DataObject(
                    arch,
                    addrIat,
                    "__imp__" + impRef.EntryName,
                    new Pointer(new CodeType(), bitSize));

                ImageSymbols[addrIlt] = ImageSymbol.DataObject(
                    arch,
                    addrIlt,
                    null,
                    PrimitiveType.CreateWord(bitSize));
            }
            return true;
        }

        private bool ReadDeferredLoadDescriptors(EndianImageReader rdr, Address addrLoad)
        {
            var symbols = new List<ImageSymbol>();
            var attributes = rdr.ReadLeUInt32();
            var offset = ((attributes & DID_RvaBased) != 0) ? 0 : (uint) addrLoad.ToLinear();
            var rvaDllName = rdr.ReadLeUInt32();
            if (rvaDllName == 0)
                return false;
            var dllName = ReadUtf8String(imgLoaded, rvaDllName - offset, 0);    // DLL name.
            if (dllName is null)
                return false;
            var rdrModule = rdr.ReadLeInt32();
            var rdrThunks = imgLoaded.CreateLeReader(rdr.ReadLeUInt32() - offset);
            var rdrNames = imgLoaded.CreateLeReader(rdr.ReadLeUInt32() - offset);
            for (;;)
            {
                var addrThunk = imgLoaded.BaseAddress + rdrThunks.Offset;
                uint rvaName = rdrNames.ReadLeUInt32();
                uint rvaThunk = rdrThunks.ReadLeUInt32();
                if (rvaName == 0)
                    break;
                rvaName -= offset;
                rvaThunk -= offset;
                var impRef =
                    innerLoader.CreateImportReference(imgLoaded, dllName, rvaName, addrThunk);

                importReferences[addrThunk] = impRef;
            }
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();  // time stamp
            return true;
        }

		private void ReadImportDescriptors(Address addrLoad)
		{
            if (rvaImportTable == 0)
                return;
			EndianImageReader rdr = imgLoaded.CreateLeReader(rvaImportTable);
			while (ReadImportDescriptor(rdr, addrLoad))
			{
			}
		}

        private void ReadDeferredLoadDescriptors(Address addrLoad)
        {
            if (rvaDelayImportDescriptor == 0)
                return;
            var rdr = imgLoaded.CreateLeReader(rvaDelayImportDescriptor);
            while (ReadDeferredLoadDescriptors(rdr, addrLoad))
            {
            }
        }

		private PeSection ReadSection(int index, EndianImageReader rdr)
		{
			var name = ReadSectionName(rdr, offsetCoffSymbols + cCoffSymbols * 18u)!;
			var virtualSize = rdr.ReadLeUInt32();
			var virtualAddress = rdr.ReadLeUInt32();

			if (name is null) {
				name = ".reko_" + virtualAddress.ToString("x16");
			}
            var sec = new PeSection(index);
            sec.Name = name;
            sec.VirtualSize = virtualSize;
            sec.VirtualAddress = virtualAddress;
            sec.SizeRawData = rdr.ReadLeUInt32();
			sec.OffsetRawData = rdr.ReadLeUInt32();
			rdr.ReadLeUInt32();			// pointer to relocations
			rdr.ReadLeUInt32();			// pointer to line numbers.
			rdr.ReadLeInt16();		// # of relocations
			rdr.ReadLeInt16();		// # of line numbers
			sec.Flags = rdr.ReadLeUInt32();
			return sec;
		}

        public class PeSection : IBinarySection
		{
            public PeSection(int index)
            {
                this.Name = "";
                this.VirtualAddress = default!;
                this.Index = index;
            }

            public int Index { get; set; }

            public string Name { get; set; }

			public uint VirtualSize { get; set; }

            ulong IBinarySection.Size => VirtualSize;

			public uint VirtualAddress { get; set; }
            Address IBinarySection.VirtualAddress => Address.Ptr32(this.VirtualAddress);

            public uint SizeRawData { get; set; }

			public ulong Flags { get; set; }

			public uint OffsetRawData { get; set; }

            ulong IBinarySection.FileOffset => OffsetRawData;

            ulong IBinarySection.FileSize => SizeRawData;

            public ulong Alignment { get; set; }

            public bool IsHidden { get; set; }

			public bool IsDiscardable
			{
				get { return (Flags & SectionFlagsDiscardable) != 0; }
			}

            public AccessMode AccessMode => AccessFromCharacteristics(this.Flags);

        }

        public uint? RvaToFileOffset(ulong rva)
        {
            foreach (var section in binaryImage.Sections)
            {
                if (section.VirtualAddress <= rva && rva < section.VirtualAddress + section.VirtualSize)
                    return (uint) (rva - section.VirtualAddress) + section.OffsetRawData;
            }
            return null;
        }

        public void ReadExceptionRecords(
            Address addrLoad,
            uint rvaExceptionTable, 
            uint sizeExceptionTable,
            SortedList<Address, ImageSymbol> symbols)
        {
            var rvaTableEnd = rvaExceptionTable + sizeExceptionTable; 
            if (rvaExceptionTable == 0 || sizeExceptionTable == 0)
                return;
            var rdr = new LeImageReader(this.imgLoaded.Bytes, rvaExceptionTable);
            switch (machine)
            {
            default: 
                listener.Warn(
                    new NullCodeLocation(ImageLocation.FilesystemPath),
                    "Exception table reading not supported for machine #{0}.", machine);
                break;
            case MACHINE_x86_64:
                while (rdr.Offset < rvaTableEnd)
                {
                    var addr = addrLoad + rdr.ReadLeUInt32();
                    var addrEnd = addrLoad + rdr.ReadLeUInt32();
                    var addrUnwind = addrLoad + rdr.ReadLeUInt32();

                    var rdrUnwind = this.imgLoaded.CreateLeReader(addrUnwind);
                    var flags = rdrUnwind.ReadByte();
                    var cbProlog = rdrUnwind.ReadByte();
                    var cUnwindCodes = rdrUnwind.ReadByte();
                    var frameReg = rdrUnwind.ReadByte();

                    const int UNW_FLAG_CHAININFO = 0x20;
                    if ((flags & 0xF0) != UNW_FLAG_CHAININFO)
                    {
                        // Only visit handlers that don't have the UNW_FLAG_CHAININFO
                        // flag set. If the flag _is_ set, then addr is pointing into
                        // the middle of a procedure. Such addresses shouldn't be used 
                        // as function symbol addresses.
                        AddFunctionSymbol(addr, symbols);
                    }
                }
                break;
            case MACHINE_R4000:
                while (rdr.Offset < rvaTableEnd)
                {
                    var addr = Address.Ptr32(rdr.ReadLeUInt32());
                    rdr.Seek(16);
                    AddFunctionSymbol(addr, symbols);
                }
                break;
            }
        }

        private void AddFunctionSymbol(Address addr, SortedList<Address, ImageSymbol> symbols)
        {
            var symNew = ImageSymbol.Procedure(arch, addr, null, new CodeType());
            symNew.ProcessorState = arch.CreateProcessorState();
            if (!symbols.TryGetValue(addr, out var symOld))
            {
                symbols.Add(addr, symNew);
            }
            else
            {
                if (symOld.Name is null && symNew.Name is not null)
                    symbols[addr] = symNew;
            }
        }
    }
}
