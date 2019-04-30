#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class CallHierarchyInteractorTests
    {
        private CallHierarchyView view;
        private Mock<IProcessorArchitecture> arch;
        private Procedure proc1;
        private Program program;
        private CallHierarchyInteractor interactor;

        [SetUp]
        public void Setup()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.program = new Program();
            this.proc1 = new Procedure(arch.Object, "proc1", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Ptr32));
            this.program.CallGraph.AddProcedure(proc1);
        }

        private void Given_CallHierarchyView()
        {
            this.view = new CallHierarchyView();
        }

        private void Given_CallHierarchyInteractor()
        {
            this.interactor = new CallHierarchyInteractor(this.view); ;
        }

        private void When_AddProcedure(Procedure proc)
        {
            ((ICallHierarchyService) interactor).AddProcedure(program, proc);
        }

        [Test]
        public void Chi_AddProcedure()
        {
            Given_CallHierarchyView();
            Given_CallHierarchyInteractor();

            When_AddProcedure(proc1);

            Assert.AreEqual("+ Proc");
        }
    }
}
