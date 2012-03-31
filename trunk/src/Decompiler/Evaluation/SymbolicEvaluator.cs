#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Evaluation
{
    /// <summary>
    /// Before we have the luxury of SSA, we need to perform some simplifications. 
    /// This class keeps a context of symbolic expressions for the different registers.
    /// </summary>
    public class SymbolicEvaluator : InstructionVisitor
    {
        private ExpressionSimplifier eval;
        private EvaluationContext ctx;

        public SymbolicEvaluator(ExpressionSimplifier expSimp, EvaluationContext ctx)
        {
            this.eval = expSimp;
            this.ctx = ctx;
        }

        public SymbolicEvaluator(EvaluationContext ctx) : this(new ExpressionSimplifier(ctx), ctx)
        {
        }

        public ExpressionSimplifier Simplifier { get { return eval; } }

        public void Evaluate(Instruction instr)
        {
            instr.Accept(this);
        }

        //public bool IsTrashed(Storage storage)
        //{
        //    RegisterStorage reg = storage as RegisterStorage;
        //    if (reg != null)
        //    {
        //        Expression exp;
        //        if (RegisterState.TryGetValue(reg, out exp))
        //        {
        //            return exp == Constant.Invalid;
        //        }
        //    }
        //    throw new NotImplementedException();
        //}


        #region InstructionVisitor Members

        public virtual void VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(eval);
            ctx.SetValue(a.Dst, valSrc);
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            b.Condition.Accept(eval);
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
            {
                var value = decl.Expression.Accept(eval);
                ctx.SetValue(decl.Identifier, value);
            }
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new NotSupportedException("Def instructions shouldn't have been generated yet.");
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            // Goto instructions always go to a constant label.
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotSupportedException("PhiAssignments shouldn't have been generated yet.");
        }

        public void VisitIndirectCall(IndirectCall ic)
        {
            ic.Callee.Accept(eval);
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression.Accept(eval);
        }

        public void VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(eval);
        }

        public void VisitStore(Store store)
        {
            var valSrc = store.Src.Accept(eval);
            var segmem = store.Dst as SegmentedAccess;
            if (segmem != null)
            {
                var basePtr = segmem.BasePointer.Accept(eval);
                var ea = segmem.EffectiveAddress.Accept(eval);
                ctx.SetValueEa(basePtr, ea, valSrc);
            }
            var access = store.Dst as MemoryAccess;
            if (access != null)
            {
                var ea = access.EffectiveAddress.Accept(eval);
                ctx.SetValueEa(ea, valSrc);
            }
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotSupportedException("Use expressions shouldn't have been generated yet.");
        }

        #endregion
    }

}
