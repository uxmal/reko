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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// Fuses stores.
/// </summary>
/// <remarks>
/// This optimistic analysis locates identifiers that are being sliced 
/// and then stored:
///     id_1 = ....
///     Store(addr, SLICE(id_1)
///     Store(addr+2, SLICE(id_1)
/// or 
///     id_1 = ....
///     id_2 = SLICE(id_1)
///     id_3 = SLICE(id_1)
///     Store(addr, id_2)
///     Store(addr+2, id_3)
/// <para>
/// This transform is a little dangerous because there could be statements between the Stores
/// that mutate memory. To reduce the risk of this happening, we assume that both stores are in 
/// the same basic block.
/// </para>
/// </remarks>
public class StoreFuser : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new TraceSwitch(nameof(StoreFuser), "Store fuser") { Level = TraceLevel.Verbose };

    private readonly AnalysisContext context;

    public StoreFuser(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "stfu";

    public string Description => "Fuses stores to adjacent memory locations";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var worker = new Worker(ssa, context.EventListener);
        var changed = worker.Transform();
        return (ssa, changed);
    }

    private class Worker
    {

        private readonly SsaState ssa;
        private readonly IProcessorArchitecture arch;
        private readonly ExpressionEmitter m;
        private readonly IEventListener listener;
        private bool changed;

        private record Candidate(
            BitRange Slice, 
            Statement Statement, 
            Identifier MemoryId,
            Expression? Segment, 
            Expression? EffectiveAddress, 
            long Displacement);

        public Worker(SsaState ssa, IEventListener listener)
        {
            this.ssa = ssa;
            this.arch = ssa.Procedure.Architecture;
            this.m = new ExpressionEmitter();
            this.listener = listener;
        }

        public bool Transform()
        {
            var candidates = FindStoresOfSlicedIdentifiers();
            ReplaceStores(candidates);
            return changed;
        }

        private void ReplaceStores(Dictionary<(SsaIdentifier, Block, Expression?), List<Candidate>> candidates)
        {
            foreach (var ((sid, block, ea), stores) in candidates)
            {
                if (listener.IsCanceled())
                    return;
                ReplaceStore(sid, block, ea, stores);
            }
        }

        private void ReplaceStore(SsaIdentifier sid, Block block, Expression? ea, List<Candidate> stores)
        {
            trace.Verbose("{0} stored in block {1}: {2}", sid.Identifier.Name, block.Id, string.Join(",", stores));
            var sortedStores = stores.OrderBy(c => c.Slice.Lsb).ToList();
            if (!AllSlicesAdjacent(sortedStores))
                return;
            if (!SlicesCoverVariable(sid.Identifier, stores))
                return;
            sortedStores.Sort((a, b) => a.Displacement.CompareTo(b.Displacement));
            if (!AllStoresAdjacent(sortedStores))
                return;

            var lastStore = sortedStores.MaxBy(c => c.Statement.Address.Offset);
            foreach (var store in stores)
            {
                if (store != lastStore)
                {
                    ssa.DeleteStatement(store.Statement);
                }
                else
                {
                    ssa.RemoveUses(store.Statement);
                    var newEa = MakeEffectiveAddress(sid, store.Statement.Instruction, ea, sortedStores);
                    store.Statement.Instruction = new Store(
                        m.Mem(lastStore.MemoryId, sid.Identifier.DataType, newEa),
                        sid.Identifier);
                    ssa.AddUses(store.Statement);
                }
                changed = true;
            }
        }

        private Expression MakeEffectiveAddress(SsaIdentifier sid, Instruction instr, Expression? ea, List<Candidate> stores)
        {
            var store = (Store) instr;
            var minStore = stores.MinBy(c => c.Displacement)!;
            if (ea is null)
            {
                if (minStore.Segment is null)
                {
                    return ssa.Procedure.Architecture.MakeAddressFromConstant(
                        Constant.Create(sid.Identifier.DataType, minStore.Displacement),
                        false);
                }
                else
                {
                    var offset = Constant.Word16((ushort) minStore.Displacement);
                    return m.SegPtr(minStore.Segment, offset);
                }
            }
            else
            {
                if (minStore.Segment is null)
                {
                    return m.AddSubSignedInt(ea, minStore.Displacement);
                }
                else
                {
                    return m.SegPtr(minStore.Segment, m.AddSubSignedInt(ea, minStore.Displacement));
                }
            }
        }

        private bool SlicesCoverVariable(Identifier identifier, List<Candidate> stores)
        {
            if (stores[0].Slice.Lsb != 0)
                return false;
            if (stores[^1].Slice.Msb != identifier.DataType.BitSize)
                return false;
            return true;
        }

        private bool AllSlicesAdjacent(List<Candidate> store)
        {
            var cPrev = store[0];
            foreach (var c in store.Skip(1))
            {
                if (cPrev.Slice.Msb != c.Slice.Lsb)
                    return false;
                cPrev = c;
            }
            return true;
        }

        private bool AllStoresAdjacent(List<Candidate> store)
        {
            var cPrev = store[0];
            var unitSize = arch.MemoryGranularity;
            foreach (var c in store.Skip(1))
            {
                if (cPrev.Displacement * unitSize + c.Slice.Extent != c.Displacement * unitSize)
                    return false;
                cPrev = c;
            }
            return true;
        }

        private Dictionary<(SsaIdentifier, Block, Expression?), List<Candidate>> FindStoresOfSlicedIdentifiers()
        {
            var result =
                new Dictionary<(SsaIdentifier, Block, Expression?), List<Candidate>>(
                    new EaComparer());
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                foreach (var stm in block.Statements)
                {
                    if (listener.IsCanceled())
                        return result;
                    if (stm.Instruction is Store store)
                    {
                        var (sidSliced, bitrange) = FindSlicedIdentifier(store.Src);
                        if (sidSliced is not null)
                        {
                            var mem = (MemoryAccess) store.Dst;
                            var (seg, ea, displacement) = mem.Unpack();
                            if (!result.TryGetValue((sidSliced, stm.Block, ea), out var stores))
                            {
                                stores = new List<Candidate>();
                                result.Add((sidSliced, stm.Block, ea), stores);
                            }
                            stores.Add(new Candidate(bitrange, stm, mem.MemoryId, seg, ea, displacement));
                        }
                    }
                }
            }
            return result;
        }

        private class EaComparer : IEqualityComparer<(SsaIdentifier, Block, Expression?)>
        {
            private ExpressionValueComparer comparer = new ExpressionValueComparer();
            public bool Equals((SsaIdentifier, Block, Expression?) x, (SsaIdentifier, Block, Expression?) y)
            {
                if (x.Item1 != y.Item1)
                    return false;
                if (x.Item2 != y.Item2)
                    return false;
                if (x.Item3 is null)
                    return y.Item3 is null;
                if (y.Item3 is null)
                    return false;
                return comparer.Equals(x.Item3, y.Item3);
            }

            public int GetHashCode([DisallowNull] (SsaIdentifier, Block, Expression?) obj)
            {
                int hashEa = obj.Item3 is null
                    ? 0
                    : comparer.GetHashCode(obj.Item3);
                return HashCode.Combine(obj.Item1, obj.Item2, hashEa);
            }
        }

        private (SsaIdentifier?, BitRange) FindSlicedIdentifier(Expression src)
        {
            if (src is Slice slice) 
            {
                if (slice.Expression is Identifier idSliced)
                {
                    return (ssa.Identifiers[idSliced], new BitRange(slice.Offset, slice.DataType.BitSize));
                }
            } else if (src is Identifier id)
            {
                var sid = ssa.Identifiers[id];
                if (sid.DefStatement is not null && 
                    sid.DefStatement.Instruction is AliasAssignment ass &&
                    ass.Src is Slice sliced &&
                    sliced.Expression is Identifier idSliced)
                {
                    return (ssa.Identifiers[idSliced], new BitRange(sliced.Offset, sliced.Offset + sliced.DataType.BitSize));
                }
            }
            return (default, default);
        }
    }
}