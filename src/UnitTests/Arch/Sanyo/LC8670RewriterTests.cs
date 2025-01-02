#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Sanyo;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Sanyo
{
    public class LC8670RewriterTests : RewriterTestBase
    {
        private readonly LC8670Architecture arch;
        private readonly Address addr;

        public LC8670RewriterTests()
        {
            this.arch = new LC8670Architecture(CreateServiceContainer(), "lc8670", new Dictionary<string, object>());
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        [Test]
        public void LC8670Rw_add()
        {
            Given_HexString("82AF");
            AssertCode(     // add      $00AF
                "0|L--|0100(2): 2 instructions",
                "1|L--|ACC = ACC + Mem0[0x00AF<p16>:byte]",
                "2|L--|CAV = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_addc()
        {
            Given_HexString("95");
            AssertCode(     // addc     @R1
                "0|L--|0100(1): 2 instructions",
                "1|L--|ACC = ACC + Mem0[R1:byte] + C",
                "2|L--|CAV = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_and()
        {
            Given_HexString("E180");
            AssertCode(     // and      80
                "0|L--|0100(2): 1 instructions",
                "1|L--|ACC = ACC & 0x80<8>");
        }

        [Test]
        public void LC8670Rw_be()
        {
            Given_HexString("31647D");
            AssertCode(     // be       64,0854
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (ACC == 0x64<8>) branch 017F");
        }

        [Test]
        public void LC8670Rw_bn()
        {
            Given_HexString("8A2D03");
            AssertCode(     // bn       $002D,+00000002,05E6
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (!__test1(Mem0[0x002D<p16>:byte], 2<i32>)) branch 0105");
        }

        [Test]
        public void LC8670Rw_bne()
        {
            Given_HexString("4502");
            AssertCode(     // bne      R1,07C1
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Mem0[R1:byte] != R1) branch 0104");
        }

        [Test]
        public void LC8670Rw_bnz()
        {
            Given_HexString("90F2");
            AssertCode(     // bnz      33D5
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (ACC != 0<8>) branch 00F4");
        }

        [Test]
        public void LC8670Rw_bp()
        {
            Given_HexString("6F2C1E");
            AssertCode(     // bp       $012C,+00000007,0120
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (__test1(Mem0[0x012C<p16>:byte], 7<i32>)) branch 0120");
        }

        [Test]
        public void LC8670Rw_br()
        {
            Given_HexString("01EB");
            AssertCode(     // br       07B0
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 00ED");
        }

        [Test]
        public void LC8670Rw_brf()
        {
            Given_HexString("118900");
            AssertCode(     // brf      0873
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 018B");
        }

        [Test]
        public void LC8670Rw_bz()
        {
            Given_HexString("8029");
            AssertCode(     // bz       0510
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (ACC == 0<8>) branch 012B");
        }

        [Test]
        public void LC8670Rw_call()
        {
            Given_HexString("0F65");
            AssertCode(     // call     0765
                "0|T--|0100(2): 1 instructions",
                "1|T--|call 0765 (2)");
        }

        [Test]
        public void LC8670Rw_callr()
        {
            Given_HexString("100000");
            AssertCode(     // callr    193F
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 0102 (2)");
        }

        [Test]
        public void LC8670Rw_clr1()
        {
            Given_HexString("DF10");
            AssertCode(     // clr1     @T0CON,+07
                "0|L--|0100(2): 1 instructions",
                "1|L--|T0CON = __clr1(T0CON, 7<i8>)");
        }

        [Test]
        public void LC8670Rw_dbnz()
        {
            Given_HexString("5302F5");
            AssertCode(     // dbnz     @B,4639
                "0|T--|0100(3): 3 instructions",
                "1|L--|v3 = B - 1<8>",
                "2|L--|B = v3",
                "3|T--|if (v3 != 0<8>) branch 00F7");
        }

        [Test]
        public void LC8670Rw_dec()
        {
            Given_HexString("727F");
            AssertCode(     // dec      $007F
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[0x007F<p16>:byte] = Mem0[0x007F<p16>:byte] - 1<8>");
        }

        [Test]
        public void LC8670Rw_inc()
        {
            Given_HexString("6302");
            AssertCode(     // inc      @B
                "0|L--|0100(2): 1 instructions",
                "1|L--|B = B + 1<8>");
        }

        [Test]
        public void LC8670Rw_callf()
        {
            Given_HexString("2045FE");
            AssertCode(     // callf    45FE
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 45FE (2)");
        }

        [Test]
        public void LC8670Rw_div()
        {
            Given_HexString("40");
            AssertCode(     // div
                "0|L--|0100(1): 5 instructions",
                "1|L--|v5 = ACC_C",
                "2|L--|ACC_C = v5 /u B",
                "3|L--|B = v5 %u B",
                "4|L--|C = false",
                "5|L--|V = B == 0<8>");
        }

        [Test]
        public void LC8670Rw_jmp()
        {
            Given_HexString("2F45");
            AssertCode(     // jmp      0745
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 0745");
        }

        [Test]
        public void LC8670Rw_jmpf()
        {
            Given_HexString("210480");
            AssertCode(     // jmpf     0480
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0480");
        }

        [Test]
        public void LC8670Rw_ld()
        {
            Given_HexString("027E");
            AssertCode(     // ld       $007E
                "0|L--|0100(2): 1 instructions",
                "1|L--|ACC = Mem0[0x007E<p16>:byte]");
        }

        [Test]
        public void LC8670Rw_ldc()
        {
            Given_HexString("C1");
            AssertCode(     // ldc
                "0|L--|0100(1): 1 instructions",
                "1|L--|ACC = Mem0[TRH_TRL + ACC:byte]");
        }

        [Test]
        public void LC8670Rw_mov()
        {
            //$TODO: should go to RAM section
            Given_HexString("234E00");
            AssertCode(     // mov      00,@P3INT
                "0|L--|0100(3): 1 instructions",
                "1|L--|P3INT = 0<8>");
        }

        [Test]
        public void LC8670Rw_mul()
        {
            Given_HexString("30");
            AssertCode(     // mul
                "0|L--|0100(1): 3 instructions",
                "1|L--|ACC_C_B = ACC_C *u24 B",
                "2|L--|C = false",
                "3|L--|V = ACC_C_B >=u 0x10000<u24>");
        }

        [Test]
        public void LC8670Rw_nop()
        {
            Given_HexString("00");
            AssertCode(     // nop
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void LC8670Rw_not1()
        {
            Given_HexString("B80D");
            AssertCode(     // not1     @EXT,+00
                "0|L--|0100(2): 1 instructions",
                "1|L--|EXT = __not1(EXT, 0<i8>)");
        }

        [Test]
        public void LC8670Rw_or()
        {
            Given_HexString("D28D");
            AssertCode(     // or       $008D
                "0|L--|0100(2): 1 instructions",
                "1|L--|ACC = ACC | Mem0[0x008D<p16>:byte]");
        }

        [Test]
        public void LC8670Rw_pop()
        {
            Given_HexString("710E");
            AssertCode(     // pop      @OCR
                "0|L--|0100(2): 2 instructions",
                "1|L--|OCR = Mem0[SP:byte]",
                "2|L--|SP = SP - 1<i8>");
        }

        [Test]
        public void LC8670Rw_push()
        {
            Given_HexString("6101");
            AssertCode(     // push     @PSW
                "0|L--|0100(2): 2 instructions",
                "1|L--|SP = SP + 1<i8>",
                "2|L--|Mem0[SP:byte] = PSW");
        }

        [Test]
        public void LC8670Rw_ret()
        {
            Given_HexString("A0");
            AssertCode(     // ret
                "0|R--|0100(1): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void LC8670Rw_reti()
        {
            Given_HexString("B0");
            AssertCode(     // reti
                "0|R--|0100(1): 2 instructions",
                "1|L--|__leave_isr()",
                "2|R--|return (2,0)");
        }

        [Test]
        public void LC8670Rw_rol()
        {
            Given_HexString("E0");
            AssertCode(     // rol
                "0|L--|0100(1): 1 instructions",
                "1|L--|ACC = __rol<byte,byte>(ACC, 1<8>)");
        }

        [Test]
        public void LC8670Rw_rolc()
        {
            Given_HexString("F0");
            AssertCode(     // rolc
                "0|L--|0100(1): 2 instructions",
                "1|L--|ACC = __rcl<byte,byte>(ACC, 1<8>, C)",
                "2|L--|C = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_rorc()
        {
            Given_HexString("D0");
            AssertCode(     // rorc
                "0|L--|0100(1): 2 instructions",
                "1|L--|ACC = __rcr<byte,byte>(ACC, 1<8>, C)",
                "2|L--|C = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_ror()
        {
            Given_HexString("C0");
            AssertCode(     // ror
                "0|L--|0100(1): 1 instructions",
                "1|L--|ACC = __ror<byte,byte>(ACC, 1<8>)");
        }

        [Test]
        public void LC8670Rw_set1()
        {
            Given_HexString("F901");
            AssertCode(     // set1     @PSW,+01
                "0|L--|0100(2): 1 instructions",
                "1|L--|PSW = __set1(PSW, 1<i8>)");
        }

        [Test]
        public void LC8670Rw_st()
        {
            Given_HexString("1315");
            AssertCode(     // st       @T0HR
                "0|L--|0100(2): 1 instructions",
                "1|L--|T0HR = ACC");
        }

        [Test]
        public void LC8670Rw_sub()
        {
            Given_HexString("A302");
            AssertCode(     // sub      @B
                "0|L--|0100(2): 2 instructions",
                "1|L--|ACC = ACC - B",
                "2|L--|CAV = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_subc()
        {
            Given_HexString("B212");
            AssertCode(     // subc     $0012
                "0|L--|0100(2): 2 instructions",
                "1|L--|ACC = ACC - Mem0[0x0012<p16>:byte] - C",
                "2|L--|CAV = cond(ACC)");
        }

        [Test]
        public void LC8670Rw_xch()
        {
            Given_HexString("C7");
            AssertCode(     // xch      @R3
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = ACC",
                "2|L--|ACC = Mem0[R3:byte]",
                "3|L--|Mem0[R3:byte] = v4");
        }

        [Test]
        public void LC8670Rw_xor()
        {
            Given_HexString("F181");
            AssertCode(     // xor      81
                "0|L--|0100(2): 1 instructions",
                "1|L--|ACC = ACC ^ 0x81<8>");
        }







































    }

}
