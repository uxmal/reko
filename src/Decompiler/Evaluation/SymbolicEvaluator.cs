#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Evaluation
{
    /// <summary>
    /// Before we have the luxury of SSA, we need to perform some simplifications. 
    /// This class keeps a context of symbolic expressions for the different registers.
    /// </summary>
    public class SymbolicEvaluator : InstructionVisitor<Instruction>
    {
        private ExpressionSimplifier eval;
        private EvaluationContext ctx;
        private Substitutor sub;

        public SymbolicEvaluator(ExpressionSimplifier expSimp, EvaluationContext ctx)
        {
            this.eval = expSimp;
            this.ctx = ctx;
            this.sub = new Substitutor(ctx);
        }

        public ExpressionSimplifier Simplifier { get { return eval; } }

        public void Evaluate(Instruction instr)
        {
            instr.Accept(this);
        }

        #region InstructionVisitor Members

        public Instruction VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(sub).Accept(eval);
            ctx.SetValue(a.Dst, valSrc);
            return a;
        }

        public Instruction VisitBranch(Branch b)
        {
            b.Condition.Accept(eval);
            return b;
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            return ci;
        }

        public Instruction VisitComment(CodeComment comment)
        {
            return comment;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
            {
                var value = decl.Expression.Accept(eval);
                ctx.SetValue(decl.Identifier, value);
            }
            return decl;
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            return def;
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            // Goto instructions always go to a constant label.
            return gotoInstruction;
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotSupportedException("PhiAssignments shouldn't have been generated yet.");
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression.Accept(eval);
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(eval);
            return side;
        }

        public Instruction VisitStore(Store store)
        {
            var valSrc = store.Src.Accept(sub).Accept(eval);
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
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(eval);
            return si;
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            throw new NotSupportedException("Use expressions shouldn't have been generated yet.");
        }

        #endregion
    }
}
