using System;
using System.Collections.Generic;

namespace RekoMakeSigs
{

    internal class Elf : DataBuffer
    {

        const uint SHT_NULL =           0;              // Section header table entry unused
        const uint SHT_PROGBITS =       1;              // Program data
        const uint SHT_SYMTAB =         2;              // Symbol table
        const uint SHT_STRTAB =         3;              // String table
        const uint SHT_RELA =           4;              // Relocation entries with addends. Warning: Works only in 64 bit mode in my tests!
        const uint SHT_HASH =           5;              // Symbol hash table
        const uint SHT_DYNAMIC =        6;              // Dynamic linking information
        const uint SHT_NOTE =           7;              // Notes
        const uint SHT_NOBITS =         8;              // Program space with no data (bss)
        const uint SHT_REL =            9;              // Relocation entries, no addends
        const uint SHT_SHLIB =          10;             // Reserved
        const uint SHT_DYNSYM =         11;             // Dynamic linker symbol table
        const uint SHT_INIT_ARRAY =     14;             // Array of constructors
        const uint SHT_FINI_ARRAY =     15;             // Array of destructors
        const uint SHT_PREINIT_ARRAY =  16;             // Array of pre-constructors
        const uint SHT_GROUP =          17;             // Section group
        const uint SHT_SYMTAB_SHNDX =   18;             // Extended section indeces
        const uint SHT_NUM =            19;             // Number of defined types. 
        const uint SHT_LOOS =           0x60000000;     // Start OS-specific
        const uint SHT_CHECKSUM =       0x6ffffff8;     // Checksum for DSO content. 
        const uint SHT_LOSUNW =         0x6ffffffa;     // Sun-specific low bound. 
        const uint SHT_SUNW_move =      0x6ffffffa;
        const uint SHT_SUNW_COMDAT =    0x6ffffffb;
        const uint SHT_SUNW_syminfo =   0x6ffffffc;
        const uint SHT_GNU_verdef =     0x6ffffffd;     // Version definition section. 
        const uint SHT_GNU_verneed =    0x6ffffffe;     // Version needs section. 
        const uint SHT_GNU_versym =     0x6fffffff;     // Version symbol table. 
        const uint SHT_HISUNW =         0x6fffffff;     // Sun-specific high bound. 
        const uint SHT_HIOS =           0x6fffffff;     // End OS-specific type
        const uint SHT_LOPROC =         0x70000000;     // Start of processor-specific
        const uint SHT_HIPROC =         0x7fffffff;     // End of processor-specific
        const uint SHT_LOUSER =         0x80000000;     // Start of application-specific
        const uint SHT_HIUSER =         0x8fffffff;     // End of application-specific
        const uint SHT_REMOVE_ME =      0xffffff99;     // Specific to objconv program: Removed debug or exception handler section

        // Legal values for ST_BIND subfield of st_info (symbol binding).
        const uint STB_LOCAL = 0;   // Local symbol
        const uint STB_GLOBAL = 1;  // Global symbol
        const uint STB_WEAK = 2;    // Weak symbol
        const uint STB_NUM = 3;     // Number of defined types. 
        const uint STB_LOOS = 10;   // Start of OS-specific
        const uint STB_HIOS = 12;   // End of OS-specific
        const uint STB_LOPROC = 13; // Start of processor-specific
        const uint STB_HIPROC = 15; // End of processor-specific

        // Legal values for ST_TYPE subfield of st_info (symbol type). 
        const uint STT_NOTYPE = 0;      // Symbol type is unspecified
        const uint STT_OBJECT = 1;      // Symbol is a data object
        const uint STT_FUNC = 2;        // Symbol is a code object
        const uint STT_SECTION = 3;     // Symbol associated with a section
        const uint STT_FILE = 4;        // Symbol's name is file name
        const uint STT_COMMON = 5;      // Symbol is a common data object
        const uint STT_NUM = 6;         // Number of defined types. 
        const uint STT_LOOS = 10;       // Start of OS-specific
        const uint STT_GNU_IFUNC = 10;  // Symbol is an indirect code object (function dispatcher)
        const uint STT_HIOS = 12;       // End of OS-specific
        const uint STT_LOPROC = 13;     // Start of processor-specific
        const uint STT_HIPROC = 15;     // End of processor-specific

        int wordSize;
        
        List<Elf_SectionHeader> SectionHeaders;     // Copy of section headers
        int NSections;                              // Number of sections
        int SectionHeaderSize;                      // Size of each section header
        int SecStringTableLen;                      // Length of section header string table
        int SymbolTableOffset;                      // Offset to symbol table
        int SymbolTableEntrySize;                   // Entry size of symbol table
        int SymbolStringTableOffset;                // Offset to symbol string table
        int SymbolStringTableSize;                  // Size of symbol string table
        Dictionary<int, string> StringTable;        // String table for the section
        Elf_Hdr FileHeader;

        internal class Elf_Hdr
        {
            internal byte[] e_ident;       // Magic number and other info 16 bytes
            internal UInt16 e_type;        // Object file type
            internal UInt16 e_machine;     // Architecture
            internal uint e_version;       // Object file version
            internal UInt64 e_entry;       // Entry point virtual address
            internal UInt64 e_phoff;       // Program header table file offset
            internal UInt64 e_shoff;       // Section header table file offset
            internal uint e_flags;         // Processor-specific flags
            internal UInt16 e_ehsize;      // ELF header size in bytes
            internal UInt16 e_phentsize;   // Program header table entry size
            internal UInt16 e_phnum;       // Program header table entry count
            internal UInt16 e_shentsize;   // Section header table entry size
            internal UInt16 e_shnum;       // Section header table entry count
            internal UInt16 e_shstrndx;    // Section header string table index
        };


        internal class Elf_SectionHeader
        {
            internal uint sh_name;          // Section name (string tbl index)
            internal uint sh_type;          // Section type
            internal UInt64 sh_flags;       // Section flags
            internal UInt64 sh_addr;        // Section virtual addr at execution
            internal UInt64 sh_offset;      // Section file offset
            internal UInt64 sh_size;        // Section size in bytes
            internal uint sh_link;          // Link to another section
            internal uint sh_info;          // Additional section information
            internal UInt64 sh_addralign;   // Section alignment
            internal UInt64 sh_entsize;     // Entry size if section holds table
        };


        struct Elf_Phdr
        {
            uint p_type;        // Segment type
            uint p_flags;       // Segment flags
            UInt64 p_offset;    // Segment file offset
            UInt64 p_vaddr;     // Segment virtual address
            UInt64 p_paddr;     // Segment physical address
            UInt64 p_filesz;    // Segment size in file
            UInt64 p_memsz;     // Segment size in memory
            UInt64 p_align;     // Segment alignment 
        };

        struct Elf_Symbol
        {
            UInt32 st_name;       // Symbol name (string tbl index)
            byte st_type;    // Symbol type 4 bits
            byte st_bind;    // Symbol binding 4 bits
            byte st_other;      // Symbol visibility
            UInt16 st_shndx;      // Section index
            UInt64 st_value;      // Symbol value
            UInt64 st_size;       // Symbol size
        };



        public Elf(int size)
        {
            wordSize = size;
            fileParsed = false;
            StringTable = new Dictionary<int, string>();
            SectionHeaders = new List<Elf_SectionHeader>();
        }


        internal Elf_Hdr GetElfFileHeader(int offset)
        {
            Elf_Hdr header = new Elf_Hdr();

            header.e_ident = new byte[16];     
            Buffer.BlockCopy(dataStream, offset, header.e_ident, 0, 16);        // Magic number and other info 16 bytes length
            offset += 16;
            header.e_type = BitConverter.ToUInt16(dataStream, offset);          // Object file type
            offset += 2;
            header.e_machine = BitConverter.ToUInt16(dataStream, offset);       // Architecture
            offset += 2;
            header.e_version = BitConverter.ToUInt32(dataStream, offset);       // Object file version
            offset += 4;
            if (wordSize == 64)
            {
                header.e_entry = BitConverter.ToUInt64(dataStream, offset);     // Entry point virtual address
                offset += 8;
                header.e_phoff = BitConverter.ToUInt64(dataStream, offset);     // Program header table file offset
                offset += 8;
                header.e_shoff = BitConverter.ToUInt64(dataStream, offset);     // Section header table file offset
                offset += 8;
            }
            else
            {
                header.e_entry = BitConverter.ToUInt32(dataStream, offset);     // Entry point virtual address
                offset += 4;
                header.e_phoff = BitConverter.ToUInt32(dataStream, offset);     // Program header table file offset
                offset += 4;
                header.e_shoff = BitConverter.ToUInt32(dataStream, offset);     // Section header table file offset
                offset += 4;
            }
            header.e_flags = BitConverter.ToUInt32(dataStream, offset);         // Processor-specific flags
            offset += 4;
            header.e_ehsize = BitConverter.ToUInt16(dataStream, offset);        // ELF header size in bytes
            offset += 2;
            header.e_phentsize = BitConverter.ToUInt16(dataStream, offset);     // Program header table entry size
            offset += 2;
            header.e_phnum = BitConverter.ToUInt16(dataStream, offset);         // Program header table entry count
            offset += 2;
            header.e_shentsize = BitConverter.ToUInt16(dataStream, offset);     // Section header table entry size
            offset += 2;
            header.e_shnum = BitConverter.ToUInt16(dataStream, offset);         // Section header table entry count
            offset += 2;
            header.e_shstrndx = BitConverter.ToUInt16(dataStream, offset);      // Section header string table index

            return header;
        }

        internal Elf_SectionHeader GetSectionHeader(int offset)
        {
            Elf_SectionHeader sectionheader = new Elf_SectionHeader();

            sectionheader.sh_name = BitConverter.ToUInt16(dataStream, offset);      // Section name (string tbl index)
            sectionheader.sh_type = BitConverter.ToUInt16(dataStream, offset);      // Section type
            sectionheader.sh_flags = BitConverter.ToUInt16(dataStream, offset);     // Section flags
            sectionheader.sh_addr = BitConverter.ToUInt16(dataStream, offset);      // Section virtual addr at execution
            sectionheader.sh_offset = BitConverter.ToUInt16(dataStream, offset);    // Section file offset
            sectionheader.sh_size = BitConverter.ToUInt16(dataStream, offset);      // Section size in bytes
            sectionheader.sh_link = BitConverter.ToUInt16(dataStream, offset);      // Link to another section
            sectionheader.sh_info = BitConverter.ToUInt16(dataStream, offset);      // Additional section information
            sectionheader.sh_addralign = BitConverter.ToUInt16(dataStream, offset); // Section alignment
            sectionheader.sh_entsize = BitConverter.ToUInt16(dataStream, offset);   // Entry size if section holds table

            return sectionheader;
        }

        void ParseFile()
        {
            if (fileParsed == true)
            {
                return;
            }
            // Load and parse file buffer
            int i;
            FileHeader = GetElfFileHeader(0);   // Copy file header
            NSections = FileHeader.e_shnum;

            int SymbolIndex = 0;                  // Index to symbol table

            // check header integrity
            if (((int)FileHeader.e_phoff > GetDataSize()) || ((int)(FileHeader.e_phoff + FileHeader.e_phentsize) > GetDataSize()))
            {
                Console.WriteLine("Pointer out of range in object file");
            }

            // Find section headers
            SectionHeaderSize = FileHeader.e_shentsize;
            if (SectionHeaderSize <= 0)
            {
                Console.WriteLine("Error in ELF file. Record size not specified");
                return;
            }
            int SectionOffset = (int)FileHeader.e_shoff;

            for (i = 0; i < NSections; i++)
            {
                SectionHeaders.Add(GetSectionHeader(SectionOffset));
                // check section header integrity
                if (SectionHeaders[i].sh_type != SHT_NOBITS && (((int)SectionHeaders[i].sh_offset > GetDataSize())
                    || ((int)(SectionHeaders[i].sh_offset + SectionHeaders[i].sh_size) > GetDataSize())
                    || ((int)(SectionHeaders[i].sh_offset + SectionHeaders[i].sh_entsize) > GetDataSize())))
                {
                    Console.WriteLine("Pointer out of range in object file");
                }
                SectionOffset += SectionHeaderSize;
                if (SectionHeaders[i].sh_type == SHT_SYMTAB)
                {
                    // Symbol table found
                    SymbolIndex = i;
                }
            }

 //           SecStringTable = uint(SectionHeaders[FileHeader.e_shstrndx].sh_offset);
 //           SecStringTableLen = (int)SectionHeaders[FileHeader.e_shstrndx].sh_size;


            if (SectionOffset > GetDataSize())
            {
                Console.WriteLine("Pointer out of range in object file");
            }
            if (SymbolIndex > 0)
            {
                // Save offset to symbol table
                SymbolTableOffset = (int)(SectionHeaders[SymbolIndex].sh_offset);
                SymbolTableEntrySize = (int)SectionHeaders[SymbolIndex].sh_entsize; // Entry size of symbol table
                if (SymbolTableEntrySize == 0)
                {
                    Console.WriteLine("Symbol table not found in ELF file");
                    return;
                } // Avoid division by zero

  //              SymbolTableEntries = (SectionHeaders[SymbolIndex].sh_size) / SymbolTableEntrySize;
                // Find associated string table
                int Stringtabi = (int)SectionHeaders[SymbolIndex].sh_link;
                if (Stringtabi < NSections)
                {
                    SymbolStringTableOffset = (int)(SectionHeaders[Stringtabi].sh_offset);
                    SymbolStringTableSize = (int)(SectionHeaders[Stringtabi].sh_size);
                }
                else
                {
                    SymbolIndex = 0;  // Error
                }
            }
            fileParsed = true;
        }



        internal List<string> GetPublicNames()
        {
            // Make list of public names in object file
            List<string> publicNames = new List<string>();

            // Interpret header:
            ParseFile();

            // Loop through section headers
            for (int sc = 0; sc < NSections; sc++)
            {
                // Get copy of 32-bit header or converted 64-bit header
                Elf_SectionHeader sheader = SectionHeaders[sc];
                UInt64 entrysize = sheader.sh_entsize;

                if (sheader.sh_type == SHT_SYMTAB || sheader.sh_type == SHT_DYNSYM)
                {
                    // Dump symbol table

                    // Find associated string table
                    if (sheader.sh_link >= NSections)
                    {
                       // err.submit(2035);
                        sheader.sh_link = 0;
                    }
/*
                    int8* strtab = Buf() + uint32(SectionHeaders[sheader.sh_link].sh_offset);

                    // Find symbol table
                    uint32 symtabsize = uint32(sheader.sh_size);
                    int8* symtab = Buf() + uint32(sheader.sh_offset);
                    int8* symtabend = symtab + symtabsize;
                    if (entrysize < sizeof(Elf_Symbol))
                    {
                        err.submit(2033);
                        entrysize = sizeof(Elf_Symbol);
                    }

                    // Loop through symbol table
                    for (int symi = 0; symtab < symtabend; symtab += entrysize, symi++)
                    {
                        // Copy 32 bit symbol table entry or convert 64 bit entry
                        Elf_Symbol sym = *(Elf_Symbol) symtab;
                        int type = sym.st_type;
                        int binding = sym.st_bind;
                        if((int16(sym.st_shndx) > 0) && (type != STT_SECTION) && (type != STT_FILE) && (binding == STB_GLOBAL || binding == STB_WEAK))
                        {
                            // Public symbol found
                            SStringEntry se;
                            se.Member = m;
                            // Store name
                            se.String = Strings->PushString(strtab + sym.st_name);

                            // Store name index
                            publicNames.Add(se);

                        }
                    }
                    */
                }
            }
            return publicNames;
        }

    }
}
