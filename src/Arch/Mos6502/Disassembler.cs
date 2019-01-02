#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
            var opRec = decoders[op];
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
                IClass = opRec.IClass,
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

        private class Decoder
        {
            public Opcode Code;
            public InstrClass IClass;
            public string Format;

            public Decoder(Opcode code, string fmt, InstrClass iclass = InstrClass.Linear)
            {
                this.Code = code;
                this.IClass = iclass;
                this.Format = fmt;
            }
        }

        private static Decoder[] decoders = new Decoder[] {
            // 00
new Decoder(Opcode.brk, "", InstrClass.Padding|InstrClass.Zero),
    new Decoder(Opcode.ora, "Ix"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ora, "z"),
    new Decoder(Opcode.asl, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.php, ""),
    new Decoder(Opcode.ora, "#"),
    new Decoder(Opcode.asl, "a"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ora, "A"),
    new Decoder(Opcode.asl, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bpl, "j", InstrClass.ConditionalTransfer),
    new Decoder(Opcode.ora, "Iy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ora, "zx"),
    new Decoder(Opcode.asl, "zx"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.clc, ""),
    new Decoder(Opcode.ora, "Ay"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ora, "Ax"),
    new Decoder(Opcode.asl, "Ax"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.jsr, "A", InstrClass.Transfer|InstrClass.Call),
    new Decoder(Opcode.and, "Ix"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.bit, "z"),
    new Decoder(Opcode.and, "z"),
    new Decoder(Opcode.rol, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.plp, ""),
    new Decoder(Opcode.and, "#"),
    new Decoder(Opcode.rol, "a"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.bit, "A"),
    new Decoder(Opcode.and, "A"),
    new Decoder(Opcode.rol, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bmi, "j", InstrClass.ConditionalTransfer),
    new Decoder(Opcode.and, "Iy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.and, "zx"),
    new Decoder(Opcode.rol, "zx"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.sec, ""),
    new Decoder(Opcode.and, "Ay"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.and, "Ax"),
    new Decoder(Opcode.rol, "Ax"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.rti, "", InstrClass.Transfer),
    new Decoder(Opcode.eor, "Ix"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.eor, "z"),
    new Decoder(Opcode.lsr, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.pha, ""),
    new Decoder(Opcode.eor, "#"),
    new Decoder(Opcode.lsr, "a"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.jmp, "A", InstrClass.Transfer),
    new Decoder(Opcode.eor, "A"),
    new Decoder(Opcode.lsr, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bvc, "j", InstrClass.ConditionalTransfer),
    new Decoder(Opcode.eor, "Iy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.eor, "zx"),
    new Decoder(Opcode.lsr, "zx"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.cli, ""),
    new Decoder(Opcode.eor, "Ay"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.eor, "Ax"),
    new Decoder(Opcode.lsr, "Ax"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.rts, "", InstrClass.Transfer),
    new Decoder(Opcode.adc, "Ix"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.adc, "z"),
    new Decoder(Opcode.ror, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.pla, ""),
    new Decoder(Opcode.adc, "#"),
    new Decoder(Opcode.ror, "a"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.jmp, "I", InstrClass.Transfer),	
    new Decoder(Opcode.adc, "A"),
    new Decoder(Opcode.ror, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bvs, "j", InstrClass.ConditionalTransfer),
    new Decoder(Opcode.adc, "Iy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.adc, "zx"),
    new Decoder(Opcode.ror, "zx"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.sei, ""),
    new Decoder(Opcode.adc, "Ay"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.adc, "Ax"),
    new Decoder(Opcode.ror, "Ax"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.sta, "Ix"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.sty, "z"),
    new Decoder(Opcode.sta, "z"),
    new Decoder(Opcode.stx, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.dey, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.txa, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.sty, "A"),
    new Decoder(Opcode.sta, "A"),
    new Decoder(Opcode.stx, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bcc, "j", InstrClass.ConditionalTransfer),
 	new Decoder(Opcode.sta, "Iy"),
 	new Decoder(Opcode.illegal, ""),
 	new Decoder(Opcode.illegal, ""),
 	new Decoder(Opcode.sty, "zx"),
 	new Decoder(Opcode.sta, "zx"),
 	new Decoder(Opcode.stx, "zy"),
 	new Decoder(Opcode.illegal, ""),
 	new Decoder(Opcode.tya, ""),
 	new Decoder(Opcode.sta, "Ay"),
 	new Decoder(Opcode.txs, ""),
 	new Decoder(Opcode.illegal, ""),
 	new Decoder(Opcode.illegal, ""),
  	new Decoder(Opcode.sta, "Ax"),
 	new Decoder(Opcode.illegal, ""),
 	new Decoder(Opcode.illegal, ""),
 
// A0
new Decoder(Opcode.ldy, "#"),
    new Decoder(Opcode.lda, "Ix"),
    new Decoder(Opcode.ldx, "#"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ldy, "z"),
    new Decoder(Opcode.lda, "z"),
    new Decoder(Opcode.ldx, "z"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.tay, ""),
    new Decoder(Opcode.lda, "#"),
    new Decoder(Opcode.tax, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ldy, "A"),
    new Decoder(Opcode.lda, "A"),
    new Decoder(Opcode.ldx, "A"),
    new Decoder(Opcode.illegal, ""),
 
new Decoder(Opcode.bcs, "j", InstrClass.ConditionalTransfer),
    new Decoder(Opcode.lda, "Iy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ldy, "zx"),
    new Decoder(Opcode.lda, "zx"),
    new Decoder(Opcode.ldx, "zy"),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.clv, ""),
    new Decoder(Opcode.lda, "Ay"),
    new Decoder(Opcode.tsx, ""),
    new Decoder(Opcode.illegal, ""),
    new Decoder(Opcode.ldy, "Ax"),
    new Decoder(Opcode.lda, "Ax"),
    new Decoder(Opcode.ldx, "Ay"),
    new Decoder(Opcode.illegal, ""),
 
        // C0
    new Decoder(Opcode.cpy, "#"),
        new Decoder(Opcode.cmp, "Ix"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cpy, "z"),
        new Decoder(Opcode.cmp, "z"),
        new Decoder(Opcode.dec, "z"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.iny, ""),
        new Decoder(Opcode.cmp, "#"),
        new Decoder(Opcode.dex, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cpy, "A"),
        new Decoder(Opcode.cmp, "A"),
        new Decoder(Opcode.dec, "A"),
        new Decoder(Opcode.illegal, ""),
 
        // D0
        new Decoder(Opcode.bne, "j", InstrClass.ConditionalTransfer),
        new Decoder(Opcode.cmp, "Iy"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cmp, "zx"),
        new Decoder(Opcode.dec, "zx"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cld, ""),
        new Decoder(Opcode.cmp, "Ay"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cmp, "Ax"),
        new Decoder(Opcode.dec, "Ax"),
        new Decoder(Opcode.illegal, ""),

        // E0
        new Decoder(Opcode.cpx, "#"),
        new Decoder(Opcode.sbc, "Ix"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cpx, "z"),
        new Decoder(Opcode.sbc, "z"),
        new Decoder(Opcode.inc, "z"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.inx, ""),
        new Decoder(Opcode.sbc, "#"),
        new Decoder(Opcode.nop, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.cpx, "A"),
        new Decoder(Opcode.sbc, "A"),
        new Decoder(Opcode.inc, "A"),
        new Decoder(Opcode.illegal, ""),
 
        // F0
        new Decoder(Opcode.beq, "j", InstrClass.ConditionalTransfer),
        new Decoder(Opcode.sbc, "Iy"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.sbc, "zx"),
        new Decoder(Opcode.inc, "zx"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.sed, ""),
        new Decoder(Opcode.sbc, "Ay"),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.illegal, ""),
        new Decoder(Opcode.sbc, "Ax"),
        new Decoder(Opcode.inc, "Ax"),
        new Decoder(Opcode.illegal, ""),
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