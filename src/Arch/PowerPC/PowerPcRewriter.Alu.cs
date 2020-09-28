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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
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
            var cr0 = binder.EnsureFlagGroup(arch.cr, 0xF, "cr0", PrimitiveType.Byte);
            m.Assign(cr0, m.Cond(e));
        }


        private void RewriteAdd()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            RewriteAdd(opD, opL, opR);
        }

        public void RewriteAddc()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            RewriteAdd(opD, opL, opR);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteAdde()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(opD,
                m.IAdd(
                    m.IAdd(opL, opR),
                    xer));
            MaybeEmitCr0(opD);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteAddme()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opS = RewriteOperand(instr.Operands[1]);
            var cr0 = binder.EnsureFlagGroup(arch.cr, 0x1, "cr0", PrimitiveType.Byte);
            m.Assign(opD,
                m.ISub(
                    m.IAdd(opS, cr0),
                    -1));
        }

        public void RewriteAddi()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            RewriteAdd(opD, opL, opR);
        }

        public void RewriteAddic()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            RewriteAdd(opD, opL, opR);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteAddis()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = Shift16(dasm.Current.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            RewriteAdd(opD, opL, opR);
        }

        private void RewriteAddze()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = binder.EnsureRegister(arch.xer);
            var opD = RewriteOperand(instr.Operands[0]);
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
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
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
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            var s = m.And(opL, m.Comp(opR));
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        private void RewriteAndis()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            
            m.Assign(
                opD,
                m.And(
                    RewriteOperand(instr.Operands[1]),
                    Shift16(dasm.Current.Operands[2])));
            MaybeEmitCr0(opD);
        }

        private void RewriteBcdadd()
        {
            var d = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(
                d,
                host.PseudoProcedure("__bcdadd", d.DataType, a, b));
        }

        private void RewriteCmp()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpi()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpl()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpli()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmplw()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmplwi()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCmpwi()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.Assign(cr, m.Cond(
                m.ISub(r, i)));
        }

        private void RewriteCntlz(string name, DataType dt)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            if (dt.Size < arch.WordWidth.Size)
            {
                src = m.Cast(dt, src);
            }
            m.Assign(dst, host.PseudoProcedure(name, PrimitiveType.UInt32, src));
        }

        private void RewriteCreqv()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.SideEffect(host.PseudoProcedure("__creqv", VoidType.Instance, cr, r, i));
        }

        private void RewriteCrnor()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.SideEffect(host.PseudoProcedure("__crnor", VoidType.Instance, cr, r, i));
        }

        private void RewriteCror()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.SideEffect(host.PseudoProcedure("__cror", VoidType.Instance, cr, r, i));
        }

        private void RewriteCrxor()
        {
            var cr = RewriteOperand(instr.Operands[0]);
            var r = RewriteOperand(instr.Operands[1]);
            var i = RewriteOperand(instr.Operands[2]);
            m.SideEffect(host.PseudoProcedure("__crxor", VoidType.Instance, cr, r, i));
        }

        private void RewriteDivd(Func<Expression,Expression,Expression> div)
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, div(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteDivw()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.SDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteDivwu()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.UDiv(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteExts(PrimitiveType size)
        {
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            var tmp = binder.CreateTemporary(size);
            m.Assign(tmp, m.Cast(tmp.DataType, opS));
            m.Assign(
                opD, 
                m.Cast(
                    PrimitiveType.Create(Domain.SignedInt, opD.DataType.BitSize),
                    tmp));
            MaybeEmitCr0(opD);
        }

        private void RewriteMcrf()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteMfcr()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = binder.EnsureRegister(arch.cr);
            m.Assign(dst, src);
        }

        private void RewriteMfctr()
        {
            var src = binder.EnsureRegister(arch.ctr);
            var dst = RewriteOperand(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteMftb()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = host.PseudoProcedure("__mftb", dst.DataType);
            m.Assign(dst, src);
        }

        private void RewriteMflr()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = binder.EnsureRegister(arch.lr);
            m.Assign(dst, src);
        }

        private void RewriteMtcrf()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            m.SideEffect(host.PseudoProcedure("__mtcrf", VoidType.Instance, dst, src));
        }

        private void RewriteMtctr()
        {
            var src = RewriteOperand(instr.Operands[0]);
            var dst = binder.EnsureRegister(arch.ctr);
            m.Assign(dst, src);
        }

        private void RewriteMtlr()
        {
            var src= RewriteOperand(instr.Operands[0]);
            var dst = binder.EnsureRegister(arch.lr);
            m.Assign(dst, src);
        }

        private void RewriteMulhhwu()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.UMul(m.Shr(opL, 0x10), m.Shr(opR, 0x10)));
            MaybeEmitCr0(opD);
        }

        private void RewriteMulhw()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Sar(m.IMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }

        private void RewriteMulhwu()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Sar(m.UMul(opL, opR), 0x20));
            MaybeEmitCr0(opD);
        }



        private void RewriteMull()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.IMul(opL, opR));
            MaybeEmitCr0(opD);
        }

        private void RewriteNeg()
        {
            var opE = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Neg(opE));
            MaybeEmitCr0(opD);
        }


        private void RewriteOr(bool negate)
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
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
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, 
                m.Or(
                    opL,
                    m.Comp(opR)));
            MaybeEmitCr0(opD);
        }

        private void RewriteOris()
        {
            m.Assign(
                RewriteOperand(instr.Operands[0]),
                m.Or(
                    RewriteOperand(instr.Operands[1]),
                    Shift16(dasm.Current.Operands[2])));
        }

        void RewriteRlwimi()
        {
            var src = RewriteOperand(instr.Operands[1]);
            var dst = RewriteOperand(instr.Operands[0]);
            m.Assign(
                dst,
                host.PseudoProcedure(
                    "__rlwimi",
                    PrimitiveType.Word32,
                    src,
                    RewriteOperand(instr.Operands[2]),
                    RewriteOperand(instr.Operands[3]),
                    RewriteOperand(instr.Operands[4]))
                );
        }

        void RewriteRldicl()
        {
            var rd = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            byte sh = ((Constant)RewriteOperand(instr.Operands[2])).ToByte();
            byte mb = ((Constant)RewriteOperand(instr.Operands[3])).ToByte();
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
            {
                host.Error(
                    instr.Address,
                    string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                EmitUnitTest();
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            MaybeEmitCr0(rd);
        }

        private void RewriteRldicr()
        {
            var rd = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            byte sh = ((Constant)RewriteOperand(instr.Operands[2])).ToByte();
            byte me = ((Constant)RewriteOperand(instr.Operands[3])).ToByte();
            ulong maskEnd = 0ul - (ulong)(1ul << (63 - me));

            // Extract double word and right justify immediate | extrdi RA, RS, n, b   | rldicl RA, RS, b + n, 64 - n   | n > 0
            // Rotate double word left immediate               | rotldi RA, RS, n      | rldicl RA, RS, n, 0            | None
            // Rotate double word right immediate              | rotrdi RA, RS, n      | rldicl RA, RS, 64 - n, 0       | None
            // Rotate double word right immediate              | srdi RA, RS, n	       | rldicl RA, RS, 64 - n, n       | n < 64
            // Clear left double word immediate                | clrldi RA, RS, n      | rldicl RA, RS, 0, n	        | n < 64
            // Extract double word and left justify immediate  | extldi RA, RS, n, b   | rldicr RA, RS, b, n - 1        | None
            // Shift left double word immediate                | sldi RA, RS, n        | rldicr RA, RS, n, 63 - n	    | None
            // Clear right double word immediate               | clrrdi RA, RS, n      | rldicr RA, RS, 0, 63 - n	    | None
            // Clear left double word and shift left immediate | clrlsldi RA, RS, b, n | rldic RA, RS, n, b - n         | None
            // Insert double word from right immediate         | insrdi RA, RS, n, b   | rldimi RA, RS, 64 - (b + n), b | None
            // Rotate double word left                         | rotld RA, RS, RB      | rldcl RA, RS, RB, 0	        | None
            if (sh + me == 63)
            {
                // sldi
                m.Assign(rd, m.Shl(rs, sh));
            }
            else if (me == 63)
            {
                // rotldi
                m.Assign(rd, host.PseudoProcedure(
                    PseudoProcedure.Rol,
                    PrimitiveType.Word64,
                    rs, Constant.Byte(sh)));
            }
            else
            {
                host.Error(
                    instr.Address,
                    string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                EmitUnitTest();
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            MaybeEmitCr0(rd);
        }

        private void RewriteRldimi()
        {
            var rd = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            byte sh = ((Constant)RewriteOperand(instr.Operands[2])).ToByte();
            byte me = ((Constant)RewriteOperand(instr.Operands[3])).ToByte();

            MaybeEmitCr0(rd);
            if (sh == 0x20 && me == 0x00)
            {
                m.Assign(rd,
                    m.Dpb(rd, m.Cast(PrimitiveType.Word32, rs), 0x20));
            }
            else
            {
                host.Error(
                    instr.Address,
                    string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                EmitUnitTest();
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            MaybeEmitCr0(rd);

        }

        void RewriteRlwinm()
        {
            var rd = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            byte sh = ((Constant)RewriteOperand(instr.Operands[2])).ToByte();
            byte mb = ((Constant)RewriteOperand(instr.Operands[3])).ToByte();
            byte me = ((Constant)RewriteOperand(instr.Operands[4])).ToByte();
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
            }
            else if (mb == 32 - sh && me == 31)
            {
                m.Assign(rd, m.Shr(rs, (byte)(32 - sh)));
            }
            else if (mb == 0 && me == 31 - sh)
            {
                m.Assign(rd, m.Shl(rs, sh));
            }
            else if (mb == 0 && me == 31)
            {
                if (sh < 16)
                    m.Assign(rd, host.PseudoProcedure(PseudoProcedure.Rol, PrimitiveType.Word32, rs, Constant.Byte(sh)));
                else
                    m.Assign(rd, host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, rs, Constant.Byte((byte)(32 - sh))));
            }
            else if (me == 31)
            {
                int n = 32 - mb;
                int b = sh - n;
                mask = (1u << b) - 1;
                m.Assign(rd, m.And(
                    m.Shr(rs, Constant.Byte((byte)n)),
                    Constant.Word32(mask)));
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
                    MaybeEmitCr0(rd);
                    return;
                }
                else if (mb >= 32 - sh)
                {
                    // [                        ll]
                    // [                        m.]

                    m.Assign(rd, m.And(
                        m.Shr(rs, Constant.Byte((byte)(32 - sh))),
                        Constant.Word32(mask)));
                    MaybeEmitCr0(rd);
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
            MaybeEmitCr0(rd);
        }

        public void RewriteRlwnm()
        {
            var rd = RewriteOperand(instr.Operands[0]);
            var rs = RewriteOperand(instr.Operands[1]);
            var sh = RewriteOperand(instr.Operands[2]);
            byte mb = ((Constant)RewriteOperand(instr.Operands[3])).ToByte();
            byte me = ((Constant)RewriteOperand(instr.Operands[4])).ToByte();
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
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Shl(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSra()
        {
            //$TODO: identical to Sraw? If so, merge instructions
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Sar(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSrw()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Shr(opL, opR));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubf()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfc()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(xer, m.Cond(opD));
        }

        public void RewriteSubfe()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(opD, m.IAdd(m.ISub(opR, opL), xer));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfic()
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.ISub(opR, opL));
            MaybeEmitCr0(opD);
        }

        public void RewriteSubfze()
        {
            var opS = RewriteOperand(instr.Operands[1]);
            var opD = RewriteOperand(instr.Operands[0]);
            var xer = binder.EnsureRegister(arch.xer);
            m.Assign(
                opD, 
                m.IAdd(
                    m.ISub(
                        Constant.Zero(opD.DataType),
                        opS), 
                    xer));
            MaybeEmitCr0(opD);
        }


        private void RewriteXor(bool negate)
        {
            var opL = RewriteOperand(instr.Operands[1]);
            var opR = RewriteOperand(instr.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            var s = (opL == opR)
                ? Constant.Zero(opL.DataType)
                : (Expression)m.Xor(opL, opR);
            if (negate)
            {
                s = m.Comp(s);
            }
            m.Assign(opD, s);
            MaybeEmitCr0(opD);
        }

        public void RewriteXoris()
        {
            var opL = RewriteOperand(instr.Operands[1], true);
            var opR = Shift16(dasm.Current.Operands[2]);
            var opD = RewriteOperand(instr.Operands[0]);
            m.Assign(opD, m.Xor(opL, opR));
        }
    }
}
