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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Dec.Linc;

public class LincDisassembler : DisassemblerBase<LincInstruction, Mnemonic>
{
    private static readonly Decoder<LincDisassembler, Mnemonic, LincInstruction> rootDecoder;

    private readonly EndianImageReader rdr;
    private readonly LincArchitecture arch;
    private readonly List<MachineOperand> ops;

    private Address addr;
    private bool indirect;
    private ushort uInstrFirst;
    private bool nextLiteral;

    public LincDisassembler(LincArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override LincInstruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!this.rdr.TryReadBeUInt16(out this.uInstrFirst))
            return null;
        var instr = nextLiteral
            ? MakeLiteral(uInstrFirst)
            : rootDecoder.Decode(uInstrFirst, this);

        ops.Clear();
        indirect = false;

        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        return instr;
    }


    public override LincInstruction CreateInvalidInstruction()
    {
        return new LincInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override LincInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new LincInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Indirect = indirect,
            HasLiteral = nextLiteral,
            Operands = ops.ToArray()
        };
    }

    private LincInstruction MakeLiteral(ushort uInstr)
    {
        this.nextLiteral = false;
        return new LincInstruction
        {
            InstructionClass = InstrClass.Linear,
            Mnemonic = Mnemonic.Literal,
            Operands = [ Constant.Create(PdpTypes.Word12, uInstr) ]
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

    public override LincInstruction NotYetImplemented(string message)
    {
        var testSvc = this.arch.Services.GetService<ITestGenerationService>();
        var sOpcode = Octize(this.rdr.Clone());
        testSvc?.ReportMissingDecoder("LincDis", this.addr, message, sOpcode);
        return new LincInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Nyi,
        };
    }

    private static bool Direct(uint uInstr, LincDisassembler dasm)
    {
        var uAddr = uInstr & 0x3FF;
        var op = new MemoryOperand(uAddr);
        dasm.ops.Add(op);
        return true;
    }

    private static bool I(uint uInstr, LincDisassembler dasm)
    {
        dasm.indirect = (dasm.uInstrFirst & 0x10) != 0;
        return true;
    }

    /// <summary>
    /// Four bit constant in LSBs
    /// </summary>
    private static bool N4(uint uInstr, LincDisassembler dasm)
    {
        var u = uInstr & 0xFu;
        dasm.ops.Add(Constant.Create(PrimitiveType.Int8, u));
        return true;
    }

    private static bool N5(uint uInstr, LincDisassembler dasm)
    {
        var u = uInstr & 0x1Fu;
        dasm.ops.Add(Constant.Create(PrimitiveType.Int8, u));
        return true;
    }

    private static bool W12(uint uInstr, LincDisassembler dasm)
    {
        dasm.nextLiteral = true;
        return true;
    }

    private static bool Alpha(uint uInstr, LincDisassembler dasm)
    {
        var alpha = uInstr & 0xF;
        dasm.ops.Add(new MemoryOperand(alpha));
        return true;
    }

    private static bool Beta(uint uInstr, LincDisassembler dasm)
    {
        var beta = uInstr & 0xF;
        if (beta == 0)
        {
            dasm.nextLiteral = true;
        }
        else
        {
            dasm.ops.Add(new MemoryOperand(beta));
        }
        return true;
    }

    private static bool U(uint uInstr, LincDisassembler dasm)
    {
        var u = (dasm.uInstrFirst & 0x8) >> 3;
        dasm.ops.Add(Constant.Create(PrimitiveType.Byte, u));
        return true;
    }

    /// <summary>
    /// Decodes a tape-related instruction using the current disassembly context.
    /// </summary>
    private static bool Tape(uint uInstr, LincDisassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out ushort t))
            return false;
        uint mblk = (uint)t >> 9;
        uint tblk = (uint)t & 0x1FF;
        dasm.ops.Add(new TapeBlockOperator(mblk, tblk));
        return true;
    }

    private class NextWordDecoder : Decoder<LincDisassembler, Mnemonic, LincInstruction>
    {
        private readonly Decoder<LincDisassembler, Mnemonic, LincInstruction> decoder;

        public NextWordDecoder(Decoder<LincDisassembler, Mnemonic, LincInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override LincInstruction Decode(uint wInstr, LincDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var uInstr))
                return dasm.CreateInvalidInstruction();
            return decoder.Decode(uInstr, dasm);
        }
    }

    private static InstrDecoder<LincDisassembler, Mnemonic, LincInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<LincDisassembler>[] mutators)
    {
        return new InstrDecoder<LincDisassembler, Mnemonic, LincInstruction>(iclass, mnemonic, mutators);
    }

    private static InstrDecoder<LincDisassembler, Mnemonic, LincInstruction> Instr(Mnemonic mnemonic, params Mutator<LincDisassembler>[] mutators)
    {
        return new InstrDecoder<LincDisassembler, Mnemonic, LincInstruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static NextWordDecoder Next(Decoder<LincDisassembler, Mnemonic, LincInstruction> decoder)
    {
        return new NextWordDecoder(decoder);
    }

    static LincDisassembler()
    {
        var nyi = Instr(Mnemonic.Nyi, InstrClass.Invalid);

        var alpha0000 = Mask(0, 5, "  alpha 0000",
            Instr(Mnemonic.halt, InstrClass.Terminates|InstrClass.Zero),
            Instr(Mnemonic.axo),
            Instr(Mnemonic.pdp),
            Instr(Mnemonic.tac),

            nyi,
            Instr(Mnemonic.qac),
            Instr(Mnemonic.djr),
            nyi,

            nyi,
            Instr(Mnemonic.clr),
            nyi,
            nyi,

            Instr(Mnemonic.atr),
            Instr(Mnemonic.rta),
            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
            Instr(Mnemonic.com),
            
            nyi,
            Instr(Mnemonic.xoa),
            nyi,
            Instr(Mnemonic.tma),

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
            nyi);

        var alpha1000 = Sparse(0, 4, "  alpha 1000", nyi,
            (0b0011, Instr(Mnemonic.sxl, I, N4)), //??
            (0b1101, Instr(Mnemonic.kst, I)),
            (0b1110, Instr(Mnemonic.std, I)),
            (0b1111, Instr(Mnemonic.twc, I)));

        var sns = Instr(Mnemonic.sns, I, N4);

        var alpha1001 = Sparse(0, 4, "  alpha 1001", nyi,
            (0b0000, sns),
            (0b0001, sns),
            (0b0010, sns),
            (0b0011, sns),
            (0b0100, sns),
            (0b0101, sns),
            (0b1000, Instr(Mnemonic.aze, I)),
            (0b1001, Instr(Mnemonic.apo, I)),
            (0b1010, Instr(Mnemonic.lze, I)),
            (0b1011, Instr(Mnemonic.ibz, I)),
            (0b1100, Instr(Mnemonic.flo, I)),
            (0b1101, Instr(Mnemonic.qlz, I)),
            (0b1110, Instr(Mnemonic.skp, I)));

        var iob = Next(Sparse(0, 16, "  iob", nyi,
            (0b110_000_000_001, Instr(Mnemonic.ion)),
            (0b110_000_000_010, Instr(Mnemonic.iof)),
            (0b110_001_101_001, Instr(Mnemonic.lmr)),
            (0b110_010_001_100, Instr(Mnemonic.rdf)),
            (0b110_010_010_100, Instr(Mnemonic.rif)),
            (0b110_010_011_100, Instr(Mnemonic.rib)),
            (0b110_010_100_100, Instr(Mnemonic.rmf))));

    var alpha1010 = Sparse(0, 4, "  alpha 1010", nyi,
            (0b0000, iob),
            (0b1110, Instr(Mnemonic.rsw)),
            (0b1111, Instr(Mnemonic.lsw)));

        var alpha1110 = Sparse(0, 3, "  alpha 1110", nyi,
            (0b0000, Instr(Mnemonic.rcg, I, U, Tape)),
            (0b0001, Instr(Mnemonic.rdc, I, U, Tape)),
            (0b0010, Instr(Mnemonic.rde, I, U, Tape)),
            (0b0011, Instr(Mnemonic.mtb, I, U, Tape)),
            (0b0100, Instr(Mnemonic.wrc, I, U, Tape)),
            (0b0101, Instr(Mnemonic.wcg, I, U, Tape)),
            (0b0110, Instr(Mnemonic.wri, I, U, Tape)),
            (0b0111, Instr(Mnemonic.chk, I, U, Tape)));

        var alphaDecoders = Mask(5, 4, "  alpha",
            alpha0000,
            Instr(Mnemonic.set, I, Alpha, W12),
            Instr(Mnemonic.sam, N5),
            Instr(Mnemonic.dis, I, Alpha),

            Instr(Mnemonic.xsk, I, Alpha),
            Instr(Mnemonic.rol, I, N4),
            Instr(Mnemonic.ror, I, N4),
            Instr(Mnemonic.scr, I, N4),

            alpha1000,
            alpha1001,
            alpha1010,
            nyi,

            Instr(Mnemonic.lif, N5),
            Instr(Mnemonic.ldf, N5),
            alpha1110,
            nyi);
        var betaDecoders = Mask(5, 4, "  beta",
            Instr(Mnemonic.lda, I, Beta),
            Instr(Mnemonic.sta, I, Beta),
            Instr(Mnemonic.ada, I, Beta),
            Instr(Mnemonic.adm, I, Beta),

            Instr(Mnemonic.lam, I, Beta),
            Instr(Mnemonic.mul, I, Beta),
            Instr(Mnemonic.ldh, I, Beta),
            Instr(Mnemonic.sth, I, Beta),

            Instr(Mnemonic.shd, I, Beta),
            Instr(Mnemonic.sae, I, Beta),
            Instr(Mnemonic.sro, I, Beta),
            Instr(Mnemonic.bcl, I, Beta),

            Instr(Mnemonic.bse, I, Beta),
            Instr(Mnemonic.bco, I, Beta),
            nyi,
            Instr(Mnemonic.dsc, I, Beta));

        var add = Instr(Mnemonic.add, InstrClass.Linear, Direct);
        var stc = Instr(Mnemonic.stc, InstrClass.Linear, Direct);
        var jmp = Instr(Mnemonic.jmp, InstrClass.Jump, Direct);

        rootDecoder = Mask(9, 3, "LINC",
            alphaDecoders,
            betaDecoders,
            add,
            add,
            stc,
            stc,
            jmp,
            jmp);
    }
}
