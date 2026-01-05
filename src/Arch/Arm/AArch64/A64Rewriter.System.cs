#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private static readonly StringType labelType = StringType.NullTerminated(PrimitiveType.Byte);

        private void RewriteDmb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(intrinsic.dmb, Constant.String(label, labelType)));
        }

        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(intrinsic.dsb, Constant.String(label, labelType)));
        }

        private void RewriteIsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var label = memBarrier.Option.ToString().ToLower();
            m.SideEffect(m.Fn(intrinsic.isb, Constant.String(label, labelType)));
        }

        private void RewriteMrs()
        {
            m.Assign(RewriteOp(0), m.Fn(intrinsic.mrs, RewriteOp(1)));
        }

        private void RewriteMsr()
        {
            m.SideEffect(m.Fn(intrinsic.msr, RewriteOp(0), RewriteOp(1)));
        }

        private void RewriteSmc()
        {
            m.SideEffect(m.Fn(intrinsic.smc, RewriteOp(0)));
        }

        private void RewriteSvc()
        {
            m.SideEffect(m.Fn(intrinsic.svc, RewriteOp(0)));
        }
    }
}
