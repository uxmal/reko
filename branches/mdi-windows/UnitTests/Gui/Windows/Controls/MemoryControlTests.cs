/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Windows.Controls;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Controls
{
	[TestFixture]
	public class MemoryControlTests
	{
        private Form form;
		private byte [] bytes;
        private MemoryControl memctl;

		public MemoryControlTests()
		{
			bytes = GenerateTestMemory();
		}

		[SetUp]
		public void Setup()
		{
            form = new Form();
            form.Size = new Size(300, 200);
            memctl = new MemoryControl();
            memctl.Dock = DockStyle.Fill;
            form.Controls.Add(memctl);
		}

        [TearDown]
        public void Teardown()
        {
            form.Dispose();
        }

        [Test]
        public void Invalidate()
        {
            memctl.Invalidate();
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
