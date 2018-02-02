#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        private NavigationInteractor<Address> ni;
        private Address addr42;
        private Address addr43;
        private Address addr44;
        private Address addr45;
        private INavigableControl<Address> navControl;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            btnBack = mr.Stub<IButton>();
            btnForward = mr.Stub<IButton>();
            navControl = mr.Stub<INavigableControl<Address>>();

            navControl.Stub(n => n.BackButton).Return(btnBack);
            navControl.Stub(n => n.ForwardButton).Return(btnForward);
            addr42 = Address.Ptr32(42);
            addr43 = Address.Ptr32(43);
            addr44 = Address.Ptr32(44);
            addr45 = Address.Ptr32(45);
        }

        [Test]
        public void Ni_Attach()
        {
            ni = new NavigationInteractor<Address>();
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
            ni = new NavigationInteractor<Address>();
            mr.ReplayAll();


            When_Attached();
            navControl.CurrentAddress = addr42;
            Assert.IsFalse(btnBack.Enabled);
            ni.RememberAddress(addr43);
            Assert.IsTrue(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);

            mr.VerifyAll();
        }

        [Test]
        public void Ni_Back()
        {
            ni = new NavigationInteractor<Address>();
            mr.ReplayAll();

            When_Attached();
            navControl.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);
        }

        [Test]
        public void Ni_Back2_Then_Navigate()
        {
            ni = new NavigationInteractor<Address>();
            mr.ReplayAll();

            When_Attached();
            navControl.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.CurrentAddress = addr43;
            ni.RememberAddress(addr44);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);

            ni.RememberAddress(addr44);
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.CurrentAddress.ToString());
        }

        [Test]
        public void Ni_Back2_Then_Forward2()
        {
            ni = new NavigationInteractor<Address>();
            mr.ReplayAll();

            When_Attached();

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);

            navControl.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.CurrentAddress = addr43;
            ni.RememberAddress(addr44);

            Assert.IsTrue(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);

            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);

            btnForward.Raise(b => b.Click += null, btnForward, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward, EventArgs.Empty);
            Assert.AreEqual("0000002C", navControl.CurrentAddress.ToString());

            Assert.IsTrue(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);
        }

        [Test]
        public void Ni_Back2_Then_Forward2_Distinct()
        {
            ni = new NavigationInteractor<Address>();
            mr.ReplayAll();

            When_Attached();

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);

            navControl.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.CurrentAddress = addr45;
            ni.RememberAddress(addr44);

            Assert.IsTrue(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);

            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002D", navControl.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Enabled);
            Assert.IsTrue(btnForward.Enabled);

            btnForward.Raise(b => b.Click += null, btnForward, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward, EventArgs.Empty);
            Assert.AreEqual("0000002D", navControl.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward, EventArgs.Empty);
            Assert.AreEqual("0000002C", navControl.CurrentAddress.ToString());

            Assert.IsTrue(btnBack.Enabled);
            Assert.IsFalse(btnForward.Enabled);
        }
    }
}
