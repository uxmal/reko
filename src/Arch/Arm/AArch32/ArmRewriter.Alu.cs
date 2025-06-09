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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> opr, bool reverse)
        {
            Expression opDst = this.Operand(0, PrimitiveType.Word32, true);
            Expression opSrc1;
            Expression opSrc2;
            if (instr.Operands.Length == 3)
            {
                opSrc1 = this.Operand(1);
                opSrc2 = this.Operand(2);
            }
            else
            {
                opSrc1 =  this.Operand(0, PrimitiveType.Word32);
                opSrc2 = this.Operand(1);
            }
            if (reverse)
            {
                var tmp = opSrc1;
                opSrc1 = opSrc2;
                opSrc2 = tmp;
            }
            // We do not take the trouble of widening the CF to the word size
            // to simplify code analysis in later stages. 
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(
                opDst,
                opr(
                    opr(opSrc1, opSrc2),
                    c));
            MaybeUpdateFlags(opDst);
        }

        private void RewriteAddw()
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src1 = Operand(1);
            var src2 = Operand(2);
            m.Assign(dst, m.IAdd(src1, src2));
        }

        static class Bits
        {
            public
                static UInt32 Mask32(int lsb, int bitsize)
            {
                return ((1u << bitsize) - 1) << lsb;
            }
        };

        private void RewriteAdr()
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src = (Address)instr.Operands[1];
            m.Assign(dst, src);
        }

        private void RewriteBfc()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var lsb = ((Constant)instr.Operands[1]).ToInt32();
            var bitsize = ((Constant)instr.Operands[2]).ToInt32();
            m.Assign(opDst, m.And(opDst, Constant.UInt32(~Bits.Mask32(lsb, bitsize))));
        }

        private void RewriteBfi()
        {
            var opSrc = this.Operand(1);
            var opDst = (Identifier) this.Operand(0, PrimitiveType.Word32, true);
            var lsb = ((Constant)instr.Operands[2]).ToInt32();
            var bitsize = ((Constant)instr.Operands[3]).ToInt32();
            var tmp = binder.CreateTemporary(PrimitiveType.CreateWord(bitsize));
            m.Assign(tmp, m.Slice(opSrc, 0, bitsize));
            m.Assign(opDst, m.Dpb(opDst, tmp, lsb));
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> op)
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
            {
                var src1 = this.Operand(1);
                var src2 = this.Operand(2);
                m.Assign(opDst, op(src1, src2));
            }
            else
            {
                var dst = Operand(0, PrimitiveType.Word32, true);
                var src = Operand(1);
                m.Assign(dst, op(dst, src));
            }
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteLogical(Func<Expression, Expression, Expression> cons)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
            {
                var opSrc1 = this.Operand(1);
                var opSrc2 = this.Operand(2);
                m.Assign(dst, cons(opSrc1, opSrc2));
            }
            else
            {
                var src = Operand(1);
                m.Assign(dst, cons(dst, src));
            }
            if (instr.SetFlags)
            {
                m.Assign(NZC(), m.Cond(dst));
            }
        }

        private void RewriteRev(PrimitiveType dt)
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var intrinsic = rev_intrinsic.MakeInstance(dt);
            var application = m.Fn(intrinsic, this.Operand(1));
            m.Assign(opDst, application);
        }

        private void RewriteRevsh()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var intrinsic = m.Convert(
                m.Fn(
                    rev_intrinsic.MakeInstance(PrimitiveType.Word16),
                    m.Slice(this.Operand(1), PrimitiveType.Word16)),
                PrimitiveType.Word16,
                PrimitiveType.Int32);
            m.Assign(opDst, intrinsic);
        }

        private void RewriteRevBinOp(Func<Expression, Expression, Expression> op, bool setflags)
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            if (instr.Operands.Length > 2)
            {
                var src1 = this.Operand(1);
                var src2 = this.Operand(2);
                m.Assign(opDst, op(src2, src1));
            }
            else
            {
                var dst = Operand(0, PrimitiveType.Word32, true);
                var src = Operand(1);
                m.Assign(dst, op(dst, src));
            }
            if (setflags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private Expression Bic(Expression a, Expression b)
        {
            return m.And(a, m.Comp(b));
        }

        private void RewriteClz()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc = this.Operand(1);
            var intrinsic = m.Fn(CommonOps.CountLeadingZeros, opSrc);
            m.Assign(opDst, intrinsic);
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src = Operand(1);
            var flags = binder.EnsureFlagGroup(Registers.NZCV);
            m.Assign(flags, m.Cond(op(dst, src)));
        }

        private void RewriteCrc(IntrinsicProcedure intrinsic, DataType dt)
        {
            var src1 = this.Operand(1);
            var src2 = this.Operand(2);
            if (src1.DataType.BitSize > dt.BitSize)
            {
                src2 = EmitNarrowingSlice(src2, dt);
            }
            var dst = this.Operand(0);
            var src = m.Fn(intrinsic.MakeInstance(dt), src1, src2);
            m.Assign(dst, src);
        }


        private void RewriteDiv(Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src1 = Operand(1);
            var src2 = Operand(2);
            m.Assign(dst, op(src1, src2));
        }

        private void RewriteTableBranch(DataType elemSize)
        {
            this.iclass = InstrClass.Transfer;
            var mem = (MemoryOperand) instr.Operands[0];
            Expression tableBase;
            if (mem.BaseRegister == Registers.pc)
            {
                // If the base register is PC, the table starts right after the instruction.
                tableBase = instr.Address + instr.Length;
            }
            else
            {
                tableBase = binder.EnsureRegister(mem.BaseRegister!);
            }
            var idxReg = binder.EnsureRegister(mem.Index!);
            Expression ea;
            if (elemSize.Size != 1)
            {
                ea = m.IAdd(tableBase, m.IMul(idxReg, elemSize.Size));
            }
            else
            {
                ea = m.IAdd(tableBase, idxReg);
            }
            m.Goto(m.IAdd(
                instr.Address + this.pcValueOffset,
                m.IMul(
                    m.Convert(m.Mem(elemSize, ea), elemSize, PrimitiveType.UInt32),
                    2)));
        }

        private void RewriteTeq()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc = this.Operand(1);
            m.Assign(
                NZC(),
                m.Cond(m.Xor(opDst, opSrc)));
        }

        private void RewriteTst()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc = this.Operand(1);
            m.Assign(
                NZC(),
                m.Cond(m.And(opDst, opSrc)));
        }

        private void RewriteLoadAcquire(IntrinsicProcedure intrinsic, DataType dt)
        {
            var mem = (MemoryOperand) instr.Operands[1];
            var ea = binder.EnsureRegister(mem.BaseRegister!);
            var dst = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            intrinsic = intrinsic.MakeInstance(32, dt);
            Expression src = m.Fn(intrinsic, ea);
            if (src.DataType.BitSize != dst.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, src);
                src = m.Convert(tmp, dt, dst.DataType);
            }
            m.Assign(dst, src);
        }

        private void RewriteLoadAcquireDouble(IntrinsicProcedure intrinsic)
        {
            var mem = (MemoryOperand) instr.Operands[2];
            var ea = binder.EnsureRegister(mem.BaseRegister!);
            ea.DataType = new Pointer(PrimitiveType.Word64, 32);
            var regHi = (RegisterStorage) instr.Operands[0];
            var regLo = (RegisterStorage) instr.Operands[1];
            var dst = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var src = m.Fn(intrinsic.MakeInstance(32, dst.DataType), ea);
            m.Assign(dst, src);
        }

        private void RewriteLdr(PrimitiveType dtDst, PrimitiveType dtSrc)
        {
            var mem = (MemoryOperand)instr.Operands[1];
            var rDst = (RegisterStorage)instr.Operands[0];
            bool isJump = rDst == Registers.pc;

            Expression dst = Operand(0, PrimitiveType.Word32, true);
            Expression src;
            if (mem.PreIndex && instr.Writeback)
            {
                // Pre-index operand.
                Expression baseReg = Reg(mem.BaseRegister!);
                Expression ea = EffectiveAddress(mem);
                m.Assign(baseReg, ea);
                src = m.Mem(dtSrc, baseReg);
            }
            else
            {
                src = Operand(1, dtSrc);
            }
            if (dtDst != dtSrc)
            {
                src = m.Convert(src, dtSrc, dtDst);
            }
            if (!mem.PreIndex && instr.Writeback)
            {
                // Post-index operand.
                var tmp = binder.CreateTemporary(dtDst);
                m.Assign(tmp, src);
                var baseReg = binder.EnsureRegister(mem.BaseRegister!);
                if (isJump && instr.IsSinglePop())
                {
                    //$TODO: this is a cheat; we could be popping
                    // something other than the LR (or continuation)
                    // of this procedure. That requires more advanced 
                    // analyses than Reko can manage presently.
                    iclass = instr.Condition == ArmCondition.AL
                        ? InstrClass.Transfer
                        : InstrClass.ConditionalTransfer;
                    m.Assign(baseReg, m.IAdd(baseReg, mem.Offset!));
                    m.Return(0, 0);
                    return;
                }
                Expression ea = baseReg;
                if (mem.Offset is not null)
                {
                    ea = m.IAdd(baseReg, mem.Offset);
                }
                m.Assign(baseReg, ea);
                src = tmp;
            }
            if (isJump)
            {
                iclass = InstrClass.Transfer;
                m.Goto(src);
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private void RewriteLdrd()
        {
            var regLo = (RegisterStorage)instr.Operands[0];
            var regHi = (RegisterStorage)instr.Operands[1];
            var opDst = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var opSrc = this.Operand(2);
            m.Assign(opDst, opSrc);
            MaybePostOperand(2);
        }

        private void RewriteStoreRelease(IntrinsicProcedure intrinsic, PrimitiveType dt)
        {
            var src = MaybeSlice(
                binder.EnsureRegister((RegisterStorage) instr.Operands[0]),
                dt);
            var mem = (MemoryOperand) instr.Operands[1];
            var ea = binder.EnsureRegister(mem.BaseRegister!);
            ea.DataType = new Pointer(dt, 32);
            var store = m.Fn(intrinsic.MakeInstance(32, dt), ea, src);
            m.SideEffect(store);
        }

        private void RewriteStoreExclusive(IntrinsicProcedure intrinsic, PrimitiveType dt)
        {
            var src = MaybeSlice(
                binder.EnsureRegister((RegisterStorage) instr.Operands[1]),
                dt);
            var mem = (MemoryOperand) instr.Operands[2];
            var ea = binder.EnsureRegister(mem.BaseRegister!);
            ea.DataType = new Pointer(dt, 32);
            var store = m.Fn(intrinsic.MakeInstance(32, dt), ea, src);
            var dst = Operand(0);
            m.Assign(dst, store);
        }

        private void RewriteStoreReleaseDoubleExclusive(IntrinsicProcedure intrinsic)
        {
            var mem = (MemoryOperand) instr.Operands[3];
            var ea = binder.EnsureRegister(mem.BaseRegister!);
            ea.DataType = new Pointer(PrimitiveType.Word64, 32);
            var regHi = (RegisterStorage) instr.Operands[1];
            var regLo = (RegisterStorage) instr.Operands[2];
            var src = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var store = m.Fn(intrinsic.MakeInstance(32, src.DataType), ea, src);
            var dst = Operand(0);
            m.Assign(dst, store);
        }

        private void RewriteStr(PrimitiveType size)
        {
            var opSrc = MaybeSlice(
                this.Operand(0, PrimitiveType.Word32, true),
                size);
            var opDst = this.Operand(1);
            m.Assign(opDst, opSrc);
            MaybePostOperand(1);
        }

        private void RewriteStrd()
        {
            var regLo = (RegisterStorage)instr.Operands[0];
            var regHi = (RegisterStorage)instr.Operands[1];
            var opSrc = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            var opDst = this.Operand(2);
            m.Assign(opDst, opSrc);
            MaybePostOperand(2);
        }

        private void RewriteSubw()
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src1 = Operand(1);
            var src2 = Operand(2);
            m.Assign(dst, m.ISub(src1, src2));
        }

        private void RewriteSwp(PrimitiveType type)
        {
            var src1 = Operand(1);
            if (src1.DataType.BitSize > type.BitSize)
            {
                src1 = EmitNarrowingSlice(src1, type);
            }
            Expression result = m.Fn(
                AtomicOps.atomic_exchange.MakeInstance(32, type),
                m.AddrOf(new Pointer(type, 32), Operand(2)),
                src1);
            var dst = Operand(0, PrimitiveType.Word32, true);
            if (dst.DataType.BitSize > type.BitSize)
            {
                var tmp = binder.CreateTemporary(type);
                m.Assign(tmp, result);
                result = m.Convert(tmp, type, dst.DataType);
            }
            m.Assign(dst, result);
        }

        private void RewriteMultiplyAccumulate(Func<Expression, Expression, Expression> op)
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(1);
            var opSrc2 = this.Operand(2);
            var opSrc3 = this.Operand(3);
            m.Assign(opDst, op(opSrc3, m.IMul(opSrc1, opSrc2)));
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteHint()
        {
            var ldrex = m.Fn(ldrex_intrinsic.MakeInstance(32, PrimitiveType.Word32), m.AddrOf(PrimitiveType.Ptr32, Operand(0)));
            m.SideEffect(m.Fn(ldrex));
        }

        private void RewriteLdm(int initialOffset, Func<Expression, Expression, Expression> op)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            RewriteLdm(dst, 1, initialOffset, op, instr.Writeback);
        }

        private void RewriteLdm(Expression dst, int skip, int offset, Func<Expression, Expression, Expression> op, bool writeback)
        {
            bool pcRestored = false;
            var mop = (MultiRegisterOperand)instr.Operands[instr.Operands.Length > 1 ? 1 : 0];
            foreach (var r in mop.GetRegisters())
            {
                Expression ea = offset != 0
                    ? op(dst, m.Int32(offset))
                    : dst;
                Identifier dstReg;
                if (r == Registers.pc)
                {
                    pcRestored = true;
                    // We must simulate the effect of reading the PC out of memory,
                    // even though we don't use the PC register explicitly
                    dstReg = binder.CreateTemporary(PrimitiveType.Word32);
                }
                else
                {
                    dstReg = Reg(r);
                }
                m.Assign(dstReg, m.Mem32(ea));
                offset += 4;
            }
            if (writeback)
            {
                m.Assign(dst, m.IAddS(dst, offset));
            }
            if (pcRestored)
            {
                iclass = instr.Condition == ArmCondition.AL
                    ? InstrClass.Transfer
                    : InstrClass.ConditionalTransfer;
                m.Return(0, 0);
            }
        }

        private void RewriteLdrex(PrimitiveType dt)
        {
            m.Assign(Operand(0), m.Fn(ldrex_intrinsic.MakeInstance(32, dt), Operand(1)));
        }

        private void RewriteLdrexd()
        {
            var regLo = (RegisterStorage) instr.Operands[0];
            var regHi = (RegisterStorage) instr.Operands[1];
            var opDst = binder.EnsureSequence(PrimitiveType.Word64, regHi, regLo);
            m.Assign(opDst, m.Fn(ldrex_intrinsic.MakeInstance(32, PrimitiveType.Word64), this.Operand(2)));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> ctor)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src1 = Operand(1);
            Expression src2;
            if (instr.Operands.Length == 2)
            {
                src2 = src1;
                src1 = dst;
            }
            else
            {
                src2 = Operand(2);
            }
            m.Assign(dst, ctor(src1, src2));
            if (instr.SetFlags)
            {
                m.Assign(this.NZC(), m.Cond(dst));
            }
        }

        private Expression Ror(Expression left, Expression right)
        {
            var intrinsic = m.Fn(CommonOps.Ror, left, right);
            return intrinsic;
        }

        private Expression Rrx(Expression left, Expression right)
        {
            var intrinsic = m.Fn(
                CommonOps.RorC.MakeInstance(left.DataType, right.DataType),
                left, right, C());
            return intrinsic;
        }

        private void RewriteMla(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> op)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);

            var left = Operand(1);
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Convert(left, left.DataType, dt);

            var right = Operand(2);
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Convert(right, right.DataType, dt);

            m.Assign(dst, m.IAdd(op(left, right), Operand(3)));
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteMov()
        {
            if (instr.Operands[0] is RegisterStorage rOp && rOp == Registers.pc)
            {
                if (instr.Operands[1] is RegisterStorage ropSrc && ropSrc == Registers.lr)
                {
                    m.Return(0, 0);
                }
                else if (instr.Operands[1] is Constant imm)
                {
                    m.Goto(arch.MakeAddressFromConstant(imm, true));
                }
                else
                {
                    m.Goto(Operand(1));
                }
                return;
            }
            var opDst = Operand(0, PrimitiveType.Word32, true);
            var opSrc = Operand(1);
            m.Assign(opDst, opSrc);
            if (instr.SetFlags)
            {
                m.Assign(NZC(), m.Cond(opDst));
            }
        }

        private void RewriteMovt()
        {
            var iSrc = (Constant) instr.Operands[1];
            var opDst = (Identifier) Operand(0, PrimitiveType.Word32, true);
            Debug.Assert(iSrc.DataType.BitSize == 16);
            var opSrc = m.Dpb(opDst, iSrc, 16);
            m.Assign(opDst, opSrc);
        }

        private void RewriteMovw()
        {
            var src = m.Word32(((Constant)instr.Operands[1]).ToUInt32());
            var dst = Operand(0, PrimitiveType.Word32, true);
            m.Assign(dst, src);
        }

        private void RewriteMulbb(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> mul)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);

            var left = Operand(1);
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Convert(left, left.DataType, dt);

            var right = Operand(2);
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Convert(right, right.DataType, dt);

            m.Assign(dst, mul(left, right));
        }

        private void RewriteMull(PrimitiveType dtResult, Func<Expression, Expression, Expression> op)
        {
            var regLo = (RegisterStorage)instr.Operands[0];
            var regHi = (RegisterStorage)instr.Operands[1];

            var opDst = binder.EnsureSequence(dtResult, regHi, regLo);
            var opSrc1 = this.Operand(3);
            var opSrc2 = this.Operand(2);
            var result = op(opSrc1, opSrc2);
            result.DataType = dtResult;
            m.Assign(opDst, result);
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewritePk(IntrinsicProcedure intrinsic)
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(intrinsic, src1, src2));
        }

        private void RewritePld(IntrinsicProcedure intrinsic)
        {
            var dst = ((MemoryAccess) this.Operand(0)).EffectiveAddress;
            m.SideEffect(m.Fn(intrinsic, dst));
        }

        private void RewritePop()
        {
            var sp = Reg(Registers.sp);
            RewriteLdm(sp, 0, 0, m.IAdd, true);
        }

        private void RewritePush()
        {
            Expression dst = Reg(Registers.sp);
            var regs = ((MultiRegisterOperand)instr.Operands[0]).GetRegisters().ToArray();
            m.Assign(dst, m.ISubS(dst, regs.Length * 4));

            int offset = 0;
            foreach (var reg in regs)
            { 
                var ea = offset != 0
                    ? m.IAddS(dst, offset)
                    : dst;
                m.Assign(m.Mem32(ea), Reg(reg));
                offset += 4;
            }
        }

        private void RewriteQAddSub(IntrinsicProcedure intrinsic)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var src1 = Operand(1);
            var src2 = Operand(2);
            var sat = m.Fn(intrinsic.MakeInstance(PrimitiveType.Int32), src1, src2);
            m.Assign(dst, sat);
            m.Assign(
                Q(),
                m.Cond(dst));
        }

        private void RewriteQDAddSub(IntrinsicProcedure  op)
        {
            var dst = Operand(0, PrimitiveType.Word32, true);
            var sat = m.Fn(CommonOps.SatMul.MakeInstance(PrimitiveType.Int32), Operand(2), m.Int32(2));
            Expression src2 = sat;
            var src1 = Operand(1);
            var sum = m.Fn(op.MakeInstance(PrimitiveType.Int32), src1, src2);
            m.Assign(dst, sum);
            m.Assign(
                Q(),
                m.Cond(dst));
        }

        private void RewriteSbfx()
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var src = m.Slice(
                    this.Operand(1),
                    ((Constant)instr.Operands[2]).ToInt32(),
                    ((Constant)instr.Operands[3]).ToInt32());
            m.Assign(dst, m.Convert(src, src.DataType, PrimitiveType.Int32));
        }

        private void RewriteSmlal()
        {
            var r1 = (RegisterStorage)instr.Operands[0];
            var r2 = (RegisterStorage)instr.Operands[1];
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);
            var fac1 = Operand(2);
            var fac2 = Operand(3);
            m.Assign(dst, m.IAdd(m.SMul(fac1, fac2), dst));
        }

        private void RewriteMlal(bool hiLeft, bool hiRight, PrimitiveType dt, Func<Expression, Expression, Expression> op)
        {
            var r1 = (RegisterStorage)instr.Operands[0];
            var r2 = (RegisterStorage)instr.Operands[1];
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);

            var left = Operand(2);
            left = hiLeft ? m.Sar(left, m.Int32(16)) : left;
            left = m.Convert(left, left.DataType, dt);

            var right = Operand(3);
            right = hiRight ? m.Sar(right, m.Int32(16)) : right;
            right = m.Convert(right, right.DataType, dt);

            m.Assign(dst, m.IAdd(op(left, right), dst));
        }

        private void RewriteMlxd(bool swap, PrimitiveType dt, Func<Expression, Expression, Expression> mul, Func<Expression, Expression, Expression> addSub)
        {
            // The ARM manual states that the double return value is in [op2,op1]
            var r1 = (RegisterStorage)instr.Operands[1];
            var r2 = (RegisterStorage)instr.Operands[0];
            var dst = binder.EnsureSequence(PrimitiveType.Int64, r1, r2);

            var left = Operand(2);
            var right = Operand(3);

            var product1 = mul(
                m.Convert(left, left.DataType, dt),
                swap ? m.Sar(right, m.Int32(16)) : (Expression)m.Convert(right, right.DataType, dt));
            var product2 = mul(
                m.Sar(left, m.Int32(16)),
                swap ? m.Convert(right, right.DataType, dt) : (Expression)m.Sar(right, m.Int32(16)));

            m.Assign(dst, m.IAdd(dst, addSub(product1, product2)));
        }

        private void RewriteMxd(bool swap, PrimitiveType dt, Func<Expression, Expression, Expression> mul, Func<Expression, Expression, Expression> addSub)
        {
            var dst = Operand(0);
            var left = Operand(1);
            var right = Operand(2);

            var product1 = mul(
                m.Convert(left, left.DataType, dt),
                swap ? m.Sar(right, m.Int32(16)) : (Expression)m.Convert(right, right.DataType, dt));
            var product2 = mul(
                m.Sar(left, m.Int32(16)),
                swap ? m.Convert(right, right.DataType, dt) : (Expression)m.Sar(right, m.Int32(16)));

            m.Assign(dst, m.IAdd(dst, addSub(product1, product2)));
        }

        private void RewriteSmlaw(bool highPart)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var fac1 = this.Operand(1);
            var fac2 = this.Operand(2);
            var src = highPart ? m.Sar(fac2, m.Int32(16)) : fac2;
            fac2 = m.Convert(src, src.DataType, PrimitiveType.Int16); 

            var acc = this.Operand(3);
            m.Assign(dst, m.IAdd(
                m.Sar(
                    m.SMul(fac1, fac2),
                    m.Int32(16)),
                acc));
        }

        private void RewriteSmml(Func<Expression,Expression,Expression> fn)
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var src3 = Operand(3);
            var dst = Operand(0);
                
            var mul = m.SMul(src1, src2);
            mul.DataType = PrimitiveType.Int64;
            var src = m.Sar(mul, m.Int32(32));
            m.Assign(dst, fn(m.Convert(src, src.DataType, PrimitiveType.Int32), src3));
        }

        // Like smml, but rounding
        private void RewriteSmmlr(Func<Expression, Expression, Expression> fn)
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var src3 = Operand(3);
            var dst = Operand(0);

            var mul = m.SMul(src1, src2);
            mul.DataType = PrimitiveType.Int64;
            var src = m.Sar(m.IAdd(mul, 0x80000000), m.Int32(32));
            m.Assign(dst, fn(m.Convert(src, src.DataType, PrimitiveType.Int32), src3));
        }

        private void RewriteSmmul()
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var dst = Operand(0);

            var mul = m.SMul(src1, src2);
            mul.DataType = PrimitiveType.Int64;
            var src = m.Sar(mul, m.Int32(32));
            m.Assign(dst, m.Convert(src, src.DataType, PrimitiveType.Int32));
        }

        private void RewriteSmusd()
        {
            var s16 = PrimitiveType.Int16;
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var rn = this.Operand(1);
            var rm = this.Operand(2);
            var p1 = binder.CreateTemporary(PrimitiveType.Int32);
            var p2 = binder.CreateTemporary(PrimitiveType.Int32);
            m.Assign(p1, m.SMul(m.Slice(rn, s16, 0), m.Slice(rm, s16, 0)));
            m.Assign(p2, m.SMul(m.Slice(rn, s16, 16), m.Slice(rm, s16, 16)));
            m.Assign(dst, m.ISub(p1, p2));
        }

        private void RewriteMulw(bool highPart)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var fac1 = this.Operand(1);
            var fac2 = this.Operand(2);
            var src = highPart ? m.Sar(fac2, m.Int32(16)) : fac2;
            fac2 = m.Convert(src, src.DataType, PrimitiveType.Int16);
            m.Assign(dst, m.Sar(
                m.SMul(fac1, fac2),
                m.Int32(16)));
        }

        private void RewriteHasx(PrimitiveType elemType)
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var diff = binder.CreateTemporary(elemType);
            var sum = binder.CreateTemporary(elemType);
            m.Assign(diff, m.ISub(m.Slice(src1, elemType, 0), m.Slice(src2, elemType, 16)));
            m.Assign(sum, m.ISub(m.Slice(src1, elemType, 16), m.Slice(src2, elemType, 0)));
            var dst = Operand(0);
            m.Assign(dst, m.Seq(sum, diff));
        }

        private void RewriteShsax()
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var diff = binder.CreateTemporary(PrimitiveType.Int16);
            var sum = binder.CreateTemporary(PrimitiveType.Int16);
            m.Assign(sum, m.ISub(m.Slice(src1, PrimitiveType.Int16, 0), m.Slice(src2, PrimitiveType.Int16, 16)));
            m.Assign(diff, m.ISub(m.Slice(src1, PrimitiveType.Int16, 16), m.Slice(src2, PrimitiveType.Int16, 0)));
            var dst = Operand(0);
            m.Assign(dst, m.Seq(diff, sum));
        }

        private void RewriteSsat()
        {
            var dst = this.Operand(0);
            var src1 = this.Operand(1);
            var src2 = this.Operand(2);
            var intrinsic = m.Fn(ssat_intrinsic, src1, src2);
            m.Assign(dst, intrinsic);
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteStm(bool add, bool updateAfter)
        {
            var regs = ((MultiRegisterOperand)instr.Operands[1]).GetRegisters().ToArray();
            if (regs.Length == 0)
            {
                Invalid();
                return;
            }
            var rSrc = this.Operand(0, PrimitiveType.Word32, true);
            int regSize = regs[0].DataType.Size;
            int totalRegsize = regs.Length * regSize;
            int offset;
            if (add)
            {
                offset = updateAfter ? 0 : regSize;
            }
            else
            {
                offset = -totalRegsize;
                if (updateAfter)
                    offset += regSize;
            }
            foreach (var r in regs)
            {
                var dst = Reg(r);
                Expression ea =
                    offset != 0
                    ? m.IAddS(rSrc, offset)
                    : rSrc;
                m.Assign(m.Mem(r.DataType, ea), dst);
                offset += r.DataType.Size;
            }
            if (instr.Writeback)
            {
                if (add)
                {
                    m.Assign(rSrc, m.IAddS(rSrc, totalRegsize));
                }
                else
                {
                    m.Assign(rSrc, m.ISubS(rSrc, totalRegsize));
                }
            }
        }

        private void RewriteStmib()
        {
            throw new NotImplementedException();
            /*
	var dst = this.Operand(0, PrimitiveType.Word32, true);
	var begin = &instr.detail.arm.operands[1];	// Skip the dst register
	var end = begin + instr.detail.arm.op_count - 1;
	int offset = 4;
	for (var r = begin; r != end; ++r)
	{
		var ea = m.IAdd(dst, m.Int32(offset));
		var srcReg = Reg(r.reg);
		m.Assign(m.Mem32(ea), srcReg);
		offset += 4;
	}
	if (offset != 4 && instr.detail.arm.writeback)
	{
		m.Assign(dst, m.IAdd(dst, m.Int32(offset)));
	}
    */
        }

        private void RewriteUasx()
        {
            var diff = binder.CreateTemporary(PrimitiveType.UInt16);
            var sum = binder.CreateTemporary(PrimitiveType.UInt16);
            var rn = Operand(1);
            var rm = Operand(2);
            m.Assign(diff, m.ISub(m.Slice(rn, 0, 16), m.Slice(rm, 16, 16)));
            m.Assign(sum, m.IAdd(m.Slice(rn, 16, 16), m.Slice(rm, 0, 16)));
            var rd = Operand(0);
            m.Assign(rd, m.Seq(sum, diff));
            //$REVIEW: flags?
        }

        private void RewriteUbfx()
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var src = m.Slice(
                    this.Operand(1),
                    ((Constant)instr.Operands[2]).ToInt32(),
                    ((Constant)instr.Operands[3]).ToInt32());
            m.Assign(dst, m.Convert(src, src.DataType, PrimitiveType.UInt32));
        }

        private void RewriteUhsax()
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var sum = binder.CreateTemporary(PrimitiveType.UInt16);
            var diff = binder.CreateTemporary(PrimitiveType.UInt16);
            m.Assign(sum, m.ISub(m.Slice(src1, PrimitiveType.UInt16, 0), m.Slice(src2, PrimitiveType.UInt16, 16)));
            m.Assign(diff, m.ISub(m.Slice(src1, PrimitiveType.UInt16, 16), m.Slice(src2, PrimitiveType.UInt16, 0)));
            var dst = Operand(0);
            m.Assign(dst, m.Seq(diff, sum));
        }

        private void RewriteUmaal()
        {
            var tmp = binder.CreateTemporary(PrimitiveType.UInt64);
            var lo = Operand(0, PrimitiveType.Word32, true);
            var hi = Operand(1);
            var rn = Operand(2);
            var rm = Operand(3);
            var dst = binder.EnsureSequence(
                PrimitiveType.UInt64,
                (RegisterStorage)instr.Operands[1],
                (RegisterStorage)instr.Operands[0]);
            m.Assign(tmp, m.UMul(rn, rm));
            m.Assign(tmp, m.IAdd(tmp, m.Convert(hi, hi.DataType, PrimitiveType.UInt64)));
            m.Assign(dst, m.IAdd(tmp, m.Convert(lo, lo.DataType, PrimitiveType.UInt64)));
        }

        private void RewriteUmlal()
        {
            var dst = binder.EnsureSequence(
                PrimitiveType.Word64,
                (RegisterStorage) instr.Operands[1],
                (RegisterStorage) instr.Operands[0]);
            var left = this.Operand(2);
            var right = this.Operand(3);
            m.Assign(dst, m.IAdd(m.UMul(left, right), dst));
            MaybeUpdateFlags(dst);
        }

        private void RewriteUnaryOp(Func<Expression, Expression> op)
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc = this.Operand(1);
            m.Assign(opDst, op(opSrc));
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        private void RewriteUsad8()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(1);
            var opSrc2 = this.Operand(2);
            var vSrc1 = binder.CreateTemporary(ab_4);
            var vSrc2 = binder.CreateTemporary(ab_4);
            m.Assign(vSrc1, opSrc1);
            m.Assign(vSrc2, opSrc2);
            var intrinsic = m.Fn(usad8_intrinsic.MakeInstance(ab_4), vSrc1, vSrc2);
            m.Assign(opDst, intrinsic);
        }

        private void RewriteUsada8()
        {
            var opDst = this.Operand(0, PrimitiveType.Word32, true);
            var opSrc1 = this.Operand(1);
            var opSrc2 = this.Operand(2);
            var opAcc = this.Operand(3);
            var vSrc1 = binder.CreateTemporary(ab_4);
            var vSrc2 = binder.CreateTemporary(ab_4);
            m.Assign(vSrc1, opSrc1);
            m.Assign(vSrc2, opSrc2);
            var intrinsic = m.Fn(usada8_intrinsic.MakeInstance(ab_4), vSrc1, vSrc2);
            m.Assign(opDst, intrinsic);
        }

        private void RewriteUsat()
        {
            var src1 = this.Operand(1);
            var src2 = this.Operand(2);
            var dst = this.Operand(0);
            var intrinsic = m.Fn(usat_intrinsic, src1, src2);
            m.Assign(dst, intrinsic);
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteSat16(IntrinsicProcedure intrinsic)
        {
            var src1 = this.Operand(1);
            var src2 = this.Operand(2);
            var dst = this.Operand(0);
            m.Assign(dst, m.Fn(intrinsic, src1, src2));
            m.Assign(Q(), m.Cond(dst));
        }

        private void RewriteSax(PrimitiveType elemType)
        {
            var sum = binder.CreateTemporary(elemType);
            var diff = binder.CreateTemporary(elemType);
            var rn = Operand(1);
            var rm = Operand(2);
            m.Assign(sum, m.IAdd(m.Slice(rn, 0, 16), m.Slice(rm, 16, 16)));
            m.Assign(diff, m.ISub(m.Slice(rn, 16, 16), m.Slice(rm, 0, 16)));
            var rd = Operand(0);
            m.Assign(rd, m.Seq(diff, sum));
            //$REVIEW: flags?
        }

        private void RewriteXtab(PrimitiveType dt)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var src = MaybeSlice(Operand(2), dt);
            src = m.Convert(src, src.DataType, dst.DataType);
            m.Assign(dst, m.IAdd(this.Operand(1), src));
        }

        private void RewriteXtab16(
            IntrinsicProcedure intrinsicNoRot,
            IntrinsicProcedure intrinsicRotate)
        {
            var src1 = Operand(1);
            var src2 = this.Reg((RegisterStorage) instr.Operands[2]);
            var dst = Operand(0);
            if (instr.ShiftType == Mnemonic.ror &&
                instr.ShiftValue is Constant imm)
            {
                var rotation = imm.ToInt32();
                if (rotation != 0)
                {
                    m.Assign(dst, m.Fn(intrinsicRotate, src1, src2, imm));
                    return;
                }
            }
            m.Assign(dst, m.Fn(intrinsicNoRot, src1, src2));
        }


        private void RewriteXtb(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var dst = this.Operand(0, PrimitiveType.Word32, true);
            var src = MaybeSlice(Operand(1), dtSrc);
            src = m.Convert(src, dtSrc, dtDst);
            m.Assign(dst, src);
        }

        private void RewriteXtb16(
            IntrinsicProcedure intrinsicNoRot,
            IntrinsicProcedure intrinsicRotate)
        {
            var src = this.Reg((RegisterStorage) instr.Operands[1]);
            var dst = Operand(0);
            if (instr.ShiftType == Mnemonic.ror && 
                instr.ShiftValue is Constant imm)
            {
                var rotation = imm.ToInt32();
                if (rotation != 0)
                {
                    m.Assign(dst, m.Fn(intrinsicRotate, src, imm));
                    return;
                }
            }
            m.Assign(dst, m.Fn(intrinsicNoRot, src));
        }

        private void RewriteYield()
        {
            m.SideEffect(m.Fn(yield_intrinsic));
        }
    }
}

