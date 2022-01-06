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
using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public partial class A64RewriterTests : RewriterTestBase
    {
        private readonly Arm64Architecture arch = new Arm64Architecture(CreateServiceContainer(), "aarch64", new Dictionary<string, object>());
        private readonly Address baseAddress = Address.Ptr64(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddress;

        private void Given_Instruction(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.BitStringToUInt32(bits))
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            base.Given_MemoryArea(new ByteMemoryArea(Address.Ptr32(0x00100000), bytes));
        }

        private void Given_Instruction(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            Given_MemoryArea(new ByteMemoryArea(Address.Ptr32(0x00100000), bytes));
        }

        [Test]
        public void AArch64Rw_abs()
        {
            Given_HexString("21BA200E");
            AssertCode(     // abs	v1.8b,v17.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d17",
                "2|L--|d1 = __abs_i8(v2)");
        }

        [Test]
        public void AArch64Rw_adc()
        {
            Given_HexString("6300029A");
            AssertCode(     // adc	x3,x3,x2
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x3 = x3 + x2 + C");
        }

        [Test]
        public void AArch64Rw_adcs()
        {
            Given_HexString("630002BA");
            AssertCode(     // adcs	x3,x3,x2
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|x3 = x3 + x2 + C",
                "2|L--|NZCV = cond(x3)");
        }

        [Test]
        public void AArch64Rw_add_Wn_imm()
        {
            Given_Instruction("000 10001 01 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w19 = w17 + (0x7FF<32> << 12<i32>)");
        }

        [Test]
        public void AArch64Rw_addhn2()
        {
            Given_HexString("00402B4E");
            AssertCode(     // addhn2	v0.16b,v0.8h,v11.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q11",
                "3|L--|q0 = __addhn2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_adds_Xn_imm()
        {
            Given_Instruction("101 10001 00 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|x19 = x17 + 0x7FF<64>",
                "2|L--|NZCV = cond(x19)");
        }

        [Test]
        public void AArch64Rw_addp_scalar()
        {
            Given_HexString("21B8F15E");
            AssertCode(     // addp	d1,v1.2d
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = q1",
                "3|L--|d1 = __addp_i64(v3, v2)");
        }

        [Test]
        public void AArch64Rw_addp_vector()
        {
            Given_HexString("21BC210E");
            AssertCode(     // addp	v1.8b,v1.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = d1",
                "3|L--|d1 = __addp_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_addv()
        {
            Given_HexString("0AB8310E");
            AssertCode(     // addv b10,v0.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|b10 = __sum_i8(v2)");
        }

        [Test]
        public void AArch64Rw_adrp()
        {
            Given_Instruction(0xF00000E2);
            AssertCode(     // adrp\tx2,#&1F000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = 000000000011F000");
        }

        [Test]
        public void AArch64Rw_adrp_00001()
        {
            Given_Instruction(0xB0000001);
            AssertCode(     // adrp\tx1,#&1000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x1 = 0000000000101000");
        }

        [Test]
        public void AArch64Rw_ands()
        {
            Given_HexString("4F67DCEA");
            AssertCode(     // ands	x15,x26,x28,ror #&19
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|x15 = x26 & __ror(x28, 25<i32>)",
                "2|L--|NZ = cond(x15)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void AArch64Rw_and_Wn_imm()
        {
            Given_Instruction(0x120F3041);
            AssertCode( // and\tw1,w2,#&3FFE0000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = w2 & 0x3FFE0000<32>");
        }

        [Test]
        public void AArch64Rw_and_Xn_imm()
        {
            Given_Instruction(0x920F3041);
            AssertCode(     // and\tx1,x2,#&3FFE0000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x1 = x2 & 0x3FFE00003FFE0000<64>");
        }

        [Test]
        public void AArch64Rw_ands_Xn_imm()
        {
            Given_Instruction("111 100100 0 010101 010101 00100 00111");
            AssertCode(     // ands\tx7,x4,#&FFFFF801
                "0|L--|00100000(4): 4 instructions",
                "1|L--|x7 = x4 & 0xFFFFF801FFFFF801<64>",
                "2|L--|NZ = cond(x7)",
                "3|L--|C = false",
                "4|L--|V = false");
        }


        [Test]
        public void AArch64Rw_b_label()
        {
            Given_Instruction("00010111 11111111 11111111 00000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFC00");
        }

        [Test]
        public void AArch64Rw_bic_reg_32()
        {
            Given_Instruction(0x0A350021);
            AssertCode(     // bic\tw1,w1,w21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = w1 & ~w21");
        }

        [Test]
        public void AArch64Rw_bics()
        {
            Given_HexString("E3E4EDEA");
            AssertCode(     // bics	x3,x7,x13,ror #&39
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|x3 = x7 & ~__ror(x13, 57<i32>)",
                "2|L--|NZ = cond(x3)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void AArch64Rw_bic_vector_imm()
        {
            Given_HexString("FE94052F");
            AssertCode(     // bic	v30.4h,#&A700A7
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|d30 = d30 & ~0x0A700A700A700A7<64>");
        }

        [Test]
        public void AArch64Rw_bif()
        {
            Given_HexString("C11FEA2E");
            AssertCode(     // bif	v1.8b,v30.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d10",
                "3|L--|d1 = __bif_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_bit()
        {
            Given_HexString("BF1DA16E");
            AssertCode(     // bit	v31.16b,v13.16b,v1.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q13",
                "2|L--|v3 = q1",
                "3|L--|q31 = __bit_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_bl_label()
        {
            Given_Instruction("10010111 11111111 11111111 00000000");
            AssertCode(     // bl\t#&FFC00
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 000FFC00 (0)");
        }

        [Test]
        public void AArch64Rw_br_Xn()
        {
            Given_Instruction("11010110 00011111 00000011 11000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto x30");
        }

        [Test]
        public void AArch64Rw_brk()
        {
            Given_HexString("007D20D4");
            AssertCode(     // brk	#&3E8
                "0|H--|0000000000100000(4): 1 instructions",
                "1|L--|__brk(0x3E8<16>)");
        }

        [Test]
        public void AArch64Rw_bsl()
        {
            Given_HexString("831C606E");
            AssertCode(     // bsl	v3.16b,v4.16b,v0.16b
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|q3 = q3 ^ (q3 ^ q0) & q4");
        }

        [Test]
        public void AArch64Rw_cls()
        {
            Given_HexString("3E48200E");
            AssertCode(     // cls	v30.8b,v1.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d30 = __cls_i8(v2)");
        }

        [Test]
        public void AArch64Rw_cmeq()
        {
            Given_Instruction(0x6E208C21);	// cmeq	v1.16b,v1.16b,v0.16b
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q0",
                "3|L--|q1 = __cmeq_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmge()
        {
            Given_HexString("513D2B0E");
            AssertCode(     // cmge	v17.8b,v10.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d11",
                "3|L--|d17 = __cmge_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmgt()
        {
            Given_HexString("2D34200E");
            AssertCode(     // cmgt	v13.8b,v1.8b,v0.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = d0",
                "3|L--|d13 = __cmgt_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmhi_vs()
        {
            Given_HexString("E034A06E");
            AssertCode( //cmhi\tv0.4s,v7.4s,v0.4s
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q7",
                "2|L--|v3 = q0",
                "3|L--|q0 = __cmhi_i32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmhs()
        {
            Given_HexString("233C236E");
            AssertCode(     // cmhs	v3.16b,v1.16b,v3.16b,#0
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q3",
                "3|L--|q3 = __cmhs_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmhs_v()
        {
            Given_HexString("213CA26E");
            AssertCode(//cmhs\tv1.16b,v1.16b,v2.16b,#0
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q2",
                "3|L--|q1 = __cmhs_i32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cmle()
        {
            Given_HexString("5E99202E");
            AssertCode(     // cmle	v30.8b,v10.8b,#0
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d10",
                "2|L--|d30 = __cmle_i8(v2, 0<32>)");
        }

        [Test]
        public void AArch64Rw_cmlt()
        {
            Given_HexString("E0AB200E");
            AssertCode(     // cmlt	v0.2s,v31.2s,#0
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d0 = __cmlt_i32(v2, 0<64>)");
        }

        [Test]
        public void AArch64Rw_cmtst()
        {
            Given_HexString("AB8D210E");
            AssertCode(     // cmtst	v11.8b,v13.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d13",
                "2|L--|v3 = d1",
                "3|L--|d11 = __cmtst_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_cnt()
        {
            Given_HexString("6C59200E");
            AssertCode(     // cnt	v12.8b,v11.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d11",
                "2|L--|d12 = __cnt_i8(v2)");
        }

        [Test]
        public void AArch64Rw_ldarh()
        {
            Given_HexString("01FCDF48");
            AssertCode(     // ldarh	w1,[x0]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w1 = CONVERT(__load_acquire_word16(&Mem0[x0:word16]), word16, word32)");
        }

        [Test]
        public void AArch64Rw_ldaxrh()
        {
            Given_HexString("06FC5F48");
            AssertCode(     // ldaxrh	w6,[x0]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w6 = CONVERT(__load_acquire_exclusive_word16(&Mem0[x0:word16]), word16, word32)");
        }

        [Test]
        public void AArch64Rw_ldnp()
        {
            Given_HexString("48656C6C");
            AssertCode(     // ldnp	d8,d25,[x10,#-&140]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v5 = x10 + -320<i64>",
                "2|L--|d8 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8<i64>",
                "4|L--|d25 = Mem0[v5:word64]");
        }

        [Test]
        public void AArch64Rw_ldp()
        {
            Given_Instruction(0x2D646C2F);
            AssertCode(     // ldp\ts15,27,[x1,-#&E0]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x1 + -224<i64>",
                "2|L--|s15 = Mem0[v5:word32]",
                "3|L--|v5 = v5 + 4<i64>",
                "4|L--|s27 = Mem0[v5:word32]");
        }

        [Test]
        public void AArch64Rw_ldxr()
        {
            Given_HexString("000E5BC8");
            AssertCode(     // ldxr	x0,[x16]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = &Mem0[x16:word64]",
                "2|L--|x0 = __load_exclusive_word64(v4)");
        }

        [Test]
        public void AArch64Rw_mla_by_element()
        {
            Given_HexString("81016D2F");
            AssertCode(     // mla	v1.4h,v12.4h,v13.h[2]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = d12",
                "3|L--|v4 = d13[2<i32>]",
                "4|L--|d1 = __mla_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_mls_by_element()
        {
            Given_HexString("3E42502F");
            AssertCode(     // mls	v30.8h,v17.8h,v0.h[1]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q30",
                "2|L--|v3 = q17",
                "3|L--|v4 = q0[1<i32>]",
                "4|L--|q30 = __mls_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_movk_imm()
        {
            Given_Instruction("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
            AssertCode(     // movk\tx7,#&AAA4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x7 = SEQ(SLICE(x7, word48, 16), 0xAAA4<16>)");
        }

        [Test]
        public void AArch64_movk()
        {
            Given_HexString("49 86 8A 72 ");
            AssertCode(// movk w9,#0x5432
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w9 = SEQ(SLICE(w9, word16, 16), 0x5432<16>)");
        }

        [Test]
        public void AArch64Rw_ldr_UnsignedOffset()
        {
            Given_Instruction(0xF947E442);
            AssertCode(     // ldr\tx2,[x2,#&FC8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = Mem0[x2 + 4040<i64>:word64]");
        }



        [Test]
        public void AArch64Rw_mov_reg64()
        {
            Given_Instruction(0xAA0103F4);
            AssertCode(     // mov\tx20,x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x20 = x1");
        }



        [Test]
        public void AArch64Rw_mov_reg32()
        {
            Given_Instruction(0x2A0003F5);
            AssertCode(     // mov\tw21,w0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w21 = w0");
        }

        [Test]
        public void AArch64Rw_movz_imm32()
        {
            Given_Instruction(0x528000C0);
            AssertCode(     // movz\tw0,#6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = 6<32>");
        }

        [Test]
        public void AArch64Rw_mvni()
        {
            Given_HexString("7F86016F");
            AssertCode(     // mvni	v31.8h,#&330033
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|q31 = 0x0FFCCFFCCFFCCFFCCFFCCFFCCFFCCFFCC<128>");
        }

        [Test]
        public void AArch64Rw_neg()
        {
            Given_HexString("CBBB202E");
            AssertCode(     // neg	v11.8b,v30.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d30",
                "2|L--|d11 = __neg_i8(v2)");
        }

        [Test]
        public void AArch64Rw_ldrsw()
        {
            Given_Instruction(0xB9800033);
            AssertCode(     // ldrsw\tx19,[x1]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[x1:int32]",
                "2|L--|x19 = CONVERT(v4, int32, int64)");
        }

        [Test]
        public void AArch64Rw_add_reg()
        {
            Given_Instruction(0x8B130280);
            AssertCode(     // add\tx0,x20,x19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = x20 + x19");
        }

        [Test]
        public void AArch64Rw_add_reg_with_shift()
        {
            Given_Instruction(0x8B130A80);
            AssertCode( // add\tx0,x20,x19,lsl #2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = x20 + (x19 << 2<i32>)");
        }

        [Test]
        public void AArch64Rw_cbz()
        {
            Given_Instruction(0xB4001341);
            AssertCode(     // cbz\tx1,#&100268
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (x1 == 0<64>) branch 00100268");
        }

        [Test]
        public void AArch64Rw_lsl()
        {
            Given_Instruction(0xD37DF29C);
            AssertCode(     // ubfm\tx28,x20,#0,#&3D
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x28 = x20 << 3<i32>");
        }

        [Test]
        public void AArch64Rw_ccmp_imm()
        {
            Given_Instruction(0xFA400B84);
            AssertCode(     // ccmp\tx28,#0,#4,EQ
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(NE,Z)",
                "2|L--|NZCV = 0x40000000<32>",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(x28 - 0<64>)");
        }

        [Test]
        public void AArch64Rw_ret()
        {
            Given_Instruction(0xD65F03C0);
            AssertCode(     // ret\tx30
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void AArch64Rw_st4_post()
        {
            Given_HexString("60059F4C");
            AssertCode(//"st4 {v0.8h-v3.8h},[x11], #64");
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|__st4(x11, v0, v1, v2, v3)",
                "2|L--|x11 = x11 + 64<i64>");
        }

        [Test]
        public void AArch64Rw_stlrh()
        {
            Given_HexString("01FC9F48");
            AssertCode(     // stlrh	w1,[x0]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v3 = &Mem0[x0:word16]",
                "2|L--|__store_release_16(v3, w1)");
        }

        [Test]
        public void AArch64Rw_str_UnsignedImmediate()
        {
            Given_Instruction(0xF9000AE0);
            AssertCode(     // str\tx0,[x23,#&10]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x23 + 16<i64>:word64] = x0");
        }

        [Test]
        public void AArch64Rw_stxr_w32()
        {
            Given_HexString("067C0788");
            AssertCode(     // stxr	w7,w6,[x0]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = &Mem0[x0:word32]",
                "2|L--|w7 = __store_exclusive_word32(v4, w6)");
        }

        [Test]
        public void AArch64Rw_stxr_w64()
        {
            Given_HexString("067C07C8");
            AssertCode(     // stxr	w7,x6,[x0]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = &Mem0[x0:word64]",
                "2|L--|w7 = __store_exclusive_word64(v4, x6)");
        }

        [Test]
        public void AArch64Rw_stxrb()
        {
            Given_HexString("FF0B0008");
            AssertCode(     // stxrb	w0,w31,[sp]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = &Mem0[sp:byte]",
                "2|L--|w0 = __store_exclusive_byte(v4, w31)");
        }

        [Test]
        public void AArch64Rw_str_reg()
        {
            Given_Instruction(0xB8356B7F);
            AssertCode(     // str\tw31,[x27,x21]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x27 + x21:word32] = 0<32>");
        }

        [Test]
        public void AArch64Rw_nop()
        {
            Given_Instruction(0xD503201F);
            AssertCode(     // nop
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }


        [Test]
        public void AArch64Rw_str_w32()
        {
            Given_Instruction(0xB9006FA0);
            AssertCode(     // str\tw0,[x29,#&6C]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x29 + 108<i64>:word32] = w0");
        }

        [Test]
        public void AArch64Rw_ldr()
        {
            Given_Instruction(0xF9400000);
            AssertCode(     // ldr\tx0,[x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = Mem0[x0:word64]");
        }

        [Test]
        public void AArch64Rw_subs()
        {
            Given_Instruction(0xEB13001F);
            AssertCode(     // subs\tx31,x0,x19
                "0|L--|00100000(4): 2 instructions",
                "1|L--|x31 = x0 - x19",
                "2|L--|NZCV = cond(x31)");
        }

        [Test]
        public void AArch64Rw_cset()
        {
            Given_Instruction(0x1A9F17E0);
            AssertCode(     // csinc\tw0,w31,w31,NE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = CONVERT(Test(EQ,Z), bool, word32)");
        }

        [Test]
        public void AArch64Rw_cinc()
        {
            Given_Instruction(0x1A88A518);
            AssertCode(     // csinc\tw0,w31,w31,NE
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,NV)) branch 00100004",
                "2|L--|w24 = w8 + 1<32>");
        }


        [Test]
        public void AArch64Rw_ldrb()
        {
            Given_Instruction(0x39402260);
            AssertCode(     // ldrb\tw0,[x19,#&8]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[x19 + 8<i64>:byte]",
                "2|L--|w0 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AArch64Rw_cbnz()
        {
            Given_Instruction(0x35000140);
            AssertCode(     // cbnz\tw0,#&100028
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (w0 != 0<32>) branch 00100028");
        }

        [Test]
        public void AArch64Rw_strb()
        {
            Given_Instruction(0x39002260);
            AssertCode(     // strb\tw0,[x19,#&8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x19 + 8<i64>:byte] = SLICE(w0, byte, 0)");
        }

        [Test]
        public void AArch64Rw_ldr_w32()
        {
            Given_Instruction(0xB9400001);
            AssertCode(     // ldr\tw1,[x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = Mem0[x0:word32]");
        }

        [Test]
        public void AArch64Rw_cbnz_negative_offset()
        {
            Given_Instruction(0x35FFFE73);
            AssertCode(     // cbnz\tw19,#&FFFCC
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (w19 != 0<32>) branch 000FFFCC");
        }

        [Test]
        public void AArch64Rw_bne1()
        {
            Given_Instruction(0x54000401);
            AssertCode(     // b.ne\t#&100080
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100080");
        }

        [Test]
        public void AArch64Rw_ldr_reg_shift()
        {
            Given_Instruction(0xF8737AA3);
            AssertCode(     // ldr\tx3,[x21,x19,lsl,#3]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = Mem0[x21 + x19 * 8<i64>:word64]");
        }

        [Test]
        public void AArch64Rw_blr()
        {
            Given_Instruction(0xD63F0060);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call x3 (0)");
        }

        [Test]
        public void AArch64Rw_bne_backward()
        {
            Given_Instruction(0x54FFFF21);
            AssertCode(     // b.ne\t#&FFFE4
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 000FFFE4");
        }

        [Test]
        public void AArch64Rw_stp_preindex()
        {
            Given_Instruction(0xA9B87BFD);
            AssertCode(     // stp\tx29,x30,[x31,-#&80]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|sp = sp + -128<i64>",
                "2|L--|Mem0[sp:word64] = x29",
                "3|L--|sp = sp + 8<i64>",
                "4|L--|Mem0[sp:word64] = x30");
        }

        [Test]
        public void AArch64Rw_ldp_w64()
        {
            Given_Instruction(0xA9446BB9);
            AssertCode(     // ldp\tx25,x26,[x29,#&40]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x29 + 64<i64>",
                "2|L--|x25 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8<i64>",
                "4|L--|x26 = Mem0[v5:word64]");
        }



        [Test]
        public void AArch64Rw_ldp_post()
        {
            Given_Instruction(0xA8C17BFD);
            AssertCode(     // ldp\tx29,x30,[x31],#&8
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v5 = sp",
                "2|L--|x29 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8<i64>",
                "4|L--|x30 = Mem0[v5:word64]",
                "5|L--|sp = sp + 16<i64>");
        }

        [Test]
        public void AArch64Rw_ldr_64_neg_lit()
        {
            Given_Instruction(0x18FFFFE0);
            AssertCode(     // ldr\tw0,#&FFFFC
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = Mem0[0x000FFFFC<p32>:word32]");
        }

        [Test]
        public void AArch64Rw_movn()
        {
            Given_Instruction(0x12800000);
            AssertCode(     // movn\tw0,#0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = 0xFFFFFFFF<32>");
        }

        [Test]
        public void AArch64Rw_asr()
        {
            Given_Instruction(0x13017E73);
            AssertCode(     // sbfm\tw19,w19,#1,#&1F
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w19 = w19 >> 1<i32>");
        }

        [Test]
        public void AArch64Rw_and_reg()
        {
            Given_Instruction(0x8A140000);
            AssertCode(     // and\tx0,x0,x20
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x0 = x0 & x20");
        }

        [Test]
        public void AArch64Rw_and_reg_reg()
        {
            Given_Instruction(0x0A000020);
            AssertCode(     // and\tw0,w1,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = w1 & w0");
        }

        [Test]
        public void AArch64Rw_sfbiz()
        {
            Given_Instruction(0x937D7C63);
            AssertCode(     // sbfm\tx3,x3,#&3,#&20
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x3 = __sbfiz(x3, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_mul_64()
        {
            Given_Instruction(0x9B017C14);
            AssertCode(     // mul\tx20,x0,x1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x20 = x0 * x1");
        }

        [Test]
        public void AArch64Rw_madd_64()
        {
            Given_Instruction(0x9B013C14);
            AssertCode(     // madd\tx20,x0,x1,x15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x20 = x15 + x0 * x1");
        }

        [Test]
        public void AArch64Rw_ands_64_reg()
        {
            Given_Instruction(0xEA010013);
            AssertCode(     // ands\tx19,x0,x1
                 "0|L--|00100000(4): 4 instructions",
                 "1|L--|x19 = x0 & x1",
                 "2|L--|NZ = cond(x19)",
                 "3|L--|C = false",
                 "4|L--|V = false");
        }

        [Test]
        public void AArch64Rw_test_64_reg()
        {
            Given_Instruction(0xEA01001F);
            AssertCode(     // test\tx31,x0,x1
                 "0|L--|00100000(4): 3 instructions",
                 "1|L--|NZ = cond(x0 & x1)",
                 "2|L--|C = false",
                 "3|L--|V = false");
        }

        [Test]
        public void AArch64Rw_ldurb()
        {
            Given_Instruction(0x385F9019);
            AssertCode(     // ldurb\tw25,[x0,-#&7]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v4 = Mem0[x0 + -7<i64>:byte]",
                 "2|L--|w25 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AArch64Rw_strb_postidx()
        {
            Given_Instruction(0x38018C14);
            AssertCode(     // strb\tw20,[x0,#&18]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|x0 = x0 + 24<i64>",
                 "2|L--|Mem0[x0:byte] = SLICE(w20, byte, 0)");
        }

        [Test]
        public void AArch64Rw_strb_preidx_sp()
        {
            Given_Instruction(0x38018FFF);
            AssertCode(     // strb\tw31,[sp,#&18]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|sp = sp + 24<i64>",
                 "2|L--|Mem0[sp:byte] = 0<8>");
        }

        [Test]
        public void AArch64Rw_ldrh_32_off0()
        {
            Given_Instruction(0x79400021);
            AssertCode(     // ldrh\tw1,[x1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v4 = Mem0[x1:word16]",
                 "2|L--|w1 = CONVERT(v4, word16, word32)");
        }

        [Test]
        public void AArch64Rw_add_extension()
        {
            Given_Instruction(0x8B34C2D9);
            AssertCode(     // add\tx25,x22,w20,sxtw #0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x25 = x22 + CONVERT(w20, int32, int64)");
        }

        [Test]
        public void AArch64Rw_madd_32()
        {
            Given_Instruction(0x1B003C21);
            AssertCode(     // madd\tw1,w1,w0,w15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = w15 + w1 * w0");
        }

        [Test]
        public void AArch64Rw_mneg_32()
        {
            Given_Instruction(0x1B00FC21);
            AssertCode(     // mneg\tw1,w1,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = -(w1 * w0)");
        }

        [Test]
        public void AArch64Rw_msub_32()
        {
            Given_Instruction(0x1B00BC21);
            AssertCode(     // msub\tw1,w1,w0,w15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = w15 - w1 * w0");
        }

        [Test]
        public void AArch64Rw_strb_post_idx()
        {
            Given_Instruction(0x38001410);
            AssertCode(     // strb\tw16,[x0],#&1
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[x0:byte] = SLICE(w16, byte, 0)",
                 "2|L--|x0 = x0 + 1<i64>");
        }

        [Test]
        public void AArch64Rw_strb_post_idx_zero()
        {
            Given_Instruction(0x3800141F);
            AssertCode(     // strb\tw31,[x0],#&1
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[x0:byte] = 0<8>",
                 "2|L--|x0 = x0 + 1<i64>");
        }

        [Test]
        public void AArch64Rw_strb_post_sp()
        {
            Given_Instruction(0x381FC7FF);
            AssertCode(     // strb\tw31,[sp],-#&4
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[sp:byte] = 0<8>",
                 "2|L--|sp = sp + -4<i64>");
        }

        [Test]
        public void AArch64Rw_msub_64()
        {
            Given_Instruction(0x9B038441);
            AssertCode(     // msub\tx1,x2,x3,x1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x1 = x1 - x2 * x3");
        }

        [Test]
        public void AArch64Rw_ldur_64_negative_offset()
        {
            Given_Instruction(0xF85F8260);
            AssertCode(     // ldur\tx0,[x19,-#&8]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x0 = Mem0[x19 + -8<i64>:word64]");
        }

        [Test]
        public void AArch64Rw_strb_indexed()
        {
            Given_Instruction(0x3820483F);
            AssertCode(     // strb\tw31,[x1,w0,uxtw]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x1 + CONVERT(w0, uint32, uint64):byte] = 0<8>");
        }

        [Test]
        public void AArch64Rw_str_q0()
        {
            Given_Instruction(0x3D8027A0);
            AssertCode(     // str\tq0,[x29,#&90]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x29 + 144<i64>:word128] = q0");
        }

        [Test]
        public void AArch64Rw_cmp_32_uxtb()
        {
            Given_Instruction(0x6B20031F);
            AssertCode(     // cmp\tw0,w0,uxtb #0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|NZCV = cond(w24 - CONVERT(SLICE(w0, byte, 0), byte, uint32))");
        }

        [Test]
        public void AArch64Rw_ldrb_idx()
        {
            Given_Instruction(0x38616857);
            AssertCode(     // ldrb\tw23,[x2,x1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x2 + x1:byte]",
                 "2|L--|w23 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void AArch64Rw_strb_index()
        {
            Given_Instruction(0x38216A63);
            AssertCode(     // strb\tw3,[x19,x1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x19 + x1:byte] = SLICE(w3, byte, 0)");
        }

        [Test]
        public void AArch64Rw_strb_zero()
        {
            Given_Instruction(0x38216A7F);
            AssertCode(     // strb\tw3,[x19,x1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x19 + x1:byte] = 0<8>");
        }

        [Test]
        public void AArch64Rw_add_ext_reg_sxtw()
        {
            Given_Instruction(0x8B33D2D3);
            AssertCode(     // add\tx19,x22,w19,sxtw #4
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x19 = x22 + CONVERT(w19, int32, int64)");
        }

        [Test]
        public void AArch64Rw_ldr_64_preidx()
        {
            Given_Instruction(0xF8410E81);
            AssertCode(     // ldr\tx1,[x20,#&10]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|x20 = x20 + 16<i64>",
                 "2|L--|x1 = Mem0[x20:word64]");
        }

        [Test]
        public void AArch64Rw_ldrb_post()
        {
            Given_Instruction(0x38401420);
            AssertCode(     // ldrb\tw0,[x1],#&1
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[x1:byte]",
                "2|L--|w0 = CONVERT(v4, byte, word32)",
                "3|L--|x1 = x1 + 1<i64>");
        }

        [Test]
        public void AArch64Rw_strb_uxtw()
        {
            Given_Instruction(0x38344B23);
            AssertCode(     // strb\tw3,[x25,w20,uxtw]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x25 + CONVERT(w20, uint32, uint64):byte] = SLICE(w3, byte, 0)");
        }

        [Test]
        public void AArch64Rw_sdiv()
        {
            Given_Instruction(0x1AC00F03);
            AssertCode(     // sdiv\tw3,w24,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w3 = w24 / w0");
        }


        [Test]
        public void AArch64Rw_ldrb_preidx()
        {
            Given_Instruction(0x38401C41);
            AssertCode(     // ldrb\tw1,[x2,#&1]!
                 "0|L--|00100000(4): 3 instructions",
                 "1|L--|x2 = x2 + 1<i64>",
                 "2|L--|v4 = Mem0[x2:byte]",
                 "3|L--|w1 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void AArch64Rw_ldrb_idx_uxtw()
        {
            Given_Instruction(0x38614873);
            AssertCode(     // ldrb\tw19,[x3,w1,uxtw]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[x3 + CONVERT(w1, uint32, uint64):byte]",
                "2|L--|w19 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void AArch64Rw_ldrh_32_idx_lsl()
        {
            Given_Instruction(0x787B7B20);
            AssertCode(     // ldrh\tw0,[x25,x27,lsl #1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x25 + x27 * 2<i64>:word16]",
                 "2|L--|w0 = CONVERT(v5, word16, word32)");
        }

        [Test]
        public void AArch64Rw_ldrh_32_sxtw()
        {
            Given_Instruction(0x7876D800);
            AssertCode(     // ldrh\tw0,[x0,w22,sxtw #1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x0 + CONVERT(w22, int32, int64):word16]",
                 "2|L--|w0 = CONVERT(v5, word16, word32)");
        }

        [Test]
        public void AArch64Rw_ldrb_sxtw()
        {
            Given_HexString("00C87338");
            AssertCode(     // ldrb w0,[x0,w19,sxtw]
                 "0|L--|0000000000100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x0 + CONVERT(w19, int32, int64):byte]",
                 "2|L--|w0 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void AArch64Rw_movn_imm()
        {
            Given_Instruction(0x9280000A);
            AssertCode(     // movn\tx10,#0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x10 = 0xFFFFFFFFFFFFFFFF<64>");
        }

        [Test]
        public void AArch64Rw_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            AssertCode(     // sturb\tw31,[x4,-#&1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x4 + -1<i64>:byte] = 0<8>");
        }

        [Test]
        public void AArch64Rw_adr()
        {
            Given_Instruction(0x10000063);
            AssertCode(     // adr\tx3,#&10000C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = 0010000C");
        }

        [Test]
        public void AArch64Rw_orr()
        {
            Given_HexString("2B91D6AA");
            AssertCode(     // orr	x11,x9,x22,ror #&24
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x11 = x9 | __ror(x22, 36<i32>)");
        }

        [Test]
        public void AArch64Rw_orr_zero()
        {
            Given_HexString("E0077EB2");
            AssertCode(     // orr	x0,xzr,#&C
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x0 = 0<64> | 0xC<64>");
        }

        [Test]
        public void AArch64Rw_orn()
        {
            Given_Instruction(0x2A2200F8);
            AssertCode(     // orn\tw24,w7,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w24 = w7 | ~w2");
        }

        [Test]
        public void AArch64Rw_mvn()
        {
            Given_Instruction(0x2A2203F8);
            AssertCode(     // mvn\tw24,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w24 = ~w2");
        }

        [Test]
        public void AArch64Rw_sdiv_64()
        {
            Given_Instruction(0x9AC20C62);
            AssertCode(     // sdiv\tx2,x3,x2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = x3 / x2");
        }

        [Test]
        public void AArch64Rw_eon()
        {
            Given_HexString("B715EACA");
            AssertCode(     // eon	x23,x13,x10,ror #5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x23 = x13 ^ ~__ror(x10, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_eor_reg_32()
        {
            Given_Instruction(0x4A140074);
            AssertCode(     // eor\tw20,w3,w20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w20 = w3 ^ w20");
        }

        [Test]
        public void AArch64Rw_eor()
        {
            Given_HexString("A640C54A");
            AssertCode(     // eor	w6,w5,w5,ror #&10
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w6 = w5 ^ __ror(w5, 16<i32>)");
        }

        [Test]
        public void AArch64Rw_eret()
        {
            Given_HexString("E0039FD6");
            AssertCode(     // eret
                "0|R--|0000000000100000(4): 2 instructions",
                "1|L--|__eret()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void AArch64Rw_eon_reg32()
        {
            Given_HexString("0000214A");
            AssertCode( // eon
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w0 = w0 ^ ~w1");
        }

        [Test]
        public void AArch64Rw_eon_reg32_shifted()
        {
            Given_HexString("000C214A");
            AssertCode( // eon
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w0 = w0 ^ ~(w1 << 3<i32>)");
        }

        [Test]
        public void AArch64Rw_ext()
        {
            Given_HexString("0240016E");
            AssertCode(     // ext	v2.16b,v0.16b,v1.16b,#8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q2 = __ext_i8(v2, q1)");
        }

        [Test]
        public void AArch64Rw_ext_vec()
        {
            Given_HexString("20581F6E");
            AssertCode(     // Nyi
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|q0 = __ext_i8(v2, q31)");
        }

        [Test]
        public void AArch64Rw_extr()
        {
            Given_HexString("410CC193");
            AssertCode(     // extr	x1,x2,x1,#3
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x1 = __ror(x2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_sub_reg_ext_64()
        {
            Given_Instruction(0xCB214F18);
            AssertCode(     // sub\tx24,x24,w1,uxtw #3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x24 = x24 - CONVERT(w1, word32, uint64)");
        }

        [Test]
        public void AArch64Rw_sub()
        {
            Given_HexString("C2C5CCCB");
            AssertCode(     // sub	x2,x14,x12,ror #&31
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x2 = x14 - __ror(x12, 49<i32>)");
        }

        [Test]
        public void AArch64Rw_subs_Wn_imm()
        {
            Given_Instruction("011 10001 00 011111111111 10001 10011");
            AssertCode( // subs\tw19, w17,#&7FF
                "0|L--|00100000(4): 2 instructions",
                "1|L--|w19 = w17 - 0x7FF<32>",
                "2|L--|NZCV = cond(w19)");
        }

        [Test]
        public void AArch64Rw_sub_Xn_imm()
        {
            Given_Instruction("110 10001 00 011111111111 10001 10011");
            AssertCode( // sub\tx19,x17,#&7FF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x19 = x17 - 0x7FF<64>");
        }

        [Test]
        public void AArch64Rw_umulh()
        {
            Given_Instruction(0x9BC57C00);
            AssertCode(     // umulh\tx0,w0,w5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = SLICE(w0 *u w5, uint64, 64)");
        }

        [Test]
        public void AArch64Rw_lsrv()
        {
            Given_Instruction(0x1AC22462);
            AssertCode(     // lsrv\tw2,w3,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w2 = w3 >>u w2");
        }



        [Test]
        public void AArch64Rw_smaddl()
        {
            Given_Instruction(0x9B233C43);
            AssertCode(     // smaddl\tx3,w2,w3,x15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = x15 + CONVERT(w2 *s w3, int32, int64)");
        }

        [Test]
        public void AArch64Rw_strh_reg()
        {
            Given_Instruction(0x78206A62);
            AssertCode(     // strh\tw2,[x19,x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x19 + x0:word16] = SLICE(w2, word16, 0)");
        }

        [Test]
        public void AArch64Rw_stp_r64_pre()
        {
            Given_Instruction(0x6DB73BEF);
            AssertCode(     // stp\td15,d14,[sp,-#&90]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|sp = sp + -144<i64>",
                "2|L--|Mem0[sp:word64] = d15",
                "3|L--|sp = sp + 8<i64>",
                "4|L--|Mem0[sp:word64] = d14");
        }

        [Test]
        public void AArch64Rw_ldr_r64_off()
        {
            Given_Instruction(0xFD45E540);
            AssertCode(     // ldr\td0,[x10,#&BC8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = Mem0[x10 + 3016<i64>:word64]");
        }

        [Test]
        public void AArch64Rw_str_r64_imm()
        {
            Given_Instruction(0xFD001BE0);
            AssertCode(     // str\td0,[sp,#&30]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 48<i64>:word64] = d0");
        }

        [Test]
        public void AArch64Rw_scvtf_int()
        {
            Given_Instruction(0x1E220120);
            AssertCode(     // scvtf\ts0,w9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = CONVERT(w9, int32, real32)");
        }

        [Test]
        public void AArch64Rw_mov_vector_to_vector()
        {
            Given_Instruction(0x4EA31C68);
            AssertCode(     // @@@
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q3");
        }

        [Test]
        public void AArch64Rw_ldp_w32()
        {
            Given_Instruction(0x296107A2);
            AssertCode(     // ldp\tw2,w1,[x29,-#&F8]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x29 + -248<i64>",
                "2|L--|w2 = Mem0[v5:word32]",
                "3|L--|v5 = v5 + 4<i64>",
                "4|L--|w1 = Mem0[v5:word32]");
        }

        [Test]
        public void AArch64Rw_scvtf_r32()
        {
            Given_Instruction(0x5E21D82F);
            AssertCode(     // scvtf\ts15,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s15 = CONVERT(s1, int32, real32)");
        }

        [Test]
        public void AArch64Rw_stp_r32()
        {
            Given_Instruction(0x2D010FE2);
            AssertCode(     // stp\ts2,s3,[sp,#&8]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = sp + 8<i64>",
                "2|L--|Mem0[v5:word32] = s2",
                "3|L--|v5 = v5 + 4<i64>",
                "4|L--|Mem0[v5:word32] = s3");
        }

        [Test]
        public void AArch64Rw_fcvtms_f32_to_i32()
        {
            Given_Instruction(0x1E300003);
            AssertCode(     // fcvtms\tw3,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w3 = CONVERT(floorf(s0), real32, int32)");
        }

        [Test]
        public void AArch64Rw_udiv_w32()
        {
            Given_Instruction(0x1ADA0908);
            AssertCode(     // udiv\tw8,w8,w26
            "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = w8 /u w26");
        }

        [Test]
        public void AArch64Rw_rev16_w32()
        {
            Given_Instruction(0x5AC0056B);
            AssertCode(     // rev16\tw11,w11
            "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = __rev16(w11)");
        }

        [Test]
        public void AArch64Rw_add_32_ext()
        {
            Given_Instruction(0x0B20A1EF);
            AssertCode(     // add\tw15,w15,w0,sxth #0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w15 = w15 + CONVERT(SLICE(w0, int16, 0), int16, int32)");
        }

        [Test]
        public void AArch64Rw_scvtf_i32_to_f32()
        {
            Given_Instruction(0x1E6202E0);
            AssertCode(     // scvtf\td0,w23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = CONVERT(w23, int32, real64)");
        }

        [Test]
        public void AArch64Rw_fmov_f64_to_i64()
        {
            Given_Instruction(0x9E6701B0);
            AssertCode(     // fmov\td16,x13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = x13");
        }

        [Test]
        public void AArch64Rw_sxtl()
        {
            Given_Instruction(0x0F10A673);
            AssertCode(     // sxtl\tv19.4s,v19.4h
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = d19",
                "2|L--|q19 = __sxtl_i16(v2)");
        }

        [Test]
        public void AArch64Rw_fcvtzs_vector_real32()
        {
            Given_Instruction(0x4EA1BAB5);
            AssertCode(     // fcvtzs\tv21.4s,v21.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q21",
                "2|L--|q21 = __trunc_f32(v2)");
        }

        [Test]
        public void AArch64Rw_fmul_real32()
        {
            Given_Instruction(0x1E210B25);
            AssertCode(     // fmul\ts5,s25,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s5 = s25 * s1");
        }

        [Test]
        public void AArch64Rw_fcvtzs_i32_from_f32()
        {
            Given_Instruction(0x1E380069);
            AssertCode(     // fcvtzs\tw3,s9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w3 = CONVERT(truncf(s9), real32, int32)");
        }

        [Test]
        public void AArch64Rw_fcvtzs_i32_from_f64()
        {
            Given_HexString("0C00781E");
            AssertCode(     // fcvtzs	w12,d0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|w12 = CONVERT(trunc(d0), real64, int32)");
        }

        [Test]
        public void AArch64Rw_fcsel()
        {
            Given_Instruction(0x1E2B1C00);
            AssertCode(     // fcsel\ts0,s0,s11,NE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = Test(NE,Z) ? s0 : s11");
        }

        [Test]
        public void AArch64Rw_fcvtps_f32_to_i32()
        {
            Given_Instruction(0x1E280008);
            AssertCode(     // fcvtps\tw8,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = CONVERT(ceilf(s0), real32, int32)");
        }

        [Test]
        public void AArch64Rw_fcmp_f32()
        {
            Given_Instruction(0x1E222060);
            AssertCode(     // fcmp\ts3,s2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s3 - s2)");
        }

        [Test]
        public void AArch64Rw_fabs_f32()
        {
            Given_Instruction(0x1E20C021);
            AssertCode(     // fabs\ts1,s1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = s1",
                "2|L--|s1 = fabsf(v3)");
        }

        [Test]
        public void AArch64Rw_fneg_f32()
        {
            Given_Instruction(0x1E214021);
            AssertCode(     // fneg\ts1,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = -s1");
        }

        [Test]
        public void AArch64Rw_fsqrt()
        {
            Given_Instruction(0x1E21C001);
            AssertCode(     // fsqrt\ts1,s0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = s0",
                "2|L--|s1 = sqrtf(v2)");
        }

        [Test]
        public void AArch64Rw_fmov_i32_to_f32()
        {
            Given_Instruction(0x1E2703E1);
            AssertCode(     // fmov\ts1,w31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = 0<32>");
        }

        [Test]
        public void AArch64Rw_fcvt_f32_to_f64()
        {
            Given_Instruction(0x1E22C041);
            AssertCode(     // fcvt\td1,s2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = CONVERT(s2, real32, real64)");
        }

        [Test]
        public void AArch64Rw_fcvt_f64_to_f32()
        {
            Given_Instruction(0x1E624000);
            AssertCode(     // fcvt\ts0,d0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = CONVERT(d0, real64, real32)");
        }

        [Test]
        public void AArch64Rw_mov_w128()
        {
            Given_Instruction(0x4EA91D22);
            AssertCode(     // mov\tv2.16b,v9.16b
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q2 = q9");
        }





        


        [Test]
        public void AArch64Rw_fmov_f32_to_w32()
        {
            Given_Instruction(0x1E26002B);
            AssertCode(     // fmov\tw11,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = s1");
        }

        [Test]
        public void AArch64Rw_fmov_vector_immediate()
        {
            Given_Instruction(0x4F03F600);
            AssertCode(     // fmov\tv0.4s,#1.0F
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 1.0F",
                "2|L--|q0 = __fmov_f32(v2)");
        }

        [Test]
        public void AArch64Rw_dsb()
        {
            Given_Instruction(0xD5033F9F);	// dsb	#&F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dsb_sy()");
        }

        [Test]
        public void AArch64Rw_dmb_sy()
        {
            Given_HexString("BF3F03D5");
            AssertCode(     // dmb	sy
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__dmb_sy()");
        }

        [Test]
        public void AArch64Rw_dup_element_w32()
        {
            Given_Instruction(0x4E0406E2);
            AssertCode(     // dup\tv2.4s,v23.s[0]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q23[0<i32>]",
                "2|L--|q2 = __dup_i32(v2)");
        }

        [Test]
        public void AArch64Rw_ext_vector()
        {
            Given_HexString("AB091E6E");
            AssertCode(     // Nyi
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q13",
                "2|L--|q11 = __ext_i8(v2, q30)");
        }

        [Test]
        public void AArch64Rw_fadd_vector_f32()
        {
            Given_Instruction(0x4E30D4D0);
            AssertCode(     // fadd\tv16.4s,v6.4s,v16.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q6",
                "2|L--|v3 = q16",
                "3|L--|q16 = __fadd_f32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_scvtf_vector_i32()
        {
            Given_HexString("10DA214E");
            AssertCode(     // scvtf\tv16.4s,v16.4s
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v3 = q16",
                "2|L--|q16 = __scvtf_i32(v3)");
        }

        [Test]
        public void AArch64Rw_mul_vector_i16()
        {
            Given_Instruction(0x4E609C20);
            AssertCode(     // mul\tv0.8h,v1.8h,v0.8h
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q0",
                "3|L--|q0 = __mul_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_mul_vector_s()
        {
            Given_HexString("009CAC4E");
            AssertCode(
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q12",
                "3|L--|q0 = __mul_i32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_addv_i32()
        {
            Given_Instruction(0x4EB1B821);
            AssertCode(     // addv\ts1,v1.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|s1 = __sum_i32(v2)");
        }

        [Test]
        public void AArch64Rw_mov_vector_element_i16()
        {
            Given_Instruction(0x6E0A5633);
            AssertCode(     // mov\tv19.h[2],v17.h[5]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q19[2<i32>] = q17[5<i32>]");
        }

        [Test]
        public void AArch64Rw_add_vector_s64()
        {
            Given_HexString("6384EB4E");
            AssertCode(
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q3",
                "2|L--|v3 = q11",
                "3|L--|q3 = __add_i64(v2, v3)");
        }

        [Test]
        public void AArch64Rw_add_vector_i32()
        {
            Given_Instruction(0x4EA28482);
            AssertCode(     // add\tv2.4s,v4.4s,v2.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = q2",
                "3|L--|q2 = __add_i32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_fmul_vector_f32()
        {
            Given_Instruction(0x6E30DC90);
            AssertCode(     // fmul\tv16.4s,v4.4s,v16.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = q16",
                "3|L--|q16 = __fmul_f32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ccmp_w32()
        {
            Given_Instruction(0x7A43B900);
            AssertCode(     // ccmp\tw8,#3,#0,LT
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(GE,NZV)",
                "2|L--|NZCV = 0<32>",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w8 - 3<32>)");
        }

        [Test]
        public void AArch64Rw_ucvtf_f32()
        {
            Given_Instruction(0x7E21D821);
            AssertCode(     // ucvtf\ts1,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = CONVERT(s1, uint32, real32)");
        }

        [Test]
        public void AArch64Rw_clz()
        {
            Given_Instruction(0xDAC01002);
            AssertCode(     // clz\tx2,x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = __clz(x0)");
        }

        [Test]
        public void AArch64Rw_stp_zero_offset()
        {
            Given_Instruction(0xA9007EBF);
            AssertCode(     // stp\tx31,x31,[x21]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = x21",
                "2|L--|Mem0[v3:word64] = 0<64>",
                "3|L--|v3 = v3 + 8<i64>",
                "4|L--|Mem0[v3:word64] = 0<64>");
        }

        [Test]
        public void AArch64Rw_ld2()
        {
            Given_Instruction(0x4C4081C1);	// ld2	{v1.8b,v2.8b},[x14]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld2(x14, out v1, out v2)");
        }

        [Test]
        public void AArch64Rw_ld3()
        {
            Given_Instruction(0x0C404565);
            AssertCode(     // ld3\t{v5.4h,v6.4h,v7.4h},[x11]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld3(x11, out v5, out v6, out v7)");
        }

        [Test]
        public void AArch64Rw_movi_b()
        {
            Given_Instruction(0x0F00E460);
            AssertCode(     // movi\tv0.8b,#&3030303
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0x303030303030303<64>",
                "2|L--|d0 = __movi_i8(v2)");
        }

        [Test]
        public void AArch64Rw_movi_w16()
        {
            Given_Instruction(0x4F008441);
            AssertCode(     // movi\tv1.8h,#&20002
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0x2000200020002<64>",
                "2|L--|q1 = __movi_i16(v2)");
        }

        [Test]
        public void AArch64Rw_movi_w64_0()
        {
            Given_Instruction(0x6F00E401);
            AssertCode(     // movi\tv1.2d,#0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0<64>",
                "2|L--|q1 = __movi_i64(v2)");
        }

        [Test]
        public void AArch64Dis_scvtf_i64_f64()
        {
            Given_HexString("10D8615E");
            AssertCode(     // scvtf\td16, d0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|d16 = CONVERT(d0, int64, real64)");
        }



 

   

        [Test]
        public void AArch64Rw_fmov_vector_hiword()
        {
            Given_Instruction(0x9EAF0060);
            AssertCode(     // fmov\tq0.d[1],x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q0[1<i32>] = x3");
        }

        [Test]
        public void AArch64Rw_smaxv()
        {
            Given_Instruction(0x4EB0A800);
            AssertCode(     // smaxv\ts0,v0.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|s0 = __smaxv_i32(v2)");
        }

        [Test]
        public void AArch64Rw_smax_vector()
        {
            Given_Instruction(0x4EA16400);
            AssertCode(     // smax\tv0.4s,v0.4s,v1.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q1",
                "3|L--|q0 = __smax_i32(v2, v3)");
        }

        [Test]
        public void AArch64Rw_umull_vector()
        {
            Given_Instruction(0x2E20C084);
            AssertCode(     // umull\tv4.8h,v4.8b,v0.8b
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = d4",
                "2|L--|v3 = d0",
                "3|L--|q4 = __mull_i8(v2, v3)");
        }





        [Test]
        public void AArch64Rw_add_with_extension()
        {
            Given_Instruction(0x8B3F63E0);	// add	x0,sp,x31,uxtx #0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = sp + 0<64>");
        }


        [Test]
        public void AArch64Rw_bfm()
        {
            Given_Instruction(0x33101D28);	// bfm	w8,w9,#&10,#7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = __bfm(w9, 16<i32>, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_csinv()
        {
            Given_Instruction(0x5A9F03E8);	// csinv	w8,w31,w31,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = Test(EQ,Z) ? 0<32> : ~0<32>");
        }

        [Test]
        public void AArch64Rw_csel()
        {
            Given_Instruction(0x9A8903E2);	// csel	x2,x31,x9,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = Test(EQ,Z) ? 0<64> : x9");
        }

        [Test]
        public void AArch64Rw_fdiv()
        {
            Given_Instruction(0x1E201820);	// fdiv	s0,s1,s0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = s1 / s0");
        }


        [Test]
        public void AArch64Rw_fsub()
        {
            Given_Instruction(0x1E203920);	// fsub	s0,s9,s0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = s9 - s0");
        }

        [Test]
        public void AArch64Rw_sxtb()
        {
            Given_Instruction(0x13001F6A);	// sxtb	w10,w27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w10 = CONVERT(SLICE(w27, int8, 0), int8, int32)");
        }

        [Test]
        public void AArch64Rw_fmax()
        {
            Given_Instruction(0x1E2A4800);	// fmax	s0,s0,s10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = fmaxf(s0, s10)");
        }



        [Test]
        public void AArch64Rw_csneg()
        {
            Given_Instruction(0x5A895528);	// csneg	w8,w9,w9,PL
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = Test(GE,N) ? w9 : -w9");
        }


        [Test]
        public void AArch64Rw_fmin()
        {
            Given_Instruction(0x1E285800);	// fmin	s0,s0,s8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = fminf(s0, s8)");
        }

        [Test]
        public void AArch64Rw_rorv()
        {
            Given_Instruction(0x1AC92D09);	// rorv	w9,w8,w9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w9 = __ror(w8, w9)");
        }

        [Test]
        public void AArch64Rw_cmp()
        {
            Given_Instruction(0x6B98069F);	// cmp	w20,w24,asr #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(w20 - (w24 >> 1<i32>))");
        }

        [Test]
        public void AArch64Rw_ldpsw()
        {
            Given_Instruction(0x69404222);	// ldpsw	x2,x16,[x17]
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x17",
                "2|L--|x2 = CONVERT(Mem0[v5:int32], int32, int64)",
                "3|L--|v5 = v5 + 4<i64>",
                "4|L--|x16 = CONVERT(Mem0[v5:int32], int32, int64)");
        }

        [Test]
        public void AArch64Rw_fnmul()
        {
            Given_Instruction(0x1E227800);	// fnmul	s0,s0,s2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = -(s0 * s2)");
        }


        [Test]
        public void AArch64Rw_st1()
        {
            Given_Instruction(0x0D0041B5);	// st1	{v21.h}[0],[x13]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x13:word16] = v21[0<i32>]");
        }

        [Test]
        public void AArch64Rw_not_vector()
        {
            Given_Instruction(0x6E205821);	// not	v1.16b,v1.16b
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|q1 = __not_i8(v2)");
        }

        [Test]
        public void AArch64Rw_ubfm()
        {
            Given_Instruction(0x5302096B);	// ubfm	w11,w11,#2,#2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = __ubfm(w11, 2<i32>, 2<i32>)");
        }

        [Test]
        public void AArch64Rw_ushr()
        {
            Given_HexString("63043F6F");
            AssertCode(     // ushr	v3.4s,v3.4s,#1
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q3",
                "2|L--|q3 = __ushr_u32(v2, 1<i32>)");
        }

        [Test]
        public void AArch64Rw_uxtb()
        {
            Given_Instruction(0x53001E63);	// uxtb	w3,w19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w3 = CONVERT(SLICE(w19, uint8, 0), uint8, uint32)");
        }

        [Test]
        public void AArch64Rw_csinc()
        {
            Given_Instruction(0x9A800660);	// csinc	x0,x19,x0,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = Test(EQ,Z) ? x19 : x0 + 1<64>");
        }

        [Test]
        public void AArch64Rw_fmadd_f32()
        {
            Given_Instruction(0x1F0000B9);
            AssertCode(     // fmadd\ts25,s5,s0,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s25 = __fmaddf(s5, s0, s0)");
        }


        [Test]
        public void AArch64Rw_fmsub_f32()
        {
            Given_Instruction(0x1F00A08B);
            AssertCode(     // fmsub\ts11,s16,s0,s8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s11 = __fmsubf(s4, s0, s8)");
        }


        [Test]
        public void AArch64Rw_fmadd_f16()
        {
            Given_Instruction(0x1FD61F00);
            AssertCode(     // fmadd\th0,h24,h22,h7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|h0 = __fmaddf(h24, h22, h7)");
        }

        [Test]
        public void AArch64Rw_prfm()
        {
            Given_Instruction(0xD8545280);
            AssertCode(     // prfm\t#0,#&1A8A50"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__prfm(0<8>, 0x001A8A50<p32>)");
        }


        [Test]
        public void AArch64Rw_ld1r_i8()
        {
            Given_Instruction(0x4D40C220);
            AssertCode(     // ld1r\t{v0.16b},[x17]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld1r(x17, out v0)");
        }

        [Test]
        [Ignore("Wait for indexed ldX")]
        public void AArch64Rw_ld1x()
        {
            Given_HexString("0000400D");
            AssertCode(     // ld1	{v0.b}[0],[x0]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void AArch64Rw_shrn_i8()
        {
            Given_Instruction(0x0F0E8463);
            AssertCode(     // shrn\tv3.8b,v3.8h,#2
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q3",
                "2|L--|d3 = __shrn_i16(v2, 2<i32>)");
        }

        [Test]
        public void AArch64Rw_ccmp_reg()
        {
            Given_Instruction(0x7A42D020);
            AssertCode(     // ccmp\tw1,w2,#0,LE
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(GT,NZV)",
                "2|L--|NZCV = 0<32>",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w1 - w2)");
        }

        [Test]
        public void AArch64Rw_str_sxtx()
        {
            Given_Instruction(0x3CBBEBC8);
            AssertCode( // str	q8, [x30,x27,sxtx]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x30 + x27:word128] = q8");
        }

        [Test]
        public void AArch64Rw_svc()
        {
            Given_Instruction(0xD41B7B61);	// svc	#&DBDB
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__supervisor_call(0xDBDB<16>)");
        }

        [Test]
        public void AArch64Rw_mrs()
        {
            Given_Instruction(0xD53B0020);	// mrs	x0,CTR_EL0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = __mrs(CTR_EL0)");
        }

        [Test]
        public void AArch64Rw_isb()
        {
            Given_Instruction(0xD5033FDF);	// isb	#&F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__isb_sy()");
        }

        [Test]
        public void AArch64Rw_smc()
        {
            Given_Instruction(0xD4000003);	// smc	#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__secure_monitor_call(0<16>)");
        }

        [Test]
        public void AArch64Rw_msr()
        {
            Given_Instruction(0xD50343DF);	// msr	pstate,#3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__msr(pstate, 3<8>)");
        }

        [Test]
        public void AArch64Rw_ccmn()
        {
            Given_Instruction(0x3A4D09C0);	// ccmn	w14,#&D,#0,EQ
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(NE,Z)",
                "2|L--|NZCV = 0<32>",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w14 + 0xD<32>)");
        }

        [Test]
        public void AArch64Rw_stlr()
        {
            Given_HexString("00FD9FC8");
            AssertCode(     // stlr	x0,[x8]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v3 = &Mem0[x8:word64]",
                "2|L--|__store_release_64(v3, x0)");
        }

        [Test]
        public void AArch64Rw_GitHub_898()
        {
            Given_HexString("427CAE9B");
            AssertCode(
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x2 = CONVERT(w2 *u w14, uint32, uint64)");
        }

        [Test]
        public void AArch64Rw_fcmpe_reg()
        {
            Given_HexString("3020201E");
            AssertCode(     // fcmpe	s1,s0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|NZCV = cond(s1 - s0)");
        }

        [Test]
        public void AArch64Rw_fcmpe_reg_exchanged()
        {
            Given_HexString("1020211E");
            AssertCode(     // fcmpe	s0,s1
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|NZCV = cond(s0 - s1)");
        }

        [Test]
        public void AArch64Rw_smsubl()
        {
            Given_HexString("F1D8269B");
            AssertCode(     // smsubl	x17,w7,w6,x22
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x17 = x22 - w7 *64 w6");
        }

        [Test]
        public void AArch64Rw_smulh()
        {
            Given_HexString("407C409B");
            AssertCode(     // smulh	x0,w2,w0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x0 = SLICE(x2 *s128 x0, int64, 64)");
        }

        [Test]
        public void AArch64Rw_fcmpe_real64_imm()
        {
            Given_HexString("1820601E");
            AssertCode(     // fcmpe	d0,#0.0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|NZCV = cond(d0 - 0.0)");
        }

        [Test]
        public void AArch64Rw_sbc()
        {
            Given_HexString("630002DA");
            AssertCode(     // sbc	x3,x3,x2
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x3 = x3 - x2 - C");
        }

        [Test]
        public void AArch64Rw_rev32()
        {
            Given_HexString("C608C0DA");
            AssertCode(     // rev32	x6,x6
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x6 = __rev32(x6)");
        }

        [Test]
        public void AArch64Rw_rev()
        {
            Given_HexString("C60CC0DA");
            AssertCode(     // rev	x6,x6
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x6 = __rev_64(x6)");
        }

        [Test]
        public void AArch64Rw_rbit()
        {
            Given_HexString("2100C0DA");
            AssertCode(     // rbit	x1,x1
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|x1 = __rbit_64(x1)");
        }

        [Test]
        public void AArch64Rw_hlt()
        {
            Given_HexString("000046D4");
            AssertCode(     // hlt	#&3000
                "0|H--|0000000000100000(4): 1 instructions",
                "1|L--|__hlt(0x3000<16>)");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void AArch64Rw_ld1()
        {
            Given_HexString("0000400D");
            AssertCode(     // ld1	{v0.b}[0],[x0]
                "0|L--|0000000100002A50(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void AArch64Rw_ld1_1()
        {
            Given_HexString("0000400D");
            AssertCode(     // ld1	{v0.b}[0],[x0]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void AArch64Rw_ld1_2reg()
        {
            Given_HexString("00A0404C");
            AssertCode(     // Nyi
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__ld1(x0, out v0, out v1)");
        }

        [Test]
        public void AArch64Rw_ld1_q()
        {
            Given_HexString("4178DF4C");
            AssertCode(     // ld1\t{v1.4s},[x2],#&10
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|__ld1(x2, out v1)",
                "2|L--|x2 = x2 + 16<i64>");
        }

        [Test]
        public void AArch64Rw_ld1_d()
        {
            Given_HexString("4178DF0C");
            AssertCode( // ld1\t{v1.2s},[x2],#&10
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|__ld1(x2, out v1)",
                "2|L--|x2 = x2 + 8<i64>");
        }

        [Test]
        [Ignore("Read up on the specs")]
        public void AArch64Rw_ld1_2()
        {
            Given_HexString("0070404C");
            AssertCode(     // ld1 {v0.16b},[x0]
                "0|L--|0000000100002884(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void AArch64Rw_orr_vector()
        {
            Given_HexString("4195060F");
            AssertCode(     // orr	v1.4h,#&CA00CA
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|d1 = d1 | 0x0CA00CA00CA00CA<64>");
        }

        [Test]
        public void AArch64Rw_orr_vector_lsl()
        {
            Given_HexString("41B5060F");
            AssertCode(     // orr	v1.4h,#&CA00CA,lsl #8
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|d1 = d1 | 0x0CA00CA00CA00CA00<64>");
        }

        [Test]
        public void AArch64Rw_pmul()
        {
            Given_HexString("019C2C2E");
            AssertCode(     // pmul	v1.8b,v0.8b,v12.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d12",
                "3|L--|d1 = __pmul_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_pmull()
        {
            Given_HexString("81E13E0E");
            AssertCode(     // pmull	v1.8h,v12.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d30",
                "3|L--|q1 = __pmull_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_pmull_word64()
        {
            Given_HexString("60E1E00E");
            AssertCode(     // pmull	v0.1q,v11.1d,v0.1d
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d0",
                "3|L--|q0 = __pmull_i64(v2, v3)");
        }

        [Test]
        public void AArch64Rw_pmull2()
        {
            Given_HexString("3FE02D4E");
            AssertCode(     // pmull2	v31.8h,v1.16b,v13.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q13",
                "3|L--|q31 = __pmull2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_pmull2_word64()
        {
            Given_HexString("DFE3ED4E");
            AssertCode(     // pmull2	v31.1q,v30.2d,v13.2d
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q30",
                "2|L--|v3 = q13",
                "3|L--|q31 = __pmull2_i64(v2, v3)");
        }

        [Test]
        public void AArch64Rw_raddhn()
        {
            Given_HexString("2140312E");
            AssertCode(     // raddhn	v1.8b,v1.8h,v17.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q17",
                "3|L--|d1 = __raddhn_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_raddhn2()
        {
            Given_HexString("2D402B6E");
            AssertCode(     // raddhn2	v13.16b,v1.8h,v11.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q11",
                "3|L--|q13 = __raddhn2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_rev64()
        {
            Given_HexString("1108200E");
            AssertCode(     // rev64	v17.8b,v0.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|d17 = __rev64_i8(v2)");
        }

        [Test]
        public void AArch64Rw_rshrn()
        {
            Given_HexString("4B8D0B0F");
            AssertCode(     // rshrn	v11.8b,v10.8h,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q10",
                "2|L--|d11 = __rshrn_i16(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_rshrn2()
        {
            Given_HexString("0A8C094F");
            AssertCode(     // rshrn2	v10.16b,v0.8h,#7
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q10 = __rshrn2_i16(v2, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_rsubhn()
        {
            Given_HexString("4D61312E");
            AssertCode(     // rsubhn	v13.8b,v10.8h,v17.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q10",
                "2|L--|v3 = q17",
                "3|L--|d13 = __rsubhn_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_rsubhn2()
        {
            Given_HexString("5F613E6E");
            AssertCode(     // rsubhn2	v31.16b,v10.8h,v30.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q10",
                "2|L--|v3 = q30",
                "3|L--|q31 = __rsubhn2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_saba()
        {
            Given_HexString("7E7D3F0E");
            AssertCode(     // saba	v30.8b,v11.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d31",
                "3|L--|d30 = __saba_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sabal()
        {
            Given_HexString("6151210E");
            AssertCode(     // sabal	v1.8h,v11.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d1",
                "3|L--|q1 = __sabal_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sabal2()
        {
            Given_HexString("20523E4E");
            AssertCode(     // sabal2	v0.8h,v17.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d30",
                "3|L--|q0 = __sabal2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sabd()
        {
            Given_HexString("FE773E0E");
            AssertCode(     // sabd	v30.8b,v31.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d30",
                "3|L--|d30 = __sabd_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sabdl()
        {
            Given_HexString("6D712D0E");
            AssertCode(     // sabdl	v13.8h,v11.8b,v13.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d13",
                "3|L--|q13 = __sabdl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sabdl2()
        {
            Given_HexString("AB712A4E");
            AssertCode(     // sabdl2	v11.8h,v13.16b,v10.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q13",
                "2|L--|v3 = q10",
                "3|L--|q11 = __sabdl2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sadalp()
        {
            Given_HexString("BE69200E");
            AssertCode(     // sadalp	v30.4h,v13.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d13",
                "3|L--|d30 = __sadalp_i8(v3, v2)");
        }

        [Test]
        public void AArch64Rw_saddl()
        {
            Given_HexString("9F012A0E");
            AssertCode(     // saddl	v31.8h,v12.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d10",
                "3|L--|q31 = __saddl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_saddl2()
        {
            Given_HexString("31003E4E");
            AssertCode(     // saddl2	v17.8h,v1.16b,v30.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q30",
                "3|L--|q17 = __saddl2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_saddlp()
        {
            Given_HexString("E02B200E");
            AssertCode(     // saddlp	v0.4h,v31.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d0 = __saddlp_i8(v2)");
        }

        [Test]
        public void AArch64Rw_saddlv()
        {
            Given_HexString("2B38300E");
            AssertCode(     // saddlv	h11,v1.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|h11 = __saddlv_i8(v2)");
        }

        [Test]
        public void AArch64Rw_saddw()
        {
            Given_HexString("6A113F0E");
            AssertCode(     // saddw	v10.8h,v11.8h,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = d31",
                "3|L--|q10 = __saddw_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_saddw2()
        {
            Given_HexString("3F10314E");
            AssertCode(     // saddw2	v31.8h,v1.8h,v17.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q17",
                "3|L--|q31 = __saddw2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_shadd()
        {
            Given_HexString("1E042A0E");
            AssertCode(     // shadd	v30.8b,v0.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d10",
                "3|L--|d30 = __shadd_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_shl()
        {
            Given_HexString("2C560D0F");
            AssertCode(     // shl	v12.8b,v17.8b,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d17",
                "2|L--|d12 = __shl_i8(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_shll()
        {
            Given_HexString("8C39212E");
            AssertCode(     // shll	v12.8h,v12.8b,#8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d12",
                "2|L--|q12 = __shll_i8(v2, 8<i32>)");
        }

        [Test]
        public void AArch64Rw_shll2()
        {
            Given_HexString("1E38216E");
            AssertCode(     // shll2	v30.8h,v0.16b,lsl #8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q30 = __shll2_i8(v2, 8<i32>)");
        }

        [Test]
        public void AArch64Rw_shsub()
        {
            Given_HexString("4A253F0E");
            AssertCode(     // shsub	v10.8b,v10.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d31",
                "3|L--|d10 = __shsub_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sli()
        {
            Given_HexString("3F56082F");
            AssertCode(     // sli	v31.8b,v17.8b,#8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d17",
                "2|L--|d31 = __sli_i8(v2, 8<i32>)");
        }

        [Test]
        public void AArch64Rw_smaxp()
        {
            Given_HexString("2BA63E0E");
            AssertCode(     // smaxp	v11.8b,v17.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d30",
                "3|L--|d11 = __smaxp_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_smin()
        {
            Given_HexString("0B6C310E");
            AssertCode(     // smin	v11.8b,v0.8b,v17.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d17",
                "3|L--|d11 = __smin_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sminp()
        {
            Given_HexString("CBAF3F0E");
            AssertCode(     // sminp	v11.8b,v30.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d31",
                "3|L--|d11 = __sminp_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sminv()
        {
            Given_HexString("0DA8310E");
            AssertCode(     // sminv	b13,v0.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|b13 = __sminv_i8(v2)");
        }

        [Test]
        public void AArch64Rw_smlal()
        {
            Given_HexString("2B80210E");
            AssertCode(     // smlal	v11.8h,v1.8b,v1.8b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = d1",
                "3|L--|v4 = d1",
                "4|L--|q11 = __smlal_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_smlal2()
        {
            Given_HexString("4081314E");
            AssertCode(     // smlal2	v0.8h,v10.16b,v17.16b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q10",
                "3|L--|v4 = q17",
                "4|L--|q0 = __smlal2_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_smlsl()
        {
            Given_HexString("C1A33E0E");
            AssertCode(     // smlsl	v1.8h,v30.8b,v30.8b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = d30",
                "3|L--|v4 = d30",
                "4|L--|q1 = __smlsl_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_smlsl2()
        {
            //$BUG: should be ternary
            Given_HexString("C0A32C4E");
            AssertCode(     // smlsl2	v0.8h,v30.16b,v12.16b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q30",
                "3|L--|v4 = q12",
                "4|L--|q0 = __smlsl2_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_smov()
        {
            Given_HexString("E42F170E");
            AssertCode(     // smov	w4,v31.b[11]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q31",
                "2|L--|w4 = CONVERT(v2[11<i32>], int8, int32)");
        }

        [Test]
        public void AArch64Rw_smull()
        {
            Given_Instruction(0x9B237C43);
            AssertCode(     // smull\tx3,w2,w3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = CONVERT(w2 *s w3, int32, int64)");
        }

        [Test]
        public void AArch64Rw_smull2_vector()
        {
            Given_HexString("E1C3314E");
            AssertCode(     // smull2	v1.8h,v31.16b,v17.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q31",
                "2|L--|v3 = q17",
                "3|L--|q1 = __smull2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqabs()
        {
            Given_HexString("ED7B200E");
            AssertCode(     // sqabs	v13.8b,v31.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d13 = __sqabs_i8(v2)");
        }

        [Test]
        public void AArch64Rw_sqadd()
        {
            Given_HexString("F10F2A0E");
            AssertCode(     // sqadd	v17.8b,v31.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d10",
                "3|L--|d17 = __sqadd_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqdmlal_by_element()
        {
            Given_HexString("D13B5B0F");
            AssertCode(     // sqdmlal	v17.4s,v30.4h,v11.h[5]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q17",
                "2|L--|v3 = d30",
                "3|L--|v4 = d11[5<i32>]",
                "4|L--|q17 = __sqdmlal_i32(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqdmlal_scalar_by_element()
        {
            Given_HexString("4B317D5F");
            AssertCode(     // sqdmlal	s11,h10,v13.h[3]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|s11 = __sqdmlal(s11, h10, d13[3<i32>])");
        }

        [Test]
        public void AArch64Rw_sqdmlal2_by_element()
        {
            Given_HexString("BE39514F");
            AssertCode(     // sqdmlal2	v30.4s,v13.8h,v1.h[5]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q30",
                "2|L--|v3 = q13",
                "3|L--|v4 = q1[5<i32>]",
                "4|L--|q30 = __sqdmlal2_i32(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqdmlsl2_scalar_by_element()
        {
            Given_HexString("6171705F");
            AssertCode(     // sqdmlsl2	s1,h11,v0.h[3]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|s1 = __sqdmlsl2(s1, h11, d0[3<i32>])");
        }

        [Test]
        public void AArch64Rw_sqdmlsl_vector_by_element()
        {
            Given_HexString("AB715D0F");
            AssertCode(     // sqdmlsl	v11.4s,v13.4h,v13.h[1]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = d13",
                "3|L--|v4 = d13[1<i32>]",
                "4|L--|q11 = __sqdmlsl_i32(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqdmlsl2_vector_by_element()
        {
            Given_HexString("6079614F");
            AssertCode(     // sqdmlsl2	v0.4s,v11.8h,v1.h[6]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q11",
                "3|L--|v4 = q1[6<i32>]",
                "4|L--|q0 = __sqdmlsl2_i32(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqdmulh()
        {
            Given_HexString("31B4600E");
            AssertCode(     // sqdmulh	v17.4h,v1.4h,v0.4h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = d0",
                "3|L--|d17 = __sqdmulh_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqdmulh_element()
        {
            Given_HexString("BFC95D5F");
            AssertCode(     // sqdmulh	h31,h13,v13.h[5]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|h31 = __sqdmulh(h13, d13[5<i32>])");
        }

        [Test]
        public void AArch64Rw_sqdmull_scalar_by_element()
        {
            Given_HexString("1FB87B5F");
            AssertCode(     // sqdmull	s31,h0,v11.h[7]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|s31 = __sqdmull(s31, h0, d11[7<i32>])");
        }

        [Test]
        public void AArch64Rw_sqdmull_vector_by_element()
        {
            Given_HexString("D1BB4D0F");
            AssertCode(     // sqdmull	v17.4s,v30.4h,v13.h[4]
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d13[4<i32>]",
                "3|L--|q17 = __sqdmull_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqdmull2_by_element()
        {
            Given_HexString("01B0414F");
            AssertCode(     // sqdmull2	v1.4s,v0.8h,v1.h[0]
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q1[0<i32>]",
                "3|L--|q1 = __sqdmull2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqneg()
        {
            Given_HexString("9F79202E");
            AssertCode(     // sqneg	v31.8b,v12.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d12",
                "2|L--|d31 = __sqneg_i8(v2)");
        }

        [Test]
        public void AArch64Rw_sqrdmlah_scalar_by_element()
        {
            Given_HexString("0DD8A17F");
            AssertCode(     // sqrdmlah	s13,s0,v1.s[3]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|s13 = __sqrdmlah(s13, s0, d1[3<i32>])");
        }

        [Test]
        public void AArch64Rw_sqrdmlah_vector_by_element()
        {
            Given_HexString("6BD97D2F");
            AssertCode(     // sqrdmlah	v11.4h,v11.4h,v13.h[7]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d11",
                "3|L--|v4 = d13[7<i32>]",
                "4|L--|d11 = __sqrdmlah_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqrdmlsh_scalar_by_element()
        {
            Given_HexString("2CF8AD7F");
            AssertCode(     // sqrdmlsh	s12,s1,v13.s[3]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|s12 = __sqrdmlsh(s12, s1, d13[3<i32>])");
        }

        [Test]
        public void AArch64Rw_sqrdmlsh_vector_by_element()
        {
            Given_HexString("EAFB7B2F");
            AssertCode(     // sqrdmlsh	v10.4h,v31.4h,v11.h[7]
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d31",
                "3|L--|v4 = d11[7<i32>]",
                "4|L--|d10 = __sqrdmlsh_i16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_sqrdmulh_scalar_by_element()
        {
            Given_HexString("3ED2715F");
            AssertCode(     // sqrdmulh	h30,h17,v1.h[3]
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|h30 = __sqrdmulh(h30, h17, d1[3<i32>])");
        }

        [Test]
        public void AArch64Rw_sqrdmulh_vector_by_element()
        {
            Given_HexString("8CD97B0F");
            AssertCode(     // sqrdmulh	v12.4h,v12.4h,v11.h[7]
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d11[7<i32>]",
                "3|L--|d12 = __sqrdmulh_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqrshl()
        {
            Given_HexString("8A5D210E");
            AssertCode(     // sqrshl	v10.8b,v12.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d1",
                "3|L--|d10 = __sqrshl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqrshrn()
        {
            Given_HexString("2D9C0B0F");
            AssertCode(     // sqrshrn	v13.8b,v1.8h,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|d13 = __sqrshrn_i16(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_sqrshrn2()
        {
            Given_HexString("AC9D094F");
            AssertCode(     // sqrshrn2	v12.16b,v13.8h,#7
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q13",
                "2|L--|q12 = __sqrshrn2_i16(v2, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_sqrshrun()
        {
            Given_HexString("CD8F0A2F");
            AssertCode(     // sqrshrun	v13.8b,v30.8h,#6
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q30",
                "2|L--|d13 = __sqrshrun_i16(v2, 6<i32>)");
        }

        [Test]
        public void AArch64Rw_sqrshrun2()
        {
            Given_HexString("B18D0A6F");
            AssertCode(     // sqrshrun2	v17.16b,v13.8h,#6
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q13",
                "2|L--|q17 = __sqrshrun2_i16(v2, 6<i32>)");
        }

        [Test]
        public void AArch64Rw_sqshl()
        {
            Given_HexString("EA4F3E0E");
            AssertCode(     // sqshl	v10.8b,v31.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d30",
                "3|L--|d10 = __sqshl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqshlu()
        {
            Given_HexString("00640D2F");
            AssertCode(     // sqshlu	v0.8b,v0.8b,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|d0 = __sqshlu_i8(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_sqshrn()
        {
            Given_HexString("8B950B0F");
            AssertCode(     // sqshrn	v11.8b,v12.8h,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q12",
                "2|L--|d11 = __sqshrn_i16(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_sqshrn2()
        {
            Given_HexString("7F950B4F");
            AssertCode(     // sqshrn2	v31.16b,v11.8h,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q11",
                "2|L--|q31 = __sqshrn2_i16(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_sqsub()
        {
            Given_HexString("4D2D2C0E");
            AssertCode(     // sqsub	v13.8b,v10.8b,v12.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d12",
                "3|L--|d13 = __sqsub_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sqxtn()
        {
            Given_HexString("6049210E");
            AssertCode(     // sqxtn	v0.8b,v11.8h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q11",
                "2|L--|d0 = __sqxtn_i16(v2)");
        }

        [Test]
        public void AArch64Rw_sqxtn2()
        {
            Given_HexString("0048214E");
            AssertCode(     // sqxtn2	v0.16b,v0.8h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q0 = __sqxtn2_i16(v2)");
        }

        [Test]
        public void AArch64Rw_sqxtun()
        {
            Given_HexString("A129212E");
            AssertCode(     // sqxtun	v1.8b,v13.8h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q13",
                "2|L--|d1 = __sqxtun_i16(v2)");
        }

        [Test]
        public void AArch64Rw_sqxtun2()
        {
            Given_HexString("2D2A216E");
            AssertCode(     // sqxtun2	v13.16b,v17.8h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q17",
                "2|L--|q13 = __sqxtun2_i16(v2)");
        }

        [Test]
        public void AArch64Rw_srhadd()
        {
            Given_HexString("2A162A0E");
            AssertCode(     // srhadd	v10.8b,v17.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d10",
                "3|L--|d10 = __srhadd_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sri()
        {
            Given_HexString("F147092F");
            AssertCode(     // sri	v17.8b,v31.8b,#7
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d17 = __sri_i8(v2, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_srshl()
        {
            Given_HexString("6B55200E");
            AssertCode(     // srshl	v11.8b,v11.8b,v0.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d0",
                "3|L--|d11 = __srshl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_srshr()
        {
            Given_HexString("AC250C0F");
            AssertCode(     // srshr	v12.8b,v13.8b,#4
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d13",
                "2|L--|d12 = __srshr_i8(v2, 4<i32>)");
        }

        [Test]
        public void AArch64Rw_srsra()
        {
            Given_HexString("2A340D0F");
            AssertCode(     // srsra	v10.8b,v1.8b,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d10 = __srsra_i8(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_sshl()
        {
            Given_HexString("2A443E0E");
            AssertCode(     // sshl	v10.8b,v1.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d1",
                "2|L--|v3 = d30",
                "3|L--|d10 = __sshl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_sshll()
        {
            Given_HexString("01A40E0F");
            AssertCode(     // sshll	v1.8h,v0.8b,#6
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|q1 = __sshll_i8(v2, 6<i32>)");
        }

        [Test]
        public void AArch64Rw_sshll2()
        {
            Given_HexString("3EA4094F");
            AssertCode(     // sshll2	v30.8h,v1.16b,#1
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|q30 = __sshll2_i8(v2, 1<i32>)");
        }

        [Test]
        public void AArch64Rw_sshr()
        {
            Given_HexString("E107090F");
            AssertCode(     // sshr	v1.8b,v31.16b,#7
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d1 = __sshr_i8(v2, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_sshr_imm()
        {
            Given_HexString("C1070F0F");
            AssertCode(     // sshr	v1.8b,v30.16b,#1
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d30",
                "2|L--|d1 = __sshr_i8(v2, 1<i32>)");
        }

        [Test]
        public void AArch64Rw_ssra()
        {
            Given_HexString("4B150D0F");
            AssertCode(     // ssra	v11.8b,v10.8b,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d10",
                "2|L--|d11 = __ssra_i8(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_ssubl()
        {
            Given_HexString("2022310E");
            AssertCode(     // ssubl	v0.8b,v17.4h,v17.4h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d17",
                "3|L--|q0 = __ssubl_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ssubw()
        {
            Given_HexString("AB312C0E");
            AssertCode(     // ssubw	v11.8h,v13.8h,v12.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q13",
                "2|L--|v3 = d12",
                "3|L--|q11 = __ssubw_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ssubw2()
        {
            Given_HexString("EC33314E");
            AssertCode(     // ssubw2	v12.8h,v31.8h,v17.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q31",
                "2|L--|v3 = q17",
                "3|L--|q12 = __ssubw2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ssubl2()
        {
            Given_HexString("7E212C4E");
            AssertCode(     // ssubl2	v30.16b,v12.8h,v12.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = q12",
                "3|L--|q30 = __ssubl2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_subhn()
        {
            Given_HexString("2D602C0E");
            AssertCode(     // subhn	v13.8b,v1.8h,v12.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q12",
                "3|L--|d13 = __subhn_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_subhn2()
        {
            Given_HexString("6A612B4E");
            AssertCode(     // subhn2	v10.16b,v11.8h,v11.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = q11",
                "3|L--|q10 = __subhn2_i16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_suqadd()
        {
            Given_HexString("AA39200E");
            AssertCode(     // suqadd	v10.8b,v13.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d13",
                "3|L--|d10 = __suqadd_i8(v3, v2)");
        }

        [Test]
        public void AArch64Rw_tbl()
        {
            Given_HexString("5101010E");
            AssertCode(     // tbl	v17.8b,{v10.8b},v17.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d17 = __tbl_1(v10, v2)");
        }

        [Test]
        public void AArch64Rw_tbl_r3l()
        {
            Given_HexString("4A41010E");
            AssertCode(     // tbl	v10.8b,{v10.8b-v12.8b},v1.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d10 = __tbl_3(v10, v11, v12, v2)");
        }

        [Test]
        public void AArch64Rw_tbx()
        {
            Given_HexString("5E110A0E");
            AssertCode(     // tbx	v30.8b,{v10.16b},v10.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d10",
                "2|L--|d30 = __tbx_1(v10, v2)");
        }

        [Test]
        public void AArch64Rw_tbz()
        {
            Given_Instruction(0x36686372);
            AssertCode(     // tbz\tw18,#&D,#&100C6C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((w18 & 0x2000<32>) == 0<32>) branch 00100C6C");
        }

        [Test]
        public void AArch64Rw_trn1()
        {
            Given_HexString("01281F0E");
            AssertCode(     // trn1	v1.8b,v0.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d31",
                "3|L--|d1 = __trn1_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_trn2()
        {
            Given_HexString("8D690C0E");
            AssertCode(     // trn2	v13.8b,v12.8b,v12.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d12",
                "3|L--|d13 = __trn2_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uaba()
        {
            Given_HexString("207E3F2E");
            AssertCode(     // uaba	v0.8b,v17.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d31",
                "3|L--|d0 = __uaba_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uabal()
        {
            Given_HexString("8D512B2E");
            AssertCode(     // uabal	v13.8h,v12.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d11",
                "3|L--|q13 = __uabal_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uabal2()
        {
            Given_HexString("81512A6E");
            AssertCode(     // uabal2	v1.8h,v12.16b,v10.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q12",
                "2|L--|v3 = q10",
                "3|L--|q1 = __uabal2_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uabd()
        {
            Given_HexString("6174696E");
            AssertCode(     // uabd	v1.8h,v3.8h,v9.8h
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q3",
                "2|L--|v3 = q9",
                "3|L--|q1 = __uabd_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uabdl()
        {
            Given_HexString("AC71312E");
            AssertCode(     // uabdl	v12.8h,v13.8b,v17.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d13",
                "2|L--|v3 = d17",
                "3|L--|q12 = __uabdl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uabdl2()
        {
            Given_HexString("6B71206E");
            AssertCode(     // uabdl2	v11.8h,v11.16b,v0.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q11",
                "2|L--|v3 = q0",
                "3|L--|q11 = __uabdl2_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uadalp()
        {
            Given_HexString("C06B202E");
            AssertCode(     // uadalp	v0.4h,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d30",
                "3|L--|d0 = __uadalp_u8(v3, v2)");
        }

        [Test]
        public void AArch64Rw_uaddl()
        {
            Given_HexString("80012C2E");
            AssertCode(     // uaddl	v0.8h,v12.8b,v12.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d12",
                "3|L--|q0 = __uaddl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uaddl2()
        {
            Given_HexString("DF032A6E");
            AssertCode(     // uaddl2	v31.8h,v30.16b,v10.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q30",
                "2|L--|v3 = q10",
                "3|L--|q31 = __uaddl2_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uaddlp()
        {
            Given_HexString("AD29202E");
            AssertCode(     // uaddlp	v13.4h,v13.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d13",
                "2|L--|v3 = d13",
                "3|L--|d13 = __uaddlp_u8(v3, v2)");
        }

        [Test]
        public void AArch64Rw_uaddlv()
        {
            Given_HexString("1138302E");
            AssertCode(     // uaddlv	h17,v0.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|h17 = __uaddlv_u8(v2)");
        }

        [Test]
        public void AArch64Rw_uaddw_i16()
        {
            Given_HexString("8210232E");
            AssertCode(     // uaddw\tv2.8h,v4.8h,v3.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = d3",
                "3|L--|q2 = __uaddw_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uaddw2()
        {
            Given_HexString("2A102D6E");
            AssertCode(     // uaddw2	v10.8h,v1.8h,v13.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q13",
                "3|L--|q10 = __uaddw_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ucvtf_vector()
        {
            Given_HexString("EBDB792E");
            AssertCode(     // ucvtf	v11.4h,v31.4h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = d31",
                "2|L--|d11 = __ucvtf_u16(v4)");
        }

        [Test]
        public void AArch64Rw_ucvtf_real32_int32()
        {
            Given_Instruction(0x1E230101);
            AssertCode(     // ucvtf\ts1,w8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = CONVERT(w8, uint32, real32)");
        }

        [Test]
        public void AArch64Rw_ucvtf_vector_fixed()
        {
            Given_HexString("4AE56D6F");
            AssertCode( // ucvtf\tv10.2d,v10.2d,#&13
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v3 = q10",
                "2|L--|q10 = __ucvtf_fixed(v3, 19<i32>)");
        }

        [Test]
        public void AArch64Rw_uhadd()
        {
            Given_HexString("E1073E2E");
            AssertCode(     // uhadd	v1.8b,v31.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d30",
                "3|L--|d1 = __uhadd_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uhsub()
        {
            Given_HexString("8125312E");
            AssertCode(     // uhsub	v1.16b,v12.16b,v17.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q12",
                "2|L--|v3 = q17",
                "3|L--|q1 = __uhsub_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_umax()
        {
            Given_HexString("C1672B2E");
            AssertCode(     // umax	v1.8b,v30.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d11",
                "3|L--|d1 = __umax_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_umaxp()
        {
            Given_HexString("1EA4212E");
            AssertCode(     // umaxp	v30.8b,v0.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d1",
                "3|L--|d30 = __umaxp_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_umaxv()
        {
            Given_HexString("AAA9302E");
            AssertCode(     // umaxv	b10,v13.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d13",
                "2|L--|b10 = __umaxv_u8(v2)");
        }

        [Test]
        public void AArch64Rw_umin()
        {
            Given_HexString("116C3E2E");
            AssertCode(     // umin	v17.8b,v0.8b,v30.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d0",
                "2|L--|v3 = d30",
                "3|L--|d17 = __umin_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uminp()
        {
            Given_HexString("4BAD2B2E");
            AssertCode(     // uminp	v11.8b,v10.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d11",
                "3|L--|d11 = __uminp_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uminv()
        {
            Given_HexString("2BAA312E");
            AssertCode(     // uminv	b11,v17.8b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d17",
                "2|L--|b11 = __uminv_u8(v2)");
        }

        [Test]
        public void AArch64Rw_umlal_vector()
        {
            Given_Instruction(0x2E208045);
            AssertCode(     // umlal\tv5.4h,v2.8b,v0.8b
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v2 = q5",
                "2|L--|v3 = d2",
                "3|L--|v4 = d0",
                "4|L--|q5 = __umlal_u16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_umlal2()
        {
            Given_HexString("EC83216E");
            AssertCode(     // umlal2	v12.8h,v31.16b,v1.16b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q12",
                "2|L--|v3 = q31",
                "3|L--|v4 = q1",
                "4|L--|q12 = __umlal2_u16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_umlsl()
        {
            Given_HexString("CDA3312E");
            AssertCode(     // umlsl	v13.8h,v30.8b,v17.8b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q13",
                "2|L--|v3 = d30",
                "3|L--|v4 = d17",
                "4|L--|q13 = __umlsl_u16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_umlsl2()
        {
            Given_HexString("9FA12B6E");
            AssertCode(     // umlsl2	v31.8h,v12.16b,v11.16b
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v2 = q31",
                "2|L--|v3 = q12",
                "3|L--|v4 = q11",
                "4|L--|q31 = __umlsl2_u16(v2, v3, v4)");
        }

        [Test]
        public void AArch64Rw_umov()
        {
            Given_HexString("043C0D0E");
            AssertCode(     // umov	w4,v0.b[6]
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|w4 = CONVERT(v2[6<i32>], uint8, uint32)");
        }

        [Test]
        public void AArch64Rw_umull2()
        {
            Given_HexString("20C02D6E");
            AssertCode(     // umull2	v0.8h,v1.16b,v13.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q13",
                "3|L--|q0 = __umull2_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uqadd()
        {
            Given_HexString("810D2A2E");
            AssertCode(     // uqadd	v1.8b,v12.8b,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d10",
                "3|L--|d1 = __uqadd_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uqrshl()
        {
            Given_HexString("715D3F2E");
            AssertCode(     // uqrshl	v17.8b,v11.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d11",
                "2|L--|v3 = d31",
                "3|L--|d17 = __uqrshl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uqrshrn()
        {
            Given_HexString("FE9F0C2F");
            AssertCode(     // uqrshrn	v30.8b,v31.4h,#4
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d30 = __uqrshrn_u16(v2, 4<i32>)");
        }

        [Test]
        public void AArch64Rw_uqrshrn2()
        {
            Given_HexString("EA9F0D6F");
            AssertCode(     // uqrshrn2	v10.8b,v31.4h,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d10 = __uqrshrn2_u16(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_uqshl()
        {
            Given_HexString("CD4F3F2E");
            AssertCode(     // uqshl	v13.8b,v30.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d30",
                "2|L--|v3 = d31",
                "3|L--|d13 = __uqshl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uqshrn()
        {
            Given_HexString("AA950D2F");
            AssertCode(     // uqshrn	v10.8b,v13.4h,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d13",
                "2|L--|d10 = __uqshrn_u16(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_uqshrn2()
        {
            Given_HexString("1F940B6F");
            AssertCode(     // uqshrn2	v31.8b,v0.4h,#5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|d31 = __uqshrn2_u16(v2, 5<i32>)");
        }

        [Test]
        public void AArch64Rw_uqsub()
        {
            Given_HexString("BE2D2A2E");
            AssertCode(     // uqsub	v30.16b,v13.16b,v10.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q13",
                "2|L--|v3 = q10",
                "3|L--|q30 = __uqsub_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uqxtn()
        {
            Given_HexString("5F49212E");
            AssertCode(     // uqxtn	v31.8b,v10.4h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d10",
                "2|L--|d31 = __uqxtn_u16(v2)");
        }

        [Test]
        public void AArch64Rw_uqxtn2()
        {
            Given_HexString("E04B216E");
            AssertCode(     // uqxtn2	v0.8b,v31.4h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|d0 = __uqxtn2_u16(v2)");
        }

        [Test]
        public void AArch64Rw_urecpe()
        {
            Given_HexString("DFCBA10E");
            AssertCode(     // urecpe	v31.2s,v30.2s
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d30",
                "2|L--|d31 = __urecpe_u32(v2)");
        }

        [Test]
        public void AArch64Rw_urhadd()
        {
            Given_HexString("AA153F2E");
            AssertCode(     // urhadd	v10.8b,v13.8b,v31.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d13",
                "2|L--|v3 = d31",
                "3|L--|d10 = __urhadd_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_urshl()
        {
            Given_HexString("F157202E");
            AssertCode(     // urshl	v17.8b,v31.8b,v0.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d0",
                "3|L--|d17 = __urshl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_urshr()
        {
            Given_HexString("3F24082F");
            AssertCode(     // urshr	v31.8b,v1.8b,#8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d31 = __urshr_u8(v2, 8<i32>)");
        }

        [Test]
        public void AArch64Rw_ursqrte()
        {
            Given_HexString("21C8A12E");
            AssertCode(     // ursqrte	v1.2s,v1.2s
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d1",
                "2|L--|d1 = __ursqrte_u32(v2)");
        }

        [Test]
        public void AArch64Rw_ursra()
        {
            Given_HexString("6A350D2F");
            AssertCode(     // ursra	v10.8b,v11.8b,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d11",
                "2|L--|d10 = __ursra_u8(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_ushl()
        {
            Given_HexString("E147312E");
            AssertCode(     // ushl	v1.8b,v31.8b,v17.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d17",
                "3|L--|d1 = __ushl_u8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_ushll()
        {
            Given_HexString("EDA70B2F");
            AssertCode(     // ushll	v13.8h,v31.8b,#3
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d31",
                "2|L--|q13 = __ushll_u8(v2, 3<i32>)");
        }

        [Test]
        public void AArch64Rw_ushll2()
        {
            Given_HexString("11A40C6F");
            AssertCode(     // ushll2	v17.8h,v0.16b,#4
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q17 = __ushll2_u8(v2, 4<i32>)");
        }

        [Test]
        public void AArch64Rw_usqadd()
        {
            Given_HexString("B139202E");
            AssertCode(     // usqadd	v17.8b,v13.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d13",
                "3|L--|d17 = __usqadd_u8(v3, v2)");
        }

        [Test]
        public void AArch64Rw_usra()
        {
            Given_HexString("8C15092F");
            AssertCode(     // usra	v12.8b,v12.8b,#7
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = d12",
                "2|L--|d12 = __usra_u8(v2, 7<i32>)");
        }

        [Test]
        public void AArch64Rw_usubl()
        {
            Given_HexString("2C22202E");
            AssertCode(     // usubl	v12.8h,v17.8b,v0.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d0",
                "3|L--|q12 = __usubl_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_usubl2()
        {
            Given_HexString("8121216E");
            AssertCode(     // usubl2	v1.16h,v12.8b,v1.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q12",
                "2|L--|v3 = q1",
                "3|L--|q1 = __usubl2_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_usubw()
        {
            Given_HexString("11302A2E");
            AssertCode(     // usubw	v17.8h,v0.8h,v10.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = d10",
                "3|L--|q17 = __usubw_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_usubw2()
        {
            Given_HexString("40312D6E");
            AssertCode(     // usubw2	v0.8h,v10.8h,v13.16b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = q10",
                "2|L--|v3 = q13",
                "3|L--|q0 = __usubw2_u16(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uxth()
        {
            Given_Instruction(0x53003C00);	// uxth	w0,w0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = CONVERT(SLICE(w0, uint16, 0), uint16, uint32)");
        }

        [Test]
        public void AArch64Rw_uxtl()
        {
            Given_Instruction(0x2F08A400);
            AssertCode(     // uxtl\tv0.8h,v0.8b
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|q0 = __uxtl_u8(v2)");
        }

        [Test]
        public void AArch64Rw_uxtl2()
        {
            Given_HexString("11A4086F");
            AssertCode(     // uxtl2	v17.8h,v0.16b
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|q17 = __uxtl2_u8(v2)");
        }

        [Test]
        public void AArch64Rw_uzp1()
        {
            Given_HexString("51190B0E");
            AssertCode(     // uzp1	v17.8b,v10.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d10",
                "2|L--|v3 = d11",
                "3|L--|d17 = __uzp1_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_uzp2()
        {
            Given_HexString("9159110E");
            AssertCode(     // uzp2	v17.8b,v12.8b,v17.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d12",
                "2|L--|v3 = d17",
                "3|L--|d17 = __uzp2_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_xtn()
        {
            Given_Instruction(0x0E612A10);
            AssertCode(     // xtn\tv16.4h,v16.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q16",
                "2|L--|d16 = __xtn_i32(v2)");
        }

        [Test]
        public void AArch64Rw_xtn2()
        {
            Given_HexString("C02B214E");
            AssertCode(     // xtn2	v0.16b,v30.8h
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v2 = q30",
                "2|L--|q0 = __xtn2_i16(v2)");
        }

        [Test]
        public void AArch64Rw_zip1()
        {
            Given_HexString("2D3A0B0E");
            AssertCode(     // zip1	v13.8b,v17.8b,v11.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d17",
                "2|L--|v3 = d11",
                "3|L--|d13 = __zip1_i8(v2, v3)");
        }

        [Test]
        public void AArch64Rw_zip2()
        {
            Given_HexString("F17B110E");
            AssertCode(     // zip2	v17.8b,v31.8b,v17.8b
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v2 = d31",
                "2|L--|v3 = d17",
                "3|L--|d17 = __zip2_i8(v2, v3)");
        }
    }
}