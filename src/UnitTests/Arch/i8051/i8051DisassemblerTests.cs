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
using Reko.Arch.i8051;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.i8051
{
    [TestFixture]
    public class i8051DisassemblerTests : DisassemblerTestBase<i8051Instruction>
    {
        private i8051Architecture arch;

        public i8051DisassemblerTests()
        {
            this.arch = new i8051Architecture("8051");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr16(0); }
        }

        [Test]
        public void I8051_dis_nop()
        {
            var instr = DisassembleBytes(0);
            Assert.AreEqual("nop", instr.ToString());
        }

        [Test]
        public void I8051_dis_ajmp()
        {
            var instr = DisassembleBytes(0xE1, 0xFF);
            Assert.AreEqual("ajmp\t07FF", instr.ToString());
        }

        [Test]
        public void I8051_dis_ljmp()
        {
            var instr = DisassembleBytes(0x02, 0x12, 0x34);
            Assert.AreEqual("ljmp\t1234", instr.ToString());
        }

        [Test]
        public void I8051_dis_jbc()
        {
            var instr = DisassembleBytes(0x10, 0x82, 0x0D);
            Assert.AreEqual("jbc\tP0.2,0010", instr.ToString());
        }

        [Test]
        public void I8051_dis_inc()
        {
            var instr = DisassembleBytes(0x08);
            Assert.AreEqual("inc\tR0", instr.ToString());

            instr = DisassembleBytes(0x04);
            Assert.AreEqual("inc\tA", instr.ToString());

            instr = DisassembleBytes(0x05, 0x88);
            Assert.AreEqual("inc\t[0088]", instr.ToString());
        }

        [Test]
        public void I8051_dis_add()
        {
            var instr = DisassembleBytes(0x24, 02);
            Assert.AreEqual("add\tA,02", instr.ToString());

            instr = DisassembleBytes(0x25, 0x25);
            Assert.AreEqual("add\tA,[0025]", instr.ToString());

            instr = DisassembleBytes(0x27);
            Assert.AreEqual("add\tA,@R1", instr.ToString());

            instr = DisassembleBytes(0x2B);
            Assert.AreEqual("add\tA,R3", instr.ToString());
        }

        [Test]
        public void I8051_dis_anl()
        {
            var instr = DisassembleBytes(0xB0, 0x93);
            Assert.AreEqual("anl\tC,/P1.3", instr.ToString());

            instr = DisassembleBytes(0x56);
            Assert.AreEqual("anl\tA,@R0", instr.ToString());
        }

        [Test]
        public void I8051_dis_mov()
        {
            var instr = DisassembleBytes(0x75, 0x42, 0x1);
            Assert.AreEqual("mov\t[0042],01", instr.ToString());

            instr = DisassembleBytes(0x79, 0x42);
            Assert.AreEqual("mov\tR1,42", instr.ToString());

            instr = DisassembleBytes(0xA2, 0x84);
            Assert.AreEqual("mov\tC,P0.4", instr.ToString());

            instr = DisassembleBytes(0x85, 0x90, 0x80);
            Assert.AreEqual("mov\t[0090],[0080]", instr.ToString());

            instr = DisassembleBytes(0x8A, 0x42);
            Assert.AreEqual("mov\t[0042],R2", instr.ToString());

            instr = DisassembleBytes(0xAA, 0x42);
            Assert.AreEqual("mov\tR2,[0042]", instr.ToString());

            instr = DisassembleBytes(0xAF, 0x42);
            Assert.AreEqual("mov\tR7,[0042]", instr.ToString());

            instr = DisassembleBytes(0xE5, 0x42);
            Assert.AreEqual("mov\tA,[0042]", instr.ToString());

            instr = DisassembleBytes(0xEC, 0x42);
            Assert.AreEqual("mov\tA,R4", instr.ToString());

            instr = DisassembleBytes(0xF5, 0x42);
            Assert.AreEqual("mov\t[0042],A", instr.ToString());

            instr = DisassembleBytes(0xFF);
            Assert.AreEqual("mov\tR7,A", instr.ToString());
        }

        [Test]
        public void I8051_dis_sjmp()
        {
            var instr = DisassembleBytes(0x80, 0xFE);
            Assert.AreEqual("sjmp\t0000", instr.ToString());
        }


        [Test]
        public void I8051_dis_clr()
        {
            var instr = DisassembleBytes(0xC2, 0x87);
            Assert.AreEqual("clr\tP0.7", instr.ToString());

            instr = DisassembleBytes(0xC3);
            Assert.AreEqual("clr\tC", instr.ToString());
        }


        [Test]
        public void I8051_dis_reti()
        {
            var instr = DisassembleBytes(0x32);
            Assert.AreEqual("reti", instr.ToString());
        }

        [Test]
        public void I8051_dis_pop()
        {
            var instr = DisassembleBytes(0xD0, 0x82);
            Assert.AreEqual("pop\t[0082]", instr.ToString());
        }

        [Test]
        public void I8051_dis_movx()
        {
            var instr = DisassembleBytes(0xE0);
            Assert.AreEqual("movx\tA,@DPTR", instr.ToString());
        }

        [Test]
        public void I8051_dis_xrl()
        {
            var instr = DisassembleBytes(0x67);
            Assert.AreEqual("xrl\tA,@R1", instr.ToString());
        }

        [Test]
        public void I8051_dis_addc()
        {
            var instr = DisassembleBytes(0x34, 0x00);
            Assert.AreEqual("addc\tA,00", instr.ToString());
        }

        [Test]
        public void I8051_dis_setb()
        {
            var instr = DisassembleBytes(0xD2, 0x96);
            Assert.AreEqual("setb\tP1.6", instr.ToString());
        }

        [Test]
        public void I8051_dis_cjne()
        {
            var instr = DisassembleBytes(0xBC, 0x09, 0x03);
            Assert.AreEqual("cjne\tR4,09,0006", instr.ToString());
        }

        [Test]
        public void I8051_dis_subb()
        {
            var instr = DisassembleBytes(0x95, 0x42);
            Assert.AreEqual("subb\tA,[0042]", instr.ToString());
        }

        [Test]
        public void I8051_dis_jc()
        {
            var instr = DisassembleBytes(0x40, 0x0E);
            Assert.AreEqual("jc\t0010", instr.ToString());
        }

        [Test]
        public void I8051_dis_xch()
        {
            var instr = DisassembleBytes(0xCE);
            Assert.AreEqual("xch\tA,R6", instr.ToString());
        }

        [Test]
        public void I8051_dis_jnz()
        {
            var instr = DisassembleBytes(0x70, 0x2E);
            Assert.AreEqual("jnz\t0030", instr.ToString());
        }

        [Test]
        public void I8051_dis_jnb()
        {
            var instr = DisassembleBytes(0x30, 0x90, 0x3D);
            Assert.AreEqual("jnb\tP1.0,0040", instr.ToString());
        }

        [Test]
        public void I8051_dis_orl()
        {
            var instr = DisassembleBytes(0x44, 0x42);
            Assert.AreEqual("orl\tA,42", instr.ToString());
        }

        [Test]
        public void I8051_dis_dec()
        {
            var instr = DisassembleBytes(0x18);
            Assert.AreEqual("dec\tR0", instr.ToString());
        }

        [Test]
        public void I8051_dis_djnz()
        {
            var instr = DisassembleBytes(0xD9, 0x08);
            Assert.AreEqual("djnz\tR1,000A", instr.ToString());
        }

        [Test]
        public void I8051_dis_ret()
        {
            var instr = DisassembleBytes(0x22);
            Assert.AreEqual("ret", instr.ToString());
        }

        [Test]
        public void I8051_dis_rl()
        {
            var instr = DisassembleBytes(0x23);
            Assert.AreEqual("rl\tA", instr.ToString());
        }

        [Test]
        public void I8051_dis_jnc()
        {
            var instr = DisassembleBytes(0x50, 0x1E);
            Assert.AreEqual("jnc\t0020", instr.ToString());
        }

        [Test]
        public void I8051_dis_jb()
        {
            var instr = DisassembleBytes(0x20, 0x85, 0x6D);
            Assert.AreEqual("jb\tP0.5,0070", instr.ToString());
        }

        [Test]
        public void I8051_dis_jz()
        {
            var instr = DisassembleBytes(0x60, 0x6E);
            Assert.AreEqual("jz\t0070", instr.ToString());
        }

        [Test]
        public void I8051_dis_push()
        {
            var instr = DisassembleBytes(0xC0, 0x90);
            Assert.AreEqual("push\t[0090]", instr.ToString());
        }

        [Test]
        public void I8051_dis_mov_dptr()
        {
            var instr = DisassembleBytes(0x90, 0x12, 0x34);
            Assert.AreEqual("mov\tDPTR,1234", instr.ToString());
        }

        [Test]
        public void I8051_dis_movc()
        {
            var instr = DisassembleBytes(0x93);
            Assert.AreEqual("movc\t@DPTR+A", instr.ToString());
        }

        [Test]
        public void I8051_dis_movc_pcrel()
        {
            var instr = DisassembleBytes(0x83);
            Assert.AreEqual("movc\t@PC+A", instr.ToString());
        }

        [Test]
        public void I8051_dis_mul()
        {
            var instr = DisassembleBytes(0xA4);
            Assert.AreEqual("mul\tAB", instr.ToString());
        }

        [Test]
        public void I8051_dis_jmp()
        {
            var instr = DisassembleBytes(0x73);
            Assert.AreEqual("jmp\t@DPTR+A", instr.ToString());
        }

        [Test]
        public void I8051_dis_swap()
        {
            var instr = DisassembleBytes(0xC4);
            Assert.AreEqual("swap\tA", instr.ToString());
        }
    }
}