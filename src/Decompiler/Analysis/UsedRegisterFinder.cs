#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// This class determines what identifiers are live-in to a
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
        private readonly IReadOnlyProgram program;
        private readonly ProgramDataFlow flow;
        private readonly HashSet<Procedure> scc;
        private readonly Dictionary<PhiAssignment, BitRange> visited;
        private readonly IDecompilerEventListener eventListener;
        private Identifier? idCur;
        private ProcedureFlow? procFlow;
        private SsaState? ssa;
        private bool useLiveness;

        public UsedRegisterFinder(
            IReadOnlyProgram program,
            ProgramDataFlow flow,
            IEnumerable<Procedure> scc,
            IDecompilerEventListener eventListener)
        {
            this.program = program;
            this.flow = flow;
            this.scc = scc.ToHashSet();
            this.eventListener = eventListener;
            this.visited = new Dictionary<PhiAssignment, BitRange>();
        }

        /// <summary>
        /// Compute the live-in and preliminary data types of the procedure 
        /// whose SSA state is in <paramref name="ssaState"/>.
        /// </summary>
        /// <remarks>
        /// Assmumes that any live-in parameters are located in the
        /// entry block of the procedure.</remarks>
        /// <param name="ssaState"></param>
        public ProcedureFlow ComputeLiveIn(SsaState ssaState, bool ignoreUse)
        {
            var proc = ssaState.Procedure;
            this.procFlow = flow[proc];
            this.ssa = ssaState;
            this.useLiveness = ignoreUse;
            var store = new TypeStore();
            var desc = new LocalTypeDescender(program, store, new TypeFactory());
            desc.BuildEquivalenceClasses(ssa);
            foreach (var stm in proc.EntryBlock.Statements)
            {
                if (stm.Instruction is not DefInstruction def)
                    continue;
                var sid = ssa.Identifiers[def.Identifier];
                var stg = sid.Identifier.Storage;
                if (IsProcedureArgumentStorage(stg, proc))
                     //$REVIEW: flag groups could theoretically be live in
                     // although it's uncommon.
                {
                    var n = Classify(ssa, sid, stg, ignoreUse);
                    if (!n.IsEmpty)
                    {
                        procFlow.BitsUsed[stg] = n;
                        procFlow.LiveInDataTypes[stg] = DataTypeOf(sid, desc);
                    }
                }
            }
            return procFlow;
        }


        /// <summary>
        /// Returns true if the provided storage could be an argument.
        /// </summary>
        private static bool IsProcedureArgumentStorage(Storage stg, Procedure proc)
        {
            return stg switch
            {
                RegisterStorage _ => true,
                StackStorage stk => proc.Architecture.IsStackArgumentOffset(stk.StackOffset),
                SequenceStorage _ => true,
                FpuStackStorage _ => true,
                _ => false
            };
        }

        public BitRange Classify(
            SsaState ssa, 
            SsaIdentifier sid,
            Storage storage,
            bool ignoreUseInstructions)
        {
            this.procFlow = flow[ssa.Procedure];
            this.ssa = ssa;
            this.useLiveness = ignoreUseInstructions;
            if (storage is RegisterStorage ||
                storage is StackStorage ||
                storage is FpuStackStorage ||
                storage is FlagGroupStorage ||
                storage is SequenceStorage)
            {
                var n = Classify(sid);
                return n;
            }
            else
            {
                return BitRange.Empty;
            }
        }

        /// <summary>
        /// Find all places where the SSA identifer <paramref name="sid"/> is used,
        /// discounting any uses in the exit block; uses in the exit block 
        /// model calling procedures uses of return values.
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>The bit range used by sid</returns>
        public BitRange Classify(SsaIdentifier sid)
        {
            idCur = sid.Identifier;
            return sid.Uses
                .Aggregate(BitRange.Empty, (w, stm) => w | stm.Instruction.Accept(this));
        }

        private DataType DataTypeOf(
            SsaIdentifier sidParam,
            LocalTypeDescender typeDescender)
        {
            SsaIdentifier SidOf(PhiArgument a)
            {
                return this.ssa!.Identifiers[(Identifier) a.Value];
            }
            var visited = new HashSet<SsaIdentifier>();
            var wl = new WorkList<SsaIdentifier>();
            wl.Add(sidParam);
            while (wl.TryGetWorkItem(out var sid))
            {
                if (visited.Contains(sid))
                    continue;
                visited.Add(sid);
                foreach (var use in sid.Uses)
                {
                    switch (use.Instruction)
                    {
                    case Assignment ass:
                        typeDescender.Visit(ass.Src);
                        break;
                    case Store store:
                        typeDescender.Visit(store.Src);
                        typeDescender.Visit(store.Dst);
                        break;
                    case SideEffect side:
                        typeDescender.Visit(side.Expression);
                        break;
                    case PhiAssignment phi:
                        wl.AddRange(phi.Src.Arguments.Select(SidOf));
                        break;
                    case Branch branch:
                        typeDescender.Visit(branch.Condition);
                        break;
                    case GotoInstruction g:
                        typeDescender.Visit(g.Target);
                        break;
                    case CallInstruction call:
                        typeDescender.Visit(call.Callee);
                        foreach (var u in call.Uses)
                        {
                            typeDescender.Visit(u.Expression);
                        }
                        break;
                    case SwitchInstruction sw:
                        typeDescender.Visit(sw.Expression);
                        break;
                    }
                }
            }
            typeDescender.MergeDataTypes();
            return typeDescender.GetType(sidParam.Identifier);
        }

        public BitRange VisitAssignment(Assignment ass)
        {
            var ssa = this.ssa!;
            switch (ass.Src)
            {
            case Identifier id when id == idCur:
                {
                    // A simple assignment a = b is a copy, and so we must chase
                    // the uses of a.
                    var idOld = idCur;
                    idCur = ass.Dst;
                    var n = Classify(ssa.Identifiers[ass.Dst]);
                    idCur = idOld;
                    return n;
                }
            case Slice slice:
                {
                    // a = SLICE(a', b) is also a copy, so we must chase the uses of a'.
                    var idOld = idCur;
                    var n = Classify(ssa.Identifiers[ass.Dst]);
                    n <<= slice.Offset;
                    idCur = idOld;
                    return n;
                }
            case MkSequence seq:
                {
                    // First verify that we are indeed using the sequence.
                    var idOld = idCur;
                    var n = Classify(ssa.Identifiers[ass.Dst]);
                    idCur = idOld;
                    if (n.IsEmpty)
                        return n;
                    // Now check the elements of the sequence.
                    int offset = seq.DataType.BitSize;
                    var total = new BitRange();
                    foreach (var elem in seq.Expressions)
                    {
                        var bitsElem = elem.DataType.BitSize;
                        offset -= bitsElem;
                        var rangeElem = new BitRange(offset, offset+bitsElem);
                        var intersect = n.Intersect(rangeElem);
                        if (!intersect.IsEmpty)
                        {
                            var r = elem.Accept(this);
                            total |= (r & intersect.Offset(-offset));
                        }
                    }
                    return total;
                }
            }
            return ass.Src.Accept(this);
        }

        public BitRange VisitBranch(Branch branch)
        {
            return branch.Condition.Accept(this);
        }

        public BitRange VisitCallInstruction(CallInstruction ci)
        {
            if (ci.Callee is ProcedureConstant pc &&
                pc.Procedure is Procedure procCallee &&
                scc.Contains(procCallee))
            {
                // calls to self will force "loop" of live
                // variables.
                return BitRange.Empty;
            }
            var brFn = ci.Callee.Accept(this);
            var brArgs = ci.Uses.Aggregate(
                    BitRange.Empty,
                    (br, cb) => br | cb.Expression.Accept(this));
            return brFn | brArgs;
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
            if (!visited.TryGetValue(phi, out BitRange value))
            {
                visited[phi] = BitRange.Empty;
                value = Classify(ssa!.Identifiers[phi.Dst]);
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
            return si.Expression.Accept(this);
        }

        public BitRange VisitUseInstruction(UseInstruction use)
        {
            if (useLiveness)
            {
                var stg = ((Identifier)use.Expression).Storage;
                var bitrange = procFlow!.BitsLiveOut.Aggregate(
                    BitRange.Empty,
                    (br, entry) =>
                    {
                        if (entry.Key.OverlapsWith(stg))
                            return br | entry.Value.Range;
                        else
                            return br;
                    });
                return bitrange & stg.GetBitRange();
            }
            else
            {
                var br = use.Expression.Accept(this);
                return br;
            }
        }

        public BitRange VisitAddress(Address addr)
        {
            return BitRange.Empty;
        }

        public BitRange VisitApplication(Application appl)
        {
            var brFn = appl.Procedure.Accept(this);
            var brArgs = appl.Arguments
                .Aggregate(
                    BitRange.Empty,
                    (br, e) => br | e.Accept(this));
            return brFn | brArgs;
        }

        public BitRange VisitArrayAccess(ArrayAccess acc)
        {
            var arr = acc.Array.Accept(this);
            var idx = acc.Index.Accept(this);
            return arr | idx;
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

        public BitRange VisitComment(CodeComment comment)
        {
            return BitRange.Empty;
        }

        public BitRange VisitConditionalExpression(ConditionalExpression c)
        {
            return c.Condition.Accept(this);
        }

        public BitRange VisitConditionOf(ConditionOf cof)
        {
            return cof.Expression.Accept(this);
        }

        public BitRange VisitConstant(Constant c)
        {
            return BitRange.Empty;
        }

        public BitRange VisitConversion(Conversion conversion)
        {
            return conversion.Expression.Accept(this);
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
            if (id == idCur)
                return new BitRange(0, id.DataType.BitSize);
            else
                return BitRange.Empty;
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
            return seq.Expressions.Aggregate(
                BitRange.Empty, (range, e) => range | e.Accept(this));
        }

        public BitRange VisitOutArgument(OutArgument outArgument)
        {
            if (outArgument.Expression is Identifier)
            {
                return BitRange.Empty;
            }
            else
            {
                return outArgument.Expression.Accept(this);
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
            return BitRange.Empty;
        }

        public BitRange VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public BitRange VisitSegmentedAddress(SegmentedPointer access)
        {
            var useBase = access.BasePointer.Accept(this);
            var useEa = access.Offset.Accept(this);
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

        public BitRange VisitStringConstant(StringConstant str)
        {
            return BitRange.Empty;
        }

        public BitRange VisitTestCondition(TestCondition tc)
        {
            var use = tc.Expression.Accept(this);
            return use;
        }

        public BitRange VisitUnaryExpression(UnaryExpression unary)
        {
            return unary.Expression.Accept(this);
        }
    }
}
