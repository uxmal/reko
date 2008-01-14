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

using Decompiler.Core.Types;
using System;
using System.Collections;

namespace Decompiler.Typing
{
	/// <summary>
	/// If a particular class is of the type "primitive" or "ptr(...)", replace all references to it.
	/// </summary>
	public class PtrPrimitiveReplacer : DataTypeTransformer
	{
		private TypeFactory factory;
		private TypeStore store; 
		private bool changed;

		public PtrPrimitiveReplacer(TypeFactory factory, TypeStore store)
		{
			this.factory = factory;
			this.store = store;
		}

		private void DumpStore()
		{
			string tmpdir = Environment.GetEnvironmentVariable("TEMP");
			string outfile = System.IO.Path.Combine(tmpdir, "storedump.txt");
			using (System.IO.StreamWriter w = new System.IO.StreamWriter(outfile, true))
			{
				w.WriteLine("====================================");
				store.Write(w);
			}
		}

		public DataType Replace(DataType dt)
		{
			return dt != null 
				? dt.Accept(this)
				: null;
		}

		public bool ReplaceAll()
		{
			changed = false;
			
			Hashtable classesInUse = new Hashtable();
			foreach (TypeVariable tv in store.TypeVariables)
			{
				EquivalenceClass eq = tv.Class;
				if (!classesInUse.Contains(eq))
				{
					classesInUse.Add(eq, eq);
					eq.DataType = Replace(eq.DataType);
				}
			}

			foreach (TypeVariable tv in store.TypeVariables)
			{
				tv.DataType = Replace(tv.DataType);
			}

			foreach (EquivalenceClass eq in classesInUse.Values)
			{
				if (eq.DataType is PrimitiveType ||
					eq.DataType is EquivalenceClass)
				{
					eq.DataType = null;
					changed = true;
					continue;
				}
				
				Pointer ptr = eq.DataType as Pointer;
				if (ptr != null)
				{
					eq.DataType = ptr.Pointee;
					changed = true;
					continue;
				}

				MemberPointer mp = eq.DataType as MemberPointer;
				if (mp != null)
				{
					eq.DataType = mp.Pointee;
					changed = true;
				}
			}
			return changed;
		}

		#region DataTypeTransformer methods //////////////////////////

		public override DataType TransformEquivalenceClass(EquivalenceClass eq)
		{
			DataType dt = eq.DataType;
			PrimitiveType pr = dt as PrimitiveType;
			if (pr != null)
			{
				changed = true;
				return pr;
			}
			Pointer ptr = dt as Pointer;
			if (ptr != null)
			{
				changed = true;
				return factory.CreatePointer(eq, ptr.Size);
			}
			MemberPointer mp = dt as MemberPointer;
			if (mp != null)
			{
				changed = true;
				return factory.CreateMemberPointer(mp.BasePointer, eq, mp.Size);
			}
			EquivalenceClass eq2 = dt as EquivalenceClass;
			if (eq2 != null)
			{
				changed = true;
				return eq2;
			}
			return eq;
		}

		public override DataType TransformTypeVar(TypeVariable tv)
		{
			throw new TypeInferenceException("Type variables mustn't occur at this stage.");
		}


		#endregion
	}
}
