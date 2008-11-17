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
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.WindowsGui.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Decompiler.UnitTests.WindowsGui.Forms
{
    [TestFixture]
    public class AnalyzedPageInteractorTests
    {
        private Program prog;
        private DecompilerDriver decompiler;
        private IMainForm form;
        private TestMainFormInteractor main;
        private TestAnalyzedPageInteractor interactor;
    

        [SetUp]
        public void Setup()
        {
            prog = new Program();
            TestLoader ldr = new TestLoader(prog);
            form = new MainForm2();
            main = new TestMainFormInteractor(form, prog, ldr);
            interactor = new TestAnalyzedPageInteractor(prog, form.AnalyzedPage, main);
            main.OpenBinary("");
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
        }

        [Test]
        public void Populate()
        {
            form.Show();
            prog.Procedures.Add(new Address(0x12345), new Procedure("foo", new Frame(PrimitiveType.Word32)));
            main.SwitchInteractor(interactor);
            Assert.IsTrue(form.BrowserList.Visible, "Browserlist should be visible");

            Assert.AreEqual(1, form.BrowserList.Items.Count);
            KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) form.BrowserList.Items[0].Tag;
            Assert.AreEqual(0x12345, entry.Key.Offset);
            Assert.AreEqual("foo", entry.Value.Name);

        }

        [Test]
        public void SelectProcedure()
        {
            form.Show();
            Procedure p = new Procedure("foo_proc", new Frame(PrimitiveType.Word32));
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });

            interactor.Decompiler.Program.Procedures.Add(new Address(0x12345), new Procedure("bar", new Frame(PrimitiveType.Word32)));
            interactor.Decompiler.Program.Procedures.Add(new Address(0x12346), p);
            main.SwitchInteractor(interactor);
            form.BrowserList.Items[1].Focused = true;
            form.BrowserList.Items[1].Selected = true;
            Assert.IsTrue(form.AnalyzedPage.ProcedureText.Text.StartsWith("word32 foo_proc"));
        }

        [Test]
        public void ShowEditProcedureDialog()
        {
            form.Show();
            Procedure p = new Procedure("foo_proc", new Frame(PrimitiveType.Word32));
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });
            interactor.Decompiler.Program.Procedures.Add(new Address(0x12345), new Procedure("bar", new Frame(PrimitiveType.Word32)));
            main.SwitchInteractor(interactor);
            form.BrowserList.Items[0].Selected = true;
            Assert.IsTrue(interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ActionEditSignature), "Should have executed command.");
            Assert.AreSame(typeof(ProcedureDialog), interactor.ProbeLastShownDialog);
        }

        private class TestAnalyzedPageInteractor : AnalyzedPageInteractor
        {
            private Form lastDlg; 

            public TestAnalyzedPageInteractor(Program prog, AnalyzedPage page, MainFormInteractor main)
                : base(page, main)
            {
            }

            public override DialogResult ShowModalDialog(Form dlg)
            {
                lastDlg = dlg;
                return DialogResult.OK;
            }

            public Form ProbeLastShownDialog
            {
                get { return lastDlg; }
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
                Program.Image = new ProgramImage(new Address(0x1234), new byte[4211]);
                Program.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
                return new DecompilerProject();
            }
        }
    }
}
