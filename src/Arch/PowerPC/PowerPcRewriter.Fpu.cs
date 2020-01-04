#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, host.PseudoProcedure("fabs", PrimitiveType.Real64, opS));
        }

        public void RewriteFadd()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.FAdd(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFcfid()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, m.Cast(PrimitiveType.Real64, src));
        }

        public void RewriteFcmpo()
        {
            //$TODO: How to deal with the "orderered" part, i.e. 
            // if there are NaNs involved.
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }

        public void RewriteFcmpu()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Cond(m.FSub(opL, opR)));
        }


        private void RewriteFctid()
        {
            //$TODO: require <math.h>
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("round", PrimitiveType.Real64, src));
        }

        private void RewriteFctidz()
        {
            //$TODO: require <math.h>
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("trunc", PrimitiveType.Real64, src));
        }

        private void RewriteFctiwz()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, m.Cast(PrimitiveType.Int32, src));
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
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.FMul(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFneg()
        {
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Neg(opS));
            MaybeEmitCr1(opD);
        }

        public void RewriteFrsp()
        {
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Cast(PrimitiveType.Real32, opS));
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
            //$TODO: include math.h
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(
                dst,
                host.PseudoProcedure("sqrt", PrimitiveType.Real64, src));
        }

        private void RewriteFrsqrte()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(
                dst,
                host.PseudoProcedure("__frsqrte", PrimitiveType.Real64, src));
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

        public void RewriteMtfsf()
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);
            m.SideEffect(
                host.PseudoProcedure("__mtfsf",
                    VoidType.Instance,
                    op2,
                    op1));
            MaybeEmitCr1(op1);
        }
    }
}
