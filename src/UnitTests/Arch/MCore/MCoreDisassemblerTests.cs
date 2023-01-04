#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.MCore;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.H8
{
    [TestFixture]
    public class MCoreDisassemblerTests : DisassemblerTestBase<MCoreInstruction>
    {
        private readonly MCoreArchitecture arch;
        private readonly Address addrLoad;

        public MCoreDisassemblerTests()
        {
            this.arch = new MCoreArchitecture(CreateServiceContainer(), "mcore", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        //[Test] // This spams the output.
        public void MCoreDis_Gen()
        {
            var rnd = new Random(0x1234);
            var bytes = new byte[0x10000];
            rnd.NextBytes(bytes);
            var mem = new ByteMemoryArea(Address.Ptr32(0x0010_0000), bytes);
            var dasm = arch.CreateDisassembler(mem.CreateBeReader(0));
            foreach (var instr in dasm)
            {
                instr.ToString();
            }
        }

        [Test]
        public void MCoreDis_addc()
        {
            AssertCode("addc\tr12,r11", "06BC");
        }

        [Test]
        public void MCoreDis_addi()
        {
            AssertCode("addi\tr13,0000001D", "21DD");
        }

        [Test]
        public void MCoreDis_andi()
        {
            AssertCode("andi\tr2,0000000E", "2EE2");
        }

        [Test]
        public void MCoreDis_andn()
        {
            AssertCode("andn\tr11,r15", "1FFB");
        }

        [Test]
        public void MCoreDis_bclri()
        {
            AssertCode("bclri\tr5,0000001C", "31C5");
        }

        [Test]
        public void MCoreDis_bf()
        {
            AssertCode("bf\t00008000", "EFFF");
        }

        [Test]
        public void MCoreDis_bsr()
        {
            AssertCode("bsr\t00008496", "FA4A");
        }

        [Test]
        public void MCoreDis_bt()
        {
            AssertCode("bt\t00007F6A", "E7B4");
        }

        [Test]
        public void MCoreDis_cmplti()
        {
            AssertCode("cmplti\tr9,00000003", "2239");
        }

        [Test]
        public void MCoreDis_ixw()
        {
            AssertCode("ixw\tr7,r3", "1537");
        }

        [Test]
        public void MCoreDis_ld_h()
        {
            AssertCode("ld.h\tr7,(r8,$C)", "C768");
        }

        [Test]
        public void MCoreDis_lrw()
        {
            AssertCode("lrw\t00007E34", "728D");
        }

        [Test]
        public void MCoreDis_lsli()
        {
            AssertCode("lsli\tr8,00000003", "3C38");
        }

        [Test]
        public void MCoreDis_lsr()
        {
            AssertCode("lsr\tr12,r1", "0B1C");
        }

        [Test]
        public void MCoreDis_mfcr()
        {
            AssertCode("mfcr\tr2,fpsr", "1132");
        }

        [Test]
        public void MCoreDis_movi()
        {
            AssertCode("movi\tr11,0000002D", "62DB");
        }

        [Test]
        public void MCoreDis_mtcr()
        {
            AssertCode("mtcr\tr13,gsr", "19CD");
        }

        [Test]
        public void MCoreDis_rotli()
        {
            AssertCode("rotli\tr7,00000017", "3977");
        }

        [Test]
        public void MCoreDis_rsubi()
        {
            AssertCode("rsubi\tr12,00000005", "285C");
        }

        [Test]
        public void MCoreDis_st()
        {
            AssertCode("st\tr6,(r6,$0)", "9606");
        }

        [Test]
        public void MCoreDis_st_b()
        {
            AssertCode("st.b\tr1,(r1,$1)", "B111");
        }

        [Test]
        public void MCoreDis_st_h()
        {
            AssertCode("st.h\tr8,(r6,$6)", "D836");
        }

        [Test]
        public void MCoreDis_tstnbz()
        {
            AssertCode("tstnbz\tr13", "019D");
        }

        [Test]
        public void MCoreDis_xsr()
        {
            AssertCode("xsr\tr7", "3807");
        }
    }
}