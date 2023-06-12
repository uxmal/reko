#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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

namespace Reko.Arch.X86
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
            //$TODO: supported segmented mode.
            if (arch.ProcessorMode.PointerType == PrimitiveType.SegPtr32)
                return false;
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
            var cEax = state.GetRegister(Registers.eax);
            if (!cEax.IsZero)
                return false;
            var cEcx = state.GetRegister(Registers.ecx);
            if (!cEcx.IsMaxUnsigned)
                return false;
            var size = binder.CreateTemporary("size", instrCur.addrWidth);
            m.Assign(size, m.IAddS(m.Fn(Strlen(), RegDi), 1));
            var regCx = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
            m.Assign(regCx, m.ISub(regCx, size));
            m.Assign(RegDi, m.IAdd(RegDi, size));
            m.Assign(
                binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                m.Cond(m.Const(instrCur.dataWidth, 0)));
            return true;
        }

        private bool RewriteMovsToMemcpy()
        {
            var regCx = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
            var size = binder.CreateTemporary("size", instrCur.addrWidth);
            m.Assign(size, m.SMul(regCx, instrCur.dataWidth.Size));
            m.SideEffect(m.Fn(Memcpy(), RegDi, RegSi, size));
            m.Assign(regCx, m.Const(regCx.DataType, 0));
            m.Assign(RegSi, m.IAdd(RegSi, size));
            m.Assign(RegDi, m.IAdd(RegDi, size));
            return true;
        }

        private bool RewriteCmpsToToFindFirstDifference()
        {
            var regCx = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
            var result = binder.CreateTemporary(
                "cmpResult", instrCur.addrWidth);
            var firstDifference = binder.CreateTemporary(
                "firstDifference", instrCur.addrWidth);
            var size = m.SMul(regCx, instrCur.dataWidth.Size);
            m.Assign(result, m.Fn(Memcmp(), RegSi, RegDi, size));
            m.Assign(firstDifference, m.Fn(FindFirstDifference(), RegSi, RegDi));
            m.Assign(regCx, m.ISub(regCx, firstDifference));
            m.Assign(RegSi, m.IAdd(RegSi, firstDifference));
            m.Assign(RegDi, m.IAdd(RegDi, firstDifference));
            m.Assign(
                binder.EnsureFlagGroup(X86Instruction.DefCc(Mnemonic.cmp)!),
                m.Cond(result));
            return true;
        }

        private bool IsRepNe()
        {
            return instrCur.repPrefix == 2;
        }

        private bool IsRepE()
        {
            return instrCur.repPrefix == 3;
        }

        private bool IsRep()
        {
            return instrCur.repPrefix == 3;
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
