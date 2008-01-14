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
	/// Walks through all union types in the type graphReplaces a union of TypeVars with a reference to an equivalence class.
	/// </summary>
	public class UnionTypeVarsReplacer : DataTypeTransformer
	{
		private TypeStore store;

		public UnionTypeVarsReplacer(TypeStore store)
		{
			this.store = store;
		}

		public override DataType TransformUnionType(UnionType ut)
		{
			ArrayList typeVars = new ArrayList();
			foreach (UnionAlternative a in ut.Alternatives)
			{
				TypeVariable tv = a.DataType as TypeVariable;
				if (tv == null)
					return ut;
				typeVars.Add(tv);
			}

			// Merge all the type variables.

			EquivalenceClass eq = null;
			foreach (TypeVariable tv in typeVars)
			{
				if (eq == null)
					eq = tv.Class;
				else
					eq = store.MergeClasses(eq.Representative, tv);
			}
			eq.DataType = ut;
			return eq.Representative;
		}
	}
}
