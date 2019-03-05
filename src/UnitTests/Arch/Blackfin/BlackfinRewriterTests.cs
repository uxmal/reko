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
using Reko.Arch.Blackfin;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Blackfin
{
    public class BlackfinRewriterTests : RewriterTestBase
    {
        public BlackfinRewriterTests()
        {
            this.Architecture = new BlackfinArchitecture("blackfin");
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress => Address.Ptr32(0x00100000);

        [Test]
        public void BlackfinRw_mov()
        {
            RewriteCode("0EE1EC0F");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_RTN()
        {
            RewriteCode("1300");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_mov_x()
        {
            RewriteCode("20E1E803");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_invalid()
        {
            RewriteCode("A100");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_xor3()
        {
            RewriteCode("C858");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_JUMP_L()
        {
            RewriteCode("FFE2D05C");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_RTS()
        {
            RewriteCode("1000");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mul()
        {
            RewriteCode("CA40");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mov_zb()
        {
            RewriteCode("4043");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mov_xb()
        {
            RewriteCode("0143");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mov_cc_eq()
        {
            RewriteCode("010C");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mov_cc_n_bittest()
        {
            RewriteCode("3948");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void BlackfinRw_sub3()
        {
            RewriteCode("4152");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_mov_cc_le()
        {
            RewriteCode("020D");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void BlackfinRw_CLI()
        {
            RewriteCode("3000");
            AssertCode(
                "0|S--|00100000(2): 1 instructions",
                "1|L--|__cli()");
        }

        [Test]
        public void BlackfinRw_lsr3()
        {
            RewriteCode("82C6F885");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }
    }
}