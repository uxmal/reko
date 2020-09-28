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
using Reko.UserInterfaces.WindowsForms.Controls;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class MemoryControlTests
	{
        private Form form;
		private byte [] bytes;
        private MemoryControl memctl;
        private ServiceContainer sc;

		public MemoryControlTests()
		{
			bytes = GenerateTestMemory();
		}

		[SetUp]
		public void Setup()
		{
            sc = new ServiceContainer();
            form = new Form();
            form.Size = new Size(300, 200);
            memctl = new MemoryControl();
            memctl.Dock = DockStyle.Fill;
            form.Controls.Add(memctl);
            memctl.Services = sc;
		}

        [TearDown]
        public void Teardown()
        {
            form.Dispose();
        }

        [Test]
        public void MemCtl_Invalidate()
        {
            memctl.Invalidate();
        }

        [Test]
        public void MemCtl_SetSelectedAddressShouldResetAnchor()
        {
            memctl.SelectedAddress = Address.Ptr32(0x010);
            AddressRange ar = memctl.GetAddressRange();
            Assert.AreEqual(0x010, ar.Begin.ToLinear());
            Assert.AreEqual(0x010, ar.End.ToLinear());
        }

        [Test]
        public void MemCtl_ChangeSelection_RaisesSelectionServiceEvent()
        {
            var selSvc = new Mock<ISelectionService>();
            selSvc.Setup(s => s.SetSelectedComponents(
                It.Is<ICollection>(c => ExpectedRange(c, 0x10, 0x10))))
                .Verifiable();
            sc.AddService<ISelectionService>(selSvc.Object);

            memctl.Services = sc;
            selSvc.Object.SelectionChanged += delegate { };
            memctl.SelectedAddress = Address.Ptr32(0x10);

            selSvc.VerifyAll();
        }

        private bool ExpectedRange(ICollection c, ulong uAddrBegin, ulong uAddrEnd)
        {
            var ar = c.Cast<AddressRange>().First();
            Assert.AreEqual(uAddrBegin, ar.Begin.ToLinear());
            Assert.AreEqual(uAddrEnd, ar.End.ToLinear());
            return true;
        }

		private byte [] GenerateTestMemory()
		{
			System.IO.MemoryStream stm = new System.IO.MemoryStream();
			for (int i = 0; i < 1024; ++i)
			{
				stm.WriteByte((byte)i);
			}
			return stm.ToArray();
		}
	}
}
