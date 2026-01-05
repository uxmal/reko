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
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Motorola.M88k;

public class M88kDisassembler : DisassemblerBase<M88kInstruction, Mnemonic>
{
    private static readonly Decoder<M88kDisassembler, Mnemonic, M88kInstruction> rootDecoder;
    private static readonly Bitfield bf0_5 = new(0, 5);
    private static readonly Bitfield bf0_9 = new(0, 9);
    private static readonly Bitfield bf0_16 = new(0, 16);
    private static readonly Bitfield bf0_26 = new(0, 26);
    private static readonly Bitfield bf5_2 = new(5, 2);
    private static readonly Bitfield bf5_5 = new(5, 5);
    private static readonly Bitfield bf5_6 = new(5, 6);
    private static readonly Bitfield bf7_2 = new(7, 2);
    private static readonly Bitfield bf9_2 = new(9, 2);
    private static readonly Bitfield bf16_5 = new(16, 5);
    private static readonly Bitfield bf21_5 = new(21, 5);

    private readonly M88kArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private uint? floatSizes;
    private bool userSpace;

    public M88kDisassembler(M88kArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override M88kInstruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadUInt32(out var uInstr))
            return null;

        var instr = rootDecoder.Decode(uInstr, this);
        this.floatSizes = null;
        this.userSpace = false;
        this.ops.Clear();
        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        return instr;
    }

    public override M88kInstruction CreateInvalidInstruction()
    {
        return new M88kInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }


    public override M88kInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new M88kInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray(),
            FloatSizes = this.floatSizes,
            UserSpace = this.userSpace,
        };
    }

    public override M88kInstruction NotYetImplemented(string message)
    {
        var genSvc = arch.Services.GetService<ITestGenerationService>();
        genSvc?.ReportMissingDecoder("M88kDasm", this.addr, this.rdr, message);
        return new M88kInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Nyi
        };
    }

    private static bool D(uint uInstr, M88kDisassembler dasm)
    {
        var ireg = bf21_5.Read(uInstr);
        var reg = Registers.GpRegisters[ireg];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool Dn0(uint uInstr, M88kDisassembler dasm)
    {
        var ireg = bf21_5.Read(uInstr);
        if (ireg == 0)
            return false;
        var reg = Registers.GpRegisters[ireg];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool S1(uint uInstr, M88kDisassembler dasm)
    {
        var ireg = bf16_5.Read(uInstr);
        var reg = Registers.GpRegisters[ireg];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool S2(uint uInstr, M88kDisassembler dasm)
    {
        var ireg = bf0_5.Read(uInstr);
        var reg = Registers.GpRegisters[ireg];
        dasm.ops.Add(reg);
        return true;
    }

    private static bool cr(uint uInstr , M88kDisassembler dasm)
    {
        var ireg = bf5_6.Read(uInstr);
        var reg = Registers.CrRegisters[ireg];
        if (reg is null)
            return false;
        dasm.ops.Add(reg);
        return true;
    }

    private static bool fcr(uint uInstr, M88kDisassembler dasm)
    {
        var ireg = bf5_6.Read(uInstr);
        var reg = Registers.FcrRegisters[ireg];
        if (reg is null)
            return false;
        dasm.ops.Add(reg);
        return true;
    }

    private static bool B5(uint uInstr, M88kDisassembler dasm)
    {
        var bitnum = bf5_5.Read(uInstr);
        dasm.ops.Add(Constant.Byte((byte)bitnum));
        return true;
    }

    private static bool O5(uint uInstr, M88kDisassembler dasm)
    {
        var bitnum = bf0_5.Read(uInstr);
        dasm.ops.Add(Constant.Byte((byte) bitnum));
        return true;
    }


    private static bool VEC9(uint uInstr, M88kDisassembler dasm)
    {
        var bitnum = bf0_9.Read(uInstr);
        dasm.ops.Add(Constant.Word16((ushort) bitnum));
        return true;
    }

    private static bool W5(uint uInstr, M88kDisassembler dasm)
    {
        var bitnum = bf21_5.Read(uInstr);
        dasm.ops.Add(Constant.Byte((byte) bitnum));
        return true;
    }

    private static bool uimm16( uint uInstr, M88kDisassembler dasm)
    {
        var imm = bf0_16.Read(uInstr);
        dasm.ops.Add(Constant.Word32((ushort)imm));
        return true;
    }

    private static bool M5(uint uInstr, M88kDisassembler dasm)
    {
        var cond = bf21_5.Read(uInstr);
        MachineOperand op = cond switch
        {
            0b00010 => ceq0,
            0b01101 => cne0,
            0b00001 => cgt0,
            0b01100 => clt0,
            0b00011 => cge0,
            0b01110 => cle0,
            _ => Constant.Byte((byte) cond)
        };
        dasm.ops.Add(op);
        return true;
    }
    private static readonly MachineOperand ceq0 = ConditionOperand.Create(CCode.eq0);
    private static readonly MachineOperand cne0 = ConditionOperand.Create(CCode.ne0);
    private static readonly MachineOperand cgt0 = ConditionOperand.Create(CCode.gt0);
    private static readonly MachineOperand clt0 = ConditionOperand.Create(CCode.lt0);
    private static readonly MachineOperand cge0 = ConditionOperand.Create(CCode.ge0);
    private static readonly MachineOperand cle0 = ConditionOperand.Create(CCode.le0);

    private static bool D16(uint uInstr, M88kDisassembler dasm)
    {
        var imm = bf0_16.ReadSigned(uInstr);
        imm <<= 2;
        dasm.ops.Add(dasm.addr + imm);
        return true;
    }

    private static bool D26(uint uInstr, M88kDisassembler dasm)
    {
        var imm = bf0_26.ReadSigned(uInstr);
        imm <<= 2;
        dasm.ops.Add(dasm.addr + imm);
        return true;
    }

    private static Mutator<M88kDisassembler> TFloat(Bitfield bf, int shift)
    {
        return (u, d) =>
        {
            var t = bf.Read(u);
            if (t > 1)
                return false;
            d.floatSizes = (d.floatSizes??0) | (t+1) << shift;
            return true;
        };
    }
    private static readonly Mutator<M88kDisassembler> TD = TFloat(bf5_2, 0);
    private static readonly Mutator<M88kDisassembler> T1 = TFloat(bf9_2, 2);
    private static readonly Mutator<M88kDisassembler> T2 = TFloat(bf7_2, 4);

    /// <summary>
    /// Memory access with an unsigned 16-bit offset.
    /// </summary>
    private static Mutator<M88kDisassembler> Muo(PrimitiveType dt)
    {
        return (u, d) =>
        {
            var ireg = bf16_5.Read(u);
            var reg = Registers.GpRegisters[ireg];
            var offset = bf0_16.Read(u);
            var mem = MemoryOperand.Indirect(dt, reg, offset);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<M88kDisassembler> Muo8 = Muo(PrimitiveType.Byte);
    private static readonly Mutator<M88kDisassembler> Muos8 = Muo(PrimitiveType.SByte);
    private static readonly Mutator<M88kDisassembler> Muo16 = Muo(PrimitiveType.Word16);
    private static readonly Mutator<M88kDisassembler> Muos16 = Muo(PrimitiveType.Int16);
    private static readonly Mutator<M88kDisassembler> Muo32 = Muo(PrimitiveType.Word32);
    private static readonly Mutator<M88kDisassembler> Muo64 = Muo(PrimitiveType.Word64);

    private static Mutator<M88kDisassembler> Mix(PrimitiveType dt, bool disallowUserspace = false)
    {
        return (u, d) =>
        {
            var breg = bf16_5.Read(u);
            var ireg = bf0_5.Read(u);
            var b = Registers.GpRegisters[breg];
            var i = Registers.GpRegisters[ireg];
            int scale = Bits.IsBitSet(u, 9)
                ? dt.Size
                : 0;
            d.userSpace = Bits.IsBitSet(u, 8);
            if (d.userSpace && disallowUserspace)
                return false;
            var mem = MemoryOperand.Indexed(dt, b, i, scale);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<M88kDisassembler> Mix8 = Mix(PrimitiveType.Byte);
    private static readonly Mutator<M88kDisassembler> Mix8u = Mix(PrimitiveType.Byte, true);
    private static readonly Mutator<M88kDisassembler> Mix8s = Mix(PrimitiveType.Int8);
    private static readonly Mutator<M88kDisassembler> Mix16 = Mix(PrimitiveType.Word16);
    private static readonly Mutator<M88kDisassembler> Mix16s = Mix(PrimitiveType.Int16);
    private static readonly Mutator<M88kDisassembler> Mix16u = Mix(PrimitiveType.Word16, true);
    private static readonly Mutator<M88kDisassembler> Mix32 = Mix(PrimitiveType.Word32);
    private static readonly Mutator<M88kDisassembler> Mix32u = Mix(PrimitiveType.Word32, true);
    private static readonly Mutator<M88kDisassembler> Mix64 = Mix(PrimitiveType.Word64);
    private static readonly Mutator<M88kDisassembler> Mix64u = Mix(PrimitiveType.Word64, true);


    private static Decoder<M88kDisassembler, Mnemonic, M88kInstruction> Instr(Mnemonic mnemonic, params Mutator<M88kDisassembler>[] operands)
    {
        return new InstrDecoder<M88kDisassembler, Mnemonic, M88kInstruction>(InstrClass.Linear, mnemonic, operands);
    }

    private static Decoder<M88kDisassembler, Mnemonic, M88kInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<M88kDisassembler>[] operands)
    {
        return new InstrDecoder<M88kDisassembler, Mnemonic, M88kInstruction>(iclass, mnemonic, operands);
    }

    static M88kDisassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        var add = Mask(8, 2, "  add",
            Instr(Mnemonic.add, D, S1, S2),
            Instr(Mnemonic.add_co, D, S1, S2),
            Instr(Mnemonic.add_ci, D, S1, S2),
            Instr(Mnemonic.add_cio, D, S1, S2));

        var addu = Mask(8, 2, "  addu",
            Instr(Mnemonic.addu, D, S1, S2),
            Instr(Mnemonic.addu_co, D, S1, S2),
            Instr(Mnemonic.addu_ci, D, S1, S2),
            Instr(Mnemonic.addu_cio, D, S1, S2));

        var cmp = Mask(8, 2, "  cmp",
            Instr(Mnemonic.cmp, D, S1, S2),
            invalid,
            invalid,
            invalid);

        var div = Mask(8, 2, "  div",
            Instr(Mnemonic.div, D, S1, S2),
            invalid,
            invalid,
            invalid);

        var divu = Mask(8, 2, "  divu",
            Instr(Mnemonic.divu, D, S1, S2),
            invalid,
            invalid,
            invalid);

        var mul = Mask(8, 2, "  mul",
            Instr(Mnemonic.mul, D, S1, S2),
            invalid,
            invalid,
            invalid);

        var sub = Mask(8, 2, "  sub",
            Instr(Mnemonic.sub, D, S1, S2),
            Instr(Mnemonic.sub_co, D, S1, S2),
            Instr(Mnemonic.sub_ci, D, S1, S2),
            Instr(Mnemonic.sub_cio, D, S1, S2));

        var subu = Mask(8, 2, "  subu",
            Instr(Mnemonic.subu, D, S1, S2),
            Instr(Mnemonic.subu_co, D, S1, S2),
            Instr(Mnemonic.subu_ci, D, S1, S2),
            Instr(Mnemonic.subu_cio, D, S1, S2));


        var decode100000 = Sparse(11, 5, "  100000", invalid,
            (0b01000, Instr(Mnemonic.ldcr, D, cr)),
            (0b01001, Instr(Mnemonic.fldcr, D, fcr)),
            (0b10000, Instr(Mnemonic.stcr, cr, S1)),
            (0b10001, Instr(Mnemonic.fstcr, fcr, S1)),
            (0b11000, Instr(Mnemonic.xcr, D, S1, cr)),
            (0b11001, Instr(Mnemonic.fxcr, D, S1, fcr)));

        var decode100001 = Sparse(11, 5, "  100001", invalid,
            (0b00000, Instr(Mnemonic.fmul, Dn0, S1, S2, T1, T2, TD)),
            (0b00100, Instr(Mnemonic.flt, Dn0, S2, TD)),
            (0b00101, Instr(Mnemonic.fadd, Dn0, S1, S2, T1, T2, TD)),
            (0b00110, Instr(Mnemonic.fsub, Dn0, S1, S2, T1, T2, TD)),
            (0b00111, Instr(Mnemonic.fcmp, Dn0, S1, S2, T1, T2, TD)),
            (0b01001, Instr(Mnemonic.@int, Dn0, S2, T2)),
            (0b01010, Instr(Mnemonic.nint, Dn0, S2, T2)),
            (0b01011, Instr(Mnemonic.trnc, Dn0, S2, T2)),
            (0b01110, Instr(Mnemonic.fdiv, Dn0, S1, S2, T1, T2, TD)));

        var decode111100 = Sparse(10, 6, "  111100", invalid,
            (0b100000, Instr(Mnemonic.clr, D, S1, W5, O5)),
            (0b100010, Instr(Mnemonic.set, D, S1, W5, O5)),
            (0b100100, Instr(Mnemonic.ext, D, S1, W5, O5)),
            (0b100110, Instr(Mnemonic.extu, D, S1, W5, O5)),
            (0b101000, Instr(Mnemonic.mak, D, S1, W5, O5)),
            (0b101010, Instr(Mnemonic.rot, D, S1, O5)),
            (0b110100, Instr(Mnemonic.tb0, B5, S1, VEC9)),
            (0b110110, Instr(Mnemonic.tb1, B5, S1, VEC9)),
            (0b111010, Instr(Mnemonic.tcnd, M5, S1, VEC9)));

        var decode111101 = Sparse(10, 6, "  111101", invalid,
            (0b000000, Instr(Mnemonic.xmem, D, Mix32)),
            (0b000001, Instr(Mnemonic.xmem_bu, D, Mix8)),
            (0b000010, Instr(Mnemonic.ld_hu, D, Mix16)),
            (0b000011, Instr(Mnemonic.ld_bu, D, Mix8)),

            (0b000100, Instr(Mnemonic.ld_d, D, Mix64)),
            (0b000101, Instr(Mnemonic.ld, D, Mix32)),
            (0b000110, Instr(Mnemonic.ld_h, D, Mix16s)),
            (0b000111, Instr(Mnemonic.ld_b, D, Mix8s)),

            (0b001000, Instr(Mnemonic.st_d, D, Mix64)),
            (0b001001, Instr(Mnemonic.st, D, Mix32)),
            (0b001010, Instr(Mnemonic.st_h, D, Mix16)),
            (0b001011, Instr(Mnemonic.st_b, D, Mix8)),

            (0b001100, Instr(Mnemonic.lda_d, D, Mix64u)),
            (0b001101, Instr(Mnemonic.lda, D, Mix32u)),
            (0b001110, Instr(Mnemonic.lda_h, D, Mix16u)),
            (0b001111, Instr(Mnemonic.lda_b, D, Mix8u)),

            (0b010000, Instr(Mnemonic.and, D, S1, S2)),
            (0b010001, Instr(Mnemonic.and_c, D, S1, S2)),
            (0b010100, Instr(Mnemonic.xor, D, S1, S2)),
            (0b010101, Instr(Mnemonic.xor_c, D, S1, S2)),
            (0b010110, Instr(Mnemonic.or, D, S1, S2)),
            (0b010111, Instr(Mnemonic.or_c, D, S1, S2)),
            (0b011000, addu),
            (0b011001, subu),
            (0b011010, divu),
            (0b011011, mul),
            (0b011100, add),
            (0b011101, sub),
            (0b011110, div),
            (0b011111, cmp),
            (0b100000, Instr(Mnemonic.clr, D, S1, S2)),
            (0b100010, Instr(Mnemonic.set, D, S1, S2)),
            (0b100100, Instr(Mnemonic.ext, D, S1, S2)),
            (0b100110, Instr(Mnemonic.extu, D, S1, S2)),
            (0b101000, Instr(Mnemonic.mak, D, S1, S2)),
            (0b101010, Instr(Mnemonic.rot, D, S1, S2)),

            (0b110000, Instr(Mnemonic.jmp, InstrClass.Transfer, S2)),
            (0b110001, Instr(Mnemonic.jmp_n, InstrClass.Transfer | InstrClass.Delay, S2)),
            (0b110010, Instr(Mnemonic.jsr, InstrClass.Transfer | InstrClass.Call, S2)),
            (0b110011, Instr(Mnemonic.jsr_n, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay, S2)),

            (0b111010, Instr(Mnemonic.ff1, D, S2)),
            (0b111011, Instr(Mnemonic.ff0, D, S2)),
            (0b111110, Instr(Mnemonic.tbnd, S1, S2)),
            (0b111111, Instr(Mnemonic.rte, InstrClass.Transfer | InstrClass.Return | InstrClass.Privileged)));


        rootDecoder = Mask(26, 6, "M88k",
            Instr(Mnemonic.xmem_bu, D, Muo8),
            Instr(Mnemonic.xmem, D, Muo32),
            Instr(Mnemonic.ld_hu, D, Muo16),
            Instr(Mnemonic.ld_bu, D, Muo8),

            Instr(Mnemonic.ld_d, D, Muo64),
            Instr(Mnemonic.ld, D, Muo32),
            Instr(Mnemonic.ld_h, D, Muos16),
            Instr(Mnemonic.ld_b, D, Muos8),

            Instr(Mnemonic.st_d, D, Muo64),
            Instr(Mnemonic.st, D, Muo32),
            Instr(Mnemonic.st_h, D, Muo16),
            Instr(Mnemonic.st_b, D, Muo8),

            Instr(Mnemonic.lda_d, D, Muo64),
            Instr(Mnemonic.lda, D, Muo32),
            Instr(Mnemonic.lda_h, D, Muo16),
            Instr(Mnemonic.lda_b, D, Muo8),

            // 0x10
            Instr(Mnemonic.and, D, S1, uimm16),
            Instr(Mnemonic.and_u, D, S1, uimm16),
            Instr(Mnemonic.mask, D, S1, uimm16),
            Instr(Mnemonic.mask_u, D, S1, uimm16),

            Instr(Mnemonic.xor, D, S1, uimm16),
            Instr(Mnemonic.xor_u, D, S1, uimm16),
            Instr(Mnemonic.or, D, S1, uimm16),
            Instr(Mnemonic.or_u, D, S1, uimm16),

            Instr(Mnemonic.addu, D, S1, uimm16),
            Instr(Mnemonic.subu, D, S1, uimm16),
            Instr(Mnemonic.divu, D, S1, uimm16),
            Instr(Mnemonic.mul, D, S1, uimm16),

            Instr(Mnemonic.add, D, S1, uimm16),
            Instr(Mnemonic.sub, D, S1, uimm16),
            Instr(Mnemonic.div, D, S1, uimm16),
            Instr(Mnemonic.cmp, D, S1, uimm16),

            // 0x20
            decode100000,
            decode100001,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 0x30
            Instr(Mnemonic.br, InstrClass.Transfer, D26),
            Instr(Mnemonic.br_n, InstrClass.Transfer | InstrClass.Delay, D26),
            Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, D26),
            Instr(Mnemonic.bsr_n, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay, D26),

            Instr(Mnemonic.bb0, InstrClass.ConditionalTransfer, B5, S1, D16),
            Instr(Mnemonic.bb0_n, InstrClass.ConditionalTransfer | InstrClass.Delay, B5, S1, D16),
            Instr(Mnemonic.bb1, InstrClass.ConditionalTransfer, B5, S1, D16),
            Instr(Mnemonic.bb1_n, InstrClass.ConditionalTransfer | InstrClass.Delay, B5, S1, D16),

            invalid,
            invalid,
            Instr(Mnemonic.bcnd, InstrClass.ConditionalTransfer, M5, S1, D16),
            Instr(Mnemonic.bcnd_n, InstrClass.ConditionalTransfer | InstrClass.Delay, M5, S1, D16),

            decode111100,
            decode111101,
            Instr(Mnemonic.tbnd, S1, uimm16),
            invalid);
    }
}
