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
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PowerPC
{
    public class PowerPcXenonDisassemblerTests : DisassemblerTestBase<PowerPcInstruction>
    {
        private PowerPcBe32Architecture arch;

        public PowerPcXenonDisassemblerTests()
        {
            this.arch = new PowerPcBe32Architecture(CreateServiceContainer(), "ppc-be-32", new Dictionary<string, object>
            {
                { ProcessorOption.Model, "Xenon" }
            });
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; } = Address.Ptr32(0x00100000);

        private void AssertCode(uint instr, string sExpected)
        {
            var i = DisassembleWord(instr);
            Assert.AreEqual(sExpected, i.ToString());
        }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var i = base.DisassembleHexBytes(hexBytes);
            if (sExpected != i.ToString()) // && i.Mnemonic == Mnemonic.nyi)
            {
                Assert.AreEqual(sExpected, i.ToString());
            }
        }

        [Test]
        public void PPCDis_Altivec_ops()
        {
            AssertCode(0x10c6600a, "vaddfp\tv6,v6,v12");
            AssertCode(0x10000ac6, "vcmpgtfp\tv0,v0,v1");
            AssertCode(0x118108c6, "vcmpeqfp\tv12,v1,v1");
            AssertCode(0x10ed436e, "vmaddfp\tv7,v13,v13,v8");
            AssertCode(0x10a9426e, "vmaddfp\tv5,v9,v9,v8");
            AssertCode(0x10200a8c, "vspltw\tv1,v1,00");
            AssertCode(0x1160094a, "vrsqrtefp\tv11,v1");
            AssertCode(0x102b406e, "vmaddfp\tv1,v11,v1,v8");
            AssertCode(0x1020014a, "vrsqrtefp\tv1,v0");
            AssertCode(0x116b0b2a, "vsel\tv11,v11,v1,v12");
            AssertCode(0x1000012c, "vsldoi\tv0,v0,v0,04");
            AssertCode(0x101f038c, "vspltisw\tv0,-00000001");
            AssertCode(0x1000028c, "vspltw\tv0,v0,00");
            AssertCode(0x112c484a, "vsubfp\tv9,v12,v9");
            AssertCode(0x118000c6, "vcmpeqfp\tv12,v0,v0");
            AssertCode(0x11ad498c, "vmrglw\tv13,v13,v9");
            AssertCode(0x118c088c, "vmrghw\tv12,v12,v1");
            AssertCode(0x125264c4, "vxor\tv18,v18,v12");
        }

        [Test]
        public void PPCDis_Xenon_mfvsrd()
        {
            AssertCode(0x7C7C2066, "mffprd\tr28,f3");
        }

        [Test]
        public void PPCDis_Xenon_regression8()
        {
            AssertCode(0x13E058C7, "lvx128\tv63,r0,r11");
            AssertCode(0x11400484, "vor\tv10,v0,v0");
            AssertCode(0x100004c4, "vxor\tv0,v0,v0");
            AssertCode(0x11A91D03, "stvlx128\tv13,r9,r3");
            AssertCode(0x1001350B, "stvlx128\tv64,r1,r6");
            AssertCode(0x7C6BF82A, "ldx\tr3,r11,r31");
            AssertCode(0x7D0018A8, "ldarx\tr8,r0,r3");
            AssertCode(0x7D40592D, "stwcx.\tr10,r0,r11");
            AssertCode(0x7DA10164, "mtmsrd\tr13,01");
            AssertCode(0x7D4019AD, "stdcx.\tr10,r0,r3");

            AssertCode(0x13040000, "vaddubm\tv24,v4,v0");
            AssertCode(0x10011002, "vmaxub\tv0,v1,v2");
            AssertCode(0x10000022, "vmladduhm\tv0,v0,v0,v0");
            AssertCode(0x10000042, "vmaxuh\tv0,v0,v0");
            AssertCode(0x11b268e2, "vmladduhm\tv13,v18,v13,v3");
            AssertCode(0x12020100, "vadduqm\tv16,v2,v0");
            AssertCode(0x1003c200, "vaddubs\tv0,v3,v24");
            AssertCode(0x10601a8c, "vspltw\tv3,v3,00");
            AssertCode(0x10010401, "bcdadd.\tv0,v1,v0,00");
            AssertCode(0x117d9406, "vcmpequb.\tv11,v29,v18");
        }

        [Test]
        public void PPCDis_Xenon_stbcx()
        {
            AssertCode(0x7D0A656C, "stbcx.\tr8,r10,r12");
        }

        [Test]
        public void PPCDis_Xenon_upkhsb128()
        {
            AssertCode(0x1B003B84, "vupkhsb128\tv56,v7");
        }

        [Test]
        public void PPCDis_Xenon_vaddfp128()
        {
            //| 0 0 0 1 0 1 | VD128 | VA128 | VB128 | A | 0 0 0 0 | a | 1 | VDh | VBh |
            // 000101 01010 11111 10101 1 0000 1 1 10 01
            // 0001 0101 0101 1111 1010 1100 0011 1001
            AssertCode(0x155FAC39, "vaddfp128\tv74,v127,v53");
        }

        [Test]
        public void PPCDis_Xenon_vaddudm()
        {
            AssertCode(0x104810C0, "vaddudm\tv2,v8,v2");
        }

        [Test]
        public void PPCDis_Xenon_vandc128()
        {
            AssertCode(0x15BAAA71, "vandc128\tv13,v58,v53");
        }

        [Test]
        public void PPCDis_Xenon_VMX128()
        {
            AssertCode(0x102038C3, "lvx128\tv1,r0,r7");         // 04 - 0C3(195)
            AssertCode(0x102338CB, "lvx128\tv65,r3,r7");        // 04 - 0CB(203)
            AssertCode(0x13C100CF, "lvx128\tv126,r1,r0");       // 04 - 0CF(207)
            //AssertCode(0x13A05187, "@@@"); // 04 - 187(391)
            AssertCode(0x116021C3, "stvx128\tv11,r0,r4");       // 04 - 1C3(451)
            AssertCode(0x13C031C7, "stvx128\tv62,r0,r6");       // 04 - 1C7(455)
            AssertCode(0x100B61CB, "stvx128\tv64,r11,r12");     // 04 - 1CB(459)
            AssertCode(0x13C161CF, "stvx128\tv126,r1,r12");     // 04 - 1CF(463)
            //AssertCode(0x13D29A35, "@@@"); // 04 - 235(565)
            AssertCode(0x13C55C47, "lvrx128\tv62,r5,r11");      // 04 - 447(1095)
            AssertCode(0x13A05C07, "lvlx128\tv61,r0,r11");      // 04 - 407(1031)
            AssertCode(0x13E04507, "stvlx128\tv63,r0,r8");      // 04 - 507(1287)
            AssertCode(0x13E85D47, "stvrx128\tv63,r8,r11");     // 04 - 547(1351)

            AssertCode(0x1497B0B1, "vmulfp128\tv4,v55,v54");    // 05 - 009(9)
            AssertCode(0x1400E851, "vsubfp128\tv0,v0,v61");     // 05 - 005(5)
            AssertCode(0x14020100, "vperm128\tv0,v2,v0,v4");    // 05 - 010(16)
            AssertCode(0x177B011C, "vmaddcfp128\tv123,v27,v0,v123"); // 05 - 011(17)
            AssertCode(0x173FE1B5, "vmsub3fp128\tv57,v63,v60"); // 05 - 019(25)
            AssertCode(0x157FA9F1, "vmsub4fp128\tv11,v63,v53"); // 05 - 01D(29)
            AssertCode(0x16D6BA35, "vand128\tv54,v54,v55");     // 05 - 021(33)
            //AssertCode(0x15BAAA71, "@@@"); // 05 - 025(37)
            AssertCode(0x15B8C2F1, "vor128\tv13,v56,v56");      // 05 - 02D(45)
            AssertCode(0x145AE331, "vxor128\tv2,v58,v60");      // 05 - 031(49)

            AssertCode(0x18000000, "vcmpeqfp128\tv0,v0,v0");    // 06 - 000(0)
            AssertCode(0x187EF823, "vcmpeqfp128\tv3,v62,v127"); // 06 - 002(2)
            AssertCode(0x1B5FF8F5, "vslw128\tv58,v63,v63");     // 06 - 00F(15)
            AssertCode(0x18F7E121, "vcmpgtfp128\tv7,v55,v60");  // 06 - 012(18)
            AssertCode(0x18280186, "vcmpbfp128\tv33,v8,v64");   // 06 - 018(24)
            AssertCode(0x195CB9F1, "vsrw128\tv10,v60,v55");     // 06 - 01F(31)
            //AssertCode(0x1BA5AA15, "@@@");    // 06 - 021(33)  - permutation odd encoding
            AssertCode(0x1AC2FA35, "vcfpsxws128\tv54,v63,+00000002"); // 06 - 023(35)
            //AssertCode(0x1918F251, "@@@");    // 06 - 025(37)
            //AssertCode(0x180EB291, "@@@");    // 06 - 029(41)
            AssertCode(0x1BDEE2A5, "vmaxfp128\tv62,v62,v60");   // 06 - 02A(42)
            AssertCode(0x1801F2B1, "vcsxwfp128\tv0,v62,+00000001");    // 06 - 02B(43)
            //AssertCode(0x1AE1D2D5, "@@@");    // 06 - 02D(45)
            AssertCode(0x1BFFF2E5, "vminfp128\tv63,v63,v62");   // 06 - 02E(46)
            //AssertCode(0x1B04AB15, "@@@");    // 06 - 031(49)
            AssertCode(0x1B1FF325, "vmrghw128\tv56,v63,v62");   // 06 - 032(50)
            //AssertCode(0x1BA0AB35, "@@@");      // 06 - 033(51)
            //AssertCode(0x1B1BD355, "@@@");    // 06 - 035(53)
            AssertCode(0x1BFFF365, "vmrglw128\tv63,v63,v62");   // 06 - 036(54)
            AssertCode(0x1BC0DB75, "vrfin128\tv62,v59");        // 06 - 037(55)
            //AssertCode(0x1B4CD395, "@@@");    // 06 - 039(57)
            //AssertCode(0x18ADA3D1, "@@@");    // 06 - 03D(61)


            AssertCode(0x18A0DBF1, "vrfiz128\tv5,v59");         // 06 - 03F(63)
            AssertCode(0x1BCECCED, "vcmpgefp128.\tv126,v110,v57"); // 06 - 04E(78)
            AssertCode(0x1800F631, "vrefp128\tv0,v62");         // 06 - 063(99)
            AssertCode(0x19000640, "vcmpequw128.\tv8,v64,v0"); // 06 - 064(100)
            AssertCode(0x1800F671, "vrsqrtefp128\tv0,v62"); // 06 - 067(103)
            AssertCode(0x1BA0EEB5, "vexptefp128\tv61,v61"); // 06 - 06B(107)
            AssertCode(0x1AA0EEF5, "vlogefp128\tv53,v61");      // 06 - 06F(111)
            //AssertCode(0x19C49F15, "vrlimi128\tv46,v51,04,02"); // 06 - 071(113)
            AssertCode(0x1923CF31, "vspltw128\tv9,v57,03");     // 06 - 073(115)
            //AssertCode(0x18019F51, "vrlimi128\tv0,v51,01,02");  // 06 - 075(117)
            //AssertCode(0x1B600774, "vspltisw128\tv59,v0,+00000000");  // 06 - 077(119)
            //AssertCode(0x19ACFF91, "vrlimi128\tv13,v63,0C,03"); // 06 - 079(121)
            //AssertCode(0x18099FD1, "vrlimi128\tv0,v51,09,02");  // 06 - 07D(125)
            //AssertCode(0x1B24DFF5, "vupkd3d128\tv57,v59,04");   // 06 - 07F(127)
        }

        ///////////////////////////////////////////////

        [Test]
        public void PPCDis_Xenon_vcfpsxws128()
        {
            AssertCode(0x1BC5A215, "vcfpsxws128\tv62,v52,+00000005");
        }

        [Test]
        public void vcfpuxws128_Xenon_vcfpuxws128()
        {
            AssertCode(0x18F4D251, "vcfpuxws128\tv7,v58,14");
        }

        [Test]
        public void PPCDis_Xenon_vcsxwfp128()
        {
            AssertCode(0x194E9291, "vcsxwfp128\tv10,v50,+0000000E");
        }

        [Test]
        public void PPCDis_Xenon_vcuxwfp128()
        {
            AssertCode(0x1AE1D2D5, "vcuxwfp128\tv55,v58,01");
            AssertCode(0x1BD2AAD5, "vcuxwfp128\tv62,v53,12");
        }

        [Test]
        public void PPCDis_Xenon_vrfim128()
        {
            AssertCode(0x1BA0AB35, "vrfim128\tv61,v53");
        }

        [Test]
        public void PPCDis_Xenon_vrfip128()
        {
            AssertCode(0x18069391, "vrfip128\tv0,v50");
        }

        [Test]
        public void PPCDis_Xenon_vsububm()
        {
            AssertCode(0x12000400, "vsububm\tv16,v0,v0");
        }

        [Test]
        public void PPCDis_Xenon_vsubcuq()
        {
            AssertCode(0x12000540, "vsubcuq\tv16,v0,v0");
        }

        [Test]
        public void PPCDis_Xenon_vpkshss128()
        {
            AssertCode(0x1487020F, "vpkshss128\tv100,v7,v96");
        }

        [Test]
        public void PPCDis_Xenon_vcmpgtsb()
        {
            AssertCode(0x10000306, "vcmpgtsb\tv0,v0,v0");
        }


        [Test]
        public void PPCDis_Xenon_vexptefp()
        {
            AssertCode(0x1268018A, "vexptefp\tv19,v0");
        }


        [Test]
        public void PPCDis_Xenon_vmhaddshs()
        {
            AssertCode(0x12161D20, "vmhaddshs\tv16,v22,v3,v20");
        }

        [Test]
        public void PPCDis_Xenon_vmaddfp128()
        {
            AssertCode(0x153200F9, "vmaddfp128\tv73,v50,v32,v73");
        }

        [Test]
        public void PPCDis_Xenon_vmsumuhm()
        {
            AssertCode(0x1268FFE6, "vmsumuhm\tv19,v8,v31,v31");
        }

        [Test]
        public void PPCDis_Xenon_vpkuhus128()
        {
            AssertCode(0x14AF0341, "vpkuhus128\tv5,v15,v32");
        }

        [Test]
        public void PPCDis_Xenon_vpkuwum128()
        {
            AssertCode(0x176D5BAD, "vpkuwum128\tv123,v45,v43");
        }

        [Test]
        public void PPCDis_Xenon_vpkshus128()
        {
            AssertCode(0x16181A6C, "vpkshus128\tv112,v56,v3");
            AssertCode(0x16481660, "vpkshus128\tv18,v104,v2");
        }

        [Test]
        public void PPCDis_Xenon_vmsumubm()
        {
            AssertCode(0x107413A4, "vmsumubm\tv3,v20,v2,v14");
        }

        [Test]
        public void PPCDis_Xenon_vmulesb()
        {
            AssertCode(0x13A41308, "vmulesb\tv29,v4,v2");
        }

        [Test]
        public void PPCDis_Xenon_vmuleuw()
        {
            AssertCode(0x13A40288, "vmuleuw\tv29,v4,v0");
        }

        [Test]
        public void PPCDis_Xenon_vmulosh()
        {
            AssertCode(0x113C1148, "vmulosh\tv9,v28,v2");
        }

        [Test]
        public void PPCDis_Xenon_vmuloub()
        {
            AssertCode(0x10100008, "vmuloub\tv0,v16,v0");
        }

        [Test]
        public void PPCDis_Xenon_vnor128()
        {
            AssertCode(0x16781690, "vnor128\tv19,v88,v2");
        }

        [Test]
        public void PPCDis_Xenon_vpkswus128()
        {
            AssertCode(0x16A816C4, "vpkswus128\tv53,v72,v2");
        }

        [Test]
        public void PPCDis_Xenon_vpmsumh()
        {
            AssertCode(0x11901448, "vpmsumh\tv12,v16,v2");
        }

        [Test]
        public void PPCDis_Xenon_vrlh()
        {
            AssertCode(0x10381044, "vrlh\tv1,v24,v2");
        }

        [Test]
        public void PPCDis_Xenon_vsel128()
        {
            AssertCode(0x168C1778, "vsel128\tv84,v108,v2,v84");
        }

        [Test]
        public void PPCDis_Xenon_vslo128()
        {
            AssertCode(0x152823BC, "vslo128\tv105,v40,v4");
        }

        [Test]
        public void PPCDis_Xenon_vsldoi128_allbitsset()
        {
            // 000100 11111 11111 11111 1 1011 1 1 11 11
            AssertCode(0x13FFFEFF, "vsldoi128\tv127,v127,v127,0B");
        }

        [Test]
        public void PPCDis_Xenon_vspltisb()
        {
            AssertCode(0x12F8130C, "vspltisb\tv23,-00000008");
        }

        [Test]
        public void PPCDis_Xenon_vspltw128()
        {
            AssertCode(0x1BCFFB15, "vspltw128\tv62,v63,0F");
        }

        [Test]
        public void PPCDis_Xenon_vsrah()
        {
            AssertCode(0x13201344, "vsrah\tv25,v0,v2");
        }

        [Test]
        public void PPCDis_Xenon_vsrb()
        {
            AssertCode(0x11FC1204, "vsrb\tv15,v28,v2");
        }

        [Test]
        public void PPCDis_Xenon_vupkd3d128()
        {
            AssertCode(0x18ADA3D1, "vupkd3d128\tv5,v52,0D");
        }

        [Test]
        public void PPCDis_Xenon_vnmsubfp()
        {
            AssertCode(0x118C682F, "vnmsubfp\tv12,v12,v0,v13");
        }
    }
}
