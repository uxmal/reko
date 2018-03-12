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

using NUnit.Framework;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class FindDialogInteractorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EnableFindButton()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            using (FindDialog dlg = i.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABE";
                Assert.IsTrue(dlg.FindButton.Enabled);
            }
        }

        [Test]
        public void ToHexadecimal()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            Assert.AreEqual(new byte[] { 0x0A }, i.ToHexadecimal("0A"));
            Assert.AreEqual(new byte[] { 0x0A, 0x0B }, i.ToHexadecimal("0A0b"));
            Assert.AreNotEqual(new byte[] { 0x0A, 0x0B }, i.ToHexadecimal("0A0C"));
            Assert.IsNull(i.ToHexadecimal("AGFA"));
        }

        [Test]
        public void EnableFindButtonValidText()
        {
            FindDialogInteractor i = new FindDialogInteractor();
            using (FindDialog dlg = i.CreateDialog())
            {
                dlg.Show();
                Assert.IsFalse(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABF";
                Assert.IsTrue(dlg.FindButton.Enabled);
                dlg.FindText.Text = "0ABG";
                Assert.IsFalse(dlg.FindButton.Enabled);
            }
        }

    }
}
