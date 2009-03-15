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
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class AnalyzedPageInteractorTests
    {
        private Program prog;
        private MainForm form;
        private AnalyzedPageInteractor interactor;
        private FakeUiService uiSvc;

        [SetUp]
        public void Setup()
        {
            form = new MainForm();
            interactor = new AnalyzedPageInteractor(form.AnalyzedPage);

            FakeComponentSite site = new FakeComponentSite(interactor);

            uiSvc = new FakeUiService();
            site.AddService(typeof(IDecompilerUIService), uiSvc);

            prog = new Program();
            TestLoader ldr = new TestLoader(prog);
            DecompilerService decSvc = new DecompilerService();
            decSvc.Decompiler = new DecompilerDriver(ldr, prog, new FakeDecompilerHost());
            decSvc.Decompiler.LoadProgram();
            decSvc.Decompiler.ScanProgram();
            site.AddService(typeof(IDecompilerService), decSvc);

            ProgramImageBrowserService brSvc = new ProgramImageBrowserService(form.BrowserList);
            site.AddService(typeof(IProgramImageBrowserService), brSvc);
            interactor.Site = site;
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
            interactor.EnterPage();
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
            interactor.EnterPage();
            form.BrowserList.Items[1].Focused = true;
            form.BrowserList.Items[1].Selected = true;
            Console.WriteLine(form.AnalyzedPage.ProcedureText.Text);
            Assert.AreEqual("word32 foo_proc", form.AnalyzedPage.ProcedureText.Text.Remove(15));
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
            interactor.EnterPage();
            form.BrowserList.Items[0].Selected = true;
            Assert.IsTrue(interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ActionEditSignature), "Should have executed command.");
            Assert.AreSame(typeof(ProcedureDialog), uiSvc.ProbeLastShownDialog.GetType());
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
