#region License
/* 
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    // http://www.e-tradition.net/bytes/6502/6502_instruction_set.html
    // 65816 = http://www.zophar.net/fileuploads/2/10538ivwiu/65816info.txt
    public class Disassembler : DisassemblerBase<Instruction>
    {
        private EndianImageReader rdr;
        private Instruction instr;

        public Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            var op = rdr.ReadByte();
            var opRec = opRecs[op];
            var fmt = opRec.Format;
            Operand operand = null;
            for (int i = 0; i < fmt.Length; ++i)
            {
                operand = null;
                switch (fmt[i++])
                {
                case '#':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.Immediate,
                        Offset = rdr.Read(PrimitiveType.Byte)
                    };
                    break;
                case 'a':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.Accumulator,
                        Register = Registers.a,
                    };
                    break;
                case 'x':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.IndexedIndirect,
                        Register = Registers.x,
                        Offset = rdr.Read(PrimitiveType.Byte)
                    };
                    break;
                case 'z':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.ZeroPage,
                        Register = null,
                        Offset = rdr.Read(PrimitiveType.Byte)
                    };
                    if (i < fmt.Length)
                    {
                        if (fmt[i] == 'x')
                            operand.Register = Registers.x;
                        if (fmt[i] == 'y')
                            operand.Register = Registers.y;
                        ++i;
                    }
                    break;
                case 'i':
                    operand = Indirect();
                    break;
                case 'I':
                    switch (fmt[i++])
                    {
                    case 'x':
                        operand = new Operand(PrimitiveType.Byte)
                        {
                            Mode = AddressMode.IndexedIndirect,
                            Register = Registers.x,
                            Offset = rdr.Read(PrimitiveType.Byte)
                        };
                        break;

                    case 'y':
                        operand = new Operand(PrimitiveType.Byte)
                        {
                            Mode = AddressMode.IndirectIndexed,
                            Register = Registers.y,
                            Offset = rdr.Read(PrimitiveType.Byte)
                        };
                        break;
                    }
                    break;
                case 'A':
                    operand = AbsoluteOperand(fmt, ref i);
                    break;
                case 'j':
                    short offset = rdr.ReadSByte();
                    operand = new Operand(PrimitiveType.Ptr16)
                    {
                        Mode = AddressMode.Immediate,
                        Offset = Constant.Create(
                            PrimitiveType.Ptr16,
                            (rdr.Address.ToUInt16() + offset)),
                    };
                    break;
                default: throw new NotImplementedException(string.Format("Unknown format character {0}.", fmt[i - 1]));
                }
            }
            this.instr = new Instruction
            {
                Code = opRec.Code,
                Operand = operand,
                Address = addr,
                Length = (int)(rdr.Address - addr),
            };
            return instr;
        }

        private Operand Indirect()
        {
            return new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Indirect,
                Offset = rdr.Read(PrimitiveType.Word16)
            };
        }

        private Operand AbsoluteOperand(string fmt, ref int i)
        {
            if (i < fmt.Length)
            {
                if (fmt[i] == 'x')
                {
                    ++i;
                    return new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.AbsoluteX,
                        Register = Registers.x,
                        Offset = rdr.Read(PrimitiveType.Word16)
                    };
                }
                else if (fmt[i] == 'y')
                {
                    ++i;
                    return new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.AbsoluteY,
                        Register = Registers.y,
                        Offset = rdr.Read(PrimitiveType.Word16)
                    };
                }
            }
            return new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Absolute,
                Register = null,
                Offset = rdr.Read(PrimitiveType.Word16)
            };
        }

        private class OpRec
        {
            public Opcode Code;
            public string Format;

            public OpRec(Opcode code, string fmt)
            {
                this.Code = code;
                this.Format = fmt;
            }
        }

        private static OpRec[] opRecs = new OpRec[] {
            // 00
new OpRec(Opcode.brk, ""),
    new OpRec(Opcode.ora, "Ix"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ora, "z"),
    new OpRec(Opcode.asl, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.php, ""),
    new OpRec(Opcode.ora, "#"),
    new OpRec(Opcode.asl, "a"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ora, "A"),
    new OpRec(Opcode.asl, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bpl, "j"),
    new OpRec(Opcode.ora, "Iy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ora, "zx"),
    new OpRec(Opcode.asl, "zx"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.clc, ""),
    new OpRec(Opcode.ora, "Ay"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ora, "Ax"),
    new OpRec(Opcode.asl, "Ax"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.jsr, "A"),
    new OpRec(Opcode.and, "Ix"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.bit, "z"),
    new OpRec(Opcode.and, "z"),
    new OpRec(Opcode.rol, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.plp, ""),
    new OpRec(Opcode.and, "#"),
    new OpRec(Opcode.rol, "a"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.bit, "A"),
    new OpRec(Opcode.and, "A"),
    new OpRec(Opcode.rol, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bmi, "j"),
    new OpRec(Opcode.and, "Iy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.and, "zx"),
    new OpRec(Opcode.rol, "zx"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.sec, ""),
    new OpRec(Opcode.and, "Ay"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.and, "Ax"),
    new OpRec(Opcode.rol, "Ax"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.rti, ""),
    new OpRec(Opcode.eor, "Ix"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.eor, "z"),
    new OpRec(Opcode.lsr, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.pha, ""),
    new OpRec(Opcode.eor, "#"),
    new OpRec(Opcode.lsr, "a"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.jmp, "A"),
    new OpRec(Opcode.eor, "A"),
    new OpRec(Opcode.lsr, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bvc, "j"),
    new OpRec(Opcode.eor, "Iy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.eor, "zx"),
    new OpRec(Opcode.lsr, "zx"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.cli, ""),
    new OpRec(Opcode.eor, "Ay"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.eor, "Ax"),
    new OpRec(Opcode.lsr, "Ax"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.rts, ""),
    new OpRec(Opcode.adc, "Ix"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.adc, "z"),
    new OpRec(Opcode.ror, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.pla, ""),
    new OpRec(Opcode.adc, "#"),
    new OpRec(Opcode.ror, "a"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.jmp, "I"),	
    new OpRec(Opcode.adc, "A"),
    new OpRec(Opcode.ror, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bvs, "j"),
    new OpRec(Opcode.adc, "Iy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.adc, "zx"),
    new OpRec(Opcode.ror, "zx"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.sei, ""),
    new OpRec(Opcode.adc, "Ay"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.adc, "Ax"),
    new OpRec(Opcode.ror, "Ax"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.sta, "Ix"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.sty, "z"),
    new OpRec(Opcode.sta, "z"),
    new OpRec(Opcode.stx, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.dey, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.txa, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.sty, "A"),
    new OpRec(Opcode.sta, "A"),
    new OpRec(Opcode.stx, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bcc, "j"),
 	new OpRec(Opcode.sta, "Iy"),
 	new OpRec(Opcode.illegal, ""),
 	new OpRec(Opcode.illegal, ""),
 	new OpRec(Opcode.sty, "zx"),
 	new OpRec(Opcode.sta, "zx"),
 	new OpRec(Opcode.stx, "zy"),
 	new OpRec(Opcode.illegal, ""),
 	new OpRec(Opcode.tya, ""),
 	new OpRec(Opcode.sta, "Ay"),
 	new OpRec(Opcode.txs, ""),
 	new OpRec(Opcode.illegal, ""),
 	new OpRec(Opcode.illegal, ""),
  	new OpRec(Opcode.sta, "Ax"),
 	new OpRec(Opcode.illegal, ""),
 	new OpRec(Opcode.illegal, ""),
 
// A0
new OpRec(Opcode.ldy, "#"),
    new OpRec(Opcode.lda, "Ix"),
    new OpRec(Opcode.ldx, "#"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ldy, "z"),
    new OpRec(Opcode.lda, "z"),
    new OpRec(Opcode.ldx, "z"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.tay, ""),
    new OpRec(Opcode.lda, "#"),
    new OpRec(Opcode.tax, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ldy, "A"),
    new OpRec(Opcode.lda, "A"),
    new OpRec(Opcode.ldx, "A"),
    new OpRec(Opcode.illegal, ""),
 
new OpRec(Opcode.bcs, "j"),
    new OpRec(Opcode.lda, "Iy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ldy, "zx"),
    new OpRec(Opcode.lda, "zx"),
    new OpRec(Opcode.ldx, "zy"),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.clv, ""),
    new OpRec(Opcode.lda, "Ay"),
    new OpRec(Opcode.tsx, ""),
    new OpRec(Opcode.illegal, ""),
    new OpRec(Opcode.ldy, "Ax"),
    new OpRec(Opcode.lda, "Ax"),
    new OpRec(Opcode.ldx, "Ay"),
    new OpRec(Opcode.illegal, ""),
 
        // C0
    new OpRec(Opcode.cpy, "#"),
        new OpRec(Opcode.cmp, "Ix"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cpy, "z"),
        new OpRec(Opcode.cmp, "z"),
        new OpRec(Opcode.dec, "z"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.iny, ""),
        new OpRec(Opcode.cmp, "#"),
        new OpRec(Opcode.dex, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cpy, "A"),
        new OpRec(Opcode.cmp, "A"),
        new OpRec(Opcode.dec, "A"),
        new OpRec(Opcode.illegal, ""),
 
        // D0
        new OpRec(Opcode.bne, "j"),
        new OpRec(Opcode.cmp, "Iy"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cmp, "zx"),
        new OpRec(Opcode.dec, "zx"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cld, ""),
        new OpRec(Opcode.cmp, "Ay"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cmp, "Ax"),
        new OpRec(Opcode.dec, "Ax"),
        new OpRec(Opcode.illegal, ""),

        // E0
        new OpRec(Opcode.cpx, "#"),
        new OpRec(Opcode.sbc, "Ix"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cpx, "z"),
        new OpRec(Opcode.sbc, "z"),
        new OpRec(Opcode.inc, "z"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.inx, ""),
        new OpRec(Opcode.sbc, "#"),
        new OpRec(Opcode.nop, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.cpx, "A"),
        new OpRec(Opcode.sbc, "A"),
        new OpRec(Opcode.inc, "A"),
        new OpRec(Opcode.illegal, ""),
 
        // F0
        new OpRec(Opcode.beq, "j"),
        new OpRec(Opcode.sbc, "Iy"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.sbc, "zx"),
        new OpRec(Opcode.inc, "zx"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.sed, ""),
        new OpRec(Opcode.sbc, "Ay"),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.illegal, ""),
        new OpRec(Opcode.sbc, "Ax"),
        new OpRec(Opcode.inc, "Ax"),
        new OpRec(Opcode.illegal, ""),
        };
    }
}

    /*
imm = #$00
zp = $00
zpx = $00,X
zpy = $00,Y
izx = ($00,X)
izy = ($00),Y
abs = $0000
abx = $0000,X
aby = $0000,Y
ind = ($0000)
rel = $0000 (PC-relative) 
     */

/*

 *
00	BRK impl	ORA X,ind	??? ---	??? ---	??? --- 	ORA zpg	    ASL zpg	    ??? ---	PHP impl	ORA #	    ASL A	    ??? ---	??? ---	    ORA abs	    ASL abs	??? ---
10	BPL rel   	ORA ind,Y	??? ---	??? ---	??? --- 	ORA zpg,X	ASL zpg,X	??? ---	CLC impl	ORA abs,Y	??? ---	    ??? ---	??? ---	    ORA abs,X	ASL abs,X	??? ---
20	JSR abs	    AND X,ind	??? ---	??? ---	BIT zpg 	AND zpg	    ROL zpg	    ??? ---	PLP impl	AND #	    ROL A	    ??? ---	BIT abs	    AND abs	    ROL abs	??? ---
30	BMI rel	    AND ind,Y	??? ---	??? ---	??? --- 	AND zpg,X	ROL zpg,X	??? ---	SEC impl	AND abs,Y	??? ---	    ??? ---	??? ---	    AND abs,X	ROL abs,X	??? ---
40	RTI impl	EOR X,ind	??? ---	??? ---	??? --- 	EOR zpg	    LSR zpg	    ??? ---	PHA impl	EOR #	    LSR A	    ??? ---	JMP abs	    EOR abs	    LSR abs	??? ---
50	BVC rel	    EOR ind,Y	??? ---	??? ---	??? --- 	EOR zpg,X	LSR zpg,X	??? ---	CLI impl	EOR abs,Y	??? ---	    ??? ---	??? ---	    EOR abs,X	LSR abs,X	??? ---
60	RTS impl	ADC X,ind	??? ---	??? ---	??? --- 	ADC zpg	    ROR zpg	    ??? ---	PLA impl	ADC #	    ROR A	    ??? ---	JMP ind	    ADC abs	    ROR abs	??? ---
70	BVS rel	    ADC ind,Y	??? ---	??? ---	??? --- 	ADC zpg,X	ROR zpg,X	??? ---	SEI impl	ADC abs,Y	??? ---	    ??? ---	??? ---	    ADC abs,X	ROR abs,X	??? ---
80	??? --- 	STA X,ind	??? ---	??? ---	STY zpg	    STA zpg	    STX zpg	    ??? ---	DEY impl	??? --- 	TXA impl	??? ---	STY abs	    STA abs 	STX abs	??? ---
90	BCC rel	    STA ind,Y	??? ---	??? ---	STY zpg,X	STA zpg,X	STX zpg,Y	??? ---	TYA impl	STA abs,Y	TXS impl	??? ---	??? --- 	STA abs,X	??? ---	??? ---
A0	LDY #	    LDA X,ind	LDX #	??? ---	LDY zpg 	LDA zpg	    LDX zpg 	??? ---	TAY impl	LDA #	    TAX impl	??? ---	LDY abs	    LDA abs 	LDX abs	??? ---
B0	BCS rel	    LDA ind,Y	??? ---	??? ---	LDY zpg,X	LDA zpg,X	LDX zpg,Y	??? ---	CLV impl	LDA abs,Y	TSX impl	??? ---	LDY abs,X	LDA abs,X	LDX abs,Y	??? ---
C0	CPY #	    CMP X,ind	??? ---	??? ---	CPY zpg 	CMP zpg 	DEC zpg	    ??? ---	INY impl	CMP #	    DEX impl	??? ---	CPY abs 	CMP abs 	DEC abs	??? ---
D0	BNE rel	    CMP ind,Y	??? ---	??? ---	??? --- 	CMP zpg,X	DEC zpg,X	??? ---	CLD impl	CMP abs,Y	??? ---	    ??? ---	??? ---	    CMP abs,X	DEC abs,X	??? ---
E0	CPX #	    SBC X,ind	??? ---	??? ---	CPX zpg 	SBC zpg	    INC zpg	    ??? ---	INX impl	SBC #	    NOP impl	??? ---	CPX abs	    SBC abs 	INC abs	??? ---
F0	BEQ rel	    SBC ind,Y	??? ---	??? ---	??? --- 	SBC zpg,X	INC zpg,X	??? ---	SED impl	SBC abs,Y	??? ---	    ??? ---	??? ---	    SBC abs,X	INC abs,X	??? ---
*/