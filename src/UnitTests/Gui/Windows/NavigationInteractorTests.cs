#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Reko.Core;
using Reko.Gui;
using Reko.Gui.Components;
using Reko.Gui.Controls;
using Reko.Gui.Windows;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class NavigationInteractorTests
    {
        private MockRepository mr;
        private IButton btnBack;
        private IButton btnForward;
        private ITimer timer;
        private NavigationInteractor ni;
        private Address addr42;
        private Address addr43;
        private Address addr44;
        private INavigableControl navControl;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            btnBack = mr.Stub<IButton>();
            btnForward = mr.Stub<IButton>();
            timer = mr.Stub<ITimer>();
            navControl = mr.Stub<INavigableControl>();

            navControl.Stub(n => n.BackButton).Return(btnBack);
            navControl.Stub(n => n.ForwardButton).Return(btnForward);
            addr42 = Address.Ptr32(42);
            addr43 = Address.Ptr32(43);
            addr44 = Address.Ptr32(44);
        }

        [Test]
        public void Ni_Attach()
        {
            ni = new NavigationInteractor();
            mr.ReplayAll();

            When_Attached();

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);
        }

        private void When_Attached()
        {
            ni.Attach(navControl);
        }

        [Test]
        public void Ni_UserNavigateTo()
        {
            ni = new NavigationInteractor();
            mr.ReplayAll();


            When_Attached();
            navControl.CurrentAddress = addr42;
            Assert.IsFalse(btnBack.Enabled);
            ni.UserNavigateTo(addr43);
            Assert.IsTrue(btnBack.Enabled);
            Assert.AreEqual(addr43, navControl.CurrentAddress);

            mr.VerifyAll();
        }

        [Test]
        public void Ni_Back()
        {
            ni = new NavigationInteractor();
            mr.ReplayAll();

            When_Attached();
            navControl.CurrentAddress = addr42;
            ni.UserNavigateTo(addr43);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);
            Assert.AreSame(addr42, navControl.CurrentAddress);
        }

        [Test]
        public void Ni_Back2_Then_Navigate()
        {
            ni = new NavigationInteractor();
            mr.ReplayAll();

            When_Attached();
            navControl.CurrentAddress = addr42;
            ni.UserNavigateTo(addr43);
            ni.UserNavigateTo(addr44);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);

            ni.UserNavigateTo(addr44);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreSame(addr42, navControl.CurrentAddress);
        }
    }
}
