#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private Identifier Grf(FlagM grf)
        {
            return frame.EnsureFlagGroup(
                (uint) grf, 
                arch.GrfToString((uint) grf),
                (grf & (grf - 1)) != 0 ? PrimitiveType.Byte : PrimitiveType.Bool);
        }

        private void RewriteBranch(Expression cond)
        {
            // SPARC architecture always has delay slot.
            var rtlClass = RtlClass.ConditionalTransfer | RtlClass.Delay;
            if (instrCur.Annul)
                rtlClass |= RtlClass.Annul;
            emitter.Branch(cond, ((AddressOperand) instrCur.Op1).Address, rtlClass);
        }

        private void RewriteCall()
        {
            emitter.CallD(((AddressOperand) instrCur.Op1).Address, 0);
        }

        private void RewriteJmpl()
        {
            var rDst = instrCur.Op3 as RegisterOperand;
            var dst = RewriteOp(instrCur.Op3, true);
            var src1 = RewriteOp(instrCur.Op1, true);
            var src2 = RewriteOp(instrCur.Op2, true);
            emitter.Assign(dst, instrCur.Address);
            var target = SimplifySum(src1, src2);
            if (rDst.Register == Registers.o7)
            {
                emitter.CallD(target, 0);
            }
            else if (rDst.Register == Registers.g0)
            {
                emitter.ReturnD(0, 0);
            }
            else
            {
                emitter.GotoD(target);
            }
        }

        private void RewriteRett()
        {
        }

        private void RewriteTrap(Expression cond)
        {
            //$REVIEW: does a SPARC trap instruction have a delay slot?
            var src1 = RewriteOp(instrCur.Op1, true);
            var src2 = RewriteOp(instrCur.Op2, true);
            emitter.If(
                cond,
                new RtlSideEffect(
                    PseudoProc(
                        "__syscall", 
                        VoidType.Instance, 
                        SimplifySum(src1, src2))));
        }
    }
}
