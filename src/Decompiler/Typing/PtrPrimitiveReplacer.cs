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

using Reko.Core;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
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
		private readonly TypeFactory factory;
		private readonly TypeStore store; 
        private readonly Program program;
        private readonly HashSet<EquivalenceClass> classesVisited;
        private readonly IDecompilerEventListener eventListener;
        private int recursionGuard;
        private int nestCount;
        private bool changed;

        public PtrPrimitiveReplacer(TypeFactory factory, TypeStore store, Program program, IDecompilerEventListener eventListener)
		{
			this.factory = factory;
			this.store = store;
            this.program = program;
            this.eventListener = eventListener;
            this.classesVisited = new HashSet<EquivalenceClass>();
        }

		public DataType? Replace(DataType? dt)
		{
			return dt?.Accept(this);
		}

		public bool ReplaceAll()
		{
			changed = false;
            classesVisited.Clear();

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
                    eq.DataType = dt!;
				}
			}

            // Replace the DataType of all the TypeVariables
			foreach (TypeVariable tv in store.TypeVariables)
			{
                if (eventListener.IsCanceled())
                    return false;
                tv.DataType = Replace(tv.DataType)!;
			}

			foreach (EquivalenceClass eq in classesVisited)
			{
                if (eventListener.IsCanceled())
                    return false;
                if (eq != store.GetTypeVariable(program.Globals).Class &&
                    (eq.DataType is PrimitiveType ||
                    eq.DataType is VoidType ||
					eq.DataType is EquivalenceClass ||
                    eq.DataType is CodeType))
				{
					eq.DataType = null!;
					changed = true;
					continue;
				}

                if (eq.DataType is Pointer ptr)
                {
                    eq.DataType = ptr.Pointee;
                    changed = true;
                    continue;
                }

                if (eq.DataType is MemberPointer mp)
                {
                    eq.DataType = mp.Pointee;
                    changed = true;
                }

                if (eq.DataType is ArrayType array)
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
            {
                eventListener.Error(
                    new NullCodeLocation("TypeAnalysis"),
                    "PprPrimitiveReplacer found a cycle in type graph.");
                return new UnknownType();
            }
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
                if (eq.DataType is not null)
                {
                    eq.DataType = eq.DataType.Accept(this);
                }
            }

            DataType? dt = eq.DataType;
            if (dt is PrimitiveType pr)
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
            if (dt is Pointer ptr)
            {
                changed = true;
                DataType pointee = eq;
                return factory.CreatePointer(pointee, ptr.BitSize);
            }
            if (dt is MemberPointer mp)
            {
                changed = true;
                return factory.CreateMemberPointer(mp.BasePointer, eq, mp.BitSize);
            }
            if (dt is ArrayType array)
            {
                changed = true;
                return factory.CreateArrayType(array.ElementType, array.Length);
            }
            if (dt is EquivalenceClass eq2)
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
                eventListener.Warn("Recursion too deep in PtrPrimitiveReplacer");
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
