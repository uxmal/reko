#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Gui;
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
    public class MemoryViewServiceTests
    {
        private MockRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        [Test]
        public void ShowingWindowCreatesWindowFrame()
        {
            ServiceContainer sc = new ServiceContainer();
            var shellUi = repository.DynamicMock<IDecompilerShellUiService>();
            var windowFrame = repository.DynamicMock<IWindowFrame>();
            sc.AddService(typeof(IDecompilerShellUiService), shellUi);

            var interactor = repository.DynamicMock<MemoryViewInteractor>();
            interactor.Stub(x => x.Control).Return(new MemoryControl());

            var service = repository.Stub<MemoryViewServiceImpl>(sc);
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);

            var svc = (IMemoryViewService)service;
            Expect.Call(shellUi.FindWindow("memoryViewWindow")).Return(null);
            Expect.Call(shellUi.CreateWindow(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal("Memory View"),
                Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            Expect.Call(windowFrame.Show);
            repository.ReplayAll();

            svc.ShowMemoryAtAddress(new Address(0x10000));
            repository.VerifyAll();
        }

        private T AddStubService<T>(IServiceContainer sc)
        {
            var svc = repository.Stub<T>();
            sc.AddService<T>(svc);
            return svc;
        }

        [Test]
        public void ShowMemoryAtAddressShouldChangeMemoryControl()
        {
            ServiceContainer sc = new ServiceContainer();
            MemoryControl ctrl = new MemoryControl();
            var interactor = repository.DynamicMock<MemoryViewInteractor>();
            interactor.Expect(x => x.SelectedAddress).SetPropertyWithArgument(new Address(0x4711));
            interactor.Stub(x => x.Control).Return(ctrl);
            var uiSvc = AddStubService<IDecompilerShellUiService>(sc);
            uiSvc.Stub(x => x.FindWindow(Arg<string>.Is.Anything)).Return(null);
            uiSvc.Stub(x => x.CreateWindow("", "", null))
                .IgnoreArguments()
                .Return(repository.Stub<IWindowFrame>());

            var service = repository.Stub<MemoryViewServiceImpl>(sc);
            service.Stub(x => x.CreateMemoryViewInteractor()).Return(interactor);
            repository.ReplayAll();

            service.ShowMemoryAtAddress(new Address(0x4711));

            repository.VerifyAll();

        }


        private class TestMainFormInteractor : MainFormInteractor
        {
            protected override void CreateServices(ServiceContainer sc, DecompilerMenus dm)
            {
                sc.AddService(typeof(IMemoryViewService), new MemoryViewServiceImpl(sc));
            }
        }

    }
}
