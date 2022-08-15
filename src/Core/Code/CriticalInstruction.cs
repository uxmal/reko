#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

		public static bool IsCritical(Instruction instr)
		{
            return instr.Accept(instance);
		}

		public static bool IsCritical(Expression expr)
		{
            return expr.Accept(instance);
		}

		#region InstructionVisitor //////////////////////////////////

        public bool VisitAssignment(Assignment ass)
        {
            return ass.Src.Accept(this);
        }
            
		public bool VisitBranch(Branch b)
		{
			return true;
		}

		public bool VisitCallInstruction(CallInstruction ci)
		{
			return true;
		}

        public bool VisitComment(CodeComment comment)
        {
            return true;
        }

        public bool VisitDeclaration(Declaration decl)
        {
            return decl.Expression != null
                ? decl.Expression.Accept(this)
                : false;
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            return false;
        }

        public bool VisitGotoInstruction(GotoInstruction g)
        {
            return true;
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            return false;
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
		{
			return true;
		}

		public bool VisitSideEffect(SideEffect side)
		{
			return true;
		}

		public bool VisitStore(Store store)
		{
			return true;
		}

		public bool VisitSwitchInstruction(SwitchInstruction si)
		{
			return true;
		}

		public bool VisitUseInstruction(UseInstruction u)
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

        public bool VisitSegmentedAccess(SegmentedAccess segmem) =>
            segmem.BasePointer.Accept(this) ||
            segmem.EffectiveAddress.Accept(this);

        public bool VisitSlice(Slice slice) =>
            slice.Expression.Accept(this);

        public bool VisitTestCondition(TestCondition test) =>
            test.Expression.Accept(this);

        public bool VisitUnaryExpression(UnaryExpression unary) =>
            unary.Expression.Accept(this);

		#endregion
	}
}
