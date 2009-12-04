/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Environments.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.ImageLoaders.MzExe
{
	public class PeImageLoader : ImageLoader
	{
        private IProcessorArchitecture arch;
        private Platform platform;
        private Dictionary<uint, PseudoProcedure> importThunks;
        private List<ImportedFunction> unresolvedImports;

		private short optionalHeaderSize;
		private int sections;
		private uint sectionOffset;
		private ProgramImage imgLoaded;
		private uint preferredBaseOfImage;
		private SortedDictionary<string, Section> sectionMap;
		private uint rvaStartAddress;		// unrelocated start address of the image.
		private uint rvaExportTable;
		private uint sizeExportTable;
		private uint rvaImportTable;

		private const short MACHINE_i386 = 0x14C;

		private const short ImageFileRelocationsStripped = 0x0001;
		private const short ImageFileExecutable = 0x0002;


		public PeImageLoader(IServiceProvider services, byte [] img, uint peOffset) : base(services, img)
		{
			ImageReader rdr = new ImageReader(RawImage, peOffset);
			if (rdr.ReadByte() != 'P' ||
				rdr.ReadByte() != 'E' ||
				rdr.ReadByte() != 0x0 ||
				rdr.ReadByte() != 0x0)
			{
				throw new ApplicationException("Not a valid PE header.");
			}
            importThunks = new Dictionary<uint, PseudoProcedure>();
			ReadCoffHeader(rdr);
			ReadOptionalHeader(rdr);
		}

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Platform Platform
        {
            get { return platform; }
        }

		private void AddExportedEntryPoints(Address addrLoad, ImageMap imageMap, List<EntryPoint> entryPoints)
		{
			ImageReader rdr = imgLoaded.CreateReader(rvaExportTable);
			rdr.ReadLeUint32();	// Characteristics
			rdr.ReadLeUint32(); // timestamp
			rdr.ReadLeUint32();	// version.
			rdr.ReadLeUint32();	// binary name.
			rdr.ReadLeUint32();	// base ordinal
			int nExports = rdr.ReadLeInt32();
			int nNames = rdr.ReadLeInt32();
			if (nExports != nNames)
				throw new ApplicationException("Unexpected discrepancy in PE image.");
			uint rvaApfn = rdr.ReadLeUint32();
			uint rvaNames = rdr.ReadLeUint32();

			ImageReader rdrAddrs = imgLoaded.CreateReader(rvaApfn);
			ImageReader rdrNames = imgLoaded.CreateReader(rvaNames);
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
            uint addr = rdrAddrs.ReadLeUint32();
            int iNameMin = rdrNames.ReadLeInt32();
            int j;
            for (j = iNameMin; imgLoaded.Bytes[j] != 0; ++j)
                ;
            char[] chars = Encoding.ASCII.GetChars(imgLoaded.Bytes, iNameMin, j - iNameMin);
            EntryPoint ep = new EntryPoint(addrLoad + addr, new string(chars), arch.CreateProcessorState());
            return ep;
        }

		public IProcessorArchitecture CreateArchitecture(short peMachineType)
		{
			switch (peMachineType)
			{
			case MACHINE_i386: return new IntelArchitecture(ProcessorMode.ProtectedFlat);
			default: throw new ArgumentException(string.Format("Unsupported machine type 0x{0:X4} in PE header.", peMachineType));
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

			short machine = rdr.ReadLeInt16();
			arch = CreateArchitecture(machine);
			platform = new Win32Platform(arch);

			sections = rdr.ReadLeInt16();
			sectionMap = new SortedDictionary<string, Section>();
			rdr.ReadLeUint32();		// timestamp.
			rdr.ReadLeUint32();		// COFF symbol table.
			rdr.ReadLeUint32();		// #of symbols.
			optionalHeaderSize = rdr.ReadLeInt16();
			short fileFlags = rdr.ReadLeInt16();
			sectionOffset = (uint) (rdr.Offset + optionalHeaderSize);
		}

		public void ReadOptionalHeader(ImageReader rdr)
		{
			if (optionalHeaderSize <= 0)
				throw new ApplicationException("Optional header size should be larger than 0 in a PE executable image file.");

			short magic = rdr.ReadLeInt16();
			if (magic != 0x010B)
				throw new ApplicationException("Not a valid PE Header.");
			rdr.ReadByte();		// Linker major version
			rdr.ReadByte();		// Linker minor version
			rdr.ReadLeUint32();		// code size (== .text section size)
			rdr.ReadLeUint32();		// size of initialized data
			rdr.ReadLeUint32();		// size of uninitialized data
			rvaStartAddress = rdr.ReadLeUint32();
			uint rvaBaseOfCode = rdr.ReadLeUint32();
			uint rvaBaseOfData = rdr.ReadLeUint32();
			preferredBaseOfImage = rdr.ReadLeUint32();
			rdr.ReadLeUint32();		// section alignment
			rdr.ReadLeUint32();		// file alignment
			rdr.ReadLeUint16();		// OS major version
			rdr.ReadLeUint16();		// OS minor version
			rdr.ReadLeUint16();		// Image major version
			rdr.ReadLeUint16();		// Image minor version
			rdr.ReadLeUint16();		// Subsystem major version
			rdr.ReadLeUint16();		// Subsystem minor version
			rdr.ReadLeUint32();		// reserved
			uint sizeOfImage = rdr.ReadLeUint32();
			uint sizeOfHeaders = rdr.ReadLeUint32();
			uint checksum = rdr.ReadLeUint32();
			ushort subsystem = rdr.ReadLeUint16();
			ushort dllFlags = rdr.ReadLeUint16();
			uint stackReserve  = rdr.ReadLeUint32();
			uint stackCommit  = rdr.ReadLeUint32();
			uint heapReserve  = rdr.ReadLeUint32();
			uint heapCommit   = rdr.ReadLeUint32();
			rdr.ReadLeUint32();			// obsolete
			uint dictionaryCount = rdr.ReadLeUint32();

			rvaExportTable = rdr.ReadLeUint32();
			sizeExportTable = rdr.ReadLeUint32();
			rvaImportTable = rdr.ReadLeUint32();
			uint importTableSize = rdr.ReadLeUint32();
			rdr.ReadLeUint32();			// resource address
			rdr.ReadLeUint32();			// resource size
			rdr.ReadLeUint32();			// exception address
			rdr.ReadLeUint32();			// exception size
			rdr.ReadLeUint32();			// certificate address
			rdr.ReadLeUint32();			// certificate size
			uint rvaBaseRelocAddress = rdr.ReadLeUint32();
			uint baseRelocSize = rdr.ReadLeUint32();
		}

		private const ushort RelocationAbsolute = 0;
		private const ushort RelocationHigh = 1;
		private const ushort RelocationLow = 2;
		private const ushort RelocationHighLow = 3;

		public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
		{
			ImageMap imageMap = imgLoaded.Map;
            AddSectionsToImageMap(addrLoad, imageMap);
			
			Section relocSection;
            if (sectionMap.TryGetValue(".reloc", out relocSection))
			{
				ApplyRelocations(relocSection.OffsetRawData, relocSection.VirtualSize, (uint) addrLoad.Linear, relocations);
			}
			entryPoints.Add(new EntryPoint(addrLoad + rvaStartAddress, new IntelState()));
			AddExportedEntryPoints(addrLoad, imageMap, entryPoints);
			ReadImportDescriptors(addrLoad);
		}

        private void AddSectionsToImageMap(Address addrLoad, ImageMap imageMap)
        {
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
        }

		public void ApplyRelocation(uint baseOfImage, uint page, ImageReader rdr, RelocationDictionary relocations)
		{
			ushort fixup = rdr.ReadLeUint16();
			switch (fixup >> 12)
			{
			case RelocationAbsolute:
				// Used for padding to 4-byte boundary, ignore.
				break;
			case RelocationHighLow:
			{
				int offset = (int)(page + (fixup & 0x0FFF));
				uint n = (uint) (imgLoaded.ReadLeUint32(offset) + (baseOfImage - preferredBaseOfImage));
				imgLoaded.WriteLeUint32(offset, n);
				relocations.AddPointerReference(offset, n);
				break;
			}
			default:
				throw new NotImplementedException(string.Format("Fixup type: {0:X}", fixup >> 12));
			}

		}

		public void ApplyRelocations(uint rvaReloc, uint size, uint baseOfImage, RelocationDictionary relocations)
		{
			ImageReader rdr = new ImageReader(RawImage, rvaReloc);
			uint rvaStop = rvaReloc + size;
			while (rdr.Offset < rvaStop)
			{
				// Read fixup block header.

				uint page = rdr.ReadLeUint32();
				int cbBlock = rdr.ReadLeInt32();
				uint offBlockEnd = (uint)(rdr.Offset + cbBlock - 8);
				while (rdr.Offset < offBlockEnd)
				{
					ApplyRelocation(baseOfImage, page, rdr, relocations);
				}
			}
		}

        public override Dictionary<uint, PseudoProcedure> ImportThunks
        {
            get
            {
                return importThunks;
            }
        }

		public string ImportFileLocation(string dllName)
		{
			string assemblyDir = Path.GetDirectoryName(GetType().Assembly.Location);
			return System.IO.Path.Combine(assemblyDir, System.IO.Path.ChangeExtension(dllName, ".xml"));
		}

		public string ReadAsciiString(uint rva, int maxLength)
		{
			ImageReader rdr = imgLoaded.CreateReader(rva);
			List<byte> bytes = new List<byte>();
			byte b;
			while ((b = rdr.ReadByte()) != 0)
			{
				bytes.Add(b);
				if (bytes.Count == maxLength)
					break;
			}
			return Encoding.ASCII.GetString(bytes.ToArray());
		}

		public ImportDescriptor ReadImportDescriptor(ImageReader rdr, Address addrLoad)
		{
			ImportDescriptor id = new ImportDescriptor();
			id.RvaEntries = rdr.ReadLeUint32();		// IAT
			if (id.RvaEntries == 0)
				return null;
			rdr.ReadLeUint32();		// datestamp
			rdr.ReadLeUint32();		// forwarder chain
			id.DllName = ReadAsciiString(rdr.ReadLeUint32(), 0);		// DLL name
			id.RvaThunks = rdr.ReadLeUint32();		// first thunk

            SignatureLibrary lib = LoadSignatureLibrary(arch, id.DllName);
			ImageReader rdrEntries = imgLoaded.CreateReader(id.RvaEntries);
			ImageReader rdrThunks  = imgLoaded.CreateReader(id.RvaThunks);
			for (;;)
			{
				Address addrThunk = imgLoaded.BaseAddress + rdrThunks.Offset;
				uint rvaEntry = rdrEntries.ReadLeUint32();
				uint rvaThunk = rdrThunks.ReadLeUint32();
				if (rvaEntry == 0)
					break;

                ResolveImportedFunction(id, lib, rvaEntry, addrThunk);
			}
			return id;
		}


        private void ResolveImportedFunction(ImportDescriptor id, SignatureLibrary lib, uint rvaEntry, Address addrThunk)
        {
            if (!ImportedFunctionNameSpecified(rvaEntry))
            {
                unresolvedImports.Add(new ImportedFunction(id, string.Format("Ordinal_{0}", rvaEntry & 0x7FFFFFF)));
                return;
            }
            string fnName = ReadAsciiString(rvaEntry + 2, 0);
            if (lib == null)
            {
                unresolvedImports.Add(new ImportedFunction(id, fnName));
                return;
            }
            ProcedureSignature sig = lib.Lookup(fnName);
            if (sig == null)
            {
                unresolvedImports.Add(new ImportedFunction(id, fnName));
                return;
            }

            importThunks.Add(addrThunk.Offset, new PseudoProcedure(fnName, sig));
        }

        private bool ImportedFunctionNameSpecified(uint rvaEntry)
        {
            return (rvaEntry & 0x80000000) == 0;
        }

        protected virtual SignatureLibrary LoadSignatureLibrary(IProcessorArchitecture arch, string dllName)
        {
            try
            {
                SignatureLibrary lib = new SignatureLibrary(arch);
                lib.Load(ImportFileLocation(dllName));
                return lib;
            }
            catch
            {
                return null;
            }
        }

		private void ReadImportDescriptors(Address addrLoad)
		{
            unresolvedImports = new List<ImportedFunction>();
			ImageReader rdr = imgLoaded.CreateReader(rvaImportTable);
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
			sec.VirtualSize = rdr.ReadLeUint32();
			sec.VirtualAddress = rdr.ReadLeUint32();
			sec.SizeRawData = rdr.ReadLeUint32();
			sec.OffsetRawData = rdr.ReadLeUint32();
			rdr.ReadLeUint32();			// pointer to relocations
			rdr.ReadLeUint32();			// pointer to line numbers.
			rdr.ReadLeInt16();		// # of relocations
			rdr.ReadLeInt16();		// # of line numbers
			sec.Flags = rdr.ReadLeUint32();

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

        public class ImportedFunction
        {
            public ImportDescriptor ImportDescriptor;
            public string FunctionName;

            public ImportedFunction(ImportDescriptor id, string functionName)
            {
                this.ImportDescriptor = id;
                this.FunctionName = functionName;
            }
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
