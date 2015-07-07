#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.Arm;
using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Scanning;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class HeuristicScannerTests
    {
        private Program prog;
        private MockRepository mr;
        private IRewriterHost host;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        private LoadedImage CreateImage(Address addr, params uint[] opcodes)
        {
            byte[] bytes = new byte[0x20];
            var writer = new LeImageWriter(bytes);
            uint offset = 0;
            for (int i = 0; i < opcodes.Length; ++i, offset += 4)
            {
                writer.WriteLeUInt32(offset, opcodes[i]);
            }
            return new LoadedImage(addr, bytes);
        }

        [Test]
        public void HSC_x86_FindCallOpcode()
        {
            var image = new LoadedImage(Address.Ptr32(0x001000), new byte[] {
                0xE8, 0x03, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
                0xC3
            });
            prog = new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = new IntelArchitecture(ProcessorMode.Protected32),
            };
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);
            var addr = hsc.FindCallOpcodes(new Address[] {
                Address.Ptr32(0x1008)
            }).ToList();

            Assert.AreEqual(1, addr.Count);
            Assert.AreEqual(0x001000, (uint)addr[0].ToLinear());
        }

        [Test]
        public void HSC_x86_FindCallsToProcedure()
        {
            var image = new LoadedImage(Address.Ptr32(0x001000), new byte[] {
                0xE8, 0x0B, 0x00, 0x00,  0x00, 0xE8, 0x07, 0x00,
                0x00, 0x00, 0xC3, 0x00,  0x00, 0x00, 0x00, 0x00,
                0xC3, 0xC3                                      // 1010, 1011
            });
            var prog = new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = new IntelArchitecture(ProcessorMode.Protected32),
            };
            Given_RewriterHost();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);
            var linAddrs = hsc.FindCallOpcodes(new Address[]{
                Address.Ptr32(0x1010),
                Address.Ptr32(0x1011)}).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1000)));
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1005)));
        }

        private void Given_RewriterHost()
        {
            host = mr.Stub<IRewriterHost>();
            host.Stub(h => h.EnsurePseudoProcedure(null, null, 0))
                .IgnoreArguments()
                .Do(new Func<string, DataType, int, PseudoProcedure>((n, dt, a) =>
                {
                    return new PseudoProcedure(n, dt, a);
                }));
        }

        [Test]
        public void HSC_x86_16bitNearCall()
        {
            var image = new LoadedImage(Address.SegPtr(0xC00, 0), new byte[] {
                0xC3, 0x90, 0xE8, 0xFB, 0xFF, 0xC3, 
            });
            var prog = new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = new IntelArchitecture(ProcessorMode.Real),
            };
            var host = mr.Stub<IRewriterHost>();
            var hsc = new HeuristicScanner(prog, host);
            mr.ReplayAll();

            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                Address.SegPtr(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual("0C00:0002", linAddrs[0].ToString());
        }

        [Test]
        public void HSC_x86_16bitFarCall()
        {
            var image = new LoadedImage(Address.SegPtr(0xC00, 0), new byte[] {
                0xC3, 0x90, 0x9A, 0x00, 0x00, 0x00, 0x0C, 0xC3 
            });
            var prog = new Program
            {
                Image = image,
                ImageMap = image.CreateImageMap(),
                Architecture = new IntelArchitecture(ProcessorMode.Real),
            };
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);

            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                Address.SegPtr(0x0C00, 0)}).ToList();

            Assert.AreEqual(1, linAddrs.Count);
            Assert.AreEqual("0C00:0002", linAddrs[0].ToString());
        }

        [Test]
        public void HSC_ARM32_Calls()
        {
            var image = CreateImage(Address.Ptr32(0x1000),
                0xE1A0F00E,     // mov r15,r14 (return)
                0xEBFFFFFD,
                0xEBFFFFFC);
            prog = new Program
            {
                Image = image,
                Architecture = new ArmProcessorArchitecture(),
            };
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);
            var linAddrs = hsc.FindCallOpcodes(new Address[] {
                Address.Ptr32(0x1000),
            }).ToList();

            Assert.AreEqual(2, linAddrs.Count);
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1004)));
            Assert.IsTrue(linAddrs.Contains(Address.Ptr32(0x1008)));
        }

        private void Given_Image32(uint addr, string sBytes)
        {
            var bytes = sBytes.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (byte)Convert.ToInt16(x, 16))
                .ToArray();
            var imag = new LoadedImage(Address.Ptr32(addr), bytes);
            prog = new Program
            {
                Image = imag,
                ImageMap = imag.CreateImageMap()
            };
        }

        private void Given_x86_32()
        {
            prog.Architecture = new X86ArchitectureFlat32();
        }

        [Test]
        public void HSC_FindPossibleProcedureEntries() // "Starts"
        {
            Given_Image32(0x10000, "CC CC CC 55 8B EC C3 00   00 00 55 8B EC");
            Given_x86_32();
            var host = mr.Stub<IRewriterHost>();
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);
            var ranges = hsc.FindPossibleFunctions().ToArray();
            Assert.AreEqual(0x10003, ranges[0].Item1.ToLinear());
            Assert.AreEqual(0x1000A, ranges[0].Item2.ToLinear());
            Assert.AreEqual(0x1000A, ranges[1].Item1.ToLinear());
            Assert.AreEqual(0x1000D, ranges[1].Item2.ToLinear());
        }

        private void AssertBlocks(string sExpected, IEnumerable<HeuristicBlock> hBlocks)
        {
            var sb = new StringBuilder();
            foreach (var hblock in hBlocks.OrderBy(hb => hb.Address))
            {
                sb.AppendFormat("{0}:", hblock.Address);
                sb.AppendLine();
                var lastAddr = hblock.Statements.Last().Address;
                var dasm = prog.Architecture.CreateDisassembler(
                    prog.Architecture.CreateImageReader(prog.Image, hblock.Address));
                foreach (var instr in dasm.TakeWhile(i => i.Address <= lastAddr))
                {
                    sb.AppendFormat("    {0}", instr);
                    sb.AppendLine();
                }
            }
            var sActual = sb.ToString();
            if (sActual != sExpected)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void HSC_HeuristicDisassembleProc()
        {
            Given_Image32(
                0x10000,
                "55 89 e5 e8 00 00 74 11 " +
                "0a 05 3c 00 75 06 " +
                "b0 00 " +
                "eb 07 " +
                "0a 05 a1 00 00 74 " +
                "01 89 ec 5d c3 90");
            Given_x86_32();
            Given_RewriterHost();
            host.Stub(h => h.GetImportedProcedure(null, null))
                .IgnoreArguments()
                .Return(null);
            mr.ReplayAll();

            var hsc = new HeuristicScanner(prog, host);
            var proc = hsc.DisassembleProcedure(
                prog.Image.BaseAddress,
                prog.Image.BaseAddress + prog.Image.Length);
            AssertBlocks(
@"@@@",
                proc.Cfg.Nodes);
        }
    }
}
