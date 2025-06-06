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

using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Core.Types
{
	/// <summary>
	/// Every expression in the program has a type variable associated with it.
	/// </summary>
	public class TypeVariable : DataType
	{
		private DataType? dtOriginal;
        private EquivalenceClass? eqClass;

        /// <summary>
        /// Constructs a type variable.
        /// </summary>
        /// <param name="n">The identifier of the type variable.</param>
		public TypeVariable(int n) : base(Domain.Any, "T_" + n)
		{
			this.Number = n;
		}

        /// <summary>
        /// Constructs a type variable with a given name.
        /// </summary>
        /// <param name="name">Name of the type variable.</param>
        /// <param name="n">The identifier of the type variable.</param>
		public TypeVariable(string name, int n) : base(Domain.Any, name)
		{
			this.Number = n;
		}

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitTypeVariable(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitTypeVariable(this);
        }

		/// <summary>
		/// The equivalence class this type variable belongs to.
		/// </summary>
		public EquivalenceClass Class
        { 
            get { return eqClass!; }
            set { eqClass = value;  }
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
			return this;
		}

		/// <summary>
		/// Inferred DataType corresponding to type variable when equivalence class 
		/// is taken into consideration.
		/// </summary>
		public DataType DataType
        {
            get { return dt!; }
            set { MonitorChange(dt, value); dt = value; }
        }
        private DataType? dt;

        /// <summary>
        /// The number identify this type variable.
        /// </summary>
		public int Number { get; }

		/// <summary>
		/// The original inferred datatype, before the other members of the equivalence class
		/// were taken into consideration.
		/// </summary>
		public DataType OriginalDataType
		{
			get { return dtOriginal!; }
			set { MonitorChange(dtOriginal, value); dtOriginal = value; }
		}

        /// <inheritdoc/>
        public override int Size
		{
			get { return 0; }
			set { ThrowBadSize(); }
		}

        /// <summary>
        /// Debugging method used to inspect modifications of 
        /// the <see cref="DataType"/> property of this class.
        /// </summary>
        /// <param name="dtOld">Old value of the property.</param>
        /// <param name="dtNew">New valie of the property.</param>
        [Conditional("DEBUG")]
        private void MonitorChange(DataType? dtOld, DataType dtNew)
        {
        }
    }
}
