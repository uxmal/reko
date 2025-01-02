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
using Reko.Core.Machine;

namespace Reko.Arch.Padauk
{
    partial class PadaukDisassembler
    {
        internal class Pdk14InstructionSet : InstructionSet
        {
            // https://github.com/free-pdk/fppa-pdk-documentation/blob/master/fppa_instructions_sets_notes/13bit.txt
            // Reference: http://www.eevblog.com/forum/blog/eevblog-1144-padauk-programmer-reverse-engineering/msg1990253/#msg1990253
            // Thanks to user oPossum

            public override Decoder<PadaukDisassembler, Mnemonic, PadaukInstruction> CreateDecoder()
            {
                var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
                var decoder00_0000_0 = Sparse(0, 7, "", invalid,
                    (0b000_0000, Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero)),
                    (0b000_0110, Instr(Mnemonic.ldsptl)),
                    (0b000_0111, Instr(Mnemonic.ldspth)),
                    (0b110_0000, Instr(Mnemonic.addc, a)),
                    (0b110_0001, Instr(Mnemonic.subc, a)),
                    (0b110_0010, Instr(Mnemonic.izsn, a)),
                    (0b110_0011, Instr(Mnemonic.dzsn, a)),
                    (0b110_0111, Instr(Mnemonic.pcadd, a)),
                    (0b110_1000, Instr(Mnemonic.not, a)),
                    (0b110_1001, Instr(Mnemonic.neg, a)),
                    (0b110_1010, Instr(Mnemonic.sr, a)),
                    (0b110_1011, Instr(Mnemonic.sl, a)),
                    (0b110_1100, Instr(Mnemonic.src, a)),
                    (0b110_1101, Instr(Mnemonic.slc, a)),
                    (0b110_1110, Instr(Mnemonic.swap, a)),
                    (0b111_0000, Instr(Mnemonic.wdreset)),
                    (0b111_0010, Instr(Mnemonic.pushaf)),
                    (0b111_0011, Instr(Mnemonic.popaf)),
                    (0b111_0101, Instr(Mnemonic.reset)),
                    (0b111_0110, Instr(Mnemonic.stopsys)),
                    (0b111_0111, Instr(Mnemonic.stopexe)),
                    (0b111_1000, Instr(Mnemonic.engint)),
                    (0b111_1001, Instr(Mnemonic.disgint)),
                    (0b111_1010, Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return)),
                    (0b111_1011, Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return)),
                    (0b111_1100, Instr(Mnemonic.mul)));

                var decoder00_0 = Mask(7, 4, "",
                    decoder00_0000_0,
                    Mask(6, 1, "",
                        invalid,
                        Instr(Mnemonic.xor, p0_6, a)),
                    invalid,
                    Mask(6, 1, "",
                        Instr(Mnemonic.mov, p0_6, a),
                        Instr(Mnemonic.mov, a, p0_6)),

                    Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, i0_8),
                    Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, i0_8),
                    Mask(0, 1, "",
                        Instr(Mnemonic.stt16, w1_7),
                        Instr(Mnemonic.ldt16, w1_7)),
                    Mask(0, 1, "",
                        Instr(Mnemonic.idxm, im1_7, a),
                        Instr(Mnemonic.idxm, a, im1_7)),

                    Instr(Mnemonic.swapc, Pn_0_6),
                    Instr(Mnemonic.swapc, Pn_0_6),
                    Instr(Mnemonic.swapc, Pn_0_6),
                    Instr(Mnemonic.swapc, Pn_0_6),

                    Instr(Mnemonic.comp, a, m0_7),
                    Instr(Mnemonic.comp, m0_7, a),
                    Instr(Mnemonic.nadd, a, m0_7),
                    Instr(Mnemonic.nadd, m0_7, a));

                var decoder00_1 = Mask(7, 4, "",
                    Instr(Mnemonic.add, m0_7, a),
                    Instr(Mnemonic.sub, m0_7, a),
                    Instr(Mnemonic.addc, m0_7, a),
                    Instr(Mnemonic.subc, m0_7, a),

                    Instr(Mnemonic.and, m0_7, a),
                    Instr(Mnemonic.or, m0_7, a),
                    Instr(Mnemonic.xor, m0_7, a),
                    Instr(Mnemonic.mov, m0_7, a),

                    Instr(Mnemonic.add, a, m0_7),
                    Instr(Mnemonic.sub, a, m0_7),
                    Instr(Mnemonic.addc, a, m0_7),
                    Instr(Mnemonic.subc, a, m0_7),

                    Instr(Mnemonic.and, a, m0_7),
                    Instr(Mnemonic.or, a, m0_7),
                    Instr(Mnemonic.xor, a, m0_7),
                    Instr(Mnemonic.mov, a, m0_7));

                var decoder01_0 = Mask(7, 4, "",
                    Instr(Mnemonic.addc, m0_7),
                    Instr(Mnemonic.subc, m0_7),
                    Instr(Mnemonic.izsn, InstrClass.ConditionalTransfer, m0_7),
                    Instr(Mnemonic.dzsn, InstrClass.ConditionalTransfer, m0_7),

                    Instr(Mnemonic.inc, m0_7),
                    Instr(Mnemonic.dec, m0_7),
                    Instr(Mnemonic.clear, m0_7),
                    Instr(Mnemonic.xch, m0_7),

                    Instr(Mnemonic.not, m0_7),
                    Instr(Mnemonic.neg, m0_7),
                    Instr(Mnemonic.sr, m0_7),
                    Instr(Mnemonic.sl, m0_7),

                    Instr(Mnemonic.src, m0_7),
                    Instr(Mnemonic.slc, m0_7),
                    Instr(Mnemonic.ceqsn, InstrClass.Transfer | InstrClass.Conditional, a, m0_7),
                    Instr(Mnemonic.cneqsn, InstrClass.Transfer | InstrClass.Conditional, a, m0_7));

                var decoder01_1 = Mask(9, 2, "",
                    Instr(Mnemonic.t0sn, Pn_0_6),
                    Instr(Mnemonic.t1sn, Pn_0_6),
                    Instr(Mnemonic.set0, Pn_0_6),
                    Instr(Mnemonic.set1, Pn_0_6));

                var decoder10_0 = Mask(9, 2, "",
                    Instr(Mnemonic.t0sn, InstrClass.ConditionalTransfer, Mn_0_6),
                    Instr(Mnemonic.t1sn, InstrClass.ConditionalTransfer, Mn_0_6),
                    Instr(Mnemonic.set0, Mn_0_6),
                    Instr(Mnemonic.set1, Mn_0_6));

                var decoder10_1 = Mask(8, 3, "",
                     Instr(Mnemonic.add, a, i0_8),
                     Instr(Mnemonic.sub, a, i0_8),
                     Instr(Mnemonic.ceqsn, InstrClass.Transfer | InstrClass.Conditional, a, i0_8),
                     Instr(Mnemonic.cneqsn, InstrClass.Transfer | InstrClass.Conditional, a, i0_8),
                     Instr(Mnemonic.and, a, i0_8),
                     Instr(Mnemonic.or, a, i0_8),
                     Instr(Mnemonic.xor, a, i0_8),
                     Instr(Mnemonic.mov, a, i0_8));

                return Mask(11, 3, "PDK14",
                    decoder00_0,
                    decoder00_1,
                    decoder01_0,
                    decoder01_1,

                    decoder10_0,
                    decoder10_1,
                    Instr(Mnemonic.@goto, InstrClass.Transfer, a11),
                    Instr(Mnemonic.call, InstrClass.Transfer | InstrClass.Call, a11));
            }
        }
    }
}
