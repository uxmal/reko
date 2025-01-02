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
using Reko.Arch.Angstrem;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Angstrem
{
    [TestFixture]
    public class KR1878DisassemblerTests : DisassemblerTestBase<KR1878Instruction>
    {
        private readonly KR1878Architecture arch;
        private readonly Address addrLoad;

        public KR1878DisassemblerTests()
        {
            this.arch = new KR1878Architecture(CreateServiceContainer(), "KR1878", new());
            this.addrLoad = Address.Ptr16(0x100);
        }
        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void KR1878Dis_adc()
        {
            AssertCode("adc\tr21", "3501");
        }

        [Test]
        public void KR1878Dis_addl()
        {
            AssertCode("addl\tr1,-01", "E133");
        }

        [Test]
        public void KR1878Dis_cmp()
        {
            AssertCode("cmp\tr10,r5", "AA08");
        }

        [Test]
        public void KR1878Dis_cst()
        {
            AssertCode("cst\t0F", "CF01");
        }

        [Test]
        public void KR1878Dis_ijmp()
        {
            AssertCode("ijmp", "0300");
        }

        [Test]
        public void KR1878Dis_ijsr()
        {
            AssertCode("ijsr", "0700");
        }

        [Test]
        public void KR1878Dis_jc()
        {
            AssertCode("jc\t03FF", "FFF3");
        }

        [Test]
        public void KR1878Dis_jmp()
        {
            AssertCode("jmp\t03FF", "FF83");
        }

        [Test]
        public void KR1878Dis_jnc()
        {
            AssertCode("jnc\t03FF", "FFE3");
        }

        [Test]
        public void KR1878Dis_jns()
        {
            AssertCode("jns\t03FF", "FFC3");
        }

        [Test]
        public void KR1878Dis_jnz()
        {
            AssertCode("jnz\t03FF", "FFB3");
        }

        [Test]
        public void KR1878Dis_js()
        {
            AssertCode("js\t03FF", "FFD3");
        }

        [Test]
        public void KR1878Dis_jsr()
        {
            AssertCode("jsr\t03FF", "FF93");
        }

        [Test]
        public void KR1878Dis_jz()
        {
            AssertCode("jz\t03FF", "FFA3");
        }

        [Test]
        public void KR1878Dis_ldr()
        {
            AssertCode("ldr\tsr2,B5", "AA25");
        }

        [Test]
        public void KR1878Dis_mfpr()
        {
            AssertCode("mfpr\tr10,sr5", "AA03");
        }

        [Test]
        public void KR1878Dis_mov()
        {
            AssertCode("mov\tr10,r21", "AA06");
        }

        [Test]
        public void KR1878Dis_movl()
        {
            AssertCode("movl\tr21,AA", "5555");
        }

        [Test]
        public void KR1878Dis_mtpr()
        {
            AssertCode("mtpr\tsr5,r10", "AA02");
        }

        [Test]
        public void KR1878Dis_neg()
        {
            AssertCode("neg\tr21", "5500");
        }

        [Test]
        public void KR1878Dis_nop()
        {
            AssertCode("nop", "0000");
        }

        [Test]
        public void KR1878Dis_not()
        {
            AssertCode("not\tr21", "7500");
        }

        [Test]
        public void KR1878Dis_pop()
        {
            AssertCode("pop\tr7", "1F00");
        }

        [Test]
        public void KR1878Dis_push()
        {
            AssertCode("push\tr7", "1700");
        }

        [Test]
        public void KR1878Dis_reset()
        {
            AssertCode("reset", "0200");
        }

        [Test]
        public void KR1878Dis_rlc()
        {
            AssertCode("rlc\tr21", "F500");
        }

        [Test]
        public void KR1878Dis_rrc()
        {
            AssertCode("rrc\tr21", "1501");
        }

        [Test]
        public void KR1878Dis_rti()
        {
            AssertCode("rti", "0D00");
        }

        [Test]
        public void KR1878Dis_rts()
        {
            AssertCode("rts", "0C00");
        }

        [Test]
        public void KR1878Dis_rts1()
        {
            AssertCode("rts1", "0F00");
        }

        [Test]
        public void KR1878Dis_sbc()
        {
            AssertCode("sbc\tr21", "5501");
        }

        [Test]
        public void KR1878Dis_shl()
        {
            AssertCode("shl\tr21", "9500");
        }

        [Test]
        public void KR1878Dis_shr()
        {
            AssertCode("shr\tr21", "B500");
        }

        [Test]
        public void KR1878Dis_shra()
        {
            AssertCode("shra\tr21", "D500");
        }

        [Test]
        public void KR1878Dis_sksp()
        {
            AssertCode("sksp", "0600");
        }

        [Test]
        public void KR1878Dis_sst()
        {
            AssertCode("sst\t0F", "8F01");
        }

        [Test]
        public void KR1878Dis_stop()
        {
            AssertCode("stop", "0800");
        }

        [Test]
        public void KR1878Dis_swap()
        {
            AssertCode("swap\tr21", "3500");
        }

        [Test]
        public void KR1878Dis_tdc()
        {
            AssertCode("tdc", "0500");
        }

        [Test]
        public void KR1878Dis_tof()
        {
            AssertCode("tof", "0400");
        }

        [Test]
        public void KR1878Dis_wait()
        {
            AssertCode("wait", "0100");
        }

    }
}
        /*

        Ï V åðåñûëêà M d O 0 c 000 01ss sssd ddd sr 3dst** - 0 -
Ñ P ðàâíåíèå C d M 0 C 000 10ss sssd ddd dst - src S, Z, 3S*****
Ñ D ëîæåíèå A d D 0 c 001 00ss sssd ddd dst + sr 3dst*****
Â B û÷èòàíèå S d U 0 c 000 11ss sssd ddd dst - sr 3dst*****
Ë D îãè÷åñêîå È A d N 0 c 001 01ss sssd ddd dst.AND.sr 3dst** 0 0 0
Ë R îãè÷åñêîå ÈËÈ O d 0 c 001 10ss sssd ddd dst .OR.sr 3dst** 0 0 0
È R ñêëþ÷àþùåå ÈËÈ X d O 0 c 001 11ss sssd ddd dst .XOR.sr 3dst** 0 0 0
Ë û èòåðíûå êîìàíäû
Ï L åðåñûëêà ëèòåðû M d OV 0 t 10c ñccc cccd ddd cons 3dst** - 0 -
Ñ L ðàâíåíèå ñ ëèòåðîé C d MP 0 C 11c ñccc cccd ddd dst - const S, Z, 3RS*****
Ñ L ëîæåíèå ñ ëèòåðîé A d DD 0 t 011 00cc cccd ddd dst + scons 3dst*****
Â L û÷èòàíèå ëèòåðû S d UB 0 t 010 11cc cccd ddd dst - scons 3dst*****
Ñ C áðîñ ðàçðÿäîâ B d I 0 t 010 10pc cccd ddd NOT(const).AND.ds 3dst** 0 0 0
Ó S ñòàíîâêà ðàçðÿäîâ B d I 0 t 011 10pc cccd ddd dst.OR.tcons 3dst** 0 0 0
È G íâåðñèÿ ðàçðÿäîâ B d T 0 t 011 11pc cccd ddd dst.XOR.tcons 3dst** 0 0 0
Ï T ðîâåðêà ðàçðÿäîâ B d T 0 Z 011 01pc cccd ddd dst.AND.tconst, S, 3RS** 0 0 0

    Î P áìåí òåòðàä S d WA 0000 0000 001d ddd dst(n)3dst(n+4) n<4
dst(n)3dst(n-4) * * 0 0 0
Ñ G ìåíà çíàêà N d E 0 t 000 0000 010d ddd - ds 3dst*****
Èíâåðñèÿ âñåõ
ðàçðÿäîâ N d OT 0 ) 000 0000 011d ddd NOT(dst 3dst** - 0 -
Ëîãè÷åñêèé ñäâèã
âëåâî S d HL 0000 0000 100d ddd dst(n)3dst(n+1), 03dst(0), dst(7)3C**** 0
Ëîãè÷åñêèé ñäâèã
âïðàâî S d HR 0000 0000 101d ddd dst(n+1)3dst(n), 03dst(7), dst(0)3C
0 * * 0 0
Àðèôìåòè÷åñêèé
ñäâèã âïðàâî S d HRA 0000 0000 110d ddd dst(n+1)3dst(n),dst(7)3dst(7), dst(0)3C*** 0 0
Öèêëè÷åñêèé ñäâèã
âëåâî R d LC 0000 0000 111d ddd dst(n)3dst(n+1),C3dst(0), dst(7)3C**** 0
Öèêëè÷åñêèé ñäâèã
âïðàâî R d RC 0000 0001 000d ddd dst(n+1)3dst(n), C3dst(7), dst(0)3C*** 0 0
Ñëîæåíèå ñ
ïåðåíîñîì A d DC 0 C 000 0001 001d ddd dst + 3dst*****
Â C û÷èòàíèå ïåðåíîñà S d B 0 C 000 0001 010d ddd dst - 3dst*****

            Çàãðóçêà ñëóæåáíûõ
ðåãèñòðîâ L n DR 0 t 010 0ccc cccc cnn cons 3reg - - - - -
Çàïèñü â ñëóæåáíûå
ðåãèñòðû M s TPR 0 c 000 0010 nnns sss sr 3reg - - - - -
×òåíèå ñëóæåáíûõ
ðåãèñòðîâ M d FPR 0 g 000 0011 nnnd ddd re 3dst - - - - -
Ç H àïèñü â ñòåê äàííûõ P n US 0 g 000 0000 0001 0nn re 3data stack, DSP = DSP + 1 - - - - -
×òåíèå èç ñòåêà
äàííûõ P n OP 0 k 000 0000 0001 1nn data stac 3reg, DSP=DSP- 1 - - - - -
Óñòàíîâêà ðàçðÿäîâ
RS S b ST 0 1 000 0001 1000 bbb if mask(n)=1 then RS(n)= * * * - -
Ñ T áðîñ ðàçðÿäîâ RS C b S 0 0 000 0001 1100 bbb if mask(n)=1 then RS(n)= * * * - -
Ïðîâåðêà
ïåðåïîëíåíèÿ T 0 OF 0 F 000 0000 0000 010 O 3Z - * - - -
Ïðîâåðêà òåòðàäíîãî
ïåðåíîñà T 1 DC 0 C 000 0000 0000 010 D 3Z - * 


            Á P åçóñëîâíûé ïåðåõîä J a M 1 s 000 00aa aaaa aaa addres 3PC - - - - -
Ïåðåõîä ê
ïîäïðîãðàììå J a SR 1001 00aa aaaa aaa PC3istack, address3PC, ISP = ISP + 1 - - - - -
Ïåðåõîä ïî Z = 0(íå
ðàâíî)
JNZ
(JNE) 1 s 011 00aa aaaa aaaa addres 3PC if Z = 0 - - - - -
Ïåðåõîä ïî Z=1
(ðàâíî )
JZ
(JEQ) 1 s 010 00aa aaaa aaaa addres 3PC if Z = 1 - - - - -
Ïåðåõîä ïî S=0
(ïëþñ ) J a NS 1 s 100 00aa aaaa aaa addres 3PC if S = 0 - - - - -
Ïåðåõîä ïî S=1
(ìèíóñ ) J a S 1 s 101 00aa aaaa aaa addres 3PC if S = 1 - - - - -
Ï C åðåõîä ïî C=0 J a N 1 s 110 00aa aaaa aaa addres 3PC if C = 0 - - - - -
Ï C åðåõîä ïî C=1 J a 1 s 111 00aa aaaa aaa addres 3PC if C = 1 - - - - -
K P îñâåííûé ïåðåõîä I 1 JM 0 1 000 0000 0000 001 IR 3PC - - - - -
Kîñâåííûé ïåðåõîä ê
ïîäïðîãðàìììå I 1 JSR 0000 0000 0000 011 PC3istack, IR13PC, ISP=ISP+ 1 - - - - -
Âîçâðàò èç
ïîäïðîãðàììû R 0 TS 0 k 000 0000 0000 110 istac 3PC, ISP = ISP- 1 - - - - -
Âîçâðàò èç ïîäïðî- ãðàììû ñ áèòîì C R c TSC 0000 0000 0000 111 istack3PC c3RS(0), ISP = ISP- 1 - - * - -
Âîçâðàò èç
ïðåðûâàíèÿ R 1 TI 0 k 000 0000 0000 110 istac 3PC , data stack3RS * * * * *
Ñ û ïåöèàëüíûå êîìàíäû
Í P åò îïåðàöèè N 0 O 0000 0000 0000 000 - - - - -
Î T æèäàíèå W 1 AI 0 ) 000 0000 0000 000 RS(3) = 1 (INT Enable - - - - -
Î P ñòàíîâ S 0 TO 0 ) 000 0000 0000 100 RS(3) = 1 (INT Enable - - - - -
Ñ T áðîñ R 0 ESE 0 0 000 0000 0000 001 DSP=0, ISP= - - - - -
Ï P ðîãîí ñòåêà êîìàíä S 0 KS 0 1

    }
}
*/