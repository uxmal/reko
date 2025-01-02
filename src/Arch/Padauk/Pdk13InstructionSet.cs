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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Padauk
{
    using Decoder = Reko.Core.Machine.Decoder<PadaukDisassembler, Mnemonic, PadaukInstruction>;

    partial class PadaukDisassembler
    {

        internal class Pdk13InstructionSet : InstructionSet
        {
            // https://github.com/free-pdk/fppa-pdk-documentation/blob/master/fppa_instructions_sets_notes/13bit.txt
            // Reference: http://www.eevblog.com/forum/blog/eevblog-1144-padauk-programmer-reverse-engineering/msg1990262/#msg1990262
            // Thanks to user oPossum

            public override Decoder CreateDecoder()
            {
                var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

                var decoder0_0000_000 = Sparse(0, 5, "", invalid,
                    (0b0_0000, Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Zero|InstrClass.Padding)),
                    (0b0_0001, Instr(Mnemonic.trap)),   //$REVIEW: this is a guess. Can't find it in the docs provided.
                    (0b1_0000, Instr(Mnemonic.addc, a)),
                    (0b1_0001, Instr(Mnemonic.subc, a)),
                    (0b1_0010, Instr(Mnemonic.izsn, InstrClass.ConditionalTransfer, a)),
                    (0b1_0011, Instr(Mnemonic.dzsn, InstrClass.ConditionalTransfer, a)),
                    (0b1_0111, Instr(Mnemonic.pcadd, a)),
                    (0b1_1000, Instr(Mnemonic.not, a)),
                    (0b1_1001, Instr(Mnemonic.neg, a)),
                    (0b1_1010, Instr(Mnemonic.sr, a)),
                    (0b1_1011, Instr(Mnemonic.sl, a)),
                    (0b1_1100, Instr(Mnemonic.src, a)),
                    (0b1_1101, Instr(Mnemonic.slc, a)),
                    (0b1_1110, Instr(Mnemonic.swap, a)));

                var decoder0_0000_001 = Sparse(0, 5, "", invalid,
                    (0b1_0000, Instr(Mnemonic.wdreset)),
                    (0b1_0010, Instr(Mnemonic.pushaf)),
                    (0b1_0011, Instr(Mnemonic.popaf)),
                    (0b1_0101, Instr(Mnemonic.reset)),
                    (0b1_0110, Instr(Mnemonic.stopsys)),
                    (0b1_0111, Instr(Mnemonic.stopexe)),
                    (0b1_1000, Instr(Mnemonic.engint)),
                    (0b1_1001, Instr(Mnemonic.disgint)),
                    (0b1_1010, Instr(Mnemonic.ret, InstrClass.Transfer|InstrClass.Return)),
                    (0b1_1011, Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return)));

                var decoder0_0000 = Mask(5, 3, "",
                    decoder0_0000_000,
                    decoder0_0000_001,
                    invalid,
                    Instr(Mnemonic.xor, p0_5, a),

                    Instr(Mnemonic.mov, p0_5, a),
                    Instr(Mnemonic.mov, a, p0_5),
                    Mask(0, 1,
                        Instr(Mnemonic.stt16, w1_4),
                        Instr(Mnemonic.ldt16, w1_4)),
                    Mask(0, 1,
                        Instr(Mnemonic.idxm, im1_4, a),
                        Instr(Mnemonic.idxm, a, im1_4)));

                var decoder0_00 = Mask(8, 2, "  0_00",
                    decoder0_0000,
                    Instr(Mnemonic.ret, InstrClass.Transfer|InstrClass.Return, i0_8),
                    Mask(4, 1, "",
                        Instr(Mnemonic.t0sn, InstrClass.ConditionalTransfer, Mn_0_5),
                        Instr(Mnemonic.t1sn, InstrClass.ConditionalTransfer, Mn_0_5)),
                    Mask(4, 1, "",
                        Instr(Mnemonic.set0, m0_4, i5_3),
                        Instr(Mnemonic.set1, m0_4, i5_3)));

                var decoder0_01 = PadaukDisassembler.Mask(6, 4, "  0_01",
                    Instr(Mnemonic.add, m0_6, a),
                    Instr(Mnemonic.sub, m0_6, a),
                    Instr(Mnemonic.addc, m0_6, a),
                    Instr(Mnemonic.subc, m0_6, a),
                    Instr(Mnemonic.and, m0_6, a),
                    Instr(Mnemonic.or, m0_6, a),
                    Instr(Mnemonic.xor, m0_6, a),
                    Instr(Mnemonic.mov, m0_6, a),

                    Instr(Mnemonic.add, a, m0_6),
                    Instr(Mnemonic.sub, a, m0_6),
                    Instr(Mnemonic.addc, a, m0_6),
                    Instr(Mnemonic.subc, a, m0_6),
                    Instr(Mnemonic.and, a, m0_6),
                    Instr(Mnemonic.or, a, m0_6),
                    Instr(Mnemonic.xor, a, m0_6),
                    Instr(Mnemonic.mov, a, m0_6));

                var decoder0_10 = PadaukDisassembler.Mask(6, 4, "  0_10",
                    invalid,
                    invalid,
                    Instr(Mnemonic.izsn, m0_6),
                    Instr(Mnemonic.dzsn, m0_6),

                    Instr(Mnemonic.inc, m0_6),
                    Instr(Mnemonic.dec, m0_6),
                    Instr(Mnemonic.clear, m0_6),
                    Instr(Mnemonic.xch, m0_6),

                    Instr(Mnemonic.not, m0_6),
                    Instr(Mnemonic.neg, m0_6),
                    Instr(Mnemonic.sr, m0_6),
                    Instr(Mnemonic.sl, m0_6),

                    Instr(Mnemonic.src, m0_6),
                    Instr(Mnemonic.slc, m0_6),
                    Instr(Mnemonic.ceqsn, InstrClass.Transfer | InstrClass.Conditional, a, m0_6),
                    invalid);

                var decoder0_11 = Mask(8, 2, "  0_11",
                    Instr(Mnemonic.t0sn, InstrClass.ConditionalTransfer, Pn_0_5),
                    Instr(Mnemonic.t1sn, InstrClass.ConditionalTransfer, Pn_0_5),
                    Instr(Mnemonic.set0, Pn_0_5),
                    Instr(Mnemonic.set1, Pn_0_5));
                var decoder1_00 = Mask(8, 2, "  1_00",
                    Instr(Mnemonic.add, a, i0_8),
                    Instr(Mnemonic.sub, a, i0_8),
                    Instr(Mnemonic.ceqsn, InstrClass.Transfer|InstrClass.Conditional, a, i0_8),
                    invalid);

                var decoder1_01 = PadaukDisassembler.Mask(8, 2, "  1_01",
                    Instr(Mnemonic.and, a, i0_8),
                    Instr(Mnemonic.or, a, i0_8),
                    Instr(Mnemonic.xor, a, i0_8),
                    Instr(Mnemonic.mov, a, i0_8));

                return Mask(10, 3, "PDK13",
                    decoder0_00,
                    decoder0_01,
                    decoder0_10,
                    decoder0_11,

                    decoder1_00,
                    decoder1_01,
                    Instr(Mnemonic.@goto, InstrClass.Transfer, a10),
                    Instr(Mnemonic.call, InstrClass.Transfer, a10));
            }
        }
    }
}