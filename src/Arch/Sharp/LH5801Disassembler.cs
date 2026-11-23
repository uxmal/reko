#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Sharp;

public class LH5801Disassembler : DisassemblerBase<LH5801Instruction, Mnemonic>
{
    private static readonly MaskDecoder<LH5801Disassembler, Mnemonic, LH5801Instruction> rootDecoder;
    private static readonly Bitfield bf4_2 = new(4, 2);

    private readonly LH5801Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public LH5801Disassembler(LH5801Architecture arch, EndianImageReader imageReader)
    {
        this.arch = arch;
        this.rdr = imageReader;
        this.ops = [];
    }

    public override LH5801Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadByte(out byte uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        ops.Clear();
        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        return instr;
    }


    public override LH5801Instruction CreateInvalidInstruction()
    {
        return MakeInstruction(InstrClass.Invalid, Mnemonic.Invalid);
    }


    public override LH5801Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new LH5801Instruction
        {
            InstructionClass = iclass, 
            Mnemonic = mnemonic, 
            Operands = ops.ToArray()
        };
    }

    public override LH5801Instruction NotYetImplemented(string message)
    {
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        testSvc?.ReportMissingDecoder("LH5801Dis", addr, rdr, message);
        return MakeInstruction(InstrClass.Invalid, Mnemonic.Nyi);
    }

    private static readonly RegisterStorage[] LowRegs =
        [
        Registers.XL,
        Registers.YL,
        Registers.UL,
        ];
    private static readonly RegisterStorage[] HighRegs =
        [
        Registers.XH,
        Registers.YH,
        Registers.UH,
        ];
    private static readonly RegisterStorage[] WideRegs =
        [
        Registers.X,
        Registers.Y,
        Registers.U,
        ];


    private static bool Rl(uint uInstr, LH5801Disassembler dasm)
    {
        var ireg = bf4_2.Read(uInstr);
        dasm.ops.Add(LowRegs[ireg]);
        return true;
    }

    private static bool Rh(uint uInstr, LH5801Disassembler dasm)
    {
        var ireg = bf4_2.Read(uInstr);
        dasm.ops.Add(HighRegs[ireg]);
        return true;
    }

    private static bool R(uint uInstr, LH5801Disassembler dasm)
    {
        var ireg = bf4_2.Read(uInstr);
        dasm.ops.Add(WideRegs[ireg]);
        return true;
    }

    private static bool A(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(Registers.A);
        return true;
    }

    private static bool S(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(Registers.S);
        return true;
    }

    private static bool P(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(Registers.P);
        return true;
    }

    private static bool UL(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(Registers.UL);
        return true;
    }

    private static bool MR(uint uInstr, LH5801Disassembler dasm)
    {
        var ireg = bf4_2.Read(uInstr);
        dasm.ops.Add(new MemoryOperand(WideRegs[ireg], false));
        return true;
    }

    private static bool M1R(uint uInstr, LH5801Disassembler dasm)
    {
        var ireg = bf4_2.Read(uInstr);
        dasm.ops.Add(new MemoryOperand(WideRegs[ireg], true));
        return true;
    }

    private static bool Maddr(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out ushort uAddr))
            return false;
        dasm.ops.Add(new MemoryOperand(Address.Ptr16(uAddr), false));
        return true;
    }

    private static bool M1addr(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out ushort uAddr))
            return false;
        dasm.ops.Add(new MemoryOperand(Address.Ptr16(uAddr), true));
        return true;
    }

    private static bool imm(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        dasm.ops.Add(Constant.Byte(b));
        return true;
    }

    private static bool imm16(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out ushort w))
            return false;
        dasm.ops.Add(Constant.Word16(w));
        return true;
    }

    private static bool vej(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(Constant.Byte((byte)uInstr));
        return true;
    }

    private static bool J(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out ushort w))
            return false;
        dasm.ops.Add(Address.Ptr16(w));
        return true;
    }

    private static bool Jdisp(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte off))
            return false;
        long offset = Bits.IsBitSet(uInstr, 4)
            ? -off
            : off;
        dasm.ops.Add(dasm.addr + offset);
        return true;
    }

    private static bool Jdispm(uint uInstr, LH5801Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte w))
            return false;
        dasm.ops.Add(dasm.addr - w);
        return true;
    }


    private static bool ME1(uint uInstr, LH5801Disassembler dasm)
    {
        dasm.ops.Add(new LiteralOperand("#"));
        return true;
    }

    private class NextByteDecoder : Decoder<LH5801Disassembler, Mnemonic, LH5801Instruction>
    {
        private readonly Decoder<LH5801Disassembler, Mnemonic, LH5801Instruction> decoder;

        public NextByteDecoder(Decoder<LH5801Disassembler, Mnemonic, LH5801Instruction> decoder)
        {
            this.decoder = decoder;
        }

        public override LH5801Instruction Decode(uint wInstr, LH5801Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte uInstrNext))
                return dasm.CreateInvalidInstruction();
            return this.decoder.Decode(uInstrNext, dasm);
        }
    }


    private static Decoder<LH5801Disassembler, Mnemonic, LH5801Instruction> Instr(
        Mnemonic mnemonic,
        InstrClass iclass, 
        params Mutator<LH5801Disassembler>[] mutators)
    {
        return new InstrDecoder<LH5801Disassembler, Mnemonic, LH5801Instruction>(iclass, mnemonic, mutators);
    }

    private static Decoder<LH5801Disassembler, Mnemonic, LH5801Instruction> Instr(
        Mnemonic mnemonic,
        params Mutator<LH5801Disassembler>[] mutators)
    {
        return new InstrDecoder<LH5801Disassembler, Mnemonic, LH5801Instruction>(InstrClass.Linear, mnemonic, mutators);
    }
    
    static LH5801Disassembler()
    {
        var nyi = new NyiDecoder<LH5801Disassembler , Mnemonic, LH5801Instruction>("nyi");
        var vejDecoder = Instr(Mnemonic.vej, vej);

        var fdDecoder = Sparse(0, 8, "  FD", nyi,
            (0x01, Instr(Mnemonic.sbc, M1R)),
            (0x11, Instr(Mnemonic.sbc, M1R)),
            (0x21, Instr(Mnemonic.sbc, M1R)),
            (0x03, Instr(Mnemonic.adc, M1R)),
            (0x13, Instr(Mnemonic.adc, M1R)),
            (0x23, Instr(Mnemonic.adc, M1R)),
            (0x05, Instr(Mnemonic.lda, M1R)),
            (0x15, Instr(Mnemonic.lda, M1R)),
            (0x25, Instr(Mnemonic.lda, M1R)),
            (0x07, Instr(Mnemonic.cpa, M1R)),
            (0x17, Instr(Mnemonic.cpa, M1R)),
            (0x27, Instr(Mnemonic.cpa, M1R)),
            (0x08, Instr(Mnemonic.ldx, R)),
            (0x18, Instr(Mnemonic.ldx, S)),
            (0x28, Instr(Mnemonic.ldx, P)),
            (0x09, Instr(Mnemonic.and, M1R)),
            (0x19, Instr(Mnemonic.and, M1R)),
            (0x29, Instr(Mnemonic.and, M1R)),
            (0x0A, Instr(Mnemonic.pop, R)),
            (0x1A, Instr(Mnemonic.pop, R)),
            (0x2A, Instr(Mnemonic.pop, R)),
            (0x0B, Instr(Mnemonic.ora, M1R)),
            (0x1B, Instr(Mnemonic.ora, M1R)),
            (0x2B, Instr(Mnemonic.ora, M1R)),
            (0x0C, Instr(Mnemonic.dcs, M1R)),
            (0x1C, Instr(Mnemonic.dcs, M1R)),
            (0x2C, Instr(Mnemonic.dcs, M1R)),
            (0x0D, Instr(Mnemonic.eor, M1R)),
            (0x1D, Instr(Mnemonic.eor, M1R)),
            (0x2D, Instr(Mnemonic.eor, M1R)),
            (0x0E, Instr(Mnemonic.sta, M1R)),
            (0x1E, Instr(Mnemonic.sta, M1R)),
            (0x2E, Instr(Mnemonic.sta, M1R)),
            (0x0F, Instr(Mnemonic.bit, M1R)),
            (0x1F, Instr(Mnemonic.bit, M1R)),
            (0x2F, Instr(Mnemonic.bit, M1R)),
            (0x40, Instr(Mnemonic.inc, Rh)),
            (0x50, Instr(Mnemonic.inc, Rh)),
            (0x60, Instr(Mnemonic.inc, Rh)),
            (0x42, Instr(Mnemonic.dec, Rh)),
            (0x52, Instr(Mnemonic.dec, Rh)),
            (0x62, Instr(Mnemonic.dec, Rh)),
            (0x49, Instr(Mnemonic.ani, M1R, imm)),
            (0x59, Instr(Mnemonic.ani, M1R, imm)),
            (0x69, Instr(Mnemonic.ani, M1R, imm)),
            (0x4A, Instr(Mnemonic.stx, R)),
            (0x5A, Instr(Mnemonic.stx, R)),
            (0x6A, Instr(Mnemonic.stx, R)),
            (0x4B, Instr(Mnemonic.ori, M1R, imm)),
            (0x5B, Instr(Mnemonic.ori, M1R, imm)),
            (0x6B, Instr(Mnemonic.ori, M1R, imm)),
            (0x4C, Instr(Mnemonic.off)),
            (0x4D, Instr(Mnemonic.bii, M1R, imm)),
            (0x5D, Instr(Mnemonic.bii, M1R, imm)),
            (0x6D, Instr(Mnemonic.bii, M1R, imm)),
            (0x4E, Instr(Mnemonic.stx, S)),
            (0x4F, Instr(Mnemonic.adi, M1R, imm)),
            (0x5E, Instr(Mnemonic.stx, P)),
            (0x5F, Instr(Mnemonic.adi, M1R, imm)),
            (0x6F, Instr(Mnemonic.adi, M1R, imm)),
            (0x88, Instr(Mnemonic.psh, R)),
            (0x98, Instr(Mnemonic.psh, R)),
            (0xA8, Instr(Mnemonic.psh, R)),
            (0x81, Instr(Mnemonic.sie)),
            (0x8A, Instr(Mnemonic.pop, A)),
            (0x8C, Instr(Mnemonic.dca, M1R)),
            (0x8E, Instr(Mnemonic.cdv)),
            (0x9C, Instr(Mnemonic.dca, M1R)),
            (0xA1, Instr(Mnemonic.sbc, M1addr)),
            (0xA3, Instr(Mnemonic.adc, M1addr)),
            (0xA5, Instr(Mnemonic.lda, M1addr)),
            (0xA7, Instr(Mnemonic.cpa, M1addr)),
            (0xA9, Instr(Mnemonic.and, M1addr)),
            (0xAA, Instr(Mnemonic.tta)),
            (0xAB, Instr(Mnemonic.ora, M1addr)),
            (0xAC, Instr(Mnemonic.dca, M1R)),
            (0xAD, Instr(Mnemonic.eor, M1addr)),
            (0xAE, Instr(Mnemonic.sta, M1addr)),
            (0xAF, Instr(Mnemonic.bit, M1addr)),
            (0xB1, Instr(Mnemonic.hlt, InstrClass.Terminates)),
            (0xBA, Instr(Mnemonic.ita)),
            (0xBE, Instr(Mnemonic.rie)),
            (0xC0, Instr(Mnemonic.rdp)),
            (0xC1, Instr(Mnemonic.sdp)),
            (0xC8, Instr(Mnemonic.psh, A)),
            (0xCC, Instr(Mnemonic.atp)),
            (0xCA, Instr(Mnemonic.adr, R)),
            (0xDA, Instr(Mnemonic.adr, R)),
            (0xEA, Instr(Mnemonic.adr, R)),
            (0xCE, Instr(Mnemonic.am0)),
            (0xD3, Instr(Mnemonic.drr, ME1)),
            (0xD7, Instr(Mnemonic.drl, ME1)),
            (0xDE, Instr(Mnemonic.am1)),
            (0xE9, Instr(Mnemonic.ani, M1addr, imm)),
            (0xEB, Instr(Mnemonic.ori, M1addr, imm)),
            (0xEC, Instr(Mnemonic.att)),
            (0xED, Instr(Mnemonic.bii, M1addr, imm)),
            (0xEF, Instr(Mnemonic.adi, M1addr, imm)));
        rootDecoder = Mask(0, 8, "LH5801",
            Instr(Mnemonic.sbc, Rl),
            nyi,
            Instr(Mnemonic.adc, Rl),
            Instr(Mnemonic.adc, MR),

            Instr(Mnemonic.lda, Rl),
            Instr(Mnemonic.lda, MR),
            Instr(Mnemonic.cpa, Rl),
            Instr(Mnemonic.cpa, MR),

            Instr(Mnemonic.sta, Rh),
            Instr(Mnemonic.and, MR),
            Instr(Mnemonic.sta, Rl),
            Instr(Mnemonic.ora, MR),

            Instr(Mnemonic.dcs, MR),
            Instr(Mnemonic.eor, MR),
            Instr(Mnemonic.sta, MR),
            Instr(Mnemonic.bit, MR),

            // 0x10
            Instr(Mnemonic.sbc, Rl),
            nyi,
            Instr(Mnemonic.adc, Rl),
            Instr(Mnemonic.adc, MR),

            Instr(Mnemonic.lda, Rl),
            Instr(Mnemonic.lda, MR),
            Instr(Mnemonic.cpa, Rl),
            Instr(Mnemonic.cpa, MR),

            Instr(Mnemonic.sta, Rh),
            Instr(Mnemonic.and, MR),
            Instr(Mnemonic.sta, Rl),
            Instr(Mnemonic.ora, MR),


            Instr(Mnemonic.dcs, MR),
            Instr(Mnemonic.eor, MR),
            Instr(Mnemonic.sta, MR),
            Instr(Mnemonic.bit, MR),

            // 0x20
            Instr(Mnemonic.sbc, Rl),
            nyi,
            Instr(Mnemonic.adc, Rl),
            Instr(Mnemonic.adc, MR),

            Instr(Mnemonic.lda, Rl),
            Instr(Mnemonic.lda, MR),
            Instr(Mnemonic.cpa, Rl),
            Instr(Mnemonic.cpa, MR),

            Instr(Mnemonic.sta, Rh),
            Instr(Mnemonic.and, MR),
            Instr(Mnemonic.sta, Rl),
            Instr(Mnemonic.ora, MR),

            Instr(Mnemonic.dcs, MR),
            Instr(Mnemonic.eor, MR),
            Instr(Mnemonic.sta, MR),
            Instr(Mnemonic.bit, MR),

            // 0x30
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x40
            Instr(Mnemonic.inc, Rl),
            Instr(Mnemonic.sin, R),
            Instr(Mnemonic.dec, Rl),
            Instr(Mnemonic.sde, R),

            Instr(Mnemonic.inc, R),
            Instr(Mnemonic.lin, R),
            Instr(Mnemonic.dec, R),
            Instr(Mnemonic.lde, R),

            Instr(Mnemonic.ldi, Rh,imm),
            Instr(Mnemonic.ani, MR,imm),
            Instr(Mnemonic.ldi, Rl,imm),
            Instr(Mnemonic.ori, MR,imm),

            Instr(Mnemonic.cpi, Rh, imm),
            Instr(Mnemonic.bii, MR, imm),
            Instr(Mnemonic.cpi, Rl, imm),
            Instr(Mnemonic.adi, MR, imm),

            // 0x50
            Instr(Mnemonic.inc, Rl),
            Instr(Mnemonic.sin, R),
            Instr(Mnemonic.dec, Rl),
            Instr(Mnemonic.sde, R),

            Instr(Mnemonic.inc, R),
            Instr(Mnemonic.lin, R),
            Instr(Mnemonic.dec, R),
            Instr(Mnemonic.lde, R),

            nyi,
            Instr(Mnemonic.ani, MR, imm),
            nyi,
            Instr(Mnemonic.ori, MR,imm),

            Instr(Mnemonic.cpi, Rh, imm),
            Instr(Mnemonic.bii, MR, imm),
            Instr(Mnemonic.cpi, Rl, imm),
            Instr(Mnemonic.adi, MR, imm),

            // 0x60
            Instr(Mnemonic.inc, Rl),
            Instr(Mnemonic.sin, R),
            Instr(Mnemonic.dec, Rl),
            Instr(Mnemonic.sde, R),

            Instr(Mnemonic.inc, R),
            Instr(Mnemonic.lin, R),
            Instr(Mnemonic.dec, R),
            Instr(Mnemonic.lde, R),

            nyi,
            Instr(Mnemonic.ani, MR,imm),
            nyi,
            Instr(Mnemonic.ori, MR,imm),

            Instr(Mnemonic.cpi, Rh, imm),
            Instr(Mnemonic.bii, MR, imm),
            Instr(Mnemonic.cpi, Rl, imm),
            Instr(Mnemonic.adi, MR, imm),

            // 0x70
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x80
            Instr(Mnemonic.sbc, Rh),
            Instr(Mnemonic.bcr, Jdisp),
            Instr(Mnemonic.adc, Rh),
            Instr(Mnemonic.bcs, Jdisp),

            Instr(Mnemonic.lda, Rh),
            Instr(Mnemonic.bhr, Jdisp),
            Instr(Mnemonic.cpa, Rh),
            Instr(Mnemonic.bhs, Jdisp),

            nyi,
            Instr(Mnemonic.bzr, Jdisp),
            Instr(Mnemonic.rti, InstrClass.Return),
            Instr(Mnemonic.bzs, Jdisp),

            Instr(Mnemonic.dca, MR),
            Instr(Mnemonic.bvr, Jdisp),
            Instr(Mnemonic.bch, Jdisp),
            Instr(Mnemonic.bvs, Jdisp),

            // 0x90
            Instr(Mnemonic.sbc, Rh),
            Instr(Mnemonic.bcr, Jdisp),
            Instr(Mnemonic.adc, Rh),
            Instr(Mnemonic.bcs, Jdisp),

            Instr(Mnemonic.lda, Rh),
            Instr(Mnemonic.bhr, Jdisp),
            Instr(Mnemonic.cpa, Rh),
            Instr(Mnemonic.bhs, Jdisp),

            Instr(Mnemonic.lop, UL,Jdispm),
            Instr(Mnemonic.bzr, Jdisp),
            Instr(Mnemonic.rtn, InstrClass.Return),
            Instr(Mnemonic.bzs, Jdisp),

            Instr(Mnemonic.dca, MR),
            Instr(Mnemonic.bvr, Jdisp),
            Instr(Mnemonic.bch, Jdisp),
            Instr(Mnemonic.bvs, Jdisp),

            // 0xA0
            Instr(Mnemonic.sbc, Rh),
            Instr(Mnemonic.sbc, Maddr),
            Instr(Mnemonic.adc, Rh),
            Instr(Mnemonic.adc, Maddr),

            Instr(Mnemonic.lda, Rh),
            Instr(Mnemonic.lda, Maddr),
            Instr(Mnemonic.cpa, Rh),
            Instr(Mnemonic.cpa, Maddr),

            Instr(Mnemonic.spv),
            Instr(Mnemonic.and, Maddr),
            Instr(Mnemonic.ldi, S, imm16),
            Instr(Mnemonic.ora, Maddr),

            Instr(Mnemonic.dca, MR),
            Instr(Mnemonic.eor, Maddr),
            Instr(Mnemonic.sta, Maddr),
            Instr(Mnemonic.bit, Maddr),

            // 0xB0
            nyi,
            Instr(Mnemonic.sbi, A, imm),
            nyi,
            Instr(Mnemonic.adi, A, imm),

            nyi,
            Instr(Mnemonic.ldi, A, imm),
            nyi,
            Instr(Mnemonic.cpi, A, imm),

            Instr(Mnemonic.rpv),
            Instr(Mnemonic.ani, A, imm),
            Instr(Mnemonic.jmp, InstrClass.Jump, J),
            Instr(Mnemonic.ori, A, imm),

            nyi,
            Instr(Mnemonic.eai, A, imm),
            Instr(Mnemonic.sjp, InstrClass.Call, J),
            Instr(Mnemonic.bii, A, imm),

            // 0xC0
            vejDecoder,
            Instr(Mnemonic.vcr, imm),
            vejDecoder,
            Instr(Mnemonic.vcs, imm),

            vejDecoder,
            Instr(Mnemonic.vhr, imm),
            vejDecoder,
            Instr(Mnemonic.vhs, imm),

            vejDecoder,
            Instr(Mnemonic.vzr, imm),
            vejDecoder,
            Instr(Mnemonic.vzs, imm),

            vejDecoder,
            Instr(Mnemonic.vmj, imm),
            vejDecoder,
            Instr(Mnemonic.vvs, imm),

            // 0xD0
            vejDecoder,
            Instr(Mnemonic.ror),
            vejDecoder,
            Instr(Mnemonic.drr),

            vejDecoder,
            Instr(Mnemonic.shr),
            vejDecoder,
            Instr(Mnemonic.drl),

            vejDecoder,
            Instr(Mnemonic.shl),
            vejDecoder,
            Instr(Mnemonic.rol),

            vejDecoder,
            Instr(Mnemonic.inc, A),
            vejDecoder,
            Instr(Mnemonic.dec, A),

            // 0xE0
            vejDecoder,
            Instr(Mnemonic.spu),
            vejDecoder,
            Instr(Mnemonic.rpu),

            vejDecoder,
            nyi,
            vejDecoder,
            nyi,

            vejDecoder,
            Instr(Mnemonic.ani, Maddr, imm),
            vejDecoder,
            Instr(Mnemonic.ori, Maddr, imm),

            vejDecoder,
            Instr(Mnemonic.bii, Maddr, imm),
            vejDecoder,
            Instr(Mnemonic.adi, Maddr, imm),

            // 0xF0
            vejDecoder,
            Instr(Mnemonic.aex),
            vejDecoder,
            nyi,

            vejDecoder,
            Instr(Mnemonic.tin),
            vejDecoder,
            Instr(Mnemonic.cin),

            nyi,
            Instr(Mnemonic.rec),
            nyi,
            Instr(Mnemonic.sec),

            nyi,
            new NextByteDecoder(fdDecoder),
            nyi,
            nyi);
    }
}