#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Collections;
using Reko.Core.Graphs;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class uses a provided interprocedural control flow graph to 
    /// detect the starts of procedures.
    /// </summary>
    /// <remarks>
    /// Inspired by "Compiler-Agnostic Function Detection in Binaries", by 
    /// Dennis Andriesse, Asia Slowinska, Herbert Bos.
    /// </remarks>
    public class ProcedureDetector
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ProcedureDetector), "");

        private readonly ScanResultsV2 sr;
        private readonly ScanResultsGraph srGraph;
        private readonly IDecompilerEventListener listener;
        private readonly HashSet<Address> procedures;
        private readonly IDictionary<Address, RtlBlock> mpAddrToBlock;

        public ProcedureDetector(ScanResultsV2 sr, IDecompilerEventListener listener)
        {
            this.sr = sr;
            this.srGraph = sr.ICFG;
            this.listener = listener;
            this.procedures = sr.Procedures.Keys.Concat(sr.SpeculativeProcedures.Keys).ToHashSet();
            DumpDuplicates(sr.Blocks.Values);
            this.mpAddrToBlock = sr.Blocks;
        }

        [Conditional("DEBUG")]
        public static void DumpDuplicates(IEnumerable<RtlBlock> blocks)
        {
            if (trace.Level != TraceLevel.Verbose)
                return;
            var q = from b in blocks
                    orderby b.Address
                    group b by b.Address into g
                    where g.Count() > 1
                    select new { g.Key, Count = g.Count() };
            Debug.WriteLine("ProcedureDetector.DumpDuplicates");
            foreach (var item in q)
            {
                Debug.Print("{0}: {1}", item.Key, item.Count);
            }
        }

        /// <summary>
        /// Master function to locate <see cref="Cluster"/>s of <see cref="RtlBlock"/> 
        /// from the ICFG passed in the ScanResults. These clusters are then refined
        /// to <see cref="Procedure"/>s ready for data flow analysis.
        /// </summary>
        /// <returns></returns>
        public List<RtlProcedure> DetectProcedures()
        {
            PreprocessIcfg();
            var clusters = FindClusters();
            return BuildProcedures(clusters);
        }

        private void PreprocessIcfg()
        {
            RemoveJumpsToKnownProcedures();
        }

        /// <summary>
        /// Remove any links between nodes where the destination is 
        /// a known call target.
        /// </summary>
        public void RemoveJumpsToKnownProcedures()
        {
            foreach (var calldest in this.procedures)
            {
                if (listener.IsCanceled())
                    break;
                var preds = sr.ICFG.Predecessors(calldest).ToList();
                foreach (var p in preds)
                {
                    srGraph.RemoveEdge(p, calldest);
                }
            }
        }

        /// <summary>
        /// A cluster is a proto-procedure. It consists of a set of entries
        /// and a set of blocks. 
        /// </summary>
        public class Cluster
        {
            public readonly SortedSet<RtlBlock> Blocks;
            public readonly SortedSet<RtlBlock> Entries;
            public readonly Dictionary<RtlBlock, List<RtlBlock>> Successors;

            public Cluster()
            {
                this.Entries = new SortedSet<RtlBlock>(Cmp.Instance);
                this.Blocks = new SortedSet<RtlBlock>(Cmp.Instance);
                this.Successors = new Dictionary<RtlBlock, List<RtlBlock>>();
            }

            public Cluster(IEnumerable<RtlBlock> entries, IEnumerable<RtlBlock> blocks)
            {
                this.Entries = new SortedSet<RtlBlock>(entries, Cmp.Instance);
                this.Blocks = new SortedSet<RtlBlock>(blocks, Cmp.Instance);
                this.Successors = new Dictionary<RtlBlock, List<RtlBlock>>();
            }

            private class Cmp : Comparer<RtlBlock>
            {
                public override int Compare(RtlBlock? x, RtlBlock? y)
                {
                    if (x is null)
                        return y is null ? 0 : -1;
                    if (y is null)
                        return 1;
                    return x.Address.CompareTo(y.Address);
                }

                public static readonly Cmp Instance = new();
            }

            [Conditional("DEBUG")]
            public void Dump(DirectedGraph<RtlBlock> icfg)
            {
                Debug.Print("Cluster with sources: [{0}]", string.Join(",",
                    from b in Blocks
                    where icfg.Predecessors(b).Count == 0
                    orderby b.Address
                    select b));
                foreach (var b in Blocks)
                {
                    var isEntry = Entries.Contains(b);
                    Debug.Print("{0}: {1}", b.Address, isEntry ? "*" : "");
                    Debug.Print("  pred: {0}",
                       string.Join(",", icfg.Predecessors(b)));

                    foreach (var c in b.Instructions)
                    {
                        //Debug.Print("  {0} - {1}", c.Address, c.Length);
                        foreach (var r in c.Instructions)
                        {
                            Debug.Print("    {0}", r);
                        }
                    }
                    Debug.Print("  succ: {0}",
                        string.Join(",", icfg.Successors(b)));
                }
                Debug.Print("Cluster with sinks: [{0}]", string.Join(",",
                    from b in Blocks
                    where icfg.Successors(b).Count == 0
                    orderby b.Address
                    select b));
                Debug.WriteLine("");
            }

            public override string ToString()
            {
                return $"{Blocks.OrderBy(b => b.Address.ToLinear()).FirstOrDefault()}: {Blocks.Count} blocks";
            }
        }

        /// <summary>
        /// Collects weakly connected components from the ICFG and gathers
        /// them into <see cref="Cluster"/>s.
        /// </summary>
        public List<Cluster> FindClusters()
        {
            var nodesLeft = new HashSet<RtlBlock>(sr.Blocks.Values);
            var clusters = new List<Cluster>();
            int totalCount = nodesLeft.Count;
            if (totalCount > 0)
            {
                listener.Progress.ShowProgress("Finding procedure candidates", 0, totalCount);
                var wl = WorkList.Create(nodesLeft.Select(n => n.Address));
                while (wl.TryGetWorkItem(out var node))
                {
                    if (listener.IsCanceled())
                        break;
                    var cluster = new Cluster();
                    clusters.Add(cluster);

                    BuildWCC(node, cluster, wl);
                    BreakOnWatchedAddress(cluster.Blocks.Select(b => b.Address));
                    listener.Progress.ShowProgress("Finding procedure candidates", totalCount - wl.Count, totalCount);
                }
            }
            return clusters;
        }

        [Conditional("DEBUG")]
        private void BreakOnWatchedAddress(IEnumerable<Address> enumerable)
        {
        }

        /// <summary>
        /// Build the weakly connected component for a cluster by following 
        /// both predecessors and successors in the graph. However, we never
        /// follow the predecessors of nodes that are marked directly called,
        /// and we never follow successors that are marked directly called
        /// (tail calls).
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="cluster"></param>
        /// <param name="unvisited"></param>
        private void BuildWCC(
            Address startNode,
            Cluster cluster,
            WorkList<Address> unvisited)
        {
            var queue = new Queue<Address>();
            unvisited.Add(startNode);
            queue.Enqueue(startNode);
            while (queue.TryDequeue(out var node))
            {
                if (!unvisited.Contains(node))
                    continue;
                unvisited.Remove(node);
                cluster.Blocks.Add(sr.Blocks[node]);
                if (sr.Successors.TryGetValue(node, out var succ))
                {
                    queue.EnqueueRange(succ
                        .Where(s => !procedures.Contains(s)));
                }
                if (!procedures.Contains(node) &&
                    sr.Predecessors.TryGetValue(node, out var preds))
                {
                    var pred = sr.Predecessors[node];
                    queue.EnqueueRange(pred);
                }
            }
        }

        /// <summary>
        /// For each of the given clusters, finds all the entries for the cluster 
        /// and tries to partition each cluster into procedures with single
        /// entries and exits.
        /// </summary>
        /// <param name="clusters"></param>
        private List<RtlProcedure> BuildProcedures(IList<Cluster> clusters)
        {
            var procs = new List<RtlProcedure>();
            if (clusters.Count == 0)
                return procs;
            listener.Progress.ShowProgress("Building procedures", 0, clusters.Count);
            foreach (var cluster in clusters)
            {
                //$PERF each cluster could be processed in parallel.
                if (listener.IsCanceled())
                    break;
                FuseLinearBlocks(cluster); 
                // cluster.Dump(sr.ICFG);
                if (FindClusterEntries(cluster))
                {
                    procs.AddRange(PostProcessCluster(cluster));
                }
                listener.Progress.Advance(1);
            }
            return procs;
        }

        /// <summary>
        /// As far as possible, try fusing consecutive linear blocks in the 
        /// cluster.
        /// </summary>
        /// <param name="cluster"></param>
        public void FuseLinearBlocks(Cluster cluster)
        {
            //$PERF: validate that this can be done w/o breaking other 
            // threads
            var wl = WorkList.Create(cluster.Blocks);
            while (wl.TryGetWorkItem(out var block))
            {
                if (!sr.Successors.TryGetValue(block.Address, out var succs) ||
                    succs.Count != 1)
                    continue;
                var succ = sr.Blocks[succs[0]];
                if (!sr.Predecessors.TryGetValue(succ.Address, out var preds) ||
                    preds.Count != 1)
                    continue;
                Debug.Assert(preds[0] == block.Address, "Inconsistent graph");
                if (block.Instructions.Last().Instructions.Last() is not RtlAssignment)
                    continue;

                // Move all instructions into predecessor.
                block.Instructions.AddRange(succ.Instructions);
                block.Length += succ.Length;
                block.FallThrough = succ.FallThrough;
                srGraph.RemoveEdge(block.Address, succ.Address);
                var succSuccs = sr.ICFG.Successors(succ.Address).ToList();
                foreach (var ss in succSuccs)
                {
                    srGraph.RemoveEdge(succ.Address, ss);
                    srGraph.AddEdge(block.Address, ss);
                }
                cluster.Blocks.Remove(sr.Blocks[succ.Address]);
                // May be more blocks.
                wl.Add(block);
            }
        }

        /// <summary>
        /// For a given cluster, find the probable entries.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="cluster"></param>
        public bool FindClusterEntries(Cluster cluster)
        {
            var nopreds = new List<RtlBlock>();
            foreach (var block in cluster.Blocks)
            {
                if (procedures.Contains(block.Address))
                {
                    cluster.Entries.Add(block);
                }
                if (!sr.Predecessors.TryGetValue(block.Address, out var preds) ||
                    preds.Count == 0)
                {
                    nopreds.Add(block);
                }
            }

            //$REVIEW: the heuristic of returning the nodes with zero predecessor
            // yields a lot of false positives. Consider using "possible pointers"
            // as a discriminator.
            // If one or more nodes were the destination of a direct call,
            // use those as entries.
            if (cluster.Entries.Count > 0)
                return true;

            // If a single node has 0 predecessors, make it the entry.
            if (nopreds.Count == 1)
            {
                // This is disabled as we get a lot of false positives.
                // If we can generate a cross reference lookup then perhaps
                // this will improve.

                //cluster.Entries.UnionWith(nopreds);
                //return true;
                return false;
            }

            /*
            // Otherwise, if one or more nodes has zero predecessors, pick it.
            if (nopreds.Count > 0)
            {
                cluster.Entries.UnionWith(nopreds);
                return;
            }

            // If we can't find another possibility, return the node with the
            // lowest address.
            cluster.Entries.Add(cluster.Blocks.OrderBy(b => b.Address).First());
             */
            return false;
        }

        /// <summary>
        /// Processes a cluster into 1..n procedures.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public List<RtlProcedure> PostProcessCluster(Cluster cluster)
        {
            var entries = cluster.Entries;

            // Remove all nodes with no predecessors which haven't been marked as entries.
            var deadNodes = cluster.Blocks
                .Where(b => !entries.Contains(b) && 
                            (!sr.Predecessors.TryGetValue(b.Address, out var preds) ||
                             preds.Count == 0))
                .ToHashSet();
            cluster.Blocks.ExceptWith(deadNodes);
            if (cluster.Blocks.Count == 0 || entries.Count == 0)
            {
                //$TODO: investigate why this happens.
                return new List<RtlProcedure>();
            }

            // Join blocks which have a single successor / single predecessor
            // relationship.

            // If the cluster has more than one entry, we have to try to pick it apart.
            if (entries.Count > 1)
            {
                Debug.Print("Entries {0} share common code", string.Join(",", entries.Select(e => e.Name)));
                return PartitionIntoSubclusters(cluster);
            }
            else
            {
                var entryBlock = cluster.Entries.First();
                var name = NamingPolicy.Instance.ProcedureName(entryBlock.Address); //$TODO: what about user procs, symbols? Needs a method in `Program`.
                var rtlProc = new RtlProcedure(entryBlock.Architecture, entryBlock.Address, name, entryBlock.Provenance, cluster.Blocks);
                return new List<RtlProcedure> { rtlProc };
            }
        }

        /// <summary>
        /// Splits a multiple entry cluster into separate sub-clusters by 
        /// partitioning all the blocks into subsets where each subset is 
        /// dominated by one of the original entries. 
        /// </summary>
        /// <remarks>
        /// Many binaries contain cross-procedure jumps. If the target of 
        /// those jumps is a single block with no successors, it is very
        /// likely an instance of a "shared exit node" pattern that many
        /// compilers+linkers emit. We handle that case separately.
        /// </remarks>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public List<RtlProcedure> PartitionIntoSubclusters(Cluster cluster)
        {
            // Create a fake node that will serve as the parent of all the 
            // existing entries. That node will be used to compute all
            // immediate dominators of all reachable blocks.
            var auxNode = RtlBlock.CreateEmpty(null!, Address.Ptr64(~0ul), "<root>");

            var srGraph = MakeGraph(cluster, auxNode);
            var idoms = LTDominatorGraph<RtlBlock>.Create(srGraph, auxNode);
            // DumpDominatorTrees(idoms);

            // Find all nodes whose immediate dominator is "<root>". 
            // Those are the entries to new clusters and may contain blocks
            // that are shared between procedures in the source program.
            var newEntries = cluster.Blocks.Where(b => idoms[b] == auxNode).ToList();
            var dominatedEntries = newEntries.ToDictionary(k => k, v => new HashSet<RtlBlock> { v });

            // Partition the nodes in the cluster into categories depending on which
            // one of the newEntries they are dominated by.
            foreach (var b in cluster.Blocks)
            {
                if (dominatedEntries.ContainsKey(b))
                    continue; // already there.
                var n = b;
                for (; ; )
                {
                    var i = idoms[n];
                    if (i is null)
                        break;
                    if (dominatedEntries.ContainsKey(i))
                    {
                        // If my idom is already in the set, add me too.
                        dominatedEntries[i].Add(b);
                        break;
                    }
                    n = i;
                }
            }

            // Now remove the fake node 
            srGraph.AdjacencyList.Remove(auxNode);

            // Handle the special case with new entries that weren't there before,
            // and only consist of a linear sequence of blocks. Mark such nodes as "shared".
            // Later stages will copy these nodes into their respective procedures.
            foreach (var newEntry in dominatedEntries.Keys
                .Where(e => !cluster.Entries.Contains(e)).ToList())
            {
                if (srGraph.Successors(newEntry).Count == 0)
                {
                    newEntry.IsSharedExitBlock = true;
                    dominatedEntries.Remove(newEntry);
                }
            }

            return dominatedEntries
                .OrderBy(e => e.Key.Address)
                .Select(e => new RtlProcedure(
                    e.Key.Architecture,
                    e.Key.Address,
                    e.Key.Name,
                    e.Key.Provenance,
                    e.Value))
                .ToList();
        }

        private AdjacencyListGraph<RtlBlock> MakeGraph(Cluster cluster, RtlBlock auxNode)
        {
            var adj = cluster.Blocks.ToDictionary(k => k, v => new List<RtlBlock>(2));
            var allEntries =
                cluster.Entries.Concat(
                cluster.Blocks
                    .Where(b => srGraph.Predecessors(b.Address).Count == 0))
                .Distinct()
                .OrderBy(b => b.Address)
                .ToList();
            adj.Add(auxNode, allEntries);

            var visited = new HashSet<RtlBlock>();
            var stack = new Stack<RtlBlock>();
            foreach (var n in adj.Keys)
            {
                if (visited.Contains(n))
                    continue;
                visited.Add(n);
                if (sr.Successors.TryGetValue(n.Address, out var succs))
                {
                    adj[n].AddRange(succs.Select(s => sr.Blocks[s]));
                }
                foreach (var s in adj[n])
                {
                    stack.Push(s);
                }
            }
            return new AdjacencyListGraph<RtlBlock>(adj);
        }

        private void DumpDominatorTrees(Dictionary<RtlBlock, RtlBlock> idoms)
        {
            var roots = new SortedSet<Address>();
            var tree = new SortedList<Address, SortedSet<Address>>();
            foreach (var de in idoms)
            {
                if (de.Value is null)
                    roots.Add(de.Key.Address);
                else
                {
                    if (!tree.TryGetValue(de.Value.Address, out var kids))
                    {
                        kids = new SortedSet<Address>();
                        tree.Add(de.Value.Address, kids);
                    }
                    kids.Add(de.Key.Address);
                }
            }
            foreach (var root in roots)
            {
                Debug.Print("== {0} =======", root);
                DumpDominatorTree(root, tree, "");
            }
        }

        private void DumpDominatorTree(Address node, SortedList<Address, SortedSet<Address>> tree, string sIndent)
        {
            Debug.Print("{0}+ {1}", sIndent, node);
            if (tree.TryGetValue(node, out var kids))
            {
                sIndent += "  ";
                foreach (var kid in kids)
                {
                    DumpDominatorTree(kid, tree, sIndent);
                }
            }
        }

        [Conditional("DEBUG")]
        private static void DumpDomGraph(IEnumerable<RtlBlock> nodes, Dictionary<RtlBlock, RtlBlock> domGraph)
        {
            var q =
                from n in nodes
                join de in domGraph on n equals de.Value into des
                from de in des.DefaultIfEmpty()
                orderby n.Name, de.Key is not null ? de.Key.Name : ""
                select new { n.Name, Kid = de.Key is not null ? de.Key.Name : "*" };
            foreach (var item in q)
            {
                Debug.Print("{0}: {1}", item.Name, item.Kid);
            }
        }

        private void FuseBlocks(Cluster cluster)
        {
            foreach (var block in Enumerable.Reverse(cluster.Blocks).ToList())
            {
                var succs = srGraph.Successors(block.Address);
                if (succs.Count == 1)
                {
                    var s = succs.First();
                    var sBlock = sr.Blocks[s];
                    var preds = srGraph.Predecessors(s);
                    if (preds.Count != 1 || preds.First() != block.Address)
                        continue;

                    if (block.FallThrough != s)
                        continue;
                    var ss = srGraph.Successors(s).ToList();
                    srGraph.RemoveEdge(block.Address, s);
                    block.Instructions.AddRange(sBlock.Instructions);
                    srGraph.Nodes.Remove(s);
                    cluster.Blocks.Remove(sBlock);
                    foreach (var n in ss)
                    {
                        srGraph.AddEdge(block.Address, n);
                    }
                    Debug.Print("Fused {0} {1}", block.Address, s);
                }
            }
        }

        [Conditional("DEBUG")]
        private void DumpClusters(List<Cluster> clusters, ScanResultsV2 sr)
        {
            // Sort clusters by their earliest address
            foreach (var cc in
                (from c in clusters
                 let min = c.Blocks.Min(b => b.Address)
                 orderby min
                 select c))
            {
                DumpCluster(cc, sr);
            }
        }

        [Conditional("DEBUG")]
        private void DumpCluster(Cluster cc, ScanResultsV2 sr)
        {
            Debug.Print("-- Cluster -----------------------------");
            Debug.Print("{0} nodes", cc.Blocks.Count);
            foreach (var block in cc.Blocks.OrderBy(n => n.Address))
            {
                var addrEnd = block.GetEndAddress();
                if (sr.Procedures.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: known procedure ----------", block.Address);
                }
                else if (sr.SpeculativeProcedures.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: possible procedure, called {1} time(s) ----------",
                        block.Address,
                        sr.SpeculativeProcedures[block.Address]);
                }
                Debug.Print("{0}:  //  pred: {1}",
                    block.Name,
                    string.Join(" ", srGraph.Predecessors(block.Address)
                        .OrderBy(n => n)));
                foreach (var instr in block.Instructions.SelectMany(c => c.Instructions))
                {
                    Debug.Print("    {0}", instr);
                }
                Debug.Print("  // succ: {0}", string.Join(" ", srGraph.Successors(block.Address)
                    .OrderBy(n => n)));
            }
            Debug.Print("");
        }
    }
}
