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
using Reko.Arch.PaRisc;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PaRisc
{
    [TestFixture]
    public class PaRiscRewriterTests : RewriterTestBase
    {
        private PaRiscArchitecture arch;
        private MemoryArea mem;

        public PaRiscRewriterTests()
        {
            this.arch = new PaRiscArchitecture("parisc");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        private void BuildTest(string hexBytes)
        {
            var bytes = base.ParseHexPattern(hexBytes);
            this.mem = new MemoryArea(LoadAddress, bytes);
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return new PaRiscRewriter(
                arch,
                mem.CreateBeReader(0),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void PaRiscRw_add()
        {
            BuildTest("08E18624");  // add\tr1,r7,r4,tr
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r4 = r1 + r7",
                "2|T--|if (Test(OV,r4 - 0x00000000)) branch 00100008");
        }

        // memMgmt
        [Test]
        [Ignore("Format is complex; try simpler ones first")]
        public void PaRiscRw_058C7910()
        {
            BuildTest("058C7910");  // @@@
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void PaRiscRw_break()
        {
            BuildTest("00000000");  // break\t00,0000
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__break()");
        }

        [Test]
        public void PaRiscRw_bl()
        {
            BuildTest("E800A3D8");  // bl\t00101EC8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 001001F4 (0)");
        }

        [Test]
        public void PaRiscRw_nop()
        {
            BuildTest("08000240");  // or\tr0,r0,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void PaRiscRw_ldw()
        {
            BuildTest("4BC23FD1");  // ldw\t-24(sr0,r30),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = Mem0[r30 + -24:word32]");
        }

        [Test]
        public void PaRiscRw_bv_n_r2()
        {
            BuildTest("E840D002");  // bv,n\tr0(r2)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void PaRiscRw_bv_n_r3()
        {
            BuildTest("E860D002");  // bv,n\tr0(r3)
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TD-|goto r3");
        }

        [Test]
        public void PaRiscRw_ldx_short()
        {
            BuildTest("0EC41093");  // ldw\t4(sr0,r22),r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = Mem0[r27 + 4:word32]");
        }

        [Test]
        public void PaRiscRw_ldsid()
        {
            BuildTest("02C010A1");  // ldsid\tr22,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_mtsp()
        {
            BuildTest("00011820");  // mtsp\tr1,sr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_be()
        {
            BuildTest("E2C00000");  // be\t0(sr0,r22)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_stw()
        {
            BuildTest("6BC23FD1");  // stw\tr2,-18(sp)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30 + -24:word32] = r2");
        }

        [Test]
        public void PaRiscRw_ldo()
        {
            BuildTest("37DE0080");  // ldo\t40(r30),r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 + 64");
        }

        [Test]
        public void PaRiscRw_ldil()
        {
            BuildTest("23E12000");  // ldil\t00012000,r31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_ble()
        {
            BuildTest("E7E02EF0");  // ble\t7648(sr0,r31)
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r31 + 7648");
        }

        [Test]
        public void PaRiscRw_ldo_copy()
        {
            BuildTest("37E20000");  // ldo\t0(r31),r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r31");
        }

        [Test]
        public void PaRiscRw_cmpb_ult()
        {
            BuildTest("83C78EEC");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 <u r30) branch 0010177C");
        }

        [Test]
        public void PaRiscRw_addib_64()
        {
            BuildTest("AFC1CFD5");  // addibf\t+00000001,r30,00101FB4
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + 1",
                "2|TD-|goto 00101FB4");
        }

        [Test]
        public void PaRiscRw_stb()
        {
            BuildTest("61716B15");  // stb\tr17,-2A76(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + -2678:byte] = SLICE(r17, byte, 0)");
        }

        [Test]
        public void PaRiscRw_sth()
        {
            BuildTest("656e6400");  // sth\tr14,1024(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 4608:word16] = SLICE(r14, word16, 0)");
        }

        [Test]
        public void PaRiscRw_depwi()
        {
            BuildTest("d7c01c1d");  // depwi\t00,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_depwi_z()
        {
            BuildTest("d7c0181d");  // depwi,z\t00,1F,00000003,r30
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fstw()
        {
            BuildTest("27791200");  // fstw\tfr0,-4(r27)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_fldw()
        {
            BuildTest("27791000");  // fldw\t-4(r27),fr0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_addi()
        {
            BuildTest("b40010c2");  // addi,tr\t+00000061,r0,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r0 + 97",
                "2|T--|goto 00100008");
        }

        [Test]
        public void PaRiscRw_cmpb_ugt_n()
        {
            BuildTest("8bd7a06a");  // cmpb,>>,n\tr23,r30,0010003C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 >u r30) branch 0010003C");
        }

        [Test]
        public void PaRiscRw_cmpb_ult_n()
        {
            BuildTest("83178062");  // cmpb,<<,n\tr23,r24,00100038
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 >u r30) branch 00100008");
        }

        [Test]
        public void PaRiscRw_extrw_u()
        {
            BuildTest("d0a619fa");  // extrw,u\tr5,0F,06,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = (uint32) SLICE(r5, word6, 11)");
        }

        [Test]
        public void PaRiscRw_addil()
        {
            BuildTest("2B6AAAAA");  // addil\tL%55595000,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + 1431916544");
        }

        [Test]
        public void PaRiscRw_stw_ma()
        {
            BuildTest("6fc30100");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = r30 + 128",
                "2|L--|r30 = v4",
                "3|L--|Mem0[v4:word32] = r3");
        }

        [Test]
        public void PaRiscRw_stw_mb()
        {
            BuildTest("6fc32100");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = r30 + 4224",
                "2|L--|r30 = v4",
                "3|L--|Mem0[v4:word32] = v3");
        }

        [Test]
        public void PaRiscRw_stw_ma_negative_offset()
        {
            BuildTest("6FC35555");  // stw,ma\tr3,128(r30)
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|@@@",
                "2|L--|@@@",
                "3|L--|@@@");
        }

        [Test]
        public void PaRiscRw_ldb()
        {
            BuildTest("0fe01018");  // ldb\t0(sr0,r31),r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = (uint32) Mem0[r31:byte]");
        }

        [Test]
        public void PaRiscRw_stb_disp()
        {
            BuildTest("0f201212");  // stb\tr0,9(r25)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void PaRiscRw_addil_neg()
        {
            BuildTest("2B7FFFFF");	// addil	L%-00000800,r27,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r27 + -2048");
        }

        [Test]
        public void PaRiscRw_shladd()
        {
            BuildTest("0BE30A84");	// shladd 2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r31 + (r3 << 2)");
        }

        [Test]
        public void PaRiscRw_addib()
        {
            BuildTest("AC7F5FDD");	// addibf	-1,r3,00003140
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + -1",
                "2|TD-|if (r3 >= 0x00000000) branch 000FFFF4");
        }

        [Test]
        public void PaRiscRw_ldw_mb()
        {
            BuildTest("4FC33F81");	// ldw,mb
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r30 = r30 + -64",
                "2|L--|r3 = Mem0[r30:word32]");
        }

        // If you're making a backward jump, annul the following instruction
        // only if you don't take the branch back.
        [Test]
        public void PaRiscRw_addib_annul_back()
        {
            BuildTest("A45930FF"); //  "addib,=\t-00000004,r2,000FF884");
            AssertCode(
                "0|TD-|00100000(4): 3 instructions",
                "1|L--|r2 = r2 + -4",
                "2|T--|if (r2 != 0x00000000) branch 00100008",
                "3|TD-|goto 000FF884");
        }

        // If you're making a forward jump, annul the following instruction
        // only if you do take the branch forward.
        [Test]
        public void PaRiscRw_addib_annul_forward()
        {
            BuildTest("A459200A"); //  "addib,*=\t-00000004,r2,0010F7F0");
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 + -4",
                "2|T--|if (r2 == 0x00000000) branch 0010000C");
        }
    }
}
