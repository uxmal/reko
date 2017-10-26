#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using Is = Rhino.Mocks.Constraints.Is;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class LowLevelViewServiceTests
    {
        private MockRepository mr;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [Test]
        [Category(Categories.UserInterface)]
        public void MVS_ShowingWindowCreatesWindowFrame()
        {
            ServiceContainer sc = new ServiceContainer();
            var shellUi = mr.DynamicMock<IDecompilerShellUiService>();
            var decSvc = mr.StrictMock<IDecompilerService>();
            var windowFrame = mr.DynamicMock<IWindowFrame>();
            sc.AddService(typeof(IDecompilerShellUiService), shellUi);
            sc.AddService<IDecompilerService>(decSvc);
            AddStubService<IUiPreferencesService>(sc).Stub(u => u.Styles).Return(new Dictionary<string, UiStyle>());
            Given_Program();
            var service = mr.Stub<LowLevelViewServiceImpl>(sc);
            var interactor = new LowLevelViewInteractor();
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);

            var svc = (ILowLevelViewService)service;
            shellUi.Expect(s => s.FindDocumentWindow("memoryViewWindow", program)).Return(null);
            shellUi.Expect(s => s.CreateDocumentWindow(
                Arg<string>.Is.Anything,
                Arg<Program>.Is.Same(program),
                Arg<string>.Is.Equal("Memory View"),
                Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            Expect.Call(windowFrame.Show);
            mr.ReplayAll();

            interactor.SetSite(sc);
            interactor.CreateControl();
            svc.ShowMemoryAtAddress(this.program, (Address)this.program.ImageMap.BaseAddress);

            mr.VerifyAll();
        }

        private void Given_Program()
        {
            var addrBase = Address.Ptr32(0x10000);
            var mem = new MemoryArea(addrBase, new byte[100]);
            var map = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var arch = mr.Stub<IProcessorArchitecture>();
            var dasm = mr.Stub<IEnumerable<MachineInstruction>>();
            var e = mr.Stub<IEnumerator<MachineInstruction>>();

            arch.Stub(a => a.CreateDisassembler(Arg<EndianImageReader>.Is.NotNull)).Return(dasm);
            arch.Stub(a => a.InstructionBitSize).Return(8);
            arch.Stub(a => a.CreateImageReader(
                Arg<MemoryArea>.Is.NotNull,
                Arg<Address>.Is.NotNull)).Return(mem.CreateLeReader(addrBase));
            dasm.Stub(d => d.GetEnumerator()).Return(e);
            arch.Replay();
            dasm.Replay();
            e.Replay();
            this.program = new Program(map, arch, null);
        }

        private T AddStubService<T>(IServiceContainer sc)
        {
            var svc = mr.Stub<T>();
            sc.AddService<T>(svc);
            return svc;
        }

        [Test]
        [Category(Categories.UserInterface)]
        public void LLI_ShowMemoryAtAddressShouldChangeMemoryControl()
        {
            var sc = new ServiceContainer();
            var ctrl = new LowLevelView();
            var interactor = mr.DynamicMock<LowLevelViewInteractor>();
            interactor.Expect(i => i.SelectedAddress).SetPropertyWithArgument(Address.Ptr32(0x4711));
            var uiSvc = AddStubService<IDecompilerShellUiService>(sc);
            AddStubService<IUiPreferencesService>(sc).Stub(u => u.Styles).Return(new Dictionary<string, UiStyle>());
            Given_Program();
            uiSvc.Stub(x => x.FindDocumentWindow(null, null)).IgnoreArguments().Return(null);
            uiSvc.Stub(x => x.CreateDocumentWindow("", null, "", null))
                .IgnoreArguments()
                .Return(mr.Stub<IWindowFrame>());
            uiSvc.Stub(x => x.SetContextMenu(null, MenuIds.CtxMemoryControl)).IgnoreArguments();
            //uiSvc.Stub(x => x.SetContextMenu(null, MenuIds.CtxMemoryControl));
            //uiSvc.Stub(x => x.GetContextMenu(MenuIds.CtxDisassembler)).Return(new ContextMenu());

            var service = mr.Stub<LowLevelViewServiceImpl>(sc);
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);
            var image = new MemoryArea(Address.Ptr32(0x1000), new byte[300]);
            mr.ReplayAll();

            interactor.SetSite(sc);
            interactor.CreateControl();
            service.ShowMemoryAtAddress(program, Address.Ptr32(0x4711));

            mr.VerifyAll();
        }
    }
}
