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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.X86.Rewriter
{
    [TestFixture]
    public class V20RewriterTests : Arch.RewriterTestBase
    {
        private readonly IntelArchitecture arch;

        public V20RewriterTests()
        {
            arch = new X86ArchitectureReal(
                CreateServiceContainer(),
                "x86-real-16",
                new Dictionary<string, object>
                {
                    { ProcessorOption.InstructionSet, "v20" }
                });
            LoadAddress = Address.SegPtr(0x0C00, 0x0000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress { get; }

        [Test]
        public void V20Rw_enter_is_rewritten()
        {
            Given_HexString("C8 10 00 00");
            AssertCode(
                "0|L--|0C00:0000(4): 4 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[ss:sp:word16] = bp",
                "3|L--|bp = sp",
                "4|L--|sp = sp - 16<i16>");
        }

        [Test]
        public void V20Rw_identifiers_use_v20_register_names()
        {
            Given_HexString("01 C8");
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|aw = aw + cw",
                "2|L--|SCZO = cond(aw)");
        }
    }
}
