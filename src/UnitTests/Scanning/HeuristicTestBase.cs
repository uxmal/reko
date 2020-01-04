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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Scanning;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    public class HeuristicTestBase
    {
        protected const string TrickyProc =
            "55 89 e5 e8 00 00 74 11 " +
            "0a 05 3c 00 75 06 " +
            "b0 00 " +
            "eb 07 " +
            "0a 05 a1 00 00 74 " +
            "01 89 ec 5d c3 90";

        protected Program program;
        protected ImageSegment segment;
        protected Mock<IRewriterHost> host;
        protected Mock<DecompilerEventListener> eventListener;
        private MemoryArea mem;

        public virtual void Setup()
        {
            eventListener = new Mock<DecompilerEventListener>();
        }

        protected MemoryArea CreateMemoryArea(Address addr, params uint[] opcodes)
        {
            byte[] bytes = new byte[0x20];
            var writer = new LeImageWriter(bytes);
            uint offset = 0;
            for (int i = 0; i < opcodes.Length; ++i, offset += 4)
            {
                writer.WriteLeUInt32(offset, opcodes[i]);
            }
            return new MemoryArea(addr, bytes);
        }

        protected void Given_RewriterHost()
        {
            host = new Mock<IRewriterHost>();
            host.Setup(h => h.PseudoProcedure(
                It.IsAny<string>(),
                It.IsAny<DataType>(),
                It.IsAny<Expression[]>()))
               .Returns((string n, DataType dt, Expression[] a) =>
                {
                    var fn = new FunctionType();
                    var ppp = new PseudoProcedure(n, fn);
                    return new Application(new ProcedureConstant(fn, ppp),
                        dt,
                        a);
            });
        }

        protected void Given_Image32(uint addr, string sBytes)
        {
            var bytes = HexStringToBytes(sBytes);
            mem = new MemoryArea(Address.Ptr32(addr), bytes);
            program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("prôg", mem, AccessMode.ReadExecute))
            };
            program.ImageMap = program.SegmentMap.CreateImageMap();
            segment = program.SegmentMap.Segments.Values.First();
        }

        protected void Given_DataBlob(uint uAddr, DataType dt, string sBytes)
        {
            var bytes = HexStringToBytes(sBytes);
            var addr = Address.Ptr32(uAddr);
            var w = program.CreateImageWriter(program.Architecture, addr);
            w.WriteBytes(bytes);
            program.ImageMap.AddItemWithSize(
                addr,
                new ImageMapItem((uint)dt.Size)
                {
                    DataType = dt
                });
        }

        protected void Given_NoImportedProcedures()
        {
            host.Setup(h => h.GetImportedProcedure(
                It.IsAny<IProcessorArchitecture>(),
                It.IsAny<Address>(),
                It.IsAny<Address>()))
                .Returns((ExternalProcedure)null);
            host.Setup(h => h.GetImport(
                It.IsAny<Address>(),
                It.IsAny<Address>()))
                .Returns((Expression)null);
        }

        private static byte[] HexStringToBytes(string sBytes)
        {
            int chunkSize = 2;
            var str = sBytes.Replace(" ", "");

            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => (byte)Convert.ToInt16(
                    str.Substring(i * chunkSize, chunkSize),
                    16))
                .ToArray();
        }

        protected void Given_x86_32()
        {
            program.Architecture = new X86ArchitectureFlat32("x86-protected-32");
            program.Platform = new DefaultPlatform(null, program.Architecture);
            program.Platform.Heuristics.ProcedurePrologs = new MaskedPattern[] {
                new MaskedPattern
                {
                    Bytes = new byte[] {0x55, 0x8B, 0xEC },
                    Mask = new byte[] { 0xFF, 0xFF, 0xff }
                }
            };
        }

        internal void Given_x86_16()
        {
            program.Architecture = new X86ArchitectureReal("x86-real-16");
        }

        internal void Given_ImageSeg(ushort seg, ushort offset, string sBytes)
        {
            var bytes = HexStringToBytes(sBytes);
            mem = new MemoryArea(Address.SegPtr(seg, offset), bytes);
            segment = new ImageSegment("prôg", mem, AccessMode.ReadExecute);
            program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    segment)
            };
        }

        protected void AssertBlocks(string sExpected, DirectedGraph<RtlBlock> cfg)
        {
            var sb = new StringBuilder();
            foreach (var hblock in cfg.Nodes.OrderBy(hb => hb.Address))
            {
                sb.AppendFormat("{0}:  // pred:", hblock.Name);
                foreach (var pred in cfg.Predecessors(hblock).OrderBy(hb => hb.Address))
                {
                    sb.AppendFormat(" {0}", pred.Name);
                }
                sb.AppendLine();
                var lastAddr = hblock.GetEndAddress();
                var dasm = program.Architecture.CreateDisassembler(
                    program.Architecture.CreateImageReader(mem, hblock.Address));
                foreach (var instr in dasm.TakeWhile(i => i.Address < lastAddr))
                {
                    sb.AppendFormat("    {0}", instr);
                    sb.AppendLine();
                }
            }
            var sActual = sb.Replace('\t', ' ').ToString();
            if (sActual != sExpected)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }
    }
}
