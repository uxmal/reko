#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter
    {
        private void RewriteBicpsw()
        {
            var mask = RewriteSrcOp(0, PrimitiveType.UInt16);
            var psw = binder.EnsureRegister(Registers.psw);
            m.Assign(psw, m.And(psw, m.Comp(mask)));
        }

        private void RewriteBispsw()
        {
            var mask = RewriteSrcOp(0, PrimitiveType.UInt16);
            var psw = binder.EnsureRegister(Registers.psw);
            m.Assign(psw, m.Or(psw, mask));
        }

        private void RewriteHalt()
        {
            m.SideEffect(host.Intrinsic("__halt", true, VoidType.Instance));
        }

        private void RewriteInsque()
        {
            var entry = RewriteSrcOp(0, PrimitiveType.Word32);
            var queue = RewriteSrcOp(1, PrimitiveType.Word32);
            var grf = FlagGroup(Registers.CZN);
            m.Assign(grf, host.Intrinsic("__insque", true, grf.DataType, queue, entry));
            m.Assign(FlagGroup(Registers.V), Constant.False());
        }

        private void RewriteBpt()
        {
            m.SideEffect(host.Intrinsic("vax_bpt", true, VoidType.Instance));
        }

        private void RewriteChm(string name)
        {
            m.SideEffect(host.Intrinsic(name, true, VoidType.Instance,
                RewriteSrcOp(0, PrimitiveType.Word16)));
        }

        private void RewriteCmpc3()
        {
            var len = RewriteSrcOp(0, PrimitiveType.Word16);
            var str1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var str2 = RewriteSrcOp(2, PrimitiveType.Ptr32);
            var addrCur = instr.Address;
            var r0 = binder.EnsureRegister(Registers.r0);
            var r1 = binder.EnsureRegister(Registers.r1);
            var r2 = binder.EnsureRegister(Registers.r2);
            var r3 = binder.EnsureRegister(Registers.r3);
            var addrNext = addrCur + instr.Length;

            m.Assign(r0, len);
            m.Assign(r1, str1);
            m.Assign(r2, str2);
            //$TODO: emit clusters.
        }

        private void RewriteMovp()
        {
            var len = RewriteSrcOp(0, PrimitiveType.Word16);
            var src = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var dst = RewriteSrcOp(2, PrimitiveType.Ptr32);
            m.SideEffect(host.Intrinsic(
                "__movp", 
                true,
                VoidType.Instance,
                dst, src, len));
        }

        private void RewriteProber()
        {
            var mode = RewriteSrcOp(0, PrimitiveType.Word16);
            var len = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var @base = RewriteSrcOp(2, PrimitiveType.Ptr32);
            var z = FlagGroup(Registers.Z);
            m.Assign(z, host.Intrinsic(
                "__prober",
                true,
                PrimitiveType.Bool,
                @base, len, mode));
        }

        private void RewriteScanc()
        {
            var len = RewriteSrcOp(0, PrimitiveType.Word16);
            var addr = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var tbl = RewriteSrcOp(2, PrimitiveType.Ptr32);
            var mask = RewriteSrcOp(3, PrimitiveType.Byte);
            var r0 = binder.EnsureRegister(Registers.r0);
            var r1 = binder.EnsureRegister(Registers.r1);
            var r2 = binder.EnsureRegister(Registers.r2);
            var r3 = binder.EnsureRegister(Registers.r3);
            var z = FlagGroup(Registers.Z);
            m.Assign(r3, tbl);
            m.Assign(z, host.Intrinsic("__scanc", true, z.DataType, len, addr, tbl, mask,
                m.Out(PrimitiveType.Word32, r0),
                m.Out(PrimitiveType.Word32, r1)));
            m.Assign(r2, 0);
            m.Assign(FlagGroup(Registers.CVN), 0);
        }
    }
}
