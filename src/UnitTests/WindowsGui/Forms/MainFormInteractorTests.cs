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
using Decompiler.Loading;
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
			form = new MainForm();
		}

		[TearDown]
		public void TearDown()
		{
			form.Dispose();
		}

		[Test]
		public void CreateForm()
		{
			interactor = new TestMainFormInteractor(form, (Program) null);
			// When opening the application for the very first time, we should be on the initial page, and 
			// most controls on the mainform should be disabled.

			Assert.IsFalse(form.BrowserList.Enabled, "Browser list should be disabled");

			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPage);
		}

		[Test]
		public void OpenBinary()
		{
			Program prog = CreateFakeProgram();
			interactor = new TestMainFormInteractor(form, prog);

			interactor.OpenBinary(null);
			Assert.IsTrue(form.BrowserList.Enabled);
		}

		[Test]
		public void NextPhase()
		{
			Program prog = CreateFakeProgram();
			interactor = new TestMainFormInteractor(form, prog);
			interactor.OpenBinary(null);
			Assert.AreSame(interactor.LoadedPageInteractor, interactor.CurrentPage);
			interactor.NextPhase();
			Assert.AreSame(interactor.AnalyzedPageInteractor, interactor.CurrentPage);
		}

        [Test]
        public void Save()
        {
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(form, prog);

            interactor.OpenBinary("foo.exe");
            Decompiler.Core.Serialization.SerializedProcedure p = new Decompiler.Core.Serialization.SerializedProcedure();
            p.Address = "12345";
            p.Name = "MyProc";
            interactor.Decompiler.Project.UserProcedures.Add(p);
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
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(form, prog);
            Assert.IsNull(interactor.ProjectFileName);
            interactor.OpenBinary("foo.exe");
            Assert.AreEqual("foo.exe", interactor.Decompiler.Project.Input.Filename);
            Assert.IsTrue(string.IsNullOrEmpty(interactor.ProjectFileName), "project filename should be clear");
            interactor.Save();
            Assert.IsTrue(interactor.ProbePromptedForSaving, "Should have prompted for saving as no file name was supplied.");
            Assert.AreEqual("foo.dcproject", interactor.ProbeFilename);
        }

		private Program CreateFakeProgram()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			prog.Image = new ProgramImage(new Address(0xC00, 0), new byte [300]);
			return prog;
		}

        [Test]
        public void GetDiagnostics()
        {
            Program prog = new Program();
            interactor = new TestMainFormInteractor(form, prog, new FakeLoader("fake.exe", prog));
            object oSvc = interactor.GetService(typeof(IDiagnosticsService));
            Assert.IsNotNull(oSvc, "IDiagnosticsService should be available!");
            IDiagnosticsService svc = (IDiagnosticsService) oSvc;
            svc.AddDiagnostic(Diagnostic.Warning, new Address(0x30000), "Whoa");
            Assert.AreEqual(1, form.DiagnosticsList.Items.Count, "Should have added an item to diagnostics list.");
        }

        [Test]
        public void IsNextPhaseEnabled()
        {
            Program prog = new Program();
            interactor = new TestMainFormInteractor(form, prog, new FakeLoader("fake.exe", prog));
            CommandStatus status = QueryStatus(CmdIds.ActionNextPhase);
            Assert.IsNotNull(status, "MainFormInteractor should know this command.");
            Assert.AreEqual(MenuStatus.Visible, status.Status);
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

		public TestMainFormInteractor(IMainForm form, Program prog) : base(form)
		{
            this.program = prog;
		}

		public TestMainFormInteractor(IMainForm form, DecompilerDriver decompiler) : base(form)
		{
            this.decompiler = decompiler;
		}

        public TestMainFormInteractor(IMainForm form, Program prog, LoaderBase ldr)
            : base(form)
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


		public override void ShowError(string format, params object [] args)
		{
			throw new ApplicationException(string.Format(format, args));
        }

        protected override string PromptForFilename(string suggestedName)
        {
            promptedForSaving = true;
            testFilename = suggestedName;
            return suggestedName;
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

	public class FakeLoader : LoaderBase
	{
        private string filename;
        private IProcessorArchitecture arch;
        private ProgramImage image;


		public FakeLoader(string filename, Program p) : base(p)
		{
            this.filename = filename;
		}

        public IProcessorArchitecture Architecture
        {
            get { return arch; }
        }


        public ProgramImage Image
        {
            get { return Image; }
        }

        public override DecompilerProject Load(Address addrLoad)
        {
            DecompilerProject project = new DecompilerProject();
            SetDefaultFilenames(filename, project);
            if (arch == null)
            {
                arch = new IntelArchitecture(ProcessorMode.Real);
            }
            Program.Architecture = Architecture;

            if (addrLoad == null)
            {
                addrLoad = new Address(0xC00, 0);
            }
            if (image == null)
            {
                image = new ProgramImage(addrLoad, new byte[300]);
            }
            Program.Image = image;
            return project;

        }
	}
}
