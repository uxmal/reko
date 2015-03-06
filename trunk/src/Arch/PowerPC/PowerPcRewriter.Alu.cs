#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void MaybeEmitCr0(Expression e)
        {
            if (!instr.setsCR0)
                return;
            var cr0 = frame.EnsureFlagGroup(0x1, "cr0", PrimitiveType.Byte);
            emitter.Assign(cr0, emitter.Cond(e));
        }

        private void RewriteAdd()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
        }

        public void RewriteAddi()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
        }

        public void RewriteAddis()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = Shift16(dasm.Current.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
        }
        
        private void RewriteAdd(Expression opD, Expression opL, Expression opR)
        {
            if (opL.IsZero)
                emitter.Assign(opD, opR);
            else if (opR.IsZero)
                emitter.Assign(opD, opL);
            else 
                emitter.Assign(opD, emitter.IAdd(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteMflr()
        {
            var dst = RewriteOperand(instr.op1);
            var src = frame.EnsureRegister(Registers.lr);
            emitter.Assign(dst, src);
        }

        private void RewriteMtctr()
        {
            var src = RewriteOperand(instr.op1);
            var dst = frame.EnsureRegister(Registers.ctr);
            emitter.Assign(dst, src);
        }

        private void RewriteMtlr()
        {
            var src= RewriteOperand(instr.op1);
            var dst = frame.EnsureRegister(Registers.lr);
            emitter.Assign(dst, src);
        }

        public void RewriteNeg()
        {
            var opE = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Neg(opE));
            MaybeEmitCr0(opD);
        }

        public void RewriteOr()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            if (opL == opR)
            {
                emitter.Assign(opD, opL);
            }
            else
            {
                emitter.Assign(opD, emitter.Or(opL, opR));
            }
            MaybeEmitCr0(opD);
        }

        void RewriteRlwinm()
        {
            var rd = RewriteOperand(instr.op1);
            var rs = RewriteOperand(instr.op2);
            byte sh = ((Constant)RewriteOperand(instr.op3)).ToByte();
            byte mb = ((Constant)RewriteOperand(instr.op4)).ToByte();
            byte me = ((Constant)RewriteOperand(instr.op5)).ToByte();
            uint maskBegin = (uint)(1ul << (32 - mb)); 
            uint maskEnd = 1u << (31-me);
            uint mask =  maskBegin - maskEnd;
            if (sh == 0)
            {
                emitter.Assign(rd, emitter.And(rs, Constant.UInt32(mask)));
            }
            else
                throw new NotImplementedException();
        }

        public void RewriteSlw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Shl(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSrawi()
        {
            //$TODO: identical to Sraw? If so, merge instructions
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Sar(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubf()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }
    }
}
