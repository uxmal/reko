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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Typing
{
	/// <summary>
	/// Extracts nested types from the insides of each other. Thus, (struct (0 union x)) becomes (struct (0 eq_1)) where eq_1: (union x)
	/// </summary>
	public class NestedComplexTypeExtractor : DataTypeTransformer
	{
		private readonly TypeFactory factory;
		private readonly TypeStore store;
        private readonly HashSet<DataType> visitedTypes;
		private bool insideComplexType;
		private bool changed;
        private int stackDepth; //$HACK until overly deep recursion is fixed.
 
		public NestedComplexTypeExtractor(
            TypeFactory factory,
            TypeStore store,
            int stackDepth = 0,
            HashSet<DataType>? visitedTypes = null)
		{
			this.factory = factory;
			this.store = store;
            this.stackDepth = stackDepth;
            this.visitedTypes = visitedTypes ?? new HashSet<DataType>();
		}

		public bool Changed
		{
			get { return changed; }
		}

		public EquivalenceClass CreateEquivalenceClass(DataType dt)
		{
			TypeVariable tv = store.CreateTypeVariable(factory);
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
				if (eqs[i].DataType is not null)
				{
					var nctr = new NestedComplexTypeExtractor(factory, store);
					eqs[i].DataType = eqs[i].DataType.Accept(nctr);
					changed |= nctr.Changed;
				}
			}
			return changed;
		}

        public override DataType VisitArray(ArrayType at)
        {
            if (insideComplexType)
            {
                var nctr = CreateNestedComplexTypeExtractor(factory, store);
                at.Accept(nctr);
                return at;
            }
            else
            {
                insideComplexType = true;
                return base.VisitArray(at);
            }
        }

		public override DataType VisitStructure(StructureType str)
		{
            // Do not transform user-defined types
            if (str.UserDefined)
                return str;
            if (visitedTypes.Contains(str))
                return str;
            visitedTypes.Add(str);
            if (++this.stackDepth > 50)
            {
                --this.stackDepth;
                return str;
            }
			if (insideComplexType)
			{
				changed = true;
				var nctr = CreateNestedComplexTypeExtractor(factory, store);
                str.Accept(nctr);
                var dt = CreateEquivalenceClass(str);
                --this.stackDepth;
                return dt;
            }
			else
			{
				insideComplexType = true;
                var dt = base.VisitStructure(str);
                --this.stackDepth;
                return dt;
            }
		}

		public override DataType VisitUnion(UnionType ut)
		{
            // Do not transform user-defined types
            if (ut.UserDefined)
                return ut;
            if (++this.stackDepth > 50)
            {
                --this.stackDepth;
                return ut;
            }
            if (insideComplexType)
			{

                changed = true;
				var nctr = CreateNestedComplexTypeExtractor(factory, store);
                ut.Accept(nctr);
				var eq = CreateEquivalenceClass(ut);
                --this.stackDepth;
                return eq;
			}
			else
			{
				insideComplexType = true;
				var dt = base.VisitUnion(ut);
                --this.stackDepth;
                return dt;
			}
		}

        private NestedComplexTypeExtractor CreateNestedComplexTypeExtractor(
            TypeFactory factory, TypeStore store)
        {
            return new NestedComplexTypeExtractor(
                factory, store, stackDepth, visitedTypes);
        }
    }
}
