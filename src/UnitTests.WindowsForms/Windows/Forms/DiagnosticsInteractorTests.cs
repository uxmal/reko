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
using Reko.Core.Services;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class DiagnosticsInteractorTests
    {
        private ListView lv;
        private TestDiagnosticsInteractor interactor;
        private IDiagnosticsService svc;

        [SetUp]
        public void Setup()
        {
            lv = new ListView();
            lv.CreateControl();
            interactor = new TestDiagnosticsInteractor();
            interactor.Attach(lv);
            svc = interactor;
        }

        [TearDown]
        public void Teardown()
        {
            lv.Dispose();
        }

        [Test]
        public void AddDiagnostic()
        {
            svc.Warn(new NullCodeLocation("01000"), "test");
            Assert.AreEqual(1, lv.Items.Count);
            Assert.AreEqual("01000", lv.Items[0].Text);
        }

        [Test]
        public void ClearDiagnostics()
        {
            svc.Warn(new NullCodeLocation("01000"), "test");
            svc.ClearDiagnostics();
            Assert.AreEqual(0, lv.Items.Count);
        }

        [Test]
        public void NavigateOnDoubleClick()
        {
            var location = new Mock<ICodeLocation>();
            location.Setup(x => x.NavigateTo()).Verifiable();

            svc.Warn(location.Object, "Hello");
            interactor.FocusedListItem = lv.Items[0];
            interactor.UserDoubleClicked();

            location.VerifyAll();
        }

        [Test(Description = "If no items selected, the Copy menu item should be visible but disabled")]
        public void Di_NoItemsSelected_DisableCopy()
        {
            var cmd = new CommandID(CmdSets.GuidReko, CmdIds.EditCopy);
            var status = new CommandStatus();
            var txt = new CommandText();
            Assert.IsTrue(interactor.QueryStatus(cmd, status, txt));

            Assert.AreEqual(MenuStatus.Visible, status.Status);
        }

        [Test(Description = "If >0 items selected, the Copy menu item should be visible and enabled")]
        public void Di_ItemsSelected_EnableCopy()
        {
            svc.Error("Nilz");
            lv.SelectedIndices.Add(0);

            var cmd = new CommandID(CmdSets.GuidReko, CmdIds.EditCopy);
            var status = new CommandStatus();
            var txt = new CommandText();
            Assert.IsTrue(interactor.QueryStatus(cmd, status, txt));

            Assert.AreEqual(MenuStatus.Visible|MenuStatus.Enabled, status.Status);
        }        

        private class TestDiagnosticsInteractor : DiagnosticsInteractor
        {
            public void UserDoubleClicked()
            {
                base.listView_DoubleClick(null, EventArgs.Empty);
            }

            public override ListViewItem FocusedListItem
            {
                get; set;
            }
        }
    }
}
