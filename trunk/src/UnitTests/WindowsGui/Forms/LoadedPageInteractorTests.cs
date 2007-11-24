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
using Decompiler.Gui;
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
	[TestFixture]
	public class LoadedPageInteractorTests
	{
		private MainForm form;
		private TestMainFormInteractor mi;

		[SetUp]
		public void Setup()
		{
			form = new MainForm();
			mi = new TestMainFormInteractor(form, BuildFakeProgram());
			mi.OpenBinary(null);
		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

		[Test]
		public void Populate()
		{
			TreeView tv = mi.MainForm.BrowserTree;
			Assert.AreEqual(3, tv.Nodes.Count);
		}

		[Test]
		public void SelectBrowserItem()
		{
			form.BrowserTree.SelectedNode = form.BrowserTree.Nodes[1];
			mi.OnBrowserItemSelected(null, null);
			Assert.AreEqual(new Address(0xC10, 0), form.LoadedPage.MemoryControl.TopAddress);
		}

		[Test]
		public void QueryStatus()
		{
			Assert.AreEqual(MenuStatus.Enabled|MenuStatus.Visible, QueryStatus(CmdIds.ViewFindFragments));
		}

		private MenuStatus QueryStatus(int cmdId)
		{
			CommandStatus status = new CommandStatus();
			mi.LoadedPageInteractor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null);
			return status.Status;
		}

		private Program BuildFakeProgram()
		{
			Program prog = new Program();
			prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
			prog.Image.Map.AddSegment(new Address(0x0C10, 0), null, AccessMode.ReadWrite);
			prog.Image.Map.AddSegment(new Address(0x0C20, 0), null, AccessMode.ReadWrite);
			return prog;
		}
	}
}
