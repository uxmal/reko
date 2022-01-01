#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.IO;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a pointer type. Pointers point to another type, and have a size.
	/// </summary>
	public class Pointer : DataType
	{
		private readonly int bitSize;

		public Pointer(DataType pointee, int bitSize)
		{
            if (bitSize <= 0)
                throw new ArgumentOutOfRangeException("bitSize", "Invalid pointer size.");
            this.Pointee = pointee;
			this.bitSize = bitSize;
		}
        public override int BitSize => this.bitSize;

        public override bool IsComplex => true;

        public override bool IsPointer => true;

        public DataType Pointee { get; set; }


        public override int Size
        {
            get { return (bitSize + (BitsPerByte - 1)) / BitsPerByte; }
            set { ThrowBadSize(); }
        }


        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitPointer(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitPointer(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
            return new Pointer(Pointee.Clone(clonedTypes), bitSize)
            {
                Qualifier = this.Qualifier,
            };
		}
	}
}
