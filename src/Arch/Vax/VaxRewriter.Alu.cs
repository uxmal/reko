#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        private Expression Bic(Expression a, Expression mask)
        {
            return emitter.And(a, emitter.Comp(mask));
        }

        private void RewriteAddp4()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Pointer32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Pointer32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            emitter.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_addp4", 
                    PrimitiveType.Byte,
                    op0, op1, op2, op3));
            var c = FlagGroup(FlagM.CF);
            emitter.Assign(c, Constant.False());
        }

        private void RewriteAddp6()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Pointer32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Pointer32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Pointer32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            emitter.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_addp6", 
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            emitter.Assign(c, Constant.False());
        }

        private void RewriteAdwc()
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(1, PrimitiveType.Word32,
                e => emitter.IAdd(
                        emitter.IAdd(e, op1),
                        FlagGroup(FlagM.CF)));
            AllFlags(dst);
        }

        private void RewriteAlu2(PrimitiveType width, Func<Expression, Expression, Expression> fn, Action<Expression> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var dst = RewriteDstOp(1, width, e => fn(e, op1));
            genFlags(dst);
        }

        private void RewriteAlu3(PrimitiveType width, Func<Expression, Expression, Expression> fn, Action<Expression> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var op2 = RewriteSrcOp(1, width);
            var dst = RewriteDstOp(2, width, e => fn(op2, op1));
            genFlags(dst);
        }

        private void RewriteAsh(PrimitiveType width)
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.SByte);
            var c = op1 as Constant;
            if (c != null)
            {
                Func<Expression, Expression, Expression> fn;
                var sh = c.ToInt16();
                if (sh > 0)
                {
                    fn = emitter.Shl;
                }
                else
                {
                    fn = emitter.Sar;
                    sh = (short) -sh;
                }
                var op2 = RewriteSrcOp(1, width);
                var dst = RewriteDstOp(2, width, e => fn(op2, Constant.SByte((sbyte)sh)));
                this.NZV0(dst);
                return;
            }
            throw new NotImplementedException();
        }

        private void RewriteAshp()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Pointer32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Pointer32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Pointer32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            emitter.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_ashp",
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            emitter.Assign(c, Constant.False());
        }


    }
}
