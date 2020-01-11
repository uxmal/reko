#region License
/* 
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
    using Decoder = Decoder<Disassembler, Mnemonic, Instruction>;

    // http://www.e-tradition.net/bytes/6502/6502_instruction_set.html
    // 65816 = http://www.zophar.net/fileuploads/2/10538ivwiu/65816info.txt
    public class Disassembler : DisassemblerBase<Instruction, Mnemonic>
    {
        private readonly EndianImageReader rdr;
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
            var instr = decoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray(),
            };
            return instr;
        }

        public override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands
            };
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
                Mode = AddressMode.ZeroPageX,
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
                Mode = AddressMode.ZeroPageY,
                Register = Registers.y,
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

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Disassembler> [] mutators)
        {
            return new InstrDecoder<Disassembler, Mnemonic, Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(InstrClass iclass, Mnemonic mnemonic, params Mutator<Disassembler>[] mutators)
        {
            return new InstrDecoder<Disassembler, Mnemonic, Instruction>(iclass, mnemonic, mutators);
        }

        private static readonly Decoder invalid = Instr(InstrClass.Invalid, Mnemonic.illegal);

        private static readonly Decoder[] decoders = new Decoder[] {
            // 00
Instr(InstrClass.Padding|InstrClass.Zero, Mnemonic.brk),
    Instr(Mnemonic.ora, Ix),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.ora, z),
    Instr(Mnemonic.asl, z),
    invalid,
    Instr(Mnemonic.php),
    Instr(Mnemonic.ora, Imm),
    Instr(Mnemonic.asl, a),
    invalid,
    invalid,
    Instr(Mnemonic.ora, A),
    Instr(Mnemonic.asl, A),
    invalid,

            // 10
Instr(InstrClass.ConditionalTransfer, Mnemonic.bpl, j),
    Instr(Mnemonic.ora, Iy),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.ora, zx),
    Instr(Mnemonic.asl, zx),
    invalid,
    Instr(Mnemonic.clc),
    Instr(Mnemonic.ora, Ay),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.ora, Ax),
    Instr(Mnemonic.asl, Ax),
    invalid,
            // 20
Instr(InstrClass.Transfer|InstrClass.Call, Mnemonic.jsr, A),
    Instr(Mnemonic.and, Ix),
    invalid,
    invalid,
    Instr(Mnemonic.bit, z),
    Instr(Mnemonic.and, z),
    Instr(Mnemonic.rol, z),
    invalid,
    Instr(Mnemonic.plp),
    Instr(Mnemonic.and, Imm),
    Instr(Mnemonic.rol, a),
    invalid,
    Instr(Mnemonic.bit, A),
    Instr(Mnemonic.and, A),
    Instr(Mnemonic.rol, A),
    invalid,
        // 30
Instr(InstrClass.ConditionalTransfer, Mnemonic.bmi, j),
    Instr(Mnemonic.and, Iy),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.and, zx),
    Instr(Mnemonic.rol, zx),
    invalid,
    Instr(Mnemonic.sec),
    Instr(Mnemonic.and, Ay),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.and, Ax),
    Instr(Mnemonic.rol, Ax),
    invalid,
 // 40
Instr(InstrClass.Transfer, Mnemonic.rti),
    Instr(Mnemonic.eor, Ix),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.eor, z),
    Instr(Mnemonic.lsr, z),
    invalid,
    Instr(Mnemonic.pha),
    Instr(Mnemonic.eor, Imm),
    Instr(Mnemonic.lsr, a),
    invalid,
    Instr(InstrClass.Transfer, Mnemonic.jmp, A),
    Instr(Mnemonic.eor, A),
    Instr(Mnemonic.lsr, A),
    invalid,
 // 50
Instr(InstrClass.ConditionalTransfer, Mnemonic.bvc, j),
    Instr(Mnemonic.eor, Iy),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.eor, zx),
    Instr(Mnemonic.lsr, zx),
    invalid,
    Instr(Mnemonic.cli),
    Instr(Mnemonic.eor, Ay),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.eor, Ax),
    Instr(Mnemonic.lsr, Ax),
    invalid,
 	// 60
Instr(InstrClass.Transfer, Mnemonic.rts),
    Instr(Mnemonic.adc, Ix),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.adc, z),
    Instr(Mnemonic.ror, z),
    invalid,
    Instr(Mnemonic.pla),
    Instr(Mnemonic.adc, Imm),
    Instr(Mnemonic.ror, a),
    invalid,
    Instr(InstrClass.Transfer, Mnemonic.jmp, i),	
    Instr(Mnemonic.adc, A),
    Instr(Mnemonic.ror, A),
    invalid,
 	// 70
Instr(InstrClass.ConditionalTransfer, Mnemonic.bvs, j),
    Instr(Mnemonic.adc, Iy),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.adc, zx),
    Instr(Mnemonic.ror, zx),
    invalid,
    Instr(Mnemonic.sei),
    Instr(Mnemonic.adc, Ay),
    invalid,
    invalid,
    invalid,
    Instr(Mnemonic.adc, Ax),
    Instr(Mnemonic.ror, Ax),
    invalid,
 	// 80
invalid,
    Instr(Mnemonic.sta, Ix),
    invalid,
    invalid,
    Instr(Mnemonic.sty, z),
    Instr(Mnemonic.sta, z),
    Instr(Mnemonic.stx, z),
    invalid,
    Instr(Mnemonic.dey),
    invalid,
    Instr(Mnemonic.txa),
    invalid,
    Instr(Mnemonic.sty, A),
    Instr(Mnemonic.sta, A),
    Instr(Mnemonic.stx, A),
    invalid,
 	// 90
Instr(InstrClass.ConditionalTransfer, Mnemonic.bcc, j),
 	Instr(Mnemonic.sta, Iy),
 	invalid,
 	invalid,
 	Instr(Mnemonic.sty, zx),
 	Instr(Mnemonic.sta, zx),
 	Instr(Mnemonic.stx, zy),
 	invalid,
 	Instr(Mnemonic.tya),
 	Instr(Mnemonic.sta, Ay),
 	Instr(Mnemonic.txs),
 	invalid,
 	invalid,
  	Instr(Mnemonic.sta, Ax),
 	invalid,
 	invalid,

// A0
Instr(Mnemonic.ldy, Imm),
    Instr(Mnemonic.lda, Ix),
    Instr(Mnemonic.ldx, Imm),
    invalid,
    Instr(Mnemonic.ldy, z),
    Instr(Mnemonic.lda, z),
    Instr(Mnemonic.ldx, z),
    invalid,
    Instr(Mnemonic.tay),
    Instr(Mnemonic.lda, Imm),
    Instr(Mnemonic.tax),
    invalid,
    Instr(Mnemonic.ldy, A),
    Instr(Mnemonic.lda, A),
    Instr(Mnemonic.ldx, A),
    invalid,
 	// B0
Instr(InstrClass.ConditionalTransfer, Mnemonic.bcs, j),
    Instr(Mnemonic.lda, Iy),
    invalid,
    invalid,
    Instr(Mnemonic.ldy, zx),
    Instr(Mnemonic.lda, zx),
    Instr(Mnemonic.ldx, zy),
    invalid,
    Instr(Mnemonic.clv),
    Instr(Mnemonic.lda, Ay),
    Instr(Mnemonic.tsx),
    invalid,
    Instr(Mnemonic.ldy, Ax),
    Instr(Mnemonic.lda, Ax),
    Instr(Mnemonic.ldx, Ay),
    invalid,
    
    // C0
    Instr(Mnemonic.cpy, Imm),
        Instr(Mnemonic.cmp, Ix),
        invalid,
        invalid,
        Instr(Mnemonic.cpy, z),
        Instr(Mnemonic.cmp, z),
        Instr(Mnemonic.dec, z),
        invalid,
        Instr(Mnemonic.iny),
        Instr(Mnemonic.cmp, Imm),
        Instr(Mnemonic.dex),
        invalid,
        Instr(Mnemonic.cpy, A),
        Instr(Mnemonic.cmp, A),
        Instr(Mnemonic.dec, A),
        invalid,
 
        // D0
        Instr(InstrClass.ConditionalTransfer, Mnemonic.bne, j),
        Instr(Mnemonic.cmp, Iy),
        invalid,
        invalid,
        invalid,
        Instr(Mnemonic.cmp, zx),
        Instr(Mnemonic.dec, zx),
        invalid,
        Instr(Mnemonic.cld),
        Instr(Mnemonic.cmp, Ay),
        invalid,
        invalid,
        invalid,
        Instr(Mnemonic.cmp, Ax),
        Instr(Mnemonic.dec, Ax),
        invalid,

        // E0
        Instr(Mnemonic.cpx, Imm),
        Instr(Mnemonic.sbc, Ix),
        invalid,
        invalid,
        Instr(Mnemonic.cpx, z),
        Instr(Mnemonic.sbc, z),
        Instr(Mnemonic.inc, z),
        invalid,
        Instr(Mnemonic.inx),
        Instr(Mnemonic.sbc, Imm),
        Instr(Mnemonic.nop),
        invalid,
        Instr(Mnemonic.cpx, A),
        Instr(Mnemonic.sbc, A),
        Instr(Mnemonic.inc, A),
        invalid,
 
        // F0
        Instr(InstrClass.ConditionalTransfer, Mnemonic.beq, j),
        Instr(Mnemonic.sbc, Iy),
        invalid,
        invalid,
        invalid,
        Instr(Mnemonic.sbc, zx),
        Instr(Mnemonic.inc, zx),
        invalid,
        Instr(Mnemonic.sed),
        Instr(Mnemonic.sbc, Ay),
        invalid,
        invalid,
        invalid,
        Instr(Mnemonic.sbc, Ax),
        Instr(Mnemonic.inc, Ax),
        invalid,
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