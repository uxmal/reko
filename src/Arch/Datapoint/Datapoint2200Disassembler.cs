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
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.Arch.Datapoint;

public class Datapoint2200Disassembler : DisassemblerBase<Datapoint2200Instruction, Mnemonic>
{
    private static readonly Decoder<
        Datapoint2200Disassembler,
        Mnemonic,
        Datapoint2200Instruction> rootDecoder;

    private readonly Datapoint2200Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public Datapoint2200Disassembler(Datapoint2200Architecture arch, EndianImageReader rdr) 
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override Datapoint2200Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadByte(out var opcode))
        {
            return null;
        }
        var instr = rootDecoder.Decode(opcode, this);
        ops.Clear();
        instr.Address = this.addr;
        instr.Length = (int) (rdr.Address - this.addr);
        return instr;
    }


    public override Datapoint2200Instruction CreateInvalidInstruction()
    {
        return new Datapoint2200Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    private static bool I8(uint uInstr, Datapoint2200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var imm))
            return false;
        dasm.ops.Add(Constant.Byte(imm));
        return true;
    }

    private static bool J(uint uInstr, Datapoint2200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out var uAddr))
            return false;
        dasm.ops.Add(Address.Ptr16(uAddr));
        return true;
    }

    private static Mutator<Datapoint2200Disassembler> Lit(string value)
    {
        var op = new LiteralOperand(value);
        return (u, d) =>
        {
            d.ops.Add(op);
            return true;
        };
    }

    public override Datapoint2200Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new Datapoint2200Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = this.ops.ToArray()
        };
    }

    public override Datapoint2200Instruction NotYetImplemented(string message)
    {
        throw new System.NotImplementedException();
    }

    private static InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Datapoint2200Disassembler>[] opDecoders)
    {
        return new InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction>(iclass, mnemonic, opDecoders);
    }

    private static InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction> Instr(Mnemonic mnemonic, params Mutator<Datapoint2200Disassembler>[] opDecoders)
    {
        return new InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction>(InstrClass.Linear, mnemonic, opDecoders);
    }

    private static InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction> Instr2(Mnemonic mnemonic, params Mutator<Datapoint2200Disassembler>[] opDecoders)
    {
        return new InstrDecoder<Datapoint2200Disassembler, Mnemonic, Datapoint2200Instruction>(InstrClass.Linear, mnemonic, opDecoders);
    }


    static Datapoint2200Disassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        rootDecoder = Mask(0, 8, "DG2200",
            // 0x00
            Instr(Mnemonic.halt, InstrClass.Terminates, []),
            Instr(Mnemonic.halt, InstrClass.Terminates, []),
            Instr(Mnemonic.slc),
            Instr(Mnemonic.rfc, InstrClass.CondReturn),

            Instr(Mnemonic.ad, I8),
            invalid,
            Instr(Mnemonic.la, I8),
            Instr(Mnemonic.@return, InstrClass.Return),

            invalid,
            invalid,
            Instr(Mnemonic.src),
            Instr(Mnemonic.rfz, InstrClass.CondReturn),

            Instr(Mnemonic.ac, I8),
            invalid,
            Instr(Mnemonic.lb, I8),
            invalid,

            // 0x10
            Instr2(Mnemonic.beta),
            invalid,
            invalid,
            Instr(Mnemonic.rfs, InstrClass.CondReturn),

            Instr(Mnemonic.su, I8),
            invalid,
            Instr(Mnemonic.lc, I8),
            invalid,

            Instr2(Mnemonic.alpha),
            invalid,
            invalid,
            Instr(Mnemonic.rfp, InstrClass.CondReturn),

            Instr(Mnemonic.sb, I8),
            invalid,
            Instr(Mnemonic.ld, I8),
            invalid,

            // 0x20
            Instr2(Mnemonic.di),
            invalid,
            invalid,
            Instr(Mnemonic.rtc, InstrClass.CondReturn),

            Instr(Mnemonic.nd, I8),
            invalid,
            Instr(Mnemonic.le, I8),
            invalid,

            Instr2(Mnemonic.ei),
            invalid,
            invalid,
            Instr(Mnemonic.rtz, InstrClass.CondReturn),

            Instr(Mnemonic.xr, I8),
            invalid,
            Instr(Mnemonic.lh, I8),
            invalid,

            // 0x30
            Instr2(Mnemonic.pop),
            invalid,
            invalid,
            Instr(Mnemonic.rts, InstrClass.CondReturn),

            Instr(Mnemonic.or, I8),
            invalid,
            Instr(Mnemonic.ll, I8),
            invalid,

            Instr2(Mnemonic.push),
            invalid,
            invalid,
            Instr(Mnemonic.rtp, InstrClass.CondReturn),

            Instr(Mnemonic.cp, I8),
            invalid,
            Instr(Mnemonic.lm, I8),
            invalid,

            // 0x40
            Instr(Mnemonic.jfc, InstrClass.CondJump, J),
            Instr(Mnemonic.input),
            Instr(Mnemonic.cfc, InstrClass.CondCall, J),
            invalid,

            Instr(Mnemonic.jmp, InstrClass.Jump, J),
            invalid,
            Instr(Mnemonic.call, InstrClass.Call, J),
            invalid,

            Instr(Mnemonic.jfz, InstrClass.CondJump, J),
            invalid,
            Instr(Mnemonic.cfz, InstrClass.CondCall, J),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 0x50
            Instr(Mnemonic.jfs, InstrClass.CondJump, J),
            Instr(Mnemonic.ex, Lit("adr")),
            Instr(Mnemonic.cfs, InstrClass.CondCall, J),
            Instr(Mnemonic.ex, Lit("status")),

            invalid,
            Instr(Mnemonic.ex, Lit("data")),
            invalid,
            Instr(Mnemonic.ex, Lit("write")),

            Instr(Mnemonic.jfp, InstrClass.CondJump, J),
            Instr(Mnemonic.ex, Lit("com1")),
            Instr(Mnemonic.cfp, InstrClass.CondCall, J),
            Instr(Mnemonic.ex, Lit("com2")),

            invalid,
            Instr(Mnemonic.ex, Lit("com3")),
            invalid,
            Instr(Mnemonic.ex, Lit("com4")),

            // 0x60
            Instr(Mnemonic.jtc, InstrClass.CondJump, J),
            invalid,
            Instr(Mnemonic.ctc, InstrClass.CondCall, J),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.jtz, InstrClass.CondJump, J),
            Instr(Mnemonic.ex, Lit("beep")),
            Instr(Mnemonic.ctz, InstrClass.CondCall, J),
            Instr(Mnemonic.ex, Lit("click")),

            invalid,
            Instr(Mnemonic.ex, Lit("deck1")),
            invalid,
            Instr(Mnemonic.ex, Lit("deck2")),

            // 0x70
            Instr(Mnemonic.jts, InstrClass.CondJump, J),
            Instr(Mnemonic.ex, Lit("rbk")),
            Instr(Mnemonic.cts, InstrClass.CondCall, J),
            Instr(Mnemonic.ex, Lit("wbk")),

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.ex, Lit("bsp")),

            Instr(Mnemonic.jtp, InstrClass.CondJump, J),
            Instr(Mnemonic.ex, Lit("sf")),
            Instr(Mnemonic.ctp, InstrClass.CondCall, J),
            Instr(Mnemonic.ex, Lit("sb")),

            invalid,
            Instr(Mnemonic.ex, Lit("rewnd")),
            invalid,
            Instr(Mnemonic.ex, Lit("tstop")),

            // 0x80
            Instr(Mnemonic.ada),
            Instr(Mnemonic.adb),
            Instr(Mnemonic.adc),
            Instr(Mnemonic.add),
            Instr(Mnemonic.ade),
            Instr(Mnemonic.adh),
            Instr(Mnemonic.adl),
            Instr(Mnemonic.adm),

            Instr(Mnemonic.aca),
            Instr(Mnemonic.acb),
            Instr(Mnemonic.acc),
            Instr(Mnemonic.acd),
            Instr(Mnemonic.ace),
            Instr(Mnemonic.ach),
            Instr(Mnemonic.acl),
            Instr(Mnemonic.acm),

            // 0x90
            Instr(Mnemonic.sua),
            Instr(Mnemonic.sub),
            Instr(Mnemonic.suc),
            Instr(Mnemonic.sud),
            Instr(Mnemonic.sue),
            Instr(Mnemonic.suh),
            Instr(Mnemonic.sul),
            Instr(Mnemonic.sum),

            Instr(Mnemonic.sba),
            Instr(Mnemonic.sbb),
            Instr(Mnemonic.sbc),
            Instr(Mnemonic.sbd),
            Instr(Mnemonic.sbe),
            Instr(Mnemonic.sbh),
            Instr(Mnemonic.sbl),
            Instr(Mnemonic.sbm),

            // 0xA0
            Instr(Mnemonic.nda),
            Instr(Mnemonic.ndb),
            Instr(Mnemonic.ndc),
            Instr(Mnemonic.ndd),
            Instr(Mnemonic.nde),
            Instr(Mnemonic.ndh),
            Instr(Mnemonic.ndl),
            Instr(Mnemonic.ndm),

            Instr(Mnemonic.xra),
            Instr(Mnemonic.xrb),
            Instr(Mnemonic.xrc),
            Instr(Mnemonic.xrd),
            Instr(Mnemonic.xre),
            Instr(Mnemonic.xrh),
            Instr(Mnemonic.xrl),
            Instr(Mnemonic.xrm),

            // 0xB0
            Instr(Mnemonic.ora),
            Instr(Mnemonic.orb),
            Instr(Mnemonic.orc),
            Instr(Mnemonic.ord),
            Instr(Mnemonic.ore),
            Instr(Mnemonic.orh),
            Instr(Mnemonic.orl),
            Instr(Mnemonic.orm),

            Instr(Mnemonic.cpa),
            Instr(Mnemonic.cpb),
            Instr(Mnemonic.cpc),
            Instr(Mnemonic.cpd),
            Instr(Mnemonic.cpe),
            Instr(Mnemonic.cph),
            Instr(Mnemonic.cpl),
            Instr(Mnemonic.cpm),

            // 0xC0
            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
            Instr(Mnemonic.lab),
            Instr(Mnemonic.lac),
            Instr(Mnemonic.lad),
            Instr(Mnemonic.lae),
            Instr(Mnemonic.lah),
            Instr(Mnemonic.lal),
            Instr(Mnemonic.lam),

            Instr(Mnemonic.lba),
            invalid,
            Instr(Mnemonic.lbc),
            Instr(Mnemonic.lbd),
            Instr(Mnemonic.lbe),
            Instr(Mnemonic.lbh),
            Instr(Mnemonic.lbl),
            Instr(Mnemonic.lbm),

            // 0xD0
            Instr(Mnemonic.lca),
            Instr(Mnemonic.lcb),
            invalid,
            Instr(Mnemonic.lcd),
            Instr(Mnemonic.lce),
            Instr(Mnemonic.lch),
            Instr(Mnemonic.lcl),
            Instr(Mnemonic.lcm),

            Instr(Mnemonic.lda),
            Instr(Mnemonic.ldb),
            Instr(Mnemonic.ldc),
            invalid,
            Instr(Mnemonic.lde),
            Instr(Mnemonic.ldh),
            Instr(Mnemonic.ldl),
            Instr(Mnemonic.ldm),

            // 0xE0
            Instr(Mnemonic.lea),
            Instr(Mnemonic.leb),
            Instr(Mnemonic.lec),
            Instr(Mnemonic.led),
            invalid,
            Instr(Mnemonic.leh),
            Instr(Mnemonic.lel),
            Instr(Mnemonic.lem),

            Instr(Mnemonic.lha),
            Instr(Mnemonic.lhb),
            Instr(Mnemonic.lhc),
            Instr(Mnemonic.lhd),
            Instr(Mnemonic.lhe),
            invalid,
            Instr(Mnemonic.lhl),
            Instr(Mnemonic.lhm),

            // 0xF0
            Instr(Mnemonic.lla),
            Instr(Mnemonic.llb),
            Instr(Mnemonic.llc),
            Instr(Mnemonic.lld),
            Instr(Mnemonic.lle),
            Instr(Mnemonic.llh),
            invalid,
            Instr(Mnemonic.llm),

            Instr(Mnemonic.lma),
            Instr(Mnemonic.lmb),
            Instr(Mnemonic.lmc),
            Instr(Mnemonic.lmd),
            Instr(Mnemonic.lme),
            Instr(Mnemonic.lmh),
            Instr(Mnemonic.lml),
            Instr(Mnemonic.halt));
    }
}