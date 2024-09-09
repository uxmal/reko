#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.CompactRisc;
using Reko.Core;

namespace Reko.UnitTests.Arch.CompactRisc
{
    public class Cr16RewriterTests : RewriterTestBase
    {
        public Cr16RewriterTests()
        {
            this.Architecture = new Cr16Architecture(CreateServiceContainer(), "cr16c", new());
            this.LoadAddress = Address.Ptr32(0x3000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        [Test]
        public void Cr16Rw_addb()
        {
            Given_HexString("2030");
            AssertCode(     // addb	$2,r0
                "0|L--|00003000(2): 5 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v5 = SLICE(2<16>, byte, 0)",
                "3|L--|v6 = v4 + v5",
                "4|L--|r0 = SEQ(SLICE(r0, byte, 8), v6)",
                "5|L--|CF = cond(v4)");
        }

        [Test]
        public void Cr16Rw_addd()
        {
            Given_HexString("2F60");
            AssertCode(     // addd	$2,sp
                "0|L--|00003000(2): 2 instructions",
                "1|L--|sp = sp + 2<i32>",
                "2|L--|CF = cond(sp)");
        }

        [Test]
        public void Cr16Rw_addd_imm16()
        {
            Given_HexString("BF60F0FF");
            AssertCode(     // addd	$2,sp
                "0|L--|00003000(4): 2 instructions",
                "1|L--|sp = sp + -16<i32>",
                "2|L--|CF = cond(sp)");
        }

        [Test]
        public void Cr16Rw_addw()
        {
            Given_HexString("9032");
            AssertCode(     // addw	$FFFF,r0
                "0|L--|00003000(2): 2 instructions",
                "1|L--|r0 = r0 + 0xFFFF<16>",
                "2|L--|CF = cond(r0)");
        }

        [Test]
        public void Cr16Rw_addcb()
        {
            Given_HexString("8E34");
            AssertCode(     // addcb	$8,ra
                "0|L--|00003000(2): 5 instructions",
                "1|L--|v4 = SLICE(ra, byte, 0)",
                "2|L--|v5 = SLICE(8<16>, byte, 0)",
                "3|L--|v7 = v4 + v5 + C",
                "4|L--|ra = SEQ(SLICE(ra, word24, 8), v7)",
                "5|L--|CF = cond(v4)");
        }

        [Test]
        public void Cr16Rw_addcw()
        {
            Given_HexString("3936");
            AssertCode(     // addcw	$3,r9
                "0|L--|00003000(2): 2 instructions",
                "1|L--|r9 = r9 + 3<16> + C",
                "2|L--|CF = cond(r9)");
        }

        [Test]
        public void Cr16Rw_addub()
        {
            Given_HexString("402C");
            AssertCode(     // addub $4,r0
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v5 = v4 + 4<8>",
                "3|L--|r0 = SEQ(SLICE(r0, byte, 8), v5)");
        }

        [Test]
        public void Cr16Rw_adduw()
        {
            Given_HexString("FF2F");
            AssertCode(     // adduw	sp,sp
                "0|L--|00003000(2): 4 instructions",
                "1|L--|v4 = SLICE(sp, word16, 0)",
                "2|L--|v5 = SLICE(sp, word16, 0)",
                "3|L--|v6 = v4 + v5",
                "4|L--|sp = SEQ(SLICE(sp, word16, 16), v6)");
        }

        [Test]
        public void Cr16Rw_andb()
        {
            Given_HexString("8020");
            AssertCode(     // andb	$8,r0
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v5 = v4 & 8<8>",
                "3|L--|r0 = SEQ(SLICE(r0, byte, 8), v5)");
        }

        [Test]
        public void Cr16Rw_andd()
        {
            Given_HexString("40000000FFFF");
            AssertCode(     // andd	$FFFF,(r1,r0)
                "0|L--|00003000(6): 1 instructions",
                "1|L--|r1_r0 = r1_r0 & 0xFFFF<32>");
        }

        [Test]
        public void Cr16Rw_andw()
        {
            Given_HexString("B622F801");
            AssertCode(     // andw	$1F8,r6
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r6 = r6 & 0x1F8<16>");
        }

        [Test]
        public void Cr16Rw_ashub()
        {
            Given_HexString("4540");
            AssertCode(     // ashub	$4,r5
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r5, byte, 0)",
                "2|L--|v5 = v4 << 4<8>",
                "3|L--|r5 = SEQ(SLICE(r5, byte, 8), v5)");
        }

        [Test]
        public void Cr16Rw_ashub_reg()
        {
            Given_HexString("4041");
            AssertCode(     // ashub	r4,r0
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v6 = __a_shift<byte,word16>(v4, r4)",
                "3|L--|r0 = SEQ(SLICE(r0, byte, 8), v6)");
        }

        [Test]
        public void Cr16Rw_ashuw()
        {
            Given_HexString("8142");
            AssertCode(     // ashuw	$8,r1
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r1 = r1 << 8<8>");
        }

        [Test]
        public void Cr16Rw_ashuw_right()
        {
            Given_HexString("C243");
            AssertCode(     // ashuw	$-12,r2
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r2 = r2 >> 4<16>");
        }

        [Test]
        public void Cr16Rw_ashud()
        {
            Given_HexString("304C");
            AssertCode(     // ashud	$3,(r1,r0)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r1_r0 = r1_r0 << 3<8>");
        }

        [Test]
        public void Cr16Rw_bal()
        {
            Given_HexString("FFC0C1FE");
            AssertCode(     // bal	ra,031E
                "0|T--|00003000(4): 1 instructions",
                "1|T--|call 00002EC0 (0)");
        }

        [Test]
        public void Cr16Rw_bcc()
        {
            Given_HexString("3211");
            AssertCode(     // bcc	0000235A
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (!C) branch 00003024");
        }

        [Test]
        public void Cr16Rw_bcs()
        {
            Given_HexString("2A11");
            AssertCode(     // bcs	00002372
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (C) branch 00003034");
        }

        [Test]
        public void Cr16Rw_beq()
        {
            Given_HexString("0E15");
            AssertCode(     // beq	027E
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 000030BC");
        }

        [Test]
        public void Cr16Rw_beq0b()
        {
            Given_HexString("060C");
            AssertCode(     // beq0b	r6,0000180A
                "0|L--|00003000(2): 2 instructions",
                "1|L--|v4 = SLICE(r6, byte, 0)",
                "2|T--|if (v4 == 0<8>) branch 00003002");
        }

        [Test]
        public void Cr16Rw_beq0w()
        {
            Given_HexString("620E");
            AssertCode(     // beq0w	r2,000049F0
                "0|L--|00003000(2): 1 instructions",
                "1|T--|if (r2 == 0<16>) branch 0000300E");
        }

        [Test]
        public void Cr16Rw_bfc()
        {
            Given_HexString("9611");
            AssertCode(     // bfc	026E
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (!F) branch 0000302C");
        }

        [Test]
        public void Cr16Rw_bfs()
        {
            Given_HexString("8810");
            AssertCode(     // bfs	0000B088
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (F) branch 00003010");
        }

        [Test]
        public void Cr16Rw_bgt()
        {
            Given_HexString("60184201");
            AssertCode(     // bgt	044E
                "0|T--|00003000(4): 1 instructions",
                "1|T--|if (Test(GT,N)) branch 00003142");
        }

        [Test]
        public void Cr16Rw_bhi()
        {
            Given_HexString("4716");
            AssertCode(     // bhi	000011A2
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (Test(ULT,L)) branch 000030CE");
        }

        [Test]
        public void Cr16Rw_bhs()
        {
            Given_HexString("BC1F");
            AssertCode(     // bhs	0000C3E8
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (Test(ULE,LZ)) branch 00002FF8");
        }

        [Test]
        public void Cr16Rw_blo()
        {
            Given_HexString("A810");
            AssertCode(     // blo	0000B114
                "0|T--|00003000(2): 1 instructions",
                "1|T--|if (Test(UGT,LZ)) branch 00003010");
        }

        [Test]
        public void Cr16Rw_bne0b()
        {
            Given_HexString("0A0D");
            AssertCode(     // bne0b	r10,000001A0
                "0|L--|00003000(2): 2 instructions",
                "1|L--|v4 = SLICE(r10, byte, 0)",
                "2|T--|if (v4 != 0<8>) branch 00003002");
        }

        [Test]
        public void Cr16Rw_bne0w()
        {
            Given_HexString("E00F");
            AssertCode(     // bne0w	r0,044C
                "0|L--|00003000(2): 1 instructions",
                "1|T--|if (r0 != 0<16>) branch 0000301E");
        }

        [Test]
        public void Cr16Rw_br()
        {
            Given_HexString("E0183201");
            AssertCode(     // br	043E
                "0|T--|00003000(4): 1 instructions",
                "1|T--|goto 00003132");
        }

        [Test]
        public void Cr16Rw_cbitb()
        {
            Given_HexString("EF6B0204");
            AssertCode(     // cbitb	$7,(0x0F0402)
                "0|L--|00003000(4): 1 instructions",
                "1|L--|Mem0[0x000F0402<p32>:byte] = __clear_bit<byte,byte>(Mem0[0x000F0402<p32>:byte], 7<8>)");
        }

        [Test]
        public void Cr16Rw_cbitw()
        {
            Given_HexString("026E");
            AssertCode(     // cbitw	$2,(r3,r2)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|Mem0[r3_r2:word16] = __clear_bit<word16,byte>(Mem0[r3_r2:word16], 2<8>)");
        }

        [Test]
        public void Cr16Rw_cinv()
        {
            Given_HexString("0F00");
            AssertCode(     // cinv	[i,d,u]
                "0|L--|00003000(2): 1 instructions",
                "1|L--|__invalidate_cache(\"idu\")");
        }

        [Test]
        public void Cr16Rw_cmpb()
        {
            Given_HexString("B0509FFF");
            AssertCode(     // cmpb	$9F,r0
                "0|L--|00003000(4): 2 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|LNZ = cond(v4 - 0x9F<8>)");
        }

        [Test]
        public void Cr16Rw_cmpd()
        {
            Given_HexString("0456");
            AssertCode(     // cmpd	$0,(r5,r4)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|LNZ = cond(r5_r4 - 0<32>)");
        }

        [Test]
        public void Cr16Rw_cmpw()
        {
            Given_HexString("B0522100");
            AssertCode(     // cmpw	$21,r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|LNZ = cond(r0 - 0x21<16>)");
        }

        [Test]
        public void Cr16Rw_di()
        {
            Given_HexString("0400");
            AssertCode(     // di
                "0|S--|00003000(2): 1 instructions",
                "1|L--|__di()");
        }

        [Test]
        public void Cr16Rw_ei()
        {
            Given_HexString("0500");
            AssertCode(     // ei
                "0|S--|00003000(2): 1 instructions",
                "1|L--|__ei()");
        }

        [Test]
        public void Cr16Rw_eiwait()
        {
            Given_HexString("0700");
            AssertCode(     // eiwait
                "0|L--|00003000(2): 1 instructions",
                "1|L--|__ei_wait()");
        }

        [Test]
        public void Cr16Rw_excp()
        {
            Given_HexString("C600");
            AssertCode(     // excp	DVZ
                "0|L--|00003000(2): 1 instructions",
                "1|L--|__raise_exception(DVZ)");
        }

        [Test]
        public void Cr16Rw_jal()
        {
            Given_HexString("D000");
            AssertCode(     // jal	ra,(r1,r0)
                "0|T--|00003000(2): 1 instructions",
                "1|T--|call r1_r0 (0)");
        }

        [Test]
        public void Cr16Rw_jcs()
        {
            Given_HexString("2A0A");
            AssertCode(     // jcs	(r11,r10)
                "0|T--|00003000(2): 2 instructions",
                "1|T--|if (!C) branch 00003002",
                "2|T--|goto r11_r10");
        }

        [Test]
        public void Cr16Rw_jls()
        {
            Given_HexString("520A");
            AssertCode(     // jls	(r3,r2)
                "0|T--|00003000(2): 2 instructions",
                "1|T--|if (Test(UGT,L)) branch 00003002",
                "2|T--|goto r3_r2");
        }

        [Test]
        public void Cr16Rw_jr_ra()
        {
            Given_HexString("EE0A");
            AssertCode(     // jr	ra
                "0|T--|00003000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void Cr16Rw_loadb_abs()
        {
            Given_HexString("12002071E10F");
            AssertCode(     // loadb	(0x010FE1),r2
                "0|L--|00003000(6): 2 instructions",
                "1|L--|v4 = Mem0[0x00010FE1<p32>:byte]",
                "2|L--|r2 = SEQ(SLICE(r2, byte, 8), v4)");
        }

        [Test]
        public void Cr16Rw_loadd()
        {
            Given_HexString("01870A04");
            AssertCode(     // loadw	(0x01040A),r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r1_r0 = Mem0[0x0001040A<p32>:word32]");
        }

        [Test]
        public void Cr16Rw_loadw()
        {
            Given_HexString("01890A04");
            AssertCode(     // loadw	(0x01040A),r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r0 = Mem0[0x0001040A<p32>:word16]");
        }

        [Test]
        public void Cr16Rw_lpr()
        {
            Given_HexString("14009000");
            AssertCode(     // lpr	r4,DSR
                "0|S--|00003000(4): 1 instructions",
                "1|L--|__write_program_register<word16>(PSR, r0)");
        }

        [Test]
        public void Cr16Rw_lprd()
        {
            Given_HexString("1400A010");
            AssertCode(     // lprd (r1,r0),INTBASE
                "0|S--|00003000(4): 1 instructions",
                "1|L--|__write_program_register<word32>(INTBASEL, r1_r0)");
        }

        [Test]
        public void Cr16Rw_lshb()
        {
            Given_HexString("C009");
            AssertCode(     // lshb	$4,r0
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v5 = v4 << 4<8>",
                "3|L--|r0 = SEQ(SLICE(r0, byte, 8), v5)");
        }

        [Test]
        public void Cr16Rw_lshd()
        {
            Given_HexString("164A");
            AssertCode(     // lshd	$-1,(r7,r6)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r7_r6 = r7_r6 >>u 0x1F<8>");
        }

        [Test]
        public void Cr16Rw_lshw()
        {
            Given_HexString("7049");
            AssertCode(     // lshw	$7,r0
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r0 = r0 >>u 9<8>");
        }

        [Test]
        public void Cr16Rw_lshw_reg()
        {
            Given_HexString("2746");
            AssertCode(     // lshw	r2,r7
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r7 = __l_shift<word16,word16>(r7, r2)");
        }

        [Test]
        public void Cr16Rw_movb_imm()
        {
            Given_HexString("1258");   // movb\t$1,r2
            AssertCode(
                "0|L--|00003000(2): 2 instructions",
                "1|L--|v4 = 1<8>",
                "2|L--|r2 = SEQ(SLICE(r2, byte, 8), v4)");
        }

        [Test]
        public void Cr16Rw_movd()
        {
            Given_HexString("0005C2E2");
            AssertCode(     // movd	$E2C2,(r1,r0)
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r1_r0 = 0xE2C2<32>");
        }

        [Test]
        public void Cr16Rw_movxb()
        {
            Given_HexString("375C");
            AssertCode(     // movxb	r3,(r8,r7)
                "0|L--|00003000(2): 2 instructions",
                "1|L--|v4 = SLICE(r3, byte, 0)",
                "2|L--|r8_r7 = CONVERT(v4, byte, int32)");
        }

        [Test]
        public void Cr16Rw_movxw()
        {
            Given_HexString("005E");
            AssertCode(     // movxw	r0,(r1,r0)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r1_r0 = CONVERT(r0, word16, int32)");
        }

        [Test]
        public void Cr16Rw_movw()
        {
            Given_HexString("B05A409C");
            AssertCode(     // movw	$9C40,r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r0 = 0x9C40<16>");
        }

        [Test]
        public void Cr16Rw_movzb()
        {
            Given_HexString("205D");
            AssertCode(     // movzb	r2,(r1,r0)
                "0|L--|00003000(2): 2 instructions",
                "1|L--|v4 = SLICE(r2, byte, 0)",
                "2|L--|r1_r0 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void Cr16Rw_movzw()
        {
            Given_HexString("005F");
            AssertCode(     // movzw	r0,(r1,r0)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r1_r0 = CONVERT(r0, word16, word32)");
        }

        [Test]
        public void Cr16Rw_mulb()
        {
            Given_HexString("7265");
            AssertCode(     // mulb	r7,r2
                "0|L--|00003000(2): 4 instructions",
                "1|L--|v4 = SLICE(r2, byte, 0)",
                "2|L--|v6 = SLICE(r7, byte, 0)",
                "3|L--|v7 = v4 *s v6",
                "4|L--|r2 = SEQ(SLICE(r2, byte, 8), v7)");
        }

        [Test]
        public void Cr16Rw_mulsb()
        {
            Given_HexString("000B");
            AssertCode(     // mulsb	r0,r0
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(SLICE(r0, word16, 0), int8, 0)",
                "2|L--|v5 = SLICE(r0, int8, 0)",
                "3|L--|r0 = v4 *s16 v5");
        }

        [Test]
        public void Cr16Rw_muluw()
        {
            Given_HexString("0E63");
            AssertCode(     // muluw	r0,ra
                "0|L--|00003000(2): 1 instructions",
                "1|L--|ra = SLICE(ra, word16, 0) *s32 r0");
        }

        [Test]
        public void Cr16Rw_mulw()
        {
            Given_HexString("3266");
            AssertCode(     // mulw	$3,r2
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r2 = r2 *s 3<16>");
        }

        [Test]
        public void Cr16Rw_nop()
        {
            Given_HexString("002C");
            AssertCode(     // nop
                "0|L--|00003000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Cr16Rw_orb()
        {
            Given_HexString("8024");
            AssertCode(     // orb	$0,r8
                "0|L--|00003000(2): 3 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v5 = v4 | 8<8>",
                "3|L--|r0 = SEQ(SLICE(r0, byte, 8), v5)");
        }

        [Test]
        public void Cr16Rw_ord()
        {
            Given_HexString("56008C9FF611");
            AssertCode(     // ord	$9F8C11F6,(r7,r6)
                "0|L--|00003000(6): 1 instructions",
                "1|L--|r7_r6 = r7_r6 | 0x9F8C11F6<32>");
        }

        [Test]
        public void Cr16Rw_orw()
        {
            Given_HexString("B0261001");
            AssertCode(     // orw	$110,r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r0 = r0 | 0x110<16>");
        }

        [Test]
        public void Cr16Rw_popret()
        {
            Given_HexString("9703");
            AssertCode(     // popret	$2,r7,ra
                "0|L--|00003000(2): 5 instructions",
                "1|L--|r8 = Mem0[sp:word16]",
                "2|L--|sp = sp + 2<i32>",
                "3|L--|r7 = Mem0[sp:word16]",
                "4|L--|sp = sp + 2<i32>",
                "5|R--|return (0,0)");
        }

        [Test]
        public void Cr16Rw_push()
        {
            Given_HexString("1001");
            AssertCode(     // push	$2,r0
                "0|L--|00003000(2): 4 instructions",
                "1|L--|sp = sp - 2<i32>",
                "2|L--|Mem0[sp:word16] = r0",
                "3|L--|sp = sp - 2<i32>",
                "4|L--|Mem0[sp:word16] = r1");
        }

        [Test]
        public void Cr16Rw_retx()
        {
            Given_HexString("0300");
            AssertCode(     // retx
                "0|R--|00003000(2): 2 instructions",
                "1|L--|__return_from_exception()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void Cr16Rw_sbitb()
        {
            Given_HexString("100070B18308");
            AssertCode(     // sbitb	$0,(0x010883)
                "0|L--|00003000(6): 1 instructions",
                "1|L--|Mem0[0x00010883<p32>:byte] = __set_bit<byte,byte>(Mem0[0x00010883<p32>:byte], 0<8>)");
        }

        [Test]
        public void Cr16Rw_sbitw()
        {
            Given_HexString("01774C03");
            AssertCode(     // sbitw	$1,(0x01034C)
                "0|L--|00003000(4): 1 instructions",
                "1|L--|Mem0[0x0001034C<p32>:word16] = __set_bit<word16,byte>(Mem0[0x0001034C<p32>:word16], 1<8>)");
        }

        [Test]
        public void Cr16Rw_scc()
        {
            Given_HexString("3608");
            AssertCode(     // scc	r6
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r6 = CONVERT(!C, bool, int16)");
        }

        [Test]
        public void Cr16Rw_sfs()
        {
            Given_HexString("8008");
            AssertCode(     // sfs	r0
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r0 = CONVERT(F != 0<16>, bool, word16)");
        }

        [Test]
        public void Cr16Rw_shs()
        {
            Given_HexString("B008");
            AssertCode(     // shs	r0
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r0 = CONVERT(Test(UGE,LZ), bool, int16)");
        }

        [Test]
        public void Cr16Rw_sls()
        {
            Given_HexString("5E08");
            AssertCode(     // sls	ra
                "0|L--|00003000(2): 1 instructions",
                "1|L--|ra = CONVERT(Test(ULE,L), bool, int32)");
        }

        [Test]
        public void Cr16Rw_spr()
        {
            Given_HexString("14008020");
            AssertCode(     // spr	CFG,r0
                "0|S--|00003000(4): 1 instructions",
                "1|L--|r0 = __read_program_register<word16>(CFG)");
        }

        [Test]
        public void Cr16Rw_storb()
        {
            Given_HexString("0FC884F8");
            AssertCode(     // storb	r0,(0x0FF884)
                "0|L--|00003000(4): 2 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|Mem0[0x000FF884<p32>:byte] = v4");
        }

        [Test]
        public void Cr16Rw_stord()
        {
            Given_HexString("1FE2");
            AssertCode(     // stord	(r2,r1),4(sp)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|Mem0[sp + 4<32>:word32] = r2_r1");
        }

        [Test]
        public void Cr16Rw_storw()
        {
            Given_HexString("0FC986F3");
            AssertCode(     // storw	r0,(0x0FF386)
                "0|L--|00003000(4): 1 instructions",
                "1|L--|Mem0[0x000FF386<p32>:word16] = r0");
        }

        [Test]
        public void Cr16Rw_subb()
        {
            Given_HexString("4039");
            AssertCode(     // subb	r4,r0
                "0|L--|00003000(2): 5 instructions",
                "1|L--|v4 = SLICE(r0, byte, 0)",
                "2|L--|v6 = SLICE(r4, byte, 0)",
                "3|L--|v7 = v4 - v6",
                "4|L--|r0 = SEQ(SLICE(r0, byte, 8), v7)",
                "5|L--|CF = cond(v4)");
        }

        [Test]
        public void Cr16Rw_subd()
        {
            Given_HexString("34000100000B");
            AssertCode(     // subd	$10B00,(r5,r4)
                "0|L--|00003000(6): 2 instructions",
                "1|L--|r5_r4 = r5_r4 - 0x10B00<32>",
                "2|L--|CF = cond(r5_r4)");
        }

        [Test]
        public void Cr16Rw_subcb()
        {
            Given_HexString("D43D");
            AssertCode(     // subcb	r13,r4
                "0|L--|00003000(2): 5 instructions",
                "1|L--|v4 = SLICE(r4, byte, 0)",
                "2|L--|v6 = SLICE(r13, byte, 0)",
                "3|L--|v8 = v4 - v6 - C",
                "4|L--|r4 = SEQ(SLICE(r4, byte, 8), v8)",
                "5|L--|CF = cond(v4)");
        }

        [Test]
        public void Cr16Rw_subcw()
        {
            Given_HexString("B63F");
            AssertCode(     // subcw	r11,r6
                "0|L--|00003000(2): 2 instructions",
                "1|L--|r6 = r6 - r11 - C",
                "2|L--|CF = cond(r6)");
        }

        [Test]
        public void Cr16Rw_tbit()
        {
            Given_HexString("5006");
            AssertCode(     // tbit	$0,r5
                "0|L--|00003000(2): 1 instructions",
                "1|L--|F = __bit<word16,byte>(r5, 0<8>)");
        }

        [Test]
        public void Cr16Rw_tbitb()
        {
            Given_HexString("607A");
            AssertCode(     // tbitb	$0,(r1,r0)
                "0|L--|00003000(2): 1 instructions",
                "1|L--|F = __bit<byte,byte>(Mem0[r1_r0:byte], 0<8>)");
        }

        [Test]
        public void Cr16Rw_tbitw()
        {
            Given_HexString("487F0000");
            AssertCode(     // tbitw	$8,(0x080000)
                "0|L--|00003000(4): 1 instructions",
                "1|L--|F = __bit<word16,byte>(Mem0[0x00080000<p32>:word16], 8<8>)");
        }

        [Test]
        public void Cr16Rw_wait()
        {
            Given_HexString("0600");
            AssertCode(     // wait
                "0|L--|00003000(2): 1 instructions",
                "1|L--|__wait()");
        }

        [Test]
        public void Cr16Rw_xorb()
        {
            Given_HexString("9228");
            AssertCode(     // xorb	$FFFF,r9
                "0|L--|00003000(2): 4 instructions",
                "1|L--|v4 = SLICE(r2, byte, 0)",
                "2|L--|v5 = SLICE(0xFFFF<16>, byte, 0)",
                "3|L--|v6 = v4 ^ v5",
                "4|L--|r2 = SEQ(SLICE(r2, byte, 8), v6)");
        }

        [Test]
        public void Cr16Rw_xord()
        {
            Given_HexString("6000FFFFFFFF");
            AssertCode(     // xord	$FFFFFFFF,(r1,r0)
                "0|L--|00003000(6): 1 instructions",
                "1|L--|r1_r0 = r1_r0 ^ 0xFFFFFFFF<32>");
        }

    }
}
