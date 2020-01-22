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
using Reko.Arch.V850;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.V850
{
    [TestFixture]
    public class V850DisassemblerTests : DisassemblerTestBase<V850Instruction>
    {
        private readonly V850Architecture arch = new V850Architecture("v850");
        private readonly Address addr = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexInstruction)
        {
            var instr = base.DisassembleHexBytes(hexInstruction);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void V850Dis_Generate()
        {
            var buf = new byte[1000];
            var rnd = new Random(0x4711);
            rnd.NextBytes(buf);
            var mem = new MemoryArea(addr, buf);
            var rdr = mem.CreateLeReader(mem.BaseAddress);
            var dasm = arch.CreateDisassembler(rdr);
            foreach (var instr in dasm)
            {
                dasm.ToString();
            }
        }


        [Test]
        public void V850Dis_mov()
        {
            AssertCode("mov\tr31,r25", "1FC8");
        }

        [Test]
        public void V850Dis_mov_neg_imm()
        {
            AssertCode("mov\tFFFFFFF0,r17", "108A");
        }

        [Test]
        public void V850Dis_nop()
        {
            AssertCode("nop", "0000");
        }

        [Test]
        public void V850Dis_or()
        {
            AssertCode("or\tr2,r24", "02C1");
        }

        [Test]
        public void V850Dis_sld_b()
        {
            AssertCode("sld.b\t3[ep],r14", "0373");
        }

        [Test]
        public void V850Dis_sld_h()
        {
            AssertCode("sld.h\t6[ep],r24", "06C4");
        }

        [Test]
        public void V850Dis_sld_w()
        {
            AssertCode("sld.w\t0[ep],r8", "0045");
        }

        [Test]
        public void V850Dis_sst_b()
        {
            AssertCode("sst.b\tr15,108[ep]", "EC7B");
        }

        [Test]
        public void V850Dis_sst_h()
        {
            AssertCode("sst.h\tr24,90[ep]", "DAC4");
        }

        [Test]
        public void V850Dis_sst_w()
        {
            AssertCode("sst.w\tr24,2[ep]", "05C5");
        }

        /////////////////////////////////////////////////////////////////////////

    }
}
