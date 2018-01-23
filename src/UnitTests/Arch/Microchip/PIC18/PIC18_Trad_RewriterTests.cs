using Microchip.Crownking;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Rewriter
{
    [TestFixture]
    public class PIC18_Trad_RewriterTests : PIC18RewriterTestsBase
    {
        [SetUp]
        public void Setup()
        {
            SetPICMode(InstructionSetID.PIC18, PICExecMode.Traditional);
        }

        [Test]
        public void Rewriter_Trad_Trad_ADDLW()
        {
            Rewrite(0x0F00);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + 0x00", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0F55);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + 0x55", "2|L--|CDCZOVN = cond(WREG)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_ADDWF()
        {
            Rewrite(0x2400);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[0x00:byte]", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2401);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[0x01:byte]", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x24C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + ADRESL", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2500);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[(BSR << 0x08) + 0x00:byte]", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2501);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2601);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = WREG + Mem0[0x01:byte]", "2|L--|CDCZOVN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x2701);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x01:byte] = WREG + Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0x01:byte])"
                );
            Rewrite(0x27C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC3:byte] = WREG + Mem0[(BSR << 0x08) + 0xC3:byte]", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0xC3:byte])"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_ADDWFC()
        {
            Rewrite(0x2000);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[0x00:byte] + C", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2001);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[0x01:byte] + C", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x20C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + ADRESL + C", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2100);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[(BSR << 0x08) + 0x00:byte] + C", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2101);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG + Mem0[(BSR << 0x08) + 0x01:byte] + C", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2201);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = WREG + Mem0[0x01:byte] + C", "2|L--|CDCZOVN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x2301);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x01:byte] = WREG + Mem0[(BSR << 0x08) + 0x01:byte] + C", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0x01:byte])"
                );
            Rewrite(0x23C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC3:byte] = WREG + Mem0[(BSR << 0x08) + 0xC3:byte] + C", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0xC3:byte])"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_ANDLW()
        {
            Rewrite(0x0B00);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & 0x00", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x0B55);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & 0x55", "2|L--|ZN = cond(WREG)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_ANDWF()
        {
            Rewrite(0x1400);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & Mem0[0x00:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1401);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & Mem0[0x01:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x14C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & ADRESL", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1500);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & Mem0[(BSR << 0x08) + 0x00:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1501);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG & Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1601);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = WREG & Mem0[0x01:byte]", "2|L--|ZN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x1701);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x01:byte] = WREG & Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|ZN = cond(Mem0[(BSR << 0x08) + 0x01:byte])"
                );
            Rewrite(0x17C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC3:byte] = WREG & Mem0[(BSR << 0x08) + 0xC3:byte]", "2|L--|ZN = cond(Mem0[(BSR << 0x08) + 0xC3:byte])"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_CALL()
        {
            Rewrite(0xEC06, 0xF000);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0x0000000C (1)"
                );

            Rewrite(0xEC12, 0xF345);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0x00068A24 (1)"
                );

            Rewrite(0xED06, 0xF000);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0x0000000C (1)"
                );

            Rewrite(0xED12, 0xF345);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0x00068A24 (1)"
                );


        }

        [Test]
        public void Rewriter_Trad_Trad_CLRWDT()
        {
            Rewrite(0x0004);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|RCON = DPB(RCON, true, 2)", "2|L--|RCON = DPB(RCON, true, 3)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_DAW()
        {
            Rewrite(0x0007);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = __daw(WREG, C, DC)()"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_DECF()
        {
            Rewrite(0x0400);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[0x00:byte] - 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0401);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[0x01:byte] - 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x04C4);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = ADRESH - 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0500);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[(BSR << 0x08) + 0x00:byte] - 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0501);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[(BSR << 0x08) + 0x01:byte] - 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0601);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = Mem0[0x01:byte] - 0x01", "2|L--|CDCZOVN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x0744);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x44:byte] = Mem0[(BSR << 0x08) + 0x44:byte] - 0x01", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0x44:byte])"
                );
            Rewrite(0x07C4);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC4:byte] = Mem0[(BSR << 0x08) + 0xC4:byte] - 0x01", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0xC4:byte])"
                );

        }

        [Test]
        public void Rewriter_Trad_Trad_INCF()
        {
            Rewrite(0x2800);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[0x00:byte] + 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2801);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[0x01:byte] + 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x28C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = ADRESL + 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2900);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[(BSR << 0x08) + 0x00:byte] + 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2901);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = Mem0[(BSR << 0x08) + 0x01:byte] + 0x01", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x2A01);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = Mem0[0x01:byte] + 0x01", "2|L--|CDCZOVN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x2B33);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x33:byte] = Mem0[(BSR << 0x08) + 0x33:byte] + 0x01", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0x33:byte])"
                );
            Rewrite(0x2BC3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC3:byte] = Mem0[(BSR << 0x08) + 0xC3:byte] + 0x01", "2|L--|CDCZOVN = cond(Mem0[(BSR << 0x08) + 0xC3:byte])"
                );

        }

        [Test]
        public void Rewriter_Trad_Trad_IORLW()
        {
            Rewrite(0x0900);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG | 0x00", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x0955);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG | 0x55", "2|L--|ZN = cond(WREG)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_MOVLB()
        {
            Rewrite(0x0100);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|BSR = 0x00"
                );
            Rewrite(0x0105);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|BSR = 0x05"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_MOVLW()
        {
            Rewrite(0x0E00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|WREG = 0x00"
                );
            Rewrite(0x0E55);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|WREG = 0x55"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_MULLW()
        {
            Rewrite(0x0D00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = WREG *u 0x00"
                );
            Rewrite(0x0D55);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = WREG *u 0x55"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_MULWF()
        {
            Rewrite(0x0344);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = Mem0[(BSR << 0x08) + 0x44:byte] *u WREG"
                );
            Rewrite(0x0389);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = Mem0[(BSR << 0x08) + 0x89:byte] *u WREG"
                );
            Rewrite(0x0200);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = Mem0[0x00:byte] *u WREG"
                );
            Rewrite(0x025F);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = Mem0[0x5F:byte] *u WREG"
                );
            Rewrite(0x02A8);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|PRODH_PRODL = EEDATA *u WREG"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_NOP()
        {

            Rewrite(0x0000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|nop"
                );

            Rewrite(0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|nop"
                );

            Rewrite(0xF123);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|nop"
                );

            Rewrite(0xFEDC, 0xF256);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|nop",
                "2|L--|00000202(2): 1 instructions", "3|L--|nop"
                );

        }

        [Test]
        public void Rewriter_Trad_Trad_POP()
        {
            Rewrite(0x0006);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|STKPTR = STKPTR - 0x01"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_PUSH()
        {
            Rewrite(0x0005);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|STKPTR = STKPTR + 0x01"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_RESET()
        {
            Rewrite(0x00FF);
            AssertCode(
                "0|H--|00000200(2): 2 instructions", "1|L--|STKPTR = 0x00", "2|L--|__reset()"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_RETFIE()
        {
            Rewrite(0x0010);
            AssertCode(
                "0|T--|00000200(2): 1 instructions", "1|T--|return (1,0)"
                );
            Rewrite(0x0011);
            AssertCode(
                "0|T--|00000200(2): 1 instructions", "1|T--|return (1,0)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_RETLW()
        {
            Rewrite(0x0C00);
            AssertCode(
                "0|T--|00000200(2): 2 instructions", "1|L--|WREG = 0x00", "2|T--|return (1,0)"
                );
            Rewrite(0x0C55);
            AssertCode(
                "0|T--|00000200(2): 2 instructions", "1|L--|WREG = 0x55", "2|T--|return (1,0)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_RETURN()
        {
            Rewrite(0x0012);
            AssertCode(
                "0|T--|00000200(2): 1 instructions", "1|T--|return (1,0)"
                );
            Rewrite(0x0013);
            AssertCode(
                "0|T--|00000200(2): 1 instructions", "1|T--|return (1,0)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_SLEEP()
        {
            Rewrite(0x0003);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|RCON = DPB(RCON, false, 2)", "2|L--|RCON = DPB(RCON, true, 3)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_SUBLW()
        {
            Rewrite(0x0800);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = 0x00 - WREG", "2|L--|CDCZOVN = cond(WREG)"
                );
            Rewrite(0x0855);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = 0x55 - WREG", "2|L--|CDCZOVN = cond(WREG)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_TBLRD()
        {
            Rewrite(0x0008);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblrd(0x00)"
                );
            Rewrite(0x0009);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblrd(0x01)"
                );
            Rewrite(0x000A);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblrd(0x02)"
                );
            Rewrite(0x000B);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblrd(0x03)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_TBLWT()
        {
            Rewrite(0x000C);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblwt(0x00)"
                );
            Rewrite(0x000D);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblwt(0x01)"
                );
            Rewrite(0x000E);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblwt(0x02)"
                );
            Rewrite(0x000F);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|L--|__tblwt(0x03)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_XORLW()
        {
            Rewrite(0x0A00);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ 0x00", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x0A55);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ 0x55", "2|L--|ZN = cond(WREG)"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_XORWF()
        {
            Rewrite(0x1800);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ Mem0[0x00:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1801);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ Mem0[0x01:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x18C3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ ADRESL", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1900);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ Mem0[(BSR << 0x08) + 0x00:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1901);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|WREG = WREG ^ Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|ZN = cond(WREG)"
                );
            Rewrite(0x1A01);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[0x01:byte] = WREG ^ Mem0[0x01:byte]", "2|L--|ZN = cond(Mem0[0x01:byte])"
                );
            Rewrite(0x1B01);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0x01:byte] = WREG ^ Mem0[(BSR << 0x08) + 0x01:byte]", "2|L--|ZN = cond(Mem0[(BSR << 0x08) + 0x01:byte])"
                );
            Rewrite(0x1BC3);
            AssertCode(
                "0|L--|00000200(2): 2 instructions", "1|L--|Mem0[(BSR << 0x08) + 0xC3:byte] = WREG ^ Mem0[(BSR << 0x08) + 0xC3:byte]", "2|L--|ZN = cond(Mem0[(BSR << 0x08) + 0xC3:byte])"
                );
        }

        [Test]
        public void Rewriter_Trad_Trad_Invalid()
        {
            Rewrite(0x0001);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0002);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0002, 0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 1 instructions", "3|L--|nop"
                       );

            Rewrite(0x0002, 0xF000, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 1 instructions", "3|L--|nop",
                "4|L--|00000204(2): 2 instructions", "5|L--|Mem0[0x34:byte] = WREG | Mem0[0x34:byte]", "6|L--|ZN = cond(Mem0[0x34:byte])"
                );

            Rewrite(0x0015);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0016);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0017);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0018);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0019);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x001A);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x001B);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x001C);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x001D);
            AssertCode("0|L--|00000200(2): 1 instructions", "1|---|<invalid>");

            Rewrite(0x001E);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x001F);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0020);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0040);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0060);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0067, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 2 instructions", "3|L--|Mem0[0x34:byte] = WREG | Mem0[0x34:byte]", "4|L--|ZN = cond(Mem0[0x34:byte])"
                );

            Rewrite(0x006F, 0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 1 instructions", "3|L--|nop"
                );

            Rewrite(0x0080);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x00F0);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0140);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x0180);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x01E0);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xC000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xC000, 0x0123);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 1 instructions", "3|---|<invalid>"
                );

            Rewrite(0xEB00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEB00, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 2 instructions", "3|L--|Mem0[0x34:byte] = WREG | Mem0[0x34:byte]", "4|L--|ZN = cond(Mem0[0x34:byte])"
                );

            Rewrite(0xEB80);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEB80, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 2 instructions", "3|L--|Mem0[0x34:byte] = WREG | Mem0[0x34:byte]", "4|L--|ZN = cond(Mem0[0x34:byte])"
                );

            Rewrite(0xEC00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEC00, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 2 instructions", "3|L--|Mem0[0x34:byte] = WREG | Mem0[0x34:byte]", "4|L--|ZN = cond(Mem0[0x34:byte])"
                );

            Rewrite(0xED00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xED00, 0x989D);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                "2|L--|00000202(2): 1 instructions", "3|L--|BCF"
                );

            Rewrite(0xEE00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEE00, 0x6543);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                ""
                );


            Rewrite(0xEE00, 0xF400);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                ""
                );

            Rewrite(0xEE30, 0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                ""
                );

            Rewrite(0xEE40);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEEF0);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEF00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEF00, 0xEDCB);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>",
                ""
                );

        }

    }

}
