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
using Reko.Arch.Avr;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr8DisassemblerTests : DisassemblerTestBase<AvrInstruction>
    {
        private readonly Avr8Architecture arch;

        public Avr8DisassemblerTests()
        {
            this.arch = new Avr8Architecture("avr8");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr16(0x0000); } }

        private void AssertCode(string sExp, params ushort[] uInstrs)
        {
            // Convert to LE.
            var bytes =
            uInstrs.Select(u => new byte[] { (byte)u, (byte)(u >> 8) })
                .SelectMany(b => b)
                .ToArray();
            var i = Disassemble(new MemoryArea(LoadAddress, bytes));
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Avr8_dis_rjmp()
        {
            AssertCode("rjmp\t001A", 0xC00C);
        }

        [Test]
        public void Avr8_dis_eor()
        {
            AssertCode("eor\tr1,r1", 0x2411);
        }

        [Test]
        public void Avr8_dis_out()
        {
            AssertCode("out\t3F,r1", 0xBE1F);
        }

        [Test]
        public void Avr8_dis_in()
        {
            AssertCode("in\tr1,3F", 0xB61F);
        }

        [Test]
        public void Avr8_dis_ldi()
        {
            AssertCode("ldi\tr28,5F", 0xE5CF);
        }

        [Test]
        public void Avr8_dis_rcall()
        {
            AssertCode("rcall\t0006", 0xD002);
        }

        [Test]
        public void Avr8_dis_push()
        {
            AssertCode("push\tr29", 0x93DF);
        }

        [Test]
        public void Avr8_dis_pop()
        {
            AssertCode("pop\tr28", 0x91CF);
        }

        [Test]
        public void Avr8_dis_ret()
        {
            AssertCode("ret", 0x9508);
        }

        [Test]
        public void Avr8_dis_cli()
        {
            AssertCode("cli", 0x94F8);
        }

        [Test]
        public void Avr8_dis_940x()
        {
            AssertCode("com\tr0", 0x9400);
            AssertCode("neg\tr0", 0x9401);
            AssertCode("swap\tr0", 0x9402);
            AssertCode("inc\tr0", 0x9403);
            AssertCode("invalid", 0x9404);
            AssertCode("asr\tr0", 0x9405);
            AssertCode("lsr\tr0", 0x9406);
            AssertCode("ror\tr0", 0x9407);
            AssertCode("sec", 0x9408);
            AssertCode("ijmp", 0x9409);
            AssertCode("dec\tr0", 0x940A);
            AssertCode("des\t00", 0x940B);
            AssertCode("jmp\t00600000", 0x958C, 0x0000);
            AssertCode("jmp\t00420000", 0x950D, 0x0000);
            AssertCode("call\t00440000", 0x951E, 0x0000);
            AssertCode("call\t007F2468", 0x95FF, 0x9234);
    }

        [Test]
        public void Avr8_dis_cpi()
        {
            AssertCode("cpi\tr26,09", 0x30A9);
            AssertCode("cpi\tr31,FF", 0x3FFF);
        }

        [Test]
        public void Avr8_dis_cpc()
        {
            AssertCode("cpc\tr18,r4", 0x0524);
            AssertCode("cpc\tr31,r31", 0x07FF);
        }

        [Test]
        public void Avr8_dis_brcc()
        {
            AssertCode("brcs\t0002", 0xF000);
            AssertCode("brid\t0040", 0xF4FF);
        }

        [Test]
        public void Avr8_dis_movw()
        {
            AssertCode("movw\tr0,r8", 0x0104);
            AssertCode("movw\tr30,r28", 0x01FE);
        }

        [Test]
        public void Avr8_dis_adiw()
        {
            AssertCode("adiw\tr24,01", 0x9601);
            AssertCode("adiw\tr26,15", 0x9655);
        }

        [Test]
        public void Avr8_dis_adc()
        {
            AssertCode("adc\tr26,r1", 0x1DA1);
        }

        [Test]
        public void Avr8_dis_sts()
        {
            AssertCode("sts\t1234,r1", 0x9210, 0x1234);
        }

        [Test]
        public void Avr8_dis_st_z()
        {
            AssertCode("st\tz,r24", 0x8380);
        }

        [Test]
        public void Avr8_dis_st_z_postinc()
        {
            AssertCode("st\tz+,r9", 0x9291);
        }
        
        [Test]
        public void Avr8_dis_regression1()
        {
            AssertCode("sbis\t05,00", 0x9BA8);
            AssertCode("ld\tr24,x", 0x8180);
            AssertCode("rjmp\tFFFC", 0xCFFD);
        }

        [Test]
        public void Avr8_dis_lds()
        {
            AssertCode("lds\tr18,0080", 0x9120, 0x0080);
        }

        [Test]
        public void Avr8_dis_lpm()
        {
            AssertCode("lpm", 0x95C8);
            AssertCode("lpm\tr25,z+", 0x9195);
        }

        [Test]
        public void Avr8_dis_858F()
        {
            AssertCode("ldd\tr24,y+0F", 0x858F);
        }

        [Test]
        public void Avr8_dis_878F()
        {
            AssertCode("std\ty+0F,r24", 0x878F);
        }

        [Test]
        public void Avr8_dis_muls()
        {
            AssertCode("muls\tr16,r16", 0x0200);
            AssertCode("muls\tr30,r18", 0x02E2);
            AssertCode("muls\tr30,r31", 0x02EF);
        }

        public void Avr8_dis_FF84()
        {
            AssertCode("sbrs\tr24,4", 0xFF84);
        }

        [Test]
        public void Avr8_dis_8B4B()
        {
            AssertCode("std\ty+13,r20", 0x8B4B);
        }

        [Test]
        public void Avr8_dis_8A1C()
        {
            AssertCode("std\ty+14,r1", 0x8A1C);
        }

        [Test]
        public void Avr8_dis_sbrs()
        {
            AssertCode("sbrc\tr8,04", 0xFC84);
            AssertCode("sbrc\tr15,07", 0xFCF7);
            AssertCode("invalid", 0xFCF8);
            AssertCode("sbrc\tr30,07", 0xFDE7);
            AssertCode("invalid", 0xFCE8);
        }
    }
}
