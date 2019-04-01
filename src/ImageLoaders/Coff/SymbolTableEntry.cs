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
    internal class SymbolTableEntry
    {
        internal SymbolTableEntry_S s;
        internal SymbolTableEntry_func func;
        internal SymbolTableEntry_bfef bfef;
        internal SymbolTableEntry_weak weak;
        internal SymbolTableEntry_filename filename;
        internal SymbolTableEntry_section section;

        public const int Size = 18;
    }


    // Normal symbol table entry
    internal class SymbolTableEntry_S
    {
        internal string Name;
        internal uint Value;
        internal Int16 SectionNumber;
        internal UInt16 Type;
        internal byte StorageClass;
        internal byte NumAuxSymbols;

        internal int symbolIndex;

        public static SymbolTableEntry_S Load(LeImageReader rdr, int index, Dictionary<int, string> StringTable)
        {
            var symTable = new SymbolTableEntry_S();

            symTable.symbolIndex = index;

            int stringOffset = rdr.ReadInt32();

            if (stringOffset == 0)
            {
                // Need to get the entry point into the string table
                stringOffset = rdr.ReadInt32();
                symTable.Name = StringTable[stringOffset];
            }
            else
            {
                rdr.Offset -= 4;
                symTable.Name = Encoding.UTF8.GetString(rdr.Bytes, (int) rdr.Offset, 8);
                rdr.Offset += 8;
                string tmp = symTable.Name.Replace('\0', ' ');
                char[] charsToTrim = { ',', '.', ' ' };
                symTable.Name = tmp.TrimEnd(charsToTrim);
            }


            symTable.Value = rdr.ReadUInt32();
            symTable.SectionNumber = rdr.ReadInt16();
            symTable.Type = rdr.ReadUInt16();
            symTable.StorageClass = rdr.ReadByte();
            symTable.NumAuxSymbols = rdr.ReadByte();

 

            return symTable;
        }
    }

    // Auxiliary symbol table entry types:
    //************************************

    // Function definition
    internal class SymbolTableEntry_func
    {
        internal uint TagIndex; // Index to .bf entry
        internal uint TotalSize; // Size of function code
        internal uint PointerToLineNumber; // Pointer to line number entry
        internal uint PointerToNextFunction; // Symbol table index of next function
        internal UInt16 x_tvndx;      // Unused

        public static SymbolTableEntry_func Load(LeImageReader rdr)
        {
            var symTable = new SymbolTableEntry_func();

            symTable.TagIndex = rdr.ReadUInt32();                   // Index to .bf entry
            symTable.TotalSize = rdr.ReadUInt32();                  // Size of function code
            symTable.PointerToLineNumber = rdr.ReadUInt32();        // Pointer to line number entry
            symTable.PointerToNextFunction = rdr.ReadUInt32();      // Symbol table index of next function
            symTable.x_tvndx = rdr.ReadUInt16();                    // Unused

            return symTable;
        }
    }

    // .bf abd .ef
    internal class SymbolTableEntry_bfef
    {
        internal uint Unused1;
        internal UInt16 SourceLineNumber; // Line number in source file
        internal UInt16 Unused2;
        internal uint Unused3; // Pointer to line number entry
        internal uint PointerToNextFunction; // Symbol table index of next function

        public static SymbolTableEntry_bfef Load(LeImageReader rdr)
        {
            var symTable = new SymbolTableEntry_bfef();

            symTable.Unused1 = rdr.ReadUInt32();
            symTable.SourceLineNumber = rdr.ReadUInt16();        // Line number in source file
            symTable.Unused2 = rdr.ReadUInt16();
            symTable.Unused3 = rdr.ReadUInt32();                 // Pointer to line number entry
            symTable.PointerToNextFunction = rdr.ReadUInt32();

            return symTable;
        }
    }

    // Weak external
    internal class SymbolTableEntry_weak
    {
        internal uint TagIndex; // Symbol table index of alternative symbol2
        internal uint Characteristics; //
        internal uint Unused1;
        internal uint Unused2;
        internal UInt16 Unused3;      // Unused

        public static SymbolTableEntry_weak Load(LeImageReader rdr)
        {
            var symTable = new SymbolTableEntry_weak();

            symTable.TagIndex = rdr.ReadUInt32();                   // Symbol table index of alternative symbol2
            symTable.Characteristics = rdr.ReadUInt32();
            symTable.Unused1 = rdr.ReadUInt32();
            symTable.Unused2 = rdr.ReadUInt32();
            symTable.Unused3 = rdr.ReadUInt16();                    // Unused

            return symTable;
        }
    }

    // File name
    internal class SymbolTableEntry_filename
    {
        internal string FileName;    // File name

        public static SymbolTableEntry_filename Load(LeImageReader rdr)
        {
            var symTable = new SymbolTableEntry_filename();

            symTable.FileName = Encoding.UTF8.GetString(rdr.Bytes, (int)rdr.Offset, 18);
            rdr.Offset += 18;
            string tmp = symTable.FileName.Replace('\0', ' ');
            char[] charsToTrim = { ',', '.', ' ' };
            symTable.FileName = tmp.TrimEnd(charsToTrim);

            return symTable;
        }
    }


    // Section definition
    internal class SymbolTableEntry_section
    {
        internal uint Length;
        internal UInt16 NumberOfRelocations;    // Line number in source file
        internal UInt16 NumberOfLineNumbers;
        internal uint CheckSum;                 // Pointer to line number entry
        internal UInt16 Number;                 // Symbol table index of next function
        internal byte Selection;                // Unused


        public static SymbolTableEntry_section Load(LeImageReader rdr)
        {
            var symTable = new SymbolTableEntry_section();

            symTable.Length = rdr.ReadUInt32();
            symTable.NumberOfRelocations = rdr.ReadUInt16();       // Line number in source file
            symTable.NumberOfLineNumbers = rdr.ReadUInt16();
            symTable.CheckSum = rdr.ReadUInt32();                  // Pointer to line number entry
            symTable.Number = rdr.ReadUInt16();                    // Symbol table index of next function
            symTable.Selection = rdr.ReadByte();                   // Unused
           
            return symTable;
        }
    }   
}