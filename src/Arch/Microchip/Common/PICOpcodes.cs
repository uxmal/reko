#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// Values that represent symbolic opcodes of the 8-bit MCU PIC16/18 family.
    /// Note: Opcodes are defined in uppercase to conform to Microchip MPASM user's guide syntax.
    /// </summary>
    public enum Mnemonic : byte
    {

        /// <summary>Invalid instruction. (PIC16, PIC18) </summary>
        invalid,
        /// <summary>Unaligned instruction. (PIC16, PIC18) </summary>
        unaligned,
        ///<summary>Add literal with FSR. (PIC16E, PIC18E))</summary>
        ADDFSR,
        ///<summary>Add literal and WREG. (PIC16, PIC18)</summary>
        ADDLW,
        ///<summary>Add literal to FSR2 and return. (PIC18E)</summary>
        ADDULNK,
        ///<summary>Add WREG and f. (PIC16, PIC18)</summary>
        ADDWF,
        ///<summary>Add WREG and Carry to f. (PIC16E, PIC18)</summary>
        ADDWFC,
        ///<summary>AND literal with WREG. (PIC16, PIC18)</summary>
        ANDLW,
        ///<summary>AND WREG with f. (PIC16, PIC18)</summary>
        ANDWF,
        ///<summary>Arithmetic Right shift. (PIC16E)</summary>
        ASRF,
        ///<summary>Branch if carry. (PIC18)</summary>
        BC,
        ///<summary>Bit clear f. (PIC16, PIC18)</summary>
        BCF,
        ///<summary>Branch if negative. (PIC18)</summary>
        BN,
        ///<summary>Branch if not carry. (PIC18)</summary>
        BNC,
        ///<summary>Branch if not negative. (PIC18)</summary>
        BNN,
        ///<summary>Branch if not overflow. (PIC18)</summary>
        BNOV,
        ///<summary>Branch if not zero. (PIC18)</summary>
        BNZ,
        ///<summary>Branch if overflow. (PIC18)</summary>
        BOV,
        ///<summary>Branch unconditionally. (PIC16E, PIC18)</summary>
        BRA,
        ///<summary>Relative branch with W. (PIC16E)</summary>
        BRW,
        ///<summary>Bit set f. (PIC16, PIC18)</summary>
        BSF,
        ///<summary>Bit test f, skip if clear. (PIC16, PIC18)</summary>
        BTFSC,
        ///<summary>Bit test f, skip if set. (PIC16, PIC18)</summary>
        BTFSS,
        ///<summary>Bit toggle f. (PIC18)</summary>
        BTG,
        ///<summary>Branch if zero. (PIC18)</summary>
        BZ,
        ///<summary>Call subroutine. (PIC16, PIC18)</summary>
        CALL,
        ///<summary>Call subroutine using WREG. (PIC16E)</summary>
        CALLW,
        ///<summary>Clear f. (PIC16, PIC18)</summary>
        CLRF,
        ///<summary>Clear W. (PIC16)</summary>
        CLRW,
        ///<summary>Clear Watchdog Timer. (PIC16, PIC18)</summary>
        CLRWDT,
        ///<summary>Complement f. (PIC16, PIC18)</summary>
        COMF,
        ///<summary>Compare f with WREG, skip =. (PIC18)</summary>
        CPFSEQ,
        ///<summary>Compare f with WREG, skip >. (PIC18)</summary>
        CPFSGT,
        ///<summary>Compare f with WREG,  skip &lt;. (PIC18)</summary>
        CPFSLT,
        ///<summary>Decimal adjust W register. (PIC18)</summary>
        DAW,
        ///<summary>Decrement f, skip if not 0. (PIC18)</summary>
        DCFSNZ,
        ///<summary>Decrement f. (PIC16, PIC18)</summary>
        DECF,
        ///<summary>Decrement f,  skip if 0. (PIC16, PIC18)</summary>
        DECFSZ,
        ///<summary>Go to address.</summary>
        GOTO,
        ///<summary>Increment f. (PIC16, PIC18)</summary>
        INCF,
        ///<summary>Increment f,  skip if 0. (PIC16, PIC18)</summary>
        INCFSZ,
        ///<summary>Increment f, skip if not 0. (PIC18)</summary>
        INFSNZ,
        ///<summary>Inclusive OR literal with WREG. (PIC16, PIC18)</summary>
        IORLW,
        ///<summary>Inclusive OR WREG with f. (PIC16, PIC18)</summary>
        IORWF,
        ///<summary>Move literal to FSRn. (PIC16E, PIC18)</summary>
        LFSR,
        ///<summary>Logical Left Shift. (PIC16E)</summary>
        LSLF,
        ///<summary>Logical Right Shift. (PIC16E)</summary>
        LSRF,
        ///<summary>Move f. (PIC16, PIC18)</summary>
        MOVF,
        ///<summary>Move source to destination. (PIC18)</summary>
        MOVFF,
        ///<summary>Move source to destination. (PIC18E+)</summary>
        MOVFFL,
        ///<summary>Move INDFn to W - or - Move k[FSRn] to W. (PIC16E)</summary>
        MOVIW,
        ///<summary>Move literal to low nibble in BSR. (PIC16E)</summary>
        MOVLB,
        ///<summary>Move literal to PCLATH. (PIC16E)</summary>
        MOVLP,
        ///<summary>Move literal to WREG. (PIC16, PIC18)</summary>
        MOVLW,
        ///<summary>Move indexed to f. (PIC18E)</summary>
        MOVSF,
        ///<summary>Move indexed to f. (PIC18E+).</summary>
        MOVSFL,
        ///<summary>Move indexed to indexed. (PIC18E)</summary>
        MOVSS,
        ///<summary>Move WREG to f. (PIC18)</summary>
        MOVWF,
        ///<summary>Move W to INDFn -or - Move W to k[FSRn]. (PIC16E)</summary>
        MOVWI,
        ///<summary>Multiply literal with WREG. (PIC18)</summary>
        MULLW,
        ///<summary>Multiply WREG with f. (PIC18)</summary>
        MULWF,
        ///<summary>Negate f. (PIC16, PIC18)</summary>
        NEGF,
        ///<summary>No operation. (PIC16, PIC18)</summary>
        NOP,
        ///<summary>Pop top of return stack. (PIC18)</summary>
        POP,
        ///<summary>Push top of return stack. (PIC18)</summary>
        PUSH,
        ///<summary>Push literal at FSR2, decrement FSR2. (PIC18E)</summary>
        PUSHL,
        ///<summary>Relative call. (PIC18)</summary>
        RCALL,
        ///<summary>Reset. (PIC16E, PIC18)</summary>
        RESET,
        ///<summary>Return from interrupt. (PIC16, PIC18)</summary>
        RETFIE,
        ///<summary>Return with literal in WREG. (PIC16, PIC18)</summary>
        RETLW,
        ///<summary>Return from subroutine. (PIC16, PIC18)</summary>
        RETURN,
        ///<summary>Rotate left f through Carry. (PIC18)</summary>
        RLCF,
        ///<summary>Rotate left f through Carry. (PIC16)</summary>
        RLF,
        ///<summary>Rotate left f (No Carry). (PIC18)</summary>
        RLNCF,
        ///<summary>Rotate right f through Carry. (PIC18)</summary>
        RRCF,
        ///<summary>Rotate right f through Carry. (PIC16)</summary>
        RRF,
        ///<summary>Rotate right f (No Carry). (PIC18)</summary>
        RRNCF,
        ///<summary>Set f. (PIC18)</summary>
        SETF,
        ///<summary>Enter sleep mode. (PIC16, PIC18)</summary>
        SLEEP,
        ///<summary>Subtracts literal from FSR. (PIC18E)</summary>
        SUBFSR,
        ///<summary>Subtracts f from WREG with Borrow. (PIC18)</summary>
        SUBFWB,
        ///<summary>Subtracts WREG from literal. (PIC16, PIC18)</summary>
        SUBLW,
        ///<summary>Subtracts literal from FSR2 and return. (PIC18E)</summary>
        SUBULNK,
        ///<summary>Subtracts WREG from f. (PIC16, PIC18)</summary>
        SUBWF,
        ///<summary>Subtracts WREG from f with Borrow. (PIC16E, PIC18)</summary>
        SUBWFB,
        ///<summary>Swap nibbles in f. (PIC16, PIC18)</summary>
        SWAPF,
        ///<summary>Table read. (PIC18)</summary>
        TBLRD,
        ///<summary>Table write. (PIC18)</summary>
        TBLWT,
        ///<summary>Load TRIS register with W. (PIC16+)</summary>
        TRIS,
        ///<summary>Test f,  skip if 0. (PIC18)</summary>
        TSTFSZ,
        ///<summary>Exclusive OR literal with WREG. (PIC16, PIC18)</summary>
        XORLW,
        ///<summary>Exclusive OR WREG with f. (PIC16, PIC18)</summary>
        XORWF,

        // Pseudo-instructions

        /// <summary>Configuration bits pseudo-instruction. (PIC16)</summary>
        __CONFIG,
        /// <summary>Configuration bits pseudo-instruction. (PIC18)</summary>
        CONFIG,
        /// <summary>Store string in program memory pseudo-instruction. (PIC16, PIC18)</summary>
        DA,
        /// <summary>Store data byte in program memory pseudo-instruction. (PIC16, PIC18)</summary>
        DB,
        /// <summary>Store data byte in EEPROM memory pseudo-instruction. (PIC16, PIC18)</summary>
        DE,
        /// <summary>Define table in program memory pseudo-instruction. (PIC16)</summary>
        DT,
        /// <summary>Define table in program memory pseudo-instruction. (PIC16E)</summary>
        DTM,
        /// <summary>Store data word in program memory pseudo-instruction. (PIC16, PIC18)</summary>
        DW,
        /// <summary>Store word in ID locations memory pseudo-instruction. (PIC16, PIC18)</summary>
        __IDLOCS
    }

}

