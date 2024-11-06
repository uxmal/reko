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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.M16C;

public class M16CDisassembler : DisassemblerBase<M16CInstruction, Mnemonic>
{
    private static readonly Decoder<M16CDisassembler, Mnemonic, M16CInstruction> rootDecoder;
    private static readonly Bitfield bf0L2 = new(0, 2);
    private static readonly Bitfield bf0L3 = new(0, 3);
    private static readonly Bitfield bf0L4 = new(0, 4);
    private static readonly Bitfield bf0L6 = new(0, 6);
    private static readonly Bitfield bf4L3 = new(4, 3);
    private static readonly Bitfield bf4L4 = new(4, 4);
    private static readonly RegisterStorage[] pushmreg = new[]
    {
        Registers.r0,
        Registers.r1,
        Registers.r2,
        Registers.r3,
        Registers.a0,
        Registers.a1,
        Registers.sb,
        Registers.fb,
    };
    private static readonly FlagGroupStorage[] flagBits = new[]
    {
        Registers.C,
        Registers.D,
        Registers.Z,
        Registers.S,
        Registers.B,
        Registers.O,
        Registers.I,
        Registers.U,
    };
    private static readonly RegisterStorage[] w16registers = new[]
    {
        Registers.r0,
        Registers.r1,
        Registers.r2,
        Registers.r3,
    };



    private readonly M16CArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private bool IsWord;
    private SizeSuffix sizeSuffix;
    private InstrSuffix instrSuffix;
    private Mnemonic overriddenMnemonic;

    public M16CDisassembler(M16CArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = new List<MachineOperand>();
        this.addr = default!;
    }

    public override M16CInstruction? DisassembleInstruction()
    {
        var offset = rdr.Offset;
        this.addr = rdr.Address;
        if (!rdr.TryReadByte(out var uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        instr.Length = (int) (rdr.Offset - offset);
        instr.Address = addr;

        sizeSuffix = SizeSuffix.None;
        instrSuffix = InstrSuffix.None;
        overriddenMnemonic = Mnemonic.Invalid;
        ops.Clear();

        return instr;
    }

    public override M16CInstruction CreateInvalidInstruction()
    {
        return new M16CInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override M16CInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new M16CInstruction
        {
            InstructionClass = iclass,
            Mnemonic = this.overriddenMnemonic != Mnemonic.Invalid
                ? this.overriddenMnemonic
                : mnemonic,
            SizeSuffix = sizeSuffix,
            InstrSuffix = instrSuffix,
            Operands = ops.ToArray()
        };
    }

    public override M16CInstruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("M16cDis", this.addr, this.rdr, message);
        return new M16CInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.nyi,
        };
    }

    private PrimitiveType DataSize()
    {
        return this.sizeSuffix switch
        {
            SizeSuffix.B => PrimitiveType.Byte,
            SizeSuffix.W => PrimitiveType.Word16,
            SizeSuffix.A => M16CArchitecture.Word20,
            _ => throw new NotImplementedException(),
        };
    }

    private PrimitiveType SDataSize()
    {
        return this.sizeSuffix == SizeSuffix.B
                ? PrimitiveType.Int8
                : PrimitiveType.Int16;
    }

    private static Mutator<M16CDisassembler> Register(RegisterStorage reg)
    {
        return (u, d) =>
        {
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> a0 = Register(Registers.a0);
    private static readonly Mutator<M16CDisassembler> a1 = Register(Registers.a1);
    private static readonly Mutator<M16CDisassembler> r0 = Register(Registers.r0);
    private static readonly Mutator<M16CDisassembler> r0h = Register(Registers.r0h);
    private static readonly Mutator<M16CDisassembler> r0l = Register(Registers.r0l);
    private static readonly Mutator<M16CDisassembler> r1 = Register(Registers.r1);
    private static readonly Mutator<M16CDisassembler> r1h = Register(Registers.r1h);
    private static readonly Mutator<M16CDisassembler> sp = Register(Registers.usp);

    private static Mutator<M16CDisassembler> RegisterPair(SequenceStorage seq)
    {
        return (u, d) =>
        {
            d.ops.Add(seq);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> r2r0 = RegisterPair(Registers.r2r0);
    private static readonly Mutator<M16CDisassembler> r3r1 = RegisterPair(Registers.r3r1);


    private static bool cFlag(uint uInstr, M16CDisassembler dasm)
    {
        dasm.ops.Add(Registers.C);
        return true;
    }

    /// <summary>
    /// Decode an immediate value based on the current size,
    /// and inserts it as the first operand.
    /// </summary>
    private static bool imm(uint uInstr, M16CDisassembler dasm)
    {
        if (dasm.sizeSuffix == SizeSuffix.B)
        {
            if (!dasm.rdr.TryReadByte(out var b))
                return false;
            var imm = ImmediateOperand.Byte(b);
            dasm.ops.Insert(0, imm);
            return true;
        }
        if (dasm.sizeSuffix == SizeSuffix.W)
        {
            if (!dasm.rdr.TryReadLeUInt16(out var w))
                return false;
            var imm = ImmediateOperand.Word16(w);
            dasm.ops.Insert(0, imm);
            return true;
        }
        throw new NotImplementedException("No size suffix?");
    }

    /// <summary>
    /// Decode an 8-bit immediate value and inserts it at
    /// position <paramref name="iop"/> in the operand list.
    /// </summary>
    private static Mutator<M16CDisassembler> imm8(int iop)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadByte(out var b))
                return false;
            var imm = ImmediateOperand.Byte(b);
            d.ops.Insert(iop, imm);
            return true;
        };
    }

    /// <summary>
    /// Decode an 16-bit immediate value and inserts it at
    /// position <paramref name="iop"/> in the operand list.
    /// </summary>
    private static Mutator<M16CDisassembler> imm16(int iop)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadLeUInt16(out var w))
                return false;
            var imm = ImmediateOperand.Word16(w);
            d.ops.Insert(iop, imm);
            return true;
        };
    }

    /// <summary>
    /// Decode a signed immediate value based on the current size,
    /// and inserts it as the first operand.
    /// </summary>
    private static bool simm(uint uInstr, M16CDisassembler dasm)
    {
        if (dasm.sizeSuffix == SizeSuffix.B)
        {
            if (!dasm.rdr.TryReadByte(out var b))
                return false;
            var imm = ImmediateOperand.SByte((sbyte) b);
            dasm.ops.Insert(0, imm);
            return true;
        }
        if (dasm.sizeSuffix == SizeSuffix.W)
        {
            if (!dasm.rdr.TryReadLeInt16(out var w))
                return false;
            var imm = ImmediateOperand.Int16(w);
            dasm.ops.Insert(0, imm);
            return true;
        }
        throw new NotImplementedException("No size suffix?");
    }

    /// <summary>
    /// Generate an immediate 0 operand implicitly
    /// specified by the current instruction.
    /// </summary>
    private static bool zero(uint uInstr, M16CDisassembler dasm)
    {
        dasm.instrSuffix = InstrSuffix.Z;
        if (dasm.sizeSuffix == SizeSuffix.B)
        {
            var imm = ImmediateOperand.Byte(0);
            dasm.ops.Insert(0, imm);
            return true;
        }
        if (dasm.sizeSuffix == SizeSuffix.W)
        {
            var imm = ImmediateOperand.Word16(0);
            dasm.ops.Insert(0, imm);
            return true;
        }
        throw new NotImplementedException("No size suffix?");
    }

    /// <summary>
    /// Decode an immediate value based on the current size,
    /// and inserts it as the first operand.
    /// </summary>
    private static Mutator<M16CDisassembler> immq(Bitfield bf)
    {
        return (uint uInstr, M16CDisassembler dasm) =>
        {
            var imm = bf.ReadSigned(uInstr);
            var dt = dasm.sizeSuffix == SizeSuffix.B
                ? PrimitiveType.Byte
                : PrimitiveType.Word16;
            var op = ImmediateOperand.Create(Constant.Create(dt, imm));
            dasm.ops.Add(op);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> immq_0 = immq(bf0L4);
    private static readonly Mutator<M16CDisassembler> immq_4 = immq(bf4L4);

    /// <summary>
    /// Decode an immediate value from the bottom 3 bits of the
    /// opcode.
    /// </summary>
    private static bool immq3(uint uInstr, M16CDisassembler dasm)
    {
        var imm = bf0L3.Read(uInstr);
        var op = ImmediateOperand.Byte((byte) imm);
        dasm.ops.Add(op);
        return true;
    }

    /// <summary>
    /// Decode an immediate value from the bottom 6 bits of the
    /// opcode.
    /// </summary>
    private static bool immq6(uint uInstr, M16CDisassembler dasm)
    {
        var imm = bf0L6.Read(uInstr);
        var op = ImmediateOperand.Byte((byte) imm);
        dasm.ops.Add(op);
        return true;
    }


    /// <summary>
    /// Decode a signed immediate value based on the current size,
    /// and inserts it as the first operand.
    /// </summary>
    private static Mutator<M16CDisassembler> simmq(Bitfield bf)
    {
        return (uint uInstr, M16CDisassembler dasm) =>
        {
            var imm = bf.ReadSigned(uInstr);
            dasm.instrSuffix = InstrSuffix.Q;
            var dt = dasm.SDataSize();
            var op = ImmediateOperand.Create(Constant.Create(dt, imm));
            dasm.ops.Add(op);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> simmq_0 = simmq(bf0L4);
    private static readonly Mutator<M16CDisassembler> simmq_4 = simmq(bf4L4);

    private static bool grf4L3(uint uInstr, M16CDisassembler dasm)
    {
        var code = bf4L3.Read(uInstr);
        var grf = flagBits[code];
        dasm.ops.Add(grf);
        return true;
    }

    private static bool abs16(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort w))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Direct(dt, w);
        dasm.ops.Add(op);
        return true;
    }

    private static Mutator<M16CDisassembler> abs20(int iop)
    {
        return (uInstr, dasm) =>
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort lo))
                return false;
            if (!dasm.rdr.TryReadByte(out byte hi))
                return false;
            var value = ((hi & 0xF) << 16) | lo;
            var dt = dasm.DataSize();
            var op = MemoryOperand.Direct(dt, value);
            dasm.ops.Insert(iop, op);
            return true;
        };
    }

    private static bool addr20(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort lo))
            return false;
        if (!dasm.rdr.TryReadByte(out byte hi))
            return false;
        var value = ((hi & 0xFu) << 16) | lo;
        var dt = dasm.DataSize();
        var op = Address.Ptr32(value);
        dasm.ops.Add(op);
        return true;
    }


    private static Mutator<M16CDisassembler> indA1A0(int iop)
    {
        return (uint uInstr, M16CDisassembler dasm) =>
        {
            var dt = dasm.DataSize();
            var op = MemoryOperand.Indirect(dt, Registers.a1a0);
            dasm.ops.Insert(iop, op);
            return true;
        };
    }

    private static bool dsp8_a0(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.a0, b);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp8_a1(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.a1, b);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp16_a0(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort w))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.a0, w);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp16_a1(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort w))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.a1, w);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp20_a0(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort lo))
            return false;
        if (!dasm.rdr.TryReadByte(out byte hi))
            return false;
        var dt = dasm.DataSize();
        var disp = ((hi & 0xF) << 16) | lo;
        var op = MemoryOperand.Indirect(dt, Registers.a0, disp);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp20_a1(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort lo))
            return false;
        if (!dasm.rdr.TryReadByte(out byte hi))
            return false;
        var dt = dasm.DataSize();
        var disp = ((hi & 0xF) << 16) | lo;
        var op = MemoryOperand.Indirect(dt, Registers.a1, disp);
        dasm.ops.Add(op);
        return true;
    }

    /// <summary>
    /// Generate a signed eight-bit displacement from FB.
    /// </summary>
    private static bool dsp8_fb(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.fb, (sbyte) b);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp8_sb(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.sb, b);
        dasm.ops.Add(op);
        return true;
    }

    private static bool dsp16_sb(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort w))
            return false;
        var dt = dasm.DataSize();
        var op = MemoryOperand.Indirect(dt, Registers.sb, w);
        dasm.ops.Add(op);
        return true;
    }

    private static bool bitdsp8_sb(uint uInstr, M16CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var op = MemoryOperand.Indirect(PrimitiveType.Byte, Registers.sb, b);
        dasm.ops.Add(op);
        return true;
    }

    /// <summary>
    /// Select an operand from 2 bits
    /// </summary>
    private static Mutator<M16CDisassembler> op2(RegisterStorage reg00)
    {
        return (u, d) =>
        {
            var opcode = bf0L2.Read(u);
            MachineOperand op;
            switch (opcode)
            {
            case 0: op = reg00; break;
            case 1:
                if (!d.rdr.TryReadByte(out byte b))
                    return false;
                op = MemoryOperand.Indirect(PrimitiveType.Byte, Registers.sb, b);
                break;
            case 2:
                if (!d.rdr.TryReadByte(out b))
                    return false;
                op = MemoryOperand.Indirect(PrimitiveType.Byte, Registers.fb, (sbyte) b);
                break;
            default:
                if (!d.rdr.TryReadLeUInt16(out ushort w))
                    return false;
                op = MemoryOperand.Direct(PrimitiveType.Byte, w);
                break;
            };
            d.ops.Add(op);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> op2_h = op2(Registers.r0h);
    private static readonly Mutator<M16CDisassembler> op2_l = op2(Registers.r0l);

    /// <summary>
    /// Select an operand from 3 bits.
    /// </summary>
    /// <param name="bitPos"></param>
    private static Mutator<M16CDisassembler> op3(int bitPos)
    {
        var bf = new Bitfield(bitPos, 3);
        return (u, d) =>
        {
            var destCode = bf.Read(u);
            var dt = d.sizeSuffix == SizeSuffix.B
                ? PrimitiveType.Byte
                : PrimitiveType.Word16;
            MachineOperand op;
            switch (destCode)
            {
            default: return false;
            case 3:
                op = Registers.r0h;
                break;
            case 4:
                op = Registers.r0l;
                break;
            case 5:
                if (!d.rdr.TryReadByte(out var b))
                    return false;
                op = MemoryOperand.Indirect(dt, Registers.sb, b);
                break;
            case 6:
                if (!d.rdr.TryReadByte(out b))
                    return false;
                op = MemoryOperand.Indirect(dt, Registers.fb, (sbyte) b);
                break;
            case 7:
                if (!d.rdr.TryReadUInt16(out var w))
                    return false;
                op = MemoryOperand.Direct(dt, w);
                break;
            }
            d.ops.Add(op);
            return true;
        };
    }

    /// <summary>
    /// Select an destination address mode from 4 bits.
    /// </summary>
    /// <param name="bitPos"></param>
    private static Mutator<M16CDisassembler> op4(int bitPos)
    {
        var bf = new Bitfield(bitPos, 4);
        return (u, d) =>
        {
            var destCode = bf.Read(u);
            var dt = d.sizeSuffix == SizeSuffix.B
                ? PrimitiveType.Byte
                : PrimitiveType.Word16;
            MachineOperand op;
            switch (destCode)
            {
            case 0:
                if (d.sizeSuffix == SizeSuffix.B)
                    op = Registers.r0l;
                else
                    op = Registers.r0;
                break;
            case 1:
                if (d.sizeSuffix == SizeSuffix.B)
                    op = Registers.r0h;
                else
                    op = Registers.r1;
                break;
            case 2:
                if (d.sizeSuffix == SizeSuffix.B)
                    op = Registers.r1l;
                else
                    op = Registers.r2;
                break;
            case 3:
                if (d.sizeSuffix == SizeSuffix.B)
                    op = Registers.r1h;
                else
                    op = Registers.r3;
                break;
            case 4:
                op = Registers.a0;
                break;
            case 5:
                op = Registers.a1;
                break;
            case 6:
                op = MemoryOperand.Indirect(dt, Registers.a0);
                break;
            case 7:
                op = MemoryOperand.Indirect(dt, Registers.a1);
                break;
            case 8: // unsigned dsp8
                return dsp8_a0(u, d);
            case 9: // unsigned dsp8
                return dsp8_a1(u, d);
            case 10: // unsigned dsp8[sb]
                return dsp8_sb(u, d);
            case 11: // signed dsp8[fb]
                return dsp8_fb(u, d);
            default:
                return abs16(u, d);
            }
            d.ops.Add(op);
            return true;
        };
    }

    /// <summary>
    /// Decode a jmp operand at position 0.
    /// </summary>
    /// <param name="uInstr"></param>
    /// <param name="dasm"></param>
    /// <returns></returns>
    private static bool jmpop4(uint uInstr, M16CDisassembler dasm)
    {
        var code = bf0L4.Read(uInstr);
        MachineOperand op;
        var dt = dasm.DataSize();
        switch (code)
        {
        case 0x0: op = Registers.r0; break;
        case 0x1: op = Registers.r1; break;
        case 0x2: op = Registers.r2; break;
        case 0x3: op = Registers.r3; break;
        case 0x4: op = Registers.a0; break;
        case 0x5: op = Registers.a1; break;
        case 0x6: op = MemoryOperand.Indirect(dt, Registers.a0); break;
        case 0x7: op = MemoryOperand.Indirect(dt, Registers.a1); break;
        case 0x8: return dsp8_a0(uInstr, dasm);
        case 0x9: return dsp8_a1(uInstr, dasm);
        case 0xA: return dsp8_sb(uInstr, dasm);
        case 0xB: return dsp8_fb(uInstr, dasm);
        case 0xC: return dsp20_a0(uInstr, dasm);
        case 0xD: return dsp20_a1(uInstr, dasm);
        case 0xE: return dsp16_sb(uInstr, dasm);
        default:
        case 0xF: return abs16(uInstr, dasm);
        }
        dasm.ops.Add(op);
        return true;
    }


    private static bool jmpaop4(uint uInstr, M16CDisassembler dasm)
    {
        var code = bf0L4.Read(uInstr);
        MachineOperand op;
        var dt = dasm.DataSize();
        switch (code)
        {
        case 0x0: op = Registers.r2r0; break;
        case 0x1: op = Registers.r3r1; break;
        case 0x4: op = Registers.a1a0; break;
        case 0x6: op = MemoryOperand.Indirect(dt, Registers.a0); break;
        case 0x7: op = MemoryOperand.Indirect(dt, Registers.a1); break;
        case 0x8: return dsp8_a0(uInstr, dasm);
        case 0x9: return dsp8_a1(uInstr, dasm);
        case 0xA: return dsp8_sb(uInstr, dasm);
        case 0xB: return dsp8_fb(uInstr, dasm);
        case 0xC: return dsp20_a0(uInstr, dasm);
        case 0xD: return dsp20_a1(uInstr, dasm);
        case 0xE: return dsp16_sb(uInstr, dasm);
        case 0xF: return abs16(uInstr, dasm);
        default: return false;
        }
        dasm.ops.Add(op);
        return true;
    }

    /// <summary>
    /// Decode a bit operand at bit position 0.
    /// </summary>
    private static bool bitop4(uint uInstr, M16CDisassembler dasm)
    {
        var code = bf0L4.Read(uInstr);
        MachineOperand op;
        switch (code)
        {
        case 0x0:
        case 0x1:
        case 0x2:
        case 0x3:
            //$REVIEW: the manual is really hard to interpret. Assume
            // the bit number is in a byte following the instruction.
            if (!dasm.rdr.TryReadByte(out var b))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(b));
            dasm.ops.Add(w16registers[code]);
            return true;
        case 0x6:
            op = MemoryOperand.Indirect(PrimitiveType.Byte, Registers.a0);
            break;
        case 0x7:
            op = MemoryOperand.Indirect(PrimitiveType.Byte, Registers.a1);
            break;
        case 0xF:
            if (!dasm.rdr.TryReadUInt16(out var w))
                return false;
            op = MemoryOperand.Direct(PrimitiveType.Byte, w);
            break;
        default:
            dasm.NotYetImplemented("bitop4");
            return false;
        }
        dasm.ops.Add(op);
        return true;
    }

    /// <summary>
    /// Decode the operand used by the multiplication destination.
    /// </summary>
    public static bool mulop4(uint uInstr, M16CDisassembler dasm)
    {
        var destCode = bf0L4.Read(uInstr);
        var dt = dasm.DataSize();
        bool isByte = dt.BitSize == 8;
        MachineOperand op;
        switch (destCode)
        {
        case 0:
            op = isByte ? Registers.r0l : Registers.r0;
            break;
        case 1:
            if (isByte)
                return false;
            op = Registers.r1;
            break;
        case 2:
            if (!isByte)
                return false;
            op = Registers.r1l;
            break;
        case 3: return false;
        case 4:
            op = Registers.a0;
            break;
        case 5: return false;
        case 6:
            op = MemoryOperand.Indirect(dt, Registers.a0);
            break;
        case 7:
            op = MemoryOperand.Indirect(dt, Registers.a0);
            break;

        case 8:
            return dsp8_a0(uInstr, dasm);
        case 9:
            return dsp8_a1(uInstr, dasm);
        case 10:
            return dsp8_sb(uInstr, dasm);
        case 11:
            return dsp8_fb(uInstr, dasm);
        case 12:
            return dsp16_a0(uInstr, dasm);
        case 13:
            return dsp16_a1(uInstr, dasm);
        case 14:
            return dsp16_sb(uInstr, dasm);
        case 15:
        default:
            return abs16(uInstr, dasm);
        }
        dasm.ops.Add(op);
        return true;
    }

    public static bool rotop4(uint uInstr, M16CDisassembler dasm)
    {
        var destCode = bf0L4.Read(uInstr);
        var dt = dasm.DataSize();
        bool isByte = dt.BitSize == 8;
        MachineOperand op;
        switch (destCode)
        {
        case 0:
            op = isByte ? Registers.r0l : Registers.r0;
            break;
        case 1:
            if (!isByte)
                return false;
            op = Registers.r0h;
            break;
        case 2:
            op = isByte ? Registers.r1l : Registers.r2;
            break;
        case 3:
            if (isByte)
                return false;
            op = Registers.r3;
            break;
        case 4:
            op = Registers.a0;
            break;
        case 5: return false;
        case 6:
            op = MemoryOperand.Indirect(dt, Registers.a0);
            break;
        case 7:
            op = MemoryOperand.Indirect(dt, Registers.a0);
            break;

        case 8:
            return dsp8_a0(uInstr, dasm);
        case 9:
            return dsp8_a1(uInstr, dasm);
        case 10:
            return dsp8_sb(uInstr, dasm);
        case 11:
            return dsp8_fb(uInstr, dasm);
        case 12:
            return dsp16_a0(uInstr, dasm);
        case 13:
            return dsp16_a1(uInstr, dasm);
        case 14:
            return dsp16_sb(uInstr, dasm);
        case 15:
        default:
            return abs16(uInstr, dasm);
        }
        dasm.ops.Add(op);
        return true;
    }

    private static bool movareg(uint uInstr, M16CDisassembler dasm)
    {
        var regCode = bf4L3.Read(uInstr);
        MachineOperand op;
        switch (regCode)
        {
        case 0: op = Registers.r0; break;
        case 1: op = Registers.r1; break;
        case 2: op = Registers.r2; break;
        case 3: op = Registers.r3; break;
        case 4: op = Registers.a0; break;
        case 5: op = Registers.a1; break;
        default: return false;
        }
        dasm.ops.Add(op);
        return true;
    }

    private static bool regc(uint uInstr, M16CDisassembler dasm)
    {
        var regCode = bf4L3.Read(uInstr);
        MachineOperand op;
        switch (regCode)
        {
        case 1: op = Registers.intbl; break;
        case 2: op = Registers.intbh; break;
        case 3: op = Registers.flg; break;
        case 4: op = Registers.isp; break;
        case 5: op = Registers.usp; break;
        case 6: op = Registers.sb; break;
        case 7: op = Registers.fb; break;
        default: return false;
        }
        dasm.ops.Add(op);
        return true;

    }

    private static Mutator<M16CDisassembler> mnemonicOverrideq(
        Bitfield bf,
        Dictionary<byte, Mnemonic> dict)
    {
        return (u, d) =>
        {
            var b = (byte)bf.Read(u);
            if (!dict.TryGetValue(b, out var mnemonic))
                return false;
            d.overriddenMnemonic = mnemonic;
            return true;
        };
    }

    /// <summary>
    /// Reads a byte and uses that to override the mnemonic
    /// for this instruction, based on looking it up in the provided
    /// dictionary.
    /// </summary>
    private static Mutator<M16CDisassembler> mnemonicOverride(
        Dictionary<byte, Mnemonic> dict)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadByte(out byte b))
                return false;
            if (!dict.TryGetValue(b, out var mnemonic))
                return false;
            d.overriddenMnemonic = mnemonic;
            return true;
        };
    }

    /// <summary>
    /// Read the operand size from the given bit position
    /// </summary>
    private static Mutator<M16CDisassembler> sz(int bitPos)
    {
        var bit = 1u << bitPos;
        return (u, d) =>
        {
            d.sizeSuffix = (u & bit) != 0 ? SizeSuffix.W : SizeSuffix.B;
            return true;
        };
    }

    private static bool szA(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.sizeSuffix = SizeSuffix.A;
        return true;
    }

    private static bool szB(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.sizeSuffix = SizeSuffix.B;
        return true;
    }

    private static bool szW(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.sizeSuffix = SizeSuffix.W;
        return true;
    }

    private static bool szL(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.sizeSuffix = SizeSuffix.L;
        return true;
    }

    private static bool Q(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.instrSuffix = InstrSuffix.Q;
        return true;
    }

    private static bool S(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.instrSuffix = InstrSuffix.S;
        return true;
    }

    private static bool Z(uint uUnstr, M16CDisassembler dasm)
    {
        dasm.instrSuffix = InstrSuffix.Z;
        return true;
    }
    private static Mutator<M16CDisassembler> label(int offset, int bitsize)
    {
        return (u, d) =>
        {
            int displacement;
            if (bitsize == 8)
            {
                if (!d.rdr.TryReadByte(out byte b))
                    return false;
                displacement = offset + (sbyte) b;
            }
            else if (bitsize == 16)
            {
                if (!d.rdr.TryReadLeInt16(out short s))
                    return false;
                displacement = offset + s;
            }
            else
                throw new NotImplementedException();
            d.ops.Add(d.addr + displacement);
            return true;
        };
    }
    private static readonly Mutator<M16CDisassembler> labelP1_8 = label(1, 8);
    private static readonly Mutator<M16CDisassembler> labelP1_16 = label(1, 16);
    private static readonly Mutator<M16CDisassembler> labelP2_8 = label(2, 8);


    /// <summary>
    /// Special displacement used by jmp.s
    /// </summary>
    private static bool labelP2_3(uint uInstr, M16CDisassembler dasm)
    {
        var displacement = bf0L3.ReadSigned(uInstr) + 2;
        dasm.ops.Add(dasm.addr + displacement);
        return true;
    }

    private static Mutator<M16CDisassembler> multireg(RegisterStorage[] regs, bool reverse)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadByte(out byte b))
                return false;
            var op = new MultiRegisterOperand(b, regs, reverse);
            d.ops.Add(op);
            return true;
        };
    }


    private static Decoder<M16CDisassembler, Mnemonic, M16CInstruction> NextByte(Decoder<M16CDisassembler, Mnemonic, M16CInstruction> value)
    {
        return new NextByteDecoder(value);
    }

    private class NextByteDecoder : Decoder<M16CDisassembler, Mnemonic, M16CInstruction>
    {
        private Decoder<M16CDisassembler, Mnemonic, M16CInstruction> decoder;

        public NextByteDecoder(Decoder<M16CDisassembler, Mnemonic, M16CInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override M16CInstruction Decode(uint wInstr, M16CDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return dasm.CreateInvalidInstruction();
            wInstr = (wInstr << 8) | b;
            return decoder.Decode(wInstr, dasm);
        }
    }


    public static Decoder<M16CDisassembler, Mnemonic, M16CInstruction> Instr(Mnemonic mnemonic, params Mutator<M16CDisassembler>[] mutators)
    {
        return new InstrDecoder<M16CDisassembler, Mnemonic, M16CInstruction>(InstrClass.Linear, mnemonic, mutators);
    }

    public static Decoder<M16CDisassembler, Mnemonic, M16CInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<M16CDisassembler>[] mutators)
    {
        return new InstrDecoder<M16CDisassembler, Mnemonic, M16CInstruction>(iclass, mnemonic, mutators);
    }

    static M16CDisassembler()
    {
        var nyi = new NyiDecoder<M16CDisassembler, Mnemonic, M16CInstruction>("nyi");

        var placeHolder = Instr(Mnemonic.nop);

        var bmcndMnemonics = new Dictionary<byte, Mnemonic>
        {
            { 0x00, Mnemonic.bmgeu },
            { 0x01, Mnemonic.bmgtu },
            { 0x02, Mnemonic.bmeq },
            { 0x03, Mnemonic.bmn },
            { 0x04, Mnemonic.bmle },
            { 0x05, Mnemonic.bmo },
            { 0x06, Mnemonic.bmge },
            { 0xF8, Mnemonic.bmltu },
            { 0xF9, Mnemonic.bmleu },
            { 0xFA, Mnemonic.bmne },
            { 0xFB, Mnemonic.bmpz },
            { 0xFC, Mnemonic.bmgt },
            { 0xFD, Mnemonic.bmno },
            { 0xFE, Mnemonic.bmlt },
        };

        var bmcndMnemonicsq = new Dictionary<byte, Mnemonic>
        {
            { 0x0, Mnemonic.bmgeu },
            { 0x1, Mnemonic.bmgtu },
            { 0x2, Mnemonic.bmeq },
            { 0x3, Mnemonic.bmn },
            { 0x4, Mnemonic.bmltu },
            { 0x5, Mnemonic.bmleu },
            { 0x6, Mnemonic.bmne },
            { 0x7, Mnemonic.bmpz },
            { 0x8, Mnemonic.bmle },
            { 0x9, Mnemonic.bmo },
            { 0xA, Mnemonic.bmge },
            { 0xC, Mnemonic.bmgt },
            { 0xD, Mnemonic.bmno },
            { 0xE, Mnemonic.bmlt },
        };



        var decode7C = Mask(6, 2, "  7C",
            nyi, // movdir
            Mask(4, 2, "  07C4..7",
                Instr(Mnemonic.mulu, szB, mulop4, imm8(0)),
                Instr(Mnemonic.mul, szB, mulop4, imm8(0)),
                nyi,
                nyi),
            nyi, // Instr(Mnemonic.exts, szB, op4b),
            Sparse(0, 8, "  0x7C", nyi,
                (0xE0, Instr(Mnemonic.divu, szB, imm8(0))),
                (0xE2, Instr(Mnemonic.push, szB, imm8(0))),
                (0xE1, Instr(Mnemonic.div, szB, imm8(0))),
                (0xE3, Instr(Mnemonic.divx, szB, imm8(0))),
                (0xE4, Instr(Mnemonic.dadd, szB, r0h, r0l)),
                (0xE5, Instr(Mnemonic.dsub, szB, r0h, r0l)),
                (0xE6, Instr(Mnemonic.dadc, szB, r0h, r0l)),
                (0xE7, Instr(Mnemonic.dsbb, szB, r0h, r0l)),
                (0xE8, Instr(Mnemonic.smovf, sz(8))),
                (0xE9, Instr(Mnemonic.smovb, sz(8))),
                (0xEA, Instr(Mnemonic.sstr, sz(8))),
                (0xEB, Instr(Mnemonic.add, sz(8), simm, sp)),
                (0xEC, Instr(Mnemonic.dadd, szB, imm8(0), r0l)),
                (0xED, Instr(Mnemonic.dsub, szB, imm8(0), r0l)),
                (0xEE, Instr(Mnemonic.dadc, szB, imm8(0), r0l)),
                (0xEF, Instr(Mnemonic.dsbb, szB, imm8(0), r0l)),
                (0xF0, Instr(Mnemonic.ldctx, abs16, abs20(1))),
                (0xF1, Instr(Mnemonic.rmpa, sz(8))),
                (0xF2, Instr(Mnemonic.enter, imm8(0)))));

        var decode7D = Mask(4, 4, "  0x7D",
            Instr(Mnemonic.jmpi, InstrClass.Transfer, szA, jmpaop4),
            Instr(Mnemonic.jsri, InstrClass.Transfer | InstrClass.Call, szA, jmpaop4),
            Instr(Mnemonic.jmpi, InstrClass.Transfer, szW, jmpop4),
            Instr(Mnemonic.jsri, InstrClass.Transfer | InstrClass.Call, szW, jmpop4),

            Instr(Mnemonic.mulu, szW, mulop4, imm16(0)),
            Instr(Mnemonic.mul, szW, mulop4, imm16(0)),
            nyi,
            nyi,

            nyi,
            nyi, // pusha
            nyi, // ldipl
            Instr(Mnemonic.add, simmq_0, sp),

            Instr(Mnemonic.nop, labelP2_8, mnemonicOverrideq(
                bf0L4,
                new Dictionary<byte, Mnemonic>
            {
                { 0x8, Mnemonic.jle },
                { 0x9, Mnemonic.jo },
                { 0xA, Mnemonic.jge },
                { 0xC, Mnemonic.jgt },
                { 0xD, Mnemonic.jno },
                { 0xE, Mnemonic.jlt },
            })),
            Instr(Mnemonic.nop, cFlag, mnemonicOverrideq(bf0L4, bmcndMnemonicsq)),
            Sparse(0, 4, "  0x7DE", nyi,
                (0x0, Instr(Mnemonic.divu, szW, imm16(0))),
                (0x1, Instr(Mnemonic.div, szW, imm16(0))),
                (0x2, Instr(Mnemonic.push, szW, imm16(0))),
                (0x3, Instr(Mnemonic.divx, szW, imm16(0))),
                (0x4, Instr(Mnemonic.dadd, szW, r1, r0)),
                (0x5, Instr(Mnemonic.dsub, szW, r1, r0)),
                (0x6, Instr(Mnemonic.dadc, szW, r1, r0)),
                (0x7, Instr(Mnemonic.dsbb, szW, r1, r0)),
                (0x8, Instr(Mnemonic.smovf, sz(8))),
                (0x9, Instr(Mnemonic.smovb, sz(8))),
                (0xA, Instr(Mnemonic.sstr, sz(8))),
                (0xB, Instr(Mnemonic.add, sz(8), simm, sp)),
                (0xC, Instr(Mnemonic.dadd, szW, imm16(0), r0)),
                (0xD, Instr(Mnemonic.dsub, szW, imm16(0), r0)),
                (0xE, Instr(Mnemonic.dadc, szW, imm16(0), r0)),
                (0xF, Instr(Mnemonic.dsbb, szW, imm16(0), r0))),
            Sparse(0, 4, "  0x7DF", nyi,
                (0x0, Instr(Mnemonic.stctx, abs16, abs20(1))),
                (0x1, Instr(Mnemonic.rmpa, sz(8))),
                (0x2, Instr(Mnemonic.exitd)),
                (0x3, Instr(Mnemonic.wait))));

        // MOVA 1110 1011 0 DST3 SRC4
        // POPC 1110 1011 0 DST3 0011
        var decodeEB0 = Mask(0, 4, "  EB0",
            Instr(Mnemonic.ldc, imm16(0), regc),
            Mask(4, 2, "  SHA.L",
                Instr(Mnemonic.shl, szL, r1h, r2r0),
                Instr(Mnemonic.shl, szL, r1h, r3r1),
                Instr(Mnemonic.sha, szL, r1h, r2r0),
                Instr(Mnemonic.sha, szL, r1h, r3r1)),
            nyi, // pushc 
            nyi, // popc Instr(Mnemonic.fset, grf4L3),

            Instr(Mnemonic.fset, grf4L3),
            Instr(Mnemonic.fclr, grf4L3),
            nyi,
            nyi,

            Instr(Mnemonic.mova, movareg, dsp8_a0),
            Instr(Mnemonic.mova, movareg, dsp8_a1),
            Instr(Mnemonic.mova, movareg, dsp8_sb),
            Instr(Mnemonic.mova, movareg, dsp8_fb),
            
            Instr(Mnemonic.mova, movareg, dsp16_a0),
            Instr(Mnemonic.mova, movareg, dsp16_a1),
            Instr(Mnemonic.mova, movareg, dsp16_sb),
            Instr(Mnemonic.mova, movareg, abs16));

        var decodeEB10 = Mask(4, 2, "  EB01",
            Instr(Mnemonic.shl, szL, immq_0, r2r0),
            Instr(Mnemonic.shl, szL, immq_0, r3r1),
            nyi,
            nyi);

        var jCnd = Mask(0, 3, "  jCnd",
            Instr(Mnemonic.jgeu, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jgtu, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jeq, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jn, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jltu, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jleu, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, labelP1_8),
            Instr(Mnemonic.jpz, InstrClass.ConditionalTransfer, labelP1_8));

        var jmp_s = Instr(Mnemonic.jmp, InstrClass.Transfer, labelP2_3);
        var stnz = Instr(Mnemonic.stnz, imm8(0), op3(0));

        var stz = Instr(Mnemonic.stz, imm8(0), op3(0));

        var stzx = Instr(Mnemonic.stzx, imm8(0), op3(0), imm8(1));

        rootDecoder = Mask(1, 7, "M16C", new Decoder<M16CDisassembler, Mnemonic, M16CInstruction>[128]
        {
            // 0x00
            Mask(0, 1, "  00/01",
                Instr(Mnemonic.brk, InstrClass.Terminates|InstrClass.Padding|InstrClass.Zero),
                Instr(Mnemonic.mov, szB, S, r0l, dsp8_sb)),
            Mask(0, 1, "  00/01",
                Instr(Mnemonic.mov, szB, S, r0l, dsp8_fb),
                Instr(Mnemonic.mov, szB, S, r0l, abs16)),
            Mask(0, 1, "  04/05",
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.mov, szB, S, r0h, dsp8_sb)),
            Mask(0, 1, "  06/07",
                Instr(Mnemonic.mov, szB, S, r0h, dsp8_fb),
                Instr(Mnemonic.mov, szB, S, r0h, abs16)),
            Mask(0, 1, "  08/09",
                Instr(Mnemonic.mov, szB, S, r0h, r0l),
                Instr(Mnemonic.mov, szB, S, dsp8_sb, r0l)),
            Mask(0, 1, "  0A/0B",
                Instr(Mnemonic.mov, szB, S, dsp8_fb, r0l),
                Instr(Mnemonic.mov, szB, S, abs16, r0l)),
            Mask(0, 1, "  0C/0D",
                Instr(Mnemonic.mov, szB, S, r0l, r0h),
                Instr(Mnemonic.mov, szB, S, dsp8_sb, r0h)),
            Mask(0, 1, "  0E/0F",
                Instr(Mnemonic.mov, szB, S, dsp8_fb, r0h),
                Instr(Mnemonic.mov, szB, S, abs16, r0h)),

            // 0x10
            Instr(Mnemonic.and, szB, S, op2_h, r0l),
            Instr(Mnemonic.and, szB, S, op2_h, r0l),
            Instr(Mnemonic.and, szB, S, op2_l, r0h),
            Instr(Mnemonic.and, szB, S, op2_l, r0h),
            Instr(Mnemonic.or, szB, S, op2_h, r0l),
            Instr(Mnemonic.or, szB, S, op2_h, r0l),
            Instr(Mnemonic.or, szB, S, op2_l, r0h),
            Instr(Mnemonic.or, szB, S, op2_l, r0h),

            // 0x20
            Instr(Mnemonic.add, szB, S, op2_h, r0l),
            Instr(Mnemonic.add, szB, S, op2_h, r0l),
            Instr(Mnemonic.add, szB, S, op2_l, r0h),
            Instr(Mnemonic.add, szB, S, op2_l, r0h),
            Instr(Mnemonic.sub, szB, S, op2_h, r0l),
            Instr(Mnemonic.sub, szB, S, op2_h, r0l),
            Instr(Mnemonic.sub, szB, S, op2_l, r0h),
            Instr(Mnemonic.sub, szB, S, op2_l, r0h),

            // 0x30
            Instr(Mnemonic.mov, szB, S, op2_h, a0),
            Instr(Mnemonic.mov, szB, S, op2_h, a0),
            Instr(Mnemonic.mov, szB, S, op2_h, a1),
            Instr(Mnemonic.mov, szB, S, op2_h, a1),
            Instr(Mnemonic.cmp, szB, S, op2_h, r0l),
            Instr(Mnemonic.cmp, szB, S, op2_h, r0l),
            Instr(Mnemonic.cmp, szB, S, op2_l, r0h),
            Instr(Mnemonic.cmp, szB, S, op2_l, r0h),

            // 0x40
            Instr(Mnemonic.bclr, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bclr, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bclr, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bclr, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bset, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bset, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bset, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bset, S, immq3, bitdsp8_sb),

            // 0x50
            Instr(Mnemonic.bnot, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bnot, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bnot, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.bnot, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.btst, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.btst, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.btst, S, immq3, bitdsp8_sb),
            Instr(Mnemonic.btst, S, immq3, bitdsp8_sb),

            // 0x60
            jmp_s,
            jmp_s,
            jmp_s,
            jmp_s,
            jCnd,
            jCnd,
            jCnd,
            jCnd,

            // 0x70
            NextByte(Instr(Mnemonic.mulu, sz(8),op4(4), mulop4)),
            NextByte(Instr(Mnemonic.mov, sz(8), op4(4), op4(0))),
            NextByte(Sparse(4, 4, "  0x74/75", nyi,
                (0x0, Instr(Mnemonic.ste, sz(8), op4(0), abs20(1))),
                (0x2, Instr(Mnemonic.ste, sz(8), op4(0), indA1A0(1))),
                (0x3, Instr(Mnemonic.mov, sz(8), op4(0),  dsp8_sb)),
                (0x4, Instr(Mnemonic.push, sz(8), op4(0))),
                (0x5, Instr(Mnemonic.neg, sz(8), op4(0))),
                (0x7, Instr(Mnemonic.not, sz(8), op4(0))),
                (0x6, Instr(Mnemonic.rot, sz(8), r1h, rotop4)),
                (0x8, Instr(Mnemonic.lde, sz(8), op4(0), abs20(0))),
                (0x9, nyi), // Instr(Mnemonic.lde, sz(8), op4(0))),
                (0xA, Instr(Mnemonic.lde, sz(8), op4(0), indA1A0(0))),
                (0xB, Instr(Mnemonic.mov, sz(8), dsp8_sb, op4(0))),
                (0xC, Instr(Mnemonic.mov, sz(8), op4(0), imm)),
                (0xD, Instr(Mnemonic.pop, sz(8), op4(0))),
                (0xF, nyi))), // Instr(Mnemonic.sha, sz(8), op4(0))))),
            NextByte(Sparse(4, 4, "  0x76/77", nyi,
                (0x0, Instr(Mnemonic.test, sz(8), op4(0), imm)),
                (0x1, Instr(Mnemonic.xor, sz(8), op4(0), imm)),
                (0x2, Instr(Mnemonic.and, sz(8), op4(0), imm)),
                (0x3, Instr(Mnemonic.or, sz(8), op4(0), imm)),
                (0x4, Instr(Mnemonic.add, sz(8), op4(0), imm)),
                (0x5, Instr(Mnemonic.sub, sz(8), op4(0), imm)),
                (0x6, Instr(Mnemonic.adc, sz(8), op4(0), imm)),
                (0x7, Instr(Mnemonic.sbb, sz(8), op4(0), imm)),
                (0x8, Instr(Mnemonic.cmp, sz(8), op4(0), imm)),
                (0x9, Instr(Mnemonic.divx, sz(8), op4(0))),
                (0xA, Instr(Mnemonic.rolc, sz(8), op4(0))),
                (0xB, Instr(Mnemonic.rorc, sz(8), op4(0))),
                (0xC, Instr(Mnemonic.divu, sz(8), op4(0))),
                (0xD, Instr(Mnemonic.div, sz(8), op4(0))),
                (0xE, Instr(Mnemonic.adcf, sz(8), op4(0))),
                (0xF, Instr(Mnemonic.abs, sz(8), op4(0))))),
            NextByte(Instr(Mnemonic.mul, sz(8),op4(4), mulop4)),
            NextByte(Mask(8, 1, "  7A/7B",
                Mask(7, 1, "  7A",
                    nyi, // ldc src,dst
                    nyi), // stc src,dst
                // xchg? look these up
                nyi)),
            NextByte(Mask(8, 1, "  0x7C",
                decode7C,
                decode7D)),
            NextByte(Mask(8, 1, "  0x7E",
                Sparse(4, 4, "  0x7E", nyi,
                    (0x1, Instr(Mnemonic.btsts, bitop4)),
                    (0x2, Instr(Mnemonic.nop, bitop4, mnemonicOverride(bmcndMnemonics))),
                    (0x3, Instr(Mnemonic.bntst, bitop4)),
                    (0x4, Instr(Mnemonic.band, bitop4)),
                    (0x5, Instr(Mnemonic.bnand, bitop4)),
                    (0x6, Instr(Mnemonic.bor, bitop4)),
                    (0x7, Instr(Mnemonic.bnor, bitop4)),
                    (0x8, Instr(Mnemonic.bclr, bitop4)),
                    (0x9, Instr(Mnemonic.bset, bitop4)),
                    (0xB, Instr(Mnemonic.btst, bitop4)),
                    (0xC, Instr(Mnemonic.bxor, bitop4)),
                    (0xD, Instr(Mnemonic.bnxor, bitop4))),
                Sparse(4, 4, " 0x7F", nyi,
                    (0x0, nyi)))),

            // 0x80
            NextByte(Instr(Mnemonic.tst, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  82/83",
                Instr(Mnemonic.push, szB, S, r0l),
                Instr(Mnemonic.add, szB, S, imm, op3(0))),
            Instr(Mnemonic.add, szB, S, imm, op3(0)),
            Instr(Mnemonic.add, szB, S, imm, op3(0)),
            NextByte(Instr(Mnemonic.xor, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  8A/8B",
                Instr(Mnemonic.push, szB, S, r0h),
                Instr(Mnemonic.sub, szB, S, imm, op3(0))),
            Instr(Mnemonic.sub, szB, S, imm, op3(0)),
            Instr(Mnemonic.sub, szB, S, imm, op3(0)),

            // 0x90
            NextByte(Instr(Mnemonic.and, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  92/93",
                Instr(Mnemonic.pop, szB, S, r0l),
                Instr(Mnemonic.and, szB, S, imm, op3(0))),
            Instr(Mnemonic.and, szB, S, imm, op3(0)),
            Instr(Mnemonic.and, szB, S, imm, op3(0)),
            NextByte(Instr(Mnemonic.or, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  9A/9B",
                Instr(Mnemonic.pop, S, szB, r0h),
                Instr(Mnemonic.or, szB, S, imm, op3(0))),
            Instr(Mnemonic.or, szB, S, imm, op3(0)),
            Instr(Mnemonic.or, szB, S, imm, op3(0)),

            // 0xA0
            NextByte(Instr(Mnemonic.add, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  A2/A3",
                Instr(Mnemonic.mov, szW, S, imm16(0), a0),
                Instr(Mnemonic.inc, szB, op3(0))),
            Instr(Mnemonic.inc, szB, op3(0)),
            Instr(Mnemonic.inc, szB, op3(0)),
            NextByte(Instr(Mnemonic.sub, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  AA/AB",
                Instr(Mnemonic.mov, szW, S, imm16(0), a1),
                Instr(Mnemonic.dec, szB, op3(0))),
            Instr(Mnemonic.dec, szB, op3(0)),
            Instr(Mnemonic.dec, szB, op3(0)),

            // 0xB0
            NextByte(Instr(Mnemonic.adc, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  B2/B3",
                Instr(Mnemonic.inc, szW, a0),
                Instr(Mnemonic.mov, szB, Z, zero, op3(0))),
            Instr(Mnemonic.mov, szB, Z, zero, op3(0)),
            Instr(Mnemonic.mov, szB, Z, zero, op3(0)),
            NextByte(Instr(Mnemonic.sbb, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  BA/BB",
                Instr(Mnemonic.inc, szW, a1),
                Instr(Mnemonic.not, szB, S, imm8(0), op3(0))),
            Instr(Mnemonic.not, szB, S, imm8(0), op3(0)),
            Instr(Mnemonic.not, szB, S, imm8(0), op3(0)),

            // 0xC0
            NextByte(Instr(Mnemonic.cmp, sz(8), op4(4), op4(0))),
            Mask(0, 1, "  C2/C3",
                Instr(Mnemonic.push, S, szW, a0),
                Instr(Mnemonic.mov, szB, S, imm8(0), op3(0))),
            Instr(Mnemonic.mov, szB, S, imm8(0), op3(0)),
            Instr(Mnemonic.mov, szB, S,imm8(0), op3(0)),
            NextByte(Instr(Mnemonic.add, Q, sz(8), immq_4, op4(0))),
            Mask(0, 1, "  CA/CB",
                Instr(Mnemonic.push, S, szW, a1),
                stz),
            stz,
            stz,

            // 0xD0
            NextByte(Instr(Mnemonic.cmp, sz(8), Q, immq_4, op4(0))),
            Mask(0, 1, "  D2/D3",
                Instr(Mnemonic.pop, S, szW, a0),
                stnz),
            stnz,
            stnz,
            NextByte(Instr(Mnemonic.mov, sz(8), Q, immq_4, op4(0))),
            Mask(0, 1, "  DA/DB",
                Instr(Mnemonic.pop, S, szW, a1),
                stzx),
            stzx,
            stzx,

            // 0xE0
            NextByte(Instr(Mnemonic.rot, simmq_4, op4(0))),
            Mask(0, 1, "  E2/E3",
                Instr(Mnemonic.mov, szB, S, imm8(0), a0),
                Instr(Mnemonic.cmp, szB, S, imm8(0), op3(0))),
            Instr(Mnemonic.cmp, szB, S, imm8(0), op3(0)),
            Instr(Mnemonic.cmp, szB, S, imm8(0), op3(0)),
            NextByte(Instr(Mnemonic.shl, simmq_4, op4(0))),
            Mask(0, 1, "  EA/EB",
                Instr(Mnemonic.mov, szB, S, imm8(0), a1),
                NextByte(Mask(6, 2, "  EB",
                    decodeEB0, // ldintb, // mova?? popc
                    decodeEB0,
                    decodeEB10,
                    Instr(Mnemonic.@int, InstrClass.Transfer|InstrClass.Call, immq6)))),
            Mask(0, 1, "  EC/ED",
                Instr(Mnemonic.pushm, multireg(pushmreg, false)),
                Instr(Mnemonic.popm, multireg(pushmreg, true))), // ldc #16?
            Mask(0, 1, "  EE/EF",
                Instr(Mnemonic.jmps, imm8(0)),
                Instr(Mnemonic.jsrs, imm8(0))),

            // 0xF0
            NextByte(Instr(Mnemonic.sha, sz(8),simmq_4, op4(0))),
            Mask(0, 1, "  F2/F3",
                Instr(Mnemonic.dec, szW, a0),   
                Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return)),
            Mask(0, 1, "  F4/F5",
                Instr(Mnemonic.jmp, szW, labelP1_16),
                Instr(Mnemonic.jsr, szW, labelP1_16)),
            Mask(0, 1, "  F6/F7",
                Instr(Mnemonic.into, InstrClass.Transfer|InstrClass.Call),
                nyi),
            NextByte(
                Instr(Mnemonic.adjnz, InstrClass.ConditionalTransfer, sz(8), simmq_4, op4(0), labelP2_8)),
            Mask(0, 1, "  FA/FB",
                Instr(Mnemonic.dec, szW, a1),
                Instr(Mnemonic.reit)),
            Mask(0, 1, "  FC/FD",
                Instr(Mnemonic.jmp, szA, addr20),
                Instr(Mnemonic.jsr, szA, addr20)),
            Mask(0, 1,
                Instr(Mnemonic.jmp, szB, labelP1_8),
                Instr(Mnemonic.und, InstrClass.Terminates|InstrClass.Padding))
        });
    }

    private class LiteralOperand : AbstractMachineOperand
    {
        private string literal;

        public LiteralOperand(string value) : base(PrimitiveType.Byte)
        {
            this.literal = value;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(literal);
        }
    }
}
