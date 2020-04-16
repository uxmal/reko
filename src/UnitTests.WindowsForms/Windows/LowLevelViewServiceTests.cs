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
using Reko.Core.Machine;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    public class LowLevelViewServiceTests
    {
        private Program program;

        [Test]
        [Category(Categories.UserInterface)]
        public void MVS_ShowingWindowCreatesWindowFrame()
        {
            ServiceContainer sc = new ServiceContainer();
            var shellUi = new Mock<IDecompilerShellUiService>();
            var decSvc = new Mock<IDecompilerService>();
            var windowFrame = new Mock<IWindowFrame>();
            sc.AddService<IDecompilerShellUiService>(shellUi.Object);
            sc.AddService<IDecompilerService>(decSvc.Object);
            AddStubService<IUiPreferencesService>(sc).Setup(u => u.Styles)
                .Returns(new Dictionary<string, UiStyle>());
            Given_Program();
            var service = new Mock<LowLevelViewServiceImpl>(sc);
            var interactor = new LowLevelViewInteractor();
            service.Setup(x => x.CreateMemoryViewInteractor())
                .Returns(interactor);

            var svc = (ILowLevelViewService)service.Object;
            shellUi.Setup(s => s.FindDocumentWindow("memoryViewWindow", program))
                .Returns((IWindowFrame) null)
                .Verifiable();
            shellUi.Setup(s => s.CreateDocumentWindow(
                It.IsAny<string>(),
                program,
                "Memory View",
                It.IsAny<IWindowPane>()))
                .Returns(windowFrame.Object)
                .Verifiable();
            windowFrame.Setup(w => w.Show());

            interactor.SetSite(sc);
            interactor.CreateControl();
            svc.ShowMemoryAtAddress(this.program, (Address)this.program.ImageMap.BaseAddress);

            shellUi.Verify();
            windowFrame.Verify();
        }

        private void Given_Program()
        {
            var addrBase = Address.Ptr32(0x10000);
            var mem = new MemoryArea(addrBase, new byte[100]);
            var map = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var arch = new Mock<IProcessorArchitecture>();
            var dasm = new Mock<IEnumerable<MachineInstruction>>();
            var e = new Mock<IEnumerator<MachineInstruction>>();

            arch.Setup(a => a.Name).Returns("FakeArch");
            arch.Setup(a => a.CreateDisassembler(It.IsNotNull<EndianImageReader>())).Returns(dasm.Object);
            arch.Setup(a => a.InstructionBitSize).Returns(8);
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<MemoryArea>(),
                It.IsNotNull<Address>())).Returns(mem.CreateLeReader(addrBase));
            dasm.Setup(d => d.GetEnumerator()).Returns(e.Object);
            this.program = new Program(map, arch.Object, null);
        }

        private Mock<T> AddStubService<T>(IServiceContainer sc) where T: class
        {
            var svc = new Mock<T>();
            sc.AddService<T>(svc.Object);
            return svc;
        }

        [Test]
        [Category(Categories.UserInterface)]
        public void LLI_ShowMemoryAtAddressShouldChangeMemoryControl()
        {
            var sc = new ServiceContainer();
            var ctrl = new LowLevelView();
            var interactor = new Mock<LowLevelViewInteractor>();
            //interactor.Expect(i => i.SelectedAddress).SetPropertyWithArgument(Address.Ptr32(0x4711));
            var uiSvc = AddStubService<IDecompilerShellUiService>(sc);
            AddStubService<IUiPreferencesService>(sc).Setup(u => u.Styles).Returns(new Dictionary<string, UiStyle>());
            Given_Program();
            uiSvc.Setup(x => x.FindDocumentWindow(It.IsAny<string>(), It.IsAny<object>()))
                .Returns((IWindowFrame) null);
            uiSvc.Setup(x => x.CreateDocumentWindow(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<IWindowPane>()))
                .Returns(new Mock<IWindowFrame>().Object);
            //uiSvc.Setup(x => x.SetContextMenu(null, MenuIds.CtxMemoryControl)).IgnoreArguments();
            //uiSvc.Setup(x => x.SetContextMenu(null, MenuIds.CtxMemoryControl));
            //uiSvc.Setup(x => x.GetContextMenu(MenuIds.CtxDisassembler)).Return(new ContextMenu());

            var service = new Mock<LowLevelViewServiceImpl>(sc);
            service.Setup(x => x.CreateMemoryViewInteractor()).Returns(interactor.Object);
            var image = new MemoryArea(Address.Ptr32(0x1000), new byte[300]);

            interactor.Object.SetSite(sc);
            interactor.Object.CreateControl();
            service.Object.ShowMemoryAtAddress(program, Address.Ptr32(0x4711));
        }
    }
}
