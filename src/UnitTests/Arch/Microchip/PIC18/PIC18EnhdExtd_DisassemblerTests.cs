#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work of:
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

using NUnit.Framework;
using Reko.Libraries.Microchip;

// As of today there are 3 flavors of PIC18 with potentially 2 execution modes:
//  - legacy (a.k.a. "pic18"), traditional execution mode
//  - extended (a.k.a. "egg"), traditional/extended execution modes
//  - enhanced (a.k.a. "cpu_pic18f_v6"), traditional/extended execution modes
// This means we have 5 different contexts to check.
// In addition the extended and enhanced PIC18 disassemblers use the legacy disassembler for legacy instructions.
//
namespace Reko.UnitTests.Arch.Microchip.PIC18.Disasm
{
    using Common;
    using static Common.Sample;

    [TestFixture]
    public class PIC18EnhdExtd_DisassemblerTests : DisassemblerTestsBase
    {
        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC18EnhancedName, PICExecMode.Extended);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_ADDFSR()
        {
            VerifyDisasm("ADDFSR\tFSR0,0x00", "", 0xE800);
            VerifyDisasm("ADDFSR\tFSR1,0x35", "", 0xE875);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_ADDULNK()
        {
            VerifyDisasm("ADDULNK\t0x00", "", 0xE8C0);
            VerifyDisasm("ADDULNK\t0x35", "", 0xE8F5);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_ADDWF()
        {
            VerifyDisasm("ADDWF\t[0x12],W", "", 0x2412);
            VerifyDisasm("ADDWF\t[0x5A],W", "", 0x245A);
            VerifyDisasm("ADDWF\tT3CLK,W,ACCESS", "", 0x24A9);
            VerifyDisasm("ADDWF\t[0x23],F", "", 0x2623);
            VerifyDisasm("ADDWF\tCCP3CAP,F,ACCESS", "", 0x2677);
            VerifyDisasm("ADDWF\tLATB,F,ACCESS", "", 0x26BB);
            VerifyDisasm("ADDWF\t0x03,W", "", 0x2503);
            VerifyDisasm("ADDWF\t0x5A,W", "", 0x255A);
            VerifyDisasm("ADDWF\t0x8A,W", "", 0x258A);
            VerifyDisasm("ADDWF\t0x23,F", "", 0x2723);
            VerifyDisasm("ADDWF\t0x5A,F", "", 0x275A);
            VerifyDisasm("ADDWF\t0x89,F", "", 0x2789);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_ADDWFC()
        {
            VerifyDisasm("ADDWFC\t[0x12],W", "", 0x2012);
            VerifyDisasm("ADDWFC\t[0x5A],W", "", 0x205A);
            VerifyDisasm("ADDWFC\tT3CLK,W,ACCESS", "", 0x20A9);
            VerifyDisasm("ADDWFC\t[0x23],F", "", 0x2223);
            VerifyDisasm("ADDWFC\tCCP3CAP,F,ACCESS", "", 0x2277);
            VerifyDisasm("ADDWFC\tLATB,F,ACCESS", "", 0x22BB);
            VerifyDisasm("ADDWFC\t0x23,W", "", 0x2123);
            VerifyDisasm("ADDWFC\t0x5A,W", "", 0x215A);
            VerifyDisasm("ADDWFC\t0x8A,W", "", 0x218A);
            VerifyDisasm("ADDWFC\t0x23,F", "", 0x2323);
            VerifyDisasm("ADDWFC\t0x5A,F", "", 0x235A);
            VerifyDisasm("ADDWFC\t0x89,F", "", 0x2389);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_ANDWF()
        {
            VerifyDisasm("ANDWF\t[0x23],W", "", 0x1423);
            VerifyDisasm("ANDWF\t[0x5A],W", "", 0x145A);
            VerifyDisasm("ANDWF\tLATA,W,ACCESS", "", 0x14BA);
            VerifyDisasm("ANDWF\t[0x23],F", "", 0x1623);
            VerifyDisasm("ANDWF\t[0x5A],F", "", 0x165A);
            VerifyDisasm("ANDWF\tLATB,F,ACCESS", "", 0x16BB);
            VerifyDisasm("ANDWF\t0x23,W", "", 0x1523);
            VerifyDisasm("ANDWF\t0x5A,W", "", 0x155A);
            VerifyDisasm("ANDWF\t0x8A,W", "", 0x158A);
            VerifyDisasm("ANDWF\t0x23,F", "", 0x1723);
            VerifyDisasm("ANDWF\t0x5A,F", "", 0x175A);
            VerifyDisasm("ANDWF\t0x89,F", "", 0x1789);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BCF()
        {
            VerifyDisasm("BCF\t[0x03],0", "", 0x9003);
            VerifyDisasm("BCF\tPORTA,RA7,ACCESS", "", 0x9ECA);
            VerifyDisasm("BCF\tPORTB,RB0,ACCESS", "", 0x90CB);
            VerifyDisasm("BCF\t0x00,0", "", 0x9100);
            VerifyDisasm("BCF\t0x5A,1", "", 0x935A);
            VerifyDisasm("BCF\t0x8A,5", "", 0x9B8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BC()
        {
            VerifyDisasm("BC\t0x00000202", "", 0xE200);
            VerifyDisasm("BC\t0x00000248", "", 0xE223);
            VerifyDisasm("BC\t0x0000010E", "", 0xE286);
            VerifyDisasm("BC\t0x00000200", "", 0xE2FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BN()
        {
            VerifyDisasm("BN\t0x00000202", "", 0xE600);
            VerifyDisasm("BN\t0x00000248", "", 0xE623);
            VerifyDisasm("BN\t0x0000010E", "", 0xE686);
            VerifyDisasm("BN\t0x00000200", "", 0xE6FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BNC()
        {
            VerifyDisasm("BNC\t0x00000202", "", 0xE300);
            VerifyDisasm("BNC\t0x00000248", "", 0xE323);
            VerifyDisasm("BNC\t0x0000010E", "", 0xE386);
            VerifyDisasm("BNC\t0x00000200", "", 0xE3FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BNN()
        {
            VerifyDisasm("BNN\t0x00000202", "", 0xE700);
            VerifyDisasm("BNN\t0x00000248", "", 0xE723);
            VerifyDisasm("BNN\t0x0000010E", "", 0xE786);
            VerifyDisasm("BNN\t0x00000200", "", 0xE7FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BNOV()
        {
            VerifyDisasm("BNOV\t0x00000202", "", 0xE500);
            VerifyDisasm("BNOV\t0x00000248", "", 0xE523);
            VerifyDisasm("BNOV\t0x0000010E", "", 0xE586);
            VerifyDisasm("BNOV\t0x00000200", "", 0xE5FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BOV()
        {
            VerifyDisasm("BOV\t0x00000202", "", 0xE400);
            VerifyDisasm("BOV\t0x00000248", "", 0xE423);
            VerifyDisasm("BOV\t0x0000010E", "", 0xE486);
            VerifyDisasm("BOV\t0x00000200", "", 0xE4FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BNZ()
        {
            VerifyDisasm("BNZ\t0x00000202", "", 0xE100);
            VerifyDisasm("BNZ\t0x00000248", "", 0xE123);
            VerifyDisasm("BNZ\t0x0000010E", "", 0xE186);
            VerifyDisasm("BNZ\t0x00000200", "", 0xE1FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BRA()
        {
            VerifyDisasm("BRA\t0x00000202", "", 0xD000);
            VerifyDisasm("BRA\t0x000002AC", "", 0xD055);
            VerifyDisasm("BRA\t0x000000AC", "", 0xD755);
            VerifyDisasm("BRA\t0x00000200", "", 0xD7FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BSF()
        {
            VerifyDisasm("BSF\t[0x03],0", "", 0x8003);
            VerifyDisasm("BSF\tPORTB,RB7,ACCESS", "", 0x8ECB);
            VerifyDisasm("BSF\tPORTA,RA0,ACCESS", "", 0x80CA);
            VerifyDisasm("BSF\t0x00,0", "", 0x8100);
            VerifyDisasm("BSF\t0x5A,1", "", 0x835A);
            VerifyDisasm("BSF\t0x8A,5", "", 0x8B8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BTFSC()
        {
            VerifyDisasm("BTFSC\t[0x03],0", "", 0xB003);
            VerifyDisasm("BTFSC\tPORTA,RA7,ACCESS", "", 0xBECA);
            VerifyDisasm("BTFSC\tPORTB,RB0,ACCESS", "", 0xB0CB);
            VerifyDisasm("BTFSC\t0x00,0", "", 0xB100);
            VerifyDisasm("BTFSC\t0x5A,1", "", 0xB35A);
            VerifyDisasm("BTFSC\t0x8A,5", "", 0xBB8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BTFSS()
        {
            VerifyDisasm("BTFSS\t[0x03],0", "", 0xA003);
            VerifyDisasm("BTFSS\tPORTA,RA7,ACCESS", "", 0xAECA);
            VerifyDisasm("BTFSS\tPORTB,RB0,ACCESS", "", 0xA0CB);
            VerifyDisasm("BTFSS\t0x00,0", "", 0xA100);
            VerifyDisasm("BTFSS\t0x5A,1", "", 0xA35A);
            VerifyDisasm("BTFSS\t0x8A,5", "", 0xAB8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BZ()
        {
            VerifyDisasm("BZ\t0x00000202", "", 0xE000);
            VerifyDisasm("BZ\t0x00000248", "", 0xE023);
            VerifyDisasm("BZ\t0x0000010E", "", 0xE086);
            VerifyDisasm("BZ\t0x00000200", "", 0xE0FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_BTG()
        {
            VerifyDisasm("BTG\t[0x03],0", "", 0x7003);
            VerifyDisasm("BTG\tTRISA,TRISA7,ACCESS", "", 0x7EC2);
            VerifyDisasm("BTG\tTRISB,TRISB0,ACCESS", "", 0x70C3);
            VerifyDisasm("BTG\t0x00,0", "", 0x7100);
            VerifyDisasm("BTG\t0x5A,1", "", 0x735A);
            VerifyDisasm("BTG\t0x8A,5", "", 0x7B8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CALL()
        {
            VerifyDisasm("CALL\t0x0000000C", "", 0xEC06, 0xF000);
            VerifyDisasm("CALL\t0x00068A24", "", 0xEC12, 0xF345);
            VerifyDisasm("CALL\t0x0000000C,FAST", "", 0xED06, 0xF000);
            VerifyDisasm("CALL\t0x00068A24,FAST", "", 0xED12, 0xF345);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CALLW()
        {
            VerifyDisasm("CALLW", "", 0x0014);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CLRF()
        {
            VerifyDisasm("CLRF\t[0x12]", "", 0x6A12);
            VerifyDisasm("CLRF\t[0x5A]", "", 0x6A5A);
            VerifyDisasm("CLRF\tFSR0H,ACCESS", "", 0x6AEA);
            VerifyDisasm("CLRF\t0x03", "", 0x6B03);
            VerifyDisasm("CLRF\t0x5A", "", 0x6B5A);
            VerifyDisasm("CLRF\t0x8A", "", 0x6B8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CLRWDT()
        {
            VerifyDisasm("CLRWDT", "", 0x0004);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_COMF()
        {
            VerifyDisasm("COMF\t[0x12],W", "", 0x1C12);
            VerifyDisasm("COMF\t[0x5A],W", "", 0x1C5A);
            VerifyDisasm("COMF\tLATA,W,ACCESS", "", 0x1CBA);
            VerifyDisasm("COMF\t[0x23],F", "", 0x1E23);
            VerifyDisasm("COMF\tCCP3CAP,F,ACCESS", "", 0x1E77);
            VerifyDisasm("COMF\tLATB,F,ACCESS", "", 0x1EBB);
            VerifyDisasm("COMF\t0x23,W", "", 0x1D23);
            VerifyDisasm("COMF\t0x5A,W", "", 0x1D5A);
            VerifyDisasm("COMF\t0x8A,W", "", 0x1D8A);
            VerifyDisasm("COMF\t0x23,F", "", 0x1F23);
            VerifyDisasm("COMF\t0x5A,F", "", 0x1F5A);
            VerifyDisasm("COMF\t0x89,F", "", 0x1F89);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CPFSEQ()
        {
            VerifyDisasm("CPFSEQ\t[0x12]", "", 0x6212);
            VerifyDisasm("CPFSEQ\t[0x5A]", "", 0x625A);
            VerifyDisasm("CPFSEQ\tFSR0H,ACCESS", "", 0x62EA);
            VerifyDisasm("CPFSEQ\t0x03", "", 0x6303);
            VerifyDisasm("CPFSEQ\t0x5A", "", 0x635A);
            VerifyDisasm("CPFSEQ\t0x8A", "", 0x638A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CPFSLT()
        {
            VerifyDisasm("CPFSLT\t[0x12]", "", 0x6012);
            VerifyDisasm("CPFSLT\t[0x5A]", "", 0x605A);
            VerifyDisasm("CPFSLT\tFSR0L,ACCESS", "", 0x60E9);
            VerifyDisasm("CPFSLT\t0x03", "", 0x6103);
            VerifyDisasm("CPFSLT\t0x5A", "", 0x615A);
            VerifyDisasm("CPFSLT\t0x8A", "", 0x618A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_CPFSGT()
        {
            VerifyDisasm("CPFSGT\t[0x12]", "", 0x6412);
            VerifyDisasm("CPFSGT\t[0x5A]", "", 0x645A);
            VerifyDisasm("CPFSGT\tFSR0H,ACCESS", "", 0x64EA);
            VerifyDisasm("CPFSGT\t0x03", "", 0x6503);
            VerifyDisasm("CPFSGT\t0x5A", "", 0x655A);
            VerifyDisasm("CPFSGT\t0x8A", "", 0x658A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_DAW()
        {
            VerifyDisasm("DAW", "", 0x0007);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_DCFSNZ()
        {
            VerifyDisasm("DCFSNZ\t[0x12],W", "", 0x4C12);
            VerifyDisasm("DCFSNZ\t[0x5A],W", "", 0x4C5A);
            VerifyDisasm("DCFSNZ\tFSR0L,W,ACCESS", "", 0x4CE9);
            VerifyDisasm("DCFSNZ\t[0x23],F", "", 0x4E23);
            VerifyDisasm("DCFSNZ\tCCP3CAP,F,ACCESS", "", 0x4E77);
            VerifyDisasm("DCFSNZ\tFSR1L,F,ACCESS", "", 0x4EE1);
            VerifyDisasm("DCFSNZ\t0x03,W", "", 0x4D03);
            VerifyDisasm("DCFSNZ\t0x5A,W", "", 0x4D5A);
            VerifyDisasm("DCFSNZ\t0x8A,W", "", 0x4D8A);
            VerifyDisasm("DCFSNZ\t0x23,F", "", 0x4F23);
            VerifyDisasm("DCFSNZ\t0x5A,F", "", 0x4F5A);
            VerifyDisasm("DCFSNZ\t0xE1,F", "", 0x4FE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_DECF()
        {
            VerifyDisasm("DECF\t[0x01],W", "", 0x0401);
            VerifyDisasm("DECF\t[0x5F],W", "", 0x045F);
            VerifyDisasm("DECF\tPLUSW2,W,ACCESS", "", 0x04DB);
            VerifyDisasm("DECF\t[0x44],F", "", 0x0644);
            VerifyDisasm("DECF\tINDF1,F,ACCESS", "", 0x06E7);
            VerifyDisasm("DECF\t0x01,W", "", 0x0501);
            VerifyDisasm("DECF\t0x5F,W", "", 0x055F);
            VerifyDisasm("DECF\t0xBB,W", "", 0x05BB);
            VerifyDisasm("DECF\t0x44,F", "", 0x0744);
            VerifyDisasm("DECF\t0xDB,F", "", 0x07DB);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_DECFSZ()
        {
            VerifyDisasm("DECFSZ\t[0x12],W", "", 0x2C12);
            VerifyDisasm("DECFSZ\t[0x5A],W", "", 0x2C5A);
            VerifyDisasm("DECFSZ\tFSR0H,W,ACCESS", "", 0x2CEA);
            VerifyDisasm("DECFSZ\t[0x23],F", "", 0x2E23);
            VerifyDisasm("DECFSZ\tCCP3CAP,F,ACCESS", "", 0x2E77);
            VerifyDisasm("DECFSZ\tFSR1L,F,ACCESS", "", 0x2EE1);
            VerifyDisasm("DECFSZ\t0x03,W", "", 0x2D03);
            VerifyDisasm("DECFSZ\t0x5A,W", "", 0x2D5A);
            VerifyDisasm("DECFSZ\t0x8A,W", "", 0x2D8A);
            VerifyDisasm("DECFSZ\t0x23,F", "", 0x2F23);
            VerifyDisasm("DECFSZ\t0x5A,F", "", 0x2F5A);
            VerifyDisasm("DECFSZ\t0xE1,F", "", 0x2FE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_GOTO()
        {
            VerifyDisasm("GOTO\t0x00000006", "", 0xEF03, 0xF000);
            VerifyDisasm("GOTO\t0x000F12AC", "", 0xEF56, 0xF789);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_INCF()
        {
            VerifyDisasm("INCF\t[0x01],W", "", 0x2801);
            VerifyDisasm("INCF\t[0x5F],W", "", 0x285F);
            VerifyDisasm("INCF\tPLUSW2,W,ACCESS", "", 0x28DB);
            VerifyDisasm("INCF\t[0x44],F", "", 0x2A44);
            VerifyDisasm("INCF\tINDF1,F,ACCESS", "", 0x2AE7);
            VerifyDisasm("INCF\t0x01,W", "", 0x2901);
            VerifyDisasm("INCF\t0x5F,W", "", 0x295F);
            VerifyDisasm("INCF\t0xBB,W", "", 0x29BB);
            VerifyDisasm("INCF\t0x44,F", "", 0x2B44);
            VerifyDisasm("INCF\t0xDB,F", "", 0x2BDB);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_INCFSZ()
        {
            VerifyDisasm("INCFSZ\t[0x12],W", "", 0x3C12);
            VerifyDisasm("INCFSZ\t[0x5A],W", "", 0x3C5A);
            VerifyDisasm("INCFSZ\tFSR0L,W,ACCESS", "", 0x3CE9);
            VerifyDisasm("INCFSZ\t[0x23],F", "", 0x3E23);
            VerifyDisasm("INCFSZ\tCCP3CAP,F,ACCESS", "", 0x3E77);
            VerifyDisasm("INCFSZ\tFSR1L,F,ACCESS", "", 0x3EE1);
            VerifyDisasm("INCFSZ\t0x03,W", "", 0x3D03);
            VerifyDisasm("INCFSZ\t0x5A,W", "", 0x3D5A);
            VerifyDisasm("INCFSZ\t0x8A,W", "", 0x3D8A);
            VerifyDisasm("INCFSZ\t0x23,F", "", 0x3F23);
            VerifyDisasm("INCFSZ\t0x5A,F", "", 0x3F5A);
            VerifyDisasm("INCFSZ\t0xE1,F", "", 0x3FE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_INFSNZ()
        {
            VerifyDisasm("INFSNZ\t[0x12],W", "", 0x4812);
            VerifyDisasm("INFSNZ\t[0x5A],W", "", 0x485A);
            VerifyDisasm("INFSNZ\tFSR0H,W,ACCESS", "", 0x48EA);
            VerifyDisasm("INFSNZ\t[0x23],F", "", 0x4A23);
            VerifyDisasm("INFSNZ\tCCP3CAP,F,ACCESS", "", 0x4A77);
            VerifyDisasm("INFSNZ\tFSR1L,F,ACCESS", "", 0x4AE1);
            VerifyDisasm("INFSNZ\t0x03,W", "", 0x4903);
            VerifyDisasm("INFSNZ\t0x5A,W", "", 0x495A);
            VerifyDisasm("INFSNZ\t0x8A,W", "", 0x498A);
            VerifyDisasm("INFSNZ\t0x23,F", "", 0x4B23);
            VerifyDisasm("INFSNZ\t0x5A,F", "", 0x4B5A);
            VerifyDisasm("INFSNZ\t0xE1,F", "", 0x4BE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_IORWF()
        {
            VerifyDisasm("IORWF\t[0x23],W", "", 0x1023);
            VerifyDisasm("IORWF\t[0x5A],W", "", 0x105A);
            VerifyDisasm("IORWF\tLATA,W,ACCESS", "", 0x10BA);
            VerifyDisasm("IORWF\t[0x23],F", "", 0x1223);
            VerifyDisasm("IORWF\t[0x5A],F", "", 0x125A);
            VerifyDisasm("IORWF\tLATB,F,ACCESS", "", 0x12BB);
            VerifyDisasm("IORWF\t0x23,W", "", 0x1123);
            VerifyDisasm("IORWF\t0x5A,W", "", 0x115A);
            VerifyDisasm("IORWF\t0x8A,W", "", 0x118A);
            VerifyDisasm("IORWF\t0x23,F", "", 0x1323);
            VerifyDisasm("IORWF\t0x5A,F", "", 0x135A);
            VerifyDisasm("IORWF\t0x89,F", "", 0x1389);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_LFSR()
        {
            VerifyDisasm("LFSR\tFSR0,0x0C00", "", 0xEE03, 0xF000);
            VerifyDisasm("LFSR\tFSR0,0x0034", "", 0xEE00, 0xF034);
            VerifyDisasm("LFSR\tFSR2,0x1889", "", 0xEE26, 0xF089);
            VerifyDisasm("LFSR\tFSR0,0x3FBA", "", 0xEE0F, 0xF3BA);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVLB()
        {
            VerifyDisasm("MOVLB\t0x00", "", 0x0100);
            VerifyDisasm("MOVLB\t0x07", "", 0x0107);
            VerifyDisasm("MOVLB\t0x0F", "", 0x010F);
            VerifyDisasm("MOVLB\t0x10", "", 0x0110);
            VerifyDisasm("MOVLB\t0x3F", "", 0x013F);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVF()
        {
            VerifyDisasm("MOVF\t[0x12],W", "", 0x5012);
            VerifyDisasm("MOVF\t[0x5A],W", "", 0x505A);
            VerifyDisasm("MOVF\tFSR0L,W,ACCESS", "", 0x50E9);
            VerifyDisasm("MOVF\t[0x23],F", "", 0x5223);
            VerifyDisasm("MOVF\tCCP3CAP,F,ACCESS", "", 0x5277);
            VerifyDisasm("MOVF\tFSR1L,F,ACCESS", "", 0x52E1);
            VerifyDisasm("MOVF\t0x03,W", "", 0x5103);
            VerifyDisasm("MOVF\t0x5A,W", "", 0x515A);
            VerifyDisasm("MOVF\t0x8A,W", "", 0x518A);
            VerifyDisasm("MOVF\t0x23,F", "", 0x5323);
            VerifyDisasm("MOVF\t0x5A,F", "", 0x535A);
            VerifyDisasm("MOVF\t0xE1,F", "", 0x53E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVFF()
        {
            VerifyDisasm("MOVFF\t0x0000,0x0123", "", 0xC000, 0xF123);
            VerifyDisasm("MOVFF\t0x0FCA,0x0FCB", "", 0xCFCA, 0xFFCB);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVWF()
        {
            VerifyDisasm("MOVWF\t[0x12]", "", 0x6E12);
            VerifyDisasm("MOVWF\t[0x5A]", "", 0x6E5A);
            VerifyDisasm("MOVWF\tFSR0L,ACCESS", "", 0x6EE9);
            VerifyDisasm("MOVWF\t0x00", "", 0x6F00);
            VerifyDisasm("MOVWF\t0x5A", "", 0x6F5A);
            VerifyDisasm("MOVWF\t0x8A", "", 0x6F8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVSF()
        {
            VerifyDisasm("MOVSF\t[0x00],0x0000", "", 0xEB00, 0xF000);
            VerifyDisasm("MOVSF\t[0x75],0x0033", "", 0xEB75, 0xF033);
            VerifyDisasm("MOVSF\t[0x75],0x0FE3", "", 0xEB75, 0xFFE3);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVSFL()
        {
            VerifyDisasm("MOVSFL\t[0x00],0x0123", "", 0x0002, 0xF000, 0xF123);
            VerifyDisasm("MOVSFL\t[0x04],0x0789", "", 0x0002, 0xF010, 0xF789);
            VerifyDisasm("MOVSFL\t[0x78],PLUSW1", "", 0x0002, 0xFFE3, 0xFFE3);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MOVSS()
        {
            VerifyDisasm("MOVSS\t[0x00],[0x01]", "", 0xEB80, 0xF001);
            VerifyDisasm("MOVSS\t[0x75],[0x44]", "", 0xEBF5, 0xF044);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_MULWF()
        {
            VerifyDisasm("MULWF\t[0x01]", "", 0x0201);
            VerifyDisasm("MULWF\t[0x43]", "", 0x0243);
            VerifyDisasm("MULWF\tWREG,ACCESS", "", 0x02E8);
            VerifyDisasm("MULWF\t0x00", "", 0x0300);
            VerifyDisasm("MULWF\t0x43", "", 0x0343);
            VerifyDisasm("MULWF\t0xE8", "", 0x03E8);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_NEGF()
        {
            VerifyDisasm("NEGF\t[0x12]", "", 0x6C12);
            VerifyDisasm("NEGF\t[0x5A]", "", 0x6C5A);
            VerifyDisasm("NEGF\tFSR0H,ACCESS", "", 0x6CEA);
            VerifyDisasm("NEGF\t0x03", "", 0x6D03);
            VerifyDisasm("NEGF\t0x5A", "", 0x6D5A);
            VerifyDisasm("NEGF\t0x8A", "", 0x6D8A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_NOP()
        {
            VerifyDisasm("NOP", "", 0x0000);
            VerifyDisasm("NOP", "", 0xF000);
            VerifyDisasm("NOP", "", 0xF528);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_POP()
        {
            VerifyDisasm("POP", "", 0x0006);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_PUSH()
        {
            VerifyDisasm("PUSH", "", 0x0005);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_PUSHL()
        {
            VerifyDisasm("PUSHL\t0x00", "", 0xEA00);
            VerifyDisasm("PUSHL\t0xF5", "", 0xEAF5);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RCALL()
        {
            VerifyDisasm("RCALL\t0x00000204", "", 0xD801);
            VerifyDisasm("RCALL\t0x00000356", "", 0xD8AA);
            VerifyDisasm("RCALL\t0x00000A00", "", 0xDBFF);
            VerifyDisasm("RCALL\t0x003FFA02", "", 0xDC00);
            VerifyDisasm("RCALL\t0x00000156", "", 0xDFAA);
            VerifyDisasm("RCALL\t0x00000200", "", 0xDFFF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RESET()
        {
            VerifyDisasm("RESET", "", 0x00FF);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RETFIE()
        {
            VerifyDisasm("RETFIE", "", 0x0010);
            VerifyDisasm("RETFIE,\tFAST", "", 0x0011);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RETURN()
        {
            VerifyDisasm("RETURN", "", 0x0012);
            VerifyDisasm("RETURN,\tFAST", "", 0x0013);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RLCF()
        {
            VerifyDisasm("RLCF\t[0x12],W", "", 0x3412);
            VerifyDisasm("RLCF\t[0x5A],W", "", 0x345A);
            VerifyDisasm("RLCF\tFSR0H,W,ACCESS", "", 0x34EA);
            VerifyDisasm("RLCF\t[0x23],F", "", 0x3623);
            VerifyDisasm("RLCF\tCCP3CAP,F,ACCESS", "", 0x3677);
            VerifyDisasm("RLCF\tFSR1L,F,ACCESS", "", 0x36E1);
            VerifyDisasm("RLCF\t0x03,W", "", 0x3503);
            VerifyDisasm("RLCF\t0x5A,W", "", 0x355A);
            VerifyDisasm("RLCF\t0x8A,W", "", 0x358A);
            VerifyDisasm("RLCF\t0x23,F", "", 0x3723);
            VerifyDisasm("RLCF\t0x5A,F", "", 0x375A);
            VerifyDisasm("RLCF\t0xE1,F", "", 0x37E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RLNCF()
        {
            VerifyDisasm("RLNCF\t[0x12],W", "", 0x4412);
            VerifyDisasm("RLNCF\t[0x5A],W", "", 0x445A);
            VerifyDisasm("RLNCF\tFSR0H,W,ACCESS", "", 0x44EA);
            VerifyDisasm("RLNCF\t[0x23],F", "", 0x4623);
            VerifyDisasm("RLNCF\tCCP3CAP,F,ACCESS", "", 0x4677);
            VerifyDisasm("RLNCF\tFSR1L,F,ACCESS", "", 0x46E1);
            VerifyDisasm("RLNCF\t0x03,W", "", 0x4503);
            VerifyDisasm("RLNCF\t0x5A,W", "", 0x455A);
            VerifyDisasm("RLNCF\t0x8A,W", "", 0x458A);
            VerifyDisasm("RLNCF\t0x23,F", "", 0x4723);
            VerifyDisasm("RLNCF\t0x5A,F", "", 0x475A);
            VerifyDisasm("RLNCF\t0xE1,F", "", 0x47E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RRCF()
        {
            VerifyDisasm("RRCF\t[0x12],W", "", 0x3012);
            VerifyDisasm("RRCF\t[0x5A],W", "", 0x305A);
            VerifyDisasm("RRCF\tFSR0L,W,ACCESS", "", 0x30E9);
            VerifyDisasm("RRCF\t[0x23],F", "", 0x3223);
            VerifyDisasm("RRCF\tCCP3CAP,F,ACCESS", "", 0x3277);
            VerifyDisasm("RRCF\tFSR1L,F,ACCESS", "", 0x32E1);
            VerifyDisasm("RRCF\t0x03,W", "", 0x3103);
            VerifyDisasm("RRCF\t0x5A,W", "", 0x315A);
            VerifyDisasm("RRCF\t0x8A,W", "", 0x318A);
            VerifyDisasm("RRCF\t0x23,F", "", 0x3323);
            VerifyDisasm("RRCF\t0x5A,F", "", 0x335A);
            VerifyDisasm("RRCF\t0xE1,F", "", 0x33E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_RRNCF()
        {
            VerifyDisasm("RRNCF\t[0x12],W", "", 0x4012);
            VerifyDisasm("RRNCF\t[0x5A],W", "", 0x405A);
            VerifyDisasm("RRNCF\tFSR0L,W,ACCESS", "", 0x40E9);
            VerifyDisasm("RRNCF\t[0x23],F", "", 0x4223);
            VerifyDisasm("RRNCF\tCCP3CAP,F,ACCESS", "", 0x4277);
            VerifyDisasm("RRNCF\tFSR1L,F,ACCESS", "", 0x42E1);
            VerifyDisasm("RRNCF\t0x03,W", "", 0x4103);
            VerifyDisasm("RRNCF\t0x5A,W", "", 0x415A);
            VerifyDisasm("RRNCF\t0x8A,W", "", 0x418A);
            VerifyDisasm("RRNCF\t0x23,F", "", 0x4323);
            VerifyDisasm("RRNCF\t0x5A,F", "", 0x435A);
            VerifyDisasm("RRNCF\t0xE1,F", "", 0x43E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SETF()
        {
            VerifyDisasm("SETF\t[0x12]", "", 0x6812);
            VerifyDisasm("SETF\t[0x5A]", "", 0x685A);
            VerifyDisasm("SETF\tFSR0L,ACCESS", "", 0x68E9);
            VerifyDisasm("SETF\t0x03", "", 0x6903);
            VerifyDisasm("SETF\t0x5A", "", 0x695A);
            VerifyDisasm("SETF\t0x8A", "", 0x698A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SLEEP()
        {
            VerifyDisasm("SLEEP", "", 0x0003);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SUBFWB()
        {
            VerifyDisasm("SUBFWB\t[0x12],W", "", 0x5412);
            VerifyDisasm("SUBFWB\t[0x5A],W", "", 0x545A);
            VerifyDisasm("SUBFWB\tFSR0H,W,ACCESS", "", 0x54EA);
            VerifyDisasm("SUBFWB\t[0x23],F", "", 0x5623);
            VerifyDisasm("SUBFWB\tCCP3CAP,F,ACCESS", "", 0x5677);
            VerifyDisasm("SUBFWB\tFSR1L,F,ACCESS", "", 0x56E1);
            VerifyDisasm("SUBFWB\t0x03,W", "", 0x5503);
            VerifyDisasm("SUBFWB\t0x5A,W", "", 0x555A);
            VerifyDisasm("SUBFWB\t0x8A,W", "", 0x558A);
            VerifyDisasm("SUBFWB\t0x23,F", "", 0x5723);
            VerifyDisasm("SUBFWB\t0x5A,F", "", 0x575A);
            VerifyDisasm("SUBFWB\t0xE1,F", "", 0x57E1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SUBFSR()
        {
            VerifyDisasm("SUBFSR\tFSR0,0x00", "", 0xE900);
            VerifyDisasm("SUBFSR\tFSR1,0x35", "", 0xE975);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SUBULNK()
        {
            VerifyDisasm("SUBULNK\t0x00", "", 0xE9C0);
            VerifyDisasm("SUBULNK\t0x35", "", 0xE9F5);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SUBWF()
        {
            VerifyDisasm("SUBWF\t[0x12],W", "", 0x5C12);
            VerifyDisasm("SUBWF\t[0x5A],W", "", 0x5C5A);
            VerifyDisasm("SUBWF\tFSR0L,W,ACCESS", "", 0x5CE9);
            VerifyDisasm("SUBWF\t[0x23],F", "", 0x5E23);
            VerifyDisasm("SUBWF\tCCP3CAP,F,ACCESS", "", 0x5E77);
            VerifyDisasm("SUBWF\tFSR1L,F,ACCESS", "", 0x5EE1);
            VerifyDisasm("SUBWF\t0x03,W", "", 0x5D03);
            VerifyDisasm("SUBWF\t0x5A,W", "", 0x5D5A);
            VerifyDisasm("SUBWF\t0x8A,W", "", 0x5D8A);
            VerifyDisasm("SUBWF\t0x23,F", "", 0x5F23);
            VerifyDisasm("SUBWF\t0x5A,F", "", 0x5F5A);
            VerifyDisasm("SUBWF\t0xE1,F", "", 0x5FE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SUBWFB()
        {
            VerifyDisasm("SUBWFB\t[0x12],W", "", 0x5812);
            VerifyDisasm("SUBWFB\t[0x5A],W", "", 0x585A);
            VerifyDisasm("SUBWFB\tFSR0H,W,ACCESS", "", 0x58EA);
            VerifyDisasm("SUBWFB\t[0x23],F", "", 0x5A23);
            VerifyDisasm("SUBWFB\tCCP3CAP,F,ACCESS", "", 0x5A77);
            VerifyDisasm("SUBWFB\tFSR1L,F,ACCESS", "", 0x5AE1);
            VerifyDisasm("SUBWFB\t0x03,W", "", 0x5903);
            VerifyDisasm("SUBWFB\t0x5A,W", "", 0x595A);
            VerifyDisasm("SUBWFB\t0x8A,W", "", 0x598A);
            VerifyDisasm("SUBWFB\t0x23,F", "", 0x5B23);
            VerifyDisasm("SUBWFB\t0x5A,F", "", 0x5B5A);
            VerifyDisasm("SUBWFB\t0xE1,F", "", 0x5BE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_SWAPF()
        {
            VerifyDisasm("SWAPF\t[0x12],W", "", 0x3812);
            VerifyDisasm("SWAPF\t[0x5A],W", "", 0x385A);
            VerifyDisasm("SWAPF\tFSR0L,W,ACCESS", "", 0x38E9);
            VerifyDisasm("SWAPF\t[0x23],F", "", 0x3A23);
            VerifyDisasm("SWAPF\tCCP3CAP,F,ACCESS", "", 0x3A77);
            VerifyDisasm("SWAPF\tFSR1L,F,ACCESS", "", 0x3AE1);
            VerifyDisasm("SWAPF\t0x03,W", "", 0x3903);
            VerifyDisasm("SWAPF\t0x5A,W", "", 0x395A);
            VerifyDisasm("SWAPF\t0x8A,W", "", 0x398A);
            VerifyDisasm("SWAPF\t0x23,F", "", 0x3B23);
            VerifyDisasm("SWAPF\t0x5A,F", "", 0x3B5A);
            VerifyDisasm("SWAPF\t0xE1,F", "", 0x3BE1);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_TBLRD()
        {
            VerifyDisasm("TBLRD*", "", 0x0008);
            VerifyDisasm("TBLRD*+", "", 0x0009);
            VerifyDisasm("TBLRD*-", "", 0x000A);
            VerifyDisasm("TBLRD+*", "", 0x000B);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_TBLWT()
        {
            VerifyDisasm("TBLWT*", "", 0x000C);
            VerifyDisasm("TBLWT*+", "", 0x000D);
            VerifyDisasm("TBLWT*-", "", 0x000E);
            VerifyDisasm("TBLWT+*", "", 0x000F);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_TSTFSZ()
        {
            VerifyDisasm("TSTFSZ\t[0x12]", "", 0x6612);
            VerifyDisasm("TSTFSZ\t[0x5A]", "", 0x665A);
            VerifyDisasm("TSTFSZ\tFSR0L,ACCESS", "", 0x66E9);
            VerifyDisasm("TSTFSZ\t0x00", "", 0x6700);
            VerifyDisasm("TSTFSZ\t0x5A", "", 0x675A);
            VerifyDisasm("TSTFSZ\t0x8A", "", 0x678A);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_XORWF()
        {
            VerifyDisasm("XORWF\t[0x23],W", "", 0x1823);
            VerifyDisasm("XORWF\t[0x5A],W", "", 0x185A);
            VerifyDisasm("XORWF\tLATA,W,ACCESS", "", 0x18BA);
            VerifyDisasm("XORWF\t[0x23],F", "", 0x1A23);
            VerifyDisasm("XORWF\t[0x5A],F", "", 0x1A5A);
            VerifyDisasm("XORWF\tLATB,F,ACCESS", "", 0x1ABB);
            VerifyDisasm("XORWF\t0x23,W", "", 0x1923);
            VerifyDisasm("XORWF\t0x5A,W", "", 0x195A);
            VerifyDisasm("XORWF\t0x8A,W", "", 0x198A);
            VerifyDisasm("XORWF\t0x23,F", "", 0x1B23);
            VerifyDisasm("XORWF\t0x5A,F", "", 0x1B5A);
            VerifyDisasm("XORWF\t0x89,F", "", 0x1B89);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_xxxLW()
        {
            VerifyDisasm("ADDLW\t0x33", "", 0x0F33);
            VerifyDisasm("ADDLW\t0x88", "", 0x0F88);
            VerifyDisasm("ANDLW\t0x22", "", 0x0B22);
            VerifyDisasm("ANDLW\t0x77", "", 0x0B77);
            VerifyDisasm("IORLW\t0x22", "", 0x0922);
            VerifyDisasm("IORLW\t0x77", "", 0x0977);
            VerifyDisasm("MOVLW\t0x00", "", 0x0E00);
            VerifyDisasm("MOVLW\t0xAA", "", 0x0EAA);
            VerifyDisasm("MULLW\t0x22", "", 0x0D22);
            VerifyDisasm("MULLW\t0x77", "", 0x0D77);
            VerifyDisasm("RETLW\t0x00", "", 0x0C00);
            VerifyDisasm("RETLW\t0xAA", "", 0x0CAA);
            VerifyDisasm("SUBLW\t0x00", "", 0x0800);
            VerifyDisasm("SUBLW\t0xAA", "", 0x08AA);
            VerifyDisasm("XORLW\t0x00", "", 0x0A00);
            VerifyDisasm("XORLW\t0xAA", "", 0x0AAA);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_Indirect()
        {
            VerifyDisasm("TSTFSZ\tINDF2,ACCESS", "", 0x66DF);
            VerifyDisasm("DECF\tPLUSW2,W,ACCESS", "", 0x04DB);
            VerifyDisasm("DECF\tINDF1,F,ACCESS", "", 0x06E7);
            VerifyDisasm("INCF\tPLUSW0,W,ACCESS", "", 0x28EB);
            VerifyDisasm("INCF\tINDF0,F,ACCESS", "", 0x2AEF);
            VerifyDisasm("ADDWFC\tINDF0,W,ACCESS", "", 0x20EF);
            VerifyDisasm("ANDWF\tPOSTINC1,F,ACCESS", "", 0x16E6);
            VerifyDisasm("ADDWF\tPOSTDEC0,F,ACCESS", "", 0x26ED);
            VerifyDisasm("SUBWFB\tPREINC2,F,ACCESS", "", 0x5ADC);
            VerifyDisasm("IORWF\tPLUSW1,F,ACCESS", "", 0x12E3);
            VerifyDisasm("MOVFF\t0x0FDB,0x0FEE", "", 0xCFDB, 0xFFEE);
            VerifyDisasm("MOVFFL\tPOSTINC1,PLUSW0", "", 0x006F, 0xFF9B, 0xFFEB); // POSTINC1 = 0x3FE6 , PLUSW0 = 0x3FEB 
            VerifyDisasm("MOVSF\t[0x25],0x0FEE", "", 0xEB25, 0xFFEE);
            VerifyDisasm("MOVSFL\t[0x25],PLUSW0", "", 0x0002, 0xF097, 0xFFEB);
            VerifyDisasm("MOVSS\t[0x07],[0x78]", "", 0xEB87, 0xF078);
        }

        [Test]
        public void PIC18EnhdExtd_Disasm_Invalids()
        {
            VerifyDisasm("invalid", "unknown opcode", 0x0001);
            VerifyDisasm("invalid", "missing second word", 0x0002);
            VerifyDisasm("invalid", "missing third word", 0x0002, 0xF000);
            VerifyDisasm("invalid", "invalid third word", 0x0002, 0xF000, 0x1234);
            VerifyDisasm("invalid", "unknown opcode", 0x0015);
            VerifyDisasm("invalid", "unknown opcode", 0x0016);
            VerifyDisasm("invalid", "unknown opcode", 0x0017);
            VerifyDisasm("invalid", "unknown opcode", 0x0018);
            VerifyDisasm("invalid", "unknown opcode", 0x0019);
            VerifyDisasm("invalid", "unknown opcode", 0x001A);
            VerifyDisasm("invalid", "unknown opcode", 0x001B);
            VerifyDisasm("invalid", "unknown opcode", 0x001C);
            VerifyDisasm("invalid", "unknown opcode", 0x001D);
            VerifyDisasm("invalid", "unknown opcode", 0x001E);
            VerifyDisasm("invalid", "unknown opcode", 0x001F);
            VerifyDisasm("invalid", "unknown opcode", 0x0020);
            VerifyDisasm("invalid", "unknown opcode", 0x0040);
            VerifyDisasm("invalid", "missing second word", 0x0060);
            VerifyDisasm("invalid", "invalid second word", 0x0067, 0x1234);
            VerifyDisasm("invalid", "missing third word", 0x006F, 0xF000);
            VerifyDisasm("invalid", "unknown opcode", 0x0080);
            VerifyDisasm("invalid", "unknown opcode", 0x00F0);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0140);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0180);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x01E0);
            VerifyDisasm("invalid", "missing second word", 0xC000);
            VerifyDisasm("invalid", "invalid second word", 0xC000, 0x0123);
            VerifyDisasm("invalid", "missing second word", 0xEB00);
            VerifyDisasm("invalid", "invalid second word", 0xEB00, 0x1234);
            VerifyDisasm("invalid", "missing second word", 0xEB80);
            VerifyDisasm("invalid", "invalid second word", 0xEB80, 0x1234);
            VerifyDisasm("invalid", "missing second word", 0xEC00);
            VerifyDisasm("invalid", "invalid second word", 0xEC00, 0x8765);
            VerifyDisasm("invalid", "missing second word", 0xED00);
            VerifyDisasm("invalid", "invalid second word", 0xED00, 0x9876);
            VerifyDisasm("invalid", "missing second word", 0xEE00);
            VerifyDisasm("invalid", "invalid second word", 0xEE00, 0x6543);
            VerifyDisasm("invalid", "LFSR too large literal value", 0xEE00, 0xF400);
            VerifyDisasm("invalid", "invalid FSR number", 0xEE30, 0xF000);
            VerifyDisasm("invalid", "unknown instruction", 0xEE40);
            VerifyDisasm("invalid", "unknown instruction", 0xEEF0);
            VerifyDisasm("invalid", "missing second word", 0xEF00);
            VerifyDisasm("invalid", "invalid second word", 0xEF00, 0xEDCB);

            VerifyDisasm("invalid", "(LFSR) too large value", 0xEE0F, 0xF400);
        }

    }

}
