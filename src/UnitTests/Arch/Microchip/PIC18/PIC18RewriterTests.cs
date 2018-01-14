using Microchip.Crownking;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18
{
    [TestFixture]
    public class PIC18RewriterTests : RewriterTestBase
    {
        #region Locals

        private static PIC pic = PICSamples.GetSample(InstructionSetID.PIC18);
        private PIC18Architecture arch;
        private Address baseAddr = Address.Ptr32(0x200);
        private PIC18State state;
        private MemoryArea image;

        #endregion

        private class ExpectResult
        {
            public string[] Rtls { get; }
            public string Mesg { get; set; }
            public string Instr { get; }

            public ExpectResult()
            {
                Instr = "-undefined-";
                Rtls = new string[] { "invalid" };
            }

            public ExpectResult(string instr, params string[] rtl)
            {
                Instr = instr;
                Rtls = rtl;
            }
        }

        // Instructions (bit patterns) which are supported by all flavors of PIC18 for any execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _anyPIC_anyMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0003 }, new ExpectResult("SLEEP", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0004 }, new ExpectResult("CLRWDT", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0005 }, new ExpectResult("PUSH", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0006 }, new ExpectResult("POP", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0007 }, new ExpectResult("DAW", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0008 }, new ExpectResult("TBLRD\t*", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x0009 }, new ExpectResult("TBLRD\t*+", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x000A }, new ExpectResult("TBLRD\t*-", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x000B }, new ExpectResult("TBLRD\t+*", "0|L--|00000200(2): 1 instructions") },
            { new ushort[] { 0x000C }, new ExpectResult("TBLWT\t*") },
            { new ushort[] { 0x000D }, new ExpectResult("TBLWT\t*+") },
            { new ushort[] { 0x000E }, new ExpectResult("TBLWT\t*-") },
            { new ushort[] { 0x000F }, new ExpectResult("TBLWT\t+*") },
            { new ushort[] { 0x0010 }, new ExpectResult("RETFIE") },
            { new ushort[] { 0x0011 }, new ExpectResult("RETFIE\tS") },
            { new ushort[] { 0x0012 }, new ExpectResult("RETURN") },
            { new ushort[] { 0x0013 }, new ExpectResult("RETURN\tS") },
            { new ushort[] { 0x00FF }, new ExpectResult("RESET") },

            { new ushort[] { 0x0100 }, new ExpectResult("MOVLB\t0x00") },
            { new ushort[] { 0x0107 }, new ExpectResult("MOVLB\t0x07") },
            { new ushort[] { 0x010F }, new ExpectResult("MOVLB\t0x0F") },

            { new ushort[] { 0x0300 }, new ExpectResult("MULWF\t0x00") },
            { new ushort[] { 0x0343 }, new ExpectResult("MULWF\t0x43") },
            { new ushort[] { 0x03AA }, new ExpectResult("MULWF\t0xAA") },
            { new ushort[] { 0x0501 }, new ExpectResult("DECF\t0x01,W") },
            { new ushort[] { 0x055F }, new ExpectResult("DECF\t0x5F,W") },
            { new ushort[] { 0x05BB }, new ExpectResult("DECF\t0xBB,W") },
            { new ushort[] { 0x0744 }, new ExpectResult("DECF\t0x44") },
            { new ushort[] { 0x07BB }, new ExpectResult("DECF\t0xBB") },

            { new ushort[] { 0x0800 }, new ExpectResult("SUBLW\t0x00") },
            { new ushort[] { 0x08AA }, new ExpectResult("SUBLW\t0xAA") },
            { new ushort[] { 0x0922 }, new ExpectResult("IORLW\t0x22") },
            { new ushort[] { 0x0977 }, new ExpectResult("IORLW\t0x77") },
            { new ushort[] { 0x0A00 }, new ExpectResult("XORLW\t0x00") },
            { new ushort[] { 0x0AAA }, new ExpectResult("XORLW\t0xAA") },
            { new ushort[] { 0x0B22 }, new ExpectResult("ANDLW\t0x22") },
            { new ushort[] { 0x0B77 }, new ExpectResult("ANDLW\t0x77") },
            { new ushort[] { 0x0C00 }, new ExpectResult("RETLW\t0x00") },
            { new ushort[] { 0x0CAA }, new ExpectResult("RETLW\t0xAA") },
            { new ushort[] { 0x0D22 }, new ExpectResult("MULLW\t0x22") },
            { new ushort[] { 0x0D77 }, new ExpectResult("MULLW\t0x77") },
            { new ushort[] { 0x0E00 }, new ExpectResult("MOVLW\t0x00") },
            { new ushort[] { 0x0EAA }, new ExpectResult("MOVLW\t0xAA") },
            { new ushort[] { 0x0F33 }, new ExpectResult("ADDLW\t0x33") },
            { new ushort[] { 0x0F88 }, new ExpectResult("ADDLW\t0x88") },

            { new ushort[] { 0x1123 }, new ExpectResult("IORWF\t0x23,W") },
            { new ushort[] { 0x115A }, new ExpectResult("IORWF\t0x5A,W") },
            { new ushort[] { 0x1160 }, new ExpectResult("IORWF\t0x60,W") },
            { new ushort[] { 0x1323 }, new ExpectResult("IORWF\t0x23") },
            { new ushort[] { 0x135A }, new ExpectResult("IORWF\t0x5A") },
            { new ushort[] { 0x1378 }, new ExpectResult("IORWF\t0x78") },
            { new ushort[] { 0x1523 }, new ExpectResult("ANDWF\t0x23,W") },
            { new ushort[] { 0x155A }, new ExpectResult("ANDWF\t0x5A,W") },
            { new ushort[] { 0x1569 }, new ExpectResult("ANDWF\t0x69,W") },
            { new ushort[] { 0x1723 }, new ExpectResult("ANDWF\t0x23") },
            { new ushort[] { 0x175A }, new ExpectResult("ANDWF\t0x5A") },
            { new ushort[] { 0x17A5 }, new ExpectResult("ANDWF\t0xA5") },
            { new ushort[] { 0x1923 }, new ExpectResult("XORWF\t0x23,W") },
            { new ushort[] { 0x195A }, new ExpectResult("XORWF\t0x5A,W") },
            { new ushort[] { 0x1974 }, new ExpectResult("XORWF\t0x74,W") },
            { new ushort[] { 0x1B23 }, new ExpectResult("XORWF\t0x23") },
            { new ushort[] { 0x1B5A }, new ExpectResult("XORWF\t0x5A") },
            { new ushort[] { 0x1B91 }, new ExpectResult("XORWF\t0x91") },
            { new ushort[] { 0x1D23 }, new ExpectResult("COMF\t0x23,W") },
            { new ushort[] { 0x1D5A }, new ExpectResult("COMF\t0x5A,W") },
            { new ushort[] { 0x1D66 }, new ExpectResult("COMF\t0x66,W") },
            { new ushort[] { 0x1F23 }, new ExpectResult("COMF\t0x23") },
            { new ushort[] { 0x1F5A }, new ExpectResult("COMF\t0x5A") },
            { new ushort[] { 0x1FFE }, new ExpectResult("COMF\t0xFE") },
            { new ushort[] { 0x2123 }, new ExpectResult("ADDWFC\t0x23,W") },
            { new ushort[] { 0x215A }, new ExpectResult("ADDWFC\t0x5A,W") },
            { new ushort[] { 0x2177 }, new ExpectResult("ADDWFC\t0x77,W") },
            { new ushort[] { 0x2323 }, new ExpectResult("ADDWFC\t0x23") },
            { new ushort[] { 0x235A }, new ExpectResult("ADDWFC\t0x5A") },
            { new ushort[] { 0x23BC }, new ExpectResult("ADDWFC\t0xBC") },
            { new ushort[] { 0x2523 }, new ExpectResult("ADDWF\t0x23,W") },
            { new ushort[] { 0x255A }, new ExpectResult("ADDWF\t0x5A,W") },
            { new ushort[] { 0x25CE }, new ExpectResult("ADDWF\t0xCE,W") },
            { new ushort[] { 0x2723 }, new ExpectResult("ADDWF\t0x23") },
            { new ushort[] { 0x275A }, new ExpectResult("ADDWF\t0x5A") },
            { new ushort[] { 0x27D8 }, new ExpectResult("ADDWF\t0xD8") },
            { new ushort[] { 0x2923 }, new ExpectResult("INCF\t0x23,W") },
            { new ushort[] { 0x295A }, new ExpectResult("INCF\t0x5A,W") },
            { new ushort[] { 0x29F1 }, new ExpectResult("INCF\t0xF1,W") },
            { new ushort[] { 0x2B23 }, new ExpectResult("INCF\t0x23") },
            { new ushort[] { 0x2B5A }, new ExpectResult("INCF\t0x5A") },
            { new ushort[] { 0x2B78 }, new ExpectResult("INCF\t0x78") },
            { new ushort[] { 0x2D23 }, new ExpectResult("DECFSZ\t0x23,W") },
            { new ushort[] { 0x2D5B }, new ExpectResult("DECFSZ\t0x5B,W") },
            { new ushort[] { 0x2DB5 }, new ExpectResult("DECFSZ\t0xB5,W") },
            { new ushort[] { 0x2F23 }, new ExpectResult("DECFSZ\t0x23") },
            { new ushort[] { 0x2F5A }, new ExpectResult("DECFSZ\t0x5A") },
            { new ushort[] { 0x2FC2 }, new ExpectResult("DECFSZ\t0xC2") },
            { new ushort[] { 0x3123 }, new ExpectResult("RRCF\t0x23,W") },
            { new ushort[] { 0x315A }, new ExpectResult("RRCF\t0x5A,W") },
            { new ushort[] { 0x3177 }, new ExpectResult("RRCF\t0x77,W") },
            { new ushort[] { 0x3323 }, new ExpectResult("RRCF\t0x23") },
            { new ushort[] { 0x335A }, new ExpectResult("RRCF\t0x5A") },
            { new ushort[] { 0x33DD }, new ExpectResult("RRCF\t0xDD") },
            { new ushort[] { 0x3523 }, new ExpectResult("RLCF\t0x23,W") },
            { new ushort[] { 0x355A }, new ExpectResult("RLCF\t0x5A,W") },
            { new ushort[] { 0x3574 }, new ExpectResult("RLCF\t0x74,W") },
            { new ushort[] { 0x3723 }, new ExpectResult("RLCF\t0x23") },
            { new ushort[] { 0x375A }, new ExpectResult("RLCF\t0x5A") },
            { new ushort[] { 0x37B8 }, new ExpectResult("RLCF\t0xB8") },
            { new ushort[] { 0x3923 }, new ExpectResult("SWAPF\t0x23,W") },
            { new ushort[] { 0x395A }, new ExpectResult("SWAPF\t0x5A,W") },
            { new ushort[] { 0x39A3 }, new ExpectResult("SWAPF\t0xA3,W") },
            { new ushort[] { 0x3B23 }, new ExpectResult("SWAPF\t0x23") },
            { new ushort[] { 0x3B5A }, new ExpectResult("SWAPF\t0x5A") },
            { new ushort[] { 0x3BC5 }, new ExpectResult("SWAPF\t0xC5") },
            { new ushort[] { 0x3D23 }, new ExpectResult("INCFSZ\t0x23,W") },
            { new ushort[] { 0x3D5A }, new ExpectResult("INCFSZ\t0x5A,W") },
            { new ushort[] { 0x3DD1 }, new ExpectResult("INCFSZ\t0xD1,W") },
            { new ushort[] { 0x3F23 }, new ExpectResult("INCFSZ\t0x23") },
            { new ushort[] { 0x3F5A }, new ExpectResult("INCFSZ\t0x5A") },
            { new ushort[] { 0x3FB6 }, new ExpectResult("INCFSZ\t0xB6") },
            { new ushort[] { 0x4123 }, new ExpectResult("RRNCF\t0x23,W") },
            { new ushort[] { 0x414D }, new ExpectResult("RRNCF\t0x4D,W") },
            { new ushort[] { 0x415A }, new ExpectResult("RRNCF\t0x5A,W") },
            { new ushort[] { 0x4323 }, new ExpectResult("RRNCF\t0x23") },
            { new ushort[] { 0x435A }, new ExpectResult("RRNCF\t0x5A") },
            { new ushort[] { 0x43B8 }, new ExpectResult("RRNCF\t0xB8") },
            { new ushort[] { 0x4523 }, new ExpectResult("RLNCF\t0x23,W") },
            { new ushort[] { 0x453F }, new ExpectResult("RLNCF\t0x3F,W") },
            { new ushort[] { 0x45F2 }, new ExpectResult("RLNCF\t0xF2,W") },
            { new ushort[] { 0x4723 }, new ExpectResult("RLNCF\t0x23") },
            { new ushort[] { 0x475A }, new ExpectResult("RLNCF\t0x5A") },
            { new ushort[] { 0x47D5 }, new ExpectResult("RLNCF\t0xD5") },
            { new ushort[] { 0x4923 }, new ExpectResult("INFSNZ\t0x23,W") },
            { new ushort[] { 0x495A }, new ExpectResult("INFSNZ\t0x5A,W") },
            { new ushort[] { 0x49E4 }, new ExpectResult("INFSNZ\t0xE4,W") },
            { new ushort[] { 0x4B23 }, new ExpectResult("INFSNZ\t0x23") },
            { new ushort[] { 0x4B5A }, new ExpectResult("INFSNZ\t0x5A") },
            { new ushort[] { 0x4B82 }, new ExpectResult("INFSNZ\t0x82") },
            { new ushort[] { 0x4D23 }, new ExpectResult("DCFSNZ\t0x23,W") },
            { new ushort[] { 0x4D5A }, new ExpectResult("DCFSNZ\t0x5A,W") },
            { new ushort[] { 0x4D97 }, new ExpectResult("DCFSNZ\t0x97,W") },
            { new ushort[] { 0x4F23 }, new ExpectResult("DCFSNZ\t0x23") },
            { new ushort[] { 0x4F5A }, new ExpectResult("DCFSNZ\t0x5A") },
            { new ushort[] { 0x4FDC }, new ExpectResult("DCFSNZ\t0xDC") },
            { new ushort[] { 0x5123 }, new ExpectResult("MOVF\t0x23,W") },
            { new ushort[] { 0x515A }, new ExpectResult("MOVF\t0x5A,W") },
            { new ushort[] { 0x5178 }, new ExpectResult("MOVF\t0x78,W") },
            { new ushort[] { 0x5323 }, new ExpectResult("MOVF\t0x23") },
            { new ushort[] { 0x535A }, new ExpectResult("MOVF\t0x5A") },
            { new ushort[] { 0x53B3 }, new ExpectResult("MOVF\t0xB3") },
            { new ushort[] { 0x5523 }, new ExpectResult("SUBFWB\t0x23,W") },
            { new ushort[] { 0x555A }, new ExpectResult("SUBFWB\t0x5A,W") },
            { new ushort[] { 0x557E }, new ExpectResult("SUBFWB\t0x7E,W") },
            { new ushort[] { 0x5723 }, new ExpectResult("SUBFWB\t0x23") },
            { new ushort[] { 0x575A }, new ExpectResult("SUBFWB\t0x5A") },
            { new ushort[] { 0x57F5 }, new ExpectResult("SUBFWB\t0xF5") },
            { new ushort[] { 0x5923 }, new ExpectResult("SUBWFB\t0x23,W") },
            { new ushort[] { 0x595A }, new ExpectResult("SUBWFB\t0x5A,W") },
            { new ushort[] { 0x597A }, new ExpectResult("SUBWFB\t0x7A,W") },
            { new ushort[] { 0x5B23 }, new ExpectResult("SUBWFB\t0x23") },
            { new ushort[] { 0x5B5A }, new ExpectResult("SUBWFB\t0x5A") },
            { new ushort[] { 0x5B61 }, new ExpectResult("SUBWFB\t0x61") },
            { new ushort[] { 0x5D23 }, new ExpectResult("SUBWF\t0x23,W") },
            { new ushort[] { 0x5D5A }, new ExpectResult("SUBWF\t0x5A,W") },
            { new ushort[] { 0x5D63 }, new ExpectResult("SUBWF\t0x63,W") },
            { new ushort[] { 0x5F23 }, new ExpectResult("SUBWF\t0x23") },
            { new ushort[] { 0x5F5A }, new ExpectResult("SUBWF\t0x5A") },
            { new ushort[] { 0x5F7B }, new ExpectResult("SUBWF\t0x7B") },
            { new ushort[] { 0x6100 }, new ExpectResult("CPFSLT\t0x00") },
            { new ushort[] { 0x614E }, new ExpectResult("CPFSLT\t0x4E") },
            { new ushort[] { 0x61CE }, new ExpectResult("CPFSLT\t0xCE") },
            { new ushort[] { 0x6300 }, new ExpectResult("CPFSEQ\t0x00") },
            { new ushort[] { 0x6344 }, new ExpectResult("CPFSEQ\t0x44") },
            { new ushort[] { 0x63E3 }, new ExpectResult("CPFSEQ\t0xE3") },
            { new ushort[] { 0x6500 }, new ExpectResult("CPFSGT\t0x00") },
            { new ushort[] { 0x6534 }, new ExpectResult("CPFSGT\t0x34") },
            { new ushort[] { 0x65D8 }, new ExpectResult("CPFSGT\t0xD8") },
            { new ushort[] { 0x6700 }, new ExpectResult("TSTFSZ\t0x00") },
            { new ushort[] { 0x673D }, new ExpectResult("TSTFSZ\t0x3D") },
            { new ushort[] { 0x67CE }, new ExpectResult("TSTFSZ\t0xCE") },
            { new ushort[] { 0x6901 }, new ExpectResult("SETF\t0x01") },
            { new ushort[] { 0x6956 }, new ExpectResult("SETF\t0x56") },
            { new ushort[] { 0x6964 }, new ExpectResult("SETF\t0x64") },
            { new ushort[] { 0x6B00 }, new ExpectResult("CLRF\t0x00") },
            { new ushort[] { 0x6B2D }, new ExpectResult("CLRF\t0x2D") },
            { new ushort[] { 0x6BEE }, new ExpectResult("CLRF\t0xEE") },
            { new ushort[] { 0x6D20 }, new ExpectResult("NEGF\t0x20") },
            { new ushort[] { 0x6D5E }, new ExpectResult("NEGF\t0x5E") },
            { new ushort[] { 0x6D6F }, new ExpectResult("NEGF\t0x6F") },
            { new ushort[] { 0x6F00 }, new ExpectResult("MOVWF\t0x00") },
            { new ushort[] { 0x6F20 }, new ExpectResult("MOVWF\t0x20") },
            { new ushort[] { 0x6FD8 }, new ExpectResult("MOVWF\t0xD8") },
            { new ushort[] { 0x7103 }, new ExpectResult("BTG\t0x03,0") },
            { new ushort[] { 0x714C }, new ExpectResult("BTG\t0x4C,0") },
            { new ushort[] { 0x71BC }, new ExpectResult("BTG\t0xBC,0") },
            { new ushort[] { 0x7F04 }, new ExpectResult("BTG\t0x04,7") },
            { new ushort[] { 0x7F37 }, new ExpectResult("BTG\t0x37,7") },
            { new ushort[] { 0x7FD9 }, new ExpectResult("BTG\t0xD9,7") },
            { new ushort[] { 0x8100 }, new ExpectResult("BSF\t0x00,0") },
            { new ushort[] { 0x8145 }, new ExpectResult("BSF\t0x45,0") },
            { new ushort[] { 0x81BC }, new ExpectResult("BSF\t0xBC,0") },
            { new ushort[] { 0x8F00 }, new ExpectResult("BSF\t0x00,7") },
            { new ushort[] { 0x8F5F }, new ExpectResult("BSF\t0x5F,7") },
            { new ushort[] { 0x8FFF }, new ExpectResult("BSF\t0xFF,7") },
            { new ushort[] { 0x9100 }, new ExpectResult("BCF\t0x00,0") },
            { new ushort[] { 0x9156 }, new ExpectResult("BCF\t0x56,0") },
            { new ushort[] { 0x91FE }, new ExpectResult("BCF\t0xFE,0") },
            { new ushort[] { 0x9F00 }, new ExpectResult("BCF\t0x00,7") },
            { new ushort[] { 0x9F52 }, new ExpectResult("BCF\t0x52,7") },
            { new ushort[] { 0x9FCB }, new ExpectResult("BCF\t0xCB,7") },
            { new ushort[] { 0xA100 }, new ExpectResult("BTFSS\t0x00,0") },
            { new ushort[] { 0xA144 }, new ExpectResult("BTFSS\t0x44,0") },
            { new ushort[] { 0xA1BB }, new ExpectResult("BTFSS\t0xBB,0") },
            { new ushort[] { 0xAF00 }, new ExpectResult("BTFSS\t0x00,7") },
            { new ushort[] { 0xAF55 }, new ExpectResult("BTFSS\t0x55,7") },
            { new ushort[] { 0xAF77 }, new ExpectResult("BTFSS\t0x77,7") },
            { new ushort[] { 0xB100 }, new ExpectResult("BTFSC\t0x00,0") },
            { new ushort[] { 0xB123 }, new ExpectResult("BTFSC\t0x23,0") },
            { new ushort[] { 0xB162 }, new ExpectResult("BTFSC\t0x62,0") },
            { new ushort[] { 0xBF0E }, new ExpectResult("BTFSC\t0x0E,7") },
            { new ushort[] { 0xBF46 }, new ExpectResult("BTFSC\t0x46,7") },
            { new ushort[] { 0xBFCE }, new ExpectResult("BTFSC\t0xCE,7") },

            { new ushort[] { 0xC000, 0xF123 }, new ExpectResult("MOVFF\t0x0000,0x0123") },
            { new ushort[] { 0xC879, 0xFABC }, new ExpectResult("MOVFF\t0x0879,0x0ABC") },

            { new ushort[] { 0xD000 } , new ExpectResult("BRA\t0x000202") },
            { new ushort[] { 0xD055 } , new ExpectResult("BRA\t0x0002AC") },
            { new ushort[] { 0xD755 } , new ExpectResult("BRA\t0x0000AC") },
            { new ushort[] { 0xD7FF } , new ExpectResult("BRA\t0x000200") },
            { new ushort[] { 0xD800 } , new ExpectResult("RCALL\t0x000202") },
            { new ushort[] { 0xD8AA } , new ExpectResult("RCALL\t0x000356") },
            { new ushort[] { 0xDBFF } , new ExpectResult("RCALL\t0x000A00") },
            { new ushort[] { 0xDC00 } , new ExpectResult("RCALL\t0x1FFA02") },
            { new ushort[] { 0xDFAA } , new ExpectResult("RCALL\t0x000156") },
            { new ushort[] { 0xDFFF } , new ExpectResult("RCALL\t0x000200") },

            { new ushort[] { 0xE000 } , new ExpectResult("BZ\t0x000202") },
            { new ushort[] { 0xE023 } , new ExpectResult("BZ\t0x000248") },
            { new ushort[] { 0xE0FF } , new ExpectResult("BZ\t0x000200") },
            { new ushort[] { 0xE0FE } , new ExpectResult("BZ\t0x0001FE") },
            { new ushort[] { 0xE086 } , new ExpectResult("BZ\t0x00010E") },
            { new ushort[] { 0xE100 } , new ExpectResult("BNZ\t0x000202") },
            { new ushort[] { 0xE123 } , new ExpectResult("BNZ\t0x000248") },
            { new ushort[] { 0xE1FF } , new ExpectResult("BNZ\t0x000200") },
            { new ushort[] { 0xE1FE } , new ExpectResult("BNZ\t0x0001FE") },
            { new ushort[] { 0xE186 } , new ExpectResult("BNZ\t0x00010E") },
            { new ushort[] { 0xE200 } , new ExpectResult("BC\t0x000202") },
            { new ushort[] { 0xE223 } , new ExpectResult("BC\t0x000248") },
            { new ushort[] { 0xE2FF } , new ExpectResult("BC\t0x000200") },
            { new ushort[] { 0xE2FE } , new ExpectResult("BC\t0x0001FE") },
            { new ushort[] { 0xE286 } , new ExpectResult("BC\t0x00010E") },
            { new ushort[] { 0xE300 } , new ExpectResult("BNC\t0x000202") },
            { new ushort[] { 0xE323 } , new ExpectResult("BNC\t0x000248") },
            { new ushort[] { 0xE3FF } , new ExpectResult("BNC\t0x000200") },
            { new ushort[] { 0xE3FE } , new ExpectResult("BNC\t0x0001FE") },
            { new ushort[] { 0xE386 } , new ExpectResult("BNC\t0x00010E") },
            { new ushort[] { 0xE400 } , new ExpectResult("BOV\t0x000202") },
            { new ushort[] { 0xE423 } , new ExpectResult("BOV\t0x000248") },
            { new ushort[] { 0xE4FF } , new ExpectResult("BOV\t0x000200") },
            { new ushort[] { 0xE4FE } , new ExpectResult("BOV\t0x0001FE") },
            { new ushort[] { 0xE486 } , new ExpectResult("BOV\t0x00010E") },
            { new ushort[] { 0xE500 } , new ExpectResult("BNOV\t0x000202") },
            { new ushort[] { 0xE523 } , new ExpectResult("BNOV\t0x000248") },
            { new ushort[] { 0xE5FF } , new ExpectResult("BNOV\t0x000200") },
            { new ushort[] { 0xE5FE } , new ExpectResult("BNOV\t0x0001FE") },
            { new ushort[] { 0xE586 } , new ExpectResult("BNOV\t0x00010E") },
            { new ushort[] { 0xE600 } , new ExpectResult("BN\t0x000202") },
            { new ushort[] { 0xE623 } , new ExpectResult("BN\t0x000248") },
            { new ushort[] { 0xE6FF } , new ExpectResult("BN\t0x000200") },
            { new ushort[] { 0xE6FE } , new ExpectResult("BN\t0x0001FE") },
            { new ushort[] { 0xE686 } , new ExpectResult("BN\t0x00010E") },
            { new ushort[] { 0xE700 } , new ExpectResult("BNN\t0x000202") },
            { new ushort[] { 0xE723 } , new ExpectResult("BNN\t0x000248") },
            { new ushort[] { 0xE7FF } , new ExpectResult("BNN\t0x000200") },
            { new ushort[] { 0xE7FE } , new ExpectResult("BNN\t0x0001FE") },
            { new ushort[] { 0xE786 } , new ExpectResult("BNN\t0x00010E") },

            { new ushort[] { 0xEC06, 0xF000 } , new ExpectResult("CALL\t0x00000C") },
            { new ushort[] { 0xEC12, 0xF345 } , new ExpectResult("CALL\t0x068A24") },
            { new ushort[] { 0xED06, 0xF000 } , new ExpectResult("CALL\t0x00000C,S") },
            { new ushort[] { 0xED12, 0xF345 } , new ExpectResult("CALL\t0x068A24,S") },

            { new ushort[] { 0xEF03, 0xF000 } , new ExpectResult("GOTO\t0x000006") },
            { new ushort[] { 0xEF56, 0xF789 } , new ExpectResult("GOTO\t0x0F12AC") },

            { new ushort[] { 0xF000 } , new ExpectResult("NOP") },
            { new ushort[] { 0xF123 } , new ExpectResult("NOP") },
            { new ushort[] { 0xFEDC, 0xF256 } , new ExpectResult("NOP") },
        };


        public override IProcessorArchitecture Architecture => arch;

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new PIC18Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress => baseAddr;

        [SetUp]
        public void Setup()
        {
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            state = (PIC18State)arch.CreateProcessorState();
        }

        protected override MemoryArea RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
            }).ToArray();
            image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        [Test]
        public void PIC18_Invalid()
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
                "4|L--|00000204(2): 2 instructions", "5|L--|0034 = WREG | 0x0034",""
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
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0x006F, 0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
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
            AssertCode("0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
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
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEB00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEB00, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

            Rewrite(0xEB80);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEB80, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

            Rewrite(0xEC00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEC00, 0x1234);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

            Rewrite(0xED00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xED00, 0x9876);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

            Rewrite(0xEE00);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>"
                );

            Rewrite(0xEE00, 0x6543);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );


            Rewrite(0xEE00, 0xF400);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

            Rewrite(0xEE30, 0xF000);
            AssertCode(
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
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
                "0|L--|00000200(2): 1 instructions", "1|---|<invalid>", ""
                );

        }

        [Test]
        public void PIC18_NOP()
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
        public void PIC18_CALL()
        {
            Rewrite(0xEC06, 0xF000);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0000000C (1)"
                );

            Rewrite(0xEC12, 0xF345);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 00068A24 (1)"
                );

            Rewrite(0xED06, 0xF000);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 0000000C (1)"
                );

            Rewrite(0xED12, 0xF345);
            AssertCode(
                "0|T--|00000200(4): 1 instructions", "1|T--|call 00068A24 (1)"
                );


        }
    }

}
