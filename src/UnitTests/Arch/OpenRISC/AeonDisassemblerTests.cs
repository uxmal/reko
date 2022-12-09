#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.OpenRISC;
using Reko.Arch.OpenRISC.Aeon;
using Reko.Core;
using Reko.Core.Memory;
using System.Linq;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonDisassemblerTests : DisassemblerTestBase<AeonInstruction>
    {
        private readonly AeonArchitecture arch;
        private readonly Address addrLoad;

        public AeonDisassemblerTests()
        {
            // Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
            this.arch = new AeonArchitecture(CreateServiceContainer(), "aeon", new());
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        private void AssertCode(params string[] arguments)
        {
            var hexBytes = arguments[^1];
            byte[] bytes = BytePattern.FromHexBytes(hexBytes);
            var mem = new ByteMemoryArea(LoadAddress, bytes);
            var dasm = this.CreateDisassembler(Architecture.CreateImageReader(mem, 0U));
            var instrs = dasm.Take(arguments.Length - 1).ToArray();
            for (int i = 0; i < arguments.Length - 1; ++i)
            {
                var sInstr = instrs[i].ToString();
                Assert.AreEqual(arguments[i], sInstr, $"Instruction {i}");
            }
        }

        [Test]
        public void AeonDis_bt_trap()
        {
            // XXX: actually "bt.trap 1"
            AssertCode("bt.trap", "8002");
        }

        [Test]
        public void AeonDis_bt_add__()
        {
            AssertCode("bt.add?\tr7,r6", "8C E6");
        }

        [Test]
        public void AeonDis_bn_add()
        {
            // confirmed with source
            AssertCode("bn.add\tr4,r3,r6", "40 83 34");
        }

        [Test]
        public void AeonDis_bt_addi__()
        {
            AssertCode("bt.addi?\tr1,-0x4", "9C 3C");
        }

        [Test]
        public void AeonDis_bn_addi()
        {
            // confirmed with source
            AssertCode("bn.addi\tr3,r3,0x10", "1C 63 10");
        }

        [Test]
        public void AeonDis_bg_addi()
        {
            AssertCode("bg.addi\tr3,r13,0x0", "fc 6d 00 00");
        }

        [Test]
        public void AeonDis_bg_addi_negative()
        {
            AssertCode("bg.addi\tr6,r6,-0x767C", "FC C6 89 84");
        }

        [Test]
        public void AeonDis_bn_beqi__()
        {
            AssertCode("bn.beqi?\tr3,0x0,00100011", "20 60 44");
        }

        [Test]
        public void AeonDis_bg_blti__()
        {
            AssertCode("bg.blti?\tr4,0x8,000FFFE7", "D0 88 FF 3E");
        }

        [Test]
        public void AeonDis_bn_bf()
        {
            // confirmed with source
            AssertCode("bn.bf\t000FFFF2", "23 FF C9");
        }

        [Test]
        public void AeonDis_l_cmov()
        {
            // speculative guess.
            AssertCode("l.cmov??\tr7,r7,r0", "48 E7 00");
        }

        [Test]
        public void AeonDis_l_cmovi()
        {
            // speculative guess.
            AssertCode("l.cmovi??\tr12,r12,0x1", "49 8C 0A");
        }

        [Test]
        public void AeonDis_bn_divu()
        {
            // confirmed with source
            AssertCode("bn.divu\tr6,r6,r7", "40 C6 39");
        }

        [Test]
        public void AeonDis_bg_flush_line()
        {
            // confirmed with source
            AssertCode("bg.flush.line\t(r3),0x0", "F4 03 00 06");
        }

        [Test]
        public void AeonDis_bg_invalidate_line()
        {
            // confirmed with source
            AssertCode("bg.invalidate.line\t(r3),0x1", "F4 03 00 37");
        }

        [Test]
        public void AeonDis_bt_j()
        {
            // confirmed with source
            AssertCode("bt.j\t000FFFF4", "93 F4");
        }

        [Test]
        public void AeonDis_bg_jal()
        {
            AssertCode("bg.jal\t000F17F9", "E7 FE 2F F2");
        }

        [Test]
        public void AeonDis_bt_jr()
        {
            AssertCode("bt.jr\tr7", "84 E9");
        }

        [Test]
        public void AeonDis_bt_jr_ret()
        {
            // confirmed with source
            AssertCode("bt.jr\tr9", "85 29");
        }


        [Test]
        public void AeonDis_bg_lbz__()
        {
            AssertCode("bg.lbz?\tr6,0x1EEC(r7)", "F0 C7 1E EC");
        }

        [Test]
        public void AeonDis_bn_lhz()
        {
            AssertCode("bn.lhz\tr5,0x8(r11)", "08 AB 09");
        }

        [Test]
        public void AeonDis_bn_lhz_0()
        {
            // confirmed with source
            AssertCode("bn.lhz\tr3,(r5)", "08 65 01");
        }

        [Test]
        public void AeonDis_bg_lhz__()
        {
            AssertCode("bg.lhz?\tr3,0x3A46(r7)", "E8 67 3A 47");
        }

        [Test]
        public void AeonDis_bt_mov__()
        {
            AssertCode("bt.mov?\tr10,r3", "89 43");
        }

        [Test]
        public void AeonDis_bt_movi()
        {
            AssertCode("bt.movi?\tr6,-0x1", "98 DF");
        }

        [Test]
        public void AeonDis_l_movhi_32bit()
        {
            // confirmed with source
            AssertCode("l.movhi\tr7,0xA020", "C0 F4 04 01");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_load()
        {
            AssertCode(
                "l.movhi\tr6,0x523A3C@hi",
                "bg.lwz?\tr7,0x523A3C@lo(r6)",
                "C0 C0 0A 41" +
                "EC E6 3A 3E");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_store()
        {
            AssertCode(
                "l.movhi\tr7,0x523A05@hi",
                "bg.sb?\t0x523A05@lo(r7),r6",
                "C0 E0 0A 41" +
                "F8 C7 3A 05");
        }

        [Test]
        public void AeonDis_l_movhi_fuse_with_addi()
        {
            AssertCode(
                "l.movhi\tr12,0x3988F0@hi",
                "bg.addi\tr10,r12,0x3988F0@lo",
                "C1 80 07 41" +
                "FD 4C 88 F0");
        }

        [Test]
        public void AeonDis_l_movhi_fuse_with_ori()
        {
            AssertCode(
                "l.movhi\tr6,0x7FFFFFFF@hi",
                "l.ori\tr6,r6,0x7FFFFFFF@lo",
                "C0 CF FF E1" +
                "C8 C6 FF FF");
        }

        [Test]
        public void AeonDis_bg_mfspr()
        {
            // confirmed with source
            AssertCode("bg.mfspr\tr3,r0,0x11", "C0 60 01 1F");
        }

        [Test]
        public void AeonDis_bg_mtspr()
        {
            // confirmed with source
            AssertCode("bg.mtspr\tr0,r3,0x11", "C0 60 01 1D");
        }

        [Test]
        public void AeonDis_bn_nand__()
        {
            // confirmed with source
            AssertCode("bn.nand?\tr6,r4,r4", "44 C4 27");
        }

        [Test]
        public void AeonDis_bt_nop()
        {
            // confirmed with source
            AssertCode("bt.nop", "8001");
        }

        [Test]
        public void AeonDis_bn_nop()
        {
            AssertCode("bn.nop", "000000");
        }

        [Test]
        public void AeonDis_bn_or()
        {
            // confirmed with source
            AssertCode("bn.or\tr3,r0,r1", "44 60 0D");
        }

        [Test]
        public void AeonDis_l_ori()
        {
            // confirmed with source
            AssertCode("l.ori\tr5,r7,0x2400", "C8 A7 24 00");
        }

        [Test]
        public void AeonDis_bn_sb__()
        {
            AssertCode("bn.sb?\t0x7(r1),r0", "18 01 07");
        }

        [Test]
        public void AeonDis_bg_sb__()
        {
            AssertCode("bg.sb?\t0x36D8(r10),r7", "F8 EA 36 D8");
        }

        [Test]
        public void AeonDis_bn_sfeqi()
        {
            // confirmed with source
            AssertCode("bn.sfeqi\tr3,0x0", "5C 60 01");
        }

        [Test]
        public void AeonDis_bn_sfgeu()
        {
            // confirmed with source
            AssertCode("bn.sfgeu\tr3,r4", "5C 83 17");
        }
        

        [Test]
        public void AeonDis_bn_sfleui__()
        {
            AssertCode("bn.sfleui?\tr3,0x77", "5C 6E F3");
        }

        [Test]
        public void AeonDis_bn_sfltu()
        {
            // confirmed with source
            AssertCode("bn.sfltu\tr3,r4", "5C 83 1F");
        }

        [Test]
        public void AeonDis_bn_sfne()
        {
            // confirmed with source
            AssertCode("bn.sfne\tr3,r4", "5C 64 0D");
        }

        [Test]
        public void AeonDis_bg_sfnei__()
        {
            AssertCode("bg.sfnei?\tr6,0x3E8", "C0 C0 7D 04");
        }

        [Test]
        public void AeonDis_bg_sh__()
        {
            AssertCode("bg.sh?\t0x345A(r7),r3", "EC 67 34 5B");
        }

        [Test]
        public void AeonDis_bn_srai()
        {
            AssertCode("bn.srai?\tr5,r7,0x1", "4C A7 0A");
        }

        [Test]
        public void AeonDis_bn_sub()
        {
            // confirmed with source
            AssertCode("bn.sub\tr6,r6,r5", "40 C6 2D");
        }

        [Test]
        public void AeonDis_bn_sw()
        {
            // confirmed with source
            AssertCode("bn.sw\t(r3),r0", "0C 03 00");
        }

        [Test]
        public void AeonDis_bg_sw()
        {
            // confirmed with source
            AssertCode("bg.sw\t0x88(r1),r5", "EC A1 00 88");
        }

        [Test]
        public void AeonDis_bn_xor__()
        {
            AssertCode("bn.xor?\tr7,r4,r3", "44 E4 1E");
        }
    }
}
