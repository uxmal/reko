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
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Design;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ProgramDesignerTests
    {
        private Program program;
        private ProgramDesigner programDesigner;
        private MockRepository mr;
        private ITreeNode node;
        private ITreeNodeDesignerHost host;
        private IDecompilerShellUiService uiSvc;
        private CommandID cmdidLoadSymbols;
        private ServiceContainer sc;
        private ISymbolLoadingService slSvc;
        private ISymbolSource symSrc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            this.sc = new ServiceContainer();
            this.node = mr.StrictMock<ITreeNode>();
            this.host = mr.StrictMock<ITreeNodeDesignerHost>();
            this.uiSvc = mr.StrictMock<IDecompilerShellUiService>();
            this.cmdidLoadSymbols = new CommandID(CmdSets.GuidReko, CmdIds.LoadSymbols);
            this.slSvc = mr.StrictMock<ISymbolLoadingService>();
            this.symSrc = mr.StrictMock<ISymbolSource>();

            // Add services to the Service container (which in the real program is the Reko "main window")
            this.sc.AddService<ISymbolLoadingService>(slSvc);
            this.sc.AddService<IDecompilerShellUiService>(uiSvc);
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
            mr.ReplayAll();

            Expect_MenuItem_LoadSymbols_Enabled();
            // The idea is to write statements in C# that follow the user story.
        }

        [Test]
        public void ProgDes_CHeader_ShowOpenFileDialog()
        {
            Given_Program_NoSymbols();
            Expect_OpenFileDialog();
            Expect_CallsToSymbolService();
            mr.ReplayAll();

            When_User_Selects_LoadSymbols();
        }

        private void Expect_CallsToSymbolService()
        {
            slSvc.Expect(s => s.GetSymbolSource("foo.h")).Return(symSrc);
            symSrc.Expect(s => s.GetAllSymbols()).Return(new List<ImageSymbol>
            {
                new ImageSymbol(Address.Ptr32(0x00112240))
                {
                    Name = "my_procedure",
                    Signature = new SerializedSignature
                    {
                        // let's not worry about this yet.
                    }
                }
            });
        }

        private void When_User_Selects_LoadSymbols()
        {
            var result = programDesigner.Execute(cmdidLoadSymbols);
            Assert.IsTrue(result, "Expected command handler to be implemented");
        }

        private void Expect_OpenFileDialog()
        {
            uiSvc.Expect(u => u.ShowOpenFileDialog(null)).IgnoreArguments().Return("foo.h");
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
            var addr = Address.Ptr32(0x00112200);
            this.program.ImageMap = new ImageMap(addr);
            this.program.SegmentMap = new SegmentMap(addr);
            this.programDesigner = new ProgramDesigner();
            this.programDesigner.Host = host;
            this.programDesigner.TreeNode = node;
            this.programDesigner.Services = sc;
            this.programDesigner.Initialize(program);
        }
    }
}
