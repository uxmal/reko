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
using System.Linq;

namespace Reko.UnitTests.Arch.X86.Disassembler
{
    [TestFixture]
    public class V20DisassemblerTests : DisassemblerTestBase<X86Instruction>
    {
        private readonly IProcessorArchitecture v20Arch;
        private readonly IProcessorArchitecture x86Arch;

        public V20DisassemblerTests()
        {
            v20Arch = new X86ArchitectureReal(
                CreateServiceContainer(),
                "x86-real-16",
                new Dictionary<string, object>
                {
                    { ProcessorOption.InstructionSet, "v20" }
                });
            x86Arch = new X86ArchitectureReal(
                CreateServiceContainer(),
                "x86-real-16",
                new Dictionary<string, object>());
            LoadAddress = Address.SegPtr(0x0800, 0x0000);
        }

        public override IProcessorArchitecture Architecture => v20Arch;
        public override Address LoadAddress { get; }

        private static X86Instruction Disassemble(IProcessorArchitecture arch, Address loadAddress, string hexBytes)
        {
            var bytes = BytePattern.FromHexBytes(hexBytes);
            var mem = arch.CreateCodeMemoryArea(loadAddress, bytes);
            var rdr = arch.CreateImageReader(mem, 0);
            return (X86Instruction)arch.CreateDisassembler(rdr).First();
        }

        [Test]
        public void V20Dis_pusha_is_valid()
        {
            var instr = Disassemble(v20Arch, LoadAddress, "60");
            Assert.AreEqual("pusha", instr.ToString());
        }

        [Test]
        public void V20Dis_alias_mnemonics_only_in_v20_mode()
        {
            var v20Instr = Disassemble(v20Arch, LoadAddress, "72 00");
            var x86Instr = Disassemble(x86Arch, LoadAddress, "72 00");

            Assert.AreEqual("jb\t0002h", v20Instr.ToString());
            Assert.AreEqual("jc\t0002h", x86Instr.ToString());
        }

        [Test]
        public void V20Dis_register_aliases_are_used()
        {
            var instrAw = Disassemble(v20Arch, LoadAddress, "B8 34 12");
            var instrIx = Disassemble(v20Arch, LoadAddress, "BE 78 56");

            Assert.AreEqual("mov\taw,1234h", instrAw.ToString());
            Assert.AreEqual("mov\tix,5678h", instrIx.ToString());
        }

        [Test]
        public void V20Dis_repc_for_cmps_scas()
        {
            var repCmps = Disassemble(v20Arch, LoadAddress, "F3 A6");
            var repScas = Disassemble(v20Arch, LoadAddress, "F3 AE");

            Assert.AreEqual("repc cmpbk", repCmps.ToString());
            Assert.AreEqual("repc cmpm", repScas.ToString());
        }

        [Test]
        public void V20Dis_repnc_for_cmps_scas()
        {
            var repneCmps = Disassemble(v20Arch, LoadAddress, "F2 A6");
            var repneScas = Disassemble(v20Arch, LoadAddress, "F2 AE");

            Assert.AreEqual("repnc cmpbk", repneCmps.ToString());
            Assert.AreEqual("repnc cmpm", repneScas.ToString());
        }

        [Test]
        public void V20Dis_rep_kept_for_non_compare_string_instructions()
        {
            var repMovs = Disassemble(v20Arch, LoadAddress, "F3 A4");
            Assert.AreEqual("rep movbk", repMovs.ToString());
        }

        // V20 bit-manipulation instructions (0F prefix)

        [Test]
        public void V20Dis_test1_reg8_CL()
        {
            // 0F 10 /r: TEST1 r/m8, CL
            var instr = Disassemble(v20Arch, LoadAddress, "0F 10 C3");
            Assert.AreEqual("test1\tbl,cl", instr.ToString());
        }

        [Test]
        public void V20Dis_test1_reg8_imm()
        {
            // 0F 18 /r ib: TEST1 r/m8, imm8
            var instr = Disassemble(v20Arch, LoadAddress, "0F 18 C3 05");
                Assert.AreEqual("test1\tbl,5h", instr.ToString());
        }

        [Test]
        public void V20Dis_clr1_reg16_CL()
        {
            // 0F 13 /r: CLR1 r/m16, CL
            var instr = Disassemble(v20Arch, LoadAddress, "0F 13 C1");
            Assert.AreEqual("clr1\tcw,cl", instr.ToString());
        }

        [Test]
        public void V20Dis_set1_reg8_imm()
        {
            // 0F 1C /r ib: SET1 r/m8, imm8
            var instr = Disassemble(v20Arch, LoadAddress, "0F 1C C0 03");
                Assert.AreEqual("set1\tal,3h", instr.ToString());
        }

        [Test]
        public void V20Dis_not1_reg16_CL()
        {
            // 0F 17 /r: NOT1 r/m16, CL
            var instr = Disassemble(v20Arch, LoadAddress, "0F 17 C2");
            Assert.AreEqual("not1\tdw,cl", instr.ToString());
        }

        [Test]
        public void V20Dis_add4s()
        {
            // 0F 20: ADD4S
            var instr = Disassemble(v20Arch, LoadAddress, "0F 20");
            Assert.AreEqual("add4s", instr.ToString());
        }

        [Test]
        public void V20Dis_sub4s()
        {
            // 0F 22: SUB4S
            var instr = Disassemble(v20Arch, LoadAddress, "0F 22");
            Assert.AreEqual("sub4s", instr.ToString());
        }

        [Test]
        public void V20Dis_cmp4s()
        {
            // 0F 26: CMP4S
            var instr = Disassemble(v20Arch, LoadAddress, "0F 26");
            Assert.AreEqual("cmp4s", instr.ToString());
        }

        [Test]
        public void V20Dis_rol4()
        {
            // 0F 28 /r: ROL4 r/m8
            var instr = Disassemble(v20Arch, LoadAddress, "0F 28 03");
            Assert.AreEqual("rol4\tbyte ptr [bp+iy]", instr.ToString());
        }

        [Test]
        public void V20Dis_ror4()
        {
            // 0F 2C /r: ROR4 r/m8
            var instr = Disassemble(v20Arch, LoadAddress, "0F 2C C0");
            Assert.AreEqual("ror4\tal", instr.ToString());
        }

        [Test]
        public void V20Dis_brkem()
        {
            // 0F FF ib: BRKEM imm8
            var instr = Disassemble(v20Arch, LoadAddress, "0F FF 42");
            Assert.AreEqual("brkem\t42h", instr.ToString());
        }
    }
}
