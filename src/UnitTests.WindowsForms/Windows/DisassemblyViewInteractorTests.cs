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
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class DisassemblyViewInteractorTests
    {
        private IServiceContainer sc;
        private DisassemblyViewInteractor interactor;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<IDialogFactory> dlgFactory;
        private Mock<IDecompilerService> dcSvc;

        [SetUp]
        public void Setup()
        {
            interactor = new DisassemblyViewInteractor();
            sc = new ServiceContainer();
            uiSvc = new Mock<IDecompilerShellUiService>();
            dcSvc = new Mock<IDecompilerService>();
            dlgFactory = new Mock<IDialogFactory>();
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);

            sc.AddService<IDecompilerService>(dcSvc.Object);
            sc.AddService<IDialogFactory>(dlgFactory.Object);
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
            var dlg = new Mock<IAddressPromptDialog>();
            dlg.Setup(x => x.Address).Returns(Address.Ptr32(0x41104110));
            dlgFactory.Setup(x => x.CreateAddressPromptDialog()).Returns(dlg.Object);
            uiSvc.Setup(x => x.ShowModalDialog(dlg.Object))
                .Returns(DialogResult.OK)
                .Verifiable();
            dlg.Setup(x => x.Dispose());

            Initialize();
            interactor.Execute(new CommandID(CmdSets.GuidReko, CmdIds.ViewGoToAddress));

            uiSvc.VerifyAll();
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
