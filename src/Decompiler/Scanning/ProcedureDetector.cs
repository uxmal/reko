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
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        private readonly Program program;
        private readonly ScanResults sr;
        private readonly DecompilerEventListener listener;
        private readonly HashSet<Address> procedures;
        private readonly Dictionary<Address, RtlBlock> mpAddrToBlock;

        public ProcedureDetector(Program program, ScanResults sr, DecompilerEventListener listener)
        {
            this.program = program;
            this.sr = sr;
            this.listener = listener;
            this.procedures = sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys).ToHashSet();
            DumpDuplicates(sr.ICFG.Nodes);
            this.mpAddrToBlock = sr.ICFG.Nodes.ToDictionary(de => de.Address);
        }

        [Conditional("DEBUG")]
        public void DumpDuplicates(IEnumerable<RtlBlock> blocks)
        {
            var q = from b in blocks
                    orderby b.Address
                    group b by b.Address into g
                    where g.Count() > 1
                    select new { g.Key, Count = g.Count() };
            foreach (var item in q)
            {
                Debug.Print("{0}: {1}", item.Key, item.Count);
            }
        }

        /// <summary>
        /// Master function to locate "Clusters" of RtlBlocks from the ICFG
        /// passed in the ScanResults.
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
            ProcessIndirectJumps();
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
                if (!mpAddrToBlock.TryGetValue(calldest, out var node))
                    continue;
                var preds = sr.ICFG.Predecessors(node).ToList();
                foreach (var p in preds)
                {
                    sr.ICFG.RemoveEdge(p, node);
                }
            }
        }

        private void ProcessIndirectJumps()
        {
            foreach (var address in sr.IndirectJumps)
            {
                /*
                if (!mpAddrToBlock.TryGetValue(address, out var rtlBlock))
                    continue;
                var host = new BackwardSlicerHost(this.program);
                var bws = new BackwardSlicer(host, rtlBlock, program.Architecture.CreateProcessorState());
                var te = bws.DiscoverTableExtent(address, (RtlTransfer)rtlBlock.Instructions.Last().Instructions.Last(), listener);
                */
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

            public Cluster()
            {
                this.Entries = new SortedSet<RtlBlock>(Cmp.Instance);
                this.Blocks = new SortedSet<RtlBlock>(Cmp.Instance);
            }

            public Cluster(IEnumerable<RtlBlock> entries, IEnumerable<RtlBlock> blocks)
            {
                this.Entries = new SortedSet<RtlBlock>(entries, Cmp.Instance);
                this.Blocks = new SortedSet<RtlBlock>(blocks, Cmp.Instance);
            }

            private class Cmp : Comparer<RtlBlock>
            {
                public override int Compare(RtlBlock x, RtlBlock y)
                {
                    return x.Address.CompareTo(y.Address);
                }

                public static readonly Cmp Instance = new Cmp();
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
        /// them into Clusters.
        /// </summary>
        public List<Cluster> FindClusters()
        {
            var nodesLeft = new HashSet<RtlBlock>(sr.ICFG.Nodes);
            var clusters = new List<Cluster>();
            int totalCount = nodesLeft.Count;
            if (totalCount > 0)
            {
                listener.ShowProgress("Finding procedure candidates", 0, totalCount);
                var wl = new WorkList<RtlBlock>(nodesLeft);
                while (wl.GetWorkItem(out var node))
                {
                    if (listener.IsCanceled())
                        break;
                    var cluster = new Cluster();
                    clusters.Add(cluster);

                    BuildWCC(node, cluster, wl);
                    sr.BreakOnWatchedAddress(cluster.Blocks.Select(b => b.Address));
                    listener.ShowProgress("Finding procedure candidates", totalCount - nodesLeft.Count, totalCount);
                }
            }
            return clusters;
        }

        /// <summary>
        /// Build the weakly connected component for a cluster by following 
        /// both predecessors and successors in the graph. However, we never
        /// follow the predecessors of nodes that are marked directly called,
        /// and we never follow successors that are marked directly called
        /// (tail calls).
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cluster"></param>
        /// <param name="wl"></param>
        private void BuildWCC(
            RtlBlock node,
            Cluster cluster,
            WorkList<RtlBlock> wl)
        {
            wl.Remove(node);
            cluster.Blocks.Add(node);
            foreach (var s in sr.ICFG.Successors(node))
            {
                if (wl.Contains(s))
                {
                    // Only add if successor is not CALLed.
                    if (!procedures.Contains(s.Address))
                    {
                        BuildWCC(s, cluster, wl);
                    }
                }
            }
            if (!procedures.Contains(node.Address))
            {
                // Only backtrack through predecessors if the node
                // is not CALLed.
                foreach (var p in sr.ICFG.Predecessors(node))
                {
                    if (wl.Contains(p))
                    {
                        BuildWCC(p, cluster, wl);
                    }
                }
            }
        }

        /// <summary>
        /// Given a set of clusters, finds all the entries for each cluster 
        /// and tries to partition each cluster into procedures with single
        /// entries and exits.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="clusters"></param>
        private List<RtlProcedure> BuildProcedures(IList<Cluster> clusters)
        {
            var procs = new List<RtlProcedure>();
            if (clusters.Count == 0)
                return procs;
            listener.ShowProgress("Building procedures", 0, clusters.Count);
            foreach (var cluster in clusters)
            {
                if (listener.IsCanceled())
                    break;
                FuseLinearBlocks(cluster);
                // cluster.Dump(sr.ICFG);
                if (FindClusterEntries(cluster))
                {
                    procs.AddRange(PostProcessCluster(cluster));
                }
                listener.Advance(1);
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
            var wl = new WorkList<RtlBlock>(cluster.Blocks);
            while (wl.GetWorkItem(out var block))
            {
                if (sr.ICFG.Successors(block).Count != 1)
                    continue;
                var succ = sr.ICFG.Successors(block).First();
                if (sr.ICFG.Predecessors(succ).Count != 1)
                    continue;
                Debug.Assert(sr.ICFG.Predecessors(succ).First() == block, "Inconsistent graph");
                if (!(block.Instructions.Last().Instructions.Last() is RtlAssignment))
                    continue;

                // Move all instructions into predecessor.
                block.Instructions.AddRange(succ.Instructions);
                sr.ICFG.RemoveEdge(block, succ);
                var succSuccs = sr.ICFG.Successors(succ).ToList();
                foreach (var ss in succSuccs)
                {
                    sr.ICFG.RemoveEdge(succ, ss);
                    sr.ICFG.AddEdge(block, ss);
                }
                cluster.Blocks.Remove(succ);
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
                if (sr.ICFG.Predecessors(block).Count == 0)
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
                .Where(b => !entries.Contains(b) && sr.ICFG.Predecessors(b).Count == 0)
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
                return new List<RtlProcedure> { new RtlProcedure(cluster.Entries.First(), cluster.Blocks) };
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
            var auxNode = new RtlBlock(null, "<root>");
            sr.ICFG.AddNode(auxNode);
            var allEntries =
                cluster.Entries.Concat(
                cluster.Blocks
                    .Where(b => sr.ICFG.Predecessors(b).Count == 0))
                .Distinct()
                .OrderBy(b => b.Address)
                .ToList();
            foreach (var entry in allEntries)
            {
                sr.ICFG.AddEdge(auxNode, entry);
            }
            var idoms = LTDominatorGraph<RtlBlock>.Create(sr.ICFG, auxNode);
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
                for (;;)
                {
                    var i = idoms[n];
                    if (i == null)
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
            sr.ICFG.RemoveNode(auxNode);

            // Handle the special case with new entries that weren't there before,
            // and only consist of a linear sequence of blocks. Mark such nodes as "shared".
            // Later stages will copy these nodes into their respective procedures.
            foreach (var newEntry in dominatedEntries.Keys
                .Where(e => !cluster.Entries.Contains(e)).ToList())
            {
                if (sr.ICFG.Successors(newEntry).Count == 0)
                {
                    newEntry.IsSharedExitBlock = true;
                    dominatedEntries.Remove(newEntry);
                }
            }

            return dominatedEntries
                .OrderBy(e => e.Key.Address)
                .Select(e => new RtlProcedure(e.Key, e.Value))
                .ToList();
        }

        private void DumpDominatorTrees(Dictionary<RtlBlock, RtlBlock> idoms)
        {
            var roots = new SortedSet<Address>();
            var tree = new SortedList<Address, SortedSet<Address>>();
            foreach (var de in idoms)
            {
                if (de.Key.Address == null)
                    continue;
                if (de.Value == null || de.Value.Address == null)
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
                sIndent = sIndent + "  ";
                foreach (var kid in kids)
                {
                    DumpDominatorTree(kid, tree, sIndent);
                }
            }
        }

        /// <summary>
        /// Starting at <paramref name="start"/> 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<RtlBlock> LinearSequence(RtlBlock start)
        {
            return new List<RtlBlock>();
        }

        [Conditional("DEBUG")]
        private void DumpDomGraph(IEnumerable<RtlBlock> nodes, Dictionary<RtlBlock, RtlBlock> domGraph)
        {
            var q =
                from n in nodes
                join de in domGraph on n equals de.Value into des
                from de in des.DefaultIfEmpty()
                orderby n.Name, de.Key != null ? de.Key.Name : ""
                select new { n.Name, Kid = de.Key != null ? de.Key.Name : "*" };
            foreach (var item in q)
            {
                Debug.Print("{0}: {1}", item.Name, item.Kid);
            }
        }

        private void FuseBlocks(Cluster cluster)
        {
            foreach (var block in Enumerable.Reverse(cluster.Blocks).ToList())
            {
                var succs = sr.ICFG.Successors(block);
                if (succs.Count == 1)
                {
                    var s = succs.First();
                    var preds = sr.ICFG.Predecessors(s);
                    if (preds.Count != 1 || preds.First() != block)
                        continue;

                    if (block.GetEndAddress() != s.Address)
                        continue;
                    var ss = sr.ICFG.Successors(s).ToList();
                    sr.ICFG.RemoveEdge(block, s);
                    block.Instructions.AddRange(s.Instructions);
                    sr.ICFG.RemoveNode(s);
                    cluster.Blocks.Remove(s);
                    foreach (var n in ss)
                    {
                        sr.ICFG.AddEdge(block, n);
                    }
                    Debug.Print("Fused {0} {1}", block.Address, s.Address);
                }
            }
        }

        [Conditional("DEBUG")]
        private void DumpClusters(List<Cluster> clusters, ScanResults sr)
        {
            var ICFG = sr.ICFG;
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
        private void DumpCluster(Cluster cc, ScanResults sr)
        {
            Debug.Print("-- Cluster -----------------------------");
            Debug.Print("{0} nodes", cc.Blocks.Count);
            foreach (var block in cc.Blocks.OrderBy(n => n.Address))
            {
                var addrEnd = block.GetEndAddress();
                if (sr.KnownProcedures.Contains(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: known procedure ----------", block.Address);
                }
                else if (sr.DirectlyCalledAddresses.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: possible procedure, called {1} time(s) ----------",
                        block.Address,
                        sr.DirectlyCalledAddresses[block.Address]);
                }
                Debug.Print("{0}:  //  pred: {1}",
                    block.Name,
                    string.Join(" ", sr.ICFG.Predecessors(block)
                        .OrderBy(n => n.Address)
                        .Select(n => n.Address)));
                foreach (var instr in block.Instructions.SelectMany(c => c.Instructions))
                {
                    Debug.Print("    {0}", instr);
                }
                Debug.Print("  // succ: {0}", string.Join(" ",sr.ICFG.Successors(block)
                    .OrderBy(n => n.Address)
                    .Select(n => n.Address)));
            }
            Debug.Print("");
        }
    }
}