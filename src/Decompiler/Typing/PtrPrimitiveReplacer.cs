#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Typing
{
	/// <summary>
	/// Replaces references to classes which are PrimitiveType or Pointer to T,
    /// with the actual primitive type or (ptr T) respectively.
    /// <remarks>
    /// If an expression e has the type [[e]] = Eq1 where Eq1 is PrimitiveType.Int16
    /// then after this transformation [[e]] will be PrimitiveType.Int16.
    /// If a expression e2 as the type [[e2] = Eq1 where Eq1 is Pointer(T)
    /// then after this transformation [[e2]] = Pointer(T)
    /// </remarks>
	/// </summary>
	public class PtrPrimitiveReplacer : DataTypeTransformer
	{
		private TypeFactory factory;
		private TypeStore store; 
		private bool changed;
        private HashSet<EquivalenceClass> classesVisited;

		public PtrPrimitiveReplacer(TypeFactory factory, TypeStore store)
		{
			this.factory = factory;
			this.store = store;
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
			classesVisited  = new HashSet<EquivalenceClass>();
			foreach (TypeVariable tv in store.TypeVariables)
			{
				EquivalenceClass eq = tv.Class;
				if (!classesVisited.Contains(eq))
				{
					classesVisited.Add(eq);
					eq.DataType = Replace(eq.DataType);
				}
			}

			foreach (TypeVariable tv in store.TypeVariables)
			{
                tv.DataType = Replace(tv.DataType);
			}

			foreach (EquivalenceClass eq in classesVisited)
			{
				if (eq.DataType is PrimitiveType ||
                    eq.DataType is VoidType ||
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

        public override DataType VisitEquivalenceClass(EquivalenceClass eq)
        {
            if (!classesVisited.Contains(eq))
            {
                classesVisited.Add(eq);
                if (eq.DataType != null)
                {
                    eq.DataType = eq.DataType.Accept(this);
                }
            }

            DataType dt = eq.DataType;
            PrimitiveType pr = dt as PrimitiveType;
            if (pr != null)
            {
                changed = true;
                return pr;
            }
            if (dt is VoidType)
            {
                changed = true;
                return dt;
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

		public override DataType VisitTypeVariable(TypeVariable tv)
		{
			throw new TypeInferenceException("Type variables mustn't occur at this stage.");
		}

		#endregion
	}
}
