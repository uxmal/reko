#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Ignore("This test won't pass until we've simulated a ListView...")]
    //$REVIEW: and that reminds us that we probably don't want the abtraction layer to go across this dialog.
    [Category(Categories.UserInterface)]
    public class SymbolSourceInteractorTests
    {
        private ISymbolSourceDialog dlg;
        private MockRepository mr;
        private ServiceContainer sc;
        private ISymbolLoadingService symLdrSvc;
        private IConfigurationService cfgSvc;
        private IFileSystemService fsSvc;
        private IDecompilerUIService uiSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            symLdrSvc = mr.StrictMock<ISymbolLoadingService>();
            fsSvc = mr.Stub<IFileSystemService>();
            cfgSvc = mr.Stub<IConfigurationService>();
            uiSvc = mr.Stub<IDecompilerShellUiService>();
            cfgSvc.Stub(c => c.GetSymbolSources()).Return(new List<SymbolSource>());

            sc.AddService<IConfigurationService>(cfgSvc);
            sc.AddService<IFileSystemService>(fsSvc);
            sc.AddService<IDecompilerShellUiService>(uiSvc);
        }

        [TearDown]
        public void TearDown()
        {
            if (dlg != null)
                dlg.Dispose();
            dlg = null;
        }

        [Test]
        public void SymSrcDlg_Load()
        {
            mr.ReplayAll();

            When_CreateDlg();

            Assert.IsFalse(dlg.SymbolSourceList.Enabled);
            Assert.IsFalse(dlg.AssemblyFile.Enabled);
            Assert.IsFalse(dlg.BrowseAssemblyFile.Enabled);
            Assert.IsFalse(dlg.OkButton.Enabled);
            Assert.IsFalse(dlg.SymbolSourceClasses.Enabled);
        }

        [Test]
        public void SymSrcDlg_FocusLeftSymbolFile()
        {
            mr.ReplayAll();

            When_CreateDlg();
            dlg.SymbolFileUrl.Text = "foo.sym";
            dlg.BrowseSymbolFile.Focus();

            Assert.IsTrue(dlg.SymbolSourceList.Enabled);
        }

        private void When_CreateDlg()
        {
            dlg = mr.Stub<ISymbolSourceDialog>();
            dlg.Stub(d => d.Services).Return(sc);
            dlg.Stub(d => d.Dispose());
            dlg.Replay();
        }
    }
}
