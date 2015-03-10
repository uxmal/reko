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

using Decompiler.Core;
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

        public void RewriteAddic()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = Shift16(dasm.Current.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
            var xer = frame.EnsureRegister(Registers.xer);
            emitter.Assign(xer, emitter.Cond(opD));
        }

        public void RewriteAddis()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = Shift16(dasm.Current.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
        }

        private void RewriteAddze()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = frame.EnsureRegister(Registers.xer);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
            emitter.Assign(opR, emitter.Cond(opD));
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

        private void RewriteAnd(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = (opL == opR)
                ? opL
                : emitter.And(opL, opR);
            if (negate)
                s = emitter.Comp(s);
            emitter.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteAndis()
        {
            var opD = RewriteOperand(instr.op1);
            
            emitter.Assign(
                opD,
                emitter.And(
                    RewriteOperand(instr.op2),
                    Shift16(dasm.Current.op3)));
            MaybeEmitCr0(opD);
        }

        private void RewriteCmp()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.Assign(cr, emitter.Cond(
                emitter.ISub(r, i)));
        }

        private void RewriteCmpli()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.Assign(cr, emitter.Cond(
                emitter.ISub(r, i)));
        }

        private void RewriteCmplw()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.Assign(cr, emitter.Cond(
                emitter.ISub(r, i)));
        }
        
        private void RewriteCmpwi()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.Assign(cr, emitter.Cond(
                emitter.ISub(r, i)));
        }

        private void RewriteCntlzw()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.Assign(dst, PseudoProc("__cntlzw", PrimitiveType.UInt32, src));
        }

        private void RewriteCreqv()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.SideEffect(PseudoProc("__creqv", VoidType.Instance, cr, r, i));
        }

        private void RewriteCrxor()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.SideEffect(PseudoProc("__crxor", VoidType.Instance, cr, r, i));
        }

        private void RewriteDivw()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.SDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteDivwu()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.UDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteExtsb()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            var tmp = frame.CreateTemporary(PrimitiveType.SByte);
            emitter.Assign(tmp, emitter.Cast(tmp.DataType, opS));
            emitter.Assign(
                opD, 
                emitter.Cast(
                    PrimitiveType.Create(Domain.SignedInt, opD.DataType.Size),
                    tmp));
            MaybeEmitCr0(opD);
        }

        private void RewriteMflr()
        {
            var dst = RewriteOperand(instr.op1);
            var src = frame.EnsureRegister(Registers.lr);
            emitter.Assign(dst, src);
        }

        private void RewriteMtcrf()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.SideEffect(PseudoProc("__mtcrf", VoidType.Instance, dst, src));
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

        private void RewriteMulhw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Sar(emitter.IMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }

        private void RewriteMulhwu()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Sar(emitter.UMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }

        private void RewriteMullw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.IMul(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteNeg()
        {
            var opE = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Neg(opE));
            MaybeEmitCr0(opD);
        }


        private void RewriteOr(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = (opL == opR)
                ? opL
                :  emitter.Or(opL, opR);
            if (negate)
                s = emitter.Comp(s);
            emitter.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteOris()
        {
            emitter.Assign(
                RewriteOperand(instr.op1),
                emitter.Or(
                    RewriteOperand(instr.op2),
                    Shift16(dasm.Current.op3)));
        }

        void RewriteRlwimi()
        {
            var src = RewriteOperand(instr.op2);
            var dst = RewriteOperand(instr.op1);
            emitter.Assign(
                dst,
                PseudoProc(
                    "__rlwimi",
                    PrimitiveType.Word32,
                    src,
                    RewriteOperand(instr.op3),
                    RewriteOperand(instr.op4),
                    RewriteOperand(instr.op5))
                );
        }

        void RewriteRlwinm()
        {
            var rd = RewriteOperand(instr.op1);
            var rs = RewriteOperand(instr.op2);
            byte sh = ((Constant)RewriteOperand(instr.op3)).ToByte();
            byte mb = ((Constant)RewriteOperand(instr.op4)).ToByte();
            byte me = ((Constant)RewriteOperand(instr.op5)).ToByte();
            uint maskBegin = (uint)(1ul << (32 - mb));
            uint maskEnd = 1u << (31 - me);
            uint mask = maskBegin - maskEnd;
            if (sh == 0)
            {
                emitter.Assign(rd, emitter.And(rs, Constant.UInt32(mask)));
            }
            else if (sh == 1 && mb == 31 && me == 31)
            {
                emitter.Assign(rd, emitter.Shr(rs, 31));
            }
            else if (sh == 2 && mb == 0x00)
            {
                emitter.Assign(rd, emitter.Shl(rs, sh));
            }
            else if (sh == 3)
            {
                emitter.Assign(rd, emitter.And(
                    emitter.Shl(rs, sh),
                    Constant.UInt32(mask)));
            }
            else if (sh == 0x10 && mb == 0x10 && me == 0x1F)
            {
                emitter.Assign(rd, emitter.Shr(rs, 0x10));
            }
            else
                throw new AddressCorrelatedException(dasm.Current.Address, "{0} not handled yet.", dasm.Current);
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

        public void RewriteSrw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Shr(opL, opR));
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

        public void RewriteSubfc()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.ISub(opR, opL));
            MaybeEmitCr0(opD);
            var xer = frame.EnsureRegister(Registers.xer);
            emitter.Assign(xer, emitter.Cond(opD));
        }

        public void RewriteSubfe()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(Registers.xer);
            emitter.Assign(opD, emitter.IAdd(emitter.ISub(opR, opL), xer));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfic()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfze()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(Registers.xer);
            emitter.Assign(
                opD, 
                emitter.IAdd(
                    emitter.ISub(
                        Constant.Zero(opD.DataType),
                        opS), 
                    xer));
            MaybeEmitCr0(opD);
        }


        private void RewriteXor()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = (opL == opR)
                ? opL
                : emitter.Xor(opL, opR);
            emitter.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        public void RewriteXoris()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = Shift16(dasm.Current.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Xor(opL, opR));
        }
    }
}
