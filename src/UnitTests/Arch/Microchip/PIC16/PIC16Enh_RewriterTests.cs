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

namespace Reko.UnitTests.Arch.Microchip.PIC16.Rewriter
{
    using Common;
    using static Common.Sample;

    /// <summary>
    /// PIC16 enhanced rewriter tests.
    /// </summary>
    [TestFixture]
    public class PIC16Enh_RewriterTests : PICRewriterTestsBase
    {
        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC16EnhancedName);
        }

        [Test]
        public void PIC16Enhd_Rewriter_SomeBasic()
        {
            // ADDLW 0x55
            ExecTest(Words(0x3E55),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + 0x55",
                    "2|L--|CDCZ = cond(WREG)"
                );

            // ADDWF 0x55,BANKED
            ExecTest(Words(0x0755),
                "0|L--|000200(2): 2 instructions",
                    "1|L--|WREG = WREG + Data[BSR:0x55:byte]",
                    "2|L--|CDCZ = cond(WREG)"
                );

            // BCF 0x7F,7,BANKED
            ExecTest(Words(0x13FF),
            "0|L--|000200(2): 1 instructions",
                "1|L--|Data[BSR:0x7F:byte] = Data[BSR:0x7F:byte] & 0x7F"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_ADDFSR()
        {
            ExecTest(Words(0x311A),
            "0|L--|000200(2): 1 instructions",
                "1|L--|FSR0 = FSR0 + 26"
            );
            ExecTest(Words(0x313A),
            "0|L--|000200(2): 1 instructions",
                "1|L--|FSR0 = FSR0 - 6"
            );
            ExecTest(Words(0x317A),
            "0|L--|000200(2): 1 instructions",
                "1|L--|FSR1 = FSR1 - 6"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_ADDWFC()
        {
            ExecTest(Words(0x3D01),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = WREG + Data[FSR1:byte] + C",
                "2|L--|CDCZ = cond(WREG)"
            );
            ExecTest(Words(0x3D81),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = WREG + Data[FSR1:byte] + C",
                "2|L--|CDCZ = cond(Data[FSR1:byte])"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_ASRF()
        {
            ExecTest(Words(0x3700),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = __asrf(Data[FSR0:byte])()",
                "2|L--|CZ = cond(WREG)"
            );
            ExecTest(Words(0x3780),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR0:byte] = __asrf(Data[FSR0:byte])()",
                "2|L--|CZ = cond(Data[FSR0:byte])"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_BRA()
        {
            ExecTest(Words(0x3200),
            "0|T--|000200(2): 1 instructions",
                "1|T--|goto 000202"
            );
            ExecTest(Words(0x32FF),
            "0|T--|000200(2): 1 instructions",
                "1|T--|goto 000400"
            );
            ExecTest(Words(0x33FF),
            "0|T--|000200(2): 1 instructions",
                "1|T--|goto 000200"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_BRW()
        {
            ExecTest(Words(0x000B),
            "0|T--|000200(2): 1 instructions",
                "1|T--|goto 0x000202 + WREG"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_CALLW()
        {
            ExecTest(Words(0x000A),
            "0|T--|000200(2): 3 instructions",
                "1|L--|STKPTR = STKPTR + 0x01",
                "2|L--|Stack[STKPTR] = 000202",
                "3|T--|call (PCLATH << 0x08) + WREG (0)"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_LSLF()
        {
            ExecTest(Words(0x3500),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = __lslf(Data[FSR0:byte])()",
                "2|L--|CZ = cond(WREG)"
            );
            ExecTest(Words(0x3581),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = __lslf(Data[FSR1:byte])()",
                "2|L--|CZ = cond(Data[FSR1:byte])"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_LSRF()
        {
            ExecTest(Words(0x3600),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = __lsrf(Data[FSR0:byte])()",
                "2|L--|CZ = cond(WREG)"
            );
            ExecTest(Words(0x3681),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = __lsrf(Data[FSR1:byte])()",
                "2|L--|CZ = cond(Data[FSR1:byte])"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_MOVIW()
        {
            ExecTest(Words(0x0010),
            "0|L--|000200(2): 3 instructions",
                "1|L--|FSR0 = FSR0 + 0x0001",
                "2|L--|WREG = Data[FSR0:byte]",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0011),
            "0|L--|000200(2): 3 instructions",
                "1|L--|FSR0 = FSR0 - 0x0001",
                "2|L--|WREG = Data[FSR0:byte]",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0012),
            "0|L--|000200(2): 3 instructions",
                "1|L--|WREG = Data[FSR0:byte]",
                "2|L--|FSR0 = FSR0 + 0x0001",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0013),
            "0|L--|000200(2): 3 instructions",
                "1|L--|WREG = Data[FSR0:byte]",
                "2|L--|FSR0 = FSR0 - 0x0001",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0014),
            "0|L--|000200(2): 3 instructions",
                "1|L--|FSR1 = FSR1 + 0x0001",
                "2|L--|WREG = Data[FSR1:byte]",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0015),
            "0|L--|000200(2): 3 instructions",
                "1|L--|FSR1 = FSR1 - 0x0001",
                "2|L--|WREG = Data[FSR1:byte]",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0016),
            "0|L--|000200(2): 3 instructions",
                "1|L--|WREG = Data[FSR1:byte]",
                "2|L--|FSR1 = FSR1 + 0x0001",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x0017),
            "0|L--|000200(2): 3 instructions",
                "1|L--|WREG = Data[FSR1:byte]",
                "2|L--|FSR1 = FSR1 - 0x0001",
                "3|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x3F0A),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = Data[FSR0 + 10:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x3F20),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = Data[FSR0 - 32:byte]",
                "2|L--|Z = cond(WREG)"
            );
            ExecTest(Words(0x3F45),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = Data[FSR1 + 5:byte]",
                "2|L--|Z = cond(WREG)"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_MOVLB()
        {
            ExecTest(Words(0x0025),
            "0|L--|000200(2): 1 instructions",
                "1|L--|BSR = 0x05"
            );
            ExecTest(Words(0x0145),
            "0|L--|000200(2): 1 instructions",
                "1|---|<invalid>"
            );
            ExecTest(Words(0x0175),
            "0|L--|000200(2): 1 instructions",
                "1|---|<invalid>"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_MOVLP()
        {
            ExecTest(Words(0x31DA),
            "0|L--|000200(2): 1 instructions",
                "1|L--|PCLATH = 0x5A"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_MOVWI()
        {
            ExecTest(Words(0x0018),
            "0|L--|000200(2): 2 instructions",
                "1|L--|FSR0 = FSR0 + 0x0001",
                "2|L--|Data[FSR0:byte] = WREG"
            );
            ExecTest(Words(0x0019),
            "0|L--|000200(2): 2 instructions",
                "1|L--|FSR0 = FSR0 - 0x0001",
                "2|L--|Data[FSR0:byte] = WREG"
            );
            ExecTest(Words(0x001A),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR0:byte] = WREG",
                "2|L--|FSR0 = FSR0 + 0x0001"
            );
            ExecTest(Words(0x001B),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR0:byte] = WREG",
                "2|L--|FSR0 = FSR0 - 0x0001"
            );
            ExecTest(Words(0x001C),
            "0|L--|000200(2): 2 instructions",
                "1|L--|FSR1 = FSR1 + 0x0001",
                "2|L--|Data[FSR1:byte] = WREG"
            );
            ExecTest(Words(0x001D),
            "0|L--|000200(2): 2 instructions",
                "1|L--|FSR1 = FSR1 - 0x0001",
                "2|L--|Data[FSR1:byte] = WREG"
            );
            ExecTest(Words(0x001E),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = WREG",
                "2|L--|FSR1 = FSR1 + 0x0001"
            );
            ExecTest(Words(0x001F),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = WREG",
                "2|L--|FSR1 = FSR1 - 0x0001"
            );
            ExecTest(Words(0x3F8A),
            "0|L--|000200(2): 1 instructions",
                "1|L--|Data[FSR0 + 10:byte] = WREG"
            );
            ExecTest(Words(0x3FA0),
            "0|L--|000200(2): 1 instructions",
                "1|L--|Data[FSR0 - 32:byte] = WREG"
            );
            ExecTest(Words(0x3FAA),
            "0|L--|000200(2): 1 instructions",
                "1|L--|Data[FSR0 - 22:byte] = WREG"
            );
            ExecTest(Words(0x3FC5),
            "0|L--|000200(2): 1 instructions",
                "1|L--|Data[FSR1 + 5:byte] = WREG"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_RESET()
        {
            ExecTest(Words(0x0001),
            "0|L--|000200(2): 1 instructions",
                "1|L--|nop"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_SUBWFB()
        {
            ExecTest(Words(0x3B01),
            "0|L--|000200(2): 2 instructions",
                "1|L--|WREG = Data[FSR1:byte] - WREG - !C",
                "2|L--|CDCZ = cond(WREG)"
            );
            ExecTest(Words(0x3B81),
            "0|L--|000200(2): 2 instructions",
                "1|L--|Data[FSR1:byte] = Data[FSR1:byte] - WREG - !C",
                "2|L--|CDCZ = cond(Data[FSR1:byte])"
            );
        }

        [Test]
        public void PIC16Enhd_Rewriter_TRIS()
        {
            ExecTest(Words(0x0065),
            "0|L--|000200(2): 1 instructions",
                "1|L--|nop"
            );
        }

    }

}
