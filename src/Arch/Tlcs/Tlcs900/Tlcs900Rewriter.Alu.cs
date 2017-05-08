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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public partial class Tlcs900Rewriter
    {
        private void RewriteBinOp(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteBit()
        {
            var Z = frame.EnsureFlagGroup(Tlcs900Registers.Z);
            var bit = RewriteSrc(this.instr.op1);
            var dst = RewriteSrc(this.instr.op2);
            m.Assign(Z, m.Eq0(m.And(dst, m.Shl(m.Int8(1), bit))));
            m.Assign(frame.EnsureFlagGroup(Tlcs900Registers.H), Constant.True());
            m.Assign(frame.EnsureFlagGroup(Tlcs900Registers.N), Constant.False());
        }

        private void RewriteCp(string flags)
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteSrc(this.instr.op2);
            EmitCc(m.ISub(op1, op2), flags);
        }

        private void RewriteDaa(string flags)
        {
            var src = RewriteSrc(this.instr.op1);
            var fn = host.PseudoProcedure("__daa", PrimitiveType.Byte, src);
            m.Assign(src, fn);
            EmitCc(src, flags);
        }

        private void RewriteDiv(Func<Expression, Expression, Expression> fn, string flags)
        {
            var reg = ((RegisterOperand)this.instr.op1).Register;
            var op2 = RewriteSrc(this.instr.op2);
            var tmp = frame.CreateTemporary(reg.DataType);
            var quo = frame.EnsureRegister(arch.GetSubregister(reg, 0, 8));
            var rem = frame.EnsureRegister(arch.GetSubregister(reg, 8, 8));
            m.Assign(tmp, frame.EnsureRegister(reg));
            m.Assign(quo, fn(tmp, op2));
            m.Assign(rem, m.Remainder(tmp, op2));
            EmitCc(quo, flags);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op1);
            var dst = RewriteDst(this.instr.op2, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteLd()
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, (d, s) => s);
        }

        private void RewriteLda()
        {
            var src = RewriteSrcEa((MemoryOperand)this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, (d, s) => s);
        }

        private void RewriteLdir(PrimitiveType dt, string flags)
        {
            if (instr.op1 != null || instr.op2 != null)
            {
                EmitUnitTest("Tlcs900_rw_", "00010000");
                Invalid();
                return;
            }
            var tmp = frame.CreateTemporary(dt);
            var src = frame.EnsureRegister(Tlcs900Registers.xhl);
            var dst = frame.EnsureRegister(Tlcs900Registers.xde);
            var cnt = frame.EnsureRegister(Tlcs900Registers.bc);
            m.Assign(tmp, m.Load(dt, src));
            m.Assign(m.Load(dt, dst), tmp);
            m.Assign(src, m.IAdd(src, m.Int32(dt.Size)));
            m.Assign(dst, m.IAdd(dst, m.Int32(dt.Size)));
            m.Assign(cnt, m.ISub(cnt, m.Int16(1)));
            m.Branch(m.Ne0(cnt), instr.Address, RtlClass.ConditionalTransfer);
            EmitCc(null, flags);
        }

        private void RewritePop()
        {
            var xsp = frame.EnsureRegister(Tlcs900Registers.xsp);
            var op = m.Load(instr.op1.Width, xsp);
            RewriteDst(instr.op1, op, (a, b) => b);
            m.Assign(xsp, m.IAdd(xsp, m.Int32(instr.op1.Width.Size)));
        }

        private void RewritePush()
        {
            var op = RewriteSrc(instr.op1);
            var xsp = frame.EnsureRegister(Tlcs900Registers.xsp);
            m.Assign(xsp, m.ISub(xsp, m.Int32(op.DataType.Size)));
            m.Assign(m.Load(op.DataType, xsp), op);
        }

        private void RewriteRcf()
        {
            m.Assign(frame.EnsureFlagGroup(Tlcs900Registers.C), Constant.False());
        }

        private void RewriteRes()
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteDst(this.instr.op2,
                op1,
                (a, b) => m.And(
                    a,
                    m.Comp(
                        m.Shl(m.Const(
                            PrimitiveType.Create(Domain.SignedInt, op1.DataType.Size),
                            1), b))));
        }

        private void RewriteScf()
        {
            m.Assign(frame.EnsureFlagGroup(Tlcs900Registers.C), Constant.True());
        }

        private void RewriteSet()
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteDst(this.instr.op2,
                op1,
                (a, b) => m.Or(
                    a,
                    m.Shl(m.Const(
                        PrimitiveType.Create(Domain.SignedInt, op1.DataType.Size),
                        1), b)));
        }

        private void RewriteShift(Func<Expression,Expression, Expression> shift, string flags)
        {
            if (instr.op2 == null)
            {
                EmitUnitTest();
                Invalid();
                return;
            }
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteDst(this.instr.op2, op1, shift);
            EmitCc(op2, flags);
        }
    }
}
