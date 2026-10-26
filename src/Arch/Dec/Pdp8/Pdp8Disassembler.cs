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

using Reko.Arch.Dec.Memory;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Reko.Arch.Dec.Pdp8;

public class Pdp8Disassembler : DisassemblerBase<Pdp8Instruction, Mnemonic>
{
    private static readonly Decoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction> rootDecoder;

    private readonly Pdp8Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private bool indirect;

    public Pdp8Disassembler(Pdp8Architecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override Pdp8Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadUInt16(out var uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        indirect = false;
        ops.Clear();
        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        return instr;
    }

    public override Pdp8Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new Pdp8Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray(),
            Indirect = this.indirect
        };
    }

    public override Pdp8Instruction CreateInvalidInstruction()
    {
        return new Pdp8Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override Pdp8Instruction NotYetImplemented(string message)
    {
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        var opcode = Octize(this.rdr.Clone());
        testSvc?.ReportMissingDecoder("Pdp8Dis", Address.OctalPtr(PdpTypes.Ptr12, 0x100), "", opcode);
        return new Pdp8Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Nyi
        };
    }

    private string Octize(EndianImageReader rdrClone)
    {
        long len = rdrClone.Address - this.addr;
        var sb = new StringBuilder();
        while (len > 0)
        {
            if (!rdrClone.TryReadBeUInt16(out ushort uInstr))
                break;
            sb.Append(Convert.ToString(uInstr, 8).PadLeft(4, '0'));
            sb.Append(' ');
            --len;
        }
        return sb.ToString();
    }

    private static bool mri(uint uInstr, Pdp8Disassembler dasm)
    {
        dasm.indirect = (uInstr & 0x100) != 0;
        bool currentPage = (uInstr & 0x80) != 0;
        uint uAddr = uInstr & 0x7F;
        if (currentPage)
        {
            uAddr |= dasm.addr.ToUInt16() & 0xF80u;
        }
        dasm.ops.Add(Address.OctalPtr(PdpTypes.Ptr12, uAddr));
        return true;
    }
    
    private static bool device(uint uInstr, Pdp8Disassembler dasm)
    {
        dasm.NotYetImplemented("");
        return false;
    }

    private static bool I(uint uInstr, Pdp8Disassembler dasm)
    {
        dasm.indirect = (uInstr & 0x100) != 0;
        return true;
    }

    private static InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Pdp8Disassembler> [] mutators)
    {
        return new InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction>(iclass, mnemonic, mutators);
    }

    private static InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction> Instr(Mnemonic mnemonic, params Mutator<Pdp8Disassembler>[] mutators)
    {
        return new InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction> ModeA(Mnemonic mnemonic, params Mutator<Pdp8Disassembler>[] mutators)
    {
        return new InstrDecoder<Pdp8Disassembler, Mnemonic, Pdp8Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    static Pdp8Disassembler()
    {
        var nyi = Instr(Mnemonic.Nyi, InstrClass.Invalid, []);
        var iot = Mask(0, 3, "  IOT",
            Instr(Mnemonic.skon, device),
            Instr(Mnemonic.ion, device),
            Instr(Mnemonic.iof, device),
            Instr(Mnemonic.srq, device),

            Instr(Mnemonic.gtf, device),
            Instr(Mnemonic.rtf, device),
            Instr(Mnemonic.sgt, device),
            Instr(Mnemonic.caf, device));

        var group1 = Sparse(0, 8, "  group1", nyi,
            (0b00_000_000, Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding)),
            (0b00_000_001, Instr(Mnemonic.iac)),
            (0b00_000_010, Instr(Mnemonic.bsw)),
            (0b00_000_100, Instr(Mnemonic.ral)),
            (0b00_000_110, Instr(Mnemonic.rtl)),
            (0b00_001_000, Instr(Mnemonic.rar)),
            (0b00_001_010, Instr(Mnemonic.rtr)),
            (0b00_010_000, Instr(Mnemonic.cml)),
            (0b00_100_000, Instr(Mnemonic.cma)),
            (0b00_100_001, Instr(Mnemonic.cia)),
            (0b01_000_001, Instr(Mnemonic.cll)),
            (0b01_010_000, Instr(Mnemonic.stl)),
            (0b10_000_000, Instr(Mnemonic.cla)),
            (0b10_000_100, Instr(Mnemonic.glk)),
            (0b10_100_000, Instr(Mnemonic.sta)));
        var group2 = Sparse(1, 7, "  group2", nyi,
            (0b00_000_01, Instr(Mnemonic.hlt)),
            (0b00_000_10, Instr(Mnemonic.osr)),
            (0b00_001_00, Instr(Mnemonic.skp)),
            (0b00_010_00, Instr(Mnemonic.snl)),
            (0b00_011_00, Instr(Mnemonic.szl)),
            (0b00_100_00, Instr(Mnemonic.sza)),
            (0b00_101_00, Instr(Mnemonic.sna)),
            (0b01_000_00, Instr(Mnemonic.sma)),
            (0b01_001_00, Instr(Mnemonic.spa)),
            (0b10_000_00, Instr(Mnemonic.cla)));
        var group3 = Sparse(1, 7, "  group3", nyi,
            (0b00_010_00, Instr(Mnemonic.mql)),
            (0b00_011_00, Instr(Mnemonic.swab)),
            (0b00_100_11, Instr(Mnemonic.swba)),
            (0b00_101_00, Instr(Mnemonic.dpsz)),
            (0b10_000_00, Instr(Mnemonic.cla)),
            (0b01_000_00, Instr(Mnemonic.mqa)),
            (0b01_010_00, Instr(Mnemonic.swp)),
            (0b10_010_00, Instr(Mnemonic.cam)));

        var operate = Mask(8, 1, "  operate",
            group1,
            Mask(0, 1, "  group2/3",
                group2,
                group3));

        rootDecoder = Mask(9, 3, "PDP-8",
            Instr(Mnemonic.and, I, mri),
            Instr(Mnemonic.tad, I, mri),
            Instr(Mnemonic.isz, InstrClass.CondJump, I, mri),
            Instr(Mnemonic.dca, I, mri),

            Instr(Mnemonic.jms, InstrClass.Call, I, mri),
            Instr(Mnemonic.jmp, InstrClass.Jump, I, mri),
            iot,
            operate);
    }
}
