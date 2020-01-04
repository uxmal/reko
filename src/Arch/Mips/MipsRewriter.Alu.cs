#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteAdd(MipsInstruction instr, PrimitiveType size)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }

        private void RewriteAddiupc(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var offset = ((ImmediateOperand) instr.Operands[1]).Value.ToUInt32();
            var addr = instr.Address + (instr.Length + offset);
            m.Assign(dst, addr);
        }

        private void RewriteAluipc(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteAnd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opLeft;
            else if (opRight.IsZero)
                opSrc = opRight;
            else
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }

        private void RewriteCache(MipsInstruction instr)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var opMem = RewriteOperand(instr.Operands[1]);
            m.SideEffect(host.PseudoProcedure("__cache", VoidType.Instance, op1, opMem));
        }

        private void RewriteClo(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            m.Assign(opDst, host.PseudoProcedure("__clo", PrimitiveType.Int32, opSrc));
        }

        private void RewriteClz(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            m.Assign(opDst, host.PseudoProcedure("__clz", PrimitiveType.Int32, opSrc));
        }

        private void RewriteDshiftC(MipsInstruction instr, Func<Expression,Expression,Expression> fn, int offset)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opShift = (Constant) RewriteOperand0(instr.Operands[2]);
            m.Assign(opDst, fn(opSrc, m.Int32(opShift.ToInt32() + offset)));
        }

        private void RewriteDshift(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opShift = RewriteOperand0(instr.Operands[2]);
            m.Assign(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteCopy(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteDiv(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);
            if (instr.Operands.Length > 3)
            {
                var op3 = RewriteOperand(instr.Operands[2]);
                m.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hi = binder.EnsureRegister(arch.hi);
                var lo = binder.EnsureRegister(arch.lo);
                m.Assign(lo, ctor(op1, op2));
                m.Assign(hi, m.Mod(op1, op2));
            }
        }

        private void RewriteDshift32(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            var sh = m.IAdd(RewriteOperand(instr.Operands[2]), 32);
            m.Assign(dst, ctor(src, sh));
        }

        private void RewriteExt(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            var pos = RewriteOperand(instr.Operands[2]);
            var size = RewriteOperand(instr.Operands[3]);
            m.Assign(dst, host.PseudoProcedure("__ext", dst.DataType, src, pos, size));
        }

        private void RewriteIns(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand0(instr.Operands[1]);
            var pos = RewriteOperand(instr.Operands[2]);
            var size = RewriteOperand(instr.Operands[3]);
            m.Assign(dst, host.PseudoProcedure("__ins", dst.DataType, dst, src, pos, size));
        }

        private void RewriteLoad(MipsInstruction instr, PrimitiveType dtSmall, PrimitiveType dtSmall64 = null)
        {
            var opSrc = RewriteOperand(instr.Operands[1]);
            var opDst = RewriteOperand(instr.Operands[0]);
            opSrc.DataType = (arch.WordWidth.BitSize == 64)
                ? dtSmall64 ?? dtSmall
                : dtSmall;
            if (opDst.DataType.Size != opSrc.DataType.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension/conversion.
                opSrc = m.Cast(arch.WordWidth, opSrc);
            }
            m.Assign(opDst, opSrc);
        }

        private void RewriteLoadLinked32(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_32", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLoadLinked64(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_64", PrimitiveType.Word64, opSrc));
        }

        private void RewriteStoreConditional32(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.Operands[1]);
            var opReg = RewriteOperand(instr.Operands[0]);
            m.Assign(opReg, host.PseudoProcedure("__store_conditional_32", PrimitiveType.Word32, opMem, opReg));
        }

        private void RewriteStoreConditional64(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.Operands[1]);
            var opReg = RewriteOperand(instr.Operands[0]);
            m.Assign(opReg, host.PseudoProcedure("__store_conditional_64", PrimitiveType.Word64, opMem, opReg));
        }

        private void RewriteLsa(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            var rt = RewriteOperand(instr.Operands[2]);
            var sh = ((ImmediateOperand) instr.Operands[3]).Value.ToInt32();
            m.Assign(dst, m.IAdd(rt, m.Shl(rs, sh)));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand)instr.Operands[1];
            long v = immOp.Value.ToInt16();
            var opSrc = Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            m.Assign(opDst, host.PseudoProcedure("__lwl", PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteLwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            m.Assign(opDst, host.PseudoProcedure("__lwr", PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteLcpr1(MipsInstruction instr)
        {
            var opDstFloat = RewriteOperand(instr.Operands[0]);
            var opSrcMem = RewriteOperand(instr.Operands[1]);
            int bitDiff = opDstFloat.DataType.BitSize - opSrcMem.DataType.BitSize;
            if (bitDiff > 0)
            {
                var tmpLo = binder.CreateTemporary(opSrcMem.DataType);
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(bitDiff));
                m.Assign(tmpLo, opSrcMem);
                m.Assign(tmpHi, m.Slice(tmpHi.DataType, opDstFloat, opSrcMem.DataType.BitSize));
                m.Assign(opDstFloat, m.Seq(tmpHi, tmpLo));
            }
            else
            {
                m.Assign(opDstFloat, opSrcMem);
            }
        }

        private void RewriteLdc2(MipsInstruction instr)
        {
            var iRegDst = ((RegisterOperand) instr.Operands[0]).Register.Number;
            var opSrcMem = RewriteOperand(instr.Operands[1]);
            m.SideEffect(host.PseudoProcedure("__write_cpr2", VoidType.Instance,
                Constant.Byte((byte) iRegDst),
                opSrcMem));
        }

        private void RewriteLwm(MipsInstruction instr)
        {
            int i = 0;
            int rt = ((RegisterOperand) instr.Operands[0]).Register.Number;
            var mem = ((IndirectOperand) instr.Operands[1]);
            var rs = binder.EnsureRegister(mem.Base);
            int offset = mem.Offset;
            int count = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            while (i != count)
            {
                int this_rt = (rt + i < 32) ? rt + i : rt + i - 16;
                int this_offset = offset + (i << 2);
                var dst = binder.EnsureRegister(arch.GetRegister(this_rt));
                m.Assign(dst, m.Mem32(m.IAddS(rs, this_offset)));

                // if this_rt == rs and i != count - 1:
                // raise UNPREDICTABLE()
                ++i;
            }
        }

        private void RewriteLwxs(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var idx = (IndexedOperand) instr.Operands[1];
            var idBase = binder.EnsureRegister(idx.Base);
            var idIndex = binder.EnsureRegister(idx.Index);
            m.Assign(opDst, m.Mem32(m.IAdd(idBase, m.IMul(idIndex, 4))));
        }

        private void RewriteLx(MipsInstruction instr, PrimitiveType dt)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var idx = (IndexedOperand) instr.Operands[1];
            var idBase = binder.EnsureRegister(idx.Base);
            var idIndex = binder.EnsureRegister(idx.Index);
            Expression src = m.Mem32(m.IAdd(idBase, idIndex));
            if (dst.DataType.Size != dt.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension.
                src.DataType = dt;
                src = m.Cast(dst.DataType, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteMac_int(MipsInstruction instr, Func<Expression,Expression,Expression> fn)
        {
            var op1 = RewriteOperand0(instr.Operands[0]);
            var op2 = RewriteOperand0(instr.Operands[1]);
            var hi_lo = binder.EnsureSequence(PrimitiveType.Word64, arch.hi, arch.lo);
            var product = m.IMul(op1, op2);
            product.DataType = hi_lo.DataType;
            m.Assign(hi_lo, fn(hi_lo, product));
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage reg)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, binder.EnsureRegister(reg));
        }

        private void RewriteMt(MipsInstruction instr, RegisterStorage reg)
        {
            var opSrc = RewriteOperand0(instr.Operands[0]);
            m.Assign(binder.EnsureRegister(reg), opSrc);
        }

        private void RewriteMovCc(MipsInstruction instr, Func<Expression, Expression> cmp0)
        {
            var opCond = RewriteOperand0(instr.Operands[2]);
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            m.BranchInMiddleOfInstruction(
                cmp0(opCond).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Assign(opDst, opSrc);
        }

        private void RewriteMove(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand0(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteMovep(MipsInstruction instr)
        {
            var dstHi = (RegisterOperand) instr.Operands[0];
            var dstLo = (RegisterOperand) instr.Operands[1];
            var srcHi = (RegisterOperand) instr.Operands[2];
            var srcLo = (RegisterOperand) instr.Operands[3];
            var dst = binder.EnsureSequence(PrimitiveType.Word64, dstHi.Register, dstLo.Register);
            Expression src;
            if (srcHi.Register.Number == 0 || srcLo.Register.Number == 0)
            {
                src = m.Seq(RewriteOperand(srcHi), RewriteOperand(srcLo));
            }
            else
            {
                src = binder.EnsureSequence(PrimitiveType.Word64, srcHi.Register, srcLo.Register);
            }
            m.Assign(dst, src);
        }

        private void RewriteMul(MipsInstruction instr, Func<Expression, Expression, Expression> ctor, PrimitiveType dt)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);
            if (instr.Operands.Length == 3)
            {
                var op3 = RewriteOperand(instr.Operands[2]);
                m.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hilo = binder.EnsureSequence(dt, arch.hi, arch.lo);
                m.Assign(hilo, ctor(op1, op2));
            }
        }

        private void RewriteMsub(MipsInstruction instr)
        {
            var op1 = RewriteOperand0(instr.Operands[0]);
            var op2 = RewriteOperand0(instr.Operands[1]);
            var hi_lo = binder.EnsureSequence(PrimitiveType.Word64, arch.hi, arch.lo);
            m.Assign(hi_lo, m.ISub(hi_lo, m.IMul(op1, op2)));
        }


        private void RewriteNor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, m.Comp(opSrc));
        }

        private void RewriteNot(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr.Operands[1]);
            if (src is Constant c)
                src = Operator.Comp.ApplyConstant(c);
            else
                src = m.Comp(src);
            var dst = RewriteOperand0(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteOr(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLdl(MipsInstruction instr)
        {
            var opSrc = (IndirectOperand)instr.Operands[1];
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(
                opDst,
                host.PseudoProcedure("__ldl",
                    VoidType.Instance,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.Offset)));
        }

        private void RewriteLdr(MipsInstruction instr)
        {
            var opSrc = (IndirectOperand)instr.Operands[1];
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(
                opDst,
                host.PseudoProcedure("__ldr",
                    VoidType.Instance,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.Offset)));
        }

        private void RewriteRestore(MipsInstruction instr, bool ret)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29));
            int count = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            int rt = ((RegisterOperand) instr.Operands[1]).Register.Number;
            int u = ((ImmediateOperand) instr.Operands[0]).Value.ToInt32();
            int i = 0;
            bool gp = false;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count))
                    ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var this_offset = u - ((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                var reg = binder.EnsureRegister(arch.GetRegister(this_rt));
                m.Assign(reg, ea);
                ++i;
            }
            m.Assign(sp, m.IAddS(sp, u));
            if (ret)
                m.Return(0, 0);
        }

        private void RewriteSave(MipsInstruction instr)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29));
            int count = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            int rt = ((RegisterOperand) instr.Operands[1]).Register.Number;
            int u = ((ImmediateOperand) instr.Operands[0]).Value.ToInt32();
            bool gp = false;
            int i = 0;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count)) ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var reg = binder.EnsureRegister(arch.GetRegister(this_rt));
                var this_offset = -((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                m.Assign(ea, reg);
                ++i;
            }
            m.Assign(sp, m.ISubS(sp, u));
        }

        private void RewriteSdc2(MipsInstruction instr)
        {
            var iRegSrc = ((RegisterOperand) instr.Operands[0]).Register.Number;
            var opDstMem = RewriteOperand(instr.Operands[1]);
            m.Assign(opDstMem, host.PseudoProcedure(
                "__read_cpr2",
                opDstMem.DataType,
                Constant.Byte((byte) iRegSrc)));
        }


        private void RewriteSdl(MipsInstruction instr)
        {
            var opDst = (IndirectOperand)instr.Operands[1];
            var opSrc = RewriteOperand0(instr.Operands[0]);
            m.SideEffect(
                host.PseudoProcedure("__sdl",
                    VoidType.Instance,
                    binder.EnsureRegister(opDst.Base),
                    m.Int32(opDst.Offset),
                    opSrc));
        }

        private void RewriteSdr(MipsInstruction instr)
        {
            var opDst = (IndirectOperand)instr.Operands[1];
            var opSrc = RewriteOperand0(instr.Operands[0]);
            m.SideEffect(
                host.PseudoProcedure("__sdr",
                    VoidType.Instance,
                    binder.EnsureRegister(opDst.Base),
                    m.Int32(opDst.Offset),
                    opSrc));
        }

        private void RewriteSignExtend(MipsInstruction instr, PrimitiveType dt)
        {
            var opDst = RewriteOperand(instr.Operands[1]);
            var opSrc = RewriteOperand(instr.Operands[0]);
            var tmp = binder.CreateTemporary(dt);
            var dtDst = PrimitiveType.Create(Domain.SignedInt, opDst.DataType.BitSize);
            m.Assign(tmp, m.Slice(dt, opSrc, 0));
            m.Assign(opDst, m.Cast(dtDst, tmp));
        }

        private void RewriteSll(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opShift = RewriteOperand0(instr.Operands[2]);
            m.Assign(opDst, m.Shl(opSrc, opShift));
        }

        private void RewriteSra(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opShift = RewriteOperand0(instr.Operands[2]);
            m.Assign(opDst, m.Sar(opSrc, opShift));
        }

        private void RewriteSrl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var opShift = RewriteOperand0(instr.Operands[2]);
            m.Assign(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.Operands[0]);
            var opDst = RewriteOperand0(instr.Operands[1]);
            if (opDst.DataType.Size < opSrc.DataType.Size)
                opSrc = m.Cast(opDst.DataType, opSrc);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSub(MipsInstruction instr, PrimitiveType size)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = m.Neg(opRight);
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.ISub(opLeft, opRight);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[1]);
            var opSrc = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, host.PseudoProcedure(PseudoProcedure.SwL, PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteSwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.Operands[1]);
            var opSrc = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, host.PseudoProcedure(PseudoProcedure.SwR, PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteSwxs(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr.Operands[0]);
            var idx = (IndexedOperand) instr.Operands[1];
            var idBase = binder.EnsureRegister(idx.Base);
            var idIndex = binder.EnsureRegister(idx.Index);
            m.Assign(m.Mem32(m.IAdd(idBase, m.IMul(idIndex, 4))), src);
        }

        private void RewriteSxx(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOperand0(instr.Operands[0]);
            var src1 = RewriteOperand0(instr.Operands[1]);
            var src2 = RewriteOperand0(instr.Operands[2]);
            m.Assign(
                dst,
                m.Cast(dst.DataType, op(src1,src2)));
        }

        private void RewriteXor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.Operands[1]);
            var opRight = RewriteOperand0(instr.Operands[2]);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Xor(opLeft, opRight);
            var opDst = RewriteOperand0(instr.Operands[0]);
            m.Assign(opDst, opSrc);
        }
    }
}
