/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Gui;
using Decompiler.WindowsGui;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;

namespace Decompiler.UnitTests.Gui
{
	[TestFixture]
	public class MainFormInteractorTests
	{
		private FakeMainForm mf;

		[SetUp]
		public void SetUp()
		{
			mf = new FakeMainForm();
		}

		[Test]
		public void Create()
		{
			MainFormInteractorQ interactor = new MainFormInteractorQ(mf, new string [] { "testFile.exe" });
		}

		[Test]
		[Ignore("Revisit these tests when we skin the interface off MainForm")]
		public void HandleMenuEvents()
		{
			MainFormInteractorQ interactor = new TestMainFormInteractor(mf, new string [] { "testFile.exe" });
			mf.SendMenuEvent(TestMainFormInteractor.testCmd);		// Command to set the main form's command line to 'hello'.
			Assert.AreEqual("hello", mf.TitleText);
		}
	}

	public class FakeMainForm : IMainForm
	{
		private string titleText;

		public string TitleText 
		{
			get { return titleText; }
			set { titleText = value; }
		}

		public event MenuCommandHandler CommandInvoked;

		// Fake methods

		public void SendMenuEvent(CommandID cmd)
		{
			if (CommandInvoked != null)
			{
				CommandInvoked(this, new MenuCommandArgs(new MenuCommand(null, cmd)));
			}
		}
	}


	/// <summary>
	/// MainFormInteractor with some testing extensions.
	/// </summary>
	public class TestMainFormInteractor : MainFormInteractor
	{
		public static readonly CommandID testCmd = new CommandID(new Guid("0123456789ABCDEF0123456789ABCDEF"), 42);
		
		public TestMainFormInteractor(IMainForm form ,string [] args) : base (form, args)
		{
		}

		public override bool Execute(ref Guid cmdSet, int cmdId)
		{
			if (cmdSet == testCmd.Guid)
			{
				if (cmdId==testCmd.ID)
				{
					MainForm.TitleText = "hello";
					return true;
				}
			}
			return base.Execute(ref cmdSet, cmdId);
		}
	}
}
