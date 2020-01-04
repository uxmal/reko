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

using NUnit.Framework;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms.Controls;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class StyleStackTests
    {
        private Mock<IUiPreferencesService> uiPrefs;
        private Dictionary<string, UiStyle> styles;

        [SetUp]
        public void Setup()
        {
            this.uiPrefs = new Mock<IUiPreferencesService>();
            this.styles = new Dictionary<string, UiStyle>();
            this.uiPrefs.Setup(u => u.Styles).Returns(styles);
        }

        [Test]
        public void Stst_Default()
        {
            var stst = new StyleStack(uiPrefs.Object);

            var br = stst.GetBackground(Color.FromArgb(0x12, 0x23, 0x45));
            Assert.AreEqual(0x12, br.Color.R);
            Assert.AreEqual(0x23, br.Color.G);
            Assert.AreEqual(0x45, br.Color.B);
        }

        [Test]
        public void Stst_SingleStyle()
        {
            styles.Add("style1", new UiStyle {
                Background = new SolidBrush(Color.FromArgb(0x12, 0x34, 0x56))
            });

            var stst = new StyleStack(uiPrefs.Object);
            stst.PushStyle("style1");

            var br = stst.GetBackground(Color.Black);
            Assert.AreEqual(0x12, br.Color.R);
            Assert.AreEqual(0x34, br.Color.G);
            Assert.AreEqual(0x56, br.Color.B);
        }

        [Test]
        public void Stst_DoubleStyles()
        {
            styles.Add("style1", new UiStyle
            {
                Background = new SolidBrush(Color.FromArgb(0x12, 0x34, 0x56))
            });
            styles.Add("style2", new UiStyle
            {
                PaddingBottom = 6.0f
            });

            var stst = new StyleStack(uiPrefs.Object);
            stst.PushStyle("style1 style2");

            var br = stst.GetBackground(Color.Black);
            Assert.AreEqual(0x12, br.Color.R);
            Assert.AreEqual(0x34, br.Color.G);
            Assert.AreEqual(0x56, br.Color.B);
            var padding = stst.GetNumber(u => u.PaddingBottom);
            Assert.AreEqual(6.0f, padding);
        }
    }
}
