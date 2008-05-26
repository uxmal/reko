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

using Decompiler.Gui;
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
	[TestFixture]
	public class InitialPageInteractorTests
	{
		private MainForm form;
		private MainFormInteractor formI;
		private InitialPage page;
		private TestInitialPageInteractor i;

		[SetUp]
		public void Setup()
		{
			form = new MainForm();
			formI = new MainFormInteractor(form);
			page = form.InitialPage;
			i = new TestInitialPageInteractor(page, formI);
		}

		[TearDown]
		public void Teardown()
		{
			form.Dispose();
		}

		[Test]
		public void ClickOnBrowseBinaryFile()
		{
			page.InputFile.Text = "foo.bar";
			i.OpenFileResult = "baz\\foo.bar";
			i.BrowseInputFile_Click(null, null);

			Assert.AreEqual("baz\\foo.bar", page.InputFile.Text);
		}

		[Test]
		public void CancelBrowseBinaryFile()
		{
			page.InputFile.Text = "foo.bar";
			i.OpenFileResult = "NIX";
			i.SimulateUserCancel = true;
			i.BrowseInputFile_Click(null, null);

			Assert.AreEqual("foo.bar", page.InputFile.Text);
		}
	}

	public class TestInitialPageInteractor : InitialPageInteractor
	{
		private string lastFileName;
		private bool simulateUserCancel;

		public TestInitialPageInteractor(InitialPage page, MainFormInteractor formI)
			: base(page, formI)
		{
		}

		public override string ShowOpenFileDialog(string fileName)
		{
			if (!simulateUserCancel)
			{
				return lastFileName;
			}
			else
				return null;

		}

		// Fake members

		public string OpenFileResult
		{
			get { return lastFileName; }
			set { lastFileName = value; }
		}


		public bool SimulateUserCancel
		{
			get { return simulateUserCancel; }
			set { simulateUserCancel = value; }
		}
	}
}
