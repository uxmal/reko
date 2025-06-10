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
using Reko.Arch.Mips;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Reko.Scanning;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ShingledScannerTests
    {
        private Program program;
        private ShingledScanner sh;
        private RelocationDictionary rd;
        private DiGraph<Address> graph;
        private static readonly string nl = Environment.NewLine;
        private ScanResults sr;
        private Mock<IRewriterHost> host;

        [SetUp]
        public void Setup()
        {
            rd = null;
            this.graph = new DiGraph<Address>();
        }

        [Conditional("DEBUG")]
        private void Dump(ScanResults sr)
        {
            var sw = new StringWriter();
            sw.WriteLine("ICFG");
            foreach (var node in sr.ICFG.Nodes.OrderBy(n => n.Address))
            {
                sw.WriteLine("{0} {1}", sr.DirectlyCalledAddresses.ContainsKey(node.Address) ? "C" : " ", node);
                var succs = sr.ICFG.Successors(node).OrderBy(n => n).ToList();
                if (succs.Count > 0)
                {
                    sw.Write("     ");
                    foreach (var s in succs)
                    {
                        sw.Write(" ");
                        if (!sr.DirectlyCalledAddresses.ContainsKey(s.Address))
                        {
                            // cross procedure tail call.
                            sw.Write("*");
                        }
                        sw.Write(s);
                    }
                    sw.WriteLine();
                }
            }
            Debug.WriteLine(sw.ToString());
        }

        private string DumpBlocks(DiGraph<RtlBlock> blocks)
        {
            var sb = new StringBuilder();
            foreach (var block in blocks.Nodes.OrderBy(b => b.Address))
            {
                sb.AppendFormat("{0} - {1}", block.Address, block.GetEndAddress());
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void Given_Mips_Image(params uint[] words)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                words.Select(w => new[]
                {
                    (byte) w,
                    (byte) (w >> 8),
                    (byte) (w >> 16),
                    (byte) (w >> 24)
                })
                .SelectMany(w => w)
                .ToArray());
            var arch = new MipsLe32Architecture(new ServiceContainer(), "mips-le-32", new Dictionary<string, object>());
            CreateProgram(image, arch);
        }

        private void Given_x86_Image(params byte[] bytes)
        {
            var image = new ByteMemoryArea(
                Address.Ptr32(0x10000),
                bytes);
            this.rd = image.Relocations;
            var arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string,object>());
            CreateProgram(image, arch);
        }

        private void Given_x86_Image(Action<X86Assembler> asm)
        {
            var addrBase = Address.Ptr32(0x100000);
            var arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string, object>());
            var entry = ImageSymbol.Procedure(arch, addrBase);
            var m = new X86Assembler(arch, addrBase, new List<ImageSymbol> { entry });
            asm(m);
            this.program = m.GetImage();
            this.program.Platform = new Win32Platform(arch.Services, arch);
        }

        private void Given_x86_64_Image(params byte[] bytes)
        {
            var bmem = new ByteMemoryArea(
                Address.Ptr64(0x0100000000000000),
                bytes);
            var arch = new X86ArchitectureFlat64(new ServiceContainer(), "x86-protected-64", new Dictionary<string, object>());
            CreateProgram(bmem, arch);
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
            var platform = new DefaultPlatform(arch.Services, arch);
            program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform);
        }

        /// <summary>
        /// Take only each 'n'th item
        /// </summary>
        private T[] TakeEach<T>(IEnumerable<T> items, int n)
        {
            return items.Where((item, i) => (i % n) == 0).ToArray();
        }

        private void Given_Scanner()
        {
            this.host = new Mock<IRewriterHost>();
            var dev = new Mock<IDecompilerEventListener>();
            host.Setup(h => h.GetImport(
                It.IsAny<Address>(),
                It.IsAny<Address>())).Returns((Expression)null);
            host.Setup(h => h.GetImportedProcedure(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<Address>())).Returns((ExternalProcedure)null);
            var frame = program.Architecture.CreateFrame();
            this.sr = new ScanResults
            {
                Instructions = new Dictionary<Address, RtlInstructionCluster>(),
                KnownProcedures = new HashSet<Address>(),
            };
            this.sh = new ShingledScanner(program, host.Object, frame, sr, dev.Object);
        }

        [Test]
        public void Shsc_Invalid()
        {
            Given_Mips_Image(0x00001403);
            Given_Scanner();
            var seg = program.SegmentMap.Segments.Values.First();
            var scseg = sh.ScanRange(new(program.Architecture, seg.MemoryArea, seg.Address, seg.Size), 0);
            Assert.AreEqual(new byte[] { 0 }, TakeEach(scseg, 4));
        }

        [Test]
        public void Shsc_Return_DelaySlot()
        {
            Given_Mips_Image(
                0x03E00008,     // jr ra
                0,              // nop is in the delay slot, so it's safe
                0);             // this nop falls off the end of the segment, it's unsafe.
            Given_Scanner();
            var seg = program.SegmentMap.Segments.Values.First();
            var scseg = sh.ScanRange(new(program.Architecture, seg.MemoryArea, seg.Address, seg.Size), 0);
            Assert.AreEqual(new byte[] { 1, 1, 0 }, TakeEach(scseg, 4));
        }

        [Test]
        public void Shsc_CondJump()
        {
            Given_Mips_Image(
                0x1C60FFFF,     // branch
                0,              // nop
                0x03e00008,     // jr ra
                0);             // nop is in delay slot, so it's safe.
            Given_Scanner();
            var seg = program.SegmentMap.Segments.Values.First();
            var scseg = sh.ScanRange(new(program.Architecture, seg.MemoryArea, seg.Address, seg.Size), 0);
            Assert.AreEqual(new byte[] { 1, 1, 1, 1, }, TakeEach(scseg, 4));
        }

        [Test]
        public void Shsc_FindPossiblePointersToCode()
        {
            Given_x86_Image(
                0x90, 0x90, 0x90, 0x90,      // Padding
                0x55, 0x5B, 0xC3, 0x00,     // Actual code
                0x04, 0x00, 0x01, 0x00);     // Pointer to code
            Given_Scanner();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    1, 0, 0, 0,
                    0, 0, 0, 0,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_FindPossibleMultiplePointersToCode()
        {
            Given_x86_Image(
                0x90, 0x90, 0x90, 0x90,     // Padding
                0x55, 0x5B, 0xC3, 0x00,     // Actual code
                0x04, 0x00, 0x01, 0x00,     // Pointer to code
                0x04, 0x00, 0x01, 0x00);    // Another pointer to code
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    2, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_FindPossiblePointersToEndOfSegment()
        {
            Given_x86_Image(
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0x0F, 0x00, 0x01, 0x00,     // Pointer to last byte in segment
                0xCC, 0xCC, 0xCC, 0xC3);    // Another pointer to code
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 3,
                },
                pointedTo.Values.First());
        }

        [Test]
        public void Shsc_x86_64_FindPossiblePointersToEndOfSegment()
        {
            Given_x86_64_Image(
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,     // Pointer to last byte in segment
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,    // Pointer to last byte in segment
                0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,     // Pointer to last byte in segment
                0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xC3);    // Return at the end
            Given_Scanner();
            var by = sh.ScanExecutableSegments();
            var pointedTo = sh.GetPossiblePointerTargets();
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 3,
                },
                pointedTo.Values.First());
        }

        [Test(Description ="Instructions whose extent overlaps a relocation are not valid.")]
        public void Shsc_Relocation_CrossesInstruction()
        {
            Given_x86_Image(
                0x01, 0x02, 0xC3, 0x04, 0x4);
            rd.AddPointerReference(0x10001, 0x11000000);
            Given_Scanner();

            var seg = program.SegmentMap.Segments.Values.First();
            var scseg = this.sh.ScanRange(new(program.Architecture, seg.MemoryArea, seg.Address, seg.Size), 0);
            Assert.AreEqual(new byte[]
                {
                    0, 0, 0, 0, 0
                },
                scseg);
        }

        private void AssertValidInstructions(byte[] expected, Address addrBase)
        {
            var actual =
                Enumerable.Range(0, expected.Length)
                .Select(n => (byte) (sr.Instructions.ContainsKey(addrBase + n) ? 1 : 0))
                .ToArray();
            Assert.AreEqual(expected, actual);
        }

        private RtlInstructionCluster Lin(uint addr, int length)
        {
            var instr = new RtlInstructionCluster(
                Address.Ptr32(addr), length, new RtlInstruction[1])
            {
                Class = InstrClass.Linear
            };
            return instr;
        }

        private void AddInstr(RtlInstructionCluster instr, params uint[] succs)
        {
            this.sr.Instructions.Add(instr.Address, instr);
            foreach (var succ in succs)
            {
                var addrSucc = Address.Ptr32(succ);
                this.graph.AddNode(instr.Address);
                this.graph.AddNode(addrSucc);
                this.graph.AddEdge(addrSucc, instr.Address);
            }
        }


        [Test]
        public void Shsc_BuildBlock()
        {
            Given_x86_Image();
            Given_Scanner();
            AddInstr(Lin(0x1000, 2), 0x1002);
            AddInstr(Lin(0x1002, 3));

            var icb = sh.BuildBlocks(graph);

            var sExp =
                "00001000 - 00001005" + nl;
            Assert.AreEqual(sExp, DumpBlocks(icb.Blocks));
        }

        [Test]
        public void Shsc_BuildOffsetBlocks()
        {
            Given_x86_Image();
            Given_Scanner();
            AddInstr(Lin(0x1000, 2), 0x1002);
            AddInstr(Lin(0x1001, 2), 0x1003);
            AddInstr(Lin(0x1002, 2), 0x1004);
            AddInstr(Lin(0x1003, 2));
            AddInstr(Lin(0x1004, 2));

            var icb = sh.BuildBlocks(graph);

            var sExp =
                "00001000 - 00001006" + nl +
                "00001001 - 00001005" + nl;
            Assert.AreEqual(sExp, DumpBlocks(icb.Blocks));
        }

        [Test]
        public void Shsc_BuildConvergentBlocks()
        {
            Given_x86_Image();
            Given_Scanner();
            AddInstr(Lin(0x1000, 2), 0x1002);
            AddInstr(Lin(0x1001, 4), 0x1005);
            AddInstr(Lin(0x1002, 3), 0x1005);
            AddInstr(Lin(0x1005, 2));

            var icb = sh.BuildBlocks(graph);

            var sExp =
                "00001000 - 00001005" + nl +
                "00001001 - 00001005" + nl +
                "00001005 - 00001007" + nl;
            Assert.AreEqual(sExp, DumpBlocks(icb.Blocks));
        }

        [Test]
        public void Shsc_Results()
        {
            Given_x86_Image(m =>
            {
                m.Cmp(m.eax, m.ebx);
                m.Jc("skip");
                m.Call("go");
                m.Inc(m.eax);
                m.Label("go");
                m.Mov(m.MemDw(m.esi, 4), m.eax);
                m.Ret();
            });
            Given_Scanner();
            var sr = sh.ScanNew();
            Dump(sr);
        }

    }
}

