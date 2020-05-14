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
using Reko.Arch.Avr.Avr32;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr32DisassemblerTests : DisassemblerTestBase<Avr32Instruction>
    {
        private readonly Avr32Architecture arch;

        public Avr32DisassemblerTests()
        {
            this.arch = new Avr32Architecture("avr32");
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Avr32Dis_mov_r_imm()
        {
            AssertCode("mov\tr7,00000000", "3007");
        }

        [Test]
        public void Avr32Dis_ld_w_post()
        {
            AssertCode("ld.w\tr11,sp++", "1B0B");
        }

        [Test]
        public void Avr32Dis_mov_tworegs()
        {
            AssertCode("mov\tr10,sp", "1A9A");
        }

        [Test]
        public void Avr32Dis_st_w_pre()
        {
            AssertCode("st.w\t--sp,r10", "1ADA");
        }

        [Test]
        public void Avr32Dis_lddpc()
        {
            AssertCode("lddpc\tr6,pc[20]", "4856");
        }

        [Test]
        public void Avr32Dis_lddpc_to_pc()
        {
            var instr = DisassembleHexBytes("485F");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("lddpc\tpc,pc[20]", instr.ToString());
        }

        [Test]
        public void Avr32Dis_rsub()
        {
            AssertCode("rsub\tr6,pc", "1E26");
        }

        [Test]
        public void Avr32Dis_sub3_imm()
        {
            AssertCode("sub\tr9,pc,0000002C", "FEC9002C");
        }

        [Test]
        public void Avr32Dis_ECF8()
        {
            AssertCode("ld.w\tr8,r8[456]", "ECF801C8");
        }

        [Test]
        public void Avr32Dis_mcall()
        {
            AssertCode("mcall\tr6[480]", "F0160078");
        }

        // Reko: a decoder for Avr32 instruction D421 at address 000026C0 has not been implemented. (0b110)
        [Test]
        public void Avr32Dis_D421()
        {
            AssertCode("@@@", "D421");
        }

        // Reko: a decoder for Avr32 instruction 48D6 at address 000026C2 has not been implemented. (0b010)
        [Test]
        public void Avr32Dis_48D6()
        {
            AssertCode("@@@", "48D6");
        }

        // Reko: a decoder for Avr32 instruction 1189 at address 000026CA has not been implemented. (0b000)
        [Test]
        public void Avr32Dis_1189()
        {
            AssertCode("@@@", "1189");
        }

        // Reko: a decoder for Avr32 instruction F009 at address 000026CE has not been implemented. (0b111)
        [Test]
        public void Avr32Dis_F009()
        {
            AssertCode("@@@", "F0091800");
        }

        // Reko: a decoder for Avr32 instruction C040 at address 000026D2 has not been implemented. (0b110)
        [Test]
        public void Avr32Dis_C040()
        {
            AssertCode("@@@", "C040");
        }


        /*

000026A8 4856 invalid 
000026AA 1E26 invalid 
000026AC FEC9 invalid 
000026AE 002C invalid 
000026B0 ECF8 invalid 
000026B2 01C8 invalid 
000026B4 FECC 
// Reko: a decoder for Avr32 instruction F7E9 at address 0000F4B4 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F7E9()
{
AssertCode("@@@", "F7E9");
}

// Reko: a decoder for Avr32 instruction F7E9 at address 0000F4B4 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F7E9()
{
AssertCode("@@@", "F7E9");
}

// Reko: a decoder for Avr32 instruction F8CB at address 0000F7FA has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8CB()
{
AssertCode("@@@", "F8CB");
}

// Reko: a decoder for Avr32 instruction F8CB at address 0000F7FA has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8CB()
{
AssertCode("@@@", "F8CB");
}

// Reko: a decoder for Avr32 instruction D431 at address 000104EA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D431()
{
AssertCode("@@@", "D431");
}

// Reko: a decoder for Avr32 instruction D431 at address 000104EA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D431()
{
AssertCode("@@@", "D431");
}

// Reko: a decoder for Avr32 instruction EBCD at address 0000FA54 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EBCD()
{
AssertCode("@@@", "EBCD");
}

// Reko: a decoder for Avr32 instruction EBCD at address 0000FA54 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EBCD()
{
AssertCode("@@@", "EBCD");
}

// Reko: a decoder for Avr32 instruction A17B at address 0000F976 has not been implemented. (0b101)
[Test]
public void Avr32Dis_A17B()
{
AssertCode("@@@", "A17B");
}

// Reko: a decoder for Avr32 instruction A17B at address 0000F976 has not been implemented. (0b101)
[Test]
public void Avr32Dis_A17B()
{
AssertCode("@@@", "A17B");
}

// Reko: a decoder for Avr32 instruction 103A at address 0000F8EC has not been implemented. (0b000)
[Test]
public void Avr32Dis_103A()
{
AssertCode("@@@", "103A");
}

// Reko: a decoder for Avr32 instruction 103A at address 0000F8EC has not been implemented. (0b000)
[Test]
public void Avr32Dis_103A()
{
AssertCode("@@@", "103A");
}

// Reko: a decoder for Avr32 instruction 189B at address 0000F802 has not been implemented. (0b000)
[Test]
public void Avr32Dis_189B()
{
AssertCode("@@@", "189B");
}

// Reko: a decoder for Avr32 instruction 189B at address 0000F802 has not been implemented. (0b000)
[Test]
public void Avr32Dis_189B()
{
AssertCode("@@@", "189B");
}

// Reko: a decoder for Avr32 instruction F60C at address 0000F764 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F60C()
{
AssertCode("@@@", "F60C");
}

// Reko: a decoder for Avr32 instruction F60C at address 0000F764 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F60C()
{
AssertCode("@@@", "F60C");
}

// Reko: a decoder for Avr32 instruction 580B at address 0000F760 has not been implemented. (0b010)
[Test]
public void Avr32Dis_580B()
{
AssertCode("@@@", "580B");
}

// Reko: a decoder for Avr32 instruction 580B at address 0000F760 has not been implemented. (0b010)
[Test]
public void Avr32Dis_580B()
{
AssertCode("@@@", "580B");
}

// Reko: a decoder for Avr32 instruction F5EB at address 0000F2D0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F5EB()
{
AssertCode("@@@", "F5EB");
}

// Reko: a decoder for Avr32 instruction F5EB at address 0000F2D0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F5EB()
{
AssertCode("@@@", "F5EB");
}

// Reko: a decoder for Avr32 instruction 300E at address 0000269E has not been implemented. (0b001)
[Test]
public void Avr32Dis_300E()
{
AssertCode("@@@", "300E");
}

// Reko: a decoder for Avr32 instruction 300E at address 0000269E has not been implemented. (0b001)
[Test]
public void Avr32Dis_300E()
{
AssertCode("@@@", "300E");
}



// Reko: a decoder for Avr32 instruction 1B0B at address 000026A0 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1B0B()
{
AssertCode("@@@", "1B0B");
}



// Reko: a decoder for Avr32 instruction 1ADC at address 000026A6 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1ADC()
{
AssertCode("@@@", "1ADC");
}

// Reko: a decoder for Avr32 instruction 1ADC at address 000026A6 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1ADC()
{
AssertCode("@@@", "1ADC");
}

// Reko: a decoder for Avr32 instruction 4856 at address 000026A8 has not been implemented. (0b010)


// Reko: a decoder for Avr32 instruction 4856 at address 000026A8 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4856()
{
AssertCode("@@@", "4856");
}



// Reko: a decoder for Avr32 instruction 1E26 at address 000026AA has not been implemented. (0b000)
[Test]
public void Avr32Dis_1E26()
{
AssertCode("@@@", "1E26");
}

// Reko: a decoder for Avr32 instruction FEC9 at address 000026AC has not been implemented. (0b111)
[Test]
public void Avr32Dis_FEC9()
{
AssertCode("@@@", "FEC9");
}



// Reko: a decoder for Avr32 instruction 002C at address 000026AE has not been implemented. (0b000)
[Test]
public void Avr32Dis_002C()
{
AssertCode("@@@", "002C");
}

// Reko: a decoder for Avr32 instruction ECF8 at address 000026B0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECF8()
{
AssertCode("@@@", "ECF8");
}

// Reko: a decoder for Avr32 instruction ECF8 at address 000026B0 has not been implemented. (0b111)


// Reko: a decoder for Avr32 instruction 01C8 at address 000026B2 has not been implemented. (0b000)
[Test]
public void Avr32Dis_01C8()
{
AssertCode("@@@", "01C8");
}

// Reko: a decoder for Avr32 instruction FECC at address 000026B4 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FECC()
{
AssertCode("@@@", "FECC");
}

// Reko: a decoder for Avr32 instruction FECC at address 000026B4 has not been implemented. (0b111)


// Reko: a decoder for Avr32 instruction D140 at address 000026B6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D140()
{
AssertCode("@@@", "D140");
}

// Reko: a decoder for Avr32 instruction F016 at address 000026B8 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F016()
{
AssertCode("@@@", "F016");
}



// Reko: a decoder for Avr32 instruction 0078 at address 000026BA has not been implemented. (0b000)
[Test]
public void Avr32Dis_0078()
{
AssertCode("@@@", "0078");
}

// Reko: a decoder for Avr32 instruction FFFE at address 000026BC has not been implemented. (0b111)
[Test]
public void Avr32Dis_FFFE()
{
AssertCode("@@@", "FFFE");
}

// Reko: a decoder for Avr32 instruction FFFE at address 000026BC has not been implemented. (0b111)
[Test]
public void Avr32Dis_FFFE()
{
AssertCode("@@@", "FFFE");
}

// Reko: a decoder for Avr32 instruction D6BA at address 000026BE has not been implemented. (0b110)
[Test]
public void Avr32Dis_D6BA()
{
AssertCode("@@@", "D6BA");
}

// Reko: a decoder for Avr32 instruction D6BA at address 000026BE has not been implemented. (0b110)
[Test]
public void Avr32Dis_D6BA()
{
AssertCode("@@@", "D6BA");
}

// Reko: a decoder for Avr32 instruction D421 at address 000026C0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D421()
{
AssertCode("@@@", "D421");
}
$$$$$$$$$$$$$$$$$$$$$$$$$$$



// Reko: a decoder for Avr32 instruction 0184 at address 000026C8 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0184()
{
AssertCode("@@@", "0184");
}

// Reko: a decoder for Avr32 instruction 0184 at address 000026C8 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0184()
{
AssertCode("@@@", "0184");
}



// Reko: a decoder for Avr32 instruction 1189 at address 000026CA has not been implemented. (0b000)
[Test]
public void Avr32Dis_1189()
{
AssertCode("@@@", "1189");
}

// Reko: a decoder for Avr32 instruction 3008 at address 000026CC has not been implemented. (0b001)
[Test]
public void Avr32Dis_3008()
{
AssertCode("@@@", "3008");
}

// Reko: a decoder for Avr32 instruction 3008 at address 000026CC has not been implemented. (0b001)
[Test]
public void Avr32Dis_3008()
{
AssertCode("@@@", "3008");
}

// Reko: a decoder for Avr32 instruction F009 at address 000026CE has not been implemented. (0b111)
[Test]
public void Avr32Dis_F009()
{
AssertCode("@@@", "F009");
}



// Reko: a decoder for Avr32 instruction 1800 at address 000026D0 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1800()
{
AssertCode("@@@", "1800");
}


// Reko: a decoder for Avr32 instruction C040 at address 000026D2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C040()
{
AssertCode("@@@", "C040");
}

// Reko: a decoder for Avr32 instruction D822 at address 000026D4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D822()
{
AssertCode("@@@", "D822");
}

// Reko: a decoder for Avr32 instruction D822 at address 000026D4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D822()
{
AssertCode("@@@", "D822");
}

// Reko: a decoder for Avr32 instruction 9509 at address 000026D6 has not been implemented. (0b100)
[Test]
public void Avr32Dis_9509()
{
AssertCode("@@@", "9509");
}

// Reko: a decoder for Avr32 instruction 9509 at address 000026D6 has not been implemented. (0b100)
[Test]
public void Avr32Dis_9509()
{
AssertCode("@@@", "9509");
}

// Reko: a decoder for Avr32 instruction 5D18 at address 000026D8 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D18()
{
AssertCode("@@@", "5D18");
}

// Reko: a decoder for Avr32 instruction 5D18 at address 000026D8 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D18()
{
AssertCode("@@@", "5D18");
}

// Reko: a decoder for Avr32 instruction ECFA at address 000026DA has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFA()
{
AssertCode("@@@", "ECFA");
}

// Reko: a decoder for Avr32 instruction ECFA at address 000026DA has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFA()
{
AssertCode("@@@", "ECFA");
}

// Reko: a decoder for Avr32 instruction 051C at address 000026DC has not been implemented. (0b000)
[Test]
public void Avr32Dis_051C()
{
AssertCode("@@@", "051C");
}

// Reko: a decoder for Avr32 instruction 051C at address 000026DC has not been implemented. (0b000)
[Test]
public void Avr32Dis_051C()
{
AssertCode("@@@", "051C");
}

// Reko: a decoder for Avr32 instruction 7408 at address 000026DE has not been implemented. (0b011)
[Test]
public void Avr32Dis_7408()
{
AssertCode("@@@", "7408");
}

// Reko: a decoder for Avr32 instruction 7408 at address 000026DE has not been implemented. (0b011)
[Test]
public void Avr32Dis_7408()
{
AssertCode("@@@", "7408");
}

// Reko: a decoder for Avr32 instruction F0C9 at address 000026E0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F0C9()
{
AssertCode("@@@", "F0C9");
}

// Reko: a decoder for Avr32 instruction F0C9 at address 000026E0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F0C9()
{
AssertCode("@@@", "F0C9");
}

// Reko: a decoder for Avr32 instruction FFFC at address 000026E2 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FFFC()
{
AssertCode("@@@", "FFFC");
}

// Reko: a decoder for Avr32 instruction FFFC at address 000026E2 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FFFC()
{
AssertCode("@@@", "FFFC");
}

// Reko: a decoder for Avr32 instruction 7008 at address 000026E4 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7008()
{
AssertCode("@@@", "7008");
}

// Reko: a decoder for Avr32 instruction 7008 at address 000026E4 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7008()
{
AssertCode("@@@", "7008");
}

// Reko: a decoder for Avr32 instruction 5808 at address 000026E6 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5808()
{
AssertCode("@@@", "5808");
}

// Reko: a decoder for Avr32 instruction 5808 at address 000026E6 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5808()
{
AssertCode("@@@", "5808");
}

// Reko: a decoder for Avr32 instruction CF71 at address 000026E8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CF71()
{
AssertCode("@@@", "CF71");
}

// Reko: a decoder for Avr32 instruction CF71 at address 000026E8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CF71()
{
AssertCode("@@@", "CF71");
}

// Reko: a decoder for Avr32 instruction 3019 at address 000026EE has not been implemented. (0b001)
[Test]
public void Avr32Dis_3019()
{
AssertCode("@@@", "3019");
}

// Reko: a decoder for Avr32 instruction 3019 at address 000026EE has not been implemented. (0b001)
[Test]
public void Avr32Dis_3019()
{
AssertCode("@@@", "3019");
}

// Reko: a decoder for Avr32 instruction B089 at address 000026F0 has not been implemented. (0b101)
[Test]
public void Avr32Dis_B089()
{
AssertCode("@@@", "B089");
}

// Reko: a decoder for Avr32 instruction B089 at address 000026F0 has not been implemented. (0b101)
[Test]
public void Avr32Dis_B089()
{
AssertCode("@@@", "B089");
}

// Reko: a decoder for Avr32 instruction D6D4 at address 000026F6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D6D4()
{
AssertCode("@@@", "D6D4");
}

// Reko: a decoder for Avr32 instruction D6D4 at address 000026F6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D6D4()
{
AssertCode("@@@", "D6D4");
}

// Reko: a decoder for Avr32 instruction 4876 at address 000026FA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4876()
{
AssertCode("@@@", "4876");
}

// Reko: a decoder for Avr32 instruction 4876 at address 000026FA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4876()
{
AssertCode("@@@", "4876");
}

// Reko: a decoder for Avr32 instruction ECFC at address 000026FE has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFC()
{
AssertCode("@@@", "ECFC");
}

// Reko: a decoder for Avr32 instruction ECFC at address 000026FE has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFC()
{
AssertCode("@@@", "ECFC");
}

// Reko: a decoder for Avr32 instruction 0520 at address 00002700 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0520()
{
AssertCode("@@@", "0520");
}

// Reko: a decoder for Avr32 instruction 0520 at address 00002700 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0520()
{
AssertCode("@@@", "0520");
}

// Reko: a decoder for Avr32 instruction 7808 at address 00002702 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7808()
{
AssertCode("@@@", "7808");
}

// Reko: a decoder for Avr32 instruction 7808 at address 00002702 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7808()
{
AssertCode("@@@", "7808");
}

// Reko: a decoder for Avr32 instruction C060 at address 00002706 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C060()
{
AssertCode("@@@", "C060");
}

// Reko: a decoder for Avr32 instruction C060 at address 00002706 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C060()
{
AssertCode("@@@", "C060");
}

// Reko: a decoder for Avr32 instruction 0190 at address 0000270A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0190()
{
AssertCode("@@@", "0190");
}

// Reko: a decoder for Avr32 instruction 0190 at address 0000270A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0190()
{
AssertCode("@@@", "0190");
}

// Reko: a decoder for Avr32 instruction C020 at address 0000270E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C020()
{
AssertCode("@@@", "C020");
}

// Reko: a decoder for Avr32 instruction C020 at address 0000270E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C020()
{
AssertCode("@@@", "C020");
}

// Reko: a decoder for Avr32 instruction D70C at address 00002716 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D70C()
{
AssertCode("@@@", "D70C");
}

// Reko: a decoder for Avr32 instruction D70C at address 00002716 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D70C()
{
AssertCode("@@@", "D70C");
}

// Reko: a decoder for Avr32 instruction A98A at address 00002718 has not been implemented. (0b101)
[Test]
public void Avr32Dis_A98A()
{
AssertCode("@@@", "A98A");
}

// Reko: a decoder for Avr32 instruction A98A at address 00002718 has not been implemented. (0b101)
[Test]
public void Avr32Dis_A98A()
{
AssertCode("@@@", "A98A");
}

// Reko: a decoder for Avr32 instruction 1698 at address 0000271A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1698()
{
AssertCode("@@@", "1698");
}

// Reko: a decoder for Avr32 instruction 1698 at address 0000271A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1698()
{
AssertCode("@@@", "1698");
}

// Reko: a decoder for Avr32 instruction 118A at address 0000271E has not been implemented. (0b000)
[Test]
public void Avr32Dis_118A()
{
AssertCode("@@@", "118A");
}

// Reko: a decoder for Avr32 instruction 118A at address 0000271E has not been implemented. (0b000)
[Test]
public void Avr32Dis_118A()
{
AssertCode("@@@", "118A");
}

// Reko: a decoder for Avr32 instruction E018 at address 00002720 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E018()
{
AssertCode("@@@", "E018");
}

// Reko: a decoder for Avr32 instruction E018 at address 00002720 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E018()
{
AssertCode("@@@", "E018");
}

// Reko: a decoder for Avr32 instruction F000 at address 00002722 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F000()
{
AssertCode("@@@", "F000");
}

// Reko: a decoder for Avr32 instruction F000 at address 00002722 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F000()
{
AssertCode("@@@", "F000");
}

// Reko: a decoder for Avr32 instruction F9DA at address 00002724 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F9DA()
{
AssertCode("@@@", "F9DA");
}

// Reko: a decoder for Avr32 instruction F9DA at address 00002724 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F9DA()
{
AssertCode("@@@", "F9DA");
}

// Reko: a decoder for Avr32 instruction C00C at address 00002726 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C00C()
{
AssertCode("@@@", "C00C");
}

// Reko: a decoder for Avr32 instruction C00C at address 00002726 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C00C()
{
AssertCode("@@@", "C00C");
}

// Reko: a decoder for Avr32 instruction F1EC at address 00002728 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F1EC()
{
AssertCode("@@@", "F1EC");
}

// Reko: a decoder for Avr32 instruction F1EC at address 00002728 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F1EC()
{
AssertCode("@@@", "F1EC");
}

// Reko: a decoder for Avr32 instruction 100C at address 0000272A has not been implemented. (0b000)
[Test]
public void Avr32Dis_100C()
{
AssertCode("@@@", "100C");
}

// Reko: a decoder for Avr32 instruction 100C at address 0000272A has not been implemented. (0b000)
[Test]
public void Avr32Dis_100C()
{
AssertCode("@@@", "100C");
}

// Reko: a decoder for Avr32 instruction 5EFC at address 0000272C has not been implemented. (0b010)
[Test]
public void Avr32Dis_5EFC()
{
AssertCode("@@@", "5EFC");
}

// Reko: a decoder for Avr32 instruction 5EFC at address 0000272C has not been implemented. (0b010)
[Test]
public void Avr32Dis_5EFC()
{
AssertCode("@@@", "5EFC");
}

// Reko: a decoder for Avr32 instruction F408 at address 0000272E has not been implemented. (0b111)
[Test]
public void Avr32Dis_F408()
{
AssertCode("@@@", "F408");
}

// Reko: a decoder for Avr32 instruction F408 at address 0000272E has not been implemented. (0b111)
[Test]
public void Avr32Dis_F408()
{
AssertCode("@@@", "F408");
}

// Reko: a decoder for Avr32 instruction 160C at address 00002730 has not been implemented. (0b000)
[Test]
public void Avr32Dis_160C()
{
AssertCode("@@@", "160C");
}

// Reko: a decoder for Avr32 instruction 160C at address 00002730 has not been implemented. (0b000)
[Test]
public void Avr32Dis_160C()
{
AssertCode("@@@", "160C");
}

// Reko: a decoder for Avr32 instruction F1EB at address 00002732 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F1EB()
{
AssertCode("@@@", "F1EB");
}

// Reko: a decoder for Avr32 instruction F1EB at address 00002732 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F1EB()
{
AssertCode("@@@", "F1EB");
}

// Reko: a decoder for Avr32 instruction 1148 at address 00002734 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1148()
{
AssertCode("@@@", "1148");
}

// Reko: a decoder for Avr32 instruction 1148 at address 00002734 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1148()
{
AssertCode("@@@", "1148");
}

// Reko: a decoder for Avr32 instruction FF00 at address 00002738 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FF00()
{
AssertCode("@@@", "FF00");
}

// Reko: a decoder for Avr32 instruction FF00 at address 00002738 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FF00()
{
AssertCode("@@@", "FF00");
}

// Reko: a decoder for Avr32 instruction C008 at address 0000273C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C008()
{
AssertCode("@@@", "C008");
}

// Reko: a decoder for Avr32 instruction C008 at address 0000273C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C008()
{
AssertCode("@@@", "C008");
}

// Reko: a decoder for Avr32 instruction D401 at address 00002744 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D401()
{
AssertCode("@@@", "D401");
}

// Reko: a decoder for Avr32 instruction D401 at address 00002744 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D401()
{
AssertCode("@@@", "D401");
}

// Reko: a decoder for Avr32 instruction 169E at address 00002746 has not been implemented. (0b000)
[Test]
public void Avr32Dis_169E()
{
AssertCode("@@@", "169E");
}

// Reko: a decoder for Avr32 instruction 169E at address 00002746 has not been implemented. (0b000)
[Test]
public void Avr32Dis_169E()
{
AssertCode("@@@", "169E");
}

// Reko: a decoder for Avr32 instruction 123E at address 00002748 has not been implemented. (0b000)
[Test]
public void Avr32Dis_123E()
{
AssertCode("@@@", "123E");
}

// Reko: a decoder for Avr32 instruction 123E at address 00002748 has not been implemented. (0b000)
[Test]
public void Avr32Dis_123E()
{
AssertCode("@@@", "123E");
}

// Reko: a decoder for Avr32 instruction C024 at address 0000274A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C024()
{
AssertCode("@@@", "C024");
}

// Reko: a decoder for Avr32 instruction C024 at address 0000274A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C024()
{
AssertCode("@@@", "C024");
}

// Reko: a decoder for Avr32 instruction DC0A at address 0000274C has not been implemented. (0b110)
[Test]
public void Avr32Dis_DC0A()
{
AssertCode("@@@", "DC0A");
}

// Reko: a decoder for Avr32 instruction DC0A at address 0000274C has not been implemented. (0b110)
[Test]
public void Avr32Dis_DC0A()
{
AssertCode("@@@", "DC0A");
}

// Reko: a decoder for Avr32 instruction 0108 at address 00002750 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0108()
{
AssertCode("@@@", "0108");
}

// Reko: a decoder for Avr32 instruction 0108 at address 00002750 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0108()
{
AssertCode("@@@", "0108");
}

// Reko: a decoder for Avr32 instruction F00C at address 00002754 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F00C()
{
AssertCode("@@@", "F00C");
}

// Reko: a decoder for Avr32 instruction F00C at address 00002754 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F00C()
{
AssertCode("@@@", "F00C");
}

// Reko: a decoder for Avr32 instruction 17A0 at address 00002756 has not been implemented. (0b000)
[Test]
public void Avr32Dis_17A0()
{
AssertCode("@@@", "17A0");
}

// Reko: a decoder for Avr32 instruction 17A0 at address 00002756 has not been implemented. (0b000)
[Test]
public void Avr32Dis_17A0()
{
AssertCode("@@@", "17A0");
}

// Reko: a decoder for Avr32 instruction F9BC at address 00002758 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F9BC()
{
AssertCode("@@@", "F9BC");
}

// Reko: a decoder for Avr32 instruction F9BC at address 00002758 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F9BC()
{
AssertCode("@@@", "F9BC");
}

// Reko: a decoder for Avr32 instruction 0901 at address 0000275A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0901()
{
AssertCode("@@@", "0901");
}

// Reko: a decoder for Avr32 instruction 0901 at address 0000275A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0901()
{
AssertCode("@@@", "0901");
}

// Reko: a decoder for Avr32 instruction D802 at address 0000275C has not been implemented. (0b110)
[Test]
public void Avr32Dis_D802()
{
AssertCode("@@@", "D802");
}

// Reko: a decoder for Avr32 instruction D802 at address 0000275C has not been implemented. (0b110)
[Test]
public void Avr32Dis_D802()
{
AssertCode("@@@", "D802");
}

// Reko: a decoder for Avr32 instruction 4866 at address 00002760 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4866()
{
AssertCode("@@@", "4866");
}

// Reko: a decoder for Avr32 instruction 4866 at address 00002760 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4866()
{
AssertCode("@@@", "4866");
}

// Reko: a decoder for Avr32 instruction 1699 at address 00002764 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1699()
{
AssertCode("@@@", "1699");
}

// Reko: a decoder for Avr32 instruction 1699 at address 00002764 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1699()
{
AssertCode("@@@", "1699");
}

// Reko: a decoder for Avr32 instruction F8EA at address 00002766 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8EA()
{
AssertCode("@@@", "F8EA");
}

// Reko: a decoder for Avr32 instruction F8EA at address 00002766 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8EA()
{
AssertCode("@@@", "F8EA");
}

// Reko: a decoder for Avr32 instruction 1298 at address 0000276A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1298()
{
AssertCode("@@@", "1298");
}

// Reko: a decoder for Avr32 instruction 1298 at address 0000276A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1298()
{
AssertCode("@@@", "1298");
}

// Reko: a decoder for Avr32 instruction 3009 at address 0000276C has not been implemented. (0b001)
[Test]
public void Avr32Dis_3009()
{
AssertCode("@@@", "3009");
}

// Reko: a decoder for Avr32 instruction 3009 at address 0000276C has not been implemented. (0b001)
[Test]
public void Avr32Dis_3009()
{
AssertCode("@@@", "3009");
}

// Reko: a decoder for Avr32 instruction E0A0 at address 0000276E has not been implemented. (0b111)
[Test]
public void Avr32Dis_E0A0()
{
AssertCode("@@@", "E0A0");
}

// Reko: a decoder for Avr32 instruction E0A0 at address 0000276E has not been implemented. (0b111)
[Test]
public void Avr32Dis_E0A0()
{
AssertCode("@@@", "E0A0");
}

// Reko: a decoder for Avr32 instruction 716F at address 00002770 has not been implemented. (0b011)
[Test]
public void Avr32Dis_716F()
{
AssertCode("@@@", "716F");
}

// Reko: a decoder for Avr32 instruction 716F at address 00002770 has not been implemented. (0b011)
[Test]
public void Avr32Dis_716F()
{
AssertCode("@@@", "716F");
}

// Reko: a decoder for Avr32 instruction 149C at address 00002772 has not been implemented. (0b000)
[Test]
public void Avr32Dis_149C()
{
AssertCode("@@@", "149C");
}

// Reko: a decoder for Avr32 instruction 149C at address 00002772 has not been implemented. (0b000)
[Test]
public void Avr32Dis_149C()
{
AssertCode("@@@", "149C");
}

// Reko: a decoder for Avr32 instruction D703 at address 00002776 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D703()
{
AssertCode("@@@", "D703");
}

// Reko: a decoder for Avr32 instruction D703 at address 00002776 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D703()
{
AssertCode("@@@", "D703");
}

// Reko: a decoder for Avr32 instruction D772 at address 0000277A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D772()
{
AssertCode("@@@", "D772");
}

// Reko: a decoder for Avr32 instruction D772 at address 0000277A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D772()
{
AssertCode("@@@", "D772");
}

// Reko: a decoder for Avr32 instruction FCE8 at address 00002780 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FCE8()
{
AssertCode("@@@", "FCE8");
}

// Reko: a decoder for Avr32 instruction FCE8 at address 00002780 has not been implemented. (0b111)
[Test]
public void Avr32Dis_FCE8()
{
AssertCode("@@@", "FCE8");
}

// Reko: a decoder for Avr32 instruction F20B at address 0000278A has not been implemented. (0b111)
[Test]
public void Avr32Dis_F20B()
{
AssertCode("@@@", "F20B");
}

// Reko: a decoder for Avr32 instruction F20B at address 0000278A has not been implemented. (0b111)
[Test]
public void Avr32Dis_F20B()
{
AssertCode("@@@", "F20B");
}

// Reko: a decoder for Avr32 instruction 1300 at address 0000278C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1300()
{
AssertCode("@@@", "1300");
}

// Reko: a decoder for Avr32 instruction 1300 at address 0000278C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1300()
{
AssertCode("@@@", "1300");
}

// Reko: a decoder for Avr32 instruction D80A at address 00002790 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D80A()
{
AssertCode("@@@", "D80A");
}

// Reko: a decoder for Avr32 instruction D80A at address 00002790 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D80A()
{
AssertCode("@@@", "D80A");
}

// Reko: a decoder for Avr32 instruction 0008 at address 00002794 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0008()
{
AssertCode("@@@", "0008");
}

// Reko: a decoder for Avr32 instruction 0008 at address 00002794 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0008()
{
AssertCode("@@@", "0008");
}

// Reko: a decoder for Avr32 instruction 5F0C at address 000027A0 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F0C()
{
AssertCode("@@@", "5F0C");
}

// Reko: a decoder for Avr32 instruction 5F0C at address 000027A0 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F0C()
{
AssertCode("@@@", "5F0C");
}

// Reko: a decoder for Avr32 instruction 6C29 at address 000027AA has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C29()
{
AssertCode("@@@", "6C29");
}

// Reko: a decoder for Avr32 instruction 6C29 at address 000027AA has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C29()
{
AssertCode("@@@", "6C29");
}

// Reko: a decoder for Avr32 instruction 7208 at address 000027AC has not been implemented. (0b011)
[Test]
public void Avr32Dis_7208()
{
AssertCode("@@@", "7208");
}

// Reko: a decoder for Avr32 instruction 7208 at address 000027AC has not been implemented. (0b011)
[Test]
public void Avr32Dis_7208()
{
AssertCode("@@@", "7208");
}

// Reko: a decoder for Avr32 instruction C021 at address 000027B0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C021()
{
AssertCode("@@@", "C021");
}

// Reko: a decoder for Avr32 instruction C021 at address 000027B0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C021()
{
AssertCode("@@@", "C021");
}

// Reko: a decoder for Avr32 instruction 930C at address 000027B2 has not been implemented. (0b100)
[Test]
public void Avr32Dis_930C()
{
AssertCode("@@@", "930C");
}

// Reko: a decoder for Avr32 instruction 930C at address 000027B2 has not been implemented. (0b100)
[Test]
public void Avr32Dis_930C()
{
AssertCode("@@@", "930C");
}

// Reko: a decoder for Avr32 instruction D7B8 at address 000027BA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7B8()
{
AssertCode("@@@", "D7B8");
}

// Reko: a decoder for Avr32 instruction D7B8 at address 000027BA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7B8()
{
AssertCode("@@@", "D7B8");
}

// Reko: a decoder for Avr32 instruction C041 at address 000027C8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C041()
{
AssertCode("@@@", "C041");
}

// Reko: a decoder for Avr32 instruction C041 at address 000027C8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C041()
{
AssertCode("@@@", "C041");
}

// Reko: a decoder for Avr32 instruction 7218 at address 000027CA has not been implemented. (0b011)
[Test]
public void Avr32Dis_7218()
{
AssertCode("@@@", "7218");
}

// Reko: a decoder for Avr32 instruction 7218 at address 000027CA has not been implemented. (0b011)
[Test]
public void Avr32Dis_7218()
{
AssertCode("@@@", "7218");
}

// Reko: a decoder for Avr32 instruction 2FF8 at address 000027CC has not been implemented. (0b001)
[Test]
public void Avr32Dis_2FF8()
{
AssertCode("@@@", "2FF8");
}

// Reko: a decoder for Avr32 instruction 2FF8 at address 000027CC has not been implemented. (0b001)
[Test]
public void Avr32Dis_2FF8()
{
AssertCode("@@@", "2FF8");
}

// Reko: a decoder for Avr32 instruction 9318 at address 000027CE has not been implemented. (0b100)
[Test]
public void Avr32Dis_9318()
{
AssertCode("@@@", "9318");
}

// Reko: a decoder for Avr32 instruction 9318 at address 000027CE has not been implemented. (0b100)
[Test]
public void Avr32Dis_9318()
{
AssertCode("@@@", "9318");
}

// Reko: a decoder for Avr32 instruction D7D0 at address 000027D6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7D0()
{
AssertCode("@@@", "D7D0");
}

// Reko: a decoder for Avr32 instruction D7D0 at address 000027D6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7D0()
{
AssertCode("@@@", "D7D0");
}

// Reko: a decoder for Avr32 instruction 4886 at address 000027DA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4886()
{
AssertCode("@@@", "4886");
}

// Reko: a decoder for Avr32 instruction 4886 at address 000027DA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4886()
{
AssertCode("@@@", "4886");
}

// Reko: a decoder for Avr32 instruction 580C at address 000027E0 has not been implemented. (0b010)
[Test]
public void Avr32Dis_580C()
{
AssertCode("@@@", "580C");
}

// Reko: a decoder for Avr32 instruction 580C at address 000027E0 has not been implemented. (0b010)
[Test]
public void Avr32Dis_580C()
{
AssertCode("@@@", "580C");
}

// Reko: a decoder for Avr32 instruction 3028 at address 000027E4 has not been implemented. (0b001)
[Test]
public void Avr32Dis_3028()
{
AssertCode("@@@", "3028");
}

// Reko: a decoder for Avr32 instruction 3028 at address 000027E4 has not been implemented. (0b001)
[Test]
public void Avr32Dis_3028()
{
AssertCode("@@@", "3028");
}

// Reko: a decoder for Avr32 instruction 9328 at address 000027E6 has not been implemented. (0b100)
[Test]
public void Avr32Dis_9328()
{
AssertCode("@@@", "9328");
}

// Reko: a decoder for Avr32 instruction 9328 at address 000027E6 has not been implemented. (0b100)
[Test]
public void Avr32Dis_9328()
{
AssertCode("@@@", "9328");
}

// Reko: a decoder for Avr32 instruction 7228 at address 000027EA has not been implemented. (0b011)
[Test]
public void Avr32Dis_7228()
{
AssertCode("@@@", "7228");
}

// Reko: a decoder for Avr32 instruction 7228 at address 000027EA has not been implemented. (0b011)
[Test]
public void Avr32Dis_7228()
{
AssertCode("@@@", "7228");
}

// Reko: a decoder for Avr32 instruction C031 at address 000027EE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C031()
{
AssertCode("@@@", "C031");
}

// Reko: a decoder for Avr32 instruction C031 at address 000027EE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C031()
{
AssertCode("@@@", "C031");
}

// Reko: a decoder for Avr32 instruction 3018 at address 000027F0 has not been implemented. (0b001)
[Test]
public void Avr32Dis_3018()
{
AssertCode("@@@", "3018");
}

// Reko: a decoder for Avr32 instruction 3018 at address 000027F0 has not been implemented. (0b001)
[Test]
public void Avr32Dis_3018()
{
AssertCode("@@@", "3018");
}

// Reko: a decoder for Avr32 instruction D7EC at address 000027FA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7EC()
{
AssertCode("@@@", "D7EC");
}

// Reko: a decoder for Avr32 instruction D7EC at address 000027FA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D7EC()
{
AssertCode("@@@", "D7EC");
}

// Reko: a decoder for Avr32 instruction 4896 at address 000027FE has not been implemented. (0b010)
[Test]
public void Avr32Dis_4896()
{
AssertCode("@@@", "4896");
}

// Reko: a decoder for Avr32 instruction 4896 at address 000027FE has not been implemented. (0b010)
[Test]
public void Avr32Dis_4896()
{
AssertCode("@@@", "4896");
}

// Reko: a decoder for Avr32 instruction 7772 at address 00002802 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7772()
{
AssertCode("@@@", "7772");
}

// Reko: a decoder for Avr32 instruction 7772 at address 00002802 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7772()
{
AssertCode("@@@", "7772");
}

// Reko: a decoder for Avr32 instruction 7763 at address 00002804 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7763()
{
AssertCode("@@@", "7763");
}

// Reko: a decoder for Avr32 instruction 7763 at address 00002804 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7763()
{
AssertCode("@@@", "7763");
}

// Reko: a decoder for Avr32 instruction 7978 at address 00002806 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7978()
{
AssertCode("@@@", "7978");
}

// Reko: a decoder for Avr32 instruction 7978 at address 00002806 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7978()
{
AssertCode("@@@", "7978");
}

// Reko: a decoder for Avr32 instruction 7969 at address 00002808 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7969()
{
AssertCode("@@@", "7969");
}

// Reko: a decoder for Avr32 instruction 7969 at address 00002808 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7969()
{
AssertCode("@@@", "7969");
}

// Reko: a decoder for Avr32 instruction 1697 at address 0000280A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1697()
{
AssertCode("@@@", "1697");
}

// Reko: a decoder for Avr32 instruction 1697 at address 0000280A has not been implemented. (0b000)
[Test]
public void Avr32Dis_1697()
{
AssertCode("@@@", "1697");
}

// Reko: a decoder for Avr32 instruction 1494 at address 0000280C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1494()
{
AssertCode("@@@", "1494");
}

// Reko: a decoder for Avr32 instruction 1494 at address 0000280C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1494()
{
AssertCode("@@@", "1494");
}

// Reko: a decoder for Avr32 instruction 069B at address 0000280E has not been implemented. (0b000)
[Test]
public void Avr32Dis_069B()
{
AssertCode("@@@", "069B");
}

// Reko: a decoder for Avr32 instruction 069B at address 0000280E has not been implemented. (0b000)
[Test]
public void Avr32Dis_069B()
{
AssertCode("@@@", "069B");
}

// Reko: a decoder for Avr32 instruction 049A at address 00002810 has not been implemented. (0b000)
[Test]
public void Avr32Dis_049A()
{
AssertCode("@@@", "049A");
}

// Reko: a decoder for Avr32 instruction 049A at address 00002810 has not been implemented. (0b000)
[Test]
public void Avr32Dis_049A()
{
AssertCode("@@@", "049A");
}

// Reko: a decoder for Avr32 instruction 1895 at address 00002812 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1895()
{
AssertCode("@@@", "1895");
}

// Reko: a decoder for Avr32 instruction 1895 at address 00002812 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1895()
{
AssertCode("@@@", "1895");
}

// Reko: a decoder for Avr32 instruction C98F at address 00002814 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C98F()
{
AssertCode("@@@", "C98F");
}

// Reko: a decoder for Avr32 instruction C98F at address 00002814 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C98F()
{
AssertCode("@@@", "C98F");
}

// Reko: a decoder for Avr32 instruction 6E0B at address 00002818 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6E0B()
{
AssertCode("@@@", "6E0B");
}

// Reko: a decoder for Avr32 instruction 6E0B at address 00002818 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6E0B()
{
AssertCode("@@@", "6E0B");
}

// Reko: a decoder for Avr32 instruction 6A0C at address 0000281A has not been implemented. (0b011)
[Test]
public void Avr32Dis_6A0C()
{
AssertCode("@@@", "6A0C");
}

// Reko: a decoder for Avr32 instruction 6A0C at address 0000281A has not been implemented. (0b011)
[Test]
public void Avr32Dis_6A0C()
{
AssertCode("@@@", "6A0C");
}

// Reko: a decoder for Avr32 instruction 5D14 at address 0000281C has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D14()
{
AssertCode("@@@", "5D14");
}

// Reko: a decoder for Avr32 instruction 5D14 at address 0000281C has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D14()
{
AssertCode("@@@", "5D14");
}

// Reko: a decoder for Avr32 instruction D832 at address 0000281E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D832()
{
AssertCode("@@@", "D832");
}

// Reko: a decoder for Avr32 instruction D832 at address 0000281E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D832()
{
AssertCode("@@@", "D832");
}

// Reko: a decoder for Avr32 instruction D810 at address 00002822 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D810()
{
AssertCode("@@@", "D810");
}

// Reko: a decoder for Avr32 instruction D810 at address 00002822 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D810()
{
AssertCode("@@@", "D810");
}

// Reko: a decoder for Avr32 instruction 7752 at address 0000282A has not been implemented. (0b011)
[Test]
public void Avr32Dis_7752()
{
AssertCode("@@@", "7752");
}

// Reko: a decoder for Avr32 instruction 7752 at address 0000282A has not been implemented. (0b011)
[Test]
public void Avr32Dis_7752()
{
AssertCode("@@@", "7752");
}

// Reko: a decoder for Avr32 instruction 7743 at address 0000282C has not been implemented. (0b011)
[Test]
public void Avr32Dis_7743()
{
AssertCode("@@@", "7743");
}

// Reko: a decoder for Avr32 instruction 7743 at address 0000282C has not been implemented. (0b011)
[Test]
public void Avr32Dis_7743()
{
AssertCode("@@@", "7743");
}

// Reko: a decoder for Avr32 instruction 7958 at address 0000282E has not been implemented. (0b011)
[Test]
public void Avr32Dis_7958()
{
AssertCode("@@@", "7958");
}

// Reko: a decoder for Avr32 instruction 7958 at address 0000282E has not been implemented. (0b011)
[Test]
public void Avr32Dis_7958()
{
AssertCode("@@@", "7958");
}

// Reko: a decoder for Avr32 instruction 7949 at address 00002830 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7949()
{
AssertCode("@@@", "7949");
}

// Reko: a decoder for Avr32 instruction 7949 at address 00002830 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7949()
{
AssertCode("@@@", "7949");
}

// Reko: a decoder for Avr32 instruction C84F at address 0000283C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C84F()
{
AssertCode("@@@", "C84F");
}

// Reko: a decoder for Avr32 instruction C84F at address 0000283C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C84F()
{
AssertCode("@@@", "C84F");
}

// Reko: a decoder for Avr32 instruction D838 at address 0000284A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D838()
{
AssertCode("@@@", "D838");
}

// Reko: a decoder for Avr32 instruction D838 at address 0000284A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D838()
{
AssertCode("@@@", "D838");
}

// Reko: a decoder for Avr32 instruction 7732 at address 00002852 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7732()
{
AssertCode("@@@", "7732");
}

// Reko: a decoder for Avr32 instruction 7732 at address 00002852 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7732()
{
AssertCode("@@@", "7732");
}

// Reko: a decoder for Avr32 instruction 7723 at address 00002854 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7723()
{
AssertCode("@@@", "7723");
}

// Reko: a decoder for Avr32 instruction 7723 at address 00002854 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7723()
{
AssertCode("@@@", "7723");
}

// Reko: a decoder for Avr32 instruction 7938 at address 00002856 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7938()
{
AssertCode("@@@", "7938");
}

// Reko: a decoder for Avr32 instruction 7938 at address 00002856 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7938()
{
AssertCode("@@@", "7938");
}

// Reko: a decoder for Avr32 instruction 7929 at address 00002858 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7929()
{
AssertCode("@@@", "7929");
}

// Reko: a decoder for Avr32 instruction 7929 at address 00002858 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7929()
{
AssertCode("@@@", "7929");
}

// Reko: a decoder for Avr32 instruction C70F at address 00002864 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C70F()
{
AssertCode("@@@", "C70F");
}

// Reko: a decoder for Avr32 instruction C70F at address 00002864 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C70F()
{
AssertCode("@@@", "C70F");
}

// Reko: a decoder for Avr32 instruction D860 at address 00002872 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D860()
{
AssertCode("@@@", "D860");
}

// Reko: a decoder for Avr32 instruction D860 at address 00002872 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D860()
{
AssertCode("@@@", "D860");
}

// Reko: a decoder for Avr32 instruction F6E4 at address 00002876 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F6E4()
{
AssertCode("@@@", "F6E4");
}

// Reko: a decoder for Avr32 instruction F6E4 at address 00002876 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F6E4()
{
AssertCode("@@@", "F6E4");
}

// Reko: a decoder for Avr32 instruction 0034 at address 00002878 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0034()
{
AssertCode("@@@", "0034");
}

// Reko: a decoder for Avr32 instruction 0034 at address 00002878 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0034()
{
AssertCode("@@@", "0034");
}

// Reko: a decoder for Avr32 instruction 189E at address 0000287A has not been implemented. (0b000)
[Test]
public void Avr32Dis_189E()
{
AssertCode("@@@", "189E");
}

// Reko: a decoder for Avr32 instruction 189E at address 0000287A has not been implemented. (0b000)
[Test]
public void Avr32Dis_189E()
{
AssertCode("@@@", "189E");
}

// Reko: a decoder for Avr32 instruction F8E8 at address 0000287C has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8E8()
{
AssertCode("@@@", "F8E8");
}

// Reko: a decoder for Avr32 instruction F8E8 at address 0000287C has not been implemented. (0b111)
[Test]
public void Avr32Dis_F8E8()
{
AssertCode("@@@", "F8E8");
}

// Reko: a decoder for Avr32 instruction 1034 at address 00002880 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1034()
{
AssertCode("@@@", "1034");
}

// Reko: a decoder for Avr32 instruction 1034 at address 00002880 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1034()
{
AssertCode("@@@", "1034");
}

// Reko: a decoder for Avr32 instruction F205 at address 00002882 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F205()
{
AssertCode("@@@", "F205");
}

// Reko: a decoder for Avr32 instruction F205 at address 00002882 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F205()
{
AssertCode("@@@", "F205");
}

// Reko: a decoder for Avr32 instruction DC2A at address 00002888 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DC2A()
{
AssertCode("@@@", "DC2A");
}

// Reko: a decoder for Avr32 instruction DC2A at address 00002888 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DC2A()
{
AssertCode("@@@", "DC2A");
}

// Reko: a decoder for Avr32 instruction 5F9C at address 00002890 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F9C()
{
AssertCode("@@@", "5F9C");
}

// Reko: a decoder for Avr32 instruction 5F9C at address 00002890 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F9C()
{
AssertCode("@@@", "5F9C");
}

// Reko: a decoder for Avr32 instruction 760B at address 00002896 has not been implemented. (0b011)
[Test]
public void Avr32Dis_760B()
{
AssertCode("@@@", "760B");
}

// Reko: a decoder for Avr32 instruction 760B at address 00002896 has not been implemented. (0b011)
[Test]
public void Avr32Dis_760B()
{
AssertCode("@@@", "760B");
}

// Reko: a decoder for Avr32 instruction 7C0C at address 00002898 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7C0C()
{
AssertCode("@@@", "7C0C");
}

// Reko: a decoder for Avr32 instruction 7C0C at address 00002898 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7C0C()
{
AssertCode("@@@", "7C0C");
}

// Reko: a decoder for Avr32 instruction 5D1A at address 0000289A has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D1A()
{
AssertCode("@@@", "5D1A");
}

// Reko: a decoder for Avr32 instruction 5D1A at address 0000289A has not been implemented. (0b010)
[Test]
public void Avr32Dis_5D1A()
{
AssertCode("@@@", "5D1A");
}

// Reko: a decoder for Avr32 instruction 780C at address 000028A2 has not been implemented. (0b011)
[Test]
public void Avr32Dis_780C()
{
AssertCode("@@@", "780C");
}

// Reko: a decoder for Avr32 instruction 780C at address 000028A2 has not been implemented. (0b011)
[Test]
public void Avr32Dis_780C()
{
AssertCode("@@@", "780C");
}

// Reko: a decoder for Avr32 instruction 4846 at address 000028AA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4846()
{
AssertCode("@@@", "4846");
}

// Reko: a decoder for Avr32 instruction 4846 at address 000028AA has not been implemented. (0b010)
[Test]
public void Avr32Dis_4846()
{
AssertCode("@@@", "4846");
}

// Reko: a decoder for Avr32 instruction FECA at address 000028AE has not been implemented. (0b111)
[Test]
public void Avr32Dis_FECA()
{
AssertCode("@@@", "FECA");
}

// Reko: a decoder for Avr32 instruction FECA at address 000028AE has not been implemented. (0b111)
[Test]
public void Avr32Dis_FECA()
{
AssertCode("@@@", "FECA");
}

// Reko: a decoder for Avr32 instruction EEC2 at address 000028B0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EEC2()
{
AssertCode("@@@", "EEC2");
}

// Reko: a decoder for Avr32 instruction EEC2 at address 000028B0 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EEC2()
{
AssertCode("@@@", "EEC2");
}

// Reko: a decoder for Avr32 instruction CA5F at address 000028B2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CA5F()
{
AssertCode("@@@", "CA5F");
}

// Reko: a decoder for Avr32 instruction CA5F at address 000028B2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CA5F()
{
AssertCode("@@@", "CA5F");
}

// Reko: a decoder for Avr32 instruction D8BC at address 000028BA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8BC()
{
AssertCode("@@@", "D8BC");
}

// Reko: a decoder for Avr32 instruction D8BC at address 000028BA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8BC()
{
AssertCode("@@@", "D8BC");
}

// Reko: a decoder for Avr32 instruction 4836 at address 000028BE has not been implemented. (0b010)
[Test]
public void Avr32Dis_4836()
{
AssertCode("@@@", "4836");
}

// Reko: a decoder for Avr32 instruction 4836 at address 000028BE has not been implemented. (0b010)
[Test]
public void Avr32Dis_4836()
{
AssertCode("@@@", "4836");
}

// Reko: a decoder for Avr32 instruction 6C6A at address 000028C2 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C6A()
{
AssertCode("@@@", "6C6A");
}

// Reko: a decoder for Avr32 instruction 6C6A at address 000028C2 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C6A()
{
AssertCode("@@@", "6C6A");
}

// Reko: a decoder for Avr32 instruction C9CF at address 000028C4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C9CF()
{
AssertCode("@@@", "C9CF");
}

// Reko: a decoder for Avr32 instruction C9CF at address 000028C4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C9CF()
{
AssertCode("@@@", "C9CF");
}

// Reko: a decoder for Avr32 instruction D8D0 at address 000028CA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8D0()
{
AssertCode("@@@", "D8D0");
}

// Reko: a decoder for Avr32 instruction D8D0 at address 000028CA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8D0()
{
AssertCode("@@@", "D8D0");
}

// Reko: a decoder for Avr32 instruction EEE8 at address 000028D6 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EEE8()
{
AssertCode("@@@", "EEE8");
}

// Reko: a decoder for Avr32 instruction EEE8 at address 000028D6 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EEE8()
{
AssertCode("@@@", "EEE8");
}

// Reko: a decoder for Avr32 instruction 109C at address 000028DA has not been implemented. (0b000)
[Test]
public void Avr32Dis_109C()
{
AssertCode("@@@", "109C");
}

// Reko: a decoder for Avr32 instruction 109C at address 000028DA has not been implemented. (0b000)
[Test]
public void Avr32Dis_109C()
{
AssertCode("@@@", "109C");
}

// Reko: a decoder for Avr32 instruction C90F at address 000028DC has not been implemented. (0b110)
[Test]
public void Avr32Dis_C90F()
{
AssertCode("@@@", "C90F");
}

// Reko: a decoder for Avr32 instruction C90F at address 000028DC has not been implemented. (0b110)
[Test]
public void Avr32Dis_C90F()
{
AssertCode("@@@", "C90F");
}

// Reko: a decoder for Avr32 instruction D8E0 at address 000028E2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8E0()
{
AssertCode("@@@", "D8E0");
}

// Reko: a decoder for Avr32 instruction D8E0 at address 000028E2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8E0()
{
AssertCode("@@@", "D8E0");
}

// Reko: a decoder for Avr32 instruction C85F at address 000028F2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C85F()
{
AssertCode("@@@", "C85F");
}

// Reko: a decoder for Avr32 instruction C85F at address 000028F2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C85F()
{
AssertCode("@@@", "C85F");
}

// Reko: a decoder for Avr32 instruction D8F8 at address 000028FA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8F8()
{
AssertCode("@@@", "D8F8");
}

// Reko: a decoder for Avr32 instruction D8F8 at address 000028FA has not been implemented. (0b110)
[Test]
public void Avr32Dis_D8F8()
{
AssertCode("@@@", "D8F8");
}

// Reko: a decoder for Avr32 instruction EF16 at address 00002904 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF16()
{
AssertCode("@@@", "EF16");
}

// Reko: a decoder for Avr32 instruction EF16 at address 00002904 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF16()
{
AssertCode("@@@", "EF16");
}

// Reko: a decoder for Avr32 instruction C8FF at address 00002906 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C8FF()
{
AssertCode("@@@", "C8FF");
}

// Reko: a decoder for Avr32 instruction C8FF at address 00002906 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C8FF()
{
AssertCode("@@@", "C8FF");
}

// Reko: a decoder for Avr32 instruction D910 at address 0000290E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D910()
{
AssertCode("@@@", "D910");
}

// Reko: a decoder for Avr32 instruction D910 at address 0000290E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D910()
{
AssertCode("@@@", "D910");
}

// Reko: a decoder for Avr32 instruction C86F at address 00002918 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C86F()
{
AssertCode("@@@", "C86F");
}

// Reko: a decoder for Avr32 instruction C86F at address 00002918 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C86F()
{
AssertCode("@@@", "C86F");
}

// Reko: a decoder for Avr32 instruction D924 at address 0000291E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D924()
{
AssertCode("@@@", "D924");
}

// Reko: a decoder for Avr32 instruction D924 at address 0000291E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D924()
{
AssertCode("@@@", "D924");
}

// Reko: a decoder for Avr32 instruction EF3C at address 0000292A has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF3C()
{
AssertCode("@@@", "EF3C");
}

// Reko: a decoder for Avr32 instruction EF3C at address 0000292A has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF3C()
{
AssertCode("@@@", "EF3C");
}

// Reko: a decoder for Avr32 instruction C7AF at address 00002930 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C7AF()
{
AssertCode("@@@", "C7AF");
}

// Reko: a decoder for Avr32 instruction C7AF at address 00002930 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C7AF()
{
AssertCode("@@@", "C7AF");
}

// Reko: a decoder for Avr32 instruction D934 at address 00002936 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D934()
{
AssertCode("@@@", "D934");
}

// Reko: a decoder for Avr32 instruction D934 at address 00002936 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D934()
{
AssertCode("@@@", "D934");
}

// Reko: a decoder for Avr32 instruction C6FF at address 00002946 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C6FF()
{
AssertCode("@@@", "C6FF");
}

// Reko: a decoder for Avr32 instruction C6FF at address 00002946 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C6FF()
{
AssertCode("@@@", "C6FF");
}

// Reko: a decoder for Avr32 instruction D94C at address 0000294E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D94C()
{
AssertCode("@@@", "D94C");
}

// Reko: a decoder for Avr32 instruction D94C at address 0000294E has not been implemented. (0b110)
[Test]
public void Avr32Dis_D94C()
{
AssertCode("@@@", "D94C");
}

// Reko: a decoder for Avr32 instruction EF6A at address 00002958 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF6A()
{
AssertCode("@@@", "EF6A");
}

// Reko: a decoder for Avr32 instruction EF6A at address 00002958 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF6A()
{
AssertCode("@@@", "EF6A");
}

// Reko: a decoder for Avr32 instruction C79F at address 0000295A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C79F()
{
AssertCode("@@@", "C79F");
}

// Reko: a decoder for Avr32 instruction C79F at address 0000295A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C79F()
{
AssertCode("@@@", "C79F");
}

// Reko: a decoder for Avr32 instruction D964 at address 00002962 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D964()
{
AssertCode("@@@", "D964");
}

// Reko: a decoder for Avr32 instruction D964 at address 00002962 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D964()
{
AssertCode("@@@", "D964");
}

// Reko: a decoder for Avr32 instruction D978 at address 00002972 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D978()
{
AssertCode("@@@", "D978");
}

// Reko: a decoder for Avr32 instruction D978 at address 00002972 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D978()
{
AssertCode("@@@", "D978");
}

// Reko: a decoder for Avr32 instruction EF90 at address 0000297E has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF90()
{
AssertCode("@@@", "EF90");
}

// Reko: a decoder for Avr32 instruction EF90 at address 0000297E has not been implemented. (0b111)
[Test]
public void Avr32Dis_EF90()
{
AssertCode("@@@", "EF90");
}

// Reko: a decoder for Avr32 instruction C64F at address 00002984 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C64F()
{
AssertCode("@@@", "C64F");
}

// Reko: a decoder for Avr32 instruction C64F at address 00002984 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C64F()
{
AssertCode("@@@", "C64F");
}

// Reko: a decoder for Avr32 instruction D988 at address 0000298A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D988()
{
AssertCode("@@@", "D988");
}

// Reko: a decoder for Avr32 instruction D988 at address 0000298A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D988()
{
AssertCode("@@@", "D988");
}

// Reko: a decoder for Avr32 instruction C59F at address 0000299A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C59F()
{
AssertCode("@@@", "C59F");
}

// Reko: a decoder for Avr32 instruction C59F at address 0000299A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C59F()
{
AssertCode("@@@", "C59F");
}

// Reko: a decoder for Avr32 instruction D9A0 at address 000029A2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9A0()
{
AssertCode("@@@", "D9A0");
}

// Reko: a decoder for Avr32 instruction D9A0 at address 000029A2 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9A0()
{
AssertCode("@@@", "D9A0");
}

// Reko: a decoder for Avr32 instruction EFBE at address 000029AC has not been implemented. (0b111)
[Test]
public void Avr32Dis_EFBE()
{
AssertCode("@@@", "EFBE");
}

// Reko: a decoder for Avr32 instruction EFBE at address 000029AC has not been implemented. (0b111)
[Test]
public void Avr32Dis_EFBE()
{
AssertCode("@@@", "EFBE");
}

// Reko: a decoder for Avr32 instruction C63F at address 000029AE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C63F()
{
AssertCode("@@@", "C63F");
}

// Reko: a decoder for Avr32 instruction C63F at address 000029AE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C63F()
{
AssertCode("@@@", "C63F");
}

// Reko: a decoder for Avr32 instruction D9B8 at address 000029B6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9B8()
{
AssertCode("@@@", "D9B8");
}

// Reko: a decoder for Avr32 instruction D9B8 at address 000029B6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9B8()
{
AssertCode("@@@", "D9B8");
}

// Reko: a decoder for Avr32 instruction C5AF at address 000029C0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C5AF()
{
AssertCode("@@@", "C5AF");
}

// Reko: a decoder for Avr32 instruction C5AF at address 000029C0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C5AF()
{
AssertCode("@@@", "C5AF");
}

// Reko: a decoder for Avr32 instruction D9CC at address 000029C6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9CC()
{
AssertCode("@@@", "D9CC");
}

// Reko: a decoder for Avr32 instruction D9CC at address 000029C6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9CC()
{
AssertCode("@@@", "D9CC");
}

// Reko: a decoder for Avr32 instruction EFE4 at address 000029D2 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EFE4()
{
AssertCode("@@@", "EFE4");
}

// Reko: a decoder for Avr32 instruction EFE4 at address 000029D2 has not been implemented. (0b111)
[Test]
public void Avr32Dis_EFE4()
{
AssertCode("@@@", "EFE4");
}

// Reko: a decoder for Avr32 instruction C4EF at address 000029D8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C4EF()
{
AssertCode("@@@", "C4EF");
}

// Reko: a decoder for Avr32 instruction C4EF at address 000029D8 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C4EF()
{
AssertCode("@@@", "C4EF");
}

// Reko: a decoder for Avr32 instruction D9DC at address 000029DE has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9DC()
{
AssertCode("@@@", "D9DC");
}

// Reko: a decoder for Avr32 instruction D9DC at address 000029DE has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9DC()
{
AssertCode("@@@", "D9DC");
}

// Reko: a decoder for Avr32 instruction C43F at address 000029EE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C43F()
{
AssertCode("@@@", "C43F");
}

// Reko: a decoder for Avr32 instruction C43F at address 000029EE has not been implemented. (0b110)
[Test]
public void Avr32Dis_C43F()
{
AssertCode("@@@", "C43F");
}

// Reko: a decoder for Avr32 instruction D9F4 at address 000029F6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9F4()
{
AssertCode("@@@", "D9F4");
}

// Reko: a decoder for Avr32 instruction D9F4 at address 000029F6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_D9F4()
{
AssertCode("@@@", "D9F4");
}

// Reko: a decoder for Avr32 instruction F012 at address 00002A00 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F012()
{
AssertCode("@@@", "F012");
}

// Reko: a decoder for Avr32 instruction F012 at address 00002A00 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F012()
{
AssertCode("@@@", "F012");
}

// Reko: a decoder for Avr32 instruction DA0C at address 00002A0A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA0C()
{
AssertCode("@@@", "DA0C");
}

// Reko: a decoder for Avr32 instruction DA0C at address 00002A0A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA0C()
{
AssertCode("@@@", "DA0C");
}

// Reko: a decoder for Avr32 instruction C45F at address 00002A14 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C45F()
{
AssertCode("@@@", "C45F");
}

// Reko: a decoder for Avr32 instruction C45F at address 00002A14 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C45F()
{
AssertCode("@@@", "C45F");
}

// Reko: a decoder for Avr32 instruction DA20 at address 00002A1A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA20()
{
AssertCode("@@@", "DA20");
}

// Reko: a decoder for Avr32 instruction DA20 at address 00002A1A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA20()
{
AssertCode("@@@", "DA20");
}

// Reko: a decoder for Avr32 instruction F038 at address 00002A26 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F038()
{
AssertCode("@@@", "F038");
}

// Reko: a decoder for Avr32 instruction F038 at address 00002A26 has not been implemented. (0b111)
[Test]
public void Avr32Dis_F038()
{
AssertCode("@@@", "F038");
}

// Reko: a decoder for Avr32 instruction C39F at address 00002A2C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C39F()
{
AssertCode("@@@", "C39F");
}

// Reko: a decoder for Avr32 instruction C39F at address 00002A2C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C39F()
{
AssertCode("@@@", "C39F");
}

// Reko: a decoder for Avr32 instruction DA30 at address 00002A32 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA30()
{
AssertCode("@@@", "DA30");
}

// Reko: a decoder for Avr32 instruction DA30 at address 00002A32 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA30()
{
AssertCode("@@@", "DA30");
}

// Reko: a decoder for Avr32 instruction C2EF at address 00002A42 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C2EF()
{
AssertCode("@@@", "C2EF");
}

// Reko: a decoder for Avr32 instruction C2EF at address 00002A42 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C2EF()
{
AssertCode("@@@", "C2EF");
}

// Reko: a decoder for Avr32 instruction DA48 at address 00002A4A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA48()
{
AssertCode("@@@", "DA48");
}

// Reko: a decoder for Avr32 instruction DA48 at address 00002A4A has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA48()
{
AssertCode("@@@", "DA48");
}

// Reko: a decoder for Avr32 instruction 6C28 at address 00002A52 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C28()
{
AssertCode("@@@", "6C28");
}

// Reko: a decoder for Avr32 instruction 6C28 at address 00002A52 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6C28()
{
AssertCode("@@@", "6C28");
}

// Reko: a decoder for Avr32 instruction 300A at address 00002A54 has not been implemented. (0b001)
[Test]
public void Avr32Dis_300A()
{
AssertCode("@@@", "300A");
}

// Reko: a decoder for Avr32 instruction 300A at address 00002A54 has not been implemented. (0b001)
[Test]
public void Avr32Dis_300A()
{
AssertCode("@@@", "300A");
}

// Reko: a decoder for Avr32 instruction 7059 at address 00002A56 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7059()
{
AssertCode("@@@", "7059");
}

// Reko: a decoder for Avr32 instruction 7059 at address 00002A56 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7059()
{
AssertCode("@@@", "7059");
}

// Reko: a decoder for Avr32 instruction 703B at address 00002A58 has not been implemented. (0b011)
[Test]
public void Avr32Dis_703B()
{
AssertCode("@@@", "703B");
}

// Reko: a decoder for Avr32 instruction 703B at address 00002A58 has not been implemented. (0b011)
[Test]
public void Avr32Dis_703B()
{
AssertCode("@@@", "703B");
}

// Reko: a decoder for Avr32 instruction 7048 at address 00002A5A has not been implemented. (0b011)
[Test]
public void Avr32Dis_7048()
{
AssertCode("@@@", "7048");
}

// Reko: a decoder for Avr32 instruction 7048 at address 00002A5A has not been implemented. (0b011)
[Test]
public void Avr32Dis_7048()
{
AssertCode("@@@", "7048");
}

// Reko: a decoder for Avr32 instruction C048 at address 00002A5C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C048()
{
AssertCode("@@@", "C048");
}

// Reko: a decoder for Avr32 instruction C048 at address 00002A5C has not been implemented. (0b110)
[Test]
public void Avr32Dis_C048()
{
AssertCode("@@@", "C048");
}

// Reko: a decoder for Avr32 instruction 10A9 at address 00002A5E has not been implemented. (0b000)
[Test]
public void Avr32Dis_10A9()
{
AssertCode("@@@", "10A9");
}

// Reko: a decoder for Avr32 instruction 10A9 at address 00002A5E has not been implemented. (0b000)
[Test]
public void Avr32Dis_10A9()
{
AssertCode("@@@", "10A9");
}

// Reko: a decoder for Avr32 instruction 2FFA at address 00002A60 has not been implemented. (0b001)
[Test]
public void Avr32Dis_2FFA()
{
AssertCode("@@@", "2FFA");
}

// Reko: a decoder for Avr32 instruction 2FFA at address 00002A60 has not been implemented. (0b001)
[Test]
public void Avr32Dis_2FFA()
{
AssertCode("@@@", "2FFA");
}

// Reko: a decoder for Avr32 instruction 2849 at address 00002A62 has not been implemented. (0b001)
[Test]
public void Avr32Dis_2849()
{
AssertCode("@@@", "2849");
}

// Reko: a decoder for Avr32 instruction 2849 at address 00002A62 has not been implemented. (0b001)
[Test]
public void Avr32Dis_2849()
{
AssertCode("@@@", "2849");
}

// Reko: a decoder for Avr32 instruction 163A at address 00002A64 has not been implemented. (0b000)
[Test]
public void Avr32Dis_163A()
{
AssertCode("@@@", "163A");
}

// Reko: a decoder for Avr32 instruction 163A at address 00002A64 has not been implemented. (0b000)
[Test]
public void Avr32Dis_163A()
{
AssertCode("@@@", "163A");
}

// Reko: a decoder for Avr32 instruction CFC1 at address 00002A66 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CFC1()
{
AssertCode("@@@", "CFC1");
}

// Reko: a decoder for Avr32 instruction CFC1 at address 00002A66 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CFC1()
{
AssertCode("@@@", "CFC1");
}

// Reko: a decoder for Avr32 instruction DA60 at address 00002A6E has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA60()
{
AssertCode("@@@", "DA60");
}

// Reko: a decoder for Avr32 instruction DA60 at address 00002A6E has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA60()
{
AssertCode("@@@", "DA60");
}

// Reko: a decoder for Avr32 instruction 4AF6 at address 00002A72 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4AF6()
{
AssertCode("@@@", "4AF6");
}

// Reko: a decoder for Avr32 instruction 4AF6 at address 00002A72 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4AF6()
{
AssertCode("@@@", "4AF6");
}

// Reko: a decoder for Avr32 instruction C080 at address 00002A78 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C080()
{
AssertCode("@@@", "C080");
}

// Reko: a decoder for Avr32 instruction C080 at address 00002A78 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C080()
{
AssertCode("@@@", "C080");
}

// Reko: a decoder for Avr32 instruction E218 at address 00002A7C has not been implemented. (0b111)
[Test]
public void Avr32Dis_E218()
{
AssertCode("@@@", "E218");
}

// Reko: a decoder for Avr32 instruction E218 at address 00002A7C has not been implemented. (0b111)
[Test]
public void Avr32Dis_E218()
{
AssertCode("@@@", "E218");
}

// Reko: a decoder for Avr32 instruction E048 at address 00002A80 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E048()
{
AssertCode("@@@", "E048");
}

// Reko: a decoder for Avr32 instruction E048 at address 00002A80 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E048()
{
AssertCode("@@@", "E048");
}

// Reko: a decoder for Avr32 instruction 8000 at address 00002A82 has not been implemented. (0b100)
[Test]
public void Avr32Dis_8000()
{
AssertCode("@@@", "8000");
}

// Reko: a decoder for Avr32 instruction 8000 at address 00002A82 has not been implemented. (0b100)
[Test]
public void Avr32Dis_8000()
{
AssertCode("@@@", "8000");
}

// Reko: a decoder for Avr32 instruction 5F08 at address 00002A84 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F08()
{
AssertCode("@@@", "5F08");
}

// Reko: a decoder for Avr32 instruction 5F08 at address 00002A84 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F08()
{
AssertCode("@@@", "5F08");
}

// Reko: a decoder for Avr32 instruction C038 at address 00002A86 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C038()
{
AssertCode("@@@", "C038");
}

// Reko: a decoder for Avr32 instruction C038 at address 00002A86 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C038()
{
AssertCode("@@@", "C038");
}

// Reko: a decoder for Avr32 instruction 585A at address 00002A88 has not been implemented. (0b010)
[Test]
public void Avr32Dis_585A()
{
AssertCode("@@@", "585A");
}

// Reko: a decoder for Avr32 instruction 585A at address 00002A88 has not been implemented. (0b010)
[Test]
public void Avr32Dis_585A()
{
AssertCode("@@@", "585A");
}

// Reko: a decoder for Avr32 instruction C0C0 at address 00002A8E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C0C0()
{
AssertCode("@@@", "C0C0");
}

// Reko: a decoder for Avr32 instruction C0C0 at address 00002A8E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C0C0()
{
AssertCode("@@@", "C0C0");
}

// Reko: a decoder for Avr32 instruction C4C0 at address 00002A92 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C4C0()
{
AssertCode("@@@", "C4C0");
}

// Reko: a decoder for Avr32 instruction C4C0 at address 00002A92 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C4C0()
{
AssertCode("@@@", "C4C0");
}

// Reko: a decoder for Avr32 instruction 7068 at address 00002A96 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7068()
{
AssertCode("@@@", "7068");
}

// Reko: a decoder for Avr32 instruction 7068 at address 00002A96 has not been implemented. (0b011)
[Test]
public void Avr32Dis_7068()
{
AssertCode("@@@", "7068");
}

// Reko: a decoder for Avr32 instruction 5838 at address 00002A98 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5838()
{
AssertCode("@@@", "5838");
}

// Reko: a decoder for Avr32 instruction 5838 at address 00002A98 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5838()
{
AssertCode("@@@", "5838");
}

// Reko: a decoder for Avr32 instruction C481 at address 00002A9A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C481()
{
AssertCode("@@@", "C481");
}

// Reko: a decoder for Avr32 instruction C481 at address 00002A9A has not been implemented. (0b110)
[Test]
public void Avr32Dis_C481()
{
AssertCode("@@@", "C481");
}

// Reko: a decoder for Avr32 instruction E21B at address 00002A9C has not been implemented. (0b111)
[Test]
public void Avr32Dis_E21B()
{
AssertCode("@@@", "E21B");
}

// Reko: a decoder for Avr32 instruction E21B at address 00002A9C has not been implemented. (0b111)
[Test]
public void Avr32Dis_E21B()
{
AssertCode("@@@", "E21B");
}

// Reko: a decoder for Avr32 instruction 0049 at address 00002A9E has not been implemented. (0b000)
[Test]
public void Avr32Dis_0049()
{
AssertCode("@@@", "0049");
}

// Reko: a decoder for Avr32 instruction 0049 at address 00002A9E has not been implemented. (0b000)
[Test]
public void Avr32Dis_0049()
{
AssertCode("@@@", "0049");
}

// Reko: a decoder for Avr32 instruction C450 at address 00002AA0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C450()
{
AssertCode("@@@", "C450");
}

// Reko: a decoder for Avr32 instruction C450 at address 00002AA0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C450()
{
AssertCode("@@@", "C450");
}

// Reko: a decoder for Avr32 instruction 32AC at address 00002AA2 has not been implemented. (0b001)
[Test]
public void Avr32Dis_32AC()
{
AssertCode("@@@", "32AC");
}

// Reko: a decoder for Avr32 instruction 32AC at address 00002AA2 has not been implemented. (0b001)
[Test]
public void Avr32Dis_32AC()
{
AssertCode("@@@", "32AC");
}

// Reko: a decoder for Avr32 instruction 4000 at address 00002AB2 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4000()
{
AssertCode("@@@", "4000");
}

// Reko: a decoder for Avr32 instruction 4000 at address 00002AB2 has not been implemented. (0b010)
[Test]
public void Avr32Dis_4000()
{
AssertCode("@@@", "4000");
}

// Reko: a decoder for Avr32 instruction 5F09 at address 00002AB4 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F09()
{
AssertCode("@@@", "5F09");
}

// Reko: a decoder for Avr32 instruction 5F09 at address 00002AB4 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5F09()
{
AssertCode("@@@", "5F09");
}

// Reko: a decoder for Avr32 instruction C068 at address 00002AB6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C068()
{
AssertCode("@@@", "C068");
}

// Reko: a decoder for Avr32 instruction C068 at address 00002AB6 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C068()
{
AssertCode("@@@", "C068");
}

// Reko: a decoder for Avr32 instruction 583A at address 00002AB8 has not been implemented. (0b010)
[Test]
public void Avr32Dis_583A()
{
AssertCode("@@@", "583A");
}

// Reko: a decoder for Avr32 instruction 583A at address 00002AB8 has not been implemented. (0b010)
[Test]
public void Avr32Dis_583A()
{
AssertCode("@@@", "583A");
}

// Reko: a decoder for Avr32 instruction 589A at address 00002ABC has not been implemented. (0b010)
[Test]
public void Avr32Dis_589A()
{
AssertCode("@@@", "589A");
}

// Reko: a decoder for Avr32 instruction 589A at address 00002ABC has not been implemented. (0b010)
[Test]
public void Avr32Dis_589A()
{
AssertCode("@@@", "589A");
}

// Reko: a decoder for Avr32 instruction 1049 at address 00002AC0 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1049()
{
AssertCode("@@@", "1049");
}

// Reko: a decoder for Avr32 instruction 1049 at address 00002AC0 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1049()
{
AssertCode("@@@", "1049");
}

// Reko: a decoder for Avr32 instruction 5809 at address 00002AC2 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5809()
{
AssertCode("@@@", "5809");
}

// Reko: a decoder for Avr32 instruction 5809 at address 00002AC2 has not been implemented. (0b010)
[Test]
public void Avr32Dis_5809()
{
AssertCode("@@@", "5809");
}

// Reko: a decoder for Avr32 instruction C030 at address 00002AC4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C030()
{
AssertCode("@@@", "C030");
}

// Reko: a decoder for Avr32 instruction C030 at address 00002AC4 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C030()
{
AssertCode("@@@", "C030");
}

// Reko: a decoder for Avr32 instruction 32FC at address 00002AC6 has not been implemented. (0b001)
[Test]
public void Avr32Dis_32FC()
{
AssertCode("@@@", "32FC");
}

// Reko: a decoder for Avr32 instruction 32FC at address 00002AC6 has not been implemented. (0b001)
[Test]
public void Avr32Dis_32FC()
{
AssertCode("@@@", "32FC");
}

// Reko: a decoder for Avr32 instruction 5818 at address 00002ACE has not been implemented. (0b010)
[Test]
public void Avr32Dis_5818()
{
AssertCode("@@@", "5818");
}

// Reko: a decoder for Avr32 instruction 5818 at address 00002ACE has not been implemented. (0b010)
[Test]
public void Avr32Dis_5818()
{
AssertCode("@@@", "5818");
}

// Reko: a decoder for Avr32 instruction C2D0 at address 00002AD0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C2D0()
{
AssertCode("@@@", "C2D0");
}

// Reko: a decoder for Avr32 instruction C2D0 at address 00002AD0 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C2D0()
{
AssertCode("@@@", "C2D0");
}

// Reko: a decoder for Avr32 instruction A000 at address 00002ADE has not been implemented. (0b101)
[Test]
public void Avr32Dis_A000()
{
AssertCode("@@@", "A000");
}

// Reko: a decoder for Avr32 instruction A000 at address 00002ADE has not been implemented. (0b101)
[Test]
public void Avr32Dis_A000()
{
AssertCode("@@@", "A000");
}

// Reko: a decoder for Avr32 instruction 586A at address 00002AE4 has not been implemented. (0b010)
[Test]
public void Avr32Dis_586A()
{
AssertCode("@@@", "586A");
}

// Reko: a decoder for Avr32 instruction 586A at address 00002AE4 has not been implemented. (0b010)
[Test]
public void Avr32Dis_586A()
{
AssertCode("@@@", "586A");
}

// Reko: a decoder for Avr32 instruction 340C at address 00002AEC has not been implemented. (0b001)
[Test]
public void Avr32Dis_340C()
{
AssertCode("@@@", "340C");
}

// Reko: a decoder for Avr32 instruction 340C at address 00002AEC has not been implemented. (0b001)
[Test]
public void Avr32Dis_340C()
{
AssertCode("@@@", "340C");
}

// Reko: a decoder for Avr32 instruction 1000 at address 00002AFC has not been implemented. (0b000)
[Test]
public void Avr32Dis_1000()
{
AssertCode("@@@", "1000");
}

// Reko: a decoder for Avr32 instruction 1000 at address 00002AFC has not been implemented. (0b000)
[Test]
public void Avr32Dis_1000()
{
AssertCode("@@@", "1000");
}

// Reko: a decoder for Avr32 instruction 581A at address 00002B02 has not been implemented. (0b010)
[Test]
public void Avr32Dis_581A()
{
AssertCode("@@@", "581A");
}

// Reko: a decoder for Avr32 instruction 581A at address 00002B02 has not been implemented. (0b010)
[Test]
public void Avr32Dis_581A()
{
AssertCode("@@@", "581A");
}

// Reko: a decoder for Avr32 instruction 37CC at address 00002B0A has not been implemented. (0b001)
[Test]
public void Avr32Dis_37CC()
{
AssertCode("@@@", "37CC");
}

// Reko: a decoder for Avr32 instruction 37CC at address 00002B0A has not been implemented. (0b001)
[Test]
public void Avr32Dis_37CC()
{
AssertCode("@@@", "37CC");
}

// Reko: a decoder for Avr32 instruction C070 at address 00002B10 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C070()
{
AssertCode("@@@", "C070");
}

// Reko: a decoder for Avr32 instruction C070 at address 00002B10 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C070()
{
AssertCode("@@@", "C070");
}

// Reko: a decoder for Avr32 instruction E04B at address 00002B16 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E04B()
{
AssertCode("@@@", "E04B");
}

// Reko: a decoder for Avr32 instruction E04B at address 00002B16 has not been implemented. (0b111)
[Test]
public void Avr32Dis_E04B()
{
AssertCode("@@@", "E04B");
}

// Reko: a decoder for Avr32 instruction C000 at address 00002B18 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C000()
{
AssertCode("@@@", "C000");
}

// Reko: a decoder for Avr32 instruction C000 at address 00002B18 has not been implemented. (0b110)
[Test]
public void Avr32Dis_C000()
{
AssertCode("@@@", "C000");
}

// Reko: a decoder for Avr32 instruction 587A at address 00002B1E has not been implemented. (0b010)
[Test]
public void Avr32Dis_587A()
{
AssertCode("@@@", "587A");
}

// Reko: a decoder for Avr32 instruction 587A at address 00002B1E has not been implemented. (0b010)
[Test]
public void Avr32Dis_587A()
{
AssertCode("@@@", "587A");
}

// Reko: a decoder for Avr32 instruction 33DC at address 00002B26 has not been implemented. (0b001)
[Test]
public void Avr32Dis_33DC()
{
AssertCode("@@@", "33DC");
}

// Reko: a decoder for Avr32 instruction 33DC at address 00002B26 has not been implemented. (0b001)
[Test]
public void Avr32Dis_33DC()
{
AssertCode("@@@", "33DC");
}

// Reko: a decoder for Avr32 instruction D82A at address 00002B2A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D82A()
{
AssertCode("@@@", "D82A");
}

// Reko: a decoder for Avr32 instruction D82A at address 00002B2A has not been implemented. (0b110)
[Test]
public void Avr32Dis_D82A()
{
AssertCode("@@@", "D82A");
}

// Reko: a decoder for Avr32 instruction DA84 at address 00002B2E has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA84()
{
AssertCode("@@@", "DA84");
}

// Reko: a decoder for Avr32 instruction DA84 at address 00002B2E has not been implemented. (0b110)
[Test]
public void Avr32Dis_DA84()
{
AssertCode("@@@", "DA84");
}

// Reko: a decoder for Avr32 instruction 1894 at address 00002B36 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1894()
{
AssertCode("@@@", "1894");
}

// Reko: a decoder for Avr32 instruction 1894 at address 00002B36 has not been implemented. (0b000)
[Test]
public void Avr32Dis_1894()
{
AssertCode("@@@", "1894");
}

// Reko: a decoder for Avr32 instruction ECF7 at address 00002B38 has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECF7()
{
AssertCode("@@@", "ECF7");
}

// Reko: a decoder for Avr32 instruction ECF7 at address 00002B38 has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECF7()
{
AssertCode("@@@", "ECF7");
}

// Reko: a decoder for Avr32 instruction 0148 at address 00002B3A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0148()
{
AssertCode("@@@", "0148");
}

// Reko: a decoder for Avr32 instruction 0148 at address 00002B3A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0148()
{
AssertCode("@@@", "0148");
}

// Reko: a decoder for Avr32 instruction C0D0 at address 00002B3E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C0D0()
{
AssertCode("@@@", "C0D0");
}

// Reko: a decoder for Avr32 instruction C0D0 at address 00002B3E has not been implemented. (0b110)
[Test]
public void Avr32Dis_C0D0()
{
AssertCode("@@@", "C0D0");
}

// Reko: a decoder for Avr32 instruction 0094 at address 00002B42 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0094()
{
AssertCode("@@@", "0094");
}

// Reko: a decoder for Avr32 instruction 0094 at address 00002B42 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0094()
{
AssertCode("@@@", "0094");
}

// Reko: a decoder for Avr32 instruction ECFB at address 00002B44 has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFB()
{
AssertCode("@@@", "ECFB");
}

// Reko: a decoder for Avr32 instruction ECFB at address 00002B44 has not been implemented. (0b111)
[Test]
public void Avr32Dis_ECFB()
{
AssertCode("@@@", "ECFB");
}

// Reko: a decoder for Avr32 instruction 0518 at address 00002B46 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0518()
{
AssertCode("@@@", "0518");
}

// Reko: a decoder for Avr32 instruction 0518 at address 00002B46 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0518()
{
AssertCode("@@@", "0518");
}

// Reko: a decoder for Avr32 instruction 700C at address 00002B48 has not been implemented. (0b011)
[Test]
public void Avr32Dis_700C()
{
AssertCode("@@@", "700C");
}

// Reko: a decoder for Avr32 instruction 700C at address 00002B48 has not been implemented. (0b011)
[Test]
public void Avr32Dis_700C()
{
AssertCode("@@@", "700C");
}

// Reko: a decoder for Avr32 instruction 6E08 at address 00002B4A has not been implemented. (0b011)
[Test]
public void Avr32Dis_6E08()
{
AssertCode("@@@", "6E08");
}

// Reko: a decoder for Avr32 instruction 6E08 at address 00002B4A has not been implemented. (0b011)
[Test]
public void Avr32Dis_6E08()
{
AssertCode("@@@", "6E08");
}

// Reko: a decoder for Avr32 instruction 1AD8 at address 00002B4C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1AD8()
{
AssertCode("@@@", "1AD8");
}

// Reko: a decoder for Avr32 instruction 1AD8 at address 00002B4C has not been implemented. (0b000)
[Test]
public void Avr32Dis_1AD8()
{
AssertCode("@@@", "1AD8");
}

// Reko: a decoder for Avr32 instruction 001A at address 00002B50 has not been implemented. (0b000)
[Test]
public void Avr32Dis_001A()
{
AssertCode("@@@", "001A");
}

// Reko: a decoder for Avr32 instruction 001A at address 00002B50 has not been implemented. (0b000)
[Test]
public void Avr32Dis_001A()
{
AssertCode("@@@", "001A");
}

// Reko: a decoder for Avr32 instruction CB08 at address 00002B52 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CB08()
{
AssertCode("@@@", "CB08");
}

// Reko: a decoder for Avr32 instruction CB08 at address 00002B52 has not been implemented. (0b110)
[Test]
public void Avr32Dis_CB08()
{
AssertCode("@@@", "CB08");
}

// Reko: a decoder for Avr32 instruction DB44 at address 00002B56 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DB44()
{
AssertCode("@@@", "DB44");
}

// Reko: a decoder for Avr32 instruction DB44 at address 00002B56 has not been implemented. (0b110)
[Test]
public void Avr32Dis_DB44()
{
AssertCode("@@@", "DB44");
}

// Reko: a decoder for Avr32 instruction 0514 at address 00002B5A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0514()
{
AssertCode("@@@", "0514");
}

// Reko: a decoder for Avr32 instruction 0514 at address 00002B5A has not been implemented. (0b000)
[Test]
public void Avr32Dis_0514()
{
AssertCode("@@@", "0514");
}

// Reko: a decoder for Avr32 instruction 0016 at address 00002B62 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0016()
{
AssertCode("@@@", "0016");
}

// Reko: a decoder for Avr32 instruction 0016 at address 00002B62 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0016()
{
AssertCode("@@@", "0016");
}

// Reko: a decoder for Avr32 instruction 6CA5 at address 00002B64 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6CA5()
{
AssertCode("@@@", "6CA5");
}

// Reko: a decoder for Avr32 instruction 6CA5 at address 00002B64 has not been implemented. (0b011)
[Test]
public void Avr32Dis_6CA5()
{
AssertCode("@@@", "6CA5");
}

// Reko: a decoder for Avr32 instruction 0510 at address 00002B68 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0510()
{
AssertCode("@@@", "0510");
}

// Reko: a decoder for Avr32 instruction 0510 at address 00002B68 has not been implemented. (0b000)
[Test]
public void Avr32Dis_0510()
{
AssertCode("@@@", "0510");
}

// Reko: a decoder for Avr32 instruction 6A0B at address 00002B6A has not been implemented. (0b011)
[Test]
public void Avr32Dis_6A0B()
{
AssertCode("@@@", "6A0B");
}
*/
    }
}
