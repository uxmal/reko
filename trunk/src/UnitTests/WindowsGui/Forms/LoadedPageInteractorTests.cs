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

using Decompiler.Arch.Intel;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Gui;
using Decompiler.Loading;
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
            Program prog = new Program();
            mi = new TestMainFormInteractor(prog, new TestLoader(prog));
            form = mi.CreateForm();
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
			Assert.AreEqual(3, lv.Items.Count, "There should be three segments in the image.");
		}

		[Test]
		public void PopulateBrowserWithScannedProcedures()
		{
            AddProcedure(mi, new Address(0xC20, 0), "Test1");
            AddProcedure(mi, new Address(0xC20, 2), "Test2");
			form.SetCurrentPage(form.LoadedPage);
			Assert.AreEqual(3, form.BrowserList.Items.Count);
			Assert.AreEqual("0C20", form.BrowserList.Items[2].Text);
		}

        private void AddProcedure(TestMainFormInteractor mi, Address addr, string procName)
        {
            IDecompilerService svc = (IDecompilerService) mi.ProbeGetService(typeof(IDecompilerService));
            Program prog = svc.Decompiler.Program;
            svc.Decompiler.Program.Procedures.Add(addr, 
                new Procedure(procName, new Frame(prog.Architecture.WordWidth)));
        }

		[Test]
		public void MarkingProceduresShouldAddToUserProceduresList()
		{
            IDecompilerService svc = (IDecompilerService) mi.ProbeGetService(typeof(IDecompilerService));
			Assert.AreEqual(0, svc.Decompiler.Project.UserProcedures.Count);
            mi.MainForm.LoadedPage.MemoryControl.SelectedAddress = new Address(0x0C20, 0);
			mi.LoadedPageInteractor.MarkAndScanProcedure();
			Assert.AreEqual(1, svc.Decompiler.Project.UserProcedures.Count);
			SerializedProcedure uproc = (SerializedProcedure) svc.Decompiler.Project.UserProcedures[0];
			Assert.AreEqual("0C20:0000", uproc.Address);
		}

		[Test]
		public void QueryStatus()
		{
			Assert.AreEqual(MenuStatus.Enabled|MenuStatus.Visible, QueryStatus(CmdIds.ViewFindFragments));
		}

        [Test]
        public void ExecuteEditFind()
        {
            TestLoadedPageInteractor i = new TestLoadedPageInteractor((LoadedPage)form.LoadedPage, mi, new DecompilerMenus(mi));
            i.Execute(ref CmdSets.GuidDecompiler, CmdIds.EditFind);
            Assert.AreSame(typeof(FindDialog), i.ProbeLastDialogShown);
        }

		private MenuStatus QueryStatus(int cmdId)
		{
			CommandStatus status = new CommandStatus();
			mi.LoadedPageInteractor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null);
			return status.Status;
		}


        private class TestLoadedPageInteractor : LoadedPageInteractor
        {
            private Type lastDialogType;

            public TestLoadedPageInteractor(LoadedPage page, MainFormInteractor mainInteractor, DecompilerMenus menus)
                : base(page, mainInteractor, menus)
            {
            }
            public override DialogResult ShowModalDialog(Form dlg)
            {
                lastDialogType = dlg.GetType();
                return DialogResult.OK;
            }

            public Type ProbeLastDialogShown
            {
                get { return lastDialogType; }
            }
        }

        private class TestLoader : LoaderBase
        {
            public TestLoader(Program prog)
                : base(prog)
            {
            }

            public override DecompilerProject Load(Address userSpecifiedAddress)
            {
                Program.Architecture = new IntelArchitecture(ProcessorMode.Real);
                Program.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
                Program.Image.Map.AddSegment(new Address(0x0C10, 0), "0C10", AccessMode.ReadWrite);
                Program.Image.Map.AddSegment(new Address(0x0C20, 0), "0C20", AccessMode.ReadWrite);
                return new DecompilerProject();

            }
        }
	}
}
