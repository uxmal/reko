/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class PostDominatorGraphTests
    {
        private ProcedureStructure h;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PostDominateIfElse()
        {
            ProcedureMock m = new ProcedureMock();
            m.BranchIf(m.Local32("a"), "then");
            m.Assign(m.Local32("b"), m.Int32(0));
            m.Jump("join");
            m.Label("then");
            m.Assign(m.Local32("c"), m.Int32(0));
            m.Label("join");
            m.Return();

            FindPostDominators(m);

            Assert.AreEqual("l1", h.ReverseOrdering[0].ImmPDom.Block.Name);
            Assert.AreEqual("join", h.ReverseOrdering[1].ImmPDom.Block.Name);
            Assert.AreEqual("join", h.ReverseOrdering[2].ImmPDom.Block.Name);
            Assert.AreEqual("join", h.ReverseOrdering[3].ImmPDom.Block.Name);
            Assert.AreSame(m.Procedure.ExitBlock, h.ReverseOrdering[4].ImmPDom.Block);
        }


        [Test]
        public void PostdominateLoop()
        {
            ProcedureMock m = new ProcedureMock();
            m.Jump("test");
            m.Label("test");
            m.BranchIf(m.LocalBool("f"), "done");
            m.Label("body");
            m.Store(m.Int32(30), m.Int32(0));
            m.Jump("test");
            m.Label("done");
            m.Return();

            FindPostDominators(m);

            Assert.AreEqual("ProcedureMock_entry PD> l1", PostDom(h.ReverseOrdering[0]));
            Assert.AreEqual("l1 PD> test", PostDom(h.ReverseOrdering[1]));
            Assert.AreEqual("body PD> test", PostDom(h.ReverseOrdering[2]));
            Assert.AreEqual("test PD> done", PostDom(h.ReverseOrdering[3]));
            Assert.AreEqual("done PD> ProcedureMock_exit", PostDom(h.ReverseOrdering[4]));
            Assert.IsNull(h.ReverseOrdering[5].ImmPDom, "exit block shouldn't have an immediate post dominator");
        }

        [Test]
        public void LoopWithIfElse()
        {
            ProcedureMock m = new ProcedureMock();
            Identifier c = m.Declare(PrimitiveType.Word32, "c");
            Identifier f = m.Declare(PrimitiveType.Bool, "f");
            m.Label("loopHead");
            m.BranchIf(m.Eq(c, 0), "done");
            m.BranchIf(f, "then");
            m.Label("else");
            m.SideEffect(m.Fn("CallElse"));
            m.Jump("loopHead");
            m.Label("then");
            m.SideEffect(m.Fn("CallThen"));
            m.Jump("loopHead");
            m.Label("done");
            m.Return();

            FindPostDominators(m);

            Assert.AreEqual("loopHead PD> done", PostDom("loopHead"));
            Assert.AreEqual("else PD> loopHead", PostDom("else"));
            Assert.AreEqual("then PD> loopHead", PostDom("then"));
            Assert.AreEqual("l1 PD> loopHead", PostDom("l1"));
            Assert.AreEqual("done PD> ProcedureMock_exit", PostDom("done"));
        }

        [Test]
        public void InfiniteLoop()
        {
            ProcedureMock m = new ProcedureMock();
            m.Label("Infinity");
            m.BranchIf(m.Eq(m.LoadW(m.Word16(0x1234)), 0), "hop");
            m.SideEffect(m.Fn("foo"));
            m.Label("hop");
            m.BranchIf(m.Eq(m.LoadW(m.Word16(0x5123)), 1), "Infinity");
            m.SideEffect(m.Fn("bar"));
            m.Jump("Infinity");
            m.Return();

            FindPostDominators(m);
        }

        private void FindPostDominators(ProcedureMock m)
        {
            PostDominatorGraph gr = BuildPostDominatorGraph(m); 
            gr.FindImmediatePostDominators();
        }

        private PostDominatorGraph BuildPostDominatorGraph(ProcedureMock m)
        {
            m.Procedure.RenumberBlocks();
            ProcedureStructureBuilder graphs = new ProcedureStructureBuilder(m.Procedure);
            graphs.BuildNodes();
            graphs.DefineEdges();
            h = graphs.CreateProcedureStructure();
            graphs.SetTimeStamps();

            PostDominatorGraph gr = new PostDominatorGraph(h);

            return gr;
        }

        private string PostDom(string s)
        {
            foreach (StructureNode node in h.Ordering)
            {
                if (s == node.Block.Name)
                    return PostDom(node);
            }
            throw new InvalidOperationException("Unknown node: " + s);
        }

        private string PostDom(StructureNode node)
        {
            return string.Format("{0} PD> {1}", node.Block.Name, node.ImmPDom.Block.Name);
        }
    }
}
