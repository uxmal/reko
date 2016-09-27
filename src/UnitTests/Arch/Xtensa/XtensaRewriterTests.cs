#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Arch.Xtensa;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaRewriterTests : RewriterTestBase
    {
        private XtensaArchitecture arch = new XtensaArchitecture();
        private Address baseAddr = Address.Ptr32(0x0010000);
        private XtensaProcessorState state;
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new XtensaRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        protected override MemoryArea RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
                (byte) (w >> 16),
                (byte) (w >> 24)
            }).ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            var dasm = new XtensaDisassembler(arch, image.CreateLeReader(LoadAddress));
            return image;
        }


        [SetUp]
        public void Setup()
        {
            state = (XtensaProcessorState)arch.CreateProcessorState();
        }

        [Test]
        public void Xtrw_l32r()
        {
            Rewrite(0xFFFF71u); // l32r\ta7,000FFFFC
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a7 = 0000FFFC");
            Rewrite(0xFFFE21u); // l32r\ta2,000FFFF8
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = 0000FFF8");
        }

        [Test]
        public void Xtrw_ret()
        {
            Rewrite(0x000080);  // ret
            AssertCode(
                 "0|T--|00010000(3): 1 instructions",
                 "1|T--|return (0,0)");
        }

        [Test]
        public void Xtrw_ill()
        {
            Rewrite(0x000000);  // ill
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__ill()");
        }

        [Test]
        public void Xtrw_wsr()
        {
            Rewrite(0x13E720); // wsr\ta2,VECBASE
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|VECBASE = a2");
        }

        [Test]
        public void Xtrw_or()
        {
            Rewrite(0x201110u); // "or\ta1,a1,a1
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|a1 = a1 | a1");
        }

        [Test]
        public void Xtrw_call0()
        {
            Rewrite(0x00B205u);   // call0\t00100B24
            AssertCode(
              "0|T--|00010000(3): 2 instructions",
              "1|L--|a0 = 00010003",
              "2|T--|call 00010B24 (0)");
        }

        [Test]
        public void Xtrw_reserved()
        {
            Rewrite(0xFE9200);  // reserved\t
            AssertCode(
            "0|L--|00010000(3): 1 instructions",
            "1|L--|__reserved()");
        }

        [Test]
        public void Xtrw_movi()
        {
            Rewrite(0xA0A392u); // movi\ta9,000003A0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a9 = 0x000003A0");
        }

        [Test]
        public void Xtrw_sub()
        {
            Rewrite(0xC01190); // "sub\ta1,a1,a9"
            AssertCode(
                "0|L--|00010000(3): 1 instructions");
        }

        [Test]
        public void Xtrw_s32i()
        {
            Rewrite(0xE561D2); // "s32i\ta13,a1,0x00000394"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x00000394:word32] = a13");
            Rewrite(0xE76102); // "s32i\ta0,a1,0x0000039C"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x0000039C:word32] = a0");
        }

        [Test]
        public void Xtrw_memw()
        {
            Rewrite(0x0020C0); // "memw\t"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Xtrw_l32i()
        {
            Rewrite(0x7D48); // "l32i.n\ta4,a13,28"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = Mem0[a13 + 0x0000001C:word32]");
        }
    }
}
