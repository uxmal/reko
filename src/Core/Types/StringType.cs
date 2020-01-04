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
using System.Linq;
using System.Text;

namespace Reko.Core.Types
{
    /// <summary>
    /// This class is used to model strings, including size prefix and null 
    /// termination.
    /// </summary>
    /// <remarks>
    /// Strings are variable-length arrays of characters, commonly either 
    /// length-prefixed (as is the case in many Pascal implementations
    /// and Visual Basic, for instance) or zero-terminated (as is the case 
    /// for C). They share a lot of properties with ArrayType.
    /// <para>
    /// Sometimes, strings are stored in field of well-known size. In those
    /// cases, the size property will be non-zero. Otherwise, the string 
    /// size will be zero and the length of the stringmust be discovered 
    /// by walking it.</para>
    ///$TODO: what about strings where the last ASCII character has its MSBit set?
    /// </remarks>
    public class StringType : ArrayType
    {
        public static StringType NullTerminated(DataType charType)
        {
            return new StringType(charType, null, 0);
        }

        public static StringType LengthPrefixedStringType(PrimitiveType charType, PrimitiveType lengthPrefixType)
        {
            return new StringType(charType, lengthPrefixType, 0);
        }

        public StringType(DataType charType, PrimitiveType lengthPrefixType, int prefixOffset)
            : base(charType, 0)
        {
            this.LengthPrefixType = lengthPrefixType;
            this.PrefixOffset = prefixOffset;
        }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitString(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitString(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
        {
            return new StringType(this.ElementType, this.LengthPrefixType, this.PrefixOffset)
            {
                Qualifier = this.Qualifier
            };
        }

        // The type of the length prefix, if any, otherwise null.
        public PrimitiveType LengthPrefixType { get; private set; }

        // The offset from the start of the string where the length is stored.
        // This field is not valid if the LengthPrefixType is null.
        public int PrefixOffset { get; private set; }
    }
}
