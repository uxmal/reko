/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.UnitTests.Mocks;
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
			ListView lv = mi.MainForm.BrowserList;
			Assert.AreEqual(3, lv.Items.Count);
		}

		[Test]
		public void PopulateBrowserWithScannedProcedures()
		{
			mi.Program.Procedures.Add(new Address(0xC20, 0), new Procedure("Test1", new Frame(mi.Program.Architecture.WordWidth)));
			mi.Program.Procedures.Add(new Address(0xC20, 2), new Procedure("Test2", new Frame(mi.Program.Architecture.WordWidth)));
			form.PhasePage = form.LoadedPage;
			Assert.AreEqual(3, form.BrowserList.Items.Count);
			Assert.AreEqual("0C20", form.BrowserList.Items[2].Text);
		}

		[Test]
		public void MarkingProceduresShouldAddToUserProceduresList()
		{
			Assert.AreEqual(0, mi.Decompiler.Project.UserProcedures.Count);
			mi.MainForm.LoadedPage.MemoryControl.SelectedAddress = new Address(0x0C20, 0);
			mi.LoadedPageInteractor.MarkAndScanProcedure();
			Assert.AreEqual(1, mi.Decompiler.Project.UserProcedures.Count);
			SerializedProcedure uproc = (SerializedProcedure) mi.Decompiler.Project.UserProcedures[0];
			Assert.AreEqual("0C20:0000", uproc.Address);
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
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
			prog.Image.Map.AddSegment(new Address(0x0C10, 0), "0C10", AccessMode.ReadWrite);
			prog.Image.Map.AddSegment(new Address(0x0C20, 0), "0C20", AccessMode.ReadWrite);
			return prog;
		}
	}
}
