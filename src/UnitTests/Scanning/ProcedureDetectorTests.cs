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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ProcedureDetectorTests
    {
        // Labels for the addresses used in the unit tests.
        const uint Even = 4;
        const uint Odd = 7;

        const uint Fused1 = 0x10;
        const uint Fused2 = 0x20;
        const uint Fused3 = 0x30;
        const uint FusedExit = 0x13;

        private ScanResults sr;
        private Program program;
        private FakeDecompilerEventListener listener;
        private ProcedureDetector prdet;
        private Dictionary<uint, RtlBlock> mpIntBlock;

        [SetUp]
        public void Setup()
        {
            sr = new ScanResults
            {
                ICFG = new DiGraph<RtlBlock>(),
                DirectlyCalledAddresses = new Dictionary<Address, int>(),
                KnownProcedures = new HashSet<Address>(),
            };
            this.program = new Program();
            this.listener = new FakeDecompilerEventListener();
            this.mpIntBlock = new Dictionary<uint, RtlBlock>();
        }

        private RtlBlock Block(uint uAddr)
        {
            RtlBlock block;
            if (!this.mpIntBlock.TryGetValue(uAddr, out block))
            {
                var addr = Address.Ptr32(uAddr);
                block = new RtlBlock(addr, addr.GenerateName("l", ""));
                this.mpIntBlock.Add(uAddr, block);
            }
            return block;
        }

        public void Given_Blocks(params uint [] addrs)
        {
            sr.ICFG = new DiGraph<RtlBlock>();
            foreach (var uAddr in addrs)
            {
                sr.ICFG.Nodes.Add(Block(uAddr));
            }
        }

        public void Given_Edge(uint from, uint to)
        {
            var aFrom = Block(from);
            var aTo = Block(to);
            sr.ICFG.Nodes.Add(aFrom);
            sr.ICFG.Nodes.Add(aTo);
            sr.ICFG.AddEdge(aFrom, aTo);
        }

        public void Given_DirectCall(uint from, uint to)
        {
            var aFrom = Block(from);
            var aTo = Block(to);
            sr.DirectlyCalledAddresses.Add(aTo.Address, 1);
        }

        public void Given_DirectCall(uint to)
        {
            var aTo = Block(to);
            sr.DirectlyCalledAddresses.Add(aTo.Address, 1);
        }

        public void Given_Instrs(uint iBlock, Action<RtlEmitter> b)
        {
            var block = Block(iBlock);
            var instrs = new List<RtlInstruction>();
            var trace = new RtlEmitter(instrs);
            b(trace);

            block.Instructions.Add(
                new RtlInstructionCluster(
                    Address.Ptr32(iBlock * 4),
                    4, 
                    instrs.ToArray()));
        }
        
        private void AssertCluster(
            string sExp, 
            ProcedureDetector.Cluster cluster)
        {
            var sw = new StringWriter();
            WriteCluster(cluster, sw);
            if (sExp != sw.ToString())
            {
                Debug.Print(sw.ToString());
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

        private List<ProcedureDetector.Cluster> SortClusters(IEnumerable<ProcedureDetector.Cluster> clusters)
        {
            // sort clusters by finding the node with min address.
            return (from c in clusters
                    orderby c.Blocks.Min()
                    select c).ToList();
        }

        private void WriteCluster(ProcedureDetector.Cluster cluster, StringWriter sw)
        {
            foreach (var node in cluster.Blocks.OrderBy(n => n.Address))
            {
                sw.WriteLine("{0} {1}", cluster.Entries.Contains(node) ? "*" : " ", node.Address);
                var succs = sr.ICFG.Successors(node).OrderBy(n => n.Address).ToList();
                if (succs.Count > 0)
                {
                    sw.Write("     ");
                    foreach (var s in succs)
                    {
                        sw.Write(" ");
                        if (!cluster.Blocks.Contains(s))
                        {
                            // cross procedure tail call.
                            sw.Write("*");
                        }
                        sw.Write(s.Address);
                    }
                    sw.WriteLine();
                }
            }
        }

        private void Given_ProcedureDetector()
        {
            this.prdet = new ProcedureDetector(this.program, this.sr, this.listener);
        }

        private void Given_EvenOdd()
        {
            // Main program, calls both even and odd.
            Given_Edge(1, 2);
            Given_DirectCall(1, Even);
            Given_DirectCall(1, Odd);

            // Even:
            Given_Edge(Even, 5);
            Given_Edge(Even, 6);
            Given_Edge(6, Odd);     // tail call to Odd.

            // Odd:
            Given_Edge(Odd, 8);
            Given_Edge(Odd, 9);
            Given_Edge(9, Even);    // tail call to Even.
        }

        private void Given_FusedExitNode()
        {
            // This models a common case in modern compilers/linkers, where
            // the exit blocks of functions are "fused". We have three procedures
            // 'Fused1', 'Fused2', and 'Fused3' who all jump to the exit node
            // 'FusedExit'.

            Given_Edge(Fused1, 0x11);
            Given_Edge(Fused1, 0x12);
            Given_Edge(0x11, FusedExit);
            Given_Edge(0x12, FusedExit);

            Given_Edge(Fused2, 0x21);
            Given_Edge(Fused2, 0x22);
            Given_Edge(0x21, FusedExit);
            Given_Edge(0x22, FusedExit);

            Given_Edge(Fused3, 0x31);
            Given_Edge(Fused3, 0x32);
            Given_Edge(0x31, FusedExit);
            Given_Edge(0x32, FusedExit);

            Given_DirectCall(0x42, Fused1);
            Given_DirectCall(0x42, Fused2);
            Given_DirectCall(0x42, Fused3);
        }

        private void Given_LinearCode()
        {
            // Models a simple linear set of blocks.
            Given_Edge(1, 2);
            Given_Edge(2, 3);
            Given_DirectCall(1);
            Given_Instrs(1, m =>
            {
                m.Assign(m.Mem16(m.Word16(0x1234)), m.Word16(0x5678));
            });
        }
        
        public void Given_KnownProcedures()
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
            this.sr.KnownProcedures = knownProcedureAddresses;
        }

        [Test]
        public void Prdet_BuildCluster()
        {
            Given_Edge(1, 2);
            Given_Edge(1, 3);
            Given_Edge(2, 4);
            Given_Edge(3, 4);
            Given_KnownProcedures();

            var prdet = new ProcedureDetector(this.program, this.sr, this.listener);
            var clusters = prdet.FindClusters();

            Assert.AreEqual(1, clusters.Count);
            var sExp =
            #region Expected 
@"  00000001
      00000002 00000003
  00000002
      00000004
  00000003
      00000004
  00000004
";
            #endregion
            AssertCluster(sExp, clusters[0]);
        }

        [Test(Description = "EvenOdd are a pair of mutually recursive functions that call each other with tail calls")]
        public void Prdet_FindClusters_EvenOdd()
        {
            Given_EvenOdd();
            Given_KnownProcedures();
            Given_ProcedureDetector();

            var clusters = prdet.FindClusters();

            Assert.AreEqual(3, clusters.Count);
            var sExp =
            #region Expected 
@"  00000004
      00000005 00000006
  00000005
  00000006
      *00000007
";
            #endregion
            AssertCluster(sExp, clusters[1]);

            sExp =
            #region Expected
@"  00000007
      00000008 00000009
  00000008
  00000009
      *00000004
";
            #endregion
            AssertCluster(sExp, clusters[2]);
        }

        [Test(Description = "Find the procedure entries for the EvenOdd test case")]
        public void Prdet_FindEntries_EvenOdd()
        {
            Given_EvenOdd();
            Given_KnownProcedures();
            Given_ProcedureDetector();

            var clusters = prdet.FindClusters();
            // Get the 'Odd' cluster and find its entries.
            var clOdd = clusters.Single(c => c.Blocks.Contains(Block(Odd)));
            prdet.FindClusterEntries(clOdd);

            var sExp =
            #region Expected
@"* 00000007
      00000008 00000009
  00000008
  00000009
      *00000004
";
            #endregion
            AssertCluster(sExp, clOdd);
        }

        [Test]
        public void Prdet_FindEntries_FusedExit()
        {
            Given_FusedExitNode();
            Given_KnownProcedures();
            Given_ProcedureDetector();

            var clusters = prdet.FindClusters();
            Assert.AreEqual(1, clusters.Count);
            prdet.FindClusterEntries(clusters[0]);

            var sExp =
            #region Expected
@"* 00000010
      00000011 00000012
  00000011
      00000013
  00000012
      00000013
  00000013
* 00000020
      00000021 00000022
  00000021
      00000013
  00000022
      00000013
* 00000030
      00000031 00000032
  00000031
      00000013
  00000032
      00000013
";
            #endregion

            AssertCluster(sExp, clusters[0]);
        }


        [Test(Description = "Remove all jumps to known direct call targets")]
        public void Prdet_Remove_TailCall_Edges()
        {
            Given_Edge(1, 2);   // main program
            Given_Edge(2, 3);
            Given_Edge(3, 2);
            Given_Edge(3, 4);
            Given_Edge(4, 10);      // tail call

            Given_Edge(10, 11);

            Given_DirectCall(99, 10);       // someone else (99) called 10.

            Given_FusedExitNode();
            Given_KnownProcedures();

            Given_ProcedureDetector();

            prdet.RemoveJumpsToKnownProcedures();

            Assert.False(sr.ICFG.ContainsEdge(Block(4), Block(10)), "Should have removed tail call to 10");
        }

        [Test(Description = "Partition a graph with two entries that don't dominate all of the blocks into three subgraphs")]
        public void Prdet_PartitionCluster()
        {
            Given_Edge(1, 2);
            Given_Edge(1, 3);
            Given_Edge(2, 4);
            Given_Edge(3, 4);
            Given_Edge(4, 5);
            Given_Edge(5, 100);

            Given_Edge(11, 12);
            Given_Edge(11, 13);
            Given_Edge(12, 14);
            Given_Edge(13, 14);
            Given_Edge(14, 15);
            Given_Edge(15, 100);

            Given_Edge(100, 101);
            Given_Edge(100, 102);

            Given_DirectCall(1);
            Given_DirectCall(11);

            Given_ProcedureDetector();
            var clusters = prdet.FindClusters();
            Assert.AreEqual(1, clusters.Count);
            var cluster = clusters[0];
            prdet.FindClusterEntries(cluster);
            Assert.AreEqual(2, cluster.Entries.Count);
            var newClusters = prdet.PartitionIntoSubclusters(cluster);
            Assert.AreEqual(3, newClusters.Count);
        }

        [Test(Description = "Handles the special case when multiple functions share the same epilog")]
        public void Prdet_PartitionCluster_SharedTailExit()
        {
            Given_FusedExitNode();
            Given_ProcedureDetector();
            var clusters = prdet.FindClusters();
            Assert.AreEqual(1, clusters.Count);
            var cluster = clusters[0];
            prdet.FindClusterEntries(cluster);
            Assert.AreEqual(3, cluster.Entries.Count);
            var newClusters = prdet.PartitionIntoSubclusters(cluster);
            Assert.AreEqual(3, newClusters.Count);

            Assert.IsTrue(Block(FusedExit).IsSharedExitBlock);
        }

        [Test]
        public void Prdet_SimplifyCluster()
        {
            Given_LinearCode();
            Given_ProcedureDetector();

            var clusters = prdet.FindClusters();
            Assert.AreEqual(1, clusters.Count);
            var cluster = clusters[0];
            prdet.FuseLinearBlocks(cluster);
            var sExp =
            #region Expected
@"  00000001
";
            #endregion
            AssertCluster(sExp, cluster);
        }
    }
}
