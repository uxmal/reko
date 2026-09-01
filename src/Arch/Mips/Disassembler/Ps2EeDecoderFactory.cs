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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System.Diagnostics;

namespace Reko.Arch.Mips.Disassembler;

partial class MipsDisassembler
{

    public class Ps2EeDecoderFactory : DecoderFactory
    {
        private static Mutator<MipsDisassembler> UImm_Shift(int bitpos, int bitlen, int shift)
        {
            var field = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                uint imm = field.Read(u) << shift;
                d.ops.Add(Constant.UInt32(imm));
                return true;
            };
        }

        private static Mutator<MipsDisassembler> VFdecoder(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                Decoder.DumpMaskedInstruction(32, u, field.Mask << field.Position, "  vf");
                var vf = field.Read(u);
                Debug.Assert(d.arch.vfRegisters is not null);
                d.ops.Add(d.arch.vfRegisters[vf]);
                return true;
            };
        }
        private static Mutator<MipsDisassembler> Vf6 => VFdecoder(6);
        private static Mutator<MipsDisassembler> Vf11 => VFdecoder(11);
        private static Mutator<MipsDisassembler> Vf16 => VFdecoder(16);


        private static Mutator<MipsDisassembler> VIDecoder(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                Decoder.DumpMaskedInstruction(32, u, field.Mask << field.Position, "  vi");
                var vi = field.Read(u);
                Debug.Assert(d.arch.viRegisters is not null);
                if (vi >= (uint) d.arch.viRegisters.Length)
                    return false;
                d.ops.Add(d.arch.viRegisters[vi]);
                return true;
            };
        }
        private static Mutator<MipsDisassembler> Vi6 => VIDecoder(6);
        private static Mutator<MipsDisassembler> Vi11 => VIDecoder(11);
        private static Mutator<MipsDisassembler> Vi16 => VIDecoder(16);

        private static readonly Bitfield bf_11_5 = new Bitfield(11, 5);

        private static bool CPR0_3(uint uInstr, MipsDisassembler dasm)
        {
            Debug.Assert(dasm.arch.cop0Registers is not null);
            var copreg = dasm.arch.cop0Registers[bf_11_5.Read(uInstr)];
            dasm.ops.Add(copreg);
            return true;
        }

        // Implicit destination of the VU0 "A-forms" (accumulate into ACC).
        private static readonly Mutator<MipsDisassembler> AccR = (u, d) =>
        {
            Debug.Assert(d.arch.acc is not null);
            d.ops.Add(d.arch.acc); 
            return true;
        };


        // Destination mask of VU0 macro mode instructions; the four
        // bits [24:21] select which of the x/y/z/w lanes are written.
        private static bool mask(uint wInstr, MipsDisassembler d)
        {
            d.laneMask = (int) (wInstr >> 21) & 0xF;
            return true;
        }

        // Source component selectors used by VDIV, VSQRT, VRSQRT,
        // VMTIR et al, found in bits [22:21] and [24:23].
        internal static bool fsf(uint wInstr, MipsDisassembler d)
        {
            d.laneMask = (int) (wInstr >> 21) & 3;
            return true;
        }

        // Target component selectors used by VDIV, VSQRT, VRSQRT,
        // VMTIR et al, found in bits [22:21] and [24:23].
        internal static bool ftf(uint wInstr, MipsDisassembler d)
        {
            d.laneMask = (int) (wInstr >> 23) & 3;
            return true;
        }


        /// <summary>
        /// Specific reference to the I register.
        /// </summary>
        private static bool I(uint uInstr, MipsDisassembler dasm)
        {
            Debug.Assert(dasm.arch.i is not null);
            dasm.ops.Add(dasm.arch.i);
            return true;
        }

        /// <summary>
        /// Specific reference to the Q register.
        /// </summary>
        private static bool Q(uint uInstr, MipsDisassembler dasm)
        {
            Debug.Assert(dasm.arch.q is not null);
            dasm.ops.Add(dasm.arch.q);
            return true;
        }

        /// <summary>
        /// Specific reference to the R register.
        /// </summary>
        private static bool R(uint uInstr, MipsDisassembler dasm)
        {
            Debug.Assert(dasm.arch.r is not null);
            dasm.ops.Add(dasm.arch.r);
            return true;
        }


        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1C()
        {
            var mmi0 = Mask(6, 5, "  PS2 - MMI0",
                Instr(Mnemonic.paddw, R3, R1, R2),
                Instr(Mnemonic.psubw, R3, R1, R2),
                Instr(Mnemonic.pcgtw, R3, R1, R2),
                Instr(Mnemonic.pmaxw, R3, R1, R2),

                Instr(Mnemonic.paddh, R3, R1, R2),
                Instr(Mnemonic.psubh, R3, R1, R2),
                Instr(Mnemonic.pcgth, R3, R1, R2),
                Instr(Mnemonic.pmaxh, R3, R1, R2),

                Instr(Mnemonic.paddb, R3, R1, R2),
                Instr(Mnemonic.psubb, R3, R1, R2),
                Instr(Mnemonic.pcgtb, R3, R1, R2),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.paddsw, R3, R1, R2),
                Instr(Mnemonic.psubsw, R3, R1, R2),
                Instr(Mnemonic.pextlw, R3, R1, R2),
                Instr(Mnemonic.ppacw, R3, R1, R2),

                Instr(Mnemonic.paddsh, R3, R1, R2),
                Instr(Mnemonic.psubsh, R3, R1, R2),
                Instr(Mnemonic.pextlh, R3, R1, R2),
                Instr(Mnemonic.ppach, R3, R1, R2),

                Instr(Mnemonic.paddsb, R3, R1, R2),
                Instr(Mnemonic.psubsb, R3, R1, R2),
                Instr(Mnemonic.pextlb, R3, R1, R2),
                Instr(Mnemonic.ppacb, R3, R1, R2),

                invalid,
                invalid,
                Instr(Mnemonic.pext5, R3, R2),
                Instr(Mnemonic.ppac5, R3, R2));

            var mmi1 = Mask(6, 5, "  PS2 - MMI1",
                invalid,
                Instr(Mnemonic.pabsw, R3, R2),
                Instr(Mnemonic.pceqw, R3, R1, R2),
                Instr(Mnemonic.pminw, R3, R1, R2),

                Instr(Mnemonic.padsbh, R3, R1, R2),
                Instr(Mnemonic.pabsh, R3, R2),
                Instr(Mnemonic.pceqh, R3, R2),
                Instr(Mnemonic.pminh, R3, R2),

                invalid,
                invalid,
                Instr(Mnemonic.pceqb, R3, R1, R2),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.padduw, R3, R1, R2),
                Instr(Mnemonic.psubuw, R3, R1, R2),
                Instr(Mnemonic.pextuw, R3, R1, R2),
                invalid,

                Instr(Mnemonic.padduh, R3, R1, R2),
                Instr(Mnemonic.psubuh, R3, R1, R2),
                Instr(Mnemonic.pextuh, R3, R1, R2),
                invalid,

                Instr(Mnemonic.paddub, R3, R1, R2),
                Instr(Mnemonic.psubub, R3, R1, R2),
                Instr(Mnemonic.pextub, R3, R1, R2),
                Instr(Mnemonic.qfsrv, R3, R1, R2),

                invalid,
                invalid,
                invalid,
                invalid);

            var mmi2 = Mask(6, 5, "  PS2 - MMI2",
                Instr(Mnemonic.pmaddw, R3, R1, R2),
                invalid,
                Instr(Mnemonic.psllvw, R3, R1, R2),
                Instr(Mnemonic.psrlvw, R3, R1, R2),

                Instr(Mnemonic.pmsubw, R3, R1, R2),
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.pmfhi, R3),
                Instr(Mnemonic.pmflo, R3),
                Instr(Mnemonic.pinth, R3, R1, R2),
                invalid,

                Instr(Mnemonic.pmultw, R3, R1, R2),
                Instr(Mnemonic.pdivw, R1, R2),
                Instr(Mnemonic.pcpyld, R3, R1, R2),
                invalid,

                Instr(Mnemonic.pmaddh, R3, R1, R2),
                Instr(Mnemonic.phmadh, R3, R1, R2),
                Instr(Mnemonic.pand, R3, R1, R2),
                Instr(Mnemonic.pxor, R3, R1, R2),

                Instr(Mnemonic.pmsubh, R3, R1, R2),
                Instr(Mnemonic.phmsbh, R3, R1, R2),
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.pexeh, R3, R2),
                Instr(Mnemonic.prevh, R3, R2),

                Instr(Mnemonic.pmulth, R3, R1, R2),
                Instr(Mnemonic.pdivbw, R1, R2),
                Instr(Mnemonic.pexew, R3, R2),
                Instr(Mnemonic.prot3w, R3, R2));

            var mmi3 = Sparse(6, 5, "  PS2 - MMI3",
                invalid,
                (0b000_00, Instr(Mnemonic.pmadduw, R3, R1, R2)),
                (0b000_11, Instr(Mnemonic.psravw, R3, R1, R2)),
                (0b010_00, Instr(Mnemonic.pmthi, R1)),
                (0b010_01, Instr(Mnemonic.pmtlo, R1)),
                (0b010_10, Instr(Mnemonic.pinteh, R3, R1, R2)),
                (0b011_00, Instr(Mnemonic.pmultuw, R3, R1, R2)),
                (0b011_01, Instr(Mnemonic.pdivuw, R1, R2)),
                (0b011_10, Instr(Mnemonic.pcpyud, R3, R1, R2)),
                (0b100_10, Instr(Mnemonic.por, R3, R1, R2)),
                (0b100_11, Instr(Mnemonic.pnor, R3, R1, R2)),
                (0b110_10, Instr(Mnemonic.pexch, R3, R2)),
                (0b110_11, Instr(Mnemonic.pcpyh, R3, R2)),
                (0b111_10, Instr(Mnemonic.pexcw, R3, R2)));

            var mmi = Sparse(0, 6, "  PS2 - MMI",
                invalid,
                (0b000_000, Instr(Mnemonic.madd, R3, R1, R2)),
                (0b000_001, Instr(Mnemonic.maddu, R3, R1, R2)),
                (0b000_100, Instr(Mnemonic.plzcw, R3, R1)),
                (0b001_000, mmi0),
                (0b001_001, mmi2),
                (0b010_000, Instr(Mnemonic.mfhi1, R3)),
                (0b010_001, Instr(Mnemonic.mthi1, R3)),
                (0b010_010, Instr(Mnemonic.mflo1, R3)),
                (0b010_011, Instr(Mnemonic.mtlo1, R3)),
                (0b011_000, Instr(Mnemonic.mult1, R3, R1, R2)),
                (0b011_001, Instr(Mnemonic.multu1, R3, R1, R2)),
                (0b011_010, Instr(Mnemonic.div1, R1, R2)),
                (0b011_011, Instr(Mnemonic.divu1, R1, R2)),
                (0b100_000, Instr(Mnemonic.madd1, R3, R1, R2)),
                (0b100_001, Instr(Mnemonic.maddu1, R3, R1, R2)),
                (0b101_000, mmi1),
                (0b101_001, mmi3),
                (0b110_000, Sparse(6, 5, "  pmfhl",
                    invalid,
                    (0b00000, Instr(Mnemonic.pmfhl_lw, R3)),
                    (0b00001, Instr(Mnemonic.pmfhl_uw, R3)),
                    (0b00010, Instr(Mnemonic.pmfhl_slw, R3)),
                    (0b00011, Instr(Mnemonic.pmfhl_lh, R3)),
                    (0b00100, Instr(Mnemonic.pmfhl_sh, R3)))),
                (0b110_001, Instr(Mnemonic.pmthl_lw, R1)),
                (0b110_100, Instr(Mnemonic.psllh, R3, R2, s)),
                (0b110_110, Instr(Mnemonic.psrlh, R3, R2, s)),
                (0b110_111, Instr(Mnemonic.psrah, R3, R2, s)),
                (0b111_100, Instr(Mnemonic.psllw, R3, R2, s)),
                (0b111_110, Instr(Mnemonic.psrlw, R3, R2, s)),
                (0b111_111, Instr(Mnemonic.psraw, R3, R2, s)));
            return mmi;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1E()
        {
            return Instr(Mnemonic.lq, R2, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1F()
        {
            return Instr(Mnemonic.sq, R2, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop0()
        {
            var c0 = Sparse(0, 6, "  PS2 - C0",
                invalid,
                (0b000_001, Instr(Mnemonic.tlbr)),
                (0b000_010, Instr(Mnemonic.tlbwi)),
                (0b000_110, Instr(Mnemonic.tlbwr)),
                (0b001_000, Instr(Mnemonic.tlbp)),
                (0b011_000, Instr(InstrClass.Return, Mnemonic.eret)),
                (0b111_000, Instr(Mnemonic.ei)),
                (0b111_001, Instr(Mnemonic.di)));
            var cop0 = Sparse(21, 5, "  PS2 - COP0",
                invalid,
                (0b00_000, Instr(Mnemonic.mfc0, R2, CPR0_3)),
                (0b00_100, Instr(Mnemonic.mtc0, R2, CPR0_3)),
                (0b01_000, Nyi("bc0")),
                (0b10_000, c0));
            return cop0;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1_S()
        {
            return Sparse(0, 6, "  S class",
                invalid,
                (0b000_000, Instr(Mnemonic.add_s, F4, F3, F2)),
                (0b000_001, Instr(Mnemonic.sub_s, F4, F3, F2)),
                (0b000_010, Instr(Mnemonic.mul_s, F4, F3, F2)),
                (0b000_011, Instr(Mnemonic.div_s, F4, F3, F2)),
                (0b000_100, Instr(Mnemonic.sqrt_s, F4, F3)),
                (0b000_101, Instr(Mnemonic.abs_s, F4, F3)),
                (0b000_110, Instr(Mnemonic.mov_s, F4, F3)),
                (0b000_111, Instr(Mnemonic.neg_s, F4, F3)),

                (0b010_110, Instr(Mnemonic.rsqrt_s, F4, F3, F2)),

                (0b011_000, Instr(Mnemonic.adda_s, F3, F2)),
                (0b011_001, Instr(Mnemonic.suba_s, F3, F2)),
                (0b011_010, Instr(Mnemonic.mula_s, F3, F2)),
                (0b011_100, Instr(Mnemonic.madd_s, F4, F3, F2)),
                (0b011_101, Instr(Mnemonic.msub_s, F4, F3, F2)),
                (0b011_110, Instr(Mnemonic.madda_s, F3, F2)),
                (0b011_111, Instr(Mnemonic.msuba_s, F3, F2)),

                (0b100_100, Instr(Mnemonic.cvt_w_s, F4, F3)),

                (0b101_000, Instr(Mnemonic.max_s, F4, F3, F2)),
                (0b101_001, Instr(Mnemonic.min_s, F4, F3, F2)),

                (0b110_000, Instr(Mnemonic.c_f_s, F3, F2)),
                (0b110_010, Instr(Mnemonic.c_eq_s, F3, F2)),
                (0b110_100, Instr(Mnemonic.c_lt_s, F3, F2)),
                (0b110_110, Instr(Mnemonic.c_le_s, F3, F2)));
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeWclass()
        {
            return Sparse(0, 6, " PS2 - WClass",
                invalid,
                (0b100_000, Instr(Mnemonic.cvt_s_w, F4, F3)));
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop2()
        {
            var bc2 = Sparse(16, 5, "  PS2 - BC2",
                invalid,
                (0b00_000, Instr(InstrClass.JCD, Mnemonic.bc2f, j)),
                (0b00_001, Instr(InstrClass.JCD, Mnemonic.bc2t, j)),
                (0b00_010, Instr(Mnemonic.bc2fl, R2)),
                (0b00_011, Instr(Mnemonic.bc2tl, R2)));

            var special2 = Mask(6, 5, 0, 2, "  PS2 - Cop2 - Special2",
                Instr(Mnemonic.vaddax, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vadday, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddaz, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddaw, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubax, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubay, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubaz, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubaw, AccR, Vf11, Vf16, mask),

                Instr(Mnemonic.vmaddax, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmadday, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddaz, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddaw, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubax, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubay, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubaz, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubaw, AccR, Vf11, Vf16, mask),

                Instr(Mnemonic.vitof0, Vf16, Vf11, mask),
                Instr(Mnemonic.vitof4, Vf16, Vf11, mask),
                Instr(Mnemonic.vitof12, Vf16, Vf11, mask),
                Instr(Mnemonic.vitof15, Vf16, Vf11, mask),
                Instr(Mnemonic.vftoi0, Vf16, Vf11, mask),
                Instr(Mnemonic.vftoi4, Vf16, Vf11, mask),
                Instr(Mnemonic.vftoi12, Vf16, Vf11, mask),
                Instr(Mnemonic.vftoi15, Vf16, Vf11, mask),

                Instr(Mnemonic.vmulax, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulay, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulaz, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulaw, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulaq, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vabs, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulai, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vclipw, Vf11, Vf16, mask),

                Instr(Mnemonic.vaddaq, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddaq, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddai, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddai, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubaq, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubaq, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubai, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubai, AccR, Vf11, Vf16, mask),

                Instr(Mnemonic.vadda, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmadda, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmula, AccR, Vf11, Vf16, mask),
                invalid,
                Instr(Mnemonic.vsuba, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsuba, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vopmula, AccR, Vf11, Vf16, mask),
                Instr(Mnemonic.vnop),

                Instr(Mnemonic.vmove, Vf16, Vf11, mask),
                Instr(Mnemonic.vmr32, Vf16, Vf11, mask),
                invalid,
                invalid,
                Instr(Mnemonic.vlqi, Vf16, Vi11, mask),
                Instr(Mnemonic.vsqi, Vf11, Vi16, mask),
                Instr(Mnemonic.vlqd, Vf16, Vi11, mask),
                Instr(Mnemonic.vsqd, Vf11, Vi16, mask),

                Instr(Mnemonic.vdiv, fsf, ftf, Q, Vf11, Vf16),
                Instr(Mnemonic.vsqrt, Q, Vf16, ftf),
                Instr(Mnemonic.vrsqrt, Q, Vf11, Vf16, ftf, fsf),
                Instr(Mnemonic.vwaitq),
                Instr(Mnemonic.vmtir, Vi16, Vf11, fsf),
                Instr(Mnemonic.vmfir, Vf16, Vi11, mask),
                Instr(Mnemonic.vilwr, Vi16, Vi11, mask),
                Instr(Mnemonic.viswr, Vi16, Vi11, mask),

                Instr(Mnemonic.vrnext, Vf11, Vf16, mask),
                Instr(Mnemonic.vrget, Vf11, Vf16, mask),
                Instr(Mnemonic.vrinit, Vf11, Vf16, mask),
                Instr(Mnemonic.vrxor, R, Vf11, fsf),
                invalid,
                invalid,
                invalid,
                invalid,

                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid,
                invalid, invalid, invalid, invalid, invalid, invalid, invalid, invalid);

            var special1 = Mask(0, 6, "  PS2 - COP2 - Special",

                Instr(Mnemonic.vaddx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddy, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vaddw, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vsuby, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vsubw, Vf6, Vf11, Vf16, mask),

                Instr(Mnemonic.vmaddx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddy, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaddw, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsuby, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsubw, Vf6, Vf11, Vf16, mask),

                Instr(Mnemonic.vmaxx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaxy, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaxz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmaxw, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vminix, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vminiy, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vminiz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vminiw, Vf6, Vf11, Vf16, mask),

                Instr(Mnemonic.vmulx, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmuly, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulz, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulw, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmulq, Vf6, Vf11, Q, mask),
                Instr(Mnemonic.vmaxi, Vf6, Vf11, I, mask),
                Instr(Mnemonic.vmuli, Vf6, Vf11, I, mask),
                Instr(Mnemonic.vminii, Vf6, Vf11, I, mask),

                Instr(Mnemonic.vaddq, Vf6, Vf11, Q, mask),
                Instr(Mnemonic.vmaddq, Vf6, Vf11, Q, mask),
                Instr(Mnemonic.vaddi, Vf6, Vf11, I, mask),
                Instr(Mnemonic.vmaddi, Vf6, Vf11, I, mask),
                Instr(Mnemonic.vsubq, Vf6, Vf11, Q, mask),
                Instr(Mnemonic.vmsubq, Vf6, Vf11, Q, mask),
                Instr(Mnemonic.vsubi, Vf6, Vf11, I, mask),
                Instr(Mnemonic.vmsubi, Vf6, Vf11, I, mask),

                Instr(Mnemonic.vadd, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmadd, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmul, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmax, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vsub, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmsub, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vopmsub, Vf6, Vf11, Vf16, mask),
                Instr(Mnemonic.vmini, Vf6, Vf11, Vf16, mask),

                Instr(Mnemonic.viadd, Vi6, Vi11, Vi16),
                Instr(Mnemonic.visub, Vi6, Vi11, Vi16),
                Instr(Mnemonic.viaddi, Vi16, Vi11, SImm6_5),
                invalid,
                Instr(Mnemonic.viand, Vi6, Vi11, Vi16),
                Instr(Mnemonic.vior, Vi6, Vi11, Vi16),
                invalid,
                invalid,

                Instr(Mnemonic.vcallms, UImm_Shift(6, 15, 3)),
                Instr(Mnemonic.vcallmsr),
                invalid,
                invalid,
                special2,
                special2,
                special2,
                special2);

            var cop2 = Sparse(21, 5, "  PS2 - COP2",
                invalid,

                (0b00001, Instr(Mnemonic.qmfc2, R2, Vf11)),
                (0b00010, Instr(Mnemonic.cfc2, R2, Vi11)),
                (0b00101, Instr(Mnemonic.qmtc2, R2, Vf11)),
                (0b00110, Instr(Mnemonic.ctc2, R2, Vi11)),
                (0b01000, bc2),

                (0b10000, special1),
                (0b10001, special1),
                (0b10010, special1),
                (0b10011, special1),
                (0b10100, special1),
                (0b10101, special1),
                (0b10110, special1),
                (0b10111, special1),
                (0b11000, special1),
                (0b11001, special1),
                (0b11010, special1),
                (0b11011, special1),
                (0b11100, special1),
                (0b11101, special1),
                (0b11110, special1),
                (0b11111, special1));
            return cop2;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode36()
        {
            return Instr(Mnemonic.lqc2, Vf16, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3E()
        {
            return Instr(Mnemonic.sqc2, Vf16, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1_8()
        {
            return Sparse(16, 5, "  PS2 - COP1_8",
                invalid,
                (0b00000, Instr(DCT, Mnemonic.bc1f, j)),
                (0b00001, Instr(DCT, Mnemonic.bc1t, j)),
                (0b00010, Instr(DCT, Mnemonic.bc1fl, j)),
                (0b00011, Instr(DCT, Mnemonic.bc1tl, j)));
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm18()
        {
            return Instr(Mnemonic.mtsab, R1, SImm);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm19()
        {
            return Instr(Mnemonic.mtsah, R1, SImm);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial28()
        {
            return Instr(Mnemonic.mfsa, R3);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial29()
        {
            return Instr(Mnemonic.mtsa, R1);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2C()
        {
            return Instr(Mnemonic.dadd, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2D()
        {
            return Instr(Mnemonic.daddu, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2E()
        {
            return Instr(Mnemonic.dsub, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2F()
        {
            return Instr(Mnemonic.dsubu, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Special3_Decode36()
        {
            return Instr(Mnemonic.ldc2, Vf16, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3C()
        {
            return invalid;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Instr64(Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
        {
            return Instr(mnemonic, mutators);
        }
    }
}
