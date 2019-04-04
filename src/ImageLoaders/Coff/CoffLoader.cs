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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.Coff
{
    public class CoffLoader : ImageLoader
    {
        //********************* Storage classes for symbol table entries **********************
        const byte COFF_CLASS_NULL = 0;
        const byte COFF_CLASS_AUTOMATIC = 1; // automatic variable
        const byte COFF_CLASS_EXTERNAL = 2; // external symbol 
        const byte COFF_CLASS_STATIC = 3; // static
        const byte COFF_CLASS_REGISTER = 4; // register variable
        const byte COFF_CLASS_EXTERNAL_DEF = 5; // external definition 
        const byte COFF_CLASS_LABEL = 6; // label
        const byte COFF_CLASS_UNDEFINED_LABEL = 7; // undefined label
        const byte COFF_CLASS_MEMBER_OF_STRUCTURE = 8; // member of structure
        const byte COFF_CLASS_ARGUMENT = 9; // function argument
        const byte COFF_CLASS_STRUCTURE_TAG = 10; // structure tag 
        const byte COFF_CLASS_MEMBER_OF_UNION = 11; // member of union 
        const byte COFF_CLASS_UNION_TAG = 12; // union tag 
        const byte COFF_CLASS_TYPE_DEFINITION = 13; // type definition
        const byte COFF_CLASS_UNDEFINED_STATIC = 14; // undefined static 
        const byte COFF_CLASS_ENUM_TAG = 15; // enumeration tag 
        const byte COFF_CLASS_MEMBER_OF_ENUM = 16; // member of enumeration
        const byte COFF_CLASS_REGISTER_PARAM = 17; // register parameter
        const byte COFF_CLASS_BIT_FIELD = 18; // bit field  
        const byte COFF_CLASS_AUTO_ARGUMENT = 19; // auto argument 
        const byte COFF_CLASS_LASTENTRY = 20; // dummy entry (end of block)
        const byte COFF_CLASS_BLOCK = 100; // ".bb" or ".eb" 
        const byte COFF_CLASS_FUNCTION = 101; // ".bf" or ".ef" 
        const byte COFF_CLASS_END_OF_STRUCT = 102; // end of structure 
        const byte COFF_CLASS_FILE = 103; // file name  
        const byte COFF_CLASS_LINE = 104; // line # reformatted as symbol table entry 
        const byte COFF_CLASS_SECTION = 104; // line # reformatted as symbol table entry
        const byte COFF_CLASS_ALIAS = 105; // duplicate tag 
        const byte COFF_CLASS_WEAK_EXTERNAL = 105; // duplicate tag  
        const byte COFF_CLASS_HIDDEN = 106; // ext symbol in dmert public lib 
        const byte COFF_CLASS_END_OF_FUNCTION = 0xff; // physical end of function 

        //********************* Type for symbol table entries **********************
        const byte COFF_TYPE_FUNCTION = 0x20; // Symbol is function
        const byte COFF_TYPE_NOT_FUNCTION = 0x00; // Symbol is not a function


        //********************* Section number values for symbol table entries **********************
        const short COFF_SECTION_UNDEF = 0;         // external symbol
        const short COFF_SECTION_ABSOLUTE = -1;     // value of symbol is absolute
        const short COFF_SECTION_DEBUG = -2;        // debugging symbol - value is meaningless
        const short COFF_SECTION_N_TV = -3;         // indicates symbol needs preload transfer vector
        const short COFF_SECTION_P_TV = -4;         // indicates symbol needs postload transfer vector
        const short COFF_SECTION_REMOVE_ME = -99;   // Specific for objconv program: Debug or exception section being removed



        int NumberOfSymbols;                        // Number of symbols in the Coff package
        FileHeader fileHeader;                      // File header
        List<SectionHeader> SectionHeaders;         // Copy of section headers
        List<SymbolTableEntry> SymbolTable;         // Pointer to symbol table (for object files)
        Dictionary<int, string> StringTable;        // Pointer to string table (for object files)
        OptionHeader32 optionHeader32;              // Optional header (for executable files)
        OptionHeader64 optionHeader64;              // Optional header (for executable files)
        Int64 ImageBase;                            // Image base (for executable files)
        //SCOFF_IMAGE_DATA_DIRECTORY pImageDirs;    // Pointer to image directories (for executable files)
        uint NumImageDirs;                          // Number of image directories (for executable files)
        uint EntryPoint;                            // Entry point (for executable files)
        List<SignitureEntry> signitures;

        public CoffLoader(IServiceProvider services, string filename, byte[] rawBytes)
            : base(services, filename, rawBytes)
        {
            SectionHeaders = new List<SectionHeader>();
            signitures = new List<SignitureEntry>();
            SymbolTable = new List<SymbolTableEntry>();
            StringTable = new Dictionary<int, string>();

            ParseFile();
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            throw new NotImplementedException();
        }

        
        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public List<SignitureEntry> GetSignitures()
        {
            return signitures;
        }

        public List<string> GetPublicNames()
        {
            // Make list of public names in object file
            List<string> publicNames = new List<string>();

            // Loop through symbol table
            foreach (SymbolTableEntry entry in SymbolTable)
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


        void GetSymbolTables(LeImageReader rdr)
        {
            SymbolTable.Clear();

            for (int SymbolsIndex = 0; SymbolsIndex < NumberOfSymbols; SymbolsIndex++)
            {
                SymbolTableEntry entry = new SymbolTableEntry();

                entry.s = SymbolTableEntry_S.Load(rdr, SymbolsIndex, StringTable);

                int jsym = 0;
                while ((jsym < entry.s.NumAuxSymbols) && (SymbolsIndex < NumberOfSymbols))
                {
                    long b = rdr.Offset;
                    // Detect auxiliary entry type
                    if ((entry.s.StorageClass == COFF_CLASS_EXTERNAL) && (entry.s.Type == COFF_TYPE_FUNCTION) && (entry.s.SectionNumber > 0))
                    {
                        // This is a function definition aux record
                        entry.func = SymbolTableEntry_func.Load(rdr);
                    }
                    else if ((entry.s.Name == ".bf") || (entry.s.Name == ".ef"))
                    {
                        // This is a .bf or .ef aux record
                        entry.bfef = SymbolTableEntry_bfef.Load(rdr);
                    }
                    else if (entry.s.StorageClass == COFF_CLASS_EXTERNAL &&
                       entry.s.SectionNumber == COFF_SECTION_UNDEF &&
                       entry.s.Value == 0)
                    {
                        // This is a Weak external aux record
                        entry.weak = SymbolTableEntry_weak.Load(rdr);
                    }
                    else if (entry.s.StorageClass == COFF_CLASS_FILE)
                    {
                        // This is filename aux record. Contents has already been printed
                        entry.filename = SymbolTableEntry_filename.Load(rdr);
                    }
                    else if (entry.s.StorageClass == COFF_CLASS_STATIC)
                    {
                        // This is section definition aux record
                        entry.section = SymbolTableEntry_section.Load(rdr);
                    }
                    else if (entry.s.StorageClass == COFF_CLASS_ALIAS)
                    {
                        // This is section definition aux record
                    }
                    rdr.Offset = b + 18;
                    SymbolsIndex++;
                    jsym++;
                }
                SymbolTable.Add(entry);
            }
        }


        private void GetRelocations(LeImageReader rdr)
        {
            foreach (SectionHeader header in SectionHeaders)
            {
                long tmp = rdr.Offset;
                rdr.Offset = header.PRelocations;

                for (int indexer = 0; indexer < header.NRelocations; indexer ++)
                {
                    Relocation reloc = Relocation.Load(rdr);
                    header.relocationList.Add(reloc);
                }
                rdr.Offset = tmp;
                header.relocationList.Sort();
            }
        }


        private void ParseFile()
        {
            LeImageReader rdr = new LeImageReader(RawImage);

            // Get offset to file header
            int FileHeaderOffset = 0;
            
            if ((rdr.ReadUInt16() & 0xFFF9) == 0x5A49)
            {
                // File has DOS stub
                rdr.Offset = 0;
                uint Signature = rdr.ReadUInt32(0x3C);
                rdr.Offset = 0;
                if (Signature + 8 < rdr.Bytes.Length && rdr.ReadUInt16((int) Signature) == 0x4550)
                {
                    // Executable PE file
                    FileHeaderOffset = (int) (Signature + 4);
                }
                else
                {
                    Console.WriteLine("Objconv program internal inconsistency");
                    return;
                }
            }

            // Set the offset pointer to the start of the header section
            rdr.Offset = FileHeaderOffset;
            // Read the header
            fileHeader = FileHeader.Load(rdr);
            
            // check header integrity
            if ((fileHeader.PSymbolTable + (fileHeader.NumberOfSymbols * SymbolTableEntry.Size)) > rdr.Bytes.Length)
            {
                Console.WriteLine("Pointer out of range in object file");
                return;
            }

            // Find optional header if executable file
            if (fileHeader.SizeOfOptionalHeader > 0 && FileHeaderOffset > 0)
            {
                // This will need updating so that the Optional header infomation is extracted for use,
                // there are 32 and 64 bit versions which will need to be taken into account

                if (fileHeader.SizeOfOptionalHeader == OptionHeader32.Size)
                {
                    optionHeader32 = OptionHeader32.Load(rdr);

                    // Find image data directories
                    NumImageDirs = optionHeader32.NumberOfRvaAndSizes;
                    EntryPoint = optionHeader32.AddressOfEntryPoint;
                    ImageBase = optionHeader32.ImageBase; 
                }
                else if(fileHeader.SizeOfOptionalHeader == OptionHeader64.Size)
                {
                    optionHeader64 = OptionHeader64.Load(rdr);

                    // Find image data directories
                    NumImageDirs = optionHeader64.NumberOfRvaAndSizes;
                    EntryPoint = optionHeader64.AddressOfEntryPoint;
                    ImageBase = optionHeader64.ImageBase; 
                }
                else
                {
                    // Size of option header does not match expeted data size, so jump across this to recover.
                    rdr.Offset += fileHeader.SizeOfOptionalHeader;
                }
            }

            // Find section headers
            for (int i = 0; i < fileHeader.NumberOfSections; i++)
            {
                SectionHeaders.Add(SectionHeader.Load(rdr, fileHeader.Machine));
                
                // Check for _ILDATA section
                if (SectionHeaders[i].Name == "_ILDATA")
                {
                    // This is an intermediate file for Intel compiler
                    Console.WriteLine("This is an intermediate file for whole-program-optimization in Intel compiler");
                    return;
                }
            }

            // Find symbol table
            NumberOfSymbols = fileHeader.NumberOfSymbols;

            // Find string table
            int stringTableOffset = (int) (fileHeader.PSymbolTable + NumberOfSymbols * SymbolTableEntry.Size);
            
            long tmp = rdr.Offset;
            rdr.Offset = 0;
            int StringTableSize = rdr.ReadInt32(stringTableOffset);
            rdr.Offset = tmp;

            if (StringTableSize > 0)
            {
                int startPoint = stringTableOffset + 4;
                int CharCount = 0;
                int x = 4;
                while (x < StringTableSize)
                {
                    CharCount = 0;
                    while (rdr.Bytes[startPoint + CharCount] != 0)
                    {
                        CharCount++;
                        x++;
                    }
                    string strTmp = Encoding.UTF8.GetString(rdr.Bytes, startPoint, CharCount);
                    if (strTmp != "")
                    {
                        StringTable.Add(startPoint - stringTableOffset, strTmp);
                    }

                    startPoint += CharCount;
                    startPoint++;
                    x++;
                }
            }

            long tmpDataIndexPointer = rdr.Offset;
            rdr.Offset = fileHeader.PSymbolTable;
            // Load the symbol tables
            GetSymbolTables(rdr);

            // Return the Offset Index
            rdr.Offset = tmpDataIndexPointer;

            GetRelocations(rdr);

            GenerateSignitures(rdr);
        }



        void GenerateSignitures(LeImageReader rdr)
        {
            signitures.Clear();
            
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
                            SectionHeader sh = SectionHeaders[SymbolTable[nextIndex].s.SectionNumber - 1];
                            uint CodeOffset = sh.PRawData;     // Point at which the data starts
                            
                            // Step back to find the end of the method (adjust for the align bytes)
                            while (end > start)
                            {
                                if (rdr.Bytes[CodeOffset + end] == 0xC3)
                                {
                                    //  have found the end of the method, so lets create a signture object
                                    SignitureEntry entry = new SignitureEntry();
                                    entry.Name = SymbolTable[symbIndex].s.Name;
                                    entry.Length = (int) (end - start) + 1;
                                    entry.Data = new byte[entry.Length];
                                    entry.MissBytes = sh.FindLocationPoints(start, end);
                                    Buffer.BlockCopy(rdr.Bytes, (int) (CodeOffset + start), entry.Data, 0, entry.Length);
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
        }
    }
}
