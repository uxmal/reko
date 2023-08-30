#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Types;

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
            var dst = SrcOp(0);
            var src = SrcOp(1);
            m.Assign(dst, m.Fn(aesimc_intrinsic, src));
        }

        public void RewriteClts()
        {
            var cr0 = binder.EnsureRegister(arch.GetControlRegister(0)!);
            m.Assign(cr0, m.Fn(clts_intrinsic, cr0));
        }

        private void RewriteCacheLine(IntrinsicProcedure intrinsic)
        {
            var mem = SrcOp(0);
            m.SideEffect(m.Fn(intrinsic, m.AddrOf(arch.PointerType, mem)));

        }
        public void RewriteEmms()
        {
            m.SideEffect(m.Fn(emms_intrinsic));
        }

        private void RewriteGetsec()
        {
            //$TODO: this is not correct; actual function
            // depends on EAX.
            var arg = binder.EnsureRegister(Registers.eax);
            var result = binder.EnsureSequence(PrimitiveType.Word64, Registers.edx, Registers.ebx);
            m.Assign(result, m.Fn(getsec_intrinsic, arg));
        }

        private void RewriteInvd()
        {
            m.SideEffect(m.Fn(invd_intrinsic));
        }

        private void RewriteInvlpg()
        {
            var op = SrcOp(0);
            m.SideEffect(m.Fn(invldpg_intrinsic.MakeInstance(arch.PointerType), op));
        }

        private void RewriteLar()
        {
            m.Assign(
                SrcOp(0),
                m.Fn(
                    lar_intrinsic.MakeInstance(arch.PointerType.BitSize, instrCur.Operands[0].Width),
                    m.AddrOf(arch.PointerType, SrcOp(1))));
            m.Assign(
                binder.EnsureFlagGroup(Registers.Z),
                Constant.True());
        }

        private void RewriteLmsw()
        {
            m.SideEffect(m.Fn(lmsw_intrinsic, SrcOp(0)));
        }

        private void RewriteLsl()
        {
            m.Assign(
                SrcOp(0),
                m.Fn(
                    lsl_intrinsic.MakeInstance(instrCur.Operands[0].Width),
                    SrcOp(1)));
        }

        private void RewriteLxdt(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic, SrcOp(0)));
        }

        private void RewriteSxdt(IntrinsicProcedure intrinsic)
        {
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(dst.DataType)));
        }

        public void RewriteLfence()
        {
            m.SideEffect(m.Fn(lfence_intrinsic));
        }

        public void RewriteMfence()
        {
            m.SideEffect(m.Fn(mfence_intrinsic));
        }

        private void RewriteMovdq()
        {
            var src = SrcOp(1);
            var opDst = instrCur.Operands[0];
            int dbitSize = opDst.Width.BitSize - src.DataType.BitSize;
            if (dbitSize > 0)
            {
                // Zero-extend.
                src = m.Convert(src, src.DataType, opDst.Width);
            }
            else if (dbitSize < 0)
            {
                src = m.Slice(src, opDst.Width, 0);
            }
            EmitCopy(0, src);
        }

        public void RewritePause()
        {
            m.SideEffect(m.Fn(pause_intrinsic));
        }

        public void RewritePrefetch(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic.MakeInstance(arch.PointerType), SrcOp(0)));
        }

        private void RewriteRdrand()
        {
            var arg = SrcOp(0);
            var ret = binder.EnsureFlagGroup(Registers.C);
            m.Assign(ret, m.Fn(rdrand_intrinsic, m.Out(arg.DataType, arg)));
            m.Assign(binder.EnsureFlagGroup(Registers.S), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.O), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.P), 0);
        }

        private void RewriteSmsw()
        {
            var dst = SrcOp(0);
            m.Assign(dst, m.Fn(smsw_intrinsic.MakeInstance(dst.DataType)));
        }

        public void RewriteSfence()
        {
            m.SideEffect(m.Fn(sfence_intrinsic));
        }

        private void RewriteVmptrld()
        {
            m.SideEffect(m.Fn(vmptrld_intrinsic, SrcOp(0)));
        }

        private void RewriteVmxon()
        {
            m.SideEffect(m.Fn(vmxon_intrinsic, m.AddrOf(PrimitiveType.Ptr64, SrcOp(0))));
        }

        private void RewriteVmread()
        {
            var src1 = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(
                dst,
                m.Fn(vmread_intrinsic.MakeInstance(src1.DataType, dst.DataType), src1));
        }

        private void RewriteVmwrite()
        {
            var src0 = SrcOp(0);
            var src1 = SrcOp(1);
            m.SideEffect(
                m.Fn(vmwrite_intrinsic.MakeInstance(src0.DataType, src1.DataType),
                src0,
                src1));
        }

        private void RewriteWbinvd()
        {
            m.SideEffect(m.Fn(wbinvd_intrinsic));
        }

        private void RewriteWrpkru()
        {
            var eax = binder.EnsureRegister(Registers.eax);
            m.SideEffect(m.Fn(wrpkru_intrinsic, eax));

        }

        public void RewriteWrsmr()
        {
            var edx_eax = binder.EnsureSequence(PrimitiveType.Word64, Registers.edx, Registers.eax);
            var ecx = binder.EnsureRegister(Registers.ecx);
            m.SideEffect(m.Fn(wrmsr_intrinsic, ecx, edx_eax));
        }

        private void RewriteXsaveopt()
        {
            var edx_eax = binder.EnsureSequence(PrimitiveType.Word64, Registers.edx, Registers.eax);
            m.SideEffect(m.Fn(xsaveopt_intrinsic, edx_eax, m.AddrOf(arch.PointerType, SrcOp(0))));
        }
    }
}
