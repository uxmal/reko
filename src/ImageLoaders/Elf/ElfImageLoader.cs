using Decompiler.Arch.Intel;
using Decompiler.Arch.Sparc;
using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    using StrIntMap = SortedList<string, int>;

    using RelocMap = SortedList<uint, string>;

    using ADDRESS = UInt32;

    public class SymValue
    {
        public ADDRESS uSymAddr; // Symbol native address
        public int iSymSize; // Size associated with symbol
    }

    /// <summary>
    /// ELF executable header.
    /// </summary>
    public class Elf32_Ehdr
    {
        public const byte EM_SPARC = 2;			// Sun SPARC
        public const byte EM_386 = 3;			// Intel 80386 or higher
        public const byte EM_68K = 4;			// Motorola 68000
        public const byte EM_MIPS = 8;			// MIPS
        public const byte EM_PA_RISC = 15;		// HP PA-RISC
        public const byte EM_SPARC32PLUS = 18;	// Sun SPARC 32+
        public const byte EM_PPC = 20;			// PowerPC
        public const byte EM_X86_64 = 62;
        public const byte EM_ST20 = 0xa8;		// ST20 (made up... there is no official value?)

        public const byte ET_DYN = 3;	        // Elf type (dynamic library)
        public const byte E_REL = 1;		    // Relocatable file type


        public byte[] e_ident; // 4
        public byte e_class;
        public byte Endianness;
        public byte version;
        public byte osAbi;
        public byte[] pad; // 8

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
        public ushort e_shstrndx;        // Section table string table index.
    }



    /// <summary>
    /// ELF Program header 
    /// </summary>
    public class Elf32_Phdr
    {
        public int p_type; //  entry Type 
        public int p_offset; //  file offset 
        public int p_vaddr; //  virtual address 
        public int p_paddr; //  physical address 
        public int p_filesz; //  file size 
        public int p_memsz; //  memory size 
        public int p_flags; //  entry flags 
        public int p_align; //  memory/file alignment 
    }

    /// <summary>
    /// Section header
    /// </summary>
    public class Elf32_Shdr
    {
        public const byte SHF_WRITE = 1;		// Writeable
        public const byte SHF_ALLOC = 2;		// Consumes memory in exe
        public const byte SHF_EXECINSTR = 4;		// Executable

        public const byte SHT_NOBITS = 8;		// Bss
        public const byte SHT_REL = 9;		// Relocation table (no addend)
        public const byte SHT_RELA = 4;// Relocation table (with addend, e.g. RISC)
        public const byte SHT_SYMTAB = 2;		// Symbol table
        public const byte SHT_DYNSYM = 11;		// Dynamic symbol table

        public uint sh_name;
        public uint sh_type;
        public int sh_flags;
        public uint sh_addr;
        public uint sh_offset;
        public uint sh_size;
        public int sh_link;
        public int sh_info;
        public uint sh_addralign;
        public uint sh_entsize;
    }


    public class Elf32_Sym
    {
        public int ELF32_ST_TYPE { get { return st_info & 0x0F; } }

        public uint st_name;
        public uint st_value;
        public int st_size;
        public byte st_info;
        public byte st_other;
        public short st_shndx;
    }


    public class Elf32_Rel
    {
        public const byte R_386_32 = 1;
        public const byte R_386_PC32 = 2;

        public uint r_offset;
        public int r_info;
    }



    struct Elf32_Dyn
    {
        public short d_tag; //  how to interpret value 
        public int data;
        public int d_val { get { return data; } }
        public int d_ptr { get { return data; } }
        public int d_off { get { return data; } }
    }


    public class ElfImageLoader : ImageLoader
    {
        private byte[] m_pImage; // Pointer to the loaded image
        private Elf32_Phdr[] m_pPhdrs; // Pointer to program headers
        private Elf32_Shdr[] sectionHeaders; // Array of section header structs
        private uint m_pStrings; // Pointer to the string section
        private int m_elfEndianness; // 1 = Big Endian
        private SortedList<ADDRESS, string> m_SymTab; // Map from address to symbol name; contains symbols from the
        // various elf symbol tables, and possibly some symbols with fake
        // addresses
        //private SymTab m_Reloc; // Object to store the reloc syms
        private Elf32_Sym[] m_pSym; // Pointer to loaded symbol section
        private bool m_bAddend; // true if reloc table has addend
        private ADDRESS m_uLastAddr; // Save last address looked up
        private int m_iLastSize; // Size associated with that name
        private List<SectionInfo> m_EntryPoint; // A list of one entry point
        private ADDRESS m_uPltMin; // Min address of PLT table
        private ADDRESS m_uPltMax; // Max address (1 past last) of PLT
        private ADDRESS[] m_pImportStubs; // An array of import stubs
        private ADDRESS first_extern; // where the first extern will be placed
        private ADDRESS next_extern; // where the next extern will be placed
        public int[] m_sh_link; // pointer to array of sh_link values
        public int[] m_sh_info; // pointer to array of sh_info values
        private bool m_bArchive;
        private int m_iNumSections;
        public SectionInfo[] Sections { get; private set; }        //$ImageMap
        public Elf32_Ehdr ElfHeader { get; private set; }
        private IProcessorArchitecture arch;

        public override IProcessorArchitecture Architecture { get { return arch; } }
        public override Platform Platform { get { throw new NotSupportedException(); } }

        public int ELF32_R_SYM(int info) { return ((info) >> 8); }
        public int ELF32_ST_BIND(int i) { return ((i) >> 4); }
        public int ELF32_ST_INFO(int b, int t) { return (((b) << 4) + ((t) & 0xf)); }
        public const byte STT_NOTYPE = 0;			// Symbol table type: none
        public const byte STT_FUNC = 2;				// Symbol table type: function
        public const byte STT_SECTION = 3;
        public const byte STT_FILE = 4;
        public const byte STB_GLOBAL = 1;
        public const byte STB_WEAK = 2;


        // Tag values
        public const byte DT_NULL = 0;	            // Last entry in list
        public const byte DT_STRTAB = 5;	        // String table
        public const byte DT_NEEDED = 1;		    // A needed link-type object

        public uint NO_ADDRESS = ~0U;

        public ADDRESS GetAddressByName(string pName) { return GetAddressByName(pName, false); }
        public int GetSizeByName(string pName) { return GetSizeByName(pName, false); }

        public virtual List<ADDRESS> GetExportedAddresses() { return GetExportedAddresses(true); }

        public virtual List<SectionInfo> GetEntryPoints() { return GetEntryPoints("main"); }

        public virtual SortedList<ADDRESS, string> getSymbols()
        {
            return m_SymTab;
        }

        private bool ValueByName(string pName, SymValue pVal) { return ValueByName(pName, null); }


        public ElfImageLoader(IServiceProvider services, byte[] rawImage, bool bArchive /* = false */)
            : base(services, rawImage)
        {
            m_bArchive = bArchive;
            next_extern = 0;
            Init(); // Initialise all the common stuff
        }



        // Reset internal state, except for those that keep track of which member
        // we're up to
        private void Init()
        {
            m_pImage = null;
            m_pPhdrs = null; // No program headers
            sectionHeaders = null; // No section headers
            m_pStrings = 0; // No strings
            m_pSym = null;
            m_uPltMin = 0; // No PLT limits
            m_uPltMax = 0;
            m_iLastSize = 0;
            m_pImportStubs = null;
        }

        uint elf_hash(string name)
        {
            uint g, h = 0;
            for (int i = 0; i < name.Length; ++i)
            {
                h = (h << 4) + name[i];
                if ((g = h & 0xF0000000u) != 0)
                {
                    h ^= g >> 24;
                }
                h &= ~g;
            }
            return h;
        }

        public override Address PreferredBaseAddress
        {
            get { return new Address(0x08000000U); }
        }

        protected bool RealLoad(string sName)
        {
            if (m_bArchive)
            {
                // This is a member of an archive. Should not be using this function at all
                return false;
            }

            m_pImage = RawImage;
            ElfHeader = LoadElf32Header(m_pImage); // Save a lot of casts

            // Basic checks

            if (ElfHeader.Endianness != 1 && ElfHeader.Endianness != 2)
                throw new BadImageFormatException(string.Format("Unknown ELF endianness {0:X2}.", ElfHeader.Endianness));
            m_elfEndianness = ElfHeader.Endianness - 1;
            return true;
        }

        public override ProgramImage Load(Address addrLoad)
        {
            // Load program headers (in case needed)
            m_pPhdrs = LoadElf32ProgramHeader(ElfHeader);

            // Load section headers 
            sectionHeaders = LoadElf32SectionHeaders();

            // Set up section header string table pointer
            // NOTE: it does not appear that endianness affects shorts.. they are always in little endian format
            // Gerard: I disagree. I need the elfRead on linux/i386
            uint i = ElfHeader.e_shstrndx; // pHeader.e_shstrndx;
            if (i != 0) m_pStrings = sectionHeaders[i].sh_offset;

            i = 1; // counter - # sects. Start @ 1, total m_iNumSections
            uint pName; // Section's name

            // Number of sections
            m_iNumSections = ElfHeader.e_shnum;

            // Allocate room for all the Elf sections (including the silly first one)
            Sections = new SectionInfo[m_iNumSections];

            // Set up the m_sh_link and m_sh_info arrays
            m_sh_link = new int[m_iNumSections];
            m_sh_info = new int[m_iNumSections];

            // Number of ELF sections
            bool bGotCode = false; // True when have seen a code sect
            ADDRESS arbitaryLoadAddr = PreferredBaseAddress.Linear;
            for (i = 0; i < m_iNumSections; i++)
            {
                // Get section information.
                Elf32_Shdr pShdr = sectionHeaders[i];
                pName = m_pStrings + pShdr.sh_name;
                if (pName >= m_pImage.Length)
                    throw new BadImageFormatException("The name for section " + i + " is outside the image size.");
                
                var sectionName = ReadAsciizString(pName);
                Sections[i].pSectionName = sectionName;      //$ Move.
                uint off = pShdr.sh_offset;
                if (off != 0) Sections[i].uHostAddr = off;
                Sections[i].uNativeAddr = pShdr.sh_addr;
                Sections[i].uSectionSize = pShdr.sh_size;
                if (Sections[i].uNativeAddr == 0 && !sectionName.StartsWith(".rel"))
                {
                    uint align = pShdr.sh_addralign;
                    if (align > 1)
                    {
                        if ((arbitaryLoadAddr % align) != 0)
                            arbitaryLoadAddr += align - (arbitaryLoadAddr % align);
                    }
                    Sections[i].uNativeAddr = arbitaryLoadAddr;
                    arbitaryLoadAddr += Sections[i].uSectionSize;
                }
                Sections[i].uType = pShdr.sh_type;       //$ Move
                m_sh_link[i] = pShdr.sh_link;
                m_sh_info[i] = pShdr.sh_info;
                Sections[i].uSectionEntrySize = pShdr.sh_entsize;
                if (Sections[i].uNativeAddr + Sections[i].uSectionSize > next_extern)
                    first_extern = next_extern = Sections[i].uNativeAddr + Sections[i].uSectionSize;
                if ((pShdr.sh_flags & Elf32_Shdr.SHF_WRITE) == 0)
                    Sections[i].IsReadOnly = true;
                // Can't use the Elf32_Shdr.SHF_ALLOC bit to determine bss section; the bss section has SHF_ALLOC but also Elf32_Shdr.SHT_NOBITS.
                // (But many other sections, such as .comment, also have Elf32_Shdr.SHT_NOBITS). So for now, just use the name
                //      if ((elfRead4(ref pShdr.sh_flags) & SHF_ALLOC) == 0)
                if (sectionName == ".bss")
                    Sections[i].bBss = true;
                if ((pShdr.sh_flags & Elf32_Shdr.SHF_EXECINSTR) != 0)
                {
                    Sections[i].bCode = true;
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
                    ((pShdr.sh_flags & (Elf32_Shdr.SHF_EXECINSTR | Elf32_Shdr.SHF_ALLOC)) == Elf32_Shdr.SHF_ALLOC) &&
                    (pShdr.sh_type != Elf32_Shdr.SHT_NOBITS))
                    Sections[i].bData = true;
            } // for each section

            // assign arbitary addresses to .rel.* sections too
            for (i = 0; i < m_iNumSections; i++)
                if (Sections[i].uNativeAddr == 0 && string.Compare(Sections[i].pSectionName, 0, ".rel", 0, 4) == 0)
                {
                    Sections[i].uNativeAddr = arbitaryLoadAddr;
                    arbitaryLoadAddr += Sections[i].uSectionSize;
                }

            // Add symbol info. Note that some symbols will be in the main table only, and others in the dynamic table only.
            // So the best idea is to add symbols for all sections of the appropriate type
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = Sections[i].uType;
                if (uType == Elf32_Shdr.SHT_SYMTAB || uType == Elf32_Shdr.SHT_DYNSYM)
                    AddSyms(ElfHeader, i);
            }
            return new ProgramImage(null, m_pImage);
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
            // Save the relocation to symbol table info
            SectionInfo pRel = GetSectionInfoByName(".rela.text");
            if (pRel != null)
            {
                m_bAddend = true; // Remember its a relA table
                //SetRelocInfo(pRel);
            }
            else
            {
                m_bAddend = false;
                pRel = GetSectionInfoByName(".rel.text");
            }

            // Find the PLT limits. Required for IsDynamicLinkedProc(), e.g.
            SectionInfo pPlt = GetSectionInfoByName(".plt");
            if (pPlt != null)
            {
                m_uPltMin = pPlt.uNativeAddr;
                m_uPltMax = pPlt.uNativeAddr + pPlt.uSectionSize;
            }

            // Apply relocations; important when the input program is not compiled with -fPIC
            var relocator = new Relocator(this);
            relocator.ApplyRelocations();
        }

        private SectionInfo GetSectionInfoByName(string sectionName)
        {
            throw new NotImplementedException();
        }

        public string ReadAsciizString(uint offset)
        {
            int iStart = (int)offset;
            int i = iStart;
            for (; i < m_pImage.Length && m_pImage[i] != 0; ++i)
                ;
            return Encoding.ASCII.GetString(m_pImage, iStart, i - iStart);
        }

        private Elf32_Shdr [] LoadElf32SectionHeaders()
        {
            if (ElfHeader.e_shnum == 0)
                return null;
            var rdr = CreateReader(ElfHeader.e_shoff);
            var shdrs = new Elf32_Shdr[ElfHeader.e_shnum];
            for (int i = 0; i < shdrs.Length; ++i)
            {
                var shdr = new Elf32_Shdr();

                shdr.sh_name = rdr.ReadUInt32();
                shdr.sh_type = rdr.ReadUInt32();
                shdr.sh_flags = rdr.ReadInt32();
                shdr.sh_addr = rdr.ReadUInt32();
                shdr.sh_offset = rdr.ReadUInt32();
                shdr.sh_size = rdr.ReadUInt32();
                shdr.sh_link = rdr.ReadInt32();
                shdr.sh_info = rdr.ReadInt32();
                shdr.sh_addralign = rdr.ReadUInt32();
                shdr.sh_entsize = rdr.ReadUInt32();

                shdrs[i] = shdr;
            }
            return shdrs;
        }

        private Elf32_Phdr[] LoadElf32ProgramHeader(Elf32_Ehdr header)
        {
            if (header.e_phnum == 0)
                return null;
            var rdr = CreateReader(header.e_phoff);
            var phdrs = new Elf32_Phdr[header.e_phnum];
            for (int i = 0; i < phdrs.Length; ++i)
            {
                var phdr = new Elf32_Phdr();
                phdr.p_type = rdr.ReadInt32(); //  entry Type 
                phdr.p_offset = rdr.ReadInt32(); //  file offset 
                phdr.p_vaddr = rdr.ReadInt32(); //  virtual address 
                phdr.p_paddr = rdr.ReadInt32(); //  physical address 
                phdr.p_filesz = rdr.ReadInt32(); //  file size 
                phdr.p_memsz = rdr.ReadInt32(); //  memory size 
                phdr.p_flags = rdr.ReadInt32(); //  entry flags 
                phdr.p_align = rdr.ReadInt32(); //  memory/file alignment 
                phdrs[i] = phdr;
            }
            return phdrs;
        }


        public ImageReader CreateReader(uint imageOffset)
        {
            if (m_elfEndianness > 0)
                return new BeImageReader(RawImage, imageOffset);
            else
                return new LeImageReader(RawImage, imageOffset);
        }

        public ImageWriter CreateWriter()
        {
            if (m_elfEndianness > 0)
                return new BeImageWriter(RawImage);
            else
                return new LeImageWriter(RawImage);
        }

        private Elf32_Ehdr LoadElf32Header(byte[] m_pImage)
        {
            var hdr = new Elf32_Ehdr();
            hdr.e_ident = new byte[4];
            Array.Copy(m_pImage, hdr.e_ident, 4);
            if (hdr.e_ident[0] != '\x7F' ||
                hdr.e_ident[1] != 'E' ||
                hdr.e_ident[2] != 'L' ||
                hdr.e_ident[3] != 'F')
                throw new BadImageFormatException("The file doesn't appear to be an ELF executable binary.");

            hdr.e_class = m_pImage[4];
            hdr.Endianness = m_pImage[4];
            hdr.version = m_pImage[4];
            hdr.osAbi = m_pImage[4];

            var rdr = CreateReader(16);

            //public byte [] pad; // 8

            hdr.e_type = rdr.ReadInt16();
            hdr.e_machine = rdr.ReadInt16();
            hdr.e_version = rdr.ReadInt32();
            hdr.e_entry = rdr.ReadInt32();
            hdr.e_phoff = rdr.ReadUInt32();
            hdr.e_shoff = rdr.ReadUInt32();
            hdr.e_flags = rdr.ReadInt32();
            hdr.e_ehsize = rdr.ReadInt16();
            hdr.e_phentsize = rdr.ReadInt16();
            hdr.e_phnum = rdr.ReadInt16();
            hdr.e_shentsize = rdr.ReadInt16();
            hdr.e_shnum = rdr.ReadInt16();
            hdr.e_shstrndx = rdr.ReadUInt16();

            arch = CreateArchitecture();
            return hdr;
        }


        // Like a replacement for elf_strptr()
        string GetStrPtr(int idx, uint offset)
        {
            if (idx < 0)
            {
                // Most commonly, this will be an index of -1, because a call to GetSectionIndexByName() failed
                Console.Error.WriteLine("Error! GetStrPtr passed index of {0}.\n", idx);
                return "Error!";
            }
            // Get a pointer to the start of the string table
            var pSym = Sections[idx].uHostAddr;
            // Just add the offset
            return ReadAsciizString(pSym + offset);
        }


        // Search the .rel[a].plt section for an entry with symbol table index i.
        // If found, return the native address of the associated PLT entry.
        // A linear search will be needed. However, starting at offset i and searching backwards with wraparound should
        // typically minimise the number of entries to search
        ADDRESS findRelPltOffset(int i, ADDRESS addrRelPlt, uint sizeRelPlt, uint numRelPlt, ADDRESS addrPlt)
        {
            uint first = (uint) i;
            if (first >= numRelPlt)
                first = numRelPlt - 1u;
            uint curr = first;
            do
            {
                // Each entry is sizeRelPlt bytes, and will contain the offset, then the info (addend optionally follows)
                var pEntry = CreateReader(addrRelPlt + curr * sizeRelPlt + 4);
                int entry = pEntry.ReadInt32(); // Read pEntry[1]
                int sym = entry >> 8; // The symbol index is in the top 24 bits (Elf32 only)
                if (sym == i)
                {
                    // Found! Now we want the native address of the associated PLT entry.
                    // For now, assume a size of 0x10 for each PLT entry, and assume that each entry in the .rel.plt section
                    // corresponds exactly to an entry in the .plt (except there is one dummy .plt entry)
                    return addrPlt + 0x10 * (curr + 1u);
                }
                if (--curr < 0)
                    curr = numRelPlt - 1;
            } while (curr != first); // Will eventually wrap around to first if not present
            return 0; // Exit if this happens
        }

        // Add appropriate symbols to the symbol table.  secIndex is the section index of the symbol table.
        private void AddSyms(Elf32_Ehdr header, uint secIndex)
        {
            var e_type = header.e_type;
            SectionInfo pSect = Sections[secIndex];
            // Calc number of symbols
            var nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            SectionInfo siPlt = GetSectionInfoByName(".plt");
            ADDRESS addrPlt = siPlt != null ? siPlt.uNativeAddr : 0;
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

            var rdr = CreateReader(pSect.uHostAddr);
            ReadSymbolEntry(rdr);
            // Number of entries in the PLT:
            // int max_i_for_hack = siPlt ? (int)siPlt.uSectionSize / 0x10 : 0;
            // Index 0 is a dummy entry
            for (int i = 1; i < nSyms; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                ADDRESS val = sym.st_value;
                var name = sym.st_name;
                if (name == 0)
                    continue; // Silly symbols with no names
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                // Ensure no overwriting (except functions)
                if (!m_SymTab.ContainsKey(val) || m_pSym[i].ELF32_ST_TYPE == STT_FUNC)
                {
                    if (val == 0 && siPlt != null)
                    { //&& i < max_i_for_hack) {
                        // Special hack for gcc circa 3.3.3: (e.g. test/pentium/settest).  The value in the dynamic symbol table
                        // is zero!  I was assuming that index i in the dynamic symbol table would always correspond to index i
                        // in the .plt section, but for fedora2_true, this doesn't work. So we have to look in the .rel[a].plt
                        // section. Thanks, gcc!  Note that this hack can cause strange symbol names to appear
                        val = findRelPltOffset(i, addrRelPlt, sizeRelPlt, numRelPlt, addrPlt);
                    }
                    else if (e_type == Elf32_Ehdr.E_REL)
                    {
                        int nsec = sym.st_shndx;
                        if (nsec >= 0 && nsec < m_iNumSections)
                            val += Sections[nsec].uNativeAddr;
                    }

#if		ECHO_SYMS
            Console.Error.WriteLine( "Elf AddSym: about to add " + str + " to address " + std::hex + val + std::dec);
#endif
                    m_SymTab[val] = str;
                }
            }
            ADDRESS uMain = GetMainEntryPoint();
            if (uMain != ~0U && !m_SymTab.ContainsKey(uMain))
            {
                // Ugh - main mustn't have the STT_FUNC attribute. Add it
                m_SymTab[uMain] = "main";
            }
        }

        private Elf32_Sym ReadSymbolEntry(ImageReader rdr)
        {
            var sym = new Elf32_Sym();
            sym.st_name = rdr.ReadUInt32();
            sym.st_value = rdr.ReadUInt32();
            sym.st_size = rdr.ReadInt32();
            sym.st_info = rdr.ReadByte();
            sym.st_other = rdr.ReadByte();
            sym.st_shndx = rdr.ReadInt16();
            return sym;
        }

        List<ADDRESS> GetExportedAddresses(bool funcsOnly)
        {
            var exported = new List<ADDRESS>();

            int secIndex = 0;
            for (int i = 1; i < m_iNumSections; ++i)
            {
                uint uType = Sections[i].uType;
                if (uType == Elf32_Shdr.SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return exported;

            int e_type = ElfHeader.e_type;
            SectionInfo pSect = Sections[secIndex];
            // Calc number of symbols
            uint nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            var rdr = CreateReader(pSect.uHostAddr); // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            // Index 0 is a dummy entry
            ReadSymbolEntry(rdr);
            for (int i = 1; i < nSyms; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                var val = CreateReader(sym.st_value).ReadUInt32();

                uint name = sym.st_name;
                if (name == 0) // Silly symbols with no names 
                    continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str.Remove(pos);
                if (ELF32_ST_BIND(m_pSym[i].st_info) == STB_GLOBAL || ELF32_ST_BIND(m_pSym[i].st_info) == STB_WEAK)
                {
                    if (funcsOnly == false || m_pSym[i].ELF32_ST_TYPE == STT_FUNC)
                    {
                        if (e_type == Elf32_Ehdr.E_REL)
                        {
                            var nsec = sym.st_shndx;
                            if (nsec >= 0 && nsec < m_iNumSections)
                                val += Sections[nsec].uNativeAddr;
                        }
                        exported.Add(val);
                    }
                }
            }
            return exported;

        }



        string SymbolByAddress(ADDRESS dwAddr)
        {
            string str;
            if (!m_SymTab.TryGetValue(dwAddr, out str))
                return null;
            return str;
        }


        bool ValueByName(string pName, SymValue pVal, bool bNoTypeOK /* = false */)
        {
            uint hash, numBucket, numChain, y;
            uint [] pBuckets;
            uint [] pChains; // For symbol table work
            bool found;
            int iStr; // Section index of the string table
            SectionInfo pSect;

            pSect = GetSectionInfoByName(".dynsym");
            if (pSect == null)
            {
                // We have a file with no .dynsym section, and hence no .hash section (from my understanding - MVE).
                // It seems that the only alternative is to linearly search the symbol tables.
                // This must be one of the big reasons that linking is so slow! (at least, for statically linked files)
                // Note MVE: We can't use m_SymTab because we may need the size
                return SearchValueByName(pName, pVal);
            }
            if (pSect.uHostAddr == 0) return false;
            Elf32_Sym [] pSym = LoadElf32Symbols(pSect);
            pSect = GetSectionInfoByName(".hash");
            if (pSect == null) return false;
            var pHash = CreateReader(pSect.uHostAddr);
            iStr = GetSectionIndexByName(".dynstr");

            // First load the hash table
            numBucket = pHash.ReadUInt32();
            numChain = pHash.ReadUInt32();
            pBuckets = new uint[numBucket];
            for (int i = 0; i < numBucket; ++i)
            {
                pBuckets[i] = pHash.ReadUInt32();
            }
            pChains = new uint[numChain];
            for (int i = 0; i < numBucket; ++i)
            {
                pChains[i] = pHash.ReadUInt32();
            }

            // Hash the symbol
            hash = elf_hash(pName) % numBucket;
            y = pBuckets[hash]; // Look it up in the bucket list
            // Beware of symbol tables with 0 in the buckets, e.g. libstdc++.
            // In that case, set found to false.
            found = (y != 0);
            if (y != 0)
            {
                while (pName != GetStrPtr(iStr, pSym[y].st_name))
                {
                    y = pChains[y];
                    if (y == 0)
                    {
                        found = false;
                        break;
                    }
                }
            }
            // Beware of symbols with STT_NOTYPE, e.g. "open" in libstdc++ !
            // But sometimes "main" has the STT_NOTYPE attribute, so if bNoTypeOK is passed as true, return true
            if (found && (bNoTypeOK || (pSym[y].ELF32_ST_TYPE != STT_NOTYPE)))
            {
                pVal.uSymAddr = pSym[y].st_value;
                int e_type = ElfHeader.e_type;
                if (e_type == Elf32_Ehdr.E_REL)
                {
                    int nsec = pSym[y].st_shndx;
                    if (nsec >= 0 && nsec < m_iNumSections)
                        pVal.uSymAddr += Sections[nsec].uNativeAddr;
                }
                pVal.iSymSize = pSym[y].st_size;
                return true;
            }
            else
            {
                // We may as well do a linear search of the main symbol table. Some symbols (e.g. init_dummy) are
                // in the main symbol table, but not in the hash table
                return SearchValueByName(pName, pVal);
            }
        }

        public Elf32_Sym[] LoadElf32Symbols(SectionInfo section)
        {
            if (section.uHostAddr == 0)
                return null;
            var rdr = CreateReader(section.uHostAddr);
            // Find number of symbols
            uint n = section.uSectionSize / section.uSectionEntrySize;
            var syms = new Elf32_Sym[n];
            for (int i = 0; i < syms.Length; i++)
            {
                var sym = new Elf32_Sym();
                sym.st_name = rdr.ReadUInt32();
                sym.st_value = rdr.ReadUInt32();
                sym.st_size = rdr.ReadInt32();
                sym.st_info = rdr.ReadByte();
                sym.st_other = rdr.ReadByte(); 
                sym.st_shndx = rdr.ReadInt16();
                syms[i] = sym;
            }
            return syms;
        }

        private int GetSectionIndexByName(string sectionName)
        {
            for (int i = 0; i < Sections.Length; i++)
            {
                if (Sections[i].pSectionName == sectionName)
                    return i;
            }
            return -1;
        }


        // Lookup the symbol table using linear searching. See comments above for why this appears to be needed.
        bool SearchValueByName(string pName, SymValue pVal, string pSectName, string pStrName)
        {
            // Note: this assumes .symtab. Many files don't have this section!!!
            SectionInfo pSect, pStrSect;

            pSect = GetSectionInfoByName(pSectName);
            if (pSect == null) return false;
            pStrSect = GetSectionInfoByName(pStrName);
            if (pStrSect == null) return false;
            var pStr = pStrSect.uHostAddr;
            // Find number of symbols
            uint n = pSect.uSectionSize / pSect.uSectionEntrySize;
            var rdr = CreateReader(pSect.uHostAddr);
            // Search all the symbols. It may be possible to start later than index 0
            for (int i = 0; i < n; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                if (pName == ReadAsciizString(pStrSect.uHostAddr + sym.st_name))
                {
                    // We have found the symbol
                    pVal.uSymAddr = sym.st_value;
                    int e_type = ElfHeader.e_type;
                    if (e_type == Elf32_Ehdr.E_REL)
                    {
                        short nsec = sym.st_shndx;
                        if (nsec >= 0 && nsec < m_iNumSections)
                            pVal.uSymAddr += GetSectionInfo(nsec).uNativeAddr;
                    }
                    pVal.iSymSize = sym.st_size;
                    return true;
                }
            }
            return false; // Not found (this table)
        }

        public SectionInfo GetSectionInfo(int nsec)
        {
            throw new NotImplementedException();
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
            SymValue Val = new SymValue();
            bool bSuccess = ValueByName(pName, Val, bNoTypeOK);
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
            bool bSuccess = ValueByName(pName, Val, bNoTypeOK);
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
            if (size!=0) return size; // No need to guess!
            // No need to guess, but if there are fillers, then subtracting labels will give a better answer for coverage
            // purposes. For example, switch_cc. But some programs (e.g. switch_ps) have the switch tables between the
            // end of _start and main! So we are better off overall not trying to guess the size of _start
            uint value = GetAddressByName(sName);
            if (value == 0) return 0; // Symbol doesn't even exist!

            SectionInfo pSect = GetSectionInfoByName(pSectName);
            if (pSect == null) return 0;
            // Find number of symbols
            uint n = pSect.uSectionSize / pSect.uSectionEntrySize;
            var rdr = CreateReader(pSect.uHostAddr);
            // Search all the symbols. It may be possible to start later than index 0
            uint closest = 0xFFFFFFFFu;
            int idx = -1;
            for (int i = 0; i < n; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                if ((sym.st_value > value) && sym.st_value < closest)
                {
                    idx = i;
                    closest = sym.st_value;
                }
            }
            if (idx == -1) 
                return 0;
            
            // Do some checks on the symbol's value; it might be at the end of the .text section
            pSect = GetSectionInfoByName(".text");
            ADDRESS low = pSect.uNativeAddr;
            ADDRESS hi = low + pSect.uSectionSize;
            if ((value >= low) && (value < hi))
            {
                // Our symbol is in the .text section. Put a ceiling of the end of the section on closest.
                if (closest > hi) closest = hi;
            }
            return (int) closest - (int) value;
        }


        int GetDistanceByName(string sName)
        {
            int val = GetDistanceByName(sName, ".symtab");
            if (val!= 0) return val;
            return GetDistanceByName(sName, ".dynsym");
        }


        bool IsDynamicLinkedProc(ADDRESS uNative)
        {
            if (uNative > ~1024u && uNative != ~0u)
                return true; // Say yes for fake library functions
            if (uNative >= first_extern && uNative < next_extern)
                return true; // Yes for externs (not currently used)
            if (m_uPltMin == 0) return false;
            return (uNative >= m_uPltMin) && (uNative < m_uPltMax); // Yes if a call to the PLT (false otherwise)
        }


        //
        // Returns a list of pointers to SectionInfo structs representing entry points to the program
        // Item 0 is the main() function; items 1 and 2 are .init and .fini
        List<SectionInfo> GetEntryPoints(string pEntry)
        {
            SectionInfo  pSect = GetSectionInfoByName(".text");
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
            return (ADDRESS)ElfHeader.e_entry;
        }

        // FIXME: the below assumes a fixed delta
        ADDRESS NativeToHostAddress(ADDRESS uNative)
        {
            if (m_iNumSections == 0) return 0;
            return Sections[1].uHostAddr - Sections[1].uNativeAddr + uNative;
        }

        ADDRESS GetRelocatedAddress(ADDRESS uNative)
        {
            // Not implemented yet. But we need the function to make it all link
            return 0;
        }



        private IProcessorArchitecture CreateArchitecture()
        {
            int machine = ElfHeader.e_machine;
            if (machine == Elf32_Ehdr.EM_386) 
                return new IntelArchitecture(ProcessorMode.ProtectedFlat);
            else if ((machine == Elf32_Ehdr.EM_SPARC) || (machine == Elf32_Ehdr.EM_SPARC32PLUS))
                return new SparcArchitecture();
            //else if (machine == EM_PA_RISC) return MACHINE_HPRISC;
            //else if (machine == EM_68K) return MACHINE_PALM; // Unlikely
            //else if (machine == EM_PPC) return MACHINE_PPC;
            //else if (machine == EM_ST20) return MACHINE_ST20;
            //else if (machine == EM_MIPS) return MACHINE_MIPS;
            else if (machine == Elf32_Ehdr.EM_X86_64)
            {
                throw new NotSupportedException("The AMD x86-64 architecture is not supported yet.");
            }
            // An unknown machine type
            throw new NotSupportedException(string.Format("Unsupported machine type: {0} {0:X}", machine));
        }

        public virtual bool isLibrary()
        {
            int type = ElfHeader.e_type;
            return (type == Elf32_Ehdr.ET_DYN);
        }

        List<string> getDependencyList()
        {
            var result = new List<string>();
            ADDRESS stringtab = ~0U;
            var dynsect = GetSectionInfoByName(".dynamic");
            if (dynsect == null)
                return result; //  no dynamic section = statically linked 

            var rdr = CreateReader(dynsect.uHostAddr);
            Elf32_Dyn dyn;
            for (dyn = ReadDependencyEntry(rdr); dyn.d_tag != DT_NULL; dyn = ReadDependencyEntry(rdr))
            {
                if (dyn.d_tag == DT_STRTAB)
                {
                    stringtab = (ADDRESS)dyn.d_ptr;
                    break;
                }
            }

            if (stringtab == NO_ADDRESS) //  No string table = no names 
                return result;
            stringtab = NativeToHostAddress(stringtab);

            rdr = CreateReader(dynsect.uHostAddr);
            for (dyn = ReadDependencyEntry(rdr); dyn.d_tag != DT_NULL; dyn = ReadDependencyEntry(rdr))
            {
                if (dyn.d_tag == DT_NEEDED)
                {
                    uint need = (uint)(stringtab + dyn.d_val);
                    if (need != 0)
                        result.Add(ReadAsciizString(need));
                }
            }
            return result;
        }

        private Elf32_Dyn ReadDependencyEntry(ImageReader rdr)
        {
            var dyn = new Elf32_Dyn();
            dyn.d_tag = rdr.ReadInt16();
            dyn.data = rdr.ReadInt32();
            return dyn;

        }

        /*==============================================================================
         * FUNCTION:	  ElfBinaryFile::GetImportStubs
         * OVERVIEW:	  Get an array of addresses of imported function stubs
         *					This function relies on the fact that the symbols are sorted by address, and that Elf PLT
         *					entries have successive addresses beginning soon after m_PltMin
         * PARAMETERS:	  numImports - reference to integer set to the number of these
         * RETURNS:		  An array of native ADDRESSes
         *============================================================================*/
        ADDRESS [] GetImportStubs(out int numImports)
        {
            ADDRESS a = m_uPltMin;
            int n = 0;
            int aa = m_SymTab.IndexOfKey(a);
            int ff = aa;
            bool delDummy = false;
            if (aa < 0)
            {
                // Need to insert a dummy entry at m_uPltMin
                delDummy = true;
                m_SymTab[a] = "";
                ff = m_SymTab.IndexOfKey(a);
                aa = ff;
                aa++;
            }
            while ((0 <= aa && aa < m_SymTab.Count) && (a < m_uPltMax))
            {
                n++;
                a = m_SymTab.Keys[aa];
                aa++;
            }
            // Allocate an array of ADDRESSESes
            m_pImportStubs = new ADDRESS[n];
            aa = ff; // Start at first
            a = m_SymTab.Keys[aa];
            int i = 0;
            while ((0 <= aa && aa < m_SymTab.Count) && (a < m_uPltMax))
            {
                m_pImportStubs[i++] = a;
                a = m_SymTab.Keys[aa];
                aa++;
            }
            if (delDummy)
                m_SymTab.RemoveAt(ff); // Delete dummy entry
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

        // Get a map from ADDRESS to string . This map contains the native addresses and symbolic names of global
        // data items (if any) which are shared with dynamically linked libraries. Example: __iob (basis for stdout).
        // The ADDRESS is the native address of a pointer to the real dynamic data object.

        SortedList<ADDRESS, string> GetDynamicGlobalMap()
        {
            var ret = new SortedList<ADDRESS, string>();
            SectionInfo pSect = GetSectionInfoByName(".rel.bss");
            if (pSect == null)
                pSect = GetSectionInfoByName(".rela.bss");
            if (pSect == null)
            {
                // This could easily mean that this file has no dynamic globals, and
                // that is fine.
                return ret;
            }
            uint numEnt = pSect.uSectionSize / pSect.uSectionEntrySize;

            SectionInfo dynsym = GetSectionInfoByName(".dynsym");
            if (dynsym == null)
            {
                Console.Error.WriteLine("Could not find section .dynsym in source binary file.");
                return ret;
            }
            Elf32_Sym [] pSym =  LoadElf32Symbols(dynsym);

            int idxStr = GetSectionIndexByName(".dynstr");
            if (idxStr == -1)
            {
                Console.Error.WriteLine("Could not find section .dynstr in source binary file.");
                return ret;
            }

            uint p = pSect.uHostAddr;
            for (int i = 0; i < numEnt; i++)
            {
                throw new NotImplementedException();
#if NYI
                // The ugly p[1] below is because it p might point to an Elf32_Rela struct, or an Elf32_Rel struct
                int sym = ELF32_R_SYM(((int*)p)[1]);
                uint name = pSym[sym].st_name; // Index into string table
                string s = GetStrPtr(idxStr, name);
                ADDRESS val = ((int*)p)[0];
                ret[val] = s; // Add the (val, s) mapping to ret
#endif
                p += pSect.uSectionEntrySize;
            }

            return ret;
        }

 
        // Apply relocations; important when compiled without -fPIC
        private void applyRelocations()
        {
            var relocator = new Relocator(this);
            relocator.ApplyRelocations();
        }

        public SectionInfo GetSectionInfoByAddr(uint r_offset)
        {
            throw new NotImplementedException();
        }

        bool IsRelocationAt(ADDRESS uNative)
        {
            //int nextFakeLibAddr = -2;			// See R_386_PC32 below; -1 sometimes used for main
            if (m_pImage == null) return false; // No file loaded
            int machine = ElfHeader.e_machine;
            int e_type = ElfHeader.e_type;
            switch (machine)
            {
            default:
                throw new NotSupportedException("Unsupported machine type.");
            case Elf32_Ehdr.EM_SPARC:
                throw new NotImplementedException();
            case Elf32_Ehdr.EM_386:
                for (int i = 1; i < m_iNumSections; ++i)
                {
                    SectionInfo ps = Sections[i];
                    if (ps.uType == Elf32_Shdr.SHT_REL)
                    {
                        // A section such as .rel.dyn or .rel.plt (without an addend field).
                        // Each entry has 2 words: r_offet and r_info. The r_offset is just the offset from the beginning
                        // of the section (section given by the section header's sh_info) to the word to be modified.
                        // r_info has the type in the bottom byte, and a symbol table index in the top 3 bytes.
                        // A symbol table offset of 0 (STN_UNDEF) means use value 0. The symbol table involved comes from
                        // the section header's sh_link field.
                        var pReloc = CreateReader(ps.uHostAddr);
                        uint size = ps.uSectionSize;
                        // NOTE: the r_offset is different for .o files (E_REL in the e_type header field) than for exe's
                        // and shared objects!
                        ADDRESS destNatOrigin = 0, destHostOrigin;
                        if (e_type == Elf32_Ehdr.E_REL)
                        {
                            int destSection = m_sh_info[i];
                            destNatOrigin = Sections[destSection].uNativeAddr;
                            destHostOrigin = Sections[destSection].uHostAddr;
                        }
                        //int symSection = m_sh_link[i];			// Section index for the associated symbol table
                        //int strSection = m_sh_link[symSection];	// Section index for the string section assoc with this
                        //string pStrSection = (char*)m_pSections[strSection].uHostAddr;
                        //Elf32_Sym* symOrigin = (Elf32_Sym*) m_pSections[symSection].uHostAddr;
                        for (uint u = 0; u < size; u += 2 * sizeof(uint))
                        {
                            uint r_offset = pReloc.ReadUInt32();
                            uint info = pReloc.ReadUInt32();
                            //byte relType = (byte) info;
                            //unsigned symTabIndex = info >> 8;
                            ADDRESS pRelWord; // Pointer to the word to be relocated
                            if (e_type == Elf32_Ehdr.E_REL)
                                pRelWord = destNatOrigin + r_offset;
                            else
                            {
                                if (r_offset == 0) continue;
                                var destSec = GetSectionInfoByAddr(r_offset);
                                pRelWord = destSec.uNativeAddr + r_offset;
                                destNatOrigin = 0;
                            }
                            if (uNative == pRelWord)
                                return true;
                        }
                    }
                }
                break;
            }
            return false;
        }

        string getFilenameSymbolFor(string symbol)
        {
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                uint uType = Sections[i].uType;
                if (uType == Elf32_Shdr.SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
                return null;

            //int e_type = elfRead2(&((Elf32_Ehdr*)m_pImage).e_type);
            SectionInfo pSect = Sections[secIndex];
            // Calc number of symbols
            uint nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            var rdr = CreateReader(pSect.uHostAddr); // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table
            string filename = "";

            ReadSymbolEntry(rdr); // Index 0 is a dummy entry
            for ( i = 1; i < nSyms; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                //ADDRESS val = (ADDRESS) elfRead4((int*)&m_pSym[i].st_value);
                uint name = sym.st_name;
                if (name == 0) // Silly symbols with no names
                    continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos;
                if ((pos = str.IndexOf("@@")) >= 0)
                    str = str.Remove(pos);
                if (m_pSym[i].ELF32_ST_TYPE == STT_FILE)
                {
                    filename = str;
                    continue;
                }
                if (str == symbol)
                {
                    if (string.IsNullOrEmpty(filename))
                        return null;
                    else 
                        return filename;
                }
            }
            return null;
        }

        private void getFunctionSymbols(SortedList<string, SortedList<ADDRESS, string>> syms_in_file)
        {
            int i;
            int secIndex = 0;
            for (i = 1; i < m_iNumSections; ++i)
            {
                var uType = Sections[i].uType;
                if (uType == Elf32_Shdr.SHT_SYMTAB)
                {
                    secIndex = i;
                    break;
                }
            }
            if (secIndex == 0)
            {
                Console.Error.WriteLine("no symtab section? Assuming stripped, looking for dynsym.");

                for (i = 1; i < m_iNumSections; ++i)
                {
                    var uType = Sections[i].uType;
                    if (uType == Elf32_Shdr.SHT_DYNSYM)
                    {
                        secIndex = i;
                        break;
                    }
                }

                if (secIndex == 0)
                {
                    Console.Error.WriteLine("no dynsyms either.. guess we're out of luck.");
                    return;
                }
            }

            int e_type = ElfHeader.e_type;
            SectionInfo pSect = Sections[secIndex];
            // Calc number of symbols
            var nSyms = pSect.uSectionSize / pSect.uSectionEntrySize;
            var rdr = CreateReader(pSect.uHostAddr); // Pointer to symbols
            int strIdx = m_sh_link[secIndex]; // sh_link points to the string table

            string filename = "unknown.c";

            // Index 0 is a dummy entry
            ReadSymbolEntry(rdr);
            for (i = 1; i < nSyms; i++)
            {
                var sym = ReadSymbolEntry(rdr);
                var name = sym.st_name;
                if (name == 0) /* Silly symbols with no names */ continue;
                string str = GetStrPtr(strIdx, name);
                // Hack off the "@@GLIBC_2.0" of Linux, if present
                int pos = str.IndexOf("@@");
                if (pos >= 0)
                    str = str.Remove(pos);
                if (sym.ELF32_ST_TYPE == STT_FILE)
                {
                    filename = str;
                    continue;
                }
                if (sym.ELF32_ST_TYPE == STT_FUNC)
                {
                    var val = (ADDRESS)sym.st_value;
                    if (e_type == Elf32_Ehdr.E_REL)
                    {
                        var nsec = sym.st_shndx;
                        if (nsec >= 0 && nsec < m_iNumSections)
                            val += Sections[nsec].uNativeAddr;
                    }
                    if (val == 0)
                    {
                        // ignore plt for now
                    }
                    else
                    {
                        syms_in_file[filename][val] = str;
                    }
                }
            }
        }

        // A map for extra symbols, those not in the usual Elf symbol tables

        public void AddSymbol(ADDRESS uNative, string pName)
        {
            m_SymTab[uNative] = pName;
        }


        private void dumpSymbols()
        {
            foreach (var it in m_SymTab)
                Console.Error.WriteLine("0x{0:X} 0x{1:X}        ", it.Key, it.Value);

        }
    }
}
