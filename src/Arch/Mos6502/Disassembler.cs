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
    using Decoder = Decoder<Disassembler, Opcode, Instruction>;

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

        protected override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Code = Opcode.illegal,
            };
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

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode mnemonic;
            private readonly Mutator<Disassembler> [] mutators;

            public InstrDecoder(Opcode mnemonic, params Mutator<Disassembler>[] mutators)
            {
                this.iclass = InstrClass.Linear;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public InstrDecoder(InstrClass iclass, Opcode mnemonic, params Mutator<Disassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override Instruction Decode(uint wInstr, Disassembler dasm)
            {
                for (int i = 0; i < mutators.Length; ++i)
                {
                    if (!mutators[i](wInstr, dasm))
                    {
                        return dasm.CreateInvalidInstruction();
                    }
                }

                var instr = new Instruction
                {
                    Code = mnemonic,
                    InstructionClass = iclass,
                    Operand = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                };
                return instr;
            }
        }

        private static readonly Decoder[] decoders = new InstrDecoder[] {
            // 00
new InstrDecoder(InstrClass.Padding|InstrClass.Zero, Opcode.brk),
    new InstrDecoder(Opcode.ora, Ix),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ora, z),
    new InstrDecoder(Opcode.asl, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.php),
    new InstrDecoder(Opcode.ora, Imm),
    new InstrDecoder(Opcode.asl, a),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ora, A),
    new InstrDecoder(Opcode.asl, A),
    new InstrDecoder(Opcode.illegal),

            // 10
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bpl, j),
    new InstrDecoder(Opcode.ora, Iy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ora, zx),
    new InstrDecoder(Opcode.asl, zx),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.clc),
    new InstrDecoder(Opcode.ora, Ay),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ora, Ax),
    new InstrDecoder(Opcode.asl, Ax),
    new InstrDecoder(Opcode.illegal),
            // 20
new InstrDecoder(InstrClass.Transfer|InstrClass.Call, Opcode.jsr, A),
    new InstrDecoder(Opcode.and, Ix),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.bit, z),
    new InstrDecoder(Opcode.and, z),
    new InstrDecoder(Opcode.rol, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.plp),
    new InstrDecoder(Opcode.and, Imm),
    new InstrDecoder(Opcode.rol, a),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.bit, A),
    new InstrDecoder(Opcode.and, A),
    new InstrDecoder(Opcode.rol, A),
    new InstrDecoder(Opcode.illegal),
        // 30
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bmi, j),
    new InstrDecoder(Opcode.and, Iy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.and, zx),
    new InstrDecoder(Opcode.rol, zx),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.sec),
    new InstrDecoder(Opcode.and, Ay),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.and, Ax),
    new InstrDecoder(Opcode.rol, Ax),
    new InstrDecoder(Opcode.illegal),
 // 40
new InstrDecoder(InstrClass.Transfer, Opcode.rti),
    new InstrDecoder(Opcode.eor, Ix),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.eor, z),
    new InstrDecoder(Opcode.lsr, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.pha),
    new InstrDecoder(Opcode.eor, Imm),
    new InstrDecoder(Opcode.lsr, a),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(InstrClass.Transfer, Opcode.jmp, A),
    new InstrDecoder(Opcode.eor, A),
    new InstrDecoder(Opcode.lsr, A),
    new InstrDecoder(Opcode.illegal),
 // 50
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bvc, j),
    new InstrDecoder(Opcode.eor, Iy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.eor, zx),
    new InstrDecoder(Opcode.lsr, zx),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.cli),
    new InstrDecoder(Opcode.eor, Ay),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.eor, Ax),
    new InstrDecoder(Opcode.lsr, Ax),
    new InstrDecoder(Opcode.illegal),
 	// 60
new InstrDecoder(InstrClass.Transfer, Opcode.rts),
    new InstrDecoder(Opcode.adc, Ix),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.adc, z),
    new InstrDecoder(Opcode.ror, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.pla),
    new InstrDecoder(Opcode.adc, Imm),
    new InstrDecoder(Opcode.ror, a),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(InstrClass.Transfer, Opcode.jmp, i),	
    new InstrDecoder(Opcode.adc, A),
    new InstrDecoder(Opcode.ror, A),
    new InstrDecoder(Opcode.illegal),
 	// 70
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bvs, j),
    new InstrDecoder(Opcode.adc, Iy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.adc, zx),
    new InstrDecoder(Opcode.ror, zx),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.sei),
    new InstrDecoder(Opcode.adc, Ay),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.adc, Ax),
    new InstrDecoder(Opcode.ror, Ax),
    new InstrDecoder(Opcode.illegal),
 	// 80
new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.sta, Ix),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.sty, z),
    new InstrDecoder(Opcode.sta, z),
    new InstrDecoder(Opcode.stx, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.dey),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.txa),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.sty, A),
    new InstrDecoder(Opcode.sta, A),
    new InstrDecoder(Opcode.stx, A),
    new InstrDecoder(Opcode.illegal),
 	// 90
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bcc, j),
 	new InstrDecoder(Opcode.sta, Iy),
 	new InstrDecoder(Opcode.illegal),
 	new InstrDecoder(Opcode.illegal),
 	new InstrDecoder(Opcode.sty, zx),
 	new InstrDecoder(Opcode.sta, zx),
 	new InstrDecoder(Opcode.stx, zy),
 	new InstrDecoder(Opcode.illegal),
 	new InstrDecoder(Opcode.tya),
 	new InstrDecoder(Opcode.sta, Ay),
 	new InstrDecoder(Opcode.txs),
 	new InstrDecoder(Opcode.illegal),
 	new InstrDecoder(Opcode.illegal),
  	new InstrDecoder(Opcode.sta, Ax),
 	new InstrDecoder(Opcode.illegal),
 	new InstrDecoder(Opcode.illegal),

// A0
new InstrDecoder(Opcode.ldy, Imm),
    new InstrDecoder(Opcode.lda, Ix),
    new InstrDecoder(Opcode.ldx, Imm),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ldy, z),
    new InstrDecoder(Opcode.lda, z),
    new InstrDecoder(Opcode.ldx, z),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.tay),
    new InstrDecoder(Opcode.lda, Imm),
    new InstrDecoder(Opcode.tax),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ldy, A),
    new InstrDecoder(Opcode.lda, A),
    new InstrDecoder(Opcode.ldx, A),
    new InstrDecoder(Opcode.illegal),
 	// B0
new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bcs, j),
    new InstrDecoder(Opcode.lda, Iy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ldy, zx),
    new InstrDecoder(Opcode.lda, zx),
    new InstrDecoder(Opcode.ldx, zy),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.clv),
    new InstrDecoder(Opcode.lda, Ay),
    new InstrDecoder(Opcode.tsx),
    new InstrDecoder(Opcode.illegal),
    new InstrDecoder(Opcode.ldy, Ax),
    new InstrDecoder(Opcode.lda, Ax),
    new InstrDecoder(Opcode.ldx, Ay),
    new InstrDecoder(Opcode.illegal),
    
    // C0
    new InstrDecoder(Opcode.cpy, Imm),
        new InstrDecoder(Opcode.cmp, Ix),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cpy, z),
        new InstrDecoder(Opcode.cmp, z),
        new InstrDecoder(Opcode.dec, z),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.iny),
        new InstrDecoder(Opcode.cmp, Imm),
        new InstrDecoder(Opcode.dex),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cpy, A),
        new InstrDecoder(Opcode.cmp, A),
        new InstrDecoder(Opcode.dec, A),
        new InstrDecoder(Opcode.illegal),
 
        // D0
        new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.bne, j),
        new InstrDecoder(Opcode.cmp, Iy),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cmp, zx),
        new InstrDecoder(Opcode.dec, zx),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cld),
        new InstrDecoder(Opcode.cmp, Ay),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cmp, Ax),
        new InstrDecoder(Opcode.dec, Ax),
        new InstrDecoder(Opcode.illegal),

        // E0
        new InstrDecoder(Opcode.cpx, Imm),
        new InstrDecoder(Opcode.sbc, Ix),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cpx, z),
        new InstrDecoder(Opcode.sbc, z),
        new InstrDecoder(Opcode.inc, z),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.inx),
        new InstrDecoder(Opcode.sbc, Imm),
        new InstrDecoder(Opcode.nop),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.cpx, A),
        new InstrDecoder(Opcode.sbc, A),
        new InstrDecoder(Opcode.inc, A),
        new InstrDecoder(Opcode.illegal),
 
        // F0
        new InstrDecoder(InstrClass.ConditionalTransfer, Opcode.beq, j),
        new InstrDecoder(Opcode.sbc, Iy),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.sbc, zx),
        new InstrDecoder(Opcode.inc, zx),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.sed),
        new InstrDecoder(Opcode.sbc, Ay),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.illegal),
        new InstrDecoder(Opcode.sbc, Ax),
        new InstrDecoder(Opcode.inc, Ax),
        new InstrDecoder(Opcode.illegal),
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