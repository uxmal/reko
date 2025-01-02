#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.IA64;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.IA64
{
    public class IA64DisassemblerTests : DisassemblerTestBase<IA64Instruction>
    {
        private readonly IA64Architecture arch;
        private readonly Address addrLoad;

        public IA64DisassemblerTests()
        {
            this.arch = new IA64Architecture(CreateServiceContainer(), "ia64", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr64(0x10_0000_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string hexBytes, params string[] sExpectedInstrs)
        {
            byte[] bytes = BytePattern.FromHexBytes(hexBytes);
            ByteMemoryArea mem = new ByteMemoryArea(LoadAddress, bytes);
            var dasm = arch.CreateDisassembler(arch.CreateImageReader(mem, 0U));
            var sInstrs = dasm.Select(i => i.ToString()).ToArray();
            var c = Math.Min(sExpectedInstrs.Length, sInstrs.Length);
            for (int i = 0; i < c; ++i)
            {
                Assert.AreEqual(sExpectedInstrs[i], sInstrs[i]);
            }
            Assert.AreEqual(sExpectedInstrs.Length, sInstrs.Length, "Incorrect # of instructions");
        }

        [Test]
        public void IA64Dis_break_mii()
        {
            AssertCode("E0070000 00000000 00000000 00000000",
                "(p63) break.m\t0x0",
                "break.i\t0x0",
                "break.i\t0x0");
        }

        [Test]
        public void IA64Dis_mov_nop_i()
        {
            AssertCode("0B10001C0021E0A0F8594F0000000400",
                "adds\tr2,0,r14", // ;
                "addl\tr14,-10732,r2",
                "nop.i\t0x0");
        }

        [Test]
        [Ignore("Investigate this")]
        public void IA64Dis_alloc_mlx()
        {
            AssertCode("04101C008045024C80090060F0131A60",
                "alloc\tr2,ar.pfs,7,0,0",
                "movl\tr3,0x......");
        }
    }
}
