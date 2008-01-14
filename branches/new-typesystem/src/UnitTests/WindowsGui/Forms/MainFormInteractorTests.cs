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
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
	[TestFixture]
	public class MainFormInteractorTests
	{
		private MainForm form;
		private MainFormInteractor interactor;

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
			interactor = new MainFormInteractor(form);
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
			Assert.AreEqual(form.LoadedPage, form.PhasePage);
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
			Assert.AreEqual(interactor.FinalPageInteractor, interactor.CurrentPage);
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
		private TestDecompiler decompiler; 
		private Program program;

		public TestMainFormInteractor(MainForm form, Program program) : base(form)
		{
			this.program = program;
		}

		public TestMainFormInteractor(MainForm form, TestDecompiler test) : base(form)
		{
			decompiler = test;
		}

		public override DecompilerDriver CreateDecompiler(string file)
		{
			return new TestDecompiler(program, this);
		}
	}

	public class TestDecompiler : DecompilerDriver
	{
		private Program program;

		public TestDecompiler(Program program, DecompilerHost host)  : base((string)null, program, host)
		{
			this.program = program;
		}

		public override void LoadProgram()
		{
		}
	}
}
