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
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Runtime.CompilerServices;

namespace Reko.Typing
{
    /// <summary>
    /// Generates traits based on an effective address expression.
    /// </summary>
    public class AddressTraitCollector : IExpressionVisitor
	{
		private readonly TypeFactory factory;
		private readonly ITypeStore store;
		private readonly ITraitHandler handler;
		private readonly Program program;

        private Expression? basePointer;
        private Expression? eField;
		private bool arrayContext;
		private int basePointerBitSize;
		private int arrayElementSize;
		private int arrayLength;

        /// <summary>
        /// Constructs an instance of <see cref="AddressTraitCollector"/>.
        /// </summary>
        /// <param name="factory">Type factory to use when creating types.</param>
        /// <param name="store">Type store to maintain type variables and equivalence classes.
        /// </param>
        /// <param name="handler"><see cref="ITraitHandler"/> instance that will be called
        /// when addresses are encountered.</param>
        /// <param name="program">Program being analyzed.
        /// </param>
		public AddressTraitCollector(TypeFactory factory, ITypeStore store, ITraitHandler handler, Program program)
		{
			this.factory = factory;
			this.store = store;
			this.handler = handler;
			this.program = program;
			this.arrayContext = false;
		}

        /// <summary>
        /// Collect an address trait.
        /// </summary>
        /// <param name="tvBasePointer">Optional base pointer or segment selector.</param>
        /// <param name="basePointerBitSize">Bitsize of the base pointer.</param>
        /// <param name="eField">Expression accesssing a field.</param>
        /// <param name="effectiveAddress">Effective address of the access.</param>
		public void Collect(Expression? tvBasePointer, int basePointerBitSize, Expression eField, Expression effectiveAddress)
		{
			this.basePointer = tvBasePointer;
			this.basePointerBitSize = basePointerBitSize;
			this.eField = eField;
			effectiveAddress.Accept(this);
		}

        /// <summary>
        /// Collect an array access trait.
        /// </summary>
        /// <param name="tvBasePointer">Optional base pointer or segment selector.</param>
        /// <param name="tvField">Expression accesssing a field.</param>
        /// <param name="arrayBase">Array base expression.</param>
        /// <param name="elementSize">Size of each element in storage units.</param>
        /// <param name="length">Count of elements.</param>
		public void CollectArray(Expression? tvBasePointer, Expression tvField, Expression arrayBase, int elementSize, int length)
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

		private void EmitAccessTrait(Expression? baseExpr, Expression memPtr, int ptrBitSize, int offset)
		{
			if (arrayContext)
				handler.MemAccessArrayTrait(baseExpr, memPtr, ptrBitSize, offset, arrayElementSize, arrayLength, eField!);
			else
				handler.MemAccessTrait(baseExpr, memPtr, ptrBitSize, eField!, offset);
		}

		private LinearInductionVariable? GetInductionVariable(Expression e)
		{
            if (!(e is Identifier id)) return null;
            if (!program.InductionVariables.TryGetValue(id, out LinearInductionVariable? iv))
                return null;
            return iv;
		}

		#region IExpressionVisitor members

        /// <inheritdoc/>
        public void VisitAddress(Address addr)
        {
            var offset = (int) addr.ToLinear();
            HandleConstantOffset(addr, offset);
        }

        /// <inheritdoc/>
		public void VisitApplication(Application appl)
		{
			handler.MemAccessTrait(basePointer, appl, appl.DataType.BitSize, eField!, 0);
		}

        /// <inheritdoc/>
		public void VisitArrayAccess(ArrayAccess access)
		{
			handler.MemAccessTrait(basePointer, access, access.DataType.BitSize, eField!, 0);
		}

        /// <inheritdoc/>
        public void VisitBinaryExpression(BinaryExpression bin) { VisitBinaryExpression(bin.Operator.Type, bin.DataType, bin.Left, bin.Right); }

		private void VisitBinaryExpression(OperatorType op, DataType dataType, Expression left, Expression right)
		{
			if (op == OperatorType.IAdd || op == OperatorType.ISub)
			{
                // Handle mem[x+const] case. Array accesses of the form
                // mem[x + (i * const) + const] will have been converted
                // to ArrayAccesses by a previous stage.

                if (right is Constant offset)
                {
                    if (op == OperatorType.ISub)
                        offset = offset.Negate();
                    var iv = GetInductionVariable(left);
                    if (iv is not null)
                    {
                        VisitInductionVariable((Identifier) left, iv, offset);
                        return;
                    }
                    else if (left is BinaryExpression bl)
                    {
                        //$HACK: we've already done the analysis of the mul operator!
                        // We should be using the returned value of trait collection.
                        if (bl.Operator is IMulOperator)
                        {
                            arrayContext = true;
                            EmitAccessTrait(basePointer, left, dataType.BitSize, offset.ToInt32());
                            return;
                        }
                    }
                    EmitAccessTrait(basePointer, left, dataType.BitSize, offset.ToInt32());
                    return;
                }

                // Handle odd mem[x + y] case; perhaps a later stage can detect that x (or y)
                // is a pointer and therefore y isn't.

                EmitAccessTrait(basePointer, left, dataType.BitSize, 0);
				return;
			}
            throw new TypeInferenceException($"Couldn't generate address traits for binary operator {op}.");
		}

        /// <inheritdoc/>
		public void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

        /// <inheritdoc/>
        public void VisitConditionalExpression(ConditionalExpression c)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitConditionOf(ConditionOf cond)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public void VisitConstant(Constant c)
        {
            // Globals has a field at offset C that is a tvField: [[g->c]] = ptr(tvField)
            int v = StructureField.ToOffset(c) ?? 0;
            HandleConstantOffset(c, v);
        }

        private void HandleConstantOffset(Expression c, int v)
        {
            if (basePointer is not null)
                handler.MemAccessTrait(null, basePointer, basePointerBitSize, eField!, v);
            else
                handler.MemAccessTrait(null, program.Globals, program.Platform.PointerType.BitSize, eField!, v);
            // C is a pointer to tvField: [[c]] = ptr(tvField)
            handler.MemAccessTrait(basePointer, c, c.DataType.BitSize, eField!, 0);
        }

        /// <inheritdoc/>
        public void VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
        }

        /// <inheritdoc/>
		public void VisitDereference(Dereference deref)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
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
			if (iv is not null)
			{
				VisitInductionVariable(id, iv, null);
			}
			EmitAccessTrait(basePointer, id, id.DataType.BitSize, 0);
		}

        /// <summary>
        /// Handle an expression of type 'id + offset', where id is a LinearInductionVariable.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="iv"></param>
        /// <param name="cOffset"></param>
		public void VisitInductionVariable(Identifier id, LinearInductionVariable iv, Constant? cOffset)
		{
            int delta = iv.Delta!.ToInt32();
            int offset = StructureField.ToOffset(cOffset) ?? 0;
            var tvBase = basePointer ?? program.Globals;
            var stride = Math.Abs(delta);
            int init;
            if (delta < 0)
            {
                // induction variable is decremented, so the actual array begins at ivFinal - delta.
                if (iv.Final is not null)
                {
                    init = iv.Final.ToInt32() - delta;
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset!.DataType.BitSize, init + offset, stride, iv.IterationCount, eField!);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, eField!);
                    }
                }
            }
            else
            {
                if (iv.Initial is not null)
                {
                    init = iv.Initial.ToInt32();
                    if (iv.IsSigned)
                    {
                        handler.MemAccessArrayTrait(null, tvBase, cOffset!.DataType.BitSize, init + offset, stride, iv.IterationCount, eField!);
                    }
                    else
                    {
                        handler.MemAccessArrayTrait(null, tvBase, id.DataType.BitSize, init + offset, stride, iv.IterationCount, eField!);
                    }
                }
            }
            if (iv.IsSigned)
            {
                if (cOffset is not null)
                {
                    handler.MemSizeTrait(basePointer, cOffset, Math.Abs(delta));
                    EmitAccessTrait(basePointer, cOffset, cOffset.DataType.BitSize, 0);
                }
            }
            else
            {
                handler.MemSizeTrait(basePointer, id, Math.Abs(delta));
                EmitAccessTrait(basePointer, id, id.DataType.BitSize, offset);
            }
		}

        /// <inheritdoc/>
		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			handler.MemAccessTrait(basePointer, mps, mps.DataType.BitSize, eField!, 0);
		}

        /// <inheritdoc/>
		public void VisitMkSequence(MkSequence seq)
		{
            if (seq.Expressions.Length == 2)
            {
                VisitBinaryExpression(OperatorType.IAdd, seq.DataType, seq.Expressions[0], seq.Expressions[1]);
            }
            else
            {
                throw new NotImplementedException("Does it even make sense to have a long sequence as an address?");
            }
		}

        /// <inheritdoc/>
		public void VisitMemoryAccess(MemoryAccess access)
		{
			handler.MemAccessTrait(basePointer, access, access.DataType.BitSize, eField!, 0);
		}

        /// <inheritdoc/>
		public void VisitSegmentedAddress(SegmentedPointer access)
		{
            access.Offset.Accept(this);
		}

        /// <inheritdoc/>
        public void VisitOutArgument(OutArgument outArg)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
		public void VisitPhiFunction(PhiFunction phi)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public void VisitPointerAddition(PointerAddition padd)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public void VisitProcedureConstant(ProcedureConstant pc)
		{
			handler.DataTypeTrait(pc, program.Platform.PointerType);
		}

        /// <inheritdoc/>
        public void VisitScopeResolution(ScopeResolution scope)
        {
        }

        /// <inheritdoc/>
        public void VisitSlice(Slice slice)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public void VisitStringConstant(StringConstant str)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
		public void VisitTestCondition(TestCondition test)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public void VisitUnaryExpression(UnaryExpression unary)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
