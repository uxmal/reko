#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System.Collections.Generic;

namespace Reko.Core.Types
{
    /// <summary>
    /// Represents an enumeration type.
    /// </summary>
    public class EnumType : DataType
    {
        private readonly int size;

        /// <summary>
        /// Constructs an instance of the <see cref="EnumType"/> class.
        /// </summary>
        /// <param name="name">Name of the enumerated type.</param>
        /// <param name="size">Size of the enumerated type.</param>
        /// <param name="members">The members of the enumerated type.</param>
        public EnumType(
            string name,
            int size,
            IDictionary<string, long> members)
            : base(Domain.Enum, name)
        {
            this.size = size;
            this.Members = new SortedList<string, long>(members);
        }

        /// <inheritdoc/>
        public override int Size
        {
            get { return size; }
            set { ThrowBadSize(); }
        }

        /// <summary>
        /// The members of the enumeration.
        /// </summary>
        public IReadOnlyDictionary<string, long> Members { get; }

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitEnum(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitEnum(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
        {
            return this;
        }
    }
}
