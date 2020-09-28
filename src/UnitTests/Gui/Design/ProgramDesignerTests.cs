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
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Design;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ProgramDesignerTests
    {
        private Program program;
        private ProgramDesigner programDesigner;
        private Mock<ITreeNode> node;
        private Mock<ITreeNodeDesignerHost> host;
        private Mock<IDecompilerShellUiService> uiSvc;
        private CommandID cmdidLoadSymbols;
        private ServiceContainer sc;
        private Mock<ISymbolLoadingService> slSvc;
        private Mock<ISymbolSource> symSrc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.node = new Mock<ITreeNode>();
            this.host = new Mock<ITreeNodeDesignerHost>();
            this.uiSvc = new Mock<IDecompilerShellUiService>();
            this.cmdidLoadSymbols = new CommandID(CmdSets.GuidReko, CmdIds.LoadSymbols);
            this.slSvc = new Mock<ISymbolLoadingService>();
            this.symSrc = new Mock<ISymbolSource>();

            // Add services to the Service container (which in the real program is the Reko "main window")
            this.sc.AddService<ISymbolLoadingService>(slSvc.Object);
            this.sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
        }
        /*
         * I have a binary with no debugging symbols, but i prepared a C/C++ header 
         * containing structs, enums, typedefs and externs (with function prototypes).
         *  I right-click on the binary after loading it. I pick “Load symbols…” from 
         *  the context menu. I select “C/C++ header file.” Reko parses the C file
         *   and imports the symbols/functions.
         */
        [Test]
        public void ProgDes_CHeader_MenuItemEnabled()
        {
            // A program designer is the öbject behind the "tree item" for each program.
            Given_Program_NoSymbols();

            Expect_MenuItem_LoadSymbols_Enabled();
            // The idea is to write statements in C# that follow the user story.
        }

        [Test]
        public void ProgDes_CHeader_ShowOpenFileDialog()
        {
            Given_Program_NoSymbols();
            Expect_OpenFileDialog();
            Expect_CallsToSymbolService();

            When_User_Selects_LoadSymbols();
            slSvc.VerifyAll();
            symSrc.VerifyAll();
            uiSvc.VerifyAll();
        }

        private void Expect_CallsToSymbolService()
        {
            slSvc.Setup(s => s.GetSymbolSource("foo.h")).Returns(symSrc.Object).Verifiable();
            symSrc.Setup(s => s.GetAllSymbols()).Returns(new List<ImageSymbol>
            {
                ImageSymbol.Procedure(
                    program.Architecture,
                    Address.Ptr32(0x00112240),
                    "my_procedure",
                    signature:new SerializedSignature
                    {
                        // let's not worry about this yet.
                    })
            }).Verifiable();
        }

        private void When_User_Selects_LoadSymbols()
        {
            var result = programDesigner.Execute(cmdidLoadSymbols);
            Assert.IsTrue(result, "Expected command handler to be implemented");
        }

        private void Expect_OpenFileDialog()
        {
            uiSvc.Setup(u => u.ShowOpenFileDialog(It.IsAny<string>())).Returns("foo.h");
        }

        private void Expect_MenuItem_LoadSymbols_Enabled()
        {
            var status = new CommandStatus();
            var txt = new CommandText();
            var outcome = programDesigner.QueryStatus(cmdidLoadSymbols, status, txt);
            Assert.IsTrue(outcome, "Expected command to be supported");
            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }

        private void Given_Program_NoSymbols()
        {
            this.program = new Program();
            this.program.Architecture = new FakeArchitecture();
            var addr = Address.Ptr32(0x00112200);
            this.program.ImageMap = new ImageMap(addr);
            this.program.SegmentMap = new SegmentMap(addr);
            this.programDesigner = new ProgramDesigner();
            this.programDesigner.Host = host.Object;
            this.programDesigner.TreeNode = node.Object;
            this.programDesigner.Services = sc;
            this.programDesigner.Initialize(program);
        }
    }
}
