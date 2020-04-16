#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Types;
using Reko.Gui;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;

namespace Reko.UnitTests.Gui.Windows
{
    class CombinedCodeViewInteractorTests
    {
        private CombinedCodeViewInteractor interactor;
        private CombinedCodeView combinedCodeView;
        private CommonMockFactory mockFactory;
        private Program program;
        private Procedure proc;

        [SetUp]
        public void Setup()
        {
            mockFactory = new CommonMockFactory();
            var platform = mockFactory.CreateMockPlatform();
            var imageMap = new SegmentMap(Address32.Ptr32(0x05));
            program = new Program(imageMap, platform.Object.Architecture, platform.Object);
            interactor = new CombinedCodeViewInteractor();
            var uiPreferencesSvc = new Mock<IUiPreferencesService>();
            var uiSvc = new Mock<IDecompilerShellUiService>();

            var styles = new Dictionary<string, UiStyle>()
            {
                {
                    UiStyles.CodeWindow,
                    new UiStyle
                    {
                        Background = new SolidBrush(Color.White),
                    }
                }
            };
            uiPreferencesSvc.Setup(u => u.Styles).Returns(styles);
            var sc = new ServiceContainer();
            sc.AddService<IUiPreferencesService>(uiPreferencesSvc.Object);
            sc.AddService<IDecompilerShellUiService>(uiSvc.Object);
            interactor.SetSite(sc);
        }

        [TearDown]
        public void TearDown()
        {
            interactor.Close();
        }

        private string Flatten(TextViewModel model)
        {
            var sb = new StringBuilder();
            var lines = model.GetLineSpans(model.LineCount);
            foreach (var line in lines)
            {
                foreach (var span in line.TextSpans)
                {
                    EmitSpanWrapper(span, sb);
                    sb.Append(span.GetText());
                    EmitSpanWrapper(span, sb);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void EmitSpanWrapper(TextSpan span, StringBuilder sb)
        {
            if (span.Style == "code-kw")
                sb.Append("'");
            if (span.Style == "cmt")
                sb.Append("rem ");
            if (span.Style == "link")
                sb.Append("_");
            if (span.Style == "type")
                sb.Append("`");
        }

        private void When_CombinedCodeViewCreated()
        {
            combinedCodeView = (CombinedCodeView) interactor.CreateControl();
        }

        private void When_MovedTo(uint addr)
        {
            interactor.SelectedAddress = Address.Ptr32(addr);
        }

        private void Given_ImageSegment(uint addr, params byte[] bytes)
        {
            var mem = new MemoryArea(Address.Ptr32(addr), bytes);
            var seg = new ImageSegment(".text", mem, AccessMode.ReadWrite);
            program.SegmentMap.AddSegment(seg);
            program.ImageMap = program.SegmentMap.CreateImageMap();
        }

        private void Given_StubProcedure(uint addr, uint size)
        {
            var address = Address.Ptr32(addr);
            var m = new ProcedureBuilder(program.Architecture, "fnTest", address);
            m.Return();
            this.proc = m.Procedure;
            this.program.Procedures[address] = proc;

            var item = new ImageMapBlock
            {
                Address = address,
                Size = size,
                Block = new Block(proc, "fakeBlock")
            };
            program.ImageMap.AddItemWithSize(address, item);
        }

        private void Given_ImageMapItem(uint addr, DataType dataType, string name)
        {
            var address = Address.Ptr32(addr);
            var item = new ImageMapItem
            {
                Address = address,
                DataType = dataType,
                Name = name,
                Size = (uint)dataType.Size,
            };
            program.ImageMap.AddItemWithSize(address, item);
        }

        [Test(Description = "When a user browses to the procedure, we should see it")]
        [Category(Categories.UserInterface)]
        public void Ccvi_SetProcedure()
        {
            Given_ImageSegment(0x10,
                0xC3, /* ret */
                0x01, 0x02, 0x03, 0x04,
                0xE8, 0x03, 0x00, 0x00, /* 1000 */
                0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F);
            Given_StubProcedure(0x10, 1);
            Given_ImageMapItem(0x15, PrimitiveType.Int32, "iVar");

            When_CombinedCodeViewCreated();
            interactor.DisplayProcedure(this.program, this.proc);

            var sExp =
@"'define' fnTest
{
fnTest_entry:
l1:
    'return'
fnTest_exit:
}


`int32` iVar = 1000;

";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.CodeView.Model));

            sExp =
@"00000010 C3 ret 
00000011    01 02 03 04                                  ....
00000015                E8 03 00 00                      ....
00000019                            05 06 07 08 09 0A 0B .......
00000020 0C 0D 0E 0F                                     ....
";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.MixedCodeDataView.Model));

            When_MovedTo(0x00000015);

            sExp =
@"`int32` iVar = 1000;

";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.CodeView.Model));

            sExp =
@"00000015                E8 03 00 00                      ....
00000019                            05 06 07 08 09 0A 0B .......
00000020 0C 0D 0E 0F                                     ....
";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.MixedCodeDataView.Model));
        }

        [Test(Description = "When a user browses to the global variables node, we should see it")]
        [Category(Categories.UserInterface)]
        public void Ccvi_SetGlobalVariables()
        {
            Given_ImageSegment(0x10,
                0xC3, /* ret */
                0x01, 0x02, 0x03, 0x04,
                0xE8, 0x03, 0x00, 0x00, /* 1000 */
                0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F);
            Given_StubProcedure(0x10, 1);
            Given_ImageMapItem(0x15, PrimitiveType.Int32, "iVar");

            When_CombinedCodeViewCreated();
            var segment = this.program.SegmentMap.Segments.Values[0];
            interactor.DisplayGlobals(this.program, segment);

            var sExp =
@"`int32` iVar = 1000;

";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.CodeView.Model));

            sExp =
@"00000010 C3 ret 
00000011    01 02 03 04                                  ....
00000015                E8 03 00 00                      ....
00000019                            05 06 07 08 09 0A 0B .......
00000020 0C 0D 0E 0F                                     ....
";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.MixedCodeDataView.Model));

            When_MovedTo(0x00000020);

            sExp = "";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.CodeView.Model));

            sExp =
@"00000020 0C 0D 0E 0F                                     ....
";
            Assert.AreEqual(sExp, Flatten(combinedCodeView.MixedCodeDataView.Model));
        }
    }
}
