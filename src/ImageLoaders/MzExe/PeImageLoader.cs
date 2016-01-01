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

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Loads Windows NT PE images.
    /// </summary>
	public class PeImageLoader : ImageLoader
	{
        private const ushort MACHINE_i386 = (ushort)0x014C;
        private const ushort MACHINE_x86_64 = unchecked((ushort)0x8664);
        private const ushort MACHINE_ARMNT = (ushort)0x01C4;
        private const ushort MACHINE_R4000 = (ushort)0x0166;
        private const short ImageFileRelocationsStripped = 0x0001;
        private const short ImageFileExecutable = 0x0002;
        private const short ImageFileDll = 0x2000;

        private IProcessorArchitecture arch;
        private Win32Platform platform;
        private SizeSpecificLoader innerLoader;
        private Program program;

        private ushort machine;
		private short optionalHeaderSize;
        private ushort fileFlags;
		private int sections;
        private uint rvaSectionTable;
		private LoadedImage imgLoaded;
		private Address preferredBaseOfImage;
		private SortedDictionary<string, Section> sectionMap;
        private Dictionary<uint, PseudoProcedure> importThunks;
		private uint rvaStartAddress;		// unrelocated start address of the image.
		private uint rvaExportTable;
		private uint sizeExportTable;
		private uint rvaImportTable;
        private uint rvaDelayImportDescriptor;
        private uint rvaExceptionTable;
        private uint sizeExceptionTable;
        private uint rvaResources;
        private Dictionary<Address, ImportReference> importReferences;

		public PeImageLoader(IServiceProvider services, string filename, byte [] img, uint peOffset) : base(services, filename, img)
		{
			ImageReader rdr = new LeImageReader(RawImage, peOffset);
			if (rdr.ReadByte() != 'P' ||
				rdr.ReadByte() != 'E' ||
				rdr.ReadByte() != 0x0 ||
				rdr.ReadByte() != 0x0)
			{
				throw new BadImageFormatException("Not a valid PE header.");
			}
            importThunks = new Dictionary<uint, PseudoProcedure>();
            importReferences = new Dictionary<Address, ImportReference>();
			short expectedMagic = ReadCoffHeader(rdr);
			ReadOptionalHeader(rdr, expectedMagic);
		}

        public ImageMap ImageMap { get; private set; }

		private void AddExportedEntryPoints(Address addrLoad, ImageMap imageMap, List<EntryPoint> entryPoints)
		{
			ImageReader rdr = imgLoaded.CreateLeReader(rvaExportTable);
			rdr.ReadLeUInt32();	// Characteristics
			rdr.ReadLeUInt32(); // timestamp
			rdr.ReadLeUInt32();	// version.
			rdr.ReadLeUInt32();	// binary name.
			rdr.ReadLeUInt32();	// base ordinal
			int nExports = rdr.ReadLeInt32();
			int nNames = rdr.ReadLeInt32();
			if (nExports != nNames)
				throw new BadImageFormatException("Unexpected discrepancy in PE image.");
			uint rvaApfn = rdr.ReadLeUInt32();
			uint rvaNames = rdr.ReadLeUInt32();

			ImageReader rdrAddrs = imgLoaded.CreateLeReader(rvaApfn);
			ImageReader rdrNames = imgLoaded.CreateLeReader(rvaNames);
			for (int i = 0; i < nNames; ++i)
			{
                EntryPoint ep = LoadEntryPoint(addrLoad, rdrAddrs, rdrNames);
				if (imageMap.IsExecutableAddress(ep.Address))
				{
					entryPoints.Add(ep);
				}
			}
		}

        private EntryPoint LoadEntryPoint(Address addrLoad, ImageReader rdrAddrs, ImageReader rdrNames)
        {
            uint addr = rdrAddrs.ReadLeUInt32();
            int iNameMin = rdrNames.ReadLeInt32();
            int j;
            for (j = iNameMin; imgLoaded.Bytes[j] != 0; ++j)
                ;
            char[] chars = Encoding.ASCII.GetChars(imgLoaded.Bytes, iNameMin, j - iNameMin);
            return new EntryPoint(addrLoad + addr, new string(chars), arch.CreateProcessorState());
        }

		public IProcessorArchitecture CreateArchitecture(ushort peMachineType)
		{
            string arch;
            var cfgSvc = Services.RequireService<IConfigurationService>();
			switch (peMachineType)
			{
            case MACHINE_ARMNT: arch = "arm-thumb"; break;
            case MACHINE_i386: arch = "x86-protected-32"; break;
            case MACHINE_x86_64: arch = "x86-protected-64"; break;
            case MACHINE_R4000: arch = "mips-le-32"; break;
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
            return cfgSvc.GetArchitecture(arch);
		}

        public Win32Platform CreatePlatform(ushort peMachineType, IServiceProvider sp, IProcessorArchitecture arch)
        {
            string env;
            switch (peMachineType)
            {
            case MACHINE_ARMNT: env= "winArm"; break;
            case MACHINE_i386: env = "win32"; break;
            case MACHINE_x86_64: env = "win64"; break;
            case MACHINE_R4000: env = "winMips"; break;
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE hader.", peMachineType));
            }
            return (Win32Platform) Services.RequireService<IConfigurationService>()
                .GetEnvironment(env)
                .Load(Services, this.arch);
        }

        private SizeSpecificLoader CreateInnerLoader(ushort peMachineType)
        {
            switch (peMachineType)
            {
            case MACHINE_ARMNT:
            case MACHINE_i386: 
            case MACHINE_R4000:
                return new Pe32Loader(this);
            case MACHINE_x86_64:
                return new Pe64Loader(this);
            default: throw new ArgumentException(string.Format("Unsupported machine type 0x:{0:X4} in PE hader.", peMachineType));
            }
        }

        private short GetExpectedMagic(ushort peMachineType)
        {
            switch (peMachineType)
            {
            case MACHINE_ARMNT:
            case MACHINE_i386:
            case MACHINE_R4000: 
                return 0x010B;
            case MACHINE_x86_64:
                return 0x020B;
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
        }

        public override Program Load(Address addrLoad)
        {
            if (sections > 0)
            {
                sectionMap = LoadSections(addrLoad, rvaSectionTable, sections);
                imgLoaded = LoadSectionBytes(addrLoad, sectionMap);
                ImageMap = imgLoaded.CreateImageMap();
            }
            imgLoaded.BaseAddress = addrLoad;
            this.program = new Program(imgLoaded, ImageMap, arch, platform);
            this.importReferences = program.ImportReferences;

            var rsrcLoader = new PeResourceLoader(this.imgLoaded, rvaResources);
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
		private SortedDictionary<string, Section> LoadSections(Address addrLoad, uint rvaSectionTable, int sections)
        {
            var sectionMap = new SortedDictionary<string, Section>();
			ImageReader rdr = new LeImageReader(RawImage, rvaSectionTable);
			var section = ReadSection(rdr);
			var sectionMax = section;
			sectionMap[section.Name] = section;
			
			for (int i = 1; i != sections; ++i)
			{
				section = ReadSection(rdr);
				sectionMap[section.Name] = section;
				if (section.VirtualAddress > sectionMax.VirtualAddress)
					sectionMax = section;
                Debug.Print("  Section: {0,10} {1:X8} {2:X8} {3:X8} {4:X8}", section.Name, section.OffsetRawData, section.SizeRawData, section.VirtualAddress, section.VirtualSize);
			}
            return sectionMap;
		}

        public LoadedImage LoadSectionBytes(Address addrLoad, SortedDictionary<string, Section> sections)
        {
            var vaMax = sections.Values.Max(s => s.VirtualAddress);
            var sectionMax = sections.Values.Where(s => s.VirtualAddress == vaMax).First();
            var imgLoaded = new LoadedImage(addrLoad, new byte[sectionMax.VirtualAddress + Math.Max(sectionMax.VirtualSize, sectionMax.SizeRawData)]);
            foreach (Section s in sectionMap.Values)
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

		public short ReadCoffHeader(ImageReader rdr)
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

		public void ReadOptionalHeader(ImageReader rdr, short expectedMagic)
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
            var stackReserve = rdr.Read(arch.WordWidth);
            var stackCommit = rdr.Read(arch.WordWidth);
            var heapReserve = rdr.Read(arch.WordWidth);
            var heapCommit = rdr.Read(arch.WordWidth);
			rdr.ReadLeUInt32();			// loader flags
			uint dictionaryCount = rdr.ReadLeUInt32();

            if (dictionaryCount == 0) return;
			rvaExportTable = rdr.ReadLeUInt32();
			sizeExportTable = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
            rvaImportTable = rdr.ReadLeUInt32();
			uint importTableSize = rdr.ReadLeUInt32();

            if (--dictionaryCount == 0) return;
			rvaResources = rdr.ReadLeUInt32();			// resource address
			rdr.ReadLeUInt32();			// resource size

            if (--dictionaryCount == 0) return;
			rvaExceptionTable = rdr.ReadLeUInt32();			// exception address
			sizeExceptionTable = rdr.ReadLeUInt32();			// exception size

            if (--dictionaryCount == 0) return;
			rdr.ReadLeUInt32();			// certificate address
			rdr.ReadLeUInt32();			// certificate size

            if (--dictionaryCount == 0) return;
            uint rvaBaseRelocAddress = rdr.ReadLeUInt32();
			uint baseRelocSize = rdr.ReadLeUInt32();

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

		private const ushort RelocationAbsolute = 0;
		private const ushort RelocationHigh = 1;
		private const ushort RelocationLow = 2;
		private const ushort RelocationHighLow = 3;


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
            AddSectionsToImageMap(addrLoad, ImageMap);
            var relocations = imgLoaded.Relocations;
			Section relocSection;
            if (sectionMap.TryGetValue(".reloc", out relocSection))
			{
				ApplyRelocations(relocSection.OffsetRawData, relocSection.SizeRawData, (uint) addrLoad.ToLinear(), relocations);
			}
            var addrEp = platform.AdjustProcedureAddress(addrLoad + rvaStartAddress);
            var entryPoints = new List<EntryPoint> { new EntryPoint(addrEp, arch.CreateProcessorState()) };
            var functions = ReadExceptionRecords(addrLoad, rvaExceptionTable, sizeExceptionTable);
            AddExportedEntryPoints(addrLoad, ImageMap, entryPoints);
			ReadImportDescriptors(addrLoad);
            ReadDeferredLoadDescriptors(addrLoad);
            return new RelocationResults(entryPoints, relocations, functions);
		}

        private void AddSectionsToImageMap(Address addrLoad, ImageMap imageMap)
        {
            foreach (Section s in sectionMap.Values)
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
                var seg = imageMap.AddSegment(addrLoad + s.VirtualAddress, s.Name, acc, s.VirtualSize);
                seg.IsDiscardable = s.IsDiscardable;
            }
        }

		public void ApplyRelocation(uint baseOfImage, uint page, ImageReader rdr, RelocationDictionary relocations)
		{
			ushort fixup = rdr.ReadLeUInt16();
			uint offset = page + (fixup & 0x0FFFu);
			switch (fixup >> 12)
			{
			case RelocationAbsolute:
				// Used for padding to 4-byte boundary, ignore.
				break;
			case RelocationHighLow:
			{
				uint n = (uint) (imgLoaded.ReadLeUInt32(offset) + (baseOfImage - preferredBaseOfImage.ToLinear()));
				imgLoaded.WriteLeUInt32(offset, n);
				relocations.AddPointerReference(offset, n);
				break;
			}
            case 0xA:
            break;
			default:
                var dcSvc = Services.RequireService<DecompilerEventListener>();
                dcSvc.Warn(
                    dcSvc.CreateAddressNavigator(program, Address.Ptr32(offset)),
                    string.Format(
                        "Unsupported PE fixup type: {0:X}",
                        fixup >> 12));
                break;
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

#if MIPS
public static final  short 	IMAGE_REL_MIPS_ABSOLUTE    	This relocation is ignored. 
public static final  short 	IMAGE_REL_MIPS_REFHALF    	The high 16 bits of the target's 32-bit virtual address. 
public static final  short 	IMAGE_REL_MIPS_REFWORD    	The target's 32-bit virtual address. 
public static final  short 	IMAGE_REL_MIPS_JMPADDR    	The low 26 bits of the target's virtual address. This supports the MIPS J and JAL instructions. 
public static final  short 	IMAGE_REL_MIPS_REFHI    	The high 16 bits of the target's 32-bit virtual address. Used for the first instruction in a two-instruction sequence that loads a full address. This relocation must be immediately followed by a PAIR relocations whose SymbolTableIndex contains a signed 16-bit displacement which is added to the upper 16 bits taken from the location being relocated. 
public static final  short 	IMAGE_REL_MIPS_REFLO    	The low 16 bits of the target's virtual address. 
public static final  short 	IMAGE_REL_MIPS_GPREL    	16-bit signed displacement of the target relative to the Global Pointer (GP) register. 
public static final  short 	IMAGE_REL_MIPS_LITERAL    	Same as IMAGE_REL_MIPS_GPREL. 
public static final  short 	IMAGE_REL_MIPS_SECTION    	The 16-bit section index of the section containing the target. This is used to support debugging information. 
public static final  short 	IMAGE_REL_MIPS_SECREL    	The 32-bit offset of the target from the beginning of its section. This is used to support debugging information as well as static thread local storage. 
public static final  short 	IMAGE_REL_MIPS_SECRELLO    	The low 16 bits of the 32-bit offset of the target from the beginning of its section. 
public static final  short 	IMAGE_REL_MIPS_SECRELHI    	The high 16 bits of the 32-bit offset of the target from the beginning of its section. A PAIR relocation must immediately follow this on. The SymbolTableIndex of the PAIR relocation contains a signed 16-bit displacement, which is added to the upper 16 bits taken from the location being relocated. 
public static final  short 	IMAGE_REL_MIPS_JMPADDR16    The low 26 bits of the target's virtual address. This supports the MIPS16 JAL instruction. 
public static final  short 	IMAGE_REL_MIPS_REFWORDNB    The target's 32-bit relative virtual address. 
public static final  short 	IMAGE_REL_MIPS_PAIR    	    This relocation is only valid when it immediately follows a REFHI or SECRELHI relocation. Its SymbolTableIndex contains a displacement and not an index into the symbol table. 
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


        }

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
        public void ApplyRelocations(uint rvaReloc, uint size, uint baseOfImage, RelocationDictionary relocations)
		{
			ImageReader rdr = new LeImageReader(RawImage, rvaReloc);
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
					ApplyRelocation(baseOfImage, page, rdr, relocations);
				}
			}
		}

		public string ReadUtf8String(uint rva, int maxLength)
		{
            if (rva == 0)
                return null;
			ImageReader rdr = imgLoaded.CreateLeReader(rva);
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
        public bool ReadImportDescriptor(ImageReader rdr, Address addrLoad)
        {
            var rvaILT = rdr.ReadLeUInt32();            // Import lookup table
            rdr.ReadLeUInt32();		                    // Ignore datestamp...
            rdr.ReadLeUInt32();		                    // ...and forwarder chain
            var dllName = ReadUtf8String(rdr.ReadLeUInt32(), 0);		// DLL name
            var rvaIAT = rdr.ReadLeUInt32();		    // Import address table 
            if (rvaILT == 0 && dllName == null)
                return false;

            var ptrSize = platform.PointerType.Size;
            ImageReader rdrIlt = imgLoaded.CreateLeReader(rvaILT!=0 ? rvaILT:rvaIAT);
            ImageReader rdrIat = imgLoaded.CreateLeReader(rvaIAT);
            while (true)
            {
                var addrIat = rdrIat.Address;
                var addrIlt = rdrIlt.Address;
                if (!innerLoader.ResolveImportDescriptorEntry(dllName, rdrIlt, rdrIat))
                    break;

                ImageMap.AddItemWithSize(
                    addrIat,
                    new ImageMapItem
                    {
                        Address = addrIat,
                        DataType = new Pointer(new CodeType(), ptrSize),
                        Size = (uint)ptrSize,
                    });
                ImageMap.AddItemWithSize(
                    addrIlt,
                    new ImageMapItem
                    {
                        Address = addrIlt,
                        DataType = PrimitiveType.CreateWord(ptrSize),
                        Size = (uint)ptrSize,
                    });
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

            public abstract bool ResolveImportDescriptorEntry(string dllName, ImageReader rdrIlt, ImageReader rdrIat);

            public abstract bool ImportedFunctionNameSpecified(ulong rvaEntry);

            public ImportReference ResolveImportedFunction(string dllName, ulong rvaEntry, Address addrThunk)
            {
                if (!ImportedFunctionNameSpecified(rvaEntry))
                {
                    return new OrdinalImportReference(
                        addrThunk, dllName, (int)rvaEntry & 0xFFFF);
                }
                else
                {
                    string fnName = outer.ReadUtf8String((uint)rvaEntry + 2, 0);
                    return new NamedImportReference(
                        addrThunk, dllName, fnName);
                }
            }

            public abstract Address ReadPreferredImageBase(ImageReader rdr);
        }

        private class Pe32Loader : SizeSpecificLoader
        {
            public Pe32Loader(PeImageLoader outer) : base(outer) {}

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x80000000) == 0;
            }

            public override Address ReadPreferredImageBase(ImageReader rdr)
            {
                {
                    uint rvaBaseOfData = rdr.ReadLeUInt32();        // Only exists in PE32, not PE32+
                    return Address32.Ptr32(rdr.ReadLeUInt32());
                }
            }

            public override bool ResolveImportDescriptorEntry(string dllName, ImageReader rdrIlt, ImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                uint iatEntry = rdrIat.ReadLeUInt32();
                uint iltEntry = rdrIlt.ReadLeUInt32();
                if (iltEntry == 0)
                    return false;

                outer.importReferences.Add(
                    addrThunk,
                    ResolveImportedFunction(dllName, iltEntry, addrThunk));
                return true;
            }
        }

        private class Pe64Loader : SizeSpecificLoader
        {
            public Pe64Loader(PeImageLoader outer) : base(outer) {}

            public override bool ImportedFunctionNameSpecified(ulong rvaEntry)
            {
                return (rvaEntry & 0x8000000000000000u) == 0;
            }

            public override Address ReadPreferredImageBase(ImageReader rdr)
            {
                return Address64.Ptr64(rdr.ReadLeUInt64());
            }

            public override bool ResolveImportDescriptorEntry(string dllName, ImageReader rdrIlt, ImageReader rdrIat)
            {
                Address addrThunk = rdrIat.Address;
                ulong iatEntry = rdrIat.ReadLeUInt64();
                ulong iltEntry = rdrIlt.ReadLeUInt64();
                if (iltEntry == 0)
                    return false;
                outer.importReferences.Add(
                    addrThunk,
                    ResolveImportedFunction(dllName, iltEntry, addrThunk));
                Debug.Print("{0}: {1}", addrThunk, outer.importReferences[addrThunk]);
                return true;
            }
        }

        private bool ReadDeferredLoadDescriptors(ImageReader rdr, Address addrLoad)
        {
            var attributes = rdr.ReadLeUInt32();
            var dllName = ReadUtf8String(rdr.ReadLeUInt32(), 0);    // DLL name.
            if (dllName == null)
                return false;
            var rdrModule = rdr.ReadLeInt32();
            var rdrThunks = imgLoaded.CreateLeReader(rdr.ReadLeUInt32());
            var rdrNames = imgLoaded.CreateLeReader(rdr.ReadLeUInt32());
            for (;;)
            {
                var addrThunk = imgLoaded.BaseAddress + rdrThunks.Offset;
                uint rvaName = rdrNames.ReadLeUInt32();
                uint rvaThunk = rdrThunks.ReadLeUInt32();
                if (rvaName == 0)
                    break;
                importReferences.Add(
                    addrThunk, 
                    innerLoader.ResolveImportedFunction(dllName, rvaName, addrThunk));
            }
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();  // time stamp
            return true;
        }

		private void ReadImportDescriptors(Address addrLoad)
		{
			ImageReader rdr = imgLoaded.CreateLeReader(rvaImportTable);
			while (ReadImportDescriptor(rdr, addrLoad))
			{
			}
		}

        private void ReadDeferredLoadDescriptors(Address addrLoad)
        {
            var rdr = imgLoaded.CreateLeReader(rvaDelayImportDescriptor);
            while (ReadDeferredLoadDescriptors(rdr, addrLoad))
            {
            }
        }

		private static Section ReadSection(ImageReader rdr)
		{
			Section sec = new Section();
			sec.Name = ReadSectionName(rdr);
			sec.VirtualSize = rdr.ReadLeUInt32();
			sec.VirtualAddress = rdr.ReadLeUInt32();
			sec.SizeRawData = rdr.ReadLeUInt32();
			sec.OffsetRawData = rdr.ReadLeUInt32();
			rdr.ReadLeUInt32();			// pointer to relocations
			rdr.ReadLeUInt32();			// pointer to line numbers.
			rdr.ReadLeInt16();		// # of relocations
			rdr.ReadLeInt16();		// # of line numbers
			sec.Flags = rdr.ReadLeUInt32();
			return sec;
		}

		private static string ReadSectionName(ImageReader rdr)
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

			public bool IsDiscardable
			{
				get { return (Flags & SectionFlagsDiscardable) != 0; }
			}
		}

        public uint ReadEntryPointRva()
        {
            ImageReader rdr = new LeImageReader(RawImage, rvaSectionTable);
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

        public List<Address> ReadExceptionRecords(Address addrLoad, uint rvaExceptionTable, uint sizeExceptionTable)
        {
            var rvaTableEnd = rvaExceptionTable + sizeExceptionTable; 
            var functionStarts = new List<Address>();
            if (rvaExceptionTable == 0 || sizeExceptionTable == 0)
                return functionStarts;
            switch (machine)
            {
            default: 
                Services.RequireService<IDiagnosticsService>()
                    .Warn(new NullCodeLocation(Filename), "Exception table reading not supported for machine #{0}.", machine);
                break;
            case MACHINE_R4000:
                var rdr = new LeImageReader(this.imgLoaded.Bytes, rvaExceptionTable);
                while (rdr.Offset < rvaTableEnd)
                {
                    var addr = Address.Ptr32(rdr.ReadLeUInt32());
                    rdr.Seek(16);
                    functionStarts.Add(addr);
                }
                break;
            }
            return functionStarts;
        }
    }
}
