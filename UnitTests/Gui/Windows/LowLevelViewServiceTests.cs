#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Controls;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using Is = Rhino.Mocks.Constraints.Is;

namespace Decompiler.UnitTests.Gui.Windows
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
        public void MVS_ShowingWindowCreatesWindowFrame()
        {
            ServiceContainer sc = new ServiceContainer();
            var shellUi = mr.DynamicMock<IDecompilerShellUiService>();
            var decSvc = mr.StrictMock<IDecompilerService>();
            var windowFrame = mr.DynamicMock<IWindowFrame>();
            var uiPrefSvc = mr.Stub<IUiPreferencesService>();
            sc.AddService(typeof(IDecompilerShellUiService), shellUi);
            sc.AddService<IDecompilerService>(decSvc);
            AddStubService<IUiPreferencesService>(sc);
            Given_Program();
            var service = mr.Stub<LowLevelViewServiceImpl>(sc);
            var interactor = new LowLevelViewInteractor();
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);

            var svc = (ILowLevelViewService)service;
            Expect.Call(shellUi.FindWindow("memoryViewWindow")).Return(null);
            Expect.Call(shellUi.CreateWindow(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal("Memory View"),
                Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            Expect.Call(windowFrame.Show);
            mr.ReplayAll();

            interactor.SetSite(sc);
            interactor.CreateControl();
            svc.ShowMemoryAtAddress(program, program.Image.BaseAddress);

            mr.VerifyAll();
        }

        private void Given_Program()
        {
            var addrBase = Address.Ptr32(0x10000);
            var image = new LoadedImage(addrBase, new byte[100]);
            var map = image.CreateImageMap();
            var arch = mr.Stub<IProcessorArchitecture>();
            var dasm = mr.Stub<IEnumerable<MachineInstruction>>();
            var e = mr.Stub<IEnumerator<MachineInstruction>>();

            arch.Stub(a => a.CreateDisassembler(Arg<ImageReader>.Is.NotNull)).Return(dasm);
            arch.Stub(a => a.InstructionBitSize).Return(8);
            arch.Stub(a => a.CreateImageReader(
                Arg<LoadedImage>.Is.NotNull,
                Arg<Address>.Is.NotNull)).Return(image.CreateLeReader(addrBase));
            dasm.Stub(d => d.GetEnumerator()).Return(e);
            arch.Replay();
            dasm.Replay();
            e.Replay();
            this.program = new Program(image, map, arch, null);
        }

        private T AddStubService<T>(IServiceContainer sc)
        {
            var svc = mr.Stub<T>();
            sc.AddService<T>(svc);
            return svc;
        }

        [Test]
        public void LLI_ShowMemoryAtAddressShouldChangeMemoryControl()
        {
            var sc = new ServiceContainer();
            var ctrl = new LowLevelView();
            var interactor = mr.DynamicMock<LowLevelViewInteractor>();
            interactor.Expect(i => i.SelectedAddress).SetPropertyWithArgument(Address.Ptr32(0x4711));
            var uiSvc = AddStubService<IDecompilerShellUiService>(sc);
            AddStubService<IUiPreferencesService>(sc);
            Given_Program();
            uiSvc.Stub(x => x.FindWindow(Arg<string>.Is.Anything)).Return(null);
            uiSvc.Stub(x => x.CreateWindow("", "", null))
                .IgnoreArguments()
                .Return(mr.Stub<IWindowFrame>());
            uiSvc.Stub(x => x.GetContextMenu(MenuIds.CtxMemoryControl)).Return(new ContextMenu());
            uiSvc.Stub(x => x.GetContextMenu(MenuIds.CtxDisassembler)).Return(new ContextMenu());

            var service = mr.Stub<LowLevelViewServiceImpl>(sc);
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);
            var image = new LoadedImage(Address.Ptr32(0x1000), new byte[300]);
            mr.ReplayAll();

            interactor.SetSite(sc);
            interactor.CreateControl();
            service.ShowMemoryAtAddress(program, Address.Ptr32(0x4711));

            mr.VerifyAll();
        }

        private class TestMainFormInteractor : MainFormInteractor
        {
            public static TestMainFormInteractor Create()
            {
                var services = new ServiceContainer();
                services.AddService(typeof(IServiceFactory), new ServiceFactory(services));
                return new TestMainFormInteractor(services);
            }

            private TestMainFormInteractor(IServiceProvider services)
                : base(services)
            {
            }
        }
    }
}
