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
using System.IO;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents an equivalence class of types; i.e. types that are joined by equalities, function 
	/// argument assignments, phi function merges etc.
	/// </summary>
	public class EquivalenceClass : DataType
	{
        private readonly SortedSet<TypeVariable> types = new SortedSet<TypeVariable>(
            Comparer<TypeVariable>.Create((a, b) => a.Number - b.Number));

        /// <summary>
        /// Constructs an empty equivalence class with a given representative.
        /// </summary>
        /// <param name="rep">Representative type variable for the equivalence class.</param>
		public EquivalenceClass(TypeVariable rep) : this(rep, null)
		{
		}


        /// <summary>
        /// Constructs an equivalence class with a given representative and an initial data type.
        /// </summary>
        /// <param name="rep">Representative type variable for the equivalence class.</param>
        /// <param name="dt">Initial data type.</param>
        public EquivalenceClass(TypeVariable rep, DataType? dt)
            : base(Domain.Any)
        {
            Representative = rep;
            this.dt = dt;
            types.Add(rep);
        }

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitEquivalenceClass(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitEquivalenceClass(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
		{
			return this;
		}

        /// <summary>
        /// Data type of the equivalence class.
        /// </summary>
        public DataType DataType
        {
            get { return dt!; }
            set
            {
                MonitorChange(this.dt, value);
                dt = value;
            }
        }
        private DataType? dt;
        //$REVIEW Avoid making equivalence classes early to make this "nullable" unnecessary?

        /// <inheritdoc/>
        public override bool IsComplex => true;

		/// <summary>
		/// Merges two equivalence classes.
		/// </summary>
		/// <remarks>Collects all the types in both classes and puts them (arbitrarily) in the one with the lowest
		/// numbered representative.</remarks>
		/// <param name="class1"></param>
		/// <param name="class2"></param>
		/// <returns></returns>
		public static EquivalenceClass Merge(EquivalenceClass class1, EquivalenceClass class2)
		{
			if (class1 == class2)
				return class1;
			TypeVariable newRep = class1.Representative.Number <= class2.Representative.Number
				? class1.Representative
				: class2.Representative;

			if (class1.types.Count < class2.types.Count)
			{
				EquivalenceClass tmp = class1;
				class1 = class2;
				class2 = tmp;
			}

			foreach (TypeVariable tv in class2.types)
			{
				tv.Class = class1;
				class1.types.Add(tv);
			}
			class1.Representative = newRep;
			return class1;
		}

        /// <inheritdoc/>
		public override string Name
		{
			get { return "Eq_" + Representative.Number; }
		}

        /// <summary>
        /// Unique number of the equivalence class.
        /// </summary>
		public int Number
		{
			get { return Representative.Number; }
		}
		
        /// <inheritdoc/>
		public override int Size
        {
            get
            {
                if (DataType is null)
                {
                    Debug.Print("DataType of {0} is NULL!", Name); return 4;
                }
                else
                {
                    return DataType.Size;
                }
            }
			set { ThrowBadSize(); }
		}

        /// <summary>
        /// Representative type variable of the equivalence class.
        /// </summary>
		public TypeVariable Representative { get; private set; }

		/// <summary>
		/// The set of type variables that are members of this class.
		/// </summary>
        /// <remarks>
        /// //$REVIEW: this is only used for printing the typestore, and is expensive to maintain;
        /// it ruins the efficiency of the disjoint-set data structure. Consider moving this
        /// to TypeStore.
        /// </remarks>
		public ISet<TypeVariable> ClassMembers
		{
			get { return types; }
		}

        /// <summary>
        /// Writes the body of the equivalence class to a text writer.
        /// </summary>
        /// <param name="writer">Output sink.</param>
		public void WriteBody(TextWriter writer)
		{
			writer.Write("{0}: {{", Name);
			foreach (TypeVariable tv in types)
			{
				writer.Write(" {0}", tv);
			}
			writer.WriteLine(" }}");
		}

        /// <summary>
        /// Debugging method used to inspect modifications of 
        /// the <see cref="DataType"/> property of this class.
        /// </summary>
        /// <param name="dtOld">Old value of the property.</param>
        /// <param name="dtNew">New valie of the property.</param>
        [Conditional("DEBUG")]
        private void MonitorChange(DataType? dtOld, DataType? dtNew)
        {
        }
	}
}
