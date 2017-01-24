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
        private HashSet<Address> pseudoEntries;
        private HashSet<Address> knownProcedures;

        public ProcedureDetector(Program program, ScanResults sr, DecompilerEventListener listener)
        {
            this.program = program;
            this.sr = sr;
            this.listener = listener;
            this.pseudoEntries = new HashSet<Address>();
            this.knownProcedures = CollectKnownProcedures();
        }

        public void DetectProcedures()
        {
            PreprocessIcfg();
            var clusters = FindClusters();
            BuildProcedures(clusters);
        }

        private void PreprocessIcfg()
        {
            RemoveJumpsToKnownProcedures();
            //BuildDominatorTree();
            ProcessIndirectJumps();
        }

        public HashSet<Address> CollectKnownProcedures()
        {
            // The set of known procedures is...
            var knownProcedureAddresses = new HashSet<Address>();
            // ...all procedures the loader was able to deduce
            // from symbols and other metadata..
            knownProcedureAddresses.UnionWith(
                program.ImageSymbols.Values
                    .Where(s => s.Type == SymbolType.Procedure)
                    .Select(s => s.Address));
            // ...all procedures the user has told us about...
            knownProcedureAddresses.UnionWith(
                program.User.Procedures.Keys);
            // ...and all addresses that the Scanner was able to
            //   detect as being called directly.
            knownProcedureAddresses.UnionWith(
                sr.DirectlyCalledAddresses.Keys);
            return knownProcedureAddresses;
        }

        public void RemoveJumpsToKnownProcedures()
        {
            foreach (var calldest in this.knownProcedures)
            {
                if (listener.IsCanceled())
                    break;
                var preds = sr.ICFG.Predecessors(calldest).ToList();
                foreach (var p in preds)
                {
                    sr.ICFG.RemoveEdge(p, calldest);
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
            public SortedSet<Address> Blocks = new SortedSet<Address>();
            public SortedSet<Address> Entries = new SortedSet<Address>();
        }

        /// <summary>
        /// Collects weakly connected components from the ICFG and gathers
        /// them into Clusters.
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public List<Cluster> FindClusters()
        {
            var nodesLeft = new HashSet<Address>(sr.ICFG.Nodes);
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
        /// <param name="addr"></param>
        /// <param name="cluster"></param>
        /// <param name="nodesLeft"></param>
        private void BuildWCC(Address addr, Cluster cluster, HashSet<Address> nodesLeft)
        {
            nodesLeft.Remove(addr);
            cluster.Blocks.Add(addr);

            foreach (var s in sr.ICFG.Successors(addr))
            {
                if (nodesLeft.Contains(s))
                {
                    // Only add if successor is not CALLed.
                    if (!knownProcedures.Contains(s))
                    {
                        BuildWCC(s, cluster, nodesLeft);
                    }
                }
            }
            if (!knownProcedures.Contains(addr))
            {
                // Only backtrack through predecessors if the node
                // is not CALLed.
                foreach (var p in sr.ICFG.Predecessors(addr))
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
            var preds = new Dictionary<Address, int>();
            foreach (var block in cluster.Blocks)
            {
                if (knownProcedures.Contains(block))
                {
                    cluster.Entries.Add(block);
                }
                preds[block] = sr.ICFG.Predecessors(block).Count;
            }

            // If one or more nodes were the destination of a direct call,
            // use those as entries.
            if (cluster.Entries.Count > 0)
                return;

            // Otherwise, if one or more nodes has zero predecessors, pick it.
            if (preds.Count > 0)
            {
                cluster.Entries.UnionWith(preds.Keys);
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

        private HashSet<Address> DetachFusedTails(Cluster cluster)
        {
            var aps = new ArticulationPointFinder<Address>().FindArticulationPoints(sr.ICFG, cluster.Entries);
            aps.IntersectWith(cluster.Blocks);
            var fusedTails = new HashSet<Address>();

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

        private Procedure BuildProcedure(Cluster cluster, Address entry)
        {
            return Procedure.Create(entry, new Frame(null));
        }
    }
}