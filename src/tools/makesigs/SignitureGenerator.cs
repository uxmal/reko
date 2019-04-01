#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core;
using Reko.ImageLoaders.Coff;
using RekoSig;
using System;
using System.Collections.Generic;
using System.IO;

namespace makesigs
{

    class SignitureGenerator
    {
        string InputFilePath;
        int CurrentOffset;               // Offset to current member
        int CurrentNumber;               // Number of current member
        FileBuffer.FileTypes FileType;
        SignatureCreator signiture;
        FileBuffer fileBuffer;

        internal SignitureGenerator(string filename) 
        {
            InputFilePath = filename;
            FileType = FileBuffer.FileTypes.UNKNOWN;
            fileBuffer = null;
            signiture = new SignatureCreator();
        }


        internal bool GenerateSigniture(string destFile)
        {
            if (ReadFile())
            {
                FileType = fileBuffer.GetFileType();
                if ((FileType == FileBuffer.FileTypes.FILETYPE_LIBRARY) | (FileType == FileBuffer.FileTypes.FILETYPE_OMFLIBRARY))
                {
                    // Input file is a library
                    ProcessFile();
                    signiture.SaveSignatures(destFile);
                    return true;
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


        internal void AddToSignitureTree(List<SignitureEntry> signitures)
        {
            foreach (SignitureEntry entry in signitures)
            {
                signiture.AddMethodSigniture(Path.GetFileName(InputFilePath), entry.Name, entry.Data, entry.Length, entry.MissBytes);
            }
        }

        private bool ReadFile()
        {
            using (FileStream fs = File.Open(InputFilePath, FileMode.Open))
            {
                
                byte[] dataStream = new byte[fs.Length];
                int read = fs.Read(dataStream, 0, dataStream.Length);
                if (read == dataStream.Length)
                {
                    fileBuffer = new FileBuffer();
                    fileBuffer.SetData(dataStream, 0, (int)fs.Length);
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
                case FileBuffer.FileTypes.FILETYPE_LIBRARY:
                {
                    // Extracts contents of UNIX style library
                    ExtractUNIX();
                }
                break;                        

                case FileBuffer.FileTypes.FILETYPE_OMFLIBRARY:
                {
                    // Extracts contents of OMF style library
                    ExtractOMF();
                }
                break;                        

                default:
                Console.WriteLine("Objconv program internal inconsistency");        // Should not occur
                break;
            }
        }


        string ExtractMember(FileBuffer dataSubSet)
        {
            // Dispatch according to library type
            if((FileType == FileBuffer.FileTypes.FILETYPE_OMFLIBRARY) || (FileType == FileBuffer.FileTypes.FILETYPE_OMF))
            {
                return ExtractMemberOMF(dataSubSet);
            }
            else
            {
                return ExtractMemberUNIX(dataSubSet);
            }
        }


        #region Unix Format

        void ExtractUNIX()
        {
            string MemberName = "";
            CurrentOffset = 8;
            CurrentNumber = 0;

            Console.WriteLine("\nDump of library %s", InputFilePath);
            Console.WriteLine("\n\nExported symbols by member:\n");

            // Loop through library
            while (CurrentOffset + UNIXLibraryHeader.Size < fileBuffer.GetDataSize())
            {
                FileBuffer dataSubSet = new FileBuffer();
                int tmpStart = CurrentOffset;

                // Get member name
                MemberName = ExtractMember(dataSubSet);
                if (MemberName == "")
                {
                    break;
                }
                
                Console.WriteLine(MemberName);
                // Get symbol table for specific file type
                switch (dataSubSet.GetFileType())
                {
                case FileBuffer.FileTypes.FILETYPE_ELF:
                    {
                        // Use ELF Image loader to get the data
                        ///TODO
                        ///

                        // List<SignitureEntry> signitures = elf.GetSignitures();
                        // Add to the signiture tree
                        //  AddToSignitureTree(signitures);
                    }
                    break;


                case FileBuffer.FileTypes.FILETYPE_COFF:
                    {
                        int size = dataSubSet.GetDataSize();
                        byte[] rawBytes = new byte[size];

                        Buffer.BlockCopy(dataSubSet.GetData(), 0, rawBytes, 0, size);
                        CoffLoader cl = new CoffLoader(null, "", rawBytes);

                        List<String> names = cl.GetPublicNames();
                        foreach (string name in names)
                        {
                            Console.WriteLine(name);
                        }

                        List<SignitureEntry> signitures = cl.GetSignitures();
                        // Add to the signiture tree
                        AddToSignitureTree(signitures);
                    }
                    break;

                case FileBuffer.FileTypes.FILETYPE_MACHO_LE:
                    {
                        // Use MACHO Image loader to get the data
                        ///TODO
                        ///

                        //List<SignitureEntry> signitures = mac.GetSignitures();
                        // Add to the signiture tree
                        //AddToSignitureTree(signitures);
                    }
                    break;


                default:
                    Console.WriteLine("\n   Cannot extract symbol names from this file type");
                    break;
                }
            }
        }

        
        string ExtractMemberUNIX(FileBuffer dataSubSet)
        {
            // Extract member of UNIX style library
            // This function is called repeatedly to get each member of library/archive
            UNIXLibraryHeader Header;     // Member header
            int MemberSize = 0;           // Size of member
            int HeaderExtra = 0;          // Extra added to size of header
            int LongNames;                // Offset to long names member
            int LongNamesSize = 16;       // Size of long names member
            string Name = "";             // Name of member
            int Skip = 1;                 // Skip record and search for next


            if (CurrentOffset == 0 || CurrentOffset + UNIXLibraryHeader.Size >= fileBuffer.GetDataSize())
            {
                // No more members
                return "";
            }

            // Search for member
            while (Skip > 0 && CurrentOffset > 0)
            {
                HeaderExtra = 0;
                // Extract next library member from input library
                Header = UNIXLibraryHeader.Load(fileBuffer.GetData(), CurrentOffset);
                // Size of member
                MemberSize = Convert.ToInt32(Header.FileSize);
                if (MemberSize + CurrentOffset + UNIXLibraryHeader.Size > fileBuffer.GetDataSize())
                {
                    Console.WriteLine("Library/archive file is corrupt");
                    return "";
                }
                // Member name
                Name = Header.Name;
                if (String.Compare(Name, 0, "// ", 0, 3, true) == 0)
                {
                    // This is the long names member. Remember its position
                    LongNames = CurrentOffset + UNIXLibraryHeader.Size;
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
                else if((String.Compare(Name, 0, "/ ", 0, 2, true) == 0) || (String.Compare(Name, 0, "__.SYMDEF", 0, 9, true) == 0))
                {
                    // This is a symbol index member.
                    // The symbol index is not used because we are always building a new symbol index.
                }
                /*else if((String.Compare(Name, 0, "/", 0, 1, true) == 0)
                    && (Name[1] >= '0'))
                    && (Name[1] <= '9')
                    && (LongNames > 0))
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
                }*/
                else if (String.Compare(Name, 0, "#1/", 0, 3, true) == 0)
                {
                    // Name refers to long name after the header
                    // This variant is used by Mac and some versions of BSD
                    HeaderExtra = Convert.ToInt32(Name + 3);
                    Name += UNIXLibraryHeader.Size;
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
                    dataSubSet.SetData(fileBuffer.GetData(), CurrentOffset + UNIXLibraryHeader.Size + HeaderExtra, MemberSize);
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
            UNIXLibraryHeader Header;   // Member header
            int MemberSize;          // Size of member
            int NextOffset;         // Offset of next header

            if (Offset + UNIXLibraryHeader.Size >= fileBuffer.GetDataSize())
            {
                // No more members
                return 0;
            }
            // Find header
            Header = UNIXLibraryHeader.Load(fileBuffer.GetData(), Offset);

            // Size of member
            MemberSize = Convert.ToInt32(Header.FileSize);
            if (MemberSize < 0 || MemberSize + Offset + UNIXLibraryHeader.Size > fileBuffer.GetDataSize())
            {
                Console.WriteLine("Library/archive file is corrupt");  // Points outside file
                return 0;
            }
            if (String.Compare(Header.Name, 0, "#1/", 0, 3, true) == 0)
            {
                // Name refers to long name after the header
                // This variant is used by Mac and some versions of BSD.
                // HeaderExtra is included in MemberSize:
                // uint HeaderExtra = atoi(Header.Name+3);
            }

            // Get next offset
            NextOffset = Offset + UNIXLibraryHeader.Size + MemberSize;

            // Round up to align by 2
            NextOffset = NextOffset + (NextOffset % 2);

            // Check if last
            if (NextOffset >= fileBuffer.GetDataSize())
            {
                NextOffset = 0;
            }
            return NextOffset;
        }

        #endregion


        #region OMF Format

        void ExtractOMF()
        {
            ///TODO re-write this so that it uses the OMFstructures in the loads library
            /*
            byte Flags;                                 // Dictionary flags
            uint i;                                     // Loop counter
            uint Align;                                 // Member alignment
            uint RecordEnd;                             // End of OMF record
            SOMFRecordPointer rec;                      // Current OMF record
            string MemberName;                          // Name of library member
            string SymbolName;                          // Name of public symbol in member
            uint MemberStart;                           // Start of member
            uint MemberEnd;                             // End of member
            uint MemberNum = 0;                         // Member number
            uint FirstPublic;                           // Index to first public name of current member
            CMemoryBuffer Strings;                      // Local string buffer
            CSList<SStringEntry> MemberIndex;           // Local member index buffer
            COMF Member;                                // Local buffer for member

            DictionaryOffset = GetDataSize();             // Loop end. This value is changed when library header is read
            rec.Start(Buf(), 0, DictionaryOffset);        // Initialize record pointer
            PageSize = 0;


            // Loop through the records of all OMF modules
            do
            {
                // Check record type
                switch (rec.Type2)
                {
                    case OMF_LIBHEAD:  // Library header. This should be the first record
                    {
                        if (PageSize || rec.FileOffset > 0)
                        {
                            Console.WriteLine("Library has more than one header");
                            break;            // More than one header
                        }

                        // Read library header  
                        DictionaryOffset = rec.GetDword();
                        DictionarySize = rec.GetWord();
                        Flags = rec.GetByte();

                        // Page size / alignment for members
                        PageSize = rec.End + 1;
                        Align = 1 << FloorLog2(PageSize);       // Make power of 2
                        if (PageSize != Align)
                        {
                            Console.WriteLine("Library page size (%i) is not a power of 2", PageSize);
                            return;
                        }

                        // Reset record loop end when DictionaryOffset is known
                        rec.FileEnd = DictionaryOffset;
    
                        // Print values from LIBHEAD
                        printf("\nOMF Library. Page size %i. %s.", PageSize, Lookup(OMFLibraryFlags, Flags));
                    }
                    break;

                    case OMF_THEADR: // Module header. Member starts here
                    {
                        MemberName = rec.GetString();           // Get name
                        MemberStart = rec.FileOffset;           // Get start address

                        printf("\nMember %s Offset 0x%X", MemberName, MemberStart);// Print member name
                    }
                    break;

                    case OMF_MODEND: // Member ends here.
                    {
                        RecordEnd = rec.FileOffset + rec.End + 1;// End of record
                        MemberEnd = RecordEnd;                   // = member end address

                        // Store member in temporary buffer
                        Member.SetSize(0);
                        Member.Push(Buf() + MemberStart, MemberEnd - MemberStart);

                        // Get public names from member
                        FirstPublic = MemberIndex.GetNumEntries();
                        Member.PublicNames(&Strings, &MemberIndex, ++MemberNum);

                        // Print public names
                        for (i = FirstPublic; i < MemberIndex.GetNumEntries(); i++)
                        {
                            SymbolName = Strings.Buf() + MemberIndex[i].String;
                            printf("\n  %s", SymbolName);
                        }

                        // Align next member by PageSize;
                        MemberEnd = (MemberEnd + PageSize - 1) & -(int32) PageSize;
                        rec.End = MemberEnd - rec.FileOffset - 1;
                    }
                    break;

                    case OMF_LIBEND: // Last member should end here
                    {
                        RecordEnd = rec.FileOffset + rec.End + 1;// End of record
                        if (RecordEnd != DictionaryOffset)
                        {
                            Console.WriteLine("Library end record does not match dictionary offset in OMF library");
                            return;
                        }
                    }
                    break;
                }
            }  // Go to next record
            while (rec.GetNext());                        // End of loop through records

            // Check hash table integrity
            CheckOMFHash(Strings, MemberIndex);

            // Check if there is an extended library dictionary
            uint ExtendedDictionaryOffset = DictionaryOffset + DictionarySize * 512;


            if (ExtendedDictionaryOffset > GetDataSize())
            {
                Console.WriteLine("Library/archive file is corrupt");
                return;
            }

            if (ExtendedDictionaryOffset < GetDataSize())
            {
                // Library contains extended dictionary
                uint ExtendedDictionarySize = GetDataSize() - ExtendedDictionaryOffset;
                byte DictionaryType = Get<byte>(ExtendedDictionaryOffset); // Read first byte of extended dictionary

                if (DictionaryType == OMF_LIBEXT)
                {
                    // Extended dictionary in the official format
                    printf("\nExtended dictionary IBM/MS format. size %i", ExtendedDictionarySize);
                }
                else if (ExtendedDictionarySize >= 10 && (DictionaryType == 0xAD || Get<ushort>(ExtendedDictionaryOffset + 2) == MemberNum))
                {
                    // Extended dictionary in the proprietary Borland format, documented only in US Patent 5408665, 1995
                    printf("\nExtended dictionary Borland format. size %i", ExtendedDictionarySize);
                }
                else
                {
                    // Unknown format
                    printf("\nExtended dictionary size %i, unknown type 0x%02X", ExtendedDictionarySize, DictionaryType);
                }
            }
            */
        }


        string ExtractMemberOMF(FileBuffer dataSubSet)
        {
            ///TODO re-write this so that it uses the OMFstructures in the loads library
            /*
            uint RecordEnd;                   // End of OMF record
            SOMFRecordPointer rec;            // Current OMF record
            string MemberName;                // Name of library member
            uint MemberStart = 0;             // Start of member
            uint MemberEnd = 0;                // End of member

            if (CurrentOffset >= DictionaryOffset) 
            {
                return "";// No more members
            }

            rec.Start(Buf(), CurrentOffset, DictionaryOffset);// Initialize record pointer
            // Loop through the records of all OMF modules
            do 
            {
                // Check record type
                switch (rec.Type2) 
                {
                    case OMF_THEADR: // Module header. Member starts here
                    {
                        MemberName  = rec.GetString();          // Get name
                        MemberStart = rec.FileOffset;           // Get start address
                    }
                    break;

                    case OMF_MODEND: // Member ends here.
                    {
                        RecordEnd = rec.FileOffset + rec.End +1;// End of record
                        MemberEnd = RecordEnd;                  // = member end address

                        // Save member as raw data
                        if (Destination) 
                        {
                            Destination->SetSize(0);             // Make sure destination buffer is empt
                            Destination->FileType = Destination->WordSize = 0;
                            Destination->Push(Buf() + MemberStart, MemberEnd - MemberStart);
                        }

                        // Align next member by PageSize;
                        rec.GetNext(PageSize);
                        CurrentOffset = rec.FileOffset;

                        // Check name
                        if (MemberName[0] == 0) 
                        {
                            MemberName = "NoName!";
                        }

                        // Return member name
                        return MemberName;
                    }       

                    case OMF_LIBEND: // Last member should end here
                    {
                        RecordEnd = rec.FileOffset + rec.End + 1;// End of record

                        if (RecordEnd != DictionaryOffset)
                        {
                            Console.WriteLine("Library end record does not match dictionary offset in OMF library");
                        }
                        // No more members:
                        return "";
                    }
                }     
            }  // Go to next record     
            while (rec.GetNext());                        // End of loop through records


            Console.WriteLine("Library end record not found");
           */
            // Return member name
            return "";
        }

        #endregion

    }
}
