#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVRewriterTests : RewriterTestBase
    {
        private RiscVArchitecture arch = new RiscVArchitecture();
        private Address baseAddr = Address.Ptr64(0x0010000);
        private MemoryArea image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            var segMap = new SegmentMap(baseAddr, new ImageSegment("code", image, AccessMode.ReadExecute));
            var state = (RiscVState)arch.CreateProcessorState(segMap);
            return new RiscVRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
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
            var dasm = new RiscVDisassembler(arch, image.CreateLeReader(LoadAddress));
            return image;
        }


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RiscV_rw_auipc()
        {
            Rewrite(0xFFFFF517u); // auipc\tgp,0x000FFFFD
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|a0 = 000000000000F000");
        }

        [Test]
        public void RiscV_rw_lb()
        {
            Rewrite(0x87010183u);
            AssertCode(
               "0|L--|0000000000010000(4): 1 instructions",
               "1|L--|gp = (word64) Mem0[sp + -1936:int8]");
        }

        [Test]
        public void RiscV_rw_jal_zero()
        {
            Rewrite(0x9F4FF06Fu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto 000000000000F1F4");
        }

        [Test]
        public void RiscV_rw_jal_not_zero()
        {
            Rewrite(0x9F4FF0EFu);
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call 000000000000F1F4 (0)");
        }

        [Test]
        public void RiscV_rw_jalr_zero()
        {
            Rewrite(0x00078067); // jalr zero, a5, 0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|goto a5");
        }

        [Test]
        public void RiscV_rw_jalr_zero_ra()
        {
            Rewrite(0x00008067); // jalr zero,ra,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void RiscV_rw_jalr_ra()
        {
            Rewrite(0x003780E7);    // jalr ra,a5,0
            AssertCode(
                "0|T--|0000000000010000(4): 1 instructions",
                "1|T--|call a5 + 3 (0)");
        }

        [Test]
        public void RiscV_rw_sd()
        {
            Rewrite(0x19513423u);    // sd\ts5,sp,392
            AssertCode(
                "0|L--|0000000000010000(4): 1 instructions",
                "1|L--|Mem0[sp + 392:word64] = s5");
        }

        [Test]
        public void RiscV_rw_lui()
        {
            Rewrite(0x000114B7u);   // lui s1,0x00000011
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|s1 = 0x0000000000011000");
        }

        [Test]
        public void RiscV_rw_lh()
        {
            Rewrite(0x03131083u);   // lh
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|ra = (word64) Mem0[t1 + 49:int16]");
        }

        [Test]
        public void RiscV_rw_fmadd()
        {
            Rewrite(0x8293FD43);
            AssertCode(
                 "0|L--|0000000000010000(4): 1 instructions",
                 "1|L--|fs10 = ft7 * fs1 + fa6");
        }
    }
}
