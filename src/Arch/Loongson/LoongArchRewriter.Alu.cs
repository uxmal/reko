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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Loongson
{
    public partial class LoongArchRewriter
    {
        private void RewriteAlsl(bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            var sa = ((Constant)instr.Operands[3]).ToInt32() + 1;
            var src = m.IAdd(m.Shl(src1, sa), src2);
            Assign(instr.Operands[0], src);
        }

        private void RewriteAlslU(bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            var sa = ((Constant)instr.Operands[3]).ToInt32() + 1;
            var src = m.IAdd(m.Shl(src1, sa), src2);
            AssignU(instr.Operands[0], src);
        }

        private void RewriteAsr(Func<Expression,Expression,Expression> fn)
        {
            var src0 = Op(0, false);
            var src1 = Op(1, false);
            m.Branch(fn(src0, src1), instr.Address + instr.Length);
            m.SideEffect(m.Fn(raise_exception, m.Word32(0xA)));
        }

        private void RewriteAtomic(IntrinsicProcedure proc, bool slice32bit)
        {
            var src1 = Op(1, false);
            var src2 = Op(2, slice32bit);
            var fn = proc.MakeInstance(arch.PointerType.BitSize, slice32bit
                ? PrimitiveType.Word32
                : PrimitiveType.Word64);
            Assign(instr.Operands[0], m.Fn(fn, src1, src2));
        }

        private void RewriteBinaryOp(Func<Expression,Expression,Expression> fn, bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            Assign(instr.Operands[0], fn(src1, src2));
        }

        private void RewriteBsrpick(bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            var src3 = Op(3, slice32bit);
            var dt = slice32bit
                ? PrimitiveType.Word32
                : PrimitiveType.Word64;
            Assign(instr.Operands[0], m.Fn(bstrpick.MakeInstance(dt), src1, src2, src3));
        }

        private void RewriteBytepick(bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            var src3 = Op(3, slice32bit);
            var dt = slice32bit
                ? PrimitiveType.Word32
                : PrimitiveType.Word64;
            Assign(instr.Operands[0], m.Fn(bytepick.MakeInstance(dt), src1, src2, src3));
        }

        private void RewriteExt(PrimitiveType dtDst, PrimitiveType dtSrc)
        {
            var src = RewriteStoreSource(dtSrc);
            Assign(instr.Operands[0], m.Convert(src, dtSrc, dtDst));
        }

        private void RewriteLdptr(PrimitiveType dt)
        {
            var ea = EffectiveAddress();
            Assign(instr.Operands[0], m.Mem(dt, ea));
        }

        private void RewriteLoad(PrimitiveType dt)
        {
            var ea = EffectiveAddress();
            Assign(instr.Operands[0], m.Mem(dt, ea));
        }

        private void RewriteLoadBoundsCheck(PrimitiveType dt, Func<Expression, Expression, Expression> fn)
        {
            var ea = binder.EnsureRegister((RegisterStorage) instr.Operands[1]);
            var limit = binder.EnsureRegister((RegisterStorage) instr.Operands[2]);
            m.MicroBranch(fn(ea, limit), this.rtls.Count + 2);
            m.SideEffect(m.Fn(raise_exception, m.Word32(0xA)));
            Assign(instr.Operands[0], m.Mem(dt, ea));
        }

        private void RewriteLu12i()
        {
            var imm = (Constant) instr.Operands[1];
            var value = imm.ToInt32() << 12;
            Assign(instr.Operands[0], value);
        }

        private void RewriteLu32i()
        {
            var imm = (Constant) instr.Operands[1];
            var prefix = m.Word32(imm.ToUInt32());
            var reg = Op(0, false);
            m.Assign(reg, m.Seq(prefix, m.Slice(reg, PrimitiveType.Word32, 0)));
        }

        private void RewriteLu52i()
        {
            var imm = (Constant) instr.Operands[2];
            var dtPrefix = PrimitiveType.CreateWord(12);
            var prefix = Constant.Create(dtPrefix, imm.ToUInt32());
            var reg = Op(1, false);
            m.Assign(Op(0, false), m.Seq(prefix, m.Slice(reg, 0, 52)));
        }

        private void RewriteMask(Func<Expression, Expression> fn)
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            var zero = Constant.Zero(src1.DataType);
            Assign(instr.Operands[0], m.Conditional(src1.DataType, fn(src2), zero, src1));
        }

        private void RewriteMul(bool slice32bit)
        {
            var src1 = Op(1, slice32bit);
            var src2 = Op(2, slice32bit);
            Assign(instr.Operands[0], m.IMul(src1, src2));
        }

        private void RewriteMulh_d()
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            var tmp = binder.CreateTemporary(PrimitiveType.Int128);
            m.Assign(tmp, m.SMul(PrimitiveType.Int128, src1, src2));
            Assign(instr.Operands[0], m.Slice(tmp, PrimitiveType.Int64, 64));
        }

        private void RewriteMulh_du()
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            var tmp = binder.CreateTemporary(PrimitiveType.UInt128);
            m.Assign(tmp, m.UMul(PrimitiveType.UInt128, src1, src2));
            Assign(instr.Operands[0], m.Slice(tmp, PrimitiveType.UInt64, 64));
        }

        private void RewriteMulh_w()
        {
            var src1 = Op(1, true);
            var src2 = Op(2, true);
            var tmp = binder.CreateTemporary(PrimitiveType.Int64);
            m.Assign(tmp, m.SMul(PrimitiveType.Int64, src1, src2));
            Assign(instr.Operands[0], m.Slice(tmp, PrimitiveType.Int32, 32));
        }

        private void RewriteMulh_wu()
        {
            var src1 = Op(1, true);
            var src2 = Op(2, true);
            var tmp = binder.CreateTemporary(PrimitiveType.UInt64);
            m.Assign(tmp, m.UMul(PrimitiveType.UInt64, src1, src2));
            Assign(instr.Operands[0], m.Slice(tmp, PrimitiveType.UInt32, 32));
        }

        private void RewriteMulw(PrimitiveType dtDst, PrimitiveType dtSrc)
        {
            var src1 = binder.CreateTemporary(dtSrc);
            m.Assign(src1, m.Slice(binder.EnsureRegister((RegisterStorage) instr.Operands[1]), dtSrc, 0));
            var src2 = binder.CreateTemporary(dtSrc);
            m.Assign(src2, m.Slice(binder.EnsureRegister((RegisterStorage) instr.Operands[2]), dtSrc, 0));
            Assign(instr.Operands[0], m.SMul(dtDst, src1, src2));
        }

        private void RewriteMulwu(PrimitiveType dtDst, PrimitiveType dtSrc)
        {
            var src1 = binder.CreateTemporary(dtSrc);
            m.Assign(src1, m.Slice(binder.EnsureRegister((RegisterStorage) instr.Operands[1]), dtSrc, 0));
            var src2 = binder.CreateTemporary(dtSrc);
            m.Assign(src2, m.Slice(binder.EnsureRegister((RegisterStorage) instr.Operands[2]), dtSrc, 0));
            Assign(instr.Operands[0], m.UMul(dtDst, src1, src2));
        }

        private void RewriteNor()
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            if (src1.IsZero)
            {
                if (src2.IsZero)
                {
                    Assign(instr.Operands[0], ((Constant) src2).Complement());
                }
                else
                {
                    Assign(instr.Operands[0], m.Comp(src2));
                }
            }
            else if (src2.IsZero)
            {
                Assign(instr.Operands[0], m.Comp(src1));
            }
            else
            {
                Assign(instr.Operands[0], m.Comp(m.Or(src1, src2)));
            }
        }

        private void RewriteOr()
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            if (src1.IsZero)
            {
                Assign(instr.Operands[0], src2);
            }
            else if (src2.IsZero)
            {
                Assign(instr.Operands[0], src1);
            }
            else
            {
                Assign(instr.Operands[0], m.Or(src1, src2));
            }
        }

        private void RewriteOri()
        {
            var imm = (Constant) instr.Operands[2];
            if (imm.IsZero)
            {
                if (instr.Operands[0] == instr.Operands[1])
                {
                    m.Nop();
                    iclass = InstrClass.Linear | InstrClass.Padding;
                    return;
                }
            }
            var rDst = instr.Operands[0];
            var c = Constant.Create(rDst.DataType, imm.ToUInt64());
            var src1 = Op(1, false);
            if (src1.IsZero)
            {
                Assign(rDst, c);
            }
            else
            {
                Assign(rDst, m.Or(src1, c));
            }
        }

        private void RewriteRotr(bool slice32bits)
        {
            var src1 = Op(1, slice32bits);
            var src2 = Op(2, false);
            Assign(instr.Operands[0], m.Fn(CommonOps.Ror, src1, src2));
        }

        private void RewriteSet(Func<Expression,Expression,Expression> fn)
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            var oDst = instr.Operands[0];
            Assign(oDst, m.Conditional(
                oDst.DataType,
                fn(src1, src2),
                Constant.Create(oDst.DataType, 1),
                Constant.Create(oDst.DataType, 0)));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn, bool slice32bits)
        {
            var src1 = Op(1, slice32bits);
            var shift = Op(2, false);
            Assign(instr.Operands[0], fn(src1, shift));
        }

        private void RewriteStore(PrimitiveType dt)
        {
            Expression src = RewriteStoreSource(dt);
            Expression ea = EffectiveAddress();
            m.Assign(m.Mem(dt, ea), src);
        }

        private Expression RewriteStoreSource(PrimitiveType dt)
        {
            var rSrc = (RegisterStorage) instr.Operands[0];
            Expression src;
            if (rSrc.Number == 0)
            {
                src = Constant.Zero(dt);
            }
            else
            {
                src = binder.EnsureRegister(rSrc);
                if (src.DataType.BitSize > dt.BitSize)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, m.Slice(src, dt, 0));
                    src = tmp;
                }
            }

            return src;
        }

        private void RewriteStoreBoundsCheck(PrimitiveType dt, Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteStoreSource(dt);
            var ea = binder.EnsureRegister((RegisterStorage) instr.Operands[1]);
            var limit = binder.EnsureRegister((RegisterStorage) instr.Operands[2]);
            m.MicroBranch(fn(ea, limit), this.rtls.Count + 2);
            m.SideEffect(m.Fn(raise_exception, m.Word32(0xA)));
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteUnaryOp(Func<Expression, Expression> fn, bool slice32bit)
        {
            var src = Op(1, slice32bit);
            Assign(instr.Operands[0], fn(src));
        }

        private void RewriteXor()
        {
            var src1 = Op(1, false);
            var src2 = Op(2, false);
            var oDst = instr.Operands[0];
            if (src1.IsZero)
            {
                Assign(oDst, src2);
            }
            else if (src2.IsZero)
            {
                Assign(oDst, src1);
            }
            else if (src1 == src2)
            {
                Assign(oDst, Constant.Zero(oDst.DataType));
            }
            else
            {
                Assign(oDst, m.Xor(src1, src2));
            }
        }
    }
}
