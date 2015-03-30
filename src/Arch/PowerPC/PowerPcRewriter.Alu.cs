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

        public void RewriteAddc()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
            var xer = frame.EnsureRegister(arch.xer);
            emitter.Assign(xer, emitter.Cond(opD));
        }

        public void RewriteAdde()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(arch.xer);
            emitter.Assign(opD,
                emitter.IAdd(
                    emitter.IAdd(opL, opR),
                    xer));
            MaybeEmitCr0(opD);
            emitter.Assign(xer, emitter.Cond(opD));
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
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            RewriteAdd(opD, opL, opR);
            var xer = frame.EnsureRegister(arch.xer);
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
            var opR = frame.EnsureRegister(arch.xer);
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

        private void RewriteAndc()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = emitter.And(opL, emitter.Comp(opR));
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

        private void RewriteCmpi()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.Assign(cr, emitter.Cond(
                emitter.ISub(r, i)));
        }

        private void RewriteCmpl()
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

        private void RewriteCmplwi()
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

        private void RewriteCror()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            emitter.SideEffect(PseudoProc("__cror", VoidType.Instance, cr, r, i));
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

        private void RewriteExts(PrimitiveType size)
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            var tmp = frame.CreateTemporary(size);
            emitter.Assign(tmp, emitter.Cast(tmp.DataType, opS));
            emitter.Assign(
                opD, 
                emitter.Cast(
                    PrimitiveType.Create(Domain.SignedInt, opD.DataType.Size),
                    tmp));
            MaybeEmitCr0(opD);
        }

        private void RewriteMcrf()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.Assign(dst, src);
        }

        private void RewriteMflr()
        {
            var dst = RewriteOperand(instr.op1);
            var src = frame.EnsureRegister(arch.lr);
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
            var dst = frame.EnsureRegister(arch.ctr);
            emitter.Assign(dst, src);
        }

        private void RewriteMtlr()
        {
            var src= RewriteOperand(instr.op1);
            var dst = frame.EnsureRegister(arch.lr);
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

        private void RewriteMull()
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

        private void RewriteOrc(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, 
                emitter.Or(
                    opL,
                    emitter.Comp(opR)));
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

        void RewriteRldicl()
        {
            var rd = RewriteOperand(instr.op1);
            var rs = RewriteOperand(instr.op2);
            byte sh = ((Constant)RewriteOperand(instr.op3)).ToByte();
            byte mb = ((Constant)RewriteOperand(instr.op4)).ToByte();
            ulong maskBegin = (ulong)(1ul << (64 - mb)) - 1;
            if (sh == 0)
            {
                emitter.Assign(rd, emitter.And(rs, Constant.Word64(maskBegin)));
            }
            else
                throw new NotImplementedException();
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

//Extract and left justify immediate 	extlwi RA, RS, n, b 	rlwinm RA, RS, b, 0, n-1             	32 > n > 0
//Extract and right justify immediate 	extrwi RA, RS, n, b 	rlwinm RA, RS, b+n, 32-n, 31 	        32 > n > 0 & b+n =< 32
//Insert from left immediate         	inslwi RA, RS, n, b 	rlwinm RA, RS, 32-b, b, (b+n)-1 	    b+n <=32 & 32>n > 0 & 32 > b >= 0
//Insert from right immediate       	insrwi RA, RS, n, b 	rlwinm RA, RS, 32-(b+n), b, (b+n)-1 	b+n <= 32 & 32>n > 0
//Rotate left immediate             	rotlwi RA, RS, n    	rlwinm RA, RS, n, 0, 31 	            32 > n >= 0
//Rotate right immediate            	rotrwi RA, RS, n    	rlwinm RA, RS, 32-n, 0, 31 	            32 > n >= 0
//Rotate left                       	rotlw RA, RS, b      	rlwinm RA, RS, RB, 0, 31             	None
//Shift left immediate              	slwi RA, RS, n       	rlwinm RA, RS, n, 0, 31-n 	            32 > n >= 0
//Shift right immediate             	srwi RA, RS, n      	rlwinm RA, RS, 32-n, n, 31 	            32 > n >= 0
//Clear left immediate              	clrlwi RA, RS, n     	rlwinm RA, RS, 0, n, 31 	            32 > n >= 0
//Clear right immediate             	clrrwi RA, RS, n     	rlwinm RA, RS, 0, 0, 31-n 	            32 > n >= 0
//Clear left and shift left immediate 	clrslwi RA, RS, b, n 	rlwinm RA, RS, b-n, 31-n 	            b-n >= 0 & 32 > n >= 0 & 32 > b>= 0
            if (sh == 0)
            {
                emitter.Assign(rd, emitter.And(rs, Constant.UInt32(mask)));
            }
            else if (mb == 32 - sh && me == 31)
            {
                emitter.Assign(rd, emitter.Shr(rs, (byte)(32-sh)));
            }
            else if (mb == 0 && me == 31-sh)
            {
                emitter.Assign(rd, emitter.Shl(rs, sh));
            }
            else if (mb == 0 && me == 31)
            {
                if (sh < 16)
                    emitter.Assign(rd, PseudoProc(PseudoProcedure.Rol, PrimitiveType.Word32, rs, Constant.Byte(sh)));
                else
                    emitter.Assign(rd, PseudoProc(PseudoProcedure.Ror, PrimitiveType.Word32, rs, Constant.Byte((byte)(32 - sh))));
            }
            else if (me == 31)
            {
                int n = 32 - mb;
                int b = sh - n;
                mask = (1u << b) - 1;
                emitter.Assign(rd, emitter.And(
                    emitter.Shr(rs, Constant.Byte((byte)n)),
                    Constant.Word32(mask)));
            }
            else if (mb < me && me < 32-sh)
            {
                mask = (1u << (32-mb)) - (1u << (31-me));
                emitter.Assign(rd, emitter.And(
                    emitter.Shl(rs, Constant.Byte((byte)sh)),
                    Constant.Word32(mask)));
            }
            else
                throw new AddressCorrelatedException(dasm.Current.Address, "{0} not handled yet.", dasm.Current);

//Error,10034E20,rlwinm	r9,r31,1D,1B,1D not handled yet.
//Error,10028B50,rlwinm	r8,r8,04,18,1B not handled yet.
//Error,1002641C,rlwinm	r4,r4,04,18,1B not handled yet.
//Error,10026364,rlwinm	r4,r4,04,18,1B not handled yet.
//Error,1003078C,rlwinm	r8,r8,04,1A,1B not handled yet.
//Error,100294D4,rlwinm	r0,r0,04,18,1B not handled yet.
//Error,100338A0,rlwinm	r4,r11,08,08,0F not handled yet.

        }

        public void RewriteRlwnm()
        {
            var rd = RewriteOperand(instr.op1);
            var rs = RewriteOperand(instr.op2);
            var sh = RewriteOperand(instr.op3);
            byte mb = ((Constant)RewriteOperand(instr.op4)).ToByte();
            byte me = ((Constant)RewriteOperand(instr.op5)).ToByte();
            var rol = PseudoProc(PseudoProcedure.Rol, rd.DataType, rs, sh );
            if (mb == 0 && me == 31)
            {
                emitter.Assign(rd, rol);
                return;
            }
            var mask = (1u << (32-mb)) - (1u << (31-me));
            emitter.Assign(rd, emitter.And(rol, Constant.UInt32(mask)));
        }

        public void RewriteSl(PrimitiveType dt)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            emitter.Assign(opD, emitter.Shl(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSra()
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
            var xer = frame.EnsureRegister(arch.xer);
            emitter.Assign(xer, emitter.Cond(opD));
        }

        public void RewriteSubfe()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(arch.xer);
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
            var xer = frame.EnsureRegister(arch.xer);
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
                ? Constant.Zero(opL.DataType)
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
