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

using System;
using System.Collections.Generic;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents a pointer type. Pointers point to another type, and have a size.
	/// </summary>
	public class Pointer : DataType
	{
		private readonly int bitSize;

        /// <summary>
        /// Creates a pointer type. The pointer points to a <see cref="DataType"/> 
        /// <paramref name="pointee"/>. The pointer itself has the bit size <see cref="bitSize"/>.
        /// </summary>
        /// <param name="pointee"></param>
        /// <param name="bitSize">The size of the pointer in bits. The special value 0 means
        /// "this pointer has an unknown size". This is used in generic intrinsic signatures
        /// only, and should not occur during type reference.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
		public Pointer(DataType pointee, int bitSize)
            : base(Domain.Pointer)
		{
            if (bitSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bitSize), "Invalid pointer size.");
            this.Pointee = pointee;
			this.bitSize = bitSize;
		}

        /// <inheritdoc/>
        public override int BitSize => this.bitSize;

        /// <inheritdoc/>
        public override bool IsComplex => true;

        /// <inheritdoc/>
        public override bool IsPointer => true;

        public DataType Pointee { get; set; }


        /// <inheritdoc/>
        public override int Size
        {
            get { return (bitSize + (BitsPerByte - 1)) / BitsPerByte; }
            set { ThrowBadSize(); }
        }

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitPointer(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitPointer(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
            return new Pointer(Pointee.Clone(clonedTypes), bitSize)
            {
                Qualifier = this.Qualifier,
            };
		}
	}
}
