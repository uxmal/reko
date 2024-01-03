#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Text;

namespace Reko.Core.Types
{
    /// <summary>
    /// Refers to another type by name; think of C's "typedef"
    /// type builder.
    /// </summary>
    public class TypeReference : DataType
    {
        private DataType referent;

        public TypeReference(DataType dataType) : base(dataType.Domain)
        {
            this.referent = dataType;
        }

        public TypeReference(string name, DataType dataType) : base(dataType.Domain, name)
        {
            this.referent = dataType;
        }

        /// <summary>
        /// This constructor is used to generate generic type variables.
        /// </summary>
        /// <param name="name">The name of the generic type variable.</param>
        public TypeReference(string name) : base(Domain.Any, name)
        {
            // This kind of type reference is used as a placeholder, and should never
            // participate in Reko's type inference.
            this.referent = default!;
        }

        public override bool IsComplex => Referent.IsComplex;
        public override bool IsIntegral => Referent.IsIntegral;
        public override bool IsPointer => Referent.IsPointer;
        public override bool IsReal => Referent.IsReal;

        public DataType Referent
        {
            get { return referent; }
            set { referent = value; Domain = referent.Domain; }
        }

        public override int Size
        {
            get { return Referent.Size; }
            set { ThrowBadSize(); }
        }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitTypeReference(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitTypeReference(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
        {
            return this;
        }
    }
}
