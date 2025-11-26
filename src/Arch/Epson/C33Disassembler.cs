#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Types;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reko.Arch.Epson;

public class C33Disassembler : DisassemblerBase<C33Instruction, Mnemonic>
{
    private static readonly Decoder<C33Disassembler, Mnemonic, C33Instruction> rootDecoder;
    private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
    private static readonly Bitfield bf0_13 = new Bitfield(0, 13);
    private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
    private static readonly Bitfield bf4_6 = new Bitfield(4, 6);
    private static readonly RegisterStorage?[] sysRegisters =
    [
        Registers.PSR,
        Registers.SP,
        Registers.ALR,
        Registers.AHR,
        null,
        null,
        null,
        null,
        Registers.TTBR,
        null,
        Registers.IDIR,
        Registers.DBBR,
        null,
        null,
        null,
        Registers.PC
    ];

    private readonly C33Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private int cExtBits;
    private int ext;

    public C33Disassembler(C33Architecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override C33Instruction CreateInvalidInstruction()
    {
        return new C33Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid
        };
    }

    public override C33Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        this.ops.Clear();
        this.ext = 0;
        C33Instruction instr;
        if (!rdr.TryReadUInt16(out ushort uInstr))
            return null;
        for (; ; )
        {

            instr = rootDecoder.Decode(uInstr, this);
            if (instr.Mnemonic != Mnemonic.ext)
                break;
            cExtBits += 13;
            if (cExtBits > 26)
            {
                instr = CreateInvalidInstruction();
                break;
            }
            if (!rdr.TryReadUInt16(out uInstr))
            {
                instr = CreateInvalidInstruction();
                break;
            }
        }
        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        return instr;
    }

    public override C33Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new C33Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray()
        };
    }

    public override C33Instruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("C33Dasm", addr, rdr, message);
        return new C33Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Nyi
        };
    }

    private static Mutator<C33Disassembler> Register(int bitpos, RegisterStorage?[] regs)
    {
        var field = new Bitfield(bitpos, 4);
        return (u, d) =>
        {
            var ireg = field.Read(u);
            var reg = regs[ireg];
            if (reg is null)
                return false;
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly Mutator<C33Disassembler> r0 = Register(0, Registers.GpRegisters);
    private static readonly Mutator<C33Disassembler> r4 = Register(4, Registers.GpRegisters);
    private static readonly Mutator<C33Disassembler> s0 = Register(0, sysRegisters!);
    private static readonly Mutator<C33Disassembler> s4 = Register(4, sysRegisters!);

    private static bool sp(uint uInstr, C33Disassembler dasm)
    {
        dasm.ops.Add(Registers.SP);
        return true;
    }

    private static bool s0_ahr(uint uInstr, C33Disassembler dasm)
    {
        var ireg = bf0_4.Read(uInstr);
        var sreg = sysRegisters[ireg];
        if (sreg != Registers.AHR && sreg != Registers.ALR)
            return false;
        dasm.ops.Add(sreg);
        return true;
    }

    private static Mutator<C33Disassembler> imm(int bitpos, int bitlength, int shift)
    {
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            var imm = field.Read(u);
            imm = ((uint) d.ext << field.Length) | imm;
            d.ops.Add(Constant.Word32(imm << shift));
            return true;
        };
    }
    private static readonly Mutator<C33Disassembler> imm0_5 = imm(0, 5, 0);
    private static readonly Mutator<C33Disassembler> imm0_6 = imm(0, 6, 0);
    private static readonly Mutator<C33Disassembler> imm4_6 = imm(4, 6, 0);
    private static readonly Mutator<C33Disassembler> imm4_4 = imm(4, 4, 0);
    private static readonly Mutator<C33Disassembler> imm0_10_4 = imm(0, 10, 2);

    private static bool imm2(uint uInstr, C33Disassembler dasm)
    {
        var imm = uInstr & 0b11;
        dasm.ops.Add(Constant.Word32(imm));
        return true;
    }

    private static bool imm3(uint uInstr, C33Disassembler dasm)
    {
        var imm = uInstr & 0b111;
        dasm.ops.Add(Constant.Word32(imm));
        return true;
    }


    private static Mutator<C33Disassembler> sign(int bitpos, int bitlength)
    {
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            var imm = field.Read(u);
            imm = ((uint) d.ext << field.Length) | imm;
            var simm = Bits.SignExtend(imm, field.Length + d.cExtBits);
            d.ops.Add(Constant.Word32((uint)simm));
            return true;
        };
    }
    private static readonly Mutator<C33Disassembler> sign4_6 = sign(4, 6);

    private static Mutator<C33Disassembler> disp(int bitpos, int bitlength)
    {
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            var imm = field.Read(u);
            imm = ((uint) d.ext << field.Length) | imm;
            long simm = Bits.SignExtend(imm, field.Length + d.cExtBits);
            if (d.cExtBits == 0)
                simm <<= 1;
            var target = d.rdr.Address + (simm - 2);
            if ((target.Offset & 1) != 0)
                return false;
            d.ops.Add(target);
            return true;
        };
    }
    private static readonly Mutator<C33Disassembler> disp8 = disp(0, 8);


    private static bool extImm(uint uInstr, C33Disassembler dasm)
    {
        if (dasm.cExtBits == 0)
            return true;
        dasm.ops.Add(Constant.Word32(dasm.ext));
        return true;
    }

    private static bool NoExt(uint uInstr, C33Disassembler dasm)
    {
        return dasm.cExtBits == 0;
    }

    private static bool M4(uint uInstr, C33Disassembler dasm)
    {
        var ireg = bf4_4.Read(uInstr);
        var reg = Registers.GpRegisters[ireg];
        Constant? offset = null;
        if (dasm.cExtBits != 0)
        {
            offset = Constant.UInt32((uint)dasm.ext);
        }
        var mem = new MemoryOperand(reg, offset, false);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool M4p(uint uInstr, C33Disassembler dasm)
    {
        var ireg = bf4_4.Read(uInstr);
        var reg = Registers.GpRegisters[ireg];
        var mem = new MemoryOperand(reg, null, true);
        dasm.ops.Add(mem);
        return true;
    }

    private static Mutator<C33Disassembler> Msp_disp(PrimitiveType dt)
    {
        int scale = dt.Size;
        return (uInstr, dasm) =>
        {
            var imm = bf4_6.Read(uInstr);
            Constant? offset;
            if (dasm.cExtBits != 0)
            {
                offset = Constant.UInt32((uint) (dasm.ext << 6) | imm);
            } 
            else if (imm != 0)
            {
                offset = Constant.UInt32(imm * (uint)scale);
            }
            else
            {
                offset = null;
            }
            var mem = new MemoryOperand(Registers.SP, offset, false);
            dasm.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<C33Disassembler> Msp_b = Msp_disp(PrimitiveType.Byte);
    private static readonly Mutator<C33Disassembler> Msp_h = Msp_disp(PrimitiveType.Word16);
    private static readonly Mutator<C33Disassembler> Msp_w = Msp_disp(PrimitiveType.Word32);

    private static bool ExtWord(uint uInstr, C33Disassembler dasm)
    {
        var ext = bf0_13.Read(uInstr);
        dasm.ext = (dasm.ext << 13) | (int) ext;
        return true;
    }

    private static Decoder<C33Disassembler, Mnemonic, C33Instruction> Instr(Mnemonic mnemonic, params Mutator<C33Disassembler>[] mutators)
    {
        return new InstrDecoder<C33Disassembler, Mnemonic, C33Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static Decoder<C33Disassembler, Mnemonic, C33Instruction> Instr(Mnemonic mneonic, InstrClass iclass, params Mutator<C33Disassembler>[] mutators)
    {
        return new InstrDecoder<C33Disassembler, Mnemonic, C33Instruction>(iclass, mneonic, mutators);
    }

    private static Decoder<C33Disassembler, Mnemonic, C33Instruction> Instr_8(
        Mnemonic mnemonic0,
        Mnemonic mnemonic1,
        InstrClass iclass0,
        InstrClass iclass1,
        params Mutator<C33Disassembler>[] mutators)
    {
        return Mask(8, 1, "",
            Instr(mnemonic0, iclass0, mutators),
            Instr(mnemonic1, iclass1, mutators));
    }

    private static bool Eq0(uint u) => u == 0;

    static C33Disassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid, []);
        var nyi = Instr(Mnemonic.Nyi, InstrClass.Invalid, []);

        var decode000_0000 = Sparse(4, 5, "  000 0000", nyi,
            (0b00000, If(0, 4, Eq0, Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Zero|InstrClass.Padding))),
            (0b00100, If(0, 4, Eq0, Instr(Mnemonic.slp))),
            (0b01000, If(0, 4, Eq0, Instr(Mnemonic.halt, InstrClass.Terminates))),
            (0b00001, Instr(Mnemonic.push, r0)),
            (0b00101, Instr(Mnemonic.pop, r0)),
            (0b01001, Instr(Mnemonic.pushs, s0_ahr)),
            (0b01101, Instr(Mnemonic.pops, s0_ahr)),
            (0b11101, If(0, 4, Eq0, Instr(Mnemonic.ld_cf, s0))));
        var decode000_0001 = Sparse(4, 5, "  000 0001", nyi,
            (0b00000, Instr(Mnemonic.pushn, r0)),
            (0b00100, Instr(Mnemonic.popn, r0)),
            (0b01100, Instr(Mnemonic.jpr, InstrClass.Transfer, r0)),
            (0b11100, Instr(Mnemonic.jpr_d, InstrClass.Transfer|InstrClass.Delay, r0)));
        var decode000_0010 = Sparse(4, 5, "  000 0010", nyi,
            (0b00000, If(0, 4, Eq0, Instr(Mnemonic.brk, InstrClass.Terminates))),
            (0b00100, If(0, 4, Eq0, Instr(Mnemonic.retd, InstrClass.Transfer | InstrClass.Return))),
            (0b01000, If(0, 4, Eq0, Instr(Mnemonic.@int, InstrClass.Transfer | InstrClass.Call, imm2))),
            (0b01100, If(0, 4, Eq0, Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return))));
        var decode000_0011 = Sparse(4, 5, "  000 0011", nyi,
            (0b00000, Instr(Mnemonic.call, InstrClass.Transfer | InstrClass.Call, NoExt, r0)),
            (0b00100, If(0, 4, Eq0, Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return))),
            (0b01000, Instr(Mnemonic.jp, InstrClass.Transfer, r0)),
            (0b10000, Instr(Mnemonic.call_d, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay, r0)),
            (0b10100, Instr(Mnemonic.ret_d, InstrClass.Transfer | InstrClass.Return | InstrClass.Delay)),
            (0b11000, Instr(Mnemonic.jp_d, InstrClass.Transfer | InstrClass.Delay, r0)));

        var CT = InstrClass.ConditionalTransfer;
        var CTD = CT | InstrClass.Delay;
        var Tcall = InstrClass.Transfer | InstrClass.Call;
        var TcallD = Tcall | InstrClass.Delay;
        var T = InstrClass.Transfer;
        var TD = InstrClass.Transfer | InstrClass.Delay;
        var decode000_0100 = Instr_8(Mnemonic.jrgt, Mnemonic.jrgt_d, CT, CTD, disp8);
        var decode000_0101 = Instr_8(Mnemonic.jrge, Mnemonic.jrge_d, CT, CTD, disp8);
        var decode000_0110 = Instr_8(Mnemonic.jrlt, Mnemonic.jrlt_d, CT, CTD, disp8);
        var decode000_0111 = Instr_8(Mnemonic.jrle, Mnemonic.jrle_d, CT, CTD, disp8);
        var decode000_1000 = Instr_8(Mnemonic.jrugt, Mnemonic.jrugt_d, CT, CTD, disp8);
        var decode000_1001 = Instr_8(Mnemonic.jruge, Mnemonic.jruge_d, CT, CTD, disp8);
        var decode000_1010 = Instr_8(Mnemonic.jrult, Mnemonic.jrult_d, CT, CTD, disp8);
        var decode000_1011 = Instr_8(Mnemonic.jrule, Mnemonic.jrule_d, CT, CTD, disp8);
        var decode000_1100 = Instr_8(Mnemonic.jreq, Mnemonic.jreq_d, CT, CTD, disp8);
        var decode000_1101 = Instr_8(Mnemonic.jrne, Mnemonic.jrne_d, CT, CTD, disp8);
        var decode000_1110 = Instr_8(Mnemonic.call, Mnemonic.call_d, Tcall, TcallD, disp8);
        var decode000_1111 = Instr_8(Mnemonic.jp, Mnemonic.jp_d, T, TD, disp8);


        var decode000 = Mask(9, 4, "  000",
            decode000_0000,
            decode000_0001,
            decode000_0010,
            decode000_0011,
            decode000_0100,
            decode000_0101,
            decode000_0110,
            decode000_0111,
            decode000_1000,
            decode000_1001,
            decode000_1010,
            decode000_1011,
            decode000_1100,
            decode000_1101,
            decode000_1110,
            decode000_1111);

        var decode001 = Sparse(8, 5, "  001", invalid,
            (0b00000, Instr(Mnemonic.ld_b, r0, M4)),
            (0b00001, Instr(Mnemonic.ld_b, r0, M4p)),
            (0b00010, Instr(Mnemonic.add, r0, r4, extImm)),
            (0b00011, Instr(Mnemonic.srl, r0, imm4_4)),

            (0b00100, Instr(Mnemonic.ld_ub, r0, M4)),
            (0b00101, Instr(Mnemonic.ld_ub, r0, M4p)),
            (0b00110, Instr(Mnemonic.sub, r0, r4)),
            (0b00111, Instr(Mnemonic.sll, r0, imm4_4)),

            (0b01000, Instr(Mnemonic.ld_h, r0, M4)),
            (0b01001, Instr(Mnemonic.ld_h, r0, M4p)),
            (0b01010, Instr(Mnemonic.cmp, r0, r4)),
            (0b01011, Instr(Mnemonic.sra, r0, imm4_4)),

            (0b01100, Instr(Mnemonic.ld_uh, r0, M4)),
            (0b01101, Instr(Mnemonic.ld_uh, r0, M4p)),
            (0b01110, Instr(Mnemonic.ld_w, r0, r4)),
            (0b01111, Instr(Mnemonic.sla, r0, imm4_4)),

            (0b10000, Instr(Mnemonic.ld_w, r0, M4)),
            (0b10001, Instr(Mnemonic.ld_w, r0, M4p)),
            (0b10010, Instr(Mnemonic.and, r0, r4, extImm)),
            (0b10011, Instr(Mnemonic.rr, r0, imm4_4)),

            (0b10100, Instr(Mnemonic.ld_b, M4, r0)),
            (0b10101, Instr(Mnemonic.ld_b, M4p, r0)),
            (0b10110, Instr(Mnemonic.or, r0, r4)),
            (0b10111, Instr(Mnemonic.rl, r0, imm4_4)),

            (0b11000, Instr(Mnemonic.ld_h, M4, r0)),
            (0b11001, Instr(Mnemonic.ld_h, M4p, r0)),
            (0b11010, Instr(Mnemonic.xor, r0, r4)),

            (0b11100, Instr(Mnemonic.ld_w, M4, r0)),
            (0b11101, Instr(Mnemonic.ld_w, M4p, r0)),
            (0b11110, Instr(Mnemonic.not, r0, r4)));

        var decode010 = Mask(10, 3, "  010",
            Instr(Mnemonic.ld_b, r0, Msp_b),
            Instr(Mnemonic.ld_ub, r0, Msp_b),
            Instr(Mnemonic.ld_h, r0, Msp_h),
            Instr(Mnemonic.ld_uh, r0, Msp_h),
            Instr(Mnemonic.ld_w, r0, Msp_w),
            Instr(Mnemonic.ld_b, Msp_b, r0),
            Instr(Mnemonic.ld_h, Msp_h, r0),
            Instr(Mnemonic.ld_w, Msp_w, r0));

        var decode011 = Mask(10, 3, "  011",
            Instr(Mnemonic.add, r0, imm4_6),
            Instr(Mnemonic.sub, r0, imm4_6),
            Instr(Mnemonic.cmp, r0, sign4_6),
            Instr(Mnemonic.ld_w, r0, sign4_6),
            Instr(Mnemonic.and, r0, sign4_6),
            Instr(Mnemonic.or, r0, sign4_6),
            Instr(Mnemonic.xor, r0, sign4_6),
            Instr(Mnemonic.not, r0, sign4_6));
        var decode100 = Mask(10, 3, "  100",
            Instr(Mnemonic.add, sp, imm0_10_4),
            Instr(Mnemonic.sub, sp, imm0_10_4),
            Mask(8, 2, "  010",
                Instr(Mnemonic.srl, r0, imm4_4),
                Instr(Mnemonic.srl, r0, r4),
                invalid,
                invalid),
            Mask(8, 2, "  011",
                Instr(Mnemonic.sll, r0, imm4_4),
                Instr(Mnemonic.sll, r0, r4),
                invalid,
                invalid),
            Mask(8, 2, "  100",
                Instr(Mnemonic.sra, r0, imm4_4),
                Instr(Mnemonic.sra, r0, r4),
                Instr(Mnemonic.swap, r0, r4),
                invalid),
            Mask(8, 2, "  101",
                Instr(Mnemonic.sla, r0, imm4_4),
                Instr(Mnemonic.sla, r0, r4),
                invalid,
                invalid),
            Mask(8, 2, "  110",
                Instr(Mnemonic.rr, r0, imm4_4),
                Instr(Mnemonic.rr, r0, r4),
                Instr(Mnemonic.swaph, r0, r4),
                invalid),
            Mask(8, 2, "  111",
                Instr(Mnemonic.rl, r0, imm4_4),
                Instr(Mnemonic.rl, r0, r4),
                invalid,
                invalid));


        var decode101 = Mask(10, 3, "  101",
            Mask(8, 2, "  000",
                Instr(Mnemonic.ld_w, s0, r4),
                Instr(Mnemonic.ld_b, r0, r4),
                Instr(Mnemonic.mlt_h, r0, r4),
                invalid),
            Mask(8, 2, "  001",
                Instr(Mnemonic.ld_w, r0, s4),
                Instr(Mnemonic.ld_ub, r0, r4),
                Instr(Mnemonic.mltu_h, r0, r4),
                invalid),
            Mask(8, 2, "  010",
                Instr(Mnemonic.btst, M4, imm3),
                Instr(Mnemonic.ld_h, r0, r4),
                Instr(Mnemonic.mlt_w, r0, r4),
                invalid),
            Mask(8, 2, "  011",
                Instr(Mnemonic.bclr, M4, imm3),
                Instr(Mnemonic.ld_uh, r0, r4),
                Instr(Mnemonic.mltu_w, r0, r4),
                invalid),
            Mask(8, 2, "  100",
                Instr(Mnemonic.bset, M4, imm3),
                Instr(Mnemonic.ld_c, r0, imm4_4),
                invalid,
                invalid),
            Mask(8, 2, "  101",
                Instr(Mnemonic.bnot, M4, imm3),
                Instr(Mnemonic.ld_c, imm4_4, r0),
                invalid,
                invalid),
            Mask(8, 2, "  110",
                Instr(Mnemonic.adc, r0, r4),
                invalid,
                invalid,
                invalid),
            Mask(8, 2, "  111",
                Instr(Mnemonic.sbc, r0, r4),
                invalid,
                invalid,
                Mask(6, 2, "  11",
                    Instr(Mnemonic.do_c, imm0_6),
                    Instr(Mnemonic.psrset, imm0_5),
                    Instr(Mnemonic.psrclr, imm0_5),
                    invalid)));

        rootDecoder = Mask(13, 3, "C33",
            decode000,
            decode001,
            decode010,
            decode011,
            decode100,
            decode100,
            Instr(Mnemonic.ext, ExtWord),
            invalid);
    }
}
