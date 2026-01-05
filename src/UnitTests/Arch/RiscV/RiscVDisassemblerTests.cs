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
using Reko.Arch.RiscV;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVDisassemblerTests : DisassemblerTestBase<RiscVInstruction>
    {
        private RiscVArchitecture arch;
        private Address addrLoad;

        public RiscVDisassemblerTests()
        {
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new RiscVArchitecture(
                CreateServiceContainer(),
                "riscV",
                new Dictionary<string, object>
                {
                    { ProcessorOption.WordSize, "64" },
                    { ProcessorOption.FloatABI, 64 }
                });
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        private void Given_32bit()
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize, "32" },
                { ProcessorOption.FloatABI, 32 }
            });
        }

        private void Given_64bit()
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize, "64" },
                { ProcessorOption.FloatABI, 64 }
            });
        }

        private void Given_128bit()
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize, "128" },
                { ProcessorOption.FloatABI, 128 }
            });
        }

        private void Given_ZfaExtension(int floatSize)
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.FloatABI, floatSize },
                { "Zfa", true }
            });
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
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
        public void RiscV_dasm_beq()
        {
            AssertCode("beq\ta1,a5,0x00100000", 0x00F58063u);
        }

        [Test]
        public void RiscV_dasm_lui()
        {
            AssertCode("lui\tt6,0x12345", 0b00010010001101000101_11111_01101_11);
        }

        [Test]
        public void RiscV_dasm_lui_addi()
        {
            AssertCode(
                "lui\tt6,pcrel_hi(0x12345678)",
                "addi\tt6,t6,pcrel_lo(0x12345678)",
                "B75F3412 938F 8F67");
        }

        [Test]
        public void RiscV_dasm_sh()
        {
            AssertCode("sh\ts5,0x182(sp)", 0b0001100_10101_00010_001_00010_01000_11);
        }

        [Test]
        public void RiscV_dasm_lb()
        {
            AssertCode("lb\tgp,-0x790(sp)", 0b100001110000_00010_000_00011_00000_11);
        }

        [Test]
        public void RiscV_dasm_lb_offset_zero()
        {
            AssertCode("lb\tgp,(sp)", 0b000000000000_00010_000_00011_00000_11);
        }

        [Test]
        public void RiscV_dasm_addi()
        {
            AssertCode("addi\tsp,sp,-0x1C0", 0b1110010000000001000000010_00100_11);
        }

        [Test]
        public void RiscV_dasm_addi_li()
        {
            AssertCode("li\ta0,-0x1C0", 0b111001000000_00000_000_01010_00100_11);
        }

        [Test]
        public void RiscV_dasm_addi_mv()
        {
            AssertCode("mv\ta0,s8", 0b000000000000_11000_000_01010_00100_11);
        }

        [Test]
        public void RiscV_dasm_auipc()
        {
            AssertCode("auipc\tgp,0xFFFFD", 0b11111111111111111_101_00011_00101_11);
        }

        [Test]
        public void RiscVDis_auipc_addi()
        {
            AssertCode(
                "auipc\ts1,pcrel_hi(0x000FF9A6)",
                "addi\ts1,s1,pcrel_lo(0x000FF9A6)",
                "97040000 9384649A");
            //0497 0000       auipc s1,0x0!!!8493 0A64 addi    s1,s1,0xA6

        }

        [Test]
        public void RiscV_dasm_auipc_sb()
        {
            AssertCode(
                "auipc\ta4,pcrel_hi(0x00102402)",
                "sb\ta5,pcrel_lo(0x00102402)",
                "1727 0000 2301 F740");
            ///2717 0000     	auipc a4,0x2 !!!0123 40F7     	sb a5,0x402(a4)
        }
        [Test]
        public void RiscV_dasm_jal_ra()
        {
            AssertCode("jal\t0x000FF1F4", 0x9F4FF0EF);
        }

        [Test]
        public void RiscV_dasm_jal_zero()
        {
            AssertCode("j\t0x000FF1F4", 0x9F4FF06F);
        }

        [Test]
        public void RiscV_dasm_jal()
        {
            AssertCode("jal\ttp,0x000FF1F4", 0x9F4FF26F);
        }

        [Test]
        public void RiscV_dasm_sd()
        {
            AssertCode("sd\ts5,0x188(sp)", 0x19513423u);
        }

        [Test]
        public void RiscV_dasm_addiw()
        {
            AssertCode("addiw\ta5,a5,0x8", 0x0087879Bu);
        }

        [Test]
        public void RiscV_dasm_addiw_negative()
        {
            AssertCode("addiw\ta5,a5,-0x8", 0xFF87879Bu);
        }

        [Test]
        public void RiscV_dasm_x1()
        {
            AssertCode("beq\ta0,a4,0x00100128", 0x12E50463u);
        }

        [Test]
        public void RiscV_dasm_jalr()
        {
            AssertCode("jalr\tzero,a5,0x0", 0x00078067u);
        }

        [Test]
        public void Riscv_dasm_lr_w()
        {
            AssertCode("lr.w.aqrl\ttp,(s0)", "2F222416");
        }

        [Test]
        public void RiscV_dasm_or()
        {
            AssertCode("or\ts0,s0,s8", 0x01846433u);
        }

        [Test]
        public void RiscV_dasm_add()
        {
            AssertCode("add\ta5,a5,a4", 0x00E787B3u);
        }

        [Test]
        public void RiscV_dasm_and()
        {
            AssertCode("and\ta5,s0,a5", 0x00F477B3u);
        }

        [Test]
        public void RiscV_dasm_c_and()
        {
            AssertCode("c.and\ta5,a3", 0x8FF5);
        }

        [Test]
        public void RiscV_dasm_c_ebreak()
        {
            AssertCode("c.ebreak", "0290");
        }

        [Test]
        public void RiscV_dasm_c_j()
        {
            AssertCode("c.j\t0x000FFFE4", 0x0000B7D5);
        }

        [Test]
        public void RiscV_dasm_c_j_2()
        {
            AssertCode("c.j\t0x000FFFE8", "E5B7");
        }

        [Test]
        public void RiscV_dasm_c_or()
        {
            AssertCode("c.or\ta2,a3", 0x8E55);
        }

        [Test]
        public void RiscV_dasm_c_nop()
        {
            AssertCode("c.nop", "0100");
        }

        [Test]
        public void RiscV_dasm_c_sd()
        {
            AssertCode("c.sd\ts0,0x38(s0)", "00FC");
        }

        [Test]
        public void RiscV_dasm_c_fld()
        {
            AssertCode("c.fld\tfs1,0xD8(a2)", 0x00002E64);
        }

        [Test]
        public void RiscV_dasm_c_fsw_32()
        {
            Given_32bit();
            AssertCode("c.fsw\ts0,0x70(s1)", "A0F8");
        }

        [Test]
        public void RiscV_dasm_csrrs()
        {
            AssertCode("csrrs\tt5,mtvec,a1", "73AF5530");
        }

        [Test]
        public void RiscV_dasm_csrrc()
        {
            AssertCode("csrrc\tt5,mtvec,a1", "73BF5530");
        }

        [Test]
        public void RiscV_dasm_csrrwi()
        {
            AssertCode("csrrwi\tt5,mtvec,0xB", "73 df 55 30");
        }

        [Test]
        public void RiscV_dasm_csrrw()
        {
            AssertCode("csrrw\tt5,mtvec,a1", "739F5530");
        }

        [Test]
        public void RiscV_dasm_csrrw_unknown()
        {
            AssertCode("csrrw\tzero,0xBFC,a0", "7310C5BF");
        }

        [Test]
        public void RiscV_dasm_csrrsi()
        {
            AssertCode("csrrsi\tt5,mtvec,0xB", "73 ef 55 30");
        }

        [Test]
        public void RiscV_dasm_csrrci()
        {
            AssertCode("csrrci\tt5,mtvec,0xB", "73 ff 55 30");
        }

        [Test]
        public void RiscV_dasm_sraw()
        {
            AssertCode("sraw\ts7,a0,t4", "BB 5B D5 41");
        }

        [Test]
        public void RiscV_dasm_subw()
        {
            AssertCode("subw\ta3,a3,a5", 0x40F686BBu);
        }

        [Test]
        public void RiscV_dasm_srliw()
        {
            AssertCode("srliw\ta4,a5,0x1", 0x0017D71Bu);
        }

        [Test]
        public void RiscV_dasm_lbu()
        {
            AssertCode("lbu\ta4,(s2)", 0x00094703u);
        }

        [Test]
        public void RiscV_dasm_fence_i()
        {
            AssertCode("fence.i", "0F10 0000");
        }

        [Test]
        public void RiscV_dasm_fence()
        {
            AssertCode("fence\tiorw,iorw", "0F00 F00F");
        }

        [Test]
        public void RiscV_dasm_fli_d()
        {
            Given_ZfaExtension(64);
            AssertCode("fli.d\tfa5,0.3125", 0xF21487D3u);    // fli.s\tfa5,0.3125
        }

        [Test]
        public void RiscV_dasm_fli_s()
        {
            Given_ZfaExtension(64);
            AssertCode("fli.s\tfa5,0.4375", 0xF01587D3u);    // fli.s\tfa5,0.3125
        }

        [Test]
        public void RiscV_dasm_fli_s_inf()
        {
            Given_ZfaExtension(64);
            AssertCode("fli.s\tfa5,inf", 0xF01F07D3u);    // fli.s\tfa5,0.3125
        }

        [Test]
        public void RiscV_dasm_flw()
        {
            AssertCode("flw\tfa4,0x34(s2)", 0x03492707u);
        }

        [Test]
        public void RiscV_dasm_fmaxm_d()
        {
            AssertCode("fmaxm.d\tft2,ft0,ft4", "5331402A");
        }

        [Test]
        public void RiscV_dasm_fminm_s()
        {
            AssertCode("fminm.s\tft6,ft0,ft1", "53231028");
        }

        [Test]
        public void RiscV_dasm_fmv_d_x()
        {
            AssertCode("fmv.d.x\tfa4,a4", 0xF2070753u);
        }

        [Test]
        public void RiscV_dasm_fmv_w_x()
        {
            AssertCode("fmv.w.x\tfa5,zero", 0xF00007D3u);
        }

        [Test]
        public void RiscV_dasm_fmv_x_d()
        {
            AssertCode("fmv.x.d\ta2,ft3", "538601E2");
        }

        [Test]
        public void RiscV_dasm_lwu()
        {
            AssertCode("lwu\ta4,0x4(s0)", 0x00446703u);
        }

        [Test]
        public void RiscV_dasm_fclass_d()
        {
            AssertCode("fclass.d\ta0,fa3", "539506e2"); //  fmv.d.x fa0,a3
        }

        [Test]
        public void RiscV_dasm_fclass_s()
        {
            AssertCode("fclass.s\ta3,fa2", "d31606e0"); //        fmv.x.w a3,fa2
        }

        [Test]
        public void RiscV_dasm_fcvt_d_s()
        {
            AssertCode("fcvt.d.s\tfa4,fa4", 0x42070753u);
        }

        [Test]
        public void RiscV_dasm_feq_s()
        {
            // 1010000 011110111001001111 10100 11
            AssertCode("feq.s\ta5,fa4,fa5", 0xA0F727D3u);
        }

        [Test]
        public void RiscV_dasm_fmadd_d()
        {
            AssertCode("fmadd.d\tfs6,ft10,ft5,fa0,rmm", "434b5f52"); //  fmadd.d fs6,ft10,ft5,fa0
        }

        [Test]
        public void RiscV_dasm_fmadd_s()
        {
            AssertCode("fmadd.s\tfs10,ft7,fs1,fa6,dyn", 0x8093FD43);
        }

        [Test]
        public void RiscV_dasm_fround_s()
        {
            AssertCode("fround.s\tfa4,fa4,rne", 0x40470753u);
        }

        [Test]
        public void RiscV_dasm_c_addiw_negative()
        {
            AssertCode("c.addiw\ts0,-0x1", 0x0000347D);
        }

        [Test]
        public void RiscV_dasm_c_sw()
        {
            AssertCode("c.sw\ta5,(a3)", 0xC29C);
        }

        [Test]
        public void RiscV_dasm_c_sdsp()
        {
            AssertCode("c.sdsp\ts3,0x48(sp)", 0xE4CE);
        }

        [Test]
        public void RiscV_dasm_c_beqz()
        {
            AssertCode("c.beqz\ta0,0x00100040", 0x0000C121);
        }

        [Test]
        public void RiscV_dasm_c_lui()
        {
            AssertCode("c.lui\ta1,0x1000", 0x00006585);
        }

        [Test]
        public void RiscV_dasm_mret()
        {
            AssertCode("mret", 0x30200073);
        }

        [Test]
        public void RiscV_dasm_negative_3()
        {
            AssertCode("c.addiw\ts1,-0x3", 0x34F5);
        }

        [Test]
        public void RiscV_dasm_c_ld()
        {
            AssertCode("c.ld\ta0,0xC8(a0)", 0x00006568);
        }

        [Test]
        public void RiscV_dasm_c_bnez()
        {
            AssertCode("c.bnez\ta4,0x0010001A", 0x0000EF09);
        }

        [Test]
        public void RiscV_dasm_pause()
        {
            AssertCode("pause", "0F00 0001");
        }

        [Test]
        public void RiscV_dasm_remuw()
        {
            AssertCode("remuw\ta6,a6,a2", 0x02C8783B);
        }

        [Test]
        public void RiscV_dasm_c_li()
        {
            AssertCode("c.li\ta0,0x8", 0x00004521);
        }

        [Test]
        public void RiscV_dasm_c_swsp()
        {
            AssertCode("c.swsp\ta0,0x4(sp)", 0xC22A);
        }

        [Test]
        public void RiscV_dasm_c_li_minus3()
        {
            AssertCode("c.li\ta4,-0x3", 0x00005775);
        }

        [Test]
        public void RiscV_dasm_c_lwsp()
        {
            AssertCode("c.lwsp\ta0,0x4(sp)", 0x00004512);
        }

        [Test]
        public void RiscV_dasm_c_lwsp_regression()
        {
            //$NOTE: @gregoral has an incoming fix for this.
            AssertCode("c.lwsp\tra,0xD4(sp)", 0x40DEu);
        }

        [Test]
        public void RiscV_dasm_c_mv()
        {
            AssertCode("c.mv\ts0,s3", 0x844E);
        }

        [Test]
        public void RiscV_dasm_c_lw()
        {
            AssertCode("c.lw\ta3,0x44(a5)", 0x000043F4);
        }

        [Test]
        public void RiscV_dasm_divw()
        {
            AssertCode("divw\ts0,s0,a1", 0x02B4443B);
        }

        [Test]
        public void RiscV_dasm_c_addi16sp()
        {
            AssertCode("c.addi16sp\tsp,0xD0", 0x6169);
        }

        [Test]
        public void RiscV_dasm_beqz_backward()
        {
            AssertCode("c.beqz\ta5,0x000FFF06", 0xD399);
        }

        [Test]
        public void RiscV_dasm_addiw_sign_extend()
        {
            AssertCode("c.addiw\tt1,0x0", 0x00002301);
        }

        [Test]
        public void RiscV_dasm_li()
        {
            AssertCode("c.li\tt2,0x1", 0x00004385);
        }

        [Test]
        public void RiscV_dasm_beqz_0000C3F1()
        {
            AssertCode("c.beqz\ta5,0x001000C4", 0x0000C3F1);
        }

        [Test]
        public void RiscV_dasm_c_bnez_backward()
        {
            AssertCode("c.bnez\ta4,0x000FFF30", 0xFB05);
        }

        [Test]
        public void RiscV_dasm_c_addiw()
        {
            AssertCode("c.addiw\ts0,0x1", 0x00002405);
        }

        [Test]
        public void RiscV_dasm_c_fldsp()
        {
            AssertCode("c.fldsp\tfs0,0x168(sp)", 0x00003436);
        }

        [Test]
        public void RiscV_dasm_invalid()
        {
            AssertCode("invalid", 0x00000000);
        }

        [Test]
        public void RiscV_dasm_jr_ra()
        {
            AssertCode("c.jr\tra", 0x00008082);
        }

        [Test]
        public void RiscV_dasm_c_sub()
        {
            AssertCode("c.sub\ta1,a0", 0x8D89);
        }

        [Test]
        public void RiscV_dasm_c_j_backward()
        {
            AssertCode("c.j\t0x000FFF36", 0x0000BF1D);
        }

        [Test]
        public void RiscV_dasm_c_addi4spn()
        {
            AssertCode("c.addi4spn\ta5,sp,0x20", 0x0000101C);
        }

        [Test]
        public void RiscV_dasm_c_jr()
        {
            AssertCode("c.jr\ta5", 0x00008782);
        }

        [Test]
        public void RiscV_dasm_c_subw()
        {
            AssertCode("c.subw\ta0,a5", 0x00009D1D);
        }

        [Test]
        public void RiscV_dasm_c_addi()
        {
            AssertCode("c.addi\ta5,0x1", 0x00000785);
        }

        [Test]
        public void RiscV_dasm_c_addw()
        {
            AssertCode("c.addw\ta5,a3", 0x00009FB5);
        }

        [Test]
        public void RiscV_dasm_c_srli()
        {
            AssertCode("c.srli\ta5,0xA", 0x000083A9);
        }

        [Test]
        public void RiscV_dasm_c_srli64()
        {
            AssertCode("c.srli64\ta2", 0x00008201);
        }

        [Test]
        public void RiscV_dasm_c_srai()
        {
            AssertCode("c.srai\ta4,0x3F", 0x0000977D);
        }

        [Test]
        public void RiscV_dasm_c_srai64()
        {
            AssertCode("c.srai64\ta3", 0x00008681);
        }

        [Test]
        public void RiscV_dasm_c_andi()
        {
            AssertCode("c.andi\ta2,0x18", 0x00008A61);
        }

        [Test]
        public void RiscV_dasm_c_ldsp()
        {
            AssertCode("c.ldsp\ts7,0x8(sp)", 0x00006BA2);
        }

        [Test]
        public void RiscV_dasm_c_slli()
        {
            AssertCode("c.slli\ts0,0x3", 0x0000040E);
        }

        [Test]
        public void RiscV_dasm_c_slli64()
        {
            AssertCode("c.slli64\ta5", 0x00000782);
        }

        [Test]
        public void RiscV_dasm_sll()
        {
            AssertCode("sll\ta5,s6,s0", 0x008B17B3);
        }

        [Test]
        public void RiscV_dasm_slli()
        {
            AssertCode("slli\ta2,s2,0x20", 0x02091613);
        }

        [Test]
        public void RiscV_dasm_sltu()
        {
            AssertCode("sltu\ta0,zero,a0", 0x00A03533);
        }

        [Test]
        public void RiscV_dasm_slt()
        {
            AssertCode("slt\ta0,a5,a0", 0x00A7A533);
        }

        [Test]
        public void RiscV_dasm_remw()
        {
            AssertCode("remw\ta3,a5,a3", 0x02D7E6BB);
        }

        [Test]
        public void RiscV_dasm_fmsub_d()
        {
            AssertCode("fmsub.s\tfa1,fa7,fa7,fa2,rup", 0x6118B5C7);
        }

        [Test]
        public void RiscV_dasm_fmsub_s()
        {
            AssertCode("fmsub.s\tfa1,fa7,fa7,fa2,rup", 0x6118B5C7);
        }

        [Test]
        public void RiscV_dasm_fnmsub_q()
        {
            Given_128bit();
            AssertCode("fnmsub.q\tft0,fs2,fs8,fs0,rne", 0x4789004B);
        }

        [Test]
        public void RiscV_dasm_fnmsub_s()
        {
            AssertCode("fnmsub.s\tft0,fs2,fs8,fs0,rne", 0x4189004B);
        }

        [Test]
        public void RiscV_dasm_fnmadd_s()
        {
            AssertCode("fnmadd.s\tfs11,ft7,fa1,ft0,dyn", 0x00B3FDCF);
        }

        [Test]
        public void RiscV_dasm_fnmadd_h()
        {
            AssertCode("fnmadd.h\tfs11,ft7,fa1,ft0,dyn", 0x04B3FDCF);
        }

        [Test]
        public void RiscV_dasm_divuw()
        {
            AssertCode("divuw\ta5,a6,a2", 0x02C857BB);
        }

        [Test]
        public void RiscV_dasm_c_fsd()
        {
            AssertCode("c.fsd\tfs1,0x8(a2)", 0x0000A604);
        }

        [Test]
        public void RiscV_dasm_c_fsdsp()
        {
            AssertCode("c.fsdsp\tfs9,0x1C8(sp)", 0xA7E6);
        }

        [Test]
        public void RiscV_dasm_wfi()
        {
            //10500073
            AssertCode("wfi", "73005010");
        }

        [Test]
        public void RiscV_dasm_srlw()
        {
            AssertCode("srlw\ta0,a3,a4", "3BD5E600");
        }

        [Test]
        public void RiscV_dasm_sllw()
        {
            AssertCode("sllw\ta1,a1,a4", "BB95E500");
        }

        [Test]
        public void RiscV_dasm_sret()
        {
            AssertCode("sret", 0b0001000_00010_00000_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_sfence_vm()
        {
            AssertCode("sfence.vm\ta3", 0b0001000_00100_01101_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_sfence_vma()
        {
            AssertCode("sfence.vma\ts11,s5", 0b0001001_10101_11011_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_sfence_w_inval()
        {
            AssertCode("sfence.w.inval", 0b0001100_00000_00000_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_sfence_inval_ir()
        {
            AssertCode("sfence.inval.ir", 0b0001100_00001_00000_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hfence_gvma()
        {
            AssertCode("hfence.gvma\ta7,s5", 0b0110001_10101_10001_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hfence_vvma()
        {
            AssertCode("hfence.vvma\ta7,s5", 0b0010001_10101_10001_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hinval_gvma()
        {
            AssertCode("hinval.gvma\ta7,s5", 0b0110011_10101_10001_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hinval_vvma()
        {
            AssertCode("hinval.vvma\ta7,s5", 0b0010011_10101_10001_000_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_b()
        {
            AssertCode("hlv.b\ts11,(a7)", 0b0110000_00000_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_bu()
        {
            AssertCode("hlv.bu\ts11,(a7)", 0b0110000_00001_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_h()
        {
            AssertCode("hlv.h\ts11,(a7)", 0b0110010_00000_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_hu()
        {
            AssertCode("hlv.hu\ts11,(a7)", 0b0110010_00001_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlvx_hu()
        {
            AssertCode("hlvx.hu\ts11,(a7)", 0b0110010_00011_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_w()
        {
            AssertCode("hlv.w\ts11,(a7)", 0b0110100_00000_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlvx_wu()
        {
            AssertCode("hlvx.wu\ts11,(a7)", 0b0110100_00011_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hsv_b()
        {
            AssertCode("hsv.b\ts5,(a7)", 0b0110001_10101_10001_100_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hsv_d()
        {
            AssertCode("hsv.d\ts5,(a7)", 0b0110111_10101_10001_100_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hsv_h()
        {
            AssertCode("hsv.h\ts5,(a7)", 0b0110011_10101_10001_100_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hsv_w()
        {
            AssertCode("hsv.w\ts5,(a7)", 0b0110101_10101_10001_100_00000_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_wu()
        {
            AssertCode("hlv.wu\ts11,(a7)", 0b0110100_00001_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_hlv_d()
        {
            AssertCode("hlv.d\ts11,(a7)", 0b0110110_00000_10001_100_11011_1110011);
        }

        [Test]
        public void RiscV_dasm_regressions1()
        {
            AssertCode("fence.i", "0f100000"); //    invalid
            AssertCode("csrrw\ts2,0x315,t0", "73995231"); //    invalid
            AssertCode("csrrs\ta1,sstatus,a5", "f3a50710"); //    csrrs a1,sstatus,a5
            AssertCode("csrrc\ttp,0x5F0,s0", "7332045f"); //    invalid
            AssertCode("csrrc\ttp,0x5F0,zero", "7332005f"); //    invalid
            AssertCode("csrrsi\ts2,0x6E6,0x1E", "73696f6e"); //    invalid
            AssertCode("csrrci\ts0,0x6A5,0x1A", "73745d6a"); //    invalid
            AssertCode("mul\ta0,s5,s7", "33857a03"); //    add a0,s5,s7
            AssertCode("mulh\tt5,a7,s1", "339f9802"); //    sll t5,a7,s1
            AssertCode("mulhsu\tt3,sp,a6", "332e0103"); //    slt t3,sp,a6
            AssertCode("mulhu\tt3,sp,a6", "333e0103"); //    sltu t3,sp,a6
            AssertCode("mulhsu\tt3,sp,a6", "332e0103"); //    slt t3,sp,a6
            AssertCode("divu\tt4,t3,t2", "b35e7e02"); //    srl t4,t3,t2
            AssertCode("rem\ta4,a1,a2", "33e7c502"); //    or a4,a1,a2
            AssertCode("remu\ta6,a1,t2", "33f87502"); //    and a6,a1,t2
            AssertCode("fsw\tfs1,0x48(a1)", "27a49504"); //    fsw fs1,288(a1)
            AssertCode("fmadd.s\tfs10,ft10,ft5,fa0,rmm", "434d5f50"); //  fmadd.s fs10,ft10,ft5,fa0
            AssertCode("fmsub.s\tft8,ft10,fs11,ft7,rdn", "472ebf39"); //  fmsub.s ft8,ft10,fs11,ft7
            AssertCode("fnmsub.s\tfs7,fa4,fs3,fs0,rne", "cb0b3741"); //    fnmsub.s fs7,fa4,fs3,fs0
            AssertCode("fnmadd.s\tfa4,ft4,fs5,fs0,rmm", "4f475241"); //  fnmadd.s fa4,ft4,fs5,fs0
            AssertCode("fclass.s\ta3,fa2", "d31606e0"); //        fmv.x.w a3,fa2
            AssertCode("fsd\tfs4,0x138(sp)", "273c4113"); //  fsd fs4,2496(a0)
            AssertCode("fmsub.d\tft0,ft0,fs0,fa6,rne", "47008082"); //  fmsub.s ft0,ft0,fs0,fa6
            AssertCode("fnmsub.d\tfs0,ft4,ft2,fa5,rdn", "4b24227a"); //  fnmsub.s fs0,ft4,ft2,fa5
            AssertCode("fnmadd.d\tfa2,ft10,ft5,fa0,rmm", "4f465f52"); //  fnmadd.s fa2,ft10,ft5,fa0
            AssertCode("fcvt.l.d\tra,ft1,rtz", "d39020c2"); //  fcvt.l.d ra,ft1
            AssertCode("fcvt.lu.d\ta0,fa3,rtz", "539536c2"); //  fcvt.lu.d a0,fa3
        }

        [Test]
        public void RiscVDis_add_uw()
        {
            AssertCode("add.uw\ts3,t2,s4", "BB894309");
        }

        [Test]
        public void RiscVDis_andn()
        {
            AssertCode("andn\ta4,a7,s11", 0b0100000_11011_10001_111_01110_0110011);
        }

        [Test]
        public void RiscVDis_bclr()
        {
            AssertCode("bclr\ta4,a7,s11", 0b0100100_11011_10001_001_01110_0110011);
        }

        [Test]
        public void RiscVDis_bclri32()
        {
            AssertCode("bclri\ts11,a7,0x16", 0b0100100_10110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_bclri64()
        {
            AssertCode("bclri\ts11,a7,0x36", 0b010010_110110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_bext()
        {
            AssertCode("bext\ts11,a7,s2", 0b0100100_10010_10001_101_11011_0110011);
        }

        [Test]
        public void RiscVDis_bexti32()
        {
            AssertCode("bexti\ts11,a7,0x16", 0b0100100_10110_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_bexti64()
        {
            AssertCode("bexti\ts11,a7,0x36", 0b010010_110110_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_binv()
        {
            AssertCode("binv\ts11,a7,s2", 0b0110100_10010_10001_001_11011_0110011);
        }

        [Test]
        public void RiscVDis_binvi32()
        {
            AssertCode("binvi\ts11,a7,0x16", 0b0110100_10110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_binvi64()
        {
            AssertCode("binvi\ts11,a7,0x36", 0b011010_110110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_bset()
        {
            AssertCode("bset\ts11,a7,s2", 0b0010100_10010_10001_001_11011_0110011);
        }

        [Test]
        public void RiscVDis_bseti32()
        {
            AssertCode("bseti\ts11,a7,0x16", 0b0010100_10110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_bseti64()
        {
            AssertCode("bseti\ts11,a7,0x36", 0b001010_110110_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_clmul()
        {
            AssertCode("clmul\ts11,a7,s2", 0b0000101_10010_10001_001_11011_0110011);
        }

        [Test]
        public void RiscVDis_clmulh()
        {
            AssertCode("clmulh\ts11,a7,s2", 0b0000101_10010_10001_011_11011_0110011);
        }

        [Test]
        public void RiscVDis_clmulr()
        {
            AssertCode("clmulr\ts11,a7,s2", 0b0000101_10010_10001_010_11011_0110011);
        }

        [Test]
        public void RiscVDis_clz()
        {
            AssertCode("clz\ts11,a7", 0b011000000000_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_clzw()
        {
            AssertCode("clzw\ts11,a7", 0b011000000000_10001_001_11011_0011011);
        }

        [Test]
        public void RiscVDis_cpop()
        {
            AssertCode("cpop\ts11,a7", 0b011000000010_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_cpopw()
        {
            AssertCode("cpopw\ts11,a7", 0b011000000010_10001_001_11011_0011011);
        }

        [Test]
        public void RiscVDis_ctz()
        {
            AssertCode("ctz\ts11,a7", 0b011000000001_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_ctzw()
        {
            AssertCode("ctzw\ts11,a7", 0b011000000001_10001_001_11011_0011011);
        }

        [Test]
        public void RiscVDis_max()
        {
            AssertCode("max\ts11,a7,s2", 0b0000101_10010_10001_110_11011_0110011);
        }

        [Test]
        public void RiscVDis_maxu()
        {
            AssertCode("maxu\ts11,a7,s2", 0b0000101_10010_10001_111_11011_0110011);
        }

        [Test]
        public void RiscVDis_min()
        {
            AssertCode("min\ts11,a7,s2", 0b0000101_10010_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_minu()
        {
            AssertCode("minu\ts11,a7,s2", 0b0000101_10010_10001_101_11011_0110011);
        }

        [Test]
        public void RiscVDis_orc_b()
        {
            AssertCode("orc.b\ts11,a7", 0b001010000111_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_orn()
        {
            AssertCode("orn\ts11,a7,s2", 0b0100000_10010_10001_110_11011_0110011);
        }

        [Test]
        public void RiscVDis_pack()
        {
            AssertCode("pack\ts11,a7,s2", 0b0000100_10010_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_packh()
        {
            AssertCode("packh\ts11,a7,s2", 0b0000100_10010_10001_111_11011_0110011);
        }

        [Test]
        public void RiscVDis_packw()
        {
            AssertCode("packw\ts11,a7,s2", 0b0000100_10010_10001_100_11011_0111011);
        }

        [Test]
        public void RiscVDis_rev8_32()
        {
            Given_32bit();
            AssertCode("rev8\ts11,a7", 0b011010011000_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_rev8_64()
        {
            Given_64bit();
            AssertCode("rev8\ts11,a7", 0b011010111000_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_rol()
        {
            AssertCode("rol\ts11,a7,s2", 0b0110000_10010_10001_001_11011_0110011);
        }

        [Test]
        public void RiscVDis_rolw()
        {
            AssertCode("rolw\ts11,a7,s2", 0b0110000_10010_10001_001_11011_0111011);
        }

        [Test]
        public void RiscVDis_ror()
        {
            AssertCode("ror\ts11,a7,s2", 0b0110000_10010_10001_101_11011_0110011);
        }

        [Test]
        public void RiscVDis_rori_32()
        {
            AssertCode("rori\ts11,a7,0x16", 0b0110000_10110_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_rori_64()
        {
            AssertCode("rori\ts11,a7,0x36", 0b011000_110110_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_roriw()
        {
            AssertCode("roriw\ts11,a7,0x16", 0b0110000_10110_10001_101_11011_0011011);
        }

        [Test]
        public void RiscVDis_rorw()
        {
            AssertCode("rorw\ts11,a7,s2", 0b0110000_10010_10001_101_11011_0111011);
        }

        [Test]
        public void RiscVDis_sext_b()
        {
            AssertCode("sext.b\ts11,a7", 0b011000000100_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_sext_h()
        {
            AssertCode("sext.h\ts11,a7", 0b011000000101_10001_001_11011_0010011);
        }

        [Test]
        public void RiscVDis_sh1add()
        {
            AssertCode("sh1add\ts11,a7,s2", 0b0010000_10010_10001_010_11011_0110011);
        }

        [Test]
        public void RiscVDis_sh1add_uw()
        {
            AssertCode("sh1add.uw\ts11,a7,s2", 0b0010000_10010_10001_010_11011_0111011);
        }

        [Test]
        public void RiscVDis_sh2add()
        {
            AssertCode("sh2add\ts11,a7,s2", 0b0010000_10010_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_sh2add_uw()
        {
            AssertCode("sh2add.uw\ts11,a7,s2", 0b0010000_10010_10001_100_11011_0111011);
        }

        [Test]
        public void RiscVDis_sh3add()
        {
            AssertCode("sh3add\ts11,a7,s2", 0b0010000_10010_10001_110_11011_0110011);
        }

        [Test]
        public void RiscVDis_sh3add_uw()
        {
            AssertCode("sh3add.uw\ts11,a7,s2", 0b0010000_10010_10001_110_11011_0111011);
        }

        [Test]
        public void RiscVDis_slli_uw()
        {
            AssertCode("slli.uw\ts11,a7,0x16", 0b000010_110110_10001_001_11011_0011011);
        }

        [Test]
        public void RiscVDis_unzip()
        {
            Given_32bit();
            AssertCode("unzip\ts11,a7", 0b000010011111_10001_101_11011_0010011);
        }

        [Test]
        public void RiscVDis_xnor()
        {
            AssertCode("xnor\ts11,a7,s2", 0b0100000_10010_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_xperm_b()
        {
            AssertCode("xperm.b\ts11,a7,s2", 0b0010100_10010_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_xperm_n()
        {
            AssertCode("xperm.n\ts11,a7,s2", 0b0010100_10010_10001_010_11011_0110011);
        }

        [Test]
        public void RiscVDis_zext_h_32()
        {
            Given_32bit();
            AssertCode("zext.h\ts11,a7", 0b000010000000_10001_100_11011_0110011);
        }

        [Test]
        public void RiscVDis_zext_h_64()
        {
            AssertCode("zext.h\ts11,a7", 0b000010000000_10001_100_11011_0111011);
        }

        [Test]
        public void RiscVDis_zip()
        {
            AssertCode("zip\ts11,a7", 0b000010011110_10001_001_11011_0010011);
        }

    }
}
