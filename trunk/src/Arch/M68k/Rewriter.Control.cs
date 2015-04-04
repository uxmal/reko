#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public partial class Rewriter
    {
        private void RewriteBcc(ConditionCode cc, FlagM flags)
        {
            emitter.Branch(
                emitter.Test(cc, orw.FlagGroup(flags)),
                ((M68kAddressOperand) di.op1).Address,
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBra()
        {
            emitter.Goto(orw.RewriteSrc(di.op1, di.Address, true));
        }

        private void RewriteBsr()
        {
            emitter.Call(orw.RewriteSrc(di.op1, di.Address, true), 4);
        }

        private void RewriteJmp()
        {
            emitter.Goto(orw.RewriteSrc(di.op1, di.Address, true));
        }

        private void RewriteJsr()
        {
            var src = orw.RewriteSrc(di.op1, di.Address, true);
            var mem = src as MemoryAccess;
            if (mem != null)
                src = mem.EffectiveAddress;
            emitter.Call(src, 4);
        }

        private void RewriteDbcc(ConditionCode cc, FlagM flags)
        {
            if (cc != ConditionCode.None)
            {
                emitter.Branch(
                    emitter.Test(cc, orw.FlagGroup(flags)),
                    di.Address + 4,
                    RtlClass.ConditionalTransfer);
            }
            var src = orw.RewriteSrc(di.op1, di.Address);

            emitter.Assign(src, emitter.ISub(src, 1));
            emitter.Branch(
                emitter.Ne(src, emitter.Int32(-1)),
                (Address) orw.RewriteSrc(di.op2, di.Address, true),
                RtlClass.ConditionalTransfer);
        }

        private bool RewriteIllegal()
        {
            if (dasm.Current.op1 == null)
                return false;
            emitter.SideEffect(PseudoProc("__syscall", VoidType.Instance,  RewriteSrcOperand(dasm.Current.op1)));
            return true;
        }
    }
}
