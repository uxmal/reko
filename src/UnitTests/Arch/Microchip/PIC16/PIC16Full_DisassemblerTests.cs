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

    // This series of tests permits to check the instructions of the Full-Featured PIC16.
    // 
    [TestFixture]
    public class PIC16Full_DisassemblerTests : DisassemblerTestsBase
    {

        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC16FullFeaturedName);
        }

        [Test]
        public void PIC16Full_Disasm_basics()
        {
            VerifyDisasm("ADDWF\t0x12,W", "", 0x0712);
            VerifyDisasm("ANDWF\t0x23,F", "", 0x05A3);
            VerifyDisasm("BCF\tSTATUS,C", "", 0x1003);
            VerifyDisasm("BSF\tSTATUS,DC", "", 0x1483);
            VerifyDisasm("BTFSC\tINDF0,7", "", 0x1B80);
            VerifyDisasm("BTFSS\tINDF0,0", "", 0x1C00);
            VerifyDisasm("CALL\t0x00000006", "", 0x2003);
            VerifyDisasm("CLRF\tFSR0L", "", 0x0184);
            VerifyDisasm("CLRF\tFSR0H", "", 0x0185);
            VerifyDisasm("CLRWDT", "", 0x0064);
            VerifyDisasm("COMF\t0x23,W", "", 0x0923);
            VerifyDisasm("DECF\tINDF1,F", "", 0x0381);
            VerifyDisasm("DECFSZ\tINDF0,W", "", 0x0B00);
            VerifyDisasm("GOTO\t0x00000EAC", "", 0x2F56);
            VerifyDisasm("INCF\tINDF1,F", "", 0x0A81);
            VerifyDisasm("INCFSZ\tPCLATH,F", "", 0x0F8A);
            VerifyDisasm("IORWF\tWREG,F", "", 0x0489);
            VerifyDisasm("MOVF\t0x12,W", "", 0x0812);
            VerifyDisasm("MOVWF\tFSR0L", "", 0x0084);
            VerifyDisasm("RETFIE", "", 0x0009);
            VerifyDisasm("RETURN", "", 0x0008);
            VerifyDisasm("RLF\t0x6A,F", "", 0x0DEA);
            VerifyDisasm("RRF\t0x77,W", "", 0x0C77);
            VerifyDisasm("SLEEP", "", 0x0063);
            VerifyDisasm("SUBWF\t0x69,F", "", 0x02E9);
            VerifyDisasm("SWAPF\tSTATUS,W", "", 0x0E03);
            VerifyDisasm("XORWF\t0x61,F", "", 0x06E1);
        }

        [Test]
        public void PIC16Full_Disasm_ADDFSR()
        {
            VerifyDisasm("ADDFSR\tFSR0,0x12", "", 0x3112);
            VerifyDisasm("ADDFSR\tFSR1,0x1A", "", 0x315A);
            VerifyDisasm("ADDFSR\tFSR1,-0x0D", "", 0x3173);
            VerifyDisasm("ADDFSR\tFSR0,0x00", "", 0x3100);
        }

        [Test]
        public void PIC16Full_Disasm_ADDWFC()
        {
            VerifyDisasm("ADDWFC\t0x12,W", "", 0x3D12);
            VerifyDisasm("ADDWFC\t0x5A,W", "", 0x3D5A);
            VerifyDisasm("ADDWFC\t0x73,W", "", 0x3D73);
            VerifyDisasm("ADDWFC\tINDF0,W", "", 0x3D00);
            VerifyDisasm("ADDWFC\tINDF0,F", "", 0x3D80);
        }

        [Test]
        public void PIC16Full_Disasm_ASRF()
        {
            VerifyDisasm("ASRF\t0x23,W", "", 0x3723);
            VerifyDisasm("ASRF\t0x5A,W", "", 0x375A);
            VerifyDisasm("ASRF\tWREG,F", "", 0x3789);
            VerifyDisasm("ASRF\t0x5A,F", "", 0x37DA);
            VerifyDisasm("ASRF\t0x1A,F", "", 0x379A);
            VerifyDisasm("ASRF\t0x23,W", "", 0x3723);
            VerifyDisasm("ASRF\t0x5A,W", "", 0x375A);
            VerifyDisasm("ASRF\tPCLATH,F", "", 0x378A);
        }

        [Test]
        public void PIC16Full_Disasm_BRA()
        {
            VerifyDisasm("BRA\t0x00000202", "", 0x3200);
            VerifyDisasm("BRA\t0x00000302", "", 0x3280);
            VerifyDisasm("BRA\t0x00000106", "", 0x3382);
        }

        [Test]
        public void PIC16Full_Disasm_BRW()
        {
            VerifyDisasm("BRW", "", 0x000B);
        }

        public void PIC16Full_Disasm_CALLW()
        {
            VerifyDisasm("CALLW", "", 0x000A);
        }

        [Test]
        public void PIC16Full_Disasm_CLRW()
        {
            for (ushort uCode = 0x0100; uCode < 0x0104; uCode++)
            {
                VerifyDisasm("CLRW", "", uCode);
            }
        }

        [Test]
        public void PIC16Full_Disasm_LSLF()
        {
            VerifyDisasm("LSLF\t0x23,W", "", 0x3523);
            VerifyDisasm("LSLF\t0x5A,W", "", 0x355A);
            VerifyDisasm("LSLF\tWREG,F", "", 0x3589);
            VerifyDisasm("LSLF\t0x5A,F", "", 0x35DA);
            VerifyDisasm("LSLF\t0x1A,F", "", 0x359A);
            VerifyDisasm("LSLF\t0x23,W", "", 0x3523);
            VerifyDisasm("LSLF\t0x5A,W", "", 0x355A);
            VerifyDisasm("LSLF\tPCLATH,F", "", 0x358A);
        }

        [Test]
        public void PIC16Full_Disasm_LSRF()
        {
            VerifyDisasm("LSRF\t0x23,W", "", 0x3623);
            VerifyDisasm("LSRF\t0x5A,W", "", 0x365A);
            VerifyDisasm("LSRF\tWREG,F", "", 0x3689);
            VerifyDisasm("LSRF\t0x5A,F", "", 0x36DA);
            VerifyDisasm("LSRF\t0x1A,F", "", 0x369A);
            VerifyDisasm("LSRF\t0x23,W", "", 0x3623);
            VerifyDisasm("LSRF\t0x5A,W", "", 0x365A);
            VerifyDisasm("LSRF\tPCLATH,F", "", 0x368A);
        }

        [Test]
        public void PIC16Full_Disasm_MOVIW()
        {
            VerifyDisasm("MOVIW\t++FSR0", "", 0x0010);
            VerifyDisasm("MOVIW\t--FSR0", "", 0x0011);
            VerifyDisasm("MOVIW\tFSR0++", "", 0x0012);
            VerifyDisasm("MOVIW\tFSR0--", "", 0x0013);
            VerifyDisasm("MOVIW\t++FSR1", "", 0x0014);
            VerifyDisasm("MOVIW\t--FSR1", "", 0x0015);
            VerifyDisasm("MOVIW\tFSR1++", "", 0x0016);
            VerifyDisasm("MOVIW\tFSR1--", "", 0x0017);

            VerifyDisasm("MOVIW\t0[0]", "", 0x3F00);
            VerifyDisasm("MOVIW\t0[1]", "", 0x3F40);
            VerifyDisasm("MOVIW\t30[0]", "", 0x3F1E);
            VerifyDisasm("MOVIW\t22[1]", "", 0x3F56);
            VerifyDisasm("MOVIW\t-2[0]", "", 0x3F3E);
            VerifyDisasm("MOVIW\t-10[1]", "", 0x3F76);
        }

        [Test]
        public void PIC16Full_Disasm_MOVLB()
        {
            VerifyDisasm("MOVLB\t0x00", "", 0x0140);
            VerifyDisasm("MOVLB\t0x12", "", 0x0152);
            VerifyDisasm("MOVLB\t0x3F", "", 0x017F);
        }

        [Test]
        public void PIC16Full_Disasm_MOVLP()
        {
            VerifyDisasm("MOVLP\t0x00", "", 0x3180);
            VerifyDisasm("MOVLP\t0x12", "", 0x3192);
            VerifyDisasm("MOVLP\t0x5A", "", 0x31DA);
            VerifyDisasm("MOVLP\t0x69", "", 0x31E9);
            VerifyDisasm("MOVLP\t0x0A", "", 0x318A);
        }

        [Test]
        public void PIC16Full_Disasm_MOVWI()
        {
            VerifyDisasm("MOVWI\t++FSR0", "", 0x0018);
            VerifyDisasm("MOVWI\t--FSR0", "", 0x0019);
            VerifyDisasm("MOVWI\tFSR0++", "", 0x001A);
            VerifyDisasm("MOVWI\tFSR0--", "", 0x001B);
            VerifyDisasm("MOVWI\t++FSR1", "", 0x001C);
            VerifyDisasm("MOVWI\t--FSR1", "", 0x001D);
            VerifyDisasm("MOVWI\tFSR1++", "", 0x001E);
            VerifyDisasm("MOVWI\tFSR1--", "", 0x001F);

            VerifyDisasm("MOVWI\t0[0]", "", 0x3F80);
            VerifyDisasm("MOVWI\t0[1]", "", 0x3FC0);
            VerifyDisasm("MOVWI\t30[0]", "", 0x3F9E);
            VerifyDisasm("MOVWI\t22[1]", "", 0x3FD6);
            VerifyDisasm("MOVWI\t-2[0]", "", 0x3FBE);
            VerifyDisasm("MOVWI\t-10[1]", "", 0x3FF6);
        }

        [Test]
        public void PIC16Full_Disasm_NOP()
        {
            VerifyDisasm("NOP", "", 0x0000);
        }

        [Test]
        public void PIC16Full_Disasm_RESET()
        {
            VerifyDisasm("RESET", "", 0x0001);
        }

        [Test]
        public void PIC16Full_Disasm_SUBWFB()
        {
            VerifyDisasm("SUBWFB\t0x12,W", "", 0x3B12);
            VerifyDisasm("SUBWFB\t0x5A,W", "", 0x3B5A);
            VerifyDisasm("SUBWFB\t0x69,F", "", 0x3BE9);
            VerifyDisasm("SUBWFB\t0x23,W", "", 0x3B23);
            VerifyDisasm("SUBWFB\t0x77,W", "", 0x3B77);
            VerifyDisasm("SUBWFB\t0x61,F", "", 0x3BE1);
            VerifyDisasm("SUBWFB\tSTATUS,W", "", 0x3B03);
            VerifyDisasm("SUBWFB\tPCLATH,F", "", 0x3B8A);
        }

        [Test]
        public void PIC16Enhd_Disasm_TRIS()
        {
            VerifyDisasm("TRIS\tA", "", 0x0065);
            VerifyDisasm("TRIS\tB", "", 0x0066);
            VerifyDisasm("TRIS\tC", "", 0x0067);
        }

        [Test]
        public void PIC16Full_Disasm_xxxLW()
        {
            VerifyDisasm("ADDLW\t0x88", "", 0x3E88);
            VerifyDisasm("ANDLW\t0x22", "", 0x3922);
            VerifyDisasm("ANDLW\t0x77", "", 0x3977);
            VerifyDisasm("IORLW\t0x22", "", 0x3822);
            VerifyDisasm("IORLW\t0x77", "", 0x3877);
            VerifyDisasm("MOVLW\t0x55", "", 0x3055);
            VerifyDisasm("RETLW\t0x01", "", 0x3401);
            VerifyDisasm("SUBLW\t0x05", "", 0x3C05);
            VerifyDisasm("XORLW\t0x00", "", 0x3A00);
            VerifyDisasm("XORLW\t0xAA", "", 0x3AAA);
        }

        [Test]
        public void PIC16Full_Disasm_Invalids()
        {
            VerifyDisasm("invalid", "unknown opcode", 0x0020);
            VerifyDisasm("invalid", "unknown opcode", 0x0040);
            VerifyDisasm("invalid", "unknown opcode", 0x0060);
        }

    }

}
