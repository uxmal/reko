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

using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Pdp11
{
    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        private Pdp11Architecture arch = new Pdp11Architecture("pdp11");
        private Address addrBase = Address.Ptr16(0x0200);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var dasm = new Pdp11Disassembler(arch.CreateImageReader(mem, 0), arch);
            return new Pdp11Rewriter(arch, dasm, binder, base.CreateHost());
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        [Test]
        public void Pdp11Rw_xor()
        {
            Given_UInt16s(0x7811);
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|v3 = Mem0[r1:word16]",
                "2|L--|r1 = r1 + 0x0002",
                "3|L--|r0 = r0 ^ v3",
                "4|L--|NZ = cond(r0)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_mov()
        {
            Given_UInt16s(0x12C2);
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r2 = Mem0[r3:word16]",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_movb()
        {
            Given_UInt16s(0x92C2);
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r2 = (int16) Mem0[r3:byte]",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_clrb()
        {
            Given_UInt16s(0x8A10);
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|Mem0[r0:byte] = 0x00",
                "2|L--|r0 = r0 + 0x0001",
                "3|L--|C = false",
                "4|L--|V = false",
                "5|L--|N = false",
                "6|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_jsr_relative()
        {
            Given_UInt16s(0x09F7, 0x0582);  // jsr\tpc,0582(pc)
            AssertCode(
                "0|T--|0200(4): 1 instructions",
                "1|T--|call 0786 (2)");
        }

        [Test]
        public void Pdp11Rw_br()
        {
            Given_UInt16s(0x01FF);  // br\t0200
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|goto 0200");
        }

        [Test]
        public void Pdp11Rw_bne()
        {
            Given_UInt16s(0x02FE);  // bne\t01FE
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 01FE");
        }

        [Test]
        public void Pdp11Rw_clr()
        {
            Given_UInt16s(0x0A22);  // clr -(r2)
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|r2 = r2 - 0x0002",
                "2|L--|Mem0[r2:word16] = 0x0000",
                "3|L--|C = false",
                "4|L--|V = false",
                "5|L--|N = false",
                "6|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_bisb()
        {
            Given_UInt16s(0xD5DF, 0x0020, 0x0024);  // "bisb\t#0024,@#2000",
            AssertCode(
                "0|L--|0200(6): 4 instructions",
                "1|L--|v2 = Mem0[0x0024:byte] | 0x20",
                "2|L--|Mem0[0x0024:byte] = v2",
                "3|L--|NZ = cond(v2)",
                "4|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_tst()
        {
            Given_UInt16s(0x8BF3, 0x0075); // tst 0075(r3)
            AssertCode(
                "0|L--|0200(4): 5 instructions",
                "1|L--|v4 = Mem0[r3 + 0x0075:byte]",
                "2|L--|v4 = v4 & v4",
                "3|L--|NZ = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_bic()
        {
            Given_UInt16s(0x45C4, 0x0001); // bic r4,#0001
            AssertCode(
                "0|L--|0200(4): 3 instructions",
                "1|L--|r4 = r4 & ~0x0001",
                "2|L--|NZ = cond(r4)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_add()
        {
            Given_UInt16s(0x65C2, 0x00B2); // add #00B2,r2
            AssertCode(
                "0|L--|0200(4): 2 instructions",
                "1|L--|r2 = r2 + 0x00B2",
                "2|L--|NZVC = cond(r2)");
        }

        [Test]
        public void Pdp11Rw_sub()
        {
            Given_UInt16s(0xE5C6, 0x0010); // sub #0010,sp
            AssertCode(
                "0|L--|0200(4): 2 instructions",
                "1|L--|sp = sp - 0x0010",
                "2|L--|NZVC = cond(sp)");
        }

        [Test]
        public void Pdp11Rw_sub_indirect()
        {
            Given_UInt16s(0xE5CE, 0x000A);  // sub #000A,@sp
            AssertCode(
                "0|L--|0200(4): 3 instructions",
                "1|L--|v3 = Mem0[sp:word16] - 0x000A",
                "2|L--|Mem0[sp:word16] = v3",
                "3|L--|NZVC = cond(v3)");
        }

        [Test]
        public void Pdp11Rw_bit()
        {
            Given_UInt16s(0x35C0, 0x1000); // bit #1000,r0
            AssertCode(
                "0|L--|0200(4): 3 instructions",
                "1|L--|r0 = r0 & 0x1000",
                "2|L--|NZ = cond(r0)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_tst_predec()
        {
            Given_UInt16s(0x0BE4);          // tst -(r4)
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|r4 = r4 - 0x0002",
                "2|L--|v4 = Mem0[r4:word16]",
                "3|L--|v4 = v4 & v4",
                "4|L--|NZ = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_dec()
        {
            Given_UInt16s(0x0AC2);          // dec r2
            AssertCode(
                "0|L--|0200(2): 2 instructions",
                "1|L--|r2 = r2 - 0x0001",
                "2|L--|NZV = cond(r2)");
        }

        [Test]
        public void Pdp11Rw_Const()
        {
            Given_UInt16s(0x15C0, 0x0397);          // mov #0397,r0
            AssertCode(
                  "0|L--|0200(4): 3 instructions",
                  "1|L--|r0 = 0x0397",
                  "2|L--|NZ = cond(r0)",
                  "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_Indexed()
        {
            Given_UInt16s(0x65F3, 0x0022, 0x0050); // add #0022,0050(r3)
            AssertCode(
                "0|L--|0200(6): 3 instructions",
                "1|L--|v3 = Mem0[r3 + 0x0050:word16] + 0x0022",
                "2|L--|Mem0[r3 + 0x0050:word16] = v3",
                "3|L--|NZVC = cond(v3)");
        }

        [Test]
        public void Pdp11Rw_IndexDeferred_pc()
        {
            Given_UInt16s(0x453F, 0x7272); // bic(r4)+,@7272(pc)
            AssertCode(
                "0|L--|0200(4): 6 instructions",
                "1|L--|v3 = Mem0[r4:word16]",
                "2|L--|r4 = r4 + 0x0002",
                "3|L--|v5 = Mem0[0x0204:word16] & ~v3",
                "4|L--|Mem0[0x0204:word16] = v5",
                "5|L--|NZ = cond(v5)",
                "6|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_PreDecDef()
        {
            Given_UInt16s(0x1AE6);  //  mov @-(r3),-(sp)
            AssertCode(
                "0|L--|0200(2): 6 instructions",
                "1|L--|r3 = r3 - 0x0002",
                "2|L--|v3 = Mem0[Mem0[r3:ptr16]:word16]",
                "3|L--|sp = sp - 0x0002",
                "4|L--|Mem0[sp:word16] = v3",
                "5|L--|NZ = cond(v3)",
                "6|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_jmp()
        {
            Given_UInt16s(0x004C);  //﻿ 4C 00 jmp @r4
            AssertCode(
                "0|T--|0200(2): 1 instructions",
                "1|T--|goto r4");
        }

        [Test]
        public void Pdp11Rw_rts()
        {
            Given_UInt16s(0x0087); // rts pc
            AssertCode(
                  "0|T--|0200(2): 1 instructions",
                  "1|T--|return (2,0)");
        }

        [Test]
        public void Pdp11Rw_PostIncrDef()
        {
            Given_UInt16s(0x0054); // jmp @(r4)+
            AssertCode(
                 "0|T--|0200(2): 3 instructions",
                 "1|L--|v3 = Mem0[r4:ptr16]",
                 "2|L--|r4 = r4 + 0x0002",
                 "3|T--|goto v3");
        }

        [Test]
        public void Pdp11Rw_PostIncDst()
        {
            Given_UInt16s(0x3520);  // bit (r4)+,-(r0)
            AssertCode(
                "0|L--|0200(2): 7 instructions",
                "1|L--|v3 = Mem0[r4:word16]",
                "2|L--|r4 = r4 + 0x0002",
                "3|L--|r0 = r0 - 0x0002",
                "4|L--|v5 = Mem0[r0:word16] & v3",
                "5|L--|Mem0[r0:word16] = v5",
                "6|L--|NZ = cond(v5)",
                "7|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_sxt()
        {
            Given_UInt16s(0x0DC0);  // sxt r0
            AssertCode(
                "0|L--|0200(2): 2 instructions",
                "1|L--|r0 = 0 - N",
                "2|L--|Z = cond(r0)");
        }

        [Test]
        public void Pdp11Rw_jmpRel()
        {
            // 78 00 C2 1C jmp @1CC2(r0)
            Given_UInt16s(0x0078, 0x1CC2);
            AssertCode(
                "0|T--|0200(4): 1 instructions",
                "1|T--|goto Mem0[r0 + 0x1CC2:ptr16]");
        }

        [Test]
        public void Pdp11Rw_div()
        {
            //17 72 C8 00 div #00C8,r0
            Given_UInt16s(0x7217, 0x00C8);
            AssertCode(
                "0|L--|0200(4): 4 instructions",
                "1|L--|v3 = r0_r1",
                "2|L--|r0 = v3 / 0x00C8",
                "3|L--|r1 = v3 % 0x00C8",
                "4|L--|NZVC = cond(r0)");
        }

        [Test]
        public void Pdp11Rw_movb_imm()
        {
            Given_UInt16s(0x95D5, 0x003D);  // movb #003D,(r5)+
            AssertCode(
              "0|L--|0200(4): 4 instructions",
              "1|L--|Mem0[r5:byte] = 0x3D",
              "2|L--|r5 = r5 + 0x0001",
              "3|L--|NZ = cond(0x3D)",
              "4|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_clrb_reg()
        {
            Given_UInt16s(0x8A03);
            AssertCode(
              "0|L--|0200(2): 5 instructions",
              "1|L--|r3 = DPB(r3, 0x00, 0)",
              "2|L--|C = false",
              "3|L--|V = false",
              "4|L--|N = false",
              "5|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_pc_relative()
        {
            Given_UInt16s(0x1DC0, 0x1B8E);
            AssertCode(
              "0|L--|0200(4): 3 instructions",
              "1|L--|r0 = Mem0[0x1D90:word16]",
              "2|L--|NZ = cond(r0)",
              "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_com()
        {
            Given_UInt16s(0x0A43);
            AssertCode(
              "0|L--|0200(2): 4 instructions",
              "1|L--|r3 = ~r3",
              "2|L--|NZ = cond(r3)",
              "3|L--|V = false",
              "4|L--|C = true");
        }

        [Test]
        public void Pdp11Rw_rts_r5()
        {
            Given_UInt16s(0x0085);
            AssertCode(
              "0|T--|0200(2): 4 instructions",
              "1|L--|v2 = r5",
              "2|L--|r5 = Mem0[sp:word16]",
              "3|L--|sp = sp + 2",
              "4|T--|goto v2");
        }

        [Test]
        public void Pdp11Rw_jsr_indirect_relative()
        {
            // 036C FF 09 8A 0B jsrpc,@0B8A(pc)
            Given_UInt16s(0x09FF, 0x0B8A);
            AssertCode(
                "0|T--|0200(4): 1 instructions",
                "1|T--|call Mem0[0x0D8E:word16] (2)");
        }

        [Test]
        public void Pdp11Rw_jmp_indirect()
        {
            // 79 00 CC 02 jmp@02CC(r1)
            Given_UInt16s(0x0079, 0x02CC);
            AssertCode(
                "0|T--|0200(4): 1 instructions",
                "1|T--|goto Mem0[r1 + 0x02CC:ptr16]");
        }

        [Test]
        public void Pdp11Rw_clr_pcrel_deferred()
        {
            Given_UInt16s(0x0A3F, 0x0010);      // clr @0010(pc)
            AssertCode(
                "0|L--|0200(4): 6 instructions",
                "1|L--|v3 = Mem0[0x0214:ptr16]",
                "2|L--|Mem0[v3:word16] = 0x0000",
                "3|L--|C = false",
                "4|L--|V = false",
                "5|L--|N = false",
                "6|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_xor_pcrel_deferred()
        {
            Given_UInt16s(0x783F, 0x0010);     // "xor\t@0010(pc),r0
            AssertCode(
                "0|L--|0200(4): 6 instructions",
                "1|L--|v3 = Mem0[0x0214:ptr16]",
                "2|L--|v3 = Mem0[v3:word16]",
                "3|L--|r0 = r0 ^ v3",
                "4|L--|NZ = cond(r0)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test(Description = "Destination mustn't be an immediate")]
        public void Pdp11Rw_invalid_dst_immediate()
        {
            Given_UInt16s(0x0A3F, 0x0010);      // clr @0010(pc)
            AssertCode(
                "0|L--|0200(4): 6 instructions",
                "1|L--|v3 = Mem0[0x0214:ptr16]",
                "2|L--|Mem0[v3:word16] = 0x0000",
                "3|L--|C = false",
                "4|L--|V = false",
                "5|L--|N = false",
                "6|L--|Z = true");
        }

        [Test(Description = "Destination mustn't be an immediate")]
        public void Pdp11Rw_invalid_dst_immediate_2()
        {
            // 57 58 59 5A
            Given_UInt16s(0x5857, 0x5A59);
            AssertCode(
                  "0|---|0200(4): 1 instructions",
                  "1|---|<invalid>");
        }

        [Test]
        public void Pdp11Rw_clr_pcrel()
        {
            Given_UInt16s(0x0A37, 0x0010);      // clr\t0010(pc)
            AssertCode(
                "0|L--|0200(4): 5 instructions",
                "1|L--|Mem0[0x0214:word16] = 0x0000",
                "2|L--|C = false",
                "3|L--|V = false",
                "4|L--|N = false",
                "5|L--|Z = true");
        }

        [Test]
        public void Pdp11Rw_Indexed_Deferred()
        {
            Given_UInt16s(0x193C, 0x0A26); // mov-(r4),@0A26(r4)
            AssertCode(
                  "0|L--|0200(4): 5 instructions",
                  "1|L--|r4 = r4 - 0x0002",
                  "2|L--|v4 = Mem0[r4:word16]", 
                  "3|L--|Mem0[Mem0[r4 + 0x0A26:word16]:ptr16] = v4",
                  "4|L--|NZ = cond(v4)",
                  "5|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_Swab()
        {
            Given_UInt16s(0x00C3);      // swab r3
            AssertCode(
                  "0|L--|0200(2): 4 instructions",
                  "1|L--|r3 = __swab(r3)",
                  "2|L--|NZ = cond(r3)",
                  "3|L--|C = false",
                  "4|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_Sob()
        {
            Given_UInt16s(0x7E44);      // sob r0,..
            AssertCode(
                  "0|T--|0200(2): 2 instructions",
                  "1|L--|r1 = r1 - 0x0001",
                  "2|T--|if (r1 != 0x0000) branch 01FA");
        }

        [Test]
        public void Pdp11Rw_mark()
        {
            Given_UInt16s(0x0D27);      // mark 27
            AssertCode(
                "0|T--|0200(2): 5 instructions",
                "1|L--|sp = pc + 78",
                "2|L--|v4 = r5",
                "3|L--|r5 = Mem0[sp:word16]",
                "4|L--|sp = sp + 0x0002",
                "5|T--|goto v4");
        }

        [Test]
        public void Pdp11Rw_mul()
        {
            Given_UInt16s(0x7001);     // mul r1,r0
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r0_r1 = r0 *s r1",
                "2|L--|NZC = cond(r0_r1)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_mul_oddDstReg()
        {
            Given_UInt16s(0x7042);     // mul r2,r1
            AssertCode(
                "0|L--|0200(2): 3 instructions",
                "1|L--|r1 = r1 *s r2",
                "2|L--|NZC = cond(r1)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_clrflags()
        {
            Given_UInt16s(0x00AA);      // clrflags NV
            AssertCode(
                "0|L--|0200(2): 2 instructions",
                "1|L--|N = false",
                "2|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_stexp()
        {
            Given_UInt16s(0xFA4A);      // stexp ac3,@(a2)
            AssertCode(
                "0|L--|0200(2): 5 instructions",
                "1|L--|v4 = __stexp(ac2)",
                "2|L--|Mem0[r2:int16] = v4",
                "3|L--|NZ = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void Pdp11Rw_jsr()
        {
            Given_UInt16s(0x09F7, 0x02D8);  // jsr pc, 0x000004DC
            AssertCode(
               "0|T--|0200(4): 1 instructions",
               "1|T--|call 04DC (2)");
        }

        [Test]
        public void Pdp11Rw_jmp_deferred()
        {
            Given_UInt16s(0x005F, 0x00DC);  // jmp @#00DC
            AssertCode(
               "0|T--|0200(4): 1 instructions",
               "1|T--|goto 00DC");
        }

        [Test]
        public void Pdp11rw_mov_absolute()
        {
            Given_UInt16s(0x15F7, 0x0001, 0x17DA);  // mov #0001,@#57CC
            AssertCode(
                "0|L--|0200(6): 3 instructions",
                "1|L--|Mem0[0x19E0:word16] = 0x0001",
                "2|L--|NZ = cond(0x0001)",
                "3|L--|V = false");
        }

        [Test]
        public void Pdp11rw_mov_pc()
        {
            Given_UInt16s(0x11EA);  // mov pc,*-(r2)
            AssertCode(
                "0|L--|0200(2): 4 instructions",
                "1|L--|r2 = r2 - 0x0002",
                "2|L--|Mem0[Mem0[r2:ptr16]:ptr16] = 0202",
                "3|L--|NZ = cond(0x0202)",
                "4|L--|V = false");
        }

        [Test]
        public void Pdp11rw_jsr_r4()
        {
            Given_UInt16s(0x0937, 0xC9C0);  // jsr r4,CBC4
            AssertCode(
                "0|T--|0200(4): 4 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[sp:word16] = r4",
                "3|L--|r4 = 0204",
                "4|T--|goto CBC4");
        }
    }
}
