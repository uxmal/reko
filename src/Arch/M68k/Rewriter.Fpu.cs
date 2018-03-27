﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
            return host.PseudoProcedure("__is_nan", PrimitiveType.Bool, arg);
        }

        private void RewriteFbcc(ConditionCode cc)
        {
            var addr = ((M68kAddressOperand)di.op1).Address;
            if (cc == ConditionCode.NEVER)
            {
                m.Nop();
            }
            else if (cc == ConditionCode.ALWAYS)
            {
                rtlc = RtlClass.Transfer;
                m.Goto(addr);
            }
            else
            {
                rtlc = RtlClass.ConditionalTransfer;
                var test = m.Test(cc, FpuFlagGroup());
                m.Branch(test, addr, rtlc);
            }
        }

        private void RewriteFBinOp(Func<Expression, Expression, Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, binOpGen);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(FpuFlagGroup(), m.Cond(opDst));
        }

        private void RewriteFUnaryOp(Func<Expression, Expression> unaryOpGen)
        {
            var op = orw.RewriteUnary(di.op1, di.Address, di.dataWidth, unaryOpGen);
            m.Assign(FpuFlagGroup(), m.Cond(op));
        }

        private Identifier FpuFlagGroup()
        {
            return binder.EnsureFlagGroup(
                Registers.fpsr,
                0xF0000000u,
                "FPUFLAGS",
                PrimitiveType.Byte);
        }

        private void RewriteFcmp()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteSrc(di.op2, di.Address);
            m.Assign(FpuFlagGroup(), m.Cond(m.ISub(opDst, opSrc)));
        }

        private void RewriteFmove()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, MaybeCastFpuArgs);
            if (opDst == null)
            {
                EmitInvalid();
                return;
            }
            m.Assign(FpuFlagGroup(), m.Cond(opDst));
        }

        private void RewriteFmovecr()
        {
            var opSrc = (M68kImmediateOperand)di.op1;
            int n = opSrc.Constant.ToInt32();
            Expression src;
            if (fpuRomConstants.TryGetValue(n, out double d))
            {
                src = Constant.Real64(d);
                src.DataType = PrimitiveType.Real80;
            }
            else
            {
                src = host.PseudoProcedure("__fmovecr", PrimitiveType.Real64, opSrc.Constant);
            }
            var dst = orw.RewriteSrc(di.op2, di.Address);
            m.Assign(dst, src);
            m.Assign(binder.EnsureRegister(Registers.fpsr), m.Cond(dst));
        }

        private void RewriteFasin()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteDst(di.op2, di.Address, src, (s, d) =>
                host.PseudoProcedure("asin", s.DataType, s));
        }

        private void RewriteFintrz()
        {
            // C's trunc() is supposed to round
            // to zero.
            // http://en.cppreference.com/w/c/numeric/math/trunc
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteDst(di.op2, di.Address, src, (s, d) =>
                host.PseudoProcedure("trunc", s.DataType, s));
        }

        private void RewriteFsqrt()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteDst(di.op2, di.Address, src, (s, d) =>
                host.PseudoProcedure("sqrt", s.DataType, s));
        }

        private void RewriteFtan()
        {
            //$TODO: #include <math.h>
            var src = orw.RewriteSrc(di.op1, di.Address);
            var dst = orw.RewriteDst(di.op2, di.Address, src, (s, d) =>
                host.PseudoProcedure("tan", s.DataType, s));
        }

        private Expression MaybeCastFpuArgs(Expression src, Expression dst)
        {
            if (src.DataType != dst.DataType)
                return m.Cast(dst.DataType, src);
            else
                return src;
        }
    }
}
