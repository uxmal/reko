#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Environments.Gameboy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Environments.Gameboy
{
    [TestFixture]
    public class GameboyRewriterTests : Arch.RewriterTestBase
    {
        private readonly GameboyArchitecture arch;
        private readonly Address addr;

        public GameboyRewriterTests()
        {
            this.arch = new GameboyArchitecture(CreateServiceContainer(), "lr35902", new Dictionary<string, object>());
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void GameboyRw_nop()
        {
            Given_HexString("00");
            AssertCode(     // nop
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void GameboyRw_jp()
        {
            Given_HexString("C35001");
            AssertCode(     // jp	0150
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0150");
        }

        [Test]
        public void GameboyRw_di()
        {
            Given_HexString("F3");
            AssertCode(     // di
                "0|L--|0100(1): 1 instructions",
                "1|L--|__disable_interrupts()");
        }

        [Test]
        public void GameboyRw_ld()
        {
            Given_HexString("57");
            AssertCode(     // ld	d,a
                "0|L--|0100(1): 1 instructions",
                "1|L--|d = a");
        }

        [Test]
        public void GameboyRw_ld_hl_sp_signed_offset()
        {
            Given_HexString("F8FC");
            AssertCode(     // ld	hl,sp-4h
                "0|L--|0100(2): 4 instructions",
                "1|L--|hl = sp - 4<i16>",
                "2|L--|HC = cond(hl)",
                "3|L--|Z = false",
                "4|L--|N = false");
        }
    }
}
