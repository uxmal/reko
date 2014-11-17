#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Gui;
using Decompiler.Gui.Forms;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Controls;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
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
        private LoadedImage image;
        private ImageMap imageMap;
        private IUiPreferencesService uiPrefsSvc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sp = new ServiceContainer();
            uiSvc = mr.StrictMock<IDecompilerShellUiService>();
            uiPrefsSvc = mr.StrictMock<IUiPreferencesService>();
            dlgFactory = mr.StrictMock<IDialogFactory>();
            uiSvc.Stub(u => u.GetContextMenu(MenuIds.CtxMemoryControl)).Return(new ContextMenu());
            uiSvc.Replay();
            uiPrefsSvc.Stub(u => u.DisassemblyFont).Return(new System.Drawing.Font("Lucida Console", 10));
            uiPrefsSvc.Replay();
            sp.AddService(typeof(IDecompilerShellUiService), uiSvc);
			sp.AddService(typeof(IDialogFactory), dlgFactory);
            sp.AddService(typeof(IUiPreferencesService), uiPrefsSvc);
            addrBase = new Address(0x1000);
        }

        private void Given_Interactor()
        {
            interactor = mr.PartialMock<LowLevelViewInteractor>();
            interactor.SetSite(sp);
            control = (LowLevelView) interactor.CreateControl();
        }

        [Test]
        public void LLI_GotoAddressEnabled()
        {
            interactor = new LowLevelViewInteractor();
            var status = new CommandStatus();
            Assert.IsTrue(interactor.QueryStatus(new CommandID(CmdSets.GuidDecompiler, CmdIds.ViewGoToAddress), status, null));
            Assert.AreEqual(status.Status, MenuStatus.Enabled | MenuStatus.Visible);
        }

        [Test]
        public void LLI_SelectAddress()
        {
            Given_Interactor();
            mr.ReplayAll();

            interactor.Control.MemoryView.SelectedAddress = new Address(0x12321);

            Assert.AreEqual(0x12321, interactor.Control.DisassemblyView.TopAddress.Linear);
            Assert.AreEqual(0x12321, interactor.Control.DisassemblyView.SelectedAddress.Linear);
            mr.VerifyAll();
        }

        [Test]
        public void LLI_GotoAddress()
        {
            Given_Interactor();
            Given_Architecture();
            interactor.Stub(i => i.GetSelectedAddressRange())
                .Return(new AddressRange(program.Image.BaseAddress, program.Image.BaseAddress));
            Given_Program(new byte[] { 0x4, 0x3, 0x2, 0x1 });
            mr.ReplayAll();

            interactor.Program = program;
            interactor.Execute(new CommandID(CmdSets.GuidDecompiler, CmdIds.ViewGoToAddress));

            mr.VerifyAll();
            Assert.AreEqual("0x01020304", interactor.Control.ToolBarAddressTextbox.Text);
            mr.ReplayAll();
        }

        private void Given_Architecture()
        {
            arch = mr.Stub<IProcessorArchitecture>();
            arch.Stub(a => a.InstructionBitSize).Return(8);
            arch.Stub(a => a.PointerType).Return(PrimitiveType.Pointer32);
            arch.Stub(a => a.CreateImageReader(null, null))
                .IgnoreArguments()
                .Do(new Func<LoadedImage, Address, ImageReader>((i, a) => new LeImageReader(i, a)));
            arch.Replay();
        }

        private void Given_Program(byte[] bytes)
        {
            var addr = new Address(0x1000);
            var image = new LoadedImage(addr, bytes);
            var map = image.CreateImageMap();
            this.program = new Program(image, map, arch, new DefaultPlatform(null, arch));
        }

        [Test]
        public void LLI_MarkAreaWithType()
        {
            Given_Architecture();
            Given_Interactor();
            Given_Image();
            interactor.SetTypeAtAddressRange(addrBase, "i32");

            ImageMapItem item;
            Assert.IsTrue(imageMap.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(addrBase, item.Address);
            Assert.AreEqual("int32", item.DataType.ToString());
        }

        private void Given_Image()
        {
            Given_Image(new byte[0x100]);
        }

        private void Given_Image(params byte[] bytes)
        {
            image = new LoadedImage(addrBase, bytes);
            imageMap = image.CreateImageMap();
            program = new Program(image, imageMap, arch, null);
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
            Assert.AreEqual(0x01000, control.MemoryView.SelectedAddress.Linear);
            When_EnterAddressInBar("1004");
            When_GoPushed();
            Assert.AreEqual(0x01004, control.MemoryView.SelectedAddress.Linear);
            When_EnterAddressInBar("10010");
            When_GoPushed();
            Assert.AreEqual(0x01004, control.MemoryView.SelectedAddress.Linear);
        }

        private void When_EnterAddressInBar(string address)
        {
            control.ToolBarAddressTextbox.Text = address;
        }

        private void When_GoPushed()
        {
            control.ToolBarGoButton.PerformClick();
        }

        [Test]
        public void LLI_GetDataSize_of_Integer()
        {
            Given_Architecture();
            Given_Interactor();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);
            mr.ReplayAll();

            Assert.AreEqual(4u, interactor.GetDataSize(addrBase, PrimitiveType.Int32));
        }

        [Test]
        public void LLI_GetDataSize_of_ZeroTerminatedString()
        {
            Given_Architecture();
            Given_Interactor();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);
            mr.ReplayAll();

            var dt = StringType.NullTerminated(PrimitiveType.Char);
            Assert.AreEqual(6u, interactor.GetDataSize(addrBase, dt), "5 bytes for 'hello' and 1 for the terminating null'");
        }
    }
}
