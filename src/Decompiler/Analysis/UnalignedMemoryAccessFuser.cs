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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis;

public class UnalignedMemoryAccessFuser : IAnalysis<SsaState>
{
    private readonly AnalysisContext context;

    public UnalignedMemoryAccessFuser(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "uma";

    public string Description => "Fuses unaligned memory accesses";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(ssa);
        var changed = worker.Transform();
        return (ssa, changed);
    }

    /// <summary>
    /// Fuses pairs of unaligned loads or stores.
    /// </summary>
    /// <remarks>
    /// This transform must run before value propagation, because 
    /// it depends on the availability of the source/target registers
    /// of the LWR/LWL and SWL/SWR instructions. Value propagation
    /// can wipe these out.
    /// </remarks>
    private class Worker : InstructionVisitorBase
    {
        private readonly SsaState ssa;
        private bool changed;

        public Worker(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public bool Transform()
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
            return changed;
        }

        public override void VisitAssignment(Assignment a)
        {
            base.VisitAssignment(a);
            FuseUnalignedLoads(a);
        }

        private static readonly string[] unalignedLoadsLe = new[]
        {
            IntrinsicProcedure.LwR
        };

        private static readonly string[] unalignedLoadsBe = new[]
        {
            IntrinsicProcedure.LwL
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
            if (appR is null)
                return;

            var regR = assR.Dst;
            var stmR = ssa.Identifiers[regR].DefStatement;

            var memR = appR.Value.Item2.Arguments[1];
            var offR = GetOffsetOf(memR);

            var appL = appR.Value.Item2.Arguments[0] as Application;
            Statement? stmL = null;
            Assignment? assL = null;
            if (appL is null)
            {
                var regL = (Identifier)appR.Value.Item2.Arguments[0];
                stmL = ssa.Identifiers[regL].DefStatement;
                if (stmL is null)
                    return;
                assL = stmL.Instruction as Assignment;
                if (assL is null)
                    return;

                appL = assL.Src as Application;
            }
            var pairL = MatchIntrinsicApplication(appL, unalignedLoadsBe);
            if (pairL is null)
                return;

            var memL = appL!.Arguments[1];
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
            if (assL is not null)
            {
                assL.Src = appL.Arguments[0];
                ssa.AddUses(stmL!);
            }
            assR.Src = mem;
            if (stmL is not null)
            {
                stmL.Block.Statements.Remove(stmL);
            }
            ssa.AddUses(stmR!);
            this.changed = true;
        }

        private void FuseUnalignedPairs(List<(UnalignedAccess, UnalignedAccess)> pairs)
        {
            foreach (var pair in pairs)
            {
                FuseStorePair(pair);
                changed = true;
            }
        }

        private void FuseStorePair((UnalignedAccess, UnalignedAccess) pair)
        {
            Statement stmL, stmR;
            if (pair.Item1.isLeft)
            {
                stmL = pair.Item1.stm!;
                stmR = pair.Item2.stm!;
            }
            else
            {
                stmL = pair.Item2.stm!;
                stmR = pair.Item1.stm!;
            }

            ssa.RemoveUses(stmL);
            ssa.RemoveUses(stmR);

            if (pair.Item1.mem is Identifier id)
            {
                stmR.Instruction = new Assignment(id, pair.Item1.value!);
            }
            else
            {
                var memId = ((MemoryAccess)((Store)pair.Item2.stm!.Instruction).Dst).MemoryId;
                var sidMem = ssa.Identifiers[memId];
                sidMem.DefStatement = null!;
                stmR.Instruction = new Store(
                    pair.Item1.mem!,
                    pair.Item1.value!);
            }
            stmL.Block.Statements.Remove(stmL);
            ssa.AddUses(stmR);
        }

        private static List<(UnalignedAccess, UnalignedAccess)> GroupUnalignedStorePairs(
            Dictionary<Identifier, List<UnalignedAccess>> unalignedStores)
        {
            var pairs = new List<(UnalignedAccess, UnalignedAccess)>();
            foreach (var de in unalignedStores)
            {
                var sorted = de.Value.ToSortedList(k => k.offset);
                foreach (var se in sorted)
                {
                    if (!sorted.TryGetValue(se.Key + 3, out UnalignedAccess? other))
                        continue;
                    if (se.Value.isLeft == other.isLeft)
                        continue;

                    var pair = (se.Value, other);
                    pairs.Add(pair);
                }
            }
            return pairs;
        }

        private readonly string[] unalignedIntrinsics =
        {
            IntrinsicProcedure.SwL,
            IntrinsicProcedure.SwR,
        };

        public class UnalignedAccess
        {
            public Statement? stm;
            public Identifier? reg;
            public int offset;
            public bool isLeft;
            public Expression? value;
            public Expression? mem;
        }

        private Dictionary<Identifier, List<UnalignedAccess>> FindAllUnalignedStoreInstructions(IEnumerable<Statement> stms)
        {
            var dict = new Dictionary<Identifier, List<UnalignedAccess>>();
            foreach (var stm in stms)
            {
                Expression src;
                if (stm.Instruction is Store store)
                {
                    src = store.Src;
                }
                else
                {
                    if (stm.Instruction is not Assignment ass)
                        continue;
                    src = ass.Src;
                }
                var tup = MatchIntrinsicApplication(src, unalignedIntrinsics);
                if (tup is null)
                    continue;
                var appName = tup.Value.Item1;
                var app = tup.Value.Item2;
                var reg = GetRegisterOf(app.Arguments[0]);
                var offset = GetOffsetOf(app.Arguments[0]);
                var mem = GetModifiedMemory(stm.Instruction);
                var ua = new UnalignedAccess
                {
                    stm = stm,
                    reg = reg,
                    offset = offset,
                    isLeft = appName == IntrinsicProcedure.SwL,
                    value = app.Arguments[1],
                    mem = mem
                };
                if (!dict.TryGetValue(reg, out var accesses))
                {
                    accesses = new List<UnalignedAccess>();
                    dict.Add(reg, accesses);
                }
                accesses.Add(ua);
            }
            return dict;
        }

        private static Expression GetModifiedMemory(Instruction instruction)
        {
            if (instruction is Assignment ass)
                return ass.Dst;
            else
                return ((Store)instruction).Dst;
        }

        private Identifier GetRegisterOf(Expression e)
        {
            if (e is MemoryAccess mem)
            {
                if (mem.EffectiveAddress is Identifier id)
                    return id;
                else
                    return (Identifier)((BinaryExpression)mem.EffectiveAddress).Left;
            }
            else
            {
                return ssa.Procedure.Frame.FramePointer;
            }
        }

        private static int GetOffsetOf(Expression e)
        {
            if (e is Identifier id)
            {
                if (id.Storage is RegisterStorage)
                {
                    return 0;
                }
                else
                {
                    var mem = (StackStorage) id.Storage;
                    return mem.StackOffset;
                }
            }
            else
            {
                var mem = (MemoryAccess)e;
                if (mem.EffectiveAddress is BinaryExpression binL)
                {
                    return ((Constant)binL.Right).ToInt32();
                }
                else
                {
                    return 0;
                }
            }
        }

        private static (string, Application)? MatchIntrinsicApplication(Expression? e, string [] names)
        {
            if (e is Application app &&
                app.Procedure is ProcedureConstant pc &&
                pc.Procedure is IntrinsicProcedure intrinsic &&
                names.Contains(intrinsic.Name))
            {
                return (intrinsic.Name, app);
            }
            else
            {
                return null;
            }
        }
    }
}
