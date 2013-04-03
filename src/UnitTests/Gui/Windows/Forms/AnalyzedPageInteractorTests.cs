#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Gui;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class AnalyzedPageInteractorTests
    {
        private Program prog;
        private IMainForm form;
        private MockRepository repository;
        private AnalyzedPageInteractorImpl interactor;
        private IDecompilerShellUiService uiSvc;
        private ICodeViewerService codeViewSvc;
        private IMemoryViewService memViewSvc;
        private IDisassemblyViewService disasmViewSvc;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            form = new MainForm();
            repository = new MockRepository();

            sc = new ServiceContainer();
            uiSvc = AddService<IDecompilerShellUiService>();
            sc.AddService(typeof(IDecompilerUIService), uiSvc);
            codeViewSvc = AddService<ICodeViewerService>();
            memViewSvc = AddService<IMemoryViewService>();
            disasmViewSvc = AddService<IDisassemblyViewService>();

            TestLoader ldr = new TestLoader();
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            DecompilerService decSvc = new DecompilerService();
            decSvc.Decompiler = new DecompilerDriver(ldr, new FakeDecompilerHost(), sc);
            decSvc.Decompiler.LoadProgram("test.exe");
            prog = decSvc.Decompiler.Program;
            decSvc.Decompiler.ScanProgram();
            sc.AddService(typeof(IDecompilerService), decSvc);

            sc.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());

            ProgramImageBrowserService brSvc = new ProgramImageBrowserService(form.BrowserList);
            sc.AddService(typeof(IProgramImageBrowserService), brSvc);
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
        }

        [Test]
        public void Populate()
        {
            CreateInteractor();
            form.Show();
            prog.Procedures.Add(new Address(0x12345), new Procedure("foo", prog.Architecture.CreateFrame()));
            interactor.EnterPage();
            Assert.IsTrue(form.BrowserList.Visible, "Browserlist should be visible");

            Assert.AreEqual(1, form.BrowserList.Items.Count);
            KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) form.BrowserList.Items[0].Tag;
            Assert.AreEqual(0x12345, entry.Key.Offset);
            Assert.AreEqual("foo", entry.Value.Name);

        }

        private void CreateInteractor()
        {
            interactor = new AnalyzedPageInteractorImpl();
            var site = new FakeComponentSite(interactor, sc);
            interactor.Site = site;
        }

        [Test]
        public void SelectProcedure()
        {
            codeViewSvc.Expect(s => s.DisplayProcedure(
                Arg<Procedure>.Matches(proc => proc.Name == "foo_proc")));
            memViewSvc.Expect(s => s.ShowMemoryAtAddress(
                Arg<Address>.Matches(address => address.Linear == 0x12346)));

            disasmViewSvc.Expect(s => s.DisassembleStartingAtAddress(
                Arg<Address>.Matches(address => address.Linear == 0x12346)));
            repository.ReplayAll();


            CreateInteractor();
            form.Show();
            Procedure p = new Procedure("foo_proc", prog.Architecture.CreateFrame());
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });

            interactor.Decompiler.Program.Procedures.Add(new Address(0x12345), new Procedure("bar", prog.Architecture.CreateFrame()));
            interactor.Decompiler.Program.Procedures.Add(new Address(0x12346), p);
            interactor.EnterPage();

            form.BrowserList.Items[1].Focused = true;
            form.BrowserList.Items[1].Selected = true;

            repository.VerifyAll();
        }

        [Test]
        public void ShowEditProcedureDialog()
        {
            uiSvc.Expect(s => s.ShowModalDialog(
                    Arg<ProcedureDialog>.Is.TypeOf))
                .Return(DialogResult.Cancel);
            repository.ReplayAll();

            CreateInteractor();

            form.Show();
            Procedure p = new Procedure("foo_proc", prog.Architecture.CreateFrame());
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });
            interactor.Decompiler.Program.Procedures.Add(new Address(0x12345), new Procedure("bar", prog.Architecture.CreateFrame()));
            interactor.EnterPage();
            form.BrowserList.Items[0].Selected = true;

            Assert.IsTrue(interactor.Execute(ref CmdSets.GuidDecompiler, CmdIds.ActionEditSignature), "Should have executed command.");
            repository.VerifyAll();
        }


        private T AddService<T>() where T : class
        {
            var svc = repository.DynamicMock<T>();
            sc.AddService(typeof(T), svc);
            return svc;
        }

        private class TestLoader : LoaderBase
        {
            public TestLoader()
            {
            }

            public override Program Load(byte[] imageFile, Address userSpecifiedAddress)
            {
                Program prog = new Program();
                prog.Image = new ProgramImage(new Address(0x00100000), imageFile);
                prog.Architecture = new IntelArchitecture(ProcessorMode.Protected32);
                return prog;
            }

            public override byte[] LoadImageBytes(string fileName, int offset)
            {
                return new byte[4711];
            }
        }
    }
}
