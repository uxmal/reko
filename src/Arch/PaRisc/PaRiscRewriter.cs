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
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly PaRiscArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<PaRiscInstruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> instrs;
        private PaRiscInstruction instr;
        private InstrClass iclass;

        public PaRiscRewriter(PaRiscArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new PaRiscDisassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.instr = null!;
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
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid: m.Invalid(); break;
                case Mnemonic.add: RewriteAdd(true); break;
                case Mnemonic.add_c: RewriteAdd_c(); break;
                case Mnemonic.add_l: RewriteAdd(false); break;
                case Mnemonic.addb: RewriteAddb(); break;
                case Mnemonic.addi: RewriteAddi(false); break;
                case Mnemonic.addi_tc: RewriteAddi(true); break;
                case Mnemonic.addib: RewriteAddb(); break;
                case Mnemonic.addil: RewriteAddi(false); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.andcm: RewriteLogical((a, b) => m.And(a, m.Comp(b))); break;
                case Mnemonic.bb: RewriteBb(); break;
                case Mnemonic.b_l: RewriteBranch(); break;
                case Mnemonic.be: RewriteBe(false); break;
                case Mnemonic.be_l: RewriteBe(true); break;
                case Mnemonic.bv: RewriteBv(); break;
                case Mnemonic.cmpb: RewriteCmpb(0, 1); break;
                case Mnemonic.cmpclr: RewriteCmpclr(0, 1); break;
                case Mnemonic.cmpib: RewriteCmpb(1, 0); break;
                case Mnemonic.cmpiclr: RewriteCmpclr(1, 0); break;
                case Mnemonic.@break: RewriteBreak(); break;
                case Mnemonic.depw: RewriteDepw(); break;
                case Mnemonic.depwi: RewriteDepwi(); break;
                case Mnemonic.diag: RewriteDiag(); break;
                case Mnemonic.ds: RewriteDs(); break;
                case Mnemonic.extrw: RewriteExtrw(); break;
                case Mnemonic.fadd: RewriteFpArithmetic(m.FAdd); break;
                case Mnemonic.fcnv: RewriteFcnv(); break;
                case Mnemonic.fcnvxf: RewriteFcnvxf(); break;
                case Mnemonic.fcpy: RewriteFcpy(); break;
                case Mnemonic.fid: RewriteFid(); break;
                case Mnemonic.fldd: RewriteFld(PrimitiveType.Real64); break;
                case Mnemonic.fldw: RewriteFld(PrimitiveType.Real32); break;
                case Mnemonic.fmpy: RewriteFpArithmetic(m.FMul); break;
                case Mnemonic.fstd: RewriteFst(PrimitiveType.Real64); break;
                case Mnemonic.fstw: RewriteFst(PrimitiveType.Real32); break;
                case Mnemonic.fsub: RewriteFpArithmetic(m.FSub); break;
                case Mnemonic.ldb: RewriteLd(PrimitiveType.Byte); break;
                case Mnemonic.ldd: RewriteLd(PrimitiveType.Word64); break;
                case Mnemonic.ldh: RewriteLd(PrimitiveType.Word16); break;
                case Mnemonic.ldil: RewriteLdil(); break;
                case Mnemonic.ldo: RewriteLdo(); break;
                case Mnemonic.ldsid: RewriteLdsid(); break;
                case Mnemonic.ldw: RewriteLd(PrimitiveType.Word32); break;
                case Mnemonic.ldwa: RewriteLd(PrimitiveType.Word32); break;
                case Mnemonic.mfctl: RewriteMfctl(); break;
                case Mnemonic.mfctl_w: RewriteMfctl(); break;
                case Mnemonic.movb: RewriteMovb(); break;
                case Mnemonic.mtctl: RewriteMtctl(); break;
                case Mnemonic.mtsm: RewriteMtsm(); break;
                case Mnemonic.mtsp: RewriteMtsp(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.rfi: RewriteRfi(rfi_intrinsic); break;
                case Mnemonic.rfi_r: RewriteRfi(rfi_r_intrinsic); break;
                case Mnemonic.shladd: RewriteShladd(); break;
                case Mnemonic.shrpd: RewriteShrp(PrimitiveType.Word64, PrimitiveType.Word128); break;
                case Mnemonic.shrpw: RewriteShrp(PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Mnemonic.stb: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.std: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stda: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.sth: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.stw: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.sub_b: RewriteSub_b(); break;
                case Mnemonic.subi: RewriteSubi(); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("PaRiscRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void MaybeAnnulNextInstruction(InstrClass iclass, Expression e)
        {
            var addrNext = instr.Address + 8;
            MaybeConditionalJump(InstrClass.ConditionalTransfer, addrNext, RewriteCondition, false, e);
        }

        private bool MaybeSkipNextInstruction(InstrClass iclass, bool invert, Expression left, Expression? right = null)
        {
            var addrNext = instr.Address + 8;
            if (MaybeConditionalJump(iclass, addrNext, RewriteCondition, invert, left, right))
            {
                this.iclass = InstrClass.ConditionalTransfer;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MaybeConditionalJump(
            InstrClass iclass,
            Address addrTaken,
            Func<Expression,Expression,Expression> rewriteCondition,
            bool invert, 
            Expression left,
            Expression? right = null)
        {
            if (instr.Condition is null ||
                instr.Condition.Type == ConditionType.Never ||
                instr.Condition.Type == ConditionType.Never64)
                return false;

            right = right ?? Constant.Word(left.DataType.BitSize, 0);
            Expression e = rewriteCondition(left, right);
            if (e is Constant c)
            {
                if (!c.IsZero)
                {
                    m.Goto(addrTaken);
                    return true;
                }
            }
            if (invert)
                e = e.Invert();
            m.Branch(e, addrTaken, iclass);
            return true;
        }

        private Expression RewriteOp(int iOp)
        {
            switch (instr.Operands[iOp])
            {
            case RegisterStorage r:
                if (r == arch.Registers.GpRegs[0])
                    return Constant.Zero(r.DataType);
                else
                    return binder.EnsureRegister(r);
            case Constant i:
                return i;
            case LeftImmediateOperand l:
                return l.Value;
            case Address a:
                return a;
            case MemoryOperand mem:
                Identifier rb = binder.EnsureRegister(mem.Base);
                Expression ea = rb;
                if (mem.Index is not null)
                {
                    if (mem.Index != arch.Registers.GpRegs[0])
                    {
                        var idx = binder.EnsureRegister(mem.Index);
                        ea = m.IAdd(ea, idx);
                    }
                }
                else if (mem.Offset != 0)
                {
                    ea = m.IAddS(ea, mem.Offset);
                }
                if (instr.BaseReg == AddrRegMod.mb)
                {
                    m.Assign(rb, ea);
                    ea = rb;
                }
                else if (instr.BaseReg == AddrRegMod.ma)
                {
                    var tmp = binder.CreateTemporary(rb.DataType);
                    m.Assign(tmp, ea);
                    m.Assign(rb, tmp);
                    ea = tmp;
                }
                return m.Mem(mem.DataType, ea);
            }
            throw new NotImplementedException($"Unimplemented PA-RISC operand type {instr.Operands[iOp].GetType()}.");
        }

        private Expression RewriteCondition(Expression left, Expression right)
        {
            Expression e;
            switch (instr.Condition!.Type)
            {
            case ConditionType.Tr: e = Constant.True(); break;
            case ConditionType.Never:
            case ConditionType.Never64:
                e = Constant.False(); break;
            case ConditionType.Eq:
            case ConditionType.Eq64:
                e = m.Eq(left, right); break;
            case ConditionType.Ne:
            case ConditionType.Ne64:
                e = m.Ne(left, right); break;
            case ConditionType.Lt:
            case ConditionType.Lt64:
                e = m.Lt(left, right); break;
            case ConditionType.Le:
            case ConditionType.Le64:
                e = m.Le(left, right); break;
            case ConditionType.Ge:
            case ConditionType.Ge64:
                e = m.Ge(left, right); break;
            case ConditionType.Gt:
            case ConditionType.Gt64:
                e = m.Gt(left, right); break;
            case ConditionType.Ult:
            case ConditionType.Ult64:
                e = m.Ult(left, right); break;
            case ConditionType.Ule:
            case ConditionType.Ule64:
                e = m.Ule(left, right); break;
            case ConditionType.Uge:
            case ConditionType.Uge64:
                e = m.Uge(left, right); break;
            case ConditionType.Ugt:
            case ConditionType.Ugt64:
                e = m.Ugt(left, right); break;
            case ConditionType.Uv:
            case ConditionType.Uv64:
                e = m.Test(ConditionCode.NO, m.USub(left, right)); break;
            case ConditionType.Sv:
            case ConditionType.Sv64:
                e = m.Test(ConditionCode.NO, m.ISub(left, right)); break;
            case ConditionType.Nuv:
            case ConditionType.Nuv64:
                e = m.Test(ConditionCode.OV, m.USub(left, right)); break;
            case ConditionType.Nsv:
            case ConditionType.Nsv64:
                e = m.Test(ConditionCode.OV, m.ISub(left, right)); break;
            case ConditionType.Even:
            case ConditionType.Even64:
                e = m.Eq0(m.And(left, 1)); break;
            case ConditionType.Odd:
            case ConditionType.Odd64:
                e = m.Ne0(m.And(left, 1)); break;
            case ConditionType.Vnz:
            case ConditionType.Vnz64:
                e = m.And(m.Eq(left, right), m.Test(ConditionCode.NO, m.ISub(left, right))); break;
            case ConditionType.Znv:
            case ConditionType.Znv64:
                e = m.And(m.Ne(left, right), m.Test(ConditionCode.OV, m.ISub(left, right))); break;
            default:
                throw new NotImplementedException(instr.Condition.ToString());
            }
            return e;
        }

        static PaRiscRewriter()
        {
            depw_intrinsic = new IntrinsicBuilder("__depw", false)
                .Param(PrimitiveType.Word64)
                .Param(PrimitiveType.Int32)
                .Param(PrimitiveType.Int32)
                .Returns(PrimitiveType.Word32);
        }

        private static readonly IntrinsicProcedure break_intrinsic = new IntrinsicBuilder("__break", true)
            .Void();
        private static readonly IntrinsicProcedure depw_intrinsic;
        private static readonly IntrinsicProcedure diag_intrinsic = new IntrinsicBuilder("__diag", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure division_step_intrinsic = IntrinsicBuilder.GenericBinary("__division_step");
        private static readonly IntrinsicProcedure fid_intrinsic = new IntrinsicBuilder("__fid", true)
            .Void();
        private static readonly IntrinsicProcedure is_bit_set_intrinsic = new IntrinsicBuilder("__is_bit_set", false)
            .GenericTypes("TValue", "TPos")
            .Param("TValue")
            .Param("TPos")
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure mtsm_intrinsic = new IntrinsicBuilder("__mtsm", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure rfi_intrinsic = new IntrinsicBuilder("__rfi", true)
            .Void();
        private static readonly IntrinsicProcedure rfi_r_intrinsic = new IntrinsicBuilder("__rfi_r", true)
            .Void();
        private static readonly IntrinsicProcedure trap_intrinsic = new IntrinsicBuilder("__trap", true)
            .Void();
    }
}
