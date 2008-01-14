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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Core.Output;
using System;
using System.Collections;
using System.IO;

namespace Decompiler.Typing
{
	/// <summary>
	/// Stores, for a particular program, all type variables, equivalence classes, and their mappings to
	/// each other.
	/// </summary>
	public class TypeStore
	{
		private TypeVariableCollection typevars;
		private SortedList usedClasses;
		private DataTypeComparer tycomp = new DataTypeComparer();

		public TypeStore()
		{
			typevars = new TypeVariableCollection();
			usedClasses = new SortedList();
		}

		public TypeVariable EnsureTypeVariable(TypeFactory factory, Expression e)
		{
			return EnsureTypeVariable(factory, e, null);
		}

		public TypeVariable EnsureTypeVariable(TypeFactory factory, Expression e, string name)
		{
			if (e == null || e.TypeVariable == null)
			{
				TypeVariable tv = name != null ? factory.CreateTypeVariable(name) : factory.CreateTypeVariable();
				tv.Expression = e;
				tv.Class = new EquivalenceClass(tv);
				if (e != null)
					e.TypeVariable = tv;
				typevars.Add(tv);
				usedClasses.Add(tv.Class.Number, tv.Class);
				return tv;
			}
			return e.TypeVariable;
		}

		public void Dump()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			System.Diagnostics.Debug.WriteLine(sw.ToString());
		}

		public EquivalenceClass MergeClasses(TypeVariable tv1, TypeVariable tv2)
		{
			EquivalenceClass class1 = tv1.Class;
			EquivalenceClass class2 = tv2.Class;
			usedClasses.Remove(class1.Number);
			usedClasses.Remove(class2.Number);
			EquivalenceClass merged = EquivalenceClass.Merge(class1, class2);
			usedClasses.Add(merged.Number, merged);
			tv1.Class = merged;
			tv2.Class = merged;
			return merged;
		}

		public TypeVariableCollection TypeVariables		//$REVIEW: consider renaming to "TypeVariables"
		{
			get { return typevars; }
		}


		/// <summary>
		/// For each equivalence class, ensures that all of its constituent type variables
		/// have the same data type.
		/// </summary>
		public void CopyClassDataTypesToTypeVariables()
		{
			foreach (TypeVariable tv in TypeVariables)
			{
				DataType dt = tv.Class.DataType;
				tv.DataType = dt;
			}
		}

		public DataType ResolvePossibleTypeVar(DataType dt)
		{
			TypeVariable tv = dt as TypeVariable;
			while (tv != null)
			{
				dt = tv.Class.DataType;
				tv = dt as TypeVariable;
			}
			EquivalenceClass eq = dt as EquivalenceClass;
			while (eq != null)
			{
				dt = eq.DataType;
				eq = dt as EquivalenceClass;
			}
			return dt;
		}

		public ICollection UsedEquivalenceClasses
		{
			get { return usedClasses.Values; }
		}

		public void Write(TextWriter writer)
		{
			writer.WriteLine("// Equivalence classes ////////////");
			foreach (TypeVariable tv in TypeVariables)
			{
				if (tv.Class.Representative == tv && tv.Class.DataType != null)
				{
					writer.WriteLine("{0}: {1}", tv.Class, tv.Class.DataType);
					foreach (TypeVariable tvMember in tv.Class.ClassMembers)
					{
						writer.Write("\t{0}", tvMember);
						if (tvMember.Expression != null)
						{
							writer.Write(" (in {0} : {1})", tvMember.Expression, tvMember.Expression.DataType);
						}
						writer.WriteLine();
					}
				}
			}

			writer.WriteLine("// Type Variables ////////////");
			foreach (TypeVariable tv in TypeVariables)
			{
				WriteEntry(tv, writer);
			}
		}

		public void WriteEntry(TypeVariable tv, TextWriter writer)
		{
			writer.Write(tv.Name);
			writer.Write(":");
			if (tv.Expression != null)
			{
				writer.Write("(in ");
				writer.Write(tv.Expression.ToString());
				writer.Write(")");
			}
			writer.WriteLine();

			writer.Write("  Class: ");
			writer.WriteLine(tv.Class.Name);

			writer.Write("  DataType: ");
			writer.WriteLine(tv.DataType);

			writer.Write("  OrigDataType: ");
			writer.WriteLine(tv.OriginalDataType);
		}

		private void WriteEntry(DictionaryEntry de, TextWriter writer)
		{
			WriteEntry((TypeVariable) de.Key, (DataType) de.Value, writer);
		}

		private void WriteEntry(TypeVariable tv, DataType dt, TextWriter writer)
		{
			writer.Write("{0}: ", tv);
			if (dt != null)
			{
				dt.Write(writer);
				if (tv != null && tv.Expression != null)
				{
					writer.Write(" (in {0})", tv.Expression);
				}
			}

			writer.WriteLine();
		}
	}
}
