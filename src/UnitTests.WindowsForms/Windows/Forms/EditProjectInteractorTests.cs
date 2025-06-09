#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
#endregion

using NUnit.Framework;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;

namespace Reko.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class EditProjectInteractorTests
    {
        private EditProjectDialog dlg;

        [SetUp]
        public void Setup()
        {
            dlg = new EditProjectDialog();
        }

        [TearDown]
        public void Teardown()
        {
            if (dlg is not null)
                dlg.Dispose();
            dlg = null;
        }

        [Test]
        public void EditProjectAndCancel()
        {
            FakeShellUiService uiSvc = new FakeShellUiService();
            uiSvc.SimulateUserCancel = true;
            var project = CreateTestProject();
            var epi = new EditProjectInteractor();
            var ret = epi.EditProjectProperties(uiSvc, project, delegate(Project_v3 p)
            {
                Assert.Fail("Should not save if user cancels.");
            });
            Assert.IsFalse(ret);
            var input = (DecompilerInput_v3)project.Inputs[0];
            Assert.AreEqual("test.exe", input.Filename);
            Assert.AreEqual("test.asm", input.DisassemblyFilename);
            Assert.AreEqual("test.dis", input.IntermediateFilename);
            Assert.AreEqual("test.h", input.TypesFilename);
            Assert.AreEqual("test.c", input.OutputFilename);
        }

        [Test]
        public void EditProjectAndSave()
        {
            var uiSvc = new FakeShellUiService();
            var epi = new EditProjectInteractor();
            var p = CreateTestProject();

            var ret = epi.EditProjectProperties(uiSvc, p, delegate(Project_v3 project)
            {
                var input = (DecompilerInput_v3)project.Inputs[0];
                Assert.AreEqual("test.exe", input.Filename);
                Assert.AreEqual("test.asm", input.DisassemblyFilename);
                Assert.AreEqual("test.dis", input.IntermediateFilename);
                Assert.AreEqual("test.h",   input.TypesFilename);
                Assert.AreEqual("test.c",   input.OutputFilename);
            });
            Assert.IsTrue(ret);
        }

        [Test]
        public void ClickInputButton()
        {
            var interactor = new TestEditProjectInteractor();
            interactor.Attach(dlg);
            dlg.Show();
            interactor.FakeUiService.OpenFileResult = "booga.exe";
            interactor.UserClickBrowseBinaryFileButton();
            Assert.AreEqual("booga.exe", dlg.BinaryFilename.Text);
        }

        [Test]
        public void ClickOutputButton()
        {
            var interactor = new TestEditProjectInteractor();
            interactor.Attach(dlg);
            dlg.Show();
            interactor.FakeUiService.SaveFileResult = "fooga.exe";
            interactor.UserClickBrowseBinaryFileButton();
            Assert.AreEqual("fooga.exe", dlg.BinaryFilename.Text);
        }

        private Project_v3 CreateTestProject()
        {
            var project = new Project_v3
            {
                Inputs =
                {
                    new DecompilerInput_v3 {
                        Filename = "test.exe",
                        DisassemblyFilename = "test.asm",
                        IntermediateFilename = "test.dis",
                        TypesFilename = "test.h",
                        OutputFilename = "test.c",
                    }
                }
            };
            return project;
        }
    }

    public class TestEditProjectInteractor : EditProjectInteractor
    {
        public FakeUiService FakeUiService;

        protected override IDecompilerUIService CreateUIService(System.Windows.Forms.Form dlg, System.Windows.Forms.OpenFileDialog ofd, System.Windows.Forms.SaveFileDialog sfd)
        {
            FakeUiService = new FakeUiService();
            return FakeUiService;
        }

        internal void UserClickBrowseBinaryFileButton()
        {
            base.BrowseBinaryFileButton_Click(null, EventArgs.Empty);
        }

        internal void UserClickBrowseOutputFileButton()
        {
            base.BrowseOutputFileButton_Click(null, EventArgs.Empty);
        }
    }
}
