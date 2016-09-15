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
            var cr1 = frame.EnsureFlagGroup(arch.cr, 0x2, "cr1", PrimitiveType.Byte);
            emitter.Assign(cr1, emitter.Cond(e));
        }

        public void RewriteFadd()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.FAdd(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFcfid()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.Assign(dst, emitter.Cast(PrimitiveType.Real64, src));
        }

        public void RewriteFcmpu()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Cond(emitter.FSub(opL, opR)));
        }

        private void RewriteFctiwz()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.Assign(dst, emitter.Cast(PrimitiveType.Int32, src));
        }

        public void RewriteFdiv()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.FDiv(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFmr()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, opS);
            MaybeEmitCr1(opD);
        }

        public void RewriteFmadd()
        {
            var opS = RewriteOperand(instr.op4);
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.FAdd(opS, emitter.FMul(opL, opR)));
            MaybeEmitCr1(opD);
        }

        public void RewriteFmsub()
        {
            var opc = RewriteOperand(instr.op4);
            var opb = RewriteOperand(instr.op3);
            var opa = RewriteOperand(instr.op2);
            var opt = RewriteOperand(instr.op1);
            emitter.Assign(opt, emitter.FSub(emitter.FMul(opa, opb), opc));
            MaybeEmitCr1(opt);
        }

        public void RewriteFmul()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.FMul(opL, opR));
            MaybeEmitCr1(opD);
        }

        public void RewriteFneg()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Neg(opS));
            MaybeEmitCr1(opD);
        }

        public void RewriteFrsp()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Cast(PrimitiveType.Real32, opS));
            MaybeEmitCr1(opD);
        }

        public void RewriteFsub()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.FSub(opL, opR));
            
            MaybeEmitCr1(opD);
        }

        public void RewriteMffs()
        {
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, frame.EnsureRegister(arch.fpscr));
            MaybeEmitCr1(opD);
        }

        public void RewriteMtfsf()
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            emitter.SideEffect(
                host.PseudoProcedure("__mtfsf",
                    VoidType.Instance,
                    op2,
                    op1));
            MaybeEmitCr1(op1);
        }
    }
}
