#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Intrinsics;
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
            var ra = RewriteOperand(0, true);
            var rb = RewriteOperand(1);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.SideEffect(m.Fn(dcbtst.MakeInstance(rb.DataType), rb));
        }

        private void RewriteDcbz()
        {
            var ra = RewriteOperand(0, true);
            var rb = RewriteOperand(1);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.SideEffect(m.Fn(dcbz.MakeInstance(rb.DataType), rb));
        }

        private void RewriteEieio()
        {
            m.SideEffect(m.Fn(eieio));
        }

        private void RewriteLfd()
        {
            var op1 = RewriteOperand(0);
            var ea = EffectiveAddress(dasm.Current.Operands[1], m);
            m.Assign(op1, m.Mem(PrimitiveType.Real64, ea));
        }

        private void RewriteLfdp()
        {
            var ea = EffectiveAddress_r0(1, 0);
            var dstReg = (RegisterStorage) instr.Operands[0];
            var dst = binder.EnsureRegister(dstReg);
            m.Assign(dst, m.Mem(PrimitiveType.Real64, ea));
            ea = EffectiveAddress_r0(1, 8);
            var dst2 = binder.EnsureRegister(arch.GetRegister(dstReg.Number + 1)!);
            m.Assign(dst2, m.Mem(PrimitiveType.Real64, ea));
        }

        private void RewriteLfdpx()
        {
            Expression ea = RewriteOperand(0, true);
            var rb = RewriteOperand(1);
            if (!ea.IsZero)
            {
                rb = m.IAdd(ea, rb);
            }
            else
            {
                ea = rb;
            }
            var dstReg = (RegisterStorage) instr.Operands[0];
            var dst = binder.EnsureRegister(dstReg);
            m.Assign(dst, m.Mem(PrimitiveType.Real64, ea));
            ea = m.IAddS(ea, 8);
            var dst2 = binder.EnsureRegister(arch.GetRegister(dstReg.Number + 1)!);
            m.Assign(dst2, m.Mem(PrimitiveType.Real64, ea));
        }



        private void RewriteLfiwax()
        {
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            Expression src = m.Mem(PrimitiveType.Int32, ea);

            var dst = RewriteOperand(0);
            m.Assign(dst, m.Convert(
                src,
                PrimitiveType.Int32,
                PrimitiveType.Int64));
        }

        private void RewriteLfs()
        {
            var op1 = RewriteOperand(0);
            var ea = EffectiveAddress(dasm.Current.Operands[1], m);
            m.Assign(op1, m.Convert(
                m.Mem(PrimitiveType.Real32, ea),
                PrimitiveType.Real32,
                PrimitiveType.Real64));
        }

        private void RewriteLa(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var ea = EffectiveAddress_r0(1);
            m.Assign(op1, m.Convert(m.Mem(dtSrc, ea), dtSrc, dtDst));
        }

        private void RewriteLax(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var ea = m.IAdd(
                RewriteOperand(1, true),
                RewriteOperand(2));
            m.Assign(op1, m.Convert(m.Mem(dtSrc, ea), dtSrc, dtDst));
        }

        private void RewriteLau(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var opD = RewriteOperand(0);
            var opA = EffectiveAddress(instr.Operands[1], m);
            var ea = opA;
            m.Assign(opD, m.Convert(m.Mem(dtSrc, ea), dtSrc, dtDst));
            m.Assign(opD, ea);
        }

        private void RewriteLaux(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var opA = RewriteOperand(1);
            var opB = RewriteOperand(2, true);
            var opD = RewriteOperand(0);
            var ea = opA;
            if (!opB.IsZero)
            {
                ea = m.IAdd(opA, opB);
            }
            m.Assign(opD, m.Convert(m.Mem(dtSrc, ea), dtSrc, dtDst));
            m.Assign(opD, ea);
        }

        private void RewriteLhbrx()
        {
            var opD = RewriteOperand(0);
            var ea = EffectiveAddress_r0(instr.Operands[1], instr.Operands[2]);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp, m.Mem16(ea));
            var swap = m.Fn(swap16, tmp);
            m.Assign(opD, m.Convert(swap, PrimitiveType.Word16, opD.DataType));
        }

        private void RewriteLmw()
        {
            var r = ((RegisterStorage)instr.Operands[0]).Number;
            var ea = EffectiveAddress_r0(instr.Operands[1]);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            while (r <= 31)
            {
                var reg = binder.EnsureRegister(arch.GetRegister(r)!);
                if (reg.DataType.BitSize > 32)
                {
                    var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
                    var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(reg.DataType.BitSize - 32));
                    m.Assign(tmp2, m.Mem32(tmp));
                    m.Assign(tmpHi, m.Slice(reg, tmpHi.DataType, 32));
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
            var rDst = (RegisterStorage)instr.Operands[0];
            var rDstNext = arch.GetRegister(rDst.Number + 1)!;
            var regPair = binder.EnsureSequence(PrimitiveType.Word128, rDst, rDstNext);
            var ea = EffectiveAddress_r0(1);
            m.Assign(regPair, m.Mem(regPair.DataType, ea));
        }

        private void RewriteLswi()
        {
            var nDst = ((RegisterStorage) instr.Operands[0]).Number;
            var ea = RewriteOperand(1);
            int n = ((Constant) ImmOperand(2)).ToInt32();
            if (n == 0) n = 32;
            int cRegs = (n + 3) / 4;
            for (int i = 0; i < cRegs; ++i)
            {
                var r = arch.Registers[(nDst + i) & 0x1F];
                m.Assign(binder.EnsureRegister(r), m.Mem32(ea));
                m.Assign(ea, m.IAddS(ea, 4));
            }
        }

        private void RewriteLswx()
        {
            var a = RewriteOperand(1, true);
            var ea = RewriteOperand(2);
            if (!a.IsZero)
            {
                ea = m.IAdd(a, ea);
            }
            var ptr = new Pointer(PrimitiveType.Word32, arch.PointerType.BitSize);
            m.SideEffect(m.Fn(
                lwsx.MakeInstance(ptr, arch.xer.DataType),
                ea,
                binder.EnsureRegister(arch.xer)));
        }

        private void RewriteLvewx(PrimitiveType dtElem)
        {
            var vrt = RewriteOperand(0);
            var ra = RewriteOperand(1, true);
            var rb = RewriteOperand(2);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(
                vrt,
                m.Fn(lve.MakeInstance(arch.PointerType.BitSize, dtElem), rb, vrt));
        }

        private void RewriteStvex(PrimitiveType dt)
        {
            var vrs = RewriteOperand(0);
            var ra = RewriteOperand(1, true);
            var rb = RewriteOperand(2);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            var ptr = new Pointer(dt, arch.PointerType.BitSize);
            m.SideEffect(
                m.Fn(
                    stve.MakeInstance(ptr, dt),
                    vrs,
                    rb));
        }
        private void RewriteLvsX(IntrinsicProcedure intrinsic)
        {
            var ra = RewriteOperand(1, true);
            var rb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(vrt, m.Fn(intrinsic, rb));
        }

        private void RewriteLarx(PrimitiveType dt)
        {
            var ra = RewriteOperand(1, true);
            var rb = RewriteOperand(2);
            var dst = RewriteOperand(0);
            if (!ra.IsZero)
            {
                rb = m.IAdd(ra, rb);
            }
            m.Assign(
                dst,
                m.Fn(
                    larx.MakeInstance(arch.PointerType.BitSize, dt),
                    m.AddrOf(arch.PointerType, m.Mem(dt, rb))));
        }

        private void RewriteLwa()
        {
            var op1 = RewriteOperand(0);
            var ea = EffectiveAddress_r0(1);
            Expression src = m.Mem(PrimitiveType.Word32, ea);
            src = m.Convert(src, src.DataType, PrimitiveType.Int64);
            m.Assign(op1, src);
        }

        private void RewriteLbyteReverse(PrimitiveType dt)
        {
            var op1 = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(
                op1,
                m.Fn(
                    CommonOps.ReverseBytes.MakeInstance(dt),
                    m.Mem(dt, ea)));
        }

        private void RewriteLxsdx()
        {
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Dpb(dst, m.Mem64(ea), 64));
        }

        private void RewriteLxvd2x()
        {
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            var dst = RewriteOperand(0);
            m.Assign(dst, m.Mem(dst.DataType, ea));
        }

        private void RewriteLz(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var ea = EffectiveAddress_r0(1);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Convert(src, src.DataType, op1.DataType);
            }
            m.Assign(op1, src);
        }

        private void RewriteLzu(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var  ea = EffectiveAddress(dasm.Current.Operands[1], m);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Convert(src, src.DataType, dtDst);
            }
            m.Assign(op1, src);
            m.Assign(UpdatedRegister(ea), ea);
        }
        
        private void RewriteLzux(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var a = RewriteOperand(1);
            var b = RewriteOperand(2);
            var ea = m.IAdd(a, b);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Convert(src, src.DataType, dtDst);
            }
            m.Assign(op1, src);
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteLzx(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var op1 = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            Expression src = m.Mem(dtSrc, ea);
            if (dtDst != dtSrc)
            {
                src = m.Convert(src, src.DataType, dtDst);
            }
            m.Assign(op1, src);
        }

        private void RewriteSt(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var ea = EffectiveAddress_r0(instr.Operands[1]);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteStbrx(PrimitiveType dt)
        {
            var opS = RewriteOperand(0);
            var ea = EffectiveAddress_r0(instr.Operands[1], instr.Operands[2]);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp, m.Slice(opS, dt, 0));
            m.Assign(m.Mem(dt, ea), m.Fn(
                CommonOps.ReverseBytes.MakeInstance(dt),
                tmp));
        }

        private void RewriteStfdp()
        {
            var ea = EffectiveAddress_r0(1);
            var srcReg = (RegisterStorage) instr.Operands[0];
            var src = binder.EnsureRegister(srcReg);
            m.Assign(m.Mem(PrimitiveType.Real64, ea), src);
            ea = EffectiveAddress_r0(1, 8);
            var src2 = binder.EnsureRegister(arch.GetRegister(srcReg.Number + 1)!);
            m.Assign(m.Mem(PrimitiveType.Real64, ea), src2);
        }

        private void RewriteStfdpx()
        {
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            var srcReg = (RegisterStorage) instr.Operands[0];
            var src = binder.EnsureRegister(srcReg);
            m.Assign(m.Mem(PrimitiveType.Real64, ea), src);
            ea = m.IAddS(ea, 8);
            var src2 = binder.EnsureRegister(arch.GetRegister(srcReg.Number + 1)!);
            m.Assign(m.Mem(PrimitiveType.Real64, ea), src2);
        }


        private void RewriteStmw()
        {
            var r = ((RegisterStorage)instr.Operands[0]).Number;
            var ea = EffectiveAddress_r0(1);
            var tmp = binder.CreateTemporary(ea.DataType);
            m.Assign(tmp, ea);
            while (r <= 31)
            {
                var reg = arch.GetRegister(r)!;
                Expression w = binder.EnsureRegister(reg);
                if (reg.DataType.Size > 4)
                {
                    w = m.Slice(w, PrimitiveType.Word32);
                }
                m.Assign(m.Mem32(tmp), w);
                m.Assign(tmp, m.IAddS(tmp, 4));
                ++r;
            }
        }

        private void RewriteStq()
        {
            var rSrc = (RegisterStorage) instr.Operands[0];
            var rSrcNext = arch.GetRegister(rSrc.Number + 1)!;
            var regPair = binder.EnsureSequence(PrimitiveType.Word128, rSrc, rSrcNext);
            var ea = EffectiveAddress_r0(1);
            m.Assign(m.Mem(regPair.DataType, ea), regPair);
        }

        private void RewriteStswi()
        {
            var nDst = ((RegisterStorage) instr.Operands[0]).Number;
            var ea = RewriteOperand(1);
            int n = ((Constant) ImmOperand(2)).ToInt32();
            if (n == 0) n = 32;
            int cRegs = (n + 3) / 4;
            for (int i = 0; i < cRegs; ++i)
            {
                var r = arch.Registers[(nDst + i) & 0x1F];
                m.Assign(m.Mem32(ea), binder.EnsureRegister(r));
                m.Assign(ea, m.IAddS(ea, 4));
            }
        }

        private void RewriteStu(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var ea = EffectiveAddress(instr.Operands[1], m);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
            m.Assign(((BinaryExpression)ea).Left, EffectiveAddress(instr.Operands[1], m));
        }

        private Expression MaybeNarrow(PrimitiveType pt, Expression e)
        {
            if (e.DataType.Size != pt.Size)
            {
                if (pt.Domain == Domain.Real)
                {
                    return m.Convert(e, PrimitiveType.Create(Domain.Real, e.DataType.BitSize), pt);
                }
                else
                {
                    return m.Slice(e, pt);
                }
            }
            else
            {
                return e;
            }
        }

        private void RewriteStux(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var a = RewriteOperand(1);
            var b = RewriteOperand(2);
            m.Assign(m.Mem(dataType, m.IAdd(a, b)), MaybeNarrow(dataType, s));
            m.Assign(a, m.IAdd(a, b));
        }

        private void RewriteStcx(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            var cr0 = binder.EnsureFlagGroup(arch.cr0);
            var ptr = new Pointer(dataType, arch.PointerType.BitSize);
            m.Assign(
                cr0,
                m.Fn(
                    stcx.MakeInstance(ptr, dataType),
                    m.AddrOf(ptr, m.Mem(dataType, ea)),
                    MaybeNarrow(dataType, s)));
        }

        private void RewriteStwcix(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.SideEffect(m.Fn(
                stwcix.MakeInstance(arch.PointerType.BitSize, dataType),
                m.AddrOf(arch.PointerType, m.Mem(dataType, ea)),
                MaybeNarrow(dataType, s)));
        }

        private void RewriteStx(PrimitiveType dataType)
        {
            var s = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(m.Mem(dataType, ea), MaybeNarrow(dataType, s));
        }

        private void RewriteStxsdx()
        {
            var s = RewriteOperand(0);
            var a = RewriteOperand(1, true);
            var b = RewriteOperand(2);
            var ea = (a.IsZero)
                ? b
                : m.IAdd(a, b);
            m.Assign(
                m.Mem64(ea),
                m.Slice(s, PrimitiveType.Word64, 64));
        }

        private void RewriteSync()
        {
            m.SideEffect(m.Fn(sync_intrinsic));
        }

        private void RewriteTrap(PrimitiveType size)
        {
            var c = (Constant) RewriteOperand(0);
            var ra = RewriteOperand(1);
            var rb = RewriteOperand(2);
            Func<Expression,Expression,Expression>? op = null;
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
                m.SideEffect(m.Fn(trap_intrinsic));
                return;
            default:
                host.Warn(
                    instr.Address,
                    string.Format("Unsupported trap operand {0:X2}.", c.ToInt32()));
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.BranchInMiddleOfInstruction(
                op(ra, rb).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(m.Fn(trap_intrinsic));
        }
    }
}
