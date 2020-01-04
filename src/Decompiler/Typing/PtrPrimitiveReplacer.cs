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

using Reko.Core;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Typing
{
	/// <summary>
	/// Replaces references to classes which are PrimitiveType, Pointer to T, or
    /// Pointer to Array of T
    /// with the actual primitive type or (ptr T) respectively.
    /// </summary>
    /// <remarks>
    /// If an expression e has the type [[e]] = Eq1 where Eq1 is PrimitiveType.Int16
    /// then after this transformation [[e]] will be PrimitiveType.Int16.
    /// If a expression e2 as the type [[e2] = Eq1 where Eq1 is Pointer(T)
    /// then after this transformation [[e2]] = Pointer(T)
    /// </remarks>
	public class PtrPrimitiveReplacer : DataTypeTransformer
	{
		private TypeFactory factory;
		private TypeStore store; 
		private bool changed;
        private Program program;
        private HashSet<EquivalenceClass> classesVisited;
        private HashSet<DataType> visitedTypes;
        private DecompilerEventListener eventListener;
        private int recursionGuard;
        private int nestCount;

        public PtrPrimitiveReplacer(TypeFactory factory, TypeStore store, Program program)
		{
			this.factory = factory;
			this.store = store;
            this.program = program;
            this.visitedTypes = new HashSet<DataType>();
		}

		public DataType Replace(DataType dt)
		{
			return dt != null 
				? dt.Accept(this)
				: null;
		}

		public bool ReplaceAll(DecompilerEventListener eventListener)
		{
			changed = false;
			classesVisited  = new HashSet<EquivalenceClass>();
            this.eventListener = eventListener;

            // Replace the DataType of all the equivalence classes
			foreach (TypeVariable tv in store.TypeVariables)
			{
                if (eventListener.IsCanceled())
                    return false;
				EquivalenceClass eq = tv.Class;
				if (!classesVisited.Contains(eq))
				{
					classesVisited.Add(eq);
                    var dt = Replace(eq.DataType);
                    eq.DataType = dt;
				}
			}

            // Replace the DataType of all the TypeVariables
			foreach (TypeVariable tv in store.TypeVariables)
			{
                if (eventListener.IsCanceled())
                    return false;
                tv.DataType = Replace(tv.DataType);
			}

			foreach (EquivalenceClass eq in classesVisited)
			{
                if (eventListener.IsCanceled())
                    return false;
                if (eq != program.Globals.TypeVariable.Class &&
                    (eq.DataType is PrimitiveType ||
                    eq.DataType is VoidType ||
					eq.DataType is EquivalenceClass ||
                    eq.DataType is CodeType))
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

                ArrayType array = eq.DataType as ArrayType;
                if (array != null)
                {
                    eq.DataType = array.ElementType;
                    changed = true;
                }
			}
			return changed;
		}

        #region DataTypeTransformer methods //////////////////////////

        public override DataType VisitArray(ArrayType at)
        {
            if (nestCount > 90)
                throw new TypeInferenceException("PprPrimitiveReplacer found a cycle in type graph.");
            ++this.nestCount;
            var dt = base.VisitArray(at);
            --this.nestCount;
            return dt;
        }

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
            if (dt is CodeType)
            {
                changed = true;
                return dt;
            }
            Pointer ptr = dt as Pointer;
            if (ptr != null)
            {
                changed = true;
                DataType pointee = eq;
                return factory.CreatePointer(pointee, ptr.BitSize);
            }
            MemberPointer mp = dt as MemberPointer;
            if (mp != null)
            {
                changed = true;
                return factory.CreateMemberPointer(mp.BasePointer, eq, mp.Size);
            }
            ArrayType array = dt as ArrayType;
            if (array != null)
            {
                changed = true;
                return factory.CreateArrayType(array.ElementType, array.Length);
            }
            EquivalenceClass eq2 = dt as EquivalenceClass;
            if (eq2 != null)
            {
                changed = true;
                return eq2;
            }
            return eq;
        }

        public override DataType VisitStructure(StructureType str)
        {
            ++recursionGuard;
            DataType dt;
            if (recursionGuard > 100)
            {
                eventListener.Warn(new NullCodeLocation(""), "Recursion too deep in PtrPrimitiveReplacer");
                dt = str;
            }
            else
            {

                //if (visitedTypes.Contains(str))
                //   return str;
                //visitedTypes.Add(str);
                dt = base.VisitStructure(str);
            }
            --recursionGuard;
            return dt;
        }

		public override DataType VisitTypeVariable(TypeVariable tv)
		{
			throw new TypeInferenceException("Type variables mustn't occur at this stage.");
		}

		#endregion
	}
}
