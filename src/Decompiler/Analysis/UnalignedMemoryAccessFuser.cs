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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// Fuses pairs of unaligned loads or stores.
    /// </summary>
    /// <remarks>
    /// This transform must run before value propagation, because 
    /// it depends on the availability of the source/target registers
    /// of the LWR/LWL and SWL/SWR instructions. Value propagation
    /// can wipe these out.
    /// </remarks>
    public class UnalignedMemoryAccessFuser : InstructionVisitorBase
    {
        private SsaState ssa;

        public UnalignedMemoryAccessFuser(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public void Transform()
        {
            foreach (var stm in ssa.Procedure.Statements.ToList())
            {
                stm.Instruction.Accept(this);
            }

            // Find all unaligned stores and group them by
            // register + instruction.
            var stores = FindAllUnalignedStoreInstructions(ssa.Procedure.Statements);

            // Find pairs of instructions that are (left/right) and
            // whose offsets differ by 3 bytes.
            var pairs = GroupUnalignedStorePairs(stores);

            // Fuse the pairs.
            FuseUnalignedPairs(pairs);
        }

        public override void VisitAssignment(Assignment a)
        {
            base.VisitAssignment(a);
            FuseUnalignedLoads(a);
        }

        private static readonly string[] unalignedLoadsLe = new[]
        {
            PseudoProcedure.LwR
        };

        private static readonly string[] unalignedLoadsBe = new[]
        {
            PseudoProcedure.LwL
        };


        // On MIPS-LE the sequence
        //   lwl rx,K+3(ry)
        //   lwr rx,K(ry)
        // is an unaligned read. On MIPS-BE it instead is:
        //   lwl rx,K(ry)
        //   lwr rx,K+3(ry)
        public void FuseUnalignedLoads(Assignment assR)
        {
            var appR = MatchIntrinsicApplication(assR.Src, unalignedLoadsLe);
            if (appR == null)
                return;

            var regR = assR.Dst;
            var stmR = ssa.Identifiers[regR].DefStatement;

            var memR = appR.Item2.Arguments[1];
            var offR = GetOffsetOf(memR);

            var appL = appR.Item2.Arguments[0] as Application;
            Statement stmL = null;
            Assignment assL = null;
            if (appL == null)
            {
                var regL = (Identifier)appR.Item2.Arguments[0];
                stmL = ssa.Identifiers[regL].DefStatement;
                if (stmL == null)
                    return;
                assL = stmL.Instruction as Assignment;
                if (assL == null)
                    return;

                appL = assL.Src as Application;
            }
            var pairL = MatchIntrinsicApplication(appL, unalignedLoadsBe);
            if (pairL == null)
                return;

            var applL = pairL.Item2;
            var memL = appL.Arguments[1];
            var offL = GetOffsetOf(memL);

            Expression mem;
            if (offR + 3 == offL)
            {
                // Little endian use
                mem = memR;
            }
            else if (offL + 3 == offR)
            {
                // Big endian use
                mem = memL;
            }
            else
                return;

            ssa.RemoveUses(stmL);
            ssa.RemoveUses(stmR);
            if (assL != null)
            {
                assL.Src = appL.Arguments[0];
                ssa.AddUses(stmL);
            }
            assR.Src = mem;
            ssa.AddUses(stmR);
        }

        private void FuseUnalignedPairs(List<Tuple<UnalignedAccess, UnalignedAccess>> pairs)
        {
            foreach (var pair in pairs)
            {
                FuseStorePair(pair);
            }
        }

        private void FuseStorePair(Tuple<UnalignedAccess, UnalignedAccess> pair)
        {
            Statement stmL, stmR;
            if (pair.Item1.isLeft)
            {
                stmL = pair.Item1.stm;
                stmR = pair.Item2.stm;
            }
            else
            {
                stmL = pair.Item2.stm;
                stmR = pair.Item1.stm;
            }

            ssa.RemoveUses(stmL);
            ssa.RemoveUses(stmR);

            if (pair.Item1.mem is Identifier)
            {
                stmR.Instruction = new Assignment(
                    (Identifier)pair.Item1.mem,
                    pair.Item1.value);
            }
            else
            {
                var memId = ((MemoryAccess)((Store)pair.Item2.stm.Instruction).Dst).MemoryId;
                var sidMem = ssa.Identifiers[memId];
                sidMem.DefStatement = null;
                stmR.Instruction = new Store(
                    pair.Item1.mem,
                    pair.Item1.value);
            }
            stmL.Block.Statements.Remove(stmL);
            ssa.AddUses(stmR);
        }

        private List<Tuple<UnalignedAccess, UnalignedAccess>> GroupUnalignedStorePairs(Dictionary<Identifier, List<UnalignedAccess>> unalignedStores)
        {
            var pairs = new List<Tuple<UnalignedAccess, UnalignedAccess>>();
            foreach (var de in unalignedStores)
            {
                var sorted = de.Value.ToSortedList(k => k.offset);
                foreach (var se in sorted)
                {
                    UnalignedAccess other;
                    if (!sorted.TryGetValue(se.Key + 3, out other))
                        continue;
                    if (se.Value.isLeft == other.isLeft)
                        continue;

                    var pair = Tuple.Create(se.Value, other);
                    pairs.Add(pair);
                }
            }
            return pairs;
        }

        private readonly string[] unalignedIntrinsics =
        {
            PseudoProcedure.SwL,
            PseudoProcedure.SwR,
        };

        public class UnalignedAccess
        {
            public Statement stm;
            public Identifier reg;
            public int offset;
            public bool isLeft;
            public Expression value;
            public Expression mem;
        }

        private Dictionary<Identifier, List<UnalignedAccess>> FindAllUnalignedStoreInstructions(IEnumerable<Statement> stms)
        {
            var dict = new Dictionary<Identifier, List<UnalignedAccess>>();
            foreach (var stm in stms)
            {
                var store = stm.Instruction as Store;
                if (store == null)
                    continue;
                var tup = MatchIntrinsicApplication(store.Src, unalignedIntrinsics);
                if (tup == null)
                    continue;
                var appName = tup.Item1;
                var app = tup.Item2;
                var reg = GetRegisterOf(app.Arguments[0]);
                var offset = GetOffsetOf(app.Arguments[0]);
                var mem = GetModifiedMemory(stm.Instruction);
                var ua = new UnalignedAccess
                {
                    stm = stm,
                    reg = reg,
                    offset = offset,
                    isLeft = appName == PseudoProcedure.SwL,
                    value = app.Arguments[1],
                    mem = mem
                };
                List<UnalignedAccess> accesses;
                if (!dict.TryGetValue(reg, out accesses))
                {
                    accesses = new List<UnalignedAccess>();
                    dict.Add(reg, accesses);
                }
                accesses.Add(ua);
            }
            return dict;
        }

        private Expression GetModifiedMemory(Instruction instruction)
        {
            Assignment ass = instruction as Assignment;
            if (ass != null)
                return ass.Dst;
            else
                return ((Store)instruction).Dst;
        }

        private Identifier GetRegisterOf(Expression e)
        {
            MemoryAccess mem = e as MemoryAccess;
            if (mem != null)
            {
                return (Identifier)((BinaryExpression)mem.EffectiveAddress).Left;
            }
            else
            {
                return ssa.Procedure.Frame.FramePointer;
            }
        }

        private int GetOffsetOf(Expression e)
        {
            var id = e as Identifier;
            if (id != null)
            {
                var mem = id.Storage as StackStorage;
                return mem.StackOffset;
            }
            else
            {
                var mem = (MemoryAccess)e;
                var binL = mem.EffectiveAddress as BinaryExpression;
                if (binL != null)
                {
                    return ((Constant)binL.Right).ToInt32();
                }
                else
                {
                    return 0;
                }
            }
        }

        private Tuple<string, Application> MatchIntrinsicApplication(Expression e, string [] names)
        {
            var app = e as Application;
            if (app == null)
                return null;
            var pc = app.Procedure as ProcedureConstant;
            if (pc == null)
                return null;
            var ppp = pc.Procedure as PseudoProcedure;
            if (ppp == null)
                return null;
            if (!names.Contains(ppp.Name))
                return null;
            return Tuple.Create(ppp.Name, app);
        }
    }
}
