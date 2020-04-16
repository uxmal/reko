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
using Reko.Gui.Controls;
using Reko.UserInterfaces.WindowsForms;
using System;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class NavigationInteractorTests
    {
        private Mock<IButton> btnBack;
        private Mock<IButton> btnForward;
        private NavigationInteractor<Address> ni;
        private Address addr42;
        private Address addr43;
        private Address addr44;
        private Address addr45;
        private Mock<INavigableControl<Address>> navControl;

        [SetUp]
        public void Setup()
        {
            btnBack = new Mock<IButton>();
            btnForward = new Mock<IButton>();
            navControl = new Mock<INavigableControl<Address>>();
            btnBack.SetupAllProperties();
            btnForward.SetupAllProperties();
            navControl.SetupAllProperties();
            navControl.Setup(n => n.BackButton).Returns(btnBack.Object);
            navControl.Setup(n => n.ForwardButton).Returns(btnForward.Object);
            addr42 = Address.Ptr32(42);
            addr43 = Address.Ptr32(43);
            addr44 = Address.Ptr32(44);
            addr45 = Address.Ptr32(45);
        }

        [Test]
        public void Ni_Attach()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);
        }

        private void When_Attached()
        {
            ni.Attach(navControl.Object);
        }

        [Test]
        public void Ni_UserNavigateTo()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();
            navControl.Object.CurrentAddress = addr42;
            Assert.IsFalse(btnBack.Object.Enabled);
            ni.RememberAddress(addr43);
            Assert.IsTrue(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);
        }

        [Test]
        public void Ni_Back()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();
            navControl.Object.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsTrue(btnForward.Object.Enabled);
        }

        [Test]
        public void Ni_Back2_Then_Navigate()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();
            navControl.Object.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.Object.CurrentAddress = addr43;
            ni.RememberAddress(addr44);
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.Object.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.Object.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsTrue(btnForward.Object.Enabled);

            ni.RememberAddress(addr44);
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.Object.CurrentAddress.ToString());
        }

        [Test]
        public void Ni_Back2_Then_Forward2()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);

            navControl.Object.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.Object.CurrentAddress = addr43;
            ni.RememberAddress(addr44);

            Assert.IsTrue(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);

            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.Object.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.Object.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsTrue(btnForward.Object.Enabled);

            btnForward.Raise(b => b.Click += null, btnForward.Object, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.Object.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward.Object, EventArgs.Empty);
            Assert.AreEqual("0000002C", navControl.Object.CurrentAddress.ToString());

            Assert.IsTrue(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);
        }

        [Test]
        public void Ni_Back2_Then_Forward2_Distinct()
        {
            ni = new NavigationInteractor<Address>();

            When_Attached();

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);

            navControl.Object.CurrentAddress = addr42;
            ni.RememberAddress(addr43);
            navControl.Object.CurrentAddress = addr45;
            ni.RememberAddress(addr44);

            Assert.IsTrue(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);

            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002D", navControl.Object.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.Object.CurrentAddress.ToString());
            btnBack.Raise(b => b.Click += null, btnBack.Object, EventArgs.Empty);
            Assert.AreEqual("0000002A", navControl.Object.CurrentAddress.ToString());

            Assert.IsFalse(btnBack.Object.Enabled);
            Assert.IsTrue(btnForward.Object.Enabled);

            btnForward.Raise(b => b.Click += null, btnForward.Object, EventArgs.Empty);
            Assert.AreEqual("0000002B", navControl.Object.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward.Object, EventArgs.Empty);
            Assert.AreEqual("0000002D", navControl.Object.CurrentAddress.ToString());
            btnForward.Raise(b => b.Click += null, btnForward.Object, EventArgs.Empty);
            Assert.AreEqual("0000002C", navControl.Object.CurrentAddress.ToString());

            Assert.IsTrue(btnBack.Object.Enabled);
            Assert.IsFalse(btnForward.Object.Enabled);
        }
    }
}
