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
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Forms;
using Rhino.Mocks;
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
        MockRepository mr;
        private MainForm mainForm;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
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
            var pane = mr.StrictMock<IWindowPane>();
            pane.Expect(p => p.SetSite(sc));
            pane.Expect(p => p.CreateControl()).Return(new Control());
            mr.ReplayAll();

            IWindowFrame window = svc.CreateWindow("testWin", "Test Window", pane);
            Assert.IsNotNull(window);
            Assert.AreEqual(1, svc.DocumentWindows.Count());
            Assert.AreSame(pane, svc.DocumentWindows.First().Pane);
            window.Show();
            mr.VerifyAll();
        }


        [Test]
        public void DSU_UserCloseWindow()
        {
            form.Show();

            var pane = mr.StrictMock<IWindowPane>();
            Expect.Call(() => pane.SetSite(sc));
            Expect.Call(pane.CreateControl()).IgnoreArguments().Return(new Control());

            Expect.Call(() => pane.Close());

            mr.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            Assert.AreEqual(1, svc.DocumentWindows.Count());
            Assert.IsNotNull(svc.FindWindow("testWindow"));
            frame.Close();
            Assert.IsNull(svc.FindWindow("testWindow"));

            mr.VerifyAll();

        }

        [Test]
        public void DSU_MultipleCallsToShowShouldntCreateNewPaneControl()
        {
            form.Show();

            var pane = mr.StrictMock<IWindowPane>();
            var ctrl1 = new Control();
            pane.Expect(s => s.SetSite(Arg<IServiceProvider>.Is.Anything));
            pane.Expect(s => s.CreateControl()).Return(ctrl1);
            pane.Expect(s => s.Close());
            mr.ReplayAll();

            var frame = svc.CreateWindow("testWindow", "Test Window", pane);
            frame.Show();
            frame.Show();
            frame.Close();

            mr.VerifyAll();
        }
     }
}
