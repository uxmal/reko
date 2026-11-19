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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Reko.Arch.ControlData.CDC6600;

using WideDecoder = WideDecoder<CDC6600Disassembler, Mnemonic, CDC6600Instruction>;

public class CDC6600Disassembler : DisassemblerBase<CDC6600Bundle, Mnemonic>
{
    private static readonly WideMaskDecoder rootDecoder;

    private readonly CDC6600Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private readonly List<CDC6600Instruction> instrs;
    private ulong uInstr;
    private ulong uInstrCur;
    private int validBits;
    private Address addr;

    public CDC6600Disassembler(CDC6600Architecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
        this.instrs = [];
    }

    public override CDC6600Bundle? DisassembleInstruction()
    {
        this.addr = this.rdr.Address;

        if (!this.rdr.TryReadUInt64(out this.uInstr))
            return null;
         
        this.validBits = 60;
        while (validBits > 0)
        {
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = 1;
            this.instrs.Add(instr);
            ops.Clear();
        }
        var bundle = new CDC6600Bundle
        {
            Instructions = this.instrs.ToArray(),
            Address = this.addr,
            Length = 1
        };
        return bundle;
    }


    public override CDC6600Bundle CreateInvalidInstruction()
    {
        return new CDC6600Bundle
        {
            Instructions = new[]
            {
                new CDC6600Instruction
                {
                    InstructionClass = InstrClass.Invalid,
                    Mnemonic = Mnemonic.Invalid
                }
            }
        };
    }


    public override CDC6600Bundle MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new CDC6600Bundle
        {
            Instructions = this.instrs.ToArray()
        };
    }

    public override CDC6600Bundle NotYetImplemented(string message)
    {
        throw new System.NotImplementedException();
    }

    private static bool i30(ulong u, CDC6600Disassembler d)
    {
        d.uInstrCur = d.uInstr >> 30;
        d.uInstr <<= 30;
        d.validBits -= 30;
        return d.validBits >= 0;
    }

    private static bool i15(ulong u, CDC6600Disassembler d)
    {
        d.uInstrCur = d.uInstr >> 45;
        d.uInstr <<= 15;
        d.validBits -= 15;
        return d.validBits >= 0;
    }

    // Registers
    private static WideMutator<CDC6600Disassembler> Register(int bitpos, RegisterStorage[] regs)
    {
        var field = new Bitfield(bitpos, 3);
        return (u, d) =>
        {
            var ireg = field.Read(d.uInstrCur);
            var reg = regs[ireg];
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly WideMutator<CDC6600Disassembler> Ai = Register(6, Registers.ARegisters);
    private static readonly WideMutator<CDC6600Disassembler> Aj = Register(3, Registers.ARegisters);
    private static readonly WideMutator<CDC6600Disassembler> Ak = Register(0, Registers.ARegisters);

    private static readonly WideMutator<CDC6600Disassembler> Bi = Register(6, Registers.BRegisters);
    private static readonly WideMutator<CDC6600Disassembler> Bj = Register(3, Registers.BRegisters);
    private static readonly WideMutator<CDC6600Disassembler> Bk = Register(0, Registers.BRegisters);

    private static readonly WideMutator<CDC6600Disassembler> Xi = Register(6, Registers.XRegisters);
    private static readonly WideMutator<CDC6600Disassembler> Xj = Register(3, Registers.XRegisters);
    private static readonly WideMutator<CDC6600Disassembler> Xk = Register(0, Registers.XRegisters);


    private static bool K(ulong _, CDC6600Disassembler dasm)
    {
        var k = (dasm.uInstrCur & 0x3FFFF);
        dasm.ops.Add(Constant.Create(CDC6600Architecture.Word60, k));
        return true;
    }

    private static bool Kaddr(ulong _, CDC6600Disassembler dasm)
    {
        var k = (dasm.uInstrCur & 0x3FFFF);
        dasm.ops.Add(CDC6600Architecture.Ptr((uint) k));
        return true;
    }

    private static bool jk(ulong _, CDC6600Disassembler dasm)
    {
        var jk = dasm.uInstrCur & 0x3F;
        dasm.ops.Add(Constant.Create(CDC6600Architecture.Word60, jk));
        return true;
    }

    private class InstrDecoder : WideDecoder
    {
        private readonly InstrClass iclass;
        private readonly Mnemonic mnemonic;
        private readonly WideMutator<CDC6600Disassembler>[] opDecoders;
        private readonly bool fuseMnemonic;

        public InstrDecoder(InstrClass iclass, bool fuseMnemonic, Mnemonic mnemonic, params WideMutator<CDC6600Disassembler>[] opDecoders)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.opDecoders = opDecoders;
            this.fuseMnemonic = fuseMnemonic;
        }

        public override CDC6600Instruction Decode(ulong ulInstr, CDC6600Disassembler dasm)
        {
            foreach (var opDecoder in opDecoders)
            {
                if (!opDecoder(ulInstr, dasm))
                    return new CDC6600Instruction
                    {
                        InstructionClass = InstrClass.Invalid,
                        Mnemonic = Mnemonic.Invalid
                    };
            }
            return new CDC6600Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                FuseMnemonic = fuseMnemonic,
                Operands = dasm.ops.ToArray()
            };
        }
    }

    private class WideMaskDecoder : WideDecoder
    {
        private readonly Bitfield bitfield;
        private readonly string tag;
        private readonly WideDecoder[] decoders;

        public WideMaskDecoder(int bitpos, int bitlen, string tag, params WideDecoder[] decoders)
        {
            this.bitfield = new Bitfield(bitpos, bitlen);
            this.tag = tag;
            this.decoders = decoders;
        }

        public override CDC6600Instruction Decode(ulong ulInstr, CDC6600Disassembler dasm)
        {
            TraceDecoder(ulInstr);
            uint op = bitfield.Read(ulInstr);
            return decoders[op].Decode(ulInstr, dasm);
        }

        [Conditional("DEBUG")]
        public void TraceDecoder(ulong wInstr)
        {
            var shMask = (ulong) this.bitfield.Mask << this.bitfield.Position;
            DumpMaskedInstruction(64, wInstr, shMask, tag);
        }

        public override string ToString()
        {
            return $"{{Mask {bitfield}}}";
        }
    }

    private static InstrDecoder Instr(
        Mnemonic mnemonic,
        InstrClass iclass,
        params WideMutator<CDC6600Disassembler>[] opDecoders)
    {
        return new InstrDecoder(iclass, false, mnemonic, opDecoders);
    }


    private static InstrDecoder InstrI(
        Mnemonic mnemonic,
        params WideMutator<CDC6600Disassembler>[] opDecoders)
    {
        return new InstrDecoder(InstrClass.Linear, true, mnemonic, opDecoders);
    }

    static CDC6600Disassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid, i15);

        var decode01 = new WideMaskDecoder(51, 3, "  01",
            // 010 - 017: Return Jump, Extended Core Memory (ECS), Exchange Jump & Undefined
            Instr(Mnemonic.RJ, InstrClass.Call, i30, Kaddr),
            Instr(Mnemonic.REC, InstrClass.Linear, i30, Bj, K),
            Instr(Mnemonic.WEC, InstrClass.Linear, i30, Bj, K),
            invalid,
            invalid,
            invalid,
            invalid,
            invalid);

        // 030 - 037
        var decode03 = new WideMaskDecoder(51, 3, "  03",
            Instr(Mnemonic.ZR, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.NZ, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.PL, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.NG, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.IR, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.OR, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.DF, InstrClass.CondJump, i30, Xj, K),
            Instr(Mnemonic.ID, InstrClass.CondJump, i30, Xj, K));

        rootDecoder = new WideMaskDecoder(54, 6, "CDC6600",
            // 00
            Instr(Mnemonic.PS, InstrClass.Terminates, i30),
            decode01,
            Instr(Mnemonic.JP, InstrClass.Jump, i30, Bi, Kaddr),
            decode03,
            Instr(Mnemonic.EQ, InstrClass.CondJump, i30, Bi, Bj, Kaddr),
            Instr(Mnemonic.NE, InstrClass.CondJump, i30, Bi, Bj, Kaddr),
            Instr(Mnemonic.GE, InstrClass.CondJump, i30, Bi, Bj, Kaddr),
            Instr(Mnemonic.LT, InstrClass.CondJump, i30, Bi, Bj, Kaddr),

            // 10
            InstrI(Mnemonic.mov, i15, Xi, Xj),
            InstrI(Mnemonic.and, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.or, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.xor, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.neg, i15, Xi, Xk),
            InstrI(Mnemonic.andn, i15, Xi, Xk, Xj),
            InstrI(Mnemonic.orn, i15, Xi, Xk, Xj),
            InstrI(Mnemonic.xorn, i15, Xi, Xk, Xj),

            // 20
            InstrI(Mnemonic.lsl, i15, Xi, jk),
            InstrI(Mnemonic.asr, i15, Xi, jk),
            InstrI(Mnemonic.lsln, i15, Xi, Bj, Xk),
            InstrI(Mnemonic.asrn, i15, Xi, Bj, Xk),
            InstrI(Mnemonic.normalize, i15, Xi, Bj, Xk),
            InstrI(Mnemonic.round_normalize, i15, Xi, Bj, Xk),
            InstrI(Mnemonic.unpack, i15, Xi, Bj, Xk),
            InstrI(Mnemonic.pack, i15, Xi, Bj, Xk),

            // 30
            InstrI(Mnemonic.fadd, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.fsub, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.dadd, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.dsub, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.fadd_round, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.fsub_round, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.iadd, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.isub, i15, Xi, Xj, Xk),

            // 40
            InstrI(Mnemonic.fmul, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.fmul_round, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.dmul, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.mask, i15, Xi, jk),
            InstrI(Mnemonic.fdiv, i15, Xi, Xj, Xk),
            InstrI(Mnemonic.fdiv_round, i15, Xi, Xj, Xk),
            Instr(Mnemonic.NO, InstrClass.Linear | InstrClass.Padding, i15),
            InstrI(Mnemonic.popcnt, i15, Xi, Xk),

            // 50
            InstrI(Mnemonic.sadd, i30, Ai, Aj, K),
            InstrI(Mnemonic.sadd, i30, Ai, Bj, K),
            InstrI(Mnemonic.sadd, i30, Ai, Xj, K),
            InstrI(Mnemonic.sadd, i15, Ai, Xj, Bk),
            InstrI(Mnemonic.sadd, i15, Ai, Aj, Bk),
            InstrI(Mnemonic.ssub, i15, Ai, Aj, Bk),
            InstrI(Mnemonic.sadd, i15, Ai, Bj, Bk),
            InstrI(Mnemonic.ssub, i15, Ai, Bj, Bk),

            // 60
            InstrI(Mnemonic.sadd, i30, Bi, Aj, K),
            InstrI(Mnemonic.sadd, i30, Bi, Bj, K),
            InstrI(Mnemonic.sadd, i30, Bi, Xj, K),
            InstrI(Mnemonic.sadd, i15, Bi, Xj, Bk),
            InstrI(Mnemonic.sadd, i15, Bi, Aj, Bk),
            InstrI(Mnemonic.ssub, i15, Bi, Aj, Bk),
            InstrI(Mnemonic.sadd, i15, Bi, Bj, Bk),
            InstrI(Mnemonic.ssub, i15, Bi, Bj, Bk),

            // 70
            InstrI(Mnemonic.sadd, i30, Xi, Aj, K),
            InstrI(Mnemonic.sadd, i30, Xi, Bj, K),
            InstrI(Mnemonic.sadd, i30, Xi, Xj, K),
            InstrI(Mnemonic.sadd, i15, Xi, Xj, Bk),
            InstrI(Mnemonic.sadd, i15, Xi, Aj, Bk),
            InstrI(Mnemonic.ssub, i15, Xi, Aj, Bk),
            InstrI(Mnemonic.sadd, i15, Xi, Bj, Bk),
            InstrI(Mnemonic.ssub, i15, Xi, Bj, Bk));
    }
}