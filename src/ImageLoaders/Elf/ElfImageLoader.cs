using Decompiler.Arch.X86;
using Decompiler.Arch.Sparc;
using Decompiler.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    using ADDRESS = UInt32;
    using StrIntMap = Dictionary<string, int>;
    using RelocMap = Dictionary<ADDRESS, string>;

    public class ElfLoader : ImageLoader
    {
        struct SymValue
        {
            ADDRESS uSymAddr; // Symbol native address
            int iSymSize; // Size associated with symbol
        }

        // Internal elf info

        public class Elf32_Ehdr
        {
            public uint e_ident;            // [4];
            public byte e_class;
            public byte endianness;
            public byte version;
            public byte osAbi;
            public byte[] pad;  // [8];
            public short e_type;
            public short e_machine;
            public int e_version;
            public int e_entry;
            public uint e_phoff;
            public uint e_shoff;
            public int e_flags;
            public short e_ehsize;
            public short e_phentsize;
            public short e_phnum;
            public short e_shentsize;
            public short e_shnum;
            public short e_shstrndx;
        }

        const int EM_SPARC = 2;			// Sun SPARC
        const int EM_386 = 3;			// Intel 80386 or higher
        const int EM_68K = 4;			// Motorola 68000
        const int EM_MIPS = 8;			// MIPS
        const int EM_PA_RISC = 15;			// HP PA-RISC
        const int EM_SPARC32PLUS = 18;			// Sun SPARC 32+
        const int EM_PPC = 20;			// PowerPC
        const int EM_X86_64 = 62;
        const int EM_ST20 = 0xA8;			// ST20 (made up... there is no official value?)

        const int ET_DYN = 3;		// Elf type (dynamic library)

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

        // Section header

        public class Elf32_Shdr
        {
            public uint sh_name;
            public uint sh_type;
            public uint sh_flags;
            public uint sh_addr;
            public uint sh_offset;
            public uint sh_size;
            public int sh_link;
            public int sh_info;
            public int sh_addralign;
            public uint sh_entsize;
        };

        const int SHF_WRITE = 1;		// Writeable
        const int SHF_ALLOC = 2;		// Consumes memory in exe
        const int SHF_EXECINSTR = 4;		// Executable

        const int SHT_NOBITS = 8;		// Bss
        const int SHT_REL = 9;		// Relocation table (no addend)
        const int SHT_RELA = 4;		// Relocation table (with addend, e.g. RISC)
        const int SHT_SYMTAB = 2;		// Symbol table
        const int SHT_DYNSYM = 11;		// Dynamic symbol table

        public class Elf32_Sym
        {
            public int st_name;
            public uint st_value;
            public int st_size;
            public byte st_info;
            public byte st_other;
            public short st_shndx;
        }

        public class Elf32_Rel
        {
            public uint r_offset;
            public int r_info;
        }


        private int ELF32_R_SYM(int info) { return ((info) >> 8); }
        private int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        private int ELF32_ST_TYPE(int i) { return ((i) & 0xf); }
        private int ELF32_ST_INFO(int b, int t) { return (((b) << 4) + ((t) & 0xf)); }

        const int STT_NOTYPE = 0;			// Symbol table type: none
        const int STT_FUNC = 2;				// Symbol table type: function
        const int STT_SECTION = 3;
        const int STT_FILE = 4;
        const int STB_GLOBAL = 1;
        const int STB_WEAK = 2;

        public class Elf32_Dyn
        {
            public short d_tag; /* how to interpret value */
            private int val;
            public int d_val { get { return val; } set { val = value; } }
            public int d_ptr { get { return val; } set { val = value; } }
            public int d_off { get { return val; } set { val = value; } }
        }


        // Tag values
        const int DT_NULL = 0;		// Last entry in list
        const int DT_STRTAB = 5;		// String table
        const int DT_NEEDED = 1;		// A needed link-type object

        const int E_REL = 1;		// Relocatable file type

        virtual string getFilename()
        {
            return m_pFileName;
        }

        virtual Dictionary<ADDRESS, string> getSymbols()
        {
            return m_SymTab;
        }


        string m_pFileName; // Pointer to input file name

        private

            ImageReader m_fd; // File stream
        long m_lImageSize; // Size of image in bytes
        byte[] m_pImage; // Pointer to the loaded image
        Elf32_Ehdr pHeader;
        Elf32_Phdr[] m_pPhdrs; // Pointer to program headers
        Elf32_Shdr[] m_pShdrs; // Array of section header structs
        uint m_pStringTableOffset; // Pointer to the string section
        bool bigEndian; // 1 = Big Endian
        Dictionary<ADDRESS, string> m_SymTab; // Map from address to symbol name; contains symbols from the
        // various elf symbol tables, and possibly some symbols with fake
        // addresses
        SymTab m_Reloc; // Object to store the reloc syms
        Elf32_Rel m_pReloc; // Pointer to the relocation section
        Elf32_Sym [] m_pSym; // Pointer to loaded symbol section
        bool m_bAddend; // true if reloc table has addend
        ADDRESS m_uLastAddr; // Save last address looked up
        int m_iLastSize; // Size associated with that name
        ADDRESS m_uPltMin; // Min address of PLT table
        ADDRESS m_uPltMax; // Max address (1 past last) of PLT
        List<SectionInfo> m_EntryPoint; // A list of one entry point
        ADDRESS m_pImportStubs; // An array of import stubs
        ADDRESS m_uBaseAddr; // Base image virtual address
        uint m_uImageSize; // total image size (bytes)
        ADDRESS first_extern; // where the first extern will be placed
        ADDRESS next_extern; // where the next extern will be placed
        int[] m_sh_link; // pointer to array of sh_link values
        int[] m_sh_info; // pointer to array of sh_info values
        int m_iNumSections;
        SectionInfo[] m_pSections;

        public ElfLoader(IServiceProvider sp, byte[] rawImage)
            : base(sp, rawImage)
        {
            next_extern = (0);
            m_fd = null;
            m_pFileName = null;
            Init(); // Initialise all the common stuff
        }


        // Reset internal state, except for those that keep track of which member
        // we're up to
        private void Init()
        {
            m_pImage = null;
            m_pPhdrs = null; // No program headers
            m_pShdrs = null; // No section headers
            m_pStringTableOffset = 0; // No strings
            m_pReloc = null;
            m_pSym = null;
            m_uPltMin = 0; // No PLT limits
            m_uPltMax = 0;
            m_iLastSize = 0;
            m_pImportStubs = 0;
        }
#if NYU

        // Hand decompiled from sparc library function
        //extern "C" { // So we can call this with dlopen()

        //    uint elf_hash(string  o0) {
        //        int o3 = *o0;
        //        string  g1 = o0;
        //        uint o4 = 0;
        //        while (o3 != 0) {
        //            o4.Write(= 4);
        //            o3 += o4;
        //            g1++;
        //            o4 = o3 & 0xf0000000;
        //            if (o4 != 0) {
        //                int o2 = (int) ((uint) o4 >> 24);
        //                o3 = o3 ^ o2;
        //            }
        //            o4 = o3 & ~o4;
        //            o3 = *g1;
        //        }
        //        return o4;
        //    }
        //} // extern "C"

        // Return true for a good load

        private Elf32_Ehdr ReadElfHeaderStart()
        {
            var h = new Elf32_Ehdr();
            var rdr = new ImageReader(RawImage, 0);
            h.e_ident = m_fd.ReadBeUInt32();
            h.e_class = m_fd.ReadByte();
            h.endianness = m_fd.ReadByte();
            h.version = m_fd.ReadByte();
            h.osAbi = m_fd.ReadByte();
            m_fd.ReadBeInt64();             // 8 bytes of padding.

            rdr = CreateImageReader(m_fd.Offset);
            h.e_type = rdr.ReadInt16();
            h.e_machine = rdr.ReadInt16();
            h.e_version = rdr.ReadInt32();
            h.e_entry = rdr.ReadInt32();
            h.e_phoff = rdr.ReadUInt32();
            h.e_shoff = rdr.ReadUInt32();
            h.e_flags = rdr.ReadInt32();
            h.e_ehsize = rdr.ReadInt16();
            h.e_phentsize = rdr.ReadInt16();
            h.e_phnum = rdr.ReadInt16();
            h.e_shentsize = rdr.ReadInt16();
            h.e_shnum = rdr.ReadInt16();
            h.e_shstrndx = rdr.ReadInt16();

            return h;
        }

        private ImageReader CreateImageReader(uint offset)
        {
            if (bigEndian)
                return new BeImageReader(RawImage, offset);
            else
                return new LeImageReader(RawImage, offset);
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

        bool RealLoad(string sName)
        {
            int i;

            m_pFileName = sName;
            m_fd = new ImageReader(RawImage, 0);

            // Determine file size
            m_lImageSize = RawImage.Length;

            // Allocate memory to hold the file
            m_pImage = RawImage;
            pHeader = ReadElfHeaderStart();

            // Basic checks
            if (pHeader.e_ident != 0x7F454C46)          // "\x7FELF"
                throw new BadImageFormatException("Incorrect ELF header.");

            if ((pHeader.endianness != 1) && (pHeader.endianness != 2))
            {
                throw new BadImageFormatException("Unknown endianness.");
            }
            // Needed for elfRead4 to work:
            bigEndian = (pHeader.endianness - 1) != 0;

            // Set up program header pointer (in case needed)
            if (pHeader.e_phoff != 0)
                m_pPhdrs = LoadProgramHeader(pHeader.e_phoff);

            //// Set up section header pointer
            if (pHeader.e_shoff != 0)
                m_pShdrs = new Elf32_Shdr[pHeader.e_shnum];

            // Set up section header string table pointer
            // NOTE: it does not appear that endianness affects shorts.. they are always in little endian format
            // Gerard: I disagree. I need the elfRead on linux/i386
            if (pHeader.e_shstrndx != 0)
                m_pStringTableOffset = m_pShdrs[pHeader.e_shstrndx].sh_offset;

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
            Address arbitaryLoadAddr = new Address(0x08000000);
            var rdr = CreateImageReader(pHeader.e_shoff);
            for (i = 0; i < m_iNumSections; i++)
            {
                // Get section information.
                Elf32_Shdr pShdr = new Elf32_Shdr();
                pShdr.sh_name = rdr.ReadUInt32();
                pShdr.sh_type = rdr.ReadUInt32();
                pShdr.sh_flags = rdr.ReadUInt32();
                pShdr.sh_addr = rdr.ReadUInt32();
                pShdr.sh_offset = rdr.ReadUInt32();
                pShdr.sh_size = rdr.ReadUInt32();
                pShdr.sh_link = rdr.ReadInt32();
                pShdr.sh_info = rdr.ReadInt32();
                pShdr.sh_addralign = rdr.ReadInt32();
                pShdr.sh_entsize = rdr.ReadUInt32();

                string pName = ReadAsciiString(m_pShdrs[pHeader.e_shstrndx].sh_offset + pShdr.sh_name);
                m_pSections[i].pSectionName = pName;
                var off = pShdr.sh_offset;
                if (pShdr.sh_offset != 0)
                    m_pSections[i].uHostAddr = off;
                m_pSections[i].uNativeAddr = pShdr.sh_addr;
                m_pSections[i].uSectionSize = pShdr.sh_size;
                if (m_pSections[i].uNativeAddr == 0 && pName.StartsWith(".rel"))
                {
                    int align = pShdr.sh_addralign;
                    if (align > 1)
                    {
                        if ((arbitaryLoadAddr.Linear % align) != 0)
                            arbitaryLoadAddr += (int)(align - (arbitaryLoadAddr.Linear % align));
                    }
                    m_pSections[i].uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += m_pSections[i].uSectionSize;
                }
                m_pSections[i].uType = pShdr.sh_type;
                m_sh_link[i] = pShdr.sh_link;
                m_sh_info[i] = pShdr.sh_info;
                m_pSections[i].uSectionEntrySize = pShdr.sh_entsize;
                if (m_pSections[i].uNativeAddr + m_pSections[i].uSectionSize > next_extern)
                    first_extern = next_extern = m_pSections[i].uNativeAddr + m_pSections[i].uSectionSize;
                if ((pShdr.sh_flags & SHF_WRITE) == 0)
                    m_pSections[i].bReadOnly = true;
                // Can't use the SHF_ALLOC bit to determine bss section; the bss section has SHF_ALLOC but also SHT_NOBITS.
                // (But many other sections, such as .comment, also have SHT_NOBITS). So for now, just use the name
                //      if ((elfRead4(&pShdr.sh_flags) & SHF_ALLOC) == 0)
                if (pName == ".bss")
                    m_pSections[i].bBss = true;
                if ((pShdr.sh_flags & SHF_EXECINSTR) != 0)
                {
                    m_pSections[i].bCode = true;
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
                    (pShdr.sh_flags & (SHF_EXECINSTR | SHF_ALLOC)) == SHF_ALLOC &&
                       pShdr.sh_type != SHT_NOBITS)
                    m_pSections[i].bData = true;
            } // for each section

            // assign arbitary addresses to .rel.* sections too
            for (i = 0; i < m_iNumSections; i++)
                if (m_pSections[i].uNativeAddr == 0 && m_pSections[i].pSectionName.StartsWith(".rel"))
                {
                    m_pSections[i].uNativeAddr = arbitaryLoadAddr.Offset;
                    arbitaryLoadAddr += m_pSections[i].uSectionSize;
                }

            // Add symbol info. Note that some symbols will be in the main table only, and others in the dynamic table only.
            // So the best idea is to add symbols for all sections of the appropriate type
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = m_pSections[i].uType;
                if (uType == SHT_SYMTAB || uType == SHT_DYNSYM)
                    AddSyms(i);
#if NEVER	// Ick; bad logic. Done with fake library function pointers now (-2 .. -1024)
        if (uType == SHT_REL || uType == SHT_RELA)
            AddRelocsAsSyms(i);
#endif
            }

            // Save the relocation to symbol table info
            SectionInfo pRel = GetSectionInfoByName(".rela.text");
            if (pRel != null)
            {
                m_bAddend = true; // Remember its a relA table
                m_pReloc = (Elf32_Rel*)pRel.uHostAddr; // Save pointer to reloc table
                //SetRelocInfo(pRel);
            }
            else
            {
                m_bAddend = false;
                pRel = GetSectionInfoByName(".rel.text");
                if (pRel != null)
                {
                    //SetRelocInfo(pRel);
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

            // Apply relocations; important when the input program is not compiled with -fPIC
            applyRelocations();

            return true; // Success
        }

        private Elf32_Phdr[] LoadProgramHeader(uint imageOffset)
        {
            Elf32_Phdr phdr = new Elf32_Phdr();
            var rdr = CreateImageReader(imageOffset);
            phdr.p_type = rdr.ReadUInt32(); /* entry Type */
            phdr.p_offset = rdr.ReadUInt32(); /* file offset */
            phdr.p_vaddr = rdr.ReadUInt32(); /* virtual address */
            phdr.p_paddr = rdr.ReadUInt32(); /* physical address */
            phdr.p_filesz = rdr.ReadUInt32(); /* file size */
            phdr.p_memsz = rdr.ReadUInt32(); /* memory size */
            phdr.p_flags = rdr.ReadUInt32(); /* entry flags */
            phdr.p_align = rdr.ReadUInt32(); /* memory/file alignment */

            throw new NotImplementedException();
        }

        private SectionInfo GetSectionInfoByName(string name)
        {
            return m_pSections.FirstOrDefault(sec => sec.pSectionName == name);
        }

        // Clean up and unload the binary image

        private void UnLoad()
        {
            Init(); // Set all internal state to 0
        }

        // Like a replacement for elf_strptr()
        string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                throw new ArgumentException(string.Format("GetStrPtr passed index of {0}.", idx));
            }
            // Get a pointer to the start of the string table and add the offset
            return ReadAsciiString(m_pSections[idx].uHostAddr + offset);
        }

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
                int* pEntry = (int*)(addrRelPlt + (curr * sizeRelPlt));
                int entry = elfRead4(pEntry + 1); // Read pEntry[1]
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

        // Add appropriate symbols to the symbol table.  secIndex is the section index of the symbol table.
        private void AddSyms(int secIndex)
        {
            int e_type = pHeader.e_type;
            SectionInfo pSect = m_pSections[secIndex];
            // Calc number of symbols
            uint nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            m_pSym = (Elf32_Sym*)pSect.uHostAddr; // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            SectionInfo siPlt = GetSectionInfoByName(".plt");
            ADDRESS addrPlt = siPlt ? siPlt.uNativeAddr : 0;
            SectionInfo siRelPlt = GetSectionInfoByName(".rel.plt");
            uint sizeRelPlt = 8; // Size of each entry in the .rel.plt table
            if (siRelPlt == null)
            {
                siRelPlt = GetSectionInfoByName(".rela.plt");
                sizeRelPlt = 12; // Size of each entry in the .rela.plt table is 12 bytes
            }
            ADDRESS addrRelPlt = 0;
            uint numRelPlt = 0;
            if (siRelPlt != null)
            {
                addrRelPlt = siRelPlt.uHostAddr;
                numRelPlt = sizeRelPlt != 0 ? siRelPlt.uSectionSize / sizeRelPlt : 0u;
            }
            // Number of entries in the PLT:
            // int max_i_for_hack = siPlt ? (int)siPlt.uSectionSize / 0x10 : 0;
            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                ADDRESS val = (ADDRESS)elfRead4((int)m_pSym[i].st_value);
                int name = elfRead4(m_pSym[i].st_name);
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Ensure no overwriting (except functions)
                if (m_SymTab.ContainsKey(val) || ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FUNC)
                {
                    if (val == 0 && siPlt != null)
                    { //&& i < max_i_for_hack) {
                        // Special hack for gcc circa 3.3.3: (e.g. test/pentium/settest).  The value in the dynamic symbol table
                        // is zero!  I was assuming that index i in the dynamic symbol table would always correspond to index i
                        // in the .plt section, but for fedora2_true, this doesn't work. So we have to look in the .rel[a].plt
                        // section. Thanks, gcc!  Note that this hack can cause strange symbol names to appear
                        val = findRelPltOffset(i, addrRelPlt, sizeRelPlt, numRelPlt, addrPlt);
                    }
                    else if (e_type == E_REL)
                    {
                        int nsec = elfRead2(m_pSym[i].st_shndx);
                        if (nsec >= 0 && nsec < m_iNumSections)
                            val += GetSectionInfo(nsec)->uNativeAddr;
                    }

#if		ECHO_SYMS
            Console.Error.WriteLine( "Elf AddSym: about to add " + str + " to address " + std::hex + val + std::dec);
#endif
                    m_SymTab[val] = str;
                }
            }
            ADDRESS uMain = GetMainEntryPoint();
            if (uMain != NO_ADDRESS && m_SymTab.find(uMain) == m_SymTab.end())
            {
                // Ugh - main mustn't have the STT_FUNC attribute. Add it
                string sMain = "main";
                m_SymTab[uMain] = sMain;
            }
            return;
        }

        List<ADDRESS> GetExportedAddresses(bool funcsOnly)
        {
            List<ADDRESS> exported = new List<ADDRESS>();

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
            PSectionInfo pSect = &m_pSections[secIndex];
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
                    str.erase(pos);
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
            return exported;

        }


        // FIXME: this function is way off the rails. It seems to always overwrite the relocation entry with the 32 bit value
        // from the symbol table. Totally invalid for SPARC, and most X86 relocations!
        // So currently not called
        private void AddRelocsAsSyms(int relSecIdx)
        {
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
                    str.erase(pos);
                std_map<ADDRESS, string>.iterator it;
                // Linear search!
                for (it = m_SymTab.begin(); it != m_SymTab.end(); it++)
                    if ((*it).second == str)
                        break;
                // Add new extern
                if (it == m_SymTab.end())
                {
                    m_SymTab[next_extern] = str;
                    it = m_SymTab.find(next_extern);
                    next_extern += 4;
                }
                writeNative4(val, (*it).first - val - 4);
            }
            return;
        }

        // Note: this function overrides a simple "return 0" function in the base class (i.e. BinaryFile::SymbolByAddress())
        string SymbolByAddress(ADDRESS dwAddr)
        {
            string sym;
            if (m_SymTab.TryGetValue(dwAddr, out sym))
                return sym;
            return null;
        }

        bool ValueByName(string pName, SymValue pVal, bool bNoTypeOK /* = false */)
        {
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
        }

        // Lookup the symbol table using linear searching. See comments above for why this appears to be needed.
        bool SearchValueByName(string pName, SymValue pVal, string pSectName, string pStrName)
        {
            // Note: this assumes .symtab. Many files don't have this section!!!
            PSectionInfo pSect, pStrSect;

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
            SymValue Val;
            bool bSuccess = ValueByName(pName, &Val, bNoTypeOK);
            if (bSuccess)
            {
                m_iLastSize = Val.iSymSize;
                m_uLastAddr = Val.uSymAddr;
                return Val.uSymAddr;
            }
            else return ADDRESS.NO_ADDRESS;
        }

        int GetSizeByName(string pName, bool bNoTypeOK /* = false */)
        {
            SymValue Val;
            bool bSuccess = ValueByName(pName, &Val, bNoTypeOK);
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
        }

        int GetDistanceByName(string sName)
        {
            int val = GetDistanceByName(sName, ".symtab");
            if (val) return val;
            return GetDistanceByName(sName, ".dynsym");
        }

        bool IsDynamicLinkedProc(ADDRESS uNative)
        {
            if (uNative > (uint)-1024 && uNative != (uint)-1)
                return true; // Say yes for fake library functions
            if (uNative >= first_extern && uNative < next_extern)
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
            m_EntryPoint.push_back(pSect);
            // .init and .fini sections
            pSect = GetSectionInfoByName(".init");
            m_EntryPoint.push_back(pSect);
            pSect = GetSectionInfoByName(".fini");
            m_EntryPoint.push_back(pSect);
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
            return (ADDRESS)elfRead4(&((Elf32_Ehdr*)m_pImage)->e_entry);
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

        MACHINE GetMachine()
        {
            int machine = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_machine);
            if ((machine == EM_SPARC) || (machine == EM_SPARC32PLUS)) return MACHINE_SPARC;
            else if (machine == EM_386) return MACHINE_PENTIUM;
            else if (machine == EM_PA_RISC) return MACHINE_HPRISC;
            else if (machine == EM_68K) return MACHINE_PALM; // Unlikely
            else if (machine == EM_PPC) return MACHINE_PPC;
            else if (machine == EM_ST20) return MACHINE_ST20;
            else if (machine == EM_MIPS) return MACHINE_MIPS;
            else if (machine == EM_X86_64)
            {
                Console.Error.WriteLine("Error: ElfBinaryFile::GetMachine: The AMD x86-64 architecture is not supported yet");
                return (MACHINE) - 1;
            }
            // An unknown machine type
            Console.Error.WriteLine("Error: ElfBinaryFile::GetMachine: Unsupported machine type: (0x{0:X})", machine);
            Console.Error.WriteLine("(Please add a description for this type, thanks!)");
            return (MACHINE) - 1;
        }

        bool isLibrary()
        {
            int type = elfRead2(&((Elf32_Ehdr*)m_pImage)->e_type);
            return (type == ET_DYN);
        }

        List<string> getDependencyList()
        {
            std_list<string> result;
            ADDRESS stringtab = ADDRESS.NO_ADDRESS;
            SectionInfo dynsect = GetSectionInfoByName(".dynamic");
            if (dynsect == null)
                return result; /* no dynamic section = statically linked */

            Elf32_Dyn* dyn;
            for (dyn = (Elf32_Dyn)dynsect.uHostAddr; dyn.d_tag != DT_NULL; dyn++)
            {
                if (dyn.d_tag == DT_STRTAB)
                {
                    stringtab = (ADDRESS)dyn.d_un.d_ptr;
                    break;
                }
            }

            if (stringtab == ADDRESS.NO_ADDRESS) /* No string table = no names */
                return result;
            stringtab = NativeToHostAddress(stringtab);

            for (dyn = (Elf32_Dyn*)dynsect.uHostAddr; dyn.d_tag != DT_NULL; dyn++)
            {
                if (dyn.d_tag == DT_NEEDED)
                {
                    string need = (byte*)stringtab + dyn.d_un.d_val;
                    if (need != null)
                        result.push_back(need);
                }
            }
            return result;
        }

        ADDRESS getImageBase()
        {
            return m_uBaseAddr;
        }

        uint getImageSize()
        {
            return m_uImageSize;
        }

        /*==============================================================================
         * FUNCTION:	  ElfBinaryFile::GetImportStubs
         * OVERVIEW:	  Get an array of addresses of imported function stubs
         *					This function relies on the fact that the symbols are sorted by address, and that Elf PLT
         *					entries have successive addresses beginning soon after m_PltMin
         * PARAMETERS:	  numImports - reference to integer set to the number of these
         * RETURNS:		  An array of native ADDRESSes
         *============================================================================*/
        ADDRESS GetImportStubs(out int numImports)
        {
            ADDRESS a = m_uPltMin;
            int n = 0;
            std_map<ADDRESS, string>.iterator aa = m_SymTab.find(a);
            std_map<ADDRESS, string>.iterator ff = aa;
            bool delDummy = false;
            if (aa == m_SymTab.end())
            {
                // Need to insert a dummy entry at m_uPltMin
                delDummy = true;
                m_SymTab[a] = "";
                ff = m_SymTab.find(a);
                aa = ff;
                aa++;
            }
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                n++;
                a = aa.first;
                aa++;
            }
            // Allocate an array of ADDRESSESes
            m_pImportStubs = new ADDRESS[n];
            aa = ff; // Start at first
            a = aa.first;
            int i = 0;
            while ((aa != m_SymTab.end()) && (a < m_uPltMax))
            {
                m_pImportStubs[i++] = a;
                a = aa.first;
                aa++;
            }
            if (delDummy)
                m_SymTab.erase(ff); // Delete dummy entry
            numImports = n;
            return m_pImportStubs;
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
            std_map<ADDRESS, string> ret = new std_map<ADDRESS, string>();
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
            SectionInfo* sym = GetSectionInfoByName(".dynsym");
            if (sym == 0)
            {
                fprintf(stderr, "Could not find section .dynsym in source binary file");
                return ret;
            }
            Elf32_Sym* pSym = (Elf32_Sym*)sym.uHostAddr;
            int idxStr = GetSectionIndexByName(".dynstr");
            if (idxStr == -1)
            {
                fprintf(stderr, "Could not find section .dynstr in source binary file");
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

            return ret;
        }

        /*==============================================================================
         * FUNCTION:	ElfBinaryFile::elfRead2 and elfRead4
         * OVERVIEW:	Read a 2 or 4 byte quantity from host address (C pointer) p
         * NOTE:		Takes care of reading the correct endianness, set early on into m_elfEndianness
         * PARAMETERS:	ps or pi: host pointer to the data
         * RETURNS:		An integer representing the data
         *============================================================================*/
        int elfRead2(short* ps)
        {
            byte* p = (byte*)ps;
            if (bigEndian)
            {
                // Big endian
                return (int)((p[0] << 8) + p[1]);
            }
            else
            {
                // Little endian
                return (int)(p[0] + (p[1] << 8));
            }
        }

        int elfRead4(int* pi)
        {
            short* p = (short*)pi;
            if (bigEndian)
            {
                return (int)((elfRead2(p) << 16) + elfRead2(p + 1));
            }
            else
                return (int)(elfRead2(p) + (elfRead2(p + 1) << 16));
        }

        private void elfWrite4(int* pi, int val)
        {
            string p = (byte*)pi;
            if (bigEndian)
            {
                // Big endian
                *p++ = (byte)(val >> 24);
                *p++ = (byte)(val >> 16);
                *p++ = (byte)(val >> 8);
                *p = (byte)val;
            }
            else
            {
                *p++ = (byte)val;
                *p++ = (byte)(val >> 8);
                *p++ = (byte)(val >> 16);
                *p = (byte)(val >> 24);
            }
        }

        private void writeNative4(ADDRESS nat, uint n)
        {
            PSectionInfo si = GetSectionInfoByAddr(nat);
            if (si == 0) return;
            ADDRESS host = si.uHostAddr - si.uNativeAddr + nat;
            if (bigEndian)
            {
                *(byte*)host = (n >> 24) & 0xff;
                *(byte*)(host + 1) = (n >> 16) & 0xff;
                *(byte*)(host + 2) = (n >> 8) & 0xff;
                *(byte*)(host + 3) = n & 0xff;
            }
            else
            {
                *(byte*)(host + 3) = (n >> 24) & 0xff;
                *(byte*)(host + 2) = (n >> 16) & 0xff;
                *(byte*)(host + 1) = (n >> 8) & 0xff;
                *(byte*)host = n & 0xff;
            }
        }


        private void applyRelocations()
        {
            int nextFakeLibAddr = -2; // See R_386_PC32 below; -1 sometimes used for main
            if (m_pImage == 0) return; // No file loaded
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
                        SectionInfo* ps = &m_pSections[i];
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
                            ADDRESS destNatOrigin = 0, destHostOrigin = 0;
                            if (e_type == E_REL)
                            {
                                int destSection = m_sh_info[i];
                                destNatOrigin = m_pSections[destSection].uNativeAddr;
                                destHostOrigin = m_pSections[destSection].uHostAddr;
                            }
                            int symSection = m_sh_link[i]; // Section index for the associated symbol table
                            int strSection = m_sh_link[symSection]; // Section index for the string section assoc with this
                            string pStrSection = (char*)m_pSections[strSection].uHostAddr;
                            Elf32_Sym* symOrigin = (Elf32_Sym*)m_pSections[symSection].uHostAddr;
                            for (uint u = 0; u < size; u += 2 * sizeof(uint))
                            {
                                uint r_offset = elfRead4(pReloc++);
                                uint info = elfRead4(pReloc++);
                                byte relType = (byte)info;
                                uint symTabIndex = info >> 8;
                                int* pRelWord; // Pointer to the word to be relocated
                                if (e_type == E_REL)
                                    pRelWord = ((int*)(destHostOrigin + r_offset));
                                else
                                {
                                    if (r_offset == 0) continue;
                                    SectionInfo* destSec = GetSectionInfoByAddr(r_offset);
                                    pRelWord = (int*)(destSec.uHostAddr - destSec.uNativeAddr + r_offset);
                                    destNatOrigin = 0;
                                }
                                ADDRESS A, S = 0, P;
                                int nsec;
                                switch (relType)
                                {
                                case 0: // R_386_NONE: just ignore (common)
                                    break;
                                case 1: // R_386_32: S + A
                                    S = elfRead4((int*)&symOrigin[symTabIndex].st_value);
                                    if (e_type == E_REL)
                                    {
                                        nsec = elfRead2(&symOrigin[symTabIndex].st_shndx);
                                        if (nsec >= 0 && nsec < m_iNumSections)
                                            S += GetSectionInfo(nsec)->uNativeAddr;
                                    }
                                    A = elfRead4(pRelWord);
                                    elfWrite4(pRelWord, S + A);
                                    break;
                                case 2: // R_386_PC32: S + A - P
                                    if (ELF32_ST_TYPE(symOrigin[symTabIndex].st_info) == STT_SECTION)
                                    {
                                        nsec = elfRead2(&symOrigin[symTabIndex].st_shndx);
                                        if (nsec >= 0 && nsec < m_iNumSections)
                                            S = GetSectionInfo(nsec)->uNativeAddr;
                                    }
                                    else
                                    {
                                        S = elfRead4((int*)&symOrigin[symTabIndex].st_value);
                                        if (S == 0)
                                        {
                                            // This means that the symbol doesn't exist in this module, and is not accessed
                                            // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                                            // name of the symbol right here in the symbol table entry, but the only way
                                            // to communicate with the loader is through the target address of the call.
                                            // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                                            // in the symbol table
                                            int nameOffset = elfRead4((int*)&symOrigin[symTabIndex].st_name);
                                            string pName = pStrSection + nameOffset;
                                            // this is too slow, I'm just going to assume it is 0
                                            //S = GetAddressByName(pName);
                                            //if (S == (e_type == E_REL ? 0x8000000 : 0)) {
                                            S = nextFakeLibAddr--; // Allocate a new fake address
                                            AddSymbol(S, pName);
                                            //}
                                        }
                                        else if (e_type == E_REL)
                                        {
                                            nsec = elfRead2(&symOrigin[symTabIndex].st_shndx);
                                            if (nsec >= 0 && nsec < m_iNumSections)
                                                S += GetSectionInfo(nsec)->uNativeAddr;
                                        }
                                    }
                                    A = elfRead4(pRelWord);
                                    P = destNatOrigin + r_offset;
                                    elfWrite4(pRelWord, S + A - P);
                                    break;
                                case 7:
                                case 8: // R_386_RELATIVE
                                    break; // No need to do anything with these, if a shared object
                                default:
                                    // Console.Out.WriteLine( "Relocation type " + (int)relType + " not handled yet");
                                    ;
                                }
                            }
                        }
                    }
                }
            default:
                break; // Not implemented
            }
        }

        bool IsRelocationAt(ADDRESS uNative)
        {
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
                        SectionInfo* ps = &m_pSections[i];
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
                                    SectionInfo* destSec = GetSectionInfoByAddr(r_offset);
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
            return false;
        }

        string getFilenameSymbolFor(string sym)
        {
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
            PSectionInfo pSect = &m_pSections[secIndex];
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
                uint pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                if (ELF32_ST_TYPE(m_pSym[i].st_info) == STT_FILE)
                {
                    filename = str;
                    continue;
                }
                if (str == sym)
                {
                    if (filename.length())
                        return strdup(filename.c_str());
                    return null;
                }
            }
            return null;
        }

        private void getFunctionSymbols(SortedList<string, SortedList<ADDRESS, string>> syms_in_file) {
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
        fprintf(stderr, "no symtab section? Assuming stripped, looking for dynsym.\n");

        for (i = 1; i < m_iNumSections; ++i) {
            uint uType = m_pSections[i].uType;
            if (uType == SHT_DYNSYM) {
                secIndex = i;
                break;
            }
        }

        if (secIndex == 0) {
            fprintf(stderr, "no dynsyms either.. guess we're out of luck.\n");
            return;
        }
    }

    int e_type = elfRead2(&((Elf32_Ehdr*) m_pImage)->e_type);
    PSectionInfo pSect = &m_pSections[secIndex];
    // Calc number of symbols
    int nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
    m_pSym = (Elf32_Sym*) pSect.uHostAddr; // Pointer to symbols
    int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

    string filename = "unknown.c";

    // Index 0 is a dummy entry
    for (int i = 1; i < nSyms; i++) {
        int name = elfRead4(&m_pSym[i].st_name);
        if (name == 0) /* Silly symbols with no names */ continue;
        string str(GetStrPtr(strIdx, name));
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
        }

        // A map for extra symbols, those not in the usual Elf symbol tables

        private void AddSymbol(ADDRESS uNative, string pName)
        {
            m_SymTab[uNative] = pName;
        }

        private void dumpSymbols()
        {
            foreach (var de in m_SymTab)
                Console.Error.WriteLine("0x{0:X} {1}        ", de.Key, de.Value);
        }
#endif

    }

    public class SymTab
    {
    }

    public class SectionInfo
    {
        public string pSectionName;
        public uint uNativeAddr;
        public uint uSectionSize;
        public uint uHostAddr;
        public uint uType;
        public uint uSectionEntrySize;
        public bool bReadOnly;
        public bool bBss;
        public bool bCode;
        public bool bData;
    }

}

									