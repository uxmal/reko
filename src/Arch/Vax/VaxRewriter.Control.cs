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
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter
    {
        private void RewriteAobleq()
        {
            var limit = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(
                1,
                PrimitiveType.Word32,
                e => emitter.IAdd(e, emitter.Word32(1)));
            AllFlags(dst);
            emitter.Branch(
                emitter.Le(dst, limit),
                ((AddressOperand)dasm.Current.Operands[2]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteBb(bool set)
        {
            var pos = RewriteSrcOp(0, PrimitiveType.Word32);
            var @base = RewriteSrcOp(1, PrimitiveType.Word32);
            Expression test = emitter.And(
                @base,
                emitter.Shl(emitter.Word32(1), pos));
            if (set)
            {
                test = emitter.Ne0(test);
            }
            else
            {
                test = emitter.Eq0(test);
            }
            emitter.Branch(test,
                ((AddressOperand)dasm.Current.Operands[2]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteBranch(ConditionCode cc, FlagM flags)
        {
            emitter.Branch(
                emitter.Test(cc, FlagGroup(flags)),
                ((AddressOperand)dasm.Current.Operands[0]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteRet()
        {
            emitter.Return(0, 0);
            rtlc.Class = RtlClass.Transfer;
        }
    }
}
