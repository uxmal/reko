#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class MemoryViewInteractorTests
    {
        private MemoryViewInteractor interactor;
        private MockRepository repository;
        private IDecompilerShellUiService decShellUiSvc;
        private IDialogFactory dlgFactory;
        private ServiceContainer sp;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            sp = new ServiceContainer();
            decShellUiSvc = repository.DynamicMock<IDecompilerShellUiService>();
			dlgFactory = repository.DynamicMock<IDialogFactory>();
            sp.AddService(typeof(IDecompilerShellUiService), decShellUiSvc);
			sp.AddService(typeof(IDialogFactory), dlgFactory);
        }

        private void Initialize()
        {
            interactor.SetSite(sp);
            interactor.CreateControl();
        }

        [Test]
        public void GotoAddressEnabled()
        {
            interactor = new MemoryViewInteractor();
            var status = new CommandStatus();
            Assert.IsTrue(interactor.QueryStatus(ref CmdSets.GuidDecompiler, CmdIds.ViewGoToAddress, status, null));
            Assert.AreEqual(status.Status, MenuStatus.Enabled | MenuStatus.Visible);
        }

        [Test]
        public void GotoAddress()
        {
            var dlg = repository.Stub<IAddressPromptDialog>();
            dlg.Stub(x => dlg.Address).Return(new Address(0x12345678));
            decShellUiSvc.Expect(x => x.ShowModalDialog(
                    Arg<IAddressPromptDialog>.Is.Same(dlg)))
                .Return(DialogResult.OK);
            dlg.Expect(x => x.Dispose());
            repository.ReplayAll();

            interactor = new MemoryViewInteractor();
            Initialize();
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ViewGoToAddress);

            repository.VerifyAll();
            Assert.AreEqual(0x12345678, interactor.Control.SelectedAddress.Linear);
        }
    }
}
