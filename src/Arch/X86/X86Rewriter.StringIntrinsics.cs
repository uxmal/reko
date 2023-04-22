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
        private static readonly IntrinsicProcedure findFirstChange = new IntrinsicBuilder("__find_first_change__", false)
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
                    return RewriteScasbIntrinsic();
                break;
            case Mnemonic.movs:
            case Mnemonic.movsb:
                // rep movs
                if (IsRep())
                    return RewriteMovsIntrinsic();
                break;
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
                // repe cmps
                if (IsRepE())
                    return RewriteCmpsIntrinsic();
                break;
            }
            return false;
        }

        private bool RewriteScasbIntrinsic()
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

        private bool RewriteMovsIntrinsic()
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

        private bool RewriteCmpsIntrinsic()
        {
            var regCx = orw.AluRegister(Registers.rcx, instrCur.addrWidth);
            var result = binder.CreateTemporary(
                "cmpResult", instrCur.addrWidth);
            var firstChange = binder.CreateTemporary(
                "firstChange", instrCur.addrWidth);
            var size = m.SMul(regCx, instrCur.dataWidth.Size);
            m.Assign(result, m.Fn(Memcmp(), RegSi, RegDi, size));
            m.Assign(firstChange, m.Fn(findFirstChange, RegSi, RegDi));
            m.Assign(regCx, m.ISub(regCx, firstChange));
            m.Assign(RegSi, m.IAdd(RegSi, firstChange));
            m.Assign(RegDi, m.IAdd(RegDi, firstChange));
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

        private IntrinsicProcedure Strlen()
        {
            return CommonOps.Strlen;
        }

        private IntrinsicProcedure Memcpy()
        {
            return CommonOps.Memcpy;
        }

        private IntrinsicProcedure Memcmp()
        {
            return CommonOps.Memcmp;
        }
    }
}
