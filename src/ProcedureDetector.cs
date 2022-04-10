using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RtlBlock = Reko.Scanning.RtlBlock;
using RtlProcedure = Reko.Scanning.RtlProcedure;

namespace Reko.ScannerV2
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
        private readonly DecompilerEventListener listener;
        private readonly HashSet<Address> procedures;
        private readonly IDictionary<Address, RtlBlock> mpAddrToBlock;

        public ProcedureDetector(Core.Program program, ScanResultsV2 cfg, DecompilerEventListener listener)
        {
            this.sr = cfg;
            this.srGraph = cfg.ICFG;
            this.listener = listener;
            this.procedures = cfg.Procedures.Keys.Concat(cfg.SpeculativeProcedures.Keys).ToHashSet();
            DumpDuplicates(cfg.Blocks.Values);
            this.mpAddrToBlock = cfg.Blocks;
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
                var preds = sr.ICFG.Predecessors(calldest);
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
                public override int Compare(RtlBlock? x, RtlBlock? y)
                {
                    if (x is null)
                        return y is null ? 0 : -1;
                    if (y is null)
                        return 1;
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
            var nodesLeft = new HashSet<RtlBlock>(sr.Blocks.Values);
            var clusters = new List<Cluster>();
            int totalCount = nodesLeft.Count;
            if (totalCount > 0)
            {
                listener.ShowProgress("Finding procedure candidates", 0, totalCount);
                var wl = WorkList.Create(nodesLeft.Select(n => n.Address));
                while (wl.TryGetWorkItem(out var node))
                {
                    if (listener.IsCanceled())
                        break;
                    var cluster = new Cluster();
                    clusters.Add(cluster);

                    BuildWCC(node, cluster, wl);
                    BreakOnWatchedAddress(cluster.Blocks.Select(b => b.Address));
                    listener.ShowProgress("Finding procedure candidates", totalCount - wl.Count, totalCount);
                }
            }
            return clusters;
        }

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
        /// <param name="node"></param>
        /// <param name="cluster"></param>
        /// <param name="unvisited"></param>
        private void BuildWCC(
            Address startNode,
            Cluster cluster,
            WorkList<Address> unvisited)
        {
            var queue = new Queue<Address>();
            cluster.Blocks.Add(sr.Blocks[startNode]);
            queue.EnqueueRange(sr.Successors[startNode]
                .Where(e => !procedures.Contains(e.To))
                .Select(e => e.To));
            while (queue.TryDequeue(out var node))
            {
                if (!unvisited.Contains(node))
                    continue;
                unvisited.Remove(node);
                cluster.Blocks.Add(sr.Blocks[node]);
                if (sr.Successors.TryGetValue(node, out var succ))
                {
                    queue.EnqueueRange(succ
                        .Where(e => !procedures.Contains(e.To))
                        .Select(e => e.To));
                }
                if (!procedures.Contains(node) && 
                    sr.Predecessors.TryGetValue(node, out var preds))
                {
                    var pred = sr.Predecessors[node]
                        .Select(e => e.From);
                    queue.EnqueueRange(pred);
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
            var wl = WorkList.Create(cluster.Blocks);
            while (wl.TryGetWorkItem(out var block))
            {
                if (!sr.Successors.TryGetValue(block.Address, out var succs) ||
                    succs.Count != 1)
                    continue;
                var succ = sr.Blocks[succs[0].To];
                if (!sr.Predecessors.TryGetValue(succ.Address, out var preds) ||
                    preds.Count != 1)
                    continue;
                Debug.Assert(preds[0].From == block.Address, "Inconsistent graph");
                if (block.Instructions.Last().Instructions.Last() is not RtlAssignment)
                    continue;

                // Move all instructions into predecessor.
                block.Instructions.AddRange(succ.Instructions);
                block.Length += succ.Length;
                block.FallThrough = succ.FallThrough;
                srGraph.RemoveEdge(block.Address, succ.Address);
                var succSuccs = sr.ICFG.Successors(succ.Address);
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
                .Where(b => !entries.Contains(b) && sr.Predecessors[b.Address].Count == 0)
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
            var auxNode = new RtlBlock(Address.Ptr64(~0ul), "<root>");
            sr.Blocks.TryAdd(auxNode.Address, auxNode);
            var xxxx = sr.ICFG.Nodes.Count;
            var allEntries =
                cluster.Entries.Concat(
                cluster.Blocks
                    .Where(b => srGraph.Predecessors(b.Address).Count == 0))
                .Distinct()
                .OrderBy(b => b.Address)
                .ToList();
            foreach (var entry in allEntries)
            {
                srGraph.AddEdge(auxNode.Address, entry.Address);
            }
            var idoms = LTDominatorGraph<Address>.Create(srGraph, auxNode.Address);
            // DumpDominatorTrees(idoms);
            // Find all nodes whose immediate dominator is "<root>". 
            // Those are the entries to new clusters and may contain blocks
            // that are shared between procedures in the source program.
            var newEntries = cluster.Blocks.Where(b => idoms[b.Address] == auxNode.Address).ToList();
            var dominatedEntries = newEntries.ToDictionary(k => k.Address, v => new HashSet<Address> { v.Address });

            // Partition the nodes in the cluster into categories depending on which
            // one of the newEntries they are dominated by.
            foreach (var b in cluster.Blocks)
            {
                if (dominatedEntries.ContainsKey(b.Address))
                    continue; // already there.
                var n = b.Address;
                for (; ; )
                {
                    var i = idoms[n];
                    if (i == null)
                        break;
                    if (dominatedEntries.ContainsKey(i))
                    {
                        // If my idom is already in the set, add me too.
                        dominatedEntries[i].Add(b.Address);
                        break;
                    }
                    n = i;
                }
            }

            // Now remove the fake node 
            sr.Blocks.TryRemove(auxNode.Address, out _);

            // Handle the special case with new entries that weren't there before,
            // and only consist of a linear sequence of blocks. Mark such nodes as "shared".
            // Later stages will copy these nodes into their respective procedures.
            foreach (var newEntry in dominatedEntries.Keys
                .Where(e => !cluster.Entries.Contains(sr.Blocks[e])).ToList())
            {
                if (srGraph.Successors(newEntry).Count == 0)
                {
                    sr.Blocks[newEntry].IsSharedExitBlock = true;
                    dominatedEntries.Remove(newEntry);
                }
            }

            return dominatedEntries
                .OrderBy(e => e.Key)
                .Select(e => new RtlProcedure(
                    sr.Blocks[e.Key],
                    e.Value.Select(a => sr.Blocks[a]).ToHashSet()))
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