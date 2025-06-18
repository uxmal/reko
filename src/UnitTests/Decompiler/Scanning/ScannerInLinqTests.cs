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
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ScannerInLinqTests
    {
        private readonly string nl = Environment.NewLine;

        private ScanResults sr;
        private ScannerInLinq siq;
        private Program program;
        private RelocationDictionary rd;
        private ServiceContainer sc;
        private Mock<IRewriterHost> host;
        private FakeDecompilerEventListener eventListener;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            this.host = new Mock<IRewriterHost>();
            this.sr = new ScanResults
            {
                FlatInstructions = new Dictionary<ulong, ScanResults.Instr>(),
                FlatEdges = new List<ScanResults.Link>(),
                KnownProcedures = new HashSet<Address>(),
                DirectlyCalledAddresses = new Dictionary<Address, int>()
            };
            this.eventListener = new FakeDecompilerEventListener();
        }

        private void Given_x86_Image(params byte[] bytes)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            CreateProgram(image, arch);
        }

        private void Given_Image(IProcessorArchitecture arch, params byte[] bytes)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            CreateProgram(image, arch);
        }

        private void Given_x86_Image(Action<X86Assembler> asm)
        {
            var addrBase = Address.Ptr32(0x100000);
            var arch = new X86ArchitectureFlat32(sc, "x86-protected-32", new Dictionary<string, object>());
            var entry = ImageSymbol.Procedure(arch, addrBase);
            var m = new X86Assembler(arch, addrBase, new List<ImageSymbol> { entry });
            asm(m);
            this.program = m.GetImage();
            this.program.Platform = new DefaultPlatform(null, arch);
        }

        private void CreateProgram(ByteMemoryArea bmem, IProcessorArchitecture arch)
        {
            var segmentMap = new SegmentMap(bmem.BaseAddress);
            var seg = segmentMap.AddSegment(new ImageSegment(
                ".text",
                bmem,
                AccessMode.ReadExecute)
            {
                Size = (uint)bmem.Bytes.Length
            });
            seg.Access = AccessMode.ReadExecute;
            var platform = new DefaultPlatform(null, arch);
            program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform);
        }

        private void Given_CodeBlock(IProcessorArchitecture arch, uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var proc = Procedure.Create(arch, addr, arch.CreateFrame());
            var block = new Block(proc, addr, $"l{addr}");
            program.ImageMap.AddItem(addr, new ImageMapBlock(addr)
            { 
                Block = block,
                Size = len
            });
        }

        private void Given_UnknownBlock(uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var item = new ImageMapItem(addr) { Size = len };
            program.ImageMap.AddItem(addr, item);
        }

        private void Given_DataBlock(uint uAddr, uint len)
        {
            var addr = Address.Ptr32(uAddr);
            var item = new ImageMapItem(addr)
            {
                Size = len,
                DataType = new ArrayType(PrimitiveType.Byte, 0)
            };
            program.ImageMap.AddItem(addr, item);
        }

        private void AssertBlocks(string sExp, Dictionary<Address, RtlBlock> blocks)
        {
            var sw = new StringWriter();
            this.siq.DumpBlocks(sr, blocks, sw.WriteLine);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.WriteLine("* Failed AssertBlocks ***");
                Debug.Write(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void Lin(int uAddr, int len, int next)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var cluster = new RtlInstructionCluster(addr, len, [])
            {
                Class = InstrClass.Linear
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster,
            });
            Link(addr, next);
        }

        private void Call(int uAddr, int len, int next, int uAddrDst)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var iclass = InstrClass.Call | InstrClass.Transfer;
            var cluster = new RtlInstructionCluster(addr, len, [
                new RtlCall(
                    Address.Ptr32((uint)uAddrDst),
                    0,
                    iclass)])
            {
                Class = iclass,
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster,
            });
            Link(addr, next);
        }

        private void Bra(int uAddr, int len, int a, int b)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var m = new RtlEmitter([]);
            m.Branch(
                Constant.True(),
                Address.Ptr32((uint)a),
                InstrClass.ConditionalTransfer);
            var cluster = new RtlInstructionCluster(addr, len, m.Instructions.ToArray())
            {
                Class = InstrClass.ConditionalTransfer,
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster
            });
            Link(addr, a);
            Link(addr, b);
        }

        private void BraD(int uAddr, int len, int a, int b, Func<ExpressionEmitter, Expression> generator)
        {
            var addr = Address.Ptr32((uint) uAddr);
            var m = new RtlEmitter([]);
            var iclass = InstrClass.ConditionalTransfer | InstrClass.Delay;
            if (generator is not null)
            {
                var ee = generator(m);
                m.Branch(ee, Address.Ptr32((uint) a), iclass);
            }
            var cluster = new RtlInstructionCluster(addr, len, m.Instructions.ToArray())
            {
                Class = iclass,
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster
            });
            Link(addr, a);
            Link(addr, b);
        }


        private void Bad(int uAddr, int len)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var cluster = new RtlInstructionCluster(addr, len, [])
            {
                Class = InstrClass.Invalid
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster,
            });
        }

        private void End(int uAddr, int len)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var cluster = new RtlInstructionCluster(addr, len, [])
            {
                Class = InstrClass.Transfer
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster,
            });
        }

        private void Pad(uint uAddr, int len, int next)
        {
            var addr = Address.Ptr32((uint)uAddr);
            var cluster = new RtlInstructionCluster(addr, len, [])
            {
                Class = InstrClass.Linear | InstrClass.Padding
            };
            sr.FlatInstructions.Add(addr.ToLinear(), new ScanResults.Instr
            {
                block_id = addr,
                rtl = cluster,
            });
            Link(addr, next);
        }

        private void Link(Address addrFrom, int uAddrTo)
        {
            var addrTo = Address.Ptr32((uint)uAddrTo);
            sr.FlatEdges.Add(new ScanResults.Link(addrFrom, addrTo));
        }

        private void Given_OverlappingLinearTraces()
        {
            Lin(0x100, 2, 0x102);
            Lin(0x101, 2, 0x103);
            Lin(0x102, 2, 0x104);
            Bad(0x103, 2);
            End(0x104, 2);
        }

        private void CreateScanner()
        {
            this.siq = new ScannerInLinq(null, program, host.Object, eventListener);
        }

        [Test]
        public void Siq_Blocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());

            Assert.AreEqual(2, blocks.Count);
        }

        [Test]
        public void Siq_InvalidBlocks()
        {
            Given_OverlappingLinearTraces();

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00000100-00000106 (6): End
";
            #endregion

            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Siq_ShingledBlocks()
        {
            Lin(0x0001B0D7, 2, 0x0001B0D9);
            Lin(0x0001B0D8, 6, 0x0001B0DE);
            Lin(0x0001B0D9, 1, 0x0001B0DA);
            Lin(0x0001B0DA, 1, 0x0001B0DB);
            Bra(0x0001B0DB, 2, 0x0001B0DD, 0x0001B0DE);

            Lin(0x0001B0DC, 2, 0x0001B0DE);
            Lin(0x0001B0DD, 1, 0x0001B0DE);

            End(0x0001B0DE, 2);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            var sExp =
            #region Expected
@"0001B0D7-0001B0DD (6): Bra 0001B0DD, 0001B0DE
0001B0D8-0001B0DE (6): Lin 0001B0DE
0001B0DC-0001B0DE (2): Lin 0001B0DE
0001B0DD-0001B0DE (1): Lin 0001B0DE
0001B0DE-0001B0E0 (2): End
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Siq_Overlapping()
        {
            // At offset 0, we have 0x33, 0xC0, garbage.
            // At offset 1, we have rol al,0x90, ret.
            Lin(0x00010000, 2, 0x00010002);
            Lin(0x00010001, 3, 0x00010004);
            Bad(0x00010002, 3);
            End(0x00010004, 1);
            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);
            var sExp =
            #region Expected
@"00010001-00010005 (4): End
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Siq_Regression_0001()
        {
            Given_x86_Image(
                0x55,                               // 0000
                0x8B, 0xEC,                         // 0001
                0x81, 0xEC, 0x68, 0x01, 0x00, 0x00, // 0003
                0x53,                               // 0009
                0x56,                               // 000A
                0x57,                               // 000B
                0x8D, 0xBD, 0x98, 0xFE, 0xFF, 0xFF, // 000C
                0xB9, 0x5A, 0x00, 0x00, 0x00,       // 0012
                0xC3,                               // 0017
                0xC3,
                0xC3);
            CreateScanner();
            var seg = program.SegmentMap.Segments.Values.First();
            var binder = new StorageBinder();
            var scseg = siq.ScanInstructions(sr, binder);
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, binder);
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);
            var sExp =
                "00010000-00010003 (3): Lin 00010003" + nl +
                "00010002-00010003 (1): Lin 00010003" + nl +
                "00010003-00010009 (6): Lin 00010009" + nl +
                "00010004-0001000A (6): Lin 0001000A" + nl +
                "00010006-00010008 (2): Lin 00010008" + nl +
                "00010007-00010009 (2): Zer 00010009" + nl +
                "00010008-0001000B (3): Zer 0001000B" + nl +
                "00010009-0001000A (1): Lin 0001000A" + nl +
                "0001000A-0001000B (1): Lin 0001000B" + nl +
                "0001000B-00010012 (7): Lin 00010012" + nl +
                "0001000D-00010012 (5): Lin 00010012" + nl +
                "00010012-00010017 (5): Lin 00010017" + nl +
                "00010013-00010014 (1): Lin 00010014" + nl +
                "00010014-00010018 (4): Zer 00010018" + nl +
                "00010015-00010017 (2): Zer 00010017" + nl +
                "00010017-00010018 (1): End" + nl +
                "00010018-00010019 (1): End" + nl +
                "00010019-0001001A (1): End" + nl;
            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Siq_Terminate()
        {
            Given_x86_Image(m =>
            {
                m.Hlt();
                m.Mov(m.eax, 0);
                m.Ret();
            });
            CreateScanner();

            var binder = new StorageBinder();
            siq.ScanInstructions(sr, binder);
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, binder);
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var from = blocks.Values.Single(n => n.Address.Offset == 0x00100000);
            var to = blocks.Values.Single(n => n.Address.Offset == 0x00100001);
            Assert.IsFalse(sr.FlatEdges.Any(e => e.From == from.Address && e.To == to.Address));
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Siq_Call_TerminatesBlock()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 2, 0x1005);
            Call(0x1005, 5, 0x100A, 0x1010);
            End(0x100A, 1);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00001000-0000100A (10): Cal 0000100A
0000100A-0000100B (1): End
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Siq_CallThen_Invalid()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 2, 0x1005);
            Call(0x1005, 5, 0x100A, 0x1010);
            Bad(0x100A, 1);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00001000-0000100A (10): Cal 
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        [Test(Description = "Don't make blocks containing a possible call target")]
        public void Siq_calltargetinBlock()
        {
            Lin(0x1000, 3, 0x1003);
            Lin(0x1003, 5, 0x1008);
            Lin(0x1008, 4, 0x100C);
            End(0x100C, 4);

            CreateScanner();
            sr.KnownProcedures = [];
            sr.DirectlyCalledAddresses = new Dictionary<Address, int>
            {
                { Address.Ptr32(0x1008), 2 }
            };
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            Assert.IsTrue(blocks.ContainsKey(Address.Ptr32(0x1008)));
        }

        [Test]
        public void Siq_PaddingBlocks()
        {
            Lin(0x1000, 4, 0x1004);
            End(0x1004, 4);
            Pad(0x1008, 4, 0x100C);
            Pad(0x100C, 4, 0x1010);
            Lin(0x1010, 4, 0x1014);
            End(0x1014, 4);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            var sExp =
            #region Expected
@"00001000-00001008 (8): End
00001008-00001010 (8): Pad 00001010
00001010-00001018 (8): End
";
            #endregion

            AssertBlocks(sExp, blocks);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_AUA()
        {
            var A = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            Given_Image(A.Object, new byte[100]);
            Given_CodeBlock(A.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(A.Object, 0x1040, 60);
            CreateScanner();

            var ranges = siq.FindUnscannedRanges().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreSame(A.Object, ranges[0].Architecture);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_UA()
        {
            var A = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            Given_Image(A.Object, new byte[100]);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(A.Object, 0x1040, 80);
            CreateScanner();

            var ranges = siq.FindUnscannedRanges().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreSame(A.Object, ranges[0].Architecture);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_BUB()
        {
            var A = new Mock<IProcessorArchitecture>();
            var B = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            B.Setup(a => a.Name).Returns("B");

            Given_Image(A.Object, new byte[100]);
            Given_CodeBlock(B.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(B.Object, 0x1040, 60);
            CreateScanner();

            var ranges = siq.FindUnscannedRanges().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreSame(B.Object, ranges[0].Architecture);
        }

        [Test]
        public void Shsc_FindUnscannedRanges_AUBB()
        {
            var A = new Mock<IProcessorArchitecture>();
            var B = new Mock<IProcessorArchitecture>();
            A.Setup(a => a.Name).Returns("A");
            B.Setup(a => a.Name).Returns("B");

            Given_Image(A.Object, new byte[100]);
            Given_CodeBlock(A.Object, 0x1000, 20);
            Given_UnknownBlock(0x1020, 20);
            Given_CodeBlock(B.Object, 0x1040, 60);
            CreateScanner();

            var ranges = siq.FindUnscannedRanges().ToArray();

            Assert.AreEqual(1, ranges.Length);
            Assert.AreSame(B.Object, ranges[0].Architecture);
        }

        [Test(Description = "Stop tracing invalid blocks at call boundaries")]
        public void Siq_CallThen_()
        {
            Call(0x1000, 4, 0x1004, 0x1010);
            Bra(0x1004, 4, 0x1008, 0x100C);
            Bad(0x1008, 4);
            Bad(0x100C, 4);

            Lin(0x1010, 4, 0x1014);
            Bad(0x1014, 4);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00001000-00001004 (4): Cal 
";
            #endregion
            AssertBlocks(sExp, blocks);
        }

        [Test(Description = "Steal delay slot instructions after transfer instructions.")]
        public void Siq_StealDelaySlot()
        {
            Lin(0x1000, 3, 0x1003);
            BraD(0x1003, 2, 0x1005, 0x1007, m => m.Word32(0x123400));
            Lin(0x1005, 2, 0x1007); // Delay slot stolen by the branch.
            End(0x1007, 1);

            CreateScanner();
            var blocks = ScannerInLinq.BuildBasicBlocks(sr, new());
            blocks = ScannerInLinq.RemoveInvalidBlocks(sr, blocks);

            var sExp =
            #region Expected
@"00001000-00001005 (5): BraD 00001005, 00001007
00001005-00001007 (2): Lin 00001007
00001007-00001008 (1): End
";
            #endregion
            AssertBlocks(sExp, blocks);
        }
    }
}
