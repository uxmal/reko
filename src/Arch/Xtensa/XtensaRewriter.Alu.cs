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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteAddi()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteSimm(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            if (src2.IsNegative)
            {
                m.Assign(dst, m.ISub(src1, src2.Negate()));
            }
            else
            {
                m.Assign(dst, m.IAdd(src1, src2));
            }
        }

        private void RewriteAddx(int scale)
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);

            m.Assign(dst, m.IAdd(src2, m.IMul(src1, scale)));
        }

        private void RewriteAll(int n, Func<Expression, Expression, Expression> fn)
        {
            var bReg = ((RegisterOperand) this.instr.Operands[1]).Register;
            var bNumber = bReg.Number - Registers.b0.Number;
            Debug.Assert(n >= 0);
            Expression e = binder.EnsureRegister(bReg);
            for (int i = 1; i < n; ++i)
            {
                bReg = arch.GetBoolRegister(bNumber + i);
                e = fn(e, binder.EnsureRegister(bReg));
            }
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, e);
        }

        private void RewriteSubx(int scale)
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);

            m.Assign(dst, m.ISub(m.IMul(src1, scale), src2));
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> fn)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteIntrinsicFn(string name, bool hasSideEffect)
        {
            var aSrc = instr.Operands.Skip(1)
                .Select(o => RewriteOp(o))
                .ToArray();
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.Intrinsic(name, hasSideEffect, dst.DataType, aSrc));
        }

        private void RewriteIntrinsicProc(string name)
        {
            var aSrc = instr.Operands
                .Select(o => RewriteOp(o))
                .ToArray();
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance, aSrc));
        }

        private void RewriteClamps()
        {
            var src = RewriteOp(instr.Operands[1]);
            var clampSize = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.Intrinsic("__clamps", false, PrimitiveType.Int32, src, clampSize));
        }

        private void RewriteCopy()
        {
            var src = RewriteOp(this.instr.Operands[1]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteCvtFloatToIntegral(string fnName, PrimitiveType dtIntegral)
        {
            var src = RewriteOp(instr.Operands[1]);
            var scale = (Constant) RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            if (scale.GetValue() is float rScale && rScale != 1.0F)
            {
                src = m.FMul(src, scale);
            }
            m.Assign(dst, host.Intrinsic(fnName, false, dtIntegral, src));
        }

        /// <summary>
        /// Generate a copy when the dst and src are reversed in the assembly language instruction.
        /// </summary>
        private void RewriteInverseCopy()
        {
            var src = RewriteOp(this.instr.Operands[0]);
            var dst = RewriteOp(this.instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteExtui()
        {
            var src = RewriteOp(this.instr.Operands[1]);
            var dst = RewriteOp(this.instr.Operands[0]);
            var sh = RewriteUimm(this.instr.Operands[2]);
            var ms = RewriteUimm(this.instr.Operands[3]);
            var mask = (1u << (int)ms.ToInt32()) - 1;
            Expression shifted;
            if (sh.IsZero)
                shifted = src;
            else
                shifted = m.Shr(src, sh);
            m.Assign(
                dst,
                m.And(shifted, mask));
        }

        private void RewriteEntry()
        {
            var frameSize = RewriteOp(this.instr.Operands[1]);
            var reg = RewriteOp(this.instr.Operands[0]);
            m.Assign(reg, m.ISub(reg, frameSize));
        }

        private void RewriteMax()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, host.Intrinsic("max", false, PrimitiveType.Int32, src1, src2));
        }

        private void RewriteMaxu()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, host.Intrinsic("__maxu", false, PrimitiveType.UInt32, src1, src2));
        }

        private void RewriteMin()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, host.Intrinsic("min", false, PrimitiveType.Int32, src1, src2));
        }

        private void RewriteMinu()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, host.Intrinsic("__minu", false, PrimitiveType.UInt32, src1, src2));
        }

        private void RewriteMovft(Func<Expression, Expression> fn)
        {
            var flag = RewriteOp(this.instr.Operands[2]);
            var src = RewriteOp(this.instr.Operands[1]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Branch(fn(flag), instr.Address + instr.Length);
            m.Assign(dst, src);
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void RewriteOr()
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, m.Or(src1, src2));
        }

        private void RewriteL32i()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var offset = Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32());
            m.Assign(
                dst,
                m.Mem32(
                    m.IAdd(
                        RewriteOp(instr.Operands[1]),
                        offset)));
        }

        private void RewriteLddecinc(Func<Expression, Expression, Expression> fn)
        {
            var ptr = RewriteOp(instr.Operands[1]);
            m.Assign(ptr, fn(ptr, m.Int32(4)));
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, m.Mem32(ptr));
        }

        private void RewriteLsi(DataType dt)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(
                tmp,
                m.Mem(
                    dt,
                    m.IAdd(
                        RewriteOp(instr.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32()))));
            m.Assign(dst, m.Convert(tmp, tmp.DataType, PrimitiveType.Int32));
        }

        private void RewriteLsiu()
        {
            var a = RewriteOp(this.instr.Operands[1]);
            var off = Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32());
            Expression? ea = null;
            var dst = RewriteOp(this.instr.Operands[0]);
            if (off.IsZero)
            {
                ea = a;
            }
            else
            {
                ea = binder.CreateTemporary(a.DataType);
                m.Assign(ea, m.IAdd(a, off));
            }
            m.Assign(dst, m.Mem(PrimitiveType.Real32, ea));
            if (!off.IsZero)
            { 
                m.Assign(a, ea);
            }
        }

        private void RewriteLui(DataType dt)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(
                tmp,
                m.Mem(
                    dt,
                    m.IAdd(
                        RewriteOp(instr.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32()))));
            m.Assign(dst, m.Convert(tmp, tmp.DataType, PrimitiveType.UInt32));
        }

        private void RewriteMaddSub(Func<Expression, Expression, Expression> fn)
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, fn(dst, m.FMul(src1, src2)));
        }

        private void RewriteMovcc(Func<Expression,Expression,Expression> fn)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var src = RewriteOp(this.instr.Operands[1]);
            var cond = RewriteOp(this.instr.Operands[2]);
            m.BranchInMiddleOfInstruction(
                fn(cond, Constant.Zero(cond.DataType)).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Assign(dst, src);
        }

        private void RewriteMovi_n()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var src = Constant.Int32(
                ((ImmediateOperand)this.instr.Operands[1]).Value.ToInt32());
            m.Assign(dst, src);
        }

        private void RewriteMul(string fnName, PrimitiveType dtProduct)
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var product = host.Intrinsic(fnName, false, dtProduct, src1, src2);
            var dst = binder.EnsureSequence(PrimitiveType.CreateWord(40), Registers.ACCHI, Registers.ACCLO);
            m.Assign(dst, product);
        }

        private void RewriteMula(string fnName, PrimitiveType dtProduct)
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var product = host.Intrinsic(fnName, false, dtProduct, src1, src2);
            var dst = binder.EnsureSequence(PrimitiveType.CreateWord(40), Registers.ACCHI, Registers.ACCLO);
            m.Assign(dst, m.IAdd(dst, product));
        }

        private void RewriteMulaIncDec(string fnName, PrimitiveType dtProduct, int increment)
        {
            var addr = RewriteOp(instr.Operands[1]);
            m.Assign(addr, m.AddSubSignedInt(addr, increment));
            var src1 = RewriteOp(instr.Operands[2]);
            var src2 = RewriteOp(instr.Operands[3]);
            var product = host.Intrinsic(fnName, false, dtProduct, src1, src2);
            var dst = binder.EnsureSequence(PrimitiveType.CreateWord(40), Registers.ACCHI, Registers.ACCLO);
            m.Assign(dst, m.IAdd(dst, product));
            m.Assign(RewriteOp(instr.Operands[0]), m.Mem32(addr));
        }

        private void RewriteMuls(string fnName, PrimitiveType dtProduct)
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var product = host.Intrinsic(fnName, false, dtProduct, src1, src2);
            var dst = binder.EnsureSequence(PrimitiveType.CreateWord(40), Registers.ACCHI, Registers.ACCLO);
            m.Assign(dst, m.ISub(dst, product));
        }



        private void RewriteMul16(Func<Expression, Expression, Expression> mul, Domain dom)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var tmp1 = binder.CreateTemporary(PrimitiveType.Create(dom, 16));
            var tmp2 = binder.CreateTemporary(PrimitiveType.Create(dom, 16));
            m.Assign(tmp1, m.Convert(src1, src1.DataType, tmp1.DataType));
            m.Assign(tmp2, m.Convert(src2, src2.DataType, tmp2.DataType));
            m.Assign(dst, mul(tmp1, tmp2));
        }


        private void RewriteMulh(string fnName, PrimitiveType dt)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var mul = host.Intrinsic(fnName, false, dt, src1, src2);
            m.Assign(dst, mul);
        }

        private void RewriteSi(DataType dt)
        {
            var src = RewriteOp(instr.Operands[0]);
            var ea = RewriteOp(instr.Operands[1]);
            var off = Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32());
            if (!off.IsZero)
            {
                ea = m.IAdd(ea, off);
            }
            m.Assign(
                m.Mem(dt, ea),
                src);
        }

        private void RewriteSext()
        {
            var src = RewriteOp(instr.Operands[1]);
            var bits = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, host.Intrinsic(
                "__sext",
                false,
                PrimitiveType.Int32,
                src, bits));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            //$REVIEW: the Xtensa spec is unclear on left shifts, shouild it be
            // a[0] = a[1] << (32 - SAR)?
            var src1 = RewriteOp(instr.Operands[1]);
            var sa = binder.EnsureRegister(Registers.SAR);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, fn(src1, sa));
        }

        private void RewriteShiftI(Func<Expression,Expression,Expression> fn)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteSimm(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteSrc()
        {
            var src1 = (Identifier)RewriteOp(instr.Operands[1]);
            var src2 = (Identifier)RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var sa = binder.EnsureRegister(Registers.SAR);
            var cat = binder.EnsureSequence(
                PrimitiveType.CreateWord(src1.DataType.BitSize + src2.DataType.BitSize),
                src1.Storage,
                src2.Storage);
            var shifted = m.Shr(cat, sa);
            m.Assign(
                dst,
                m.Convert(shifted, shifted.DataType, dst.DataType));
        }

        private void RewriteSsa()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, src);
        }

        private void RewriteSsl()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, m.ISub(Constant.Create(src.DataType, 32), src));
        }

        private void RewriteSsa8b()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, m.ISub(m.Int32(32), m.IMul(src, 8)));
        }

        private void RewriteSsa8l()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, m.IMul(src, 8));
        }

        private void RewriteFloat_s(PrimitiveType dt)
        {
            var src = RewriteOp(instr.Operands[1]);
            var scale = (Constant) RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var u = binder.CreateTemporary(dt);
            m.Assign(u, src);

            src = m.Convert(u, dt, PrimitiveType.Real32);
            if (scale.GetValue() is float rScale && rScale != 1.0F)
            {
                src = m.FDiv(src, scale);
            }
            m.Assign(dst, src);
        }

        private void RewriteUnaryOp(Func<Expression, Expression> fn)
        {
            var src = RewriteOp(instr.Operands[1]);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, fn(src));
        }

    }
}
