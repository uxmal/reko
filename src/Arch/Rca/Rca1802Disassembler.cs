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
using Reko.Core.Services;
using System.Collections.Generic;

namespace Reko.Arch.Rca;

public class Rca1802Disassembler : DisassemblerBase<Rca1802Instruction, Mnemonic>
{
    private static readonly Decoder<Rca1802Disassembler, Mnemonic, Rca1802Instruction> rootDecoder;

    private readonly Rca1802Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public Rca1802Disassembler(Rca1802Architecture arch, EndianImageReader imageReader)
    {
        this.arch = arch;
        this.rdr = imageReader;
        this.ops = [];
    }

    public override Rca1802Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (this.addr.Offset == 0x0856)
            _ = this; //$DEBUG
        if (!rdr.TryReadByte(out byte uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        ops.Clear();
        instr.Address = this.addr;
        instr.Length = (int) (rdr.Address - this.addr);
        return instr;
    }

    public override Rca1802Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new Rca1802Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray()
        };
    }

    public override Rca1802Instruction CreateInvalidInstruction()
    {
        return new Rca1802Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid
        };
    }

    public override Rca1802Instruction NotYetImplemented(string message)
    {
        var svc = arch.Services.GetService<ITestGenerationService>();
        svc?.ReportMissingDecoder("Rca1802Dis", Address.Ptr16(0x100), rdr, message);
        return new Rca1802Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Nyi
        };
    }

    /// <summary>
    /// Extract the register number from the instruction.
    /// </summary>
    private static bool RegN(uint uInstr, Rca1802Disassembler dasm)
    {
        var n = uInstr & 0xF;
        dasm.ops.Add(Registers.GpRegisters[n]);
        return true;
    }

    /// <summary>
    /// Extract the device number from the instruction.
    /// </summary>
    private static bool Device(uint uInstr, Rca1802Disassembler dasm)
    {
        var n = uInstr & 0xF;
        dasm.ops.Add(Constant.Byte((byte)n));
        return true;
    }

    /// <summary>
    /// Immediate data byte.
    /// </summary>
    private static bool Data(uint uInstr, Rca1802Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte uByte))
            return false;
        dasm.ops.Add(Constant.Byte(uByte));
        return true;
    }

    /// <summary>
    /// Short jump address.
    /// </summary>
    private static bool Addr(uint uInstr, Rca1802Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte uByte))
            return false;
        var uAddr = (dasm.addr.Offset & ~0xFFu) | uByte;
        var addr = Address.Ptr16((ushort)uAddr);
        dasm.ops.Add(addr);
        return true;
    }

    /// <summary>
    /// Read a long address from the instruction stream.
    /// </summary>
    private static bool LAddress(uint uInstr, Rca1802Disassembler dasm)
    {
        if (!dasm.rdr.TryReadUInt16(out ushort uWord))
            return false;
        var addr = Address.Ptr16(uWord);
        dasm.ops.Add(addr);
        return true;
    }

    private static Decoder<Rca1802Disassembler, Mnemonic, Rca1802Instruction> Instr(
        Mnemonic mnemonic, 
        params Mutator<Rca1802Disassembler>[] mutators)
    {
        return new InstrDecoder<Rca1802Disassembler, Mnemonic, Rca1802Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static Decoder<Rca1802Disassembler, Mnemonic, Rca1802Instruction> Instr(
        Mnemonic mnemonic,
        InstrClass iclass,
        params Mutator<Rca1802Disassembler>[] mutators)
    {
        return new InstrDecoder<Rca1802Disassembler, Mnemonic, Rca1802Instruction>(iclass, mnemonic, mutators);
    }


    static Rca1802Disassembler()
    {
        rootDecoder = Mask(0, 8, "RCA1802",
            Instr(Mnemonic.IDL, InstrClass.Linear | InstrClass.Padding),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),

            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),

            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),

            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),
            Instr(Mnemonic.LDN, RegN),

            // 0x10
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),

            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),

            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),

            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),
            Instr(Mnemonic.INC, RegN),

            // 0x20
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),

            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),

            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),

            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),
            Instr(Mnemonic.DEC, RegN),

            // 0x30
            Instr(Mnemonic.BR, InstrClass.Jump, Addr),
            Instr(Mnemonic.BQ, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BZ, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BDF, InstrClass.CondJump, Addr),

            Instr(Mnemonic.B1, InstrClass.CondJump, Addr),
            Instr(Mnemonic.B2, InstrClass.CondJump, Addr),
            Instr(Mnemonic.B3, InstrClass.CondJump, Addr),
            Instr(Mnemonic.B4, InstrClass.CondJump, Addr),

            Instr(Mnemonic.NBR, InstrClass.Jump), //$TODO: what is this instr doing?
            Instr(Mnemonic.BNQ, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BNZ, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BNF, InstrClass.CondJump, Addr),

            Instr(Mnemonic.BN1, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BN2, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BN3, InstrClass.CondJump, Addr),
            Instr(Mnemonic.BN4, InstrClass.CondJump, Addr),

            // 0x40
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),

            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),

            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),

            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),
            Instr(Mnemonic.LDA, RegN),

            // 0x50
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),

            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),

            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),

            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),
            Instr(Mnemonic.STR, RegN),

            // 0x60
            Instr(Mnemonic.IRX, InstrClass.Linear),
            Instr(Mnemonic.OUT1, Device),
            Instr(Mnemonic.OUT2, Device),
            Instr(Mnemonic.OUT3, Device),

            Instr(Mnemonic.OUT4, Device),
            Instr(Mnemonic.OUT5, Device),
            Instr(Mnemonic.OUT6, Device),
            Instr(Mnemonic.OUT7, Device),

            Instr(Mnemonic.Invalid, InstrClass.Invalid),
            Instr(Mnemonic.INP1, Device),
            Instr(Mnemonic.INP2, Device),
            Instr(Mnemonic.INP3, Device),

            Instr(Mnemonic.INP4, Device),
            Instr(Mnemonic.INP5, Device),
            Instr(Mnemonic.INP6, Device),
            Instr(Mnemonic.INP7, Device),

            // 0x70
            Instr(Mnemonic.RET),
            Instr(Mnemonic.DIS),
            Instr(Mnemonic.LDXA),
            Instr(Mnemonic.STXD),

            Instr(Mnemonic.ADC),
            Instr(Mnemonic.SDB),
            Instr(Mnemonic.SHRC),
            Instr(Mnemonic.SMB),

            Instr(Mnemonic.SAV),
            Instr(Mnemonic.MARK),
            Instr(Mnemonic.REQ),
            Instr(Mnemonic.SEQ),

            Instr(Mnemonic.ADCI, Data),
            Instr(Mnemonic.SDBI, Data),
            Instr(Mnemonic.SHLC),
            Instr(Mnemonic.SMBI, Data),

            // 0x80
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),

            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),

            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),

            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),
            Instr(Mnemonic.GLO, RegN),

            // 0x90
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),

            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),

            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),

            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),
            Instr(Mnemonic.GHI, RegN),

            // 0xA0
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),

            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),

            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),

            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),
            Instr(Mnemonic.PLO, RegN),

            // 0xB0
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),

            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),

            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),

            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),
            Instr(Mnemonic.PHI, RegN),

            // 0xC0
            Instr(Mnemonic.LBR, InstrClass.Jump, LAddress),
            Instr(Mnemonic.LBQ, InstrClass.CondJump, LAddress),
            Instr(Mnemonic.LBZ, InstrClass.CondJump, LAddress),
            Instr(Mnemonic.LBDF, InstrClass.CondJump, LAddress),

            Instr(Mnemonic.NOP, InstrClass.Linear | InstrClass.Padding),
            Instr(Mnemonic.LSNQ, InstrClass.CondJump),
            Instr(Mnemonic.LSNZ, InstrClass.CondJump),
            Instr(Mnemonic.LSNF, InstrClass.CondJump),

            Instr(Mnemonic.LNBR, InstrClass.Jump),
            Instr(Mnemonic.LBNQ, InstrClass.CondJump, LAddress),
            Instr(Mnemonic.LBNZ, InstrClass.CondJump, LAddress),
            Instr(Mnemonic.LBNF, InstrClass.CondJump, LAddress),

            Instr(Mnemonic.LSIE, InstrClass.CondJump),
            Instr(Mnemonic.LSQ, InstrClass.CondJump),
            Instr(Mnemonic.LSZ, InstrClass.CondJump),
            Instr(Mnemonic.LSDF, InstrClass.CondJump),

            // 0xD0
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),

            Instr(Mnemonic.CALL, InstrClass.Call, LAddress),    // SCRT convention.
            Instr(Mnemonic.EXIT, InstrClass.Return),            // SCRT convention.
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),

            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),

            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),
            Instr(Mnemonic.SEP, InstrClass.JumpInd, RegN),

            // 0xE0
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),

            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),

            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),

            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),
            Instr(Mnemonic.SEX, RegN),

            // 0xF0
            Instr(Mnemonic.LDX),
            Instr(Mnemonic.OR),
            Instr(Mnemonic.AND),
            Instr(Mnemonic.XOR),

            Instr(Mnemonic.ADD),
            Instr(Mnemonic.SD),
            Instr(Mnemonic.SHR),
            Instr(Mnemonic.SM),

            Instr(Mnemonic.LDI, Data),
            Instr(Mnemonic.ORI, Data),
            Instr(Mnemonic.ANI, Data),
            Instr(Mnemonic.XRI, Data),

            Instr(Mnemonic.ADI, Data),
            Instr(Mnemonic.SDI, Data),
            Instr(Mnemonic.SHL),
            Instr(Mnemonic.SMI, Data)


            );
    }
}