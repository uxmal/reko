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
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Windows;
using Rhino.Mocks;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class DisassemblyViewInteractorTests
    {
        private DisassemblyViewInteractor interactor;
        private MockRepository repository;
        private IDecompilerShellUiService uiSvc;
        private IDialogFactory dlgFactory;
        private IServiceContainer sc;
        private IDecompilerService dcSvc;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            interactor = repository.Stub<DisassemblyViewInteractor>();
            sc = new ServiceContainer();
            uiSvc = repository.DynamicMock<IDecompilerShellUiService>();
            dcSvc = repository.Stub<IDecompilerService>();
            dlgFactory = repository.DynamicMock<IDialogFactory>();
            sc.AddService<IDecompilerShellUiService>(uiSvc);

            sc.AddService<IDecompilerService>(dcSvc);
            sc.AddService<IDialogFactory>(dlgFactory);
        }

        private void Initialize()
        {
            interactor.SetSite(sc);
            interactor.CreateControl();
        }

        [Test]
        public void GotoAddressEnabled()
        {
            var status = new CommandStatus();
            Assert.IsTrue(interactor.QueryStatus(new CommandID(CmdSets.GuidReko, CmdIds.ViewGoToAddress), status, null));
            Assert.AreEqual(MenuStatus.Enabled | MenuStatus.Visible, status.Status);
        }

        [Test]
        public void DviGotoAddress()
        {
            var dlg = repository.Stub<IAddressPromptDialog>();
            dlg.Stub(x => dlg.Address).Return(Address.Ptr32(0x41104110));
            dlgFactory.Stub(x => x.CreateAddressPromptDialog()).Return(dlg);
            uiSvc.Expect(x => uiSvc.ShowModalDialog(
                    Arg<IAddressPromptDialog>.Is.Same(dlg)))
                .Return(DialogResult.OK);
            dlg.Expect(x => x.Dispose());
            repository.ReplayAll();

            Initialize();
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.ViewGoToAddress));

            repository.VerifyAll();
            Assert.AreEqual(0x41104110ul, interactor.StartAddress.ToLinear());
        }

        [Test]
        public void DviSupportMarkProcedure()
        {
            var status = new CommandStatus();
            var ret = interactor.QueryStatus(new CommandID(CmdSets.GuidReko, CmdIds.ActionMarkProcedure), status, null);
            Assert.IsTrue(ret);
            Assert.AreEqual(MenuStatus.Enabled | MenuStatus.Visible, status.Status);
        }
    }
}
