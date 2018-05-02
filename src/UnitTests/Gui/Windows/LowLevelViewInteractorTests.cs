#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using Rhino.Mocks;
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
        private LowLevelViewInteractor interactor;
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IDecompilerShellUiService uiSvc;
        private IDialogFactory dlgFactory;
        private ServiceContainer sp;
        private Address addrBase;
        private LowLevelView control;
        private MemoryArea mem;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private IUiPreferencesService uiPrefsSvc;
        private Program program;
        private Form form;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sp = new ServiceContainer();
            uiSvc = mr.StrictMock<IDecompilerShellUiService>();
            uiPrefsSvc = mr.StrictMock<IUiPreferencesService>();
            dlgFactory = mr.StrictMock<IDialogFactory>();
            uiSvc.Stub(u => u.SetContextMenu(null, 0)).IgnoreArguments();
            uiSvc.Replay();
            uiPrefsSvc.Stub(u => u.Styles).Return(new Dictionary<string, UiStyle>());
            uiPrefsSvc.Replay();
            sp.AddService(typeof(IDecompilerShellUiService), uiSvc);
			sp.AddService(typeof(IDialogFactory), dlgFactory);
            sp.AddService(typeof(IUiPreferencesService), uiPrefsSvc);
            addrBase = Address.Ptr32(0x1000);
        }

        [TearDown]
        public void TearDown()
        {
            if (form != null) form.Dispose(); form = null;
        }

        private void Given_Interactor()
        {
            interactor = mr.PartialMock<LowLevelViewInteractor>();
            interactor.SetSite(sp);
            control = (LowLevelView) interactor.CreateControl();
            interactor.Program = program;
        }

        [Test]
        public void LLI_GotoAddressEnabled()
        {
            Given_Architecture();
            Given_Program(new byte[0x13000]); 
            Given_Interactor();
            mr.ReplayAll();

            When_ShowControl();
            control.MemoryView.Focus();
            var status = new CommandStatus();
            Assert.IsTrue(interactor.QueryStatus(new CommandID(CmdSets.GuidReko, CmdIds.ViewGoToAddress), status, null));
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
            mr.ReplayAll();

            interactor.Control.MemoryView.SelectedAddress = Address.Ptr32(0x12321);

            Assert.AreEqual(0x12321ul, interactor.Control.DisassemblyView.TopAddress.ToLinear());
            mr.VerifyAll();
        }

        [Test]
        public void LLI_GotoAddress()
        {
            Given_Interactor();
            Given_Architecture();
            Given_Program(new byte[] { 0x4, 0x3, 0x2, 0x1 });
            interactor.Stub(i => i.GetSelectedAddressRange())
                .Return(new AddressRange(program.ImageMap.BaseAddress, program.ImageMap.BaseAddress));
            mr.ReplayAll();

            When_ShowControl();
            interactor.Control.MemoryView.Focus();
            interactor.Program = program;
            interactor.GotoAddress();

            mr.VerifyAll();
            Assert.AreEqual("0x01020304", interactor.Control.ToolBarAddressTextbox.Text);
            mr.ReplayAll();
        }

        private void Given_Architecture()
        {
            arch = mr.Stub<IProcessorArchitecture>();
            var dasm = mr.Stub<IEnumerable<MachineInstruction>>();
            var e = mr.Stub<IEnumerator<MachineInstruction>>();
            arch.Stub(a => a.InstructionBitSize).Return(8);
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Ptr32);
            arch.Stub(a => a.CreateImageReader(null, null))
                .IgnoreArguments()
                .Do(new Func<MemoryArea, Address, EndianImageReader>((i, a) => new LeImageReader(i, a)));
            arch.Stub(a => a.CreateDisassembler(
                Arg<EndianImageReader>.Is.NotNull)).Return(dasm);
            Address dummy;
            arch.Stub(a => a.TryParseAddress(null, out dummy)).IgnoreArguments().WhenCalled(m =>
                {
                    Address addr;
                    bool ret = Address.TryParse32((string)m.Arguments[0], out addr);
                    m.Arguments[1] = addr;
                    m.ReturnValue = ret;
                }).Return(false);
            dasm.Stub(d => d.GetEnumerator()).Return(e);
            arch.Replay();
            dasm.Replay();
            e.Replay();
        }

        private void Given_Program(byte[] bytes)
        {
            var addr = Address.Ptr32(0x1000);
            var mem = new MemoryArea(addr, bytes);
            this.segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            this.imageMap = segmentMap.CreateImageMap();
            this.program = new Program(
                segmentMap,
                arch, 
                new DefaultPlatform(null, arch));
            this.program.ImageMap = imageMap;
        }

        [Test]
        public void LLI_MarkAreaWithType()
        {
            Given_Architecture();
            Given_Program(new byte[100]);
            Given_Interactor();
            mr.ReplayAll();

            interactor.SetTypeAtAddressRange(addrBase, "i32");

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
            mr.ReplayAll();

            control.MemoryView.SetAddressRange(addrBase, addrBase + 12);
            interactor.SetTypeAtAddressRange(addrBase, "apx");

            ImageMapItem item;
            Assert.IsTrue(imageMap.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(addrBase, item.Address);
            Assert.AreEqual("(arr (ptr code) 3)", item.DataType.ToString());
        }

        private void Given_Image()
        {
            Given_Image(new byte[0x100]);
        }

        private void Given_Image(params byte[] bytes)
        {
            mem = new MemoryArea(addrBase, bytes);
            segmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(
                        "code", mem, AccessMode.ReadWriteExecute));
            program = new Program(segmentMap, arch, null);
            interactor.Program = program;
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
