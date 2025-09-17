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
            // confirmed with source
            AssertCode("bt.trap\t0x1", "80 02");
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
        public void AeonDis_bn_addc__()
        {
            // Always found right after an add.
            AssertCode("bn.addc?\tr4,r4,r8", "40 84 47");
        }

        [Test]
        public void AeonDis_bg_addci__()
        {
            // Always found right after an addi.
            AssertCode("bg.addci?\tr8,r8,-0x1", "DD 08 FF FF");
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
            AssertCode("bg.addi\tr3,r13,0x0", "FC 6D 00 00");
        }

        [Test]
        public void AeonDis_bg_addi_negative()
        {
            AssertCode("bg.addi\tr6,r6,-0x767C", "FC C6 89 84");
        }

        [Test]
        public void AeonDis_bg_andi()
        {
            // confirmed with source
            AssertCode("bg.andi\tr3,r3,0x8000", "C4 63 80 00");
        }

        [Test]
        public void AeonDis_bg_b__bitset__()
        {
            AssertCode("bg.b?bitseti?\tr3,0x1F,000FFF6F", "D0 7F FB 7C");
        }

        [Test]
        public void AeonDis_bg_beq__()
        {
            AssertCode("bg.beq?\tr7,r3,000FFFF1", "D4 E3 FF 8A");
        }

        [Test]
        public void AeonDis_bn_beqi__()
        {
            AssertCode("bn.beqi?\tr3,0x0,00100011", "20 60 44");
        }

        [Test]
        public void AeonDis_bg_beqi__()
        {
            AssertCode("bg.beqi?\tr10,0x1,00100095", "D1 41 04 AA");
        }

        [Test]
        public void AeonDis_bn_bf()
        {
            // confirmed with source
            AssertCode("bn.bf\t000FFFF2", "23 FF C9");
        }

        [Test]
        public void AeonDis_bg_bf()
        {
            // confirmed with source
            AssertCode("bg.bf\t00100DDD", "D4 00 6E EB");
        }

        [Test]
        public void AeonDis_bg_bges__()
        {
            AssertCode("bg.bges?\tr3,r11,00100097", "D4 6B 04 B9");
        }

        [Test]
        public void AeonDis_bg_bgeu__()
        {
            // for-loop with unsigned limits.
            AssertCode("bg.bgeu?\tr7,r5,000FFFE3", "D4 E5 FF 1D");
        }

        [Test]
        public void AeonDis_bn_bgtui__()
        {
            AssertCode("bn.bgtui?\tr5,0x3,000FFFF0", "24 AF C3");
        }

        [Test]
        public void AeonDis_bg_bgts()
        {
            AssertCode("bg.bgts?\tr23,r5,000FFFF2", "D6 E5 FF 94 ");
        }

        [Test]
        public void AeonDis_bg_bgtui__()
        {
            // This is being used in unsigned comparisons
            // swtich (...) statements.
            AssertCode("bg.bgtui?\tr3,0xD,00100016", "D0 6D 00 B5");
        }

        [Test]
        public void AeonDis_bg_blesi__()
        {
            AssertCode("bg.blesi?\tr10,-0x1,001000B1", "D1 5F 05 88");
        }

        [Test]
        public void AeonDis_bn_blesi__2()
        {
            AssertCode("bn.blesi?\tr23,0x0,00100017", "26 E0 5C");
        }

        [Test]
        public void AeonDis_bn_blesi__()
        {
            AssertCode("bn.blesi??\tr3,0x0,000FFFCD", "24 63 36");
        }

        [Test]
        public void AeonDis_bg_bleu__()
        {
            AssertCode("bg.bleu?\tr7,r5,000FFFE6", "D4 E5 FF 30");
        }

        [Test]
        public void AeonDis_bg_bltsi__()
        {
            AssertCode("bg.bltsi?\tr3,0x6,0010003E", "D0 66 01 F1");
        }

        [Test]
        public void AeonDis_bg_bltui__()
        {
            // seen in comparing u16 return value of a procedure.
            AssertCode("bg.bltui?\tr4,0x8,000FFFE7", "D0 88 FF 3E");
        }

        [Test]
        public void AeonDis_bg_bne__()
        {
            AssertCode("bg.bne?\tr6,r7,0010000D", "D4 C7 00 6E");
        }

        [Test]
        public void AeonDis_bn_bnei__()
        {
            AssertCode("bn.bnei?\tr6,0x0,00100017", "20 C0 5E");
        }

        [Test]
        public void AeonDis_bn_bnf__()
        {
            AssertCode("bn.bnf?\t001004C2", "20 13 0B");
        }

        [Test]
        public void AeonDis_bn_cmov____()
        {
            // speculative guess.
            AssertCode("bn.cmov??\tr7,r7,r0", "48 E7 00");
        }

        [Test]
        public void AeonDis_bn_cmovsi__negative()
        {
            // speculative guess.
            AssertCode("bn.cmovsi?\tr6,r0,-0x1", "48 C0 FA");
        }

        [Test]
        public void AeonDis_bn_cmovsi__positive()
        {
            // speculative guess.
            AssertCode("bn.cmovsi?\tr12,r12,0x1", "49 8C 0A");
        }

        [Test]
        public void AeonDis_bn_cmovii__()
        {
            // really looks like a cmov of two signed constants
            AssertCode("bn.cmovi??\tr7,0x1,-0x1", "48 E1 FB");
        }

        [Test]
        public void AeonDis_bn_divs()
        {
            //$REVIEW: this might be bn.divu 
            AssertCode("bn.divs?\tr7,r7,r6", "40 E7 30");
        }

        [Test]
        public void AeonDis_bn_divu()
        {
            // confirmed with source
            AssertCode("bn.divu\tr6,r6,r7", "40 C6 39");
        }

        [Test]
        public void AeonDis_bn_entri__()
        {
            AssertCode("bn.entri?\t0x1,0x3", "5C 40 78");
        }

        [Test]
        public void AeonDis_bn_extbz__()
        {
            AssertCode("bn.extbz?\tr3,r4", "5C 64 00");
        }

        [Test]
        public void AeonDis_bn_exths__()
        {
            AssertCode("bn.exths?\tr27,r7", "5F 67 06");
        }

        [Test]
        public void AeonDis_bn_extbs__()
        {
            AssertCode("bn.extbs?\tr5,r5", "5C A5 02");
        }

        [Test]
        public void AeonDis_bn_exthz__()
        {
            AssertCode("bn.exthz?\tr10,r7", "5D 47 04");
        }

        [Test]
        public void AeonDis_bn_ff1__()
        {
            AssertCode("bn.ff1?\tr6,r7", "5C C7 08");
        }

        [Test]
        public void AeonDis_bg_flush_invalidate()
        {
            // confirmed with source (mostly)
            AssertCode("bg.flush.invalidate\t(r3)", "F4 03 00 04");
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
            AssertCode("bg.invalidate.line\t(r3),0x1", "F4 03 00 17");
        }

        [Test]
        public void AeonDis_bt_j()
        {
            // confirmed with source
            AssertCode("bt.j\t000FFFF4", "93 F4");
        }

        [Test]
        public void AeonDis_bn_j____()
        {
            // Found in switch statements
            AssertCode("bn.j??\t000FF17E", "2F F1 7E");
        }

        [Test]
        public void AeonDis_bn_jal__()
        {
            AssertCode("bn.jal?\t000FFF2A", "2B FF 2A");
        }

        [Test]
        public void AeonDis_bg_j()
        {
            AssertCode("bg.j\t000FFFF3", "E7 FF FF E7");
        }

        [Test]
        public void AeonDis_bg_jal()
        {
            AssertCode("bg.jal\t000F17F9", "E7 FE 2F F2");
        }

        [Test]
        public void AeonDis_bt_jalr__()
        {
            AssertCode("bt.jalr?\tr7", "84 E8");
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
        public void AeonDis_bn_rtnei__()
        {
            AssertCode("bn.rtnei?\t0x1,0x3", "5C 40 7C");
        }

        [Test]
        public void AeonDis_bg_lbs__()
        {
            AssertCode("bg.lbs?\tr17,-0x3900(r3)", "F6 23 C7 00");
        }

        [Test]
        public void AeonDis_bg_lbz__()
        {
            AssertCode("bg.lbz?\tr6,0x1EEC(r7)", "F0 C7 1E EC");
        }

        [Test]
        public void AeonDis_bn_lbz()
        {
            AssertCode("bn.lbz?\tr3,(r4)", "10 64 00");
        }

        [Test]
        public void AeonDis_bg_lhs()
        {
            AssertCode("bg.lhs?\tr7,-0x5A7E(r6)", "E8 E6 A5 82");
        }

        [Test]
        public void AeonDis_bn_lhs()
        {
            AssertCode("bn.lhs\tr7,0x4(r3)");
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
        public void AeonDis_bt_lwst____()
        {
            AssertCode("bt.lwst??\tr4,0x14(r1)", "80 8B");
        }

        [Test]
        public void AeonDis_bn_lwz()
        {
            // confirmed with source
            AssertCode("bn.lwz\tr6,0x18(r1)", "0C C1 1A");
        }

        [Test]
        public void AeonDis_bn_lwz_0()
        {
            AssertCode("bn.lwz\tr30,(r21)", "0F D5 02");
        }

        [Test]
        public void AeonDis_bg_lwz()
        {
            // confirmed with source
            AssertCode("bg.lwz\tr4,0x90(r1)", "EC 81 00 92");
        }

        [Test]
        public void AeonDis_bg_lwz_0()
        {
            AssertCode("bg.lwz\tr29,(r16)", "EF B0 00 02");
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
        public void AeonDis_bg_movhi()
        {
            // confirmed with source
            AssertCode("bg.movhi\tr7,0xA020", "C0 F4 04 01");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_load()
        {
            AssertCode(
                "bg.movhi\tr6,0x523A3C@hi",
                "bg.lwz\tr7,0x523A3C@lo(r6)",
                "C0 C0 0A 41" +
                "EC E6 3A 3E");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_bg_lbs__()
        {
            AssertCode(
                "bg.movhi\tr7,0x550A4B@hi",
                "bg.lbs?\tr7,0x550A4B@lo(r7)",
                "C0 E0 0A A1" +
                "F4 E7 0A 4B");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_store()
        {
            AssertCode(
                "bg.movhi\tr7,0x523A05@hi",
                "bg.sb?\t0x523A05@lo(r7),r6",
                "C0 E0 0A 41" +
                "F8 C7 3A 05");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_addi()
        {
            AssertCode(
                "bg.movhi\tr12,0x3988F0@hi",
                "bg.addi\tr10,r12,0x3988F0@lo",
                "C1 80 07 41" +
                "FD 4C 88 F0");
        }

        [Test]
        public void AeonDis_bg_movhi_fuse_with_ori()
        {
            AssertCode(
                "bg.movhi\tr6,0x7FFFFFFF@hi",
                "bg.ori\tr6,r6,0x7FFFFFFF@lo",
                "C0 CF FF E1" +
                "C8 C6 FF FF");
        }

        [Test]
        public void AeonDis_bt_movhi_fuse_with_ori()
        {
            AssertCode(
                "bt.movhi?\tr10,0x103126@hi",
                "bg.ori\tr3,r10,0x103126@lo",
                "95 50" +
                "C8 6A 31 26");
        }

        [Test]
        public void AeonDis_bg_mfspr()
        {
            // confirmed with source
            AssertCode("bg.mfspr\tr3,r0,0x11", "C0 60 01 1F");
        }

        [Test]
        public void AeonDis_bg_mfspr1__()
        {
            AssertCode("bg.mfspr1?\tr30,0x2808", "C3 C5 01 07");
        }

        [Test]
        public void AeonDis_bg_mtspr()
        {
            // confirmed with source
            AssertCode("bg.mtspr\tr0,r3,0x11", "C0 60 01 1D");
        }

        [Test]
        public void AeonDis_bg_mtspr1__()
        {
            AssertCode("bg.mtspr1?\tr7,0x11", "C0 E0 02 25");
        }

        [Test]
        public void AeonDis_bn_mul()
        {
            // confirmed with source
            AssertCode("bn.mul\tr7,r3,r6", "40 E3 33");
        }

        [Test]
        public void AeonDis_bg_muli__()
        {
            AssertCode("bg.muli?\tr3,r4,0x48", "CC 64 00 48");
        }

        [Test]
        public void AeonDis_bn_mulu____()
        {
            AssertCode("bn.mulu??\tr7,r14,r20", "40 EE A2");
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
            AssertCode("bt.nop\t0x0", "80 01");
        }

        [Test]
        public void AeonDis_bn_nop()
        {
            // confirmed with source
            AssertCode("bn.nop", "00 00 00");
        }

        [Test]
        public void AeonDis_bn_or()
        {
            // confirmed with source
            AssertCode("bn.or\tr3,r0,r1", "44 60 0D");
        }

        [Test]
        public void AeonDis_bn_ori()
        {
            // confirmed with source
            AssertCode("bn.ori\tr5,r0,0x11", "50 A0 11");
        }

        [Test]
        public void AeonDis_bg_ori()
        {
            // confirmed with source
            AssertCode("bg.ori\tr5,r7,0x2400", "C8 A7 24 00");
        }

        [Test]
        public void AeonDis_bt_rfe()
        {
            // confirmed with source
            AssertCode("bt.rfe", "84 00");
        }

        [Test]
        public void AeonDis_bn_ror__()
        {
            AssertCode("bn.ror?\tr5,r5,r7", "4C A5 3F");
        }

        [Test]
        public void AeonDis_bn_rori__()
        {
            AssertCode("bn.rori?\tr10,r4,0x19", "4D 44 CB");
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
        public void AeonDis_bn_sfeq__()
        {
            AssertCode("bn.sfeq?\tr6,r7", "5C C7 05");
        }

        [Test]
        public void AeonDis_bn_sfeqi()
        {
            // confirmed with source
            AssertCode("bn.sfeqi\tr3,0x0", "5C 60 01");
        }

        [Test]
        public void AeonDis_bn_sfges__()
        {
            AssertCode("bn.sfges?\tr11,r7", "5D 67 15");
        }

        [Test]
        public void AeonDis_bg_sfgesi__()
        {
            AssertCode("bg.sfgesi?\tr10,0x85", "C1 40 10 AC");
        }

        [Test]
        public void AeonDis_bn_sfgesi__()
        {
            AssertCode("bn.sfgesi?\tr10,-0x1", "5D 5F F9");
        }

        [Test]
        public void AeonDis_bn_sfgeu()
        {
            // confirmed with source
            AssertCode("bn.sfgeu\tr3,r4", "5C 83 17");
        }

        [Test]
        public void AeonDis_bn_sfgtu()
        {
            // confirmed with source
            // in source: bn.sfltu r3,r4
            AssertCode("bn.sfgtu\tr4,r3", "5C 83 1F");
        }

        [Test]
        public void AeonDis_bg_sfgeui__()
        {
            AssertCode("bg.sfgeui?\tr4,0xFF" , "C0 80 1F E0");
        }

        [Test]
        public void AeonDis_bg_sfgtui__()
        {
            // Found as a guard statement to a switch statement
            AssertCode("bg.sfgtui?\tr3,0x8C", "C0 60 11 8E");
        }

        [Test]
        public void AeonDis_bg_sflesi__()
        {
            AssertCode("bg.sflesi?\tr3,0x700", "C0 60 E0 08");
        }

        [Test]
        public void AeonDis_bn_sflesi__()
        {
            AssertCode("bn.sflesi?\tr6,-0x1", "5C DF F1");
        }

        [Test]
        public void AeonDis_bg_sfleui__()
        {
            AssertCode("bg.sfleui?\tr7,0xC8", "C0 E0 19 0A");
        }

        [Test]
        public void AeonDis_bn_sfleui__()
        {
            AssertCode("bn.sfleui?\tr3,0x77", "5C 6E F3");
        }

        [Test]
        public void AeonDis_bn_sflts__()
        {
            AssertCode("bn.sflts?\tr6,r7", "5C C7 1D");
        }

        [Test]
        public void AeonDis_bn_lbs__()
        {
            AssertCode("bn.lbs?\tr15,0x3(r3)", "15 E3 03");
        }

        [Test]
        public void AeonDis_bn_sfne()
        {
            // confirmed with source
            AssertCode("bn.sfne\tr3,r4", "5C 64 0D");
        }

        [Test]
        public void AeonDis_bn_sfnei__()
        {
            AssertCode("bn.sfnei?\tr7,0x3A", "5C E7 49");
        }

        [Test]
        public void AeonDis_bg_sfnei__()
        {
            AssertCode("bg.sfnei?\tr6,0x3E8", "C0 C0 7D 04");
        }

        [Test]
        public void AeonDis_bg_sh()
        {
            AssertCode("bg.sh\t0x345A(r7),r3", "EC 67 34 5B");
        }

        [Test]
        public void AeonDis_bn_sll__()
        {
            AssertCode("bn.sll?\tr6,r3,r18", "4C C3 94");
        }

        [Test]
        public void AeonDis_bn_slli__()
        {
            AssertCode("bn.slli?\tr7,r10,0x3", "4C EA 18");
        }

        [Test]
        public void AeonDis_bn_sra__()
        {
            AssertCode("bn.sra?\tr7,r11,r27", "4C EB DE");
        }

        [Test]
        public void AeonDis_bn_srai__()
        {
            AssertCode("bn.srai?\tr5,r7,0x1", "4C A7 0A");
        }

        [Test]
        public void AeonDis_bn_srl__()
        {
            AssertCode("bn.srl?\tr6,r15,r5", "4C CF 2D");
        }

        [Test]
        public void AeonDis_bn_srli__()
        {
            AssertCode("bn.srli?\tr7,r24,0x3", "4C F8 19");
        }

        [Test]
        public void AeonDis_bn_sub()
        {
            // confirmed with source
            AssertCode("bn.sub\tr6,r6,r5", "40 C6 2D");
        }

        [Test]
        public void AeonDis_bn_subb__()
        {
            // Always found right after a sub
            AssertCode("bn.subb?\tr13,r13,r6", "41 AD 36");
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
        public void AeonDis_bt_swst____()
        {
            AssertCode("bt.swst??\t0x1C(r1),r15", "81 EE");
        }

        [Test]
        public void AeonDis_bg_syncwritebuffer()
        {
            // confirmed with source
            AssertCode("bg.syncwritebuffer", "F4 00 00 05");
        }

        [Test]
        public void AeonDis_bn_xor__()
        {
            AssertCode("bn.xor?\tr7,r4,r3", "44 E4 1E");
        }

        [Test]
        public void AeonDis_bg_xori__()
        {
            AssertCode("bg.xori?\tr4,r4,0x8", "D8 84 00 08");
        }

    }
}
