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
using Decompiler.WindowsGui;
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
        private FakeUiService uiSvc;
        private Program prog;
        private LoadedPageInteractor interactor;
        private DecompilerService decSvc;

		[SetUp]
		public void Setup()
		{
            form = new MainForm();
            interactor = new LoadedPageInteractor(form.LoadedPage);

            FakeComponentSite site = new FakeComponentSite(interactor);

            uiSvc = new FakeUiService();
            site.AddService(typeof(IDecompilerUIService), uiSvc);

            ProgramImageBrowserService svc = new ProgramImageBrowserService(form.BrowserList);
            site.AddService(typeof(IProgramImageBrowserService), svc);

            prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[10000]);
            prog.Image.Map.AddSegment(new Address(0x0C10, 0), "0C10", AccessMode.ReadWrite);
            prog.Image.Map.AddSegment(new Address(0x0C20, 0), "0C20", AccessMode.ReadWrite);

            TestLoader ldr = new TestLoader(prog);
            decSvc = new DecompilerService();
            decSvc.Decompiler = new DecompilerDriver(ldr, prog, new FakeDecompilerHost());
            decSvc.Decompiler.LoadProgram();
            site.AddService(typeof(IDecompilerService), decSvc);

            interactor.Site = site;
		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

		[Test]
		public void Populate()
		{
            interactor.EnterPage();
			ListView lv = form.BrowserList;
			Assert.AreEqual(3, lv.Items.Count, "There should be three segments in the image.");
		}

		[Test]
		public void PopulateBrowserWithScannedProcedures()
		{
            AddProcedure(new Address(0xC20, 0x0000), "Test1");
            AddProcedure(new Address(0xC20, 0x0002), "Test2");
            interactor.EnterPage();
            Assert.AreEqual(3, form.BrowserList.Items.Count);
			Assert.AreEqual("0C20", form.BrowserList.Items[2].Text);
		}

        private void AddProcedure(Address addr, string procName)
        {
            Program prog = decSvc.Decompiler.Program;
            decSvc.Decompiler.Program.Procedures.Add(addr, 
                new Procedure(procName, new Frame(prog.Architecture.WordWidth)));
        }

		[Test]
		public void MarkingProceduresShouldAddToUserProceduresList()
		{
			Assert.AreEqual(0, decSvc.Decompiler.Project.UserProcedures.Count);
            form.LoadedPage.MemoryControl.SelectedAddress = new Address(0x0C20, 0);
			interactor.MarkAndScanProcedure();
			Assert.AreEqual(1, decSvc.Decompiler.Project.UserProcedures.Count);
			SerializedProcedure uproc = (SerializedProcedure) decSvc.Decompiler.Project.UserProcedures[0];
			Assert.AreEqual("0C20:0000", uproc.Address);
		}

		[Test]
		public void QueryStatus()
		{
			Assert.AreEqual(MenuStatus.Enabled|MenuStatus.Visible, QueryStatus(CmdIds.ViewFindFragments));
		}

        [Test]
        public void ShowEditFindDialogButDontRunIt()
        {
            uiSvc.SimulateUserCancel = true;
            interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.EditFind);
            Assert.AreSame(typeof(FindDialog), uiSvc.ProbeLastShownDialog.GetType());
        }

		private MenuStatus QueryStatus(int cmdId)
		{
			CommandStatus status = new CommandStatus();
			interactor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null);
			return status.Status;
		}


        private class TestLoader : LoaderBase
        {
            public TestLoader(Program prog)
                : base(prog)
            {
            }

            public override DecompilerProject Load(Address userSpecifiedAddress)
            {
                return new DecompilerProject();

            }
        }
	}
}
