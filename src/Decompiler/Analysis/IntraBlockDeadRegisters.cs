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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Finds all provably dead registers within the basic blocks of the 
    /// program, and removes them. 
    /// </summary>
    /// <remarks>
    /// This is a cheap preprocessing stage that will remove the great majority 
    /// of unused registers, especially condition codes, of disassembled and 
    /// rewritten code. This avoids the unneccesary generation of SSA variables 
    /// for dead code.
    /// </remarks>
    public class IntraBlockDeadRegisters : 
        InstructionVisitor<bool>,
        StorageVisitor<bool, bool>
    {
        private readonly ExpVisitor expVisitor;
        private readonly HashSet<RegisterStorage> deadRegs;
        private uint deadFlags;

        /// <summary>
        /// Removes intra block dead registers in all procedures in 
        /// the given <paramref name="program"/>.
        /// </summary>
        /// <param name="program">Program to transform.</param>
        /// <param name="eventListener"><see cref="IEventListener"/> instance to
        /// report errors to.
        /// </param>
        public static void Apply(Program program, IEventListener eventListener)
        {
            foreach (var block in program.Procedures.Values.SelectMany(p => p.ControlGraph.Blocks))
            {
                if (eventListener.IsCanceled())
                    break;
                var ibdr = new IntraBlockDeadRegisters();
                ibdr.Apply(block);
            }
        }

        /// <summary>
        /// Constructs an instance of <see cref="IntraBlockDeadRegisters"/>.
        /// </summary>
        public IntraBlockDeadRegisters()
        {
            this.expVisitor = new ExpVisitor(this);
            this.deadRegs = [];
        }

        /// <summary>
        /// Applies this transformation to a basic block, removing any
        /// statements with no side effects.
        /// </summary>
        /// <param name="block"><see cref="Block">Basic block</see> to transform.</param>
        public void Apply(Block block)
        {
            var dead = new HashSet<int>();
            for (int i = block.Statements.Count - 1; i >= 0; --i)
            {
                if (block.Statements[i].Instruction.Accept(this))
                {
                    dead.Add(i);
                }
            }
            int iDst = 0;
            for (int iSrc = 0; iSrc < block.Statements.Count; ++iSrc)
            {
                if (!dead.Contains(iSrc))
                {
                    block.Statements[iDst] = block.Statements[iSrc];
                    ++iDst;
                }
            }
            block.Statements.RemoveRange(iDst, block.Statements.Count - iDst);
        }

        private class ExpVisitor : ExpressionVisitor<bool>
        {
            private readonly IntraBlockDeadRegisters outer;

            public ExpVisitor(IntraBlockDeadRegisters outer)
            {
                this.outer = outer;
            }

            public bool VisitIdentifier(Identifier id)
            {
                outer.Use(id);
                return true;
            }

            public bool VisitAddress(Address addr)
            {
                return true;
            }

            public bool VisitApplication(Application appl)
            {
                foreach (var arg in appl.Arguments)
                {
                    arg.Accept(this);
                }
                return false;
            }

            public bool VisitArrayAccess(ArrayAccess acc)
            {
                var dead = acc.Array.Accept(this);
                dead &= acc.Index.Accept(this);
                return dead;
            }

            public bool VisitBinaryExpression(BinaryExpression binExp)
            {
                bool dead = binExp.Left.Accept(this);
                dead &= binExp.Right.Accept(this);
                return dead;
            }

            public bool VisitCast(Cast cast)
            {
                return cast.Expression.Accept(this);
            }

            public bool VisitConditionalExpression(ConditionalExpression cond)
            {
                var dead = cond.Condition.Accept(this);
                dead &= cond.ThenExp.Accept(this);
                dead &= cond.FalseExp.Accept(this);
                return dead;
            }

            public bool VisitConditionOf(ConditionOf cof)
            {
                return cof.Expression.Accept(this);
            }

            public bool VisitConstant(Constant c)
            {
                return true;
            }

            public bool VisitConversion(Conversion conversion)
            {
                return conversion.Expression.Accept(this);
            }

            public bool VisitDereference(Dereference deref)
            {
                throw new NotImplementedException();
            }

            public bool VisitFieldAccess(FieldAccess acc)
            {
                throw new NotImplementedException();
            }

            public bool VisitMemberPointerSelector(MemberPointerSelector mps)
            {
                throw new NotImplementedException();
            }

            public bool VisitMemoryAccess(MemoryAccess access)
            {
                access.EffectiveAddress.Accept(this);
                return false;
            }

            public bool VisitMkSequence(MkSequence seq)
            {
                var dead = seq.Expressions.All(e => e.Accept(this));
                return dead;
            }

            public bool VisitOutArgument(OutArgument outArg)
            {
                return false;
            }

            public bool VisitPhiFunction(PhiFunction phi)
            {
                throw new NotImplementedException();
            }

            public bool VisitPointerAddition(PointerAddition pa)
            {
                throw new NotImplementedException();
            }

            public bool VisitProcedureConstant(ProcedureConstant pc)
            {
                return false;
            }

            public bool VisitScopeResolution(ScopeResolution scopeResolution)
            {
                throw new NotImplementedException();
            }

            public bool VisitSegmentedAddress(SegmentedPointer address)
            {
                address.BasePointer.Accept(this);
                address.Offset.Accept(this);
                return false;
            }

            public bool VisitSlice(Slice slice)
            {
                return slice.Expression.Accept(this);
            }

            public bool VisitStringConstant(StringConstant str)
            {
                return true;
            }

            public bool VisitTestCondition(TestCondition tc)
            {
                return tc.Expression.Accept(this);
            }

            public bool VisitUnaryExpression(UnaryExpression unary)
            {
                return unary.Expression.Accept(this);
            }
        }

        private void Define(Identifier id)
        {
            id.Storage.Accept(this, true);
        }

        private void Use(Identifier id)
        {
            id.Storage.Accept(this, false);
        }

        private bool IsDead(Identifier id)
        {
            return id.Storage switch
            {
                RegisterStorage reg => deadRegs.Contains(reg),
                FlagGroupStorage flags => (flags.FlagGroupBits & deadFlags) == flags.FlagGroupBits,
                FpuStackStorage => false,
                StackStorage => false,
                SequenceStorage => false,
                MemoryStorage => false,
                TemporaryStorage => false,
                _ => throw new NotImplementedException(id.Storage.GetType().Name)
            };
        }

        /// <inheritdoc/>
        public bool VisitAssignment(Assignment ass)
        {
            bool isDead = IsDead(ass.Dst);
            Define(ass.Dst);
            isDead &= ass.Src.Accept(expVisitor);
            return isDead;
        }

        /// <inheritdoc/>
        public bool VisitBranch(Branch branch)
        {
            branch.Condition.Accept(expVisitor);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitCallInstruction(CallInstruction ci)
        {
            ci.Callee.Accept(expVisitor);
            deadRegs.Clear();
            deadFlags = 0;
            return false;
        }

        /// <inheritdoc/>
        public bool VisitComment(CodeComment comment)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            gotoInstruction.Target.Accept(expVisitor);
            return true;
        }

        /// <inheritdoc/>
        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression is not null)
                ret.Expression.Accept(expVisitor);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(expVisitor);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitStore(Store store)
        {
            store.Dst.Accept(expVisitor);
            store.Src.Accept(expVisitor);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(expVisitor);
            return false;
        }

        /// <inheritdoc/>
        public bool VisitUseInstruction(UseInstruction use)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitFlagGroupStorage(FlagGroupStorage grf, bool defining)
        {
            if (defining)
            {
                deadFlags |= grf.FlagGroupBits;
            }
            else
            {
                deadFlags &= ~grf.FlagGroupBits;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool VisitFpuStackStorage(FpuStackStorage fpu, bool defining)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool VisitMemoryStorage(MemoryStorage global, bool defining)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool VisitRegisterStorage(RegisterStorage reg, bool defining)
        {
            if (defining)
            {
                deadRegs.Add(reg);
            }
            else
            {
                deadRegs.RemoveWhere(r => r.OverlapsWith(reg));
            }
            return true;
        }

        /// <inheritdoc/>
        public bool VisitSequenceStorage(SequenceStorage seq, bool defining)
        {
            foreach (var e in seq.Elements)
            {
                e.Accept(this, defining);
            }
            return true;
        }

        /// <inheritdoc/>
        public bool VisitStackStorage(StackStorage stack, bool defining)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool VisitTemporaryStorage(TemporaryStorage temp, bool defining)
        {
            return true;
        }
    }
}
