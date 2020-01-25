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
using Reko.Arch.V850;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.V850
{
    [TestFixture]
    public class V850DisassemblerTests : DisassemblerTestBase<V850Instruction>
    {
        private readonly V850Architecture arch = new V850Architecture("v850");
        private readonly Address addr = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexInstruction)
        {
            var instr = base.DisassembleHexBytes(hexInstruction);
            if (sExpected != instr.ToString())
            {
                Debug.Print("AssertCode(\"{0}\", \"{1}\");", instr, hexInstruction);
                Assert.AreEqual(sExpected, instr.ToString());
            }
        }

        [Test]
        public void V850Dis_Generate()
        {
            var buf = new byte[10000];
            var rnd = new Random(0x4711);
            rnd.NextBytes(buf);
            var mem = new MemoryArea(addr, buf);
            var rdr = mem.CreateLeReader(mem.BaseAddress);
            var dasm = arch.CreateDisassembler(rdr);
            foreach (var instr in dasm)
            {
                dasm.ToString();
            }
        }

        [Test]
        public void V850Dis_mov()
        {
            AssertCode("mov\tr31,r25", "1FC8");
        }

        [Test]
        public void V850Dis_mov_neg_imm()
        {
            AssertCode("mov\tFFFFFFF0,r17", "108A");
        }

        [Test]
        public void V850Dis_nop()
        {
            AssertCode("nop", "0000");
        }

        [Test]
        public void V850Dis_or()
        {
            AssertCode("or\tr2,r24", "02C1");
        }

        [Test]
        public void V850Dis_sld_b()
        {
            AssertCode("sld.b\t3[ep],r14", "0373");
        }

        [Test]
        public void V850Dis_sld_h()
        {
            AssertCode("sld.h\t6[ep],r24", "06C4");
        }

        [Test]
        public void V850Dis_sld_w()
        {
            AssertCode("sld.w\t0[ep],r8", "0045");
        }

        [Test]
        public void V850Dis_sst_b()
        {
            AssertCode("sst.b\tr15,108[ep]", "EC7B");
        }

        [Test]
        public void V850Dis_sst_h()
        {
            AssertCode("sst.h\tr24,90[ep]", "DAC4");
        }

        [Test]
        public void V850Dis_sst_w()
        {
            AssertCode("sst.w\tr24,2[ep]", "05C5");
        }

        /////////////////////////////////////////////////////////////////////////

        // ................::.::001000:.::.
        // ................::.::..:...:.::. or
        // ................::.::101001...:.
        // Reko: a decoder for V850 instruction 22DD at address 00100002 has not been implemented.
        [Test]
        public void V850Dis_22DD()
        {
            AssertCode("@@@", "22DD");
        }

        // ................::...001100..::.
        // Reko: a decoder for V850 instruction 86C1 at address 00100004 has not been implemented.
        [Test]
        public void V850Dis_86C1()
        {
            AssertCode("@@@", "86C1");
        }

        // ................:::::010101.:.::
        // Reko: a decoder for V850 instruction ABFA at address 00100006 has not been implemented.
        [Test]
        public void V850Dis_ABFA()
        {
            AssertCode("@@@", "ABFA");
        }

        // ................:....011110.::::
        // ................:.....::::..:::: sst_b
        // ................::...001111.:..:
        // Reko: a decoder for V850 instruction E9C1 at address 0010000A has not been implemented.
        [Test]
        public void V850Dis_E9C1()
        {
            AssertCode("@@@", "E9C1");
        }

        // ................::...101011:..::
        // Reko: a decoder for V850 instruction 73C5 at address 0010000C has not been implemented.
        [Test]
        public void V850Dis_73C5()
        {
            AssertCode("@@@", "73C5");
        }

        // ................:::..101101....:
        // Reko: a decoder for V850 instruction A1E5 at address 0010000E has not been implemented.
        [Test]
        public void V850Dis_A1E5()
        {
            AssertCode("@@@", "A1E5");
        }

        // ................::.::100110:....
        // ................::.:::..::.:.... sst_h
        // .................:..:000101:::.:
        // Reko: a decoder for V850 instruction BD48 at address 00100012 has not been implemented.
        [Test]
        public void V850Dis_BD48()
        {
            AssertCode("@@@", "BD48");
        }

        // ................:::.:010010..:.:
        // Reko: a decoder for V850 instruction 45EA at address 00100014 has not been implemented.
        [Test]
        public void V850Dis_45EA()
        {
            AssertCode("@@@", "45EA");
        }

        // ................:.:..110110:....
        // Reko: a decoder for V850 instruction D0A6 at address 00100016 has not been implemented.
        [Test]
        public void V850Dis_D0A6()
        {
            AssertCode("@@@", "D0A6");
        }

        // ................:...:000100:.::.
        // Reko: a decoder for V850 instruction 9688 at address 00100018 has not been implemented.
        [Test]
        public void V850Dis_9688()
        {
            AssertCode("@@@", "9688");
        }

        // ................:::..010101::::.
        // Reko: a decoder for V850 instruction BEE2 at address 0010001A has not been implemented.
        [Test]
        public void V850Dis_BEE2()
        {
            AssertCode("@@@", "BEE2");
        }

        // ..................:..101010.....
        // Reko: a decoder for V850 instruction 4025 at address 0010001C has not been implemented.
        [Test]
        public void V850Dis_4025()
        {
            AssertCode("@@@", "4025");
        }

        // ................:...:000111::.:.
        // Reko: a decoder for V850 instruction FA88 at address 0010001E has not been implemented.
        [Test]
        public void V850Dis_FA88()
        {
            AssertCode("@@@", "FA88");
        }

        // ................::..:011010:....
        // ................::..:.::.:.:.... sld_b
        // .................:..:100000:..:.
        // .................:..::.....:..:. sld_h
        // .................:...001000::.:.
        // .................:.....:...::.:. or
        // ................:::::011110::..:
        // ................:::::.::::.::..: sst_b
        // .....................001011.::..
        // Reko: a decoder for V850 instruction 6C01 at address 00100028 has not been implemented.
        [Test]
        public void V850Dis_6C01()
        {
            AssertCode("@@@", "6C01");
        }

        // ................::.::001101::.:.
        // Reko: a decoder for V850 instruction BAD9 at address 0010002A has not been implemented.
        [Test]
        public void V850Dis_BAD9()
        {
            AssertCode("@@@", "BAD9");
        }

        // ..................::.000100..::.
        // Reko: a decoder for V850 instruction 8630 at address 0010002C has not been implemented.
        [Test]
        public void V850Dis_8630()
        {
            AssertCode("@@@", "8630");
        }

        // .................:...101000.....
        // .................:...:.:.......0 Sld.w/Sst.w
        // .................:...:.:........ sld_w
        // ................:.::.111100:...:
        // Reko: a decoder for V850 instruction 91B7 at address 00100030 has not been implemented.
        [Test]
        public void V850Dis_91B7()
        {
            AssertCode("@@@", "91B7");
        }

        // ....................:111000.:.:.
        // Reko: a decoder for V850 instruction 0A0F at address 00100032 has not been implemented.
        [Test]
        public void V850Dis_0A0F()
        {
            AssertCode("@@@", "0A0F");
        }

        // ................:.:..011001:....
        // ................:.:...::..::.... sld_b
        // ..................:.:011001::.::
        // ..................:.:.::..:::.:: sld_b
        // .................::::011000.:..:
        // .................::::.::....:..: sld_b
        // ................:::::101010..:::
        // Reko: a decoder for V850 instruction 47FD at address 0010003A has not been implemented.
        [Test]
        public void V850Dis_47FD()
        {
            AssertCode("@@@", "47FD");
        }

        // ................::..:001000...:.
        // ................::..:..:......:. or
        // ................:.:.:001101::..:
        // Reko: a decoder for V850 instruction B9A9 at address 0010003E has not been implemented.
        [Test]
        public void V850Dis_B9A9()
        {
            AssertCode("@@@", "B9A9");
        }

        // .................::..001100.:..:
        // Reko: a decoder for V850 instruction 8961 at address 00100040 has not been implemented.
        [Test]
        public void V850Dis_8961()
        {
            AssertCode("@@@", "8961");
        }

        // ................:.:.:000011:....
        // Reko: a decoder for V850 instruction 70A8 at address 00100042 has not been implemented.
        [Test]
        public void V850Dis_70A8()
        {
            AssertCode("@@@", "70A8");
        }

        // .................:.:.101101:..::
        // Reko: a decoder for V850 instruction B355 at address 00100044 has not been implemented.
        [Test]
        public void V850Dis_B355()
        {
            AssertCode("@@@", "B355");
        }

        // ................:...:110110.:.::
        // Reko: a decoder for V850 instruction CB8E at address 00100046 has not been implemented.
        [Test]
        public void V850Dis_CB8E()
        {
            AssertCode("@@@", "CB8E");
        }

        // ................::...001000...:.
        // ................::.....:......:. or
        // .................:...011110:::..
        // .................:....::::.:::.. sst_b
        // ................::::.111001:....
        // Reko: a decoder for V850 instruction 30F7 at address 0010004C has not been implemented.
        [Test]
        public void V850Dis_30F7()
        {
            AssertCode("@@@", "30F7");
        }

        // .................::..110000:::::
        // Reko: a decoder for V850 instruction 1F66 at address 0010004E has not been implemented.
        [Test]
        public void V850Dis_1F66()
        {
            AssertCode("@@@", "1F66");
        }

        // .................::::001100..:::
        // Reko: a decoder for V850 instruction 8779 at address 00100050 has not been implemented.
        [Test]
        public void V850Dis_8779()
        {
            AssertCode("@@@", "8779");
        }

        // ................:...:000010:....
        // ................10001....:.:....
        // ................:...:....:.10000
        // ................:...:....:.:.... divh
        // ................:::.:011000::.::
        // ................:::.:.::...::.:: sld_b
        // ................:.:.:100010::...
        // ................:.:.::...:.::... sld_h
        // ................:::..110100.::::
        // Reko: a decoder for V850 instruction 8FE6 at address 00100058 has not been implemented.
        [Test]
        public void V850Dis_8FE6()
        {
            AssertCode("@@@", "8FE6");
        }

        // ................:..::110010...:.
        // Reko: a decoder for V850 instruction 429E at address 0010005A has not been implemented.
        [Test]
        public void V850Dis_429E()
        {
            AssertCode("@@@", "429E");
        }

        // ................::.::100111::.:.
        // ................::.:::..:::::.:. sst_h
        // ................:.:..011111::..:
        // ................:.:...:::::::..: sst_b
        // ...................:.000010..:::
        // ................00010....:...:::
        // ...................:.....:.00111
        // ...................:.....:...::: divh
        // ..................:..101010.::::
        // Reko: a decoder for V850 instruction 4F25 at address 00100062 has not been implemented.
        [Test]
        public void V850Dis_4F25()
        {
            AssertCode("@@@", "4F25");
        }

        // .................:.::000101:::::
        // Reko: a decoder for V850 instruction BF58 at address 00100064 has not been implemented.
        [Test]
        public void V850Dis_BF58()
        {
            AssertCode("@@@", "BF58");
        }

        // ..................:::100100:..:.
        // ..................::::..:..:..:. sst_h
        // .................::::011111.::..
        // .................::::.:::::.::.. sst_b
        // .................::.:111000.::..
        // Reko: a decoder for V850 instruction 0C6F at address 0010006A has not been implemented.
        [Test]
        public void V850Dis_0C6F()
        {
            AssertCode("@@@", "0C6F");
        }

        // ................:::::000110..:::
        // Reko: a decoder for V850 instruction C7F8 at address 0010006C has not been implemented.
        [Test]
        public void V850Dis_C7F8()
        {
            AssertCode("@@@", "C7F8");
        }

        // ...................:.001111::.::
        // Reko: a decoder for V850 instruction FB11 at address 0010006E has not been implemented.
        [Test]
        public void V850Dis_FB11()
        {
            AssertCode("@@@", "FB11");
        }

        // ................:::..000010:::..
        // ................11100....:.:::..
        // ................:::......:.11100
        // ................:::......:.:::.. divh
        // ................:...:001011:.:.:
        // Reko: a decoder for V850 instruction 7589 at address 00100072 has not been implemented.
        [Test]
        public void V850Dis_7589()
        {
            AssertCode("@@@", "7589");
        }

        // ..................::.000011::...
        // Reko: a decoder for V850 instruction 7830 at address 00100074 has not been implemented.
        [Test]
        public void V850Dis_7830()
        {
            AssertCode("@@@", "7830");
        }

        // ................:..::101100.::.:
        // Reko: a decoder for V850 instruction 8D9D at address 00100076 has not been implemented.
        [Test]
        public void V850Dis_8D9D()
        {
            AssertCode("@@@", "8D9D");
        }

        // ................::...100000.:...
        // ................::...:......:... sld_h
        // ................::..:100000::..:
        // ................::..::.....::..: sld_h
        // ..................:.:101000:..:.
        // ..................:.::.:...:..:0 Sld.w/Sst.w
        // ..................:.::.:...:..:. sld_w
        // ..................:..101100:..::
        // Reko: a decoder for V850 instruction 9325 at address 0010007E has not been implemented.
        [Test]
        public void V850Dis_9325()
        {
            AssertCode("@@@", "9325");
        }

        // ..................:::001110..:::
        // Reko: a decoder for V850 instruction C739 at address 00100080 has not been implemented.
        [Test]
        public void V850Dis_C739()
        {
            AssertCode("@@@", "C739");
        }

        // ...................::111111:::..
        // Reko: a decoder for V850 instruction FC1F at address 00100082 has not been implemented.
        [Test]
        public void V850Dis_FC1F()
        {
            AssertCode("@@@", "FC1F");
        }

        // ................:::::001101..:::
        // Reko: a decoder for V850 instruction A7F9 at address 00100084 has not been implemented.
        [Test]
        public void V850Dis_A7F9()
        {
            AssertCode("@@@", "A7F9");
        }

        // ..................:.:111101..:::
        // Reko: a decoder for V850 instruction A72F at address 00100086 has not been implemented.
        [Test]
        public void V850Dis_A72F()
        {
            AssertCode("@@@", "A72F");
        }

        // ...................:.100111..::.
        // ...................:.:..:::..::. sst_h
        // ................:.:..000100:::..
        // Reko: a decoder for V850 instruction 9CA0 at address 0010008A has not been implemented.
        [Test]
        public void V850Dis_9CA0()
        {
            AssertCode("@@@", "9CA0");
        }

        // ................:::.:000110::::.
        // Reko: a decoder for V850 instruction DEE8 at address 0010008C has not been implemented.
        [Test]
        public void V850Dis_DEE8()
        {
            AssertCode("@@@", "DEE8");
        }

        // ................:..:.111111..:..
        // Reko: a decoder for V850 instruction E497 at address 0010008E has not been implemented.
        [Test]
        public void V850Dis_E497()
        {
            AssertCode("@@@", "E497");
        }

        // ..................::.001111.:.:.
        // Reko: a decoder for V850 instruction EA31 at address 00100090 has not been implemented.
        [Test]
        public void V850Dis_EA31()
        {
            AssertCode("@@@", "EA31");
        }

        // ...................::010010...:.
        // Reko: a decoder for V850 instruction 421A at address 00100092 has not been implemented.
        [Test]
        public void V850Dis_421A()
        {
            AssertCode("@@@", "421A");
        }

        // .................::.:111011..:::
        // Reko: a decoder for V850 instruction 676F at address 00100094 has not been implemented.
        [Test]
        public void V850Dis_676F()
        {
            AssertCode("@@@", "676F");
        }

        // ...................:.110001.:.:.
        // Reko: a decoder for V850 instruction 2A16 at address 00100096 has not been implemented.
        [Test]
        public void V850Dis_2A16()
        {
            AssertCode("@@@", "2A16");
        }

        // ................:.::.001100...:.
        // Reko: a decoder for V850 instruction 82B1 at address 00100098 has not been implemented.
        [Test]
        public void V850Dis_82B1()
        {
            AssertCode("@@@", "82B1");
        }

        // ................:::::010000:::..
        // ................:::::.:....11100
        // ................:::::.:....:::.. mov
        // ................::...101001:::::
        // Reko: a decoder for V850 instruction 3FC5 at address 0010009C has not been implemented.
        [Test]
        public void V850Dis_3FC5()
        {
            AssertCode("@@@", "3FC5");
        }

        // ................:.:::110001.:..:
        // Reko: a decoder for V850 instruction 29BE at address 0010009E has not been implemented.
        [Test]
        public void V850Dis_29BE()
        {
            AssertCode("@@@", "29BE");
        }

        // ................:::..110111..:::
        // Reko: a decoder for V850 instruction E7E6 at address 001000A0 has not been implemented.
        [Test]
        public void V850Dis_E7E6()
        {
            AssertCode("@@@", "E7E6");
        }

        // ................:...:101101..:::
        // Reko: a decoder for V850 instruction A78D at address 001000A2 has not been implemented.
        [Test]
        public void V850Dis_A78D()
        {
            AssertCode("@@@", "A78D");
        }

        // ................::.::100111::.::
        // ................::.:::..:::::.:: sst_h
        // .................::..010011.:..:
        // Reko: a decoder for V850 instruction 6962 at address 001000A6 has not been implemented.
        [Test]
        public void V850Dis_6962()
        {
            AssertCode("@@@", "6962");
        }

        // .................:...101001::..:
        // Reko: a decoder for V850 instruction 3945 at address 001000A8 has not been implemented.
        [Test]
        public void V850Dis_3945()
        {
            AssertCode("@@@", "3945");
        }

        // .................::..011101...:.
        // .................::...:::.:...:. sst_b
        // ................:::.:101100:.:..
        // Reko: a decoder for V850 instruction 94ED at address 001000AC has not been implemented.
        [Test]
        public void V850Dis_94ED()
        {
            AssertCode("@@@", "94ED");
        }

        // ..................:..000101:::::
        // Reko: a decoder for V850 instruction BF20 at address 001000AE has not been implemented.
        [Test]
        public void V850Dis_BF20()
        {
            AssertCode("@@@", "BF20");
        }

        // ................:....010110:::::
        // Reko: a decoder for V850 instruction DF82 at address 001000B0 has not been implemented.
        [Test]
        public void V850Dis_DF82()
        {
            AssertCode("@@@", "DF82");
        }

        // ................:::..001111...:.
        // Reko: a decoder for V850 instruction E2E1 at address 001000B2 has not been implemented.
        [Test]
        public void V850Dis_E2E1()
        {
            AssertCode("@@@", "E2E1");
        }

        // .................::::001111.:.:.
        // Reko: a decoder for V850 instruction EA79 at address 001000B4 has not been implemented.
        [Test]
        public void V850Dis_EA79()
        {
            AssertCode("@@@", "EA79");
        }

        // .................:.::000101::::.
        // Reko: a decoder for V850 instruction BE58 at address 001000B6 has not been implemented.
        [Test]
        public void V850Dis_BE58()
        {
            AssertCode("@@@", "BE58");
        }

        // ................:::..110000.:::.
        // Reko: a decoder for V850 instruction 0EE6 at address 001000B8 has not been implemented.
        [Test]
        public void V850Dis_0EE6()
        {
            AssertCode("@@@", "0EE6");
        }

        // ................::::.110100:::.:
        // Reko: a decoder for V850 instruction 9DF6 at address 001000BA has not been implemented.
        [Test]
        public void V850Dis_9DF6()
        {
            AssertCode("@@@", "9DF6");
        }

        // ................:.:.:100110:::..
        // ................:.:.::..::.:::.. sst_h
        // ..................:..001111.::::
        // Reko: a decoder for V850 instruction EF21 at address 001000BE has not been implemented.
        [Test]
        public void V850Dis_EF21()
        {
            AssertCode("@@@", "EF21");
        }

        // ................::.::110111.:.:.
        // Reko: a decoder for V850 instruction EADE at address 001000C0 has not been implemented.
        [Test]
        public void V850Dis_EADE()
        {
            AssertCode("@@@", "EADE");
        }

        // ................:::::000101::...
        // Reko: a decoder for V850 instruction B8F8 at address 001000C2 has not been implemented.
        [Test]
        public void V850Dis_B8F8()
        {
            AssertCode("@@@", "B8F8");
        }

        // .................:.:.111010....:
        // Reko: a decoder for V850 instruction 4157 at address 001000C4 has not been implemented.
        [Test]
        public void V850Dis_4157()
        {
            AssertCode("@@@", "4157");
        }

        // ....................:110100::.::
        // Reko: a decoder for V850 instruction 9B0E at address 001000C6 has not been implemented.
        [Test]
        public void V850Dis_9B0E()
        {
            AssertCode("@@@", "9B0E");
        }

        // ................:::..000110..:::
        // Reko: a decoder for V850 instruction C7E0 at address 001000C8 has not been implemented.
        [Test]
        public void V850Dis_C7E0()
        {
            AssertCode("@@@", "C7E0");
        }

        // ..................:..001111.....
        // Reko: a decoder for V850 instruction E021 at address 001000CA has not been implemented.
        [Test]
        public void V850Dis_E021()
        {
            AssertCode("@@@", "E021");
        }

        // ................::...110110.::.:
        // Reko: a decoder for V850 instruction CDC6 at address 001000CC has not been implemented.
        [Test]
        public void V850Dis_CDC6()
        {
            AssertCode("@@@", "CDC6");
        }

        // ................:..:.101100...:.
        // Reko: a decoder for V850 instruction 8295 at address 001000CE has not been implemented.
        [Test]
        public void V850Dis_8295()
        {
            AssertCode("@@@", "8295");
        }

        // ................:::::100111:..:.
        // ................::::::..::::..:. sst_h
        // ................:.:::101000::.::
        // ................:.::::.:...::.:1 Sld.w/Sst.w
        // ................:.::::.:...::.:: sst_w
        // .................:..:000111...:.
        // Reko: a decoder for V850 instruction E248 at address 001000D4 has not been implemented.
        [Test]
        public void V850Dis_E248()
        {
            AssertCode("@@@", "E248");
        }

        // .................::.:110110:..::
        // Reko: a decoder for V850 instruction D36E at address 001000D6 has not been implemented.
        [Test]
        public void V850Dis_D36E()
        {
            AssertCode("@@@", "D36E");
        }

        // .................:..:111001.:...
        // Reko: a decoder for V850 instruction 284F at address 001000D8 has not been implemented.
        [Test]
        public void V850Dis_284F()
        {
            AssertCode("@@@", "284F");
        }

        // ...................:.100011::::.
        // ...................:.:...::::::. sld_h
        // .................:::.011001:::::
        // .................:::..::..:::::: sld_b
        // ................:...:101100..:.:
        // Reko: a decoder for V850 instruction 858D at address 001000DE has not been implemented.
        [Test]
        public void V850Dis_858D()
        {
            AssertCode("@@@", "858D");
        }

        // ................:....001010:.:.:
        // Reko: a decoder for V850 instruction 5581 at address 001000E0 has not been implemented.
        [Test]
        public void V850Dis_5581()
        {
            AssertCode("@@@", "5581");
        }

        // ................:.::.101011::...
        // Reko: a decoder for V850 instruction 78B5 at address 001000E2 has not been implemented.
        [Test]
        public void V850Dis_78B5()
        {
            AssertCode("@@@", "78B5");
        }

        // ................::...111001:.:..
        // Reko: a decoder for V850 instruction 34C7 at address 001000E4 has not been implemented.
        [Test]
        public void V850Dis_34C7()
        {
            AssertCode("@@@", "34C7");
        }

        // ................:...:000110::::.
        // Reko: a decoder for V850 instruction DE88 at address 001000E6 has not been implemented.
        [Test]
        public void V850Dis_DE88()
        {
            AssertCode("@@@", "DE88");
        }

        // ................::.:.100111..::.
        // ................::.:.:..:::..::. sst_h
        // ....................:010000::..:
        // ....................:.:....11001
        // ....................:.:....::..: mov
        // ................:..::100100::...
        // ................:..:::..:..::... sst_h
        // .................:..:000100.:.::
        // Reko: a decoder for V850 instruction 8B48 at address 001000EE has not been implemented.
        [Test]
        public void V850Dis_8B48()
        {
            AssertCode("@@@", "8B48");
        }

        // .....................111011..:..
        // Reko: a decoder for V850 instruction 6407 at address 001000F0 has not been implemented.
        [Test]
        public void V850Dis_6407()
        {
            AssertCode("@@@", "6407");
        }

        // ................:::..000000...::
        // ................11100.........::
        // ................:::...........:: mov
        // ..................::.010100:...:
        // Reko: a decoder for V850 instruction 9132 at address 001000F4 has not been implemented.
        [Test]
        public void V850Dis_9132()
        {
            AssertCode("@@@", "9132");
        }

        // ................:.:..010001::::.
        // Reko: a decoder for V850 instruction 3EA2 at address 001000F6 has not been implemented.
        [Test]
        public void V850Dis_3EA2()
        {
            AssertCode("@@@", "3EA2");
        }

        // .................:..:010000.:.:.
        // .................:..:.:....01010
        // .................:..:.:.....:.:. mov
        // ................::::.010110.::::
        // Reko: a decoder for V850 instruction CFF2 at address 001000FA has not been implemented.
        [Test]
        public void V850Dis_CFF2()
        {
            AssertCode("@@@", "CFF2");
        }

        // ................:....110110::...
        // Reko: a decoder for V850 instruction D886 at address 001000FC has not been implemented.
        [Test]
        public void V850Dis_D886()
        {
            AssertCode("@@@", "D886");
        }

        // .................:.:.011110.::::
        // .................:.:..::::..:::: sst_b
        // .................:.:.011010::.::
        // .................:.:..::.:.::.:: sld_b
        // .................:..:100110.:.::
        // .................:..::..::..:.:: sst_h
        // ................::..:100010.::.:
        // ................::..::...:..::.: sld_h
        // ..................:.:011011.....
        // ..................:.:.::.::..... sld_b
        // ................:..:.010000:::.:
        // ................:..:..:....11101
        // ................:..:..:....:::.: mov
        // ....................:110110:.:::
        // Reko: a decoder for V850 instruction D70E at address 0010010A has not been implemented.
        [Test]
        public void V850Dis_D70E()
        {
            AssertCode("@@@", "D70E");
        }

        // ................::..:001001.:::.
        // Reko: a decoder for V850 instruction 2EC9 at address 0010010C has not been implemented.
        [Test]
        public void V850Dis_2EC9()
        {
            AssertCode("@@@", "2EC9");
        }

        // ................:...:011110:.:.:
        // ................:...:.::::.:.:.: sst_b
        // ..................::.110110..:..
        // Reko: a decoder for V850 instruction C436 at address 00100110 has not been implemented.
        [Test]
        public void V850Dis_C436()
        {
            AssertCode("@@@", "C436");
        }

        // ..................::.111101:....
        // Reko: a decoder for V850 instruction B037 at address 00100112 has not been implemented.
        [Test]
        public void V850Dis_B037()
        {
            AssertCode("@@@", "B037");
        }

        // ................:.:..101011.::::
        // Reko: a decoder for V850 instruction 6FA5 at address 00100114 has not been implemented.
        [Test]
        public void V850Dis_6FA5()
        {
            AssertCode("@@@", "6FA5");
        }

        // ..................:..011111:::..
        // ..................:...::::::::.. sst_b
        // ................::.:.110010...:.
        // Reko: a decoder for V850 instruction 42D6 at address 00100118 has not been implemented.
        [Test]
        public void V850Dis_42D6()
        {
            AssertCode("@@@", "42D6");
        }

        // ................:::.:010101.:.::
        // Reko: a decoder for V850 instruction ABEA at address 0010011A has not been implemented.
        [Test]
        public void V850Dis_ABEA()
        {
            AssertCode("@@@", "ABEA");
        }

        // ................:::.:100111:.:::
        // ................:::.::..::::.::: sst_h
        // .................:.::111101:....
        // Reko: a decoder for V850 instruction B05F at address 0010011E has not been implemented.
        [Test]
        public void V850Dis_B05F()
        {
            AssertCode("@@@", "B05F");
        }

        // ................::...101000..:.:
        // ................::...:.:.....:.1 Sld.w/Sst.w
        // ................::...:.:.....:.: sst_w
        // .................:...101101.::::
        // Reko: a decoder for V850 instruction AF45 at address 00100122 has not been implemented.
        [Test]
        public void V850Dis_AF45()
        {
            AssertCode("@@@", "AF45");
        }

        // ..................:::111110:....
        // Reko: a decoder for V850 instruction D03F at address 00100124 has not been implemented.
        [Test]
        public void V850Dis_D03F()
        {
            AssertCode("@@@", "D03F");
        }

        // ................:..:.111111::.:.
        // Reko: a decoder for V850 instruction FA97 at address 00100126 has not been implemented.
        [Test]
        public void V850Dis_FA97()
        {
            AssertCode("@@@", "FA97");
        }

        // ................::.::000101..:::
        // Reko: a decoder for V850 instruction A7D8 at address 00100128 has not been implemented.
        [Test]
        public void V850Dis_A7D8()
        {
            AssertCode("@@@", "A7D8");
        }

        // ................:::::110110...:.
        // Reko: a decoder for V850 instruction C2FE at address 0010012A has not been implemented.
        [Test]
        public void V850Dis_C2FE()
        {
            AssertCode("@@@", "C2FE");
        }

        // ..................::.000010::.::
        // ................00110....:.::.::
        // ..................::.....:.11011
        // ..................::.....:.::.:: divh
        // .................:...001011:..::
        // Reko: a decoder for V850 instruction 7341 at address 0010012E has not been implemented.
        [Test]
        public void V850Dis_7341()
        {
            AssertCode("@@@", "7341");
        }

        // .................:...011011:.:..
        // .................:....::.:::.:.. sld_b
        // ................:.:..001001.:::.
        // Reko: a decoder for V850 instruction 2EA1 at address 00100132 has not been implemented.
        [Test]
        public void V850Dis_2EA1()
        {
            AssertCode("@@@", "2EA1");
        }

        // ................:..:.111110:.::.
        // Reko: a decoder for V850 instruction D697 at address 00100134 has not been implemented.
        [Test]
        public void V850Dis_D697()
        {
            AssertCode("@@@", "D697");
        }

        // ................:.::.101111...::
        // Reko: a decoder for V850 instruction E3B5 at address 00100136 has not been implemented.
        [Test]
        public void V850Dis_E3B5()
        {
            AssertCode("@@@", "E3B5");
        }

        // ................:.:.:000010:.:::
        // ................10101....:.:.:::
        // ................:.:.:....:.10111
        // ................:.:.:....:.:.::: divh
        // .................:::.111000.:..:
        // Reko: a decoder for V850 instruction 0977 at address 0010013A has not been implemented.
        [Test]
        public void V850Dis_0977()
        {
            AssertCode("@@@", "0977");
        }

        // .................:::.101001.....
        // Reko: a decoder for V850 instruction 2075 at address 0010013C has not been implemented.
        [Test]
        public void V850Dis_2075()
        {
            AssertCode("@@@", "2075");
        }

        // ..................::.001001:..::
        // Reko: a decoder for V850 instruction 3331 at address 0010013E has not been implemented.
        [Test]
        public void V850Dis_3331()
        {
            AssertCode("@@@", "3331");
        }

        // .................:::.111111....:
        // Reko: a decoder for V850 instruction E177 at address 00100140 has not been implemented.
        [Test]
        public void V850Dis_E177()
        {
            AssertCode("@@@", "E177");
        }

        // .................::.:001000.:..:
        // .................::.:..:....:..: or
        // ................:..:.000000::..:
        // ................10010......::..:
        // ................:..:.......::..: mov
        // ................:....100101::.::
        // ................:....:..:.:::.:: sst_h
        // ...................::000001:::..
        // ...................::.....::::.. not
        // ................:..:.111100.::::
        // Reko: a decoder for V850 instruction 8F97 at address 0010014A has not been implemented.
        [Test]
        public void V850Dis_8F97()
        {
            AssertCode("@@@", "8F97");
        }

        // ................:::::101111...:.
        // Reko: a decoder for V850 instruction E2FD at address 0010014C has not been implemented.
        [Test]
        public void V850Dis_E2FD()
        {
            AssertCode("@@@", "E2FD");
        }

        // ...................:.001110.:...
        // Reko: a decoder for V850 instruction C811 at address 0010014E has not been implemented.
        [Test]
        public void V850Dis_C811()
        {
            AssertCode("@@@", "C811");
        }

        // .................::.:010011...::
        // Reko: a decoder for V850 instruction 636A at address 00100150 has not been implemented.
        [Test]
        public void V850Dis_636A()
        {
            AssertCode("@@@", "636A");
        }

        // ................:.::.011011:.::.
        // ................:.::..::.:::.::. sld_b
        // ....................:110101::::.
        // Reko: a decoder for V850 instruction BE0E at address 00100154 has not been implemented.
        [Test]
        public void V850Dis_BE0E()
        {
            AssertCode("@@@", "BE0E");
        }

        // .................::.:101100.:..:
        // Reko: a decoder for V850 instruction 896D at address 00100156 has not been implemented.
        [Test]
        public void V850Dis_896D()
        {
            AssertCode("@@@", "896D");
        }

        // ................:::::001111....:
        // Reko: a decoder for V850 instruction E1F9 at address 00100158 has not been implemented.
        [Test]
        public void V850Dis_E1F9()
        {
            AssertCode("@@@", "E1F9");
        }

        // ..................:.:000111.::.:
        // Reko: a decoder for V850 instruction ED28 at address 0010015A has not been implemented.
        [Test]
        public void V850Dis_ED28()
        {
            AssertCode("@@@", "ED28");
        }

        // ................:....011001:.:.:
        // ................:.....::..::.:.: sld_b
        // ..................::.001011:.:::
        // Reko: a decoder for V850 instruction 7731 at address 0010015E has not been implemented.
        [Test]
        public void V850Dis_7731()
        {
            AssertCode("@@@", "7731");
        }

        // ................:..:.001101..:..
        // Reko: a decoder for V850 instruction A491 at address 00100160 has not been implemented.
        [Test]
        public void V850Dis_A491()
        {
            AssertCode("@@@", "A491");
        }

        // ................:::..011000:::..
        // ................:::...::...:::.. sld_b
        // ....................:010001..:::
        // Reko: a decoder for V850 instruction 270A at address 00100164 has not been implemented.
        [Test]
        public void V850Dis_270A()
        {
            AssertCode("@@@", "270A");
        }

        // ................:::..011001..:::
        // ................:::...::..:..::: sld_b
        // ................:.:.:001100.:...
        // Reko: a decoder for V850 instruction 88A9 at address 00100168 has not been implemented.
        [Test]
        public void V850Dis_88A9()
        {
            AssertCode("@@@", "88A9");
        }

        // ................:.:::101011..:.:
        // Reko: a decoder for V850 instruction 65BD at address 0010016A has not been implemented.
        [Test]
        public void V850Dis_65BD()
        {
            AssertCode("@@@", "65BD");
        }

        // .................:..:100000.:...
        // .................:..::......:... sld_h
        // .................:.::010110:..::
        // Reko: a decoder for V850 instruction D35A at address 0010016E has not been implemented.
        [Test]
        public void V850Dis_D35A()
        {
            AssertCode("@@@", "D35A");
        }

        // ................::::.111111.....
        // Reko: a decoder for V850 instruction E0F7 at address 00100170 has not been implemented.
        [Test]
        public void V850Dis_E0F7()
        {
            AssertCode("@@@", "E0F7");
        }

        // ................:.::.110000:..::
        // Reko: a decoder for V850 instruction 13B6 at address 00100172 has not been implemented.
        [Test]
        public void V850Dis_13B6()
        {
            AssertCode("@@@", "13B6");
        }

        // .................::::111111:::::
        // Reko: a decoder for V850 instruction FF7F at address 00100174 has not been implemented.
        [Test]
        public void V850Dis_FF7F()
        {
            AssertCode("@@@", "FF7F");
        }

        // ................::.:.011011:..:.
        // ................::.:..::.:::..:. sld_b
        // ................:..::011111.:::.
        // ................:..::.:::::.:::. sst_b
        // ...................::111100..:::
        // Reko: a decoder for V850 instruction 871F at address 0010017A has not been implemented.
        [Test]
        public void V850Dis_871F()
        {
            AssertCode("@@@", "871F");
        }

        // ................:..:.010101...:.
        // Reko: a decoder for V850 instruction A292 at address 0010017C has not been implemented.
        [Test]
        public void V850Dis_A292()
        {
            AssertCode("@@@", "A292");
        }

        // ................:.:::000010.::::
        // ................10111....:..::::
        // ................:.:::....:.01111
        // ................:.:::....:..:::: divh
        // .................:.::101101....:
        // Reko: a decoder for V850 instruction A15D at address 00100180 has not been implemented.
        [Test]
        public void V850Dis_A15D()
        {
            AssertCode("@@@", "A15D");
        }

        // .................:.:.001111:..:.
        // Reko: a decoder for V850 instruction F251 at address 00100182 has not been implemented.
        [Test]
        public void V850Dis_F251()
        {
            AssertCode("@@@", "F251");
        }

        // ................:::..100111.....
        // ................:::..:..:::..... sst_h
        // .................:.::001111.:.:.
        // Reko: a decoder for V850 instruction EA59 at address 00100186 has not been implemented.
        [Test]
        public void V850Dis_EA59()
        {
            AssertCode("@@@", "EA59");
        }

        // ................:..:.010010...:.
        // Reko: a decoder for V850 instruction 4292 at address 00100188 has not been implemented.
        [Test]
        public void V850Dis_4292()
        {
            AssertCode("@@@", "4292");
        }

        // ...................:.101001.:.::
        // Reko: a decoder for V850 instruction 2B15 at address 0010018A has not been implemented.
        [Test]
        public void V850Dis_2B15()
        {
            AssertCode("@@@", "2B15");
        }

        // ................::..:011101.:..:
        // ................::..:.:::.:.:..: sst_b
        // ................:..:.101011..:.:
        // Reko: a decoder for V850 instruction 6595 at address 0010018E has not been implemented.
        [Test]
        public void V850Dis_6595()
        {
            AssertCode("@@@", "6595");
        }

        // ................:..:.011001..::.
        // ................:..:..::..:..::. sld_b
        // ..................:::101010..:::
        // Reko: a decoder for V850 instruction 473D at address 00100192 has not been implemented.
        [Test]
        public void V850Dis_473D()
        {
            AssertCode("@@@", "473D");
        }

        // ................::...000011.::::
        // Reko: a decoder for V850 instruction 6FC0 at address 00100194 has not been implemented.
        [Test]
        public void V850Dis_6FC0()
        {
            AssertCode("@@@", "6FC0");
        }

        // ..................:..100001:..:.
        // ..................:..:....::..:. sld_h
        // .................:..:001000::::.
        // .................:..:..:...::::. or
        // ................:...:011111:.:.:
        // ................:...:.::::::.:.: sst_b
        // .....................111100...:.
        // Reko: a decoder for V850 instruction 8207 at address 0010019C has not been implemented.
        [Test]
        public void V850Dis_8207()
        {
            AssertCode("@@@", "8207");
        }

        // ................::::.101010.....
        // Reko: a decoder for V850 instruction 40F5 at address 0010019E has not been implemented.
        [Test]
        public void V850Dis_40F5()
        {
            AssertCode("@@@", "40F5");
        }

        // ................:.:.:101010::..:
        // Reko: a decoder for V850 instruction 59AD at address 001001A0 has not been implemented.
        [Test]
        public void V850Dis_59AD()
        {
            AssertCode("@@@", "59AD");
        }

        // ................:.:.:010000:::..
        // ................:.:.:.:....11100
        // ................:.:.:.:....:::.. mov
        // ................:....001010::..:
        // Reko: a decoder for V850 instruction 5981 at address 001001A4 has not been implemented.
        [Test]
        public void V850Dis_5981()
        {
            AssertCode("@@@", "5981");
        }

        // ................:::::100011::..:
        // ................::::::...::::..: sld_h
        // ................:.:::010000.::..
        // ................:.:::.:....01100
        // ................:.:::.:.....::.. mov
        // .................::.:100011:.:..
        // .................::.::...:::.:.. sld_h
        // .................:.::101010:.:..
        // Reko: a decoder for V850 instruction 545D at address 001001AC has not been implemented.
        [Test]
        public void V850Dis_545D()
        {
            AssertCode("@@@", "545D");
        }

        // ..................:..011001.:..:
        // ..................:...::..:.:..: sld_b
        // ................::::.010001:.::.
        // Reko: a decoder for V850 instruction 36F2 at address 001001B0 has not been implemented.
        [Test]
        public void V850Dis_36F2()
        {
            AssertCode("@@@", "36F2");
        }

        // ....................:111111::..:
        // Reko: a decoder for V850 instruction F90F at address 001001B2 has not been implemented.
        [Test]
        public void V850Dis_F90F()
        {
            AssertCode("@@@", "F90F");
        }

        // ...................:.001010.:.::
        // Reko: a decoder for V850 instruction 4B11 at address 001001B4 has not been implemented.
        [Test]
        public void V850Dis_4B11()
        {
            AssertCode("@@@", "4B11");
        }

        // ................:::..000010.:..:
        // ................11100....:..:..:
        // ................:::......:.01001
        // ................:::......:..:..: divh
        // ................::..:101001.:.::
        // Reko: a decoder for V850 instruction 2BCD at address 001001B8 has not been implemented.
        [Test]
        public void V850Dis_2BCD()
        {
            AssertCode("@@@", "2BCD");
        }

        // ................:..:.110001:..::
        // Reko: a decoder for V850 instruction 3396 at address 001001BA has not been implemented.
        [Test]
        public void V850Dis_3396()
        {
            AssertCode("@@@", "3396");
        }

        // .................:.::111111.::::
        // Reko: a decoder for V850 instruction EF5F at address 001001BC has not been implemented.
        [Test]
        public void V850Dis_EF5F()
        {
            AssertCode("@@@", "EF5F");
        }

        // ..................:::010110:.::.
        // Reko: a decoder for V850 instruction D63A at address 001001BE has not been implemented.
        [Test]
        public void V850Dis_D63A()
        {
            AssertCode("@@@", "D63A");
        }

        // ..................::.101010:..:.
        // Reko: a decoder for V850 instruction 5235 at address 001001C0 has not been implemented.
        [Test]
        public void V850Dis_5235()
        {
            AssertCode("@@@", "5235");
        }

        // ................:::::100101::.::
        // ................::::::..:.:::.:: sst_h
        // ................:.:::010101.::::
        // Reko: a decoder for V850 instruction AFBA at address 001001C4 has not been implemented.
        [Test]
        public void V850Dis_AFBA()
        {
            AssertCode("@@@", "AFBA");
        }

        // ................:.:..101001::.::
        // Reko: a decoder for V850 instruction 3BA5 at address 001001C6 has not been implemented.
        [Test]
        public void V850Dis_3BA5()
        {
            AssertCode("@@@", "3BA5");
        }

        // .................:..:011000::.:.
        // .................:..:.::...::.:. sld_b
        // ................:.::.100001:....
        // ................:.::.:....::.... sld_h
        // ................::...101010.::..
        // Reko: a decoder for V850 instruction 4CC5 at address 001001CC has not been implemented.
        [Test]
        public void V850Dis_4CC5()
        {
            AssertCode("@@@", "4CC5");
        }

        // ................::...001110:....
        // Reko: a decoder for V850 instruction D0C1 at address 001001CE has not been implemented.
        [Test]
        public void V850Dis_D0C1()
        {
            AssertCode("@@@", "D0C1");
        }

        // ................::.:.010000:::::
        // ................::.:..:....11111
        // ................::.:..:....::::: mov
        // ................:..:.000010:.:..
        // ................10010....:.:.:..
        // ................:..:.....:.10100
        // ................:..:.....:.:.:.. divh
        // ..................::.001000.:::.
        // ..................::...:....:::. or
        // .................:..:000101.:.:.
        // Reko: a decoder for V850 instruction AA48 at address 001001D6 has not been implemented.
        [Test]
        public void V850Dis_AA48()
        {
            AssertCode("@@@", "AA48");
        }

        // ..................:::011011..:..
        // ..................:::.::.::..:.. sld_b
        // ................:...:100011::::.
        // ................:...::...::::::. sld_h
        // ................:::..010010.::.:
        // Reko: a decoder for V850 instruction 4DE2 at address 001001DC has not been implemented.
        [Test]
        public void V850Dis_4DE2()
        {
            AssertCode("@@@", "4DE2");
        }

        // ................:.:.:101000.::..
        // ................:.:.::.:....::.0 Sld.w/Sst.w
        // ................:.:.::.:....::.. sld_w
        // ..................:::001111..:..
        // Reko: a decoder for V850 instruction E439 at address 001001E0 has not been implemented.
        [Test]
        public void V850Dis_E439()
        {
            AssertCode("@@@", "E439");
        }

        // ...................::110000::.:.
        // Reko: a decoder for V850 instruction 1A1E at address 001001E2 has not been implemented.
        [Test]
        public void V850Dis_1A1E()
        {
            AssertCode("@@@", "1A1E");
        }

        // ..................:.:101101....:
        // Reko: a decoder for V850 instruction A12D at address 001001E4 has not been implemented.
        [Test]
        public void V850Dis_A12D()
        {
            AssertCode("@@@", "A12D");
        }

        // .................::::011011:..::
        // .................::::.::.:::..:: sld_b
        // .................:.:.011101:.::.
        // .................:.:..:::.::.::. sst_b
        // ................:::::111111:.:.:
        // Reko: a decoder for V850 instruction F5FF at address 001001EA has not been implemented.
        [Test]
        public void V850Dis_F5FF()
        {
            AssertCode("@@@", "F5FF");
        }

        // ................:..:.100111....:
        // ................:..:.:..:::....: sst_h
        // .................::..110100:.:..
        // Reko: a decoder for V850 instruction 9466 at address 001001EE has not been implemented.
        [Test]
        public void V850Dis_9466()
        {
            AssertCode("@@@", "9466");
        }

        // .................:::.001111:::.:
        // Reko: a decoder for V850 instruction FD71 at address 001001F0 has not been implemented.
        [Test]
        public void V850Dis_FD71()
        {
            AssertCode("@@@", "FD71");
        }

        // ...................::101011:.::.
        // Reko: a decoder for V850 instruction 761D at address 001001F2 has not been implemented.
        [Test]
        public void V850Dis_761D()
        {
            AssertCode("@@@", "761D");
        }

        // .................:...101000.:.::
        // .................:...:.:....:.:1 Sld.w/Sst.w
        // .................:...:.:....:.:: sst_w
        // ..................:..000001.::..
        // ..................:.......:.::.. not
        // .................:::.011100.:.::
        // .................:::..:::...:.:: sst_b
        // .................:::.100100::...
        // .................:::.:..:..::... sst_h
        // ................:.:.:111001:::..
        // Reko: a decoder for V850 instruction 3CAF at address 001001FC has not been implemented.
        [Test]
        public void V850Dis_3CAF()
        {
            AssertCode("@@@", "3CAF");
        }

        // ..................::.111010:.:::
        // Reko: a decoder for V850 instruction 5737 at address 001001FE has not been implemented.
        [Test]
        public void V850Dis_5737()
        {
            AssertCode("@@@", "5737");
        }

        // ................:....011001::::.
        // ................:.....::..:::::. sld_b
        // ...................:.011110:....
        // ...................:..::::.:.... sst_b
        // ................:.::.010101.:.:.
        // Reko: a decoder for V850 instruction AAB2 at address 00100204 has not been implemented.
        [Test]
        public void V850Dis_AAB2()
        {
            AssertCode("@@@", "AAB2");
        }

        // ................::::.010000:::::
        // ................::::..:....11111
        // ................::::..:....::::: mov
        // ................::.::000010::::.
        // ................11011....:.::::.
        // ................::.::....:.11110
        // ................::.::....:.::::. divh
        // ................:.:::010110::..:
        // Reko: a decoder for V850 instruction D9BA at address 0010020A has not been implemented.
        [Test]
        public void V850Dis_D9BA()
        {
            AssertCode("@@@", "D9BA");
        }

        // ................:.:.:011001:::..
        // ................:.:.:.::..::::.. sld_b
        // ................::..:111011..::.
        // Reko: a decoder for V850 instruction 66CF at address 0010020E has not been implemented.
        [Test]
        public void V850Dis_66CF()
        {
            AssertCode("@@@", "66CF");
        }

        // ...................::000101..::.
        // Reko: a decoder for V850 instruction A618 at address 00100210 has not been implemented.
        [Test]
        public void V850Dis_A618()
        {
            AssertCode("@@@", "A618");
        }

        // ................::.::011100.:::.
        // ................::.::.:::...:::. sst_b
        // ................:::.:111011.::..
        // Reko: a decoder for V850 instruction 6CEF at address 00100214 has not been implemented.
        [Test]
        public void V850Dis_6CEF()
        {
            AssertCode("@@@", "6CEF");
        }

        // ................:..::110101....:
        // Reko: a decoder for V850 instruction A19E at address 00100216 has not been implemented.
        [Test]
        public void V850Dis_A19E()
        {
            AssertCode("@@@", "A19E");
        }

        // ................:::::001000.::.:
        // ................:::::..:....::.: or
        // ..................:.:110100:..::
        // Reko: a decoder for V850 instruction 932E at address 0010021A has not been implemented.
        [Test]
        public void V850Dis_932E()
        {
            AssertCode("@@@", "932E");
        }

        // ................:::::111100:.:.:
        // Reko: a decoder for V850 instruction 95FF at address 0010021C has not been implemented.
        [Test]
        public void V850Dis_95FF()
        {
            AssertCode("@@@", "95FF");
        }

        // .....................111001:::::
        // Reko: a decoder for V850 instruction 3F07 at address 0010021E has not been implemented.
        [Test]
        public void V850Dis_3F07()
        {
            AssertCode("@@@", "3F07");
        }

        // ................:.:::101111:::..
        // Reko: a decoder for V850 instruction FCBD at address 00100220 has not been implemented.
        [Test]
        public void V850Dis_FCBD()
        {
            AssertCode("@@@", "FCBD");
        }

        // .................:.::101110.....
        // Reko: a decoder for V850 instruction C05D at address 00100222 has not been implemented.
        [Test]
        public void V850Dis_C05D()
        {
            AssertCode("@@@", "C05D");
        }

        // ................:....001110..:..
        // Reko: a decoder for V850 instruction C481 at address 00100224 has not been implemented.
        [Test]
        public void V850Dis_C481()
        {
            AssertCode("@@@", "C481");
        }

        // .................:..:011101::.::
        // .................:..:.:::.:::.:: sst_b
        // ................:....100010:..:.
        // ................:....:...:.:..:. sld_h
        // ..................::.010101::::.
        // Reko: a decoder for V850 instruction BE32 at address 0010022A has not been implemented.
        [Test]
        public void V850Dis_BE32()
        {
            AssertCode("@@@", "BE32");
        }

        // .................:::.001011.::..
        // Reko: a decoder for V850 instruction 6C71 at address 0010022C has not been implemented.
        [Test]
        public void V850Dis_6C71()
        {
            AssertCode("@@@", "6C71");
        }

        // ................:::..000111..:..
        // Reko: a decoder for V850 instruction E4E0 at address 0010022E has not been implemented.
        [Test]
        public void V850Dis_E4E0()
        {
            AssertCode("@@@", "E4E0");
        }

        // ................::..:000000.::..
        // ................11001.......::..
        // ................::..:.......::.. mov
        // ..................:..011110.::.:
        // ..................:...::::..::.: sst_b
        // .................::::011001.....
        // .................::::.::..:..... sld_b
        // .................:..:111110.:.::
        // Reko: a decoder for V850 instruction CB4F at address 00100236 has not been implemented.
        [Test]
        public void V850Dis_CB4F()
        {
            AssertCode("@@@", "CB4F");
        }

        // ..................::.001111....:
        // Reko: a decoder for V850 instruction E131 at address 00100238 has not been implemented.
        [Test]
        public void V850Dis_E131()
        {
            AssertCode("@@@", "E131");
        }

        // ................:.::.000000..::.
        // ................10110........::.
        // ................:.::.........::. mov
        // ................::::.001000:::::
        // ................::::...:...::::: or
        // .................:.::111010:::..
        // Reko: a decoder for V850 instruction 5C5F at address 0010023E has not been implemented.
        [Test]
        public void V850Dis_5C5F()
        {
            AssertCode("@@@", "5C5F");
        }

        // ................::.:.010100::..:
        // Reko: a decoder for V850 instruction 99D2 at address 00100240 has not been implemented.
        [Test]
        public void V850Dis_99D2()
        {
            AssertCode("@@@", "99D2");
        }

        // .................::::110101::::.
        // Reko: a decoder for V850 instruction BE7E at address 00100242 has not been implemented.
        [Test]
        public void V850Dis_BE7E()
        {
            AssertCode("@@@", "BE7E");
        }

        // ....................:001111.:.::
        // Reko: a decoder for V850 instruction EB09 at address 00100244 has not been implemented.
        [Test]
        public void V850Dis_EB09()
        {
            AssertCode("@@@", "EB09");
        }

        // ..................:..101000.:.::
        // ..................:..:.:....:.:1 Sld.w/Sst.w
        // ..................:..:.:....:.:: sst_w
        // .................:...011010:::.:
        // .................:....::.:.:::.: sld_b
        // ................:::..111100.:..:
        // Reko: a decoder for V850 instruction 89E7 at address 0010024A has not been implemented.
        [Test]
        public void V850Dis_89E7()
        {
            AssertCode("@@@", "89E7");
        }

        // .................::.:111001:...:
        // Reko: a decoder for V850 instruction 316F at address 0010024C has not been implemented.
        [Test]
        public void V850Dis_316F()
        {
            AssertCode("@@@", "316F");
        }

        // ................:..::011001:..:.
        // ................:..::.::..::..:. sld_b
        // ................:.::.010000:.:.:
        // ................:.::..:....10101
        // ................:.::..:....:.:.: mov
        // ................::..:101001....:
        // Reko: a decoder for V850 instruction 21CD at address 00100252 has not been implemented.
        [Test]
        public void V850Dis_21CD()
        {
            AssertCode("@@@", "21CD");
        }

        // ...................::011001:..:.
        // ...................::.::..::..:. sld_b
        // ................:....000111..::.
        // Reko: a decoder for V850 instruction E680 at address 00100256 has not been implemented.
        [Test]
        public void V850Dis_E680()
        {
            AssertCode("@@@", "E680");
        }

        // .................:::.001111:..:.
        // Reko: a decoder for V850 instruction F271 at address 00100258 has not been implemented.
        [Test]
        public void V850Dis_F271()
        {
            AssertCode("@@@", "F271");
        }

        // ................:..:.010011::.::
        // Reko: a decoder for V850 instruction 7B92 at address 0010025A has not been implemented.
        [Test]
        public void V850Dis_7B92()
        {
            AssertCode("@@@", "7B92");
        }

        // ....................:010011::.:.
        // Reko: a decoder for V850 instruction 7A0A at address 0010025C has not been implemented.
        [Test]
        public void V850Dis_7A0A()
        {
            AssertCode("@@@", "7A0A");
        }

        // .................::..000001.::..
        // .................::.......:.::.. not
        // .................:.::111001..:::
        // Reko: a decoder for V850 instruction 275F at address 00100260 has not been implemented.
        [Test]
        public void V850Dis_275F()
        {
            AssertCode("@@@", "275F");
        }

        // ................:..::010100::...
        // Reko: a decoder for V850 instruction 989A at address 00100262 has not been implemented.
        [Test]
        public void V850Dis_989A()
        {
            AssertCode("@@@", "989A");
        }

        // .................::..110101:..::
        // Reko: a decoder for V850 instruction B366 at address 00100264 has not been implemented.
        [Test]
        public void V850Dis_B366()
        {
            AssertCode("@@@", "B366");
        }

        // .....................011111:.:.:
        // ......................::::::.:.: sst_b
        // ................:.:..111101:::..
        // Reko: a decoder for V850 instruction BCA7 at address 00100268 has not been implemented.
        [Test]
        public void V850Dis_BCA7()
        {
            AssertCode("@@@", "BCA7");
        }

        // ................::.::100110..::.
        // ................::.:::..::...::. sst_h
        // ................:::..011111:..:.
        // ................:::...::::::..:. sst_b
        // .................:::.010000:::.:
        // .................:::..:....11101
        // .................:::..:....:::.: mov
        // .................::.:010111:::::
        // Reko: a decoder for V850 instruction FF6A at address 00100270 has not been implemented.
        [Test]
        public void V850Dis_FF6A()
        {
            AssertCode("@@@", "FF6A");
        }

        // .................::.:100100::.::
        // .................::.::..:..::.:: sst_h
        // ................:...:111110:....
        // Reko: a decoder for V850 instruction D08F at address 00100274 has not been implemented.
        [Test]
        public void V850Dis_D08F()
        {
            AssertCode("@@@", "D08F");
        }

        // .................::::110001.::.:
        // Reko: a decoder for V850 instruction 2D7E at address 00100276 has not been implemented.
        [Test]
        public void V850Dis_2D7E()
        {
            AssertCode("@@@", "2D7E");
        }

        // ..................:::110111.:.::
        // Reko: a decoder for V850 instruction EB3E at address 00100278 has not been implemented.
        [Test]
        public void V850Dis_EB3E()
        {
            AssertCode("@@@", "EB3E");
        }

        // .................::::001100.:.::
        // Reko: a decoder for V850 instruction 8B79 at address 0010027A has not been implemented.
        [Test]
        public void V850Dis_8B79()
        {
            AssertCode("@@@", "8B79");
        }

        // .................::::000100.::.:
        // Reko: a decoder for V850 instruction 8D78 at address 0010027C has not been implemented.
        [Test]
        public void V850Dis_8D78()
        {
            AssertCode("@@@", "8D78");
        }

        // .................:.:.010101.:.::
        // Reko: a decoder for V850 instruction AB52 at address 0010027E has not been implemented.
        [Test]
        public void V850Dis_AB52()
        {
            AssertCode("@@@", "AB52");
        }

        // ..................:.:001000:.:::
        // ..................:.:..:...:.::: or
        // ................::.:.010110.....
        // Reko: a decoder for V850 instruction C0D2 at address 00100282 has not been implemented.
        [Test]
        public void V850Dis_C0D2()
        {
            AssertCode("@@@", "C0D2");
        }

        // ................:..:.111110:.::.
        // ................:.:.:110111.:...
        // Reko: a decoder for V850 instruction E8AE at address 00100286 has not been implemented.
        [Test]
        public void V850Dis_E8AE()
        {
            AssertCode("@@@", "E8AE");
        }

        // ...................::101101:::.:
        // Reko: a decoder for V850 instruction BD1D at address 00100288 has not been implemented.
        [Test]
        public void V850Dis_BD1D()
        {
            AssertCode("@@@", "BD1D");
        }

        // ................:...:010000:....
        // ................:...:.:....10000
        // ................:...:.:....:.... mov
        // ....................:010010:.:..
        // Reko: a decoder for V850 instruction 540A at address 0010028C has not been implemented.
        [Test]
        public void V850Dis_540A()
        {
            AssertCode("@@@", "540A");
        }

        // ....................:111100.:::.
        // Reko: a decoder for V850 instruction 8E0F at address 0010028E has not been implemented.
        [Test]
        public void V850Dis_8E0F()
        {
            AssertCode("@@@", "8E0F");
        }

        // ....................:001010:..::
        // Reko: a decoder for V850 instruction 5309 at address 00100290 has not been implemented.
        [Test]
        public void V850Dis_5309()
        {
            AssertCode("@@@", "5309");
        }

        // ...................:.000100:..::
        // Reko: a decoder for V850 instruction 9310 at address 00100292 has not been implemented.
        [Test]
        public void V850Dis_9310()
        {
            AssertCode("@@@", "9310");
        }

        // ................:.:::111011.:::.
        // Reko: a decoder for V850 instruction 6EBF at address 00100294 has not been implemented.
        [Test]
        public void V850Dis_6EBF()
        {
            AssertCode("@@@", "6EBF");
        }

        // ................:..:.111100.::::
        // ...................::010001:...:
        // Reko: a decoder for V850 instruction 311A at address 00100298 has not been implemented.
        [Test]
        public void V850Dis_311A()
        {
            AssertCode("@@@", "311A");
        }

        // .................:::.100101.::::
        // .................:::.:..:.:.:::: sst_h
        // .................::::011110::.:.
        // .................::::.::::.::.:. sst_b
        // .................:...100011:.::.
        // .................:...:...:::.::. sld_h
        // .................:::.100111:::..
        // .................:::.:..::::::.. sst_h
        // ................::..:000110..:.:
        // Reko: a decoder for V850 instruction C5C8 at address 001002A2 has not been implemented.
        [Test]
        public void V850Dis_C5C8()
        {
            AssertCode("@@@", "C5C8");
        }

        // .................:..:011001...::
        // .................:..:.::..:...:: sld_b
        // .................::..111100:::..
        // Reko: a decoder for V850 instruction 9C67 at address 001002A6 has not been implemented.
        [Test]
        public void V850Dis_9C67()
        {
            AssertCode("@@@", "9C67");
        }

        // ................:::.:100100...:.
        // ................:::.::..:.....:. sst_h
        // ................:.::.010101.:::.
        // Reko: a decoder for V850 instruction AEB2 at address 001002AA has not been implemented.
        [Test]
        public void V850Dis_AEB2()
        {
            AssertCode("@@@", "AEB2");
        }

        // ................:.:..010011:::::
        // Reko: a decoder for V850 instruction 7FA2 at address 001002AC has not been implemented.
        [Test]
        public void V850Dis_7FA2()
        {
            AssertCode("@@@", "7FA2");
        }

        // ................:::..001001.:..:
        // Reko: a decoder for V850 instruction 29E1 at address 001002AE has not been implemented.
        [Test]
        public void V850Dis_29E1()
        {
            AssertCode("@@@", "29E1");
        }

        // .................::::011101:....
        // .................::::.:::.::.... sst_b
        // ................:....011001..::.
        // ................:.....::..:..::. sld_b
        // ................:..::010111..:.:
        // Reko: a decoder for V850 instruction E59A at address 001002B4 has not been implemented.
        [Test]
        public void V850Dis_E59A()
        {
            AssertCode("@@@", "E59A");
        }

        // .................:.:.111111...::
        // Reko: a decoder for V850 instruction E357 at address 001002B6 has not been implemented.
        [Test]
        public void V850Dis_E357()
        {
            AssertCode("@@@", "E357");
        }

        // ..................:.:000100::..:
        // Reko: a decoder for V850 instruction 9928 at address 001002B8 has not been implemented.
        [Test]
        public void V850Dis_9928()
        {
            AssertCode("@@@", "9928");
        }

        // ................:.:::011101.....
        // ................:.:::.:::.:..... sst_b
        // .................:::.011111.:...
        // .................:::..:::::.:... sst_b
        // .................:...010110:.:..
        // Reko: a decoder for V850 instruction D442 at address 001002BE has not been implemented.
        [Test]
        public void V850Dis_D442()
        {
            AssertCode("@@@", "D442");
        }

        // ................::..:100101..:::
        // ................::..::..:.:..::: sst_h
        // ................:::..000100.:::.
        // Reko: a decoder for V850 instruction 8EE0 at address 001002C2 has not been implemented.
        [Test]
        public void V850Dis_8EE0()
        {
            AssertCode("@@@", "8EE0");
        }

        // ................::...101010..:..
        // Reko: a decoder for V850 instruction 44C5 at address 001002C4 has not been implemented.
        [Test]
        public void V850Dis_44C5()
        {
            AssertCode("@@@", "44C5");
        }

        // ....................:000111.::..
        // Reko: a decoder for V850 instruction EC08 at address 001002C6 has not been implemented.
        [Test]
        public void V850Dis_EC08()
        {
            AssertCode("@@@", "EC08");
        }

        // ..................:.:011011.::.:
        // ..................:.:.::.::.::.: sld_b
        // ................:....010100.:::.
        // Reko: a decoder for V850 instruction 8E82 at address 001002CA has not been implemented.
        [Test]
        public void V850Dis_8E82()
        {
            AssertCode("@@@", "8E82");
        }

        // ................::.::101000:...:
        // ................::.:::.:...:...1 Sld.w/Sst.w
        // ................::.:::.:...:...: sst_w
        // ................:...:111000::...
        // Reko: a decoder for V850 instruction 188F at address 001002CE has not been implemented.
        [Test]
        public void V850Dis_188F()
        {
            AssertCode("@@@", "188F");
        }

        // ................::..:110111:...:
        // Reko: a decoder for V850 instruction F1CE at address 001002D0 has not been implemented.
        [Test]
        public void V850Dis_F1CE()
        {
            AssertCode("@@@", "F1CE");
        }

        // .................:.::111110..:..
        // Reko: a decoder for V850 instruction C45F at address 001002D2 has not been implemented.
        [Test]
        public void V850Dis_C45F()
        {
            AssertCode("@@@", "C45F");
        }

        // ................::::.010010:.:..
        // Reko: a decoder for V850 instruction 54F2 at address 001002D4 has not been implemented.
        [Test]
        public void V850Dis_54F2()
        {
            AssertCode("@@@", "54F2");
        }

        // .................::..001010::::.
        // Reko: a decoder for V850 instruction 5E61 at address 001002D6 has not been implemented.
        [Test]
        public void V850Dis_5E61()
        {
            AssertCode("@@@", "5E61");
        }

        // .................::.:101100:....
        // Reko: a decoder for V850 instruction 906D at address 001002D8 has not been implemented.
        [Test]
        public void V850Dis_906D()
        {
            AssertCode("@@@", "906D");
        }

        // ................:::::010001.::::
        // Reko: a decoder for V850 instruction 2FFA at address 001002DA has not been implemented.
        [Test]
        public void V850Dis_2FFA()
        {
            AssertCode("@@@", "2FFA");
        }

        // ................:::..000101.:.:.
        // Reko: a decoder for V850 instruction AAE0 at address 001002DC has not been implemented.
        [Test]
        public void V850Dis_AAE0()
        {
            AssertCode("@@@", "AAE0");
        }

        // ....................:111011:::::
        // Reko: a decoder for V850 instruction 7F0F at address 001002DE has not been implemented.
        [Test]
        public void V850Dis_7F0F()
        {
            AssertCode("@@@", "7F0F");
        }

        // .................::.:011000::...
        // .................::.:.::...::... sld_b
        // ................:.::.011000.:.:.
        // ................:.::..::....:.:. sld_b
        // .................:..:001000:..::
        // .................:..:..:...:..:: or
        // ................:::.:010100:::..
        // Reko: a decoder for V850 instruction 9CEA at address 001002E6 has not been implemented.
        [Test]
        public void V850Dis_9CEA()
        {
            AssertCode("@@@", "9CEA");
        }

        // ...................::101100.::::
        // Reko: a decoder for V850 instruction 8F1D at address 001002E8 has not been implemented.
        [Test]
        public void V850Dis_8F1D()
        {
            AssertCode("@@@", "8F1D");
        }

        // ................:.:::001000:.::.
        // ................:.:::..:...:.::. or
        // .................::..001000.::..
        // .................::....:....::.. or
        // ................:.:::100010..::.
        // ................:.::::...:...::. sld_h
        // ...................:.001000.::::
        // ...................:...:....:::: or
        // ...................::010110.:.:.
        // Reko: a decoder for V850 instruction CA1A at address 001002F2 has not been implemented.
        [Test]
        public void V850Dis_CA1A()
        {
            AssertCode("@@@", "CA1A");
        }

        // .................:::.101101.:::.
        // Reko: a decoder for V850 instruction AE75 at address 001002F4 has not been implemented.
        [Test]
        public void V850Dis_AE75()
        {
            AssertCode("@@@", "AE75");
        }

        // ................:.::.101111.::.:
        // Reko: a decoder for V850 instruction EDB5 at address 001002F6 has not been implemented.
        [Test]
        public void V850Dis_EDB5()
        {
            AssertCode("@@@", "EDB5");
        }

        // ..................:.:101011.::.:
        // Reko: a decoder for V850 instruction 6D2D at address 001002F8 has not been implemented.
        [Test]
        public void V850Dis_6D2D()
        {
            AssertCode("@@@", "6D2D");
        }

        // ................::.:.111010.::::
        // Reko: a decoder for V850 instruction 4FD7 at address 001002FA has not been implemented.
        [Test]
        public void V850Dis_4FD7()
        {
            AssertCode("@@@", "4FD7");
        }

        // ................::::.001100:.::.
        // Reko: a decoder for V850 instruction 96F1 at address 001002FC has not been implemented.
        [Test]
        public void V850Dis_96F1()
        {
            AssertCode("@@@", "96F1");
        }

        // ................:...:100010:::.:
        // ................:...::...:.:::.: sld_h
        // .................::::111101.::..
        // Reko: a decoder for V850 instruction AC7F at address 00100300 has not been implemented.
        [Test]
        public void V850Dis_AC7F()
        {
            AssertCode("@@@", "AC7F");
        }

        // ................:.:..101011.:..:
        // Reko: a decoder for V850 instruction 69A5 at address 00100302 has not been implemented.
        [Test]
        public void V850Dis_69A5()
        {
            AssertCode("@@@", "69A5");
        }

        // .................::..101110:..:.
        // Reko: a decoder for V850 instruction D265 at address 00100304 has not been implemented.
        [Test]
        public void V850Dis_D265()
        {
            AssertCode("@@@", "D265");
        }

        // ................:.:..111011::.::
        // Reko: a decoder for V850 instruction 7BA7 at address 00100306 has not been implemented.
        [Test]
        public void V850Dis_7BA7()
        {
            AssertCode("@@@", "7BA7");
        }

        // ................::.::001001:...:
        // Reko: a decoder for V850 instruction 31D9 at address 00100308 has not been implemented.
        [Test]
        public void V850Dis_31D9()
        {
            AssertCode("@@@", "31D9");
        }

        // ..................::.111110.::::
        // Reko: a decoder for V850 instruction CF37 at address 0010030A has not been implemented.
        [Test]
        public void V850Dis_CF37()
        {
            AssertCode("@@@", "CF37");
        }

        // ................:.:..100110::.::
        // ................:.:..:..::.::.:: sst_h
        // ..................:.:111010:.:..
        // Reko: a decoder for V850 instruction 542F at address 0010030E has not been implemented.
        [Test]
        public void V850Dis_542F()
        {
            AssertCode("@@@", "542F");
        }

        // .................:::.011001..::.
        // .................:::..::..:..::. sld_b
        // ................:..::001111.:.::
        // Reko: a decoder for V850 instruction EB99 at address 00100312 has not been implemented.
        [Test]
        public void V850Dis_EB99()
        {
            AssertCode("@@@", "EB99");
        }

        // .................::..100000:.::.
        // .................::..:.....:.::. sld_h
        // ................:.:..011011.....
        // ................:.:...::.::..... sld_b
        // .................:.:.101011::::.
        // Reko: a decoder for V850 instruction 7E55 at address 00100318 has not been implemented.
        [Test]
        public void V850Dis_7E55()
        {
            AssertCode("@@@", "7E55");
        }

        // ................:::..110010..:.:
        // Reko: a decoder for V850 instruction 45E6 at address 0010031A has not been implemented.
        [Test]
        public void V850Dis_45E6()
        {
            AssertCode("@@@", "45E6");
        }

        // ................::...100111::.:.
        // ................::...:..:::::.:. sst_h
        // ................:..::110010:.:..
        // Reko: a decoder for V850 instruction 549E at address 0010031E has not been implemented.
        [Test]
        public void V850Dis_549E()
        {
            AssertCode("@@@", "549E");
        }

        // ................:...:001110.....
        // Reko: a decoder for V850 instruction C089 at address 00100320 has not been implemented.
        [Test]
        public void V850Dis_C089()
        {
            AssertCode("@@@", "C089");
        }

        // ................:...:101000.::.:
        // ................:...::.:....::.1 Sld.w/Sst.w
        // ................:...::.:....::.: sst_w
        // ................:.:..000111:.:::
        // Reko: a decoder for V850 instruction F7A0 at address 00100324 has not been implemented.
        [Test]
        public void V850Dis_F7A0()
        {
            AssertCode("@@@", "F7A0");
        }

        // ................:.:.:010111.:..:
        // Reko: a decoder for V850 instruction E9AA at address 00100326 has not been implemented.
        [Test]
        public void V850Dis_E9AA()
        {
            AssertCode("@@@", "E9AA");
        }

        // ..................:..010100:.:.:
        // Reko: a decoder for V850 instruction 9522 at address 00100328 has not been implemented.
        [Test]
        public void V850Dis_9522()
        {
            AssertCode("@@@", "9522");
        }

        // ................::.:.100111.:...
        // ................::.:.:..:::.:... sst_h
        // ................:.::.110101..:.:
        // Reko: a decoder for V850 instruction A5B6 at address 0010032C has not been implemented.
        [Test]
        public void V850Dis_A5B6()
        {
            AssertCode("@@@", "A5B6");
        }

        // ................::..:000110::..:
        // Reko: a decoder for V850 instruction D9C8 at address 0010032E has not been implemented.
        [Test]
        public void V850Dis_D9C8()
        {
            AssertCode("@@@", "D9C8");
        }

        // ...................::111110::...
        // Reko: a decoder for V850 instruction D81F at address 00100330 has not been implemented.
        [Test]
        public void V850Dis_D81F()
        {
            AssertCode("@@@", "D81F");
        }

        // ..................:..010101:....
        // Reko: a decoder for V850 instruction B022 at address 00100332 has not been implemented.
        [Test]
        public void V850Dis_B022()
        {
            AssertCode("@@@", "B022");
        }

        // ................::...100000..::.
        // ................::...:.......::. sld_h
        // .................:...111011:.::.
        // Reko: a decoder for V850 instruction 7647 at address 00100336 has not been implemented.
        [Test]
        public void V850Dis_7647()
        {
            AssertCode("@@@", "7647");
        }

        // ................::...110000::::.
        // Reko: a decoder for V850 instruction 1EC6 at address 00100338 has not been implemented.
        [Test]
        public void V850Dis_1EC6()
        {
            AssertCode("@@@", "1EC6");
        }

        // .................::::101001..:::
        // Reko: a decoder for V850 instruction 277D at address 0010033A has not been implemented.
        [Test]
        public void V850Dis_277D()
        {
            AssertCode("@@@", "277D");
        }

        // ................:..:.100000:::::
        // ................:..:.:.....::::: sld_h
        // .................::.:101101.::.:
        // Reko: a decoder for V850 instruction AD6D at address 0010033E has not been implemented.
        [Test]
        public void V850Dis_AD6D()
        {
            AssertCode("@@@", "AD6D");
        }

        // ..................::.001100..:.:
        // Reko: a decoder for V850 instruction 8531 at address 00100340 has not been implemented.
        [Test]
        public void V850Dis_8531()
        {
            AssertCode("@@@", "8531");
        }

        // .................:.:.001011:.::.
        // Reko: a decoder for V850 instruction 7651 at address 00100342 has not been implemented.
        [Test]
        public void V850Dis_7651()
        {
            AssertCode("@@@", "7651");
        }

        // ................::...111100:.:::
        // Reko: a decoder for V850 instruction 97C7 at address 00100344 has not been implemented.
        [Test]
        public void V850Dis_97C7()
        {
            AssertCode("@@@", "97C7");
        }

        // ................:....110001::...
        // Reko: a decoder for V850 instruction 3886 at address 00100346 has not been implemented.
        [Test]
        public void V850Dis_3886()
        {
            AssertCode("@@@", "3886");
        }

        // .................:...000100.:..:
        // Reko: a decoder for V850 instruction 8940 at address 00100348 has not been implemented.
        [Test]
        public void V850Dis_8940()
        {
            AssertCode("@@@", "8940");
        }

        // ................::::.011000...::
        // ................::::..::......:: sld_b
        // ................:...:100011:::..
        // ................:...::...:::::.. sld_h
        // ................::...111111:::.:
        // Reko: a decoder for V850 instruction FDC7 at address 0010034E has not been implemented.
        [Test]
        public void V850Dis_FDC7()
        {
            AssertCode("@@@", "FDC7");
        }

        // .................::::100011::.::
        // .................:::::...::::.:: sld_h
        // ................::.::010000.:::.
        // ................::.::.:....01110
        // ................::.::.:.....:::. mov
        // ..................::.001000:.:..
        // ..................::...:...:.:.. or
        // ................:::::011100:.:::
        // ................:::::.:::..:.::: sst_b
        // ................::...101000:..::
        // ................::...:.:...:..:1 Sld.w/Sst.w
        // ................::...:.:...:..:: sst_w
        // ..................::.000011.::::
        // Reko: a decoder for V850 instruction 6F30 at address 0010035A has not been implemented.
        [Test]
        public void V850Dis_6F30()
        {
            AssertCode("@@@", "6F30");
        }

        // .................::.:100011::..:
        // .................::.::...::::..: sld_h
        // .....................001100.:.::
        // Reko: a decoder for V850 instruction 8B01 at address 0010035E has not been implemented.
        [Test]
        public void V850Dis_8B01()
        {
            AssertCode("@@@", "8B01");
        }

        // .................::::011011:.:.:
        // .................::::.::.:::.:.: sld_b
        // .................:::.100010.::::
        // .................:::.:...:..:::: sld_h
        // ................:....111001:::::
        // Reko: a decoder for V850 instruction 3F87 at address 00100364 has not been implemented.
        [Test]
        public void V850Dis_3F87()
        {
            AssertCode("@@@", "3F87");
        }

        // ...................:.001001:...:
        // Reko: a decoder for V850 instruction 3111 at address 00100366 has not been implemented.
        [Test]
        public void V850Dis_3111()
        {
            AssertCode("@@@", "3111");
        }

        // ..................:.:010111..::.
        // Reko: a decoder for V850 instruction E62A at address 00100368 has not been implemented.
        [Test]
        public void V850Dis_E62A()
        {
            AssertCode("@@@", "E62A");
        }

        // ................::...101100::..:
        // Reko: a decoder for V850 instruction 99C5 at address 0010036A has not been implemented.
        [Test]
        public void V850Dis_99C5()
        {
            AssertCode("@@@", "99C5");
        }

        // ................:....010110.....
        // Reko: a decoder for V850 instruction C082 at address 0010036C has not been implemented.
        [Test]
        public void V850Dis_C082()
        {
            AssertCode("@@@", "C082");
        }

        // ................:..:.010110.:.::
        // Reko: a decoder for V850 instruction CB92 at address 0010036E has not been implemented.
        [Test]
        public void V850Dis_CB92()
        {
            AssertCode("@@@", "CB92");
        }

        // .................:.::111110.:...
        // Reko: a decoder for V850 instruction C85F at address 00100370 has not been implemented.
        [Test]
        public void V850Dis_C85F()
        {
            AssertCode("@@@", "C85F");
        }

        // ................:.:..010000....:
        // ................:.:...:....00001
        // ................:.:...:........: mov
        // ................::.:.010100..:.:
        // Reko: a decoder for V850 instruction 85D2 at address 00100374 has not been implemented.
        [Test]
        public void V850Dis_85D2()
        {
            AssertCode("@@@", "85D2");
        }

        // .................:.:.011010::..:
        // .................:.:..::.:.::..: sld_b
        // .................::::010100::..:
        // Reko: a decoder for V850 instruction 997A at address 00100378 has not been implemented.
        [Test]
        public void V850Dis_997A()
        {
            AssertCode("@@@", "997A");
        }

        // ................::.:.001001::::.
        // Reko: a decoder for V850 instruction 3ED1 at address 0010037A has not been implemented.
        [Test]
        public void V850Dis_3ED1()
        {
            AssertCode("@@@", "3ED1");
        }

        // .....................111010:.:::
        // Reko: a decoder for V850 instruction 5707 at address 0010037C has not been implemented.
        [Test]
        public void V850Dis_5707()
        {
            AssertCode("@@@", "5707");
        }

        // ...................::101000.::..
        // ...................:::.:....::.0 Sld.w/Sst.w
        // ...................:::.:....::.. sld_w
        // .....................010101:.:.:
        // Reko: a decoder for V850 instruction B502 at address 00100380 has not been implemented.
        [Test]
        public void V850Dis_B502()
        {
            AssertCode("@@@", "B502");
        }

        // .....................000011:::.:
        // Reko: a decoder for V850 instruction 7D00 at address 00100382 has not been implemented.
        [Test]
        public void V850Dis_7D00()
        {
            AssertCode("@@@", "7D00");
        }

        // ................:...:001001:::..
        // Reko: a decoder for V850 instruction 3C89 at address 00100384 has not been implemented.
        [Test]
        public void V850Dis_3C89()
        {
            AssertCode("@@@", "3C89");
        }

        // ................::::.011100..:::
        // ................::::..:::....::: sst_b
        // ................:::::101010.:.::
        // Reko: a decoder for V850 instruction 4BFD at address 00100388 has not been implemented.
        [Test]
        public void V850Dis_4BFD()
        {
            AssertCode("@@@", "4BFD");
        }

        // ................:::.:001111:.:..
        // Reko: a decoder for V850 instruction F4E9 at address 0010038A has not been implemented.
        [Test]
        public void V850Dis_F4E9()
        {
            AssertCode("@@@", "F4E9");
        }

        // ................::.:.010100::...
        // Reko: a decoder for V850 instruction 98D2 at address 0010038C has not been implemented.
        [Test]
        public void V850Dis_98D2()
        {
            AssertCode("@@@", "98D2");
        }

        // ................:..:.000001::.::
        // ................:..:......:::.:: not
        // ................::.::100111::..:
        // ................::.:::..:::::..: sst_h
        // ...................::001011.:...
        // Reko: a decoder for V850 instruction 6819 at address 00100392 has not been implemented.
        [Test]
        public void V850Dis_6819()
        {
            AssertCode("@@@", "6819");
        }

        // ................:::.:001011.:.:.
        // Reko: a decoder for V850 instruction 6AE9 at address 00100394 has not been implemented.
        [Test]
        public void V850Dis_6AE9()
        {
            AssertCode("@@@", "6AE9");
        }

        // ................:.:..011011::.::
        // ................:.:...::.::::.:: sld_b
        // ................:::::100001...:.
        // ................::::::....:...:. sld_h
        // ................::...100110::.:.
        // ................::...:..::.::.:. sst_h
        // .................:.::111010.:..:
        // Reko: a decoder for V850 instruction 495F at address 0010039C has not been implemented.
        [Test]
        public void V850Dis_495F()
        {
            AssertCode("@@@", "495F");
        }

        // ................::.::111101::..:
        // Reko: a decoder for V850 instruction B9DF at address 0010039E has not been implemented.
        [Test]
        public void V850Dis_B9DF()
        {
            AssertCode("@@@", "B9DF");
        }

        // .................::::011000::::.
        // .................::::.::...::::. sld_b
        // ................:.:::110000:....
        // Reko: a decoder for V850 instruction 10BE at address 001003A2 has not been implemented.
        [Test]
        public void V850Dis_10BE()
        {
            AssertCode("@@@", "10BE");
        }

        // ................::..:011000..:.:
        // ................::..:.::.....:.: sld_b
        // ..................:::110010:.:.:
        // Reko: a decoder for V850 instruction 553E at address 001003A6 has not been implemented.
        [Test]
        public void V850Dis_553E()
        {
            AssertCode("@@@", "553E");
        }

        // ....................:101110:.:::
        // Reko: a decoder for V850 instruction D70D at address 001003A8 has not been implemented.
        [Test]
        public void V850Dis_D70D()
        {
            AssertCode("@@@", "D70D");
        }

        // ................:...:000010:.:::
        // ................10001....:.:.:::
        // ................:...:....:.10111
        // ................:...:....:.:.::: divh
        // .................::.:111110:::.:
        // Reko: a decoder for V850 instruction DD6F at address 001003AC has not been implemented.
        [Test]
        public void V850Dis_DD6F()
        {
            AssertCode("@@@", "DD6F");
        }

        // ................::...111101::.:.
        // Reko: a decoder for V850 instruction BAC7 at address 001003AE has not been implemented.
        [Test]
        public void V850Dis_BAC7()
        {
            AssertCode("@@@", "BAC7");
        }

        // ................:.:.:101001:::::
        // Reko: a decoder for V850 instruction 3FAD at address 001003B0 has not been implemented.
        [Test]
        public void V850Dis_3FAD()
        {
            AssertCode("@@@", "3FAD");
        }

        // .................::::011110::...
        // .................::::.::::.::... sst_b
        // ................::::.010100:::::
        // Reko: a decoder for V850 instruction 9FF2 at address 001003B4 has not been implemented.
        [Test]
        public void V850Dis_9FF2()
        {
            AssertCode("@@@", "9FF2");
        }

        // ................::..:011101:..::
        // ................::..:.:::.::..:: sst_b
        // ................::.::001100..::.
        // Reko: a decoder for V850 instruction 86D9 at address 001003B8 has not been implemented.
        [Test]
        public void V850Dis_86D9()
        {
            AssertCode("@@@", "86D9");
        }

        // .................:...000110::::.
        // Reko: a decoder for V850 instruction DE40 at address 001003BA has not been implemented.
        [Test]
        public void V850Dis_DE40()
        {
            AssertCode("@@@", "DE40");
        }

        // ................::...011101.::::
        // ................::....:::.:.:::: sst_b
        // ................:::.:011101.:.:.
        // ................:::.:.:::.:.:.:. sst_b
        // ...................:.101010..:..
        // Reko: a decoder for V850 instruction 4415 at address 001003C0 has not been implemented.
        [Test]
        public void V850Dis_4415()
        {
            AssertCode("@@@", "4415");
        }

        // ...................::100110.:.::
        // ...................:::..::..:.:: sst_h
        // .................::::101110....:
        // Reko: a decoder for V850 instruction C17D at address 001003C4 has not been implemented.
        [Test]
        public void V850Dis_C17D()
        {
            AssertCode("@@@", "C17D");
        }

        // ..................:.:110100.:.::
        // Reko: a decoder for V850 instruction 8B2E at address 001003C6 has not been implemented.
        [Test]
        public void V850Dis_8B2E()
        {
            AssertCode("@@@", "8B2E");
        }

        // ..................:.:001100..:::
        // Reko: a decoder for V850 instruction 8729 at address 001003C8 has not been implemented.
        [Test]
        public void V850Dis_8729()
        {
            AssertCode("@@@", "8729");
        }

        // .................:.::100010...:.
        // .................:.:::...:....:. sld_h
        // ................::::.011100:...:
        // ................::::..:::..:...: sst_b
        // ................:.::.010110..:.:
        // Reko: a decoder for V850 instruction C5B2 at address 001003CE has not been implemented.
        [Test]
        public void V850Dis_C5B2()
        {
            AssertCode("@@@", "C5B2");
        }

        // ...................:.011010....:
        // ...................:..::.:.....: sld_b
        // ................:..::100100..:.:
        // ................:..:::..:....:.: sst_h
        // ..................:::110100..:::
        // Reko: a decoder for V850 instruction 873E at address 001003D4 has not been implemented.
        [Test]
        public void V850Dis_873E()
        {
            AssertCode("@@@", "873E");
        }

        // ..................:.:011001:::::
        // ..................:.:.::..:::::: sld_b
        // .................:...100110.:...
        // .................:...:..::..:... sst_h
        // ..................:.:011001:.:::
        // ..................:.:.::..::.::: sld_b
        // ...................:.101111.::..
        // Reko: a decoder for V850 instruction EC15 at address 001003DC has not been implemented.
        [Test]
        public void V850Dis_EC15()
        {
            AssertCode("@@@", "EC15");
        }

        // ...................:.100100.::::
        // ...................:.:..:...:::: sst_h
        // .................::.:011011...::
        // .................::.:.::.::...:: sld_b
        // ................::..:000010...::
        // ................11001....:....::
        // ................::..:....:.00011
        // ................::..:....:....:: divh
        // ................:..::101101...::
        // Reko: a decoder for V850 instruction A39D at address 001003E4 has not been implemented.
        [Test]
        public void V850Dis_A39D()
        {
            AssertCode("@@@", "A39D");
        }

        // ................::...001000..::.
        // ................::.....:.....::. or
        // ................:.:.:001001....:
        // Reko: a decoder for V850 instruction 21A9 at address 001003E8 has not been implemented.
        [Test]
        public void V850Dis_21A9()
        {
            AssertCode("@@@", "21A9");
        }

        // .................:::.110111:..::
        // Reko: a decoder for V850 instruction F376 at address 001003EA has not been implemented.
        [Test]
        public void V850Dis_F376()
        {
            AssertCode("@@@", "F376");
        }

        // .................:.:.111101:....
        // Reko: a decoder for V850 instruction B057 at address 001003EC has not been implemented.
        [Test]
        public void V850Dis_B057()
        {
            AssertCode("@@@", "B057");
        }

        // ................:..:.011001::.:.
        // ................:..:..::..:::.:. sld_b
        // ..................:.:011000:.:..
        // ..................:.:.::...:.:.. sld_b
        // ................:..::100111::::.
        // ................:..:::..:::::::. sst_h
        // ....................:110001:::.:
        // Reko: a decoder for V850 instruction 3D0E at address 001003F4 has not been implemented.
        [Test]
        public void V850Dis_3D0E()
        {
            AssertCode("@@@", "3D0E");
        }

        // .....................110011...::
        // Reko: a decoder for V850 instruction 6306 at address 001003F6 has not been implemented.
        [Test]
        public void V850Dis_6306()
        {
            AssertCode("@@@", "6306");
        }

        // ................:.:..000110:.:.:
        // Reko: a decoder for V850 instruction D5A0 at address 001003F8 has not been implemented.
        [Test]
        public void V850Dis_D5A0()
        {
            AssertCode("@@@", "D5A0");
        }

        // .................::::100010:.:..
        // .................:::::...:.:.:.. sld_h
        // .................::..000010..::.
        // ................01100....:...::.
        // .................::......:.00110
        // .................::......:...::. divh
        // .................:::.010010....:
        // Reko: a decoder for V850 instruction 4172 at address 001003FE has not been implemented.
        [Test]
        public void V850Dis_4172()
        {
            AssertCode("@@@", "4172");
        }

        // ..................:.:101100::..:
        // Reko: a decoder for V850 instruction 992D at address 00100400 has not been implemented.
        [Test]
        public void V850Dis_992D()
        {
            AssertCode("@@@", "992D");
        }

        // ..................:..110111::...
        // Reko: a decoder for V850 instruction F826 at address 00100402 has not been implemented.
        [Test]
        public void V850Dis_F826()
        {
            AssertCode("@@@", "F826");
        }

        // ................:::::100101.::::
        // ................::::::..:.:.:::: sst_h
        // ................:.:..011000.::::
        // ................:.:...::....:::: sld_b
        // ................::...011000.::..
        // ................::....::....::.. sld_b
        // ................::.::110011::.:.
        // Reko: a decoder for V850 instruction 7ADE at address 0010040A has not been implemented.
        [Test]
        public void V850Dis_7ADE()
        {
            AssertCode("@@@", "7ADE");
        }

        // ................::..:000010.:.:.
        // ................11001....:..:.:.
        // ................::..:....:.01010
        // ................::..:....:..:.:. divh
        // .................:::.001011::.::
        // Reko: a decoder for V850 instruction 7B71 at address 0010040E has not been implemented.
        [Test]
        public void V850Dis_7B71()
        {
            AssertCode("@@@", "7B71");
        }

        // ................:.:..011000.:.:.
        // ................:.:...::....:.:. sld_b
        // ................::...001000:.:::
        // ................::.....:...:.::: or
        // ................::::.011000:.::.
        // ................::::..::...:.::. sld_b
        // .................:.:.101110:.::.
        // Reko: a decoder for V850 instruction D655 at address 00100416 has not been implemented.
        [Test]
        public void V850Dis_D655()
        {
            AssertCode("@@@", "D655");
        }

        // ..................:::101000..:::
        // ..................::::.:.....::1 Sld.w/Sst.w
        // ..................::::.:.....::: sst_w
        // .....................011111:..::
        // ......................::::::..:: sst_b
        // ................:...:001010.:...
        // Reko: a decoder for V850 instruction 4889 at address 0010041C has not been implemented.
        [Test]
        public void V850Dis_4889()
        {
            AssertCode("@@@", "4889");
        }

        // ................::...001011::.::
        // Reko: a decoder for V850 instruction 7BC1 at address 0010041E has not been implemented.
        [Test]
        public void V850Dis_7BC1()
        {
            AssertCode("@@@", "7BC1");
        }

        // ................:....001011..:::
        // Reko: a decoder for V850 instruction 6781 at address 00100420 has not been implemented.
        [Test]
        public void V850Dis_6781()
        {
            AssertCode("@@@", "6781");
        }

        // ................:....010110:::..
        // Reko: a decoder for V850 instruction DC82 at address 00100422 has not been implemented.
        [Test]
        public void V850Dis_DC82()
        {
            AssertCode("@@@", "DC82");
        }

        // ...................:.011010::::.
        // ...................:..::.:.::::. sld_b
        // ...................:.111111...::
        // Reko: a decoder for V850 instruction E317 at address 00100426 has not been implemented.
        [Test]
        public void V850Dis_E317()
        {
            AssertCode("@@@", "E317");
        }

        // .................:.::011000:::..
        // .................:.::.::...:::.. sld_b
        // .................::::001100.::::
        // Reko: a decoder for V850 instruction 8F79 at address 0010042A has not been implemented.
        [Test]
        public void V850Dis_8F79()
        {
            AssertCode("@@@", "8F79");
        }

        // ................:....101100:..::
        // Reko: a decoder for V850 instruction 9385 at address 0010042C has not been implemented.
        [Test]
        public void V850Dis_9385()
        {
            AssertCode("@@@", "9385");
        }

        // ....................:100101::.::
        // ....................::..:.:::.:: sst_h
        // ................:::..011001..:..
        // ................:::...::..:..:.. sld_b
        // ................:.:..010011:..:.
        // Reko: a decoder for V850 instruction 72A2 at address 00100432 has not been implemented.
        [Test]
        public void V850Dis_72A2()
        {
            AssertCode("@@@", "72A2");
        }

        // ................:....000010.:..:
        // ................10000....:..:..:
        // ................:........:.01001
        // ................:........:..:..: divh
        // ................:.:..101010::.::
        // Reko: a decoder for V850 instruction 5BA5 at address 00100436 has not been implemented.
        [Test]
        public void V850Dis_5BA5()
        {
            AssertCode("@@@", "5BA5");
        }

        // ................:.:..011010:.::.
        // ................:.:...::.:.:.::. sld_b
        // .................:::.001000::::.
        // .................:::...:...::::. or
        // ....................:011000.:..:
        // ....................:.::....:..: sld_b
        // ................:....011010::.::
        // ................:.....::.:.::.:: sld_b
        // ................:.:::001010.:...
        // Reko: a decoder for V850 instruction 48B9 at address 00100440 has not been implemented.
        [Test]
        public void V850Dis_48B9()
        {
            AssertCode("@@@", "48B9");
        }

        // ................::..:000011:.::.
        // Reko: a decoder for V850 instruction 76C8 at address 00100442 has not been implemented.
        [Test]
        public void V850Dis_76C8()
        {
            AssertCode("@@@", "76C8");
        }

        // ................:::::000111.:.::
        // Reko: a decoder for V850 instruction EBF8 at address 00100444 has not been implemented.
        [Test]
        public void V850Dis_EBF8()
        {
            AssertCode("@@@", "EBF8");
        }

        // ................::::.110000:..:.
        // Reko: a decoder for V850 instruction 12F6 at address 00100446 has not been implemented.
        [Test]
        public void V850Dis_12F6()
        {
            AssertCode("@@@", "12F6");
        }

        // ................:::::111101:::::
        // Reko: a decoder for V850 instruction BFFF at address 00100448 has not been implemented.
        [Test]
        public void V850Dis_BFFF()
        {
            AssertCode("@@@", "BFFF");
        }

        // ................:.:::011101..:..
        // ................:.:::.:::.:..:.. sst_b
        // .................:.::101011..:..
        // Reko: a decoder for V850 instruction 645D at address 0010044C has not been implemented.
        [Test]
        public void V850Dis_645D()
        {
            AssertCode("@@@", "645D");
        }

        // ................:....010110....:
        // Reko: a decoder for V850 instruction C182 at address 0010044E has not been implemented.
        [Test]
        public void V850Dis_C182()
        {
            AssertCode("@@@", "C182");
        }

        // ................:::..110100...:.
        // Reko: a decoder for V850 instruction 82E6 at address 00100450 has not been implemented.
        [Test]
        public void V850Dis_82E6()
        {
            AssertCode("@@@", "82E6");
        }

        // .................::..101110:::::
        // Reko: a decoder for V850 instruction DF65 at address 00100452 has not been implemented.
        [Test]
        public void V850Dis_DF65()
        {
            AssertCode("@@@", "DF65");
        }

        // ................::.::000000:.:::
        // ................11011......:.:::
        // ................::.::......:.::: mov
        // ................:::..110011:.:::
        // Reko: a decoder for V850 instruction 77E6 at address 00100456 has not been implemented.
        [Test]
        public void V850Dis_77E6()
        {
            AssertCode("@@@", "77E6");
        }

        // ..................::.110001..:.:
        // Reko: a decoder for V850 instruction 2536 at address 00100458 has not been implemented.
        [Test]
        public void V850Dis_2536()
        {
            AssertCode("@@@", "2536");
        }

        // ................:.:::011001.:.::
        // ................:.:::.::..:.:.:: sld_b
        // .................:::.010111:.:.:
        // Reko: a decoder for V850 instruction F572 at address 0010045C has not been implemented.
        [Test]
        public void V850Dis_F572()
        {
            AssertCode("@@@", "F572");
        }

        // ...................:.001000.::.:
        // ...................:...:....::.: or
        // ....................:011111:::::
        // ....................:.:::::::::: sst_b
        // ................::.::010001:...:
        // Reko: a decoder for V850 instruction 31DA at address 00100462 has not been implemented.
        [Test]
        public void V850Dis_31DA()
        {
            AssertCode("@@@", "31DA");
        }

        // ................::::.011000.:::.
        // ................::::..::....:::. sld_b
        // ..................:.:011001.....
        // ..................:.:.::..:..... sld_b
        // .................::::011110:...:
        // .................::::.::::.:...: sst_b
        // .................:..:001111...:.
        // Reko: a decoder for V850 instruction E249 at address 0010046A has not been implemented.
        [Test]
        public void V850Dis_E249()
        {
            AssertCode("@@@", "E249");
        }

        // ................:.:..000110::.::
        // Reko: a decoder for V850 instruction DBA0 at address 0010046C has not been implemented.
        [Test]
        public void V850Dis_DBA0()
        {
            AssertCode("@@@", "DBA0");
        }

        // ................:::::001010....:
        // Reko: a decoder for V850 instruction 41F9 at address 0010046E has not been implemented.
        [Test]
        public void V850Dis_41F9()
        {
            AssertCode("@@@", "41F9");
        }

        // ................:..::011111....:
        // ................:..::.:::::....: sst_b
        // ..................:..011111.::::
        // ..................:...:::::.:::: sst_b
        // ................::::.110001.::..
        // Reko: a decoder for V850 instruction 2CF6 at address 00100474 has not been implemented.
        [Test]
        public void V850Dis_2CF6()
        {
            AssertCode("@@@", "2CF6");
        }

        // .................::.:111011.::..
        // Reko: a decoder for V850 instruction 6C6F at address 00100476 has not been implemented.
        [Test]
        public void V850Dis_6C6F()
        {
            AssertCode("@@@", "6C6F");
        }

        // ................:..:.000010....:
        // ................10010....:.....:
        // ................:..:.....:.00001
        // ................:..:.....:.....: divh
        // ................:.::.101101...:.
        // Reko: a decoder for V850 instruction A2B5 at address 0010047A has not been implemented.
        [Test]
        public void V850Dis_A2B5()
        {
            AssertCode("@@@", "A2B5");
        }

        // .................:.:.111110.::.:
        // Reko: a decoder for V850 instruction CD57 at address 0010047C has not been implemented.
        [Test]
        public void V850Dis_CD57()
        {
            AssertCode("@@@", "CD57");
        }

        // .................:..:101000....:
        // .................:..::.:.......1 Sld.w/Sst.w
        // .................:..::.:.......: sst_w
        // ................:..:.011111:..:.
        // ................:..:..::::::..:. sst_b
        // .................:.::000101::.::
        // Reko: a decoder for V850 instruction BB58 at address 00100482 has not been implemented.
        [Test]
        public void V850Dis_BB58()
        {
            AssertCode("@@@", "BB58");
        }

        // ................:::..110001.:.::
        // Reko: a decoder for V850 instruction 2BE6 at address 00100484 has not been implemented.
        [Test]
        public void V850Dis_2BE6()
        {
            AssertCode("@@@", "2BE6");
        }

        // ................:...:111011:.:..
        // Reko: a decoder for V850 instruction 748F at address 00100486 has not been implemented.
        [Test]
        public void V850Dis_748F()
        {
            AssertCode("@@@", "748F");
        }

        // ................:.::.011110..:.:
        // ................:.::..::::...:.: sst_b
        // ................:..::100100:..::
        // ................:..:::..:..:..:: sst_h
        // ..................:.:101111:.::.
        // Reko: a decoder for V850 instruction F62D at address 0010048C has not been implemented.
        [Test]
        public void V850Dis_F62D()
        {
            AssertCode("@@@", "F62D");
        }

        // ................:....101000.:.::
        // ................:....:.:....:.:1 Sld.w/Sst.w
        // ................:....:.:....:.:: sst_w
        // ..................::.001111:.:.:
        // Reko: a decoder for V850 instruction F531 at address 00100490 has not been implemented.
        [Test]
        public void V850Dis_F531()
        {
            AssertCode("@@@", "F531");
        }

        // .................:.::001110::..:
        // Reko: a decoder for V850 instruction D959 at address 00100492 has not been implemented.
        [Test]
        public void V850Dis_D959()
        {
            AssertCode("@@@", "D959");
        }

        // ................:::.:001100...:.
        // Reko: a decoder for V850 instruction 82E9 at address 00100494 has not been implemented.
        [Test]
        public void V850Dis_82E9()
        {
            AssertCode("@@@", "82E9");
        }

        // ....................:001111..:.:
        // Reko: a decoder for V850 instruction E509 at address 00100496 has not been implemented.
        [Test]
        public void V850Dis_E509()
        {
            AssertCode("@@@", "E509");
        }

        // ................::...001100:::::
        // Reko: a decoder for V850 instruction 9FC1 at address 00100498 has not been implemented.
        [Test]
        public void V850Dis_9FC1()
        {
            AssertCode("@@@", "9FC1");
        }

        // .................::::110100::..:
        // Reko: a decoder for V850 instruction 997E at address 0010049A has not been implemented.
        [Test]
        public void V850Dis_997E()
        {
            AssertCode("@@@", "997E");
        }

        // .................::.:011010:....
        // .................::.:.::.:.:.... sld_b
        // .................::::010010::::.
        // Reko: a decoder for V850 instruction 5E7A at address 0010049E has not been implemented.
        [Test]
        public void V850Dis_5E7A()
        {
            AssertCode("@@@", "5E7A");
        }

        // ................:..:.101011::..:
        // Reko: a decoder for V850 instruction 7995 at address 001004A0 has not been implemented.
        [Test]
        public void V850Dis_7995()
        {
            AssertCode("@@@", "7995");
        }

        // .................:...111010:.:::
        // Reko: a decoder for V850 instruction 5747 at address 001004A2 has not been implemented.
        [Test]
        public void V850Dis_5747()
        {
            AssertCode("@@@", "5747");
        }

        // ................:::.:000111..:.:
        // Reko: a decoder for V850 instruction E5E8 at address 001004A4 has not been implemented.
        [Test]
        public void V850Dis_E5E8()
        {
            AssertCode("@@@", "E5E8");
        }

        // ................:::::011110.:::.
        // ................:::::.::::..:::. sst_b
        // .................::..000001..:::
        // .................::.......:..::: not
        // .................::::000010::::.
        // ................01111....:.::::.
        // .................::::....:.11110
        // .................::::....:.::::. divh
        // ................::.:.000011...:.
        // Reko: a decoder for V850 instruction 62D0 at address 001004AC has not been implemented.
        [Test]
        public void V850Dis_62D0()
        {
            AssertCode("@@@", "62D0");
        }

        // ...................:.011011::..:
        // ...................:..::.::::..: sld_b
        // ...................::101100..:..
        // Reko: a decoder for V850 instruction 841D at address 001004B0 has not been implemented.
        [Test]
        public void V850Dis_841D()
        {
            AssertCode("@@@", "841D");
        }

        // ................:..::011101:::::
        // ................:..::.:::.:::::: sst_b
        // ................:.:..111011:::.:
        // Reko: a decoder for V850 instruction 7DA7 at address 001004B4 has not been implemented.
        [Test]
        public void V850Dis_7DA7()
        {
            AssertCode("@@@", "7DA7");
        }

        // ....................:000110.:.:.
        // Reko: a decoder for V850 instruction CA08 at address 001004B6 has not been implemented.
        [Test]
        public void V850Dis_CA08()
        {
            AssertCode("@@@", "CA08");
        }

        // ................:.::.001101.:::.
        // Reko: a decoder for V850 instruction AEB1 at address 001004B8 has not been implemented.
        [Test]
        public void V850Dis_AEB1()
        {
            AssertCode("@@@", "AEB1");
        }

        // .................::.:001101::..:
        // Reko: a decoder for V850 instruction B969 at address 001004BA has not been implemented.
        [Test]
        public void V850Dis_B969()
        {
            AssertCode("@@@", "B969");
        }

        // ................::::.101010.::.:
        // Reko: a decoder for V850 instruction 4DF5 at address 001004BC has not been implemented.
        [Test]
        public void V850Dis_4DF5()
        {
            AssertCode("@@@", "4DF5");
        }

        // .................:.::001001::::.
        // Reko: a decoder for V850 instruction 3E59 at address 001004BE has not been implemented.
        [Test]
        public void V850Dis_3E59()
        {
            AssertCode("@@@", "3E59");
        }

        // ...................::001010:.:.:
        // Reko: a decoder for V850 instruction 5519 at address 001004C0 has not been implemented.
        [Test]
        public void V850Dis_5519()
        {
            AssertCode("@@@", "5519");
        }

        // .................::..001001...:.
        // Reko: a decoder for V850 instruction 2261 at address 001004C2 has not been implemented.
        [Test]
        public void V850Dis_2261()
        {
            AssertCode("@@@", "2261");
        }

        // ................::...011110:.::.
        // ................::....::::.:.::. sst_b
        // ....................:100100:::::
        // ....................::..:..::::: sst_h
        // ................::.::110011...::
        // Reko: a decoder for V850 instruction 63DE at address 001004C8 has not been implemented.
        [Test]
        public void V850Dis_63DE()
        {
            AssertCode("@@@", "63DE");
        }

        // ..................:..010001:..:.
        // Reko: a decoder for V850 instruction 3222 at address 001004CA has not been implemented.
        [Test]
        public void V850Dis_3222()
        {
            AssertCode("@@@", "3222");
        }

        // .................::.:100100.:.::
        // .................::.::..:...:.:: sst_h
        // ................::..:111101..:::
        // Reko: a decoder for V850 instruction A7CF at address 001004CE has not been implemented.
        [Test]
        public void V850Dis_A7CF()
        {
            AssertCode("@@@", "A7CF");
        }

        // ................:....101010.:...
        // Reko: a decoder for V850 instruction 4885 at address 001004D0 has not been implemented.
        [Test]
        public void V850Dis_4885()
        {
            AssertCode("@@@", "4885");
        }

        // ..................::.010111::..:
        // Reko: a decoder for V850 instruction F932 at address 001004D2 has not been implemented.
        [Test]
        public void V850Dis_F932()
        {
            AssertCode("@@@", "F932");
        }

        // ................::...010101.::..
        // Reko: a decoder for V850 instruction ACC2 at address 001004D4 has not been implemented.
        [Test]
        public void V850Dis_ACC2()
        {
            AssertCode("@@@", "ACC2");
        }

        // ................::.:.010111:::..
        // Reko: a decoder for V850 instruction FCD2 at address 001004D6 has not been implemented.
        [Test]
        public void V850Dis_FCD2()
        {
            AssertCode("@@@", "FCD2");
        }

        // .................:..:110110.:.:.
        // Reko: a decoder for V850 instruction CA4E at address 001004D8 has not been implemented.
        [Test]
        public void V850Dis_CA4E()
        {
            AssertCode("@@@", "CA4E");
        }

        // ..................::.011100::..:
        // ..................::..:::..::..: sst_b
        // .................::..100001.::::
        // .................::..:....:.:::: sld_h
        // ..................::.001101.::.:
        // Reko: a decoder for V850 instruction AD31 at address 001004DE has not been implemented.
        [Test]
        public void V850Dis_AD31()
        {
            AssertCode("@@@", "AD31");
        }

        // .....................101001....:
        // Reko: a decoder for V850 instruction 2105 at address 001004E0 has not been implemented.
        [Test]
        public void V850Dis_2105()
        {
            AssertCode("@@@", "2105");
        }

        // .................:..:000001...::
        // .................:..:.....:...:: not
        // ...................::000101.::.:
        // Reko: a decoder for V850 instruction AD18 at address 001004E4 has not been implemented.
        [Test]
        public void V850Dis_AD18()
        {
            AssertCode("@@@", "AD18");
        }

        // ................::...001001:::..
        // Reko: a decoder for V850 instruction 3CC1 at address 001004E6 has not been implemented.
        [Test]
        public void V850Dis_3CC1()
        {
            AssertCode("@@@", "3CC1");
        }

        // ................:.::.010011:::.:
        // Reko: a decoder for V850 instruction 7DB2 at address 001004E8 has not been implemented.
        [Test]
        public void V850Dis_7DB2()
        {
            AssertCode("@@@", "7DB2");
        }

        // ................:..::111001::...
        // Reko: a decoder for V850 instruction 389F at address 001004EA has not been implemented.
        [Test]
        public void V850Dis_389F()
        {
            AssertCode("@@@", "389F");
        }

        // ................:.:.:000011:.:.:
        // Reko: a decoder for V850 instruction 75A8 at address 001004EC has not been implemented.
        [Test]
        public void V850Dis_75A8()
        {
            AssertCode("@@@", "75A8");
        }

        // .................:...010011:::.:
        // Reko: a decoder for V850 instruction 7D42 at address 001004EE has not been implemented.
        [Test]
        public void V850Dis_7D42()
        {
            AssertCode("@@@", "7D42");
        }

        // ................:::.:010000.:.:.
        // ................:::.:.:....01010
        // ................:::.:.:.....:.:. mov
        // ................::..:000001....:
        // ................::..:.....:....: not
        // ....................:011111:::..
        // ....................:.::::::::.. sst_b
        // ................:..:.010101.::.:
        // Reko: a decoder for V850 instruction AD92 at address 001004F6 has not been implemented.
        [Test]
        public void V850Dis_AD92()
        {
            AssertCode("@@@", "AD92");
        }

        // .................:.:.000000:::.:
        // ................01010......:::.:
        // .................:.:.......:::.: mov
        // ................:...:000100:.:::
        // Reko: a decoder for V850 instruction 9788 at address 001004FA has not been implemented.
        [Test]
        public void V850Dis_9788()
        {
            AssertCode("@@@", "9788");
        }

        // .................::.:011001.:.:.
        // .................::.:.::..:.:.:. sld_b
        // ................:::::111110:::.:
        // Reko: a decoder for V850 instruction DDFF at address 001004FE has not been implemented.
        [Test]
        public void V850Dis_DDFF()
        {
            AssertCode("@@@", "DDFF");
        }

        // .....................000001:....
        // ..........................::.... not
        // ................:....110000.....
        // Reko: a decoder for V850 instruction 0086 at address 00100502 has not been implemented.
        [Test]
        public void V850Dis_0086()
        {
            AssertCode("@@@", "0086");
        }

        // .................:.::110010.:...
        // Reko: a decoder for V850 instruction 485E at address 00100504 has not been implemented.
        [Test]
        public void V850Dis_485E()
        {
            AssertCode("@@@", "485E");
        }

        // ..................::.000001....:
        // ..................::......:....: not
        // ..................::.111010.:...
        // Reko: a decoder for V850 instruction 4837 at address 00100508 has not been implemented.
        [Test]
        public void V850Dis_4837()
        {
            AssertCode("@@@", "4837");
        }

        // ................:::::001101:.:.:
        // Reko: a decoder for V850 instruction B5F9 at address 0010050A has not been implemented.
        [Test]
        public void V850Dis_B5F9()
        {
            AssertCode("@@@", "B5F9");
        }

        // .................:.::100100.:.:.
        // .................:.:::..:...:.:. sst_h
        // ..................:..001010:::..
        // Reko: a decoder for V850 instruction 5C21 at address 0010050E has not been implemented.
        [Test]
        public void V850Dis_5C21()
        {
            AssertCode("@@@", "5C21");
        }

        // .................:.:.110110:....
        // Reko: a decoder for V850 instruction D056 at address 00100510 has not been implemented.
        [Test]
        public void V850Dis_D056()
        {
            AssertCode("@@@", "D056");
        }

        // .................:...100001.:..:
        // .................:...:....:.:..: sld_h
        // ................:::..101010...::
        // Reko: a decoder for V850 instruction 43E5 at address 00100514 has not been implemented.
        [Test]
        public void V850Dis_43E5()
        {
            AssertCode("@@@", "43E5");
        }

        // ...................:.101001:.:..
        // Reko: a decoder for V850 instruction 3415 at address 00100516 has not been implemented.
        [Test]
        public void V850Dis_3415()
        {
            AssertCode("@@@", "3415");
        }

        // ................:..:.000010:.:::
        // ................10010....:.:.:::
        // ................:..:.....:.10111
        // ................:..:.....:.:.::: divh
        // .................:.::101001.:.::
        // Reko: a decoder for V850 instruction 2B5D at address 0010051A has not been implemented.
        [Test]
        public void V850Dis_2B5D()
        {
            AssertCode("@@@", "2B5D");
        }

        // ................:.::.100100....:
        // ................:.::.:..:......: sst_h
        // ...................:.001100:.:::
        // Reko: a decoder for V850 instruction 9711 at address 0010051E has not been implemented.
        [Test]
        public void V850Dis_9711()
        {
            AssertCode("@@@", "9711");
        }

        // ..................:::001110:.:.:
        // Reko: a decoder for V850 instruction D539 at address 00100520 has not been implemented.
        [Test]
        public void V850Dis_D539()
        {
            AssertCode("@@@", "D539");
        }

        // .................:::.100011.::::
        // .................:::.:...::.:::: sld_h
        // ................::::.111101..:::
        // Reko: a decoder for V850 instruction A7F7 at address 00100524 has not been implemented.
        [Test]
        public void V850Dis_A7F7()
        {
            AssertCode("@@@", "A7F7");
        }

        // ................:.:.:011111::..:
        // ................:.:.:.:::::::..: sst_b
        // ................::::.000110.:...
        // Reko: a decoder for V850 instruction C8F0 at address 00100528 has not been implemented.
        [Test]
        public void V850Dis_C8F0()
        {
            AssertCode("@@@", "C8F0");
        }

        // ................::...100011:::::
        // ................::...:...::::::: sld_h
        // ................:.::.011010:.::.
        // ................:.::..::.:.:.::. sld_b
        // ................::...000000.:...
        // ................11000.......:...
        // ................::..........:... mov
        // .................:::.101111:..::
        // Reko: a decoder for V850 instruction F375 at address 00100530 has not been implemented.
        [Test]
        public void V850Dis_F375()
        {
            AssertCode("@@@", "F375");
        }

        // ................::.:.011101::...
        // ................::.:..:::.:::... sst_b
        // ................:..::000010...:.
        // ................10011....:....:.
        // ................:..::....:.00010
        // ................:..::....:....:. divh
        // .................:..:011101::.::
        // .................:..:.:::.:::.:: sst_b
        // ................:::.:011110.:.::
        // ................:::.:.::::..:.:: sst_b
        // ................:.::.111001.:::.
        // Reko: a decoder for V850 instruction 2EB7 at address 0010053A has not been implemented.
        [Test]
        public void V850Dis_2EB7()
        {
            AssertCode("@@@", "2EB7");
        }

        // ................::...100001:..::
        // ................::...:....::..:: sld_h
        // ................:..:.100101.::::
        // ................:..:.:..:.:.:::: sst_h
        // ................:.:..011101.....
        // ................:.:...:::.:..... sst_b
        // .................:.:.000001..:..
        // .................:.:......:..:.. not
        // ................:::..111111.::.:
        // Reko: a decoder for V850 instruction EDE7 at address 00100544 has not been implemented.
        [Test]
        public void V850Dis_EDE7()
        {
            AssertCode("@@@", "EDE7");
        }

        // ................::.::000011::..:
        // Reko: a decoder for V850 instruction 79D8 at address 00100546 has not been implemented.
        [Test]
        public void V850Dis_79D8()
        {
            AssertCode("@@@", "79D8");
        }

        // .................::::101010:::.:
        // Reko: a decoder for V850 instruction 5D7D at address 00100548 has not been implemented.
        [Test]
        public void V850Dis_5D7D()
        {
            AssertCode("@@@", "5D7D");
        }

        // .................:.:.010011:::..
        // Reko: a decoder for V850 instruction 7C52 at address 0010054A has not been implemented.
        [Test]
        public void V850Dis_7C52()
        {
            AssertCode("@@@", "7C52");
        }

        // .................::.:111011..::.
        // Reko: a decoder for V850 instruction 666F at address 0010054C has not been implemented.
        [Test]
        public void V850Dis_666F()
        {
            AssertCode("@@@", "666F");
        }

        // ................:.:..100101::::.
        // ................:.:..:..:.:::::. sst_h
        // .................::.:010100.:...
        // Reko: a decoder for V850 instruction 886A at address 00100550 has not been implemented.
        [Test]
        public void V850Dis_886A()
        {
            AssertCode("@@@", "886A");
        }

        // ....................:011011.:..:
        // ....................:.::.::.:..: sld_b
        // ................::...100111::.::
        // ................::...:..:::::.:: sst_h
        // ..................:::100110.::::
        // ..................::::..::..:::: sst_h
        // ..................:..100011:::.:
        // ..................:..:...:::::.: sld_h
        // ................:.:::100101.:...
        // ................:.::::..:.:.:... sst_h
        // .................:...001110.:..:
        // Reko: a decoder for V850 instruction C941 at address 0010055C has not been implemented.
        [Test]
        public void V850Dis_C941()
        {
            AssertCode("@@@", "C941");
        }

        // .....................011011::...
        // ......................::.::::... sld_b
        // ................:::.:010010....:
        // Reko: a decoder for V850 instruction 41EA at address 00100560 has not been implemented.
        [Test]
        public void V850Dis_41EA()
        {
            AssertCode("@@@", "41EA");
        }

        // ................:.::.011001..:..
        // ................:.::..::..:..:.. sld_b
        // ................:.::.111100.::::
        // Reko: a decoder for V850 instruction 8FB7 at address 00100564 has not been implemented.
        [Test]
        public void V850Dis_8FB7()
        {
            AssertCode("@@@", "8FB7");
        }

        // ....................:011110:..::
        // ....................:.::::.:..:: sst_b
        // ................::.::111111::.::
        // Reko: a decoder for V850 instruction FBDF at address 00100568 has not been implemented.
        [Test]
        public void V850Dis_FBDF()
        {
            AssertCode("@@@", "FBDF");
        }

        // ................::...100011:.:.:
        // ................::...:...:::.:.: sld_h
        // .................::.:000000:::..
        // ................01101......:::..
        // .................::.:......:::.. mov
        // .................:.::011111..:.:
        // .................:.::.:::::..:.: sst_b
        // ................:...:010001.::..
        // Reko: a decoder for V850 instruction 2C8A at address 00100570 has not been implemented.
        [Test]
        public void V850Dis_2C8A()
        {
            AssertCode("@@@", "2C8A");
        }

        // ................::..:000001.::::
        // ................::..:.....:.:::: not
        // ................:.:..011010::.:.
        // ................:.:...::.:.::.:. sld_b
        // ................::.::011100::...
        // ................::.::.:::..::... sst_b
        // ................:::..111110:.:..
        // Reko: a decoder for V850 instruction D4E7 at address 00100578 has not been implemented.
        [Test]
        public void V850Dis_D4E7()
        {
            AssertCode("@@@", "D4E7");
        }

        // ................::..:001110:..:.
        // Reko: a decoder for V850 instruction D2C9 at address 0010057A has not been implemented.
        [Test]
        public void V850Dis_D2C9()
        {
            AssertCode("@@@", "D2C9");
        }

        // ................:.:::101001::::.
        // Reko: a decoder for V850 instruction 3EBD at address 0010057C has not been implemented.
        [Test]
        public void V850Dis_3EBD()
        {
            AssertCode("@@@", "3EBD");
        }

        // ...................::011000.::::
        // ...................::.::....:::: sld_b
        // .................::::000000..:.:
        // ................01111........:.:
        // .................::::........:.: mov
        // .................::::011000:...:
        // .................::::.::...:...: sld_b
        // ....................:011010.:.:.
        // ....................:.::.:..:.:. sld_b
        // ................::.:.000000:.:..
        // ................11010......:.:..
        // ................::.:.......:.:.. mov
        // .................:.::101100:.:::
        // Reko: a decoder for V850 instruction 975D at address 00100588 has not been implemented.
        [Test]
        public void V850Dis_975D()
        {
            AssertCode("@@@", "975D");
        }

        // ...................::100000:....
        // ...................:::.....:.... sld_h
        // ....................:011010.::::
        // ....................:.::.:..:::: sld_b
        // ...................:.100001.....
        // ...................:.:....:..... sld_h
        // .................:..:100001:::::
        // .................:..::....:::::: sld_h
        // ..................:::111100.::::
        // Reko: a decoder for V850 instruction 8F3F at address 00100592 has not been implemented.
        [Test]
        public void V850Dis_8F3F()
        {
            AssertCode("@@@", "8F3F");
        }

        // ................:.::.000000:..:.
        // ................10110......:..:.
        // ................:.::.......:..:. mov
        // ................:..::101101.:...
        // Reko: a decoder for V850 instruction A89D at address 00100596 has not been implemented.
        [Test]
        public void V850Dis_A89D()
        {
            AssertCode("@@@", "A89D");
        }

        // .................:..:001010:..:.
        // Reko: a decoder for V850 instruction 5249 at address 00100598 has not been implemented.
        [Test]
        public void V850Dis_5249()
        {
            AssertCode("@@@", "5249");
        }

        // ................:.:..111110:::::
        // Reko: a decoder for V850 instruction DFA7 at address 0010059A has not been implemented.
        [Test]
        public void V850Dis_DFA7()
        {
            AssertCode("@@@", "DFA7");
        }

        // ....................:010111..:.:
        // Reko: a decoder for V850 instruction E50A at address 0010059C has not been implemented.
        [Test]
        public void V850Dis_E50A()
        {
            AssertCode("@@@", "E50A");
        }

        // ..................:::101110.::.:
        // Reko: a decoder for V850 instruction CD3D at address 0010059E has not been implemented.
        [Test]
        public void V850Dis_CD3D()
        {
            AssertCode("@@@", "CD3D");
        }

        // .................:.::010110:....
        // Reko: a decoder for V850 instruction D05A at address 001005A0 has not been implemented.
        [Test]
        public void V850Dis_D05A()
        {
            AssertCode("@@@", "D05A");
        }

        // ................:.:..011101:::::
        // ................:.:...:::.:::::: sst_b
        // .................::.:001010:.:::
        // Reko: a decoder for V850 instruction 5769 at address 001005A4 has not been implemented.
        [Test]
        public void V850Dis_5769()
        {
            AssertCode("@@@", "5769");
        }

        // ..................:..000000:...:
        // ................00100......:...:
        // ..................:........:...: mov
        // .................:.::111011:.:.:
        // Reko: a decoder for V850 instruction 755F at address 001005A8 has not been implemented.
        [Test]
        public void V850Dis_755F()
        {
            AssertCode("@@@", "755F");
        }

        // ................:::::101001:....
        // Reko: a decoder for V850 instruction 30FD at address 001005AA has not been implemented.
        [Test]
        public void V850Dis_30FD()
        {
            AssertCode("@@@", "30FD");
        }

        // .................::::100100:..::
        // .................:::::..:..:..:: sst_h
        // ................::..:000100.::..
        // Reko: a decoder for V850 instruction 8CC8 at address 001005AE has not been implemented.
        [Test]
        public void V850Dis_8CC8()
        {
            AssertCode("@@@", "8CC8");
        }

        // ................:.:::110110..:::
        // Reko: a decoder for V850 instruction C7BE at address 001005B0 has not been implemented.
        [Test]
        public void V850Dis_C7BE()
        {
            AssertCode("@@@", "C7BE");
        }

        // ................::::.001100.:..:
        // Reko: a decoder for V850 instruction 89F1 at address 001005B2 has not been implemented.
        [Test]
        public void V850Dis_89F1()
        {
            AssertCode("@@@", "89F1");
        }

        // ................::.:.000001.::.:
        // ................::.:......:.::.: not
        // .................:.:.100000.:..:
        // .................:.:.:......:..: sld_h
        // .................:::.011110.::::
        // .................:::..::::..:::: sst_b
        // .....................001001.:...
        // Reko: a decoder for V850 instruction 2801 at address 001005BA has not been implemented.
        [Test]
        public void V850Dis_2801()
        {
            AssertCode("@@@", "2801");
        }

        // .................::.:100001.::..
        // .................::.::....:.::.. sld_h
        // ................:...:101111.:.::
        // Reko: a decoder for V850 instruction EB8D at address 001005BE has not been implemented.
        [Test]
        public void V850Dis_EB8D()
        {
            AssertCode("@@@", "EB8D");
        }

        // ................::.:.010100:....
        // Reko: a decoder for V850 instruction 90D2 at address 001005C0 has not been implemented.
        [Test]
        public void V850Dis_90D2()
        {
            AssertCode("@@@", "90D2");
        }

        // ................::::.100010.::..
        // ................::::.:...:..::.. sld_h
        // .................::::100010.:.::
        // .................:::::...:..:.:: sld_h
        // ................::.:.101101:::.:
        // Reko: a decoder for V850 instruction BDD5 at address 001005C6 has not been implemented.
        [Test]
        public void V850Dis_BDD5()
        {
            AssertCode("@@@", "BDD5");
        }

        // .................::.:110001::.::
        // Reko: a decoder for V850 instruction 3B6E at address 001005C8 has not been implemented.
        [Test]
        public void V850Dis_3B6E()
        {
            AssertCode("@@@", "3B6E");
        }

        // ................:.::.011110.:..:
        // ................:.::..::::..:..: sst_b
        // ................:.:.:011011:::::
        // ................:.:.:.::.::::::: sld_b
        // ................::.:.101000.:.:.
        // ................::.:.:.:....:.:0 Sld.w/Sst.w
        // ................::.:.:.:....:.:. sld_w
        // ...................:.110101:::.:
        // Reko: a decoder for V850 instruction BD16 at address 001005D0 has not been implemented.
        [Test]
        public void V850Dis_BD16()
        {
            AssertCode("@@@", "BD16");
        }

        // ..................:..111110:::::
        // Reko: a decoder for V850 instruction DF27 at address 001005D2 has not been implemented.
        [Test]
        public void V850Dis_DF27()
        {
            AssertCode("@@@", "DF27");
        }

        // ................::.::011100.....
        // ................::.::.:::....... sst_b
        // ................:::::111000.::::
        // Reko: a decoder for V850 instruction 0FFF at address 001005D6 has not been implemented.
        [Test]
        public void V850Dis_0FFF()
        {
            AssertCode("@@@", "0FFF");
        }

        // .................::.:010010:...:
        // Reko: a decoder for V850 instruction 516A at address 001005D8 has not been implemented.
        [Test]
        public void V850Dis_516A()
        {
            AssertCode("@@@", "516A");
        }

        // ................:::..100110:..::
        // ................:::..:..::.:..:: sst_h
        // ....................:111010....:
        // Reko: a decoder for V850 instruction 410F at address 001005DC has not been implemented.
        [Test]
        public void V850Dis_410F()
        {
            AssertCode("@@@", "410F");
        }

        // ....................:000111:.:..
        // Reko: a decoder for V850 instruction F408 at address 001005DE has not been implemented.
        [Test]
        public void V850Dis_F408()
        {
            AssertCode("@@@", "F408");
        }

        // ................:.:..011011:..::
        // ................:.:...::.:::..:: sld_b
        // ................::...001011.::.:
        // Reko: a decoder for V850 instruction 6DC1 at address 001005E2 has not been implemented.
        [Test]
        public void V850Dis_6DC1()
        {
            AssertCode("@@@", "6DC1");
        }

        // ................:..:.111001:....
        // Reko: a decoder for V850 instruction 3097 at address 001005E4 has not been implemented.
        [Test]
        public void V850Dis_3097()
        {
            AssertCode("@@@", "3097");
        }

        // .................:..:011011:::.:
        // .................:..:.::.:::::.: sld_b
        // ................:.::.100000.....
        // ................:.::.:.......... sld_h
        // ................:.:::111101:.::.
        // Reko: a decoder for V850 instruction B6BF at address 001005EA has not been implemented.
        [Test]
        public void V850Dis_B6BF()
        {
            AssertCode("@@@", "B6BF");
        }

        // .................:.:.101000..::.
        // .................:.:.:.:.....::0 Sld.w/Sst.w
        // .................:.:.:.:.....::. sld_w
        // ..................:..100110:.:.:
        // ..................:..:..::.:.:.: sst_h
        // .................:.:.011011.:..:
        // .................:.:..::.::.:..: sld_b
        // ...................:.110010..:..
        // Reko: a decoder for V850 instruction 4416 at address 001005F2 has not been implemented.
        [Test]
        public void V850Dis_4416()
        {
            AssertCode("@@@", "4416");
        }

        // ................::...100100.::.:
        // ................::...:..:...::.: sst_h
        // ................:.::.101000.::.:
        // ................:.::.:.:....::.1 Sld.w/Sst.w
        // ................:.::.:.:....::.: sst_w
        // .................:..:100110...:.
        // .................:..::..::....:. sst_h
        // ................:::..001101...::
        // Reko: a decoder for V850 instruction A3E1 at address 001005FA has not been implemented.
        [Test]
        public void V850Dis_A3E1()
        {
            AssertCode("@@@", "A3E1");
        }

        // ................::.::001101.:...
        // Reko: a decoder for V850 instruction A8D9 at address 001005FC has not been implemented.
        [Test]
        public void V850Dis_A8D9()
        {
            AssertCode("@@@", "A8D9");
        }

        // ..................:.:100100:.:..
        // ..................:.::..:..:.:.. sst_h
        // ................::...000011::.:.
        // Reko: a decoder for V850 instruction 7AC0 at address 00100600 has not been implemented.
        [Test]
        public void V850Dis_7AC0()
        {
            AssertCode("@@@", "7AC0");
        }

        // ................::.::011001:::::
        // ................::.::.::..:::::: sld_b
        // .................:..:000001::::.
        // .................:..:.....:::::. not
        // ..................:..101101..:.:
        // Reko: a decoder for V850 instruction A525 at address 00100606 has not been implemented.
        [Test]
        public void V850Dis_A525()
        {
            AssertCode("@@@", "A525");
        }

        // ................:..:.100100::..:
        // ................:..:.:..:..::..: sst_h
        // ................::..:011001..::.
        // ................::..:.::..:..::. sld_b
        // .................:.:.000001..:..
        // .................:.:......:..:.. not
        // ................:::::011111::...
        // ................:::::.:::::::... sst_b
        // ................:.:.:111100:.:..
        // Reko: a decoder for V850 instruction 94AF at address 00100610 has not been implemented.
        [Test]
        public void V850Dis_94AF()
        {
            AssertCode("@@@", "94AF");
        }

        // ................:::.:101011::.:.
        // Reko: a decoder for V850 instruction 7AED at address 00100612 has not been implemented.
        [Test]
        public void V850Dis_7AED()
        {
            AssertCode("@@@", "7AED");
        }

        // ................::.::110110.:.::
        // Reko: a decoder for V850 instruction CBDE at address 00100614 has not been implemented.
        [Test]
        public void V850Dis_CBDE()
        {
            AssertCode("@@@", "CBDE");
        }

        // ................:.:.:111011::.::
        // Reko: a decoder for V850 instruction 7BAF at address 00100616 has not been implemented.
        [Test]
        public void V850Dis_7BAF()
        {
            AssertCode("@@@", "7BAF");
        }

        // ................:.::.111100:.::.
        // Reko: a decoder for V850 instruction 96B7 at address 00100618 has not been implemented.
        [Test]
        public void V850Dis_96B7()
        {
            AssertCode("@@@", "96B7");
        }

        // ................:::..100111:::::
        // ................:::..:..:::::::: sst_h
        // ................:..::011111:..::
        // ................:..::.::::::..:: sst_b
        // ..................:..111101...:.
        // Reko: a decoder for V850 instruction A227 at address 0010061E has not been implemented.
        [Test]
        public void V850Dis_A227()
        {
            AssertCode("@@@", "A227");
        }

        // ................:...:010001.....
        // Reko: a decoder for V850 instruction 208A at address 00100620 has not been implemented.
        [Test]
        public void V850Dis_208A()
        {
            AssertCode("@@@", "208A");
        }

        // .................:...110010..:..
        // Reko: a decoder for V850 instruction 4446 at address 00100622 has not been implemented.
        [Test]
        public void V850Dis_4446()
        {
            AssertCode("@@@", "4446");
        }

        // ................:::::001000:.:.:
        // ................:::::..:...:.:.: or
        // ..................:..000111..:.:
        // Reko: a decoder for V850 instruction E520 at address 00100626 has not been implemented.
        [Test]
        public void V850Dis_E520()
        {
            AssertCode("@@@", "E520");
        }

        // ...................::110101.:::.
        // Reko: a decoder for V850 instruction AE1E at address 00100628 has not been implemented.
        [Test]
        public void V850Dis_AE1E()
        {
            AssertCode("@@@", "AE1E");
        }

        // ................:::::000011:::.:
        // Reko: a decoder for V850 instruction 7DF8 at address 0010062A has not been implemented.
        [Test]
        public void V850Dis_7DF8()
        {
            AssertCode("@@@", "7DF8");
        }

        // .................:...001100:::.:
        // Reko: a decoder for V850 instruction 9D41 at address 0010062C has not been implemented.
        [Test]
        public void V850Dis_9D41()
        {
            AssertCode("@@@", "9D41");
        }

        // .................:::.001100:....
        // Reko: a decoder for V850 instruction 9071 at address 0010062E has not been implemented.
        [Test]
        public void V850Dis_9071()
        {
            AssertCode("@@@", "9071");
        }

        // ................:.:.:000010:..::
        // ................10101....:.:..::
        // ................:.:.:....:.10011
        // ................:.:.:....:.:..:: divh
        // ................:::::000010.::..
        // ................11111....:..::..
        // ................:::::....:.01100
        // ................:::::....:..::.. divh
        // ................:.:..110010::::.
        // Reko: a decoder for V850 instruction 5EA6 at address 00100634 has not been implemented.
        [Test]
        public void V850Dis_5EA6()
        {
            AssertCode("@@@", "5EA6");
        }

        // ................:..::011011.....
        // ................:..::.::.::..... sld_b
        // ................:..:.000010..:.:
        // ................10010....:...:.:
        // ................:..:.....:.00101
        // ................:..:.....:...:.: divh
        // ................:....110010..:.:
        // Reko: a decoder for V850 instruction 4586 at address 0010063A has not been implemented.
        [Test]
        public void V850Dis_4586()
        {
            AssertCode("@@@", "4586");
        }

        // ................::...000010.:..:
        // ................11000....:..:..:
        // ................::.......:.01001
        // ................::.......:..:..: divh
        // ................:::::101001:..:.
        // Reko: a decoder for V850 instruction 32FD at address 0010063E has not been implemented.
        [Test]
        public void V850Dis_32FD()
        {
            AssertCode("@@@", "32FD");
        }

        // ................:::::111111:..:.
        // Reko: a decoder for V850 instruction F2FF at address 00100640 has not been implemented.
        [Test]
        public void V850Dis_F2FF()
        {
            AssertCode("@@@", "F2FF");
        }

        // ................:..::010101.:.:.
        // Reko: a decoder for V850 instruction AA9A at address 00100642 has not been implemented.
        [Test]
        public void V850Dis_AA9A()
        {
            AssertCode("@@@", "AA9A");
        }

        // ................:.::.010000.::..
        // ................:.::..:....01100
        // ................:.::..:.....::.. mov
        // ................:..::011111..:.:
        // ................:..::.:::::..:.: sst_b
        // .................:.::010110.:.:.
        // Reko: a decoder for V850 instruction CA5A at address 00100648 has not been implemented.
        [Test]
        public void V850Dis_CA5A()
        {
            AssertCode("@@@", "CA5A");
        }

        // ................:.:.:101001:::::
        // ................:....011011....:
        // ................:.....::.::....: sld_b
        // .................:.:.101000:...:
        // .................:.:.:.:...:...1 Sld.w/Sst.w
        // .................:.:.:.:...:...: sst_w
        // ................:...:101001..:::
        // Reko: a decoder for V850 instruction 278D at address 00100650 has not been implemented.
        [Test]
        public void V850Dis_278D()
        {
            AssertCode("@@@", "278D");
        }

        // .................:..:011100:....
        // .................:..:.:::..:.... sst_b
        // ................:.:.:001010.:::.
        // Reko: a decoder for V850 instruction 4EA9 at address 00100654 has not been implemented.
        [Test]
        public void V850Dis_4EA9()
        {
            AssertCode("@@@", "4EA9");
        }

        // .................::::001110.:...
        // Reko: a decoder for V850 instruction C879 at address 00100656 has not been implemented.
        [Test]
        public void V850Dis_C879()
        {
            AssertCode("@@@", "C879");
        }

        // ................:.:.:000001.:..:
        // ................:.:.:.....:.:..: not
        // ................:....101000....:
        // ................:....:.:.......1 Sld.w/Sst.w
        // ................:....:.:.......: sst_w
        // .................:.::110101:.:..
        // Reko: a decoder for V850 instruction B45E at address 0010065C has not been implemented.
        [Test]
        public void V850Dis_B45E()
        {
            AssertCode("@@@", "B45E");
        }

        // ................:::.:101110:.::.
        // Reko: a decoder for V850 instruction D6ED at address 0010065E has not been implemented.
        [Test]
        public void V850Dis_D6ED()
        {
            AssertCode("@@@", "D6ED");
        }

        // .................::::111111.::..
        // Reko: a decoder for V850 instruction EC7F at address 00100660 has not been implemented.
        [Test]
        public void V850Dis_EC7F()
        {
            AssertCode("@@@", "EC7F");
        }

        // ................:..::110000..:.:
        // Reko: a decoder for V850 instruction 059E at address 00100662 has not been implemented.
        [Test]
        public void V850Dis_059E()
        {
            AssertCode("@@@", "059E");
        }

        // ................::::.101100:.::.
        // Reko: a decoder for V850 instruction 96F5 at address 00100664 has not been implemented.
        [Test]
        public void V850Dis_96F5()
        {
            AssertCode("@@@", "96F5");
        }

        // ................:.:..001011..:.:
        // Reko: a decoder for V850 instruction 65A1 at address 00100666 has not been implemented.
        [Test]
        public void V850Dis_65A1()
        {
            AssertCode("@@@", "65A1");
        }

        // ................:.::.001110...:.
        // Reko: a decoder for V850 instruction C2B1 at address 00100668 has not been implemented.
        [Test]
        public void V850Dis_C2B1()
        {
            AssertCode("@@@", "C2B1");
        }

        // .....................100001.:::.
        // .....................:....:.:::. sld_h
        // ................:.::.010011..:::
        // Reko: a decoder for V850 instruction 67B2 at address 0010066C has not been implemented.
        [Test]
        public void V850Dis_67B2()
        {
            AssertCode("@@@", "67B2");
        }

        // ................::...001001::.:.
        // Reko: a decoder for V850 instruction 3AC1 at address 0010066E has not been implemented.
        [Test]
        public void V850Dis_3AC1()
        {
            AssertCode("@@@", "3AC1");
        }

        // ................:::.:111011::::.
        // Reko: a decoder for V850 instruction 7EEF at address 00100670 has not been implemented.
        [Test]
        public void V850Dis_7EEF()
        {
            AssertCode("@@@", "7EEF");
        }

        // ................:.:::100010:::::
        // ................:.::::...:.::::: sld_h
        // ................:::..111001:....
        // Reko: a decoder for V850 instruction 30E7 at address 00100674 has not been implemented.
        [Test]
        public void V850Dis_30E7()
        {
            AssertCode("@@@", "30E7");
        }

        // .................:..:000101.:::.
        // Reko: a decoder for V850 instruction AE48 at address 00100676 has not been implemented.
        [Test]
        public void V850Dis_AE48()
        {
            AssertCode("@@@", "AE48");
        }

        // ..................::.001001:.::.
        // Reko: a decoder for V850 instruction 3631 at address 00100678 has not been implemented.
        [Test]
        public void V850Dis_3631()
        {
            AssertCode("@@@", "3631");
        }

        // .................::..011011:...:
        // .................::...::.:::...: sld_b
        // .................:.::111101:...:
        // Reko: a decoder for V850 instruction B15F at address 0010067C has not been implemented.
        [Test]
        public void V850Dis_B15F()
        {
            AssertCode("@@@", "B15F");
        }

        // .................::.:011111..::.
        // .................::.:.:::::..::. sst_b
        // .................:.:.001100..:..
        // Reko: a decoder for V850 instruction 8451 at address 00100680 has not been implemented.
        [Test]
        public void V850Dis_8451()
        {
            AssertCode("@@@", "8451");
        }

        // ................:::..001110....:
        // Reko: a decoder for V850 instruction C1E1 at address 00100682 has not been implemented.
        [Test]
        public void V850Dis_C1E1()
        {
            AssertCode("@@@", "C1E1");
        }

        // .................:::.010011:::.:
        // Reko: a decoder for V850 instruction 7D72 at address 00100684 has not been implemented.
        [Test]
        public void V850Dis_7D72()
        {
            AssertCode("@@@", "7D72");
        }

        // ..................::.001101::::.
        // Reko: a decoder for V850 instruction BE31 at address 00100686 has not been implemented.
        [Test]
        public void V850Dis_BE31()
        {
            AssertCode("@@@", "BE31");
        }

        // ................:::.:111001.:...
        // Reko: a decoder for V850 instruction 28EF at address 00100688 has not been implemented.
        [Test]
        public void V850Dis_28EF()
        {
            AssertCode("@@@", "28EF");
        }

        // ................:..::101100.:...
        // Reko: a decoder for V850 instruction 889D at address 0010068A has not been implemented.
        [Test]
        public void V850Dis_889D()
        {
            AssertCode("@@@", "889D");
        }

        // ................::...100011::.:.
        // ................::...:...::::.:. sld_h
        // .................:::.111000:..:.
        // Reko: a decoder for V850 instruction 1277 at address 0010068E has not been implemented.
        [Test]
        public void V850Dis_1277()
        {
            AssertCode("@@@", "1277");
        }

        // ..................:::111011.:::.
        // Reko: a decoder for V850 instruction 6E3F at address 00100690 has not been implemented.
        [Test]
        public void V850Dis_6E3F()
        {
            AssertCode("@@@", "6E3F");
        }

        // ................::...100000..:::
        // ................::...:.......::: sld_h
        // ...................::010111:::::
        // Reko: a decoder for V850 instruction FF1A at address 00100694 has not been implemented.
        [Test]
        public void V850Dis_FF1A()
        {
            AssertCode("@@@", "FF1A");
        }

        // .....................100101:::.:
        // .....................:..:.::::.: sst_h
        // ................:.:::100110:....
        // ................:.::::..::.:.... sst_h
        // .................::..101011.:...
        // Reko: a decoder for V850 instruction 6865 at address 0010069A has not been implemented.
        [Test]
        public void V850Dis_6865()
        {
            AssertCode("@@@", "6865");
        }

        // .....................010100..:..
        // Reko: a decoder for V850 instruction 8402 at address 0010069C has not been implemented.
        [Test]
        public void V850Dis_8402()
        {
            AssertCode("@@@", "8402");
        }

        // .................::..010111.::::
        // Reko: a decoder for V850 instruction EF62 at address 0010069E has not been implemented.
        [Test]
        public void V850Dis_EF62()
        {
            AssertCode("@@@", "EF62");
        }

        // ................::...011110.:.:.
        // ................::....::::..:.:. sst_b
        // ...................:.101100.....
        // Reko: a decoder for V850 instruction 8015 at address 001006A2 has not been implemented.
        [Test]
        public void V850Dis_8015()
        {
            AssertCode("@@@", "8015");
        }

        // .................:.::000111:...:
        // Reko: a decoder for V850 instruction F158 at address 001006A4 has not been implemented.
        [Test]
        public void V850Dis_F158()
        {
            AssertCode("@@@", "F158");
        }

        // ....................:011010..:..
        // ....................:.::.:...:.. sld_b
        // ..................:.:101001:...:
        // Reko: a decoder for V850 instruction 312D at address 001006A8 has not been implemented.
        [Test]
        public void V850Dis_312D()
        {
            AssertCode("@@@", "312D");
        }

        // .................:...000100:..::
        // Reko: a decoder for V850 instruction 9340 at address 001006AA has not been implemented.
        [Test]
        public void V850Dis_9340()
        {
            AssertCode("@@@", "9340");
        }

        // ...................:.001010::::.
        // Reko: a decoder for V850 instruction 5E11 at address 001006AC has not been implemented.
        [Test]
        public void V850Dis_5E11()
        {
            AssertCode("@@@", "5E11");
        }

        // .................:::.001110.:::.
        // Reko: a decoder for V850 instruction CE71 at address 001006AE has not been implemented.
        [Test]
        public void V850Dis_CE71()
        {
            AssertCode("@@@", "CE71");
        }

        // ................:::::001000::::.
        // ................:::::..:...::::. or
        // .................:::.010111:.:..
        // Reko: a decoder for V850 instruction F472 at address 001006B2 has not been implemented.
        [Test]
        public void V850Dis_F472()
        {
            AssertCode("@@@", "F472");
        }

        // ..................:..001010::...
        // Reko: a decoder for V850 instruction 5821 at address 001006B4 has not been implemented.
        [Test]
        public void V850Dis_5821()
        {
            AssertCode("@@@", "5821");
        }

        // .................::.:001011.:.::
        // Reko: a decoder for V850 instruction 6B69 at address 001006B6 has not been implemented.
        [Test]
        public void V850Dis_6B69()
        {
            AssertCode("@@@", "6B69");
        }

        // ................:.:::100100:.:..
        // ................:.::::..:..:.:.. sst_h
        // ................::...001000:...:
        // ................::.....:...:...: or
        // .................:.::001000.:.:.
        // .................:.::..:....:.:. or
        // ..................:..110101.::.:
        // Reko: a decoder for V850 instruction AD26 at address 001006BE has not been implemented.
        [Test]
        public void V850Dis_AD26()
        {
            AssertCode("@@@", "AD26");
        }

        // ..................:..101111:::::
        // Reko: a decoder for V850 instruction FF25 at address 001006C0 has not been implemented.
        [Test]
        public void V850Dis_FF25()
        {
            AssertCode("@@@", "FF25");
        }

        // ................:.::.111110:..:.
        // Reko: a decoder for V850 instruction D2B7 at address 001006C2 has not been implemented.
        [Test]
        public void V850Dis_D2B7()
        {
            AssertCode("@@@", "D2B7");
        }

        // ................:::::100010..:..
        // ................::::::...:...:.. sld_h
        // ...................:.101100..:.:
        // Reko: a decoder for V850 instruction 8515 at address 001006C6 has not been implemented.
        [Test]
        public void V850Dis_8515()
        {
            AssertCode("@@@", "8515");
        }

        // ................:::::011111::.::
        // ................:::::.:::::::.:: sst_b
        // ................::.:.001100:..::
        // Reko: a decoder for V850 instruction 93D1 at address 001006CA has not been implemented.
        [Test]
        public void V850Dis_93D1()
        {
            AssertCode("@@@", "93D1");
        }

        // .................::::100100..::.
        // .................:::::..:....::. sst_h
        // ................:.:::111101..::.
        // Reko: a decoder for V850 instruction A6BF at address 001006CE has not been implemented.
        [Test]
        public void V850Dis_A6BF()
        {
            AssertCode("@@@", "A6BF");
        }

        // ................::::.111111.:::.
        // Reko: a decoder for V850 instruction EEF7 at address 001006D0 has not been implemented.
        [Test]
        public void V850Dis_EEF7()
        {
            AssertCode("@@@", "EEF7");
        }

        // ................:...:010010..::.
        // Reko: a decoder for V850 instruction 468A at address 001006D2 has not been implemented.
        [Test]
        public void V850Dis_468A()
        {
            AssertCode("@@@", "468A");
        }

        // .................::::101000.::.:
        // .................:::::.:....::.1 Sld.w/Sst.w
        // .................:::::.:....::.: sst_w
        // ................:.:.:000000.:..:
        // ................10101.......:..:
        // ................:.:.:.......:..: mov
        // ...................:.110010:.:::
        // Reko: a decoder for V850 instruction 5716 at address 001006D8 has not been implemented.
        [Test]
        public void V850Dis_5716()
        {
            AssertCode("@@@", "5716");
        }

        // ..................::.100100....:
        // ..................::.:..:......: sst_h
        // ................:....010010..:::
        // Reko: a decoder for V850 instruction 4782 at address 001006DC has not been implemented.
        [Test]
        public void V850Dis_4782()
        {
            AssertCode("@@@", "4782");
        }

        // ................::.::000000....:
        // ................11011..........:
        // ................::.::..........: mov
        // .................::.:101100.....
        // Reko: a decoder for V850 instruction 806D at address 001006E0 has not been implemented.
        [Test]
        public void V850Dis_806D()
        {
            AssertCode("@@@", "806D");
        }

        // ..................:::000010....:
        // ................00111....:.....:
        // ..................:::....:.00001
        // ..................:::....:.....: divh
        // ...................:.110001::::.
        // Reko: a decoder for V850 instruction 3E16 at address 001006E4 has not been implemented.
        [Test]
        public void V850Dis_3E16()
        {
            AssertCode("@@@", "3E16");
        }

        // ..................:..001001.::..
        // Reko: a decoder for V850 instruction 2C21 at address 001006E6 has not been implemented.
        [Test]
        public void V850Dis_2C21()
        {
            AssertCode("@@@", "2C21");
        }

        // ................::.::110011:.:..
        // Reko: a decoder for V850 instruction 74DE at address 001006E8 has not been implemented.
        [Test]
        public void V850Dis_74DE()
        {
            AssertCode("@@@", "74DE");
        }

        // .................:.::101011:.:::
        // Reko: a decoder for V850 instruction 775D at address 001006EA has not been implemented.
        [Test]
        public void V850Dis_775D()
        {
            AssertCode("@@@", "775D");
        }

        // ................:..::010100.:::.
        // Reko: a decoder for V850 instruction 8E9A at address 001006EC has not been implemented.
        [Test]
        public void V850Dis_8E9A()
        {
            AssertCode("@@@", "8E9A");
        }

        // ...................::000111...::
        // Reko: a decoder for V850 instruction E318 at address 001006EE has not been implemented.
        [Test]
        public void V850Dis_E318()
        {
            AssertCode("@@@", "E318");
        }

        // .................:.:.001000:.::.
        // .................:.:...:...:.::. or
        // ...................:.010110:..:.
        // Reko: a decoder for V850 instruction D212 at address 001006F2 has not been implemented.
        [Test]
        public void V850Dis_D212()
        {
            AssertCode("@@@", "D212");
        }

        // ..................:..010000:..:.
        // ..................:...:....10010
        // ..................:...:....:..:. mov
        // ................:....010000::...
        // ................:.....:....11000
        // ................:.....:....::... mov
        // ..................:.:001000::.::
        // ..................:.:..:...::.:: or
        // ..................:.:110011.....
        // Reko: a decoder for V850 instruction 602E at address 001006FA has not been implemented.
        [Test]
        public void V850Dis_602E()
        {
            AssertCode("@@@", "602E");
        }

        // .................:.:.001011::.::
        // Reko: a decoder for V850 instruction 7B51 at address 001006FC has not been implemented.
        [Test]
        public void V850Dis_7B51()
        {
            AssertCode("@@@", "7B51");
        }

        // .................::::000110.:::.
        // Reko: a decoder for V850 instruction CE78 at address 001006FE has not been implemented.
        [Test]
        public void V850Dis_CE78()
        {
            AssertCode("@@@", "CE78");
        }

        // ................:.:::010111::..:
        // Reko: a decoder for V850 instruction F9BA at address 00100700 has not been implemented.
        [Test]
        public void V850Dis_F9BA()
        {
            AssertCode("@@@", "F9BA");
        }

        // ...................::001010:...:
        // Reko: a decoder for V850 instruction 5119 at address 00100702 has not been implemented.
        [Test]
        public void V850Dis_5119()
        {
            AssertCode("@@@", "5119");
        }

        // .................::.:101001::.:.
        // Reko: a decoder for V850 instruction 3A6D at address 00100704 has not been implemented.
        [Test]
        public void V850Dis_3A6D()
        {
            AssertCode("@@@", "3A6D");
        }

        // ................::.::000100....:
        // Reko: a decoder for V850 instruction 81D8 at address 00100706 has not been implemented.
        [Test]
        public void V850Dis_81D8()
        {
            AssertCode("@@@", "81D8");
        }

        // ..................:..101110.:.:.
        // Reko: a decoder for V850 instruction CA25 at address 00100708 has not been implemented.
        [Test]
        public void V850Dis_CA25()
        {
            AssertCode("@@@", "CA25");
        }

        // ..................:.:110000:.::.
        // Reko: a decoder for V850 instruction 162E at address 0010070A has not been implemented.
        [Test]
        public void V850Dis_162E()
        {
            AssertCode("@@@", "162E");
        }

        // ................:.:.:011000..::.
        // ................:.:.:.::.....::. sld_b
        // ................:.:::101000::.:.
        // ................:.::::.:...::.:0 Sld.w/Sst.w
        // ................:.::::.:...::.:. sld_w
        // .................::.:000001:..::
        // .................::.:.....::..:: not
        // ................::::.110000::::.
        // Reko: a decoder for V850 instruction 1EF6 at address 00100712 has not been implemented.
        [Test]
        public void V850Dis_1EF6()
        {
            AssertCode("@@@", "1EF6");
        }

        // ................:::.:111101:....
        // Reko: a decoder for V850 instruction B0EF at address 00100714 has not been implemented.
        [Test]
        public void V850Dis_B0EF()
        {
            AssertCode("@@@", "B0EF");
        }

        // .................:.::101110..::.
        // Reko: a decoder for V850 instruction C65D at address 00100716 has not been implemented.
        [Test]
        public void V850Dis_C65D()
        {
            AssertCode("@@@", "C65D");
        }

        // ................:.:::110010:.:..
        // Reko: a decoder for V850 instruction 54BE at address 00100718 has not been implemented.
        [Test]
        public void V850Dis_54BE()
        {
            AssertCode("@@@", "54BE");
        }

        // ...................:.100000:::..
        // ...................:.:.....:::.. sld_h
        // ................:::::110101:.::.
        // Reko: a decoder for V850 instruction B6FE at address 0010071C has not been implemented.
        [Test]
        public void V850Dis_B6FE()
        {
            AssertCode("@@@", "B6FE");
        }

        // ..................:..011101..::.
        // ..................:...:::.:..::. sst_b
        // ................:::::110000:....
        // Reko: a decoder for V850 instruction 10FE at address 00100720 has not been implemented.
        [Test]
        public void V850Dis_10FE()
        {
            AssertCode("@@@", "10FE");
        }

        // ................::.:.011011..:..
        // ................::.:..::.::..:.. sld_b
        // ................::..:010010.:...
        // Reko: a decoder for V850 instruction 48CA at address 00100724 has not been implemented.
        [Test]
        public void V850Dis_48CA()
        {
            AssertCode("@@@", "48CA");
        }

        // ................:.:.:000110:::.:
        // Reko: a decoder for V850 instruction DDA8 at address 00100726 has not been implemented.
        [Test]
        public void V850Dis_DDA8()
        {
            AssertCode("@@@", "DDA8");
        }

        // ................:::::001110:....
        // Reko: a decoder for V850 instruction D0F9 at address 00100728 has not been implemented.
        [Test]
        public void V850Dis_D0F9()
        {
            AssertCode("@@@", "D0F9");
        }

        // ................:::.:100010.:...
        // ................:::.::...:..:... sld_h
        // ................:::.:010000.::..
        // ................:::.:.:....01100
        // ................:::.:.:.....::.. mov
        // .................:::.000011:::..
        // Reko: a decoder for V850 instruction 7C70 at address 0010072E has not been implemented.
        [Test]
        public void V850Dis_7C70()
        {
            AssertCode("@@@", "7C70");
        }

        // ................:.:..011000.::::
        // ................:.:...::....:::: sld_b
        // ...................:.010111::.::
        // Reko: a decoder for V850 instruction FB12 at address 00100732 has not been implemented.
        [Test]
        public void V850Dis_FB12()
        {
            AssertCode("@@@", "FB12");
        }

        // ................::.:.111001:..:.
        // Reko: a decoder for V850 instruction 32D7 at address 00100734 has not been implemented.
        [Test]
        public void V850Dis_32D7()
        {
            AssertCode("@@@", "32D7");
        }

        // ....................:010110..:::
        // Reko: a decoder for V850 instruction C70A at address 00100736 has not been implemented.
        [Test]
        public void V850Dis_C70A()
        {
            AssertCode("@@@", "C70A");
        }

        // ................::::.011111:.:..
        // ................::::..::::::.:.. sst_b
        // .................::::100110..:..
        // .................:::::..::...:.. sst_h
        // .................::.:100010:....
        // .................::.::...:.:.... sld_h
        // ................::..:011001...:.
        // ................::..:.::..:...:. sld_b
        // ................::::.011011:::::
        // ................::::..::.::::::: sld_b
        // .....................111000:::.:
        // Reko: a decoder for V850 instruction 1D07 at address 00100742 has not been implemented.
        [Test]
        public void V850Dis_1D07()
        {
            AssertCode("@@@", "1D07");
        }

        // .................:...110010..:::
        // Reko: a decoder for V850 instruction 4746 at address 00100744 has not been implemented.
        [Test]
        public void V850Dis_4746()
        {
            AssertCode("@@@", "4746");
        }

        // .................::.:001011:.:..
        // Reko: a decoder for V850 instruction 7469 at address 00100746 has not been implemented.
        [Test]
        public void V850Dis_7469()
        {
            AssertCode("@@@", "7469");
        }

        // .................:::.110100.:.::
        // Reko: a decoder for V850 instruction 8B76 at address 00100748 has not been implemented.
        [Test]
        public void V850Dis_8B76()
        {
            AssertCode("@@@", "8B76");
        }

        // ................:.::.110001..::.
        // Reko: a decoder for V850 instruction 26B6 at address 0010074A has not been implemented.
        [Test]
        public void V850Dis_26B6()
        {
            AssertCode("@@@", "26B6");
        }

        // ................::.::001101..::.
        // Reko: a decoder for V850 instruction A6D9 at address 0010074C has not been implemented.
        [Test]
        public void V850Dis_A6D9()
        {
            AssertCode("@@@", "A6D9");
        }

        // .................::.:010010:...:
        // ................:.:.:100010...:.
        // ................:.:.::...:....:. sld_h
        // ...................:.010000..:..
        // ...................:..:....00100
        // ...................:..:......:.. mov
        // ................:..:.011000...::
        // ................:..:..::......:: sld_b
        // ..................:::001111:....
        // Reko: a decoder for V850 instruction F039 at address 00100756 has not been implemented.
        [Test]
        public void V850Dis_F039()
        {
            AssertCode("@@@", "F039");
        }

        // .................:.::010001:.:::
        // Reko: a decoder for V850 instruction 375A at address 00100758 has not been implemented.
        [Test]
        public void V850Dis_375A()
        {
            AssertCode("@@@", "375A");
        }

        // .................:.:.101110::::.
        // Reko: a decoder for V850 instruction DE55 at address 0010075A has not been implemented.
        [Test]
        public void V850Dis_DE55()
        {
            AssertCode("@@@", "DE55");
        }

        // .................::..001000..:.:
        // .................::....:.....:.: or
        // .................::..011010:.:::
        // .................::...::.:.:.::: sld_b
        // .................::::100110:.:::
        // .................:::::..::.:.::: sst_h
        // ...................::001011.::.:
        // Reko: a decoder for V850 instruction 6D19 at address 00100762 has not been implemented.
        [Test]
        public void V850Dis_6D19()
        {
            AssertCode("@@@", "6D19");
        }

        // .................:::.100110.::..
        // .................:::.:..::..::.. sst_h
        // ................::..:001001.:...
        // Reko: a decoder for V850 instruction 28C9 at address 00100766 has not been implemented.
        [Test]
        public void V850Dis_28C9()
        {
            AssertCode("@@@", "28C9");
        }

        // ................:..:.001001.:::.
        // Reko: a decoder for V850 instruction 2E91 at address 00100768 has not been implemented.
        [Test]
        public void V850Dis_2E91()
        {
            AssertCode("@@@", "2E91");
        }

        // ................:.:::100100..:::
        // ................:.::::..:....::: sst_h
        // ...................:.000101:...:
        // Reko: a decoder for V850 instruction B110 at address 0010076C has not been implemented.
        [Test]
        public void V850Dis_B110()
        {
            AssertCode("@@@", "B110");
        }

        // ...................::011011...::
        // ...................::.::.::...:: sld_b
        // .................:.::001101...:.
        // Reko: a decoder for V850 instruction A259 at address 00100770 has not been implemented.
        [Test]
        public void V850Dis_A259()
        {
            AssertCode("@@@", "A259");
        }

        // ................:.:..100001::..:
        // ................:.:..:....:::..: sld_h
        // ....................:111011.:...
        // Reko: a decoder for V850 instruction 680F at address 00100774 has not been implemented.
        [Test]
        public void V850Dis_680F()
        {
            AssertCode("@@@", "680F");
        }

        // ................:::.:011110.:...
        // ................:::.:.::::..:... sst_b
        // ................:::..100000...:.
        // ................:::..:........:. sld_h
        // ................:::.:100110:....
        // ................:::.::..::.:.... sst_h
        // ...................::111011..:::
        // Reko: a decoder for V850 instruction 671F at address 0010077C has not been implemented.
        [Test]
        public void V850Dis_671F()
        {
            AssertCode("@@@", "671F");
        }

        // ..................:.:010011...::
        // Reko: a decoder for V850 instruction 632A at address 0010077E has not been implemented.
        [Test]
        public void V850Dis_632A()
        {
            AssertCode("@@@", "632A");
        }

        // ................::...010000::::.
        // ................::....:....11110
        // ................::....:....::::. mov
        // ..................:.:001110::::.
        // Reko: a decoder for V850 instruction DE29 at address 00100782 has not been implemented.
        [Test]
        public void V850Dis_DE29()
        {
            AssertCode("@@@", "DE29");
        }

        // ..................:::000011.:.::
        // Reko: a decoder for V850 instruction 6B38 at address 00100784 has not been implemented.
        [Test]
        public void V850Dis_6B38()
        {
            AssertCode("@@@", "6B38");
        }

        // ................::..:101100::::.
        // Reko: a decoder for V850 instruction 9ECD at address 00100786 has not been implemented.
        [Test]
        public void V850Dis_9ECD()
        {
            AssertCode("@@@", "9ECD");
        }

        // ..................:::011100..:..
        // ..................:::.:::....:.. sst_b
        // .................:::.001111..:..
        // Reko: a decoder for V850 instruction E471 at address 0010078A has not been implemented.
        [Test]
        public void V850Dis_E471()
        {
            AssertCode("@@@", "E471");
        }

        // ..................::.100000.:.::
        // ..................::.:......:.:: sld_h
        // ..................:..110100..:::
        // Reko: a decoder for V850 instruction 8726 at address 0010078E has not been implemented.
        [Test]
        public void V850Dis_8726()
        {
            AssertCode("@@@", "8726");
        }

        // ................::...011111:.::.
        // ................::....::::::.::. sst_b
        // ................:.:.:100101:..:.
        // ................:.:.::..:.::..:. sst_h
        // ................:.::.011001..:::
        // ................:.::..::..:..::: sld_b
        // ................::..:000111::.::
        // Reko: a decoder for V850 instruction FBC8 at address 00100796 has not been implemented.
        [Test]
        public void V850Dis_FBC8()
        {
            AssertCode("@@@", "FBC8");
        }

        // ................:....001101:.:..
        // Reko: a decoder for V850 instruction B481 at address 00100798 has not been implemented.
        [Test]
        public void V850Dis_B481()
        {
            AssertCode("@@@", "B481");
        }

        // ................:::.:000000:.::.
        // ................11101......:.::.
        // ................:::.:......:.::. mov
        // ..................:::011101..:..
        // ..................:::.:::.:..:.. sst_b
        // ....................:111011....:
        // Reko: a decoder for V850 instruction 610F at address 0010079E has not been implemented.
        [Test]
        public void V850Dis_610F()
        {
            AssertCode("@@@", "610F");
        }

        // .................:.::101001.:::.
        // Reko: a decoder for V850 instruction 2E5D at address 001007A0 has not been implemented.
        [Test]
        public void V850Dis_2E5D()
        {
            AssertCode("@@@", "2E5D");
        }

        // ................:::.:110100:::.:
        // Reko: a decoder for V850 instruction 9DEE at address 001007A2 has not been implemented.
        [Test]
        public void V850Dis_9DEE()
        {
            AssertCode("@@@", "9DEE");
        }

        // ..................:::001001:...:
        // Reko: a decoder for V850 instruction 3139 at address 001007A4 has not been implemented.
        [Test]
        public void V850Dis_3139()
        {
            AssertCode("@@@", "3139");
        }

        // .................::.:010101.::::
        // Reko: a decoder for V850 instruction AF6A at address 001007A6 has not been implemented.
        [Test]
        public void V850Dis_AF6A()
        {
            AssertCode("@@@", "AF6A");
        }

        // .................::.:011101::.:.
        // .................::.:.:::.:::.:. sst_b
        // ..................:.:100000:::::
        // ..................:.::.....::::: sld_h
        // .................:.:.111001.:.::
        // Reko: a decoder for V850 instruction 2B57 at address 001007AC has not been implemented.
        [Test]
        public void V850Dis_2B57()
        {
            AssertCode("@@@", "2B57");
        }

        // ................::..:110111.....
        // Reko: a decoder for V850 instruction E0CE at address 001007AE has not been implemented.
        [Test]
        public void V850Dis_E0CE()
        {
            AssertCode("@@@", "E0CE");
        }

        // ................:.:.:001010:::..
        // Reko: a decoder for V850 instruction 5CA9 at address 001007B0 has not been implemented.
        [Test]
        public void V850Dis_5CA9()
        {
            AssertCode("@@@", "5CA9");
        }

        // ................:.:..011111:.::.
        // ................:.:...::::::.::. sst_b
        // ................:.::.110011.::..
        // Reko: a decoder for V850 instruction 6CB6 at address 001007B4 has not been implemented.
        [Test]
        public void V850Dis_6CB6()
        {
            AssertCode("@@@", "6CB6");
        }

        // .................::.:011000...:.
        // .................::.:.::......:. sld_b
        // ...................:.110110..:::
        // Reko: a decoder for V850 instruction C716 at address 001007B8 has not been implemented.
        [Test]
        public void V850Dis_C716()
        {
            AssertCode("@@@", "C716");
        }

        // ................:::.:001011:.:.:
        // Reko: a decoder for V850 instruction 75E9 at address 001007BA has not been implemented.
        [Test]
        public void V850Dis_75E9()
        {
            AssertCode("@@@", "75E9");
        }

        // ................:.::.110001....:
        // Reko: a decoder for V850 instruction 21B6 at address 001007BC has not been implemented.
        [Test]
        public void V850Dis_21B6()
        {
            AssertCode("@@@", "21B6");
        }

        // .................:..:000001.:..:
        // .................:..:.....:.:..: not
        // ................::.:.100110::..:
        // ................::.:.:..::.::..: sst_h
        // ................:.:.:101010...:.
        // Reko: a decoder for V850 instruction 42AD at address 001007C2 has not been implemented.
        [Test]
        public void V850Dis_42AD()
        {
            AssertCode("@@@", "42AD");
        }

        // ................:..::000100:.:::
        // Reko: a decoder for V850 instruction 9798 at address 001007C4 has not been implemented.
        [Test]
        public void V850Dis_9798()
        {
            AssertCode("@@@", "9798");
        }

        // ................:.:::101111:.:..
        // Reko: a decoder for V850 instruction F4BD at address 001007C6 has not been implemented.
        [Test]
        public void V850Dis_F4BD()
        {
            AssertCode("@@@", "F4BD");
        }

        // .................:...111000:..::
        // Reko: a decoder for V850 instruction 1347 at address 001007C8 has not been implemented.
        [Test]
        public void V850Dis_1347()
        {
            AssertCode("@@@", "1347");
        }

        // ................:.:::011111:...:
        // ................:.:::.::::::...: sst_b
        // ................::..:110100:..::
        // Reko: a decoder for V850 instruction 93CE at address 001007CC has not been implemented.
        [Test]
        public void V850Dis_93CE()
        {
            AssertCode("@@@", "93CE");
        }

        // .................:.::101100:::.:
        // Reko: a decoder for V850 instruction 9D5D at address 001007CE has not been implemented.
        [Test]
        public void V850Dis_9D5D()
        {
            AssertCode("@@@", "9D5D");
        }

        // .................:...111101....:
        // Reko: a decoder for V850 instruction A147 at address 001007D0 has not been implemented.
        [Test]
        public void V850Dis_A147()
        {
            AssertCode("@@@", "A147");
        }

        // ................:::::010100.:.::
        // Reko: a decoder for V850 instruction 8BFA at address 001007D2 has not been implemented.
        [Test]
        public void V850Dis_8BFA()
        {
            AssertCode("@@@", "8BFA");
        }

        // ................:.:::101010..:.:
        // Reko: a decoder for V850 instruction 45BD at address 001007D4 has not been implemented.
        [Test]
        public void V850Dis_45BD()
        {
            AssertCode("@@@", "45BD");
        }

        // .................:::.111101...:.
        // Reko: a decoder for V850 instruction A277 at address 001007D6 has not been implemented.
        [Test]
        public void V850Dis_A277()
        {
            AssertCode("@@@", "A277");
        }

        // ..................::.010010::.:.
        // Reko: a decoder for V850 instruction 5A32 at address 001007D8 has not been implemented.
        [Test]
        public void V850Dis_5A32()
        {
            AssertCode("@@@", "5A32");
        }

        // ...................::011001..:::
        // ...................::.::..:..::: sld_b
        // ................::...110110..:..
        // Reko: a decoder for V850 instruction C4C6 at address 001007DC has not been implemented.
        [Test]
        public void V850Dis_C4C6()
        {
            AssertCode("@@@", "C4C6");
        }

        // .....................100010.:...
        // .....................:...:..:... sld_h
        // ................::.:.111010...:.
        // Reko: a decoder for V850 instruction 42D7 at address 001007E0 has not been implemented.
        [Test]
        public void V850Dis_42D7()
        {
            AssertCode("@@@", "42D7");
        }

        // .................:.:.110010:..:.
        // Reko: a decoder for V850 instruction 5256 at address 001007E2 has not been implemented.
        [Test]
        public void V850Dis_5256()
        {
            AssertCode("@@@", "5256");
        }

        // ..................::.010000:.:..
        // ..................::..:....10100
        // ..................::..:....:.:.. mov
        // ................::...011001:.:::
        // ................::....::..::.::: sld_b
        // ..................:::000101:.:.:
        // Reko: a decoder for V850 instruction B538 at address 001007E8 has not been implemented.
        [Test]
        public void V850Dis_B538()
        {
            AssertCode("@@@", "B538");
        }

        // ..................:..101100:....
        // Reko: a decoder for V850 instruction 9025 at address 001007EA has not been implemented.
        [Test]
        public void V850Dis_9025()
        {
            AssertCode("@@@", "9025");
        }

        // .................:...111110..:..
        // Reko: a decoder for V850 instruction C447 at address 001007EC has not been implemented.
        [Test]
        public void V850Dis_C447()
        {
            AssertCode("@@@", "C447");
        }

        // ................:::::000110:.:::
        // Reko: a decoder for V850 instruction D7F8 at address 001007EE has not been implemented.
        [Test]
        public void V850Dis_D7F8()
        {
            AssertCode("@@@", "D7F8");
        }

        // ...................::000011::..:
        // Reko: a decoder for V850 instruction 7918 at address 001007F0 has not been implemented.
        [Test]
        public void V850Dis_7918()
        {
            AssertCode("@@@", "7918");
        }

        // ................::.::001010..:::
        // Reko: a decoder for V850 instruction 47D9 at address 001007F2 has not been implemented.
        [Test]
        public void V850Dis_47D9()
        {
            AssertCode("@@@", "47D9");
        }

        // ..................:.:110001.:.:.
        // Reko: a decoder for V850 instruction 2A2E at address 001007F4 has not been implemented.
        [Test]
        public void V850Dis_2A2E()
        {
            AssertCode("@@@", "2A2E");
        }

        // ...................::100000...:.
        // ...................:::........:. sld_h
        // ................::..:010001:...:
        // Reko: a decoder for V850 instruction 31CA at address 001007F8 has not been implemented.
        [Test]
        public void V850Dis_31CA()
        {
            AssertCode("@@@", "31CA");
        }

        // .................::..100010:..:.
        // .................::..:...:.:..:. sld_h
        // ................::.::000011:...:
        // Reko: a decoder for V850 instruction 71D8 at address 001007FC has not been implemented.
        [Test]
        public void V850Dis_71D8()
        {
            AssertCode("@@@", "71D8");
        }

        // .................:..:101111::..:
        // Reko: a decoder for V850 instruction F94D at address 001007FE has not been implemented.
        [Test]
        public void V850Dis_F94D()
        {
            AssertCode("@@@", "F94D");
        }

        // ................:::.:101111::::.
        // Reko: a decoder for V850 instruction FEED at address 00100800 has not been implemented.
        [Test]
        public void V850Dis_FEED()
        {
            AssertCode("@@@", "FEED");
        }

        // ................:.:::100011::...
        // ................:.::::...::::... sld_h
        // .................:...110011:::..
        // Reko: a decoder for V850 instruction 7C46 at address 00100804 has not been implemented.
        [Test]
        public void V850Dis_7C46()
        {
            AssertCode("@@@", "7C46");
        }

        // .................::.:110010.:...
        // Reko: a decoder for V850 instruction 486E at address 00100806 has not been implemented.
        [Test]
        public void V850Dis_486E()
        {
            AssertCode("@@@", "486E");
        }

        // ................::...111000:....
        // Reko: a decoder for V850 instruction 10C7 at address 00100808 has not been implemented.
        [Test]
        public void V850Dis_10C7()
        {
            AssertCode("@@@", "10C7");
        }

        // ....................:100010..:.:
        // ....................::...:...:.: sld_h
        // .................::::101001.::.:
        // Reko: a decoder for V850 instruction 2D7D at address 0010080C has not been implemented.
        [Test]
        public void V850Dis_2D7D()
        {
            AssertCode("@@@", "2D7D");
        }

        // ...................:.011101:..::
        // ...................:..:::.::..:: sst_b
        // ..................:.:110010::.:.
        // Reko: a decoder for V850 instruction 5A2E at address 00100810 has not been implemented.
        [Test]
        public void V850Dis_5A2E()
        {
            AssertCode("@@@", "5A2E");
        }

        // ................:.:.:011101...:.
        // ................:.:.:.:::.:...:. sst_b
        // .................::.:110011::::.
        // Reko: a decoder for V850 instruction 7E6E at address 00100814 has not been implemented.
        [Test]
        public void V850Dis_7E6E()
        {
            AssertCode("@@@", "7E6E");
        }

        // ...................:.100110::..:
        // ...................:.:..::.::..: sst_h
        // ..................::.110110:.:..
        // Reko: a decoder for V850 instruction D436 at address 00100818 has not been implemented.
        [Test]
        public void V850Dis_D436()
        {
            AssertCode("@@@", "D436");
        }

        // .................:..:001001..:..
        // Reko: a decoder for V850 instruction 2449 at address 0010081A has not been implemented.
        [Test]
        public void V850Dis_2449()
        {
            AssertCode("@@@", "2449");
        }

        // ................::.:.010110:::::
        // Reko: a decoder for V850 instruction DFD2 at address 0010081C has not been implemented.
        [Test]
        public void V850Dis_DFD2()
        {
            AssertCode("@@@", "DFD2");
        }

        // ................::.::100010:..:.
        // ................::.:::...:.:..:. sld_h
        // .................:...011001:::::
        // .................:....::..:::::: sld_b
        // ................::.:.111001..::.
        // Reko: a decoder for V850 instruction 26D7 at address 00100822 has not been implemented.
        [Test]
        public void V850Dis_26D7()
        {
            AssertCode("@@@", "26D7");
        }

        // ...................::011110.:::.
        // ...................::.::::..:::. sst_b
        // ..................::.010011:::..
        // Reko: a decoder for V850 instruction 7C32 at address 00100826 has not been implemented.
        [Test]
        public void V850Dis_7C32()
        {
            AssertCode("@@@", "7C32");
        }

        // ................::.::001110.::::
        // Reko: a decoder for V850 instruction CFD9 at address 00100828 has not been implemented.
        [Test]
        public void V850Dis_CFD9()
        {
            AssertCode("@@@", "CFD9");
        }

        // .................::..011110.:..:
        // .................::...::::..:..: sst_b
        // ................::::.101111.:..:
        // Reko: a decoder for V850 instruction E9F5 at address 0010082C has not been implemented.
        [Test]
        public void V850Dis_E9F5()
        {
            AssertCode("@@@", "E9F5");
        }

        // ................:.::.100111.::::
        // ................:.::.:..:::.:::: sst_h
        // ..................:::111000:.:::
        // Reko: a decoder for V850 instruction 173F at address 00100830 has not been implemented.
        [Test]
        public void V850Dis_173F()
        {
            AssertCode("@@@", "173F");
        }

        // .................:...010000.:.:.
        // .................:....:....01010
        // .................:....:.....:.:. mov
        // .................:..:101001:.::.
        // Reko: a decoder for V850 instruction 364D at address 00100834 has not been implemented.
        [Test]
        public void V850Dis_364D()
        {
            AssertCode("@@@", "364D");
        }

        // ................:..:.000110.:::.
        // Reko: a decoder for V850 instruction CE90 at address 00100836 has not been implemented.
        [Test]
        public void V850Dis_CE90()
        {
            AssertCode("@@@", "CE90");
        }

        // .................::..100000:..::
        // .................::..:.....:..:: sld_h
        // .................:...101111..:::
        // Reko: a decoder for V850 instruction E745 at address 0010083A has not been implemented.
        [Test]
        public void V850Dis_E745()
        {
            AssertCode("@@@", "E745");
        }

        // ................:::::111001....:
        // Reko: a decoder for V850 instruction 21FF at address 0010083C has not been implemented.
        [Test]
        public void V850Dis_21FF()
        {
            AssertCode("@@@", "21FF");
        }

        // ..................:::110100.::::
        // Reko: a decoder for V850 instruction 8F3E at address 0010083E has not been implemented.
        [Test]
        public void V850Dis_8F3E()
        {
            AssertCode("@@@", "8F3E");
        }

        // .................::.:000011:.:..
        // Reko: a decoder for V850 instruction 7468 at address 00100840 has not been implemented.
        [Test]
        public void V850Dis_7468()
        {
            AssertCode("@@@", "7468");
        }

        // ................:::.:001110.::..
        // Reko: a decoder for V850 instruction CCE9 at address 00100842 has not been implemented.
        [Test]
        public void V850Dis_CCE9()
        {
            AssertCode("@@@", "CCE9");
        }

        // ................::.::011010:.:::
        // ................::.::.::.:.:.::: sld_b
        // ..................:::111010..:.:
        // Reko: a decoder for V850 instruction 453F at address 00100846 has not been implemented.
        [Test]
        public void V850Dis_453F()
        {
            AssertCode("@@@", "453F");
        }

        // .................::.:111101:..:.
        // Reko: a decoder for V850 instruction B26F at address 00100848 has not been implemented.
        [Test]
        public void V850Dis_B26F()
        {
            AssertCode("@@@", "B26F");
        }

        // ................:.:..101110:::..
        // Reko: a decoder for V850 instruction DCA5 at address 0010084A has not been implemented.
        [Test]
        public void V850Dis_DCA5()
        {
            AssertCode("@@@", "DCA5");
        }

        // .................:::.110101..:.:
        // Reko: a decoder for V850 instruction A576 at address 0010084C has not been implemented.
        [Test]
        public void V850Dis_A576()
        {
            AssertCode("@@@", "A576");
        }

        // ................::.::110001.:.:.
        // Reko: a decoder for V850 instruction 2ADE at address 0010084E has not been implemented.
        [Test]
        public void V850Dis_2ADE()
        {
            AssertCode("@@@", "2ADE");
        }

        // .................:::.000010..:::
        // ................01110....:...:::
        // .................:::.....:.00111
        // .................:::.....:...::: divh
        // ................:.:..000001:...:
        // ................:.:.......::...: not
        // ...................:.000110..:::
        // Reko: a decoder for V850 instruction C710 at address 00100854 has not been implemented.
        [Test]
        public void V850Dis_C710()
        {
            AssertCode("@@@", "C710");
        }

        // ................::::.010101..:.:
        // Reko: a decoder for V850 instruction A5F2 at address 00100856 has not been implemented.
        [Test]
        public void V850Dis_A5F2()
        {
            AssertCode("@@@", "A5F2");
        }

        // ................:..:.110011:.:..
        // Reko: a decoder for V850 instruction 7496 at address 00100858 has not been implemented.
        [Test]
        public void V850Dis_7496()
        {
            AssertCode("@@@", "7496");
        }

        // .................::.:001110...::
        // Reko: a decoder for V850 instruction C369 at address 0010085A has not been implemented.
        [Test]
        public void V850Dis_C369()
        {
            AssertCode("@@@", "C369");
        }

        // ..................::.110001:..::
        // Reko: a decoder for V850 instruction 3336 at address 0010085C has not been implemented.
        [Test]
        public void V850Dis_3336()
        {
            AssertCode("@@@", "3336");
        }

        // ................::..:111000:...:
        // Reko: a decoder for V850 instruction 11CF at address 0010085E has not been implemented.
        [Test]
        public void V850Dis_11CF()
        {
            AssertCode("@@@", "11CF");
        }

        // ................:...:010010.:.:.
        // Reko: a decoder for V850 instruction 4A8A at address 00100860 has not been implemented.
        [Test]
        public void V850Dis_4A8A()
        {
            AssertCode("@@@", "4A8A");
        }

        // ................:....000111.:::.
        // Reko: a decoder for V850 instruction EE80 at address 00100862 has not been implemented.
        [Test]
        public void V850Dis_EE80()
        {
            AssertCode("@@@", "EE80");
        }

        // .....................101001.:..:
        // Reko: a decoder for V850 instruction 2905 at address 00100864 has not been implemented.
        [Test]
        public void V850Dis_2905()
        {
            AssertCode("@@@", "2905");
        }

        // ..................:::100010:::.:
        // ..................::::...:.:::.: sld_h
        // ................::..:011111::..:
        // ................::..:.:::::::..: sst_b
        // ................::...111100.::::
        // Reko: a decoder for V850 instruction 8FC7 at address 0010086A has not been implemented.
        [Test]
        public void V850Dis_8FC7()
        {
            AssertCode("@@@", "8FC7");
        }

        // ..................:.:000011:...:
        // Reko: a decoder for V850 instruction 7128 at address 0010086C has not been implemented.
        [Test]
        public void V850Dis_7128()
        {
            AssertCode("@@@", "7128");
        }

        // ................:..::101111.:.:.
        // Reko: a decoder for V850 instruction EA9D at address 0010086E has not been implemented.
        [Test]
        public void V850Dis_EA9D()
        {
            AssertCode("@@@", "EA9D");
        }

        // ....................:000001::.:.
        // ....................:.....:::.:. not
        // ................:.::.000111::::.
        // Reko: a decoder for V850 instruction FEB0 at address 00100872 has not been implemented.
        [Test]
        public void V850Dis_FEB0()
        {
            AssertCode("@@@", "FEB0");
        }

        // ................:::.:110110.:::.
        // Reko: a decoder for V850 instruction CEEE at address 00100874 has not been implemented.
        [Test]
        public void V850Dis_CEEE()
        {
            AssertCode("@@@", "CEEE");
        }

        // .................::..100011:.::.
        // .................::..:...:::.::. sld_h
        // ................::.::010110...::
        // Reko: a decoder for V850 instruction C3DA at address 00100878 has not been implemented.
        [Test]
        public void V850Dis_C3DA()
        {
            AssertCode("@@@", "C3DA");
        }

        // ................::...001011:.:..
        // Reko: a decoder for V850 instruction 74C1 at address 0010087A has not been implemented.
        [Test]
        public void V850Dis_74C1()
        {
            AssertCode("@@@", "74C1");
        }

        // ................::.::100000:.:::
        // ................::.:::.....:.::: sld_h
        // .................::::011000.:.::
        // .................::::.::....:.:: sld_b
        // ....................:100010::::.
        // ....................::...:.::::. sld_h
        // ...................::010010::.::
        // Reko: a decoder for V850 instruction 5B1A at address 00100882 has not been implemented.
        [Test]
        public void V850Dis_5B1A()
        {
            AssertCode("@@@", "5B1A");
        }

        // ................:.:.:001100..:::
        // Reko: a decoder for V850 instruction 87A9 at address 00100884 has not been implemented.
        [Test]
        public void V850Dis_87A9()
        {
            AssertCode("@@@", "87A9");
        }

        // .................:...010101:.:.:
        // Reko: a decoder for V850 instruction B542 at address 00100886 has not been implemented.
        [Test]
        public void V850Dis_B542()
        {
            AssertCode("@@@", "B542");
        }

        // ................::::.101000:..::
        // ................::::.:.:...:..:1 Sld.w/Sst.w
        // ................::::.:.:...:..:: sst_w
        // ................:::::100101..::.
        // ................::::::..:.:..::. sst_h
        // ................::.::110100.....
        // Reko: a decoder for V850 instruction 80DE at address 0010088C has not been implemented.
        [Test]
        public void V850Dis_80DE()
        {
            AssertCode("@@@", "80DE");
        }

        // .................:..:100100.....
        // .................:..::..:....... sst_h
        // ..................:..101101.:.::
        // Reko: a decoder for V850 instruction AB25 at address 00100890 has not been implemented.
        [Test]
        public void V850Dis_AB25()
        {
            AssertCode("@@@", "AB25");
        }

        // ..................:.:011001.::::
        // ..................:.:.::..:.:::: sld_b
        // .................::..000001::...
        // .................::.......:::... not
        // .................:.::011000....:
        // .................:.::.::.......: sld_b
        // ................:...:010000:..::
        // ................:...:.:....10011
        // ................:...:.:....:..:: mov
        // .................:..:110101:::.:
        // Reko: a decoder for V850 instruction BD4E at address 0010089A has not been implemented.
        [Test]
        public void V850Dis_BD4E()
        {
            AssertCode("@@@", "BD4E");
        }

        // ................:..::100100:....
        // ................:..:::..:..:.... sst_h
        // ...................::101001..:..
        // Reko: a decoder for V850 instruction 241D at address 0010089E has not been implemented.
        [Test]
        public void V850Dis_241D()
        {
            AssertCode("@@@", "241D");
        }

        // ...................:.011101:::::
        // ...................:..:::.:::::: sst_b
        // .................::..101011.:..:
        // Reko: a decoder for V850 instruction 6965 at address 001008A2 has not been implemented.
        [Test]
        public void V850Dis_6965()
        {
            AssertCode("@@@", "6965");
        }

        // ................::..:111110.::.:
        // Reko: a decoder for V850 instruction CDCF at address 001008A4 has not been implemented.
        [Test]
        public void V850Dis_CDCF()
        {
            AssertCode("@@@", "CDCF");
        }

        // ................:..:.000000:.:.:
        // ................10010......:.:.:
        // ................:..:.......:.:.: mov
        // ................:.:::100010:..:.
        // ................:.::::...:.:..:. sld_h
        // ................::.::000100:::.:
        // Reko: a decoder for V850 instruction 9DD8 at address 001008AA has not been implemented.
        [Test]
        public void V850Dis_9DD8()
        {
            AssertCode("@@@", "9DD8");
        }

        // .................::::010010..:::
        // Reko: a decoder for V850 instruction 477A at address 001008AC has not been implemented.
        [Test]
        public void V850Dis_477A()
        {
            AssertCode("@@@", "477A");
        }

        // ................:::..100111...::
        // ................:::..:..:::...:: sst_h
        // ..................:.:000010::..:
        // ................00101....:.::..:
        // ..................:.:....:.11001
        // ..................:.:....:.::..: divh
        // ................::::.010000:.:.:
        // ................::::..:....10101
        // ................::::..:....:.:.: mov
        // ................:::..000101.::.:
        // Reko: a decoder for V850 instruction ADE0 at address 001008B4 has not been implemented.
        [Test]
        public void V850Dis_ADE0()
        {
            AssertCode("@@@", "ADE0");
        }

        // ................:::::110010...::
        // Reko: a decoder for V850 instruction 43FE at address 001008B6 has not been implemented.
        [Test]
        public void V850Dis_43FE()
        {
            AssertCode("@@@", "43FE");
        }

        // .....................000000.:.:.
        // ................00000.......:.:.
        // ...........................01010
        // ............................:.:. invalid
        // ................:::::101000..:::
        // ................::::::.:.....::1 Sld.w/Sst.w
        // ................::::::.:.....::: sst_w
        // .................::..111111.::..
        // Reko: a decoder for V850 instruction EC67 at address 001008BC has not been implemented.
        [Test]
        public void V850Dis_EC67()
        {
            AssertCode("@@@", "EC67");
        }

        // .................:::.111101:...:
        // Reko: a decoder for V850 instruction B177 at address 001008BE has not been implemented.
        [Test]
        public void V850Dis_B177()
        {
            AssertCode("@@@", "B177");
        }

        // ................:...:001110:....
        // Reko: a decoder for V850 instruction D089 at address 001008C0 has not been implemented.
        [Test]
        public void V850Dis_D089()
        {
            AssertCode("@@@", "D089");
        }

        // .................::.:100001:::.:
        // .................::.::....::::.: sld_h
        // ...................::011011:.:.:
        // ...................::.::.:::.:.: sld_b
        // ................::.::011011::::.
        // ................::.::.::.::::::. sld_b
        // ................:..::110000:....
        // Reko: a decoder for V850 instruction 109E at address 001008C8 has not been implemented.
        [Test]
        public void V850Dis_109E()
        {
            AssertCode("@@@", "109E");
        }

        // .................::::011110::...
        // .................::::.::::.::... sst_b
        // ..................:.:001110...:.
        // Reko: a decoder for V850 instruction C229 at address 001008CC has not been implemented.
        [Test]
        public void V850Dis_C229()
        {
            AssertCode("@@@", "C229");
        }

        // ................:..::001000:..::
        // ................:..::..:...:..:: or
        // ................::.::001101..::.
        // .................::..111111:.:..
        // Reko: a decoder for V850 instruction F467 at address 001008D2 has not been implemented.
        [Test]
        public void V850Dis_F467()
        {
            AssertCode("@@@", "F467");
        }

        // ..................::.010100..::.
        // Reko: a decoder for V850 instruction 8632 at address 001008D4 has not been implemented.
        [Test]
        public void V850Dis_8632()
        {
            AssertCode("@@@", "8632");
        }

        // ................::.::110011:....
        // Reko: a decoder for V850 instruction 70DE at address 001008D6 has not been implemented.
        [Test]
        public void V850Dis_70DE()
        {
            AssertCode("@@@", "70DE");
        }

        // .................::.:010110:....
        // Reko: a decoder for V850 instruction D06A at address 001008D8 has not been implemented.
        [Test]
        public void V850Dis_D06A()
        {
            AssertCode("@@@", "D06A");
        }

        // ................::..:101010::.::
        // Reko: a decoder for V850 instruction 5BCD at address 001008DA has not been implemented.
        [Test]
        public void V850Dis_5BCD()
        {
            AssertCode("@@@", "5BCD");
        }

        // ...................::000110.:...
        // Reko: a decoder for V850 instruction C818 at address 001008DC has not been implemented.
        [Test]
        public void V850Dis_C818()
        {
            AssertCode("@@@", "C818");
        }

        // ................:::.:011101..:..
        // ................:::.:.:::.:..:.. sst_b
        // ..................:..110000.:.:.
        // Reko: a decoder for V850 instruction 0A26 at address 001008E0 has not been implemented.
        [Test]
        public void V850Dis_0A26()
        {
            AssertCode("@@@", "0A26");
        }

        // ................:.:::110000..:::
        // Reko: a decoder for V850 instruction 07BE at address 001008E2 has not been implemented.
        [Test]
        public void V850Dis_07BE()
        {
            AssertCode("@@@", "07BE");
        }

        // .................:::.110001:::..
        // Reko: a decoder for V850 instruction 3C76 at address 001008E4 has not been implemented.
        [Test]
        public void V850Dis_3C76()
        {
            AssertCode("@@@", "3C76");
        }

        // ..................:::110011.::::
        // Reko: a decoder for V850 instruction 6F3E at address 001008E6 has not been implemented.
        [Test]
        public void V850Dis_6F3E()
        {
            AssertCode("@@@", "6F3E");
        }

        // ..................:::001101.:..:
        // Reko: a decoder for V850 instruction A939 at address 001008E8 has not been implemented.
        [Test]
        public void V850Dis_A939()
        {
            AssertCode("@@@", "A939");
        }

        // ....................:110111....:
        // Reko: a decoder for V850 instruction E10E at address 001008EA has not been implemented.
        [Test]
        public void V850Dis_E10E()
        {
            AssertCode("@@@", "E10E");
        }

        // ................::...111000.:...
        // Reko: a decoder for V850 instruction 08C7 at address 001008EC has not been implemented.
        [Test]
        public void V850Dis_08C7()
        {
            AssertCode("@@@", "08C7");
        }

        // ................:::..001001::.::
        // Reko: a decoder for V850 instruction 3BE1 at address 001008EE has not been implemented.
        [Test]
        public void V850Dis_3BE1()
        {
            AssertCode("@@@", "3BE1");
        }

        // .................::.:101111.::..
        // Reko: a decoder for V850 instruction EC6D at address 001008F0 has not been implemented.
        [Test]
        public void V850Dis_EC6D()
        {
            AssertCode("@@@", "EC6D");
        }

        // ...................:.011010:.:::
        // ...................:..::.:.:.::: sld_b
        // .................:..:010011:..:.
        // Reko: a decoder for V850 instruction 724A at address 001008F4 has not been implemented.
        [Test]
        public void V850Dis_724A()
        {
            AssertCode("@@@", "724A");
        }

        // ................:..::110111:....
        // Reko: a decoder for V850 instruction F09E at address 001008F6 has not been implemented.
        [Test]
        public void V850Dis_F09E()
        {
            AssertCode("@@@", "F09E");
        }

        // .................:.::110000::..:
        // Reko: a decoder for V850 instruction 195E at address 001008F8 has not been implemented.
        [Test]
        public void V850Dis_195E()
        {
            AssertCode("@@@", "195E");
        }

        // ....................:010100:::..
        // Reko: a decoder for V850 instruction 9C0A at address 001008FA has not been implemented.
        [Test]
        public void V850Dis_9C0A()
        {
            AssertCode("@@@", "9C0A");
        }

        // ................:.::.001110.....
        // Reko: a decoder for V850 instruction C0B1 at address 001008FC has not been implemented.
        [Test]
        public void V850Dis_C0B1()
        {
            AssertCode("@@@", "C0B1");
        }

        // ................::::.111000:..::
        // Reko: a decoder for V850 instruction 13F7 at address 001008FE has not been implemented.
        [Test]
        public void V850Dis_13F7()
        {
            AssertCode("@@@", "13F7");
        }

        // ................:::.:100111::.:.
        // ................:::.::..:::::.:. sst_h
        // ................:..::100011:...:
        // ................:..:::...:::...: sld_h
        // .................:.:.101001....:
        // Reko: a decoder for V850 instruction 2155 at address 00100904 has not been implemented.
        [Test]
        public void V850Dis_2155()
        {
            AssertCode("@@@", "2155");
        }

        // ..................:.:111010:::..
        // Reko: a decoder for V850 instruction 5C2F at address 00100906 has not been implemented.
        [Test]
        public void V850Dis_5C2F()
        {
            AssertCode("@@@", "5C2F");
        }

        // ................:.::.110011.:..:
        // Reko: a decoder for V850 instruction 69B6 at address 00100908 has not been implemented.
        [Test]
        public void V850Dis_69B6()
        {
            AssertCode("@@@", "69B6");
        }

        // .................:..:101101:::.:
        // Reko: a decoder for V850 instruction BD4D at address 0010090A has not been implemented.
        [Test]
        public void V850Dis_BD4D()
        {
            AssertCode("@@@", "BD4D");
        }

        // .................::..001010:....
        // Reko: a decoder for V850 instruction 5061 at address 0010090C has not been implemented.
        [Test]
        public void V850Dis_5061()
        {
            AssertCode("@@@", "5061");
        }

        // ....................:000110:.::.
        // Reko: a decoder for V850 instruction D608 at address 0010090E has not been implemented.
        [Test]
        public void V850Dis_D608()
        {
            AssertCode("@@@", "D608");
        }

        // .................::::010001.::::
        // Reko: a decoder for V850 instruction 2F7A at address 00100910 has not been implemented.
        [Test]
        public void V850Dis_2F7A()
        {
            AssertCode("@@@", "2F7A");
        }

        // .................:.::010111....:
        // Reko: a decoder for V850 instruction E15A at address 00100912 has not been implemented.
        [Test]
        public void V850Dis_E15A()
        {
            AssertCode("@@@", "E15A");
        }

        // ................:..:.001110.....
        // Reko: a decoder for V850 instruction C091 at address 00100914 has not been implemented.
        [Test]
        public void V850Dis_C091()
        {
            AssertCode("@@@", "C091");
        }

        // ................:.:::111011::...
        // Reko: a decoder for V850 instruction 78BF at address 00100916 has not been implemented.
        [Test]
        public void V850Dis_78BF()
        {
            AssertCode("@@@", "78BF");
        }

        // .................::.:001001:.:.:
        // Reko: a decoder for V850 instruction 3569 at address 00100918 has not been implemented.
        [Test]
        public void V850Dis_3569()
        {
            AssertCode("@@@", "3569");
        }

        // ................::.::110101..:.:
        // Reko: a decoder for V850 instruction A5DE at address 0010091A has not been implemented.
        [Test]
        public void V850Dis_A5DE()
        {
            AssertCode("@@@", "A5DE");
        }

        // .................::..101110::..:
        // Reko: a decoder for V850 instruction D965 at address 0010091C has not been implemented.
        [Test]
        public void V850Dis_D965()
        {
            AssertCode("@@@", "D965");
        }

        // ................:::::000011:::.:
        // ................:::.:010001..:.:
        // Reko: a decoder for V850 instruction 25EA at address 00100920 has not been implemented.
        [Test]
        public void V850Dis_25EA()
        {
            AssertCode("@@@", "25EA");
        }

        // ...................::100000:..::
        // ...................:::.....:..:: sld_h
        // ................:..::111010:.::.
        // Reko: a decoder for V850 instruction 569F at address 00100924 has not been implemented.
        [Test]
        public void V850Dis_569F()
        {
            AssertCode("@@@", "569F");
        }

        // ................:..:.110110.....
        // Reko: a decoder for V850 instruction C096 at address 00100926 has not been implemented.
        [Test]
        public void V850Dis_C096()
        {
            AssertCode("@@@", "C096");
        }

        // ..................:..111000:....
        // Reko: a decoder for V850 instruction 1027 at address 00100928 has not been implemented.
        [Test]
        public void V850Dis_1027()
        {
            AssertCode("@@@", "1027");
        }

        // ................:.:::100101.:..:
        // ................:.::::..:.:.:..: sst_h
        // ................:.:..011100.::..
        // ................:.:...:::...::.. sst_b
        // ................:.::.111010.:::.
        // Reko: a decoder for V850 instruction 4EB7 at address 0010092E has not been implemented.
        [Test]
        public void V850Dis_4EB7()
        {
            AssertCode("@@@", "4EB7");
        }

        // ................:..:.100100.:...
        // ................:..:.:..:...:... sst_h
        // .................:...110110::.::
        // Reko: a decoder for V850 instruction DB46 at address 00100932 has not been implemented.
        [Test]
        public void V850Dis_DB46()
        {
            AssertCode("@@@", "DB46");
        }

        // ................:.:::000110:....
        // Reko: a decoder for V850 instruction D0B8 at address 00100934 has not been implemented.
        [Test]
        public void V850Dis_D0B8()
        {
            AssertCode("@@@", "D0B8");
        }

        // .................::.:001001:.:::
        // Reko: a decoder for V850 instruction 3769 at address 00100936 has not been implemented.
        [Test]
        public void V850Dis_3769()
        {
            AssertCode("@@@", "3769");
        }

        // ................:.::.001011:..::
        // Reko: a decoder for V850 instruction 73B1 at address 00100938 has not been implemented.
        [Test]
        public void V850Dis_73B1()
        {
            AssertCode("@@@", "73B1");
        }

        // ................:.::.111011..::.
        // Reko: a decoder for V850 instruction 66B7 at address 0010093A has not been implemented.
        [Test]
        public void V850Dis_66B7()
        {
            AssertCode("@@@", "66B7");
        }

        // .................::::110101:....
        // Reko: a decoder for V850 instruction B07E at address 0010093C has not been implemented.
        [Test]
        public void V850Dis_B07E()
        {
            AssertCode("@@@", "B07E");
        }

        // .....................100010:.:.:
        // .....................:...:.:.:.: sld_h
        // ................::...101001::...
        // Reko: a decoder for V850 instruction 38C5 at address 00100940 has not been implemented.
        [Test]
        public void V850Dis_38C5()
        {
            AssertCode("@@@", "38C5");
        }

        // .................::..110001..:::
        // Reko: a decoder for V850 instruction 2766 at address 00100942 has not been implemented.
        [Test]
        public void V850Dis_2766()
        {
            AssertCode("@@@", "2766");
        }

        // ................:.:::010010.::.:
        // Reko: a decoder for V850 instruction 4DBA at address 00100944 has not been implemented.
        [Test]
        public void V850Dis_4DBA()
        {
            AssertCode("@@@", "4DBA");
        }

        // ................:..:.000101:..:.
        // Reko: a decoder for V850 instruction B290 at address 00100946 has not been implemented.
        [Test]
        public void V850Dis_B290()
        {
            AssertCode("@@@", "B290");
        }

        // .................:..:010101::..:
        // Reko: a decoder for V850 instruction B94A at address 00100948 has not been implemented.
        [Test]
        public void V850Dis_B94A()
        {
            AssertCode("@@@", "B94A");
        }

        // ................:..::001010.:..:
        // Reko: a decoder for V850 instruction 4999 at address 0010094A has not been implemented.
        [Test]
        public void V850Dis_4999()
        {
            AssertCode("@@@", "4999");
        }

        // ................:.:::100111..:::
        // ................:.::::..:::..::: sst_h
        // ................:..:.010001:..::
        // Reko: a decoder for V850 instruction 3392 at address 0010094E has not been implemented.
        [Test]
        public void V850Dis_3392()
        {
            AssertCode("@@@", "3392");
        }

        // ................:::.:101000::.::
        // ................:::.::.:...::.:1 Sld.w/Sst.w
        // ................:::.::.:...::.:: sst_w
        // .................:...101010:.::.
        // Reko: a decoder for V850 instruction 5645 at address 00100952 has not been implemented.
        [Test]
        public void V850Dis_5645()
        {
            AssertCode("@@@", "5645");
        }

        // ..................::.111100.:.:.
        // Reko: a decoder for V850 instruction 8A37 at address 00100954 has not been implemented.
        [Test]
        public void V850Dis_8A37()
        {
            AssertCode("@@@", "8A37");
        }

        // .................::.:100001.:...
        // .................::.::....:.:... sld_h
        // ................:.:.:010101:..:.
        // Reko: a decoder for V850 instruction B2AA at address 00100958 has not been implemented.
        [Test]
        public void V850Dis_B2AA()
        {
            AssertCode("@@@", "B2AA");
        }

        // ................:.:..100101.:...
        // ................:.:..:..:.:.:... sst_h
        // ....................:000001::...
        // ....................:.....:::... not
        // ................:..:.010111..::.
        // Reko: a decoder for V850 instruction E692 at address 0010095E has not been implemented.
        [Test]
        public void V850Dis_E692()
        {
            AssertCode("@@@", "E692");
        }

        // ................:.:..101110:...:
        // Reko: a decoder for V850 instruction D1A5 at address 00100960 has not been implemented.
        [Test]
        public void V850Dis_D1A5()
        {
            AssertCode("@@@", "D1A5");
        }

        // ................::...111100...::
        // Reko: a decoder for V850 instruction 83C7 at address 00100962 has not been implemented.
        [Test]
        public void V850Dis_83C7()
        {
            AssertCode("@@@", "83C7");
        }

        // ................:::..111011:::..
        // Reko: a decoder for V850 instruction 7CE7 at address 00100964 has not been implemented.
        [Test]
        public void V850Dis_7CE7()
        {
            AssertCode("@@@", "7CE7");
        }

        // ................::..:101011.:..:
        // Reko: a decoder for V850 instruction 69CD at address 00100966 has not been implemented.
        [Test]
        public void V850Dis_69CD()
        {
            AssertCode("@@@", "69CD");
        }

        // .................:..:010111...:.
        // Reko: a decoder for V850 instruction E24A at address 00100968 has not been implemented.
        [Test]
        public void V850Dis_E24A()
        {
            AssertCode("@@@", "E24A");
        }

        // ................:....101100.::..
        // Reko: a decoder for V850 instruction 8C85 at address 0010096A has not been implemented.
        [Test]
        public void V850Dis_8C85()
        {
            AssertCode("@@@", "8C85");
        }

        // ................:..::110011.::::
        // Reko: a decoder for V850 instruction 6F9E at address 0010096C has not been implemented.
        [Test]
        public void V850Dis_6F9E()
        {
            AssertCode("@@@", "6F9E");
        }

        // ................:.::.110100...:.
        // Reko: a decoder for V850 instruction 82B6 at address 0010096E has not been implemented.
        [Test]
        public void V850Dis_82B6()
        {
            AssertCode("@@@", "82B6");
        }

        // ................::.:.100011::::.
        // ................::.:.:...::::::. sld_h
        // ................::...010100:::..
        // Reko: a decoder for V850 instruction 9CC2 at address 00100972 has not been implemented.
        [Test]
        public void V850Dis_9CC2()
        {
            AssertCode("@@@", "9CC2");
        }

        // ...................:.000001.:...
        // ...................:......:.:... not
        // .....................001011::.:.
        // Reko: a decoder for V850 instruction 7A01 at address 00100976 has not been implemented.
        [Test]
        public void V850Dis_7A01()
        {
            AssertCode("@@@", "7A01");
        }

        // ................:.:::010100:::.:
        // Reko: a decoder for V850 instruction 9DBA at address 00100978 has not been implemented.
        [Test]
        public void V850Dis_9DBA()
        {
            AssertCode("@@@", "9DBA");
        }

        // ................:.:..011101:..::
        // ................:.:...:::.::..:: sst_b
        // ....................:101000:...:
        // ....................::.:...:...1 Sld.w/Sst.w
        // ....................::.:...:...: sst_w
        // ................:.::.001010:.:::
        // Reko: a decoder for V850 instruction 57B1 at address 0010097E has not been implemented.
        [Test]
        public void V850Dis_57B1()
        {
            AssertCode("@@@", "57B1");
        }

        // ................:.::.111011..:..
        // Reko: a decoder for V850 instruction 64B7 at address 00100980 has not been implemented.
        [Test]
        public void V850Dis_64B7()
        {
            AssertCode("@@@", "64B7");
        }

        // .................:...010110..:::
        // Reko: a decoder for V850 instruction C742 at address 00100982 has not been implemented.
        [Test]
        public void V850Dis_C742()
        {
            AssertCode("@@@", "C742");
        }

        // .................::.:011001::..:
        // .................::.:.::..:::..: sld_b
        // ..................::.011000:.::.
        // ..................::..::...:.::. sld_b
        // ................:...:000100..:..
        // Reko: a decoder for V850 instruction 8488 at address 00100988 has not been implemented.
        [Test]
        public void V850Dis_8488()
        {
            AssertCode("@@@", "8488");
        }

        // ..................:::111011...::
        // Reko: a decoder for V850 instruction 633F at address 0010098A has not been implemented.
        [Test]
        public void V850Dis_633F()
        {
            AssertCode("@@@", "633F");
        }

        // ................:.:..010101.:.:.
        // Reko: a decoder for V850 instruction AAA2 at address 0010098C has not been implemented.
        [Test]
        public void V850Dis_AAA2()
        {
            AssertCode("@@@", "AAA2");
        }

        // ...................:.100111:::.:
        // ...................:.:..::::::.: sst_h
        // ................::::.001001.:...
        // Reko: a decoder for V850 instruction 28F1 at address 00100990 has not been implemented.
        [Test]
        public void V850Dis_28F1()
        {
            AssertCode("@@@", "28F1");
        }

        // .................::..100001..::.
        // .................::..:....:..::. sld_h
        // ..................:..011011.::..
        // ..................:...::.::.::.. sld_b
        // ................::...001011.:.:.
        // Reko: a decoder for V850 instruction 6AC1 at address 00100996 has not been implemented.
        [Test]
        public void V850Dis_6AC1()
        {
            AssertCode("@@@", "6AC1");
        }

        // ................:....001001.:.:.
        // Reko: a decoder for V850 instruction 2A81 at address 00100998 has not been implemented.
        [Test]
        public void V850Dis_2A81()
        {
            AssertCode("@@@", "2A81");
        }

        // ................::...001001.:.:.
        // Reko: a decoder for V850 instruction 2AC1 at address 0010099A has not been implemented.
        [Test]
        public void V850Dis_2AC1()
        {
            AssertCode("@@@", "2AC1");
        }

        // ................::...101001:.:..
        // Reko: a decoder for V850 instruction 34C5 at address 0010099C has not been implemented.
        [Test]
        public void V850Dis_34C5()
        {
            AssertCode("@@@", "34C5");
        }

        // ................::.:.100101:::..
        // ................::.:.:..:.::::.. sst_h
        // ................::.::011111:..::
        // ................::.::.::::::..:: sst_b
        // ................:.::.111001....:
        // Reko: a decoder for V850 instruction 21B7 at address 001009A2 has not been implemented.
        [Test]
        public void V850Dis_21B7()
        {
            AssertCode("@@@", "21B7");
        }

        // ..................:::111110:.::.
        // Reko: a decoder for V850 instruction D63F at address 001009A4 has not been implemented.
        [Test]
        public void V850Dis_D63F()
        {
            AssertCode("@@@", "D63F");
        }

        // ...................:.011011:::.:
        // ...................:..::.:::::.: sld_b
        // .................::.:001101:::.:
        // Reko: a decoder for V850 instruction BD69 at address 001009A8 has not been implemented.
        [Test]
        public void V850Dis_BD69()
        {
            AssertCode("@@@", "BD69");
        }

        // ................:..::111001::::.
        // Reko: a decoder for V850 instruction 3E9F at address 001009AA has not been implemented.
        [Test]
        public void V850Dis_3E9F()
        {
            AssertCode("@@@", "3E9F");
        }

        // ..................:::011101.::..
        // ..................:::.:::.:.::.. sst_b
        // ................:::::010010:.::.
        // Reko: a decoder for V850 instruction 56FA at address 001009AE has not been implemented.
        [Test]
        public void V850Dis_56FA()
        {
            AssertCode("@@@", "56FA");
        }

        // ................:..::110101:::..
        // Reko: a decoder for V850 instruction BC9E at address 001009B0 has not been implemented.
        [Test]
        public void V850Dis_BC9E()
        {
            AssertCode("@@@", "BC9E");
        }

        // ...................::111011::.::
        // Reko: a decoder for V850 instruction 7B1F at address 001009B2 has not been implemented.
        [Test]
        public void V850Dis_7B1F()
        {
            AssertCode("@@@", "7B1F");
        }

        // ................::::.010111..:::
        // Reko: a decoder for V850 instruction E7F2 at address 001009B4 has not been implemented.
        [Test]
        public void V850Dis_E7F2()
        {
            AssertCode("@@@", "E7F2");
        }

        // .................:...000010..:.:
        // ................01000....:...:.:
        // .................:.......:.00101
        // .................:.......:...:.: divh
        // .....................101010.::.:
        // Reko: a decoder for V850 instruction 4D05 at address 001009B8 has not been implemented.
        [Test]
        public void V850Dis_4D05()
        {
            AssertCode("@@@", "4D05");
        }

        // ................:.::.111000:.:::
        // Reko: a decoder for V850 instruction 17B7 at address 001009BA has not been implemented.
        [Test]
        public void V850Dis_17B7()
        {
            AssertCode("@@@", "17B7");
        }

        // .................:.:.100010.....
        // .................:.:.:...:...... sld_h
        // ................:.:::110111::::.
        // Reko: a decoder for V850 instruction FEBE at address 001009BE has not been implemented.
        [Test]
        public void V850Dis_FEBE()
        {
            AssertCode("@@@", "FEBE");
        }

        // ................:...:110110.::..
        // Reko: a decoder for V850 instruction CC8E at address 001009C0 has not been implemented.
        [Test]
        public void V850Dis_CC8E()
        {
            AssertCode("@@@", "CC8E");
        }

        // ................::..:111010.:.::
        // Reko: a decoder for V850 instruction 4BCF at address 001009C2 has not been implemented.
        [Test]
        public void V850Dis_4BCF()
        {
            AssertCode("@@@", "4BCF");
        }

        // .................:...110100.....
        // Reko: a decoder for V850 instruction 8046 at address 001009C4 has not been implemented.
        [Test]
        public void V850Dis_8046()
        {
            AssertCode("@@@", "8046");
        }

        // ................:::.:000001::::.
        // ................:::.:.....:::::. not
        // ...................:.010011:.:..
        // Reko: a decoder for V850 instruction 7412 at address 001009C8 has not been implemented.
        [Test]
        public void V850Dis_7412()
        {
            AssertCode("@@@", "7412");
        }

        // .....................011101..::.
        // ......................:::.:..::. sst_b
        // ................::..:010111..:.:
        // Reko: a decoder for V850 instruction E5CA at address 001009CC has not been implemented.
        [Test]
        public void V850Dis_E5CA()
        {
            AssertCode("@@@", "E5CA");
        }

        // ................:::.:111000:.:..
        // Reko: a decoder for V850 instruction 14EF at address 001009CE has not been implemented.
        [Test]
        public void V850Dis_14EF()
        {
            AssertCode("@@@", "14EF");
        }

        // ..................:.:111001.:.::
        // Reko: a decoder for V850 instruction 2B2F at address 001009D0 has not been implemented.
        [Test]
        public void V850Dis_2B2F()
        {
            AssertCode("@@@", "2B2F");
        }

        // ................:..:.110000..:.:
        // Reko: a decoder for V850 instruction 0596 at address 001009D2 has not been implemented.
        [Test]
        public void V850Dis_0596()
        {
            AssertCode("@@@", "0596");
        }

        // ................:..::100010.:.:.
        // ................:..:::...:..:.:. sld_h
        // .....................000111.::.:
        // Reko: a decoder for V850 instruction ED00 at address 001009D6 has not been implemented.
        [Test]
        public void V850Dis_ED00()
        {
            AssertCode("@@@", "ED00");
        }

        // ................:::..000100:.::.
        // Reko: a decoder for V850 instruction 96E0 at address 001009D8 has not been implemented.
        [Test]
        public void V850Dis_96E0()
        {
            AssertCode("@@@", "96E0");
        }

        // ................::.:.001011.:.:.
        // Reko: a decoder for V850 instruction 6AD1 at address 001009DA has not been implemented.
        [Test]
        public void V850Dis_6AD1()
        {
            AssertCode("@@@", "6AD1");
        }

        // ................::...101001..:::
        // Reko: a decoder for V850 instruction 27C5 at address 001009DC has not been implemented.
        [Test]
        public void V850Dis_27C5()
        {
            AssertCode("@@@", "27C5");
        }

        // .................::.:001110:..:.
        // Reko: a decoder for V850 instruction D269 at address 001009DE has not been implemented.
        [Test]
        public void V850Dis_D269()
        {
            AssertCode("@@@", "D269");
        }

        // .................::::111011.:.:.
        // Reko: a decoder for V850 instruction 6A7F at address 001009E0 has not been implemented.
        [Test]
        public void V850Dis_6A7F()
        {
            AssertCode("@@@", "6A7F");
        }

        // ...................::110110:..::
        // Reko: a decoder for V850 instruction D31E at address 001009E2 has not been implemented.
        [Test]
        public void V850Dis_D31E()
        {
            AssertCode("@@@", "D31E");
        }

        // ................:....110111.::::
        // Reko: a decoder for V850 instruction EF86 at address 001009E4 has not been implemented.
        [Test]
        public void V850Dis_EF86()
        {
            AssertCode("@@@", "EF86");
        }

        // .................:::.110011::.:.
        // Reko: a decoder for V850 instruction 7A76 at address 001009E6 has not been implemented.
        [Test]
        public void V850Dis_7A76()
        {
            AssertCode("@@@", "7A76");
        }

        // ................:..:.010011.....
        // Reko: a decoder for V850 instruction 6092 at address 001009E8 has not been implemented.
        [Test]
        public void V850Dis_6092()
        {
            AssertCode("@@@", "6092");
        }

        // ................::.:.100101.:.::
        // ................::.:.:..:.:.:.:: sst_h
        // .................:...010010.:.::
        // Reko: a decoder for V850 instruction 4B42 at address 001009EC has not been implemented.
        [Test]
        public void V850Dis_4B42()
        {
            AssertCode("@@@", "4B42");
        }

        // ................:....010010::.::
        // Reko: a decoder for V850 instruction 5B82 at address 001009EE has not been implemented.
        [Test]
        public void V850Dis_5B82()
        {
            AssertCode("@@@", "5B82");
        }

        // ..................:.:000111:....
        // Reko: a decoder for V850 instruction F028 at address 001009F0 has not been implemented.
        [Test]
        public void V850Dis_F028()
        {
            AssertCode("@@@", "F028");
        }

        // ...................:.000100.:.::
        // Reko: a decoder for V850 instruction 8B10 at address 001009F2 has not been implemented.
        [Test]
        public void V850Dis_8B10()
        {
            AssertCode("@@@", "8B10");
        }

        // .................::.:000010.::::
        // ................01101....:..::::
        // .................::.:....:.01111
        // .................::.:....:..:::: divh
        // ..................:.:111011:.:..
        // Reko: a decoder for V850 instruction 742F at address 001009F6 has not been implemented.
        [Test]
        public void V850Dis_742F()
        {
            AssertCode("@@@", "742F");
        }

        // .................:..:010101.....
        // Reko: a decoder for V850 instruction A04A at address 001009F8 has not been implemented.
        [Test]
        public void V850Dis_A04A()
        {
            AssertCode("@@@", "A04A");
        }

        // ................:..::111001::..:
        // Reko: a decoder for V850 instruction 399F at address 001009FA has not been implemented.
        [Test]
        public void V850Dis_399F()
        {
            AssertCode("@@@", "399F");
        }

        // .................::.:100110::.::
        // .................::.::..::.::.:: sst_h
        // ................:.:.:110110.....
        // Reko: a decoder for V850 instruction C0AE at address 001009FE has not been implemented.
        [Test]
        public void V850Dis_C0AE()
        {
            AssertCode("@@@", "C0AE");
        }

        // ..................:::101001:::::
        // Reko: a decoder for V850 instruction 3F3D at address 00100A00 has not been implemented.
        [Test]
        public void V850Dis_3F3D()
        {
            AssertCode("@@@", "3F3D");
        }

        // .................::..110100::...
        // Reko: a decoder for V850 instruction 9866 at address 00100A02 has not been implemented.
        [Test]
        public void V850Dis_9866()
        {
            AssertCode("@@@", "9866");
        }

        // ................::::.110111:.:::
        // Reko: a decoder for V850 instruction F7F6 at address 00100A04 has not been implemented.
        [Test]
        public void V850Dis_F7F6()
        {
            AssertCode("@@@", "F7F6");
        }

        // ................:.:..100111:::::
        // ................:.:..:..:::::::: sst_h
        // ................:...:110101:.:.:
        // Reko: a decoder for V850 instruction B58E at address 00100A08 has not been implemented.
        [Test]
        public void V850Dis_B58E()
        {
            AssertCode("@@@", "B58E");
        }

        // ................:.:::000001:.::.
        // ................:.:::.....::.::. not
        // ...................::001111:...:
        // Reko: a decoder for V850 instruction F119 at address 00100A0C has not been implemented.
        [Test]
        public void V850Dis_F119()
        {
            AssertCode("@@@", "F119");
        }

        // .................:.:.100101:.:..
        // .................:.:.:..:.::.:.. sst_h
        // ................:::..111100..:.:
        // Reko: a decoder for V850 instruction 85E7 at address 00100A10 has not been implemented.
        [Test]
        public void V850Dis_85E7()
        {
            AssertCode("@@@", "85E7");
        }

        // ................:::::111111.....
        // Reko: a decoder for V850 instruction E0FF at address 00100A12 has not been implemented.
        [Test]
        public void V850Dis_E0FF()
        {
            AssertCode("@@@", "E0FF");
        }

        // ................::...010001::..:
        // Reko: a decoder for V850 instruction 39C2 at address 00100A14 has not been implemented.
        [Test]
        public void V850Dis_39C2()
        {
            AssertCode("@@@", "39C2");
        }

        // .....................010000::.:.
        // ......................:....11010
        // ......................:....::.:. mov
        // ................:.:..011000.:.::
        // ................:.:...::....:.:: sld_b
        // ................:.:..100011:::.:
        // ................:.:..:...:::::.: sld_h
        // ................::.::010010.::.:
        // Reko: a decoder for V850 instruction 4DDA at address 00100A1C has not been implemented.
        [Test]
        public void V850Dis_4DDA()
        {
            AssertCode("@@@", "4DDA");
        }

        // ................::::.011100::.:.
        // ................::::..:::..::.:. sst_b
        // ................:::::100110:...:
        // ................::::::..::.:...: sst_h
        // ....................:110100:.:..
        // Reko: a decoder for V850 instruction 940E at address 00100A22 has not been implemented.
        [Test]
        public void V850Dis_940E()
        {
            AssertCode("@@@", "940E");
        }

        // ................::::.100101.:..:
        // ................::::.:..:.:.:..: sst_h
        // ................:::::010100.:.::
        // ................:::..111001.:..:
        // Reko: a decoder for V850 instruction 29E7 at address 00100A28 has not been implemented.
        [Test]
        public void V850Dis_29E7()
        {
            AssertCode("@@@", "29E7");
        }

        // ................::...000010::.:.
        // ................11000....:.::.:.
        // ................::.......:.11010
        // ................::.......:.::.:. divh
        // ................:.:::011001:..:.
        // ................:.:::.::..::..:. sld_b
        // ................:....111001::::.
        // Reko: a decoder for V850 instruction 3E87 at address 00100A2E has not been implemented.
        [Test]
        public void V850Dis_3E87()
        {
            AssertCode("@@@", "3E87");
        }

        // ................:::..101100:.::.
        // Reko: a decoder for V850 instruction 96E5 at address 00100A30 has not been implemented.
        [Test]
        public void V850Dis_96E5()
        {
            AssertCode("@@@", "96E5");
        }

        // ................::::.011000::..:
        // ................::::..::...::..: sld_b
        // ................::...001100.::..
        // Reko: a decoder for V850 instruction 8CC1 at address 00100A34 has not been implemented.
        [Test]
        public void V850Dis_8CC1()
        {
            AssertCode("@@@", "8CC1");
        }

        // .................::::101011:.:..
        // Reko: a decoder for V850 instruction 747D at address 00100A36 has not been implemented.
        [Test]
        public void V850Dis_747D()
        {
            AssertCode("@@@", "747D");
        }

        // ................:..:.110001...::
        // Reko: a decoder for V850 instruction 2396 at address 00100A38 has not been implemented.
        [Test]
        public void V850Dis_2396()
        {
            AssertCode("@@@", "2396");
        }

        // .................:.:.011010::.::
        // .................:.:..::.:.::.:: sld_b
        // .................:.::010011::..:
        // Reko: a decoder for V850 instruction 795A at address 00100A3C has not been implemented.
        [Test]
        public void V850Dis_795A()
        {
            AssertCode("@@@", "795A");
        }

        // ................::.::010010:.:::
        // Reko: a decoder for V850 instruction 57DA at address 00100A3E has not been implemented.
        [Test]
        public void V850Dis_57DA()
        {
            AssertCode("@@@", "57DA");
        }

        // .................:...010111:..::
        // Reko: a decoder for V850 instruction F342 at address 00100A40 has not been implemented.
        [Test]
        public void V850Dis_F342()
        {
            AssertCode("@@@", "F342");
        }

        // ................::::.100111..:::
        // ................::::.:..:::..::: sst_h
        // ................:.:..110100..:.:
        // Reko: a decoder for V850 instruction 85A6 at address 00100A44 has not been implemented.
        [Test]
        public void V850Dis_85A6()
        {
            AssertCode("@@@", "85A6");
        }

        // ................:..:.000101.:.:.
        // Reko: a decoder for V850 instruction AA90 at address 00100A46 has not been implemented.
        [Test]
        public void V850Dis_AA90()
        {
            AssertCode("@@@", "AA90");
        }

        // ................:::..110010:::..
        // Reko: a decoder for V850 instruction 5CE6 at address 00100A48 has not been implemented.
        [Test]
        public void V850Dis_5CE6()
        {
            AssertCode("@@@", "5CE6");
        }

        // .................:.:.010110:.::.
        // Reko: a decoder for V850 instruction D652 at address 00100A4A has not been implemented.
        [Test]
        public void V850Dis_D652()
        {
            AssertCode("@@@", "D652");
        }

        // .................:.::001011..:::
        // Reko: a decoder for V850 instruction 6759 at address 00100A4C has not been implemented.
        [Test]
        public void V850Dis_6759()
        {
            AssertCode("@@@", "6759");
        }

        // .................:..:111110:....
        // Reko: a decoder for V850 instruction D04F at address 00100A4E has not been implemented.
        [Test]
        public void V850Dis_D04F()
        {
            AssertCode("@@@", "D04F");
        }

        // ................::::.110011..:..
        // Reko: a decoder for V850 instruction 64F6 at address 00100A50 has not been implemented.
        [Test]
        public void V850Dis_64F6()
        {
            AssertCode("@@@", "64F6");
        }

        // .................::..111000.:::.
        // Reko: a decoder for V850 instruction 0E67 at address 00100A52 has not been implemented.
        [Test]
        public void V850Dis_0E67()
        {
            AssertCode("@@@", "0E67");
        }

        // ................:.:..111110.....
        // Reko: a decoder for V850 instruction C0A7 at address 00100A54 has not been implemented.
        [Test]
        public void V850Dis_C0A7()
        {
            AssertCode("@@@", "C0A7");
        }

        // ....................:111011..::.
        // Reko: a decoder for V850 instruction 660F at address 00100A56 has not been implemented.
        [Test]
        public void V850Dis_660F()
        {
            AssertCode("@@@", "660F");
        }

        // ...................:.110100..:::
        // Reko: a decoder for V850 instruction 8716 at address 00100A58 has not been implemented.
        [Test]
        public void V850Dis_8716()
        {
            AssertCode("@@@", "8716");
        }

        // ...................:.011111.:.:.
        // ...................:..:::::.:.:. sst_b
        // ..................:::000100::..:
        // Reko: a decoder for V850 instruction 9938 at address 00100A5C has not been implemented.
        [Test]
        public void V850Dis_9938()
        {
            AssertCode("@@@", "9938");
        }

        // ................::..:110100.....
        // Reko: a decoder for V850 instruction 80CE at address 00100A5E has not been implemented.
        [Test]
        public void V850Dis_80CE()
        {
            AssertCode("@@@", "80CE");
        }

        // .................::::111100.::::
        // Reko: a decoder for V850 instruction 8F7F at address 00100A60 has not been implemented.
        [Test]
        public void V850Dis_8F7F()
        {
            AssertCode("@@@", "8F7F");
        }

        // ................:::.:111110.::..
        // Reko: a decoder for V850 instruction CCEF at address 00100A62 has not been implemented.
        [Test]
        public void V850Dis_CCEF()
        {
            AssertCode("@@@", "CCEF");
        }

        // .................:..:001110:.:..
        // Reko: a decoder for V850 instruction D449 at address 00100A64 has not been implemented.
        [Test]
        public void V850Dis_D449()
        {
            AssertCode("@@@", "D449");
        }

        // ................:::.:111000....:
        // Reko: a decoder for V850 instruction 01EF at address 00100A66 has not been implemented.
        [Test]
        public void V850Dis_01EF()
        {
            AssertCode("@@@", "01EF");
        }

        // ................:...:001001::.::
        // Reko: a decoder for V850 instruction 3B89 at address 00100A68 has not been implemented.
        [Test]
        public void V850Dis_3B89()
        {
            AssertCode("@@@", "3B89");
        }

        // ................:.:..101100:.:::
        // Reko: a decoder for V850 instruction 97A5 at address 00100A6A has not been implemented.
        [Test]
        public void V850Dis_97A5()
        {
            AssertCode("@@@", "97A5");
        }

        // ..................:..010111.:.::
        // Reko: a decoder for V850 instruction EB22 at address 00100A6C has not been implemented.
        [Test]
        public void V850Dis_EB22()
        {
            AssertCode("@@@", "EB22");
        }

        // ................::..:001000:.:.:
        // ................::..:..:...:.:.: or
        // ....................:011110..::.
        // ....................:.::::...::. sst_b
        // ................:....010111.:::.
        // Reko: a decoder for V850 instruction EE82 at address 00100A72 has not been implemented.
        [Test]
        public void V850Dis_EE82()
        {
            AssertCode("@@@", "EE82");
        }

        // ................::::.000010.:.::
        // ................11110....:..:.::
        // ................::::.....:.01011
        // ................::::.....:..:.:: divh
        // .................:..:100000::.:.
        // .................:..::.....::.:. sld_h
        // ................::.:.111110::.::
        // Reko: a decoder for V850 instruction DBD7 at address 00100A78 has not been implemented.
        [Test]
        public void V850Dis_DBD7()
        {
            AssertCode("@@@", "DBD7");
        }

        // .................::.:110011.::.:
        // Reko: a decoder for V850 instruction 6D6E at address 00100A7A has not been implemented.
        [Test]
        public void V850Dis_6D6E()
        {
            AssertCode("@@@", "6D6E");
        }

        // ................:..:.111101::.::
        // Reko: a decoder for V850 instruction BB97 at address 00100A7C has not been implemented.
        [Test]
        public void V850Dis_BB97()
        {
            AssertCode("@@@", "BB97");
        }

        // ..................:..100111:.:::
        // ..................:..:..::::.::: sst_h
        // .....................111011..:.:
        // Reko: a decoder for V850 instruction 6507 at address 00100A80 has not been implemented.
        [Test]
        public void V850Dis_6507()
        {
            AssertCode("@@@", "6507");
        }

        // ................:::..111110...::
        // Reko: a decoder for V850 instruction C3E7 at address 00100A82 has not been implemented.
        [Test]
        public void V850Dis_C3E7()
        {
            AssertCode("@@@", "C3E7");
        }

        // ................:::..001100.::.:
        // Reko: a decoder for V850 instruction 8DE1 at address 00100A84 has not been implemented.
        [Test]
        public void V850Dis_8DE1()
        {
            AssertCode("@@@", "8DE1");
        }

        // ...................::010011::.::
        // Reko: a decoder for V850 instruction 7B1A at address 00100A86 has not been implemented.
        [Test]
        public void V850Dis_7B1A()
        {
            AssertCode("@@@", "7B1A");
        }

        // ...................::111111:.:..
        // Reko: a decoder for V850 instruction F41F at address 00100A88 has not been implemented.
        [Test]
        public void V850Dis_F41F()
        {
            AssertCode("@@@", "F41F");
        }

        // ..................::.111001.::..
        // Reko: a decoder for V850 instruction 2C37 at address 00100A8A has not been implemented.
        [Test]
        public void V850Dis_2C37()
        {
            AssertCode("@@@", "2C37");
        }

        // ................::...001000.::::
        // ................::.....:....:::: or
        // .................::..101001..:..
        // Reko: a decoder for V850 instruction 2465 at address 00100A8E has not been implemented.
        [Test]
        public void V850Dis_2465()
        {
            AssertCode("@@@", "2465");
        }

        // ..................:..001000....:
        // ..................:....:.......: or
        // ................:...:110010.::.:
        // Reko: a decoder for V850 instruction 4D8E at address 00100A92 has not been implemented.
        [Test]
        public void V850Dis_4D8E()
        {
            AssertCode("@@@", "4D8E");
        }

        // ................:::::110010.:..:
        // Reko: a decoder for V850 instruction 49FE at address 00100A94 has not been implemented.
        [Test]
        public void V850Dis_49FE()
        {
            AssertCode("@@@", "49FE");
        }

        // ................:..::111100...::
        // Reko: a decoder for V850 instruction 839F at address 00100A96 has not been implemented.
        [Test]
        public void V850Dis_839F()
        {
            AssertCode("@@@", "839F");
        }

        // .................::::111011..:.:
        // Reko: a decoder for V850 instruction 657F at address 00100A98 has not been implemented.
        [Test]
        public void V850Dis_657F()
        {
            AssertCode("@@@", "657F");
        }

        // ................:::::101000:..::
        // ................::::::.:...:..:1 Sld.w/Sst.w
        // ................::::::.:...:..:: sst_w
        // ................:..:.011110:::..
        // ................:..:..::::.:::.. sst_b
        // ................:.:..011001:.:..
        // ................:.:...::..::.:.. sld_b
        // .................:::.011100:...:
        // .................:::..:::..:...: sst_b
        // ................:::..011010.....
        // ................:::...::.:...... sld_b
        // ..................:.:101010::::.
        // Reko: a decoder for V850 instruction 5E2D at address 00100AA4 has not been implemented.
        [Test]
        public void V850Dis_5E2D()
        {
            AssertCode("@@@", "5E2D");
        }

        // ................:::..100001:::..
        // ................:::..:....::::.. sld_h
        // ................::.:.100100:....
        // ................::.:.:..:..:.... sst_h
        // ..................:.:100100.:::.
        // ..................:.::..:...:::. sst_h
        // ................:::..010110...::
        // Reko: a decoder for V850 instruction C3E2 at address 00100AAC has not been implemented.
        [Test]
        public void V850Dis_C3E2()
        {
            AssertCode("@@@", "C3E2");
        }

        // ...................::010001:::..
        // Reko: a decoder for V850 instruction 3C1A at address 00100AAE has not been implemented.
        [Test]
        public void V850Dis_3C1A()
        {
            AssertCode("@@@", "3C1A");
        }

        // ....................:000101:..:.
        // Reko: a decoder for V850 instruction B208 at address 00100AB0 has not been implemented.
        [Test]
        public void V850Dis_B208()
        {
            AssertCode("@@@", "B208");
        }

        // ................:..::010011.::.:
        // Reko: a decoder for V850 instruction 6D9A at address 00100AB2 has not been implemented.
        [Test]
        public void V850Dis_6D9A()
        {
            AssertCode("@@@", "6D9A");
        }

        // .................::.:000010.:.:.
        // ................01101....:..:.:.
        // .................::.:....:.01010
        // .................::.:....:..:.:. divh
        // .................::..111110::.:.
        // Reko: a decoder for V850 instruction DA67 at address 00100AB6 has not been implemented.
        [Test]
        public void V850Dis_DA67()
        {
            AssertCode("@@@", "DA67");
        }

        // ..................:..100100..:..
        // ..................:..:..:....:.. sst_h
        // ....................:101100...:.
        // Reko: a decoder for V850 instruction 820D at address 00100ABA has not been implemented.
        [Test]
        public void V850Dis_820D()
        {
            AssertCode("@@@", "820D");
        }

        // .................::::101110.::.:
        // Reko: a decoder for V850 instruction CD7D at address 00100ABC has not been implemented.
        [Test]
        public void V850Dis_CD7D()
        {
            AssertCode("@@@", "CD7D");
        }

        // .................::..001001::::.
        // Reko: a decoder for V850 instruction 3E61 at address 00100ABE has not been implemented.
        [Test]
        public void V850Dis_3E61()
        {
            AssertCode("@@@", "3E61");
        }

        // ................:...:000111.:.::
        // Reko: a decoder for V850 instruction EB88 at address 00100AC0 has not been implemented.
        [Test]
        public void V850Dis_EB88()
        {
            AssertCode("@@@", "EB88");
        }

        // ................:..::100101..:.:
        // ................:..:::..:.:..:.: sst_h
        // .................:...000100.....
        // Reko: a decoder for V850 instruction 8040 at address 00100AC4 has not been implemented.
        [Test]
        public void V850Dis_8040()
        {
            AssertCode("@@@", "8040");
        }

        // ................::.:.011000..::.
        // ................::.:..::.....::. sld_b
        // .................::.:000111..:.:
        // Reko: a decoder for V850 instruction E568 at address 00100AC8 has not been implemented.
        [Test]
        public void V850Dis_E568()
        {
            AssertCode("@@@", "E568");
        }

        // .................:::.101111::::.
        // Reko: a decoder for V850 instruction FE75 at address 00100ACA has not been implemented.
        [Test]
        public void V850Dis_FE75()
        {
            AssertCode("@@@", "FE75");
        }

        // .................:.:.111011.::::
        // Reko: a decoder for V850 instruction 6F57 at address 00100ACC has not been implemented.
        [Test]
        public void V850Dis_6F57()
        {
            AssertCode("@@@", "6F57");
        }

        // ................:....011110::.::
        // ................:.....::::.::.:: sst_b
        // ................:::::000010...::
        // ................11111....:....::
        // ................:::::....:.00011
        // ................:::::....:....:: divh
        // ................::.:.100010.:.:.
        // ................::.:.:...:..:.:. sld_h
        // ................:..::001001..:.:
        // Reko: a decoder for V850 instruction 2599 at address 00100AD4 has not been implemented.
        [Test]
        public void V850Dis_2599()
        {
            AssertCode("@@@", "2599");
        }

        // ..................:.:000010::...
        // ................00101....:.::...
        // ..................:.:....:.11000
        // ..................:.:....:.::... divh
        // ................::.::000100::...
        // Reko: a decoder for V850 instruction 98D8 at address 00100AD8 has not been implemented.
        [Test]
        public void V850Dis_98D8()
        {
            AssertCode("@@@", "98D8");
        }

        // ..................:::010010:::::
        // Reko: a decoder for V850 instruction 5F3A at address 00100ADA has not been implemented.
        [Test]
        public void V850Dis_5F3A()
        {
            AssertCode("@@@", "5F3A");
        }

        // ..................:.:110101.:.::
        // Reko: a decoder for V850 instruction AB2E at address 00100ADC has not been implemented.
        [Test]
        public void V850Dis_AB2E()
        {
            AssertCode("@@@", "AB2E");
        }

        // ...................:.010000:.::.
        // ...................:..:....10110
        // ...................:..:....:.::. mov
        // ..................:.:101100:.::.
        // Reko: a decoder for V850 instruction 962D at address 00100AE0 has not been implemented.
        [Test]
        public void V850Dis_962D()
        {
            AssertCode("@@@", "962D");
        }

        // ..................:::010010.....
        // Reko: a decoder for V850 instruction 403A at address 00100AE2 has not been implemented.
        [Test]
        public void V850Dis_403A()
        {
            AssertCode("@@@", "403A");
        }

        // ................:.:..000001:::..
        // ................:.:.......::::.. not
        // .................:::.001100::.:.
        // Reko: a decoder for V850 instruction 9A71 at address 00100AE6 has not been implemented.
        [Test]
        public void V850Dis_9A71()
        {
            AssertCode("@@@", "9A71");
        }

        // ................:..::001000....:
        // ................:..::..:.......: or
        // ................:::..010101:.:.:
        // Reko: a decoder for V850 instruction B5E2 at address 00100AEA has not been implemented.
        [Test]
        public void V850Dis_B5E2()
        {
            AssertCode("@@@", "B5E2");
        }

        // .................::..101011.:.:.
        // Reko: a decoder for V850 instruction 6A65 at address 00100AEC has not been implemented.
        [Test]
        public void V850Dis_6A65()
        {
            AssertCode("@@@", "6A65");
        }

        // ..................:.:101111::...
        // Reko: a decoder for V850 instruction F82D at address 00100AEE has not been implemented.
        [Test]
        public void V850Dis_F82D()
        {
            AssertCode("@@@", "F82D");
        }

        // ................:::::110010.:...
        // Reko: a decoder for V850 instruction 48FE at address 00100AF0 has not been implemented.
        [Test]
        public void V850Dis_48FE()
        {
            AssertCode("@@@", "48FE");
        }

        // ................::.:.101110.:.:.
        // Reko: a decoder for V850 instruction CAD5 at address 00100AF2 has not been implemented.
        [Test]
        public void V850Dis_CAD5()
        {
            AssertCode("@@@", "CAD5");
        }

        // .................::.:001001:..::
        // Reko: a decoder for V850 instruction 3369 at address 00100AF4 has not been implemented.
        [Test]
        public void V850Dis_3369()
        {
            AssertCode("@@@", "3369");
        }

        // .................:.:.001001::.::
        // Reko: a decoder for V850 instruction 3B51 at address 00100AF6 has not been implemented.
        [Test]
        public void V850Dis_3B51()
        {
            AssertCode("@@@", "3B51");
        }

        // .................::::101001:....
        // Reko: a decoder for V850 instruction 307D at address 00100AF8 has not been implemented.
        [Test]
        public void V850Dis_307D()
        {
            AssertCode("@@@", "307D");
        }

        // ................:.:..111000...::
        // Reko: a decoder for V850 instruction 03A7 at address 00100AFA has not been implemented.
        [Test]
        public void V850Dis_03A7()
        {
            AssertCode("@@@", "03A7");
        }

        // ................::..:011111.....
        // ................::..:.:::::..... sst_b
        // ................:.::.110001.:...
        // Reko: a decoder for V850 instruction 28B6 at address 00100AFE has not been implemented.
        [Test]
        public void V850Dis_28B6()
        {
            AssertCode("@@@", "28B6");
        }

        // ................:::.:100010:...:
        // ................:::.::...:.:...: sld_h
        // .................:...010110::::.
        // Reko: a decoder for V850 instruction DE42 at address 00100B02 has not been implemented.
        [Test]
        public void V850Dis_DE42()
        {
            AssertCode("@@@", "DE42");
        }

        // ................:.:..000000:.::.
        // ................10100......:.::.
        // ................:.:........:.::. mov
        // ................:.:..010010..:::
        // Reko: a decoder for V850 instruction 47A2 at address 00100B06 has not been implemented.
        [Test]
        public void V850Dis_47A2()
        {
            AssertCode("@@@", "47A2");
        }

        // ................::.::000010::::.
        // ................11011....:.::::.
        // ................::.::....:.11110
        // ................::.::....:.::::. divh
        // ................:...:011110:..:.
        // ................:...:.::::.:..:. sst_b
        // .................:::.110111..:..
        // Reko: a decoder for V850 instruction E476 at address 00100B0C has not been implemented.
        [Test]
        public void V850Dis_E476()
        {
            AssertCode("@@@", "E476");
        }

        // ..................::.010101::::.
        // ..................::.010111.....
        // Reko: a decoder for V850 instruction E032 at address 00100B10 has not been implemented.
        [Test]
        public void V850Dis_E032()
        {
            AssertCode("@@@", "E032");
        }

        // ................:.:.:100111:..:.
        // ................:.:.::..::::..:. sst_h
        // .................:...001011...::
        // Reko: a decoder for V850 instruction 6341 at address 00100B14 has not been implemented.
        [Test]
        public void V850Dis_6341()
        {
            AssertCode("@@@", "6341");
        }

        // ..................:.:100110::::.
        // ..................:.::..::.::::. sst_h
        // ................:::.:111111:...:
        // Reko: a decoder for V850 instruction F1EF at address 00100B18 has not been implemented.
        [Test]
        public void V850Dis_F1EF()
        {
            AssertCode("@@@", "F1EF");
        }

        // ................:.:::111000.:.:.
        // Reko: a decoder for V850 instruction 0ABF at address 00100B1A has not been implemented.
        [Test]
        public void V850Dis_0ABF()
        {
            AssertCode("@@@", "0ABF");
        }

        // ................::::.010100:::..
        // Reko: a decoder for V850 instruction 9CF2 at address 00100B1C has not been implemented.
        [Test]
        public void V850Dis_9CF2()
        {
            AssertCode("@@@", "9CF2");
        }

        // ..................::.110100:...:
        // Reko: a decoder for V850 instruction 9136 at address 00100B1E has not been implemented.
        [Test]
        public void V850Dis_9136()
        {
            AssertCode("@@@", "9136");
        }

        // ................:::::110011:...:
        // Reko: a decoder for V850 instruction 71FE at address 00100B20 has not been implemented.
        [Test]
        public void V850Dis_71FE()
        {
            AssertCode("@@@", "71FE");
        }

        // .................::::110100:...:
        // Reko: a decoder for V850 instruction 917E at address 00100B22 has not been implemented.
        [Test]
        public void V850Dis_917E()
        {
            AssertCode("@@@", "917E");
        }

        // ................:.::.101100..::.
        // Reko: a decoder for V850 instruction 86B5 at address 00100B24 has not been implemented.
        [Test]
        public void V850Dis_86B5()
        {
            AssertCode("@@@", "86B5");
        }

        // ................:.:.:000000:.:::
        // ................10101......:.:::
        // ................:.:.:......:.::: mov
        // ..................:.:000101:.:::
        // Reko: a decoder for V850 instruction B728 at address 00100B28 has not been implemented.
        [Test]
        public void V850Dis_B728()
        {
            AssertCode("@@@", "B728");
        }

        // .................:.::010011:.:::
        // Reko: a decoder for V850 instruction 775A at address 00100B2A has not been implemented.
        [Test]
        public void V850Dis_775A()
        {
            AssertCode("@@@", "775A");
        }

        // ................:.:.:111100:.::.
        // Reko: a decoder for V850 instruction 96AF at address 00100B2C has not been implemented.
        [Test]
        public void V850Dis_96AF()
        {
            AssertCode("@@@", "96AF");
        }

        // ................:.:::001011.::..
        // Reko: a decoder for V850 instruction 6CB9 at address 00100B2E has not been implemented.
        [Test]
        public void V850Dis_6CB9()
        {
            AssertCode("@@@", "6CB9");
        }

        // ................::.:.000101::::.
        // Reko: a decoder for V850 instruction BED0 at address 00100B30 has not been implemented.
        [Test]
        public void V850Dis_BED0()
        {
            AssertCode("@@@", "BED0");
        }

        // ................:.:.:110110..::.
        // Reko: a decoder for V850 instruction C6AE at address 00100B32 has not been implemented.
        [Test]
        public void V850Dis_C6AE()
        {
            AssertCode("@@@", "C6AE");
        }

        // .................::::011110::..:
        // .................::::.::::.::..: sst_b
        // ....................:111010:..::
        // Reko: a decoder for V850 instruction 530F at address 00100B36 has not been implemented.
        [Test]
        public void V850Dis_530F()
        {
            AssertCode("@@@", "530F");
        }

        // ................:.::.010000.::.:
        // ................:.::..:....01101
        // ................:.::..:.....::.: mov
        // ..................:..110010:....
        // Reko: a decoder for V850 instruction 5026 at address 00100B3A has not been implemented.
        [Test]
        public void V850Dis_5026()
        {
            AssertCode("@@@", "5026");
        }

        // ................:....111100:.:.:
        // Reko: a decoder for V850 instruction 9587 at address 00100B3C has not been implemented.
        [Test]
        public void V850Dis_9587()
        {
            AssertCode("@@@", "9587");
        }

        // .................::.:011000..:.:
        // .................::.:.::.....:.: sld_b
        // ................:..::100010..:::
        // ................:..:::...:...::: sld_h
        // ................:::..110000::.:.
        // Reko: a decoder for V850 instruction 1AE6 at address 00100B42 has not been implemented.
        [Test]
        public void V850Dis_1AE6()
        {
            AssertCode("@@@", "1AE6");
        }

        // ..................:::111111..:.:
        // Reko: a decoder for V850 instruction E53F at address 00100B44 has not been implemented.
        [Test]
        public void V850Dis_E53F()
        {
            AssertCode("@@@", "E53F");
        }

        // ..................:.:010101.::..
        // Reko: a decoder for V850 instruction AC2A at address 00100B46 has not been implemented.
        [Test]
        public void V850Dis_AC2A()
        {
            AssertCode("@@@", "AC2A");
        }

        // .................:..:010000::.::
        // .................:..:.:....11011
        // .................:..:.:....::.:: mov
        // ..................:::011111:.:..
        // ..................:::.::::::.:.. sst_b
        // ................:....100110.:.:.
        // ................:....:..::..:.:. sst_h
        // .................:...010100:.::.
        // Reko: a decoder for V850 instruction 9642 at address 00100B4E has not been implemented.
        [Test]
        public void V850Dis_9642()
        {
            AssertCode("@@@", "9642");
        }

        // .................:.:.001100...::
        // Reko: a decoder for V850 instruction 8351 at address 00100B50 has not been implemented.
        [Test]
        public void V850Dis_8351()
        {
            AssertCode("@@@", "8351");
        }

        // ................::..:011000.....
        // ................::..:.::........ sld_b
        // ................:::..011001.:.::
        // ................:::...::..:.:.:: sld_b
        // ................::::.110010:::.:
        // Reko: a decoder for V850 instruction 5DF6 at address 00100B56 has not been implemented.
        [Test]
        public void V850Dis_5DF6()
        {
            AssertCode("@@@", "5DF6");
        }

        // ................:....001101.:.::
        // Reko: a decoder for V850 instruction AB81 at address 00100B58 has not been implemented.
        [Test]
        public void V850Dis_AB81()
        {
            AssertCode("@@@", "AB81");
        }

        // ................::.:.100011:...:
        // ................::.:.:...:::...: sld_h
        // ................::::.000011..:.:
        // Reko: a decoder for V850 instruction 65F0 at address 00100B5C has not been implemented.
        [Test]
        public void V850Dis_65F0()
        {
            AssertCode("@@@", "65F0");
        }

        // ..................::.000000:..:.
        // ................00110......:..:.
        // ..................::.......:..:. mov
        // ....................:011001...:.
        // ....................:.::..:...:. sld_b
        // ................:::::001000:..:.
        // ................:::::..:...:..:. or
        // ................:....101100:.:.:
        // Reko: a decoder for V850 instruction 9585 at address 00100B64 has not been implemented.
        [Test]
        public void V850Dis_9585()
        {
            AssertCode("@@@", "9585");
        }

        // .................::::111110:.:..
        // Reko: a decoder for V850 instruction D47F at address 00100B66 has not been implemented.
        [Test]
        public void V850Dis_D47F()
        {
            AssertCode("@@@", "D47F");
        }

        // ................:..::011001..:..
        // ................:..::.::..:..:.. sld_b
        // ................:...:111100:..:.
        // Reko: a decoder for V850 instruction 928F at address 00100B6A has not been implemented.
        [Test]
        public void V850Dis_928F()
        {
            AssertCode("@@@", "928F");
        }

        // ...................:.111100..::.
        // Reko: a decoder for V850 instruction 8617 at address 00100B6C has not been implemented.
        [Test]
        public void V850Dis_8617()
        {
            AssertCode("@@@", "8617");
        }

        // ................:...:000010..:.:
        // ................10001....:...:.:
        // ................:...:....:.00101
        // ................:...:....:...:.: divh
        // ....................:110000:::..
        // Reko: a decoder for V850 instruction 1C0E at address 00100B70 has not been implemented.
        [Test]
        public void V850Dis_1C0E()
        {
            AssertCode("@@@", "1C0E");
        }

        // .................:...100101...::
        // .................:...:..:.:...:: sst_h
        // ..................:::010100..:::
        // Reko: a decoder for V850 instruction 873A at address 00100B74 has not been implemented.
        [Test]
        public void V850Dis_873A()
        {
            AssertCode("@@@", "873A");
        }

        // .................::..100010.....
        // .................::..:...:...... sld_h
        // ..................:..100001:::::
        // ..................:..:....:::::: sld_h
        // .................::..011001::.::
        // .................::...::..:::.:: sld_b
        // ................::.:.111110.:::.
        // Reko: a decoder for V850 instruction CED7 at address 00100B7C has not been implemented.
        [Test]
        public void V850Dis_CED7()
        {
            AssertCode("@@@", "CED7");
        }

        // ..................:.:010110..:.:
        // Reko: a decoder for V850 instruction C52A at address 00100B7E has not been implemented.
        [Test]
        public void V850Dis_C52A()
        {
            AssertCode("@@@", "C52A");
        }

        // ................::...100001:.:::
        // ................::...:....::.::: sld_h
        // ................:.:::110000::..:
        // Reko: a decoder for V850 instruction 19BE at address 00100B82 has not been implemented.
        [Test]
        public void V850Dis_19BE()
        {
            AssertCode("@@@", "19BE");
        }

        // ................:..::100011:...:
        // ................:..:::...:::...: sld_h
        // ................:::::101101.::..
        // Reko: a decoder for V850 instruction ACFD at address 00100B86 has not been implemented.
        [Test]
        public void V850Dis_ACFD()
        {
            AssertCode("@@@", "ACFD");
        }

        // ................:....001011:::..
        // Reko: a decoder for V850 instruction 7C81 at address 00100B88 has not been implemented.
        [Test]
        public void V850Dis_7C81()
        {
            AssertCode("@@@", "7C81");
        }

        // ................:...:111101..:::
        // Reko: a decoder for V850 instruction A78F at address 00100B8A has not been implemented.
        [Test]
        public void V850Dis_A78F()
        {
            AssertCode("@@@", "A78F");
        }

        // ................::..:110010:....
        // Reko: a decoder for V850 instruction 50CE at address 00100B8C has not been implemented.
        [Test]
        public void V850Dis_50CE()
        {
            AssertCode("@@@", "50CE");
        }

        // ................:..:.011011.::::
        // ................:..:..::.::.:::: sld_b
        // ................:::.:000001::.::
        // ................:::.:.....:::.:: not
        // .................:.:.111101::...
        // Reko: a decoder for V850 instruction B857 at address 00100B92 has not been implemented.
        [Test]
        public void V850Dis_B857()
        {
            AssertCode("@@@", "B857");
        }

        // ................::..:101010.::.:
        // Reko: a decoder for V850 instruction 4DCD at address 00100B94 has not been implemented.
        [Test]
        public void V850Dis_4DCD()
        {
            AssertCode("@@@", "4DCD");
        }

        // ................:::..111101.:...
        // Reko: a decoder for V850 instruction A8E7 at address 00100B96 has not been implemented.
        [Test]
        public void V850Dis_A8E7()
        {
            AssertCode("@@@", "A8E7");
        }

        // ................:.:.:110110.:.::
        // Reko: a decoder for V850 instruction CBAE at address 00100B98 has not been implemented.
        [Test]
        public void V850Dis_CBAE()
        {
            AssertCode("@@@", "CBAE");
        }

        // .................:::.001101::.:.
        // Reko: a decoder for V850 instruction BA71 at address 00100B9A has not been implemented.
        [Test]
        public void V850Dis_BA71()
        {
            AssertCode("@@@", "BA71");
        }

        // .................:::.001010.:.:.
        // Reko: a decoder for V850 instruction 4A71 at address 00100B9C has not been implemented.
        [Test]
        public void V850Dis_4A71()
        {
            AssertCode("@@@", "4A71");
        }

        // .................:..:101101:....
        // Reko: a decoder for V850 instruction B04D at address 00100B9E has not been implemented.
        [Test]
        public void V850Dis_B04D()
        {
            AssertCode("@@@", "B04D");
        }

        // .................::..111110:.::.
        // Reko: a decoder for V850 instruction D667 at address 00100BA0 has not been implemented.
        [Test]
        public void V850Dis_D667()
        {
            AssertCode("@@@", "D667");
        }

        // ................::...001010:.:::
        // Reko: a decoder for V850 instruction 57C1 at address 00100BA2 has not been implemented.
        [Test]
        public void V850Dis_57C1()
        {
            AssertCode("@@@", "57C1");
        }

        // ................:....111111:::.:
        // Reko: a decoder for V850 instruction FD87 at address 00100BA4 has not been implemented.
        [Test]
        public void V850Dis_FD87()
        {
            AssertCode("@@@", "FD87");
        }

        // ................:....000000:.:::
        // ................10000......:.:::
        // ................:..........:.::: mov
        // ................:.:..101011....:
        // Reko: a decoder for V850 instruction 61A5 at address 00100BA8 has not been implemented.
        [Test]
        public void V850Dis_61A5()
        {
            AssertCode("@@@", "61A5");
        }

        // .....................101110.:...
        // Reko: a decoder for V850 instruction C805 at address 00100BAA has not been implemented.
        [Test]
        public void V850Dis_C805()
        {
            AssertCode("@@@", "C805");
        }

        // ................:.::.000100:..::
        // Reko: a decoder for V850 instruction 93B0 at address 00100BAC has not been implemented.
        [Test]
        public void V850Dis_93B0()
        {
            AssertCode("@@@", "93B0");
        }

        // .................:::.001000:..::
        // .................:::...:...:..:: or
        // ................:.:..111101:.:..
        // Reko: a decoder for V850 instruction B4A7 at address 00100BB0 has not been implemented.
        [Test]
        public void V850Dis_B4A7()
        {
            AssertCode("@@@", "B4A7");
        }

        // ................:::..101001..:::
        // Reko: a decoder for V850 instruction 27E5 at address 00100BB2 has not been implemented.
        [Test]
        public void V850Dis_27E5()
        {
            AssertCode("@@@", "27E5");
        }

        // .................::.:101000:::::
        // .................::.::.:...::::1 Sld.w/Sst.w
        // .................::.::.:...::::: sst_w
        // .................::.:001110:::.:
        // Reko: a decoder for V850 instruction DD69 at address 00100BB6 has not been implemented.
        [Test]
        public void V850Dis_DD69()
        {
            AssertCode("@@@", "DD69");
        }

        // ..................::.001000::.::
        // ..................::...:...::.:: or
        // ................::...011111:..::
        // ................::....::::::..:: sst_b
        // ..................:::011111...:.
        // ..................:::.:::::...:. sst_b
        // ....................:011101:..:.
        // ....................:.:::.::..:. sst_b
        // .................:.::001110:....
        // Reko: a decoder for V850 instruction D059 at address 00100BC0 has not been implemented.
        [Test]
        public void V850Dis_D059()
        {
            AssertCode("@@@", "D059");
        }

        // ................:::.:001101::...
        // Reko: a decoder for V850 instruction B8E9 at address 00100BC2 has not been implemented.
        [Test]
        public void V850Dis_B8E9()
        {
            AssertCode("@@@", "B8E9");
        }

        // ................:.:.:101011:.::.
        // Reko: a decoder for V850 instruction 76AD at address 00100BC4 has not been implemented.
        [Test]
        public void V850Dis_76AD()
        {
            AssertCode("@@@", "76AD");
        }

        // ................:.::.100100:.:.:
        // ................:.::.:..:..:.:.: sst_h
        // ..................::.111110:...:
        // Reko: a decoder for V850 instruction D137 at address 00100BC8 has not been implemented.
        [Test]
        public void V850Dis_D137()
        {
            AssertCode("@@@", "D137");
        }

        // ................:.:..111111:.::.
        // Reko: a decoder for V850 instruction F6A7 at address 00100BCA has not been implemented.
        [Test]
        public void V850Dis_F6A7()
        {
            AssertCode("@@@", "F6A7");
        }

        // ................:.:..011000..:.:
        // ................:.:...::.....:.: sld_b
        // ...................::010010:.:..
        // Reko: a decoder for V850 instruction 541A at address 00100BCE has not been implemented.
        [Test]
        public void V850Dis_541A()
        {
            AssertCode("@@@", "541A");
        }

        // .................:..:000100::.:.
        // Reko: a decoder for V850 instruction 9A48 at address 00100BD0 has not been implemented.
        [Test]
        public void V850Dis_9A48()
        {
            AssertCode("@@@", "9A48");
        }

        // ................:.:..010101:::..
        // Reko: a decoder for V850 instruction BCA2 at address 00100BD2 has not been implemented.
        [Test]
        public void V850Dis_BCA2()
        {
            AssertCode("@@@", "BCA2");
        }

        // ................::..:010010.:..:
        // Reko: a decoder for V850 instruction 49CA at address 00100BD4 has not been implemented.
        [Test]
        public void V850Dis_49CA()
        {
            AssertCode("@@@", "49CA");
        }

        // .................::.:001001.::.:
        // Reko: a decoder for V850 instruction 2D69 at address 00100BD6 has not been implemented.
        [Test]
        public void V850Dis_2D69()
        {
            AssertCode("@@@", "2D69");
        }

        // ................:::.:101100.:.:.
        // Reko: a decoder for V850 instruction 8AED at address 00100BD8 has not been implemented.
        [Test]
        public void V850Dis_8AED()
        {
            AssertCode("@@@", "8AED");
        }

        // ................::..:100101..:.:
        // ................::..::..:.:..:.: sst_h
        // .................:.:.100100:.:..
        // .................:.:.:..:..:.:.. sst_h
        // ..................:..110100::::.
        // Reko: a decoder for V850 instruction 9E26 at address 00100BDE has not been implemented.
        [Test]
        public void V850Dis_9E26()
        {
            AssertCode("@@@", "9E26");
        }

        // ................:.:::100111:..::
        // ................:.::::..::::..:: sst_h
        // ..................:::001001:.:..
        // Reko: a decoder for V850 instruction 3439 at address 00100BE2 has not been implemented.
        [Test]
        public void V850Dis_3439()
        {
            AssertCode("@@@", "3439");
        }

        // ..................:.:010111:.:::
        // Reko: a decoder for V850 instruction F72A at address 00100BE4 has not been implemented.
        [Test]
        public void V850Dis_F72A()
        {
            AssertCode("@@@", "F72A");
        }

        // .....................110111::.::
        // Reko: a decoder for V850 instruction FB06 at address 00100BE6 has not been implemented.
        [Test]
        public void V850Dis_FB06()
        {
            AssertCode("@@@", "FB06");
        }

        // .................:::.010000:...:
        // .................:::..:....10001
        // .................:::..:....:...: mov
        // ................:::..111000:.:..
        // Reko: a decoder for V850 instruction 14E7 at address 00100BEA has not been implemented.
        [Test]
        public void V850Dis_14E7()
        {
            AssertCode("@@@", "14E7");
        }

        // ..................::.101011:.:::
        // Reko: a decoder for V850 instruction 7735 at address 00100BEC has not been implemented.
        [Test]
        public void V850Dis_7735()
        {
            AssertCode("@@@", "7735");
        }

        // .................::::000011...::
        // Reko: a decoder for V850 instruction 6378 at address 00100BEE has not been implemented.
        [Test]
        public void V850Dis_6378()
        {
            AssertCode("@@@", "6378");
        }

        // ................::.::000110:::.:
        // Reko: a decoder for V850 instruction DDD8 at address 00100BF0 has not been implemented.
        [Test]
        public void V850Dis_DDD8()
        {
            AssertCode("@@@", "DDD8");
        }

        // ................:..::001001.:...
        // Reko: a decoder for V850 instruction 2899 at address 00100BF2 has not been implemented.
        [Test]
        public void V850Dis_2899()
        {
            AssertCode("@@@", "2899");
        }

        // ...................:.000011::::.
        // Reko: a decoder for V850 instruction 7E10 at address 00100BF4 has not been implemented.
        [Test]
        public void V850Dis_7E10()
        {
            AssertCode("@@@", "7E10");
        }

        // .....................101110....:
        // Reko: a decoder for V850 instruction C105 at address 00100BF6 has not been implemented.
        [Test]
        public void V850Dis_C105()
        {
            AssertCode("@@@", "C105");
        }

        // .................:..:111001.::..
        // Reko: a decoder for V850 instruction 2C4F at address 00100BF8 has not been implemented.
        [Test]
        public void V850Dis_2C4F()
        {
            AssertCode("@@@", "2C4F");
        }

        // ................:...:000010::::.
        // ................10001....:.::::.
        // ................:...:....:.11110
        // ................:...:....:.::::. divh
        // ................::..:001000.:...
        // ................::..:..:....:... or
        // .................::::100001.....
        // .................:::::....:..... sld_h
        // ................::..:111100::..:
        // Reko: a decoder for V850 instruction 99CF at address 00100C00 has not been implemented.
        [Test]
        public void V850Dis_99CF()
        {
            AssertCode("@@@", "99CF");
        }

        // .................:..:001101:.:..
        // Reko: a decoder for V850 instruction B449 at address 00100C02 has not been implemented.
        [Test]
        public void V850Dis_B449()
        {
            AssertCode("@@@", "B449");
        }

        // ...................::011011.:::.
        // ...................::.::.::.:::. sld_b
        // .................::.:111001...:.
        // Reko: a decoder for V850 instruction 226F at address 00100C06 has not been implemented.
        [Test]
        public void V850Dis_226F()
        {
            AssertCode("@@@", "226F");
        }

        // ................:.::.110010.::.:
        // Reko: a decoder for V850 instruction 4DB6 at address 00100C08 has not been implemented.
        [Test]
        public void V850Dis_4DB6()
        {
            AssertCode("@@@", "4DB6");
        }

        // ................::.:.110100:...:
        // Reko: a decoder for V850 instruction 91D6 at address 00100C0A has not been implemented.
        [Test]
        public void V850Dis_91D6()
        {
            AssertCode("@@@", "91D6");
        }

        // .................:...101101:.::.
        // Reko: a decoder for V850 instruction B645 at address 00100C0C has not been implemented.
        [Test]
        public void V850Dis_B645()
        {
            AssertCode("@@@", "B645");
        }

        // .................:.:.100111:...:
        // .................:.:.:..::::...: sst_h
        // ..................:.:101100.:.:.
        // Reko: a decoder for V850 instruction 8A2D at address 00100C10 has not been implemented.
        [Test]
        public void V850Dis_8A2D()
        {
            AssertCode("@@@", "8A2D");
        }

        // ................:.:::011111.::::
        // ................:.:::.:::::.:::: sst_b
        // .....................100001.:.::
        // .....................:....:.:.:: sld_h
        // ................:::..010101.:...
        // Reko: a decoder for V850 instruction A8E2 at address 00100C16 has not been implemented.
        [Test]
        public void V850Dis_A8E2()
        {
            AssertCode("@@@", "A8E2");
        }

        // ..................:.:110111::.:.
        // Reko: a decoder for V850 instruction FA2E at address 00100C18 has not been implemented.
        [Test]
        public void V850Dis_FA2E()
        {
            AssertCode("@@@", "FA2E");
        }

        // ................:.:.:000000.::..
        // ................10101.......::..
        // ................:.:.:.......::.. mov
        // .................:::.010110.:.::
        // Reko: a decoder for V850 instruction CB72 at address 00100C1C has not been implemented.
        [Test]
        public void V850Dis_CB72()
        {
            AssertCode("@@@", "CB72");
        }

        // .................:...111111::::.
        // Reko: a decoder for V850 instruction FE47 at address 00100C1E has not been implemented.
        [Test]
        public void V850Dis_FE47()
        {
            AssertCode("@@@", "FE47");
        }

        // ................:..:.111010:..:.
        // Reko: a decoder for V850 instruction 5297 at address 00100C20 has not been implemented.
        [Test]
        public void V850Dis_5297()
        {
            AssertCode("@@@", "5297");
        }

        // ................:.:..111010.::.:
        // Reko: a decoder for V850 instruction 4DA7 at address 00100C22 has not been implemented.
        [Test]
        public void V850Dis_4DA7()
        {
            AssertCode("@@@", "4DA7");
        }

        // ...................::010100....:
        // Reko: a decoder for V850 instruction 811A at address 00100C24 has not been implemented.
        [Test]
        public void V850Dis_811A()
        {
            AssertCode("@@@", "811A");
        }

        // ................::...001000.:..:
        // ................::.....:....:..: or
        // ................:.:::000101:.:.:
        // Reko: a decoder for V850 instruction B5B8 at address 00100C28 has not been implemented.
        [Test]
        public void V850Dis_B5B8()
        {
            AssertCode("@@@", "B5B8");
        }

        // ................::...111010.:.::
        // Reko: a decoder for V850 instruction 4BC7 at address 00100C2A has not been implemented.
        [Test]
        public void V850Dis_4BC7()
        {
            AssertCode("@@@", "4BC7");
        }

        // ................:::.:011011:::::
        // ................:::.:.::.::::::: sld_b
        // ................:::..110010.::::
        // Reko: a decoder for V850 instruction 4FE6 at address 00100C2E has not been implemented.
        [Test]
        public void V850Dis_4FE6()
        {
            AssertCode("@@@", "4FE6");
        }

        // ....................:010010:::.:
        // Reko: a decoder for V850 instruction 5D0A at address 00100C30 has not been implemented.
        [Test]
        public void V850Dis_5D0A()
        {
            AssertCode("@@@", "5D0A");
        }

        // ................::.::010111::::.
        // Reko: a decoder for V850 instruction FEDA at address 00100C32 has not been implemented.
        [Test]
        public void V850Dis_FEDA()
        {
            AssertCode("@@@", "FEDA");
        }

        // .................::..101110::.:.
        // Reko: a decoder for V850 instruction DA65 at address 00100C34 has not been implemented.
        [Test]
        public void V850Dis_DA65()
        {
            AssertCode("@@@", "DA65");
        }

        // ................:..:.101010:....
        // Reko: a decoder for V850 instruction 5095 at address 00100C36 has not been implemented.
        [Test]
        public void V850Dis_5095()
        {
            AssertCode("@@@", "5095");
        }

        // ................::.:.001001..:::
        // Reko: a decoder for V850 instruction 27D1 at address 00100C38 has not been implemented.
        [Test]
        public void V850Dis_27D1()
        {
            AssertCode("@@@", "27D1");
        }

        // ..................:::111010.:::.
        // Reko: a decoder for V850 instruction 4E3F at address 00100C3A has not been implemented.
        [Test]
        public void V850Dis_4E3F()
        {
            AssertCode("@@@", "4E3F");
        }

        // .................::::010000.:::.
        // .................::::.:....01110
        // .................::::.:.....:::. mov
        // ................::.::011101..:..
        // ................::.::.:::.:..:.. sst_b
        // .................:..:001101:.:::
        // Reko: a decoder for V850 instruction B749 at address 00100C40 has not been implemented.
        [Test]
        public void V850Dis_B749()
        {
            AssertCode("@@@", "B749");
        }

        // ...................::110100..:..
        // Reko: a decoder for V850 instruction 841E at address 00100C42 has not been implemented.
        [Test]
        public void V850Dis_841E()
        {
            AssertCode("@@@", "841E");
        }

        // .................:..:010111::...
        // Reko: a decoder for V850 instruction F84A at address 00100C44 has not been implemented.
        [Test]
        public void V850Dis_F84A()
        {
            AssertCode("@@@", "F84A");
        }

        // .................:::.000110:..:.
        // Reko: a decoder for V850 instruction D270 at address 00100C46 has not been implemented.
        [Test]
        public void V850Dis_D270()
        {
            AssertCode("@@@", "D270");
        }

        // ..................:.:101001...::
        // Reko: a decoder for V850 instruction 232D at address 00100C48 has not been implemented.
        [Test]
        public void V850Dis_232D()
        {
            AssertCode("@@@", "232D");
        }

        // .................:::.011000..:.:
        // .................:::..::.....:.: sld_b
        // ................:::..000101::...
        // Reko: a decoder for V850 instruction B8E0 at address 00100C4C has not been implemented.
        [Test]
        public void V850Dis_B8E0()
        {
            AssertCode("@@@", "B8E0");
        }

        // ....................:111011...::
        // Reko: a decoder for V850 instruction 630F at address 00100C4E has not been implemented.
        [Test]
        public void V850Dis_630F()
        {
            AssertCode("@@@", "630F");
        }

        // ..................:..110110:::::
        // Reko: a decoder for V850 instruction DF26 at address 00100C50 has not been implemented.
        [Test]
        public void V850Dis_DF26()
        {
            AssertCode("@@@", "DF26");
        }

        // ................::...001010.:.:.
        // Reko: a decoder for V850 instruction 4AC1 at address 00100C52 has not been implemented.
        [Test]
        public void V850Dis_4AC1()
        {
            AssertCode("@@@", "4AC1");
        }

        // ..................:..011011:.:..
        // ..................:...::.:::.:.. sld_b
        // ................:::.:100011.::..
        // ................:::.::...::.::.. sld_h
        // ................:.:::000010..::.
        // ................10111....:...::.
        // ................:.:::....:.00110
        // ................:.:::....:...::. divh
        // ................:.::.000011:::::
        // Reko: a decoder for V850 instruction 7FB0 at address 00100C5A has not been implemented.
        [Test]
        public void V850Dis_7FB0()
        {
            AssertCode("@@@", "7FB0");
        }

        // ................::..:001110.:.::
        // Reko: a decoder for V850 instruction CBC9 at address 00100C5C has not been implemented.
        [Test]
        public void V850Dis_CBC9()
        {
            AssertCode("@@@", "CBC9");
        }

        // ..................:::011101:..:.
        // ..................:::.:::.::..:. sst_b
        // .................:::.000000:.:..
        // ................01110......:.:..
        // .................:::.......:.:.. mov
        // ..................::.101000:....
        // ..................::.:.:...:...0 Sld.w/Sst.w
        // ..................::.:.:...:.... sld_w
        // ..................::.000011..::.
        // Reko: a decoder for V850 instruction 6630 at address 00100C64 has not been implemented.
        [Test]
        public void V850Dis_6630()
        {
            AssertCode("@@@", "6630");
        }

        // ...................:.011111.:::.
        // ...................:..:::::.:::. sst_b
        // ................:...:101001:.:::
        // Reko: a decoder for V850 instruction 378D at address 00100C68 has not been implemented.
        [Test]
        public void V850Dis_378D()
        {
            AssertCode("@@@", "378D");
        }

        // ................:.:.:101101:.::.
        // Reko: a decoder for V850 instruction B6AD at address 00100C6A has not been implemented.
        [Test]
        public void V850Dis_B6AD()
        {
            AssertCode("@@@", "B6AD");
        }

        // ................::.::101010:::::
        // Reko: a decoder for V850 instruction 5FDD at address 00100C6C has not been implemented.
        [Test]
        public void V850Dis_5FDD()
        {
            AssertCode("@@@", "5FDD");
        }

        // .................:...111110:::..
        // Reko: a decoder for V850 instruction DC47 at address 00100C6E has not been implemented.
        [Test]
        public void V850Dis_DC47()
        {
            AssertCode("@@@", "DC47");
        }

        // ..................:::111011.::.:
        // Reko: a decoder for V850 instruction 6D3F at address 00100C70 has not been implemented.
        [Test]
        public void V850Dis_6D3F()
        {
            AssertCode("@@@", "6D3F");
        }

        // ................:::.:000011.....
        // Reko: a decoder for V850 instruction 60E8 at address 00100C72 has not been implemented.
        [Test]
        public void V850Dis_60E8()
        {
            AssertCode("@@@", "60E8");
        }

        // ................:::..011001.::::
        // ................:::...::..:.:::: sld_b
        // ................:..:.011011..::.
        // ................:..:..::.::..::. sld_b
        // ................:..:.111110:::..
        // Reko: a decoder for V850 instruction DC97 at address 00100C78 has not been implemented.
        [Test]
        public void V850Dis_DC97()
        {
            AssertCode("@@@", "DC97");
        }

        // ..................:::111110:.:::
        // Reko: a decoder for V850 instruction D73F at address 00100C7A has not been implemented.
        [Test]
        public void V850Dis_D73F()
        {
            AssertCode("@@@", "D73F");
        }

        // ..................:..010110.:.::
        // Reko: a decoder for V850 instruction CB22 at address 00100C7C has not been implemented.
        [Test]
        public void V850Dis_CB22()
        {
            AssertCode("@@@", "CB22");
        }

        // .................:.::010101..:.:
        // Reko: a decoder for V850 instruction A55A at address 00100C7E has not been implemented.
        [Test]
        public void V850Dis_A55A()
        {
            AssertCode("@@@", "A55A");
        }

        // ................::..:010011::.:.
        // Reko: a decoder for V850 instruction 7ACA at address 00100C80 has not been implemented.
        [Test]
        public void V850Dis_7ACA()
        {
            AssertCode("@@@", "7ACA");
        }

        // .................:..:000010:::::
        // ................01001....:.:::::
        // .................:..:....:.11111
        // .................:..:....:.::::: divh
        // ..................:.:110110:....
        // Reko: a decoder for V850 instruction D02E at address 00100C84 has not been implemented.
        [Test]
        public void V850Dis_D02E()
        {
            AssertCode("@@@", "D02E");
        }

        // ................:.:.:110101.:...
        // Reko: a decoder for V850 instruction A8AE at address 00100C86 has not been implemented.
        [Test]
        public void V850Dis_A8AE()
        {
            AssertCode("@@@", "A8AE");
        }

        // ..................::.111001:.:::
        // Reko: a decoder for V850 instruction 3737 at address 00100C88 has not been implemented.
        [Test]
        public void V850Dis_3737()
        {
            AssertCode("@@@", "3737");
        }

        // ................:::..110100.:..:
        // Reko: a decoder for V850 instruction 89E6 at address 00100C8A has not been implemented.
        [Test]
        public void V850Dis_89E6()
        {
            AssertCode("@@@", "89E6");
        }

        // ................:.:::111011.::.:
        // Reko: a decoder for V850 instruction 6DBF at address 00100C8C has not been implemented.
        [Test]
        public void V850Dis_6DBF()
        {
            AssertCode("@@@", "6DBF");
        }

        // .................::.:001100.::.:
        // Reko: a decoder for V850 instruction 8D69 at address 00100C8E has not been implemented.
        [Test]
        public void V850Dis_8D69()
        {
            AssertCode("@@@", "8D69");
        }

        // ..................::.111110:::..
        // Reko: a decoder for V850 instruction DC37 at address 00100C90 has not been implemented.
        [Test]
        public void V850Dis_DC37()
        {
            AssertCode("@@@", "DC37");
        }

        // ................:...:011010...:.
        // ................:...:.::.:....:. sld_b
        // ................::..:001011.:..:
        // Reko: a decoder for V850 instruction 69C9 at address 00100C94 has not been implemented.
        [Test]
        public void V850Dis_69C9()
        {
            AssertCode("@@@", "69C9");
        }

        // ..................::.001000.:.::
        // ..................::...:....:.:: or
        // .................::::100000.:..:
        // .................:::::......:..: sld_h
        // ................::..:110010::..:
        // Reko: a decoder for V850 instruction 59CE at address 00100C9A has not been implemented.
        [Test]
        public void V850Dis_59CE()
        {
            AssertCode("@@@", "59CE");
        }

        // ................:.:.:110010::..:
        // Reko: a decoder for V850 instruction 59AE at address 00100C9C has not been implemented.
        [Test]
        public void V850Dis_59AE()
        {
            AssertCode("@@@", "59AE");
        }

        // ...................:.101010.:...
        // Reko: a decoder for V850 instruction 4815 at address 00100C9E has not been implemented.
        [Test]
        public void V850Dis_4815()
        {
            AssertCode("@@@", "4815");
        }

        // .................:.::100111.:...
        // .................:.:::..:::.:... sst_h
        // ................:..:.101001:..:.
        // Reko: a decoder for V850 instruction 3295 at address 00100CA2 has not been implemented.
        [Test]
        public void V850Dis_3295()
        {
            AssertCode("@@@", "3295");
        }

        // ................:..:.100011:::.:
        // ................:..:.:...:::::.: sld_h
        // ..................:::111011:.:::
        // Reko: a decoder for V850 instruction 773F at address 00100CA6 has not been implemented.
        [Test]
        public void V850Dis_773F()
        {
            AssertCode("@@@", "773F");
        }

        // ................:.::.010100:.::.
        // Reko: a decoder for V850 instruction 96B2 at address 00100CA8 has not been implemented.
        [Test]
        public void V850Dis_96B2()
        {
            AssertCode("@@@", "96B2");
        }

        // ................:::::000101:....
        // Reko: a decoder for V850 instruction B0F8 at address 00100CAA has not been implemented.
        [Test]
        public void V850Dis_B0F8()
        {
            AssertCode("@@@", "B0F8");
        }

        // ................:....000010::..:
        // ................10000....:.::..:
        // ................:........:.11001
        // ................:........:.::..: divh
        // ...................::100001..::.
        // ...................:::....:..::. sld_h
        // .................::.:110000.:.:.
        // Reko: a decoder for V850 instruction 0A6E at address 00100CB0 has not been implemented.
        [Test]
        public void V850Dis_0A6E()
        {
            AssertCode("@@@", "0A6E");
        }

        // ................:..:.011011...::
        // ................:..:..::.::...:: sld_b
        // ...................::001111.....
        // Reko: a decoder for V850 instruction E019 at address 00100CB4 has not been implemented.
        [Test]
        public void V850Dis_E019()
        {
            AssertCode("@@@", "E019");
        }

        // ................:.::.001111:....
        // Reko: a decoder for V850 instruction F0B1 at address 00100CB6 has not been implemented.
        [Test]
        public void V850Dis_F0B1()
        {
            AssertCode("@@@", "F0B1");
        }

        // ..................:.:110101::::.
        // Reko: a decoder for V850 instruction BE2E at address 00100CB8 has not been implemented.
        [Test]
        public void V850Dis_BE2E()
        {
            AssertCode("@@@", "BE2E");
        }

        // .................:.:.011001::::.
        // .................:.:..::..:::::. sld_b
        // ................::.::010110:.:..
        // Reko: a decoder for V850 instruction D4DA at address 00100CBC has not been implemented.
        [Test]
        public void V850Dis_D4DA()
        {
            AssertCode("@@@", "D4DA");
        }

        // ................:...:001010:.:.:
        // Reko: a decoder for V850 instruction 5589 at address 00100CBE has not been implemented.
        [Test]
        public void V850Dis_5589()
        {
            AssertCode("@@@", "5589");
        }

        // .................:::.100111.::::
        // .................:::.:..:::.:::: sst_h
        // ...................:.000111:::.:
        // Reko: a decoder for V850 instruction FD10 at address 00100CC2 has not been implemented.
        [Test]
        public void V850Dis_FD10()
        {
            AssertCode("@@@", "FD10");
        }

        // ................:::::000100.::.:
        // Reko: a decoder for V850 instruction 8DF8 at address 00100CC4 has not been implemented.
        [Test]
        public void V850Dis_8DF8()
        {
            AssertCode("@@@", "8DF8");
        }

        // .................:...111111.:.::
        // Reko: a decoder for V850 instruction EB47 at address 00100CC6 has not been implemented.
        [Test]
        public void V850Dis_EB47()
        {
            AssertCode("@@@", "EB47");
        }

        // .....................011101:::::
        // ......................:::.:::::: sst_b
        // ................:.::.111111:.:..
        // Reko: a decoder for V850 instruction F4B7 at address 00100CCA has not been implemented.
        [Test]
        public void V850Dis_F4B7()
        {
            AssertCode("@@@", "F4B7");
        }

        // ...................:.011000::...
        // ...................:..::...::... sld_b
        // ................:...:001110:.:::
        // Reko: a decoder for V850 instruction D789 at address 00100CCE has not been implemented.
        [Test]
        public void V850Dis_D789()
        {
            AssertCode("@@@", "D789");
        }

        // ..................:::100010:.:.:
        // ..................::::...:.:.:.: sld_h
        // ................:::.:010110...::
        // Reko: a decoder for V850 instruction C3EA at address 00100CD2 has not been implemented.
        [Test]
        public void V850Dis_C3EA()
        {
            AssertCode("@@@", "C3EA");
        }

        // ................:.::.100010.:.::
        // ................:.::.:...:..:.:: sld_h
        // ................::..:111001:.:.:
        // Reko: a decoder for V850 instruction 35CF at address 00100CD6 has not been implemented.
        [Test]
        public void V850Dis_35CF()
        {
            AssertCode("@@@", "35CF");
        }

        // ................:....001011.:.::
        // Reko: a decoder for V850 instruction 6B81 at address 00100CD8 has not been implemented.
        [Test]
        public void V850Dis_6B81()
        {
            AssertCode("@@@", "6B81");
        }

        // .................:..:111110:.::.
        // Reko: a decoder for V850 instruction D64F at address 00100CDA has not been implemented.
        [Test]
        public void V850Dis_D64F()
        {
            AssertCode("@@@", "D64F");
        }

        // ..................:..011010:.::.
        // ..................:...::.:.:.::. sld_b
        // ................:.:::100011.:.:.
        // ................:.::::...::.:.:. sld_h
        // ..................:..111010:::..
        // Reko: a decoder for V850 instruction 5C27 at address 00100CE0 has not been implemented.
        [Test]
        public void V850Dis_5C27()
        {
            AssertCode("@@@", "5C27");
        }

        // ................:::..101000.:...
        // ................:::..:.:....:..0 Sld.w/Sst.w
        // ................:::..:.:....:... sld_w
        // ...................:.110100...:.
        // Reko: a decoder for V850 instruction 8216 at address 00100CE4 has not been implemented.
        [Test]
        public void V850Dis_8216()
        {
            AssertCode("@@@", "8216");
        }

        // ...................:.010100.::::
        // Reko: a decoder for V850 instruction 8F12 at address 00100CE6 has not been implemented.
        [Test]
        public void V850Dis_8F12()
        {
            AssertCode("@@@", "8F12");
        }

        // ...................::011100...:.
        // ...................::.:::.....:. sst_b
        // ................::.::100110:..::
        // ................::.:::..::.:..:: sst_h
        // ..................:::001001..:..
        // Reko: a decoder for V850 instruction 2439 at address 00100CEC has not been implemented.
        [Test]
        public void V850Dis_2439()
        {
            AssertCode("@@@", "2439");
        }

        // ................:.:.:010100::...
        // Reko: a decoder for V850 instruction 98AA at address 00100CEE has not been implemented.
        [Test]
        public void V850Dis_98AA()
        {
            AssertCode("@@@", "98AA");
        }

        // ................:.::.100010:.::.
        // ................:.::.:...:.:.::. sld_h
        // ................:..::000111::::.
        // Reko: a decoder for V850 instruction FE98 at address 00100CF2 has not been implemented.
        [Test]
        public void V850Dis_FE98()
        {
            AssertCode("@@@", "FE98");
        }

        // .................::.:010000:.::.
        // .................::.:.:....10110
        // .................::.:.:....:.::. mov
        // ..................:::010001::::.
        // Reko: a decoder for V850 instruction 3E3A at address 00100CF6 has not been implemented.
        [Test]
        public void V850Dis_3E3A()
        {
            AssertCode("@@@", "3E3A");
        }

        // ..................:.:110001::::.
        // Reko: a decoder for V850 instruction 3E2E at address 00100CF8 has not been implemented.
        [Test]
        public void V850Dis_3E2E()
        {
            AssertCode("@@@", "3E2E");
        }

        // ....................:011101..:..
        // ....................:.:::.:..:.. sst_b
        // ................:..::011001....:
        // ................:..::.::..:....: sld_b
        // ................:..::100111:...:
        // ................:..:::..::::...: sst_h
        // ..................:::000100::...
        // Reko: a decoder for V850 instruction 9838 at address 00100D00 has not been implemented.
        [Test]
        public void V850Dis_9838()
        {
            AssertCode("@@@", "9838");
        }

        // ................::::.000010::.:.
        // ................11110....:.::.:.
        // ................::::.....:.11010
        // ................::::.....:.::.:. divh
        // ................::::.001000.:.:.
        // ................::::...:....:.:. or
        // ..................:::110000..:::
        // Reko: a decoder for V850 instruction 073E at address 00100D06 has not been implemented.
        [Test]
        public void V850Dis_073E()
        {
            AssertCode("@@@", "073E");
        }

        // ................:.::.001101.::..
        // Reko: a decoder for V850 instruction ACB1 at address 00100D08 has not been implemented.
        [Test]
        public void V850Dis_ACB1()
        {
            AssertCode("@@@", "ACB1");
        }

        // ..................:.:111011.:...
        // Reko: a decoder for V850 instruction 682F at address 00100D0A has not been implemented.
        [Test]
        public void V850Dis_682F()
        {
            AssertCode("@@@", "682F");
        }

        // .................:.::000111....:
        // Reko: a decoder for V850 instruction E158 at address 00100D0C has not been implemented.
        [Test]
        public void V850Dis_E158()
        {
            AssertCode("@@@", "E158");
        }

        // ..................::.010101.:.:.
        // Reko: a decoder for V850 instruction AA32 at address 00100D0E has not been implemented.
        [Test]
        public void V850Dis_AA32()
        {
            AssertCode("@@@", "AA32");
        }

        // ..................:.:011111.:...
        // ..................:.:.:::::.:... sst_b
        // ................:.:..001111::...
        // Reko: a decoder for V850 instruction F8A1 at address 00100D12 has not been implemented.
        [Test]
        public void V850Dis_F8A1()
        {
            AssertCode("@@@", "F8A1");
        }

        // ................::.:.010001..:.:
        // Reko: a decoder for V850 instruction 25D2 at address 00100D14 has not been implemented.
        [Test]
        public void V850Dis_25D2()
        {
            AssertCode("@@@", "25D2");
        }

        // ................::::.010101..::.
        // Reko: a decoder for V850 instruction A6F2 at address 00100D16 has not been implemented.
        [Test]
        public void V850Dis_A6F2()
        {
            AssertCode("@@@", "A6F2");
        }

        // ................::..:110111.:..:
        // Reko: a decoder for V850 instruction E9CE at address 00100D18 has not been implemented.
        [Test]
        public void V850Dis_E9CE()
        {
            AssertCode("@@@", "E9CE");
        }

        // .................:.:.011101..::.
        // .................:.:..:::.:..::. sst_b
        // ................:....100011:..:.
        // ................:....:...:::..:. sld_h
        // ................:::..110111:....
        // Reko: a decoder for V850 instruction F0E6 at address 00100D1E has not been implemented.
        [Test]
        public void V850Dis_F0E6()
        {
            AssertCode("@@@", "F0E6");
        }

        // ..................::.111001.:..:
        // Reko: a decoder for V850 instruction 2937 at address 00100D20 has not been implemented.
        [Test]
        public void V850Dis_2937()
        {
            AssertCode("@@@", "2937");
        }

        // ................:::.:100010..:..
        // ................:::.::...:...:.. sld_h
        // ................:.:..111110:::::
        // .................::..101101.....
        // Reko: a decoder for V850 instruction A065 at address 00100D26 has not been implemented.
        [Test]
        public void V850Dis_A065()
        {
            AssertCode("@@@", "A065");
        }

        // ................:.:::111101.::.:
        // Reko: a decoder for V850 instruction ADBF at address 00100D28 has not been implemented.
        [Test]
        public void V850Dis_ADBF()
        {
            AssertCode("@@@", "ADBF");
        }

        // .................::..100111.:.::
        // .................::..:..:::.:.:: sst_h
        // ....................:110000...:.
        // Reko: a decoder for V850 instruction 020E at address 00100D2C has not been implemented.
        [Test]
        public void V850Dis_020E()
        {
            AssertCode("@@@", "020E");
        }

        // ................:::..110010::..:
        // Reko: a decoder for V850 instruction 59E6 at address 00100D2E has not been implemented.
        [Test]
        public void V850Dis_59E6()
        {
            AssertCode("@@@", "59E6");
        }

        // .................:::.010100..:..
        // Reko: a decoder for V850 instruction 8472 at address 00100D30 has not been implemented.
        [Test]
        public void V850Dis_8472()
        {
            AssertCode("@@@", "8472");
        }

        // ................::::.101001...::
        // Reko: a decoder for V850 instruction 23F5 at address 00100D32 has not been implemented.
        [Test]
        public void V850Dis_23F5()
        {
            AssertCode("@@@", "23F5");
        }

        // .................:.:.000101...::
        // Reko: a decoder for V850 instruction A350 at address 00100D34 has not been implemented.
        [Test]
        public void V850Dis_A350()
        {
            AssertCode("@@@", "A350");
        }

        // ................::...110011:.::.
        // Reko: a decoder for V850 instruction 76C6 at address 00100D36 has not been implemented.
        [Test]
        public void V850Dis_76C6()
        {
            AssertCode("@@@", "76C6");
        }

        // .................::.:000100:..:.
        // Reko: a decoder for V850 instruction 9268 at address 00100D38 has not been implemented.
        [Test]
        public void V850Dis_9268()
        {
            AssertCode("@@@", "9268");
        }

        // ..................:::100000..:::
        // ..................::::.......::: sld_h
        // ................:.::.011010.:.:.
        // ................:.::..::.:..:.:. sld_b
        // ..................:.:000110.::..
        // Reko: a decoder for V850 instruction CC28 at address 00100D3E has not been implemented.
        [Test]
        public void V850Dis_CC28()
        {
            AssertCode("@@@", "CC28");
        }

        // ................:....001110.....
        // Reko: a decoder for V850 instruction C081 at address 00100D40 has not been implemented.
        [Test]
        public void V850Dis_C081()
        {
            AssertCode("@@@", "C081");
        }

        // ................:.:.:001000..:.:
        // ................:.:.:..:.....:.: or
        // ................:.:::101000:..::
        // ................:.::::.:...:..:1 Sld.w/Sst.w
        // ................:.::::.:...:..:: sst_w
        // .................:...001010:..:.
        // Reko: a decoder for V850 instruction 5241 at address 00100D46 has not been implemented.
        [Test]
        public void V850Dis_5241()
        {
            AssertCode("@@@", "5241");
        }

        // ................:..:.011100.:.::
        // ................:..:..:::...:.:: sst_b
        // .................::..101111:.:..
        // Reko: a decoder for V850 instruction F465 at address 00100D4A has not been implemented.
        [Test]
        public void V850Dis_F465()
        {
            AssertCode("@@@", "F465");
        }

        // .................:...001111..::.
        // Reko: a decoder for V850 instruction E641 at address 00100D4C has not been implemented.
        [Test]
        public void V850Dis_E641()
        {
            AssertCode("@@@", "E641");
        }

        // ................::.::011111.::::
        // ................::.::.:::::.:::: sst_b
        // ................::...000011:.:..
        // Reko: a decoder for V850 instruction 74C0 at address 00100D50 has not been implemented.
        [Test]
        public void V850Dis_74C0()
        {
            AssertCode("@@@", "74C0");
        }

        // .....................000110.::::
        // Reko: a decoder for V850 instruction CF00 at address 00100D52 has not been implemented.
        [Test]
        public void V850Dis_CF00()
        {
            AssertCode("@@@", "CF00");
        }

        // ................:::::011011.....
        // ................:::::.::.::..... sld_b
        // ................::.::001010...::
        // Reko: a decoder for V850 instruction 43D9 at address 00100D56 has not been implemented.
        [Test]
        public void V850Dis_43D9()
        {
            AssertCode("@@@", "43D9");
        }

        // .................::::110110.....
        // Reko: a decoder for V850 instruction C07E at address 00100D58 has not been implemented.
        [Test]
        public void V850Dis_C07E()
        {
            AssertCode("@@@", "C07E");
        }

        // .................:::.111010::.:.
        // Reko: a decoder for V850 instruction 5A77 at address 00100D5A has not been implemented.
        [Test]
        public void V850Dis_5A77()
        {
            AssertCode("@@@", "5A77");
        }

        // .................::..011101.....
        // .................::...:::.:..... sst_b
        // ................:::::001000::.::
        // ................:::::..:...::.:: or
        // ................::...011111:..::
        // ................::....::::::..:: sst_b
        // ................:....001101..:..
        // Reko: a decoder for V850 instruction A481 at address 00100D62 has not been implemented.
        [Test]
        public void V850Dis_A481()
        {
            AssertCode("@@@", "A481");
        }

        // ................:.:.:111000.:..:
        // Reko: a decoder for V850 instruction 09AF at address 00100D64 has not been implemented.
        [Test]
        public void V850Dis_09AF()
        {
            AssertCode("@@@", "09AF");
        }

        // ................::...110110:..:.
        // Reko: a decoder for V850 instruction D2C6 at address 00100D66 has not been implemented.
        [Test]
        public void V850Dis_D2C6()
        {
            AssertCode("@@@", "D2C6");
        }

        // ................:::..001000:::::
        // ................:::....:...::::: or
        // ....................:111011.:.:.
        // Reko: a decoder for V850 instruction 6A0F at address 00100D6A has not been implemented.
        [Test]
        public void V850Dis_6A0F()
        {
            AssertCode("@@@", "6A0F");
        }

        // ...................:.001010::.::
        // Reko: a decoder for V850 instruction 5B11 at address 00100D6C has not been implemented.
        [Test]
        public void V850Dis_5B11()
        {
            AssertCode("@@@", "5B11");
        }

        // .................:.:.000110:::::
        // Reko: a decoder for V850 instruction DF50 at address 00100D6E has not been implemented.
        [Test]
        public void V850Dis_DF50()
        {
            AssertCode("@@@", "DF50");
        }

        // ..................:.:100011::...
        // ..................:.::...::::... sld_h
        // ................:...:010110..:::
        // Reko: a decoder for V850 instruction C78A at address 00100D72 has not been implemented.
        [Test]
        public void V850Dis_C78A()
        {
            AssertCode("@@@", "C78A");
        }

        // ................::..:100111...::
        // ................::..::..:::...:: sst_h
        // ................::...101110..:::
        // Reko: a decoder for V850 instruction C7C5 at address 00100D76 has not been implemented.
        [Test]
        public void V850Dis_C7C5()
        {
            AssertCode("@@@", "C7C5");
        }

        // ..................:.:100001:::.:
        // ..................:.::....::::.: sld_h
        // ................:..:.101111.:..:
        // Reko: a decoder for V850 instruction E995 at address 00100D7A has not been implemented.
        [Test]
        public void V850Dis_E995()
        {
            AssertCode("@@@", "E995");
        }

        // ................::.::011011...::
        // ................::.::.::.::...:: sld_b
        // ..................:..111101....:
        // Reko: a decoder for V850 instruction A127 at address 00100D7E has not been implemented.
        [Test]
        public void V850Dis_A127()
        {
            AssertCode("@@@", "A127");
        }

        // ................:::::010011::...
        // Reko: a decoder for V850 instruction 78FA at address 00100D80 has not been implemented.
        [Test]
        public void V850Dis_78FA()
        {
            AssertCode("@@@", "78FA");
        }

        // ..................:..010011:..:.
        // Reko: a decoder for V850 instruction 7222 at address 00100D82 has not been implemented.
        [Test]
        public void V850Dis_7222()
        {
            AssertCode("@@@", "7222");
        }

        // .................::.:110100:::.:
        // Reko: a decoder for V850 instruction 9D6E at address 00100D84 has not been implemented.
        [Test]
        public void V850Dis_9D6E()
        {
            AssertCode("@@@", "9D6E");
        }

        // ................::...101110:...:
        // Reko: a decoder for V850 instruction D1C5 at address 00100D86 has not been implemented.
        [Test]
        public void V850Dis_D1C5()
        {
            AssertCode("@@@", "D1C5");
        }

        // ....................:001111.::.:
        // Reko: a decoder for V850 instruction ED09 at address 00100D88 has not been implemented.
        [Test]
        public void V850Dis_ED09()
        {
            AssertCode("@@@", "ED09");
        }

        // .................::::111111.....
        // Reko: a decoder for V850 instruction E07F at address 00100D8A has not been implemented.
        [Test]
        public void V850Dis_E07F()
        {
            AssertCode("@@@", "E07F");
        }

        // ..................::.011100:....
        // ..................::..:::..:.... sst_b
        // ................:.:.:111011:::.:
        // Reko: a decoder for V850 instruction 7DAF at address 00100D8E has not been implemented.
        [Test]
        public void V850Dis_7DAF()
        {
            AssertCode("@@@", "7DAF");
        }

        // ....................:001100::::.
        // Reko: a decoder for V850 instruction 9E09 at address 00100D90 has not been implemented.
        [Test]
        public void V850Dis_9E09()
        {
            AssertCode("@@@", "9E09");
        }

        // .................:::.100111:::::
        // .................:::.:..:::::::: sst_h
        // ................:..:.000100:::..
        // Reko: a decoder for V850 instruction 9C90 at address 00100D94 has not been implemented.
        [Test]
        public void V850Dis_9C90()
        {
            AssertCode("@@@", "9C90");
        }

        // ..................:..111000:.::.
        // Reko: a decoder for V850 instruction 1627 at address 00100D96 has not been implemented.
        [Test]
        public void V850Dis_1627()
        {
            AssertCode("@@@", "1627");
        }

        // ................::.::110111:::..
        // Reko: a decoder for V850 instruction FCDE at address 00100D98 has not been implemented.
        [Test]
        public void V850Dis_FCDE()
        {
            AssertCode("@@@", "FCDE");
        }

        // ................::.::100010...::
        // ................::.:::...:....:: sld_h
        // ..................:::100110..::.
        // ..................::::..::...::. sst_h
        // .................:...100011...:.
        // .................:...:...::...:. sld_h
        // .................:...010010.....
        // Reko: a decoder for V850 instruction 4042 at address 00100DA0 has not been implemented.
        [Test]
        public void V850Dis_4042()
        {
            AssertCode("@@@", "4042");
        }

        // .................::..001100:.:::
        // Reko: a decoder for V850 instruction 9761 at address 00100DA2 has not been implemented.
        [Test]
        public void V850Dis_9761()
        {
            AssertCode("@@@", "9761");
        }

        // ................:.:::100100::::.
        // ................:.::::..:..::::. sst_h
        // ....................:001101:..::
        // Reko: a decoder for V850 instruction B309 at address 00100DA6 has not been implemented.
        [Test]
        public void V850Dis_B309()
        {
            AssertCode("@@@", "B309");
        }

        // .....................010010::.:.
        // Reko: a decoder for V850 instruction 5A02 at address 00100DA8 has not been implemented.
        [Test]
        public void V850Dis_5A02()
        {
            AssertCode("@@@", "5A02");
        }

        // ................::.::010100:::..
        // Reko: a decoder for V850 instruction 9CDA at address 00100DAA has not been implemented.
        [Test]
        public void V850Dis_9CDA()
        {
            AssertCode("@@@", "9CDA");
        }

        // .................:..:000111.:.::
        // Reko: a decoder for V850 instruction EB48 at address 00100DAC has not been implemented.
        [Test]
        public void V850Dis_EB48()
        {
            AssertCode("@@@", "EB48");
        }

        // ....................:001001:.:..
        // Reko: a decoder for V850 instruction 3409 at address 00100DAE has not been implemented.
        [Test]
        public void V850Dis_3409()
        {
            AssertCode("@@@", "3409");
        }

        // ..................:::010101.:::.
        // Reko: a decoder for V850 instruction AE3A at address 00100DB0 has not been implemented.
        [Test]
        public void V850Dis_AE3A()
        {
            AssertCode("@@@", "AE3A");
        }

        // .................:.::001111:.:::
        // Reko: a decoder for V850 instruction F759 at address 00100DB2 has not been implemented.
        [Test]
        public void V850Dis_F759()
        {
            AssertCode("@@@", "F759");
        }

        // ..................:.:100110:::..
        // ..................:.::..::.:::.. sst_h
        // ................:::..111100.:.:.
        // Reko: a decoder for V850 instruction 8AE7 at address 00100DB6 has not been implemented.
        [Test]
        public void V850Dis_8AE7()
        {
            AssertCode("@@@", "8AE7");
        }

        // .................:..:011111..:..
        // .................:..:.:::::..:.. sst_b
        // ................:.:::110001..::.
        // Reko: a decoder for V850 instruction 26BE at address 00100DBA has not been implemented.
        [Test]
        public void V850Dis_26BE()
        {
            AssertCode("@@@", "26BE");
        }

        // ................::::.101001.:.::
        // Reko: a decoder for V850 instruction 2BF5 at address 00100DBC has not been implemented.
        [Test]
        public void V850Dis_2BF5()
        {
            AssertCode("@@@", "2BF5");
        }

        // ................:.::.000111::::.
        // ................:..::100101..::.
        // ................:..:::..:.:..::. sst_h
        // .................:..:101001::::.
        // Reko: a decoder for V850 instruction 3E4D at address 00100DC2 has not been implemented.
        [Test]
        public void V850Dis_3E4D()
        {
            AssertCode("@@@", "3E4D");
        }

        // ...................::100100::.::
        // ...................:::..:..::.:: sst_h
        // ................:::..001000:....
        // ................:::....:...:.... or
        // ................::::.101010:.::.
        // Reko: a decoder for V850 instruction 56F5 at address 00100DC8 has not been implemented.
        [Test]
        public void V850Dis_56F5()
        {
            AssertCode("@@@", "56F5");
        }

        // ................:..::010000::.:.
        // ................:..::.:....11010
        // ................:..::.:....::.:. mov
        // ..................:::100111:.:..
        // ..................::::..::::.:.. sst_h
        // ................:.::.011001::.::
        // ................:.::..::..:::.:: sld_b
        // ..................:::001101.:.:.
        // Reko: a decoder for V850 instruction AA39 at address 00100DD0 has not been implemented.
        [Test]
        public void V850Dis_AA39()
        {
            AssertCode("@@@", "AA39");
        }

        // ................:...:100001.:::.
        // ................:...::....:.:::. sld_h
        // ....................:001010..:.:
        // Reko: a decoder for V850 instruction 4509 at address 00100DD4 has not been implemented.
        [Test]
        public void V850Dis_4509()
        {
            AssertCode("@@@", "4509");
        }

        // ...................:.011011.:...
        // ...................:..::.::.:... sld_b
        // ................:.::.000101::...
        // Reko: a decoder for V850 instruction B8B0 at address 00100DD8 has not been implemented.
        [Test]
        public void V850Dis_B8B0()
        {
            AssertCode("@@@", "B8B0");
        }

        // .................:.:.011011:::..
        // .................:.:..::.:::::.. sld_b
        // ................::::.101100:.:.:
        // Reko: a decoder for V850 instruction 95F5 at address 00100DDC has not been implemented.
        [Test]
        public void V850Dis_95F5()
        {
            AssertCode("@@@", "95F5");
        }

        // .................::..100110:::.:
        // .................::..:..::.:::.: sst_h
        // ................:::.:011000...::
        // ................:::.:.::......:: sld_b
        // .................:..:111001:..::
        // Reko: a decoder for V850 instruction 334F at address 00100DE2 has not been implemented.
        [Test]
        public void V850Dis_334F()
        {
            AssertCode("@@@", "334F");
        }

        // ................:::..110000.:..:
        // Reko: a decoder for V850 instruction 09E6 at address 00100DE4 has not been implemented.
        [Test]
        public void V850Dis_09E6()
        {
            AssertCode("@@@", "09E6");
        }

        // ................:..:.010011.:::.
        // Reko: a decoder for V850 instruction 6E92 at address 00100DE6 has not been implemented.
        [Test]
        public void V850Dis_6E92()
        {
            AssertCode("@@@", "6E92");
        }

        // ...................:.110001.:..:
        // Reko: a decoder for V850 instruction 2916 at address 00100DE8 has not been implemented.
        [Test]
        public void V850Dis_2916()
        {
            AssertCode("@@@", "2916");
        }

        // ................:::..110000...::
        // Reko: a decoder for V850 instruction 03E6 at address 00100DEA has not been implemented.
        [Test]
        public void V850Dis_03E6()
        {
            AssertCode("@@@", "03E6");
        }

        // ................::::.000000:..:.
        // ................11110......:..:.
        // ................::::.......:..:. mov
        // ................:.:.:000111:..:.
        // Reko: a decoder for V850 instruction F2A8 at address 00100DEE has not been implemented.
        [Test]
        public void V850Dis_F2A8()
        {
            AssertCode("@@@", "F2A8");
        }

        // .................:::.011000.::::
        // .................:::..::....:::: sld_b
        // ................::::.001000:..::
        // ................::::...:...:..:: or
        // .................:::.010110..:::
        // Reko: a decoder for V850 instruction C772 at address 00100DF4 has not been implemented.
        [Test]
        public void V850Dis_C772()
        {
            AssertCode("@@@", "C772");
        }

        // ................:..::101011.:.::
        // Reko: a decoder for V850 instruction 6B9D at address 00100DF6 has not been implemented.
        [Test]
        public void V850Dis_6B9D()
        {
            AssertCode("@@@", "6B9D");
        }

        // ..................:.:010001:.:..
        // Reko: a decoder for V850 instruction 342A at address 00100DF8 has not been implemented.
        [Test]
        public void V850Dis_342A()
        {
            AssertCode("@@@", "342A");
        }

        // ................:::.:011100:.:.:
        // ................:::.:.:::..:.:.: sst_b
        // ................:.:::100100:::::
        // ................:.::::..:..::::: sst_h
        // .................::..000010.:.::
        // ................01100....:..:.::
        // .................::......:.01011
        // .................::......:..:.:: divh
        // ................:.::.101000:.:::
        // ................:.::.:.:...:.::1 Sld.w/Sst.w
        // ................:.::.:.:...:.::: sst_w
        // ....................:001100:.:::
        // Reko: a decoder for V850 instruction 9709 at address 00100E02 has not been implemented.
        [Test]
        public void V850Dis_9709()
        {
            AssertCode("@@@", "9709");
        }

        // ................:::.:100000.:...
        // ................:::.::......:... sld_h
        // ................::...011101.:.:.
        // ................::....:::.:.:.:. sst_b
        // ................:..::011110.:.::
        // ................:..::.::::..:.:: sst_b
        // ..................:.:111011...:.
        // Reko: a decoder for V850 instruction 622F at address 00100E0A has not been implemented.
        [Test]
        public void V850Dis_622F()
        {
            AssertCode("@@@", "622F");
        }

        // ................:....010000..::.
        // ................:.....:....00110
        // ................:.....:......::. mov
        // ................::...111000.....
        // Reko: a decoder for V850 instruction 00C7 at address 00100E0E has not been implemented.
        [Test]
        public void V850Dis_00C7()
        {
            AssertCode("@@@", "00C7");
        }

        // ................::.:.100101::::.
        // ................::.:.:..:.:::::. sst_h
        // ..................:..010010...::
        // Reko: a decoder for V850 instruction 4322 at address 00100E12 has not been implemented.
        [Test]
        public void V850Dis_4322()
        {
            AssertCode("@@@", "4322");
        }

        // ................:::.:011111....:
        // ................:::.:.:::::....: sst_b
        // ................:..:.000100:::.:
        // Reko: a decoder for V850 instruction 9D90 at address 00100E16 has not been implemented.
        [Test]
        public void V850Dis_9D90()
        {
            AssertCode("@@@", "9D90");
        }

        // ................:..:.110011:::::
        // Reko: a decoder for V850 instruction 7F96 at address 00100E18 has not been implemented.
        [Test]
        public void V850Dis_7F96()
        {
            AssertCode("@@@", "7F96");
        }

        // ................::.::111000::.::
        // Reko: a decoder for V850 instruction 1BDF at address 00100E1A has not been implemented.
        [Test]
        public void V850Dis_1BDF()
        {
            AssertCode("@@@", "1BDF");
        }

        // ................:....011010:...:
        // ................:.....::.:.:...: sld_b
        // .................::.:100111:..:.
        // .................::.::..::::..:. sst_h
        // ................:.:..010110.:.:.
        // Reko: a decoder for V850 instruction CAA2 at address 00100E20 has not been implemented.
        [Test]
        public void V850Dis_CAA2()
        {
            AssertCode("@@@", "CAA2");
        }

        // .................:.::101110.:::.
        // Reko: a decoder for V850 instruction CE5D at address 00100E22 has not been implemented.
        [Test]
        public void V850Dis_CE5D()
        {
            AssertCode("@@@", "CE5D");
        }

        // ................:::.:001010::..:
        // Reko: a decoder for V850 instruction 59E9 at address 00100E24 has not been implemented.
        [Test]
        public void V850Dis_59E9()
        {
            AssertCode("@@@", "59E9");
        }

        // ..................:..011100:::::
        // ..................:...:::..::::: sst_b
        // .................:.:.000110.:...
        // Reko: a decoder for V850 instruction C850 at address 00100E28 has not been implemented.
        [Test]
        public void V850Dis_C850()
        {
            AssertCode("@@@", "C850");
        }

        // ..................:.:011001..::.
        // ..................:.:.::..:..::. sld_b
        // ..................:::100000.::::
        // ..................::::......:::: sld_h
        // ................:.::.001100:.:::
        // Reko: a decoder for V850 instruction 97B1 at address 00100E2E has not been implemented.
        [Test]
        public void V850Dis_97B1()
        {
            AssertCode("@@@", "97B1");
        }

        // ................::..:101001.:..:
        // Reko: a decoder for V850 instruction 29CD at address 00100E30 has not been implemented.
        [Test]
        public void V850Dis_29CD()
        {
            AssertCode("@@@", "29CD");
        }

        // ................::..:011001.::..
        // ................::..:.::..:.::.. sld_b
        // ..................:.:000011::...
        // Reko: a decoder for V850 instruction 7828 at address 00100E34 has not been implemented.
        [Test]
        public void V850Dis_7828()
        {
            AssertCode("@@@", "7828");
        }

        // ..................:.:100011::::.
        // ..................:.::...::::::. sld_h
        // .....................110000:.:::
        // Reko: a decoder for V850 instruction 1706 at address 00100E38 has not been implemented.
        [Test]
        public void V850Dis_1706()
        {
            AssertCode("@@@", "1706");
        }

        // .................:::.010100.:..:
        // Reko: a decoder for V850 instruction 8972 at address 00100E3A has not been implemented.
        [Test]
        public void V850Dis_8972()
        {
            AssertCode("@@@", "8972");
        }

        // ................::..:010110:...:
        // Reko: a decoder for V850 instruction D1CA at address 00100E3C has not been implemented.
        [Test]
        public void V850Dis_D1CA()
        {
            AssertCode("@@@", "D1CA");
        }

        // .................:..:000011:..:.
        // Reko: a decoder for V850 instruction 7248 at address 00100E3E has not been implemented.
        [Test]
        public void V850Dis_7248()
        {
            AssertCode("@@@", "7248");
        }

        // ................::::.110101.:..:
        // Reko: a decoder for V850 instruction A9F6 at address 00100E40 has not been implemented.
        [Test]
        public void V850Dis_A9F6()
        {
            AssertCode("@@@", "A9F6");
        }

        // .................::..011011..:..
        // .................::...::.::..:.. sld_b
        // ................:.:..011101:..::
        // ................:.:...:::.::..:: sst_b
        // ................::.:.100011.:::.
        // ................::.:.:...::.:::. sld_h
        // ..................:..000001:.:..
        // ..................:.......::.:.. not
        // ................:..:.001010::..:
        // Reko: a decoder for V850 instruction 5991 at address 00100E4A has not been implemented.
        [Test]
        public void V850Dis_5991()
        {
            AssertCode("@@@", "5991");
        }

        // .................:::.010110..:.:
        // Reko: a decoder for V850 instruction C572 at address 00100E4C has not been implemented.
        [Test]
        public void V850Dis_C572()
        {
            AssertCode("@@@", "C572");
        }

        // .................:...011100.....
        // .................:....:::....... sst_b
        // .................::.:001111::::.
        // Reko: a decoder for V850 instruction FE69 at address 00100E50 has not been implemented.
        [Test]
        public void V850Dis_FE69()
        {
            AssertCode("@@@", "FE69");
        }

        // ................:....011101:.:.:
        // ................:.....:::.::.:.: sst_b
        // ..................:..110010:.::.
        // Reko: a decoder for V850 instruction 5626 at address 00100E54 has not been implemented.
        [Test]
        public void V850Dis_5626()
        {
            AssertCode("@@@", "5626");
        }

        // ................:.:..001111:..::
        // Reko: a decoder for V850 instruction F3A1 at address 00100E56 has not been implemented.
        [Test]
        public void V850Dis_F3A1()
        {
            AssertCode("@@@", "F3A1");
        }

        // ................:.:..010001..:..
        // Reko: a decoder for V850 instruction 24A2 at address 00100E58 has not been implemented.
        [Test]
        public void V850Dis_24A2()
        {
            AssertCode("@@@", "24A2");
        }

        // .................:.:.010010..::.
        // Reko: a decoder for V850 instruction 4652 at address 00100E5A has not been implemented.
        [Test]
        public void V850Dis_4652()
        {
            AssertCode("@@@", "4652");
        }

        // ..................:.:100011.....
        // ..................:.::...::..... sld_h
        // ................:::::110010:...:
        // Reko: a decoder for V850 instruction 51FE at address 00100E5E has not been implemented.
        [Test]
        public void V850Dis_51FE()
        {
            AssertCode("@@@", "51FE");
        }

        // ................::.::101110:::.:
        // Reko: a decoder for V850 instruction DDDD at address 00100E60 has not been implemented.
        [Test]
        public void V850Dis_DDDD()
        {
            AssertCode("@@@", "DDDD");
        }

        // ...................::001100....:
        // Reko: a decoder for V850 instruction 8119 at address 00100E62 has not been implemented.
        [Test]
        public void V850Dis_8119()
        {
            AssertCode("@@@", "8119");
        }

        // ..................::.011110:.:::
        // ..................::..::::.:.::: sst_b
        // ................:....110111::::.
        // Reko: a decoder for V850 instruction FE86 at address 00100E66 has not been implemented.
        [Test]
        public void V850Dis_FE86()
        {
            AssertCode("@@@", "FE86");
        }

        // .................:.::000010:::::
        // ................01011....:.:::::
        // .................:.::....:.11111
        // .................:.::....:.::::: divh
        // .................:.::000100:.::.
        // Reko: a decoder for V850 instruction 9658 at address 00100E6A has not been implemented.
        [Test]
        public void V850Dis_9658()
        {
            AssertCode("@@@", "9658");
        }

        // ................:::.:100110.:::.
        // ................:::.::..::..:::. sst_h
        // ................:.:..100011..::.
        // ................:.:..:...::..::. sld_h
        // .................:...101100..:.:
        // Reko: a decoder for V850 instruction 8545 at address 00100E70 has not been implemented.
        [Test]
        public void V850Dis_8545()
        {
            AssertCode("@@@", "8545");
        }

        // .................::.:000011:..::
        // Reko: a decoder for V850 instruction 7368 at address 00100E72 has not been implemented.
        [Test]
        public void V850Dis_7368()
        {
            AssertCode("@@@", "7368");
        }

        // ................:::.:111000:.:.:
        // Reko: a decoder for V850 instruction 15EF at address 00100E74 has not been implemented.
        [Test]
        public void V850Dis_15EF()
        {
            AssertCode("@@@", "15EF");
        }

        // ................:....010111:..:.
        // Reko: a decoder for V850 instruction F282 at address 00100E76 has not been implemented.
        [Test]
        public void V850Dis_F282()
        {
            AssertCode("@@@", "F282");
        }

        // ................::...010000...:.
        // ................::....:....00010
        // ................::....:.......:. mov
        // ...................:.001001:::::
        // Reko: a decoder for V850 instruction 3F11 at address 00100E7A has not been implemented.
        [Test]
        public void V850Dis_3F11()
        {
            AssertCode("@@@", "3F11");
        }

        // ...................::011010:::..
        // ...................::.::.:.:::.. sld_b
        // .....................111011:.:..
        // Reko: a decoder for V850 instruction 7407 at address 00100E7E has not been implemented.
        [Test]
        public void V850Dis_7407()
        {
            AssertCode("@@@", "7407");
        }

        // .................:.::011110.:::.
        // .................:.::.::::..:::. sst_b
        // ................:::..111101:.:..
        // Reko: a decoder for V850 instruction B4E7 at address 00100E82 has not been implemented.
        [Test]
        public void V850Dis_B4E7()
        {
            AssertCode("@@@", "B4E7");
        }

        // .................::..110111:....
        // Reko: a decoder for V850 instruction F066 at address 00100E84 has not been implemented.
        [Test]
        public void V850Dis_F066()
        {
            AssertCode("@@@", "F066");
        }

        // ................::..:011011.:.::
        // ................::..:.::.::.:.:: sld_b
        // ..................:.:111011.:.:.
        // Reko: a decoder for V850 instruction 6A2F at address 00100E88 has not been implemented.
        [Test]
        public void V850Dis_6A2F()
        {
            AssertCode("@@@", "6A2F");
        }

        // ................:::::110001...::
        // Reko: a decoder for V850 instruction 23FE at address 00100E8A has not been implemented.
        [Test]
        public void V850Dis_23FE()
        {
            AssertCode("@@@", "23FE");
        }

        // ................:..::011100.::::
        // ................:..::.:::...:::: sst_b
        // ..................::.111110:..::
        // Reko: a decoder for V850 instruction D337 at address 00100E8E has not been implemented.
        [Test]
        public void V850Dis_D337()
        {
            AssertCode("@@@", "D337");
        }

        // ................:.:..001001::.::
        // Reko: a decoder for V850 instruction 3BA1 at address 00100E90 has not been implemented.
        [Test]
        public void V850Dis_3BA1()
        {
            AssertCode("@@@", "3BA1");
        }

        // ...................::010110.::..
        // Reko: a decoder for V850 instruction CC1A at address 00100E92 has not been implemented.
        [Test]
        public void V850Dis_CC1A()
        {
            AssertCode("@@@", "CC1A");
        }

        // ................:::.:001101::...
        // ................:::.:101111.:...
        // Reko: a decoder for V850 instruction E8ED at address 00100E96 has not been implemented.
        [Test]
        public void V850Dis_E8ED()
        {
            AssertCode("@@@", "E8ED");
        }

        // ................:::::110111.:.::
        // Reko: a decoder for V850 instruction EBFE at address 00100E98 has not been implemented.
        [Test]
        public void V850Dis_EBFE()
        {
            AssertCode("@@@", "EBFE");
        }

        // ...................:.100000:.:::
        // ...................:.:.....:.::: sld_h
        // ................:::.:100111:.:..
        // ................:::.::..::::.:.. sst_h
        // .................:...011001.:..:
        // .................:....::..:.:..: sld_b
        // ................:...:111111...::
        // Reko: a decoder for V850 instruction E38F at address 00100EA0 has not been implemented.
        [Test]
        public void V850Dis_E38F()
        {
            AssertCode("@@@", "E38F");
        }

        // .................:::.010100.:..:
        // .................::::110001::...
        // Reko: a decoder for V850 instruction 387E at address 00100EA4 has not been implemented.
        [Test]
        public void V850Dis_387E()
        {
            AssertCode("@@@", "387E");
        }

        // ...................::110101:..::
        // Reko: a decoder for V850 instruction B31E at address 00100EA6 has not been implemented.
        [Test]
        public void V850Dis_B31E()
        {
            AssertCode("@@@", "B31E");
        }

        // ................:.:.:000110::.:.
        // Reko: a decoder for V850 instruction DAA8 at address 00100EA8 has not been implemented.
        [Test]
        public void V850Dis_DAA8()
        {
            AssertCode("@@@", "DAA8");
        }

        // ................:::..110111:::.:
        // Reko: a decoder for V850 instruction FDE6 at address 00100EAA has not been implemented.
        [Test]
        public void V850Dis_FDE6()
        {
            AssertCode("@@@", "FDE6");
        }

        // ................::::.100110.:.::
        // ................::::.:..::..:.:: sst_h
        // .................::..110111:..::
        // Reko: a decoder for V850 instruction F366 at address 00100EAE has not been implemented.
        [Test]
        public void V850Dis_F366()
        {
            AssertCode("@@@", "F366");
        }

        // .....................111111.::::
        // Reko: a decoder for V850 instruction EF07 at address 00100EB0 has not been implemented.
        [Test]
        public void V850Dis_EF07()
        {
            AssertCode("@@@", "EF07");
        }

        // ................:.:::011110:.:.:
        // ................:.:::.::::.:.:.: sst_b
        // .................:.::001010.::::
        // Reko: a decoder for V850 instruction 4F59 at address 00100EB4 has not been implemented.
        [Test]
        public void V850Dis_4F59()
        {
            AssertCode("@@@", "4F59");
        }

        // ................:::..100010.:::.
        // ................:::..:...:..:::. sld_h
        // ................::...110011:..::
        // Reko: a decoder for V850 instruction 73C6 at address 00100EB8 has not been implemented.
        [Test]
        public void V850Dis_73C6()
        {
            AssertCode("@@@", "73C6");
        }

        // ................::::.001111:::..
        // Reko: a decoder for V850 instruction FCF1 at address 00100EBA has not been implemented.
        [Test]
        public void V850Dis_FCF1()
        {
            AssertCode("@@@", "FCF1");
        }

        // .................:.:.110010.::::
        // Reko: a decoder for V850 instruction 4F56 at address 00100EBC has not been implemented.
        [Test]
        public void V850Dis_4F56()
        {
            AssertCode("@@@", "4F56");
        }

        // .................::::110110:.::.
        // Reko: a decoder for V850 instruction D67E at address 00100EBE has not been implemented.
        [Test]
        public void V850Dis_D67E()
        {
            AssertCode("@@@", "D67E");
        }

        // ................::.::111000..:.:
        // Reko: a decoder for V850 instruction 05DF at address 00100EC0 has not been implemented.
        [Test]
        public void V850Dis_05DF()
        {
            AssertCode("@@@", "05DF");
        }

        // .....................000000::.:.
        // ................00000......::.:.
        // ...........................11010
        // ...........................::.:. invalid
        // .................::..000000:...:
        // ................01100......:...:
        // .................::........:...: mov
        // ................:.:::101111:::::
        // Reko: a decoder for V850 instruction FFBD at address 00100EC6 has not been implemented.
        [Test]
        public void V850Dis_FFBD()
        {
            AssertCode("@@@", "FFBD");
        }

        // ................:.:.:101111.::.:
        // Reko: a decoder for V850 instruction EDAD at address 00100EC8 has not been implemented.
        [Test]
        public void V850Dis_EDAD()
        {
            AssertCode("@@@", "EDAD");
        }

        // ...................:.000010.....
        // ................00010....:......
        // ...................:.....:.00000
        // ...................:.....:...... fetrap
        // .....................010111.::..
        // Reko: a decoder for V850 instruction EC02 at address 00100ECC has not been implemented.
        [Test]
        public void V850Dis_EC02()
        {
            AssertCode("@@@", "EC02");
        }

        // ................::::.111001....:
        // Reko: a decoder for V850 instruction 21F7 at address 00100ECE has not been implemented.
        [Test]
        public void V850Dis_21F7()
        {
            AssertCode("@@@", "21F7");
        }

        // ................:.::.000000.:.::
        // ................10110.......:.::
        // ................:.::........:.:: mov
        // ................:::.:100001..:.:
        // ................:::.::....:..:.: sld_h
        // .................::.:110000:.::.
        // Reko: a decoder for V850 instruction 166E at address 00100ED4 has not been implemented.
        [Test]
        public void V850Dis_166E()
        {
            AssertCode("@@@", "166E");
        }

        // ................:...:010111:.:..
        // Reko: a decoder for V850 instruction F48A at address 00100ED6 has not been implemented.
        [Test]
        public void V850Dis_F48A()
        {
            AssertCode("@@@", "F48A");
        }

        // ................:.:..100010.....
        // ................:.:..:...:...... sld_h
        // .................:::.001111:::::
        // Reko: a decoder for V850 instruction FF71 at address 00100EDA has not been implemented.
        [Test]
        public void V850Dis_FF71()
        {
            AssertCode("@@@", "FF71");
        }

        // ................::...010100...:.
        // Reko: a decoder for V850 instruction 82C2 at address 00100EDC has not been implemented.
        [Test]
        public void V850Dis_82C2()
        {
            AssertCode("@@@", "82C2");
        }

        // ................:....011110.::::
        // ................:.....::::..:::: sst_b
        // .................::::110110:...:
        // Reko: a decoder for V850 instruction D17E at address 00100EE0 has not been implemented.
        [Test]
        public void V850Dis_D17E()
        {
            AssertCode("@@@", "D17E");
        }

        // ................:::.:100111.....
        // ................:::.::..:::..... sst_h
        // ................::::.010110::.:.
        // Reko: a decoder for V850 instruction DAF2 at address 00100EE4 has not been implemented.
        [Test]
        public void V850Dis_DAF2()
        {
            AssertCode("@@@", "DAF2");
        }

        // ................:...:111010:.:..
        // Reko: a decoder for V850 instruction 548F at address 00100EE6 has not been implemented.
        [Test]
        public void V850Dis_548F()
        {
            AssertCode("@@@", "548F");
        }

        // ...................::000000.:...
        // ................00011.......:...
        // ...................::.......:... mov
        // ................:.:..001110.::.:
        // Reko: a decoder for V850 instruction CDA1 at address 00100EEA has not been implemented.
        [Test]
        public void V850Dis_CDA1()
        {
            AssertCode("@@@", "CDA1");
        }

        // ..................:::110000::...
        // Reko: a decoder for V850 instruction 183E at address 00100EEC has not been implemented.
        [Test]
        public void V850Dis_183E()
        {
            AssertCode("@@@", "183E");
        }

        // .................:::.001111:.:::
        // Reko: a decoder for V850 instruction F771 at address 00100EEE has not been implemented.
        [Test]
        public void V850Dis_F771()
        {
            AssertCode("@@@", "F771");
        }

        // .....................101101..:..
        // Reko: a decoder for V850 instruction A405 at address 00100EF0 has not been implemented.
        [Test]
        public void V850Dis_A405()
        {
            AssertCode("@@@", "A405");
        }

        // ................:..::110111..::.
        // Reko: a decoder for V850 instruction E69E at address 00100EF2 has not been implemented.
        [Test]
        public void V850Dis_E69E()
        {
            AssertCode("@@@", "E69E");
        }

        // ................:::.:010001:...:
        // Reko: a decoder for V850 instruction 31EA at address 00100EF4 has not been implemented.
        [Test]
        public void V850Dis_31EA()
        {
            AssertCode("@@@", "31EA");
        }

        // ................:..:.111011..:::
        // Reko: a decoder for V850 instruction 6797 at address 00100EF6 has not been implemented.
        [Test]
        public void V850Dis_6797()
        {
            AssertCode("@@@", "6797");
        }

        // ................:...:111111.:.::
        // Reko: a decoder for V850 instruction EB8F at address 00100EF8 has not been implemented.
        [Test]
        public void V850Dis_EB8F()
        {
            AssertCode("@@@", "EB8F");
        }

        // .................::.:101110.....
        // Reko: a decoder for V850 instruction C06D at address 00100EFA has not been implemented.
        [Test]
        public void V850Dis_C06D()
        {
            AssertCode("@@@", "C06D");
        }

        // ................:...:101011....:
        // Reko: a decoder for V850 instruction 618D at address 00100EFC has not been implemented.
        [Test]
        public void V850Dis_618D()
        {
            AssertCode("@@@", "618D");
        }

        // ..................:.:011001::.:.
        // ..................:.:.::..:::.:. sld_b
        // ................:.:::101110::::.
        // Reko: a decoder for V850 instruction DEBD at address 00100F00 has not been implemented.
        [Test]
        public void V850Dis_DEBD()
        {
            AssertCode("@@@", "DEBD");
        }

        // .................::.:110001::::.
        // Reko: a decoder for V850 instruction 3E6E at address 00100F02 has not been implemented.
        [Test]
        public void V850Dis_3E6E()
        {
            AssertCode("@@@", "3E6E");
        }

        // ..................::.101001....:
        // Reko: a decoder for V850 instruction 2135 at address 00100F04 has not been implemented.
        [Test]
        public void V850Dis_2135()
        {
            AssertCode("@@@", "2135");
        }

        // ...................::001000:::.:
        // ...................::..:...:::.: or
        // ................:..:.101010::.::
        // Reko: a decoder for V850 instruction 5B95 at address 00100F08 has not been implemented.
        [Test]
        public void V850Dis_5B95()
        {
            AssertCode("@@@", "5B95");
        }

        // ................:::::110111...::
        // Reko: a decoder for V850 instruction E3FE at address 00100F0A has not been implemented.
        [Test]
        public void V850Dis_E3FE()
        {
            AssertCode("@@@", "E3FE");
        }

        // .................:.:.011101.....
        // .................:.:..:::.:..... sst_b
        // .....................001011:...:
        // Reko: a decoder for V850 instruction 7101 at address 00100F0E has not been implemented.
        [Test]
        public void V850Dis_7101()
        {
            AssertCode("@@@", "7101");
        }

        // ................:...:110101.::..
        // Reko: a decoder for V850 instruction AC8E at address 00100F10 has not been implemented.
        [Test]
        public void V850Dis_AC8E()
        {
            AssertCode("@@@", "AC8E");
        }

        // .................::::101110.::.:
        // ..................::.000110::.::
        // Reko: a decoder for V850 instruction DB30 at address 00100F14 has not been implemented.
        [Test]
        public void V850Dis_DB30()
        {
            AssertCode("@@@", "DB30");
        }

        // ................:::..111010:...:
        // Reko: a decoder for V850 instruction 51E7 at address 00100F16 has not been implemented.
        [Test]
        public void V850Dis_51E7()
        {
            AssertCode("@@@", "51E7");
        }

        // .................:..:001000:.:::
        // .................:..:..:...:.::: or
        // .................:..:010000.....
        // .................:..:.:....00000
        // .................:..:.:......... invalid
        // ................:::..111001:..:.
        // Reko: a decoder for V850 instruction 32E7 at address 00100F1C has not been implemented.
        [Test]
        public void V850Dis_32E7()
        {
            AssertCode("@@@", "32E7");
        }

        // .................::::011001.:::.
        // .................::::.::..:.:::. sld_b
        // ................:.:..010110:::.:
        // Reko: a decoder for V850 instruction DDA2 at address 00100F20 has not been implemented.
        [Test]
        public void V850Dis_DDA2()
        {
            AssertCode("@@@", "DDA2");
        }

        // .................:.::011110...:.
        // .................:.::.::::....:. sst_b
        // ................:...:001111:::::
        // Reko: a decoder for V850 instruction FF89 at address 00100F24 has not been implemented.
        [Test]
        public void V850Dis_FF89()
        {
            AssertCode("@@@", "FF89");
        }

        // .................::.:110010.::::
        // Reko: a decoder for V850 instruction 4F6E at address 00100F26 has not been implemented.
        [Test]
        public void V850Dis_4F6E()
        {
            AssertCode("@@@", "4F6E");
        }

        // ................::..:101111.:...
        // Reko: a decoder for V850 instruction E8CD at address 00100F28 has not been implemented.
        [Test]
        public void V850Dis_E8CD()
        {
            AssertCode("@@@", "E8CD");
        }

        // ................:..::100010...:.
        // ................:..:::...:....:. sld_h
        // .................::.:000000..::.
        // ................01101........::.
        // .................::.:........::. mov
        // ................:..:.111111:.:::
        // Reko: a decoder for V850 instruction F797 at address 00100F2E has not been implemented.
        [Test]
        public void V850Dis_F797()
        {
            AssertCode("@@@", "F797");
        }

        // ................:.:::110000:::.:
        // Reko: a decoder for V850 instruction 1DBE at address 00100F30 has not been implemented.
        [Test]
        public void V850Dis_1DBE()
        {
            AssertCode("@@@", "1DBE");
        }

        // ................::.:.010110.....
        // ................:.:::101110.....
        // Reko: a decoder for V850 instruction C0BD at address 00100F34 has not been implemented.
        [Test]
        public void V850Dis_C0BD()
        {
            AssertCode("@@@", "C0BD");
        }

        // ................:.:.:101010.::::
        // Reko: a decoder for V850 instruction 4FAD at address 00100F36 has not been implemented.
        [Test]
        public void V850Dis_4FAD()
        {
            AssertCode("@@@", "4FAD");
        }

        // .................:.:.111011.::..
        // Reko: a decoder for V850 instruction 6C57 at address 00100F38 has not been implemented.
        [Test]
        public void V850Dis_6C57()
        {
            AssertCode("@@@", "6C57");
        }

        // ................::.::000010:.::.
        // ................11011....:.:.::.
        // ................::.::....:.10110
        // ................::.::....:.:.::. divh
        // ................::.:.010001:.:.:
        // Reko: a decoder for V850 instruction 35D2 at address 00100F3C has not been implemented.
        [Test]
        public void V850Dis_35D2()
        {
            AssertCode("@@@", "35D2");
        }

        // .................:::.011111..::.
        // .................:::..:::::..::. sst_b
        // .................::.:000011..:::
        // Reko: a decoder for V850 instruction 6768 at address 00100F40 has not been implemented.
        [Test]
        public void V850Dis_6768()
        {
            AssertCode("@@@", "6768");
        }

        // ................:::::110001.....
        // Reko: a decoder for V850 instruction 20FE at address 00100F42 has not been implemented.
        [Test]
        public void V850Dis_20FE()
        {
            AssertCode("@@@", "20FE");
        }

        // ...................:.110100:....
        // Reko: a decoder for V850 instruction 9016 at address 00100F44 has not been implemented.
        [Test]
        public void V850Dis_9016()
        {
            AssertCode("@@@", "9016");
        }

        // ..................:..011000....:
        // ..................:...::.......: sld_b
        // .................:.::110001::::.
        // Reko: a decoder for V850 instruction 3E5E at address 00100F48 has not been implemented.
        [Test]
        public void V850Dis_3E5E()
        {
            AssertCode("@@@", "3E5E");
        }

        // ....................:110100:.:.:
        // Reko: a decoder for V850 instruction 950E at address 00100F4A has not been implemented.
        [Test]
        public void V850Dis_950E()
        {
            AssertCode("@@@", "950E");
        }

        // ................:.::.101111.:::.
        // Reko: a decoder for V850 instruction EEB5 at address 00100F4C has not been implemented.
        [Test]
        public void V850Dis_EEB5()
        {
            AssertCode("@@@", "EEB5");
        }

        // ................:.:.:110111.....
        // Reko: a decoder for V850 instruction E0AE at address 00100F4E has not been implemented.
        [Test]
        public void V850Dis_E0AE()
        {
            AssertCode("@@@", "E0AE");
        }

        // .................::.:000010:..:.
        // ................01101....:.:..:.
        // .................::.:....:.10010
        // .................::.:....:.:..:. divh
        // .................:::.011001.::..
        // .................:::..::..:.::.. sld_b
        // .................:.::011001..:::
        // .................:.::.::..:..::: sld_b
        // ..................:..000101::.:.
        // Reko: a decoder for V850 instruction BA20 at address 00100F56 has not been implemented.
        [Test]
        public void V850Dis_BA20()
        {
            AssertCode("@@@", "BA20");
        }

        // ...................:.101010:..:.
        // Reko: a decoder for V850 instruction 5215 at address 00100F58 has not been implemented.
        [Test]
        public void V850Dis_5215()
        {
            AssertCode("@@@", "5215");
        }

        // ................:.:.:000111.:::.
        // Reko: a decoder for V850 instruction EEA8 at address 00100F5A has not been implemented.
        [Test]
        public void V850Dis_EEA8()
        {
            AssertCode("@@@", "EEA8");
        }

        // .................:::.111001:..::
        // Reko: a decoder for V850 instruction 3377 at address 00100F5C has not been implemented.
        [Test]
        public void V850Dis_3377()
        {
            AssertCode("@@@", "3377");
        }

        // ...................:.101001::..:
        // Reko: a decoder for V850 instruction 3915 at address 00100F5E has not been implemented.
        [Test]
        public void V850Dis_3915()
        {
            AssertCode("@@@", "3915");
        }

        // ................::..:110111..::.
        // Reko: a decoder for V850 instruction E6CE at address 00100F60 has not been implemented.
        [Test]
        public void V850Dis_E6CE()
        {
            AssertCode("@@@", "E6CE");
        }

        // ................:..::101001:.:..
        // Reko: a decoder for V850 instruction 349D at address 00100F62 has not been implemented.
        [Test]
        public void V850Dis_349D()
        {
            AssertCode("@@@", "349D");
        }

        // ................:::::000010.:...
        // ................11111....:..:...
        // ................:::::....:.01000
        // ................:::::....:..:... divh
        // .....................111000..:::
        // Reko: a decoder for V850 instruction 0707 at address 00100F66 has not been implemented.
        [Test]
        public void V850Dis_0707()
        {
            AssertCode("@@@", "0707");
        }

        // ................:..::100101:::..
        // ................:..:::..:.::::.. sst_h
        // .................::..010100:.:..
        // Reko: a decoder for V850 instruction 9462 at address 00100F6A has not been implemented.
        [Test]
        public void V850Dis_9462()
        {
            AssertCode("@@@", "9462");
        }

        // .................:...001001.:...
        // Reko: a decoder for V850 instruction 2841 at address 00100F6C has not been implemented.
        [Test]
        public void V850Dis_2841()
        {
            AssertCode("@@@", "2841");
        }

        // ................:.::.111101:::::
        // Reko: a decoder for V850 instruction BFB7 at address 00100F6E has not been implemented.
        [Test]
        public void V850Dis_BFB7()
        {
            AssertCode("@@@", "BFB7");
        }

        // ................:.:..111011:.::.
        // Reko: a decoder for V850 instruction 76A7 at address 00100F70 has not been implemented.
        [Test]
        public void V850Dis_76A7()
        {
            AssertCode("@@@", "76A7");
        }

        // ................::..:100100..:.:
        // ................::..::..:....:.: sst_h
        // .................:::.011101..:.:
        // .................:::..:::.:..:.: sst_b
        // ....................:100010.::..
        // ....................::...:..::.. sld_h
        // .....................000101.:::.
        // Reko: a decoder for V850 instruction AE00 at address 00100F78 has not been implemented.
        [Test]
        public void V850Dis_AE00()
        {
            AssertCode("@@@", "AE00");
        }

        // .................::::011101.::..
        // .................::::.:::.:.::.. sst_b
        // .................:.::001001..:::
        // Reko: a decoder for V850 instruction 2759 at address 00100F7C has not been implemented.
        [Test]
        public void V850Dis_2759()
        {
            AssertCode("@@@", "2759");
        }

        // ................::...110111.::::
        // Reko: a decoder for V850 instruction EFC6 at address 00100F7E has not been implemented.
        [Test]
        public void V850Dis_EFC6()
        {
            AssertCode("@@@", "EFC6");
        }

        // .................::::111001..:..
        // Reko: a decoder for V850 instruction 247F at address 00100F80 has not been implemented.
        [Test]
        public void V850Dis_247F()
        {
            AssertCode("@@@", "247F");
        }

        // ...................::111001.:...
        // Reko: a decoder for V850 instruction 281F at address 00100F82 has not been implemented.
        [Test]
        public void V850Dis_281F()
        {
            AssertCode("@@@", "281F");
        }

        // .................:...011100.....
        // .................:....:::....... sst_b
        // .................:.::001011..::.
        // Reko: a decoder for V850 instruction 6659 at address 00100F86 has not been implemented.
        [Test]
        public void V850Dis_6659()
        {
            AssertCode("@@@", "6659");
        }

        // ..................:..101011....:
        // Reko: a decoder for V850 instruction 6125 at address 00100F88 has not been implemented.
        [Test]
        public void V850Dis_6125()
        {
            AssertCode("@@@", "6125");
        }

        // ................:...:010101:.::.
        // Reko: a decoder for V850 instruction B68A at address 00100F8A has not been implemented.
        [Test]
        public void V850Dis_B68A()
        {
            AssertCode("@@@", "B68A");
        }

        // .................:.::000110..::.
        // Reko: a decoder for V850 instruction C658 at address 00100F8C has not been implemented.
        [Test]
        public void V850Dis_C658()
        {
            AssertCode("@@@", "C658");
        }

        // ...................:.001111::...
        // Reko: a decoder for V850 instruction F811 at address 00100F8E has not been implemented.
        [Test]
        public void V850Dis_F811()
        {
            AssertCode("@@@", "F811");
        }

        // ..................::.111010:.::.
        // Reko: a decoder for V850 instruction 5637 at address 00100F90 has not been implemented.
        [Test]
        public void V850Dis_5637()
        {
            AssertCode("@@@", "5637");
        }

        // ................:...:011001:..:.
        // ................:...:.::..::..:. sld_b
        // .................::.:101111:...:
        // Reko: a decoder for V850 instruction F16D at address 00100F94 has not been implemented.
        [Test]
        public void V850Dis_F16D()
        {
            AssertCode("@@@", "F16D");
        }

        // .................:::.011011.::::
        // .................:::..::.::.:::: sld_b
        // ..................:.:000100...:.
        // Reko: a decoder for V850 instruction 8228 at address 00100F98 has not been implemented.
        [Test]
        public void V850Dis_8228()
        {
            AssertCode("@@@", "8228");
        }

        // .................:..:000111.::::
        // Reko: a decoder for V850 instruction EF48 at address 00100F9A has not been implemented.
        [Test]
        public void V850Dis_EF48()
        {
            AssertCode("@@@", "EF48");
        }

        // ................:...:100010.::..
        // ................:...::...:..::.. sld_h
        // .................::..011111.....
        // .................::...:::::..... sst_b
        // ................::..:110101.::.:
        // Reko: a decoder for V850 instruction ADCE at address 00100FA0 has not been implemented.
        [Test]
        public void V850Dis_ADCE()
        {
            AssertCode("@@@", "ADCE");
        }

        // ................:.:.:000001:::.:
        // ................:.:.:.....::::.: not
        // ................:.:..000000::..:
        // ................10100......::..:
        // ................:.:........::..: mov
        // ..................::.011001:.::.
        // ..................::..::..::.::. sld_b
        // ..................:.:100010....:
        // ..................:.::...:.....: sld_h
        // ................:....000011.:.:.
        // Reko: a decoder for V850 instruction 6A80 at address 00100FAA has not been implemented.
        [Test]
        public void V850Dis_6A80()
        {
            AssertCode("@@@", "6A80");
        }

        // ................::...001101:::..
        // Reko: a decoder for V850 instruction BCC1 at address 00100FAC has not been implemented.
        [Test]
        public void V850Dis_BCC1()
        {
            AssertCode("@@@", "BCC1");
        }

        // .................:.:.110010..:.:
        // Reko: a decoder for V850 instruction 4556 at address 00100FAE has not been implemented.
        [Test]
        public void V850Dis_4556()
        {
            AssertCode("@@@", "4556");
        }

        // ................:..::010000..:::
        // ................:..::.:....00111
        // ................:..::.:......::: mov
        // ................:::.:111001..:..
        // Reko: a decoder for V850 instruction 24EF at address 00100FB2 has not been implemented.
        [Test]
        public void V850Dis_24EF()
        {
            AssertCode("@@@", "24EF");
        }

        // .................::..100001..::.
        // .................::..:....:..::. sld_h
        // ................:.::.111110:.:..
        // Reko: a decoder for V850 instruction D4B7 at address 00100FB6 has not been implemented.
        [Test]
        public void V850Dis_D4B7()
        {
            AssertCode("@@@", "D4B7");
        }

        // ................:.::.100000:....
        // ................:.::.:.....:.... sld_h
        // .................:.:.111100:::..
        // Reko: a decoder for V850 instruction 9C57 at address 00100FBA has not been implemented.
        [Test]
        public void V850Dis_9C57()
        {
            AssertCode("@@@", "9C57");
        }

        // ...................::101010:..::
        // Reko: a decoder for V850 instruction 531D at address 00100FBC has not been implemented.
        [Test]
        public void V850Dis_531D()
        {
            AssertCode("@@@", "531D");
        }

        // ................::.:.101000.::..
        // ................::.:.:.:....::.0 Sld.w/Sst.w
        // ................::.:.:.:....::.. sld_w
        // .................:.:.011010..:.:
        // .................:.:..::.:...:.: sld_b
        // ................::::.111110:::.:
        // Reko: a decoder for V850 instruction DDF7 at address 00100FC2 has not been implemented.
        [Test]
        public void V850Dis_DDF7()
        {
            AssertCode("@@@", "DDF7");
        }

        // .................:.:.000000::.:.
        // ................01010......::.:.
        // .................:.:.......::.:. mov
        // ................:.::.110111::...
        // Reko: a decoder for V850 instruction F8B6 at address 00100FC6 has not been implemented.
        [Test]
        public void V850Dis_F8B6()
        {
            AssertCode("@@@", "F8B6");
        }

        // ................:::::111000....:
        // Reko: a decoder for V850 instruction 01FF at address 00100FC8 has not been implemented.
        [Test]
        public void V850Dis_01FF()
        {
            AssertCode("@@@", "01FF");
        }

        // ................::...101010.:.:.
        // Reko: a decoder for V850 instruction 4AC5 at address 00100FCA has not been implemented.
        [Test]
        public void V850Dis_4AC5()
        {
            AssertCode("@@@", "4AC5");
        }

        // ................:::.:111000...:.
        // Reko: a decoder for V850 instruction 02EF at address 00100FCC has not been implemented.
        [Test]
        public void V850Dis_02EF()
        {
            AssertCode("@@@", "02EF");
        }

        // ................::...001101:.:::
        // Reko: a decoder for V850 instruction B7C1 at address 00100FCE has not been implemented.
        [Test]
        public void V850Dis_B7C1()
        {
            AssertCode("@@@", "B7C1");
        }

        // ................:..::001111...:.
        // Reko: a decoder for V850 instruction E299 at address 00100FD0 has not been implemented.
        [Test]
        public void V850Dis_E299()
        {
            AssertCode("@@@", "E299");
        }

        // ................:.::.001010.....
        // Reko: a decoder for V850 instruction 40B1 at address 00100FD2 has not been implemented.
        [Test]
        public void V850Dis_40B1()
        {
            AssertCode("@@@", "40B1");
        }

        // ................::::.000011..:::
        // Reko: a decoder for V850 instruction 67F0 at address 00100FD4 has not been implemented.
        [Test]
        public void V850Dis_67F0()
        {
            AssertCode("@@@", "67F0");
        }

        // .................:..:001001:::.:
        // Reko: a decoder for V850 instruction 3D49 at address 00100FD6 has not been implemented.
        [Test]
        public void V850Dis_3D49()
        {
            AssertCode("@@@", "3D49");
        }

        // ................:....110111::.:.
        // Reko: a decoder for V850 instruction FA86 at address 00100FD8 has not been implemented.
        [Test]
        public void V850Dis_FA86()
        {
            AssertCode("@@@", "FA86");
        }

        // .................::..100100::...
        // .................::..:..:..::... sst_h
        // ................::.::110000...::
        // Reko: a decoder for V850 instruction 03DE at address 00100FDC has not been implemented.
        [Test]
        public void V850Dis_03DE()
        {
            AssertCode("@@@", "03DE");
        }

        // ..................:..011110:::::
        // ..................:...::::.::::: sst_b
        // ................:..:.101000:::::
        // ................:..:.:.:...::::1 Sld.w/Sst.w
        // ................:..:.:.:...::::: sst_w
        // .................::.:000001::.::
        // .................::.:.....:::.:: not
        // .................:..:110111...::
        // Reko: a decoder for V850 instruction E34E at address 00100FE4 has not been implemented.
        [Test]
        public void V850Dis_E34E()
        {
            AssertCode("@@@", "E34E");
        }

        // ................:.::.111001::.::
        // Reko: a decoder for V850 instruction 3BB7 at address 00100FE6 has not been implemented.
        [Test]
        public void V850Dis_3BB7()
        {
            AssertCode("@@@", "3BB7");
        }

        // .................::.:110101...:.
        // Reko: a decoder for V850 instruction A26E at address 00100FE8 has not been implemented.
        [Test]
        public void V850Dis_A26E()
        {
            AssertCode("@@@", "A26E");
        }

        // ..................:..110111.:::.
        // Reko: a decoder for V850 instruction EE26 at address 00100FEA has not been implemented.
        [Test]
        public void V850Dis_EE26()
        {
            AssertCode("@@@", "EE26");
        }

        // ....................:110000::.:.
        // Reko: a decoder for V850 instruction 1A0E at address 00100FEC has not been implemented.
        [Test]
        public void V850Dis_1A0E()
        {
            AssertCode("@@@", "1A0E");
        }

        // ..................:..001101:.:..
        // Reko: a decoder for V850 instruction B421 at address 00100FEE has not been implemented.
        [Test]
        public void V850Dis_B421()
        {
            AssertCode("@@@", "B421");
        }

        // ................::.::010111:::.:
        // Reko: a decoder for V850 instruction FDDA at address 00100FF0 has not been implemented.
        [Test]
        public void V850Dis_FDDA()
        {
            AssertCode("@@@", "FDDA");
        }

        // ................:.:::010011:.:..
        // Reko: a decoder for V850 instruction 74BA at address 00100FF2 has not been implemented.
        [Test]
        public void V850Dis_74BA()
        {
            AssertCode("@@@", "74BA");
        }

        // .................:.::010110:::.:
        // Reko: a decoder for V850 instruction DD5A at address 00100FF4 has not been implemented.
        [Test]
        public void V850Dis_DD5A()
        {
            AssertCode("@@@", "DD5A");
        }

        // .................:.:.100011.::.:
        // .................:.:.:...::.::.: sld_h
        // ................:..:.100000:.::.
        // ................:..:.:.....:.::. sld_h
        // ................:..:.100111:::..
        // ................:..:.:..::::::.. sst_h
        // ................:..:.011101:.:::
        // ................:..:..:::.::.::: sst_b
        // ..................:..010101:..:.
        // Reko: a decoder for V850 instruction B222 at address 00100FFE has not been implemented.
        [Test]
        public void V850Dis_B222()
        {
            AssertCode("@@@", "B222");
        }

        // ..................:..110001.....
        // Reko: a decoder for V850 instruction 2026 at address 00101000 has not been implemented.
        [Test]
        public void V850Dis_2026()
        {
            AssertCode("@@@", "2026");
        }

        // .................::.:101101..:.:
        // Reko: a decoder for V850 instruction A56D at address 00101002 has not been implemented.
        [Test]
        public void V850Dis_A56D()
        {
            AssertCode("@@@", "A56D");
        }

        // .................:..:110101:..::
        // Reko: a decoder for V850 instruction B34E at address 00101004 has not been implemented.
        [Test]
        public void V850Dis_B34E()
        {
            AssertCode("@@@", "B34E");
        }

        // ................:..:.100110:::.:
        // ................:..:.:..::.:::.: sst_h
        // ................:...:001010:::.:
        // Reko: a decoder for V850 instruction 5D89 at address 00101008 has not been implemented.
        [Test]
        public void V850Dis_5D89()
        {
            AssertCode("@@@", "5D89");
        }

        // ................:::::000000.:::.
        // ................11111.......:::.
        // ................:::::.......:::. mov
        // ...................:.110000...:.
        // Reko: a decoder for V850 instruction 0216 at address 0010100C has not been implemented.
        [Test]
        public void V850Dis_0216()
        {
            AssertCode("@@@", "0216");
        }

        // ................:::.:011001.:::.
        // ................:::.:.::..:.:::. sld_b
        // .................:::.111110:...:
        // Reko: a decoder for V850 instruction D177 at address 00101010 has not been implemented.
        [Test]
        public void V850Dis_D177()
        {
            AssertCode("@@@", "D177");
        }

        // ..................:.:000011..::.
        // Reko: a decoder for V850 instruction 6628 at address 00101012 has not been implemented.
        [Test]
        public void V850Dis_6628()
        {
            AssertCode("@@@", "6628");
        }

        // ..................:..100011.:..:
        // ..................:..:...::.:..: sld_h
        // ................::...100010..::.
        // ................::...:...:...::. sld_h
        // ................:::..110001..:::
        // Reko: a decoder for V850 instruction 27E6 at address 00101018 has not been implemented.
        [Test]
        public void V850Dis_27E6()
        {
            AssertCode("@@@", "27E6");
        }

        // .................:..:110010:..:.
        // Reko: a decoder for V850 instruction 524E at address 0010101A has not been implemented.
        [Test]
        public void V850Dis_524E()
        {
            AssertCode("@@@", "524E");
        }

        // ................:.:..111010:...:
        // Reko: a decoder for V850 instruction 51A7 at address 0010101C has not been implemented.
        [Test]
        public void V850Dis_51A7()
        {
            AssertCode("@@@", "51A7");
        }

        // ....................:111111:::::
        // Reko: a decoder for V850 instruction FF0F at address 0010101E has not been implemented.
        [Test]
        public void V850Dis_FF0F()
        {
            AssertCode("@@@", "FF0F");
        }

        // ................::..:011101:::..
        // ................::..:.:::.::::.. sst_b
        // ................::::.011000..::.
        // ................::::..::.....::. sld_b
        // .................:...111011.:..:
        // Reko: a decoder for V850 instruction 6947 at address 00101024 has not been implemented.
        [Test]
        public void V850Dis_6947()
        {
            AssertCode("@@@", "6947");
        }

        // ................:.:.:111011.::.:
        // Reko: a decoder for V850 instruction 6DAF at address 00101026 has not been implemented.
        [Test]
        public void V850Dis_6DAF()
        {
            AssertCode("@@@", "6DAF");
        }

        // ................::.::111111:::..
        // Reko: a decoder for V850 instruction FCDF at address 00101028 has not been implemented.
        [Test]
        public void V850Dis_FCDF()
        {
            AssertCode("@@@", "FCDF");
        }

        // .................:.:.100010:::.:
        // .................:.:.:...:.:::.: sld_h
        // .................:::.101010.::..
        // Reko: a decoder for V850 instruction 4C75 at address 0010102C has not been implemented.
        [Test]
        public void V850Dis_4C75()
        {
            AssertCode("@@@", "4C75");
        }

        // ................:::::111010:..:.
        // Reko: a decoder for V850 instruction 52FF at address 0010102E has not been implemented.
        [Test]
        public void V850Dis_52FF()
        {
            AssertCode("@@@", "52FF");
        }

        // ...................:.001011..:.:
        // Reko: a decoder for V850 instruction 6511 at address 00101030 has not been implemented.
        [Test]
        public void V850Dis_6511()
        {
            AssertCode("@@@", "6511");
        }

        // .................:...000110...::
        // Reko: a decoder for V850 instruction C340 at address 00101032 has not been implemented.
        [Test]
        public void V850Dis_C340()
        {
            AssertCode("@@@", "C340");
        }

        // ................:...:001001.::.:
        // Reko: a decoder for V850 instruction 2D89 at address 00101034 has not been implemented.
        [Test]
        public void V850Dis_2D89()
        {
            AssertCode("@@@", "2D89");
        }

        // ................:::::011101::...
        // ................:::::.:::.:::... sst_b
        // ................:::..000111.....
        // Reko: a decoder for V850 instruction E0E0 at address 00101038 has not been implemented.
        [Test]
        public void V850Dis_E0E0()
        {
            AssertCode("@@@", "E0E0");
        }

        // ................::..:100010..:.:
        // ................::..::...:...:.: sld_h
        // ................:...:111111::.::
        // Reko: a decoder for V850 instruction FB8F at address 0010103C has not been implemented.
        [Test]
        public void V850Dis_FB8F()
        {
            AssertCode("@@@", "FB8F");
        }

        // ................:.::.110010...::
        // Reko: a decoder for V850 instruction 43B6 at address 0010103E has not been implemented.
        [Test]
        public void V850Dis_43B6()
        {
            AssertCode("@@@", "43B6");
        }

        // ................:::::111100.:..:
        // Reko: a decoder for V850 instruction 89FF at address 00101040 has not been implemented.
        [Test]
        public void V850Dis_89FF()
        {
            AssertCode("@@@", "89FF");
        }

        // ..................::.110001::.::
        // Reko: a decoder for V850 instruction 3B36 at address 00101042 has not been implemented.
        [Test]
        public void V850Dis_3B36()
        {
            AssertCode("@@@", "3B36");
        }

        // ..................:::011000.::::
        // ..................:::.::....:::: sld_b
        // ................:...:001100...:.
        // Reko: a decoder for V850 instruction 8289 at address 00101046 has not been implemented.
        [Test]
        public void V850Dis_8289()
        {
            AssertCode("@@@", "8289");
        }

        // ................:.::.110000.:..:
        // Reko: a decoder for V850 instruction 09B6 at address 00101048 has not been implemented.
        [Test]
        public void V850Dis_09B6()
        {
            AssertCode("@@@", "09B6");
        }

        // ................:...:001001.::..
        // Reko: a decoder for V850 instruction 2C89 at address 0010104A has not been implemented.
        [Test]
        public void V850Dis_2C89()
        {
            AssertCode("@@@", "2C89");
        }

        // ................::::.010110..:::
        // Reko: a decoder for V850 instruction C7F2 at address 0010104C has not been implemented.
        [Test]
        public void V850Dis_C7F2()
        {
            AssertCode("@@@", "C7F2");
        }

        // ................:.::.001011::...
        // Reko: a decoder for V850 instruction 78B1 at address 0010104E has not been implemented.
        [Test]
        public void V850Dis_78B1()
        {
            AssertCode("@@@", "78B1");
        }

        // .................:.:.011100:.:..
        // .................:.:..:::..:.:.. sst_b
        // ..................:::111111.:..:
        // Reko: a decoder for V850 instruction E93F at address 00101052 has not been implemented.
        [Test]
        public void V850Dis_E93F()
        {
            AssertCode("@@@", "E93F");
        }

        // ................:.:::111111..:..
        // Reko: a decoder for V850 instruction E4BF at address 00101054 has not been implemented.
        [Test]
        public void V850Dis_E4BF()
        {
            AssertCode("@@@", "E4BF");
        }

        // ..................::.011111....:
        // ..................::..:::::....: sst_b
        // ....................:010000:..::
        // ....................:.:....10011
        // ....................:.:....:..:: mov
        // ................:...:000000:..::
        // ................10001......:..::
        // ................:...:......:..:: mov
        // ................:.:..001000....:
        // ................:.:....:.......: or
        // .................::.:101101:.:..
        // Reko: a decoder for V850 instruction B46D at address 0010105E has not been implemented.
        [Test]
        public void V850Dis_B46D()
        {
            AssertCode("@@@", "B46D");
        }

        // ................:.:..110100:::..
        // Reko: a decoder for V850 instruction 9CA6 at address 00101060 has not been implemented.
        [Test]
        public void V850Dis_9CA6()
        {
            AssertCode("@@@", "9CA6");
        }

        // .................:..:100110.:.:.
        // .................:..::..::..:.:. sst_h
        // ...................::100001::..:
        // ...................:::....:::..: sld_h
        // ..................:.:010111.::::
        // Reko: a decoder for V850 instruction EF2A at address 00101066 has not been implemented.
        [Test]
        public void V850Dis_EF2A()
        {
            AssertCode("@@@", "EF2A");
        }

        // ..................:::001100.:::.
        // Reko: a decoder for V850 instruction 8E39 at address 00101068 has not been implemented.
        [Test]
        public void V850Dis_8E39()
        {
            AssertCode("@@@", "8E39");
        }

        // .................:::.110001:.:::
        // Reko: a decoder for V850 instruction 3776 at address 0010106A has not been implemented.
        [Test]
        public void V850Dis_3776()
        {
            AssertCode("@@@", "3776");
        }

        // ..................:.:110010:::.:
        // Reko: a decoder for V850 instruction 5D2E at address 0010106C has not been implemented.
        [Test]
        public void V850Dis_5D2E()
        {
            AssertCode("@@@", "5D2E");
        }

        // ................:::.:101001:.:..
        // Reko: a decoder for V850 instruction 34ED at address 0010106E has not been implemented.
        [Test]
        public void V850Dis_34ED()
        {
            AssertCode("@@@", "34ED");
        }

        // ................:..:.100011.:...
        // ................:..:.:...::.:... sld_h
        // ................:.:.:000001::...
        // ................:.:.:.....:::... not
        // .....................011101..:.:
        // ......................:::.:..:.: sst_b
        // ................::..:010110:...:
        // ....................:000000::::.
        // ................00001......::::.
        // ....................:......::::. mov
        // .....................100001...::
        // .....................:....:...:: sld_h
        // ................:::::010001..:::
        // Reko: a decoder for V850 instruction 27FA at address 0010107C has not been implemented.
        [Test]
        public void V850Dis_27FA()
        {
            AssertCode("@@@", "27FA");
        }

        // .................::.:000100.:...
        // Reko: a decoder for V850 instruction 8868 at address 0010107E has not been implemented.
        [Test]
        public void V850Dis_8868()
        {
            AssertCode("@@@", "8868");
        }

        // ................:.:::110000...:.
        // Reko: a decoder for V850 instruction 02BE at address 00101080 has not been implemented.
        [Test]
        public void V850Dis_02BE()
        {
            AssertCode("@@@", "02BE");
        }

        // ..................:..000111.::.:
        // Reko: a decoder for V850 instruction ED20 at address 00101082 has not been implemented.
        [Test]
        public void V850Dis_ED20()
        {
            AssertCode("@@@", "ED20");
        }

        // ..................:.:100001..:::
        // ..................:.::....:..::: sld_h
        // .................:::.111011:.:::
        // Reko: a decoder for V850 instruction 7777 at address 00101086 has not been implemented.
        [Test]
        public void V850Dis_7777()
        {
            AssertCode("@@@", "7777");
        }

        // ................:.:::110011...::
        // Reko: a decoder for V850 instruction 63BE at address 00101088 has not been implemented.
        [Test]
        public void V850Dis_63BE()
        {
            AssertCode("@@@", "63BE");
        }

        // ................:.:.:011101:...:
        // ................:.:.:.:::.::...: sst_b
        // .................::.:010100.:...
        // ................:::..100110:.:.:
        // ................:::..:..::.:.:.: sst_h
        // ..................:..101110:.::.
        // Reko: a decoder for V850 instruction D625 at address 00101090 has not been implemented.
        [Test]
        public void V850Dis_D625()
        {
            AssertCode("@@@", "D625");
        }

        // .................::.:101001.....
        // Reko: a decoder for V850 instruction 206D at address 00101092 has not been implemented.
        [Test]
        public void V850Dis_206D()
        {
            AssertCode("@@@", "206D");
        }

        // ....................:011011.:...
        // ....................:.::.::.:... sld_b
        // ................:..::000110..:::
        // Reko: a decoder for V850 instruction C798 at address 00101096 has not been implemented.
        [Test]
        public void V850Dis_C798()
        {
            AssertCode("@@@", "C798");
        }

        // .....................000110:.:..
        // Reko: a decoder for V850 instruction D400 at address 00101098 has not been implemented.
        [Test]
        public void V850Dis_D400()
        {
            AssertCode("@@@", "D400");
        }

        // ..................::.001001.:::.
        // Reko: a decoder for V850 instruction 2E31 at address 0010109A has not been implemented.
        [Test]
        public void V850Dis_2E31()
        {
            AssertCode("@@@", "2E31");
        }

        // ................:::.:010111::..:
        // Reko: a decoder for V850 instruction F9EA at address 0010109C has not been implemented.
        [Test]
        public void V850Dis_F9EA()
        {
            AssertCode("@@@", "F9EA");
        }

        // ................:..:.011000...:.
        // ................:..:..::......:. sld_b
        // ................::..:110101:....
        // Reko: a decoder for V850 instruction B0CE at address 001010A0 has not been implemented.
        [Test]
        public void V850Dis_B0CE()
        {
            AssertCode("@@@", "B0CE");
        }

        // ................:..::111011:.:..
        // Reko: a decoder for V850 instruction 749F at address 001010A2 has not been implemented.
        [Test]
        public void V850Dis_749F()
        {
            AssertCode("@@@", "749F");
        }

        // ...................:.011010....:
        // ...................:..::.:.....: sld_b
        // ..................:::100110..::.
        // ..................::::..::...::. sst_h
        // ................::...000000:::..
        // ................11000......:::..
        // ................::.........:::.. mov
        // ................:::..111010..:.:
        // Reko: a decoder for V850 instruction 45E7 at address 001010AA has not been implemented.
        [Test]
        public void V850Dis_45E7()
        {
            AssertCode("@@@", "45E7");
        }

        // ..................:..110010:..:.
        // Reko: a decoder for V850 instruction 5226 at address 001010AC has not been implemented.
        [Test]
        public void V850Dis_5226()
        {
            AssertCode("@@@", "5226");
        }

        // ................:.::.011010...:.
        // ................:.::..::.:....:. sld_b
        // ..................:::111001:..:.
        // Reko: a decoder for V850 instruction 323F at address 001010B0 has not been implemented.
        [Test]
        public void V850Dis_323F()
        {
            AssertCode("@@@", "323F");
        }

        // .....................001001.:::.
        // Reko: a decoder for V850 instruction 2E01 at address 001010B2 has not been implemented.
        [Test]
        public void V850Dis_2E01()
        {
            AssertCode("@@@", "2E01");
        }

        // ...................::010110::.:.
        // Reko: a decoder for V850 instruction DA1A at address 001010B4 has not been implemented.
        [Test]
        public void V850Dis_DA1A()
        {
            AssertCode("@@@", "DA1A");
        }

        // ................::::.110111:::::
        // Reko: a decoder for V850 instruction FFF6 at address 001010B6 has not been implemented.
        [Test]
        public void V850Dis_FFF6()
        {
            AssertCode("@@@", "FFF6");
        }

        // .................:.:.101111:.::.
        // Reko: a decoder for V850 instruction F655 at address 001010B8 has not been implemented.
        [Test]
        public void V850Dis_F655()
        {
            AssertCode("@@@", "F655");
        }

        // ..................:..110010.:.::
        // Reko: a decoder for V850 instruction 4B26 at address 001010BA has not been implemented.
        [Test]
        public void V850Dis_4B26()
        {
            AssertCode("@@@", "4B26");
        }

        // .................:...110111:::.:
        // Reko: a decoder for V850 instruction FD46 at address 001010BC has not been implemented.
        [Test]
        public void V850Dis_FD46()
        {
            AssertCode("@@@", "FD46");
        }

        // .................::::000011::::.
        // Reko: a decoder for V850 instruction 7E78 at address 001010BE has not been implemented.
        [Test]
        public void V850Dis_7E78()
        {
            AssertCode("@@@", "7E78");
        }

        // ...................::110101::.::
        // Reko: a decoder for V850 instruction BB1E at address 001010C0 has not been implemented.
        [Test]
        public void V850Dis_BB1E()
        {
            AssertCode("@@@", "BB1E");
        }

        // ................:.:::001111::.:.
        // Reko: a decoder for V850 instruction FAB9 at address 001010C2 has not been implemented.
        [Test]
        public void V850Dis_FAB9()
        {
            AssertCode("@@@", "FAB9");
        }

        // ..................::.110111:.::.
        // Reko: a decoder for V850 instruction F636 at address 001010C4 has not been implemented.
        [Test]
        public void V850Dis_F636()
        {
            AssertCode("@@@", "F636");
        }

        // ................::...010101...:.
        // Reko: a decoder for V850 instruction A2C2 at address 001010C6 has not been implemented.
        [Test]
        public void V850Dis_A2C2()
        {
            AssertCode("@@@", "A2C2");
        }

        // ................:::..011010:::::
        // ................:::...::.:.::::: sld_b
        // ................:.:.:000010:...:
        // ................10101....:.:...:
        // ................:.:.:....:.10001
        // ................:.:.:....:.:...: divh
        // ................:::..000110..::.
        // Reko: a decoder for V850 instruction C6E0 at address 001010CC has not been implemented.
        [Test]
        public void V850Dis_C6E0()
        {
            AssertCode("@@@", "C6E0");
        }

        // ................:.:.:110010..::.
        // Reko: a decoder for V850 instruction 46AE at address 001010CE has not been implemented.
        [Test]
        public void V850Dis_46AE()
        {
            AssertCode("@@@", "46AE");
        }

        // .................::::010101::::.
        // Reko: a decoder for V850 instruction BE7A at address 001010D0 has not been implemented.
        [Test]
        public void V850Dis_BE7A()
        {
            AssertCode("@@@", "BE7A");
        }

        // ................:.:::010111::::.
        // Reko: a decoder for V850 instruction FEBA at address 001010D2 has not been implemented.
        [Test]
        public void V850Dis_FEBA()
        {
            AssertCode("@@@", "FEBA");
        }

        // .....................001101:::..
        // Reko: a decoder for V850 instruction BC01 at address 001010D4 has not been implemented.
        [Test]
        public void V850Dis_BC01()
        {
            AssertCode("@@@", "BC01");
        }

        // ................:..:.110101::...
        // Reko: a decoder for V850 instruction B896 at address 001010D6 has not been implemented.
        [Test]
        public void V850Dis_B896()
        {
            AssertCode("@@@", "B896");
        }

        // .................::::101110.::::
        // Reko: a decoder for V850 instruction CF7D at address 001010D8 has not been implemented.
        [Test]
        public void V850Dis_CF7D()
        {
            AssertCode("@@@", "CF7D");
        }

        // ................:::.:011101.:...
        // ................:::.:.:::.:.:... sst_b
        // ................:..::111110.:...
        // Reko: a decoder for V850 instruction C89F at address 001010DC has not been implemented.
        [Test]
        public void V850Dis_C89F()
        {
            AssertCode("@@@", "C89F");
        }

        // ................::.:.101001::::.
        // Reko: a decoder for V850 instruction 3ED5 at address 001010DE has not been implemented.
        [Test]
        public void V850Dis_3ED5()
        {
            AssertCode("@@@", "3ED5");
        }

        // ................::..:100010....:
        // ................::..::...:.....: sld_h
        // ..................::.100001.:.::
        // ..................::.:....:.:.:: sld_h
        // ................:...:000001.::..
        // ................:...:.....:.::.. not
        // ................:::::011101:::.:
        // ................:::::.:::.::::.: sst_b
        // .................::.:011100:::..
        // .................::.:.:::..:::.. sst_b
        // ................::::.110101....:
        // Reko: a decoder for V850 instruction A1F6 at address 001010EA has not been implemented.
        [Test]
        public void V850Dis_A1F6()
        {
            AssertCode("@@@", "A1F6");
        }

        // .................:.:.111110.:..:
        // Reko: a decoder for V850 instruction C957 at address 001010EC has not been implemented.
        [Test]
        public void V850Dis_C957()
        {
            AssertCode("@@@", "C957");
        }

        // ...................:.110001:....
        // Reko: a decoder for V850 instruction 3016 at address 001010EE has not been implemented.
        [Test]
        public void V850Dis_3016()
        {
            AssertCode("@@@", "3016");
        }

        // ................:..::101000.::::
        // ................:..:::.:....:::1 Sld.w/Sst.w
        // ................:..:::.:....:::: sst_w
        // ................:....011011.:...
        // ................:.....::.::.:... sld_b
        // ................::...100010..:::
        // ................::...:...:...::: sld_h
        // ................:.:::010101::.::
        // Reko: a decoder for V850 instruction BBBA at address 001010F6 has not been implemented.
        [Test]
        public void V850Dis_BBBA()
        {
            AssertCode("@@@", "BBBA");
        }

        // .................::..100011..:.:
        // .................::..:...::..:.: sld_h
        // .................::::000111.:.:.
        // Reko: a decoder for V850 instruction EA78 at address 001010FA has not been implemented.
        [Test]
        public void V850Dis_EA78()
        {
            AssertCode("@@@", "EA78");
        }

        // ................:.::.111100.:::.
        // Reko: a decoder for V850 instruction 8EB7 at address 001010FC has not been implemented.
        [Test]
        public void V850Dis_8EB7()
        {
            AssertCode("@@@", "8EB7");
        }

        // ................::...000111::.:.
        // Reko: a decoder for V850 instruction FAC0 at address 001010FE has not been implemented.
        [Test]
        public void V850Dis_FAC0()
        {
            AssertCode("@@@", "FAC0");
        }

        // .................::::011101..:.:
        // .................::::.:::.:..:.: sst_b
        // ................:::::001011..::.
        // Reko: a decoder for V850 instruction 66F9 at address 00101102 has not been implemented.
        [Test]
        public void V850Dis_66F9()
        {
            AssertCode("@@@", "66F9");
        }

        // ...................:.010101:.:..
        // Reko: a decoder for V850 instruction B412 at address 00101104 has not been implemented.
        [Test]
        public void V850Dis_B412()
        {
            AssertCode("@@@", "B412");
        }

        // ..................::.101100....:
        // Reko: a decoder for V850 instruction 8135 at address 00101106 has not been implemented.
        [Test]
        public void V850Dis_8135()
        {
            AssertCode("@@@", "8135");
        }

        // .....................011101:::..
        // ......................:::.::::.. sst_b
        // .................:.:.000000::::.
        // ................01010......::::.
        // .................:.:.......::::. mov
        // ................::...010010:::::
        // Reko: a decoder for V850 instruction 5FC2 at address 0010110C has not been implemented.
        [Test]
        public void V850Dis_5FC2()
        {
            AssertCode("@@@", "5FC2");
        }

        // .................::::000110.::..
        // Reko: a decoder for V850 instruction CC78 at address 0010110E has not been implemented.
        [Test]
        public void V850Dis_CC78()
        {
            AssertCode("@@@", "CC78");
        }

        // ................:..:.010010.::.:
        // Reko: a decoder for V850 instruction 4D92 at address 00101110 has not been implemented.
        [Test]
        public void V850Dis_4D92()
        {
            AssertCode("@@@", "4D92");
        }

        // ..................:.:010110:::..
        // Reko: a decoder for V850 instruction DC2A at address 00101112 has not been implemented.
        [Test]
        public void V850Dis_DC2A()
        {
            AssertCode("@@@", "DC2A");
        }

        // ................:.:::010001:.::.
        // Reko: a decoder for V850 instruction 36BA at address 00101114 has not been implemented.
        [Test]
        public void V850Dis_36BA()
        {
            AssertCode("@@@", "36BA");
        }

        // .................::::100100.::.:
        // .................:::::..:...::.: sst_h
        // .................:::.001000:...:
        // .................:::...:...:...: or
        // ................::..:000110.:::.
        // Reko: a decoder for V850 instruction CEC8 at address 0010111A has not been implemented.
        [Test]
        public void V850Dis_CEC8()
        {
            AssertCode("@@@", "CEC8");
        }

        // .................:...101100::::.
        // Reko: a decoder for V850 instruction 9E45 at address 0010111C has not been implemented.
        [Test]
        public void V850Dis_9E45()
        {
            AssertCode("@@@", "9E45");
        }

        // ................:::..100011.::.:
        // ................:::..:...::.::.: sld_h
        // ................:::..001011:....
        // Reko: a decoder for V850 instruction 70E1 at address 00101120 has not been implemented.
        [Test]
        public void V850Dis_70E1()
        {
            AssertCode("@@@", "70E1");
        }

        // .................:..:101010:....
        // Reko: a decoder for V850 instruction 504D at address 00101122 has not been implemented.
        [Test]
        public void V850Dis_504D()
        {
            AssertCode("@@@", "504D");
        }

        // ..................::.110111:....
        // Reko: a decoder for V850 instruction F036 at address 00101124 has not been implemented.
        [Test]
        public void V850Dis_F036()
        {
            AssertCode("@@@", "F036");
        }

        // ................:::::101011....:
        // Reko: a decoder for V850 instruction 61FD at address 00101126 has not been implemented.
        [Test]
        public void V850Dis_61FD()
        {
            AssertCode("@@@", "61FD");
        }

        // ..................::.011000::.::
        // ..................::..::...::.:: sld_b
        // .................:...100110..::.
        // .................:...:..::...::. sst_h
        // .................::.:010101..::.
        // Reko: a decoder for V850 instruction A66A at address 0010112C has not been implemented.
        [Test]
        public void V850Dis_A66A()
        {
            AssertCode("@@@", "A66A");
        }

        // ................:.:..010010::.::
        // Reko: a decoder for V850 instruction 5BA2 at address 0010112E has not been implemented.
        [Test]
        public void V850Dis_5BA2()
        {
            AssertCode("@@@", "5BA2");
        }

        // .................:::.010100:.:::
        // Reko: a decoder for V850 instruction 9772 at address 00101130 has not been implemented.
        [Test]
        public void V850Dis_9772()
        {
            AssertCode("@@@", "9772");
        }

        // ................:::::100001.:.:.
        // ................::::::....:.:.:. sld_h
        // ................::.:.000110::.:.
        // Reko: a decoder for V850 instruction DAD0 at address 00101134 has not been implemented.
        [Test]
        public void V850Dis_DAD0()
        {
            AssertCode("@@@", "DAD0");
        }

        // ................:::.:010100.:..:
        // Reko: a decoder for V850 instruction 89EA at address 00101136 has not been implemented.
        [Test]
        public void V850Dis_89EA()
        {
            AssertCode("@@@", "89EA");
        }

        // ................:::.:010111.::.:
        // Reko: a decoder for V850 instruction EDEA at address 00101138 has not been implemented.
        [Test]
        public void V850Dis_EDEA()
        {
            AssertCode("@@@", "EDEA");
        }

        // .................:...011111.:...
        // .................:....:::::.:... sst_b
        // ................:.:::001010...::
        // Reko: a decoder for V850 instruction 43B9 at address 0010113C has not been implemented.
        [Test]
        public void V850Dis_43B9()
        {
            AssertCode("@@@", "43B9");
        }

        // .................:::.111100:.::.
        // Reko: a decoder for V850 instruction 9677 at address 0010113E has not been implemented.
        [Test]
        public void V850Dis_9677()
        {
            AssertCode("@@@", "9677");
        }

        // ..................:::001100:.::.
        // Reko: a decoder for V850 instruction 9639 at address 00101140 has not been implemented.
        [Test]
        public void V850Dis_9639()
        {
            AssertCode("@@@", "9639");
        }

        // .................::::101110:::::
        // Reko: a decoder for V850 instruction DF7D at address 00101142 has not been implemented.
        [Test]
        public void V850Dis_DF7D()
        {
            AssertCode("@@@", "DF7D");
        }

        // .................::::111011:...:
        // Reko: a decoder for V850 instruction 717F at address 00101144 has not been implemented.
        [Test]
        public void V850Dis_717F()
        {
            AssertCode("@@@", "717F");
        }

        // ...................:.111100..:::
        // Reko: a decoder for V850 instruction 8717 at address 00101146 has not been implemented.
        [Test]
        public void V850Dis_8717()
        {
            AssertCode("@@@", "8717");
        }

        // ................::.::111001:...:
        // Reko: a decoder for V850 instruction 31DF at address 00101148 has not been implemented.
        [Test]
        public void V850Dis_31DF()
        {
            AssertCode("@@@", "31DF");
        }

        // .....................011000.:::.
        // ......................::....:::. sld_b
        // .................:..:001111:.:..
        // Reko: a decoder for V850 instruction F449 at address 0010114C has not been implemented.
        [Test]
        public void V850Dis_F449()
        {
            AssertCode("@@@", "F449");
        }

        // ................:.:..110110:.::.
        // Reko: a decoder for V850 instruction D6A6 at address 0010114E has not been implemented.
        [Test]
        public void V850Dis_D6A6()
        {
            AssertCode("@@@", "D6A6");
        }

        // ..................:.:100000:.:.:
        // ..................:.::.....:.:.: sld_h
        // ..................:.:100001:....
        // ..................:.::....::.... sld_h
        // .................:::.001000::.:.
        // .................:::...:...::.:. or
        // ................:.:..000000.:.:.
        // ................10100.......:.:.
        // ................:.:.........:.:. mov
        // .................::..110010:.:::
        // Reko: a decoder for V850 instruction 5766 at address 00101158 has not been implemented.
        [Test]
        public void V850Dis_5766()
        {
            AssertCode("@@@", "5766");
        }

        // .....................110010:::::
        // Reko: a decoder for V850 instruction 5F06 at address 0010115A has not been implemented.
        [Test]
        public void V850Dis_5F06()
        {
            AssertCode("@@@", "5F06");
        }

        // ...................::101010.::.:
        // Reko: a decoder for V850 instruction 4D1D at address 0010115C has not been implemented.
        [Test]
        public void V850Dis_4D1D()
        {
            AssertCode("@@@", "4D1D");
        }

        // .................::..010101::..:
        // Reko: a decoder for V850 instruction B962 at address 0010115E has not been implemented.
        [Test]
        public void V850Dis_B962()
        {
            AssertCode("@@@", "B962");
        }

        // .................:..:110100:::..
        // Reko: a decoder for V850 instruction 9C4E at address 00101160 has not been implemented.
        [Test]
        public void V850Dis_9C4E()
        {
            AssertCode("@@@", "9C4E");
        }

        // .................::.:101101.:::.
        // Reko: a decoder for V850 instruction AE6D at address 00101162 has not been implemented.
        [Test]
        public void V850Dis_AE6D()
        {
            AssertCode("@@@", "AE6D");
        }

        // ................::.::101100.:.::
        // Reko: a decoder for V850 instruction 8BDD at address 00101164 has not been implemented.
        [Test]
        public void V850Dis_8BDD()
        {
            AssertCode("@@@", "8BDD");
        }

        // ...................::000001:...:
        // ...................::.....::...: not
        // ...................:.011111.:.:.
        // ...................:..:::::.:.:. sst_b
        // ................:::::010110.:.:.
        // Reko: a decoder for V850 instruction CAFA at address 0010116A has not been implemented.
        [Test]
        public void V850Dis_CAFA()
        {
            AssertCode("@@@", "CAFA");
        }

        // ................:....110110....:
        // Reko: a decoder for V850 instruction C186 at address 0010116C has not been implemented.
        [Test]
        public void V850Dis_C186()
        {
            AssertCode("@@@", "C186");
        }

        // ................:.:..100111:.::.
        // ................:.:..:..::::.::. sst_h
        // .................:...010000:..::
        // .................:....:....10011
        // .................:....:....:..:: mov
        // ...................:.111001.::.:
        // Reko: a decoder for V850 instruction 2D17 at address 00101172 has not been implemented.
        [Test]
        public void V850Dis_2D17()
        {
            AssertCode("@@@", "2D17");
        }

        // .................::.:010100.:...
        // ..................:..101010:::..
        // Reko: a decoder for V850 instruction 5C25 at address 00101176 has not been implemented.
        [Test]
        public void V850Dis_5C25()
        {
            AssertCode("@@@", "5C25");
        }

        // ..................:::111001.:::.
        // Reko: a decoder for V850 instruction 2E3F at address 00101178 has not been implemented.
        [Test]
        public void V850Dis_2E3F()
        {
            AssertCode("@@@", "2E3F");
        }

        // ....................:011001..::.
        // ....................:.::..:..::. sld_b
        // ................:....001000:::::
        // ................:......:...::::: or
        // ...................:.011110.:..:
        // ...................:..::::..:..: sst_b
        // ................:.:.:011001..:.:
        // ................:.:.:.::..:..:.: sld_b
        // ................:.:..101011..:::
        // Reko: a decoder for V850 instruction 67A5 at address 00101182 has not been implemented.
        [Test]
        public void V850Dis_67A5()
        {
            AssertCode("@@@", "67A5");
        }

        // .................::.:000100::.::
        // Reko: a decoder for V850 instruction 9B68 at address 00101184 has not been implemented.
        [Test]
        public void V850Dis_9B68()
        {
            AssertCode("@@@", "9B68");
        }

        // ..................:::000000::.::
        // ................00111......::.::
        // ..................:::......::.:: mov
        // ...................::000111::.:.
        // Reko: a decoder for V850 instruction FA18 at address 00101188 has not been implemented.
        [Test]
        public void V850Dis_FA18()
        {
            AssertCode("@@@", "FA18");
        }

        // .....................111010....:
        // Reko: a decoder for V850 instruction 4107 at address 0010118A has not been implemented.
        [Test]
        public void V850Dis_4107()
        {
            AssertCode("@@@", "4107");
        }

        // ....................:111101..::.
        // Reko: a decoder for V850 instruction A60F at address 0010118C has not been implemented.
        [Test]
        public void V850Dis_A60F()
        {
            AssertCode("@@@", "A60F");
        }

        // ................::.:.001110:::::
        // Reko: a decoder for V850 instruction DFD1 at address 0010118E has not been implemented.
        [Test]
        public void V850Dis_DFD1()
        {
            AssertCode("@@@", "DFD1");
        }

        // ................:.:::011011.::::
        // ................:.:::.::.::.:::: sld_b
        // ....................:011111:..:.
        // ....................:.::::::..:. sst_b
        // ................:.:..010111.::::
        // Reko: a decoder for V850 instruction EFA2 at address 00101194 has not been implemented.
        [Test]
        public void V850Dis_EFA2()
        {
            AssertCode("@@@", "EFA2");
        }

        // ..................::.001110::.:.
        // Reko: a decoder for V850 instruction DA31 at address 00101196 has not been implemented.
        [Test]
        public void V850Dis_DA31()
        {
            AssertCode("@@@", "DA31");
        }

        // ................:...:001111:...:
        // Reko: a decoder for V850 instruction F189 at address 00101198 has not been implemented.
        [Test]
        public void V850Dis_F189()
        {
            AssertCode("@@@", "F189");
        }

        // .................:..:011001::::.
        // .................:..:.::..:::::. sld_b
        // ..................:..110101:.:::
        // Reko: a decoder for V850 instruction B726 at address 0010119C has not been implemented.
        [Test]
        public void V850Dis_B726()
        {
            AssertCode("@@@", "B726");
        }

        // .................::.:000111::..:
        // Reko: a decoder for V850 instruction F968 at address 0010119E has not been implemented.
        [Test]
        public void V850Dis_F968()
        {
            AssertCode("@@@", "F968");
        }

        // ................:.::.111010.:.:.
        // Reko: a decoder for V850 instruction 4AB7 at address 001011A0 has not been implemented.
        [Test]
        public void V850Dis_4AB7()
        {
            AssertCode("@@@", "4AB7");
        }

        // ...................:.101110:.:.:
        // Reko: a decoder for V850 instruction D515 at address 001011A2 has not been implemented.
        [Test]
        public void V850Dis_D515()
        {
            AssertCode("@@@", "D515");
        }

        // .................:.:.000000::::.
        // ................01010......::::.
        // .................:.:.......::::. mov
        // ................:.:.:010000.:...
        // ................:.:.:.:....01000
        // ................:.:.:.:.....:... mov
        // ................::::.101001..::.
        // Reko: a decoder for V850 instruction 26F5 at address 001011A8 has not been implemented.
        [Test]
        public void V850Dis_26F5()
        {
            AssertCode("@@@", "26F5");
        }

        // .................:::.000000:::..
        // ................01110......:::..
        // .................:::.......:::.. mov
        // .................:.:.101001.:...
        // Reko: a decoder for V850 instruction 2855 at address 001011AC has not been implemented.
        [Test]
        public void V850Dis_2855()
        {
            AssertCode("@@@", "2855");
        }

        // ...................::111011::::.
        // Reko: a decoder for V850 instruction 7E1F at address 001011AE has not been implemented.
        [Test]
        public void V850Dis_7E1F()
        {
            AssertCode("@@@", "7E1F");
        }

        // .................:.:.100010:::::
        // .................:.:.:...:.::::: sld_h
        // .................::..011100::.::
        // .................::...:::..::.:: sst_b
        // ................:.:::101100.::::
        // Reko: a decoder for V850 instruction 8FBD at address 001011B4 has not been implemented.
        [Test]
        public void V850Dis_8FBD()
        {
            AssertCode("@@@", "8FBD");
        }

        // ................:....010001...::
        // Reko: a decoder for V850 instruction 2382 at address 001011B6 has not been implemented.
        [Test]
        public void V850Dis_2382()
        {
            AssertCode("@@@", "2382");
        }

        // ..................::.110110:...:
        // Reko: a decoder for V850 instruction D136 at address 001011B8 has not been implemented.
        [Test]
        public void V850Dis_D136()
        {
            AssertCode("@@@", "D136");
        }

        // ...................:.010101:..::
        // Reko: a decoder for V850 instruction B312 at address 001011BA has not been implemented.
        [Test]
        public void V850Dis_B312()
        {
            AssertCode("@@@", "B312");
        }

        // ................::..:111001.:.:.
        // Reko: a decoder for V850 instruction 2ACF at address 001011BC has not been implemented.
        [Test]
        public void V850Dis_2ACF()
        {
            AssertCode("@@@", "2ACF");
        }

        // ................::.:.100100....:
        // ................::.:.:..:......: sst_h
        // ................::.::001000:::::
        // ................::.::..:...::::: or
        // ................:::.:110101:::.:
        // Reko: a decoder for V850 instruction BDEE at address 001011C2 has not been implemented.
        [Test]
        public void V850Dis_BDEE()
        {
            AssertCode("@@@", "BDEE");
        }

        // ................::..:001001::.:.
        // Reko: a decoder for V850 instruction 3AC9 at address 001011C4 has not been implemented.
        [Test]
        public void V850Dis_3AC9()
        {
            AssertCode("@@@", "3AC9");
        }

        // ...................::110101:..:.
        // Reko: a decoder for V850 instruction B21E at address 001011C6 has not been implemented.
        [Test]
        public void V850Dis_B21E()
        {
            AssertCode("@@@", "B21E");
        }

        // .................:...111101:..:.
        // Reko: a decoder for V850 instruction B247 at address 001011C8 has not been implemented.
        [Test]
        public void V850Dis_B247()
        {
            AssertCode("@@@", "B247");
        }

        // ................:::::010111..:..
        // Reko: a decoder for V850 instruction E4FA at address 001011CA has not been implemented.
        [Test]
        public void V850Dis_E4FA()
        {
            AssertCode("@@@", "E4FA");
        }

        // .................::.:010100..::.
        // Reko: a decoder for V850 instruction 866A at address 001011CC has not been implemented.
        [Test]
        public void V850Dis_866A()
        {
            AssertCode("@@@", "866A");
        }

        // ................:..::011000.:...
        // ................:..::.::....:... sld_b
        // ...................::111000.:.:.
        // Reko: a decoder for V850 instruction 0A1F at address 001011D0 has not been implemented.
        [Test]
        public void V850Dis_0A1F()
        {
            AssertCode("@@@", "0A1F");
        }

        // .................::..010111.::..
        // Reko: a decoder for V850 instruction EC62 at address 001011D2 has not been implemented.
        [Test]
        public void V850Dis_EC62()
        {
            AssertCode("@@@", "EC62");
        }

        // ................:..:.101100.:.:.
        // Reko: a decoder for V850 instruction 8A95 at address 001011D4 has not been implemented.
        [Test]
        public void V850Dis_8A95()
        {
            AssertCode("@@@", "8A95");
        }

        // ................:...:100110::...
        // ................:...::..::.::... sst_h
        // .................:.:.010100:..::
        // Reko: a decoder for V850 instruction 9352 at address 001011D8 has not been implemented.
        [Test]
        public void V850Dis_9352()
        {
            AssertCode("@@@", "9352");
        }

        // ................:::.:000010...::
        // ................11101....:....::
        // ................:::.:....:.00011
        // ................:::.:....:....:: divh
        // ................::::.110100:::..
        // Reko: a decoder for V850 instruction 9CF6 at address 001011DC has not been implemented.
        [Test]
        public void V850Dis_9CF6()
        {
            AssertCode("@@@", "9CF6");
        }

        // .................:.:.111100.....
        // Reko: a decoder for V850 instruction 8057 at address 001011DE has not been implemented.
        [Test]
        public void V850Dis_8057()
        {
            AssertCode("@@@", "8057");
        }

        // .................:...111011:..::
        // Reko: a decoder for V850 instruction 7347 at address 001011E0 has not been implemented.
        [Test]
        public void V850Dis_7347()
        {
            AssertCode("@@@", "7347");
        }

        // .................:..:111010:...:
        // Reko: a decoder for V850 instruction 514F at address 001011E2 has not been implemented.
        [Test]
        public void V850Dis_514F()
        {
            AssertCode("@@@", "514F");
        }

        // ................:..:.000100::...
        // Reko: a decoder for V850 instruction 9890 at address 001011E4 has not been implemented.
        [Test]
        public void V850Dis_9890()
        {
            AssertCode("@@@", "9890");
        }

        // ................:..:.101111..:..
        // Reko: a decoder for V850 instruction E495 at address 001011E6 has not been implemented.
        [Test]
        public void V850Dis_E495()
        {
            AssertCode("@@@", "E495");
        }

        // .................::::100101...:.
        // .................:::::..:.:...:. sst_h
        // .................:..:000101:...:
        // Reko: a decoder for V850 instruction B148 at address 001011EA has not been implemented.
        [Test]
        public void V850Dis_B148()
        {
            AssertCode("@@@", "B148");
        }

        // ..................:.:001110::...
        // Reko: a decoder for V850 instruction D829 at address 001011EC has not been implemented.
        [Test]
        public void V850Dis_D829()
        {
            AssertCode("@@@", "D829");
        }

        // .................::..111111:::..
        // Reko: a decoder for V850 instruction FC67 at address 001011EE has not been implemented.
        [Test]
        public void V850Dis_FC67()
        {
            AssertCode("@@@", "FC67");
        }

        // ...................:.111001.::.:
        // ....................:010000.:...
        // ....................:.:....01000
        // ....................:.:.....:... mov
        // ...................::111111...::
        // Reko: a decoder for V850 instruction E31F at address 001011F4 has not been implemented.
        [Test]
        public void V850Dis_E31F()
        {
            AssertCode("@@@", "E31F");
        }

        // ................:...:001010.:.:.
        // Reko: a decoder for V850 instruction 4A89 at address 001011F6 has not been implemented.
        [Test]
        public void V850Dis_4A89()
        {
            AssertCode("@@@", "4A89");
        }

        // ..................::.000000.....
        // ................00110...........
        // ..................::............ mov
        // ................:::..111010::.::
        // Reko: a decoder for V850 instruction 5BE7 at address 001011FA has not been implemented.
        [Test]
        public void V850Dis_5BE7()
        {
            AssertCode("@@@", "5BE7");
        }

        // ................::..:001100..::.
        // Reko: a decoder for V850 instruction 86C9 at address 001011FC has not been implemented.
        [Test]
        public void V850Dis_86C9()
        {
            AssertCode("@@@", "86C9");
        }

        // ................:.:::100100...:.
        // ................:.::::..:.....:. sst_h
        // ................:...:101110..::.
        // Reko: a decoder for V850 instruction C68D at address 00101200 has not been implemented.
        [Test]
        public void V850Dis_C68D()
        {
            AssertCode("@@@", "C68D");
        }

        // ..................:::110100..::.
        // Reko: a decoder for V850 instruction 863E at address 00101202 has not been implemented.
        [Test]
        public void V850Dis_863E()
        {
            AssertCode("@@@", "863E");
        }

        // ................:.:::000000::...
        // ................10111......::...
        // ................:.:::......::... mov
        // .................::::010000...::
        // .................::::.:....00011
        // .................::::.:.......:: mov
        // .................:.:.110001::.::
        // Reko: a decoder for V850 instruction 3B56 at address 00101208 has not been implemented.
        [Test]
        public void V850Dis_3B56()
        {
            AssertCode("@@@", "3B56");
        }

        // ....................:101101:::::
        // Reko: a decoder for V850 instruction BF0D at address 0010120A has not been implemented.
        [Test]
        public void V850Dis_BF0D()
        {
            AssertCode("@@@", "BF0D");
        }

        // ................:..:.000111..:..
        // Reko: a decoder for V850 instruction E490 at address 0010120C has not been implemented.
        [Test]
        public void V850Dis_E490()
        {
            AssertCode("@@@", "E490");
        }

        // .................::.:010101:.:..
        // Reko: a decoder for V850 instruction B46A at address 0010120E has not been implemented.
        [Test]
        public void V850Dis_B46A()
        {
            AssertCode("@@@", "B46A");
        }

        // ................::.::100010:.::.
        // ................::.:::...:.:.::. sld_h
        // ................:....100101::.:.
        // ................:....:..:.:::.:. sst_h
        // .................:::.110111.:::.
        // Reko: a decoder for V850 instruction EE76 at address 00101214 has not been implemented.
        [Test]
        public void V850Dis_EE76()
        {
            AssertCode("@@@", "EE76");
        }

        // .................:.:.100011:.:..
        // .................:.:.:...:::.:.. sld_h
        // ................::...111111:::.:
        // .................::.:000010.::::
        // ................01101....:..::::
        // .................::.:....:.01111
        // .................::.:....:..:::: divh
        // ................:::::101001:.:..
        // Reko: a decoder for V850 instruction 34FD at address 0010121C has not been implemented.
        [Test]
        public void V850Dis_34FD()
        {
            AssertCode("@@@", "34FD");
        }

        // ................::.::000000.::::
        // ................11011.......::::
        // ................::.::.......:::: mov
        // ................::::.100111::.:.
        // ................::::.:..:::::.:. sst_h
        // .................:..:010100....:
        // Reko: a decoder for V850 instruction 814A at address 00101222 has not been implemented.
        [Test]
        public void V850Dis_814A()
        {
            AssertCode("@@@", "814A");
        }

        // ................:.:::101101...:.
        // Reko: a decoder for V850 instruction A2BD at address 00101224 has not been implemented.
        [Test]
        public void V850Dis_A2BD()
        {
            AssertCode("@@@", "A2BD");
        }

        // .................:::.101010.:::.
        // Reko: a decoder for V850 instruction 4E75 at address 00101226 has not been implemented.
        [Test]
        public void V850Dis_4E75()
        {
            AssertCode("@@@", "4E75");
        }

        // ................:...:101000:.:..
        // ................:...::.:...:.:.0 Sld.w/Sst.w
        // ................:...::.:...:.:.. sld_w
        // ................:...:100110.::::
        // ................:...::..::..:::: sst_h
        // ..................:::100010:::::
        // ..................::::...:.::::: sld_h
        // .................::.:111101..:.:
        // Reko: a decoder for V850 instruction A56F at address 0010122E has not been implemented.
        [Test]
        public void V850Dis_A56F()
        {
            AssertCode("@@@", "A56F");
        }

        // ................::::.001011:::..
        // Reko: a decoder for V850 instruction 7CF1 at address 00101230 has not been implemented.
        [Test]
        public void V850Dis_7CF1()
        {
            AssertCode("@@@", "7CF1");
        }

        // ................:.:.:010100:...:
        // Reko: a decoder for V850 instruction 91AA at address 00101232 has not been implemented.
        [Test]
        public void V850Dis_91AA()
        {
            AssertCode("@@@", "91AA");
        }

        // ................:::::101000.:::.
        // ................::::::.:....:::0 Sld.w/Sst.w
        // ................::::::.:....:::. sld_w
        // .................:.:.000110.::.:
        // Reko: a decoder for V850 instruction CD50 at address 00101236 has not been implemented.
        [Test]
        public void V850Dis_CD50()
        {
            AssertCode("@@@", "CD50");
        }

        // ..................::.010000::..:
        // ..................::..:....11001
        // ..................::..:....::..: mov
        // .................:.:.001010....:
        // Reko: a decoder for V850 instruction 4151 at address 0010123A has not been implemented.
        [Test]
        public void V850Dis_4151()
        {
            AssertCode("@@@", "4151");
        }

        // ................:..::010011.:..:
        // Reko: a decoder for V850 instruction 699A at address 0010123C has not been implemented.
        [Test]
        public void V850Dis_699A()
        {
            AssertCode("@@@", "699A");
        }

        // ..................:::101010..:.:
        // Reko: a decoder for V850 instruction 453D at address 0010123E has not been implemented.
        [Test]
        public void V850Dis_453D()
        {
            AssertCode("@@@", "453D");
        }

        // ................:::..110010..::.
        // Reko: a decoder for V850 instruction 46E6 at address 00101240 has not been implemented.
        [Test]
        public void V850Dis_46E6()
        {
            AssertCode("@@@", "46E6");
        }

        // ................:::.:111000:..:.
        // Reko: a decoder for V850 instruction 12EF at address 00101242 has not been implemented.
        [Test]
        public void V850Dis_12EF()
        {
            AssertCode("@@@", "12EF");
        }

        // .................::.:001000.::::
        // .................::.:..:....:::: or
        // ................:..::001110..:::
        // Reko: a decoder for V850 instruction C799 at address 00101246 has not been implemented.
        [Test]
        public void V850Dis_C799()
        {
            AssertCode("@@@", "C799");
        }

        // .................:...101100.::.:
        // Reko: a decoder for V850 instruction 8D45 at address 00101248 has not been implemented.
        [Test]
        public void V850Dis_8D45()
        {
            AssertCode("@@@", "8D45");
        }

        // .................::..000011:....
        // Reko: a decoder for V850 instruction 7060 at address 0010124A has not been implemented.
        [Test]
        public void V850Dis_7060()
        {
            AssertCode("@@@", "7060");
        }

        // ................:::..111101..:::
        // Reko: a decoder for V850 instruction A7E7 at address 0010124C has not been implemented.
        [Test]
        public void V850Dis_A7E7()
        {
            AssertCode("@@@", "A7E7");
        }

        // ................::...000111:.:..
        // Reko: a decoder for V850 instruction F4C0 at address 0010124E has not been implemented.
        [Test]
        public void V850Dis_F4C0()
        {
            AssertCode("@@@", "F4C0");
        }

        // ................::.::111001...:.
        // Reko: a decoder for V850 instruction 22DF at address 00101250 has not been implemented.
        [Test]
        public void V850Dis_22DF()
        {
            AssertCode("@@@", "22DF");
        }

        // .................:...011111.::..
        // .................:....:::::.::.. sst_b
        // .................::..101011.::..
        // Reko: a decoder for V850 instruction 6C65 at address 00101254 has not been implemented.
        [Test]
        public void V850Dis_6C65()
        {
            AssertCode("@@@", "6C65");
        }

        // ................:::::101110.:.:.
        // Reko: a decoder for V850 instruction CAFD at address 00101256 has not been implemented.
        [Test]
        public void V850Dis_CAFD()
        {
            AssertCode("@@@", "CAFD");
        }

        // ..................::.000001..:::
        // ..................::......:..::: not
        // .................:::.000001:...:
        // .................:::......::...: not
        // ................:::::101011::.::
        // Reko: a decoder for V850 instruction 7BFD at address 0010125C has not been implemented.
        [Test]
        public void V850Dis_7BFD()
        {
            AssertCode("@@@", "7BFD");
        }

        // .................::::001000.:.::
        // .................::::..:....:.:: or
        // ................:..:.001010.:...
        // Reko: a decoder for V850 instruction 4891 at address 00101260 has not been implemented.
        [Test]
        public void V850Dis_4891()
        {
            AssertCode("@@@", "4891");
        }

        // .................::::000010..:.:
        // ................01111....:...:.:
        // .................::::....:.00101
        // .................::::....:...:.: divh
        // ................:.::.101001.:.:.
        // Reko: a decoder for V850 instruction 2AB5 at address 00101264 has not been implemented.
        [Test]
        public void V850Dis_2AB5()
        {
            AssertCode("@@@", "2AB5");
        }

        // ...................:.011010:::::
        // ...................:..::.:.::::: sld_b
        // ................:::::000001.:.:.
        // ................:::::.....:.:.:. not
        // ................::..:000000:::..
        // ................11001......:::..
        // ................::..:......:::.. mov
        // .................::.:100100.::.:
        // .................::.::..:...::.: sst_h
        // ..................::.001101.:..:
        // Reko: a decoder for V850 instruction A931 at address 0010126E has not been implemented.
        [Test]
        public void V850Dis_A931()
        {
            AssertCode("@@@", "A931");
        }

        // ................:....001001:::.:
        // Reko: a decoder for V850 instruction 3D81 at address 00101270 has not been implemented.
        [Test]
        public void V850Dis_3D81()
        {
            AssertCode("@@@", "3D81");
        }

        // ................:...:010001.:::.
        // Reko: a decoder for V850 instruction 2E8A at address 00101272 has not been implemented.
        [Test]
        public void V850Dis_2E8A()
        {
            AssertCode("@@@", "2E8A");
        }

        // .....................001101.:::.
        // Reko: a decoder for V850 instruction AE01 at address 00101274 has not been implemented.
        [Test]
        public void V850Dis_AE01()
        {
            AssertCode("@@@", "AE01");
        }

        // ................:::..001110:...:
        // Reko: a decoder for V850 instruction D1E1 at address 00101276 has not been implemented.
        [Test]
        public void V850Dis_D1E1()
        {
            AssertCode("@@@", "D1E1");
        }

        // ...................:.101000:::..
        // ...................:.:.:...:::.0 Sld.w/Sst.w
        // ...................:.:.:...:::.. sld_w
        // ................::.::111110..:::
        // Reko: a decoder for V850 instruction C7DF at address 0010127A has not been implemented.
        [Test]
        public void V850Dis_C7DF()
        {
            AssertCode("@@@", "C7DF");
        }

        // .................:.:.110001:.:::
        // Reko: a decoder for V850 instruction 3756 at address 0010127C has not been implemented.
        [Test]
        public void V850Dis_3756()
        {
            AssertCode("@@@", "3756");
        }

        // ................:...:111000:::.:
        // Reko: a decoder for V850 instruction 1D8F at address 0010127E has not been implemented.
        [Test]
        public void V850Dis_1D8F()
        {
            AssertCode("@@@", "1D8F");
        }

        // ................::::.110001::.:.
        // Reko: a decoder for V850 instruction 3AF6 at address 00101280 has not been implemented.
        [Test]
        public void V850Dis_3AF6()
        {
            AssertCode("@@@", "3AF6");
        }

        // ...................:.101000:.:::
        // ...................:.:.:...:.::1 Sld.w/Sst.w
        // ...................:.:.:...:.::: sst_w
        // .................::::011101...:.
        // .................::::.:::.:...:. sst_b
        // .................::.:100100:.::.
        // .................::.::..:..:.::. sst_h
        // ................::.::001011:::::
        // Reko: a decoder for V850 instruction 7FD9 at address 00101288 has not been implemented.
        [Test]
        public void V850Dis_7FD9()
        {
            AssertCode("@@@", "7FD9");
        }

        // .................:::.011000::..:
        // .................:::..::...::..: sld_b
        // .....................001010.:...
        // Reko: a decoder for V850 instruction 4801 at address 0010128C has not been implemented.
        [Test]
        public void V850Dis_4801()
        {
            AssertCode("@@@", "4801");
        }

        // ................:.:::010011:....
        // Reko: a decoder for V850 instruction 70BA at address 0010128E has not been implemented.
        [Test]
        public void V850Dis_70BA()
        {
            AssertCode("@@@", "70BA");
        }

        // .....................000100..::.
        // Reko: a decoder for V850 instruction 8600 at address 00101290 has not been implemented.
        [Test]
        public void V850Dis_8600()
        {
            AssertCode("@@@", "8600");
        }

        // ................:::::010001:..::
        // Reko: a decoder for V850 instruction 33FA at address 00101292 has not been implemented.
        [Test]
        public void V850Dis_33FA()
        {
            AssertCode("@@@", "33FA");
        }

        // ................:....001110.:::.
        // Reko: a decoder for V850 instruction CE81 at address 00101294 has not been implemented.
        [Test]
        public void V850Dis_CE81()
        {
            AssertCode("@@@", "CE81");
        }

        // .................:...111110.:.::
        // Reko: a decoder for V850 instruction CB47 at address 00101296 has not been implemented.
        [Test]
        public void V850Dis_CB47()
        {
            AssertCode("@@@", "CB47");
        }

        // .................::..100101:::::
        // .................::..:..:.:::::: sst_h
        // ...................:.100010:::..
        // ...................:.:...:.:::.. sld_h
        // ................:....000111.::.:
        // Reko: a decoder for V850 instruction ED80 at address 0010129C has not been implemented.
        [Test]
        public void V850Dis_ED80()
        {
            AssertCode("@@@", "ED80");
        }

        // ................::.:.011110::.::
        // ................::.:..::::.::.:: sst_b
        // ................:...:101110::.::
        // Reko: a decoder for V850 instruction DB8D at address 001012A0 has not been implemented.
        [Test]
        public void V850Dis_DB8D()
        {
            AssertCode("@@@", "DB8D");
        }

        // ................:..:.110100.:::.
        // Reko: a decoder for V850 instruction 8E96 at address 001012A2 has not been implemented.
        [Test]
        public void V850Dis_8E96()
        {
            AssertCode("@@@", "8E96");
        }

        // ................:..:.011010:.:..
        // ................:..:..::.:.:.:.. sld_b
        // ................::...001100.::::
        // Reko: a decoder for V850 instruction 8FC1 at address 001012A6 has not been implemented.
        [Test]
        public void V850Dis_8FC1()
        {
            AssertCode("@@@", "8FC1");
        }

        // ................::...010111.:.:.
        // Reko: a decoder for V850 instruction EAC2 at address 001012A8 has not been implemented.
        [Test]
        public void V850Dis_EAC2()
        {
            AssertCode("@@@", "EAC2");
        }

        // ................::.:.101000.:.:.
        // ................::.:.:.:....:.:0 Sld.w/Sst.w
        // ................::.:.:.:....:.:. sld_w
        // .................:.::101111.:...
        // Reko: a decoder for V850 instruction E85D at address 001012AC has not been implemented.
        [Test]
        public void V850Dis_E85D()
        {
            AssertCode("@@@", "E85D");
        }

        // ...................::011100::..:
        // ...................::.:::..::..: sst_b
        // ....................:100101..:.:
        // ....................::..:.:..:.: sst_h
        // ..................::.111010::...
        // Reko: a decoder for V850 instruction 5837 at address 001012B2 has not been implemented.
        [Test]
        public void V850Dis_5837()
        {
            AssertCode("@@@", "5837");
        }

        // ..................:..010001...::
        // Reko: a decoder for V850 instruction 2322 at address 001012B4 has not been implemented.
        [Test]
        public void V850Dis_2322()
        {
            AssertCode("@@@", "2322");
        }

        // ................:.:::000110....:
        // Reko: a decoder for V850 instruction C1B8 at address 001012B6 has not been implemented.
        [Test]
        public void V850Dis_C1B8()
        {
            AssertCode("@@@", "C1B8");
        }

        // ................::.:.000001.:.::
        // ................::.:......:.:.:: not
        // ..................:::110010:.::.
        // Reko: a decoder for V850 instruction 563E at address 001012BA has not been implemented.
        [Test]
        public void V850Dis_563E()
        {
            AssertCode("@@@", "563E");
        }

        // ................:....001000:::::
        // ................:......:...::::: or
        // ................:::::111011:::::
        // Reko: a decoder for V850 instruction 7FFF at address 001012BE has not been implemented.
        [Test]
        public void V850Dis_7FFF()
        {
            AssertCode("@@@", "7FFF");
        }

        // .................:...110111:::.:
        // ................:.:::011100::...
        // ................:.:::.:::..::... sst_b
        // ................::.::001011:..:.
        // Reko: a decoder for V850 instruction 72D9 at address 001012C4 has not been implemented.
        [Test]
        public void V850Dis_72D9()
        {
            AssertCode("@@@", "72D9");
        }

        // ................::::.011011..::.
        // ................::::..::.::..::. sld_b
        // .................:::.001011:...:
        // Reko: a decoder for V850 instruction 7171 at address 001012C8 has not been implemented.
        [Test]
        public void V850Dis_7171()
        {
            AssertCode("@@@", "7171");
        }

        // ....................:100000.::::
        // ....................::......:::: sld_h
        // ................::::.101011:.:::
        // Reko: a decoder for V850 instruction 77F5 at address 001012CC has not been implemented.
        [Test]
        public void V850Dis_77F5()
        {
            AssertCode("@@@", "77F5");
        }

        // .................::..001010:::::
        // Reko: a decoder for V850 instruction 5F61 at address 001012CE has not been implemented.
        [Test]
        public void V850Dis_5F61()
        {
            AssertCode("@@@", "5F61");
        }

        // .................:...000110.:.:.
        // Reko: a decoder for V850 instruction CA40 at address 001012D0 has not been implemented.
        [Test]
        public void V850Dis_CA40()
        {
            AssertCode("@@@", "CA40");
        }

        // ................:::..000011.::::
        // Reko: a decoder for V850 instruction 6FE0 at address 001012D2 has not been implemented.
        [Test]
        public void V850Dis_6FE0()
        {
            AssertCode("@@@", "6FE0");
        }

        // ................:.:..100001.:...
        // ................:.:..:....:.:... sld_h
        // ................:.:::000101.::::
        // Reko: a decoder for V850 instruction AFB8 at address 001012D6 has not been implemented.
        [Test]
        public void V850Dis_AFB8()
        {
            AssertCode("@@@", "AFB8");
        }

        // ................::.:.101110.:.::
        // Reko: a decoder for V850 instruction CBD5 at address 001012D8 has not been implemented.
        [Test]
        public void V850Dis_CBD5()
        {
            AssertCode("@@@", "CBD5");
        }

        // ................:....100011.:.::
        // ................:....:...::.:.:: sld_h
        // .................:.:.001001:::.:
        // Reko: a decoder for V850 instruction 3D51 at address 001012DC has not been implemented.
        [Test]
        public void V850Dis_3D51()
        {
            AssertCode("@@@", "3D51");
        }

        // .................::.:001101....:
        // Reko: a decoder for V850 instruction A169 at address 001012DE has not been implemented.
        [Test]
        public void V850Dis_A169()
        {
            AssertCode("@@@", "A169");
        }

        // ....................:011010...:.
        // ....................:.::.:....:. sld_b
        // ................:.:..010110::...
        // Reko: a decoder for V850 instruction D8A2 at address 001012E2 has not been implemented.
        [Test]
        public void V850Dis_D8A2()
        {
            AssertCode("@@@", "D8A2");
        }

        // ................::.::101110..:.:
        // Reko: a decoder for V850 instruction C5DD at address 001012E4 has not been implemented.
        [Test]
        public void V850Dis_C5DD()
        {
            AssertCode("@@@", "C5DD");
        }

        // ................::..:100101.:..:
        // ................::..::..:.:.:..: sst_h
        // .................::..101101..::.
        // Reko: a decoder for V850 instruction A665 at address 001012E8 has not been implemented.
        [Test]
        public void V850Dis_A665()
        {
            AssertCode("@@@", "A665");
        }

        // ................:.::.010110..:.:
        // ................:.::.100000:..:.
        // ................:.::.:.....:..:. sld_h
        // ..................::.101010....:
        // Reko: a decoder for V850 instruction 4135 at address 001012EE has not been implemented.
        [Test]
        public void V850Dis_4135()
        {
            AssertCode("@@@", "4135");
        }

        // ................::::.100011:...:
        // ................::::.:...:::...: sld_h
        // ................::.::111011:..::
        // Reko: a decoder for V850 instruction 73DF at address 001012F2 has not been implemented.
        [Test]
        public void V850Dis_73DF()
        {
            AssertCode("@@@", "73DF");
        }

        // ................:..::110000:..:.
        // Reko: a decoder for V850 instruction 129E at address 001012F4 has not been implemented.
        [Test]
        public void V850Dis_129E()
        {
            AssertCode("@@@", "129E");
        }

        // .................:.::000110:.:::
        // Reko: a decoder for V850 instruction D758 at address 001012F6 has not been implemented.
        [Test]
        public void V850Dis_D758()
        {
            AssertCode("@@@", "D758");
        }

        // ................:::..000100:.::.
        // ................:..::100111.::::
        // ................:..:::..:::.:::: sst_h
        // ................:::..010011.:::.
        // Reko: a decoder for V850 instruction 6EE2 at address 001012FC has not been implemented.
        [Test]
        public void V850Dis_6EE2()
        {
            AssertCode("@@@", "6EE2");
        }

        // ...................::111101:.:.:
        // Reko: a decoder for V850 instruction B51F at address 001012FE has not been implemented.
        [Test]
        public void V850Dis_B51F()
        {
            AssertCode("@@@", "B51F");
        }

        // ................:.:..110110.::::
        // Reko: a decoder for V850 instruction CFA6 at address 00101300 has not been implemented.
        [Test]
        public void V850Dis_CFA6()
        {
            AssertCode("@@@", "CFA6");
        }

        // .................::.:011110.:..:
        // .................::.:.::::..:..: sst_b
        // ................:.:::101000:::.:
        // ................:.::::.:...:::.1 Sld.w/Sst.w
        // ................:.::::.:...:::.: sst_w
        // ................:::.:100100:::..
        // ................:::.::..:..:::.. sst_h
        // ................:.:..011100:.:::
        // ................:.:...:::..:.::: sst_b
        // ................::...010001::.:.
        // Reko: a decoder for V850 instruction 3AC2 at address 0010130A has not been implemented.
        [Test]
        public void V850Dis_3AC2()
        {
            AssertCode("@@@", "3AC2");
        }

        // ................:::::101110::::.
        // Reko: a decoder for V850 instruction DEFD at address 0010130C has not been implemented.
        [Test]
        public void V850Dis_DEFD()
        {
            AssertCode("@@@", "DEFD");
        }

        // ...................:.110101..::.
        // Reko: a decoder for V850 instruction A616 at address 0010130E has not been implemented.
        [Test]
        public void V850Dis_A616()
        {
            AssertCode("@@@", "A616");
        }

        // ..................::.101100:.:..
        // Reko: a decoder for V850 instruction 9435 at address 00101310 has not been implemented.
        [Test]
        public void V850Dis_9435()
        {
            AssertCode("@@@", "9435");
        }

        // .................:..:001000:..:.
        // .................:..:..:...:..:. or
        // ................::...010110:::.:
        // Reko: a decoder for V850 instruction DDC2 at address 00101314 has not been implemented.
        [Test]
        public void V850Dis_DDC2()
        {
            AssertCode("@@@", "DDC2");
        }

        // ................:.:..100010:.::.
        // ................:.:..:...:.:.::. sld_h
        // .................::::111001:.:..
        // Reko: a decoder for V850 instruction 347F at address 00101318 has not been implemented.
        [Test]
        public void V850Dis_347F()
        {
            AssertCode("@@@", "347F");
        }

        // ................:::..100000.:.::
        // ................:::..:......:.:: sld_h
        // ....................:100111.:::.
        // ....................::..:::.:::. sst_h
        // ................::...100010:::.:
        // ................::...:...:.:::.: sld_h
        // ................:.:..101101:....
        // Reko: a decoder for V850 instruction B0A5 at address 00101320 has not been implemented.
        [Test]
        public void V850Dis_B0A5()
        {
            AssertCode("@@@", "B0A5");
        }

        // .................::.:011111...:.
        // .................::.:.:::::...:. sst_b
        // ................::.:.101111.:.::
        // Reko: a decoder for V850 instruction EBD5 at address 00101324 has not been implemented.
        [Test]
        public void V850Dis_EBD5()
        {
            AssertCode("@@@", "EBD5");
        }

        // ................:.::.100000::...
        // ................:.::.:.....::... sld_h
        // ................:....110010:.:::
        // Reko: a decoder for V850 instruction 5786 at address 00101328 has not been implemented.
        [Test]
        public void V850Dis_5786()
        {
            AssertCode("@@@", "5786");
        }

        // .................::.:111010.:...
        // Reko: a decoder for V850 instruction 486F at address 0010132A has not been implemented.
        [Test]
        public void V850Dis_486F()
        {
            AssertCode("@@@", "486F");
        }

        // ...................:.100011..:..
        // ...................:.:...::..:.. sld_h
        // ................:..::000011::.:.
        // Reko: a decoder for V850 instruction 7A98 at address 0010132E has not been implemented.
        [Test]
        public void V850Dis_7A98()
        {
            AssertCode("@@@", "7A98");
        }

        // ................::.::000001::..:
        // ................::.::.....:::..: not
        // ..................:::000000.:...
        // ................00111.......:...
        // ..................:::.......:... mov
        // .................::.:011110.::::
        // .................::.:.::::..:::: sst_b
        // ....................:101010...:.
        // Reko: a decoder for V850 instruction 420D at address 00101336 has not been implemented.
        [Test]
        public void V850Dis_420D()
        {
            AssertCode("@@@", "420D");
        }

        // ..................:..100010.::::
        // ..................:..:...:..:::: sld_h
        // ................:..::101001:.:::
        // Reko: a decoder for V850 instruction 379D at address 0010133A has not been implemented.
        [Test]
        public void V850Dis_379D()
        {
            AssertCode("@@@", "379D");
        }

        // ................:.::.111101:...:
        // Reko: a decoder for V850 instruction B1B7 at address 0010133C has not been implemented.
        [Test]
        public void V850Dis_B1B7()
        {
            AssertCode("@@@", "B1B7");
        }

        // ................:...:010111::::.
        // Reko: a decoder for V850 instruction FE8A at address 0010133E has not been implemented.
        [Test]
        public void V850Dis_FE8A()
        {
            AssertCode("@@@", "FE8A");
        }

        // .................:::.101010..:.:
        // Reko: a decoder for V850 instruction 4575 at address 00101340 has not been implemented.
        [Test]
        public void V850Dis_4575()
        {
            AssertCode("@@@", "4575");
        }

        // ..................:::000000:..:.
        // ................00111......:..:.
        // ..................:::......:..:. mov
        // ..................:::010000::.:.
        // ..................:::.:....11010
        // ..................:::.:....::.:. mov
        // ................:.:::110001.:.::
        // Reko: a decoder for V850 instruction 2BBE at address 00101346 has not been implemented.
        [Test]
        public void V850Dis_2BBE()
        {
            AssertCode("@@@", "2BBE");
        }

        // .................:.::101000:::..
        // .................:.:::.:...:::.0 Sld.w/Sst.w
        // .................:.:::.:...:::.. sld_w
        // .................:.:.111111:...:
        // Reko: a decoder for V850 instruction F157 at address 0010134A has not been implemented.
        [Test]
        public void V850Dis_F157()
        {
            AssertCode("@@@", "F157");
        }

        // ................:::..111011::..:
        // Reko: a decoder for V850 instruction 79E7 at address 0010134C has not been implemented.
        [Test]
        public void V850Dis_79E7()
        {
            AssertCode("@@@", "79E7");
        }

        // ...................::111010.....
        // Reko: a decoder for V850 instruction 401F at address 0010134E has not been implemented.
        [Test]
        public void V850Dis_401F()
        {
            AssertCode("@@@", "401F");
        }

        // .................:::.010000..:.:
        // .................:::..:....00101
        // .................:::..:......:.: mov
        // ...................:.101101.:.::
        // Reko: a decoder for V850 instruction AB15 at address 00101352 has not been implemented.
        [Test]
        public void V850Dis_AB15()
        {
            AssertCode("@@@", "AB15");
        }

        // ..................:..101000..:..
        // ..................:..:.:.....:.0 Sld.w/Sst.w
        // ..................:..:.:.....:.. sld_w
        // .................:...100111:.:..
        // .................:...:..::::.:.. sst_h
        // ................::.:.100011...::
        // ................::.:.:...::...:: sld_h
        // ................::...110000::.::
        // Reko: a decoder for V850 instruction 1BC6 at address 0010135A has not been implemented.
        [Test]
        public void V850Dis_1BC6()
        {
            AssertCode("@@@", "1BC6");
        }

        // .................::::010100:::.:
        // Reko: a decoder for V850 instruction 9D7A at address 0010135C has not been implemented.
        [Test]
        public void V850Dis_9D7A()
        {
            AssertCode("@@@", "9D7A");
        }

        // ................:.:..000000...::
        // ................10100.........::
        // ................:.:...........:: mov
        // ................:.:::101100.:...
        // Reko: a decoder for V850 instruction 88BD at address 00101360 has not been implemented.
        [Test]
        public void V850Dis_88BD()
        {
            AssertCode("@@@", "88BD");
        }

        // ................:::.:111001.:.:.
        // Reko: a decoder for V850 instruction 2AEF at address 00101362 has not been implemented.
        [Test]
        public void V850Dis_2AEF()
        {
            AssertCode("@@@", "2AEF");
        }

        // .................:...001000...:.
        // .................:.....:......:. or
        // ................:::::110011:::.:
        // Reko: a decoder for V850 instruction 7DFE at address 00101366 has not been implemented.
        [Test]
        public void V850Dis_7DFE()
        {
            AssertCode("@@@", "7DFE");
        }

        // .................:..:010101.::.:
        // Reko: a decoder for V850 instruction AD4A at address 00101368 has not been implemented.
        [Test]
        public void V850Dis_AD4A()
        {
            AssertCode("@@@", "AD4A");
        }

        // .................:::.001000::.::
        // .................:::...:...::.:: or
        // ................:::.:010011::.:.
        // Reko: a decoder for V850 instruction 7AEA at address 0010136C has not been implemented.
        [Test]
        public void V850Dis_7AEA()
        {
            AssertCode("@@@", "7AEA");
        }

        // .................::.:000100:..::
        // Reko: a decoder for V850 instruction 9368 at address 0010136E has not been implemented.
        [Test]
        public void V850Dis_9368()
        {
            AssertCode("@@@", "9368");
        }

        // ...................::000111..:..
        // Reko: a decoder for V850 instruction E418 at address 00101370 has not been implemented.
        [Test]
        public void V850Dis_E418()
        {
            AssertCode("@@@", "E418");
        }

        // ..................:::110100::...
        // Reko: a decoder for V850 instruction 983E at address 00101372 has not been implemented.
        [Test]
        public void V850Dis_983E()
        {
            AssertCode("@@@", "983E");
        }

        // ................:::.:000000.::..
        // ................11101.......::..
        // ................:::.:.......::.. mov
        // ...................::111100..::.
        // Reko: a decoder for V850 instruction 861F at address 00101376 has not been implemented.
        [Test]
        public void V850Dis_861F()
        {
            AssertCode("@@@", "861F");
        }

        // ................::..:101100.....
        // Reko: a decoder for V850 instruction 80CD at address 00101378 has not been implemented.
        [Test]
        public void V850Dis_80CD()
        {
            AssertCode("@@@", "80CD");
        }

        // .................:...101110:.:.:
        // Reko: a decoder for V850 instruction D545 at address 0010137A has not been implemented.
        [Test]
        public void V850Dis_D545()
        {
            AssertCode("@@@", "D545");
        }

        // .................::..100000:::::
        // .................::..:.....::::: sld_h
        // ................:.:..001001.....
        // Reko: a decoder for V850 instruction 20A1 at address 0010137E has not been implemented.
        [Test]
        public void V850Dis_20A1()
        {
            AssertCode("@@@", "20A1");
        }

        // .................:.:.001010::.:.
        // Reko: a decoder for V850 instruction 5A51 at address 00101380 has not been implemented.
        [Test]
        public void V850Dis_5A51()
        {
            AssertCode("@@@", "5A51");
        }

        // ................:.:::011110.::::
        // ................:.:::.::::..:::: sst_b
        // .................:.:.001101:::.:
        // Reko: a decoder for V850 instruction BD51 at address 00101384 has not been implemented.
        [Test]
        public void V850Dis_BD51()
        {
            AssertCode("@@@", "BD51");
        }

        // ................::...100000:::.:
        // ................::...:.....:::.: sld_h
        // ................:.:.:100111:.:..
        // ................:.:.::..::::.:.. sst_h
        // ................:.:::010011.:...
        // Reko: a decoder for V850 instruction 68BA at address 0010138A has not been implemented.
        [Test]
        public void V850Dis_68BA()
        {
            AssertCode("@@@", "68BA");
        }

        // ................:....011000.:..:
        // ................:.....::....:..: sld_b
        // .................::::000110.:.:.
        // Reko: a decoder for V850 instruction CA78 at address 0010138E has not been implemented.
        [Test]
        public void V850Dis_CA78()
        {
            AssertCode("@@@", "CA78");
        }

        // ................:.::.011010....:
        // ................:.::..::.:.....: sld_b
        // ................:....101111....:
        // Reko: a decoder for V850 instruction E185 at address 00101392 has not been implemented.
        [Test]
        public void V850Dis_E185()
        {
            AssertCode("@@@", "E185");
        }

        // ................::...100111...:.
        // ................::...:..:::...:. sst_h
        // ................:..::111100:..::
        // Reko: a decoder for V850 instruction 939F at address 00101396 has not been implemented.
        [Test]
        public void V850Dis_939F()
        {
            AssertCode("@@@", "939F");
        }

        // ....................:010001:.:::
        // Reko: a decoder for V850 instruction 370A at address 00101398 has not been implemented.
        [Test]
        public void V850Dis_370A()
        {
            AssertCode("@@@", "370A");
        }

        // ..................::.100011.:::.
        // ..................::.:...::.:::. sld_h
        // ..................:::000011.:.::
        // .................:..:000110:::::
        // Reko: a decoder for V850 instruction DF48 at address 0010139E has not been implemented.
        [Test]
        public void V850Dis_DF48()
        {
            AssertCode("@@@", "DF48");
        }

        // .................::::001001.:..:
        // Reko: a decoder for V850 instruction 2979 at address 001013A0 has not been implemented.
        [Test]
        public void V850Dis_2979()
        {
            AssertCode("@@@", "2979");
        }

        // ..................:.:000000:.:::
        // ................00101......:.:::
        // ..................:.:......:.::: mov
        // ................::.:.111000::.::
        // Reko: a decoder for V850 instruction 1BD7 at address 001013A4 has not been implemented.
        [Test]
        public void V850Dis_1BD7()
        {
            AssertCode("@@@", "1BD7");
        }

        // ................:..:.011101.:.::
        // ................:..:..:::.:.:.:: sst_b
        // ................::.:.100111::.::
        // ................::.:.:..:::::.:: sst_h
        // .................:.::111010.:.:.
        // Reko: a decoder for V850 instruction 4A5F at address 001013AA has not been implemented.
        [Test]
        public void V850Dis_4A5F()
        {
            AssertCode("@@@", "4A5F");
        }

        // ................::..:100100.....
        // ................::..::..:....... sst_h
        // ................:::::101000:.::.
        // ................::::::.:...:.::0 Sld.w/Sst.w
        // ................::::::.:...:.::. sld_w
        // .................:.::100000...::
        // .................:.:::........:: sld_h
        // .................::.:100000..:..
        // .................::.::.......:.. sld_h
        // ................:..::011100...:.
        // ................:..::.:::.....:. sst_b
        // ................:..:.110101:::::
        // Reko: a decoder for V850 instruction BF96 at address 001013B6 has not been implemented.
        [Test]
        public void V850Dis_BF96()
        {
            AssertCode("@@@", "BF96");
        }

        // ..................:.:111101:::.:
        // Reko: a decoder for V850 instruction BD2F at address 001013B8 has not been implemented.
        [Test]
        public void V850Dis_BD2F()
        {
            AssertCode("@@@", "BD2F");
        }

        // ................:.::.010100..:..
        // Reko: a decoder for V850 instruction 84B2 at address 001013BA has not been implemented.
        [Test]
        public void V850Dis_84B2()
        {
            AssertCode("@@@", "84B2");
        }

        // ................:::.:000111...:.
        // Reko: a decoder for V850 instruction E2E8 at address 001013BC has not been implemented.
        [Test]
        public void V850Dis_E2E8()
        {
            AssertCode("@@@", "E2E8");
        }

        // ................:.:::100010::...
        // ................:.::::...:.::... sld_h
        // ...................::111110.::.:
        // Reko: a decoder for V850 instruction CD1F at address 001013C0 has not been implemented.
        [Test]
        public void V850Dis_CD1F()
        {
            AssertCode("@@@", "CD1F");
        }

        // ................:...:111100:....
        // Reko: a decoder for V850 instruction 908F at address 001013C2 has not been implemented.
        [Test]
        public void V850Dis_908F()
        {
            AssertCode("@@@", "908F");
        }

        // ................:.:..010011.:.::
        // Reko: a decoder for V850 instruction 6BA2 at address 001013C4 has not been implemented.
        [Test]
        public void V850Dis_6BA2()
        {
            AssertCode("@@@", "6BA2");
        }

        // .................::.:001010:::..
        // Reko: a decoder for V850 instruction 5C69 at address 001013C6 has not been implemented.
        [Test]
        public void V850Dis_5C69()
        {
            AssertCode("@@@", "5C69");
        }

        // .................:..:110000..:::
        // Reko: a decoder for V850 instruction 074E at address 001013C8 has not been implemented.
        [Test]
        public void V850Dis_074E()
        {
            AssertCode("@@@", "074E");
        }

        // ....................:110100.:.:.
        // Reko: a decoder for V850 instruction 8A0E at address 001013CA has not been implemented.
        [Test]
        public void V850Dis_8A0E()
        {
            AssertCode("@@@", "8A0E");
        }

        // ..................::.100011::.:.
        // ..................::.:...::::.:. sld_h
        // .................::.:011000:::::
        // .................::.:.::...::::: sld_b
        // .................:::.001111:.:..
        // Reko: a decoder for V850 instruction F471 at address 001013D0 has not been implemented.
        [Test]
        public void V850Dis_F471()
        {
            AssertCode("@@@", "F471");
        }

        // ....................:111001:...:
        // Reko: a decoder for V850 instruction 310F at address 001013D2 has not been implemented.
        [Test]
        public void V850Dis_310F()
        {
            AssertCode("@@@", "310F");
        }

        // .................:::.010001:..::
        // Reko: a decoder for V850 instruction 3372 at address 001013D4 has not been implemented.
        [Test]
        public void V850Dis_3372()
        {
            AssertCode("@@@", "3372");
        }

        // ................:...:110110..:.:
        // Reko: a decoder for V850 instruction C58E at address 001013D6 has not been implemented.
        [Test]
        public void V850Dis_C58E()
        {
            AssertCode("@@@", "C58E");
        }

        // ................:....001101::.:.
        // Reko: a decoder for V850 instruction BA81 at address 001013D8 has not been implemented.
        [Test]
        public void V850Dis_BA81()
        {
            AssertCode("@@@", "BA81");
        }

        // ................:::.:100011.:.:.
        // ................:::.::...::.:.:. sld_h
        // ................:::::000010:..::
        // ................11111....:.:..::
        // ................:::::....:.10011
        // ................:::::....:.:..:: divh
        // ...................:.010101:....
        // Reko: a decoder for V850 instruction B012 at address 001013DE has not been implemented.
        [Test]
        public void V850Dis_B012()
        {
            AssertCode("@@@", "B012");
        }

        // ................:...:101011::.::
        // Reko: a decoder for V850 instruction 7B8D at address 001013E0 has not been implemented.
        [Test]
        public void V850Dis_7B8D()
        {
            AssertCode("@@@", "7B8D");
        }

        // .................::..000100:...:
        // Reko: a decoder for V850 instruction 9160 at address 001013E2 has not been implemented.
        [Test]
        public void V850Dis_9160()
        {
            AssertCode("@@@", "9160");
        }

        // ................:....110001::.::
        // Reko: a decoder for V850 instruction 3B86 at address 001013E4 has not been implemented.
        [Test]
        public void V850Dis_3B86()
        {
            AssertCode("@@@", "3B86");
        }

        // .................::..001100:...:
        // Reko: a decoder for V850 instruction 9161 at address 001013E6 has not been implemented.
        [Test]
        public void V850Dis_9161()
        {
            AssertCode("@@@", "9161");
        }

        // ................:..::011111:::::
        // ................:..::.:::::::::: sst_b
        // ..................::.100011..:..
        // ..................::.:...::..:.. sld_h
        // ................:.::.000000:....
        // ................10110......:....
        // ................:.::.......:.... mov
        // ................:...:001000::.::
        // ................:...:..:...::.:: or
        // ...................::001000:....
        // ...................::..:...:.... or
        // .................:::.001101::::.
        // Reko: a decoder for V850 instruction BE71 at address 001013F2 has not been implemented.
        [Test]
        public void V850Dis_BE71()
        {
            AssertCode("@@@", "BE71");
        }

        // .................:..:000101:.:::
        // Reko: a decoder for V850 instruction B748 at address 001013F4 has not been implemented.
        [Test]
        public void V850Dis_B748()
        {
            AssertCode("@@@", "B748");
        }

        // .................:.::010100.:..:
        // Reko: a decoder for V850 instruction 895A at address 001013F6 has not been implemented.
        [Test]
        public void V850Dis_895A()
        {
            AssertCode("@@@", "895A");
        }

        // .....................001010::..:
        // Reko: a decoder for V850 instruction 5901 at address 001013F8 has not been implemented.
        [Test]
        public void V850Dis_5901()
        {
            AssertCode("@@@", "5901");
        }

        // ................:::.:010110:.:.:
        // Reko: a decoder for V850 instruction D5EA at address 001013FA has not been implemented.
        [Test]
        public void V850Dis_D5EA()
        {
            AssertCode("@@@", "D5EA");
        }

        // .................:::.000001:.:::
        // .................:::......::.::: not
        // ....................:111000:.:.:
        // Reko: a decoder for V850 instruction 150F at address 001013FE has not been implemented.
        [Test]
        public void V850Dis_150F()
        {
            AssertCode("@@@", "150F");
        }

        // .................:::.111100:::.:
        // Reko: a decoder for V850 instruction 9D77 at address 00101400 has not been implemented.
        [Test]
        public void V850Dis_9D77()
        {
            AssertCode("@@@", "9D77");
        }

        // ................:::.:101100:..:.
        // Reko: a decoder for V850 instruction 92ED at address 00101402 has not been implemented.
        [Test]
        public void V850Dis_92ED()
        {
            AssertCode("@@@", "92ED");
        }

        // ................:.:::111101...:.
        // Reko: a decoder for V850 instruction A2BF at address 00101404 has not been implemented.
        [Test]
        public void V850Dis_A2BF()
        {
            AssertCode("@@@", "A2BF");
        }

        // .................::.:110001.::::
        // Reko: a decoder for V850 instruction 2F6E at address 00101406 has not been implemented.
        [Test]
        public void V850Dis_2F6E()
        {
            AssertCode("@@@", "2F6E");
        }

        // ................::.:.000110:::::
        // Reko: a decoder for V850 instruction DFD0 at address 00101408 has not been implemented.
        [Test]
        public void V850Dis_DFD0()
        {
            AssertCode("@@@", "DFD0");
        }

        // ................:..:.111000:....
        // Reko: a decoder for V850 instruction 1097 at address 0010140A has not been implemented.
        [Test]
        public void V850Dis_1097()
        {
            AssertCode("@@@", "1097");
        }

        // ................:..:.001000.:::.
        // ................:..:...:....:::. or
        // ....................:001011:::.:
        // Reko: a decoder for V850 instruction 7D09 at address 0010140E has not been implemented.
        [Test]
        public void V850Dis_7D09()
        {
            AssertCode("@@@", "7D09");
        }

        // ................:::..000011..:.:
        // Reko: a decoder for V850 instruction 65E0 at address 00101410 has not been implemented.
        [Test]
        public void V850Dis_65E0()
        {
            AssertCode("@@@", "65E0");
        }

        // ..................:::001110:::..
        // Reko: a decoder for V850 instruction DC39 at address 00101412 has not been implemented.
        [Test]
        public void V850Dis_DC39()
        {
            AssertCode("@@@", "DC39");
        }

        // ..................:::111001::.:.
        // Reko: a decoder for V850 instruction 3A3F at address 00101414 has not been implemented.
        [Test]
        public void V850Dis_3A3F()
        {
            AssertCode("@@@", "3A3F");
        }

        // ..................::.011010::.::
        // ..................::..::.:.::.:: sld_b
        // ..................::.111000...::
        // Reko: a decoder for V850 instruction 0337 at address 00101418 has not been implemented.
        [Test]
        public void V850Dis_0337()
        {
            AssertCode("@@@", "0337");
        }

        // ..................:::001000..::.
        // ..................:::..:.....::. or
        // ................:.:..111101:...:
        // Reko: a decoder for V850 instruction B1A7 at address 0010141C has not been implemented.
        [Test]
        public void V850Dis_B1A7()
        {
            AssertCode("@@@", "B1A7");
        }

        // ................:...:110001.:..:
        // Reko: a decoder for V850 instruction 298E at address 0010141E has not been implemented.
        [Test]
        public void V850Dis_298E()
        {
            AssertCode("@@@", "298E");
        }

        // .................:.:.101100..:.:
        // Reko: a decoder for V850 instruction 8555 at address 00101420 has not been implemented.
        [Test]
        public void V850Dis_8555()
        {
            AssertCode("@@@", "8555");
        }

        // ................:..::001100:.::.
        // Reko: a decoder for V850 instruction 9699 at address 00101422 has not been implemented.
        [Test]
        public void V850Dis_9699()
        {
            AssertCode("@@@", "9699");
        }

        // ..................:.:110000::::.
        // Reko: a decoder for V850 instruction 1E2E at address 00101424 has not been implemented.
        [Test]
        public void V850Dis_1E2E()
        {
            AssertCode("@@@", "1E2E");
        }

        // .................:.:.001111..:::
        // Reko: a decoder for V850 instruction E751 at address 00101426 has not been implemented.
        [Test]
        public void V850Dis_E751()
        {
            AssertCode("@@@", "E751");
        }

        // .................:.:.000111.:.:.
        // Reko: a decoder for V850 instruction EA50 at address 00101428 has not been implemented.
        [Test]
        public void V850Dis_EA50()
        {
            AssertCode("@@@", "EA50");
        }

        // ................:::..111100:...:
        // Reko: a decoder for V850 instruction 91E7 at address 0010142A has not been implemented.
        [Test]
        public void V850Dis_91E7()
        {
            AssertCode("@@@", "91E7");
        }

        // ................::::.010001::...
        // Reko: a decoder for V850 instruction 38F2 at address 0010142C has not been implemented.
        [Test]
        public void V850Dis_38F2()
        {
            AssertCode("@@@", "38F2");
        }

        // ................::..:000010.::..
        // ................11001....:..::..
        // ................::..:....:.01100
        // ................::..:....:..::.. divh
        // ................::..:011100..:..
        // ................::..:.:::....:.. sst_b
        // .................:.:.110100..:..
        // Reko: a decoder for V850 instruction 8456 at address 00101432 has not been implemented.
        [Test]
        public void V850Dis_8456()
        {
            AssertCode("@@@", "8456");
        }

        // ................::.::011100:.:..
        // ................::.::.:::..:.:.. sst_b
        // .................:.::101110:.:.:
        // Reko: a decoder for V850 instruction D55D at address 00101436 has not been implemented.
        [Test]
        public void V850Dis_D55D()
        {
            AssertCode("@@@", "D55D");
        }

        // .................:.::111000::.::
        // Reko: a decoder for V850 instruction 1B5F at address 00101438 has not been implemented.
        [Test]
        public void V850Dis_1B5F()
        {
            AssertCode("@@@", "1B5F");
        }

        // .................::.:011111.:..:
        // .................::.:.:::::.:..: sst_b
        // ................::::.101101::...
        // Reko: a decoder for V850 instruction B8F5 at address 0010143C has not been implemented.
        [Test]
        public void V850Dis_B8F5()
        {
            AssertCode("@@@", "B8F5");
        }

        // ..................:::000101:::..
        // Reko: a decoder for V850 instruction BC38 at address 0010143E has not been implemented.
        [Test]
        public void V850Dis_BC38()
        {
            AssertCode("@@@", "BC38");
        }

        // ................:....001101..::.
        // Reko: a decoder for V850 instruction A681 at address 00101440 has not been implemented.
        [Test]
        public void V850Dis_A681()
        {
            AssertCode("@@@", "A681");
        }

        // ................:.:::000000:...:
        // ................10111......:...:
        // ................:.:::......:...: mov
        // ................:::..011111::.:.
        // ................:::...:::::::.:. sst_b
        // ..................::.111111.:.::
        // Reko: a decoder for V850 instruction EB37 at address 00101446 has not been implemented.
        [Test]
        public void V850Dis_EB37()
        {
            AssertCode("@@@", "EB37");
        }

        // ................:...:011111::..:
        // ................:...:.:::::::..: sst_b
        // ................:::.:001010.::::
        // Reko: a decoder for V850 instruction 4FE9 at address 0010144A has not been implemented.
        [Test]
        public void V850Dis_4FE9()
        {
            AssertCode("@@@", "4FE9");
        }

        // .................:::.100101.:::.
        // .................:::.:..:.:.:::. sst_h
        // ...................:.001111::.:.
        // Reko: a decoder for V850 instruction FA11 at address 0010144E has not been implemented.
        [Test]
        public void V850Dis_FA11()
        {
            AssertCode("@@@", "FA11");
        }

        // ..................:::110111.:.:.
        // Reko: a decoder for V850 instruction EA3E at address 00101450 has not been implemented.
        [Test]
        public void V850Dis_EA3E()
        {
            AssertCode("@@@", "EA3E");
        }

        // ................:::..110101:.:.:
        // Reko: a decoder for V850 instruction B5E6 at address 00101452 has not been implemented.
        [Test]
        public void V850Dis_B5E6()
        {
            AssertCode("@@@", "B5E6");
        }

        // ................::.:.010001...:.
        // Reko: a decoder for V850 instruction 22D2 at address 00101454 has not been implemented.
        [Test]
        public void V850Dis_22D2()
        {
            AssertCode("@@@", "22D2");
        }

        // ................:.:.:001111::.:.
        // Reko: a decoder for V850 instruction FAA9 at address 00101456 has not been implemented.
        [Test]
        public void V850Dis_FAA9()
        {
            AssertCode("@@@", "FAA9");
        }

        // ..................:::001100.....
        // Reko: a decoder for V850 instruction 8039 at address 00101458 has not been implemented.
        [Test]
        public void V850Dis_8039()
        {
            AssertCode("@@@", "8039");
        }

        // ................:.:::111011::::.
        // Reko: a decoder for V850 instruction 7EBF at address 0010145A has not been implemented.
        [Test]
        public void V850Dis_7EBF()
        {
            AssertCode("@@@", "7EBF");
        }

        // .................::::100010..:.:
        // .................:::::...:...:.: sld_h
        // ................::::.101100::...
        // Reko: a decoder for V850 instruction 98F5 at address 0010145E has not been implemented.
        [Test]
        public void V850Dis_98F5()
        {
            AssertCode("@@@", "98F5");
        }

        // .................:.::000100:..::
        // Reko: a decoder for V850 instruction 9358 at address 00101460 has not been implemented.
        [Test]
        public void V850Dis_9358()
        {
            AssertCode("@@@", "9358");
        }

        // ................:.::.111010.....
        // Reko: a decoder for V850 instruction 40B7 at address 00101462 has not been implemented.
        [Test]
        public void V850Dis_40B7()
        {
            AssertCode("@@@", "40B7");
        }

        // ................:..:.011111.....
        // ................:..:..:::::..... sst_b
        // ................:.:..000110.::.:
        // Reko: a decoder for V850 instruction CDA0 at address 00101466 has not been implemented.
        [Test]
        public void V850Dis_CDA0()
        {
            AssertCode("@@@", "CDA0");
        }

        // .................:..:101110:::::
        // Reko: a decoder for V850 instruction DF4D at address 00101468 has not been implemented.
        [Test]
        public void V850Dis_DF4D()
        {
            AssertCode("@@@", "DF4D");
        }

        // ....................:001010:::.:
        // Reko: a decoder for V850 instruction 5D09 at address 0010146A has not been implemented.
        [Test]
        public void V850Dis_5D09()
        {
            AssertCode("@@@", "5D09");
        }

        // ................:::.:011100.::..
        // ................:::.:.:::...::.. sst_b
        // ................:.:..111101.::::
        // Reko: a decoder for V850 instruction AFA7 at address 0010146E has not been implemented.
        [Test]
        public void V850Dis_AFA7()
        {
            AssertCode("@@@", "AFA7");
        }

        // ................::.:.111011..:..
        // Reko: a decoder for V850 instruction 64D7 at address 00101470 has not been implemented.
        [Test]
        public void V850Dis_64D7()
        {
            AssertCode("@@@", "64D7");
        }

        // .................::::010100.....
        // Reko: a decoder for V850 instruction 807A at address 00101472 has not been implemented.
        [Test]
        public void V850Dis_807A()
        {
            AssertCode("@@@", "807A");
        }

        // ................::.:.110001:::::
        // Reko: a decoder for V850 instruction 3FD6 at address 00101474 has not been implemented.
        [Test]
        public void V850Dis_3FD6()
        {
            AssertCode("@@@", "3FD6");
        }

        // ................::.:.011000:.::.
        // ................::.:..::...:.::. sld_b
        // .................::..111100..::.
        // Reko: a decoder for V850 instruction 8667 at address 00101478 has not been implemented.
        [Test]
        public void V850Dis_8667()
        {
            AssertCode("@@@", "8667");
        }

        // ................::...001001::...
        // Reko: a decoder for V850 instruction 38C1 at address 0010147A has not been implemented.
        [Test]
        public void V850Dis_38C1()
        {
            AssertCode("@@@", "38C1");
        }

        // ..................:.:100011..:.:
        // ..................:.::...::..:.: sld_h
        // .................::::101111:..:.
        // Reko: a decoder for V850 instruction F27D at address 0010147E has not been implemented.
        [Test]
        public void V850Dis_F27D()
        {
            AssertCode("@@@", "F27D");
        }

        // .................:.::010111:..:.
        // Reko: a decoder for V850 instruction F25A at address 00101480 has not been implemented.
        [Test]
        public void V850Dis_F25A()
        {
            AssertCode("@@@", "F25A");
        }

        // .................:.:.110010:.::.
        // Reko: a decoder for V850 instruction 5656 at address 00101482 has not been implemented.
        [Test]
        public void V850Dis_5656()
        {
            AssertCode("@@@", "5656");
        }

        // .................:...010001:.:..
        // Reko: a decoder for V850 instruction 3442 at address 00101484 has not been implemented.
        [Test]
        public void V850Dis_3442()
        {
            AssertCode("@@@", "3442");
        }

        // .................:.:.111001:....
        // Reko: a decoder for V850 instruction 3057 at address 00101486 has not been implemented.
        [Test]
        public void V850Dis_3057()
        {
            AssertCode("@@@", "3057");
        }

        // ...................:.101011:....
        // Reko: a decoder for V850 instruction 7015 at address 00101488 has not been implemented.
        [Test]
        public void V850Dis_7015()
        {
            AssertCode("@@@", "7015");
        }

        // ................::.:.100000..::.
        // ................::.:.:.......::. sld_h
        // ................::::.001011:.:.:
        // Reko: a decoder for V850 instruction 75F1 at address 0010148C has not been implemented.
        [Test]
        public void V850Dis_75F1()
        {
            AssertCode("@@@", "75F1");
        }

        // ................:..:.100000:::.:
        // ................:..:.:.....:::.: sld_h
        // ................::.:.110100.:..:
        // Reko: a decoder for V850 instruction 89D6 at address 00101490 has not been implemented.
        [Test]
        public void V850Dis_89D6()
        {
            AssertCode("@@@", "89D6");
        }

        // .................::.:101010::.:.
        // Reko: a decoder for V850 instruction 5A6D at address 00101492 has not been implemented.
        [Test]
        public void V850Dis_5A6D()
        {
            AssertCode("@@@", "5A6D");
        }

        // ...................::110111:::..
        // Reko: a decoder for V850 instruction FC1E at address 00101494 has not been implemented.
        [Test]
        public void V850Dis_FC1E()
        {
            AssertCode("@@@", "FC1E");
        }

        // ................:.:::100101:.:.:
        // ................:.::::..:.::.:.: sst_h
        // .................::.:100010....:
        // .................::.::...:.....: sld_h
        // .................::::001001:...:
        // Reko: a decoder for V850 instruction 3179 at address 0010149A has not been implemented.
        [Test]
        public void V850Dis_3179()
        {
            AssertCode("@@@", "3179");
        }

        // ....................:100010::.:.
        // ....................::...:.::.:. sld_h
        // ................:.::.011001::.:.
        // ................:.::..::..:::.:. sld_b
        // ................::.::111010::.::
        // Reko: a decoder for V850 instruction 5BDF at address 001014A0 has not been implemented.
        [Test]
        public void V850Dis_5BDF()
        {
            AssertCode("@@@", "5BDF");
        }

        // ..................::.010000:.::.
        // ..................::..:....10110
        // ..................::..:....:.::. mov
        // .................:.::000100:.:.:
        // Reko: a decoder for V850 instruction 9558 at address 001014A4 has not been implemented.
        [Test]
        public void V850Dis_9558()
        {
            AssertCode("@@@", "9558");
        }

        // ..................:..010011:..:.
        // ..................:.:001101..:::
        // Reko: a decoder for V850 instruction A729 at address 001014A8 has not been implemented.
        [Test]
        public void V850Dis_A729()
        {
            AssertCode("@@@", "A729");
        }

        // ..................:.:010000.:.:.
        // ..................:.:.:....01010
        // ..................:.:.:.....:.:. mov
        // .................:...010110:....
        // Reko: a decoder for V850 instruction D042 at address 001014AC has not been implemented.
        [Test]
        public void V850Dis_D042()
        {
            AssertCode("@@@", "D042");
        }

        // ................:..:.101010::::.
        // Reko: a decoder for V850 instruction 5E95 at address 001014AE has not been implemented.
        [Test]
        public void V850Dis_5E95()
        {
            AssertCode("@@@", "5E95");
        }

        // ................:.:..100010.:.:.
        // ................:.:..:...:..:.:. sld_h
        // ................:...:110001:.:::
        // Reko: a decoder for V850 instruction 378E at address 001014B2 has not been implemented.
        [Test]
        public void V850Dis_378E()
        {
            AssertCode("@@@", "378E");
        }

        // ................:....101110:..:.
        // Reko: a decoder for V850 instruction D285 at address 001014B4 has not been implemented.
        [Test]
        public void V850Dis_D285()
        {
            AssertCode("@@@", "D285");
        }

        // ................::.:.100100....:
        // ................::.:.:..:......: sst_h
        // ................:..::001101..:..
        // Reko: a decoder for V850 instruction A499 at address 001014B8 has not been implemented.
        [Test]
        public void V850Dis_A499()
        {
            AssertCode("@@@", "A499");
        }

        // ................::...111000:.:..
        // Reko: a decoder for V850 instruction 14C7 at address 001014BA has not been implemented.
        [Test]
        public void V850Dis_14C7()
        {
            AssertCode("@@@", "14C7");
        }

        // ................:.::.111000:...:
        // Reko: a decoder for V850 instruction 11B7 at address 001014BC has not been implemented.
        [Test]
        public void V850Dis_11B7()
        {
            AssertCode("@@@", "11B7");
        }

        // .................::..100111:::..
        // .................::..:..::::::.. sst_h
        // .................:.:.010110::.:.
        // Reko: a decoder for V850 instruction DA52 at address 001014C0 has not been implemented.
        [Test]
        public void V850Dis_DA52()
        {
            AssertCode("@@@", "DA52");
        }

        // ................:..:.101011::...
        // Reko: a decoder for V850 instruction 7895 at address 001014C2 has not been implemented.
        [Test]
        public void V850Dis_7895()
        {
            AssertCode("@@@", "7895");
        }

        // ................:::.:011110::.:.
        // ................:::.:.::::.::.:. sst_b
        // ..................::.000111:::::
        // Reko: a decoder for V850 instruction FF30 at address 001014C6 has not been implemented.
        [Test]
        public void V850Dis_FF30()
        {
            AssertCode("@@@", "FF30");
        }

        // ..................:::000011..:..
        // Reko: a decoder for V850 instruction 6438 at address 001014C8 has not been implemented.
        [Test]
        public void V850Dis_6438()
        {
            AssertCode("@@@", "6438");
        }

        // ................::.:.010110..::.
        // Reko: a decoder for V850 instruction C6D2 at address 001014CA has not been implemented.
        [Test]
        public void V850Dis_C6D2()
        {
            AssertCode("@@@", "C6D2");
        }

        // ................:...:011000:..::
        // ................:...:.::...:..:: sld_b
        // ................:::::111111.:.::
        // Reko: a decoder for V850 instruction EBFF at address 001014CE has not been implemented.
        [Test]
        public void V850Dis_EBFF()
        {
            AssertCode("@@@", "EBFF");
        }

        // ................:..::011000.:::.
        // ................:..::.::....:::. sld_b
        // ................:.::.110001.:::.
        // Reko: a decoder for V850 instruction 2EB6 at address 001014D2 has not been implemented.
        [Test]
        public void V850Dis_2EB6()
        {
            AssertCode("@@@", "2EB6");
        }

        // ................:.:.:011110:.:..
        // ................:.:.:.::::.:.:.. sst_b
        // ................::.:.110111....:
        // Reko: a decoder for V850 instruction E1D6 at address 001014D6 has not been implemented.
        [Test]
        public void V850Dis_E1D6()
        {
            AssertCode("@@@", "E1D6");
        }

        // .................:...010010::::.
        // Reko: a decoder for V850 instruction 5E42 at address 001014D8 has not been implemented.
        [Test]
        public void V850Dis_5E42()
        {
            AssertCode("@@@", "5E42");
        }

        // ................:::::011100.::.:
        // ................:::::.:::...::.: sst_b
        // ................:.:.:010010..:..
        // Reko: a decoder for V850 instruction 44AA at address 001014DC has not been implemented.
        [Test]
        public void V850Dis_44AA()
        {
            AssertCode("@@@", "44AA");
        }

        // ................:::.:111000:....
        // Reko: a decoder for V850 instruction 10EF at address 001014DE has not been implemented.
        [Test]
        public void V850Dis_10EF()
        {
            AssertCode("@@@", "10EF");
        }

        // ................:.:..101001.::..
        // Reko: a decoder for V850 instruction 2CA5 at address 001014E0 has not been implemented.
        [Test]
        public void V850Dis_2CA5()
        {
            AssertCode("@@@", "2CA5");
        }

        // .................::::101010.::::
        // Reko: a decoder for V850 instruction 4F7D at address 001014E2 has not been implemented.
        [Test]
        public void V850Dis_4F7D()
        {
            AssertCode("@@@", "4F7D");
        }

        // ................::..:000110.:.:.
        // Reko: a decoder for V850 instruction CAC8 at address 001014E4 has not been implemented.
        [Test]
        public void V850Dis_CAC8()
        {
            AssertCode("@@@", "CAC8");
        }

        // .................:.::110101::.::
        // Reko: a decoder for V850 instruction BB5E at address 001014E6 has not been implemented.
        [Test]
        public void V850Dis_BB5E()
        {
            AssertCode("@@@", "BB5E");
        }

        // .....................110101..:..
        // Reko: a decoder for V850 instruction A406 at address 001014E8 has not been implemented.
        [Test]
        public void V850Dis_A406()
        {
            AssertCode("@@@", "A406");
        }

        // ................:..::010001.:.:.
        // Reko: a decoder for V850 instruction 2A9A at address 001014EA has not been implemented.
        [Test]
        public void V850Dis_2A9A()
        {
            AssertCode("@@@", "2A9A");
        }

        // ................:.:.:110101:::::
        // Reko: a decoder for V850 instruction BFAE at address 001014EC has not been implemented.
        [Test]
        public void V850Dis_BFAE()
        {
            AssertCode("@@@", "BFAE");
        }

        // ...................::001110.....
        // Reko: a decoder for V850 instruction C019 at address 001014EE has not been implemented.
        [Test]
        public void V850Dis_C019()
        {
            AssertCode("@@@", "C019");
        }

        // ...................:.101101.::.:
        // Reko: a decoder for V850 instruction AD15 at address 001014F0 has not been implemented.
        [Test]
        public void V850Dis_AD15()
        {
            AssertCode("@@@", "AD15");
        }

        // .................:::.110101::..:
        // Reko: a decoder for V850 instruction B976 at address 001014F2 has not been implemented.
        [Test]
        public void V850Dis_B976()
        {
            AssertCode("@@@", "B976");
        }

        // .................:...101100.:...
        // Reko: a decoder for V850 instruction 8845 at address 001014F4 has not been implemented.
        [Test]
        public void V850Dis_8845()
        {
            AssertCode("@@@", "8845");
        }

        // ..................:.:110100.::::
        // Reko: a decoder for V850 instruction 8F2E at address 001014F6 has not been implemented.
        [Test]
        public void V850Dis_8F2E()
        {
            AssertCode("@@@", "8F2E");
        }

        // ................:.:..001011:...:
        // Reko: a decoder for V850 instruction 71A1 at address 001014F8 has not been implemented.
        [Test]
        public void V850Dis_71A1()
        {
            AssertCode("@@@", "71A1");
        }

        // ................:..::000001:.:::
        // ................:..::.....::.::: not
        // .....................100010::::.
        // .....................:...:.::::. sld_h
        // ................:.:::001111.::..
        // Reko: a decoder for V850 instruction ECB9 at address 001014FE has not been implemented.
        [Test]
        public void V850Dis_ECB9()
        {
            AssertCode("@@@", "ECB9");
        }

        // ................::.:.110001..:::
        // Reko: a decoder for V850 instruction 27D6 at address 00101500 has not been implemented.
        [Test]
        public void V850Dis_27D6()
        {
            AssertCode("@@@", "27D6");
        }

        // .................::.:101101..::.
        // Reko: a decoder for V850 instruction A66D at address 00101502 has not been implemented.
        [Test]
        public void V850Dis_A66D()
        {
            AssertCode("@@@", "A66D");
        }

        // .................::.:110001::.::
        // .................:...101001:.:.:
        // Reko: a decoder for V850 instruction 3545 at address 00101506 has not been implemented.
        [Test]
        public void V850Dis_3545()
        {
            AssertCode("@@@", "3545");
        }

        // ................::..:111111.....
        // Reko: a decoder for V850 instruction E0CF at address 00101508 has not been implemented.
        [Test]
        public void V850Dis_E0CF()
        {
            AssertCode("@@@", "E0CF");
        }

        // ................::..:101000:...:
        // ................::..::.:...:...1 Sld.w/Sst.w
        // ................::..::.:...:...: sst_w
        // .................:...111100....:
        // Reko: a decoder for V850 instruction 8147 at address 0010150C has not been implemented.
        [Test]
        public void V850Dis_8147()
        {
            AssertCode("@@@", "8147");
        }

        // ................:.::.000000:.:::
        // ................10110......:.:::
        // ................:.::.......:.::: mov
        // .................:::.011100....:
        // .................:::..:::......: sst_b
        // ..................:.:110010.::.:
        // Reko: a decoder for V850 instruction 4D2E at address 00101512 has not been implemented.
        [Test]
        public void V850Dis_4D2E()
        {
            AssertCode("@@@", "4D2E");
        }

        // ................:..::010111:...:
        // Reko: a decoder for V850 instruction F19A at address 00101514 has not been implemented.
        [Test]
        public void V850Dis_F19A()
        {
            AssertCode("@@@", "F19A");
        }

        // ................:::..110011..::.
        // Reko: a decoder for V850 instruction 66E6 at address 00101516 has not been implemented.
        [Test]
        public void V850Dis_66E6()
        {
            AssertCode("@@@", "66E6");
        }

        // ..................:..000000:.::.
        // ................00100......:.::.
        // ..................:........:.::. mov
        // ..................:.:000000.::..
        // ................00101.......::..
        // ..................:.:.......::.. mov
        // ..................:..010100:...:
        // Reko: a decoder for V850 instruction 9122 at address 0010151C has not been implemented.
        [Test]
        public void V850Dis_9122()
        {
            AssertCode("@@@", "9122");
        }

        // ................:.:..000000.....
        // ................10100...........
        // ................:.:............. mov
        // .................:::.001000::.:.
        // .................:::...:...::.:. or
        // ................:::.:000011:..:.
        // Reko: a decoder for V850 instruction 72E8 at address 00101522 has not been implemented.
        [Test]
        public void V850Dis_72E8()
        {
            AssertCode("@@@", "72E8");
        }

        // .................:.:.010000.:...
        // .................:.:..:....01000
        // .................:.:..:.....:... mov
        // ..................:::111110::::.
        // Reko: a decoder for V850 instruction DE3F at address 00101526 has not been implemented.
        [Test]
        public void V850Dis_DE3F()
        {
            AssertCode("@@@", "DE3F");
        }

        // .................:::.011111.....
        // .................:::..:::::..... sst_b
        // ................:.:::000100:.:.:
        // Reko: a decoder for V850 instruction 95B8 at address 0010152A has not been implemented.
        [Test]
        public void V850Dis_95B8()
        {
            AssertCode("@@@", "95B8");
        }

        // ................::...010001:.:..
        // Reko: a decoder for V850 instruction 34C2 at address 0010152C has not been implemented.
        [Test]
        public void V850Dis_34C2()
        {
            AssertCode("@@@", "34C2");
        }

        // ..................:.:010101.::.:
        // Reko: a decoder for V850 instruction AD2A at address 0010152E has not been implemented.
        [Test]
        public void V850Dis_AD2A()
        {
            AssertCode("@@@", "AD2A");
        }

        // ................:....110100.:.:.
        // Reko: a decoder for V850 instruction 8A86 at address 00101530 has not been implemented.
        [Test]
        public void V850Dis_8A86()
        {
            AssertCode("@@@", "8A86");
        }

        // ................:::.:011000:.::.
        // ................:::.:.::...:.::. sld_b
        // ................:.:::101101:.::.
        // Reko: a decoder for V850 instruction B6BD at address 00101534 has not been implemented.
        [Test]
        public void V850Dis_B6BD()
        {
            AssertCode("@@@", "B6BD");
        }

        // ................:...:101110..:::
        // Reko: a decoder for V850 instruction C78D at address 00101536 has not been implemented.
        [Test]
        public void V850Dis_C78D()
        {
            AssertCode("@@@", "C78D");
        }

        // ................:.:::111011:....
        // Reko: a decoder for V850 instruction 70BF at address 00101538 has not been implemented.
        [Test]
        public void V850Dis_70BF()
        {
            AssertCode("@@@", "70BF");
        }

        // ...................::010010:.:::
        // Reko: a decoder for V850 instruction 571A at address 0010153A has not been implemented.
        [Test]
        public void V850Dis_571A()
        {
            AssertCode("@@@", "571A");
        }

        // ....................:100011...:.
        // ....................::...::...:. sld_h
        // ................:.:::101101:.:..
        // Reko: a decoder for V850 instruction B4BD at address 0010153E has not been implemented.
        [Test]
        public void V850Dis_B4BD()
        {
            AssertCode("@@@", "B4BD");
        }

        // .................:::.001110.::::
        // Reko: a decoder for V850 instruction CF71 at address 00101540 has not been implemented.
        [Test]
        public void V850Dis_CF71()
        {
            AssertCode("@@@", "CF71");
        }

        // ....................:111101:..::
        // Reko: a decoder for V850 instruction B30F at address 00101542 has not been implemented.
        [Test]
        public void V850Dis_B30F()
        {
            AssertCode("@@@", "B30F");
        }

        // ..................:.:111110:.:..
        // Reko: a decoder for V850 instruction D42F at address 00101544 has not been implemented.
        [Test]
        public void V850Dis_D42F()
        {
            AssertCode("@@@", "D42F");
        }

        // ..................:.:111101.:...
        // Reko: a decoder for V850 instruction A82F at address 00101546 has not been implemented.
        [Test]
        public void V850Dis_A82F()
        {
            AssertCode("@@@", "A82F");
        }

        // ....................:101100:.:..
        // Reko: a decoder for V850 instruction 940D at address 00101548 has not been implemented.
        [Test]
        public void V850Dis_940D()
        {
            AssertCode("@@@", "940D");
        }

        // .................::::101010.:::.
        // Reko: a decoder for V850 instruction 4E7D at address 0010154A has not been implemented.
        [Test]
        public void V850Dis_4E7D()
        {
            AssertCode("@@@", "4E7D");
        }

        // ................:.:.:101000..:..
        // ................:.:.::.:.....:.0 Sld.w/Sst.w
        // ................:.:.::.:.....:.. sld_w
        // .................:.:.100101:..:.
        // .................:.:.:..:.::..:. sst_h
        // ................:::..001011:..::
        // Reko: a decoder for V850 instruction 73E1 at address 00101550 has not been implemented.
        [Test]
        public void V850Dis_73E1()
        {
            AssertCode("@@@", "73E1");
        }

        // ....................:010100:::.:
        // Reko: a decoder for V850 instruction 9D0A at address 00101552 has not been implemented.
        [Test]
        public void V850Dis_9D0A()
        {
            AssertCode("@@@", "9D0A");
        }

        // ...................:.101000.::..
        // ...................:.:.:....::.0 Sld.w/Sst.w
        // ...................:.:.:....::.. sld_w
        // .................:.::100111.:..:
        // .................:.:::..:::.:..: sst_h
        // ................:::..100101.:..:
        // ................:::..:..:.:.:..: sst_h
        // .................:..:000011:.:::
        // Reko: a decoder for V850 instruction 7748 at address 0010155A has not been implemented.
        [Test]
        public void V850Dis_7748()
        {
            AssertCode("@@@", "7748");
        }

        // ................::...100111::.:.
        // ................::...:..:::::.:. sst_h
        // ................::.:.011110:::.:
        // ................::.:..::::.:::.: sst_b
        // ................::.::000101:::::
        // Reko: a decoder for V850 instruction BFD8 at address 00101560 has not been implemented.
        [Test]
        public void V850Dis_BFD8()
        {
            AssertCode("@@@", "BFD8");
        }

        // ................::...011111.:..:
        // ................::....:::::.:..: sst_b
        // ................:..::110000.:::.
        // Reko: a decoder for V850 instruction 0E9E at address 00101564 has not been implemented.
        [Test]
        public void V850Dis_0E9E()
        {
            AssertCode("@@@", "0E9E");
        }

        // .................:.::010010:.:.:
        // Reko: a decoder for V850 instruction 555A at address 00101566 has not been implemented.
        [Test]
        public void V850Dis_555A()
        {
            AssertCode("@@@", "555A");
        }

        // ................:::..110110::::.
        // Reko: a decoder for V850 instruction DEE6 at address 00101568 has not been implemented.
        [Test]
        public void V850Dis_DEE6()
        {
            AssertCode("@@@", "DEE6");
        }

        // ................:.:.:001010:.::.
        // Reko: a decoder for V850 instruction 56A9 at address 0010156A has not been implemented.
        [Test]
        public void V850Dis_56A9()
        {
            AssertCode("@@@", "56A9");
        }

        // .................:..:010011.::::
        // Reko: a decoder for V850 instruction 6F4A at address 0010156C has not been implemented.
        [Test]
        public void V850Dis_6F4A()
        {
            AssertCode("@@@", "6F4A");
        }

        // ................::...011100.:..:
        // ................::....:::...:..: sst_b
        // .....................011000.::..
        // ......................::....::.. sld_b
        // ................:....000101..:::
        // Reko: a decoder for V850 instruction A780 at address 00101572 has not been implemented.
        [Test]
        public void V850Dis_A780()
        {
            AssertCode("@@@", "A780");
        }

        // ................:.:.:010011.:::.
        // Reko: a decoder for V850 instruction 6EAA at address 00101574 has not been implemented.
        [Test]
        public void V850Dis_6EAA()
        {
            AssertCode("@@@", "6EAA");
        }

        // ................:.:::001101:....
        // Reko: a decoder for V850 instruction B0B9 at address 00101576 has not been implemented.
        [Test]
        public void V850Dis_B0B9()
        {
            AssertCode("@@@", "B0B9");
        }

        // .................:.:.111100..:::
        // Reko: a decoder for V850 instruction 8757 at address 00101578 has not been implemented.
        [Test]
        public void V850Dis_8757()
        {
            AssertCode("@@@", "8757");
        }

        // ................::::.000011..:.:
        // .................:.::111101:.:::
        // Reko: a decoder for V850 instruction B75F at address 0010157C has not been implemented.
        [Test]
        public void V850Dis_B75F()
        {
            AssertCode("@@@", "B75F");
        }

        // ................::..:111001:.:..
        // Reko: a decoder for V850 instruction 34CF at address 0010157E has not been implemented.
        [Test]
        public void V850Dis_34CF()
        {
            AssertCode("@@@", "34CF");
        }

        // .................::::010001.::::
        // ..................:.:100101::::.
        // ..................:.::..:.:::::. sst_h
        // ................:::.:111110...::
        // Reko: a decoder for V850 instruction C3EF at address 00101584 has not been implemented.
        [Test]
        public void V850Dis_C3EF()
        {
            AssertCode("@@@", "C3EF");
        }

        // ................::.:.100010..:.:
        // ................::.:.:...:...:.: sld_h
        // .................:...011100.::..
        // .................:....:::...::.. sst_b
        // ..................:..101001.:.::
        // Reko: a decoder for V850 instruction 2B25 at address 0010158A has not been implemented.
        [Test]
        public void V850Dis_2B25()
        {
            AssertCode("@@@", "2B25");
        }

        // .................:...000101::::.
        // Reko: a decoder for V850 instruction BE40 at address 0010158C has not been implemented.
        [Test]
        public void V850Dis_BE40()
        {
            AssertCode("@@@", "BE40");
        }

        // .................:.::111111.::..
        // Reko: a decoder for V850 instruction EC5F at address 0010158E has not been implemented.
        [Test]
        public void V850Dis_EC5F()
        {
            AssertCode("@@@", "EC5F");
        }

        // ................:.::.100010::.::
        // ................:.::.:...:.::.:: sld_h
        // ................::::.110001::.::
        // Reko: a decoder for V850 instruction 3BF6 at address 00101592 has not been implemented.
        [Test]
        public void V850Dis_3BF6()
        {
            AssertCode("@@@", "3BF6");
        }

        // .................:.::101000:::.:
        // .................:.:::.:...:::.1 Sld.w/Sst.w
        // .................:.:::.:...:::.: sst_w
        // ...................:.100011..:..
        // ...................:.:...::..:.. sld_h
        // ..................:.:111001..:::
        // Reko: a decoder for V850 instruction 272F at address 00101598 has not been implemented.
        [Test]
        public void V850Dis_272F()
        {
            AssertCode("@@@", "272F");
        }

        // ................:.::.110001::.::
        // Reko: a decoder for V850 instruction 3BB6 at address 0010159A has not been implemented.
        [Test]
        public void V850Dis_3BB6()
        {
            AssertCode("@@@", "3BB6");
        }

        // .................::..101001::...
        // Reko: a decoder for V850 instruction 3865 at address 0010159C has not been implemented.
        [Test]
        public void V850Dis_3865()
        {
            AssertCode("@@@", "3865");
        }

        // .................::::110101...::
        // Reko: a decoder for V850 instruction A37E at address 0010159E has not been implemented.
        [Test]
        public void V850Dis_A37E()
        {
            AssertCode("@@@", "A37E");
        }

        // ................:....110101:..:.
        // Reko: a decoder for V850 instruction B286 at address 001015A0 has not been implemented.
        [Test]
        public void V850Dis_B286()
        {
            AssertCode("@@@", "B286");
        }

        // ................::::.100011::..:
        // ................::::.:...::::..: sld_h
        // .................:.::101100.::..
        // Reko: a decoder for V850 instruction 8C5D at address 001015A4 has not been implemented.
        [Test]
        public void V850Dis_8C5D()
        {
            AssertCode("@@@", "8C5D");
        }

        // ...................::101111:::::
        // Reko: a decoder for V850 instruction FF1D at address 001015A6 has not been implemented.
        [Test]
        public void V850Dis_FF1D()
        {
            AssertCode("@@@", "FF1D");
        }

        // ................::.:.010101::::.
        // Reko: a decoder for V850 instruction BED2 at address 001015A8 has not been implemented.
        [Test]
        public void V850Dis_BED2()
        {
            AssertCode("@@@", "BED2");
        }

        // ..................:.:011111:.:..
        // ..................:.:.::::::.:.. sst_b
        // ................:...:010011::::.
        // Reko: a decoder for V850 instruction 7E8A at address 001015AC has not been implemented.
        [Test]
        public void V850Dis_7E8A()
        {
            AssertCode("@@@", "7E8A");
        }

        // .................:...111111::.:.
        // Reko: a decoder for V850 instruction FA47 at address 001015AE has not been implemented.
        [Test]
        public void V850Dis_FA47()
        {
            AssertCode("@@@", "FA47");
        }

        // .....................110011.:.:.
        // Reko: a decoder for V850 instruction 6A06 at address 001015B0 has not been implemented.
        [Test]
        public void V850Dis_6A06()
        {
            AssertCode("@@@", "6A06");
        }

        // .....................011100:.:.:
        // ......................:::..:.:.: sst_b
        // ..................:::101001..:..
        // Reko: a decoder for V850 instruction 243D at address 001015B4 has not been implemented.
        [Test]
        public void V850Dis_243D()
        {
            AssertCode("@@@", "243D");
        }

        // ................::.:.010101:..:.
        // Reko: a decoder for V850 instruction B2D2 at address 001015B6 has not been implemented.
        [Test]
        public void V850Dis_B2D2()
        {
            AssertCode("@@@", "B2D2");
        }

        // ................:.:.:001000:.::.
        // ................:.:.:..:...:.::. or
        // ................:..:.011000..:..
        // ................:..:..::.....:.. sld_b
        // ................:...:110101:..::
        // Reko: a decoder for V850 instruction B38E at address 001015BC has not been implemented.
        [Test]
        public void V850Dis_B38E()
        {
            AssertCode("@@@", "B38E");
        }

        // ..................:..110100::.::
        // Reko: a decoder for V850 instruction 9B26 at address 001015BE has not been implemented.
        [Test]
        public void V850Dis_9B26()
        {
            AssertCode("@@@", "9B26");
        }

        // ................:.:.:101100:::::
        // Reko: a decoder for V850 instruction 9FAD at address 001015C0 has not been implemented.
        [Test]
        public void V850Dis_9FAD()
        {
            AssertCode("@@@", "9FAD");
        }

        // ..................:::000011:..::
        // Reko: a decoder for V850 instruction 7338 at address 001015C2 has not been implemented.
        [Test]
        public void V850Dis_7338()
        {
            AssertCode("@@@", "7338");
        }

        // ................:::::000110..::.
        // Reko: a decoder for V850 instruction C6F8 at address 001015C4 has not been implemented.
        [Test]
        public void V850Dis_C6F8()
        {
            AssertCode("@@@", "C6F8");
        }

        // ................:::::110110:..::
        // Reko: a decoder for V850 instruction D3FE at address 001015C6 has not been implemented.
        [Test]
        public void V850Dis_D3FE()
        {
            AssertCode("@@@", "D3FE");
        }

        // ...................::110101:.:.:
        // Reko: a decoder for V850 instruction B51E at address 001015C8 has not been implemented.
        [Test]
        public void V850Dis_B51E()
        {
            AssertCode("@@@", "B51E");
        }

        // .................:..:011001::...
        // .................:..:.::..:::... sld_b
        // ..................:::000011.:...
        // Reko: a decoder for V850 instruction 6838 at address 001015CC has not been implemented.
        [Test]
        public void V850Dis_6838()
        {
            AssertCode("@@@", "6838");
        }

        // ................:..::100100:.::.
        // ................:..:::..:..:.::. sst_h
        // ................::::.100001:.:..
        // ................::::.:....::.:.. sld_h
        // ..................::.010010.::..
        // Reko: a decoder for V850 instruction 4C32 at address 001015D2 has not been implemented.
        [Test]
        public void V850Dis_4C32()
        {
            AssertCode("@@@", "4C32");
        }

        // ................:.:..000110.::::
        // Reko: a decoder for V850 instruction CFA0 at address 001015D4 has not been implemented.
        [Test]
        public void V850Dis_CFA0()
        {
            AssertCode("@@@", "CFA0");
        }

        // .................:::.100010::.:.
        // .................:::.:...:.::.:. sld_h
        // ................:.:..111110:.:..
        // Reko: a decoder for V850 instruction D4A7 at address 001015D8 has not been implemented.
        [Test]
        public void V850Dis_D4A7()
        {
            AssertCode("@@@", "D4A7");
        }

        // ................:::..010110::::.
        // Reko: a decoder for V850 instruction DEE2 at address 001015DA has not been implemented.
        [Test]
        public void V850Dis_DEE2()
        {
            AssertCode("@@@", "DEE2");
        }

        // .................::.:011010::...
        // .................::.:.::.:.::... sld_b
        // ..................:.:111011.:..:
        // Reko: a decoder for V850 instruction 692F at address 001015DE has not been implemented.
        [Test]
        public void V850Dis_692F()
        {
            AssertCode("@@@", "692F");
        }

        // ................::..:101001:.::.
        // Reko: a decoder for V850 instruction 36CD at address 001015E0 has not been implemented.
        [Test]
        public void V850Dis_36CD()
        {
            AssertCode("@@@", "36CD");
        }

        // ................::.:.001100.:.::
        // Reko: a decoder for V850 instruction 8BD1 at address 001015E2 has not been implemented.
        [Test]
        public void V850Dis_8BD1()
        {
            AssertCode("@@@", "8BD1");
        }

        // ................::...010000:.:::
        // ................::....:....10111
        // ................::....:....:.::: mov
        // .................:::.010100.....
        // Reko: a decoder for V850 instruction 8072 at address 001015E6 has not been implemented.
        [Test]
        public void V850Dis_8072()
        {
            AssertCode("@@@", "8072");
        }

        // ................:..:.111001:..:.
        // Reko: a decoder for V850 instruction 3297 at address 001015E8 has not been implemented.
        [Test]
        public void V850Dis_3297()
        {
            AssertCode("@@@", "3297");
        }

        // .....................110010.::.:
        // Reko: a decoder for V850 instruction 4D06 at address 001015EA has not been implemented.
        [Test]
        public void V850Dis_4D06()
        {
            AssertCode("@@@", "4D06");
        }

        // .................::..111000..:.:
        // Reko: a decoder for V850 instruction 0567 at address 001015EC has not been implemented.
        [Test]
        public void V850Dis_0567()
        {
            AssertCode("@@@", "0567");
        }

        // ................::.::101011.:..:
        // Reko: a decoder for V850 instruction 69DD at address 001015EE has not been implemented.
        [Test]
        public void V850Dis_69DD()
        {
            AssertCode("@@@", "69DD");
        }

        // .................::.:000000:..::
        // ................01101......:..::
        // .................::.:......:..:: mov
        // ................:.:::111010::::.
        // Reko: a decoder for V850 instruction 5EBF at address 001015F2 has not been implemented.
        [Test]
        public void V850Dis_5EBF()
        {
            AssertCode("@@@", "5EBF");
        }

        // .................::.:001010...:.
        // Reko: a decoder for V850 instruction 4269 at address 001015F4 has not been implemented.
        [Test]
        public void V850Dis_4269()
        {
            AssertCode("@@@", "4269");
        }

        // ................:::::111010:.:::
        // Reko: a decoder for V850 instruction 57FF at address 001015F6 has not been implemented.
        [Test]
        public void V850Dis_57FF()
        {
            AssertCode("@@@", "57FF");
        }

        // ................:::::111010:..:.
        // ...................::111011...::
        // Reko: a decoder for V850 instruction 631F at address 001015FA has not been implemented.
        [Test]
        public void V850Dis_631F()
        {
            AssertCode("@@@", "631F");
        }

        // ................::::.000000::..:
        // ................11110......::..:
        // ................::::.......::..: mov
        // .................:..:001101..::.
        // Reko: a decoder for V850 instruction A649 at address 001015FE has not been implemented.
        [Test]
        public void V850Dis_A649()
        {
            AssertCode("@@@", "A649");
        }

        // ....................:001101:.:..
        // Reko: a decoder for V850 instruction B409 at address 00101600 has not been implemented.
        [Test]
        public void V850Dis_B409()
        {
            AssertCode("@@@", "B409");
        }

        // ................:..::011000:.:..
        // ................:..::.::...:.:.. sld_b
        // ................::...100101.::.:
        // ................::...:..:.:.::.: sst_h
        // .................:::.001100..:.:
        // Reko: a decoder for V850 instruction 8571 at address 00101606 has not been implemented.
        [Test]
        public void V850Dis_8571()
        {
            AssertCode("@@@", "8571");
        }

        // ................::.::001011:..::
        // Reko: a decoder for V850 instruction 73D9 at address 00101608 has not been implemented.
        [Test]
        public void V850Dis_73D9()
        {
            AssertCode("@@@", "73D9");
        }

        // ..................:::000111:::::
        // Reko: a decoder for V850 instruction FF38 at address 0010160A has not been implemented.
        [Test]
        public void V850Dis_FF38()
        {
            AssertCode("@@@", "FF38");
        }

        // .................:.:.100010:..:.
        // .................:.:.:...:.:..:. sld_h
        // .................::.:101011.::::
        // Reko: a decoder for V850 instruction 6F6D at address 0010160E has not been implemented.
        [Test]
        public void V850Dis_6F6D()
        {
            AssertCode("@@@", "6F6D");
        }

        // .....................001001:::.:
        // Reko: a decoder for V850 instruction 3D01 at address 00101610 has not been implemented.
        [Test]
        public void V850Dis_3D01()
        {
            AssertCode("@@@", "3D01");
        }

        // ................:::.:111110.::::
        // Reko: a decoder for V850 instruction CFEF at address 00101612 has not been implemented.
        [Test]
        public void V850Dis_CFEF()
        {
            AssertCode("@@@", "CFEF");
        }

        // ................:.:.:010000.::..
        // ................:.:.:.:....01100
        // ................:.:.:.:.....::.. mov
        // ................::..:101111.::.:
        // Reko: a decoder for V850 instruction EDCD at address 00101616 has not been implemented.
        [Test]
        public void V850Dis_EDCD()
        {
            AssertCode("@@@", "EDCD");
        }

        // ................:...:100011:.::.
        // ................:...::...:::.::. sld_h
        // ...................:.111011::::.
        // Reko: a decoder for V850 instruction 7E17 at address 0010161A has not been implemented.
        [Test]
        public void V850Dis_7E17()
        {
            AssertCode("@@@", "7E17");
        }

        // .................::..001010:::::
        // .................:...010010::..:
        // Reko: a decoder for V850 instruction 5942 at address 0010161E has not been implemented.
        [Test]
        public void V850Dis_5942()
        {
            AssertCode("@@@", "5942");
        }

        // .....................100111:...:
        // .....................:..::::...: sst_h
        // ................:::::100010:...:
        // ................::::::...:.:...: sld_h
        // ................::..:110010:..:.
        // Reko: a decoder for V850 instruction 52CE at address 00101624 has not been implemented.
        [Test]
        public void V850Dis_52CE()
        {
            AssertCode("@@@", "52CE");
        }

        // .................:..:110001.::::
        // Reko: a decoder for V850 instruction 2F4E at address 00101626 has not been implemented.
        [Test]
        public void V850Dis_2F4E()
        {
            AssertCode("@@@", "2F4E");
        }

        // ................:::.:100111...::
        // ................:::.::..:::...:: sst_h
        // .................::.:000010.:.::
        // ................01101....:..:.::
        // .................::.:....:.01011
        // .................::.:....:..:.:: divh
        // ...................::110011.:..:
        // Reko: a decoder for V850 instruction 691E at address 0010162C has not been implemented.
        [Test]
        public void V850Dis_691E()
        {
            AssertCode("@@@", "691E");
        }

        // ................:::::110101.::.:
        // Reko: a decoder for V850 instruction ADFE at address 0010162E has not been implemented.
        [Test]
        public void V850Dis_ADFE()
        {
            AssertCode("@@@", "ADFE");
        }

        // ................::::.110100:....
        // Reko: a decoder for V850 instruction 90F6 at address 00101630 has not been implemented.
        [Test]
        public void V850Dis_90F6()
        {
            AssertCode("@@@", "90F6");
        }

        // ...................:.111111...:.
        // Reko: a decoder for V850 instruction E217 at address 00101632 has not been implemented.
        [Test]
        public void V850Dis_E217()
        {
            AssertCode("@@@", "E217");
        }

        // ................:.::.110001....:
        // ....................:010001:::..
        // Reko: a decoder for V850 instruction 3C0A at address 00101636 has not been implemented.
        [Test]
        public void V850Dis_3C0A()
        {
            AssertCode("@@@", "3C0A");
        }

        // .................:...111000::.::
        // Reko: a decoder for V850 instruction 1B47 at address 00101638 has not been implemented.
        [Test]
        public void V850Dis_1B47()
        {
            AssertCode("@@@", "1B47");
        }

        // ..................:..001001..:.:
        // Reko: a decoder for V850 instruction 2521 at address 0010163A has not been implemented.
        [Test]
        public void V850Dis_2521()
        {
            AssertCode("@@@", "2521");
        }

        // .................::.:101010..::.
        // Reko: a decoder for V850 instruction 466D at address 0010163C has not been implemented.
        [Test]
        public void V850Dis_466D()
        {
            AssertCode("@@@", "466D");
        }

        // ...................:.010000:..:.
        // ...................:..:....10010
        // ...................:..:....:..:. mov
        // ................:.:::101100.....
        // Reko: a decoder for V850 instruction 80BD at address 00101640 has not been implemented.
        [Test]
        public void V850Dis_80BD()
        {
            AssertCode("@@@", "80BD");
        }

        // .................:..:101010..:::
        // Reko: a decoder for V850 instruction 474D at address 00101642 has not been implemented.
        [Test]
        public void V850Dis_474D()
        {
            AssertCode("@@@", "474D");
        }

        // .................:::.010000...:.
        // .................:::..:....00010
        // .................:::..:.......:. mov
        // .................::.:111000::.:.
        // Reko: a decoder for V850 instruction 1A6F at address 00101646 has not been implemented.
        [Test]
        public void V850Dis_1A6F()
        {
            AssertCode("@@@", "1A6F");
        }

        // ................:....000110:...:
        // Reko: a decoder for V850 instruction D180 at address 00101648 has not been implemented.
        [Test]
        public void V850Dis_D180()
        {
            AssertCode("@@@", "D180");
        }

        // ...................::111000.:.::
        // Reko: a decoder for V850 instruction 0B1F at address 0010164A has not been implemented.
        [Test]
        public void V850Dis_0B1F()
        {
            AssertCode("@@@", "0B1F");
        }

        // ................:....100010::::.
        // ................:....:...:.::::. sld_h
        // .................:.:.111011..:..
        // Reko: a decoder for V850 instruction 6457 at address 0010164E has not been implemented.
        [Test]
        public void V850Dis_6457()
        {
            AssertCode("@@@", "6457");
        }

        // .................::::111110::::.
        // Reko: a decoder for V850 instruction DE7F at address 00101650 has not been implemented.
        [Test]
        public void V850Dis_DE7F()
        {
            AssertCode("@@@", "DE7F");
        }

        // .................::.:001100..:::
        // Reko: a decoder for V850 instruction 8769 at address 00101652 has not been implemented.
        [Test]
        public void V850Dis_8769()
        {
            AssertCode("@@@", "8769");
        }

        // .................:...001011::::.
        // Reko: a decoder for V850 instruction 7E41 at address 00101654 has not been implemented.
        [Test]
        public void V850Dis_7E41()
        {
            AssertCode("@@@", "7E41");
        }

        // ..................:::010001.....
        // Reko: a decoder for V850 instruction 203A at address 00101656 has not been implemented.
        [Test]
        public void V850Dis_203A()
        {
            AssertCode("@@@", "203A");
        }

        // .................:...111110..:::
        // Reko: a decoder for V850 instruction C747 at address 00101658 has not been implemented.
        [Test]
        public void V850Dis_C747()
        {
            AssertCode("@@@", "C747");
        }

        // ....................:011111.....
        // ....................:.:::::..... sst_b
        // ....................:110101.:...
        // Reko: a decoder for V850 instruction A80E at address 0010165C has not been implemented.
        [Test]
        public void V850Dis_A80E()
        {
            AssertCode("@@@", "A80E");
        }

        // .................:::.110000.:...
        // Reko: a decoder for V850 instruction 0876 at address 0010165E has not been implemented.
        [Test]
        public void V850Dis_0876()
        {
            AssertCode("@@@", "0876");
        }

        // ..................:::001110::.:.
        // Reko: a decoder for V850 instruction DA39 at address 00101660 has not been implemented.
        [Test]
        public void V850Dis_DA39()
        {
            AssertCode("@@@", "DA39");
        }

        // ................:.:.:011111..:::
        // ................:.:.:.:::::..::: sst_b
        // .................:.::111110:.::.
        // Reko: a decoder for V850 instruction D65F at address 00101664 has not been implemented.
        [Test]
        public void V850Dis_D65F()
        {
            AssertCode("@@@", "D65F");
        }

        // ...................::101111:::..
        // Reko: a decoder for V850 instruction FC1D at address 00101666 has not been implemented.
        [Test]
        public void V850Dis_FC1D()
        {
            AssertCode("@@@", "FC1D");
        }

        // .................:::.010110::.::
        // Reko: a decoder for V850 instruction DB72 at address 00101668 has not been implemented.
        [Test]
        public void V850Dis_DB72()
        {
            AssertCode("@@@", "DB72");
        }

        // ................:.:..001010..:.:
        // Reko: a decoder for V850 instruction 45A1 at address 0010166A has not been implemented.
        [Test]
        public void V850Dis_45A1()
        {
            AssertCode("@@@", "45A1");
        }

        // ...................::101101.:.:.
        // Reko: a decoder for V850 instruction AA1D at address 0010166C has not been implemented.
        [Test]
        public void V850Dis_AA1D()
        {
            AssertCode("@@@", "AA1D");
        }

        // ................:..:.111101.::..
        // Reko: a decoder for V850 instruction AC97 at address 0010166E has not been implemented.
        [Test]
        public void V850Dis_AC97()
        {
            AssertCode("@@@", "AC97");
        }

        // ................::..:101111...:.
        // Reko: a decoder for V850 instruction E2CD at address 00101670 has not been implemented.
        [Test]
        public void V850Dis_E2CD()
        {
            AssertCode("@@@", "E2CD");
        }

        // ................::...110010...:.
        // Reko: a decoder for V850 instruction 42C6 at address 00101672 has not been implemented.
        [Test]
        public void V850Dis_42C6()
        {
            AssertCode("@@@", "42C6");
        }

        // ................:.:.:001111..:.:
        // Reko: a decoder for V850 instruction E5A9 at address 00101674 has not been implemented.
        [Test]
        public void V850Dis_E5A9()
        {
            AssertCode("@@@", "E5A9");
        }

        // ..................:::111100:..::
        // Reko: a decoder for V850 instruction 933F at address 00101676 has not been implemented.
        [Test]
        public void V850Dis_933F()
        {
            AssertCode("@@@", "933F");
        }

        // ....................:100100:::.:
        // ....................::..:..:::.: sst_h
        // ................:.:::010100..::.
        // Reko: a decoder for V850 instruction 86BA at address 0010167A has not been implemented.
        [Test]
        public void V850Dis_86BA()
        {
            AssertCode("@@@", "86BA");
        }

        // ....................:111100:..:.
        // Reko: a decoder for V850 instruction 920F at address 0010167C has not been implemented.
        [Test]
        public void V850Dis_920F()
        {
            AssertCode("@@@", "920F");
        }

        // ................::...011110..::.
        // ................::....::::...::. sst_b
        // ................:..:.101011:.:::
        // Reko: a decoder for V850 instruction 7795 at address 00101680 has not been implemented.
        [Test]
        public void V850Dis_7795()
        {
            AssertCode("@@@", "7795");
        }

        // ..................:..100010..:.:
        // ..................:..:...:...:.: sld_h
        // ................:.:::001100:::..
        // Reko: a decoder for V850 instruction 9CB9 at address 00101684 has not been implemented.
        [Test]
        public void V850Dis_9CB9()
        {
            AssertCode("@@@", "9CB9");
        }

        // .................::::110100....:
        // Reko: a decoder for V850 instruction 817E at address 00101686 has not been implemented.
        [Test]
        public void V850Dis_817E()
        {
            AssertCode("@@@", "817E");
        }

        // .................::.:010100...::
        // Reko: a decoder for V850 instruction 836A at address 00101688 has not been implemented.
        [Test]
        public void V850Dis_836A()
        {
            AssertCode("@@@", "836A");
        }

        // ....................:100100.:::.
        // ....................::..:...:::. sst_h
        // .................::::111111::.::
        // Reko: a decoder for V850 instruction FB7F at address 0010168C has not been implemented.
        [Test]
        public void V850Dis_FB7F()
        {
            AssertCode("@@@", "FB7F");
        }

        // ................:.:.:010100.::::
        // Reko: a decoder for V850 instruction 8FAA at address 0010168E has not been implemented.
        [Test]
        public void V850Dis_8FAA()
        {
            AssertCode("@@@", "8FAA");
        }

        // .................:..:000100::.::
        // Reko: a decoder for V850 instruction 9B48 at address 00101690 has not been implemented.
        [Test]
        public void V850Dis_9B48()
        {
            AssertCode("@@@", "9B48");
        }

        // ................::.::011001.:...
        // ................::.::.::..:.:... sld_b
        // .................:...010110.::..
        // Reko: a decoder for V850 instruction CC42 at address 00101694 has not been implemented.
        [Test]
        public void V850Dis_CC42()
        {
            AssertCode("@@@", "CC42");
        }

        // ..................::.000100:...:
        // Reko: a decoder for V850 instruction 9130 at address 00101696 has not been implemented.
        [Test]
        public void V850Dis_9130()
        {
            AssertCode("@@@", "9130");
        }

        // ................:.:.:000101..:.:
        // Reko: a decoder for V850 instruction A5A8 at address 00101698 has not been implemented.
        [Test]
        public void V850Dis_A5A8()
        {
            AssertCode("@@@", "A5A8");
        }

        // ................::..:001000.::.:
        // ................::..:..:....::.: or
        // .................:...001110::..:
        // Reko: a decoder for V850 instruction D941 at address 0010169C has not been implemented.
        [Test]
        public void V850Dis_D941()
        {
            AssertCode("@@@", "D941");
        }

        // ................::..:011100.:.:.
        // ................::..:.:::...:.:. sst_b
        // ................:....010101.::..
        // Reko: a decoder for V850 instruction AC82 at address 001016A0 has not been implemented.
        [Test]
        public void V850Dis_AC82()
        {
            AssertCode("@@@", "AC82");
        }

        // ...................:.101001.:..:
        // Reko: a decoder for V850 instruction 2915 at address 001016A2 has not been implemented.
        [Test]
        public void V850Dis_2915()
        {
            AssertCode("@@@", "2915");
        }

        // ................:....111110:.:::
        // Reko: a decoder for V850 instruction D787 at address 001016A4 has not been implemented.
        [Test]
        public void V850Dis_D787()
        {
            AssertCode("@@@", "D787");
        }

        // ..................:.:001111::.:.
        // Reko: a decoder for V850 instruction FA29 at address 001016A6 has not been implemented.
        [Test]
        public void V850Dis_FA29()
        {
            AssertCode("@@@", "FA29");
        }

        // ................::...011010.::..
        // ................::....::.:..::.. sld_b
        // .................::::011010...:.
        // .................::::.::.:....:. sld_b
        // ................:....111000::.::
        // Reko: a decoder for V850 instruction 1B87 at address 001016AC has not been implemented.
        [Test]
        public void V850Dis_1B87()
        {
            AssertCode("@@@", "1B87");
        }

        // ...................::110010...::
        // Reko: a decoder for V850 instruction 431E at address 001016AE has not been implemented.
        [Test]
        public void V850Dis_431E()
        {
            AssertCode("@@@", "431E");
        }

        // ................::.::011011:::..
        // ................::.::.::.:::::.. sld_b
        // .................:..:001000:::::
        // .................:..:..:...::::: or
        // ................:::.:010111..:::
        // Reko: a decoder for V850 instruction E7EA at address 001016B4 has not been implemented.
        [Test]
        public void V850Dis_E7EA()
        {
            AssertCode("@@@", "E7EA");
        }

        // ..................::.100111:.:::
        // ..................::.:..::::.::: sst_h
        // ...................:.100000..:..
        // ...................:.:.......:.. sld_h
        // ................::::.011011::::.
        // ................::::..::.::::::. sld_b
        // ................:.::.111101.:.::
        // Reko: a decoder for V850 instruction ABB7 at address 001016BC has not been implemented.
        [Test]
        public void V850Dis_ABB7()
        {
            AssertCode("@@@", "ABB7");
        }

        // .................:...001101..:.:
        // Reko: a decoder for V850 instruction A541 at address 001016BE has not been implemented.
        [Test]
        public void V850Dis_A541()
        {
            AssertCode("@@@", "A541");
        }

        // ................::...010110:::::
        // Reko: a decoder for V850 instruction DFC2 at address 001016C0 has not been implemented.
        [Test]
        public void V850Dis_DFC2()
        {
            AssertCode("@@@", "DFC2");
        }

        // .................::::001010:::::
        // Reko: a decoder for V850 instruction 5F79 at address 001016C2 has not been implemented.
        [Test]
        public void V850Dis_5F79()
        {
            AssertCode("@@@", "5F79");
        }

        // .................::::010010:.:.:
        // Reko: a decoder for V850 instruction 557A at address 001016C4 has not been implemented.
        [Test]
        public void V850Dis_557A()
        {
            AssertCode("@@@", "557A");
        }

        // ...................:.011110:..:.
        // ...................:..::::.:..:. sst_b
        // ................:::::110010.:::.
        // Reko: a decoder for V850 instruction 4EFE at address 001016C8 has not been implemented.
        [Test]
        public void V850Dis_4EFE()
        {
            AssertCode("@@@", "4EFE");
        }

        // ....................:000100.::::
        // Reko: a decoder for V850 instruction 8F08 at address 001016CA has not been implemented.
        [Test]
        public void V850Dis_8F08()
        {
            AssertCode("@@@", "8F08");
        }

        // ...................:.101111:::::
        // Reko: a decoder for V850 instruction FF15 at address 001016CC has not been implemented.
        [Test]
        public void V850Dis_FF15()
        {
            AssertCode("@@@", "FF15");
        }

        // ...................::110000:.:.:
        // Reko: a decoder for V850 instruction 151E at address 001016CE has not been implemented.
        [Test]
        public void V850Dis_151E()
        {
            AssertCode("@@@", "151E");
        }

        // ................:::.:111011..:.:
        // Reko: a decoder for V850 instruction 65EF at address 001016D0 has not been implemented.
        [Test]
        public void V850Dis_65EF()
        {
            AssertCode("@@@", "65EF");
        }

        // ................:::::101010.::..
        // Reko: a decoder for V850 instruction 4CFD at address 001016D2 has not been implemented.
        [Test]
        public void V850Dis_4CFD()
        {
            AssertCode("@@@", "4CFD");
        }

        // .................:...001001...:.
        // Reko: a decoder for V850 instruction 2241 at address 001016D4 has not been implemented.
        [Test]
        public void V850Dis_2241()
        {
            AssertCode("@@@", "2241");
        }

        // ................::...001111..:..
        // Reko: a decoder for V850 instruction E4C1 at address 001016D6 has not been implemented.
        [Test]
        public void V850Dis_E4C1()
        {
            AssertCode("@@@", "E4C1");
        }

        // ................::::.100100.:.::
        // ................::::.:..:...:.:: sst_h
        // ................::...011000:....
        // ................::....::...:.... sld_b
        // .....................110000.:...
        // Reko: a decoder for V850 instruction 0806 at address 001016DC has not been implemented.
        [Test]
        public void V850Dis_0806()
        {
            AssertCode("@@@", "0806");
        }

        // ................:..:.100011::::.
        // ................:..:.:...::::::. sld_h
        // .....................001000::::.
        // .......................:...::::. or
        // .................:.::000100::.::
        // Reko: a decoder for V850 instruction 9B58 at address 001016E2 has not been implemented.
        [Test]
        public void V850Dis_9B58()
        {
            AssertCode("@@@", "9B58");
        }

        // ................::..:010001..:::
        // Reko: a decoder for V850 instruction 27CA at address 001016E4 has not been implemented.
        [Test]
        public void V850Dis_27CA()
        {
            AssertCode("@@@", "27CA");
        }

        // .....................010110.:..:
        // Reko: a decoder for V850 instruction C902 at address 001016E6 has not been implemented.
        [Test]
        public void V850Dis_C902()
        {
            AssertCode("@@@", "C902");
        }

        // ....................:100000.:...
        // ....................::......:... sld_h
        // ................:::.:000111::.::
        // Reko: a decoder for V850 instruction FBE8 at address 001016EA has not been implemented.
        [Test]
        public void V850Dis_FBE8()
        {
            AssertCode("@@@", "FBE8");
        }

        // ................:::.:110010::.:.
        // Reko: a decoder for V850 instruction 5AEE at address 001016EC has not been implemented.
        [Test]
        public void V850Dis_5AEE()
        {
            AssertCode("@@@", "5AEE");
        }

        // ................:::.:110001:.:.:
        // Reko: a decoder for V850 instruction 35EE at address 001016EE has not been implemented.
        [Test]
        public void V850Dis_35EE()
        {
            AssertCode("@@@", "35EE");
        }

        // .................::..000111::::.
        // Reko: a decoder for V850 instruction FE60 at address 001016F0 has not been implemented.
        [Test]
        public void V850Dis_FE60()
        {
            AssertCode("@@@", "FE60");
        }

        // ................:.:::011100.:::.
        // ................:.:::.:::...:::. sst_b
        // ................:.:..111011.:.:.
        // Reko: a decoder for V850 instruction 6AA7 at address 001016F4 has not been implemented.
        [Test]
        public void V850Dis_6AA7()
        {
            AssertCode("@@@", "6AA7");
        }

        // ................:..::110000:::::
        // Reko: a decoder for V850 instruction 1F9E at address 001016F6 has not been implemented.
        [Test]
        public void V850Dis_1F9E()
        {
            AssertCode("@@@", "1F9E");
        }

        // ................:..::101110::::.
        // Reko: a decoder for V850 instruction DE9D at address 001016F8 has not been implemented.
        [Test]
        public void V850Dis_DE9D()
        {
            AssertCode("@@@", "DE9D");
        }

        // .................::..000111.::.:
        // Reko: a decoder for V850 instruction ED60 at address 001016FA has not been implemented.
        [Test]
        public void V850Dis_ED60()
        {
            AssertCode("@@@", "ED60");
        }

        // ....................:111011.:..:
        // Reko: a decoder for V850 instruction 690F at address 001016FC has not been implemented.
        [Test]
        public void V850Dis_690F()
        {
            AssertCode("@@@", "690F");
        }

        // .................:...111000.:.:.
        // Reko: a decoder for V850 instruction 0A47 at address 001016FE has not been implemented.
        [Test]
        public void V850Dis_0A47()
        {
            AssertCode("@@@", "0A47");
        }

        // ................:::::010011:::::
        // Reko: a decoder for V850 instruction 7FFA at address 00101700 has not been implemented.
        [Test]
        public void V850Dis_7FFA()
        {
            AssertCode("@@@", "7FFA");
        }

        // ................:::::110111.:.:.
        // Reko: a decoder for V850 instruction EAFE at address 00101702 has not been implemented.
        [Test]
        public void V850Dis_EAFE()
        {
            AssertCode("@@@", "EAFE");
        }

        // ................:.:::100011::..:
        // ................:.::::...::::..: sld_h
        // ................:..::011111:.::.
        // ................:..::.::::::.::. sst_b
        // .................:..:010001..::.
        // Reko: a decoder for V850 instruction 264A at address 00101708 has not been implemented.
        [Test]
        public void V850Dis_264A()
        {
            AssertCode("@@@", "264A");
        }

        // ...................:.101111:.:.:
        // Reko: a decoder for V850 instruction F515 at address 0010170A has not been implemented.
        [Test]
        public void V850Dis_F515()
        {
            AssertCode("@@@", "F515");
        }

        // ................:::::011010..:.:
        // ................:::::.::.:...:.: sld_b
        // ................:..::100011..:::
        // ................:..:::...::..::: sld_h
        // ..................:..010101:::::
        // Reko: a decoder for V850 instruction BF22 at address 00101710 has not been implemented.
        [Test]
        public void V850Dis_BF22()
        {
            AssertCode("@@@", "BF22");
        }

        // ................:.:..111110..:..
        // Reko: a decoder for V850 instruction C4A7 at address 00101712 has not been implemented.
        [Test]
        public void V850Dis_C4A7()
        {
            AssertCode("@@@", "C4A7");
        }

        // ................::...010011::...
        // Reko: a decoder for V850 instruction 78C2 at address 00101714 has not been implemented.
        [Test]
        public void V850Dis_78C2()
        {
            AssertCode("@@@", "78C2");
        }

        // .................:::.110001.:.:.
        // Reko: a decoder for V850 instruction 2A76 at address 00101716 has not been implemented.
        [Test]
        public void V850Dis_2A76()
        {
            AssertCode("@@@", "2A76");
        }

        // ................:::::101111....:
        // Reko: a decoder for V850 instruction E1FD at address 00101718 has not been implemented.
        [Test]
        public void V850Dis_E1FD()
        {
            AssertCode("@@@", "E1FD");
        }

        // ................:...:010011::.:.
        // Reko: a decoder for V850 instruction 7A8A at address 0010171A has not been implemented.
        [Test]
        public void V850Dis_7A8A()
        {
            AssertCode("@@@", "7A8A");
        }

        // .................::.:000110:::..
        // Reko: a decoder for V850 instruction DC68 at address 0010171C has not been implemented.
        [Test]
        public void V850Dis_DC68()
        {
            AssertCode("@@@", "DC68");
        }

        // ................:::::000100::...
        // Reko: a decoder for V850 instruction 98F8 at address 0010171E has not been implemented.
        [Test]
        public void V850Dis_98F8()
        {
            AssertCode("@@@", "98F8");
        }

        // ................:.::.011000....:
        // ................:.::..::.......: sld_b
        // .................:.::111011.:..:
        // Reko: a decoder for V850 instruction 695F at address 00101722 has not been implemented.
        [Test]
        public void V850Dis_695F()
        {
            AssertCode("@@@", "695F");
        }

        // ..................::.110000..:..
        // Reko: a decoder for V850 instruction 0436 at address 00101724 has not been implemented.
        [Test]
        public void V850Dis_0436()
        {
            AssertCode("@@@", "0436");
        }

        // .................:...001011:.:..
        // Reko: a decoder for V850 instruction 7441 at address 00101726 has not been implemented.
        [Test]
        public void V850Dis_7441()
        {
            AssertCode("@@@", "7441");
        }

        // ................::::.011011.:.:.
        // ................::::..::.::.:.:. sld_b
        // ...................::111100:.:.:
        // Reko: a decoder for V850 instruction 951F at address 0010172A has not been implemented.
        [Test]
        public void V850Dis_951F()
        {
            AssertCode("@@@", "951F");
        }

        // ....................:001101:..:.
        // Reko: a decoder for V850 instruction B209 at address 0010172C has not been implemented.
        [Test]
        public void V850Dis_B209()
        {
            AssertCode("@@@", "B209");
        }

        // ................:::..011010::...
        // ................:::...::.:.::... sld_b
        // .................:.:.001001:.::.
        // Reko: a decoder for V850 instruction 3651 at address 00101730 has not been implemented.
        [Test]
        public void V850Dis_3651()
        {
            AssertCode("@@@", "3651");
        }

        // .................:...111101.....
        // Reko: a decoder for V850 instruction A047 at address 00101732 has not been implemented.
        [Test]
        public void V850Dis_A047()
        {
            AssertCode("@@@", "A047");
        }

        // .................::..010010.:.::
        // Reko: a decoder for V850 instruction 4B62 at address 00101734 has not been implemented.
        [Test]
        public void V850Dis_4B62()
        {
            AssertCode("@@@", "4B62");
        }

        // ................:.:::101110.::::
        // Reko: a decoder for V850 instruction CFBD at address 00101736 has not been implemented.
        [Test]
        public void V850Dis_CFBD()
        {
            AssertCode("@@@", "CFBD");
        }

        // .................:::.100110:....
        // .................:::.:..::.:.... sst_h
        // .................::::100000:::..
        // .................:::::.....:::.. sld_h
        // .................::.:100010...:.
        // .................::.::...:....:. sld_h
        // ................:.:::101101::::.
        // Reko: a decoder for V850 instruction BEBD at address 0010173E has not been implemented.
        [Test]
        public void V850Dis_BEBD()
        {
            AssertCode("@@@", "BEBD");
        }

        // ................:::::100101:..:.
        // ................::::::..:.::..:. sst_h
        // ................:..:.001000:.:..
        // ................:..:...:...:.:.. or
        // .....................111100:...:
        // Reko: a decoder for V850 instruction 9107 at address 00101744 has not been implemented.
        [Test]
        public void V850Dis_9107()
        {
            AssertCode("@@@", "9107");
        }

        // ................:...:001100::...
        // Reko: a decoder for V850 instruction 9889 at address 00101746 has not been implemented.
        [Test]
        public void V850Dis_9889()
        {
            AssertCode("@@@", "9889");
        }

        // ................:....010101.::.:
        // Reko: a decoder for V850 instruction AD82 at address 00101748 has not been implemented.
        [Test]
        public void V850Dis_AD82()
        {
            AssertCode("@@@", "AD82");
        }

        // ................:....100001:::.:
        // ................:....:....::::.: sld_h
        // ....................:011001.::..
        // ....................:.::..:.::.. sld_b
        // ................::.:.111110..:..
        // Reko: a decoder for V850 instruction C4D7 at address 0010174E has not been implemented.
        [Test]
        public void V850Dis_C4D7()
        {
            AssertCode("@@@", "C4D7");
        }

        // ................:..:.110101..:..
        // Reko: a decoder for V850 instruction A496 at address 00101750 has not been implemented.
        [Test]
        public void V850Dis_A496()
        {
            AssertCode("@@@", "A496");
        }

        // ................:...:011010:..::
        // ................:...:.::.:.:..:: sld_b
        // .................:.:.000110.:...
        // ................:..::111101.::.:
        // Reko: a decoder for V850 instruction AD9F at address 00101756 has not been implemented.
        [Test]
        public void V850Dis_AD9F()
        {
            AssertCode("@@@", "AD9F");
        }

        // ................:.:.:011111..:..
        // ................:.:.:.:::::..:.. sst_b
        // ................:..:.000100.::::
        // Reko: a decoder for V850 instruction 8F90 at address 0010175A has not been implemented.
        [Test]
        public void V850Dis_8F90()
        {
            AssertCode("@@@", "8F90");
        }

        // ................:::::000000::..:
        // ................11111......::..:
        // ................:::::......::..: mov
        // ................:::::110111::::.
        // Reko: a decoder for V850 instruction FEFE at address 0010175E has not been implemented.
        [Test]
        public void V850Dis_FEFE()
        {
            AssertCode("@@@", "FEFE");
        }

        // ................::.::000001:.:..
        // ................::.::.....::.:.. not
        // ................:.::.101011.::.:
        // Reko: a decoder for V850 instruction 6DB5 at address 00101762 has not been implemented.
        [Test]
        public void V850Dis_6DB5()
        {
            AssertCode("@@@", "6DB5");
        }

        // ................::...111111:.:.:
        // Reko: a decoder for V850 instruction F5C7 at address 00101764 has not been implemented.
        [Test]
        public void V850Dis_F5C7()
        {
            AssertCode("@@@", "F5C7");
        }

        // ..................:.:111010:...:
        // Reko: a decoder for V850 instruction 512F at address 00101766 has not been implemented.
        [Test]
        public void V850Dis_512F()
        {
            AssertCode("@@@", "512F");
        }

        // ...................:.111101::...
        // Reko: a decoder for V850 instruction B817 at address 00101768 has not been implemented.
        [Test]
        public void V850Dis_B817()
        {
            AssertCode("@@@", "B817");
        }

        // ................::..:000100::.:.
        // Reko: a decoder for V850 instruction 9AC8 at address 0010176A has not been implemented.
        [Test]
        public void V850Dis_9AC8()
        {
            AssertCode("@@@", "9AC8");
        }

        // .................:..:011001..:.:
        // .................:..:.::..:..:.: sld_b
        // ................::...101100:....
        // Reko: a decoder for V850 instruction 90C5 at address 0010176E has not been implemented.
        [Test]
        public void V850Dis_90C5()
        {
            AssertCode("@@@", "90C5");
        }

        // .................:...100101.::::
        // .................:...:..:.:.:::: sst_h
        // ................:.:.:100110::...
        // ................:.:.::..::.::... sst_h
        // ..................::.011000::...
        // ..................::..::...::... sld_b
        // .................::..001111:.:..
        // Reko: a decoder for V850 instruction F461 at address 00101776 has not been implemented.
        [Test]
        public void V850Dis_F461()
        {
            AssertCode("@@@", "F461");
        }

        // .................:::.101010.:::.
        // ................:::..110101.::.:
        // Reko: a decoder for V850 instruction ADE6 at address 0010177A has not been implemented.
        [Test]
        public void V850Dis_ADE6()
        {
            AssertCode("@@@", "ADE6");
        }

        // ....................:000011:.:::
        // Reko: a decoder for V850 instruction 7708 at address 0010177C has not been implemented.
        [Test]
        public void V850Dis_7708()
        {
            AssertCode("@@@", "7708");
        }

        // ................:.::.101011.::::
        // Reko: a decoder for V850 instruction 6FB5 at address 0010177E has not been implemented.
        [Test]
        public void V850Dis_6FB5()
        {
            AssertCode("@@@", "6FB5");
        }

        // ..................:::110100...::
        // Reko: a decoder for V850 instruction 833E at address 00101780 has not been implemented.
        [Test]
        public void V850Dis_833E()
        {
            AssertCode("@@@", "833E");
        }

        // .................:.:.011010.::::
        // .................:.:..::.:..:::: sld_b
        // ....................:110100:::.:
        // Reko: a decoder for V850 instruction 9D0E at address 00101784 has not been implemented.
        [Test]
        public void V850Dis_9D0E()
        {
            AssertCode("@@@", "9D0E");
        }

        // ................::.::101111...:.
        // Reko: a decoder for V850 instruction E2DD at address 00101786 has not been implemented.
        [Test]
        public void V850Dis_E2DD()
        {
            AssertCode("@@@", "E2DD");
        }

        // ..................:..011010..:..
        // ..................:...::.:...:.. sld_b
        // ................:.::.000110:..:.
        // Reko: a decoder for V850 instruction D2B0 at address 0010178A has not been implemented.
        [Test]
        public void V850Dis_D2B0()
        {
            AssertCode("@@@", "D2B0");
        }

        // ................:::..101101:.::.
        // Reko: a decoder for V850 instruction B6E5 at address 0010178C has not been implemented.
        [Test]
        public void V850Dis_B6E5()
        {
            AssertCode("@@@", "B6E5");
        }

        // ................:..::001011::..:
        // Reko: a decoder for V850 instruction 7999 at address 0010178E has not been implemented.
        [Test]
        public void V850Dis_7999()
        {
            AssertCode("@@@", "7999");
        }

        // ................::..:010000::.::
        // ................::..:.:....11011
        // ................::..:.:....::.:: mov
        // ................::.:.100111.....
        // ................::.:.:..:::..... sst_h
        // .................:.:.010000:::::
        // .................:.:..:....11111
        // .................:.:..:....::::: mov
        // .....................001111..:.:
        // Reko: a decoder for V850 instruction E501 at address 00101796 has not been implemented.
        [Test]
        public void V850Dis_E501()
        {
            AssertCode("@@@", "E501");
        }

        // ....................:011111...::
        // ....................:.:::::...:: sst_b
        // ................:....000011..::.
        // Reko: a decoder for V850 instruction 6680 at address 0010179A has not been implemented.
        [Test]
        public void V850Dis_6680()
        {
            AssertCode("@@@", "6680");
        }

        // .................::.:011000::..:
        // .................::.:.::...::..: sld_b
        // ................:.::.000101:.:::
        // Reko: a decoder for V850 instruction B7B0 at address 0010179E has not been implemented.
        [Test]
        public void V850Dis_B7B0()
        {
            AssertCode("@@@", "B7B0");
        }

        // ................:::..100101.:...
        // ................:::..:..:.:.:... sst_h
        // ................:::..110010..:..
        // Reko: a decoder for V850 instruction 44E6 at address 001017A2 has not been implemented.
        [Test]
        public void V850Dis_44E6()
        {
            AssertCode("@@@", "44E6");
        }

        // ..................:::100111::.::
        // ..................::::..:::::.:: sst_h
        // ................:.:..001001..:::
        // Reko: a decoder for V850 instruction 27A1 at address 001017A6 has not been implemented.
        [Test]
        public void V850Dis_27A1()
        {
            AssertCode("@@@", "27A1");
        }

        // ................:::::010011...:.
        // Reko: a decoder for V850 instruction 62FA at address 001017A8 has not been implemented.
        [Test]
        public void V850Dis_62FA()
        {
            AssertCode("@@@", "62FA");
        }

        // ................::::.100011..:::
        // ................::::.:...::..::: sld_h
        // .................:...011011.....
        // .................:....::.::..... sld_b
        // .................::.:001101.:.:.
        // Reko: a decoder for V850 instruction AA69 at address 001017AE has not been implemented.
        [Test]
        public void V850Dis_AA69()
        {
            AssertCode("@@@", "AA69");
        }

        // ...................:.100111::.::
        // ...................:.:..:::::.:: sst_h
        // ................:.:.:101110.:.:.
        // Reko: a decoder for V850 instruction CAAD at address 001017B2 has not been implemented.
        [Test]
        public void V850Dis_CAAD()
        {
            AssertCode("@@@", "CAAD");
        }

        // ................:..::010001..:::
        // Reko: a decoder for V850 instruction 279A at address 001017B4 has not been implemented.
        [Test]
        public void V850Dis_279A()
        {
            AssertCode("@@@", "279A");
        }

        // ..................::.001100:.:.:
        // Reko: a decoder for V850 instruction 9531 at address 001017B6 has not been implemented.
        [Test]
        public void V850Dis_9531()
        {
            AssertCode("@@@", "9531");
        }

        // .................:..:110010::...
        // Reko: a decoder for V850 instruction 584E at address 001017B8 has not been implemented.
        [Test]
        public void V850Dis_584E()
        {
            AssertCode("@@@", "584E");
        }

        // ................:..:.001011:....
        // Reko: a decoder for V850 instruction 7091 at address 001017BA has not been implemented.
        [Test]
        public void V850Dis_7091()
        {
            AssertCode("@@@", "7091");
        }

        // .................::..001101..:::
        // Reko: a decoder for V850 instruction A761 at address 001017BC has not been implemented.
        [Test]
        public void V850Dis_A761()
        {
            AssertCode("@@@", "A761");
        }

        // ................::.::001110...::
        // Reko: a decoder for V850 instruction C3D9 at address 001017BE has not been implemented.
        [Test]
        public void V850Dis_C3D9()
        {
            AssertCode("@@@", "C3D9");
        }

        // ..................:..010011.::..
        // Reko: a decoder for V850 instruction 6C22 at address 001017C0 has not been implemented.
        [Test]
        public void V850Dis_6C22()
        {
            AssertCode("@@@", "6C22");
        }

        // ................::.:.010000.:...
        // ................::.:..:....01000
        // ................::.:..:.....:... mov
        // ................:..:.011101.....
        // ................:..:..:::.:..... sst_b
        // ................::.::110100:::.:
        // Reko: a decoder for V850 instruction 9DDE at address 001017C6 has not been implemented.
        [Test]
        public void V850Dis_9DDE()
        {
            AssertCode("@@@", "9DDE");
        }

        // ..................:::111101...:.
        // Reko: a decoder for V850 instruction A23F at address 001017C8 has not been implemented.
        [Test]
        public void V850Dis_A23F()
        {
            AssertCode("@@@", "A23F");
        }

        // ..................:..101011:..:.
        // Reko: a decoder for V850 instruction 7225 at address 001017CA has not been implemented.
        [Test]
        public void V850Dis_7225()
        {
            AssertCode("@@@", "7225");
        }

        // ................::::.000111.:.::
        // Reko: a decoder for V850 instruction EBF0 at address 001017CC has not been implemented.
        [Test]
        public void V850Dis_EBF0()
        {
            AssertCode("@@@", "EBF0");
        }

        // ................:..::111101.....
        // Reko: a decoder for V850 instruction A09F at address 001017CE has not been implemented.
        [Test]
        public void V850Dis_A09F()
        {
            AssertCode("@@@", "A09F");
        }

        // ................:::::101011....:
        // .....................101100..:.:
        // Reko: a decoder for V850 instruction 8505 at address 001017D2 has not been implemented.
        [Test]
        public void V850Dis_8505()
        {
            AssertCode("@@@", "8505");
        }

        // ....................:001101.....
        // Reko: a decoder for V850 instruction A009 at address 001017D4 has not been implemented.
        [Test]
        public void V850Dis_A009()
        {
            AssertCode("@@@", "A009");
        }

        // ....................:101100.:...
        // Reko: a decoder for V850 instruction 880D at address 001017D6 has not been implemented.
        [Test]
        public void V850Dis_880D()
        {
            AssertCode("@@@", "880D");
        }

        // ...................:.011010.::::
        // ...................:..::.:..:::: sld_b
        // ................:.:.:100100.::.:
        // ................:.:.::..:...::.: sst_h
        // ................:..:.101110.::..
        // Reko: a decoder for V850 instruction CC95 at address 001017DC has not been implemented.
        [Test]
        public void V850Dis_CC95()
        {
            AssertCode("@@@", "CC95");
        }

        // .....................000111::..:
        // Reko: a decoder for V850 instruction F900 at address 001017DE has not been implemented.
        [Test]
        public void V850Dis_F900()
        {
            AssertCode("@@@", "F900");
        }

        // ................:...:101001:.:::
        // ..................:::110100.:...
        // Reko: a decoder for V850 instruction 883E at address 001017E2 has not been implemented.
        [Test]
        public void V850Dis_883E()
        {
            AssertCode("@@@", "883E");
        }

        // ................::.::000001::.::
        // ................::.::.....:::.:: not
        // .................::.:000110.:...
        // Reko: a decoder for V850 instruction C868 at address 001017E6 has not been implemented.
        [Test]
        public void V850Dis_C868()
        {
            AssertCode("@@@", "C868");
        }

        // ................:::.:011011:.::.
        // ................:::.:.::.:::.::. sld_b
        // ................:::.:000000.:.::
        // ................11101.......:.::
        // ................:::.:.......:.:: mov
        // .................:::.000001..:::
        // .................:::......:..::: not
        // .................::..111010..:.:
        // Reko: a decoder for V850 instruction 4567 at address 001017EE has not been implemented.
        [Test]
        public void V850Dis_4567()
        {
            AssertCode("@@@", "4567");
        }

        // ................::.:.000101.:::.
        // Reko: a decoder for V850 instruction AED0 at address 001017F0 has not been implemented.
        [Test]
        public void V850Dis_AED0()
        {
            AssertCode("@@@", "AED0");
        }

        // ................:.:.:010001:....
        // Reko: a decoder for V850 instruction 30AA at address 001017F2 has not been implemented.
        [Test]
        public void V850Dis_30AA()
        {
            AssertCode("@@@", "30AA");
        }

        // ................:.:::110110:::..
        // Reko: a decoder for V850 instruction DCBE at address 001017F4 has not been implemented.
        [Test]
        public void V850Dis_DCBE()
        {
            AssertCode("@@@", "DCBE");
        }

        // .................::..010001::...
        // Reko: a decoder for V850 instruction 3862 at address 001017F6 has not been implemented.
        [Test]
        public void V850Dis_3862()
        {
            AssertCode("@@@", "3862");
        }

        // ................:::::011100::..:
        // ................:::::.:::..::..: sst_b
        // ................:...:101100...::
        // Reko: a decoder for V850 instruction 838D at address 001017FA has not been implemented.
        [Test]
        public void V850Dis_838D()
        {
            AssertCode("@@@", "838D");
        }

        // ................::::.001000..:.:
        // ................::::...:.....:.: or
        // ....................:100000:..:.
        // ....................::.....:..:. sld_h
        // .................:::.001010..:.:
        // Reko: a decoder for V850 instruction 4571 at address 00101800 has not been implemented.
        [Test]
        public void V850Dis_4571()
        {
            AssertCode("@@@", "4571");
        }

        // .................:.::101111.::.:
        // Reko: a decoder for V850 instruction ED5D at address 00101802 has not been implemented.
        [Test]
        public void V850Dis_ED5D()
        {
            AssertCode("@@@", "ED5D");
        }

        // .................::..001011.:...
        // Reko: a decoder for V850 instruction 6861 at address 00101804 has not been implemented.
        [Test]
        public void V850Dis_6861()
        {
            AssertCode("@@@", "6861");
        }

        // ................:...:000011..:..
        // Reko: a decoder for V850 instruction 6488 at address 00101806 has not been implemented.
        [Test]
        public void V850Dis_6488()
        {
            AssertCode("@@@", "6488");
        }

        // ...................::101001:.:..
        // Reko: a decoder for V850 instruction 341D at address 00101808 has not been implemented.
        [Test]
        public void V850Dis_341D()
        {
            AssertCode("@@@", "341D");
        }

        // ................:.::.100100.:::.
        // ................:.::.:..:...:::. sst_h
        // ................:..::111111::::.
        // Reko: a decoder for V850 instruction FE9F at address 0010180C has not been implemented.
        [Test]
        public void V850Dis_FE9F()
        {
            AssertCode("@@@", "FE9F");
        }

        // ................::.::110111..:.:
        // Reko: a decoder for V850 instruction E5DE at address 0010180E has not been implemented.
        [Test]
        public void V850Dis_E5DE()
        {
            AssertCode("@@@", "E5DE");
        }

        // ..................:..101110.::.:
        // Reko: a decoder for V850 instruction CD25 at address 00101810 has not been implemented.
        [Test]
        public void V850Dis_CD25()
        {
            AssertCode("@@@", "CD25");
        }

        // ................:::::100111:::.:
        // ................::::::..::::::.: sst_h
        // .................:..:111011..:.:
        // Reko: a decoder for V850 instruction 654F at address 00101814 has not been implemented.
        [Test]
        public void V850Dis_654F()
        {
            AssertCode("@@@", "654F");
        }

        // .................::::001001...::
        // Reko: a decoder for V850 instruction 2379 at address 00101816 has not been implemented.
        [Test]
        public void V850Dis_2379()
        {
            AssertCode("@@@", "2379");
        }

        // ..................:..110010:.:.:
        // Reko: a decoder for V850 instruction 5526 at address 00101818 has not been implemented.
        [Test]
        public void V850Dis_5526()
        {
            AssertCode("@@@", "5526");
        }

        // ..................:::111101..:..
        // Reko: a decoder for V850 instruction A43F at address 0010181A has not been implemented.
        [Test]
        public void V850Dis_A43F()
        {
            AssertCode("@@@", "A43F");
        }

        // ..................:::011010:.:.:
        // ..................:::.::.:.:.:.: sld_b
        // ................:....101011...:.
        // Reko: a decoder for V850 instruction 6285 at address 0010181E has not been implemented.
        [Test]
        public void V850Dis_6285()
        {
            AssertCode("@@@", "6285");
        }

        // ................:::::110110::..:
        // Reko: a decoder for V850 instruction D9FE at address 00101820 has not been implemented.
        [Test]
        public void V850Dis_D9FE()
        {
            AssertCode("@@@", "D9FE");
        }

        // ................:.::.110101...::
        // Reko: a decoder for V850 instruction A3B6 at address 00101822 has not been implemented.
        [Test]
        public void V850Dis_A3B6()
        {
            AssertCode("@@@", "A3B6");
        }

        // ................:::.:000100...:.
        // Reko: a decoder for V850 instruction 82E8 at address 00101824 has not been implemented.
        [Test]
        public void V850Dis_82E8()
        {
            AssertCode("@@@", "82E8");
        }

        // .................:..:100111:::::
        // .................:..::..:::::::: sst_h
        // ................:.:..111011.:.::
        // Reko: a decoder for V850 instruction 6BA7 at address 00101828 has not been implemented.
        [Test]
        public void V850Dis_6BA7()
        {
            AssertCode("@@@", "6BA7");
        }

        // ................:.:::111011:.:.:
        // Reko: a decoder for V850 instruction 75BF at address 0010182A has not been implemented.
        [Test]
        public void V850Dis_75BF()
        {
            AssertCode("@@@", "75BF");
        }

        // ................:....011001.::::
        // ................:.....::..:.:::: sld_b
        // ................:::::010011...::
        // Reko: a decoder for V850 instruction 63FA at address 0010182E has not been implemented.
        [Test]
        public void V850Dis_63FA()
        {
            AssertCode("@@@", "63FA");
        }

        // ................:.:..100000:.:.:
        // ................:.:..:.....:.:.: sld_h
        // ................::.::111101:::::
        // Reko: a decoder for V850 instruction BFDF at address 00101832 has not been implemented.
        [Test]
        public void V850Dis_BFDF()
        {
            AssertCode("@@@", "BFDF");
        }

        // ...................:.110111:..::
        // Reko: a decoder for V850 instruction F316 at address 00101834 has not been implemented.
        [Test]
        public void V850Dis_F316()
        {
            AssertCode("@@@", "F316");
        }

        // ................::::.101101..:::
        // Reko: a decoder for V850 instruction A7F5 at address 00101836 has not been implemented.
        [Test]
        public void V850Dis_A7F5()
        {
            AssertCode("@@@", "A7F5");
        }

        // .................:::.100010.:::.
        // .................:::.:...:..:::. sld_h
        // .................:...001000.:...
        // .................:.....:....:... or
        // ..................:..100101:::..
        // ..................:..:..:.::::.. sst_h
        // ................:::::000001:..::
        // ................:::::.....::..:: not
        // ....................:001101::.:.
        // Reko: a decoder for V850 instruction BA09 at address 00101840 has not been implemented.
        [Test]
        public void V850Dis_BA09()
        {
            AssertCode("@@@", "BA09");
        }

        // ................:::::111110::.::
        // Reko: a decoder for V850 instruction DBFF at address 00101842 has not been implemented.
        [Test]
        public void V850Dis_DBFF()
        {
            AssertCode("@@@", "DBFF");
        }

        // ..................:.:111111:::..
        // Reko: a decoder for V850 instruction FC2F at address 00101844 has not been implemented.
        [Test]
        public void V850Dis_FC2F()
        {
            AssertCode("@@@", "FC2F");
        }

        // ................:::..101010:::..
        // Reko: a decoder for V850 instruction 5CE5 at address 00101846 has not been implemented.
        [Test]
        public void V850Dis_5CE5()
        {
            AssertCode("@@@", "5CE5");
        }

        // ................:.::.000001..:.:
        // ................:.::......:..:.: not
        // ................:.:::110100:....
        // Reko: a decoder for V850 instruction 90BE at address 0010184A has not been implemented.
        [Test]
        public void V850Dis_90BE()
        {
            AssertCode("@@@", "90BE");
        }

        // .................::..100110::.:.
        // .................::..:..::.::.:. sst_h
        // ................::.:.001010.:.:.
        // Reko: a decoder for V850 instruction 4AD1 at address 0010184E has not been implemented.
        [Test]
        public void V850Dis_4AD1()
        {
            AssertCode("@@@", "4AD1");
        }

        // ................:.:.:010110...:.
        // Reko: a decoder for V850 instruction C2AA at address 00101850 has not been implemented.
        [Test]
        public void V850Dis_C2AA()
        {
            AssertCode("@@@", "C2AA");
        }

        // ................:.::.001001.:..:
        // Reko: a decoder for V850 instruction 29B1 at address 00101852 has not been implemented.
        [Test]
        public void V850Dis_29B1()
        {
            AssertCode("@@@", "29B1");
        }

        // ................:....010011:::..
        // Reko: a decoder for V850 instruction 7C82 at address 00101854 has not been implemented.
        [Test]
        public void V850Dis_7C82()
        {
            AssertCode("@@@", "7C82");
        }

        // ................::...011100:...:
        // ................::....:::..:...: sst_b
        // ................:.:.:110010:.:::
        // Reko: a decoder for V850 instruction 57AE at address 00101858 has not been implemented.
        [Test]
        public void V850Dis_57AE()
        {
            AssertCode("@@@", "57AE");
        }

        // ....................:110011.:...
        // Reko: a decoder for V850 instruction 680E at address 0010185A has not been implemented.
        [Test]
        public void V850Dis_680E()
        {
            AssertCode("@@@", "680E");
        }

        // ................:.:::110111.....
        // Reko: a decoder for V850 instruction E0BE at address 0010185C has not been implemented.
        [Test]
        public void V850Dis_E0BE()
        {
            AssertCode("@@@", "E0BE");
        }

        // .................:...110100:....
        // Reko: a decoder for V850 instruction 9046 at address 0010185E has not been implemented.
        [Test]
        public void V850Dis_9046()
        {
            AssertCode("@@@", "9046");
        }

        // .................::::100011:..::
        // .................:::::...:::..:: sld_h
        // ..................:..101000..:.:
        // ..................:..:.:.....:.1 Sld.w/Sst.w
        // ..................:..:.:.....:.: sst_w
        // .................::..011101.:...
        // .................::...:::.:.:... sst_b
        // ................:::..110111::::.
        // Reko: a decoder for V850 instruction FEE6 at address 00101866 has not been implemented.
        [Test]
        public void V850Dis_FEE6()
        {
            AssertCode("@@@", "FEE6");
        }

        // ................::.::001010.:...
        // Reko: a decoder for V850 instruction 48D9 at address 00101868 has not been implemented.
        [Test]
        public void V850Dis_48D9()
        {
            AssertCode("@@@", "48D9");
        }

        // .................:...010101::.:.
        // Reko: a decoder for V850 instruction BA42 at address 0010186A has not been implemented.
        [Test]
        public void V850Dis_BA42()
        {
            AssertCode("@@@", "BA42");
        }

        // ................:::.:001100..:.:
        // Reko: a decoder for V850 instruction 85E9 at address 0010186C has not been implemented.
        [Test]
        public void V850Dis_85E9()
        {
            AssertCode("@@@", "85E9");
        }

        // ................:::.:001000::.::
        // ................:::.:..:...::.:: or
        // ..................::.110001.:.:.
        // Reko: a decoder for V850 instruction 2A36 at address 00101870 has not been implemented.
        [Test]
        public void V850Dis_2A36()
        {
            AssertCode("@@@", "2A36");
        }

        // ...................:.010011:::::
        // Reko: a decoder for V850 instruction 7F12 at address 00101872 has not been implemented.
        [Test]
        public void V850Dis_7F12()
        {
            AssertCode("@@@", "7F12");
        }

        // ................:....010111::.:.
        // Reko: a decoder for V850 instruction FA82 at address 00101874 has not been implemented.
        [Test]
        public void V850Dis_FA82()
        {
            AssertCode("@@@", "FA82");
        }

        // ..................::.111011::.::
        // Reko: a decoder for V850 instruction 7B37 at address 00101876 has not been implemented.
        [Test]
        public void V850Dis_7B37()
        {
            AssertCode("@@@", "7B37");
        }

        // ...................:.111011::...
        // Reko: a decoder for V850 instruction 7817 at address 00101878 has not been implemented.
        [Test]
        public void V850Dis_7817()
        {
            AssertCode("@@@", "7817");
        }

        // .................:..:110101.:...
        // Reko: a decoder for V850 instruction A84E at address 0010187A has not been implemented.
        [Test]
        public void V850Dis_A84E()
        {
            AssertCode("@@@", "A84E");
        }

        // .................:..:101110..:::
        // Reko: a decoder for V850 instruction C74D at address 0010187C has not been implemented.
        [Test]
        public void V850Dis_C74D()
        {
            AssertCode("@@@", "C74D");
        }

        // .................::..111000..:.:
        // .................:..:001001.....
        // Reko: a decoder for V850 instruction 2049 at address 00101880 has not been implemented.
        [Test]
        public void V850Dis_2049()
        {
            AssertCode("@@@", "2049");
        }

        // .................:.::101010.:.:.
        // Reko: a decoder for V850 instruction 4A5D at address 00101882 has not been implemented.
        [Test]
        public void V850Dis_4A5D()
        {
            AssertCode("@@@", "4A5D");
        }

        // ..................:..100010:::::
        // ..................:..:...:.::::: sld_h
        // .................:.::110001.:..:
        // Reko: a decoder for V850 instruction 295E at address 00101886 has not been implemented.
        [Test]
        public void V850Dis_295E()
        {
            AssertCode("@@@", "295E");
        }

        // .................:...011101.::..
        // .................:....:::.:.::.. sst_b
        // ................:.:..011011.:...
        // ................:.:...::.::.:... sld_b
        // .................:..:110110.:...
        // Reko: a decoder for V850 instruction C84E at address 0010188C has not been implemented.
        [Test]
        public void V850Dis_C84E()
        {
            AssertCode("@@@", "C84E");
        }

        // .................::.:110001:::.:
        // Reko: a decoder for V850 instruction 3D6E at address 0010188E has not been implemented.
        [Test]
        public void V850Dis_3D6E()
        {
            AssertCode("@@@", "3D6E");
        }

        // .................::::110100:..:.
        // Reko: a decoder for V850 instruction 927E at address 00101890 has not been implemented.
        [Test]
        public void V850Dis_927E()
        {
            AssertCode("@@@", "927E");
        }

        // ................:.:.:001111...::
        // Reko: a decoder for V850 instruction E3A9 at address 00101892 has not been implemented.
        [Test]
        public void V850Dis_E3A9()
        {
            AssertCode("@@@", "E3A9");
        }

        // .................::::101001:::::
        // Reko: a decoder for V850 instruction 3F7D at address 00101894 has not been implemented.
        [Test]
        public void V850Dis_3F7D()
        {
            AssertCode("@@@", "3F7D");
        }

        // ................::::.001010.:.::
        // Reko: a decoder for V850 instruction 4BF1 at address 00101896 has not been implemented.
        [Test]
        public void V850Dis_4BF1()
        {
            AssertCode("@@@", "4BF1");
        }

        // ................::..:101000....:
        // ................::..::.:.......1 Sld.w/Sst.w
        // ................::..::.:.......: sst_w
        // ................:..:.000101.::.:
        // Reko: a decoder for V850 instruction AD90 at address 0010189A has not been implemented.
        [Test]
        public void V850Dis_AD90()
        {
            AssertCode("@@@", "AD90");
        }

        // ................:.::.000101::.::
        // Reko: a decoder for V850 instruction BBB0 at address 0010189C has not been implemented.
        [Test]
        public void V850Dis_BBB0()
        {
            AssertCode("@@@", "BBB0");
        }

        // ................:::::010000::::.
        // ................:::::.:....11110
        // ................:::::.:....::::. mov
        // .................:.:.010110:.:..
        // Reko: a decoder for V850 instruction D452 at address 001018A0 has not been implemented.
        [Test]
        public void V850Dis_D452()
        {
            AssertCode("@@@", "D452");
        }

        // ..................:::100001....:
        // ..................::::....:....: sld_h
        // ................:.:::101100::::.
        // Reko: a decoder for V850 instruction 9EBD at address 001018A4 has not been implemented.
        [Test]
        public void V850Dis_9EBD()
        {
            AssertCode("@@@", "9EBD");
        }

        // .....................101100.:.:.
        // Reko: a decoder for V850 instruction 8A05 at address 001018A6 has not been implemented.
        [Test]
        public void V850Dis_8A05()
        {
            AssertCode("@@@", "8A05");
        }

        // ..................:..000000.::..
        // ................00100.......::..
        // ..................:.........::.. mov
        // ................:.::.111011..::.
        // ................::.::000000::..:
        // ................11011......::..:
        // ................::.::......::..: mov
        // ..................:.:001011.::::
        // Reko: a decoder for V850 instruction 6F29 at address 001018AE has not been implemented.
        [Test]
        public void V850Dis_6F29()
        {
            AssertCode("@@@", "6F29");
        }

        // ..................:::010110::.:.
        // Reko: a decoder for V850 instruction DA3A at address 001018B0 has not been implemented.
        [Test]
        public void V850Dis_DA3A()
        {
            AssertCode("@@@", "DA3A");
        }

        // .................:..:001101::.::
        // Reko: a decoder for V850 instruction BB49 at address 001018B2 has not been implemented.
        [Test]
        public void V850Dis_BB49()
        {
            AssertCode("@@@", "BB49");
        }

        // .................:.::011011.:..:
        // .................:.::.::.::.:..: sld_b
        // ................:.:..011001.:...
        // ................:.:...::..:.:... sld_b
        // .................:.::001111:::.:
        // Reko: a decoder for V850 instruction FD59 at address 001018B8 has not been implemented.
        [Test]
        public void V850Dis_FD59()
        {
            AssertCode("@@@", "FD59");
        }

        // ................:..:.001010:::..
        // Reko: a decoder for V850 instruction 5C91 at address 001018BA has not been implemented.
        [Test]
        public void V850Dis_5C91()
        {
            AssertCode("@@@", "5C91");
        }

        // ................:..::001011:.:::
        // Reko: a decoder for V850 instruction 7799 at address 001018BC has not been implemented.
        [Test]
        public void V850Dis_7799()
        {
            AssertCode("@@@", "7799");
        }

        // ................:::::100101...::
        // ................::::::..:.:...:: sst_h
        // .................::.:110001..:..
        // Reko: a decoder for V850 instruction 246E at address 001018C0 has not been implemented.
        [Test]
        public void V850Dis_246E()
        {
            AssertCode("@@@", "246E");
        }

        // .................:::.110110.::::
        // Reko: a decoder for V850 instruction CF76 at address 001018C2 has not been implemented.
        [Test]
        public void V850Dis_CF76()
        {
            AssertCode("@@@", "CF76");
        }

        // .....................001001.::..
        // Reko: a decoder for V850 instruction 2C01 at address 001018C4 has not been implemented.
        [Test]
        public void V850Dis_2C01()
        {
            AssertCode("@@@", "2C01");
        }

        // ................::.:.100110.::::
        // ................::.:.:..::..:::: sst_h
        // ................::.::110111:..::
        // Reko: a decoder for V850 instruction F3DE at address 001018C8 has not been implemented.
        [Test]
        public void V850Dis_F3DE()
        {
            AssertCode("@@@", "F3DE");
        }

        // ...................::111100:::.:
        // Reko: a decoder for V850 instruction 9D1F at address 001018CA has not been implemented.
        [Test]
        public void V850Dis_9D1F()
        {
            AssertCode("@@@", "9D1F");
        }

        // ................:..:.100000:.:::
        // ................:..:.:.....:.::: sld_h
        // ..................:.:000110:.:::
        // Reko: a decoder for V850 instruction D728 at address 001018CE has not been implemented.
        [Test]
        public void V850Dis_D728()
        {
            AssertCode("@@@", "D728");
        }

        // ................:....011010::::.
        // ................:.....::.:.::::. sld_b
        // ................:....000101:.:.:
        // Reko: a decoder for V850 instruction B580 at address 001018D2 has not been implemented.
        [Test]
        public void V850Dis_B580()
        {
            AssertCode("@@@", "B580");
        }

        // ................::.:.101111:.:..
        // Reko: a decoder for V850 instruction F4D5 at address 001018D4 has not been implemented.
        [Test]
        public void V850Dis_F4D5()
        {
            AssertCode("@@@", "F4D5");
        }

        // .................::::000100:....
        // Reko: a decoder for V850 instruction 9078 at address 001018D6 has not been implemented.
        [Test]
        public void V850Dis_9078()
        {
            AssertCode("@@@", "9078");
        }

        // .................::::110001.:.:.
        // Reko: a decoder for V850 instruction 2A7E at address 001018D8 has not been implemented.
        [Test]
        public void V850Dis_2A7E()
        {
            AssertCode("@@@", "2A7E");
        }

        // .................:...101001::::.
        // Reko: a decoder for V850 instruction 3E45 at address 001018DA has not been implemented.
        [Test]
        public void V850Dis_3E45()
        {
            AssertCode("@@@", "3E45");
        }

        // ................:::::000011....:
        // Reko: a decoder for V850 instruction 61F8 at address 001018DC has not been implemented.
        [Test]
        public void V850Dis_61F8()
        {
            AssertCode("@@@", "61F8");
        }

        // .................:::.011100.:::.
        // .................:::..:::...:::. sst_b
        // .................::.:010011:::..
        // Reko: a decoder for V850 instruction 7C6A at address 001018E0 has not been implemented.
        [Test]
        public void V850Dis_7C6A()
        {
            AssertCode("@@@", "7C6A");
        }

        // ................:.:.:011100:..:.
        // ................:.:.:.:::..:..:. sst_b
        // ................:::::000000.:..:
        // ................11111.......:..:
        // ................:::::.......:..: mov
        // ................::.::001111:::..
        // Reko: a decoder for V850 instruction FCD9 at address 001018E6 has not been implemented.
        [Test]
        public void V850Dis_FCD9()
        {
            AssertCode("@@@", "FCD9");
        }

        // ................:::..110011.:.:.
        // Reko: a decoder for V850 instruction 6AE6 at address 001018E8 has not been implemented.
        [Test]
        public void V850Dis_6AE6()
        {
            AssertCode("@@@", "6AE6");
        }

        // ................:...:011010:.:.:
        // ................:...:.::.:.:.:.: sld_b
        // ....................:001101::::.
        // Reko: a decoder for V850 instruction BE09 at address 001018EC has not been implemented.
        [Test]
        public void V850Dis_BE09()
        {
            AssertCode("@@@", "BE09");
        }

        // .................::.:001100.::..
        // Reko: a decoder for V850 instruction 8C69 at address 001018EE has not been implemented.
        [Test]
        public void V850Dis_8C69()
        {
            AssertCode("@@@", "8C69");
        }

        // ..................::.011100....:
        // ..................::..:::......: sst_b
        // ................::::.011001:..::
        // ................::::..::..::..:: sld_b
        // ..................:..010111...::
        // Reko: a decoder for V850 instruction E322 at address 001018F4 has not been implemented.
        [Test]
        public void V850Dis_E322()
        {
            AssertCode("@@@", "E322");
        }

        // .................:..:111000..:::
        // Reko: a decoder for V850 instruction 074F at address 001018F6 has not been implemented.
        [Test]
        public void V850Dis_074F()
        {
            AssertCode("@@@", "074F");
        }

        // .................:.:.110110:::.:
        // Reko: a decoder for V850 instruction DD56 at address 001018F8 has not been implemented.
        [Test]
        public void V850Dis_DD56()
        {
            AssertCode("@@@", "DD56");
        }

        // ................:.:.:110010.::..
        // Reko: a decoder for V850 instruction 4CAE at address 001018FA has not been implemented.
        [Test]
        public void V850Dis_4CAE()
        {
            AssertCode("@@@", "4CAE");
        }

        // ................:...:010110...:.
        // Reko: a decoder for V850 instruction C28A at address 001018FC has not been implemented.
        [Test]
        public void V850Dis_C28A()
        {
            AssertCode("@@@", "C28A");
        }

        // ................:::::011011:..::
        // ................:::::.::.:::..:: sld_b
        // ..................:.:001010.::::
        // Reko: a decoder for V850 instruction 4F29 at address 00101900 has not been implemented.
        [Test]
        public void V850Dis_4F29()
        {
            AssertCode("@@@", "4F29");
        }

        // ................:.:.:101101...:.
        // Reko: a decoder for V850 instruction A2AD at address 00101902 has not been implemented.
        [Test]
        public void V850Dis_A2AD()
        {
            AssertCode("@@@", "A2AD");
        }

        // ..................:.:011000...:.
        // ..................:.:.::......:. sld_b
        // .................::..101000:::::
        // .................::..:.:...::::1 Sld.w/Sst.w
        // .................::..:.:...::::: sst_w
        // ................::.::100100..::.
        // ................::.:::..:....::. sst_h
        // ....................:110000:.::.
        // Reko: a decoder for V850 instruction 160E at address 0010190A has not been implemented.
        [Test]
        public void V850Dis_160E()
        {
            AssertCode("@@@", "160E");
        }

        // .....................100100.....
        // .....................:..:....... sst_h
        // ..................:..000101::.:.
        // ................::.:.100111:..:.
        // ................::.:.:..::::..:. sst_h
        // ..................:.:110110...::
        // Reko: a decoder for V850 instruction C32E at address 00101912 has not been implemented.
        [Test]
        public void V850Dis_C32E()
        {
            AssertCode("@@@", "C32E");
        }

        // ................:..::010110..:..
        // Reko: a decoder for V850 instruction C49A at address 00101914 has not been implemented.
        [Test]
        public void V850Dis_C49A()
        {
            AssertCode("@@@", "C49A");
        }

        // .................:.::001100:....
        // Reko: a decoder for V850 instruction 9059 at address 00101916 has not been implemented.
        [Test]
        public void V850Dis_9059()
        {
            AssertCode("@@@", "9059");
        }

        // .................:...011011...:.
        // .................:....::.::...:. sld_b
        // ................:.::.010110.::.:
        // Reko: a decoder for V850 instruction CDB2 at address 0010191A has not been implemented.
        [Test]
        public void V850Dis_CDB2()
        {
            AssertCode("@@@", "CDB2");
        }

        // .................:..:110101.::..
        // Reko: a decoder for V850 instruction AC4E at address 0010191C has not been implemented.
        [Test]
        public void V850Dis_AC4E()
        {
            AssertCode("@@@", "AC4E");
        }

        // ................:::..000000:.::.
        // ................11100......:.::.
        // ................:::........:.::. mov
        // .................:.::001011:..::
        // Reko: a decoder for V850 instruction 7359 at address 00101920 has not been implemented.
        [Test]
        public void V850Dis_7359()
        {
            AssertCode("@@@", "7359");
        }

        // ................:..:.101001:::..
        // Reko: a decoder for V850 instruction 3C95 at address 00101922 has not been implemented.
        [Test]
        public void V850Dis_3C95()
        {
            AssertCode("@@@", "3C95");
        }

        // ................::.::111011..::.
        // Reko: a decoder for V850 instruction 66DF at address 00101924 has not been implemented.
        [Test]
        public void V850Dis_66DF()
        {
            AssertCode("@@@", "66DF");
        }

        // .................:.:.101011..:::
        // Reko: a decoder for V850 instruction 6755 at address 00101926 has not been implemented.
        [Test]
        public void V850Dis_6755()
        {
            AssertCode("@@@", "6755");
        }

        // ................::..:110000:.:..
        // Reko: a decoder for V850 instruction 14CE at address 00101928 has not been implemented.
        [Test]
        public void V850Dis_14CE()
        {
            AssertCode("@@@", "14CE");
        }

        // .....................110011.::.:
        // Reko: a decoder for V850 instruction 6D06 at address 0010192A has not been implemented.
        [Test]
        public void V850Dis_6D06()
        {
            AssertCode("@@@", "6D06");
        }

        // ................:::::000000.::..
        // ................11111.......::..
        // ................:::::.......::.. mov
        // ................::.::001110.:::.
        // Reko: a decoder for V850 instruction CED9 at address 0010192E has not been implemented.
        [Test]
        public void V850Dis_CED9()
        {
            AssertCode("@@@", "CED9");
        }

        // ..................:.:011100:::..
        // ..................:.:.:::..:::.. sst_b
        // ................:::.:110101::.::
        // Reko: a decoder for V850 instruction BBEE at address 00101932 has not been implemented.
        [Test]
        public void V850Dis_BBEE()
        {
            AssertCode("@@@", "BBEE");
        }

        // .................:...100110..::.
        // .................:...:..::...::. sst_h
        // ................:.::.101001:.::.
        // Reko: a decoder for V850 instruction 36B5 at address 00101936 has not been implemented.
        [Test]
        public void V850Dis_36B5()
        {
            AssertCode("@@@", "36B5");
        }

        // .................:..:000100::..:
        // Reko: a decoder for V850 instruction 9948 at address 00101938 has not been implemented.
        [Test]
        public void V850Dis_9948()
        {
            AssertCode("@@@", "9948");
        }

        // ................:.:::111010.:.:.
        // Reko: a decoder for V850 instruction 4ABF at address 0010193A has not been implemented.
        [Test]
        public void V850Dis_4ABF()
        {
            AssertCode("@@@", "4ABF");
        }

        // .................::.:100010::::.
        // .................::.::...:.::::. sld_h
        // ..................:::000101::...
        // Reko: a decoder for V850 instruction B838 at address 0010193E has not been implemented.
        [Test]
        public void V850Dis_B838()
        {
            AssertCode("@@@", "B838");
        }

        // ..................::.110110..:.:
        // Reko: a decoder for V850 instruction C536 at address 00101940 has not been implemented.
        [Test]
        public void V850Dis_C536()
        {
            AssertCode("@@@", "C536");
        }

        // ..................:..110100::.::
        // ..................:..100110.:...
        // ..................:..:..::..:... sst_h
        // ...................:.011101::..:
        // ...................:..:::.:::..: sst_b
        // .................::.:101011.::.:
        // Reko: a decoder for V850 instruction 6D6D at address 00101948 has not been implemented.
        [Test]
        public void V850Dis_6D6D()
        {
            AssertCode("@@@", "6D6D");
        }

        // ................::::.110000::..:
        // Reko: a decoder for V850 instruction 19F6 at address 0010194A has not been implemented.
        [Test]
        public void V850Dis_19F6()
        {
            AssertCode("@@@", "19F6");
        }

        // ................:...:001001.::.:
        // .................::.:010010.::.:
        // Reko: a decoder for V850 instruction 4D6A at address 0010194E has not been implemented.
        [Test]
        public void V850Dis_4D6A()
        {
            AssertCode("@@@", "4D6A");
        }

        // ................::::.100011:.:..
        // ................::::.:...:::.:.. sld_h
        // ................:....001000:.::.
        // ................:......:...:.::. or
        // ..................:.:000100:..:.
        // Reko: a decoder for V850 instruction 9228 at address 00101954 has not been implemented.
        [Test]
        public void V850Dis_9228()
        {
            AssertCode("@@@", "9228");
        }

        // ..................:.:110000::..:
        // Reko: a decoder for V850 instruction 192E at address 00101956 has not been implemented.
        [Test]
        public void V850Dis_192E()
        {
            AssertCode("@@@", "192E");
        }

        // ................:....110001...::
        // Reko: a decoder for V850 instruction 2386 at address 00101958 has not been implemented.
        [Test]
        public void V850Dis_2386()
        {
            AssertCode("@@@", "2386");
        }

        // ...................::110111:::..
        // ................:.:.:000100:.:..
        // Reko: a decoder for V850 instruction 94A8 at address 0010195C has not been implemented.
        [Test]
        public void V850Dis_94A8()
        {
            AssertCode("@@@", "94A8");
        }

        // ................:.:.:000111:.:::
        // Reko: a decoder for V850 instruction F7A8 at address 0010195E has not been implemented.
        [Test]
        public void V850Dis_F7A8()
        {
            AssertCode("@@@", "F7A8");
        }

        // ..................::.101000:.:.:
        // ..................::.:.:...:.:.1 Sld.w/Sst.w
        // ..................::.:.:...:.:.: sst_w
        // ................::.:.101010.....
        // Reko: a decoder for V850 instruction 40D5 at address 00101962 has not been implemented.
        [Test]
        public void V850Dis_40D5()
        {
            AssertCode("@@@", "40D5");
        }

        // ................:.:..111010:::.:
        // Reko: a decoder for V850 instruction 5DA7 at address 00101964 has not been implemented.
        [Test]
        public void V850Dis_5DA7()
        {
            AssertCode("@@@", "5DA7");
        }

        // .................:::.111000:....
        // Reko: a decoder for V850 instruction 1077 at address 00101966 has not been implemented.
        [Test]
        public void V850Dis_1077()
        {
            AssertCode("@@@", "1077");
        }

        // ................:.:.:000011:...:
        // Reko: a decoder for V850 instruction 71A8 at address 00101968 has not been implemented.
        [Test]
        public void V850Dis_71A8()
        {
            AssertCode("@@@", "71A8");
        }

        // .................:.::000100.....
        // Reko: a decoder for V850 instruction 8058 at address 0010196A has not been implemented.
        [Test]
        public void V850Dis_8058()
        {
            AssertCode("@@@", "8058");
        }

        // ..................:::111001.:.:.
        // Reko: a decoder for V850 instruction 2A3F at address 0010196C has not been implemented.
        [Test]
        public void V850Dis_2A3F()
        {
            AssertCode("@@@", "2A3F");
        }

        // ....................:111100.:...
        // Reko: a decoder for V850 instruction 880F at address 0010196E has not been implemented.
        [Test]
        public void V850Dis_880F()
        {
            AssertCode("@@@", "880F");
        }

        // ................::.::111111::.::
        // .................::.:010010.:.:.
        // Reko: a decoder for V850 instruction 4A6A at address 00101972 has not been implemented.
        [Test]
        public void V850Dis_4A6A()
        {
            AssertCode("@@@", "4A6A");
        }

        // ..................::.111010:.:.:
        // Reko: a decoder for V850 instruction 5537 at address 00101974 has not been implemented.
        [Test]
        public void V850Dis_5537()
        {
            AssertCode("@@@", "5537");
        }

        // ................:..::101101..:.:
        // Reko: a decoder for V850 instruction A59D at address 00101976 has not been implemented.
        [Test]
        public void V850Dis_A59D()
        {
            AssertCode("@@@", "A59D");
        }

        // .................::.:100000:::..
        // .................::.::.....:::.. sld_h
        // .................:...001000...::
        // .................:.....:......:: or
        // ................:..::011001.:...
        // ................:..::.::..:.:... sld_b
        // ................::...101011::::.
        // Reko: a decoder for V850 instruction 7EC5 at address 0010197E has not been implemented.
        [Test]
        public void V850Dis_7EC5()
        {
            AssertCode("@@@", "7EC5");
        }

        // .................:::.001011:.::.
        // Reko: a decoder for V850 instruction 7671 at address 00101980 has not been implemented.
        [Test]
        public void V850Dis_7671()
        {
            AssertCode("@@@", "7671");
        }

        // ................:::::000111....:
        // Reko: a decoder for V850 instruction E1F8 at address 00101982 has not been implemented.
        [Test]
        public void V850Dis_E1F8()
        {
            AssertCode("@@@", "E1F8");
        }

        // .................:::.111010.:...
        // Reko: a decoder for V850 instruction 4877 at address 00101984 has not been implemented.
        [Test]
        public void V850Dis_4877()
        {
            AssertCode("@@@", "4877");
        }

        // ................::..:100000.::..
        // ................::..::......::.. sld_h
        // ................:..::110111...::
        // Reko: a decoder for V850 instruction E39E at address 00101988 has not been implemented.
        [Test]
        public void V850Dis_E39E()
        {
            AssertCode("@@@", "E39E");
        }

        // ................:::.:010000.::::
        // ................:::.:.:....01111
        // ................:::.:.:.....:::: mov
        // ................::...000101..:::
        // Reko: a decoder for V850 instruction A7C0 at address 0010198C has not been implemented.
        [Test]
        public void V850Dis_A7C0()
        {
            AssertCode("@@@", "A7C0");
        }

        // ................:::..011000...::
        // ................:::...::......:: sld_b
        // ................:::.:100111::::.
        // ................:::.::..:::::::. sst_h
        // ................:.::.101001...::
        // Reko: a decoder for V850 instruction 23B5 at address 00101992 has not been implemented.
        [Test]
        public void V850Dis_23B5()
        {
            AssertCode("@@@", "23B5");
        }

        // ................:...:100010:::.:
        // ................:...::...:.:::.: sld_h
        // ................::.::101010:..:.
        // Reko: a decoder for V850 instruction 52DD at address 00101996 has not been implemented.
        [Test]
        public void V850Dis_52DD()
        {
            AssertCode("@@@", "52DD");
        }

        // ................:.:..011100.::::
        // ................:.:...:::...:::: sst_b
        // ................::::.000101::..:
        // Reko: a decoder for V850 instruction B9F0 at address 0010199A has not been implemented.
        [Test]
        public void V850Dis_B9F0()
        {
            AssertCode("@@@", "B9F0");
        }

        // ................::..:111101..:..
        // Reko: a decoder for V850 instruction A4CF at address 0010199C has not been implemented.
        [Test]
        public void V850Dis_A4CF()
        {
            AssertCode("@@@", "A4CF");
        }

        // ................::.:.110010.:::.
        // Reko: a decoder for V850 instruction 4ED6 at address 0010199E has not been implemented.
        [Test]
        public void V850Dis_4ED6()
        {
            AssertCode("@@@", "4ED6");
        }

        // ................:.:::011001.:..:
        // ................:.:::.::..:.:..: sld_b
        // ................:.:::001111...:.
        // Reko: a decoder for V850 instruction E2B9 at address 001019A2 has not been implemented.
        [Test]
        public void V850Dis_E2B9()
        {
            AssertCode("@@@", "E2B9");
        }

        // ................:...:111010::::.
        // Reko: a decoder for V850 instruction 5E8F at address 001019A4 has not been implemented.
        [Test]
        public void V850Dis_5E8F()
        {
            AssertCode("@@@", "5E8F");
        }

        // ................:....011110..:::
        // ................:.....::::...::: sst_b
        // .................::::110110:..::
        // Reko: a decoder for V850 instruction D37E at address 001019A8 has not been implemented.
        [Test]
        public void V850Dis_D37E()
        {
            AssertCode("@@@", "D37E");
        }

        // ................:.::.110100..:::
        // Reko: a decoder for V850 instruction 87B6 at address 001019AA has not been implemented.
        [Test]
        public void V850Dis_87B6()
        {
            AssertCode("@@@", "87B6");
        }

        // ................:.:::010001.:...
        // Reko: a decoder for V850 instruction 28BA at address 001019AC has not been implemented.
        [Test]
        public void V850Dis_28BA()
        {
            AssertCode("@@@", "28BA");
        }

        // .................:.::011111:.:.:
        // .................:.::.::::::.:.: sst_b
        // ...................::111011.:...
        // Reko: a decoder for V850 instruction 681F at address 001019B0 has not been implemented.
        [Test]
        public void V850Dis_681F()
        {
            AssertCode("@@@", "681F");
        }

        // ..................:::100010...::
        // ..................::::...:....:: sld_h
        // ................::..:001011::...
        // Reko: a decoder for V850 instruction 78C9 at address 001019B4 has not been implemented.
        [Test]
        public void V850Dis_78C9()
        {
            AssertCode("@@@", "78C9");
        }

        // ................:::.:001011..:::
        // Reko: a decoder for V850 instruction 67E9 at address 001019B6 has not been implemented.
        [Test]
        public void V850Dis_67E9()
        {
            AssertCode("@@@", "67E9");
        }

        // .....................011000:::::
        // ......................::...::::: sld_b
        // ................:.:..100011.:...
        // ................:.:..:...::.:... sld_h
        // ...................::011101::::.
        // ...................::.:::.:::::. sst_b
        // ...................:.100001.:...
        // ...................:.:....:.:... sld_h
        // ..................:::001010.::::
        // Reko: a decoder for V850 instruction 4F39 at address 001019C0 has not been implemented.
        [Test]
        public void V850Dis_4F39()
        {
            AssertCode("@@@", "4F39");
        }

        // ................:::.:100110.....
        // ................:::.::..::...... sst_h
        // .................:..:010110::::.
        // Reko: a decoder for V850 instruction DE4A at address 001019C4 has not been implemented.
        [Test]
        public void V850Dis_DE4A()
        {
            AssertCode("@@@", "DE4A");
        }

        // .................::.:110100..:.:
        // Reko: a decoder for V850 instruction 856E at address 001019C6 has not been implemented.
        [Test]
        public void V850Dis_856E()
        {
            AssertCode("@@@", "856E");
        }

        // ................:..::111001..:.:
        // Reko: a decoder for V850 instruction 259F at address 001019C8 has not been implemented.
        [Test]
        public void V850Dis_259F()
        {
            AssertCode("@@@", "259F");
        }

        // ................::.::111111....:
        // Reko: a decoder for V850 instruction E1DF at address 001019CA has not been implemented.
        [Test]
        public void V850Dis_E1DF()
        {
            AssertCode("@@@", "E1DF");
        }

        // ................:..::011000..:.:
        // ................:..::.::.....:.: sld_b
        // ................::.:.101101:.:.:
        // Reko: a decoder for V850 instruction B5D5 at address 001019CE has not been implemented.
        [Test]
        public void V850Dis_B5D5()
        {
            AssertCode("@@@", "B5D5");
        }

        // .................:.::101101.::.:
        // Reko: a decoder for V850 instruction AD5D at address 001019D0 has not been implemented.
        [Test]
        public void V850Dis_AD5D()
        {
            AssertCode("@@@", "AD5D");
        }

        // ................:....100100.:...
        // ................:....:..:...:... sst_h
        // ...................:.010100.:.::
        // Reko: a decoder for V850 instruction 8B12 at address 001019D4 has not been implemented.
        [Test]
        public void V850Dis_8B12()
        {
            AssertCode("@@@", "8B12");
        }

        // .................::..000010:::.:
        // ................01100....:.:::.:
        // .................::......:.11101
        // .................::......:.:::.: divh
        // ................:::::000010:..::
        // ................11111....:.:..::
        // ................:::::....:.10011
        // ................:::::....:.:..:: divh
        // .................:.::010100::.:.
        // Reko: a decoder for V850 instruction 9A5A at address 001019DA has not been implemented.
        [Test]
        public void V850Dis_9A5A()
        {
            AssertCode("@@@", "9A5A");
        }

        // ..................:..010001..:::
        // Reko: a decoder for V850 instruction 2722 at address 001019DC has not been implemented.
        [Test]
        public void V850Dis_2722()
        {
            AssertCode("@@@", "2722");
        }

        // ................:.::.111110..:.:
        // Reko: a decoder for V850 instruction C5B7 at address 001019DE has not been implemented.
        [Test]
        public void V850Dis_C5B7()
        {
            AssertCode("@@@", "C5B7");
        }

        // .................:::.010010:.::.
        // Reko: a decoder for V850 instruction 5672 at address 001019E0 has not been implemented.
        [Test]
        public void V850Dis_5672()
        {
            AssertCode("@@@", "5672");
        }

        // ................:::.:110011..::.
        // Reko: a decoder for V850 instruction 66EE at address 001019E2 has not been implemented.
        [Test]
        public void V850Dis_66EE()
        {
            AssertCode("@@@", "66EE");
        }

        // ....................:001111::..:
        // Reko: a decoder for V850 instruction F909 at address 001019E4 has not been implemented.
        [Test]
        public void V850Dis_F909()
        {
            AssertCode("@@@", "F909");
        }

        // ...................::101011:::.:
        // Reko: a decoder for V850 instruction 7D1D at address 001019E6 has not been implemented.
        [Test]
        public void V850Dis_7D1D()
        {
            AssertCode("@@@", "7D1D");
        }

        // ................::.:.100100::.:.
        // ................::.:.:..:..::.:. sst_h
        // ................::.::000000:.::.
        // ................11011......:.::.
        // ................::.::......:.::. mov
        // ................:....111111..:::
        // Reko: a decoder for V850 instruction E787 at address 001019EC has not been implemented.
        [Test]
        public void V850Dis_E787()
        {
            AssertCode("@@@", "E787");
        }

        // ................:....011111...::
        // ................:.....:::::...:: sst_b
        // ................:..:.011010.:::.
        // ................:..:..::.:..:::. sld_b
        // .................::..000111:.:::
        // Reko: a decoder for V850 instruction F760 at address 001019F2 has not been implemented.
        [Test]
        public void V850Dis_F760()
        {
            AssertCode("@@@", "F760");
        }

        // ................:.:..011100:..::
        // ................:.:...:::..:..:: sst_b
        // ..................:::101100.:...
        // Reko: a decoder for V850 instruction 883D at address 001019F6 has not been implemented.
        [Test]
        public void V850Dis_883D()
        {
            AssertCode("@@@", "883D");
        }

        // .................:.::111110::.::
        // Reko: a decoder for V850 instruction DB5F at address 001019F8 has not been implemented.
        [Test]
        public void V850Dis_DB5F()
        {
            AssertCode("@@@", "DB5F");
        }

        // ................:::..110100::...
        // Reko: a decoder for V850 instruction 98E6 at address 001019FA has not been implemented.
        [Test]
        public void V850Dis_98E6()
        {
            AssertCode("@@@", "98E6");
        }

        // ..................:.:010101:....
        // Reko: a decoder for V850 instruction B02A at address 001019FC has not been implemented.
        [Test]
        public void V850Dis_B02A()
        {
            AssertCode("@@@", "B02A");
        }

        // .....................011010..::.
        // ......................::.:...::. sld_b
        // ..................:.:010110::.:.
        // Reko: a decoder for V850 instruction DA2A at address 00101A00 has not been implemented.
        [Test]
        public void V850Dis_DA2A()
        {
            AssertCode("@@@", "DA2A");
        }

        // ................:..:.011100.:..:
        // ................:..:..:::...:..: sst_b
        // ................::...110001:.:..
        // Reko: a decoder for V850 instruction 34C6 at address 00101A04 has not been implemented.
        [Test]
        public void V850Dis_34C6()
        {
            AssertCode("@@@", "34C6");
        }

        // ................:.:..100110::.::
        // ................:.:..:..::.::.:: sst_h
        // .................::.:010111.....
        // Reko: a decoder for V850 instruction E06A at address 00101A08 has not been implemented.
        [Test]
        public void V850Dis_E06A()
        {
            AssertCode("@@@", "E06A");
        }

        // ................:.::.110111.:..:
        // Reko: a decoder for V850 instruction E9B6 at address 00101A0A has not been implemented.
        [Test]
        public void V850Dis_E9B6()
        {
            AssertCode("@@@", "E9B6");
        }

        // ................:....100111::.::
        // ................:....:..:::::.:: sst_h
        // ................::..:011011::..:
        // ................::..:.::.::::..: sld_b
        // ...................:.110000:.:.:
        // Reko: a decoder for V850 instruction 1516 at address 00101A10 has not been implemented.
        [Test]
        public void V850Dis_1516()
        {
            AssertCode("@@@", "1516");
        }

        // ................:..:.100000.::..
        // ................:..:.:......::.. sld_h
        // .................::..100001.:.:.
        // .................::..:....:.:.:. sld_h
        // ................:.::.010001...::
        // Reko: a decoder for V850 instruction 23B2 at address 00101A16 has not been implemented.
        [Test]
        public void V850Dis_23B2()
        {
            AssertCode("@@@", "23B2");
        }

        // ..................:.:001111.:.:.
        // Reko: a decoder for V850 instruction EA29 at address 00101A18 has not been implemented.
        [Test]
        public void V850Dis_EA29()
        {
            AssertCode("@@@", "EA29");
        }

        // ................:..::010000:..::
        // ................:..::.:....10011
        // ................:..::.:....:..:: mov
        // ................:..:.111011:....
        // Reko: a decoder for V850 instruction 7097 at address 00101A1C has not been implemented.
        [Test]
        public void V850Dis_7097()
        {
            AssertCode("@@@", "7097");
        }

        // .................:::.000011.::.:
        // Reko: a decoder for V850 instruction 6D70 at address 00101A1E has not been implemented.
        [Test]
        public void V850Dis_6D70()
        {
            AssertCode("@@@", "6D70");
        }

        // ...................:.011100.:::.
        // ...................:..:::...:::. sst_b
        // ................:.:::100111::::.
        // ................:.::::..:::::::. sst_h
        // .................:.:.000111::::.
        // Reko: a decoder for V850 instruction FE50 at address 00101A24 has not been implemented.
        [Test]
        public void V850Dis_FE50()
        {
            AssertCode("@@@", "FE50");
        }

        // ................:....111010.:::.
        // Reko: a decoder for V850 instruction 4E87 at address 00101A26 has not been implemented.
        [Test]
        public void V850Dis_4E87()
        {
            AssertCode("@@@", "4E87");
        }

        // .................:.:.010101:.:::
        // Reko: a decoder for V850 instruction B752 at address 00101A28 has not been implemented.
        [Test]
        public void V850Dis_B752()
        {
            AssertCode("@@@", "B752");
        }

        // ..................:.:001100.....
        // Reko: a decoder for V850 instruction 8029 at address 00101A2A has not been implemented.
        [Test]
        public void V850Dis_8029()
        {
            AssertCode("@@@", "8029");
        }

        // ................::.:.001101::..:
        // Reko: a decoder for V850 instruction B9D1 at address 00101A2C has not been implemented.
        [Test]
        public void V850Dis_B9D1()
        {
            AssertCode("@@@", "B9D1");
        }

        // .................:.:.111010....:
        // ................::..:100111..::.
        // ................::..::..:::..::. sst_h
        // ................:..::001110:...:
        // Reko: a decoder for V850 instruction D199 at address 00101A32 has not been implemented.
        [Test]
        public void V850Dis_D199()
        {
            AssertCode("@@@", "D199");
        }

        // ................:.::.010000:::.:
        // ................:.::..:....11101
        // ................:.::..:....:::.: mov
        // .................:::.110110::...
        // Reko: a decoder for V850 instruction D876 at address 00101A36 has not been implemented.
        [Test]
        public void V850Dis_D876()
        {
            AssertCode("@@@", "D876");
        }

        // ................::.:.110000..:::
        // Reko: a decoder for V850 instruction 07D6 at address 00101A38 has not been implemented.
        [Test]
        public void V850Dis_07D6()
        {
            AssertCode("@@@", "07D6");
        }

        // ....................:011101.:...
        // ....................:.:::.:.:... sst_b
        // .................:...001101:..::
        // Reko: a decoder for V850 instruction B341 at address 00101A3C has not been implemented.
        [Test]
        public void V850Dis_B341()
        {
            AssertCode("@@@", "B341");
        }

        // .................:..:001001:..::
        // Reko: a decoder for V850 instruction 3349 at address 00101A3E has not been implemented.
        [Test]
        public void V850Dis_3349()
        {
            AssertCode("@@@", "3349");
        }

        // .................::::001111:::..
        // Reko: a decoder for V850 instruction FC79 at address 00101A40 has not been implemented.
        [Test]
        public void V850Dis_FC79()
        {
            AssertCode("@@@", "FC79");
        }

        // ................:::..111001.:...
        // Reko: a decoder for V850 instruction 28E7 at address 00101A42 has not been implemented.
        [Test]
        public void V850Dis_28E7()
        {
            AssertCode("@@@", "28E7");
        }

        // ................:.:::100100..::.
        // ................:.::::..:....::. sst_h
        // ................::...100110.::.:
        // ................::...:..::..::.: sst_h
        // ................:....101110.:...
        // Reko: a decoder for V850 instruction C885 at address 00101A48 has not been implemented.
        [Test]
        public void V850Dis_C885()
        {
            AssertCode("@@@", "C885");
        }

        // ................::.::000110:::..
        // Reko: a decoder for V850 instruction DCD8 at address 00101A4A has not been implemented.
        [Test]
        public void V850Dis_DCD8()
        {
            AssertCode("@@@", "DCD8");
        }

        // ................:::::001111..:..
        // Reko: a decoder for V850 instruction E4F9 at address 00101A4C has not been implemented.
        [Test]
        public void V850Dis_E4F9()
        {
            AssertCode("@@@", "E4F9");
        }

        // ...................::000111::...
        // Reko: a decoder for V850 instruction F818 at address 00101A4E has not been implemented.
        [Test]
        public void V850Dis_F818()
        {
            AssertCode("@@@", "F818");
        }

        // ................:.:::100111..:::
        // ................:.::::..:::..::: sst_h
        // ................:.:..011101:..::
        // ................:.:...:::.::..:: sst_b
        // ................::.:.011110..:.:
        // ................::.:..::::...:.: sst_b
        // ................::.::011010:..:.
        // ................::.::.::.:.:..:. sld_b
        // ................:...:000001::.::
        // ................:...:.....:::.:: not
        // ..................:..111101:.:..
        // Reko: a decoder for V850 instruction B427 at address 00101A5A has not been implemented.
        [Test]
        public void V850Dis_B427()
        {
            AssertCode("@@@", "B427");
        }

        // .................:...010101.:...
        // Reko: a decoder for V850 instruction A842 at address 00101A5C has not been implemented.
        [Test]
        public void V850Dis_A842()
        {
            AssertCode("@@@", "A842");
        }

        // .................:::.110110:.:..
        // Reko: a decoder for V850 instruction D476 at address 00101A5E has not been implemented.
        [Test]
        public void V850Dis_D476()
        {
            AssertCode("@@@", "D476");
        }

        // ..................::.110000::::.
        // Reko: a decoder for V850 instruction 1E36 at address 00101A60 has not been implemented.
        [Test]
        public void V850Dis_1E36()
        {
            AssertCode("@@@", "1E36");
        }

        // .................:...000001.::.:
        // .................:........:.::.: not
        // .................:.::001101.:...
        // Reko: a decoder for V850 instruction A859 at address 00101A64 has not been implemented.
        [Test]
        public void V850Dis_A859()
        {
            AssertCode("@@@", "A859");
        }

        // ..................:.:001110:...:
        // Reko: a decoder for V850 instruction D129 at address 00101A66 has not been implemented.
        [Test]
        public void V850Dis_D129()
        {
            AssertCode("@@@", "D129");
        }

        // ....................:100111:::::
        // ....................::..:::::::: sst_h
        // ................:..::000110:...:
        // Reko: a decoder for V850 instruction D198 at address 00101A6A has not been implemented.
        [Test]
        public void V850Dis_D198()
        {
            AssertCode("@@@", "D198");
        }

        // ................:::::111110:.:.:
        // Reko: a decoder for V850 instruction D5FF at address 00101A6C has not been implemented.
        [Test]
        public void V850Dis_D5FF()
        {
            AssertCode("@@@", "D5FF");
        }

        // ....................:110100:..:.
        // Reko: a decoder for V850 instruction 920E at address 00101A6E has not been implemented.
        [Test]
        public void V850Dis_920E()
        {
            AssertCode("@@@", "920E");
        }

        // ................:...:111110::::.
        // Reko: a decoder for V850 instruction DE8F at address 00101A70 has not been implemented.
        [Test]
        public void V850Dis_DE8F()
        {
            AssertCode("@@@", "DE8F");
        }

        // ................::::.111001...::
        // Reko: a decoder for V850 instruction 23F7 at address 00101A72 has not been implemented.
        [Test]
        public void V850Dis_23F7()
        {
            AssertCode("@@@", "23F7");
        }

        // ................:..:.000100.::.:
        // Reko: a decoder for V850 instruction 8D90 at address 00101A74 has not been implemented.
        [Test]
        public void V850Dis_8D90()
        {
            AssertCode("@@@", "8D90");
        }

        // ..................:.:001100...::
        // Reko: a decoder for V850 instruction 8329 at address 00101A76 has not been implemented.
        [Test]
        public void V850Dis_8329()
        {
            AssertCode("@@@", "8329");
        }

        // .................:..:100001..::.
        // .................:..::....:..::. sld_h
        // ................:::::101101.::..
        // ................:.:..101000.:...
        // ................:.:..:.:....:..0 Sld.w/Sst.w
        // ................:.:..:.:....:... sld_w
        // ................:....101000:::..
        // ................:....:.:...:::.0 Sld.w/Sst.w
        // ................:....:.:...:::.. sld_w
        // .................::..110101:....
        // Reko: a decoder for V850 instruction B066 at address 00101A80 has not been implemented.
        [Test]
        public void V850Dis_B066()
        {
            AssertCode("@@@", "B066");
        }

        // ................:.:.:101101::..:
        // Reko: a decoder for V850 instruction B9AD at address 00101A82 has not been implemented.
        [Test]
        public void V850Dis_B9AD()
        {
            AssertCode("@@@", "B9AD");
        }

        // ................:.::.111110.::..
        // Reko: a decoder for V850 instruction CCB7 at address 00101A84 has not been implemented.
        [Test]
        public void V850Dis_CCB7()
        {
            AssertCode("@@@", "CCB7");
        }

        // ................:...:110011.::::
        // Reko: a decoder for V850 instruction 6F8E at address 00101A86 has not been implemented.
        [Test]
        public void V850Dis_6F8E()
        {
            AssertCode("@@@", "6F8E");
        }

        // ................:...:001111.:.::
        // Reko: a decoder for V850 instruction EB89 at address 00101A88 has not been implemented.
        [Test]
        public void V850Dis_EB89()
        {
            AssertCode("@@@", "EB89");
        }

        // ................:.:::001101..:..
        // Reko: a decoder for V850 instruction A4B9 at address 00101A8A has not been implemented.
        [Test]
        public void V850Dis_A4B9()
        {
            AssertCode("@@@", "A4B9");
        }

        // ................:.:::010000...:.
        // ................:.:::.:....00010
        // ................:.:::.:.......:. mov
        // ..................:::011000..::.
        // ..................:::.::.....::. sld_b
        // ................:.:..110111:.::.
        // Reko: a decoder for V850 instruction F6A6 at address 00101A90 has not been implemented.
        [Test]
        public void V850Dis_F6A6()
        {
            AssertCode("@@@", "F6A6");
        }

        // ...................::000010.:..:
        // ................00011....:..:..:
        // ...................::....:.01001
        // ...................::....:..:..: divh
        // ................::.::101000:::::
        // ................::.:::.:...::::1 Sld.w/Sst.w
        // ................::.:::.:...::::: sst_w
        // ................:...:110111.:...
        // Reko: a decoder for V850 instruction E88E at address 00101A96 has not been implemented.
        [Test]
        public void V850Dis_E88E()
        {
            AssertCode("@@@", "E88E");
        }

        // .....................011101:..::
        // ......................:::.::..:: sst_b
        // .................:.::100000::.:.
        // .................:.:::.....::.:. sld_h
        // ................::.:.011101.::.:
        // ................::.:..:::.:.::.: sst_b
        // .................:.::001001....:
        // Reko: a decoder for V850 instruction 2159 at address 00101A9E has not been implemented.
        [Test]
        public void V850Dis_2159()
        {
            AssertCode("@@@", "2159");
        }

        // .................:..:011111:....
        // .................:..:.::::::.... sst_b
        // .................::.:110111..:::
        // Reko: a decoder for V850 instruction E76E at address 00101AA2 has not been implemented.
        [Test]
        public void V850Dis_E76E()
        {
            AssertCode("@@@", "E76E");
        }

        // ................:::..100010..::.
        // ................:::..:...:...::. sld_h
        // ..................:..110010...:.
        // Reko: a decoder for V850 instruction 4226 at address 00101AA6 has not been implemented.
        [Test]
        public void V850Dis_4226()
        {
            AssertCode("@@@", "4226");
        }

        // ................:..:.101001.....
        // Reko: a decoder for V850 instruction 2095 at address 00101AA8 has not been implemented.
        [Test]
        public void V850Dis_2095()
        {
            AssertCode("@@@", "2095");
        }

        // .....................100000.::..
        // .....................:......::.. sld_h
        // ................::..:010111.:.::
        // Reko: a decoder for V850 instruction EBCA at address 00101AAC has not been implemented.
        [Test]
        public void V850Dis_EBCA()
        {
            AssertCode("@@@", "EBCA");
        }

        // .................::.:011001..::.
        // .................::.:.::..:..::. sld_b
        // .................:::.000010..:.:
        // ................01110....:...:.:
        // .................:::.....:.00101
        // .................:::.....:...:.: divh
        // .................::..001000..:::
        // .................::....:.....::: or
        // .....................100010:::..
        // .....................:...:.:::.. sld_h
        // ................::.:.011011..::.
        // ................::.:..::.::..::. sld_b
        // ..................:.:010011::::.
        // Reko: a decoder for V850 instruction 7E2A at address 00101AB8 has not been implemented.
        [Test]
        public void V850Dis_7E2A()
        {
            AssertCode("@@@", "7E2A");
        }

        // ................::..:000111::..:
        // Reko: a decoder for V850 instruction F9C8 at address 00101ABA has not been implemented.
        [Test]
        public void V850Dis_F9C8()
        {
            AssertCode("@@@", "F9C8");
        }

        // ...................:.011100:::.:
        // ...................:..:::..:::.: sst_b
        // ...................:.111111.....
        // Reko: a decoder for V850 instruction E017 at address 00101ABE has not been implemented.
        [Test]
        public void V850Dis_E017()
        {
            AssertCode("@@@", "E017");
        }

        // .................:..:010011..:::
        // Reko: a decoder for V850 instruction 674A at address 00101AC0 has not been implemented.
        [Test]
        public void V850Dis_674A()
        {
            AssertCode("@@@", "674A");
        }

        // ................:.::.111110.:...
        // Reko: a decoder for V850 instruction C8B7 at address 00101AC2 has not been implemented.
        [Test]
        public void V850Dis_C8B7()
        {
            AssertCode("@@@", "C8B7");
        }

        // ................:..::000110:..:.
        // Reko: a decoder for V850 instruction D298 at address 00101AC4 has not been implemented.
        [Test]
        public void V850Dis_D298()
        {
            AssertCode("@@@", "D298");
        }

        // ...................:.001111:.:.:
        // Reko: a decoder for V850 instruction F511 at address 00101AC6 has not been implemented.
        [Test]
        public void V850Dis_F511()
        {
            AssertCode("@@@", "F511");
        }

        // ..................:..010011..:..
        // Reko: a decoder for V850 instruction 6422 at address 00101AC8 has not been implemented.
        [Test]
        public void V850Dis_6422()
        {
            AssertCode("@@@", "6422");
        }

        // ................:...:010111::...
        // Reko: a decoder for V850 instruction F88A at address 00101ACA has not been implemented.
        [Test]
        public void V850Dis_F88A()
        {
            AssertCode("@@@", "F88A");
        }

        // ................:::..011110:...:
        // ................:::...::::.:...: sst_b
        // ................:::.:001101...::
        // Reko: a decoder for V850 instruction A3E9 at address 00101ACE has not been implemented.
        [Test]
        public void V850Dis_A3E9()
        {
            AssertCode("@@@", "A3E9");
        }

        // ................:.:.:110110:::.:
        // Reko: a decoder for V850 instruction DDAE at address 00101AD0 has not been implemented.
        [Test]
        public void V850Dis_DDAE()
        {
            AssertCode("@@@", "DDAE");
        }

        // ..................:::101000:.::.
        // ..................::::.:...:.::0 Sld.w/Sst.w
        // ..................::::.:...:.::. sld_w
        // ................::...000110.::..
        // Reko: a decoder for V850 instruction CCC0 at address 00101AD4 has not been implemented.
        [Test]
        public void V850Dis_CCC0()
        {
            AssertCode("@@@", "CCC0");
        }

        // ................:::.:100111:::.:
        // ................:::.::..::::::.: sst_h
        // ...................:.011111..:.:
        // ...................:..:::::..:.: sst_b
        // ...................::011111:....
        // ...................::.::::::.... sst_b
        // .................::::001111.:.:.
        // ....................:100100.:...
        // ....................::..:...:... sst_h
        // ................::::.100101:.:..
        // ................::::.:..:.::.:.. sst_h
        // ................:.:..000100:::.:
        // Reko: a decoder for V850 instruction 9DA0 at address 00101AE2 has not been implemented.
        [Test]
        public void V850Dis_9DA0()
        {
            AssertCode("@@@", "9DA0");
        }

        // .................::.:111000....:
        // Reko: a decoder for V850 instruction 016F at address 00101AE4 has not been implemented.
        [Test]
        public void V850Dis_016F()
        {
            AssertCode("@@@", "016F");
        }

        // ................:.:.:101100::..:
        // Reko: a decoder for V850 instruction 99AD at address 00101AE6 has not been implemented.
        [Test]
        public void V850Dis_99AD()
        {
            AssertCode("@@@", "99AD");
        }

        // ................::::.101011::.:.
        // Reko: a decoder for V850 instruction 7AF5 at address 00101AE8 has not been implemented.
        [Test]
        public void V850Dis_7AF5()
        {
            AssertCode("@@@", "7AF5");
        }

        // ..................:::001111:::.:
        // Reko: a decoder for V850 instruction FD39 at address 00101AEA has not been implemented.
        [Test]
        public void V850Dis_FD39()
        {
            AssertCode("@@@", "FD39");
        }

        // ................::.::100000.::..
        // ................::.:::......::.. sld_h
        // ................:..::010000...:.
        // ................:..::.:....00010
        // ................:..::.:.......:. mov
        // ...................:.000100..:::
        // Reko: a decoder for V850 instruction 8710 at address 00101AF0 has not been implemented.
        [Test]
        public void V850Dis_8710()
        {
            AssertCode("@@@", "8710");
        }

        // ................:::.:111111.:.:.
        // Reko: a decoder for V850 instruction EAEF at address 00101AF2 has not been implemented.
        [Test]
        public void V850Dis_EAEF()
        {
            AssertCode("@@@", "EAEF");
        }

        // ................:.:..010111:::.:
        // Reko: a decoder for V850 instruction FDA2 at address 00101AF4 has not been implemented.
        [Test]
        public void V850Dis_FDA2()
        {
            AssertCode("@@@", "FDA2");
        }

        // ................:.:..111010.:.::
        // Reko: a decoder for V850 instruction 4BA7 at address 00101AF6 has not been implemented.
        [Test]
        public void V850Dis_4BA7()
        {
            AssertCode("@@@", "4BA7");
        }

        // ................::.::100010.::.:
        // ................::.:::...:..::.: sld_h
        // ................:.:::111110:...:
        // Reko: a decoder for V850 instruction D1BF at address 00101AFA has not been implemented.
        [Test]
        public void V850Dis_D1BF()
        {
            AssertCode("@@@", "D1BF");
        }

        // ................::.::001101..:::
        // Reko: a decoder for V850 instruction A7D9 at address 00101AFC has not been implemented.
        [Test]
        public void V850Dis_A7D9()
        {
            AssertCode("@@@", "A7D9");
        }

        // ................:::.:010001..::.
        // Reko: a decoder for V850 instruction 26EA at address 00101AFE has not been implemented.
        [Test]
        public void V850Dis_26EA()
        {
            AssertCode("@@@", "26EA");
        }

        // ................:::.:011100::.:.
        // ................:::.:.:::..::.:. sst_b
        // ................::.::100110:.:.:
        // ................::.:::..::.:.:.: sst_h
        // .....................010010..::.
        // Reko: a decoder for V850 instruction 4602 at address 00101B04 has not been implemented.
        [Test]
        public void V850Dis_4602()
        {
            AssertCode("@@@", "4602");
        }

        // .................::.:101111..:::
        // Reko: a decoder for V850 instruction E76D at address 00101B06 has not been implemented.
        [Test]
        public void V850Dis_E76D()
        {
            AssertCode("@@@", "E76D");
        }

        // .................::.:000000:.:.:
        // ................01101......:.:.:
        // .................::.:......:.:.: mov
        // ................::.:.110110...:.
        // Reko: a decoder for V850 instruction C2D6 at address 00101B0A has not been implemented.
        [Test]
        public void V850Dis_C2D6()
        {
            AssertCode("@@@", "C2D6");
        }

        // ................::...011110...:.
        // ................::....::::....:. sst_b
        // ....................:001111.....
        // Reko: a decoder for V850 instruction E009 at address 00101B0E has not been implemented.
        [Test]
        public void V850Dis_E009()
        {
            AssertCode("@@@", "E009");
        }

        // .................:.:.110000:....
        // Reko: a decoder for V850 instruction 1056 at address 00101B10 has not been implemented.
        [Test]
        public void V850Dis_1056()
        {
            AssertCode("@@@", "1056");
        }

        // ................::.::001100:.:..
        // Reko: a decoder for V850 instruction 94D9 at address 00101B12 has not been implemented.
        [Test]
        public void V850Dis_94D9()
        {
            AssertCode("@@@", "94D9");
        }

        // ................:..::001100.:::.
        // Reko: a decoder for V850 instruction 8E99 at address 00101B14 has not been implemented.
        [Test]
        public void V850Dis_8E99()
        {
            AssertCode("@@@", "8E99");
        }

        // ...................:.001000.::::
        // ...................:...:....:::: or
        // ................::::.101101.:..:
        // Reko: a decoder for V850 instruction A9F5 at address 00101B18 has not been implemented.
        [Test]
        public void V850Dis_A9F5()
        {
            AssertCode("@@@", "A9F5");
        }

        // ..................:..100010:..:.
        // ..................:..:...:.:..:. sld_h
        // ................::.::001100:::.:
        // Reko: a decoder for V850 instruction 9DD9 at address 00101B1C has not been implemented.
        [Test]
        public void V850Dis_9DD9()
        {
            AssertCode("@@@", "9DD9");
        }

        // ................:.:..001000..:.:
        // ................:.:....:.....:.: or
        // ...................:.010110.:::.
        // Reko: a decoder for V850 instruction CE12 at address 00101B20 has not been implemented.
        [Test]
        public void V850Dis_CE12()
        {
            AssertCode("@@@", "CE12");
        }

        // ..................:..000100:::::
        // Reko: a decoder for V850 instruction 9F20 at address 00101B22 has not been implemented.
        [Test]
        public void V850Dis_9F20()
        {
            AssertCode("@@@", "9F20");
        }

        // ..................:..110000..::.
        // Reko: a decoder for V850 instruction 0626 at address 00101B24 has not been implemented.
        [Test]
        public void V850Dis_0626()
        {
            AssertCode("@@@", "0626");
        }

        // ................:....101010:..::
        // Reko: a decoder for V850 instruction 5385 at address 00101B26 has not been implemented.
        [Test]
        public void V850Dis_5385()
        {
            AssertCode("@@@", "5385");
        }

        // .................::::101001.:...
        // Reko: a decoder for V850 instruction 287D at address 00101B28 has not been implemented.
        [Test]
        public void V850Dis_287D()
        {
            AssertCode("@@@", "287D");
        }

        // ................:..:.100110::..:
        // ................:..:.:..::.::..: sst_h
        // .................:::.100111.....
        // .................:::.:..:::..... sst_h
        // ................:...:010111..:..
        // Reko: a decoder for V850 instruction E48A at address 00101B2E has not been implemented.
        [Test]
        public void V850Dis_E48A()
        {
            AssertCode("@@@", "E48A");
        }

        // ................::..:000111:::..
        // Reko: a decoder for V850 instruction FCC8 at address 00101B30 has not been implemented.
        [Test]
        public void V850Dis_FCC8()
        {
            AssertCode("@@@", "FCC8");
        }

        // .................:.:.001101.:::.
        // Reko: a decoder for V850 instruction AE51 at address 00101B32 has not been implemented.
        [Test]
        public void V850Dis_AE51()
        {
            AssertCode("@@@", "AE51");
        }

        // .................:..:100010..:.:
        // .................:..::...:...:.: sld_h
        // .....................000010::.::
        // ................00000....:.::.::
        // .........................:.11011
        // .........................:.::.:: switch
        // ................::...011110::.::
        // ................::....::::.::.:: sst_b
        // .................:.:.001001:..::
        // Reko: a decoder for V850 instruction 3351 at address 00101B3A has not been implemented.
        [Test]
        public void V850Dis_3351()
        {
            AssertCode("@@@", "3351");
        }

        // ................::...011101.::::
        // ................::....:::.:.:::: sst_b
        // ..................:::011110.::::
        // ..................:::.::::..:::: sst_b
        // ..................:..000011...:.
        // Reko: a decoder for V850 instruction 6220 at address 00101B40 has not been implemented.
        [Test]
        public void V850Dis_6220()
        {
            AssertCode("@@@", "6220");
        }

        // ................:.::.000000.:...
        // ................10110.......:...
        // ................:.::........:... mov
        // ................:.:::111001..:..
        // Reko: a decoder for V850 instruction 24BF at address 00101B44 has not been implemented.
        [Test]
        public void V850Dis_24BF()
        {
            AssertCode("@@@", "24BF");
        }

        // ................:::.:010000...:.
        // ................:::.:.:....00010
        // ................:::.:.:.......:. mov
        // ....................:110000...:.
        // ...................:.001101:...:
        // Reko: a decoder for V850 instruction B111 at address 00101B4A has not been implemented.
        [Test]
        public void V850Dis_B111()
        {
            AssertCode("@@@", "B111");
        }

        // .................::::011101:::::
        // .................::::.:::.:::::: sst_b
        // ..................::.100001:...:
        // ..................::.:....::...: sld_h
        // ................::..:000000:...:
        // ................11001......:...:
        // ................::..:......:...: mov
        // ................::.:.101001..:::
        // Reko: a decoder for V850 instruction 27D5 at address 00101B52 has not been implemented.
        [Test]
        public void V850Dis_27D5()
        {
            AssertCode("@@@", "27D5");
        }

        // ................:.::.100001.:.::
        // ................:.::.:....:.:.:: sld_h
        // ................:....001010:::..
        // Reko: a decoder for V850 instruction 5C81 at address 00101B56 has not been implemented.
        [Test]
        public void V850Dis_5C81()
        {
            AssertCode("@@@", "5C81");
        }

        // ................:..::110101:.::.
        // Reko: a decoder for V850 instruction B69E at address 00101B58 has not been implemented.
        [Test]
        public void V850Dis_B69E()
        {
            AssertCode("@@@", "B69E");
        }

        // .................:...011010..:..
        // .................:....::.:...:.. sld_b
        // .....................010111:..:.
        // Reko: a decoder for V850 instruction F202 at address 00101B5C has not been implemented.
        [Test]
        public void V850Dis_F202()
        {
            AssertCode("@@@", "F202");
        }

        // .................::..101110:.::.
        // Reko: a decoder for V850 instruction D665 at address 00101B5E has not been implemented.
        [Test]
        public void V850Dis_D665()
        {
            AssertCode("@@@", "D665");
        }

        // ................:..::110101.::.:
        // Reko: a decoder for V850 instruction AD9E at address 00101B60 has not been implemented.
        [Test]
        public void V850Dis_AD9E()
        {
            AssertCode("@@@", "AD9E");
        }

        // ................:.:::111001:...:
        // Reko: a decoder for V850 instruction 31BF at address 00101B62 has not been implemented.
        [Test]
        public void V850Dis_31BF()
        {
            AssertCode("@@@", "31BF");
        }

        // ..................::.100011.::..
        // ..................::.:...::.::.. sld_h
        // ..................:::101011..::.
        // Reko: a decoder for V850 instruction 663D at address 00101B66 has not been implemented.
        [Test]
        public void V850Dis_663D()
        {
            AssertCode("@@@", "663D");
        }

        // ................::...011110..::.
        // ................::....::::...::. sst_b
        // ..................::.110010.:::.
        // Reko: a decoder for V850 instruction 4E36 at address 00101B6A has not been implemented.
        [Test]
        public void V850Dis_4E36()
        {
            AssertCode("@@@", "4E36");
        }

        // .................:..:010100::.::
        // Reko: a decoder for V850 instruction 9B4A at address 00101B6C has not been implemented.
        [Test]
        public void V850Dis_9B4A()
        {
            AssertCode("@@@", "9B4A");
        }

        // .................::..000010.....
        // ................01100....:......
        // .................::......:.00000
        // .................::......:...... fetrap
        // ................:::::110100:..:.
        // Reko: a decoder for V850 instruction 92FE at address 00101B70 has not been implemented.
        [Test]
        public void V850Dis_92FE()
        {
            AssertCode("@@@", "92FE");
        }

        // ................:::..110001:::::
        // Reko: a decoder for V850 instruction 3FE6 at address 00101B72 has not been implemented.
        [Test]
        public void V850Dis_3FE6()
        {
            AssertCode("@@@", "3FE6");
        }

        // ................:::::010100:::..
        // Reko: a decoder for V850 instruction 9CFA at address 00101B74 has not been implemented.
        [Test]
        public void V850Dis_9CFA()
        {
            AssertCode("@@@", "9CFA");
        }

        // ................:.:.:110000:....
        // Reko: a decoder for V850 instruction 10AE at address 00101B76 has not been implemented.
        [Test]
        public void V850Dis_10AE()
        {
            AssertCode("@@@", "10AE");
        }

        // ................:....110110..:..
        // Reko: a decoder for V850 instruction C486 at address 00101B78 has not been implemented.
        [Test]
        public void V850Dis_C486()
        {
            AssertCode("@@@", "C486");
        }

        // ................:....101111::..:
        // Reko: a decoder for V850 instruction F985 at address 00101B7A has not been implemented.
        [Test]
        public void V850Dis_F985()
        {
            AssertCode("@@@", "F985");
        }

        // ................:.:::111011::.:.
        // Reko: a decoder for V850 instruction 7ABF at address 00101B7C has not been implemented.
        [Test]
        public void V850Dis_7ABF()
        {
            AssertCode("@@@", "7ABF");
        }

        // .....................000111:.:::
        // Reko: a decoder for V850 instruction F700 at address 00101B7E has not been implemented.
        [Test]
        public void V850Dis_F700()
        {
            AssertCode("@@@", "F700");
        }

        // .................:..:011001::...
        // .................:..:.::..:::... sld_b
        // ..................:..001011..:..
        // Reko: a decoder for V850 instruction 6421 at address 00101B82 has not been implemented.
        [Test]
        public void V850Dis_6421()
        {
            AssertCode("@@@", "6421");
        }

        // .................:::.001010.:..:
        // Reko: a decoder for V850 instruction 4971 at address 00101B84 has not been implemented.
        [Test]
        public void V850Dis_4971()
        {
            AssertCode("@@@", "4971");
        }

        // ................::.::101110..:::
        // Reko: a decoder for V850 instruction C7DD at address 00101B86 has not been implemented.
        [Test]
        public void V850Dis_C7DD()
        {
            AssertCode("@@@", "C7DD");
        }

        // ................:::.:010011...:.
        // Reko: a decoder for V850 instruction 62EA at address 00101B88 has not been implemented.
        [Test]
        public void V850Dis_62EA()
        {
            AssertCode("@@@", "62EA");
        }

        // .................::..111000.:::.
        // ..................:..110011..::.
        // Reko: a decoder for V850 instruction 6626 at address 00101B8C has not been implemented.
        [Test]
        public void V850Dis_6626()
        {
            AssertCode("@@@", "6626");
        }

        // .................::.:100111..:.:
        // .................::.::..:::..:.: sst_h
        // ................:::..100010:::.:
        // ................:::..:...:.:::.: sld_h
        // ................::::.011101:....
        // ................::::..:::.::.... sst_b
        // ................:::.:111110...:.
        // Reko: a decoder for V850 instruction C2EF at address 00101B94 has not been implemented.
        [Test]
        public void V850Dis_C2EF()
        {
            AssertCode("@@@", "C2EF");
        }

        // ................:.::.010110.:..:
        // Reko: a decoder for V850 instruction C9B2 at address 00101B96 has not been implemented.
        [Test]
        public void V850Dis_C9B2()
        {
            AssertCode("@@@", "C9B2");
        }

        // ................:....011100.::.:
        // ................:.....:::...::.: sst_b
        // ................:::..110111::.::
        // Reko: a decoder for V850 instruction FBE6 at address 00101B9A has not been implemented.
        [Test]
        public void V850Dis_FBE6()
        {
            AssertCode("@@@", "FBE6");
        }

        // ................:::..000001::.:.
        // ................:::.......:::.:. not
        // .....................110110...:.
        // Reko: a decoder for V850 instruction C206 at address 00101B9E has not been implemented.
        [Test]
        public void V850Dis_C206()
        {
            AssertCode("@@@", "C206");
        }

        // .................:..:101110.:.::
        // Reko: a decoder for V850 instruction CB4D at address 00101BA0 has not been implemented.
        [Test]
        public void V850Dis_CB4D()
        {
            AssertCode("@@@", "CB4D");
        }

        // .................:.:.000111:::.:
        // Reko: a decoder for V850 instruction FD50 at address 00101BA2 has not been implemented.
        [Test]
        public void V850Dis_FD50()
        {
            AssertCode("@@@", "FD50");
        }

        // ...................::111111..::.
        // Reko: a decoder for V850 instruction E61F at address 00101BA4 has not been implemented.
        [Test]
        public void V850Dis_E61F()
        {
            AssertCode("@@@", "E61F");
        }

        // ..................:..000000:.::.
        // ................00100......:.::.
        // ..................:........:.::. mov
        // .................::..010001:.::.
        // Reko: a decoder for V850 instruction 3662 at address 00101BA8 has not been implemented.
        [Test]
        public void V850Dis_3662()
        {
            AssertCode("@@@", "3662");
        }

        // ................:.::.001100..:..
        // Reko: a decoder for V850 instruction 84B1 at address 00101BAA has not been implemented.
        [Test]
        public void V850Dis_84B1()
        {
            AssertCode("@@@", "84B1");
        }

        // ................:.:.:000111.::..
        // Reko: a decoder for V850 instruction ECA8 at address 00101BAC has not been implemented.
        [Test]
        public void V850Dis_ECA8()
        {
            AssertCode("@@@", "ECA8");
        }

        // ................:..::110010.:...
        // Reko: a decoder for V850 instruction 489E at address 00101BAE has not been implemented.
        [Test]
        public void V850Dis_489E()
        {
            AssertCode("@@@", "489E");
        }

        // ................:...:100101.....
        // ................:...::..:.:..... sst_h
        // ................:..:.110001..:::
        // Reko: a decoder for V850 instruction 2796 at address 00101BB2 has not been implemented.
        [Test]
        public void V850Dis_2796()
        {
            AssertCode("@@@", "2796");
        }

        // .....................011000.:::.
        // ......................::....:::. sld_b
        // .................:..:001001::::.
        // Reko: a decoder for V850 instruction 3E49 at address 00101BB6 has not been implemented.
        [Test]
        public void V850Dis_3E49()
        {
            AssertCode("@@@", "3E49");
        }

        // ................:.::.001100...:.
        // ................::...110100:..::
        // Reko: a decoder for V850 instruction 93C6 at address 00101BBA has not been implemented.
        [Test]
        public void V850Dis_93C6()
        {
            AssertCode("@@@", "93C6");
        }

        // ................:::..001011:.::.
        // Reko: a decoder for V850 instruction 76E1 at address 00101BBC has not been implemented.
        [Test]
        public void V850Dis_76E1()
        {
            AssertCode("@@@", "76E1");
        }

        // ................:....010101...:.
        // Reko: a decoder for V850 instruction A282 at address 00101BBE has not been implemented.
        [Test]
        public void V850Dis_A282()
        {
            AssertCode("@@@", "A282");
        }

        // .....................111001..:::
        // Reko: a decoder for V850 instruction 2707 at address 00101BC0 has not been implemented.
        [Test]
        public void V850Dis_2707()
        {
            AssertCode("@@@", "2707");
        }

        // ...................::000100::.::
        // Reko: a decoder for V850 instruction 9B18 at address 00101BC2 has not been implemented.
        [Test]
        public void V850Dis_9B18()
        {
            AssertCode("@@@", "9B18");
        }

        // ................:..:.101001.:...
        // Reko: a decoder for V850 instruction 2895 at address 00101BC4 has not been implemented.
        [Test]
        public void V850Dis_2895()
        {
            AssertCode("@@@", "2895");
        }

        // ..................:::110100..::.
        // ................:..:.000110.::.:
        // Reko: a decoder for V850 instruction CD90 at address 00101BC8 has not been implemented.
        [Test]
        public void V850Dis_CD90()
        {
            AssertCode("@@@", "CD90");
        }

        // .................:.::111101:::.:
        // Reko: a decoder for V850 instruction BD5F at address 00101BCA has not been implemented.
        [Test]
        public void V850Dis_BD5F()
        {
            AssertCode("@@@", "BD5F");
        }

        // ...................:.111011.:.::
        // Reko: a decoder for V850 instruction 6B17 at address 00101BCC has not been implemented.
        [Test]
        public void V850Dis_6B17()
        {
            AssertCode("@@@", "6B17");
        }

        // ................:::..101110..::.
        // Reko: a decoder for V850 instruction C6E5 at address 00101BCE has not been implemented.
        [Test]
        public void V850Dis_C6E5()
        {
            AssertCode("@@@", "C6E5");
        }

        // .................:.::100001::.::
        // .................:.:::....:::.:: sld_h
        // ................:.:.:110010..:.:
        // Reko: a decoder for V850 instruction 45AE at address 00101BD2 has not been implemented.
        [Test]
        public void V850Dis_45AE()
        {
            AssertCode("@@@", "45AE");
        }

        // ..................:.:100101::...
        // ..................:.::..:.:::... sst_h
        // ................::..:000111::...
        // Reko: a decoder for V850 instruction F8C8 at address 00101BD6 has not been implemented.
        [Test]
        public void V850Dis_F8C8()
        {
            AssertCode("@@@", "F8C8");
        }

        // ................:.::.100000.::::
        // ................:.::.:......:::: sld_h
        // ..................::.100110.::.:
        // ..................::.:..::..::.: sst_h
        // .................:.:.000100.:.::
        // Reko: a decoder for V850 instruction 8B50 at address 00101BDC has not been implemented.
        [Test]
        public void V850Dis_8B50()
        {
            AssertCode("@@@", "8B50");
        }

        // .................:.:.101101.:..:
        // Reko: a decoder for V850 instruction A955 at address 00101BDE has not been implemented.
        [Test]
        public void V850Dis_A955()
        {
            AssertCode("@@@", "A955");
        }

        // .....................001110.....
        // Reko: a decoder for V850 instruction C001 at address 00101BE0 has not been implemented.
        [Test]
        public void V850Dis_C001()
        {
            AssertCode("@@@", "C001");
        }

        // ................:::..100100.:.:.
        // ................:::..:..:...:.:. sst_h
        // ..................:.:111000.::.:
        // Reko: a decoder for V850 instruction 0D2F at address 00101BE4 has not been implemented.
        [Test]
        public void V850Dis_0D2F()
        {
            AssertCode("@@@", "0D2F");
        }

        // ....................:011011:.:.:
        // ....................:.::.:::.:.: sld_b
        // ................:::.:000000..::.
        // ................11101........::.
        // ................:::.:........::. mov
        // .................::::101110.:...
        // Reko: a decoder for V850 instruction C87D at address 00101BEA has not been implemented.
        [Test]
        public void V850Dis_C87D()
        {
            AssertCode("@@@", "C87D");
        }

        // ................::.::110010..::.
        // Reko: a decoder for V850 instruction 46DE at address 00101BEC has not been implemented.
        [Test]
        public void V850Dis_46DE()
        {
            AssertCode("@@@", "46DE");
        }

        // .................::.:010110:::.:
        // Reko: a decoder for V850 instruction DD6A at address 00101BEE has not been implemented.
        [Test]
        public void V850Dis_DD6A()
        {
            AssertCode("@@@", "DD6A");
        }

        // ................:.:.:110111.:.::
        // Reko: a decoder for V850 instruction EBAE at address 00101BF0 has not been implemented.
        [Test]
        public void V850Dis_EBAE()
        {
            AssertCode("@@@", "EBAE");
        }

        // ...................::010100.:.::
        // Reko: a decoder for V850 instruction 8B1A at address 00101BF2 has not been implemented.
        [Test]
        public void V850Dis_8B1A()
        {
            AssertCode("@@@", "8B1A");
        }

        // ................::::.011100:::..
        // ................::::..:::..:::.. sst_b
        // ................:::::010110.:..:
        // Reko: a decoder for V850 instruction C9FA at address 00101BF6 has not been implemented.
        [Test]
        public void V850Dis_C9FA()
        {
            AssertCode("@@@", "C9FA");
        }

        // ................::.:.011000.::::
        // ................::.:..::....:::: sld_b
        // .................:::.100000.:...
        // .................:::.:......:... sld_h
        // .................:.:.001110..:::
        // Reko: a decoder for V850 instruction C751 at address 00101BFC has not been implemented.
        [Test]
        public void V850Dis_C751()
        {
            AssertCode("@@@", "C751");
        }

        // .................::::101101:..::
        // Reko: a decoder for V850 instruction B37D at address 00101BFE has not been implemented.
        [Test]
        public void V850Dis_B37D()
        {
            AssertCode("@@@", "B37D");
        }

        // .................::..111111..:::
        // Reko: a decoder for V850 instruction E767 at address 00101C00 has not been implemented.
        [Test]
        public void V850Dis_E767()
        {
            AssertCode("@@@", "E767");
        }

        // .................::.:001100:::::
        // Reko: a decoder for V850 instruction 9F69 at address 00101C02 has not been implemented.
        [Test]
        public void V850Dis_9F69()
        {
            AssertCode("@@@", "9F69");
        }

        // ................:::..001100.::.:
        // ....................:011110:.:::
        // ....................:.::::.:.::: sst_b
        // ..................:::010111..:::
        // Reko: a decoder for V850 instruction E73A at address 00101C08 has not been implemented.
        [Test]
        public void V850Dis_E73A()
        {
            AssertCode("@@@", "E73A");
        }

        // ................::..:111101.:...
        // Reko: a decoder for V850 instruction A8CF at address 00101C0A has not been implemented.
        [Test]
        public void V850Dis_A8CF()
        {
            AssertCode("@@@", "A8CF");
        }

        // .................::::010011...::
        // Reko: a decoder for V850 instruction 637A at address 00101C0C has not been implemented.
        [Test]
        public void V850Dis_637A()
        {
            AssertCode("@@@", "637A");
        }

        // ..................::.001100....:
        // Reko: a decoder for V850 instruction 8131 at address 00101C0E has not been implemented.
        [Test]
        public void V850Dis_8131()
        {
            AssertCode("@@@", "8131");
        }

        // .................::..011110:.::.
        // .................::...::::.:.::. sst_b
        // ................::.::100010.:..:
        // ................::.:::...:..:..: sld_h
        // ................:...:111110..:..
        // Reko: a decoder for V850 instruction C48F at address 00101C14 has not been implemented.
        [Test]
        public void V850Dis_C48F()
        {
            AssertCode("@@@", "C48F");
        }

        // ................::..:100101::...
        // ................::..::..:.:::... sst_h
        // ................:...:111001::...
        // Reko: a decoder for V850 instruction 388F at address 00101C18 has not been implemented.
        [Test]
        public void V850Dis_388F()
        {
            AssertCode("@@@", "388F");
        }

        // ..................:::001110:.:.:
        // .....................000001..:::
        // ..........................:..::: not
        // ................:.::.100010...::
        // ................:.::.:...:....:: sld_h
        // .................:..:011001:.:.:
        // .................:..:.::..::.:.: sld_b
        // ................::.::110100:.::.
        // Reko: a decoder for V850 instruction 96DE at address 00101C22 has not been implemented.
        [Test]
        public void V850Dis_96DE()
        {
            AssertCode("@@@", "96DE");
        }

        // .................:::.100001::::.
        // .................:::.:....:::::. sld_h
        // ....................:001110:::.:
        // Reko: a decoder for V850 instruction DD09 at address 00101C26 has not been implemented.
        [Test]
        public void V850Dis_DD09()
        {
            AssertCode("@@@", "DD09");
        }

        // ................:....000110:.::.
        // Reko: a decoder for V850 instruction D680 at address 00101C28 has not been implemented.
        [Test]
        public void V850Dis_D680()
        {
            AssertCode("@@@", "D680");
        }

        // .................::..010001:..:.
        // Reko: a decoder for V850 instruction 3262 at address 00101C2A has not been implemented.
        [Test]
        public void V850Dis_3262()
        {
            AssertCode("@@@", "3262");
        }

        // ................:::::001010.:.::
        // Reko: a decoder for V850 instruction 4BF9 at address 00101C2C has not been implemented.
        [Test]
        public void V850Dis_4BF9()
        {
            AssertCode("@@@", "4BF9");
        }

        // ................:..:.100100:.::.
        // ................:..:.:..:..:.::. sst_h
        // ................::.:.111010:..:.
        // Reko: a decoder for V850 instruction 52D7 at address 00101C30 has not been implemented.
        [Test]
        public void V850Dis_52D7()
        {
            AssertCode("@@@", "52D7");
        }

        // .................::..100100::::.
        // .................::..:..:..::::. sst_h
        // ................::.:.110000.:...
        // Reko: a decoder for V850 instruction 08D6 at address 00101C34 has not been implemented.
        [Test]
        public void V850Dis_08D6()
        {
            AssertCode("@@@", "08D6");
        }

        // .................:.::000101::...
        // Reko: a decoder for V850 instruction B858 at address 00101C36 has not been implemented.
        [Test]
        public void V850Dis_B858()
        {
            AssertCode("@@@", "B858");
        }

        // ................::.:.011101.::::
        // ................::.:..:::.:.:::: sst_b
        // ................:::::101001:....
        // ................:..::110000.::..
        // Reko: a decoder for V850 instruction 0C9E at address 00101C3C has not been implemented.
        [Test]
        public void V850Dis_0C9E()
        {
            AssertCode("@@@", "0C9E");
        }

        // ................:::..111111..:..
        // Reko: a decoder for V850 instruction E4E7 at address 00101C3E has not been implemented.
        [Test]
        public void V850Dis_E4E7()
        {
            AssertCode("@@@", "E4E7");
        }

        // ................::::.100111:.::.
        // ................::::.:..::::.::. sst_h
        // ...................::000100::.:.
        // Reko: a decoder for V850 instruction 9A18 at address 00101C42 has not been implemented.
        [Test]
        public void V850Dis_9A18()
        {
            AssertCode("@@@", "9A18");
        }

        // ................:.:..010111..:..
        // Reko: a decoder for V850 instruction E4A2 at address 00101C44 has not been implemented.
        [Test]
        public void V850Dis_E4A2()
        {
            AssertCode("@@@", "E4A2");
        }

        // .................::..001111:..:.
        // Reko: a decoder for V850 instruction F261 at address 00101C46 has not been implemented.
        [Test]
        public void V850Dis_F261()
        {
            AssertCode("@@@", "F261");
        }

        // ..................:::111100..:.:
        // Reko: a decoder for V850 instruction 853F at address 00101C48 has not been implemented.
        [Test]
        public void V850Dis_853F()
        {
            AssertCode("@@@", "853F");
        }

        // .................:...100000..::.
        // .................:...:.......::. sld_h
        // .................:.:.110010:::..
        // Reko: a decoder for V850 instruction 5C56 at address 00101C4C has not been implemented.
        [Test]
        public void V850Dis_5C56()
        {
            AssertCode("@@@", "5C56");
        }

        // ..................:::111100.....
        // Reko: a decoder for V850 instruction 803F at address 00101C4E has not been implemented.
        [Test]
        public void V850Dis_803F()
        {
            AssertCode("@@@", "803F");
        }

        // .................:...001111::...
        // Reko: a decoder for V850 instruction F841 at address 00101C50 has not been implemented.
        [Test]
        public void V850Dis_F841()
        {
            AssertCode("@@@", "F841");
        }

        // .................:..:111111..:::
        // Reko: a decoder for V850 instruction E74F at address 00101C52 has not been implemented.
        [Test]
        public void V850Dis_E74F()
        {
            AssertCode("@@@", "E74F");
        }

        // ................::.::111011....:
        // Reko: a decoder for V850 instruction 61DF at address 00101C54 has not been implemented.
        [Test]
        public void V850Dis_61DF()
        {
            AssertCode("@@@", "61DF");
        }

        // .................:.::111101.:.::
        // Reko: a decoder for V850 instruction AB5F at address 00101C56 has not been implemented.
        [Test]
        public void V850Dis_AB5F()
        {
            AssertCode("@@@", "AB5F");
        }

        // ..................:::110100:..:.
        // Reko: a decoder for V850 instruction 923E at address 00101C58 has not been implemented.
        [Test]
        public void V850Dis_923E()
        {
            AssertCode("@@@", "923E");
        }

        // .................::.:011001.::::
        // .................::.:.::..:.:::: sld_b
        // ................:::..000010...::
        // ................11100....:....::
        // ................:::......:.00011
        // ................:::......:....:: divh
        // ..................:::000111:::.:
        // Reko: a decoder for V850 instruction FD38 at address 00101C5E has not been implemented.
        [Test]
        public void V850Dis_FD38()
        {
            AssertCode("@@@", "FD38");
        }

        // .................:..:010100:::..
        // Reko: a decoder for V850 instruction 9C4A at address 00101C60 has not been implemented.
        [Test]
        public void V850Dis_9C4A()
        {
            AssertCode("@@@", "9C4A");
        }

        // .................:.:.111011.::..
        // .................::::110010:::::
        // Reko: a decoder for V850 instruction 5F7E at address 00101C64 has not been implemented.
        [Test]
        public void V850Dis_5F7E()
        {
            AssertCode("@@@", "5F7E");
        }

        // ................:.:.:111101.::::
        // Reko: a decoder for V850 instruction AFAF at address 00101C66 has not been implemented.
        [Test]
        public void V850Dis_AFAF()
        {
            AssertCode("@@@", "AFAF");
        }

        // ..................:::100111..:..
        // ..................::::..:::..:.. sst_h
        // ................::..:000110::::.
        // Reko: a decoder for V850 instruction DEC8 at address 00101C6A has not been implemented.
        [Test]
        public void V850Dis_DEC8()
        {
            AssertCode("@@@", "DEC8");
        }

        // .................:::.100110:....
        // .................:::.:..::.:.... sst_h
        // .................:.::000111::.::
        // Reko: a decoder for V850 instruction FB58 at address 00101C6E has not been implemented.
        [Test]
        public void V850Dis_FB58()
        {
            AssertCode("@@@", "FB58");
        }

        // ................::::.000010:..:.
        // ................11110....:.:..:.
        // ................::::.....:.10010
        // ................::::.....:.:..:. divh
        // ................::..:011000..:..
        // ................::..:.::.....:.. sld_b
        // ................:..:.100101:.::.
        // ................:..:.:..:.::.::. sst_h
        // ...................:.110100..:.:
        // Reko: a decoder for V850 instruction 8516 at address 00101C76 has not been implemented.
        [Test]
        public void V850Dis_8516()
        {
            AssertCode("@@@", "8516");
        }

        // ..................:::010010.:...
        // Reko: a decoder for V850 instruction 483A at address 00101C78 has not been implemented.
        [Test]
        public void V850Dis_483A()
        {
            AssertCode("@@@", "483A");
        }

        // ................:.:..110100..::.
        // Reko: a decoder for V850 instruction 86A6 at address 00101C7A has not been implemented.
        [Test]
        public void V850Dis_86A6()
        {
            AssertCode("@@@", "86A6");
        }

        // ................:....110011:..:.
        // Reko: a decoder for V850 instruction 7286 at address 00101C7C has not been implemented.
        [Test]
        public void V850Dis_7286()
        {
            AssertCode("@@@", "7286");
        }

        // ................:.:..101000::::.
        // ................:.:..:.:...::::0 Sld.w/Sst.w
        // ................:.:..:.:...::::. sld_w
        // ................::..:101010...:.
        // Reko: a decoder for V850 instruction 42CD at address 00101C80 has not been implemented.
        [Test]
        public void V850Dis_42CD()
        {
            AssertCode("@@@", "42CD");
        }

        // ...................:.001101.:...
        // Reko: a decoder for V850 instruction A811 at address 00101C82 has not been implemented.
        [Test]
        public void V850Dis_A811()
        {
            AssertCode("@@@", "A811");
        }

        // ..................:.:001111.:.:.
        // .................::::010111.....
        // Reko: a decoder for V850 instruction E07A at address 00101C86 has not been implemented.
        [Test]
        public void V850Dis_E07A()
        {
            AssertCode("@@@", "E07A");
        }

        // ..................::.111100:...:
        // Reko: a decoder for V850 instruction 9137 at address 00101C88 has not been implemented.
        [Test]
        public void V850Dis_9137()
        {
            AssertCode("@@@", "9137");
        }

        // ..................:..101011.:.:.
        // Reko: a decoder for V850 instruction 6A25 at address 00101C8A has not been implemented.
        [Test]
        public void V850Dis_6A25()
        {
            AssertCode("@@@", "6A25");
        }

        // ................:::..011000.....
        // ................:::...::........ sld_b
        // ...................::101100.::::
        // ................::.:.110010...:.
        // .................:.:.010000::..:
        // .................:.:..:....11001
        // .................:.:..:....::..: mov
        // ..................::.010110::.::
        // Reko: a decoder for V850 instruction DB32 at address 00101C94 has not been implemented.
        [Test]
        public void V850Dis_DB32()
        {
            AssertCode("@@@", "DB32");
        }

        // .....................111100....:
        // Reko: a decoder for V850 instruction 8107 at address 00101C96 has not been implemented.
        [Test]
        public void V850Dis_8107()
        {
            AssertCode("@@@", "8107");
        }

        // .................:.:.101110..:.:
        // Reko: a decoder for V850 instruction C555 at address 00101C98 has not been implemented.
        [Test]
        public void V850Dis_C555()
        {
            AssertCode("@@@", "C555");
        }

        // ..................:..100000.::::
        // ..................:..:......:::: sld_h
        // ....................:001111:.:::
        // Reko: a decoder for V850 instruction F709 at address 00101C9C has not been implemented.
        [Test]
        public void V850Dis_F709()
        {
            AssertCode("@@@", "F709");
        }

        // .................:.::110001:::.:
        // Reko: a decoder for V850 instruction 3D5E at address 00101C9E has not been implemented.
        [Test]
        public void V850Dis_3D5E()
        {
            AssertCode("@@@", "3D5E");
        }

        // ..................:::001000::::.
        // ..................:::..:...::::. or
        // .....................010100..::.
        // Reko: a decoder for V850 instruction 8602 at address 00101CA2 has not been implemented.
        [Test]
        public void V850Dis_8602()
        {
            AssertCode("@@@", "8602");
        }

        // ................:::.:001110.::..
        // ..................:.:000011.:::.
        // Reko: a decoder for V850 instruction 6E28 at address 00101CA6 has not been implemented.
        [Test]
        public void V850Dis_6E28()
        {
            AssertCode("@@@", "6E28");
        }

        // ................:...:010000:....
        // ................:...:.:....10000
        // ................:...:.:....:.... mov
        // .................::::111001::..:
        // Reko: a decoder for V850 instruction 397F at address 00101CAA has not been implemented.
        [Test]
        public void V850Dis_397F()
        {
            AssertCode("@@@", "397F");
        }

        // .................::..000001.:..:
        // .................::.......:.:..: not
        // .................::..101000:.:.:
        // .................::..:.:...:.:.1 Sld.w/Sst.w
        // .................::..:.:...:.:.: sst_w
        // .................::.:000101.:.::
        // Reko: a decoder for V850 instruction AB68 at address 00101CB0 has not been implemented.
        [Test]
        public void V850Dis_AB68()
        {
            AssertCode("@@@", "AB68");
        }

        // ................:..::100011...::
        // ................:..:::...::...:: sld_h
        // ................::..:011011.::..
        // ................::..:.::.::.::.. sld_b
        // ...................:.000110.:..:
        // Reko: a decoder for V850 instruction C910 at address 00101CB6 has not been implemented.
        [Test]
        public void V850Dis_C910()
        {
            AssertCode("@@@", "C910");
        }

        // ................:.:..000010.::..
        // ................10100....:..::..
        // ................:.:......:.01100
        // ................:.:......:..::.. divh
        // ................:..:.100010.::..
        // ................:..:.:...:..::.. sld_h
        // ................:.:::100000::.:.
        // ................:.::::.....::.:. sld_h
        // ................:...:000100...:.
        // Reko: a decoder for V850 instruction 8288 at address 00101CBE has not been implemented.
        [Test]
        public void V850Dis_8288()
        {
            AssertCode("@@@", "8288");
        }

        // ....................:100111::.:.
        // ....................::..:::::.:. sst_h
        // ................::...111000..::.
        // Reko: a decoder for V850 instruction 06C7 at address 00101CC2 has not been implemented.
        [Test]
        public void V850Dis_06C7()
        {
            AssertCode("@@@", "06C7");
        }

        // ................:...:100010:::.:
        // ................:...::...:.:::.: sld_h
        // .................:.::001010:...:
        // Reko: a decoder for V850 instruction 5159 at address 00101CC6 has not been implemented.
        [Test]
        public void V850Dis_5159()
        {
            AssertCode("@@@", "5159");
        }

        // ................::::.000011..:::
        // .................:.:.001010...:.
        // Reko: a decoder for V850 instruction 4251 at address 00101CCA has not been implemented.
        [Test]
        public void V850Dis_4251()
        {
            AssertCode("@@@", "4251");
        }

        // .....................001111::...
        // Reko: a decoder for V850 instruction F801 at address 00101CCC has not been implemented.
        [Test]
        public void V850Dis_F801()
        {
            AssertCode("@@@", "F801");
        }

        // .................::..101110:::.:
        // Reko: a decoder for V850 instruction DD65 at address 00101CCE has not been implemented.
        [Test]
        public void V850Dis_DD65()
        {
            AssertCode("@@@", "DD65");
        }

        // ................:.:.:010010.....
        // Reko: a decoder for V850 instruction 40AA at address 00101CD0 has not been implemented.
        [Test]
        public void V850Dis_40AA()
        {
            AssertCode("@@@", "40AA");
        }

        // ................:...:110011::..:
        // Reko: a decoder for V850 instruction 798E at address 00101CD2 has not been implemented.
        [Test]
        public void V850Dis_798E()
        {
            AssertCode("@@@", "798E");
        }

        // ................:.:..000101..:.:
        // Reko: a decoder for V850 instruction A5A0 at address 00101CD4 has not been implemented.
        [Test]
        public void V850Dis_A5A0()
        {
            AssertCode("@@@", "A5A0");
        }

        // .................:.:.011111:...:
        // .................:.:..::::::...: sst_b
        // .................:::.101011:....
        // Reko: a decoder for V850 instruction 7075 at address 00101CD8 has not been implemented.
        [Test]
        public void V850Dis_7075()
        {
            AssertCode("@@@", "7075");
        }

        // ..................:.:011101:.::.
        // ..................:.:.:::.::.::. sst_b
        // ................::.::010100:::..
        // .................:.:.100000.:::.
        // .................:.:.:......:::. sld_h
        // ................:.::.001000..:::
        // ................:.::...:.....::: or
        // ...................::101100..:.:
        // Reko: a decoder for V850 instruction 851D at address 00101CE2 has not been implemented.
        [Test]
        public void V850Dis_851D()
        {
            AssertCode("@@@", "851D");
        }

        // .................:..:110010::..:
        // Reko: a decoder for V850 instruction 594E at address 00101CE4 has not been implemented.
        [Test]
        public void V850Dis_594E()
        {
            AssertCode("@@@", "594E");
        }

        // ...................::110000.:...
        // Reko: a decoder for V850 instruction 081E at address 00101CE6 has not been implemented.
        [Test]
        public void V850Dis_081E()
        {
            AssertCode("@@@", "081E");
        }

        // ....................:010000:.:::
        // ....................:.:....10111
        // ....................:.:....:.::: mov
        // .................::::011001:.:..
        // .................::::.::..::.:.. sld_b
        // .................::::000100.:...
        // Reko: a decoder for V850 instruction 8878 at address 00101CEC has not been implemented.
        [Test]
        public void V850Dis_8878()
        {
            AssertCode("@@@", "8878");
        }

        // .................:..:010000::...
        // .................:..:.:....11000
        // .................:..:.:....::... mov
        // ................:::..110110...::
        // Reko: a decoder for V850 instruction C3E6 at address 00101CF0 has not been implemented.
        [Test]
        public void V850Dis_C3E6()
        {
            AssertCode("@@@", "C3E6");
        }

        // .................::.:111010:..::
        // Reko: a decoder for V850 instruction 536F at address 00101CF2 has not been implemented.
        [Test]
        public void V850Dis_536F()
        {
            AssertCode("@@@", "536F");
        }

        // ................::::.011010...:.
        // ................::::..::.:....:. sld_b
        // .................:.::001111...:.
        // Reko: a decoder for V850 instruction E259 at address 00101CF6 has not been implemented.
        [Test]
        public void V850Dis_E259()
        {
            AssertCode("@@@", "E259");
        }

        // ................:.::.011000::.:.
        // ................:.::..::...::.:. sld_b
        // ................:::..111010:.:::
        // Reko: a decoder for V850 instruction 57E7 at address 00101CFA has not been implemented.
        [Test]
        public void V850Dis_57E7()
        {
            AssertCode("@@@", "57E7");
        }

        // ..................:..110110:.:.:
        // Reko: a decoder for V850 instruction D526 at address 00101CFC has not been implemented.
        [Test]
        public void V850Dis_D526()
        {
            AssertCode("@@@", "D526");
        }

        // ................:...:101101:::..
        // Reko: a decoder for V850 instruction BC8D at address 00101CFE has not been implemented.
        [Test]
        public void V850Dis_BC8D()
        {
            AssertCode("@@@", "BC8D");
        }

        // ................:::.:110111...:.
        // Reko: a decoder for V850 instruction E2EE at address 00101D00 has not been implemented.
        [Test]
        public void V850Dis_E2EE()
        {
            AssertCode("@@@", "E2EE");
        }

        // .................:...110010.:..:
        // Reko: a decoder for V850 instruction 4946 at address 00101D02 has not been implemented.
        [Test]
        public void V850Dis_4946()
        {
            AssertCode("@@@", "4946");
        }

        // ................::...000011:::..
        // Reko: a decoder for V850 instruction 7CC0 at address 00101D04 has not been implemented.
        [Test]
        public void V850Dis_7CC0()
        {
            AssertCode("@@@", "7CC0");
        }

        // ................::::.010000.::..
        // ................::::..:....01100
        // ................::::..:.....::.. mov
        // .................:.::010101...:.
        // Reko: a decoder for V850 instruction A25A at address 00101D08 has not been implemented.
        [Test]
        public void V850Dis_A25A()
        {
            AssertCode("@@@", "A25A");
        }

        // ................:..::011011:.:::
        // ................:..::.::.:::.::: sld_b
        // .................:::.101011.::..
        // Reko: a decoder for V850 instruction 6C75 at address 00101D0C has not been implemented.
        [Test]
        public void V850Dis_6C75()
        {
            AssertCode("@@@", "6C75");
        }

        // ................:::::000110.:.:.
        // Reko: a decoder for V850 instruction CAF8 at address 00101D0E has not been implemented.
        [Test]
        public void V850Dis_CAF8()
        {
            AssertCode("@@@", "CAF8");
        }

        // .................::.:100010:::.:
        // .................::.::...:.:::.: sld_h
        // ................:.::.101011.:...
        // Reko: a decoder for V850 instruction 68B5 at address 00101D12 has not been implemented.
        [Test]
        public void V850Dis_68B5()
        {
            AssertCode("@@@", "68B5");
        }

        // ................:..::110100..:::
        // Reko: a decoder for V850 instruction 879E at address 00101D14 has not been implemented.
        [Test]
        public void V850Dis_879E()
        {
            AssertCode("@@@", "879E");
        }

        // ...................:.011000:...:
        // ...................:..::...:...: sld_b
        // ..................:.:100110.::::
        // ..................:.::..::..:::: sst_h
        // ................:.:..101000...:.
        // ................:.:..:.:......:0 Sld.w/Sst.w
        // ................:.:..:.:......:. sld_w
        // ..................:..000111:.:::
        // Reko: a decoder for V850 instruction F720 at address 00101D1C has not been implemented.
        [Test]
        public void V850Dis_F720()
        {
            AssertCode("@@@", "F720");
        }

        // ................::::.001010.:...
        // Reko: a decoder for V850 instruction 48F1 at address 00101D1E has not been implemented.
        [Test]
        public void V850Dis_48F1()
        {
            AssertCode("@@@", "48F1");
        }

        // ................:.:..111010.::.:
        // ................:..::001100::..:
        // Reko: a decoder for V850 instruction 9999 at address 00101D22 has not been implemented.
        [Test]
        public void V850Dis_9999()
        {
            AssertCode("@@@", "9999");
        }

        // ................::.:.001001.:::.
        // Reko: a decoder for V850 instruction 2ED1 at address 00101D24 has not been implemented.
        [Test]
        public void V850Dis_2ED1()
        {
            AssertCode("@@@", "2ED1");
        }

        // .....................010110.::.:
        // Reko: a decoder for V850 instruction CD02 at address 00101D26 has not been implemented.
        [Test]
        public void V850Dis_CD02()
        {
            AssertCode("@@@", "CD02");
        }

        // .................::..001110::.:.
        // Reko: a decoder for V850 instruction DA61 at address 00101D28 has not been implemented.
        [Test]
        public void V850Dis_DA61()
        {
            AssertCode("@@@", "DA61");
        }

        // ................:::.:000110.::.:
        // Reko: a decoder for V850 instruction CDE8 at address 00101D2A has not been implemented.
        [Test]
        public void V850Dis_CDE8()
        {
            AssertCode("@@@", "CDE8");
        }

        // .................:...111011:::..
        // Reko: a decoder for V850 instruction 7C47 at address 00101D2C has not been implemented.
        [Test]
        public void V850Dis_7C47()
        {
            AssertCode("@@@", "7C47");
        }

        // ................:.:..101111.::..
        // Reko: a decoder for V850 instruction ECA5 at address 00101D2E has not been implemented.
        [Test]
        public void V850Dis_ECA5()
        {
            AssertCode("@@@", "ECA5");
        }

        // .................:.::111111.:...
        // Reko: a decoder for V850 instruction E85F at address 00101D30 has not been implemented.
        [Test]
        public void V850Dis_E85F()
        {
            AssertCode("@@@", "E85F");
        }

        // .................::.:001100.:..:
        // Reko: a decoder for V850 instruction 8969 at address 00101D32 has not been implemented.
        [Test]
        public void V850Dis_8969()
        {
            AssertCode("@@@", "8969");
        }

        // .....................110101:::.:
        // Reko: a decoder for V850 instruction BD06 at address 00101D34 has not been implemented.
        [Test]
        public void V850Dis_BD06()
        {
            AssertCode("@@@", "BD06");
        }

        // .................:...011000..::.
        // .................:....::.....::. sld_b
        // ..................::.101110:::..
        // Reko: a decoder for V850 instruction DC35 at address 00101D38 has not been implemented.
        [Test]
        public void V850Dis_DC35()
        {
            AssertCode("@@@", "DC35");
        }

        // .................:.:.000011:.:::
        // Reko: a decoder for V850 instruction 7750 at address 00101D3A has not been implemented.
        [Test]
        public void V850Dis_7750()
        {
            AssertCode("@@@", "7750");
        }

        // .................::..110101:::.:
        // Reko: a decoder for V850 instruction BD66 at address 00101D3C has not been implemented.
        [Test]
        public void V850Dis_BD66()
        {
            AssertCode("@@@", "BD66");
        }

        // ................:....001111::.::
        // Reko: a decoder for V850 instruction FB81 at address 00101D3E has not been implemented.
        [Test]
        public void V850Dis_FB81()
        {
            AssertCode("@@@", "FB81");
        }

        // ................:....110000:...:
        // Reko: a decoder for V850 instruction 1186 at address 00101D40 has not been implemented.
        [Test]
        public void V850Dis_1186()
        {
            AssertCode("@@@", "1186");
        }

        // ................::...100010.::.:
        // ................::...:...:..::.: sld_h
        // ..................::.001110::.::
        // Reko: a decoder for V850 instruction DB31 at address 00101D44 has not been implemented.
        [Test]
        public void V850Dis_DB31()
        {
            AssertCode("@@@", "DB31");
        }

        // ................:...:100110.:..:
        // ................:...::..::..:..: sst_h
        // .................::..101100::::.
        // Reko: a decoder for V850 instruction 9E65 at address 00101D48 has not been implemented.
        [Test]
        public void V850Dis_9E65()
        {
            AssertCode("@@@", "9E65");
        }

        // ..................:..101110::.::
        // Reko: a decoder for V850 instruction DB25 at address 00101D4A has not been implemented.
        [Test]
        public void V850Dis_DB25()
        {
            AssertCode("@@@", "DB25");
        }

        // ..................:.:001110:....
        // Reko: a decoder for V850 instruction D029 at address 00101D4C has not been implemented.
        [Test]
        public void V850Dis_D029()
        {
            AssertCode("@@@", "D029");
        }

        // ................:...:000100:.:::
        // .................:.::101001:::::
        // Reko: a decoder for V850 instruction 3F5D at address 00101D50 has not been implemented.
        [Test]
        public void V850Dis_3F5D()
        {
            AssertCode("@@@", "3F5D");
        }

        // ................:..::000101:::.:
        // Reko: a decoder for V850 instruction BD98 at address 00101D52 has not been implemented.
        [Test]
        public void V850Dis_BD98()
        {
            AssertCode("@@@", "BD98");
        }

        // ................::.::111100:.::.
        // Reko: a decoder for V850 instruction 96DF at address 00101D54 has not been implemented.
        [Test]
        public void V850Dis_96DF()
        {
            AssertCode("@@@", "96DF");
        }

        // .................:...110001:..::
        // Reko: a decoder for V850 instruction 3346 at address 00101D56 has not been implemented.
        [Test]
        public void V850Dis_3346()
        {
            AssertCode("@@@", "3346");
        }

        // .................:.:.110101....:
        // Reko: a decoder for V850 instruction A156 at address 00101D58 has not been implemented.
        [Test]
        public void V850Dis_A156()
        {
            AssertCode("@@@", "A156");
        }

        // ................:::::000101:::..
        // Reko: a decoder for V850 instruction BCF8 at address 00101D5A has not been implemented.
        [Test]
        public void V850Dis_BCF8()
        {
            AssertCode("@@@", "BCF8");
        }

        // .................::::101010::..:
        // Reko: a decoder for V850 instruction 597D at address 00101D5C has not been implemented.
        [Test]
        public void V850Dis_597D()
        {
            AssertCode("@@@", "597D");
        }

        // .................:::.100010..:..
        // .................:::.:...:...:.. sld_h
        // .................:..:100011..:.:
        // .................:..::...::..:.: sld_h
        // ................::::.101110:.::.
        // Reko: a decoder for V850 instruction D6F5 at address 00101D62 has not been implemented.
        [Test]
        public void V850Dis_D6F5()
        {
            AssertCode("@@@", "D6F5");
        }

        // ..................:.:000111::..:
        // Reko: a decoder for V850 instruction F928 at address 00101D64 has not been implemented.
        [Test]
        public void V850Dis_F928()
        {
            AssertCode("@@@", "F928");
        }

        // ................:.::.111110.:..:
        // Reko: a decoder for V850 instruction C9B7 at address 00101D66 has not been implemented.
        [Test]
        public void V850Dis_C9B7()
        {
            AssertCode("@@@", "C9B7");
        }

        // ................:::::101100:.:.:
        // Reko: a decoder for V850 instruction 95FD at address 00101D68 has not been implemented.
        [Test]
        public void V850Dis_95FD()
        {
            AssertCode("@@@", "95FD");
        }

        // .................:.::000110.:.:.
        // Reko: a decoder for V850 instruction CA58 at address 00101D6A has not been implemented.
        [Test]
        public void V850Dis_CA58()
        {
            AssertCode("@@@", "CA58");
        }

        // ................:::..000001.:.::
        // ................:::.......:.:.:: not
        // ................:.::.011011:..:.
        // ................:.::..::.:::..:. sld_b
        // ................:::.:111100:::.:
        // Reko: a decoder for V850 instruction 9DEF at address 00101D70 has not been implemented.
        [Test]
        public void V850Dis_9DEF()
        {
            AssertCode("@@@", "9DEF");
        }

        // .................::..000000:....
        // ................01100......:....
        // .................::........:.... mov
        // .................::..011101.:..:
        // .................::...:::.:.:..: sst_b
        // ..................::.010111.:.:.
        // Reko: a decoder for V850 instruction EA32 at address 00101D76 has not been implemented.
        [Test]
        public void V850Dis_EA32()
        {
            AssertCode("@@@", "EA32");
        }

        // .....................111010:..::
        // Reko: a decoder for V850 instruction 5307 at address 00101D78 has not been implemented.
        [Test]
        public void V850Dis_5307()
        {
            AssertCode("@@@", "5307");
        }

        // ................:....101001...::
        // Reko: a decoder for V850 instruction 2385 at address 00101D7A has not been implemented.
        [Test]
        public void V850Dis_2385()
        {
            AssertCode("@@@", "2385");
        }

        // ................::.:.000011:.:..
        // Reko: a decoder for V850 instruction 74D0 at address 00101D7C has not been implemented.
        [Test]
        public void V850Dis_74D0()
        {
            AssertCode("@@@", "74D0");
        }

        // ..................:..001001:..:.
        // Reko: a decoder for V850 instruction 3221 at address 00101D7E has not been implemented.
        [Test]
        public void V850Dis_3221()
        {
            AssertCode("@@@", "3221");
        }

        // .................::..111001.....
        // Reko: a decoder for V850 instruction 2067 at address 00101D80 has not been implemented.
        [Test]
        public void V850Dis_2067()
        {
            AssertCode("@@@", "2067");
        }

        // ................:....100110.....
        // ................:....:..::...... sst_h
        // ................:.:..010010:..:.
        // Reko: a decoder for V850 instruction 52A2 at address 00101D84 has not been implemented.
        [Test]
        public void V850Dis_52A2()
        {
            AssertCode("@@@", "52A2");
        }

        // ...................:.111100.:::.
        // Reko: a decoder for V850 instruction 8E17 at address 00101D86 has not been implemented.
        [Test]
        public void V850Dis_8E17()
        {
            AssertCode("@@@", "8E17");
        }

        // .....................101100:.:..
        // Reko: a decoder for V850 instruction 9405 at address 00101D88 has not been implemented.
        [Test]
        public void V850Dis_9405()
        {
            AssertCode("@@@", "9405");
        }

        // ................:..::001000...:.
        // ................:..::..:......:. or
        // ................::.::010000:.:.:
        // ................::.::.:....10101
        // ................::.::.:....:.:.: mov
        // ................::...001000::.::
        // ................::.....:...::.:: or
        // ....................:001111..:..
        // Reko: a decoder for V850 instruction E409 at address 00101D90 has not been implemented.
        [Test]
        public void V850Dis_E409()
        {
            AssertCode("@@@", "E409");
        }

        // .................::.:010010::.::
        // Reko: a decoder for V850 instruction 5B6A at address 00101D92 has not been implemented.
        [Test]
        public void V850Dis_5B6A()
        {
            AssertCode("@@@", "5B6A");
        }

        // ................:::..100011.::..
        // ................:::..:...::.::.. sld_h
        // .....................010110.:.:.
        // Reko: a decoder for V850 instruction CA02 at address 00101D96 has not been implemented.
        [Test]
        public void V850Dis_CA02()
        {
            AssertCode("@@@", "CA02");
        }

        // ................:.:..100011...:.
        // ................:.:..:...::...:. sld_h
        // ................::::.010101....:
        // Reko: a decoder for V850 instruction A1F2 at address 00101D9A has not been implemented.
        [Test]
        public void V850Dis_A1F2()
        {
            AssertCode("@@@", "A1F2");
        }

        // .................:...011000..:..
        // .................:....::.....:.. sld_b
        // ................::...100010...::
        // ................::...:...:....:: sld_h
        // ................:.:.:001110.:.:.
        // Reko: a decoder for V850 instruction CAA9 at address 00101DA0 has not been implemented.
        [Test]
        public void V850Dis_CAA9()
        {
            AssertCode("@@@", "CAA9");
        }

        // ................::...100001:.:::
        // ................::...:....::.::: sld_h
        // ................:::.:110001.....
        // Reko: a decoder for V850 instruction 20EE at address 00101DA4 has not been implemented.
        [Test]
        public void V850Dis_20EE()
        {
            AssertCode("@@@", "20EE");
        }

        // ................:::::011011....:
        // ................:::::.::.::....: sld_b
        // ................:::::001011.....
        // Reko: a decoder for V850 instruction 60F9 at address 00101DA8 has not been implemented.
        [Test]
        public void V850Dis_60F9()
        {
            AssertCode("@@@", "60F9");
        }

        // ................:.:..011110.:.::
        // ................:.:...::::..:.:: sst_b
        // .................:.:.000011.....
        // Reko: a decoder for V850 instruction 6050 at address 00101DAC has not been implemented.
        [Test]
        public void V850Dis_6050()
        {
            AssertCode("@@@", "6050");
        }

        // .................::::000000:::..
        // ................01111......:::..
        // .................::::......:::.. mov
        // .................::..001111.:.::
        // Reko: a decoder for V850 instruction EB61 at address 00101DB0 has not been implemented.
        [Test]
        public void V850Dis_EB61()
        {
            AssertCode("@@@", "EB61");
        }

        // .................::.:010101....:
        // Reko: a decoder for V850 instruction A16A at address 00101DB2 has not been implemented.
        [Test]
        public void V850Dis_A16A()
        {
            AssertCode("@@@", "A16A");
        }

        // ................::..:000011:.:.:
        // Reko: a decoder for V850 instruction 75C8 at address 00101DB4 has not been implemented.
        [Test]
        public void V850Dis_75C8()
        {
            AssertCode("@@@", "75C8");
        }

        // ..................:::100101:.:..
        // ..................::::..:.::.:.. sst_h
        // ................:.:::101100:::..
        // Reko: a decoder for V850 instruction 9CBD at address 00101DB8 has not been implemented.
        [Test]
        public void V850Dis_9CBD()
        {
            AssertCode("@@@", "9CBD");
        }

        // ................:.:.:110001...:.
        // Reko: a decoder for V850 instruction 22AE at address 00101DBA has not been implemented.
        [Test]
        public void V850Dis_22AE()
        {
            AssertCode("@@@", "22AE");
        }

        // ................:..::100000.....
        // ................:..:::.......... sld_h
        // .................:.:.000000:..:.
        // ................01010......:..:.
        // .................:.:.......:..:. mov
        // ..................:::101110...:.
        // Reko: a decoder for V850 instruction C23D at address 00101DC0 has not been implemented.
        [Test]
        public void V850Dis_C23D()
        {
            AssertCode("@@@", "C23D");
        }

        // .................::.:011110.:::.
        // .................::.:.::::..:::. sst_b
        // .................:.:.110101...::
        // Reko: a decoder for V850 instruction A356 at address 00101DC4 has not been implemented.
        [Test]
        public void V850Dis_A356()
        {
            AssertCode("@@@", "A356");
        }

        // ................::::.101101....:
        // Reko: a decoder for V850 instruction A1F5 at address 00101DC6 has not been implemented.
        [Test]
        public void V850Dis_A1F5()
        {
            AssertCode("@@@", "A1F5");
        }

        // .................:.::111101.:...
        // Reko: a decoder for V850 instruction A85F at address 00101DC8 has not been implemented.
        [Test]
        public void V850Dis_A85F()
        {
            AssertCode("@@@", "A85F");
        }

        // .................:::.011000.:..:
        // .................:::..::....:..: sld_b
        // ..................:..110000::..:
        // Reko: a decoder for V850 instruction 1926 at address 00101DCC has not been implemented.
        [Test]
        public void V850Dis_1926()
        {
            AssertCode("@@@", "1926");
        }

        // ...................:.001101....:
        // Reko: a decoder for V850 instruction A111 at address 00101DCE has not been implemented.
        [Test]
        public void V850Dis_A111()
        {
            AssertCode("@@@", "A111");
        }

        // ..................:.:001100..:::
        // ................:.:..010000..::.
        // ................:.:...:....00110
        // ................:.:...:......::. mov
        // ................::.::000101...:.
        // Reko: a decoder for V850 instruction A2D8 at address 00101DD4 has not been implemented.
        [Test]
        public void V850Dis_A2D8()
        {
            AssertCode("@@@", "A2D8");
        }

        // .....................010010.::::
        // Reko: a decoder for V850 instruction 4F02 at address 00101DD6 has not been implemented.
        [Test]
        public void V850Dis_4F02()
        {
            AssertCode("@@@", "4F02");
        }

        // ................:::::011111:.:.:
        // ................:::::.::::::.:.: sst_b
        // .................::..010001..:::
        // Reko: a decoder for V850 instruction 2762 at address 00101DDA has not been implemented.
        [Test]
        public void V850Dis_2762()
        {
            AssertCode("@@@", "2762");
        }

        // ................:.::.011110.:.::
        // ................:.::..::::..:.:: sst_b
        // ................::...100111::.:.
        // ................::...:..:::::.:. sst_h
        // .................::::011111..::.
        // .................::::.:::::..::. sst_b
        // ..................:..011111....:
        // ..................:...:::::....: sst_b
        // ................:.::.000100...:.
        // Reko: a decoder for V850 instruction 82B0 at address 00101DE4 has not been implemented.
        [Test]
        public void V850Dis_82B0()
        {
            AssertCode("@@@", "82B0");
        }

        // ................:..:.101110:.:..
        // Reko: a decoder for V850 instruction D495 at address 00101DE6 has not been implemented.
        [Test]
        public void V850Dis_D495()
        {
            AssertCode("@@@", "D495");
        }

        // ................:.:.:011110.....
        // ................:.:.:.::::...... sst_b
        // ...................:.101110...:.
        // Reko: a decoder for V850 instruction C215 at address 00101DEA has not been implemented.
        [Test]
        public void V850Dis_C215()
        {
            AssertCode("@@@", "C215");
        }

        // .................:...000101::::.
        // .................:::.101001...::
        // Reko: a decoder for V850 instruction 2375 at address 00101DEE has not been implemented.
        [Test]
        public void V850Dis_2375()
        {
            AssertCode("@@@", "2375");
        }

        // ...................:.000000:::..
        // ................00010......:::..
        // ...................:.......:::.. mov
        // ................::.:.111001..::.
        // .................::.:111100:.::.
        // Reko: a decoder for V850 instruction 966F at address 00101DF4 has not been implemented.
        [Test]
        public void V850Dis_966F()
        {
            AssertCode("@@@", "966F");
        }

        // ................:::.:001101.:::.
        // Reko: a decoder for V850 instruction AEE9 at address 00101DF6 has not been implemented.
        [Test]
        public void V850Dis_AEE9()
        {
            AssertCode("@@@", "AEE9");
        }

        // ................::..:100111.:::.
        // ................::..::..:::.:::. sst_h
        // ................:.:.:000011:.::.
        // Reko: a decoder for V850 instruction 76A8 at address 00101DFA has not been implemented.
        [Test]
        public void V850Dis_76A8()
        {
            AssertCode("@@@", "76A8");
        }

        // ..................:::110001.:::.
        // Reko: a decoder for V850 instruction 2E3E at address 00101DFC has not been implemented.
        [Test]
        public void V850Dis_2E3E()
        {
            AssertCode("@@@", "2E3E");
        }

        // ................::::.100001.:.:.
        // ................::::.:....:.:.:. sld_h
        // .................:...100011..:.:
        // .................:...:...::..:.: sld_h
        // ................:..::101100.::..
        // Reko: a decoder for V850 instruction 8C9D at address 00101E02 has not been implemented.
        [Test]
        public void V850Dis_8C9D()
        {
            AssertCode("@@@", "8C9D");
        }

        // .................::::110010..:.:
        // Reko: a decoder for V850 instruction 457E at address 00101E04 has not been implemented.
        [Test]
        public void V850Dis_457E()
        {
            AssertCode("@@@", "457E");
        }

        // ................::.:.110100.::::
        // Reko: a decoder for V850 instruction 8FD6 at address 00101E06 has not been implemented.
        [Test]
        public void V850Dis_8FD6()
        {
            AssertCode("@@@", "8FD6");
        }

        // .................:::.001010:.:..
        // Reko: a decoder for V850 instruction 5471 at address 00101E08 has not been implemented.
        [Test]
        public void V850Dis_5471()
        {
            AssertCode("@@@", "5471");
        }

        // ................::::.110111...:.
        // Reko: a decoder for V850 instruction E2F6 at address 00101E0A has not been implemented.
        [Test]
        public void V850Dis_E2F6()
        {
            AssertCode("@@@", "E2F6");
        }

        // ..................:::001000:.:.:
        // ..................:::..:...:.:.: or
        // ................:.::.100010...::
        // ................:.::.:...:....:: sld_h
        // ................:.::.010110::...
        // Reko: a decoder for V850 instruction D8B2 at address 00101E10 has not been implemented.
        [Test]
        public void V850Dis_D8B2()
        {
            AssertCode("@@@", "D8B2");
        }

        // ................:.:::010010..::.
        // Reko: a decoder for V850 instruction 46BA at address 00101E12 has not been implemented.
        [Test]
        public void V850Dis_46BA()
        {
            AssertCode("@@@", "46BA");
        }

        // ..................:..011100.::..
        // ..................:...:::...::.. sst_b
        // .................:::.111001.:::.
        // Reko: a decoder for V850 instruction 2E77 at address 00101E16 has not been implemented.
        [Test]
        public void V850Dis_2E77()
        {
            AssertCode("@@@", "2E77");
        }

        // ................::::.111110.::..
        // Reko: a decoder for V850 instruction CCF7 at address 00101E18 has not been implemented.
        [Test]
        public void V850Dis_CCF7()
        {
            AssertCode("@@@", "CCF7");
        }

        // ................:.::.110001:.:.:
        // Reko: a decoder for V850 instruction 35B6 at address 00101E1A has not been implemented.
        [Test]
        public void V850Dis_35B6()
        {
            AssertCode("@@@", "35B6");
        }

        // ..................:.:100001::.:.
        // ..................:.::....:::.:. sld_h
        // ................:....010011..::.
        // Reko: a decoder for V850 instruction 6682 at address 00101E1E has not been implemented.
        [Test]
        public void V850Dis_6682()
        {
            AssertCode("@@@", "6682");
        }

        // ................::..:101100....:
        // Reko: a decoder for V850 instruction 81CD at address 00101E20 has not been implemented.
        [Test]
        public void V850Dis_81CD()
        {
            AssertCode("@@@", "81CD");
        }

        // .................::::001101:....
        // Reko: a decoder for V850 instruction B079 at address 00101E22 has not been implemented.
        [Test]
        public void V850Dis_B079()
        {
            AssertCode("@@@", "B079");
        }

        // ................:....101101:..::
        // Reko: a decoder for V850 instruction B385 at address 00101E24 has not been implemented.
        [Test]
        public void V850Dis_B385()
        {
            AssertCode("@@@", "B385");
        }

        // ................:..::110001:....
        // Reko: a decoder for V850 instruction 309E at address 00101E26 has not been implemented.
        [Test]
        public void V850Dis_309E()
        {
            AssertCode("@@@", "309E");
        }

        // .................:..:111100....:
        // Reko: a decoder for V850 instruction 814F at address 00101E28 has not been implemented.
        [Test]
        public void V850Dis_814F()
        {
            AssertCode("@@@", "814F");
        }

        // ..................:..101100...:.
        // Reko: a decoder for V850 instruction 8225 at address 00101E2A has not been implemented.
        [Test]
        public void V850Dis_8225()
        {
            AssertCode("@@@", "8225");
        }

        // ................:.::.111100.::.:
        // Reko: a decoder for V850 instruction 8DB7 at address 00101E2C has not been implemented.
        [Test]
        public void V850Dis_8DB7()
        {
            AssertCode("@@@", "8DB7");
        }

        // ................:.::.100110:..::
        // ................:.::.:..::.:..:: sst_h
        // ................::...010100.:...
        // Reko: a decoder for V850 instruction 88C2 at address 00101E30 has not been implemented.
        [Test]
        public void V850Dis_88C2()
        {
            AssertCode("@@@", "88C2");
        }

        // .................::::100110.::::
        // .................:::::..::..:::: sst_h
        // .................::.:111111::...
        // Reko: a decoder for V850 instruction F86F at address 00101E34 has not been implemented.
        [Test]
        public void V850Dis_F86F()
        {
            AssertCode("@@@", "F86F");
        }

        // .................:...001011..:::
        // Reko: a decoder for V850 instruction 6741 at address 00101E36 has not been implemented.
        [Test]
        public void V850Dis_6741()
        {
            AssertCode("@@@", "6741");
        }

        // ...................:.101000:.::.
        // ...................:.:.:...:.::0 Sld.w/Sst.w
        // ...................:.:.:...:.::. sld_w
        // .................:..:101110:....
        // Reko: a decoder for V850 instruction D04D at address 00101E3A has not been implemented.
        [Test]
        public void V850Dis_D04D()
        {
            AssertCode("@@@", "D04D");
        }

        // ................::.::000010.:...
        // ................11011....:..:...
        // ................::.::....:.01000
        // ................::.::....:..:... divh
        // ..................:..111100:::..
        // Reko: a decoder for V850 instruction 9C27 at address 00101E3E has not been implemented.
        [Test]
        public void V850Dis_9C27()
        {
            AssertCode("@@@", "9C27");
        }

        // .................:.::111000.:.:.
        // Reko: a decoder for V850 instruction 0A5F at address 00101E40 has not been implemented.
        [Test]
        public void V850Dis_0A5F()
        {
            AssertCode("@@@", "0A5F");
        }

        // .................:..:000011:.:.:
        // Reko: a decoder for V850 instruction 7548 at address 00101E42 has not been implemented.
        [Test]
        public void V850Dis_7548()
        {
            AssertCode("@@@", "7548");
        }

        // ................::..:001100.:...
        // Reko: a decoder for V850 instruction 88C9 at address 00101E44 has not been implemented.
        [Test]
        public void V850Dis_88C9()
        {
            AssertCode("@@@", "88C9");
        }

        // .................:.:.011000.....
        // .................:.:..::........ sld_b
        // ................:.:.:000100....:
        // Reko: a decoder for V850 instruction 81A8 at address 00101E48 has not been implemented.
        [Test]
        public void V850Dis_81A8()
        {
            AssertCode("@@@", "81A8");
        }

        // ..................:::101001::..:
        // Reko: a decoder for V850 instruction 393D at address 00101E4A has not been implemented.
        [Test]
        public void V850Dis_393D()
        {
            AssertCode("@@@", "393D");
        }

        // ....................:001101.....
        // ...................:.101111.:..:
        // Reko: a decoder for V850 instruction E915 at address 00101E4E has not been implemented.
        [Test]
        public void V850Dis_E915()
        {
            AssertCode("@@@", "E915");
        }

        // ................:....000001..:..
        // ................:.........:..:.. not
        // .................:::.111001.::.:
        // Reko: a decoder for V850 instruction 2D77 at address 00101E52 has not been implemented.
        [Test]
        public void V850Dis_2D77()
        {
            AssertCode("@@@", "2D77");
        }

        // ................:::.:001010:::.:
        // Reko: a decoder for V850 instruction 5DE9 at address 00101E54 has not been implemented.
        [Test]
        public void V850Dis_5DE9()
        {
            AssertCode("@@@", "5DE9");
        }

        // ...................:.001100.:..:
        // Reko: a decoder for V850 instruction 8911 at address 00101E56 has not been implemented.
        [Test]
        public void V850Dis_8911()
        {
            AssertCode("@@@", "8911");
        }

        // .................::.:111011..:.:
        // Reko: a decoder for V850 instruction 656F at address 00101E58 has not been implemented.
        [Test]
        public void V850Dis_656F()
        {
            AssertCode("@@@", "656F");
        }

        // ................:..::110011...::
        // Reko: a decoder for V850 instruction 639E at address 00101E5A has not been implemented.
        [Test]
        public void V850Dis_639E()
        {
            AssertCode("@@@", "639E");
        }

        // ................:::..010101:.:..
        // Reko: a decoder for V850 instruction B4E2 at address 00101E5C has not been implemented.
        [Test]
        public void V850Dis_B4E2()
        {
            AssertCode("@@@", "B4E2");
        }

        // ................:.:.:000010:.:.:
        // ................10101....:.:.:.:
        // ................:.:.:....:.10101
        // ................:.:.:....:.:.:.: divh
        // .................:.::010101:..:.
        // Reko: a decoder for V850 instruction B25A at address 00101E60 has not been implemented.
        [Test]
        public void V850Dis_B25A()
        {
            AssertCode("@@@", "B25A");
        }

        // ..................:.:101000::.:.
        // ..................:.::.:...::.:0 Sld.w/Sst.w
        // ..................:.::.:...::.:. sld_w
        // ................:...:011010....:
        // ................:...:.::.:.....: sld_b
        // ................:.:::110001.::..
        // Reko: a decoder for V850 instruction 2CBE at address 00101E66 has not been implemented.
        [Test]
        public void V850Dis_2CBE()
        {
            AssertCode("@@@", "2CBE");
        }

        // .................::::100110....:
        // .................:::::..::.....: sst_h
        // .................:.:.000111::.:.
        // Reko: a decoder for V850 instruction FA50 at address 00101E6A has not been implemented.
        [Test]
        public void V850Dis_FA50()
        {
            AssertCode("@@@", "FA50");
        }

        // ..................:.:010001:.::.
        // Reko: a decoder for V850 instruction 362A at address 00101E6C has not been implemented.
        [Test]
        public void V850Dis_362A()
        {
            AssertCode("@@@", "362A");
        }

        // ....................:101101.....
        // Reko: a decoder for V850 instruction A00D at address 00101E6E has not been implemented.
        [Test]
        public void V850Dis_A00D()
        {
            AssertCode("@@@", "A00D");
        }

        // ................:.:::011001.:.::
        // ................:.:::.::..:.:.:: sld_b
        // ................::..:000001.:..:
        // ................::..:.....:.:..: not
        // ..................:..101101.:.::
        // ..................:..000110.:.:.
        // Reko: a decoder for V850 instruction CA20 at address 00101E76 has not been implemented.
        [Test]
        public void V850Dis_CA20()
        {
            AssertCode("@@@", "CA20");
        }

        // .................::..011110:.::.
        // .................::...::::.:.::. sst_b
        // ...................::001111...:.
        // Reko: a decoder for V850 instruction E219 at address 00101E7A has not been implemented.
        [Test]
        public void V850Dis_E219()
        {
            AssertCode("@@@", "E219");
        }

        // .................::..001011..::.
        // Reko: a decoder for V850 instruction 6661 at address 00101E7C has not been implemented.
        [Test]
        public void V850Dis_6661()
        {
            AssertCode("@@@", "6661");
        }

        // ................:..::110100::::.
        // Reko: a decoder for V850 instruction 9E9E at address 00101E7E has not been implemented.
        [Test]
        public void V850Dis_9E9E()
        {
            AssertCode("@@@", "9E9E");
        }

        // ................:..:.000010:..:.
        // ................10010....:.:..:.
        // ................:..:.....:.10010
        // ................:..:.....:.:..:. divh
        // .................:...101100.:.::
        // Reko: a decoder for V850 instruction 8B45 at address 00101E82 has not been implemented.
        [Test]
        public void V850Dis_8B45()
        {
            AssertCode("@@@", "8B45");
        }

        // ................:.:::011111.:::.
        // ................:.:::.:::::.:::. sst_b
        // ................:..::000110:..::
        // Reko: a decoder for V850 instruction D398 at address 00101E86 has not been implemented.
        [Test]
        public void V850Dis_D398()
        {
            AssertCode("@@@", "D398");
        }

        // .................::.:111010:.:..
        // Reko: a decoder for V850 instruction 546F at address 00101E88 has not been implemented.
        [Test]
        public void V850Dis_546F()
        {
            AssertCode("@@@", "546F");
        }

        // ................:::..000101:.:.:
        // Reko: a decoder for V850 instruction B5E0 at address 00101E8A has not been implemented.
        [Test]
        public void V850Dis_B5E0()
        {
            AssertCode("@@@", "B5E0");
        }

        // ..................:::001111.:::.
        // Reko: a decoder for V850 instruction EE39 at address 00101E8C has not been implemented.
        [Test]
        public void V850Dis_EE39()
        {
            AssertCode("@@@", "EE39");
        }

        // ..................:::011110::.::
        // ..................:::.::::.::.:: sst_b
        // .................:.:.101110.:::.
        // Reko: a decoder for V850 instruction CE55 at address 00101E90 has not been implemented.
        [Test]
        public void V850Dis_CE55()
        {
            AssertCode("@@@", "CE55");
        }

        // ................:::::000011:..:.
        // Reko: a decoder for V850 instruction 72F8 at address 00101E92 has not been implemented.
        [Test]
        public void V850Dis_72F8()
        {
            AssertCode("@@@", "72F8");
        }

        // ................:...:100101::..:
        // ................:...::..:.:::..: sst_h
        // ................:...:101111:::.:
        // Reko: a decoder for V850 instruction FD8D at address 00101E96 has not been implemented.
        [Test]
        public void V850Dis_FD8D()
        {
            AssertCode("@@@", "FD8D");
        }

        // ................:::::001100:....
        // Reko: a decoder for V850 instruction 90F9 at address 00101E98 has not been implemented.
        [Test]
        public void V850Dis_90F9()
        {
            AssertCode("@@@", "90F9");
        }

        // ................::.::110010:.:::
        // Reko: a decoder for V850 instruction 57DE at address 00101E9A has not been implemented.
        [Test]
        public void V850Dis_57DE()
        {
            AssertCode("@@@", "57DE");
        }

        // ...................:.010101.:..:
        // Reko: a decoder for V850 instruction A912 at address 00101E9C has not been implemented.
        [Test]
        public void V850Dis_A912()
        {
            AssertCode("@@@", "A912");
        }

        // .................:.::111010:.:::
        // Reko: a decoder for V850 instruction 575F at address 00101E9E has not been implemented.
        [Test]
        public void V850Dis_575F()
        {
            AssertCode("@@@", "575F");
        }

        // .................:.::011110:::.:
        // .................:.::.::::.:::.: sst_b
        // ................:.:..101111:::.:
        // Reko: a decoder for V850 instruction FDA5 at address 00101EA2 has not been implemented.
        [Test]
        public void V850Dis_FDA5()
        {
            AssertCode("@@@", "FDA5");
        }

        // .................:.::010100:::::
        // Reko: a decoder for V850 instruction 9F5A at address 00101EA4 has not been implemented.
        [Test]
        public void V850Dis_9F5A()
        {
            AssertCode("@@@", "9F5A");
        }

        // .................:::.000000::::.
        // ................01110......::::.
        // .................:::.......::::. mov
        // ................:..:.000111..:::
        // Reko: a decoder for V850 instruction E790 at address 00101EA8 has not been implemented.
        [Test]
        public void V850Dis_E790()
        {
            AssertCode("@@@", "E790");
        }

        // ..................:::011011:.:..
        // ..................:::.::.:::.:.. sld_b
        // ................:::.:010011.::::
        // Reko: a decoder for V850 instruction 6FEA at address 00101EAC has not been implemented.
        [Test]
        public void V850Dis_6FEA()
        {
            AssertCode("@@@", "6FEA");
        }

        // ................:..::100001:..:.
        // ................:..:::....::..:. sld_h
        // ................:.:..110100..:::
        // Reko: a decoder for V850 instruction 87A6 at address 00101EB0 has not been implemented.
        [Test]
        public void V850Dis_87A6()
        {
            AssertCode("@@@", "87A6");
        }

        // ...................:.000010.:.:.
        // ................00010....:..:.:.
        // ...................:.....:.01010
        // ...................:.....:..:.:. divh
        // ................:.:..110111.::::
        // Reko: a decoder for V850 instruction EFA6 at address 00101EB4 has not been implemented.
        [Test]
        public void V850Dis_EFA6()
        {
            AssertCode("@@@", "EFA6");
        }

        // ................::...101111..:.:
        // Reko: a decoder for V850 instruction E5C5 at address 00101EB6 has not been implemented.
        [Test]
        public void V850Dis_E5C5()
        {
            AssertCode("@@@", "E5C5");
        }

        // ................:::::110100:..:.
        // ................::::.101101:.:.:
        // Reko: a decoder for V850 instruction B5F5 at address 00101EBA has not been implemented.
        [Test]
        public void V850Dis_B5F5()
        {
            AssertCode("@@@", "B5F5");
        }

        // ................::::.101011...::
        // Reko: a decoder for V850 instruction 63F5 at address 00101EBC has not been implemented.
        [Test]
        public void V850Dis_63F5()
        {
            AssertCode("@@@", "63F5");
        }

        // .................:...001111.::::
        // Reko: a decoder for V850 instruction EF41 at address 00101EBE has not been implemented.
        [Test]
        public void V850Dis_EF41()
        {
            AssertCode("@@@", "EF41");
        }

        // .................:.:.101000::...
        // .................:.:.:.:...::..0 Sld.w/Sst.w
        // .................:.:.:.:...::... sld_w
        // ................:..:.010000...:.
        // ................:..:..:....00010
        // ................:..:..:.......:. mov
        // ..................::.101001:::..
        // Reko: a decoder for V850 instruction 3C35 at address 00101EC4 has not been implemented.
        [Test]
        public void V850Dis_3C35()
        {
            AssertCode("@@@", "3C35");
        }

        // .................:::.011100:::..
        // .................:::..:::..:::.. sst_b
        // .....................010001:.:::
        // Reko: a decoder for V850 instruction 3702 at address 00101EC8 has not been implemented.
        [Test]
        public void V850Dis_3702()
        {
            AssertCode("@@@", "3702");
        }

        // ..................:.:000000:...:
        // ................00101......:...:
        // ..................:.:......:...: mov
        // ................::...010000::...
        // ................::....:....11000
        // ................::....:....::... mov
        // ................:.:..110000:::.:
        // Reko: a decoder for V850 instruction 1DA6 at address 00101ECE has not been implemented.
        [Test]
        public void V850Dis_1DA6()
        {
            AssertCode("@@@", "1DA6");
        }

        // ................:.:::011110..:::
        // ................:.:::.::::...::: sst_b
        // .....................010010:.::.
        // Reko: a decoder for V850 instruction 5602 at address 00101ED2 has not been implemented.
        [Test]
        public void V850Dis_5602()
        {
            AssertCode("@@@", "5602");
        }

        // .................:...110110.:...
        // Reko: a decoder for V850 instruction C846 at address 00101ED4 has not been implemented.
        [Test]
        public void V850Dis_C846()
        {
            AssertCode("@@@", "C846");
        }

        // ..................::.111011:....
        // Reko: a decoder for V850 instruction 7037 at address 00101ED6 has not been implemented.
        [Test]
        public void V850Dis_7037()
        {
            AssertCode("@@@", "7037");
        }

        // ..................::.111011:.::.
        // Reko: a decoder for V850 instruction 7637 at address 00101ED8 has not been implemented.
        [Test]
        public void V850Dis_7637()
        {
            AssertCode("@@@", "7637");
        }

        // ................:.:..000000:..::
        // ................10100......:..::
        // ................:.:........:..:: mov
        // ..................:.:001101..:.:
        // Reko: a decoder for V850 instruction A529 at address 00101EDC has not been implemented.
        [Test]
        public void V850Dis_A529()
        {
            AssertCode("@@@", "A529");
        }

        // ................::::.001000.::..
        // ................::::...:....::.. or
        // ..................::.011101....:
        // ..................::..:::.:....: sst_b
        // ...................::001001...::
        // Reko: a decoder for V850 instruction 2319 at address 00101EE2 has not been implemented.
        [Test]
        public void V850Dis_2319()
        {
            AssertCode("@@@", "2319");
        }

        // ................:..::111111..:::
        // Reko: a decoder for V850 instruction E79F at address 00101EE4 has not been implemented.
        [Test]
        public void V850Dis_E79F()
        {
            AssertCode("@@@", "E79F");
        }

        // .................:.:.010011.....
        // Reko: a decoder for V850 instruction 6052 at address 00101EE6 has not been implemented.
        [Test]
        public void V850Dis_6052()
        {
            AssertCode("@@@", "6052");
        }

        // ................::.:.110000.:.:.
        // Reko: a decoder for V850 instruction 0AD6 at address 00101EE8 has not been implemented.
        [Test]
        public void V850Dis_0AD6()
        {
            AssertCode("@@@", "0AD6");
        }

        // ................:::.:100110::..:
        // ................:::.::..::.::..: sst_h
        // ................:.:::100100:.:..
        // ................:.::::..:..:.:.. sst_h
        // ................::.:.000101.::.:
        // Reko: a decoder for V850 instruction ADD0 at address 00101EEE has not been implemented.
        [Test]
        public void V850Dis_ADD0()
        {
            AssertCode("@@@", "ADD0");
        }

        // ....................:111111.....
        // Reko: a decoder for V850 instruction E00F at address 00101EF0 has not been implemented.
        [Test]
        public void V850Dis_E00F()
        {
            AssertCode("@@@", "E00F");
        }

        // ................:.:.:000001.::.:
        // ................:.:.:.....:.::.: not
        // ................:::.:101100:::::
        // Reko: a decoder for V850 instruction 9FED at address 00101EF4 has not been implemented.
        [Test]
        public void V850Dis_9FED()
        {
            AssertCode("@@@", "9FED");
        }

        // ................::.:.001011::...
        // Reko: a decoder for V850 instruction 78D1 at address 00101EF6 has not been implemented.
        [Test]
        public void V850Dis_78D1()
        {
            AssertCode("@@@", "78D1");
        }

        // ................::..:011111..:.:
        // ................::..:.:::::..:.: sst_b
        // .....................100000:::..
        // .....................:.....:::.. sld_h
        // ................:::::011001...:.
        // ................:::::.::..:...:. sld_b
        // ....................:101110.:::.
        // Reko: a decoder for V850 instruction CE0D at address 00101EFE has not been implemented.
        [Test]
        public void V850Dis_CE0D()
        {
            AssertCode("@@@", "CE0D");
        }

        // ...................::111111:.::.
        // Reko: a decoder for V850 instruction F61F at address 00101F00 has not been implemented.
        [Test]
        public void V850Dis_F61F()
        {
            AssertCode("@@@", "F61F");
        }

        // ................:::..101100..:::
        // Reko: a decoder for V850 instruction 87E5 at address 00101F02 has not been implemented.
        [Test]
        public void V850Dis_87E5()
        {
            AssertCode("@@@", "87E5");
        }

        // .....................100100:::::
        // .....................:..:..::::: sst_h
        // ..................:.:000101:::::
        // Reko: a decoder for V850 instruction BF28 at address 00101F06 has not been implemented.
        [Test]
        public void V850Dis_BF28()
        {
            AssertCode("@@@", "BF28");
        }

        // .....................100010::.:.
        // .....................:...:.::.:. sld_h
        // ................::::.001111:.:::
        // Reko: a decoder for V850 instruction F7F1 at address 00101F0A has not been implemented.
        [Test]
        public void V850Dis_F7F1()
        {
            AssertCode("@@@", "F7F1");
        }

        // ................:....100011.::.:
        // ................:....:...::.::.: sld_h
        // ................:.:::001101...::
        // Reko: a decoder for V850 instruction A3B9 at address 00101F0E has not been implemented.
        [Test]
        public void V850Dis_A3B9()
        {
            AssertCode("@@@", "A3B9");
        }

        // .................:...011100.:.:.
        // .................:....:::...:.:. sst_b
        // ................:..:.101110.....
        // Reko: a decoder for V850 instruction C095 at address 00101F12 has not been implemented.
        [Test]
        public void V850Dis_C095()
        {
            AssertCode("@@@", "C095");
        }

        // .................::..011111::.::
        // .................::...:::::::.:: sst_b
        // ................:.::.011010:..:.
        // ................:.::..::.:.:..:. sld_b
        // .................:.:.001101::.:.
        // Reko: a decoder for V850 instruction BA51 at address 00101F18 has not been implemented.
        [Test]
        public void V850Dis_BA51()
        {
            AssertCode("@@@", "BA51");
        }

        // ...................::011001:.:..
        // ...................::.::..::.:.. sld_b
        // .................:.::011100...:.
        // .................:.::.:::.....:. sst_b
        // ....................:111001.::::
        // Reko: a decoder for V850 instruction 2F0F at address 00101F1E has not been implemented.
        [Test]
        public void V850Dis_2F0F()
        {
            AssertCode("@@@", "2F0F");
        }

        // ................::..:011000..:::
        // ................::..:.::.....::: sld_b
        // .................:::.100111:.::.
        // .................:::.:..::::.::. sst_h
        // ................::..:000001:.:..
        // ................::..:.....::.:.. not
        // ................::.::100001:....
        // ................::.:::....::.... sld_h
        // .....................101010:....
        // Reko: a decoder for V850 instruction 5005 at address 00101F28 has not been implemented.
        [Test]
        public void V850Dis_5005()
        {
            AssertCode("@@@", "5005");
        }

        // ................:..::011010.:::.
        // ................:..::.::.:..:::. sld_b
        // ................:....111111:.:.:
        // Reko: a decoder for V850 instruction F587 at address 00101F2C has not been implemented.
        [Test]
        public void V850Dis_F587()
        {
            AssertCode("@@@", "F587");
        }

        // .................::..001011..:..
        // Reko: a decoder for V850 instruction 6461 at address 00101F2E has not been implemented.
        [Test]
        public void V850Dis_6461()
        {
            AssertCode("@@@", "6461");
        }

        // .................::..010001.:...
        // Reko: a decoder for V850 instruction 2862 at address 00101F30 has not been implemented.
        [Test]
        public void V850Dis_2862()
        {
            AssertCode("@@@", "2862");
        }

        // ................::.::110011::.:.
        // ..................:::000001::.:.
        // ..................:::.....:::.:. not
        // ................:..:.011000:..:.
        // ................:..:..::...:..:. sld_b
        // ................::.:.100110.::.:
        // ................::.:.:..::..::.: sst_h
        // .................:..:101001.:.::
        // Reko: a decoder for V850 instruction 2B4D at address 00101F3A has not been implemented.
        [Test]
        public void V850Dis_2B4D()
        {
            AssertCode("@@@", "2B4D");
        }

        // ................:.:..011110.::::
        // ................:.:...::::..:::: sst_b
        // ................:::::110101..:.:
        // Reko: a decoder for V850 instruction A5FE at address 00101F3E has not been implemented.
        [Test]
        public void V850Dis_A5FE()
        {
            AssertCode("@@@", "A5FE");
        }

        // ................:::.:000110:.:..
        // Reko: a decoder for V850 instruction D4E8 at address 00101F40 has not been implemented.
        [Test]
        public void V850Dis_D4E8()
        {
            AssertCode("@@@", "D4E8");
        }

        // ................:.:..001111.:..:
        // Reko: a decoder for V850 instruction E9A1 at address 00101F42 has not been implemented.
        [Test]
        public void V850Dis_E9A1()
        {
            AssertCode("@@@", "E9A1");
        }

        // ..................:.:110100.::.:
        // Reko: a decoder for V850 instruction 8D2E at address 00101F44 has not been implemented.
        [Test]
        public void V850Dis_8D2E()
        {
            AssertCode("@@@", "8D2E");
        }

        // ................::...010100..:.:
        // Reko: a decoder for V850 instruction 85C2 at address 00101F46 has not been implemented.
        [Test]
        public void V850Dis_85C2()
        {
            AssertCode("@@@", "85C2");
        }

        // ................:::..100000:..:.
        // ................:::..:.....:..:. sld_h
        // ................::::.110010..:.:
        // Reko: a decoder for V850 instruction 45F6 at address 00101F4A has not been implemented.
        [Test]
        public void V850Dis_45F6()
        {
            AssertCode("@@@", "45F6");
        }

        // ................:.::.110000:.:.:
        // Reko: a decoder for V850 instruction 15B6 at address 00101F4C has not been implemented.
        [Test]
        public void V850Dis_15B6()
        {
            AssertCode("@@@", "15B6");
        }

        // ..................::.010101::::.
        // ................::.:.011111.::.:
        // ................::.:..:::::.::.: sst_b
        // ..................:..000111:..::
        // Reko: a decoder for V850 instruction F320 at address 00101F52 has not been implemented.
        [Test]
        public void V850Dis_F320()
        {
            AssertCode("@@@", "F320");
        }

        // .................:.:.001111....:
        // Reko: a decoder for V850 instruction E151 at address 00101F54 has not been implemented.
        [Test]
        public void V850Dis_E151()
        {
            AssertCode("@@@", "E151");
        }

        // ................::..:111110:.:..
        // Reko: a decoder for V850 instruction D4CF at address 00101F56 has not been implemented.
        [Test]
        public void V850Dis_D4CF()
        {
            AssertCode("@@@", "D4CF");
        }

        // .................::..011101::..:
        // .................::...:::.:::..: sst_b
        // .................::..000101..::.
        // Reko: a decoder for V850 instruction A660 at address 00101F5A has not been implemented.
        [Test]
        public void V850Dis_A660()
        {
            AssertCode("@@@", "A660");
        }

        // ................:::..011100:::..
        // ................:::...:::..:::.. sst_b
        // ................:.:.:100000.::..
        // ................:.:.::......::.. sld_h
        // .................:..:111011.....
        // Reko: a decoder for V850 instruction 604F at address 00101F60 has not been implemented.
        [Test]
        public void V850Dis_604F()
        {
            AssertCode("@@@", "604F");
        }

        // ....................:100110..:::
        // ....................::..::...::: sst_h
        // ................::...011100:::.:
        // ................::....:::..:::.: sst_b
        // ................:::::001110:..::
        // Reko: a decoder for V850 instruction D3F9 at address 00101F66 has not been implemented.
        [Test]
        public void V850Dis_D3F9()
        {
            AssertCode("@@@", "D3F9");
        }

        // ................:.:::000110:::.:
        // Reko: a decoder for V850 instruction DDB8 at address 00101F68 has not been implemented.
        [Test]
        public void V850Dis_DDB8()
        {
            AssertCode("@@@", "DDB8");
        }

        // .................:.:.110110.::..
        // Reko: a decoder for V850 instruction CC56 at address 00101F6A has not been implemented.
        [Test]
        public void V850Dis_CC56()
        {
            AssertCode("@@@", "CC56");
        }

        // ...................::100111:..::
        // ...................:::..::::..:: sst_h
        // ...................:.111011:::.:
        // Reko: a decoder for V850 instruction 7D17 at address 00101F6E has not been implemented.
        [Test]
        public void V850Dis_7D17()
        {
            AssertCode("@@@", "7D17");
        }

        // ................:::::000000:.:.:
        // ................11111......:.:.:
        // ................:::::......:.:.: mov
        // ................:::::100011.....
        // ................::::::...::..... sld_h
        // ................:....100101.::::
        // ................:....:..:.:.:::: sst_h
        // ................:....011000:::.:
        // ................:.....::...:::.: sld_b
        // ...................::010000:.:..
        // ...................::.:....10100
        // ...................::.:....:.:.. mov
        // ..................:.:010111..:::
        // Reko: a decoder for V850 instruction E72A at address 00101F7A has not been implemented.
        [Test]
        public void V850Dis_E72A()
        {
            AssertCode("@@@", "E72A");
        }

        // ..................:..100100..:::
        // ..................:..:..:....::: sst_h
        // ..................:.:111001..:.:
        // Reko: a decoder for V850 instruction 252F at address 00101F7E has not been implemented.
        [Test]
        public void V850Dis_252F()
        {
            AssertCode("@@@", "252F");
        }

        // ................:..::000110:.:::
        // Reko: a decoder for V850 instruction D798 at address 00101F80 has not been implemented.
        [Test]
        public void V850Dis_D798()
        {
            AssertCode("@@@", "D798");
        }

        // ................::...101100:.:.:
        // Reko: a decoder for V850 instruction 95C5 at address 00101F82 has not been implemented.
        [Test]
        public void V850Dis_95C5()
        {
            AssertCode("@@@", "95C5");
        }

        // ................:.::.001111.::::
        // Reko: a decoder for V850 instruction EFB1 at address 00101F84 has not been implemented.
        [Test]
        public void V850Dis_EFB1()
        {
            AssertCode("@@@", "EFB1");
        }

        // ..................:.:010100:.:.:
        // Reko: a decoder for V850 instruction 952A at address 00101F86 has not been implemented.
        [Test]
        public void V850Dis_952A()
        {
            AssertCode("@@@", "952A");
        }

        // ................:::::001111:::::
        // Reko: a decoder for V850 instruction FFF9 at address 00101F88 has not been implemented.
        [Test]
        public void V850Dis_FFF9()
        {
            AssertCode("@@@", "FFF9");
        }

        // ..................:.:001010...::
        // Reko: a decoder for V850 instruction 4329 at address 00101F8A has not been implemented.
        [Test]
        public void V850Dis_4329()
        {
            AssertCode("@@@", "4329");
        }

        // .................::::101100..:..
        // Reko: a decoder for V850 instruction 847D at address 00101F8C has not been implemented.
        [Test]
        public void V850Dis_847D()
        {
            AssertCode("@@@", "847D");
        }

        // ................:..::101110::.::
        // Reko: a decoder for V850 instruction DB9D at address 00101F8E has not been implemented.
        [Test]
        public void V850Dis_DB9D()
        {
            AssertCode("@@@", "DB9D");
        }

        // ................:...:111111..:.:
        // Reko: a decoder for V850 instruction E58F at address 00101F90 has not been implemented.
        [Test]
        public void V850Dis_E58F()
        {
            AssertCode("@@@", "E58F");
        }

        // ................:.:..100010.:.:.
        // ................:.:..:...:..:.:. sld_h
        // ....................:111100...::
        // Reko: a decoder for V850 instruction 830F at address 00101F94 has not been implemented.
        [Test]
        public void V850Dis_830F()
        {
            AssertCode("@@@", "830F");
        }

        // ................::.::011111:::..
        // ................::.::.::::::::.. sst_b
        // .................:...100001:..:.
        // .................:...:....::..:. sld_h
        // ................:....011111:.:::
        // ................:.....::::::.::: sst_b
        // ................:.:.:000110::.::
        // Reko: a decoder for V850 instruction DBA8 at address 00101F9C has not been implemented.
        [Test]
        public void V850Dis_DBA8()
        {
            AssertCode("@@@", "DBA8");
        }

        // ................:.:::000011:..:.
        // Reko: a decoder for V850 instruction 72B8 at address 00101F9E has not been implemented.
        [Test]
        public void V850Dis_72B8()
        {
            AssertCode("@@@", "72B8");
        }

        // ................:..::100100:..::
        // ................:..:::..:..:..:: sst_h
        // .................:.::010011::::.
        // Reko: a decoder for V850 instruction 7E5A at address 00101FA2 has not been implemented.
        [Test]
        public void V850Dis_7E5A()
        {
            AssertCode("@@@", "7E5A");
        }

        // ................:.::.111100..::.
        // Reko: a decoder for V850 instruction 86B7 at address 00101FA4 has not been implemented.
        [Test]
        public void V850Dis_86B7()
        {
            AssertCode("@@@", "86B7");
        }

        // .................::..011001..:::
        // .................::...::..:..::: sld_b
        // ..................::.101011...::
        // Reko: a decoder for V850 instruction 6335 at address 00101FA8 has not been implemented.
        [Test]
        public void V850Dis_6335()
        {
            AssertCode("@@@", "6335");
        }

        // ................:.::.110111:::..
        // Reko: a decoder for V850 instruction FCB6 at address 00101FAA has not been implemented.
        [Test]
        public void V850Dis_FCB6()
        {
            AssertCode("@@@", "FCB6");
        }

        // ................::::.100010....:
        // ................::::.:...:.....: sld_h
        // ................:..:.111111::::.
        // Reko: a decoder for V850 instruction FE97 at address 00101FAE has not been implemented.
        [Test]
        public void V850Dis_FE97()
        {
            AssertCode("@@@", "FE97");
        }

        // .................:..:010001::::.
        // Reko: a decoder for V850 instruction 3E4A at address 00101FB0 has not been implemented.
        [Test]
        public void V850Dis_3E4A()
        {
            AssertCode("@@@", "3E4A");
        }

        // ................::::.111010..:..
        // Reko: a decoder for V850 instruction 44F7 at address 00101FB2 has not been implemented.
        [Test]
        public void V850Dis_44F7()
        {
            AssertCode("@@@", "44F7");
        }

        // ................:....000110::..:
        // Reko: a decoder for V850 instruction D980 at address 00101FB4 has not been implemented.
        [Test]
        public void V850Dis_D980()
        {
            AssertCode("@@@", "D980");
        }

        // ................::...111101.:.::
        // Reko: a decoder for V850 instruction ABC7 at address 00101FB6 has not been implemented.
        [Test]
        public void V850Dis_ABC7()
        {
            AssertCode("@@@", "ABC7");
        }

        // ................:.:::010100::.::
        // Reko: a decoder for V850 instruction 9BBA at address 00101FB8 has not been implemented.
        [Test]
        public void V850Dis_9BBA()
        {
            AssertCode("@@@", "9BBA");
        }

        // ................:.:.:011100:..::
        // ................:.:.:.:::..:..:: sst_b
        // ...................:.010101::..:
        // Reko: a decoder for V850 instruction B912 at address 00101FBC has not been implemented.
        [Test]
        public void V850Dis_B912()
        {
            AssertCode("@@@", "B912");
        }

        // .................:.:.110010.::::
        // ................:...:011100..::.
        // ................:...:.:::....::. sst_b
        // ................:::..111100:.:.:
        // Reko: a decoder for V850 instruction 95E7 at address 00101FC2 has not been implemented.
        [Test]
        public void V850Dis_95E7()
        {
            AssertCode("@@@", "95E7");
        }

        // ................:....001111:::::
        // Reko: a decoder for V850 instruction FF81 at address 00101FC4 has not been implemented.
        [Test]
        public void V850Dis_FF81()
        {
            AssertCode("@@@", "FF81");
        }

        // ..................:.:101000:.:::
        // ..................:.::.:...:.::1 Sld.w/Sst.w
        // ..................:.::.:...:.::: sst_w
        // ................:::..110011.:...
        // Reko: a decoder for V850 instruction 68E6 at address 00101FC8 has not been implemented.
        [Test]
        public void V850Dis_68E6()
        {
            AssertCode("@@@", "68E6");
        }

        // .................:..:101010....:
        // Reko: a decoder for V850 instruction 414D at address 00101FCA has not been implemented.
        [Test]
        public void V850Dis_414D()
        {
            AssertCode("@@@", "414D");
        }

        // .................:...110000:..::
        // Reko: a decoder for V850 instruction 1346 at address 00101FCC has not been implemented.
        [Test]
        public void V850Dis_1346()
        {
            AssertCode("@@@", "1346");
        }

        // ..................:::101100::.:.
        // Reko: a decoder for V850 instruction 9A3D at address 00101FCE has not been implemented.
        [Test]
        public void V850Dis_9A3D()
        {
            AssertCode("@@@", "9A3D");
        }

        // .................::..000010..:.:
        // ................01100....:...:.:
        // .................::......:.00101
        // .................::......:...:.: divh
        // ................:..:.000010..:..
        // ................10010....:...:..
        // ................:..:.....:.00100
        // ................:..:.....:...:.. divh
        // .................::::010011..:..
        // Reko: a decoder for V850 instruction 647A at address 00101FD4 has not been implemented.
        [Test]
        public void V850Dis_647A()
        {
            AssertCode("@@@", "647A");
        }

        // ...................:.010110::::.
        // Reko: a decoder for V850 instruction DE12 at address 00101FD6 has not been implemented.
        [Test]
        public void V850Dis_DE12()
        {
            AssertCode("@@@", "DE12");
        }

        // ................:.::.110111:...:
        // Reko: a decoder for V850 instruction F1B6 at address 00101FD8 has not been implemented.
        [Test]
        public void V850Dis_F1B6()
        {
            AssertCode("@@@", "F1B6");
        }

        // ................::..:011101::::.
        // ................::..:.:::.:::::. sst_b
        // .................::::100001...::
        // .................:::::....:...:: sld_h
        // .................:.:.001101.:..:
        // Reko: a decoder for V850 instruction A951 at address 00101FDE has not been implemented.
        [Test]
        public void V850Dis_A951()
        {
            AssertCode("@@@", "A951");
        }

        // ................:.:..101111..::.
        // Reko: a decoder for V850 instruction E6A5 at address 00101FE0 has not been implemented.
        [Test]
        public void V850Dis_E6A5()
        {
            AssertCode("@@@", "E6A5");
        }

        // ................:.::.101001.::::
        // Reko: a decoder for V850 instruction 2FB5 at address 00101FE2 has not been implemented.
        [Test]
        public void V850Dis_2FB5()
        {
            AssertCode("@@@", "2FB5");
        }

        // ...................:.110010::::.
        // Reko: a decoder for V850 instruction 5E16 at address 00101FE4 has not been implemented.
        [Test]
        public void V850Dis_5E16()
        {
            AssertCode("@@@", "5E16");
        }

        // ................:.:::101100:.:::
        // Reko: a decoder for V850 instruction 97BD at address 00101FE6 has not been implemented.
        [Test]
        public void V850Dis_97BD()
        {
            AssertCode("@@@", "97BD");
        }

        // ...................:.111001:..::
        // Reko: a decoder for V850 instruction 3317 at address 00101FE8 has not been implemented.
        [Test]
        public void V850Dis_3317()
        {
            AssertCode("@@@", "3317");
        }

        // ................::::.011100.::::
        // ................::::..:::...:::: sst_b
        // .................:.::110001:::::
        // Reko: a decoder for V850 instruction 3F5E at address 00101FEC has not been implemented.
        [Test]
        public void V850Dis_3F5E()
        {
            AssertCode("@@@", "3F5E");
        }

        // .................:.:.101101:.:..
        // Reko: a decoder for V850 instruction B455 at address 00101FEE has not been implemented.
        [Test]
        public void V850Dis_B455()
        {
            AssertCode("@@@", "B455");
        }

        // .................:.:.101000:::::
        // .................:.:.:.:...::::1 Sld.w/Sst.w
        // .................:.:.:.:...::::: sst_w
        // .................:.::001011..:.:
        // Reko: a decoder for V850 instruction 6559 at address 00101FF2 has not been implemented.
        [Test]
        public void V850Dis_6559()
        {
            AssertCode("@@@", "6559");
        }

        // ................:.:::110110.::.:
        // Reko: a decoder for V850 instruction CDBE at address 00101FF4 has not been implemented.
        [Test]
        public void V850Dis_CDBE()
        {
            AssertCode("@@@", "CDBE");
        }

        // ....................:011111:..:.
        // ....................:.::::::..:. sst_b
        // ................:....011101.::..
        // ................:.....:::.:.::.. sst_b
        // .................:..:000111:.::.
        // Reko: a decoder for V850 instruction F648 at address 00101FFA has not been implemented.
        [Test]
        public void V850Dis_F648()
        {
            AssertCode("@@@", "F648");
        }

        // .................:..:011110...:.
        // .................:..:.::::....:. sst_b
        // ................:::.:100000.:..:
        // ................:::.::......:..: sld_h
        // ................:::.:111001:::..
        // Reko: a decoder for V850 instruction 3CEF at address 00102000 has not been implemented.
        [Test]
        public void V850Dis_3CEF()
        {
            AssertCode("@@@", "3CEF");
        }

        // .................::.:101011..:::
        // Reko: a decoder for V850 instruction 676D at address 00102002 has not been implemented.
        [Test]
        public void V850Dis_676D()
        {
            AssertCode("@@@", "676D");
        }

        // ................:::..100000:.::.
        // ................:::..:.....:.::. sld_h
        // ..................:.:110110:::::
        // Reko: a decoder for V850 instruction DF2E at address 00102006 has not been implemented.
        [Test]
        public void V850Dis_DF2E()
        {
            AssertCode("@@@", "DF2E");
        }

        // ................:....110110.:...
        // Reko: a decoder for V850 instruction C886 at address 00102008 has not been implemented.
        [Test]
        public void V850Dis_C886()
        {
            AssertCode("@@@", "C886");
        }

        // .................:..:100010:::.:
        // .................:..::...:.:::.: sld_h
        // ................:::.:011111.:.::
        // ................:::.:.:::::.:.:: sst_b
        // ................:..:.011110:..:.
        // ................:..:..::::.:..:. sst_b
        // .................::.:001000....:
        // .................::.:..:.......: or
        // ................::..:110101.::..
        // Reko: a decoder for V850 instruction ACCE at address 00102012 has not been implemented.
        [Test]
        public void V850Dis_ACCE()
        {
            AssertCode("@@@", "ACCE");
        }

        // .................:.:.000000:.::.
        // ................01010......:.::.
        // .................:.:.......:.::. mov
        // ..................:..111100...::
        // Reko: a decoder for V850 instruction 8327 at address 00102016 has not been implemented.
        [Test]
        public void V850Dis_8327()
        {
            AssertCode("@@@", "8327");
        }

        // ..................:..011101:..:.
        // ..................:...:::.::..:. sst_b
        // ................::.::011000.:...
        // ................::.::.::....:... sld_b
        // .................:..:111000:::::
        // Reko: a decoder for V850 instruction 1F4F at address 0010201C has not been implemented.
        [Test]
        public void V850Dis_1F4F()
        {
            AssertCode("@@@", "1F4F");
        }

        // ................:::..111111::.::
        // Reko: a decoder for V850 instruction FBE7 at address 0010201E has not been implemented.
        [Test]
        public void V850Dis_FBE7()
        {
            AssertCode("@@@", "FBE7");
        }

        // ................:.:..011000.::.:
        // ................:.:...::....::.: sld_b
        // .................:..:111101:.::.
        // Reko: a decoder for V850 instruction B64F at address 00102022 has not been implemented.
        [Test]
        public void V850Dis_B64F()
        {
            AssertCode("@@@", "B64F");
        }

        // .................:...110111:.::.
        // Reko: a decoder for V850 instruction F646 at address 00102024 has not been implemented.
        [Test]
        public void V850Dis_F646()
        {
            AssertCode("@@@", "F646");
        }

        // ..................:::010001:::::
        // Reko: a decoder for V850 instruction 3F3A at address 00102026 has not been implemented.
        [Test]
        public void V850Dis_3F3A()
        {
            AssertCode("@@@", "3F3A");
        }

        // ..................::.110011:.::.
        // Reko: a decoder for V850 instruction 7636 at address 00102028 has not been implemented.
        [Test]
        public void V850Dis_7636()
        {
            AssertCode("@@@", "7636");
        }

        // .................:...111100:....
        // Reko: a decoder for V850 instruction 9047 at address 0010202A has not been implemented.
        [Test]
        public void V850Dis_9047()
        {
            AssertCode("@@@", "9047");
        }

        // ................:.:..101011.....
        // Reko: a decoder for V850 instruction 60A5 at address 0010202C has not been implemented.
        [Test]
        public void V850Dis_60A5()
        {
            AssertCode("@@@", "60A5");
        }

        // ................::...001001.....
        // Reko: a decoder for V850 instruction 20C1 at address 0010202E has not been implemented.
        [Test]
        public void V850Dis_20C1()
        {
            AssertCode("@@@", "20C1");
        }

        // .................::..011101:....
        // .................::...:::.::.... sst_b
        // .................:.::001010..::.
        // Reko: a decoder for V850 instruction 4659 at address 00102032 has not been implemented.
        [Test]
        public void V850Dis_4659()
        {
            AssertCode("@@@", "4659");
        }

        // ..................:::011100::::.
        // ..................:::.:::..::::. sst_b
        // ................:::.:100110:.:.:
        // ................:::.::..::.:.:.: sst_h
        // ..................:::111011.::..
        // Reko: a decoder for V850 instruction 6C3F at address 00102038 has not been implemented.
        [Test]
        public void V850Dis_6C3F()
        {
            AssertCode("@@@", "6C3F");
        }

        // ................::::.010101::.:.
        // Reko: a decoder for V850 instruction BAF2 at address 0010203A has not been implemented.
        [Test]
        public void V850Dis_BAF2()
        {
            AssertCode("@@@", "BAF2");
        }

        // .....................100110:::..
        // .....................:..::.:::.. sst_h
        // .................::::001000.::::
        // .................::::..:....:::: or
        // .................:::.110100.:.::
        // .................:..:000001:::::
        // .................:..:.....:::::: not
        // ................:....011001:.:.:
        // ................:.....::..::.:.: sld_b
        // ................:.:::010100:::..
        // Reko: a decoder for V850 instruction 9CBA at address 00102046 has not been implemented.
        [Test]
        public void V850Dis_9CBA()
        {
            AssertCode("@@@", "9CBA");
        }

        // .................:::.001001.:.:.
        // Reko: a decoder for V850 instruction 2A71 at address 00102048 has not been implemented.
        [Test]
        public void V850Dis_2A71()
        {
            AssertCode("@@@", "2A71");
        }

        // ................::.::111010:.:::
        // Reko: a decoder for V850 instruction 57DF at address 0010204A has not been implemented.
        [Test]
        public void V850Dis_57DF()
        {
            AssertCode("@@@", "57DF");
        }

        // ..................:::100110.....
        // ..................::::..::...... sst_h
        // ....................:100110..:::
        // ....................::..::...::: sst_h
        // .................:...111000...::
        // Reko: a decoder for V850 instruction 0347 at address 00102050 has not been implemented.
        [Test]
        public void V850Dis_0347()
        {
            AssertCode("@@@", "0347");
        }

        // ................:.:::100001.:.:.
        // ................:.::::....:.:.:. sld_h
        // ................:.:..001000.:...
        // ................:.:....:....:... or
        // ................::.:.001010.:...
        // Reko: a decoder for V850 instruction 48D1 at address 00102056 has not been implemented.
        [Test]
        public void V850Dis_48D1()
        {
            AssertCode("@@@", "48D1");
        }

        // ................::..:010110.::.:
        // Reko: a decoder for V850 instruction CDCA at address 00102058 has not been implemented.
        [Test]
        public void V850Dis_CDCA()
        {
            AssertCode("@@@", "CDCA");
        }

        // ................:.::.111111...::
        // Reko: a decoder for V850 instruction E3B7 at address 0010205A has not been implemented.
        [Test]
        public void V850Dis_E3B7()
        {
            AssertCode("@@@", "E3B7");
        }

        // .................:..:100100.:.::
        // .................:..::..:...:.:: sst_h
        // .................:::.001010::::.
        // Reko: a decoder for V850 instruction 5E71 at address 0010205E has not been implemented.
        [Test]
        public void V850Dis_5E71()
        {
            AssertCode("@@@", "5E71");
        }

        // ...................:.110001..:::
        // Reko: a decoder for V850 instruction 2716 at address 00102060 has not been implemented.
        [Test]
        public void V850Dis_2716()
        {
            AssertCode("@@@", "2716");
        }

        // ................:::.:010101:::..
        // Reko: a decoder for V850 instruction BCEA at address 00102062 has not been implemented.
        [Test]
        public void V850Dis_BCEA()
        {
            AssertCode("@@@", "BCEA");
        }

        // ................::.:.111011..::.
        // Reko: a decoder for V850 instruction 66D7 at address 00102064 has not been implemented.
        [Test]
        public void V850Dis_66D7()
        {
            AssertCode("@@@", "66D7");
        }

        // ..................:.:101100.::..
        // Reko: a decoder for V850 instruction 8C2D at address 00102066 has not been implemented.
        [Test]
        public void V850Dis_8C2D()
        {
            AssertCode("@@@", "8C2D");
        }

        // ................:...:011110..:::
        // ................:...:.::::...::: sst_b
        // ..................:.:101001.::::
        // Reko: a decoder for V850 instruction 2F2D at address 0010206A has not been implemented.
        [Test]
        public void V850Dis_2F2D()
        {
            AssertCode("@@@", "2F2D");
        }

        // ................::::.110111..:..
        // Reko: a decoder for V850 instruction E4F6 at address 0010206C has not been implemented.
        [Test]
        public void V850Dis_E4F6()
        {
            AssertCode("@@@", "E4F6");
        }

        // ..................::.000001.:.::
        // ..................::......:.:.:: not
        // ................:.:.:110011::...
        // Reko: a decoder for V850 instruction 78AE at address 00102070 has not been implemented.
        [Test]
        public void V850Dis_78AE()
        {
            AssertCode("@@@", "78AE");
        }

        // ................:..:.100111.:::.
        // ................:..:.:..:::.:::. sst_h
        // .................:.:.010110::..:
        // Reko: a decoder for V850 instruction D952 at address 00102074 has not been implemented.
        [Test]
        public void V850Dis_D952()
        {
            AssertCode("@@@", "D952");
        }

        // ................:::.:001011:....
        // Reko: a decoder for V850 instruction 70E9 at address 00102076 has not been implemented.
        [Test]
        public void V850Dis_70E9()
        {
            AssertCode("@@@", "70E9");
        }

        // .................::.:110001.::.:
        // Reko: a decoder for V850 instruction 2D6E at address 00102078 has not been implemented.
        [Test]
        public void V850Dis_2D6E()
        {
            AssertCode("@@@", "2D6E");
        }

        // .................::.:010011::.:.
        // Reko: a decoder for V850 instruction 7A6A at address 0010207A has not been implemented.
        [Test]
        public void V850Dis_7A6A()
        {
            AssertCode("@@@", "7A6A");
        }

        // ................:::..101100:::::
        // Reko: a decoder for V850 instruction 9FE5 at address 0010207C has not been implemented.
        [Test]
        public void V850Dis_9FE5()
        {
            AssertCode("@@@", "9FE5");
        }

        // ................::.::101001.:::.
        // Reko: a decoder for V850 instruction 2EDD at address 0010207E has not been implemented.
        [Test]
        public void V850Dis_2EDD()
        {
            AssertCode("@@@", "2EDD");
        }

        // ................:::..110000:..:.
        // Reko: a decoder for V850 instruction 12E6 at address 00102080 has not been implemented.
        [Test]
        public void V850Dis_12E6()
        {
            AssertCode("@@@", "12E6");
        }

        // ................:.:.:001101:.:::
        // Reko: a decoder for V850 instruction B7A9 at address 00102082 has not been implemented.
        [Test]
        public void V850Dis_B7A9()
        {
            AssertCode("@@@", "B7A9");
        }

        // ................::.::100100.....
        // ................::.:::..:....... sst_h
        // ..................:.:011101..::.
        // ..................:.:.:::.:..::. sst_b
        // ................:::::101101::.:.
        // Reko: a decoder for V850 instruction BAFD at address 00102088 has not been implemented.
        [Test]
        public void V850Dis_BAFD()
        {
            AssertCode("@@@", "BAFD");
        }

        // .................::::101111:.:..
        // Reko: a decoder for V850 instruction F47D at address 0010208A has not been implemented.
        [Test]
        public void V850Dis_F47D()
        {
            AssertCode("@@@", "F47D");
        }

        // ...................::011011:..:.
        // ...................::.::.:::..:. sld_b
        // ................::.:.111111.::.:
        // Reko: a decoder for V850 instruction EDD7 at address 0010208E has not been implemented.
        [Test]
        public void V850Dis_EDD7()
        {
            AssertCode("@@@", "EDD7");
        }

        // ................:.::.011100::::.
        // ................:.::..:::..::::. sst_b
        // ................::.::101001:::::
        // Reko: a decoder for V850 instruction 3FDD at address 00102092 has not been implemented.
        [Test]
        public void V850Dis_3FDD()
        {
            AssertCode("@@@", "3FDD");
        }

        // ................::..:001010:::.:
        // Reko: a decoder for V850 instruction 5DC9 at address 00102094 has not been implemented.
        [Test]
        public void V850Dis_5DC9()
        {
            AssertCode("@@@", "5DC9");
        }

        // ................::.:.100100::...
        // ................::.:.:..:..::... sst_h
        // ................::.:.010101..::.
        // Reko: a decoder for V850 instruction A6D2 at address 00102098 has not been implemented.
        [Test]
        public void V850Dis_A6D2()
        {
            AssertCode("@@@", "A6D2");
        }

        // ................::::.111101:::..
        // Reko: a decoder for V850 instruction BCF7 at address 0010209A has not been implemented.
        [Test]
        public void V850Dis_BCF7()
        {
            AssertCode("@@@", "BCF7");
        }

        // ..................:..001010:::.:
        // Reko: a decoder for V850 instruction 5D21 at address 0010209C has not been implemented.
        [Test]
        public void V850Dis_5D21()
        {
            AssertCode("@@@", "5D21");
        }

        // ................:::..010100.::.:
        // Reko: a decoder for V850 instruction 8DE2 at address 0010209E has not been implemented.
        [Test]
        public void V850Dis_8DE2()
        {
            AssertCode("@@@", "8DE2");
        }

        // .................:.:.001010:::..
        // Reko: a decoder for V850 instruction 5C51 at address 001020A0 has not been implemented.
        [Test]
        public void V850Dis_5C51()
        {
            AssertCode("@@@", "5C51");
        }

        // ................:::::101000::.:.
        // ................::::::.:...::.:0 Sld.w/Sst.w
        // ................::::::.:...::.:. sld_w
        // ................:....010001::::.
        // Reko: a decoder for V850 instruction 3E82 at address 001020A4 has not been implemented.
        [Test]
        public void V850Dis_3E82()
        {
            AssertCode("@@@", "3E82");
        }

        // ................:..::011101:....
        // ................:..::.:::.::.... sst_b
        // ................::...010000..:::
        // ................::....:....00111
        // ................::....:......::: mov
        // ................::.::100110::.:.
        // ................::.:::..::.::.:. sst_h
        // ................::::.011010:::.:
        // ................::::..::.:.:::.: sld_b
        // ...................:.010011:.:::
        // Reko: a decoder for V850 instruction 7712 at address 001020AE has not been implemented.
        [Test]
        public void V850Dis_7712()
        {
            AssertCode("@@@", "7712");
        }

        // ................:.:..011100....:
        // ................:.:...:::......: sst_b
        // ................:::.:100110.::..
        // ................:::.::..::..::.. sst_h
        // .................:.:.001101..:.:
        // Reko: a decoder for V850 instruction A551 at address 001020B4 has not been implemented.
        [Test]
        public void V850Dis_A551()
        {
            AssertCode("@@@", "A551");
        }

        // .................:..:000011:::::
        // Reko: a decoder for V850 instruction 7F48 at address 001020B6 has not been implemented.
        [Test]
        public void V850Dis_7F48()
        {
            AssertCode("@@@", "7F48");
        }

        // ................:::..010010.:::.
        // Reko: a decoder for V850 instruction 4EE2 at address 001020B8 has not been implemented.
        [Test]
        public void V850Dis_4EE2()
        {
            AssertCode("@@@", "4EE2");
        }

        // ................:.:.:101000...::
        // ................:.:.::.:......:1 Sld.w/Sst.w
        // ................:.:.::.:......:: sst_w
        // ................:.:.:111001.....
        // Reko: a decoder for V850 instruction 20AF at address 001020BC has not been implemented.
        [Test]
        public void V850Dis_20AF()
        {
            AssertCode("@@@", "20AF");
        }

        // ................:..::000110.:::.
        // Reko: a decoder for V850 instruction CE98 at address 001020BE has not been implemented.
        [Test]
        public void V850Dis_CE98()
        {
            AssertCode("@@@", "CE98");
        }

        // ...................:.010011.::::
        // Reko: a decoder for V850 instruction 6F12 at address 001020C0 has not been implemented.
        [Test]
        public void V850Dis_6F12()
        {
            AssertCode("@@@", "6F12");
        }

        // ..................:..000001.....
        // ..................:.......:..... not
        // ................:::.:111000.....
        // Reko: a decoder for V850 instruction 00EF at address 001020C4 has not been implemented.
        [Test]
        public void V850Dis_00EF()
        {
            AssertCode("@@@", "00EF");
        }

        // ...................::100100::..:
        // ...................:::..:..::..: sst_h
        // ................:.:..100000...:.
        // ................:.:..:........:. sld_h
        // ................:..::010110:.::.
        // Reko: a decoder for V850 instruction D69A at address 001020CA has not been implemented.
        [Test]
        public void V850Dis_D69A()
        {
            AssertCode("@@@", "D69A");
        }

        // ................:.:::011111.::::
        // ................:.:::.:::::.:::: sst_b
        // ................:.::.010011:.::.
        // Reko: a decoder for V850 instruction 76B2 at address 001020CE has not been implemented.
        [Test]
        public void V850Dis_76B2()
        {
            AssertCode("@@@", "76B2");
        }

        // ................:.:.:010010::.::
        // Reko: a decoder for V850 instruction 5BAA at address 001020D0 has not been implemented.
        [Test]
        public void V850Dis_5BAA()
        {
            AssertCode("@@@", "5BAA");
        }

        // ................:.:::001011:.:.:
        // Reko: a decoder for V850 instruction 75B9 at address 001020D2 has not been implemented.
        [Test]
        public void V850Dis_75B9()
        {
            AssertCode("@@@", "75B9");
        }

        // ................:.:..001010:.:.:
        // Reko: a decoder for V850 instruction 55A1 at address 001020D4 has not been implemented.
        [Test]
        public void V850Dis_55A1()
        {
            AssertCode("@@@", "55A1");
        }

        // ....................:011001:::..
        // ....................:.::..::::.. sld_b
        // ................::.:.001110:...:
        // Reko: a decoder for V850 instruction D1D1 at address 001020D8 has not been implemented.
        [Test]
        public void V850Dis_D1D1()
        {
            AssertCode("@@@", "D1D1");
        }

        // .................:.::011101.::::
        // .................:.::.:::.:.:::: sst_b
        // .....................010011::::.
        // Reko: a decoder for V850 instruction 7E02 at address 001020DC has not been implemented.
        [Test]
        public void V850Dis_7E02()
        {
            AssertCode("@@@", "7E02");
        }

        // .................:.::000011::.:.
        // Reko: a decoder for V850 instruction 7A58 at address 001020DE has not been implemented.
        [Test]
        public void V850Dis_7A58()
        {
            AssertCode("@@@", "7A58");
        }

        // .................:...010111:.:..
        // Reko: a decoder for V850 instruction F442 at address 001020E0 has not been implemented.
        [Test]
        public void V850Dis_F442()
        {
            AssertCode("@@@", "F442");
        }

        // .................:..:011011.::..
        // .................:..:.::.::.::.. sld_b
        // .................:.:.110110:..::
        // Reko: a decoder for V850 instruction D356 at address 001020E4 has not been implemented.
        [Test]
        public void V850Dis_D356()
        {
            AssertCode("@@@", "D356");
        }

        // ................:..:.010000:...:
        // ................:..:..:....10001
        // ................:..:..:....:...: mov
        // ................:.:.:111000.:..:
        // .....................001111.:..:
        // Reko: a decoder for V850 instruction E901 at address 001020EA has not been implemented.
        [Test]
        public void V850Dis_E901()
        {
            AssertCode("@@@", "E901");
        }

        // ................:::..101011::.:.
        // Reko: a decoder for V850 instruction 7AE5 at address 001020EC has not been implemented.
        [Test]
        public void V850Dis_7AE5()
        {
            AssertCode("@@@", "7AE5");
        }

        // ................:..:.010010::..:
        // Reko: a decoder for V850 instruction 5992 at address 001020EE has not been implemented.
        [Test]
        public void V850Dis_5992()
        {
            AssertCode("@@@", "5992");
        }

        // .................:.:.000011.:.::
        // Reko: a decoder for V850 instruction 6B50 at address 001020F0 has not been implemented.
        [Test]
        public void V850Dis_6B50()
        {
            AssertCode("@@@", "6B50");
        }

        // .................:::.101010:...:
        // Reko: a decoder for V850 instruction 5175 at address 001020F2 has not been implemented.
        [Test]
        public void V850Dis_5175()
        {
            AssertCode("@@@", "5175");
        }

        // ...................:.100001::..:
        // ...................:.:....:::..: sld_h
        // ................::..:110010...::
        // Reko: a decoder for V850 instruction 43CE at address 001020F6 has not been implemented.
        [Test]
        public void V850Dis_43CE()
        {
            AssertCode("@@@", "43CE");
        }

        // ...................:.100110:.::.
        // ...................:.:..::.:.::. sst_h
        // ..................:.:110010.:::.
        // Reko: a decoder for V850 instruction 4E2E at address 001020FA has not been implemented.
        [Test]
        public void V850Dis_4E2E()
        {
            AssertCode("@@@", "4E2E");
        }

        // ..................:::110001:::::
        // Reko: a decoder for V850 instruction 3F3E at address 001020FC has not been implemented.
        [Test]
        public void V850Dis_3F3E()
        {
            AssertCode("@@@", "3F3E");
        }

        // .....................000100:::.:
        // Reko: a decoder for V850 instruction 9D00 at address 001020FE has not been implemented.
        [Test]
        public void V850Dis_9D00()
        {
            AssertCode("@@@", "9D00");
        }

        // .................::::101001.:..:
        // Reko: a decoder for V850 instruction 297D at address 00102100 has not been implemented.
        [Test]
        public void V850Dis_297D()
        {
            AssertCode("@@@", "297D");
        }

        // ................:.:.:100101..::.
        // ................:.:.::..:.:..::. sst_h
        // ..................:.:011010.::::
        // ..................:.:.::.:..:::: sld_b
        // .....................101110:::::
        // Reko: a decoder for V850 instruction DF05 at address 00102106 has not been implemented.
        [Test]
        public void V850Dis_DF05()
        {
            AssertCode("@@@", "DF05");
        }

        // ................:::..011100::..:
        // ................:::...:::..::..: sst_b
        // ................:.:..101101:....
        // ..................:::010101:.:::
        // Reko: a decoder for V850 instruction B73A at address 0010210C has not been implemented.
        [Test]
        public void V850Dis_B73A()
        {
            AssertCode("@@@", "B73A");
        }

        // ................:::.:100100:....
        // ................:::.::..:..:.... sst_h
        // ...................::101011:.:::
        // Reko: a decoder for V850 instruction 771D at address 00102110 has not been implemented.
        [Test]
        public void V850Dis_771D()
        {
            AssertCode("@@@", "771D");
        }

        // ..................:.:110111.::::
        // Reko: a decoder for V850 instruction EF2E at address 00102112 has not been implemented.
        [Test]
        public void V850Dis_EF2E()
        {
            AssertCode("@@@", "EF2E");
        }

        // .....................101101:....
        // Reko: a decoder for V850 instruction B005 at address 00102114 has not been implemented.
        [Test]
        public void V850Dis_B005()
        {
            AssertCode("@@@", "B005");
        }

        // ................::.::111000::::.
        // Reko: a decoder for V850 instruction 1EDF at address 00102116 has not been implemented.
        [Test]
        public void V850Dis_1EDF()
        {
            AssertCode("@@@", "1EDF");
        }

        // ................:..::101111:::::
        // Reko: a decoder for V850 instruction FF9D at address 00102118 has not been implemented.
        [Test]
        public void V850Dis_FF9D()
        {
            AssertCode("@@@", "FF9D");
        }

        // ................:.:::110011:.:.:
        // Reko: a decoder for V850 instruction 75BE at address 0010211A has not been implemented.
        [Test]
        public void V850Dis_75BE()
        {
            AssertCode("@@@", "75BE");
        }

        // ................:::..010000..:::
        // ................:::...:....00111
        // ................:::...:......::: mov
        // ................::..:011010:..::
        // ................::..:.::.:.:..:: sld_b
        // ................:::.:001000:..:.
        // ................:::.:..:...:..:. or
        // ................:::::101110::...
        // Reko: a decoder for V850 instruction D8FD at address 00102122 has not been implemented.
        [Test]
        public void V850Dis_D8FD()
        {
            AssertCode("@@@", "D8FD");
        }

        // ................:.:.:100001::::.
        // ................:.:.::....:::::. sld_h
        // .................:...000010...:.
        // ................01000....:....:.
        // .................:.......:.00010
        // .................:.......:....:. divh
        // .................:..:100011:....
        // .................:..::...:::.... sld_h
        // .................:.:.101110:::..
        // Reko: a decoder for V850 instruction DC55 at address 0010212A has not been implemented.
        [Test]
        public void V850Dis_DC55()
        {
            AssertCode("@@@", "DC55");
        }

        // ................:..::101011..:..
        // Reko: a decoder for V850 instruction 649D at address 0010212C has not been implemented.
        [Test]
        public void V850Dis_649D()
        {
            AssertCode("@@@", "649D");
        }

        // ................:..::011000:.:::
        // ................:..::.::...:.::: sld_b
        // .................::..010100..:..
        // Reko: a decoder for V850 instruction 8462 at address 00102130 has not been implemented.
        [Test]
        public void V850Dis_8462()
        {
            AssertCode("@@@", "8462");
        }

        // ..................:..001101:.:::
        // Reko: a decoder for V850 instruction B721 at address 00102132 has not been implemented.
        [Test]
        public void V850Dis_B721()
        {
            AssertCode("@@@", "B721");
        }

        // .................::.:110010.:::.
        // Reko: a decoder for V850 instruction 4E6E at address 00102134 has not been implemented.
        [Test]
        public void V850Dis_4E6E()
        {
            AssertCode("@@@", "4E6E");
        }

        // ..................:..011010.::::
        // ..................:...::.:..:::: sld_b
        // ................::...111010:::::
        // Reko: a decoder for V850 instruction 5FC7 at address 00102138 has not been implemented.
        [Test]
        public void V850Dis_5FC7()
        {
            AssertCode("@@@", "5FC7");
        }

        // ................:.::.001101.::.:
        // Reko: a decoder for V850 instruction ADB1 at address 0010213A has not been implemented.
        [Test]
        public void V850Dis_ADB1()
        {
            AssertCode("@@@", "ADB1");
        }

        // ..................:..001101:.::.
        // Reko: a decoder for V850 instruction B621 at address 0010213C has not been implemented.
        [Test]
        public void V850Dis_B621()
        {
            AssertCode("@@@", "B621");
        }

        // ................:.::.110111:::.:
        // Reko: a decoder for V850 instruction FDB6 at address 0010213E has not been implemented.
        [Test]
        public void V850Dis_FDB6()
        {
            AssertCode("@@@", "FDB6");
        }

        // ................:::..100100:....
        // ................:::..:..:..:.... sst_h
        // ................::..:101100:..:.
        // Reko: a decoder for V850 instruction 92CD at address 00102142 has not been implemented.
        [Test]
        public void V850Dis_92CD()
        {
            AssertCode("@@@", "92CD");
        }

        // ................:..:.011011...:.
        // ................:..:..::.::...:. sld_b
        // ................::..:011101.::.:
        // ................::..:.:::.:.::.: sst_b
        // ................:.:.:111110::.:.
        // Reko: a decoder for V850 instruction DAAF at address 00102148 has not been implemented.
        [Test]
        public void V850Dis_DAAF()
        {
            AssertCode("@@@", "DAAF");
        }

        // .................::..100101::::.
        // .................::..:..:.:::::. sst_h
        // ................::..:000001.:..:
        // ................::..:.....:.:..: not
        // .................::..001011::.:.
        // Reko: a decoder for V850 instruction 7A61 at address 0010214E has not been implemented.
        [Test]
        public void V850Dis_7A61()
        {
            AssertCode("@@@", "7A61");
        }

        // ................::.::010100..::.
        // Reko: a decoder for V850 instruction 86DA at address 00102150 has not been implemented.
        [Test]
        public void V850Dis_86DA()
        {
            AssertCode("@@@", "86DA");
        }

        // ................:.:..101001::.:.
        // Reko: a decoder for V850 instruction 3AA5 at address 00102152 has not been implemented.
        [Test]
        public void V850Dis_3AA5()
        {
            AssertCode("@@@", "3AA5");
        }

        // ..................::.001001.:.::
        // Reko: a decoder for V850 instruction 2B31 at address 00102154 has not been implemented.
        [Test]
        public void V850Dis_2B31()
        {
            AssertCode("@@@", "2B31");
        }

        // ................:.:..011011:::..
        // ................:.:...::.:::::.. sld_b
        // ................:.::.100100::.:.
        // ................:.::.:..:..::.:. sst_h
        // .................:::.111100::::.
        // Reko: a decoder for V850 instruction 9E77 at address 0010215A has not been implemented.
        [Test]
        public void V850Dis_9E77()
        {
            AssertCode("@@@", "9E77");
        }

        // ................:..:.001111:::::
        // Reko: a decoder for V850 instruction FF91 at address 0010215C has not been implemented.
        [Test]
        public void V850Dis_FF91()
        {
            AssertCode("@@@", "FF91");
        }

        // .................:..:111100.:.:.
        // Reko: a decoder for V850 instruction 8A4F at address 0010215E has not been implemented.
        [Test]
        public void V850Dis_8A4F()
        {
            AssertCode("@@@", "8A4F");
        }

        // ..................:..101010.::::
        // .................::::111110..:.:
        // Reko: a decoder for V850 instruction C57F at address 00102162 has not been implemented.
        [Test]
        public void V850Dis_C57F()
        {
            AssertCode("@@@", "C57F");
        }

        // .................:..:001000.:.:.
        // .................:..:..:....:.:. or
        // ................::::.000001::...
        // ................::::......:::... not
        // ................:::.:100101:.:..
        // ................:::.::..:.::.:.. sst_h
        // ................:..::111010..:::
        // Reko: a decoder for V850 instruction 479F at address 0010216A has not been implemented.
        [Test]
        public void V850Dis_479F()
        {
            AssertCode("@@@", "479F");
        }

        // ................:::.:011101:....
        // ................:::.:.:::.::.... sst_b
        // ................:..:.110111::.:.
        // Reko: a decoder for V850 instruction FA96 at address 0010216E has not been implemented.
        [Test]
        public void V850Dis_FA96()
        {
            AssertCode("@@@", "FA96");
        }

        // .................:..:100010.::..
        // .................:..::...:..::.. sld_h
        // ................::.::100001.:.::
        // ................::.:::....:.:.:: sld_h
        // .................:.::000111..::.
        // Reko: a decoder for V850 instruction E658 at address 00102174 has not been implemented.
        [Test]
        public void V850Dis_E658()
        {
            AssertCode("@@@", "E658");
        }

        // .................:.::111100.:.:.
        // Reko: a decoder for V850 instruction 8A5F at address 00102176 has not been implemented.
        [Test]
        public void V850Dis_8A5F()
        {
            AssertCode("@@@", "8A5F");
        }

        // ................:::.:110011..:::
        // Reko: a decoder for V850 instruction 67EE at address 00102178 has not been implemented.
        [Test]
        public void V850Dis_67EE()
        {
            AssertCode("@@@", "67EE");
        }

        // ................:.:.:101001:..::
        // Reko: a decoder for V850 instruction 33AD at address 0010217A has not been implemented.
        [Test]
        public void V850Dis_33AD()
        {
            AssertCode("@@@", "33AD");
        }

        // ..................::.110111:.:.:
        // Reko: a decoder for V850 instruction F536 at address 0010217C has not been implemented.
        [Test]
        public void V850Dis_F536()
        {
            AssertCode("@@@", "F536");
        }

        // .................:..:001110.::..
        // Reko: a decoder for V850 instruction CC49 at address 0010217E has not been implemented.
        [Test]
        public void V850Dis_CC49()
        {
            AssertCode("@@@", "CC49");
        }

        // .................::.:111001..:..
        // Reko: a decoder for V850 instruction 246F at address 00102180 has not been implemented.
        [Test]
        public void V850Dis_246F()
        {
            AssertCode("@@@", "246F");
        }

        // .....................011000:.:..
        // ......................::...:.:.. sld_b
        // ................:::::010000...::
        // ................:::::.:....00011
        // ................:::::.:.......:: mov
        // ..................:::101010:.::.
        // Reko: a decoder for V850 instruction 563D at address 00102186 has not been implemented.
        [Test]
        public void V850Dis_563D()
        {
            AssertCode("@@@", "563D");
        }

        // .................:..:001101....:
        // Reko: a decoder for V850 instruction A149 at address 00102188 has not been implemented.
        [Test]
        public void V850Dis_A149()
        {
            AssertCode("@@@", "A149");
        }

        // ..................:::111111:....
        // Reko: a decoder for V850 instruction F03F at address 0010218A has not been implemented.
        [Test]
        public void V850Dis_F03F()
        {
            AssertCode("@@@", "F03F");
        }

        // ................::.::101111.:.:.
        // Reko: a decoder for V850 instruction EADD at address 0010218C has not been implemented.
        [Test]
        public void V850Dis_EADD()
        {
            AssertCode("@@@", "EADD");
        }

        // ................:.:.:111111:..::
        // Reko: a decoder for V850 instruction F3AF at address 0010218E has not been implemented.
        [Test]
        public void V850Dis_F3AF()
        {
            AssertCode("@@@", "F3AF");
        }

        // ....................:000101::.:.
        // Reko: a decoder for V850 instruction BA08 at address 00102190 has not been implemented.
        [Test]
        public void V850Dis_BA08()
        {
            AssertCode("@@@", "BA08");
        }

        // ................:.::.010001.:.:.
        // Reko: a decoder for V850 instruction 2AB2 at address 00102192 has not been implemented.
        [Test]
        public void V850Dis_2AB2()
        {
            AssertCode("@@@", "2AB2");
        }

        // ................:.:.:101011..:.:
        // Reko: a decoder for V850 instruction 65AD at address 00102194 has not been implemented.
        [Test]
        public void V850Dis_65AD()
        {
            AssertCode("@@@", "65AD");
        }

        // ................::::.111011.:...
        // Reko: a decoder for V850 instruction 68F7 at address 00102196 has not been implemented.
        [Test]
        public void V850Dis_68F7()
        {
            AssertCode("@@@", "68F7");
        }

        // .................::..110100::.::
        // Reko: a decoder for V850 instruction 9B66 at address 00102198 has not been implemented.
        [Test]
        public void V850Dis_9B66()
        {
            AssertCode("@@@", "9B66");
        }

        // ...................::011000::...
        // ...................::.::...::... sld_b
        // ................:...:010000:.:.:
        // ................:...:.:....10101
        // ................:...:.:....:.:.: mov
        // .................::::110111::.::
        // Reko: a decoder for V850 instruction FB7E at address 0010219E has not been implemented.
        [Test]
        public void V850Dis_FB7E()
        {
            AssertCode("@@@", "FB7E");
        }

        // ................:::::101001.....
        // Reko: a decoder for V850 instruction 20FD at address 001021A0 has not been implemented.
        [Test]
        public void V850Dis_20FD()
        {
            AssertCode("@@@", "20FD");
        }

        // .................:...001011::.:.
        // Reko: a decoder for V850 instruction 7A41 at address 001021A2 has not been implemented.
        [Test]
        public void V850Dis_7A41()
        {
            AssertCode("@@@", "7A41");
        }

        // ................::::.111110:.::.
        // Reko: a decoder for V850 instruction D6F7 at address 001021A4 has not been implemented.
        [Test]
        public void V850Dis_D6F7()
        {
            AssertCode("@@@", "D6F7");
        }

        // .................:.:.001100:..:.
        // Reko: a decoder for V850 instruction 9251 at address 001021A6 has not been implemented.
        [Test]
        public void V850Dis_9251()
        {
            AssertCode("@@@", "9251");
        }

        // ................:::.:110111:.:.:
        // Reko: a decoder for V850 instruction F5EE at address 001021A8 has not been implemented.
        [Test]
        public void V850Dis_F5EE()
        {
            AssertCode("@@@", "F5EE");
        }

        // ................:..::100001::.:.
        // ................:..:::....:::.:. sld_h
        // .................:..:011011..:::
        // .................:..:.::.::..::: sld_b
        // ................:...:010011:.:.:
        // Reko: a decoder for V850 instruction 758A at address 001021AE has not been implemented.
        [Test]
        public void V850Dis_758A()
        {
            AssertCode("@@@", "758A");
        }

        // ................:....100111::.::
        // ................:....:..:::::.:: sst_h
        // ................:::.:101111:..::
        // Reko: a decoder for V850 instruction F3ED at address 001021B2 has not been implemented.
        [Test]
        public void V850Dis_F3ED()
        {
            AssertCode("@@@", "F3ED");
        }

        // ...................::001000.:.::
        // ...................::..:....:.:: or
        // .................:::.110111...::
        // Reko: a decoder for V850 instruction E376 at address 001021B6 has not been implemented.
        [Test]
        public void V850Dis_E376()
        {
            AssertCode("@@@", "E376");
        }

        // ...................::101000..::.
        // ...................:::.:.....::0 Sld.w/Sst.w
        // ...................:::.:.....::. sld_w
        // ................:..::101011..:::
        // Reko: a decoder for V850 instruction 679D at address 001021BA has not been implemented.
        [Test]
        public void V850Dis_679D()
        {
            AssertCode("@@@", "679D");
        }

        // ..................:::010111...:.
        // Reko: a decoder for V850 instruction E23A at address 001021BC has not been implemented.
        [Test]
        public void V850Dis_E23A()
        {
            AssertCode("@@@", "E23A");
        }

        // ...................:.110001..:::
        // .................:::.010010.:::.
        // Reko: a decoder for V850 instruction 4E72 at address 001021C0 has not been implemented.
        [Test]
        public void V850Dis_4E72()
        {
            AssertCode("@@@", "4E72");
        }

        // ................:::.:100000:::::
        // ................:::.::.....::::: sld_h
        // ................:.::.010011...:.
        // Reko: a decoder for V850 instruction 62B2 at address 001021C4 has not been implemented.
        [Test]
        public void V850Dis_62B2()
        {
            AssertCode("@@@", "62B2");
        }

        // ................::...010110::...
        // Reko: a decoder for V850 instruction D8C2 at address 001021C6 has not been implemented.
        [Test]
        public void V850Dis_D8C2()
        {
            AssertCode("@@@", "D8C2");
        }

        // ................::.::001011:.:.:
        // Reko: a decoder for V850 instruction 75D9 at address 001021C8 has not been implemented.
        [Test]
        public void V850Dis_75D9()
        {
            AssertCode("@@@", "75D9");
        }

        // .................:::.111101:::.:
        // Reko: a decoder for V850 instruction BD77 at address 001021CA has not been implemented.
        [Test]
        public void V850Dis_BD77()
        {
            AssertCode("@@@", "BD77");
        }

        // ................::..:011011:..::
        // ................::..:.::.:::..:: sld_b
        // .................:.:.000100.::::
        // Reko: a decoder for V850 instruction 8F50 at address 001021CE has not been implemented.
        [Test]
        public void V850Dis_8F50()
        {
            AssertCode("@@@", "8F50");
        }

        // ................:...:101111:...:
        // Reko: a decoder for V850 instruction F18D at address 001021D0 has not been implemented.
        [Test]
        public void V850Dis_F18D()
        {
            AssertCode("@@@", "F18D");
        }

        // ................:..:.000001.....
        // ................:..:......:..... not
        // ....................:101100:.::.
        // Reko: a decoder for V850 instruction 960D at address 001021D4 has not been implemented.
        [Test]
        public void V850Dis_960D()
        {
            AssertCode("@@@", "960D");
        }

        // .....................110011:..:.
        // Reko: a decoder for V850 instruction 7206 at address 001021D6 has not been implemented.
        [Test]
        public void V850Dis_7206()
        {
            AssertCode("@@@", "7206");
        }

        // .....................011000::.:.
        // ......................::...::.:. sld_b
        // ................:.:::000001::.:.
        // ................:.:::.....:::.:. not
        // ................::::.101100.::::
        // Reko: a decoder for V850 instruction 8FF5 at address 001021DC has not been implemented.
        [Test]
        public void V850Dis_8FF5()
        {
            AssertCode("@@@", "8FF5");
        }

        // ................:.:::010011.::::
        // Reko: a decoder for V850 instruction 6FBA at address 001021DE has not been implemented.
        [Test]
        public void V850Dis_6FBA()
        {
            AssertCode("@@@", "6FBA");
        }

        // ..................:..011110..::.
        // ..................:...::::...::. sst_b
        // ................::::.100010.:::.
        // ................::::.:...:..:::. sld_h
        // ................:...:000001.:.::
        // ................:...:.....:.:.:: not
        // .................:..:000001..:::
        // .................:..:.....:..::: not
        // ..................::.001101.::..
        // Reko: a decoder for V850 instruction AC31 at address 001021E8 has not been implemented.
        [Test]
        public void V850Dis_AC31()
        {
            AssertCode("@@@", "AC31");
        }

        // ..................::.010011:.:::
        // Reko: a decoder for V850 instruction 7732 at address 001021EA has not been implemented.
        [Test]
        public void V850Dis_7732()
        {
            AssertCode("@@@", "7732");
        }

        // .................::.:100010:::..
        // .................::.::...:.:::.. sld_h
        // ..................:::010000...::
        // ..................:::.:....00011
        // ..................:::.:.......:: mov
        // ...................:.111100.::.:
        // Reko: a decoder for V850 instruction 8D17 at address 001021F0 has not been implemented.
        [Test]
        public void V850Dis_8D17()
        {
            AssertCode("@@@", "8D17");
        }

        // .................:.:.100101.::..
        // .................:.:.:..:.:.::.. sst_h
        // ................:..:.110000::.:.
        // Reko: a decoder for V850 instruction 1A96 at address 001021F4 has not been implemented.
        [Test]
        public void V850Dis_1A96()
        {
            AssertCode("@@@", "1A96");
        }

        // .................:...001011:::::
        // Reko: a decoder for V850 instruction 7F41 at address 001021F6 has not been implemented.
        [Test]
        public void V850Dis_7F41()
        {
            AssertCode("@@@", "7F41");
        }

        // ...................::000000.....
        // ................00011...........
        // ...................::........... mov
        // .................:.::110110:..:.
        // Reko: a decoder for V850 instruction D25E at address 001021FA has not been implemented.
        [Test]
        public void V850Dis_D25E()
        {
            AssertCode("@@@", "D25E");
        }

        // ...................::111011:.:::
        // Reko: a decoder for V850 instruction 771F at address 001021FC has not been implemented.
        [Test]
        public void V850Dis_771F()
        {
            AssertCode("@@@", "771F");
        }

        // ................:....000001:..::
        // ................:.........::..:: not
        // .....................010011.:..:
        // Reko: a decoder for V850 instruction 6902 at address 00102200 has not been implemented.
        [Test]
        public void V850Dis_6902()
        {
            AssertCode("@@@", "6902");
        }

        // .................:..:111101:....
        // Reko: a decoder for V850 instruction B04F at address 00102202 has not been implemented.
        [Test]
        public void V850Dis_B04F()
        {
            AssertCode("@@@", "B04F");
        }

        // ................:..::011011:::.:
        // ................:..::.::.:::::.: sld_b
        // .................::.:000001..:..
        // .................::.:.....:..:.. not
        // ................::.:.111011..:.:
        // Reko: a decoder for V850 instruction 65D7 at address 00102208 has not been implemented.
        [Test]
        public void V850Dis_65D7()
        {
            AssertCode("@@@", "65D7");
        }

        // .................::..101111..:..
        // Reko: a decoder for V850 instruction E465 at address 0010220A has not been implemented.
        [Test]
        public void V850Dis_E465()
        {
            AssertCode("@@@", "E465");
        }

        // .................:...000100:.:.:
        // Reko: a decoder for V850 instruction 9540 at address 0010220C has not been implemented.
        [Test]
        public void V850Dis_9540()
        {
            AssertCode("@@@", "9540");
        }

        // ................:.:.:101101.:.:.
        // Reko: a decoder for V850 instruction AAAD at address 0010220E has not been implemented.
        [Test]
        public void V850Dis_AAAD()
        {
            AssertCode("@@@", "AAAD");
        }

        // ................:::::111000.....
        // Reko: a decoder for V850 instruction 00FF at address 00102210 has not been implemented.
        [Test]
        public void V850Dis_00FF()
        {
            AssertCode("@@@", "00FF");
        }

        // .................:::.111001.:.:.
        // Reko: a decoder for V850 instruction 2A77 at address 00102212 has not been implemented.
        [Test]
        public void V850Dis_2A77()
        {
            AssertCode("@@@", "2A77");
        }

        // ...................::010010.:..:
        // Reko: a decoder for V850 instruction 491A at address 00102214 has not been implemented.
        [Test]
        public void V850Dis_491A()
        {
            AssertCode("@@@", "491A");
        }

        // ..................:.:111101.....
        // Reko: a decoder for V850 instruction A02F at address 00102216 has not been implemented.
        [Test]
        public void V850Dis_A02F()
        {
            AssertCode("@@@", "A02F");
        }

        // ....................:100101...::
        // ....................::..:.:...:: sst_h
        // ...................:.010111:.:..
        // Reko: a decoder for V850 instruction F412 at address 0010221A has not been implemented.
        [Test]
        public void V850Dis_F412()
        {
            AssertCode("@@@", "F412");
        }

        // ................::..:001101:.::.
        // Reko: a decoder for V850 instruction B6C9 at address 0010221C has not been implemented.
        [Test]
        public void V850Dis_B6C9()
        {
            AssertCode("@@@", "B6C9");
        }

        // ................:...:100110:...:
        // ................:...::..::.:...: sst_h
        // ................::::.111111:::.:
        // Reko: a decoder for V850 instruction FDF7 at address 00102220 has not been implemented.
        [Test]
        public void V850Dis_FDF7()
        {
            AssertCode("@@@", "FDF7");
        }

        // .................:.::001110.:...
        // Reko: a decoder for V850 instruction C859 at address 00102222 has not been implemented.
        [Test]
        public void V850Dis_C859()
        {
            AssertCode("@@@", "C859");
        }

        // ................:.::.100101::.::
        // ................:.::.:..:.:::.:: sst_h
        // ................::::.010101:::.:
        // Reko: a decoder for V850 instruction BDF2 at address 00102226 has not been implemented.
        [Test]
        public void V850Dis_BDF2()
        {
            AssertCode("@@@", "BDF2");
        }

        // .................:...011111:..::
        // .................:....::::::..:: sst_b
        // .................:...011111.:::.
        // .................:....:::::.:::. sst_b
        // ...................::010101:..:.
        // Reko: a decoder for V850 instruction B21A at address 0010222C has not been implemented.
        [Test]
        public void V850Dis_B21A()
        {
            AssertCode("@@@", "B21A");
        }

        // ................::...000101.:.::
        // Reko: a decoder for V850 instruction ABC0 at address 0010222E has not been implemented.
        [Test]
        public void V850Dis_ABC0()
        {
            AssertCode("@@@", "ABC0");
        }

        // ..................:..100011.:::.
        // ..................:..:...::.:::. sld_h
        // .................::::000010::::.
        // ................01111....:.::::.
        // .................::::....:.11110
        // .................::::....:.::::. divh
        // ................:.:::100111:.:.:
        // ................:.::::..::::.:.: sst_h
        // .................:..:111001:.:::
        // Reko: a decoder for V850 instruction 374F at address 00102236 has not been implemented.
        [Test]
        public void V850Dis_374F()
        {
            AssertCode("@@@", "374F");
        }

        // ................:....001011...:.
        // Reko: a decoder for V850 instruction 6281 at address 00102238 has not been implemented.
        [Test]
        public void V850Dis_6281()
        {
            AssertCode("@@@", "6281");
        }

        // .................:::.000101.:.::
        // Reko: a decoder for V850 instruction AB70 at address 0010223A has not been implemented.
        [Test]
        public void V850Dis_AB70()
        {
            AssertCode("@@@", "AB70");
        }

        // ...................:.001101..:::
        // Reko: a decoder for V850 instruction A711 at address 0010223C has not been implemented.
        [Test]
        public void V850Dis_A711()
        {
            AssertCode("@@@", "A711");
        }

        // ................:..::100101:...:
        // ................:..:::..:.::...: sst_h
        // .................:.::000000..::.
        // ................01011........::.
        // .................:.::........::. mov
        // ................:..::110011.:...
        // Reko: a decoder for V850 instruction 689E at address 00102242 has not been implemented.
        [Test]
        public void V850Dis_689E()
        {
            AssertCode("@@@", "689E");
        }

        // .................:.:.000011:.:::
        // .................:..:011111:..:.
        // .................:..:.::::::..:. sst_b
        // ..................:::000010...:.
        // ................00111....:....:.
        // ..................:::....:.00010
        // ..................:::....:....:. divh
        // .....................101100...::
        // Reko: a decoder for V850 instruction 8305 at address 0010224A has not been implemented.
        [Test]
        public void V850Dis_8305()
        {
            AssertCode("@@@", "8305");
        }

        // .................:.::101001.:.::
        // ................:...:001011:::.:
        // Reko: a decoder for V850 instruction 7D89 at address 0010224E has not been implemented.
        [Test]
        public void V850Dis_7D89()
        {
            AssertCode("@@@", "7D89");
        }

        // ..................::.100011....:
        // ..................::.:...::....: sld_h
        // ................:..:.001101..:..
        // .................:.::001011.:.:.
        // Reko: a decoder for V850 instruction 6A59 at address 00102254 has not been implemented.
        [Test]
        public void V850Dis_6A59()
        {
            AssertCode("@@@", "6A59");
        }

        // .................:...001100:.:::
        // Reko: a decoder for V850 instruction 9741 at address 00102256 has not been implemented.
        [Test]
        public void V850Dis_9741()
        {
            AssertCode("@@@", "9741");
        }

        // .................::::001110.....
        // Reko: a decoder for V850 instruction C079 at address 00102258 has not been implemented.
        [Test]
        public void V850Dis_C079()
        {
            AssertCode("@@@", "C079");
        }

        // ..................:::001111:.:::
        // Reko: a decoder for V850 instruction F739 at address 0010225A has not been implemented.
        [Test]
        public void V850Dis_F739()
        {
            AssertCode("@@@", "F739");
        }

        // .................:..:100000.:..:
        // .................:..::......:..: sld_h
        // ................:::..010010.:.::
        // Reko: a decoder for V850 instruction 4BE2 at address 0010225E has not been implemented.
        [Test]
        public void V850Dis_4BE2()
        {
            AssertCode("@@@", "4BE2");
        }

        // .................:.:.010100:..:.
        // Reko: a decoder for V850 instruction 9252 at address 00102260 has not been implemented.
        [Test]
        public void V850Dis_9252()
        {
            AssertCode("@@@", "9252");
        }

        // .................:.::010001:::..
        // Reko: a decoder for V850 instruction 3C5A at address 00102262 has not been implemented.
        [Test]
        public void V850Dis_3C5A()
        {
            AssertCode("@@@", "3C5A");
        }

        // ....................:100101:..:.
        // ....................::..:.::..:. sst_h
        // ...................::101010.:...
        // Reko: a decoder for V850 instruction 481D at address 00102266 has not been implemented.
        [Test]
        public void V850Dis_481D()
        {
            AssertCode("@@@", "481D");
        }

        // ...................:.011001:...:
        // ...................:..::..::...: sld_b
        // ................:.:::100001:.:.:
        // ................:.::::....::.:.: sld_h
        // ..................::.010001::...
        // Reko: a decoder for V850 instruction 3832 at address 0010226C has not been implemented.
        [Test]
        public void V850Dis_3832()
        {
            AssertCode("@@@", "3832");
        }

        // .....................101001...::
        // Reko: a decoder for V850 instruction 2305 at address 0010226E has not been implemented.
        [Test]
        public void V850Dis_2305()
        {
            AssertCode("@@@", "2305");
        }

        // ..................:..001000..:..
        // ..................:....:.....:.. or
        // .................:::.011000.:::.
        // .................:::..::....:::. sld_b
        // ...................::111011.::..
        // Reko: a decoder for V850 instruction 6C1F at address 00102274 has not been implemented.
        [Test]
        public void V850Dis_6C1F()
        {
            AssertCode("@@@", "6C1F");
        }

        // ................:.:.:100001:..:.
        // ................:.:.::....::..:. sld_h
        // ..................:..110110.....
        // Reko: a decoder for V850 instruction C026 at address 00102278 has not been implemented.
        [Test]
        public void V850Dis_C026()
        {
            AssertCode("@@@", "C026");
        }

        // ................:::::110110::::.
        // Reko: a decoder for V850 instruction DEFE at address 0010227A has not been implemented.
        [Test]
        public void V850Dis_DEFE()
        {
            AssertCode("@@@", "DEFE");
        }

        // ................:.:::000010::..:
        // ................10111....:.::..:
        // ................:.:::....:.11001
        // ................:.:::....:.::..: divh
        // ................::::.101010...:.
        // Reko: a decoder for V850 instruction 42F5 at address 0010227E has not been implemented.
        [Test]
        public void V850Dis_42F5()
        {
            AssertCode("@@@", "42F5");
        }

        // ................:.:..001111.::.:
        // Reko: a decoder for V850 instruction EDA1 at address 00102280 has not been implemented.
        [Test]
        public void V850Dis_EDA1()
        {
            AssertCode("@@@", "EDA1");
        }

        // ................::.::001011:..::
        // ..................:..011001.....
        // ..................:...::..:..... sld_b
        // .................:.:.100110:.:::
        // .................:.:.:..::.:.::: sst_h
        // ................:....110111.:.::
        // Reko: a decoder for V850 instruction EB86 at address 00102288 has not been implemented.
        [Test]
        public void V850Dis_EB86()
        {
            AssertCode("@@@", "EB86");
        }

        // .................:.:.111011.....
        // Reko: a decoder for V850 instruction 6057 at address 0010228A has not been implemented.
        [Test]
        public void V850Dis_6057()
        {
            AssertCode("@@@", "6057");
        }

        // ................::.::010001...::
        // Reko: a decoder for V850 instruction 23DA at address 0010228C has not been implemented.
        [Test]
        public void V850Dis_23DA()
        {
            AssertCode("@@@", "23DA");
        }

        // ................:...:110000.:..:
        // Reko: a decoder for V850 instruction 098E at address 0010228E has not been implemented.
        [Test]
        public void V850Dis_098E()
        {
            AssertCode("@@@", "098E");
        }

        // ................::::.010010:.:.:
        // Reko: a decoder for V850 instruction 55F2 at address 00102290 has not been implemented.
        [Test]
        public void V850Dis_55F2()
        {
            AssertCode("@@@", "55F2");
        }

        // ................:::..111001:.:.:
        // Reko: a decoder for V850 instruction 35E7 at address 00102292 has not been implemented.
        [Test]
        public void V850Dis_35E7()
        {
            AssertCode("@@@", "35E7");
        }

        // ................::.::000001::::.
        // ................::.::.....:::::. not
        // .................:::.010011:.:.:
        // Reko: a decoder for V850 instruction 7572 at address 00102296 has not been implemented.
        [Test]
        public void V850Dis_7572()
        {
            AssertCode("@@@", "7572");
        }

        // ................:...:111000:::::
        // Reko: a decoder for V850 instruction 1F8F at address 00102298 has not been implemented.
        [Test]
        public void V850Dis_1F8F()
        {
            AssertCode("@@@", "1F8F");
        }

        // ................:...:011100::..:
        // ................:...:.:::..::..: sst_b
        // .................:..:010001.:::.
        // Reko: a decoder for V850 instruction 2E4A at address 0010229C has not been implemented.
        [Test]
        public void V850Dis_2E4A()
        {
            AssertCode("@@@", "2E4A");
        }

        // .................::::001110..:..
        // Reko: a decoder for V850 instruction C479 at address 0010229E has not been implemented.
        [Test]
        public void V850Dis_C479()
        {
            AssertCode("@@@", "C479");
        }

        // .................:...000110:....
        // Reko: a decoder for V850 instruction D040 at address 001022A0 has not been implemented.
        [Test]
        public void V850Dis_D040()
        {
            AssertCode("@@@", "D040");
        }

        // ................:..:.110110.::::
        // Reko: a decoder for V850 instruction CF96 at address 001022A2 has not been implemented.
        [Test]
        public void V850Dis_CF96()
        {
            AssertCode("@@@", "CF96");
        }

        // .................:..:010101:::::
        // Reko: a decoder for V850 instruction BF4A at address 001022A4 has not been implemented.
        [Test]
        public void V850Dis_BF4A()
        {
            AssertCode("@@@", "BF4A");
        }

        // ................:::..001111..:..
        // Reko: a decoder for V850 instruction E4E1 at address 001022A6 has not been implemented.
        [Test]
        public void V850Dis_E4E1()
        {
            AssertCode("@@@", "E4E1");
        }

        // ................:.:::010010.:.:.
        // Reko: a decoder for V850 instruction 4ABA at address 001022A8 has not been implemented.
        [Test]
        public void V850Dis_4ABA()
        {
            AssertCode("@@@", "4ABA");
        }

        // ................:::..101100..:::
        // ................::.::011101:::::
        // ................::.::.:::.:::::: sst_b
        // ................:::..110100.:..:
        // .................:.:.000000:::.:
        // ................01010......:::.:
        // .................:.:.......:::.: mov
        // .................::..110101.:...
        // Reko: a decoder for V850 instruction A866 at address 001022B2 has not been implemented.
        [Test]
        public void V850Dis_A866()
        {
            AssertCode("@@@", "A866");
        }

        // .................:.::011100...::
        // .................:.::.:::.....:: sst_b
        // ...................:.100101:.:::
        // ...................:.:..:.::.::: sst_h
        // .....................000001.::..
        // ..........................:.::.. not
        // ................:..:.000101::..:
        // Reko: a decoder for V850 instruction B990 at address 001022BA has not been implemented.
        [Test]
        public void V850Dis_B990()
        {
            AssertCode("@@@", "B990");
        }

        // .................:..:011100.:..:
        // .................:..:.:::...:..: sst_b
        // ................:.:..001001..:.:
        // Reko: a decoder for V850 instruction 25A1 at address 001022BE has not been implemented.
        [Test]
        public void V850Dis_25A1()
        {
            AssertCode("@@@", "25A1");
        }

        // ................:::..110110...:.
        // Reko: a decoder for V850 instruction C2E6 at address 001022C0 has not been implemented.
        [Test]
        public void V850Dis_C2E6()
        {
            AssertCode("@@@", "C2E6");
        }

        // ................:::..010100..::.
        // Reko: a decoder for V850 instruction 86E2 at address 001022C2 has not been implemented.
        [Test]
        public void V850Dis_86E2()
        {
            AssertCode("@@@", "86E2");
        }

        // .................:::.010000.:.::
        // .................:::..:....01011
        // .................:::..:.....:.:: mov
        // ....................:011110.:::.
        // ....................:.::::..:::. sst_b
        // .................:.:.100000.::.:
        // .................:.:.:......::.: sld_h
        // ................:....011100:::.:
        // ................:.....:::..:::.: sst_b
        // ................:...:111010:....
        // Reko: a decoder for V850 instruction 508F at address 001022CC has not been implemented.
        [Test]
        public void V850Dis_508F()
        {
            AssertCode("@@@", "508F");
        }

        // .................:...100101:..:.
        // .................:...:..:.::..:. sst_h
        // ................:.::.010000..::.
        // ................:.::..:....00110
        // ................:.::..:......::. mov
        // ................::.::110011.:::.
        // Reko: a decoder for V850 instruction 6EDE at address 001022D2 has not been implemented.
        [Test]
        public void V850Dis_6EDE()
        {
            AssertCode("@@@", "6EDE");
        }

        // .................:.::101101....:
        // .................:::.101111:.::.
        // Reko: a decoder for V850 instruction F675 at address 001022D6 has not been implemented.
        [Test]
        public void V850Dis_F675()
        {
            AssertCode("@@@", "F675");
        }

        // ................:.:::010100.:..:
        // Reko: a decoder for V850 instruction 89BA at address 001022D8 has not been implemented.
        [Test]
        public void V850Dis_89BA()
        {
            AssertCode("@@@", "89BA");
        }

        // ................:.:::110011.:.:.
        // Reko: a decoder for V850 instruction 6ABE at address 001022DA has not been implemented.
        [Test]
        public void V850Dis_6ABE()
        {
            AssertCode("@@@", "6ABE");
        }

        // .................:.:.100100:....
        // .................:.:.:..:..:.... sst_h
        // ................:::::110010:.:::
        // Reko: a decoder for V850 instruction 57FE at address 001022DE has not been implemented.
        [Test]
        public void V850Dis_57FE()
        {
            AssertCode("@@@", "57FE");
        }

        // ................:::..101100:.:..
        // Reko: a decoder for V850 instruction 94E5 at address 001022E0 has not been implemented.
        [Test]
        public void V850Dis_94E5()
        {
            AssertCode("@@@", "94E5");
        }

        // ................::.::000001...::
        // ................::.::.....:...:: not
        // ................:.:..110010:.:..
        // Reko: a decoder for V850 instruction 54A6 at address 001022E4 has not been implemented.
        [Test]
        public void V850Dis_54A6()
        {
            AssertCode("@@@", "54A6");
        }

        // ................:.:.:010110::.::
        // Reko: a decoder for V850 instruction DBAA at address 001022E6 has not been implemented.
        [Test]
        public void V850Dis_DBAA()
        {
            AssertCode("@@@", "DBAA");
        }

        // ................:..::101100....:
        // Reko: a decoder for V850 instruction 819D at address 001022E8 has not been implemented.
        [Test]
        public void V850Dis_819D()
        {
            AssertCode("@@@", "819D");
        }

        // ..................:.:110010::..:
        // Reko: a decoder for V850 instruction 592E at address 001022EA has not been implemented.
        [Test]
        public void V850Dis_592E()
        {
            AssertCode("@@@", "592E");
        }

        // ..................::.011101:::.:
        // ..................::..:::.::::.: sst_b
        // ................:..::100110..:..
        // ................:..:::..::...:.. sst_h
        // .................:::.101010.:::.
        // ................::.:.110100.:..:
        // .................:...111110:::.:
        // Reko: a decoder for V850 instruction DD47 at address 001022F4 has not been implemented.
        [Test]
        public void V850Dis_DD47()
        {
            AssertCode("@@@", "DD47");
        }

        // .................::..101000.....
        // .................::..:.:.......0 Sld.w/Sst.w
        // .................::..:.:........ sld_w
        // ...................:.001111:....
        // Reko: a decoder for V850 instruction F011 at address 001022F8 has not been implemented.
        [Test]
        public void V850Dis_F011()
        {
            AssertCode("@@@", "F011");
        }

        // .................:.:.000010::..:
        // ................01010....:.::..:
        // .................:.:.....:.11001
        // .................:.:.....:.::..: divh
        // ...................:.000000..:::
        // ................00010........:::
        // ...................:.........::: mov
        // ................:.:::001011::.:.
        // Reko: a decoder for V850 instruction 7AB9 at address 001022FE has not been implemented.
        [Test]
        public void V850Dis_7AB9()
        {
            AssertCode("@@@", "7AB9");
        }

        // ................:..::110111:::.:
        // Reko: a decoder for V850 instruction FD9E at address 00102300 has not been implemented.
        [Test]
        public void V850Dis_FD9E()
        {
            AssertCode("@@@", "FD9E");
        }

        // .................::.:011111.::::
        // .................::.:.:::::.:::: sst_b
        // ................::.::001011.:.::
        // Reko: a decoder for V850 instruction 6BD9 at address 00102304 has not been implemented.
        [Test]
        public void V850Dis_6BD9()
        {
            AssertCode("@@@", "6BD9");
        }

        // .................:.::111111:....
        // Reko: a decoder for V850 instruction F05F at address 00102306 has not been implemented.
        [Test]
        public void V850Dis_F05F()
        {
            AssertCode("@@@", "F05F");
        }

        // ................::...011110:.:::
        // ................::....::::.:.::: sst_b
        // .....................011010:::.:
        // ......................::.:.:::.: sld_b
        // ................::...111000..:..
        // Reko: a decoder for V850 instruction 04C7 at address 0010230C has not been implemented.
        [Test]
        public void V850Dis_04C7()
        {
            AssertCode("@@@", "04C7");
        }

        // .................:.:.110101:.:::
        // Reko: a decoder for V850 instruction B756 at address 0010230E has not been implemented.
        [Test]
        public void V850Dis_B756()
        {
            AssertCode("@@@", "B756");
        }

        // ................::..:101111:.::.
        // Reko: a decoder for V850 instruction F6CD at address 00102310 has not been implemented.
        [Test]
        public void V850Dis_F6CD()
        {
            AssertCode("@@@", "F6CD");
        }

        // ...................::011011:....
        // ...................::.::.:::.... sld_b
        // ................:....000110.:.:.
        // Reko: a decoder for V850 instruction CA80 at address 00102314 has not been implemented.
        [Test]
        public void V850Dis_CA80()
        {
            AssertCode("@@@", "CA80");
        }

        // .................:..:100001....:
        // .................:..::....:....: sld_h
        // ................:.:::110111..:..
        // Reko: a decoder for V850 instruction E4BE at address 00102318 has not been implemented.
        [Test]
        public void V850Dis_E4BE()
        {
            AssertCode("@@@", "E4BE");
        }

        // .................:...011111..:::
        // .................:....:::::..::: sst_b
        // ................:...:010010.::.:
        // Reko: a decoder for V850 instruction 4D8A at address 0010231C has not been implemented.
        [Test]
        public void V850Dis_4D8A()
        {
            AssertCode("@@@", "4D8A");
        }

        // .................:::.000101...::
        // Reko: a decoder for V850 instruction A370 at address 0010231E has not been implemented.
        [Test]
        public void V850Dis_A370()
        {
            AssertCode("@@@", "A370");
        }

        // ................:.:..000001...:.
        // ................:.:.......:...:. not
        // ...................::111001:....
        // Reko: a decoder for V850 instruction 301F at address 00102322 has not been implemented.
        [Test]
        public void V850Dis_301F()
        {
            AssertCode("@@@", "301F");
        }

        // .................:.::000010...::
        // ................01011....:....::
        // .................:.::....:.00011
        // .................:.::....:....:: divh
        // .................:::.100001:....
        // .................:::.:....::.... sld_h
        // ..................:.:010100..:..
        // Reko: a decoder for V850 instruction 842A at address 00102328 has not been implemented.
        [Test]
        public void V850Dis_842A()
        {
            AssertCode("@@@", "842A");
        }

        // ...................::001111:::::
        // Reko: a decoder for V850 instruction FF19 at address 0010232A has not been implemented.
        [Test]
        public void V850Dis_FF19()
        {
            AssertCode("@@@", "FF19");
        }

        // ................:::::101111.:..:
        // Reko: a decoder for V850 instruction E9FD at address 0010232C has not been implemented.
        [Test]
        public void V850Dis_E9FD()
        {
            AssertCode("@@@", "E9FD");
        }

        // ..................:.:001011.....
        // Reko: a decoder for V850 instruction 6029 at address 0010232E has not been implemented.
        [Test]
        public void V850Dis_6029()
        {
            AssertCode("@@@", "6029");
        }

        // .....................010010::..:
        // Reko: a decoder for V850 instruction 5902 at address 00102330 has not been implemented.
        [Test]
        public void V850Dis_5902()
        {
            AssertCode("@@@", "5902");
        }

        // ..................:::001010::.:.
        // Reko: a decoder for V850 instruction 5A39 at address 00102332 has not been implemented.
        [Test]
        public void V850Dis_5A39()
        {
            AssertCode("@@@", "5A39");
        }

        // .................:.::111100:::::
        // Reko: a decoder for V850 instruction 9F5F at address 00102334 has not been implemented.
        [Test]
        public void V850Dis_9F5F()
        {
            AssertCode("@@@", "9F5F");
        }

        // .................::::101111.:::.
        // Reko: a decoder for V850 instruction EE7D at address 00102336 has not been implemented.
        [Test]
        public void V850Dis_EE7D()
        {
            AssertCode("@@@", "EE7D");
        }

        // ................:.:..010011:::.:
        // Reko: a decoder for V850 instruction 7DA2 at address 00102338 has not been implemented.
        [Test]
        public void V850Dis_7DA2()
        {
            AssertCode("@@@", "7DA2");
        }

        // ................:.:.:101100..::.
        // Reko: a decoder for V850 instruction 86AD at address 0010233A has not been implemented.
        [Test]
        public void V850Dis_86AD()
        {
            AssertCode("@@@", "86AD");
        }

        // ................:.:.:100111:...:
        // ................:.:.::..::::...: sst_h
        // .................:..:101000:..:.
        // .................:..::.:...:..:0 Sld.w/Sst.w
        // .................:..::.:...:..:. sld_w
        // ................:::.:100001.....
        // ................:::.::....:..... sld_h
        // .................::..100111.....
        // .................::..:..:::..... sst_h
        // ................:..::000100:.::.
        // Reko: a decoder for V850 instruction 9698 at address 00102344 has not been implemented.
        [Test]
        public void V850Dis_9698()
        {
            AssertCode("@@@", "9698");
        }

        // ................:..::101000:..:.
        // ................:..:::.:...:..:0 Sld.w/Sst.w
        // ................:..:::.:...:..:. sld_w
        // ................:::::100100:::.:
        // ................::::::..:..:::.: sst_h
        // ................:.:..000100:.::.
        // Reko: a decoder for V850 instruction 96A0 at address 0010234A has not been implemented.
        [Test]
        public void V850Dis_96A0()
        {
            AssertCode("@@@", "96A0");
        }

        // .....................111100.....
        // Reko: a decoder for V850 instruction 8007 at address 0010234C has not been implemented.
        [Test]
        public void V850Dis_8007()
        {
            AssertCode("@@@", "8007");
        }

        // ................:::..111011...:.
        // Reko: a decoder for V850 instruction 62E7 at address 0010234E has not been implemented.
        [Test]
        public void V850Dis_62E7()
        {
            AssertCode("@@@", "62E7");
        }

        // ................:.:::110010:::.:
        // Reko: a decoder for V850 instruction 5DBE at address 00102350 has not been implemented.
        [Test]
        public void V850Dis_5DBE()
        {
            AssertCode("@@@", "5DBE");
        }

        // .................:..:010111.:..:
        // Reko: a decoder for V850 instruction E94A at address 00102352 has not been implemented.
        [Test]
        public void V850Dis_E94A()
        {
            AssertCode("@@@", "E94A");
        }

        // .................::.:001001:....
        // Reko: a decoder for V850 instruction 3069 at address 00102354 has not been implemented.
        [Test]
        public void V850Dis_3069()
        {
            AssertCode("@@@", "3069");
        }

        // ................::...011110:...:
        // ................::....::::.:...: sst_b
        // ................:.::.011101:...:
        // ................:.::..:::.::...: sst_b
        // ................:.:..000101....:
        // Reko: a decoder for V850 instruction A1A0 at address 0010235A has not been implemented.
        [Test]
        public void V850Dis_A1A0()
        {
            AssertCode("@@@", "A1A0");
        }

        // ................:....011110:..:.
        // ................:.....::::.:..:. sst_b
        // ................::.::000100...:.
        // Reko: a decoder for V850 instruction 82D8 at address 0010235E has not been implemented.
        [Test]
        public void V850Dis_82D8()
        {
            AssertCode("@@@", "82D8");
        }

        // ................:.::.010000:.:::
        // ................:.::..:....10111
        // ................:.::..:....:.::: mov
        // ................:::::101111::..:
        // Reko: a decoder for V850 instruction F9FD at address 00102362 has not been implemented.
        [Test]
        public void V850Dis_F9FD()
        {
            AssertCode("@@@", "F9FD");
        }

        // ................:::::100000:::..
        // ................::::::.....:::.. sld_h
        // ................::...001100:..:.
        // Reko: a decoder for V850 instruction 92C1 at address 00102366 has not been implemented.
        [Test]
        public void V850Dis_92C1()
        {
            AssertCode("@@@", "92C1");
        }

        // ................:.:::100111:....
        // ................:.::::..::::.... sst_h
        // ................:.:..011100:::..
        // ................:.:...:::..:::.. sst_b
        // .................:..:110110.:...
        // .................:::.110111:::..
        // Reko: a decoder for V850 instruction FC76 at address 0010236E has not been implemented.
        [Test]
        public void V850Dis_FC76()
        {
            AssertCode("@@@", "FC76");
        }

        // ................:..::110001:::::
        // Reko: a decoder for V850 instruction 3F9E at address 00102370 has not been implemented.
        [Test]
        public void V850Dis_3F9E()
        {
            AssertCode("@@@", "3F9E");
        }

        // ..................::.011010.::::
        // ..................::..::.:..:::: sld_b
        // ................::...111110...::
        // Reko: a decoder for V850 instruction C3C7 at address 00102374 has not been implemented.
        [Test]
        public void V850Dis_C3C7()
        {
            AssertCode("@@@", "C3C7");
        }

        // ................:.::.111000:::..
        // Reko: a decoder for V850 instruction 1CB7 at address 00102376 has not been implemented.
        [Test]
        public void V850Dis_1CB7()
        {
            AssertCode("@@@", "1CB7");
        }

        // ...................::101000::.::
        // ...................:::.:...::.:1 Sld.w/Sst.w
        // ...................:::.:...::.:: sst_w
        // ................:::..010101:..:.
        // Reko: a decoder for V850 instruction B2E2 at address 0010237A has not been implemented.
        [Test]
        public void V850Dis_B2E2()
        {
            AssertCode("@@@", "B2E2");
        }

        // .................:::.010111:.:::
        // Reko: a decoder for V850 instruction F772 at address 0010237C has not been implemented.
        [Test]
        public void V850Dis_F772()
        {
            AssertCode("@@@", "F772");
        }

        // ...................::001110.:.::
        // Reko: a decoder for V850 instruction CB19 at address 0010237E has not been implemented.
        [Test]
        public void V850Dis_CB19()
        {
            AssertCode("@@@", "CB19");
        }

        // ................:.:::110011::.:.
        // Reko: a decoder for V850 instruction 7ABE at address 00102380 has not been implemented.
        [Test]
        public void V850Dis_7ABE()
        {
            AssertCode("@@@", "7ABE");
        }

        // ................::..:110100.:...
        // Reko: a decoder for V850 instruction 88CE at address 00102382 has not been implemented.
        [Test]
        public void V850Dis_88CE()
        {
            AssertCode("@@@", "88CE");
        }

        // .................::..101000.:::.
        // .................::..:.:....:::0 Sld.w/Sst.w
        // .................::..:.:....:::. sld_w
        // .................::..000110.:.::
        // Reko: a decoder for V850 instruction CB60 at address 00102386 has not been implemented.
        [Test]
        public void V850Dis_CB60()
        {
            AssertCode("@@@", "CB60");
        }

        // ..................:.:000001.::..
        // ..................:.:.....:.::.. not
        // .................:::.100010::.:.
        // .................:::.:...:.::.:. sld_h
        // ..................:.:101110.::..
        // Reko: a decoder for V850 instruction CC2D at address 0010238C has not been implemented.
        [Test]
        public void V850Dis_CC2D()
        {
            AssertCode("@@@", "CC2D");
        }

        // .................::..010111::.:.
        // Reko: a decoder for V850 instruction FA62 at address 0010238E has not been implemented.
        [Test]
        public void V850Dis_FA62()
        {
            AssertCode("@@@", "FA62");
        }

        // ..................:.:010101:.:::
        // Reko: a decoder for V850 instruction B72A at address 00102390 has not been implemented.
        [Test]
        public void V850Dis_B72A()
        {
            AssertCode("@@@", "B72A");
        }

        // ..................::.100011.....
        // ..................::.:...::..... sld_h
        // .................:..:111001:..::
        // .................:..:111000:.:..
        // Reko: a decoder for V850 instruction 144F at address 00102396 has not been implemented.
        [Test]
        public void V850Dis_144F()
        {
            AssertCode("@@@", "144F");
        }

        // .................:...001100:.:.:
        // Reko: a decoder for V850 instruction 9541 at address 00102398 has not been implemented.
        [Test]
        public void V850Dis_9541()
        {
            AssertCode("@@@", "9541");
        }

        // ................:::::111111...:.
        // Reko: a decoder for V850 instruction E2FF at address 0010239A has not been implemented.
        [Test]
        public void V850Dis_E2FF()
        {
            AssertCode("@@@", "E2FF");
        }

        // ................:.:.:111010.:..:
        // Reko: a decoder for V850 instruction 49AF at address 0010239C has not been implemented.
        [Test]
        public void V850Dis_49AF()
        {
            AssertCode("@@@", "49AF");
        }

        // .................::::101110.:.:.
        // Reko: a decoder for V850 instruction CA7D at address 0010239E has not been implemented.
        [Test]
        public void V850Dis_CA7D()
        {
            AssertCode("@@@", "CA7D");
        }

        // ................:....011111:...:
        // ................:.....::::::...: sst_b
        // ....................:010001.:..:
        // Reko: a decoder for V850 instruction 290A at address 001023A2 has not been implemented.
        [Test]
        public void V850Dis_290A()
        {
            AssertCode("@@@", "290A");
        }

        // ..................:.:101110..::.
        // Reko: a decoder for V850 instruction C62D at address 001023A4 has not been implemented.
        [Test]
        public void V850Dis_C62D()
        {
            AssertCode("@@@", "C62D");
        }

        // ................::.::010011.:...
        // Reko: a decoder for V850 instruction 68DA at address 001023A6 has not been implemented.
        [Test]
        public void V850Dis_68DA()
        {
            AssertCode("@@@", "68DA");
        }

        // ................:::.:110110:..:.
        // Reko: a decoder for V850 instruction D2EE at address 001023A8 has not been implemented.
        [Test]
        public void V850Dis_D2EE()
        {
            AssertCode("@@@", "D2EE");
        }

        // ................:..::010000..::.
        // ................:..::.:....00110
        // ................:..::.:......::. mov
        // ................:.:.:000011.::.:
        // Reko: a decoder for V850 instruction 6DA8 at address 001023AC has not been implemented.
        [Test]
        public void V850Dis_6DA8()
        {
            AssertCode("@@@", "6DA8");
        }

        // ................:::.:101111.:.:.
        // Reko: a decoder for V850 instruction EAED at address 001023AE has not been implemented.
        [Test]
        public void V850Dis_EAED()
        {
            AssertCode("@@@", "EAED");
        }

        // .................:.:.000001...:.
        // .................:.:......:...:. not
        // ................::..:100001.:.::
        // ................::..::....:.:.:: sld_h
        // ................:..:.110000:...:
        // Reko: a decoder for V850 instruction 1196 at address 001023B4 has not been implemented.
        [Test]
        public void V850Dis_1196()
        {
            AssertCode("@@@", "1196");
        }

        // ..................:.:011111..:.:
        // ..................:.:.:::::..:.: sst_b
        // ..................:::000101.:..:
        // Reko: a decoder for V850 instruction A938 at address 001023B8 has not been implemented.
        [Test]
        public void V850Dis_A938()
        {
            AssertCode("@@@", "A938");
        }

        // ................::..:100001::...
        // ................::..::....:::... sld_h
        // ................::..:100100...::
        // ................::..::..:.....:: sst_h
        // .................::::101000:.::.
        // .................:::::.:...:.::0 Sld.w/Sst.w
        // .................:::::.:...:.::. sld_w
        // ................::.::100010::::.
        // ................::.:::...:.::::. sld_h
        // .................:..:001100...:.
        // Reko: a decoder for V850 instruction 8249 at address 001023C2 has not been implemented.
        [Test]
        public void V850Dis_8249()
        {
            AssertCode("@@@", "8249");
        }

        // ................::::.000000..:..
        // ................11110........:..
        // ................::::.........:.. mov
        // ................:...:001100:::..
        // Reko: a decoder for V850 instruction 9C89 at address 001023C6 has not been implemented.
        [Test]
        public void V850Dis_9C89()
        {
            AssertCode("@@@", "9C89");
        }

        // ................:....110110...:.
        // Reko: a decoder for V850 instruction C286 at address 001023C8 has not been implemented.
        [Test]
        public void V850Dis_C286()
        {
            AssertCode("@@@", "C286");
        }

        // .................:...100011....:
        // .................:...:...::....: sld_h
        // .................::::010010.:...
        // Reko: a decoder for V850 instruction 487A at address 001023CC has not been implemented.
        [Test]
        public void V850Dis_487A()
        {
            AssertCode("@@@", "487A");
        }

        // ................:::.:101111...:.
        // Reko: a decoder for V850 instruction E2ED at address 001023CE has not been implemented.
        [Test]
        public void V850Dis_E2ED()
        {
            AssertCode("@@@", "E2ED");
        }

        // ................::::.100010:.:::
        // ................::::.:...:.:.::: sld_h
        // ................:::::001110:::.:
        // Reko: a decoder for V850 instruction DDF9 at address 001023D2 has not been implemented.
        [Test]
        public void V850Dis_DDF9()
        {
            AssertCode("@@@", "DDF9");
        }

        // ................:::::101100..:..
        // Reko: a decoder for V850 instruction 84FD at address 001023D4 has not been implemented.
        [Test]
        public void V850Dis_84FD()
        {
            AssertCode("@@@", "84FD");
        }

        // .................:.::010011.:.::
        // Reko: a decoder for V850 instruction 6B5A at address 001023D6 has not been implemented.
        [Test]
        public void V850Dis_6B5A()
        {
            AssertCode("@@@", "6B5A");
        }

        // ................:::::110100:::.:
        // Reko: a decoder for V850 instruction 9DFE at address 001023D8 has not been implemented.
        [Test]
        public void V850Dis_9DFE()
        {
            AssertCode("@@@", "9DFE");
        }

        // ................:...:101011.....
        // Reko: a decoder for V850 instruction 608D at address 001023DA has not been implemented.
        [Test]
        public void V850Dis_608D()
        {
            AssertCode("@@@", "608D");
        }

        // ................:..::011111:.:..
        // ................:..::.::::::.:.. sst_b
        // .....................110010:.:::
        // Reko: a decoder for V850 instruction 5706 at address 001023DE has not been implemented.
        [Test]
        public void V850Dis_5706()
        {
            AssertCode("@@@", "5706");
        }

        // ................:...:001110::...
        // Reko: a decoder for V850 instruction D889 at address 001023E0 has not been implemented.
        [Test]
        public void V850Dis_D889()
        {
            AssertCode("@@@", "D889");
        }

        // ................:..:.000001:::..
        // ................:..:......::::.. not
        // ................:.:..000001..:.:
        // ................:.:.......:..:.: not
        // ..................::.001111.:..:
        // Reko: a decoder for V850 instruction E931 at address 001023E6 has not been implemented.
        [Test]
        public void V850Dis_E931()
        {
            AssertCode("@@@", "E931");
        }

        // ................:.:..001101:..::
        // Reko: a decoder for V850 instruction B3A1 at address 001023E8 has not been implemented.
        [Test]
        public void V850Dis_B3A1()
        {
            AssertCode("@@@", "B3A1");
        }

        // ................:...:011000.:..:
        // ................:...:.::....:..: sld_b
        // ................:.:..001001:.:.:
        // Reko: a decoder for V850 instruction 35A1 at address 001023EC has not been implemented.
        [Test]
        public void V850Dis_35A1()
        {
            AssertCode("@@@", "35A1");
        }

        // ..................:.:111111...:.
        // Reko: a decoder for V850 instruction E22F at address 001023EE has not been implemented.
        [Test]
        public void V850Dis_E22F()
        {
            AssertCode("@@@", "E22F");
        }

        // .................:..:011010:.::.
        // .................:..:.::.:.:.::. sld_b
        // ................:...:110011:.:..
        // Reko: a decoder for V850 instruction 748E at address 001023F2 has not been implemented.
        [Test]
        public void V850Dis_748E()
        {
            AssertCode("@@@", "748E");
        }

        // ...................::100111.:::.
        // ...................:::..:::.:::. sst_h
        // .................::..000111::..:
        // Reko: a decoder for V850 instruction F960 at address 001023F6 has not been implemented.
        [Test]
        public void V850Dis_F960()
        {
            AssertCode("@@@", "F960");
        }

        // ..................:..111011:....
        // Reko: a decoder for V850 instruction 7027 at address 001023F8 has not been implemented.
        [Test]
        public void V850Dis_7027()
        {
            AssertCode("@@@", "7027");
        }

        // .....................110101.::..
        // Reko: a decoder for V850 instruction AC06 at address 001023FA has not been implemented.
        [Test]
        public void V850Dis_AC06()
        {
            AssertCode("@@@", "AC06");
        }

        // ....................:110100:....
        // Reko: a decoder for V850 instruction 900E at address 001023FC has not been implemented.
        [Test]
        public void V850Dis_900E()
        {
            AssertCode("@@@", "900E");
        }

        // ..................:..110100:.:.:
        // Reko: a decoder for V850 instruction 9526 at address 001023FE has not been implemented.
        [Test]
        public void V850Dis_9526()
        {
            AssertCode("@@@", "9526");
        }

        // .................:.::010001.:::.
        // Reko: a decoder for V850 instruction 2E5A at address 00102400 has not been implemented.
        [Test]
        public void V850Dis_2E5A()
        {
            AssertCode("@@@", "2E5A");
        }

        // ................:.:::111011.::..
        // Reko: a decoder for V850 instruction 6CBF at address 00102402 has not been implemented.
        [Test]
        public void V850Dis_6CBF()
        {
            AssertCode("@@@", "6CBF");
        }

        // .................:.:.001001:::.:
        // ................:.::.110110..:::
        // Reko: a decoder for V850 instruction C7B6 at address 00102406 has not been implemented.
        [Test]
        public void V850Dis_C7B6()
        {
            AssertCode("@@@", "C7B6");
        }

        // ................:.:.:100000.:.:.
        // ................:.:.::......:.:. sld_h
        // ................:::..010010..::.
        // Reko: a decoder for V850 instruction 46E2 at address 0010240A has not been implemented.
        [Test]
        public void V850Dis_46E2()
        {
            AssertCode("@@@", "46E2");
        }

        // ................:::..000111:..::
        // Reko: a decoder for V850 instruction F3E0 at address 0010240C has not been implemented.
        [Test]
        public void V850Dis_F3E0()
        {
            AssertCode("@@@", "F3E0");
        }

        // ................:::::100001..:.:
        // ................::::::....:..:.: sld_h
        // ..................::.000000::.::
        // ................00110......::.::
        // ..................::.......::.:: mov
        // ................:.:.:001001:.:::
        // Reko: a decoder for V850 instruction 37A9 at address 00102412 has not been implemented.
        [Test]
        public void V850Dis_37A9()
        {
            AssertCode("@@@", "37A9");
        }

        // ................::..:000001..::.
        // ................::..:.....:..::. not
        // ................:.:::011000::...
        // ................:.:::.::...::... sld_b
        // ................::.::011100:....
        // ................::.::.:::..:.... sst_b
        // ................:::::110000:::::
        // Reko: a decoder for V850 instruction 1FFE at address 0010241A has not been implemented.
        [Test]
        public void V850Dis_1FFE()
        {
            AssertCode("@@@", "1FFE");
        }

        // ................:::..011111:.:..
        // ................:::...::::::.:.. sst_b
        // ................:.:..101101.....
        // Reko: a decoder for V850 instruction A0A5 at address 0010241E has not been implemented.
        [Test]
        public void V850Dis_A0A5()
        {
            AssertCode("@@@", "A0A5");
        }

        // ................:::..011000.::..
        // ................:::...::....::.. sld_b
        // ................::.::010010:::..
        // Reko: a decoder for V850 instruction 5CDA at address 00102422 has not been implemented.
        [Test]
        public void V850Dis_5CDA()
        {
            AssertCode("@@@", "5CDA");
        }

        // ..................:..011001:.:..
        // ..................:...::..::.:.. sld_b
        // .....................100111:..:.
        // .....................:..::::..:. sst_h
        // ................:.:::110100..:..
        // Reko: a decoder for V850 instruction 84BE at address 00102428 has not been implemented.
        [Test]
        public void V850Dis_84BE()
        {
            AssertCode("@@@", "84BE");
        }

        // .................:...010100...::
        // Reko: a decoder for V850 instruction 8342 at address 0010242A has not been implemented.
        [Test]
        public void V850Dis_8342()
        {
            AssertCode("@@@", "8342");
        }

        // ...................:.110110:.::.
        // Reko: a decoder for V850 instruction D616 at address 0010242C has not been implemented.
        [Test]
        public void V850Dis_D616()
        {
            AssertCode("@@@", "D616");
        }

        // ................:..:.000011.::.:
        // Reko: a decoder for V850 instruction 6D90 at address 0010242E has not been implemented.
        [Test]
        public void V850Dis_6D90()
        {
            AssertCode("@@@", "6D90");
        }

        // ................:.:.:111000...:.
        // Reko: a decoder for V850 instruction 02AF at address 00102430 has not been implemented.
        [Test]
        public void V850Dis_02AF()
        {
            AssertCode("@@@", "02AF");
        }

        // .................:.::111111.:.:.
        // Reko: a decoder for V850 instruction EA5F at address 00102432 has not been implemented.
        [Test]
        public void V850Dis_EA5F()
        {
            AssertCode("@@@", "EA5F");
        }

        // ................:::.:011110:.:::
        // ................:::.:.::::.:.::: sst_b
        // .................::..110111:::::
        // Reko: a decoder for V850 instruction FF66 at address 00102436 has not been implemented.
        [Test]
        public void V850Dis_FF66()
        {
            AssertCode("@@@", "FF66");
        }

        // ................:.::.001010...:.
        // Reko: a decoder for V850 instruction 42B1 at address 00102438 has not been implemented.
        [Test]
        public void V850Dis_42B1()
        {
            AssertCode("@@@", "42B1");
        }

        // .................::..001001.:::.
        // Reko: a decoder for V850 instruction 2E61 at address 0010243A has not been implemented.
        [Test]
        public void V850Dis_2E61()
        {
            AssertCode("@@@", "2E61");
        }

        // ................::..:001001:...:
        // Reko: a decoder for V850 instruction 31C9 at address 0010243C has not been implemented.
        [Test]
        public void V850Dis_31C9()
        {
            AssertCode("@@@", "31C9");
        }

        // ..................:..111110...:.
        // Reko: a decoder for V850 instruction C227 at address 0010243E has not been implemented.
        [Test]
        public void V850Dis_C227()
        {
            AssertCode("@@@", "C227");
        }

        // ................:.:..001000.:.::
        // ................:.:....:....:.:: or
        // ...................:.000110:.::.
        // Reko: a decoder for V850 instruction D610 at address 00102442 has not been implemented.
        [Test]
        public void V850Dis_D610()
        {
            AssertCode("@@@", "D610");
        }

        // .................:..:010100...::
        // Reko: a decoder for V850 instruction 834A at address 00102444 has not been implemented.
        [Test]
        public void V850Dis_834A()
        {
            AssertCode("@@@", "834A");
        }

        // ................::::.111110..:::
        // Reko: a decoder for V850 instruction C7F7 at address 00102446 has not been implemented.
        [Test]
        public void V850Dis_C7F7()
        {
            AssertCode("@@@", "C7F7");
        }

        // ..................::.010001::::.
        // Reko: a decoder for V850 instruction 3E32 at address 00102448 has not been implemented.
        [Test]
        public void V850Dis_3E32()
        {
            AssertCode("@@@", "3E32");
        }

        // .................::.:000001..:.:
        // .................::.:.....:..:.: not
        // ................::.:.101010..:..
        // Reko: a decoder for V850 instruction 44D5 at address 0010244C has not been implemented.
        [Test]
        public void V850Dis_44D5()
        {
            AssertCode("@@@", "44D5");
        }

        // .................::::001111..:..
        // Reko: a decoder for V850 instruction E479 at address 0010244E has not been implemented.
        [Test]
        public void V850Dis_E479()
        {
            AssertCode("@@@", "E479");
        }

        // ................:...:111011.:::.
        // Reko: a decoder for V850 instruction 6E8F at address 00102450 has not been implemented.
        [Test]
        public void V850Dis_6E8F()
        {
            AssertCode("@@@", "6E8F");
        }

        // .................:...100111::.::
        // .................:...:..:::::.:: sst_h
        // .................:...001111::...
        // ..................:..000110.::.:
        // Reko: a decoder for V850 instruction CD20 at address 00102456 has not been implemented.
        [Test]
        public void V850Dis_CD20()
        {
            AssertCode("@@@", "CD20");
        }

        // ................::::.110111...::
        // Reko: a decoder for V850 instruction E3F6 at address 00102458 has not been implemented.
        [Test]
        public void V850Dis_E3F6()
        {
            AssertCode("@@@", "E3F6");
        }

        // ................:....010100::...
        // Reko: a decoder for V850 instruction 9882 at address 0010245A has not been implemented.
        [Test]
        public void V850Dis_9882()
        {
            AssertCode("@@@", "9882");
        }

        // ................:..:.000111:.:..
        // Reko: a decoder for V850 instruction F490 at address 0010245C has not been implemented.
        [Test]
        public void V850Dis_F490()
        {
            AssertCode("@@@", "F490");
        }

        // ................:.:::010110:..::
        // Reko: a decoder for V850 instruction D3BA at address 0010245E has not been implemented.
        [Test]
        public void V850Dis_D3BA()
        {
            AssertCode("@@@", "D3BA");
        }

        // .................:.::100111::.::
        // .................:.:::..:::::.:: sst_h
        // ..................::.101001:.:::
        // Reko: a decoder for V850 instruction 3735 at address 00102462 has not been implemented.
        [Test]
        public void V850Dis_3735()
        {
            AssertCode("@@@", "3735");
        }

        // .................:.::100010.....
        // .................:.:::...:...... sld_h
        // ................:.:::000000.::.:
        // ................10111.......::.:
        // ................:.:::.......::.: mov
        // ................::::.010111..:::
        // ................:.:..100001....:
        // ................:.:..:....:....: sld_h
        // ................:..:.111110..::.
        // Reko: a decoder for V850 instruction C697 at address 0010246C has not been implemented.
        [Test]
        public void V850Dis_C697()
        {
            AssertCode("@@@", "C697");
        }

        // .................::.:100001...:.
        // .................::.::....:...:. sld_h
        // ................:.::.101110.::.:
        // Reko: a decoder for V850 instruction CDB5 at address 00102470 has not been implemented.
        [Test]
        public void V850Dis_CDB5()
        {
            AssertCode("@@@", "CDB5");
        }

        // ................:.:..010111:..:.
        // Reko: a decoder for V850 instruction F2A2 at address 00102472 has not been implemented.
        [Test]
        public void V850Dis_F2A2()
        {
            AssertCode("@@@", "F2A2");
        }

        // .................::::101110.:::.
        // Reko: a decoder for V850 instruction CE7D at address 00102474 has not been implemented.
        [Test]
        public void V850Dis_CE7D()
        {
            AssertCode("@@@", "CE7D");
        }

        // ................::..:001001.::::
        // Reko: a decoder for V850 instruction 2FC9 at address 00102476 has not been implemented.
        [Test]
        public void V850Dis_2FC9()
        {
            AssertCode("@@@", "2FC9");
        }

        // ................:.::.101110:..::
        // Reko: a decoder for V850 instruction D3B5 at address 00102478 has not been implemented.
        [Test]
        public void V850Dis_D3B5()
        {
            AssertCode("@@@", "D3B5");
        }

        // ................:...:101001.::.:
        // Reko: a decoder for V850 instruction 2D8D at address 0010247A has not been implemented.
        [Test]
        public void V850Dis_2D8D()
        {
            AssertCode("@@@", "2D8D");
        }

        // .................:...101101:...:
        // Reko: a decoder for V850 instruction B145 at address 0010247C has not been implemented.
        [Test]
        public void V850Dis_B145()
        {
            AssertCode("@@@", "B145");
        }

        // ................:.:.:101000...::
        // ................:.:.::.:......:1 Sld.w/Sst.w
        // ................:.:.::.:......:: sst_w
        // .................::.:010010:::::
        // Reko: a decoder for V850 instruction 5F6A at address 00102480 has not been implemented.
        [Test]
        public void V850Dis_5F6A()
        {
            AssertCode("@@@", "5F6A");
        }

        // ................:::.:000011.::.:
        // Reko: a decoder for V850 instruction 6DE8 at address 00102482 has not been implemented.
        [Test]
        public void V850Dis_6DE8()
        {
            AssertCode("@@@", "6DE8");
        }

        // ................:.:.:110100::::.
        // Reko: a decoder for V850 instruction 9EAE at address 00102484 has not been implemented.
        [Test]
        public void V850Dis_9EAE()
        {
            AssertCode("@@@", "9EAE");
        }

        // ...................:.010001::..:
        // Reko: a decoder for V850 instruction 3912 at address 00102486 has not been implemented.
        [Test]
        public void V850Dis_3912()
        {
            AssertCode("@@@", "3912");
        }

        // .................:...011100....:
        // .................:....:::......: sst_b
        // .....................101010:::..
        // Reko: a decoder for V850 instruction 5C05 at address 0010248A has not been implemented.
        [Test]
        public void V850Dis_5C05()
        {
            AssertCode("@@@", "5C05");
        }

        // ..................:.:001001.....
        // Reko: a decoder for V850 instruction 2029 at address 0010248C has not been implemented.
        [Test]
        public void V850Dis_2029()
        {
            AssertCode("@@@", "2029");
        }

        // .................:..:011010::.:.
        // .................:..:.::.:.::.:. sld_b
        // ..................:.:011110:..::
        // ..................:.:.::::.:..:: sst_b
        // ..................:::111101:.:.:
        // Reko: a decoder for V850 instruction B53F at address 00102492 has not been implemented.
        [Test]
        public void V850Dis_B53F()
        {
            AssertCode("@@@", "B53F");
        }

        // ..................::.001100:::.:
        // Reko: a decoder for V850 instruction 9D31 at address 00102494 has not been implemented.
        [Test]
        public void V850Dis_9D31()
        {
            AssertCode("@@@", "9D31");
        }

        // .................::::110111.::..
        // Reko: a decoder for V850 instruction EC7E at address 00102496 has not been implemented.
        [Test]
        public void V850Dis_EC7E()
        {
            AssertCode("@@@", "EC7E");
        }

        // .................::.:101001.::..
        // Reko: a decoder for V850 instruction 2C6D at address 00102498 has not been implemented.
        [Test]
        public void V850Dis_2C6D()
        {
            AssertCode("@@@", "2C6D");
        }

        // ................:...:011011...:.
        // ................:...:.::.::...:. sld_b
        // ................:....000001.::::
        // ................:.........:.:::: not
        // ................:.:..010000..::.
        // ................:.:...:....00110
        // ................:.:...:......::. mov
        // .................:::.011111.::::
        // .................:::..:::::.:::: sst_b
        // .................:.::011010..:.:
        // .................:.::.::.:...:.: sld_b
        // ..................:::010001.:..:
        // Reko: a decoder for V850 instruction 293A at address 001024A4 has not been implemented.
        [Test]
        public void V850Dis_293A()
        {
            AssertCode("@@@", "293A");
        }

        // ...................::110110.:::.
        // Reko: a decoder for V850 instruction CE1E at address 001024A6 has not been implemented.
        [Test]
        public void V850Dis_CE1E()
        {
            AssertCode("@@@", "CE1E");
        }

        // ................:::..000011:::..
        // Reko: a decoder for V850 instruction 7CE0 at address 001024A8 has not been implemented.
        [Test]
        public void V850Dis_7CE0()
        {
            AssertCode("@@@", "7CE0");
        }

        // ................:...:010001.....
        // ..................:.:010001....:
        // Reko: a decoder for V850 instruction 212A at address 001024AC has not been implemented.
        [Test]
        public void V850Dis_212A()
        {
            AssertCode("@@@", "212A");
        }

        // ................:.:.:001101.:..:
        // Reko: a decoder for V850 instruction A9A9 at address 001024AE has not been implemented.
        [Test]
        public void V850Dis_A9A9()
        {
            AssertCode("@@@", "A9A9");
        }

        // ................:::..001010::.::
        // Reko: a decoder for V850 instruction 5BE1 at address 001024B0 has not been implemented.
        [Test]
        public void V850Dis_5BE1()
        {
            AssertCode("@@@", "5BE1");
        }

        // ................:....101101::..:
        // Reko: a decoder for V850 instruction B985 at address 001024B2 has not been implemented.
        [Test]
        public void V850Dis_B985()
        {
            AssertCode("@@@", "B985");
        }

        // ................::...011100.::::
        // ................::....:::...:::: sst_b
        // ..................:.:101000:....
        // ..................:.::.:...:...0 Sld.w/Sst.w
        // ..................:.::.:...:.... sld_w
        // ................:::.:111011::::.
        // ..................::.000101::.::
        // Reko: a decoder for V850 instruction BB30 at address 001024BA has not been implemented.
        [Test]
        public void V850Dis_BB30()
        {
            AssertCode("@@@", "BB30");
        }

        // ................:.:.:101010.::..
        // Reko: a decoder for V850 instruction 4CAD at address 001024BC has not been implemented.
        [Test]
        public void V850Dis_4CAD()
        {
            AssertCode("@@@", "4CAD");
        }

        // .....................000111...:.
        // Reko: a decoder for V850 instruction E200 at address 001024BE has not been implemented.
        [Test]
        public void V850Dis_E200()
        {
            AssertCode("@@@", "E200");
        }

        // ................:.:::001001:::..
        // Reko: a decoder for V850 instruction 3CB9 at address 001024C0 has not been implemented.
        [Test]
        public void V850Dis_3CB9()
        {
            AssertCode("@@@", "3CB9");
        }

        // ................:.:.:101000:.:.:
        // ................:.:.::.:...:.:.1 Sld.w/Sst.w
        // ................:.:.::.:...:.:.: sst_w
        // ................:::::110111...::
        // ................:..::001001...:.
        // Reko: a decoder for V850 instruction 2299 at address 001024C6 has not been implemented.
        [Test]
        public void V850Dis_2299()
        {
            AssertCode("@@@", "2299");
        }

        // ................:..:.111010:::.:
        // Reko: a decoder for V850 instruction 5D97 at address 001024C8 has not been implemented.
        [Test]
        public void V850Dis_5D97()
        {
            AssertCode("@@@", "5D97");
        }

        // ................:.:::100110...::
        // ................:.::::..::....:: sst_h
        // .................::..001000:....
        // .................::....:...:.... or
        // .....................010010:::..
        // Reko: a decoder for V850 instruction 5C02 at address 001024CE has not been implemented.
        [Test]
        public void V850Dis_5C02()
        {
            AssertCode("@@@", "5C02");
        }

        // ................:.:::000110..:..
        // Reko: a decoder for V850 instruction C4B8 at address 001024D0 has not been implemented.
        [Test]
        public void V850Dis_C4B8()
        {
            AssertCode("@@@", "C4B8");
        }

        // .................:..:101001.::::
        // Reko: a decoder for V850 instruction 2F4D at address 001024D2 has not been implemented.
        [Test]
        public void V850Dis_2F4D()
        {
            AssertCode("@@@", "2F4D");
        }

        // ................:....000110..::.
        // Reko: a decoder for V850 instruction C680 at address 001024D4 has not been implemented.
        [Test]
        public void V850Dis_C680()
        {
            AssertCode("@@@", "C680");
        }

        // ..................:.:011000:..::
        // ..................:.:.::...:..:: sld_b
        // ...................:.111011...:.
        // Reko: a decoder for V850 instruction 6217 at address 001024D8 has not been implemented.
        [Test]
        public void V850Dis_6217()
        {
            AssertCode("@@@", "6217");
        }

        // ..................:::010110:::.:
        // Reko: a decoder for V850 instruction DD3A at address 001024DA has not been implemented.
        [Test]
        public void V850Dis_DD3A()
        {
            AssertCode("@@@", "DD3A");
        }

        // ................:..::101011::::.
        // Reko: a decoder for V850 instruction 7E9D at address 001024DC has not been implemented.
        [Test]
        public void V850Dis_7E9D()
        {
            AssertCode("@@@", "7E9D");
        }

        // ................::..:110110:..:.
        // Reko: a decoder for V850 instruction D2CE at address 001024DE has not been implemented.
        [Test]
        public void V850Dis_D2CE()
        {
            AssertCode("@@@", "D2CE");
        }

        // ...................::111111:::.:
        // Reko: a decoder for V850 instruction FD1F at address 001024E0 has not been implemented.
        [Test]
        public void V850Dis_FD1F()
        {
            AssertCode("@@@", "FD1F");
        }

        // .................::..111010.::.:
        // Reko: a decoder for V850 instruction 4D67 at address 001024E2 has not been implemented.
        [Test]
        public void V850Dis_4D67()
        {
            AssertCode("@@@", "4D67");
        }

        // ................:::::100000:.:..
        // ................::::::.....:.:.. sld_h
        // .................:.::101110..:.:
        // Reko: a decoder for V850 instruction C55D at address 001024E6 has not been implemented.
        [Test]
        public void V850Dis_C55D()
        {
            AssertCode("@@@", "C55D");
        }

        // ..................:..000101:::::
        // ................:::::000001.:...
        // ................:::::.....:.:... not
        // .................:.:.011000.....
        // .................:.:..::........ sld_b
        // ...................::100000:::.:
        // ...................:::.....:::.: sld_h
        // ................:.:::001100:..:.
        // Reko: a decoder for V850 instruction 92B9 at address 001024F0 has not been implemented.
        [Test]
        public void V850Dis_92B9()
        {
            AssertCode("@@@", "92B9");
        }

        // ................:..:.011011.::..
        // ................:..:..::.::.::.. sld_b
        // ................:..:.101011::::.
        // Reko: a decoder for V850 instruction 7E95 at address 001024F4 has not been implemented.
        [Test]
        public void V850Dis_7E95()
        {
            AssertCode("@@@", "7E95");
        }

        // ................:.:::100001::.:.
        // ................:.::::....:::.:. sld_h
        // ................:::.:010101..::.
        // Reko: a decoder for V850 instruction A6EA at address 001024F8 has not been implemented.
        [Test]
        public void V850Dis_A6EA()
        {
            AssertCode("@@@", "A6EA");
        }

        // ................::..:100010.:.:.
        // ................::..::...:..:.:. sld_h
        // ................:::..111001.....
        // Reko: a decoder for V850 instruction 20E7 at address 001024FC has not been implemented.
        [Test]
        public void V850Dis_20E7()
        {
            AssertCode("@@@", "20E7");
        }

        // ................:.:::111000::.:.
        // Reko: a decoder for V850 instruction 1ABF at address 001024FE has not been implemented.
        [Test]
        public void V850Dis_1ABF()
        {
            AssertCode("@@@", "1ABF");
        }

        // ................::::.101110..:..
        // Reko: a decoder for V850 instruction C4F5 at address 00102500 has not been implemented.
        [Test]
        public void V850Dis_C4F5()
        {
            AssertCode("@@@", "C4F5");
        }

        // ................::::.000101::::.
        // Reko: a decoder for V850 instruction BEF0 at address 00102502 has not been implemented.
        [Test]
        public void V850Dis_BEF0()
        {
            AssertCode("@@@", "BEF0");
        }

        // ................::::.101000:..::
        // ................::::.:.:...:..:1 Sld.w/Sst.w
        // ................::::.:.:...:..:: sst_w
        // ................::...111111.::.:
        // Reko: a decoder for V850 instruction EDC7 at address 00102506 has not been implemented.
        [Test]
        public void V850Dis_EDC7()
        {
            AssertCode("@@@", "EDC7");
        }

        // ................::.:.010111:..::
        // Reko: a decoder for V850 instruction F3D2 at address 00102508 has not been implemented.
        [Test]
        public void V850Dis_F3D2()
        {
            AssertCode("@@@", "F3D2");
        }

        // ................:.:..110100.:::.
        // Reko: a decoder for V850 instruction 8EA6 at address 0010250A has not been implemented.
        [Test]
        public void V850Dis_8EA6()
        {
            AssertCode("@@@", "8EA6");
        }

        // ...................::011010::...
        // ...................::.::.:.::... sld_b
        // ....................:110001.:.::
        // Reko: a decoder for V850 instruction 2B0E at address 0010250E has not been implemented.
        [Test]
        public void V850Dis_2B0E()
        {
            AssertCode("@@@", "2B0E");
        }

        // ................::...001111::.:.
        // Reko: a decoder for V850 instruction FAC1 at address 00102510 has not been implemented.
        [Test]
        public void V850Dis_FAC1()
        {
            AssertCode("@@@", "FAC1");
        }

        // ................::...101101.:...
        // Reko: a decoder for V850 instruction A8C5 at address 00102512 has not been implemented.
        [Test]
        public void V850Dis_A8C5()
        {
            AssertCode("@@@", "A8C5");
        }

        // ..................:::111001:...:
        // Reko: a decoder for V850 instruction 313F at address 00102514 has not been implemented.
        [Test]
        public void V850Dis_313F()
        {
            AssertCode("@@@", "313F");
        }

        // .................::.:000010.::::
        // ................01101....:..::::
        // .................::.:....:.01111
        // .................::.:....:..:::: divh
        // ................:..:.001111..:.:
        // Reko: a decoder for V850 instruction E591 at address 00102518 has not been implemented.
        [Test]
        public void V850Dis_E591()
        {
            AssertCode("@@@", "E591");
        }

        // ..................:.:010110....:
        // Reko: a decoder for V850 instruction C12A at address 0010251A has not been implemented.
        [Test]
        public void V850Dis_C12A()
        {
            AssertCode("@@@", "C12A");
        }

        // ................:::::001101:...:
        // Reko: a decoder for V850 instruction B1F9 at address 0010251C has not been implemented.
        [Test]
        public void V850Dis_B1F9()
        {
            AssertCode("@@@", "B1F9");
        }

        // ................::.::000001:::..
        // ................::.::.....::::.. not
        // .................::.:000000..:.:
        // ................01101........:.:
        // .................::.:........:.: mov
        // ....................:011001:..::
        // ....................:.::..::..:: sld_b
        // ..................:.:100100:.:..
        // ..................:.::..:..:.:.. sst_h
        // ................:..::100000.:...
        // ................:..:::......:... sld_h
        // ................:.:..100110.:.::
        // ................:.:..:..::..:.:: sst_h
        // ................:.:.:011101.....
        // ................:.:.:.:::.:..... sst_b
        // ................:..:.011000..:::
        // ................:..:..::.....::: sld_b
        // ................:...:010011..:..
        // Reko: a decoder for V850 instruction 648A at address 0010252E has not been implemented.
        [Test]
        public void V850Dis_648A()
        {
            AssertCode("@@@", "648A");
        }

        // ..................:::011101:::::
        // ..................:::.:::.:::::: sst_b
        // .................:.::111110:...:
        // Reko: a decoder for V850 instruction D15F at address 00102532 has not been implemented.
        [Test]
        public void V850Dis_D15F()
        {
            AssertCode("@@@", "D15F");
        }

        // .................:.:.100001::::.
        // .................:.:.:....:::::. sld_h
        // ................:....101100.:::.
        // Reko: a decoder for V850 instruction 8E85 at address 00102536 has not been implemented.
        [Test]
        public void V850Dis_8E85()
        {
            AssertCode("@@@", "8E85");
        }

        // .................:.:.110101..:.:
        // Reko: a decoder for V850 instruction A556 at address 00102538 has not been implemented.
        [Test]
        public void V850Dis_A556()
        {
            AssertCode("@@@", "A556");
        }

        // ................:....010000.:.::
        // ................:.....:....01011
        // ................:.....:.....:.:: mov
        // ................::...011001:.:..
        // ................::....::..::.:.. sld_b
        // ................:::::001000:.::.
        // ................:::::..:...:.::. or
        // ................:.::.101100:.:.:
        // Reko: a decoder for V850 instruction 95B5 at address 00102540 has not been implemented.
        [Test]
        public void V850Dis_95B5()
        {
            AssertCode("@@@", "95B5");
        }

        // ................:::.:111101.....
        // Reko: a decoder for V850 instruction A0EF at address 00102542 has not been implemented.
        [Test]
        public void V850Dis_A0EF()
        {
            AssertCode("@@@", "A0EF");
        }

        // ...................::111111.:...
        // Reko: a decoder for V850 instruction E81F at address 00102544 has not been implemented.
        [Test]
        public void V850Dis_E81F()
        {
            AssertCode("@@@", "E81F");
        }

        // ................::..:110011::.:.
        // Reko: a decoder for V850 instruction 7ACE at address 00102546 has not been implemented.
        [Test]
        public void V850Dis_7ACE()
        {
            AssertCode("@@@", "7ACE");
        }

        // ....................:100101::...
        // ....................::..:.:::... sst_h
        // ................:...:100111::.:.
        // ................:...::..:::::.:. sst_h
        // ................:.:..100100::::.
        // ................:.:..:..:..::::. sst_h
        // .................:.:.010011....:
        // Reko: a decoder for V850 instruction 6152 at address 0010254E has not been implemented.
        [Test]
        public void V850Dis_6152()
        {
            AssertCode("@@@", "6152");
        }

        // ..................::.110001.::.:
        // Reko: a decoder for V850 instruction 2D36 at address 00102550 has not been implemented.
        [Test]
        public void V850Dis_2D36()
        {
            AssertCode("@@@", "2D36");
        }

        // .................:::.101011.:.::
        // Reko: a decoder for V850 instruction 6B75 at address 00102552 has not been implemented.
        [Test]
        public void V850Dis_6B75()
        {
            AssertCode("@@@", "6B75");
        }

        // ................::.::101001..:::
        // Reko: a decoder for V850 instruction 27DD at address 00102554 has not been implemented.
        [Test]
        public void V850Dis_27DD()
        {
            AssertCode("@@@", "27DD");
        }

        // ................:.::.001100::..:
        // Reko: a decoder for V850 instruction 99B1 at address 00102556 has not been implemented.
        [Test]
        public void V850Dis_99B1()
        {
            AssertCode("@@@", "99B1");
        }

        // ................:.:.:101110::..:
        // Reko: a decoder for V850 instruction D9AD at address 00102558 has not been implemented.
        [Test]
        public void V850Dis_D9AD()
        {
            AssertCode("@@@", "D9AD");
        }

        // ..................:::110011..::.
        // Reko: a decoder for V850 instruction 663E at address 0010255A has not been implemented.
        [Test]
        public void V850Dis_663E()
        {
            AssertCode("@@@", "663E");
        }

        // ................:....110001....:
        // Reko: a decoder for V850 instruction 2186 at address 0010255C has not been implemented.
        [Test]
        public void V850Dis_2186()
        {
            AssertCode("@@@", "2186");
        }

        // .....................111011.:...
        // Reko: a decoder for V850 instruction 6807 at address 0010255E has not been implemented.
        [Test]
        public void V850Dis_6807()
        {
            AssertCode("@@@", "6807");
        }

        // ................:.:..110100.:::.
        // .................:.:.001000:.:.:
        // .................:.:...:...:.:.: or
        // .................:::.100111:..:.
        // .................:::.:..::::..:. sst_h
        // ................:..::111101...:.
        // Reko: a decoder for V850 instruction A29F at address 00102566 has not been implemented.
        [Test]
        public void V850Dis_A29F()
        {
            AssertCode("@@@", "A29F");
        }

        // .....................011110....:
        // ......................::::.....: sst_b
        // ..................::.010101..::.
        // Reko: a decoder for V850 instruction A632 at address 0010256A has not been implemented.
        [Test]
        public void V850Dis_A632()
        {
            AssertCode("@@@", "A632");
        }

        // .....................010010::.:.
        // .....................000111..::.
        // Reko: a decoder for V850 instruction E600 at address 0010256E has not been implemented.
        [Test]
        public void V850Dis_E600()
        {
            AssertCode("@@@", "E600");
        }

        // ................:.:::000111:.:..
        // Reko: a decoder for V850 instruction F4B8 at address 00102570 has not been implemented.
        [Test]
        public void V850Dis_F4B8()
        {
            AssertCode("@@@", "F4B8");
        }

        // ................:::::101010:.:..
        // Reko: a decoder for V850 instruction 54FD at address 00102572 has not been implemented.
        [Test]
        public void V850Dis_54FD()
        {
            AssertCode("@@@", "54FD");
        }

        // ................:.:..000010::...
        // ................10100....:.::...
        // ................:.:......:.11000
        // ................:.:......:.::... divh
        // ................:.:::000110:...:
        // Reko: a decoder for V850 instruction D1B8 at address 00102576 has not been implemented.
        [Test]
        public void V850Dis_D1B8()
        {
            AssertCode("@@@", "D1B8");
        }

        // ................:::.:111000::.::
        // Reko: a decoder for V850 instruction 1BEF at address 00102578 has not been implemented.
        [Test]
        public void V850Dis_1BEF()
        {
            AssertCode("@@@", "1BEF");
        }

        // ..................:::010000:.:.:
        // ..................:::.:....10101
        // ..................:::.:....:.:.: mov
        // ..................:::011101::..:
        // ..................:::.:::.:::..: sst_b
        // ..................::.010101.::.:
        // Reko: a decoder for V850 instruction AD32 at address 0010257E has not been implemented.
        [Test]
        public void V850Dis_AD32()
        {
            AssertCode("@@@", "AD32");
        }

        // ................::::.010101...::
        // Reko: a decoder for V850 instruction A3F2 at address 00102580 has not been implemented.
        [Test]
        public void V850Dis_A3F2()
        {
            AssertCode("@@@", "A3F2");
        }

        // ................:::::000111::::.
        // Reko: a decoder for V850 instruction FEF8 at address 00102582 has not been implemented.
        [Test]
        public void V850Dis_FEF8()
        {
            AssertCode("@@@", "FEF8");
        }

        // ...................:.000100.:::.
        // Reko: a decoder for V850 instruction 8E10 at address 00102584 has not been implemented.
        [Test]
        public void V850Dis_8E10()
        {
            AssertCode("@@@", "8E10");
        }

        // ................:.:::001010:::::
        // Reko: a decoder for V850 instruction 5FB9 at address 00102586 has not been implemented.
        [Test]
        public void V850Dis_5FB9()
        {
            AssertCode("@@@", "5FB9");
        }

        // ................::..:100100:.:..
        // ................::..::..:..:.:.. sst_h
        // ..................:..100101:..::
        // ..................:..:..:.::..:: sst_h
        // .................::..111001:.:::
        // Reko: a decoder for V850 instruction 3767 at address 0010258C has not been implemented.
        [Test]
        public void V850Dis_3767()
        {
            AssertCode("@@@", "3767");
        }

        // ................::.:.111010:.::.
        // Reko: a decoder for V850 instruction 56D7 at address 0010258E has not been implemented.
        [Test]
        public void V850Dis_56D7()
        {
            AssertCode("@@@", "56D7");
        }

        // .................::..101110..::.
        // Reko: a decoder for V850 instruction C665 at address 00102590 has not been implemented.
        [Test]
        public void V850Dis_C665()
        {
            AssertCode("@@@", "C665");
        }

        // .................::.:000010.:.:.
        // ................01101....:..:.:.
        // .................::.:....:.01010
        // .................::.:....:..:.:. divh
        // .................::.:011001:..:.
        // .................::.:.::..::..:. sld_b
        // ................:::.:101101.::::
        // Reko: a decoder for V850 instruction AFED at address 00102596 has not been implemented.
        [Test]
        public void V850Dis_AFED()
        {
            AssertCode("@@@", "AFED");
        }

        // .................:.::101110:.:..
        // Reko: a decoder for V850 instruction D45D at address 00102598 has not been implemented.
        [Test]
        public void V850Dis_D45D()
        {
            AssertCode("@@@", "D45D");
        }

        // .....................011001:.:.:
        // ......................::..::.:.: sld_b
        // .................::..111010::::.
        // Reko: a decoder for V850 instruction 5E67 at address 0010259C has not been implemented.
        [Test]
        public void V850Dis_5E67()
        {
            AssertCode("@@@", "5E67");
        }

        // ................:....110111..::.
        // Reko: a decoder for V850 instruction E686 at address 0010259E has not been implemented.
        [Test]
        public void V850Dis_E686()
        {
            AssertCode("@@@", "E686");
        }

        // .................:::.100010:.::.
        // .................:::.:...:.:.::. sld_h
        // .................::.:000100.:::.
        // Reko: a decoder for V850 instruction 8E68 at address 001025A2 has not been implemented.
        [Test]
        public void V850Dis_8E68()
        {
            AssertCode("@@@", "8E68");
        }

        // ................:::.:110000..:..
        // Reko: a decoder for V850 instruction 04EE at address 001025A4 has not been implemented.
        [Test]
        public void V850Dis_04EE()
        {
            AssertCode("@@@", "04EE");
        }

        // ................:::..100011:..:.
        // ................:::..:...:::..:. sld_h
        // ................:..::010010::...
        // Reko: a decoder for V850 instruction 589A at address 001025A8 has not been implemented.
        [Test]
        public void V850Dis_589A()
        {
            AssertCode("@@@", "589A");
        }

        // ................:...:100011.:...
        // ................:...::...::.:... sld_h
        // ................:.:.:101111.::..
        // Reko: a decoder for V850 instruction ECAD at address 001025AC has not been implemented.
        [Test]
        public void V850Dis_ECAD()
        {
            AssertCode("@@@", "ECAD");
        }

        // ................:.::.100100.....
        // ................:.::.:..:....... sst_h
        // ..................:::110100::...
        // .................:.:.011011:.:..
        // .................:.:..::.:::.:.. sld_b
        // .................:...100111:...:
        // .................:...:..::::...: sst_h
        // ..................:::000111:::::
        // ...................:.000010...:.
        // ................00010....:....:.
        // ...................:.....:.00010
        // ...................:.....:....:. divh
        // ..................::.001001...::
        // Reko: a decoder for V850 instruction 2331 at address 001025BA has not been implemented.
        [Test]
        public void V850Dis_2331()
        {
            AssertCode("@@@", "2331");
        }

        // .................:.::100110::.::
        // .................:.:::..::.::.:: sst_h
        // ..................:.:101010::.:.
        // Reko: a decoder for V850 instruction 5A2D at address 001025BE has not been implemented.
        [Test]
        public void V850Dis_5A2D()
        {
            AssertCode("@@@", "5A2D");
        }

        // ..................:.:101111..:.:
        // Reko: a decoder for V850 instruction E52D at address 001025C0 has not been implemented.
        [Test]
        public void V850Dis_E52D()
        {
            AssertCode("@@@", "E52D");
        }

        // ................::...011110.:::.
        // ................::....::::..:::. sst_b
        // ................:::.:110110::...
        // Reko: a decoder for V850 instruction D8EE at address 001025C4 has not been implemented.
        [Test]
        public void V850Dis_D8EE()
        {
            AssertCode("@@@", "D8EE");
        }

        // ................::.::000110:..::
        // Reko: a decoder for V850 instruction D3D8 at address 001025C6 has not been implemented.
        [Test]
        public void V850Dis_D3D8()
        {
            AssertCode("@@@", "D3D8");
        }

        // .................::..101111:..::
        // Reko: a decoder for V850 instruction F365 at address 001025C8 has not been implemented.
        [Test]
        public void V850Dis_F365()
        {
            AssertCode("@@@", "F365");
        }

        // ................:..:.111000.::::
        // Reko: a decoder for V850 instruction 0F97 at address 001025CA has not been implemented.
        [Test]
        public void V850Dis_0F97()
        {
            AssertCode("@@@", "0F97");
        }

        // ..................:..011000...:.
        // ..................:...::......:. sld_b
        // ..................:..111000.....
        // Reko: a decoder for V850 instruction 0027 at address 001025CE has not been implemented.
        [Test]
        public void V850Dis_0027()
        {
            AssertCode("@@@", "0027");
        }

        // ................:....000110:::..
        // Reko: a decoder for V850 instruction DC80 at address 001025D0 has not been implemented.
        [Test]
        public void V850Dis_DC80()
        {
            AssertCode("@@@", "DC80");
        }

        // ..................:..000011.:.:.
        // Reko: a decoder for V850 instruction 6A20 at address 001025D2 has not been implemented.
        [Test]
        public void V850Dis_6A20()
        {
            AssertCode("@@@", "6A20");
        }

        // ................:..:.010111:..::
        // Reko: a decoder for V850 instruction F392 at address 001025D4 has not been implemented.
        [Test]
        public void V850Dis_F392()
        {
            AssertCode("@@@", "F392");
        }

        // ...................:.001100:.:..
        // Reko: a decoder for V850 instruction 9411 at address 001025D6 has not been implemented.
        [Test]
        public void V850Dis_9411()
        {
            AssertCode("@@@", "9411");
        }

        // .................:.:.110011:.:..
        // Reko: a decoder for V850 instruction 7456 at address 001025D8 has not been implemented.
        [Test]
        public void V850Dis_7456()
        {
            AssertCode("@@@", "7456");
        }

        // ................::::.011001..:.:
        // ................::::..::..:..:.: sld_b
        // .................:...001110.:.:.
        // Reko: a decoder for V850 instruction CA41 at address 001025DC has not been implemented.
        [Test]
        public void V850Dis_CA41()
        {
            AssertCode("@@@", "CA41");
        }

        // ................:::::100000.:..:
        // ................::::::......:..: sld_h
        // ..................:::010010.....
        // ................:.:::111101..::.
        // ................:.:::101110:::::
        // Reko: a decoder for V850 instruction DFBD at address 001025E4 has not been implemented.
        [Test]
        public void V850Dis_DFBD()
        {
            AssertCode("@@@", "DFBD");
        }

        // ................:.:.:010110::.::
        // ................:..::011011.:.::
        // ................:..::.::.::.:.:: sld_b
        // ................:...:011011.....
        // ................:...:.::.::..... sld_b
        // .................::.:000001:.:.:
        // .................::.:.....::.:.: not
        // ...................::110001:.:.:
        // Reko: a decoder for V850 instruction 351E at address 001025EE has not been implemented.
        [Test]
        public void V850Dis_351E()
        {
            AssertCode("@@@", "351E");
        }

        // ................:::::100000.::::
        // ................::::::......:::: sld_h
        // .................:.::010010:.:..
        // Reko: a decoder for V850 instruction 545A at address 001025F2 has not been implemented.
        [Test]
        public void V850Dis_545A()
        {
            AssertCode("@@@", "545A");
        }

        // ..................:::010111:..:.
        // Reko: a decoder for V850 instruction F23A at address 001025F4 has not been implemented.
        [Test]
        public void V850Dis_F23A()
        {
            AssertCode("@@@", "F23A");
        }

        // .................:.:.011001::.:.
        // .................:.:..::..:::.:. sld_b
        // ................:.:::100100::...
        // ................:.::::..:..::... sst_h
        // ................:....010010.:::.
        // Reko: a decoder for V850 instruction 4E82 at address 001025FA has not been implemented.
        [Test]
        public void V850Dis_4E82()
        {
            AssertCode("@@@", "4E82");
        }

        // ................::.::111110.:...
        // Reko: a decoder for V850 instruction C8DF at address 001025FC has not been implemented.
        [Test]
        public void V850Dis_C8DF()
        {
            AssertCode("@@@", "C8DF");
        }

        // ................:.::.010000.::.:
        // ................:.::..:....01101
        // ................:.::..:.....::.: mov
        // ...................:.011010:::..
        // ...................:..::.:.:::.. sld_b
        // ................::...111010:.:::
        // Reko: a decoder for V850 instruction 57C7 at address 00102602 has not been implemented.
        [Test]
        public void V850Dis_57C7()
        {
            AssertCode("@@@", "57C7");
        }

        // .................:...001011:::.:
        // Reko: a decoder for V850 instruction 7D41 at address 00102604 has not been implemented.
        [Test]
        public void V850Dis_7D41()
        {
            AssertCode("@@@", "7D41");
        }

        // ...................::110010..:::
        // Reko: a decoder for V850 instruction 471E at address 00102606 has not been implemented.
        [Test]
        public void V850Dis_471E()
        {
            AssertCode("@@@", "471E");
        }

        // ................::...000101..:.:
        // Reko: a decoder for V850 instruction A5C0 at address 00102608 has not been implemented.
        [Test]
        public void V850Dis_A5C0()
        {
            AssertCode("@@@", "A5C0");
        }

        // .................:.:.111101:.:.:
        // Reko: a decoder for V850 instruction B557 at address 0010260A has not been implemented.
        [Test]
        public void V850Dis_B557()
        {
            AssertCode("@@@", "B557");
        }

        // ....................:001001:..:.
        // Reko: a decoder for V850 instruction 3209 at address 0010260C has not been implemented.
        [Test]
        public void V850Dis_3209()
        {
            AssertCode("@@@", "3209");
        }

        // ....................:011110:::..
        // ....................:.::::.:::.. sst_b
        // .....................111001.....
        // Reko: a decoder for V850 instruction 2007 at address 00102610 has not been implemented.
        [Test]
        public void V850Dis_2007()
        {
            AssertCode("@@@", "2007");
        }

        // ................::..:110111...::
        // Reko: a decoder for V850 instruction E3CE at address 00102612 has not been implemented.
        [Test]
        public void V850Dis_E3CE()
        {
            AssertCode("@@@", "E3CE");
        }

        // ................:.:.:110111.::..
        // Reko: a decoder for V850 instruction ECAE at address 00102614 has not been implemented.
        [Test]
        public void V850Dis_ECAE()
        {
            AssertCode("@@@", "ECAE");
        }

        // .....................101000.:.:.
        // .....................:.:....:.:0 Sld.w/Sst.w
        // .....................:.:....:.:. sld_w
        // .................:.:.010000.....
        // .................:.:..:....00000
        // .................:.:..:......... invalid
        // ..................:..010001..::.
        // Reko: a decoder for V850 instruction 2622 at address 0010261A has not been implemented.
        [Test]
        public void V850Dis_2622()
        {
            AssertCode("@@@", "2622");
        }

        // .................:.::000011.::::
        // Reko: a decoder for V850 instruction 6F58 at address 0010261C has not been implemented.
        [Test]
        public void V850Dis_6F58()
        {
            AssertCode("@@@", "6F58");
        }

        // ................:...:011111....:
        // ................:...:.:::::....: sst_b
        // ................:.:.:110100.:::.
        // Reko: a decoder for V850 instruction 8EAE at address 00102620 has not been implemented.
        [Test]
        public void V850Dis_8EAE()
        {
            AssertCode("@@@", "8EAE");
        }

        // ..................:..001001.:::.
        // Reko: a decoder for V850 instruction 2E21 at address 00102622 has not been implemented.
        [Test]
        public void V850Dis_2E21()
        {
            AssertCode("@@@", "2E21");
        }

        // .................::.:101000:...:
        // .................::.::.:...:...1 Sld.w/Sst.w
        // .................::.::.:...:...: sst_w
        // ................::..:110101.....
        // Reko: a decoder for V850 instruction A0CE at address 00102626 has not been implemented.
        [Test]
        public void V850Dis_A0CE()
        {
            AssertCode("@@@", "A0CE");
        }

        // ..................::.101101:.:..
        // Reko: a decoder for V850 instruction B435 at address 00102628 has not been implemented.
        [Test]
        public void V850Dis_B435()
        {
            AssertCode("@@@", "B435");
        }

        // ..................::.010101:.:..
        // Reko: a decoder for V850 instruction B432 at address 0010262A has not been implemented.
        [Test]
        public void V850Dis_B432()
        {
            AssertCode("@@@", "B432");
        }

        // ................:::..011100..:.:
        // ................:::...:::....:.: sst_b
        // ................:...:111001....:
        // Reko: a decoder for V850 instruction 218F at address 0010262E has not been implemented.
        [Test]
        public void V850Dis_218F()
        {
            AssertCode("@@@", "218F");
        }

        // .................:...010110:::::
        // Reko: a decoder for V850 instruction DF42 at address 00102630 has not been implemented.
        [Test]
        public void V850Dis_DF42()
        {
            AssertCode("@@@", "DF42");
        }

        // ................::...000011...:.
        // Reko: a decoder for V850 instruction 62C0 at address 00102632 has not been implemented.
        [Test]
        public void V850Dis_62C0()
        {
            AssertCode("@@@", "62C0");
        }

        // ..................:::111111::.::
        // Reko: a decoder for V850 instruction FB3F at address 00102634 has not been implemented.
        [Test]
        public void V850Dis_FB3F()
        {
            AssertCode("@@@", "FB3F");
        }

        // ................:.:.:110110..:.:
        // Reko: a decoder for V850 instruction C5AE at address 00102636 has not been implemented.
        [Test]
        public void V850Dis_C5AE()
        {
            AssertCode("@@@", "C5AE");
        }

        // .................:.:.001000.:..:
        // .................:.:...:....:..: or
        // ..................:.:011110..:::
        // ..................:.:.::::...::: sst_b
        // ..................:..100000::.::
        // ..................:..:.....::.:: sld_h
        // .................:..:100101.::::
        // .................:..::..:.:.:::: sst_h
        // ..................:.:001110::::.
        // ................:....011110.:..:
        // ................:.....::::..:..: sst_b
        // ................:.:::010110::.:.
        // Reko: a decoder for V850 instruction DABA at address 00102644 has not been implemented.
        [Test]
        public void V850Dis_DABA()
        {
            AssertCode("@@@", "DABA");
        }

        // ................:.::.011111::.:.
        // ................:.::..:::::::.:. sst_b
        // ...................:.101011..:::
        // Reko: a decoder for V850 instruction 6715 at address 00102648 has not been implemented.
        [Test]
        public void V850Dis_6715()
        {
            AssertCode("@@@", "6715");
        }

        // ................:.::.111000::..:
        // Reko: a decoder for V850 instruction 19B7 at address 0010264A has not been implemented.
        [Test]
        public void V850Dis_19B7()
        {
            AssertCode("@@@", "19B7");
        }

        // ................::.:.111111::..:
        // Reko: a decoder for V850 instruction F9D7 at address 0010264C has not been implemented.
        [Test]
        public void V850Dis_F9D7()
        {
            AssertCode("@@@", "F9D7");
        }

        // ...................::101100.....
        // Reko: a decoder for V850 instruction 801D at address 0010264E has not been implemented.
        [Test]
        public void V850Dis_801D()
        {
            AssertCode("@@@", "801D");
        }

        // ................:..:.111001:...:
        // Reko: a decoder for V850 instruction 3197 at address 00102650 has not been implemented.
        [Test]
        public void V850Dis_3197()
        {
            AssertCode("@@@", "3197");
        }

        // ..................:.:100010...::
        // ..................:.::...:....:: sld_h
        // ..................:..000111:.::.
        // Reko: a decoder for V850 instruction F620 at address 00102654 has not been implemented.
        [Test]
        public void V850Dis_F620()
        {
            AssertCode("@@@", "F620");
        }

        // .................:..:110100.::::
        // Reko: a decoder for V850 instruction 8F4E at address 00102656 has not been implemented.
        [Test]
        public void V850Dis_8F4E()
        {
            AssertCode("@@@", "8F4E");
        }

        // ................:....000111.:...
        // Reko: a decoder for V850 instruction E880 at address 00102658 has not been implemented.
        [Test]
        public void V850Dis_E880()
        {
            AssertCode("@@@", "E880");
        }

        // ................:.:::111000::...
        // Reko: a decoder for V850 instruction 18BF at address 0010265A has not been implemented.
        [Test]
        public void V850Dis_18BF()
        {
            AssertCode("@@@", "18BF");
        }

        // .................:::.101101..::.
        // Reko: a decoder for V850 instruction A675 at address 0010265C has not been implemented.
        [Test]
        public void V850Dis_A675()
        {
            AssertCode("@@@", "A675");
        }

        // ................:...:111101:..::
        // Reko: a decoder for V850 instruction B38F at address 0010265E has not been implemented.
        [Test]
        public void V850Dis_B38F()
        {
            AssertCode("@@@", "B38F");
        }

        // .................::.:000100..:.:
        // Reko: a decoder for V850 instruction 8568 at address 00102660 has not been implemented.
        [Test]
        public void V850Dis_8568()
        {
            AssertCode("@@@", "8568");
        }

        // .................:.::011010:..::
        // .................:.::.::.:.:..:: sld_b
        // ................:..::101000::..:
        // ................:..:::.:...::..1 Sld.w/Sst.w
        // ................:..:::.:...::..: sst_w
        // ..................:..100101:.:..
        // ..................:..:..:.::.:.. sst_h
        // ................:.:.:111010.:...
        // Reko: a decoder for V850 instruction 48AF at address 00102668 has not been implemented.
        [Test]
        public void V850Dis_48AF()
        {
            AssertCode("@@@", "48AF");
        }

        // ................:::..110010::..:
        // ....................:110001..::.
        // Reko: a decoder for V850 instruction 260E at address 0010266C has not been implemented.
        [Test]
        public void V850Dis_260E()
        {
            AssertCode("@@@", "260E");
        }

        // ..................::.001101:.:..
        // Reko: a decoder for V850 instruction B431 at address 0010266E has not been implemented.
        [Test]
        public void V850Dis_B431()
        {
            AssertCode("@@@", "B431");
        }

        // ................:.:.:010110:...:
        // Reko: a decoder for V850 instruction D1AA at address 00102670 has not been implemented.
        [Test]
        public void V850Dis_D1AA()
        {
            AssertCode("@@@", "D1AA");
        }

        // ................:....011111::.:.
        // ................:.....:::::::.:. sst_b
        // ................:....011111....:
        // ................:.....:::::....: sst_b
        // ................:.:::101010:.::.
        // Reko: a decoder for V850 instruction 56BD at address 00102676 has not been implemented.
        [Test]
        public void V850Dis_56BD()
        {
            AssertCode("@@@", "56BD");
        }

        // .................::::010100::..:
        // .................:.::010100::.:.
        // ..................:::011101...:.
        // ..................:::.:::.:...:. sst_b
        // ................::::.010000.::..
        // ................::::..:....01100
        // ................::::..:.....::.. mov
        // ................:...:001011....:
        // Reko: a decoder for V850 instruction 6189 at address 00102680 has not been implemented.
        [Test]
        public void V850Dis_6189()
        {
            AssertCode("@@@", "6189");
        }

        // ................:..:.001001:...:
        // Reko: a decoder for V850 instruction 3191 at address 00102682 has not been implemented.
        [Test]
        public void V850Dis_3191()
        {
            AssertCode("@@@", "3191");
        }

        // ..................:..101100..:..
        // Reko: a decoder for V850 instruction 8425 at address 00102684 has not been implemented.
        [Test]
        public void V850Dis_8425()
        {
            AssertCode("@@@", "8425");
        }

        // ................:..:.011000...::
        // ................:..:..::......:: sld_b
        // ...................::110111...:.
        // Reko: a decoder for V850 instruction E21E at address 00102688 has not been implemented.
        [Test]
        public void V850Dis_E21E()
        {
            AssertCode("@@@", "E21E");
        }

        // .................:...111111..:..
        // Reko: a decoder for V850 instruction E447 at address 0010268A has not been implemented.
        [Test]
        public void V850Dis_E447()
        {
            AssertCode("@@@", "E447");
        }

        // ................:.:.:001110..:::
        // Reko: a decoder for V850 instruction C7A9 at address 0010268C has not been implemented.
        [Test]
        public void V850Dis_C7A9()
        {
            AssertCode("@@@", "C7A9");
        }

        // ................::.::010001.:...
        // Reko: a decoder for V850 instruction 28DA at address 0010268E has not been implemented.
        [Test]
        public void V850Dis_28DA()
        {
            AssertCode("@@@", "28DA");
        }

        // ................:::..111110.::..
        // Reko: a decoder for V850 instruction CCE7 at address 00102690 has not been implemented.
        [Test]
        public void V850Dis_CCE7()
        {
            AssertCode("@@@", "CCE7");
        }

        // ................:::::011111.:::.
        // ................:::::.:::::.:::. sst_b
        // ..................:.:111011::.::
        // Reko: a decoder for V850 instruction 7B2F at address 00102694 has not been implemented.
        [Test]
        public void V850Dis_7B2F()
        {
            AssertCode("@@@", "7B2F");
        }

        // .....................010101.::.:
        // Reko: a decoder for V850 instruction AD02 at address 00102696 has not been implemented.
        [Test]
        public void V850Dis_AD02()
        {
            AssertCode("@@@", "AD02");
        }

        // ................:..:.110000:...:
        // ................:..::110110....:
        // Reko: a decoder for V850 instruction C19E at address 0010269A has not been implemented.
        [Test]
        public void V850Dis_C19E()
        {
            AssertCode("@@@", "C19E");
        }

        // .................:.::010000...:.
        // .................:.::.:....00010
        // .................:.::.:.......:. mov
        // ....................:100100...:.
        // ....................::..:.....:. sst_h
        // .................::..111101...:.
        // Reko: a decoder for V850 instruction A267 at address 001026A0 has not been implemented.
        [Test]
        public void V850Dis_A267()
        {
            AssertCode("@@@", "A267");
        }

        // ................:..::100100..:..
        // ................:..:::..:....:.. sst_h
        // ..................:..010110:::.:
        // Reko: a decoder for V850 instruction DD22 at address 001026A4 has not been implemented.
        [Test]
        public void V850Dis_DD22()
        {
            AssertCode("@@@", "DD22");
        }

        // ................:.:.:100101.::.:
        // ................:.:.::..:.:.::.: sst_h
        // .................::..110101..::.
        // Reko: a decoder for V850 instruction A666 at address 001026A8 has not been implemented.
        [Test]
        public void V850Dis_A666()
        {
            AssertCode("@@@", "A666");
        }

        // ................::...010101....:
        // Reko: a decoder for V850 instruction A1C2 at address 001026AA has not been implemented.
        [Test]
        public void V850Dis_A1C2()
        {
            AssertCode("@@@", "A1C2");
        }

        // ....................:110100:::::
        // Reko: a decoder for V850 instruction 9F0E at address 001026AC has not been implemented.
        [Test]
        public void V850Dis_9F0E()
        {
            AssertCode("@@@", "9F0E");
        }

        // ................::::.000111:.::.
        // Reko: a decoder for V850 instruction F6F0 at address 001026AE has not been implemented.
        [Test]
        public void V850Dis_F6F0()
        {
            AssertCode("@@@", "F6F0");
        }

        // ................::...000010:..:.
        // ................11000....:.:..:.
        // ................::.......:.10010
        // ................::.......:.:..:. divh
        // ................:.:::010100.:::.
        // Reko: a decoder for V850 instruction 8EBA at address 001026B2 has not been implemented.
        [Test]
        public void V850Dis_8EBA()
        {
            AssertCode("@@@", "8EBA");
        }

        // ...................:.000010.::.:
        // ................00010....:..::.:
        // ...................:.....:.01101
        // ...................:.....:..::.: divh
        // ..................::.010011:.::.
        // Reko: a decoder for V850 instruction 7632 at address 001026B6 has not been implemented.
        [Test]
        public void V850Dis_7632()
        {
            AssertCode("@@@", "7632");
        }

        // ..................:.:110110:::..
        // Reko: a decoder for V850 instruction DC2E at address 001026B8 has not been implemented.
        [Test]
        public void V850Dis_DC2E()
        {
            AssertCode("@@@", "DC2E");
        }

        // ................:::.:110011:::::
        // Reko: a decoder for V850 instruction 7FEE at address 001026BA has not been implemented.
        [Test]
        public void V850Dis_7FEE()
        {
            AssertCode("@@@", "7FEE");
        }

        // .................::..100011...::
        // .................::..:...::...:: sld_h
        // ................:...:000100:....
        // Reko: a decoder for V850 instruction 9088 at address 001026BE has not been implemented.
        [Test]
        public void V850Dis_9088()
        {
            AssertCode("@@@", "9088");
        }

        // ................::.:.111100:::..
        // Reko: a decoder for V850 instruction 9CD7 at address 001026C0 has not been implemented.
        [Test]
        public void V850Dis_9CD7()
        {
            AssertCode("@@@", "9CD7");
        }

        // .................:.::111101..:.:
        // Reko: a decoder for V850 instruction A55F at address 001026C2 has not been implemented.
        [Test]
        public void V850Dis_A55F()
        {
            AssertCode("@@@", "A55F");
        }

        // ................:...:011001..:..
        // ................:...:.::..:..:.. sld_b
        // ................:.:.:001111:::.:
        // Reko: a decoder for V850 instruction FDA9 at address 001026C6 has not been implemented.
        [Test]
        public void V850Dis_FDA9()
        {
            AssertCode("@@@", "FDA9");
        }

        // .................:...010001::.:.
        // Reko: a decoder for V850 instruction 3A42 at address 001026C8 has not been implemented.
        [Test]
        public void V850Dis_3A42()
        {
            AssertCode("@@@", "3A42");
        }

        // ...................:.100010:.:.:
        // ...................:.:...:.:.:.: sld_h
        // ................:::.:011100.:::.
        // ................:::.:.:::...:::. sst_b
        // .....................011011...::
        // ......................::.::...:: sld_b
        // ................::.:.000100:::::
        // Reko: a decoder for V850 instruction 9FD0 at address 001026D0 has not been implemented.
        [Test]
        public void V850Dis_9FD0()
        {
            AssertCode("@@@", "9FD0");
        }

        // .................:...010010.::..
        // Reko: a decoder for V850 instruction 4C42 at address 001026D2 has not been implemented.
        [Test]
        public void V850Dis_4C42()
        {
            AssertCode("@@@", "4C42");
        }

        // ................::...111110.:.::
        // Reko: a decoder for V850 instruction CBC7 at address 001026D4 has not been implemented.
        [Test]
        public void V850Dis_CBC7()
        {
            AssertCode("@@@", "CBC7");
        }

        // ................:..:.001101:::::
        // Reko: a decoder for V850 instruction BF91 at address 001026D6 has not been implemented.
        [Test]
        public void V850Dis_BF91()
        {
            AssertCode("@@@", "BF91");
        }

        // .................:.:.010111:....
        // Reko: a decoder for V850 instruction F052 at address 001026D8 has not been implemented.
        [Test]
        public void V850Dis_F052()
        {
            AssertCode("@@@", "F052");
        }

        // ................:.:.:110110.....
        // ................:.:::110101...:.
        // Reko: a decoder for V850 instruction A2BE at address 001026DC has not been implemented.
        [Test]
        public void V850Dis_A2BE()
        {
            AssertCode("@@@", "A2BE");
        }

        // .................:...010010.:...
        // Reko: a decoder for V850 instruction 4842 at address 001026DE has not been implemented.
        [Test]
        public void V850Dis_4842()
        {
            AssertCode("@@@", "4842");
        }

        // ...................::000110:.:.:
        // Reko: a decoder for V850 instruction D518 at address 001026E0 has not been implemented.
        [Test]
        public void V850Dis_D518()
        {
            AssertCode("@@@", "D518");
        }

        // ................::...111001..::.
        // Reko: a decoder for V850 instruction 26C7 at address 001026E2 has not been implemented.
        [Test]
        public void V850Dis_26C7()
        {
            AssertCode("@@@", "26C7");
        }

        // ................:..:.110011.:..:
        // Reko: a decoder for V850 instruction 6996 at address 001026E4 has not been implemented.
        [Test]
        public void V850Dis_6996()
        {
            AssertCode("@@@", "6996");
        }

        // ................::...111110.::..
        // Reko: a decoder for V850 instruction CCC7 at address 001026E6 has not been implemented.
        [Test]
        public void V850Dis_CCC7()
        {
            AssertCode("@@@", "CCC7");
        }

        // ................:::..101110...:.
        // Reko: a decoder for V850 instruction C2E5 at address 001026E8 has not been implemented.
        [Test]
        public void V850Dis_C2E5()
        {
            AssertCode("@@@", "C2E5");
        }

        // ....................:010100.....
        // Reko: a decoder for V850 instruction 800A at address 001026EA has not been implemented.
        [Test]
        public void V850Dis_800A()
        {
            AssertCode("@@@", "800A");
        }

        // .................::..010101::.::
        // Reko: a decoder for V850 instruction BB62 at address 001026EC has not been implemented.
        [Test]
        public void V850Dis_BB62()
        {
            AssertCode("@@@", "BB62");
        }

        // ................::::.000101...::
        // Reko: a decoder for V850 instruction A3F0 at address 001026EE has not been implemented.
        [Test]
        public void V850Dis_A3F0()
        {
            AssertCode("@@@", "A3F0");
        }

        // .................::::100110.:.::
        // .................:::::..::..:.:: sst_h
        // ................:..:.011010.:::.
        // ................:..:..::.:..:::. sld_b
        // .................:..:110000::...
        // Reko: a decoder for V850 instruction 184E at address 001026F4 has not been implemented.
        [Test]
        public void V850Dis_184E()
        {
            AssertCode("@@@", "184E");
        }

        // ................::.:.100101:::.:
        // ................::.:.:..:.::::.: sst_h
        // ...................:.100000:.:::
        // ...................:.:.....:.::: sld_h
        // ................::.:.001011.::::
        // Reko: a decoder for V850 instruction 6FD1 at address 001026FA has not been implemented.
        [Test]
        public void V850Dis_6FD1()
        {
            AssertCode("@@@", "6FD1");
        }

        // .................:..:111110.:.::
        // .................::::011000..::.
        // .................::::.::.....::. sld_b
        // ...................:.010111::.:.
        // Reko: a decoder for V850 instruction FA12 at address 00102700 has not been implemented.
        [Test]
        public void V850Dis_FA12()
        {
            AssertCode("@@@", "FA12");
        }

        // .................:::.101001:::::
        // Reko: a decoder for V850 instruction 3F75 at address 00102702 has not been implemented.
        [Test]
        public void V850Dis_3F75()
        {
            AssertCode("@@@", "3F75");
        }

        // ................:..::011110..:..
        // ................:..::.::::...:.. sst_b
        // ....................:001100::..:
        // Reko: a decoder for V850 instruction 9909 at address 00102706 has not been implemented.
        [Test]
        public void V850Dis_9909()
        {
            AssertCode("@@@", "9909");
        }

        // ................:....100000..:..
        // ................:....:.......:.. sld_h
        // ................:::..110011:::::
        // Reko: a decoder for V850 instruction 7FE6 at address 0010270A has not been implemented.
        [Test]
        public void V850Dis_7FE6()
        {
            AssertCode("@@@", "7FE6");
        }

        // ................:.::.101010..:::
        // Reko: a decoder for V850 instruction 47B5 at address 0010270C has not been implemented.
        [Test]
        public void V850Dis_47B5()
        {
            AssertCode("@@@", "47B5");
        }

        // ................:...:110110:.:.:
        // Reko: a decoder for V850 instruction D58E at address 0010270E has not been implemented.
        [Test]
        public void V850Dis_D58E()
        {
            AssertCode("@@@", "D58E");
        }


    }
}
