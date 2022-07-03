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
using Reko.Arch.Fujitsu;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Fujitsu.F2MC16FX
{
    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        private F2MC16FXArchitecture arch;

        public RewriterTests()
        {
            this.arch = new F2MC16FXArchitecture(CreateServiceContainer(), "f2mc16fx", new());
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        [Test]
        public void F2MC16FXRw_movx()
        {
            Given_HexString("576FB0");
            AssertCode(     // movx	a,0B06Fh
                "0|L--|00100000(3): 2 instructions",
                "1|L--|ah = al",
                "2|L--|al = CONVERT(Mem0[0xB06F<16>:byte], byte, int16)");
        }

        [Test]
        public void F2MC16FXRw_call()
        {
            Given_HexString("648576");
            AssertCode(     // call	7685h
                "0|T--|00100000(3): 1 instructions",
                "1|T--|call 7685 (2)");
        }

        [Test]
        public void F2MC16FXRw_nop()
        {
            Given_HexString("00");
            AssertCode(     // nop
                "0|L--|00100000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void F2MC16FXRw_mulu()
        {
            Given_HexString("7804");
            AssertCode(     // mulu a,r4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v2 = SLICE(a, uint8, 0)",
                "2|L--|al = v2 *u16 r4");
        }
    }
}
