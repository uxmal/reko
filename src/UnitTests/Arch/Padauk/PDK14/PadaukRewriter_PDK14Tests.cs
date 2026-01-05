#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Padauk;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.UnitTests.Arch.Padauk.PDK14
{
    [TestFixture]
    public class PadaukRewriter_PDK14Tests : RewriterTestBase
    {
        private readonly PadaukArchitecture arch;
        private readonly Address addrLoad;

        public PadaukRewriter_PDK14Tests()
        {
            this.arch = new PadaukArchitecture(
                CreateServiceContainer(),
                "padauk",
                new()
                {
                    { ProcessorOption.InstructionSet, "14" }
                });
            this.addrLoad = Address.Ptr16(0x100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        [Test]
        public void Pdk14Rw_add_m_a()
        {
            Given_HexString("4208");
            AssertCode(     // add [m],a
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = Mem0[0x0042<p16>:byte] + a",
                "2|L--|Mem0[0x0042<p16>:byte] = v4",
                "3|L--|ZCAV = cond(v4)");
        }

        [Test]
        public void Pdk14Rw_nop()
        {
            Given_HexString("0000");
            AssertCode(     // nop
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }
    }
}
