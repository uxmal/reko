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
using Reko.Core.Lib;
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
        private Program program;
        private ScanResults sr;
        private DecompilerEventListener listener;
        private HashSet<Address> knownProcedures;
        private Dictionary<Address, HeuristicBlock> mpAddrToBlock;

        public ProcedureDetector(Program program, ScanResults sr, DecompilerEventListener listener)
        {
            this.program = program;
            this.sr = sr;
            this.listener = listener;
            this.knownProcedures = sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys).ToHashSet();
            this.mpAddrToBlock = sr.ICFG.Nodes.ToDictionary(de => de.Address);
        }

        public List<Procedure> DetectProcedures()
        {
            PreprocessIcfg();
            var clusters = FindClusters();
            return BuildProcedures(clusters);
        }

        private void PreprocessIcfg()
        {
            RemoveJumpsToKnownProcedures();
            //BuildDominatorTree();
            ProcessIndirectJumps();
        }

        public void RemoveJumpsToKnownProcedures()
        {
            foreach (var calldest in this.knownProcedures)
            {
                if (listener.IsCanceled())
                    break;
                var node = mpAddrToBlock[calldest];
                var preds = sr.ICFG.Predecessors(node).ToList();
                foreach (var p in preds)
                {
                    sr.ICFG.RemoveEdge(p, node);
                }
            }
        }

        private void ProcessIndirectJumps()
        {
            //$TODO: need some form of backwalking here.
        }

        /// <summary>
        /// A cluster is a proto-procedure. It consists of a set of entries
        /// and a set of blocks. 
        /// </summary>
        public class Cluster
        {
            public SortedSet<HeuristicBlock> Blocks = new SortedSet<HeuristicBlock>(Cmp.Instance);
            public SortedSet<HeuristicBlock> Entries = new SortedSet<HeuristicBlock>(Cmp.Instance);

            private class Cmp : Comparer<HeuristicBlock>
            {
                public override int Compare(HeuristicBlock x, HeuristicBlock y)
                {
                    return x.Address.CompareTo(y.Address);
                }

                public static readonly Cmp Instance = new Cmp();
            }
        }

        /// <summary>
        /// Collects weakly connected components from the ICFG and gathers
        /// them into Clusters.
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public List<Cluster> FindClusters()
        {
            var nodesLeft = new HashSet<HeuristicBlock>(sr.ICFG.Nodes);
            var clusters = new List<Cluster>();
            while (nodesLeft.Count > 0)
            {
                if (listener.IsCanceled())
                    break;
                var node = nodesLeft.First();
                var cluster = new Cluster();
                clusters.Add(cluster);

                BuildWCC(node, cluster, nodesLeft);
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
        /// <param name="nodesLeft"></param>
        private void BuildWCC(
            HeuristicBlock node,
            Cluster cluster,
            HashSet<HeuristicBlock> nodesLeft)
        {
            nodesLeft.Remove(node);
            cluster.Blocks.Add(node);

            foreach (var s in sr.ICFG.Successors(node))
            {
                if (nodesLeft.Contains(s))
                {
                    // Only add if successor is not CALLed.
                    if (!knownProcedures.Contains(s.Address))
                    {
                        BuildWCC(s, cluster, nodesLeft);
                    }
                }
            }
            if (!knownProcedures.Contains(node.Address))
            {
                // Only backtrack through predecessors if the node
                // is not CALLed.
                foreach (var p in sr.ICFG.Predecessors(node))
                {
                    if (nodesLeft.Contains(p))
                    {
                        BuildWCC(p, cluster, nodesLeft);
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
        public List<Procedure> BuildProcedures(IEnumerable<Cluster> clusters)
        {
            var procs = new List<Procedure>();
            foreach (var cluster in clusters)
            {
                if (listener.IsCanceled())
                    break;
                FindClusterEntries(cluster);
                procs.AddRange(PostProcessCluster(cluster));
            }
            return procs;
        }

        /// <summary>
        /// For a given cluster, find the probable entries.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="cluster"></param>
        public void FindClusterEntries(Cluster cluster)
        {
            var nopreds = new List<HeuristicBlock>();
            foreach (var block in cluster.Blocks)
            {
                if (knownProcedures.Contains(block.Address))
                {
                    cluster.Entries.Add(block);
                }
                if (sr.ICFG.Predecessors(block).Count == 0)
                {
                    nopreds.Add(block);
                }
            }

            // If one or more nodes were the destination of a direct call,
            // use those as entries.
            if (cluster.Entries.Count > 0)
                return;

            // Otherwise, if one or more nodes has zero predecessors, pick it.
            if (nopreds.Count > 0)
            {
                cluster.Entries.UnionWith(nopreds);
                return;
            }

            // If we can't find another possibility, return the node with the
            // lowest address.
            cluster.Entries.Add(cluster.Blocks.Min());
        }

        /// <summary>
        /// Processes a cluster into 1..n procedures.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public List<Procedure> PostProcessCluster(Cluster cluster)
        {
            var entries = cluster.Entries;
            var procs = new List<Procedure>();

            // Remove all nodes with no predecessors which haven't been marked as entries.
            var deadNodes = cluster.Blocks
                .Where(b => !entries.Contains(b) && sr.ICFG.Predecessors(b).Count == 0)
                .ToHashSet();
            cluster.Blocks.ExceptWith(deadNodes);

            // If the cluster has more than one entry, we have to try to pick it apart.
            if (entries.Count > 1)
            {
                Debug.Print("Entries {0} share common code", string.Join(",", entries.Select(e => e.Name)));
                var fusedTails = DetachFusedTails(cluster);

                // for each Articulation point ap:
                //    if (ap.Succ.Count(1) == 0 || is_return_ap
                //      preds = ap.preds;
                //      Remove ap from cluster)
                //      foreach (p in ap.pred)
                //          add Clone(p) as succ p.
                //   

                // Foreach entry in entries
                //      c = new cluster
                //      foreach (var node in DFS(entry))
                //          if (node.domby(entry))
                //          {
                //              c.nodes.Add(node)
                //              cluster.nodes.remove();
                //              foreach (s in node.siccs)
                //                  if (c.nodes.!domBy(entry)
                //                      newEntries.Add(s)
                //                  else addEdge(node, s)
                //          }
                //      }

                // $TODO: Build dominator trees for each entry. Nodes dominated
                // by an entry constitute a procedure.

                // After removing the nodes in the dominator trees, there may be 
                // nodes left. Each one of those nodes is part of 1..n clusters.
                // Redo the processing work on those.

            }
            else
            {
                procs.Add(BuildProcedure(cluster, entries.First()));
            }
            return procs;
        }

        private HashSet<HeuristicBlock> DetachFusedTails(Cluster cluster)
        {
            var aps = new ArticulationPointFinder<HeuristicBlock>().FindArticulationPoints(sr.ICFG, cluster.Entries);
            aps.IntersectWith(cluster.Blocks);
            var fusedTails = new HashSet<HeuristicBlock>();

            foreach (var ap in aps)
            {
                if (sr.ICFG.Successors(ap).Count == 0)
                {
                    var preds = sr.ICFG.Predecessors(ap).ToList();
                    cluster.Blocks.Remove(ap);
                    sr.ICFG.Nodes.Remove(ap);
                    fusedTails.Add(ap);
                }
            }
            return fusedTails;
        }

        private Procedure BuildProcedure(Cluster cluster, HeuristicBlock entry)
        {
            FuseBlocks(cluster);
            return Procedure.Create(entry.Address, program.Architecture.CreateFrame());
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

                    var lastInstr = block.Instructions.Last();
                    if (lastInstr.Address + lastInstr.Length != s.Address)
                        continue;
                    var ss = sr.ICFG.Successors(s).ToList();
                    sr.ICFG.RemoveEdge(block, s);
                    block.Instructions.AddRange(s.Instructions);
                    sr.ICFG.RemoveNode(s);
                    foreach (var n in ss)
                    {
                        sr.ICFG.AddEdge(block, n);
                    }
                }
            }
        }
    }
}