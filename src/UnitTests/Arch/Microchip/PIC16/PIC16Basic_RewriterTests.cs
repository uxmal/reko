#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Libraries.Microchip;
using NUnit.Framework;

namespace Reko.UnitTests.Arch.Microchip.PIC16.Rewriter
{

    using Common;
    using static Common.Sample;

    /// <summary>
    /// PIC16 basic rewriter tests.
    /// </summary>
    [TestFixture]
    public class PIC16Basic_RewriterTests : PICRewriterTestsBase
    {
        [TestFixtureSetUp]
        public void OneSetup()
        {
            SetPICMode(PIC16BasicName);
        }

        [Test]
        public void PIC16Basic_Rewriter_ADDLW()
        {
            ExecTest(Words(0x3E00),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG + 0x00",
                    "2|L--|CDCZ = cond(WREG)"
                );
            ExecTest(Words(0x3E55),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG + 0x55",
                    "2|L--|CDCZ = cond(WREG)"
                );
        }

        [Test]
        public void PIC16Basic_Rewriter_ADDWF()
        {
            ExecTest(Words(0x0700),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG + INDF",
                    "2|L--|CDCZ = cond(WREG)"
                );
            ExecTest(Words(0x0755),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[0x0055:byte]",
                    "2|L--|CDCZ = cond(WREG)"
                );
            ExecTest(Words(0x0780),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|INDF = WREG + INDF",
                    "2|L--|CDCZ = cond(INDF)"
                );
            ExecTest(Words(0x07D5),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|Data[0x0055:byte] = WREG + Data[0x0055:byte]",
                    "2|L--|CDCZ = cond(Data[0x0055:byte])"
                );
        }

        [Test]
        public void PIC16Basic_Rewriter_ANDLW()
        {
            ExecTest(Words(0x3900),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG & 0x00",
                    "2|L--|Z = cond(WREG)"
                );
            ExecTest(Words(0x39AA),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG & 0xAA",
                    "2|L--|Z = cond(WREG)"
                );
        }

        [Test]
        public void PIC16Basic_Rewriter_ANDWF()
        {
            ExecTest(Words(0x0505),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = WREG & Data[0x0005:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0585),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0005:byte] = WREG & Data[0x0005:byte]",
                "2|L--|Z = cond(Data[0x0005:byte])"
            );
        }


        [Test]
        public void PIC16Basic_Rewriter_BCF()
        {
            ExecTest(Words(0x108A),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x000A:byte] = Data[0x000A:byte] & 0xFD"
            );
            ExecTest(Words(0x13FF),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x007F:byte] = Data[0x007F:byte] & 0x7F"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_BSF()
        {
            ExecTest(Words(0x148A),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x000A:byte] = Data[0x000A:byte] | 0x02"
            );
            ExecTest(Words(0x17FF),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x007F:byte] = Data[0x007F:byte] | 0x80"
            );
        }



        [Test]
        public void PIC16Basic_Rewriter_BTFSC()
        {
            ExecTest(Words(0x180A),
            "0|T--|00000200(2): 1 instructions",
                "1|T--|if ((Data[0x000A:byte] & 0x01) == 0x00) branch 00000204"
            );
            ExecTest(Words(0x1BFF),
            "0|T--|00000200(2): 1 instructions",
                "1|T--|if ((Data[0x007F:byte] & 0x80) == 0x00) branch 00000204"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_BTFSS()
        {
            ExecTest(Words(0x1C7A),
            "0|T--|00000200(2): 1 instructions",
                "1|T--|if ((Data[0x007A:byte] & 0x01) != 0x00) branch 00000204"
            );
            ExecTest(Words(0x1FFF),
            "0|T--|00000200(2): 1 instructions",
                "1|T--|if ((Data[0x007F:byte] & 0x80) != 0x00) branch 00000204"
            );
        }



        [Test]
        public void PIC16Basic_Rewriter_CALL()
        {
            ExecTest(Words(0x2345),
            "0|T--|00000200(2): 3 instructions",
                "1|L--|STKPTR = STKPTR + 0x01",
                "2|L--|Stack[STKPTR] = 00000202",
                "3|T--|call 0000068A (0)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_CLRF()
        {
            ExecTest(Words(0x0185),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0005:byte] = 0x00",
                "2|L--|Z = true"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_CLRW()
        {
            ExecTest(Words(0x0100),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = 0x00",
                "2|L--|Z = true"
            );
            ExecTest(Words(0x0101),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = 0x00",
                "2|L--|Z = true"
            );
            ExecTest(Words(0x0102),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = 0x00",
                "2|L--|Z = true"
            );
            ExecTest(Words(0x0103),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = 0x00",
                "2|L--|Z = true"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_CLRWDT()
        {
            ExecTest(Words(0x0064),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|STATUS = STATUS | 0x18"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_COMF()
        {
            ExecTest(Words(0x0955),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = ~Data[0x0055:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x09F5),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0075:byte] = ~Data[0x0075:byte]",
                "2|L--|Z = cond(Data[0x0075:byte])"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_DECF()
        {
            ExecTest(Words(0x0355),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = Data[0x0055:byte] - 0x01",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x03F5),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0075:byte] = Data[0x0075:byte] - 0x01",
                "2|L--|Z = cond(Data[0x0075:byte])"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_DECFSZ()
        {
            ExecTest(Words(0x0B37),
            "0|T--|00000200(2): 2 instructions",
                "1|L--|WREG = Data[0x0037:byte] - 0x01",
                "2|T--|if (WREG == 0x00) branch 00000204"
            );
            ExecTest(Words(0x0BF7),
            "0|T--|00000200(2): 2 instructions",
                "1|L--|Data[0x0077:byte] = Data[0x0077:byte] - 0x01",
                "2|T--|if (Data[0x0077:byte] == 0x00) branch 00000204"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_GOTO()
        {
            ExecTest(Words(0x2B45),
            "0|T--|00000200(2): 1 instructions",
                "1|T--|goto 0000068A"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_INCF()
        {
            ExecTest(Words(0x0A55),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = Data[0x0055:byte] + 0x01",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0AF5),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0075:byte] = Data[0x0075:byte] + 0x01",
                "2|L--|Z = cond(Data[0x0075:byte])"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_INCFSZ()
        {
            ExecTest(Words(0x0F37),
            "0|T--|00000200(2): 2 instructions",
                "1|L--|WREG = Data[0x0037:byte] + 0x01",
                "2|T--|if (WREG == 0x00) branch 00000204"
            );
            ExecTest(Words(0x0FF7),
            "0|T--|00000200(2): 2 instructions",
                "1|L--|Data[0x0077:byte] = Data[0x0077:byte] + 0x01",
                "2|T--|if (Data[0x0077:byte] == 0x00) branch 00000204"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_IORLW()
        {
            ExecTest(Words(0x385A),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = WREG | 0x5A",
                "2|L--|Z = cond(WREG)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_IORWF()
        {
            ExecTest(Words(0x0417),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = WREG | Data[0x0017:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x04F5),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0075:byte] = WREG | Data[0x0075:byte]",
                "2|L--|Z = cond(Data[0x0075:byte])"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_MOVF()
        {
            ExecTest(Words(0x0876),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = Data[0x0076:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x08F6),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|Data[0x0076:byte] = Data[0x0076:byte]",
                "2|L--|Z = cond(Data[0x0076:byte])"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_MOVLW()
        {
            ExecTest(Words(0x305A),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|WREG = 0x5A"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_MOVWF()
        {
            ExecTest(Words(0x0087),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x0007:byte] = WREG"
            );
            ExecTest(Words(0x00FF),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x007F:byte] = WREG"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_NOP()
        {
            ExecTest(Words(0x0000),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|nop"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_RETFIE()
        {
            ExecTest(Words(0x0009),
            "0|T--|00000200(2): 3 instructions",
                "1|L--|INTCON = INTCON | 0x80",
                "2|L--|STKPTR = STKPTR - 0x01",
                "3|T--|return (0,0)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_RETLW()
        {
            ExecTest(Words(0x3401),
            "0|T--|00000200(2): 3 instructions",
                "1|L--|WREG = 0x01",
                "2|L--|STKPTR = STKPTR - 0x01",
                "3|T--|return (0,0)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_RETURN()
        {
            ExecTest(Words(0x0008),
            "0|T--|00000200(2): 2 instructions",
                "1|L--|STKPTR = STKPTR - 0x01",
                "2|T--|return (0,0)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_RLF()
        {
            ExecTest(Words(0x0D08),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|WREG = __rlf(Data[0x0008:byte])()"
            );
            ExecTest(Words(0x0D88),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x0008:byte] = __rlf(Data[0x0008:byte])()"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_RRF()
        {
            ExecTest(Words(0x0C08),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|WREG = __rrf(Data[0x0008:byte])()"
            );
            ExecTest(Words(0x0C88),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x0008:byte] = __rrf(Data[0x0008:byte])()"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_SLEEP()
        {
            ExecTest(Words(0x0063),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|nop"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_SUBLW()
        {
            ExecTest(Words(0x3C0D),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = 0x0D - WREG",
                "2|L--|CDCZ = cond(WREG)"
            );
        }

        [Test]
        public void PIC16Basic_Rewriter_SUBWF()
        {
            ExecTest(Words(0x0200),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = INDF - WREG",
                    "2|L--|CDCZ = cond(WREG)"
                );
            ExecTest(Words(0x0255),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = Data[0x0055:byte] - WREG",
                    "2|L--|CDCZ = cond(WREG)"
                );
            ExecTest(Words(0x0280),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|INDF = INDF - WREG",
                    "2|L--|CDCZ = cond(INDF)"
                );
            ExecTest(Words(0x02D5),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|Data[0x0055:byte] = Data[0x0055:byte] - WREG",
                    "2|L--|CDCZ = cond(Data[0x0055:byte])"
                );
        }

        [Test]
        public void PIC16Basic_Rewriter_SWAPF()
        {
            ExecTest(Words(0x0E33),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|WREG = __swapf(Data[0x0033:byte])()"
            );
            ExecTest(Words(0x0EB3),
            "0|L--|00000200(2): 1 instructions",
                "1|L--|Data[0x0033:byte] = __swapf(Data[0x0033:byte])()"
            );
        }



        [Test]
        public void PIC16Basic_Rewriter_XORLW()
        {
            ExecTest(Words(0x3AAA),
            "0|L--|00000200(2): 2 instructions",
                "1|L--|WREG = WREG ^ 0xAA",
                "2|L--|Z = cond(WREG)"
            );
        }



        [Test]
        public void PIC16Basic_Rewriter_XORWF()
        {
            ExecTest(Words(0x0600),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ INDF",
                    "2|L--|Z = cond(WREG)"
                );
            ExecTest(Words(0x0655),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|WREG = WREG ^ Data[0x0055:byte]",
                    "2|L--|Z = cond(WREG)"
                );
            ExecTest(Words(0x0680),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|INDF = WREG ^ INDF",
                    "2|L--|Z = cond(INDF)"
                );
            ExecTest(Words(0x06D5),
                "0|L--|00000200(2): 2 instructions",
                    "1|L--|Data[0x0055:byte] = WREG ^ Data[0x0055:byte]",
                    "2|L--|Z = cond(Data[0x0055:byte])"
                );
        }


    }

}
