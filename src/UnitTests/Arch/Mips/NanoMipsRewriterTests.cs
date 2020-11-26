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

using NUnit.Framework;
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class NanoMipsRewriterTests : RewriterTestBase
    {
        private readonly MipsLe32Architecture arch;
        private readonly Address addr;

        public NanoMipsRewriterTests()
        {
            this.arch = new MipsLe32Architecture(CreateServiceContainer(), "nano-mips");
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var rdr = arch.CreateImageReader(mem, mem.BaseAddress);
            var dasm = new NanoMipsDisassembler(arch, rdr);
            var rewriter = new MipsRewriter(arch, rdr, dasm, binder, host);
            return rewriter;
        }

        [Test]
        public void NanoMipsRw_swx()
        {
            Given_HexString("89208734");
            AssertCode(     // swx	r6,r9(r4)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r4 + r9:word32] = r6");
        }

        [Test]
        public void NanoMipsRw_modu()
        {
            Given_HexString("8821D829");
            AssertCode(     // modu	r5,r8,r12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r8 % r12");
        }

        [Test]
        public void NanoMipsRw_lwpc()
        {
            Given_HexString("EB60A4540200");
            AssertCode(     // lwpc	r7,004303A0
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r7 = Mem0[0x001254AA<p32>:word32]");
        }

        [Test]
        public void NanoMipsRw_muhu()
        {
            Given_HexString("2821D850");
            AssertCode(     // muhu	r10,r8,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = SLICE(r8 *u r9, word32, 32)");
        }

        [Test]
        public void NanoMipsRw_swm()
        {
            Given_HexString("DDA4282C");
            AssertCode(     // swm	r6,0028(sp),00000002
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[sp + 40<i32>:word32] = r6",
                "2|L--|Mem0[sp + 44<i32>:word32] = r7");
        }

        [Test]
        public void NanoMipsRw_seqi()
        {
            Given_HexString("E7802560");
            AssertCode(     // seqi	r7,r7,00000025
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = CONVERT(r7 == 0x25<32>, bool, word32)");
        }

        [Test]
        public void NanoMipsRw_sbx()
        {
            Given_HexString("62218760");
            AssertCode(     // sbx	r12,r2(r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + r2:byte] = SLICE(r12, byte, 0)");
        }

        [Test]
        public void NanoMipsRw_lhuxs()
        {
            Given_HexString("87234753");
            AssertCode(     // lhuxs	r5,r16(r30)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = CONVERT(Mem0[r28 + r7 * 2<32>:word16], word16, word32)");
        }

        [Test]
        public void NanoMipsRw_mod()
        {
            Given_HexString("06215839");
            AssertCode(     // mod	r7,r6,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r6 % r8");
        }

        [Test]
        public void NanoMipsRw_rotx()
        {
            Given_HexString("848008D6");
            AssertCode(     // rotx	r4,r4,00000018,00000008
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __rotx(r4, 0x18<32>, 8<32>, 0<32>)");
        }

        [Test]
        public void NanoMipsRw_shxs()
        {
            Given_HexString("8722C702");
            AssertCode(     // shxs	r0,r7(r20)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r20 + r7 * 2<32>:word16] = SLICE(0<32>, word16, 0)");
        }

        [Test]
        public void NanoMipsRw_lhxs()
        {
            Given_HexString("E4204722");
            AssertCode(     // lhxs	r4,r4(r7)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r7 + r4 * 2<32>:int16], int16, int32)");
        }

        [Test]
        public void NanoMipsRw_swpc()
        {
            Given_HexString("EF6016280500");
            AssertCode(     // swpc	r7,004544B8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[0x0015281C<p32>:word32] = r7");
        }
    }
}
