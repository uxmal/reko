#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public partial class ArmRewriter
    {
        private void RewriteCps()
        {
			if (instr.ArchitectureDetail.CpsMode == ArmCpsMode.ID)
            {
                m.SideEffect(host.PseudoProcedure("__cps_id", VoidType.Instance));
                return;
            }
            NotImplementedYet();
        }

        private void RewriteDmb()
        {
            var memBarrier = instr.ArchitectureDetail.MemoryBarrier.ToString().ToLowerInvariant();
            var name = "__dmb_" + memBarrier;
            m.SideEffect(host.PseudoProcedure(name, VoidType.Instance));
        }

        private void RewriteMcr()
        {
            m.SideEffect(host.PseudoProcedure(
                "__mcr",
                VoidType.Instance,
                instr.ArchitectureDetail.Operands
                    .Select(o => Operand(o))
                    .ToArray()));
        }

        private void RewriteMrc()
        {
            var ops = instr.ArchitectureDetail.Operands
                    .Select(o => Operand(o))
                    .ToList();
            var regDst = ops.OfType<Identifier>().SingleOrDefault();
            ops.Remove(regDst);
            m.Assign(regDst, host.PseudoProcedure(
                "__mrc",
                PrimitiveType.Word32,
                ops.ToArray()));
        }

        private void RewriteMrs()
        {
            ConditionalSkip();
            m.Assign(Operand(Dst), host.PseudoProcedure("__mrs", PrimitiveType.Word32, Operand(Src1)));
        }

        private void RewriteMsr()
        {
            ConditionalSkip();
            m.SideEffect(host.PseudoProcedure("__msr", PrimitiveType.Word32, Operand(Dst), Operand(Src1)));
        }

        private void RewriteSvc()
        {
            rtlc = RtlClass.Call | RtlClass.Transfer;
            var svcNum = Operand(Dst);
            m.SideEffect(host.PseudoProcedure(
                PseudoProcedure.Syscall,
                VoidType.Instance,
                Operand(Dst)));
        }
    }
}
