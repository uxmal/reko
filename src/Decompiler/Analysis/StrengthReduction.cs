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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System.Collections.Generic;

namespace Reko.Analysis
{
    /// <summary>
    /// If an induction variable i is being used in an addition with a
    /// constant <c>(i + c)</c> and it initially is assigned a constant value
    /// <c>i = Q</c>, this class simplifies the variable so that the initial
    /// assigment becomes <c>i = (Q + c)</c> and the induction variable is
    /// simply used as <c>i</c>. This overcomes problems in hand-coded assembly
    /// programs where the programmer has been thinking in offsets rather
    /// than straightforward pointers.
    /// </summary>
    public class StrengthReduction
    {
        private readonly SsaState ssa;
        private readonly LinearInductionVariable liv;
        private readonly LinearInductionVariableContext ctx;
        private readonly List<IncrementedUse> incrUses;

        /// <summary>
        /// Constructs an instance of <see cref="StrengthReduction"/>.
        /// </summary>
        /// <param name="ssa"><see cref="SsaState"/> being transformed.</param>
        /// <param name="liv"><see cref="LinearInductionVariable">Induction variable</see> being analyzed.</param>
        /// <param name="ctx"><see cref="LinearInductionVariableContext"/> used for analysis.</param>
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
            foreach (Statement stm in ssa.Identifiers[ctx.PhiIdentifier!].Uses)
            {
                if (stm == ctx.DeltaStatement)
                    continue;
                if (stm == ctx.TestStatement)
                    continue;
                var iuf = new IncrementedUseFinder(incrUses);
                iuf.Match(ctx.PhiIdentifier!, stm);
            }
        }

        /// <summary>
        /// Incremented uses of the induction variable.
        /// </summary>
        public List<IncrementedUse> IncrementedUses
        {
            get { return incrUses; }
        }

        /// <summary>
        /// Transform any incremented uses of the induction variable.
        /// </summary>
        public void ModifyUses()
        {
            if (incrUses.Count != 1)
                return;
            IncrementedUse use = incrUses[0];
            if (use.Increment is null)
                return;
            if (ModifyInitialAssigment(use))
            {
                use.Expression!.Right = Constant.Create(use.Increment.DataType, 0);
                ModifyTest(ctx, use);
                liv.AddIncrement(use.Increment);
            }
        }

        private static void ModifyTest(LinearInductionVariableContext ctx, IncrementedUse use)
        {
            if (ctx.TestStatement is null)
                return;
            if (ctx.TestStatement.Instruction is Branch branch &&
                branch.Condition is BinaryExpression exp &&
                exp.Right is Constant c)
            {
                exp.Right = Operator.ISub.ApplyConstants(c.DataType, c, use.Increment!);
            }
        }

        private bool ModifyInitialAssigment(IncrementedUse use)
        {
            if (ctx.InitialStatement is null)
                return false;
            if (ctx.InitialStatement.Instruction is not Assignment ass)
                return false;
            if (ass.Src is Constant c)
            {
                ass.Src = Operator.IAdd.ApplyConstants(c.DataType, c, use.Increment!);
            }
            else
            {
                // Change expression d = x to d = x + c.
                ass.Src = new BinaryExpression(
                    Operator.IAdd,
                    ass.Src.DataType,
                    ass.Src,
                    use.Increment!);
            }
            return true;
            
        }

        /// <summary>
        /// Keeps track identifiers and any incremented uses of those identifiers.
        /// </summary>
        public class IncrementedUse
        {
            /// <summary>
            /// Constructs an instance of <see cref="IncrementedUses"/>.
            /// </summary>
            /// <param name="stm">Statement in which the use occurs.</param>
            /// <param name="exp"></param>
            /// <param name="inc"></param>
            public IncrementedUse(Statement stm, BinaryExpression? exp, Constant? inc)
            {
                this.Statement = stm;
                this.Expression = exp;
                this.Increment = inc;
            }

            /// <summary>
            /// Statement where the use takes place.
            /// </summary>
            public Statement Statement { get; }

            /// <summary>
            /// Expression that inrements or decrements the identifier.
            /// </summary>
            public BinaryExpression? Expression { get; }

            /// <summary>
            /// The constant that is added to or subtracted from the identifier,
            /// or 
            /// </summary>
            public Constant? Increment { get; }
        }

        private class IncrementedUseFinder : InstructionVisitorBase
        {
            private readonly List<IncrementedUse> uses;
            private Identifier? id;
            private Statement? stmCur;

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
                if (binExp.Operator.Type == OperatorType.IAdd
                    &&
                    binExp.Left == id
                    &&
                    binExp.Right is Constant c)
                {
                    uses.Add(new IncrementedUse(stmCur!, binExp, c));
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
                    uses.Add(new IncrementedUse(stmCur!, null, null));
                }
            }
        }
    }
}
