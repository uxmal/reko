#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// The purpose of this class is to resolve projections to make for cleaner
    /// code. We have widening projections and narrowing projections to take care of.
    /// Widening projections come in two kinds:
    /// * Sequences where we use adjacent pieces of the same register:
    ///     de = SEQ(h, l)
    /// * Sequences where we use two separate registers as a whole register.
    ///     es_bx = SEQ(dx, ax)
    /// We convert SEQ(reg1,reg2) to either the widened register (i.e. hl)
    /// or the combined register dx_ax, then "push" the widened register to all the 
    /// statements that use both halves.
    /// 
    /// Narrowing projections are casts or slices:
    ///     al = (byte) rax
    ///     bh = SLICE(rbx, 8, 8)
    /// </summary>
    public class ProjectionPropagator : InstructionTransformer
    {
        private IProcessorArchitecture arch;
        private SsaState ssa;

        public ProjectionPropagator(IProcessorArchitecture arch, SsaState ssa)
        {
            this.arch = arch;
            this.ssa = ssa;
        }

        public void Transform()
        {
            var prjf = new ProjectionFilter(arch, ssa);
            var wl = new WorkList<Statement>(ssa.Procedure.Statements);
            while (wl.GetWorkItem(out var stm))
            {
                prjf.Statement = stm;
                var instr = stm.Instruction.Accept(prjf);
                stm.Instruction = instr;
                wl.AddRange(prjf.NewStatements);
                prjf.NewStatements.Clear();
            }
        }

        private class ProjectionFilter : InstructionTransformer
        {
            private IProcessorArchitecture arch;
            private SsaState ssa;

            public ProjectionFilter(IProcessorArchitecture arch, SsaState ssa)
            {
                this.arch = arch;
                this.ssa = ssa;
                this.NewStatements = new HashSet<Statement>();
            }

            public HashSet<Statement> NewStatements{ get; }
            public Statement Statement { get; set; }

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
            /// <returns></returns>
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

            public override Expression VisitMkSequence(MkSequence seq)
            {
                Debug.Assert(seq.Expressions.Length > 0);
                var ids = seq.Expressions.Select(e => e as Identifier).ToArray();
                if (ids.Any(i => i == null))
                    return seq;
                // Are all the definitions of the ids in the same basic block? If they're
                // not, we give up.
                var sids = ids.Select(i => ssa.Identifiers[i]).ToArray();
                if (!AllSame(sids, (a, b) => a.DefStatement.Block == b.DefStatement.Block))
                    return seq;

                Identifier idWide = GenerateWideIdentifier(seq.DataType, sids);

                var ass = sids.Select(s => s.DefStatement.Instruction as Assignment).ToArray();
                if (ass.All(a => a != null))
                {
                    // All assignments. Are they all slices?
                    var slices = ass.Select(AsSlice).ToArray();
                    if (slices.All(s => s != null))
                    {
                        if (AllSame(slices, (a, b) => a.Expression == b.Expression) &&
                            AllAdjacent(slices))
                        {
                            return RewriteSeqOfSlices(seq.DataType, sids, slices);
                        }
                        return seq;
                    }
                    throw new NotImplementedException();
                }

                if (sids.All(s => s.DefStatement.Instruction is DefInstruction))
                {
                    // All the identifiers are generated by def statements.
                    return RewriteSeqOfDefs(sids, idWide);
                }

                var phis = sids.Select(s => s.DefStatement.Instruction as PhiAssignment).ToArray();
                if (phis.All(a => a != null))
                {
                    // We have a sequence of phi functions
                    return RewriteSeqOfPhi(sids, phis, idWide);
                }
                throw new NotImplementedException();
            }

            private Identifier GenerateWideIdentifier(DataType dt, SsaIdentifier[] sids)
            {
                var sd = sids[0].Identifier.Storage.Domain;
                Identifier idWide;
                if (AllSame(sids, (a, b) => a.Identifier.Storage.Domain == b.Identifier.Storage.Domain))
                {
                    var bits = sids.Aggregate(
                        new BitRange(),
                        (br, sid) => br | sid.Identifier.Storage.GetBitRange());
                    var regWide = arch.GetRegister(sd, bits);
                    idWide = ssa.Procedure.Frame.EnsureRegister(regWide);
                }
                else
                {
                    //$BUG: EnsureSequence needs to take an arbitrary # of subregisters.
                    idWide = ssa.Procedure.Frame.EnsureSequence(
                        dt,
                        sids[0].Identifier.Storage,
                        sids[1].Identifier.Storage);
                }
                return idWide;
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
                var totalSliceOffset = slices[slices.Length - 1].Offset;
                var idWide = (Identifier) slices[0].Expression;
                var sidWide = ssa.Identifiers[idWide];
                sidWide.Uses.Add(this.Statement);
                if ((int) idWide.Storage.BitAddress == totalSliceOffset &&
                    (int) idWide.Storage.BitSize == totalSliceSize)
                {
                    return idWide;
                }
                else
                {
                    return new Slice(dtSequence, idWide, totalSliceOffset);
                }
            }

            private Slice AsSlice(Assignment ass)
            {
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
                var sidWide = ssa.Identifiers.Add(idWide, null, null, false);
                sidWide.DefStatement = ssa.Procedure.EntryBlock.Statements.Insert(
                    0,
                    ssa.Procedure.EntryAddress.ToLinear(),
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
                //    ...ab_3

                foreach (var s in sids)
                {
                    s.Uses.Remove(this.Statement);
                }

                // Insert a PHI statement placeholder at the beginning
                // of the basic block.
                var stmPhi = sids[0].DefStatement.Block.Statements.Insert(
                    0,
                    sids[0].DefStatement.LinearAddress,
                    null);

                // Generate fused identifiers for all phi slots.
                var widePhiArgs = new List<PhiArgument>();
                for (var iBlock = 0; iBlock < phis[0].Src.Arguments.Length; ++iBlock)
                {
                    // Make a fused identifier in each predecessor block and "push"
                    // the SEQ statements into the predecessors.
                    var pred = phis[0].Src.Arguments[iBlock].Block;
                    var stmPred = pred.Statements.Add(
                        sids[0].DefStatement.LinearAddress,
                        null);
                    var sidPred = ssa.Identifiers.Add(idWide, stmPred, null, false);
                    var phiArgs = phis.Select(p => p.Src.Arguments[iBlock].Value).ToArray();
                    stmPred.Instruction = 
                        new Assignment(
                            sidPred.Identifier,
                            new MkSequence(
                                sidPred.Identifier.DataType,
                                phiArgs));
                    ssa.AddUses(stmPred);
                    sidPred.Uses.Add(stmPhi);
                    this.NewStatements.Add(stmPred);

                    widePhiArgs.Add(new PhiArgument(pred, sidPred.Identifier));
                }
                var sidDst = ssa.Identifiers.Add(idWide, stmPhi, null, false);
                stmPhi.Instruction = new PhiAssignment(
                        sidDst.Identifier,
                        widePhiArgs.ToArray());
                sidDst.Uses.Add(this.Statement);
                return sidDst.Identifier;
            }
        }
    }
}
