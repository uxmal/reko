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
using Reko.Arch.Altera;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.Altera
{
    [TestFixture]
    public class Nios2RewriterTests : RewriterTestBase
    {
        private readonly Nios2Architecture arch;
        private readonly Address addr;

        public Nios2RewriterTests()
        {
            this.arch = new Nios2Architecture(CreateServiceContainer(), "nios2", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void Nios2Rw_add()
        {
            Given_HexString("3A888731");
            AssertCode(     // add	r3,r6,r6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r6 + r6");
        }

        [Test]
        public void Nios2Rw_add_zero()
        {
            Given_HexString("3A880520");
            AssertCode(     // add	r2,r4,zero
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r4");
        }

        [Test]
        public void Nios2Rw_addi()
        {
            Given_HexString("04EDFFDE");
            AssertCode(     // addi	sp,sp,0xFFFFFFB4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = sp - 76<i32>");
        }

        [Test]
        public void Nios2Rw_and()
        {
            Given_HexString("3A70C410");
            AssertCode(     // and	r2,r2,r3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 & r3");
        }

        [Test]
        public void Nios2Rw_andi()
        {
            Given_HexString("CC3F8010");
            AssertCode(     // andi	r2,r2,0xFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 & 0xFF<32>");
        }

        [Test]
        public void Nios2Rw_beq()
        {
            Given_HexString("26070030");
            AssertCode(     // beq	r6,zero,C4001F50
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r6 == 0<32>) branch 00100020");
        }

        [Test]
        public void Nios2Rw_br()
        {
            Given_HexString("06020000");
            AssertCode(     // br	C4001FC0
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0010000C");
        }

        [Test]
        public void Nios2Rw_call()
        {
            Given_HexString("804E0040");
            AssertCode(     // call	C8001AF0
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 041004EC (0)");
        }

        [Test]
        public void Nios2Rw_cmpltu()
        {
            Given_HexString("3A808718");
            AssertCode(     // cmpltu	r3,r3,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = CONVERT(r3 <u r2, bool, word32)");
        }

        [Test]
        public void Nios2Rw_flushp()
        {
            Given_HexString("3A200000");
            AssertCode(     // flushp
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__flushp()");
        }

        [Test]
        public void Nios2Rw_initi()
        {
            Given_HexString("3A480108");
            AssertCode(     // initi	r1
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__initi(r1)");
        }

        [Test]
        public void Nios2Rw_jmp()
        {
            Given_HexString("3A680008");
            AssertCode(     // jmp	r1
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r1");
        }

        [Test]
        public void Nios2Rw_jmpi()
        {
            Given_HexString("01000000");
            AssertCode(     // jmpi	C4002A84
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 00100004");
        }

        [Test]
        public void Nios2Rw_ldb()
        {
            Given_HexString("0700C010");
            AssertCode(     // ldb	r3,0(r2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = CONVERT(Mem0[r2:int8], int8, int32)");
        }

        [Test]
        public void Nios2Rw_ldw()
        {
            Given_HexString("1700C010");
            AssertCode(     // ldw	r3,0(r2)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r2:word32]");
        }

        [Test]
        public void Nios2Rw_nextpc()
        {
            Given_HexString("3AE00200");
            AssertCode(     // nextpc	r1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = 00100004");
        }

        [Test]
        public void Nios2Rw_nor()
        {
            Given_HexString("3A308402");
            AssertCode(     // nor	r2,zero,r10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = ~r10");
        }

        [Test]
        public void Nios2Rw_or()
        {
            Given_HexString("3AB08A28");
            AssertCode(     // or	r5,r5,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 | r2");
        }

        [Test]
        public void Nios2Rw_orhi()
        {
            Given_HexString("3400B100");
            AssertCode(     // orhi	r2,zero,0xC400
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = 0xC4000000<32>");
        }

        [Test]
        public void Nios2Rw_ret()
        {
            Given_HexString("3A2800F8");
            AssertCode(     // ret
                "0|R--|00100000(4): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void Nios2Rw_sll()
        {
            Given_HexString("3A988420");
            AssertCode(     // sll	r2,r4,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r4 << r2");
        }

        [Test]
        public void Nios2Rw_slli()
        {
            Given_HexString("FA902698");
            AssertCode(     // slli	r19,r19,0x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = r19 << 3<i32>");
        }

        [Test]
        public void Nios2Rw_srli()
        {
            Given_HexString("3AD20C10");
            AssertCode(     // srli	r6,r2,0x8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = r2 >>u 8<i32>");
        }

        [Test]
        public void Nios2Rw_stb()
        {
            Given_HexString("05004019");
            AssertCode(     // stb	r5,0(r3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = SLICE(r5, byte, 0)",
                "2|L--|Mem0[r3:byte] = v3");
        }

        [Test]
        public void Nios2Rw_stw()
        {
            Given_HexString("150080D9");
            AssertCode(     // stw	r6,0(sp)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp:word32] = r6");
        }

        [Test]
        public void Nios2Rw_sub()
        {
            Given_HexString("3AC88308");
            AssertCode(     // sub	r1,r1,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r1 - r2");
        }

        [Test]
        public void Nios2Rw_wrctl()
        {
            Given_HexString("3A700100");
            AssertCode(     // wrctl	0x0,zero
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__wrctl(status, 0<32>)");
        }

        [Test]
        public void Nios2Rw_xor()
        {
            Given_HexString("3AF04411");
            AssertCode(     // xor	r2,r2,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 ^ r5");
        }

        [Test]
        public void Nios2Rw_xori()
        {
            Given_HexString("1C208010");
            AssertCode(     // xori	r2,r2,0x80
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 ^ 0x80<32>");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues






































    }
}
