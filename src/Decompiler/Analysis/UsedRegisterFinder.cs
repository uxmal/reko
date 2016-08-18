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
    public class UsedRegisterFinder :
        InstructionVisitor<int>,
        ExpressionVisitor<int>
    {
        private IProcessorArchitecture arch;
        private DecompilerEventListener eventListener;
        private DataFlow2 flow;
        private Identifier idCur;
        private ProcedureFlow2 procFlow;
        private SsaState ssa;
        private SsaTransform2[] ssts;
        private Dictionary<SsaIdentifier, int> uses;


        public UsedRegisterFinder(
            IProcessorArchitecture arch,
            DataFlow2 flow,
            SsaTransform2[] ssts,
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
        public ProcedureFlow2 Compute(SsaState ssaState)
        {
            this.procFlow = flow.ProcedureFlows[ssaState.Procedure];
            this.uses = new Dictionary<SsaIdentifier, int>();
            this.ssa = ssaState;
            foreach (var sid in ssaState.Identifiers)
            {
                if (sid.DefStatement.Block != ssa.Procedure.EntryBlock ||
                    !(sid.Identifier.Storage is RegisterStorage ||
                      sid.Identifier.Storage is StackArgumentStorage))
                    continue;
                int n = Classify(sid);
                procFlow.Used[sid.Identifier.Storage] = n;
            }
            return procFlow;
        }

        private int Classify(SsaIdentifier sid)
        {
            idCur = sid.Identifier;
            return sid.Uses.Aggregate(0, (w, stm) => Math.Max(w, stm.Instruction.Accept(this)));
        }

        public int VisitAssignment(Assignment ass)
        {
            if (ass.Src == idCur) 
            {
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
            throw new NotImplementedException();
        }

        public int VisitCallInstruction(CallInstruction ci)
        {
            //$BUG: the whole premise of this class is buggy. 
            // we need to review how uses are to be discovered.
            return 0;
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
            throw new NotImplementedException();
        }

        public int VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        public int VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            Debug.Print("Using {0}: {1}", id.Storage, id.Storage.BitSize);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}