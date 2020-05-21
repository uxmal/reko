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
using Reko.Core.Dfa;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// This transformation attempts to reduce the number of 
    /// <see cref="Slice"/> expressions in a procedure by locating all
    /// occurrences of slices and "pushing" them towards the roots
    /// of expressions.
    /// </summary>
    /// <remarks>
    /// The transformation consists of an analysis step followed by the 
    /// actual transformation. The analysis starts by collecting all the 
    /// available identifier slices, which is trivial. It then does a second
    /// pass, discovering what slices of identifiers are actually needed.
    /// This may require traversing "backwards" past copy statements (a = b)
    /// or phi functions (a_3 = phi(a_1, a_2).
    /// 
    /// The transformation then determines which variables are too wide, and
    /// injects slice expressions where appropriate to narrow them down.
    /// 
    /// </remarks>
    public class SlicePropagator
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(SlicePropagator), "Trace progress of SlicePropagator") { Level = TraceLevel.Verbose }; private readonly SsaState ssa;

        private readonly DecompilerEventListener listener;
        private readonly Dictionary<SsaIdentifier, HashSet<BitRange>> neededSlices;  // Sliced identifiers we want, but don't have
        private readonly Dictionary<SsaIdentifier, Dictionary<BitRange, SsaIdentifier>> availableSlices; // Sliced identifiers we do have.
        private readonly Dictionary<SsaIdentifier, SsaIdentifier> replaceIds;   // Identifiers that can be replaced.

        public SlicePropagator(SsaState ssa, DecompilerEventListener listener)
        {
            this.ssa = ssa;
            this.listener = listener;
            this.neededSlices = new Dictionary<SsaIdentifier, HashSet<BitRange>>();
            this.availableSlices = new Dictionary<SsaIdentifier, Dictionary<BitRange, SsaIdentifier>>();
            this.replaceIds = new Dictionary<SsaIdentifier, SsaIdentifier>();
            foreach (var sid in ssa.Identifiers)
            {
                if (sid.Uses.Count == 0 || sid.Identifier.Storage is FlagGroupStorage)
                    continue;
                var available = new Dictionary<BitRange, SsaIdentifier> { { Ctx(sid.Identifier).Bitrange, sid } };
                this.availableSlices.Add(sid, available);

                this.neededSlices.Add(sid, new HashSet<BitRange>());
            }
        }

        public void Transform()
        {
            DetermineNeededSlices();
            GenerateNeededSlices();
            ReplaceSlices();
            ssa.Validate(e => { ssa.Procedure.Dump(true); });
        }

        private void DetermineNeededSlices()
        {
            static bool IsCopy(Instruction instr)
            {
                if (instr is PhiAssignment)
                    return true;
                if (instr is Assignment ass)
                {
                    return (ass.Src is Identifier);
                }
                return false;
            }

            var wl = new WorkList<(SsaIdentifier, NarrowContext)>(ssa.Identifiers
                .Where(s => 
                    s.DefStatement != null &&
                    !(s.Identifier.Storage is FlagGroupStorage) &&
                    !IsCopy(s.DefStatement.Instruction))
                .Select(s => (s, Ctx(s.Identifier))));
            var sliceFinder = new SliceFinder(this, wl);
            while (wl.GetWorkItem(out var item))
            {
                sliceFinder.stm = item.Item1.DefStatement!;
                item.Item1.DefStatement!.Instruction.Accept(sliceFinder, item.Item2);
            }
        }

        private void GenerateNeededSlices()
        {
            foreach (var needed in this.neededSlices)
            {
                var sidOld = needed.Key;
                if (needed.Value.Count != 1)
                    continue;
                var brNeeded = needed.Value.First();
                var ctxOriginal = Ctx(sidOld.Identifier);
                if (brNeeded.Covers(ctxOriginal.Bitrange))
                    continue;
                if (sidOld.DefStatement?.Instruction is CallInstruction)
                    continue;
                // At this point, we've discovered that only one slice size 
                // of the sidOld is ever used, and it is always smaller than 
                // the original size of needed.Key. We can create a smaller 
                // identifier, and replace all uses of the original wide variable
                // with the new narrower identifier.
                trace.Verbose("SLP: Id {0} is only used with bitrange {1}", sidOld.Identifier, brNeeded);
                // We need a smaller slice. Does it already exist?
                if (this.availableSlices[sidOld].TryGetValue(brNeeded, out var sidNew))
                {
                    // Delete the existing alias statement, and rely on SlicePusher to 
                    // replace it.
                    sidNew.DefStatement!.Block.Statements.Remove(sidNew.DefStatement);
                }
                else
                {
                    // An appropriate variable doesn't exist, so we need to make a new slicing alias.
                    sidNew = MakeSlicedIdentifier(sidOld, brNeeded);
                    this.availableSlices[sidOld].Add(brNeeded, sidNew);
                    this.availableSlices.Add(sidNew, new Dictionary<BitRange, SsaIdentifier> { { brNeeded, sidNew } });
                }

                // Now remove the uses of the old identifier. New uses will be added
                // by the SlicePusher.
                sidOld.Uses.Clear();
                this.replaceIds.Add(sidOld, sidNew);
                trace.Verbose("Slp: Narrowing {0} down to {1} ({2})", sidOld.Identifier, sidNew.Identifier, brNeeded);
            }
        }

        private SsaIdentifier MakeSlicedIdentifier(SsaIdentifier sidOriginal, BitRange brNeeded)
        {
            var dt = PrimitiveType.CreateWord(brNeeded.Extent);
            Identifier idSliced;
            switch (sidOriginal.Identifier.Storage)
            {
            case RegisterStorage reg:
                var regSliced = ssa.Procedure.Architecture.GetRegister(reg.Domain, brNeeded);
                idSliced = ssa.Procedure.Frame.EnsureRegister(regSliced!);
                idSliced.DataType = dt;
                break;
            case StackStorage stk:
                var stkSliced = ssa.Procedure.Architecture.Endianness.SliceStackStorage(stk, brNeeded);
                idSliced = ssa.Procedure.Frame.EnsureStackVariable(stkSliced.StackOffset, dt);
                break;
            case TemporaryStorage _:
                idSliced = ssa.Procedure.Frame.CreateTemporary(dt);
                break;
            default:
                throw new NotImplementedException($"Support for slicing {sidOriginal.Identifier.Storage.GetType().Name} not implemented yet.");
            }
            var sidTo = ssa.Identifiers.Add(idSliced, sidOriginal.DefStatement, new Slice(dt, sidOriginal.Identifier, brNeeded.Lsb), false);
            return sidTo;
        }

        private void ReplaceSlices()
        {
            var slicePusher = new SlicePusher(this, default!);
            foreach (var stm in ssa.Procedure.Statements)
            {
                if (stm.Block.Name == "l00100073")
                    stm.ToString(); //$DEBUG
                slicePusher.Statement = stm;
                var instrNew = stm.Instruction.Accept(slicePusher);
                stm.Instruction = instrNew;
            }
        }

        private static NarrowContext Ctx(Expression e)
        {
            return new NarrowContext(e.DataType, 0);
        }

        private static NarrowContext Ctx(Slice slice)
        {
            return new NarrowContext(slice.DataType, slice.Offset);
        }

        private static PrimitiveType Word(BitRange range)
        {
            return PrimitiveType.CreateWord(range.Extent);
        }

        /// <summary>
        /// This visitor class discovers <see cref="Slice" />s, <see cref="Convert"/>s and 
        /// <see cref="Cast"/>s in the program and propagates these 
        /// to the leaves of the <see cref="Expression"/>s in the <see cref="Procedure"/>. 
        /// If an <see cref="Identifier"/> is encountered it records the size of the slice
        /// at that point.
        /// </summary>
        public class SliceFinder : InstructionVisitor<Instruction, NarrowContext>, ExpressionVisitor<Expression, NarrowContext>
        {
            private readonly SlicePropagator outer;
            private readonly WorkList<(SsaIdentifier, NarrowContext)> wl;
            internal Statement? stm;

            public SliceFinder(SlicePropagator outer, WorkList<(SsaIdentifier, NarrowContext)> wl)
            {
                this.outer = outer;
                this.wl = wl;
            }

            public Instruction VisitAssignment(Assignment ass, NarrowContext ctx)
            {
                ass.Src.Accept(this, ctx);
                if (ass.Src is Slice slice && slice.Expression is Identifier slicedId)
                {
                    var sidSlice = outer.ssa.Identifiers[ass.Dst];
                    var sidOriginal = outer.ssa.Identifiers[slicedId];
                    var br = Ctx(slice);
                    if (!outer.availableSlices[sidOriginal].ContainsKey(br.Bitrange))
                    {
                        outer.availableSlices[sidOriginal].Add(br.Bitrange, sidSlice);
                    }
                }
                return ass;
            }

            public Instruction VisitBranch(Branch branch, NarrowContext ctx)
            {
                branch.Condition.Accept(this, Ctx(branch.Condition));
                return branch;
            }

            public Instruction VisitCallInstruction(CallInstruction ci, NarrowContext ctx)
            {
                foreach (var use in ci.Uses)
                {
                    use.Expression.Accept(this, Ctx(use.Expression));
                }
                return ci;
            }

            public Instruction VisitComment(CodeComment comment, NarrowContext ctx)
            {
                return comment;
            }

            public Instruction VisitDeclaration(Declaration decl, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitDefInstruction(DefInstruction def, NarrowContext ctx)
            {
                return def;
            }

            public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction, NarrowContext ctx)
            {
                gotoInstruction.Target.Accept(this, Ctx(gotoInstruction.Target));
                throw new NotImplementedException();
            }

            public Instruction VisitPhiAssignment(PhiAssignment phi, NarrowContext ctx)
            {
                foreach (var arg in phi.Src.Arguments)
                {
                    arg.Value.Accept(this, ctx);
                }
                return phi;
            }

            public Instruction VisitReturnInstruction(ReturnInstruction ret, NarrowContext ctx)
            {
                if (ret.Expression != null)
                {
                    ret.Expression.Accept(this, Ctx(ret.Expression));
                }
                return ret;
            }

            public Instruction VisitSideEffect(SideEffect side, NarrowContext ctx)
            {
                side.Expression.Accept(this, Ctx(side.Expression));
                return side;
            }

            public Instruction VisitStore(Store store, NarrowContext ctx)
            {
                var br = Ctx(store.Dst);
                store.Src.Accept(this, br);
                store.Dst.Accept(this, Ctx(store.Dst));
                return store;
            }

            public Instruction VisitSwitchInstruction(SwitchInstruction si, NarrowContext ctx)
            {
                si.Expression.Accept(this, Ctx(si.Expression));
                return si;
            }

            public Instruction VisitUseInstruction(UseInstruction use, NarrowContext ctx)
            {
                use.Expression.Accept(this, Ctx(use.Expression));
                return use;
            }

            public Expression VisitAddress(Address addr, NarrowContext ctx)
            {
                return addr;
            }

            public Expression VisitApplication(Application appl, NarrowContext ctx)
            {
                appl.Procedure.Accept(this, Ctx(appl.Procedure));
                foreach (var arg in appl.Arguments)
                {
                    arg.Accept(this, Ctx(arg));
                }
                return appl;
            }

            public Expression VisitArrayAccess(ArrayAccess acc, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitBinaryExpression(BinaryExpression binExp, NarrowContext ctx)
            {
                switch (binExp.Operator)
                {
                case IAddOperator _:
                case ISubOperator _:
                case USubOperator _:
                case AndOperator _:
                case OrOperator _:
                case XorOperator _:
                    if (ctx.Offset == 0)
                    {
                        binExp.Left.Accept(this, ctx);
                        binExp.Right.Accept(this, ctx);
                    }
                    else
                    {
                        binExp.Left.Accept(this, Ctx(binExp.Left));
                        binExp.Right.Accept(this, Ctx(binExp.Right));
                    }
                    return binExp;
                case ShlOperator _:
                case ShrOperator _:
                case SarOperator _:
                    binExp.Left.Accept(this, ctx);
                    binExp.Right.Accept(this, Ctx(binExp.Right));
                    return binExp;
                case IMulOperator _:
                case SDivOperator _:
                case UDivOperator _:
                case FAddOperator _:
                case FSubOperator _:
                case FMulOperator _:
                case FDivOperator _:
                case IModOperator _:
                case ConditionalOperator _:
                case CandOperator _:
                case CorOperator _:
                    binExp.Left.Accept(this, Ctx(binExp.Left));
                    binExp.Right.Accept(this, Ctx(binExp.Right));
                    return binExp;
                }
                throw new NotImplementedException($"SliceFinder not implemented for {binExp.Operator.GetType().Name}.");
            }

            public Expression VisitCast(Cast cast, NarrowContext ctx)
            {
                cast.Expression.Accept(this, Ctx(cast.Expression));
                return cast;
            }

            public Expression VisitConditionalExpression(ConditionalExpression c, NarrowContext ctx)
            {
                c.Condition.Accept(this, Ctx(c.Condition));
                c.ThenExp.Accept(this, ctx);
                c.FalseExp.Accept(this, ctx);
                return c;
            }

            public Expression VisitConditionOf(ConditionOf cof, NarrowContext ctx)
            {
                cof.Expression.Accept(this, Ctx(cof.Expression));
                return cof;
            }

            public Expression VisitConversion(Conversion cnv, NarrowContext ctx)
            {
                cnv.Expression.Accept(this, Ctx(cnv.Expression));
                return cnv;
            }

            public Expression VisitConstant(Constant c, NarrowContext ctx)
            {
                return c;
            }

            public Expression VisitDereference(Dereference deref, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitFieldAccess(FieldAccess acc, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitIdentifier(Identifier id, NarrowContext ctx)
            {
                var sid = outer.ssa.Identifiers[id];
                var added = outer.neededSlices[sid].Add(ctx.Bitrange);
                if (!ctx.Bitrange.Covers(Ctx(id).Bitrange))
                {
                    trace.Verbose("SLP: found a narrowed use of {0} ({1}) in {2}", id, ctx, stm!);
                    if (added)
                    {
                        trace.Verbose("SLP: Id {0} needs bitrange {1}", sid.Identifier.Name, ctx);
                        wl.Add((sid, ctx));
                    }
                }
                return id;
            }

            public Expression VisitMemberPointerSelector(MemberPointerSelector mps, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMemoryAccess(MemoryAccess access, NarrowContext ctx)
            {
                access.EffectiveAddress.Accept(this, Ctx(access.EffectiveAddress));
                return access;
            }

            public Expression VisitMkSequence(MkSequence seq, NarrowContext ctx)
            {
                int bitPos = 0;
                for (int i = seq.Expressions.Length - 1; i >= 0; --i)
                {
                    var elem = seq.Expressions[i];
                    var br = Ctx(elem);
                    var brElem = ctx.Bitrange.Offset(-bitPos);
                    var btIntersect = brElem & br.Bitrange;
                    if (!btIntersect.IsEmpty)
                    {
                        var ctxElem = br.Narrow(btIntersect);
                        elem.Accept(this, ctxElem);
                    }
                    bitPos += br.Bitrange.Extent;
                }
                return seq;
            }

            public Expression VisitOutArgument(OutArgument outArgument, NarrowContext ctx)
            {
                return outArgument;
            }

            public Expression VisitPhiFunction(PhiFunction phi, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPointerAddition(PointerAddition pa, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitProcedureConstant(ProcedureConstant pc, NarrowContext ctx)
            {
                return pc;
            }

            public Expression VisitScopeResolution(ScopeResolution scopeResolution, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSegmentedAccess(SegmentedAccess access, NarrowContext ctx)
            {
                access.BasePointer.Accept(this, Ctx(access.BasePointer));
                access.EffectiveAddress.Accept(this, Ctx(access.EffectiveAddress));
                return access;
            }

            public Expression VisitSlice(Slice slice, NarrowContext ctx)
            {
                var brSlice = Ctx(slice);
                slice.Expression.Accept(this, brSlice);
                return slice;
            }

            public Expression VisitTestCondition(TestCondition tc, NarrowContext ctx)
            {
                tc.Expression.Accept(this, Ctx(tc.Expression));
                return tc;
            }

            public Expression VisitUnaryExpression(UnaryExpression unary, NarrowContext ctx)
            {
                switch (unary.Operator)
                {
                case NegateOperator _:
                case FNegOperator _:
                case ComplementOperator _:
                    unary.Expression.Accept(this, ctx);
                    return unary;
                case NotOperator _:
                case AddressOfOperator _:
                    unary.Expression.Accept(this, Ctx(unary.Expression));
                    return unary;
                }
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This instruction visitor will 'push' slice expressions towards the leaves of an expression. Slicing an
        /// identifier may result in the use of an existing slice alias -- a code improvement. Likewise, slicing
        /// a Constant will also result in a code improvement.
        /// </summary>
        public class SlicePusher : InstructionVisitor<Instruction>, ExpressionVisitor<Expression, NarrowContext>
        {
            private readonly SlicePropagator outer;

            public SlicePusher(SlicePropagator outer, Statement stm)
            {
                this.outer = outer;
                this.Statement = stm;
            }

            public Statement Statement { get; set; }

            public Instruction VisitAssignment(Assignment ass)
            {
                static Assignment MkAlias(Identifier id, Expression e) => new AliasAssignment(id, e);
                static Assignment MkAssign(Identifier id, Expression e) => new Assignment(id, e);
                Func<Identifier, Expression, Assignment> mk = ass is AliasAssignment
                    ? new Func<Identifier, Expression, Assignment>(MkAlias)
                    : MkAssign;

                var sidDst = outer.ssa.Identifiers[ass.Dst];
                if (outer.replaceIds.TryGetValue(sidDst, out var sidDstNew))
                {
                    var src = ass.Src.Accept(this, Ctx(sidDstNew.Identifier));
                    sidDstNew.DefStatement = this.Statement;
                    sidDstNew.DefExpression = src;
                    sidDst.DefStatement = null;
                    sidDst.DefExpression = null;
                    return mk(sidDstNew.Identifier, src);
                }
                else
                {
                    var src = ass.Src.Accept(this, Ctx(ass.Dst));
                    sidDst.DefExpression = src;
                    return mk(ass.Dst, src);
                }
            }

            public Instruction VisitBranch(Branch branch)
            {
                var cond = branch.Condition.Accept(this, Ctx(branch.Condition));
                return new Branch(cond, branch.Target);
            }

            public Instruction VisitCallInstruction(CallInstruction ci)
            {
                var callee = ci.Callee.Accept(this, Ctx(ci.Callee));
                var uses = new List<CallBinding>();
                foreach (var use in ci.Uses) 
                {
                    var e = use.Expression.Accept(this, Ctx(use.Expression));
                    uses.Add(new CallBinding(use.Storage, e) { BitRange = use.BitRange });
                }
                var newCall = new CallInstruction(callee, ci.CallSite);
                newCall.Uses.UnionWith(uses);
                newCall.Definitions.UnionWith(ci.Definitions);
                return newCall;
            }

            public Instruction VisitComment(CodeComment comment)
            {
                return comment;
            }

            public Instruction VisitDeclaration(Declaration decl)
            {
                return decl;
            }

            public Instruction VisitDefInstruction(DefInstruction def)
            {
                //$REVIEW: what about user-defined parameters?
                var sidDst = outer.ssa.Identifiers[def.Identifier];
                if (outer.replaceIds.TryGetValue(sidDst, out var sidDstNew))
                {
                    trace.Verbose("SLP: Replacing def {0} with {1}", sidDst.Identifier, sidDstNew.Identifier);
                    sidDstNew.DefStatement = Statement;
                    sidDst.DefStatement = null;
                    sidDst.DefExpression = null;
                    return new DefInstruction(sidDstNew.Identifier);
                }
                else
                {
                    return def;
                }
            }

            public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitPhiAssignment(PhiAssignment phi)
            {
                var newArgs = new List<PhiArgument>();
                foreach (var oldArg in phi.Src.Arguments)
                {
                    var sidOld = outer.ssa.Identifiers[(Identifier) oldArg.Value];
                    if (outer.replaceIds.TryGetValue(sidOld, out var sidNew))
                    {
                        sidNew.Uses.Add(Statement);
                        newArgs.Add(new PhiArgument(oldArg.Block, sidNew.Identifier));
                    }
                    else
                    {
                        newArgs.Add(oldArg);
                    }
                }
                var phiFn = new PhiFunction(phi.Src.DataType, newArgs.ToArray());
                var sidDst = outer.ssa.Identifiers[phi.Dst];
                if (outer.replaceIds.TryGetValue(sidDst, out var sidDstNew))
                {

                    sidDstNew.DefStatement = this.Statement;
                    sidDstNew.DefExpression = phiFn;
                    sidDst.DefStatement = null;
                    sidDst.DefExpression = null;
                    trace.Verbose("SLP: Replacing {0} with {1}={2}", phi, sidDstNew.Identifier, phiFn);
                    return new PhiAssignment(sidDstNew.Identifier, phiFn);
                }
                else
                {
                    sidDst.DefExpression = phiFn;
                    trace.Verbose("SLP: Replacing {0} with {1}={2}", phi, phi.Dst, phiFn);
                    return new PhiAssignment(phi.Dst, phiFn);
                }
            }

            public Instruction VisitReturnInstruction(ReturnInstruction ret)
            {
                if (ret.Expression == null)
                    return ret;
                else
                {
                    var e = ret.Expression.Accept(this, Ctx(ret.Expression));
                    return new ReturnInstruction(e);
                }
            }

            public Instruction VisitSideEffect(SideEffect side)
            {
                var e = side.Expression.Accept(this, Ctx(side.Expression));
                return new SideEffect(e);
            }

            public Instruction VisitStore(Store store)
            {
                var br = Ctx(store.Dst);
                var src = store.Src.Accept(this, br);
                var dst = store.Dst.Accept(this, br);
                return new Store(dst, src);
            }

            public Instruction VisitSwitchInstruction(SwitchInstruction si)
            {
                var c = si.Expression.Accept(this, Ctx(si.Expression));
                return new SwitchInstruction(c, si.Targets);
            }

            public Instruction VisitUseInstruction(UseInstruction use)
            {
                use.Expression.Accept(this, Ctx(use.Expression));
                return use;
            }

            public Expression VisitAddress(Address addr, NarrowContext ctx)
            {
                return addr;
            }

            public Expression VisitApplication(Application appl, NarrowContext ctx)
            {
                var callee = appl.Procedure.Accept(this, Ctx(appl.Procedure));
                var args = new List<Expression>();
                foreach (var arg in appl.Arguments)
                {
                    var argNew = arg.Accept(this, Ctx(arg));
                    args.Add(argNew);
                }
                return new Application(callee, appl.DataType, args.ToArray());
            }

            public Expression VisitArrayAccess(ArrayAccess acc, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitBinaryExpression(BinaryExpression binExp, NarrowContext ctx)
            {
                Expression left;
                Expression right = binExp.Right;
                DataType dt = binExp.DataType;
                switch (binExp.Operator)
                {
                case IAddOperator _:
                case ISubOperator _:
                    if (ctx.Offset == 0)
                    {
                        left = binExp.Left.Accept(this, ctx);
                        right = binExp.Right.Accept(this, ctx);
                        dt = left.DataType;
                    }
                    else
                    {
                        left = binExp.Left.Accept(this, Ctx(binExp.Left));
                        right = binExp.Right.Accept(this, Ctx(binExp.Right));
                        return MaybeSlice(new BinaryExpression(binExp.Operator, dt, left, right), ctx);
                    }
                    break;
                case USubOperator _:
                case AndOperator _:
                case OrOperator _:
                case XorOperator _:
                    left = binExp.Left.Accept(this, ctx);
                    right = binExp.Right.Accept(this, ctx);
                    dt = left.DataType;
                    break;
                case ShlOperator _:
                case ShrOperator _:
                case SarOperator _:
                    left = binExp.Left.Accept(this, ctx);
                    dt = left.DataType;
                    break;
                case ConditionalOperator _:
                case IMulOperator _:
                case FAddOperator _:
                case FSubOperator _:
                case FMulOperator _:
                case SDivOperator _:
                case UDivOperator _:
                case FDivOperator _:
                case IModOperator _:
                case CandOperator _:
                case CorOperator _:
                    left = binExp.Left.Accept(this, Ctx(binExp.Left));
                    right = binExp.Right.Accept(this, Ctx(binExp.Right));
                    return MaybeSlice(new BinaryExpression(binExp.Operator, dt, left, right), ctx);
                default:
                    throw new NotImplementedException($"SLP: support for {binExp.Operator.GetType().Name} not implemented yet.");
                }
                return new BinaryExpression(binExp.Operator, dt, left, right);
            }

            public Expression VisitCast(Cast cast, NarrowContext ctx)
            {
                var br = new BitRange(0, cast.DataType.BitSize);
                if (!ctx.Bitrange.Covers(br))
                {
                    var brExp = Ctx(cast.Expression);
                    if (brExp.Bitrange.Covers(ctx.Bitrange))
                    {
                        var e = cast.Expression.Accept(this, ctx);
                        return e;
                    }
                    else
                    {
                        var e = cast.Expression.Accept(this, brExp);
                        return new Cast(Word(ctx.Bitrange), e);
                    }
                }
                else
                {
                    return cast;
                }
            }

            public Expression VisitConditionalExpression(ConditionalExpression c, NarrowContext context)
            {
                var cond = c.Condition.Accept(this, Ctx(c.Condition));
                var t = c.ThenExp.Accept(this, context);
                var f = c.FalseExp.Accept(this, context);
                return new ConditionalExpression(t.DataType, cond, t, f);
            }

            public Expression VisitConditionOf(ConditionOf cof, NarrowContext ctx)
            {
                var c = cof.Expression.Accept(this, Ctx(cof.Expression));
                return new ConditionOf(c);
            }

            public Expression VisitConstant(Constant c, NarrowContext ctx)
            {
                var br = Ctx(c);
                if (!ctx.Bitrange.Covers(br.Bitrange))
                {
                    var bitfield = new Bitfield(ctx.Bitrange.Lsb, ctx.Bitrange.Extent);
                    var dt = PrimitiveType.CreateWord(ctx.Bitrange.Extent);
                    var cNew = Constant.Create(dt, bitfield.Read(c.ToUInt64()));
                    return cNew;
                }
                else
                {
                    return c;
                }
            }

            public Expression VisitConversion(Conversion cnv, NarrowContext ctx)
            {
                var e = cnv.Expression.Accept(this, Ctx(cnv.Expression));
                var dtSrc = cnv.SourceDataType.ResolveAs<PrimitiveType>();
                var dtDst = cnv.DataType.ResolveAs<PrimitiveType>();
                if (dtSrc != null && dtDst != null)
                {
                    if ((dtDst.Domain & dtSrc.Domain & Domain.Integer) != 0)
                    {
                        // Slicing a sign- or zero-extension.
                        var range = new BitRange(0, dtSrc.BitSize);
                        if (range.Covers(ctx.Bitrange))
                        {
                            if (ctx.Bitrange.Covers(range))
                            {
                                return e;
                            }
                            else
                            {
                                return new Slice(ctx.DataType, e, ctx.Offset);
                            }
                        }
                    }
                }
                return new Conversion(e, cnv.SourceDataType, cnv.DataType);
            }

            public Expression VisitDereference(Dereference deref, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitFieldAccess(FieldAccess acc, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitIdentifier(Identifier id, NarrowContext ctx)
            {
                var sid = outer.ssa.Identifiers[id];
                if (!outer.availableSlices.TryGetValue(sid, out var available))
                    return id;

                // Buck stops here. Previous analysis pass has made sure there is 
                // an available slice.

                if (ctx.Bitrange.IsEmpty)
                {
                    sid.Uses.Remove(Statement);
                    return id;
                }
                if (outer.replaceIds.TryGetValue(sid, out var sidNew))
                {
                    sidNew.Uses.Add(Statement);
                    sid = sidNew;
                }

                if (available.TryGetValue(ctx.Bitrange, out var sidSlice) &&
                    sidSlice.DefStatement != Statement)
                {
                    sid.Uses.Remove(Statement);
                    sidSlice.Uses.Add(Statement);
                    return sidSlice.Identifier;
                }
                return MaybeSlice(sid.Identifier, ctx);
            }

            public Expression VisitMemberPointerSelector(MemberPointerSelector mps, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMemoryAccess(MemoryAccess access, NarrowContext ctx)
            {
                var ea = access.EffectiveAddress.Accept(this, Ctx(access.EffectiveAddress));
                var brMem = Ctx(access);
                Expression result = new MemoryAccess(access.MemoryId, ea, access.DataType);
                if (brMem.Bitrange.Covers(ctx.Bitrange) && !ctx.Bitrange.Covers(brMem.Bitrange))
                {
                    result = new Slice(ctx.DataType, result, ctx.Offset);
                }
                return result;
            }

            public Expression VisitMkSequence(MkSequence seq, NarrowContext ctx)
            {
                var seqNew = new List<Expression>();
                int bitPos = 0;
                int totalBits = 0;
                for (int i = seq.Expressions.Length - 1; i >= 0; --i)
                {
                    var elem = seq.Expressions[i];
                    var br = Ctx(elem);
                    var ctxElem = ctx.Bitrange.Offset(-bitPos);
                    var btIntersect = ctxElem & br.Bitrange;
                    if (btIntersect.IsEmpty)
                    {
                        outer.ssa.RemoveUses(Statement, elem);
                    }
                    else
                    {
                        var newElem = elem.Accept(this, br.Narrow(btIntersect));
                        seqNew.Add(newElem);
                        totalBits += newElem.DataType.BitSize;
                    }
                    bitPos += br.Bitrange.Extent;
                }
                Debug.Assert(seqNew.Count > 0, "What to do if 0 sequence elements are live?");
                if (seqNew.Count == 1)
                {
                    return seqNew[0];
                }
                else
                {
                    seqNew.Reverse();
                    var dtNew = PrimitiveType.CreateWord(totalBits);
                    return new MkSequence(dtNew, seqNew.ToArray());
                }
            }

            public Expression VisitOutArgument(OutArgument outArgument, NarrowContext ctx)
            {
                return outArgument;
            }

            public Expression VisitPhiFunction(PhiFunction phi, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPointerAddition(PointerAddition pa, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitProcedureConstant(ProcedureConstant pc, NarrowContext ctx)
            {
                return pc;
            }

            public Expression VisitScopeResolution(ScopeResolution scopeResolution, NarrowContext ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSegmentedAccess(SegmentedAccess access, NarrowContext ctx)
            {
                var b = access.BasePointer.Accept(this, Ctx(access.BasePointer));
                var ea = access.EffectiveAddress.Accept(this, Ctx(access.EffectiveAddress));
                var brMem = Ctx(access);
                Expression result = new SegmentedAccess(access.MemoryId, b, ea, access.DataType);
                if (brMem.Bitrange.Covers(ctx.Bitrange) && !ctx.Bitrange.Covers(brMem.Bitrange))
                {
                    result = new Slice(ctx.DataType, result, ctx.Offset);
                }
                return result;
            }

            public Expression VisitSlice(Slice slice, NarrowContext ctx)
            {
                var brSlice = Ctx(slice);
                var e = slice.Expression.Accept(this, brSlice);
                // If e has already been sliced, just pass it on.
                if (e.DataType.BitSize == slice.DataType.BitSize)
                    return e;
                else 
                    return new Slice(slice.DataType, e, slice.Offset);
            }

            public Expression VisitTestCondition(TestCondition tc, NarrowContext ctx)
            {
                var e = tc.Expression.Accept(this, Ctx(tc.Expression));
                return new TestCondition(tc.ConditionCode, e);
            }

            public Expression VisitUnaryExpression(UnaryExpression unary, NarrowContext ctx)
            {
                switch (unary.Operator)
                {
                case NegateOperator _:
                case FNegOperator _:
                case ComplementOperator _:
                    var e = unary.Expression.Accept(this, ctx);
                    return new UnaryExpression(unary.Operator, e.DataType, e);

                case NotOperator _:
                case AddressOfOperator _:
                    var ee = unary.Expression.Accept(this, Ctx(unary.Expression));
                    return new UnaryExpression(unary.Operator, unary.DataType, ee);
                }
                throw new NotImplementedException();
            }

            private Expression MaybeSlice(Expression expr, NarrowContext ctx)
            {
                var br = Ctx(expr);
                if (ctx.Bitrange.Covers(br.Bitrange))
                {
                    return expr;
                }
                else
                {
                    return new Slice(ctx.DataType, expr, ctx.Offset);
                }
            }
        }

        public class NarrowContext
        {
            public NarrowContext(DataType dt, int offset)
            {
                this.DataType = dt;
                this.Offset = offset;
                this.Bitrange = new BitRange(offset, offset + dt.BitSize);
            }

            public DataType DataType { get; }
            public int Offset { get; }
            public BitRange Bitrange { get; }

            public NarrowContext Narrow(BitRange range)
            {
                if (!range.Covers(this.Bitrange))
                {
                    var dt = PrimitiveType.CreateWord(range.Extent);
                    return new NarrowContext(dt, range.Lsb);
                }
                else
                {
                    return this;
                }
            }

            public override int GetHashCode()
            {
                return Bitrange.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is NarrowContext that &&
                    this.Bitrange.Equals(that.Bitrange);
            }

            public override string ToString()
            {
                return $"{DataType}: {this.Bitrange}";
            }
        }
    }
}