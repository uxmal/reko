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
using Reko.Arch.MN103;
using Reko.Core;
using Reko.Core.Memory;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.MN103
{
    [TestFixture]
    public class MN103DisassemblerTests : DisassemblerTestBase<MN103Instruction>
    {
        private readonly MN103Architecture arch;
        private readonly Address addr;

        public MN103DisassemblerTests()
        {
            this.arch = new MN103Architecture(CreateServiceContainer(), "mn103", new(), new(), new());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes, InstrClass expectedClass = InstrClass.Linear)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
            Assert.AreEqual(expectedClass, instr.InstructionClass);
        }

        [Test]
        public void MN103Dis_calls_indirect()
        {
            AssertCode("calls\t(a2)", "F0F2", InstrClass.CallInd);
        }

        [Test]
        public void MN103Dis_calls_d16()
        {
            AssertCode("calls\t00101234", "FAFF3412", InstrClass.Call);
        }

        [Test]
        public void MN103Dis_calls_d32()
        {
            // FC FF: calls (d32,PC); the target is relative to the address
            // of the calls instruction itself.
            AssertCode("calls\t00101234", "FCFF34120000", InstrClass.Call);
        }

        [Test]
        public void MN103Dis_calls_d32_negative_displacement()
        {
            AssertCode("calls\t000FFF00", "FCFF00FFFFFF", InstrClass.Call);
        }

        [Test]
        public void MN103Dis_mov_d32_sp_load()
        {
            // FC Bx: (d32,SP) accesses are 6-byte instructions carrying
            // a full 32-bit displacement.
            AssertCode("mov\t(12345678,sp),d1", "FCB578563412");
        }

        [Test]
        public void MN103Dis_mov_d32_sp_store()
        {
            AssertCode("mov\td1,(12345678,sp)", "FC9578563412");
        }

        [Test]
        public void MN103Dis_movbu_d32_sp_store()
        {
            AssertCode("movbu\td1,(12345678,sp)", "FC9678563412");
        }

        [Test]
        public void MN103Dis_movhu_d32_sp_load()
        {
            AssertCode("movhu\t(12345678,sp),d1", "FCBD78563412");
        }

        [Test]
        public void MN103Dis_Generate()
        {
            var rnd = new Random(0x103);
            var buf = new byte[65536];
            rnd.NextBytes(buf);
            var mem = new ByteMemoryArea(addr, buf);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            var instrs = dasm.ToArray();
        }
    }
}
