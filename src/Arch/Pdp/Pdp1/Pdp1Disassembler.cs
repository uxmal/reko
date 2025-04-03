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

using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Numerics;

namespace Reko.Arch.Pdp.Pdp1;

public class Pdp1Disassembler : DisassemblerBase<Pdp1Instruction, Mnemonic>
{
    private static readonly Decoder<Pdp1Disassembler, Mnemonic, Pdp1Instruction> rootDecoder;
    private static readonly Bitfield bf3L3 = Bf(3, 3);
    private readonly Pdp1Architecture arch;
    private readonly Word18BeImageReader rdr;
    private readonly List<MachineOperand> ops;

    public Pdp1Disassembler(Pdp1Architecture arch, Word18BeImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override Pdp1Instruction? DisassembleInstruction()
    {
        var addr = rdr.Address;
        if (!rdr.TryReadBeUInt18(out uint uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        ops.Clear();
        instr.Address = addr;
        instr.Length = 1;
        return instr;
    }

    public override Pdp1Instruction CreateInvalidInstruction()
    {
        return new Pdp1Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override Pdp1Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new Pdp1Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray(),
        };
    }
    public override Pdp1Instruction NotYetImplemented(string message)
    {
        return CreateInvalidInstruction();
    }

    /// <summary>
    /// Instruction memory address.
    /// </summary>
    private static bool Y(uint uInstr, Pdp1Disassembler dasm)
    {
        var addr = Pdp10Architecture.Ptr18(uInstr & 0xFFF);
        dasm.ops.Add(addr);
        return true;
    }

    /// <summary>
    /// Shift amount, where the number is "the number of ONE's 
    /// in bits 9-17 of the instruction word."
    /// </summary>
    private static bool Sh(uint uInstr, Pdp1Disassembler dasm)
    {
        var shift = Constant.Create(
            PrimitiveType.Byte,
            BitOperations.PopCount(uInstr & 0x1FF));
        dasm.ops.Add(shift);
        return true;
    }

    /// <summary>
    /// Program flag.
    /// </summary>
    private static bool F(uint uInstr, Pdp1Disassembler dasm)
    {
        var flag = Constant.Byte((byte) (uInstr & 7));
        dasm.ops.Add(flag);
        return true;
    }

    private static bool Npos(uint uInstr, Pdp1Disassembler dasm)
    {
        dasm.ops.Add(Constant.Create(PdpTypes.Word18, uInstr & 0xFFF));
        return true;
    }

    private static bool Nneg(uint uInstr, Pdp1Disassembler dasm)
    {
        dasm.ops.Add(Constant.Create(PdpTypes.Word18, -(int) (uInstr & 0xFFF)));
        return true;
    }

    /// <summary>
    /// Sense switch 
    /// </summary>
    private static bool Sw(uint uInstr, Pdp1Disassembler dasm)
    {
        dasm.ops.Add(Constant.Byte((byte)bf3L3.Read(uInstr)));
        return true;
    }

    private static InstrDecoder<Pdp1Disassembler, Mnemonic, Pdp1Instruction> Instr(Mnemonic mnemonic, params Mutator<Pdp1Disassembler>[] mutators)
    {
        return new InstrDecoder<Pdp1Disassembler, Mnemonic, Pdp1Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static InstrDecoder<Pdp1Disassembler, Mnemonic, Pdp1Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Pdp1Disassembler>[] mutators)
    {
        return new InstrDecoder<Pdp1Disassembler, Mnemonic, Pdp1Instruction>(iclass, mnemonic, mutators);
    }

    static Pdp1Disassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        var skp = Instr(Mnemonic.skp);
        var szs = Instr(Mnemonic.szs, InstrClass.ConditionalTransfer, Sw);
        var szf = Instr(Mnemonic.szf, InstrClass.ConditionalTransfer, F);
        var decode6400 = Sparse(0, 6, "  6400..", skp,
            (0, szs),
            (1, szf),
            (2, szf),
            (3, szf),
            (4, szf),
            (5, szf),
            (6, szf),
            (7, szf),

            (8, szs),
            (16, szs),
            (24, szs),
            (32, szs),
            (40, szs),
            (48, szs),
            (56, szs));

        var skp_i = Instr(Mnemonic.skp_i);
        var szs_i = Instr(Mnemonic.szs_i, InstrClass.ConditionalTransfer, Sw);
        var szf_i = Instr(Mnemonic.szf_i, InstrClass.ConditionalTransfer, F);
        var decode6500 = Sparse(0, 6, "  6400..", skp_i,
            (0, szs_i),
            (1, szf_i),
            (2, szf_i),
            (3, szf_i),
            (4, szf_i),
            (5, szf_i),
            (6, szf_i),
            (7, szf_i),

            (8, szs_i),
            (16, szs_i),
            (24, szs_i),
            (32, szs_i),
            (40, szs_i),
            (48, szs_i),
            (56, szs_i));

        //$TODO: if low 6 bits are not zero, should be a skip instruction?
        var decode64 = Sparse(6, 6, "  64....", invalid,
            (0b000000, decode6400),
            (0b000001, Instr(Mnemonic.sza, InstrClass.ConditionalTransfer)),
            (0b000010, Instr(Mnemonic.spa, InstrClass.ConditionalTransfer)),
            (0b000100, Instr(Mnemonic.sma, InstrClass.ConditionalTransfer)),
            (0b001000, Instr(Mnemonic.szo, InstrClass.ConditionalTransfer)),
            (0b010000, Instr(Mnemonic.spi, InstrClass.ConditionalTransfer)));

        var decode65 = Sparse(6, 6, "  65....", invalid,
            (0b000000, decode6500),
            (0b000001, Instr(Mnemonic.sza_i, InstrClass.ConditionalTransfer)),
            (0b000010, Instr(Mnemonic.spa_i, InstrClass.ConditionalTransfer)),
            (0b000100, Instr(Mnemonic.sma_i, InstrClass.ConditionalTransfer)),
            (0b001000, Instr(Mnemonic.szo_i, InstrClass.ConditionalTransfer)),
            (0b010000, Instr(Mnemonic.spi_i, InstrClass.ConditionalTransfer)));

        var decode66 = Mask(9, 3, "  66....",
            invalid,
            Instr(Mnemonic.ral, Sh),
            Instr(Mnemonic.ril, Sh),
            Instr(Mnemonic.rcl, Sh),
            invalid,
            Instr(Mnemonic.sal, Sh),
            Instr(Mnemonic.sil, Sh),
            Instr(Mnemonic.scl, Sh));


        var decoder72 = Select((6, 6), n => n != 0, "  72....",
            invalid,
            Sparse(0, 6, "7200..", invalid,
                (0x01, Instr(Mnemonic.rpa)),
                (0x02, Instr(Mnemonic.rpb)),
                (0x04, Instr(Mnemonic.tyi)),
                (0x05, Instr(Mnemonic.ppa)),
                (0x06, Instr(Mnemonic.ppb)),
                (0x18, Instr(Mnemonic.rrb)),
                (0x1B, Instr(Mnemonic.cks)),
                (0x2C, Instr(Mnemonic.lsm)),
                (0x2D, Instr(Mnemonic.esm)),
                (0x2E, Instr(Mnemonic.cbs))));

        var decoder73 = Select((0, 12), n => n == 3, "  73....",
            Instr(Mnemonic.tyo),
            invalid);

        var decoder76 = Sparse(6, 6, "  76....", invalid,
            (0x00, Mask(3, 3, "  7600..",
                Select((0, 6), n => n == 0, "  760...",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    Instr(Mnemonic.clf, F)),
                Instr(Mnemonic.stf, F),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid)),
            (0x01, If(0, 6, n => n == 0, Instr(Mnemonic.lap))),
            (0x02, If(0, 6, n => n == 0, Instr(Mnemonic.cla))),
            (0x04, If(0, 6, n => n == 0, Instr(Mnemonic.hlt, InstrClass.Terminates))),
            (0x08, If(0, 6, n => n == 0, Instr(Mnemonic.cma))),
            (0x10, If(0, 6, n => n == 0, Instr(Mnemonic.lat))),
            (0x20, If(0, 6, n => n == 0, Instr(Mnemonic.cli))));

        var decode67 = Mask(9, 3, "  67....",
            invalid,
            Instr(Mnemonic.rar, Sh),
            Instr(Mnemonic.rir, Sh),
            Instr(Mnemonic.rcr, Sh),
            invalid,
            Instr(Mnemonic.sar, Sh),
            Instr(Mnemonic.sir, Sh),
            Instr(Mnemonic.scr, Sh));

        rootDecoder = Mask(12, 6, "PDP-1",
            invalid,
            invalid,
            Instr(Mnemonic.and, Y),
            Instr(Mnemonic.and_i, Y),
            Instr(Mnemonic.ior, Y),
            Instr(Mnemonic.ior_i, Y),
            Instr(Mnemonic.xor, Y),
            Instr(Mnemonic.xor_i, Y),

            Instr(Mnemonic.xct, Y),
            Instr(Mnemonic.xct_i, Y),
            invalid,
            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.cal, Y),
            Instr(Mnemonic.jda, InstrClass.Transfer, Y),

            Instr(Mnemonic.lac, Y),
            Instr(Mnemonic.lac_i, Y),
            Instr(Mnemonic.lio, Y),
            Instr(Mnemonic.lio_i, Y),
            Instr(Mnemonic.dac, Y),
            Instr(Mnemonic.dac_i, Y),
            Instr(Mnemonic.dap, Y),
            Instr(Mnemonic.dap_i, Y),

            Instr(Mnemonic.dip, Y),
            Instr(Mnemonic.dip_i, Y),
            Instr(Mnemonic.dio, Y),
            Instr(Mnemonic.dio_i, Y),
            Instr(Mnemonic.dzm, Y),
            Instr(Mnemonic.dzm_i, Y),
            invalid,
            invalid,

            Instr(Mnemonic.add, Y),
            Instr(Mnemonic.add_i, Y),
            Instr(Mnemonic.sub, Y),
            Instr(Mnemonic.sub_i, Y),
            Instr(Mnemonic.idx, Y),
            Instr(Mnemonic.idx_i, Y),
            Instr(Mnemonic.isp, InstrClass.ConditionalTransfer, Y),
            Instr(Mnemonic.isp_i, InstrClass.ConditionalTransfer, Y),

            Instr(Mnemonic.sad, InstrClass.ConditionalTransfer, Y),
            Instr(Mnemonic.sad_i, InstrClass.ConditionalTransfer, Y),
            Instr(Mnemonic.sas, InstrClass.ConditionalTransfer, Y),
            Instr(Mnemonic.sas_i, InstrClass.ConditionalTransfer, Y),
            Instr(Mnemonic.mul, Y),
            Instr(Mnemonic.mul_i, Y),
            Instr(Mnemonic.div, Y),
            Instr(Mnemonic.div_i, Y),
            //{ Mnemonic.mus,   0540000, 0770000, PDP1_INSTR_MEMREF },
            //{ Mnemonic.mus_i, 0550000, 0770000, PDP1_INSTR_MEMREF },
            //{ Mnemonic.dis,   0560000, 0770000, PDP1_INSTR_MEMREF },
            //{ Mnemonic.dis_i, 0570000, 0770000, PDP1_INSTR_MEMREF },

            Instr(Mnemonic.jmp, InstrClass.Transfer, Y),
            Instr(Mnemonic.jmp_i, InstrClass.Transfer, Y),
            Instr(Mnemonic.jsp, InstrClass.Transfer | InstrClass.Call, Y),
            Instr(Mnemonic.jsp_i, InstrClass.Transfer | InstrClass.Call, Y),
            decode64,
            decode65,
            decode66,
            decode67,

            Instr(Mnemonic.law, Npos),
            Instr(Mnemonic.law, Nneg),
            decoder72,
            decoder73,
            invalid,
            invalid,
            decoder76,
            invalid);
        }

}
