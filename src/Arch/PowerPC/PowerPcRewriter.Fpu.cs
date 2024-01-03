#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void MaybeEmitCr1(Expression e)
        {
            if (!instr.setsCR0)
                return;
            var cr1 = binder.EnsureFlagGroup(arch.cr, 0x2, "cr1", PrimitiveType.Byte);
            m.Assign(cr1, m.Cond(e));
        }

        public void RewriteFabs()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.Fn(FpOps.fabs, opS));
        }

        public void RewriteFadd()
        {
            var opL = RewriteOperand(1);
            var opR = RewriteOperand(2);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.FAdd(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFcfid()
        {
            var dst = RewriteOperand(0);
            var src = RewriteOperand(1);
            var dtSrc = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(dst, m.Convert(src, dtSrc, PrimitiveType.Real64));
        }

        public void RewriteFcmpo()
        {
            //$TODO: How to deal with the "orderered" part, i.e. 
            // if there are NaNs involved.
            var opL = RewriteOperand(1);
            var opR = RewriteOperand(2);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }

        public void RewriteFcmpu()
        {
            var opL = RewriteOperand(1);
            var opR = RewriteOperand(2);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }

        private void RewriteFctid()
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Fn(FpOps.round, src));
        }

        private void RewriteFctidz()
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Fn(FpOps.trunc, src));
        }

        private void RewriteFctiw()
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            var tmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(tmp, src);
            m.Assign(dst, m.Fn(fctiw, tmp));
        }

        private void RewriteFctiwz()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, m.Convert(src, PrimitiveType.Real64, PrimitiveType.Int32));
        }

        public void RewriteFdiv()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.FDiv(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFmr()
        {
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, opS);
            MaybeEmitCr1(opD);
        }

        public void RewriteFmadd(PrimitiveType dt, Func<Expression,Expression,Expression> add, bool negate)
        {
            bool needsCast = dt == PrimitiveType.Real32;
            var opb = RewriteOperand(instr.Operands[3]);
            var opc = RewriteOperand(instr.Operands[2]);
            var opa = RewriteOperand(instr.Operands[1]);
            var opt = RewriteOperand(instr.Operands[0]);
            if (needsCast)
            {
                opa = m.Slice(opa, dt);
                opb = m.Slice(opb, dt);
                opc = m.Slice(opc, dt);
            }

            var exp = add(m.FMul(opa, opc), opb);
            if (negate)
            {
                exp = m.FNeg(exp);
            }
            if (needsCast)
            {
                exp = m.Convert(exp, dt, PrimitiveType.Real64);
            }
            m.Assign(opt, exp);
            MaybeEmitCr1(opt);
        }

        public void RewriteFmul()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.FMul(opL, opR));
            MaybeEmitCr1(opD);
        }

        private void RewriteFnabs()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.FNeg(m.Fn(FpOps.fabs, opS)));
            MaybeEmitCr1(opD);
        }

        public void RewriteFneg()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.Neg(opS));
            MaybeEmitCr1(opD);
        }

        private void RewriteFres()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            var tmp = binder.CreateTemporary(PrimitiveType.Real32);
            m.Assign(tmp, m.Fn(
                fre.MakeInstance(PrimitiveType.Real32),
                m.Convert(opS, PrimitiveType.Real64, PrimitiveType.Real32)));
            m.Assign(opD, m.Convert(tmp, tmp.DataType, PrimitiveType.Real64));
            MaybeEmitCr1(tmp);
        }
        public void RewriteFrsp()
        {
            var opS = RewriteOperand(1);
            var opD = RewriteOperand(0);
            m.Assign(opD, m.Convert(opS, PrimitiveType.Real64, PrimitiveType.Real32));
            MaybeEmitCr1(opD);
        }

        private void RewriteFsel()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opC = RewriteOperand(instr.Operands[1]);
            var opT = RewriteOperand(instr.Operands[2]);
            var opE = RewriteOperand(instr.Operands[3]);
            m.Assign(
                opD,
                m.Conditional(
                    PrimitiveType.Real64,
                    m.FGe(opC, Constant.Real64(0.0)),
                    opT,
                    opE));
        }

        private void RewriteFsqrt()
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Fn(FpOps.sqrt, src));
            MaybeEmitCr1(dst);
        }

        private void RewriteFsqrts()
        {
            var src = RewriteOperand(1);
            var dst = RewriteOperand(0);
            var tmp = binder.CreateTemporary(PrimitiveType.Real32);
            m.Assign(tmp, m.Fn(FpOps.sqrtf, m.Convert(src, PrimitiveType.Real64, PrimitiveType.Real32)));
            m.Assign(dst, m.Convert(tmp, tmp.DataType, PrimitiveType.Real64));
            MaybeEmitCr1(tmp);
        }

        private void RewriteFrsqrte()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, m.Fn(frsqrte, src));
        }

        public void RewriteFsub()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.FSub(opL, opR));
            
            MaybeEmitCr1(opD);
        }

        public void RewriteMffs()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, binder.EnsureRegister(arch.fpscr));
            MaybeEmitCr1(opD);
        }

        private void RewriteMtfsb1()
        {
            var op = RewriteOperand(0);
            m.SideEffect(m.Fn(mtfsb1, op));
        }

        private void RewriteMtfsf()
        {
            var op1 = ((ImmediateOperand)instr.Operands[0]).Value;
            var op2 = RewriteOperand(1);
            m.SideEffect(
                m.Fn(mtfsf.MakeInstance(op2.DataType),
                op2,
                op1));
            MaybeEmitCr1(op1);
        }
    }
}
