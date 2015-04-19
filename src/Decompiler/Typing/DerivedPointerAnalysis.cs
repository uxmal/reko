#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
    /// Determines whether something is a pointer.
    /// </summary>
    /// <remarks>
    /// Given an expression of type (ptr(X)):
    ///  [[C]] = ptr(T)                                      => merge field of type T at offset C in globals
    ///  [[C]] = memptr(R,T)                                 => merge field of type T at offset C in R
    ///  [[x + C]] = ptr(T) and [[x]] = ptr(S)               => merge field of type T at offset C in S
    ///  [[x + C]] = memptr(*,T) and [[x]] = memptr(*,S)     => merge field of type T at offset C in S
    /// </remarks>
	/// Some useful inferences can be made when looking at expressions like
	///     reg + Const
	/// if [[reg]] = ptr(struct(...))
	public class DerivedPointerAnalysis : InstructionVisitorBase
	{
		private TypeFactory factory;
		private TypeStore store;
		private Program prog;
		private Identifier globals;
		private Unifier unifier;

		public DerivedPointerAnalysis(TypeFactory factory, TypeStore store, Program prog)
		{
			this.factory = factory;
			this.store = store;
            this.unifier = new DataTypeBuilderUnifier(factory, store);
            this.prog = prog;
		}

		public Pointer CreatePointerToField(int offset, DataType tvField)
		{
			return factory.CreatePointer(
				factory.CreateStructureType(null, 0, new StructureField(offset, tvField)),
				prog.Platform.PointerType.Size);
		}

		public void FollowDerivedPointers()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
                foreach (var stm in proc.Statements)
                {
                    stm.Instruction.Accept(this);
                }
			}
		}

		private TypeVariable GetTypeVariableForField(DataType fieldType)
		{
			StructureType s = fieldType as StructureType;
			if (s != null)
			{
				StructureField f = s.Fields.AtOffset(0);
				if (f == null)
					return null;
				return f.DataType as TypeVariable;
			}
			FunctionType fn = fieldType as FunctionType;
			if (fn != null)
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
					return prog.Globals;
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

        public override void VisitBinaryExpression(BinaryExpression binExp)
        {
            base.VisitBinaryExpression(binExp);
            if (binExp.Operator == Operator.IAdd)
            {
                var ptSum = ResolveAs<Pointer>(binExp.TypeVariable.DataType);
                var ptAddend = ResolveAs<Pointer>(binExp.Left.TypeVariable.DataType);
                var prtSum = ResolveAs < PrimitiveType> (binExp.TypeVariable.DataType);
                var prtAddend = ResolveAs<PrimitiveType>(binExp.Left.TypeVariable.DataType);
                var c = binExp.Right as Constant;
                if (ptSum != null)
                {
                    if (ptAddend != null && c != null)
                    {
                        var strAddend = ResolveAs<StructureType>(ptAddend.Pointee);
                        if (strAddend != null)
                        {
                            strAddend.Fields.Add(c.ToInt32(), ptSum.Pointee);
                        }
                    }
                    else if (prtAddend != null && prtAddend.Domain == Domain.Pointer && c != null)
                    {
                        if (prtAddend != null && prtAddend.Domain == Domain.Pointer && ptAddend != null && c != null)
                        {
                        }
                    }
                }
            }
        }

		public override void VisitConstant(Constant c)
		{
			 DataType dt = c.TypeVariable.DataType;
            int offset = StructureField.ToOffset(c);
			Pointer ptr = dt as Pointer;
			if (ptr != null)
			{
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
                    strGlobals.Fields.Add(offset, pointee);
                }
				return;
			}
			MemberPointer mptr = dt as MemberPointer;
			if (mptr != null)
			{
                // C is a constant offset into a segment.
                var seg = ((Pointer) mptr.BasePointer).Pointee.ResolveAs<StructureType>();
                if (seg.Fields.AtOffset(offset) == null)
                {
                    seg.Fields.Add(offset, mptr.Pointee);
                }
//				VisitConstantMemberPointer(offset, mptr);
			}
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
