#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Arch.XCore;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.XCore
{
    [TestFixture]
    public class XCoreDisassemblerTests : DisassemblerTestBase<XCoreInstruction>
    {
        private XCore200Architecture arch;
        private Address addr;

        public XCoreDisassemblerTests()
        {
            this.arch = new XCore200Architecture("xcore");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void XCoreDis_foo()
        {
            var mem = new MemoryArea(Address.Ptr32(0x00100000), new byte[65536]);
            var rnd = new Random(4711);
            rnd.NextBytes(mem.Bytes);
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(mem, 0));
            var instrs = dasm.ToArray();
        }

        private void AssertCode(string sExpected, string hexbytes)
        {
            var instr = base.DisassembleHexBytes(hexbytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void XCoreDis_add()
        {
            AssertCode("add\tr4,r11,r3", "CF13");
        }

        [Test]
        public void XCoreDis_add_r3_r3_r3()
        {
            AssertCode("add\tr3,r3,r3", "3F10");
        }

        [Test]
        public void XCoreDis_add_r3_r7_r3()
        {
            AssertCode("add\tr3,r7,r3", "FF10");
        }

        [Test]
        public void XCoreDis_add_r7_r3_r3()
        {
            AssertCode("add\tr7,r3,r3", "7F12");
        }

        [Test]
        public void XCoreDis_addi()
        {
            AssertCode("addi\tr0,r1,00000004@", "0497");
        }

        [Test]
        public void XCoreDis_and()
        {
            AssertCode("and\tr0,r7,r6", "CE3F");
        }

        [Test]
        public void XCoreDis_andnot()
        {
            AssertCode("andnot\tr11,r1", "2D2F");
        }

        /////////////////////////////////////////////////////////////////////////

        // Reko: a decoder for XCore instruction E804 at address 00100000 has not been implemented. (1D)
        [Test]
        public void XCoreDis_E804()
        {
            AssertCode("@@@", "04E8");
        }

        // Reko: a decoder for XCore instruction 027C at address 00100002 has not been implemented. (00)
        [Test]
        public void XCoreDis_027C()
        {
            AssertCode("@@@", "7C02");
        }

        // Reko: a decoder for XCore instruction D101 at address 00100004 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D101()
        {
            AssertCode("@@@", "01D1");
        }

        // Reko: a decoder for XCore instruction C0BC at address 00100006 has not been implemented. (18)
        [Test]
        public void XCoreDis_C0BC()
        {
            AssertCode("@@@", "BCC0");
        }

        // Reko: a decoder for XCore instruction 08E5 at address 00100008 has not been implemented. (01)
        [Test]
        public void XCoreDis_08E5()
        {
            AssertCode("@@@", "E508");
        }

        // Reko: a decoder for XCore instruction 4383 at address 0010000A has not been implemented. (08)
        [Test]
        public void XCoreDis_4383()
        {
            AssertCode("@@@", "8343");
        }

        // Reko: a decoder for XCore instruction 61E1 at address 0010000C has not been implemented. (0C)
        [Test]
        public void XCoreDis_61E1()
        {
            AssertCode("@@@", "E161");
        }

        // Reko: a decoder for XCore instruction AA7E at address 0010000E has not been implemented. (15)
        [Test]
        public void XCoreDis_AA7E()
        {
            AssertCode("@@@", "7EAA");
        }

        // Reko: a decoder for XCore instruction 15FB at address 00100010 has not been implemented. (02)
        [Test]
        public void XCoreDis_15FB()
        {
            AssertCode("@@@", "FB15");
        }

        // Reko: a decoder for XCore instruction 8A38 at address 00100012 has not been implemented. (11)
        [Test]
        public void XCoreDis_8A38()
        {
            AssertCode("@@@", "388A");
        }

        // Reko: a decoder for XCore instruction EA2D at address 00100014 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EA2D()
        {
            AssertCode("@@@", "2DEA");
        }

        // Reko: a decoder for XCore instruction 021A at address 00100016 has not been implemented. (00)
        [Test]
        public void XCoreDis_021A()
        {
            AssertCode("@@@", "1A02");
        }

        // Reko: a decoder for XCore instruction 176F at address 00100018 has not been implemented. (02)
        [Test]
        public void XCoreDis_176F()
        {
            AssertCode("@@@", "6F17");
        }

        // Reko: a decoder for XCore instruction C6F7 at address 0010001A has not been implemented. (18)
        [Test]
        public void XCoreDis_C6F7()
        {
            AssertCode("@@@", "F7C6");
        }

        // Reko: a decoder for XCore instruction 56BC at address 0010001C has not been implemented. (0A)
        [Test]
        public void XCoreDis_56BC()
        {
            AssertCode("@@@", "BC56");
        }

        // Reko: a decoder for XCore instruction 0B1B at address 0010001E has not been implemented. (01)
        [Test]
        public void XCoreDis_0B1B()
        {
            AssertCode("@@@", "1B0B");
        }

        // Reko: a decoder for XCore instruction E5AE at address 00100020 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E5AE()
        {
            AssertCode("@@@", "AEE5");
        }

        // Reko: a decoder for XCore instruction 6FBE at address 00100022 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6FBE()
        {
            AssertCode("@@@", "BE6F");
        }

        // Reko: a decoder for XCore instruction A738 at address 00100024 has not been implemented. (14)
        [Test]
        public void XCoreDis_A738()
        {
            AssertCode("@@@", "38A7");
        }

        // Reko: a decoder for XCore instruction 6041 at address 00100026 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6041()
        {
            AssertCode("@@@", "4160");
        }

        // Reko: a decoder for XCore instruction 37E4 at address 00100028 has not been implemented. (06)
        [Test]
        public void XCoreDis_37E4()
        {
            AssertCode("@@@", "E437");
        }

        // Reko: a decoder for XCore instruction D982 at address 0010002A has not been implemented. (1B)
        [Test]
        public void XCoreDis_D982()
        {
            AssertCode("@@@", "82D9");
        }

        // Reko: a decoder for XCore instruction F362 at address 0010002C has not been implemented. (1E)
        [Test]
        public void XCoreDis_F362()
        {
            AssertCode("@@@", "62F3");
        }

        // Reko: a decoder for XCore instruction D006 at address 0010002E has not been implemented. (1A)
        [Test]
        public void XCoreDis_D006()
        {
            AssertCode("@@@", "06D0");
        }

        // Reko: a decoder for XCore instruction 733E at address 00100030 has not been implemented. (0E)
        [Test]
        public void XCoreDis_733E()
        {
            AssertCode("@@@", "3E73");
        }

        // Reko: a decoder for XCore instruction 9C07 at address 00100032 has not been implemented. (13)
        [Test]
        public void XCoreDis_9C07()
        {
            AssertCode("@@@", "079C");
        }

        // Reko: a decoder for XCore instruction 6142 at address 00100034 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6142()
        {
            AssertCode("@@@", "4261");
        }

        // Reko: a decoder for XCore instruction 1ACA at address 00100036 has not been implemented. (03)
        [Test]
        public void XCoreDis_1ACA()
        {
            AssertCode("@@@", "CA1A");
        }

        // Reko: a decoder for XCore instruction 79CD at address 00100038 has not been implemented. (0F)
        [Test]
        public void XCoreDis_79CD()
        {
            AssertCode("@@@", "CD79");
        }

        // Reko: a decoder for XCore instruction E992 at address 0010003A has not been implemented. (1D)
        [Test]
        public void XCoreDis_E992()
        {
            AssertCode("@@@", "92E9");
        }

        // Reko: a decoder for XCore instruction F6DA at address 0010003C has not been implemented. (1E)
        [Test]
        public void XCoreDis_F6DA()
        {
            AssertCode("@@@", "DAF6");
        }

        // Reko: a decoder for XCore instruction 8F04 at address 0010003E has not been implemented. (11)
        [Test]
        public void XCoreDis_8F04()
        {
            AssertCode("@@@", "048F");
        }

        // Reko: a decoder for XCore instruction 77ED at address 00100040 has not been implemented. (0E)
        [Test]
        public void XCoreDis_77ED()
        {
            AssertCode("@@@", "ED77");
        }

        // Reko: a decoder for XCore instruction FC94 at address 00100042 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FC94()
        {
            AssertCode("@@@", "94FCEC17");
        }

        // Reko: a decoder for XCore instruction 0EA2 at address 00100044 has not been implemented. (01)
        [Test]
        public void XCoreDis_0EA2()
        {
            AssertCode("@@@", "A20E");
        }

        // Reko: a decoder for XCore instruction 5371 at address 00100046 has not been implemented. (0A)
        [Test]
        public void XCoreDis_5371()
        {
            AssertCode("@@@", "7153");
        }

        // Reko: a decoder for XCore instruction D7D4 at address 00100048 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D7D4()
        {
            AssertCode("@@@", "D4D7");
        }

        // Reko: a decoder for XCore instruction F6A6 at address 0010004A has not been implemented. (1E)
        [Test]
        public void XCoreDis_F6A6()
        {
            AssertCode("@@@", "A6F6");
        }

        // Reko: a decoder for XCore instruction 4068 at address 0010004C has not been implemented. (08)
        [Test]
        public void XCoreDis_4068()
        {
            AssertCode("@@@", "6840");
        }

        // Reko: a decoder for XCore instruction 7BA0 at address 0010004E has not been implemented. (0F)
        [Test]
        public void XCoreDis_7BA0()
        {
            AssertCode("@@@", "A07B");
        }

        // Reko: a decoder for XCore instruction 2711 at address 00100050 has not been implemented. (04)
        [Test]
        public void XCoreDis_2711()
        {
            AssertCode("@@@", "1127");
        }

        // Reko: a decoder for XCore instruction 4888 at address 00100052 has not been implemented. (09)
        [Test]
        public void XCoreDis_4888()
        {
            AssertCode("@@@", "8848");
        }

        // Reko: a decoder for XCore instruction 7E4E at address 00100054 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7E4E()
        {
            AssertCode("@@@", "4E7E");
        }

        // Reko: a decoder for XCore instruction 4DC8 at address 00100056 has not been implemented. (09)
        [Test]
        public void XCoreDis_4DC8()
        {
            AssertCode("@@@", "C84D");
        }

        // Reko: a decoder for XCore instruction A41A at address 00100058 has not been implemented. (14)
        [Test]
        public void XCoreDis_A41A()
        {
            AssertCode("@@@", "1AA4");
        }

        // Reko: a decoder for XCore instruction BEA1 at address 0010005A has not been implemented. (17)
        [Test]
        public void XCoreDis_BEA1()
        {
            AssertCode("@@@", "A1BE");
        }

        // Reko: a decoder for XCore instruction 5715 at address 0010005C has not been implemented. (0A)
        [Test]
        public void XCoreDis_5715()
        {
            AssertCode("@@@", "1557");
        }

        // Reko: a decoder for XCore instruction EE86 at address 0010005E has not been implemented. (1D)
        [Test]
        public void XCoreDis_EE86()
        {
            AssertCode("@@@", "86EE");
        }

        // Reko: a decoder for XCore instruction F233 at address 00100060 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F233()
        {
            AssertCode("@@@", "33F2");
        }

        // Reko: a decoder for XCore instruction EBEC at address 00100062 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EBEC()
        {
            AssertCode("@@@", "ECEB");
        }

        // Reko: a decoder for XCore instruction 0A5E at address 00100064 has not been implemented. (01)
        [Test]
        public void XCoreDis_0A5E()
        {
            AssertCode("@@@", "5E0A");
        }

        

        // Reko: a decoder for XCore instruction B402 at address 00100068 has not been implemented. (16)
        [Test]
        public void XCoreDis_B402()
        {
            AssertCode("@@@", "02B4");
        }

        // Reko: a decoder for XCore instruction 6BC8 at address 0010006A has not been implemented. (0D)
        [Test]
        public void XCoreDis_6BC8()
        {
            AssertCode("@@@", "C86B");
        }

        // Reko: a decoder for XCore instruction D4BA at address 0010006C has not been implemented. (1A)
        [Test]
        public void XCoreDis_D4BA()
        {
            AssertCode("@@@", "BAD4");
        }

        // Reko: a decoder for XCore instruction 8DB2 at address 0010006E has not been implemented. (11)
        [Test]
        public void XCoreDis_8DB2()
        {
            AssertCode("@@@", "B28D");
        }

        // Reko: a decoder for XCore instruction 16D9 at address 00100070 has not been implemented. (02)
        [Test]
        public void XCoreDis_16D9()
        {
            AssertCode("@@@", "D916");
        }

        // Reko: a decoder for XCore instruction B3D8 at address 00100072 has not been implemented. (16)
        [Test]
        public void XCoreDis_B3D8()
        {
            AssertCode("@@@", "D8B3");
        }

        // Reko: a decoder for XCore instruction BB6D at address 00100074 has not been implemented. (17)
        [Test]
        public void XCoreDis_BB6D()
        {
            AssertCode("@@@", "6DBB");
        }

        // Reko: a decoder for XCore instruction 6E41 at address 00100076 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6E41()
        {
            AssertCode("@@@", "416E");
        }

        // Reko: a decoder for XCore instruction 47AE at address 00100078 has not been implemented. (08)
        [Test]
        public void XCoreDis_47AE()
        {
            AssertCode("@@@", "AE47");
        }

        // Reko: a decoder for XCore instruction FEE1 at address 0010007A has not been implemented. (1F)
        [Test]
        public void XCoreDis_FEE1()
        {
            AssertCode("@@@", "E1FE");
        }

        // Reko: a decoder for XCore instruction B36C at address 0010007C has not been implemented. (16)
        [Test]
        public void XCoreDis_B36C()
        {
            AssertCode("@@@", "6CB3");
        }

        // Reko: a decoder for XCore instruction 7D3E at address 0010007E has not been implemented. (0F)
        [Test]
        public void XCoreDis_7D3E()
        {
            AssertCode("@@@", "3E7D");
        }

        // Reko: a decoder for XCore instruction B850 at address 00100080 has not been implemented. (17)
        [Test]
        public void XCoreDis_B850()
        {
            AssertCode("@@@", "50B8");
        }

        // Reko: a decoder for XCore instruction 75C2 at address 00100082 has not been implemented. (0E)
        [Test]
        public void XCoreDis_75C2()
        {
            AssertCode("@@@", "C275");
        }

        // Reko: a decoder for XCore instruction B554 at address 00100084 has not been implemented. (16)
        [Test]
        public void XCoreDis_B554()
        {
            AssertCode("@@@", "54B5");
        }

        // Reko: a decoder for XCore instruction 071D at address 00100086 has not been implemented. (00)
        [Test]
        public void XCoreDis_071D()
        {
            AssertCode("@@@", "1D07");
        }

        // Reko: a decoder for XCore instruction 58F9 at address 00100088 has not been implemented. (0B)
        [Test]
        public void XCoreDis_58F9()
        {
            AssertCode("@@@", "F958");
        }

        // Reko: a decoder for XCore instruction 9946 at address 0010008A has not been implemented. (13)
        [Test]
        public void XCoreDis_9946()
        {
            AssertCode("@@@", "4699");
        }

        // Reko: a decoder for XCore instruction 5DB6 at address 0010008C has not been implemented. (0B)
        [Test]
        public void XCoreDis_5DB6()
        {
            AssertCode("@@@", "B65D");
        }

        // Reko: a decoder for XCore instruction 4692 at address 0010008E has not been implemented. (08)
        [Test]
        public void XCoreDis_4692()
        {
            AssertCode("@@@", "9246");
        }

        // Reko: a decoder for XCore instruction 14F2 at address 00100090 has not been implemented. (02)
        [Test]
        public void XCoreDis_14F2()
        {
            AssertCode("@@@", "F214");
        }

        // Reko: a decoder for XCore instruction FEE4 at address 00100092 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FEE4()
        {
            AssertCode("@@@", "E4FE");
        }

        // Reko: a decoder for XCore instruction D37F at address 00100094 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D37F()
        {
            AssertCode("@@@", "7FD3");
        }

        // Reko: a decoder for XCore instruction 7880 at address 00100096 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7880()
        {
            AssertCode("@@@", "8078");
        }

        // Reko: a decoder for XCore instruction 7DB1 at address 00100098 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7DB1()
        {
            AssertCode("@@@", "B17D");
        }

        // Reko: a decoder for XCore instruction 173C at address 0010009A has not been implemented. (02)
        [Test]
        public void XCoreDis_173C()
        {
            AssertCode("@@@", "3C17");
        }

        // Reko: a decoder for XCore instruction 2F28 at address 0010009C has not been implemented. (05)
        [Test]
        public void XCoreDis_2F28()
        {
            AssertCode("@@@", "282F");
        }

        // Reko: a decoder for XCore instruction 4EC3 at address 0010009E has not been implemented. (09)
        [Test]
        public void XCoreDis_4EC3()
        {
            AssertCode("@@@", "C34E");
        }

        // Reko: a decoder for XCore instruction 4B76 at address 001000A0 has not been implemented. (09)
        [Test]
        public void XCoreDis_4B76()
        {
            AssertCode("@@@", "764B");
        }

        // Reko: a decoder for XCore instruction 3C11 at address 001000A4 has not been implemented. (07)
        [Test]
        public void XCoreDis_3C11()
        {
            AssertCode("@@@", "113C");
        }

        // Reko: a decoder for XCore instruction 2438 at address 001000A6 has not been implemented. (04)
        [Test]
        public void XCoreDis_2438()
        {
            AssertCode("@@@", "3824");
        }

        // Reko: a decoder for XCore instruction D0F9 at address 001000A8 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D0F9()
        {
            AssertCode("@@@", "F9D0");
        }

        // Reko: a decoder for XCore instruction 15B9 at address 001000AA has not been implemented. (02)
        [Test]
        public void XCoreDis_15B9()
        {
            AssertCode("@@@", "B915");
        }

        // Reko: a decoder for XCore instruction A774 at address 001000AC has not been implemented. (14)
        [Test]
        public void XCoreDis_A774()
        {
            AssertCode("@@@", "74A7");
        }

        // Reko: a decoder for XCore instruction 50B8 at address 001000AE has not been implemented. (0A)
        [Test]
        public void XCoreDis_50B8()
        {
            AssertCode("@@@", "B850");
        }

        // Reko: a decoder for XCore instruction 9BB4 at address 001000B0 has not been implemented. (13)
        [Test]
        public void XCoreDis_9BB4()
        {
            AssertCode("@@@", "B49B");
        }

        // Reko: a decoder for XCore instruction 570C at address 001000B2 has not been implemented. (0A)
        [Test]
        public void XCoreDis_570C()
        {
            AssertCode("@@@", "0C57");
        }


        // Reko: a decoder for XCore instruction 7DFD at address 001000B6 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7DFD()
        {
            AssertCode("@@@", "FD7D");
        }

        // Reko: a decoder for XCore instruction 4A37 at address 001000B8 has not been implemented. (09)
        [Test]
        public void XCoreDis_4A37()
        {
            AssertCode("@@@", "374A");
        }

        // Reko: a decoder for XCore instruction D6C3 at address 001000BA has not been implemented. (1A)
        [Test]
        public void XCoreDis_D6C3()
        {
            AssertCode("@@@", "C3D6");
        }

        // Reko: a decoder for XCore instruction 0578 at address 001000BC has not been implemented. (00)
        [Test]
        public void XCoreDis_0578()
        {
            AssertCode("@@@", "7805");
        }

        // Reko: a decoder for XCore instruction CADE at address 001000BE has not been implemented. (19)
        [Test]
        public void XCoreDis_CADE()
        {
            AssertCode("@@@", "DECA");
        }

        // Reko: a decoder for XCore instruction F795 at address 001000C0 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F795()
        {
            AssertCode("@@@", "95F7");
        }

        // Reko: a decoder for XCore instruction 6B23 at address 001000C2 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B23()
        {
            AssertCode("@@@", "236B");
        }

        // Reko: a decoder for XCore instruction 8F43 at address 001000C4 has not been implemented. (11)
        [Test]
        public void XCoreDis_8F43()
        {
            AssertCode("@@@", "438F");
        }

        // Reko: a decoder for XCore instruction B534 at address 001000C6 has not been implemented. (16)
        [Test]
        public void XCoreDis_B534()
        {
            AssertCode("@@@", "34B5");
        }

        // Reko: a decoder for XCore instruction BFDC at address 001000C8 has not been implemented. (17)
        [Test]
        public void XCoreDis_BFDC()
        {
            AssertCode("@@@", "DCBF");
        }

        // Reko: a decoder for XCore instruction AE04 at address 001000CA has not been implemented. (15)
        [Test]
        public void XCoreDis_AE04()
        {
            AssertCode("@@@", "04AE");
        }

        // Reko: a decoder for XCore instruction 6B19 at address 001000CC has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B19()
        {
            AssertCode("@@@", "196B");
        }

        // Reko: a decoder for XCore instruction 0A03 at address 001000CE has not been implemented. (01)
        [Test]
        public void XCoreDis_0A03()
        {
            AssertCode("@@@", "030A");
        }

        // Reko: a decoder for XCore instruction EBC5 at address 001000D0 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EBC5()
        {
            AssertCode("@@@", "C5EB");
        }

        // Reko: a decoder for XCore instruction 8D62 at address 001000D2 has not been implemented. (11)
        [Test]
        public void XCoreDis_8D62()
        {
            AssertCode("@@@", "628D");
        }

        // Reko: a decoder for XCore instruction 6B22 at address 001000D4 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B22()
        {
            AssertCode("@@@", "226B");
        }

        // Reko: a decoder for XCore instruction 3680 at address 001000D6 has not been implemented. (06)
        [Test]
        public void XCoreDis_3680()
        {
            AssertCode("@@@", "8036");
        }

        // Reko: a decoder for XCore instruction 9D4D at address 001000D8 has not been implemented. (13)
        [Test]
        public void XCoreDis_9D4D()
        {
            AssertCode("@@@", "4D9D");
        }

        // Reko: a decoder for XCore instruction C6CA at address 001000DA has not been implemented. (18)
        [Test]
        public void XCoreDis_C6CA()
        {
            AssertCode("@@@", "CAC6");
        }

        // Reko: a decoder for XCore instruction 6179 at address 001000DC has not been implemented. (0C)
        [Test]
        public void XCoreDis_6179()
        {
            AssertCode("@@@", "7961");
        }

        // Reko: a decoder for XCore instruction F4AC at address 001000DE has not been implemented. (1E)
        [Test]
        public void XCoreDis_F4AC()
        {
            AssertCode("@@@", "ACF4");
        }

        // Reko: a decoder for XCore instruction EFF1 at address 001000E0 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EFF1()
        {
            AssertCode("@@@", "F1EF");
        }

        // Reko: a decoder for XCore instruction 7C7F at address 001000E2 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7C7F()
        {
            AssertCode("@@@", "7F7C");
        }

        // Reko: a decoder for XCore instruction 4D83 at address 001000E4 has not been implemented. (09)
        [Test]
        public void XCoreDis_4D83()
        {
            AssertCode("@@@", "834D");
        }

        // Reko: a decoder for XCore instruction 240D at address 001000E6 has not been implemented. (04)
        [Test]
        public void XCoreDis_240D()
        {
            AssertCode("@@@", "0D24");
        }

        // Reko: a decoder for XCore instruction 5666 at address 001000E8 has not been implemented. (0A)
        [Test]
        public void XCoreDis_5666()
        {
            AssertCode("@@@", "6656");
        }

        // Reko: a decoder for XCore instruction 0E7B at address 001000EA has not been implemented. (01)
        [Test]
        public void XCoreDis_0E7B()
        {
            AssertCode("@@@", "7B0E");
        }

        // Reko: a decoder for XCore instruction 4F3A at address 001000EC has not been implemented. (09)
        [Test]
        public void XCoreDis_4F3A()
        {
            AssertCode("@@@", "3A4F");
        }

        // Reko: a decoder for XCore instruction CC63 at address 001000EE has not been implemented. (19)
        [Test]
        public void XCoreDis_CC63()
        {
            AssertCode("@@@", "63CC");
        }

        // Reko: a decoder for XCore instruction B946 at address 001000F0 has not been implemented. (17)
        [Test]
        public void XCoreDis_B946()
        {
            AssertCode("@@@", "46B9");
        }

        // Reko: a decoder for XCore instruction 8D11 at address 001000F2 has not been implemented. (11)
        [Test]
        public void XCoreDis_8D11()
        {
            AssertCode("@@@", "118D");
        }

        // Reko: a decoder for XCore instruction 51A2 at address 001000F4 has not been implemented. (0A)
        [Test]
        public void XCoreDis_51A2()
        {
            AssertCode("@@@", "A251");
        }

        // Reko: a decoder for XCore instruction 2AA7 at address 001000F6 has not been implemented. (05)
        [Test]
        public void XCoreDis_2AA7()
        {
            AssertCode("@@@", "A72A");
        }

        // Reko: a decoder for XCore instruction EC77 at address 001000F8 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EC77()
        {
            AssertCode("@@@", "77EC");
        }

        // Reko: a decoder for XCore instruction A61D at address 001000FA has not been implemented. (14)
        [Test]
        public void XCoreDis_A61D()
        {
            AssertCode("@@@", "1DA6");
        }

        // Reko: a decoder for XCore instruction 6DC5 at address 001000FC has not been implemented. (0D)
        [Test]
        public void XCoreDis_6DC5()
        {
            AssertCode("@@@", "C56D");
        }

        // Reko: a decoder for XCore instruction 7A3C at address 001000FE has not been implemented. (0F)
        [Test]
        public void XCoreDis_7A3C()
        {
            AssertCode("@@@", "3C7A");
        }

        // Reko: a decoder for XCore instruction 1013 at address 00100100 has not been implemented. (02)
        [Test]
        public void XCoreDis_1013()
        {
            AssertCode("@@@", "1310");
        }

        // Reko: a decoder for XCore instruction 2ABC at address 00100102 has not been implemented. (05)
        [Test]
        public void XCoreDis_2ABC()
        {
            AssertCode("@@@", "BC2A");
        }

        // Reko: a decoder for XCore instruction 87EB at address 00100104 has not been implemented. (10)
        [Test]
        public void XCoreDis_87EB()
        {
            AssertCode("@@@", "EB87");
        }

        // Reko: a decoder for XCore instruction 7786 at address 00100106 has not been implemented. (0E)
        [Test]
        public void XCoreDis_7786()
        {
            AssertCode("@@@", "8677");
        }

        // Reko: a decoder for XCore instruction 3DDE at address 00100108 has not been implemented. (07)
        [Test]
        public void XCoreDis_3DDE()
        {
            AssertCode("@@@", "DE3D");
        }

        // Reko: a decoder for XCore instruction CC27 at address 0010010A has not been implemented. (19)
        [Test]
        public void XCoreDis_CC27()
        {
            AssertCode("@@@", "27CC");
        }

        // Reko: a decoder for XCore instruction 71EF at address 0010010C has not been implemented. (0E)
        [Test]
        public void XCoreDis_71EF()
        {
            AssertCode("@@@", "EF71");
        }

        // Reko: a decoder for XCore instruction FEFC at address 0010010E has not been implemented. (1F)
        [Test]
        public void XCoreDis_FEFC()
        {
            AssertCode("@@@", "FCFE");
        }

        // Reko: a decoder for XCore instruction FD39 at address 00100110 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FD39()
        {
            AssertCode("@@@", "39FD");
        }

        // Reko: a decoder for XCore instruction BF80 at address 00100112 has not been implemented. (17)
        [Test]
        public void XCoreDis_BF80()
        {
            AssertCode("@@@", "80BF");
        }

        // Reko: a decoder for XCore instruction 1F50 at address 00100114 has not been implemented. (03)
        [Test]
        public void XCoreDis_1F50()
        {
            AssertCode("@@@", "501F");
        }

        // Reko: a decoder for XCore instruction A051 at address 00100116 has not been implemented. (14)
        [Test]
        public void XCoreDis_A051()
        {
            AssertCode("@@@", "51A0");
        }

        // Reko: a decoder for XCore instruction 5547 at address 00100118 has not been implemented. (0A)
        [Test]
        public void XCoreDis_5547()
        {
            AssertCode("@@@", "4755");
        }



        // Reko: a decoder for XCore instruction 662F at address 0010011C has not been implemented. (0C)
        [Test]
        public void XCoreDis_662F()
        {
            AssertCode("@@@", "2F66");
        }

        // Reko: a decoder for XCore instruction F85F at address 0010011E has not been implemented. (1F)
        [Test]
        public void XCoreDis_F85F()
        {
            AssertCode("@@@", "5FF8");
        }

        // Reko: a decoder for XCore instruction 011A at address 00100120 has not been implemented. (00)
        [Test]
        public void XCoreDis_011A()
        {
            AssertCode("@@@", "1A01");
        }

        // Reko: a decoder for XCore instruction 29FA at address 00100122 has not been implemented. (05)
        [Test]
        public void XCoreDis_29FA()
        {
            AssertCode("@@@", "FA29");
        }

        // Reko: a decoder for XCore instruction 3892 at address 00100124 has not been implemented. (07)
        [Test]
        public void XCoreDis_3892()
        {
            AssertCode("@@@", "9238");
        }

        // Reko: a decoder for XCore instruction BFE1 at address 00100126 has not been implemented. (17)
        [Test]
        public void XCoreDis_BFE1()
        {
            AssertCode("@@@", "E1BF");
        }

        // Reko: a decoder for XCore instruction 9933 at address 00100128 has not been implemented. (13)
        [Test]
        public void XCoreDis_9933()
        {
            AssertCode("@@@", "3399");
        }

        // Reko: a decoder for XCore instruction 64AE at address 0010012A has not been implemented. (0C)
        [Test]
        public void XCoreDis_64AE()
        {
            AssertCode("@@@", "AE64");
        }

        // Reko: a decoder for XCore instruction DB2A at address 0010012C has not been implemented. (1B)
        [Test]
        public void XCoreDis_DB2A()
        {
            AssertCode("@@@", "2ADB");
        }

        // Reko: a decoder for XCore instruction 063A at address 0010012E has not been implemented. (00)
        [Test]
        public void XCoreDis_063A()
        {
            AssertCode("@@@", "3A06");
        }

        // Reko: a decoder for XCore instruction 1FF0 at address 00100130 has not been implemented. (03)
        [Test]
        public void XCoreDis_1FF0()
        {
            AssertCode("@@@", "F01F");
        }

        // Reko: a decoder for XCore instruction C76C at address 00100132 has not been implemented. (18)
        [Test]
        public void XCoreDis_C76C()
        {
            AssertCode("@@@", "6CC7");
        }

        // Reko: a decoder for XCore instruction 7CED at address 00100134 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7CED()
        {
            AssertCode("@@@", "ED7C");
        }

        // Reko: a decoder for XCore instruction F42A at address 00100136 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F42A()
        {
            AssertCode("@@@", "2AF4");
        }

        // Reko: a decoder for XCore instruction 1CBF at address 00100138 has not been implemented. (03)
        [Test]
        public void XCoreDis_1CBF()
        {
            AssertCode("@@@", "BF1C");
        }

        // Reko: a decoder for XCore instruction 95E2 at address 0010013A has not been implemented. (12)
        [Test]
        public void XCoreDis_95E2()
        {
            AssertCode("@@@", "E295");
        }

        // Reko: a decoder for XCore instruction EE82 at address 0010013C has not been implemented. (1D)
        [Test]
        public void XCoreDis_EE82()
        {
            AssertCode("@@@", "82EE");
        }

        // Reko: a decoder for XCore instruction 7748 at address 0010013E has not been implemented. (0E)
        [Test]
        public void XCoreDis_7748()
        {
            AssertCode("@@@", "4877");
        }

        // Reko: a decoder for XCore instruction 2EDE at address 00100140 has not been implemented. (05)
        [Test]
        public void XCoreDis_2EDE()
        {
            AssertCode("@@@", "DE2E");
        }

        // Reko: a decoder for XCore instruction EEB2 at address 00100142 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EEB2()
        {
            AssertCode("@@@", "B2EE");
        }

        // Reko: a decoder for XCore instruction D276 at address 00100144 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D276()
        {
            AssertCode("@@@", "76D2");
        }

        // Reko: a decoder for XCore instruction 016B at address 00100146 has not been implemented. (00)
        [Test]
        public void XCoreDis_016B()
        {
            AssertCode("@@@", "6B01");
        }

        // Reko: a decoder for XCore instruction C11C at address 00100148 has not been implemented. (18)
        [Test]
        public void XCoreDis_C11C()
        {
            AssertCode("@@@", "1CC1");
        }

        // Reko: a decoder for XCore instruction B68C at address 0010014A has not been implemented. (16)
        [Test]
        public void XCoreDis_B68C()
        {
            AssertCode("@@@", "8CB6");
        }

        // Reko: a decoder for XCore instruction EC70 at address 0010014C has not been implemented. (1D)
        [Test]
        public void XCoreDis_EC70()
        {
            AssertCode("@@@", "70EC");
        }

        // Reko: a decoder for XCore instruction 6C75 at address 0010014E has not been implemented. (0D)
        [Test]
        public void XCoreDis_6C75()
        {
            AssertCode("@@@", "756C");
        }

        // Reko: a decoder for XCore instruction FE1B at address 00100150 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FE1B()
        {
            AssertCode("@@@", "1BFE");
        }

        // Reko: a decoder for XCore instruction 0FA7 at address 00100152 has not been implemented. (01)
        [Test]
        public void XCoreDis_0FA7()
        {
            AssertCode("@@@", "A70F");
        }

        // Reko: a decoder for XCore instruction 98FA at address 00100154 has not been implemented. (13)
        [Test]
        public void XCoreDis_98FA()
        {
            AssertCode("@@@", "FA98");
        }

        // Reko: a decoder for XCore instruction 9D0A at address 00100156 has not been implemented. (13)
        [Test]
        public void XCoreDis_9D0A()
        {
            AssertCode("@@@", "0A9D");
        }

        // Reko: a decoder for XCore instruction 06D7 at address 00100158 has not been implemented. (00)
        [Test]
        public void XCoreDis_06D7()
        {
            AssertCode("@@@", "D706");
        }

        // Reko: a decoder for XCore instruction 756A at address 0010015A has not been implemented. (0E)
        [Test]
        public void XCoreDis_756A()
        {
            AssertCode("@@@", "6A75");
        }

        // Reko: a decoder for XCore instruction 4C55 at address 0010015C has not been implemented. (09)
        [Test]
        public void XCoreDis_4C55()
        {
            AssertCode("@@@", "554C");
        }

        // Reko: a decoder for XCore instruction 443C at address 0010015E has not been implemented. (08)
        [Test]
        public void XCoreDis_443C()
        {
            AssertCode("@@@", "3C44");
        }

        // Reko: a decoder for XCore instruction 3751 at address 00100160 has not been implemented. (06)
        [Test]
        public void XCoreDis_3751()
        {
            AssertCode("@@@", "5137");
        }

        // Reko: a decoder for XCore instruction FB86 at address 00100162 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FB86()
        {
            AssertCode("@@@", "86FB");
        }

        // Reko: a decoder for XCore instruction 4B29 at address 00100164 has not been implemented. (09)
        [Test]
        public void XCoreDis_4B29()
        {
            AssertCode("@@@", "294B");
        }

        // Reko: a decoder for XCore instruction 1D8F at address 00100166 has not been implemented. (03)
        [Test]
        public void XCoreDis_1D8F()
        {
            AssertCode("@@@", "8F1D");
        }

        // Reko: a decoder for XCore instruction 6BB3 at address 00100168 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6BB3()
        {
            AssertCode("@@@", "B36B");
        }

        // Reko: a decoder for XCore instruction 2CAA at address 0010016A has not been implemented. (05)
        [Test]
        public void XCoreDis_2CAA()
        {
            AssertCode("@@@", "AA2C");
        }

        // Reko: a decoder for XCore instruction 73F0 at address 0010016C has not been implemented. (0E)
        [Test]
        public void XCoreDis_73F0()
        {
            AssertCode("@@@", "F073");
        }

        // Reko: a decoder for XCore instruction D384 at address 0010016E has not been implemented. (1A)
        [Test]
        public void XCoreDis_D384()
        {
            AssertCode("@@@", "84D3");
        }

        // Reko: a decoder for XCore instruction 76A6 at address 00100170 has not been implemented. (0E)
        [Test]
        public void XCoreDis_76A6()
        {
            AssertCode("@@@", "A676");
        }

        // Reko: a decoder for XCore instruction 8479 at address 00100172 has not been implemented. (10)
        [Test]
        public void XCoreDis_8479()
        {
            AssertCode("@@@", "7984");
        }

        // Reko: a decoder for XCore instruction 3847 at address 00100174 has not been implemented. (07)
        [Test]
        public void XCoreDis_3847()
        {
            AssertCode("@@@", "4738");
        }

        // Reko: a decoder for XCore instruction 457D at address 00100176 has not been implemented. (08)
        [Test]
        public void XCoreDis_457D()
        {
            AssertCode("@@@", "7D45");
        }

        // Reko: a decoder for XCore instruction 1423 at address 00100178 has not been implemented. (02)
        [Test]
        public void XCoreDis_1423()
        {
            AssertCode("@@@", "2314");
        }

        // Reko: a decoder for XCore instruction 7017 at address 0010017A has not been implemented. (0E)
        [Test]
        public void XCoreDis_7017()
        {
            AssertCode("@@@", "1770");
        }

        // Reko: a decoder for XCore instruction F668 at address 0010017C has not been implemented. (1E)
        [Test]
        public void XCoreDis_F668()
        {
            AssertCode("@@@", "68F6");
        }

        // Reko: a decoder for XCore instruction D0AB at address 0010017E has not been implemented. (1A)
        [Test]
        public void XCoreDis_D0AB()
        {
            AssertCode("@@@", "ABD0");
        }

        // Reko: a decoder for XCore instruction 4785 at address 00100180 has not been implemented. (08)
        [Test]
        public void XCoreDis_4785()
        {
            AssertCode("@@@", "8547");
        }

        // Reko: a decoder for XCore instruction 3964 at address 00100182 has not been implemented. (07)
        [Test]
        public void XCoreDis_3964()
        {
            AssertCode("@@@", "6439");
        }

        // Reko: a decoder for XCore instruction 7A65 at address 00100184 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7A65()
        {
            AssertCode("@@@", "657A");
        }

        // Reko: a decoder for XCore instruction CF43 at address 00100186 has not been implemented. (19)
        [Test]
        public void XCoreDis_CF43()
        {
            AssertCode("@@@", "43CF");
        }

        // Reko: a decoder for XCore instruction 896E at address 00100188 has not been implemented. (11)
        [Test]
        public void XCoreDis_896E()
        {
            AssertCode("@@@", "6E89");
        }

        // Reko: a decoder for XCore instruction 8F5C at address 0010018A has not been implemented. (11)
        [Test]
        public void XCoreDis_8F5C()
        {
            AssertCode("@@@", "5C8F");
        }

        // Reko: a decoder for XCore instruction DEED at address 0010018C has not been implemented. (1B)
        [Test]
        public void XCoreDis_DEED()
        {
            AssertCode("@@@", "EDDE");
        }

        // Reko: a decoder for XCore instruction 63AC at address 0010018E has not been implemented. (0C)
        [Test]
        public void XCoreDis_63AC()
        {
            AssertCode("@@@", "AC63");
        }

        // Reko: a decoder for XCore instruction 9782 at address 00100190 has not been implemented. (12)
        [Test]
        public void XCoreDis_9782()
        {
            AssertCode("@@@", "8297");
        }

        // Reko: a decoder for XCore instruction DFCE at address 00100192 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DFCE()
        {
            AssertCode("@@@", "CEDF");
        }

        // Reko: a decoder for XCore instruction B7D2 at address 00100194 has not been implemented. (16)
        [Test]
        public void XCoreDis_B7D2()
        {
            AssertCode("@@@", "D2B7");
        }

        // Reko: a decoder for XCore instruction 19FC at address 00100196 has not been implemented. (03)
        [Test]
        public void XCoreDis_19FC()
        {
            AssertCode("@@@", "FC19");
        }

        // Reko: a decoder for XCore instruction 41B9 at address 00100198 has not been implemented. (08)
        [Test]
        public void XCoreDis_41B9()
        {
            AssertCode("@@@", "B941");
        }

        // Reko: a decoder for XCore instruction 14D7 at address 0010019A has not been implemented. (02)
        [Test]
        public void XCoreDis_14D7()
        {
            AssertCode("@@@", "D714");
        }

        // Reko: a decoder for XCore instruction 1F34 at address 0010019C has not been implemented. (03)
        [Test]
        public void XCoreDis_1F34()
        {
            AssertCode("@@@", "341F");
        }

        // Reko: a decoder for XCore instruction BDB5 at address 0010019E has not been implemented. (17)
        [Test]
        public void XCoreDis_BDB5()
        {
            AssertCode("@@@", "B5BD");
        }

        // Reko: a decoder for XCore instruction DABF at address 001001A0 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DABF()
        {
            AssertCode("@@@", "BFDA");
        }

        // Reko: a decoder for XCore instruction A8A7 at address 001001A2 has not been implemented. (15)
        [Test]
        public void XCoreDis_A8A7()
        {
            AssertCode("@@@", "A7A8");
        }

        // Reko: a decoder for XCore instruction 4A0F at address 001001A4 has not been implemented. (09)
        [Test]
        public void XCoreDis_4A0F()
        {
            AssertCode("@@@", "0F4A");
        }

        // Reko: a decoder for XCore instruction 2C6D at address 001001A6 has not been implemented. (05)
        [Test]
        public void XCoreDis_2C6D()
        {
            AssertCode("@@@", "6D2C");
        }

        // Reko: a decoder for XCore instruction AA32 at address 001001A8 has not been implemented. (15)
        [Test]
        public void XCoreDis_AA32()
        {
            AssertCode("@@@", "32AA");
        }

        // Reko: a decoder for XCore instruction BE15 at address 001001AA has not been implemented. (17)
        [Test]
        public void XCoreDis_BE15()
        {
            AssertCode("@@@", "15BE");
        }

        // Reko: a decoder for XCore instruction EDDC at address 001001AC has not been implemented. (1D)
        [Test]
        public void XCoreDis_EDDC()
        {
            AssertCode("@@@", "DCED");
        }

        // Reko: a decoder for XCore instruction 4557 at address 001001AE has not been implemented. (08)
        [Test]
        public void XCoreDis_4557()
        {
            AssertCode("@@@", "5745");
        }

        // Reko: a decoder for XCore instruction B367 at address 001001B0 has not been implemented. (16)
        [Test]
        public void XCoreDis_B367()
        {
            AssertCode("@@@", "67B3");
        }

        // Reko: a decoder for XCore instruction D1EE at address 001001B2 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D1EE()
        {
            AssertCode("@@@", "EED1");
        }

        // Reko: a decoder for XCore instruction CDFD at address 001001B6 has not been implemented. (19)
        [Test]
        public void XCoreDis_CDFD()
        {
            AssertCode("@@@", "FDCD");
        }

        // Reko: a decoder for XCore instruction 4A4B at address 001001B8 has not been implemented. (09)
        [Test]
        public void XCoreDis_4A4B()
        {
            AssertCode("@@@", "4B4A");
        }

        // Reko: a decoder for XCore instruction 247F at address 001001BA has not been implemented. (04)
        [Test]
        public void XCoreDis_247F()
        {
            AssertCode("@@@", "7F24");
        }

        // Reko: a decoder for XCore instruction 2EA2 at address 001001BC has not been implemented. (05)
        [Test]
        public void XCoreDis_2EA2()
        {
            AssertCode("@@@", "A22E");
        }

        // Reko: a decoder for XCore instruction 4F9A at address 001001BE has not been implemented. (09)
        [Test]
        public void XCoreDis_4F9A()
        {
            AssertCode("@@@", "9A4F");
        }

        // Reko: a decoder for XCore instruction 9ED3 at address 001001C0 has not been implemented. (13)
        [Test]
        public void XCoreDis_9ED3()
        {
            AssertCode("@@@", "D39E");
        }

        // Reko: a decoder for XCore instruction 13CF at address 001001C2 has not been implemented. (02)


        // Reko: a decoder for XCore instruction 0337 at address 001001C4 has not been implemented. (00)
        [Test]
        public void XCoreDis_0337()
        {
            AssertCode("@@@", "3703");
        }

        // Reko: a decoder for XCore instruction 3753 at address 001001C6 has not been implemented. (06)
        [Test]
        public void XCoreDis_3753()
        {
            AssertCode("@@@", "5337");
        }

        // Reko: a decoder for XCore instruction A129 at address 001001C8 has not been implemented. (14)
        [Test]
        public void XCoreDis_A129()
        {
            AssertCode("@@@", "29A1");
        }

        // Reko: a decoder for XCore instruction 27AC at address 001001CA has not been implemented. (04)
        [Test]
        public void XCoreDis_27AC()
        {
            AssertCode("@@@", "AC27");
        }

        // Reko: a decoder for XCore instruction 3DA1 at address 001001CC has not been implemented. (07)
        [Test]
        public void XCoreDis_3DA1()
        {
            AssertCode("@@@", "A13D");
        }

        // Reko: a decoder for XCore instruction CC3D at address 001001CE has not been implemented. (19)
        [Test]
        public void XCoreDis_CC3D()
        {
            AssertCode("@@@", "3DCC");
        }

        // Reko: a decoder for XCore instruction 92E9 at address 001001D0 has not been implemented. (12)
        [Test]
        public void XCoreDis_92E9()
        {
            AssertCode("@@@", "E992");
        }

        // Reko: a decoder for XCore instruction 80AC at address 001001D2 has not been implemented. (10)
        [Test]
        public void XCoreDis_80AC()
        {
            AssertCode("@@@", "AC80");
        }

        // Reko: a decoder for XCore instruction E431 at address 001001D4 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E431()
        {
            AssertCode("@@@", "31E4");
        }

        // Reko: a decoder for XCore instruction F396 at address 001001D6 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F396()
        {
            AssertCode("@@@", "96F3");
        }

        // Reko: a decoder for XCore instruction D9DC at address 001001D8 has not been implemented. (1B)
        [Test]
        public void XCoreDis_D9DC()
        {
            AssertCode("@@@", "DCD9");
        }

        // Reko: a decoder for XCore instruction C55D at address 001001DA has not been implemented. (18)
        [Test]
        public void XCoreDis_C55D()
        {
            AssertCode("@@@", "5DC5");
        }

        // Reko: a decoder for XCore instruction 49CB at address 001001DC has not been implemented. (09)
        [Test]
        public void XCoreDis_49CB()
        {
            AssertCode("@@@", "CB49");
        }

        // Reko: a decoder for XCore instruction 038A at address 001001DE has not been implemented. (00)
        [Test]
        public void XCoreDis_038A()
        {
            AssertCode("@@@", "8A03");
        }

        // Reko: a decoder for XCore instruction C60F at address 001001E0 has not been implemented. (18)
        [Test]
        public void XCoreDis_C60F()
        {
            AssertCode("@@@", "0FC6");
        }

        // Reko: a decoder for XCore instruction 3DEB at address 001001E2 has not been implemented. (07)
        [Test]
        public void XCoreDis_3DEB()
        {
            AssertCode("@@@", "EB3D");
        }

        // Reko: a decoder for XCore instruction 441D at address 001001E4 has not been implemented. (08)
        [Test]
        public void XCoreDis_441D()
        {
            AssertCode("@@@", "1D44");
        }

        // Reko: a decoder for XCore instruction 640D at address 001001E6 has not been implemented. (0C)
        [Test]
        public void XCoreDis_640D()
        {
            AssertCode("@@@", "0D64");
        }

        // Reko: a decoder for XCore instruction B660 at address 001001E8 has not been implemented. (16)
        [Test]
        public void XCoreDis_B660()
        {
            AssertCode("@@@", "60B6");
        }

        // Reko: a decoder for XCore instruction 85A7 at address 001001EA has not been implemented. (10)
        [Test]
        public void XCoreDis_85A7()
        {
            AssertCode("@@@", "A785");
        }

        // Reko: a decoder for XCore instruction D61F at address 001001EC has not been implemented. (1A)
        [Test]
        public void XCoreDis_D61F()
        {
            AssertCode("@@@", "1FD6");
        }

        // Reko: a decoder for XCore instruction 0D2B at address 001001EE has not been implemented. (01)
        [Test]
        public void XCoreDis_0D2B()
        {
            AssertCode("@@@", "2B0D");
        }

        // Reko: a decoder for XCore instruction B30C at address 001001F0 has not been implemented. (16)
        [Test]
        public void XCoreDis_B30C()
        {
            AssertCode("@@@", "0CB3");
        }

        // Reko: a decoder for XCore instruction 103A at address 001001F2 has not been implemented. (02)
        [Test]
        public void XCoreDis_103A()
        {
            AssertCode("@@@", "3A10");
        }

        // Reko: a decoder for XCore instruction 1982 at address 001001F4 has not been implemented. (03)
        [Test]
        public void XCoreDis_1982()
        {
            AssertCode("@@@", "8219");
        }

        // Reko: a decoder for XCore instruction EE1E at address 001001F6 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EE1E()
        {
            AssertCode("@@@", "1EEE");
        }

        // Reko: a decoder for XCore instruction DC08 at address 001001F8 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DC08()
        {
            AssertCode("@@@", "08DC");
        }

        // Reko: a decoder for XCore instruction 5E36 at address 001001FA has not been implemented. (0B)
        [Test]
        public void XCoreDis_5E36()
        {
            AssertCode("@@@", "365E");
        }

        // Reko: a decoder for XCore instruction 8EA6 at address 001001FC has not been implemented. (11)
        [Test]
        public void XCoreDis_8EA6()
        {
            AssertCode("@@@", "A68E");
        }

        // Reko: a decoder for XCore instruction DF6C at address 001001FE has not been implemented. (1B)
        [Test]
        public void XCoreDis_DF6C()
        {
            AssertCode("@@@", "6CDF");
        }

        // Reko: a decoder for XCore instruction A817 at address 00100200 has not been implemented. (15)
        [Test]
        public void XCoreDis_A817()
        {
            AssertCode("@@@", "17A8");
        }

        // Reko: a decoder for XCore instruction DB17 at address 00100202 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DB17()
        {
            AssertCode("@@@", "17DB");
        }

        // Reko: a decoder for XCore instruction 0052 at address 00100204 has not been implemented. (00)
        [Test]
        public void XCoreDis_0052()
        {
            AssertCode("@@@", "5200");
        }

        // Reko: a decoder for XCore instruction A5AE at address 00100206 has not been implemented. (14)
        [Test]
        public void XCoreDis_A5AE()
        {
            AssertCode("@@@", "AEA5");
        }

        // Reko: a decoder for XCore instruction 4884 at address 00100208 has not been implemented. (09)
        [Test]
        public void XCoreDis_4884()
        {
            AssertCode("@@@", "8448");
        }

        // Reko: a decoder for XCore instruction 7A20 at address 0010020A has not been implemented. (0F)
        [Test]
        public void XCoreDis_7A20()
        {
            AssertCode("@@@", "207A");
        }

        // Reko: a decoder for XCore instruction 103D at address 0010020C has not been implemented. (02)
        [Test]
        public void XCoreDis_103D()
        {
            AssertCode("@@@", "3D10");
        }

        // Reko: a decoder for XCore instruction 06D4 at address 0010020E has not been implemented. (00)
        [Test]
        public void XCoreDis_06D4()
        {
            AssertCode("@@@", "D406");
        }

        // Reko: a decoder for XCore instruction 4FAD at address 00100210 has not been implemented. (09)
        [Test]
        public void XCoreDis_4FAD()
        {
            AssertCode("@@@", "AD4F");
        }

        // Reko: a decoder for XCore instruction 17B8 at address 00100212 has not been implemented. (02)
        [Test]
        public void XCoreDis_17B8()
        {
            AssertCode("@@@", "B817");
        }

        // Reko: a decoder for XCore instruction 7A0F at address 00100214 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7A0F()
        {
            AssertCode("@@@", "0F7A");
        }

        // Reko: a decoder for XCore instruction F681 at address 00100216 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F681()
        {
            AssertCode("@@@", "81F6");
        }

        // Reko: a decoder for XCore instruction FDA8 at address 00100218 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FDA8()
        {
            AssertCode("@@@", "A8FD");
        }

        // Reko: a decoder for XCore instruction 4135 at address 0010021A has not been implemented. (08)
        [Test]
        public void XCoreDis_4135()
        {
            AssertCode("@@@", "3541");
        }

        // Reko: a decoder for XCore instruction AF0D at address 0010021C has not been implemented. (15)
        [Test]
        public void XCoreDis_AF0D()
        {
            AssertCode("@@@", "0DAF");
        }

        // Reko: a decoder for XCore instruction D2BD at address 0010021E has not been implemented. (1A)
        [Test]
        public void XCoreDis_D2BD()
        {
            AssertCode("@@@", "BDD2");
        }

        // Reko: a decoder for XCore instruction C84A at address 00100220 has not been implemented. (19)
        [Test]
        public void XCoreDis_C84A()
        {
            AssertCode("@@@", "4AC8");
        }

        // Reko: a decoder for XCore instruction 766D at address 00100222 has not been implemented. (0E)
        [Test]
        public void XCoreDis_766D()
        {
            AssertCode("@@@", "6D76");
        }

        // Reko: a decoder for XCore instruction 50BE at address 00100224 has not been implemented. (0A)
        [Test]
        public void XCoreDis_50BE()
        {
            AssertCode("@@@", "BE50");
        }

        // Reko: a decoder for XCore instruction 0CBB at address 00100226 has not been implemented. (01)
        [Test]
        public void XCoreDis_0CBB()
        {
            AssertCode("@@@", "BB0C");
        }

        // Reko: a decoder for XCore instruction 9405 at address 00100228 has not been implemented. (12)
        [Test]
        public void XCoreDis_9405()
        {
            AssertCode("@@@", "0594");
        }

        // Reko: a decoder for XCore instruction 3A8B at address 0010022A has not been implemented. (07)
        [Test]
        public void XCoreDis_3A8B()
        {
            AssertCode("@@@", "8B3A");
        }

        // Reko: a decoder for XCore instruction A4F9 at address 0010022C has not been implemented. (14)
        [Test]
        public void XCoreDis_A4F9()
        {
            AssertCode("@@@", "F9A4");
        }

        // Reko: a decoder for XCore instruction F7B1 at address 0010022E has not been implemented. (1E)
        [Test]
        public void XCoreDis_F7B1()
        {
            AssertCode("@@@", "B1F7");
        }

        // Reko: a decoder for XCore instruction 2F08 at address 00100230 has not been implemented. (05)
        [Test]
        public void XCoreDis_2F08()
        {
            AssertCode("@@@", "082F");
        }

        // Reko: a decoder for XCore instruction 56B0 at address 00100232 has not been implemented. (0A)
        [Test]
        public void XCoreDis_56B0()
        {
            AssertCode("@@@", "B056");
        }

        // Reko: a decoder for XCore instruction 54D5 at address 00100234 has not been implemented. (0A)
        [Test]
        public void XCoreDis_54D5()
        {
            AssertCode("@@@", "D554");
        }

        // Reko: a decoder for XCore instruction 9DD0 at address 00100236 has not been implemented. (13)
        [Test]
        public void XCoreDis_9DD0()
        {
            AssertCode("@@@", "D09D");
        }

        // Reko: a decoder for XCore instruction 2127 at address 00100238 has not been implemented. (04)
        [Test]
        public void XCoreDis_2127()
        {
            AssertCode("@@@", "2721");
        }

        // Reko: a decoder for XCore instruction 5432 at address 0010023A has not been implemented. (0A)
        [Test]
        public void XCoreDis_5432()
        {
            AssertCode("@@@", "3254");
        }

        // Reko: a decoder for XCore instruction 6CCA at address 0010023C has not been implemented. (0D)
        [Test]
        public void XCoreDis_6CCA()
        {
            AssertCode("@@@", "CA6C");
        }

        // Reko: a decoder for XCore instruction D497 at address 0010023E has not been implemented. (1A)
        [Test]
        public void XCoreDis_D497()
        {
            AssertCode("@@@", "97D4");
        }

        // Reko: a decoder for XCore instruction 4E8A at address 00100240 has not been implemented. (09)
        [Test]
        public void XCoreDis_4E8A()
        {
            AssertCode("@@@", "8A4E");
        }

        // Reko: a decoder for XCore instruction 752F at address 00100242 has not been implemented. (0E)
        [Test]
        public void XCoreDis_752F()
        {
            AssertCode("@@@", "2F75");
        }

        // Reko: a decoder for XCore instruction 5DA3 at address 00100244 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5DA3()
        {
            AssertCode("@@@", "A35D");
        }

        // Reko: a decoder for XCore instruction 5D48 at address 00100246 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5D48()
        {
            AssertCode("@@@", "485D");
        }

        // Reko: a decoder for XCore instruction AC94 at address 00100248 has not been implemented. (15)
        [Test]
        public void XCoreDis_AC94()
        {
            AssertCode("@@@", "94AC");
        }

        // Reko: a decoder for XCore instruction 7B12 at address 0010024A has not been implemented. (0F)
        [Test]
        public void XCoreDis_7B12()
        {
            AssertCode("@@@", "127B");
        }

        // Reko: a decoder for XCore instruction 47EE at address 0010024C has not been implemented. (08)
        [Test]
        public void XCoreDis_47EE()
        {
            AssertCode("@@@", "EE47");
        }

        // Reko: a decoder for XCore instruction 04FC at address 0010024E has not been implemented. (00)
        [Test]
        public void XCoreDis_04FC()
        {
            AssertCode("@@@", "FC04");
        }

        // Reko: a decoder for XCore instruction 3E4C at address 00100250 has not been implemented. (07)
        [Test]
        public void XCoreDis_3E4C()
        {
            AssertCode("@@@", "4C3E");
        }

        // Reko: a decoder for XCore instruction DE39 at address 00100252 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DE39()
        {
            AssertCode("@@@", "39DE");
        }

        // Reko: a decoder for XCore instruction 67FF at address 00100254 has not been implemented. (0C)
        [Test]
        public void XCoreDis_67FF()
        {
            AssertCode("@@@", "FF67");
        }

        // Reko: a decoder for XCore instruction F6FD at address 00100256 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F6FD()
        {
            AssertCode("@@@", "FDF6");
        }

        // Reko: a decoder for XCore instruction D0F8 at address 00100258 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D0F8()
        {
            AssertCode("@@@", "F8D0");
        }

        // Reko: a decoder for XCore instruction 9C4F at address 0010025A has not been implemented. (13)
        [Test]
        public void XCoreDis_9C4F()
        {
            AssertCode("@@@", "4F9C");
        }

        // Reko: a decoder for XCore instruction 661D at address 0010025C has not been implemented. (0C)
        [Test]
        public void XCoreDis_661D()
        {
            AssertCode("@@@", "1D66");
        }

        // Reko: a decoder for XCore instruction 9842 at address 0010025E has not been implemented. (13)
        [Test]
        public void XCoreDis_9842()
        {
            AssertCode("@@@", "4298");
        }

        // Reko: a decoder for XCore instruction B6FC at address 00100260 has not been implemented. (16)
        [Test]
        public void XCoreDis_B6FC()
        {
            AssertCode("@@@", "FCB6");
        }

        // Reko: a decoder for XCore instruction ABAF at address 00100262 has not been implemented. (15)
        [Test]
        public void XCoreDis_ABAF()
        {
            AssertCode("@@@", "AFAB");
        }

        // Reko: a decoder for XCore instruction 3C74 at address 00100264 has not been implemented. (07)
        [Test]
        public void XCoreDis_3C74()
        {
            AssertCode("@@@", "743C");
        }

        // Reko: a decoder for XCore instruction AA54 at address 00100266 has not been implemented. (15)
        [Test]
        public void XCoreDis_AA54()
        {
            AssertCode("@@@", "54AA");
        }

        // Reko: a decoder for XCore instruction 53E7 at address 00100268 has not been implemented. (0A)
        [Test]
        public void XCoreDis_53E7()
        {
            AssertCode("@@@", "E753");
        }

        // Reko: a decoder for XCore instruction 28C1 at address 0010026A has not been implemented. (05)
        [Test]
        public void XCoreDis_28C1()
        {
            AssertCode("@@@", "C128");
        }

        // Reko: a decoder for XCore instruction 5542 at address 0010026C has not been implemented. (0A)
        [Test]
        public void XCoreDis_5542()
        {
            AssertCode("@@@", "4255");
        }

        // Reko: a decoder for XCore instruction E0AF at address 0010026E has not been implemented. (1C)
        [Test]
        public void XCoreDis_E0AF()
        {
            AssertCode("@@@", "AFE0");
        }

        // Reko: a decoder for XCore instruction 2E25 at address 00100270 has not been implemented. (05)
        [Test]
        public void XCoreDis_2E25()
        {
            AssertCode("@@@", "252E");
        }

        // Reko: a decoder for XCore instruction 8C08 at address 00100272 has not been implemented. (11)
        [Test]
        public void XCoreDis_8C08()
        {
            AssertCode("@@@", "088C");
        }

        // Reko: a decoder for XCore instruction B933 at address 00100274 has not been implemented. (17)
        [Test]
        public void XCoreDis_B933()
        {
            AssertCode("@@@", "33B9");
        }

        // Reko: a decoder for XCore instruction 23D5 at address 00100276 has not been implemented. (04)
        [Test]
        public void XCoreDis_23D5()
        {
            AssertCode("@@@", "D523");
        }

        // Reko: a decoder for XCore instruction 3951 at address 00100278 has not been implemented. (07)
        [Test]
        public void XCoreDis_3951()
        {
            AssertCode("@@@", "5139");
        }

        // Reko: a decoder for XCore instruction D27C at address 0010027A has not been implemented. (1A)
        [Test]
        public void XCoreDis_D27C()
        {
            AssertCode("@@@", "7CD2");
        }

        // Reko: a decoder for XCore instruction AC0E at address 0010027C has not been implemented. (15)
        [Test]
        public void XCoreDis_AC0E()
        {
            AssertCode("@@@", "0EAC");
        }

        // Reko: a decoder for XCore instruction 2E3F at address 0010027E has not been implemented. (05)
        [Test]
        public void XCoreDis_2E3F()
        {
            AssertCode("@@@", "3F2E");
        }

        // Reko: a decoder for XCore instruction 796A at address 00100280 has not been implemented. (0F)
        [Test]
        public void XCoreDis_796A()
        {
            AssertCode("@@@", "6A79");
        }

        // Reko: a decoder for XCore instruction 377E at address 00100282 has not been implemented. (06)
        [Test]
        public void XCoreDis_377E()
        {
            AssertCode("@@@", "7E37");
        }

        // Reko: a decoder for XCore instruction 5198 at address 00100284 has not been implemented. (0A)
        [Test]
        public void XCoreDis_5198()
        {
            AssertCode("@@@", "9851");
        }

        // Reko: a decoder for XCore instruction 108F at address 00100286 has not been implemented. (02)
        [Test]
        public void XCoreDis_108F()
        {
            AssertCode("@@@", "8F10");
        }

        // Reko: a decoder for XCore instruction 8EEA at address 00100288 has not been implemented. (11)
        [Test]
        public void XCoreDis_8EEA()
        {
            AssertCode("@@@", "EA8E");
        }

        // Reko: a decoder for XCore instruction ACF7 at address 0010028A has not been implemented. (15)
        [Test]
        public void XCoreDis_ACF7()
        {
            AssertCode("@@@", "F7AC");
        }

        // Reko: a decoder for XCore instruction D4A5 at address 0010028C has not been implemented. (1A)
        [Test]
        public void XCoreDis_D4A5()
        {
            AssertCode("@@@", "A5D4");
        }

        // Reko: a decoder for XCore instruction A2B4 at address 0010028E has not been implemented. (14)
        [Test]
        public void XCoreDis_A2B4()
        {
            AssertCode("@@@", "B4A2");
        }

        // Reko: a decoder for XCore instruction 6F20 at address 00100290 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6F20()
        {
            AssertCode("@@@", "206F");
        }

        // Reko: a decoder for XCore instruction EF77 at address 00100292 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EF77()
        {
            AssertCode("@@@", "77EF");
        }

        // Reko: a decoder for XCore instruction B55D at address 00100294 has not been implemented. (16)
        [Test]
        public void XCoreDis_B55D()
        {
            AssertCode("@@@", "5DB5");
        }

        // Reko: a decoder for XCore instruction 4365 at address 00100296 has not been implemented. (08)
        [Test]
        public void XCoreDis_4365()
        {
            AssertCode("@@@", "6543");
        }

        // Reko: a decoder for XCore instruction 8BE1 at address 00100298 has not been implemented. (11)
        [Test]
        public void XCoreDis_8BE1()
        {
            AssertCode("@@@", "E18B");
        }

        // Reko: a decoder for XCore instruction 3B5A at address 0010029A has not been implemented. (07)
        [Test]
        public void XCoreDis_3B5A()
        {
            AssertCode("@@@", "5A3B");
        }

        // Reko: a decoder for XCore instruction 81BF at address 0010029C has not been implemented. (10)
        [Test]
        public void XCoreDis_81BF()
        {
            AssertCode("@@@", "BF81");
        }

        // Reko: a decoder for XCore instruction 3A9C at address 0010029E has not been implemented. (07)
        [Test]
        public void XCoreDis_3A9C()
        {
            AssertCode("@@@", "9C3A");
        }

        // Reko: a decoder for XCore instruction 9313 at address 001002A0 has not been implemented. (12)
        [Test]
        public void XCoreDis_9313()
        {
            AssertCode("@@@", "1393");
        }

        // Reko: a decoder for XCore instruction C8BE at address 001002A2 has not been implemented. (19)
        [Test]
        public void XCoreDis_C8BE()
        {
            AssertCode("@@@", "BEC8");
        }

        // Reko: a decoder for XCore instruction 77D6 at address 001002A4 has not been implemented. (0E)
        [Test]
        public void XCoreDis_77D6()
        {
            AssertCode("@@@", "D677");
        }

        // Reko: a decoder for XCore instruction D347 at address 001002A6 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D347()
        {
            AssertCode("@@@", "47D3");
        }

        // Reko: a decoder for XCore instruction F89E at address 001002A8 has not been implemented. (1F)
        [Test]
        public void XCoreDis_F89E()
        {
            AssertCode("@@@", "9EF8");
        }

        // Reko: a decoder for XCore instruction A4A1 at address 001002AA has not been implemented. (14)
        [Test]
        public void XCoreDis_A4A1()
        {
            AssertCode("@@@", "A1A4");
        }

        // Reko: a decoder for XCore instruction 29C2 at address 001002AC has not been implemented. (05)
        [Test]
        public void XCoreDis_29C2()
        {
            AssertCode("@@@", "C229");
        }

        // Reko: a decoder for XCore instruction 7C7E at address 001002AE has not been implemented. (0F)
        [Test]
        public void XCoreDis_7C7E()
        {
            AssertCode("@@@", "7E7C");
        }

        // Reko: a decoder for XCore instruction DA85 at address 001002B0 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DA85()
        {
            AssertCode("@@@", "85DA");
        }

        // Reko: a decoder for XCore instruction 9EB2 at address 001002B2 has not been implemented. (13)
        [Test]
        public void XCoreDis_9EB2()
        {
            AssertCode("@@@", "B29E");
        }

        // Reko: a decoder for XCore instruction 4F35 at address 001002B4 has not been implemented. (09)
        [Test]
        public void XCoreDis_4F35()
        {
            AssertCode("@@@", "354F");
        }

        // Reko: a decoder for XCore instruction B4D0 at address 001002B6 has not been implemented. (16)
        [Test]
        public void XCoreDis_B4D0()
        {
            AssertCode("@@@", "D0B4");
        }

        // Reko: a decoder for XCore instruction 3B14 at address 001002B8 has not been implemented. (07)
        [Test]
        public void XCoreDis_3B14()
        {
            AssertCode("@@@", "143B");
        }

        // Reko: a decoder for XCore instruction 0C56 at address 001002BA has not been implemented. (01)
        [Test]
        public void XCoreDis_0C56()
        {
            AssertCode("@@@", "560C");
        }

        // Reko: a decoder for XCore instruction 54F7 at address 001002BC has not been implemented. (0A)
        [Test]
        public void XCoreDis_54F7()
        {
            AssertCode("@@@", "F754");
        }

        // Reko: a decoder for XCore instruction 6850 at address 001002BE has not been implemented. (0D)
        [Test]
        public void XCoreDis_6850()
        {
            AssertCode("@@@", "5068");
        }

        // Reko: a decoder for XCore instruction BCF1 at address 001002C0 has not been implemented. (17)
        [Test]
        public void XCoreDis_BCF1()
        {
            AssertCode("@@@", "F1BC");
        }

        // Reko: a decoder for XCore instruction 1198 at address 001002C2 has not been implemented. (02)
        [Test]
        public void XCoreDis_1198()
        {
            AssertCode("@@@", "9811");
        }

        // Reko: a decoder for XCore instruction EB15 at address 001002C4 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EB15()
        {
            AssertCode("@@@", "15EB");
        }

        // Reko: a decoder for XCore instruction A9CC at address 001002C6 has not been implemented. (15)
        [Test]
        public void XCoreDis_A9CC()
        {
            AssertCode("@@@", "CCA9");
        }

        // Reko: a decoder for XCore instruction A327 at address 001002C8 has not been implemented. (14)
        [Test]
        public void XCoreDis_A327()
        {
            AssertCode("@@@", "27A3");
        }

        // Reko: a decoder for XCore instruction 6550 at address 001002CA has not been implemented. (0C)
        [Test]
        public void XCoreDis_6550()
        {
            AssertCode("@@@", "5065");
        }

        // Reko: a decoder for XCore instruction C013 at address 001002CC has not been implemented. (18)
        [Test]
        public void XCoreDis_C013()
        {
            AssertCode("@@@", "13C0");
        }

        // Reko: a decoder for XCore instruction B780 at address 001002CE has not been implemented. (16)
        [Test]
        public void XCoreDis_B780()
        {
            AssertCode("@@@", "80B7");
        }

        // Reko: a decoder for XCore instruction DD0D at address 001002D0 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DD0D()
        {
            AssertCode("@@@", "0DDD");
        }

        // Reko: a decoder for XCore instruction E5B5 at address 001002D2 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E5B5()
        {
            AssertCode("@@@", "B5E5");
        }

        // Reko: a decoder for XCore instruction FDCE at address 001002D4 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FDCE()
        {
            AssertCode("@@@", "CEFD");
        }

        // Reko: a decoder for XCore instruction C305 at address 001002D6 has not been implemented. (18)
        [Test]
        public void XCoreDis_C305()
        {
            AssertCode("@@@", "05C3");
        }

        // Reko: a decoder for XCore instruction 09C3 at address 001002D8 has not been implemented. (01)
        [Test]
        public void XCoreDis_09C3()
        {
            AssertCode("@@@", "C309");
        }

        // Reko: a decoder for XCore instruction 9BB3 at address 001002DA has not been implemented. (13)
        [Test]
        public void XCoreDis_9BB3()
        {
            AssertCode("@@@", "B39B");
        }

        // Reko: a decoder for XCore instruction 3B21 at address 001002DC has not been implemented. (07)
        [Test]
        public void XCoreDis_3B21()
        {
            AssertCode("@@@", "213B");
        }

        // Reko: a decoder for XCore instruction 4ADC at address 001002DE has not been implemented. (09)
        [Test]
        public void XCoreDis_4ADC()
        {
            AssertCode("@@@", "DC4A");
        }

        // Reko: a decoder for XCore instruction 39A7 at address 001002E0 has not been implemented. (07)
        [Test]
        public void XCoreDis_39A7()
        {
            AssertCode("@@@", "A739");
        }

        // Reko: a decoder for XCore instruction 05B2 at address 001002E2 has not been implemented. (00)
        [Test]
        public void XCoreDis_05B2()
        {
            AssertCode("@@@", "B205");
        }

        // Reko: a decoder for XCore instruction 6C91 at address 001002E4 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6C91()
        {
            AssertCode("@@@", "916C");
        }

        // Reko: a decoder for XCore instruction 9966 at address 001002E6 has not been implemented. (13)
        [Test]
        public void XCoreDis_9966()
        {
            AssertCode("@@@", "6699");
        }

        // Reko: a decoder for XCore instruction 090D at address 001002E8 has not been implemented. (01)
        [Test]
        public void XCoreDis_090D()
        {
            AssertCode("@@@", "0D09");
        }

        // Reko: a decoder for XCore instruction 9177 at address 001002EA has not been implemented. (12)
        [Test]
        public void XCoreDis_9177()
        {
            AssertCode("@@@", "7791");
        }

        // Reko: a decoder for XCore instruction 6BFF at address 001002EC has not been implemented. (0D)
        [Test]
        public void XCoreDis_6BFF()
        {
            AssertCode("@@@", "FF6B");
        }

        // Reko: a decoder for XCore instruction 53A0 at address 001002EE has not been implemented. (0A)
        [Test]
        public void XCoreDis_53A0()
        {
            AssertCode("@@@", "A053");
        }

        // Reko: a decoder for XCore instruction 9EBA at address 001002F0 has not been implemented. (13)
        [Test]
        public void XCoreDis_9EBA()
        {
            AssertCode("@@@", "BA9E");
        }

        // Reko: a decoder for XCore instruction 1AFE at address 001002F2 has not been implemented. (03)
        [Test]
        public void XCoreDis_1AFE()
        {
            AssertCode("@@@", "FE1A");
        }

        // Reko: a decoder for XCore instruction 6B9E at address 001002F4 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B9E()
        {
            AssertCode("@@@", "9E6B");
        }

        // Reko: a decoder for XCore instruction F399 at address 001002F6 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F399()
        {
            AssertCode("@@@", "99F3");
        }

        // Reko: a decoder for XCore instruction D4B6 at address 001002F8 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D4B6()
        {
            AssertCode("@@@", "B6D4");
        }

        // Reko: a decoder for XCore instruction 0B4E at address 001002FA has not been implemented. (01)
        [Test]
        public void XCoreDis_0B4E()
        {
            AssertCode("@@@", "4E0B");
        }

        // Reko: a decoder for XCore instruction 3038 at address 001002FC has not been implemented. (06)
        [Test]
        public void XCoreDis_3038()
        {
            AssertCode("@@@", "3830");
        }

        // Reko: a decoder for XCore instruction EB87 at address 001002FE has not been implemented. (1D)
        [Test]
        public void XCoreDis_EB87()
        {
            AssertCode("@@@", "87EB");
        }

        // Reko: a decoder for XCore instruction 06C7 at address 00100300 has not been implemented. (00)
        [Test]
        public void XCoreDis_06C7()
        {
            AssertCode("@@@", "C706");
        }

        // Reko: a decoder for XCore instruction DABD at address 00100302 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DABD()
        {
            AssertCode("@@@", "BDDA");
        }

        // Reko: a decoder for XCore instruction 7A0D at address 00100304 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7A0D()
        {
            AssertCode("@@@", "0D7A");
        }

        // Reko: a decoder for XCore instruction A126 at address 00100306 has not been implemented. (14)
        [Test]
        public void XCoreDis_A126()
        {
            AssertCode("@@@", "26A1");
        }

        // Reko: a decoder for XCore instruction 1C76 at address 00100308 has not been implemented. (03)
        [Test]
        public void XCoreDis_1C76()
        {
            AssertCode("@@@", "761C");
        }

        // Reko: a decoder for XCore instruction C5D7 at address 0010030A has not been implemented. (18)
        [Test]
        public void XCoreDis_C5D7()
        {
            AssertCode("@@@", "D7C5");
        }

        // Reko: a decoder for XCore instruction 7486 at address 0010030C has not been implemented. (0E)
        [Test]
        public void XCoreDis_7486()
        {
            AssertCode("@@@", "8674");
        }

        // Reko: a decoder for XCore instruction 58C4 at address 0010030E has not been implemented. (0B)
        [Test]
        public void XCoreDis_58C4()
        {
            AssertCode("@@@", "C458");
        }

        // Reko: a decoder for XCore instruction 6069 at address 00100310 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6069()
        {
            AssertCode("@@@", "6960");
        }

        // Reko: a decoder for XCore instruction 82E1 at address 00100312 has not been implemented. (10)
        [Test]
        public void XCoreDis_82E1()
        {
            AssertCode("@@@", "E182");
        }

        // Reko: a decoder for XCore instruction C13D at address 00100314 has not been implemented. (18)
        [Test]
        public void XCoreDis_C13D()
        {
            AssertCode("@@@", "3DC1");
        }

        // Reko: a decoder for XCore instruction 3BAB at address 00100316 has not been implemented. (07)
        [Test]
        public void XCoreDis_3BAB()
        {
            AssertCode("@@@", "AB3B");
        }

        // Reko: a decoder for XCore instruction BF9F at address 00100318 has not been implemented. (17)
        [Test]
        public void XCoreDis_BF9F()
        {
            AssertCode("@@@", "9FBF");
        }

        // Reko: a decoder for XCore instruction BD4F at address 0010031A has not been implemented. (17)
        [Test]
        public void XCoreDis_BD4F()
        {
            AssertCode("@@@", "4FBD");
        }

        // Reko: a decoder for XCore instruction 5B1E at address 0010031C has not been implemented. (0B)
        [Test]
        public void XCoreDis_5B1E()
        {
            AssertCode("@@@", "1E5B");
        }

        // Reko: a decoder for XCore instruction DC61 at address 0010031E has not been implemented. (1B)
        [Test]
        public void XCoreDis_DC61()
        {
            AssertCode("@@@", "61DC");
        }

        // Reko: a decoder for XCore instruction 8C81 at address 00100320 has not been implemented. (11)
        [Test]
        public void XCoreDis_8C81()
        {
            AssertCode("@@@", "818C");
        }

        // Reko: a decoder for XCore instruction F8CA at address 00100322 has not been implemented. (1F)
        [Test]
        public void XCoreDis_F8CA()
        {
            AssertCode("@@@", "CAF8");
        }

        // Reko: a decoder for XCore instruction C5AD at address 00100324 has not been implemented. (18)
        [Test]
        public void XCoreDis_C5AD()
        {
            AssertCode("@@@", "ADC5");
        }

        // Reko: a decoder for XCore instruction 3F45 at address 00100326 has not been implemented. (07)
        [Test]
        public void XCoreDis_3F45()
        {
            AssertCode("@@@", "453F");
        }

        // Reko: a decoder for XCore instruction 5D78 at address 00100328 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5D78()
        {
            AssertCode("@@@", "785D");
        }

        // Reko: a decoder for XCore instruction 81A3 at address 0010032A has not been implemented. (10)
        [Test]
        public void XCoreDis_81A3()
        {
            AssertCode("@@@", "A381");
        }

        // Reko: a decoder for XCore instruction D494 at address 0010032C has not been implemented. (1A)
        [Test]
        public void XCoreDis_D494()
        {
            AssertCode("@@@", "94D4");
        }

        // Reko: a decoder for XCore instruction 426D at address 0010032E has not been implemented. (08)
        [Test]
        public void XCoreDis_426D()
        {
            AssertCode("@@@", "6D42");
        }

        // Reko: a decoder for XCore instruction F60F at address 00100330 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F60F()
        {
            AssertCode("@@@", "0FF6");
        }

        // Reko: a decoder for XCore instruction D8A2 at address 00100332 has not been implemented. (1B)
        [Test]
        public void XCoreDis_D8A2()
        {
            AssertCode("@@@", "A2D8");
        }

        // Reko: a decoder for XCore instruction 054F at address 00100334 has not been implemented. (00)
        [Test]
        public void XCoreDis_054F()
        {
            AssertCode("@@@", "4F05");
        }

        // Reko: a decoder for XCore instruction 05AE at address 00100336 has not been implemented. (00)
        [Test]
        public void XCoreDis_05AE()
        {
            AssertCode("@@@", "AE05");
        }

        // Reko: a decoder for XCore instruction 815B at address 00100338 has not been implemented. (10)
        [Test]
        public void XCoreDis_815B()
        {
            AssertCode("@@@", "5B81");
        }

        // Reko: a decoder for XCore instruction 4E3B at address 0010033A has not been implemented. (09)
        [Test]
        public void XCoreDis_4E3B()
        {
            AssertCode("@@@", "3B4E");
        }

        // Reko: a decoder for XCore instruction 682B at address 0010033C has not been implemented. (0D)
        [Test]
        public void XCoreDis_682B()
        {
            AssertCode("@@@", "2B68");
        }

        // Reko: a decoder for XCore instruction 1B83 at address 0010033E has not been implemented. (03)
        [Test]
        public void XCoreDis_1B83()
        {
            AssertCode("@@@", "831B");
        }

        // Reko: a decoder for XCore instruction FABB at address 00100340 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FABB()
        {
            AssertCode("@@@", "BBFA");
        }

        // Reko: a decoder for XCore instruction FA44 at address 00100342 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FA44()
        {
            AssertCode("@@@", "44FA");
        }

        // Reko: a decoder for XCore instruction CCA9 at address 00100344 has not been implemented. (19)
        [Test]
        public void XCoreDis_CCA9()
        {
            AssertCode("@@@", "A9CC");
        }

        // Reko: a decoder for XCore instruction A3AA at address 00100346 has not been implemented. (14)
        [Test]
        public void XCoreDis_A3AA()
        {
            AssertCode("@@@", "AAA3");
        }

        // Reko: a decoder for XCore instruction A21A at address 00100348 has not been implemented. (14)
        [Test]
        public void XCoreDis_A21A()
        {
            AssertCode("@@@", "1AA2");
        }

        // Reko: a decoder for XCore instruction DF09 at address 0010034A has not been implemented. (1B)
        [Test]
        public void XCoreDis_DF09()
        {
            AssertCode("@@@", "09DF");
        }

        // Reko: a decoder for XCore instruction 291E at address 0010034C has not been implemented. (05)
        [Test]
        public void XCoreDis_291E()
        {
            AssertCode("@@@", "1E29");
        }

        // Reko: a decoder for XCore instruction CBA7 at address 0010034E has not been implemented. (19)
        [Test]
        public void XCoreDis_CBA7()
        {
            AssertCode("@@@", "A7CB");
        }

        // Reko: a decoder for XCore instruction 0C51 at address 00100350 has not been implemented. (01)
        [Test]
        public void XCoreDis_0C51()
        {
            AssertCode("@@@", "510C");
        }

        // Reko: a decoder for XCore instruction 27AE at address 00100352 has not been implemented. (04)
        [Test]
        public void XCoreDis_27AE()
        {
            AssertCode("@@@", "AE27");
        }

        // Reko: a decoder for XCore instruction 89B8 at address 00100354 has not been implemented. (11)
        [Test]
        public void XCoreDis_89B8()
        {
            AssertCode("@@@", "B889");
        }

        // Reko: a decoder for XCore instruction 7C8D at address 00100356 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7C8D()
        {
            AssertCode("@@@", "8D7C");
        }

        // Reko: a decoder for XCore instruction C4DD at address 00100358 has not been implemented. (18)
        [Test]
        public void XCoreDis_C4DD()
        {
            AssertCode("@@@", "DDC4");
        }

        // Reko: a decoder for XCore instruction 2C9D at address 0010035A has not been implemented. (05)
        [Test]
        public void XCoreDis_2C9D()
        {
            AssertCode("@@@", "9D2C");
        }

        // Reko: a decoder for XCore instruction F789 at address 0010035C has not been implemented. (1E)
        [Test]
        public void XCoreDis_F789()
        {
            AssertCode("@@@", "89F7");
        }

        // Reko: a decoder for XCore instruction 88D9 at address 00100360 has not been implemented. (11)
        [Test]
        public void XCoreDis_88D9()
        {
            AssertCode("@@@", "D988");
        }

        // Reko: a decoder for XCore instruction 99C6 at address 00100362 has not been implemented. (13)
        [Test]
        public void XCoreDis_99C6()
        {
            AssertCode("@@@", "C699");
        }

        // Reko: a decoder for XCore instruction 738F at address 00100364 has not been implemented. (0E)
        [Test]
        public void XCoreDis_738F()
        {
            AssertCode("@@@", "8F73");
        }

        // Reko: a decoder for XCore instruction 4298 at address 00100366 has not been implemented. (08)
        [Test]
        public void XCoreDis_4298()
        {
            AssertCode("@@@", "9842");
        }

        // Reko: a decoder for XCore instruction FE4B at address 00100368 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FE4B()
        {
            AssertCode("@@@", "4BFE");
        }

        // Reko: a decoder for XCore instruction ADBE at address 0010036A has not been implemented. (15)
        [Test]
        public void XCoreDis_ADBE()
        {
            AssertCode("@@@", "BEAD");
        }

        // Reko: a decoder for XCore instruction CEFB at address 0010036C has not been implemented. (19)
        [Test]
        public void XCoreDis_CEFB()
        {
            AssertCode("@@@", "FBCE");
        }

        // Reko: a decoder for XCore instruction 31E7 at address 0010036E has not been implemented. (06)
        [Test]
        public void XCoreDis_31E7()
        {
            AssertCode("@@@", "E731");
        }

        // Reko: a decoder for XCore instruction 70DA at address 00100370 has not been implemented. (0E)
        [Test]
        public void XCoreDis_70DA()
        {
            AssertCode("@@@", "DA70");
        }

        // Reko: a decoder for XCore instruction 1EFC at address 00100372 has not been implemented. (03)
        [Test]
        public void XCoreDis_1EFC()
        {
            AssertCode("@@@", "FC1E");
        }

        // Reko: a decoder for XCore instruction 5BBA at address 00100374 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5BBA()
        {
            AssertCode("@@@", "BA5B");
        }

        // Reko: a decoder for XCore instruction 3262 at address 00100376 has not been implemented. (06)
        [Test]
        public void XCoreDis_3262()
        {
            AssertCode("@@@", "6232");
        }

        // Reko: a decoder for XCore instruction C86C at address 00100378 has not been implemented. (19)
        [Test]
        public void XCoreDis_C86C()
        {
            AssertCode("@@@", "6CC8");
        }

        // Reko: a decoder for XCore instruction E51C at address 0010037A has not been implemented. (1C)
        [Test]
        public void XCoreDis_E51C()
        {
            AssertCode("@@@", "1CE5");
        }

        // Reko: a decoder for XCore instruction 7E2F at address 0010037C has not been implemented. (0F)
        [Test]
        public void XCoreDis_7E2F()
        {
            AssertCode("@@@", "2F7E");
        }

        // Reko: a decoder for XCore instruction 221A at address 0010037E has not been implemented. (04)
        [Test]
        public void XCoreDis_221A()
        {
            AssertCode("@@@", "1A22");
        }

        // Reko: a decoder for XCore instruction F98E at address 00100380 has not been implemented. (1F)
        [Test]
        public void XCoreDis_F98E()
        {
            AssertCode("@@@", "8EF9");
        }

        // Reko: a decoder for XCore instruction 9506 at address 00100382 has not been implemented. (12)
        [Test]
        public void XCoreDis_9506()
        {
            AssertCode("@@@", "0695");
        }

        // Reko: a decoder for XCore instruction 0D62 at address 00100384 has not been implemented. (01)
        [Test]
        public void XCoreDis_0D62()
        {
            AssertCode("@@@", "620D");
        }

        // Reko: a decoder for XCore instruction DE3B at address 00100386 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DE3B()
        {
            AssertCode("@@@", "3BDE");
        }

        // Reko: a decoder for XCore instruction 6B74 at address 00100388 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B74()
        {
            AssertCode("@@@", "746B");
        }

        // Reko: a decoder for XCore instruction BADC at address 0010038A has not been implemented. (17)
        [Test]
        public void XCoreDis_BADC()
        {
            AssertCode("@@@", "DCBA");
        }

        // Reko: a decoder for XCore instruction E0CB at address 0010038C has not been implemented. (1C)
        [Test]
        public void XCoreDis_E0CB()
        {
            AssertCode("@@@", "CBE0");
        }

        // Reko: a decoder for XCore instruction 0E80 at address 0010038E has not been implemented. (01)
        [Test]
        public void XCoreDis_0E80()
        {
            AssertCode("@@@", "800E");
        }

        // Reko: a decoder for XCore instruction 6BDD at address 00100390 has not been implemented. (0D)
        [Test]
        public void XCoreDis_6BDD()
        {
            AssertCode("@@@", "DD6B");
        }

        // Reko: a decoder for XCore instruction 1952 at address 00100392 has not been implemented. (03)
        [Test]
        public void XCoreDis_1952()
        {
            AssertCode("@@@", "5219");
        }

        // Reko: a decoder for XCore instruction F4FA at address 00100394 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F4FA()
        {
            AssertCode("@@@", "FAF4");
        }

        // Reko: a decoder for XCore instruction 7E55 at address 00100396 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7E55()
        {
            AssertCode("@@@", "557E");
        }

        // Reko: a decoder for XCore instruction 9426 at address 00100398 has not been implemented. (12)
        [Test]
        public void XCoreDis_9426()
        {
            AssertCode("@@@", "2694");
        }

        // Reko: a decoder for XCore instruction C72D at address 0010039A has not been implemented. (18)
        [Test]
        public void XCoreDis_C72D()
        {
            AssertCode("@@@", "2DC7");
        }

        // Reko: a decoder for XCore instruction B356 at address 0010039C has not been implemented. (16)
        [Test]
        public void XCoreDis_B356()
        {
            AssertCode("@@@", "56B3");
        }

        // Reko: a decoder for XCore instruction CD13 at address 0010039E has not been implemented. (19)
        [Test]
        public void XCoreDis_CD13()
        {
            AssertCode("@@@", "13CD");
        }

        // Reko: a decoder for XCore instruction 9BE4 at address 001003A0 has not been implemented. (13)
        [Test]
        public void XCoreDis_9BE4()
        {
            AssertCode("@@@", "E49B");
        }

        // Reko: a decoder for XCore instruction 021E at address 001003A2 has not been implemented. (00)
        [Test]
        public void XCoreDis_021E()
        {
            AssertCode("@@@", "1E02");
        }

        // Reko: a decoder for XCore instruction 51C8 at address 001003A4 has not been implemented. (0A)
        [Test]
        public void XCoreDis_51C8()
        {
            AssertCode("@@@", "C851");
        }

        // Reko: a decoder for XCore instruction CCCE at address 001003A6 has not been implemented. (19)
        [Test]
        public void XCoreDis_CCCE()
        {
            AssertCode("@@@", "CECC");
        }

        // Reko: a decoder for XCore instruction 1E35 at address 001003A8 has not been implemented. (03)
        [Test]
        public void XCoreDis_1E35()
        {
            AssertCode("@@@", "351E");
        }

        // Reko: a decoder for XCore instruction 4FAA at address 001003AA has not been implemented. (09)
        [Test]
        public void XCoreDis_4FAA()
        {
            AssertCode("@@@", "AA4F");
        }

        // Reko: a decoder for XCore instruction A87F at address 001003AC has not been implemented. (15)
        [Test]
        public void XCoreDis_A87F()
        {
            AssertCode("@@@", "7FA8");
        }

        // Reko: a decoder for XCore instruction 8C67 at address 001003AE has not been implemented. (11)
        [Test]
        public void XCoreDis_8C67()
        {
            AssertCode("@@@", "678C");
        }

        // Reko: a decoder for XCore instruction 0E47 at address 001003B0 has not been implemented. (01)
        [Test]
        public void XCoreDis_0E47()
        {
            AssertCode("@@@", "470E");
        }

        // Reko: a decoder for XCore instruction C307 at address 001003B2 has not been implemented. (18)
        [Test]
        public void XCoreDis_C307()
        {
            AssertCode("@@@", "07C3");
        }

        // Reko: a decoder for XCore instruction 002B at address 001003B4 has not been implemented. (00)
        [Test]
        public void XCoreDis_002B()
        {
            AssertCode("@@@", "2B00");
        }

        // Reko: a decoder for XCore instruction 9928 at address 001003B6 has not been implemented. (13)
        [Test]
        public void XCoreDis_9928()
        {
            AssertCode("@@@", "2899");
        }

        // Reko: a decoder for XCore instruction 87A4 at address 001003B8 has not been implemented. (10)
        [Test]
        public void XCoreDis_87A4()
        {
            AssertCode("@@@", "A487");
        }

        // Reko: a decoder for XCore instruction CE6F at address 001003BA has not been implemented. (19)
        [Test]
        public void XCoreDis_CE6F()
        {
            AssertCode("@@@", "6FCE");
        }

        // Reko: a decoder for XCore instruction 73E0 at address 001003BC has not been implemented. (0E)
        [Test]
        public void XCoreDis_73E0()
        {
            AssertCode("@@@", "E073");
        }

        // Reko: a decoder for XCore instruction C087 at address 001003BE has not been implemented. (18)
        [Test]
        public void XCoreDis_C087()
        {
            AssertCode("@@@", "87C0");
        }

        // Reko: a decoder for XCore instruction 0E57 at address 001003C0 has not been implemented. (01)
        [Test]
        public void XCoreDis_0E57()
        {
            AssertCode("@@@", "570E");
        }

        // Reko: a decoder for XCore instruction 2FD6 at address 001003C2 has not been implemented. (05)
        [Test]
        public void XCoreDis_2FD6()
        {
            AssertCode("@@@", "D62F");
        }

        // Reko: a decoder for XCore instruction 7EC1 at address 001003C4 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7EC1()
        {
            AssertCode("@@@", "C17E");
        }

        // Reko: a decoder for XCore instruction 8B46 at address 001003C6 has not been implemented. (11)
        [Test]
        public void XCoreDis_8B46()
        {
            AssertCode("@@@", "468B");
        }

        // Reko: a decoder for XCore instruction 859C at address 001003C8 has not been implemented. (10)
        [Test]
        public void XCoreDis_859C()
        {
            AssertCode("@@@", "9C85");
        }

        // Reko: a decoder for XCore instruction DCE3 at address 001003CA has not been implemented. (1B)
        [Test]
        public void XCoreDis_DCE3()
        {
            AssertCode("@@@", "E3DC");
        }

        // Reko: a decoder for XCore instruction 0549 at address 001003CC has not been implemented. (00)
        [Test]
        public void XCoreDis_0549()
        {
            AssertCode("@@@", "4905");
        }

        // Reko: a decoder for XCore instruction 7EFF at address 001003CE has not been implemented. (0F)
        [Test]
        public void XCoreDis_7EFF()
        {
            AssertCode("@@@", "FF7E");
        }

        // Reko: a decoder for XCore instruction A02C at address 001003D0 has not been implemented. (14)
        [Test]
        public void XCoreDis_A02C()
        {
            AssertCode("@@@", "2CA0");
        }

        // Reko: a decoder for XCore instruction 4880 at address 001003D2 has not been implemented. (09)
        [Test]
        public void XCoreDis_4880()
        {
            AssertCode("@@@", "8048");
        }

        // Reko: a decoder for XCore instruction 4FAB at address 001003D4 has not been implemented. (09)
        [Test]
        public void XCoreDis_4FAB()
        {
            AssertCode("@@@", "AB4F");
        }

        // Reko: a decoder for XCore instruction E3A1 at address 001003D6 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E3A1()
        {
            AssertCode("@@@", "A1E3");
        }

        // Reko: a decoder for XCore instruction 8473 at address 001003D8 has not been implemented. (10)
        [Test]
        public void XCoreDis_8473()
        {
            AssertCode("@@@", "7384");
        }

        // Reko: a decoder for XCore instruction 415D at address 001003DA has not been implemented. (08)
        [Test]
        public void XCoreDis_415D()
        {
            AssertCode("@@@", "5D41");
        }

        // Reko: a decoder for XCore instruction 00E2 at address 001003DC has not been implemented. (00)
        [Test]
        public void XCoreDis_00E2()
        {
            AssertCode("@@@", "E200");
        }

        // Reko: a decoder for XCore instruction C1EC at address 001003DE has not been implemented. (18)
        [Test]
        public void XCoreDis_C1EC()
        {
            AssertCode("@@@", "ECC1");
        }

        // Reko: a decoder for XCore instruction EA96 at address 001003E0 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EA96()
        {
            AssertCode("@@@", "96EA");
        }

        // Reko: a decoder for XCore instruction 70F8 at address 001003E2 has not been implemented. (0E)
        [Test]
        public void XCoreDis_70F8()
        {
            AssertCode("@@@", "F870");
        }

        // Reko: a decoder for XCore instruction 37D1 at address 001003E4 has not been implemented. (06)
        [Test]
        public void XCoreDis_37D1()
        {
            AssertCode("@@@", "D137");
        }

        // Reko: a decoder for XCore instruction C9CB at address 001003E6 has not been implemented. (19)
        [Test]
        public void XCoreDis_C9CB()
        {
            AssertCode("@@@", "CBC9");
        }

        // Reko: a decoder for XCore instruction 7BC7 at address 001003E8 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7BC7()
        {
            AssertCode("@@@", "C77B");
        }

        // Reko: a decoder for XCore instruction A627 at address 001003EA has not been implemented. (14)
        [Test]
        public void XCoreDis_A627()
        {
            AssertCode("@@@", "27A6");
        }

        // Reko: a decoder for XCore instruction 4B1D at address 001003EC has not been implemented. (09)
        [Test]
        public void XCoreDis_4B1D()
        {
            AssertCode("@@@", "1D4B");
        }

        // Reko: a decoder for XCore instruction 9E50 at address 001003EE has not been implemented. (13)
        [Test]
        public void XCoreDis_9E50()
        {
            AssertCode("@@@", "509E");
        }

        // Reko: a decoder for XCore instruction F188 at address 001003F0 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F188()
        {
            AssertCode("@@@", "88F1");
        }

        // Reko: a decoder for XCore instruction 3FA1 at address 001003F2 has not been implemented. (07)
        [Test]
        public void XCoreDis_3FA1()
        {
            AssertCode("@@@", "A13F");
        }

        // Reko: a decoder for XCore instruction 3FF3 at address 001003F4 has not been implemented. (07)
        [Test]
        public void XCoreDis_3FF3()
        {
            AssertCode("@@@", "F33F");
        }

        // Reko: a decoder for XCore instruction 0715 at address 001003F6 has not been implemented. (00)
        [Test]
        public void XCoreDis_0715()
        {
            AssertCode("@@@", "1507");
        }

        // Reko: a decoder for XCore instruction F26D at address 001003F8 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F26D()
        {
            AssertCode("@@@", "6DF2");
        }

        // Reko: a decoder for XCore instruction 3CBC at address 001003FA has not been implemented. (07)
        [Test]
        public void XCoreDis_3CBC()
        {
            AssertCode("@@@", "BC3C");
        }

        // Reko: a decoder for XCore instruction 0420 at address 001003FC has not been implemented. (00)
        [Test]
        public void XCoreDis_0420()
        {
            AssertCode("@@@", "2004");
        }

        // Reko: a decoder for XCore instruction 9CA9 at address 001003FE has not been implemented. (13)
        [Test]
        public void XCoreDis_9CA9()
        {
            AssertCode("@@@", "A99C");
        }

        // Reko: a decoder for XCore instruction 2199 at address 00100400 has not been implemented. (04)
        [Test]
        public void XCoreDis_2199()
        {
            AssertCode("@@@", "9921");
        }

        // Reko: a decoder for XCore instruction 5F45 at address 00100402 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5F45()
        {
            AssertCode("@@@", "455F");
        }

        // Reko: a decoder for XCore instruction 8E0D at address 00100404 has not been implemented. (11)
        [Test]
        public void XCoreDis_8E0D()
        {
            AssertCode("@@@", "0D8E");
        }

        // Reko: a decoder for XCore instruction F5AC at address 00100406 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F5AC()
        {
            AssertCode("@@@", "ACF5");
        }

        // Reko: a decoder for XCore instruction B6D5 at address 00100408 has not been implemented. (16)
        [Test]
        public void XCoreDis_B6D5()
        {
            AssertCode("@@@", "D5B6");
        }

        // Reko: a decoder for XCore instruction 2F80 at address 0010040A has not been implemented. (05)
        [Test]
        public void XCoreDis_2F80()
        {
            AssertCode("@@@", "802F");
        }

        // Reko: a decoder for XCore instruction FB28 at address 0010040C has not been implemented. (1F)
        [Test]
        public void XCoreDis_FB28()
        {
            AssertCode("@@@", "28FB");
        }

        // Reko: a decoder for XCore instruction 27C6 at address 0010040E has not been implemented. (04)
        [Test]
        public void XCoreDis_27C6()
        {
            AssertCode("@@@", "C627");
        }

        // Reko: a decoder for XCore instruction BF34 at address 00100410 has not been implemented. (17)
        [Test]
        public void XCoreDis_BF34()
        {
            AssertCode("@@@", "34BF");
        }

        // Reko: a decoder for XCore instruction F0B8 at address 00100412 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F0B8()
        {
            AssertCode("@@@", "B8F0");
        }

        // Reko: a decoder for XCore instruction AC5E at address 00100414 has not been implemented. (15)
        [Test]
        public void XCoreDis_AC5E()
        {
            AssertCode("@@@", "5EAC");
        }

        // Reko: a decoder for XCore instruction 57CE at address 00100416 has not been implemented. (0A)
        [Test]
        public void XCoreDis_57CE()
        {
            AssertCode("@@@", "CE57");
        }

        // Reko: a decoder for XCore instruction F1D4 at address 00100418 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F1D4()
        {
            AssertCode("@@@", "D4F1");
        }

        // Reko: a decoder for XCore instruction DF03 at address 0010041A has not been implemented. (1B)
        [Test]
        public void XCoreDis_DF03()
        {
            AssertCode("@@@", "03DF");
        }

        // Reko: a decoder for XCore instruction 8E7B at address 0010041C has not been implemented. (11)
        [Test]
        public void XCoreDis_8E7B()
        {
            AssertCode("@@@", "7B8E");
        }

        // Reko: a decoder for XCore instruction C2A8 at address 0010041E has not been implemented. (18)
        [Test]
        public void XCoreDis_C2A8()
        {
            AssertCode("@@@", "A8C2");
        }

        // Reko: a decoder for XCore instruction 8AD2 at address 00100420 has not been implemented. (11)
        [Test]
        public void XCoreDis_8AD2()
        {
            AssertCode("@@@", "D28A");
        }

        // Reko: a decoder for XCore instruction FB0C at address 00100422 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FB0C()
        {
            AssertCode("@@@", "0CFB");
        }

        // Reko: a decoder for XCore instruction F006 at address 00100424 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F006()
        {
            AssertCode("@@@", "06F0");
        }

        // Reko: a decoder for XCore instruction F991 at address 00100426 has not been implemented. (1F)
        [Test]
        public void XCoreDis_F991()
        {
            AssertCode("@@@", "91F9");
        }

        // Reko: a decoder for XCore instruction AC45 at address 00100428 has not been implemented. (15)
        [Test]
        public void XCoreDis_AC45()
        {
            AssertCode("@@@", "45AC");
        }

        // Reko: a decoder for XCore instruction 3C6A at address 0010042A has not been implemented. (07)
        [Test]
        public void XCoreDis_3C6A()
        {
            AssertCode("@@@", "6A3C");
        }

        // Reko: a decoder for XCore instruction E5BE at address 0010042C has not been implemented. (1C)
        [Test]
        public void XCoreDis_E5BE()
        {
            AssertCode("@@@", "BEE5");
        }

        // Reko: a decoder for XCore instruction 72DE at address 0010042E has not been implemented. (0E)
        [Test]
        public void XCoreDis_72DE()
        {
            AssertCode("@@@", "DE72");
        }

        // Reko: a decoder for XCore instruction 952C at address 00100430 has not been implemented. (12)
        [Test]
        public void XCoreDis_952C()
        {
            AssertCode("@@@", "2C95");
        }

        // Reko: a decoder for XCore instruction 6108 at address 00100432 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6108()
        {
            AssertCode("@@@", "0861");
        }

        // Reko: a decoder for XCore instruction B84C at address 00100434 has not been implemented. (17)
        [Test]
        public void XCoreDis_B84C()
        {
            AssertCode("@@@", "4CB8");
        }

        // Reko: a decoder for XCore instruction ED3E at address 00100436 has not been implemented. (1D)
        [Test]
        public void XCoreDis_ED3E()
        {
            AssertCode("@@@", "3EED");
        }

        // Reko: a decoder for XCore instruction EE53 at address 00100438 has not been implemented. (1D)
        [Test]
        public void XCoreDis_EE53()
        {
            AssertCode("@@@", "53EE");
        }

        // Reko: a decoder for XCore instruction 1C8A at address 0010043A has not been implemented. (03)
        [Test]
        public void XCoreDis_1C8A()
        {
            AssertCode("@@@", "8A1C");
        }

        // Reko: a decoder for XCore instruction CD8B at address 0010043C has not been implemented. (19)
        [Test]
        public void XCoreDis_CD8B()
        {
            AssertCode("@@@", "8BCD");
        }

        // Reko: a decoder for XCore instruction 477A at address 0010043E has not been implemented. (08)
        [Test]
        public void XCoreDis_477A()
        {
            AssertCode("@@@", "7A47");
        }

        // Reko: a decoder for XCore instruction BD0E at address 00100440 has not been implemented. (17)
        [Test]
        public void XCoreDis_BD0E()
        {
            AssertCode("@@@", "0EBD");
        }

        // Reko: a decoder for XCore instruction 9D5D at address 00100442 has not been implemented. (13)
        [Test]
        public void XCoreDis_9D5D()
        {
            AssertCode("@@@", "5D9D");
        }

        // Reko: a decoder for XCore instruction CAEE at address 00100444 has not been implemented. (19)
        [Test]
        public void XCoreDis_CAEE()
        {
            AssertCode("@@@", "EECA");
        }

        // Reko: a decoder for XCore instruction 4420 at address 00100446 has not been implemented. (08)
        [Test]
        public void XCoreDis_4420()
        {
            AssertCode("@@@", "2044");
        }

        // Reko: a decoder for XCore instruction BF2D at address 00100448 has not been implemented. (17)
        [Test]
        public void XCoreDis_BF2D()
        {
            AssertCode("@@@", "2DBF");
        }

        // Reko: a decoder for XCore instruction B1AB at address 0010044A has not been implemented. (16)
        [Test]
        public void XCoreDis_B1AB()
        {
            AssertCode("@@@", "ABB1");
        }

        // Reko: a decoder for XCore instruction 9241 at address 0010044C has not been implemented. (12)
        [Test]
        public void XCoreDis_9241()
        {
            AssertCode("@@@", "4192");
        }

        // Reko: a decoder for XCore instruction EF98 at address 0010044E has not been implemented. (1D)
        [Test]
        public void XCoreDis_EF98()
        {
            AssertCode("@@@", "98EF");
        }

        // Reko: a decoder for XCore instruction 9112 at address 00100450 has not been implemented. (12)
        [Test]
        public void XCoreDis_9112()
        {
            AssertCode("@@@", "1291");
        }

        // Reko: a decoder for XCore instruction E5B2 at address 00100452 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E5B2()
        {
            AssertCode("@@@", "B2E5");
        }

        // Reko: a decoder for XCore instruction 1976 at address 00100456 has not been implemented. (03)
        [Test]
        public void XCoreDis_1976()
        {
            AssertCode("@@@", "7619");
        }

        // Reko: a decoder for XCore instruction 1F4C at address 00100458 has not been implemented. (03)
        [Test]
        public void XCoreDis_1F4C()
        {
            AssertCode("@@@", "4C1F");
        }

        // Reko: a decoder for XCore instruction 18A8 at address 0010045A has not been implemented. (03)
        [Test]
        public void XCoreDis_18A8()
        {
            AssertCode("@@@", "A818");
        }

        // Reko: a decoder for XCore instruction 7565 at address 0010045C has not been implemented. (0E)
        [Test]
        public void XCoreDis_7565()
        {
            AssertCode("@@@", "6575");
        }

        // Reko: a decoder for XCore instruction 786E at address 0010045E has not been implemented. (0F)
        [Test]
        public void XCoreDis_786E()
        {
            AssertCode("@@@", "6E78");
        }

        // Reko: a decoder for XCore instruction 2232 at address 00100460 has not been implemented. (04)
        [Test]
        public void XCoreDis_2232()
        {
            AssertCode("@@@", "3222");
        }

        // Reko: a decoder for XCore instruction 012E at address 00100462 has not been implemented. (00)
        [Test]
        public void XCoreDis_012E()
        {
            AssertCode("@@@", "2E01");
        }

        // Reko: a decoder for XCore instruction 4087 at address 00100464 has not been implemented. (08)
        [Test]
        public void XCoreDis_4087()
        {
            AssertCode("@@@", "8740");
        }

        // Reko: a decoder for XCore instruction 6283 at address 00100466 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6283()
        {
            AssertCode("@@@", "8362");
        }

        // Reko: a decoder for XCore instruction C474 at address 00100468 has not been implemented. (18)
        [Test]
        public void XCoreDis_C474()
        {
            AssertCode("@@@", "74C4");
        }

        // Reko: a decoder for XCore instruction 8D33 at address 0010046A has not been implemented. (11)
        [Test]
        public void XCoreDis_8D33()
        {
            AssertCode("@@@", "338D");
        }

        // Reko: a decoder for XCore instruction 8C0C at address 0010046C has not been implemented. (11)
        [Test]
        public void XCoreDis_8C0C()
        {
            AssertCode("@@@", "0C8C");
        }

        // Reko: a decoder for XCore instruction C0AB at address 0010046E has not been implemented. (18)
        [Test]
        public void XCoreDis_C0AB()
        {
            AssertCode("@@@", "ABC0");
        }

        // Reko: a decoder for XCore instruction 9B55 at address 00100470 has not been implemented. (13)
        [Test]
        public void XCoreDis_9B55()
        {
            AssertCode("@@@", "559B");
        }

        // Reko: a decoder for XCore instruction FA09 at address 00100472 has not been implemented. (1F)
        [Test]
        public void XCoreDis_FA09()
        {
            AssertCode("@@@", "09FA");
        }

        // Reko: a decoder for XCore instruction 941A at address 00100474 has not been implemented. (12)
        [Test]
        public void XCoreDis_941A()
        {
            AssertCode("@@@", "1A94");
        }

        // Reko: a decoder for XCore instruction C6C1 at address 00100476 has not been implemented. (18)
        [Test]
        public void XCoreDis_C6C1()
        {
            AssertCode("@@@", "C1C6");
        }

        // Reko: a decoder for XCore instruction 4446 at address 00100478 has not been implemented. (08)
        [Test]
        public void XCoreDis_4446()
        {
            AssertCode("@@@", "4644");
        }

        // Reko: a decoder for XCore instruction CE51 at address 0010047A has not been implemented. (19)
        [Test]
        public void XCoreDis_CE51()
        {
            AssertCode("@@@", "51CE");
        }

        // Reko: a decoder for XCore instruction 0822 at address 0010047C has not been implemented. (01)
        [Test]
        public void XCoreDis_0822()
        {
            AssertCode("@@@", "2208");
        }

        // Reko: a decoder for XCore instruction B8DE at address 0010047E has not been implemented. (17)
        [Test]
        public void XCoreDis_B8DE()
        {
            AssertCode("@@@", "DEB8");
        }

        // Reko: a decoder for XCore instruction 3350 at address 00100480 has not been implemented. (06)
        [Test]
        public void XCoreDis_3350()
        {
            AssertCode("@@@", "5033");
        }

        // Reko: a decoder for XCore instruction 1E7F at address 00100482 has not been implemented. (03)
        [Test]
        public void XCoreDis_1E7F()
        {
            AssertCode("@@@", "7F1E");
        }

        // Reko: a decoder for XCore instruction 9763 at address 00100484 has not been implemented. (12)
        [Test]
        public void XCoreDis_9763()
        {
            AssertCode("@@@", "6397");
        }

        // Reko: a decoder for XCore instruction D268 at address 00100486 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D268()
        {
            AssertCode("@@@", "68D2");
        }

        // Reko: a decoder for XCore instruction 500D at address 00100488 has not been implemented. (0A)
        [Test]
        public void XCoreDis_500D()
        {
            AssertCode("@@@", "0D50");
        }

        // Reko: a decoder for XCore instruction C171 at address 0010048A has not been implemented. (18)
        [Test]
        public void XCoreDis_C171()
        {
            AssertCode("@@@", "71C1");
        }

        // Reko: a decoder for XCore instruction E913 at address 0010048C has not been implemented. (1D)
        [Test]
        public void XCoreDis_E913()
        {
            AssertCode("@@@", "13E9");
        }

        // Reko: a decoder for XCore instruction C00C at address 0010048E has not been implemented. (18)
        [Test]
        public void XCoreDis_C00C()
        {
            AssertCode("@@@", "0CC0");
        }

        // Reko: a decoder for XCore instruction E774 at address 00100490 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E774()
        {
            AssertCode("@@@", "74E7");
        }

        // Reko: a decoder for XCore instruction CAC2 at address 00100492 has not been implemented. (19)
        [Test]
        public void XCoreDis_CAC2()
        {
            AssertCode("@@@", "C2CA");
        }

        // Reko: a decoder for XCore instruction 746B at address 00100494 has not been implemented. (0E)
        [Test]
        public void XCoreDis_746B()
        {
            AssertCode("@@@", "6B74");
        }

        // Reko: a decoder for XCore instruction 6761 at address 00100498 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6761()
        {
            AssertCode("@@@", "6167");
        }

        // Reko: a decoder for XCore instruction 43BA at address 0010049A has not been implemented. (08)
        [Test]
        public void XCoreDis_43BA()
        {
            AssertCode("@@@", "BA43");
        }

        // Reko: a decoder for XCore instruction B5EF at address 0010049C has not been implemented. (16)
        [Test]
        public void XCoreDis_B5EF()
        {
            AssertCode("@@@", "EFB5");
        }

        // Reko: a decoder for XCore instruction 6B40 at address 0010049E has not been implemented. (0D)
        [Test]
        public void XCoreDis_6B40()
        {
            AssertCode("@@@", "406B");
        }

        // Reko: a decoder for XCore instruction 7BE5 at address 001004A0 has not been implemented. (0F)
        [Test]
        public void XCoreDis_7BE5()
        {
            AssertCode("@@@", "E57B");
        }

        // Reko: a decoder for XCore instruction D93C at address 001004A2 has not been implemented. (1B)
        [Test]
        public void XCoreDis_D93C()
        {
            AssertCode("@@@", "3CD9");
        }

        // Reko: a decoder for XCore instruction BD5D at address 001004A6 has not been implemented. (17)
        [Test]
        public void XCoreDis_BD5D()
        {
            AssertCode("@@@", "5DBD");
        }

        // Reko: a decoder for XCore instruction 3733 at address 001004A8 has not been implemented. (06)
        [Test]
        public void XCoreDis_3733()
        {
            AssertCode("@@@", "3337");
        }

        // Reko: a decoder for XCore instruction CAEC at address 001004AA has not been implemented. (19)
        [Test]
        public void XCoreDis_CAEC()
        {
            AssertCode("@@@", "ECCA");
        }

        // Reko: a decoder for XCore instruction FF22 at address 001004AC has not been implemented. (1F)
        [Test]
        public void XCoreDis_FF22()
        {
            AssertCode("@@@", "22FF");
        }

        // Reko: a decoder for XCore instruction 5DB3 at address 001004AE has not been implemented. (0B)
        [Test]
        public void XCoreDis_5DB3()
        {
            AssertCode("@@@", "B35D");
        }

        // Reko: a decoder for XCore instruction 9037 at address 001004B0 has not been implemented. (12)
        [Test]
        public void XCoreDis_9037()
        {
            AssertCode("@@@", "3790");
        }

        // Reko: a decoder for XCore instruction 3A59 at address 001004B2 has not been implemented. (07)
        [Test]
        public void XCoreDis_3A59()
        {
            AssertCode("@@@", "593A");
        }

        // Reko: a decoder for XCore instruction 1445 at address 001004B4 has not been implemented. (02)
        [Test]
        public void XCoreDis_1445()
        {
            AssertCode("@@@", "4514");
        }

        // Reko: a decoder for XCore instruction DC4C at address 001004B6 has not been implemented. (1B)
        [Test]
        public void XCoreDis_DC4C()
        {
            AssertCode("@@@", "4CDC");
        }

        // Reko: a decoder for XCore instruction E1D6 at address 001004B8 has not been implemented. (1C)
        [Test]
        public void XCoreDis_E1D6()
        {
            AssertCode("@@@", "D6E1");
        }

        // Reko: a decoder for XCore instruction FCBD at address 001004BA has not been implemented. (1F)
        [Test]
        public void XCoreDis_FCBD()
        {
            AssertCode("@@@", "BDFC");
        }

        // Reko: a decoder for XCore instruction 24DD at address 001004BC has not been implemented. (04)
        [Test]
        public void XCoreDis_24DD()
        {
            AssertCode("@@@", "DD24");
        }

        // Reko: a decoder for XCore instruction 58E2 at address 001004BE has not been implemented. (0B)
        [Test]
        public void XCoreDis_58E2()
        {
            AssertCode("@@@", "E258");
        }

        // Reko: a decoder for XCore instruction 050F at address 001004C0 has not been implemented. (00)
        [Test]
        public void XCoreDis_050F()
        {
            AssertCode("@@@", "0F05");
        }

        // Reko: a decoder for XCore instruction 97DB at address 001004C2 has not been implemented. (12)
        [Test]
        public void XCoreDis_97DB()
        {
            AssertCode("@@@", "DB97");
        }

        // Reko: a decoder for XCore instruction 33AD at address 001004C4 has not been implemented. (06)
        [Test]
        public void XCoreDis_33AD()
        {
            AssertCode("@@@", "AD33");
        }

        // Reko: a decoder for XCore instruction E8B3 at address 001004C6 has not been implemented. (1D)
        [Test]
        public void XCoreDis_E8B3()
        {
            AssertCode("@@@", "B3E8");
        }

        // Reko: a decoder for XCore instruction 048A at address 001004C8 has not been implemented. (00)
        [Test]
        public void XCoreDis_048A()
        {
            AssertCode("@@@", "8A04");
        }

        // Reko: a decoder for XCore instruction 3497 at address 001004CA has not been implemented. (06)
        [Test]
        public void XCoreDis_3497()
        {
            AssertCode("@@@", "9734");
        }

        // Reko: a decoder for XCore instruction 9287 at address 001004CC has not been implemented. (12)
        [Test]
        public void XCoreDis_9287()
        {
            AssertCode("@@@", "8792");
        }

        // Reko: a decoder for XCore instruction 617B at address 001004CE has not been implemented. (0C)
        [Test]
        public void XCoreDis_617B()
        {
            AssertCode("@@@", "7B61");
        }

        // Reko: a decoder for XCore instruction 5CB4 at address 001004D0 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5CB4()
        {
            AssertCode("@@@", "B45C");
        }

        // Reko: a decoder for XCore instruction 5E0B at address 001004D2 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5E0B()
        {
            AssertCode("@@@", "0B5E");
        }

        // Reko: a decoder for XCore instruction 055B at address 001004D4 has not been implemented. (00)
        [Test]
        public void XCoreDis_055B()
        {
            AssertCode("@@@", "5B05");
        }

        // Reko: a decoder for XCore instruction D125 at address 001004D6 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D125()
        {
            AssertCode("@@@", "25D1");
        }

        // Reko: a decoder for XCore instruction 5F2F at address 001004D8 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5F2F()
        {
            AssertCode("@@@", "2F5F");
        }

        // Reko: a decoder for XCore instruction 2B03 at address 001004DA has not been implemented. (05)
        [Test]
        public void XCoreDis_2B03()
        {
            AssertCode("@@@", "032B");
        }

        // Reko: a decoder for XCore instruction 60CE at address 001004DC has not been implemented. (0C)
        [Test]
        public void XCoreDis_60CE()
        {
            AssertCode("@@@", "CE60");
        }

        // Reko: a decoder for XCore instruction 0EE0 at address 001004DE has not been implemented. (01)
        [Test]
        public void XCoreDis_0EE0()
        {
            AssertCode("@@@", "E00E");
        }

        // Reko: a decoder for XCore instruction 9454 at address 001004E0 has not been implemented. (12)
        [Test]
        public void XCoreDis_9454()
        {
            AssertCode("@@@", "5494");
        }

        // Reko: a decoder for XCore instruction 1CBA at address 001004E2 has not been implemented. (03)
        [Test]
        public void XCoreDis_1CBA()
        {
            AssertCode("@@@", "BA1C");
        }

        // Reko: a decoder for XCore instruction 1B24 at address 001004E4 has not been implemented. (03)
        [Test]
        public void XCoreDis_1B24()
        {
            AssertCode("@@@", "241B");
        }

        // Reko: a decoder for XCore instruction 04B0 at address 001004E6 has not been implemented. (00)
        [Test]
        public void XCoreDis_04B0()
        {
            AssertCode("@@@", "B004");
        }

        // Reko: a decoder for XCore instruction 71DC at address 001004E8 has not been implemented. (0E)
        [Test]
        public void XCoreDis_71DC()
        {
            AssertCode("@@@", "DC71");
        }

        // Reko: a decoder for XCore instruction 40B0 at address 001004EA has not been implemented. (08)
        [Test]
        public void XCoreDis_40B0()
        {
            AssertCode("@@@", "B040");
        }

        // Reko: a decoder for XCore instruction 177D at address 001004EC has not been implemented. (02)
        [Test]
        public void XCoreDis_177D()
        {
            AssertCode("@@@", "7D17");
        }

        // Reko: a decoder for XCore instruction 4355 at address 001004EE has not been implemented. (08)
        [Test]
        public void XCoreDis_4355()
        {
            AssertCode("@@@", "5543");
        }

        // Reko: a decoder for XCore instruction 5C66 at address 001004F0 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5C66()
        {
            AssertCode("@@@", "665C");
        }

        // Reko: a decoder for XCore instruction 8047 at address 001004F2 has not been implemented. (10)
        [Test]
        public void XCoreDis_8047()
        {
            AssertCode("@@@", "4780");
        }

        // Reko: a decoder for XCore instruction 8418 at address 001004F4 has not been implemented. (10)
        [Test]
        public void XCoreDis_8418()
        {
            AssertCode("@@@", "1884");
        }

        // Reko: a decoder for XCore instruction 0AFC at address 001004F6 has not been implemented. (01)
        [Test]
        public void XCoreDis_0AFC()
        {
            AssertCode("@@@", "FC0A");
        }

        // Reko: a decoder for XCore instruction 0ADF at address 001004F8 has not been implemented. (01)
        [Test]
        public void XCoreDis_0ADF()
        {
            AssertCode("@@@", "DF0A");
        }

        // Reko: a decoder for XCore instruction 4D68 at address 001004FA has not been implemented. (09)
        [Test]
        public void XCoreDis_4D68()
        {
            AssertCode("@@@", "684D");
        }

        // Reko: a decoder for XCore instruction 8830 at address 001004FC has not been implemented. (11)
        [Test]
        public void XCoreDis_8830()
        {
            AssertCode("@@@", "3088");
        }

        // Reko: a decoder for XCore instruction 2919 at address 001004FE has not been implemented. (05)
        [Test]
        public void XCoreDis_2919()
        {
            AssertCode("@@@", "1929");
        }

        // Reko: a decoder for XCore instruction 8824 at address 00100500 has not been implemented. (11)
        [Test]
        public void XCoreDis_8824()
        {
            AssertCode("@@@", "2488");
        }

        // Reko: a decoder for XCore instruction F2DF at address 00100502 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F2DF()
        {
            AssertCode("@@@", "DFF2");
        }

        // Reko: a decoder for XCore instruction 5ED8 at address 00100504 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5ED8()
        {
            AssertCode("@@@", "D85E");
        }

        // Reko: a decoder for XCore instruction 983D at address 00100506 has not been implemented. (13)
        [Test]
        public void XCoreDis_983D()
        {
            AssertCode("@@@", "3D98");
        }

        // Reko: a decoder for XCore instruction 07AC at address 00100508 has not been implemented. (00)
        [Test]
        public void XCoreDis_07AC()
        {
            AssertCode("@@@", "AC07");
        }

        // Reko: a decoder for XCore instruction EA81 at address 0010050A has not been implemented. (1D)
        [Test]
        public void XCoreDis_EA81()
        {
            AssertCode("@@@", "81EA");
        }

        // Reko: a decoder for XCore instruction E455 at address 0010050C has not been implemented. (1C)
        [Test]
        public void XCoreDis_E455()
        {
            AssertCode("@@@", "55E4");
        }

        // Reko: a decoder for XCore instruction 1754 at address 0010050E has not been implemented. (02)
        [Test]
        public void XCoreDis_1754()
        {
            AssertCode("@@@", "5417");
        }

        // Reko: a decoder for XCore instruction BF09 at address 00100510 has not been implemented. (17)
        [Test]
        public void XCoreDis_BF09()
        {
            AssertCode("@@@", "09BF");
        }

        // Reko: a decoder for XCore instruction 72C4 at address 00100512 has not been implemented. (0E)
        [Test]
        public void XCoreDis_72C4()
        {
            AssertCode("@@@", "C472");
        }

        // Reko: a decoder for XCore instruction 5F19 at address 00100514 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5F19()
        {
            AssertCode("@@@", "195F");
        }

        // Reko: a decoder for XCore instruction D0F5 at address 00100516 has not been implemented. (1A)
        [Test]
        public void XCoreDis_D0F5()
        {
            AssertCode("@@@", "F5D0");
        }

        // Reko: a decoder for XCore instruction B098 at address 00100518 has not been implemented. (16)
        [Test]
        public void XCoreDis_B098()
        {
            AssertCode("@@@", "98B0");
        }

        // Reko: a decoder for XCore instruction 193D at address 0010051A has not been implemented. (03)
        [Test]
        public void XCoreDis_193D()
        {
            AssertCode("@@@", "3D19");
        }

        // Reko: a decoder for XCore instruction 62B3 at address 0010051C has not been implemented. (0C)
        [Test]
        public void XCoreDis_62B3()
        {
            AssertCode("@@@", "B362");
        }

        // Reko: a decoder for XCore instruction 54D4 at address 0010051E has not been implemented. (0A)
        [Test]
        public void XCoreDis_54D4()
        {
            AssertCode("@@@", "D454");
        }

        // Reko: a decoder for XCore instruction 8657 at address 00100520 has not been implemented. (10)
        [Test]
        public void XCoreDis_8657()
        {
            AssertCode("@@@", "5786");
        }

        // Reko: a decoder for XCore instruction F41C at address 00100522 has not been implemented. (1E)
        [Test]
        public void XCoreDis_F41C()
        {
            AssertCode("@@@", "1CF4");
        }

        // Reko: a decoder for XCore instruction 6237 at address 00100524 has not been implemented. (0C)
        [Test]
        public void XCoreDis_6237()
        {
            AssertCode("@@@", "3762");
        }

        // Reko: a decoder for XCore instruction 076B at address 00100526 has not been implemented. (00)
        [Test]
        public void XCoreDis_076B()
        {
            AssertCode("@@@", "6B07");
        }

        // Reko: a decoder for XCore instruction AE1E at address 00100528 has not been implemented. (15)
        [Test]
        public void XCoreDis_AE1E()
        {
            AssertCode("@@@", "1EAE");
        }

        // Reko: a decoder for XCore instruction 11D4 at address 0010052A has not been implemented. (02)
        [Test]
        public void XCoreDis_11D4()
        {
            AssertCode("@@@", "D411");
        }

        // Reko: a decoder for XCore instruction 1202 at address 0010052C has not been implemented. (02)
        [Test]
        public void XCoreDis_1202()
        {
            AssertCode("@@@", "0212");
        }

        // Reko: a decoder for XCore instruction FAB4 at address 0010052E has not been implemented. (1F)
        [Test]
        public void XCoreDis_FAB4()
        {
            AssertCode("@@@", "B4FA");
        }

        // Reko: a decoder for XCore instruction 50B6 at address 00100530 has not been implemented. (0A)
        [Test]
        public void XCoreDis_50B6()
        {
            AssertCode("@@@", "B650");
        }

        // Reko: a decoder for XCore instruction 7043 at address 00100532 has not been implemented. (0E)
        [Test]
        public void XCoreDis_7043()
        {
            AssertCode("@@@", "4370");
        }

        // Reko: a decoder for XCore instruction A6C3 at address 00100534 has not been implemented. (14)
        [Test]
        public void XCoreDis_A6C3()
        {
            AssertCode("@@@", "C3A6");
        }

        // Reko: a decoder for XCore instruction C510 at address 00100536 has not been implemented. (18)
        [Test]
        public void XCoreDis_C510()
        {
            AssertCode("@@@", "10C5");
        }

        // Reko: a decoder for XCore instruction 0F92 at address 00100538 has not been implemented. (01)
        [Test]
        public void XCoreDis_0F92()
        {
            AssertCode("@@@", "920F");
        }

        // Reko: a decoder for XCore instruction 285A at address 0010053A has not been implemented. (05)
        [Test]
        public void XCoreDis_285A()
        {
            AssertCode("@@@", "5A28");
        }

        // Reko: a decoder for XCore instruction 2321 at address 0010053C has not been implemented. (04)
        [Test]
        public void XCoreDis_2321()
        {
            AssertCode("@@@", "2123");
        }

        // Reko: a decoder for XCore instruction 49E5 at address 0010053E has not been implemented. (09)
        [Test]
        public void XCoreDis_49E5()
        {
            AssertCode("@@@", "E549");
        }

        // Reko: a decoder for XCore instruction 2D33 at address 00100540 has not been implemented. (05)
        [Test]
        public void XCoreDis_2D33()
        {
            AssertCode("@@@", "332D");
        }

        // Reko: a decoder for XCore instruction CF92 at address 00100542 has not been implemented. (19)
        [Test]
        public void XCoreDis_CF92()
        {
            AssertCode("@@@", "92CF");
        }

        // Reko: a decoder for XCore instruction 5FC8 at address 00100544 has not been implemented. (0B)
        [Test]
        public void XCoreDis_5FC8()
        {
            AssertCode("@@@", "C85F");
        }
    }
}