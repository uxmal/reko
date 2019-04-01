using System;
using System.Collections.Generic;

namespace RekoMakeSigs
{
    internal class Coff : DataBuffer
    {
        //********************* Storage classes for symbol table entries **********************
        const byte COFF_CLASS_NULL =                   0;
        const byte COFF_CLASS_AUTOMATIC =              1; // automatic variable
        const byte COFF_CLASS_EXTERNAL =               2; // external symbol 
        const byte COFF_CLASS_STATIC =                 3; // static
        const byte COFF_CLASS_REGISTER =               4; // register variable
        const byte COFF_CLASS_EXTERNAL_DEF =           5; // external definition 
        const byte COFF_CLASS_LABEL =                  6; // label
        const byte COFF_CLASS_UNDEFINED_LABEL =        7; // undefined label
        const byte COFF_CLASS_MEMBER_OF_STRUCTURE =    8; // member of structure
        const byte COFF_CLASS_ARGUMENT =               9; // function argument
        const byte COFF_CLASS_STRUCTURE_TAG =         10; // structure tag 
        const byte COFF_CLASS_MEMBER_OF_UNION =       11; // member of union 
        const byte COFF_CLASS_UNION_TAG =             12; // union tag 
        const byte COFF_CLASS_TYPE_DEFINITION =       13; // type definition
        const byte COFF_CLASS_UNDEFINED_STATIC =      14; // undefined static 
        const byte COFF_CLASS_ENUM_TAG =              15; // enumeration tag 
        const byte COFF_CLASS_MEMBER_OF_ENUM =        16; // member of enumeration
        const byte COFF_CLASS_REGISTER_PARAM =        17; // register parameter
        const byte COFF_CLASS_BIT_FIELD =             18; // bit field  
        const byte COFF_CLASS_AUTO_ARGUMENT =         19; // auto argument 
        const byte COFF_CLASS_LASTENTRY =             20; // dummy entry (end of block)
        const byte COFF_CLASS_BLOCK =                100; // ".bb" or ".eb" 
        const byte COFF_CLASS_FUNCTION =             101; // ".bf" or ".ef" 
        const byte COFF_CLASS_END_OF_STRUCT =        102; // end of structure 
        const byte COFF_CLASS_FILE =                 103; // file name  
        const byte COFF_CLASS_LINE =                 104; // line # reformatted as symbol table entry 
        const byte COFF_CLASS_SECTION =              104; // line # reformatted as symbol table entry
        const byte COFF_CLASS_ALIAS =                105; // duplicate tag 
        const byte COFF_CLASS_WEAK_EXTERNAL =        105; // duplicate tag  
        const byte COFF_CLASS_HIDDEN =               106; // ext symbol in dmert public lib 
        const byte COFF_CLASS_END_OF_FUNCTION =     0xff; // physical end of function 

        //********************* Type for symbol table entries **********************
        const byte COFF_TYPE_FUNCTION =             0x20; // Symbol is function
        const byte COFF_TYPE_NOT_FUNCTION =         0x00; // Symbol is not a function


        //********************* Section number values for symbol table entries **********************
        const short COFF_SECTION_UNDEF = 0;         // external symbol
        const short COFF_SECTION_ABSOLUTE = -1;     // value of symbol is absolute
        const short COFF_SECTION_DEBUG = -2;        // debugging symbol - value is meaningless
        const short COFF_SECTION_N_TV = -3;         // indicates symbol needs preload transfer vector
        const short COFF_SECTION_P_TV = -4;         // indicates symbol needs postload transfer vector
        const short COFF_SECTION_REMOVE_ME = -99;   // Specific for objconv program: Debug or exception section being removed

        internal struct SCOFF_SectionHeader
        {
            internal string Name;        // section name char Name[8]; 
            internal uint VirtualSize;    // size of section when loaded. (Should be 0 for object files, but it seems to be accumulated size of all sections)
            internal uint VirtualAddress; // subtracted from offsets during relocation. preferably 0
            internal uint SizeOfRawData;  // section size in file
            internal uint PRawData;       // file  to raw data for section
            internal uint PRelocations;   // file  to relocation entries
            internal uint PLineNumbers;   // file  to line number entries
            internal ushort NRelocations;   // number of relocation entries
            internal ushort NLineNumbers;   // number of line number entries
            internal uint Flags;          // flags   
        };
        const int SCOFF_SectionHeaderSize = 40;

        internal struct SCOFF_FileHeader
        {
            internal ushort Machine;              // Machine ID (magic number)
            internal ushort NumberOfSections;     // number of sections
            internal uint TimeDateStamp;        // time & date stamp 
            internal uint PSymbolTable;         // file pointer to symbol table
            internal uint NumberOfSymbols;      // number of symbol table entries 
            internal ushort SizeOfOptionalHeader; // size of optional header
            internal ushort Flags;                // Flags indicating attributes
        };
        const int SCOFF_FileHeaderSize = 20;

        internal struct SCOFF_IMAGE_DATA_DIRECTORY
        {
            internal uint VirtualAddress;              // Image relative address of table
            internal uint Size;                        // Size of table
        };
        const int SCOFF_IMAGE_DATA_DIRECTORY_Size = 8;

        const int SIZE_SCOFF_SymTableEntry = 18;  // Size of SCOFF_SymTableEntry packed

        internal struct SCOFF_OptionalHeader
        {
            internal ushort Magic;                    // Magic number
            internal byte LinkerMajorVersion;
            internal byte LinkerMinorVersion;
            internal uint SizeOfCode;
            internal uint SizeOfInitializedData;
            internal uint SizeOfUninitializedData;
            internal uint AddressOfEntryPoint;      // Entry point relative to image base
            internal uint BaseOfCode;
            // Windows specific fields
            internal Int64 ImageBase;                // Image base
            internal uint SectionAlignment;
            internal uint FileAlignment;
            internal ushort MajorOperatingSystemVersion;
            internal ushort MinorOperatingSystemVersion;
            internal ushort MajorImageVersion;
            internal ushort MinorImageVersion;
            internal ushort MajorSubsystemVersion;
            internal ushort MinorSubsystemVersion;
            internal uint Win32VersionValue;        // must be 0
            internal uint SizeOfImage;
            internal uint SizeOfHeaders;
            internal uint CheckSum;
            internal ushort Subsystem;
            internal ushort DllCharacteristics;
            internal UInt64 SizeOfStackReserve;
            internal UInt64 SizeOfStackCommit;
            internal UInt64 SizeOfHeapReserve;
            internal UInt64 SizeOfHeapCommit;
            internal uint LoaderFlags;              // 0
            internal uint NumberOfRvaAndSizes;
            // Data directories
            internal SCOFF_IMAGE_DATA_DIRECTORY ExportTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY ImportTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY ResourceTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY ExceptionTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY CertificateTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY BaseRelocationTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY Debug;
            internal SCOFF_IMAGE_DATA_DIRECTORY Architecture;   // 0
            internal SCOFF_IMAGE_DATA_DIRECTORY GlobalPtr;      // 0
            internal SCOFF_IMAGE_DATA_DIRECTORY TLSTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY LoadConfigTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY BoundImportTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY ImportAddressTable;
            internal SCOFF_IMAGE_DATA_DIRECTORY DelayImportDescriptor;
            internal SCOFF_IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
            internal SCOFF_IMAGE_DATA_DIRECTORY Reserved;       // 0
        };


        internal struct SCOFF_SymTableEntry
        {
            internal SCOFF_SymTableEntry_S s;
            internal SCOFF_SymTableEntry_func func;
            internal SCOFF_SymTableEntry_bfef bfef;
            internal SCOFF_SymTableEntry_weak weak;
            internal SCOFF_SymTableEntry_filename filename;
            internal SCOFF_SymTableEntry_section section;
        }

        // Normal symbol table entry
        internal class SCOFF_SymTableEntry_S
        {
            internal string Name;
            internal uint Value;
            internal Int16 SectionNumber;
            internal UInt16 Type;
            internal byte StorageClass;
            internal byte NumAuxSymbols;
        }

        // Auxiliary symbol table entry types:
        //************************************

        // Function definition
        internal class SCOFF_SymTableEntry_func
        {
            internal uint TagIndex; // Index to .bf entry
            internal uint TotalSize; // Size of function code
            internal uint PointerToLineNumber; // Pointer to line number entry
            internal uint PointerToNextFunction; // Symbol table index of next function
            internal UInt16 x_tvndx;      // Unused
        }

        // .bf abd .ef
        internal class SCOFF_SymTableEntry_bfef
        {
            internal uint Unused1;
            internal UInt16 SourceLineNumber; // Line number in source file
            internal UInt16 Unused2;
            internal uint Unused3; // Pointer to line number entry
            internal uint PointerToNextFunction; // Symbol table index of next function
        }

        // Weak external
        internal class SCOFF_SymTableEntry_weak
        {
            internal uint TagIndex; // Symbol table index of alternative symbol2
            internal uint Characteristics; //
            internal uint Unused1;
            internal uint Unused2;
            internal UInt16 Unused3;      // Unused
        }

        // File name
        internal class SCOFF_SymTableEntry_filename
        {
            internal string FileName;    // File name
        }


        // Section definition
        internal class SCOFF_SymTableEntry_section
        {
            internal uint Length;
            internal UInt16 NumberOfRelocations;    // Line number in source file
            internal UInt16 NumberOfLineNumbers;
            internal uint CheckSum;                 // Pointer to line number entry
            internal UInt16 Number;                 // Symbol table index of next function
            internal byte Selection;                // Unused
        }


        List<SCOFF_SectionHeader> SectionHeaders;   // Copy of section headers
        UInt16 NSections;                           // Number of sections
        SCOFF_FileHeader FileHeader;                // File header
        List<SCOFF_SymTableEntry> SymbolTable;      // Pointer to symbol table (for object files)
        Dictionary<int,string> StringTable;         // Pointer to string table (for object files)
        uint StringTableSize;                       // Size of string table (for object files)
        int NumberOfSymbols;                        // Number of symbol table entries (for object files)
        Int64 ImageBase;                            // Image base (for executable files)
        SCOFF_OptionalHeader OptionalHeader;        // Optional header (for executable files)
        //SCOFF_IMAGE_DATA_DIRECTORY pImageDirs;    // Pointer to image directories (for executable files)
        uint NumImageDirs;                          // Number of image directories (for executable files)
        uint EntryPoint;                            // Entry point (for executable files)


        public Coff()
        {
            SectionHeaders = new List<SCOFF_SectionHeader>();
            SymbolTable = new List<SCOFF_SymTableEntry>();
            StringTable = new Dictionary<int, string>();
            fileParsed = false;
        }


        internal SCOFF_FileHeader GetCoffFileHeader(int offset)
        {
            SCOFF_FileHeader header;

            header.Machine = BitConverter.ToUInt16(dataStream, offset);                     // Machine ID (magic number)
            header.NumberOfSections = BitConverter.ToUInt16(dataStream, offset + 2);        // number of sections
            header.TimeDateStamp = BitConverter.ToUInt32(dataStream, offset + 4);           // time & date stamp 
            header.PSymbolTable = BitConverter.ToUInt32(dataStream, offset + 8);            // file pointer to symbol table
            header.NumberOfSymbols = BitConverter.ToUInt32(dataStream, offset + 12);        // number of symbol table entries 
            header.SizeOfOptionalHeader = BitConverter.ToUInt16(dataStream, offset + 16);   // size of optional header
            header.Flags = BitConverter.ToUInt16(dataStream, offset + 18);
            return header;
        }

        internal SCOFF_SectionHeader GetCoffSectionHeader(int offset)
        {
            SCOFF_SectionHeader header;

            header.Name = System.Text.Encoding.UTF8.GetString(dataStream, offset, 8);   // section name char Name[8]; 
            string tmp = header.Name.Replace('\0', ' ');
            char[] charsToTrim = { ',', '.', ' ' };
            header.Name = tmp.TrimEnd(charsToTrim);
            header.VirtualSize = BitConverter.ToUInt32(dataStream, offset + 8);         // size of section when loaded. (Should be 0 for object files, but it seems to be accumulated size of all sections)
            header.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 12);     // subtracted from offsets during relocation. preferably 0
            header.SizeOfRawData = BitConverter.ToUInt32(dataStream, offset + 16);      // section size in file
            header.PRawData = BitConverter.ToUInt32(dataStream, offset + 20);           // file  to raw data for section
            header.PRelocations = BitConverter.ToUInt32(dataStream, offset + 24);       // file  to relocation entries
            header.PLineNumbers = BitConverter.ToUInt32(dataStream, offset + 28);       // file  to line number entries
            header.NRelocations = BitConverter.ToUInt16(dataStream, offset + 32);       // number of relocation entries
            header.NLineNumbers = BitConverter.ToUInt16(dataStream, offset + 34);       // number of line number entries
            header.Flags = BitConverter.ToUInt32(dataStream, offset + 36);              // flags  

            return header;
        }

        internal SCOFF_OptionalHeader GetCoffOptionHeader(int offset)
        {
            SCOFF_OptionalHeader header;

            header.Magic = BitConverter.ToUInt16(dataStream, offset + 36);                    // Magic number
            header.LinkerMajorVersion = dataStream[0];
            header.LinkerMinorVersion = dataStream[0];
            header.SizeOfCode = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfInitializedData = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfUninitializedData = BitConverter.ToUInt32(dataStream, offset + 36);
            header.AddressOfEntryPoint = BitConverter.ToUInt32(dataStream, offset + 36);      // Entry point relative to image base
            header.BaseOfCode = BitConverter.ToUInt32(dataStream, offset + 36);
            // Windows specific fields
            header.ImageBase = BitConverter.ToUInt32(dataStream, offset + 36);                // Image base
            header.SectionAlignment = BitConverter.ToUInt32(dataStream, offset + 36);
            header.FileAlignment = BitConverter.ToUInt32(dataStream, offset + 36);
            header.MajorOperatingSystemVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.MinorOperatingSystemVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.MajorImageVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.MinorImageVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.MajorSubsystemVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.MinorSubsystemVersion = BitConverter.ToUInt16(dataStream, offset + 36);
            header.Win32VersionValue = BitConverter.ToUInt32(dataStream, offset + 36);        // must be 0
            header.SizeOfImage = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfHeaders = BitConverter.ToUInt32(dataStream, offset + 36);
            header.CheckSum = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Subsystem = BitConverter.ToUInt16(dataStream, offset + 36);
            header.DllCharacteristics = BitConverter.ToUInt16(dataStream, offset + 36);
            header.SizeOfStackReserve = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfStackCommit = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfHeapReserve = BitConverter.ToUInt32(dataStream, offset + 36);
            header.SizeOfHeapCommit = BitConverter.ToUInt32(dataStream, offset + 36);
            header.LoaderFlags = BitConverter.ToUInt32(dataStream, offset + 36);              // 0
            header.NumberOfRvaAndSizes = BitConverter.ToUInt32(dataStream, offset + 36);
            // Data directories
            header.ExportTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ExportTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ImportTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ImportTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ResourceTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ResourceTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ExceptionTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ExceptionTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.CertificateTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.CertificateTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.BaseRelocationTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.BaseRelocationTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Debug.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Debug.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Architecture.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Architecture.Size = BitConverter.ToUInt32(dataStream, offset + 36);// 0
            header.GlobalPtr.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.GlobalPtr.Size = BitConverter.ToUInt32(dataStream, offset + 36);       // 0
            header.TLSTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.TLSTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.LoadConfigTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.LoadConfigTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.BoundImportTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.BoundImportTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ImportAddressTable.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.ImportAddressTable.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.DelayImportDescriptor.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.DelayImportDescriptor.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.CLRRuntimeHeader.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);
            header.CLRRuntimeHeader.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            header.Reserved.VirtualAddress = BitConverter.ToUInt32(dataStream, offset + 36);   
            header.Reserved.Size = BitConverter.ToUInt32(dataStream, offset + 36);
            return header;
        }


        void GetSymbolTables(int offset)
        {
            SymbolTable.Clear();

            for (int index = 0; index < NumberOfSymbols; index++)
            {
                SCOFF_SymTableEntry entry = new SCOFF_SymTableEntry();
                SCOFF_SymTableEntry_S entry_s = new SCOFF_SymTableEntry_S();

                int stringOffset = BitConverter.ToInt32(dataStream, offset );

                if (stringOffset == 0)
                {
                    // Need to get the entry point into the string table
                    stringOffset = BitConverter.ToInt32(dataStream, offset + 4);
                    entry_s.Name = StringTable[stringOffset];
                }
                else
                {
                    entry_s.Name = System.Text.Encoding.UTF8.GetString(dataStream, offset, 8);  // section name char Name[8]; 
                    string tmp = entry_s.Name.Replace('\0', ' ');
                    char[] charsToTrim = { ',', '.', ' ' };
                    entry_s.Name = tmp.TrimEnd(charsToTrim);
                }
                

                entry_s.Value = BitConverter.ToUInt32(dataStream, offset + 8);
                entry_s.SectionNumber = BitConverter.ToInt16(dataStream, offset + 12);
                entry_s.Type = BitConverter.ToUInt16(dataStream, offset + 14);
                entry_s.StorageClass = dataStream[offset + 16];
                entry_s.NumAuxSymbols = dataStream[offset + 17];

                offset += 18;
                entry.s = entry_s;


                int jsym = 0;
                while (jsym < entry_s.NumAuxSymbols && index < NumberOfSymbols)
                {
                    // Detect auxiliary entry type
                    if (entry_s.StorageClass == COFF_CLASS_EXTERNAL
                       && entry_s.Type == COFF_TYPE_FUNCTION
                       && entry_s.SectionNumber > 0)
                    {
                        // This is a function definition aux record
                        SCOFF_SymTableEntry_func entry_func = new SCOFF_SymTableEntry_func();

                        entry_func.TagIndex = BitConverter.ToUInt32(dataStream, offset);                    // Index to .bf entry
                        entry_func.TotalSize = BitConverter.ToUInt32(dataStream, offset + 4);               // Size of function code
                        entry_func.PointerToLineNumber = BitConverter.ToUInt32(dataStream, offset + 8);     // Pointer to line number entry
                        entry_func.PointerToNextFunction = BitConverter.ToUInt32(dataStream, offset + 12);  // Symbol table index of next function
                        entry_func.x_tvndx = BitConverter.ToUInt16(dataStream, offset + 16);                // Unused
                        entry.func = entry_func;
                    }
                    else if ((entry.s.Name == ".bf") || (entry.s.Name == ".ef"))
                    {
                        // This is a .bf or .ef aux record
                        SCOFF_SymTableEntry_bfef entry_bfef = new SCOFF_SymTableEntry_bfef();
                        entry_bfef.Unused1 = BitConverter.ToUInt32(dataStream, offset);
                        entry_bfef.SourceLineNumber = BitConverter.ToUInt16(dataStream, offset + 4);        // Line number in source file
                        entry_bfef.Unused2 = BitConverter.ToUInt16(dataStream, offset + 8);
                        entry_bfef.Unused3 = BitConverter.ToUInt32(dataStream, offset + 10);                // Pointer to line number entry
                        entry_bfef.PointerToNextFunction = BitConverter.ToUInt32(dataStream, offset + 14);
                        entry.bfef = entry_bfef;
                    }
                    else if (entry_s.StorageClass == COFF_CLASS_EXTERNAL &&
                       entry_s.SectionNumber == COFF_SECTION_UNDEF &&
                       entry_s.Value == 0)
                    {
                        // This is a Weak external aux record
                        SCOFF_SymTableEntry_weak entry_weak = new SCOFF_SymTableEntry_weak();

                        entry_weak.TagIndex = BitConverter.ToUInt32(dataStream, offset);                    // Symbol table index of alternative symbol2
                        entry_weak.Characteristics = BitConverter.ToUInt32(dataStream, offset + 4); 
                        entry_weak.Unused1 = BitConverter.ToUInt32(dataStream, offset + 8);
                        entry_weak.Unused2 = BitConverter.ToUInt32(dataStream, offset + 12);
                        entry_weak.Unused3 = BitConverter.ToUInt16(dataStream, offset + 16);               // Unused

                        entry.weak = entry_weak;
                    }
                    else if (entry_s.StorageClass == COFF_CLASS_FILE)
                    {
                        // This is filename aux record. Contents has already been printed
                        SCOFF_SymTableEntry_filename entry_filename = new SCOFF_SymTableEntry_filename();

                        entry_filename.FileName = System.Text.Encoding.UTF8.GetString(dataStream, offset, 18);
                        string tmp = entry_filename.FileName.Replace('\0', ' ');
                        char[] charsToTrim = { ',', '.', ' ' };
                        entry_filename.FileName = tmp.TrimEnd(charsToTrim);

                        entry.filename = entry_filename;
                    }
                    else if (entry_s.StorageClass == COFF_CLASS_STATIC)
                    {
                        // This is section definition aux record
                        SCOFF_SymTableEntry_section entry_section = new SCOFF_SymTableEntry_section();

                        entry_section.Length = BitConverter.ToUInt32(dataStream, offset);
                        entry_section.NumberOfRelocations = BitConverter.ToUInt16(dataStream, offset + 4);      // Line number in source file
                        entry_section.NumberOfLineNumbers = BitConverter.ToUInt16(dataStream, offset + 6);
                        entry_section.CheckSum = BitConverter.ToUInt32(dataStream, offset + 8);                 // Pointer to line number entry
                        entry_section.Number = BitConverter.ToUInt16(dataStream, offset + 12);                  // Symbol table index of next function
                        entry_section.Selection = dataStream[offset + 14];                                      // Unused
                        entry.section = entry_section;
                    }
                    else if (entry_s.StorageClass == COFF_CLASS_ALIAS)
                    {
                        // This is section definition aux record
                        //printf("\n  Aux alias definition record:");
                        //printf("\n  symbol index: %i, ", sa->weak.TagIndex);
                        //switch (sa->weak.Characteristics)
                        //{
                        //    case IMAGE_WEAK_EXTERN_SEARCH_NOLIBRARY:
                        //        printf("no library search"); break;
                        //    case IMAGE_WEAK_EXTERN_SEARCH_LIBRARY:
                        //        printf("library search"); break;
                        //    case IMAGE_WEAK_EXTERN_SEARCH_ALIAS:
                        //        printf("alias symbol"); break;
                        //    default:
                        //        printf("unknown characteristics 0x%X", sa->weak.Characteristics);
                        //}
                    }
                    offset += 18;
                    index++;
                    jsym++;
                }
                SymbolTable.Add(entry);
            }

        }
        void ParseFile()
        {
            if(fileParsed == true)
            {
                return;
            }
            // Load and parse file buffer
            // Get offset to file header
            int FileHeaderOffset = 0;
            if ((GetUint16(0) & 0xFFF9) == 0x5A49)
            {
                // File has DOS stub
                uint Signature = GetUint32(0x3C);
                if (Signature + 8 < DataSize && GetUint16((int)Signature) == 0x4550)
                {
                    // Executable PE file
                    FileHeaderOffset = (int)(Signature + 4);
                }
                else
                {
                    Console.WriteLine("Objconv program internal inconsistency");
                    return;
                }
            }
            // Find file header
            FileHeader = GetCoffFileHeader(FileHeaderOffset);
            NSections = FileHeader.NumberOfSections;

            // check header integrity
            if ((FileHeader.PSymbolTable + (FileHeader.NumberOfSymbols * SIZE_SCOFF_SymTableEntry)) > GetDataSize())
            {
                Console.WriteLine("Pointer out of range in object file");
                //return;
            }

            // Find optional header if executable file
            if (FileHeader.SizeOfOptionalHeader > 0 && FileHeaderOffset > 0)
            {
                OptionalHeader = GetCoffOptionHeader(FileHeaderOffset + SCOFF_FileHeaderSize);

                // Find image data directories
                if (OptionalHeader.Magic == 0) //COFF_Magic_PE64)
                {
                    // 64 bit version
                    //pImageDirs = &(OptionalHeader.ExportTable);
                    NumImageDirs = OptionalHeader.NumberOfRvaAndSizes;
                    EntryPoint = OptionalHeader.AddressOfEntryPoint;
                    ImageBase = OptionalHeader.ImageBase;
                }
                else
                {
                    // 32 bit version
                    //pImageDirs = &(OptionalHeader.ExportTable);
                    NumImageDirs = OptionalHeader.NumberOfRvaAndSizes;
                    EntryPoint = OptionalHeader.AddressOfEntryPoint;
                    ImageBase = OptionalHeader.ImageBase;
                }

            }

            // Find section headers
            int SectionOffset = FileHeaderOffset + SCOFF_FileHeaderSize + FileHeader.SizeOfOptionalHeader;
            for (int i = 0; i < NSections; i++)
            {
                SectionHeaders.Add(GetCoffSectionHeader(SectionOffset));
                SectionOffset += SCOFF_SectionHeaderSize;
                // Check for _ILDATA section
                if (SectionHeaders[i].Name == "_ILDATA")
                {
                    // This is an intermediate file for Intel compiler
                    Console.WriteLine("This is an intermediate file for whole-program-optimization in Intel compiler");
                }
            }
            
            if (SectionOffset > GetDataSize())
            {
                Console.WriteLine("COFF file section table corrupt");
                return;             // Section table points to outside file
            }
            // Find symbol table
            NumberOfSymbols = (int)FileHeader.NumberOfSymbols;
            
            // Find string table
            int stringTableOffset = (int)(FileHeader.PSymbolTable + NumberOfSymbols * SIZE_SCOFF_SymTableEntry);
            StringTableSize = BitConverter.ToUInt32(dataStream, stringTableOffset);
            if(StringTableSize > 0)
            {
                int startPoint = stringTableOffset + 4;
                int CharCount = 0;
                int x = 4;
                while(x < StringTableSize)
                {
                    CharCount = 0;
                    while (dataStream[startPoint + CharCount] != 0)
                    {
                        CharCount++;
                        x++;
                    }
                    string strTmp = System.Text.Encoding.UTF8.GetString(dataStream, startPoint, CharCount);
                    if(strTmp != "")
                    {
                        StringTable.Add(startPoint - stringTableOffset, strTmp);
                    }

                    startPoint += CharCount;
                    startPoint++;
                    x++;
                }    
            }

            // Load the symbol tables
            GetSymbolTables((int)FileHeader.PSymbolTable);
            fileParsed = true;
        }



        internal List<string> GetPublicNames()
        {
            // Make list of public names in object file
            List<string> publicNames = new List<string>();

            // Interpret header:
            ParseFile();

            // Loop through symbol table
            foreach (SCOFF_SymTableEntry entry in SymbolTable)
            {
                // Search for public symbol
                if ((entry.s.SectionNumber > 0 && entry.s.StorageClass == COFF_CLASS_EXTERNAL)
                || entry.s.StorageClass == COFF_CLASS_ALIAS)
                {
                    publicNames.Add(entry.s.Name);
                }
            }
            return publicNames;
        }

       
        internal List<SignitureEntry> GetSignitures()
        {
            List<SignitureEntry> signitures = new List<SignitureEntry>();
            // Interpret header:
            ParseFile();

            int symbIndex = 0;

            while (symbIndex < SymbolTable.Count)        
            {
                // Search for method
                if ((SymbolTable[symbIndex].s.SectionNumber > 0) && (SymbolTable[symbIndex].s.Type == COFF_TYPE_FUNCTION))
                {
                    // Method found

                    // attempt to find the next method
                    int nextIndex = symbIndex + 1;
                    while (nextIndex < SymbolTable.Count)
                    {
                        // Search for method
                        if ((SymbolTable[nextIndex].s.SectionNumber > 0) && (SymbolTable[nextIndex].s.Type == COFF_TYPE_FUNCTION) && (SymbolTable[symbIndex].s.SectionNumber == SymbolTable[nextIndex].s.SectionNumber))
                        {
                            uint end = SymbolTable[nextIndex].s.Value;
                            uint start = SymbolTable[symbIndex].s.Value;
                            uint CodeOffset = SectionHeaders[SymbolTable[nextIndex].s.SectionNumber - 1].PRawData;     // Point at which the data starts
                            // Step back to find the end of the method (adjust for the align bytes)

                            while (end > start)
                            {
                                if (dataStream[CodeOffset + end] == 0xC3)
                                {
                                    //  have found the end of the method, so lets create a signture object
                                    SignitureEntry entry = new SignitureEntry();
                                    entry.name = SymbolTable[symbIndex].s.Name;
                                    entry.length = (int)(end - start) + 1;
                                    entry.data = new byte[entry.length];
                                    Buffer.BlockCopy(dataStream, (int)(CodeOffset + start), entry.data, 0, entry.length);
                                    nextIndex = SymbolTable.Count;
                                    signitures.Add(entry);
                                    break;
                                }
                                end--;
                            }
                        }
                        nextIndex++;
                    }  
                }
                symbIndex++;
            }
            return signitures;
        }
    }
}
