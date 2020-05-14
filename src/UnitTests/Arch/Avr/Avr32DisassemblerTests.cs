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
            this.arch = new Avr32Architecture(CreateServiceContainer(), "avr32");
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

        [Test]
        public void Avr32Dis_pushm()
        {
            AssertCode("pushm\tr4-r7,lr", "D421");
        }

        [Test]
        public void Avr32Dis_ld_ub()
        {
            AssertCode("ld.ub\tr8[0]", "1189");
        }

        [Test]
        public void Avr32Dis_cp_b()
        {
            AssertCode("cp.b\tr9,r8", "F0091800");
        }

        [Test]
        public void Avr32Dis_breq()
        {
            AssertCode("breq\t00100008", "C040");
        }

        [Test]
        public void Avr32Dis_popm_pc()
        {
            var instr = DisassembleHexBytes("D822");
            Assert.AreEqual("popm\tr4-r7,pc", instr.ToString());
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
        }

        [Test]
        public void Avr32Dis_st_w_disp4()
        {
            AssertCode("st.w\tr10[0],r9", "9509");
        }

        [Test]
        public void Avr32Dis_icall()
        {
            AssertCode("icall\tr8", "5D18");
        }

        [Test]
        public void Avr32Dis_ld_w_format3()
        {
            AssertCode("ld.w\tr8,r8[0]", "7008");
        }

        [Test]
        public void Avr32Dis_cp_w()
        {
            AssertCode("cp.w\tr8,00000000", "5808");
        }

        [Test]
        public void Avr32Dis_B089()
        {
            AssertCode("st.b\tr8[0],r9", "B089");
        }

        [Test]
        public void Avr32Dis_cp_w_reg()
        {
            AssertCode("cp.w\tlr,r9", "123E");
        }

        // Reko: a decoder for AVR32 instruction D7B8 at address 000027BA has not been implemented. (0b1000)
        [Test]
        public void Avr32Dis_D7B8()
        {
            AssertCode("@@@", "D7B8");
        }


        // Reko: a decoder for AVR32 instruction D838 at address 0000284A has not been implemented. (0b1000)
        [Test]
        public void Avr32Dis_D838()
        {
            AssertCode("@@@", "D838");
        }




        /*




        // Reko: a decoder for AVR32 instruction 269C at address 0000101A has not been implemented. (0)
[Test]
public void Avr32Dis_269C()
{
AssertCode("@@@", "269C");
}

// Reko: a decoder for AVR32 instruction 269C at address 0000101A has not been implemented. (0)
[Test]
public void Avr32Dis_269C()
{
AssertCode("@@@", "269C");
}


// Reko: a decoder for AVR32 instruction F7E9200C at address 0000F4B4 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F7E9200C()
{
AssertCode("@@@", "F7E9200C");
}

// Reko: a decoder for AVR32 instruction F7E9200C at address 0000F4B4 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F7E9200C()
{
AssertCode("@@@", "F7E9200C");
}



// Reko: a decoder for AVR32 instruction EBCD40FF at address 0000FA54 has not been implemented. (0b11100)
[Test]
public void Avr32Dis_EBCD40FF()
{
AssertCode("@@@", "EBCD40FF");
}

// Reko: a decoder for AVR32 instruction EBCD40FF at address 0000FA54 has not been implemented. (0b11100)
[Test]
public void Avr32Dis_EBCD40FF()
{
AssertCode("@@@", "EBCD40FF");
}

// Reko: a decoder for AVR32 instruction A17B at address 0000F976 has not been implemented. (0b10)
[Test]
public void Avr32Dis_A17B()
{
AssertCode("@@@", "A17B");
}

// Reko: a decoder for AVR32 instruction A17B at address 0000F976 has not been implemented. (0b10)
[Test]
public void Avr32Dis_A17B()
{
AssertCode("@@@", "A17B");
}



// Reko: a decoder for AVR32 instruction F60C1501 at address 0000F764 has not been implemented.
[Test]
public void Avr32Dis_F60C1501()
{
AssertCode("@@@", "F60C1501");
}

// Reko: a decoder for AVR32 instruction F60C1501 at address 0000F764 has not been implemented.
[Test]
public void Avr32Dis_F60C1501()
{
AssertCode("@@@", "F60C1501");
}

// Reko: a decoder for AVR32 instruction F5EB101C at address 0000F2D0 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F5EB101C()
{
AssertCode("@@@", "F5EB101C");
}

// Reko: a decoder for AVR32 instruction F5EB101C at address 0000F2D0 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F5EB101C()
{
AssertCode("@@@", "F5EB101C");
}

// Reko: a decoder for AVR32 instruction FFFED6BA at address 000026BC has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED6BA()
{
AssertCode("@@@", "FFFED6BA");
}

// Reko: a decoder for AVR32 instruction FFFED6BA at address 000026BC has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED6BA()
{
AssertCode("@@@", "FFFED6BA");
}

// Reko: a decoder for AVR32 instruction FFFED6D4 at address 000026F4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED6D4()
{
AssertCode("@@@", "FFFED6D4");
}

// Reko: a decoder for AVR32 instruction FFFED6D4 at address 000026F4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED6D4()
{
AssertCode("@@@", "FFFED6D4");
}

// Reko: a decoder for AVR32 instruction FFFED70C at address 00002714 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED70C()
{
AssertCode("@@@", "FFFED70C");
}

// Reko: a decoder for AVR32 instruction FFFED70C at address 00002714 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED70C()
{
AssertCode("@@@", "FFFED70C");
}

// Reko: a decoder for AVR32 instruction A98A at address 00002718 has not been implemented. (0b11)
[Test]
public void Avr32Dis_A98A()
{
AssertCode("@@@", "A98A");
}

// Reko: a decoder for AVR32 instruction A98A at address 00002718 has not been implemented. (0b11)
[Test]
public void Avr32Dis_A98A()
{
AssertCode("@@@", "A98A");
}

// Reko: a decoder for AVR32 instruction F5EB118A at address 0000271C has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F5EB118A()
{
AssertCode("@@@", "F5EB118A");
}

// Reko: a decoder for AVR32 instruction F5EB118A at address 0000271C has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F5EB118A()
{
AssertCode("@@@", "F5EB118A");
}

// Reko: a decoder for AVR32 instruction E018F000 at address 00002720 has not been implemented. (0b0000)
[Test]
public void Avr32Dis_E018F000()
{
AssertCode("@@@", "E018F000");
}

// Reko: a decoder for AVR32 instruction E018F000 at address 00002720 has not been implemented. (0b0000)
[Test]
public void Avr32Dis_E018F000()
{
AssertCode("@@@", "E018F000");
}

// Reko: a decoder for AVR32 instruction F9DAC00C at address 00002724 has not been implemented. (0b11101)
[Test]
public void Avr32Dis_F9DAC00C()
{
AssertCode("@@@", "F9DAC00C");
}

// Reko: a decoder for AVR32 instruction F9DAC00C at address 00002724 has not been implemented. (0b11101)
[Test]
public void Avr32Dis_F9DAC00C()
{
AssertCode("@@@", "F9DAC00C");
}

// Reko: a decoder for AVR32 instruction F1EC100C at address 00002728 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F1EC100C()
{
AssertCode("@@@", "F1EC100C");
}

// Reko: a decoder for AVR32 instruction F1EC100C at address 00002728 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F1EC100C()
{
AssertCode("@@@", "F1EC100C");
}

// Reko: a decoder for AVR32 instruction 5EFC at address 0000272C has not been implemented. (return and test)
[Test]
public void Avr32Dis_5EFC()
{
AssertCode("@@@", "5EFC");
}

// Reko: a decoder for AVR32 instruction 5EFC at address 0000272C has not been implemented. (return and test)
[Test]
public void Avr32Dis_5EFC()
{
AssertCode("@@@", "5EFC");
}

// Reko: a decoder for AVR32 instruction F408160C at address 0000272E has not been implemented.
[Test]
public void Avr32Dis_F408160C()
{
AssertCode("@@@", "F408160C");
}

// Reko: a decoder for AVR32 instruction F408160C at address 0000272E has not been implemented.
[Test]
public void Avr32Dis_F408160C()
{
AssertCode("@@@", "F408160C");
}

// Reko: a decoder for AVR32 instruction F1EB1148 at address 00002732 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F1EB1148()
{
AssertCode("@@@", "F1EB1148");
}

// Reko: a decoder for AVR32 instruction F1EB1148 at address 00002732 has not been implemented. (0b11110)
[Test]
public void Avr32Dis_F1EB1148()
{
AssertCode("@@@", "F1EB1148");
}

// Reko: a decoder for AVR32 instruction E018FF00 at address 00002736 has not been implemented. (0b0000)
[Test]
public void Avr32Dis_E018FF00()
{
AssertCode("@@@", "E018FF00");
}

// Reko: a decoder for AVR32 instruction E018FF00 at address 00002736 has not been implemented. (0b0000)
[Test]
public void Avr32Dis_E018FF00()
{
AssertCode("@@@", "E018FF00");
}

// Reko: a decoder for AVR32 instruction F9DAC008 at address 0000273A has not been implemented. (0b11101)
[Test]
public void Avr32Dis_F9DAC008()
{
AssertCode("@@@", "F9DAC008");
}

// Reko: a decoder for AVR32 instruction F9DAC008 at address 0000273A has not been implemented. (0b11101)
[Test]
public void Avr32Dis_F9DAC008()
{
AssertCode("@@@", "F9DAC008");
}

// Reko: a decoder for AVR32 instruction F4080108 at address 0000274E has not been implemented.
[Test]
public void Avr32Dis_F4080108()
{
AssertCode("@@@", "F4080108");
}

// Reko: a decoder for AVR32 instruction F4080108 at address 0000274E has not been implemented.
[Test]
public void Avr32Dis_F4080108()
{
AssertCode("@@@", "F4080108");
}

// Reko: a decoder for AVR32 instruction F00C17A0 at address 00002754 has not been implemented.
[Test]
public void Avr32Dis_F00C17A0()
{
AssertCode("@@@", "F00C17A0");
}

// Reko: a decoder for AVR32 instruction F00C17A0 at address 00002754 has not been implemented.
[Test]
public void Avr32Dis_F00C17A0()
{
AssertCode("@@@", "F00C17A0");
}

// Reko: a decoder for AVR32 instruction F9BC0901 at address 00002758 has not been implemented. (0b11011)
[Test]
public void Avr32Dis_F9BC0901()
{
AssertCode("@@@", "F9BC0901");
}

// Reko: a decoder for AVR32 instruction F9BC0901 at address 00002758 has not been implemented. (0b11011)
[Test]
public void Avr32Dis_F9BC0901()
{
AssertCode("@@@", "F9BC0901");
}

// Reko: a decoder for AVR32 instruction F8EA0000 at address 00002766 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8EA0000()
{
AssertCode("@@@", "F8EA0000");
}

// Reko: a decoder for AVR32 instruction F8EA0000 at address 00002766 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8EA0000()
{
AssertCode("@@@", "F8EA0000");
}

// Reko: a decoder for AVR32 instruction E0A0716F at address 0000276E has not been implemented. (0b01010)
[Test]
public void Avr32Dis_E0A0716F()
{
AssertCode("@@@", "E0A0716F");
}

// Reko: a decoder for AVR32 instruction E0A0716F at address 0000276E has not been implemented. (0b01010)
[Test]
public void Avr32Dis_E0A0716F()
{
AssertCode("@@@", "E0A0716F");
}

// Reko: a decoder for AVR32 instruction D703 at address 00002776 has not been implemented. (0b0011)
[Test]
public void Avr32Dis_D703()
{
AssertCode("@@@", "D703");
}

// Reko: a decoder for AVR32 instruction D703 at address 00002776 has not been implemented. (0b0011)
[Test]
public void Avr32Dis_D703()
{
AssertCode("@@@", "D703");
}

// Reko: a decoder for AVR32 instruction FFFED772 at address 00002778 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED772()
{
AssertCode("@@@", "FFFED772");
}

// Reko: a decoder for AVR32 instruction FFFED772 at address 00002778 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED772()
{
AssertCode("@@@", "FFFED772");
}

// Reko: a decoder for AVR32 instruction FCE80000 at address 00002780 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_FCE80000()
{
AssertCode("@@@", "FCE80000");
}

// Reko: a decoder for AVR32 instruction FCE80000 at address 00002780 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_FCE80000()
{
AssertCode("@@@", "FCE80000");
}

// Reko: a decoder for AVR32 instruction F20B1300 at address 0000278A has not been implemented.
[Test]
public void Avr32Dis_F20B1300()
{
AssertCode("@@@", "F20B1300");
}

// Reko: a decoder for AVR32 instruction F20B1300 at address 0000278A has not been implemented.
[Test]
public void Avr32Dis_F20B1300()
{
AssertCode("@@@", "F20B1300");
}

// Reko: a decoder for AVR32 instruction F8EA0008 at address 00002792 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8EA0008()
{
AssertCode("@@@", "F8EA0008");
}

// Reko: a decoder for AVR32 instruction F8EA0008 at address 00002792 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8EA0008()
{
AssertCode("@@@", "F8EA0008");
}

// Reko: a decoder for AVR32 instruction FCE80008 at address 00002796 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_FCE80008()
{
AssertCode("@@@", "FCE80008");
}

// Reko: a decoder for AVR32 instruction FCE80008 at address 00002796 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_FCE80008()
{
AssertCode("@@@", "FCE80008");
}

// Reko: a decoder for AVR32 instruction 5F0C at address 000027A0 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F0C()
{
AssertCode("@@@", "5F0C");
}

// Reko: a decoder for AVR32 instruction 5F0C at address 000027A0 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F0C()
{
AssertCode("@@@", "5F0C");
}

// Reko: a decoder for AVR32 instruction FFFED7B8 at address 000027B8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7B8()
{
AssertCode("@@@", "FFFED7B8");
}

// Reko: a decoder for AVR32 instruction FFFED7B8 at address 000027B8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7B8()
{
AssertCode("@@@", "FFFED7B8");
}

// Reko: a decoder for AVR32 instruction 2FF8 at address 000027CC has not been implemented. (0)
[Test]
public void Avr32Dis_2FF8()
{
AssertCode("@@@", "2FF8");
}

// Reko: a decoder for AVR32 instruction 2FF8 at address 000027CC has not been implemented. (0)
[Test]
public void Avr32Dis_2FF8()
{
AssertCode("@@@", "2FF8");
}

// Reko: a decoder for AVR32 instruction FFFED7D0 at address 000027D4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7D0()
{
AssertCode("@@@", "FFFED7D0");
}

// Reko: a decoder for AVR32 instruction FFFED7D0 at address 000027D4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7D0()
{
AssertCode("@@@", "FFFED7D0");
}

// Reko: a decoder for AVR32 instruction FFFED7EC at address 000027F8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7EC()
{
AssertCode("@@@", "FFFED7EC");
}

// Reko: a decoder for AVR32 instruction FFFED7EC at address 000027F8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED7EC()
{
AssertCode("@@@", "FFFED7EC");
}

// Reko: a decoder for AVR32 instruction C98F at address 00002814 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C98F()
{
AssertCode("@@@", "C98F");
}

// Reko: a decoder for AVR32 instruction C98F at address 00002814 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C98F()
{
AssertCode("@@@", "C98F");
}

// Reko: a decoder for AVR32 instruction FFFED810 at address 00002820 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED810()
{
AssertCode("@@@", "FFFED810");
}

// Reko: a decoder for AVR32 instruction FFFED810 at address 00002820 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED810()
{
AssertCode("@@@", "FFFED810");
}

// Reko: a decoder for AVR32 instruction C84F at address 0000283C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C84F()
{
AssertCode("@@@", "C84F");
}

// Reko: a decoder for AVR32 instruction C84F at address 0000283C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C84F()
{
AssertCode("@@@", "C84F");
}

// Reko: a decoder for AVR32 instruction FFFED838 at address 00002848 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED838()
{
AssertCode("@@@", "FFFED838");
}

// Reko: a decoder for AVR32 instruction FFFED838 at address 00002848 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED838()
{
AssertCode("@@@", "FFFED838");
}

// Reko: a decoder for AVR32 instruction C70F at address 00002864 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C70F()
{
AssertCode("@@@", "C70F");
}

// Reko: a decoder for AVR32 instruction C70F at address 00002864 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C70F()
{
AssertCode("@@@", "C70F");
}

// Reko: a decoder for AVR32 instruction FFFED860 at address 00002870 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED860()
{
AssertCode("@@@", "FFFED860");
}

// Reko: a decoder for AVR32 instruction FFFED860 at address 00002870 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED860()
{
AssertCode("@@@", "FFFED860");
}

// Reko: a decoder for AVR32 instruction F6E40034 at address 00002876 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F6E40034()
{
AssertCode("@@@", "F6E40034");
}

// Reko: a decoder for AVR32 instruction F6E40034 at address 00002876 has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F6E40034()
{
AssertCode("@@@", "F6E40034");
}

// Reko: a decoder for AVR32 instruction F8E80034 at address 0000287C has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8E80034()
{
AssertCode("@@@", "F8E80034");
}

// Reko: a decoder for AVR32 instruction F8E80034 at address 0000287C has not been implemented. (0b01110)
[Test]
public void Avr32Dis_F8E80034()
{
AssertCode("@@@", "F8E80034");
}

// Reko: a decoder for AVR32 instruction F2051300 at address 00002882 has not been implemented.
[Test]
public void Avr32Dis_F2051300()
{
AssertCode("@@@", "F2051300");
}

// Reko: a decoder for AVR32 instruction F2051300 at address 00002882 has not been implemented.
[Test]
public void Avr32Dis_F2051300()
{
AssertCode("@@@", "F2051300");
}

// Reko: a decoder for AVR32 instruction 5F9C at address 00002890 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F9C()
{
AssertCode("@@@", "5F9C");
}

// Reko: a decoder for AVR32 instruction 5F9C at address 00002890 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F9C()
{
AssertCode("@@@", "5F9C");
}

// Reko: a decoder for AVR32 instruction CA5F at address 000028B2 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_CA5F()
{
AssertCode("@@@", "CA5F");
}

// Reko: a decoder for AVR32 instruction CA5F at address 000028B2 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_CA5F()
{
AssertCode("@@@", "CA5F");
}

// Reko: a decoder for AVR32 instruction FFFED8BC at address 000028B8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8BC()
{
AssertCode("@@@", "FFFED8BC");
}

// Reko: a decoder for AVR32 instruction FFFED8BC at address 000028B8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8BC()
{
AssertCode("@@@", "FFFED8BC");
}

// Reko: a decoder for AVR32 instruction C9CF at address 000028C4 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C9CF()
{
AssertCode("@@@", "C9CF");
}

// Reko: a decoder for AVR32 instruction C9CF at address 000028C4 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C9CF()
{
AssertCode("@@@", "C9CF");
}

// Reko: a decoder for AVR32 instruction FFFED8D0 at address 000028C8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8D0()
{
AssertCode("@@@", "FFFED8D0");
}

// Reko: a decoder for AVR32 instruction FFFED8D0 at address 000028C8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8D0()
{
AssertCode("@@@", "FFFED8D0");
}

// Reko: a decoder for AVR32 instruction C90F at address 000028DC has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C90F()
{
AssertCode("@@@", "C90F");
}

// Reko: a decoder for AVR32 instruction C90F at address 000028DC has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C90F()
{
AssertCode("@@@", "C90F");
}

// Reko: a decoder for AVR32 instruction FFFED8E0 at address 000028E0 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8E0()
{
AssertCode("@@@", "FFFED8E0");
}

// Reko: a decoder for AVR32 instruction FFFED8E0 at address 000028E0 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8E0()
{
AssertCode("@@@", "FFFED8E0");
}

// Reko: a decoder for AVR32 instruction C85F at address 000028F2 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C85F()
{
AssertCode("@@@", "C85F");
}

// Reko: a decoder for AVR32 instruction C85F at address 000028F2 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C85F()
{
AssertCode("@@@", "C85F");
}

// Reko: a decoder for AVR32 instruction FFFED8F8 at address 000028F8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8F8()
{
AssertCode("@@@", "FFFED8F8");
}

// Reko: a decoder for AVR32 instruction FFFED8F8 at address 000028F8 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED8F8()
{
AssertCode("@@@", "FFFED8F8");
}

// Reko: a decoder for AVR32 instruction C8FF at address 00002906 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C8FF()
{
AssertCode("@@@", "C8FF");
}

// Reko: a decoder for AVR32 instruction C8FF at address 00002906 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C8FF()
{
AssertCode("@@@", "C8FF");
}

// Reko: a decoder for AVR32 instruction FFFED910 at address 0000290C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED910()
{
AssertCode("@@@", "FFFED910");
}

// Reko: a decoder for AVR32 instruction FFFED910 at address 0000290C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED910()
{
AssertCode("@@@", "FFFED910");
}

// Reko: a decoder for AVR32 instruction C86F at address 00002918 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C86F()
{
AssertCode("@@@", "C86F");
}

// Reko: a decoder for AVR32 instruction C86F at address 00002918 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C86F()
{
AssertCode("@@@", "C86F");
}

// Reko: a decoder for AVR32 instruction FFFED924 at address 0000291C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED924()
{
AssertCode("@@@", "FFFED924");
}

// Reko: a decoder for AVR32 instruction FFFED924 at address 0000291C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED924()
{
AssertCode("@@@", "FFFED924");
}

// Reko: a decoder for AVR32 instruction C7AF at address 00002930 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C7AF()
{
AssertCode("@@@", "C7AF");
}

// Reko: a decoder for AVR32 instruction C7AF at address 00002930 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C7AF()
{
AssertCode("@@@", "C7AF");
}

// Reko: a decoder for AVR32 instruction FFFED934 at address 00002934 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED934()
{
AssertCode("@@@", "FFFED934");
}

// Reko: a decoder for AVR32 instruction FFFED934 at address 00002934 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED934()
{
AssertCode("@@@", "FFFED934");
}

// Reko: a decoder for AVR32 instruction C6FF at address 00002946 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C6FF()
{
AssertCode("@@@", "C6FF");
}

// Reko: a decoder for AVR32 instruction C6FF at address 00002946 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C6FF()
{
AssertCode("@@@", "C6FF");
}

// Reko: a decoder for AVR32 instruction FFFED94C at address 0000294C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED94C()
{
AssertCode("@@@", "FFFED94C");
}

// Reko: a decoder for AVR32 instruction FFFED94C at address 0000294C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED94C()
{
AssertCode("@@@", "FFFED94C");
}

// Reko: a decoder for AVR32 instruction C79F at address 0000295A has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C79F()
{
AssertCode("@@@", "C79F");
}

// Reko: a decoder for AVR32 instruction C79F at address 0000295A has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C79F()
{
AssertCode("@@@", "C79F");
}

// Reko: a decoder for AVR32 instruction FFFED964 at address 00002960 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED964()
{
AssertCode("@@@", "FFFED964");
}

// Reko: a decoder for AVR32 instruction FFFED964 at address 00002960 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED964()
{
AssertCode("@@@", "FFFED964");
}

// Reko: a decoder for AVR32 instruction FFFED978 at address 00002970 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED978()
{
AssertCode("@@@", "FFFED978");
}

// Reko: a decoder for AVR32 instruction FFFED978 at address 00002970 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED978()
{
AssertCode("@@@", "FFFED978");
}

// Reko: a decoder for AVR32 instruction C64F at address 00002984 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C64F()
{
AssertCode("@@@", "C64F");
}

// Reko: a decoder for AVR32 instruction C64F at address 00002984 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C64F()
{
AssertCode("@@@", "C64F");
}

// Reko: a decoder for AVR32 instruction FFFED988 at address 00002988 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED988()
{
AssertCode("@@@", "FFFED988");
}

// Reko: a decoder for AVR32 instruction FFFED988 at address 00002988 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED988()
{
AssertCode("@@@", "FFFED988");
}

// Reko: a decoder for AVR32 instruction C59F at address 0000299A has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C59F()
{
AssertCode("@@@", "C59F");
}

// Reko: a decoder for AVR32 instruction C59F at address 0000299A has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C59F()
{
AssertCode("@@@", "C59F");
}

// Reko: a decoder for AVR32 instruction FFFED9A0 at address 000029A0 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9A0()
{
AssertCode("@@@", "FFFED9A0");
}

// Reko: a decoder for AVR32 instruction FFFED9A0 at address 000029A0 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9A0()
{
AssertCode("@@@", "FFFED9A0");
}

// Reko: a decoder for AVR32 instruction C63F at address 000029AE has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C63F()
{
AssertCode("@@@", "C63F");
}

// Reko: a decoder for AVR32 instruction C63F at address 000029AE has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C63F()
{
AssertCode("@@@", "C63F");
}

// Reko: a decoder for AVR32 instruction FFFED9B8 at address 000029B4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9B8()
{
AssertCode("@@@", "FFFED9B8");
}

// Reko: a decoder for AVR32 instruction FFFED9B8 at address 000029B4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9B8()
{
AssertCode("@@@", "FFFED9B8");
}

// Reko: a decoder for AVR32 instruction C5AF at address 000029C0 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C5AF()
{
AssertCode("@@@", "C5AF");
}

// Reko: a decoder for AVR32 instruction C5AF at address 000029C0 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C5AF()
{
AssertCode("@@@", "C5AF");
}

// Reko: a decoder for AVR32 instruction FFFED9CC at address 000029C4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9CC()
{
AssertCode("@@@", "FFFED9CC");
}

// Reko: a decoder for AVR32 instruction FFFED9CC at address 000029C4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9CC()
{
AssertCode("@@@", "FFFED9CC");
}

// Reko: a decoder for AVR32 instruction C4EF at address 000029D8 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C4EF()
{
AssertCode("@@@", "C4EF");
}

// Reko: a decoder for AVR32 instruction C4EF at address 000029D8 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C4EF()
{
AssertCode("@@@", "C4EF");
}

// Reko: a decoder for AVR32 instruction FFFED9DC at address 000029DC has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9DC()
{
AssertCode("@@@", "FFFED9DC");
}

// Reko: a decoder for AVR32 instruction FFFED9DC at address 000029DC has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9DC()
{
AssertCode("@@@", "FFFED9DC");
}

// Reko: a decoder for AVR32 instruction C43F at address 000029EE has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C43F()
{
AssertCode("@@@", "C43F");
}

// Reko: a decoder for AVR32 instruction C43F at address 000029EE has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C43F()
{
AssertCode("@@@", "C43F");
}

// Reko: a decoder for AVR32 instruction FFFED9F4 at address 000029F4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9F4()
{
AssertCode("@@@", "FFFED9F4");
}

// Reko: a decoder for AVR32 instruction FFFED9F4 at address 000029F4 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFED9F4()
{
AssertCode("@@@", "FFFED9F4");
}

// Reko: a decoder for AVR32 instruction FFFEDA0C at address 00002A08 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA0C()
{
AssertCode("@@@", "FFFEDA0C");
}

// Reko: a decoder for AVR32 instruction FFFEDA0C at address 00002A08 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA0C()
{
AssertCode("@@@", "FFFEDA0C");
}

// Reko: a decoder for AVR32 instruction C45F at address 00002A14 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C45F()
{
AssertCode("@@@", "C45F");
}

// Reko: a decoder for AVR32 instruction C45F at address 00002A14 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C45F()
{
AssertCode("@@@", "C45F");
}

// Reko: a decoder for AVR32 instruction FFFEDA20 at address 00002A18 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA20()
{
AssertCode("@@@", "FFFEDA20");
}

// Reko: a decoder for AVR32 instruction FFFEDA20 at address 00002A18 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA20()
{
AssertCode("@@@", "FFFEDA20");
}

// Reko: a decoder for AVR32 instruction C39F at address 00002A2C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C39F()
{
AssertCode("@@@", "C39F");
}

// Reko: a decoder for AVR32 instruction C39F at address 00002A2C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C39F()
{
AssertCode("@@@", "C39F");
}

// Reko: a decoder for AVR32 instruction FFFEDA30 at address 00002A30 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA30()
{
AssertCode("@@@", "FFFEDA30");
}

// Reko: a decoder for AVR32 instruction FFFEDA30 at address 00002A30 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA30()
{
AssertCode("@@@", "FFFEDA30");
}

// Reko: a decoder for AVR32 instruction C2EF at address 00002A42 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C2EF()
{
AssertCode("@@@", "C2EF");
}

// Reko: a decoder for AVR32 instruction C2EF at address 00002A42 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C2EF()
{
AssertCode("@@@", "C2EF");
}

// Reko: a decoder for AVR32 instruction FFFEDA48 at address 00002A48 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA48()
{
AssertCode("@@@", "FFFEDA48");
}

// Reko: a decoder for AVR32 instruction FFFEDA48 at address 00002A48 has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA48()
{
AssertCode("@@@", "FFFEDA48");
}

// Reko: a decoder for AVR32 instruction C048 at address 00002A5C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C048()
{
AssertCode("@@@", "C048");
}

// Reko: a decoder for AVR32 instruction C048 at address 00002A5C has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C048()
{
AssertCode("@@@", "C048");
}

// Reko: a decoder for AVR32 instruction 10A9 at address 00002A5E has not been implemented. (0b01010)
[Test]
public void Avr32Dis_10A9()
{
AssertCode("@@@", "10A9");
}

// Reko: a decoder for AVR32 instruction 10A9 at address 00002A5E has not been implemented. (0b01010)
[Test]
public void Avr32Dis_10A9()
{
AssertCode("@@@", "10A9");
}

// Reko: a decoder for AVR32 instruction 2FFA at address 00002A60 has not been implemented. (0)
[Test]
public void Avr32Dis_2FFA()
{
AssertCode("@@@", "2FFA");
}

// Reko: a decoder for AVR32 instruction 2FFA at address 00002A60 has not been implemented. (0)
[Test]
public void Avr32Dis_2FFA()
{
AssertCode("@@@", "2FFA");
}

// Reko: a decoder for AVR32 instruction 2849 at address 00002A62 has not been implemented. (0)
[Test]
public void Avr32Dis_2849()
{
AssertCode("@@@", "2849");
}

// Reko: a decoder for AVR32 instruction 2849 at address 00002A62 has not been implemented. (0)
[Test]
public void Avr32Dis_2849()
{
AssertCode("@@@", "2849");
}

// Reko: a decoder for AVR32 instruction FFFEDA60 at address 00002A6C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA60()
{
AssertCode("@@@", "FFFEDA60");
}

// Reko: a decoder for AVR32 instruction FFFEDA60 at address 00002A6C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA60()
{
AssertCode("@@@", "FFFEDA60");
}

// Reko: a decoder for AVR32 instruction E218F000 at address 00002A7C has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E218F000()
{
AssertCode("@@@", "E218F000");
}

// Reko: a decoder for AVR32 instruction E218F000 at address 00002A7C has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E218F000()
{
AssertCode("@@@", "E218F000");
}

// Reko: a decoder for AVR32 instruction E0488000 at address 00002A80 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0488000()
{
AssertCode("@@@", "E0488000");
}

// Reko: a decoder for AVR32 instruction E0488000 at address 00002A80 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0488000()
{
AssertCode("@@@", "E0488000");
}

// Reko: a decoder for AVR32 instruction 5F08 at address 00002A84 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F08()
{
AssertCode("@@@", "5F08");
}

// Reko: a decoder for AVR32 instruction 5F08 at address 00002A84 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F08()
{
AssertCode("@@@", "5F08");
}

// Reko: a decoder for AVR32 instruction C038 at address 00002A86 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C038()
{
AssertCode("@@@", "C038");
}

// Reko: a decoder for AVR32 instruction C038 at address 00002A86 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C038()
{
AssertCode("@@@", "C038");
}

// Reko: a decoder for AVR32 instruction E21B0049 at address 00002A9C has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E21B0049()
{
AssertCode("@@@", "E21B0049");
}

// Reko: a decoder for AVR32 instruction E21B0049 at address 00002A9C has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E21B0049()
{
AssertCode("@@@", "E21B0049");
}

// Reko: a decoder for AVR32 instruction E0484000 at address 00002AB0 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0484000()
{
AssertCode("@@@", "E0484000");
}

// Reko: a decoder for AVR32 instruction E0484000 at address 00002AB0 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0484000()
{
AssertCode("@@@", "E0484000");
}

// Reko: a decoder for AVR32 instruction 5F09 at address 00002AB4 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F09()
{
AssertCode("@@@", "5F09");
}

// Reko: a decoder for AVR32 instruction 5F09 at address 00002AB4 has not been implemented. (return and test)
[Test]
public void Avr32Dis_5F09()
{
AssertCode("@@@", "5F09");
}

// Reko: a decoder for AVR32 instruction C068 at address 00002AB6 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C068()
{
AssertCode("@@@", "C068");
}

// Reko: a decoder for AVR32 instruction C068 at address 00002AB6 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_C068()
{
AssertCode("@@@", "C068");
}

// Reko: a decoder for AVR32 instruction 1049 at address 00002AC0 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_1049()
{
AssertCode("@@@", "1049");
}

// Reko: a decoder for AVR32 instruction 1049 at address 00002AC0 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_1049()
{
AssertCode("@@@", "1049");
}

// Reko: a decoder for AVR32 instruction E048A000 at address 00002ADC has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E048A000()
{
AssertCode("@@@", "E048A000");
}

// Reko: a decoder for AVR32 instruction E048A000 at address 00002ADC has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E048A000()
{
AssertCode("@@@", "E048A000");
}

// Reko: a decoder for AVR32 instruction E0481000 at address 00002AFA has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0481000()
{
AssertCode("@@@", "E0481000");
}

// Reko: a decoder for AVR32 instruction E0481000 at address 00002AFA has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E0481000()
{
AssertCode("@@@", "E0481000");
}

// Reko: a decoder for AVR32 instruction E21BF000 at address 00002B12 has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E21BF000()
{
AssertCode("@@@", "E21BF000");
}

// Reko: a decoder for AVR32 instruction E21BF000 at address 00002B12 has not been implemented. (0b0001)
[Test]
public void Avr32Dis_E21BF000()
{
AssertCode("@@@", "E21BF000");
}

// Reko: a decoder for AVR32 instruction E04BC000 at address 00002B16 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E04BC000()
{
AssertCode("@@@", "E04BC000");
}

// Reko: a decoder for AVR32 instruction E04BC000 at address 00002B16 has not been implemented. (0b00100)
[Test]
public void Avr32Dis_E04BC000()
{
AssertCode("@@@", "E04BC000");
}

// Reko: a decoder for AVR32 instruction FFFEDA84 at address 00002B2C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA84()
{
AssertCode("@@@", "FFFEDA84");
}

// Reko: a decoder for AVR32 instruction FFFEDA84 at address 00002B2C has not been implemented. (0b11111)
[Test]
public void Avr32Dis_FFFEDA84()
{
AssertCode("@@@", "FFFEDA84");
}

// Reko: a decoder for AVR32 instruction CB08 at address 00002B52 has not been implemented. (Relative jump and call)
[Test]
public void Avr32Dis_CB08()
{
AssertCode("@@@", "CB08");
}
*/
    }
}
