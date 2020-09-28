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
using Reko.Core.Machine;
using Reko.Core.Operators;
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
        private void RewriteDcbtst()
        {
            var ra = RewriteOperand(instr.Operands[0], true);
            var rb = RewriteOperand(instr.Operands[1]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.SideEffect(
                host.PseudoProcedure("__dcbtst", VoidType.Instance, rb));
        }

        private void RewriteDcbz()
        {
            var ra = RewriteOperand(instr.Operands[0], true);
            var rb = RewriteOperand(instr.Operands[1]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.SideEffect(
                host.PseudoProcedure("__dcbz", VoidType.Instance, rb));
        }

        private void RewriteEieio()
        {
            m.SideEffect(host.PseudoProcedure("__eieio", VoidType.Instance));
        }

        private void RewriteLfd()
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress(dasm.Current.Operands[1], m);
            m.Assign(op1, m.Mem(PrimitiveType.Real64, ea));
        }

        private void RewriteLfs()
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress(dasm.Current.Operands[1], m);
            m.Assign(op1, m.Cast(PrimitiveType.Real64,
                m.Mem(PrimitiveType.Real32, ea)));
        }

        private void RewriteLa(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress_r0(dasm.Current.Operands[1], m);
            m.Assign(op1, m.Cast(dtDst, m.Mem(dtSrc, ea)));
        }

        private void RewriteLax(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var ea = m.IAdd(
                RewriteOperand(instr.Operands[1], true),
                RewriteOperand(instr.Operands[2]));
            m.Assign(op1, m.Cast(dtDst, m.Mem(dtSrc, ea)));
        }

        private void RewriteLau(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opA = EffectiveAddress(instr.Operands[1], m);
            var ea = opA;
            m.Assign(opD, m.Cast(dtDst, m.Mem(dtSrc, ea)));
            m.Assign(opD, ea);
        }

        private void RewriteLaux(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var opA = RewriteOperand(instr.Operands[1]);
            var opB = RewriteOperand(instr.Operands[2], true);
            var ea = opA;
            if (!opB.IsZero)
            {
                ea = m.IAdd(opA, opB);
            }
            //$TODO: should be convert...
            m.Assign(opD, m.Cast(dtDst, m.Mem(dtSrc, ea)));
            m.Assign(opD, ea);
        }

        private void RewriteLhbrx()
        {
            var opD = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress_r0(instr.Operands[1], instr.Operands[2]);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp, m.Mem16(ea));
            m.Assign(opD, m.Cast(opD.DataType, host.PseudoProcedure(
                "__swap16", tmp.DataType, tmp)));
        }


        private void RewriteLmw()
        {
            var r = ((RegisterOperand)instr.Operands[0]).Register.Number;
            var ea = EffectiveAddress_r0(instr.Operands[1], m);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            while (r <= 31)
            {
                var reg = binder.EnsureRegister(arch.GetRegister(r));
                Expression w = reg;
                if (reg.DataType.BitSize > 32)
                {
                    var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
                    var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(reg.DataType.BitSize - 32));
                    m.Assign(tmp2, m.Mem32(tmp));
                    m.Assign(tmpHi, m.Slice(tmpHi.DataType, reg, 32));
                    m.Assign(reg, m.Seq(tmpHi, tmp2));
                }
                else
                {
                    m.Assign(reg, m.Mem32(tmp));
                }
                m.Assign(tmp, m.IAddS(tmp, 4));
                ++r;
            }
        }

        private void RewriteLq()
        {
            var rDst = ((RegisterOperand)instr.Operands[0]).Register;
            if ((rDst.Number & 1) == 1)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var rDstNext = arch.GetRegister(rDst.Number + 1);
            var regPair = binder.EnsureSequence(PrimitiveType.Word128, rDst, rDstNext);
            var ea = EffectiveAddress_r0(instr.Operands[1], m);
            m.Assign(regPair, m.Mem(regPair.DataType, ea));
        }

        private void RewriteLvewx()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var ra = RewriteOperand(instr.Operands[1], true);
            var rb = RewriteOperand(instr.Operands[2]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__lvewx",
                    PrimitiveType.Word128,
                    rb));
        }

        private void RewriteStvewx()
        {
            var vrs = RewriteOperand(instr.Operands[0]);
            var ra = RewriteOperand(instr.Operands[1], true);
            var rb = RewriteOperand(instr.Operands[2]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.SideEffect(
                host.PseudoProcedure(
                    "__stvewx",
                    PrimitiveType.Word128,
                    vrs,
                    rb));
        }
        private void RewriteLvsl()
        {
            var vrt = RewriteOperand(instr.Operands[0]);
            var ra = RewriteOperand(instr.Operands[1], true);
            var rb = RewriteOperand(instr.Operands[2]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(
                vrt,
                host.PseudoProcedure(
                    "__lvsl",
                    PrimitiveType.Word128,
                    rb));
        }

        private void RewriteLarx(string intrinsic, PrimitiveType dt)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var ra = RewriteOperand(instr.Operands[1], true);
            var rb = RewriteOperand(instr.Operands[2]);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(
                dst,
                host.PseudoProcedure(intrinsic, dt, m.AddrOf(arch.PointerType, m.Mem(dt, rb))));
        }

        private void RewriteLwbrx()
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1], true);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(
                op1,
                host.PseudoProcedure(
                    "__reverse_bytes_32",
                    PrimitiveType.Word32,
                    m.Mem(PrimitiveType.Word32, ea)));
        }

        private void RewriteLz(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress_r0(dasm.Current.Operands[1], m);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Cast(op1.DataType, src);
            }
            m.Assign(op1, src);
        }

        private void RewriteLzu(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(dasm.Current.Operands[0]);
            var  ea = EffectiveAddress(dasm.Current.Operands[1], m);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Cast(dtDst, src);
            }
            m.Assign(op1, src);
            m.Assign(UpdatedRegister(ea), ea);
        }
        
        private void RewriteLzux(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = m.IAdd(a, b);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Cast(dtDst, src);
            }
            m.Assign(op1, src);
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteLzx(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1], true);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Cast(dtDst, src);
            }
            m.Assign(op1, src);
        }

        private void RewriteSt(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress_r0(instr.Operands[1], m);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteStmw()
        {
            var r = ((RegisterOperand)instr.Operands[0]).Register.Number;
            var ea = EffectiveAddress_r0(instr.Operands[1], m);
            var tmp = binder.CreateTemporary(ea.DataType);
            while (r <= 31)
            {
                var reg = arch.GetRegister(r);
                Expression w = binder.EnsureRegister(reg);
                if (reg.DataType.Size > 4)
                {
                    w = m.Slice(PrimitiveType.Word32, w, 0);
                }
                m.Assign(m.Mem32(tmp), w);
                m.Assign(tmp, m.IAddS(tmp, 4));
                ++r;
            }
        }

        private void RewriteStu(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.Operands[0]);
            var ea = EffectiveAddress(instr.Operands[1], m);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
            m.Assign(((BinaryExpression)ea).Left, EffectiveAddress(instr.Operands[1], m));
        }

        private Expression MaybeNarrow(PrimitiveType pt, Expression e)
        {
            if (e.DataType.Size != pt.Size)
                return m.Cast(pt, e);
            else
                return e;
        }

        private void RewriteStux(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1]);
            var b = RewriteOperand(instr.Operands[2]);
            m.Assign(m.Mem(dataType, m.IAdd(a, b)), MaybeNarrow(dataType, s));
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteStwbrx()
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1], true);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(
                m.Mem(PrimitiveType.Word32, ea),
                host.PseudoProcedure(
                    "__reverse_bytes_32",
                    PrimitiveType.Word32,
                    op1));
        }

        private void RewriteStcx(string intrinsic, PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1], true);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            var cr0 = binder.EnsureFlagGroup(arch.cr, 0xF, "cr0", PrimitiveType.Byte);
            m.Assign(
                cr0,
                host.PseudoProcedure(
                    intrinsic,
                    VoidType.Instance,
                    m.AddrOf(arch.PointerType, m.Mem(dataType, ea)),
                    MaybeNarrow(dataType, s)));
        }

        private void RewriteStx(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.Operands[0]);
            var a = RewriteOperand(instr.Operands[1], true);
            var b = RewriteOperand(instr.Operands[2]);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteSync()
        {
            m.SideEffect(host.PseudoProcedure("__sync", VoidType.Instance));
        }

        private void RewriteTrap(PrimitiveType size)
        {
            var c = (Constant) RewriteOperand(instr.Operands[0]);
            var ra = RewriteOperand(instr.Operands[1]);
            var rb = RewriteOperand(instr.Operands[2]);
            Func<Expression,Expression,Expression> op = null;
            switch (c.ToInt32())
            {
            case 0x01: op = m.Ugt; break;
            case 0x02: op = m.Ult; break;
            case 0x04: op = m.Eq; break;
            case 0x05: op = m.Uge; break;
            case 0x06: op = m.Ule; break;
            case 0x08: op = m.Gt; break;
            case 0x0C: op = m.Ge; break;
            case 0x10: op = m.Lt; break;
            case 0x14: op = m.Le; break;
            case 0x18: op = m.Ne; break;
            case 0x1F:
                m.SideEffect(
                    host.PseudoProcedure(
                        "__trap",
                        VoidType.Instance));
                return;
            default:
                host.Error(
                    instr.Address,
                    string.Format("Unsupported trap operand {0:X2}.", c.ToInt32()));
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.BranchInMiddleOfInstruction(
                op(ra, rb).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                host.PseudoProcedure(
                    "__trap",
                    VoidType.Instance));
        }
    }
}
