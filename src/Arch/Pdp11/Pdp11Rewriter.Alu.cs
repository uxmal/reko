#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Arch.Pdp11
{
    public partial class Pdp11Rewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var src = binder.EnsureFlagGroup(Registers.C);
            var dst = RewriteDst(instr.Operands[0], src, fn);
            if (dst == null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
            }
            else
            {
                SetFlags(dst, Registers.NZVC);
            }
        }

        private void RewriteAdd()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, m.IAdd);
            SetFlags(dst, Registers.NZVC);
        }

        private void RewriteAshc()
        {
            var r = ((RegisterOperand)instr.Operands[1]).Register;
            if (r == Registers.pc)
            {
                m.Invalid();
                return;
            }
            var r1 = arch.GetRegister(r.Number + 1);
            var dst = binder.EnsureSequence(PrimitiveType.Word32, r, r1);
            var sh = RewriteSrc(instr.Operands[0]);
            if (sh == null)
            {
                m.Invalid();
                return;
            }
            var csh = sh as Constant;
            if (csh != null)
            {
                var n = csh.ToInt16();
                if (n >= 0)
                {
                    m.Assign(dst, m.Shl(dst, Constant.Int16(n)));
                }
                else
                {
                    m.Assign(dst, m.Sar(dst, Constant.Int16(n)));
                }
            }
            else
            {
                m.Assign(dst, host.Intrinsic("__shift", true, dst.DataType, dst, sh));
            }
            SetFlags(dst, Registers.NVC);
        }

        private void RewriteAsl()
        {
            var src = Constant.Int16(1);
            var dst = RewriteDst(instr.Operands[0], src, m.Shl);
            SetFlags(dst, Registers.NZVC);
        }

        private void RewriteAsr()
        {
            var src = Constant.Int16(1);
            var dst = RewriteDst(instr.Operands[0], src, m.Sar);
            SetFlags(dst, Registers.NZVC);
        }

        private void RewriteBic()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, (a, b) => m.And(a, m.Comp(b)));
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteBis()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, m.Or);
            SetFlags(dst, Registers.NZ);
            if (dst != null)
            {
                SetFalse(Registers.V);
            }
        }

        private void RewriteBit()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, m.And);
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteClr(Pdp11Instruction instr, Expression src)
        {
            var dst = RewriteDst(instr.Operands[0], src, s => s);
            SetFalse(Registers.C);
            SetFalse(Registers.V);
            SetFalse(Registers.N);
            SetTrue(Registers.Z);
        }

        private void RewriteClrSetFlags(Func<Constant> gen)
        {
            var grf = ((ImmediateOperand)instr.Operands[0]).Value.ToByte();
            AddFlagAssignment(grf, Registers.N, gen);
            AddFlagAssignment(grf, Registers.Z, gen);
            AddFlagAssignment(grf, Registers.V, gen);
            AddFlagAssignment(grf, Registers.C, gen);
        }

        private void AddFlagAssignment(uint grf, FlagGroupStorage flag, Func<Constant> gen)
        {
            if ((grf & flag.FlagGroupBits) != 0)
            {
                m.Assign(
                    binder.EnsureFlagGroup(flag),
                    gen());
            }
        }

        private void RewriteCmp()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteSrc(instr.Operands[1]);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, m.ISub(dst, src));
            SetFlags(tmp, Registers.NZVC);
        }

        private void RewriteCom()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, m.Comp);
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.V);
            SetTrue(Registers.C);
        }

        private void RewriteDiv()
        {
            var reg = ((RegisterOperand)instr.Operands[1]).Register;
            var reg1 = arch.GetRegister(reg.Number | 1);
            var reg_reg = binder.EnsureSequence(PrimitiveType.Int32, reg, reg1);
            var dividend = binder.CreateTemporary(PrimitiveType.Int32);
            var divisor = RewriteSrc(instr.Operands[0]);
            var quotient = binder.EnsureRegister(reg);
            var remainder = binder.EnsureRegister(reg1);
            m.Assign(dividend, reg_reg);
            m.Assign(quotient, m.SDiv(dividend, divisor));
            m.Assign(remainder, m.Mod(dividend, divisor));
            SetFlags(quotient, Registers.NZVC);
        }

        private void RewriteIncDec(Func<Expression, int, Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, s => fn(s, 1));
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteMfpd()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src == null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, host.Intrinsic("__mfpd", false, tmp.DataType, src));
            m.Assign(sp, m.ISub(sp, 2));
            m.Assign(m.Mem16(sp), tmp);
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMfpi()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src == null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, host.Intrinsic("__mfpi", false, tmp.DataType, src));
            m.Assign(sp, m.ISub(sp, 2));
            m.Assign(m.Mem16(sp), tmp);
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMtpi()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src == null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, m.Mem16(sp));
            m.Assign(sp, m.ISub(sp, 2));
            m.SideEffect(host.Intrinsic("__mtpi", false, tmp.DataType, src, tmp));
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMov()
        {
            var src = RewriteSrc(instr.Operands[0]);
            Expression dst;
            if (instr.Operands[1] is RegisterOperand && instr.DataWidth.Size == 1)
            {
                dst = RewriteDst(instr.Operands[1], src, s => m.Convert(s, s.DataType, PrimitiveType.Int16));
            }
            else
            {
                dst = RewriteDst(instr.Operands[1], src, s => s);
            }
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMul()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var reg = ((RegisterOperand)instr.Operands[1]).Register;
            // Even numbered register R_2n means result
            // goes into the pair [R_2n:R_2n+1]
            Identifier dst;
            if ((reg.Number & 1) == 0)
            {
                var regLo = arch.GetRegister(reg.Number + 1);
                dst = binder.EnsureSequence(PrimitiveType.Int32, reg, regLo);
            }
            else
            {
                dst = binder.EnsureRegister(reg);
            }
            m.Assign(dst, m.SMul(RewriteSrc(instr.Operands[1]), src));
            SetFlags(dst, Registers.NZC);
            SetFalse(Registers.V);
        }

        private void RewriteNeg()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, m.Neg);
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteRotate(string op, uint cyMask)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.psw, (uint) FlagM.CF));
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) =>
                host.Intrinsic(op, true, instr.DataWidth, a, m.Int16(1), C));
            m.Assign(C, m.Ne0(m.And(tmp, cyMask)));
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteShift()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var immSrc = src as Constant;
            Func<Expression, Expression, Expression> fn = null;
            if (immSrc != null)
            {
                int sh = immSrc.ToInt32();
                if (sh < 0)
                {
                    fn = m.Sar;
                    src = Constant.Int16((short)-sh);
                }
                else
                {
                    fn = m.Shl;
                    src = Constant.Int16((short)sh);
                }
            }
            else
            {
                fn = (a, b) =>
                    host.Intrinsic("__shift", true, instr.DataWidth, a, b);
            }
            var dst = RewriteDst(instr.Operands[1], src, fn);
            SetFlags(dst, Registers.NZVC);
        }

        private void RewriteSub()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, m.ISub);
            SetFlags(dst, Registers.NZVC);
        }

        private void RewriteStexp()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, e =>
                host.Intrinsic("__stexp",  false, PrimitiveType.Int16, e));
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.C);
            SetFalse(Registers.V);
        }

        private void RewriteSxt()
        {
            var n  = binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.psw, (uint)FlagM.NF));

            var src = m.ISub(Constant.Int16(0), n);
            var dst = RewriteDst(instr.Operands[0], src, s => s);
            SetFlags(dst, Registers.Z);
        }

        private void RewriteSwab()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, e => host.Intrinsic("__swab", true, PrimitiveType.Word16, e));
            if (dst == null)
            {
                m.Invalid();
            }
            else
            {
                SetFlags(dst, Registers.NZ);
                SetFalse(Registers.C);
                SetFalse(Registers.V);
            }
        }
        private void RewriteTst()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            m.Assign(tmp, m.And(tmp, tmp));
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.C);
            SetFalse(Registers.V);
        }

        private void RewriteXor()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[1], src, m.Xor);
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.C);
            SetFalse(Registers.V);
        }
    }
}
