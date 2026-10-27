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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Dec.Linc;

public class LincRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly LincArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<LincInstruction> dasm;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private InstrClass iclass;
    private int cWords;
    private bool treatLowAddressesAsRegs;

    public LincRewriter(LincArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new LincDisassembler(arch, rdr).GetEnumerator();
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.treatLowAddressesAsRegs = true;
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            this.iclass = instr.InstructionClass;
            this.cWords = instr.Length;
            switch (instr.Mnemonic)
            {
            default:
                EmitUnitTest(instr);
                break;
            case Mnemonic.Invalid:
                m.Invalid();
                break;
            case Mnemonic.ada: RewriteAdd(instr); break;
            case Mnemonic.add: RewriteAdd(instr); break;
            case Mnemonic.adm: RewriteAdm(instr); break;
            case Mnemonic.apo: RewriteApo(instr); break;
            case Mnemonic.atr: RewriteAtr(instr); break;
            case Mnemonic.axo: RewriteAxo(instr); break;
            case Mnemonic.aze: RewriteAze(instr); break;
            case Mnemonic.bcl: RewriteBcl(instr); break;
            case Mnemonic.bco: RewriteBco(instr); break;
            case Mnemonic.bse: RewriteBse(instr); break;
            case Mnemonic.chk: RewriteChk(instr); break;
            case Mnemonic.clr: RewriteClr(instr); break;
            case Mnemonic.com: RewriteCom(instr); break;
            case Mnemonic.dis: RewriteDis(instr); break;
            case Mnemonic.djr: RewriteDjr(instr); break;
            case Mnemonic.dsc: RewriteDsc(instr); break;
            case Mnemonic.flo: RewriteFlo(instr); break;
            case Mnemonic.halt: RewriteHalt(instr); break;
            case Mnemonic.ibz: RewriteIbz(instr); break;
            case Mnemonic.iof: RewriteIof(instr); break;
            case Mnemonic.ion: RewriteIon(instr); break;
            case Mnemonic.jmp: RewriteJmp(instr); break;
            case Mnemonic.kst: RewriteKst(instr); break;
            case Mnemonic.lam: RewriteLam(instr); break;
            case Mnemonic.lda: RewriteLda(instr); break;
            case Mnemonic.ldf: RewriteLdf(instr); break;
            case Mnemonic.ldh: RewriteLdh(instr); break;
            case Mnemonic.lif: RewriteLif(instr); break;
            case Mnemonic.lmr: RewriteLmr(instr); break;
            case Mnemonic.lsw: RewriteLsw(instr); break;
            case Mnemonic.lze: RewriteLze(instr); break;
            case Mnemonic.mtb: RewriteMtb(instr); break;
            case Mnemonic.mul: RewriteMul(instr); break;
            case Mnemonic.nop: RewriteNop(instr); break;
            case Mnemonic.pdp: RewritePdp(instr); break;
            case Mnemonic.qac: RewriteQac(instr); break;
            case Mnemonic.qlz: RewriteQlz(instr); break;
            case Mnemonic.rcg: RewriteRcg(instr); break;
            case Mnemonic.rdc: RewriteRdc(instr); break;
            case Mnemonic.rde: RewriteRde(instr); break;
            case Mnemonic.rdf: RewriteRdf(instr); break;
            case Mnemonic.rib: RewriteRib(instr); break;
            case Mnemonic.rif: RewriteRif(instr); break;
            case Mnemonic.rmf: RewriteRmf(instr); break;
            case Mnemonic.rol: RewriteRol(instr); break;
            case Mnemonic.ror: RewriteRor(instr); break;
            case Mnemonic.rsw: RewriteRsw(instr); break;
            case Mnemonic.rta: RewriteRta(instr); break;
            case Mnemonic.sae: RewriteSae(instr); break;
            case Mnemonic.sam: RewriteSam(instr); break;
            case Mnemonic.scr: RewriteScr(instr); break;
            case Mnemonic.set: RewriteSet(instr); break;
            case Mnemonic.shd: RewriteShd(instr); break;
            case Mnemonic.skp: RewriteSkp(instr); break;
            case Mnemonic.sns: RewriteSns(instr); break;
            case Mnemonic.sro: RewriteSro(instr); break;
            case Mnemonic.sta: RewriteSta(instr); break;
            case Mnemonic.stc: RewriteStc(instr); break;
            case Mnemonic.std: RewriteStd(instr); break;
            case Mnemonic.sth: RewriteSth(instr); break;
            case Mnemonic.sxl: RewriteSxl(instr); break;
            case Mnemonic.tac: RewriteTac(instr); break;
            case Mnemonic.tma: RewriteTma(instr); break;
            case Mnemonic.twc: RewriteTwc(instr); break;
            case Mnemonic.wcg: RewriteWcg(instr); break;
            case Mnemonic.wrc: RewriteWrc(instr); break;
            case Mnemonic.wri: RewriteWri(instr); break;
            case Mnemonic.xoa: RewriteXoa(instr); break;
            case Mnemonic.xsk: RewriteXsk(instr); break;
            }

            yield return new RtlInstructionCluster(instr.Address, this.cWords, rtls.ToArray())
            {
                Class = iclass
            };
        }
    }

    private void EmitUnitTest(LincInstruction instr)
    {
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        testSvc?.ReportMissingRewriter("LincRw", instr, instr.MnemonicAsString, rdr, "", "");
        m.Invalid();
        iclass = InstrClass.Invalid;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Identifier Ac() => binder.EnsureRegister(Registers.Accumulator);

    private Identifier AlphaRegister(LincInstruction instr)
    {
        var mem = (MemoryOperand) instr.Operands[0];
        var reg = Registers.IndexRegisters[mem.Offset & 0xF];
        return binder.EnsureRegister(reg);
    }

    private void EmitInvalid()
    {
        m.Invalid();
        iclass = InstrClass.Invalid;
    }

    private Expression? RewriteOp(LincInstruction instr, bool effectiveAddress = false)
    {
        if (instr.Operands.Length == 0)
        {
            return RewriteNextLiteral(instr.Indirect, effectiveAddress);
        }
        var op = instr.Operands[0];
        return RewriteOp(op, instr.Indirect, effectiveAddress);
    }

    private Expression RewriteOp(
        MachineOperand op,
        bool incrementIndex,
        bool effectiveAddress)
    {
        switch (op)
        {
        case Constant c:
            return c;
        case MemoryOperand mop:
            Expression ea;
            if (treatLowAddressesAsRegs && mop.Offset <= 0xF)
            {
                var reg = arch.RegisterBank.GetRegisterByDomain((StorageDomain) mop.Offset);
                var id = binder.EnsureRegister(reg);
                if (incrementIndex)
                    m.Assign(id, m.IAdd(id, 1));
                ea = id;
            }
            else
            {
                ea = LincArchitecture.Ptr12(mop.Offset);
            }
            if (effectiveAddress)
                return ea;
            return m.Mem(PdpTypes.Word12, ea);
        }
        throw new NotImplementedException($"Operand type {op.GetType().Name} not implemented for rewriting.");
    }

    private Expression? RewriteNextLiteral(bool indirect, bool effectiveAddress)
    {
        // We expect another instruction to follow, which will be a literal.
        // If the next instruction is not a literal, we have a problem.
        if (!dasm.MoveNext())
            return null;
        var instr = dasm.Current;
        if (instr.Mnemonic != Mnemonic.Literal)
            return null;
        ++cWords;
        Constant c = (Constant) instr.Operands[0];
        if (indirect)
        {
            if (effectiveAddress)
                return instr.Address + 1;
            else
                return c;
        }
        // Bits 1..11 (skipping the MSB) are used as the effective address.
        Expression e = LincArchitecture.Ptr12(c.ToUInt16() & 0x7FFu);
        if (!effectiveAddress)
            e = m.Mem(PdpTypes.Word12, e);
        return e;
    }

    private void RewriteAdd(LincInstruction instr)
    {
        var ac = Ac();
        var addend = RewriteOp(instr);
        if (addend is null)
        {
            EmitInvalid();
            return;
        }
        m.Assign(ac, m.IAdd(ac, addend));
    }

    private void RewriteAdm(LincInstruction instr)
    {
        var ac = Ac();
        var tmp = binder.CreateTemporary(PdpTypes.Word12);
        var addend = RewriteOp(instr);
        if (addend is null)
        {
            m.Invalid();
            return;
        }
        m.Assign(tmp, addend);
        m.Assign(ac, m.IAdd(ac, tmp));
        m.Assign(addend, tmp);
    }

    private void RewriteApo(LincInstruction instr)
    {
        var ac = Ac();
        RewriteSkip(instr, m.Ge0(ac));
    }

    private void RewriteAtr(LincInstruction _)
    {
        var ac = Ac();
        m.SideEffect(m.Fn(atr_intrinsic, ac));
    }

    private void RewriteAxo(LincInstruction _)
    {
        var ac = Ac();
        m.SideEffect(m.Fn(axo_intrinsic, ac));
    }

    private void RewriteAze(LincInstruction instr)
    {
        var ac = Ac();
        RewriteSkip(instr, m.Eq0(ac));
    }

    private void RewriteBcl(LincInstruction instr)
    {
        var ac = Ac();
        var exp = RewriteOp(instr);
        if (exp is null)
        {
            EmitInvalid();
            return;
        }    
        m.Assign(ac, m.And(ac, m.Comp(exp)));
    }

    private void RewriteBco(LincInstruction instr)
    {
        var ac = Ac();
        var exp = RewriteOp(instr);
        if (exp is null)
        {
            EmitInvalid();
            return;
        }
        m.Assign(ac, m.Xor(ac, exp));
    }

    private void RewriteBse(LincInstruction instr)
    {
        var ac = Ac();
        var exp = RewriteOp(instr);
        if (exp is null)
        {
            EmitInvalid();
            return;
        }
        m.Assign(ac, m.Or(ac, exp));
    }

    private void RewriteChk(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            chk_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteClr(LincInstruction _)
    {
        var ac = Ac();
        var mq = binder.EnsureRegister(Registers.Mq);
        var l = binder.EnsureFlagGroup(Registers.L);

        m.Assign(ac, 0);
        m.Assign(mq, 0);
        m.Assign(l, 0);
    }

    private void RewriteCom(LincInstruction _)
    {
        var ac = Ac();
        m.Assign(ac, m.Comp(ac));
    }

    private void RewriteDis(LincInstruction instr)
    {
        var vert = Ac();
        var horz = RewriteOp(instr);
        if (horz is null)
        {
            EmitInvalid();
            return;
        }    
        m.SideEffect(m.Fn(
            dis_intrinsic,
            horz,
            vert));
    }

    private void RewriteDjr(LincInstruction _)
    {
        m.SideEffect(m.Fn(djr_intrinsic));
    }

    private void RewriteDsc(LincInstruction instr)
    {
        var vert = Ac();
        var horz = binder.EnsureRegister(Registers.IndexRegisters[1]);
        var pattern = RewriteOp(instr);
        if (pattern is null)
        {
            EmitInvalid();
            return;
        }
        m.SideEffect(m.Fn(dsc_intrinsic, horz, vert, pattern));
        m.Assign(horz, m.IAdd(horz, 8));
    }

    private void RewriteFlo(LincInstruction instr)
    {
        var flow = binder.EnsureFlagGroup(Registers.Flow);
        RewriteSkip(instr, m.Test(ConditionCode.OV, flow));
    }

    private void RewriteHalt(LincInstruction _)
    {
        m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
    }

    private void RewriteIbz(LincInstruction instr)
    {
        RewriteSkip(instr, m.Fn(ibz_intrinsic));
    }

    private void RewriteIof(LincInstruction _)
    {
        m.SideEffect(m.Fn(iof_intrinsic));
    }

    private void RewriteIon(LincInstruction _)
    {
        m.SideEffect(m.Fn(ion_intrinsic));
    }

    private void RewriteJmp(LincInstruction instr)
    {
        var mem = (MemoryOperand) instr.Operands[0];
        m.Goto(LincArchitecture.Ptr12(mem.Offset));
    }

    private void RewriteKst(LincInstruction instr)
    {
        RewriteSkip(instr, m.Fn(kst_intrinsic));
    }

    private void RewriteLam(LincInstruction instr)
    {
        var ac = Ac();
        var exp = RewriteOp(instr);
        if (exp is null)
        {
            EmitInvalid();
            return;
        }
        var l = binder.EnsureFlagGroup(Registers.L);
        m.Assign(ac, m.IAddC(ac, exp, l));
        m.Assign(l, m.Cond(l.DataType, ac));
    }

    private void RewriteLda(LincInstruction instr)
    {
        var ea = RewriteOp(instr);
        if (ea is null)
        {
            EmitInvalid();
            return;
        }
        var ac = Ac();
        m.Assign(ac, m.Mem(PdpTypes.Word12, ea));
    }

    private void RewriteLdf(LincInstruction instr)
    {
        var n = RewriteOp(instr);
        if (n is null)
        {
            EmitInvalid();
            return;
        }
        m.SideEffect(m.Fn(ldf_intrinsic, n));
    }

    private void RewriteLdh(LincInstruction instr)
    {
        var n = RewriteOp(instr);
        if (n is null)
        {
            EmitInvalid();
            return;
        }
        Expression src;
        var ac = Ac();
        if (n is MemoryAccess mem)
        {
            src = m.Fn(ldh_intrinsic, n);
            src = m.ExtendZ(src, ac.DataType);
        }
        else if (n is Constant c)
        {
            var v = c.ToUInt16() >> 6;
            src = Constant.Create(PdpTypes.Word12, v & 0x3F);
        }
        else
        {
            throw new NotImplementedException(n.GetType().Name);
        }
        m.Assign(ac, src);
    }

    private void RewriteLif(LincInstruction instr)
    {
        m.SideEffect(m.Fn(lif_intrinsic));
    }

    private void RewriteLmr(LincInstruction instr)
    {
        m.SideEffect(m.Fn(lmr_intrinsic));
    }

    private void RewriteLsw(LincInstruction instr)
    {
        var ac = Ac();
        m.Assign(ac, m.Fn(lsw_intrinsic));
    }

    private void RewriteLze(LincInstruction instr)
    {
        var l = binder.EnsureFlagGroup(Registers.L);
        RewriteSkip(instr, m.Eq0(l));
    }

    private void RewriteMtb(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            mtb_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteMul(LincInstruction instr)
    {
        var ac = Ac();
        var mul = RewriteOp(instr);
        if (mul is null)
        {
            EmitInvalid();
            return;
        }
        var result = binder.EnsureSequence(PdpTypes.Word24, Registers.Accumulator, Registers.Mq);
        m.Assign(result, m.Bin(Operator.IMul, result.DataType, ac, mul));
    }

    private void RewriteNop(LincInstruction instr)
    {
        m.Nop();
    }

    private void RewritePdp(LincInstruction instr)
    {
        m.SideEffect(m.Fn(pdp_intrinsic));
    }

    private void RewriteQac(LincInstruction instr)
    {
        var ac = Ac();
        var mq = binder.EnsureRegister(Registers.Mq);
        m.Assign(ac, m.ExtendZ(m.Slice(mq, 1, 11), ac.DataType));
    }

    private void RewriteQlz(LincInstruction instr)
    {
        var mq = binder.EnsureRegister(Registers.Mq);
        RewriteSkip(instr, m.Eq0(m.And(mq, 1)));
    }

    private void RewriteRcg(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            rcg_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteRdc(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            rdc_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteRde(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            rde_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteRdf(LincInstruction instr)
    {
        var tmp = binder.CreateTemporary(PdpTypes.Word3);
        m.Assign(tmp, m.Fn(rdf_intrinsic));
        var ac = Ac();
        m.Assign(ac, m.Or(ac, m.Dpb(m.Zero(ac.DataType), tmp, 3)));
    }

    private void RewriteRib(LincInstruction instr)
    {
        var ac = Ac();
        m.Assign(ac, m.Fn(rib_intrinsic));
    }

    private void RewriteRif(LincInstruction instr)
    {
        var ac = Ac();
        m.Assign(ac, m.Fn(rif_intrinsic));
    }

    private void RewriteRmf(LincInstruction instr)
    {
        m.SideEffect(m.Fn(rmf_intrinsic));
    }

    private void RewriteRol(LincInstruction instr)
    {
        var ac = Ac();
        var sh = (Constant) instr.Operands[0];
        if (instr.Indirect)
        {
            var l = binder.EnsureFlagGroup(Registers.L);
            var t = binder.CreateTemporary(ac.DataType);
            var rolc = CommonOps.RolC.MakeInstance(t.DataType, sh.DataType);
            m.Assign(t, ac);
            m.Assign(ac, m.Fn(rolc, t, sh, l));
            m.Assign(l, m.Cond(l.DataType, m.Ne0(m.And(t, 0x800))));
        }
        else
        {
            m.Assign(ac, m.Fn(CommonOps.Rol, ac, sh));
        }
    }

    private void RewriteRor(LincInstruction instr)
    {
        var ac = Ac();
        var sh = (Constant) instr.Operands[0];
        if (instr.Indirect)
        {
            var l = binder.EnsureFlagGroup(Registers.L);
            var t = binder.CreateTemporary(ac.DataType);
            var rorc = CommonOps.RorC.MakeInstance(t.DataType, sh.DataType);
            m.Assign(t, ac);
            m.Assign(ac, m.Fn(rorc, t, sh, l));
            m.Assign(l, m.Cond(l.DataType, m.Ne0(m.And(t, 1))));
        }
        else
        {
            m.Assign(ac, m.Fn(CommonOps.Ror, ac, sh));
        }
    }

    private void RewriteRsw(LincInstruction instr)
    {
        var ac = Ac();
        m.Assign(ac, m.Fn(rsw_intrinsic));

    }

    private void RewriteRta(LincInstruction instr)
    {
        var tmp = binder.CreateTemporary(PdpTypes.Word6);
        var ac = Ac();
        m.Assign(tmp, m.Fn(rta_intrinsic));
        m.Assign(ac, m.Dpb(ac, tmp, 0));
    }

    private void RewriteSae(LincInstruction instr)
    {
        var ac = Ac();
        var y = RewriteOp(instr);
        if (y is null)
        {
            EmitInvalid();
            return;
        }
        RewriteSkip(instr, m.Eq(ac, y));
    }

    private void RewriteSam(LincInstruction instr)
    {
        var ac = Ac();
        var channel = (Constant) instr.Operands[0];
        m.Assign(ac, m.Fn(sam_intrinsic, channel));
    }

    private void RewriteScr(LincInstruction instr)
    {
        var mq = binder.EnsureRegister(Registers.Mq);
        var sh = (Constant) instr.Operands[0];
        var seq = binder.EnsureSequence(PdpTypes.Word24, Registers.Accumulator, Registers.Mq);
        m.Assign(seq, m.Sar(seq, sh));
        if (instr.Indirect)
        {
            var l = binder.EnsureFlagGroup(Registers.L);
            m.Assign(l, m.Ne0(m.And(mq, 0x800)));
        }
    }

    private void RewriteSet(LincInstruction instr)
    {
        var dst = AlphaRegister(instr);
        var src = this.RewriteNextLiteral(instr.Indirect, false);
        if (src is null)
        {
            EmitInvalid();
            return;
        }
        m.Assign(dst, src);
    }

    private void RewriteShd(LincInstruction instr)
    {
        var n = RewriteOp(instr);
        if (n is null)
        {
            EmitInvalid();
            return;
        }
        Expression src;
        var ac = Ac();
        if (n is MemoryAccess mem)
        {
            src = m.Fn(ldh_intrinsic, n);
        }
        else if (n is Constant c)
        {
            var v = c.ToUInt16();
            src = Constant.Create(PdpTypes.Word6, v & 0x3F);
        }
        else
        {
            throw new NotImplementedException(n.GetType().Name);
        }
        var t = binder.CreateTemporary(PdpTypes.Word6);
        m.Assign(t, m.Slice(ac, t.DataType));
        RewriteSkip(instr.Address, false, m.Ne(t, src));
    }

    private void RewriteSkp(LincInstruction instr)
    {
        m.Goto(instr.Address + 2);
    }

    private void RewriteSns(LincInstruction instr)
    {
        RewriteSkip(instr, m.Fn(sns_intrinsic, (Constant) instr.Operands[0]));  
    }

    private void RewriteSro(LincInstruction instr)
    {
        var y = RewriteOp(instr);
        if (y is null)
        {
            EmitInvalid();
            return;
        }
        var t = binder.CreateTemporary(PrimitiveType.Bool);
        m.Assign(t, m.Eq0(m.And(y, 1)));
        RewriteSkip(instr, t);
    }

    private void RewriteSta(LincInstruction instr)
    {
        var ea = RewriteOp(instr, true);
        if (ea is null)
        {
            EmitInvalid();
            return;
        }
        var ac = Ac();
        m.Assign(m.Mem(PdpTypes.Word12, ea), ac);
    }

    private void RewriteStc(LincInstruction instr)
    {
        var ac = Ac();
        var y = RewriteOp(instr, true);
        if (y is null)
        {
            EmitInvalid();
            return;
        }
        m.Assign(m.Mem(PdpTypes.Word12, y), ac);
        m.Assign(ac, 0);
    }

    private void RewriteStd(LincInstruction instr)
    {
        RewriteSkip(instr, m.Fn(std_intrinsic));
    }

    private void RewriteSth(LincInstruction instr)
    {
        var n = RewriteOp(instr);
        if (n is null)
        {
            EmitInvalid();
            return;
        }
        Expression dst;
        var ac = Ac();
        var t = binder.CreateTemporary(PdpTypes.Word6);
        m.Assign(t, m.Slice(ac, t.DataType));
        if (n is MemoryAccess mem)
        {
            dst = m.Fn(sth_intrinsic, n, t);
            m.SideEffect(dst);
        }
        else
        {
            EmitUnitTest(instr);
            return;
        }
    }

    private void RewriteSxl(LincInstruction instr)
    {
        var n = (Constant) instr.Operands[0];
        RewriteSkip(instr, m.Fn(sxl_intrinsic, n));
    }

    private void RewriteTac(LincInstruction instr)
    {
        var tac = binder.EnsureRegister(Registers.Tac);
        var ac = Ac();
        m.Assign(ac, tac);
    }

    private void RewriteTma(LincInstruction instr)
    {
        var ac = Ac();  
        m.SideEffect(m.Fn(tma_intrinsic, ac));
    }

    private void RewriteTwc(LincInstruction instr)
    {
        RewriteSkip(instr, m.Fn(twc_intrinsic));
    }

    private void RewriteWcg(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            wcg_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);

    }

    private void RewriteWrc(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            wrc_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteWri(LincInstruction instr)
    {
        var ac = Ac();
        var tac = binder.EnsureRegister(Registers.Tac);
        var unit = (Constant) instr.Operands[0];
        var tape = (TapeBlockOperator) instr.Operands[1];
        m.Assign(ac, m.Fn(
            wri_intrinsic,
            m.Bool(instr.Indirect),
            m.Const(PdpTypes.Word12, unit.ToInt32()),
            m.Const(PdpTypes.Word12, tape.Mblk),
            m.Const(PdpTypes.Word12, tape.Tblk)));
        m.Assign(tac, ac);
    }

    private void RewriteXoa(LincInstruction instr)
    {
        var ac = Ac();
        m.Assign(ac, m.Fn(xoa_intrinsic));
    }

    private void RewriteXsk(LincInstruction instr)
    {
        var y = AlphaRegister(instr);
        if (instr.Indirect)
        {
            m.Assign(y, m.IAddS(y, 1));
        }
        RewriteSkip(instr.Address, false, m.Eq(y, 0x3FF));
    }

    private void RewriteSkip(LincInstruction instr, Expression condition)
    {
        RewriteSkip(instr.Address, instr.Indirect, condition);
    }

    private void RewriteSkip(Address addrInstr, bool invertTest, Expression condition)
    {
        if (invertTest)
        {
            condition = condition.Invert();
        }
        m.Branch(condition, addrInstr + (cWords + 1));
    }

    private static readonly IntrinsicProcedure atr_intrinsic = IntrinsicBuilder.SideEffect("__transfer_to_relays")
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure axo_intrinsic = IntrinsicBuilder.SideEffect("__write_to_xob")
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure chk_intrinsic = IntrinsicBuilder.SideEffect("__check_one_tape_block")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure dis_intrinsic = IntrinsicBuilder.SideEffect("__display")
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure djr_intrinsic = IntrinsicBuilder.SideEffect("__disable_jump_return")
        .Void();

    private static readonly IntrinsicProcedure dsc_intrinsic = IntrinsicBuilder.SideEffect("__display_character")
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure ibz_intrinsic = IntrinsicBuilder.SideEffect("__is_tape_in_interblock_zone")
        .Returns(PrimitiveType.Bool);
    private static readonly IntrinsicProcedure iof_intrinsic = IntrinsicBuilder.SideEffect("__interrupts_off")
        .Void();
    private static readonly IntrinsicProcedure ion_intrinsic = IntrinsicBuilder.SideEffect("__interrupts_on")
        .Void();

    private static readonly IntrinsicProcedure kst_intrinsic = IntrinsicBuilder.SideEffect("__is_key_struck")
        .Returns(PrimitiveType.Bool);

    private static readonly IntrinsicProcedure ldf_intrinsic = IntrinsicBuilder.SideEffect("__load_data_field_register")
        .Param(PdpTypes.Word12)
        .Void();
    private static readonly IntrinsicProcedure ldh_intrinsic = new IntrinsicBuilder("load_halfword", false)
        .Param(PdpTypes.Ptr12)
        .Returns(PdpTypes.Word6);
    private static readonly IntrinsicProcedure lif_intrinsic = IntrinsicBuilder.SideEffect("__load_linc_instruction_field_buffer")
        .Void();
    private static readonly IntrinsicProcedure lmr_intrinsic = IntrinsicBuilder.SideEffect("__load_maintenance_register")
        .Void();
    private static readonly IntrinsicProcedure lsw_intrinsic = IntrinsicBuilder.SideEffect("__load_left_switches")
        .Returns(PdpTypes.Word12);

    private static readonly IntrinsicProcedure mtb_intrinsic = IntrinsicBuilder.SideEffect("__move_toward_block")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Void();

    private static readonly IntrinsicProcedure pdp_intrinsic = IntrinsicBuilder.SideEffect("__switch_to_PDP8_mode")
        .Void();

    private static readonly IntrinsicProcedure rcg_intrinsic = IntrinsicBuilder.SideEffect("__read_and_check_consecutive")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure rdc_intrinsic = IntrinsicBuilder.SideEffect("__check_one_tape_block")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure rde_intrinsic = IntrinsicBuilder.SideEffect("__check_one_tape_block")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure rdf_intrinsic = IntrinsicBuilder.SideEffect("__read_data_field")
        .Returns(PdpTypes.Word3);
    private static readonly IntrinsicProcedure rib_intrinsic = IntrinsicBuilder.SideEffect("__read_interrupt_buffer")
        .Returns(PdpTypes.Word3);
    private static readonly IntrinsicProcedure rif_intrinsic = IntrinsicBuilder.SideEffect("__read_instruction_field")
        .Returns(PdpTypes.Word3);
    private static readonly IntrinsicProcedure rmf_intrinsic = IntrinsicBuilder.SideEffect("__restore_memory_field")
        .Void();
    private static readonly IntrinsicProcedure rsw_intrinsic = IntrinsicBuilder.SideEffect("__load_right_switches")
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure rta_intrinsic = IntrinsicBuilder.SideEffect("__load_relays")
        .Returns(PdpTypes.Word6);

    private static readonly IntrinsicProcedure sam_intrinsic = IntrinsicBuilder.SideEffect("__sample_channel")
        .Param(PrimitiveType.Byte)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure sns_intrinsic = IntrinsicBuilder.SideEffect("__sense_switch")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Bool);
    private static readonly IntrinsicProcedure std_intrinsic = IntrinsicBuilder.SideEffect("__is_tape_done")
        .Returns(PrimitiveType.Bool);
    private static readonly IntrinsicProcedure sth_intrinsic = new IntrinsicBuilder("__store_halfword", false)
        .Param(PdpTypes.Ptr12)
        .Param(PdpTypes.Word6)
        .Void();
    private static readonly IntrinsicProcedure sxl_intrinsic = IntrinsicBuilder.SideEffect("__is_external_level_high")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Bool);

    private static readonly IntrinsicProcedure tma_intrinsic = IntrinsicBuilder.SideEffect("__write_tma_setup_register")
        .Param(PdpTypes.Word12)
        .Void();
    private static readonly IntrinsicProcedure twc_intrinsic = IntrinsicBuilder.SideEffect("__is_tape_word_complete")
        .Returns(PrimitiveType.Bool);

    private static readonly IntrinsicProcedure wcg_intrinsic = IntrinsicBuilder.SideEffect("__write_and_check_consecutive")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure wrc_intrinsic = IntrinsicBuilder.SideEffect("__write_and_check")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);
    private static readonly IntrinsicProcedure wri_intrinsic = IntrinsicBuilder.SideEffect("__write_tape")
        .Param(PrimitiveType.Bool)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Param(PdpTypes.Word12)
        .Returns(PdpTypes.Word12);

    private static readonly IntrinsicProcedure xoa_intrinsic = IntrinsicBuilder.SideEffect("__read_from_xob")
        .Returns(PdpTypes.Word12);
}
