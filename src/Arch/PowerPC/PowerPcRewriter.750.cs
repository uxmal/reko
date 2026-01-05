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

using Reko.Core;
using Reko.Core.Expressions;
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

        private void Rewrite_psq_l(bool update) {

            var (ea, baseReg) = Rewrite_psq_EffectiveAddress();
            var tmp1 = binder.CreateTemporary(PrimitiveType.Word64);
            var tmp2 = binder.CreateTemporary(fpPair);
            m.Assign(tmp1, m.Mem64(ea));
            m.Assign(tmp2, m.Fn(
                unpack_quantized,
                tmp1,
                ImmOperand(3),
                ImmOperand(4)));
            m.Assign(RewriteOperand(0), tmp2);
            if (update)
            {
                m.Assign(baseReg, ea);
            }
        }

        private void Rewrite_psq_st(bool update)
        {
            var (ea, baseReg) = Rewrite_psq_EffectiveAddress();
            var tmp1 = binder.CreateTemporary(PrimitiveType.Word64);
            var tmp2 = binder.CreateTemporary(fpPair);

            m.Assign(tmp1, RewriteOperand(0));

            m.Assign(tmp2, m.Fn(
                pack_quantized,
                tmp1,
                RewriteOperand(3),
                RewriteOperand(4)));

            m.Assign(m.Mem64(ea), tmp2);

            if (update)
            {
                m.Assign(baseReg, ea);
            }
        }

        private (Expression, Expression) Rewrite_psq_EffectiveAddress()
        {
            Expression ea;
            Expression baseReg;
            if (((RegisterStorage)instr.Operands[1]).Number == 0)
            {
                ea = RewriteOperand(2);
                baseReg = ea;
            }
            else
            {
                ea = RewriteOperand(1);
                baseReg = ea;
                if (instr.Operands[2] is RegisterStorage rIdx)
                {
                    if (rIdx.Number != 0)
                    {
                        ea = m.IAdd(ea, binder.EnsureRegister(rIdx));
                    }
                }
                else
                {
                    var offset = ((Constant)instr.Operands[2]).ToInt64();
                    ea = m.IAdd(ea, m.Const(arch.SignedWord, offset));
                }
            }
            return (ea, baseReg);
        }

        private void Rewrite_ps_cmpo(IntrinsicProcedure intrinsic) {

            var cr = RewriteOperand(0);
            var opA = RewriteOperand(1);
            var opB = RewriteOperand(2);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, opA);
            m.Assign(tmpB, opB);
            m.Assign(cr, m.Fn(intrinsic.MakeInstance(fpPair, cr.DataType), tmpA, tmpB));
        }

        private void Rewrite_ps_mr()
        {
            m.Assign(
                RewriteOperand(0),
                RewriteOperand(1));
        }

        private void RewritePairedInstruction_Src1(IntrinsicProcedure intrinsic)
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, src);
            m.Assign(tmpD, m.Fn(intrinsic.MakeInstance(fpPair), tmpA));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

        private void RewritePairedInstruction_Src2(IntrinsicProcedure intrinsic)
        {
            var srcA = RewriteOperand(1);
            var srcB = RewriteOperand(2);
            var dst = RewriteOperand(0);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, srcA);
            m.Assign(tmpB, srcB);
            m.Assign(tmpD, m.Fn(intrinsic.MakeInstance(fpPair), tmpA, tmpB));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

        private void RewritePairedInstruction_Src3(IntrinsicProcedure intrinsic)
        {
            var srcA = RewriteOperand(1);
            var srcB = RewriteOperand(2);
            var srcC = RewriteOperand(3);
            var dst = RewriteOperand(0);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            var tmpC= binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, srcA);
            m.Assign(tmpB, srcB);
            m.Assign(tmpC, srcC);
            m.Assign(tmpD, m.Fn(intrinsic.MakeInstance(fpPair), tmpA, tmpB, tmpC));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

    }
}
