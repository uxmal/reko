using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RekoMakeSigs
{
    internal class Macho : DataBuffer
    {

        // Constant for the magic field of the MAC_header (32-bit architectures)
        const UInt32 MAC_MAGIC_32 = 0xFEEDFACE;  // 32 bit little endian
        const UInt32 MAC_MAGIC_64 = 0xFEEDFACF;  // 64 bit little endian
        const UInt32 MAC_CIGAM_32 = 0xCEFAEDFE;  // 32 bit big endian
        const UInt32 MAC_CIGAM_64 = 0xCFFAEDFE;  // 64 bit big endian
        const UInt32 MAC_CIGAM_UNIV = 0xBEBAFECA;  // MacIntosh universal binary

        // Constants for cputype
        const UInt32 MAC_CPU_TYPE_I386 = 7;
        const UInt32 MAC_CPU_TYPE_X86_64 = 0x1000007;
        const UInt32 MAC_CPU_TYPE_ARM = 12;
        const UInt32 MAC_CPU_TYPE_SPARC = 14;
        const UInt32 MAC_CPU_TYPE_POWERPC = 18;
        const UInt32 MAC_CPU_TYPE_POWERPC64 = 0x1000012;

        // Constants for cpusubtype
        const UInt32 MAC_CPU_SUBTYPE_I386_ALL = 3;
        const UInt32 MAC_CPU_SUBTYPE_X86_64_ALL = 3;
        const UInt32 MAC_CPU_SUBTYPE_ARM_ALL = 0;
        const UInt32 MAC_CPU_SUBTYPE_SPARC_ALL = 0;
        const UInt32 MAC_CPU_SUBTYPE_POWERPC_ALL = 0;

        // Constants for the filetype field of the MAC_header
        const UInt32 MAC_OBJECT = 0x1;      // relocatable object file 
        const UInt32 MAC_EXECUTE = 0x2;     // demand paged executable file 
        const UInt32 MAC_FVMLIB = 0x3;      // fixed VM shared library file 
        const UInt32 MAC_CORE = 0x4;        // core file *
        const UInt32 MAC_PRELOAD = 0x5;     // preloaded executable file 
        const UInt32 MAC_DYLIB = 0x6;       // dynamicly bound shared library file
        const UInt32 MAC_DYLINKER = 0x7;    // dynamic link editor 
        const UInt32 MAC_BUNDLE = 0x8;      // dynamicly bound bundle file 

        // Constants for the flags field of the MAC_header
        const UInt32 MAC_NOUNDEFS = 0x1;                    // the object file has no undefined references, can be executed
        const UInt32 MAC_INCRLINK = 0x2;                    // the object file is the output of an incremental link against a base file and can't be link edited again
        const UInt32 MAC_DYLDLINK = 0x4;                    // the object file is input for the dynamic linker and can't be staticly link edited again
        const UInt32 MAC_BINDATLOAD = 0x8;                  // the object file's undefined references are bound by the dynamic linker when loaded.
        const UInt32 MAC_PREBOUND = 0x10;                   // the file has it's dynamic undefined references prebound.
        const UInt32 MAC_SPLIT_SEGS = 0x20;                 // the file has its read-only and read-write segments split
        const UInt32 MAC_LAZY_INIT = 0x40;                  // the shared library init routine is to be run lazily via catching memory faults to its writeable segments (obsolete)
        const UInt32 MAC_TWOLEVEL = 0x80;                   // the image is using two-level name space bindings
        const UInt32 MAC_FORCE_FLAT = 0x100;                // the executable is forcing all images to use flat name space bindings
        const UInt32 MAC_NOMULTIDEFS = 0x200;               // this umbrella guarantees no multiple defintions of symbols in its sub-images so the two-level namespace hints can always be used
        const UInt32 MAC_NOFIXPREBINDING = 0x400;           // do not have dyld notify the prebinding agent about this executable
        const UInt32 MAC_PREBINDABLE = 0x800;               // the binary is not prebound but can have its prebinding redone. only used when MH_PREBOUND is not set
        const UInt32 MAC_ALLMODSBOUND = 0x1000;             // indicates that this binary binds to all two-level namespace modules of its dependent libraries. only used when MH_PREBINDABLE and MH_TWOLEVEL are both set
        const UInt32 MAC_SUBSECTIONS_VIA_SYMBOLS = 0x2000;  // safe to divide up the sections into sub-sections via symbols for dead code stripping
        const UInt32 MAC_CANONICAL = 0x4000;                // the binary has been canonicalized via the unprebind operation

        // Constants for the cmd field of all load commands, the type
        const UInt32 MAC_LC_REQ_DYLD = 0x80000000;                          // This bit is added if unknown command cannot be ignored
        const UInt32 MAC_LC_SEGMENT = 0x1;                                  // segment of this file to be mapped 
        const UInt32 MAC_LC_SYMTAB = 0x2;	                                // link-edit stab symbol table info 
        const UInt32 MAC_LC_SYMSEG = 0x3;	                                // link-edit gdb symbol table info (obsolete) 
        const UInt32 MAC_LC_THREAD = 0x4;	                                // thread 
        const UInt32 MAC_LC_UNIXTHREAD = 0x5;	                            // unix thread (includes a stack)
        const UInt32 MAC_LC_LOADFVMLIB = 0x6;	                            // load a specified fixed VM shared library
        const UInt32 MAC_LC_IDFVMLIB = 0x7;	                                // fixed VM shared library identification
        const UInt32 MAC_LC_IDENT = 0x8;	                                // object identification info (obsolete)
        const UInt32 MAC_LC_FVMFILE = 0x9;	                                // fixed VM file inclusion (internal use)
        const UInt32 MAC_LC_PREPAGE = 0xa;                                  // prepage command (internal use)
        const UInt32 MAC_LC_DYSYMTAB = 0xb;	                                // dynamic link-edit symbol table info
        const UInt32 MAC_LC_LOAD_DYLIB = 0xc;	                            // load a dynamicly linked shared library
        const UInt32 MAC_LC_ID_DYLIB = 0xd;	                                // dynamicly linked shared lib identification
        const UInt32 MAC_LC_LOAD_DYLINKER = 0xe;	                        // load a dynamic linker
        const UInt32 MAC_LC_ID_DYLINKER = 0xf;	                            // dynamic linker identification
        const UInt32 MAC_LC_PREBOUND_DYLIB = 0x10;	                        // modules prebound for a dynamicly linked shared library
        const UInt32 MAC_LC_ROUTINES = 0x11;	                            // image routines
        const UInt32 MAC_LC_SUB_FRAMEWORK = 0x12;                           // sub framework
        const UInt32 MAC_LC_SUB_UMBRELLA = 0x13;                            // sub umbrella 
        const UInt32 MAC_LC_SUB_CLIENT = 0x14;                              // sub client 
        const UInt32 MAC_LC_SUB_LIBRARY = 0x15;                             // sub library
        const UInt32 MAC_LC_TWOLEVEL_HINTS = 0x16;                          // two-level namespace lookup hints
        const UInt32 MAC_LC_PREBIND_CKSUM = 0x17;                           // prebind checksum 
        const UInt32 MAC_LC_LOAD_WEAK_DYLIB = (0x18 | MAC_LC_REQ_DYLD);
        const UInt32 MAC_LC_SEGMENT_64 = 0x19;                              // 64-bit segment of this file to be mapped 
        const UInt32 MAC_LC_ROUTINES_64 = 0x1a;                             // 64-bit image routines
        const UInt32 MAC_LC_UUID = 0x1b;                                    // the uuid

        internal struct MACHO_FileHeader
        {
            internal UInt32 Magic;		// mach magic number identifier
            internal UInt32 Cputype;	   // cpu specifier
            internal UInt32 Cpusubtype;	// machine specifier
            internal UInt32 Filetype;	// type of file
            internal UInt32 Ncmds;		// number of load commands 
            internal UInt32 Sizeofcmds;	// the size of all the load commands
            internal UInt32 Flags;      // Flags indicating attributes
        };



        MACHO_FileHeader FileHeader;                // File header


        public Macho()
        {

        }

        internal MACHO_FileHeader GetCoffFileHeader(int offset)
        {
            MACHO_FileHeader header;

            header.Magic = BitConverter.ToUInt32(dataStream, offset);                  
            header.Cputype = BitConverter.ToUInt32(dataStream, offset + 4);      
            header.Cpusubtype = BitConverter.ToUInt32(dataStream, offset + 8);         
            header.Filetype = BitConverter.ToUInt32(dataStream, offset + 12);          
            header.Ncmds = BitConverter.ToUInt32(dataStream, offset + 16);       
            header.Sizeofcmds = BitConverter.ToUInt32(dataStream, offset + 20);  
            header.Flags = BitConverter.ToUInt32(dataStream, offset + 24);
            
            return header;
        }

        void ParseFile()
        {
            if (fileParsed == true)
            {
                return;
            }
            int CurrentOffset = 0;

            // Find file header
            FileHeader = GetCoffFileHeader(CurrentOffset);
            CurrentOffset = 28;
            if((FileHeader.Magic == MAC_MAGIC_64) || (FileHeader.Magic == MAC_CIGAM_64))
            {
                CurrentOffset += 4;
            }

            int CmdSize = 0;
            UInt32 Cmd = 0;

            for (int i = 1; i <= FileHeader.Ncmds; i++)
            {
                if (CurrentOffset >= DataSize)
                {
                    //Error
                    return;
                }
/*
                uint8* currentp = (uint8*) (Buf() + currentoffset);
                Cmd = ((MAC_load_command*) currentp)->cmd;

                CmdSize = ((MAC_load_command*) currentp)->cmdsize;
*/
                // Interpret specific command type

                switch (Cmd)
                {
                    case MAC_LC_SEGMENT:
                    {
                        if (WordSize != 32)
                        {
                            // Error
                            return;
                        }
                        /*
                        MAC_segment_command_32* sh = (MAC_segment_command_32*) currentp;
                        SegmentOffset = sh->fileoff;              // File offset of segment
                        SegmentSize = sh->filesize;               // Size of segment
                        NumSections = sh->nsects;                 // Number of sections
                        SectionHeaderOffset = currentoffset + sizeof(TMAC_segment_command); // File offset of section headers                 
                        if (!ImageBase && strcmp(sh->segname, "__TEXT") == 0)
                        {
                            ImageBase = sh->vmaddr; // Find image base
                        }
                        */
                        break;
                    }

                    case MAC_LC_SEGMENT_64:
                    {
                        if (WordSize != 64)
                        {
                            // Error
                            return;
                        }
                        /*
                        MAC_segment_command_64* sh = (MAC_segment_command_64*) currentp;

                        SegmentOffset = (uint32) sh->fileoff;      // File offset of segment
                        SegmentSize = (uint32) sh->filesize;       // Size of segment
                        NumSections = sh->nsects;                 // Number of sections
                        SectionHeaderOffset = currentoffset + sizeof(TMAC_segment_command); // File offset of section headers
                        if (!ImageBase && strcmp(sh->segname, "__TEXT") == 0)
                        {
                            ImageBase = sh->vmaddr; // Find image base
                        }
                        */
                        break;
                    }

                    case MAC_LC_SYMTAB:
                    {
                        /*
                        MAC_symtab_command* sh = (MAC_symtab_command*) currentp;
                        SymTabOffset = sh->symoff;                // File offset of symbol table
                        SymTabNumber = sh->nsyms;                 // Number of entries in symbol table
                        StringTabOffset = sh->stroff;             // File offset of string table
                        StringTabSize = sh->strsize;              // Size of string table
                        */
                        break;
                    }

                    case MAC_LC_DYSYMTAB:
                    {
                        /*
                        MAC_dysymtab_command* sh = (MAC_dysymtab_command*) currentp;
                        ilocalsym = sh->ilocalsym;                 // index to local symbols
                        nlocalsym = sh->nlocalsym;                 // number of local symbols 
                        iextdefsym = sh->iextdefsym;                // index to externally defined symbols
                        nextdefsym = sh->nextdefsym;                // number of externally defined symbols 
                        iundefsym = sh->iundefsym;                 // index to undefined symbols
                        nundefsym = sh->nundefsym;                 // number of undefined symbols
                        IndirectSymTabOffset = sh->indirectsymoff;// file offset to the indirect symbol table
                        IndirectSymTabNumber = sh->nindirectsyms; // number of indirect symbol table entries
                        */
                        break;
                    }

                }
                CurrentOffset += CmdSize;
            }
        }

        internal List<string> GetPublicNames()
        {
            // Make list of public names in object file
            List<string> publicNames = new List<string>();

            // Interpret header:
            ParseFile();

            // loop through public symbol table
            /*
            TMAC_nlist * symp = (TMAC_nlist*)(Buf() + SymTabOffset + iextdefsym * sizeof(TMAC_nlist));

            for (int i = 0; i < nextdefsym; i++, symp++) 
            {
                if (symp->n_strx < StringTabSize && !(symp->n_type & MAC_N_STAB))
                {
                    // Public symbol found
                    se.Member = m;

                    // Store name
                    se.String = Strings->PushString(strtab + symp->n_strx);         

                    // Store name index
                    publicNames.Add(entry.s.Name);
                }
            }

            */
            return publicNames;
        }

        internal List<SignitureEntry> GetSignitures()
        {
            List<SignitureEntry> signitures = new List<SignitureEntry>();
            // Interpret header:
            ParseFile();

            int symbIndex = 0;

            
            
            return signitures;
        }
    }
}
