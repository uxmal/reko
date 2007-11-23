/* 
 * Copyright (C) 1999-2007 John Källén.
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
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
	[TestFixture]
	public class LoadedPageInteractorTests
	{
		[Test]
		public void Populate()
		{
			using (MainForm form = new MainForm())
			{
				Program prog = BuildFakeProgram();

				TestMainFormInteractor mi = new TestMainFormInteractor(form, prog);
				mi.OpenBinary(null);
				TreeView tv = mi.MainForm.BrowserTree;
				Assert.AreEqual(3, tv.Nodes.Count);
			}
		}

		[Test]
		public void SelectBrowserItem()
		{
			using (MainForm form = new MainForm())
			{
				Program prog = BuildFakeProgram();
				MainFormInteractor mi = new TestMainFormInteractor(form, prog);
				mi.OpenBinary(null);
				form.BrowserTree.SelectedNode = form.BrowserTree.Nodes[1];
				mi.OnBrowserItemSelected(null, null);
				Assert.AreEqual(new Address(0xC10, 0), form.LoadedPage.MemoryControl.TopAddress);
			}
		}

		private Program BuildFakeProgram()
		{
			Program prog = new Program();
			prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
			prog.ImageMap = new ImageMap(prog.Image);
			prog.ImageMap.AddSegment(new Address(0x0C10, 0), null, AccessMode.ReadWrite);
			prog.ImageMap.AddSegment(new Address(0x0C20, 0), null, AccessMode.ReadWrite);
			return prog;
		}
	}
}
