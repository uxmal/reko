#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Analysis
{
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
    public class StoreFuser
    {
        private readonly SsaState ssa;

        public StoreFuser(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public void Transform()
        {
            var candidates = FindStoresOfSlicedIdentifiers();
            var storeClusters = GroupStoresByLocation(candidates);
        }

        private object GroupStoresByLocation(Dictionary<SsaIdentifier, Dictionary<Block, List<(BitRange, Statement, Expression)>>> candidates)
        {
            return null!;    //$TODO: make me work!
        }

        private Dictionary<SsaIdentifier, Dictionary<Block, List<(BitRange, Statement, Expression)>>> FindStoresOfSlicedIdentifiers()
        {
            var result =
                new Dictionary<SsaIdentifier, Dictionary<Block, List<(BitRange, Statement, Expression)>>>();
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                foreach (var stm in block.Statements)
                {
                    if (stm.Instruction is Store store)
                    {
                        var (sidSliced, bitrange) = FindSlicedIdentifier(store.Src);
                        if (sidSliced != null)
                        {
                            if (!result.TryGetValue(sidSliced, out var blocks))
                            {
                                blocks = new Dictionary<Block, List<(BitRange, Statement, Expression)>>();
                                result.Add(sidSliced, blocks);
                            }
                            if (!blocks.TryGetValue(stm.Block, out var stores))
                            {
                                stores = new List<(BitRange, Statement, Expression)>();
                                blocks.Add(stm.Block, stores);
                            }
                            stores.Add((bitrange, stm, store.Dst));
                        }
                    }
                }
            }
            return result;
        }

        private static void AddSlice(SsaIdentifier sidDef, BitRange range, Statement stm, Expression usingStore , Dictionary<SsaIdentifier, (BitRange, Statement, Expression)> result)
        {
            throw new NotImplementedException();
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
                if (sid.DefStatement != null && 
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