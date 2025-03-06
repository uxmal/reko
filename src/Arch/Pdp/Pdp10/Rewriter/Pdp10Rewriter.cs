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
using Reko.Arch.Pdp.Pdp10.Disassembler;
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

namespace Reko.Arch.Pdp.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly PrimitiveType word18 = PdpTypes.Word18;
        private static readonly PrimitiveType ptr18 = PdpTypes.Ptr18;
        private static readonly PrimitiveType word36 = PdpTypes.Word36;
        private static readonly PrimitiveType word72 = PdpTypes.Word72;
        private static readonly PrimitiveType int36 = PdpTypes.Int36;
        private static readonly PrimitiveType real36 = PdpTypes.Real36;
        private static readonly PrimitiveType real72 = PdpTypes.Real72;

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
                case Mnemonic.addb: BMode(m.IAdd, C0C1VT); break;
                case Mnemonic.addi: IMode(m.IAdd, C0C1VT); break;
                case Mnemonic.addm: MMode(m.IAdd, C0C1VT); break;
                case Mnemonic.and: Basic(m.And); break;
                case Mnemonic.andca: Basic(Andca); break;
                case Mnemonic.andcai: IMode(Andca); break;
                case Mnemonic.andcam: MMode(Andca); break;
                case Mnemonic.andcb: Basic(Andcb); break;
                case Mnemonic.andcm: Basic(Andcm); break;
                case Mnemonic.andcmi: IMode(Andcm); break;
                case Mnemonic.andcmm: MMode(Andcm); break;
                case Mnemonic.andi: IMode(m.And); break;
                case Mnemonic.andm: MMode(m.And); break;
                case Mnemonic.aobjn: RewriteAobjn(); break;
                case Mnemonic.aoj: RewriteAoj(); break;
                case Mnemonic.aoja: RewriteAoja(); break;
                case Mnemonic.aojge: RewriteAoj(m.Ge0); break;
                case Mnemonic.aojl: RewriteAoj(m.Lt0); break;
                case Mnemonic.aojn: RewriteAoj(m.Ne0); break;
                case Mnemonic.aos: RewriteAos(); break;
                case Mnemonic.aosle: RewriteAos(m.Le0); break;
                case Mnemonic.ash: RewriteAsh(); break;
                case Mnemonic.ashc: RewriteAshc(); break;
                case Mnemonic.blki: RewriteBlki(); break;
                case Mnemonic.blko: RewriteBlko(); break;
                case Mnemonic.blt: RewriteBlt(); break;
                case Mnemonic.cai: Nop(); break;
                case Mnemonic.caia: Skip(); break;
                case Mnemonic.caie: RewriteCai(m.Eq); break;
                case Mnemonic.caig: RewriteCai(m.Gt); break;
                case Mnemonic.caige: RewriteCai(m.Ge); break;
                case Mnemonic.cail: RewriteCai(m.Lt); break;
                case Mnemonic.caile: RewriteCai(m.Le); break;
                case Mnemonic.cain: RewriteCai(m.Ne); break;
                case Mnemonic.cam: Nop(); break;
                case Mnemonic.cama: Skip(); break;
                case Mnemonic.came: RewriteCam(m.Eq); break;
                case Mnemonic.camg: RewriteCam(m.Gt); break;
                case Mnemonic.camge: RewriteCam(m.Ge); break;
                case Mnemonic.caml: RewriteCam(m.Lt); break;
                case Mnemonic.camle: RewriteCam(m.Le); break;
                case Mnemonic.camn: RewriteCam(m.Ne); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.calli: RewriteCalli(); break;
                case Mnemonic.close: RewriteClose(); break;
                case Mnemonic.coni: RewriteConi(); break;
                case Mnemonic.cono: RewriteCono(); break;
                case Mnemonic.conso: RewriteConso(); break;
                case Mnemonic.consz: RewriteConsz(); break;
                case Mnemonic.dadd: RewriteDadd(); break;
                case Mnemonic.datai: RewriteDatai(); break;
                case Mnemonic.datao: RewriteDatao(); break;
                case Mnemonic.dfdv: RewriteDfdv(); break;
                case Mnemonic.dpb: RewriteDpb(); break;
                case Mnemonic.eqvi: IMode(Eqv); break;
                case Mnemonic.eqvm: MMode(Eqv); break;
                case Mnemonic.fsb: RewriteFsb(); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.hll: RewriteHll(); break;
                case Mnemonic.hllm: RewriteHllm(); break;
                case Mnemonic.hllo: RewriteHllo(); break;
                case Mnemonic.hllom: RewriteHllom(); break;
                case Mnemonic.hllz: RewriteHllz(); break;
                case Mnemonic.hllzm: RewriteHllzm(); break;
                case Mnemonic.hllzs: RewriteHllzs(); break;
                case Mnemonic.hlr: RewriteHlr(); break;
                case Mnemonic.hlre: RewriteHlre(); break;
                case Mnemonic.hlrem: RewriteHlrem(); break;
                case Mnemonic.hlrm: RewriteHlrm(); break;
                case Mnemonic.hlrz: RewriteHlrz(); break;
                case Mnemonic.hrl: RewriteHrl(); break;
                case Mnemonic.hrli: RewriteHrli(); break;
                case Mnemonic.hrlm: RewriteHrlm(); break;
                case Mnemonic.hrls: RewriteHrls(); break;
                case Mnemonic.hrlzm: RewriteHrlzm(); break;
                case Mnemonic.hrr: RewriteHrr(); break;
                case Mnemonic.hrrm: RewriteHrrm(); break;
                case Mnemonic.hrrz: RewriteHrrz(); break;
                case Mnemonic.hrrzm: RewriteHrrzm(); break;
                case Mnemonic.ibp: RewriteIbp(); break;
                case Mnemonic.idpb: RewriteIdpb(); break;
                case Mnemonic.idiv: RewriteIdiv(); break;
                case Mnemonic.idivb: RewriteIdivb(); break;
                case Mnemonic.idivi: RewriteIdivi(); break;
                case Mnemonic.ildb: RewriteIldb(); break;
                case Mnemonic.imul: RewriteImul(); break;
                case Mnemonic.imuli: RewriteImuli(); break;
                case Mnemonic.inbuf: RewriteInbuf(); break;
                case Mnemonic.initi: RewriteIniti(); break;
                case Mnemonic.jrst: RewriteJrst(); break;
                case Mnemonic.jsp: RewriteJsp(); break;
                case Mnemonic.jsr: RewriteJsr(); break;
                case Mnemonic.jsys: RewriteJsys(); break;
                case Mnemonic.jumpe: RewriteJump(m.Eq0); break;
                case Mnemonic.jumpg: RewriteJump(m.Gt0); break;
                case Mnemonic.jumpge: RewriteJump(m.Ge0); break;
                case Mnemonic.jumpl: RewriteJump(m.Lt0); break;
                case Mnemonic.jumpn: RewriteJump(m.Ne0); break;
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
                case Mnemonic.luuo11: RewriteLuuo(9); break;
                case Mnemonic.luuo12: RewriteLuuo(10); break;
                case Mnemonic.luuo13: RewriteLuuo(11); break;
                case Mnemonic.luuo14: RewriteLuuo(12); break;
                case Mnemonic.luuo15: RewriteLuuo(13); break;
                case Mnemonic.luuo16: RewriteLuuo(14); break;
                case Mnemonic.luuo17: RewriteLuuo(15); break;
                case Mnemonic.luuo20: RewriteLuuo(16); break;
                case Mnemonic.luuo21: RewriteLuuo(17); break;
                case Mnemonic.luuo22: RewriteLuuo(18); break;
                case Mnemonic.luuo23: RewriteLuuo(19); break;
                case Mnemonic.luuo24: RewriteLuuo(20); break;
                case Mnemonic.luuo25: RewriteLuuo(21); break;
                case Mnemonic.luuo26: RewriteLuuo(22); break;
                case Mnemonic.luuo27: RewriteLuuo(23); break;
                case Mnemonic.luuo30: RewriteLuuo(24); break;
                case Mnemonic.luuo31: RewriteLuuo(25); break;
                case Mnemonic.luuo32: RewriteLuuo(26); break;
                case Mnemonic.move: RewriteMove(); break;
                case Mnemonic.movei: RewriteMovei(); break;
                case Mnemonic.movem: RewriteMovem(); break;
                case Mnemonic.movm: Basic(Movm); break;
                case Mnemonic.movmm: MMode(Movm); break;
                case Mnemonic.movms: RewriteMovms(); break;
                case Mnemonic.movn: Basic(Movn, C1VT); break;
                case Mnemonic.movnm: MMode(Movn, C1VT); break;
                case Mnemonic.movns: RewriteMovns(); break;
                case Mnemonic.movs: RewriteMovs(); break;
                case Mnemonic.movsi: RewriteMovsi(); break;
                case Mnemonic.muli: IMode(m.UMul); break;
                case Mnemonic.muuo42: RewriteMuuo(34); break;
                case Mnemonic.muuo43: RewriteMuuo(35); break;
                case Mnemonic.muuo44: RewriteMuuo(36); break;
                case Mnemonic.muuo45: RewriteMuuo(37); break;
                case Mnemonic.muuo46: RewriteMuuo(38); break;
                case Mnemonic.muuo52: RewriteMuuo(42); break;
                case Mnemonic.muuo53: RewriteMuuo(43); break;
                case Mnemonic.or: Basic(m.Or); break;
                case Mnemonic.ori: IMode(m.Or); break;
                case Mnemonic.orm: MMode(m.Or); break;
                case Mnemonic.orca: Basic(Orca); break;
                case Mnemonic.orcab: BMode(Orca); break;
                case Mnemonic.orcam: MMode(Orca); break;
                case Mnemonic.orcb: Basic(Orcb); break;
                case Mnemonic.orcm: Basic(Orcm); break;
                case Mnemonic.orcmi: IMode(Orcm); break;
                case Mnemonic.orcmm: MMode(Orcm); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.popj: RewritePopj(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.pushj: RewritePushj(); break;
                case Mnemonic.rename: RewriteRename(); break;
                case Mnemonic.seta: Nop(); break;
                case Mnemonic.setab: BMode(Seta); break;
                case Mnemonic.setai: Nop(); break;
                case Mnemonic.setam: MMode(Seta); break;
                case Mnemonic.setcm: Basic(Setcm); break;
                case Mnemonic.setm: Basic(Setm); break;
                case Mnemonic.setmm: MMode(Setm); break; 
                case Mnemonic.seto:  Basic(Seto); break;
                case Mnemonic.setob: BMode(Seto); break;
                case Mnemonic.setoi: IMode(Seto); break;
                case Mnemonic.setom: MMode(Seto); break;
                case Mnemonic.setsts: RewriteSetsts(); break;
                case Mnemonic.setz: RewriteSetz(); break;
                case Mnemonic.setzb: BMode(Setz); break;
                case Mnemonic.setzi: IMode(Setz); break;
                case Mnemonic.setzm: MMode(Setz); break;
                case Mnemonic.skipa: RewriteSkipa(); break;
                case Mnemonic.skipe: RewriteSkip(m.Eq0); break;
                case Mnemonic.skipg: RewriteSkip(m.Gt0); break;
                case Mnemonic.skipge: RewriteSkip(m.Ge0); break;
                case Mnemonic.skipl: RewriteSkip(m.Lt0); break;
                case Mnemonic.skiple: RewriteSkip(m.Le0); break;
                case Mnemonic.skipn: RewriteSkip(m.Ne0); break;
                case Mnemonic.soja: RewriteSoja(); break;
                case Mnemonic.sojg: RewriteSoj(m.Gt0); break;
                case Mnemonic.sojge: RewriteSoj(m.Ge0); break;
                case Mnemonic.sojl: RewriteSoj(m.Lt0); break;
                case Mnemonic.sojle: RewriteSoj(m.Le0); break;
                case Mnemonic.sos: RewriteSos(); break;
                case Mnemonic.sose: RewriteSos(m.Eq0); break;
                case Mnemonic.sosg: RewriteSos(m.Gt0); break;
                case Mnemonic.sosge: RewriteSos(m.Ge0); break;
                case Mnemonic.sosle: RewriteSos(m.Le0); break;
                case Mnemonic.sosn: RewriteSos(m.Ne0); break;
                case Mnemonic.sub: Basic(m.ISub, C0C1VT); break;
                case Mnemonic.subb: BMode(m.ISub, C0C1VT); break;
                case Mnemonic.subi: IMode(m.ISub, C0C1VT); break;
                case Mnemonic.subm: MMode(m.ISub, C0C1VT); break;
                case Mnemonic.tlc: RewriteTlc(); break;
                case Mnemonic.tlne: RewriteTln(m.Eq0); break;
                case Mnemonic.tlnn: RewriteTln(m.Ne0); break;
                case Mnemonic.tlo: RewriteTlo(); break;
                case Mnemonic.tloa: RewriteTloa(); break;
                case Mnemonic.tlz: RewriteTlz(); break;
                case Mnemonic.tlza: RewriteTlza(); break;
                case Mnemonic.trn: Nop(); break;
                case Mnemonic.tro: RewriteTro(); break;
                case Mnemonic.troa: RewriteTroa(); break;
                case Mnemonic.trne: RewriteTrn(m.Eq0); break;
                case Mnemonic.trnn: RewriteTrn(m.Ne0); break;
                case Mnemonic.trz: RewriteTrz(); break;
                case Mnemonic.trze: RewriteTrz(m.Eq0); break;
                case Mnemonic.trzn: RewriteTrz(m.Ne0); break;
                case Mnemonic.ttcall: RewriteTtcall(); break;
                case Mnemonic.ujen: RewriteUjen(); break;
                case Mnemonic.xct: RewriteXct(); break;
                case Mnemonic.xor: RewriteXor(); break;
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
            var id = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            return id;
        }

        private Identifier Ac_plus_1()
        {
            var iReg = ((RegisterStorage) instr.Operands[0]).Number + 1;
            //$REVIEW: what if iReg > 15?
            var id = binder.EnsureRegister(Registers.Accumulators[iReg]);
            return id;
        }

        /// <summary>
        /// Makes a register pair from [AC,AC+1]
        /// </summary>
        private Identifier AcPair()
        {
            //$REVIEW: is RegHi == 15 valid?
            var regHi = (RegisterStorage) instr.Operands[0];
            var regLo = Registers.Accumulators[(regHi.Number + 1) & 0xF];
            var id = binder.EnsureSequence(word72, regHi, regLo);
            return id;
        }

        private Expression Assign(Expression dst, Expression src)
        {
            if (dst is Identifier id)
            {
                m.Assign(id, src);
                return id;
            }
            else
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                m.Assign(dst, tmp);
                return tmp;
            }
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
                    return m.Mem(word36, Pdp10Architecture.Ptr18(ea.Offset));
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
            case Address addrOp:
                addr = addrOp;
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
            case Address addr:
                return addr;
            }
            throw new NotImplementedException();
        }

        private Constant Imm(int i)
        {
            var imm = (Constant) instr.Operands[i];
            return imm;
        }

        private void Nop()
        {
            iclass = InstrClass.Linear | InstrClass.Padding;
            m.Nop();
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

        // Instruction modes.
        private void Basic(Func<Expression,Expression> fn, FlagGroupStorage? grf = null)
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, fn(src));
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(dst));
            }
        }

        private void Basic(Func<Expression,Expression,Expression> fn, FlagGroupStorage? grf = null)
        {
            Expression dst;
            if (instr.Operands.Length == 2)
            {
                var src = AccessEa(1);
                dst = Ac();
                m.Assign(dst, fn(dst, src));
            }
            else
            {
                var src = AccessEa(0);
                dst = Assign(AccessEa(0), fn(src, src));
            }
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(dst));
            }
        }

        // Immediate mode: use EA directly, rather than dereferencing it.
        private void IMode(Func<Expression, Expression, Expression> fn, FlagGroupStorage? grf = null)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, fn(dst, src));
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(dst));
            }
        }

        // Memory mode: destination is EA

        private void MMode(Func<Expression, Expression> fn, FlagGroupStorage? grf = null)
        {
            var src = Ac();
            var dst = AccessEa(1);
            m.Assign(dst, fn(src));
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(dst));
            }
        }


        private void MMode(Func<Expression, Expression, Expression> fn, FlagGroupStorage? grf = null)
        {
            Expression result;
            if (instr.Operands.Length == 2)
            {
                var left = Ac();
                var right = AccessEa(1);
                if (right is Identifier idResult)
                {
                    result = idResult;
                    m.Assign(result, fn(left, right));
                }
                else
                {
                    result = binder.CreateTemporary(right.DataType);
                    m.Assign(result, fn(left, right));
                    m.Assign(AccessEa(1), result);
                }
            }
            else
            {
                var src = AccessEa(0);
                result = binder.CreateTemporary(src.DataType);
                m.Assign(result, fn(src, src));
                Assign(AccessEa(0), result);
            }

            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(result));
            }
        }

        // Both mode: destination is both EA and AC
        private void BMode(Func<Expression, Expression, Expression> fn, FlagGroupStorage? grf = null)
        {
            var left = Ac();
            var right = AccessEa(1);
            Assign(left, fn(left, right));
            m.Assign(AccessEa(1), left);
            if (grf is not null)
            {
                m.Assign(binder.EnsureFlagGroup(grf), m.Cond(left));
            }
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
        private static readonly IntrinsicProcedure absIntrinsic;
        private static readonly IntrinsicProcedure blkiIntrinsic;
        private static readonly IntrinsicProcedure blkoIntrinsic;
        private static readonly IntrinsicProcedure bltIntrinsic;
        private static readonly IntrinsicProcedure callIntrinsic;
        private static readonly IntrinsicProcedure calliIntrinsic;
        private static readonly IntrinsicProcedure closeIntrinsic;
        private static readonly IntrinsicProcedure coniIntrinsic;
        private static readonly IntrinsicProcedure conoIntrinsic;
        private static readonly IntrinsicProcedure consoIntrinsic;
        private static readonly IntrinsicProcedure conszIntrinsic;
        private static readonly IntrinsicProcedure dataiIntrinsic;
        private static readonly IntrinsicProcedure dataoIntrinsic;
        private static readonly IntrinsicProcedure dpbIntrinsic;
        private static readonly IntrinsicProcedure haltIntrinsic;
        private static readonly IntrinsicProcedure ldbIntrinsic;
        private static readonly IntrinsicProcedure ibpIntrinsic;
        private static readonly IntrinsicProcedure idpbIntrinsic;
        private static readonly IntrinsicProcedure ildbIntrinsic;
        private static readonly IntrinsicProcedure inbufIntrinsic;
        private static readonly IntrinsicProcedure initiIntrinsic;
        private static readonly IntrinsicProcedure jsysIntrinsic;
        private static readonly IntrinsicProcedure lookupIntrinsic;
        private static readonly IntrinsicProcedure lshIntrinsic;
        private static readonly IntrinsicProcedure luuoIntrinsic;
        private static readonly IntrinsicProcedure muuoIntrinsic;
        private static readonly IntrinsicProcedure renameIntrinsic;
        private static readonly IntrinsicProcedure setstsIntrinsic;
        private static readonly IntrinsicProcedure ttcallIntrinsic;
        private static readonly IntrinsicProcedure ujenIntrinsic;
        private static readonly IntrinsicProcedure xctIntrinsic;

        static Pdp10Rewriter()
        {
            C0C1VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.C0 | FlagM.C1 | FlagM.V | FlagM.T), nameof(C0C1VT));
            C1VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.C1 | FlagM.V | FlagM.T), nameof(C1VT));
            VT = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.V | FlagM.T), nameof(VT));
            VTND = new FlagGroupStorage(Registers.Psw, (uint) (FlagM.V | FlagM.T | FlagM.ND), nameof(VTND));

            absIntrinsic = new IntrinsicBuilder("abs", true)
                .Param(word36)
                .Returns(word36);
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
            callIntrinsic = new IntrinsicBuilder("pdp10_call", true)
                .Param(word36)
                .Param(word36)
                .Void();
            calliIntrinsic = new IntrinsicBuilder("pdp10_calli", true)
                .Param(word36)
                .Param(word36)
                .Void();
            closeIntrinsic = new IntrinsicBuilder("pdp10_close", true)
                .Param(word36)
                .Param(word36)
                .Void();
            coniIntrinsic = new IntrinsicBuilder("pdp10_coni", true)
                .Param(word36)
                .Param(word36)
                .Void();
            conoIntrinsic = new IntrinsicBuilder("pdp10_cono", true)
                 .Param(word36)
                 .Param(word36)
                 .Void();
            consoIntrinsic = new IntrinsicBuilder("pdp10_conso", true)
                 .Param(word36)
                 .Param(word36)
                 .Void();
            conszIntrinsic = new IntrinsicBuilder("pdp10_consz", true)
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
                .Param(ptr18)
                .Void();
            haltIntrinsic = new IntrinsicBuilder("pdp10_halt", true)
                .Param(PrimitiveType.Create(Domain.Pointer, 36))
                .Void();
            ibpIntrinsic = new IntrinsicBuilder("pdp10_inc_byte_ptr", true)
                .Param(ptr18)
                .Returns(ptr18);
            idpbIntrinsic = new IntrinsicBuilder("pdp10_inc_ptr_deposit_byte", true)
                .Param(ptr18)
                .Param(word36)
                .Returns(ptr18);
            inbufIntrinsic = new IntrinsicBuilder("pdp10_inbuf", true)
                .Param(word36)
                .Param(word36)
                .Void();
            initiIntrinsic = new IntrinsicBuilder("pdp10_initi", true)
                .Param(word36)
                .Param(word36)
                .Void();
            ildbIntrinsic = new IntrinsicBuilder("pdp10_inc_byte_ptr_and_load", true)
                .Param(ptr18)
                .Returns(word36);

            ldbIntrinsic = new IntrinsicBuilder("pdp10_load_byte", false)
                .Param(word36)
                .Returns(ptr18);
            jsysIntrinsic = new IntrinsicBuilder("pdp10_jsys", true)
                .Param(word36)
                .Param(word36)
                .Void();
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
            muuoIntrinsic = new IntrinsicBuilder("pdp10_muuo", true)
                 .Param(word36)
                 .Param(word36)
                 .Param(word36)
                 .Void();

            renameIntrinsic = new IntrinsicBuilder("pdp10_rename", true)
                .Param(word36)
                .Param(word36)
                .Void();
            setstsIntrinsic = new IntrinsicBuilder("pdp10_setsts", true)
                .Param(word36)
                .Param(word36)
                .Void();
            ttcallIntrinsic = new IntrinsicBuilder("pdp10_ttcall", true)
                .Param(word36)
                .Param(word36)
                .Void();
            ujenIntrinsic = new IntrinsicBuilder("pdp10_ujen", true)
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
