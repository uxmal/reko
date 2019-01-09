#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
            //$TODO: require <math.h>
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, host.PseudoProcedure("fabs", PrimitiveType.Real64, opS));
        }

        public void RewriteFadd()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.FAdd(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFcfid()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, m.Cast(PrimitiveType.Real64, src));
        }

        public void RewriteFcmpo()
        {
            //$TODO: How to deal with the "orderered" part, i.e. 
            // if there are NaNs involved.
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }

        public void RewriteFcmpu()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }


        private void RewriteFctid()
        {
            //$TODO: require <math.h>
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, host.PseudoProcedure("round", PrimitiveType.Real64, src));
        }

        private void RewriteFctidz()
        {
            //$TODO: require <math.h>
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, host.PseudoProcedure("trunc", PrimitiveType.Real64, src));
        }

        private void RewriteFctiwz()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, m.Cast(PrimitiveType.Int32, src));
        }

        public void RewriteFdiv()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.FDiv(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFmr()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, opS);
            MaybeEmitCr1(opD);
        }

        public void RewriteFmadd(PrimitiveType dt, Func<Expression,Expression,Expression> add, bool negate)
        {
            bool needsCast = dt == PrimitiveType.Real32;
            var opb = RewriteOperand(instr.op4);
            var opc = RewriteOperand(instr.op3);
            var opa = RewriteOperand(instr.op2);
            var opt = RewriteOperand(instr.op1);
            if (needsCast)
            {
                opa = m.Cast(dt, opa);
                opb = m.Cast(dt, opb);
                opc = m.Cast(dt, opc);
            }

            var exp = add(m.FMul(opa, opc), opb);
            if (negate)
            {
                exp = m.FNeg(exp);
            }
            if (needsCast)
            {
                exp=m.Cast(PrimitiveType.Real64, exp);
            }
            m.Assign(opt, exp);
            MaybeEmitCr1(opt);
        }

        public void RewriteFmul()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.FMul(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFneg()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Neg(opS));
            MaybeEmitCr1(opD);
        }

        public void RewriteFrsp()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Cast(PrimitiveType.Real32, opS));
            MaybeEmitCr1(opD);
        }

        private void RewriteFsel()
        {
            var opD = RewriteOperand(instr.op1);
            var opC = RewriteOperand(instr.op2);
            var opT = RewriteOperand(instr.op3);
            var opE = RewriteOperand(instr.op4);
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
            //$TODO: include math.h
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(
                dst,
                host.PseudoProcedure("sqrt", PrimitiveType.Real64, src));
        }

        private void RewriteFrsqrte()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(
                dst,
                host.PseudoProcedure("__frsqrte", PrimitiveType.Real64, src));
        }

        public void RewriteFsub()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.FSub(opL, opR));
            
            MaybeEmitCr1(opD);
        }

        public void RewriteMffs()
        {
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, binder.EnsureRegister(arch.fpscr));
            MaybeEmitCr1(opD);
        }

        public void RewriteMtfsf()
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            m.SideEffect(
                host.PseudoProcedure("__mtfsf",
                    VoidType.Instance,
                    op2,
                    op1));
            MaybeEmitCr1(op1);
        }
    }
}
