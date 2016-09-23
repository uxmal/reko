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

using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter
    {
        private void RewriteB()
        {
            var addr = Address.Ptr32((uint)ops[0].ImmediateValue.Value);
            if (instr.ArchitectureDetail.CodeCondition == ArmCodeCondition.AL)
            {
                ric.Class = RtlClass.Transfer;
                emitter.Goto(addr);
            }
            else
            {
                ric.Class = RtlClass.ConditionalTransfer;
                emitter.Branch(TestCond(instr.ArchitectureDetail.CodeCondition), addr, RtlClass.ConditionalTransfer);
            }
        }

        private void RewriteBl()
        {
            ric.Class = RtlClass.Transfer;
            emitter.Call(
                Address.Ptr32((uint)ops[0].ImmediateValue.Value),
                0);
        }

        private void RewriteBlx()
        {
            ric.Class = RtlClass.Transfer;
            emitter.Call(RewriteOp(ops[0]), 0);
        }

        private void RewriteBx()
        {
            ric.Class = RtlClass.Transfer;
            emitter.Goto(RewriteOp(ops[0]));
        }

        private void RewriteCbnz(Func<Expression, Expression> ctor)
        {
            ric.Class = RtlClass.ConditionalTransfer;
            emitter.Branch(ctor(RewriteOp(ops[0])),
                Address.Ptr32((uint)ops[1].ImmediateValue.Value),
                RtlClass.Transfer);
        }

        private void RewriteIt()
        {
            this.itState = instr.Bytes[0] & 0xF;
            this.itStateFirst = instr.Bytes[0] >> 4;
            this.itStateCondition = instr.ArchitectureDetail.CodeCondition;
        }

        private void RewriteTrap()
        {
            emitter.SideEffect(host.PseudoProcedure("__syscall", VoidType.Instance, Constant.UInt32(instr.Bytes[0])));
        }

        private void RewriteUdf()
        {
            emitter.SideEffect(host.PseudoProcedure("__syscall", VoidType.Instance, Constant.UInt32(instr.Bytes[0])));
        }
    }
}
