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


namespace Reko.ImageLoaders.Coff
{
    public class FileHeader
    {
        // Values of Machine:
        internal const ushort PE_MACHINE_I386 = 0x14c;
        internal const ushort PE_MACHINE_X8664 = 0x8664;

        // Bits for Flags:
        const short PE_F_RELFLG = 0x0001;  // relocation info stripped from file
        const short PE_F_EXEC = 0x0002;    // file is executable (no unresolved external references)
        const short PE_F_LNNO = 0x0004;    // line numbers stripped from file
        const short PE_F_LSYMS = 0x0008;   // local symbols stripped from file


        internal ushort Machine;              // Machine ID (magic number)
        internal short NumberOfSections;      // number of sections
        internal uint TimeDateStamp;          // time & date stamp 
        internal int PSymbolTable;            // file pointer to symbol table
        internal int NumberOfSymbols;         // number of symbol table entries 
        internal ushort SizeOfOptionalHeader; // size of optional header
        internal ushort Flags;                // Flags indicating attributes

        public const int Size = 20;

        public static FileHeader Load(LeImageReader rdr)
        {
            var hdr = new FileHeader();

            hdr.Machine = rdr.ReadLeUInt16();
            hdr.NumberOfSections = rdr.ReadInt16();
            hdr.TimeDateStamp = rdr.ReadUInt32();
            hdr.PSymbolTable = rdr.ReadInt32();
            hdr.NumberOfSymbols = rdr.ReadInt32();
            hdr.SizeOfOptionalHeader = rdr.ReadUInt16();
            hdr.Flags = rdr.ReadUInt16();
            return hdr;
        }
    }
}
