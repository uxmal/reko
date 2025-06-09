#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Output;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Pdp.Pdp11
{
    public partial class Pdp11Rewriter
    {
        private void RewriteAdcSbc(Func<Expression, Expression, Expression> fn)
        {
            var src = binder.EnsureFlagGroup(Registers.C);
            var dst = RewriteDst(instr.Operands[0], src, fn);
            if (dst is null)
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
            var r = (RegisterStorage)instr.Operands[1];
            if (r == Registers.pc)
            {
                m.Invalid();
                return;
            }
            var r1 = arch.GetRegister(r.Number + 1)!;
            var dst = binder.EnsureSequence(PrimitiveType.Word32, r, r1);
            var sh = RewriteSrc(instr.Operands[0]);
            if (sh is null)
            {
                m.Invalid();
                return;
            }
            if (sh is Constant csh)
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
                m.Assign(dst, m.Fn(shift_intrinsic.MakeInstance(dst.DataType, sh.DataType), dst, sh));
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
            if (dst is not null)
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

        private void RewriteClrSetFlags(bool setFlag)
        {
            var grf = ((Constant)instr.Operands[0]).ToByte();
            AddFlagAssignment(grf, Registers.N, setFlag);
            AddFlagAssignment(grf, Registers.Z, setFlag);
            AddFlagAssignment(grf, Registers.V, setFlag);
            AddFlagAssignment(grf, Registers.C, setFlag);
        }

        private void AddFlagAssignment(uint grf, FlagGroupStorage flag, bool setFlag)
        {
            if ((grf & flag.FlagGroupBits) != 0)
            {
                var dst = binder.EnsureFlagGroup(flag);
                var src = setFlag
                    ? flag.FlagGroupBits
                    : 0u;
                m.Assign(dst, src);
            }
        }

        private void RewriteCmp(bool useByte)
        {
            var src = RewriteSrc(instr.Operands[0], useByte);
            var dst = RewriteSrc(instr.Operands[1], useByte);
            var tmp = binder.CreateTemporary(src.DataType);
            // PDP-11 manual explicitly states that the src,dst
            // operands are switched for cmp instructions.
            m.Assign(tmp, m.ISub(src, dst));
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
            var reg = (RegisterStorage)instr.Operands[1];
            var reg1 = arch.GetRegister(reg.Number | 1)!;
            var reg_reg = binder.EnsureSequence(PrimitiveType.Int32, reg, reg1);
            var dividend = binder.CreateTemporary(PrimitiveType.Int32);
            var divisor = RewriteSrc(instr.Operands[0]);
            var quotient = binder.EnsureRegister(reg);
            var remainder = binder.EnsureRegister(reg1);
            m.Assign(dividend, reg_reg);
            m.Assign(quotient, m.SDiv(dividend, divisor));
            m.Assign(remainder, m.SMod(dividend, divisor));
            SetFlags(quotient, Registers.NZVC);
        }

        private void RewriteIncDec(Func<Expression, long, Expression> fn)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var dst = RewriteDst(instr.Operands[0], src, s => fn(s, 1));
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteMfpd()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src is null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, m.Fn(mfpd_intrinsic, src));
            m.Assign(sp, m.ISub(sp, 2));
            m.Assign(m.Mem16(sp), tmp);
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMfpi()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src is null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, m.Fn(mfpi_intrinsic, src));
            m.Assign(sp, m.ISub(sp, 2));
            m.Assign(m.Mem16(sp), tmp);
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMfpt()
        {
            var r0 = binder.EnsureRegister(Registers.r0);
            m.Assign(r0, m.Fn(mfpt_intrinsic));
        }

        private void RewriteMtpi()
        {
            var src = RewriteSrc(instr.Operands[0]);
            if (src is null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(tmp, m.Mem16(sp));
            m.Assign(sp, m.ISub(sp, 2));
            m.SideEffect(m.Fn(mtpi_intrinsic, src, tmp));
            SetFlags(tmp, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMov()
        {
            var src = RewriteSrc(instr.Operands[0], instr.DataWidth!.Size == 1);
            Expression dst;
            if (instr.Operands[1] is RegisterStorage && instr.DataWidth!.Size == 1)
            {
                dst = RewriteDst(instr.Operands[1], src, s => m.Convert(s, s.DataType, PrimitiveType.Int16))!;
            }
            else
            {
                dst = RewriteDst(instr.Operands[1], src, s => s)!;
            }
            SetFlags(dst, Registers.NZ);
            SetFalse(Registers.V);
        }

        private void RewriteMul()
        {
            var src = RewriteSrc(instr.Operands[0]);
            var reg = (RegisterStorage)instr.Operands[1];
            // Even numbered register R_2n means result
            // goes into the pair [R_2n:R_2n+1]
            Identifier dst;
            if ((reg.Number & 1) == 0)
            {
                var regLo = arch.GetRegister(reg.Number + 1)!;
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
            var dst = RewriteDst(instr.Operands[0], src, m.Neg)!;
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteRotate(IntrinsicProcedure op, uint cyMask)
        {
            var src = RewriteSrc(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(this.arch.GetFlagGroup(Registers.psw, (uint) FlagM.CF));
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            var dst = RewriteDst(instr.Operands[0], src, (a, b) =>
                m.Fn(op.MakeInstance(a.DataType, PrimitiveType.Byte), a, m.Byte(1), C))!;
            m.Assign(C, m.Ne0(m.And(tmp, cyMask)));
            SetFlags(dst, Registers.NZV);
        }

        private void RewriteShift()
        {
            var src = RewriteSrc(instr.Operands[0]);
            Func<Expression, Expression, Expression>? fn = null;
            if (src is Constant immSrc)
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
                    m.Fn(shift_intrinsic.MakeInstance(instr.DataWidth!, b.DataType), a, b);
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
                m.Fn(stexp_intrinsic, e));
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
            var dst = RewriteDst(instr.Operands[0], src, e => m.Fn(swab_intrinsic, e));
            if (dst is null)
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
