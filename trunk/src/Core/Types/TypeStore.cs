/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Stores, for a particular program, all type variables, equivalence classes, and their mappings to
	/// each other.
	/// </summary>
	public class TypeStore
	{
		private List<TypeVariable> typevars;
        private SortedList<int, EquivalenceClass> usedClasses;
        private Dictionary<TypeVariable, Expression> tvSources;
		private DataTypeComparer tycomp = new DataTypeComparer();

		public TypeStore()
		{
			typevars = new List<TypeVariable>();
			usedClasses = new SortedList<int, EquivalenceClass>();
            tvSources = new Dictionary<TypeVariable, Expression>();
		}

		public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Expression e)
		{
			return EnsureExpressionTypeVariable(factory, e, null);
		}

		public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Expression e, string name)
		{
			if (e == null || e.TypeVariable == null)
			{
				TypeVariable tv = name != null ? factory.CreateTypeVariable(name) : factory.CreateTypeVariable();
                AddDebugSource(tv, e);
				tv.Class = new EquivalenceClass(tv);
				if (e != null)
					e.TypeVariable = tv;
				typevars.Add(tv);
				usedClasses.Add(tv.Class.Number, tv.Class);
				return tv;
			}
			return e.TypeVariable;
		}

        private void AddDebugSource(TypeVariable tv, Expression e)
        {
            if (e != null)
                tvSources.Add(tv, e);
        }

        public TypeVariable EnsureFieldTypeVariable(TypeFactory factory, StructureField field)
        {
            throw new NotImplementedException();
        }

		public void Dump()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			System.Diagnostics.Debug.WriteLine(sw.ToString());
		}

        public Expression ExpressionOf(TypeVariable tv)
        {
            Expression e;
            if (tvSources.TryGetValue(tv, out e))
                return e;
            else
                return null; 
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

		public List<TypeVariable> TypeVariables
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

		public IList<EquivalenceClass> UsedEquivalenceClasses
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
                        WriteExpressionOf(tvMember, writer);
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

        public void WriteExpressionOf(TypeVariable tvMember, TextWriter writer)
        {
            Expression e;
            if (tvSources.TryGetValue(tvMember, out e) && e != null)
            {
                writer.Write(" (in {0}", e);
                if (e.DataType != null)
                {
                    writer.Write(" : ");
                    writer.Write(e.DataType);
                }
                writer.Write(")");
            }
        }

		public void WriteEntry(TypeVariable tv, TextWriter writer)
		{
			writer.Write(tv.Name);
			writer.Write(":");
            WriteExpressionOf(tv, writer);
			writer.WriteLine();

			writer.Write("  Class: ");
			writer.WriteLine(tv.Class.Name);

			writer.Write("  DataType: ");
			writer.WriteLine(tv.DataType);

			writer.Write("  OrigDataType: ");
			writer.WriteLine(tv.OriginalDataType);
		}

		private void WriteEntry(TypeVariable tv, DataType dt, TextWriter writer)
		{
			writer.Write("{0}: ", tv);
			if (dt != null)
			{
				dt.Write(writer);
                WriteExpressionOf(tv, writer);
			}

			writer.WriteLine();
		}
	}
}
