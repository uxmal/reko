#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Arch.Pdp10.Disassembler
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
                var opcodes0 = Mask(bf(3, 6), "  Monitor UUOs and Local UUOs",

                    invalid,
                    Nyi(Mnemonic.luuo01),
                    Nyi(Mnemonic.luuo02),
                    Nyi(Mnemonic.luuo03),
                    Nyi(Mnemonic.luuo04),
                    Nyi(Mnemonic.luuo05),
                    Nyi(Mnemonic.luuo06),
                    Nyi(Mnemonic.luuo07),
                    Nyi(Mnemonic.luuo10),
                    Nyi(Mnemonic.luuo11),
                    Nyi(Mnemonic.luuo12),
                    Nyi(Mnemonic.luuo13),
                    Nyi(Mnemonic.luuo14),
                    Nyi(Mnemonic.luuo15),
                    Nyi(Mnemonic.luuo16),
                    Nyi(Mnemonic.luuo17),
                    Nyi(Mnemonic.luuo20),
                    Nyi(Mnemonic.luuo21),
                    Nyi(Mnemonic.luuo22),
                    Nyi(Mnemonic.luuo23),
                    Nyi(Mnemonic.luuo24),
                    Nyi(Mnemonic.luuo25),
                    Nyi(Mnemonic.luuo26),
                    Nyi(Mnemonic.luuo27),
                    Nyi(Mnemonic.luuo30),
                    Nyi(Mnemonic.luuo31),
                    Nyi(Mnemonic.luuo32),
                    Nyi(Mnemonic.luuo33),
                    Nyi(Mnemonic.luuo34),
                    Nyi(Mnemonic.luuo35),
                    Nyi(Mnemonic.luuo36),
                    Nyi(Mnemonic.luuo37),
                    Nyi(Mnemonic.call),
                    Nyi(Mnemonic.initi),
                    Nyi(Mnemonic.muuo42),
                    Nyi(Mnemonic.muuo43),
                    Nyi(Mnemonic.muuo44),
                    Nyi(Mnemonic.muuo45),
                    Nyi(Mnemonic.muuo46),
                    Nyi(Mnemonic.calli),
                    Nyi(Mnemonic.open),
                    Nyi(Mnemonic.ttcall),
                    Nyi(Mnemonic.muuo52),
                    Nyi(Mnemonic.muuo53),
                    Nyi(Mnemonic.muuo54),
                    Nyi(Mnemonic.rename),
                    Nyi(Mnemonic.@in),
                    Nyi(Mnemonic.@out),
                    Nyi(Mnemonic.setsts),
                    Nyi(Mnemonic.stato),
                    Nyi(Mnemonic.status),
                    Nyi(Mnemonic.getsts),
                    Nyi(Mnemonic.inbuf),
                    Nyi(Mnemonic.outbuf),
                    Nyi(Mnemonic.input),
                    Nyi(Mnemonic.output),
                    Nyi(Mnemonic.close),
                    Nyi(Mnemonic.releas),
                    Nyi(Mnemonic.mtape),
                    Nyi(Mnemonic.ugetf),
                    Nyi(Mnemonic.useti),
                    Nyi(Mnemonic.useto),
                    Nyi(Mnemonic.lookup),
                    Nyi(Mnemonic.enter));

                var opcodes1 = Mask(bf(3,6), "  Floating point, byte manipulation, other",
                    Nyi(Mnemonic.ujen),
                    invalid,   // 101,
                    Nyi(Mnemonic.gfad),
                    Nyi(Mnemonic.gfsb),
                    Nyi(Mnemonic.jsys),
                    Nyi(Mnemonic.adjsp),
                    Nyi(Mnemonic.gfmp),
                    Nyi(Mnemonic.gfdv),

                    Nyi(Mnemonic.dfad),
                    Nyi(Mnemonic.dfsb),
                    Nyi(Mnemonic.dfmp),
                    Nyi(Mnemonic.dfdv),
                    Nyi(Mnemonic.dadd),
                    Nyi(Mnemonic.dsub),
                    Nyi(Mnemonic.dmul),
                    Nyi(Mnemonic.ddiv),

                    Nyi(Mnemonic.dmove),
                    Nyi(Mnemonic.dmovn),
                    Nyi(Mnemonic.fix),
                    Nyi(Mnemonic.extend),
                    Nyi(Mnemonic.dmovem),
                    Nyi(Mnemonic.dmovnm),
                    Nyi(Mnemonic.fixr),
                    Nyi(Mnemonic.fltr),

                    Nyi(Mnemonic.ufa),
                    Nyi(Mnemonic.dfn),
                    Nyi(Mnemonic.fsc),
                    Nyi(Mnemonic.ibp),
                    Nyi(Mnemonic.ildb),
                    Nyi(Mnemonic.ldb),
                    Nyi(Mnemonic.idpb),
                    Nyi(Mnemonic.dpb),

                    Nyi(Mnemonic.fad),
                    Nyi(Mnemonic.fadl),
                    Nyi(Mnemonic.fadm),
                    Nyi(Mnemonic.fadb),
                    Nyi(Mnemonic.fadr),
                    Nyi(Mnemonic.fadrl),
                    Nyi(Mnemonic.fadrm),
                    Nyi(Mnemonic.fadrb),

                    Nyi(Mnemonic.fsb),
                    Nyi(Mnemonic.fsbl),
                    Nyi(Mnemonic.fsbm),
                    Nyi(Mnemonic.fsbb),
                    Nyi(Mnemonic.fsbr),
                    Nyi(Mnemonic.fsbrl),
                    Nyi(Mnemonic.fsbrm),
                    Nyi(Mnemonic.fsbrb),

                    Nyi(Mnemonic.fmp),
                    Nyi(Mnemonic.fmpl),
                    Nyi(Mnemonic.fmpm),
                    Nyi(Mnemonic.fmpb),
                    Nyi(Mnemonic.fmpr),
                    Nyi(Mnemonic.fmprl),
                    Nyi(Mnemonic.fmprm),
                    Nyi(Mnemonic.fmprb),

                    Nyi(Mnemonic.fdv),
                    Nyi(Mnemonic.fdvl),
                    Nyi(Mnemonic.fdvm),
                    Nyi(Mnemonic.fdvb),
                    Nyi(Mnemonic.fdvr),
                    Nyi(Mnemonic.fdvrl),
                    Nyi(Mnemonic.fdvrm),
                    Nyi(Mnemonic.fdvrb));

                var opcodes2 = Mask(bf(3, 6), "  Integer arithmetic, jump to subroutine",
                    Nyi(Mnemonic.move),
                    Instr(Mnemonic.movei, AC, Imm),
                    Nyi(Mnemonic.movem),
                    Nyi(Mnemonic.moves),
                    Nyi(Mnemonic.movs),
                    Nyi(Mnemonic.movsi),
                    Nyi(Mnemonic.movsm),
                    Nyi(Mnemonic.movss),

                    Nyi(Mnemonic.movn),
                    Nyi(Mnemonic.movni),
                    Nyi(Mnemonic.movnm),
                    Nyi(Mnemonic.movns),
                    Nyi(Mnemonic.movm),
                    Nyi(Mnemonic.movmi),
                    Nyi(Mnemonic.movmm),
                    Nyi(Mnemonic.movms),

                    Nyi(Mnemonic.imul),
                    Nyi(Mnemonic.imuli),
                    Nyi(Mnemonic.imulm),
                    Nyi(Mnemonic.imulb),
                    Nyi(Mnemonic.mul),
                    Instr(Mnemonic.muli, AC, Imm),
                    Nyi(Mnemonic.mulm),
                    Nyi(Mnemonic.mulb),

                    Nyi(Mnemonic.idiv),
                    Nyi(Mnemonic.idivi),
                    Nyi(Mnemonic.idivm),
                    Nyi(Mnemonic.idivb),
                    Nyi(Mnemonic.div),
                    Instr(Mnemonic.divi, AC, Imm),
                    Nyi(Mnemonic.divm),
                    Nyi(Mnemonic.divb),

                    Nyi(Mnemonic.ash),
                    Nyi(Mnemonic.rot),
                    Nyi(Mnemonic.lsh),
                    Nyi(Mnemonic.jffo),
                    Nyi(Mnemonic.ashc),
                    Nyi(Mnemonic.rotc),
                    Nyi(Mnemonic.lshc),
                    invalid,   // 247),

                    Nyi(Mnemonic.exch),
                    Nyi(Mnemonic.blt),
                    Nyi(Mnemonic.aobjp),
                    Nyi(Mnemonic.aobjn),
                    Nyi(Mnemonic.jrst),
                    jfclVariants,
                    Nyi(Mnemonic.xct),
                    Nyi(Mnemonic.map),

                    Nyi(Mnemonic.pushj),
                    Nyi(Mnemonic.push),
                    Nyi(Mnemonic.pop),
                    Nyi(Mnemonic.popj),
                    Nyi(Mnemonic.jsr),
                    Nyi(Mnemonic.jsp),
                    Nyi(Mnemonic.jsa),
                    Nyi(Mnemonic.jra),

                    Nyi(Mnemonic.add),
                    Instr(Mnemonic.addi, AC, Imm),
                    Nyi(Mnemonic.addm),
                    Nyi(Mnemonic.addb),
                    Nyi(Mnemonic.sub),
                    Instr(Mnemonic.subi, AC, Imm),
                    Nyi(Mnemonic.subm),
                    Nyi(Mnemonic.subb));

                var opcodes3 = Mask(bf(3, 6), "  Hop, skip, and jump", // codes 3_0 do not skip or jump
                    Nyi(Mnemonic.cai),
                    Nyi(Mnemonic.cail),
                    Nyi(Mnemonic.caie),
                    Nyi(Mnemonic.caile),
                    Nyi(Mnemonic.caia),
                    Nyi(Mnemonic.caige),
                    Nyi(Mnemonic.cain),
                    Nyi(Mnemonic.caig),

                    Nyi(Mnemonic.cam),
                    Nyi(Mnemonic.caml),
                    Nyi(Mnemonic.came),
                    Nyi(Mnemonic.camle),
                    Nyi(Mnemonic.cama),
                    Nyi(Mnemonic.camge),
                    Nyi(Mnemonic.camn),
                    Nyi(Mnemonic.camg),

                    Instr(Mnemonic.jump, AC, J),
                    Instr(Mnemonic.jumpl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpe, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumple, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpa, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.jumpge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.jumpg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.skip, AC, J),
                    Instr(Mnemonic.skipl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.skipe, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.skiple, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.skipa, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.skipge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.skipn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.skipg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.aoj, AC, J),
                    Instr(Mnemonic.aojl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aoje, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojle, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aoja, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.aojge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.aojg, InstrClass.ConditionalTransfer, AC, J),

                    Instr(Mnemonic.aos),
                    Instr(Mnemonic.aosl),
                    Instr(Mnemonic.aose),
                    Instr(Mnemonic.aosle),
                    Instr(Mnemonic.aosa),
                    Instr(Mnemonic.aosge),
                    Instr(Mnemonic.aosn),
                    Instr(Mnemonic.aosg),

                    Instr(Mnemonic.soj, AC, J),
                    Instr(Mnemonic.sojl, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.soje, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojle, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.soja, InstrClass.Transfer, AC, J),
                    Instr(Mnemonic.sojge, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojn, InstrClass.ConditionalTransfer, AC, J),
                    Instr(Mnemonic.sojg, InstrClass.ConditionalTransfer, AC, J),

                    Nyi(Mnemonic.sos),
                    Nyi(Mnemonic.sosl),
                    Nyi(Mnemonic.sose),
                    Nyi(Mnemonic.sosle),
                    Nyi(Mnemonic.sosa),
                    Nyi(Mnemonic.sosge),
                    Nyi(Mnemonic.sosn),
                    Nyi(Mnemonic.sosg));

                var opcodes4 = Mask(bf(3, 6), "  two-argument logical operations",
                    Nyi(Mnemonic.setz),
                    Nyi(Mnemonic.setzi),
                    Nyi(Mnemonic.setzm),
                    Nyi(Mnemonic.setzb),
                    Nyi(Mnemonic.and),
                    Nyi(Mnemonic.andi),
                    Nyi(Mnemonic.andm),
                    Nyi(Mnemonic.andb),

                    Nyi(Mnemonic.andca),
                    Nyi(Mnemonic.andcai),
                    Nyi(Mnemonic.andcam),
                    Nyi(Mnemonic.andcab),
                    Nyi(Mnemonic.setm),
                    Nyi(Mnemonic.xmovei),
                    Nyi(Mnemonic.setmm),
                    Nyi(Mnemonic.setmb),

                    Nyi(Mnemonic.andcm),
                    Nyi(Mnemonic.andcmi),
                    Nyi(Mnemonic.andcmm),
                    Nyi(Mnemonic.andcmb),
                    Nyi(Mnemonic.seta),
                    Nyi(Mnemonic.setai),
                    Nyi(Mnemonic.setam),
                    Nyi(Mnemonic.setab),

                    Nyi(Mnemonic.xor),
                    Nyi(Mnemonic.xori),
                    Nyi(Mnemonic.xorm),
                    Nyi(Mnemonic.xorb),
                    Nyi(Mnemonic.or),
                    Nyi(Mnemonic.ori),
                    Nyi(Mnemonic.orm),
                    Nyi(Mnemonic.orb),

                    Nyi(Mnemonic.andcb),
                    Nyi(Mnemonic.andcbi),
                    Nyi(Mnemonic.andcbm),
                    Nyi(Mnemonic.andcbb),
                    Nyi(Mnemonic.eqv),
                    Nyi(Mnemonic.eqvi),
                    Nyi(Mnemonic.eqvm),
                    Nyi(Mnemonic.eqvb),

                    Nyi(Mnemonic.setca),
                    Nyi(Mnemonic.setcai),
                    Nyi(Mnemonic.setcam),
                    Nyi(Mnemonic.setcab),
                    Nyi(Mnemonic.orca),
                    Nyi(Mnemonic.orcai),
                    Nyi(Mnemonic.orcam),
                    Nyi(Mnemonic.orcab),

                    Nyi(Mnemonic.setcm),
                    Nyi(Mnemonic.setcmi),
                    Nyi(Mnemonic.setcmm),
                    Nyi(Mnemonic.setcmb),
                    Nyi(Mnemonic.orcm),
                    Nyi(Mnemonic.orcmi),
                    Nyi(Mnemonic.orcmm),
                    Nyi(Mnemonic.orcmb),

                    Nyi(Mnemonic.orcb),
                    Nyi(Mnemonic.orcbi),
                    Nyi(Mnemonic.orcbm),
                    Nyi(Mnemonic.orcbb),
                    Nyi(Mnemonic.seto),
                    Nyi(Mnemonic.setoi),
                    Nyi(Mnemonic.setom),
                    Nyi(Mnemonic.setob));


                // Half word {left,right} to {left,right} with {nochange,zero,ones,extend},
                //{ac,immediate,memory,self}
                var opcodes5 = Mask(bf(3, 6), "  Half words",
                    Nyi(Mnemonic.hll),
                    Nyi(Mnemonic.xhlli),
                    Nyi(Mnemonic.hllm),
                    Nyi(Mnemonic.hlls),
                    Nyi(Mnemonic.hrl),
                    Nyi(Mnemonic.hrli),
                    Nyi(Mnemonic.hrlm),
                    Nyi(Mnemonic.hrls),

                    Nyi(Mnemonic.hllz),
                    Nyi(Mnemonic.hllzi),
                    Nyi(Mnemonic.hllzm),
                    Nyi(Mnemonic.hllzs),
                    Nyi(Mnemonic.hrlz),
                    Nyi(Mnemonic.hrlzi),
                    Nyi(Mnemonic.hrlzm),
                    Nyi(Mnemonic.hrlzs),

                    Nyi(Mnemonic.hllo),
                    Nyi(Mnemonic.hlloi),
                    Nyi(Mnemonic.hllom),
                    Nyi(Mnemonic.hllos),
                    Nyi(Mnemonic.hrlo),
                    Nyi(Mnemonic.hrloi),
                    Nyi(Mnemonic.hrlom),
                    Nyi(Mnemonic.hrlos),

                    Nyi(Mnemonic.hlle),
                    Nyi(Mnemonic.hllei),
                    Nyi(Mnemonic.hllem),
                    Nyi(Mnemonic.hlles),
                    Nyi(Mnemonic.hrle),
                    Nyi(Mnemonic.hrlei),
                    Nyi(Mnemonic.hrlem),
                    Nyi(Mnemonic.hrles),

                    Nyi(Mnemonic.hrr),
                    Nyi(Mnemonic.hrri),
                    Nyi(Mnemonic.hrrm),
                    Nyi(Mnemonic.hrrs),
                    Nyi(Mnemonic.hlr),
                    Nyi(Mnemonic.hlri),
                    Nyi(Mnemonic.hlrm),
                    Nyi(Mnemonic.hlrs),

                    Nyi(Mnemonic.hrrz),
                    Nyi(Mnemonic.hrrzi),
                    Nyi(Mnemonic.hrrzm),
                    Nyi(Mnemonic.hrrzs),
                    Nyi(Mnemonic.hlrz),
                    Nyi(Mnemonic.hlrzi),
                    Nyi(Mnemonic.hlrzm),
                    Nyi(Mnemonic.hlrzs),

                    Nyi(Mnemonic.hrro),
                    Nyi(Mnemonic.hrroi),
                    Nyi(Mnemonic.hrrom),
                    Nyi(Mnemonic.hrros),
                    Nyi(Mnemonic.hlro),
                    Nyi(Mnemonic.hlroi),
                    Nyi(Mnemonic.hlrom),
                    Nyi(Mnemonic.hlros),

                    Nyi(Mnemonic.hrre),
                    Nyi(Mnemonic.hrrei),
                    Nyi(Mnemonic.hrrem),
                    Nyi(Mnemonic.hrres),
                    Nyi(Mnemonic.hlre),
                    Nyi(Mnemonic.hlrei),
                    Nyi(Mnemonic.hlrem),
                    Nyi(Mnemonic.hlres));

                // Test bits, {right,left,direct,swapped} with {nochange,zero,complement,one}
                // and skip if the masked bits were {noskip,equal,nonzero,always}
                var opcodes6 = Mask(bf(3,6), "  Test bits",
                    Nyi(Mnemonic.trn),
                    Nyi(Mnemonic.tln),
                    Nyi(Mnemonic.trne),
                    Nyi(Mnemonic.tlne),
                    Nyi(Mnemonic.trna),
                    Nyi(Mnemonic.tlna),
                    Nyi(Mnemonic.trnn),
                    Nyi(Mnemonic.tlnn),

                    Nyi(Mnemonic.tdn),
                    Nyi(Mnemonic.tsn),
                    Nyi(Mnemonic.tdne),
                    Nyi(Mnemonic.tsne),
                    Nyi(Mnemonic.tdna),
                    Nyi(Mnemonic.tsna),
                    Nyi(Mnemonic.tdnn),
                    Nyi(Mnemonic.tsnn),

                    Nyi(Mnemonic.trz),
                    Nyi(Mnemonic.tlz),
                    Nyi(Mnemonic.trze),
                    Nyi(Mnemonic.tlze),
                    Nyi(Mnemonic.trza),
                    Nyi(Mnemonic.tlza),
                    Nyi(Mnemonic.trzn),
                    Nyi(Mnemonic.tlzn),

                    Nyi(Mnemonic.tdz),
                    Nyi(Mnemonic.tsz),
                    Nyi(Mnemonic.tdze),
                    Nyi(Mnemonic.tsze),
                    Nyi(Mnemonic.tdza),
                    Nyi(Mnemonic.tsza),
                    Nyi(Mnemonic.tdzn),
                    Nyi(Mnemonic.tszn),

                    Nyi(Mnemonic.trc),
                    Nyi(Mnemonic.tlc),
                    Nyi(Mnemonic.trce),
                    Nyi(Mnemonic.tlce),
                    Nyi(Mnemonic.trca),
                    Nyi(Mnemonic.tlca),
                    Nyi(Mnemonic.trcn),
                    Nyi(Mnemonic.tlcn),

                    Nyi(Mnemonic.tdc),
                    Nyi(Mnemonic.tsc),
                    Nyi(Mnemonic.tdce),
                    Nyi(Mnemonic.tsce),
                    Nyi(Mnemonic.tdca),
                    Nyi(Mnemonic.tsca),
                    Nyi(Mnemonic.tdcn),
                    Nyi(Mnemonic.tscn),

                    Nyi(Mnemonic.tro),
                    Nyi(Mnemonic.tlo),
                    Nyi(Mnemonic.troe),
                    Nyi(Mnemonic.tloe),
                    Nyi(Mnemonic.troa),
                    Nyi(Mnemonic.tloa),
                    Nyi(Mnemonic.tron),
                    Nyi(Mnemonic.tlon),

                    Nyi(Mnemonic.tdo),
                    Nyi(Mnemonic.tso),
                    Nyi(Mnemonic.tdoe),
                    Nyi(Mnemonic.tsoe),
                    Nyi(Mnemonic.tdoa),
                    Nyi(Mnemonic.tsoa),
                    Nyi(Mnemonic.tdon),
                    Nyi(Mnemonic.tson));

                var opcodes7 = Mask(bf(10, 3), "  Monitor",
                    Nyi(Mnemonic.blki),     // Block Input, skip if I/O not finished  
                    Nyi(Mnemonic.datai),    // Data Input, from device to memory      
                    Nyi(Mnemonic.blko),     // Block Output, skip if I/O not finished 
                    Nyi(Mnemonic.datao),    // Data Output, from memory to device     
                    Nyi(Mnemonic.cono),     // Conditions Out, 36 bits AC to device   
                    Nyi(Mnemonic.coni),     // Conditions in, 36 bits device to AC    
                    Nyi(Mnemonic.consz),    // Conditions, Skip if Zero (test 18 bits)
                    Nyi(Mnemonic.conso));   // Conditions, Skip if One  (test 18 bits)

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