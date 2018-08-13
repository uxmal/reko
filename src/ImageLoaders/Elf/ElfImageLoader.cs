#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

// http://hitmen.c02.at/files/yapspd/psp_doc/chap26.html - PSP ELF

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// Loader for ELF images.
    /// </summary>
    public class ElfImageLoader : ImageLoader
    {
        #region Constants
        private const int ELF_MAGIC = 0x7F454C46;         // "\x7FELF"
        private const byte LITTLE_ENDIAN = 1;
        private const byte BIG_ENDIAN = 2;
        private const byte ELFCLASS32 = 1;              // 32-bit object file
        private const byte ELFCLASS64 = 2;              // 64-bit object file
        public const int HEADER_OFFSET = 0x0010;

        public const int ET_REL = 0x01;

        #endregion

        internal static TraceSwitch trace = new TraceSwitch("HunkLoader", "Traces the progress of the Amiga Hunk loader") { Level = TraceLevel.Warning };

        private byte fileClass;
        private byte endianness;
        private byte fileVersion;
        private byte osAbi;
        private Address addrPreferred;

        protected ElfLoader innerLoader;

        public ElfImageLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
        }

        public override Address PreferredBaseAddress 
        {
            get { return addrPreferred; }
            set { addrPreferred = value; }
        }

        public override Program Load(Address addrLoad)
        {
            LoadElfIdentification();
            this.innerLoader = CreateLoader();
            if (addrLoad == null)
                addrLoad = innerLoader.DefaultAddress;
            var platform = innerLoader.LoadPlatform(osAbi, innerLoader.Architecture);
            int cHeaders = innerLoader.LoadSegments();
            innerLoader.LoadSectionHeaders();
            innerLoader.LoadSymbolsFromSections();
            //innerLoader.Dump();           // This spews a lot into the unit test output.
            if (cHeaders > 0)
            {
                return innerLoader.LoadImage(platform, RawImage);
            }
            else
            {
                // The file we're loading is an object file, and needs to be 
                // linked before we can load it.
                var linker = innerLoader.CreateLinker();
                return linker.LinkObject(platform, addrLoad, RawImage);
            }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            var reloc = innerLoader.Relocate(program, addrLoad);
            return reloc;
        }

        public void LoadElfIdentification()
        {
            var rdr = new BeImageReader(base.RawImage, 0);
            var elfMagic = rdr.ReadBeInt32();
            if (elfMagic != ELF_MAGIC)
                throw new BadImageFormatException("File is not in ELF format.");
            this.fileClass = rdr.ReadByte();
            this.endianness = rdr.ReadByte();
            this.fileVersion = rdr.ReadByte();
            this.osAbi = rdr.ReadByte();
        }

        public EndianImageReader CreateReader(ulong fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public EndianImageReader CreateReader(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageReader(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageReader(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ImageWriter CreateWriter(uint fileOffset)
        {
            switch (endianness)
            {
            case LITTLE_ENDIAN: return new LeImageWriter(RawImage, fileOffset);
            case BIG_ENDIAN: return new BeImageWriter(RawImage, fileOffset);
            default: throw new BadImageFormatException("Endianness is incorrectly specified.");
            }
        }

        public ElfLoader CreateLoader()
        {
            var rdr = CreateReader(HEADER_OFFSET);
            if (fileClass == ELFCLASS64)
            {
                var header64 = Elf64_EHdr.Load(rdr);
                return new ElfLoader64(this, header64, RawImage, osAbi, endianness);
            }
            else
            {
                var header32 = Elf32_EHdr.Load(rdr);
                return new ElfLoader32(this, header32, RawImage, endianness);
            }
        }

        public string ReadAsciiString(ulong fileOffset)
        {
            var bytes = RawImage;
            if (fileOffset >= (ulong) bytes.Length)
                return "";
            int u = (int)fileOffset;
            while (bytes[u] != 0)
            {
                ++u;
            }
            return Encoding.ASCII.GetString(bytes, (int)fileOffset, u - (int)fileOffset);
        }
    }


#if ZLON
    public class ElfObsolete
    {
        private long m_lImageSize; // Size of image in bytes
        private byte[] m_pImage; // Pointer to the loaded image
        private Elf32_Ehdr pHeader;
        private Elf32_Phdr[] m_pPhdrs; // Pointer to program header
        private Elf32_Shdr[] m_pShdrs; // Array of section header structs
        private uint stringTableOffset; // Pointer to the string section
        private bool bigEndian; // 1 = Big Endian
        private Dictionary<ADDRESS, string> m_SymTab; // Map from address to symbol name; contains symbols from the
        // various elf symbol tables, and possibly some symbols with fake
        // addresses
        private SymTab m_Reloc; // Object to store the reloc syms
        private Elf32_Rel m_pReloc; // Pointer to the relocation section
        private Elf32_Sym[] m_pSym; // Pointer to loaded symbol section
        private bool m_bAddend; // true if reloc table has addend
        private ADDRESS m_uLastAddr; // Save last address looked up
        private int m_iLastSize; // Size associated with that name
        private ADDRESS m_uPltMin; // Min address of PLT table
        private ADDRESS m_uPltMax; // Max address (1 past last) of PLT
        private List<SectionInfo> m_EntryPoint; // A list of one entry point
        private ADDRESS m_pImportStubs; // An array of import stubs
        private ADDRESS first_extern; // where the first extern will be placed
        private ADDRESS next_extern; // where the next extern will be placed
        private int[] m_sh_link; // pointer to array of sh_link values
        private int[] m_sh_info; // pointer to array of sh_info values
        private int m_iNumSections;
        private SectionInfo[] m_pSections;
        private IProcessorArchitecture arch;
        private Platform platform;

        public ElfLoader(IServiceProvider sp, byte[] rawImage)
            : base(sp, rawImage)
        {
            next_extern = 0;
            m_pImage = null;
            m_pPhdrs = null; // No program headers
            m_pShdrs = null; // No section headers
            stringTableOffset = 0; // No strings
            m_pReloc = null;
            m_pSym = null;
            m_uPltMin = 0; // No PLT limits
            m_uPltMax = 0;
            m_iLastSize = 0;
            m_pImportStubs = 0;

            m_SymTab = new Dictionary<ADDRESS, string>();
        }

        public override Address PreferredBaseAddress { get { return Address.Ptr(0x00100000); } }
        public override IProcessorArchitecture Architecture { get { return arch; } }
        public override Platform Platform { get { return platform; } }
        
        private uint elf_hash(string s)
        {
            int i = 0;
            uint h = 0;
            while (i != s.Length)
            {
                h = (h << 4) + s[i++];
                uint g = h & 0xf0000000u;
                if (g != 0)
                    h ^= g >> 24;
                h &= ~g;
            }
            return h;
        }

        public virtual Dictionary<ADDRESS, string> getSymbols()
        {
            return m_SymTab;
        }

        /// <summary>
        /// Reads the ELF header.
        /// </summary>
        /// <returns></returns>
        private Elf32_Ehdr ReadElfHeaderStart()
        {
            var rdr = new EndianImageReader(RawImage, 0);
            var h = new Elf32_Ehdr();

            h.e_ident = rdr.ReadBeUInt32();
            
            h.e_class = rdr.ReadByte();
            h.endianness = rdr.ReadByte();
            h.version = rdr.ReadByte();
            h.osAbi = rdr.ReadByte();

            rdr.Seek(8);             // 8 bytes of padding.

            // Now that we know the endianness, read the remaining fields in endian mode.
            rdr = CreateImageReader(h.endianness, rdr.Offset);
            h.e_type = rdr.ReadInt16();
            h.e_machine = rdr.ReadInt16();
            h.e_version = rdr.ReadInt32();
            h.e_entry = rdr.ReadUInt32();
            h.e_phoff = rdr.ReadUInt32();
            h.e_shoff = rdr.ReadUInt32();
            h.e_flags = rdr.ReadInt32();
            h.e_ehsize = rdr.ReadInt16();
            h.e_phentsize = rdr.ReadInt16();
            h.e_phnum = rdr.ReadInt16();
            h.e_shentsize = rdr.ReadInt16();
            h.e_shnum = rdr.ReadInt16();
            h.e_shstrndx = rdr.ReadInt16();

            Dump("e_type: {0}", h.e_type);
            Dump("e_machine: {0}", (MachineType) h.e_machine);
            Dump("e_version: {0}", h.e_version);
            Dump("e_entry: {0:X}", h.e_entry);
            Dump("e_phoff: {0:X}", h.e_phoff);
            Dump("e_shoff: {0:X}", h.e_shoff);
            Dump("e_flags: {0:X}", h.e_flags);
            Dump("e_ehsize: {0}", h.e_ehsize);
            Dump("e_phentsize: {0}", h.e_phentsize);
            Dump("e_phnum: {0}", h.e_phnum);
            Dump("e_shentsize: {0}", h.e_shentsize);
            Dump("e_shnum: {0}", h.e_shnum);
            Dump("e_shstrndx: {0}", h.e_shstrndx);
            
            return h;
        }

        private EndianImageReader CreateImageReader(uint offset)
        {
            return CreateImageReader(pHeader.endianness, offset);
        }

        private EndianImageReader CreateImageReader(int endianness, uint offset)
        {
            switch (endianness)
            {
            case 1: return new LeImageReader(RawImage, offset);
            case 2: return new BeImageReader(RawImage, offset);
            default: throw new NotSupportedException(string.Format("Unknown endianness {0}.", endianness));
            }
        }

        private string ReadAsciiString(uint offset)
        {
            int o = (int)offset;
            var bytes = RawImage;
            int end = o;
            for (; bytes[end] != 0; ++end)
                ;
            return Encoding.ASCII.GetString(bytes, o, end - o);
        }

        public override ProgramImage Load(Address addrLoad)
        {
            int i;

            m_lImageSize = RawImage.Length;

            m_pImage = RawImage;
            pHeader = ReadElfHeaderStart();
            arch = GetProcessorArchitecture();
            platform = GetPlatform();

            if (pHeader.e_ident != ELF_MAGIC)   
                throw new BadImageFormatException("Incorrect ELF header.");

            if (pHeader.e_phoff != 0)
                m_pPhdrs = LoadProgramHeaders(pHeader.e_phnum, pHeader.e_phoff);

            if (pHeader.e_shoff != 0)
                m_pShdrs = LoadSectionHeaders(pHeader.e_shnum, pHeader.e_shoff);

            // Set up section header string table pointer
            if (pHeader.e_shstrndx != 0)
                stringTableOffset = m_pShdrs[pHeader.e_shstrndx].sh_offset;

            i = 1; // counter - # sects. Start @ 1, total m_iNumSections

            // Number of sections
            m_iNumSections = pHeader.e_shnum;

            // Allocate room for all the Elf sections (including the silly first one)
            m_pSections = new SectionInfo[m_iNumSections];

            // Set up the m_sh_link and m_sh_info arrays
            m_sh_link = new int[m_iNumSections];
            m_sh_info = new int[m_iNumSections];

            // Number of elf sections
            bool bGotCode = false; // True when have seen a code sect

            Address arbitaryLoadAddr = Address.Ptr32(addrLoad.Linear);
            var rdr = CreateImageReader(pHeader.e_shoff);
            for (i = 0; i < m_iNumSections; i++)
            {
                var pShdr = m_pShdrs[i];
                string pName = ReadAsciiString(m_pShdrs[pHeader.e_shstrndx].sh_offset + pShdr.sh_name);
                var sect = new SectionInfo();
                m_pSections[i] = sect;
                m_pSections[i].pSectionName = pName;
                var off = pShdr.sh_offset;
                if (pShdr.sh_offset != 0)
                    sect.uHostAddr = off;
                sect.uNativeAddr = pShdr.sh_addr;
                sect.uSectionSize = pShdr.sh_size;
                if (sect.uNativeAddr == 0 && pName.StartsWith(".rel"))
                {
                    int align = pShdr.sh_addralign;
                    if (align > 1)
                    {
                        if ((arbitaryLoadAddr.Linear % align) != 0)
                            arbitaryLoadAddr += (int)(align - (arbitaryLoadAddr.Linear % align));
                    }
                    sect.uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += sect.uSectionSize;
                }
                sect.uType = pShdr.sh_type;
                m_sh_link[i] = pShdr.sh_link;
                m_sh_info[i] = pShdr.sh_info;
                sect.uSectionEntrySize = pShdr.sh_entsize;
                if (sect.uNativeAddr + sect.uSectionSize > next_extern)
                    first_extern = next_extern = sect.uNativeAddr + sect.uSectionSize;
                if ((pShdr.sh_flags & SectionFlags.SHF_WRITE) == 0)
                    sect.IsReadOnly = true;
                // Can't use the SHF_ALLOC bit to determine bss section; the bss section has SHF_ALLOC but also SHT_NOBITS.
                // (But many other sections, such as .comment, also have SHT_NOBITS). So for now, just use the name
                //      if ((elfRead4(&pShdr.sh_flags) & SHF_ALLOC) == 0)
                if (pName == ".bss")
                    sect.IsBss = true;
                if ((pShdr.sh_flags & SectionFlags.SHF_EXECINSTR) != 0)
                {
                    sect.IsCode = true;
                    bGotCode = true; // We've got to a code section
                }
                // Deciding what is data and what is not is actually quite tricky but important.
                // For example, it's crucial to flag the .exception_ranges section as data, otherwise there is a "hole" in the
                // allocation map, that means that there is more than one "delta" from a read-only section to a page, and in the
                // end using -C results in a file that looks OK but when run just says "Killed".
                // So we use the Elf designations; it seems that ALLOC.!EXEC -> data
                // But we don't want sections before the .text section, like .interp, .hash, etc etc. Hence bGotCode.
                // NOTE: this ASSUMES that sections appear in a sensible order in the input binary file:
                // junk, code, rodata, data, bss
                if (bGotCode &&
                    (pShdr.sh_flags & (SectionFlags.SHF_EXECINSTR | SectionFlags.SHF_ALLOC)) == SectionFlags.SHF_ALLOC &&
                       pShdr.sh_type != SectionType.SHT_NOBITS)
                    sect.bData = true;
                
                sect.Dump();
                Debug.WriteLine("");

            } // for each section

            // assign arbitary addresses to .rel.* sections too
            for (i = 0; i < m_iNumSections; i++)
            {
                if (m_pSections[i].uNativeAddr == 0 && m_pSections[i].pSectionName.StartsWith(".rel"))
                {
                    m_pSections[i].uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += m_pSections[i].uSectionSize;
                }
            }

            // Add symbol info. Note that some symbols will be in the main table only, and others in the dynamic table only.
            // So the best idea is to add symbols for all sections of the appropriate type
            for (i = 1; i < m_iNumSections; ++i)
            {
                var uType = m_pSections[i].uType;
                if (uType == SectionType.SHT_SYMTAB || uType == SectionType.SHT_DYNSYM)
                    AddSyms(i);
            }

            // Save the relocation to symbol table info
            SectionInfo pRel = GetSectionInfoByName(".rela.text");
            if (pRel != null)
            {
                m_bAddend = true; // Remember its a relA table
                m_pReloc =   (Elf32_Rel*)pRel.uHostAddr; // Save pointer to reloc table
            }
            else
            {
                m_bAddend = false;
                pRel = GetSectionInfoByName(".rel.text");
                if (pRel != null)
                {
                    SetRelocInfo(pRel);
                    m_pReloc = (Elf32_Rel*)pRel.uHostAddr; // Save pointer to reloc table
                }
            }

            // Find the PLT limits. Required for IsDynamicLinkedProc(), e.g.
            SectionInfo pPlt = GetSectionInfoByName(".plt");
            if (pPlt != null)
            {
                m_uPltMin = pPlt.uNativeAddr;
                m_uPltMax = pPlt.uNativeAddr + pPlt.uSectionSize;
            }
            return new ProgramImage(addrLoad, new byte[arbitaryLoadAddr - addrLoad]);
        }


        private Elf32_Shdr[] LoadSectionHeaders(int count, uint imageOffset)
        {
            var rdr = CreateImageReader(imageOffset);
            var headers =  new Elf32_Shdr[count];
            for (int i = 0; i < count; ++i)
            {
                Elf32_Shdr pShdr = new Elf32_Shdr();
                pShdr.sh_name = rdr.ReadUInt32();
                pShdr.sh_type = (SectionType) rdr.ReadUInt32();
                pShdr.sh_flags = (SectionFlags) rdr.ReadUInt32();
                pShdr.sh_addr = rdr.ReadUInt32();
                pShdr.sh_offset = rdr.ReadUInt32();
                pShdr.sh_size = rdr.ReadUInt32();
                pShdr.sh_link = rdr.ReadInt32();
                pShdr.sh_info = rdr.ReadInt32();
                pShdr.sh_addralign = rdr.ReadInt32();
                pShdr.sh_entsize = rdr.ReadUInt32();
                headers[i] = pShdr;
                //Debug.Print("Section {0}", i);
                //Dump("sh_name: {0}", pShdr.sh_name);
                //Dump("sh_type: {0}", pShdr.sh_type);
                //Dump("sh_flags: {0}", pShdr.sh_flags);
                //Dump("sh_addr: {0:X}", pShdr.sh_addr);
                //Dump("sh_offset: {0}", pShdr.sh_offset);
                //Dump("sh_size: {0}", pShdr.sh_size);
                //Dump("sh_link: {0}", pShdr.sh_link);
                //Dump("sh_info: {0}", pShdr.sh_info);
                //Dump("sh_addralign: {0}", pShdr.sh_addralign);
                //Dump("sh_entsize: {0}", pShdr.sh_entsize);
            }
            return headers;
        }

        enum ProgramHeaderType
        {
            PT_NULL = 0,
            PT_LOAD = 1,
            PT_DYNAMIC = 2,
            PT_INTERP = 3,
            PT_NOTE = 4,
            PT_SHLIB = 5,
            PT_PHDR = 6,
            PT_TLS = 7,
            PT_LOOS = 0x60000000,
            PT_HIOS = 0x6fffffff,
            PT_LOPROC = 0x70000000,
            PT_HIPROC = 0x7fffffff,
        }

        private Elf32_Phdr [] LoadProgramHeaders(int headerCount, uint imageOffset)
        {
            var rdr = CreateImageReader(imageOffset);
            var headers = new Elf32_Phdr[headerCount];
            for (int i = 0; i < headerCount; ++i)
            {
                var phdr = new Elf32_Phdr();
                phdr.p_type = rdr.ReadUInt32(); /* entry Type */
                phdr.p_offset = rdr.ReadUInt32(); /* file offset */
                phdr.p_vaddr = rdr.ReadUInt32(); /* virtual address */
                phdr.p_paddr = rdr.ReadUInt32(); /* physical address */
                phdr.p_filesz = rdr.ReadUInt32(); /* file size */
                phdr.p_memsz = rdr.ReadUInt32(); /* memory size */
                phdr.p_flags = rdr.ReadUInt32(); /* entry flags */
                phdr.p_align = rdr.ReadUInt32(); /* memory/file alignment */
                headers[i] = phdr;

                Debug.Print("Program header {0}", i);
                Dump("p_type: {0}", (ProgramHeaderType) phdr.p_type);
                Dump("p_offset: {0:X}", phdr.p_offset);
                Dump("p_vaddr: {0:X}", phdr.p_vaddr);
                Dump("p_paddr: {0:X}", phdr.p_paddr);
                Dump("p_filesz: {0:X}", phdr.p_filesz);
                Dump("p_memsz: {0:X}", phdr.p_memsz);
                Dump("p_flags: {0}", phdr.p_flags);
                Dump("p_align: {0}", phdr.p_align);
            }
            return headers;
        }

        [Conditional("DEBUG")]
        public void Dump(string caption, object value)
        {
            Debug.Print(caption, value);
        }

        private SectionInfo GetSectionInfoByName(string name)
        {
            return m_pSections.FirstOrDefault(sec => sec.pSectionName == name);
        }

        // Like a replacement for elf_strptr()
 

        // Search the .rel[a].plt section for an entry with symbol table index i.
        // If found, return the native address of the associated PLT entry.
        // A linear search will be needed. However, starting at offset i and searching backwards with wraparound should
        // typically minimise the number of entries to search
        ADDRESS findRelPltOffset(uint i, ADDRESS addrRelPlt, uint sizeRelPlt, uint numRelPlt, ADDRESS addrPlt)
        {
            uint first = i;
            if (first >= numRelPlt)
                first = numRelPlt - 1;
            uint curr = first;
            do
            {
                // Each entry is sizeRelPlt bytes, and will contain the offset, then the info (addend optionally follows)
                var pEntry = CreateImageReader(addrRelPlt + (curr * sizeRelPlt));
                pEntry.ReadInt32();
                int entry = pEntry.ReadInt32(); // Read pEntry[1]
                int sym = entry >> 8; // The symbol index is in the top 24 bits (Elf32 only)
                if (sym == i)
                {
                    // Found! Now we want the native address of the associated PLT entry.
                    // For now, assume a size of 0x10 for each PLT entry, and assume that each entry in the .rel.plt section
                    // corresponds exactly to an entry in the .plt (except there is one dummy .plt entry)
                    return addrPlt + 0x10u * (curr + 1);
                }
                if (--curr < 0)
                    curr = numRelPlt - 1;
            } while (curr != first); // Will eventually wrap around to first if not present
            return 0; // Exit if this happens
        }

 

        List<ADDRESS> GetExportedAddresses(bool funcsOnly)
        {
            List<ADDRESS> exported = new List<ADDRESS>();
#if NYI
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = m_pSections[i].uType;
                if (uType == SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return exported;

            int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            SectionInfo pSect = &m_pSections[secIndex];
            // Calc number of symbols
            int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                ADDRESS val = (ADDRESS)elfRead4((int*)&m_pSym[i].st_value);
                int name = elfRead4(&m_pSym[i].st_name);
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str.Remove(pos);
                if (ELF32_ST_BIND(m_pSym[i].st_info) == STB_GLOBAL || ELF32_ST_BIND(m_pSym[i].st_info) == STB_WEAK)
                {
                    if (funcsOnly == false || ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC)
                    {
                        if (e_type == E_REL)
                        {
                            int nsec = elfRead2(&m_pSym[i].st_shndx);
                            if (nsec >= 0 && nsec < m_iNumSections)
                                val += GetSectionInfo(nsec)->uNativeAddr;
                        }
                        exported.push_back(val);
                    }
                }
            }
#endif
            return exported;

        }


        // FIXME: this function is way off the rails. It seems to always overwrite the relocation entry with the 32 bit value
        // from the symbol table. Totally invalid for SPARC, and most X86 relocations!
        // So currently not called
        private void AddRelocsAsSyms(int relSecIdx)
        {
#if NYI
            SectionInfo pSect = m_pSections[relSecIdx];
            if (pSect == null) return;
            // Calc number of relocations
            int nRelocs = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pReloc = (Elf32_Rel)pSect.uHostAddr; // Pointer to symbols
            int symSecIdx = m_sh_link[relSecIdx];
            int strSecIdx = m_sh_link[symSecIdx];

            // Index 0 is a dummy entry
            for (int i = 1; i < nRelocs; i++)
            {
                ADDRESS val = (ADDRESS)elfRead4((int*)&m_pReloc[i].r_offset);
                int symIndex = elfRead4(&m_pReloc[i].r_info) >> 8;
                int flags = elfRead4(&m_pReloc[i].r_info);
                if ((flags & 0xFF) == R_386_32)
                {
                    // Lookup the value of the symbol table entry
                    ADDRESS a = elfRead4((int*)&m_pSym[symIndex].st_value);
                    if (m_pSym[symIndex].st_info & STT_SECTION)
                        a = GetSectionInfo(elfRead2(&m_pSym[symIndex].st_shndx))->uNativeAddr;
                    // Overwrite the relocation value... ?
                    writeNative4(val, a);
                    continue;
                }
                if ((flags & R_386_PC32) == 0)
                    continue;
                if (symIndex == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strSecIdx, elfRead4(&m_pSym[symIndex].st_name));
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                uint pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Linear search!
                foreach (var it in m_SymTab)
                    if (it.Value == str)
                        break;
                if (!m_SymTab.ContainsValue(str))
                {
                // Add new extern
                    m_SymTab[next_extern] = str;
                    it = m_SymTab.find(next_extern);
                    next_extern += 4;
                }
                writeNative4(val, (*it).first - val - 4);
            }
            return;
#endif
        }

        // Note: this function overrides a simple "return 0" function in the base class (i.e. BinaryFile::SymbolByAddress())
        string SymbolByAddress(ADDRESS dwAddr)
        {
            string sym;
            if (m_SymTab.TryGetValue(dwAddr, out sym))
                return sym;
            return null;
        }

        bool ValueByName(string pName, ref SymValue pVal, bool bNoTypeOK /* = false */)
        {
            throw new NotImplementedException();
#if NYI
            int hash, numBucket, numChain, y;
            int* pBuckets;
            int* pChains; // For symbol table work
            int found;
            int [] pHash; // Pointer to hash table
            Elf32_Sym* pSym; // Pointer to the symbol table
            int iStr; // Section index of the string table
            PSectionInfo pSect;

            pSect = GetSectionInfoByName(".dynsym");
            if (pSect == 0)
            {
                // We have a file with no .dynsym section, and hence no .hash section (from my understanding - MVE).
                // It seems that the only alternative is to linearly search the symbol tables.
                // This must be one of the big reasons that linking is so slow! (at least, for statically linked files)
                // Note MVE: We can't use m_SymTab because we may need the size
                return SearchValueByName(pName, pVal);
            }
            pSym = (Elf32_Sym)pSect.uHostAddr;
            if (pSym == null) return false;
            pSect = GetSectionInfoByName(".hash");
            if (pSect == 0) return false;
            pHash = (int[])pSect.uHostAddr;
            iStr = GetSectionIndexByName(".dynstr");

            // First organise the hash table
            numBucket = elfRead4(&pHash[0]);
            numChain = elfRead4(&pHash[1]);
            pBuckets = &pHash[2];
            pChains = &pBuckets[numBucket];

            // Hash the symbol
            hash = elf_hash(pName) % numBucket;
            y = elfRead4(&pBuckets[hash]); // Look it up in the bucket list
            // Beware of symbol tables with 0 in the buckets, e.g. libstdc++.
            // In that case, set found to false.
            found = (y != 0);
            if (y)
            {
                while (strcmp(pName, GetStrPtr(iStr, elfRead4(&pSym[y].st_name))) != 0)
                {
                    y = elfRead4(&pChains[y]);
                    if (y == 0)
                    {
                        found = false;
                        break;
                    }
                }
            }
            // Beware of symbols with STT_NOTYPE, e.g. "open" in libstdc++ !
            // But sometimes "main" has the STT_NOTYPE attribute, so if bNoTypeOK is passed as true, return true
            if (found && (bNoTypeOK || (ELF32_ST_TYPE(pSym[y].st_info) != STT_NOTYPE)))
            {
                pVal.uSymAddr = elfRead4((int*)&pSym[y].st_value);
                int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
                if (e_type == E_REL)
                {
                    int nsec = elfRead2(&pSym[y].st_shndx);
                    if (nsec >= 0 && nsec < m_iNumSections)
                        pVal.uSymAddr += GetSectionInfo(nsec)->uNativeAddr;
                }
                pVal.iSymSize = elfRead4(&pSym[y].st_size);
                return true;
            }
            else
            {
                // We may as well do a linear search of the main symbol table. Some symbols (e.g. init_dummy) are
                // in the main symbol table, but not in the hash table
                return SearchValueByName(pName, pVal);
            }
#endif
        }

        // Lookup the symbol table using linear searching. See comments above for why this appears to be needed.
        bool SearchValueByName(string pName, SymValue pVal, string pSectName, string pStrName)
        {
#if NYI
            // Note: this assumes .symtab. Many files don't have this section!!!
            SectionInfo pSect, pStrSect;

            pSect = GetSectionInfoByName(pSectName);
            if (pSect == 0) return false;
            pStrSect = GetSectionInfoByName(pStrName);
            if (pStrSect == 0) return false;
            string pStr = (string)pStrSect.uHostAddr;
            // Find number of symbols
            int n = pSect.uSectionSize / pSect.uSectionEntrySize;
            Elf32_Sym* pSym = (Elf32_Sym*)pSect.uHostAddr;
            // Search all the symbols. It may be possible to start later than index 0
            for (int i = 0; i < n; i++)
            {
                int idx = elfRead4(&pSym[i].st_name);
                if (strcmp(pName, pStr + idx) == 0)
                {
                    // We have found the symbol
                    pVal.uSymAddr = elfRead4((int*)&pSym[i].st_value);
                    int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
                    if (e_type == E_REL)
                    {
                        int nsec = elfRead2(&pSym[i].st_shndx);
                        if (nsec >= 0 && nsec < m_iNumSections)
                            pVal.uSymAddr += GetSectionInfo(nsec)->uNativeAddr;
                    }
                    pVal.iSymSize = elfRead4(&pSym[i].st_size);
                    return true;
                }
            }
#endif
            return false; // Not found (this table)
        }

        // Search for the given symbol. First search .symtab (if present); if not found or the table has been stripped,
        // search .dynstr

        bool SearchValueByName(string pName, SymValue pVal)
        {
            if (SearchValueByName(pName, pVal, ".symtab", ".strtab"))
                return true;
            return SearchValueByName(pName, pVal, ".dynsym", ".dynstr");
        }

        ADDRESS GetAddressByName(string pName,
            bool bNoTypeOK /* = false */)
        {
            var Val = new SymValue ();
            bool bSuccess = ValueByName(pName, ref Val, bNoTypeOK);
            if (bSuccess)
            {
                m_iLastSize = Val.iSymSize;
                m_uLastAddr = Val.uSymAddr;
                return Val.uSymAddr;
            }
            else return NO_ADDRESS;
        }

        int GetSizeByName(string pName, bool bNoTypeOK /* = false */)
        {
            SymValue Val = new SymValue();
            bool bSuccess = ValueByName(pName, ref Val, bNoTypeOK);
            if (bSuccess)
            {
                m_iLastSize = Val.iSymSize;
                m_uLastAddr = Val.uSymAddr;
                return Val.iSymSize;
            }
            else return 0;
        }

        // Guess the size of a function by finding the next symbol after it, and subtracting the distance.
        // This function is NOT efficient; it has to compare the closeness of ALL symbols in the symbol table
        int GetDistanceByName(string sName, string pSectName)
        {
#if NYI
            int size = GetSizeByName(sName);
            if (size) return size; // No need to guess!
            // No need to guess, but if there are fillers, then subtracting labels will give a better answer for coverage
            // purposes. For example, switch_cc. But some programs (e.g. switch_ps) have the switch tables between the
            // end of _start and main! So we are better off overall not trying to guess the size of _start
            uint value = GetAddressByName(sName);
            if (value == 0) return 0; // Symbol doesn't even exist!

            SectionInfo pSect;
            pSect = GetSectionInfoByName(pSectName);
            if (pSect == 0) return 0;
            // Find number of symbols
            int n = pSect.uSectionSize / pSect.uSectionEntrySize;
            Elf32_Sym* pSym = (Elf32_Sym*)pSect.uHostAddr;
            // Search all the symbols. It may be possible to start later than index 0
            uint closest = 0xFFFFFFFF;
            int idx = -1;
            for (int i = 0; i < n; i++)
            {
                if ((pSym[i].st_value > value) && (pSym[i].st_value < closest))
                {
                    idx = i;
                    closest = pSym[i].st_value;
                }
            }
            if (idx == -1) return 0;
            // Do some checks on the symbol's value; it might be at the end of the .text section
            pSect = GetSectionInfoByName(".text");
            ADDRESS low = pSect.uNativeAddr;
            ADDRESS hi = low + pSect.uSectionSize;
            if ((value >= low) && (value < hi))
            {
                // Our symbol is in the .text section. Put a ceiling of the end of the section on closest.
                if (closest > hi) closest = hi;
            }
            return closest - value;
#endif
            return 0;
        }

        int GetDistanceByName(string sName)
        {
            int val = GetDistanceByName(sName, ".symtab");
            if (val != 0) return val;
            return GetDistanceByName(sName, ".dynsym");
        }

        bool IsDynamicLinkedProc(ADDRESS uNative)
        {
            if (uNative > unchecked((uint)-1024) && uNative != ~0U)
                return true; // Say yes for fake library functions
            if (first_extern <= uNative && uNative < next_extern)
                return true; // Yes for externs (not currently used)
            if (m_uPltMin == 0) return false;
            return (uNative >= m_uPltMin) && (uNative < m_uPltMax); // Yes if a call to the PLT (false otherwise)
        }


        //
        // GetEntryPoints()
        // Returns a list of pointers to SectionInfo structs representing entry points to the program
        // Item 0 is the main() function; items 1 and 2 are .init and .fini
        //

        List<SectionInfo> GetEntryPoints(string pEntry /* = "main" */)
        {
            SectionInfo pSect = GetSectionInfoByName(".text");
            ADDRESS uMain = GetAddressByName(pEntry, true);
            ADDRESS delta = uMain - pSect.uNativeAddr;
            pSect.uNativeAddr += delta;
            pSect.uHostAddr += delta;
            // Adjust uSectionSize so uNativeAddr + uSectionSize still is end of sect
            pSect.uSectionSize -= delta;
            m_EntryPoint.Add(pSect);
            // .init and .fini sections
            pSect = GetSectionInfoByName(".init");
            m_EntryPoint.Add(pSect);
            pSect = GetSectionInfoByName(".fini");
            m_EntryPoint.Add(pSect);
            return m_EntryPoint;
        }


        //
        // GetMainEntryPoint()
        // Returns the entry point to main (this should be a label in elf binaries generated by compilers).
        //

        ADDRESS GetMainEntryPoint()
        {
            return GetAddressByName("main", true);
        }

        ADDRESS GetEntryPoint()
        {
            return pHeader.e_entry;
        }

        // FIXME: the below assumes a fixed delta
        ADDRESS NativeToHostAddress(ADDRESS uNative)
        {
            if (m_iNumSections == 0) return 0;
            return m_pSections[1].uHostAddr - m_pSections[1].uNativeAddr + uNative;
        }

        ADDRESS GetRelocatedAddress(ADDRESS uNative)
        {
            // Not implemented yet. But we need the function to make it all link
            return 0;
        }

        IProcessorArchitecture GetProcessorArchitecture()
        {
            switch ((MachineType) pHeader.e_machine)
            {
            case MachineType.EM_386: return new X86ArchitectureFlat32(ProcessorMode.ProtectedFlat);
            case MachineType.EM_68K: return new M68kArchitecture("m68k");
            case MachineType.EM_SPARC:
            case MachineType.EM_SPARC32PLUS: return new SparcArchitecture();
            case MachineType.EM_PA_RISC:
            case MachineType.EM_PPC:
            case MachineType.EM_MIPS:
            case MachineType.EM_X86_64:
                throw new NotSupportedException(string.Format("The machine {0} is not supported yet.", (MachineType)pHeader.e_machine));
            }
            // An unknown machine type
            throw new NotSupportedException(string.Format("The machine with ELF machine ID {0:X} is not supported.", pHeader.e_machine));
        }

        bool isLibrary()
        {
            int type = pHeader.e_type;
            return (type == ET_DYN);
        }



 

        /*==============================================================================
         * FUNCTION:	ElfBinaryFile::GetDynamicGlobalMap
         * OVERVIEW:	Get a map from ADDRESS to string . This map contains the native addresses
         *					and symbolic names of global data items (if any) which are shared with dynamically
         *					linked libraries.
         *					Example: __iob (basis for stdout). The ADDRESS is the native address of a pointer
         *					to the real dynamic data object.
         * NOTE:		The caller should delete the returned map.
         * PARAMETERS:	None
         * RETURNS:		Pointer to a new map with the info, or 0 if none
         *============================================================================*/
        Dictionary<ADDRESS, string> GetDynamicGlobalMap()
        {
            var ret = new Dictionary<ADDRESS, string>();
#if NYI
            SectionInfo* pSect = GetSectionInfoByName(".rel.bss");
            if (pSect == 0)
                pSect = GetSectionInfoByName(".rela.bss");
            if (pSect == 0)
            {
                // This could easily mean that this file has no dynamic globals, and
                // that is fine.
                return ret;
            }
            int numEnt = pSect.uSectionSize / pSect.uSectionEntrySize;
            SectionInfo sym = GetSectionInfoByName(".dynsym");
            if (sym == 0)
            {
                Console.WriteLine("Could not find section .dynsym in source binary file");
                return ret;
            }
            Elf32_Sym pSym = (Elf32_Sym*)sym.uHostAddr;
            int idxStr = GetSectionIndexByName(".dynstr");
            if (idxStr == -1)
            {
                Console.WriteLine("Could not find section .dynstr in source binary file");
                return ret;
            }

            uint p = pSect.uHostAddr;
            for (int i = 0; i < numEnt; i++)
            {
                // The ugly p[1] below is because it p might point to an Elf32_Rela struct, or an Elf32_Rel struct
                int sym = ELF32_R_SYM(((int*)p)[1]);
                int name = pSym[sym].st_name; // Index into string table
                string s = GetStrPtr(idxStr, name);
                ADDRESS val = ((int*)p)[0];
                (*ret)[val] = s; // Add the (val, s) mapping to ret
                p += pSect.uSectionEntrySize;
            }
#endif
            return ret;
        }


        bool IsRelocationAt(ADDRESS uNative)
        {
#if NYI
            //int nextFakeLibAddr = -2;			// See R_386_PC32 below; -1 sometimes used for main
            if (m_pImage == 0) return false; // No file loaded
            int machine = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_machine);
            int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            switch (machine)
            {
            case EM_SPARC:
                break; // Not implemented yet
            case EM_386:
                {
                    for (int i = 1; i < m_iNumSections; ++i)
                    {
                        SectionInfo ps = &m_pSections[i];
                        if (ps.uType == SHT_REL)
                        {
                            // A section such as .rel.dyn or .rel.plt (without an addend field).
                            // Each entry has 2 words: r_offet and r_info. The r_offset is just the offset from the beginning
                            // of the section (section given by the section header's sh_info) to the word to be modified.
                            // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                            // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                            // the section header's sh_link field.
                            int* pReloc = (int*)ps.uHostAddr;
                            uint size = ps.uSectionSize;
                            // NOTE: the r_offset is different for .o files (E_REL in the e_type header field) than for exe's
                            // and shared objects!
                            ADDRESS destNatOrigin = 0, destHostOrigin;
                            if (e_type == E_REL)
                            {
                                int destSection = m_sh_info[i];
                                destNatOrigin = m_pSections[destSection].uNativeAddr;
                                destHostOrigin = m_pSections[destSection].uHostAddr;
                            }
                            //int symSection = m_sh_link[i];			// Section index for the associated symbol table
                            //int strSection = m_sh_link[symSection];	// Section index for the string section assoc with this
                            //string pStrSection = (char*)m_pSections[strSection].uHostAddr;
                            //Elf32_Sym* symOrigin = (Elf32_Sym*) m_pSections[symSection].uHostAddr;
                            for (uint u = 0; u < size; u += 2 * sizeof(uint))
                            {
                                uint r_offset = elfRead4(pReloc++);
                                //unsigned info	= elfRead4(pReloc);
                                pReloc++;
                                //byte relType = (byte) info;
                                //unsigned symTabIndex = info >> 8;
                                ADDRESS pRelWord; // Pointer to the word to be relocated
                                if (e_type == E_REL)
                                    pRelWord = destNatOrigin + r_offset;
                                else
                                {
                                    if (r_offset == 0) continue;
                                    SectionInfo destSec = GetSectionInfoByAddr(r_offset);
                                    pRelWord = destSec.uNativeAddr + r_offset;
                                    destNatOrigin = 0;
                                }
                                if (uNative == pRelWord)
                                    return true;
                            }
                        }
                    }
                }
            default:
                break; // Not implemented
            }
#endif
            return false;
        }

        string getFilenameSymbolFor(string sym)
        {
#if NYI
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = m_pSections[i].uType;
                if (uType == SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return null;

            //int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            SectionInfo pSect = m_pSections[secIndex];
            // Calc number of symbols
            int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            string filename;

            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                //ADDRESS val = (ADDRESS) elfRead4((int*)&m_pSym[i].st_value);
                int name = elfRead4(&m_pSym[i].st_name);
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FILE)
                {
                    filename = str;
                    continue;
                }
                if (str == sym)
                {
                    if (!string.IsNullOrEmpty(filename))
                        return filename;
                    return null;
                }
            }
#endif
            return null;
        }

        private void getFunctionSymbols(SortedList<string, SortedList<ADDRESS, string>> syms_in_file) {
#if NYI
    int i;
    int secIndex = 0;
    for (i = 1; i < m_iNumSections; ++i) {
        uint uType = m_pSections[i].uType;
        if (uType == SHT_SYMTAB) {
            secIndex = i;
            break;
        }
    }
    if (secIndex == 0) {
        Console.Error.WriteLine("no symtab section? Assuming stripped, looking for dynsym.\n");

        for (i = 1; i < m_iNumSections; ++i) {
            uint uType = m_pSections[i].uType;
            if (uType == SHT_DYNSYM) {
                secIndex = i;
                break;
            }
        }

        if (secIndex == 0) {
            Console.Error.WriteLine("no dynsyms either.. guess we're out of luck.\n");
            return;
        }
    }

    int e_type = pHeader.e_type;
    SectionInfo pSect = m_pSections[secIndex];
    // Calc number of symbols
    int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
    m_pSym = (Elf32_Sym*) pSect.uHostAddr; // Pointer to symbols
    int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

    string filename = "unknown.c";

    // Index 0 is a dummy entry
    for (int i = 1; i < nSyms; i++) {
        int name = elfRead4(&m_pSym[i].st_name);
        if (name == 0) /* Silly symbols with no names */ continue;
        string str = GetStrPtr(strIdx, name);
        // Hack off the "@@GLIBC_2.0" of Linux, if present
        uint pos;
        if ((pos = str.IndexOf("@@"))>= 0)
            str.erase(pos);
        if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FILE) {
            filename = str;
            continue;
        }
        if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC) {
            ADDRESS val = (ADDRESS) elfRead4((int*) & m_pSym[i].st_value);
            if (e_type == E_REL) {
                int nsec = elfRead2(&m_pSym[i].st_shndx);
                if (nsec >= 0 && nsec < m_iNumSections)
                    val += GetSectionInfo(nsec)->uNativeAddr;
            }
            if (val == 0) {
                // ignore plt for now
            } else {
                syms_in_file[filename][val] = str;
            }
        }
    }
#endif
        }


        private void dumpSymbols()
        {
            foreach (var de in m_SymTab)
                Console.Error.WriteLine("0x{0:X} {1}        ", de.Key, de.Value);
        }

        struct SymValue
        {
            public ADDRESS uSymAddr; // Symbol native address
            public int iSymSize; // Size associated with symbol
        }

        // Internal elf info

        public enum SectionType
        {
            SHT_NULL = 0,
            SHT_PROGBITS = 1,
            SHT_SYMTAB = 2,
            SHT_STRTAB = 3,
            SHT_RELA = 4,
            SHT_HASH = 5,
            SHT_DYNAMIC = 6,
            SHT_NOTE = 7,
            SHT_NOBITS = 8,
            SHT_REL = 9,
            SHT_SHLIB = 10,
            SHT_DYNSYM = 11,
            SHT_INIT_ARRAY = 14,
            SHT_FINI_ARRAY = 15,
            SHT_PREINIT_ARRAY = 16,
            SHT_GROUP = 17,
            SHT_SYMTAB_SHNDX = 18,
        }

        [Flags]
        public enum SectionFlags : uint
        {
            SHF_WRITE = 0x1,
            SHF_ALLOC = 0x2,
            SHF_EXECINSTR = 0x4,
            SHF_MERGE = 0x10,
            SHF_STRINGS = 0x20,
            SHF_INFO_LINK = 0x40,
            SHF_LINK_ORDER = 0x80,
            SHF_OS_NONCONFORMING = 0x100,
            SHF_GROUP = 0x200,
            SHF_TLS = 0x400,
            SHF_MASKOS = 0x0ff00000,
            SHF_MASKPROC = 0xf0000000u,
        }

        const int ET_NONE = 0;// No file type
        const int ET_REL = 1;// Relocatable file
        const int ET_EXEC = 2;// Executable file
        const int ET_DYN = 3; // Shared object file
        const int ET_CORE = 4; // Core dump

        const int R_386_32 = 1;
        const int R_386_PC32 = 2;

        // Program header

        public class Elf32_Phdr
        {
            public uint p_type; /* entry Type */
            public uint p_offset; /* file offset */
            public uint p_vaddr; /* virtual address */
            public uint p_paddr; /* physical address */
            public uint p_filesz; /* file size */
            public uint p_memsz; /* memory size */
            public uint p_flags; /* entry flags */
            public uint p_align; /* memory/file alignment */
        }


        // Tag values
        const int DT_NULL = 0;		// Last entry in list
        const int DT_STRTAB = 5;		// String table
        const int DT_NEEDED = 1;		// A needed link-type object

        const int E_REL = 1;		// Relocatable file type

        const uint NO_ADDRESS = ~0u;

    }

    public class SymTab
    {
    }
#endif

}
