using Reko.ImageLoaders.Coff;
using RekoSig;
using System;
using System.Collections.Generic;
using System.IO;

namespace RekoMakeSigs
{   
    // 60 bytes
    internal struct SUNIXLibraryHeader
    {
        public string Name;                      // Member name
        public string Date;                      // Member date, seconds, decimal ASCII
        public string UserID;                     // Member User ID, decimal ASCII
        public string GroupID;                    // Member Group ID, decimal ASCII
        public string FileMode;                   // Member file mode, octal
        public string FileSize;                  // Member file size, decimal ASCII
    };


    internal class Library : DataBuffer
    {
 
        const int SUNIXLibraryHeaderSize = 60;
        int CurrentOffset;               // Offset to current member
        int CurrentNumber;               // Number of current member
        Signiture signiture;

        internal Library(string filename) : base (filename)
        {
            FileType = FileTypes.UNKNOWN;
            DataSize = 0;
            dataStream = null;
            WordSize = 0;
            Executable = false;
            signiture = new Signiture();
        }


        internal bool GenerateSigniture(string destFile)
        {
            if (ReadFile())
            {
                GetFileType();
                if ((FileType == FileTypes.FILETYPE_LIBRARY) | (FileType == FileTypes.FILETYPE_OMFLIBRARY))
                {
                    // Input file is a library
                    ProcessFile();
                    signiture.SaveSignatures(destFile);
                }
                else
                {
                    Console.WriteLine("Unable to process the given file type");
                }
            }
            else
            {
                // error message
                Console.WriteLine("Error reading the input file");
            }
            return false;
        }

        private bool ReadFile()
        {
            using (FileStream fs = File.Open(InputFilePath, FileMode.Open))
            {
                dataStream = new byte[fs.Length];
                int read = fs.Read(dataStream, 0, dataStream.Length);
                if (read == dataStream.Length)
                {
                    DataSize = dataStream.Length;
                    return true;
                }
            }
            return false;
        }

       
        internal void ProcessFile()
        {
            // Dispatch according to library type
            switch (FileType)
            {
                case FileTypes.FILETYPE_LIBRARY:
                { 
                    DumpUNIX();
                } break;                        // Print contents of UNIX style library

                case FileTypes.FILETYPE_OMFLIBRARY:
                {
                    //DumpOMF();
                }
                break;                        // Print contents of OMF style library

                default:
                    Console.WriteLine("Objconv program internal inconsistency");        // Should not occur
                    break;
            }
        }

        SUNIXLibraryHeader GetUNIXLibraryHeader(int offset)
        {
            SUNIXLibraryHeader header;

            header.Name = System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset, 16);
            header.Date = System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset + 16, 12);
            header.UserID =  System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset + 2, 6);
            header.GroupID = System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset + 34, 6);
            header.FileMode = System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset + 40, 8);
            header.FileSize = System.Text.Encoding.UTF8.GetString(dataStream, CurrentOffset + 48, 10);

            return header;
        }

        void DumpUNIX()
        {
            string MemberName = "";
            CurrentOffset = 8;
            CurrentNumber = 0;

            Console.WriteLine("\nDump of library %s", InputFilePath);
            Console.WriteLine("\n\nExported symbols by member:\n");

            // Loop through library
            while (CurrentOffset + SUNIXLibraryHeaderSize < DataSize)
            {
                DataBuffer dataSubSet = new DataBuffer();
                int tmpStart = CurrentOffset;

                // Get member name
                MemberName = ExtractMember(dataSubSet);
                if (MemberName == "") break;

                dataSubSet.Filename = MemberName;

                Console.WriteLine(MemberName);
                // Get symbol table for specific file type
                switch (dataSubSet.GetFileType())
                {
                    case FileTypes.FILETYPE_ELF:
                    {
                        Elf elf = new Elf(dataSubSet.GetDataSize());
                        elf.SetData(dataSubSet.GetData(), 0, dataSubSet.GetDataSize());
                        List<String> names = elf.GetPublicNames();
                        foreach (string name in names)
                        {
                            Console.WriteLine(name);
                        }

                        // List<SignitureEntry> signitures = elf.GetSignitures();
                        // Add to the signiture tree
                        //  AddToSignitureTree(signitures);
                    }
                    break;
                    

                    case FileTypes.FILETYPE_COFF:
                    {       
                        
                        Coff coff = new Coff();
                        coff.SetData(dataSubSet.GetData(), 0, dataSubSet.GetDataSize());
                        List<String> names = coff.GetPublicNames();
                        foreach(string name in names)
                        {
                            Console.WriteLine(name);
                        }

                        List<SignitureEntry> signitures = coff.GetSignitures();
                        // Add to the signiture tree
                        AddToSignitureTree(signitures); 
                    }
                    break;
                    
                    case FileTypes.FILETYPE_MACHO_LE:
                    {
                        Macho mac = new Macho();
                        mac.SetData(dataSubSet.GetData(), 0, dataSubSet.GetDataSize());
                        List<String> names = mac.GetPublicNames();
                        foreach (string name in names)
                        {
                            Console.WriteLine(name);
                        }

                        List<SignitureEntry> signitures = mac.GetSignitures();
                        // Add to the signiture tree
                        AddToSignitureTree(signitures);
                    }
                    break;
 

                    default:
                        Console.WriteLine("\n   Cannot extract symbol names from this file type");
                        break;
                }
            }
        }

        string ExtractMember(DataBuffer dataSubSet)
        {
            // Dispatch according to library type
            if (FileType == FileTypes.FILETYPE_OMFLIBRARY || FileType == FileTypes.FILETYPE_OMF)
            {
                return ""; // ExtractMemberOMF();
            }
            else
            {
                return ExtractMemberUNIX(dataSubSet);
            }
        }

        string ExtractMemberUNIX(DataBuffer dataSubSet)
        {
            // Extract member of UNIX style library
            // This function is called repeatedly to get each member of library/archive
            SUNIXLibraryHeader Header;     // Member header
            int MemberSize = 0;           // Size of member
            int HeaderExtra = 0;          // Extra added to size of header
            int LongNames;                // Offset to long names member
            int LongNamesSize = 16;       // Size of long names member
            string Name = "";              // Name of member
            int Skip = 1;                 // Skip record and search for next


            if (CurrentOffset == 0 || CurrentOffset + SUNIXLibraryHeaderSize >= DataSize)
            {
                // No more members
                return "";
            }

            // Search for member
            while (Skip > 0 && CurrentOffset > 0)
            {
                HeaderExtra = 0;
                // Extract next library member from input library
                Header = GetUNIXLibraryHeader(CurrentOffset);
                // Size of member
                MemberSize = Convert.ToInt32(Header.FileSize);
                if (MemberSize + CurrentOffset + SUNIXLibraryHeaderSize > DataSize)
                {
                    Console.WriteLine("Library/archive file is corrupt");
                    return "";
                }
                // Member name
                Name = Header.Name;
                if (String.Compare(Name, 0, "// ", 0, 3, true) == 0)
                {
                    // This is the long names member. Remember its position
                    LongNames = CurrentOffset + SUNIXLibraryHeaderSize;
                    LongNamesSize = MemberSize;
                    // The long names are terminated by '/' or 0, depending on system,
                    // but may contain non-terminating '/'. Find out which type we have:
                    // Pointer to LongNames record
  
                    // Find out whether we have terminating zeroes:
                    //dataStream

                    //if ((LongNamesSize > 1 && p[LongNamesSize - 1] == '/') || (p[LongNamesSize - 1] <= ' ' && p[LongNamesSize - 2] == '/'))
                    //{
                        // Names are terminated by '/'. Replace all '/' by 0 in the longnames record
                    //    for (uint j = 0; j < LongNamesSize; j++, p++)
                    //    {
                    //        if (*p == '/') *p = 0;
                    //    }
                    //}
                }
                else if (String.Compare(Name, 0, "/ ", 0, 2, true) == 0
                    || String.Compare(Name, 0, "__.SYMDEF",0,  9, true) == 0)
                {
                    // This is a symbol index member.
                    // The symbol index is not used because we are always building a new symbol index.
                }
                // ******* To FIX*********
 /*               else if (Name[0] == '/' && Name[1] >= '0' && Name[1] <= '9' && LongNames)
                {
                    // Name contains index into LongNames record
                    NameIndex =(uint) Convert.ToInt32(Name + 1);
                    if (NameIndex < LongNamesSize)
                    {
                        Name = Buf() + LongNames + NameIndex;
                    }
                    else
                    {
                        Name = "NoName!";
                    }
                    Skip = 0;
                }
                */
                else if (String.Compare(Name, 0, "#1/", 0, 3, true) == 0)
                {
                    // Name refers to long name after the header
                    // This variant is used by Mac and some versions of BSD
                    HeaderExtra = Convert.ToInt32(Name + 3);
                    Name += SUNIXLibraryHeaderSize;
                    if (MemberSize > HeaderExtra)
                    {
                        // The length of the name, HeaderExtra, is included in the 
                        // Header->FileSize field. Subtract to get the real file size
                        MemberSize -= HeaderExtra;
                    }
                    if (String.Compare(Name, 0, "__.SYMDEF", 0, 9, true) == 0)
                    {
                        // Symbol table "__.SYMDEF SORTED" as long name
                        Skip = 1;
                    }
                    else
                    {
                        Skip = 0;
                    }
                }
                else
                {
                    // Ordinary short name
                    // Name may be terminated by '/' or space. Replace termination char by 0
                    string tmp = Name.Replace('/', ' ');
                    char[] charsToTrim = { ',', '.', ' ' };
                    Name = tmp.TrimEnd(charsToTrim);
                    Skip = 0;
                }
                // Save member as raw data
                if (dataSubSet != null)
                {
                    dataSubSet.SetData(dataStream, CurrentOffset + SUNIXLibraryHeaderSize + HeaderExtra, MemberSize);
                }
                // Point to next member
                CurrentOffset = NextHeader(CurrentOffset);

                // Increment number
                if (Skip == 0)
                {
                    CurrentNumber++;
                }
            }  // End of while loop

            // Check name
            if (Name == "")
            {
                Name = "NoName!";
            }
            // Return member name
            return Name;
        }

        int NextHeader(int Offset)
        {
            // Loop through library headers.
            SUNIXLibraryHeader Header;   // Member header
            int MemberSize;          // Size of member
            int HeaderExtra = 0;    // Extra added to size of header
            int NextOffset;         // Offset of next header

            if (Offset + SUNIXLibraryHeaderSize >= DataSize)
            {
                // No more members
                return 0;
            }
            // Find header
            Header = GetUNIXLibraryHeader(Offset);

            // Size of member
            MemberSize = Convert.ToInt32(Header.FileSize);
            if (MemberSize < 0 || MemberSize + Offset + SUNIXLibraryHeaderSize > DataSize)
            {
                Console.WriteLine("Library/archive file is corrupt");  // Points outside file
                return 0;
            }
            if (String.Compare(Header.Name, 0, "#1/", 0, 3, true) == 0)
            {
                // Name refers to long name after the header
                // This variant is used by Mac and some versions of BSD.
                // HeaderExtra is included in MemberSize:
                // HeaderExtra = atoi(Header->Name+3);
            }

            // Get next offset
            NextOffset = Offset + SUNIXLibraryHeaderSize + MemberSize;

            // Round up to align by 2
            NextOffset = NextOffset + (NextOffset % 2);

            // Check if last
            if (NextOffset >= DataSize)
            {
                NextOffset = 0;
            }
            return NextOffset;
        }


        internal void AddToSignitureTree(List<SignitureEntry> signitures)
        {
            foreach (SignitureEntry entry in signitures)
            {
                signiture.AddMethodSigniture(Path.GetFileName(Filename), entry.name, entry.data, entry.length);
            }
        }
    }
}
