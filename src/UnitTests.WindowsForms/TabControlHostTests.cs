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
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class TabControlHostTests
    {
        private TabControl tabCtrl;
        private Mock<IWindowPane> pane;
        private ServiceContainer services;

        [SetUp]
        public void Setup()
        {
            this.tabCtrl = new TabControl();
            this.pane = new Mock<IWindowPane>();
            this.services = new ServiceContainer();
        }

        [Test]
        public void Tch_AttachToPage()
        {
            tabCtrl.TabPages.Add("Test");
            Assert.AreEqual(1, tabCtrl.TabPages.Count);
            ITabControlHostService host = new TabControlHost(services, tabCtrl);
            IWindowFrame frame = host.Attach(pane.Object, tabCtrl.TabPages[0]);
            frame.Title = "Foo";

            Assert.AreEqual("Foo", tabCtrl.TabPages[0].Text);
        }

        [Test]
        public void Tch_AddPage()
        {
            ITabControlHostService host = new TabControlHost(services, tabCtrl);
            host.Add(pane.Object, "Foo");

            Assert.AreEqual("Foo", tabCtrl.TabPages[0].Text);
        }
    }
}
