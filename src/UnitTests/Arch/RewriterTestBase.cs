#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch
{
    public abstract class RewriterTestBase : ArchTestBase
    {
        private MemoryArea mem;

        protected static ServiceContainer CreateServiceContainer()
        {
            var sc = new ServiceContainer();
            sc.AddService<ITestGenerationService>(new UnitTestGenerationService(sc));
            return sc;
        }

        public void Given_MemoryArea(MemoryArea mem)
        {
            this.mem = mem;
        }

        protected void Given_Bytes(params byte[] bytes)
        {
            this.mem = Architecture.CreateMemoryArea(LoadAddress, bytes);
        }

        public void Given_HexString(string hexbytes)
        {
            var bytes = BytePattern.FromHexBytes(hexbytes);
            this.mem = Architecture.CreateMemoryArea(LoadAddress, bytes);
        }

        public void Given_OctalBytes(string octalBytes)
        {
            var bytes = OctalStringToBytes(octalBytes);
            this.mem = Architecture.CreateMemoryArea(LoadAddress, bytes);
        }

        public void Given_UInt16s(params ushort[] opcodes)
        {
            byte[] bytes = new byte[opcodes.Length * 2];
            var mem = Architecture.CreateMemoryArea(LoadAddress, bytes);
            var writer = Architecture.CreateImageWriter(mem, mem.BaseAddress);
            foreach (ushort opcode in opcodes)
            {
                writer.WriteUInt16(opcode);
            }
            this.mem = mem;
        }

        public void Given_UInt32s(params uint[] opcodes)
        {
            byte[] bytes = new byte[opcodes.Length * 4];
            var mem = Architecture.CreateMemoryArea(LoadAddress, bytes);
            var writer = Architecture.CreateImageWriter(mem, mem.BaseAddress);
            foreach (uint opcode in opcodes)
            {
                writer.WriteUInt32(opcode);
            }
            this.mem = mem;
        }

        public void Given_BitStrings(params string[] bitStrings)
        {
            var words = bitStrings.Select(bits => base.BitStringToUInt32(bits)).ToArray();
            Given_UInt32s(words);
        }

        protected virtual IRewriterHost CreateHost()
        {
            return new RewriterHost(this.Architecture);
        }

        protected void AssertCode(params string[] expected)
        {
            int i = 0;
            var frame = Architecture.CreateFrame();
            var host = CreateRewriterHost();
            var rewriter = GetRtlStream(mem, frame, host).GetEnumerator();
            while (i < expected.Length && rewriter.MoveNext())
            {
                Assert.AreEqual(expected[i], string.Format("{0}|{1}|{2}", i, RtlInstruction.FormatClass(rewriter.Current.Class), rewriter.Current));
                ++i;
                var ee = rewriter.Current.Instructions.OfType<RtlInstruction>().GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], string.Format("{0}|{1}|{2}", i, RtlInstruction.FormatClass(ee.Current.Class), ee.Current));
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, $"Expected {expected.Length} RTL instructions.");
            Assert.IsFalse(rewriter.MoveNext(), "More instructions were emitted than were expected.");
        }
    }
}
