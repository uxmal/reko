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

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Windows;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class CodeViewerServiceTests
    {
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
        }

        [Test]
        public void CreateViewerIfNotVisible()
        {
            var uiSvc = AddMockService<IDecompilerShellUiService>();
            uiSvc.Expect(s => s.FindWindow(
                    "codeViewerWindow"))
                .Return(null);
            var windowFrame = MockRepository.GenerateStub<IWindowFrame>();
            uiSvc.Expect(s => s.CreateWindow(
                    Arg<string>.Is.Equal("codeViewerWindow"),
                    Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            windowFrame.Expect(s => s.Show());

            var m = new ProcedureMock();
            m.Return();
            var codeViewerSvc = new CodeViewerServiceImpl(sc);
            codeViewerSvc.DisplayProcedure(m.Procedure);

            uiSvc.VerifyAllExpectations();



        }

        private T AddMockService<T>() where T : class
        {
            var svc = MockRepository.GenerateMock<T>();
            sc.AddService(typeof (T), svc);
            return svc;
        }
    }
}
