#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Services;
using Reko.Core.Code;
using Reko.Core.Expressions;
using System.Diagnostics;

namespace Reko.Analysis
{
    /// <summary>
    /// This class determines what identifier are live-in to a
    /// procedure.
    /// </summary>
    /// <remarks>
    /// The idea is to follow the def-use chains from the def statements in
    /// the procedure entry block to the places where the statements are used.
    /// We record the "width" actually used by the function. This allows us
    /// to handle cases where processor registers are 32-bit, but the code 
    /// only uses the bottom 8 bits.
    /// </remarks>
    public class UsedRegisterFinder :
        InstructionVisitor<int>,
        ExpressionVisitor<int>
    {
        private IProcessorArchitecture arch;
        private DecompilerEventListener eventListener;
        private ProgramDataFlow flow;
        private Identifier idCur;
        private ProcedureFlow procFlow;
        private SsaState ssa;
        private SsaTransform[] ssts;
        private Dictionary<SsaIdentifier, int> uses;
        private Dictionary<PhiAssignment, int> visited;

        public UsedRegisterFinder(
            IProcessorArchitecture arch,
            ProgramDataFlow flow,
            SsaTransform[] ssts,
            DecompilerEventListener eventListener)
        {
            this.arch = arch;
            this.flow = flow;
            this.ssts = ssts;
            this.eventListener = eventListener;
        }

        public bool IgnoreUseInstructions { get; set; }

        /// <summary>
        /// Compute the live-in of the procedure whose SSA state is 
        /// in <paramref name="ssaState"/>.
        /// </summary>
        /// <remarks>
        /// Assmumes that any live-in parameters are located in the
        /// entry block of the procedure.</remarks>
        /// <param name="ssaState"></param>
        public ProcedureFlow Compute(SsaState ssaState)
        {
            this.procFlow = flow[ssaState.Procedure];
            this.uses = new Dictionary<SsaIdentifier, int>();
            this.ssa = ssaState;
            foreach (var stm in ssa.Procedure.EntryBlock.Statements)
            {
                this.visited = new Dictionary<PhiAssignment, int>();
                DefInstruction def;
                if (!stm.Instruction.As(out def))
                    continue;
                Identifier id;
                if (!def.Expression.As(out id))
                    continue;
                var sid = ssa.Identifiers[id];
                if ((sid.Identifier.Storage is RegisterStorage ||
                     sid.Identifier.Storage is StackArgumentStorage ||
                     sid.Identifier.Storage is FpuStackStorage))
                {
                    int n = Classify(sid);
                    if (n > 0)
                    {
                        procFlow.BitsUsed[sid.Identifier.Storage] = n;
                    }
                }
            }
            return procFlow;
        }

        /// <summary>
        /// Find all places where the SSA identifer <paramref name="sid"/> is used,
        /// discounting any uses in the exit block; uses in the exit block 
        /// model calling procedures uses of return values.
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        private int Classify(SsaIdentifier sid)
        {
            idCur = sid.Identifier;
            return sid.Uses
                .Where(u => u.Block != ssa.Procedure.ExitBlock)
                .Aggregate(0, (w, stm) => Math.Max(w, stm.Instruction.Accept(this)));
        }

        public int VisitAssignment(Assignment ass)
        {
            if (ass.Src == idCur) 
            {
                // A simple assignment a = b is a copy, and so we must chase
                // the uses of a.
                var idOld = idCur;
                idCur = ass.Dst;
                var n = Classify(ssa.Identifiers[ass.Dst]);
                idCur = idOld;
                return n;
            }
            return ass.Src.Accept(this);
        }

        public int VisitBranch(Branch branch)
        {
            return branch.Condition.Accept(this);
        }

        public int VisitCallInstruction(CallInstruction ci)
        {
            return ci.Uses
                .Max(cb => cb.Expression.Accept(this));
        }

        public int VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public int VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public int VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public int VisitPhiAssignment(PhiAssignment phi)
        {
            // One of the phi arguments was used, but that's a trivial copy. 
            // Classify the dst of the phi statement
            int value;
            if (visited.TryGetValue(phi, out value))
                return value;       // break cycles of phis.
            visited[phi] = 0;
            int n = Classify(ssa.Identifiers[phi.Dst]);
            visited[phi] = n;
            return n;
        }

        public int VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        public int VisitSideEffect(SideEffect side)
        {
            return side.Expression.Accept(this);
        }

        public int VisitStore(Store store)
        {
            return Math.Max(
                store.Dst.Accept(this),
                store.Src.Accept(this));
        }

        public int VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public int VisitUseInstruction(UseInstruction use)
        {
            if (IgnoreUseInstructions)
                return 0;
            return use.Expression.Accept(this);
        }

        public int VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public int VisitApplication(Application appl)
        {
            return 0;
        }

        public int VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public int VisitBinaryExpression(BinaryExpression binExp)
        {
            return Math.Max(
                binExp.Left.Accept(this),
                binExp.Right.Accept(this));
        }

        public int VisitCast(Cast cast)
        {
            int n = cast.Expression.Accept(this);
            return Math.Min(n, cast.DataType.BitSize);
        }

        public int VisitConditionOf(ConditionOf cof)
        {
            return cof.Expression.Accept(this);
        }

        public int VisitConstant(Constant c)
        {
            return 0;
        }

        public int VisitDepositBits(DepositBits d)
        {
            int n = d.Source.Accept(this);
            return Math.Max(n, d.InsertedBits.Accept(this));
        }

        public int VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public int VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public int VisitIdentifier(Identifier id)
        {
            return (int)id.Storage.BitSize;
        }

        public int VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public int VisitMemoryAccess(MemoryAccess access)
        {
            return access.EffectiveAddress.Accept(this);
        }

        public int VisitMkSequence(MkSequence seq)
        {
            return Math.Max(seq.Head.Accept(this), seq.Tail.Accept(this));
        }

        public int VisitOutArgument(OutArgument outArgument)
        {
            throw new NotImplementedException();
        }

        public int VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public int VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public int VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        public int VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public int VisitSegmentedAccess(SegmentedAccess access)
        {
            var useBase = access.BasePointer.Accept(this);
            var useEa = access.EffectiveAddress.Accept(this);
            return Math.Max(useBase, useEa);
        }

        public int VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        public int VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public int VisitUnaryExpression(UnaryExpression unary)
        {
            return unary.Expression.Accept(this);
        }
    }
}