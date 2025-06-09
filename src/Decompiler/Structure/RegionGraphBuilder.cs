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
using Reko.Core.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Structure
{
    public class RegionGraphBuilder
    {
        private readonly Procedure proc;
        private readonly Dictionary<Block, Region> btor;
        private readonly DiGraph<Region> graph;
        private readonly RegionFactory regionFactory;
        private readonly Dictionary<Block, Dictionary<Block, Region>> switchPads;

        public RegionGraphBuilder(Procedure proc)
        {
            this.proc = proc;
            this.btor = new Dictionary<Block, Region>();
            this.graph = new DiGraph<Region>();
            this.regionFactory = new RegionFactory();
            this.switchPads = new Dictionary<Block, Dictionary<Block, Region>>();
        }

        public (DirectedGraph<Region>, Region) Build()
        {
            foreach (var b in proc.ControlGraph.Blocks)
            {
                if (b.Pred.Count == 0 && b != proc.EntryBlock ||
                    b == proc.ExitBlock)
                    continue;
                var reg = regionFactory.Create(b);
                btor.Add(b, reg);
                graph.AddNode(reg);
            }

            foreach (var b in proc.ControlGraph.Blocks)
            {
                if (btor.TryGetValue(b, out var reg) &&
                    reg.Type == RegionType.IncSwitch)
                {
                    MakeSwitchPads(b);
                }
            }

            foreach (var b in proc.ControlGraph.Blocks)
            {
                if (b.Pred.Count == 0 && b != proc.EntryBlock)
                    continue;
                btor.TryGetValue(b, out var from);
                foreach (var s in b.Succ)
                {
                    if (s == proc.ExitBlock)
                        continue;
                    var to = Destination(b, s);
                    graph.AddEdge(from!, to);
                }
                if (from is not null)
                {
                    if (graph.Successors(from).Count == 0)
                        from.Type = RegionType.Tail;
                }
            }

            foreach (var reg in graph.Nodes.ToList())
            {
                if (graph.Predecessors(reg).Count == 0 && reg != btor[proc.EntryBlock])
                {
                    graph.Nodes.Remove(reg);
                }
            }
            return (graph, btor[proc.EntryBlock]);
        }

        private Region Destination(Block b, Block s)
        {
            if (switchPads.TryGetValue(b, out var swp))
            {
                if (swp.TryGetValue(s, out var pad))
                    return pad;
            }
            return btor[s];
        }

        /// <summary>
        /// Introduce switch pads for all case regions that have predecessors
        /// which are switch blocks different from the current switch block.
        /// </summary>
        private void MakeSwitchPads(Block switchBlock)
        {
            foreach (var caseBlock in switchBlock.Succ.Distinct())
            {
                var caseReg = btor[caseBlock];
                var switchPredecessors = caseBlock.Pred
                    .Where(p => p != switchBlock)
                    .ToArray();
                if (switchPredecessors.Length > 0)
                {
                    if (!switchPads.TryGetValue(switchBlock, out var sws))
                    {
                        sws = new Dictionary<Block, Region>();
                        switchPads.Add(switchBlock, sws);
                    }
                    if (!sws.TryGetValue(caseBlock, out var pad))
                    {
                        pad = new Region(caseBlock) { IsSwitchPad = true };
                        sws.Add(caseBlock, pad);
                        graph.AddNode(pad);
                        graph.AddEdge(pad, caseReg);
                    }
                }
            }
        }
    }
}
