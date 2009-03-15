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
using System.IO;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
	[TestFixture]
	public class MainFormInteractorTests
	{
		private IMainForm form;
		private TestMainFormInteractor interactor;

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
			form.Dispose();
		}

		[Test]
		public void CreateForm()
		{
            CreateMainFormInteractor();

			// When opening the application for the very first time, we should be on the initial page, and 
			// most controls on the mainform should be disabled.

			Assert.IsFalse(form.BrowserList.Enabled, "Browser list should be disabled");

			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPage);
		}

		[Test]
		public void OpenBinary()
		{
            CreateMainFormInteractor();
			interactor.OpenBinary(null);
			Assert.IsTrue(form.BrowserList.Enabled, "Browser list should have been enabled after opening binary.");
		}

		[Test]
		public void NextPhase()
		{
            CreateMainFormInteractor();
			interactor.OpenBinary(null);
			Assert.AreSame(interactor.LoadedPageInteractor, interactor.CurrentPage);
			interactor.NextPhase();
			Assert.AreSame(interactor.AnalyzedPageInteractor, interactor.CurrentPage);
		}


        [Test]
        public void Save()
        {
            CreateMainFormInteractor();

            interactor.OpenBinary("foo.exe");
            Decompiler.Core.Serialization.SerializedProcedure p = new Decompiler.Core.Serialization.SerializedProcedure();
            p.Address = "12345";
            p.Name = "MyProc";
            IDecompilerService svc = (IDecompilerService) interactor.ProbeGetService(typeof(IDecompilerService));
            svc.Decompiler.Project.UserProcedures.Add(p);
            interactor.Save();
            string s =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://schemata.jklnet.org/Decompiler"">
  <input>
    <filename>foo.exe</filename>
  </input>
  <output>
    <disassembly>foo.asm</disassembly>
    <intermediate-code>foo.dis</intermediate-code>
    <output>foo.c</output>
    <type-inference>false</type-inference>
    <types-file>foo.h</types-file>
  </output>
  <procedure name=""MyProc"">
    <address>12345</address>
  </procedure>
</project>";
            Console.WriteLine(interactor.ProbeSavedProjectXml);

            Assert.AreEqual(s, interactor.ProbeSavedProjectXml);
        }

        [Test]
        public void SaveShouldShowDialog()
        {
            CreateMainFormInteractor();
            Assert.IsNull(interactor.ProjectFileName);
            interactor.OpenBinary("foo.exe");
            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();
            Assert.IsTrue(interactor.ProbePromptedForSaving, "Should have prompted for saving as no file name was supplied.");
            Assert.AreEqual("foo.dcproject", interactor.ProbeFilename);
        }

        [Test]
        public void GetDiagnostics()
        {
            CreateMainFormInteractorWithLoader();
            object oSvc = interactor.ProbeGetService(typeof(IDiagnosticsService));
            Assert.IsNotNull(oSvc, "IDiagnosticsService should be available!");
            IDiagnosticsService svc = (IDiagnosticsService) oSvc;
            svc.AddDiagnostic(Diagnostic.Warning, new Address(0x30000), "Whoa");
            Assert.AreEqual(1, form.DiagnosticsList.Items.Count, "Should have added an item to diagnostics list.");
        }

        [Test]
        public void DecompilerServiceInstalled()
        {
            CreateMainFormInteractor();
            Assert.IsNotNull(interactor.ProbeGetService(typeof (IDecompilerService)), "Should have IDecompilerService available.");
        }

        [Test]
        public void IsNextPhaseEnabled()
        {
            CreateMainFormInteractorWithLoader();
            CommandStatus status = QueryStatus(CmdIds.ActionNextPhase);
            Assert.IsNotNull(status, "MainFormInteractor should know this command.");
            Assert.AreEqual(MenuStatus.Visible, status.Status);
        }

        private Program CreateFakeProgram()
        {
            Program prog = new Program();
            prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            prog.Image = new ProgramImage(new Address(0xC00, 0), new byte[300]);
            return prog;
        }

        private void CreateMainFormInteractorWithLoader()
        {
            Program prog = new Program();
            interactor = new TestMainFormInteractor(prog, new FakeLoader("fake.exe", prog));
            form = interactor.CreateForm();
        }

        private void CreateMainFormInteractor()
        {
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(prog);
            form = interactor.CreateForm();
        }

        //$REFACTOR: copied from LoadedPageInteractor, should
        // push to base class or utility class.
        private CommandStatus QueryStatus(int cmdId)
        {
            CommandStatus status = new CommandStatus();
            if (interactor.QueryStatus(ref CmdSets.GuidDecompiler, cmdId, status, null))
                return status;
            else
                return null;
        }

	}

	public class TestMainFormInteractor : MainFormInteractor
	{
		private DecompilerDriver decompiler;
        private LoaderBase ldr;
		private Program program;
        private StringWriter sw;
        private string testFilename;
        private bool promptedForSaving;

		public TestMainFormInteractor(Program prog)
		{
            this.program = prog;
		}

		public TestMainFormInteractor(DecompilerDriver decompiler)
		{
            this.decompiler = decompiler;
		}

        public TestMainFormInteractor(Program prog, LoaderBase ldr)
        {
            this.program = prog;
            this.ldr = ldr;
        }


        public override DecompilerDriver CreateDecompiler(LoaderBase ldr, Program prog)
		{
            if (decompiler != null)
                return decompiler;
            return base.CreateDecompiler(ldr, prog);
		}

        protected override LoaderBase CreateLoader(string filename, Program prog)
        {
            if (ldr != null)
                return ldr;
            return new FakeLoader(filename, prog);
        }

        public override Program CreateProgram()
		{
			return program;
		}

        public override TextWriter CreateTextWriter(string filename)
        {
            testFilename = filename;
            sw = new StringWriter();
            return sw;
        }

        protected override string PromptForFilename(string suggestedName)
        {
            promptedForSaving = true;
            testFilename = suggestedName;
            return suggestedName;
        }

        public object ProbeGetService(Type service)
        {
            return base.GetService(service);
        }
        public string ProbeSavedProjectXml
        {
            get { return sw.ToString(); }
        }

        public string ProbeFilename
        {
            get { return testFilename; }
        }


        public bool ProbePromptedForSaving
        {
            get { return promptedForSaving; }
        }

    }
}
