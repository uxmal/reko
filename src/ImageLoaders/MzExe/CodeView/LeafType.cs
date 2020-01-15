#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Text;

namespace Reko.ImageLoaders.MzExe.CodeView
{
    // From:
    // Microsoft (R) C - DEVELOPER'S TOOLKIT REFERENCE
    // FOR THE MS(R) OS/2 AND MS-DOS(R) OPERATING SYSTEM

    public enum LeafType
    {
        ARRAY = 0x78,
        BARRAY = 0x8C,
        BASEDADDR = 0x97,
        BASEDSEG = 0x92,
        BASEDSEGADDR = 0x98,
        BASEDSEGVAL = 0x94,
        BASEDVAL = 0x93,
        BITFIELD = 0x5C,
        BOOLEAN = 0x6C,
        C_NEAR = 0x63,
        C_FAR = 0x64,
        CHARACTER = 0x6F,
        CONST = 0x71,
        FAR = 0x73,
        FAR_FASTCALL = 0x96,
        FSTRING = 0x8D,
        FARRIDX = 0x8E,
        HUGE = 0x5E,  
        INDEX = 0x83,
        INTEGER = 0x70,
        LABEL = 0x72,
        LIST = 0x7F,
        NEAR = 0x74,
        NEAR_FASTCALL = 0x95,
        NEWTYPE = 0x5D,
        Nil = 0x80,
        PACKED = 0x68,
        PARAMETER = 0x76,
        POINTER = 0x7A,
        PROCEDURE = 0x75,
        REAL = 0x7E,
        SCALAR = 0x7B,
        SIGNED_INTEGER = 0x7D,
        SKIP = 0x90,
        STRING = 0x82,
        STRINGS = 0x60,
        STRUCTURE = 0x79,
        TAG = 0x5A,
        UNPACKED = 0x69,
        UNSIGNED_INTEGER = 0x7C,
        VARIANT = 0x5B,
        UInt16 = 0x85,
        UInt32 = 0x86,
        Int8 = 0x88,
        Int16 = 0x89,
        Int32 = 0x8A,
    }
}
