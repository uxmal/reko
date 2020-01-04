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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Typing
{
	/// <summary>
    /// Determines whether a constaint is a pointer, and if so adds it to 
    /// the global data structure.
    /// </summary>
    /// <remarks>
    /// Given an expression of type (ptr(X)):
    ///  [[C]] = ptr(T)                                      => merge field of type T at offset C in globals
    ///  [[C]] = memptr(R,T)                                 => merge field of type T at offset C in R
    /// </remarks>
	public class ConstantPointerAnalysis : InstructionVisitorBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private Program program;
		private Identifier globals;
		private Unifier unifier;

		public ConstantPointerAnalysis(TypeFactory factory, TypeStore store, Program program)
		{
			this.factory = factory;
			this.store = store;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.program = program;
		}

		public Pointer CreatePointerToField(int offset, DataType tvField)
		{
			return factory.CreatePointer(
				factory.CreateStructureType(null, 0, new StructureField(offset, tvField)),
				program.Platform.PointerType.BitSize);
		}

		public void FollowConstantPointers()
		{
			foreach (Procedure proc in program.Procedures.Values)
			{
                foreach (var stm in proc.Statements)
                {
                    try
                    {
                        stm.Instruction.Accept(this);
                    }
                    catch { }
                }
			}
		}

		private TypeVariable GetTypeVariableForField(DataType fieldType)
		{
            if (fieldType is StructureType s)
            {
                StructureField f = s.Fields.AtOffset(0);
                if (f == null)
                    return null;
                return f.DataType as TypeVariable;
            }
            if (fieldType is FunctionType fn)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException(string.Format("Don't know how to handle pointers to {0}.", fieldType));
		}

		public Identifier Globals
		{
			get 
			{
				if (globals != null)
				{
					return globals;
				}
				else
					return program.Globals;
			}
			set { globals = value; }
		}

        public T ResolveAs<T>(DataType dt) where T : DataType
        {
            for (; ; )
            {
                var t = dt as T;
                if (t != null)
                    return t;
                var eq = dt as EquivalenceClass;
                if (eq == null)
                    return null;
                dt = eq.DataType;   
            }
        }

		public override void VisitConstant(Constant c)
		{
            if (!c.IsValid)
                return;
			DataType dt = c.TypeVariable.DataType;
            int offset = StructureField.ToOffset(c);
            switch (dt)
            {
            case Pointer ptr:
				// C is a constant pointer.
				if (offset == 0)
					return;				// null pointer is null (//$REVIEW: except for some platforms + archs)

                var pointee = ptr.Pointee;
                var segPointee = pointee.ResolveAs<StructureType>();
                if (segPointee != null && segPointee.IsSegment)
                {
                    //$TODO: these are getting merged earlier, perhaps this is the right place to do those merges?
                    return;
                }
                var strGlobals = Globals.TypeVariable.Class.ResolveAs<StructureType>();
                if (strGlobals.Fields.AtOffset(offset) == null)
                {
                    if (!IsInsideArray(strGlobals, offset, pointee))
                    {
                        strGlobals.Fields.Add(offset, pointee);
                    }
                }
				return;
            case MemberPointer mptr:
                // C is a constant offset into a segment.
                var seg = ((Pointer) mptr.BasePointer).Pointee.ResolveAs<StructureType>();
                if (seg != null && //$DEBUG
                    seg.Fields.AtOffset(offset) == null)
                {
                    seg.Fields.Add(offset, mptr.Pointee);
                }
                //				VisitConstantMemberPointer(offset, mptr);
                return;
			}
		}

        public bool IsInsideArray(StructureType strGlobals, int offset, DataType dt)
        {
            var field = strGlobals.Fields.LowerBound(offset - 1);
            if (field == null)
                return false;
            var array = field.DataType.ResolveAs<ArrayType>();
            if (array == null)
                return false;
            return unifier.AreCompatible(array.ElementType, dt);
        }

        /// <summary>
        /// If a constant pointer into a structure is found, make sure there is a variable there.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="mptr"></param>
        private void VisitConstantMemberPointer(int offset, MemberPointer mptr)
		{
			TypeVariable tvField = GetTypeVariableForField(mptr.Pointee);
			if (tvField == null)
				return;

			TypeVariable tvBase = (TypeVariable) mptr.BasePointer;
			Pointer ptr = CreatePointerToField(offset, tvField);
			tvBase.OriginalDataType =
				unifier.Unify(tvBase.OriginalDataType, ptr);

			ptr = CreatePointerToField(offset, tvField);
			tvBase.Class.DataType =
				unifier.Unify(tvBase.Class.DataType, ptr);
		}
	}
}
