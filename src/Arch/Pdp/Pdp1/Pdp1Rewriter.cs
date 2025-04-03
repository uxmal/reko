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
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Pdp.Pdp1;

public class Pdp1Rewriter : IEnumerable<RtlInstructionCluster>
{
    private static readonly PrimitiveType dt6 = PrimitiveType.CreateWord(6);
    private static readonly PrimitiveType dt12 = PrimitiveType.CreateWord(12);

    private readonly Pdp1Architecture arch;
    private readonly Word18BeImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<Pdp1Instruction> dasm;
    private Pdp1Instruction instr;
    private InstrClass iclass;

    public Pdp1Rewriter(Pdp1Architecture arch, Word18BeImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.dasm = new Pdp1Disassembler(arch, rdr).GetEnumerator();
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
            case Mnemonic.add:   RewriteBinOp(Operator.IAdd, false); break;
            case Mnemonic.add_i: RewriteBinOp(Operator.IAdd, true); break;
            case Mnemonic.and:   RewriteLogical(Operator.And, false); break;
            case Mnemonic.and_i: RewriteLogical(Operator.And, true); break;
            case Mnemonic.cal:   RewriteSideEffect(cal_intrinsic); break;
            case Mnemonic.cbs:   RewriteSideEffect(cbs_intrinsic); break;
            case Mnemonic.cks:   RewriteCks(); break;
            case Mnemonic.cla:   RewriteCla(); break;
            case Mnemonic.clf:   RewriteClf(); break;
            case Mnemonic.cli:   RewriteCli(); break;
            case Mnemonic.cma:   RewriteCma(); break;
            case Mnemonic.dac:   RewriteDeposit(Registers.Acc, false); break;
            case Mnemonic.dac_i: RewriteDeposit(Registers.Acc, true); break;
            case Mnemonic.dap:   RewriteDap(false); break;
            case Mnemonic.dap_i: RewriteDap(true); break;
            case Mnemonic.dio:   RewriteDeposit(Registers.IO, false); break;
            case Mnemonic.dio_i: RewriteDeposit(Registers.IO, true); break;
            case Mnemonic.dip:   RewriteDip(false); break;
            case Mnemonic.dip_i: RewriteDip(true); break;
            case Mnemonic.div:   RewriteDiv(false); break;
            case Mnemonic.div_i: RewriteDiv(true); break;
            case Mnemonic.dzm:   RewriteDzm(false); break;
            case Mnemonic.dzm_i: RewriteDzm(true); break;
            case Mnemonic.esm:   RewriteSideEffect(esm_intrinsic); break;
            case Mnemonic.hlt:   RewriteHlt(); break;
            case Mnemonic.idx:   RewriteIdx(false); break;
            case Mnemonic.idx_i: RewriteIdx(true); break;
            case Mnemonic.ior:   RewriteLogical(Operator.Or, false); break;
            case Mnemonic.ior_i: RewriteLogical(Operator.Or, true); break;
            case Mnemonic.isp:   RewriteIsp(false); break;
            case Mnemonic.isp_i: RewriteIsp(true); break;
            case Mnemonic.jda:   RewriteJda(); break;
            case Mnemonic.jmp:   RewriteJmp(false); break;
            case Mnemonic.jmp_i: RewriteJmp(true); break;
            case Mnemonic.jsp:   RewriteJsp(false); break;
            case Mnemonic.jsp_i: RewriteJsp(true); break;
            case Mnemonic.lac:   RewriteLoad(Registers.Acc, false); break;
            case Mnemonic.lac_i: RewriteLoad(Registers.Acc, true); break;
            case Mnemonic.lap:   RewriteLap(); break;
            case Mnemonic.lat:   RewriteLat(); break;
            case Mnemonic.law:   RewriteLaw(); break;
            case Mnemonic.lio:   RewriteLoad(Registers.IO, false); break;
            case Mnemonic.lio_i: RewriteLoad(Registers.IO, true); break;
            case Mnemonic.lsm:   RewriteSideEffect(lsm_intrinsic); break;
            case Mnemonic.mul:   RewriteMul(false); break;
            case Mnemonic.mul_i: RewriteMul(true); break;
            case Mnemonic.nop:   m.Nop(); break;
            case Mnemonic.ppa:   RewritePunchTape(ppa_intrinsic); break;
            case Mnemonic.ppb:   RewritePunchTape(ppb_intrinsic); break;
            case Mnemonic.ral:   RewriteRotate(CommonOps.Rol, Registers.Acc); break;
            case Mnemonic.rar:   RewriteRotate(CommonOps.Ror, Registers.Acc); break;
            case Mnemonic.rcl:   RewriteRotateCombined(CommonOps.Rol); break;
            case Mnemonic.rcr:   RewriteRotateCombined(CommonOps.Ror); break;
            case Mnemonic.ril:   RewriteRotate(CommonOps.Rol, Registers.IO); break;
            case Mnemonic.rir:   RewriteRotate(CommonOps.Ror, Registers.IO); break;
            case Mnemonic.rpa:   RewriteReadTape(rpa_intrinsic); break;
            case Mnemonic.rpb:   RewriteReadTape(rpb_intrinsic); break;
            case Mnemonic.rrb:   RewriteSideEffect(rrb_intrinsic); break;
            case Mnemonic.sad:   RewriteSad(false); break;
            case Mnemonic.sad_i: RewriteSad(true); break;
            case Mnemonic.sal:   RewriteShift(Operator.Shl, Registers.Acc); break;
            case Mnemonic.sar:   RewriteShift(Operator.Shr, Registers.Acc); break;
            case Mnemonic.sas:   RewriteSas(false); break;
            case Mnemonic.sas_i: RewriteSas(true); break;
            case Mnemonic.scl:   RewriteShiftCombined(Operator.Shl); break;
            case Mnemonic.scr:   RewriteShiftCombined(Operator.Shr); break;
            case Mnemonic.sil:   RewriteShift(Operator.Shl, Registers.IO); break;
            case Mnemonic.sir:   RewriteShift(Operator.Shr, Registers.IO); break;
            case Mnemonic.sma:   RewriteSma(false); break;
            case Mnemonic.sma_i: RewriteSma(true); break;
            case Mnemonic.spa:   RewriteSpa(false, Registers.Acc); break;
            case Mnemonic.spa_i: RewriteSpa(true, Registers.Acc); break;
            case Mnemonic.spi:   RewriteSpa(false, Registers.IO); break;
            case Mnemonic.spi_i: RewriteSpa(true, Registers.IO); break;
            case Mnemonic.stf:   RewriteStf(); break;
            case Mnemonic.sub:   RewriteBinOp(Operator.ISub, false); break;
            case Mnemonic.sub_i: RewriteBinOp(Operator.ISub, true); break;
            case Mnemonic.sza:   RewriteSza(false, Registers.Acc); break;
            case Mnemonic.sza_i: RewriteSza(true, Registers.Acc); break;
            case Mnemonic.szf:   RewriteSzf(false); break;
            case Mnemonic.szf_i: RewriteSzf(true); break;
            case Mnemonic.szo:   RewriteSzo(false, Registers.Acc); break;
            case Mnemonic.szo_i: RewriteSzo(true, Registers.Acc); break;
            case Mnemonic.szs:   RewriteSzs(false); break;
            case Mnemonic.szs_i: RewriteSzs(true); break;
            case Mnemonic.tyi:   RewriteTyi(); break;
            case Mnemonic.tyo:   RewriteTyo(); break;
            case Mnemonic.xct:   RewriteXct(false); break;
            case Mnemonic.xct_i: RewriteXct(true); break;
            case Mnemonic.xor:   RewriteLogical(Operator.Xor, false); break;
            case Mnemonic.xor_i: RewriteLogical(Operator.Xor, true); break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitUnitTest()
    {
        var offset = rdr.Offset;
        rdr.Offset -= 1;
        rdr.TryReadBeUInt18(out uint word);
        var opcodeAsString = Convert.ToString((long) word, 8).PadLeft(6, '0');
        rdr.Offset = offset;
        var instr = dasm.Current;
        arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter(
            "Pdp1Rw", instr, instr.Mnemonic.ToString(), rdr, instr.MnemonicAsString, opcodeAsString);

        iclass = InstrClass.Invalid;
        m.Invalid();
    }

    private Expression EffectiveAddress(bool indirect)
    {
        var tmp = binder.CreateTemporary(PdpTypes.Ptr18);
        var ea = (Expression) instr.Operands[0];
        if (indirect)
        {
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        return ea;
    }

    private void RewriteBinOp(BinaryOperator op, bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var v = binder.EnsureFlagGroup(Registers.V);
        var ea = EffectiveAddress(indirect);
        var tmp = binder.CreateTemporary(PdpTypes.Word18);
        m.Assign(tmp, m.Mem(tmp.DataType, ea));
        m.Assign(acc, m.Bin(op, acc, tmp));
        m.Assign(v, m.Cond(acc));
    }

    private void RewriteBinOpIndirect(BinaryOperator op)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(PdpTypes.Word18);
        var y = (Address) instr.Operands[0];
        var v = binder.EnsureFlagGroup(Registers.V);
        m.Assign(tmp, m.Mem(tmp.DataType, y));
        m.Assign(tmp, m.Mem(tmp.DataType, tmp));    
        m.Assign(acc, m.Bin(op, acc, tmp));
        m.Assign(v, m.Cond(acc));
    }

    private void RewriteCks()
    {
        var io= binder.EnsureRegister(Registers.IO);
        m.Assign(io, m.Fn(cks_intrinsic));
    }

    private void RewriteCla()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, 0);
    }

    private void RewriteClf()
    {
        m.SideEffect(m.Fn(clf_intrinsic, (Expression) instr.Operands[0]));
    }

    private void RewriteCli()
    {
        var io = binder.EnsureRegister(Registers.IO);
        m.Assign(io, 0);
    }

    private void RewriteCma()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Comp(acc));
    }

    private void RewriteDap(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(dt12);
        var tmp2 = binder.CreateTemporary(PdpTypes.Word18);
        m.Assign(tmp, m.Slice(acc, dt12));
        var ea = EffectiveAddress(indirect);
        m.Assign(tmp2, m.Mem(tmp2.DataType, ea));
        m.Assign(tmp2, m.Dpb(tmp2, tmp, 0));
        m.Assign(m.Mem(tmp2.DataType, ea), tmp2);
    }

    private void RewriteDip(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(dt6);
        var tmp2 = binder.CreateTemporary(PdpTypes.Word18);
        m.Assign(tmp, m.Slice(acc, dt12));
        var ea = EffectiveAddress(indirect);
        m.Assign(tmp2, m.Mem(tmp2.DataType, ea));
        m.Assign(tmp2, m.Dpb(tmp2, tmp, 12));
        m.Assign(m.Mem(tmp2.DataType, ea), tmp2);
    }

    private void RewriteDeposit(RegisterStorage register, bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        var id = binder.EnsureRegister(register);
        var tmp = binder.CreateTemporary(PdpTypes.Word18);
        
        if (indirect)
        {
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        m.Assign(m.Mem(id.DataType, ea), id);
    }

    private void RewriteDiv(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var io = binder.EnsureRegister(Registers.IO);
        var divisor = binder.CreateTemporary(PdpTypes.Word18);
        var dividend = binder.EnsureSequence(PdpTypes.Word36, Registers.Acc, Registers.IO);
        var y = (Address) instr.Operands[0];
        m.Assign(divisor, m.Mem(divisor.DataType, y));
        if (indirect)
            m.Assign(divisor, m.Mem(divisor.DataType, divisor));
        m.Assign(acc, m.SDiv(PdpTypes.Word18, dividend, divisor));
        m.Assign(io, m.SMod(PdpTypes.Word18, dividend, divisor));
    }

    private void RewriteDzm(bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        var tmp = binder.CreateTemporary(PdpTypes.Word18);

        if (indirect)
        {
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        m.Assign(m.Mem(PdpTypes.Ptr18, ea), 0);
    }

    private void RewriteHlt()
    {
        m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
    }

    private void RewriteIdx(bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Mem(acc.DataType, ea));
        m.Assign(acc, m.IAdd(acc, 1));
        m.Assign(m.Mem(acc.DataType, ea), acc);
    }

    private void RewriteIsp(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        RewriteIdx(indirect);
        m.Branch(m.Gt0(acc), instr.Address + 2);
    }

    private void RewriteJda()
    {
        var y = (Address) instr.Operands[0];
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(m.Mem(acc.DataType, y), acc);
        var yp1 = y + 1;
        m.Assign(acc, instr.Address);
        m.Goto(yp1);
    }

    private void RewriteJmp(bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        m.Goto(ea);
    }

    private void RewriteJsp(bool indirect)
    {
        var ea = EffectiveAddress(indirect);
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, instr.Address + 1);
        m.Call(ea, 0);
    }

    private void RewriteLoad(RegisterStorage reg, bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        var result = binder.EnsureRegister(reg);
        m.Assign(result, m.Mem(result.DataType, ea));
    }

    private void RewriteLap()
    {
        var pc1 = instr.Address + 1;
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Or(acc, pc1));
    }

    private void RewriteLat()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Fn(lat_intrinsic));
    }

    private void RewriteLaw()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, (Expression) instr.Operands[0]);
    }


    private void RewriteLogical(BinaryOperator op, bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(PdpTypes.Word18);
        var y = (Address) instr.Operands[0];
        m.Assign(tmp, m.Mem(tmp.DataType, y));
        if (indirect)
            m.Assign(tmp, m.Mem(tmp.DataType, tmp));
        m.Assign(acc, m.Bin(op, acc, tmp));
    }

    private void RewriteMul(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(PdpTypes.Word18);
        var product = binder.EnsureSequence(PdpTypes.Word36, Registers.Acc, Registers.IO);
        var y = (Address) instr.Operands[0];
        m.Assign(tmp, m.Mem(tmp.DataType, y));
        if (indirect)
            m.Assign(tmp, m.Mem(tmp.DataType, tmp));
        m.Assign(product, m.IMul(PdpTypes.Word36, acc, tmp));
    }

    private void RewritePunchTape(IntrinsicProcedure intrinsic)
    {
        var io = binder.EnsureRegister(Registers.IO);
        m.SideEffect(m.Fn(intrinsic, io));
    }

    private void RewriteReadTape(IntrinsicProcedure intrinsic)
    {
        var io = binder.EnsureRegister(Registers.IO);
        m.Assign(io, m.Fn(intrinsic));
    }

    private void RewriteRotate(IntrinsicProcedure rotate, RegisterStorage reg)
    {
        var id = binder.EnsureRegister(reg);
        m.Assign(id, m.Fn(rotate, id, (Expression) instr.Operands[0]));
    }

    private void RewriteRotateCombined(IntrinsicProcedure rotate)
    {
        var combo = binder.EnsureSequence(PdpTypes.Word36, Registers.Acc, Registers.IO);
        m.Assign(combo, m.Fn(rotate, combo, (Expression) instr.Operands[0]));
    }

    private void RewriteSad(bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Branch(m.Ne(acc, m.Mem(acc.DataType, ea)), instr.Address + 2);
    }

    private void RewriteSas(bool indirect)
    {
        Expression ea = (Address) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Branch(m.Eq(acc, m.Mem(acc.DataType, ea)), instr.Address + 2);
    }

    private void RewriteShift(BinaryOperator shift, RegisterStorage reg)
    {
        var id = binder.EnsureRegister(reg);
        m.Assign(id, m.Bin(shift, id, (Expression) instr.Operands[0]));
    }

    private void RewriteShiftCombined(BinaryOperator shift)
    {
        var combo = binder.EnsureSequence(PdpTypes.Word36, Registers.Acc, Registers.IO);
        m.Assign(combo, m.Bin(shift, combo, (Expression) instr.Operands[0]));
    }

    private void RewriteSideEffect(IntrinsicProcedure intrinsic)
    {
        m.SideEffect(m.Fn(intrinsic));
    }

    private void RewriteSma(bool indirect)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        Expression cond = indirect
            ? m.Ge0(acc)
            : m.Lt0(acc);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteSpa(bool indirect, RegisterStorage reg)
    {
        var register = binder.EnsureRegister(reg);
        var cond = indirect
            ? m.Lt0(register)
            : m.Ge0(register);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteStf()
    {
        m.SideEffect(m.Fn(stf_intrinsic, (Expression) instr.Operands[0]));
    }

    private void RewriteSza(bool indirect, RegisterStorage reg)
    {
        var register = binder.EnsureRegister(reg);
        var cond = indirect
            ? m.Ne0(register)
            : m.Eq0(register);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteSzf(bool indirect)
    {
        var gss = m.Fn(get_program_flag, (Expression) instr.Operands[0]);
        Expression cond = indirect
            ? gss
            : m.Not(gss);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteSzo(bool indirect, RegisterStorage reg)
    {
        var v = binder.EnsureFlagGroup(Registers.V);
        var cond = indirect
            ? m.Test(ConditionCode.OV, v)
            : m.Test(ConditionCode.NO, v);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteSzs(bool indirect)
    {
        var gss = m.Fn(get_sense_switch, (Expression) instr.Operands[0]);
        Expression cond = indirect
            ? gss
            : m.Not(gss);
        m.Branch(cond, instr.Address + 2);
    }

    private void RewriteTyi()
    {
        var io = binder.EnsureRegister(Registers.IO);
        m.Assign(io, m.Fn(tyi_intrinsic));
    }

    private void RewriteTyo()
    {
        var io = binder.EnsureRegister(Registers.IO);
        m.SideEffect(m.Fn(tyo_intrinsic, io));
    }

    private void RewriteXct(bool indirect)
    {
        Expression ea = (Expression) instr.Operands[0];
        if (indirect)
        {
            var tmp = binder.CreateTemporary(PdpTypes.Word18);
            m.Assign(tmp, m.Mem(tmp.DataType, ea));
            ea = tmp;
        }
        m.SideEffect(m.Fn(xct_intrinsic, ea));
    }

    private static readonly IntrinsicProcedure cal_intrinsic = IntrinsicBuilder.SideEffect("__call_subroutine")
        .Void();
    private static readonly IntrinsicProcedure cbs_intrinsic = IntrinsicBuilder.SideEffect("__clear_sequence_break_system")
        .Void();
    private static readonly IntrinsicProcedure cks_intrinsic = IntrinsicBuilder.SideEffect("__check_IO_status")
        .Returns(PdpTypes.Word18);
    private static readonly IntrinsicProcedure clf_intrinsic = IntrinsicBuilder.SideEffect("__clear_program_flag")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure esm_intrinsic = IntrinsicBuilder.SideEffect("__enter_sequence_break_mode")
        .Void();
    private static readonly IntrinsicProcedure lat_intrinsic = new IntrinsicBuilder("__load_test_word", true)
        .Returns(PdpTypes.Word18);
    private static readonly IntrinsicProcedure lsm_intrinsic = IntrinsicBuilder.SideEffect("__leave_sequence_break_mode")
        .Void();
    private static readonly IntrinsicProcedure ppa_intrinsic = IntrinsicBuilder.SideEffect("__punch_performated_tape_alphanumeric")
        .Param(PdpTypes.Word18)
        .Void();
    private static readonly IntrinsicProcedure ppb_intrinsic = IntrinsicBuilder.SideEffect("__punch_performated_tape_binary")
        .Param(PdpTypes.Word18)
        .Void();
    private static readonly IntrinsicProcedure rpa_intrinsic = IntrinsicBuilder.SideEffect("__read_performated_tape_alphanumeric")
        .Returns(PdpTypes.Word18);
    private static readonly IntrinsicProcedure rpb_intrinsic = IntrinsicBuilder.SideEffect("__read_performated_tape_binary")
        .Returns(PdpTypes.Word18);
    private static readonly IntrinsicProcedure rrb_intrinsic = IntrinsicBuilder.SideEffect("__read_reader_buffer")
        .Void();
    private static readonly IntrinsicProcedure stf_intrinsic = IntrinsicBuilder.SideEffect("__set_program_flag")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure tyi_intrinsic = IntrinsicBuilder.SideEffect("__type_in")
        .Returns(PdpTypes.Word18);
    private static readonly IntrinsicProcedure tyo_intrinsic = IntrinsicBuilder.SideEffect("__type_out")
        .Param(PdpTypes.Word18)
        .Void();
    private static readonly IntrinsicProcedure xct_intrinsic = IntrinsicBuilder.SideEffect("__execute")
        .Param(PdpTypes.Word18)
        .Void();

    private static readonly IntrinsicProcedure get_program_flag = IntrinsicBuilder.SideEffect("__get_sense_switch")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Bool);
    private static readonly IntrinsicProcedure get_sense_switch = IntrinsicBuilder.SideEffect("__get_sense_switch")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Bool);
}
