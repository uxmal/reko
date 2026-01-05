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
using Reko.Core.Lib;
using Reko.Core.Machine;

// Converted by hand from https://www.inwap.com/pdp10/opcodes.html

// Instruction set differences http://pdp10.nocrew.org/cpu/processors.html

namespace Reko.Arch.Pdp.Pdp10.Disassembler
{
    using Decoder = WideDecoder<Pdp10Disassembler, Mnemonic, Pdp10Instruction>;
    using InstrDecoder = WideInstrDecoder<Pdp10Disassembler, Mnemonic, Pdp10Instruction>;

    public partial class Pdp10Disassembler
    {
        public class InstructionSet
        {
            private static Decoder Mask(Bitfield bitfield, string tag, params Decoder[] decoders)
            {
                return new WideMaskDecoder<Pdp10Disassembler, Mnemonic, Pdp10Instruction>(bitfield, tag, decoders);
            }

            private static Decoder Instr(Mnemonic mnemonic, params WideMutator<Pdp10Disassembler>[] mutators)
            {
                return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
            }

            private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params WideMutator<Pdp10Disassembler>[] mutators)
            {
                return new InstrDecoder(iclass, mnemonic, mutators);
            }

            private static Decoder Nyi(Mnemonic mnemonic)
            {
                return new WideNyiDecoder<Pdp10Disassembler, Mnemonic, Pdp10Instruction>(mnemonic.ToString());
            }

            public static Decoder Create()
            {
                var invalid = new InstrDecoder(InstrClass.Invalid, Mnemonic.Invalid);

                var jfcl = Instr(Mnemonic.jfcl, ImmAc, J);
                var jfclVariants = Mask(acField, "  jfcl",
                    Instr(Mnemonic.jfcl, InstrClass.Linear | InstrClass.Padding),
                    Instr(Mnemonic.jfov, InstrClass.ConditionalTransfer, J),
                    Instr(Mnemonic.jcry1, InstrClass.ConditionalTransfer, J),
                    jfcl,

                    Instr(Mnemonic.jcry0, InstrClass.ConditionalTransfer, J),
                    jfcl,
                    Instr(Mnemonic.jcry, InstrClass.ConditionalTransfer, J),
                    jfcl,

                    Instr(Mnemonic.jov, InstrClass.ConditionalTransfer, J),
                    jfcl,
                    jfcl,
                    jfcl,

                    jfcl,
                    jfcl,
                    jfcl,
                    jfcl);

                // KL10-KS10
                var jrstVariants = Mask(acField, "  jrst",
                    Instr(Mnemonic.jrst, InstrClass.Transfer, J),
                    Instr(Mnemonic.portal),
                    Instr(Mnemonic.jrstf, InstrClass.Transfer, J),
                    invalid,

                    Instr(Mnemonic.halt, InstrClass.Terminates, J),
                    Instr(Mnemonic.xjrstf, InstrClass.Transfer, J),
                    Instr(Mnemonic.xjen, InstrClass.Transfer, J),
                    Instr(Mnemonic.xpcw, InstrClass.Transfer, J),

                    Nyi(Mnemonic.jrst),
                    invalid,
                    Instr(Mnemonic.jen, InstrClass.Transfer, J),
                    invalid,

                    Instr(Mnemonic.sfm, InstrClass.Transfer, J),
                    invalid,
                    invalid,
                    invalid);

                var opcodes0 = Mask(bf(3, 6), "  Monitor UUOs and Local UUOs",

                    invalid,
                    Instr(Mnemonic.luuo01, AC, E),
                    Instr(Mnemonic.luuo02, AC, E),
                    Instr(Mnemonic.luuo03, AC, E),

                    Instr(Mnemonic.luuo04, AC, E),
                    Instr(Mnemonic.luuo05, AC, E),
                    Instr(Mnemonic.luuo06, AC, E),
                    Instr(Mnemonic.luuo07, AC, E),
                    // 0o10
                    Instr(Mnemonic.luuo10, AC, E),
                    Instr(Mnemonic.luuo11, AC, E),
                    Instr(Mnemonic.luuo12, AC, E),
                    Instr(Mnemonic.luuo13, AC, E),

                    Instr(Mnemonic.luuo14, AC, E),
                    Instr(Mnemonic.luuo15, AC, E),
                    Instr(Mnemonic.luuo16, AC, E),
                    Instr(Mnemonic.luuo17, AC, E),
                    // 0o20
                    Instr(Mnemonic.luuo20, AC, E),
                    Instr(Mnemonic.luuo21, AC, E),
                    Instr(Mnemonic.luuo22, AC, E),
                    Instr(Mnemonic.luuo23, AC, E),

                    Instr(Mnemonic.luuo24, AC, E),
                    Instr(Mnemonic.luuo25, AC, E),
                    Instr(Mnemonic.luuo26, AC, E),
                    Instr(Mnemonic.luuo27, AC, E),
                    // 0o30
                    Instr(Mnemonic.luuo30, AC, E),
                    Instr(Mnemonic.luuo31, AC, E),
                    Instr(Mnemonic.luuo32, AC, E),
                    Instr(Mnemonic.luuo33, AC, E),

                    Instr(Mnemonic.luuo34, AC, E),
                    Instr(Mnemonic.luuo35, AC, E),
                    Instr(Mnemonic.luuo36, AC, E),
                    Instr(Mnemonic.luuo37, AC, E),
                    // 0o40
                    Instr(Mnemonic.call, AC, E),
                    Instr(Mnemonic.initi, AC, E),
                    Instr(Mnemonic.muuo42, AC, E),
                    Instr(Mnemonic.muuo43, AC, E),
                    
                    Instr(Mnemonic.muuo44, AC, E),
                    Instr(Mnemonic.muuo45, AC, E),
                    Instr(Mnemonic.muuo46, AC, E),
                    Instr(Mnemonic.calli, AC, E),
                    // 0o50
                    Instr(Mnemonic.open, AC, E),
                    Instr(Mnemonic.ttcall, AC, E),
                    Instr(Mnemonic.muuo52, AC, E),
                    Instr(Mnemonic.muuo53, AC, E),
                    
                    Instr(Mnemonic.muuo54, AC, E),
                    Instr(Mnemonic.rename, AC, E),
                    Instr(Mnemonic.@in, AC, E),
                    Instr(Mnemonic.@out, AC, E),
                    // 0o60
                    Instr(Mnemonic.setsts, AC, E),
                    Instr(Mnemonic.stato, AC, E),
                    Instr(Mnemonic.status, AC, E),
                    Instr(Mnemonic.getsts, AC, E),

                    Instr(Mnemonic.inbuf, AC, E),
                    Instr(Mnemonic.outbuf, AC, E),
                    Instr(Mnemonic.input, AC, E),
                    Instr(Mnemonic.output, AC, E),
                    // 0o70
                    Instr(Mnemonic.close, AC, E),
                    Instr(Mnemonic.releas, AC, E),
                    Instr(Mnemonic.mtape, AC, E),
                    Instr(Mnemonic.ugetf, AC, E),

                    Instr(Mnemonic.useti, AC, E),
                    Instr(Mnemonic.useto, AC, E),
                    Instr(Mnemonic.lookup, AC, E),
                    Instr(Mnemonic.enter, AC, E));

                var opcodes1 = Mask(bf(3,6), "  Floating point, byte manipulation, other",
                    Instr(Mnemonic.ujen, AC, E),
                    invalid,   // 101,
                    Instr(Mnemonic.gfad, AC, E),
                    Instr(Mnemonic.gfsb, AC, E),
                    Instr(Mnemonic.jsys, AC, E),
                    Instr(Mnemonic.adjsp, AC, E),
                    Instr(Mnemonic.gfmp, AC, E),
                    Instr(Mnemonic.gfdv, AC, E),

                    Instr(Mnemonic.dfad, AC, E),
                    Instr(Mnemonic.dfsb, AC, E),
                    Instr(Mnemonic.dfmp, AC, E),
                    Instr(Mnemonic.dfdv, AC, E),
                    Instr(Mnemonic.dadd, AC, E),
                    Instr(Mnemonic.dsub, AC, E),
                    Instr(Mnemonic.dmul, AC, E),
                    Instr(Mnemonic.ddiv, AC, E),

                    Instr(Mnemonic.dmove, AC,E),
                    Instr(Mnemonic.dmovn, AC, E),
                    Instr(Mnemonic.fix, AC, E),
                    Instr(Mnemonic.extend, AC, E),
                    Instr(Mnemonic.dmovem, AC, E),
                    Instr(Mnemonic.dmovnm, AC, E),
                    Instr(Mnemonic.fixr, AC, E),
                    Instr(Mnemonic.fltr, AC, E),

                    Instr(Mnemonic.ufa, AC, E),
                    Instr(Mnemonic.dfn, AC, E),
                    Instr(Mnemonic.fsc, AC, E),
                    Instr(Mnemonic.ibp, AC, E),
                    Instr(Mnemonic.ildb, AC, E),
                    Instr(Mnemonic.ldb, AC, E),
                    Instr(Mnemonic.idpb, AC, E),
                    Instr(Mnemonic.dpb, AC, E),

                    Instr(Mnemonic.fad, AC, E),
                    Instr(Mnemonic.fadl, AC, E),
                    Instr(Mnemonic.fadm, AC, E),
                    Instr(Mnemonic.fadb, AC, E),
                    Instr(Mnemonic.fadr, AC, E),
                    Instr(Mnemonic.fadrl, AC, E),
                    Instr(Mnemonic.fadrm, AC, E),
                    Instr(Mnemonic.fadrb, AC, E),

                    Instr(Mnemonic.fsb, AC, E),
                    Instr(Mnemonic.fsbl, AC, E),
                    Instr(Mnemonic.fsbm, AC, E),
                    Instr(Mnemonic.fsbb, AC, E),
                    Instr(Mnemonic.fsbr, AC, E),
                    Instr(Mnemonic.fsbrl, AC, E),
                    Instr(Mnemonic.fsbrm, AC, E),
                    Instr(Mnemonic.fsbrb, AC, E),

                    Instr(Mnemonic.fmp, AC, E),
                    Instr(Mnemonic.fmpl, AC, E),
                    Instr(Mnemonic.fmpm, AC, E),
                    Instr(Mnemonic.fmpb, AC, E),
                    Instr(Mnemonic.fmpr, AC, E),
                    Instr(Mnemonic.fmprl, AC, E),
                    Instr(Mnemonic.fmprm, AC, E),
                    Instr(Mnemonic.fmprb, AC, E),

                    Instr(Mnemonic.fdv, AC, E),
                    Instr(Mnemonic.fdvl, AC, E),
                    Instr(Mnemonic.fdvm, AC, E),
                    Instr(Mnemonic.fdvb, AC, E),
                    Instr(Mnemonic.fdvr, AC, E),
                    Instr(Mnemonic.fdvrl, AC, E),
                    Instr(Mnemonic.fdvrm, AC, E),
                    Instr(Mnemonic.fdvrb, AC, E));

                var opcodes2 = Mask(bf(3, 6), "  Integer arithmetic, jump to subroutine",
                    Instr(Mnemonic.move, AC, E),
                    Instr(Mnemonic.movei, AC, E),
                    Instr(Mnemonic.movem, AC, E),
                    Instr(Mnemonic.moves, AC, E),
                    Instr(Mnemonic.movs, AC, E),
                    Instr(Mnemonic.movsi, AC, E),
                    Instr(Mnemonic.movsm, AC, E),
                    Instr(Mnemonic.movss, AC, E),

                    Instr(Mnemonic.movn, AC, E),
                    Instr(Mnemonic.movni, AC, E),
                    Instr(Mnemonic.movnm, AC, E),
                    Instr(Mnemonic.movns, AC, E),
                    Instr(Mnemonic.movm, AC, E),
                    Instr(Mnemonic.movmi, AC, E),
                    Instr(Mnemonic.movmm, AC, E),
                    Instr(Mnemonic.movms, AC, E),

                    Instr(Mnemonic.imul, AC, E),
                    Instr(Mnemonic.imuli, AC, E),
                    Instr(Mnemonic.imulm, AC, E),
                    Instr(Mnemonic.imulb, AC, E),
                    Instr(Mnemonic.mul, AC, E),
                    Instr(Mnemonic.muli, AC, E),
                    Instr(Mnemonic.mulm, AC, E),
                    Instr(Mnemonic.mulb, AC, E),

                    Instr(Mnemonic.idiv, AC, E),
                    Instr(Mnemonic.idivi, AC, E),
                    Instr(Mnemonic.idivm, AC, E),
                    Instr(Mnemonic.idivb, AC, E),
                    Instr(Mnemonic.div, AC, E),
                    Instr(Mnemonic.divi, AC, E),
                    Instr(Mnemonic.divm, AC, E),
                    Instr(Mnemonic.divb, AC, E),

                    Instr(Mnemonic.ash, AC, E),
                    Instr(Mnemonic.rot, AC, E),
                    Instr(Mnemonic.lsh, AC, E),
                    Instr(Mnemonic.jffo, AC, E),
                    Instr(Mnemonic.ashc, AC, E),
                    Instr(Mnemonic.rotc, AC, E),
                    Instr(Mnemonic.lshc, AC, E),
                    invalid,   // 247),

                    Instr(Mnemonic.exch, AC, E),
                    Instr(Mnemonic.blt, AC, E),
                    Instr(Mnemonic.aobjp, AC, E),
                    Instr(Mnemonic.aobjn, AC, E),
                    jrstVariants,
                    jfclVariants,
                    Instr(Mnemonic.xct, AC, E),
                    Instr(Mnemonic.map, AC, E),

                    Instr(Mnemonic.pushj, InstrClass.Transfer|InstrClass.Call, AC,J),
                    Instr(Mnemonic.push, AC,E),
                    Instr(Mnemonic.pop, AC,E),
                    Instr(Mnemonic.popj, InstrClass.Transfer|InstrClass.Return, AC,E),
                    Instr(Mnemonic.jsr, InstrClass.Transfer, J),
                    Instr(Mnemonic.jsp, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.jsa, AC,E),
                    Instr(Mnemonic.jra, AC,J),

                    Instr(Mnemonic.addi, AC, E),
                    Instr(Mnemonic.addi, AC, E),
                    Instr(Mnemonic.addm, AC, E),
                    Instr(Mnemonic.addb, AC, E),
                    Instr(Mnemonic.subi, AC, E),
                    Instr(Mnemonic.subi, AC, E),
                    Instr(Mnemonic.subm, AC, E),
                    Instr(Mnemonic.subb, AC, E));

                var opcodes3 = Mask(bf(3, 6), "  Hop, skip, and jump", // codes 3_0 do not skip or jump
                    Instr(Mnemonic.cai, InstrClass.Linear | InstrClass.Padding, ACnot0, E),
                    Instr(Mnemonic.cail, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.caie, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.caile, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.caia, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.caige, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.cain, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.caig, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.cam, InstrClass.Linear|InstrClass.Padding, ACnot0, E),
                    Instr(Mnemonic.caml, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.came, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.camle, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.cama, InstrClass.Transfer, ACnot0, E),
                    Instr(Mnemonic.camge, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.camn, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.camg, InstrClass.ConditionalTransfer, ACnot0, E),

                    Instr(Mnemonic.jump, AC, J),
                    Instr(Mnemonic.jumpl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpe, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumple, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpa, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.jumpge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.skip, ACnot0, E),
                    Instr(Mnemonic.skipl, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.skipe, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.skiple, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.skipa, InstrClass.Transfer, ACnot0, E),
                    Instr(Mnemonic.skipge, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.skipn, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.skipg, InstrClass.ConditionalTransfer, ACnot0, E),

                    Instr(Mnemonic.aoj, AC, J),
                    Instr(Mnemonic.aojl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aoje, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojle, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aoja, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.aojge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.aos, ACnot0, E),
                    Instr(Mnemonic.aosl, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.aose, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.aosle, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.aosa, InstrClass.Transfer, ACnot0, E),
                    Instr(Mnemonic.aosge, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.aosn, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.aosg, InstrClass.ConditionalTransfer, ACnot0, E),

                    Instr(Mnemonic.soj, AC, J),
                    Instr(Mnemonic.sojl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.soje, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojle, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.soja, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.sojge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.sos, ACnot0, E),
                    Instr(Mnemonic.sosl, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.sose, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.sosle, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.sosa, InstrClass.Transfer, ACnot0, E),
                    Instr(Mnemonic.sosge, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.sosn, InstrClass.ConditionalTransfer, ACnot0, E),
                    Instr(Mnemonic.sosg, InstrClass.ConditionalTransfer, ACnot0, E));

                var opcodes4 = Mask(bf(3, 6), "  two-argument logical operations",
                    Instr(Mnemonic.setz, AC),
                    Instr(Mnemonic.setzi, AC, E),
                    Instr(Mnemonic.setzm, E),
                    Instr(Mnemonic.setzb, AC, E),
                    Instr(Mnemonic.and, AC, E),
                    Instr(Mnemonic.andi, AC, E),
                    Instr(Mnemonic.andm, AC, E),
                    Instr(Mnemonic.andb, AC, E),

                    Instr(Mnemonic.andca, AC, E),
                    Instr(Mnemonic.andcai, AC, E),
                    Instr(Mnemonic.andcam, AC, E),
                    Instr(Mnemonic.andcab, AC, E),
                    Instr(Mnemonic.setm, AC, E),
                    Instr(Mnemonic.xmovei, AC, E),
                    Instr(Mnemonic.setmm, AC, E),
                    Instr(Mnemonic.setmb, AC, E),

                    Instr(Mnemonic.andcm, AC, E),
                    Instr(Mnemonic.andcmi, AC, E),
                    Instr(Mnemonic.andcmm, AC, E),
                    Instr(Mnemonic.andcmb, AC, E),
                    Instr(Mnemonic.seta, AC, E),
                    Instr(Mnemonic.setai, AC, E),
                    Instr(Mnemonic.setam, AC, E),
                    Instr(Mnemonic.setab, AC, E),

                    Instr(Mnemonic.xor, AC, E),
                    Instr(Mnemonic.xori, AC, E),
                    Instr(Mnemonic.xorm, AC, E),
                    Instr(Mnemonic.xorb, AC, E),
                    Instr(Mnemonic.or, AC, E),
                    Instr(Mnemonic.ori, AC, E),
                    Instr(Mnemonic.orm, AC, E),
                    Instr(Mnemonic.orb, AC, E),

                    Instr(Mnemonic.andcb, AC, E),
                    Instr(Mnemonic.andcbi, AC, E),
                    Instr(Mnemonic.andcbm, AC, E),
                    Instr(Mnemonic.andcbb, AC, E),
                    Instr(Mnemonic.eqv, AC, E),
                    Instr(Mnemonic.eqvi, AC, E),
                    Instr(Mnemonic.eqvm, AC, E),
                    Instr(Mnemonic.eqvb, AC, E),

                    Instr(Mnemonic.setca, AC, E),
                    Instr(Mnemonic.setcai, AC, E),
                    Instr(Mnemonic.setcam, AC, E),
                    Instr(Mnemonic.setcab, AC, E),
                    Instr(Mnemonic.orca, AC, E),
                    Instr(Mnemonic.orcai, AC, E),
                    Instr(Mnemonic.orcam, AC, E),
                    Instr(Mnemonic.orcab, AC, E),

                    Instr(Mnemonic.setcm, AC, E),
                    Instr(Mnemonic.setcmi, AC, E),
                    Instr(Mnemonic.setcmm, AC, E),
                    Instr(Mnemonic.setcmb, AC, E),
                    Instr(Mnemonic.orcm, AC, E),
                    Instr(Mnemonic.orcmi, AC, E),
                    Instr(Mnemonic.orcmm, AC, E),
                    Instr(Mnemonic.orcmb, AC, E),

                    Instr(Mnemonic.orcb, AC, E),
                    Instr(Mnemonic.orcbi, AC, E),
                    Instr(Mnemonic.orcbm, AC, E),
                    Instr(Mnemonic.orcbb, AC, E),
                    Instr(Mnemonic.seto, AC, E),
                    Instr(Mnemonic.setoi, AC, E),
                    Instr(Mnemonic.setom, AC, E),
                    Instr(Mnemonic.setob, AC, E));

                // Half word {left,right} to {left,right} with {nochange,zero,ones,extend},
                //{ac,immediate,memory,self}
                var opcodes5 = Mask(bf(3, 6), "  Half words",
                    Instr(Mnemonic.hll, AC, E),
                    Instr(Mnemonic.xhlli, AC, E),
                    Instr(Mnemonic.hllm, AC, E),
                    Instr(Mnemonic.hlls, AC, E),
                    Instr(Mnemonic.hrl, AC, E),
                    Instr(Mnemonic.hrli, AC, E),
                    Instr(Mnemonic.hrlm, AC, E),
                    Instr(Mnemonic.hrls, AC, E),

                    Instr(Mnemonic.hllz, AC, E),
                    Instr(Mnemonic.hllzi, AC, E),
                    Instr(Mnemonic.hllzm, AC, E),
                    Instr(Mnemonic.hllzs, AC, E),
                    Instr(Mnemonic.hrlz, AC, E),
                    Instr(Mnemonic.hrlzi, AC, E),
                    Instr(Mnemonic.hrlzm, AC, E),
                    Instr(Mnemonic.hrlzs, AC, E),

                    Instr(Mnemonic.hllo, AC, E),
                    Instr(Mnemonic.hlloi, AC, E),
                    Instr(Mnemonic.hllom, AC, E),
                    Instr(Mnemonic.hllos, AC, E),
                    Instr(Mnemonic.hrlo, AC, E),
                    Instr(Mnemonic.hrloi, AC, E),
                    Instr(Mnemonic.hrlom, AC, E),
                    Instr(Mnemonic.hrlos, AC, E),

                    Instr(Mnemonic.hlle, AC, E),
                    Instr(Mnemonic.hllei, AC, E),
                    Instr(Mnemonic.hllem, AC, E),
                    Instr(Mnemonic.hlles, AC, E),
                    Instr(Mnemonic.hrle, AC, E),
                    Instr(Mnemonic.hrlei, AC, E),
                    Instr(Mnemonic.hrlem, AC, E),
                    Instr(Mnemonic.hrles, AC, E),

                    Instr(Mnemonic.hrr, AC, E),
                    Instr(Mnemonic.hrri, AC, E),
                    Instr(Mnemonic.hrrm, AC, E),
                    Instr(Mnemonic.hrrs, AC, E),
                    Instr(Mnemonic.hlr, AC, E),
                    Instr(Mnemonic.hlri, AC, E),
                    Instr(Mnemonic.hlrm, AC, E),
                    Instr(Mnemonic.hlrs, AC, E),

                    Instr(Mnemonic.hrrz, AC, E),
                    Instr(Mnemonic.hrrzi, AC, E),
                    Instr(Mnemonic.hrrzm, AC, E),
                    Instr(Mnemonic.hrrzs, AC, E),
                    Instr(Mnemonic.hlrz, AC, E),
                    Instr(Mnemonic.hlrzi, AC, E),
                    Instr(Mnemonic.hlrzm, AC, E),
                    Instr(Mnemonic.hlrzs, AC, E),

                    Instr(Mnemonic.hrro, AC, E),
                    Instr(Mnemonic.hrroi, AC, E),
                    Instr(Mnemonic.hrrom, AC, E),
                    Instr(Mnemonic.hrros, AC, E),
                    Instr(Mnemonic.hlro, AC, E),
                    Instr(Mnemonic.hlroi, AC, E),
                    Instr(Mnemonic.hlrom, AC, E),
                    Instr(Mnemonic.hlros, AC, E),

                    Instr(Mnemonic.hrre, AC, E),
                    Instr(Mnemonic.hrrei, AC, E),
                    Instr(Mnemonic.hrrem, AC, E),
                    Instr(Mnemonic.hrres, AC, E),
                    Instr(Mnemonic.hlre, AC, E),
                    Instr(Mnemonic.hlrei, AC, E),
                    Instr(Mnemonic.hlrem, AC, E),
                    Instr(Mnemonic.hlres, AC, E));

                // Test bits, {right,left,direct,swapped} with {nochange,zero,complement,one}
                // and skip if the masked bits were {noskip,equal,nonzero,always}
                var opcodes6 = Mask(bf(3, 6), "  Test bits",
                    Instr(Mnemonic.trn, AC, E),
                    Instr(Mnemonic.tln, AC, E),
                    Instr(Mnemonic.trne, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlne, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.trna, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tlna, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.trnn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlnn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.tdn, AC, E),
                    Instr(Mnemonic.tsn, AC, E),
                    Instr(Mnemonic.tdne, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tsne, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdna, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tsna, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdnn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tsnn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.trz, AC, E),
                    Instr(Mnemonic.tlz, AC, E),
                    Instr(Mnemonic.trze, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlze, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.trza, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tlza, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.trzn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlzn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.tdz, AC, E),
                    Instr(Mnemonic.tsz, AC, E),
                    Instr(Mnemonic.tdze, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tsze, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdza, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tsza, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdzn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tszn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.trc, AC, E),
                    Instr(Mnemonic.tlc, AC, E),
                    Instr(Mnemonic.trce, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlce, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.trca, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tlca, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.trcn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlcn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.tdc, AC, E),
                    Instr(Mnemonic.tsc, AC, E),
                    Instr(Mnemonic.tdce, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tsce, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdca, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tsca, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tdcn, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tscn, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.tro, AC, E),
                    Instr(Mnemonic.tlo, AC, E),
                    Instr(Mnemonic.troe, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tloe, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.troa, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tloa, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tron, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tlon, InstrClass.ConditionalTransfer, AC, E),

                    Instr(Mnemonic.tdo, AC, E),
                    Instr(Mnemonic.tso, AC, E),
                    Instr(Mnemonic.tdoe, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tsoe, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdoa, InstrClass.Transfer, AC, E),
                    Instr(Mnemonic.tsoa, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tdon, InstrClass.ConditionalTransfer, AC, E),
                    Instr(Mnemonic.tson, InstrClass.ConditionalTransfer, AC, E));

                var opcodes7 = Mask(bf(10, 3), "  Monitor",
                    Instr(Mnemonic.blki, D, E),     // Block Input, skip if I/O not finished  
                    Instr(Mnemonic.datai, D, E),    // Data Input, from device to memory      
                    Instr(Mnemonic.blko, D, E),     // Block Output, skip if I/O not finished 
                    Instr(Mnemonic.datao, D, E),    // Data Output, from memory to device     
                    Instr(Mnemonic.cono, D, E),     // Conditions Out, 36 bits AC to device   
                    Instr(Mnemonic.coni, D, E),     // Conditions in, 36 bits device to AC    
                    Instr(Mnemonic.consz, D, E),    // Conditions, Skip if Zero (test 18 bits)
                    Instr(Mnemonic.conso, D, E));   // Conditions, Skip if One  (test 18 bits)

                return Mask(bf(0, 3), "PDP-10",
                    opcodes0,
                    opcodes1,
                    opcodes2,
                    opcodes3,
                    opcodes4,
                    opcodes5,
                    opcodes6,
                    opcodes7);
            }
        }
    }
}