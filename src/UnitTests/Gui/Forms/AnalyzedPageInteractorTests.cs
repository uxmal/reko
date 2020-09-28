#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Gui.Forms
{
    [TestFixture]
    public class AnalyzedPageInteractorTests
    {
        private Program program;
        private Mock<IMainForm> form;
        private AnalyzedPageInteractorImpl interactor;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<ICodeViewerService> codeViewSvc;
        private Mock<ILowLevelViewService> memViewSvc;
        private Mock<IDisassemblyViewService> disasmViewSvc;
        private ServiceContainer sc;
        private Mock<IProjectBrowserService> pbSvc;
        private DecompilerService decSvc;

        [SetUp]
        public void Setup()
        {
            form = new Mock<IMainForm>();
            sc = new ServiceContainer();
            uiSvc = AddService<IDecompilerShellUiService>();
            sc.AddService<IDecompilerUIService>(uiSvc.Object);
            codeViewSvc = AddService<ICodeViewerService>();
            memViewSvc = AddService<ILowLevelViewService>();
            disasmViewSvc = AddService<IDisassemblyViewService>();
            pbSvc = AddService<IProjectBrowserService>();

            form.Setup(f => f.Show());

            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.CreateMetadata()).Returns(new TypeLibrary());
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
                Platform = platform.Object,
            };
            var ldr = new Mock<ILoader>();
            ldr.Setup(l => l.LoadExecutable(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<Address>())).Returns(program);
            ldr.Setup(l => l.LoadImageBytes(
                It.IsAny<string>(),
                It.IsAny<int>())).Returns(bytes);
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
            sc.AddService<IDecompiledFileService>(new FakeDecompiledFileService());
            this.decSvc = new DecompilerService();
            decSvc.Decompiler = new Decompiler(ldr.Object, sc);
            decSvc.Decompiler.Load("test.exe");
            this.program = this.decSvc.Decompiler.Project.Programs.First();
            sc.AddService<IDecompilerService>(decSvc);
            sc.AddService<IWorkerDialogService>(new FakeWorkerDialogService());
        }

        [TearDown]
        public void TearDown()
        {
        }

        private void Given_Interactor()
        {
            interactor = new AnalyzedPageInteractorImpl(sc);
        }

        private Mock<T> AddService<T>() where T : class
        {
            var svc = new Mock<T>();
            sc.AddService(typeof(T), svc.Object);
            return svc;
        }

        [Test]
        public void Anpi_Populate()
        {
            Given_Interactor();
            pbSvc.Setup(p => p.Reload()).Verifiable();

            form.Object.Show();
            program.Procedures.Add(
                Address.Ptr32(0x12345), 
                Procedure.Create(program.Architecture, "foo", Address.Ptr32(0x12345), program.Architecture.CreateFrame()));
            interactor.EnterPage();

            pbSvc.VerifyAll();
        }


        [Test]
        public void Anpi_SelectProcedure()
        {
            codeViewSvc.Setup(s => s.DisplayProcedure(
                It.IsAny<Program>(),
                It.Is<Procedure>(proc => proc.Name == "foo_proc"),
                true));

            Given_Interactor();
            form.Object.Show();
            Procedure p = new Procedure(program.Architecture, "foo_proc", Address.Ptr32(0x12346), program.Architecture.CreateFrame());
            p.Signature = FunctionType.Func(
                Identifier.Create(Registers.eax),
                new Identifier("arg04", PrimitiveType.Word32, new StackArgumentStorage(4, PrimitiveType.Word32)));
            var p2 = new Procedure(program.Architecture, "bar", Address.Ptr32(0x12345), program.Architecture.CreateFrame());
            program.Procedures.Add(p.EntryAddress, p);
            program.Procedures.Add(p2.EntryAddress, p2);
            interactor.EnterPage();

            //form.BrowserList.Items[1].Selected = true;
            //form.BrowserList.FocusedItem = form.BrowserList.Items[1];
        }
    }
}
