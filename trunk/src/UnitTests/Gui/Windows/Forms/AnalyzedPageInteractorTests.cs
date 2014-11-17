#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Assemblers;
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
using System.Linq;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class AnalyzedPageInteractorTests
    {
        private MockRepository mr;
        private Program program;
        private IMainForm form;
        private AnalyzedPageInteractorImpl interactor;
        private IDecompilerShellUiService uiSvc;
        private ICodeViewerService codeViewSvc;
        private ILowLevelViewService memViewSvc;
        private IDisassemblyViewService disasmViewSvc;
        private ServiceContainer sc;
        private IProjectBrowserService pbSvc;
        private DecompilerService decSvc;

        [SetUp]
        public void Setup()
        {
            form = new MainForm();
            mr = new MockRepository();

            sc = new ServiceContainer();
            uiSvc = AddService<IDecompilerShellUiService>();
            sc.AddService(typeof(IDecompilerUIService), uiSvc);
            codeViewSvc = AddService<ICodeViewerService>();
            memViewSvc = AddService<ILowLevelViewService>();
            disasmViewSvc = AddService<IDisassemblyViewService>();
            pbSvc = AddService<IProjectBrowserService>();

            TestLoader ldr = new TestLoader(sc);
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            this.decSvc = new DecompilerService();
            decSvc.Decompiler = new DecompilerDriver(ldr, new FakeDecompilerHost(), sc);
            decSvc.Decompiler.Load("test.exe");
            program = decSvc.Decompiler.Programs.First();
            decSvc.Decompiler.ScanProgram();
            sc.AddService(typeof(IDecompilerService), decSvc);

            sc.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
        }

        private T AddService<T>() where T : class
        {
            var svc = mr.DynamicMock<T>();
            sc.AddService(typeof(T), svc);
            return svc;
        }

        [Test]
        public void Anpi_Populate()
        {
            Given_Interactor();
            pbSvc.Expect(p => p.Reload());
            mr.ReplayAll();
            
            form.Show();
            program.Procedures.Add(new Address(0x12345), new Procedure("foo", program.Architecture.CreateFrame()));
            interactor.EnterPage();

            //Assert.AreEqual(1, form.BrowserList.Items.Count);
            //KeyValuePair<Address, Procedure> entry = (KeyValuePair<Address, Procedure>) form.BrowserList.Items[0].Tag;
            //Assert.AreEqual(0x12345, entry.Key.Offset);
            //Assert.AreEqual("foo", entry.Value.Name);
            mr.VerifyAll();
        }

        private void Given_Interactor()
        {
            interactor = new AnalyzedPageInteractorImpl(sc);
        }

        [Test]
        public void Anpi_SelectProcedure()
        {
            codeViewSvc.Stub(s => s.DisplayProcedure(
                Arg<Procedure>.Matches(proc => proc.Name == "foo_proc")));
            //memViewSvc.Expect(s => s.ShowMemoryAtAddress(
            //    Arg<Program>.Is.NotNull,
            //    Arg<Address>.Matches(address => address.Linear == 0x12346)));

            //disasmViewSvc.Expect(s => s.DisassembleStartingAtAddress(
            //    Arg<Address>.Matches(address => address.Linear == 0x12346)));
            mr.ReplayAll();

            Given_Interactor();
            form.Show();
            Procedure p = new Procedure("foo_proc", program.Architecture.CreateFrame());
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });

            program.Procedures.Add(new Address(0x12345), new Procedure("bar", program.Architecture.CreateFrame()));
            program.Procedures.Add(new Address(0x12346), p);
            interactor.EnterPage();

            //form.BrowserList.Items[1].Selected = true;
            //form.BrowserList.FocusedItem = form.BrowserList.Items[1];

            mr.VerifyAll();
        }

        [Test]
        [Ignore("Other mechanism needed")]
        public void Anpi_ShowEditProcedureDialog()
        {
            uiSvc.Expect(s => s.ShowModalDialog(
                    Arg<ProcedureDialog>.Is.TypeOf))
                .Return(DialogResult.Cancel);
            mr.ReplayAll();

            Given_Interactor();

            form.Show();
            Procedure p = new Procedure("foo_proc", program.Architecture.CreateFrame());
            p.Signature = new ProcedureSignature(
                new Identifier("eax", 0, PrimitiveType.Word32, Registers.eax),
                new Identifier[] {
                    new Identifier("arg04", 1, PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32))
                });
            program.Procedures.Add(new Address(0x12345), new Procedure("bar", program.Architecture.CreateFrame()));
            interactor.EnterPage();
            //form.BrowserList.Items[0].Selected = true;

            Assert.IsTrue(interactor.Execute(new CommandID(CmdSets.GuidDecompiler, CmdIds.ActionEditSignature)), "Should have executed command.");
            mr.VerifyAll();
        }

        private class TestLoader : ILoader
        {
            public TestLoader(IServiceProvider services)
            {
            }

            public byte[] LoadImageBytes(string fileName, int offset)
            {
                return new byte[4711];
            }

            public Program LoadExecutable(InputFile file)
            {
                throw new NotImplementedException();
            }

            public Program LoadExecutable(string fileName, byte[] bytes, Address loadAddress)
            {
                loadAddress = loadAddress ?? new Address(0x100000);
                Program prog = new Program();
                prog.Image = new LoadedImage(loadAddress, bytes);
                prog.ImageMap = prog.Image.CreateImageMap();
                prog.Architecture = new IntelArchitecture(ProcessorMode.Protected32);
                return prog;
            }

            public Program AssembleExecutable(string fileName, Assembler asm, Address loadAddress)
            {
                throw new NotImplementedException();
            }

            public Program AssembleExecutable(string fileName, byte[] bytes, Assembler asm, Address loadAddress)
            {
                throw new NotImplementedException();
            }

            public TypeLibrary LoadMetadata(string fileName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
