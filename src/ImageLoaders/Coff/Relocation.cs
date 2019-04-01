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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Coff
{
    public class Relocation :IEquatable<Relocation>, IComparable<Relocation>
    {

        /********************** Relocation types for 32-bit COFF **********************/

        public const ushort COFF32_RELOC_ABS           = 0x00;   // Ignored
        public const ushort COFF32_RELOC_DIR16         = 0x01;   // Not supported
        public const ushort COFF32_RELOC_REL16         = 0x02;   // Not supported
        public const ushort COFF32_RELOC_DIR32         = 0x06;   // 32-bit absolute virtual address
        public const ushort COFF32_RELOC_IMGREL        = 0x07;   // 32-bit image relative virtual address
        public const ushort COFF32_RELOC_SEG12         = 0x09;   // not supported
        public const ushort COFF32_RELOC_SECTION       = 0x0A;   // 16-bit section index in file
        public const ushort COFF32_RELOC_SECREL        = 0x0B;   // 32-bit section-relative
        public const ushort COFF32_RELOC_SECREL7       = 0x0D;   // 7-bit section-relative
        public const ushort COFF32_RELOC_TOKEN         = 0x0C;   // CLR token
        public const ushort COFF32_RELOC_REL32         = 0x14;   // 32-bit EIP-relative

        /********************** Relocation types for 64-bit COFF **********************/

        public const ushort COFF64_RELOC_ABS           = 0x00;   // Ignored
        public const ushort COFF64_RELOC_ABS64         = 0x01;   // 64 bit absolute virtual address
        public const ushort COFF64_RELOC_ABS32         = 0x02;   // 32 bit absolute virtual address
        public const ushort COFF64_RELOC_IMGREL        = 0x03;   // 32 bit image-relative
        public const ushort COFF64_RELOC_REL32         = 0x04;   // 32 bit, RIP-relative
        public const ushort COFF64_RELOC_REL32_1       = 0x05;   // 32 bit, relative to RIP - 1. For instruction with immediate byte operand
        public const ushort COFF64_RELOC_REL32_2       = 0x06;   // 32 bit, relative to RIP - 2. For instruction with immediate word operand
        public const ushort COFF64_RELOC_REL32_3       = 0x07;   // 32 bit, relative to RIP - 3. (useless)
        public const ushort COFF64_RELOC_REL32_4       = 0x08;   // 32 bit, relative to RIP - 4. For instruction with immediate dword operand
        public const ushort COFF64_RELOC_REL32_5       = 0x09;   // 32 bit, relative to RIP - 5. (useless)
        public const ushort COFF64_RELOC_SECTION       = 0x0A;   // 16-bit section index in file. For debug purpose
        public const ushort COFF64_RELOC_SECREL        = 0x0B;   // 32-bit section-relative
        public const ushort COFF64_RELOC_SECREL7       = 0x0C;   //  7-bit section-relative
        public const ushort COFF64_RELOC_TOKEN         = 0x0D;   // CLR token = 64 bit absolute virtual address. Inline addend ignored
        public const ushort COFF64_RELOC_SREL32        = 0x0E;   // 32 bit signed span dependent
        public const ushort COFF64_RELOC_PAIR          = 0x0F;   // pair after span dependent
        public const ushort COFF64_RELOC_PPC_REFHI     = 0x10;   // high 16 bits of 32 bit abs addr
        public const ushort COFF64_RELOC_PPC_REFLO     = 0x11;   // low  16 bits of 32 bit abs addr
        public const ushort COFF64_RELOC_PPC_PAIR      = 0x12;   // pair after REFHI
        public const ushort COFF64_RELOC_PPC_SECRELO   = 0x13;   // low 16 bits of section relative
        public const ushort COFF64_RELOC_PPC_GPREL     = 0x15;   // 16 bit signed relative to GP
        public const ushort COFF64_RELOC_PPC_TOKEN     = 0x16;    // CLR token



        internal uint VirtualAddress;     // Section-relative address of relocation source
        internal uint SymbolTableIndex;   // Zero-based index into symbol table
        internal ushort Type;             // Relocation type

        public static Relocation Load(LeImageReader rdr)
        {
            var relocation = new Relocation();

            relocation.VirtualAddress = rdr.ReadUInt32();
            relocation.SymbolTableIndex = rdr.ReadUInt32();
            relocation.Type = rdr.ReadUInt16();
            return relocation;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Relocation objAsPart = obj as Relocation;
            if (objAsPart == null)
            {
                return false;
            }
            else
            {
                return Equals(objAsPart);
            }
        }

        public int CompareTo(Relocation compareLocation)
        {
            // A null value means that this object is greater.
            if (compareLocation == null)
            {
                return 1;
            }
            else
            {
                return this.VirtualAddress.CompareTo(compareLocation.VirtualAddress);
            }
        }

        public bool Equals(Relocation other)
        {
            if (other == null)
            {
                return false;
            }
            return (this.VirtualAddress.Equals(other.VirtualAddress));
        }

        public override int GetHashCode()
        {
            return (int)VirtualAddress;
        }
    }
}
