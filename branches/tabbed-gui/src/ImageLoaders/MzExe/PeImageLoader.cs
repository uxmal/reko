#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Environments.Win32;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.ImageLoaders.MzExe
{
    /// <summary>
    /// Loads Windows NT PE images.
    /// </summary>
	public class PeImageLoader : ImageLoader
	{
        private IProcessorArchitecture arch;
        private Platform platform;

		private short optionalHeaderSize;
		private int sections;
		private uint sectionOffset;
		private LoadedImage imgLoaded;
        private ImageMap imageMap;
		private uint preferredBaseOfImage;
		private SortedDictionary<string, Section> sectionMap;
        private Dictionary<uint, PseudoProcedure> importThunks;
		private uint rvaStartAddress;		// unrelocated start address of the image.
		private uint rvaExportTable;
		private uint sizeExportTable;
		private uint rvaImportTable;
        private uint rvaDelayImportDescriptor;
        private Dictionary<Address, ImportReference> importReferences;
		private const short MACHINE_i386 = (short) 0x014C;
        private const short MACHINE_x86_64 = unchecked((short)0x8664);
		private const short ImageFileRelocationsStripped = 0x0001;
		private const short ImageFileExecutable = 0x0002;

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

		public IProcessorArchitecture CreateArchitecture(short peMachineType)
		{
			switch (peMachineType)
			{
			case MACHINE_i386: return new IntelArchitecture(ProcessorMode.Protected32);
            case MACHINE_x86_64: return new IntelArchitecture(ProcessorMode.Protected64);
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
		}

        private short GetExpectedMagic(short peMachineType)
        {
            switch (peMachineType)
            {
            case MACHINE_i386: return 0x010B;
            case MACHINE_x86_64: return 0x020B;
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
			}
        }

        public override Program Load(Address addrLoad)
		{
			if (sections > 0)
			{
				LoadSections(addrLoad, sectionOffset, sections);
			}
			imgLoaded.BaseAddress = addrLoad;
            var program = new Program(imgLoaded, imageMap, arch, platform);
            this.importReferences = program.ImportReferences;
            return program;
		}

		public void LoadSectionBytes(Section s, byte [] rawImage, byte [] loadedImage)
		{
			Array.Copy(rawImage, s.OffsetRawData, loadedImage, s.VirtualAddress,
                s.SizeRawData);
            // s.VirtualSize);
		}

        private static IEnumerable<Section> ReadSections(LeImageReader rdr, int sections)
        {
            for (int i = 0; i < sections; ++i)
            {
                yield return ReadSection(rdr);
            }
        }

		/// <summary>
		/// Loads the sections
		/// </summary>
		/// <param name="sectionOffset"></param>
		/// <returns></returns>
		private void LoadSections(Address addrLoad, uint sectionOffset, int sections)
		{
			ImageReader rdr = new LeImageReader(RawImage, sectionOffset);
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

			imgLoaded = new LoadedImage(addrLoad, new byte[sectionMax.VirtualAddress + Math.Max(sectionMax.VirtualSize, sectionMax.SizeRawData)]);
            imageMap = imgLoaded.CreateImageMap();

			foreach (Section s in sectionMap.Values)
			{
				LoadSectionBytes(s, RawImage, imgLoaded.Bytes);
			}
		}

		public override Address PreferredBaseAddress
		{
			get { return Address.Ptr32(this.preferredBaseOfImage); }
		}

		public short ReadCoffHeader(ImageReader rdr)
		{
			short machine = rdr.ReadLeInt16();
            short expectedMagic = GetExpectedMagic(machine);
            arch = CreateArchitecture(machine);
			platform = new Win32Platform(Services, arch);

			sections = rdr.ReadLeInt16();
			sectionMap = new SortedDictionary<string, Section>();
			rdr.ReadLeUInt32();		// timestamp.
			rdr.ReadLeUInt32();		// COFF symbol table.
			rdr.ReadLeUInt32();		// #of symbols.
			optionalHeaderSize = rdr.ReadLeInt16();
			short fileFlags = rdr.ReadLeInt16();
			sectionOffset = (uint) ((int)rdr.Offset + optionalHeaderSize);
            return expectedMagic;
		}

		public void ReadOptionalHeader(ImageReader rdr, short expectedMagic)
		{
			if (optionalHeaderSize <= 0)
				throw new BadImageFormatException("Optional header size should be larger than 0 in a PE executable image file.");

			short magic = rdr.ReadLeInt16();
			if (magic != expectedMagic) // 0x010B || magic != 0x020B)
				throw new BadImageFormatException("Not a valid PE Header.");
			rdr.ReadByte();		// Linker major version
			rdr.ReadByte();		// Linker minor version
			rdr.ReadLeUInt32();		// code size (== .text section size)
			rdr.ReadLeUInt32();		// size of initialized data
			rdr.ReadLeUInt32();		// size of uninitialized data
			rvaStartAddress = rdr.ReadLeUInt32();
			uint rvaBaseOfCode = rdr.ReadLeUInt32();
			uint rvaBaseOfData = rdr.ReadLeUInt32();
			preferredBaseOfImage = rdr.ReadLeUInt32();
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
			uint stackReserve = rdr.ReadLeUInt32();
			uint stackCommit = rdr.ReadLeUInt32();
			uint heapReserve = rdr.ReadLeUInt32();
			uint heapCommit = rdr.ReadLeUInt32();
			rdr.ReadLeUInt32();			// obsolete
			uint dictionaryCount = rdr.ReadLeUInt32();

			rvaExportTable = rdr.ReadLeUInt32();
			sizeExportTable = rdr.ReadLeUInt32();
			rvaImportTable = rdr.ReadLeUInt32();
			uint importTableSize = rdr.ReadLeUInt32();
			rdr.ReadLeUInt32();			// resource address
			rdr.ReadLeUInt32();			// resource size
			rdr.ReadLeUInt32();			// exception address
			rdr.ReadLeUInt32();			// exception size
			rdr.ReadLeUInt32();			// certificate address
			rdr.ReadLeUInt32();			// certificate size
			uint rvaBaseRelocAddress = rdr.ReadLeUInt32();
			uint baseRelocSize = rdr.ReadLeUInt32();
            uint rvaDebug = rdr.ReadLeUInt32();
            uint cbDebug = rdr.ReadLeUInt32();
            uint rvaArchitecture = rdr.ReadLeUInt32();
            uint cbArchitecture = rdr.ReadLeUInt32();
            uint rvaGlobalPointer = rdr.ReadLeUInt32();
            uint cbGlobalPointer = rdr.ReadLeUInt32();
            uint rvaTls = rdr.ReadLeUInt32();
            uint cbTls = rdr.ReadLeUInt32();
            uint rvaLoadConfig = rdr.ReadLeUInt32();
            uint cbLoadConfig = rdr.ReadLeUInt32();
            uint rvaBoundImport = rdr.ReadLeUInt32();
            uint cbBoundImport = rdr.ReadLeUInt32();
            uint rvaIat = rdr.ReadLeUInt32();
            uint cbIat = rdr.ReadLeUInt32();
            this.rvaDelayImportDescriptor = rdr.ReadLeUInt32();
            uint cbDelayImportDescriptor = rdr.ReadLeUInt32();
		}

		private const ushort RelocationAbsolute = 0;
		private const ushort RelocationHigh = 1;
		private const ushort RelocationLow = 2;
		private const ushort RelocationHighLow = 3;

		public override RelocationResults Relocate(Address addrLoad)
		{
            AddSectionsToImageMap(addrLoad, imageMap);
            var relocations = imgLoaded.Relocations;
			Section relocSection;
            if (sectionMap.TryGetValue(".reloc", out relocSection))
			{
				ApplyRelocations(relocSection.OffsetRawData, relocSection.SizeRawData, (uint) addrLoad.ToLinear(), relocations);
			}
            var entryPoints = new List<EntryPoint> { new EntryPoint(addrLoad + rvaStartAddress, arch.CreateProcessorState()) };
			AddExportedEntryPoints(addrLoad, imageMap, entryPoints);
			ReadImportDescriptors(addrLoad);
            ReadDeferredLoadDescriptors(addrLoad);
            return new RelocationResults(entryPoints, relocations);
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
                var seg = imageMap.AddSegment(addrLoad + s.VirtualAddress, s.Name, acc);
                seg.IsDiscardable = s.IsDiscardable;
            }
        }

		public void ApplyRelocation(uint baseOfImage, uint page, ImageReader rdr, RelocationDictionary relocations)
		{
			ushort fixup = rdr.ReadLeUInt16();
			switch (fixup >> 12)
			{
			case RelocationAbsolute:
				// Used for padding to 4-byte boundary, ignore.
				break;
			case RelocationHighLow:
			{
				uint offset = page + (fixup & 0x0FFFu);
				uint n = (uint) (imgLoaded.ReadLeUInt32(offset) + (baseOfImage - preferredBaseOfImage));
				imgLoaded.WriteLeUInt32(offset, n);
				relocations.AddPointerReference(offset, n);
				break;
			}
			default:
				throw new NotImplementedException(string.Format("Fixup type: {0:X}", fixup >> 12));
			}
		}

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

            ImageReader rdrIlt = imgLoaded.CreateLeReader(rvaILT);
            ImageReader rdrIat = imgLoaded.CreateLeReader(rvaIAT);
            for (; ; )
            {
                Address addrThunk = rdrIat.Address;
                uint iatEntry = rdrIat.ReadLeUInt32();
                uint iltEntry = rdrIlt.ReadLeUInt32();
                if (iltEntry == 0)
                    break;
                 
                importReferences.Add(
                    addrThunk,
                    ResolveImportedFunction(dllName, iltEntry, addrThunk));
            }
            return true;
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
                    ResolveImportedFunction(dllName, rvaName, addrThunk));
            }
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();
            rdr.ReadLeInt32();  // time stamp
            return true;
        }
        
        private ImportReference ResolveImportedFunction(string dllName, uint rvaEntry, Address addrThunk)
        {
            if (!ImportedFunctionNameSpecified(rvaEntry))
            {
                return new OrdinalImportReference(
                    addrThunk, dllName, (int) rvaEntry & 0x7FFFFFF);
            }
            else
            {
                string fnName = ReadUtf8String(rvaEntry + 2, 0);
                return new NamedImportReference(
                    addrThunk, dllName, fnName);
            }
        }

        private bool ImportedFunctionNameSpecified(uint rvaEntry)
        {
            return (rvaEntry & 0x80000000) == 0;
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

        internal static uint ReadEntryPoint(byte[] image, uint peHeaderOffset)
        {
            short sections = LoadedImage.ReadLeInt16(image, peHeaderOffset + 0x06);
            uint pEntryPoint= LoadedImage.ReadLeUInt32(image, peHeaderOffset + 0x28);
            var section = ReadSections(new LeImageReader(image, peHeaderOffset + 0xF8), sections)
                .Where(s => s.VirtualAddress <= pEntryPoint && pEntryPoint < s.VirtualAddress + s.VirtualSize)
                .FirstOrDefault();
            if (section == null)
                return 0;
            else
                return (pEntryPoint - section.VirtualAddress) + section.OffsetRawData;
        }
    }
}
