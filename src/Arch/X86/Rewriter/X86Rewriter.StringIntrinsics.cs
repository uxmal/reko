#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Core.Types;

namespace Reko.Arch.X86.Rewriter
{
    /// <summary>
    /// Rewrite string intrinsics(strlen, memcpy, etc.).
    /// </summary>
    public partial class X86Rewriter
    {
        private static readonly IntrinsicProcedure findFirstDifference = new IntrinsicBuilder("__find_first_difference__", false)
            .PtrParam(new UnknownType())
            .PtrParam(new UnknownType())
            .Returns(new UnknownType());

        private bool RewriteStringIntrinsic()
        {
            if (!IsAutoIncrementingMode())
                return false;
            switch (instrCur.Mnemonic)
            {
            case Mnemonic.scasb:
                // repne scasb
                if (IsRepNe())
                    return RewriteScasbToStrlen();
                break;
            case Mnemonic.movs:
            case Mnemonic.movsb:
                // rep movs
                if (IsRep())
                    return RewriteMovsToMemcpy();
                break;
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
                // repe cmps
                if (IsRepE())
                    return RewriteCmpsToToFindFirstDifference();
                break;
            }
            return false;
        }

        private bool RewriteScasbToStrlen()
        {
            var cAl = state.GetRegister(Registers.al);
            if (!cAl.IsZero)
                return false;
            var cEcx = state.GetRegister((RegisterStorage)RegCx.Storage);
            if (!cEcx.IsMaxUnsigned)
                return false;
            var size = binder.CreateTemporary("size", PrimitiveType.Create(Domain.UnsignedInt, RegCx.DataType.BitSize));
            var di = RegDi;
            m.Assign(size, m.IAddS(m.Fn(Strlen(), MemIndexPtr(1, Registers.es, di)), 1));
            var cx = RegCx;
            m.Assign(cx, m.ISub(cx, MaybeSlice(cx.DataType, size)));
            m.Assign(di, m.IAdd(di, MaybeSlice(cx.DataType, size)));
            var grf = binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!);
            m.Assign(
                grf,
                m.Cond(grf.DataType, m.Const(instrCur.DataWidth, 0)));
            return true;
        }

        private bool RewriteMovsToMemcpy()
        {
            var regCx = RegCx;
            var sizeExpr = MakeSizeExpression(regCx);
            var size = binder.CreateTemporary("size", sizeExpr.DataType);
            m.Assign(size, sizeExpr);
            var si = RegSi;
            var di = RegDi;
            m.SideEffect(m.Fn(Memcpy(), MemIndexPtr(0, Registers.es, di), MemIndexPtr(1, Registers.ds, si), size));
            m.Assign(regCx, m.Const(regCx.DataType, 0));
            m.Assign(si, m.IAdd(si, MaybeSlice(regCx.DataType, size)));
            m.Assign(di, m.IAdd(di, MaybeSlice(regCx.DataType, size)));
            return true;
        }

        private Expression MakeSizeExpression(Identifier regCx)
        {
            var dt = regCx.DataType;
            if (dt.BitSize == 16)
            {
                dt = PrimitiveType.UInt32;
            }
            return m.UMul(dt, regCx, m.Const(regCx.DataType, instrCur.DataWidth.Size));
        }

        private bool RewriteCmpsToToFindFirstDifference()
        {
            var regCx = RegCx;
            var result = binder.CreateTemporary(
                "cmpResult", instrCur.AddressWidth);
            var firstDifference = binder.CreateTemporary(
                "firstDifference", instrCur.AddressWidth);
            var size = MakeSizeExpression(regCx);
            var si = RegSi;
            var di = RegDi;
            var cx = RegCx;
            m.Assign(result, m.Fn(Memcmp(), MemIndexPtr(0, Registers.ds, si), MemIndexPtr(1, Registers.es, di), size));
            m.Assign(firstDifference, m.Fn(FindFirstDifference(), MemIndexPtr(0, Registers.ds, si), MemIndexPtr(1, Registers.es, di)));
            m.Assign(cx, m.ISub(cx, firstDifference));
            m.Assign(si, m.IAdd(si, firstDifference));
            m.Assign(di, m.IAdd(di, firstDifference));
            var grf = binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!);
            m.Assign(
                grf,
                m.Cond(grf.DataType, result));
            return true;
        }

        private bool IsRepNe()
        {
            return instrCur.RepPrefix == 2;
        }

        private bool IsRepE()
        {
            return instrCur.RepPrefix == 3;
        }

        private bool IsRep()
        {
            return instrCur.RepPrefix == 3;
        }

        private bool IsAutoIncrementingMode()
        {
            var direction = state.GetFlagGroup((uint) FlagM.DF);
            if (!direction.IsValid)
                return true;
            return !direction.ToBoolean();
        }

        private IntrinsicProcedure FindFirstDifference()
        {
            return findFirstDifference.ResolvePointers(arch.PointerType.BitSize);
        }

        private IntrinsicProcedure Strlen()
        {
            return CommonOps.Strlen.ResolvePointers(arch.PointerType.BitSize);
        }

        private IntrinsicProcedure Memcpy()
        {
            return CommonOps.Memcpy.ResolvePointers(arch.PointerType.BitSize);
        }

        private IntrinsicProcedure Memcmp()
        {
            return CommonOps.Memcmp.ResolvePointers(arch.PointerType.BitSize);
        }
    }
}
