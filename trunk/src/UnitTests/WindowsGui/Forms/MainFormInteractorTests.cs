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
		private MainForm form;
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

			Assert.IsFalse(form.BrowserFilter.Enabled, "Browser filter should be disabled");
			Assert.IsFalse(form.BrowserTree.Enabled, "Browser tree should be disabled");
			Assert.IsFalse(form.BrowserList.Enabled, "Browser list should be disabled");

			Assert.AreSame(interactor.InitialPageInteractor, interactor.CurrentPage);
		}

		[Test]
		public void OpenBinary()
		{
			Program prog = CreateFakeProgram();
			interactor = new TestMainFormInteractor(form, prog);

			interactor.OpenBinary(null);
			Assert.AreEqual(form.LoadedPage, form.LoadedPage);
			Assert.IsFalse(form.BrowserFilter.Enabled);
			Assert.IsFalse(form.BrowserTree.Enabled);
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
			Assert.AreEqual(interactor.AnalyzedPageInteractor, interactor.CurrentPage);
		}

        [Test]
        public void Save()
        {
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(form, prog);

            interactor.OpenBinary("foo.project");
            Decompiler.Core.Serialization.SerializedProcedure p = new Decompiler.Core.Serialization.SerializedProcedure();
            p.Address = "12345";
            p.Name = "MyProc";
            interactor.Decompiler.Project.UserProcedures.Add(p);
            interactor.Save();
            string s = 
@"<?xml version=""1.0"" encoding=""utf-16""?>
<project xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""http://schemata.jklnet.org/Decompiler"">
  <input>
    <filename />
    <address>0C00:0000</address>
  </input>
  <output>
    <disassembly />
    <intermediate-code />
    <output />
    <type-inference>false</type-inference>
    <types-file />
  </output>
  <procedure name=""MyProc"">
    <address>12345</address>
  </procedure>
</project>";
            Console.WriteLine(interactor.TestSavedProjectXml);

            Assert.AreEqual(s, interactor.TestSavedProjectXml);
        }

        [Test]
        public void SaveShouldShowDialog()
        {
            Program prog = CreateFakeProgram();
            interactor = new TestMainFormInteractor(form, prog);
            Assert.IsNull(interactor.ProjectFileName);
            interactor.OpenBinary("foo.exe");
            interactor.Save();
            Assert.IsTrue(interactor.TestPromptedForSaving);
            Assert.AreEqual("foo.project", interactor.ProjectFileName);
        }

		private Program CreateFakeProgram()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			prog.Image = new ProgramImage(new Address(0xC00, 0), new byte [300]);
			return prog;
		}
	}

	public class TestMainFormInteractor : MainFormInteractor
	{
		private DecompilerDriver decompiler; 
		private Loader ldr;
		private Program program;
        private StringWriter sw;
        private string testFilename;
        private bool promptedForSaving;

		public TestMainFormInteractor(MainForm form, Program program) : base(form)
		{
			this.program = program;
			this.ldr = new TestLoader(program);

		}

		public TestMainFormInteractor(MainForm form, DecompilerDriver test) : base(form)
		{
			decompiler = test;
			this.ldr = new TestLoader(program);
		}

		public override DecompilerDriver CreateDecompiler(string filename, Program prog)
		{
			return new TestDecompilerDriver(prog, this);
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

        public Loader Loader
		{
			get { return ldr; }
			set { ldr = value; }
		}


		public override void ShowError(string format, params object [] args)
		{
			throw new ApplicationException(string.Format(format, args));
        }

        protected override string PromptForFilename(string suggestedName)
        {
            testFilename = suggestedName;
            return suggestedName;
        }

        public string TestSavedProjectXml
        {
            get { return sw.ToString(); }
        }

        public string TestFilename
        {
            get { return testFilename; }
        }


        public bool TestPromptedForSaving
        {
            get { return promptedForSaving; }
        }
    }

	public class TestDecompilerDriver : DecompilerDriver
	{
		public TestDecompilerDriver(Program prog, DecompilerHost host) : base("", prog, host)
		{
		}

		protected override Loader CreateLoader(Program prog)
		{
			return new TestLoader(prog);
		}
	}

	public class TestLoader : Loader
	{
		public TestLoader(Program p) : base(p)
		{
		}

		public override byte[] LoadImageBytes(string fileName, int offset)
		{
			return base.Program.Image.Bytes;
		}

		public override void LoadExecutable(string pstrFileName, Address addrLoad)
		{
		}
	}
}
