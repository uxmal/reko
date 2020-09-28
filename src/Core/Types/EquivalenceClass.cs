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
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Types
{
	/// <summary>
	/// Represents an equivalence class of types; i.e. types that are joined by equalities, function 
	/// argument assignments, phi function merges &c.
	/// </summary>
	public class EquivalenceClass : DataType
	{
		private TypeVariableSet types = new TypeVariableSet();

		public EquivalenceClass(TypeVariable rep) : this(rep, null)
		{
		}

        public EquivalenceClass(TypeVariable rep, DataType dt)
        {
            Representative = rep;
            DataType = dt;
            types.Add(rep);
        }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitEquivalenceClass(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitEquivalenceClass(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
		{
			return this;
		}

        public DataType DataType
        {
            get { return dt; }
            set
            {
                dt = value;
            }
        }
        private DataType dt;

		public override bool IsComplex
		{
			get { return true; }
		}

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

		public override string Name
		{
			get { return "Eq_" + Representative.Number; }
		}

		public int Number
		{
			get { return Representative.Number; }
		}
		
		public override int Size
		{
            get
            {
                if (DataType == null)
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

		public TypeVariable Representative { get; private set; }

		/// <summary>
		/// The set of type variables that are members of this class.
		/// </summary>
        /// <remarks>
        /// //$REVIEW: this is only used for printing the typestore, and is expensive to maintain;
        /// it ruins the efficiency of the disjoint-set data structure. Consider moving this
        /// to TypeStore.
        /// </remarks>
		public TypeVariableSet ClassMembers
		{
			get { return types; }
		}

		public void WriteBody(TextWriter writer)
		{
			writer.Write("{0}: {{", Name);
			foreach (TypeVariable tv in types)
			{
				writer.Write(" {0}", tv);
			}
			writer.WriteLine(" }}");
		}
	}
}
