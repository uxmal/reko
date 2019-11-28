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
    using Decoder = Decoder<Disassembler, Mnemonic, Instruction>;

    // http://www.e-tradition.net/bytes/6502/6502_instruction_set.html
    // 65816 = http://www.zophar.net/fileuploads/2/10538ivwiu/65816info.txt
    public class Disassembler : DisassemblerBase<Instruction>
    {
        private readonly EndianImageReader rdr;
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
                Mnemonic = Mnemonic.illegal,
                Operands = MachineInstruction.NoOperands
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
            private readonly Mnemonic mnemonic;
            private readonly Mutator<Disassembler> [] mutators;

            public InstrDecoder(Mnemonic mnemonic, params Mutator<Disassembler>[] mutators)
            {
                this.iclass = InstrClass.Linear;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, params Mutator<Disassembler>[] mutators)
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
                    Mnemonic = mnemonic,
                    InstructionClass = iclass,
                    Operands = dasm.ops.ToArray(),
                };
                return instr;
            }
        }

        private static readonly Decoder[] decoders = new InstrDecoder[] {
            // 00
new InstrDecoder(InstrClass.Padding|InstrClass.Zero, Mnemonic.brk),
    new InstrDecoder(Mnemonic.ora, Ix),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ora, z),
    new InstrDecoder(Mnemonic.asl, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.php),
    new InstrDecoder(Mnemonic.ora, Imm),
    new InstrDecoder(Mnemonic.asl, a),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ora, A),
    new InstrDecoder(Mnemonic.asl, A),
    new InstrDecoder(Mnemonic.illegal),

            // 10
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bpl, j),
    new InstrDecoder(Mnemonic.ora, Iy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ora, zx),
    new InstrDecoder(Mnemonic.asl, zx),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.clc),
    new InstrDecoder(Mnemonic.ora, Ay),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ora, Ax),
    new InstrDecoder(Mnemonic.asl, Ax),
    new InstrDecoder(Mnemonic.illegal),
            // 20
new InstrDecoder(InstrClass.Transfer|InstrClass.Call, Mnemonic.jsr, A),
    new InstrDecoder(Mnemonic.and, Ix),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.bit, z),
    new InstrDecoder(Mnemonic.and, z),
    new InstrDecoder(Mnemonic.rol, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.plp),
    new InstrDecoder(Mnemonic.and, Imm),
    new InstrDecoder(Mnemonic.rol, a),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.bit, A),
    new InstrDecoder(Mnemonic.and, A),
    new InstrDecoder(Mnemonic.rol, A),
    new InstrDecoder(Mnemonic.illegal),
        // 30
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bmi, j),
    new InstrDecoder(Mnemonic.and, Iy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.and, zx),
    new InstrDecoder(Mnemonic.rol, zx),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.sec),
    new InstrDecoder(Mnemonic.and, Ay),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.and, Ax),
    new InstrDecoder(Mnemonic.rol, Ax),
    new InstrDecoder(Mnemonic.illegal),
 // 40
new InstrDecoder(InstrClass.Transfer, Mnemonic.rti),
    new InstrDecoder(Mnemonic.eor, Ix),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.eor, z),
    new InstrDecoder(Mnemonic.lsr, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.pha),
    new InstrDecoder(Mnemonic.eor, Imm),
    new InstrDecoder(Mnemonic.lsr, a),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(InstrClass.Transfer, Mnemonic.jmp, A),
    new InstrDecoder(Mnemonic.eor, A),
    new InstrDecoder(Mnemonic.lsr, A),
    new InstrDecoder(Mnemonic.illegal),
 // 50
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bvc, j),
    new InstrDecoder(Mnemonic.eor, Iy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.eor, zx),
    new InstrDecoder(Mnemonic.lsr, zx),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.cli),
    new InstrDecoder(Mnemonic.eor, Ay),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.eor, Ax),
    new InstrDecoder(Mnemonic.lsr, Ax),
    new InstrDecoder(Mnemonic.illegal),
 	// 60
new InstrDecoder(InstrClass.Transfer, Mnemonic.rts),
    new InstrDecoder(Mnemonic.adc, Ix),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.adc, z),
    new InstrDecoder(Mnemonic.ror, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.pla),
    new InstrDecoder(Mnemonic.adc, Imm),
    new InstrDecoder(Mnemonic.ror, a),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(InstrClass.Transfer, Mnemonic.jmp, i),	
    new InstrDecoder(Mnemonic.adc, A),
    new InstrDecoder(Mnemonic.ror, A),
    new InstrDecoder(Mnemonic.illegal),
 	// 70
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bvs, j),
    new InstrDecoder(Mnemonic.adc, Iy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.adc, zx),
    new InstrDecoder(Mnemonic.ror, zx),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.sei),
    new InstrDecoder(Mnemonic.adc, Ay),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.adc, Ax),
    new InstrDecoder(Mnemonic.ror, Ax),
    new InstrDecoder(Mnemonic.illegal),
 	// 80
new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.sta, Ix),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.sty, z),
    new InstrDecoder(Mnemonic.sta, z),
    new InstrDecoder(Mnemonic.stx, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.dey),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.txa),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.sty, A),
    new InstrDecoder(Mnemonic.sta, A),
    new InstrDecoder(Mnemonic.stx, A),
    new InstrDecoder(Mnemonic.illegal),
 	// 90
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bcc, j),
 	new InstrDecoder(Mnemonic.sta, Iy),
 	new InstrDecoder(Mnemonic.illegal),
 	new InstrDecoder(Mnemonic.illegal),
 	new InstrDecoder(Mnemonic.sty, zx),
 	new InstrDecoder(Mnemonic.sta, zx),
 	new InstrDecoder(Mnemonic.stx, zy),
 	new InstrDecoder(Mnemonic.illegal),
 	new InstrDecoder(Mnemonic.tya),
 	new InstrDecoder(Mnemonic.sta, Ay),
 	new InstrDecoder(Mnemonic.txs),
 	new InstrDecoder(Mnemonic.illegal),
 	new InstrDecoder(Mnemonic.illegal),
  	new InstrDecoder(Mnemonic.sta, Ax),
 	new InstrDecoder(Mnemonic.illegal),
 	new InstrDecoder(Mnemonic.illegal),

// A0
new InstrDecoder(Mnemonic.ldy, Imm),
    new InstrDecoder(Mnemonic.lda, Ix),
    new InstrDecoder(Mnemonic.ldx, Imm),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ldy, z),
    new InstrDecoder(Mnemonic.lda, z),
    new InstrDecoder(Mnemonic.ldx, z),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.tay),
    new InstrDecoder(Mnemonic.lda, Imm),
    new InstrDecoder(Mnemonic.tax),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ldy, A),
    new InstrDecoder(Mnemonic.lda, A),
    new InstrDecoder(Mnemonic.ldx, A),
    new InstrDecoder(Mnemonic.illegal),
 	// B0
new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bcs, j),
    new InstrDecoder(Mnemonic.lda, Iy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ldy, zx),
    new InstrDecoder(Mnemonic.lda, zx),
    new InstrDecoder(Mnemonic.ldx, zy),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.clv),
    new InstrDecoder(Mnemonic.lda, Ay),
    new InstrDecoder(Mnemonic.tsx),
    new InstrDecoder(Mnemonic.illegal),
    new InstrDecoder(Mnemonic.ldy, Ax),
    new InstrDecoder(Mnemonic.lda, Ax),
    new InstrDecoder(Mnemonic.ldx, Ay),
    new InstrDecoder(Mnemonic.illegal),
    
    // C0
    new InstrDecoder(Mnemonic.cpy, Imm),
        new InstrDecoder(Mnemonic.cmp, Ix),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cpy, z),
        new InstrDecoder(Mnemonic.cmp, z),
        new InstrDecoder(Mnemonic.dec, z),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.iny),
        new InstrDecoder(Mnemonic.cmp, Imm),
        new InstrDecoder(Mnemonic.dex),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cpy, A),
        new InstrDecoder(Mnemonic.cmp, A),
        new InstrDecoder(Mnemonic.dec, A),
        new InstrDecoder(Mnemonic.illegal),
 
        // D0
        new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.bne, j),
        new InstrDecoder(Mnemonic.cmp, Iy),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cmp, zx),
        new InstrDecoder(Mnemonic.dec, zx),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cld),
        new InstrDecoder(Mnemonic.cmp, Ay),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cmp, Ax),
        new InstrDecoder(Mnemonic.dec, Ax),
        new InstrDecoder(Mnemonic.illegal),

        // E0
        new InstrDecoder(Mnemonic.cpx, Imm),
        new InstrDecoder(Mnemonic.sbc, Ix),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cpx, z),
        new InstrDecoder(Mnemonic.sbc, z),
        new InstrDecoder(Mnemonic.inc, z),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.inx),
        new InstrDecoder(Mnemonic.sbc, Imm),
        new InstrDecoder(Mnemonic.nop),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.cpx, A),
        new InstrDecoder(Mnemonic.sbc, A),
        new InstrDecoder(Mnemonic.inc, A),
        new InstrDecoder(Mnemonic.illegal),
 
        // F0
        new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.beq, j),
        new InstrDecoder(Mnemonic.sbc, Iy),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.sbc, zx),
        new InstrDecoder(Mnemonic.inc, zx),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.sed),
        new InstrDecoder(Mnemonic.sbc, Ay),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.illegal),
        new InstrDecoder(Mnemonic.sbc, Ax),
        new InstrDecoder(Mnemonic.inc, Ax),
        new InstrDecoder(Mnemonic.illegal),
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