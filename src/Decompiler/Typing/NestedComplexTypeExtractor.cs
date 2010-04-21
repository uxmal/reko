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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Extracts nested types from the insides of each other. Thus, (struct (0 union x)) becomes (struct (0 eq_1)) where eq_1: (union x)
	/// </summary>
	public class NestedComplexTypeExtractor : DataTypeTransformer
	{
		private TypeFactory factory;
		private TypeStore store;
		private bool insideComplexType;
		private bool changed;

		public NestedComplexTypeExtractor(TypeFactory factory, TypeStore store)
		{
			this.factory = factory;
			this.store = store;
		}

		public bool Changed
		{
			get { return changed; }
		}

		public EquivalenceClass CreateEquivalenceClass(DataType dt)
		{
			TypeVariable tv = store.EnsureExpressionTypeVariable(factory, (Decompiler.Core.Code.Expression) null);
			tv.Class.DataType = dt;
			tv.DataType = tv.Class;
			return tv.Class;
		}

		public static bool ReplaceAll(TypeFactory factory, TypeStore store)
		{
			EquivalenceClass [] eqs = new EquivalenceClass[store.UsedEquivalenceClasses.Count];
			store.UsedEquivalenceClasses.CopyTo(eqs, 0);
			bool changed = false;
			for (int i = 0; i < eqs.Length; ++i)
			{
				if (eqs[i].DataType != null)
				{
					NestedComplexTypeExtractor nctr = new NestedComplexTypeExtractor(factory, store);
					eqs[i].DataType = eqs[i].DataType.Accept(nctr);
					changed |= nctr.Changed;
				}
			}
			return changed;
		}

        public override DataType TransformArrayType(ArrayType at)
        {
            if (insideComplexType)
            {
                NestedComplexTypeExtractor nctr = new NestedComplexTypeExtractor(factory, store);
                at.Accept(nctr);
                return at;
            }
            else
            {
                insideComplexType = true;
                return base.TransformArrayType(at);
            }

        }

		public override DataType TransformStructure(StructureType str)
		{
			if (insideComplexType)
			{
				changed = true;
				NestedComplexTypeExtractor nctr = new NestedComplexTypeExtractor(factory, store);
				str.Accept(nctr);
				return CreateEquivalenceClass(str);
			}
			else
			{
				insideComplexType = true;
				return base.TransformStructure(str);
			}
		}

		public override DataType TransformUnionType(UnionType ut)
		{
			if (insideComplexType)
			{
				changed = true;
				NestedComplexTypeExtractor nctr = new NestedComplexTypeExtractor(factory, store);
				ut.Accept(nctr);
				return CreateEquivalenceClass(ut);
			}
			else
			{
				insideComplexType = true;
				return base.TransformUnionType(ut);
			}
		}
	}
}
