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
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Gui.Windows.Controls
{
    [TestFixture]
    public class MixedCodeDataModelTests
    {
        private Mock<IProcessorArchitecture> arch;
        private Mock<IPlatform> platform;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private Program program;
        private StringWriter writer;
        private Procedure proc;

        [SetUp]
        public void Setup()
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.platform = new Mock<IPlatform>();
            this.platform.Setup(p => p.Architecture).Returns(arch.Object);
            this.arch.Setup(a => a.Name).Returns("FakeArch");
            this.arch.Setup(a => a.CreateImageReader(
                It.IsAny<MemoryArea>(),
                It.IsAny<Address>()))
                .Returns((MemoryArea m, Address a) => new LeImageReader(m, a));
            this.arch.Setup(a => a.CreateDisassembler(
                It.IsAny<EndianImageReader>()))
                .Returns((EndianImageReader rdr) => new MachineInstruction[]
                {
                    Instr(rdr.Address.ToUInt32()),
                    Instr(rdr.Address.ToUInt32()+2),
                    Instr(rdr.Address.ToUInt32()+4)
                });
            this.arch.Setup(a => a.InstructionBitSize).Returns(8);
            this.writer = new StringWriter();
            this.proc = Procedure.Create(arch.Object, Address.Ptr32(0x42), new Frame(PrimitiveType.Ptr32));
        }

        private void Given_Program()
        {
            this.imageMap = segmentMap.CreateImageMap();
            this.program = new Program(segmentMap, arch.Object, platform.Object);
            this.program.ImageMap = imageMap;
        }

        private void Given_Comment(uint addr, string comment)
        {
            program.User.Annotations[Address.Ptr32(addr)] = comment;
        }

        private FakeInstruction Instr(uint addr)
        {
            var reg = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
            return new FakeInstruction(Mnemonic.Add, new RegisterOperand(reg), new RegisterOperand(reg))
            {
                Address = Address.Ptr32(addr),
                Length = 2,
            };
        }

        private void Given_CodeBlock(Address addr, int size)
        {
            imageMap.AddItem(addr, new ImageMapBlock
            {
                Address = addr,
                Block = new Block(proc, NamingPolicy.Instance.BlockName(addr)),
                DataType = new CodeType(),
                Size = (uint)size,
            });
        }

        private string SpanStyle(string style)
        {
            if (style == "dasm-bytes")
                return "b";
            if (style == "dasm-bytes lastLine")
                return "b-last";
            if (style == "dasm-opcode")
                return "op";
            if (style == "dasm-opcode lastLine")
                return "op-last";
            if (style == "link")
                return "link";
            if (style == "link lastLine")
                return "link-last";
            if (style == "code-cmt")
                return "cmt";
            return style;
        }

        private void WriteLine(LineSpan line, TextWriter writer)
        {
            writer.Write($"{line.Position}:");
            foreach (var span in line.TextSpans)
            {
                writer.Write($@"{SpanStyle(span.Style)}({span.GetText()})");
            }
            writer.WriteLine();
        }

        private void GetLineSpans(TextViewModel model, int count)
        {
            var lines = model.GetLineSpans(count);
            foreach (var line in lines)
            {
                WriteLine(line, writer);
            }
        }

        private void MoveToLine(TextViewModel model, uint addr)
        {
            var pos = MixedCodeDataModel.Position(Address.Ptr32(addr), 0);
            model.MoveToLine(pos, 0);
        }

        private void AssertOutput(string expected)
        {
            Assert.AreEqual(expected, writer.ToString());
        }

        [Test]
        public void Mcdm_AddDasm()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 2);

            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_AddDasmAndMemory()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_Move()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(segmentMap, arch.Object, platform.Object);
            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);
            var lines = mcdm.GetLineSpans(2);

            Assert.AreEqual(2, lines.Length);
        }

        [Test]
        public void Mcdm_GetLineSpans_MemoryAreaIsLargerThanSegment()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[100]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute) { Size = 4 },
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);

            // Read the first instruction
            var lines = mcdm.GetLineSpans(1);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("00041000(0)", lines[0].Position.ToString());
            Assert.AreEqual("00041002(0)", mcdm.CurrentPosition.ToString());

            // Read the second and last instruction.
            lines = mcdm.GetLineSpans(1);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("00041002(0)", lines[0].Position.ToString());
            Assert.AreEqual("00042000(0)", mcdm.CurrentPosition.ToString());

            // Read the 8 remaining bytes from .data
            lines = mcdm.GetLineSpans(1);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("00042000(0)", lines[0].Position.ToString());
            Assert.AreEqual("00042008(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_GetLineSpans_AllLines()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[100]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute) { Size = 4 },
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);
            Given_CodeBlock(Address.Ptr32(0x42004), 4);

            var mcdm = new MixedCodeDataModel(program);

            // Read all lines
            var lines = mcdm.GetLineSpans(5);
            Assert.AreEqual(5, lines.Length);
            Assert.AreEqual("00041000(0)", lines[0].Position.ToString());
            Assert.AreEqual("00041002(0)", lines[1].Position.ToString());
            Assert.AreEqual("00042000(0)", lines[2].Position.ToString());
            Assert.AreEqual("00042004(0)", lines[3].Position.ToString());
            Assert.AreEqual("00042006(0)", lines[4].Position.ToString());
            Assert.AreEqual("00042008(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_GetLineSpans_InvalidAddress()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[8]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(segmentMap, arch.Object, platform.Object);

            var mcdm = new MixedCodeDataModel(program);

            // This places the curpos right after the last item in the .text
            // segment.
            var pos = MixedCodeDataModel.Position(Address.Ptr32(0x41008), 0);
            mcdm.MoveToLine(pos, 0);

            // This should return the first line of the .data segment.
            var lines = mcdm.GetLineSpans(1);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("00042000(0)", lines[0].Position.ToString());
            Assert.AreEqual("00042008(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_FindInstructionIndex()
        {
            var instrs = new MachineInstruction[]
            {
                new FakeInstruction(Mnemonic.Add) { Address = Address.Ptr32(0x1000), Length = 2 },
                new FakeInstruction(Mnemonic.Add) { Address = Address.Ptr32(0x1002), Length = 2 },
            };
            Func<uint, int> Idx = u =>
                MixedCodeDataModel.FindIndexOfInstructionAddress(
                    instrs,
                    Address.Ptr32(u));
            Assert.AreEqual(-1, Idx(0x0FFF));
            Assert.AreEqual(0, Idx(0x1000));
            Assert.AreEqual(0, Idx(0x1001));
            Assert.AreEqual(1, Idx(0x1002));
            Assert.AreEqual(1, Idx(0x1003));
            Assert.AreEqual(-1, Idx(0x1004));
        }

        [Test]
        public void Mcdm_MoveForward()
        {
            var addrBase = Address.Ptr32(0x40000);

            // 041000: inst0
            // 041002: inst1
            // 042000: data <16 bytes>
            // 042010: data <16 bytes>

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[4]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();

            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);
            // Advance 1 line into another piece of code.
            int delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual(1, delta);
            // move another line of code and then into data.
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 2);
            Assert.AreEqual(2, delta);
            Assert.AreEqual("00042010(0)", mcdm.CurrentPosition.ToString());
            // Another line of data
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual("00042020(0)", mcdm.CurrentPosition.ToString());
            Assert.AreEqual(1, delta);
            // Pegged at end
            delta = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual("00042020(0)", mcdm.CurrentPosition.ToString());
            Assert.AreEqual(0, delta);

            mcdm.MoveToLine(mcdm.StartPosition, 2);
            Assert.AreEqual("00042000(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_MoveToLastLineOfItem()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x40FD5), new byte[64]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(segmentMap, arch.Object, platform.Object);

            Given_CodeBlock(Address.Ptr32(0x40FF9), 4);

            var mcdm = new MixedCodeDataModel(program);

            mcdm.MoveToLine(mcdm.CurrentPosition, 2);
            var curPos = mcdm.CurrentPosition;

            /*
            ***************start position**************************
            0x40FD5                FF FF FF FF FF FF FF FF FF FF FF
            0x40FE0 FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF
            *********curent position after moving******************
            0x40FF0 FF FF FF FF FF FF FF FF FF
            0x40FF9 add r2,r2
            0x40FFA add r2,r2
            */

            Assert.AreEqual("00040FF0(0)", curPos.ToString());
        }

        [Test]
        public void Mcdm_MoveFraction()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[4]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            var program = new Program(segmentMap, arch.Object, platform.Object);

            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);
            Debug.Print("LineCount: {0}", mcdm.LineCount);
            mcdm.SetPositionAsFraction(0, 1);
            Assert.AreSame(mcdm.StartPosition, mcdm.CurrentPosition);
            mcdm.SetPositionAsFraction(2, 1);
            Assert.AreSame(mcdm.EndPosition, mcdm.CurrentPosition);
            mcdm.SetPositionAsFraction(1, 2);
            Assert.AreEqual("00042000(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_GetPositionAsFraction()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[4]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[32]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute),
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);
            var num_lines = 4;
            for (int i = 0; i <= num_lines; i++)
            {
                mcdm.MoveToLine(mcdm.StartPosition, i);
                var frac = mcdm.GetPositionAsFraction();

                var format = @"
  Expected: {0}/{1}
  But was:  {2}/{3}
";
                var msg = string.Format(format, i, num_lines, frac.Item1, frac.Item2);
                Assert.IsTrue((i * frac.Item2 == num_lines * frac.Item1), msg);
            }
        }

        [Test(Description = "After placing the curpos in a 'gap' in the address space, we expect MoveToLine to 'do the right thing'")]
        public void Mcdm_MoveToLine_FromInvalidPosition()
        {
            var addrBase = Address.Ptr32(0x40000);

            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[100]);
            var memData = new MemoryArea(Address.Ptr32(0x42000), new byte[8]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute) { Size = 4 },
                new ImageSegment(".data", memData, AccessMode.ReadWriteExecute));
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);

            var mcdm = new MixedCodeDataModel(program);

            // Read the two instructions, placing curpos in the 'gap'
            // of invalid addresses between the .text and .data segments
            // GetLineSpans should Sanitize the addres and move it to
            // the beginning of .data
            var lines = mcdm.GetLineSpans(2);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("00042000(0)", mcdm.CurrentPosition.ToString());

            // Advance a line. 
            int cLines = mcdm.MoveToLine(mcdm.CurrentPosition, 1);
            Assert.AreEqual(1, cLines);
            Assert.AreEqual("00042008(0)", mcdm.CurrentPosition.ToString());
        }

        [Test]
        public void Mcdm_GetLineSpans_Comments()
        {
            var addrBase = Address.Ptr32(0x40000);
            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[100]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute)
                {
                    Size = 6
                });
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 6);
            Given_Comment(0x41000,
@"This is comment
Second line");
            Given_Comment(0x41004, "Single line comment");

            var mcdm = new MixedCodeDataModel(program);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);
            MoveToLine(mcdm, 0x41004);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);

            AssertOutput(
@"00041000(0):cmt(; This is comment)
00041000(1):cmt(; Second line)
00041000(2):link(00041000 )b(00 00 )op(add )
00041004(0):cmt(; Single line comment)
00041004(1):link-last(00041004 )b-last(00 00 )op-last(add )
");
        }

        [Test]
        public void Mcdm_GetLineSpans_TwoComments()
        {
            var addrBase = Address.Ptr32(0x40000);
            var memText = new MemoryArea(Address.Ptr32(0x41000), new byte[100]);
            this.segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(".text", memText, AccessMode.ReadExecute)
                {
                    Size = 4
                });
            Given_Program();
            Given_CodeBlock(memText.BaseAddress, 4);
            Given_Comment(0x41000, "First comment");
            Given_Comment(0x41002, "Second comment");

            var mcdm = new MixedCodeDataModel(program);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);
            GetLineSpans(mcdm, 1);

            AssertOutput(
@"00041000(0):cmt(; First comment)
00041000(1):link(00041000 )b(00 00 )op(add )
00041002(0):cmt(; Second comment)
00041002(1):link-last(00041002 )b-last(00 00 )op-last(add )
");
        }
    }
}
