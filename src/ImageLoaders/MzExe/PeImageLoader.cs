#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
				throw new BadImageFormatException("Not a valid PE header.");
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
			rdr.ReadLeUInt32();	// Characteristics
			rdr.ReadLeUInt32(); // timestamp
			rdr.ReadLeUInt32();	// version.
			rdr.ReadLeUInt32();	// binary name.
			rdr.ReadLeUInt32();	// base ordinal
			int nExports = rdr.ReadLeInt32();
			int nNames = rdr.ReadLeInt32();
			if (nExports != nNames)
				throw new ApplicationException("Unexpected discrepancy in PE image.");
			uint rvaApfn = rdr.ReadLeUInt32();
			uint rvaNames = rdr.ReadLeUInt32();

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
            uint addr = rdrAddrs.ReadLeUInt32();
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
			ImageReader rdr = new ImageReader(RawImage, sectionOffset);
			var section = ReadSection(rdr);
			var sectionMax = section;
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
			short machine = rdr.ReadLeInt16();
			arch = CreateArchitecture(machine);
			platform = new Win32Platform(arch);

			sections = rdr.ReadLeInt16();
			sectionMap = new SortedDictionary<string, Section>();
			rdr.ReadLeUInt32();		// timestamp.
			rdr.ReadLeUInt32();		// COFF symbol table.
			rdr.ReadLeUInt32();		// #of symbols.
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
			uint stackReserve  = rdr.ReadLeUInt32();
			uint stackCommit  = rdr.ReadLeUInt32();
			uint heapReserve  = rdr.ReadLeUInt32();
			uint heapCommit   = rdr.ReadLeUInt32();
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
			entryPoints.Add(new EntryPoint(addrLoad + rvaStartAddress, arch.CreateProcessorState()));
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

				uint page = rdr.ReadLeUInt32();
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
			return Path.Combine(assemblyDir, Path.ChangeExtension(dllName, ".xml"));
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
			id.RvaEntries = rdr.ReadLeUInt32();		// IAT
			if (id.RvaEntries == 0)
				return null;
			rdr.ReadLeUInt32();		// datestamp
			rdr.ReadLeUInt32();		// forwarder chain
			id.DllName = ReadAsciiString(rdr.ReadLeUInt32(), 0);		// DLL name
			id.RvaThunks = rdr.ReadLeUInt32();		// first thunk

            SignatureLibrary lib = LoadSignatureLibrary(arch, id.DllName);
            if (lib == null)
            {
                GetService<DecompilerEventListener>().AddDiagnostic(new NullCodeLocation(""),
                    new Diagnostic(string.Format("Unable to locate signature library for {0}.", id.DllName)));
            }
			ImageReader rdrEntries = imgLoaded.CreateReader(id.RvaEntries);
			ImageReader rdrThunks  = imgLoaded.CreateReader(id.RvaThunks);
			for (;;)
			{
				Address addrThunk = imgLoaded.BaseAddress + rdrThunks.Offset;
				uint rvaEntry = rdrEntries.ReadLeUInt32();
				uint rvaThunk = rdrThunks.ReadLeUInt32();
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
                AddUnresolvedImport(id, string.Format("Ordinal_{0}", rvaEntry & 0x7FFFFFF));
                return;
            }
            string fnName = ReadAsciiString(rvaEntry + 2, 0);
            if (lib == null)
            {
                AddUnresolvedImport(id, fnName);
                return;
            }
            ProcedureSignature sig = lib.Lookup(fnName);
            if (sig == null)
            {
                AddUnresolvedImport(id, fnName);
                return;
            }
            importThunks.Add(addrThunk.Offset, new PseudoProcedure(fnName, sig));
        }

        private void AddUnresolvedImport(ImportDescriptor id, string fnName)
        {
            unresolvedImports.Add(new ImportedFunction(id, fnName));
            GetService<DecompilerEventListener>().AddDiagnostic(new NullCodeLocation(""),
                new Diagnostic(string.Format("Unable to locate signature for {0} ({1}).", fnName, id.DllName)));
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
                string libFileName = ImportFileLocation(dllName);
                if (!File.Exists(libFileName))
                    return null;
                lib.Load(libFileName);
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
