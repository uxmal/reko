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
using System.Runtime.CompilerServices;

namespace Reko.Analysis
{
    /// <summary>
    /// This transformation attempts to reduce the number of 
    /// <see cref="Slice"/> expressions in a procedure by locating all
    /// occurrences of slices and "pushing" them towards the roots
    /// of expressions.
    /// </summary>
    public class SlicePropagator
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(SlicePropagator), "Trace progress of SlicePropagator") { Level = TraceLevel.Verbose };        private readonly SsaState ssa;
        
        private readonly DecompilerEventListener listener;
        private readonly Dictionary<SsaIdentifier, HashSet<BitRange>> neededSlices;  // Slices we want, but don't have
        private readonly Dictionary<SsaIdentifier, Dictionary<BitRange, SsaIdentifier>> availableSlices; // Slices we do have.
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
                if (sid.Uses.Count == 0)
                    continue;
                var available = new Dictionary<BitRange, SsaIdentifier> { { Br(sid.Identifier), sid } };
                this.availableSlices.Add(sid, available);

                this.neededSlices.Add(sid, new HashSet<BitRange>());
            }
        }

        public void Transform()
        {
            DetermineNeededSlices();
            GenerateNeededSlices();
            ReplaceSlices();
        }

        private void DetermineNeededSlices()
        {
            var wl = new WorkList<(SsaIdentifier, BitRange)>(ssa.Identifiers
                .Where(s => s.DefStatement != null)
                .Select(s => (s, Br(s.Identifier))));
            var sliceFinder = new SliceFinder(this, wl);
            while (wl.GetWorkItem(out var item))
            {
                item.Item1.DefStatement.Instruction.Accept(sliceFinder, item.Item2);
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
                var brOriginal = Br(sidOld.Identifier);
                if (brNeeded.Covers(brOriginal))
                    continue;

                // At this point, we've discovered that only one slice size 
                // of the sidOld is ever used, and it is always smaller than 
                // the original size of needed.Key. We can create a smaller 
                // identifier, and replace all uses of the original wide variable
                // with the new narrower identifier.

                // We need a smaller slice. Does it already exist?
                if (this.availableSlices[sidOld].TryGetValue(brNeeded, out var sidNew))
                {
                    // Delete the existing alias statement, and rely on SlicePusher to 
                    // replace it.
                    sidNew.DefStatement.Block.Statements.Remove(sidNew.DefStatement);
                }
                else
                {
                    // An appropriate variable doesn't exist, so we need to make a new slicing alias.
                    sidNew = MakeSlicedIdentifier(sidOld, brNeeded);
                    this.availableSlices[sidOld].Add(brNeeded, sidNew);
                }

                // Now remmove the uses of the old identifier. New uses will be added
                // by the SlicePusher.
                sidOld.Uses.Clear();
                this.replaceIds.Add(sidOld, sidNew);
                this.availableSlices.Add(sidNew, new Dictionary<BitRange, SsaIdentifier> { { brNeeded, sidNew } });
                trace.Verbose("Slp: Narrowing {0} down to {1} ({2})", sidOld.Identifier, sidNew.Identifier, brNeeded);
            }
        }

        private SsaIdentifier MakeSlicedIdentifier(SsaIdentifier sidOriginal, BitRange brNeeded)
        {
            var dt = PrimitiveType.CreateWord(brNeeded.Extent);
            switch (sidOriginal.Identifier.Storage)
            {
            case RegisterStorage reg:
                var regSliced = ssa.Procedure.Architecture.GetRegister(reg.Domain, brNeeded);
                var idSliced = ssa.Procedure.Frame.EnsureRegister(regSliced);
                idSliced.DataType = dt;
                var sidTo = ssa.Identifiers.Add(idSliced, sidOriginal.DefStatement, new Slice(dt, sidOriginal.Identifier, brNeeded.Lsb), false);
                return sidTo;
            default:
                break;
            }
            throw new NotImplementedException();
        }

        private void ReplaceSlices()
        {
            var slicePusher = new SlicePusher(this);
            foreach (var stm in ssa.Procedure.Statements)
            {
                slicePusher.Statement = stm;
                stm.Instruction = stm.Instruction.Accept(slicePusher);
            }
        }

        private static BitRange Br(Expression e)
        {
            return new BitRange(0, e.DataType.BitSize);
        }

        private static BitRange Br(Slice slice)
        {
            return new BitRange(slice.Offset, slice.Offset + slice.DataType.BitSize);
        }

        private static PrimitiveType Word(BitRange range)
        {
            return PrimitiveType.CreateWord(range.Extent);
        }

        /// <summary>
        /// This visitor class discovers Slices and Casts in the program and propagates these 
        /// to the leaves of the Expressions in the procedure. If an Identifier is encountered
        /// it records the size of the slice at that point.
        /// </summary>
        public class SliceFinder : InstructionVisitor<Instruction, BitRange>, ExpressionVisitor<Expression, BitRange>
        {
            private readonly SlicePropagator outer;
            private readonly WorkList<(SsaIdentifier, BitRange)> wl;

            public SliceFinder(SlicePropagator outer, WorkList<(SsaIdentifier, BitRange)> wl)
            {
                this.outer = outer;
                this.wl = wl;
            }

            public Instruction VisitAssignment(Assignment ass, BitRange ctx)
            {
                ass.Src.Accept(this, ctx);
                if (ass.Src is Slice slice && slice.Expression is Identifier slicedId)
                {
                    var sidSlice = outer.ssa.Identifiers[ass.Dst];
                    var sidOriginal = outer.ssa.Identifiers[slicedId];
                    var br = Br(slice);
                    if (!outer.availableSlices[sidOriginal].ContainsKey(br))
                    {
                        outer.availableSlices[sidOriginal].Add(br, sidSlice);
                    }
                }
                return ass;
            }

            public Instruction VisitBranch(Branch branch, BitRange ctx)
            {
                branch.Condition.Accept(this, Br(branch.Condition));
                return branch;
            }

            public Instruction VisitCallInstruction(CallInstruction ci, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitComment(CodeComment comment, BitRange ctx)
            {
                return comment;
            }

            public Instruction VisitDeclaration(Declaration decl, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitDefInstruction(DefInstruction def, BitRange ctx)
            {
                return def;
            }

            public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction, BitRange ctx)
            {
                gotoInstruction.Target.Accept(this, Br(gotoInstruction.Target));
                throw new NotImplementedException();
            }

            public Instruction VisitPhiAssignment(PhiAssignment phi, BitRange ctx)
            {
                foreach (var arg in phi.Src.Arguments)
                {
                    arg.Value.Accept(this, ctx);
                }
                return phi;
            }

            public Instruction VisitReturnInstruction(ReturnInstruction ret, BitRange ctx)
            {
                if (ret.Expression != null)
                {
                    ret.Expression.Accept(this, Br(ret.Expression));
                }
                return ret;
            }

            public Instruction VisitSideEffect(SideEffect side, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitStore(Store store, BitRange ctx)
            {
                var br = Br(store.Dst);
                store.Src.Accept(this, br);
                store.Dst.Accept(this, Br(store.Dst));
                return store;
            }

            public Instruction VisitSwitchInstruction(SwitchInstruction si, BitRange ctx)
            {
                si.Expression.Accept(this, Br(si.Expression));
                return si;
            }

            public Instruction VisitUseInstruction(UseInstruction use, BitRange ctx)
            {
                use.Expression.Accept(this, Br(use.Expression));
                return use;
            }

            public Expression VisitAddress(Address addr, BitRange ctx)
            {
                return addr;
            }

public Expression VisitApplication(Core.Expressions.Application appl, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitArrayAccess(ArrayAccess acc, BitRange ctx)
{
    throw new NotImplementedException();
}

            public Expression VisitBinaryExpression(BinaryExpression binExp, BitRange ctx)
            {
                switch (binExp.Operator)
                {
                case IAddOperator _:
                case ISubOperator _:
                case AndOperator _:
                case OrOperator _:
                case XorOperator _:
                case ConditionalOperator _:
                    binExp.Left.Accept(this, ctx);
                    binExp.Right.Accept(this, ctx);
                    return binExp;
                case ShlOperator _:
                case ShrOperator _:
                case SarOperator _:
                    binExp.Left.Accept(this, ctx);
                    binExp.Right.Accept(this, Br(binExp.Right));
                    return binExp;
                }
                throw new NotImplementedException($"SliceFinder not implemented for {binExp.Operator.GetType().Name}.");
            }

            public Expression VisitCast(Cast cast, BitRange ctx)
            {
                cast.Expression.Accept(this, Br(cast.Expression));
                return cast;
            }

            public Expression VisitConditionalExpression(ConditionalExpression c, BitRange context)
            {
                c.Condition.Accept(this, Br(c.Condition));
                c.ThenExp.Accept(this, Br(c.ThenExp));
                c.FalseExp.Accept(this, Br(c.FalseExp));
                return c;
            }

            public Expression VisitConditionOf(ConditionOf cof, BitRange ctx)
            {
                cof.Expression.Accept(this, Br(cof.Expression));
                return cof;
            }

            public Expression VisitConstant(Constant c, BitRange ctx)
            {
                return c;
            }

public Expression VisitDepositBits(DepositBits d, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitDereference(Dereference deref, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitFieldAccess(FieldAccess acc, BitRange ctx)
{
    throw new NotImplementedException();
}

            public Expression VisitIdentifier(Identifier id, BitRange ctx)
            {
                if (!ctx.Covers(Br(id)))
                {
                    trace.Verbose("SLP: found a narrowed use of {0} ({1}", id, ctx);
                    var sid = outer.ssa.Identifiers[id];
                    var added = outer.neededSlices[sid].Add(ctx);
                    if (added)
                    {
                        trace.Verbose("SLP: Id {0} needs bitrange {1}", sid.Identifier.Name, ctx);
                        wl.Add((sid, ctx));
                    }
                }
                return id;
            }

public Expression VisitMemberPointerSelector(MemberPointerSelector mps, BitRange ctx)
{
    throw new NotImplementedException();
}

            public Expression VisitMemoryAccess(MemoryAccess access, BitRange ctx)
            {
                access.EffectiveAddress.Accept(this, Br(access.EffectiveAddress));
                return access;
            }

            public Expression VisitMkSequence(MkSequence seq, BitRange ctx)
            {
                int bitPos = 0;
                for (int i = seq.Expressions.Length-1; i >=0; --i)
                {
                    var elem = seq.Expressions[i];
                    var br = Br(elem);
                    var ctxElem = ctx.Offset(-bitPos);
                    var btIntersect = ctxElem & br;
                    if (!btIntersect.IsEmpty)
                    {
                        elem.Accept(this, btIntersect);
                    }
                    bitPos += br.Extent;
                }
                return seq;
            }

public Expression VisitOutArgument(OutArgument outArgument, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitPhiFunction(PhiFunction phi, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitPointerAddition(PointerAddition pa, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitProcedureConstant(ProcedureConstant pc, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitScopeResolution(ScopeResolution scopeResolution, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitSegmentedAccess(SegmentedAccess access, BitRange ctx)
{
    throw new NotImplementedException();
}

            public Expression VisitSlice(Slice slice, BitRange ctx)
            {
                var brSlice = Br(slice);
                slice.Expression.Accept(this, brSlice);
                return slice;
            }

public Expression VisitTestCondition(TestCondition tc, BitRange ctx)
{
    throw new NotImplementedException();
}

public Expression VisitUnaryExpression(UnaryExpression unary, BitRange ctx)
{
    throw new NotImplementedException();
}

        }

        /// <summary>
        /// This instruction visitor will 'push' slice expressions towards the leaves of an expression. Slicing an
        /// identifier may result in the use of an existing slice alias -- a code improvement. Likewise, slicing
        /// a Constant will also result in a code improvement.
        /// </summary>
        public class SlicePusher : InstructionVisitor<Instruction>, ExpressionVisitor<Expression, BitRange>
        {
            private readonly SlicePropagator outer;

            public SlicePusher(SlicePropagator outer)
            {
                this.outer = outer;
            }

            public Statement Statement { get; set; }

            public Instruction VisitAssignment(Assignment ass)
            {
                var sidDst = outer.ssa.Identifiers[ass.Dst];
                if (outer.replaceIds.TryGetValue(sidDst, out var sidDstNew))
                {
                    var src = ass.Src.Accept(this, Br(sidDstNew.Identifier));
                    sidDstNew.DefStatement = this.Statement;
                    sidDstNew.DefExpression = src;
                    sidDst.DefStatement = null;
                    sidDst.DefExpression = null;
                    return new Assignment(sidDstNew.Identifier, src);
                }
                else
                {
                    var src = ass.Src.Accept(this, Br(ass.Dst));
                    sidDst.DefExpression = src;
                    return new Assignment(ass.Dst, src);
                }
            }

            public Instruction VisitBranch(Branch branch)
            {
                var cond = branch.Condition.Accept(this, Br(branch.Condition));
                return new Branch(cond, branch.Target);
            }

            public Instruction VisitCallInstruction(CallInstruction ci)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitComment(CodeComment comment)
            {
                return comment;
            }

            public Instruction VisitDeclaration(Declaration decl)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitDefInstruction(DefInstruction def)
            {
                //$REVIEW: what about user-defined parameters?
                var sidDst = outer.ssa.Identifiers[def.Identifier];
                if (outer.replaceIds.TryGetValue(sidDst, out var sidDstNew))
                {
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
                    return new PhiAssignment(sidDstNew.Identifier, phiFn);
                }
                else
                {
                    sidDst.DefExpression = phiFn;
                    return new PhiAssignment(phi.Dst, phiFn);
                }
            }

            public Instruction VisitReturnInstruction(ReturnInstruction ret)
            {
                if (ret.Expression == null)
                    return ret;
                else
                {
                    var e = ret.Expression.Accept(this, Br(ret.Expression));
                    return new ReturnInstruction(e);
                }
            }

            public Instruction VisitSideEffect(SideEffect side)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitStore(Store store)
            {
                var br = Br(store.Dst);
                var src = store.Src.Accept(this, br);
                var dst = store.Dst.Accept(this, br);
                return new Store(dst, src);
            }

            public Instruction VisitSwitchInstruction(SwitchInstruction si)
            {
                throw new NotImplementedException();
            }

            public Instruction VisitUseInstruction(UseInstruction use)
            {
                return use;
            }

            public Expression VisitAddress(Address addr, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitApplication(Core.Expressions.Application appl, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitArrayAccess(ArrayAccess acc, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitBinaryExpression(BinaryExpression binExp, BitRange ctx)
            {
                DataType dt = binExp.DataType;
                Expression left = binExp.Left;
                Expression right = binExp.Right;
                switch (binExp.Operator)
                {
                case IAddOperator _:
                case ISubOperator _:
                case AndOperator _:
                case OrOperator _:
                case XorOperator _:
                case ConditionalOperator _:
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
                default:
                    throw new NotImplementedException();
                }
                return new BinaryExpression(binExp.Operator, dt, left, right);
            }

            public Expression VisitCast(Cast cast, BitRange ctx)
            {
                var br = new BitRange(0, cast.DataType.BitSize);
                if (!ctx.Covers(br))
                {
                    var brExp = Br(cast.Expression);
                    if (brExp.Covers(ctx))
                    {
                        var e = cast.Expression.Accept(this, ctx);
                        return e;
                    }
                    else
                    {
                        var e = cast.Expression.Accept(this, brExp);
                        return new Cast(Word(ctx), e);
                    }
                }
                else
                {
                    return cast;
                }
            }

            public Expression VisitConditionalExpression(Core.Expressions.ConditionalExpression c, BitRange context)
            {
                throw new NotImplementedException();
            }

            public Expression VisitConditionOf(ConditionOf cof, BitRange ctx)
            {
                var c = cof.Expression.Accept(this, Br(cof.Expression));
                return new ConditionOf(c);
            }

            public Expression VisitConstant(Constant c, BitRange ctx)
            {
                var br = Br(c);
                if (!ctx.Covers(br))
                {
                    var bitfield = new Bitfield(ctx.Lsb, ctx.Extent);
                    var dt = PrimitiveType.CreateWord(ctx.Extent);
                    var cNew = Constant.Create(dt, bitfield.Read(c.ToUInt64()));
                    return cNew;
                }
                else
                {
                    return c;
                }
            }

            public Expression VisitDepositBits(DepositBits d, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitDereference(Dereference deref, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitFieldAccess(FieldAccess acc, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitIdentifier(Identifier id, BitRange ctx)
            {
                // Buck stops here. Previous analysis pass has made sure there is 
                // an available slice.

                var sid = outer.ssa.Identifiers[id];
                if (ctx.IsEmpty)
                {
                    sid.Uses.Remove(Statement);
                    return id;
                }
                if (outer.replaceIds.TryGetValue(sid, out var sidNew))
                {
                    sidNew.Uses.Add(Statement);
                    sid = sidNew;
                }

                if (outer.availableSlices[sid].TryGetValue(ctx, out var sidSlice))
                {
                    sid.Uses.Remove(Statement);
                    sidSlice.Uses.Add(Statement);
                    return sidSlice.Identifier;
                }
                return sid.Identifier;
            }

            public Expression VisitMemberPointerSelector(MemberPointerSelector mps, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitMemoryAccess(MemoryAccess access, BitRange ctx)
            {
                var ea = access.EffectiveAddress.Accept(this, Br(access.EffectiveAddress));
                var brMem = Br(access);
                DataType dt;
                if (brMem.Covers(ctx) && !ctx.Covers(brMem))
                {
                    dt = Word(ctx);
                }
                else
                {
                    dt = access.DataType;
                }
                return new MemoryAccess(access.MemoryId, ea, dt);
            }

            public Expression VisitMkSequence(MkSequence seq, BitRange ctx)
            {
                var seqNew = new List<Expression>();
                int bitPos = 0;
                int totalBits = 0;
                for (int i = seq.Expressions.Length - 1; i >= 0; --i)
                {
                    var elem = seq.Expressions[i];
                    var br = Br(elem);
                    var ctxElem = ctx.Offset(-bitPos);
                    var btIntersect = ctxElem & br;
                    var newElem = elem.Accept(this, btIntersect);
                    if (!btIntersect.IsEmpty)
                    {
                        seqNew.Add(newElem);
                        totalBits += newElem.DataType.BitSize;
                    }
                    bitPos += br.Extent;
                }
                Debug.Assert(seqNew.Count > 0, "What to do if 0 sequence elements are live?");
                if (seqNew.Count == 1)
                {
                    return seqNew[0];
                }
                else
                {
                    var dtNew = PrimitiveType.CreateWord(totalBits);
                    return new MkSequence(dtNew, seqNew.ToArray());
                }
            }

            public Expression VisitOutArgument(OutArgument outArgument, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPhiFunction(PhiFunction phi, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitPointerAddition(PointerAddition pa, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitProcedureConstant(ProcedureConstant pc, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitScopeResolution(ScopeResolution scopeResolution, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSegmentedAccess(SegmentedAccess access, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitSlice(Slice slice, BitRange ctx)
            {
                var brSlice = Br(slice);
                var e = slice.Expression.Accept(this, brSlice);
                return e;
            }

            public Expression VisitTestCondition(TestCondition tc, BitRange ctx)
            {
                throw new NotImplementedException();
            }

            public Expression VisitUnaryExpression(UnaryExpression unary, BitRange ctx)
            {
                throw new NotImplementedException();
            }
        }
    }
}