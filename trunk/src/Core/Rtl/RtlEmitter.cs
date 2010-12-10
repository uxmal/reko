#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Rtl
{
    public class RtlEmitter : ExpressionEmitter
    {
        private List<RtlInstruction> instrs;
        private Address addr;
        private byte length;

        public RtlEmitter(Address addr, uint length, List<RtlInstruction> instrs)
        {
            this.addr= addr;
            this.length = (byte) length;
            this.instrs = instrs;
        }

        public RtlAssignment Assign(Expression dst, Expression src)
        {
            var ass = new RtlAssignment(addr, length, dst, src);
            instrs.Add(ass);
            return ass;
        }

        public void Branch(Expression condition, Address target)
        {
            instrs.Add(new RtlBranch(addr, length, condition, target));
        }

        /// <summary>
        /// Called when we need to generate an RtlBranch in the middle of an operation. Normally 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="target"></param>
        /// <param name="?"></param>
        public void BranchInMiddleOfInstruction(Expression condition, Address target)
        {
            var branch = new RtlBranch(addr, length, condition, target);
            branch.NextStatementRequiresLabel = true;
            instrs.Add(branch);
        }

        public void Call(Expression target)
        {
            instrs.Add(new RtlCall(addr, length, target));
        }

        public void Goto(Expression target)
        {
            instrs.Add(new RtlGoto(addr, length, target));
        }

        public void Return(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            instrs.Add(new RtlReturn(addr, length, returnAddressBytes, extraBytesPopped));
        }

        public void SideEffect(Expression sideEffect)
        {
            instrs.Add(new RtlSideEffect(addr, length, sideEffect));
        }
    }
}
