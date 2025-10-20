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

using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private void RewriteFabss()
        {
            var dst = RewriteOp(instrCur.Operands[1]);
            var src = RewriteOp(instrCur.Operands[0]);
            m.Assign(dst, m.Fn(FpOps.FAbs32, src));
        }

        private void RewriteFadds()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst =  RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FAdd(src1, src2));
        }

        private void RewriteFcmped()
        {
            var f1 = RewriteOp(instrCur.Operands[0]);
            var f2 = RewriteOp(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU")!);
            m.Assign(grf, m.Cond(grf.DataType, m.FSub(f1, f2)));
        }

        private void RewriteFcmpes()
        {
            var f1 = RewriteOp(instrCur.Operands[0]);
            var f2 = RewriteOp(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU")!);
            m.Assign(grf, m.Cond(grf.DataType, m.FSub(f1, f2)));
        }

        private void RewriteFcmpd()
        {
            var r1 = RewriteOp(instrCur.Operands[0]);
            var r2 = RewriteOp(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU")!);
            m.Assign(grf, m.Cond(grf.DataType, m.FSub(r2, r1)));
        }

        private void RewriteFcmpq()
        {
            var r1 = RewriteOp(instrCur.Operands[0]);
            var r2 = RewriteOp(instrCur.Operands[1]);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU")!);
            m.Assign(grf, m.Cond(grf.DataType, m.FSub(r2, r1)));
        }

        private void RewriteFcmps()
        {
            var r1 = RewriteRegister(0);
            var r2 = RewriteRegister(1);
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ELGU")!);
            m.Assign(grf, m.Cond(grf.DataType, m.FSub(r2, r1)));
        }

        private void RewriteFdivd()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst = RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FDiv(src1, src2));
        }

        private void RewriteFdivs()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst =  RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FDiv(src1, src2));
        }

        private void RewriteFitod()
        {
            var dtFrom = PrimitiveType.Int32;
            var dtTo = PrimitiveType.Real64;
            var fpDst = RewriteOp(instrCur.Operands[1]);
            m.Assign(fpDst, m.Convert(MaybeSlice(RewriteOp(0), dtFrom), dtFrom, dtTo));
        }

        private void RewriteFitoq()
        {
            var fpDst = RewriteOp(1);
            var dtFrom = PrimitiveType.Int32;
            var dtTo = PrimitiveType.Real128;
            m.Assign(fpDst, m.Convert(MaybeSlice(RewriteOp(0), dtFrom), dtFrom, dtTo));
        }

        private void RewriteFdtos()
        {
            var fpDst = RewriteOp(instrCur.Operands[1]);
            m.Assign(fpDst, m.Convert(RewriteOp(0), PrimitiveType.Real64, PrimitiveType.Real32));
        }

        private void RewriteFstod()
        {
            var fpDst = RewriteOp(instrCur.Operands[1]);
            var dt = PrimitiveType.Real64;
            m.Assign(fpDst, m.Convert(RewriteOp(0), PrimitiveType.Real32, dt));
        }

        private void RewriteFitos()
        {
            var dst = (RegisterStorage) instrCur.Operands[1];
            var fpDst = binder.EnsureRegister(dst);
            var dt = PrimitiveType.Real32;
            m.Assign(fpDst, m.Convert(MaybeSlice(RewriteOp(0), PrimitiveType.Int32), PrimitiveType.Int32, dt));
        }

        private void RewriteFmovs()
        {
            var src = RewriteOp(instrCur.Operands[0]);
            var dst = RewriteOp(instrCur.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteFmuld()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst = RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FMul(src1, src2));
        }

        private void RewriteFmuls()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst =  RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FMul(src1, src2));
        }

        private void RewriteFnegs()
        {
            var src = RewriteOp(instrCur.Operands[0]);
            var dst = RewriteOp(instrCur.Operands[1]);
            m.Assign(dst, m.Neg(src));
        }

        private void RewriteFsubs()
        {
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            var dst =  RewriteOp(instrCur.Operands[2]);
            m.Assign(dst, m.FSub(src1, src2));
        }
    }
}
