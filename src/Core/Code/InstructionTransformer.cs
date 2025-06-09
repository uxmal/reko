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
using Reko.Core.Expressions;
using System;

namespace Reko.Core.Code
{
	/// <summary>
	/// Base class for rebuilding instructions -- and expressions therein.
	/// </summary>
    /// <remarks>Use this class if most of your transformations will be simple
    /// copies, but you need to make exceptions for a few types of 
    /// expressions. If your transformation will affect most or all of the
    /// instruction and/or expression types, use <see cref="ExpressionVisitor{T}" />
    /// directly instead.</remarks>
	public class InstructionTransformer : ExpressionVisitor<Expression>
	{
        /// <summary>
        /// Applies a transformation to the given <paramref name="instr"/>.
        /// </summary>
        /// <param name="instr">Instruction to transform.</param>
        /// <returns>A transformed instruction.</returns>
        public Instruction Transform(Instruction instr)
		{
			return instr.Accept(this);
		}

        #region InstructionTransformer Members

        /// <summary>
        /// Transforms an assignment instruction.
        /// </summary>
        /// <param name="a">Assignment instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
        public virtual Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			a.Dst = (Identifier) a.Dst.Accept(this);
			return a;
		}

        /// <summary>
        /// Transforms a branch instruction.
        /// </summary>
        /// <param name="b">Branch instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformBranch(Branch b)
		{
			b.Condition = b.Condition.Accept(this);
			return b;
		}

        /// <summary>
        /// Transforms a call instruction.
        /// </summary>
        /// <param name="ci">Call instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformCallInstruction(CallInstruction ci)
		{
			ci.Callee = ci.Callee.Accept(this);
            return ci;
		}

        /// <summary>
        /// Transforms a comment.
        /// </summary>
        /// <param name="codeComment">Comment to transform.</param>
        /// <returns>Transformed instruction.</returns>
        public virtual Instruction TransformComment(CodeComment codeComment)
        {
            return codeComment;
        }

        /// <summary>
        /// Transforms a <see cref="DefInstruction"/>.
        /// </summary>
        /// <param name="def"><see cref="DefInstruction"/> to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformDefInstruction(DefInstruction def)
		{
			return def;
		}

        /// <summary>
        /// Transforms a goto instruction.
        /// </summary>
        /// <param name="gotoInstruction">Goto instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
        public virtual Instruction TransformGotoInstruction(GotoInstruction gotoInstruction)
        {
            gotoInstruction.Target = gotoInstruction.Target.Accept(this);
            return gotoInstruction;
        }

        /// <summary>
        /// Transforms a <see cref="PhiAssignment"/>.
        /// </summary>
        /// <param name="phi"><see cref="PhiAssignment"/> to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformPhiAssignment(PhiAssignment phi)
		{
            var args = phi.Src.Arguments;
			for (int i = 0; i < args.Length; ++i)
			{
                var value = args[i].Value.Accept(this);
                args[i] = new PhiArgument(args[i].Block, value);
			}
			phi.Dst = (Identifier) phi.Dst.Accept(this);
			return phi;
		}

        /// <summary>
        /// Transforms a return instruction.
        /// </summary>
        /// <param name="ret">Return instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
        public virtual Instruction TransformReturnInstruction(ReturnInstruction ret)
		{
			if (ret.Expression is not null)
				ret.Expression = ret.Expression.Accept(this);
			return ret;
		}

        /// <summary>
        /// Transforms a side effect instruction.
        /// </summary>
        /// <param name="side">Side effect instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformSideEffect(SideEffect side)
		{
			side.Expression = side.Expression.Accept(this);
			return side;
		}

        /// <summary>
        /// Transforms a store instruction.
        /// </summary>
        /// <param name="store">Store instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformStore(Store store)
		{
			store.Src = store.Src.Accept(this);
			store.Dst = store.Dst.Accept(this);
			return store;
		}

        /// <summary>
        /// Transforms a switch instruction.
        /// </summary>
        /// <param name="si">Switch instruction to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformSwitchInstruction(SwitchInstruction si)
		{
			si.Expression = si.Expression.Accept(this);
			return si;
		}

        /// <summary>
        /// Transforms a <see cref="UseInstruction"/>.
        /// </summary>
        /// <param name="u"><see cref="UseInstruction"/> to transform.</param>
        /// <returns>Transformed instruction.</returns>
		public virtual Instruction TransformUseInstruction(UseInstruction u)
		{
			u.Expression = u.Expression.Accept(this);
			return u;
		}

		#endregion

		#region IExpressionTransformer Members

        /// <inheritdoc/>
        public virtual Expression VisitAddress(Address addr)
        {
            return addr;
        }

        /// <inheritdoc/>
        public virtual Expression VisitApplication(Application appl)
        {
            appl.Procedure = appl.Procedure.Accept(this);
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i] = appl.Arguments[i].Accept(this);
            }
            return appl;
        }

        /// <inheritdoc/>
		public virtual Expression VisitArrayAccess(ArrayAccess acc)
		{
			var a = acc.Array.Accept(this);
			var i = acc.Index.Accept(this);
			return new ArrayAccess(acc.DataType, a, i);
		}

        /// <inheritdoc/>
		public virtual Expression VisitBinaryExpression(BinaryExpression binExp)
		{
			var left = binExp.Left.Accept(this);
			var right = binExp.Right.Accept(this);
            return new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
		}

        /// <inheritdoc/>
        public virtual Expression VisitCast(Cast cast)
        {
            var e = cast.Expression.Accept(this);
            return new Cast(cast.DataType, e);
        }

        /// <inheritdoc/>
        public virtual Expression VisitConditionalExpression(ConditionalExpression cond)
        {
            var c = cond.Condition.Accept(this);
            var i = cond.ThenExp.Accept(this);
            var e = cond.FalseExp.Accept(this);
            return new ConditionalExpression(cond.DataType, c, i, e);
        }

        /// <inheritdoc/>
		public Expression VisitConditionOf(ConditionOf cof)
		{
			var e = cof.Expression.Accept(this);
			return new ConditionOf(e);
		}

        /// <inheritdoc/>
		public virtual Expression VisitConstant(Constant c)
		{
			return c;
		}

        /// <inheritdoc/>
        public virtual Expression VisitConversion(Conversion conversion)
        {
            var e = conversion.Expression.Accept(this);
            return new Conversion(e, conversion.SourceDataType, conversion.DataType);
        }

        /// <inheritdoc/>
		public virtual Expression VisitDereference(Dereference deref)
		{
			var e = deref.Expression.Accept(this);
            return new Dereference(deref.DataType, e);
		}

        /// <inheritdoc/>
		public virtual Expression VisitFieldAccess(FieldAccess acc)
		{
			var str = acc.Structure.Accept(this);
            return new FieldAccess(acc.DataType, str, acc.Field);
		}

        /// <inheritdoc/>
		public virtual Expression VisitIdentifier(Identifier id)
		{
			return id;
		}

        /// <inheritdoc/>
		public virtual Expression VisitMemberPointerSelector(MemberPointerSelector mps)
		{
            var b = mps.BasePointer.Accept(this);
            var m = mps.MemberPointer.Accept(this);
			return new MemberPointerSelector(mps.DataType, b, m);
		}
		
        /// <inheritdoc/>
		public virtual Expression VisitMemoryAccess(MemoryAccess access)
		{
			var ea = access.EffectiveAddress.Accept(this);
			var memId = (Identifier) access.MemoryId.Accept(this);
			return new MemoryAccess(memId, ea, access.DataType);
		}

        /// <inheritdoc/>
		public virtual Expression VisitMkSequence(MkSequence seq)
		{
            for (int i = 0; i < seq.Expressions.Length; ++i)
            {
                seq.Expressions[i] = seq.Expressions[i].Accept(this);
            }
            return new MkSequence(seq.DataType, seq.Expressions);
		}

        /// <inheritdoc/>
        public virtual Expression VisitOutArgument(OutArgument outArg)
        {
            return new OutArgument(outArg.DataType, outArg.Expression.Accept(this));
        }

        /// <inheritdoc/>
		public virtual Expression VisitPhiFunction(PhiFunction phi)
		{
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
                var value = phi.Arguments[i].Value.Accept(this);
                phi.Arguments[i] = new PhiArgument(
                    phi.Arguments[i].Block,
                    value);
			}
			return phi;
		}

        /// <inheritdoc/>
		public virtual Expression VisitPointerAddition(PointerAddition pa)
		{
			return new PointerAddition(pa.DataType, pa.Pointer.Accept(this), pa.Offset);
		}

        /// <inheritdoc/>
		public virtual Expression VisitProcedureConstant(ProcedureConstant pc)
		{
			return pc;
		}

        /// <inheritdoc/>
        public virtual Expression VisitSegmentedAddress(SegmentedPointer address)
        {
            var sel = address.BasePointer.Accept(this);
            var offset = address.Offset.Accept(this);
            return new SegmentedPointer(address.DataType, sel, offset);
        }

        /// <inheritdoc/>
        public virtual Expression VisitScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

        /// <inheritdoc/>
		public virtual Expression VisitSlice(Slice slice)
		{
			var e = slice.Expression.Accept(this);
			return new Slice(slice.DataType, e, slice.Offset);
		}

        /// <inheritdoc/>
        public virtual Expression VisitStringConstant(StringConstant str)
        {
            return str;
        }

        /// <inheritdoc/>
        public virtual Expression VisitTestCondition(TestCondition tc)
        {
            var e = tc.Expression.Accept(this);
            return new TestCondition(tc.ConditionCode, e);
        }

        /// <inheritdoc/>
		public virtual Expression VisitUnaryExpression(UnaryExpression unary)
		{
			var e = unary.Expression.Accept(this);
			return new UnaryExpression(unary.Operator, unary.DataType, e);
		}

		#endregion
    }
}
