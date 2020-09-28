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

// As of today there are 3 flavors of PIC16:
//  - Basic (a.k.a. "pic16f77")
//  - Enhanced (a.k.a. "cpu_mid_v10")
//  - Full-featured (a.k.a. "cpu_p16f1_v1")
// This means we have 3 different contexts to check.
//
namespace Reko.UnitTests.Arch.Microchip.PIC16.Disasm
{
    using Common;
    using static Common.Sample;

    // This series of tests permits to most instructions common to all PIC16 as well as Basic PIC16.
    // 
    [TestFixture]
    public class PIC16Basic_DisassemblerTests : DisassemblerTestsBase
    {

        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC16BasicName);
        }

        [Test]
        public void PIC16Basic_Disasm_ADDWF()
        {
            VerifyDisasm("ADDWF\t0x12,W", "", 0x0712);
            VerifyDisasm("ADDWF\t0x5A,W", "", 0x075A);
            VerifyDisasm("ADDWF\t0x73,W", "", 0x0773);
            VerifyDisasm("ADDWF\tINDF,W", "", 0x0700);
            VerifyDisasm("ADDWF\tINDF,F", "", 0x0780);
        }

        [Test]
        public void PIC16Basic_Disasm_ANDWF()
        {
            VerifyDisasm("ANDWF\t0x23,W", "", 0x0523);
            VerifyDisasm("ANDWF\t0x5A,W", "", 0x055A);
            VerifyDisasm("ANDWF\t0x09,F", "", 0x0589);
            VerifyDisasm("ANDWF\t0x23,F", "", 0x05A3);
            VerifyDisasm("ANDWF\t0x5A,F", "", 0x05DA);
            VerifyDisasm("ANDWF\t0x1A,F", "", 0x059A);
            VerifyDisasm("ANDWF\t0x23,W", "", 0x0523);
            VerifyDisasm("ANDWF\t0x5A,W", "", 0x055A);
            VerifyDisasm("ANDWF\tPCLATH,F", "", 0x058A);
        }

        [Test]
        public void PIC16Basic_Disasm_BCF()
        {
            VerifyDisasm("BCF\tSTATUS,C", "", 0x1003);
            VerifyDisasm("BCF\tINDF,7", "", 0x1380);
            VerifyDisasm("BCF\tPCL,1", "", 0x1082);
            VerifyDisasm("BCF\tSTATUS,RP0", "", 0x1283);
            VerifyDisasm("BCF\t0x5A,6", "", 0x135A);
            VerifyDisasm("BCF\tPCLATH,7", "", 0x138A);
        }

        [Test]
        public void PIC16Basic_Disasm_BSF()
        {
            VerifyDisasm("BSF\tSTATUS,C", "", 0x1403);
            VerifyDisasm("BSF\tINDF,7", "", 0x1780);
            VerifyDisasm("BSF\tPCL,1", "", 0x1482);
            VerifyDisasm("BSF\tSTATUS,RP0", "", 0x1683);
            VerifyDisasm("BSF\t0x5A,6", "", 0x175A);
            VerifyDisasm("BSF\tPCLATH,7", "", 0x178A);
        }

        [Test]
        public void PIC16Basic_Disasm_BTFSC()
        {
            VerifyDisasm("BTFSC\tSTATUS,C", "", 0x1803);
            VerifyDisasm("BTFSC\tINDF,7", "", 0x1B80);
            VerifyDisasm("BTFSC\tSTATUS,DC", "", 0x1883);
            VerifyDisasm("BTFSC\tINDF,2", "", 0x1900);
            VerifyDisasm("BTFSC\t0x5A,6", "", 0x1B5A);
            VerifyDisasm("BTFSC\tPCLATH,7", "", 0x1B8A);
        }

        [Test]
        public void PIC16Basic_Disasm_BTFSS()
        {
            VerifyDisasm("BTFSS\tSTATUS,C", "", 0x1C03);
            VerifyDisasm("BTFSS\tINDF,5", "", 0x1E80);
            VerifyDisasm("BTFSS\tSTATUS,DC", "", 0x1C83);
            VerifyDisasm("BTFSS\tINDF,0", "", 0x1C00);
            VerifyDisasm("BTFSS\t0x5A,6", "", 0x1F5A);
            VerifyDisasm("BTFSS\tPCLATH,7", "", 0x1F8A);
        }

        [Test]
        public void PIC16Basic_Disasm_CALL()
        {
            VerifyDisasm("CALL\t0x00000006", "", 0x2003);
            VerifyDisasm("CALL\t0x00000EAC", "", 0x2756);
        }

        [Test]
        public void PIC16Basic_Disasm_CLRF()
        {
            VerifyDisasm("CLRF\t0x12", "", 0x0192);
            VerifyDisasm("CLRF\t0x5A", "", 0x01DA);
            VerifyDisasm("CLRF\tINDF", "", 0x0180);
            VerifyDisasm("CLRF\tSTATUS", "", 0x0183);
            VerifyDisasm("CLRF\tFSR", "", 0x0184);
            VerifyDisasm("CLRF\tPCLATH", "", 0x018A);
        }

        [Test]
        public void PIC16Basic_Disasm_CLRW()
        {
            for (ushort uCode = 0x0100; uCode < 0x0180; uCode++)
            {
                VerifyDisasm("CLRW", "", uCode);
            }
        }

        [Test]
        public void PIC16Basic_Disasm_CLRWDT()
        {
            VerifyDisasm("CLRWDT", "", 0x0064);
        }

        [Test]
        public void PIC16Basic_Disasm_COMF()
        {
            VerifyDisasm("COMF\t0x12,W", "", 0x0912);
            VerifyDisasm("COMF\t0x5A,W", "", 0x095A);
            VerifyDisasm("COMF\t0x09,F", "", 0x0989);
            VerifyDisasm("COMF\t0x23,W", "", 0x0923);
            VerifyDisasm("COMF\t0x77,W", "", 0x0977);
            VerifyDisasm("COMF\tPCLATH,F", "", 0x098A);
            VerifyDisasm("COMF\t0x23,W", "", 0x0923);
            VerifyDisasm("COMF\t0x5A,W", "", 0x095A);
        }

        [Test]
        public void PIC16Basic_Disasm_DECF()
        {
            VerifyDisasm("DECF\t0x01,W", "", 0x0301);
            VerifyDisasm("DECF\t0x5F,W", "", 0x035F);
            VerifyDisasm("DECF\t0x44,W", "", 0x0344);
            VerifyDisasm("DECF\t0x01,F", "", 0x0381);
            VerifyDisasm("DECF\t0x5F,F", "", 0x03DF);
            VerifyDisasm("DECF\t0x3B,F", "", 0x03BB);
            VerifyDisasm("DECF\t0x44,F", "", 0x03C4);
            VerifyDisasm("DECF\t0x5B,F", "", 0x03DB);
        }

        [Test]
        public void PIC16Basic_Disasm_DECFSZ()
        {
            VerifyDisasm("DECFSZ\t0x12,W", "", 0x0B12);
            VerifyDisasm("DECFSZ\t0x5A,W", "", 0x0B5A);
            VerifyDisasm("DECFSZ\t0x6A,F", "", 0x0BEA);
            VerifyDisasm("DECFSZ\t0x23,W", "", 0x0B23);
            VerifyDisasm("DECFSZ\t0x77,W", "", 0x0B77);
            VerifyDisasm("DECFSZ\t0x61,F", "", 0x0BE1);
            VerifyDisasm("DECFSZ\tINDF,W", "", 0x0B00);
            VerifyDisasm("DECFSZ\tPCLATH,F", "", 0x0B8A);
        }

        [Test]
        public void PIC16Basic_Disasm_GOTO()
        {
            VerifyDisasm("GOTO\t0x00000006", "", 0x2803);
            VerifyDisasm("GOTO\t0x00000EAC", "", 0x2F56);
        }

        [Test]
        public void PIC16Basic_Disasm_INCF()
        {
            VerifyDisasm("INCF\t0x01,W", "", 0x0A01);
            VerifyDisasm("INCF\t0x5F,W", "", 0x0A5F);
            VerifyDisasm("INCF\t0x44,W", "", 0x0A44);
            VerifyDisasm("INCF\t0x01,F", "", 0x0A81);
            VerifyDisasm("INCF\t0x5F,W", "", 0x0A5F);
            VerifyDisasm("INCF\t0x3B,F", "", 0x0ABB);
            VerifyDisasm("INCF\t0x5B,F", "", 0x0ADB);
        }

        [Test]
        public void PIC16Basic_Disasm_INCFSZ()
        {
            VerifyDisasm("INCFSZ\t0x12,W", "", 0x0F12);
            VerifyDisasm("INCFSZ\t0x5A,W", "", 0x0F5A);
            VerifyDisasm("INCFSZ\t0x69,F", "", 0x0FE9);
            VerifyDisasm("INCFSZ\t0x23,W", "", 0x0F23);
            VerifyDisasm("INCFSZ\t0x77,W", "", 0x0F77);
            VerifyDisasm("INCFSZ\t0x61,F", "", 0x0FE1);
            VerifyDisasm("INCFSZ\tSTATUS,W", "", 0x0F03);
            VerifyDisasm("INCFSZ\tPCLATH,F", "", 0x0F8A);
        }

        [Test]
        public void PIC16Basic_Disasm_IORWF()
        {
            VerifyDisasm("IORWF\t0x23,W", "", 0x0423);
            VerifyDisasm("IORWF\t0x5A,W", "", 0x045A);
            VerifyDisasm("IORWF\t0x09,F", "", 0x0489);
            VerifyDisasm("IORWF\t0x23,W", "", 0x0423);
            VerifyDisasm("IORWF\t0x5A,W", "", 0x045A);
            VerifyDisasm("IORWF\tPCLATH,F", "", 0x048A);
        }

        [Test]
        public void PIC16Basic_Disasm_MOVF()
        {
            VerifyDisasm("MOVF\t0x12,W", "", 0x0812);
            VerifyDisasm("MOVF\t0x5A,W", "", 0x085A);
            VerifyDisasm("MOVF\t0x69,F", "", 0x08E9);
            VerifyDisasm("MOVF\t0x23,W", "", 0x0823);
            VerifyDisasm("MOVF\t0x77,W", "", 0x0877);
            VerifyDisasm("MOVF\t0x61,F", "", 0x08E1);
            VerifyDisasm("MOVF\tSTATUS,W", "", 0x0803);
            VerifyDisasm("MOVF\t0x5A,W", "", 0x085A);
            VerifyDisasm("MOVF\tPCLATH,F", "", 0x088A);
        }

        [Test]
        public void PIC16Basic_Disasm_MOVWF()
        {
            VerifyDisasm("MOVWF\t0x12", "", 0x0092);
            VerifyDisasm("MOVWF\t0x5A", "", 0x00DA);
            VerifyDisasm("MOVWF\t0x69", "", 0x00E9);
            VerifyDisasm("MOVWF\tINDF", "", 0x0080);
            VerifyDisasm("MOVWF\tFSR", "", 0x0084);
            VerifyDisasm("MOVWF\tPCLATH", "", 0x008A);
        }

        [Test]
        public void PIC16Basic_Disasm_NOP()
        {
            VerifyDisasm("NOP", "", 0x0000);
            VerifyDisasm("NOP", "", 0x0020);
            VerifyDisasm("NOP", "", 0x0040);
            VerifyDisasm("NOP", "", 0x0060);
        }

        [Test]
        public void PIC16Basic_Disasm_RETFIE()
        {
            VerifyDisasm("RETFIE", "", 0x0009);
        }

        [Test]
        public void PIC16Basic_Disasm_RETURN()
        {
            VerifyDisasm("RETURN", "", 0x0008);
        }

        [Test]
        public void PIC16Basic_Disasm_RLF()
        {
            VerifyDisasm("RLF\t0x12,W", "", 0x0D12);
            VerifyDisasm("RLF\t0x5A,W", "", 0x0D5A);
            VerifyDisasm("RLF\t0x6A,F", "", 0x0DEA);
            VerifyDisasm("RLF\t0x23,W", "", 0x0D23);
            VerifyDisasm("RLF\t0x77,W", "", 0x0D77);
            VerifyDisasm("RLF\t0x61,F", "", 0x0DE1);
            VerifyDisasm("RLF\tSTATUS,W", "", 0x0D03);
            VerifyDisasm("RLF\t0x5A,W", "", 0x0D5A);
            VerifyDisasm("RLF\tPCLATH,F", "", 0x0D8A);
        }

        [Test]
        public void PIC16Basic_Disasm_RRF()
        {
            VerifyDisasm("RRF\t0x12,W", "", 0x0C12);
            VerifyDisasm("RRF\t0x5A,W", "", 0x0C5A);
            VerifyDisasm("RRF\t0x6A,F", "", 0x0CEA);
            VerifyDisasm("RRF\t0x23,W", "", 0x0C23);
            VerifyDisasm("RRF\t0x77,W", "", 0x0C77);
            VerifyDisasm("RRF\t0x61,F", "", 0x0CE1);
            VerifyDisasm("RRF\tSTATUS,W", "", 0x0C03);
            VerifyDisasm("RRF\t0x5A,W", "", 0x0C5A);
            VerifyDisasm("RRF\tPCLATH,F", "", 0x0C8A);
        }

        [Test]
        public void PIC16Basic_Disasm_SLEEP()
        {
            VerifyDisasm("SLEEP", "", 0x0063);
        }

        [Test]
        public void PIC16Basic_Disasm_SUBWF()
        {
            VerifyDisasm("SUBWF\t0x12,W", "", 0x0212);
            VerifyDisasm("SUBWF\t0x5A,W", "", 0x025A);
            VerifyDisasm("SUBWF\t0x69,F", "", 0x02E9);
            VerifyDisasm("SUBWF\t0x23,W", "", 0x0223);
            VerifyDisasm("SUBWF\t0x77,W", "", 0x0277);
            VerifyDisasm("SUBWF\t0x61,F", "", 0x02E1);
            VerifyDisasm("SUBWF\tSTATUS,W", "", 0x0203);
            VerifyDisasm("SUBWF\tPCLATH,F", "", 0x028A);
        }

        [Test]
        public void PIC16Basic_Disasm_SWAPF()
        {
            VerifyDisasm("SWAPF\t0x12,W", "", 0x0E12);
            VerifyDisasm("SWAPF\t0x5A,W", "", 0x0E5A);
            VerifyDisasm("SWAPF\t0x69,F", "", 0x0EE9);
            VerifyDisasm("SWAPF\t0x23,W", "", 0x0E23);
            VerifyDisasm("SWAPF\t0x77,W", "", 0x0E77);
            VerifyDisasm("SWAPF\t0x61,F", "", 0x0EE1);
            VerifyDisasm("SWAPF\tSTATUS,W", "", 0x0E03);
            VerifyDisasm("SWAPF\tPCLATH,F", "", 0x0E8A);
        }

        [Test]
        public void PIC16Basic_Disasm_XORWF()
        {
            VerifyDisasm("XORWF\t0x12,W", "", 0x0612);
            VerifyDisasm("XORWF\t0x5A,W", "", 0x065A);
            VerifyDisasm("XORWF\t0x69,F", "", 0x06E9);
            VerifyDisasm("XORWF\t0x23,W", "", 0x0623);
            VerifyDisasm("XORWF\t0x77,W", "", 0x0677);
            VerifyDisasm("XORWF\t0x61,F", "", 0x06E1);
            VerifyDisasm("XORWF\tSTATUS,W", "", 0x0603);
            VerifyDisasm("XORWF\tPCLATH,F", "", 0x068A);
        }

        [Test]
        public void PIC16Basic_Disasm_xxxLW()
        {
            VerifyDisasm("ADDLW\t0x33", "", 0x3F33);
            VerifyDisasm("ADDLW\t0x88", "", 0x3E88);
            VerifyDisasm("ANDLW\t0x22", "", 0x3922);
            VerifyDisasm("ANDLW\t0x77", "", 0x3977);
            VerifyDisasm("IORLW\t0x22", "", 0x3822);
            VerifyDisasm("IORLW\t0x77", "", 0x3877);
            VerifyDisasm("MOVLW\t0x00", "", 0x3000);
            VerifyDisasm("MOVLW\t0xAA", "", 0x31AA);
            VerifyDisasm("MOVLW\t0x00", "", 0x3200);
            VerifyDisasm("MOVLW\t0xAA", "", 0x33AA);
            VerifyDisasm("RETLW\t0x00", "", 0x3400);
            VerifyDisasm("RETLW\t0xAA", "", 0x35AA);
            VerifyDisasm("RETLW\t0x00", "", 0x3600);
            VerifyDisasm("RETLW\t0xAA", "", 0x37AA);
            VerifyDisasm("SUBLW\t0x00", "", 0x3C00);
            VerifyDisasm("SUBLW\t0xAA", "", 0x3DAA);
            VerifyDisasm("XORLW\t0x00", "", 0x3A00);
            VerifyDisasm("XORLW\t0xAA", "", 0x3AAA);
        }

        [Test]
        public void PIC16All_Disasm_Invalids()
        {
            VerifyDisasm("invalid", "unknown opcode", 0x0002);
            VerifyDisasm("invalid", "unknown opcode", 0x0003);
            VerifyDisasm("invalid", "unknown opcode", 0x0004);
            VerifyDisasm("invalid", "unknown opcode", 0x0005);
            VerifyDisasm("invalid", "unknown opcode", 0x0006);
            VerifyDisasm("invalid", "unknown opcode", 0x0007);
            VerifyDisasm("invalid", "unknown opcode", 0x000C);
            VerifyDisasm("invalid", "unknown opcode", 0x000D);
            VerifyDisasm("invalid", "unknown opcode", 0x000E);
            VerifyDisasm("invalid", "unknown opcode", 0x000F);
            VerifyDisasm("invalid", "unknown opcode", 0x0061);
            VerifyDisasm("invalid", "unknown opcode", 0x0062);
        }

        [Test]
        public void PIC16Basic_Disasm_Invalids()
        {
            VerifyDisasm("invalid", "unknown opcode", 0x0001);
            VerifyDisasm("invalid", "unknown opcode", 0x000A);
            VerifyDisasm("invalid", "unknown opcode", 0x000B);
            VerifyDisasm("invalid", "unknown opcode", 0x0065);
            VerifyDisasm("invalid", "unknown opcode", 0x0066);
            VerifyDisasm("invalid", "unknown opcode", 0x0067);
            for (ushort uCode = 0x0010; uCode <= 0x1F; uCode++)
            {
                VerifyDisasm("invalid", "unknown opcode", uCode);
            }
        }

    }

}
