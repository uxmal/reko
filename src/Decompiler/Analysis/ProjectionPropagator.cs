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
            foreach (var stm in ssa.Procedure.Statements.ToArray())
            {
                prjf.Statement = stm;
                var instr = stm.Instruction.Accept(prjf);
                stm.Instruction = instr;
            }

            var wl = new WorkList<Statement>();
            while (wl.GetWorkItem(out var stm))
            {
                var instr = stm.Instruction.Accept(this);
                stm.Instruction = instr;
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
                this.Projections = new HashSet<Expression>();
            }

            public HashSet<Expression> Projections { get; }
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

                // We have a sequence of IDs. Are they from the same storage?
                var sd = ids[0].Storage.Domain;
                Identifier idWide;
                if (AllSame(ids, (a, b) => a.Storage.Domain == b.Storage.Domain))
                {
                    var bits = ids.Aggregate(
                        new BitRange(),
                        (br, id) => br | id.Storage.GetBitRange());
                    var regWide = arch.GetRegister(sd, bits);
                    idWide = ssa.Procedure.Frame.EnsureRegister(regWide);
                }
                else
                {
                    //$BUG: EnsureSequence needs to take an arbitrary # of subregisters.
                    idWide = ssa.Procedure.Frame.EnsureSequence(ids[0].Storage, ids[1].Storage, seq.DataType);
                }

                var ass = sids.Select(s => s.DefStatement.Instruction as Assignment).ToArray();
                if (ass.All(a => a != null))
                {
                    // All assignments. Are they all slices?
                    var slices = ass.Select(AsSlice).ToArray();
                    if (slices.All(s => s != null) &&
                        AllSame(slices, (a, b) => a.Expression == b.Expression))
                    {
                        return RewriteSeqOfSlices(sids, slices);
                    }
                    throw new NotImplementedException();
                }

                if (sids.All(s => s.DefStatement.Instruction is DefInstruction))
                {
                    // All the identifiers are generated by def statements.
                    return RewriteSeqOfDefs(sids, idWide);
                }

                throw new NotImplementedException();
            }

            private Expression RewriteSeqOfSlices(SsaIdentifier[] sids, Slice[] slices)
            {
                // We have:
                //  sid_1 = SLICE(sid_0,...)
                //  sid_2 = SLICE(sid_0,...)
                //  ...
                //  ...SEQ(sid_1, sid_2)
                // We want:
                // ...sid_0
                foreach (var sid in sids)
                {
                    sid.Uses.Remove(this.Statement);
                }
                var idWide = (Identifier) slices[0].Expression;
                var sidWide = ssa.Identifiers[idWide];
                sidWide.Uses.Add(this.Statement);
                return idWide;
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

                // Remove the individual defs.
                foreach (var s in sids)
                {
                    ssa.DeleteStatement(s.DefStatement);
                    s.Uses.Remove(this.Statement);
                }
                // Add a def for the wide register.
                var sidWide = ssa.EnsureSsaIdentifier(idWide, ssa.Procedure.EntryBlock);
                sidWide.Uses.Add(this.Statement);
                return sidWide.Identifier;
            }
        }
    }
}
