#region License
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private Identifier Grf(FlagM grf)
        {
            return binder.EnsureFlagGroup(
                Registers.psr,
                (uint) grf, 
                arch.GrfToString(Registers.psr, "", (uint) grf),
                (grf & (grf - 1)) != 0 ? PrimitiveType.Byte : PrimitiveType.Bool);
        }

        private void RewriteBranch(Expression cond)
        {
            // SPARC architecture always has delay slot.
            var rtlClass = RtlClass.ConditionalTransfer | RtlClass.Delay;
            if (instrCur.Annul)
                rtlClass |= RtlClass.Annul;
            this.rtlc = rtlClass;
            if (cond is Constant c && c.ToBoolean())
            {
                m.Goto(((AddressOperand)instrCur.Op1).Address, rtlClass);
            }
            else
            {
                m.Branch(cond, ((AddressOperand)instrCur.Op1).Address, rtlClass);
            }
        }

        private void RewriteCall()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.CallD(((AddressOperand)instrCur.Op1).Address, 0);
        }

        private void RewriteJmpl()
        {
            rtlc = RtlClass.Transfer;
            var rDst = instrCur.Op3 as RegisterOperand;
            var src1 = RewriteOp(instrCur.Op1);
            var src2 = RewriteOp(instrCur.Op2);
            if (rDst.Register != Registers.g0)
            {
                var dst = RewriteOp(instrCur.Op3);
                m.Assign(dst, instrCur.Address);
            }
            var target = SimplifySum(src1, src2);
            if (rDst.Register == Registers.o7)
            {
                m.CallD(target, 0);
            }
            else if (rDst.Register == Registers.g0)
            {
                m.ReturnD(0, 0);
            }
            else
            {
                m.GotoD(target);
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
            m.BranchInMiddleOfInstruction(
                cond.Invert(),
                instrCur.Address + instrCur.Length,
                RtlClass.ConditionalTransfer);
            m.SideEffect(
                    host.PseudoProcedure(
                        PseudoProcedure.Syscall, 
                        VoidType.Instance, 
                    SimplifySum(src1, src2)));
        }
    }
}
