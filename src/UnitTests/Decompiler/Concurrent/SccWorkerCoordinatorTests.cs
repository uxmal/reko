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

using NUnit.Framework;
using Reko.Concurrent;
using Reko.Core;
using Reko.Core.Graphs;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Reko.UnitTests.Decompiler.Concurrent
{
    [TestFixture]
    public class SccWorkerCoordinatorTests
    {
        private readonly FakeArchitecture arch;
        private DirectedGraph<Procedure> callGraph;
        private uint uAddr;

        public SccWorkerCoordinatorTests()
        {
            arch = new FakeArchitecture();
        }

        class Worker<TInput, TOutput>
        {

        }

        [SetUp]
        public void Setup()
        {
            callGraph = new DiGraph<Procedure>();
            uAddr = 0x1000;
        }


        private Procedure Given_Procedure(string name)
        {
            var proc = Procedure.Create(arch, name, Address.Ptr32(uAddr), arch.CreateFrame());
            uAddr += 0x1000;
            callGraph.Nodes.Add(proc);
            return proc;
        }

        private List<Procedure> Given_Procedures(string format, int count)
        {
            var result = new List<Procedure>();
            for (int i = 0; i < count; ++i)
            {
                var proc = Given_Procedure(string.Format(format, i + 1));
                result.Add(proc);
            }
            return result;
        }

        private void Given_ProcedureTree(Procedure parent, int level)
        {
            if (--level < 0)
                return;
            for (int i = 0; i < 4; ++i)
            {
                var proc = Given_Procedure($"proc{uAddr:X5}_{level}");
                Given_CallEdge(parent, proc);
                Given_ProcedureTree(proc, level);
            }
        }

        private void Given_CallEdge(Procedure caller, Procedure callee)
        {
            callGraph.AddEdge(caller, callee);
        }

        private void Given_Successors(Procedure node, IEnumerable<Procedure> successors)
        {
            foreach (var succ in successors)
            {
                callGraph.AddEdge(node, succ);
            }
        }

        private interface IFlow
        {
            string Name { get; }
            int Hash { get; }
            int Preds { get; }
        }
        private class Flow : IFlow
        {
            public string Name { get; set; }
            public int Hash { get; set; }
            public int Preds { get; set; }
        }


        private class GlobalContext 
        {
            private ConcurrentDictionary<Procedure, IFlow> flow;
            private DirectedGraph<Procedure> callGraph;
            private Random rnd;

            public GlobalContext(DirectedGraph<Procedure> callGraph)
            {
                this.callGraph = callGraph;
                flow = new();
                rnd = new();
            }

            public GlobalContext GetContextFor(Procedure[] members)
            {
                return this;
            }

            public void PerformWork(Procedure[] members)
            {
                foreach (var mem in members)
                {
                    int nPreds = 1;
                    foreach (var succ in callGraph.Successors(mem))
                    {
                        var succFlow = this.flow[succ];
                        nPreds += succFlow.Preds;
                    }
                    var flow = new Flow
                    {
                        Name = mem.Name,
                        Hash = mem.Name.GetHashCode(),
                        Preds = nPreds,
                    };

                    Thread.Sleep(25 + rnd.Next(30));
                    this.flow.TryAdd(mem, flow);
                }
            }

            public IFlow GetFlowFor(Procedure proc)
            {
                return flow[proc];
            }
        }

        private void RunTest(string sExpected)
        {
            var condense = SccFinder.Condense(callGraph);
            var context = new GlobalContext(callGraph);
            var coordinator = new SccWorkerCoordinator<Procedure>(
                condense,
                new FakeDecompilerEventListener(),
                context.PerformWork);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            coordinator.RunAsync().Wait();

            var sw = new StringWriter();
            sw.WriteLine();
            foreach (var proc in callGraph.Nodes.OrderBy(n => n.Name))
            {
                var flow = context.GetFlowFor(proc);
                sw.WriteLine("{0}: {1}", flow.Name, flow.Preds);
            }
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void Sccwc_Simple()
        {
            var a = Given_Procedure("a");
            var b = Given_Procedure("b");

            Given_CallEdge(a, b);

            RunTest(@"
a: 2
b: 1
");
        }

        [Test]
        public void Sccwc_Forest()
        {
            var a = Given_Procedure("a");
            var b = Given_Procedure("b");
            var c = Given_Procedure("c");
            var d = Given_Procedure("d");

            Given_CallEdge(a, b);
            Given_CallEdge(c, d);

            RunTest(@"
a: 2
b: 1
c: 2
d: 1
");
        }

        [Test]
        public void Sccwc_TwoCallees()
        {
            var a = Given_Procedure("a");
            var b1 = Given_Procedure("b1");
            var b2 = Given_Procedure("b2");
            Given_CallEdge(a, b1);
            Given_CallEdge(a, b2);

            RunTest(@"
a: 3
b1: 1
b2: 1
");
        }

        [Test]
        public void Sccwc_BigFanout()
        {
            var a = Given_Procedure("a");
            var kids = Given_Procedures("b{0}", 25);
            Given_Successors(a, kids);

            RunTest(@"
a: 26
b1: 1
b10: 1
b11: 1
b12: 1
b13: 1
b14: 1
b15: 1
b16: 1
b17: 1
b18: 1
b19: 1
b2: 1
b20: 1
b21: 1
b22: 1
b23: 1
b24: 1
b25: 1
b3: 1
b4: 1
b5: 1
b6: 1
b7: 1
b8: 1
b9: 1
");
        }

        [Test]
        public void Sccwc_Tree()
        {
            var root = Given_Procedure("proc01000_2");
            Given_ProcedureTree(root, 2);

            RunTest(@"
proc01000_2: 21
proc02000_1: 5
proc03000_0: 1
proc04000_0: 1
proc05000_0: 1
proc06000_0: 1
proc07000_1: 5
proc08000_0: 1
proc09000_0: 1
proc0A000_0: 1
proc0B000_0: 1
proc0C000_1: 5
proc0D000_0: 1
proc0E000_0: 1
proc0F000_0: 1
proc10000_0: 1
proc11000_1: 5
proc12000_0: 1
proc13000_0: 1
proc14000_0: 1
proc15000_0: 1
");
        }
    }
}
