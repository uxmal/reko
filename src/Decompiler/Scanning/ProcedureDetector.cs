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

        public ProcedureDetector(Program program, ScanResults sr, DecompilerEventListener listener)
        {
            this.program = program;
            this.sr = sr;
            this.listener = listener;
        }

        public void DetectProcedures()
        {
            PreprocessIcfg(sr);
            var clusters = FindClusters();
            BuildProcedures(sr, clusters);
        }

        private void PreprocessIcfg(ScanResults sr)
        {
            ProcessIndirectJumps(sr);
        }

        private void ProcessIndirectJumps(ScanResults sr)
        {
        }

        /// <summary>
        /// A cluster is a proto-procedure. It consists of a set of entries
        /// and a set of blocks. 
        /// </summary>
        public class Cluster
        {
            public SortedSet<Address> Blocks = new SortedSet<Address>();
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
                var node = nodesLeft.First();
                var cluster = new Cluster();
                clusters.Add(cluster);

                BuildWCC(sr, node, cluster, nodesLeft);
            }
            return clusters;
        }

        private void BuildWCC(ScanResults sr, Address addr, Cluster cluster, HashSet<Address> nodesLeft)
        {
            nodesLeft.Remove(addr);
            cluster.Blocks.Add(addr);

            foreach (var s in sr.ICFG.Successors(addr))
            {
                if (nodesLeft.Contains(s) && !sr.DirectlyCalledAddresses.Contains(s))
                {
                    BuildWCC(sr, s, cluster, nodesLeft);
                }
            }
            if (!sr.DirectlyCalledAddresses.Contains(addr))
            {
                foreach (var p in sr.ICFG.Predecessors(addr))
                {
                    if (nodesLeft.Contains(p))
                    {
                        BuildWCC(sr, p, cluster, nodesLeft);
                    }
                }
            }
        }

        /// <summary>
        /// Given a set of clusters, finds all the entries for each cluster and
        /// tries to partition each cluster into procedures with single entries and exits.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="clusters"></param>
        public List<Procedure> BuildProcedures(ScanResults sr, IEnumerable<Cluster> clusters)
        {
            var procs = new List<Procedure>();
            foreach (var cluster in clusters)
            {
                var entries = FindClusterEntries(sr, cluster);
                procs.AddRange(PostProcessCluster(cluster, entries));
            }
            return procs;
        }

        /// <summary>
        /// For a given cluster, find the probable entries.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public HashSet<Address> FindClusterEntries(ScanResults sr, Cluster cluster)
        {
            var entries = new HashSet<Address>();
            var preds = new Dictionary<Address, int>();
            foreach (var block in cluster.Blocks)
            {
                if (sr.DirectlyCalledAddresses.Contains(block))
                {
                    entries.Add(block);
                }
                preds[block] = sr.ICFG.Predecessors(block).Count;
            }

            // If one or more nodes were the destination of a direct call,
            // use those as entries.
            if (entries.Count > 0)
                return entries;

            // Otherwise, if one or more nodes has zero predecessors, pick it.
            if (preds.Count > 0)
                return preds.Keys.ToHashSet();

            // If we can't find another possibility, return the node with the
            // lowest address.
            return new HashSet<Address> { cluster.Blocks.Min() };
        }

        /// <summary>
        /// Processes a cluster into 1..n procedures.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public List<Procedure> PostProcessCluster(Cluster cluster, HashSet<Address> entries)
        {
            var procs = new List<Procedure>();
            // Create BBs from addresses.
            // Remove all nodes with no predecessors which haven't been marked as entries.
            var deadNodes = cluster.Blocks
                .Where(b => !entries.Contains(b) && sr.ICFG.Predecessors(b).Count == 0)
                .ToHashSet();
            cluster.Blocks.ExceptWith(deadNodes);

            // If the cluster has more than one entry, we have to try to pick it apart.
            if (entries.Count > 1)
            {
                // common special case.
                var aps = new List<Address>();      // articulation points
                foreach(var ap in aps)
                {
                    if (sr.ICFG.Successors(ap).Count == 0)
                    {
                        var preds = sr.ICFG.Predecessors(ap).ToList();
                        cluster.Blocks.Remove(ap);
                        sr.ICFG.Nodes.Remove(ap);
                    }
                }
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
            }
            else
            {
                procs.Add(BuildProcedure(cluster, entries.First()));
            }
            return procs;
        }

        private Procedure BuildProcedure(Cluster cluster, Address entry)
        {
            return Procedure.Create(entry, new Frame(null));
        }
    }
}