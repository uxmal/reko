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
        private readonly EndianImageReader rdr;
        private Instruction instr;
        private readonly List<Operand> ops;

        public Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.ops = new List<Operand>();
        }

        public override Instruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            ops.Clear();
            var decoder = decoders[op];
            var mutators = decoder.Mutators;
            var iclass = decoder.IClass;
            var opcode = decoder.Code;
            for (int i = 0; i < mutators.Length; ++i)
            {
                if (!mutators[i](op, this))
                {
                    iclass = InstrClass.Invalid;
                    opcode = Opcode.illegal;
                    break;
                }
            }

            this.instr = new Instruction
            {
                Code = opcode,
                InstructionClass = iclass,
                Operand = ops.Count > 0 ? ops[0] : null,
                Address = addr,
                Length = (int)(rdr.Address - addr),
            };
            return instr;
        }

        private static bool Imm(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = offset
            });
            return true;
        }

        private static bool a(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Accumulator,
                Register = Registers.a,
            });
            return true;
        }

        private static bool x(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.IndexedIndirect,
                Register = Registers.x,
                Offset = offset
            });
            return true;
        }

        private static bool z(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.ZeroPage,
                Register = null,
                Offset = offset
            });
            return true;
        }

        private static bool zx(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.ZeroPage,
                Register = Registers.x,
                Offset = offset
            });
            return true;
        }

        private static bool zy(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.ZeroPage,
                Register = Registers.x,
                Offset = offset
            });
            return true;
        }

        private static bool i(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Word16, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Indirect,
                Offset = offset
            });
            return true;
        }

        private static bool Ix(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.IndexedIndirect,
                Register = Registers.x,
                Offset = offset
            });
            return true;
        }

        private static bool Iy(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Byte, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.IndirectIndexed,
                Register = Registers.y,
                Offset = offset
            });
            return true;
        }

        private static bool A(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Word16, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Absolute,
                Register = null,
                Offset = offset
            });
            return true;
        }

        private static bool Ax(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Word16, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteX,
                Register = Registers.x,
                Offset = offset
            });
            return true;
        }

        private static bool Ay(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryRead(PrimitiveType.Word16, out Constant offset))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteY,
                Register = Registers.y,
                Offset = offset
            });
            return true;
        }

        private static bool j(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte bOff))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Ptr16)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Create(
                    PrimitiveType.Ptr16,
                    (dasm.rdr.Address.ToUInt16() + (sbyte) bOff)),
            });
            return true;
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
            public readonly InstrClass IClass;
            public readonly Opcode Code;
            public readonly Mutator<Disassembler> [] Mutators;

            public Decoder(Opcode code, params Mutator<Disassembler>[] mutators)
            {
                this.IClass = InstrClass.Linear;
                this.Code = code;
                this.Mutators = mutators;
            }

            public Decoder(InstrClass iclass, Opcode code, params Mutator<Disassembler>[] mutators)
            {
                this.IClass = iclass;
                this.Code = code;
                this.Mutators = mutators;
            }
        }

        private static readonly Decoder[] decoders = new Decoder[] {
            // 00
new Decoder(InstrClass.Padding|InstrClass.Zero, Opcode.brk),
    new Decoder(Opcode.ora, Ix),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ora, z),
    new Decoder(Opcode.asl, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.php),
    new Decoder(Opcode.ora, Imm),
    new Decoder(Opcode.asl, a),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ora, A),
    new Decoder(Opcode.asl, A),
    new Decoder(Opcode.illegal),

            // 10
new Decoder(InstrClass.ConditionalTransfer, Opcode.bpl, j),
    new Decoder(Opcode.ora, Iy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ora, zx),
    new Decoder(Opcode.asl, zx),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.clc),
    new Decoder(Opcode.ora, Ay),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ora, Ax),
    new Decoder(Opcode.asl, Ax),
    new Decoder(Opcode.illegal),
            // 20
new Decoder(InstrClass.Transfer|InstrClass.Call, Opcode.jsr, A),
    new Decoder(Opcode.and, Ix),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.bit, z),
    new Decoder(Opcode.and, z),
    new Decoder(Opcode.rol, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.plp),
    new Decoder(Opcode.and, Imm),
    new Decoder(Opcode.rol, a),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.bit, A),
    new Decoder(Opcode.and, A),
    new Decoder(Opcode.rol, A),
    new Decoder(Opcode.illegal),
        // 30
new Decoder(InstrClass.ConditionalTransfer, Opcode.bmi, j),
    new Decoder(Opcode.and, Iy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.and, zx),
    new Decoder(Opcode.rol, zx),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.sec),
    new Decoder(Opcode.and, Ay),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.and, Ax),
    new Decoder(Opcode.rol, Ax),
    new Decoder(Opcode.illegal),
 // 40
new Decoder(InstrClass.Transfer, Opcode.rti),
    new Decoder(Opcode.eor, Ix),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.eor, z),
    new Decoder(Opcode.lsr, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.pha),
    new Decoder(Opcode.eor, Imm),
    new Decoder(Opcode.lsr, a),
    new Decoder(Opcode.illegal),
    new Decoder(InstrClass.Transfer, Opcode.jmp, A),
    new Decoder(Opcode.eor, A),
    new Decoder(Opcode.lsr, A),
    new Decoder(Opcode.illegal),
 // 50
new Decoder(InstrClass.ConditionalTransfer, Opcode.bvc, j),
    new Decoder(Opcode.eor, Iy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.eor, zx),
    new Decoder(Opcode.lsr, zx),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.cli),
    new Decoder(Opcode.eor, Ay),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.eor, Ax),
    new Decoder(Opcode.lsr, Ax),
    new Decoder(Opcode.illegal),
 	// 60
new Decoder(InstrClass.Transfer, Opcode.rts),
    new Decoder(Opcode.adc, Ix),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.adc, z),
    new Decoder(Opcode.ror, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.pla),
    new Decoder(Opcode.adc, Imm),
    new Decoder(Opcode.ror, a),
    new Decoder(Opcode.illegal),
    new Decoder(InstrClass.Transfer, Opcode.jmp, i),	
    new Decoder(Opcode.adc, A),
    new Decoder(Opcode.ror, A),
    new Decoder(Opcode.illegal),
 	// 70
new Decoder(InstrClass.ConditionalTransfer, Opcode.bvs, j),
    new Decoder(Opcode.adc, Iy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.adc, zx),
    new Decoder(Opcode.ror, zx),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.sei),
    new Decoder(Opcode.adc, Ay),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.adc, Ax),
    new Decoder(Opcode.ror, Ax),
    new Decoder(Opcode.illegal),
 	// 80
new Decoder(Opcode.illegal),
    new Decoder(Opcode.sta, Ix),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.sty, z),
    new Decoder(Opcode.sta, z),
    new Decoder(Opcode.stx, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.dey),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.txa),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.sty, A),
    new Decoder(Opcode.sta, A),
    new Decoder(Opcode.stx, A),
    new Decoder(Opcode.illegal),
 	// 90
new Decoder(InstrClass.ConditionalTransfer, Opcode.bcc, j),
 	new Decoder(Opcode.sta, Iy),
 	new Decoder(Opcode.illegal),
 	new Decoder(Opcode.illegal),
 	new Decoder(Opcode.sty, zx),
 	new Decoder(Opcode.sta, zx),
 	new Decoder(Opcode.stx, zy),
 	new Decoder(Opcode.illegal),
 	new Decoder(Opcode.tya),
 	new Decoder(Opcode.sta, Ay),
 	new Decoder(Opcode.txs),
 	new Decoder(Opcode.illegal),
 	new Decoder(Opcode.illegal),
  	new Decoder(Opcode.sta, Ax),
 	new Decoder(Opcode.illegal),
 	new Decoder(Opcode.illegal),

// A0
new Decoder(Opcode.ldy, Imm),
    new Decoder(Opcode.lda, Ix),
    new Decoder(Opcode.ldx, Imm),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ldy, z),
    new Decoder(Opcode.lda, z),
    new Decoder(Opcode.ldx, z),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.tay),
    new Decoder(Opcode.lda, Imm),
    new Decoder(Opcode.tax),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ldy, A),
    new Decoder(Opcode.lda, A),
    new Decoder(Opcode.ldx, A),
    new Decoder(Opcode.illegal),
 	// B0
new Decoder(InstrClass.ConditionalTransfer, Opcode.bcs, j),
    new Decoder(Opcode.lda, Iy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ldy, zx),
    new Decoder(Opcode.lda, zx),
    new Decoder(Opcode.ldx, zy),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.clv),
    new Decoder(Opcode.lda, Ay),
    new Decoder(Opcode.tsx),
    new Decoder(Opcode.illegal),
    new Decoder(Opcode.ldy, Ax),
    new Decoder(Opcode.lda, Ax),
    new Decoder(Opcode.ldx, Ay),
    new Decoder(Opcode.illegal),
    
    // C0
    new Decoder(Opcode.cpy, Imm),
        new Decoder(Opcode.cmp, Ix),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cpy, z),
        new Decoder(Opcode.cmp, z),
        new Decoder(Opcode.dec, z),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.iny),
        new Decoder(Opcode.cmp, Imm),
        new Decoder(Opcode.dex),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cpy, A),
        new Decoder(Opcode.cmp, A),
        new Decoder(Opcode.dec, A),
        new Decoder(Opcode.illegal),
 
        // D0
        new Decoder(InstrClass.ConditionalTransfer, Opcode.bne, j),
        new Decoder(Opcode.cmp, Iy),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cmp, zx),
        new Decoder(Opcode.dec, zx),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cld),
        new Decoder(Opcode.cmp, Ay),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cmp, Ax),
        new Decoder(Opcode.dec, Ax),
        new Decoder(Opcode.illegal),

        // E0
        new Decoder(Opcode.cpx, Imm),
        new Decoder(Opcode.sbc, Ix),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cpx, z),
        new Decoder(Opcode.sbc, z),
        new Decoder(Opcode.inc, z),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.inx),
        new Decoder(Opcode.sbc, Imm),
        new Decoder(Opcode.nop),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.cpx, A),
        new Decoder(Opcode.sbc, A),
        new Decoder(Opcode.inc, A),
        new Decoder(Opcode.illegal),
 
        // F0
        new Decoder(InstrClass.ConditionalTransfer, Opcode.beq, j),
        new Decoder(Opcode.sbc, Iy),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.sbc, zx),
        new Decoder(Opcode.inc, zx),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.sed),
        new Decoder(Opcode.sbc, Ay),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.illegal),
        new Decoder(Opcode.sbc, Ax),
        new Decoder(Opcode.inc, Ax),
        new Decoder(Opcode.illegal),
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