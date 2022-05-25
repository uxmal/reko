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
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture] 
    public class X86Disassembler_32bit_Tests : DisassemblerTestBase<X86Instruction>
    {
        private X86ArchitectureFlat32 arch;
        private Address addr;

        public X86Disassembler_32bit_Tests()
        {
            this.arch = new X86ArchitectureFlat32(CreateServiceContainer(), "x86-protected-32", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x10000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        private void AssertCode32(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void AssertCode32(string sExp, params byte[] bytes)
        {
            var instr = base.DisassembleBytes(bytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void X86dis_andps()
        {
            AssertCode32("andps\txmm0,[edx+42h]", 0x0F, 0x54, 0x42, 0x42);
        }

        [Test]
        public void X86dis_andnps()
        {
            AssertCode32("andnps\txmm0,[edx+42h]", 0x0F, 0x55, 0x42, 0x42);
        }

        [Test]
        public void X86dis_bsf()
        {
            AssertCode32("bsf\teax,[edx+42h]", 0x0F, 0xBC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_btc()
        {
            AssertCode32("btc\t[edx+42h],eax", "0FBB4242");
        }

        [Test]
        public void X86Dis_btr()
        {
            AssertCode32("btr\tebx,0h", 0x0F, 0xBA, 0xF3, 0x00);
        }

        [Test]
        public void X86Dis_bts()
        {
            AssertCode32("bts\t[esp],eax", 0x0F, 0xAB, 0x04, 0x24, 0xEB);
        }

        [Test]
        public void X86dis_bswap()
        {
            AssertCode32("bswap\teax", 0x0F, 0xC8); ;		// bswap eax
            AssertCode32("bswap\tedi", 0x0F, 0xCF);       //  bswap edi
        }

        [Test]
        public void X86Dis_call32()
        {
            var instr = DisassembleBytes(0xE9, 0x78, 0x56, 0x34, 012);
            var addrOp = (AddressOperand) instr.Operands[0];
            Assert.AreEqual("0C35567D", addrOp.ToString());
        }

        // o64
        [Test]
        public void X86dis_clts()
        {
            AssertCode32("clts", 0x0F, 0x06);
        }

        [Test]
        public void X86Dis_cmovnz()
        {
            AssertCode32("cmovnz\teax,ecx", 0x0F, 0x45, 0xC1);
        }

        [Test]
        public void X86dis_cmpps()
        {
            AssertCode32("cmpps\txmm0,[edx+42h],8h", 0x0F, 0xC2, 0x42, 0x42, 0x08);
        }

        [Test]
        public void X86Dis_cmpxchg()
        {
            AssertCode32("cmpxchg\t[edx],ecx", 0x0f, 0xb1, 0x0a, 0x85, 0xc0, 0x0f, 0x85, 0xdc);
        }

        [Test]
        public void X86dis_comiss()
        {
            AssertCode32("comiss\txmm0,dword ptr [edx+42h]", 0x0F, 0x2F, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_cpuid()
        {
            AssertCode32("cpuid", 0x0F, 0xA2);
        }

        [Test]
        public void X86dis_cvtdq2ps()
        {
            AssertCode32("cvtdq2ps\txmm0,[edx+42h]", 0x0F, 0x5B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvtps2pd()
        {
            AssertCode32("cvtps2pd\txmm0,[edx+42h]", 0x0F, 0x5A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_cvttps2pi()
        {
            AssertCode32("cvtps2pi\tmm0,xmmword ptr [edx+42h]", 0x0F, 0x2D, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_cvttsd2si()
        {
            AssertCode32("cvttsd2si\teax,xmm3", "F2 0F 2C C3");
        }

        [Test]
        public void X86Dis_cvtss2si()
        {
            AssertCode32("cvtss2si\teax,xmm3", "F30F2DC3");
        }

        [Test]
        public void X86Dis_cwd32()
        {
            AssertCode32("cdq", "99");
            AssertCode32("cwd", "66 99");
        }

        [Test]
        public void X86Dis_cwde32()
        {
            AssertCode32("cwde", "98");
            AssertCode32("cbw", "66 98");
        }

        [Test]
        public void X86Dis_DirectOperand32()
        {
            var instr = DisassembleBytes(0x8B, 0x15, 0x22, 0x33, 0x44, 0x55, 0x66);
            Assert.AreEqual("mov\tedx,[55443322h]", instr.ToString()); 
            var memOp = (MemoryOperand) instr.Operands[1];
            Assert.AreEqual("ptr32", memOp.Offset.DataType.ToString());
        }

        [Test]
        public void X86dis_emms()
        {
            AssertCode32("emms", 0x0F, 0x77);
        }

        [Test]
        public void X86Dis_endbr()
        {
            AssertCode32("endbr32", "F3 0F 1E FB");
            AssertCode32("endbr64", "F3 0F 1E FA");
        }

        [Test]
        public void X86dis_fclex()
        {
            AssertCode32("fclex", 0xDB, 0xE2);
        }

        [Test]
        public void X86dis_fcmovb()
        {
            AssertCode32("fcmovb\tst(0),st(1)", 0xDA, 0xC1);
        }

        [Test]
        public void X86dis_fcmove()
        {
            AssertCode32("fcmove\tst(0),st(1)", 0xDA, 0xC9);
        }

        [Test]
        public void X86dis_fcmovbe()
        {
            AssertCode32("fcmovbe\tst(0),st(1)", 0xDA, 0xD1);
        }

        [Test]
        public void X86dis_fcmovne()
        {
            AssertCode32("fcmovne\tst(0),st(1)", 0xDB, 0xC9);
        }

        [Test]
        public void X86dis_fcmovnbe()
        {
            AssertCode32("fcmovnbe\tst(0),st(1)", 0xDB, 0xD1);
        }

        [Test]
        public void X86dis_fcmovnu()
        {
            AssertCode32("fcmovnu\tst(0),st(1)", 0xDB, 0xD9);
        }

        [Test]
        public void X86dis_fcmovu()
        {
            AssertCode32("fcmovu\tst(0),st(1)", 0xDA, 0xD9);
        }

        [Test]
        public void X86dis_fcomip()
        {
            AssertCode32("fcomip\tst(0),st(2)", 0xDF, 0xF2);
        }

        [Test]
        public void X86dis_ffree()
        {
            AssertCode32("ffree\tst(2)", 0xDD, 0xC2);
        }

        [Test]
        public void X86dis_fild_i16()
        {
            AssertCode32("fild\tword ptr [eax+42h]", 0xDF, 0x40, 0x42);
        }

        [Test]
        public void X86dis_finit()
        {
            AssertCode32("fninit", 0xDB, 0xE3);
        }

        [Test]
        public void X86dis_fisttp()
        {
            AssertCode32("fisttp\tdword ptr [eax]", 0xDB, 0x08);
        }

        [Test]
        public void X86dis_fisttp_int16()
        {
            AssertCode32("fisttp\tword ptr [eax+42h]", 0xDF, 0x48, 0x42);
        }

        [Test]
        public void X86dis_fisttp_i64()
        {
            AssertCode32("fisttp\tqword ptr [eax+42h]", 0xDD, 0x48, 0x42);
        }

        [Test]
        public void X86dis_fld_real80()
        {
            AssertCode32("fld\ttword ptr [eax]", 0xDB, 0x28);
        }

        [Test]
        public void X86dis_fucom()
        {
            AssertCode32("fucom\tst(5),st(0)", 0xDD, 0xE5);
        }

        [Test]
        public void X86dis_fucomi()
        {
            AssertCode32("fucomi\tst(0),st(3)", 0xDB, 0xEB);
        }

        [Test]
        public void X86dis_fucomip()
        {
            AssertCode32("fucomip\tst(0),st(1)", 0xDF, 0xE9);
        }

        [Test]
        public void X86dis_fucomp()
        {
            AssertCode32("fucomp\tst(2)", 0xDD, 0xEA);
        }

        [Test]
        public void X86dis_fucompp()
        {
            AssertCode32("fucompp", 0xDA, 0xE9);
        }

        [Test]
        public void X86dis_getsec()
        {
            AssertCode32("getsec", 0x0F, 0x37, 0x42, 0x42);
        }

        // o64
        [Test]
        public void X86dis_invd()
        {
            AssertCode32("invd", 0x0F, 0x08);
        }

        [Test]
        public void X86Dis_instr_tool_long_prefixes()
        {
            AssertCode32("xor\teax,fs:[eax]", "64 64 64 64  64 64 64 64  64 64 64 64  64 33 00");
            AssertCode32("illegal",           "64 64 64 64  64 64 64 64  64 64 64 64  64 64 33 00");
        }

        [Test]
        public void X86Dis_InvalidKeptStateRegression()
        {
            var asm = new X86TextAssembler(arch);
            var lr = asm.AssembleFragment(
                Address.Ptr32(0x01001000),

                "db 0xf2, 0x0f, 0x70, 0x00, 0x00\r\n" +
                "db 0xf3, 0x0f, 0x70, 0x00, 0x00\r\n");

            /* Before (incorrect):
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  
             *  
             * After (correct):
             *  pshuflw xmm0, dqword ptr ds:[eax], 0
             *  pshufhw xmm0, dqword ptr ds:[eax], 0
             */

            var bmem = lr.SegmentMap.Segments.Values.First().MemoryArea;
            var dasm = arch.CreateDisassemblerImpl(bmem.CreateLeReader(0));
            var instructions = dasm.Take(2).ToArray();

            X86Instruction one = instructions[0];
            X86Instruction two = instructions[1];

            Assert.AreEqual(Mnemonic.pshuflw, one.Mnemonic);
            Assert.AreEqual("xmm0", one.Operands[0].ToString());
            Assert.AreEqual("[eax]", one.Operands[1].ToString());

            Assert.AreEqual(Mnemonic.pshufhw, two.Mnemonic);
            Assert.AreEqual("xmm0", two.Operands[0].ToString());
            Assert.AreEqual("[eax]", two.Operands[1].ToString());
        }

        [Test]
        public void X86Dis_jcxz()
        {
            AssertCode32("jecxz\t10006h", "E3 04");
        }

        [Test]
        public void X86dis_lar()
        {
            AssertCode32("lar\teax,word ptr [edx+42h]", 0x0F, 0x02, 0x42, 0x42);
        }

        [Test(Description = "Very large 32-bit offsets can be treated as negative offsets")]
        public void X86Dis_LargeNegativeOffset()
        {
            AssertCode32("mov\tesi,[eax-0FFF0h]", 0x8B, 0xB0, 0x10, 0x00, 0xFF, 0xFF);
            AssertCode32("mov\tesi,[eax+0FFFF0000h]", 0x8B, 0xB0, 0x00, 0x00, 0xFF, 0xFF);
        }

        [Test]
        public void X86Dis_lldt()
        {
            AssertCode32("lldt\tax", 0x0F, 0x00, 0xD0);
        }

        [Test]
        public void X86dis_lsl()
        {
            AssertCode32("lsl\teax,word ptr [edx+42h]", 0x0F, 0x03, 0x42, 0x42);
        }

        [Test]
        public void X86dis_maskmovq()
        {
            AssertCode32("maskmovq\tmm0,[edx+42h]", 0x0F, 0xF7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_minps()
        {
            AssertCode32("minps\txmm0,[edx+42h]", 0x0F, 0x5D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_modrm()
        {
            AssertCode32("mov\tecx,[eax]", "8B 08");
            AssertCode32("mov\tecx,[12345678h]", "8B 0D 78 56 34 12");
            AssertCode32("mov\tecx,[edi]", "8B 0F");
            AssertCode32("mov\tecx,[eax+42h]", "8B 48 42");
            AssertCode32("mov\tecx,[eax+12345678h]", "8B 88 78 56 34 12");
        }

        [Test]
        public void X86dis_modrm_sib_mod0()
        {
            AssertCode32("mov\tecx,[ecx+eax*2]", "8B 0C 41");
            AssertCode32("mov\tecx,[12345678h+eax*2]", "8B 0C 45 78 56 34 12");
            AssertCode32("mov\tecx,[edi+eax*2]", "8B 0C 47");

            AssertCode32("mov\tecx,[ecx+eiz*2]", "8B 0C 61");
            AssertCode32("mov\tecx,[12345678h+eiz*2]", "8B 0C 65 78 56 34 12");
            AssertCode32("mov\tecx,[edi+eiz*2]", "8B 0C 67");
        }

        [Test]
        public void X86dis_modrm_sib_mod1()
        {
            AssertCode32("mov\tecx,[ecx+eax*2-8h]", "8B 4C 41 F8");
            AssertCode32("mov\tecx,[ebp+eax*2-8h]", "8B 4C 45 F8");
            AssertCode32("mov\tecx,[edi+eax*2-8h]", "8B 4C 47 F8");

            AssertCode32("mov\tecx,[ecx+eiz*2-8h]", "8B 4C 61 F8");
            AssertCode32("mov\tecx,[ebp+eiz*2-8h]", "8B 4C 65 F8");
            AssertCode32("mov\tecx,[edi+eiz*2-8h]", "8B 4C 67 F8");
        }

        [Test]
        public void X86Dis_more2()
        {
            AssertCode32("movlhps\txmm3,xmm3", 0x0f, 0x16, 0xdb);
            AssertCode32("pshuflw\txmm3,xmm3,0h", 0xf2, 0x0f, 0x70, 0xdb, 0x00);
        }

        [Test]
        public void X86Dis_more3()
        {
            AssertCode32("stmxcsr\tdword ptr [ebp-0Ch]", 0x0f, 0xae, 0x5d, 0xf4);
            AssertCode32("palignr\txmm3,xmm1,0h", 0x66, 0x0f, 0x3a, 0x0f, 0xd9, 0x00);
            AssertCode32("movq\tqword ptr [edi],xmm1", 0x66, 0x0f, 0xd6, 0x0f);
            AssertCode32("ldmxcsr\tdword ptr [ebp+8h]", 0x0F, 0xAE, 0x55, 0x08);
            AssertCode32("pcmpistri\txmm0,[edi-10h],40h", 0x66, 0x0F, 0x3A, 0x63, 0x47, 0xF0, 0x40);
        }

        [Test]
        public void X86Dis_movd_32()
        {
            AssertCode32("movd\tdword ptr [esi],mm1", 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tdword ptr [esi],xmm1", 0x66, 0x0F, 0x7E, 0x0E);
            AssertCode32("movq\txmm1,qword ptr [esi]", 0xF3, 0x0F, 0x7E, 0x0E);
            AssertCode32("movd\tesi,mm1", 0x0F, 0x7E, 0xCE);
            AssertCode32("movd\tesi,xmm1", 0x66, 0x0F, 0x7E, 0xCE);
            AssertCode32("movq\txmm1,xmm6", 0xF3, 0x0F, 0x7E, 0xCE);
        }

        [Test]
        public void X86Dis_movdqa()
        {
            AssertCode32("movdqa\txmm0,[esi]", 0x66, 0x0F, 0x6F, 0x06, 0x12, 0x34, 0x56);
        }

        [Test]
        public void X86dis_movlps()
        {
            AssertCode32("movlps\txmm0,qword ptr [edx+42h]", 0x0F, 0x12, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_from_control_Reg()
        {
            AssertCode32("mov\tedx,cr0", 0x0F, 0x20, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_debug_reg()
        {
            AssertCode32("mov\tedx,dr0", 0x0F, 0x21, 0x42, 0x42);
        }

        [Test]
        public void X86dis_mov_control_reg()
        {
            AssertCode32("mov\tcr0,edx", 0x0F, 0x22, 0x42);
        }

        [Test]
        public void X86dis_mov_to_debug_reg()
        {
            AssertCode32("mov\tdr0,edx", 0x0F, 0x23, 0x42);
        }

        [Test]
        public void X86dis_movhpd()
        {
            AssertCode32("movhps\tqword ptr [edx+42h],xmm0", 0x0F, 0x17, 0x42, 0x42);
            AssertCode32("movhpd\tqword ptr [edx+42h],xmm0", 0x66, 0x0F, 0x17, 0x42, 0x42);
        }

        // v
        [Test]
        public void X86dis_movmskps()
        {
            AssertCode32("movmskps\teax,xmm2", 0x0F, 0x50, 0x42, 0x42);
        }

        [Test]
        public void X86dis_movnti()
        {
            AssertCode32("movnti\t[edx+42h],eax", 0x0F, 0xC3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_movntq()
        {
            AssertCode32("movntq\t[edx+42h],mm0", 0x0F, 0xE7, 0x42, 0x42);
        }

        [Test]
        public void X86dis_movntps()
        {
            AssertCode32("movntps\t[edx+42h],xmm0", 0x0F, 0x2B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_orps()
        {
            AssertCode32("orps\txmm0,[edx+42h]", 0x0F, 0x56, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddb()
        {
            AssertCode32("paddb\tmm0,[edx+42h]", 0x0F, 0xFC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddd()
        {
            AssertCode32("paddd\tmm0,[edx+42h]", 0x0F, 0xFE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddw()
        {
            AssertCode32("paddw\tmm0,[edx+42h]", 0x0F, 0xFD, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsb()
        {
            AssertCode32("paddsb\tmm0,[edx+42h]", 0x0F, 0xEC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packssdw()
        {
            AssertCode32("packssdw\tmm0,dword ptr [edx+42h]", 0x0F, 0x6B, 0x42, 0x42);
        }

        [Test]
        public void X86dis_packuswb()
        {
            AssertCode32("packuswb\tmm0,dword ptr [edx+42h]", 0x0F, 0x67, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddsw()
        {
            AssertCode32("paddsw\tmm0,[edx+42h]", 0x0F, 0xED, 0x42, 0x42);
        }

        [Test]
        public void X86dis_paddusb()
        {
            AssertCode32("paddusb\tmm0,[edx+42h]", 0x0F, 0xDC, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pause()
        {
            AssertCode32("pause", 0xF3, 0x90);
        }

        [Test]
        public void X86dis_pavgb()
        {
            AssertCode32("pavgb\tmm0,[edx+42h]", 0x0F, 0xE0, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtb()
        {
            AssertCode32("pcmpgtb\tmm0,[edx+42h]", 0x0F, 0x64, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtw()
        {
            AssertCode32("pcmpgtw\tmm0,dword ptr [edx+42h]", 0x0F, 0x65, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpgtd()
        {
            AssertCode32("pcmpgtd\tmm0,dword ptr [edx+42h]", 0x0F, 0x66, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqb()
        {
            AssertCode32("pcmpeqb\txmm0,[eax]", 0x66, 0x0f, 0x74, 0x00);
        }

        [Test]
        public void X86dis_pcmpeqd()
        {
            AssertCode32("pcmpeqd\tmm0,[edx+42h]", 0x0F, 0x76, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pcmpeqw()
        {
            AssertCode32("pcmpeqw\tmm0,[edx+42h]", 0x0F, 0x75, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pextrw()
        {
            AssertCode32("pextrw\teax,mm2,42h", 0x0F, 0xC5, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_phsubsw()
        {
            AssertCode32("phsubsw\tmm0,mm2", 0x0F, 0x38, 0x07, 0xC2);
        }

        [Test]
        public void X86dis_pinsrw()
        {
            //$TODO check encoding; look in the Intel spec.
            AssertCode32("pinsrw\tmm0,edx", 0x0F, 0xC4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaddwd()
        {
            AssertCode32("pmaddwd\tmm0,[edx+42h]", 0x0F, 0xF5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxsw()
        {
            AssertCode32("pmaxsw\tmm0,[edx+42h]", 0x0F, 0xEE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmaxub()
        {
            AssertCode32("pmaxub\tmm0,[edx+42h]", 0x0F, 0xDE, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminsw()
        {
            AssertCode32("pminsw\tmm0,[edx+42h]", 0x0F, 0xEA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pminub()
        {
            AssertCode32("pminub\tmm0,[edx+42h]", 0x0F, 0xDA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmovmskb()
        {
            AssertCode32("pmovmskb\teax,mm2", 0x0F, 0xD7, 0x42);
        }

        [Test]
        public void X86dis_pmulhuw()
        {
            AssertCode32("pmulhuw\tmm0,[edx+42h]", 0x0F, 0xE4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmulhw()
        {
            AssertCode32("pmulhw\tmm0,[edx+42h]", 0x0F, 0xE5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmullw()
        {
            AssertCode32("pmullw\tmm0,[edx+42h]", 0x0F, 0xD5, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pmuludq()
        {
            AssertCode32("pmuludq\tmm0,[edx+42h]", 0x0F, 0xF4, 0x42, 0x42);
        }

        [Test]
        public void X86dis_por()
        {
            AssertCode32("por\tmm0,[edx+42h]", 0x0F, 0xEB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psadbw()
        {
            AssertCode32("psadbw\tmm0,[edx+42h]", 0x0F, 0xF6, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pslld()
        {
            AssertCode32("pslld\tmm0,[edx+42h]", 0x0F, 0xF2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psllq()
        {
            AssertCode32("psllq\tmm0,[edx+42h]", 0x0F, 0xF3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psllw()
        {
            AssertCode32("psllw\tmm0,[edx+42h]", 0x0F, 0xF1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psrad()
        {
            AssertCode32("psrad\tmm0,[edx+42h]", 0x0F, 0xE2, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psraw()
        {
            AssertCode32("psraw\tmm0,[edx+42h]", 0x0F, 0xE1, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psrlq()
        {
            AssertCode32("psrlq\tmm0,[edx+42h]", 0x0F, 0xD3, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubb()
        {
            AssertCode32("psubb\tmm0,[edx+42h]", 0x0F, 0xF8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubd()
        {
            AssertCode32("psubd\tmm0,[edx+42h]", 0x0F, 0xFA, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubq()
        {
            AssertCode32("psubq\tmm0,[edx+42h]", 0x0F, 0xFB, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubw()
        {
            AssertCode32("psubw\tmm0,[edx+42h]", 0x0F, 0xF9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsb()
        {
            AssertCode32("psubsb\tmm0,[edx+42h]", 0x0F, 0xE8, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubsw()
        {
            AssertCode32("psubsw\tmm0,[edx+42h]", 0x0F, 0xE9, 0x42, 0x42);
        }

        [Test]
        public void X86dis_psubusb()
        {
            AssertCode32("psubusb\tmm0,[edx+42h]", 0x0F, 0xD8, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_punpcklbw()
        {
            AssertCode32("punpcklbw\txmm1,xmm3", 0x66, 0x0f, 0x60, 0xcb);
        }

        [Test]
        public void X86dis_punpckldq()
        {
            AssertCode32("punpckldq\tmm0,dword ptr [edx+42h]", 0x0F, 0x62, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhbw()
        {
            AssertCode32("punpckhbw\tmm0,dword ptr [edx+42h]", 0x0F, 0x68, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhwd()
        {
            AssertCode32("punpckhwd\tmm0,dword ptr [edx+42h]", 0x0F, 0x69, 0x42, 0x42);
        }

        [Test]
        public void X86dis_punpckhdq()
        {
            AssertCode32("punpckhdq\tmm0,dword ptr [edx+42h]", 0x0F, 0x6A, 0x42, 0x42);
        }

        [Test]
        public void X86dis_pxor()
        {
            AssertCode32("pxor\tmm0,[edx+42h]", 0x0F, 0xEF, 0x42, 0x42);
            AssertCode32("pxor\tmm0,mm1", 0x0F, 0xEF, 0xC1);
        }

        [Test]
        public void X86Dis_rdtsc()
        {
            AssertCode32("rdtsc", 0x0F, 0x31);
        }

        [Test]
        public void X86dis_rep_prefix_to_ucomiss()
        {
            AssertCode32("illegal", 0xF2, 0x0F, 0x2E, 0x00);
        }

        // /1
        [Test]
        public void X86dis_permq()
        {
            AssertCode32("illegal", "0F 3A 00 42 42 06");
            AssertCode32("illegal", "66 0F 3A 00 42 42 06");
        }

        [Test]
        public void X86dis_prefetch()
        {
            AssertCode32("prefetchw\tdword ptr [edx+42h]", 0x0F, 0x0D, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rcpps()
        {
            AssertCode32("rcpps\txmm0,[edx+42h]", 0x0F, 0x53, 0x42, 0x42);
        }

 

        [Test]
        public void X86dis_rdmsr()
        {
            AssertCode32("rdmsr", 0x0F, 0x32, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rdpmc()
        {
            AssertCode32("rdpmc", 0x0F, 0x33, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rdtsc()
        {
            AssertCode32("rdtsc", 0x0F, 0x31, 0x42, 0x42);
        }

        [Test]
        public void X86dis_rsqrtps()
        {
            AssertCode32("rsqrtps\txmm0,[edx+42h]", 0x0F, 0x52, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_Scaled_Index()
        {
            AssertCode32("mov\tebx,[0000h+edi*2]", "8B 1C 7D 00 00 00 00");
        }

        [Test]
        public void X86dis_shufps()
        {
            AssertCode32("shufps\txmm0,[edx+42h],7h", "0FC6424207");
        }

        [Test]
        public void X86dis_sqrtps()
        {
            AssertCode32("sqrtps\txmm0,[edx+42h]", 0x0F, 0x51, 0x42, 0x42);
        }

        [Test]
        public void X86dis_str()
        {
            AssertCode32("str\teax", "0F 00 C8");
        }

        [Test]
        public void X86Dis_StringOps()
        {
            X86TextAssembler asm = new X86TextAssembler(arch);
            var lr = asm.AssembleFragment(
                Address.Ptr32(0x01001000),

                "movsb\r\n" +
                "movsw\r\n" +
                "movsd\r\n" +

                "scasb\r\n" +
                "scasw\r\n" +
                "scasd\r\n" +

                "cmpsb\r\n" +
                "cmpsw\r\n" +
                "cmpsd\r\n" +

                "lodsb\r\n" +
                "lodsw\r\n" +
                "lodsd\r\n" +

                "stosb\r\n" +
                "stosw\r\n" +
                "stosd\r\n");

            MemoryArea mem = lr.SegmentMap.Segments.Values.First().MemoryArea;
            var dasm = arch.CreateDisassembler(mem.CreateLeReader(0));
            var instr = dasm.Cast<X86Instruction>().Take(15).ToList();
            Assert.AreEqual(Mnemonic.movsb, instr[0].Mnemonic);
            Assert.AreEqual(Mnemonic.movs, instr[1].Mnemonic);
            Assert.AreEqual("word16", instr[1].dataWidth.Name);
            Assert.AreEqual(Mnemonic.movs, instr[2].Mnemonic);
            Assert.AreEqual("word32", instr[2].dataWidth.Name);

            Assert.AreEqual(Mnemonic.scasb, instr[3].Mnemonic);
            Assert.AreEqual(Mnemonic.scas, instr[4].Mnemonic);
            Assert.AreEqual("word16", instr[4].dataWidth.Name);
            Assert.AreEqual(Mnemonic.scas, instr[5].Mnemonic);
            Assert.AreEqual("word32", instr[5].dataWidth.Name);

            Assert.AreEqual(Mnemonic.cmpsb, instr[6].Mnemonic);
            Assert.AreEqual(Mnemonic.cmps, instr[7].Mnemonic);
            Assert.AreEqual("word16", instr[7].dataWidth.Name);
            Assert.AreEqual(Mnemonic.cmps, instr[8].Mnemonic);
            Assert.AreEqual("word32", instr[8].dataWidth.Name);

            Assert.AreEqual(Mnemonic.lodsb, instr[9].Mnemonic);
            Assert.AreEqual(Mnemonic.lods, instr[10].Mnemonic);
            Assert.AreEqual("word16", instr[10].dataWidth.Name);
            Assert.AreEqual(Mnemonic.lods, instr[11].Mnemonic);
            Assert.AreEqual("word32", instr[11].dataWidth.Name);

            Assert.AreEqual(Mnemonic.stosb, instr[12].Mnemonic);
            Assert.AreEqual(Mnemonic.stos, instr[13].Mnemonic);
            Assert.AreEqual("word16", instr[13].dataWidth.Name);
            Assert.AreEqual(Mnemonic.stos, instr[14].Mnemonic);
            Assert.AreEqual("word32", instr[14].dataWidth.Name);
        }



        [Test]
        public void X86dis_syscall()
        {
            AssertCode32("illegal", 0x0F, 0x05, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sysenter()
        {
            AssertCode32("sysenter", 0x0F, 0x34, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sysexit()
        {
            AssertCode32("sysexit", 0x0F, 0x35, 0x42, 0x42);
        }

        [Test]
        public void X86dis_sysret()
        {
            AssertCode32("illegal", 0x0F, 0x07);
        }

        [Test]
        public void X86dis_vmread()
        {
            AssertCode32("vmread\t[edx+42h],eax", 0x0F, 0x78, 0x42, 0x42);
        }

        [Test]
        public void X86dis_vmwrite()
        {
            AssertCode32("vmwrite\teax,[edx+42h]", 0x0F, 0x79, 0x42, 0x42);
        }

        [Test]
        public void X86dis_vpmovsxbw()
        {
            AssertCode32("illegal", 0x0F, 0x38, 0x30, 0x42, 0x42);
            AssertCode32("pmovzxbw\txmm0,qword ptr [edx+42h]", 0x66, 0x0F, 0x38, 0x30, 0x42, 0x42);
        }

        [Test]
        public void X86dis_ud2()
        {
            AssertCode32("ud2", 0x0F, 0x0B);
        }

        [Test]
        public void X86dis_unpcklpd()
        {
            AssertCode32("unpcklps\txmm0,[edx+42h]", 0x0F, 0x14, 0x42, 0x42);
            AssertCode32("unpcklpd\txmm0,[edx+42h]", 0x66, 0x0F, 0x14, 0x42, 0x42);
        }

        [Test]
        public void X86dis_wbinvd()
        {
            AssertCode32("wbinvd", 0x0F, 0x09);
        }

        [Test]
        public void X86dis_wrmsr()
        {
            AssertCode32("wrmsr", 0x0F, 0x30, 0x42, 0x42);
        }

        [Test]
        public void X86Dis_xadd()
        {
            AssertCode32("xadd\tedx,eax", 0x0f, 0xC1, 0xC2);
        }

        [Test]
        public void X86Dis_xgetbv()
        {
            AssertCode32("xgetbv", 0x0F, 0x01, 0xD0);
        }

        [Test]
        public void X86Dis_xlat32()
        {
            var instr = DisassembleBytes(0xD7);
            Assert.AreEqual("xlat", instr.ToString());
            Assert.AreEqual(PrimitiveType.Byte, instr.dataWidth);
            Assert.AreEqual(PrimitiveType.Word32, instr.addrWidth);
        }

        [Test]
        public void X86dis_xorpd()
        {
            AssertCode32("xorpd\txmm0,xmm0", 0x66, 0x0F, 0x57, 0xC0);
        }

        [Test]
        public void X86dis_xorps()
        {
            AssertCode32("xorps\txmm0,xmm0", 0x0F, 0x57, 0xC0);
        }

        [Test]
        public void X86Dis_xsave()
        {
            AssertCode32("xsave\t[edi]", 0x0f, 0xae, 0x27);
        }
    }
}
