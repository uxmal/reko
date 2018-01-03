using Microchip.Crownking;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Microchip.PIC18
{
    // As of today there are 3 flavors of PIC18 :
    //  - legacy (a.k.a. "pic18"),
    //  - extended (a.k.a. "egg")
    //  - enhanced (a.k.a. "cpu_pic18f_v6")
    // And there are 2 execution modes (traditional and extended)
    // This means we have 6 different contexts to check.
    // Note: Legacy PIC18 supports traditional execution mode only.
    // 
    [TestFixture]
    public class PIC18DisassemblerTests
    {
        private PIC pic;
        private PIC18Architecture arch;

        private class ExpectResult
        {
            public string Instr { get; }
            public string Mesg { get; }

            public ExpectResult()
            {
                Instr = "invalid";
                Mesg = "";
            }

            public ExpectResult(string instr)
            {
                Instr = instr;
                Mesg = "";
            }

            public ExpectResult(string instr, string msg)
            {
                Instr = instr;
                Mesg = msg;
            }
        }

        // Instructions (bit patterns) which are supported by all flavors of PIC18 for any execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _anyPIC_anyMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0000 }, new ExpectResult("NOP") },
            { new ushort[] { 0x0001 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0002 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0x0002, 0xF000 }, new ExpectResult("invalid", "missing third word") },
            { new ushort[] { 0x0002, 0xF000, 0x1234 }, new ExpectResult("invalid", "invalid third word") },
            { new ushort[] { 0x0003 }, new ExpectResult("SLEEP") },
            { new ushort[] { 0x0004 }, new ExpectResult("CLRWDT") },
            { new ushort[] { 0x0005 }, new ExpectResult("PUSH") },
            { new ushort[] { 0x0006 }, new ExpectResult("POP") },
            { new ushort[] { 0x0007 }, new ExpectResult("DAW") },
            { new ushort[] { 0x0008 }, new ExpectResult("TBLRD\t*") },
            { new ushort[] { 0x0009 }, new ExpectResult("TBLRD\t*+") },
            { new ushort[] { 0x000A }, new ExpectResult("TBLRD\t*-") },
            { new ushort[] { 0x000B }, new ExpectResult("TBLRD\t+*") },
            { new ushort[] { 0x000C }, new ExpectResult("TBLWT\t*") },
            { new ushort[] { 0x000D }, new ExpectResult("TBLWT\t*+") },
            { new ushort[] { 0x000E }, new ExpectResult("TBLWT\t*-") },
            { new ushort[] { 0x000F }, new ExpectResult("TBLWT\t+*") },
            { new ushort[] { 0x0010 }, new ExpectResult("RETFIE") },
            { new ushort[] { 0x0011 }, new ExpectResult("RETFIE\tS") },
            { new ushort[] { 0x0012 }, new ExpectResult("RETURN") },
            { new ushort[] { 0x0013 }, new ExpectResult("RETURN\tS") },
            { new ushort[] { 0x0015 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0016 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0017 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0018 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0019 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001A }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001B }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001C }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001D }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001E }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x001F }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0020 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0040 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x0060 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0x0067, 0x1234 }, new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0x006F, 0xF000 }, new ExpectResult("invalid", "missing third word") },
            { new ushort[] { 0x0080 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x00F0 }, new ExpectResult("invalid", "unknown opcode") },
            { new ushort[] { 0x00FF }, new ExpectResult("RESET") },

            { new ushort[] { 0x0100 }, new ExpectResult("MOVLB\t0x00") },
            { new ushort[] { 0x0107 }, new ExpectResult("MOVLB\t0x07") },
            { new ushort[] { 0x010F }, new ExpectResult("MOVLB\t0x0F") },
            { new ushort[] { 0x0140 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x0180 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x01E0 }, new ExpectResult("invalid", "(MOVLB) too large value") },

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

            { new ushort[] { 0xC000 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xC000, 0x0123 }, new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xC000, 0xF123 }, new ExpectResult("MOVFF\t0x0000,0x0123") },
            { new ushort[] { 0xC879, 0xFABC }, new ExpectResult("MOVFF\t0x0879,0x0ABC") },

            { new ushort[] { 0xD000 } , new ExpectResult("BRA\t0x202") },
            { new ushort[] { 0xD055 } , new ExpectResult("BRA\t0x2AC") },
            { new ushort[] { 0xD755 } , new ExpectResult("BRA\t0xAC") },
            { new ushort[] { 0xD7FF } , new ExpectResult("BRA\t0x200") },
            { new ushort[] { 0xD800 } , new ExpectResult("RCALL\t0x202") },
            { new ushort[] { 0xD8AA } , new ExpectResult("RCALL\t0x356") },
            { new ushort[] { 0xDBFF } , new ExpectResult("RCALL\t0xA00") },
            { new ushort[] { 0xDC00 } , new ExpectResult("RCALL\t0x1FFA02") },
            { new ushort[] { 0xDFAA } , new ExpectResult("RCALL\t0x156") },
            { new ushort[] { 0xDFFF } , new ExpectResult("RCALL\t0x200") },

            { new ushort[] { 0xE000 } , new ExpectResult("BZ\t0x202") },
            { new ushort[] { 0xE023 } , new ExpectResult("BZ\t0x248") },
            { new ushort[] { 0xE0FF } , new ExpectResult("BZ\t0x200") },
            { new ushort[] { 0xE0FE } , new ExpectResult("BZ\t0x1FE") },
            { new ushort[] { 0xE086 } , new ExpectResult("BZ\t0x10E") },
            { new ushort[] { 0xE100 } , new ExpectResult("BNZ\t0x202") },
            { new ushort[] { 0xE123 } , new ExpectResult("BNZ\t0x248") },
            { new ushort[] { 0xE1FF } , new ExpectResult("BNZ\t0x200") },
            { new ushort[] { 0xE1FE } , new ExpectResult("BNZ\t0x1FE") },
            { new ushort[] { 0xE186 } , new ExpectResult("BNZ\t0x10E") },
            { new ushort[] { 0xE200 } , new ExpectResult("BC\t0x202") },
            { new ushort[] { 0xE223 } , new ExpectResult("BC\t0x248") },
            { new ushort[] { 0xE2FF } , new ExpectResult("BC\t0x200") },
            { new ushort[] { 0xE2FE } , new ExpectResult("BC\t0x1FE") },
            { new ushort[] { 0xE286 } , new ExpectResult("BC\t0x10E") },
            { new ushort[] { 0xE300 } , new ExpectResult("BNC\t0x202") },
            { new ushort[] { 0xE323 } , new ExpectResult("BNC\t0x248") },
            { new ushort[] { 0xE3FF } , new ExpectResult("BNC\t0x200") },
            { new ushort[] { 0xE3FE } , new ExpectResult("BNC\t0x1FE") },
            { new ushort[] { 0xE386 } , new ExpectResult("BNC\t0x10E") },
            { new ushort[] { 0xE400 } , new ExpectResult("BOV\t0x202") },
            { new ushort[] { 0xE423 } , new ExpectResult("BOV\t0x248") },
            { new ushort[] { 0xE4FF } , new ExpectResult("BOV\t0x200") },
            { new ushort[] { 0xE4FE } , new ExpectResult("BOV\t0x1FE") },
            { new ushort[] { 0xE486 } , new ExpectResult("BOV\t0x10E") },
            { new ushort[] { 0xE500 } , new ExpectResult("BNOV\t0x202") },
            { new ushort[] { 0xE523 } , new ExpectResult("BNOV\t0x248") },
            { new ushort[] { 0xE5FF } , new ExpectResult("BNOV\t0x200") },
            { new ushort[] { 0xE5FE } , new ExpectResult("BNOV\t0x1FE") },
            { new ushort[] { 0xE586 } , new ExpectResult("BNOV\t0x10E") },
            { new ushort[] { 0xE600 } , new ExpectResult("BN\t0x202") },
            { new ushort[] { 0xE623 } , new ExpectResult("BN\t0x248") },
            { new ushort[] { 0xE6FF } , new ExpectResult("BN\t0x200") },
            { new ushort[] { 0xE6FE } , new ExpectResult("BN\t0x1FE") },
            { new ushort[] { 0xE686 } , new ExpectResult("BN\t0x10E") },
            { new ushort[] { 0xE700 } , new ExpectResult("BNN\t0x202") },
            { new ushort[] { 0xE723 } , new ExpectResult("BNN\t0x248") },
            { new ushort[] { 0xE7FF } , new ExpectResult("BNN\t0x200") },
            { new ushort[] { 0xE7FE } , new ExpectResult("BNN\t0x1FE") },
            { new ushort[] { 0xE786 } , new ExpectResult("BNN\t0x10E") },

            { new ushort[] { 0xEB00 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEB00, 0x1234 }, new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEB80 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEB80, 0x1234 }, new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEC00 } , new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEC00, 0x8765 } , new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEC06, 0xF000 } , new ExpectResult("CALL\t0x00000C") },
            { new ushort[] { 0xEC12, 0xF345 } , new ExpectResult("CALL\t0x068A24") },
            { new ushort[] { 0xED00 } , new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xED00, 0x9876 } , new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xED06, 0xF000 } , new ExpectResult("CALL\t0x00000C,S") },
            { new ushort[] { 0xED12, 0xF345 } , new ExpectResult("CALL\t0x068A24,S") },

            { new ushort[] { 0xEE00 } , new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEE00, 0x6543 } , new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEE00, 0xF400 }, new ExpectResult( "invalid", "LFSR too large literal value") },
            { new ushort[] { 0xEE30, 0xF000 }, new ExpectResult( "invalid", "invalid FSR number") },
            { new ushort[] { 0xEE40 }, new ExpectResult( "invalid", "unknown instruction") },
            { new ushort[] { 0xEEF0 }, new ExpectResult( "invalid", "unknown instruction") },

            { new ushort[] { 0xEF00 } , new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEF00, 0xEDCB } , new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEF03, 0xF000 } , new ExpectResult("GOTO\t0x000006") },
            { new ushort[] { 0xEF56, 0xF789 } , new ExpectResult("GOTO\t0x0F12AC") },

            { new ushort[] { 0xF000 } , new ExpectResult("NOP") },
            { new ushort[] { 0xF123 } , new ExpectResult("NOP") },
            { new ushort[] { 0xFEDC, 0xF256 } , new ExpectResult("NOP") },
        };

        // Instructions (bit patterns) which are common to all flavors of PIC18 for traditional execution mode only.
        // 
        private Dictionary<ushort[], ExpectResult> _anyPIC_tradMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {

            { new ushort[] { 0x0200 }, new ExpectResult("MULWF\t0x00,ACCESS") },
            { new ushort[] { 0x0255 }, new ExpectResult("MULWF\t0x55,ACCESS") },
            { new ushort[] { 0x02AC }, new ExpectResult("MULWF\t0xAC,ACCESS") },
            { new ushort[] { 0x0401 }, new ExpectResult("DECF\t0x01,W,ACCESS") },
            { new ushort[] { 0x0466 }, new ExpectResult("DECF\t0x66,W,ACCESS") },
            { new ushort[] { 0x0488 }, new ExpectResult("DECF\t0x88,W,ACCESS") },
            { new ushort[] { 0x0600 }, new ExpectResult("DECF\t0x00,ACCESS") },
            { new ushort[] { 0x0666 }, new ExpectResult("DECF\t0x66,ACCESS") },

            { new ushort[] { 0x1002 }, new ExpectResult("IORWF\t0x02,W,ACCESS") },
            { new ushort[] { 0x104F }, new ExpectResult("IORWF\t0x4F,W,ACCESS") },
            { new ushort[] { 0x10A5 }, new ExpectResult("IORWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1200 }, new ExpectResult("IORWF\t0x00,ACCESS") },
            { new ushort[] { 0x1235 }, new ExpectResult("IORWF\t0x35,ACCESS") },
            { new ushort[] { 0x12A5 }, new ExpectResult("IORWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1400 }, new ExpectResult("ANDWF\t0x00,W,ACCESS") },
            { new ushort[] { 0x1421 }, new ExpectResult("ANDWF\t0x21,W,ACCESS") },
            { new ushort[] { 0x14A5 }, new ExpectResult("ANDWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1600 }, new ExpectResult("ANDWF\t0x00,ACCESS") },
            { new ushort[] { 0x165A }, new ExpectResult("ANDWF\t0x5A,ACCESS") },
            { new ushort[] { 0x16A5 }, new ExpectResult("ANDWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1800 }, new ExpectResult("XORWF\t0x00,W,ACCESS") },
            { new ushort[] { 0x1847 }, new ExpectResult("XORWF\t0x47,W,ACCESS") },
            { new ushort[] { 0x18A5 }, new ExpectResult("XORWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1A00 }, new ExpectResult("XORWF\t0x00,ACCESS") },
            { new ushort[] { 0x1A5A }, new ExpectResult("XORWF\t0x5A,ACCESS") },
            { new ushort[] { 0x1AA5 }, new ExpectResult("XORWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1C00 }, new ExpectResult("COMF\t0x00,W,ACCESS") },
            { new ushort[] { 0x1C44 }, new ExpectResult("COMF\t0x44,W,ACCESS") },
            { new ushort[] { 0x1CA5 }, new ExpectResult("COMF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1E00 }, new ExpectResult("COMF\t0x00,ACCESS") },
            { new ushort[] { 0x1E37 }, new ExpectResult("COMF\t0x37,ACCESS") },
            { new ushort[] { 0x1EA5 }, new ExpectResult("COMF\t0xA5,ACCESS") },

            { new ushort[] { 0x2000 }, new ExpectResult("ADDWFC\t0x00,W,ACCESS") },
            { new ushort[] { 0x2053 }, new ExpectResult("ADDWFC\t0x53,W,ACCESS") },
            { new ushort[] { 0x20A5 }, new ExpectResult("ADDWFC\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2200 }, new ExpectResult("ADDWFC\t0x00,ACCESS") },
            { new ushort[] { 0x22A5 }, new ExpectResult("ADDWFC\t0xA5,ACCESS") },
            { new ushort[] { 0x2243 }, new ExpectResult("ADDWFC\t0x43,ACCESS") },
            { new ushort[] { 0x2400 }, new ExpectResult("ADDWF\t0x00,W,ACCESS") },
            { new ushort[] { 0x24A5 }, new ExpectResult("ADDWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2428 }, new ExpectResult("ADDWF\t0x28,W,ACCESS") },
            { new ushort[] { 0x2600 }, new ExpectResult("ADDWF\t0x00,ACCESS") },
            { new ushort[] { 0x2633 }, new ExpectResult("ADDWF\t0x33,ACCESS") },
            { new ushort[] { 0x26A5 }, new ExpectResult("ADDWF\t0xA5,ACCESS") },
            { new ushort[] { 0x2800 }, new ExpectResult("INCF\t0x00,W,ACCESS") },
            { new ushort[] { 0x2825 }, new ExpectResult("INCF\t0x25,W,ACCESS") },
            { new ushort[] { 0x28A5 }, new ExpectResult("INCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2A00 }, new ExpectResult("INCF\t0x00,ACCESS") },
            { new ushort[] { 0x2A53 }, new ExpectResult("INCF\t0x53,ACCESS") },
            { new ushort[] { 0x2AA5 }, new ExpectResult("INCF\t0xA5,ACCESS") },
            { new ushort[] { 0x2C00 }, new ExpectResult("DECFSZ\t0x00,W,ACCESS") },
            { new ushort[] { 0x2C36 }, new ExpectResult("DECFSZ\t0x36,W,ACCESS") },
            { new ushort[] { 0x2CA5 }, new ExpectResult("DECFSZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2E00 }, new ExpectResult("DECFSZ\t0x00,ACCESS") },
            { new ushort[] { 0x2E51 }, new ExpectResult("DECFSZ\t0x51,ACCESS") },
            { new ushort[] { 0x2EA5 }, new ExpectResult("DECFSZ\t0xA5,ACCESS") },

            { new ushort[] { 0x3000 }, new ExpectResult("RRCF\t0x00,W,ACCESS") },
            { new ushort[] { 0x3047 }, new ExpectResult("RRCF\t0x47,W,ACCESS") },
            { new ushort[] { 0x30A5 }, new ExpectResult("RRCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3200 }, new ExpectResult("RRCF\t0x00,ACCESS") },
            { new ushort[] { 0x3232 }, new ExpectResult("RRCF\t0x32,ACCESS") },
            { new ushort[] { 0x32A5 }, new ExpectResult("RRCF\t0xA5,ACCESS") },
            { new ushort[] { 0x3400 }, new ExpectResult("RLCF\t0x00,W,ACCESS") },
            { new ushort[] { 0x3489 }, new ExpectResult("RLCF\t0x89,W,ACCESS") },
            { new ushort[] { 0x34A5 }, new ExpectResult("RLCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3600 }, new ExpectResult("RLCF\t0x00,ACCESS") },
            { new ushort[] { 0x3642 }, new ExpectResult("RLCF\t0x42,ACCESS") },
            { new ushort[] { 0x36A5 }, new ExpectResult("RLCF\t0xA5,ACCESS") },
            { new ushort[] { 0x3800 }, new ExpectResult("SWAPF\t0x00,W,ACCESS") },
            { new ushort[] { 0x3838 }, new ExpectResult("SWAPF\t0x38,W,ACCESS") },
            { new ushort[] { 0x38A5 }, new ExpectResult("SWAPF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3A00 }, new ExpectResult("SWAPF\t0x00,ACCESS") },
            { new ushort[] { 0x3A4B }, new ExpectResult("SWAPF\t0x4B,ACCESS") },
            { new ushort[] { 0x3AA5 }, new ExpectResult("SWAPF\t0xA5,ACCESS") },
            { new ushort[] { 0x3C00 }, new ExpectResult("INCFSZ\t0x00,W,ACCESS") },
            { new ushort[] { 0x3C24 }, new ExpectResult("INCFSZ\t0x24,W,ACCESS") },
            { new ushort[] { 0x3CA5 }, new ExpectResult("INCFSZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3E00 }, new ExpectResult("INCFSZ\t0x00,ACCESS") },
            { new ushort[] { 0x3E4E }, new ExpectResult("INCFSZ\t0x4E,ACCESS") },
            { new ushort[] { 0x3EA5 }, new ExpectResult("INCFSZ\t0xA5,ACCESS") },

            { new ushort[] { 0x4000 }, new ExpectResult("RRNCF\t0x00,W,ACCESS") },
            { new ushort[] { 0x404D }, new ExpectResult("RRNCF\t0x4D,W,ACCESS") },
            { new ushort[] { 0x40C3 }, new ExpectResult("RRNCF\t0xC3,W,ACCESS") },
            { new ushort[] { 0x4600 }, new ExpectResult("RLNCF\t0x00,ACCESS") },
            { new ushort[] { 0x4637 }, new ExpectResult("RLNCF\t0x37,ACCESS") },
            { new ushort[] { 0x46A5 }, new ExpectResult("RLNCF\t0xA5,ACCESS") },
            { new ushort[] { 0x4200 }, new ExpectResult("RRNCF\t0x00,ACCESS") },
            { new ushort[] { 0x4243 }, new ExpectResult("RRNCF\t0x43,ACCESS") },
            { new ushort[] { 0x42A5 }, new ExpectResult("RRNCF\t0xA5,ACCESS") },
            { new ushort[] { 0x4400 }, new ExpectResult("RLNCF\t0x00,W,ACCESS") },
            { new ushort[] { 0x4422 }, new ExpectResult("RLNCF\t0x22,W,ACCESS") },
            { new ushort[] { 0x44A5 }, new ExpectResult("RLNCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4800 }, new ExpectResult("INFSNZ\t0x00,W,ACCESS") },
            { new ushort[] { 0x48A5 }, new ExpectResult("INFSNZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4855 }, new ExpectResult("INFSNZ\t0x55,W,ACCESS") },
            { new ushort[] { 0x4A00 }, new ExpectResult("INFSNZ\t0x00,ACCESS") },
            { new ushort[] { 0x4A33 }, new ExpectResult("INFSNZ\t0x33,ACCESS") },
            { new ushort[] { 0x4AA5 }, new ExpectResult("INFSNZ\t0xA5,ACCESS") },
            { new ushort[] { 0x4C00 }, new ExpectResult("DCFSNZ\t0x00,W,ACCESS") },
            { new ushort[] { 0x4C27 }, new ExpectResult("DCFSNZ\t0x27,W,ACCESS") },
            { new ushort[] { 0x4CA5 }, new ExpectResult("DCFSNZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4E00 }, new ExpectResult("DCFSNZ\t0x00,ACCESS") },
            { new ushort[] { 0x4EA5 }, new ExpectResult("DCFSNZ\t0xA5,ACCESS") },
            { new ushort[] { 0x4E11 }, new ExpectResult("DCFSNZ\t0x11,ACCESS") },

            { new ushort[] { 0x5000 }, new ExpectResult("MOVF\t0x00,W,ACCESS") },
            { new ushort[] { 0x5043 }, new ExpectResult("MOVF\t0x43,W,ACCESS") },
            { new ushort[] { 0x50A5 }, new ExpectResult("MOVF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5200 }, new ExpectResult("MOVF\t0x00,ACCESS") },
            { new ushort[] { 0x5212 }, new ExpectResult("MOVF\t0x12,ACCESS") },
            { new ushort[] { 0x52A5 }, new ExpectResult("MOVF\t0xA5,ACCESS") },
            { new ushort[] { 0x5400 }, new ExpectResult("SUBFWB\t0x00,W,ACCESS") },
            { new ushort[] { 0x5427 }, new ExpectResult("SUBFWB\t0x27,W,ACCESS") },
            { new ushort[] { 0x54A5 }, new ExpectResult("SUBFWB\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5600 }, new ExpectResult("SUBFWB\t0x00,ACCESS") },
            { new ushort[] { 0x564B }, new ExpectResult("SUBFWB\t0x4B,ACCESS") },
            { new ushort[] { 0x56A5 }, new ExpectResult("SUBFWB\t0xA5,ACCESS") },
            { new ushort[] { 0x5800 }, new ExpectResult("SUBWFB\t0x00,W,ACCESS") },
            { new ushort[] { 0x5837 }, new ExpectResult("SUBWFB\t0x37,W,ACCESS") },
            { new ushort[] { 0x58A5 }, new ExpectResult("SUBWFB\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5A00 }, new ExpectResult("SUBWFB\t0x00,ACCESS") },
            { new ushort[] { 0x5A5F }, new ExpectResult("SUBWFB\t0x5F,ACCESS") },
            { new ushort[] { 0x5A60 }, new ExpectResult("SUBWFB\t0x60,ACCESS") },
            { new ushort[] { 0x5C00 }, new ExpectResult("SUBWF\t0x00,W,ACCESS") },
            { new ushort[] { 0x5C5B }, new ExpectResult("SUBWF\t0x5B,W,ACCESS") },
            { new ushort[] { 0x5CA5 }, new ExpectResult("SUBWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5E00 }, new ExpectResult("SUBWF\t0x00,ACCESS") },
            { new ushort[] { 0x5E5A }, new ExpectResult("SUBWF\t0x5A,ACCESS") },
            { new ushort[] { 0x5EB3 }, new ExpectResult("SUBWF\t0xB3,ACCESS") },

            { new ushort[] { 0x6000 }, new ExpectResult("CPFSLT\t0x00,ACCESS") },
            { new ushort[] { 0x604E }, new ExpectResult("CPFSLT\t0x4E,ACCESS") },
            { new ushort[] { 0x60CE }, new ExpectResult("CPFSLT\t0xCE,ACCESS") },
            { new ushort[] { 0x6200 }, new ExpectResult("CPFSEQ\t0x00,ACCESS") },
            { new ushort[] { 0x625E }, new ExpectResult("CPFSEQ\t0x5E,ACCESS") },
            { new ushort[] { 0x626E }, new ExpectResult("CPFSEQ\t0x6E,ACCESS") },
            { new ushort[] { 0x6400 }, new ExpectResult("CPFSGT\t0x00,ACCESS") },
            { new ushort[] { 0x645E }, new ExpectResult("CPFSGT\t0x5E,ACCESS") },
            { new ushort[] { 0x6477 }, new ExpectResult("CPFSGT\t0x77,ACCESS") },
            { new ushort[] { 0x6600 }, new ExpectResult("TSTFSZ\t0x00,ACCESS") },
            { new ushort[] { 0x664C }, new ExpectResult("TSTFSZ\t0x4C,ACCESS") },
            { new ushort[] { 0x66CE }, new ExpectResult("TSTFSZ\t0xCE,ACCESS") },
            { new ushort[] { 0x6800 }, new ExpectResult("SETF\t0x00,ACCESS") },
            { new ushort[] { 0x6846 }, new ExpectResult("SETF\t0x46,ACCESS") },
            { new ushort[] { 0x6877 }, new ExpectResult("SETF\t0x77,ACCESS") },
            { new ushort[] { 0x6A00 }, new ExpectResult("CLRF\t0x00,ACCESS") },
            { new ushort[] { 0x6A46 }, new ExpectResult("CLRF\t0x46,ACCESS") },
            { new ushort[] { 0x6ACE }, new ExpectResult("CLRF\t0xCE,ACCESS") },
            { new ushort[] { 0x6C00 }, new ExpectResult("NEGF\t0x00,ACCESS") },
            { new ushort[] { 0x6C57 }, new ExpectResult("NEGF\t0x57,ACCESS") },
            { new ushort[] { 0x6CCE }, new ExpectResult("NEGF\t0xCE,ACCESS") },
            { new ushort[] { 0x6E06 }, new ExpectResult("MOVWF\t0x06,ACCESS") },
            { new ushort[] { 0x6E56 }, new ExpectResult("MOVWF\t0x56,ACCESS") },
            { new ushort[] { 0x6E65 }, new ExpectResult("MOVWF\t0x65,ACCESS") },

            { new ushort[] { 0x7000 }, new ExpectResult("BTG\t0x00,0,ACCESS") },
            { new ushort[] { 0x705C }, new ExpectResult("BTG\t0x5C,0,ACCESS") },
            { new ushort[] { 0x70BC }, new ExpectResult("BTG\t0xBC,0,ACCESS") },
            { new ushort[] { 0x7600 }, new ExpectResult("BTG\t0x00,3,ACCESS") },
            { new ushort[] { 0x765F }, new ExpectResult("BTG\t0x5F,3,ACCESS") },
            { new ushort[] { 0x76FB }, new ExpectResult("BTG\t0xFB,3,ACCESS") },
            { new ushort[] { 0x8000 }, new ExpectResult("BSF\t0x00,0,ACCESS") },
            { new ushort[] { 0x8033 }, new ExpectResult("BSF\t0x33,0,ACCESS") },
            { new ushort[] { 0x80BC }, new ExpectResult("BSF\t0xBC,0,ACCESS") },
            { new ushort[] { 0x8600 }, new ExpectResult("BSF\t0x00,3,ACCESS") },
            { new ushort[] { 0x8638 }, new ExpectResult("BSF\t0x38,3,ACCESS") },
            { new ushort[] { 0x86CB }, new ExpectResult("BSF\t0xCB,3,ACCESS") },
            { new ushort[] { 0x9010 }, new ExpectResult("BCF\t0x10,0,ACCESS") },
            { new ushort[] { 0x9047 }, new ExpectResult("BCF\t0x47,0,ACCESS") },
            { new ushort[] { 0x9073 }, new ExpectResult("BCF\t0x73,0,ACCESS") },
            { new ushort[] { 0x9600 }, new ExpectResult("BCF\t0x00,3,ACCESS") },
            { new ushort[] { 0x9652 }, new ExpectResult("BCF\t0x52,3,ACCESS") },
            { new ushort[] { 0x96CB }, new ExpectResult("BCF\t0xCB,3,ACCESS") },
            { new ushort[] { 0xA000 }, new ExpectResult("BTFSS\t0x00,0,ACCESS") },
            { new ushort[] { 0xA04C }, new ExpectResult("BTFSS\t0x4C,0,ACCESS") },
            { new ushort[] { 0xA0A0 }, new ExpectResult("BTFSS\t0xA0,0,ACCESS") },
            { new ushort[] { 0xA611 }, new ExpectResult("BTFSS\t0x11,3,ACCESS") },
            { new ushort[] { 0xA655 }, new ExpectResult("BTFSS\t0x55,3,ACCESS") },
            { new ushort[] { 0xA666 }, new ExpectResult("BTFSS\t0x66,3,ACCESS") },
            { new ushort[] { 0xB012 }, new ExpectResult("BTFSC\t0x12,0,ACCESS") },
            { new ushort[] { 0xB042 }, new ExpectResult("BTFSC\t0x42,0,ACCESS") },
            { new ushort[] { 0xB0CC }, new ExpectResult("BTFSC\t0xCC,0,ACCESS") },
            { new ushort[] { 0xB600 }, new ExpectResult("BTFSC\t0x00,3,ACCESS") },
            { new ushort[] { 0xB659 }, new ExpectResult("BTFSC\t0x59,3,ACCESS") },
            { new ushort[] { 0xB6BC }, new ExpectResult("BTFSC\t0xBC,3,ACCESS") },

        };

        // Instructions (bit patterns) which are common to PIC18 supporting indexed addressing mode.
        // 
        private Dictionary<ushort[], ExpectResult> _anyPIC_extdMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {

            { new ushort[] { 0x0200 }, new ExpectResult("MULWF\t[0x00]") },
            { new ushort[] { 0x0255 }, new ExpectResult("MULWF\t[0x55]") },
            { new ushort[] { 0x02AC }, new ExpectResult("MULWF\t0xAC,ACCESS") },
            { new ushort[] { 0x0401 }, new ExpectResult("DECF\t[0x01],W") },
            { new ushort[] { 0x045E }, new ExpectResult("DECF\t[0x5E],W") },
            { new ushort[] { 0x0466 }, new ExpectResult("DECF\t0x66,W,ACCESS") },
            { new ushort[] { 0x0488 }, new ExpectResult("DECF\t0x88,W,ACCESS") },
            { new ushort[] { 0x0600 }, new ExpectResult("DECF\t[0x00]") },
            { new ushort[] { 0x0666 }, new ExpectResult("DECF\t0x66,ACCESS") },

            { new ushort[] { 0x1002 }, new ExpectResult("IORWF\t[0x02],W") },
            { new ushort[] { 0x104F }, new ExpectResult("IORWF\t[0x4F],W") },
            { new ushort[] { 0x10A5 }, new ExpectResult("IORWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1200 }, new ExpectResult("IORWF\t[0x00]") },
            { new ushort[] { 0x1235 }, new ExpectResult("IORWF\t[0x35]") },
            { new ushort[] { 0x12A5 }, new ExpectResult("IORWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1400 }, new ExpectResult("ANDWF\t[0x00],W") },
            { new ushort[] { 0x1421 }, new ExpectResult("ANDWF\t[0x21],W") },
            { new ushort[] { 0x14A5 }, new ExpectResult("ANDWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1600 }, new ExpectResult("ANDWF\t[0x00]") },
            { new ushort[] { 0x165A }, new ExpectResult("ANDWF\t[0x5A]") },
            { new ushort[] { 0x16A5 }, new ExpectResult("ANDWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1800 }, new ExpectResult("XORWF\t[0x00],W") },
            { new ushort[] { 0x1847 }, new ExpectResult("XORWF\t[0x47],W") },
            { new ushort[] { 0x18A5 }, new ExpectResult("XORWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1A00 }, new ExpectResult("XORWF\t[0x00]") },
            { new ushort[] { 0x1A5A }, new ExpectResult("XORWF\t[0x5A]") },
            { new ushort[] { 0x1AA5 }, new ExpectResult("XORWF\t0xA5,ACCESS") },
            { new ushort[] { 0x1C00 }, new ExpectResult("COMF\t[0x00],W") },
            { new ushort[] { 0x1C44 }, new ExpectResult("COMF\t[0x44],W") },
            { new ushort[] { 0x1CA5 }, new ExpectResult("COMF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x1E00 }, new ExpectResult("COMF\t[0x00]") },
            { new ushort[] { 0x1E37 }, new ExpectResult("COMF\t[0x37]") },
            { new ushort[] { 0x1EA5 }, new ExpectResult("COMF\t0xA5,ACCESS") },

            { new ushort[] { 0x2000 }, new ExpectResult("ADDWFC\t[0x00],W") },
            { new ushort[] { 0x2053 }, new ExpectResult("ADDWFC\t[0x53],W") },
            { new ushort[] { 0x20A5 }, new ExpectResult("ADDWFC\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2200 }, new ExpectResult("ADDWFC\t[0x00]") },
            { new ushort[] { 0x22A5 }, new ExpectResult("ADDWFC\t0xA5,ACCESS") },
            { new ushort[] { 0x2243 }, new ExpectResult("ADDWFC\t[0x43]") },
            { new ushort[] { 0x2400 }, new ExpectResult("ADDWF\t[0x00],W") },
            { new ushort[] { 0x24A5 }, new ExpectResult("ADDWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2428 }, new ExpectResult("ADDWF\t[0x28],W") },
            { new ushort[] { 0x2600 }, new ExpectResult("ADDWF\t[0x00]") },
            { new ushort[] { 0x2633 }, new ExpectResult("ADDWF\t[0x33]") },
            { new ushort[] { 0x26A5 }, new ExpectResult("ADDWF\t0xA5,ACCESS") },
            { new ushort[] { 0x2800 }, new ExpectResult("INCF\t[0x00],W") },
            { new ushort[] { 0x2825 }, new ExpectResult("INCF\t[0x25],W") },
            { new ushort[] { 0x28A5 }, new ExpectResult("INCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2A00 }, new ExpectResult("INCF\t[0x00]") },
            { new ushort[] { 0x2A53 }, new ExpectResult("INCF\t[0x53]") },
            { new ushort[] { 0x2AA5 }, new ExpectResult("INCF\t0xA5,ACCESS") },
            { new ushort[] { 0x2C00 }, new ExpectResult("DECFSZ\t[0x00],W") },
            { new ushort[] { 0x2C36 }, new ExpectResult("DECFSZ\t[0x36],W") },
            { new ushort[] { 0x2CA5 }, new ExpectResult("DECFSZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x2E00 }, new ExpectResult("DECFSZ\t[0x00]") },
            { new ushort[] { 0x2E51 }, new ExpectResult("DECFSZ\t[0x51]") },
            { new ushort[] { 0x2EA5 }, new ExpectResult("DECFSZ\t0xA5,ACCESS") },

            { new ushort[] { 0x3000 }, new ExpectResult("RRCF\t[0x00],W") },
            { new ushort[] { 0x3047 }, new ExpectResult("RRCF\t[0x47],W") },
            { new ushort[] { 0x30A5 }, new ExpectResult("RRCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3200 }, new ExpectResult("RRCF\t[0x00]") },
            { new ushort[] { 0x3232 }, new ExpectResult("RRCF\t[0x32]") },
            { new ushort[] { 0x32A5 }, new ExpectResult("RRCF\t0xA5,ACCESS") },
            { new ushort[] { 0x3400 }, new ExpectResult("RLCF\t[0x00],W") },
            { new ushort[] { 0x3439 }, new ExpectResult("RLCF\t[0x39],W") },
            { new ushort[] { 0x34A5 }, new ExpectResult("RLCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3600 }, new ExpectResult("RLCF\t[0x00]") },
            { new ushort[] { 0x3642 }, new ExpectResult("RLCF\t[0x42]") },
            { new ushort[] { 0x36A5 }, new ExpectResult("RLCF\t0xA5,ACCESS") },
            { new ushort[] { 0x3800 }, new ExpectResult("SWAPF\t[0x00],W") },
            { new ushort[] { 0x3838 }, new ExpectResult("SWAPF\t[0x38],W") },
            { new ushort[] { 0x38A5 }, new ExpectResult("SWAPF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3A00 }, new ExpectResult("SWAPF\t[0x00]") },
            { new ushort[] { 0x3A4B }, new ExpectResult("SWAPF\t[0x4B]") },
            { new ushort[] { 0x3AA5 }, new ExpectResult("SWAPF\t0xA5,ACCESS") },
            { new ushort[] { 0x3C00 }, new ExpectResult("INCFSZ\t[0x00],W") },
            { new ushort[] { 0x3C24 }, new ExpectResult("INCFSZ\t[0x24],W") },
            { new ushort[] { 0x3CA5 }, new ExpectResult("INCFSZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x3E00 }, new ExpectResult("INCFSZ\t[0x00]") },
            { new ushort[] { 0x3E4E }, new ExpectResult("INCFSZ\t[0x4E]") },
            { new ushort[] { 0x3EA5 }, new ExpectResult("INCFSZ\t0xA5,ACCESS") },

            { new ushort[] { 0x4000 }, new ExpectResult("RRNCF\t[0x00],W") },
            { new ushort[] { 0x404D }, new ExpectResult("RRNCF\t[0x4D],W") },
            { new ushort[] { 0x40C3 }, new ExpectResult("RRNCF\t0xC3,W,ACCESS") },
            { new ushort[] { 0x4600 }, new ExpectResult("RLNCF\t[0x00]") },
            { new ushort[] { 0x4637 }, new ExpectResult("RLNCF\t[0x37]") },
            { new ushort[] { 0x46A5 }, new ExpectResult("RLNCF\t0xA5,ACCESS") },
            { new ushort[] { 0x4200 }, new ExpectResult("RRNCF\t[0x00]") },
            { new ushort[] { 0x4243 }, new ExpectResult("RRNCF\t[0x43]") },
            { new ushort[] { 0x42A5 }, new ExpectResult("RRNCF\t0xA5,ACCESS") },
            { new ushort[] { 0x4400 }, new ExpectResult("RLNCF\t[0x00],W") },
            { new ushort[] { 0x4422 }, new ExpectResult("RLNCF\t[0x22],W") },
            { new ushort[] { 0x44A5 }, new ExpectResult("RLNCF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4800 }, new ExpectResult("INFSNZ\t[0x00],W") },
            { new ushort[] { 0x48A5 }, new ExpectResult("INFSNZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4855 }, new ExpectResult("INFSNZ\t[0x55],W") },
            { new ushort[] { 0x4A00 }, new ExpectResult("INFSNZ\t[0x00]") },
            { new ushort[] { 0x4A33 }, new ExpectResult("INFSNZ\t[0x33]") },
            { new ushort[] { 0x4AA5 }, new ExpectResult("INFSNZ\t0xA5,ACCESS") },
            { new ushort[] { 0x4C00 }, new ExpectResult("DCFSNZ\t[0x00],W") },
            { new ushort[] { 0x4C27 }, new ExpectResult("DCFSNZ\t[0x27],W") },
            { new ushort[] { 0x4CA5 }, new ExpectResult("DCFSNZ\t0xA5,W,ACCESS") },
            { new ushort[] { 0x4E00 }, new ExpectResult("DCFSNZ\t[0x00]") },
            { new ushort[] { 0x4EA5 }, new ExpectResult("DCFSNZ\t0xA5,ACCESS") },
            { new ushort[] { 0x4E11 }, new ExpectResult("DCFSNZ\t[0x11]") },

            { new ushort[] { 0x5000 }, new ExpectResult("MOVF\t[0x00],W") },
            { new ushort[] { 0x5043 }, new ExpectResult("MOVF\t[0x43],W") },
            { new ushort[] { 0x50A5 }, new ExpectResult("MOVF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5200 }, new ExpectResult("MOVF\t[0x00]") },
            { new ushort[] { 0x5212 }, new ExpectResult("MOVF\t[0x12]") },
            { new ushort[] { 0x52A5 }, new ExpectResult("MOVF\t0xA5,ACCESS") },
            { new ushort[] { 0x5400 }, new ExpectResult("SUBFWB\t[0x00],W") },
            { new ushort[] { 0x5427 }, new ExpectResult("SUBFWB\t[0x27],W") },
            { new ushort[] { 0x54A5 }, new ExpectResult("SUBFWB\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5600 }, new ExpectResult("SUBFWB\t[0x00]") },
            { new ushort[] { 0x564B }, new ExpectResult("SUBFWB\t[0x4B]") },
            { new ushort[] { 0x56A5 }, new ExpectResult("SUBFWB\t0xA5,ACCESS") },
            { new ushort[] { 0x5800 }, new ExpectResult("SUBWFB\t[0x00],W") },
            { new ushort[] { 0x5837 }, new ExpectResult("SUBWFB\t[0x37],W") },
            { new ushort[] { 0x58A5 }, new ExpectResult("SUBWFB\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5A00 }, new ExpectResult("SUBWFB\t[0x00]") },
            { new ushort[] { 0x5A5F }, new ExpectResult("SUBWFB\t[0x5F]") },
            { new ushort[] { 0x5A60 }, new ExpectResult("SUBWFB\t0x60,ACCESS") },
            { new ushort[] { 0x5C00 }, new ExpectResult("SUBWF\t[0x00],W") },
            { new ushort[] { 0x5C5B }, new ExpectResult("SUBWF\t[0x5B],W") },
            { new ushort[] { 0x5CA5 }, new ExpectResult("SUBWF\t0xA5,W,ACCESS") },
            { new ushort[] { 0x5E00 }, new ExpectResult("SUBWF\t[0x00]") },
            { new ushort[] { 0x5E5A }, new ExpectResult("SUBWF\t[0x5A]") },
            { new ushort[] { 0x5EB3 }, new ExpectResult("SUBWF\t0xB3,ACCESS") },

            { new ushort[] { 0x6000 }, new ExpectResult("CPFSLT\t[0x00]") },
            { new ushort[] { 0x604E }, new ExpectResult("CPFSLT\t[0x4E]") },
            { new ushort[] { 0x60CE }, new ExpectResult("CPFSLT\t0xCE,ACCESS") },
            { new ushort[] { 0x6200 }, new ExpectResult("CPFSEQ\t[0x00]") },
            { new ushort[] { 0x625E }, new ExpectResult("CPFSEQ\t[0x5E]") },
            { new ushort[] { 0x626E }, new ExpectResult("CPFSEQ\t0x6E,ACCESS") },
            { new ushort[] { 0x6400 }, new ExpectResult("CPFSGT\t[0x00]") },
            { new ushort[] { 0x645E }, new ExpectResult("CPFSGT\t[0x5E]") },
            { new ushort[] { 0x6477 }, new ExpectResult("CPFSGT\t0x77,ACCESS") },
            { new ushort[] { 0x6600 }, new ExpectResult("TSTFSZ\t[0x00]") },
            { new ushort[] { 0x664C }, new ExpectResult("TSTFSZ\t[0x4C]") },
            { new ushort[] { 0x66CE }, new ExpectResult("TSTFSZ\t0xCE,ACCESS") },
            { new ushort[] { 0x6800 }, new ExpectResult("SETF\t[0x00]") },
            { new ushort[] { 0x6846 }, new ExpectResult("SETF\t[0x46]") },
            { new ushort[] { 0x6877 }, new ExpectResult("SETF\t0x77,ACCESS") },
            { new ushort[] { 0x6A00 }, new ExpectResult("CLRF\t[0x00]") },
            { new ushort[] { 0x6A46 }, new ExpectResult("CLRF\t[0x46]") },
            { new ushort[] { 0x6ACE }, new ExpectResult("CLRF\t0xCE,ACCESS") },
            { new ushort[] { 0x6C00 }, new ExpectResult("NEGF\t[0x00]") },
            { new ushort[] { 0x6C57 }, new ExpectResult("NEGF\t[0x57]") },
            { new ushort[] { 0x6CCE }, new ExpectResult("NEGF\t0xCE,ACCESS") },
            { new ushort[] { 0x6E06 }, new ExpectResult("MOVWF\t[0x06]") },
            { new ushort[] { 0x6E56 }, new ExpectResult("MOVWF\t[0x56]") },
            { new ushort[] { 0x6E65 }, new ExpectResult("MOVWF\t0x65,ACCESS") },

            { new ushort[] { 0x7000 }, new ExpectResult("BTG\t[0x00],0") },
            { new ushort[] { 0x705C }, new ExpectResult("BTG\t[0x5C],0") },
            { new ushort[] { 0x70BC }, new ExpectResult("BTG\t0xBC,0,ACCESS") },
            { new ushort[] { 0x7600 }, new ExpectResult("BTG\t[0x00],3") },
            { new ushort[] { 0x765F }, new ExpectResult("BTG\t[0x5F],3") },
            { new ushort[] { 0x76FB }, new ExpectResult("BTG\t0xFB,3,ACCESS") },
            { new ushort[] { 0x8000 }, new ExpectResult("BSF\t[0x00],0") },
            { new ushort[] { 0x8033 }, new ExpectResult("BSF\t[0x33],0") },
            { new ushort[] { 0x80BC }, new ExpectResult("BSF\t0xBC,0,ACCESS") },
            { new ushort[] { 0x8600 }, new ExpectResult("BSF\t[0x00],3") },
            { new ushort[] { 0x8638 }, new ExpectResult("BSF\t[0x38],3") },
            { new ushort[] { 0x86CB }, new ExpectResult("BSF\t0xCB,3,ACCESS") },
            { new ushort[] { 0x9010 }, new ExpectResult("BCF\t[0x10],0") },
            { new ushort[] { 0x9047 }, new ExpectResult("BCF\t[0x47],0") },
            { new ushort[] { 0x9073 }, new ExpectResult("BCF\t0x73,0,ACCESS") },
            { new ushort[] { 0x9600 }, new ExpectResult("BCF\t[0x00],3") },
            { new ushort[] { 0x9652 }, new ExpectResult("BCF\t[0x52],3") },
            { new ushort[] { 0x96CB }, new ExpectResult("BCF\t0xCB,3,ACCESS") },
            { new ushort[] { 0xA000 }, new ExpectResult("BTFSS\t[0x00],0") },
            { new ushort[] { 0xA04C }, new ExpectResult("BTFSS\t[0x4C],0") },
            { new ushort[] { 0xA0A0 }, new ExpectResult("BTFSS\t0xA0,0,ACCESS") },
            { new ushort[] { 0xA611 }, new ExpectResult("BTFSS\t[0x11],3") },
            { new ushort[] { 0xA655 }, new ExpectResult("BTFSS\t[0x55],3") },
            { new ushort[] { 0xA666 }, new ExpectResult("BTFSS\t0x66,3,ACCESS") },
            { new ushort[] { 0xB012 }, new ExpectResult("BTFSC\t[0x12],0") },
            { new ushort[] { 0xB042 }, new ExpectResult("BTFSC\t[0x42],0") },
            { new ushort[] { 0xB0CC }, new ExpectResult("BTFSC\t0xCC,0,ACCESS") },
            { new ushort[] { 0xB600 }, new ExpectResult("BTFSC\t[0x00],3") },
            { new ushort[] { 0xB659 }, new ExpectResult("BTFSC\t[0x59],3") },
            { new ushort[] { 0xB6BC }, new ExpectResult("BTFSC\t0xBC,3,ACCESS") },

        };

        // Instructions (bit patterns) which are no supported by legacy PIC18 (any execution mode).
        // 
        private Dictionary<ushort[], ExpectResult> _pic18Only_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0002, 0xF000, 0xF000 }, new ExpectResult("invalid", "(MOVSFL) unsupported instruction") },
            { new ushort[] { 0x0014 }, new ExpectResult("invalid", "(CALLW) unsupported instruction") },
            { new ushort[] { 0x006F, 0xF000, 0xF000 }, new ExpectResult("invalid", "(MOVFFL) unsupported instruction") },
            { new ushort[] { 0x0100 }, new ExpectResult("MOVLB\t0x00") },
            { new ushort[] { 0x0110 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x0120 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0xE800 }, new ExpectResult("invalid", "(ADDFSR) not supported instruction") },
            { new ushort[] { 0xE8C0 }, new ExpectResult("invalid", "(ADDULNK) not supported instruction") },
            { new ushort[] { 0xE900 }, new ExpectResult("invalid", "(SUBFSR) not supported instruction") },
            { new ushort[] { 0xE9C0 }, new ExpectResult("invalid", "(SUBULNK) not supported instruction") },
            { new ushort[] { 0xEA00 }, new ExpectResult("invalid", "(PUSHL) not supported instruction") },
            { new ushort[] { 0xEB00, 0xF000 }, new ExpectResult("invalid", "(ADDULNK) not supported instruction") },
            { new ushort[] { 0xEB80, 0xF000 }, new ExpectResult("invalid", "(MOVSS) not supported instruction") },
            { new ushort[] { 0xEE00, 0xF034 }, new ExpectResult( "LFSR\tFSR0,0x034") },
            { new ushort[] { 0xEE00, 0xF234 }, new ExpectResult("invalid", "(LFSR) too large value") },
        };

        // Instructions (bit patterns) which supported by "egg" PIC18 in traditional execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _egg_tradMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0002, 0xF000, 0xF000}, new ExpectResult("invalid", "(MOVFSL) unsupported instruction") },
            { new ushort[] { 0x0014 }, new ExpectResult("CALLW") },
            { new ushort[] { 0x0060, 0xF000, 0xF000 }, new ExpectResult("invalid", "(MOVFFL) unsupported instruction") },
            { new ushort[] { 0x0105 }, new ExpectResult("MOVLB\t0x05") },
            { new ushort[] { 0x0110 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x0120 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x013F }, new ExpectResult("invalid", "(MOVLB) too large value") },

            { new ushort[] { 0xE800 }, new ExpectResult("invalid", "(ADDFSR) not supported instruction in traditional mode") },
            { new ushort[] { 0xE8C0 }, new ExpectResult("invalid", "(ADDULNK) not supported instruction in traditional mode") },
            { new ushort[] { 0xE900 }, new ExpectResult("invalid", "(SUBFSR) not supported instruction in traditional mode") },
            { new ushort[] { 0xE9C0 }, new ExpectResult("invalid", "(SUBULNK) not supported instruction in traditional mode") },
            { new ushort[] { 0xEA00 }, new ExpectResult("invalid", "(PUSHL) not supported instruction in traditional mode") },
            { new ushort[] { 0xEB00, 0xF000 }, new ExpectResult("invalid", "(MOVSF) not supported instruction in traditional mode") },
            { new ushort[] { 0xEB80, 0xF000 }, new ExpectResult("invalid", "(MOVSS) not supported instruction in traditional mode") },
            { new ushort[] { 0xEE00, 0xF000 } , new ExpectResult("LFSR\tFSR0,0x000") },
            { new ushort[] { 0xEE13, 0xF045 } , new ExpectResult("LFSR\tFSR1,0x345") },
            { new ushort[] { 0xEE26, 0xF078 } , new ExpectResult("LFSR\tFSR2,0x678") },
            { new ushort[] { 0xEE20, 0xF300 } , new ExpectResult("invalid", "(LSFR) too large literal value") },

        };

        // Instructions (bit patterns) which supported by "egg" PIC18 in extended execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _egg_extdMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0002, 0xF000, 0xF000}, new ExpectResult("invalid", "(MOVFSL) unsupported instruction") },
            { new ushort[] { 0x0014 }, new ExpectResult("CALLW") },
            { new ushort[] { 0x0060, 0xF000, 0xF000 }, new ExpectResult("invalid", "(MOVFFL) unsupported instruction") },
            { new ushort[] { 0x0105 }, new ExpectResult("MOVLB\t0x05") },
            { new ushort[] { 0x0110 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x0120 }, new ExpectResult("invalid", "(MOVLB) too large value") },
            { new ushort[] { 0x013F }, new ExpectResult("invalid", "(MOVLB) too large value") },

            { new ushort[] { 0xE800 }, new ExpectResult("ADDFSR\tFSR0,0x00") },
            { new ushort[] { 0xE863 }, new ExpectResult("ADDFSR\tFSR1,0x23") },
            { new ushort[] { 0xE8BF }, new ExpectResult("ADDFSR\tFSR2,0x3F") },
            { new ushort[] { 0xE8E1 }, new ExpectResult("ADDULNK\t0x21") },
            { new ushort[] { 0xE900 }, new ExpectResult("SUBFSR\tFSR0,0x00") },
            { new ushort[] { 0xE97F }, new ExpectResult("SUBFSR\tFSR1,0x3F") },
            { new ushort[] { 0xE9A3 }, new ExpectResult("SUBFSR\tFSR2,0x23") },
            { new ushort[] { 0xE9E2 }, new ExpectResult("SUBULNK\t0x22") },
            { new ushort[] { 0xEA5A }, new ExpectResult("PUSHL\t0x5A") },
            { new ushort[] { 0xEB07, 0xF234 }, new ExpectResult("MOVSF\t[0x07],0x0234") },
            { new ushort[] { 0xEB80 }, new ExpectResult("invalid", "missing second word") },
            { new ushort[] { 0xEB80, 0x4321 }, new ExpectResult("invalid", "invalid second word") },
            { new ushort[] { 0xEB8C, 0xF0FF }, new ExpectResult("MOVSS\t[0x0C],[0x7F]") },
            { new ushort[] { 0xEB88, 0xF025 }, new ExpectResult("MOVSS\t[0x08],[0x25]") },

            { new ushort[] { 0xEE00, 0xF000 } , new ExpectResult("LFSR\tFSR0,0x000") },
            { new ushort[] { 0xEE30, 0xF000 } , new ExpectResult("invalid", "invalid FSR number") },
            { new ushort[] { 0xEE13, 0xF045 } , new ExpectResult("LFSR\tFSR1,0x345") },
            { new ushort[] { 0xEE26, 0xF078 } , new ExpectResult("LFSR\tFSR2,0x678") },
            { new ushort[] { 0xEE20, 0xF300 } , new ExpectResult("invalid", "(LFSR) too large literal value") },

        };

        // Instructions (bit patterns) which supported by enhanced PIC18 in traditional execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _enhanced_tradMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0002, 0xF000, 0xF000 }, new ExpectResult("invalid", "(MOVFSL) unsupported in traditional mode") },
            { new ushort[] { 0x0014 }, new ExpectResult("CALLW") },
            { new ushort[] { 0x0066, 0xF619, 0xF987 }, new ExpectResult("MOVFFL\t0x1986,0x1987") },
            { new ushort[] { 0x0100 }, new ExpectResult("MOVLB\t0x00") },
            { new ushort[] { 0x0107 }, new ExpectResult("MOVLB\t0x07") },
            { new ushort[] { 0x010F }, new ExpectResult("MOVLB\t0x0F") },
            { new ushort[] { 0x0110 }, new ExpectResult("MOVLB\t0x10") },
            { new ushort[] { 0x0120 }, new ExpectResult("MOVLB\t0x20") },

            { new ushort[] { 0xE823 }, new ExpectResult("ADDFSR\tFSR0,0x23") },
            { new ushort[] { 0xE8C0 }, new ExpectResult("invalid", "(ADDULNK) not supported instruction in traditional mode") },
            { new ushort[] { 0xE933 }, new ExpectResult("SUBFSR\tFSR0,0x33") },
            { new ushort[] { 0xE9C0 }, new ExpectResult("invalid", "(SUBULNK) not supported instruction in traditional mode") },
            { new ushort[] { 0xEA00 }, new ExpectResult("invalid", "(PUSHL) not supported instruction in traditional mode") },
            { new ushort[] { 0xEB00 }, new ExpectResult("invalid", "(MOVSF) not supported instruction in traditional mode") },
            { new ushort[] { 0xEB80 }, new ExpectResult("invalid", "(MOVSS) not supported instruction in traditional mode") },

            { new ushort[] { 0xEE00, 0xF000 } , new ExpectResult("LFSR\tFSR0,0x0000") },
            { new ushort[] { 0xEE30, 0xF000 } , new ExpectResult("invalid", "invalid FSR number") },
            { new ushort[] { 0xEE13, 0xF045 } , new ExpectResult("LFSR\tFSR1,0x0C45") },
            { new ushort[] { 0xEE26, 0xF078 } , new ExpectResult("LFSR\tFSR2,0x1878") },
            { new ushort[] { 0xEE20, 0xF300 } , new ExpectResult("LFSR\tFSR2,0x0300") },

        };

        // Instructions (bit patterns) which supported by enhanced PIC18 in extended execution mode.
        // 
        private Dictionary<ushort[], ExpectResult> _enhanced_extdMode_Instrs = new Dictionary<ushort[], ExpectResult>()
        {
            { new ushort[] { 0x0002, 0xF010, 0xF789 }, new ExpectResult("MOVSFL\t[0x04],0x0789") },
            { new ushort[] { 0x0014 }, new ExpectResult("CALLW") },
            { new ushort[] { 0x0062, 0xF345, 0xFABC }, new ExpectResult("MOVFFL\t0x08D1,0x1ABC") },
            { new ushort[] { 0x0100 }, new ExpectResult("MOVLB\t0x00") },
            { new ushort[] { 0x0107 }, new ExpectResult("MOVLB\t0x07") },
            { new ushort[] { 0x010F }, new ExpectResult("MOVLB\t0x0F") },
            { new ushort[] { 0x0110 }, new ExpectResult("MOVLB\t0x10") },
            { new ushort[] { 0x0120 }, new ExpectResult("MOVLB\t0x20") },

            { new ushort[] { 0xC000 }, new ExpectResult("invalid", "missing second word") }, 
            { new ushort[] { 0xC000, 0x0123 }, new ExpectResult("invalid", "invalid second word") }, 
            { new ushort[] { 0xC000, 0xF123 }, new ExpectResult("MOVFF\t0x0000,0x0123") },
            { new ushort[] { 0xC879, 0xFABC }, new ExpectResult("MOVFF\t0x0879,0x0ABC") },

            { new ushort[] { 0xE823 }, new ExpectResult("ADDFSR\tFSR0,0x23") },
            { new ushort[] { 0xE8E1 }, new ExpectResult("ADDULNK\t0x21") },
            { new ushort[] { 0xE933 }, new ExpectResult("SUBFSR\tFSR0,0x33") },
            { new ushort[] { 0xE9E2 }, new ExpectResult("SUBULNK\t0x22") },
            { new ushort[] { 0xEA00 }, new ExpectResult("PUSHL\t0x00") },
            { new ushort[] { 0xEB8C, 0xF0FF }, new ExpectResult("MOVSS\t[0x0C],[0x7F]") },
            { new ushort[] { 0xEB88, 0xF025 }, new ExpectResult("MOVSS\t[0x08],[0x25]") },

            { new ushort[] { 0xEE00, 0xF000 } , new ExpectResult("LFSR\tFSR0,0x0000") },
            { new ushort[] { 0xEE30, 0xF000 } , new ExpectResult("invalid", "invalid FSR number") },
            { new ushort[] { 0xEE13, 0xF045 } , new ExpectResult("LFSR\tFSR1,0x0C45") },
            { new ushort[] { 0xEE26, 0xF078 } , new ExpectResult("LFSR\tFSR2,0x1878") },
            { new ushort[] { 0xEE20, 0xF300 } , new ExpectResult("LFSR\tFSR2,0x0300") },

        };

        private MachineInstruction RunTest(PICExecMode mode, params ushort[] words)
        {
            var bytes = new byte[words.Length * 2];
            LeImageWriter writer = new LeImageWriter(bytes);
            foreach (ushort word in words)
            {
                writer.WriteLeUInt16(word);
            }
            var image = new MemoryArea(Address.Ptr32(0x0100), bytes);

            var rdr = new LeImageReader(image, 0);
            var dasm = new PIC18Disassembler(arch, rdr) { ExecMode = mode };
            return dasm.First();
        }

        private string _fmtBinary(string mesg, params ushort[] words)
        {
            if (words.Length < 1) return $"{mesg}";
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            bool secnd = false;
            foreach (var w in words)
            {
                if (secnd) sb.Append("-");
                secnd = true;
                sb.AppendFormat("{0:X4}", w);
            }
            sb.Append("]: ");
            sb.Append(mesg);
            return sb.ToString();
        }

        private void _checkStandardIS(PICExecMode mode)
        {
            foreach (var e in _anyPIC_anyMode_Instrs)
            {
                var instr = RunTest(mode, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
            foreach (var e in _anyPIC_tradMode_Instrs)
            {
                var instr = RunTest(mode, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        private void _checkExtendedIS(PICExecMode mode)
        {
            foreach (var e in _anyPIC_anyMode_Instrs)
            {
                var instr = RunTest(mode, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
            foreach (var e in _anyPIC_extdMode_Instrs)
            {
                var instr = RunTest(mode, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18_TraditionalInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic);
            _checkStandardIS(PICExecMode.Traditional);
            foreach (var e in _pic18Only_Instrs)
            {
                var instr = RunTest(PICExecMode.Traditional, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18_AttemptExtendedInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic);
            _checkStandardIS(PICExecMode.Extended);
            foreach (var e in _pic18Only_Instrs)
            {
                var instr = RunTest(PICExecMode.Extended, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18Extd_TraditionalInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic);
            _checkStandardIS(PICExecMode.Traditional);
            foreach (var e in _egg_tradMode_Instrs)
            {
                var instr = RunTest(PICExecMode.Traditional, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18Extd_ExtendedInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic);
            _checkExtendedIS(PICExecMode.Extended);
            foreach (var e in _egg_extdMode_Instrs)
            {
                var instr = RunTest(PICExecMode.Extended, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18Enhd_TraditionalInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic);
            _checkStandardIS(PICExecMode.Traditional);
            foreach (var e in _enhanced_tradMode_Instrs)
            {
                var instr = RunTest(PICExecMode.Traditional, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

        [Test]
        public void Dis_PIC18Enhd_ExtendedInstructions()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic);
            _checkExtendedIS(PICExecMode.Extended);
            foreach (var e in _enhanced_extdMode_Instrs)
            {
                var instr = RunTest(PICExecMode.Extended, e.Key);
                Assert.AreEqual(e.Value.Instr, instr.ToString(), _fmtBinary(e.Value.Mesg, e.Key));
            }
        }

    }

}
