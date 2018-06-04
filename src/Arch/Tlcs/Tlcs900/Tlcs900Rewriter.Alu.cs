#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Registers = Reko.Arch.Tlcs.Tlcs900.Tlcs900Registers;

namespace Reko.Arch.Tlcs.Tlcs900
{
    public partial class Tlcs900Rewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn, string flags)
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, (d, s) => fn(fn(d, s), c));
            EmitCc(dst, flags);
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteBit()
        {
            var Z = binder.EnsureFlagGroup(Tlcs900Registers.Z);
            var bit = RewriteSrc(this.instr.op1);
            var dst = RewriteSrc(this.instr.op2);
            m.Assign(Z, m.Eq0(m.And(dst, m.Shl(m.Int8(1), bit))));
            m.Assign(binder.EnsureFlagGroup(Registers.H), Constant.True());
            m.Assign(binder.EnsureFlagGroup(Registers.N), Constant.False());
        }

        private void RewriteBs1b()
        {
            var a = binder.EnsureRegister(Registers.a);
            var src = RewriteSrc(instr.op2);
            var v = binder.EnsureFlagGroup(Registers.V);
            m.Assign(a, host.PseudoProcedure("__bs1b", PrimitiveType.SByte, src));
            m.Assign(v, m.Eq0(src));
        }

        private void RewriteCcf()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Not(c));
        }

        private void RewriteChg()
        {
            var bit = ((ImmediateOperand)instr.op1).Value.ToInt32();
            var src = RewriteSrc(instr.op2);
            var dst = RewriteDst(instr.op2, src, (a, b) => m.Xor(b, 1 << bit));
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
            var div = binder.EnsureRegister(arch.GetSubregister(Registers.regs[(int)reg.Domain], 0, (int)reg.BitSize * 2));
            var tmp = binder.CreateTemporary(reg.DataType);
            var quo = binder.EnsureRegister(arch.GetSubregister(reg, 0, 8));
            var rem = binder.EnsureRegister(arch.GetSubregister(reg, 8, 8));
            m.Assign(tmp, div);
            m.Assign(quo, fn(tmp, op2));
            m.Assign(rem, m.Remainder(tmp, op2));
            EmitCc(quo, flags);
        }

        private void RewriteEx()
        {
            var op1 = RewriteSrc(instr.op1);
            var op2 = RewriteSrc(instr.op2);
            var tmp = binder.CreateTemporary(op1.DataType);
            m.Assign(tmp, op1);
            m.Assign(op1, op2);
            m.Assign(op2, tmp);
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
            var tmp = binder.CreateTemporary(dt);
            var src = binder.EnsureRegister(Registers.xhl);
            var dst = binder.EnsureRegister(Registers.xde);
            var cnt = binder.EnsureRegister(Registers.bc);
            m.Assign(tmp, m.Mem(dt, src));
            m.Assign(m.Mem(dt, dst), tmp);
            m.Assign(src, m.IAdd(src, m.Int32(dt.Size)));
            m.Assign(dst, m.IAdd(dst, m.Int32(dt.Size)));
            m.Assign(cnt, m.ISub(cnt, m.Int16(1)));
            m.Branch(m.Ne0(cnt), instr.Address, RtlClass.ConditionalTransfer);
            EmitCc(null, flags);
        }

        private void RewriteMul(Func<Expression,Expression,Expression> fn)
        {
            var op1 = ((RegisterOperand)instr.op1).Register;
            var op2 = RewriteSrc(instr.op2);
            var dst = binder.EnsureRegister(Registers.regs[op1.Number]);
            m.Assign(dst, fn(binder.EnsureRegister(op1), op2));
        }
        private void RewritePop()
        {
            var xsp = binder.EnsureRegister(Tlcs900Registers.xsp);
            var op = m.Mem(instr.op1.Width, xsp);
            RewriteDst(instr.op1, op, (a, b) => b);
            m.Assign(xsp, m.IAdd(xsp, m.Int32(instr.op1.Width.Size)));
        }

        private void RewritePush()
        {
            var op = RewriteSrc(instr.op1);
            var xsp = binder.EnsureRegister(Tlcs900Registers.xsp);
            m.Assign(xsp, m.ISub(xsp, m.Int32(op.DataType.Size)));
            m.Assign(m.Mem(op.DataType, xsp), op);
        }

        private void RewriteRcf()
        {
            m.Assign(binder.EnsureFlagGroup(Tlcs900Registers.C), Constant.False());
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
                            PrimitiveType.Create(Domain.SignedInt, op1.DataType.BitSize),
                            1), b))));
        }

        private void RewriteScc()
        {
            var test = GenerateTestExpression((ConditionOperand)instr.op1, false);
            m.Assign(RewriteSrc(instr.op2), test);
        }

        private void RewriteScf()
        {
            m.Assign(binder.EnsureFlagGroup(Tlcs900Registers.C), Constant.True());
        }

        private void RewriteSet()
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteDst(this.instr.op2,
                op1,
                (a, b) => m.Or(
                    a,
                    m.Shl(m.Const(
                        PrimitiveType.Create(Domain.SignedInt, op1.DataType.BitSize),
                        1), b)));
        }

        private void RewriteShift(Func<Expression,Expression, Expression> shift, string flags)
        {
            Expression value;
            if (instr.op2 == null)
            {
                var amt = Constant.SByte(1);
                value = RewriteDst(this.instr.op1, amt, shift);

            }
            else
            {
            var op1 = RewriteSrc(this.instr.op1);
                value = RewriteDst(this.instr.op2, op1, shift);
        }
            EmitCc(value, flags);
    }

        private void RewriteZcf()
        {
            var z = binder.EnsureFlagGroup(Registers.Z);
            var c = binder.EnsureFlagGroup(Registers.C);
            var n = binder.EnsureFlagGroup(Registers.N);
            m.Assign(c, m.Not(z));
            m.Assign(n, Constant.False());
        }
    }
}
