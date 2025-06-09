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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteAdcSbc(Func<Expression,Expression,Expression> opr, Action<Expression>? setFlags = null)
        {
            var opSrc1 = RewriteOp(1);
            var opSrc2 = RewriteOp(2);
            var opDst = RewriteOp(0);

            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(
                opDst,
                opr(
                    opr(opSrc1, opSrc2),
                    c));
            setFlags?.Invoke(m.Cond(opDst));
        }

        private void RewriteAdrp()
        {
            var dst = RewriteOp(instr.Operands[0]);
            var addr = (Address)instr.Operands[1];
            m.Assign(dst, addr);
        }

        /// <summary>
        /// Rewrites an instruction where the mnemonic is the same for different
        /// operand types.
        /// </summary>
        private void RewriteMaybeSimdBinary(
            Func<Expression, Expression, Expression> fn,
            IntrinsicProcedure intrinsic,
            Domain domain = Domain.None,
            Action<Expression>? setFlags = null)
        {
            if (instr.VectorData != VectorData.Invalid || instr.Operands[0] is VectorRegisterOperand vr)
            {
                RewriteSimdBinary(intrinsic, domain, setFlags);
            }
            else
            {
                RewriteBinary(fn, setFlags);
            }
        }

        private void RewriteMaybeSimdBinary(Intrinsics.IntrinsicPair intrinsic, Domain domain = Domain.None)
        {
            if (instr.VectorData != VectorData.Invalid || instr.Operands[0] is VectorRegisterOperand vr)
            {
                RewriteSimdBinary(intrinsic.Vector, domain, null);
            }
            else
            {
                RewriteBinary(intrinsic.Scalar);
            }
        }

        private void RewriteMaybeSimdUnary(
            Func<Expression, Expression> fn,
            IntrinsicProcedure simdGeneric,
            Domain domain = Domain.None)
        {
            if (instr.VectorData != VectorData.Invalid ||
                (instr.Operands[0] is VectorRegisterOperand vr && vr.Index < 0))
            {
                RewriteSimdUnary(simdGeneric, domain);
            }
            else
            {
                RewriteUnary(fn);
            }
        }

        private void RewriteBinary(Func<Expression, Expression, Expression> fn, Action<Expression>? setFlags = null)
        {
            var dst = RewriteOp(instr.Operands[0]);
            var left = RewriteOp(instr.Operands[1], true);
            var right = RewriteOp(instr.Operands[2], true);

            var toBitSize = left.DataType.BitSize;
            right = MaybeExtendExpression(right, toBitSize);
            m.Assign(dst, fn(left, right));
            setFlags?.Invoke(m.Cond(dst));
        }

        private void RewriteBinary(IntrinsicProcedure intrinsic, Action<Expression>? setFlags = null)
        {
            var dst = RewriteOp(instr.Operands[0]);
            var left = RewriteOp(instr.Operands[1], true);
            var right = RewriteOp(instr.Operands[2], true);

            var toBitSize = left.DataType.BitSize;
            right = MaybeExtendExpression(right, toBitSize);
            var src = m.Fn(intrinsic.MakeInstance(left.DataType), left, right);
            m.Assign(dst, src);
            setFlags?.Invoke(m.Cond(dst));
        }

        private void RewriteTernary(IntrinsicProcedure intrinsic, Domain domain)
        {
            var src1 = RewriteOp(0, true);
            var src2 = RewriteOp(1, true);
            var src3 = RewriteOp(2, true);
            var dst = RewriteOp(0);

            var dtSrc1 = PrimitiveType.Create(domain, src1.DataType.BitSize);
            var src = m.Fn(intrinsic.MakeInstance(dtSrc1), src1, src2, src3);
            m.Assign(dst, src);
        }

        private Expression MaybeExtend(Expression expr, DataType dt)
        {
            if (expr.DataType.BitSize < dt.BitSize)
            {
                return m.Convert(expr, expr.DataType, dt);
            }
            else
            {
                return expr;
            }
        }

        private Expression MaybeExtendExpression(Expression right, int toBitSize)
        {
            if (instr.ShiftCode != Mnemonic.Invalid &&
                (instr.ShiftCode != Mnemonic.lsl ||
                !(instr.ShiftAmount is Constant imm) ||
                !imm.IsIntegerZero))
            {
                Expression MaybeShift(Expression e, Expression amt)
                {
                    if (amt is Constant c && c.ToInt32() > 0)
                        return m.Shl(e, amt);
                    else
                        return e;
                }
                var amt = RewriteOp(instr.ShiftAmount!);
                switch (instr.ShiftCode)
                {
                case Mnemonic.asr: right = m.Sar(right, amt); break;
                case Mnemonic.lsl: right = m.Shl(right, amt); break;
                case Mnemonic.lsr: right = m.Shr(right, amt); break;
                case Mnemonic.ror: right = m.Fn(CommonOps.Ror, right, amt); break;
                case Mnemonic.sxtb: right = MaybeShift(SignExtend(toBitSize, PrimitiveType.SByte, right), amt); break;
                case Mnemonic.sxth: right = MaybeShift(SignExtend(toBitSize, PrimitiveType.Int16, right), amt); break;
                case Mnemonic.sxtw: right = MaybeShift(SignExtend(toBitSize, PrimitiveType.Int32, right), amt); break;
                case Mnemonic.sxtx: right = MaybeShift(SignExtend(toBitSize, PrimitiveType.Int64, right), amt); break;
                case Mnemonic.uxtb: right = MaybeShift(ZeroExtend(toBitSize, PrimitiveType.Byte, right), amt); break;
                case Mnemonic.uxth: right = MaybeShift(ZeroExtend(toBitSize, PrimitiveType.Word16, right), amt); break;
                case Mnemonic.uxtw: right = MaybeShift(ZeroExtend(toBitSize, PrimitiveType.Word32, right), amt); break;
                case Mnemonic.uxtx: right = MaybeShift(ZeroExtend(toBitSize, PrimitiveType.Word64, right), amt); break;
                default:
                    EmitUnitTest();
                    break;
                }
            }

            return right;
        }

        private void RewriteBfm()
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var src3 = RewriteOp(3);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.bfm.MakeInstance(src1.DataType), dst, src1, src2, src3));
        }

        private void RewriteCcmn()
        {
            var nzcv = NZCV();
            var tmp = binder.CreateTemporary(PrimitiveType.Bool);
            var cond = Invert(((ConditionOperand<ArmCondition>) instr.Operands[3]).Condition);
            m.Assign(tmp, this.TestCond(cond));
            var c = ((Constant)instr.Operands[2]).ToUInt32();
            m.Assign(nzcv, Constant.Word32(c << 28));
            m.BranchInMiddleOfInstruction(tmp, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            m.Assign(nzcv, m.Cond(m.IAdd(left, right)));
        }

        private void RewriteCcmp()
        {
            var nzcv = NZCV();
            var tmp = binder.CreateTemporary(PrimitiveType.Bool);
            var cond = Invert(((ConditionOperand<ArmCondition>)instr.Operands[3]).Condition);
            m.Assign(tmp, this.TestCond(cond));
            var c = ((Constant)instr.Operands[2]).ToUInt32();
            m.Assign(nzcv, Constant.Word32(c << 28));
            m.BranchInMiddleOfInstruction(tmp, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            m.Assign(nzcv, m.Cond(m.ISub(left, right)));
        }

        private void RewriteClz()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(CommonOps.CountLeadingZeros, src));
        }

        private void RewriteCmp()
        {
            var left = RewriteOp(0);
            var right = RewriteOp(1);
            right = MaybeExtendExpression(right, left.DataType.BitSize);
            var nzcv = NZCV();
            m.Assign(nzcv, m.Cond(m.ISub(left, right)));
        }

        private void RewriteCsel()
        {
            var srcTrue = RewriteOp(instr.Operands[1], true);
            var srcFalse = RewriteOp(instr.Operands[2], true);
            var dst = RewriteOp(instr.Operands[0]);
            var cond = ((ConditionOperand<ArmCondition>)instr.Operands[3]).Condition;
            m.Assign(dst, m.Conditional(dst.DataType, TestCond(cond), srcTrue, srcFalse));
        }

        private void RewriteCsinc()
        {
            var dst = RewriteOp(instr.Operands[0]);
            var rTrue = (RegisterStorage)instr.Operands[1];
            var rFalse = (RegisterStorage)instr.Operands[2];
            var cond = ((ConditionOperand<ArmCondition>)instr.Operands[3]).Condition;
            Expression src;
            if (rTrue.Number == 31 && rFalse.Number == 31)
            {
                src = TestCond(Invert(cond));
                m.Assign(dst, m.Convert(src, src.DataType, dst.DataType));
                return;
            }
            src = RewriteOp(instr.Operands[1]);
            if (rFalse.Number != 31 && rTrue == rFalse)
            {
                m.BranchInMiddleOfInstruction(TestCond(Invert(cond)), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                m.Assign(dst, m.IAdd(src, 1));
                return;
            }
            var srcTrue = RewriteOp(instr.Operands[1], true);
            var srcFalse = RewriteOp(instr.Operands[2], true);
            m.Assign(dst, m.Conditional(dst.DataType, TestCond(cond), srcTrue, m.IAdd(srcFalse, 1)));
        }

        private void RewriteCsinv()
        {
            var srcTrue = RewriteOp(instr.Operands[1], true);
            var srcFalse = RewriteOp(instr.Operands[2], true);
            var dst = RewriteOp(instr.Operands[0]);
            var cond = ((ConditionOperand<ArmCondition>)instr.Operands[3]).Condition;
            m.Assign(dst, m.Conditional(dst.DataType, TestCond(cond), srcTrue, m.Comp(srcFalse)));
        }

        private void RewriteCsneg()
        {
            var srcTrue = RewriteOp(1, true);
            var srcFalse = RewriteOp(2, true);
            var dst = RewriteOp(instr.Operands[0]);
            var cond = ((ConditionOperand<ArmCondition>)instr.Operands[3]).Condition;
            m.Assign(dst, m.Conditional(dst.DataType, TestCond(cond), srcTrue, m.Neg(srcFalse)));
        }

        private void RewriteLoadAcquire(IntrinsicProcedure intrinsic, DataType dtDst)
        {
            var mem = (MemoryOperand) instr.Operands[1];
            var ptr = new Pointer(dtDst, (int)mem.Base!.BitSize);
            var ea = m.AddrOf(ptr, m.Mem(dtDst, binder.EnsureRegister(mem.Base)));
            var value = m.Fn(intrinsic.MakeInstance(64, dtDst), ea);
            var dst = RewriteOp(0);
            m.Assign(dst, MaybeExtend(value, dst.DataType));
        }

        private void RewriteLoadStorePair(bool load, DataType? dtDst = null, DataType? dtCast = null)
        {
            var reg1 = RewriteOp(instr.Operands[0], !load);
            var reg2 = RewriteOp(instr.Operands[1], !load);
            var mem = (MemoryOperand)instr.Operands[2];
            Expression regBase = binder.EnsureRegister(mem.Base!);
            Expression? offset = RewriteEffectiveAddressOffset(mem);
            Expression ea = regBase;
            dtDst ??= reg1.DataType;
            if (mem.PreIndex)
            {
                m.Assign(ea, m.IAdd(ea, offset!));
            }
            else
            {
                var tmp = binder.CreateTemporary(ea.DataType);
                if (offset is null || mem.PostIndex)
                {
                    m.Assign(tmp, ea);
                }
                else
                {
                    m.Assign(tmp, m.IAdd(ea, offset));
                }
                ea = tmp;
            }
            if (load)
            {
                Expression e = m.Mem(dtDst, ea);
                if (dtCast is not null)
                    e = m.Convert(e, e.DataType, dtCast);
                m.Assign(reg1, e);
            }
            else
            {
                m.Assign(m.Mem(dtDst, ea), reg1);
            }

            ea = m.IAddS(ea, dtDst.Size);
            if (load)
            {
                Expression e = m.Mem(dtDst, ea);
                if (dtCast is not null)
                    e = m.Convert(e, e.DataType, dtCast);
                m.Assign(reg2, e);
            }
            else
            {
                m.Assign(m.Mem(dtDst, ea), reg2);
            }
            if (mem.PostIndex && offset is not null)
            {
                m.Assign(regBase, m.IAdd(regBase, offset));
            }
        }

        private void RewriteLdr(DataType? dt, DataType? dtDst = null)
        {
            var dst = RewriteOp(instr.Operands[0]);
            dtDst ??= dst.DataType;
            Expression ea;
            MemoryOperand? mem = null;
            Identifier? baseReg = null;
            Expression? postIndex = null;
            if (instr.Operands[1] is Address aOp)
            {
                ea = aOp;
            }
            else
            {
                mem = (MemoryOperand)instr.Operands[1];
                (ea, baseReg) = RewriteEffectiveAddress(mem);
                if (mem.PreIndex)
                {
                    m.Assign(baseReg, ea);
                    ea = baseReg;
                } else if (mem.PostIndex)
                {
                    postIndex = ea;
                    ea = baseReg;
                }
            }
            if (dt is null)
            {
                m.Assign(dst, m.Mem(dtDst, ea));
            }
            else
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Mem(dt, ea));
                m.Assign(dst, m.Convert(tmp, tmp.DataType, dtDst));
            }
            if (postIndex is not null)
            {
                m.Assign(baseReg!, postIndex);
            }
        }

        private void RewriteLdx(DataType dt)
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            dst.DataType = dt;
            var tmp = binder.CreateTemporary(PrimitiveType.Create(Domain.Pointer, dst.DataType.BitSize));
            m.Assign(tmp, m.AddrOf(tmp.DataType, src));
            var value = m.Fn(intrinsic.load_exclusive.MakeInstance(64, dt), tmp);
            m.Assign(dst, this.MaybeExtend(value, dt));
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> fn)
        {
            if (instr.Operands.Length == 2 && instr.Operands[1] is Constant immOp)
            {
                // fn Vector,immediate
                var v = (VectorRegisterOperand) instr.Operands[0];
                var eType = Bitsize(v.ElementType);
                var c = ReplicateSimdConstant(v, eType, 1);
                var reg = RewriteOp(0);
                AssignSimd(0, fn(reg, c));
            }
            else
            {
                RewriteBinary(fn);
            }
        }

        private void RewriteMaddSub(Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOp(instr.Operands[1]);
            var op2 = RewriteOp(instr.Operands[2]);
            var op3 = RewriteOp(instr.Operands[3]);
            var dst = RewriteOp(instr.Operands[0]);

            m.Assign(dst, op(op3, m.IMul(op1, op2)));
        }

        private void RewriteMaddl(PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            var op1 = RewriteOp(instr.Operands[1]);
            var op2 = RewriteOp(instr.Operands[2]);
            var op3 = RewriteOp(instr.Operands[3]);
            var dst = RewriteOp(instr.Operands[0]);

            var product = mul(op1, op2);
            m.Assign(dst, m.IAdd(op3, m.Convert(product, product.DataType, dt)));
        }

        private void RewriteMov()
        {
            if (instr.Operands[0] is VectorRegisterOperand v && v.Index >= 0)
            {
                RewriteVectorElementStore(v);
            }
            else
            {
                RewriteUnary(n => n);
            }
        }

        private void RewriteVectorElementStore(VectorRegisterOperand v)
        {
            var src = RewriteOp(1, true);
            var dst = binder.EnsureRegister(v.VectorRegister);
            var eType = PrimitiveType.CreateWord(Bitsize(v.ElementType));
            if (eType.BitSize < src.DataType.BitSize)
            {
                var t = binder.CreateTemporary(eType);
                m.Assign(t, m.Slice(src, eType, 0));
                src = t;
            }
            m.Assign(dst, m.Dpb(dst, src, v.Index * eType.BitSize));
        }

        private void RewriteMovk()
        {
            var dst = (Identifier)RewriteOp(0);
            var imm = (Constant)instr.Operands[1];
            var shift = (Constant)instr.ShiftAmount!;
            m.Assign(dst, m.Dpb(dst, imm, shift.ToInt32()));
        }

        private void RewriteMovn()
        {
            var src = ((Constant)instr.Operands[1]).ToUInt64();
            var dst = (Identifier) RewriteOp(instr.Operands[0]);
            var shift = ((Constant)instr.ShiftAmount!).ToInt32();
            m.Assign(dst, Constant.Create(dst.DataType, ~(src << shift)));
        }

        private void RewriteMovz()
        {
            var dst = RewriteOp(instr.Operands[0]);
            var imm = (Constant)instr.Operands[1];
            var shift = (Constant)instr.ShiftAmount!;
            m.Assign(dst, Constant.Word(dst.DataType.BitSize, imm.ToUInt64() << shift.ToInt32()));
        }

        private void RewriteMull(
            PrimitiveType dtFrom,
            PrimitiveType dtTo, 
            BinaryOperator mul)
        {
            if (instr.Operands[1] is VectorRegisterOperand)
            {
                RewriteSimdBinaryWiden(intrinsic.mull, Domain.Integer);
                return;
            }
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);

            var product = m.Bin(mul, op1, op2);
            m.Assign(dst, m.Convert(product, dtFrom, dtTo));
        }

        private void RewriteMulh(PrimitiveType dtNarrow, PrimitiveType dtWide, BinaryOperator mul)
        {
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);

            var product = m.Bin(mul, dtWide, op1, op2);
            m.Assign(dst, m.Slice(product, dtNarrow, dtWide.BitSize - dtNarrow.BitSize));
        }

        /// <summary>
        /// Given a memory operand, returns the effective address of the memory
        /// access and the base register.
        /// </summary>
        private (Expression, Identifier) RewriteEffectiveAddress(MemoryOperand mem)
        {
            Identifier baseReg = binder.EnsureRegister(mem.Base!);
            Expression ea = baseReg;
            Expression? offset = RewriteEffectiveAddressOffset(mem);
            if (offset is not null)
            {
                ea = m.IAdd(ea, offset);
            }
            return (ea, baseReg);
        }

        private Expression? RewriteEffectiveAddressOffset(MemoryOperand mem)
        { 
            if (mem.Offset is not null && !mem.Offset.IsIntegerZero)
            {
                return Constant.Int(mem.Base!.DataType, mem.Offset.ToInt32());
            }
            else if (mem.Index is not null)
            {
                Expression idx = binder.EnsureRegister(mem.Index);
                switch (mem.IndexExtend)
                {
                case Mnemonic.lsl:
                    break;
                case Mnemonic.sxtb:
                    idx = SignExtend(64, PrimitiveType.SByte, idx);
                    break;
                case Mnemonic.sxth:
                    idx = SignExtend(64, PrimitiveType.Int16, idx);
                    break;
                case Mnemonic.sxtw:
                    idx = SignExtend(64, PrimitiveType.Int32, idx);
                    break;
                case Mnemonic.uxtb:
                    idx = ZeroExtend(64, PrimitiveType.UInt8, idx);
                    break;
                case Mnemonic.uxth:
                    idx = ZeroExtend(64, PrimitiveType.UInt16, idx);
                    break;
                case Mnemonic.uxtw:
                    idx = ZeroExtend(64, PrimitiveType.UInt32, idx);
                    break;
                case Mnemonic.sxtx:
                case Mnemonic.uxtx:
                    break;
                default:
                    throw new NotImplementedException($"Register extension {mem.IndexExtend} not implemented yet.");
                }
                if (mem.IndexShift != 0)
                {
                    idx = m.Shl(idx, mem.IndexShift);
                }
                return idx;
            }
            return null;
        }

        private Expression ZeroExtend(int bitsizeDst, PrimitiveType dtOrig, Expression e)
        {
            var dtUint = PrimitiveType.Create(Domain.UnsignedInt, bitsizeDst);
            if (e.DataType.BitSize > dtOrig.BitSize)
            {
                e = m.Slice(e, dtOrig);
            }
            return m.Convert(e, dtOrig, dtUint);
        }

        private Expression SignExtend(int bitsizeDst, PrimitiveType dtOrig, Expression e)
        {
            var dtInt = PrimitiveType.Create(Domain.SignedInt, bitsizeDst);
            if (e.DataType.BitSize > dtOrig.BitSize)
            {
                e = m.Slice(e, dtOrig);
            }
            return m.Convert(e, dtOrig, dtInt);
        }

        private void RewritePrfm()
        {
            var imm = (Constant)instr.Operands[0];
            Expression ea;
            if (instr.Operands[1] is Address addr)
            {
                ea = addr;
            }
            else if (instr.Operands[1] is MemoryOperand mem)
            {
                (ea, _) = RewriteEffectiveAddress(mem);
            }
            else
                throw new AddressCorrelatedException(instr.Address, "Expected an address as the second operand of prfm.");
            m.SideEffect(m.Fn(intrinsic.prfm.MakeInstance(64, imm.DataType), imm, ea));
        }

        private void RewriteRbit()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(CommonOps.ReverseBits, src));
        }

        private void RewriteRev()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            var bitSize = dst.DataType.BitSize;
            m.Assign(dst, m.Fn(CommonOps.ReverseBytes, src));
        }


        private void RewriteRev32()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(CommonOps.ReverseWords, src));
        }

        private void RewriteRev16()
        {
            RewriteMaybeSimdUnary(
                n => m.Fn(CommonOps.ReverseHalfwords, n),
                intrinsic.rev16);
        }

        private void RewriteRor()
        {
            RewriteBinary((a, b) => m.Fn(CommonOps.Ror, a, b));
        }

        private void RewriteSbfiz()
        {
            var src1 = RewriteOp(1, true);
            var src2 = RewriteOp(2, true);
            var src3 = RewriteOp(3, true);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.sbfiz.MakeInstance(src1.DataType), src1, src2, src3));
        }

        private void RewriteUSbfm(IntrinsicProcedure intrinsic)
        {
            var src1 = RewriteOp(1, true);
            var src2 = RewriteOp(2, true);
            var src3 = RewriteOp(2, true);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(src1.DataType), src1, src2, src3));
        }

        private void RewriteStlr(DataType dataType)
        {
            var src1 = RewriteOp(0, false);
            var ea = binder.CreateTemporary(new Pointer(dataType, arch.PointerType.BitSize));
            m.Assign(ea, m.AddrOf(ea.DataType, m.Mem(dataType, binder.EnsureRegister(((MemoryOperand) instr.Operands[1]).Base!))));
            m.SideEffect(m.Fn(intrinsic.stlr.MakeInstance(64, src1.DataType), ea, src1));
        }

        private void RewriteStr(PrimitiveType? dt)
        {
            var rSrc = (RegisterStorage)instr.Operands[0];
            Expression src = MaybeZeroRegister(rSrc, dt ?? rSrc.DataType);
            var mem = (MemoryOperand)instr.Operands[1];
            var (ea, baseReg) = RewriteEffectiveAddress(mem);
            Expression? postIndex = null;
            if (mem.PreIndex)
            {
                m.Assign(baseReg, ea);
                ea = baseReg;
            }
            else if (mem.PostIndex)
            {
                postIndex = ea;
                ea = baseReg;
            }
            if (dt is null || src is Constant)
            {
                m.Assign(m.Mem(src.DataType, ea), src);
            }
            else
            {
                m.Assign(m.Mem(dt, ea), m.Slice(src, dt));
            }
            if (postIndex is not null)
            {
                m.Assign(baseReg, postIndex);
            }
        }

        private void RewriteStx(DataType dt)
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(2);
            dst.DataType = dt;
            var tmp = binder.CreateTemporary(PrimitiveType.Create(Domain.Pointer, dst.DataType.BitSize));
            var success = RewriteOp(0);
            m.Assign(tmp, m.AddrOf(tmp.DataType, dst));
            m.Assign(success, m.Fn(intrinsic.stx.MakeInstance(64, dst.DataType), tmp, src));
        }

        private void RewriteTest()
        {
            var op1 = RewriteOp(0, true);
            var op2 = RewriteOp(1, true);
            NZ00(m.Cond(m.And(op1, op2)));
        }

        private void RewriteUnary(Func<Expression, Expression> fn)
        {
            var src = RewriteOp(1, true);
            var dst = RewriteOp(0);
            m.Assign(dst, fn(src));
        }

        private void RewriteUSxt(Domain domDst, int bitSize)
        {
            var src = RewriteOp(instr.Operands[1], true);
            var dst = RewriteOp(instr.Operands[0]);
            var dtSrc = PrimitiveType.Create(domDst, bitSize);
            if (src.DataType.BitSize > bitSize)
            {
                src = m.Slice(src, dtSrc);
            }
            var dtDst = MakeInteger(domDst, dst.DataType);
            m.Assign(dst, m.Convert(src, dtSrc, dtDst));
        }
    }
}
