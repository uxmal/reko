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


        private void RewriteLfd()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, m);
            m.Assign(op1, m.Mem(PrimitiveType.Real64, ea));
        }

        private void RewriteLfs()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, m);
            m.Assign(op1, m.Cast(PrimitiveType.Real64,
                m.Mem(PrimitiveType.Real32, ea)));
        }

        private void RewriteLha()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, m);
            m.Assign(op1, m.Cast(PrimitiveType.Int32,
                m.Mem(PrimitiveType.Int16, ea)));
        }

        private void RewriteLhax()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = m.IAdd(
                RewriteOperand(instr.op2, true),
                RewriteOperand(instr.op3));
            m.Assign(op1, m.Cast(PrimitiveType.Int32,
                m.Mem(PrimitiveType.Int16, ea)));
        }

        private void RewriteLhau()
        {
            var opD = RewriteOperand(instr.op1);
            var opA = EffectiveAddress(instr.op2, m);
            var ea = opA;
            //$TODO: should be convert...
            m.Assign(opD, m.Cast(PrimitiveType.Int32, m.Mem16(ea)));
            m.Assign(opD, ea);
        }

        private void RewriteLhaux()
        {
            var opD = RewriteOperand(instr.op1);
            var opA = RewriteOperand(instr.op2);
            var opB = RewriteOperand(instr.op3, true);
            var ea = opA;
            if (!opB.IsZero)
            {
                ea = m.IAdd(opA, opB);
            }
            //$TODO: should be convert...
            m.Assign(opD, m.Cast(PrimitiveType.Int32, m.Mem16(ea)));
            m.Assign(opD, ea);
        }

        private void RewriteLhbrx()
        {
            var opD = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, instr.op3);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp, m.Mem16(ea));
            m.Assign(opD, m.Cast(opD.DataType, host.PseudoProcedure(
                "__swap16", tmp.DataType, tmp)));
        }


        private void RewriteLmw()
        {
            var r = ((RegisterOperand)instr.op1).Register.Number;
            var ea = EffectiveAddress_r0(instr.op2, m);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            while (r <= 31)
            {
                var reg = binder.EnsureRegister(arch.GetRegister(r));
                Expression w = reg;
                if (reg.DataType.Size > 4)
                {
                    var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
                    m.Assign(tmp2, m.Mem32(tmp));
                    m.Assign(reg, m.Dpb(reg, tmp2, 0));
                }
                else
                {
                    m.Assign(reg, m.Mem32(tmp));
                }
                m.Assign(tmp, m.IAdd(tmp, m.Int32(4)));
                ++r;
            }
        }

        private void RewriteLq()
        {
            var rDst = ((RegisterOperand)instr.op1).Register;
            if ((rDst.Number & 1) == 1)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var rDstNext = arch.GetRegister(rDst.Number + 1);
            var regPair = binder.EnsureSequence(rDst, rDstNext, PrimitiveType.Word128);
            var ea = EffectiveAddress_r0(instr.op2, m);
            m.Assign(regPair, m.Mem(regPair.DataType, ea));
        }

        private void RewriteLvewx()
        {
            var vrt = RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2, true);
            var rb = RewriteOperand(instr.op3);
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
            var vrs = RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2, true);
            var rb = RewriteOperand(instr.op3);
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
            var vrt = RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2, true);
            var rb = RewriteOperand(instr.op3);
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

        private void RewriteLwbrx()
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
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

        private void RewriteLz(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, m);
            m.Assign(op1, m.Mem(dataType, ea));
        }

        private void RewriteLzu(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(dasm.Current.op1);
            var  ea = EffectiveAddress(dasm.Current.op2, m);
            m.Assign(op1, m.Mem(dataType, ea));
            m.Assign(UpdatedRegister(ea), ea);
        }
        
        private void RewriteLzux(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2);
            var b = RewriteOperand(instr.op3);
            var ea = m.IAdd(a, b);
            m.Assign(op1, m.Mem(dataType, ea));
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteLzx(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(op1, m.Mem(dataType, ea));
        }

        private void RewriteSt(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, m);
            m.Assign(m.Mem(dataType, ea), s);
        }

        private void RewriteStmw()
        {
            var r = ((RegisterOperand)instr.op1).Register.Number;
            var ea = EffectiveAddress_r0(instr.op2, m);
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
                m.Assign(tmp, m.IAdd(tmp, m.Int32(4)));
                ++r;
            }
        }

        private void RewriteStu(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(instr.op2, m);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
            m.Assign(((BinaryExpression)ea).Left, EffectiveAddress(instr.op2, m));
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
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2);
            var b = RewriteOperand(instr.op3);
            m.Assign(m.Mem(dataType, m.IAdd(a, b)), MaybeNarrow(dataType, s));
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteStwbrx()
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
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

        private void RewriteStx(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteSync()
        {
            m.SideEffect(host.PseudoProcedure("__sync", VoidType.Instance));
        }

        private void RewriteTw()
        {
            var c = (Constant) RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2);
            var rb = RewriteOperand(instr.op3);
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
            default: throw new AddressCorrelatedException(
                instr.Address,
                string.Format("Unsupported trap operand {0:X2}.", c.ToInt32()));
            }
            rtlc = InstrClass.Linear;
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
