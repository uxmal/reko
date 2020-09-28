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
using Reko.Gui;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class CodeLocationTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.program = new Program
            {
                Architecture = new FakeArchitecture()
            };
        }

        [Test]
        public void NavigateToAddress()
        {
            var project = new Project { Programs = { program } };
            var memSvc = new Mock<ILowLevelViewService>();
            var decSvc = new Mock<IDecompilerService>();
            var dec = new Mock<IDecompiler>();
            memSvc.Setup(x => x.ShowMemoryAtAddress(
                It.IsNotNull<Program>(),
                It.Is<Address>(a => a.ToLinear() == 0x1234))).Verifiable();
            decSvc.Setup(d => d.Decompiler).Returns(dec.Object);
            dec.Setup(d => d.Project).Returns(project);

            var sc = new ServiceContainer();
            sc.AddService<ILowLevelViewService>(memSvc.Object);
            sc.AddService<IDecompilerService>(decSvc.Object);
            var nav = new AddressNavigator(program, Address.Ptr32(0x1234), sc);
            nav.NavigateTo();

            memSvc.VerifyAll();
        }

        [Test]
        public void NavigateToProcedure()
        {
            var proc = new Procedure(program.Architecture, "foo", Address.Ptr32(0x00123400), null);

            var codeSvc = new Mock<ICodeViewerService>();
            codeSvc.Setup(x => x.DisplayProcedure(program, proc, true)).Verifiable();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc.Object);
            var nav = new ProcedureNavigator(program, proc, sc);
            nav.NavigateTo();
            codeSvc.VerifyAll();
        }

        [Test]
        public void NavigateToBlock()
        {
            var proc = new Procedure(program.Architecture, "foo", Address.Ptr32(0x00123400), null);
            var block = new Block(proc, "foo_block");
            var codeSvc = new Mock<ICodeViewerService>();
            codeSvc.Setup(x => x.DisplayProcedure(program, proc, true)).Verifiable();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc.Object);
            var nav = new BlockNavigator(program, block, sc);
            nav.NavigateTo();

            codeSvc.VerifyAll();
        }
    }
}
