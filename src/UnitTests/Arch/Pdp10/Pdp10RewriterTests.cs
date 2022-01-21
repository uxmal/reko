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
using Reko.Arch.Pdp10;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Pdp10
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class Pdp10RewriterTests : RewriterTestBase
    {
        private readonly Pdp10Architecture arch;
        private readonly Address18 addr;

        public Pdp10RewriterTests()
        {
            this.arch = new Pdp10Architecture(CreateServiceContainer(), "pdp10", new Dictionary<string, object>());
            this.addr = new Address18(0x0010_000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void Given_OctalWord(string octalWord)
        {
            var word = Pdp10Architecture.OctalStringToWord(octalWord);
            Given_MemoryArea(new Word36MemoryArea(addr, new[] { word }));
        }

        [Test]
        public void Pdp10Rw_jrst()
        {
            Given_OctalWord("254000034726");
            AssertCode(     // jrst	034726
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 034726");
        }

        [Test]
        public void Pdp10Rw_move()
        {
            Given_OctalWord("200300042175");
            AssertCode(     // move	6,42175
                "0|L--|200000(1): 1 instructions",
                "1|L--|r6 = Mem0[0x042175<p36>:word36]");
        }

        [Test]
        public void Pdp10Rw_movei()
        {
            Given_OctalWord("201440777777");
            AssertCode(     // movei	11,777777
                "0|L--|200000(1): 1 instructions",
                "1|L--|r9 = 0x3FFFF<36>");
        }
    }
}
