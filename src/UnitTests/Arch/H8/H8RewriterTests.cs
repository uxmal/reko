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
        public void H8Rw_beq()
        {
            Given_HexString("470A");
            AssertCode(     // beq	9B0E
                "0|T--|8000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 800C");
        }

        [Test]
        public void H8Rw_bld()
        {
            Given_HexString("7771");
            AssertCode(     // bld	#0x07,r1h
                "0|L--|8000(2): 1 instructions",
                "1|L--|C = __btst(r1h, 7<8>)");
        }

        [Test]
        public void H8Rw_bset()
        {
            Given_HexString("700C");
            AssertCode(     // bset	#0x00,r4l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r4l = __bst(r4l, true, 0<8>)");
        }

        [Test]
        public void H8Rw_bst()
        {
            Given_HexString("670A");
            AssertCode(     // bst	#0x00,r2l
                "0|L--|8000(2): 1 instructions",
                "1|L--|r2l = __bst(r2l, C, 0<8>)");
        }

        [Test]
        public void H8Rw_btst()
        {
            Given_HexString("7308");
            AssertCode(     // btst	#0x00,r0l
                "0|L--|8000(2): 1 instructions",
                "1|L--|Z = __btst(r0l, 0<8>)");
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
        public void H8Rw_jmp()
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
                "1|T--|call 9AF8 (2)");
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
        public void H8Rw_mulxu()
        {
            Given_HexString("5012");
            AssertCode(     // mulxu.b	r1h,r2h
                "0|L--|8000(2): 1 instructions",
                "1|L--|r2 = r2h *u16 r1h");
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
        public void H8Rw_rotxl()
        {
            Given_HexString("1202");
            AssertCode(     // rotxl.b	r2h
                "0|L--|8000(2): 3 instructions",
                "1|L--|r2h = __rcl<byte,int32,bool>(r2h, 1<i32>, C)",
                "2|L--|NZC = cond(r2h)",
                "3|L--|V = false");
        }

        [Test]
        public void H8Rw_rotxr()
        {
            Given_HexString("130A");
            AssertCode(     // rotxr.b	r2l
                "0|L--|8000(2): 3 instructions",
                "1|L--|r2l = __rcr<byte,int32,bool>(r2l, 1<i32>, C)",
                "2|L--|NZC = cond(r2l)",
                "3|L--|V = false");
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
        public void H8Rw_shll()
        {
            Given_HexString("1000");
            AssertCode(     // shll.b	r0h
                "0|L--|8000(2): 2 instructions",
                "1|L--|r0h = r0h << 1<i32>",
                "2|L--|NZVC = cond(r0h)");
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
        public void H8Rw_xor()
        {
            Given_HexString("652C");
            AssertCode(     // xor.w	r2,e4
                "0|L--|8000(2): 3 instructions",
                "1|L--|e4 = e4 ^ r2",
                "2|L--|NZ = cond(e4)",
                "3|L--|V = false");
        }
        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues






    }
}
