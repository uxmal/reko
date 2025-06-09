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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Typing
{
	/// <summary>
    /// Determines whether a constant is a pointer, and if so adds it to 
    /// the global data structure.
    /// </summary>
    /// <remarks>
    /// Given an expression of type (ptr(X)):
    ///  [[C]] = ptr(T)                                      => merge field of type T at offset C in globals
    ///  [[C]] = memptr(R,T)                                 => merge field of type T at offset C in R
    /// </remarks>
	public class ConstantPointerAnalysis : InstructionVisitorBase
	{
		private readonly TypeFactory factory;
        private readonly TypeStore store;
		private readonly Program program;
		private Identifier? globals;
		private readonly Unifier unifier;

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

		private static TypeVariable? GetTypeVariableForField(DataType fieldType)
		{
            if (fieldType is StructureType s)
            {
                StructureField? f = s.Fields.AtOffset(0);
                if (f is null)
                    return null;
                return f.DataType as TypeVariable;
            }
            if (fieldType is FunctionType fn)
            {
                throw new NotImplementedException();
            }
            throw new NotImplementedException($"Don't know how to handle pointers to {fieldType}.");
		}

		public Identifier Globals
		{
			get 
			{
				if (globals is not null)
				{
					return globals;
				}
				else
					return program.Globals;
			}
			set { globals = value; }
		}

        public static T? ResolveAs<T>(DataType dt) where T : DataType
        {
            for (; ; )
            {
                if (dt is T t)
                    return t;
                if (!(dt is EquivalenceClass eq))
                    return null;
                dt = eq.DataType;   
            }
        }

		public override void VisitConstant(Constant c)
		{
            if (!c.IsValid || c is BigConstant)
                return;
			DataType dt = store.GetTypeVariable(c).DataType;
            int? offset = StructureField.ToOffset(c);
            if (offset is null)
                return;
            switch (dt)
            {
            case Pointer ptr:
				// C is a constant pointer.
				if (offset.Value == 0)
					return;				// null pointer is null (//$REVIEW: except for some platforms + archs)

                var pointee = ptr.Pointee;
                var segPointee = pointee.ResolveAs<StructureType>();
                if (segPointee is not null && segPointee.IsSegment)
                {
                    //$TODO: these are getting merged earlier, perhaps this is the right place to do those merges?
                    return;
                }
                var strGlobals = store.GetTypeVariable(Globals).Class.ResolveAs<StructureType>();
                if (strGlobals!.Fields.AtOffset(offset.Value) is null)
                {
                    if (!IsInsideArray(strGlobals, offset.Value, pointee) &&
                        !IsInsideStruct(strGlobals, offset.Value))
                    {
                        strGlobals.Fields.Add(offset.Value, pointee);
                    }
                }
				return;
            case MemberPointer mptr:
                // C is a constant offset into a segment.
                var seg = ((Pointer) mptr.BasePointer).Pointee.ResolveAs<StructureType>();
                if (seg is not null && //$DEBUG
                    seg.Fields.AtOffset(offset.Value) is null)
                {
                    seg.Fields.Add(offset.Value, mptr.Pointee);
                }
                //				VisitConstantMemberPointer(offset, mptr);
                return;
			}
		}

        public bool IsInsideArray(StructureType strGlobals, int offset, DataType dt)
        {
            var field = strGlobals.Fields.LowerBound(offset - 1);
            if (field is null)
                return false;
            var array = field.DataType.ResolveAs<ArrayType>();
            if (array is null || array is StringType)
                return false;
            return unifier.AreCompatible(array.ElementType, dt);
        }

        public bool IsInsideStruct(StructureType strGlobals, int offset)
        {
            //$PERF: LowerBound have a complexity of O(n^2)
            var field = strGlobals.Fields.LowerBound(offset);
            if (field is null)
                return false;
            var str = field.DataType.ResolveAs<StructureType>();
            if (str is null)
                return false;
            return (
                offset >= field.Offset &&
                offset < field.Offset + str.GetInferredSize());
        }

        /// <summary>
        /// If a constant pointer into a structure is found, make sure there is a variable there.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="mptr"></param>
        private void VisitConstantMemberPointer(int offset, MemberPointer mptr)
		{
			TypeVariable? tvField = GetTypeVariableForField(mptr.Pointee);
			if (tvField is null)
				return;

			TypeVariable tvBase = (TypeVariable) mptr.BasePointer;
			Pointer ptr = CreatePointerToField(offset, tvField);
			tvBase.OriginalDataType =
				unifier.Unify(tvBase.OriginalDataType, ptr)!;

			ptr = CreatePointerToField(offset, tvField);
			tvBase.Class.DataType =
				unifier.Unify(tvBase.Class.DataType, ptr)!;
		}
	}
}
