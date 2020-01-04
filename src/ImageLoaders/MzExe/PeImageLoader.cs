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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Services;
using Reko.Environments.Windows;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Reko.Core.Configuration;
using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.ImageLoaders.MzExe.Pe;

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Loads Windows NT PE images.
    /// http://geos.icc.ru:8080/scripts/wwwbinv.dll/ShowR?coff.rfi
    /// </summary>
	public class PeImageLoader : ImageLoader
	{
        internal static TraceSwitch trace = new TraceSwitch(nameof(PeImageLoader), "Traces the progress of loading PE binary images") { Level = TraceLevel.Verbose };

        private const ushort MACHINE_x86_64 = (ushort) 0x8664u;
		private const ushort MACHINE_m68k = (ushort)0x0268;
        private const ushort MACHINE_UNKNOWN = 0x0;         // The contents of this field are assumed to be applicable to any machine type
        private const ushort MACHINE_AM33 = 0x1d3;          // Matsushita AM33
        private const ushort MACHINE_AMD64 = (ushort) 0x8664;    // x64
        private const ushort MACHINE_ARM = 0x01c0;          // ARM little endian -- "Modern" Windows
        private const ushort MACHINE_ARM64 = 0xAA64;        // ARM64 little endian
        private const ushort MACHINE_ARMNT = 0x01C4;        // ARM Thumb-2 little endian -- WinCE
        private const ushort MACHINE_EBC =  0x0ebc;         // EFI byte code
        private const ushort MACHINE_I386 = 0x014c;         // Intel 386 or later processors and compatible processors
        private const ushort MACHINE_IA64 = 0x0200;         // Intel Itanium processor family
        private const ushort MACHINE_M32R = 0x9041;         // Mitsubishi M32R little endian
        private const ushort MACHINE_MIPS16 = 0x0266;       // MIPS16
        private const ushort MACHINE_MIPSFPU = 0x0366;      // MIPS with FPU
        private const ushort MACHINE_MIPSFPU16 = 0x0466;    // MIPS16 with FPU
        private const ushort MACHINE_POWERPC = 0x01f0;      // Power PC little endian
        private const ushort MACHINE_POWERPCFP = 0x01f1;    // Power PC with floating point support
        private const ushort MACHINE_POWERPC_BE = (ushort) 0x0601;       // Big-endian PC: intended for PowerMac (!)
        private const ushort MACHINE_XBOX360 = 0x01F2;         // Xbox 360
        private const ushort MACHINE_R4000 = 0x0166;        // MIPS little endian
        private const ushort MACHINE_RISCV32 = 0x5032;      // RISC-V 32-bit address space
        private const ushort MACHINE_RISCV64 = 0x5064;      // RISC-V 64-bit address space
        private const ushort MACHINE_RISCV128 = 0x5128;     // RISC-V 128-bit address space
        private const ushort MACHINE_SH3 = 0x01a2;          // Hitachi SH3
        private const ushort MACHINE_SH3DSP = 0x01a3;       // Hitachi SH3 DSP
        private const ushort MACHINE_SH4 = 0x01a6;          // Hitachi SH4
        private const ushort MACHINE_SH5 = 0x01a8;          // Hitachi SH5
        private const ushort MACHINE_THUMB = 0x01c2;        // Thumb
        private const ushort MACHINE_WCEMIPSV2 = 0x0169;    // MIPS little-endian WCE v2


        private const ushort MACHINE_ALPHA = (ushort)0x0184;

        private const short ImageFileRelocationsStripped = 0x0001;
        private const short ImageFileExecutable = 0x0002;
        private const short ImageFileDll = 0x2000;
        public const uint DID_RvaBased = 1;

        private IProcessorArchitecture arch;
        private IPlatform platform;
        private SizeSpecificLoader innerLoader;
        private Program program;

        private ushort machine;
		private short optionalHeaderSize;
        private ushort fileFlags;
		private int sections;
        private uint rvaSectionTable;
		private MemoryArea imgLoaded;
		private Address preferredBaseOfImage;
        private List<Section> sectionList;
        private Dictionary<uint, PseudoProcedure> importThunks;
		private uint rvaStartAddress;		// unrelocated start address of the image.
		private uint rvaExportTable;
		private uint sizeExportTable;
		private uint rvaImportTable;
        private uint rvaDelayImportDescriptor;
        private uint rvaExceptionTable;
        private uint sizeExceptionTable;
        private uint rvaBaseRelocationTable;
        private uint sizeBaseRelocationTable;

        private uint rvaResources;
        private Dictionary<Address, ImportReference> importReferences;
        private Relocator relocator;

		public PeImageLoader(IServiceProvider services, string filename, byte [] img, uint peOffset) : base(services, filename, img)
		{
			EndianImageReader rdr = new LeImageReader(RawImage, peOffset);
			if (rdr.ReadByte() != 'P' ||
				rdr.ReadByte() != 'E' ||
				rdr.ReadByte() != 0x0 ||
				rdr.ReadByte() != 0x0)
			{
				throw new BadImageFormatException("Not a valid PE header.");
			}
            importThunks = new Dictionary<uint, PseudoProcedure>();
            importReferences = new Dictionary<Address, ImportReference>();
            ImageSymbols = new SortedList<Address, ImageSymbol>();
			short expectedMagic = ReadCoffHeader(rdr);
			ReadOptionalHeader(rdr, expectedMagic);
		}

        public SortedList<Address, ImageSymbol> ImageSymbols { get; private set; }
        public SegmentMap SegmentMap { get; private set; }

		private void AddExportedEntryPoints(Address addrLoad, SegmentMap imageMap, List<ImageSymbol> entryPoints)
		{
			EndianImageReader rdr = imgLoaded.CreateLeReader(rvaExportTable);
			uint characteristics = rdr.ReadLeUInt32();
			uint timestamp = rdr.ReadLeUInt32();
			uint version = rdr.ReadLeUInt32();
			uint binaryNameAddr = rdr.ReadLeUInt32();
			uint baseOrdinal = rdr.ReadLeUInt32();

			int nExports = rdr.ReadLeInt32();
			int nNames = rdr.ReadLeInt32();
			uint rvaApfn = rdr.ReadLeUInt32();
			uint rvaNames = rdr.ReadLeUInt32();

			EndianImageReader rdrAddrs = imgLoaded.CreateLeReader(rvaApfn);
            EndianImageReader rdrNames = nNames != 0
                ? imgLoaded.CreateLeReader(rvaNames)
                : null;
			for (int i = 0; i < nExports; ++i)
			{
                ImageSymbol ep = LoadEntryPoint(addrLoad, rdrAddrs, rdrNames);
				if (imageMap.IsExecutableAddress(ep.Address))
				{
                    ImageSymbols[ep.Address] = ep;
					entryPoints.Add(ep);
				}
			}
		}

        private ImageSymbol LoadEntryPoint(Address addrLoad, EndianImageReader rdrAddrs, EndianImageReader rdrNames)
        {
            uint rvaAddr = rdrAddrs.ReadLeUInt32();
            string name = null;
            if (rdrNames != null)
            {
                int iNameMin = rdrNames.ReadLeInt32();
                int j;
                for (j = iNameMin; imgLoaded.Bytes[j] != 0; ++j)
                    ;
                name = Encoding.ASCII.GetString(imgLoaded.Bytes, iNameMin, j - iNameMin);
            }
            return ImageSymbol.Procedure(
                arch,
                addrLoad + rvaAddr,
                name,
                new FunctionType(),
                state: arch.CreateProcessorState());
        }

		public IProcessorArchitecture CreateArchitecture(ushort peMachineType)
		{
            string arch;
            var cfgSvc = Services.RequireService<IConfigurationService>();
			switch (peMachineType)
			{
            case MACHINE_ALPHA: arch = "alpha"; break;
            case MACHINE_ARM: arch = "arm"; break;
            case MACHINE_ARM64: arch = "arm-64"; break;
            case MACHINE_ARMNT: arch = "arm-thumb"; break;
            case MACHINE_I386: arch = "x86-protected-32"; break;
            case MACHINE_x86_64: arch = "x86-protected-64"; break;
			case MACHINE_m68k: arch = "m68k"; break;
            case MACHINE_R4000: arch = "mips-le-32"; break;
            case MACHINE_POWERPC: arch = "ppc-le-32"; break;
            case MACHINE_POWERPC_BE: arch = "ppc-be-32"; break;
            case MACHINE_XBOX360: arch = "ppc-be-64"; break;
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
            return cfgSvc.GetArchitecture(arch);
		}

        public IPlatform CreatePlatform(ushort peMachineType, IServiceProvider sp, IProcessorArchitecture arch)
        {
            string env;
            switch (peMachineType)
            {
            case MACHINE_ALPHA: env = "winAlpha"; break;
            case MACHINE_ARM: env = "winArm"; break;
            case MACHINE_ARM64: env = "winArm64"; break;
            case MACHINE_ARMNT: env= "winArm"; break;
            case MACHINE_I386: env = "win32"; break;
            case MACHINE_x86_64: env = "win64"; break;
	        case MACHINE_m68k: env = "winM68k"; break;
            case MACHINE_R4000: env = "winMips"; break;
            case MACHINE_POWERPC: env = "winPpc32"; break;
            case MACHINE_POWERPC_BE: env = "winPpc32"; break;   //$REVIEW: this probably should be macOS-ppc
            case MACHINE_XBOX360: env = "xbox360"; break;
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE hader.", peMachineType));
            }
            return Services.RequireService<IConfigurationService>()
                .GetEnvironment(env)
                .Load(Services, this.arch);
        }

        private SizeSpecificLoader CreateInnerLoader(ushort peMachineType)
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
                return new Pe32Loader(this);
            case MACHINE_x86_64:
            case MACHINE_ARM64:
                return new Pe64Loader(this);
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE header.", peMachineType));
            }
        }

        private Relocator CreateRelocator(ushort peMachineType, Program program)
        {
            switch (peMachineType)
            {
            case MACHINE_ALPHA: return new AlphaRelocator(Services, program);
            case MACHINE_ARM: return new ArmRelocator(program);
            case MACHINE_ARM64: return new Arm64Relocator(Services, program);
            case MACHINE_ARMNT: return new ArmRelocator(program);
            case MACHINE_I386: return new i386Relocator(Services, program);
            case MACHINE_R4000: return new MipsRelocator(Services, program);
            case MACHINE_x86_64: return new x86_64Relocator(Services, program);
			case MACHINE_m68k: return new M68kRelocator(Services, program);
            case MACHINE_POWERPC: return new PowerPcRelocator(Services, program);
            case MACHINE_POWERPC_BE: return new PowerPcRelocator(Services, program);    //$REVIEW do we need a big-endian version of this?
            case MACHINE_XBOX360: return new PowerPcRelocator(Services, program);       //$REVIEW do we need a big-endian version of this?

            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE hader.", peMachineType));
            }
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

        public override Program Load(Address addrLoad)
        {
            SegmentMap = new SegmentMap(addrLoad);
            if (sections > 0)
            {
                sectionList = LoadSections(addrLoad, rvaSectionTable, sections);
                imgLoaded = LoadSectionBytes(addrLoad, sectionList);
                AddSectionsToImageMap(addrLoad, SegmentMap);
            }
            this.program = new Program(SegmentMap, arch, platform);
            this.importReferences = program.ImportReferences;

            var rsrcLoader = new ResourceLoader(this.imgLoaded, rvaResources);
            List<ProgramResource> items = rsrcLoader.Load();
            program.Resources.Resources.AddRange(items);
            program.Resources.Name = "PE resources";
            return program;
        }

		public void LoadSectionBytes(Section s, byte [] rawImage, byte [] loadedImage)
		{
			Array.Copy(rawImage, s.OffsetRawData, loadedImage, s.VirtualAddress,
                s.SizeRawData);
		}

        public IEnumerable<Section> ReadSections(LeImageReader rdr, int sections)
        {
            for (int i = 0; i < sections; ++i)
            {
                yield return ReadSection(rdr);
            }
        }

		/// <summary>
		/// Loads the sections
		/// </summary>
		/// <param name="rvaSectionTable"></param>
		/// <returns></returns>
		private List<Section> LoadSections(Address addrLoad, uint rvaSectionTable, int sections)
        {
            var sectionMap = new List<Section>();
			EndianImageReader rdr = new LeImageReader(RawImage, rvaSectionTable);

            Section minSection = null;
			for (int i = 0; i != sections; ++i)
			{
				Section section = ReadSection(rdr);
				sectionMap.Add(section);
				if (minSection == null || section.VirtualAddress < minSection.VirtualAddress)
					minSection = section;
                Debug.Print("  Section: {0,10} {1:X8} {2:X8} {3:X8} {4:X8}", section.Name, section.OffsetRawData, section.SizeRawData, section.VirtualAddress, section.VirtualSize);
			}

            // Map the area between addrLoad and the lowest section.
            if (minSection != null && 0 < minSection.VirtualAddress)
            {
                sectionMap.Insert(0, new Section
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

        public MemoryArea LoadSectionBytes(Address addrLoad, List<Section> sections)
        {
            var vaMax = sections.Max(s => s.VirtualAddress);
            var sectionMax = sections.Where(s => s.VirtualAddress == vaMax).First();
            var imgLoaded = new MemoryArea(addrLoad, new byte[sectionMax.VirtualAddress + Math.Max(sectionMax.VirtualSize, sectionMax.SizeRawData)]);
            foreach (Section s in sectionList)
            {
                Array.Copy(RawImage, s.OffsetRawData, imgLoaded.Bytes, s.VirtualAddress, s.SizeRawData);
            }
            return imgLoaded;
        }

		public override Address PreferredBaseAddress
		{
			get { return this.preferredBaseOfImage; }
            set { throw new NotImplementedException(); }
        }

		public short ReadCoffHeader(EndianImageReader rdr)
		{
			this.machine = rdr.ReadLeUInt16();
            short expectedMagic = GetExpectedMagic(machine);
            arch = CreateArchitecture(machine);
			platform = CreatePlatform(machine, Services, arch);
            innerLoader = CreateInnerLoader(machine);

			sections = rdr.ReadLeInt16();
			rdr.ReadLeUInt32();		// timestamp.
			rdr.ReadLeUInt32();		// COFF symbol table.
			rdr.ReadLeUInt32();		// #of symbols.
			optionalHeaderSize = rdr.ReadLeInt16();
			this.fileFlags = rdr.ReadLeUInt16();
			rvaSectionTable = (uint) ((int)rdr.Offset + optionalHeaderSize);
            return expectedMagic;
		}

		public void ReadOptionalHeader(EndianImageReader rdr, short expectedMagic)
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
			rvaStartAddress = rdr.ReadLeUInt32();
			uint rvaBaseOfCode = rdr.ReadLeUInt32();
            preferredBaseOfImage = innerLoader.ReadPreferredImageBase(rdr);
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
			uint dictionaryCount = rdr.ReadLeUInt32();

            if (dictionaryCount == 0) return;
            this.rvaExportTable = rdr.ReadLeUInt32();
            this.sizeExportTable = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            this.rvaImportTable = rdr.ReadLeUInt32();
			uint importTableSize = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            this.rvaResources = rdr.ReadLeUInt32();			// resource address
			rdr.ReadLeUInt32();			// resource size

            if (--dictionaryCount == 0) return;
			this.rvaExceptionTable = rdr.ReadLeUInt32();            // exception address
            this.sizeExceptionTable = rdr.ReadLeUInt32();			// exception size

            if (--dictionaryCount == 0) return;
			rdr.ReadLeUInt32();			// certificate address
			rdr.ReadLeUInt32();			// certificate size

            if (--dictionaryCount == 0) return;
            this.rvaBaseRelocationTable = rdr.ReadLeUInt32();
            this.sizeBaseRelocationTable = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaDebug = rdr.ReadLeUInt32();
            uint cbDebug = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaArchitecture = rdr.ReadLeUInt32();
            uint cbArchitecture = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaGlobalPointer = rdr.ReadLeUInt32();
            uint cbGlobalPointer = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaTls = rdr.ReadLeUInt32();
            uint cbTls = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaLoadConfig = rdr.ReadLeUInt32();
            uint cbLoadConfig = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaBoundImport = rdr.ReadLeUInt32();
            uint cbBoundImport = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            uint rvaIat = rdr.ReadLeUInt32();
            uint cbIat = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
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

        public override RelocationResults Relocate(Program program, Address addrLoad)
		{
            relocator = CreateRelocator(machine, program);
            var relocations = imgLoaded.Relocations;
			Section relocSection;
            if (rvaBaseRelocationTable != 0 &&
                (relocSection = sectionList.Find(section => 
                    this.rvaBaseRelocationTable >= section.VirtualAddress &&
                    this.rvaBaseRelocationTable < section.VirtualAddress + section.VirtualSize)) != null)
			{
                ApplyRelocations(relocSection.OffsetRawData, relocSection.SizeRawData, addrLoad, relocations);
			} 
            var addrEp = platform.AdjustProcedureAddress(addrLoad + rvaStartAddress);
            var entrySym = CreateMainEntryPoint(
                    (this.fileFlags & ImageFileDll) != 0,
                    addrEp,
                    platform);
            ImageSymbols[entrySym.Address] = entrySym;
            var entryPoints = new List<ImageSymbol> { entrySym };
            ReadExceptionRecords(addrLoad, rvaExceptionTable, sizeExceptionTable, ImageSymbols);
            if (rvaExportTable != 0)
            {
                AddExportedEntryPoints(addrLoad, SegmentMap, entryPoints);
            }
            ReadImportDescriptors(addrLoad);
            ReadDeferredLoadDescriptors(addrLoad);
            return new RelocationResults(entryPoints, ImageSymbols);
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
            if (s != null)
                return s;

            string name = null;
            SerializedSignature ssig = null;

            Argument_v1 Arg(string n, string t) => new Argument_v1
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

        public void AddSectionsToImageMap(Address addrLoad, SegmentMap imageMap)
        {
            foreach (Section s in sectionList)
            {
                AccessMode acc = AccessMode.Read;
                if ((s.Flags & SectionFlagsWriteable) != 0)
                {
                    acc |= AccessMode.Write;
                }
                if ((s.Flags & SectionFlagsExecutable) != 0)
                {
                    acc |= AccessMode.Execute;
                }
                var seg = SegmentMap.AddSegment(new ImageSegment(
                    s.Name,
                    addrLoad + s.VirtualAddress,
                    imgLoaded,
                    acc)
                {
                    Size = s.VirtualSize,
                    IsHidden = s.IsHidden,
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
            DebugEx.Inform(trace, "PELdr: applying relocations {0:X8}", rvaReloc);
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
				while (rdr.Offset < offBlockEnd)
				{
					relocator.ApplyRelocation(baseOfImage, page, rdr, relocations);
				}
			}
		}

		public string ReadUtf8String(uint rva, int maxLength)
		{
            if (rva == 0)
                return null;
			EndianImageReader rdr = imgLoaded.CreateLeReader(rva);
			List<byte> bytes = new List<byte>();
			byte b;
			while ((b = rdr.ReadByte()) != 0)
			{
				bytes.Add(b);
				if (bytes.Count == maxLength)
					break;
			}
			return Encoding.UTF8.GetString(bytes.ToArray());
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
            var dllName = ReadUtf8String(rdr.ReadLeUInt32(), 0);		// DLL name
            var rvaIAT = rdr.ReadLeUInt32();		    // Import address table 
            if (rvaILT == 0 && dllName == null)
                return false;

            EndianImageReader rdrIlt = imgLoaded.CreateLeReader(rvaILT!=0 ? rvaILT:rvaIAT);
            EndianImageReader rdrIat = imgLoaded.CreateLeReader(rvaIAT);
            while (true)
            {
                var addrIat = rdrIat.Address;
                var addrIlt = rdrIlt.Address;

                var (impRef, bitSize) = innerLoader.ResolveImportDescriptorEntry(dllName, rdrIlt, rdrIat);
                if (impRef == null)
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

        private abstract class SizeSpecificLoader
        {
            protected PeImageLoader outer;

            public SizeSpecificLoader(PeImageLoader outer)
            {
                this.outer = outer;
            }

            public abstract (ImportReference, int) ResolveImportDescriptorEntry(string dllName, EndianImageReader rdrIlt, EndianImageReader rdrIat);

            public abstract bool ImportedFunctionNameSpecified(ulong rvaEntry);

            public ImportReference CreateImportReference(string dllName, ulong rvaEntry, Address addrThunk)
            {
                if (!ImportedFunctionNameSpecified(rvaEntry))
                {
                    return new OrdinalImportReference(
                        addrThunk, dllName, (int)rvaEntry & 0xFFFF, SymbolType.ExternalProcedure);
                }
                else
                {
                    string fnName = outer.ReadUtf8String((uint)rvaEntry + 2, 0);
                    return new NamedImportReference(
                        addrThunk, dllName, fnName, SymbolType.ExternalProcedure);
                }
            }

            public abstract Address ReadPreferredImageBase(EndianImageReader rdr);
            public abstract long ReadWord(EndianImageReader rdr);
        }

        private class Pe32Loader : SizeSpecificLoader
        {
            public Pe32Loader(PeImageLoader outer) : base(outer) {}

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x80000000) == 0;
            }

            public override Address ReadPreferredImageBase(EndianImageReader rdr)
            {
                uint rvaBaseOfData = rdr.ReadLeUInt32();        // Only exists in PE32, not PE32+
                return Address.Ptr32(rdr.ReadLeUInt32());
            }

            public override long ReadWord(EndianImageReader rdr)
            {
                return rdr.ReadInt32();
            }

            public override (ImportReference, int) ResolveImportDescriptorEntry(string dllName, EndianImageReader rdrIlt, EndianImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                uint iatEntry = rdrIat.ReadLeUInt32();
                uint iltEntry = rdrIlt.ReadLeUInt32();
                if (iltEntry == 0)
                    return (null, 0);

                var impRef = CreateImportReference(dllName, iltEntry, addrThunk);
                outer.importReferences.Add(addrThunk, impRef);

                return (impRef, 32);
            }
        }

        private class Pe64Loader : SizeSpecificLoader
        {
            public Pe64Loader(PeImageLoader outer) : base(outer) {}

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x8000000000000000u) == 0;
            }

            public override Address ReadPreferredImageBase(EndianImageReader rdr)
            {
                return Address64.Ptr64(rdr.ReadLeUInt64());
            }

            public override long ReadWord(EndianImageReader rdr)
            {
                return rdr.ReadInt64();
            }

            public override (ImportReference, int) ResolveImportDescriptorEntry(string dllName, EndianImageReader rdrIlt, EndianImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                ulong iatEntry = rdrIat.ReadLeUInt64();
                ulong iltEntry = rdrIlt.ReadLeUInt64();
                if (iltEntry == 0)
                    return (null, 0);
                var impRef = CreateImportReference(dllName, iltEntry, addrThunk);
                outer.importReferences.Add(addrThunk, impRef);
                return (impRef, 64);
            }
        }

        private bool ReadDeferredLoadDescriptors(EndianImageReader rdr, Address addrLoad)
        {
            var symbols = new List<ImageSymbol>();
            var attributes = rdr.ReadLeUInt32();
            var offset = ((attributes & DID_RvaBased) != 0) ? 0 : (uint) addrLoad.ToLinear();
            var rvaDllName = rdr.ReadLeUInt32();
            if (rvaDllName == 0)
                return false;
            var dllName = ReadUtf8String(rvaDllName - offset, 0);    // DLL name.
            if (dllName == null)
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
                    innerLoader.CreateImportReference(dllName, rvaName, addrThunk);

                importReferences.Add(addrThunk, impRef);
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

		private static Section ReadSection(EndianImageReader rdr)
		{
			Section sec = new Section();
			sec.Name = ReadSectionName(rdr);
			sec.VirtualSize = rdr.ReadLeUInt32();
			sec.VirtualAddress = rdr.ReadLeUInt32();

			if (sec.Name == null) {
				sec.Name = ".reko_" + sec.VirtualAddress.ToString("x16");
			}

			sec.SizeRawData = rdr.ReadLeUInt32();
			sec.OffsetRawData = rdr.ReadLeUInt32();
			rdr.ReadLeUInt32();			// pointer to relocations
			rdr.ReadLeUInt32();			// pointer to line numbers.
			rdr.ReadLeInt16();		// # of relocations
			rdr.ReadLeInt16();		// # of line numbers
			sec.Flags = rdr.ReadLeUInt32();
			return sec;
		}

		private static string ReadSectionName(EndianImageReader rdr)
		{
			byte [] bytes = new Byte[8];
			for (int b = 0; b < bytes.Length; ++b)
			{
				bytes[b] = rdr.ReadByte();
			}

			Encoding asc = Encoding.ASCII;
			char [] chars = asc.GetChars(bytes);
			int i;
			for (i = chars.Length - 1; i >= 0; --i)
			{
				if (chars[i] != 0)
				{
					++i;
					break;
				}
			}
			if(i < 0) {
				return null;
			}
			return new String(chars, 0, i);
		}

		private const uint SectionFlagsInitialized = 0x00000040;
		private const uint SectionFlagsDiscardable = 0x02000000;
		private const uint SectionFlagsWriteable =   0x80000000;
		private const uint SectionFlagsExecutable =  0x00000020;

        public class Section
		{
			public string Name;
			public uint VirtualSize;
			public uint VirtualAddress;
			public uint SizeRawData;
			public uint Flags;
			public uint OffsetRawData;
            public bool IsHidden;

			public bool IsDiscardable
			{
				get { return (Flags & SectionFlagsDiscardable) != 0; }
			}

		}

        public uint ReadEntryPointRva()
        {
            EndianImageReader rdr = new LeImageReader(RawImage, rvaSectionTable);
            for (int i = 0; i < sections; ++i)
            {
                var s = ReadSection(rdr);
                if (s.VirtualAddress <= rvaStartAddress && rvaStartAddress < s.VirtualAddress + s.VirtualSize)
                {
                    return (rvaStartAddress - s.VirtualAddress) + s.OffsetRawData;
                }
            }
            return 0;
        }

        public void ReadExceptionRecords(
            Address addrLoad,
            uint rvaExceptionTable, 
            uint sizeExceptionTable,
            SortedList<Address, ImageSymbol> symbols)
        {
            var rvaTableEnd = rvaExceptionTable + sizeExceptionTable; 
            var functionStarts = new List<Address>();
            if (rvaExceptionTable == 0 || sizeExceptionTable == 0)
                return;
            var rdr = new LeImageReader(this.imgLoaded.Bytes, rvaExceptionTable);
            switch (machine)
            {
            default: 
                Services.RequireService<IDiagnosticsService>()
                    .Warn(new NullCodeLocation(Filename), "Exception table reading not supported for machine #{0}.", machine);
                break;
            case MACHINE_x86_64:
                while (rdr.Offset < rvaTableEnd)
                {
                    var addr = addrLoad + rdr.ReadLeUInt32();
                    rdr.Seek(8);
                    AddFunctionSymbol(addr, symbols);
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
                if (symOld.Name == null && symNew.Name != null)
                    symbols[addr] = symNew;
            }
        }
    }
}
