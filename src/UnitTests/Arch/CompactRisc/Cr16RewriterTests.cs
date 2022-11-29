#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.CompactRisc;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.CompactRisc
{
    public class Cr16RewriterTests : RewriterTestBase
    {
        public Cr16RewriterTests()
        {
            this.Architecture = new Cr16Architecture(CreateServiceContainer(), "cr16c", new());
            this.LoadAddress = Address.Ptr32(0x3000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        [Test]
        public void Cr16Rw_movb_imm()
        {
            Given_HexString("1258");   // movb\t$1,r2
            AssertCode(
                "0|L--|00003000(2): 1 instructions",
                "1|L--|r2 = CONVERT(1<8>, int8, int16)");
        }

        [Test]
        public void Cr16Rw_movd()
        {
            Given_HexString("0005C2E2");
            AssertCode(     // movd	$E2C2,r0
                "0|L--|00003000(4): 1 instructions",
                "1|L--|r0 = 0xE2C2<24>");
        }

        [Test]
        public void Cr16Rw_push()
        {
            Given_HexString("1001");
            AssertCode(     // push	$1,r0
                "0|L--|00003000(2): 2 instructions",
                "1|L--|sp = sp - 2<i32>",
                "2|L--|Mem0[sp:word16] = r0");
        }
    }
}
