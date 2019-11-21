#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Arch.M6800;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6809RewriterTests : RewriterTestBase
    {
        private M6809Architecture arch;
        private Address addr;
        private MemoryArea image;

        [SetUp]
        public void Setup()
        {
            this.arch = new Reko.Arch.M6800.M6809Architecture("m6809");
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = PlatformDefinition.LoadHexBytes(hexBytes)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                new BeImageReader(image, image.BaseAddress),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void M6809Rw_adda_postinc2()
        {
            RewriteCode("AB81"); // adda ,x++
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a + Mem0[x:byte]",
                "2|L--|x = x + 2",
                "3|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6809Rw_addb_postinc1()
        {
            RewriteCode("EBA0"); // addb ,y+
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|b = b + Mem0[y:byte]",
                "2|L--|y = y + 1",
                "3|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6809Rw_addd_predec2()
        {
            RewriteCode("E3C3"); // addd ,--u
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|u = u - 2",
                "2|L--|d = d + Mem0[u:word16]",
                "3|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6809Rw_anda_predec1()
        {
            RewriteCode("A4E2"); // anda ,-s
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|s = s - 1",
                "2|L--|a = a & Mem0[s:byte]",
                "3|L--|NZ = cond(a)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_andb_const_offset()
        {
            RewriteCode("E48880"); // andb -$80,x
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|b = b & Mem0[x - 128:byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_andcc()
        {
            RewriteCode("1CFE"); // andcc #$FE
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|cc = cc & 0xFE");
        }

        [Test]
        public void M6809Rw_asl_acc_offset()
        {
            RewriteCode("68A6"); // asl a,y
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v4 = Mem0[y + (int16) a:byte] << 0x01",
                "2|L--|Mem0[y + (int16) a:byte] = v4",
                "3|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6809Rw_asr_pc_offset()
        {
            RewriteCode("678CFC"); // asr -$04,pc
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|v2 = Mem0[0x00FF:byte] << 0x01",
                "2|L--|Mem0[0x00FF:byte] = v2",
                "3|L--|NZC = cond(v2)");
        }

        [Test]
        public void M6809Rw_bcc()
        {
            RewriteCode("24FC"); // bcc $00FE
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 00FE");
        }

        [Test]
        public void M6809Rw_bita_indirect_extended()
        {
            RewriteCode("A59F1234"); // bita [$1234]
            AssertCode(
                "0|L--|0100(4): 4 instructions",
                "1|L--|v3 = Mem0[Mem0[0x1234:ptr16]:byte]",
                "2|L--|v3 = a & v3",
                "3|L--|NZ = cond(v3)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_bra()
        {
            RewriteCode("20FE"); // bra $0100
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 0100");
        }

        [Test]
        public void M6809Rw_bsr()
        {
            RewriteCode("8D80"); // bsr $0100
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|call 0082 (2)");
        }

        [Test]
        public void M6809Rw_clra()
        {
            RewriteCode("4F"); // bsr $0100
            AssertCode(
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = 0x00",
                "2|L--|N = false",
                "3|L--|Z = true",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void M6809Rw_clr_postInc_indirect()
        {
            RewriteCode("6FB1"); // clr [,y++]
            AssertCode(
                "0|L--|0100(2): 7 instructions",
                "1|L--|v3 = 0x00",
                "2|L--|Mem0[Mem0[y:ptr16]:byte] = v3",
                "3|L--|y = y + 2",
                "4|L--|N = false",
                "5|L--|Z = true",
                "6|L--|V = false",
                "7|L--|C = false");
        }

        [Test]
        public void M6809Rw_cmpa_preDec_indirect()
        {
            RewriteCode("A1D3"); // cmpa [,--u]
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|u = u - 2",
                "2|L--|v4 = Mem0[Mem0[u:ptr16]:byte]",
                "3|L--|v4 = a - v4",
                "4|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6809Rw_com_indexed_indirect()
        {
            RewriteCode("639B"); // com [d,x]
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|v4 = ~Mem0[Mem0[x + d:ptr16]:byte]",
                "2|L--|Mem0[Mem0[x + d:ptr16]:byte] = v4",
                "3|L--|NZ = cond(v4)",
                "4|L--|V = false",
                "5|L--|C = true");
        }

        [Test]
        public void M6809Rw_deca()
        {
            RewriteCode("4A"); // deca
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a - 1",
                "2|L--|NZV = cond(a)");
        }

        [Test]
        public void M6809Rw_dec_extend()
        {
            RewriteCode("7A1234"); // dec $1234
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|v2 = Mem0[0x1234:byte] - 1",
                "2|L--|Mem0[0x1234:byte] = v2",
                "3|L--|NZV = cond(v2)");
        }

        [Test]
        public void M6809Rw_inc_postInc_1()
        {
            RewriteCode("6CA0"); //inc ,y+
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v3 = Mem0[y:byte] + 1",
                "2|L--|Mem0[y:byte] = v3",
                "3|L--|y = y + 1",
                "4|L--|NZV = cond(v3)");
        }

        [Test]
        public void M6809Rw_ldy()
        {
            RewriteCode("10AE04"); // ldy 4,x
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|y = Mem0[x + 4:word16]",
                "2|L--|NZ = cond(y)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_lsl_predec2()
        {
            RewriteCode("68E3"); // lsl ,--s
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|s = s - 2",
                "2|L--|v3 = Mem0[s:byte] << 0x01",
                "3|L--|Mem0[s:byte] = v3",
                "4|L--|NZVC = cond(v3)");
        }

        [Test]
        public void M6809Rw_neg_dir()
        {
            RewriteCode("0042");
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v3 = -Mem0[dp + 0x42:byte]",
                "2|L--|Mem0[dp + 0x42:byte] = v3",
                "3|L--|NZVC = cond(v3)");
        }

        [Test]
        public void M6809Rw_sta()
        {
            RewriteCode("B7FF23"); // sta $FF23
            AssertCode(
                "0|L--|0100(3): 4 instructions",
                "1|L--|v3 = a",
                "2|L--|Mem0[0xFF23:byte] = v3",
                "3|L--|NZ = cond(v3)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_std()
        {
            RewriteCode("EDC1"); // std ,u++
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|v4 = d",
                "2|L--|Mem0[u:word16] = v4",
                "3|L--|u = u + 2",
                "4|L--|NZ = cond(v4)",
                "5|L--|V = false");
        }

        [Test]
        public void M6809Rw_subd_imm()
        {
            RewriteCode("831234"); // subd #$1234
            AssertCode(
                "0|L--|0100(3): 2 instructions",
                "1|L--|d = d - 0x1234",
                "2|L--|NZVC = cond(d)");
        }
    }
}
