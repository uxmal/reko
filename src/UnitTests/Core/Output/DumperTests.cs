#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class DumperTests
    {
        private Mock<IProcessorArchitecture> arch;
        private Mock<IPlatform> platform;
        private Program program;

        private void Given_32bit_Program(int size = 32)
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("FakeArch");
            this.arch.Setup(a => a.FramePointerType).Returns(PrimitiveType.Ptr32);
            this.arch.Setup(a => a.InstructionBitSize).Returns(8);
            this.platform = new Mock<IPlatform>();
            var segmentMap = new SegmentMap(
                    Address.Ptr32(0x00010000),
                    new ImageSegment(
                        ".text",
                        new ByteMemoryArea(
                            Address.Ptr32(0x00010000),
                            Enumerable.Range(0, size)
                                .Select(i => (byte) i)
                                .ToArray()),
                        AccessMode.ReadWrite));

            this.program = new Program(
                new ByteProgramMemory(segmentMap),
                arch.Object,
                platform.Object);
            arch.Setup(a => a.CreateFrame()).Returns(
                () => new Frame(arch.Object, PrimitiveType.Ptr32));
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<ByteMemoryArea>(),
                It.IsNotNull<Address>()))
                .Returns((ByteMemoryArea m, Address a) => new LeImageReader(m, a));
            arch.Setup(a => a.CreateImageReader(
                It.IsNotNull<ByteMemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<long>()))
                .Returns((ByteMemoryArea m, Address a, long b) => new LeImageReader(m, a, b));

        }

        private void Given_Disassembly(params Mnemonic[] operations)
        {
            arch.Setup(a => a.CreateDisassembler(
                It.IsNotNull<EndianImageReader>()))
                .Returns(operations.Select((m, i) => new FakeInstruction(m)
                {
                    Address = Address.Ptr32(0x00010010 + 2 * (uint) i),
                    Length = 2,
                }));
        }

        private void Given_32bit_Program_Zeros(int size = 32)
        {
            this.arch = new Mock<IProcessorArchitecture>();
            this.arch.Setup(a => a.Name).Returns("FakeArch");
            this.arch.Setup(a => a.FramePointerType).Returns(PrimitiveType.Ptr32);
            this.arch.Setup(a => a.InstructionBitSize).Returns(8);
            this.platform = new Mock<IPlatform>();
            this.program = new Program(
                new ByteProgramMemory(new SegmentMap(
                    Address.Ptr32(0x00010000),
                    new ImageSegment(
                        ".text",
                        new ByteMemoryArea(
                            Address.Ptr32(0x00010000),
                            new byte[size]),
                        AccessMode.ReadWrite))),
                arch.Object,
                platform.Object);
        }

        private Procedure Given_ProcedureAt(Address address)
        {
            var proc = Procedure.Create(arch.Object, address, arch.Object.CreateFrame());
            var label = program.NamingPolicy.BlockName(address);
            var block = new Block(proc, address, label);
            program.Procedures.Add(address, proc);

            program.ImageMap.AddItemWithSize(
                address, new ImageMapBlock(address)
                {
                    Block = block,
                    Size = 8
                });
            return proc;
        }

        private void AssertOutput(string sExp, StringWriter sw)
        {
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Dumper_Proc()
        {
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);
            Given_ProcedureAt(Address.Ptr32(0x10010));

            var dmp = new Dumper(program);

            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................

;; fn00010010: 00010010
fn00010010 proc
	add
	mul
	add
	ret
00010018                         18 19 1A 1B 1C 1D 1E 1F         ........
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_NamedProc()
        {
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);

            var proc = Given_ProcedureAt(Address.Ptr32(0x10010));
            proc.Name = "__foo@8";

            var dmp = new Dumper(program);

            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................

;; __foo@8: 00010010
__foo@8 proc
	add
	mul
	add
	ret
00010018                         18 19 1A 1B 1C 1D 1E 1F         ........
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_Word32()
        {
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);

            program.ImageMap.AddItemWithSize(
                Address.Ptr32(0x10004),
                new ImageMapItem(Address.Ptr32(0x10004))
                {
                    DataType = PrimitiveType.Word32,
                    Size = 4,
                });

            var dmp = new Dumper(program);

            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03                                     ....            
l00010004	dd	0x07060504
00010008                         08 09 0A 0B 0C 0D 0E 0F         ........
00010010 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F ................
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_Structure()
        {
            var str = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Byte  },
                    { 2, PrimitiveType.Word16 },
                    { 8, PrimitiveType.Word32 }
                }
            };
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);

            program.ImageMap.AddItemWithSize(
                Address.Ptr32(0x10004),
                new ImageMapItem(Address.Ptr32(0x10004))
                {
                    DataType = str,
                    Size = 12,
                });

            var dmp = new Dumper(program);

            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03                                     ....            
l00010004		db	0x04
	db	0x05	; padding
	dw	0x0706
	db	0x08,0x09,0x0A,0x0B	; padding
	dd	0x0F0E0D0C
00010010 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F ................
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_LotsOfZeros()
        {
            Given_32bit_Program_Zeros(110);
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<long>()))
                .Returns(
                    (ByteMemoryArea m, Address a, long b) => new LeImageReader(m, a, b));

            var addr = program.ImageMap.BaseAddress + 8;
            var item = new ImageMapItem(addr, 90);
            program.ImageMap.AddItem(item.Address, item);
            var dmp = new Dumper(program);
            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            var sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 00 00 00 00 00 00 00                         ........        
00010008                         00 00 00 00 00 00 00 00         ........
00010010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00010060 00 00 00 00 00 00 00 00 00 00 00 00 00 00       ..............  
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_ShowAddressesInDisassembly()
        {
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);

            Given_ProcedureAt(Address.Ptr32(0x10010));

            var dmp = new Dumper(program);
            dmp.ShowAddresses = true;
            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................

;; fn00010010: 00010010
fn00010010 proc
00010010 	add
00010012 	mul
00010014 	add
00010016 	ret
00010018                         18 19 1A 1B 1C 1D 1E 1F         ........
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_ShowBytesInDisassembly()
        {
            Given_32bit_Program();
            Given_Disassembly(
                Mnemonic.Add,
                Mnemonic.Mul,
                Mnemonic.Add,
                Mnemonic.Ret);

            Given_ProcedureAt(Address.Ptr32(0x10010));

            var dmp = new Dumper(program);
            dmp.ShowCodeBytes = true;

            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................

;; fn00010010: 00010010
fn00010010 proc
10 11           	add
12 13           	mul
14 15           	add
16 17           	ret
00010018                         18 19 1A 1B 1C 1D 1E 1F         ........
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_ShowCallers()
        {
            Given_32bit_Program(48);
            Given_Disassembly(Mnemonic.Call, Mnemonic.Ret);
            var p1 = Given_ProcedureAt(Address.Ptr32(0x10010));
            var p2 = Given_ProcedureAt(Address.Ptr32(0x10020));

            var label = program.NamingPolicy.BlockName(p1.EntryAddress);
            var b1 = p1.AddBlock(p1.EntryAddress, label);
            var stm = b1.Statements.Add(
                p1.EntryAddress,
                new CallInstruction(
                    new ProcedureConstant(PrimitiveType.Ptr32, p2), new CallSite(0, 0)));
            program.CallGraph.AddEdge(stm, p2);

            var dmp = new Dumper(program);
            var sw = new StringWriter();
            dmp.Dump(new TextFormatter(sw));

            string sExp =
            #region Expected
@";;; Segment .text (00010000)
00010000 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................

;; fn00010010: 00010010
fn00010010 proc
	call
	ret
00010018                         18 19 1A 1B 1C 1D 1E 1F         ........

;; fn00010020: 00010020
;;   Called from:
;;     00010010 (in fn00010010)
fn00010020 proc
	call
	ret
00010028                         28 29 2A 2B 2C 2D 2E 2F         ()*+,-./
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_AlmostFullSpan()
        {
            Given_32bit_Program_Zeros(110);
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<long>()))
                .Returns(
                    (ByteMemoryArea m, Address a, long b) => new LeImageReader(m, a, b));

            var dmp = new Dumper(program);
            var sw = new StringWriter();
            dmp.DumpData(
                program.SegmentMap,
                program.Architecture,
                program.ImageMap.BaseAddress + 1,
                14,
                new TextFormatter(sw));

            var sExp =
            #region Expected
@"00010001    00 00 00 00 00 00 00 00 00 00 00 00 00 00     .............. 
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_Word16_Units()
        {
            program = new Program();
            var arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<long>()))
                .Returns(
                    (ByteMemoryArea m, Address a, long b) => new LeImageReader(m, a, b));

            var mem = new ByteMemoryArea(Address.Ptr32(0x0001_0000), new byte[]
            {
                0x00, 0x00, 0x01, 0x02,  0x03, 0x04, 0x05, 0x06,
                0x07, 0x08, 0xFF, 0xFF,  0x48, 0x69, 0x42, 0x49,
                0x00, 0x00, 0x01, 0x02,  0x03, 0x04, 0x05, 0x06,
                0x07, 0x08, 0xFF, 0xFF,  0x48, 0x69, 0x42, 0x41
            });
            mem.Formatter = new MemoryFormatter(PrimitiveType.Word16, 2, 8, 4, 2);
            var dmp = new Dumper(program);
            var sw = new StringWriter();
            sw.WriteLine();
            dmp.DumpData(
                arch.Object,
                mem,
                mem.BaseAddress + 2,
                0x2C,
                new TextFormatter(sw));

            var sExp =
            #region Expected
@"
00010002      0201 0403 0605 0807 FFFF 6948 4942   ..........HiBI
00010010 0000 0201 0403 0605 0807 FFFF 6948 4142 ............HiBA
";
            #endregion
            AssertOutput(sExp, sw);
        }

        [Test]
        public void Dumper_Word16MemoryArea()
        {
            program = new Program();
            var arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<MemoryArea>(),
                It.IsAny<Address>(),
                It.IsAny<long>()))
                .Returns(
                    (MemoryArea m, Address a, long b) => m.CreateBeReader(a, b));

            var mem = new Word16MemoryArea(Address.Ptr16(0x1000), new ushort[]
            {
                0x0000, 0x0102,  0x0304, 0x0506,
                0x0708, 0xFFFF,  0x4869, 0x4249,
                0x0000, 0x0102,  0x0304, 0x0506,
                0x0708, 0xFFFF,  0x4869, 0x4241
            });
            var dmp = new Dumper(program);
            var sw = new StringWriter();
            sw.WriteLine();
            dmp.DumpData(
                arch.Object,
                mem,
                mem.BaseAddress + 1,
                0x0D,
                new TextFormatter(sw));

            var sExp =
            #region Expected
@"
1001      0102 0304 0506 0708 FFFF 4869 4249   ..........HiBI
1008 0000 0102 0304 0506 0708 FFFF           ............    
";
            #endregion
            AssertOutput(sExp, sw);
        }
    }
}
