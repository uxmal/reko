﻿#region License
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
using Reko.Arch.Vax;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Vax
{
    [TestFixture]
    public class VaxRewriterTests : RewriterTestBase
    {
        private VaxArchitecture arch = new VaxArchitecture();
        private Address baseAddr = Address.Ptr32(0x0010000);
        private VaxProcessorState state;
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new VaxRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        [SetUp]
        public void Setup()
        {
            state = (VaxProcessorState)arch.CreateProcessorState();
        }

        private void BuildTest(params byte[] bytes)
        {
            image = new MemoryArea(baseAddr, bytes);
        }

        [Test]
        public void VaxRw_acbb()
        {
            BuildTest(0x9D, 0x53, 0x09, 0x52, 0xF3, 0xFF);	// acbb	
            AssertCode(
                "0|T--|00010000(6): 4 instructions",
                "1|L--|v4 = (byte) r2 + 0x09",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|VZN = cond(v4)",
                "4|T--|if (v4 <= (byte) r3) branch 0000FFF9");
        }

        [Test]
        public void VaxRw_acbd()
        {
            BuildTest(0x6F, 0x20, 0x25, 0x64, 0x2E, 0xF0);	// acbd	#8,#13,(r4),00001E1B
            AssertCode(
                "0|T--|00010000(6): 4 instructions",
                "1|L--|v3 = Mem0[r4:real64] + 13.0",
                "2|L--|Mem0[r4:real64] = v3",
                "3|L--|VZN = cond(v3)",
                "4|T--|if (v3 <= 8.0) branch 0000F034");
        }

        [Test]
        public void VaxRw_acbf()
        {
            BuildTest(0x4F, 0x53, 0x09, 0x52, 0xF3, 0xFF);	// acbf	
            AssertCode(
                "0|T--|00010000(6): 3 instructions",
                "1|L--|r2 = r2 + 1.125F",
                "2|L--|VZN = cond(r2)",
                "3|T--|if (r2 <= r3) branch 0000FFF9");
        }

        [Test]
        public void VaxRw_acbl()
        {
            BuildTest(0xF1, 0x53, 0x09, 0x52, 0xF3, 0xFF);	// acbl	
            AssertCode(
                "0|T--|00010000(6): 3 instructions",
                "1|L--|r2 = r2 + 0x00000009",
                "2|L--|VZN = cond(r2)",
                "3|T--|if (r2 <= r3) branch 0000FFF9");
        }

        [Test]
        public void VaxRw_acbw()
        {
            BuildTest(0x3D, 0x53, 0x09, 0x52, 0xF3, 0xFF);	// acbw	
            AssertCode(
                "0|T--|00010000(6): 4 instructions",
                "1|L--|v4 = (word16) r2 + 0x0009",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|VZN = cond(v4)",
                "4|T--|if (v4 <= (word16) r3) branch 0000FFF9");
        }

    
        [Test]
        public void VaxRw_addb2()
        {
            BuildTest(0x80, 0x01, 0xCC, 0x00, 0xFC, 0x00, 0x3C);	// addb2	#01,+03C00FC400(ap)
            AssertCode(
                "0|L--|00010000(7): 3 instructions",
                "1|L--|v3 = Mem0[ap + 0x3C00FC00:byte] + 0x01",
                "2|L--|Mem0[ap + 0x3C00FC00:byte] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_addb3()
        {
            BuildTest(0x81, 0x01, 0x55, 0xB1, 0x00, 0x01);	// addb3	#01,r5,+0100(r1)
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v4 = (byte) r5 + 0x01",
                "2|L--|Mem0[r1 + 256:byte] = v4",
                "3|L--|CVZN = cond(v4)");
        }
        [Test]
        public void VaxRw_addd2()
        {
            BuildTest(0x60, 0x02, 0xC4, 0x04, 0xE4, 0x04, 0xE0);	// addd2	#0.625,-1FFB1BFC(r4)
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v3 = Mem0[r4 + 0xE004E404:real64] + 0.625",
                "2|L--|Mem0[r4 + 0xE004E404:real64] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_addd3()
        {
            BuildTest(0x61, 0x01, 0x52, 0x75);	// addd3	#0.5625,r2,-(r5)
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|r5 = r5 - 0x00000008",
                "2|L--|v6 = r3_r2 + 0.5625",
                "3|L--|Mem0[r5:real64] = v6",
                "4|L--|ZN = cond(v6)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_addf2()
        {
            BuildTest(0x40, 0x01, 0x52);	// addf2	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|r2 = r2 + 0.5625F",
                "2|L--|ZN = cond(r2)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_addf3()
        {
            BuildTest(0x41, 0x52, 0x53, 0x54);	// addf3	
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|r4 = r3 + r2",
                "2|L--|ZN = cond(r4)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_addl2()
        {
            BuildTest(0xC0, 0x04, 0xAC, 0x08);	// addl2	#00000004,+08(ap)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v3 = Mem0[ap + 8:word32] + 0x00000004",
                "2|L--|Mem0[ap + 8:word32] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_addl3()
        {
            BuildTest(0xC1, 0x04, 0x54, 0x53);	// addl3	#00000004,r4,r3
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|r3 = r4 + 0x00000004",
                "2|L--|CVZN = cond(r3)");
        }

        [Test]
        public void VaxRw_addp4()
        {
            BuildTest(0x20, 0x04, 0x64, 0x04, 0x60);	// addp4	#0004,r4,#0004,r0
            AssertCode(
                "0|L--|00010000(5): 2 instructions",
                "1|L--|VZN = vax_addp4(0x0004, Mem0[r4:ptr32], 0x0004, Mem0[r0:ptr32])",
                "2|L--|C = false");
        }

        [Test]
        public void VaxRw_addp6()
        {
            BuildTest(0x21, 0x04, 0x52, 0x04, 0x53, 0x04, 0x54);	// addp6	#0004,-(r2),#0004,-(r3),#0004,-(r4)
            AssertCode(
                "0|L--|00010000(7): 2 instructions",
                "1|L--|VZN = vax_addp6(0x0004, r2, 0x0004, r3, 0x0004, r4)",
                "2|L--|C = false");
        }

        [Test]
        public void VaxRw_addw2()
        {
            BuildTest(0xA0, 0x14, 0xB0, 0xC2, 0xE7);	// addw2	#0014,-183E(r0)
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|v3 = Mem0[r0 + -6206:word16] + 0x0014",
                "2|L--|Mem0[r0 + -6206:word16] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_addw3()
        {
            BuildTest(0xA1, 0x14, 0xB0, 0xC2, 0xE7, 0x55);	// addw3	#0014,-183E(r0),r5
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v4 = Mem0[r0 + -6206:word16] + 0x0014",
                "2|L--|r5 = DPB(r5, v4, 0)",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_adwc()
        {
            BuildTest(0xD8, 0x63, 0x54);	// adwc	
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|r4 = r4 + Mem0[r3:word32] + C",
                "2|L--|CVZN = cond(r4)");
        }

        [Test]
        public void VaxRw_aobleq()
        {
            BuildTest(0xF3, 0x02, 0x54, 0xF0);	// aobleq	#00000002,r4,0000A7A8
            AssertCode(
                "0|T--|00010000(4): 3 instructions",
                "1|L--|r4 = r4 + 0x00000001",
                "2|L--|CVZN = cond(r4)",
                "3|T--|if (r4 <= 0x00000002) branch 0000FFF4");
        }

        [Test]
        public void VaxRw_ashl()
        {
            BuildTest(0x78, 0x8F, 0x05, 0x53, 0x52);	// ashl	#05,r3,r2
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|r2 = r3 << 5",
                "2|L--|VZN = cond(r2)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_ashp()
        {
            BuildTest(0xF8, 0x08, 0x53, 0x52, 0x51, 0x08, 0x54);	// ashp	
            AssertCode(
                "0|L--|00010000(7): 2 instructions",
                "1|L--|VZN = vax_ashp(0x08, r3, (word16) r2, r1, 0x0008, r4)");
        }

        [Test]
        public void VaxRw_ashq()
        {
            BuildTest(0x79, 0x02, 0x59, 0x5B);	// ashq	#02,r10,r11
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|ap_r11 = r10_r9 << 2",
                "2|L--|VZN = cond(ap_r11)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_bbc()
        {
            BuildTest(0xE1, 0x07, 0xC6, 0xF0, 0x02, 0x01, 0x00, 0x07);	// bbc	#00000007,+000102F0(r6),0000A7D8
            AssertCode(
                "0|T--|00010000(8): 1 instructions",
                "1|T--|if ((Mem0[r6 + 0x000102F0:word32] & 0x00000001 << 0x00000007) == 0x00000000) branch 0001000F");
        }

        [Test]
        public void VaxRw_bbcc()
        {
            BuildTest(0xE5, 0x02, 0x52, 0x34);	// bbcc	#02,r2,10038
            AssertCode(
                "0|T--|00010000(4): 3 instructions",
                "1|L--|v3 = r2 & 1 << 0x00000002",
                "2|L--|r2 = r2 & ~(1 << 0x00000002)",
                "3|T--|if (v3 == 0x00000000) branch 00010038");
        }

        [Test]
        public void VaxRw_bbs()
        {
            BuildTest(0xE0, 0x03, 0xA2, 0x14, 0x07);	// bbs	#00000003,+14(r2),00009CB8
            AssertCode(
                "0|T--|00010000(5): 1 instructions",
                "1|T--|if ((Mem0[r2 + 20:word32] & 0x00000001 << 0x00000003) != 0x00000000) branch 0001000C");
        }

        [Test]
        public void VaxRw_beql()
        {
            BuildTest(0x13, 0x2E);	// beql	000080FD
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00010030");
        }

        [Test]
        public void VaxRw_bgequ()
        {
            BuildTest(0x1E, 0x2B);	// bgequ	00009866
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0001002D");
        }

        [Test]
        public void VaxRw_bgeq()
        {
            BuildTest(0x18, 0x03);	// bgeq	00008378
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(GE,N)) branch 00010005");
        }

        [Test]
        public void VaxRw_bgtr()
        {
            BuildTest(0x14, 0x03);	// bgtr	00008178
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(GT,ZN)) branch 00010005");
        }

        [Test]
        public void VaxRw_bgtru()
        {
            BuildTest(0x1A, 0x29);	// bgtru	0000B43C
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(UGT,CZ)) branch 0001002B");
        }

        [Test]
        public void VaxRw_blbc()
        {
            BuildTest(0xE9, 0x50, 0x03);	// blbc	r0,00009011
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((r0 & 0x00000001) == 0x00000000) branch 00010006");
        }

        [Test]
        public void VaxRw_blbs()
        {
            BuildTest(0xE8, 0x50, 0x04);    // blbs	r0,0000809C
            AssertCode(
               "0|T--|00010000(3): 1 instructions",
               "1|T--|if ((r0 & 0x00000001) != 0x00000000) branch 00010007");
        }

        [Test]
        public void VaxRw_bleq()
        {
            BuildTest(0x15, 0x42);	// bleq	00008128
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(LE,ZN)) branch 00010044");
        }

        [Test]
        public void VaxRw_blequ()
        {
            BuildTest(0x1B, 0x16);	// blequ	00008F6E
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(ULE,CZ)) branch 00010018");
        }

        [Test]
        public void VaxRw_blss()
        {
            BuildTest(0x19, 0x04);	// blss	00008155
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(LT,N)) branch 00010006");
        }

        [Test]
        public void VaxRw_blssu()
        {
            BuildTest(0x1F, 0x04);	// blss	00008155
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00010006");
        }

        [Test]
        public void VaxRw_bneq()
        {
            BuildTest(0x12, 0x02);	// bneq	00008081
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00010004");
        }

        [Test]
        public void VaxRw_bicb2()
        {
            BuildTest(0x8A, 0x8F, 0x80, 0x50);	// bicb2	#80,r0
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v3 = (byte) r0 & ~0x80",
                "2|L--|r0 = DPB(r0, v3, 0)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bicb3()
        {
            BuildTest(0x8B, 0x8F, 0xF0, 0xC6, 0xF4, 0x02, 0x01, 0x00, 0x52);	// bicb3	#F0,+000102F4(r6),r2
            AssertCode(
                "0|L--|00010000(9): 5 instructions",
                "1|L--|v4 = Mem0[r6 + 0x000102F4:byte] & ~0xF0",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bicl2()
        {
            BuildTest(0xCA, 0x8F, 0x80, 0xFF, 0xFF, 0xFF, 0x52);	// bicl2	#FFFFFF80,r2
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r2 = r2 & ~0xFFFFFF80",
                "2|L--|ZN = cond(r2)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_bicl3()
        {
            BuildTest(0xCB, 0x8F, 0xFE, 0xFF, 0xFF, 0xFF, 0x52, 0x53);	// bicl3	#FFFFFFFE,r2,r3
            AssertCode(
                "0|L--|00010000(8): 4 instructions",
                "1|L--|r3 = r2 & ~0xFFFFFFFE",
                "2|L--|ZN = cond(r3)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_bicw2()
        {
            BuildTest(0xAA, 0xC0, 0xA9, 0xEE, 0xF8, 0xF1, 0xCD, 0xFC, 0xEF, 0xE6, 0xF4);	// bicw2	-0E071157(r0),-0B191004(fp)
            AssertCode(
                "0|L--|00010000(11): 5 instructions",
                "1|L--|v4 = Mem0[fp + 0xF4E6EFFC:word16] & ~Mem0[r0 + 0xF1F8EEA9:word16]",
                "2|L--|Mem0[fp + 0xF4E6EFFC:word16] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bicw3()
        {
            BuildTest(0xAB, 0x8F, 0x00, 0x00, 0x52, 0xAE, 0x0E);	// bicw3	#0000,r2,+0E(sp)
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v4 = (word16) r2 & ~0x0000",
                "2|L--|Mem0[sp + 14:word16] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }


        [Test]
        public void VaxRw_bisb2()
        {
            BuildTest(0x88, 0xC1, 0xFE, 0x7F, 0xD0, 0x50, 0x52);	// bisb2	+50D07FFE(r1),r2
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v4 = (byte) r2 | Mem0[r1 + 0x50D07FFE:byte]",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bisb3()
        {
            BuildTest(0x89, 0xC3, 0xD4, 0x50, 0x04, 0xD5, 0x50, 0x7B);	// bisb3	-2AFBAF2C(r3),r0,-(r11)
            AssertCode(
                "0|L--|00010000(8): 6 instructions",
                "1|L--|r11 = r11 - 0x00000001", 
                "2|L--|v5 = (byte) r0 | Mem0[r3 + 0xD50450D4:byte]",
                "3|L--|Mem0[r11:byte] = v5",
                "4|L--|ZN = cond(v5)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_bisl2()
        {
            BuildTest(0xC8, 0xC1, 0xFE, 0x7F, 0xD0, 0x50, 0x54);	// bisl2	+50D07FFE(r1),r4
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r4 = r4 | Mem0[r1 + 0x50D07FFE:word32]",
                "2|L--|ZN = cond(r4)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_bisl3()
        {
            BuildTest(0xC9, 0xC2, 0xA8, 0x0A, 0x01, 0x00, 0xC2, 0xAC, 0x0A, 0x01, 0x00, 0x5C);	// bisl3	+00010AA8(r2),+00010AAC(r2),ap
            AssertCode(
                "0|L--|00010000(12): 4 instructions",
                "1|L--|ap = Mem0[r2 + 0x00010AAC:word32] | Mem0[r2 + 0x00010AA8:word32]",
                "2|L--|ZN = cond(ap)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_bisw2()
        {
            BuildTest(0xA8, 0x05, 0xC6, 0x22, 0x02, 0x01, 0x00);	// bisw2	#0005,+00010222(r6)
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v3 = Mem0[r6 + 0x00010222:word16] | 0x0005",
                "2|L--|Mem0[r6 + 0x00010222:word16] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bisw3()
        {
            BuildTest(0xA9, 0x01, 0xD9, 0x00, 0xD9, 0x00);	// bisw3	#0001,+00(r9),+00(r9)
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|v3 = Mem0[Mem0[r9 + 0:word32]:word16] | 0x0001",
                "2|L--|Mem0[Mem0[r9 + 0:word32]:word16] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_bpt()
        {
            BuildTest(0x03);	// bpt	
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|vax_bpt()");
        }

        [Test]
        public void VaxRw_brb()
        {
            BuildTest(0x11, 0x05);	// brb	000080BA
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|goto 00010007");
        }

        [Test]
        public void VaxRw_brw()
        {
            BuildTest(0x31, 0x9C, 0x01);	// brw	00008314
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|goto 0001019F");
        }

        [Test]
        public void VaxRw_bsbb()
        {
            BuildTest(0x10, 0x02);	// bsbb	0000A7A0
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|call 00010004 (4)");
        }

        [Test]
        public void VaxRw_bsbw()
        {
            BuildTest(0x30, 0xE2, 0xFE);	// bsbw	0000A5FF
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|call 0000FEE5 (4)");
        }

        [Test]
        public void VaxRw_bvc()
        {
            BuildTest(0x1C, 0x00);	// bvc	0000B192
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(NO,V)) branch 00010002");
        }

        [Test]
        public void VaxRw_bvs()
        {
            BuildTest(0x1D, 0x00);	// bvs	000106ED
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(OV,V)) branch 00010002");
        }

        [Test]
        [Ignore("VAX calls are awful")]
        public void VaxRw_callg()
        {
            BuildTest(0xFA);	// callg	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("VAX calls are awful")]
        public void VaxRw_calls()
        {
            BuildTest(0xFB, 0x00, 0xEF, 0x0E, 0x54, 0x00, 0x00);	// calls	#00,0000D420
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void VaxRw_clrb()
        {
            BuildTest(0x94, 0x50);	// clrb	r0
            AssertCode(
                "0|L--|00010000(2): 6 instructions",
                "1|L--|v3 = 0x00",
                "2|L--|r0 = DPB(r0, v3, 0)",
                "3|L--|Z = true",
                "4|L--|N = false",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_clrl()
        {
            BuildTest(0xD4, 0x53);	// clrl	r3
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|r3 = 0x00000000",
                "2|L--|Z = true",
                "3|L--|N = false",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_clrq()
        {
            BuildTest(0x7C, 0x81);	// clrq	(r1)+
            AssertCode(
                "0|L--|00010000(2): 7 instructions",
                "1|L--|v3 = 0x0000000000000000",
                "2|L--|Mem0[r1:word64] = v3",
                "3|L--|r1 = r1 + 0x00000008",
                "4|L--|Z = true",
                "5|L--|N = false",
                "6|L--|C = false",
                "7|L--|V = false");
        }

        [Test]
        public void VaxRw_clrw()
        {
            BuildTest(0xB4, 0xBD, 0xE8, 0xFE);	// clrw	-0118(fp)
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|v3 = 0x0000",
                "2|L--|Mem0[fp + -280:word16] = v3",
                "3|L--|Z = true",
                "4|L--|N = false",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpb()
        {
            BuildTest(0x91, 0xA7, 0x00, 0x2D);	// cmpb	+00(r7),#2D
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|CZN = cond(Mem0[r7 + 0:byte] - 0x2D)",
                "2|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpd()
        {
            BuildTest(0x71, 0x54, 0x01);	// cmpd	#0.75,#0.5625
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|CZN = cond(r5_r4 - 0.5625)",
                "2|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpf()
        {
            BuildTest(0x51, 0xB5, 0x51, 0x12, 0x50);    // cmpf	+1251(r5),r0
            AssertCode(
                   "0|L--|00010000(5): 2 instructions",
                   "1|L--|CZN = cond(Mem0[r5 + 4689:real32] - r0)",
                   "2|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpl()
        {
            BuildTest(0xD1, 0xAC, 0x04, 0x01);	// cmpl	+04(ap),#00000001
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|CZN = cond(Mem0[ap + 4:word32] - 0x00000001)",
                "2|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpw()
        {
            BuildTest(0xB1, 0xAE, 0x10, 0xBB, 0x3F, 0x03);	// cmpw	+10(sp),+033F(r11)
            AssertCode(
                "0|L--|00010000(6): 2 instructions",
                "1|L--|CZN = cond(Mem0[sp + 16:word16] - Mem0[r11 + 831:word16])",
                "2|L--|V = false");
        }

        [Test]
        public void VaxRw_cmpp3()
        {
            BuildTest(0x35, 0x53, 0x64, 0x65);	// cmpp3	
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|ZN = cond(vax_cmpp3((word16) r3, Mem0[r4:ptr32], Mem0[r5:ptr32]))",
                "2|L--|C = false",
                "3|L--|V = false");
        }


        [Test]
        public void VaxRw_cmpp4()
        {
            BuildTest(0x37, 0x53, 0x64, 0x55, 0x66);	// cmpp4	
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|ZN = cond(vax_cmpp4((word16) r3, Mem0[r4:ptr32], (word16) r5, Mem0[r6:ptr32]))",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void VaxRw_cvtbd()
        {
            BuildTest(0x6C, 0x51, 0x50);	// cvtbd	
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r1_r0 = (real64) (int8) r1",
                "2|L--|VZN = cond(r1_r0)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtbf()
        {
            BuildTest(0x4C, 0x66, 0x55);	// cvtbf	
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r5 = (real32) Mem0[r6:int8]",
                "2|L--|VZN = cond(r5)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtbl()
        {
            BuildTest(0x98, 0x61, 0x56);	// cvtbl	(r1),r6
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r6 = (int32) Mem0[r1:int8]",
                "2|L--|VZN = cond(r6)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtbw()
        {
            BuildTest(0x99, 0xB6, 0xE8, 0x00, 0x56);	// cvtbw	+00E8(r6),r6
            AssertCode(
                "0|L--|00010000(5): 4 instructions",
                "1|L--|v3 = (int16) Mem0[r6 + 232:int8]",
                "2|L--|r6 = DPB(r6, v3, 0)",
                "3|L--|VZN = cond(v3)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtdb()
        {
            BuildTest(0x68, 0x64, 0x68);	// cvtdb	(r4),(r8)
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (int8) Mem0[r4:real64]",
                "2|L--|Mem0[r8:int8] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtdf()
        {
            BuildTest(0x76, 0x65, 0x72);	// cvtdf	(r5),-(r2)
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|r2 = r2 - 0x00000004",
                "2|L--|v4 = (real32) Mem0[r5:real64]",
                "3|L--|Mem0[r2:real32] = v4",
                "4|L--|VZN = cond(v4)");
        }

        [Test]
        public void VaxRw_cvtdl()
        {
            BuildTest(0x6A, 0x54, 0x52);	// cvtdl	r4,r2
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r2 = (int32) r5_r4",
                "2|L--|VZN = cond(r2)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtdw()
        {
            BuildTest(0x69, 0x70, 0x52);	// cvtdw	-(r0),r2
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|r0 = r0 - 0x00000008",
                "2|L--|v4 = (int16) Mem0[r0:real64]",
                "3|L--|r2 = DPB(r2, v4, 0)",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtfb()
        {
            BuildTest(0x48, 0x64, 0x52);	// cvtfb	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (int8) Mem0[r4:real32]",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtfd()
        {
            BuildTest(0x56, 0x64, 0x52);	// cvtfd	
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r3_r2 = (real64) Mem0[r4:real32]",
                "2|L--|VZN = cond(r3_r2)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtfl()
        {
            BuildTest(0x4A, 0x64, 0x52);	// cvtfl	
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r2 = (int32) Mem0[r4:real32]",
                "2|L--|VZN = cond(r2)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtfw()
        {
            BuildTest(0x49, 0x64, 0x52);	// cvtfw	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (int16) Mem0[r4:real32]",
                "2|L--|r2 = DPB(r2, v4, 0)",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtlb()
        {
            BuildTest(0xF6, 0x50, 0x84);	// cvtlb	r0,(r4)+
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|v4 = (int8) r0",
                "2|L--|Mem0[r4:int8] = v4",
                "3|L--|r4 = r4 + 0x00000001",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtld()
        {
            BuildTest(0x6E, 0x50, 0x6E);	// cvtld	#00000000,(sp)
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (real64) r0",
                "2|L--|Mem0[sp:real64] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtlf()
        {
            BuildTest(0x4E, 0x52, 0x53);	// cvtlf	r2,r3
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|r3 = (real32) r2",
                "2|L--|VZN = cond(r3)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtlw()
        {
            BuildTest(0xF7, 0x52, 0xC6, 0x24, 0x03, 0x01, 0x00);	// cvtlw	r2,+00010324(r6)
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|v4 = (int16) r2",
                "2|L--|Mem0[r6 + 0x00010324:int16] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtrdl()
        {
            BuildTest(0x6B, 0x73, 0x62);	// cvtrdl	-(r3),(r2)
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|r3 = r3 - 0x00000008",
                "2|L--|v4 = (int32) round(Mem0[r3:real64])",
                "3|L--|Mem0[r2:int32] = v4",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtrfl()
        {
            BuildTest(0x4B, 0x73, 0x62);	// cvtrfl	
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|r3 = r3 - 0x00000004",
                "2|L--|v4 = (int32) round(Mem0[r3:real32])",
                "3|L--|Mem0[r2:int32] = v4",
                "4|L--|VZN = cond(v4)",
                "5|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtwb()
        {
            BuildTest(0x33, 0x63, 0x62);	// cvtwb	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (int8) Mem0[r3:int16]",
                "2|L--|Mem0[r2:int8] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtwd()
        {
            BuildTest(0x6D, 0x65, 0x6D);	// cvtwd	(r5),(fp)
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (real64) Mem0[r5:int16]",
                "2|L--|Mem0[fp:real64] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtwf()
        {
            BuildTest(0x4D, 0x65, 0x6D);	// cvtwf	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (real32) Mem0[r5:int16]",
                "2|L--|Mem0[fp:real32] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_cvtwl()
        {
            BuildTest(0x32, 0x65, 0x6D);	// cvtwl	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v4 = (int32) Mem0[r5:int16]",
                "2|L--|Mem0[fp:int32] = v4",
                "3|L--|VZN = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void VaxRw_decl()
        {
            BuildTest(0xD7, 0x58);	// decl	r8
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|r8 = r8 - 0x00000001",
                "2|L--|CVZN = cond(r8)");
        }

        [Test]
        public void VaxRw_decw()
        {
            BuildTest(0xB7, 0xAE, 0x46);	// decw	+46(sp)
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v3 = Mem0[sp + 70:word16] - 0x0001",
                "2|L--|Mem0[sp + 70:word16] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_divb3()
        {
            BuildTest(0x87, 0x53, 0x51, 0x90);	// divb3	r3,r1,@(R0)+
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v5 = (byte) r1 / (byte) r3",
                "2|L--|Mem0[Mem0[r0:word32]:byte] = v5",
                "3|L--|r0 = r0 + 0x00000004",
                "4|L--|CVZN = cond(v5)");
        }

        [Test]
        public void VaxRw_divd2()
        {
            BuildTest(0x66, 0x24, 0xCA, 0x00, 0xEA, 0x00, 0xEA);	// divd2	#0.5,-15FF1600(r10)
            AssertCode(
                "0|L--|00010000(7): 3 instructions",
                "1|L--|v3 = Mem0[r10 + 0xEA00EA00:real64] / 12.0",
                "2|L--|Mem0[r10 + 0xEA00EA00:real64] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_divd3()
        {
            BuildTest(0x67, 0x20, 0x5A, 0x65);	// divd3	#80,r10,(r5)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v6 = r11_r10 / 8.0",
                "2|L--|Mem0[r5:real64] = v6",
                "3|L--|CVZN = cond(v6)");
        }

        [Test]
        public void VaxRw_divf2()
        {
            BuildTest(0x46, 0x32, 0x57);	// divf2	
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|r7 = r7 / 40.0F",
                "2|L--|CVZN = cond(r7)");
        }

        [Test]
        public void VaxRw_divf3()
        {
            BuildTest(0x47, 0x32, 0x56, 0x68);	// divf3	
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v4 = r6 / 40.0F",
                "2|L--|Mem0[r8:real32] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_divl2()
        {
            BuildTest(0xC6, 0x04, 0x50);	// divl2	#00000004,r0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|r0 = r0 / 0x00000004",
                "2|L--|CVZN = cond(r0)");
        }

        [Test]
        public void VaxRw_divl3()
        {
            BuildTest(0xC7, 0x04, 0x50, 0xA2, 0x64);	// divl3	#00000004,r0,+64(r2)
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|v4 = r0 / 0x00000004",
                "2|L--|Mem0[r2 + 100:word32] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_divp()
        {
            BuildTest(0x27, 0x05, 0x3B, 0x00, 0x11, 0x4B, 0xB5, 0x50, 0x01, 0x17);	// divp	#0005,#3B,#0005,#11,+0150(r5)[r11],#17
            AssertCode(
                "0|L--|00010000(10): 2 instructions",
                "1|L--|VZN = vax_divp(0x0005, 0x3B, 0x0000, 0x11, Mem0[r5 + 336 + r11 * 2:word16], 0x17)",
                "2|L--|C = false");
        }

        [Test]
        public void VaxRw_divw2()
        {
            BuildTest(0xA6, 0x2D, 0xAB, 0x04);	// divw2	#002D,
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v3 = Mem0[r11 + 4:word16] / 0x002D",
                "2|L--|Mem0[r11 + 4:word16] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_divw3()
        {
            BuildTest(0xA7, 0x03, 0xAC, 0x00, 0xA3, 0x00);	// divw3	#0003,+00(ap),+00(ap)
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|v4 = Mem0[ap + 0:word16] / 0x0003",
                "2|L--|Mem0[r3 + 0:word16] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_incb()
        {
            BuildTest(0x96, 0x89);	// incb	(r9)+
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v3 = Mem0[r9:byte] + 0x01",
                "2|L--|Mem0[r9:byte] = v3",
                "3|L--|r9 = r9 + 0x00000001",
                "4|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_incl()
        {
            BuildTest(0xD6, 0x53);	// incl	r3
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|r3 = r3 + 0x00000001",
                "2|L--|CVZN = cond(r3)");
        }

        [Test]
        public void VaxRw_incw()
        {
            BuildTest(0xB6, 0xAE, 0x32);	// incw	+32(sp)
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v3 = Mem0[sp + 50:word16] + 0x0001",
                "2|L--|Mem0[sp + 50:word16] = v3",
                "3|L--|CVZN = cond(v3)");
        }


        [Test]
        public void VaxRw_jmp()
        {
            BuildTest(0x17, 0xFF, 0xF2, 0xFB, 0xFF, 0x3F);	// jmp	40008000
            AssertCode(
                "0|T--|00010000(6): 1 instructions",
                "1|T--|goto 4000FBF8");
        }

        [Test]
        public void VaxRw_jsb()
        {
            BuildTest(0x16, 0xFF, 0xBD, 0x12, 0x01, 0x00);	// jsb	000192C8
            AssertCode(
                "0|T--|00010000(6): 1 instructions",
                "1|T--|call 000212C3 (4)");
        }

        [Test]
        public void VaxRw_mcomb()
        {
            BuildTest(0x92, 0x61, 0x51);	// mcomb	
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|v3 = ~Mem0[r1:byte]",
                "2|L--|r1 = DPB(r1, v3, 0)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_mcoml()
        {
            BuildTest(0xD2, 0x52, 0x52);	// mcoml	r2,r2
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|r2 = ~r2",
                "2|L--|ZN = cond(r2)",
                "3|L--|C = false",
                "4|L--|V = false");
        }


        [Test]
        public void VaxRw_mcomw()
        {
            BuildTest(0xB2, 0xA6, 0xA0, 0xA0, 0x6);	// mcomw	-60(r6),+6(r0)
            AssertCode(
                "0|L--|00010000(5): 5 instructions",
                "1|L--|v4 = ~Mem0[r6 + -96:word16]",
                "2|L--|Mem0[r0 + 6:word16] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_mnegb()
        {
            BuildTest(0x8E, 0x51, 0xB6, 0xDE, 0x00);	// mnegb	r1,+00DE(r6)
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|v4 = -(byte) r1",
                "2|L--|Mem0[r6 + 222:byte] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_mnegd()
        {
            BuildTest(0x72, 0x20, 0x56);	// mnegd	#8,r6
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|r7_r6 = -8.0",
                "2|L--|ZN = cond(r7_r6)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_mnegf()
        {
            BuildTest(0x52, 0x3E, 0xC6, 0xCE, 0x00, 0x00, 0x00);	// mnegf	#112,+000000FE(r6)
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v3 = -112.0F",
                "2|L--|Mem0[r6 + 0x000000CE:real32] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_mnegl()
        {
            BuildTest(0xCE, 0x71, 0xAC, 0x04);	// mnegl	-(r1),+04(ap)
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|r1 = r1 - 0x00000004",
                "2|L--|v4 = -Mem0[r1:word32]",
                "3|L--|Mem0[ap + 4:word32] = v4",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_mnegw()
        {
            BuildTest(0xAE, 0xAC, 0x5E, 0x8E);	// mnegw	+5E(ap),(sp)+
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v4 = -Mem0[ap + 94:word16]",
                "2|L--|Mem0[sp:word16] = v4",
                "3|L--|sp = sp + 0x00000002",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_movb()
        {
            BuildTest(0x90, 0x01, 0x50);	// movb	#01,r0
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|v3 = 0x01",
                "2|L--|r0 = DPB(r0, v3, 0)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_movd()
        {
            BuildTest(0x70, 0x04, 0xB4, 0x04, 0x03);	// movd	#0.75,+31049804(r4)
            AssertCode(
                "0|L--|00010000(5): 5 instructions",
                "1|L--|v3 = 0.75",
                "2|L--|Mem0[r4 + 772:real64] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_movf()
        {
            BuildTest(0x50, 0x8F, 0x43, 0x00, 0x00, 0x00, 0x57);	// movf	#4.76441477870438E-44,r7
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r7 = 4.764415e-44F",
                "2|L--|ZN = cond(r7)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_movl()
        {
            BuildTest(0xD0, 0x01, 0x50);	// movl	#00000001,r0
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|r0 = 0x00000001",
                "2|L--|ZN = cond(r0)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

 

        [Test]
        public void VaxRw_movw()
        {
            BuildTest(0xB0, 0x8F, 0x00, 0x02, 0xA2, 0x36);	// movw	#0200,+36(r2)
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|v3 = 0x0200",
                "2|L--|Mem0[r2 + 54:word16] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_movzbl()
        {
            BuildTest(0x9A, 0x8F, 0x5D, 0x7E);	// movzbl	#5D,-(sp)
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v3 = (uint32) 0x5D",
                "3|L--|Mem0[sp:uint32] = v3",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_movzbw()
        {
            BuildTest(0x9B, 0x8F, 0x80, 0xC6, 0x22, 0x02, 0x01, 0x00);	// movzbw	#80,+00010222(r6)
            AssertCode(
                "0|L--|00010000(8): 5 instructions",
                "1|L--|v3 = (uint16) 0x80",
                "2|L--|Mem0[r6 + 0x00010222:uint16] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_movzwl()
        {
            BuildTest(0x3C, 0x8F, 0x01, 0x04, 0x7E);	// movzwl	#0401,-(sp)
            AssertCode(
                "0|L--|00010000(5): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v3 = (uint32) 0x0401",
                "3|L--|Mem0[sp:uint32] = v3",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }


        [Test]
        public void VaxRw_mulb2()
        {
            BuildTest(0x84, 0x05, 0xA8, 0x00);	// mulb2	#05,+00(r8)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v3 = Mem0[r8 + 0:byte] * 0x05",
                "2|L--|Mem0[r8 + 0:byte] = v3",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_mulb3()
        {
            BuildTest(0x85, 0xCE, 0xFF, 0x5A, 0xD4, 0x5B, 0xB4, 0x53, 0xD4, 0x6E);	// mulb3	+5BD45AFF(sp),-2BAD(r4),(sp)
            AssertCode(
                "0|L--|00010000(10): 3 instructions",
                "1|L--|v4 = Mem0[r4 + -11181:byte] * Mem0[sp + 0x5BD45AFF:byte]",
                "2|L--|Mem0[sp:byte] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_muld2()
        {
            BuildTest(0x64, 0x01, 0x51);	// muld2	#0.5625,r1
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|r2_r1 = r2_r1 * 0.5625",
                "2|L--|CVZN = cond(r2_r1)");
        }

        [Test]
        public void VaxRw_muld3()
        {
            BuildTest(0x65, 0x00, 0x52, 0x53);	// muld3	#0.5,#0.5,r3
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|r4_r3 = r3_r2 * 0.5",
                "2|L--|CVZN = cond(r4_r3)");
        }

        [Test]
        public void VaxRw_mulf2()
        {
            BuildTest(0x44, 0x09, 0x56);	// mulf2	
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|r6 = r6 * 1.125F",
                "2|L--|CVZN = cond(r6)");
        }

        [Test]
        public void VaxRw_mulf3()
        {
            BuildTest(0x45, 0x24, 0x66, 0x52);	// mulf3	
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|r2 = Mem0[r6:real32] * 12.0F",
                "2|L--|CVZN = cond(r2)");
        }

        [Test]
        public void VaxRw_mull2()
        {
            BuildTest(0xC4, 0xB8, 0x03, 0xFB, 0x52);	// mull2	-04FD(r8),r2
            AssertCode(
                "0|L--|00010000(5): 2 instructions",
                "1|L--|r2 = r2 * Mem0[r8 + -1277:word32]",
                "2|L--|CVZN = cond(r2)");
        }

        [Test]
        public void VaxRw_mull3()
        {
            BuildTest(0xC5, 0x8F, 0x6D, 0x01, 0x00, 0x00, 0x53, 0x52);	// mull3	#0000016D,r3,r2
            AssertCode(
                "0|L--|00010000(8): 2 instructions",
                "1|L--|r2 = r3 * 0x0000016D",
                "2|L--|CVZN = cond(r2)");
        }

        [Test]
        public void VaxRw_mulp()
        {
            BuildTest(0x25, 0x64, 0x25, 0x64, 0x25, 0x73, 0x20);	// mulp	(r4),#25,(r4),#25,-(r3),#20
            AssertCode(
                "0|L--|00010000(7): 3 instructions",
                "1|L--|r3 = r3 - 0x00000002",
                "2|L--|VZN = vax_mulp(Mem0[r4:word16], 0x25, Mem0[r4:word16], 0x25, Mem0[r3:word16], 0x20)",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_mulw2()
        {
            BuildTest(0xA4, 0x08, 0x50);	// mulw2	#0008,#0000
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v3 = (word16) r0 * 0x0008",
                "2|L--|r0 = DPB(r0, v3, 0)",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_mulw3()
        {
            BuildTest(0xA5, 0x51, 0x62, 0x53);	// mulw3	#0001,#0000,#0000
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v5 = Mem0[r2:word16] * (word16) r1",
                "2|L--|r3 = DPB(r3, v5, 0)",
                "3|L--|CVZN = cond(v5)");
        }

        [Test]
        public void VaxRw_nop()
        {
            BuildTest(0x01);	// nop	
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void VaxRw_polyd()
        {
            BuildTest(0x75, 0x69, 0x74, 0x65);	// polyd	(r9),-(r4),(r5)
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|r4 = r4 - 0x00000002",
                "2|L--|r1_r0 = vax_poly(Mem0[r9:real64], Mem0[r4:word16], Mem0[r5:ptr32])",
                "3|L--|ZN = cond(r1_r0)",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void VaxRw_polyf()
        {
            BuildTest(0x55, 0x69, 0x74, 0x65);	// polyf	
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|r4 = r4 - 0x00000002",
                "2|L--|r0 = vax_poly(Mem0[r9:real32], Mem0[r4:word16], Mem0[r5:ptr32])",
                "3|L--|ZN = cond(r0)",
                "4|L--|V = false",
                "5|L--|C = false");
        }

  
        [Test]
        public void VaxRw_pushab()
        {
            BuildTest(0x9F, 0xB2, 0xEB, 0x05);	// pushab	+05EB(r2)
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v4 = r2 + 1515",
                "3|L--|Mem0[sp:word32] = v4", 
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_pushal()
        {
            BuildTest(0xDF, 0xAC, 0x08);	// pushal	+08(ap)
            AssertCode(
                "0|L--|00010000(3): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v4 = ap + 8",
                "3|L--|Mem0[sp:word32] = v4",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_pushaq()
        {
            BuildTest(0x7F, 0xA8, 0x50);	// pushaq	+50DD0F50(r8)
            AssertCode(
                "0|L--|00010000(3): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v4 = r8 + 80",
                "3|L--|Mem0[sp:word32] = v4",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_pushaw()
        {
            BuildTest(0x3F, 0x67);	// pushaw	
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|Mem0[sp:word32] = r7",
                "3|L--|ZN = cond(r7)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_pushl()
        {
            BuildTest(0xDD, 0xAC, 0x08);	// pushl	+08(ap)
            AssertCode(
                "0|L--|00010000(3): 6 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|v4 = Mem0[ap + 8:word32]",
                "3|L--|Mem0[sp:word32] = v4",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_ret()
        {
            BuildTest(0x04);	// ret	
            AssertCode(
                "0|T--|00010000(1): 4 instructions",
                "1|L--|sp = fp - 0x00000004",
                "2|L--|fp = Mem0[sp + 0x00000010:word32]",
                "3|L--|ap = Mem0[sp + 0x0000000C:word32]",
                "4|T--|return (4,0)");
        }

        [Test]
        public void VaxRw_rotl()
        {
            BuildTest(0x9C, 0x03, 0x52, 0x64);	// rotl	
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v4 = __rol(r2, 0x03)",
                "2|L--|Mem0[r4:word32] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_rsb()
        {
            BuildTest(0x05);	// rsb	
            AssertCode(
                "0|T--|00010000(1): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void VaxRw_sbwc()
        {
            BuildTest(0xD9, 0x53, 0x74);	// sbwc	
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|r4 = r4 - 0x00000004",
                "2|L--|v4 = Mem0[r4:word32] - r3 - C",
                "3|L--|Mem0[r4:word32] = v4",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_sobgeq()
        {
            BuildTest(0xF4, 0x53, 0x00);	// sobgeq	r3,00019277
            AssertCode(
                "0|T--|00010000(3): 3 instructions",
                "1|L--|r3 = r3 - 0x00000001",
                "2|L--|CVZN = cond(r3)",
                "3|T--|if (r3 >= 0x00000000) branch 00010003");
        }

        [Test]
        public void VaxRw_sobgtr()
        {
            BuildTest(0xF5, 0x50, 0xD9);	// sobgtr	#00000000,000127B5
            AssertCode(
                "0|T--|00010000(3): 3 instructions",
                "1|L--|r0 = r0 - 0x00000001",
                "2|L--|CVZN = cond(r0)",
                "3|T--|if (r0 > 0x00000000) branch 0000FFDC");
        }
        [Test]
        public void VaxRw_subb2()
        {
            BuildTest(0x82, 0x01, 0x53);	// subb2	#01,r3
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v3 = (byte) r3 - 0x01",
                "2|L--|r3 = DPB(r3, v3, 0)",
                "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_subb3()
        {
            BuildTest(0x83, 0x04, 0xA3, 0x00, 0xB3, 0x00, 0xE3);	// subb3	#04,+00(r3),-1D00(r3)
            AssertCode(
               "0|L--|00010000(7): 3 instructions",
               "1|L--|v3 = Mem0[r3 + 0:byte] - 0x04",
               "2|L--|Mem0[r3 + -7424:byte] = v3",
               "3|L--|CVZN = cond(v3)");
        }

        [Test]
        public void VaxRw_subd2()
        {
            BuildTest(0x62, 0xB0, 0x02, 0x53, 0xB0, 0x02, 0x52);	// subd2	+5302(r0),+5202(r0)
            AssertCode(
                "0|L--|00010000(7): 5 instructions",
                "1|L--|v3 = Mem0[r0 + 20994:real64] - Mem0[r0 + 21250:real64]",
                "2|L--|Mem0[r0 + 20994:real64] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_subd3()
        {
            BuildTest(0x63, 0x61, 0x72, 0x64);	// subd3	(r1),-(r2),(r4)
            AssertCode(
                "0|L--|00010000(4): 6 instructions",
                "1|L--|r2 = r2 - 0x00000008",
                "2|L--|v5 = Mem0[r2:real64] - Mem0[r1:real64]",
                "3|L--|Mem0[r4:real64] = v5",
                "4|L--|ZN = cond(v5)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_subf2()
        {
            BuildTest(0x42, 0x83, 0x84);	// subf2	
            AssertCode(
                "0|L--|00010000(3): 8 instructions",
                "1|L--|v3 = r3",
                "2|L--|r3 = r3 + 0x00000004",
                "3|L--|v5 = Mem0[r4:real32] - Mem0[v3:real32]",
                "4|L--|Mem0[r4:real32] = v5",
                "5|L--|r4 = r4 + 0x00000004",
                "6|L--|ZN = cond(v5)",
                "7|L--|C = false",
                "8|L--|V = false");
        }

        [Test]
        public void VaxRw_subf3()
        {
            BuildTest(0x43, 0x83, 0x84, 0x52);	// subf3	
            AssertCode(
                "0|L--|00010000(4): 8 instructions",
                "1|L--|v3 = r3",
                "2|L--|r3 = r3 + 0x00000004",
                "3|L--|v5 = r4",
                "4|L--|r4 = r4 + 0x00000004",
                "5|L--|r2 = Mem0[v5:real32] - Mem0[v3:real32]",
                "6|L--|ZN = cond(r2)",
                "7|L--|C = false",
                "8|L--|V = false");
        }

        [Test]
        public void VaxRw_subl2()
        {
            BuildTest(0xC2, 0x04, 0x5E);	// subl2	#00000004,sp
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|sp = sp - 0x00000004",
                "2|L--|CVZN = cond(sp)");
        }

        [Test]
        public void VaxRw_subl3()
        {
            BuildTest(0xC3, 0x04, 0xAC, 0x08, 0x54);	// subl3	#00000004,+08(ap),r4
            AssertCode(
                "0|L--|00010000(5): 2 instructions",
                "1|L--|r4 = Mem0[ap + 8:word32] - 0x00000004",
                "2|L--|CVZN = cond(r4)");
        }

        [Test]
        public void VaxRw_subp4()
        {
            BuildTest(0x22, 0x01, 0x63, 0x01, 0x62);	// subp4	#0001,(r3),#0001,(r2)
            AssertCode(
                "0|L--|00010000(5): 2 instructions",
                "1|L--|VZN = vax_subp4(0x0001, Mem0[r3:ptr32], 0x0001, Mem0[r2:ptr32])",
                "2|L--|C = false");
        }

        [Test]
        public void VaxRw_subp6()
        {
            BuildTest(0x23, 0x3D, 0x51, 0xD7, 0x70, 0x3D, 0xD7, 0xA3, 0x7C);	// subp6	#003D,r1,+3D70(r7),+7CA3(r7),#0000,(sp)+
            AssertCode(
                "0|L--|00010000(9): 3 instructions",
                "1|L--|ap = ap - 0x00000004",
                "2|L--|VZN = vax_subp6(0x003D, r1, Mem0[Mem0[r7 + 112:word32]:word16], 0x3D, Mem0[Mem0[r7 + -93:word32]:word16], Mem0[ap:ptr32])",
                "3|L--|C = false");
        }

        [Test]
        public void VaxRw_subw2()
        {
            BuildTest(0xA2, 0x56, 0xA4, 0x62);	// subw2	r6,+62(r4)
            AssertCode(
                "0|L--|00010000(4): 3 instructions",
                "1|L--|v4 = Mem0[r4 + 98:word16] - (word16) r6",
                "2|L--|Mem0[r4 + 98:word16] = v4",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void VaxRw_subw3()
        {
            BuildTest(0xA3, 0x86, 0x81, 0x57);	// subw3	(r6)+,(r1)+,r7
            AssertCode(
                "0|L--|00010000(4): 7 instructions",
                "1|L--|v3 = r6",
                "2|L--|r6 = r6 + 0x00000002",
                "3|L--|v5 = r1",
                "4|L--|r1 = r1 + 0x00000002",
                "5|L--|v7 = Mem0[v5:word16] - Mem0[v3:word16]",
                "6|L--|r7 = DPB(r7, v7, 0)",
                "7|L--|CVZN = cond(v7)");
        }

        [Test]
        public void VaxRw_tstb()
        {
            BuildTest(0x95, 0xA4, 0x02);	// tstb	+02(r4)
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|ZN = cond(Mem0[r4 + 2:byte] - 0x00)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void VaxRw_tstd()
        {
            BuildTest(0x73, 0x50);	// tstd	r0
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|ZN = cond(r1_r0 - 0.0)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void VaxRw_tstf()
        {
            BuildTest(0x53, 0xA0, 0x63);	// tstf	+63(r0)
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|ZN = cond(Mem0[r0 + 99:real32] - 0.0F)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void VaxRw_tstl()
        {
            BuildTest(0xD5, 0x50);	// tstl	r0
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|ZN = cond(r0 - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }


        [Test]
        public void VaxRw_tstw()
        {
            BuildTest(0xB5, 0x51);	// tstw	r1
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|ZN = cond((word16) r1 - 0x0000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

     
     

        [Test]
        public void VaxRw_movab()
        {
            BuildTest(0x9E, 0xC0, 0xC9, 0xD3, 0xFD, 0xFF, 0x57);	// movab	FFFE5400,r7
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r7 = r0 + 0xFFFDD3C9");
        }

        [Test]
        public void VaxRw_moval()
        {
            BuildTest(0xDE, 0xC0, 0x81, 0x5B, 0x00, 0x00, 0x54);	// moval	0000DBD4,r4
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r4 = r0 + 0x00005B81",
                "2|L--|ZN = cond(r4)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void VaxRw_movaq()
        {
            BuildTest(0x7E, 0x60, 0x85);	// movaq	#00000000,(r5)+
            AssertCode(
                "0|L--|00010000(3): 6 instructions",
                "1|L--|v4 = r0",
                "2|L--|Mem0[r5:word32] = v4",
                "3|L--|r5 = r5 + 0x00000004");
        }

        [Test]
        public void VaxRw_movaw()
        {
            BuildTest(0x3E, 0x42, 0x63, 0x51);	// movaw	
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|r1 = r3 + r2 * 2",
                "2|L--|ZN = cond(r1)",
                "3|L--|C = false",
                "4|L--|V = false");
        }



        [Test]
        public void VaxRw_xorb2()
        {
            BuildTest(0x8C, 0x02, 0x80);	// xorb2	#02,(r0)+
            AssertCode(
                "0|L--|00010000(3): 6 instructions",
                "1|L--|v3 = Mem0[r0:byte] ^ 0x02",
                "2|L--|Mem0[r0:byte] = v3",
                "3|L--|r0 = r0 + 0x00000001",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void VaxRw_xorb3()
        {
            BuildTest(0x8D, 0x5C, 0x52, 0x63);	// xorb3	ap,r2,(r3)
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v5 = (byte) r2 ^ (byte) ap",
                "2|L--|Mem0[r3:byte] = v5",
                "3|L--|ZN = cond(v5)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_xorl2()
        {
            BuildTest(0xCC, 0x8F, 0xFF, 0xFF, 0xFF, 0xFF, 0x53);	// xorl2	#FFFFFFFF,r3
            AssertCode(
                "0|L--|00010000(7): 4 instructions",
                "1|L--|r3 = r3 ^ 0xFFFFFFFF",
                "2|L--|ZN = cond(r3)",
                "3|L--|C = false",
                "4|L--|V = false");
        }


        [Test]
        public void VaxRw_xorl3()
        {
            BuildTest(0xCD, 0xCD, 0xFF, 0x58, 0xD0, 0xEA, 0x27, 0xB6, 0x00, 0x00);	// xorl3	-152FA701(fp),#00000027,+0000(r6)
            AssertCode(
                "0|L--|00010000(10): 5 instructions",
                "1|L--|v4 = 0x00000027 ^ Mem0[fp + 0xEAD058FF:word32]",
                "2|L--|Mem0[r6 + 0:word32] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_xorw2()
        {
            BuildTest(0xAC, 0x02, 0xA4, 0x03);	// xorw2	#0002,+03(r4)
            AssertCode(
                "0|L--|00010000(4): 5 instructions",
                "1|L--|v3 = Mem0[r4 + 3:word16] ^ 0x0002",
                "2|L--|Mem0[r4 + 3:word16] = v3",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void VaxRw_xorw3()
        {
            BuildTest(0xAD, 0xAC, 0xD0, 0xCC, 0x13, 0xC6, 0x00, 0x00, 0xAD, 0xD8);	// xorw3	-30(ap),+0000C613(ap),-28(fp)
            AssertCode(
                "0|L--|00010000(10): 5 instructions",
                "1|L--|v4 = Mem0[ap + 0x0000C613:word16] ^ Mem0[ap + -48:word16]",
                "2|L--|Mem0[fp + -40:word16] = v4",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        // These instructions are "extra credit" as most VAX user mode programs won't have them.

        [Test]
        public void VaxRw_adawi()
        {
            BuildTest(0x58, 0x52, 0x64);	// adawi	
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v4 = atomic_fetch_add(Mem0[r4:word16], (word16) r2)");
        }

        [Ignore("")]
        public void VaxRw_bbsc()
        {
            BuildTest(0xE4);	// bbsc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void VaxRw_bbss()
        {
            BuildTest(0xE2, 0xFC, 0x01, 0x01, 0x00, 0x53, 0xE8, 0x53, 0x0F, 0xDD, 0x53, 0xDF);	// bbss	+53000101(ap),+53DD0F53(r8),0000B1AC
            AssertCode(
                "0|T--|00010000(10): 3 instructions",
                "1|L--|v4 = Mem0[Mem0[r8 + 3923:word32]:word32] & 1 << Mem0[Mem0[ap + 0x53000101:word32]:word32]",
                "2|L--|Mem0[Mem0[r8 + 3923:word32]:word32] = Mem0[Mem0[r8 + 3923:word32]:word32] | 1 << Mem0[Mem0[ap + 0x53000101:word32]:word32]",
                "3|T--|if (v4 != 0x00000000) branch 0000FFE7");
        }

        [Test]
        public void VaxRw_bbssi()
        {
            BuildTest(0xE6, 0x52, 0x53, 0x52);	// bbssi	
            AssertCode(
                "0|T--|00010000(4): 5 instructions",
                "1|L--|__set_interlock()",
                "2|L--|v4 = r3 & 1 << r2",
                "3|L--|r3 = r3 | 1 << r2",
                "4|L--|__release_interlock()",
                "5|T--|if (v4 != 0x00000000) branch 00010056");
        }

        [Test]
        public void VaxRw_bicpsw()
        {
            BuildTest(0xB9, 0x5D);	// bicpsw	fp
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|psw = psw & ~((uint16) fp)");
        }

        [Test]
        public void VaxRw_bispsw()
        {
            BuildTest(0xB8, 0xE1, 0xFE, 0x7F, 0xD1, 0x52);	// bispsw	+52D17FFE(r1)
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|psw = psw | Mem0[Mem0[r1 + 32766:word32]:uint16]");
        }

        [Test]
        public void VaxRw_ffc()
        {
            BuildTest(0xEB, 0x52, 0x53, 0x54, 0x55);	// ffc	
            AssertCode(
                "0|L--|00010000(5): 2 instructions",
                "1|L--|Z = __ffc(r4, (byte) r3, r2, out r5)",
                "2|L--|CVN = 0x00");
        }

        [Test]
        public void VaxRw_insque()
        {
            BuildTest(0x0E, 0xD0, 0x32, 0x50, 0x54);	// insque	+5032(r0),r5
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|CZN = __insque(r0, Mem0[Mem0[r0 + 50:word32]:word32])",
                "2|L--|V = false");
        }

        [Test]
        public void VaxRw_scanc()
        {
            BuildTest(0x2A, 0x05, 0x01, 0x00, 0x52);	// scanc	#0005,#01,#00,r2
            AssertCode(
                "0|L--|00010000(5): 4 instructions",
                "1|L--|r3 = 0x00",
                "2|L--|Z = __scanc(0x0005, 0x01, 0x00, (byte) r2, out r0, out r1)",
                "3|L--|r2 = 0x00000000",
                "4|L--|CVN = 0x00");
        }

        [Test]
        [Ignore("Horrific VAX CISC instructions")]
        public void VaxRw_caseb()
        {
            BuildTest(0x8F, 0x01, 0x00, 0x50);	// caseb	#01,#00,r0
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|***");
        }

        [Test]
        [Ignore("Horrific VAX CISC instructions")]
        public void VaxRw_casel()
        {
            BuildTest(0xCF, 0x01, 0x00, 0x50);	// casel	
            AssertCode(
                "0|L--|00010000(5): 4 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void VaxRw_chme()
        {
            BuildTest(0xBD, 0x2);	// chme	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|vax_chme(0x0002)");
        }

        [Test]
        public void VaxRw_bitb()
        {
            BuildTest(0x93, 0x02, 0x51);	// bitb #02,r1
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|v3 = (byte) r1 & 0x02",
                "2|L--|ZN = cond(v3)",
                "3|L--|V = false");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_cmpc3()
        {
            BuildTest(0x29, 0x20, 51, 0x5B);	// cmpc3	#20,r1,r11
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }
#if LATER
        [Test]
        public void VaxRw_cmpv()
        {
            BuildTest(0xEC);	// cmpv	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_crc()
        {
            BuildTest(0x0B, 0xD1, 0x52, 0x50, 0x1A, 0x0B, 0xD6, 0x51, 0x11);	// crc	+5052(r1),#0000001A,#000B,+1151(r6)
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }




        [Test]
        [Ignore("")]
        public void VaxRw_cmpc5()
        {
            BuildTest(0x2D);	// cmpc5	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_cvtpl()
        {
            BuildTest(0x36);	// cvtpl	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_cvtps()
        {
            BuildTest(0x08, 0xE2, 0xFE, 0x7F, 0xE8, 0x50, 0x2B, 0xDD, 0x01, 0xDD, 0xEC, 0x13, 0xC6, 0x00, 0x00);	// cvtps	+50E87FFE(r2),#2B,-22FF(fp),+0000C613(ap)
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_cvtpt()
        {
            BuildTest(0x24, 0x00, 0x00, 0x00, 0x22);	// cvtpt	#0000,#00,#00,#0022
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_cvtsp()
        {
            BuildTest(0x09, 0x07, 0x00, 0x00, 0x5A);	// cvtsp	#0007,#00,#0000,r10
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_editpc()
        {
            BuildTest(0x38);	// editpc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_ediv()
        {
            BuildTest(0x7B);	// ediv	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_emul()
        {
            BuildTest(0x7A);	// emul	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_emodd()
        {
            BuildTest(0x74, 0x04, 0xE4, 0x04, 0xBC, 0x04, 0xE4, 0x04, 0xE4, 0x04, 0xE4, 0x04, 0xE4, 0x04);	// emodd	#0.75,-1BFB43FC(r4),#0.75,-1BFB1BFC(r4),#0.75
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_emodf()
        {
            BuildTest(0x54);	// emodf	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_extzv()
        {
            BuildTest(0xEF, 0xA5, 0x30, 0xFF, 0xFF, 0x52, 0x9E, 0xEF, 0xB6, 0xE6, 0xFD, 0xFF, 0x55, 0x7C, 0x59);	// extzv	+30(r5),EF9ED62D,-1A(r6),+597C55FF(fp)
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_index()
        {
            BuildTest(0x0A, 0x01, 0x00, 0xD4, 0x50, 0x04, 0x9A, 0xE6, 0x18, 0x0A, 0x01, 0x00, 0x52);	// index	#00000001,#00000000,+0450(r4),(r10)+,+00010A18(r6),r2
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_insqhi()
        {
            BuildTest(0x5C);	// insqhi	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_insqti()
        {
            BuildTest(0x5D);	// insqti	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_insv()
        {
            BuildTest(0xF0);	// insv	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_ldpctx()
        {
            BuildTest(0x06);	// ldpctx	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_locc()
        {
            BuildTest(0x3A);	// locc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_matchc()
        {
            BuildTest(0x39, 0x51, 0x62, 0x53, 0x64);	// matchc	
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|Z = vax_matchs(r1, Mem0[r2:byte], r3, Mem0[r4:byte], out r1, out r2, out r3, out r4)");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movc3()
        {
            BuildTest(0x28, 0x02, 0xE4, 0x04, 0x74, 0x02, 0x88, 0x02);	// movc3	#0002,-77FD8BFC(r4),#02
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movc5()
        {
            BuildTest(0x2C);	// movc5	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movp()
        {
            BuildTest(0x34);	// movp	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movpsl()
        {
            BuildTest(0xDC);	// movpsl	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movtc()
        {
            BuildTest(0x2E);	// movtc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_movtuc()
        {
            BuildTest(0x2F);	// movtuc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


    


        [Test]
        [Ignore("")]
        public void VaxRw_mtpr()
        {
            BuildTest(0xDA);	// mtpr	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_mfpr()
        {
            BuildTest(0xDB);	// mfpr	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_popr()
        {
            BuildTest(0xBA);	// popr	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_prober()
        {
            BuildTest(0x0C, 0x00, 0xC2, 0x04, 0x5E, 0x9E);	// prober	#00,+5E04(r2),(sp)+
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_probew()
        {
            BuildTest(0x0D, 0x00, 0x00, 0x00);	// probew	#00,#0000,#00
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_rei()
        {
            BuildTest(0x02);	// rei	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_remqhi()
        {
            BuildTest(0x5E);	// remqhi	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_remqti()
        {
            BuildTest(0x5F);	// remqti	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_remque()
        {
            BuildTest(0x0F, 0xC2, 0x04, 0x5E, 0x9E);	// remque	+5E04(r2),(sp)+
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore("")]
        public void VaxRw_spanc()
        {
            BuildTest(0x2B, 0x00, 0x2C, 0x00, 0x2D);	// spanc	#0000,#2C,#00,#2D
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_skpc()
        {
            BuildTest(0x3B);	// skpc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_svpctx()
        {
            BuildTest(0x07);	// svpctx	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("")]
        public void VaxRw_xfc()
        {
            BuildTest(0xFC);	// xfc	
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }
#endif
    }
}
