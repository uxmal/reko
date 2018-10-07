#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Gui.Forms
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
            mr = new MockRepository();

            form = mr.StrictMock<IMainForm>();
            sc = new ServiceContainer();
            uiSvc = AddService<IDecompilerShellUiService>();
            sc.AddService(typeof(IDecompilerUIService), uiSvc);
            codeViewSvc = AddService<ICodeViewerService>();
            memViewSvc = AddService<ILowLevelViewService>();
            disasmViewSvc = AddService<IDisassemblyViewService>();
            pbSvc = AddService<IProjectBrowserService>();

            form.Stub(f => f.Show());

            var platform = mr.Stub<IPlatform>();
            var loadAddress = Address.Ptr32(0x100000);
            var bytes = new byte[4711];
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var mem = new MemoryArea(loadAddress, bytes);
            this.program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(".text", mem, AccessMode.ReadExecute)),
                Architecture = arch,
                Platform = platform,
            };
            ILoader ldr = mr.StrictMock<ILoader>();
            ldr.Stub(l => l.LoadExecutable(null, null, null, null)).IgnoreArguments().Return(program);
            ldr.Stub(l => l.LoadImageBytes(null, 0)).IgnoreArguments().Return(bytes);
            ldr.Replay();
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
            sc.AddService<DecompilerHost>(new FakeDecompilerHost());
            this.decSvc = new DecompilerService();
            decSvc.Decompiler = new DecompilerDriver(ldr, sc);
            decSvc.Decompiler.Load("test.exe");
            this.program = this.decSvc.Decompiler.Project.Programs.First();
            sc.AddService(typeof(IDecompilerService), decSvc);

            sc.AddService(typeof(IWorkerDialogService), new FakeWorkerDialogService());
        }

        [TearDown]
        public void TearDown()
        {
        }

        private void Given_Interactor()
        {
            interactor = new AnalyzedPageInteractorImpl(sc);
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
            program.Procedures.Add(
                Address.Ptr32(0x12345), 
                Procedure.Create(program.Architecture, "foo", Address.Ptr32(0x12345), program.Architecture.CreateFrame()));
            interactor.EnterPage();

            mr.VerifyAll();
        }


        [Test]
        public void Anpi_SelectProcedure()
        {
            codeViewSvc.Stub(s => s.DisplayProcedure(
                Arg<Program>.Is.Anything,
                Arg<Procedure>.Matches(proc => proc.Name == "foo_proc"),
                Arg<bool>.Is.Equal(true)));
            mr.ReplayAll();

            Given_Interactor();
            form.Show();
            Procedure p = new Procedure(program.Architecture, "foo_proc", Address.Ptr32(0x12346), program.Architecture.CreateFrame());
            p.Signature = FunctionType.Func(
                new Identifier("eax", PrimitiveType.Word32, Registers.eax),
                new Identifier("arg04", PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32)));
            var p2 = new Procedure(program.Architecture, "bar", Address.Ptr32(0x12345), program.Architecture.CreateFrame());
            program.Procedures.Add(p.EntryAddress, p);
            program.Procedures.Add(p2.EntryAddress, p2);
            interactor.EnterPage();

            //form.BrowserList.Items[1].Selected = true;
            //form.BrowserList.FocusedItem = form.BrowserList.Items[1];

            mr.VerifyAll();
        }
    }
}
