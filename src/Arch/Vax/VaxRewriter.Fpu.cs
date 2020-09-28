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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter
    {
        private void RewriteEmod(string fnname, PrimitiveType floatType, PrimitiveType extType)
        {
            var mulr = RewriteSrcOp(0, floatType);
            var mulrx = RewriteSrcOp(1, extType);
            var muld = RewriteSrcOp(2, floatType);
            var integral = RewriteSrcOp(3, PrimitiveType.Int32);
            var frac = RewriteSrcOp(4, floatType);
            var nzv = FlagGroup(FlagM.NZV);
            m.Assign(
                nzv,
                host.PseudoProcedure(fnname, nzv.DataType, mulr, mulrx, muld,
                m.Out(PrimitiveType.Word32, integral),
                m.Out(floatType, frac)));
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
        }

        private bool RewriteFpu2(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var dst = RewriteDstOp(1, width, e => fn(e, op1));
            return genFlags(dst);
        }

        private bool RewriteFpu3(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var op2 = RewriteSrcOp(1, width);
            var dst = RewriteDstOp(2, width, e => fn(op2, op1));
            if (dst == null)
            {
                m.Invalid();
                return false;
            }
            return genFlags(dst);
        }
    }
}
