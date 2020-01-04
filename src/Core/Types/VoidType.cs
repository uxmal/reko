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
    public class VoidType : DataType
    {
        private VoidType() : base("void")
        {
        }

        static VoidType instance = new VoidType();

        public static VoidType Instance { get { return instance; } }

        public override int Size
        {
            get { return 0; }
            set { throw new NotSupportedException(); }
        }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitVoidType(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitVoidType(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
        {
            return this;
        }
    }
}
