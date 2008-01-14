/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Arch.Intel;		
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Loading
{
	public class PeImageLoader : ImageLoader
	{
		private short optionalHeaderSize;
		private int sections;
		private uint sectionOffset;
		private ProgramImage imgLoaded;
		private uint preferredBaseOfImage;
		private SortedList sectionMap;
		private Program program;
		private uint rvaStartAddress;		// unrelocated start address of the image.
		private uint rvaExportTable;
		private uint sizeExportTable;
		private uint rvaImportTable;

		private const short MACHINE_i386 = 0x14C;

		private const short ImageFileRelocationsStripped = 0x0001;
		private const short ImageFileExecutable = 0x0002;


		public PeImageLoader(Program pgm, byte [] img, uint peOffset) : base(img)
		{
			program = pgm;
			ImageReader rdr = new ImageReader(RawImage, peOffset);
			if (rdr.ReadByte() != 'P' ||
				rdr.ReadByte() != 'E' ||
				rdr.ReadByte() != 0x0 ||
				rdr.ReadByte() != 0x0)
			{
				throw new ApplicationException("Not a valid PE header");
			}
			ReadCoffHeader(rdr);
			ReadOptionalHeader(rdr);
		}

		private void AddExportedEntryPoints(Address addrLoad, ImageMap imageMap, ArrayList entryPoints)
		{
			ImageReader rdr = imgLoaded.CreateReader(rvaExportTable);
			rdr.ReadUint();	// Characteristics
			rdr.ReadUint(); // timestamp
			rdr.ReadUint();	// version.
			rdr.ReadUint();	// binary name.
			rdr.ReadUint();	// base ordinal
			int nExports = rdr.ReadInt();
			int nNames = rdr.ReadInt();
			if (nExports != nNames)
				throw new ApplicationException("Unexpected discrepancy in PE image");
			uint rvaApfn = rdr.ReadUint();
			uint rvaNames = rdr.ReadUint();

			ImageReader rdrAddrs = imgLoaded.CreateReader(rvaApfn);
			ImageReader rdrNames = imgLoaded.CreateReader(rvaNames);
			for (int i = 0; i < nNames; ++i)
			{
				uint addr = rdrAddrs.ReadUint();
				int iNameMin = rdrNames.ReadInt();
				int j;
				for (j = iNameMin; imgLoaded.Bytes[j] != 0; ++j)
					;
				char [] chars = Encoding.ASCII.GetChars(imgLoaded.Bytes, iNameMin, j - iNameMin);
				Address addrEp = addrLoad + addr;
				if (imageMap.IsExecutableAddress(addrEp))
				{
					EntryPoint ep = new EntryPoint(addrLoad + addr, new String(chars), new IntelState());
					entryPoints.Add(ep);
				}
			}
		}

		public IProcessorArchitecture CreateArchitecture(short peMachineType)
		{
			switch (peMachineType)
			{
			case MACHINE_i386: return new IntelArchitecture(ProcessorMode.ProtectedFlat);
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header", peMachineType));
			}
		}

		public override ProgramImage Load(Address addrLoad)
		{
			if (sections > 0)
			{
				LoadSections(addrLoad, sectionOffset, sections);
			}
			imgLoaded.BaseAddress = addrLoad;
			return imgLoaded;
		}

		public void LoadSectionBytes(Section s, byte [] rawImage, byte [] loadedImage)
		{
			Array.Copy(rawImage, s.OffsetRawData, loadedImage, s.VirtualAddress, s.VirtualSize);
		}

		/// <summary>
		/// Loads the sections
		/// </summary>
		/// <param name="sectionOffset"></param>
		/// <returns></returns>
		private void LoadSections(Address addrLoad, uint sectionOffset, int sections)
		{
			// Read the sections.

			Section section;
			ImageReader rdr = new ImageReader(RawImage, sectionOffset);
			section = ReadSection(rdr);
			Section sectionMax = section;
			sectionMap[section.Name] = section;
			
			for (int i = 1; i != sections; ++i)
			{
				section = ReadSection(rdr);
				sectionMap[section.Name] = section;
				if (!section.IsDiscardable && section.VirtualAddress > sectionMax.VirtualAddress)
					sectionMax = section;
			}

			imgLoaded = new ProgramImage(addrLoad, new byte[sectionMax.VirtualAddress + Math.Max(sectionMax.VirtualSize, sectionMax.SizeRawData)]);

			foreach (Section s in sectionMap.Values)
			{
				if (!s.IsDiscardable)
				{
					LoadSectionBytes(s, RawImage, imgLoaded.Bytes);
				}
			}
		}


		public override Address PreferredBaseAddress
		{
			get { return new Address(this.preferredBaseOfImage); }
		}

		public void ReadCoffHeader(ImageReader rdr)
		{
			// Read COFF header.

			short machine = rdr.ReadShort();
			program.Architecture = CreateArchitecture(machine);
			program.Platform = new Decompiler.Arch.Intel.Win32.Win32Platform(program.Architecture);

			sections = rdr.ReadShort();
			sectionMap = new SortedList(sections);
			rdr.ReadUint();		// timestamp.
			rdr.ReadUint();		// COFF symbol table.
			rdr.ReadUint();		// #of symbols.
			optionalHeaderSize = rdr.ReadShort();
			short fileFlags = rdr.ReadShort();
			sectionOffset = (uint) (rdr.Offset + optionalHeaderSize);
		}

		public void ReadOptionalHeader(ImageReader rdr)
		{
			if (optionalHeaderSize <= 0)
				throw new ApplicationException("Optional header size should be larger than 0 in a PE image file");

			short magic = rdr.ReadShort();
			if (magic != 0x010B)
				throw new ApplicationException("Not a valid PE Header");
			rdr.ReadByte();		// Linker major version
			rdr.ReadByte();		// Linker minor version
			rdr.ReadUint();		// code size (== .text section size)
			rdr.ReadUint();		// size of initialized data
			rdr.ReadUint();		// size of uninitialized data
			rvaStartAddress = rdr.ReadUint();
			uint rvaBaseOfCode = rdr.ReadUint();
			uint rvaBaseOfData = rdr.ReadUint();
			preferredBaseOfImage = rdr.ReadUint();
			rdr.ReadUint();		// section alignment
			rdr.ReadUint();		// file alignment
			rdr.ReadUShort();		// OS major version
			rdr.ReadUShort();		// OS minor version
			rdr.ReadUShort();		// Image major version
			rdr.ReadUShort();		// Image minor version
			rdr.ReadUShort();		// Subsystem major version
			rdr.ReadUShort();		// Subsystem minor version
			rdr.ReadUint();		// reserved
			uint sizeOfImage = rdr.ReadUint();
			uint sizeOfHeaders = rdr.ReadUint();
			uint checksum = rdr.ReadUint();
			ushort subsystem = rdr.ReadUShort();
			ushort dllFlags = rdr.ReadUShort();
			uint stackReserve  = rdr.ReadUint();
			uint stackCommit  = rdr.ReadUint();
			uint heapReserve  = rdr.ReadUint();
			uint heapCommit   = rdr.ReadUint();
			rdr.ReadUint();			// obsolete
			uint dictionaryCount = rdr.ReadUint();

			rvaExportTable = rdr.ReadUint();
			sizeExportTable = rdr.ReadUint();
			rvaImportTable = rdr.ReadUint();
			uint importTableSize = rdr.ReadUint();
			rdr.ReadUint();			// resource address
			rdr.ReadUint();			// resource size
			rdr.ReadUint();			// exception address
			rdr.ReadUint();			// exception size
			rdr.ReadUint();			// certificate address
			rdr.ReadUint();			// certificate size
			uint rvaBaseRelocAddress = rdr.ReadUint();
			uint baseRelocSize = rdr.ReadUint();
		}

		private const ushort RelocationAbsolute = 0;
		private const ushort RelocationHigh = 1;
		private const ushort RelocationLow = 2;
		private const ushort RelocationHighLow = 3;

		public override void Relocate(Address addrLoad, ArrayList entryPoints)
		{
			ImageMap imageMap = imgLoaded.Map;
			foreach (Section s in sectionMap.Values)
			{
				if (!s.IsDiscardable)
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
					imageMap.AddSegment(addrLoad + s.VirtualAddress, s.Name, acc);
				}
			}
			
			Section relocSection = (Section) sectionMap[".reloc"];
			if (relocSection != null)
			{
				ApplyRelocations(relocSection.OffsetRawData, relocSection.SizeRawData, (uint) addrLoad.Linear);
			}
			entryPoints.Add(new EntryPoint(addrLoad + rvaStartAddress, new IntelState()));
			AddExportedEntryPoints(addrLoad, imageMap, entryPoints);
			ReadImportDescriptors(addrLoad);
		}

		public void ApplyRelocations(uint rvaReloc, uint size, uint baseOfImage)
		{
			ImageReader rdr = new ImageReader(RawImage, rvaReloc);
			uint rvaStop = rvaReloc + size;
			while (rdr.Offset < rvaStop)
			{
				// Read fixup block header.

				uint page = rdr.ReadUint();
				int cbBlock = rdr.ReadInt();
				uint offBlockEnd = (uint)(rdr.Offset + cbBlock - 8);
				while (rdr.Offset < offBlockEnd)
				{
					// Read a fixup.

					ushort fixup = rdr.ReadUShort();
					switch (fixup >> 12)
					{
					case RelocationAbsolute:
						// Used for padding to 4-byte boundary, ignored.
						break;
					case RelocationHighLow:
					{
						int offset = (int)(page + (fixup & 0x0FFF));
						uint n = (uint) (imgLoaded.ReadUint(offset) + (baseOfImage - preferredBaseOfImage));
						imgLoaded.WriteUint(offset, n);
						//$TODO: this offset should be marked as 'contains pointer' and used for type analysis
						// and jump table analysis.
						break;
					}
					default:
						throw new NotImplementedException(string.Format("NYI {0:X}", fixup >> 12));
					}
				}
			}
		}

		public string ImportFileLocation(string dllName)
		{
			string assemblyDir = System.IO.Path.GetDirectoryName(GetType().Assembly.Location);
			return System.IO.Path.Combine(assemblyDir, System.IO.Path.ChangeExtension(dllName, ".xml"));
		}

		public string ReadAsciiString(uint rva, int maxLength)
		{
			ImageReader rdr = new ImageReader(RawImage, rva);
			ArrayList bytes = new ArrayList();
			byte b;
			while ((b = rdr.ReadByte()) != 0)
			{
				bytes.Add(b);
				if (bytes.Count == maxLength)
					break;
			}
			byte [] bs = (byte []) bytes.ToArray(typeof (byte));
			char [] chars = Encoding.ASCII.GetChars(bs);
			return new String(chars);
		}

		public ImportDescriptor ReadImportDescriptor(ImageReader rdr, Address addrLoad)
		{
			ImportDescriptor id = new ImportDescriptor();
			id.RvaEntries = rdr.ReadUint();		// IAT
			if (id.RvaEntries == 0)
				return null;
			rdr.ReadUint();		// datestamp
			rdr.ReadUint();		// forwarder chain
			id.DllName = ReadAsciiString(rdr.ReadUint(), 0);		// DLL name
			id.RvaThunks = rdr.ReadUint();		// first thunk

			SignatureLibrary lib = new SignatureLibrary(program.Architecture);
			
			lib.Load(ImportFileLocation(id.DllName));
			ImageReader rdrEntries = imgLoaded.CreateReader(id.RvaEntries);
			ImageReader rdrThunks  = imgLoaded.CreateReader(id.RvaThunks);
			for (;;)
			{
				Address addrThunk = imgLoaded.BaseAddress + rdrThunks.Offset;
				uint rvaEntry = rdrEntries.ReadUint();
				uint rvaThunk = rdrThunks.ReadUint();
				if (rvaEntry == 0)
					break;
			
				string fnName = ReadAsciiString(rvaEntry + 2, 0);
				program.ImportThunks.Add(addrThunk.off, new PseudoProcedure(fnName, lib.Lookup(fnName)));
			}
			return id;
		}

		private void ReadImportDescriptors(Address addrLoad)
		{
			ImageReader rdr = new ImageReader(RawImage, rvaImportTable);
			for (;;)
			{
				ImportDescriptor id = ReadImportDescriptor(rdr, addrLoad);
				if (id == null)
					break;
			}
		}

		private Section ReadSection(ImageReader rdr)
		{
			Section sec = new Section();
			sec.Name = ReadSectionName(rdr);
			sec.VirtualSize = rdr.ReadUint();
			sec.VirtualAddress = rdr.ReadUint();
			sec.SizeRawData = rdr.ReadUint();
			sec.OffsetRawData = rdr.ReadUint();
			rdr.ReadUint();			// pointer to relocations
			rdr.ReadUint();			// pointer to line numbers.
			rdr.ReadShort();		// # of relocations
			rdr.ReadShort();		// # of line numbers
			sec.Flags = rdr.ReadUint();

			return sec;
		}

		private string ReadSectionName(ImageReader rdr)
		{
			byte [] bytes = new Byte[8];
			for (int b = 0; b < bytes.Length; ++b)
			{
				bytes[b] = rdr.ReadByte();
			}

			ASCIIEncoding asc = new ASCIIEncoding();
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

		public class ImportDescriptor
		{
			public uint RvaEntries;
			public string DllName;
			public uint RvaThunks;
		}

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
	}
}
