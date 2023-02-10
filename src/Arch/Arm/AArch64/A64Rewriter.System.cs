#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
        private void RewriteDsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__dsb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance));
        }

        private void RewriteIsb()
        {
            var memBarrier = (BarrierOperand) instr.Operands[0];
            var name = $"__isb_{memBarrier.Option.ToString().ToLower()}";
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance));
        }

        private void RewriteMrs()
        {
            var intrinsic = host.Intrinsic("__mrs", true, PrimitiveType.Word32, RewriteOp(instr.Operands[1]));
            m.Assign(RewriteOp(0), intrinsic);
        }

        private void RewriteMsr()
        {
            var intrinsic = host.Intrinsic("__msr", true, PrimitiveType.Word32, RewriteOp(instr.Operands[0]), RewriteOp(instr.Operands[1]));
            m.SideEffect(intrinsic);
        }

        private void RewriteSmc()
        {
            var intrinsic = host.Intrinsic("__secure_monitor_call", true, VoidType.Instance, RewriteOp(instr.Operands[0]));
            m.SideEffect(intrinsic);
        }

        private void RewriteSvc()
        {
            var intrinsic = host.Intrinsic("__supervisor_call", true, VoidType.Instance, RewriteOp(instr.Operands[0]));
            m.SideEffect(intrinsic);
        }

    }
}
