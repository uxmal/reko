#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.H8;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.H8
{
    public class H8RewriterTests : RewriterTestBase
    {
        private readonly H8Architecture arch;
        private readonly Address addrLoad;

        public H8RewriterTests()
        {
            this.arch = new H8Architecture(CreateServiceContainer(), "h8", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr16(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        [Test]
        public void H8Rw_add()
        {
            Given_HexString("0933");
            AssertCode(     // add.w	r3,r3
                "0|L--|8000(2): 2 instructions",
                "1|L--|r3 = r3 + r3",
                "2|L--|NZVC = cond(r3)");
        }

        [Test]
        public void H8Rw_adds()
        {
            Given_HexString("0B87");
            AssertCode(     // adds	#0x00000002,sp
                "0|L--|8000(2): 1 instructions",
                "1|L--|sp = sp + 2<32>");
        }

        [Test]
        public void H8Rw_addx()
        {
            Given_HexString("9E42");
            AssertCode(     // addx.b	#0x42,r6l
                "0|L--|8000(2): 2 instructions",
                "1|L--|r6l = r6l + 0x42<8> + C",
                "2|L--|NZVC = cond(r6l)");
        }

        [Test]
        public void H8Rw_and()
        {
            Given_HexString("EA01");
            AssertCode(     // and.b	#0x01,r2l
                "0|L--|8000(2): 3 instructions",
                "1|L--|r2l = r2l & 1<8>",
                "2|L--|NZ = cond(r2l)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_andc()
        {
            Given_HexString("06CB");
            AssertCode(     // andc	#0xCB,ccr
                "0|L--|8000(2): 1 instructions",
                "1|L--|ccr = ccr & 0xCB<8>");
        }

        [Test]
        public void H8Rw_band()
        {
            Given_HexString("7645");
            AssertCode(     // band	#0x04,r5h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C & __bit<byte,byte>(r5h, 4<8>)");
        }

        [Test]
        public void H8Rw_bclr()
        {
            Given_HexString("7279");
            AssertCode(     // bclr	#0x07,r1l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r1l = __clear_bit<byte,byte>(r1l, 7<8>)");
        }

        [Test]
        public void H8Rw_bclr_imm_ind()
        {
            Given_HexString("7D007240");
            AssertCode(
                "0|L--|8000(4): 1 instructions",
                "1|L--|Mem0[er0:byte] = __clear_bit<byte,byte>(Mem0[er0:byte], 4<8>)");
        }

        [Test]
        public void H8Rw_beq()
        {
            Given_HexString("470A");
            AssertCode(     // beq	9B0E
                "0|T--|8000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 800C");
        }

        [Test]
        public void H8Rw_biand()
        {
            Given_HexString("76A7");
            AssertCode(     // biand	#0x02,r7h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C & ~__bit<byte,byte>(r7h, 2<8>)");
        }

        [Test]
        public void H8Rw_bild()
        {
            Given_HexString("77C8");
            AssertCode(     // bild	#0x04,r0l
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = ~__bit<byte,byte>(r0l, 4<8>)");
        }

        [Test]
        public void H8Rw_bior()
        {
            Given_HexString("74BF");
            AssertCode(     // bior	#0x03,r7l
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C | ~__bit<byte,byte>(r7l, 3<8>)");
        }

        [Test]
        public void H8Rw_bist()
        {
            Given_HexString("67B8");
            AssertCode(     // bist	#0x03,r0l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r0l = __write_bit<byte,byte>(r0l, 3<8>, ~C)");
        }

        [Test]
        public void H8Rw_bixor()
        {
            Given_HexString("75B4");
            AssertCode(     // bixor	#0x03,r4h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C ^ ~__bit<byte,byte>(r4h, 3<8>)");
        }

        [Test]
        public void H8Rw_bld()
        {
            Given_HexString("7771");
            AssertCode(     // bld	#0x07,r1h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = __btst<byte,byte>(r1h, 7<8>)");
        }

        [Test]
        public void H8Rw_bnot()
        {
            Given_HexString("7116");
            AssertCode(     // bnot	#0x01,r6h
                "0|L--|8000(2): 1 instructions",
                "1|L--|r6h = __invert_bit<byte,byte>(r6h, 1<8>)");
        }

        [Test]
        public void H8Rw_bor()
        {
            Given_HexString("7403");
            AssertCode(     // bor	#0x00,r3h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C | __bit<byte,byte>(r3h, 0<8>)");
        }

        [Test]
        public void H8Rw_bset()
        {
            Given_HexString("700C");
            AssertCode(     // bset	#0x00,r4l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r4l = __bset<byte,byte>(r4l, true, 0<8>)");
        }

        [Test]
        public void H8Rw_bsr()
        {
            Given_HexString("5588");
            AssertCode(     // bsr	00000B46
                "0|T--|8000(2): 1 instructions",
                "1|T--|call 7F88 (2)");
        }

        [Test]
        public void H8Rw_bst()
        {
            Given_HexString("670A");
            AssertCode(     // bst	#0x00,r2l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r2l = __bset<byte,byte>(r2l, C, 0<8>)");
        }

        [Test]
        public void H8Rw_btst()
        {
            Given_HexString("7308");
            AssertCode(     // btst	#0x00,r0l
                "0|L--|8000(2): 1 instructions",
                "1|L--|Z = __btst<byte,byte>(r0l, 0<8>)");
        }

        [Test]
        public void H8Rw_bxor()
        {
            Given_HexString("7519");
            AssertCode(     // bxor	#0x01,r1l
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = C ^ __bit<byte,byte>(r1l, 1<8>)");
        }

        [Test]
        public void H8Rw_clrmac()
        {
            Given_HexString("01A0");
            AssertCode(     // clrmac
                "0|L--|8000(2): 1 instructions",
                "1|L--|mac = 0<64>");
        }

        [Test]
        public void H8Rw_cmp()
        {
            Given_HexString("1D32");
            AssertCode(     // cmp.w	r3,r2
                "0|L--|8000(2): 1 instructions",
                "1|L--|NZVC = cond(r2 - r3)");
        }

        [Test]
        public void H8Rw_daa()
        {
            Given_HexString("0F02");
            AssertCode(     // daa	r2h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r2h = __decimal_adjust_add(r2h, C, H)",
                "2|L--|HNZVC = cond(r2h)");
        }

        [Test]
        public void H8Rw_das()
        {
            Given_HexString("1F02");
            AssertCode(     // das	r2h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r2h = __decimal_adjust_subtract(r2h, C, H)",
                "2|L--|HNZVC = cond(r2h)");
        }

        [Test]
        public void H8Rw_dec()
        {
            Given_HexString("1A0D");
            AssertCode(     // dec.b	r5l
                "0|L--|8000(2): 2 instructions",
                "1|L--|r5l = r5l - 1<8>",
                "2|L--|NZV = cond(r5l)");
        }

        [Test]
        public void H8Dis_divxs_b()
        {
            Given_HexString("01D051FB");
            AssertCode( // divxs.b\tr7l,e3
                "0|L--|8000(4): 5 instructions",
                "1|L--|v5 = e3 /8 r7l",
                "2|L--|v6 = e3 %s r7l",
                "3|L--|Z = cond(r7l)",
                "4|L--|N = cond(v5)",
                "5|L--|e3 = SEQ(v6, v5)" +
                "");
        }

        [Test]
        public void H8Dis_divxs_w()
        {
            Given_HexString("01D05385");
            AssertCode( // divxs.w\te0,er5
                "0|L--|8000(4): 5 instructions",
                "1|L--|v5 = er5 /16 e0",
                "2|L--|v6 = er5 %s e0",
                "3|L--|Z = cond(e0)",
                "4|L--|N = cond(v5)",
                "5|L--|er5 = SEQ(v6, v5)");
        }

        [Test]
        public void H8Rw_exts()
        {
            Given_HexString("17D8");
            AssertCode(     // exts.w	e0
                "0|L--|8000(2): 4 instructions",
                "1|L--|e0 = CONVERT(r0l, byte, int16)",
                "2|L--|Z = cond(e0)",
                "3|L--|N = false",
                "4|L--|V = false");
        }

        [Test]
        public void H8Rw_extu()
        {
            Given_HexString("1773");
            AssertCode(     // extu.l	er3
                "0|L--|8000(2): 4 instructions",
                "1|L--|er3 = CONVERT(r3, word16, uint32)",
                "2|L--|Z = cond(er3)",
                "3|L--|N = false",
                "4|L--|V = false");
        }

        [Test]
        public void H8Rw_inc()
        {
            Given_HexString("0A03");
            AssertCode(     // inc.b	r3h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r3h = r3h + 1<8>",
                "2|L--|NZV = cond(r3h)");
        }

        [Test]
        public void H8Rw_inc_w_2()
        {
            Given_HexString("0BD0");
            AssertCode(     // inc.w\t#2,r0
                "0|L--|8000(2): 2 instructions",
                "1|L--|r0 = r0 + 2<16>",
                "2|L--|NZV = cond(r0)");
        }

        [Test]
        public void H8Rw_jmp()
        {
            Given_HexString("5A01 1C48");
            AssertCode(
                "0|T--|8000(4): 1 instructions",
                "1|T--|goto 00011C48");
        }

        [Test]
        public void H8Rw_jmp_to_register()
        {
            Given_HexString("5900");
            AssertCode(     // jmp	@er0
                "0|T--|8000(2): 1 instructions",
                "1|T--|goto er0");
        }

        [Test]
        public void H8Rw_jsr()
        {
            Given_HexString("5E009AF8");
            AssertCode(     // jsr	@0x9AF8:24
                "0|T--|8000(4): 1 instructions",
                "1|T--|call 00009AF8 (2)");
        }

        [Test]
        public void H8Rw_ldc()
        {
            Given_HexString("07D0");
            AssertCode(     // ldc.b	#0xD0,ccr
                "0|L--|8000(2): 1 instructions",
                "1|L--|NZVC = 0xD0<8>");
        }

        [Test]
        public void H8Rw_ldm_l()
        {
            Given_HexString("01306D77");
            AssertCode( // ldm.l\t@sp+,(er4-sp)
                "0|L--|8000(4): 8 instructions",
                "1|L--|sp = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|L--|er6 = Mem0[sp:word32]",
                "4|L--|sp = sp + 4<i32>",
                "5|L--|er5 = Mem0[sp:word32]",
                "6|L--|sp = sp + 4<i32>",
                "7|L--|er4 = Mem0[sp:word32]",
                "8|L--|sp = sp + 4<i32>");
        }

        [Test]
        public void H8Rw_ldmac_h()
        {
            Given_HexString("0321");
            AssertCode( // ldmac\ter1,mach",
                "0|L--|8000(2): 1 instructions",
                "1|L--|mach = er1");
        }

        [Test]
        public void H8Rw_ldmac_l()
        {
            Given_HexString("0331");
            AssertCode( // ldmac\ter1,macl
                "0|L--|8000(2): 1 instructions",
                "1|L--|macl = er1");
        }

        [Test]
        public void H8Rw_mov_postinc_reg()
        {
            Given_HexString("0100 6D05");
            AssertCode(     // mov.l @er0+, er5
                "0|L--|8000(4): 5 instructions",
                "1|L--|v4 = er0",
                "2|L--|er0 = er0 + 4<i32>",
                "3|L--|er5 = Mem0[v4:word32]",
                "4|L--|NZ = cond(er5)",
                "5|L--|V = false");
        }

        [Test]
        public void H8Rw_mov_push()
        {
            Given_HexString("6DF0");
            AssertCode(     // mov.w	r0,@-sp
                "0|L--|8000(2): 4 instructions",
                "1|L--|sp = sp - 2<i32>",
                "2|L--|Mem0[sp:word16] = r0",
                "3|L--|NZ = cond(r0)",
                "4|L--|V = false");
        }

        [Test]
        public void H8Rw_movfpe()
        {
            Given_HexString("6A430123");
            AssertCode(     // movfpe\t@aa,r3
                "0|L--|8000(4): 1 instructions",
                "1|L--|r3h = __move_from_peripheral(0x123<u16>)");
        }

        [Test]
        public void H8Rw_movtpe()
        {
            Given_HexString("6AC30123");
            AssertCode(     // movtpe\tr3,@aa
                "0|L--|8000(4): 1 instructions",
                "1|L--|__move_to_peripheral(0x123<u16>, r3h)");
        }

        [Test]
        public void H8Rw_mulxs_b()
        {
            Given_HexString("01C05034");
            AssertCode(     // mulxs.b\tr3l,r4", 
                "0|L--|8000(4): 2 instructions",
                "1|L--|er4 = r4 *s32 r3h",
                "2|L--|NZ = cond(er4)");
        }

        [Test]
        public void H8Rw_mulxs_w()
        {
            Given_HexString("01C05234");
            AssertCode(     // mulxs.w\tr3l,r4", 
                "0|L--|8000(4): 2 instructions",
                "1|L--|er4 = er4 *s32 r3",
                "2|L--|NZ = cond(er4)");
        }

        [Test]
        public void H8Rw_mulxu()
        {
            Given_HexString("5012");
            AssertCode(     // mulxu.b	r1h,r2h
                "0|L--|8000(2): 1 instructions",
                "1|L--|r2 = r2h *u16 r1h");
        }

        [Test]
        public void H8Rw_neg()
        {
            Given_HexString("1786");
            AssertCode(     // neg.b	r6h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r6h = -r6h",
                "2|L--|NZVC = cond(r6h)");
        }

        [Test]
        public void H8Rw_not()
        {
            Given_HexString("1708");
            AssertCode(     // not.b	r0l
                "0|L--|8000(2): 3 instructions",
                "1|L--|r0l = ~r0l",
                "2|L--|NZ = cond(r0l)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_or()
        {
            Given_HexString("C010");
            AssertCode(     // or.b	#0x10,r0h
                "0|L--|8000(2): 3 instructions",
                "1|L--|r0h = r0h | 0x10<8>",
                "2|L--|NZ = cond(r0h)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_orc()
        {
            Given_HexString("0474");
            AssertCode(     // orc	#0x74,ccr
                "0|L--|8000(2): 1 instructions",
                "1|L--|ccr = ccr | 0x74<8>");
        }

        [Test]
        public void H8Rw_rotl()
        {
            Given_HexString("12B3");
            AssertCode(     // rotl.l	er3
                "0|L--|8000(2): 3 instructions",
                "1|L--|er3 = __rol<word32,byte>(er3, 1<8>)",
                "2|L--|NZC = cond(er3)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_rotr()
        {
            Given_HexString("1395");
            AssertCode(     // rotr.w	r5
                "0|L--|8000(2): 3 instructions",
                "1|L--|r5 = __ror<word16,byte>(r5, 1<8>)",
                "2|L--|NZC = cond(r5)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_rotxl()
        {
            Given_HexString("1202");
            AssertCode(     // rotxl.b	r2h
                "0|L--|8000(2): 3 instructions",
                "1|L--|r2h = __rcl<byte,byte>(r2h, 1<8>, C)",
                "2|L--|NZC = cond(r2h)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_rotxr()
        {
            Given_HexString("130A");
            AssertCode(     // rotxr.b	r2l
                "0|L--|8000(2): 3 instructions",
                "1|L--|r2l = __rcr<byte,byte>(r2l, 1<8>, C)",
                "2|L--|NZC = cond(r2l)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_rte()
        {
            Given_HexString("5670");
            AssertCode(     // rte
                "0|R--|8000(2): 2 instructions",
                "1|L--|__return_from_exception()",
                "2|R--|return (2,0)");
        }

        [Test]
        public void H8Rw_rts()
        {
            Given_HexString("5470");
            AssertCode(     // rts
                "0|R--|8000(2): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void H8Rw_shar()
        {
            Given_HexString("1182");
            AssertCode(     // shar.b	r2h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r2h = r2h >> 1<i32>",
                "2|L--|NZVC = cond(r2h)");
        }

        [Test]
        public void H8Rw_shll_l()
        {
            Given_HexString("1031");
            AssertCode(     // shll.l	#1,er1
                "0|L--|8000(2): 2 instructions",
                "1|L--|er1 = er1 << 1<i32>",
                "2|L--|NZVC = cond(er1)");
        }

        [Test]
        public void H8Rw_shll_l_2()
        {
            Given_HexString("1071");
            AssertCode( // shll.l  #2,er1
                "0|L--|8000(2): 2 instructions",
                "1|L--|er1 = er1 << 2<i32>",
                "2|L--|NZVC = cond(er1)");
        }

        [Test]
        public void H8Dis_sleep()
        {
            Given_HexString("0180");
            AssertCode(     // sleep
                "0|L--|8000(2): 1 instructions",
                "1|L--|__sleep()");
        }

        [Test]
        public void H8Rw_stc()
        {
            Given_HexString("0209");
            AssertCode(     // stc.b	ccr,r1l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r1l = ccr");
        }

        [Test]
        public void H8Rw_stm()
        {
            Given_HexString("01306DF4");
            AssertCode(     // stm.l\ter4-er7,@-sp
                "0|L--|8000(4): 8 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = er4",
                "3|L--|sp = sp - 4<i32>",
                "4|L--|Mem0[sp:word32] = er5",
                "5|L--|sp = sp - 4<i32>",
                "6|L--|Mem0[sp:word32] = er6",
                "7|L--|sp = sp - 4<i32>",
                "8|L--|Mem0[sp:word32] = sp");
        }

        [Test]
        public void H8Rw_stmac_h()
        {
            Given_HexString("0221");
            AssertCode( // stmac\tmach,er1
                "0|L--|8000(2): 1 instructions",
                "1|L--|er1 = mach");
        }

        [Test]
        public void H8Rw_stmac_l()
        {
            Given_HexString("0231");
            AssertCode( // stmac\tmacl,er1
                "0|L--|8000(2): 1 instructions",
                "1|L--|er1 = macl");
        }

        [Test]
        public void H8Rw_sub()
        {
            Given_HexString("1888");
            AssertCode(     // sub.b	r0l,r0l
                "0|L--|8000(2): 2 instructions",
                "1|L--|r0l = r0l - r0l",
                "2|L--|NZVC = cond(r0l)");
        }

        [Test]
        public void H8Rw_subs()
        {
            Given_HexString("1B87");
            AssertCode(     // subs	#0x00000002,sp
                "0|L--|8000(2): 1 instructions",
                "1|L--|sp = sp - 2<32>");
        }

        [Test]
        public void H8Rw_trapa()
        {
            Given_HexString("57F2");
            AssertCode(     // trapa	#0x03
                "0|L--|8000(2): 1 instructions",
                "1|L--|__syscall<byte>(3<8>)");
        }

        [Test]
        public void H8Rw_xor()
        {
            Given_HexString("652C");
            AssertCode(     // xor.w	r2,e4
                "0|L--|8000(2): 3 instructions",
                "1|L--|e4 = e4 ^ r2",
                "2|L--|NZ = cond(e4)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_xorc()
        {
            Given_HexString("051D");
            AssertCode(     // xorc	#0x1D,ccr
                "0|L--|8000(2): 1 instructions",
                "1|L--|ccr = ccr ^ 0x1D<8>");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues















    }
}
