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
using Reko.Core;
using Reko.Gui;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class CodeViewerServiceTests
    {
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
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
            uiSvc.Setup(s => s.FindDocumentWindow(
                    "CombinedCodeViewInteractor", m.Procedure))
                .Returns((IWindowFrame) null)
                .Verifiable();
            var windowPane = new Mock<CombinedCodeViewInteractor>();
            var windowFrame = new Mock<IWindowFrame>();
            windowFrame.Setup(f => f.Pane).Returns(windowPane.Object);
            uiSvc.Setup(s => s.CreateDocumentWindow(
                "CombinedCodeViewInteractor",
                m.Procedure,
                m.Procedure.Name,
                It.IsAny<IWindowPane>()))
                .Returns(windowFrame.Object)
                .Verifiable();
            windowFrame.Setup(s => s.Show())
                .Verifiable();

            var codeViewerSvc = new CodeViewerServiceImpl(sc);
            codeViewerSvc.DisplayProcedure(program, m.Procedure, true);

            uiSvc.VerifyAll();
            windowFrame.VerifyAll();
        }

        [Test]
        public void Cvp_CreateGlobalsViewerIfNotVisible()
        {
            var segment = new ImageSegment(
                ".seg", Address32.Ptr32(0x17), 0, AccessMode.ReadWrite);
            var label = ".seg global variables";

            var uiSvc = AddMockService<IDecompilerShellUiService>();
            uiSvc.Setup(s => s.FindDocumentWindow(
                    "CombinedCodeViewInteractor", segment))
                .Returns((IWindowFrame)null)
                .Verifiable();
            var windowPane = new Mock<CombinedCodeViewInteractor>();
            var windowFrame = new Mock<IWindowFrame>();
            windowFrame.Setup(f => f.Pane).Returns(windowPane.Object);
            uiSvc.Setup(s => s.CreateDocumentWindow(
                "CombinedCodeViewInteractor",
                segment,
                label,
                It.IsAny<IWindowPane>()))
                .Returns(windowFrame.Object);
            windowFrame.Setup(s => s.Show()).Verifiable();

            var codeViewerSvc = new CodeViewerServiceImpl(sc);
            codeViewerSvc.DisplayGlobals(program, segment);

            uiSvc.VerifyAll();
            windowFrame.VerifyAll();
        }

        private Mock<T> AddMockService<T>() where T : class
        {
            var svc = new Mock<T>();
            sc.AddService(typeof (T), svc.Object);
            return svc;
        }
    }
}
