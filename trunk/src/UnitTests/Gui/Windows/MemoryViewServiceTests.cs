/* 
 * Copyright (C) 1999-2010 John Källén.
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

        [Test]
        public void ObtainService()
        {
            ServiceContainer sc = new ServiceContainer();
            IDecompilerShellUiService shellUi = repository.StrictMock<IDecompilerShellUiService>();
            var windowFrame = repository.StrictMock<IWindowFrame>();
            sc.AddService(typeof(IDecompilerShellUiService), shellUi);

            var service = new MemoryViewServiceImpl(sc);
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

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
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
