#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

        private void Rewrite_psq_l(bool update) {

            var (ea, baseReg) = Rewrite_psq_EffectiveAddress();
            var tmp1 = binder.CreateTemporary(PrimitiveType.Word64);
            var tmp2 = binder.CreateTemporary(fpPair);
            m.Assign(tmp1, m.Mem64(ea));
            m.Assign(tmp2, host.Intrinsic(
                "__unpack_quantized",
                false,
                fpPair,
                tmp1,
                ImmOperand(instr.Operands[3]),
                ImmOperand(instr.Operands[4])));
            m.Assign(RewriteOperand(instr.Operands[0]), tmp2);
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

            m.Assign(tmp1, RewriteOperand(instr.Operands[0]));

            m.Assign(tmp2, host.Intrinsic(
                "__pack_quantized",
                false,
                fpPair,
                tmp1,
                RewriteOperand(instr.Operands[3]),
                RewriteOperand(instr.Operands[4])));

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
            if (((RegisterOperand)instr.Operands[1]).Register.Number == 0)
            {
                ea = RewriteOperand(instr.Operands[2]);
                baseReg = ea;
            }
            else
            {
                ea = RewriteOperand(instr.Operands[1]);
                baseReg = ea;
                if (instr.Operands[2] is RegisterOperand rIdx)
                {
                    if (rIdx.Register.Number != 0)
                    {
                        ea = m.IAdd(ea, binder.EnsureRegister(rIdx.Register));
                    }
                }
                else
                {
                    var offset = ((ImmediateOperand) instr.Operands[2]).Value.ToInt64();
                    ea = m.IAdd(ea, Constant.Create(arch.SignedWord, offset));
                }
            }
            return (ea, baseReg);
        }

        private void Rewrite_ps_cmpo(string intrinsic) {

            var cr = RewriteOperand(instr.Operands[0]);
            var opA = RewriteOperand(instr.Operands[1]);
            var opB = RewriteOperand(instr.Operands[2]);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, opA);
            m.Assign(tmpB, opB);
            m.Assign(cr, host.Intrinsic(intrinsic, false, cr.DataType, tmpA, tmpB));
        }

        private void Rewrite_ps_mr()
        {
            m.Assign(
                RewriteOperand(instr.Operands[0]),
                RewriteOperand(instr.Operands[1]));
        }

        private void RewritePairedInstruction_Src1(string intrinsic)
        {
            var src = RewriteOperand(instr.Operands[1]);
            var dst = RewriteOperand(instr.Operands[0]);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, src);
            m.Assign(tmpD, host.Intrinsic(intrinsic, false, fpPair, tmpA));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

        private void RewritePairedInstruction_Src2(string intrinsic)
        {
            var srcA = RewriteOperand(instr.Operands[1]);
            var srcB = RewriteOperand(instr.Operands[2]);
            var dst = RewriteOperand(instr.Operands[0]);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, srcA);
            m.Assign(tmpB, srcB);
            m.Assign(tmpD, host.Intrinsic(intrinsic, false, fpPair, tmpA, tmpB));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

        private void RewritePairedInstruction_Src3(string intrinsic)
        {
            var srcA = RewriteOperand(instr.Operands[1]);
            var srcB = RewriteOperand(instr.Operands[2]);
            var srcC = RewriteOperand(instr.Operands[3]);
            var dst = RewriteOperand(instr.Operands[0]);
            var tmpA = binder.CreateTemporary(fpPair);
            var tmpB = binder.CreateTemporary(fpPair);
            var tmpC= binder.CreateTemporary(fpPair);
            var tmpD = binder.CreateTemporary(fpPair);
            m.Assign(tmpA, srcA);
            m.Assign(tmpB, srcB);
            m.Assign(tmpC, srcC);
            m.Assign(tmpD, host.Intrinsic(intrinsic, false, fpPair, tmpA, tmpB, tmpC));
            m.Assign(dst, tmpD);
            MaybeEmitCr1(m.Array(PrimitiveType.Real32, dst, m.Int32(0)));
        }

    }
}
