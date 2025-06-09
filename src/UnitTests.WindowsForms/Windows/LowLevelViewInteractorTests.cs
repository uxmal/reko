#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Reko.UnitTests.Gui.Windows
{
    [TestFixture]
    [Category(Categories.UserInterface)]
    public class LowLevelViewInteractorTests
    {
        private Mock<LowLevelViewInteractor> interactor;
        private Mock<IProcessorArchitecture> arch;
        private Mock<IDecompilerShellUiService> uiSvc;
        private Mock<IUiPreferencesService> uiPrefsSvc;
        private Mock<IDialogFactory> dlgFactory;
        private ServiceContainer sp;
        private Address addrBase;
        private LowLevelView control;
        private ByteMemoryArea mem;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private Program program;
        private Form form;

        [SetUp]
        public void Setup()
        {
            sp = new ServiceContainer();
            uiSvc = new Mock<IDecompilerShellUiService>();
            uiPrefsSvc = new Mock<IUiPreferencesService>();
            dlgFactory = new Mock<IDialogFactory>();
            uiSvc.Setup(u => u.SetContextMenu(
                It.IsAny<object>(), It.IsAny<int>()));
            uiPrefsSvc.Setup(u => u.Styles).Returns(new Dictionary<string, UiStyle>());
            sp.AddService<IDecompilerShellUiService>(uiSvc.Object);
			sp.AddService<IDialogFactory>(dlgFactory.Object);
            sp.AddService<IUiPreferencesService>(uiPrefsSvc.Object);
            addrBase = Address.Ptr32(0x1000);
        }

        [TearDown]
        public void TearDown()
        {
            if (form is not null) form.Dispose(); form = null;
        }

        private void Given_Interactor()
        {
            interactor = new Mock<LowLevelViewInteractor>();
            interactor.Object.SetSite(sp);
            control = (LowLevelView) interactor.Object.CreateControl();
            interactor.Object.Program = program;
        }

        [Test]
        public void LLI_GotoAddressEnabled()
        {
            Given_Architecture();
            Given_Program(new byte[0x13000]); 
            Given_Interactor();

            When_ShowControl();
            control.MemoryView.Focus();
            var status = new CommandStatus();
            Assert.IsTrue(interactor.Object.QueryStatus(new CommandID(CmdSets.GuidReko, CmdIds.ViewGoToAddress), status, null));
            Assert.AreEqual(status.Status, MenuStatus.Enabled | MenuStatus.Visible);
        }

        private void When_ShowControl()
        {
            form = new Form();
            control.Parent = form;
            control.Dock = DockStyle.Fill;
            form.Show();
        }

        [Test]
        public void LLI_SelectAddress()
        {
            Given_Architecture();
            Given_Program(new byte[0x13000]);
            Given_Interactor();

            interactor.Object.Control.MemoryView.SelectedAddress = Address.Ptr32(0x12321);

            Assert.AreEqual(0x12321ul, interactor.Object.Control.DisassemblyView.TopAddress.ToLinear());
        }

        [Test]
        public void LLI_GotoAddress()
        {
            Given_Interactor();
            Given_Architecture();
            Given_Program(new byte[] { 0x4, 0x3, 0x2, 0x1 });
            interactor.Setup(i => i.GetSelectedAddressRange())
                .Returns(new AddressRange(program.ImageMap.BaseAddress, program.ImageMap.BaseAddress));

            When_ShowControl();
            interactor.Object.Control.MemoryView.Focus();
            interactor.Object.Program = program;
            interactor.Object.GotoAddress();

            Assert.AreEqual("0x01020304<p32>", interactor.Object.Control.ToolBarAddressTextbox.Text);
        }

        private void Given_Architecture()
        {
            arch = new Mock<IProcessorArchitecture>();
            var dasm = new Mock<IEnumerable<MachineInstruction>>();
            var e = new Mock<IEnumerator<MachineInstruction>>();
            arch.Setup(a => a.Name).Returns("FakeArch");
            arch.Setup(a => a.InstructionBitSize).Returns(8);
            arch.Setup(a => a.MemoryGranularity).Returns(8);
            arch.Setup(a => a.PointerType).Returns(PrimitiveType.Ptr32);
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>()))
                .Returns((ByteMemoryArea i, Address a) => new LeImageReader(i, a));
            arch.Setup(a => a.CreateDisassembler(
                It.IsNotNull<EndianImageReader>())).Returns(dasm.Object);
            Address dummy;
            arch.Setup(a => a.TryParseAddress(
                It.IsNotNull<string>(),
                out dummy))
                .Returns(new StringToAddress((string sAddr, out Address addr) =>
                {
                    return Address.TryParse32(sAddr, out addr);
                }));
            dasm.Setup(d => d.GetEnumerator()).Returns(e.Object);
        }

        private void Given_Program(byte[] bytes)
        {
            var addr = Address.Ptr32(0x1000);
            var mem = new ByteMemoryArea(addr, bytes);
            this.segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            this.imageMap = segmentMap.CreateImageMap();
            this.program = new Program(
                segmentMap,
                arch.Object,
                new DefaultPlatform(null, arch.Object));
            this.program.ImageMap = imageMap;
        }

        [Test]
        public void LLI_MarkAreaWithType()
        {
            Given_Architecture();
            Given_Program(new byte[100]);
            Given_Interactor();

            interactor.Object.SetTypeAtAddressRange(addrBase, "i32");

            ImageMapItem item;
            Assert.IsTrue(imageMap.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(addrBase, item.Address);
            Assert.AreEqual("int32", item.DataType.ToString());
        }

        [Test]
        public void LLI_MarkAreaWithType_array()
        {
            Given_Architecture();
            Given_Program(new byte[100]);
            Given_Interactor();

            control.MemoryView.SetAddressRange(addrBase, addrBase + 12);
            interactor.Object.SetTypeAtAddressRange(addrBase, "apx");

            ImageMapItem item;
            Assert.IsTrue(imageMap.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(addrBase, item.Address);
            Assert.AreEqual("(arr (ptr32 code) 3)", item.DataType.ToString());
        }

        private void Given_Image()
        {
            Given_Image(new byte[0x100]);
        }

        private void Given_Image(params byte[] bytes)
        {
            mem = new ByteMemoryArea(addrBase, bytes);
            segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            program = new Program(segmentMap, arch.Object, null);
            interactor.Object.Program = program;
        }

        [Test]
        public void LLI_NavigateToAddress()
        {
            Given_Architecture();
            Given_Interactor();
            Given_Image();

            When_EnterAddressInBar("100");
            When_GoPushed();
            Assert.IsNull(control.MemoryView.SelectedAddress);
            When_EnterAddressInBar("1000");
            When_GoPushed();
            Assert.AreEqual(0x01000, control.MemoryView.SelectedAddress.ToLinear());
            When_EnterAddressInBar("1004");
            When_GoPushed();
            Assert.AreEqual(0x01004, control.MemoryView.SelectedAddress.ToLinear());
            When_EnterAddressInBar("10010");
            When_GoPushed();
            Assert.AreEqual(0x01004, control.MemoryView.SelectedAddress.ToLinear());
        }

        private void When_EnterAddressInBar(string address)
        {
            control.ToolBarAddressTextbox.Text = address;
        }

        private void When_GoPushed()
        {
            control.ToolBarGoButton.PerformClick();
        }
    }
}
