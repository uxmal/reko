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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// If an induction variable i is being used in an addition with a
    /// constant (i + c) and it initially is assigned a constant value
    /// i = Q, this class simplifies the variable so that the initial
    /// assigment becomes i = (Q + c) and the induction variable is
    /// simply used as i. This overcomes problems in hand-coded assembly
    /// programs where the programmer has been thinking in offsets rather
    /// than straightforward pointers.
    /// </summary>
    public class StrengthReduction
    {
        private SsaState ssa;
        private LinearInductionVariable liv;
        private LinearInductionVariableContext ctx;
        private List<IncrementedUse> incrUses;

        public StrengthReduction(SsaState ssa, LinearInductionVariable liv, LinearInductionVariableContext ctx)
        {
            this.ssa = ssa;
            this.liv = liv;
            this.ctx = ctx;
            incrUses = new List<IncrementedUse>();
        }

        /// <summary>
        /// Find all places where phi identifiers are used except the statements
        /// that test the phi variable and increment the variable
        /// </summary>
        public void ClassifyUses()
        {
            foreach (Statement stm in ssa.Identifiers[ctx.PhiIdentifier].Uses)
            {
                if (stm == ctx.DeltaStatement)
                    continue;
                if (stm == ctx.TestStatement)
                    continue;
                IncrementedUseFinder iuf = new IncrementedUseFinder(incrUses);
                iuf.Match(ctx.PhiIdentifier, stm);
            }
        }


        public List<IncrementedUse> IncrementedUses
        {
            get { return incrUses; }
        }

        public void ModifyUses()
        {
            if (incrUses.Count != 1)
                return;
            IncrementedUse use = incrUses[0];
            if (use.Increment == null)
                return;
            if (ModifyInitialAssigment(use))
            {
                use.Expression.Right = Constant.Create(use.Increment.DataType, 0);
                ModifyTest(ctx, use);
                liv.AddIncrement(use.Increment);
            }
        }

        private void ModifyTest(LinearInductionVariableContext ctx, IncrementedUse use)
        {
            if (ctx.TestStatement == null)
                return;
            if (ctx.TestStatement.Instruction is Branch branch &&
                branch.Condition is BinaryExpression exp &&
                exp.Right is Constant c)
            {
                exp.Right = Operator.ISub.ApplyConstants(c, use.Increment);
            }
        }

        private bool ModifyInitialAssigment(IncrementedUse use)
        {
            if (ctx.InitialStatement == null)
                return false;
            if (!(ctx.InitialStatement.Instruction is Assignment ass))
                return false;
            if (ass.Src is Constant c)
            {
                ass.Src = Operator.IAdd.ApplyConstants(c, use.Increment);
            }
            else
            {
                // Change expression d = x to d = x + c.
                ass.Src = new BinaryExpression(
                    Operator.IAdd,
                    ass.Src.DataType,
                    ass.Src,
                    use.Increment);
            }
            return true;
            
        }

        public class IncrementedUse
        {
            public IncrementedUse(Statement stm, BinaryExpression exp, Constant inc)
            {
                this.Statement = stm;
                this.Expression = exp;
                this.Increment = inc;
            }

            public Statement Statement { get; }
            public BinaryExpression Expression { get; }
            public Constant Increment { get; }
        }

        public class IncrementedUseFinder : InstructionVisitorBase
        {
            private readonly List<IncrementedUse> uses;
            private Identifier id;
            private Statement stmCur;

            public IncrementedUseFinder(List<IncrementedUse> uses)
            {
                this.uses = uses;
            }

            public void Match(Identifier id, Statement stm)
            {
                this.stmCur = stm;
                this.id = id;
                stm.Instruction.Accept(this);
            }

            public override void VisitBinaryExpression(BinaryExpression binExp)
            {
                if (binExp.Operator == Operator.IAdd
                    &&
                    binExp.Left == id
                    &&
                    binExp.Right is Constant c)
                {
                    uses.Add(new IncrementedUse(stmCur, binExp, c));
                }
                else
                {
                    base.VisitBinaryExpression(binExp);
                }
            }

            public override void VisitIdentifier(Identifier id)
            {
                if (this.id == id)
                {
                    uses.Add(new IncrementedUse(stmCur, null, null));
                }
            }
        }
    }
}
