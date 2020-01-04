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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Typing
{
	/// <summary>
	/// Generates traits based on an effective address expression.
	/// </summary>
	public class AddressTraitCollector : IExpressionVisitor
	{
		private TypeFactory factory;
		private ITypeStore store;
		private ITraitHandler handler;
		private Program program;

        private Expression basePointer;
        private Expression eField;
		private bool arrayContext;
		private int basePointerSize;
		private int arrayElementSize;
		private int arrayLength;

		public AddressTraitCollector(TypeFactory factory, ITypeStore store, ITraitHandler handler, Program program)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
			this.program = program;
			this.arrayContext = false;
		}

		public void Collect(Expression tvBasePointer, int basePointerSize, Expression eField, Expression effectiveAddress)
		{
			this.basePointer = tvBasePointer;
			this.basePointerSize = basePointerSize;
			this.eField = eField;
			effectiveAddress.Accept(this);
		}

		public void CollectArray(Expression tvBasePointer, Expression tvField, Expression arrayBase, int elementSize, int length)
		{
			this.basePointer = tvBasePointer;
			this.eField = tvField;
			bool c = arrayContext;
			arrayContext = true;
			arrayElementSize = elementSize;
			arrayLength = length;
			arrayBase.Accept(this);
			arrayContext = c;
		}

		public void EmitAccessTrait(Expression baseExpr, Expression memPtr, int ptrSize, int offset)
		{
			if (arrayContext)
				handler.MemAccessArrayTrait(baseExpr, memPtr, ptrSize, offset, arrayElementSize, arrayLength, eField);
			else
				handler.MemAccessTrait(baseExpr, memPtr, ptrSize, eField, offset);
		}

		public LinearInductionVariable GetInductionVariable(Expression e)
		{
			var id = e as Identifier;
			if (id == null) return null;
            LinearInductionVariable iv;
            if (!program.InductionVariables.TryGetValue(id, out iv)) return null;
            return iv;
		}

		#region IExpressionVisitor members

        public void VisitAddress(Address addr)
        {
            var offset = (int) addr.ToLinear();
            HandleConstantOffset(addr, offset);
        }

		public void VisitApplication(Application appl)
		{
			handler.MemAccessTrait(basePointer, appl, appl.DataType.Size, eField, 0);
		}

		public void VisitArrayAccess(ArrayAccess access)
		{
			handler.MemAccessTrait(basePointer, access, access.DataType.Size, eField, 0);
		}

        public void VisitBinaryExpression(BinaryExpression bin) { VisitBinaryExpression(bin.Operator, bin.DataType, bin.Left, bin.Right); }

		public void VisitBinaryExpression(Operator op, DataType dataType, Expression left, Expression right)
		{
			if (op == Operator.IAdd || op == Operator.ISub)
			{
				// Handle mem[x+const] case. Array accesses of the form
				// mem[x + (i * const) + const] will have been converted
				// to ArrayAccesses by a previous stage.

				Constant offset = right as Constant;
				if (offset != null)
				{
                    if (op == Operator.ISub)
                        offset = offset.Negate();
					var iv = GetInductionVariable(left);
                    if (iv != null)
                    {
                        VisitInductionVariable((Identifier) left, iv, offset);
                        return;
                    }
                    else if (left is BinaryExpression)
                    {
                        var bl = (BinaryExpression) left;
                        //$HACK: we've already done the analysis of the mul operator!
                        // We should be using the returned value of trait collection.
                        if (bl.Operator is IMulOperator)
                        {
                            arrayContext = true;
                            EmitAccessTrait(basePointer, left, dataType.Size, offset.ToInt32());
                            return;
                        }
                    }
                    EmitAccessTrait(basePointer, left, dataType.Size, offset.ToInt32());
					return;
				}

				// Handle odd mem[x + y] case; perhaps a later stage can detect that x (or y)
				// is a pointer and therefore y isn't.

				EmitAccessTrait(basePointer, left, dataType.Size, 0);
				return;
			}
            throw new TypeInferenceException("Couldn't generate address traits for binary operator {0}.", op);
		}

		public void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

        public void VisitConditionalExpression(ConditionalExpression c)
        {
            throw new NotImplementedException();
        }

        public void VisitConditionOf(ConditionOf cond)
		{
			throw new NotImplementedException();
		}

		public void VisitConstant(Constant c)
		{
			// Globals has a field at offset C that is a tvField: [[g->c]] = ptr(tvField)
			int v = StructureField.ToOffset(c);
            HandleConstantOffset(c, v);
		}

        private void HandleConstantOffset(Expression c, int v)
        {
            if (basePointer != null)
                handler.MemAccessTrait(null, basePointer, basePointerSize, eField, v);
            else
                handler.MemAccessTrait(null, program.Globals, program.Platform.PointerType.Size, eField, v);
            // C is a pointer to tvField: [[c]] = ptr(tvField)
            handler.MemAccessTrait(basePointer, c, c.DataType.Size, eField, 0);
        }

		public void VisitDepositBits(DepositBits dpb)
		{
			handler.MemAccessTrait(basePointer, dpb, dpb.DataType.Size, eField, 0);
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
			var iv = GetInductionVariable(id);
			if (iv != null)
			{
				VisitInductionVariable(id, iv, null);
			}
			EmitAccessTrait(basePointer, id, id.DataType.Size, 0);
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
            var tvBase = (basePointer != null) ? basePointer : program.Globals;
            var stride = Math.Abs(delta);
            int init;
            if (delta < 0)
            {
                // induction variable is decremented, so the actual array begins at ivFinal - delta.
                if (iv.Final != null)
                {
                    init = iv.Final.ToInt32() - delta;
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                }
            }
            else
            {
                if (iv.Initial != null)
                {
                    init = iv.Initial.ToInt32();
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.Size, init + offset, stride, iv.IterationCount, eField);
                    }
                }
            }
            if (iv.IsSigned)
            {
                if (cOffset != null)
                {
                    handler.MemSizeTrait(basePointer, cOffset, Math.Abs(delta));
                    EmitAccessTrait(basePointer, cOffset, cOffset.DataType.Size, 0);
                }
            }
            else
            {
                handler.MemSizeTrait(basePointer, id, Math.Abs(delta));
                EmitAccessTrait(basePointer, id, id.DataType.Size, offset);
            }
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			handler.MemAccessTrait(basePointer, mps, mps.DataType.Size, eField, 0);
		}

		public void VisitMkSequence(MkSequence seq)
		{
            if (seq.Expressions.Length == 2)
            {
                VisitBinaryExpression(Operator.IAdd, seq.DataType, seq.Expressions[0], seq.Expressions[1]);
            }
            else
            {
                throw new NotImplementedException("Does it even make sense to have a long sequence as an address?");
            }
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			handler.MemAccessTrait(basePointer, access, access.DataType.Size, eField, 0);
		}

		public void VisitSegmentedAccess(SegmentedAccess access)
		{
			handler.MemAccessTrait(basePointer, access, access.DataType.Size, eField, 0);
		}

        public void VisitOutArgument(OutArgument outArg)
        {
            throw new NotImplementedException();
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
			handler.DataTypeTrait(pc, program.Platform.PointerType);
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
