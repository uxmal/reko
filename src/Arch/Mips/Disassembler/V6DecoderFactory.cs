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

using Reko.Arch.Mips.Machine;
using Reko.Core.Machine;

namespace Reko.Arch.Mips.Disassembler;

public partial class MipsDisassembler
{
    public class V6DecoderFactory : DecoderFactory
    {
        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Special3_Decode36()
        {
            return Instr(Mnemonic.ll, R2, ew);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Special3_Decode37()
        {
            return new A64Decoder(Mnemonic.lld, R2, el);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1x()
        {
            // COP1x removed in MIPS v6
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop2()
        {
            var cop2 = Mask(21, 5, "COP2",   // 12: COP2 
                 Nyi("mfc2"),
                 invalid,
                 Nyi("cfc2"),
                 Nyi("mfhc2"),
                 Nyi("mtc2"),
                 invalid,
                 Nyi("ctc2"),
                 Nyi("mthc2"),

                 Nyi("bc2"),
                 Nyi("bc2eqz"),
                 Instr(Mnemonic.lwc2, R2, E11w),
                 Nyi("swc2"),
                 invalid,
                 Nyi("bc2nez"),
                 Nyi("ldc2"),
                 Nyi("sdc2"),

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
                 invalid,
                 invalid);
                        return cop2;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1C()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1D()
        {
            return Nyi("POP6");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1E()
        {
            return Nyi("POP7");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode2F()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode30()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode32()
        {
            return Nyi("BC-v6");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode34()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode36()
        {
            return Nyi("POP76");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode38()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3A()
        {
            return Nyi("BALC-v6");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3B()
        {
            return Nyi("PCREL-v6");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3C()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3E()
        {
            return Nyi("POP76");
        }
    }
}
