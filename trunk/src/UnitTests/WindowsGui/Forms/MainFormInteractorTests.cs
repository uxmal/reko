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
	public class MainFormInteractorTests
	{
		private string execCommand;

		[Test]
		public void CreateForm()
		{
			// When opening the application for the very first time, we should be on the initial page, and 
			// most controls on the mainform should be disabled.

			using (MainForm form = new MainForm())
			{
				MainFormInteractor i = new MainFormInteractor(form);
				Assert.IsFalse(form.BrowserFilter.Enabled, "Browser filter should be disabled");
				Assert.IsFalse(form.BrowserTree.Enabled, "Browser tree should be disabled");
				Assert.IsFalse(form.BrowserList.Enabled, "Browser list should be disabled");

				Assert.AreSame(form.InitialPage, form.PhasePage);
			}
		}

		[Test]
		public void CommandRouting()
		{
			TestCommandTarget tgt0 = new TestCommandTarget("Hello", this);
			TestCommandTarget tgt1 = new TestCommandTarget("World", this);
			using (MainForm form = new MainForm())
			{
				MainFormInteractor i = new MainFormInteractor(form);
				i.CommandTarget = tgt0;
				Guid gid = new Guid("00001111-2222-3333-4444-555566667777");
				i.Execute(ref gid, 0);
				Assert.AreEqual("Hello", ExecutedCommand);
				i.CommandTarget = tgt1;
				i.Execute(ref gid, 0);
				Assert.AreEqual("World", ExecutedCommand);
			}
		}

		[Test]
		public void OpenBinary()
		{
			using (MainForm form = new MainForm())
			{
				Program prog = new Program();
				prog.Image = new ProgramImage(new Address(0xC00, 0), new byte [300]);
				prog.ImageMap = new ImageMap(prog.Image);
				MainFormInteractor i = new TestMainFormInteractor(form, prog);
				i.OpenBinary(null);
				Assert.AreEqual(form.LoadedPage, form.PhasePage);
				Assert.IsFalse(form.BrowserFilter.Enabled);
				Assert.IsTrue(form.BrowserTree.Enabled);
				Assert.IsFalse(form.BrowserList.Visible);
			}
		}


		public string ExecutedCommand
		{
			get { return execCommand; }
			set { execCommand = value; }
		}

		private class TestCommandTarget : ICommandTarget
		{
			private string message;
			private MainFormInteractorTests test;

			public TestCommandTarget(string message, MainFormInteractorTests test)
			{
				this.message = message;
				this.test = test;
			}

			#region ICommandTarget Members

			public bool Execute(ref Guid cmdSet, int cmdId)
			{
				test.ExecutedCommand = message;
				return true;
			}

			public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
			{
				return false;
			}

			#endregion
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

		public TestMainFormInteractor(TestDecompiler test)
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
