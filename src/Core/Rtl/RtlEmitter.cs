#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Builder class that accumulates RtlInstructions.
    /// </summary>
    public class RtlEmitter : ExpressionEmitter
    {
        public List<RtlInstruction> Instructions { get; set; }

        public RtlEmitter(List<RtlInstruction> instrs)
        {
            this.Instructions = instrs;
        }

        public RtlEmitter Assign(Expression dst, Expression src)
        {
            var ass = new RtlAssignment(dst, src);
            Instructions.Add(ass);
            return this;
        }

        public RtlEmitter Assign(Expression dst, int src)
        {
            var ass = new RtlAssignment(dst, Constant.Create(dst.DataType, src));
            Instructions.Add(ass);
            return this;
        }

        public RtlEmitter Branch(Expression condition, Address target, RtlClass rtlClass)
        {
            Instructions.Add(new RtlBranch(condition, target, rtlClass));
            return this;
        }

        /// <summary>
        /// Called when we need to generate an RtlBranch in the middle of an operation.
        /// Normally, branches are at the end of the Rtl's of a translated instruction,
        /// but in some cases, they are not.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="target"></param>
        /// <param name="?"></param>
        public RtlBranch BranchInMiddleOfInstruction(Expression condition, Address target, RtlClass rtlClass)
        {
            var branch = new RtlBranch(condition, target, rtlClass);
            branch.NextStatementRequiresLabel = true;
            Instructions.Add(branch);
            return branch;
        }

        /// <summary>
        /// Called to generate a RtlCall instruction. The <paramref name="retSize"/> is the
        /// size of the return value as placed on the stack. It will be 0 on machines
        /// which use link registers to store the return address at a function call.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="retSize"></param>
        /// <param name="rtlClass"></param>
        public RtlEmitter Call(Expression target, byte retSize)
        {
            Instructions.Add(new RtlCall(target, retSize, RtlClass.Transfer | RtlClass.Call));
            return this;
        }

        public RtlEmitter CallD(Expression target, byte retSize)
        {
            Instructions.Add(new RtlCall(target, retSize, RtlClass.Transfer | RtlClass.Call | RtlClass.Delay));
            return this;
        }

        public void Emit(RtlInstruction instr)
        {
            Instructions.Add(instr); 
        }

        /// <summary>
        /// Standard goto, for architectures where there are no delay slots.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public RtlEmitter Goto(Expression target)
        {
            Instructions.Add(new RtlGoto(target, RtlClass.Transfer));
            return this;
        }

        public RtlEmitter Goto(Expression target, RtlClass rtlClass)
        {
            Instructions.Add(new RtlGoto(target, rtlClass));
            return this;
        }

        /// <summary>
        /// Delayed goto (for RISC architectures with delay slots)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public RtlEmitter GotoD(Expression target)
        {
            Instructions.Add(new RtlGoto(target, RtlClass.Transfer|RtlClass.Delay));
            return this;
        }

        public RtlEmitter Invalid()
        {
            Instructions.Add(new RtlInvalid());
            return this;
        }

        public RtlEmitter Nop()
        {
            Instructions.Add(new RtlNop { Class = RtlClass.Linear });
            return this;
        }

        /// <summary>
        /// Return to caller.
        /// </summary>
        /// <param name="returnAddressBytes">How large the return address is on the stack.</param>
        /// <param name="extraBytesPopped">Some architectures (like x86) pop the caller's extra
        /// bytes off the stack.</param>
        /// <returns></returns>
        public RtlEmitter Return(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            Instructions.Add(new RtlReturn(returnAddressBytes, extraBytesPopped, RtlClass.Transfer));
            return this;
        }

        public RtlEmitter ReturnD(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            var ret = new RtlReturn(returnAddressBytes, extraBytesPopped, RtlClass.Transfer | RtlClass.Delay);
            Instructions.Add(ret);
            return this;
        }

        public RtlEmitter SideEffect(Expression sideEffect, RtlClass rtlc = RtlClass.Linear)
        {
            var se = new RtlSideEffect(sideEffect);
            se.Class = rtlc;
            Instructions.Add(se);
            return this;
        }

        public Expression Const(DataType dataType, int p)
        {
            return Constant.Create(dataType, p);
        }

        [Obsolete("RtlIf is going away soon", false)]
        public RtlEmitter If(Expression test, RtlInstruction rtl)
        {
            Instructions.Add(new RtlIf(test, rtl));
            return this;
        }
    }
}
