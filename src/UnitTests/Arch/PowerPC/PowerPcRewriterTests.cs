#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcRewriterTests : RewriterTestBase
    {
        private InstructionBuilder b;
        private PowerPcArchitecture arch = new PowerPcArchitecture(PrimitiveType.Word32);
        private IEnumerable<PowerPcInstruction> ppcInstrs;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return new Address(0x00100000); } }

        private void RunTest(Action<InstructionBuilder> m)
        {
            b = new InstructionBuilder(new Address(0x01000000));
            m(b);
            ppcInstrs = b.Instructions;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame)
        {
            return new PowerPcRewriter(arch, ppcInstrs, frame);
        }

        protected override LoadedImage RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) (w >> 24),
                (byte) (w >> 16),
                (byte) (w >> 8),
                (byte) w
            }).ToArray();
            var image = new LoadedImage(LoadAddress, bytes);
            ppcInstrs = new PowerPcDisassembler(arch, image.CreateBeReader(LoadAddress), PrimitiveType.Word32);
            return image;
        }

        [Test]
        public void PPCRW_Oris()
        {
            RunTest((m) =>
            {
                m.Oris(m.r4, m.r0, 0x1234);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|L--|r4 = r0 | 0x12340000");
        }

        [Test]
        public void PPCRW_Add()
        {
            RunTest((m) =>
            {
                m.Add(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|L--|r4 = r1 + r3");
        }

        [Test]
        public void PPCRW_Add_()
        {
            RunTest((m) =>
            {
                m.Add_(m.r4, m.r1, m.r3);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|L--|r4 = r1 + r3",
                "2|L--|SCZO = cond(r4)");
        }

        [Test]
        public void PPCRW_lwzu()
        {
            RunTest((m) =>
            {
                m.Lwzu(m.r2, 4, m.r1); 
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|L--|r2 = Mem0[r1 + 4:word32]",
                "2|L--|r1 = r1 + 4"
                );
        }

        [Test]
        public void PPCRW_lwz_r0()
        {
            RunTest((m) =>
            {
                m.Lwz(m.r2, -4, m.r0);
            });
            AssertCode(
                "0|01000000(4): 1 instructions",
                "1|L--|r2 = Mem0[0xFFFFFFFC:word32]"
                );
        }
        [Test]
        public void PPCRW_stbu()
        {
            RunTest((m) =>
            {
                m.Stbu(m.r2, 18, m.r3);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|L--|Mem0[r3 + 18:byte] = (byte) r2",
                "2|L--|r3 = r3 + 18"
                );
        }

        [Test]
        public void PPCRW_stbux()
        {
            RunTest((m) =>
            {
                m.Stbux(m.r2, m.r3, m.r0);
            });
            AssertCode(
                "0|01000000(4): 2 instructions",
                "1|L--|Mem0[r3 + r0:byte] = (byte) r2",
                "2|L--|r3 = r3 + r0"
                );
        }

        [Test]
        public void PPCRW_mflr()
        {
            Rewrite(0x7C0802A6);
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = lr");
        }

        [Test]
        public void PPCRw_mfcr()
        {
            AssertCode(0x7d800026,
                "0|00100000(4): 1 instructions",
                "1|L--|r12 = cr");
        }

        private void AssertCode(uint instr, params string [] sExp)
        {
            Rewrite(instr);
            AssertCode(sExp);
        }

        [Test]
        public void PPCRw_rlwinm()
        {
            AssertCode(0x57200036, // "rlwinm\tr0,r25,04,00,1B");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r25 & 0xFFFFFFF0");
        }

        [Test]
        public void PPCRw_lwzx()
        {
            AssertCode(0x7c9c002e, // "lwzx\tr4,r28,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r28 + r0:word32]");
        }

        [Test]
        public void PPCRw_stwx()
        {
            AssertCode(0x7c95012e, // "stwx\tr4,r21,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r0:word32] = r4");
        }

        [Test]
        public void PPCRw_subf()
        {
            AssertCode(0x7c154850, // "subf\tr0,r21,r9");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r9 - r21");
        }

        [Test]
        public void PPCRw_srawi()
        {
            AssertCode(0x7c002670, //"srawi\tr0,r0,04");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r0 >> 0x00000004");
        }

        [Test]
        public void PPCRw_bctr()
        {
            AssertCode(0x4e800420, //"bcctr\t14,00");
                "0|00100000(4): 1 instructions",
                "1|T--|goto ctr");
        }

        [Test]
        public void PPCRw_stwu()
        {
            AssertCode(0x9521016e, // "stwu\tr9,r1,r0");
                "0|00100000(4): 2 instructions",
                "1|L--|r9 = Mem0[r1 + 366:word32]",
                "2|L--|r1 = r1 + 366");
        }

        [Test]
        public void PPCRw_stwux()
        {
            AssertCode(0x7d21016e, // "stwux\tr9,r1,r0");
                "0|00100000(4): 2 instructions",
                "1|L--|Mem0[r1 + r0:word32] = r9",
                "2|L--|r1 = r1 + r0");
        }

        [Test]
        public void PPCRw_fmr()
        {
            AssertCode(0xFFE00890, // "fmr\tf31,f1");
                "0|00100000(4): 1 instructions",
                "1|L--|f31 = f1");
        }

        [Test]
        public void PPCRw_mtctr()
        {
            AssertCode(0x7d0903a6, // "mtctr\tr8");
                "0|00100000(4): 1 instructions",
                "1|L--|ctr = r8");
        }

        [Test]
        public void PPCRw_cmpl()
        {
            AssertCode(0x7f904840, "cmplw\tcr7,r16,r9");
        }

        [Test]
        public void PPCRw_neg()
        {
            AssertCode(0x7c0000d0, // "neg\tr0,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = -r0");
        }

        [Test]
        public void PPCRw_cntlzw()
        {
            AssertCode(0x7d4a0034, "cntlzw\tr10,r10");
        }

        [Test]
        public void PPCRw_fsub()
        {
            AssertCode(0xfc21f828, // "fsub\tf1,f1,f31");
                "0|00100000(4): 1 instructions",
                "1|L--|f1 = f1 - f31");
        }

        [Test]
        public void PPCRw_li()
        {
            AssertCode(0x38000000, 
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = 0");
        }

        [Test]
        public void PPCRw_addze()
        {
            AssertCode(0x7c000194, "addze\tr0,r0");
        }

        [Test]
        public void PPCRw_slw()
        {
            AssertCode(0x7d400030, //"slw\tr0,r10,r0");
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = r10 << r0");
        }

        [Test]
        public void PPCRw_fctiwz()
        {
            AssertCode(0xfc00081e, "fctiwz\tf0,f1");
        }
        [Test]
        public void PPCRw_fmul()
        {
            AssertCode(0xfc010032,
               "0|00100000(4): 1 instructions",
               "1|L--|f0 = f1 * f0");
        }

        [Test]
        public void PPCRw_fmul_()
        {
            AssertCode(0xfc010033,
                "0|00100000(4): 2 instructions",
                "1|L--|f0 = f1 * f0",
                "2|L--|cr1 = cond(f0)");
        }

        [Test]
        public void PPCRw_fcmpu()
        {
            AssertCode(0xff810000, "fcmpu\tcr7,f1,f0");
        }
        [Test]
        public void PPCRw_mtcrf()
        {
            AssertCode(0x7d808120, "mtcrf\t08,r12");
        }

        [Test]
        public void PPCRw_bctrl()
        {
            AssertCode(0x4e800421, // "bctrl\t14,00");
                "0|00100000(4): 1 instructions",
                "1|T--|call ctr (0)");
        }

        [Test]
        public void PPCRw_rlwimi()
        {
            AssertCode(0x5120f042,
                "0|00100000(4): 1 instructions",
                "rlwimi\tr0,r9,1E,01,01");
        }

        [Test]
        public void PPCRw_bl()
        {
            AssertCode(0x48000045, 
                "0|00100000(4): 1 instructions",
                "1|T--|call 00100044 (0)");
        }


        [Test]
        public void PPCRw_addis_r0()
        {
            AssertCode(0x3C000045,
                "0|00100000(4): 1 instructions",
                "1|L--|r0 = 0x00450000");
        }

        [Test]
        public void PPCRw_addis()
        {
            AssertCode(0x3C810045, 
                "0|00100000(4): 1 instructions",
                "1|L--|r4 = r1 + 0x00450000");
        }
    }
}
