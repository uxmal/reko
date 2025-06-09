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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.H8
{
    public class H8Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly FlagGroupStorage C = new FlagGroupStorage(Registers.CcRegister, (uint) FlagM.CF, nameof(C));
        private static readonly FlagGroupStorage H = new FlagGroupStorage(Registers.CcRegister, (uint) FlagM.HF, nameof(H));
        private static readonly FlagGroupStorage HNZVC = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.HF|FlagM.NF|FlagM.ZF|FlagM.VF|FlagM.CF), nameof(HNZVC));
        private static readonly FlagGroupStorage N = new FlagGroupStorage(Registers.CcRegister, (uint) FlagM.NF, nameof(N));
        private static readonly FlagGroupStorage NV = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.NF | FlagM.VF), nameof(NV));
        private static readonly FlagGroupStorage NZ = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.NF | FlagM.ZF), nameof(NZ));
        private static readonly FlagGroupStorage NZC = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.NF|FlagM.ZF|FlagM.VF), nameof(NZC));
        private static readonly FlagGroupStorage NZV = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.NF|FlagM.ZF|FlagM.VF), nameof(NZV));
        private static readonly FlagGroupStorage NZVC = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.NF|FlagM.ZF|FlagM.VF|FlagM.CF), nameof(NZVC));
        private static readonly FlagGroupStorage V = new FlagGroupStorage(Registers.CcRegister, (uint) FlagM.VF, nameof(V));
        private static readonly FlagGroupStorage ZC = new FlagGroupStorage(Registers.CcRegister, (uint) (FlagM.ZF | FlagM.CF), nameof(ZC));
        private static readonly FlagGroupStorage Z = new FlagGroupStorage(Registers.CcRegister, (uint) FlagM.ZF, nameof(Z));


        private readonly H8Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<H8Instruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private InstrClass iclass;

        public H8Rewriter(H8Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new H8Disassembler(arch, rdr).GetEnumerator();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    iclass = InstrClass.Invalid;
                    break;
                case Mnemonic.add: RewriteAdd(instr); break;
                case Mnemonic.adds: RewriteAdds(instr); break;
                case Mnemonic.addx: RewriteAddxSubx(instr, m.IAdd); break;
                case Mnemonic.and: RewriteLogical(instr, m.And); break;
                case Mnemonic.andc: RewriteLogicalC(instr, m.And); break;
                case Mnemonic.band: RewriteLogicalB(instr, m.And); break;
                case Mnemonic.bclr: RewriteBclr(instr); break;
                case Mnemonic.biand: RewriteLogicalB(instr, InvAnd); break;
                case Mnemonic.bild: RewriteLogicalB(instr, InvLd); break;
                case Mnemonic.bior: RewriteLogicalB(instr, InvOr); break;
                case Mnemonic.bist: RewriteBist(instr); break;
                case Mnemonic.bixor: RewriteLogicalB(instr, InvXor); break;
                case Mnemonic.bld: RewriteBtst(instr, C); break;
                case Mnemonic.bnot: RewriteBnot(instr); break;
                case Mnemonic.bor: RewriteLogicalB(instr, m.Or); break;
                case Mnemonic.bset: RewriteBset(instr, Constant.True()); break;
                case Mnemonic.bst: RewriteBset(instr, binder.EnsureFlagGroup(C)); break;
                case Mnemonic.bsr: RewriteBsr(instr); break;
                case Mnemonic.btst: RewriteBtst(instr, Z); break;
                case Mnemonic.bxor: RewriteLogicalB(instr, m.Xor); break;

                case Mnemonic.bra: RewriteBranch(instr); break;
                case Mnemonic.brn: RewriteNop(); break;
                case Mnemonic.bhi: RewriteBranch(instr, ConditionCode.UGT, ZC); break;
                case Mnemonic.bls: RewriteBranch(instr, ConditionCode.ULE, ZC); break;
                case Mnemonic.bcc: RewriteBranch(instr, ConditionCode.UGE, C); break;
                case Mnemonic.bcs: RewriteBranch(instr, ConditionCode.ULT, C); break;
                case Mnemonic.bne: RewriteBranch(instr, ConditionCode.NE, Z); break;
                case Mnemonic.beq: RewriteBranch(instr, ConditionCode.EQ, Z); break;
                case Mnemonic.bvc: RewriteBranch(instr, ConditionCode.NO, V); break;
                case Mnemonic.bvs: RewriteBranch(instr, ConditionCode.OV, V); break;
                case Mnemonic.bpl: RewriteBranch(instr, ConditionCode.GE, N); break;
                case Mnemonic.bmi: RewriteBranch(instr, ConditionCode.LT, N); break;
                case Mnemonic.bge: RewriteBranch(instr, ConditionCode.GE, NV); break;
                case Mnemonic.blt: RewriteBranch(instr, ConditionCode.LT, NV); break;
                case Mnemonic.bgt: RewriteBranch(instr, ConditionCode.GT, NZV); break;
                case Mnemonic.ble: RewriteBranch(instr, ConditionCode.LE, NZV); break;

                case Mnemonic.clrmac: RewriteClrmac(instr); break;
                case Mnemonic.cmp: RewriteCmp(instr); break;
                case Mnemonic.daa: RewriteDaaDas(instr, daa_intrinsic); break;
                case Mnemonic.das: RewriteDaaDas(instr, das_intrinsic); break;
                case Mnemonic.dec: RewriteIncDec(instr, m.ISub); break;
                case Mnemonic.divxs: RewriteDivx(instr, m.SDiv, m.SMod); break;
                case Mnemonic.divxu: RewriteDivx(instr, m.UDiv, m.UMod); break;
                case Mnemonic.exts: RewriteExt(instr, Domain.SignedInt); break;
                case Mnemonic.extu: RewriteExt(instr, Domain.UnsignedInt); break;
                case Mnemonic.inc: RewriteIncDec(instr, m.IAdd); break;
                case Mnemonic.jmp: RewriteJmp(instr); break;
                case Mnemonic.jsr: RewriteJsr(instr); break;
                case Mnemonic.ldc: RewriteLdc(instr); break;
                case Mnemonic.ldm: RewriteLdm(instr); break;
                case Mnemonic.ldmac: RewriteLdmac(instr); break;
                case Mnemonic.mov: RewriteMov(instr); break;
                case Mnemonic.movfpe: RewriteMovfpe(instr); break;
                case Mnemonic.movtpe: RewriteMovtpe(instr); break;
                case Mnemonic.mulxs: RewriteMulx(instr, m.SMul, true); break;
                case Mnemonic.mulxu: RewriteMulx(instr, m.UMul, false); break;
                case Mnemonic.nop: RewriteNop(); break;
                case Mnemonic.neg: RewriteNeg(instr); break;
                case Mnemonic.not: RewriteUnaryLogical(instr, m.Comp); break;
                case Mnemonic.or: RewriteLogical(instr, m.Or); break;
                case Mnemonic.orc: RewriteLogicalC(instr, m.Or); break;
                case Mnemonic.rotl: RewriteRotation(instr, CommonOps.Rol); break;
                case Mnemonic.rotr: RewriteRotation(instr, CommonOps.Ror); break;
                case Mnemonic.rotxl: RewriteRotationX(instr, CommonOps.RolC); break;
                case Mnemonic.rotxr: RewriteRotationX(instr, CommonOps.RorC); break;
                case Mnemonic.rte: RewriteRte(); break;
                case Mnemonic.rts: RewriteRts(); break;
                case Mnemonic.shal: RewriteShift(instr, m.Shl); break;
                case Mnemonic.shar: RewriteShift(instr, m.Sar); break;
                case Mnemonic.shll: RewriteShift(instr, m.Shl); break;
                case Mnemonic.shlr: RewriteShift(instr, m.Shr); break;
                case Mnemonic.sleep: RewriteSleep(instr); break;
                case Mnemonic.stc: RewriteStc(instr); break;
                case Mnemonic.stm: RewriteStm(instr); break;
                case Mnemonic.stmac: RewriteStmac(instr); break;
                case Mnemonic.sub: RewriteSub(instr); break;
                case Mnemonic.subs: RewriteSubs(instr); break;
                case Mnemonic.subx: RewriteAddxSubx(instr, m.ISub); break;
                case Mnemonic.trapa: RewriteTrapa(instr); break;
                case Mnemonic.xor: RewriteLogical(instr, m.Xor); break;
                case Mnemonic.xorc: RewriteLogicalC(instr, m.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                this.instrs.Clear();
            }
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression OpSrc(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base is not null)
                {
                    var regBase = binder.EnsureRegister(mem.Base);
                    if (mem.PostIncrement)
                    {
                        ea = binder.CreateTemporary(regBase.DataType);
                        m.Assign(ea, regBase);
                        m.Assign(regBase, m.IAddS(regBase, mem.DataType.Size));
                    }
                    else if (mem.PreDecrement)
                    {
                        m.Assign(regBase, m.ISubS(regBase, mem.DataType.Size));
                        ea = regBase;
                    }
                    else
                    {
                        ea = m.AddSubSignedInt(regBase, mem.Offset);
                    }
                }
                else if (mem.AddressWidth is not null && mem.AddressWidth.BitSize == 16)
                {
                    ea = Address.Ptr16((ushort) mem.Offset);
                }
                else
                {
                    ea = Address.Ptr32((uint) mem.Offset);
                }
                return m.Mem(mem.DataType ?? (DataType) VoidType.Instance, ea);
            }
            throw new NotImplementedException();
        }

        private Expression OpDst(MachineOperand op, Expression src, Func<Expression,Expression,Expression> fn)
        {
            Expression dst;
            switch (op)
            {
            case RegisterStorage reg:
                dst = binder.EnsureRegister(reg);
                m.Assign(dst, fn(dst, src));
                return dst;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base is not null)
                {
                    var regBase = binder.EnsureRegister(mem.Base);
                    if (mem.PreDecrement)
                    {
                        ea = binder.EnsureRegister(mem.Base!);
                        m.Assign(ea, m.ISubS(ea, mem.DataType.Size));
                    }
                    else if (mem.PostIncrement)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        ea = m.AddSubSignedInt(regBase, mem.Offset);
                    }
                }
                else
                {
                    ea = Address.Ptr16((ushort) mem.Offset);
                }
                dst = fn(m.Mem(mem.DataType, ea), src);
                if (dst is Identifier || dst is Constant)
                {
                    m.Assign(m.Mem(mem.DataType, ea), dst);
                }
                else
                {
                    var tmp = binder.CreateTemporary(mem.DataType);
                    m.Assign(tmp, dst);
                    dst = tmp;
                }
                if (mem.PostIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, mem.DataType.Size));
                }
                return dst;
            }
            throw new NotImplementedException();
        }

        private void EmitCond(FlagGroupStorage grf, Expression e)
        {
            m.Assign(binder.EnsureFlagGroup(grf), m.Cond(e));
        }

        private void EmitUnitTest(H8Instruction instr, string message = "")
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("H8Rw", instr, instr.Mnemonic.ToString(), rdr, message);
        }

        private Expression InvAnd(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private Expression InvLd(Expression a, Expression b)
        {
            return m.Comp(b);
        }

        private Expression InvOr(Expression a, Expression b)
        {
            return m.Or(a, m.Comp(b));
        }

        private Expression InvXor(Expression a, Expression b)
        {
            return m.Xor(a, m.Comp(b));
        }

        private void RewriteAdd(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, m.IAdd);
            EmitCond(NZVC, dst);
        }

        private void RewriteAdds(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, m.IAdd);
        }

        private void RewriteAddxSubx(H8Instruction instr, Func<Expression, Expression, Expression> fn)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, (d, s) => fn(fn(d, s), binder.EnsureFlagGroup(C)));
            EmitCond(NZVC, dst);
        }

        private void RewriteBclr(H8Instruction instr)
        {
            var left = OpSrc(instr.Operands[1]);
            var right = OpSrc(instr.Operands[0]);
            m.Assign(left, m.Fn(
                CommonOps.ClearBit.MakeInstance(left.DataType, right.DataType),
                left, right));
        }

        private void RewriteBist(H8Instruction instr)
        {
            var left = OpSrc(instr.Operands[1]);
            var right = OpSrc(instr.Operands[0]);
            m.Assign(left, m.Fn(
                CommonOps.WriteBit.MakeInstance(left.DataType, right.DataType),
                left, right, m.Comp(binder.EnsureFlagGroup(C))));
        }

        private void RewriteBnot(H8Instruction instr)
        {
            var left = OpSrc(instr.Operands[1]);
            var right = OpSrc(instr.Operands[0]);
            m.Assign(left, m.Fn(
                CommonOps.InvertBit.MakeInstance(left.DataType, right.DataType),
                left, right));

        }

        private void RewriteBranch(H8Instruction instr)
        {
            m.Goto((Address)instr.Operands[0]);
        }

        private void RewriteBranch(H8Instruction instr, ConditionCode cc, FlagGroupStorage grf)
        {
            var test = m.Test(cc, binder.EnsureFlagGroup(grf));
            m.Branch(test, (Address)instr.Operands[0]);
        }

        private void RewriteClrmac(H8Instruction instr)
        {
            var mac = binder.EnsureRegister(Registers.Mac);
            m.Assign(mac, 0);
        }

        private void RewriteCmp(H8Instruction instr)
        {
            var right = OpSrc(instr.Operands[0]);
            var left = OpSrc(instr.Operands[1]);
            EmitCond(NZVC, m.ISub(left, right));
        }

        private void RewriteDaaDas(H8Instruction instr, IntrinsicProcedure fn)
        {
            var reg = OpSrc(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(H8Rewriter.C);
            var H = binder.EnsureFlagGroup(H8Rewriter.H);
            m.Assign(reg, m.Fn(fn, reg, C, H));
            EmitCond(HNZVC, reg);
        }

        private void RewriteDivx(
            H8Instruction instr, 
            Func<Expression,Expression, Expression> div,
            Func<Expression,Expression, Expression> mod)
        {
            var dividend = OpSrc(instr.Operands[1]);
            var divisor = OpSrc(instr.Operands[0]);
            var quo = binder.CreateTemporary(divisor.DataType);
            var rem = binder.CreateTemporary(divisor.DataType);

            var d = div(dividend, divisor);
            d.DataType = divisor.DataType;
            m.Assign(quo, d);

            var r = mod(dividend, divisor);
            r.DataType = divisor.DataType;
            m.Assign(rem, r);

            EmitCond(Z, divisor);
            EmitCond(N, quo);
            m.Assign(dividend, m.Seq(rem, quo));
        }

        private void RewriteExt(H8Instruction instr, Domain domain)
        {
            var dstRange = new BitRange(0, instr.Size!.BitSize);
            var srcRange = new BitRange(0, instr.Size!.BitSize / 2);
            var dt = PrimitiveType.Create(domain, dstRange.Extent);
            var dst = (Identifier) OpSrc(instr.Operands[0]);
            var src = binder.EnsureRegister(arch.GetRegister(
                dst.Storage.Domain, 
                srcRange)!);
            m.Assign(dst, m.Convert(src, src.DataType, dt));
            EmitCond(Z, dst);
            m.Assign(binder.EnsureFlagGroup(N), Constant.False());
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private void RewriteIncDec(H8Instruction instr, Func<Expression, Expression, Expression> fn)
        {
            Identifier reg;
            Expression incr;
            if (instr.Operands.Length == 1)
            {
                reg = (Identifier) OpSrc(instr.Operands[0]);
                incr = Constant.Create(reg.DataType, 1);
            }
            else
            {
                reg = (Identifier) OpSrc(instr.Operands[1]);
                incr = OpSrc(instr.Operands[0]);
            }
            m.Assign(reg, fn(reg, incr));
            EmitCond(NZV, reg);
        }
        
        private void RewriteJmp(H8Instruction instr)
        {
            var target = ((MemoryAccess) OpSrc(instr.Operands[0])).EffectiveAddress;
            m.Goto(target);
        }

        private void RewriteJsr(H8Instruction instr)
        {
            var target = ((MemoryAccess) OpSrc(instr.Operands[0])).EffectiveAddress;
            m.Call(target, 2);      //$REVIEW: what about 'advanced mode'?
        }

        private void RewriteLdc(H8Instruction instr)
        {
            m.Assign(binder.EnsureFlagGroup(NZVC), OpSrc(instr.Operands[0]));
        }

        private void RewriteLdm(H8Instruction instr)
        {
            var regs = (RegisterListOperand) instr.Operands[1];
            var sp = binder.EnsureRegister(Registers.GpRegisters[7]);
            for (int iReg = regs.Count-1; iReg >= 0; --iReg)
            {
                var reg = Registers.GpRegisters[iReg + regs.RegisterNumber];
                m.Assign(
                    binder.EnsureRegister(reg),
                    m.Mem32(sp));
                m.Assign(sp, m.IAddS(sp, 4));
            }
        }

        private void RewriteLdmac(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, (d, s) => s);
        }

        private void RewriteBset(H8Instruction instr, Expression value)
        {
            var pos = OpSrc(instr.Operands[0]);
            var dst = OpSrc(instr.Operands[1]);
            m.Assign(dst, m.Fn(bset_intrinsic.MakeInstance(dst.DataType, pos.DataType), dst, value, pos));
        }

        private void RewriteBsr(H8Instruction instr)
        {
            m.Call(OpSrc(instr.Operands[0]), 2);
        }

        private void RewriteBtst(H8Instruction instr, FlagGroupStorage flag)
        {
            var right = OpSrc(instr.Operands[0]);
            var left = OpSrc(instr.Operands[1]);
            var dst = binder.EnsureFlagGroup(flag);
            m.Assign(
                dst,
                m.Conditional(
                    dst.DataType,
                    m.Fn(btst_intrinsic.MakeInstance(left.DataType, right.DataType), left, right),
                    Constant.Create(dst.DataType, 1),
                    Constant.Create(dst.DataType, 0)));
        }

        private void RewriteLogical(H8Instruction instr, Func<Expression,Expression, Expression> fn)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, fn);
            EmitCond(NZ, dst);
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private void RewriteLogicalB(H8Instruction instr, Func<Expression, Expression, Expression> fn)
        {
            var c = binder.EnsureFlagGroup(C);
            var right = OpSrc(instr.Operands[0]);
            var left = OpSrc(instr.Operands[1]);
            var src = m.Fn(
                CommonOps.Bit.MakeInstance(left.DataType, right.DataType),
                left,
                right);
            m.Assign(c, fn(c, src));
        }

        private void RewriteLogicalC(H8Instruction instr, Func<Expression, Expression, Expression> fn)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, fn);
        }

        private void RewriteMov(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, (d, s) => s);
            EmitCond(NZ, dst);
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private void RewriteMovfpe(H8Instruction instr)
        {
            var mem = (MemoryOperand) instr.Operands[0];
            m.Assign(
                OpSrc(instr.Operands[1]),
                m.Fn(movfpe_intrinsic, Constant.UInt16((ushort) mem.Offset)));
        }

        private void RewriteMovtpe(H8Instruction instr)
        {
            var mem = (MemoryOperand) instr.Operands[1];
            m.SideEffect(m.Fn(
                movtpe_intrinsic,
                Constant.UInt16((ushort) mem.Offset),
                OpSrc(instr.Operands[0])));
        }

        private void RewriteMulx(
            H8Instruction instr,
            Func<Expression, Expression, Expression> mul,
            bool setCc)
        {
            var right = OpSrc(instr.Operands[0]);
            var left = (Identifier) OpSrc(instr.Operands[1]);
            var dst = binder.EnsureRegister(arch.GetRegister(
                left.Storage.Domain,
                new BitRange(0, left.DataType.BitSize * 2))!);
            var product = mul(left, right);
            product.DataType = dst.DataType;
            m.Assign(dst, product);
            if (setCc)
            {
                EmitCond(NZ, dst);
            }
        }

        private void RewriteNeg(H8Instruction instr)
        {
            var reg = (Identifier) OpSrc(instr.Operands[0]);
            m.Assign(reg, m.Neg(reg));
            EmitCond(NZVC, reg);
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void RewriteRotation(H8Instruction instr, IntrinsicProcedure intrinsic)
        {
            var src = OpSrc(instr.Operands[0]);
            m.Assign(src, m.Fn(
                intrinsic.MakeInstance(src.DataType, PrimitiveType.Byte), 
                src, Constant.Byte(1)));
            EmitCond(NZC, src);
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private void RewriteRotationX(H8Instruction instr, IntrinsicProcedure intrinsic)
        {
            var src = OpSrc(instr.Operands[0]);
            var c = binder.EnsureFlagGroup(C);
            m.Assign(src, m.Fn(
                intrinsic.MakeInstance(src.DataType, PrimitiveType.Byte),
                src, Constant.Byte(1), c));
            EmitCond(NZC, src);
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private void RewriteRte()
        {
            m.SideEffect(m.Fn(rte_intrinsic));
            m.Return(2, 0);
        }

        private void RewriteRts()
        {
            m.Return(2, 0);
        }

        private void RewriteShift(H8Instruction instr, Func<Expression, Expression, Expression> fn)
        {
            int shift;
            Expression src;
            if (instr.Operands.Length == 2)
            {
                shift = ((Constant)instr.Operands[0]).ToInt32();
                src = OpSrc(instr.Operands[1]);
            }
            else
            {
                shift = 1;
                src = OpSrc(instr.Operands[0]);
            }
            m.Assign(src, fn(src, Constant.Int32(shift)));
            EmitCond(NZVC, src);
        }

        private void RewriteSleep(H8Instruction instr)
        {
            m.SideEffect(m.Fn(sleep_intrinsic));
        }

        private void RewriteStc(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpSrc(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteStm(H8Instruction instr)
        {
            var regs = (RegisterListOperand) instr.Operands[0];
            var sp = binder.EnsureRegister(Registers.GpRegisters[7]);
            for (int iReg = 0; iReg < regs.Count; ++iReg)
            {
                m.Assign(sp, m.ISubS(sp, 4));
                var reg = Registers.GpRegisters[iReg + regs.RegisterNumber];
                m.Assign(
                    m.Mem32(sp),
                    binder.EnsureRegister(reg));
            }
        }

        private void RewriteStmac(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, (d, s) => s);
        }

        private void RewriteSub(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, m.ISub);
            EmitCond(NZVC, dst);
        }

        private void RewriteSubs(H8Instruction instr)
        {
            var src = OpSrc(instr.Operands[0]);
            var dst = OpDst(instr.Operands[1], src, m.ISub);
        }

        private void RewriteTrapa(H8Instruction instr)
        {
            var vector = OpSrc(instr.Operands[0]);
            m.SideEffect(
                m.Fn(
                    CommonOps.Syscall_1.MakeInstance(vector.DataType),
                    vector));
        }

        private void RewriteUnaryLogical(H8Instruction instr, Func<Expression, Expression> fn)
        {
            var src = OpSrc(instr.Operands[0]);
            m.Assign(src, fn(src));
            EmitCond(NZ, src);
            m.Assign(binder.EnsureFlagGroup(V), Constant.False());
        }

        private static readonly IntrinsicProcedure bset_intrinsic = new IntrinsicBuilder("__bset", false)
            .GenericTypes("TValue", "TPos")
            .Param("TValue")
            .Param(PrimitiveType.Bool)
            .Param("TPos")
            .Returns("TValue");
        private static readonly IntrinsicProcedure btst_intrinsic = new IntrinsicBuilder("__btst", false)
            .GenericTypes("TValue", "TPos")
            .Param("TValue")
            .Param("TPos")
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure daa_intrinsic = new IntrinsicBuilder("__decimal_adjust_add", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Bool)
            .Param(PrimitiveType.Bool)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure das_intrinsic = new IntrinsicBuilder("__decimal_adjust_subtract", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Bool)
            .Param(PrimitiveType.Bool)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure movfpe_intrinsic = new IntrinsicBuilder("__move_from_peripheral", true)
            .Param(PrimitiveType.UInt16)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure movtpe_intrinsic = new IntrinsicBuilder("__move_to_peripheral", true)
            .Param(PrimitiveType.UInt16)
            .Param(PrimitiveType.Byte)
            .Void();

        private static readonly IntrinsicProcedure rte_intrinsic = new IntrinsicBuilder("__return_from_exception", true)
            .Void();

        private static readonly IntrinsicProcedure sleep_intrinsic = new IntrinsicBuilder("__sleep", true)
            .Void();
    }
}