#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Types;

namespace Reko.Gui
{
    /// <summary>
    /// Parses "hungarian strings" into DataTypes.
    /// </summary>
    /*

    <datadefinition> ::=

        <aligned-typespec>            // decompiler generates a suitable name

        <aligned-typespec> <identifier>    // user supplies the name

    <aligned-typespec> ::=

        <typespec>            // environment defines alignment

        <typespec>@<bitsize>    // align to bitsize boundary

    <typespec> ::=

        q<typespec>    // offset to, used by x86-style segmented pointers.

        q<segment-identifier>:<typespec>    // offset into a given segment (x86 only)

        a<array-dimensions><typespec>    // row-major array of…

        fn        // procedure 

        x        // executable code (like branch of a case statement)


        wsi<bitsize>    // 16-bit character string with length prefix

        ‘<identifier>’    // structure / union name        


    <array-dimensions> ::= 

            // it’s OK to omit size, the size of the selection will be used

        (,<dimension-size>)*    // you may omit the first dimension

        <dimension-size>(,<dimension-size>)*

    <dimension-size> ::=

        <decimal integer>   // size in decimal

        $<hex integer>      // size in hexadecimal

        x<hex integer>      // size in hexadecimal

        o<octal integer>    // size in octal

    <segment-identifier> ::=

        x<hex integer>    // segment selector in hexadecimal

        ‘<identifier>’        // named segment

Some examples:

    ai32           (array of 32-bit signed integers)

    aiw        (array of 32-bit signed integers -- same as above)

    au16        (array of 16-bit unsigned integers)

    ab        (array of bytes)

    ach buf    (array of 8-bit characters named ‘buf’)

    a100wch    (array of 100 16-bit characters)

    ax100wch    (array of 256 16-bit characters)    

    pch        (pointer to 8-bit characters)

    pfn __ctor    (pointer to procedure named ‘__ctor’)

    apx        (array of pointers to executable code)

    r64        (64-bit double precision floating point constant)

    a4,4r32    (4 x 4 array of 32-bit floats -- homogeneous matrix)

    a,4r32     (array of arrays of 4 32-bit floats  --compare float (*)[4])

    qu8        (16-bit offset to unsigned 8-bit integer -- in x86 segmented architecture)

    aqx1432:i16    (array of offsets to 16-bit integers in the segment 0x1432)

    aq’mySeg’:p    (array of offsets to 32-bit pointers in the segment named ‘mySeg’)

    ‘’         (structure of unknown type name. Decompiler generates a  type)



     */
    public class HungarianParser
    {
        private string str;
        private int i;

        private HungarianParser(string str)
        {
            //Debug.Print("Parsing " + hungarianString);
            this.str = str;
            this.i = 0;
        }

        public static DataType? Parse(string hungarianString)
        {
            if (hungarianString is null)
                return null;
            var parser = new HungarianParser(hungarianString);
            return parser.Parse(PrimitiveType.Char);
        }

        private DataType Parse(PrimitiveType charPrefix)
        {
            if (i >= str.Length)
                return new UnknownType();
            int bitSize;
            switch (str[i++])
            {
            case 'a':
                return ParseArray();
            case 'b':
                return PrimitiveType.Byte;
            case 'p':
                if (i + 2 <= str.Length && str[i] == 'f' && str[i + 1] == 'n')
                {
                    i += 2;
                    return new Pointer(new CodeType(), 32);
                }
                var pointee = Parse(PrimitiveType.Char);
                if (pointee is UnknownType)
                    return PrimitiveType.Ptr32;     //$ARch-dependent?
                else
                    return new Pointer(pointee, 32);     //$ARCH-dependent!
            case 'i':
                bitSize = ParseBitSize();
                if (bitSize == 0)
                    return PrimitiveType.Int32;         // Convenient for 2015... most ints are 32 in C code.
                return PrimitiveType.Create(Domain.SignedInt, bitSize);
            case 'u':
                bitSize = ParseBitSize();
                if (bitSize == 0)
                {
                    return PrimitiveType.UInt32;            //$REVIEW: arch word size?
                }
                return PrimitiveType.Create(Domain.UnsignedInt, bitSize);
            case 'r':
                bitSize = ParseBitSize();
                if (bitSize == 0)
                    return new UnknownType();
                return PrimitiveType.Create(Domain.Real, bitSize);
            case 'f':
                return PrimitiveType.Bool;
            case 'c':
                if (i < str.Length && str[i] == 'h')
                {
                    ++i;
                    return charPrefix;
                }
                return new UnknownType();
            case 's':
                if (i < str.Length)
                {
                    switch (str[i++])
                    {
                    case 'z':  return StringType.NullTerminated(charPrefix);
                    case 'i': return ParseLengthPrefixString(charPrefix);
                    }
                    --i;
                }
                return new ArrayType(charPrefix, 0);
            case 'w':
                if (i < str.Length)
                {
                    var dt = Parse(PrimitiveType.WChar);
                    if (dt is UnknownType)
                        dt = PrimitiveType.Word32;
                    return dt;
                }
                return PrimitiveType.Word32;
            case 'x':
                return new CodeType();
            }
            return new UnknownType();
        }

        /// <summary>
        ///     <bitsize> ::=
        /// 
        /// 8 | 16 | 32 | 64    // no support for 36-bit architectures yet
        /// 
        /// b    // 8 bits
        /// 
        /// h    // 16- bits
        /// 
        /// w    // 32-bits
        /// 
        /// d    // 64-bits
        /// 
        ///        t    // 80-bits (x86 defines this)
        /// </summary>
        /// <returns></returns>
        private int ParseBitSize()
        {
            int size = 0;
            if (i >= str.Length)
                return 0;
            switch (str[i++])
            {
            case 'b': return 8;
            case 's':
            case 'h': return 16;
            case 'w': return 32;
            case 'l': return 64;
            }
            --i;

            while (i < str.Length)
            {
                int n = str[i] - '0';
                if (0 <= n && n <= 9)
                {
                    ++i;
                    size = size * 10 + n;
                }
                else
                    break;
            }
            return size;
        }

        private ArrayType ParseArray()
        {
            if (i >= str.Length)
                return new ArrayType(new UnknownType(), 0);

            var elemType = Parse(PrimitiveType.Char);
            return new ArrayType(elemType, 0);
        }

        private DataType ParseLengthPrefixString(DataType charType)
        {
            int length = ParseBitSize();
            if (length == 0 ||
                (length != 8 && length != 16 && length != 32))
                return new UnknownType();
                return new StructureType
                {
                    Fields = {
                        new StructureField(0, PrimitiveType.Create(Domain.SignedInt, length), "length"),
                        new StructureField(1, new ArrayType(PrimitiveType.Char, 0), "chars"),
                    }
                };
        }
    }
}
