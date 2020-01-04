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
    /// Models C++ '&' references.
    /// </summary>
    public class ReferenceTo : DataType
    {
        public ReferenceTo(DataType referent)
        {
            this.Referent = referent;
        } 

        public DataType Referent { get; set; }

        public override int Size { get { return Referent.Size; } set { Referent.Size = value; } }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitReference(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitReference(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
        {
            return new ReferenceTo(Referent.Clone(clonedTypes)) { Qualifier = this.Qualifier };
        }
    }
}
