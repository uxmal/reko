#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.RiscV
{
    [TestFixture]
    public class RiscVDisassemblerTests : DisassemblerTestBase<RiscVInstruction>
    {
        private RiscVArchitecture arch;
        private Address addrLoad;

        public RiscVDisassemblerTests()
        {
            Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new RiscVArchitecture(
                new ServiceContainer(),
                "riscV", 
                new Dictionary<string, object>
                {
                    { ProcessorOption.WordSize, "64" },
                    { "FloatAbi", 64 }
                });
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        private void Given_32bit()
        {
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { ProcessorOption.WordSize, "32" },
                { "FloatAbi", 32 }
            });
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertBitString(string sExp, string bits)
        {
            var i = DisassembleBits(bits);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void RiscV_dasm_beq()
        {
            AssertCode("beq\ta1,a5,00100000", 0x00F58063u);
        }

        [Test]
        public void RiscV_dasm_lui()
        {
            AssertBitString("lui\tt6,00012345", "00010010001101000101 11111 01101 11");
        }

        [Test]
        public void RiscV_dasm_sh()
        {
            AssertBitString("sh\ts5,sp,+00000182", "0001100 10101 00010 001 00010 01000 11");
        }

        [Test]
        public void RiscV_dasm_lb()
        {
            AssertBitString("lb\tgp,sp,-00000790", "100001110000 00010 000 00011 00000 11");
        }

        [Test]
        public void RiscV_dasm_addi()
        {
            AssertBitString("addi\tsp,sp,-000001C0", "1110010000000001000000010 00100 11");
        }

        [Test]
        public void RiscV_dasm_auipc()
        {
            AssertBitString("auipc\tgp,000FFFFD", "11111111111111111 101 00011 00101 11");
        }

        [Test]
        public void RiscV_dasm_jal()
        {
            AssertCode("jal\tzero,000FF1F4", 0x9F4FF06F);
        }

        [Test]
        public void RiscV_dasm_sd()
        {
            AssertCode("sd\ts5,sp,+00000188", 0x19513423u);
        }

        [Test]
        public void RiscV_dasm_addiw()
        {
            AssertCode("addiw\ta5,a5,00000008", 0x0087879Bu);
        }

        [Test]
        public void RiscV_dasm_x1()
        {
            AssertCode("beq\ta0,a4,00100128", 0x12E50463u);
        }

        [Test]
        public void RiscV_dasm_jalr()
        {
            AssertCode("jalr\tzero,a5,+00000000", 0x00078067u);
        }

        [Test]
        public void Riscv_dasm_lr_w()
        {
            AssertCode("lr.w.aq.rl\ttp,s0", "2F222416");
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
            AssertCode("c.j\t000FFFE4", 0x0000B7D5);
        }

        [Test]
        public void RiscV_dasm_c_j_2()
        {
            AssertCode("c.j\t000FFFE8", "E5B7");
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
            AssertCode("c.sd\ts0,56(s0)", "00FC");
        }

        [Test]
        public void RiscV_dasm_c_fld()
        {
            AssertCode("c.fld\tfs1,216(a2)", 0x00002E64);
        }

        [Test]
        public void RiscV_dasm_c_fsw_32()
        {
            Given_32bit();
            AssertCode("c.fsw\ts0,112(s1)", "A0F8");
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
            AssertCode("csrrwi\tt5,mtvec,0000000B", "73 df 55 30");
        }

        [Test]
        public void RiscV_dasm_csrrw()
        {
            AssertCode("csrrw\tt5,mtvec,a1", "739F5530");
        }

        [Test]
        public void RiscV_dasm_csrrw_unknown()
        {
            AssertCode("csrrw\tzero,00000BFC,a0", "7310C5BF");
        }

        [Test]
        public void RiscV_dasm_csrrsi()
        {
            AssertCode("csrrsi\tt5,mtvec,0000000B", "73 ef 55 30");
        }

        [Test]
        public void RiscV_dasm_csrrci()
        {
            AssertCode("csrrci\tt5,mtvec,0000000B", "73 ff 55 30");
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
            AssertCode("srliw\ta4,a5,00000001", 0x0017D71Bu);
        }

        [Test]
        public void RiscV_dasm_lbu()
        {
            AssertCode("lbu\ta4,s2,+00000000", 0x00094703u);
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
        public void RiscV_dasm_flw()
        {
            AssertCode("flw\tfa4,52(s2)", 0x03492707u);
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
            AssertCode("lwu\ta4,s0,+00000004", 0x00446703u);
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
        public void RiscV_dasm_addiw_negative()
        {
            AssertCode("c.addiw\ts0,FFFFFFFFFFFFFFFF", 0x0000347D);
        }

        [Test]
        public void RiscV_dasm_c_sw()
        {
            AssertCode("c.sw\ta5,0(a3)", 0xC29C);
        }

        [Test]
        public void RiscV_dasm_c_sdsp()
        {
            AssertCode("c.sdsp\ts3,00000048", 0xE4CE);
        }

        [Test]
        public void RiscV_dasm_c_beqz()
        {
            AssertCode("c.beqz\ta0,00100040", 0x0000C121);
        }

        [Test]
        public void RiscV_dasm_c_lui()
        {
            AssertCode("c.lui\ta1,00001000", 0x00006585);
        }

        [Test]
        public void RiscV_dasm_mret()
        {
            AssertCode("mret", "73002030");
        }

        [Test]
        public void RiscV_dasm_negative_3()
        {
            AssertCode("c.addiw\ts1,FFFFFFFFFFFFFFFD", 0x34F5);
        }

        [Test]
        public void RiscV_dasm_c_ld()
        {
            AssertCode("c.ld\ta0,200(a0)", 0x00006568);
        }

        [Test]
        public void RiscV_dasm_c_bnez()
        {
            AssertCode("c.bnez\ta4,0010001A", 0x0000EF09);
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
            AssertCode("c.li\ta0,00000008", 0x00004521);
        }

        [Test]
        public void RiscV_dasm_c_swsp()
        {
            AssertCode("c.swsp\ta0,00000004", 0xC22A);
        }

        [Test]
        public void RiscV_dasm_c_li_minus3()
        {
            AssertCode("c.li\ta4,FFFFFFFFFFFFFFFD", 0x00005775);
        }

        [Test]
        public void RiscV_dasm_c_lwsp()
        {
            AssertCode("c.lwsp\ta0,00000004", 0x00004512);
        }

        [Test]
        public void RiscV_dasm_c_mv()
        {
            AssertCode("c.mv\ts0,s3", 0x844E);
        }

        [Test]
        public void RiscV_dasm_c_lw()
        {
            AssertCode("c.lw\ta3,68(a5)", 0x000043F4);
        }

        [Test]
        public void RiscV_dasm_divw()
        {
            AssertCode("divw\ts0,s0,a1", 0x02B4443B);
        }

        [Test]
        public void RiscV_dasm_c_addi16sp()
        {
            AssertCode("c.addi16sp\t000000D0", 0x6169);
        }

        [Test]
        public void RiscV_dasm_beqz_backward()
        {
            AssertCode("c.beqz\ta5,000FFF06", 0xD399);
        }

        [Test]
        public void RiscV_dasm_addiw_sign_extend()
        {
            AssertCode("c.addiw\tt1,00000000", 0x00002301);
        }

        [Test]
        public void RiscV_dasm_li()
        {
            AssertCode("c.li\tt2,00000001", 0x00004385);
        }

        [Test]
        public void RiscV_dasm_beqz_0000C3F1()
        {
            AssertCode("c.beqz\ta5,001000C4", 0x0000C3F1);
        }

        [Test]
        public void RiscV_dasm_c_bnez_backward()
        {
            AssertCode("c.bnez\ta4,000FFF30", 0xFB05);
        }

        [Test]
        public void RiscV_dasm_c_addiw()
        {
            AssertCode("c.addiw\ts0,00000001", 0x00002405);
        }

        [Test]
        public void RiscV_dasm_c_fldsp()
        {
            AssertCode("c.fldsp\tfa3,00000228", 0x00003436);
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
            AssertCode("c.j\t000FFF36", 0x0000BF1D);
        }

        [Test]
        public void RiscV_dasm_c_addi4spn()
        {
            AssertCode("c.addi4spn\ta5,00000020", 0x0000101C);
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
            AssertCode("c.addi\ta5,00000001", 0x00000785);
        }

        [Test]
        public void RiscV_dasm_c_addw()
        {
            AssertCode("c.addw\ta5,a3", 0x00009FB5);
        }

        [Test]
        public void RiscV_dasm_c_srli()
        {
            AssertCode("c.srli\ta5,0000000A", 0x000083A9);
        }

        [Test]
        public void RiscV_dasm_c_srai()
        {
            AssertCode("c.srai\ta4,0000003F", 0x0000977D);
        }

        [Test]
        public void RiscV_dasm_c_andi()
        {
            AssertCode("c.andi\ta2,00000018", 0x00008A61);
        }

        [Test]
        public void RiscV_dasm_c_ldsp()
        {
            AssertCode("c.ldsp\ts7,00000008", 0x00006BA2);
        }

        [Test]
        public void RiscV_dasm_c_slli()
        {
            AssertCode("c.slli\ts0,03", 0x0000040E);
        }



        [Test]
        public void RiscV_dasm_sll()
        {
            AssertCode("sll\ta5,s6,s0", 0x008B17B3);
        }

        [Test]
        public void RiscV_dasm_slli()
        {
            AssertCode("slli\ta2,s2,00000020", 0x02091613);
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
            AssertCode("c.fsd\tfs1,8(a2)", 0x0000A604);
        }

        [Test]
        public void RiscV_dasm_c_fsdsp()
        {
            AssertCode("c.fsdsp\tfs9,000001C8", 0xA7E6);
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
        public void RiscV_dasm_regressions1()
        {
            
AssertCode("fence.i",  "0f100000"); //    invalid
AssertCode("csrrw\ts2,00000315,t0",              "73995231"); //    invalid
AssertCode("csrrs\ta1,sstatus,a5",               "f3a50710"); //    csrrs a1,sstatus,a5
AssertCode("csrrc\ttp,000005F0,s0",              "7332045f"); //    invalid
AssertCode("csrrc\ttp,000005F0,zero",            "7332005f"); //    invalid
AssertCode("csrrsi\ts2,000006E6,0000001E",             "73696f6e"); //    invalid
AssertCode("csrrci\ts0,000006A5,0000001A",             "73745d6a"); //    invalid
AssertCode("mul\ta0,s5,s7",                      "33857a03"); //    add a0,s5,s7
AssertCode("mulh\tt5,a7,s1",                     "339f9802"); //    sll t5,a7,s1
AssertCode("mulhsu\tt3,sp,a6",                   "332e0103"); //    slt t3,sp,a6
AssertCode("mulhu\tt3,sp,a6",                    "333e0103"); //    sltu t3,sp,a6
AssertCode("mulhsu\tt3,sp,a6",                   "332e0103"); //    slt t3,sp,a6
AssertCode("divu\tt4,t3,t2",                     "b35e7e02"); //    srl t4,t3,t2
AssertCode("rem\ta4,a1,a2",                      "33e7c502"); //    or a4,a1,a2
AssertCode("remu\ta6,a1,t2",                     "33f87502"); //    and a6,a1,t2
AssertCode("fsw\tfs1,72(a1)",                    "27a49504"); //    fsw fs1,288(a1)
AssertCode("fmadd.s\tfs10,ft10,ft5,fa0,rmm",     "434d5f50"); //  fmadd.s fs10,ft10,ft5,fa0
AssertCode("fmsub.s\tft8,ft10,fs11,ft7,rdn",     "472ebf39"); //  fmsub.s ft8,ft10,fs11,ft7
AssertCode("fnmsub.s\tfs7,fa4,fs3,fs0,rne",      "cb0b3741"); //    fnmsub.s fs7,fa4,fs3,fs0
AssertCode("fnmadd.s\tfa4,ft4,fs5,fs0,rmm",      "4f475241"); //  fnmadd.s fa4,ft4,fs5,fs0
AssertCode("fclass.s\ta3,fa2",                "d31606e0"); //        fmv.x.w a3,fa2
AssertCode("fsd\tfs4,312(sp)",               "273c4113"); //  fsd fs4,2496(a0)
AssertCode("fmsub.d\tft0,ft0,fs0,fa6,rne",    "47008082"); //  fmsub.s ft0,ft0,fs0,fa6
AssertCode("fnmsub.d\tfs0,ft4,ft2,fa5,rdn",   "4b24227a"); //  fnmsub.s fs0,ft4,ft2,fa5
AssertCode("fnmadd.d\tfa2,ft10,ft5,fa0,rmm",  "4f465f52"); //  fnmadd.s fa2,ft10,ft5,fa0
AssertCode("fcvt.l.d\tra,ft1,rtz",            "d39020c2"); //  fcvt.l.d ra,ft1
AssertCode("fcvt.lu.d\ta0,fa3,rtz",           "539536c2"); //  fcvt.lu.d a0,fa3
        }
  
    }
}
