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
using Reko.Gui;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Reko.UserInterfaces.WindowsForms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class CodeViewerServiceTests
    {
        private ServiceContainer sc;
        private Program program;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            var mem = new MemoryArea(Address.Ptr32(0x0040000),  new byte[0x400]);
            this.program = new Program
            {
                SegmentMap = new SegmentMap(mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute))
            };
        }

        [Test]
        public void Cvp_CreateProcedureViewerIfNotVisible()
        {
            var m = new ProcedureBuilder();
            m.Return();

            var uiSvc = AddMockService<IDecompilerShellUiService>();
            uiSvc.Expect(s => s.FindDocumentWindow(
                    "CombinedCodeViewInteractor", m.Procedure))
                .Return(null);
            var windowPane = mr.Stub<CombinedCodeViewInteractor>();
            var windowFrame = mr.StrictMock<IWindowFrame>();
            windowFrame.Stub(f => f.Pane).Return(windowPane);
            uiSvc.Expect(s => s.CreateDocumentWindow(
                    Arg<string>.Is.Equal("CombinedCodeViewInteractor"),
                Arg<string>.Is.Equal(m.Procedure),
                Arg<string>.Is.Equal(m.Procedure.Name),
                Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            windowFrame.Expect(s => s.Show());

            mr.ReplayAll();

            var codeViewerSvc = new CodeViewerServiceImpl(sc);
            codeViewerSvc.DisplayProcedure(program, m.Procedure, true);

            uiSvc.VerifyAllExpectations();
        }

        [Test]
        public void Cvp_CreateGlobalsViewerIfNotVisible()
        {
            var segment = new ImageSegment(
                ".seg", Address32.Ptr32(0x17), 0, AccessMode.ReadWrite);
            var label = ".seg global variables";

            var uiSvc = AddMockService<IDecompilerShellUiService>();
            uiSvc.Expect(s => s.FindDocumentWindow(
                    "CombinedCodeViewInteractor", segment))
                .Return(null);
            var windowPane = mr.Stub<CombinedCodeViewInteractor>();
            var windowFrame = mr.StrictMock<IWindowFrame>();
            windowFrame.Stub(f => f.Pane).Return(windowPane);
            uiSvc.Expect(s => s.CreateDocumentWindow(
                    Arg<string>.Is.Equal("CombinedCodeViewInteractor"),
                Arg<string>.Is.Equal(segment),
                Arg<string>.Is.Equal(label),
                Arg<IWindowPane>.Is.Anything))
                .Return(windowFrame);
            windowFrame.Expect(s => s.Show());

            mr.ReplayAll();

            var codeViewerSvc = new CodeViewerServiceImpl(sc);
            codeViewerSvc.DisplayGlobals(program, segment);

            uiSvc.VerifyAllExpectations();
        }

        private T AddMockService<T>() where T : class
        {
            var svc = mr.StrictMock<T>();
            sc.AddService(typeof (T), svc);
            return svc;
        }
    }
}
