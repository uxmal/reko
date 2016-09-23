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
        private void RewriteDcbt()
        {
            // This is just a hint to the cache; makes no sense to have it in
            // high-level language. Consider adding option to have cache
            // hint instructions decompiled into intrinsics
        }

        private void RewriteLfd()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Load(PrimitiveType.Real64, ea));
        }

        private void RewriteLfs()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Cast(PrimitiveType.Real64,
                emitter.Load(PrimitiveType.Real32, ea)));
        }

        private void RewriteLha()
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Cast(PrimitiveType.Int32,
                emitter.Load(PrimitiveType.Int16, ea)));
        }

        private void RewriteLhau()
        {
            var opD = RewriteOperand(instr.op1);
            var opA = EffectiveAddress(instr.op2, emitter);
            var ea = opA;
            //$TODO: should be convert...
            emitter.Assign(opD, emitter.Cast(PrimitiveType.Int32, emitter.LoadW(ea)));
            emitter.Assign(opD, ea);
        }

        private void RewriteLhaux()
        {
            var opD = RewriteOperand(instr.op1);
            var opA = RewriteOperand(instr.op2);
            var opB = RewriteOperand(instr.op3, true);
            var ea = opA;
            if (!opB.IsZero)
            {
                ea = emitter.IAdd(opA, opB);
            }
            //$TODO: should be convert...
            emitter.Assign(opD, emitter.Cast(PrimitiveType.Int32, emitter.LoadW(ea)));
            emitter.Assign(opD, ea);
        }

        private void RewriteLvewx()
        {
            var vrt = RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2, true);
            var rb = RewriteOperand(instr.op3);
            if (!ra.IsZero)
            {
                rb = emitter.IAdd(ra, rb);
            }
            emitter.Assign(
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
                rb = emitter.IAdd(ra, rb);
            }
            emitter.SideEffect(
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
                rb = emitter.IAdd(ra, rb);
            }
            emitter.Assign(
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
                : emitter.IAdd(a, b);
            emitter.Assign(
                op1,
                host.PseudoProcedure(
                    "__reverse_bytes_32",
                    PrimitiveType.Word32,
                    emitter.Load(PrimitiveType.Word32, ea)));
        }

        private void RewriteLz(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Load(dataType, ea));
        }

        private void RewriteLzu(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(dasm.Current.op1);
            var  ea = EffectiveAddress(dasm.Current.op2, emitter);
            emitter.Assign(op1, emitter.Load(dataType, ea));
            emitter.Assign(UpdatedRegister(ea), ea);
        }
        
        private void RewriteLzux(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2);
            var b = RewriteOperand(instr.op3);
            var ea = emitter.IAdd(a, b);
            emitter.Assign(op1, emitter.Load(dataType, ea));
            emitter.Assign(a, emitter.IAdd(a, b));
        }

        private void RewriteLzx(PrimitiveType dataType)
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : emitter.IAdd(a, b);
            emitter.Assign(op1, emitter.Load(dataType, ea));
        }

        private void RewriteSt(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress_r0(instr.op2, emitter);
            emitter.Assign(emitter.Load(dataType, ea), s);
        }

        private void RewriteStu(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var ea = EffectiveAddress(instr.op2, emitter);
            emitter.Assign(emitter.Load(dataType, ea), MaybeNarrow(dataType, s));
            emitter.Assign(((BinaryExpression)ea).Left, EffectiveAddress(instr.op2, emitter));
        }

        private Expression MaybeNarrow(PrimitiveType pt, Expression e)
        {
            if (e.DataType.Size != pt.Size)
                return emitter.Cast(pt, e);
            else
                return e;
        }

        private void RewriteStux(PrimitiveType dataType)
        {
            var s = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2);
            var b = RewriteOperand(instr.op3);
            emitter.Assign(emitter.Load(dataType, emitter.IAdd(a, b)), MaybeNarrow(dataType, s));
            emitter.Assign(a, emitter.IAdd(a, b));
        }

        private void RewriteStwbrx()
        {
            var op1 = RewriteOperand(instr.op1);
            var a = RewriteOperand(instr.op2, true);
            var b = RewriteOperand(instr.op3);
            var ea = (a.IsZero)
                ? b
                : emitter.IAdd(a, b);
            emitter.Assign(
                emitter.Load(PrimitiveType.Word32, ea),
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
                : emitter.IAdd(a, b);
            emitter.Assign(emitter.Load(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteSync()
        {
            emitter.SideEffect(host.PseudoProcedure("__sync", VoidType.Instance));
        }

        private void RewriteTw()
        {
            var c = (Constant) RewriteOperand(instr.op1);
            var ra = RewriteOperand(instr.op2);
            var rb = RewriteOperand(instr.op3);
            Operator op = null;
            switch (c.ToInt32())
            {
            case 0x01: op = Operator.Ugt; break;
            case 0x02: op = Operator.Ult; break;
            case 0x04: op = Operator.Eq; break;
            case 0x05: op = Operator.Uge; break;
            case 0x06: op = Operator.Ule; break;
            case 0x08: op = Operator.Gt; break;
            case 0x0C: op = Operator.Ge; break;
            case 0x10: op = Operator.Lt; break;
            case 0x14: op = Operator.Le; break;
            case 0x18: op = Operator.Ne; break;
            default: throw new AddressCorrelatedException(
                instr.Address,
                string.Format("Unsupported trap operand {0:X2}.", c.ToInt32()));
            }
            cluster.Class = RtlClass.Linear;
            emitter.If(
                new BinaryExpression(
                    op,
                    PrimitiveType.Bool,
                    ra, rb),
                new RtlSideEffect(
                    host.PseudoProcedure(
                        "__trap",
                        VoidType.Instance)));

        }
    }
}
