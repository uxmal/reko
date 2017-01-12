#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private Avr8Architecture arch;

        public Avr8DisassemblerTests()
        {
            this.arch = new Avr8Architecture();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr16(0x0000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        private void AssertCode(string sExp, params ushort [] uInstrs)
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
            AssertCode("jmp\t00300000", 0x958C, 0x0000);
            AssertCode("jmp\t00210000", 0x950D, 0x0000);
            AssertCode("call\t00220000", 0x951E, 0x0000);
            AssertCode("call\t003F1234", 0x95FF, 0x1234);
        }
/*
}        0E 94
         1F 01
         0C 94
         26 01
         0C 94
         00 00
         08 95
         68 EE
         73 E0
         80 E0
         90 E0
         0E 94
         BE 00
         08 95
         1F 92
         0F 92
         0F B6
         0F 92
         11 24
         2F 93
         3F 93
         8F 93
         9F 93
         AF 93
         BF 93
         80 91
         04 01
         90 91
         05 01
         A0 91
         06 01
         B0 91
         07 01
         30 91
         08 01
         01 96
         A1 1D
         B1 1D
         23 2F
         2D 5F
         2D 37
         20 F0
         2D 57
         01 96
         A1 1D
         B1 1D
         20 93
         08 01
         80 93
         04 01
         90 93
         05 01
         A0 93
         06 01
         B0 93
         07 01
         80 91
         00 01
         90 91
         01 01
         A0 91
         02 01
         B0 91
         03 01
         01 96
         A1 1D
         B1 1D
         80 93
         00 01
         90 93
         01 01
         A0 93
         02 01
         B0 93
         03 01
         BF 91
         AF 91
         9F 91
         8F 91
         3F 91
         2F 91
         0F 90
         0F BE
         0F 90
         1F 90
         18 95
         9F B7
         F8 94
         20 91
         00 01
         30 91
         01 01
         40 91
         02 01
         50 91
         03 01
         86 B5
         A8 9B
         06 C0
         8F 3F
         21 F0
         2F 5F
         3F 4F
         4F 4F
         5F 4F
         9F BF
         54 2F
         43 2F
         32 2F
         22 27
         28 0F
         31 1D
         41 1D
         51 1D
         82 E0
         22 0F
         33 1F
         44 1F
         55 1F
         8A 95
         D1 F7
         B9 01
         CA 01
         08 95
         EF 92
         FF 92
         0F 93
         1F 93
         CF 93
         DF 93
         7B 01
         8C 01
         0E 94
         98 00
         EB 01
         0E C0
         0E 94
         98 00
         6C 1B
         7D 0B
         68 5E
         73 40
         C8 F3
         08 94
         E1 08
         F1 08
         01 09
         11 09
         C8 51
         DC 4F
         E1 14
         F1 04
         01 05
         11 05
         69 F7
         DF 91
         CF 91
         1F 91
         0F 91
         FF 90
         EF 90
         08 95
         78 94
         84 B5
         82 60
         84 BD
         84 B5
         81 60
         84 BD
         85 B5
         82 60
         85 BD
         85 B5
         81 60
         85 BD
         EE E6
         F0 E0
         80 81
         81 60
         80 83
         E1 E8
         F0 E0
         10 82
         80 81
         82 60
         80 83
         80 81
         81 60
         80 83
         E0 E8
         F0 E0
         80 81
         81 60
         80 83
         E1 EB
         F0 E0
         80 81
         84 60
         80 83
         E0 EB
         F0 E0
         80 81
         81 60
         80 83
         EA E7
         F0 E0
         80 81
         84 60
         80 83
         80 81
         82 60
         80 83
         80 81
         81 60
         80 83
         80 81
         80 68
         80 83
         10 92
         C1 00
         08 95
         0E 94
         E4 00
         0E 94
         48 00
         0E 94
         49 00
         */
    }
}