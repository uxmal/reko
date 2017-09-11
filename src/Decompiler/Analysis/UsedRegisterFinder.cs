#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        InstructionVisitor<BitRange>,
        ExpressionVisitor<BitRange>
    {
        private IProcessorArchitecture arch;
        private DecompilerEventListener eventListener;
        private ProgramDataFlow flow;
        private Identifier idCur;
        private ProcedureFlow procFlow;
        private SsaState ssa;
        private SsaTransform[] ssts;
        private Dictionary<SsaIdentifier, int> uses;
        private Dictionary<PhiAssignment, BitRange> visited;

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
                this.visited = new Dictionary<PhiAssignment, BitRange>();
                DefInstruction def;
                if (!stm.Instruction.As(out def))
                    continue;
                var sid = ssa.Identifiers[def.Identifier];
                if ((sid.Identifier.Storage is RegisterStorage ||
                     sid.Identifier.Storage is StackArgumentStorage ||
                     sid.Identifier.Storage is FpuStackStorage))
                {
                    var n = Classify(sid);
                    if (!n.IsEmpty)
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
        /// <returns>The bit range used by sid</returns>
        private BitRange Classify(SsaIdentifier sid)
        {
            idCur = sid.Identifier;
            return sid.Uses
                .Where(u => u.Block != ssa.Procedure.ExitBlock)
                .Aggregate(BitRange.Empty, (w, stm) => w | stm.Instruction.Accept(this));
        }

        public BitRange VisitAssignment(Assignment ass)
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

        public BitRange VisitBranch(Branch branch)
        {
            return branch.Condition.Accept(this);
        }

        public BitRange VisitCallInstruction(CallInstruction ci)
        {
            return ci.Uses
                .Aggregate(
                    BitRange.Empty,
                    (br, cb) => cb.Expression.Accept(this));
        }

        public BitRange VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitPhiAssignment(PhiAssignment phi)
        {
            // One of the phi arguments was used, but that's a trivial copy. 
            // Classify the dst of the phi statement, but avoid cycles
            // by memoizing the value we obtained.
            BitRange value;
            if (!visited.TryGetValue(phi, out value))
            {
                visited[phi] = BitRange.Empty;
                value = Classify(ssa.Identifiers[phi.Dst]);
                visited[phi] = value;
            }
            return value;
        }

        public BitRange VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitSideEffect(SideEffect side)
        {
            return side.Expression.Accept(this);
        }

        public BitRange VisitStore(Store store)
        {
            return store.Dst.Accept(this) | store.Src.Accept(this);
        }

        public BitRange VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitUseInstruction(UseInstruction use)
        {
            if (IgnoreUseInstructions)
                return BitRange.Empty;
            return use.Expression.Accept(this);
        }

        public BitRange VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitApplication(Application appl)
        {
            return appl.Arguments
                .Aggregate(
                    BitRange.Empty,
                    (br, e) => br | e.Accept(this));
        }

        public BitRange VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitBinaryExpression(BinaryExpression binExp)
        {
            return binExp.Left.Accept(this) | binExp.Right.Accept(this);
        }

        public BitRange VisitCast(Cast cast)
        {
            var n = cast.Expression.Accept(this);
            return new BitRange(n.Lsb, Math.Min(n.Msb, cast.DataType.BitSize));
        }

        public BitRange VisitConditionalExpression(ConditionalExpression c)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitConditionOf(ConditionOf cof)
        {
            return cof.Expression.Accept(this);
        }

        public BitRange VisitConstant(Constant c)
        {
            return BitRange.Empty;
        }

        public BitRange VisitDepositBits(DepositBits d)
        {
            // The bits being inserted into, d.Source, are "inert" 
            var br = d.InsertedBits.Accept(this);
            return new BitRange(br.Lsb + d.BitPosition, br.Msb + d.BitPosition);
        }

        public BitRange VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitIdentifier(Identifier id)
        {
            return new BitRange(0, (int)id.Storage.BitSize);
        }

        public BitRange VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitMemoryAccess(MemoryAccess access)
        {
            return access.EffectiveAddress.Accept(this);
        }

        public BitRange VisitMkSequence(MkSequence seq)
        {
            return seq.Head.Accept(this) | seq.Tail.Accept(this);
        }

        public BitRange VisitOutArgument(OutArgument outArgument)
        {
            if (outArgument.Expression is Identifier)
            {
                return BitRange.Empty;
            }
            else
            {
                return outArgument.Accept(this);
            }
        }

        public BitRange VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitSegmentedAccess(SegmentedAccess access)
        {
            var useBase = access.BasePointer.Accept(this);
            var useEa = access.EffectiveAddress.Accept(this);
            return useBase | useEa;
        }

        public BitRange VisitSlice(Slice slice)
        {
            var use = slice.Expression.Accept(this);
            var useSlice = new BitRange(
                Math.Max(use.Lsb, slice.Offset),
                Math.Min(use.Msb, slice.Offset + slice.DataType.BitSize));
            return useSlice;
        }

        public BitRange VisitTestCondition(TestCondition tc)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitUnaryExpression(UnaryExpression unary)
        {
            return unary.Expression.Accept(this);
        }
    }
}