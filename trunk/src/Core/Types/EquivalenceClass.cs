/* 
 * Copyright (C) 1999-2008 John Källén.
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

using System;
using System.Collections;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents an equivalence class of types; i.e. types that are joined by equalities, function 
	/// argument assignments, phi function merges &c.
	/// </summary>
	public class EquivalenceClass : DataType
	{
		private TypeVariable representative;
		private DataType dataType;
		private TypeVariableSet types = new TypeVariableSet();

		public EquivalenceClass(TypeVariable rep)
		{
			representative = rep;
			types.Add(rep);
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformEquivalenceClass(this);
		}

		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitEquivalenceClass(this);
		}

		public override DataType Clone()
		{
			return this;
		}


		public DataType DataType
		{
			get { return dataType; }
			set { dataType = value; }
		}

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
			TypeVariable newRep = class1.representative.Number <= class2.representative.Number
				? class1.representative
				: class2.representative;

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
			class1.representative = newRep;
			return class1;
		}

		public override string Name
		{
			get { return "Eq_" + representative.Number; }
		}

		public int Number
		{
			get { return representative.Number; }
		}
		
		public override int Size
		{
			get { return DataType.Size; }
			set { ThrowBadSize(); }
		}

		public TypeVariable Representative
		{
			get { return representative; }
		}

		/// <summary>
		/// The set of type variables that are members of this class.
		/// </summary>
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
