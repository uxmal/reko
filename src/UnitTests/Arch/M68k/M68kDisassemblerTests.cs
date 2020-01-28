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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Arch.M68k;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kDisassemblerTests : DisassemblerTestBase<M68kInstruction>
    {
        private M68kArchitecture arch = new M68kArchitecture("m68k");
        private IEnumerator<M68kInstruction> dasm;

        private IEnumerator<M68kInstruction> CreateDasm(byte[] bytes, uint address)
        {
            Address addr = Address.Ptr32(address);
            MemoryArea img = new MemoryArea(addr, bytes);
            return M68kDisassembler.Create68020(img.CreateBeReader(addr)).GetEnumerator();
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr32(0x10000000); }
        }

        private void RunTest(string expected, params ushort[] words)
        {
            dasm = CreateDasm(words);
            Assert.AreEqual(expected, Disassemble().ToString());
        }

        private IEnumerator<M68kInstruction> CreateDasm(params ushort[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[] { (byte)(w >> 8), (byte)w }).ToArray();
            return CreateDasm(bytes, 0x10000000);
        }

        public MachineInstruction Disassemble()
        {
            if (dasm.MoveNext())
                return dasm.Current;
            return null;
        }

        [Test]
        public void M68kdis_moveQ()
        {
            RunTest("moveq\t#$01,d1", 0x7201);
        }

        [Test]
        public void M68kdis_addQ()
        {
            RunTest("addq.l\t#$07,(a2)", 0x5E92);
        }

        [Test]
        public void M68kdis_Ori()
        {
            RunTest("ori.b\t#$12,d0", 0x0000, 0x0012);
        }

        [Test]
        public void M68kdis_OriCcr()
        {
            RunTest("ori.b\t#$42,ccr", 0x003C, 0x0042);
        }

        [Test]
        public void M68kdis_OriSr()
        {
            RunTest("ori.w\t#$0042,sr", 0x007C, 0x0042);
        }

        [Test]
        public void M68kdis_MoveW()
        {
            RunTest("move.w\t$4711(a7),d0", 0x302F, 0x4711);
        }

        [Test]
        public void M68kdis_Lea()
        {
            RunTest("lea\t$0004(a7),a1", 0x43EF, 0x0004);
        }

        [Test]
        public void M68kdis_LslD()
        {
            RunTest("lsl.w\t#$02,d1", 0xE549, 0x0004);
        }

        [Test]
        public void M68kdis_AddaW()
        {
            RunTest("adda.w\td1,a1", 0xD2C1, 0x0004);
        }

        [Test]
        public void M68kdis_Addal()
        {
            RunTest("adda.l\t(a4)+,a5", 0xDBDC);
        }

        [Test]
        public void M68kdis_MoveA()
        {
            RunTest("movea.l\t(a1),a0", 0x2051, 0x0004);
        }

        [Test]
        public void M68kdis_MoveM()
        {
            RunTest("movem.l\ta5,-(a7)", 0x48E7, 0x0004);
        }

        [Test]
        public void M68kdis_BraB()
        {
            RunTest("bra\t$1000001C", 0x601A);
        }

        [Test]
        public void M68kdis_bchg()
        {
            RunTest("bchg.l\td0,d0", 0x0140);
        }

        [Test]
        public void M68kdis_Dbra()
        {
            RunTest("dbra\td2,$0FFFFFE6", 0x51CA, 0xFFE4);
        }

        [Test]
        public void M68kdis_Moveb()
        {
            RunTest("move.b\t(a2)+,d2", 0x141A);
        }

        [Test]
        public void M68kdis_ManyMoves()
        {
            dasm = CreateDasm(new byte[] { 0x20, 0x00, 0x20, 0x27, 0x20, 0x40, 0x20, 0x67, 0x20, 0x80, 0x21, 0x40, 0x00, 0x00 }, 0x10000000);
            Assert.AreEqual("move.l\td0,d0", Disassemble().ToString());
            Assert.AreEqual("move.l\t-(a7),d0", Disassemble().ToString());
            Assert.AreEqual("movea.l\td0,a0", Disassemble().ToString());
            Assert.AreEqual("movea.l\t-(a7),a0", Disassemble().ToString());
            Assert.AreEqual("move.l\td0,(a0)", Disassemble().ToString());
            Assert.AreEqual("move.l\td0,$0000(a0)", Disassemble().ToString());
        }

        [Test]
        public void M68kdis_AddB()
        {
            RunTest("add.b\td2,d1", 0xD202);
        }

        [Test]
        public void M68kdis_Eor()
        {
            dasm = CreateDasm(0xB103, 0xB143, 0xB183);
            Assert.AreEqual("eor.b\td0,d3", Disassemble().ToString());
            Assert.AreEqual("eor.w\td0,d3", Disassemble().ToString());
            Assert.AreEqual("eor.l\td0,d3", Disassemble().ToString());
        }

        [Test]
        public void M68kdis_Bcs()
        {
            dasm = CreateDasm(0x6572);
            Assert.AreEqual("bcs\t$10000074", Disassemble().ToString());
        }

        [Test]
        public void M68kdis_or_s_with_immediate()
        {
            RunTest("or.b\t#$23,d3", 0x863c, 0x1123);
            RunTest("or.w\t#$1123,d3", 0x867c, 0x1123);
            RunTest("or.l\t#$11234455,d3", 0x86Bc, 0x1123, 0x4455);
        }

        [Test]
        public void M68kdis_Pre_and_post_dec()
        {
            RunTest("move.w\t-(a3),(a3)+", 0x36E3);
        }

        [Test]
        public void M68kdis_eori()
        {
            RunTest("eori.b\t#$34,d0", 0x0A00, 0x1234);
        }

        [Test]
        public void M68kdis_muls_w()
        {
            RunTest("muls.w\t-(a3),d0", 0xC1E3);
        }

        [Test]
        public void M68kdis_mulu_l()
        {
            RunTest("mulu.l\td0,d6,d7", 0x4c00, 0x7406);
        }

        [Test]
        public void M68kdis_not()
        {
            RunTest("not.b\td7", 0x4607);
        }

        [Test]
        public void M68kdis_and()
        {
            RunTest("and.l\t-(a3),d1", 0xC2A3);
        }

        [Test]
        public void M68kdis_and_rev()
        {
            RunTest("and.l\td1,-(a3)", 0xC3A3);
        }

        [Test]
        public void M68kdis_andi_32()
        {
            RunTest("andi.l\t#$00010000,(a4)+", 0x029C, 0x0001, 0x0000);
        }

        [Test]
        public void M68kdis_andi_8()
        {
            RunTest("andi.b\t#$F0,d2", 0x0202, 0x00F0);
        }

        [Test]
        public void M68kdis_asrb_qb()
        {
            RunTest("asr.b\t#$07,d0", 0xEE00);
        }

        [Test]
        public void M68kdis_neg_w()
        {
            RunTest("neg.w\t(a3)+", 0x445B);
        }

        [Test]
        public void M68kdis_negx_8()
        {
            RunTest("negx.w\td0", 0x4040);
        }

        [Test]
        public void M68kdis_sub_er_16()
        {
            RunTest("sub.w\t-(a4),d0", 0x9064);
        }

        [Test]
        public void M68kdis_suba_16()
        {
            RunTest("suba.w\t(a4)+,a0", 0x90DC);
        }

        [Test]
        public void M68kdis_clr()
        {
            RunTest("clr.w\t$0008(a0)", 0x4268, 0x0008);
            RunTest("clr.w\t(a0)+", 0x4258);
            RunTest("clr.b\t(a0,d0)", 0x4230, 0x0800);
        }

        [Test]
        public void M68kdis_cmpib()
        {
            RunTest("cmpi.b\t#$42,d0", 0x0C00, 0x0042);
        }

        [Test]
        public void M68kdis_cmpw_d_d()
        {
            RunTest("cmp.w\td1,d0", 0xB041);
        }

        [Test]
        public void M68kdis_jsr_mem()
        {
            RunTest("jsr.l\t(a0)", 0x4E90);
        }

        [Test]
        public void M68kdis_or()
        {
            RunTest("or.b\td0,-$0008(a0)", 0x8128, 0xFFF8);
            RunTest("or.w\td0,-$0008(a0)", 0x8168, 0xFFF8);
            RunTest("or.l\td0,-$0008(a0)", 0x81A8, 0xFFF8);
        }

        [Test]
        public void M68kdis_lsl_w()
        {
            RunTest("lsl.w\t#$08,d0", 0xE148);
        }

        [Test]
        public void M68kdis_subq_b()
        {
            RunTest("subq.b\t#$04,d6", 0x5906);
            RunTest("subq.b\t#$08,(a2)", 0x5112);
        }

        [Test]
        public void M68kdis_subq_w()
        {
            RunTest("subq.w\t#$07,-(a6)", 0x5F66);
            RunTest("subq.w\t#$01,($34,a0,d1.w)", 0x5370, 0x1034);
        }

        [Test]
        public void M68kdis_subq_l()
        {
            RunTest("subq.l\t#$06,$12345678", 0x5DB9, 0x1234, 0x5678);
            RunTest("subq.l\t#$01,$1234(a1)", 0x53A9, 0x1234);
        }

        [Test]
        public void M68kdis_subi()
        {
            RunTest("subi.b\t#$34,d0", 0x0400, 0x1234);
            RunTest("subi.w\t#$1234,d0", 0x0440, 0x1234);
            RunTest("subi.l\t#$12345678,d0", 0x0480, 0x1234, 0x5678);
        }

        [Test]
        public void M68kdis_sub_re()
        {
            RunTest("sub.l\td0,(a7)+", 0x919F);
        }

        [Test]
        public void M68kdis_rts()
        {
            RunTest("rts", 0x4E75);
        }

        [Test]
        public void M68kdis_asr_r()
        {
            RunTest("asr.b\td3,d4", 0xE624);
            RunTest("asr.w\td3,d4", 0xE664);
            RunTest("asr.l\td3,d4", 0xE6A4);
            RunTest("asr.w\t-(a5)", 0xE0E5, 1234);
        }

        [Test]
        public void M68kdis_asl()
        {
            RunTest("asl.b\t#$08,d1", 0xe101);
            RunTest("asl.w\t#$08,d1", 0xe141);
            RunTest("asl.l\t#$08,d1", 0xe181);
            RunTest("asl.b\td0,d1", 0xe121);
            RunTest("asl.w\td0,d1", 0xe161);
            RunTest("asl.l\td0,d1", 0xe1a1);
            RunTest("asl.w\t(a1)", 0xe1D1);
        }

        [Test]
        public void M68kdis_subx_mm()
        {
            RunTest("subx.b\t-(a1),-(a0)", 0x9109);
            RunTest("subx.w\t-(a1),-(a0)", 0x9149);
            RunTest("subx.l\t-(a1),-(a0)", 0x9189);
        }

        [Test]
        public void M68kdis_subx_rr()
        {
            RunTest("subx.b\td1,d0", 0x9101);
            RunTest("subx.w\td1,d0", 0x9141);
            RunTest("subx.l\td1,d3", 0x9781);
        }

        [Test]
        public void M68kdis_addx_mm()
        {
            RunTest("addx.b\t-(a1),-(a0)", 0xD109);
            RunTest("addx.w\t-(a1),-(a0)", 0xD149);
            RunTest("addx.l\t-(a1),-(a0)", 0xD189);
        }

        [Test]
        public void M68kdis_addx_rr()
        {
            RunTest("addx.b\td1,d0", 0xD101);
            RunTest("addx.w\td1,d0", 0xD141);
            RunTest("addx.l\td1,d3", 0xD781);
        }

        [Test]
        public void M68kdis_addi()
        {
            RunTest("addi.b\t#$34,(a1)", 0x0611, 0x1234);
            RunTest("addi.w\t#$1234,(a1)", 0x0651, 0x1234);
            RunTest("addi.l\t#$12345678,(a1)", 0x0691, 0x1234, 0x5678);
        }

        [Test]
        public void M68kdis_lsl()
        {
            RunTest("lsl.l\t#$03,d2", 0xE78A);
            RunTest("lsl.b\td1,d3", 0xE32B);
            RunTest("lsl.w\td1,d4", 0xE36C);
            RunTest("lsl.l\td1,d4", 0xE3AC);
            RunTest("lsl.w\t(a1)", 0xE3D1);
        }

        [Test]
        public void M68kdis_bcc()
        {
            RunTest("bra\t$0FFFFFFE", 0x60FC);
            RunTest("bhi\t$0FFFFFFE", 0x62FC);
            RunTest("bcc\t$0FFFFFFE", 0x6400, 0xFFFC);
            RunTest("bge\t$0FFFFFFE", 0x6CFF, 0xFFFF, 0xFFFC);
        }

        [Test]
        public void M68kdis_ext()
        {
            RunTest("ext.w\td4", 0x4884);
            RunTest("ext.l\td4", 0x48C4);
            RunTest("extb.l\td4", 0x49C4);
        }

        [Test]
        public void M68kdis_lsr()
        {
            RunTest("lsr.w\t#$08,d1", 0xE049);
            RunTest("lsr.l\t#$08,d1", 0xE089);
            RunTest("lsr.b\td0,d1", 0xE029);
            RunTest("lsr.w\td0,d1", 0xE069);
            RunTest("lsr.l\td0,d1", 0xE0A9);
            RunTest("lsr.w\t(a1)", 0xE2D1);
        }

        [Test]
        public void M68kdis_unlk()
        {
            RunTest("unlk\ta7", 0x4E5F);
        }

        [Test]
        public void M68kdis_cmpm()
        {
            RunTest("cmpm.b\t(a2)+,(a3)+", 0xB70A);
            RunTest("cmpm.w\t(a2)+,(a3)+", 0xB74A);
            RunTest("cmpm.l\t(a2)+,(a3)+", 0xB78A);
        }

        [Test]
        public void M68kdis_bsr()
        {
            RunTest("bsr\t$0FFFFFFE", 0x61FC);
            RunTest("bsr\t$0FFFFFFE", 0x6100, 0xFFFC);
            RunTest("bsr\t$0FFFFFFE", 0x61FF, 0xFFFF, 0xFFFC);
        }

        [Test]
        public void M68kdis_pea()
        {
            RunTest("pea\t$0004(a2)", 0x486A, 0x0004);
        }

        [Test]
        public void M68kdis_dbra()
        {
            RunTest("dbra\td2,$0FFFFFFC", 0x51CA, 0xFFFA);
        }

        [Test]
        public void M68kdis_dble()
        {
            RunTest("dble\td7,$0FFFFFFC", 0x5FCF, 0xFFFA);
        }

        [Test]
        public void M68kdis_link()
        {
            RunTest("link\ta2,#$0004", 0x4E52, 0x0004);
            RunTest("link\ta3,#$00050004", 0x480B, 0x0005, 0004);
        }

        [Test]
        public void M68kdis_divs()
        {
            RunTest("divs.w\td4,d7", 0x8FC4);
        }

        [Test]
        public void M68kdis_bset_s()
        {
            RunTest("bset\t#$001F,d0", 0x08C0, 0x001F);
            RunTest("bset\td4,(a7)", 0x09D7);
        }

        [Test]
        public void M68kdis_movem_2()
        {
            RunTest("movem.l\t(a7)+,d2/a2-a3", 0x4CDF, 0x0C04);
        }

        [Test]
        public void M68kdis_movem_3()
        {
            RunTest("movem.l\t$0030(a7),d0-d1", 0x4cef, 0x0003, 0x0030);
        }

        [Test]
        public void M68kdis_lea_pc()
        {
            RunTest("lea\t$0014(pc),a2", 0x45FA, 0x0012);
        }

        [Test]
        public void M68kdis_tst()
        {
            RunTest("tst.l\t$0126(pc)", 0x4ABA, 0x0124);
            RunTest("tst.l\t(a3,d0)", 0x4AB3, 0x0800);
        }

        [Test]
        public void M68kdis_btst()
        {
            RunTest("btst.w\t#$0000,($34,a0,d0.w)", 0x0830, 0x0000, 0x0034);
        }

        [Test]
        public void M68kdis_oril()
        {
            RunTest("ori.l\t#$00000004,$0048(a7)", 0x00AF, 0x0000, 0x0004, 0x0048);
        }

        [Test]
        public void M68kdis_move_to_ccr()
        {
            RunTest("move.w\td3,ccr", 0x44c3);
        }

        [Test]
        public void M68kdis_move_fr_ccr()
        {
            RunTest("move.w\tccr,(a3)", 0x42d3, 0x0000);
        }

        [Test]
        public void M68kdis_bclr_r()
        {
            RunTest("bclr.l\td2,d1", 0x0581);
        }

        [Test]
        public void M68kdis_bclr_s()
        {
            RunTest("bclr.l\t#$05,d4", 0x0884, 5);
        }

        [Test]
        public void M68kdis_divu()
        {
            RunTest("divu.w\t(a4),d5", 0x8AD4);
        }

        [Test]
        public void M68kdis_exg()
        {
            RunTest("exg\td1,d2", 0xc342);
            RunTest("exg\ta2,a4", 0xc54C);
            RunTest("exg\td3,a6", 0xc78E);
        }

        [Test]
        public void M68kdis_ror_q()
        {
            RunTest("ror.b\t#$01,d4", 0xE21C);
        }

        [Test]
        public void M68kdis_ror_reg()
        {
            RunTest("ror.b\td5,d3", 0xEA3B);
        }

        [Test]
        public void M68kdis_ror_ea()
        {
            RunTest("ror.l\t(a4)", 0xE6D4);
        }

        [Test]
        public void M68kdis_rte()
        {
            RunTest("rte", 0x4E73);
        }

        [Test]
        public void M68kdis_tst_i()
        {
            RunTest("tst.l\t#$12345678", 0x4ABC, 0x1234, 0x5678);
        }

        [Test]
        public void M68kdis_tst_w()
        {
            RunTest("tst.w\t(a5)", 0x4A55);
        }

        [Test]
        public void M68kdis_sbcd()
        {
            RunTest("sbcd\t-(a2),-(a1)", 0x830A);
        }

        [Test]
        public void M68kdis_rtd()
        {
            RunTest("rtd\t#$0012", 0x4E74, 0x0012);
        }

        [Test]
        public void M68kdis_address_mode()
        {
            RunTest("move.l\t(-$04,a2,d0.w*4),d2", 0x2432, 0x04fc);
        }

        [Test]
        public void M68kdis_movem()
        {
            RunTest("movem.w\t$0004000A,d0-d1", 0x4CB9, 0x0003, 0x0004, 0x000A);    // move.l\t(-04,a2,d0*2),d2",
        }

        [Test]
        public void M68kdis_move_addr()
        {
            RunTest("move.l\td0,$00FF0F08", 0x23C0, 0x00FF, 0x0F08);    // move.l d0,(dword_FF0F08).l
        }

        [Test]
        public void M68kdis_fmovem()
        {
            RunTest("fmovem.x\tfp2,-(a7)", 0xF227, 0xE004);
        }

        [Test]
        public void M68kdis_fmovem_to_reg()
        {
            RunTest("fmovem.x\t-$0018(a6),fp2", 0xF22E, 0xD020, 0xFFE8); 
        }

        [Test]
        public void M68kdis_fmoved()
        {
            RunTest("fmove.d\tfp0,-$0008(a6)", 0xF22E, 0x7400, 0xFFF8);
        }

        [Test]
        public void M68kdis_fdivd()
        {
            RunTest("fdiv.d\t#6.0,fp0", 0xF23C, 0x5420, 0x4018, 0x0000, 0x0000, 0x0000);
        }

        [Test]
        public void M68kdis_fbnge()
        {
            RunTest("fbnge\t$100000E2", 0xF29C, 0x00E0);  
        }

        [Test]
        public void M68kdis_fbcc_illegalEncoding()
        {
            // This is an fbcc instruction, which uses an encoding
            // which is not valid with a 68k FPU; it should
            // decode to an illegal instruction
            RunTest("illegal", 0xF2BC, 0x00E0);
        }

        [Test]
        public void M68kdis_cmpi_pc_relative_indexing()
        {
            RunTest("cmpi.b\t#$04,($2C,pc,d0.w)", 0x0C3B, 0x0004, 0x0028);
        }

        [Test]
        public void M68kdis_move_pc_relative_indexing()
        {
            RunTest("movea.l\t(0000025C,pc),a0", 0x207B, 0x0170, 0x0000, 0x025C);
        }

        [Test]
        public void M68kdis_cinv_invalid()
        {
            RunTest("cinv", 0xF400);
        }

        [Test]
        public void M68kdis_bfins()
        {
            RunTest("bfins\td3,d5,{d3:#$00000002}", 0xEFC5, 0x38C2);
        }

        [Test]
        public void M68kdis_fsave()
        {
            RunTest("fsave\t(a0)", 0xF310);
        }

        [Test]
        public void M68kdis_tst_i_16()
        {
            RunTest("tst.w\t#$1234", 0x4A7C, 0x1234);
        }

        [Test]
        public void M68kdis_movep()
        {
            RunTest("movep.w\t$1234(a2),d2", 0x050A, 0x1234);
            RunTest("movep.l\t$1234(a2),d2", 0x054A, 0x1234);
            RunTest("movep.w\td2,$1234(a2)", 0x058A, 0x1234);
            RunTest("movep.l\td2,$1234(a2)", 0x05CA, 0x1234);
        }

        [Test]
        public void M68kdis_eori_ccr()
        {
            RunTest("eori.b\t#$80,ccr", 0x0A3C, 0x0080);
        }
    }
}
