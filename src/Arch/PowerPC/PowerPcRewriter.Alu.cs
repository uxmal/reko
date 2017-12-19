#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void MaybeEmitCr0(Expression e)
        {
            if (!instr.setsCR0)
                return;
            var cr0 = frame.EnsureFlagGroup(arch.cr, 0x1, "cr0", PrimitiveType.Byte);
            m.Assign(cr0, m.Cond(e));
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
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteAdde()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(arch.xer);
            m.Assign(opD,
                m.IAdd(
                    m.IAdd(opL, opR),
                    xer));
            MaybeEmitCr0(opD);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteAddme()
        {
            var opD = RewriteOperand(instr.op1);
            var opS = RewriteOperand(instr.op2);
            var cr0 = frame.EnsureFlagGroup(arch.cr, 0x1, "cr0", PrimitiveType.Byte);
            m.Assign(opD,
                m.ISub(
                    m.IAdd(opS, cr0),
                    -1));
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
            m.Assign(xer, m.Cond(opD));
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
            m.Assign(opR, m.Cond(opD));
        }

        private void RewriteAdd(Expression opD, Expression opL, Expression opR)
        {
            if (opL.IsZero)
                m.Assign(opD, opR);
            else if (opR.IsZero)
                m.Assign(opD, opL);
            else 
                m.Assign(opD, m.IAdd(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteAnd(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = (opL == opR)
                ? opL
                : m.And(opL, opR);
            if (negate)
                s = m.Comp(s);
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteAndc()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = m.And(opL, m.Comp(opR));
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteAndis()
        {
            var opD = RewriteOperand(instr.op1);
            
            m.Assign(
                opD,
                m.And(
                    RewriteOperand(instr.op2),
                    Shift16(dasm.Current.op3)));
            MaybeEmitCr0(opD);
        }

        private void RewriteCmp()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpi()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpl()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpli()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmplw()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmplwi()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpwi()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCntlz(string name, DataType dt)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            if (dt.Size < arch.WordWidth.Size)
            {
                src = m.Cast(dt, src);
            }
            m.Assign(dst, host.PseudoProcedure(name, PrimitiveType.UInt32, src));
        }

        private void RewriteCreqv()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.SideEffect(host.PseudoProcedure("__creqv", VoidType.Instance, cr, r, i));
        }

        private void RewriteCrnor()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.SideEffect(host.PseudoProcedure("__crnor", VoidType.Instance, cr, r, i));
        }

        private void RewriteCror()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.SideEffect(host.PseudoProcedure("__cror", VoidType.Instance, cr, r, i));
        }

        private void RewriteCrxor()
        {
            var cr = RewriteOperand(instr.op1);
            var r = RewriteOperand(instr.op2);
            var i = RewriteOperand(instr.op3);
            m.SideEffect(host.PseudoProcedure("__crxor", VoidType.Instance, cr, r, i));
        }

        private void RewriteDivw()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.SDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteDivwu()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.UDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteExts(PrimitiveType size)
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            var tmp = frame.CreateTemporary(size);
            m.Assign(tmp, m.Cast(tmp.DataType, opS));
            m.Assign(
                opD, 
                m.Cast(
                    PrimitiveType.Create(Domain.SignedInt, opD.DataType.Size),
                    tmp));
            MaybeEmitCr0(opD);
        }

        private void RewriteMcrf()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, src);
        }

        private void RewriteMfcr()
        {
            var dst = RewriteOperand(instr.op1);
            var src = frame.EnsureRegister(arch.cr);
            m.Assign(dst, src);
        }

        private void RewriteMfctr()
        {
            var src = frame.EnsureRegister(arch.ctr);
            var dst = RewriteOperand(instr.op1);
            m.Assign(dst, src);
        }

        private void RewriteMftb()
        {
            var dst = RewriteOperand(instr.op1);
            var src = host.PseudoProcedure("__mftb", dst.DataType);
            m.Assign(dst, src);
        }

        private void RewriteMflr()
        {
            var dst = RewriteOperand(instr.op1);
            var src = frame.EnsureRegister(arch.lr);
            m.Assign(dst, src);
        }

        private void RewriteMtcrf()
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.SideEffect(host.PseudoProcedure("__mtcrf", VoidType.Instance, dst, src));
        }

        private void RewriteMtctr()
        {
            var src = RewriteOperand(instr.op1);
            var dst = frame.EnsureRegister(arch.ctr);
            m.Assign(dst, src);
        }

        private void RewriteMtlr()
        {
            var src= RewriteOperand(instr.op1);
            var dst = frame.EnsureRegister(arch.lr);
            m.Assign(dst, src);
        }

        private void RewriteMulhw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Sar(m.IMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }

        private void RewriteMulhwu()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Sar(m.UMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }

        private void RewriteMull()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.IMul(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteNeg()
        {
            var opE = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Neg(opE));
            MaybeEmitCr0(opD);
        }


        private void RewriteOr(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var s = (opL == opR)
                ? opL
                :  m.Or(opL, opR);
            if (negate)
                s = m.Comp(s);
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteOrc(bool negate)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, 
                m.Or(
                    opL,
                    m.Comp(opR)));
            MaybeEmitCr0(opD);
        }

        private void RewriteOris()
        {
            m.Assign(
                RewriteOperand(instr.op1),
                m.Or(
                    RewriteOperand(instr.op2),
                    Shift16(dasm.Current.op3)));
        }

        void RewriteRlwimi()
        {
            var src = RewriteOperand(instr.op2);
            var dst = RewriteOperand(instr.op1);
            m.Assign(
                dst,
                host.PseudoProcedure(
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
                m.Assign(rd, m.And(rs, Constant.Word64(maskBegin)));
            }
            else if (sh + mb == 64)
            {
                m.Assign(rd, m.Shr(rs, (byte)mb));
            }
            else if (sh == 0x3F && mb == 0x3F)
            {
                m.Assign(rd, m.Shr(rs, (byte)mb));
            }
            else if (sh < mb)
            {
                m.Assign(rd, m.And(
                    m.Shl(rs, (byte)sh),
                    maskBegin));
            }
            else if (sh == 0x39 && mb == 0x38)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x31 && mb == 0x3F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x10 && mb == 0x2F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x08 && mb == 0x37)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x18 && mb == 0x27)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x20 && mb == 0x1F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x02 && mb == 0x1E)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x38 && mb == 0x3F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x37 && mb == 0x3F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x21 && mb == 0x3F)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x08 && mb == 0x30)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x3D && mb == 0x23)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (sh == 0x3E && mb == 0x22)
            {
                m.Assign(rd, m.And(
                    m.Shr(rs, (byte)(64 - sh)),
                    maskBegin));
            }
            else if (mb == 0x00)
            {
                m.Assign(rd, host.PseudoProcedure(PseudoProcedure.Rol, rd.DataType, rs, Constant.Byte((byte)sh)));
            }
            else
                throw new NotImplementedException();
            MaybeEmitCr0(rd);
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
                m.Assign(rd, m.And(rs, Constant.UInt32(mask)));
                return;
            }
            else if (mb == 32 - sh && me == 31)
            {
                m.Assign(rd, m.Shr(rs, (byte)(32 - sh)));
                return;
            }
            else if (mb == 0 && me == 31 - sh)
            {
                m.Assign(rd, m.Shl(rs, sh));
                return;
            }
            else if (mb == 0 && me == 31)
            {
                if (sh < 16)
                    m.Assign(rd, host.PseudoProcedure(PseudoProcedure.Rol, PrimitiveType.Word32, rs, Constant.Byte(sh)));
                else
                    m.Assign(rd, host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, rs, Constant.Byte((byte)(32 - sh))));
                return;
            }
            else if (me == 31)
            {
                int n = 32 - mb;
                int b = sh - n;
                mask = (1u << b) - 1;
                m.Assign(rd, m.And(
                    m.Shr(rs, Constant.Byte((byte)n)),
                    Constant.Word32(mask)));
                return;
            }
            else if (mb <= me)
            {
                if (me < 32 - sh)
                {
                    // [                           llll]
                    // [                        mmm....]
                    m.Assign(rd, m.And(
                        m.Shl(rs, Constant.Byte((byte)sh)),
                        Constant.Word32(mask)));
                    return;
                }
                else if (mb >= 32 - sh)
                {
                    // [                        ll]
                    // [                        m.]

                    m.Assign(rd, m.And(
                        m.Shr(rs, Constant.Byte((byte)(32 - sh))),
                        Constant.Word32(mask)));
                    return;
                }
            }
            else
            {
                //$TODO: yeah, this one is hard...
                m.Assign(rd,
                    host.PseudoProcedure(
                        "__rlwinm",
                        PrimitiveType.Word32,
                        rs,
                        Constant.Byte(sh),
                        Constant.Byte(mb),
                        Constant.Byte(me)));
            }

            //Error,10034E20,rlwinm	r9,r31,1D,1B,1D not handled yet.
            //Error,10028B50,rlwinm	r8,r8,04,18,1B not handled yet.
            //Error,1002641C,rlwinm	r4,r4,04,18,1B not handled yet.
            //Error,10026364,rlwinm	r4,r4,04,18,1B not handled yet.
            //Error,1003078C,rlwinm	r8,r8,04,1A,1B not handled yet.
            //Error,100294D4,rlwinm	r0,r0,04,18,1B not handled yet.
            //Error,100338A0,rlwinm	r4,r11,08,08,0F not handled yet.
            //rlwinm	r12,r2,09,1D,09 
        }

        public void RewriteRlwnm()
        {
            var rd = RewriteOperand(instr.op1);
            var rs = RewriteOperand(instr.op2);
            var sh = RewriteOperand(instr.op3);
            byte mb = ((Constant)RewriteOperand(instr.op4)).ToByte();
            byte me = ((Constant)RewriteOperand(instr.op5)).ToByte();
            var rol = host.PseudoProcedure(PseudoProcedure.Rol, rd.DataType, rs, sh );
            if (mb == 0 && me == 31)
            {
                m.Assign(rd, rol);
                return;
            }
            var mask = (1u << (32-mb)) - (1u << (31-me));
            m.Assign(rd, m.And(rol, Constant.UInt32(mask)));
        }

        public void RewriteSl(PrimitiveType dt)
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Shl(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSra()
        {
            //$TODO: identical to Sraw? If so, merge instructions
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Sar(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSrw()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Shr(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubf()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfc()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
            var xer = frame.EnsureRegister(arch.xer);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteSubfe()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(arch.xer);
            m.Assign(opD, m.IAdd(m.ISub(opR, opL), xer));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfic()
        {
            var opL = RewriteOperand(instr.op2);
            var opR = RewriteOperand(instr.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfze()
        {
            var opS = RewriteOperand(instr.op2);
            var opD = RewriteOperand(instr.op1);
            var xer = frame.EnsureRegister(arch.xer);
            m.Assign(
                opD, 
                m.IAdd(
                    m.ISub(
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
                : m.Xor(opL, opR);
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        public void RewriteXoris()
        {
            var opL = RewriteOperand(instr.op2, true);
            var opR = Shift16(dasm.Current.op3);
            var opD = RewriteOperand(instr.op1);
            m.Assign(opD, m.Xor(opL, opR));
        }
    }
}
