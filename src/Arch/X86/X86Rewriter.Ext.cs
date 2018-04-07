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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Rewriter support for "extended" instructions of the x86 architecture.
    /// Basically, anything SSE or post-Pentium goes here.
    /// </summary>
    public partial class X86Rewriter
    {
        public void RewriteAesimc()
        {
            var dst = SrcOp(instrCur.op1);
            var src = SrcOp(instrCur.op2);
            m.Assign(dst, host.PseudoProcedure("__aesimc", dst.DataType, src));
        }

        public void RewriteClts()
        {
            rtlc = RtlClass.System;
            var cr0 = binder.EnsureRegister(arch.GetControlRegister(0));
            m.Assign(cr0, host.PseudoProcedure("__clts", cr0.DataType, cr0));
        }

        public void RewriteEmms()
        {
            rtlc = RtlClass.System;
            m.SideEffect(host.PseudoProcedure("__emms", VoidType.Instance));
        }

        private void RewriteInvd()
        {
            rtlc = RtlClass.System;
            m.SideEffect(host.PseudoProcedure("__invd", VoidType.Instance));
        }

        private void RewriteLar()
        {
            rtlc = RtlClass.System;
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__lar",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op2)));
            m.Assign(
                orw.FlagGroup(FlagM.ZF),
                Constant.True());
        }

        private void RewriteLsl()
        {
            rtlc = RtlClass.System;
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__lsl",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op2)));
        }

        public void RewriteLfence()
        {
            m.SideEffect(host.PseudoProcedure("__lfence", VoidType.Instance));
        }

        public void RewriteMfence()
        {
            m.SideEffect(host.PseudoProcedure("__mfence", VoidType.Instance));
        }

        public void RewritePause()
        {
            m.SideEffect(host.PseudoProcedure("__pause", VoidType.Instance));
        }

        public void RewritePrefetch(string name)
        {
            m.SideEffect(host.PseudoProcedure(name, VoidType.Instance, SrcOp(instrCur.op1)));
        }


        public void RewriteSfence()
        {
            m.SideEffect(host.PseudoProcedure("__sfence", VoidType.Instance));
        }

        private void RewriteWbinvd()
        {
            rtlc = RtlClass.System;
            m.SideEffect(host.PseudoProcedure("__wbinvd", VoidType.Instance));
        }

        public void RewriteWrsmr()
        {
            rtlc = RtlClass.System;
            var edx_eax = binder.EnsureSequence(Registers.edx, Registers.eax, PrimitiveType.Word64);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.SideEffect(host.PseudoProcedure("__wrmsr", VoidType.Instance, ecx, edx_eax));
        }
    }
}
