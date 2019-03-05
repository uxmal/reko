#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Arch.Blackfin;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Blackfin
{
    [TestFixture]
    public class BlackfinDisassemblerTests : DisassemblerTestBase<BlackfinInstruction>
    {
        private readonly BlackfinArchitecture arch;

        public BlackfinDisassemblerTests()
        {
            this.arch = new BlackfinArchitecture("blackfin");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; } = Address.Ptr32(0x00100000);

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return arch.CreateImageWriter(new MemoryArea(LoadAddress, bytes), LoadAddress);
        }

        [Test]
        public void BlackfinDasm_Jump_indirect()
        {
            var instr = DisassembleHexBytes("5300");
            Assert.AreEqual("JUMP [P3];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_Jump_pc_indexed()
        {
            var instr = DisassembleHexBytes("8400");
            Assert.AreEqual("JUMP [PC + P4];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_jump_s()
        {
            var instr = DisassembleHexBytes("FF2F");
            Assert.AreEqual("JUMP.S 000FFFFE;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_load_lo_imm()
        {
            var instr = DisassembleHexBytes("0EE1EC0F ");
            Assert.AreEqual("SP.L = 0FEC;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_load_hi_imm()
        {
            var instr = DisassembleHexBytes("4EE1EC0F ");
            Assert.AreEqual("SP.H = 0FEC;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_load_x_imm()
        {
            var instr = DisassembleHexBytes("2EE1EC0F ");
            Assert.AreEqual("SP = 0FEC (X);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_store_mem_off()
        {
            var instr = DisassembleHexBytes("21E6005C");
            Assert.AreEqual("[P4 + 0x17000] = R1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_RTS()
        {
            var instr = DisassembleHexBytes("1000");
            Assert.AreEqual("RTS;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_RTI()
        {
            var instr = DisassembleHexBytes("1100");
            Assert.AreEqual("RTI;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_RTX()
        {
            var instr = DisassembleHexBytes("1200");
            Assert.AreEqual("RTX;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_RTN()
        {
            var instr = DisassembleHexBytes("1300");
            Assert.AreEqual("RTN;", instr.ToString());
        }


        // Reko: a decoder for Blackfin instruction 6010 at address 07F40142 has not been implemented. (10600000 - C26F0000 - 90610000 - 00600000 - 90610000 - 00600000 - 90610000 - 00600000 - 0b0110............)
        [Test]
        public void BlackfinDasm_mov_dreg_negimm()
        {
            var instr = DisassembleHexBytes("F863");
            Assert.AreEqual("R0 = FFFFFFFF;", instr.ToString());
        }




        [Test]
        public void BlackfinDasm_push_i3()
        {
            var instr = DisassembleHexBytes("5301");
            Assert.AreEqual("[--SP] = I3;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_push_a0_w()
        {
            var instr = DisassembleHexBytes("6101");
            Assert.AreEqual("[--SP] = A0.W;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_push_lt0()
        {
            var instr = DisassembleHexBytes("7101");
            Assert.AreEqual("[--SP] = LT0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_iflush()
        {
            var instr = DisassembleHexBytes("5902");
            Assert.AreEqual("iflush [P1];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_flushinv_post()
        {
            var instr = DisassembleHexBytes("6802");
            Assert.AreEqual("flushinv [P0++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_flush_postinc()
        {
            var instr = DisassembleHexBytes("7002");
            Assert.AreEqual("flush [P0++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_le_d_d()
        {
            var instr = DisassembleHexBytes("070A");
            Assert.AreEqual("CC = R0 <= R7;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_eq_d_imm()
        {
            var instr = DisassembleHexBytes("000C");
            Assert.AreEqual("CC = R0 == 00000000;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_store_uimm()
        {
            var instr = DisassembleHexBytes("6EB0");
            Assert.AreEqual("[P5 + 0x0004] = R6;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_bitclr()
        {
            var instr = DisassembleHexBytes("064C");
            Assert.AreEqual("BITCLR(R6,00);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_pop_r0()
        {
            var instr = DisassembleHexBytes("3090");
            Assert.AreEqual("R0 = [SP++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_pop_a1_w()
        {
            var instr = DisassembleHexBytes("2301");
            Assert.AreEqual("A1.W = [SP++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_push_rete()
        {
            var instr = DisassembleHexBytes("7E01");
            Assert.AreEqual("[--SP] = RETE;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_csync()
        {
            var instr = DisassembleHexBytes("2300");
            Assert.AreEqual("CSYNC;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ssync()
        {
            var instr = DisassembleHexBytes("2400");
            Assert.AreEqual("SSYNC;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction A0A0 at address 07F40550 has not been implemented. (A0A00000 - E6A00000 - E6A00000 - 0b1010............)
        [Test]
        public void BlackfinDasm_load_dreg_shortoffset()
        {
            var instr = DisassembleHexBytes("A0A0");
            Assert.AreEqual("R0 = [P4 + 0x0008];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_sti()
        {
            var instr = DisassembleHexBytes("4000");
            Assert.AreEqual("STI R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_nop()
        {
            var instr = DisassembleHexBytes("0000");
            Assert.AreEqual("NOP;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_call()
        {
            var instr = DisassembleHexBytes("00E37805");
            Assert.AreEqual("CALL 00100AF0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_pop_b1()
        {
            var instr = DisassembleHexBytes("1901");
            Assert.AreEqual("B1 = [SP++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_and3()
        {
            var instr = DisassembleHexBytes("5154");
            Assert.AreEqual("R1 = R1 & R2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_ncc_jump()
        {
            var instr = DisassembleHexBytes("0310");
            Assert.AreEqual("IF !CC JUMP 00100006;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_iflush_postinc()
        {
            var instr = DisassembleHexBytes("7802");
            Assert.AreEqual("iflush [P0++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_ncc_mov_d_p()
        {
            var instr = DisassembleHexBytes("5206");
            Assert.AreEqual("IF !CC R2 = P2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_ncc_d_d()
        {
            var instr = DisassembleHexBytes("0807");
            Assert.AreEqual("IF !CC R1 = R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_ncc_p_p()
        {
            var instr = DisassembleHexBytes("F407");
            Assert.AreEqual("IF !CC SP = P4;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_bitset()
        {
            var instr = DisassembleHexBytes("C04A");
            Assert.AreEqual("BITSET(R0,18);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_n_cc_mov_p_p()
        {
            var instr = DisassembleHexBytes("F907");
            Assert.AreEqual("IF !CC FP = P1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_st_p()
        {
            var instr = DisassembleHexBytes("ECBD");
            Assert.AreEqual("[P5 + 0x001C] = P4;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0B21003 at address 07F406AE has not been implemented. (0310B2E0 - 780500E3 - EA0500E3 - 360030E6 - 360036E6 - 030426E4 - 050827E6 - 800600E3 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6)
        [Test]
        public void BlackfinDasm_E0B21003()
        {
            var instr = DisassembleHexBytes("B2E00310");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0B21002 at address 07F406CA has not been implemented. (0210B2E0 - 0310B2E0 - 780500E3 - EA0500E3 - 360030E6 - 360036E6 - 030426E4 - 050827E6 - 800600E3 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6 - 005C21E6 - 015C20E6 - 005C21E6)
        [Test]
        public void BlackfinDasm_E0B21002()
        {
            var instr = DisassembleHexBytes("B2E00210");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_pop_regs()
        {
            var instr = DisassembleHexBytes("2805");
            Assert.AreEqual("(FP:P0) = [SP++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_push_regs()
        {
            var instr = DisassembleHexBytes("EC05");
            Assert.AreEqual("[--SP] = (FP:P4);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_cli()
        {
            var instr = DisassembleHexBytes("3000");
            Assert.AreEqual("CLI R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_not()
        {
            var instr = DisassembleHexBytes("C143");
            Assert.AreEqual("R1 = ~R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_asr_dreg()
        {
            var instr = DisassembleHexBytes("1340");
            Assert.AreEqual("R3 >>>= R2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mul_dreg()
        {
            var instr = DisassembleHexBytes("C740");
            Assert.AreEqual("R7 *= R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsr()
        {
            var instr = DisassembleHexBytes("884E");
            Assert.AreEqual("R0 >>= 11;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add3()
        {
            var instr = DisassembleHexBytes("C751");
            Assert.AreEqual("R7 = R7 + R0;", instr.ToString());
        }





        // Reko: a decoder for Blackfin instruction 40F7 at address 07F40912 has not been implemented. (F7400000 - 884E0000 - C7400000 - C1430000 - C04A0000 - C04A0000 - 2A4E0000 - 2A4E0000 - 2A4E0000 - 064C0000 - 0b0100............)
        [Test]
        public void BlackfinDasm_mul()
        {
            var instr = DisassembleHexBytes("F740");
            Assert.AreEqual("R7 *= R6;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_lt_d_d()
        {
            var instr = DisassembleHexBytes("9009");
            Assert.AreEqual("CC = R2 < R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_n_bittest()
        {
            var instr = DisassembleHexBytes("0848");
            Assert.AreEqual("CC = !BITTEST(R0,01);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_eq()
        {
            var instr = DisassembleHexBytes("0802");
            Assert.AreEqual("CC = R0 == R1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_store_indirect()
        {
            var instr = DisassembleHexBytes("1293");
            Assert.AreEqual("[P2] = R2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_sub3()
        {
            var instr = DisassembleHexBytes("5152");
            Assert.AreEqual("R1 = R1 - R2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add_imm()
        {
            var instr = DisassembleHexBytes("6764");
            Assert.AreEqual("R7 += 0000000C;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add_simm()
        {
            var instr = DisassembleHexBytes("A064");
            Assert.AreEqual("R0 += 00000014;", instr.ToString());
        }

 

        [Test]
        public void BlackfinDasm_mov_reg_simm()
        {
            var instr = DisassembleHexBytes("0960");
            Assert.AreEqual("R1 = 00000001;", instr.ToString());
        }




        [Test]
        public void BlackfinDasm_load_z_byte()
        {
            var instr = DisassembleHexBytes("2899");
            Assert.AreEqual("R0 = B[P5] (Z);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_cc_jump_bp()
        {
            var instr = DisassembleHexBytes("021C");
            Assert.AreEqual("IF CC JUMP 00100004 (BP);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_emudat()
        {
            var instr = DisassembleHexBytes("3D3E");
            Assert.AreEqual("EMUDAT = R5;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_x_byte_postinc()
        {
            var instr = DisassembleHexBytes("5098");
            Assert.AreEqual("R0 = B[P2++] (X);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_eq_d_d()
        {
            var instr = DisassembleHexBytes("1808");
            Assert.AreEqual("CC = R3 == R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_store_byte_postinc()
        {
            var instr = DisassembleHexBytes("109A");
            Assert.AreEqual("B[P2++] = R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_ncc_jump_bp()
        {
            var instr = DisassembleHexBytes("0214");
            Assert.AreEqual("IF !CC JUMP 00100004 (BP);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_if_cc_jump()
        {
            var instr = DisassembleHexBytes("F61F");
            Assert.AreEqual("IF CC JUMP 000FFFEC (BP);", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4340 at address 07F40B02 has not been implemented. (40430000 - 00480000 - F84A0000 - 08480000 - 58430000 - 08480000 - 324F0000 - F7400000 - 884E0000 - C7400000 - C1430000 - C04A0000 - C04A0000 - 2A4E0000 - 2A4E0000 - 2A4E0000 - 064C0000 - 0b0100............)
        [Test]
        public void BlackfinDasm_mov_b_z()
        {
            var instr = DisassembleHexBytes("4043");
            Assert.AreEqual("R0 = R0.B (Z);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_long_bx()
        {
            var instr = DisassembleHexBytes("A8E50100");
            Assert.AreEqual("R0 = B[P5 + 0x0001] (X);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsl()
        {
            var instr = DisassembleHexBytes("404F");
            Assert.AreEqual("R0 <<= 08;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_or3()
        {
            var instr = DisassembleHexBytes("0856");
            Assert.AreEqual("R0 = R0 | R1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_jump_l()
        {
            var instr = DisassembleHexBytes("FFE295FF");
            Assert.AreEqual("JUMP.L 000FFF2A;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_store_byte_uimm()
        {
            var instr = DisassembleHexBytes("B0E61000");
            Assert.AreEqual("B[SP + 0x0010] = R0;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 04C5 at address 07F40BB0 has not been implemented. (C5040000 - 00000000 - 10000000 - 08080000 - 10000000 - 27010000 - 67010000 - 00000000 - 38050000 - 67010000 - 78050000 - 00000000 - 10000000 - A5050000 - 27010000 - A5050000 - 27010000 - 25080000 - BE090000 - 000C0000 - 00000000 - B9090000 - 28080000 - 00000000 - 010C0000 - BA090000 - 08070000 - 18080000 - 67010000 - E5050000 - 00000000 - 10000000 - AC050000 - 27010000 - 080A0000 - 000C0000 - F7070000 - 67010000 - EC050000 - 00000000 - 10000000 - 38050000 - 01070000 - 000C0000 - 27010000 - F4070000 - 67010000 - 78050000 - 00000000 - 52060000 - 11070000 - 210E0000 - 010C0000 - 10000000 - 020C0000 - 020C0000 - 00020000 - 08020000 - 00000000 - 10000000 - 28050000 - 27010000 - 90090000 - 070A0000 - 67010000 - 68050000 - 00000000 - 10000000 - 27010000 - 67010000 - 10000000 - 38050000 - 27010000 - 81090000 - 67010000 - 78050000 - 00000000 - 23000000 - 23000000 - 10000000 - 30000000 - 00000000 - 10000000 - 40000000 - F9070000 - 08080000 - 00000000 - 10000000 - 38050000 - 27010000 - 10000000 - 010C0000 - 67010000 - 78050000 - 00000000 - 10000000 - 40000000 - F9070000 - 00000000 - 10000000 - 70020000 - 00000000 - 10000000 - 68020000 - 10000000 - 70020000 - 58020000 - 10000000 - 00000000 - 78020000 - 00000000 - 13000000 - 00000000 - 11000000 - 80050000 - 10010000 - 11010000 - 12010000 - 13010000 - 14010000 - 15010000 - 18010000 - 19010000 - 1A010000 - 1B010000 - 20010000 - 21010000 - 22010000 - 23010000 - 30010000 - 33010000 - 31010000 - 34010000 - 32010000 - 35010000 - 26010000 - 27010000 - 3E010000 - 39010000 - 3A010000 - 40010000 - 7A010000 - 79010000 - 7E010000 - 7D010000 - 7C010000 - 40010000 - 74010000 - 71010000 - 73010000 - 70010000 - 63010000 - 62010000 - 61010000 - 60010000 - 5B010000 - 5A010000 - 59010000 - 58010000 - 5F010000 - 5E010000 - 5D010000 - 5C010000 - 57010000 - 56010000 - 78010000 - 4F010000 - C0050000 - 40010000 - 00000000 - 11000000 - 80050000 - 10010000 - 11010000 - 12010000 - 13010000 - 14010000 - 15010000 - 16010000 - 17010000 - 1A010000 - 1B010000 - 20010000 - 21010000 - 22010000 - 23010000 - 30010000 - 33010000 - 31010000 - 34010000 - 32010000 - 35010000 - 26010000 - 27010000 - 3C010000 - 3E010000 - 39010000 - 3A010000 - 40000000 - 23000000 - 40010000 - 7A010000 - 79010000 - 7E010000 - 7D010000 - 7C010000 - 40010000 - 67010000 - 40010000 - 70010000 - 63010000 - 62010000 - 61010000 - 60010000 - 5B010000 - 5A010000 - 59010000 - 58010000 - 5F010000 - 5E010000 - 5D010000 - 5C010000 - 57010000 - 56010000 - 55010000 - 54010000 - 53010000 - 40010000 - 12000000 - 80050000 - 10010000 - 11010000 - 12010000 - 13010000 - 14010000 - 15010000 - 16010000 - 17010000 - 1C010000 - 1D010000 - 1E010000 - 21010000 - 22010000 - 23010000 - 30010000 - 33010000 - 31010000 - 34010000 - 32010000 - 35010000 - 26010000 - 27010000 - 3C010000 - 3D010000 - 3E010000 - 39010000 - 7B010000 - 46000000 - 36000000 - 000C0000 - 40010000 - 7C010000 - 40010000 - 67010000 - 40010000 - 66010000 - 75010000 - 72010000 - 74010000 - 71010000 - 73010000 - 70010000 - 63010000 - 62010000 - 61010000 - 60010000 - 5B010000 - 5A010000 - 5E010000 - 5D010000 - 5C010000 - 57010000 - 56010000 - 55010000 - 54010000 - 53010000 - 52010000 - 51010000 - 50010000 - 78010000 - 4F010000 - C0050000 - 40010000 - 5E010000 - 5D010000 - 5C010000 - 57010000 - 56010000 - 55010000 - 54010000 - 53010000 - 52010000 - 51010000 - 50010000 - 78010000 - 4F010000 - C0050000 - 40010000 - 24000000 - 24000000 - 24000000 - 60050000 - 10000000 - 24000000 - 24000000 - 24000000 - 60050000 - 10000000 - 24000000 - 24000000 - 24000000 - 60050000 - 10000000 - 60050000 - 10000000 - 60050000 - 10000000 - 60050000 - 10000000 - 24000000 - 24000000 - 24000000 - 24000000 - 24000000 - 24000000 - 24000000 - 24000000 - 24000000 - 0b0000............)
        [Test]
        public void BlackfinDasm_push_p5()
        {
            var instr = DisassembleHexBytes("C504");
            Assert.AreEqual("[--SP] = (P5:P5);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_load_ptr()
        {
            var instr = DisassembleHexBytes("5591");
            Assert.AreEqual("P5 = [P2];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add3_p()
        {
            var instr = DisassembleHexBytes("555B");
            Assert.AreEqual("P5 = P5 + P2;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_pop_p5()
        {
            var instr = DisassembleHexBytes("8504");
            Assert.AreEqual("(P5:P5) = [SP++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_cc_eq_p_imm()
        {
            var instr = DisassembleHexBytes("420C");
            Assert.AreEqual("CC = P2 == 00000002;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_call_indir()
        {
            var instr = DisassembleHexBytes("6200");
            Assert.AreEqual("CALL [P2];", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C0801808 at address 07F67398 has not been implemented. (081880C0)
        [Test]
        public void BlackfinDasm_C0801808()
        {
            var instr = DisassembleHexBytes("80C00818");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_excpt()
        {
            var instr = DisassembleHexBytes("A100");
            Assert.AreEqual("EXCPT 01;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_st_p_long()
        {
            var instr = DisassembleHexBytes("29E70F08");
            Assert.AreEqual("[P5 + 0x203C] = P1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsr3()
        {
            var instr = DisassembleHexBytes("82C6C281");
            Assert.AreEqual("R0 = R2 >> 08;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsl_d_d()
        {
            var instr = DisassembleHexBytes("8140");
            Assert.AreEqual("R1 <<= R0;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction AC8E at address 07F403A8 has not been implemented. (8EAC0000 - 0b101011..........)
        [Test]
        public void BlackfinDasm_ld_sp()
        {
            var instr = DisassembleHexBytes("8EAC");
            Assert.AreEqual("SP = [P1 + 0x0008];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_raise()
        {
            var instr = DisassembleHexBytes("9500");
            Assert.AreEqual("RAISE 05;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_link()
        {
            var instr = DisassembleHexBytes("00E82900");
            Assert.AreEqual("LINK +00000290;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_asr3_long()
        {
            var instr = DisassembleHexBytes("82C69203");
            Assert.AreEqual("R1 = R2 >>> 0E;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_long_offs()
        {
            var instr = DisassembleHexBytes("28E42D00");
            Assert.AreEqual("R0 = [P5 + 0x00B4];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsr3_i()
        {
            var instr = DisassembleHexBytes("D144");
            Assert.AreEqual("P1 = P2 >> 01;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_idx()
        {
            var instr = DisassembleHexBytes("0B9C");
            Assert.AreEqual("R3 = [I1++];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mnop()
        {
            var instr = DisassembleHexBytes("03C80018");
            Assert.AreEqual("MNOP;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 98C9 at address 07F413C0 has not been implemented. (C9980000 - 0B9C0000 - 0B9C0000 - 019C0000 - 0b1001............)
        [Test]
        public void BlackfinDasm_98C9()
        {
            var instr = DisassembleHexBytes("C998");
            Assert.AreEqual("R1 = W[P1--].B (X);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_st_dec_b()
        {
            var instr = DisassembleHexBytes("819A");
            Assert.AreEqual("B[P0--] = R1;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_postdec_zb()
        {
            var instr = DisassembleHexBytes("9998");
            Assert.AreEqual("R1 = W[P3--].B (Z);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add3_lo()
        {
            var instr = DisassembleHexBytes("02C41104");
            Assert.AreEqual("R2.L = R2.L + R2.L;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add3_long()
        {
            var instr = DisassembleHexBytes("22C41144");
            Assert.AreEqual("R2.H = R2.L + R1.L;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_p()
        {
            var instr = DisassembleHexBytes("41AC");
            Assert.AreEqual("P1 = [P0 + 0x0004];", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_sub_p_p()
        {
            var instr = DisassembleHexBytes("2944");
            Assert.AreEqual("P1 -= P5;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4282 at address 07F422D4 has not been implemented. (C2420000 - F1420000 - 48400000 - 29440000 - 0A440000 - D1440000 - D1440000 - D2440000 - D1440000 - B8400000 - F1420000 - F0420000 - F9420000 - F8420000 - 81400000 - 81400000 - 81400000 - 0b0100............)
        [Test]
        public void BlackfinDasm_mov_d_xl()
        {
            var instr = DisassembleHexBytes("8242");
            Assert.AreEqual("R2 = R0.L (X);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsr_d_d()
        {
            var instr = DisassembleHexBytes("4740");
            Assert.AreEqual("R7 >>= R0;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_p_zl()
        {
            var instr = DisassembleHexBytes("1095");
            Assert.AreEqual("R0 = W[P2] (Z);", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction AA30 at address 07F42548 has not been implemented. (30AA0000 - 0b101010..........)
        [Test]
        public void BlackfinDasm_ld_sp_wx()
        {
            var instr = DisassembleHexBytes("30AA");
            Assert.AreEqual("R0 = W[SP + 0x0010] (X);", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C6820161 at address 07F4258A has not been implemented. (610182C6 - 670182C6 - 408682C6 - 838582C6 - 838082C6 - 848182C6 - 83018CC2 - 868182C6 - 808782C6 - 360028E4 - 360028E4 - 360020E4 - 240020E4 - 100020E4 - 110020E4 - 120020E4 - 130020E4 - 180020E4 - 190020E4 - 1A0020E4 - 1B0020E4 - 140020E4 - 150020E4 - 160020E4 - 170020E4 - 1C0020E4 - 1D0020E4 - 1E0020E4 - 1F0020E4 - 200020E4 - 210020E4 - 220020E4 - 230020E4 - 250020E4 - 260020E4 - 270020E4 - 280020E4 - 290020E4 - 2A0020E4 - 2B0020E4 - 2C0020E4 - 2D0020E4 - 2E0020E4 - 2F0020E4 - 300020E4 - 310020E4 - 320020E4 - 330020E4 - 030000E8 - 340028E4 - 330028E4 - 300028E4 - 2E0028E4 - 2B0028E4 - 2A0028E4 - 290028E4 - 280028E4 - 270028E4 - 260028E4 - 250028E4 - 230028E4 - 220028E4 - 210028E4 - 200028E4 - 1F0028E4 - 1E0028E4 - 1D0028E4 - 1C0028E4 - 1B0028E4 - 1A0028E4 - 190028E4 - 180028E4 - 170028E4 - 160028E4 - 150028E4 - 140028E4 - 130028E4 - 120028E4 - 110028E4 - 100028E4 - 848382C6 - 9C8382C6 - DC8382C6 - C48382C6 - C38182C6 - 1110B8E0 - 2B00B3E0 - 270000E4 - 260000E4 - 250000E4 - 240000E4 - 230000E4 - 220000E4 - 210000E4 - 200000E4 - 1F0000E4 - 1E0000E4 - 1D0000E4 - 1C0000E4 - 1B0000E4 - 1A0000E4 - 190000E4 - 180000E4 - 170000E4 - 160000E4 - 150000E4 - 140000E4 - 130000E4 - 120000E4 - 110000E4 - 100000E4 - 3F400AC4 - 3F000AC4 - 0220A2E0 - 0210A2E0 - 114422C4 - 110402C4 - 418482C6 - 0320A2E0 - 0320A2E0 - 001803C8 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 001803C8 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 360028E4 - 500F82C6 - 240029E4 - 13002AE4 - 120029E4 - 110028E4 - 100028E4 - 18002AE4 - 140029E4 - 200028E4 - 1C0028E4 - 19002AE4 - 150029E4 - 210028E4 - 1D0028E4 - 1A002AE4 - 160029E4 - 220028E4 - 1E0028E4 - 1B002AE4 - 170029E4 - 230028E4 - 1F0028E4 - 26002AE4 - 270029E4 - 250028E4 - 2A002AE4 - 2B0029E4 - 280028E4 - 290028E4 - 2E002AE4 - 2F0029E4 - 2C0028E4 - 2D0028E4 - 32002AE4 - 330029E4 - 300028E4 - 310028E4 - 360028E4 - 920382C6 - 2B0000E8 - 290000E8 - C38182C6 - A78582C6 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 030426E4 - C28182C6 - DB8182C6 - C38182C6 - 0F0829E7 - F88582C6 - F88582C6 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C6820161()
        {
            var instr = DisassembleHexBytes("82C66101");
            Assert.AreEqual("R0 = R1 >>> F4;", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_wz()
        {
            var instr = DisassembleHexBytes("A0A4");
            Assert.AreEqual("R0 = W[P4 + 0x0004] (Z);", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_ld_dlo()
        {
            var instr = DisassembleHexBytes("229C");
            Assert.AreEqual("R2.L = W[I0++];", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A21007 at address 07F41316 has not been implemented. (0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A21007()
        {
            var instr = DisassembleHexBytes("A2E00710");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A22006 at address 07F4132C has not been implemented. (0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A22006()
        {
            var instr = DisassembleHexBytes("A2E00620");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A32004 at address 07F41376 has not been implemented. (0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A32004()
        {
            var instr = DisassembleHexBytes("A3E00420");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A52005 at address 07F41384 has not been implemented. (0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A52005()
        {
            var instr = DisassembleHexBytes("A5E00520");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A22003 at address 07F413BC has not been implemented. (0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A22003()
        {
            var instr = DisassembleHexBytes("A2E00320");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A21002 at address 07F413FA has not been implemented. (0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A21002()
        {
            var instr = DisassembleHexBytes("A2E00210");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0A22002 at address 07F41474 has not been implemented. (0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0A22002()
        {
            var instr = DisassembleHexBytes("A2E00220");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0B3002B at address 07F4172A has not been implemented. (2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0B3002B()
        {
            var instr = DisassembleHexBytes("B3E02B00");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0B81011 at address 07F41774 has not been implemented. (1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0B81011()
        {
            var instr = DisassembleHexBytes("B8E01110");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C28C0183 at address 07F422E4 has not been implemented. (83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C28C0183()
        {
            var instr = DisassembleHexBytes("8CC28301");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9710 at address 07F4254C has not been implemented. (10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9710()
        {
            var instr = DisassembleHexBytes("1097");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_neg_cc()
        {
            var instr = DisassembleHexBytes("1802");
            Assert.AreEqual("CC = !CC;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9D00 at address 07F4553E has not been implemented. (009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9D00()
        {
            var instr = DisassembleHexBytes("009D");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9D20 at address 07F46616 has not been implemented. (209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9D20()
        {
            var instr = DisassembleHexBytes("209D");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_E0B22009()
        {
            var instr = DisassembleHexBytes("B2E00920");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 45C2 at address 07F4901E has not been implemented. (C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_45C2()
        {
            var instr = DisassembleHexBytes("C245");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 45FA at address 07F4902C has not been implemented. (FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_45FA()
        {
            var instr = DisassembleHexBytes("FA45");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_st_p_d_w()
        {
            var instr = DisassembleHexBytes("1797");
            Assert.AreEqual("W[P2] = R7;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction E0B22010 at address 07F561C8 has not been implemented. (1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_E0B22010()
        {
            var instr = DisassembleHexBytes("B2E01020");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E60 at address 07F57142 has not been implemented. (609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E60()
        {
            var instr = DisassembleHexBytes("609E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9552 at address 07F57486 has not been implemented. (52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9552()
        {
            var instr = DisassembleHexBytes("5295");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E63 at address 07F5903C has not been implemented. (639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E63()
        {
            var instr = DisassembleHexBytes("639E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9F38 at address 07F5903E has not been implemented. (389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9F38()
        {
            var instr = DisassembleHexBytes("389F");
            Assert.AreEqual("@@@", instr.ToString());
        }



        // Reko: a decoder for Blackfin instruction C28C01B8 at address 07F5A508 has not been implemented. (B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C28C01B8()
        {
            var instr = DisassembleHexBytes("8CC2B801");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C2882001 at address 07F5AA0E has not been implemented. (012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C2882001()
        {
            var instr = DisassembleHexBytes("88C20120");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C28C0031 at address 07F5AC7C has not been implemented. (31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C28C0031()
        {
            var instr = DisassembleHexBytes("8CC23100");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C28C0008 at address 07F5B0B8 has not been implemented. (08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C28C0008()
        {
            var instr = DisassembleHexBytes("8CC20800");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C2882097 at address 07F5B340 has not been implemented. (972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C2882097()
        {
            var instr = DisassembleHexBytes("88C29720");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4455 at address 07F5D460 has not been implemented. (55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_4455()
        {
            var instr = DisassembleHexBytes("5544");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 45C1 at address 07F5D488 has not been implemented. (C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_45C1()
        {
            var instr = DisassembleHexBytes("C145");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4103 at address 07F5D5FC has not been implemented. (03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_4103()
        {
            var instr = DisassembleHexBytes("0341");
            Assert.AreEqual("R3 = R3 + R0 << 1;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4468 at address 07F5DA5E has not been implemented. (68440000 - 52440000 - 52440000 - 52440000 - 50440000 - 03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_4468()
        {
            var instr = DisassembleHexBytes("6844");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_lsl_imm()
        {
            var instr = DisassembleHexBytes("4C44");
            Assert.AreEqual("P4 = P1 << 02", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 45E9 at address 07F5DA6A has not been implemented. (E9450000 - 4C440000 - 68440000 - 52440000 - 52440000 - 52440000 - 50440000 - 03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_45E9()
        {
            var instr = DisassembleHexBytes("E945");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E61 at address 07F61632 has not been implemented. (619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E61()
        {
            var instr = DisassembleHexBytes("619E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E6F at address 07F6167E has not been implemented. (6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E6F()
        {
            var instr = DisassembleHexBytes("6F9E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E62 at address 07F61696 has not been implemented. (629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E62()
        {
            var instr = DisassembleHexBytes("629E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9608 at address 07F618AC has not been implemented. (08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9608()
        {
            var instr = DisassembleHexBytes("0896");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9628 at address 07F619C4 has not been implemented. (28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9628()
        {
            var instr = DisassembleHexBytes("2896");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9488 at address 07F61A06 has not been implemented. (88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9488()
        {
            var instr = DisassembleHexBytes("8894");
            Assert.AreEqual("Dx = W[P++] (Z)", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E65 at address 07F61B96 has not been implemented. (659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E65()
        {
            var instr = DisassembleHexBytes("659E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9D2E at address 07F61B9A has not been implemented. (2E9D0000 - 659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9D2E()
        {
            var instr = DisassembleHexBytes("2E9D");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_add_sh2_p()
        {
            var instr = DisassembleHexBytes("D145");
            Assert.AreEqual("P1 = P1 + P2 << 2;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9E67 at address 07F61F76 has not been implemented. (679E0000 - 609E0000 - 619E0000 - 629E0000 - 609E0000 - 619E0000 - 629E0000 - 0E970000 - 2E9D0000 - 659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9E67()
        {
            var instr = DisassembleHexBytes("679E");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 9610 at address 07F64FD6 has not been implemented. (10960000 - 2D970000 - 11970000 - 28970000 - 10970000 - 10970000 - 10970000 - 10970000 - 20970000 - 29970000 - 639E0000 - 679E0000 - 609E0000 - 619E0000 - 629E0000 - 609E0000 - 619E0000 - 629E0000 - 0E970000 - 2E9D0000 - 659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
        [Test]
        public void BlackfinDasm_9610()
        {
            var instr = DisassembleHexBytes("1096");
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void BlackfinDasm_mov_r_cc()
        {
            var instr = DisassembleHexBytes("8603");
            Assert.AreEqual("AQ = CC;", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction 4208 at address 07F6726A has not been implemented. (08420000 - 4A440000 - 48410000 - 50410000 - 51440000 - D1450000 - 48410000 - 48410000 - 70410000 - 48410000 - 48410000 - 60410000 - 48410000 - 50410000 - 60410000 - 60410000 - 41410000 - 45410000 - 68410000 - 50410000 - 68410000 - 68410000 - 78410000 - 48410000 - 52440000 - 69440000 - 4C440000 - E9450000 - 4C440000 - 68440000 - 52440000 - 52440000 - 52440000 - 50440000 - 03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)
        [Test]
        public void BlackfinDasm_4208()
        {
            var instr = DisassembleHexBytes("0842");
            Assert.AreEqual("DIVQ (R0,R1);", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C0814608 at address 07F673A0 has not been implemented. (084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C0814608()
        {
            var instr = DisassembleHexBytes("81C00846");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C0815801 at address 07F673A4 has not been implemented. (015881C0 - 084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C0815801()
        {
            var instr = DisassembleHexBytes("81C00158");
            Assert.AreEqual("@@@", instr.ToString());
        }

        // Reko: a decoder for Blackfin instruction C08B3800 at address 07F673B0 has not been implemented. (00388BC0 - 015881C0 - 084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
        [Test]
        public void BlackfinDasm_C08B3800()
        {
            var instr = DisassembleHexBytes("8BC00038");
            Assert.AreEqual("@@@", instr.ToString());
        }
    }
}
