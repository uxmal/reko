#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Ignore("This test won't pass until we've simulated a ListView...")]
    //$REVIEW: and that reminds us that we probably don't want the abtraction layer to go across this dialog.
    [Category(Categories.UserInterface)]
    public class SymbolSourceInteractorTests
    {
        private Mock<ISymbolSourceDialog> dlg;
        private ServiceContainer sc;
        private Mock<ISymbolLoadingService> symLdrSvc;
        private Mock<IConfigurationService> cfgSvc;
        private Mock<IFileSystemService> fsSvc;
        private Mock<IDecompilerShellUiService> uiSvc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            symLdrSvc = new Mock<ISymbolLoadingService>();
            fsSvc = new Mock<IFileSystemService>();
            cfgSvc = new Mock<IConfigurationService>();
            uiSvc = new Mock<IDecompilerShellUiService>();
            cfgSvc.Setup(c => c.GetSymbolSources())
                .Returns(new List<SymbolSourceDefinition>());

            sc.AddService<IConfigurationService>(cfgSvc.Object);
            sc.AddService<IFileSystemService>(fsSvc.Object);
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
        }

        [TearDown]
        public void TearDown()
        {
            if (dlg is not null)
                dlg.Object.Dispose();
            dlg = null;
        }

        [Test]
        public void SymSrcDlg_Load()
        {
            When_CreateDlg();

            Assert.IsFalse(dlg.Object.SymbolSourceList.Enabled);
            Assert.IsFalse(dlg.Object.AssemblyFile.Enabled);
            Assert.IsFalse(dlg.Object.BrowseAssemblyFile.Enabled);
            Assert.IsFalse(dlg.Object.OkButton.Enabled);
            Assert.IsFalse(dlg.Object.SymbolSourceClasses.Enabled);
        }

        [Test]
        public void SymSrcDlg_FocusLeftSymbolFile()
        {
            When_CreateDlg();
            dlg.Object.SymbolFileUrl.Text = "foo.sym";
            dlg.Object.BrowseSymbolFile.Focus();

            Assert.IsTrue(dlg.Object.SymbolSourceList.Enabled);
        }

        private void When_CreateDlg()
        {
            dlg = new Mock<ISymbolSourceDialog>();
            dlg.Setup(d => d.Services).Returns(sc);
            dlg.Setup(d => d.Dispose());
        }
    }
}
