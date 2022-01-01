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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    public partial class Rewriter
    {
        private Expression IsNan(Expression arg)
        {
            return host.Intrinsic("__is_nan", true, PrimitiveType.Bool, arg);
        }

        private void RewriteFabs()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("fabs", true, s.DataType, s));
        }

        private void RewriteFacos()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("acos", true, s.DataType, s));
        }


        private void RewriteFatan()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("atan", true, s.DataType, s));
        }

        private void RewriteFatanh()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("atanh", true, s.DataType, s));
        }

        private void RewriteFbcc(ConditionCode cc)
        {
            var addr = ((M68kAddressOperand)instr.Operands[0]).Address;
            if (cc == ConditionCode.NEVER)
            {
                m.Nop();
            }
            else if (cc == ConditionCode.ALWAYS)
            {
                iclass = InstrClass.Transfer;
                m.Goto(addr);
            }
            else
            {
                iclass = InstrClass.ConditionalTransfer;
                var test = m.Test(cc, FpuFlagGroup());
                m.Branch(test, addr, iclass);
            }
        }

        private void RewriteFbcc(Func<Expression, Expression> fnTest)
        {
            iclass = InstrClass.ConditionalTransfer;
            m.Branch(fnTest(
                binder.EnsureRegister(Registers.fpsr)),
                ((M68kAddressOperand)instr.Operands[0]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteFBinIntrinsic(string fnName)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, (s, d) =>
                host.Intrinsic(fnName, true, s.DataType, s, d));
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(FpuFlagGroup(), m.Cond(opDst));
        }

        private void RewriteFBinOp(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, binOpGen);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(FpuFlagGroup(), m.Cond(opDst));
        }

        private void RewriteFSinCos()
        {
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dstCos = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("cos", true, s.DataType, s));
            var dstSin = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("sin", true, s.DataType, s));
            if (dstSin != null)
            {
                m.Assign(FpuFlagGroup(), m.Cond(dstSin));
            }
            EmitInvalid();
        }

        private void RewriteFUnaryIntrinsic(string fnName)
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic(fnName, true, s.DataType, s));
        }

        private void RewriteFUnaryIntrinsic(Func<Expression, Expression> preProcess, string fnName)
        {
            //$TODO: #include <math.h>
            var src = preProcess(orw.RewriteSrc(instr.Operands[0], instr.Address));
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
               host.Intrinsic(fnName, true, s.DataType, s));
        }

        private void RewriteFUnaryIntrinsic(string fnName, Func<Expression, Expression> postProcess)
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = postProcess(orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
               host.Intrinsic(fnName, true, s.DataType, s))!);
        }

        private void RewriteFUnaryOp(Func<Expression, Expression> unaryOpGen)
        {
            var op = orw.RewriteUnary(instr.Operands[0], instr.Address, instr.DataWidth!, unaryOpGen);
            m.Assign(FpuFlagGroup(), m.Cond(op));
        }

        private Identifier FpuFlagGroup()
        {
            return binder.EnsureFlagGroup(
                Registers.fpsr,
                0xFF000000u,
                "FPUFLAGS",
                PrimitiveType.Byte);
        }

        private void RewriteFcmp()
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.Assign(FpuFlagGroup(), m.Cond(m.ISub(opDst, opSrc)));
        }

        private void RewriteFmove()
        {
            var opSrc = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var opDst = orw.RewriteDst(instr.Operands[1], instr.Address, opSrc, MaybeCastFpuArgs);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(FpuFlagGroup(), m.Cond(opDst));
        }

        private void RewriteFmovecr()
        {
            var opSrc = (M68kImmediateOperand)instr.Operands[0];
            int n = opSrc.Constant.ToInt32();
            Expression src;
            if (fpuRomConstants.TryGetValue(n, out double d))
            {
                src = Constant.Real64(d);
                src.DataType = PrimitiveType.Real80;
            }
            else
            {
                src = host.Intrinsic("__fmovecr", true, PrimitiveType.Real64, opSrc.Constant);
            }
            var dst = orw.RewriteSrc(instr.Operands[1], instr.Address);
            m.Assign(dst, src);
            m.Assign(binder.EnsureRegister(Registers.fpsr), m.Cond(dst));
        }

        private void RewriteFasin()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("asin", true, s.DataType, s));
        }

        private void RewriteFintrz()
        {
            // C's trunc() is supposed to round
            // to zero.
            // http://en.cppreference.com/w/c/numeric/math/trunc
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("trunc", true, s.DataType, s));
        }

        private void RewriteFsqrt()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("sqrt", true, s.DataType, s));
        }

        private void RewriteFtan()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var dst = orw.RewriteDst(instr.Operands[1], instr.Address, src, (s, d) =>
                host.Intrinsic("tan", true, s.DataType, s));
        }

        private Expression MaybeCastFpuArgs(Expression src, Expression dst)
        {
            if (src.DataType != dst.DataType)
                return m.Convert(src, src.DataType, dst.DataType);
            else
                return src;
        }
    }
}
