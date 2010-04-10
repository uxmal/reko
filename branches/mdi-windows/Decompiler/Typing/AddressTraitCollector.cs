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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;

namespace Decompiler.Typing
{
	/// <summary>
	/// Generates traits based on an effective address expression.
	/// </summary>
	public class AddressTraitCollector : IExpressionVisitor
	{
		private TypeFactory factory;
		private TypeStore store;
		private ITraitHandler handler;
		private Program prog;

		private TypeVariable tvField;
		private bool arrayContext;
		private TypeVariable tvBasePointer;
		private int basePointerSize;
		private int arrayElementSize;
		private int arrayLength;

		public AddressTraitCollector(TypeFactory factory, TypeStore store, ITraitHandler handler, Program prog)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
			this.prog = prog;
			this.arrayContext = false;
		}

		public void Collect(TypeVariable tvBasePointer, int basePointerSize, TypeVariable tvField, Expression effectiveAddress)
		{
			this.tvBasePointer = tvBasePointer;
			this.basePointerSize = basePointerSize;
			this.tvField = tvField;
			effectiveAddress.Accept(this);
		}


		public void CollectArray(TypeVariable tvBasePointer, TypeVariable tvField, Expression arrayBase, int elementSize, int length)
		{
			this.tvBasePointer = tvBasePointer;
			this.tvField = tvField;
			bool c = arrayContext;
			arrayContext = true;
			arrayElementSize = elementSize;
			arrayLength = length;
			arrayBase.Accept(this);
			arrayContext = c;
		}

		public void EmitAccessTrait(TypeVariable tvBase, TypeVariable tvMemPtr, int ptrSize, int offset)
		{
			if (arrayContext)
				handler.MemAccessArrayTrait(tvBase, tvMemPtr, ptrSize, offset, arrayElementSize, arrayLength, tvField);
			else
				handler.MemAccessTrait(tvBase, tvMemPtr, ptrSize, tvField, offset);
		}

		public LinearInductionVariable GetInductionVariable(Expression e)
		{
			Identifier id = e as Identifier;
			if (id == null) return null;
            LinearInductionVariable iv;
            if (!prog.InductionVariables.TryGetValue(id, out iv)) return null;
            return iv;
		}

		#region IExpressionVisitor members

		public void VisitApplication(Application appl)
		{
			handler.MemAccessTrait(tvBasePointer, appl.TypeVariable, appl.DataType.Size, tvField, 0);
		}

		public void VisitArrayAccess(ArrayAccess access)
		{
			handler.MemAccessTrait(tvBasePointer, access.TypeVariable, access.DataType.Size, tvField, 0);
		}

		public void VisitBinaryExpression(BinaryExpression bin)
		{
			if (bin.op == Operator.Add || bin.op == Operator.sub)
			{
				// Handle mem[x+const] case. Array accesses of the form
				// mem[x + (i * const) + const] will have been converted
				// to ArrayAccesses by this point.

				Constant offset = bin.Right as Constant;
				if (offset != null)
				{
                    if (bin.op == Operator.sub)
                        offset = offset.Negate();
					LinearInductionVariable iv = GetInductionVariable(bin.Left);
                    if (iv != null)
                    {
                        VisitInductionVariable((Identifier) bin.Left, iv, offset);
                    }
                    else
                    {
                        EmitAccessTrait(tvBasePointer, bin.Left.TypeVariable, bin.DataType.Size, offset.ToInt32());
                    }
					return;
				}

				// Handle odd mem[x + y] case; perhaps a later stage can detect that x (or y)
				// is a pointer and therefore y isn't.

				EmitAccessTrait(tvBasePointer, bin.Left.TypeVariable, bin.DataType.Size, 0);
				return;
			}
            throw new TypeInferenceException("Couldn't generate address traits for expression {0}.", bin);
		}

		public void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

		public void VisitConditionOf(ConditionOf cond)
		{
			throw new NotImplementedException();
		}

		public void VisitConstant(Constant c)
		{
			// Globals has a field at offset C that is a tvField: [[g->c]] = ptr(tvField)
			int v = StructureField.ToOffset(c);
			if (tvBasePointer != null)
				handler.MemAccessTrait(null, tvBasePointer, basePointerSize, tvField, v);
			else
				handler.MemAccessTrait(null, prog.Globals.TypeVariable, c.DataType.Size, tvField, v);
			// C is a pointer to tvField: [[c]] = ptr(tvField)
			handler.MemAccessTrait(tvBasePointer, c.TypeVariable, c.DataType.Size, tvField, 0);
		}

		public void VisitDepositBits(DepositBits dpb)
		{
			handler.MemAccessTrait(tvBasePointer, dpb.TypeVariable, dpb.DataType.Size, tvField, 0);
		}

		public void VisitDereference(Dereference deref)
		{
			throw new NotImplementedException();
		}

		public void VisitFieldAccess(FieldAccess access)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Expression of the kind Mem[r]. 
		/// </summary>
		/// <remarks>
		/// If r is an induction variable, r points to an array element. The delta of r is therefore the 
		/// element size of the array.
		/// </remarks>
		/// <param name="id"></param>
		public void VisitIdentifier(Identifier id)
		{
			LinearInductionVariable iv = GetInductionVariable(id);
			if (iv != null)
			{
				VisitInductionVariable(id, iv, null);
			}
			EmitAccessTrait(tvBasePointer, id.TypeVariable, id.DataType.Size, 0);
		}

        /// <summary>
        /// Handle an expression of type 'id + offset', where id is a LinearInductionVariable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iv"></param>
        /// <param name="offset"></param>
		public void VisitInductionVariable(Identifier id, LinearInductionVariable iv, Constant cOffset)
		{
            int delta = iv.Delta.ToInt32();
            int offset = StructureField.ToOffset(cOffset);
            TypeVariable tvBase = (tvBasePointer != null) ? tvBasePointer : prog.Globals.TypeVariable;
            if (delta < 0)
            {
                // induction variable is decremented, so the actual array begings at ivFinal - delta.
                if (iv.Final != null)
                {
                    int init = iv.Final.ToInt32() - delta;
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset.DataType.Size, init + offset, Math.Abs(delta), iv.IterationCount, tvField);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.Size, init + offset, Math.Abs(delta), iv.IterationCount, tvField);
                    }
                }
            }
            else
            {
                if (iv.Initial != null)
                {
                    int init = iv.Initial.ToInt32();
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset.DataType.Size, init + offset, Math.Abs(delta), iv.IterationCount, tvField);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.Size, init + offset, Math.Abs(delta), iv.IterationCount, tvField);
                    }

                }
            }
            if (iv.IsSigned)
            {
                if (cOffset != null)
                {
                    handler.MemSizeTrait(tvBasePointer, cOffset.TypeVariable, Math.Abs(delta));
                    EmitAccessTrait(tvBasePointer, cOffset.TypeVariable, cOffset.DataType.Size, 0);
                }
            }
            else
            {
                handler.MemSizeTrait(tvBasePointer, id.TypeVariable, Math.Abs(delta));
                EmitAccessTrait(tvBasePointer, id.TypeVariable, id.DataType.Size, offset);
            }
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			handler.MemAccessTrait(tvBasePointer, mps.TypeVariable, mps.DataType.Size, tvField, 0);
		}

		public void VisitMkSequence(MkSequence seq)
		{
            if (arrayContext)
            {
                if (seq.Head.DataType != PrimitiveType.SegmentSelector)
                    return;
                Constant c = seq.Tail as Constant;
                if (c == null)
                    return;
                handler.MemAccessArrayTrait(null, seq.Head.TypeVariable, seq.Head.DataType.Size, c.ToInt32(), arrayElementSize, 0, tvField); 
            }
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			handler.MemAccessTrait(tvBasePointer, access.TypeVariable, access.DataType.Size, tvField, 0);
		}

		public void VisitSegmentedAccess(SegmentedAccess access)
		{
			handler.MemAccessTrait(tvBasePointer, access.TypeVariable, access.DataType.Size, tvField, 0);
		}

		public void VisitPhiFunction(PhiFunction phi)
		{
			throw new NotImplementedException();
		}

		public void VisitPointerAddition(PointerAddition padd)
		{
			throw new NotImplementedException();
		}

		public void VisitProcedureConstant(ProcedureConstant pc)
		{
			throw new NotImplementedException();
		}

		public void VisitSlice(Slice slice)
		{
			throw new NotImplementedException();
		}

		public void VisitScopeResolution(ScopeResolution scope)
		{
		}

		public void VisitTestCondition(TestCondition test)
		{
			throw new NotImplementedException();
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
