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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private static ArrayType fpPair = new ArrayType(PrimitiveType.Real32, 2);

        private void RewritePairedInstruction_Src1(string intrinsic)
        {
            var src = RewriteOperand(instr.op2);
            var dst = RewriteOperand(instr.op1);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, src);
            m.Assign(tmpD, host.PseudoProcedure(intrinsic, fpPair, tmpA));
            m.Assign(dst, tmpD);
            MaybeEmitCr0(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

        private void RewritePairedInstruction_Src2(string intrinsic)
        {
            var srcA = RewriteOperand(instr.op2);
            var srcB = RewriteOperand(instr.op3);
            var dst = RewriteOperand(instr.op1);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, srcA);
            m.Assign(tmpB, srcB);
            m.Assign(tmpD, host.PseudoProcedure(intrinsic, fpPair, tmpA, tmpB));
            m.Assign(dst, tmpD);
            MaybeEmitCr0(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }
    }
}
