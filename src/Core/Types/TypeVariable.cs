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

using Reko.Core.Code;

using System;
using System.Collections.Generic;

namespace Reko.Core.Types
{
	/// <summary>
	/// Every expression in the program has a type variable associated with it.
	/// </summary>
	public class TypeVariable : DataType
	{
		private int number;
		private DataType dtOriginal;

		public TypeVariable(int n) : base("T_" + n)
		{
			this.number = n;
		}

		public TypeVariable(string name, int n) : base(name)
		{
			this.number = n;
		}

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitTypeVariable(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitTypeVariable(this);
        }

		/// <summary>
		/// The equivalence class this type variable belongs to.
		/// </summary>
		public EquivalenceClass Class { get; set; }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
		{
			return this;
		}

		/// <summary>
		/// Inferred DataType corresponding to type variable when equivalence class 
		/// is taken into consideration.
		/// </summary>
		public DataType DataType { get { return dt; } set { dt = value; } }
        private DataType dt;

		public int Number
		{
			get { return number; }
		}

		/// <summary>
		/// The original inferred datatype, before the other members of the equivalence class
		/// were taken into consideration.
		/// </summary>
		public DataType OriginalDataType
		{
			get { return dtOriginal; }
			set { dtOriginal = value; }
		}

		public override int Size
		{
			get { return 0; }
			set { ThrowBadSize(); }
		}
	}
}
