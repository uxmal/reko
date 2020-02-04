#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class Mips16eRewriterTests : RewriterTestBase
    {
        private MipsProcessorArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture("mips-be-32");
            this.addr = Address.Ptr32(0x00100000);
            this.arch.LoadUserOptions(new Dictionary<string, object> { { "decoder", "mips16e" } });
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                arch.Endianness.CreateImageReader(mem, mem.BaseAddress),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        private void AssertCode(string sHex, params string[] sExp)
        {
            Given_HexString(sHex);
            AssertCode(sExp);
        }

        [Test]
        [Ignore("Work on disassembler first")]
        public void Mips16eRw_save()
        {
            AssertCode("save\tra,r17,r16,+00000080", "64F0");
        }

    }
}
