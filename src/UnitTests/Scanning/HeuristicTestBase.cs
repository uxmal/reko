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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Types;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
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
                ImageMap = imag.CreateImageMap()
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

    }
}
