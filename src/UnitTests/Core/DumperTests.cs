﻿#region License
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
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class DumperTests
    {
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        private void Given_32bit_Program(int size = 32)
        {
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Ptr32);
            this.arch.Stub(a => a.InstructionBitSize).Return(8);
            this.platform = mr.Stub<IPlatform>();
            this.program = new Program(
                new SegmentMap(
                    Address.Ptr32(0x00010000),
                    new ImageSegment(
                        ".text",
                        new MemoryArea(
                            Address.Ptr32(0x00010000),
                            Enumerable.Range(0, size)
                                .Select(i => (byte)i)
                                .ToArray()),
                        AccessMode.ReadWrite)),
                arch,
                platform);
            arch.Stub(a => a.CreateFrame()).Do(
                new Func<Frame>(
                    () => new Frame(PrimitiveType.Ptr32)));
            arch.Stub(a => a.CreateImageReader(null, null)).IgnoreArguments()
                .Do(new Func<MemoryArea, Address, EndianImageReader>(
                    (m, a) => new LeImageReader(m, a)));
            arch.Stub(a => a.CreateDisassembler(null)).IgnoreArguments()
                .Return(new[]
                {
                    Operation.Add,
                    Operation.Mul,
                    Operation.Add,
                    Operation.Ret
                }.Select((m, i) => new FakeInstruction(m)
                {
                    Address = Address.Ptr32(0x00010010 + 2 * (uint)i)
                }));

            arch.Replay();
        }

        private void Given_32bit_Program_Zeros(int size = 32)
        {
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.arch.Stub(a => a.FramePointerType).Return(PrimitiveType.Ptr32);
            this.arch.Stub(a => a.InstructionBitSize).Return(8);
            this.platform = mr.Stub<IPlatform>();
            this.program = new Program(
                new SegmentMap(
                    Address.Ptr32(0x00010000),
                    new ImageSegment(
                        ".text",
                        new MemoryArea(
                            Address.Ptr32(0x00010000),
                            Enumerable.Range(0, size)
                                .Select(i => (byte)0)
                                .ToArray()),
                        AccessMode.ReadWrite)),
                arch,
                platform);
        }

        private Procedure Given_ProcedureAt(Address address)
        {
            var proc = Procedure.Create(arch, address, arch.CreateFrame());
            var block = new Block(proc, Block.GenerateName(address));
            program.Procedures.Add(address, proc);

            program.ImageMap.AddItemWithSize(
                address, new ImageMapBlock
                {
                    Address = address,
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
            Given_ProcedureAt(Address.Ptr32(0x10010));
            mr.ReplayAll();

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
            mr.VerifyAll();
        }

        [Test]
        public void Dumper_NamedProc()
        {
            Given_32bit_Program();
            var proc = Given_ProcedureAt(Address.Ptr32(0x10010));
            proc.Name = "__foo@8";
            mr.ReplayAll();

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
            mr.VerifyAll();
        }

        [Test]
        public void Dumper_Word32()
        {
            Given_32bit_Program();
            program.ImageMap.AddItemWithSize(
                Address.Ptr32(0x10004),
                new ImageMapItem
                {
                    Address = Address.Ptr32(0x10004),
                    DataType = PrimitiveType.Word32,
                    Size = 4,
                });
            mr.ReplayAll();

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
            mr.VerifyAll();
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
            program.ImageMap.AddItemWithSize(
                Address.Ptr32(0x10004),
                new ImageMapItem
                {
                    Address = Address.Ptr32(0x10004),
                    DataType = str,
                    Size = 12,
                });
            mr.ReplayAll();

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
            mr.VerifyAll();
        }

        [Test]
        public void Dumper_LotsOfZeros()
        {
            Given_32bit_Program_Zeros(110);
            arch.Stub(a => a.CreateImageReader(null, null)).IgnoreArguments()
                .Do(new Func<MemoryArea, Address, EndianImageReader>(
                    (m, a) => new LeImageReader(m, a)));
            mr.ReplayAll();

            var addr = program.ImageMap.BaseAddress + 8;
            var item = new ImageMapItem(90) { Address = addr };
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
            mr.VerifyAll();
        }
    }
}
