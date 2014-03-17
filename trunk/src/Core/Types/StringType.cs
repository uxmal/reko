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
    public class StringType : DataType
    {
        public  DataType CharType { get; private set; }
        public DataType LengthPrefixType { get; private set; }
        public int PrefixOffset { get; private set; }

        public static StringType NullTerminated(DataType charType)
        {
            return new StringType(charType, null, 0);
        }

        public static StringType LengthPrefixedStringType(DataType charType, DataType lengthPrefixType)
        {
            return new StringType(charType, lengthPrefixType, 0);
        }

        public StringType(DataType charType, DataType lengthPrefixType, int prefixOffset)
        {
            this.CharType = charType;
            this.LengthPrefixType = lengthPrefixType;
            this.PrefixOffset = prefixOffset;
        }

        public override int Size
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
