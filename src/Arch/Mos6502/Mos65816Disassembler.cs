#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Mos6502
{
#pragma warning disable IDE1006

    using Decoder = Reko.Core.Machine.Decoder<Mos65816Disassembler, Mnemonic, Instruction>;

    // http://www.oxyron.de/html/opcodes816.html
    public class Mos65816Disassembler : DisassemblerBase<Instruction, Mnemonic>
    {
        private static readonly Decoder [] decoders;

        private readonly Mos65816Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Mos65816Disassembler(Mos65816Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = rdr.Address;
            this.ops = new List<MachineOperand>();
        }

        public override Instruction? DisassembleInstruction()
        {
            if (!rdr.TryReadByte(out byte op))
                return null;
            var instr = decoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            this.addr = rdr.Address;
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

        public override Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Dis65816", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        public override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
            };
        }

        #region Mutators

        private static bool imm(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Byte(imm),
            });
            return true;
        }

        private static bool sr(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.StackRelative,
                Offset = Constant.Byte(imm),
                Register = Registers.s,
            });
            return true;
        }

        // dp = $00
        private static bool dp(uint wInstr, Mos65816Disassembler dasm)
        { 
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPage,
                Offset = Constant.Byte(imm),
            });
            return true;
        }

        // dpx = $00,X
        private static bool dpx(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageX,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.X
            });
            return true;
        }

        // dpy = $00,Y
        private static bool dpy(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageY,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.Y
            });
            return true;
        }
        
        // idp = ($00)
        private static bool idp(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageIndirect,
                Offset = Constant.Byte(imm),
            });
            return true;
        }

        // idx = ($00,X)
        private static bool idx(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageIndexedIndirectX,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.X
            });
            return true;
        }

        // idy = ($00),Y
        private static bool idy(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageIndirectIndexedY,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.X
            });
            return true;
        }

        // idl = [$00]
        private static bool idl(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageIndirectLong,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.X
            });
            return true;
        }

        // idly = [$00],Y
        private static bool idly(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.DirectPageIndirectLongIndexedY,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.X
            });
            return true;
        }
        
        // isy = ($00,S),Y
        private static bool isy(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.StackRelativeIndirectIndexedY,
                Offset = Constant.Byte(imm),
                Register = Mos65816Architecture.Y
            });
            return true;
        }

        // abs = $0000
        private static bool abs(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Absolute,
                Offset = Constant.Word16(imm),
            });
            return true;
        }

        // abx = $0000,X
        private static bool abx(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteX,
                Offset = Constant.Word16(imm),
                Register = Mos65816Architecture.X,
            });
            return true;
        }
        
        // aby = $0000,Y
        private static bool aby(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteY,
                Offset = Constant.Word16(imm),
                Register = Mos65816Architecture.Y,
            });
            return true;
        }
        
        // abl = $000000
        private static bool abl(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort immLo))
                return false;
            if (!dasm.rdr.TryReadByte(out byte immHi))
                return false;

            var abs = ((uint) immHi << 16) | immLo;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteLong,
                Offset = Constant.Create(Mos65816Architecture.Word24, abs),
            });
            return true;
        }

        // alx = $000000,X
        private static bool alx(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort immLo))
                return false;
            if (!dasm.rdr.TryReadByte(out byte immHi))
                return false;

            var abs = ((uint) immHi << 16) | immLo;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteLongX,
                Offset = Constant.Create(Mos65816Architecture.Word24, abs),
                Register = Registers.x
            });
            return true;
        }

        // ind = ($0000)
        private static bool ind(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;

            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Indirect,
                Offset = Constant.Word16(imm),
            });
            return true;
        }

        // iax = ($0000,X)
        private static bool iax(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;

            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteIndexedIndirect,
                Offset = Constant.Word16(imm),
                Register = Registers.x,
            });
            return true;
        }

        // ial = [$000000]
        private static bool ial(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;

            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.AbsoluteIndirectLong,
                Offset = Constant.Word16(imm),
            });
            return true;
        }

        // rel = $0000 (8 bits PC-relative)
        private static bool rel(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte rel))
                return false;

            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Word16((ushort)((int)dasm.rdr.Address.Offset + (sbyte) rel)),
            });
            return true;
        }

        // rell = $0000 (16 bits PC-relative)
        private static bool rell(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short rel))
                return false;

            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Word16((ushort) ((int) dasm.rdr.Address.Offset + rel)),
            });
            return true;
        }

        // bm = $00,$00
        private static bool bm(uint wInstr, Mos65816Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte bankDst) ||
                !dasm.rdr.TryReadByte(out byte bankSrc))
                return false;
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Byte(bankDst)
            });
            dasm.ops.Add(new Operand(PrimitiveType.Byte)
            {
                Mode = AddressMode.Immediate,
                Offset = Constant.Byte(bankSrc)
            });
            return true; 
        }

        #endregion

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Mos65816Disassembler>[] mutators)
        {
            return new InstrDecoder<Mos65816Disassembler, Mnemonic, Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Mos65816Disassembler>[] mutators)
        {
            return new InstrDecoder<Mos65816Disassembler, Mnemonic, Instruction>(iclass, mnemonic, mutators);
        }


        static Mos65816Disassembler()
        {
            Decoder invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);

            decoders = new Decoder[256] {
            // $00
    Instr(Mnemonic.brk, InstrClass.Invalid|InstrClass.Zero|InstrClass.Padding),
    Instr(Mnemonic.ora, idx),
    Instr(Mnemonic.cop, imm),
    Instr(Mnemonic.ora, sr),

    Instr(Mnemonic.tsb, dp),
    Instr(Mnemonic.ora, dp),
    Instr(Mnemonic.asl, dp),
    Instr(Mnemonic.ora, idl),

    Instr(Mnemonic.php),
    Instr(Mnemonic.ora, imm),
    Instr(Mnemonic.asl),
    Instr(Mnemonic.phd),

    Instr(Mnemonic.tsb, abs),
    Instr(Mnemonic.ora, abs),
    Instr(Mnemonic.asl, abs),
    Instr(Mnemonic.ora, abl),


            // $10
    Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.ora, idy),
    Instr(Mnemonic.ora, idp),
    Instr(Mnemonic.ora, isy),
    Instr(Mnemonic.trb, dp),
    Instr(Mnemonic.ora, dpx),
    Instr(Mnemonic.asl, dpx),
    Instr(Mnemonic.ora, idly),
    Instr(Mnemonic.clc),
    Instr(Mnemonic.ora, aby),
    Instr(Mnemonic.inc),
    Instr(Mnemonic.tcs),
    Instr(Mnemonic.trb, abs),
    Instr(Mnemonic.ora, abx),
    Instr(Mnemonic.asl, abx),
    Instr(Mnemonic.ora, alx),


    // $20
    Instr(Mnemonic.jsr, InstrClass.Call|InstrClass.Transfer, abs),
    Instr(Mnemonic.and, idx),
    Instr(Mnemonic.jsr, InstrClass.Call|InstrClass.Transfer, abl),
    Instr(Mnemonic.and, sr),
    Instr(Mnemonic.bit, dp),
    Instr(Mnemonic.and, dp),
    Instr(Mnemonic.rol, dp),
    Instr(Mnemonic.and, idl),
    Instr(Mnemonic.plp),
    Instr(Mnemonic.and, imm),
    Instr(Mnemonic.rol),
    Instr(Mnemonic.pld),
    Instr(Mnemonic.bit, abs),
    Instr(Mnemonic.and, abs),
    Instr(Mnemonic.rol, abs),
    Instr(Mnemonic.and, abl),

    // $30
    Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.and, idy),
    Instr(Mnemonic.and, idp),
    Instr(Mnemonic.and, isy),
    Instr(Mnemonic.bit, dpx),
    Instr(Mnemonic.and, dpx),
    Instr(Mnemonic.rol, dpx),
    Instr(Mnemonic.and, idly),
    Instr(Mnemonic.sec),
    Instr(Mnemonic.and, aby),
    Instr(Mnemonic.dec),
    Instr(Mnemonic.tsc),
    Instr(Mnemonic.bit, abx),
    Instr(Mnemonic.and, abx),
    Instr(Mnemonic.rol, abx),
    Instr(Mnemonic.and, alx),

    // $40
    Instr(Mnemonic.rti, InstrClass.Transfer|InstrClass.Return),
    Instr(Mnemonic.eor, idx),
     Instr(Mnemonic.wdm, InstrClass.Invalid),
    Instr(Mnemonic.eor, sr),
    Instr(Mnemonic.mvp, bm),
    Instr(Mnemonic.eor, dp),
    Instr(Mnemonic.lsr, dp),
    Instr(Mnemonic.eor, idl),
    Instr(Mnemonic.pha),
    Instr(Mnemonic.eor, imm),
    Instr(Mnemonic.lsr),
    Instr(Mnemonic.phk),
    Instr(Mnemonic.jmp, InstrClass.Transfer, abs),
    Instr(Mnemonic.eor, abs),
    Instr(Mnemonic.lsr, abs),
    Instr(Mnemonic.eor, abl),


    // $50
    Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.eor, idy),
    Instr(Mnemonic.eor, idp),
    Instr(Mnemonic.eor, isy),
    Instr(Mnemonic.mvn, bm),
    Instr(Mnemonic.eor, dpx),
    Instr(Mnemonic.lsr, dpx),
    Instr(Mnemonic.eor, idly),
    Instr(Mnemonic.cli),
    Instr(Mnemonic.eor, aby),
    Instr(Mnemonic.phy),
    Instr(Mnemonic.tcd),
    Instr(Mnemonic.jmp, InstrClass.Transfer, abl),
    Instr(Mnemonic.eor, abx),
    Instr(Mnemonic.lsr, abx),
    Instr(Mnemonic.eor, alx),


    // $60
    Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return),
    Instr(Mnemonic.adc, idx),
    Instr(Mnemonic.per, rell),
    Instr(Mnemonic.adc, sr),
    Instr(Mnemonic.stz, dp),
    Instr(Mnemonic.adc, dp),
    Instr(Mnemonic.ror, dp),
    Instr(Mnemonic.adc, idl),
    Instr(Mnemonic.pla),
    Instr(Mnemonic.adc, imm),
    Instr(Mnemonic.ror),
    Instr(Mnemonic.rtl),
    Instr(Mnemonic.jmp, InstrClass.Transfer, ind),
    Instr(Mnemonic.adc, abs),
    Instr(Mnemonic.ror, abs),
    Instr(Mnemonic.adc, abl),

    // $70
    Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.adc, idy),
    Instr(Mnemonic.adc, idp),
    Instr(Mnemonic.adc, isy),
    Instr(Mnemonic.stz, dpx),
    Instr(Mnemonic.adc, dpx),
    Instr(Mnemonic.ror, dpx),
    Instr(Mnemonic.adc, idly),
    Instr(Mnemonic.sei),
    Instr(Mnemonic.adc, aby),
    Instr(Mnemonic.ply),
    Instr(Mnemonic.tdc),

    Instr(Mnemonic.jmp, InstrClass.Transfer, ial),
    Instr(Mnemonic.adc, abx),
    Instr(Mnemonic.ror, abx),
    Instr(Mnemonic.adc, alx),


    // $80
    Instr(Mnemonic.bra, InstrClass.Transfer, rel),
    Instr(Mnemonic.sta, idx),
    Instr(Mnemonic.brl, InstrClass.Transfer, rell),
    Instr(Mnemonic.sta, sr),
    Instr(Mnemonic.sty, dp),
    Instr(Mnemonic.sta, dp),
    Instr(Mnemonic.stx, dp),
    Instr(Mnemonic.sta, idl),
    Instr(Mnemonic.dey),
    Instr(Mnemonic.bit, imm),
    Instr(Mnemonic.txa),
    Instr(Mnemonic.phb),
    Instr(Mnemonic.sty, abs),
    Instr(Mnemonic.sta, abs),
    Instr(Mnemonic.stx, abs),
    Instr(Mnemonic.sta, abl),


    // $90
    Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.sta, idy),
    Instr(Mnemonic.sta, idp),
    Instr(Mnemonic.sta, isy),
    Instr(Mnemonic.sty, dpx),
    Instr(Mnemonic.sta, dpx),
    Instr(Mnemonic.stx, dpy),
    Instr(Mnemonic.sta, idly),
    Instr(Mnemonic.tya),
    Instr(Mnemonic.sta, aby),
    Instr(Mnemonic.txs),
    Instr(Mnemonic.txy),
    Instr(Mnemonic.stz, abs),
    Instr(Mnemonic.sta, abx),
    Instr(Mnemonic.stz, abx),
    Instr(Mnemonic.sta, alx),


    // $a0
    Instr(Mnemonic.ldy, imm),
    Instr(Mnemonic.lda, idx),
    Instr(Mnemonic.ldx, imm),
    Instr(Mnemonic.lda, sr),
    Instr(Mnemonic.ldy, dp),
    Instr(Mnemonic.lda, dp),
    Instr(Mnemonic.ldx, dp),
    Instr(Mnemonic.lda, idl),
    Instr(Mnemonic.tay),
    Instr(Mnemonic.lda, imm),
    Instr(Mnemonic.tax),
    Instr(Mnemonic.plb),
    Instr(Mnemonic.ldy, abs),
    Instr(Mnemonic.lda, abs),
    Instr(Mnemonic.ldx, abs),
    Instr(Mnemonic.lda, abl),


    // $b0
    Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.lda, idy),
    Instr(Mnemonic.lda, idp),
    Instr(Mnemonic.lda, isy),
    Instr(Mnemonic.ldy, dpx),
    Instr(Mnemonic.lda, dpx),
    Instr(Mnemonic.ldx, dpy),
    Instr(Mnemonic.lda, idly),
    Instr(Mnemonic.clv),
    Instr(Mnemonic.lda, aby),
    Instr(Mnemonic.tsx),
    Instr(Mnemonic.tyx),
    Instr(Mnemonic.ldy, abx),
    Instr(Mnemonic.lda, abx),
    Instr(Mnemonic.ldx, aby),
    Instr(Mnemonic.lda, alx),


    // $c0
    Instr(Mnemonic.cpy, imm),
    Instr(Mnemonic.cmp, idx),
    Instr(Mnemonic.rep, imm),
    Instr(Mnemonic.cmp, sr),
    Instr(Mnemonic.cpy, dp),
    Instr(Mnemonic.cmp, dp),
    Instr(Mnemonic.dec, dp),
    Instr(Mnemonic.cmp, idl),
    Instr(Mnemonic.iny),
    Instr(Mnemonic.cmp, imm),
    Instr(Mnemonic.dex),
    Instr(Mnemonic.wai),
    Instr(Mnemonic.cpy, abs),
    Instr(Mnemonic.cmp, abs),
    Instr(Mnemonic.dec, abs),
    Instr(Mnemonic.cmp, abl),


    // $d0
    Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.cmp, idy),
    Instr(Mnemonic.cmp, idp),
    Instr(Mnemonic.cmp, isy),
    Instr(Mnemonic.pei, idp),
    Instr(Mnemonic.cmp, dpx),
    Instr(Mnemonic.dec, dpx),
    Instr(Mnemonic.cmp, idly),
    Instr(Mnemonic.cld),
    Instr(Mnemonic.cmp, aby),
    Instr(Mnemonic.phx),
    Instr(Mnemonic.stp),
    Instr(Mnemonic.jmp, InstrClass.Transfer, iax),
    Instr(Mnemonic.cmp, abx),
    Instr(Mnemonic.dec, abx),
    Instr(Mnemonic.cmp, alx),


    // $e0
    Instr(Mnemonic.cpx, imm),
    Instr(Mnemonic.sbc, idx),
    Instr(Mnemonic.sep, imm),
    Instr(Mnemonic.sbc, sr),
    Instr(Mnemonic.cpx, dp),
    Instr(Mnemonic.sbc, dp),
    Instr(Mnemonic.inc, dp),
    Instr(Mnemonic.sbc, idl),
    Instr(Mnemonic.inx),
    Instr(Mnemonic.sbc, imm),
    Instr(Mnemonic.nop),
    Instr(Mnemonic.xba),
    Instr(Mnemonic.cpx, abs),
    Instr(Mnemonic.sbc, abs),
    Instr(Mnemonic.inc, abs),
    Instr(Mnemonic.sbc, abl),


    // $f0
    Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, rel),
    Instr(Mnemonic.sbc, idy),
    Instr(Mnemonic.sbc, idp),
    Instr(Mnemonic.sbc, isy),
    Instr(Mnemonic.pea, abs),
    Instr(Mnemonic.sbc, dpx),
    Instr(Mnemonic.inc, dpx),
    Instr(Mnemonic.sbc, idly),
    Instr(Mnemonic.sed),
    Instr(Mnemonic.sbc, aby),
    Instr(Mnemonic.plx),
    Instr(Mnemonic.xce),
    Instr(Mnemonic.jsr, InstrClass.Call|InstrClass.Transfer, iax),
    Instr(Mnemonic.sbc, abx),
    Instr(Mnemonic.inc, abx),
    Instr(Mnemonic.sbc, alx),
            };
        }
    }
}
