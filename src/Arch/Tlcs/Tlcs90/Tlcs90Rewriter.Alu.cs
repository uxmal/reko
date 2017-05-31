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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    partial class Tlcs90Rewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, (d, s) => fn(fn(d, s), binder.EnsureFlagGroup(Registers.C)));
            EmitCc(dst, flags);
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteCcf()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Not(c));
        }

        private void RewriteCp()
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteSrc(this.instr.op2);
            EmitCc(m.ISub(op1, op2), "**-**V1");
        }

        private void RewriteCpl(string flags)
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Comp(a));
            EmitCc(a, flags);
        }

        private void RewriteDaa()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, host.PseudoProcedure("__daa", PrimitiveType.Byte, a));
            EmitCc(a, "**-**P-*");
        }

        private void RewriteDiv()
        {
            var num = RewriteSrc(this.instr.op1);
            var den = RewriteSrc(this.instr.op2);
            var quo = binder.EnsureRegister(Registers.l);
            var rem = binder.EnsureRegister(Registers.h);
            var tmp = binder.CreateTemporary(num.DataType);
            var v = binder.EnsureFlagGroup(Registers.V);
            m.Assign(tmp, num);
            m.Assign(quo, m.SDiv(tmp, den));
            m.Assign(rem, m.Remainder(tmp, den));
            m.Assign(v, m.Cond(quo));
        }

        private void RewriteEx()
        {
            var op1 = RewriteSrc(this.instr.op1);
            var op2 = RewriteSrc(this.instr.op2);
            var tmp = binder.CreateTemporary(op1.DataType);
            m.Assign(tmp, op1);
            RewriteDst(instr.op1, op2, (d, s) => s);
            RewriteDst(instr.op2, tmp, (d, s) => s);
        }

        private void RewriteExx()
        {
            foreach (var r in new[] { "bc", "de", "hl" })
            {
                var t = binder.CreateTemporary(PrimitiveType.Word16);
                var reg = binder.EnsureRegister(arch.GetRegister(r));
                var reg_ = binder.EnsureRegister(arch.GetRegister(r + "'"));
                m.Assign(t, reg);
                m.Assign(reg, reg_);
                m.Assign(reg_, t);
            }
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn, bool ifXSet)
        {
            if (ifXSet)
            {
                var x = binder.EnsureFlagGroup(Registers.X);
                m.BranchInMiddleOfInstruction(m.Not(x), instr.Address + instr.Length, Core.Rtl.RtlClass.ConditionalTransfer);
            }
            if (this.instr.op1 is MemoryOperand)
            {
                this.instr.op1.Width = PrimitiveType.Byte;
            }
            var src = RewriteSrc(this.instr.op1);
            Expression one;
            string flg;
            if (src.DataType.Size == 1)
            {
                one = Constant.SByte(1);
                flg = ifXSet ? "**-**V1-" : "**-**V0-";
            }
            else
            {
                one = Constant.Int16(1);
                flg = "----*---";
            }
            var dst = RewriteDst(this.instr.op1, src, (a, b) => fn(b, one));
            EmitCc(dst, flg);
        }

        private void RewriteIncwDecw(Func<Expression, Expression, Expression> fn)
        {
            instr.op1.Width = PrimitiveType.Word16;
            var src = RewriteSrc(this.instr.op1);
            src.DataType = PrimitiveType.Word16;
            var one = Constant.SByte(1);
            var flg =  "**-**V0-";
            var dst = RewriteDst(this.instr.op1, src, (a, b) => fn(b, one));
            EmitCc(dst, flg);
        }

        private void RewriteLd()
        {
            var src = RewriteSrc(instr.op2);
            var dst = RewriteDst(instr.op1, src, (a, b) => b);
        }

        private void RewriteLdar()
        {
            m.Assign(
                binder.EnsureRegister(Registers.hl),
                RewriteSrc(instr.op2));
        }

        private void RewriteLdir(string flags)
        {
            var dt = PrimitiveType.Byte;
            var tmp = binder.CreateTemporary(dt);
            var src = binder.EnsureRegister(Registers.hl);
            var dst = binder.EnsureRegister(Registers.de);
            var cnt = binder.EnsureRegister(Registers.bc);
            m.Assign(tmp, m.Load(dt, src));
            m.Assign(m.Load(dt, dst), tmp);
            m.Assign(src, m.IAdd(src, m.Int32(dt.Size)));
            m.Assign(dst, m.IAdd(dst, m.Int32(dt.Size)));
            m.Assign(cnt, m.ISub(cnt, m.Int16(1)));
            m.Branch(m.Ne0(cnt), instr.Address, RtlClass.ConditionalTransfer);
            EmitCc(null, flags);
        }

        private void RewriteMul()
        {
            var src = RewriteSrc(instr.op2);
            var l = binder.EnsureRegister(Registers.l);
            src = m.IMul(l, src);
            var dst = RewriteDst(instr.op1, src, (d, s) => s);
        }

        private void RewriteNeg()
        {
            var dst = RewriteDst(instr.op1, null, (d, s) => m.Neg(d));
            EmitCc(dst, "**-**V1*");
        }

        private void RewritePop()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var src = RewriteSrc(instr.op1);
            m.Assign(src, m.LoadW(sp));
            m.Assign(sp, m.IAdd(sp, m.Int16((short)src.DataType.Size)));
        }

        private void RewritePush()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var src = RewriteSrc(instr.op1);
            m.Assign(sp, m.ISub(sp, m.Int16((short)src.DataType.Size)));
            m.Assign(m.LoadW(sp), src);
        }

        private void RewriteRcf()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, Constant.False());
        }

        private void RewriteRotation(string pseudoOp, bool useCarry)
        {
            Expression reg;
            if (instr.op1 != null)
            {
                reg = RewriteSrc(instr.op1);
            }
            else
            {
                reg = binder.EnsureRegister(Registers.a);
            }
            var c = binder.EnsureFlagGroup(Registers.C);
            Expression src;
            if (useCarry)
            {
                src = m.Fn(
                    new PseudoProcedure(pseudoOp, reg.DataType, 2),
                    reg, c);
            }
            else
            {
                src = m.Fn(
                    new PseudoProcedure(pseudoOp, reg.DataType, 1),
                    reg);
            }
            m.Assign(reg, src);
            EmitCc(reg, "**-0XP0*");
        }

        private void RewriteScf()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, Constant.True());
        }

        private void RewriteBit(string flags)
        {
            var bit = ((ImmediateOperand)instr.op1).Value.ToInt32();
            var z = binder.EnsureFlagGroup(Registers.Z);
            var src = RewriteSrc(instr.op2);
            m.Assign(z, m.Eq0(m.And(src, 1u << bit)));
            EmitCc(src, flags);
        }

        private void RewriteTset(string flags)
        {
            var bit = ((ImmediateOperand)instr.op1).Value.ToInt32();
            var z = binder.EnsureFlagGroup(Registers.Z);
            var src = RewriteSrc(instr.op2);
            m.Assign(z, m.Eq0(m.And(src, 1u << bit)));
            EmitCc(src, flags);
            RewriteDst(
                instr.op2,
                Constant.Create(instr.op2.Width, 1 << bit),
                m.Or);
        }

        private void RewriteSetRes(bool set)
        {
            var bit = ((ImmediateOperand)instr.op1).Value.ToInt32();
            if (set)
            {
                RewriteDst(
                    instr.op2,
                    Constant.Create(instr.op2.Width, 1 << bit),
                    m.Or);
            } 
            else
            {
                RewriteDst(
                    instr.op2,
                    Constant.Create(instr.op2.Width, ~(1 << bit)),
                    m.And);
            }
        }

        private void RewriteShift(Func<Expression,Expression,Expression> fn)
        {
            var op = instr.op1;
            if (op == null)
                op = new RegisterOperand(Registers.a);
            else
                op.Width = PrimitiveType.Byte;
            var src = RewriteSrc(op);
            var dst = RewriteDst(op, src, (a, b) => fn(b, Constant.SByte(1)));
            EmitCc(dst, "**-0XP0*");
        }
    }
}
