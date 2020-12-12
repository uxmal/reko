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
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class DecompilerShellUiTests
    {
        Form form;
        DecompilerShellUiService svc;
        ServiceContainer sc;
        private MainForm mainForm;

        [SetUp]
        public void Setup()
        {
            form = new Form();
            form.IsMdiContainer = true;
            sc = new ServiceContainer();
            mainForm = new MainForm();
            svc = new DecompilerShellUiService(mainForm, null, null, null, sc);
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
            form = null;
        }
 
        [Test]
        public void DSU_CreateWindow()
        {
            var pane = new Mock<IWindowPane>();
            pane.Setup(p => p.SetSite(sc));
            pane.Setup(p => p.CreateControl()).Returns(new Control());

            IWindowFrame window = svc.CreateWindow("testWin", "Test Window", pane.Object);
            Assert.IsNotNull(window);
            Assert.AreEqual(1, svc.DocumentWindows.Count());
            Assert.AreSame(pane.Object, svc.DocumentWindows.First().Pane);
            window.Show();
        }


        [Test]
        public void DSU_UserCloseWindow()
        {
            form.Show();

            var pane = new Mock<IWindowPane>();
            pane.Setup(p => p.SetSite(sc)).Verifiable();
            pane.Setup(p => p.CreateControl()).Returns(new Control()).Verifiable();
            pane.Setup(p => p.Close()).Verifiable();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane.Object);
            frame.Show();
            Assert.AreEqual(1, svc.DocumentWindows.Count());
            Assert.IsNotNull(svc.FindWindow("testWindow"));
            frame.Close();
            Assert.IsNull(svc.FindWindow("testWindow"));

            pane.VerifyAll();
        }

        [Test]
        public void DSU_MultipleCallsToShowShouldntCreateNewPaneControl()
        {
            form.Show();

            var pane = new Mock<IWindowPane>();
            var ctrl1 = new Control();
            pane.Setup(s => s.SetSite(It.IsAny<IServiceProvider>())).Verifiable();
            pane.Setup(s => s.CreateControl()).Returns(ctrl1).Verifiable();
            pane.Setup(s => s.Close());

            var frame = svc.CreateWindow("testWindow", "Test Window", pane.Object);
            frame.Show();
            frame.Show();
            frame.Close();

            pane.VerifyAll();
        }
     }
}
