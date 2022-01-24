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

using Reko.Arch.Pdp10.Disassembler;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly PrimitiveType word18 = Pdp10Architecture.Word18;
        private static readonly PrimitiveType word36 = Pdp10Architecture.Word36;

        private readonly Pdp10Architecture arch;
        private readonly Word36BeImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<Pdp10Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> instrs;
        private Pdp10Instruction instr;
        private InstrClass iclass;


        public Pdp10Rewriter(Pdp10Architecture arch, Word36BeImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new Pdp10Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    break;
                case Mnemonic.Invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.addi: RewriteAddiSubi(m.IAdd); break;
                case Mnemonic.addm: RewriteAddm(); break;
                case Mnemonic.and: RewriteAnd(); break;
                case Mnemonic.aobjn: RewriteAobjn(); break;
                case Mnemonic.aoja: RewriteAoja(); break;
                case Mnemonic.aos: RewriteAos(); break;
                case Mnemonic.ash: RewriteAsh(); break;
                case Mnemonic.blki: RewriteBlki(); break;
                case Mnemonic.blko: RewriteBlko(); break;
                case Mnemonic.blt: RewriteBlt(); break;
                case Mnemonic.caie: RewriteCai(m.Eq); break;
                case Mnemonic.caig: RewriteCai(m.Gt); break;
                case Mnemonic.caige: RewriteCai(m.Ge); break;
                case Mnemonic.cail: RewriteCai(m.Lt); break;
                case Mnemonic.caile: RewriteCai(m.Le); break;
                case Mnemonic.cain: RewriteCai(m.Ne); break;
                case Mnemonic.came: RewriteCam(m.Eq); break;
                case Mnemonic.camg: RewriteCam(m.Gt); break;
                case Mnemonic.caml: RewriteCam(m.Lt); break;
                case Mnemonic.camle: RewriteCam(m.Le); break;
                case Mnemonic.camn: RewriteCam(m.Ne); break;
                case Mnemonic.calli: RewriteCalli(); break;
                case Mnemonic.coni: RewriteConi(); break;
                case Mnemonic.datai: RewriteDatai(); break;
                case Mnemonic.datao: RewriteDatao(); break;
                case Mnemonic.dpb: RewriteDpb(); break;
                case Mnemonic.eqvm: RewriteEqvm(); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.hll: RewriteHll(); break;
                case Mnemonic.hllm: RewriteHllm(); break;
                case Mnemonic.hllz: RewriteHllz(); break;
                case Mnemonic.hllzm: RewriteHllzm(); break;
                case Mnemonic.hlr: RewriteHlr(); break;
                case Mnemonic.hlrz: RewriteHlrz(); break;
                case Mnemonic.hrli: RewriteHrli(); break;
                case Mnemonic.hrr: RewriteHrr(); break;
                case Mnemonic.hrrm: RewriteHrrm(); break;
                case Mnemonic.hrrz: RewriteHrrz(); break;
                case Mnemonic.hrrzm: RewriteHrrzm(); break;
                case Mnemonic.ibp: RewriteIbp(); break;
                case Mnemonic.idpb: RewriteIdpb(); break;
                case Mnemonic.idiv: RewriteIdiv(); break;
                case Mnemonic.idivi: RewriteIdivi(); break;
                case Mnemonic.ildb: RewriteIldb(); break;
                case Mnemonic.imul: RewriteImul(); break;
                case Mnemonic.imuli: RewriteImuli(); break;
                case Mnemonic.initi: RewriteIniti(); break;
                case Mnemonic.jrst: RewriteJrst(); break;
                case Mnemonic.jsp: RewriteJsp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.jumpe: RewriteJump(m.Eq0); break;
                case Mnemonic.jumpl: RewriteJump(m.Lt0); break;
                case Mnemonic.ldb: RewriteLdb(); break;
                case Mnemonic.lookup: RewriteLookup(); break;
                case Mnemonic.lsh: RewriteLsh(); break;
                case Mnemonic.luuo01: RewriteLuuo(1); break;
                case Mnemonic.luuo02: RewriteLuuo(2); break;
                case Mnemonic.luuo03: RewriteLuuo(3); break;
                case Mnemonic.luuo04: RewriteLuuo(4); break;
                case Mnemonic.luuo05: RewriteLuuo(5); break;
                case Mnemonic.luuo06: RewriteLuuo(6); break;
                case Mnemonic.luuo07: RewriteLuuo(7); break;
                case Mnemonic.luuo10: RewriteLuuo(8); break;
                case Mnemonic.move: RewriteMove(); break;
                case Mnemonic.movei: RewriteMovei(); break;
                case Mnemonic.movem: RewriteMovem(); break;
                case Mnemonic.movn: RewriteMovn(); break;
                case Mnemonic.movns: RewriteMovns(); break;
                case Mnemonic.movs: RewriteMovs(); break;
                case Mnemonic.movsi: RewriteMovsi(); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.popj: RewritePopj(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.pushj: RewritePushj(); break;
                case Mnemonic.rename: RewriteRename(); break;
                case Mnemonic.setmm: RewriteSetmm(); break; 
                case Mnemonic.seto:
                case Mnemonic.setoi: RewriteSeto(); break;
                case Mnemonic.setob: RewriteSetob(); break;
                case Mnemonic.setom: RewriteSetom(); break;
                case Mnemonic.setzm: RewriteSetzm(); break;
                case Mnemonic.skipa: RewriteSkipa(); break;
                case Mnemonic.skipe: RewriteSkip(m.Eq0); break;
                case Mnemonic.skipg: RewriteSkip(m.Gt0); break;
                case Mnemonic.skipge: RewriteSkip(m.Ge0); break;
                case Mnemonic.skipn: RewriteSkip(m.Ne0); break;
                case Mnemonic.soja: RewriteSoja(); break;
                case Mnemonic.sojl: RewriteSoj(m.Lt0); break;
                case Mnemonic.sos: RewriteSos(); break;
                case Mnemonic.subi: RewriteAddiSubi(m.ISub); break;
                case Mnemonic.tlc: RewriteTlc(); break;
                case Mnemonic.tlne: RewriteTln(m.Eq0); break;
                case Mnemonic.tlnn: RewriteTln(m.Ne0); break;
                case Mnemonic.tlo: RewriteTlo(); break;
                case Mnemonic.tlza: RewriteTlza(); break;
                case Mnemonic.tro: RewriteTro(); break;
                case Mnemonic.troa: RewriteTroa(); break;
                case Mnemonic.trne: RewriteTrn(m.Eq0); break;
                case Mnemonic.trnn: RewriteTrn(m.Ne0); break;
                case Mnemonic.trz: RewriteTrz(); break;
                case Mnemonic.trze: RewriteTrz(m.Eq0); break;
                case Mnemonic.trzn: RewriteTrz(m.Ne0); break;
                case Mnemonic.ttcall: RewriteTtcall(); break;
                case Mnemonic.xct: RewriteXct(); break;
                case Mnemonic.xori: RewriteXori(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }


        private void EmitUnitTest()
        {
            var offset = rdr.Offset;
            rdr.Offset -= 1;
            rdr.TryReadBeUInt36(out ulong word);
            var opcodeAsString = Convert.ToString((long) word, 8).PadLeft(12, '0');
            rdr.Offset = offset;
            var instr = dasm.Current;
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter(
                "Pdp10Rw", instr, instr.Mnemonic.ToString(), rdr, instr.MnemonicAsString, opcodeAsString);

            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Identifier Ac()
        {
            var id = binder.EnsureRegister(((RegisterOperand) instr.Operands[0]).Register);
            return id;
        }

        private Identifier Ac_plus_1()
        {
            var iReg = ((RegisterOperand) instr.Operands[0]).Register.Number + 1;
            //$REVIEW: what if iReg > 15?
            var id = binder.EnsureRegister(Registers.Accumulators[iReg]);
            return id;
        }

        private void Branch(Expression condition, Expression target)
        {
            if (target is Address addr)
            {
                m.Branch(condition, addr);
            }
            else if (target is Constant c)
            {
                m.Branch(condition, arch.MakeAddressFromConstant(c, false));
            }
            else
            {
                m.Branch(condition.Invert(), instr.Address + 1);
                m.Goto(target);
            }
        }

        private Expression AccessEa(int iOp)
        {
            Expression addr;
            switch (instr.Operands[iOp])
            {
            case MemoryOperand ea:
                if (ea.Index is null)
                {
                    // PDP-10 accumulators are memory mapped, so we check for them
                    // first
                    if (ea.Offset < 16)
                    {
                        return binder.EnsureRegister(Registers.Accumulators[ea.Offset]);
                    }
                    return m.Mem(word36, new Address18(ea.Offset));
                }
                addr = binder.EnsureRegister(ea.Index);
                if (ea.Offset != 0)
                {
                    addr = m.IAddS(addr, Bits.SignExtend(ea.Offset, 18));
                }
                if (ea.Indirect)
                {
                    addr = m.Mem(word36, addr);
                }
                break;
            case AddressOperand addrOp:
                addr = addrOp.Address;
                break;
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOp].GetType().Name}");
            }
            return m.Mem(word36, addr);
        }

        private Expression RewriteEa(int iOp, int lsh = 0)
        {
            switch (instr.Operands[iOp])
            {
            case MemoryOperand ea:
                Expression value;
                if (ea.Index is null)
                {
                    value = Constant.Create(word36, (ulong) ea.Offset << lsh);
                }
                else
                {
                    value = binder.EnsureRegister(ea.Index);
                    if (ea.Offset != 0)
                    {
                        value = m.IAddS(value, Bits.SignExtend(ea.Offset, 18));
                    }
                }
                if (ea.Indirect)
                {
                    value = m.Mem(word36, value);
                }
                return value;
            case AddressOperand addr:
                return addr.Address;
            }
            throw new NotImplementedException();
        }

        private Constant Imm(int i)
        {
            var imm = (ImmediateOperand) instr.Operands[i];
            return imm.Value;
        }

        private void Skip()
        {
            m.Goto(instr.Address + 2);
        }

        private void SkipIf(Expression exp)
        {
            m.Branch(exp, instr.Address + 2);
        }

        private void SwapWordHalves(Expression dst, Expression src)
        {
            var tmp = binder.CreateTemporary(word36);
            var left = binder.CreateTemporary(word18);
            var right = binder.CreateTemporary(word18);
            m.Assign(tmp, src);
            m.Assign(left, m.Slice(tmp, left.DataType, 18));
            m.Assign(right, m.Slice(tmp, right.DataType, 0));
            m.Assign(dst, m.Seq(right, left));
        }

        private Constant Word18OfOnes()
        {
            return Constant.Create(word18, (1ul << 18) - 1);
        }

        private Constant Word36OfOnes()
        {
            return Constant.Create(word36, (1ul << 36) - 1);
        }

        private static readonly FlagGroupStorage C0C1VT;
        private static readonly FlagGroupStorage C1VT;
        private static readonly FlagGroupStorage VT;
        private static readonly FlagGroupStorage VTND;
        private static readonly IntrinsicProcedure blkiIntrinsic;
        private static readonly IntrinsicProcedure blkoIntrinsic;
        private static readonly IntrinsicProcedure bltIntrinsic;
        private static readonly IntrinsicProcedure calliIntrinsic;
        private static readonly IntrinsicProcedure coniIntrinsic;
        private static readonly IntrinsicProcedure dataiIntrinsic;
        private static readonly IntrinsicProcedure dataoIntrinsic;
        private static readonly IntrinsicProcedure dpbIntrinsic;
        private static readonly IntrinsicProcedure haltIntrinsic;
        private static readonly IntrinsicProcedure ldbIntrinsic;
        private static readonly IntrinsicProcedure ibpIntrinsic;
        private static readonly IntrinsicProcedure idpbIntrinsic;
        private static readonly IntrinsicProcedure ildbIntrinsic;
        private static readonly IntrinsicProcedure initiIntrinsic;
        private static readonly IntrinsicProcedure lookupIntrinsic;
        private static readonly IntrinsicProcedure lshIntrinsic;
        private static readonly IntrinsicProcedure luuoIntrinsic;
        private static readonly IntrinsicProcedure renameIntrinsic;
        private static readonly IntrinsicProcedure ttcallIntrinsic;
        private static readonly IntrinsicProcedure xctIntrinsic;

        static Pdp10Rewriter()
        {
            C0C1VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.C0 | FlagM.C1 | FlagM.V | FlagM.T), "C0C1VT", word36);
            C1VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.C1 | FlagM.V | FlagM.T), "C1VT", word36);
            VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.V | FlagM.T), "VT", word36);
            VTND = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.V | FlagM.T | FlagM.ND), "VTND", word36);
            var ptr36 = PrimitiveType.Create(Domain.Pointer, 36);
            blkiIntrinsic = new IntrinsicBuilder("pdp10_blki", true)
                .Param(word36)
                .Param(word36)
                .Void();
            blkoIntrinsic = new IntrinsicBuilder("pdp10_blko", true)
                .Param(word36)
                .Param(word36)
                .Void();
            bltIntrinsic = new IntrinsicBuilder("pdp10_blt", true)
                .Param(word36)
                .Param(word36)
                .Void();
            calliIntrinsic = new IntrinsicBuilder("pdp10_calli", true)
                .Param(word36)
                .Param(word36)
                .Void();
            coniIntrinsic = new IntrinsicBuilder("pdp10_coni", true)
                .Param(word36)
                .Param(word36)
                .Void();
            dataiIntrinsic = new IntrinsicBuilder("pdp10_datai", true)
                .Param(word36)
                .Returns(word36);
            dataoIntrinsic = new IntrinsicBuilder("pdp10_datao", true)
                .Param(word36)
                .Param(word36)
                .Void();
            dpbIntrinsic = new IntrinsicBuilder("pdp10_deposit_byte", true)
                .Param(word36)
                .Param(ptr36)
                .Void();
            haltIntrinsic = new IntrinsicBuilder("pdp10_halt", true)
                .Param(PrimitiveType.Create(Domain.Pointer, 36))
                .Void();
            ibpIntrinsic = new IntrinsicBuilder("pdp10_inc_byte_ptr", true)
                .Param(ptr36)
                .Returns(ptr36);
            idpbIntrinsic = new IntrinsicBuilder("pdp10_inc_ptr_deposit_byte", true)
                .Param(ptr36)
                .Param(word36)
                .Returns(ptr36);
            initiIntrinsic = new IntrinsicBuilder("pdp10_initi", true)
                .Param(word36)
                .Param(word36)
                .Void();
            ildbIntrinsic = new IntrinsicBuilder("pdp10_inc_byte_ptr_and_load", true)
                .Param(ptr36)
                .Returns(word36);

            ldbIntrinsic = new IntrinsicBuilder("pdp10_load_byte", false)
                .Param(word36)
                .Returns(ptr36);
            lookupIntrinsic = new IntrinsicBuilder("pdp10_lookup", true)
                .Param(word36)
                .Param(word36)
                .Void();
            lshIntrinsic = new IntrinsicBuilder("logical_shift", true)
                .Param(word36)
                .Param(word36)
                .Returns(word36);
            luuoIntrinsic = new IntrinsicBuilder("pdp10_luuo", true)
                .Param(word36)
                .Param(word36)
                .Param(word36)
                .Void();
            renameIntrinsic = new IntrinsicBuilder("pdp10_rename", true)
                .Param(word36)
                .Param(word36)
                .Void();
            ttcallIntrinsic = new IntrinsicBuilder("pdp10_ttcall", true)
                .Param(word36)
                .Param(word36)
                .Void();
            xctIntrinsic = new IntrinsicBuilder("pdp10_xct", true)
                .Param(word36)
                .Param(new Pointer(word36, 36))
                .Void();
        }
    }
}
