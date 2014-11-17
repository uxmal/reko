#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

namespace Decompiler.Core.Types
{
    /// <summary>
    /// This class is used to model strings, including size prefix and null termination.
    /// </summary>
    /// <remarks>
    /// Strings are commonly either length-prefixed (as is the case in many Pascal implementations
    /// and Visual Basic, for instance) or zero-terminated (as is the case for C). They share
    /// a lot of properties with ArrayType.
    //$TODO: what about strings where the last ASCII character has its MSBit set?</remarks>
    public class StringType : DataType
    {
        // The type of the code units of the string
        public PrimitiveType CharType { get; private set; }
        // The type of the length prefix, if any, otherwise null.
        public PrimitiveType LengthPrefixType { get; private set; }

        public int PrefixOffset { get; private set; }

        public static StringType NullTerminated(PrimitiveType  charType)
        {
            return new StringType(charType, null, 0);
        }

        public static StringType LengthPrefixedStringType(PrimitiveType charType, PrimitiveType lengthPrefixType)
        {
            return new StringType(charType, lengthPrefixType, 0);
        }

        public StringType(PrimitiveType charType, PrimitiveType lengthPrefixType, int prefixOffset)
        {
            this.CharType = charType;
            this.LengthPrefixType = lengthPrefixType;
            this.PrefixOffset = prefixOffset;
        }

        public override int Size
        {
            get { return 0; }   // string lengths vary.
            set { throw new InvalidOperationException(); }
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitString(this);
        }

        public override DataType Clone()
        {
            return new StringType(this.CharType, this.LengthPrefixType, this.PrefixOffset);
        }
    }
}
