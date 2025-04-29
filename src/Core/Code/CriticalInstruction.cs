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

using Reko.Core.Expressions;
using System.Linq;

namespace Reko.Core.Code
{
	/// <summary>
	/// Determines whether an instruction is critical or not. Non-critical
    /// instructions are candidates for dead code elimination, if the
    /// decompiler can prove they are not used.
	/// </summary>
	public class CriticalInstruction : InstructionVisitor<bool>, ExpressionVisitor<bool>
	{
        private static readonly CriticalInstruction instance = new();

        /// <summary>
        /// Tests whether an instruction is critical.
        /// </summary>
        /// <param name="instr">Instruction to test.</param>
        /// <returns>True if the instruction is critical; otherwise false.</returns>
		public static bool IsCritical(Instruction instr)
		{
            return instr.Accept(instance);
		}

        /// <summary>
        /// Tests whether an expression is critical.
        /// </summary>
        /// <param name="expr">Expression to test.</param>
        /// <returns>True if the expression is critical; otherwise false.</returns>
		public static bool IsCritical(Expression expr)
		{
            return expr.Accept(instance);
		}

		#region InstructionVisitor //////////////////////////////////

        /// <inheritdoc/>
        public bool VisitAssignment(Assignment ass)
        {
            return ass.Src.Accept(this);
        }

        /// <summary>
        /// Branches are always critical.
        /// </summary>
        /// <param name="b">Branch being visited.</param>
        /// <returns>True.</returns>
		public bool VisitBranch(Branch b)
		{
			return true;
		}

        /// <summary>
        /// Calls are always critical.
        /// </summary>
        /// <param name="ci">Call being visited.</param>
        /// <returns>True.</returns>
		public bool VisitCallInstruction(CallInstruction ci)
		{
			return true;
		}

        /// <summary>
        /// Use comments are always critical.
        /// </summary>
        /// <param name="comment">Comment being visited.</param>
        /// <returns>True.</returns>
        public bool VisitComment(CodeComment comment)
        {
            return true;
        }

        /// <summary>
        /// Def instructions with no uses can safely be removed.
        /// </summary>
        /// <param name="def"><see cref="DefInstruction"/> being visited.</param>
        /// <returns>False.</returns>
        public bool VisitDefInstruction(DefInstruction def)
        {
            return false;
        }

        /// <summary>
        /// Goto instructions are always critical.
        /// </summary>
        /// <param name="g">Goto instruction being visited.</param>
        /// <returns>True.</returns>
        public bool VisitGotoInstruction(GotoInstruction g)
        {
            return true;
        }

        /// <summary>
        /// Phi assigments with no uses can safely be removed.
        /// </summary>
        /// <param name="phi"><see cref="PhiAssignment"/> being visited.</param>
        /// <returns>False.</returns>
        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            return false;
        }

        /// <summary>
        /// Return instructions are always critical.
        /// </summary>
        /// <param name="ret">Return instruction being visited.</param>
        /// <returns>True.</returns>
        public bool VisitReturnInstruction(ReturnInstruction ret)
		{
			return true;
		}

        /// <summary>
        /// Side effect instructions are always critical.
        /// </summary>
        /// <param name="side">Side effect being visited.</param>
        /// <returns>True.</returns>

		public bool VisitSideEffect(SideEffect side)
		{
			return true;
		}

        /// <summary>
        /// Store instructions are always critical.
        /// </summary>
        /// <param name="store">Store instruction being visited.</param>
        /// <returns>True.</returns>
		public bool VisitStore(Store store)
		{
			return true;
		}

        /// <summary>
        /// Switch instructions are always critical.
        /// </summary>
        /// <param name="si">Switch instruction being visited.</param>
        /// <returns>True.</returns>
		public bool VisitSwitchInstruction(SwitchInstruction si)
		{
			return true;
		}

        /// <summary>
        /// Use instructions are always critical, unless we can prove
        /// they're never live out of a procedure.
        /// </summary>
        /// <param name="use">Use instruction being visited.</param>
        /// <returns>True.</returns>
		public bool VisitUseInstruction(UseInstruction use)
		{
			return true;
		}

        #endregion

        #region ExpressionVisitor /////////////////////////////

        public bool VisitAddress(Address addr) => false;

        public bool VisitApplication(Application appl)
        {
            var argsCritical = appl.Arguments.Any(a => a.Accept(this));
            if (argsCritical)
                return true;
            if (appl.Procedure is ProcedureConstant pc)
            {
                return pc.Procedure.HasSideEffect;
            }
            else
            {
                return true;
            }
        }

        public bool VisitArrayAccess(ArrayAccess arr) =>
            arr.Array.Accept(this) || arr.Index.Accept(this);

        public bool VisitBinaryExpression(BinaryExpression bin) =>
            bin.Left.Accept(this) || bin.Right.Accept(this);

        public bool VisitCast(Cast cast) =>
            cast.Expression.Accept(this);

        public bool VisitConditionalExpression(ConditionalExpression cond) =>
            cond.Condition.Accept(this) ||
            cond.ThenExp.Accept(this) ||
            cond.FalseExp.Accept(this);

        public bool VisitConditionOf(ConditionOf cond) =>
            cond.Expression.Accept(this);

        public bool VisitConstant(Constant c) => false;

        public bool VisitConversion(Conversion conversion) =>
            conversion.Expression.Accept(this);

        public bool VisitDereference(Dereference deref) => true;
        
        public bool VisitFieldAccess(FieldAccess access) =>
            access.Structure.Accept(this);

        public bool VisitIdentifier(Identifier id) => false;

        public bool VisitMemberPointerSelector(MemberPointerSelector mps) =>
            mps.BasePointer.Accept(this) ||
            mps.MemberPointer.Accept(this);

        public bool VisitMkSequence(MkSequence seq) =>
            seq.Expressions.Any(e => e.Accept(this));

        public bool VisitOutArgument(OutArgument outArgument) => false;

        public bool VisitMemoryAccess(MemoryAccess access) =>
            access.EffectiveAddress.Accept(this);

        public bool VisitPhiFunction(PhiFunction phi) => false;
        
        public bool VisitPointerAddition(PointerAddition pa) =>
            pa.Pointer.Accept(this);

        public bool VisitProcedureConstant(ProcedureConstant pc) => false;

        public bool VisitScopeResolution(ScopeResolution sc) => false;

        public bool VisitSegmentedAddress(SegmentedPointer segptr) =>
            segptr.BasePointer.Accept(this) ||
            segptr.Offset.Accept(this);

        public bool VisitSlice(Slice slice) =>
            slice.Expression.Accept(this);

        public bool VisitStringConstant(StringConstant str) => false;
        
        public bool VisitTestCondition(TestCondition test) =>
            test.Expression.Accept(this);

        public bool VisitUnaryExpression(UnaryExpression unary) =>
            unary.Expression.Accept(this);

        #endregion
    }
}
