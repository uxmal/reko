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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Lib;
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
        private ScanResults sr;
        private Program program;
        private FakeDecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            sr = new ScanResults
            {
                ICFG = new DiGraph<Address>(),
                DirectlyCalledAddresses = new HashSet<Address>(),
            };
            this.program = new Program();
            this.listener = new FakeDecompilerEventListener();
        }

        public void Given_Addresses(params uint [] addrs)
        {
            sr.ICFG = new DiGraph<Address>();
            foreach (var uAddr in addrs)
            {
                sr.ICFG.Nodes.Add(Address.Ptr32(uAddr));
            }
        }

        public void Given_Edge(uint from, uint to)
        {
            sr.ICFG.AddEdge(Address.Ptr32(from), Address.Ptr32(to));
        }

        public void Given_DirectCalls(params uint[] directCallDestinations)
        {
            sr.DirectlyCalledAddresses = directCallDestinations
                .Select(uAddr => Address.Ptr32(uAddr))
                .ToHashSet();
        }

        private void AssertCluster(string sExp, ProcedureDetector.Cluster cluster)
        {
            var sw = new StringWriter();
            WriteCluster(cluster, sw);
            if (sExp != sw.ToString())
            {
                Debug.Print(sw.ToString());
                Assert.AreEqual(sExp, sw.ToString());
            }
        }

        private void WriteCluster(ProcedureDetector.Cluster cluster, StringWriter sw)
        {
            foreach (var node in cluster.Blocks.OrderBy(n => n))
            {
                sw.WriteLine("  {0}", node);
                var succs = sr.ICFG.Successors(node).OrderBy(n => n).ToList();
                if (succs.Count > 0)
                {
                    sw.WriteLine("      {0}", string.Join(" ", succs));
                }
            }
        }

        [Test]
        public void Prdet_BuildCluster()
        {
            Given_Addresses(1, 2, 3, 4);
            Given_Edge(1, 2);
            Given_Edge(1, 3);
            Given_Edge(2, 4);
            Given_Edge(3, 4);

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
    }
}
