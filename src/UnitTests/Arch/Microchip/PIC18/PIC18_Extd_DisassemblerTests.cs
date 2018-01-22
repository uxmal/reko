#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

using Microchip.Crownking;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Legacy.Extended
{
    /// <summary>
    /// As of today there are 3 flavors of PIC18 :
    ///  - legacy (a.k.a. "pic18"),
    ///  - extended (a.k.a. "egg")
    ///  - enhanced (a.k.a. "cpu_pic18f_v6")
    /// And there are 2 execution modes (traditional and extended).
    /// This means we have 6 different contexts to check. Note: Legacy PIC18 supports traditional
    /// execution mode only.
    /// </summary>
    [TestFixture]
    public class PIC18_Extd_DisassemblerTests : PICDisassemblerTestsBase
    {

        [Test]
        [Explicit("For debugging purpose")]
        [Ignore("For debugging purpose")]
        public void Disasm_OneInstruction()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MULWF\tWREG,ACCESS", "", 0x02E8);
        }

        [Test]
        public void Disasm_NOP()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("NOP", "", 0x0000);
            VerifyDisasm("NOP", "", 0xF000);
            VerifyDisasm("NOP", "", 0xF528);
        }

        [Test]
        public void Disasm_SLEEP()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SLEEP", "", 0x0003);
        }

        [Test]
        public void Disasm_CLRWDT()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CLRWDT", "", 0x0004);
        }

        [Test]
        public void Disasm_PUSH()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("PUSH", "", 0x0005);
        }

        [Test]
        public void Disasm_POP()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("POP", "", 0x0006);
        }

        [Test]
        public void Disasm_DAW()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("DAW", "", 0x0007);
        }

        [Test]
        public void Disasm_TBLRD()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("TBLRD\t*", "", 0x0008);
            VerifyDisasm("TBLRD\t*+", "", 0x0009);
            VerifyDisasm("TBLRD\t*-", "", 0x000A);
            VerifyDisasm("TBLRD\t+*", "", 0x000B);
        }

        [Test]
        public void Disasm_TBLWT()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("TBLWT\t*", "", 0x000C);
            VerifyDisasm("TBLWT\t*+", "", 0x000D);
            VerifyDisasm("TBLWT\t*-", "", 0x000E);
            VerifyDisasm("TBLWT\t+*", "", 0x000F);
        }

        [Test]
        public void Disasm_RETFIE()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RETFIE", "", 0x0010);
            VerifyDisasm("RETFIE\tS", "", 0x0011);
        }

        [Test]
        public void Disasm_RETURN()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RETURN", "", 0x0012);
            VerifyDisasm("RETURN\tS", "", 0x0013);
        }

        [Test]
        public void Disasm_RESET()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RESET", "", 0x00FF);
        }

        [Test]
        public void Disasm_MOVLB()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MOVLB\t0x00", "", 0x0100);
            VerifyDisasm("MOVLB\t0x07", "", 0x0107);
            VerifyDisasm("MOVLB\t0x0F", "", 0x010F);
        }

        [Test]
        public void Disasm_MULWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MULWF\t0x01,ACCESS", "", 0x0201);
            VerifyDisasm("MULWF\t0x43,ACCESS", "", 0x0243);
            VerifyDisasm("MULWF\tWREG,ACCESS", "", 0x02E8);
            VerifyDisasm("MULWF\t0x00", "", 0x0300);
            VerifyDisasm("MULWF\t0x43", "", 0x0343);
            VerifyDisasm("MULWF\t0xE8", "", 0x03E8);
        }

        [Test]
        public void Disasm_DECF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("DECF\t0x01,W,ACCESS", "", 0x0401);
            VerifyDisasm("DECF\t0x5F,W,ACCESS", "", 0x045F);
            VerifyDisasm("DECF\tPLUSW2,W,ACCESS", "", 0x04DB);
            VerifyDisasm("DECF\t0x44,F,ACCESS", "", 0x0644);
            VerifyDisasm("DECF\tINDF1,F,ACCESS", "", 0x06E7);
            VerifyDisasm("DECF\t0x01,W", "", 0x0501);
            VerifyDisasm("DECF\t0x5F,W", "", 0x055F);
            VerifyDisasm("DECF\t0xBB,W", "", 0x05BB);
            VerifyDisasm("DECF\t0x44,F", "", 0x0744);
            VerifyDisasm("DECF\t0xDB,F", "", 0x07DB);
        }

        [Test]
        public void Disasm_INCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("INCF\t0x01,W,ACCESS", "", 0x2801);
            VerifyDisasm("INCF\t0x5F,W,ACCESS", "", 0x285F);
            VerifyDisasm("INCF\tPLUSW2,W,ACCESS", "", 0x28DB);
            VerifyDisasm("INCF\t0x44,F,ACCESS", "", 0x2A44);
            VerifyDisasm("INCF\tINDF1,F,ACCESS", "", 0x2AE7);
            VerifyDisasm("INCF\t0x01,W", "", 0x2901);
            VerifyDisasm("INCF\t0x5F,W", "", 0x295F);
            VerifyDisasm("INCF\t0xBB,W", "", 0x29BB);
            VerifyDisasm("INCF\t0x44,F", "", 0x2B44);
            VerifyDisasm("INCF\t0xDB,F", "", 0x2BDB);
        }

        [Test]
        public void Disasm_xxxLW()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SUBLW\t0x00", "", 0x0800);
            VerifyDisasm("SUBLW\t0xAA", "", 0x08AA);
            VerifyDisasm("IORLW\t0x22", "", 0x0922);
            VerifyDisasm("IORLW\t0x77", "", 0x0977);
            VerifyDisasm("XORLW\t0x00", "", 0x0A00);
            VerifyDisasm("XORLW\t0xAA", "", 0x0AAA);
            VerifyDisasm("ANDLW\t0x22", "", 0x0B22);
            VerifyDisasm("ANDLW\t0x77", "", 0x0B77);
            VerifyDisasm("RETLW\t0x00", "", 0x0C00);
            VerifyDisasm("RETLW\t0xAA", "", 0x0CAA);
            VerifyDisasm("MULLW\t0x22", "", 0x0D22);
            VerifyDisasm("MULLW\t0x77", "", 0x0D77);
            VerifyDisasm("MOVLW\t0x00", "", 0x0E00);
            VerifyDisasm("MOVLW\t0xAA", "", 0x0EAA);
            VerifyDisasm("ADDLW\t0x33", "", 0x0F33);
            VerifyDisasm("ADDLW\t0x88", "", 0x0F88);
        }

        [Test]
        public void Disasm_IORWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("IORWF\t0x23,W,ACCESS", "", 0x1023);
            VerifyDisasm("IORWF\t0x5A,W,ACCESS", "", 0x105A);
            VerifyDisasm("IORWF\tLATA,W,ACCESS", "", 0x1089);
            VerifyDisasm("IORWF\t0x23,F,ACCESS", "", 0x1223);
            VerifyDisasm("IORWF\t0x5A,F,ACCESS", "", 0x125A);
            VerifyDisasm("IORWF\tLATB,F,ACCESS", "", 0x128A);
            VerifyDisasm("IORWF\t0x23,W", "", 0x1123);
            VerifyDisasm("IORWF\t0x5A,W", "", 0x115A);
            VerifyDisasm("IORWF\t0x8A,W", "", 0x118A);
            VerifyDisasm("IORWF\t0x23,F", "", 0x1323);
            VerifyDisasm("IORWF\t0x5A,F", "", 0x135A);
            VerifyDisasm("IORWF\t0x89,F", "", 0x1389);
        }

        [Test]
        public void Disasm_ANDWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("ANDWF\t0x23,W,ACCESS", "", 0x1423);
            VerifyDisasm("ANDWF\t0x5A,W,ACCESS", "", 0x145A);
            VerifyDisasm("ANDWF\tLATA,W,ACCESS", "", 0x1489);
            VerifyDisasm("ANDWF\t0x23,F,ACCESS", "", 0x1623);
            VerifyDisasm("ANDWF\t0x5A,F,ACCESS", "", 0x165A);
            VerifyDisasm("ANDWF\tLATB,F,ACCESS", "", 0x168A);
            VerifyDisasm("ANDWF\t0x23,W", "", 0x1523);
            VerifyDisasm("ANDWF\t0x5A,W", "", 0x155A);
            VerifyDisasm("ANDWF\t0x8A,W", "", 0x158A);
            VerifyDisasm("ANDWF\t0x23,F", "", 0x1723);
            VerifyDisasm("ANDWF\t0x5A,F", "", 0x175A);
            VerifyDisasm("ANDWF\t0x89,F", "", 0x1789);
        }

        [Test]
        public void Disasm_XORWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("XORWF\t0x23,W,ACCESS", "", 0x1823);
            VerifyDisasm("XORWF\t0x5A,W,ACCESS", "", 0x185A);
            VerifyDisasm("XORWF\tLATA,W,ACCESS", "", 0x1889);
            VerifyDisasm("XORWF\t0x23,F,ACCESS", "", 0x1A23);
            VerifyDisasm("XORWF\t0x5A,F,ACCESS", "", 0x1A5A);
            VerifyDisasm("XORWF\tLATB,F,ACCESS", "", 0x1A8A);
            VerifyDisasm("XORWF\t0x23,W", "", 0x1923);
            VerifyDisasm("XORWF\t0x5A,W", "", 0x195A);
            VerifyDisasm("XORWF\t0x8A,W", "", 0x198A);
            VerifyDisasm("XORWF\t0x23,F", "", 0x1B23);
            VerifyDisasm("XORWF\t0x5A,F", "", 0x1B5A);
            VerifyDisasm("XORWF\t0x89,F", "", 0x1B89);
        }

        [Test]
        public void Disasm_COMF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("COMF\t0x12,W,ACCESS", "", 0x1C12);
            VerifyDisasm("COMF\t0x5A,W,ACCESS", "", 0x1C5A);
            VerifyDisasm("COMF\tLATA,W,ACCESS", "", 0x1C89);
            VerifyDisasm("COMF\t0x23,F,ACCESS", "", 0x1E23);
            VerifyDisasm("COMF\t0x77,F,ACCESS", "", 0x1E77);
            VerifyDisasm("COMF\tLATB,F,ACCESS", "", 0x1E8A);
            VerifyDisasm("COMF\t0x23,W", "", 0x1D23);
            VerifyDisasm("COMF\t0x5A,W", "", 0x1D5A);
            VerifyDisasm("COMF\t0x8A,W", "", 0x1D8A);
            VerifyDisasm("COMF\t0x23,F", "", 0x1F23);
            VerifyDisasm("COMF\t0x5A,F", "", 0x1F5A);
            VerifyDisasm("COMF\t0x89,F", "", 0x1F89);
        }

        [Test]
        public void Disasm_ADDWFC()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("ADDWFC\t0x12,W,ACCESS", "", 0x2012);
            VerifyDisasm("ADDWFC\t0x5A,W,ACCESS", "", 0x205A);
            VerifyDisasm("ADDWFC\tEEADR,W,ACCESS", "", 0x20A9);
            VerifyDisasm("ADDWFC\t0x23,F,ACCESS", "", 0x2223);
            VerifyDisasm("ADDWFC\t0x77,F,ACCESS", "", 0x2277);
            VerifyDisasm("ADDWFC\tLATB,F,ACCESS", "", 0x228A);
            VerifyDisasm("ADDWFC\t0x23,W", "", 0x2123);
            VerifyDisasm("ADDWFC\t0x5A,W", "", 0x215A);
            VerifyDisasm("ADDWFC\t0x8A,W", "", 0x218A);
            VerifyDisasm("ADDWFC\t0x23,F", "", 0x2323);
            VerifyDisasm("ADDWFC\t0x5A,F", "", 0x235A);
            VerifyDisasm("ADDWFC\t0x89,F", "", 0x2389);
        }

        [Test]
        public void Disasm_ADDWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("ADDWF\t0x12,W,ACCESS", "", 0x2412);
            VerifyDisasm("ADDWF\t0x5A,W,ACCESS", "", 0x245A);
            VerifyDisasm("ADDWF\tEEADR,W,ACCESS", "", 0x24A9);
            VerifyDisasm("ADDWF\t0x23,F,ACCESS", "", 0x2623);
            VerifyDisasm("ADDWF\t0x77,F,ACCESS", "", 0x2677);
            VerifyDisasm("ADDWF\tLATB,F,ACCESS", "", 0x268A);
            VerifyDisasm("ADDWF\t0x03,W", "", 0x2503);
            VerifyDisasm("ADDWF\t0x5A,W", "", 0x255A);
            VerifyDisasm("ADDWF\t0x8A,W", "", 0x258A);
            VerifyDisasm("ADDWF\t0x23,F", "", 0x2723);
            VerifyDisasm("ADDWF\t0x5A,F", "", 0x275A);
            VerifyDisasm("ADDWF\t0x89,F", "", 0x2789);
        }

        [Test]
        public void Disasm_DECFSZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("DECFSZ\t0x12,W,ACCESS", "", 0x2C12);
            VerifyDisasm("DECFSZ\t0x5A,W,ACCESS", "", 0x2C5A);
            VerifyDisasm("DECFSZ\tFSR0H,W,ACCESS", "", 0x2CEA);
            VerifyDisasm("DECFSZ\t0x23,F,ACCESS", "", 0x2E23);
            VerifyDisasm("DECFSZ\t0x77,F,ACCESS", "", 0x2E77);
            VerifyDisasm("DECFSZ\tFSR1L,F,ACCESS", "", 0x2EE1);
            VerifyDisasm("DECFSZ\t0x03,W", "", 0x2D03);
            VerifyDisasm("DECFSZ\t0x5A,W", "", 0x2D5A);
            VerifyDisasm("DECFSZ\t0x8A,W", "", 0x2D8A);
            VerifyDisasm("DECFSZ\t0x23,F", "", 0x2F23);
            VerifyDisasm("DECFSZ\t0x5A,F", "", 0x2F5A);
            VerifyDisasm("DECFSZ\t0xE1,F", "", 0x2FE1);
        }

        [Test]
        public void Disasm_INCFSZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("INCFSZ\t0x12,W,ACCESS", "", 0x3C12);
            VerifyDisasm("INCFSZ\t0x5A,W,ACCESS", "", 0x3C5A);
            VerifyDisasm("INCFSZ\tFSR0L,W,ACCESS", "", 0x3CE9);
            VerifyDisasm("INCFSZ\t0x23,F,ACCESS", "", 0x3E23);
            VerifyDisasm("INCFSZ\t0x77,F,ACCESS", "", 0x3E77);
            VerifyDisasm("INCFSZ\tFSR1L,F,ACCESS", "", 0x3EE1);
            VerifyDisasm("INCFSZ\t0x03,W", "", 0x3D03);
            VerifyDisasm("INCFSZ\t0x5A,W", "", 0x3D5A);
            VerifyDisasm("INCFSZ\t0x8A,W", "", 0x3D8A);
            VerifyDisasm("INCFSZ\t0x23,F", "", 0x3F23);
            VerifyDisasm("INCFSZ\t0x5A,F", "", 0x3F5A);
            VerifyDisasm("INCFSZ\t0xE1,F", "", 0x3FE1);
        }

        [Test]
        public void Disasm_RRCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RRCF\t0x12,W,ACCESS", "", 0x3012);
            VerifyDisasm("RRCF\t0x5A,W,ACCESS", "", 0x305A);
            VerifyDisasm("RRCF\tFSR0L,W,ACCESS", "", 0x30E9);
            VerifyDisasm("RRCF\t0x23,F,ACCESS", "", 0x3223);
            VerifyDisasm("RRCF\t0x77,F,ACCESS", "", 0x3277);
            VerifyDisasm("RRCF\tFSR1L,F,ACCESS", "", 0x32E1);
            VerifyDisasm("RRCF\t0x03,W", "", 0x3103);
            VerifyDisasm("RRCF\t0x5A,W", "", 0x315A);
            VerifyDisasm("RRCF\t0x8A,W", "", 0x318A);
            VerifyDisasm("RRCF\t0x23,F", "", 0x3323);
            VerifyDisasm("RRCF\t0x5A,F", "", 0x335A);
            VerifyDisasm("RRCF\t0xE1,F", "", 0x33E1);
        }

        [Test]
        public void Disasm_RLCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RLCF\t0x12,W,ACCESS", "", 0x3412);
            VerifyDisasm("RLCF\t0x5A,W,ACCESS", "", 0x345A);
            VerifyDisasm("RLCF\tFSR0H,W,ACCESS", "", 0x34EA);
            VerifyDisasm("RLCF\t0x23,F,ACCESS", "", 0x3623);
            VerifyDisasm("RLCF\t0x77,F,ACCESS", "", 0x3677);
            VerifyDisasm("RLCF\tFSR1L,F,ACCESS", "", 0x36E1);
            VerifyDisasm("RLCF\t0x03,W", "", 0x3503);
            VerifyDisasm("RLCF\t0x5A,W", "", 0x355A);
            VerifyDisasm("RLCF\t0x8A,W", "", 0x358A);
            VerifyDisasm("RLCF\t0x23,F", "", 0x3723);
            VerifyDisasm("RLCF\t0x5A,F", "", 0x375A);
            VerifyDisasm("RLCF\t0xE1,F", "", 0x37E1);
        }

        [Test]
        public void Disasm_SWAPF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SWAPF\t0x12,W,ACCESS", "", 0x3812);
            VerifyDisasm("SWAPF\t0x5A,W,ACCESS", "", 0x385A);
            VerifyDisasm("SWAPF\tFSR0L,W,ACCESS", "", 0x38E9);
            VerifyDisasm("SWAPF\t0x23,F,ACCESS", "", 0x3A23);
            VerifyDisasm("SWAPF\t0x77,F,ACCESS", "", 0x3A77);
            VerifyDisasm("SWAPF\tFSR1L,F,ACCESS", "", 0x3AE1);
            VerifyDisasm("SWAPF\t0x03,W", "", 0x3903);
            VerifyDisasm("SWAPF\t0x5A,W", "", 0x395A);
            VerifyDisasm("SWAPF\t0x8A,W", "", 0x398A);
            VerifyDisasm("SWAPF\t0x23,F", "", 0x3B23);
            VerifyDisasm("SWAPF\t0x5A,F", "", 0x3B5A);
            VerifyDisasm("SWAPF\t0xE1,F", "", 0x3BE1);
        }

        [Test]
        public void Disasm_RRNCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RRNCF\t0x12,W,ACCESS", "", 0x4012);
            VerifyDisasm("RRNCF\t0x5A,W,ACCESS", "", 0x405A);
            VerifyDisasm("RRNCF\tFSR0L,W,ACCESS", "", 0x40E9);
            VerifyDisasm("RRNCF\t0x23,F,ACCESS", "", 0x4223);
            VerifyDisasm("RRNCF\t0x77,F,ACCESS", "", 0x4277);
            VerifyDisasm("RRNCF\tFSR1L,F,ACCESS", "", 0x42E1);
            VerifyDisasm("RRNCF\t0x03,W", "", 0x4103);
            VerifyDisasm("RRNCF\t0x5A,W", "", 0x415A);
            VerifyDisasm("RRNCF\t0x8A,W", "", 0x418A);
            VerifyDisasm("RRNCF\t0x23,F", "", 0x4323);
            VerifyDisasm("RRNCF\t0x5A,F", "", 0x435A);
            VerifyDisasm("RRNCF\t0xE1,F", "", 0x43E1);
        }

        [Test]
        public void Disasm_RLNCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RLNCF\t0x12,W,ACCESS", "", 0x4412);
            VerifyDisasm("RLNCF\t0x5A,W,ACCESS", "", 0x445A);
            VerifyDisasm("RLNCF\tFSR0H,W,ACCESS", "", 0x44EA);
            VerifyDisasm("RLNCF\t0x23,F,ACCESS", "", 0x4623);
            VerifyDisasm("RLNCF\t0x77,F,ACCESS", "", 0x4677);
            VerifyDisasm("RLNCF\tFSR1L,F,ACCESS", "", 0x46E1);
            VerifyDisasm("RLNCF\t0x03,W", "", 0x4503);
            VerifyDisasm("RLNCF\t0x5A,W", "", 0x455A);
            VerifyDisasm("RLNCF\t0x8A,W", "", 0x458A);
            VerifyDisasm("RLNCF\t0x23,F", "", 0x4723);
            VerifyDisasm("RLNCF\t0x5A,F", "", 0x475A);
            VerifyDisasm("RLNCF\t0xE1,F", "", 0x47E1);
        }

        [Test]
        public void Disasm_INFSNZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("INFSNZ\t0x12,W,ACCESS", "", 0x4812);
            VerifyDisasm("INFSNZ\t0x5A,W,ACCESS", "", 0x485A);
            VerifyDisasm("INFSNZ\tFSR0H,W,ACCESS", "", 0x48EA);
            VerifyDisasm("INFSNZ\t0x23,F,ACCESS", "", 0x4A23);
            VerifyDisasm("INFSNZ\t0x77,F,ACCESS", "", 0x4A77);
            VerifyDisasm("INFSNZ\tFSR1L,F,ACCESS", "", 0x4AE1);
            VerifyDisasm("INFSNZ\t0x03,W", "", 0x4903);
            VerifyDisasm("INFSNZ\t0x5A,W", "", 0x495A);
            VerifyDisasm("INFSNZ\t0x8A,W", "", 0x498A);
            VerifyDisasm("INFSNZ\t0x23,F", "", 0x4B23);
            VerifyDisasm("INFSNZ\t0x5A,F", "", 0x4B5A);
            VerifyDisasm("INFSNZ\t0xE1,F", "", 0x4BE1);
        }

        [Test]
        public void Disasm_DCFSNZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("DCFSNZ\t0x12,W,ACCESS", "", 0x4C12);
            VerifyDisasm("DCFSNZ\t0x5A,W,ACCESS", "", 0x4C5A);
            VerifyDisasm("DCFSNZ\tFSR0L,W,ACCESS", "", 0x4CE9);
            VerifyDisasm("DCFSNZ\t0x23,F,ACCESS", "", 0x4E23);
            VerifyDisasm("DCFSNZ\t0x77,F,ACCESS", "", 0x4E77);
            VerifyDisasm("DCFSNZ\tFSR1L,F,ACCESS", "", 0x4EE1);
            VerifyDisasm("DCFSNZ\t0x03,W", "", 0x4D03);
            VerifyDisasm("DCFSNZ\t0x5A,W", "", 0x4D5A);
            VerifyDisasm("DCFSNZ\t0x8A,W", "", 0x4D8A);
            VerifyDisasm("DCFSNZ\t0x23,F", "", 0x4F23);
            VerifyDisasm("DCFSNZ\t0x5A,F", "", 0x4F5A);
            VerifyDisasm("DCFSNZ\t0xE1,F", "", 0x4FE1);
        }

        [Test]
        public void Disasm_MOVF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MOVF\t0x12,W,ACCESS", "", 0x5012);
            VerifyDisasm("MOVF\t0x5A,W,ACCESS", "", 0x505A);
            VerifyDisasm("MOVF\tFSR0L,W,ACCESS", "", 0x50E9);
            VerifyDisasm("MOVF\t0x23,F,ACCESS", "", 0x5223);
            VerifyDisasm("MOVF\t0x77,F,ACCESS", "", 0x5277);
            VerifyDisasm("MOVF\tFSR1L,F,ACCESS", "", 0x52E1);
            VerifyDisasm("MOVF\t0x03,W", "", 0x5103);
            VerifyDisasm("MOVF\t0x5A,W", "", 0x515A);
            VerifyDisasm("MOVF\t0x8A,W", "", 0x518A);
            VerifyDisasm("MOVF\t0x23,F", "", 0x5323);
            VerifyDisasm("MOVF\t0x5A,F", "", 0x535A);
            VerifyDisasm("MOVF\t0xE1,F", "", 0x53E1);
        }

        [Test]
        public void Disasm_SUBFWB()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SUBFWB\t0x12,W,ACCESS", "", 0x5412);
            VerifyDisasm("SUBFWB\t0x5A,W,ACCESS", "", 0x545A);
            VerifyDisasm("SUBFWB\tFSR0H,W,ACCESS", "", 0x54EA);
            VerifyDisasm("SUBFWB\t0x23,F,ACCESS", "", 0x5623);
            VerifyDisasm("SUBFWB\t0x77,F,ACCESS", "", 0x5677);
            VerifyDisasm("SUBFWB\tFSR1L,F,ACCESS", "", 0x56E1);
            VerifyDisasm("SUBFWB\t0x03,W", "", 0x5503);
            VerifyDisasm("SUBFWB\t0x5A,W", "", 0x555A);
            VerifyDisasm("SUBFWB\t0x8A,W", "", 0x558A);
            VerifyDisasm("SUBFWB\t0x23,F", "", 0x5723);
            VerifyDisasm("SUBFWB\t0x5A,F", "", 0x575A);
            VerifyDisasm("SUBFWB\t0xE1,F", "", 0x57E1);
        }

        [Test]
        public void Disasm_SUBWFB()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SUBWFB\t0x12,W,ACCESS", "", 0x5812);
            VerifyDisasm("SUBWFB\t0x5A,W,ACCESS", "", 0x585A);
            VerifyDisasm("SUBWFB\tFSR0H,W,ACCESS", "", 0x58EA);
            VerifyDisasm("SUBWFB\t0x23,F,ACCESS", "", 0x5A23);
            VerifyDisasm("SUBWFB\t0x77,F,ACCESS", "", 0x5A77);
            VerifyDisasm("SUBWFB\tFSR1L,F,ACCESS", "", 0x5AE1);
            VerifyDisasm("SUBWFB\t0x03,W", "", 0x5903);
            VerifyDisasm("SUBWFB\t0x5A,W", "", 0x595A);
            VerifyDisasm("SUBWFB\t0x8A,W", "", 0x598A);
            VerifyDisasm("SUBWFB\t0x23,F", "", 0x5B23);
            VerifyDisasm("SUBWFB\t0x5A,F", "", 0x5B5A);
            VerifyDisasm("SUBWFB\t0xE1,F", "", 0x5BE1);
        }

        [Test]
        public void Disasm_SUBWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SUBWF\t0x12,W,ACCESS", "", 0x5C12);
            VerifyDisasm("SUBWF\t0x5A,W,ACCESS", "", 0x5C5A);
            VerifyDisasm("SUBWF\tFSR0L,W,ACCESS", "", 0x5CE9);
            VerifyDisasm("SUBWF\t0x23,F,ACCESS", "", 0x5E23);
            VerifyDisasm("SUBWF\t0x77,F,ACCESS", "", 0x5E77);
            VerifyDisasm("SUBWF\tFSR1L,F,ACCESS", "", 0x5EE1);
            VerifyDisasm("SUBWF\t0x03,W", "", 0x5D03);
            VerifyDisasm("SUBWF\t0x5A,W", "", 0x5D5A);
            VerifyDisasm("SUBWF\t0x8A,W", "", 0x5D8A);
            VerifyDisasm("SUBWF\t0x23,F", "", 0x5F23);
            VerifyDisasm("SUBWF\t0x5A,F", "", 0x5F5A);
            VerifyDisasm("SUBWF\t0xE1,F", "", 0x5FE1);
        }

        [Test]
        public void Disasm_CPFSLT()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CPFSLT\t0x12,ACCESS", "", 0x6012);
            VerifyDisasm("CPFSLT\t0x5A,ACCESS", "", 0x605A);
            VerifyDisasm("CPFSLT\tFSR0L,ACCESS", "", 0x60E9);
            VerifyDisasm("CPFSLT\t0x03", "", 0x6103);
            VerifyDisasm("CPFSLT\t0x5A", "", 0x615A);
            VerifyDisasm("CPFSLT\t0x8A", "", 0x618A);
        }

        [Test]
        public void Disasm_CPFSEQ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CPFSEQ\t0x12,ACCESS", "", 0x6212);
            VerifyDisasm("CPFSEQ\t0x5A,ACCESS", "", 0x625A);
            VerifyDisasm("CPFSEQ\tFSR0H,ACCESS", "", 0x62EA);
            VerifyDisasm("CPFSEQ\t0x03", "", 0x6303);
            VerifyDisasm("CPFSEQ\t0x5A", "", 0x635A);
            VerifyDisasm("CPFSEQ\t0x8A", "", 0x638A);
        }

        [Test]
        public void Disasm_CPFSGT()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CPFSGT\t0x12,ACCESS", "", 0x6412);
            VerifyDisasm("CPFSGT\t0x5A,ACCESS", "", 0x645A);
            VerifyDisasm("CPFSGT\tFSR0H,ACCESS", "", 0x64EA);
            VerifyDisasm("CPFSGT\t0x03", "", 0x6503);
            VerifyDisasm("CPFSGT\t0x5A", "", 0x655A);
            VerifyDisasm("CPFSGT\t0x8A", "", 0x658A);
        }

        [Test]
        public void Disasm_TSTFSZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("TSTFSZ\t0x12,ACCESS", "", 0x6612);
            VerifyDisasm("TSTFSZ\t0x5A,ACCESS", "", 0x665A);
            VerifyDisasm("TSTFSZ\tFSR0L,ACCESS", "", 0x66E9);
            VerifyDisasm("TSTFSZ\t0x00", "", 0x6700);
            VerifyDisasm("TSTFSZ\t0x5A", "", 0x675A);
            VerifyDisasm("TSTFSZ\t0x8A", "", 0x678A);
        }

        [Test]
        public void Disasm_SETF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("SETF\t0x12,ACCESS", "", 0x6812);
            VerifyDisasm("SETF\t0x5A,ACCESS", "", 0x685A);
            VerifyDisasm("SETF\tFSR0L,ACCESS", "", 0x68E9);
            VerifyDisasm("SETF\t0x03", "", 0x6903);
            VerifyDisasm("SETF\t0x5A", "", 0x695A);
            VerifyDisasm("SETF\t0x8A", "", 0x698A);
        }

        [Test]
        public void Disasm_CLRF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CLRF\t0x12,ACCESS", "", 0x6A12);
            VerifyDisasm("CLRF\t0x5A,ACCESS", "", 0x6A5A);
            VerifyDisasm("CLRF\tFSR0H,ACCESS", "", 0x6AEA);
            VerifyDisasm("CLRF\t0x03", "", 0x6B03);
            VerifyDisasm("CLRF\t0x5A", "", 0x6B5A);
            VerifyDisasm("CLRF\t0x8A", "", 0x6B8A);
        }

        [Test]
        public void Disasm_NEGF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("NEGF\t0x12,ACCESS", "", 0x6C12);
            VerifyDisasm("NEGF\t0x5A,ACCESS", "", 0x6C5A);
            VerifyDisasm("NEGF\tFSR0H,ACCESS", "", 0x6CEA);
            VerifyDisasm("NEGF\t0x03", "", 0x6D03);
            VerifyDisasm("NEGF\t0x5A", "", 0x6D5A);
            VerifyDisasm("NEGF\t0x8A", "", 0x6D8A);
        }

        [Test]
        public void Disasm_MOVWF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MOVWF\t0x12,ACCESS", "", 0x6E12);
            VerifyDisasm("MOVWF\t0x5A,ACCESS", "", 0x6E5A);
            VerifyDisasm("MOVWF\tFSR0L,ACCESS", "", 0x6EE9);
            VerifyDisasm("MOVWF\t0x00", "", 0x6F00);
            VerifyDisasm("MOVWF\t0x5A", "", 0x6F5A);
            VerifyDisasm("MOVWF\t0x8A", "", 0x6F8A);
        }

        [Test]
        public void Disasm_BTG()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BTG\t0x03,0,ACCESS", "", 0x7003);
            VerifyDisasm("BTG\tTRISA,7,ACCESS", "", 0x7E92);
            VerifyDisasm("BTG\tTRISB,0,ACCESS", "", 0x7093);
            VerifyDisasm("BTG\t0x00,0", "", 0x7100);
            VerifyDisasm("BTG\t0x5A,1", "", 0x735A);
            VerifyDisasm("BTG\t0x8A,5", "", 0x7B8A);
        }

        [Test]
        public void Disasm_BSF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BSF\t0x03,0,ACCESS", "", 0x8003);
            VerifyDisasm("BSF\tPORTB,7,ACCESS", "", 0x8E81);
            VerifyDisasm("BSF\tPORTA,0,ACCESS", "", 0x8080);
            VerifyDisasm("BSF\t0x00,0", "", 0x8100);
            VerifyDisasm("BSF\t0x5A,1", "", 0x835A);
            VerifyDisasm("BSF\t0x8A,5", "", 0x8B8A);
        }

        [Test]
        public void Disasm_BCF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BCF\t0x03,0,ACCESS", "", 0x9003);
            VerifyDisasm("BCF\tPORTA,7,ACCESS", "", 0x9E80);
            VerifyDisasm("BCF\tPORTB,0,ACCESS", "", 0x9081);
            VerifyDisasm("BCF\t0x00,0", "", 0x9100);
            VerifyDisasm("BCF\t0x5A,1", "", 0x935A);
            VerifyDisasm("BCF\t0x8A,5", "", 0x9B8A);
        }

        [Test]
        public void Disasm_BTFSS()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BTFSS\t0x03,0,ACCESS", "", 0xA003);
            VerifyDisasm("BTFSS\tPORTA,7,ACCESS", "", 0xAE80);
            VerifyDisasm("BTFSS\tPORTB,0,ACCESS", "", 0xA081);
            VerifyDisasm("BTFSS\t0x00,0", "", 0xA100);
            VerifyDisasm("BTFSS\t0x5A,1", "", 0xA35A);
            VerifyDisasm("BTFSS\t0x8A,5", "", 0xAB8A);
        }

        [Test]
        public void Disasm_BTFSC()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BTFSC\t0x03,0,ACCESS", "", 0xB003);
            VerifyDisasm("BTFSC\tPORTA,7,ACCESS", "", 0xBE80);
            VerifyDisasm("BTFSC\tPORTB,0,ACCESS", "", 0xB081);
            VerifyDisasm("BTFSC\t0x00,0", "", 0xB100);
            VerifyDisasm("BTFSC\t0x5A,1", "", 0xB35A);
            VerifyDisasm("BTFSC\t0x8A,5", "", 0xBB8A);
        }

        [Test]
        public void Disasm_MOVFF()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("MOVFF\t0x0000,0x0123", "", 0xC000, 0xF123);
            VerifyDisasm("MOVFF\tPORTA,PORTB", "", 0xCF80, 0xFF81);
        }

        [Test]
        public void Disasm_BRA()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BRA\t0x000202", "", 0xD000);
            VerifyDisasm("BRA\t0x0002AC", "", 0xD055);
            VerifyDisasm("BRA\t0x0000AC", "", 0xD755);
            VerifyDisasm("BRA\t0x000200", "", 0xD7FF);
        }

        [Test]
        public void Disasm_RCALL()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("RCALL\t0x000204", "", 0xD801);
            VerifyDisasm("RCALL\t0x000356", "", 0xD8AA);
            VerifyDisasm("RCALL\t0x000A00", "", 0xDBFF);
            VerifyDisasm("RCALL\t0x1FFA02", "", 0xDC00);
            VerifyDisasm("RCALL\t0x000156", "", 0xDFAA);
            VerifyDisasm("RCALL\t0x000200", "", 0xDFFF);
        }

        [Test]
        public void Disasm_BZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BZ\t0x000202", "", 0xE000);
            VerifyDisasm("BZ\t0x000248", "", 0xE023);
            VerifyDisasm("BZ\t0x00010E", "", 0xE086);
            VerifyDisasm("BZ\t0x000200", "", 0xE0FF);
        }

        [Test]
        public void Disasm_BNZ()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BNZ\t0x000202", "", 0xE100);
            VerifyDisasm("BNZ\t0x000248", "", 0xE123);
            VerifyDisasm("BNZ\t0x00010E", "", 0xE186);
            VerifyDisasm("BNZ\t0x000200", "", 0xE1FF);
        }

        [Test]
        public void Disasm_BC()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BC\t0x000202", "", 0xE200);
            VerifyDisasm("BC\t0x000248", "", 0xE223);
            VerifyDisasm("BC\t0x00010E", "", 0xE286);
            VerifyDisasm("BC\t0x000200", "", 0xE2FF);
        }

        [Test]
        public void Disasm_BNC()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BNC\t0x000202", "", 0xE300);
            VerifyDisasm("BNC\t0x000248", "", 0xE323);
            VerifyDisasm("BNC\t0x00010E", "", 0xE386);
            VerifyDisasm("BNC\t0x000200", "", 0xE3FF);
        }

        [Test]
        public void Disasm_BOV()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BOV\t0x000202", "", 0xE400);
            VerifyDisasm("BOV\t0x000248", "", 0xE423);
            VerifyDisasm("BOV\t0x00010E", "", 0xE486);
            VerifyDisasm("BOV\t0x000200", "", 0xE4FF);
        }

        [Test]
        public void Disasm_BNOV()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BNOV\t0x000202", "", 0xE500);
            VerifyDisasm("BNOV\t0x000248", "", 0xE523);
            VerifyDisasm("BNOV\t0x00010E", "", 0xE586);
            VerifyDisasm("BNOV\t0x000200", "", 0xE5FF);
        }

        [Test]
        public void Disasm_BN()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BN\t0x000202", "", 0xE600);
            VerifyDisasm("BN\t0x000248", "", 0xE623);
            VerifyDisasm("BN\t0x00010E", "", 0xE686);
            VerifyDisasm("BN\t0x000200", "", 0xE6FF);
        }

        [Test]
        public void Disasm_BNN()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("BNN\t0x000202", "", 0xE700);
            VerifyDisasm("BNN\t0x000248", "", 0xE723);
            VerifyDisasm("BNN\t0x00010E", "", 0xE786);
            VerifyDisasm("BNN\t0x000200", "", 0xE7FF);
        }

        [Test]
        public void Disasm_CALL()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("CALL\t0x00000C", "", 0xEC06, 0xF000);
            VerifyDisasm("CALL\t0x068A24", "", 0xEC12, 0xF345);
            VerifyDisasm("CALL\t0x00000C,S", "", 0xED06, 0xF000);
            VerifyDisasm("CALL\t0x068A24,S", "", 0xED12, 0xF345);
        }

        [Test]
        public void Disasm_LFSR()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("LFSR\tFSR0,0x0300", "", 0xEE03, 0xF000);
            VerifyDisasm("LFSR\tFSR0,0x0034", "", 0xEE00, 0xF034);
            VerifyDisasm("LFSR\tFSR2,0x0689", "", 0xEE26, 0xF089);
        }

        [Test]
        public void Disasm_GOTO()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
            VerifyDisasm("GOTO\t0x000006", "", 0xEF03, 0xF000);
            VerifyDisasm("GOTO\t0x0F12AC", "", 0xEF56, 0xF789);
        }

        [Test]
        public void Disasm_Invalids()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Extended);
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

            VerifyDisasm("invalid", "(MOVSFL) unsupported instruction", 0x0002, 0xF000, 0xF000);
            VerifyDisasm("invalid", "(CALLW) unsupported instruction", 0x0014);
            VerifyDisasm("invalid", "(MOVFFL) unsupported instruction", 0x006F, 0xF000, 0xF000);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0110);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0120);
            VerifyDisasm("invalid", "(ADDFSR) not supported instruction", 0xE800);
            VerifyDisasm("invalid", "(ADDULNK) not supported instruction", 0xE8C0);
            VerifyDisasm("invalid", "(SUBFSR) not supported instruction", 0xE900);
            VerifyDisasm("invalid", "(SUBULNK) not supported instruction", 0xE9C0);
            VerifyDisasm("invalid", "(PUSHL) not supported instruction", 0xEA00);
            VerifyDisasm("invalid", "(ADDULNK) not supported instruction", 0xEB00, 0xF000);
            VerifyDisasm("invalid", "(MOVSS) not supported instruction", 0xEB80, 0xF000);
            VerifyDisasm("invalid", "(LFSR) too large value", 0xEE00, 0xF234);
        }

    }

}
