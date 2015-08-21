﻿#region License
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.Scanning;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    public class HeuristicTestBase
    {
        protected Program prog;
        protected MockRepository mr;
        protected IRewriterHost host;

        public virtual void Setup()
        {
            mr = new MockRepository();
        }

        protected LoadedImage CreateImage(Address addr, params uint[] opcodes)
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

        protected void Given_RewriterHost()
        {
            host = mr.Stub<IRewriterHost>();
            host.Stub(h => h.EnsurePseudoProcedure(null, null, 0))
                .IgnoreArguments()
                .Do(new Func<string, DataType, int, PseudoProcedure>((n, dt, a) =>
                {
                    return new PseudoProcedure(n, dt, a);
                }));
        }

        protected void Given_Image32(uint addr, string sBytes)
        {
            var bytes = HexStringToBytes(sBytes);
            var imag = new LoadedImage(Address.Ptr32(addr), bytes);
            prog = new Program
            {
                Image = imag,
                ImageMap = imag.CreateImageMap(),
            };
        }

        private static byte[] HexStringToBytes(string sBytes)
        {
            int chunkSize = 2;
            var str = sBytes.Replace(" ", "");

            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i =>  (byte)Convert.ToInt16(
                    str.Substring(i * chunkSize, chunkSize),
                    16))
                .ToArray();
        }

        protected void Given_x86_32()
        {
            prog.Architecture = new X86ArchitectureFlat32();
            prog.Platform = new DefaultPlatform(null, prog.Architecture);
            prog.Platform.Heuristics.ProcedurePrologs = new BytePattern[] {
                new BytePattern
                {   
                    Bytes = new byte[] {0x55, 0x8B, 0xEC },
                    Mask = new byte[] { 0xFF, 0xFF, 0xff }
                }
            };
        }

        internal void Given_x86_16()
        {
            prog.Architecture = new X86ArchitectureReal();
        }

        internal void Given_ImageSeg(ushort seg, ushort offset, string sBytes)
        {
            var bytes = HexStringToBytes(sBytes);
            var imag = new LoadedImage(Address.SegPtr(seg, offset), bytes);
            prog = new Program
            {
                Image = imag,
                ImageMap = imag.CreateImageMap()
            };
        }

        protected void AssertBlocks(string sExpected, DirectedGraph<HeuristicBlock> cfg)
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
                var dasm = prog.Architecture.CreateDisassembler(
                    prog.Architecture.CreateImageReader(prog.Image, hblock.Address));
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

        protected const string TrickyProc =
            "55 89 e5 e8 00 00 74 11 " +
            "0a 05 3c 00 75 06 " +
            "b0 00 " +
            "eb 07 " +
            "0a 05 a1 00 00 74 " +
            "01 89 ec 5d c3 90";
    }
}
