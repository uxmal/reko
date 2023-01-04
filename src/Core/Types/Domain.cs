#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Types
{
    /// <summary>
    /// A Domain specifies the possible interpretation of a datum.
    /// </summary>
    /// <remarks>
    /// A 32-bit load from memory could mean that the variable could be 
    /// treated as an signed int, unsigned int, floating point number, a 
    /// pointer to something. As the decompiler records how the value is used,
    /// some of these alternatives will be discarded. For instance, if the
    /// 32-bit word is used in a memory access, it is certain that it is a
    /// pointer to (something), and it can't be a float.
    /// </remarks>
	[Flags]
    public enum Domain
    {
        // Domain of the Unit, or 'void' data type. 
        None = 0,

        Boolean = 1,                // f
        Character = 2,              // c
        SignedInt = 4,              // i 
        UnsignedInt = 8,            // u
        Integer = SignedInt | UnsignedInt,
        Bcd = 16,                   // b - Binary coded decimal; a decimal digit stored in each nybble of a byte.
        Real = 32,                  // r
        Pointer = 64,               // p
        Offset = 128,               // n - "near pointer" (x86)
        Selector = 256,             // S
        SegPointer = 512,           // P - Segmented pointer (x86-style)
        Enum = 0x400,               // An enumerated type.

        // Composite types are constructed from other types.
        Composite = 0x1000,
        Structure = 0x2000 | Composite, // A product type (T_1 x T_2 x ... x T_n)
        Array = 0x4000 | Composite,     // An array of values
        Union = 0x5000 | Composite,     // A union type
        Class = 0x8000 | Composite,     // A C++ class, a fancy version of Structure
        Function = 0xA000 | Composite,  // Executable code.

        Any = Boolean | Character | SignedInt | UnsignedInt | Bcd | Real | Pointer | Offset | Selector | SegPointer | Enum 
            | Structure | Array | Union | Class | Function
    }
}
