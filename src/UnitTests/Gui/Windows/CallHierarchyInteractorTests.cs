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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class CallHierarchyInteractorTests
    {
        private Mock<ICallHierarchyView> view;
        private Mock<IProcessorArchitecture> arch;
        private Procedure proc1;
        private Procedure proc2;
        private Procedure proc3;
        private Program program;
        private CallHierarchyInteractor interactor;

        [SetUp]
        public void Setup()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.program = new Program();
            // Create a program graph where proc1 calls proc2, and proc2 calls proc3.
            this.proc1 = new Procedure(arch.Object, "proc1", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            this.proc2 = new Procedure(arch.Object, "proc2", Address.Ptr32(0x00123500), new Frame(PrimitiveType.Ptr32));
            this.proc3 = new Procedure(arch.Object, "proc3", Address.Ptr32(0x00123600), new Frame(PrimitiveType.Ptr32));
            var stm1 = proc1.EntryBlock.Statements.Add(proc1.EntryAddress.ToLinear(), new CallInstruction(
                new ProcedureConstant(PrimitiveType.Ptr32, proc2),
                new CallSite(0, 0)));
            var stm2 = proc2.EntryBlock.Statements.Add(proc2.EntryAddress.ToLinear(), new CallInstruction(
                new ProcedureConstant(PrimitiveType.Ptr32, proc3),
                new CallSite(0, 0)));

            this.program.CallGraph.AddProcedure(proc1);
            this.program.CallGraph.AddProcedure(proc2);
            this.program.CallGraph.AddProcedure(proc3);
            this.program.CallGraph.AddEdge(stm1, proc2);
            this.program.CallGraph.AddEdge(stm2, proc3);
        }

        private void Given_CallHierarchyView()
        {
            var btn = new Mock<IButton>();
            this.view = new Mock<ICallHierarchyView>();
            this.view.Setup(v => v.DeleteButton).Returns(btn.Object);
        }

        private void Given_CallHierarchyInteractor()
        {
            this.interactor = new CallHierarchyInteractor(this.view.Object); ;
        }

        private void When_AddProcedure(Procedure proc)
        {
            ((ICallHierarchyService) interactor).AddProcedure(program, proc);
        }

        [Test]
        [Ignore("This will go away in Gui rework")]
        public void Chi_AddProcedure()
        {
            Given_CallHierarchyView();
            Given_CallHierarchyInteractor();

            When_AddProcedure(proc3);

            var node1 = view.Object.CallTree.Nodes[0];
            Assert.AreEqual("proc3", node1.Text);
            Assert.AreEqual(2, node1.Nodes.Count);
            Assert.AreEqual("Calls to 'proc3'", node1.Nodes[0].Text);
            Assert.AreEqual("Calls from 'proc3'", node1.Nodes[1].Text);
        }
    }
}
