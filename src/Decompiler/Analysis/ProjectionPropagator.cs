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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// The purpose of this class is to resolve projections to make for cleaner
/// code. We have widening projections and narrowing projections to take care of.
/// Widening projections come in two kinds:
/// * Sequences where we use adjacent pieces of the same register:
///     de = SEQ(h, l)
/// * Sequences where we use two separate registers as a whole register.
///     es_bx = SEQ(dx, ax)
/// * Sequences where we use adjacent parts of the stack:
///     dwLoc0010 = SEQ(wLoc0012, wLoc0010)
/// * Sequences where we use adjacent parts of memory
///     es_bx = SEQ(Mem11[0x0234:word16],Mem11[0x0232:word16])
/// We convert SEQ(reg1,reg2) to either the widened register (i.e. hl)
/// the combined register dx_ax, or a widenened memory access, then "push" the widened
/// register to all the statements that use both halves.
/// 
/// Narrowing projections are casts or slices:
///     al = (byte) rax
///     bh = SLICE(rbx, 8, 8)
/// </summary>
public class ProjectionPropagator : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new TraceSwitch(nameof(ProjectionPropagator), "Traces projection propagator") { Level = TraceLevel.Warning };

    private readonly AnalysisContext context;

    public ProjectionPropagator(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "prpr";

    public string Description => "Propagates slices and sequences, trying to build larger values";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var sac = new SegmentedAccessClassifier(ssa);
        sac.Classify();
        var worker = new Worker(ssa, sac);
        bool changed = worker.Transform();
        return (ssa, changed);
    }

    private class Worker : InstructionTransformer
    {
        private readonly SegmentedAccessClassifier sac;
        private readonly SsaState ssa;
        public bool changed;

        public Worker(SsaState ssa, SegmentedAccessClassifier sac)
        {
            this.ssa = ssa;
            this.sac = sac;
        }

        public bool Transform()
        {
            var wl = WorkList.Create(ssa.Procedure.Statements);
            while (wl.TryGetWorkItem(out var stm))
            {
                var prjf = new ProjectionFilter(ssa, stm, sac);
                var instr = stm.Instruction.Accept(prjf);
                stm.Instruction = instr;
                wl.AddRange(prjf.NewStatements);
                changed |= prjf.Changed;
            }
            return changed;
        }

        private class ProjectionFilter : InstructionTransformer
        {
            private readonly IProcessorArchitecture arch;
            private readonly SsaState ssa;
            private readonly SegmentedAccessClassifier sac;
            private readonly ExpressionValueComparer cmp;

            public ProjectionFilter(SsaState ssa, Statement stm, SegmentedAccessClassifier sac)
            {
                this.ssa = ssa;
                this.Statement = stm;
                this.sac = sac;
                this.arch = ssa.Procedure.Architecture;
                this.NewStatements = new HashSet<Statement>();
                this.cmp = new ExpressionValueComparer();
            }

            public bool Changed { get; private set; }
            public HashSet<Statement> NewStatements{ get; }
            public Statement Statement { get; private set; }

            private static bool AllSame<T>(T[] items, Func<T, T, bool> cmp)
            {
                var first = items[0];
                for (int i = 1; i < items.Length; ++i)
                {
                    if (!cmp(first, items[i]))
                        return false;
                }
                return true;
            }

            /// <summary>
            /// Returns true if all the slices are slicing the same storage and 
            /// all the slices are bitwise adjacent.
            /// </summary>
            /// <param name="slices"></param>
            private static bool AllAdjacent(Slice[] slices)
            {
                int lsbLast = slices[0].Offset;
                for (int i = 1; i < slices.Length; ++i)
                {
                    if (lsbLast != slices[i].Offset + slices[i].DataType.BitSize)
                        return false;
                    lsbLast = slices[i].Offset;
                }
                return true;
            }

            public override Expression VisitSegmentedAddress(SegmentedPointer address)
            {
                Expression? e = base.VisitSegmentedAddress(address);
                if (e is not SegmentedPointer accessNew)
                    return e;

                if (accessNew.BasePointer is not Identifier idSeg)
                    return e;
                if (sac.AssociatedIdentifier(idSeg) is null)
                    return e;
                var sidSeg = ssa.Identifiers[idSeg];
                if (accessNew.Offset is Identifier idEa)
                {
                    var sidEa = ssa.Identifiers[idEa];
                    var sids = new[] { sidSeg, sidEa };
                    e = FuseIdentifiers(MakeSegPtr(idEa.DataType), sids);
                    if (e is not null)
                        return e;
                }
                else if (accessNew.Offset is BinaryExpression binEa)
                {
                    if (binEa.Left is Identifier idLeft)
                    {
                        var sidLeft = ssa.Identifiers[idLeft];
                        e = FuseIdentifiers(MakeSegPtr(idLeft.DataType), sidSeg, sidLeft);
                        if (e is not null)
                        {
                            var r = Extend(binEa.Right, e.DataType);
                            return new BinaryExpression(
                                    binEa.Operator,
                                    e.DataType,
                                    e,
                                    r);
                        }
                    }
                    if (binEa.Right is Identifier idRight)
                    {
                        var sidRight = ssa.Identifiers[idRight];
                        e = FuseIdentifiers(MakeSegPtr(idRight.DataType), sidSeg, sidRight);
                        if (e is not null)
                        {
                            var l = Extend(binEa.Left, e.DataType);
                            return new BinaryExpression(
                                    binEa.Operator,
                                    e.DataType,
                                    l,
                                    e);
                        }
                    }
                }
                return accessNew;
            }

            private Expression Extend(Expression e, DataType dataType)
            {
                var dtExtended = PrimitiveType.Create(e.DataType.Domain, dataType.BitSize);
                if (e is Constant c)
                {
                    return Constant.Create(dtExtended, c.ToInt64());
                }
                else
                {
                    return new Conversion(e, e.DataType, dtExtended);
                }
            }

            private static DataType? MakeSegPtr(DataType dtEa)
            {
                if (dtEa is MemberPointer mptr)
                    return new Pointer(mptr.Pointee, 32);
                else
                    return PrimitiveType.SegPtr32;
            }

            public override Expression VisitMkSequence(MkSequence seq)
            {
                Debug.Assert(seq.Expressions.Length > 0);
                var ids = seq.Expressions.Select(e => e as Identifier).ToArray();
                if (ids.Any(i => i is null))
                    return seq;
                var sids = ids.Select(i => ssa.Identifiers[i!]).ToArray();

                var expFused = FuseIdentifiers(null, sids);
                return expFused ?? seq;
            }


            /// <summary>
            /// Attempt to fuse together the definitions of all the identifiers in <paramref name="sids"/>.
            /// </summary>
            /// <param name="dtWide">The data type of the result.</param>
            /// <param name="sids">Identifiers to fuse.</param>
            /// <returns>A new expression if the fusion succeeded, otherwise null.</returns>
            private Expression? FuseIdentifiers(DataType? dtWide, params SsaIdentifier[] sids)
            {
                // Are all the definitions of the ids in the same basic block? If they're
                // not, we give up.

                if (!AllSame(sids, (a, b) =>
                    a.DefStatement.Block == b.DefStatement.Block &&
                    a.Identifier.Storage.GetType() == b.Identifier.Storage.GetType()))
                    return null;

                if (dtWide is null)
                {
                    // Caller has no opinion about the resulting data type, so fall back
                    // on word.
                    dtWide = PrimitiveType.CreateWord(sids.Sum(s => s.Identifier.DataType.BitSize));
                }

                Identifier? idWide = GenerateWideIdentifier(dtWide, sids);
                if (idWide is not null && idWide.DataType.BitSize != dtWide.BitSize)
                    return null;
                var ass = sids.Select(s => s.DefStatement.Instruction as Assignment).ToArray();
                if (ass.All(a => a is not null))
                {
                    // All assignments. Are they all slices?
                    var slices = ass.Select(AsSlice).ToArray();
                    if (slices.All(s => s is not null))
                    {
                        if (AllSame(slices, (a, b) => cmp.Equals(a!.Expression, b!.Expression)) &&
                            AllAdjacent(slices!))
                        {
                            trace.Verbose("Prpr: Fusing slices in {0}", ssa.Procedure.Name);
                            trace.Verbose("{0}", string.Join(Environment.NewLine, ass.Select(a => $"    {a}")));
                            Changed = true;
                            return RewriteSeqOfSlices(dtWide, sids, slices!);
                        }
                    }
                    trace.Warn("Prpr: Couldn't fuse assignments in {0}", ssa.Procedure.Name);
                    trace.Warn("{0}", string.Join(Environment.NewLine, ass.Select(a => $"    {a}")));
                    return null;
                }

                if (idWide is not null)
                {
                    if (sids.All(s => s.DefStatement.Instruction is DefInstruction))
                    {
                        // All the identifiers are generated by def statements.
                        Changed = true;
                        return RewriteSeqOfDefs(sids, idWide);
                    }

                    var phis = sids.Select(s => s.DefStatement.Instruction as PhiAssignment).ToArray();
                    if (phis.All(a => a is not null))
                    {
                        // We have a sequence of phi functions
                        Changed = true;
                        return RewriteSeqOfPhi(sids, phis!, idWide);
                    }
                    if (sids[0].DefStatement.Instruction is CallInstruction call &&
                        sids.All(s => s.DefStatement == sids[0].DefStatement))
                    {
                        // All of the identifiers in the sequence were defined by the same call.
                        Changed = true;
                        return RewriteSeqDefinedByCall(sids, call, idWide);
                    }
                }
                trace.Warn("Prpr: Couldn't fuse statements in {0}", ssa.Procedure.Name);
                trace.Warn("{0}", string.Join(Environment.NewLine, sids.Select(s => $"    {s.DefStatement}")));
                return null;
            }

            private Identifier? GenerateWideIdentifier(DataType dt, SsaIdentifier[] sids)
            {
                var sd = sids[0].Identifier.Storage.Domain;
                Identifier? idWide;
                if (AllSame(sids, (a, b) => a.Identifier.Storage.Domain == b.Identifier.Storage.Domain))
                {
                    var bits = sids.Aggregate(
                        new BitRange(),
                        (br, sid) => br | sid.Identifier.Storage.GetBitRange());
                    var regWide = arch.GetRegister(sd, bits);
                    idWide = (regWide is not null)
                        ? ssa.Procedure.Frame.EnsureRegister(regWide)
                        : null;
                }
                else if (sids.All(sid => sid.Identifier.Storage is StackStorage))
                {
                    idWide = CombineAdjacentStorages(sids);
                }
                else
                {
                    idWide = ssa.Procedure.Frame.EnsureSequence(
                        dt,
                        sids.Select(s => s.Identifier.Storage).ToArray());
                }
                return idWide;
            }

            private Identifier? CombineAdjacentStorages(SsaIdentifier [] sids)
            {
                var stgs = sids.Select(s => (StackStorage) s.Identifier.Storage)
                    .OrderBy(s => s.StackOffset)
                    .ToArray();
                int byteOffsetMin = stgs[0].StackOffset;
                int byteOffsetMax = byteOffsetMin + stgs[0].DataType.Size;
                for (int i = 1; i < stgs.Length; ++i)
                {
                    if (stgs[i].StackOffset != byteOffsetMax)
                        return null;
                    byteOffsetMax += stgs[i].DataType.Size;
                }
                var word = PrimitiveType.CreateWord(DataType.BitsPerByte * (byteOffsetMax - byteOffsetMin));
                return ssa.Procedure.Frame.EnsureStackVariable(byteOffsetMin, word);
            }

            /// <summary>
            /// Given an sequence of adjacent slices, rewrite the sequence a single
            /// slice. If the slice is a no-op, just return the underlying storage.
            /// </summary>
            /// <param name="sids"></param>
            /// <param name="slices"></param>
            /// <returns></returns>
            private Expression RewriteSeqOfSlices(DataType dtSequence, SsaIdentifier[] sids, Slice[] slices)
            {
                // We have:
                //  sid_1 = SLICE(sid_0,...)
                //  sid_2 = SLICE(sid_0,...)
                //  ...
                //  ...SEQ(sid_1, sid_2)
                // We want:
                // ...SLICE(sid_0, ...)
                // and ideally
                // ...sid_0
                foreach (var sid in sids)
                {
                    sid.Uses.Remove(this.Statement);
                }
                var totalSliceSize = slices.Sum(s => s.DataType.BitSize);
                var totalSliceOffset = slices[^1].Offset;
                var expWide = slices[0].Expression;
                var ua = new InstructionUseAdder(this.Statement, ssa.Identifiers);
                expWide.Accept(ua);
                if (expWide is Identifier idWide)
                {
                    if ((int) idWide.Storage.BitAddress == totalSliceOffset &&
                        (int) idWide.Storage.BitSize == totalSliceSize)
                    {
                        return idWide;
                    }
                }
                else
                {
                    if (expWide.DataType.BitSize == totalSliceSize)
                    {
                        return expWide;
                    }
                }
                return new Slice(dtSequence, expWide, totalSliceOffset);
            }

            private Slice? AsSlice(Assignment? ass)
            {
                if (ass is null)
                    return null;
                if (ass.Src is Slice slice)
                    return slice;
                if (ass.Src is Cast cast)
                    return new Slice(cast.DataType, cast.Expression, 0);
                return null;
            }

            private Identifier RewriteSeqOfDefs(SsaIdentifier[] sids, Identifier idWide)
            {
                // We have:
                //  def a
                //  def b
                //  ....SEQ(a, b)
                // We want:
                //  def a_b
                //  a = SLICE(a_b...)
                //  b = SLICE(a_b...)
                //  ....a_b
                // It's likely that the a = SLICE(...) statements
                // will be dead after this transformation, which
                // DeadCode.Eliminate will discover.

                // Add a def for the wide register, placing it "above" or "before"
                // the narrow 'defs', which will be mutated to slices.
                var sidWide = ssa.Identifiers.Add(idWide, null, false);
                sidWide.DefStatement = ssa.Procedure.EntryBlock.Statements.Insert(
                    0,
                    ssa.Procedure.EntryAddress,
                    new DefInstruction(sidWide.Identifier));
                sidWide.Uses.Add(this.Statement);

                // Replace uses of the individual defs.
                foreach (var s in sids)
                {
                    s.Uses.Remove(this.Statement);
                    s.DefStatement.Instruction = new Assignment(
                        s.Identifier,
                        new Slice(s.Identifier.DataType, idWide, idWide.Storage.OffsetOf(s.Identifier.Storage)));
                    sidWide.Uses.Add(s.DefStatement);
                }
                return sidWide.Identifier;
            }

            private Expression RewriteSeqOfPhi(SsaIdentifier[] sids, PhiAssignment[] phis, Identifier idWide)
            {
                // We have
                //     a_3 = PHI(a_1, a_2)
                //     b_3 = PHI(b_1, a_2)
                //     ...SEQ(a_3,b_3)
                // and we want 
                //    ab_3 = PHI(ab_1, ab_2)
                //    a_3 = SLICE(ab_3, ...)
                //    b_3 = SLICE(ab_3, ...)
                //    ...ab_3

                foreach (var s in sids)
                {
                    s.Uses.Remove(this.Statement);
                }

                // Insert a PHI statement placeholder at the beginning
                // of the basic block.
                var stmPhi = sids[0].DefStatement.Block.Statements.Insert(
                    0,
                    sids[0].DefStatement.Address,
                    null!);

                // Generate fused identifiers for all phi slots.
                var widePhiArgs = new List<PhiArgument>();
                for (var iBlock = 0; iBlock < phis[0].Src.Arguments.Length; ++iBlock)
                {
                    // Make a fused identifier in each predecessor block and "push"
                    // the SEQ statements into the predecessors.
                    var pred = phis[0].Src.Arguments[iBlock].Block;
                    var sidPred = MakeFusedIdentifierInPredecessorBlock(sids, phis, idWide, stmPhi, iBlock, pred);
                    widePhiArgs.Add(new PhiArgument(pred, sidPred.Identifier));
                }

                var sidDst = ssa.Identifiers.Add(idWide, stmPhi, false);
                stmPhi.Instruction = new PhiAssignment(
                    sidDst.Identifier,
                    widePhiArgs.ToArray());
                sidDst.Uses.Add(this.Statement);

                // Replace all the "unfused" phis with slices of the "fused" phi.
                foreach (var sid in sids)
                {
                    ssa.RemoveUses(sid.DefStatement);
                    sid.DefStatement.Instruction = new AliasAssignment(
                        sid.Identifier,
                        new Slice(sid.Identifier.DataType, sidDst.Identifier, idWide.Storage.OffsetOf(sid.Identifier.Storage)));
                    sidDst.Uses.Add(sid.DefStatement);
                }
                return sidDst.Identifier;
            }

            /// <summary>
            /// Given a sequence of SSA identifiers in <paramref name="sids"/>, generate
            /// add a new SSA identifier whose definition is sid_new = SEQ(sids...) and
            /// place it in the basic block <paramref name="pred"/>.
            /// </summary>
            /// <remarks>
            /// If the SSA identifiers all all slices of the same identifier, short-circuit
            /// the work by using the sliced identifier directly.
            /// </remarks>
            private SsaIdentifier MakeFusedIdentifierInPredecessorBlock(SsaIdentifier[] sids, PhiAssignment[] phis, Identifier idWide, Statement stmPhi, int iBlock, Block pred)
            {
                SsaIdentifier sidPred;
                var sidPreds = phis.Select(p => ssa.Identifiers[(Identifier) p.Src.Arguments[iBlock].Value].DefStatement.Instruction as Assignment).ToArray();
                var slices = sidPreds.Select(AsSlice).ToArray();
                var aliases = sidPreds.Select(s => s as AliasAssignment).ToArray();
                if (slices.All(s => s is not null) &&
                    AllSame(slices, (a, b) => this.cmp.Equals(a!.Expression, b!.Expression)) &&
                    AllAdjacent(slices!))
                {
                    if (slices[0]!.Expression is Identifier id)
                    {
                        // All sids were slices of the same identifier `id`,
                        // so just use that instead.
                        sidPred = ssa.Identifiers[id];
                        sidPred.Uses.Add(stmPhi);
                        return sidPred;
                    }
                }

                var stmPred = AddStatementToEndOfBlock(pred, sids[0].DefStatement.Address, null!);
                sidPred = ssa.Identifiers.Add(idWide, stmPred, false);
                var phiArgs = phis.Select(p => p.Src.Arguments[iBlock].Value).ToArray();
                stmPred.Instruction =
                    new AliasAssignment(
                        sidPred.Identifier,
                        new MkSequence(
                            sidPred.Identifier.DataType,
                            phiArgs));
                this.NewStatements.Add(stmPred);
                ssa.AddUses(stmPred);

                sidPred.Uses.Add(stmPhi);
                return sidPred;
            }

            //$REFACTOR: move this to SsaState
            private static Statement AddStatementToEndOfBlock(Block pred, Address addr, Instruction instr)
            {
                int i = pred.Statements.Count;
                if (i > 0 && pred.Statements[i - 1].Instruction.IsControlFlow)
                    --i;
                var stmPred = pred.Statements.Insert(i, addr, instr);
                return stmPred;
            }

            private Expression RewriteSeqDefinedByCall(SsaIdentifier [] sids, CallInstruction call, Identifier idWide)
            {
                // We have:
                // call
                //    def: a_1, b_2
                // ...
                // ...SEQ(a_1, b_2)
                // We want 
                // call
                //    def: a_b_3
                // a_1 = SLICE(a_b_3, ...)
                // b_2 = SLICE(a_b_3, ...)
                // ...
                // ...a_b_3
                var callStm = sids[0].DefStatement;
                foreach (var s in sids)
                {
                    s.Uses.Remove(this.Statement);
                }

                // Create an SSA ID for the fused identifier and replace the
                // definitions of its parts with a single definition of the fused identifier
                var sidDst = ssa.Identifiers.Add(idWide, callStm, false);
                foreach (var sid in sids)
                {
                    call.Definitions.RemoveWhere(cb => cb.Expression == sid.Identifier);
                }
                call.Definitions.Add(new CallBinding(sidDst.Identifier.Storage, sidDst.Identifier));
                sidDst.Uses.Add(this.Statement);

                // Add alias assignments for the sub-identifiers after the call statement.
                var block = callStm!.Block;
                int iStm = block.Statements.IndexOf(callStm) + 1;
                foreach (var s in sids)
                {
                    var stmAss = block.Statements.Insert(
                        iStm,
                        callStm.Address,
                        new AliasAssignment(
                            s.Identifier,
                            new Slice(s.Identifier.DataType, sidDst.Identifier, idWide.Storage.OffsetOf(s.Identifier.Storage))));
                    sidDst.Uses.Add(stmAss);
                    s.DefStatement = stmAss;
                    ++iStm;
                }
                return sidDst.Identifier;
            }
        }
    }
}
