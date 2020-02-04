#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        public RtlEmitter(List<RtlInstruction> instrs)
        {
            this.Instructions = instrs;
        }

        public List<RtlInstruction> Instructions { get; set; }


        /// <summary>
        /// Generates a RtlAssignment ('dst = src' in the C language family).
        /// </summary>
        /// <param name="dst">Destination</param>
        /// <param name="src">Source</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Assign(Expression dst, Expression src)
        {
            var ass = new RtlAssignment(dst, src);
            Instructions.Add(ass);
            return this;
        }

        /// <summary>
        /// Convenience method to generate a RtlAssignment ('dst = const' in the C language family).
        /// The source is converted to a Constant.
        /// </summary>
        /// <param name="dst">Destination</param>
        /// <param name="src">Source</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Assign(Expression dst, int src)
        {
            var ass = new RtlAssignment(dst, Constant.Create(dst.DataType, src));
            Instructions.Add(ass);
            return this;
        }

        /// <summary>
        /// Generates a RtlBranch instruction which jumps to the address <paramref name="target"/>
        /// if the boolean expression <paramref name="condition" /> is true. The <paramref name="rtlClass" />
        /// can be used to indicate if there is a delay slot after this RTL instruction.
        /// </summary>
        /// <param name="condition">Boolean expression</param>
        /// <param name="target">Control goes to this address if condition is true</param>
        /// <param name="rtlClass">Describes details the branch instruction</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Branch(Expression condition, Address target, InstrClass rtlClass)
        {
            Instructions.Add(new RtlBranch(condition, target, rtlClass));
            return this;
        }

        /// <summary>
        /// Generates a non-delayed RtlBranch instruction which jumps to the address <paramref name="target"/>
        /// if the boolean expression <paramref name="condition" /> is true. 
        /// </summary>
        /// <param name="condition">Boolean expression</param>
        /// <param name="target">Control goes to this address if condition is true</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Branch(Expression condition, Address target)
        {
            Instructions.Add(new RtlBranch(condition, target, InstrClass.ConditionalTransfer));
            return this;
        }

        /// <summary>
        /// Called when we need to generate an RtlBranch in the middle of an operation.
        /// Normally, branches are at the end of the Rtl's of a translated instruction,
        /// but in some cases, they are not.
        /// </summary>
        /// <param name="condition">Boolean expression</param>
        /// <param name="target">Control goes to this address if condition is true</param>
        /// <param name="rtlClass">Describes details the branch instruction</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlBranch BranchInMiddleOfInstruction(Expression condition, Address target, InstrClass rtlClass)
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
        /// <param name="target">Destination of the call.</param>
        /// <param name="retSize">Size in bytes of return address on stack</param>
        /// <param name="iclass">The <see cref="InstrClass" /> of the call.</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Call(Expression target, byte retSize, InstrClass iclass)
        {
            var requiredBits = InstrClass.Transfer | InstrClass.Call;
            if ((iclass & requiredBits) != requiredBits)
                throw new ArgumentException(nameof(iclass), "InstrClass bits must be consistent with a call.");
            Instructions.Add(new RtlCall(target, retSize, iclass));
            return this;
        }

        /// <summary>
        /// Called to generate a RtlCall instruction with no delay slot. The
        /// <paramref name="retSize"/> is the size of the return value as placed on
        /// the stack. It will be 0 on machines which use link registers to store 
        /// the return address at a function call.
        /// </summary>
        /// <param name="target">Destination of the call.</param>
        /// <param name="retSize">Size in bytes of return address on stack</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Call(Expression target, byte retSize)
        {
            Instructions.Add(new RtlCall(target, retSize, InstrClass.Transfer | InstrClass.Call));
            return this;
        }

        /// <summary>
        /// Called to generate a RtlCall instruction with a delay slot. The
        /// <paramref name="retSize"/> is the size of the return value as placed on
        /// the stack. It will be 0 on machines which use link registers to store
        /// the return address at a function call.
        /// </summary>
        /// <param name="target">Destination of the call.</param>
        /// <param name="retSize">Size in bytes of return address on stack</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter CallD(Expression target, byte retSize)
        {
            Instructions.Add(new RtlCall(target, retSize, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay));
            return this;
        }

        /// <summary>
        /// Called to generate a RtlCall instruction and switch to the given processor 
        /// architecture <paramref name="arch"/>. The <paramref name="retSize"/> is the
        /// size of the return value as placed on the stack. It will be 0 on machines
        /// which use link registers to store the return address at a function call. 
        /// </summary>
        /// <param name="target">Destination of the call.</param>
        /// <param name="retSize">Size in bytes of return address on stack</param>
        /// <param name="arch">The processor architecture to switch to.</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter CallX(Expression target, byte retSize, IProcessorArchitecture arch)
        {
            Instructions.Add(new RtlCall(target, retSize, InstrClass.Transfer | InstrClass.Call, arch));
            return this;
        }

        /// <summary>
        /// Called to generate a RtlCall instruction (with a delay slot) and switch to the given processor 
        /// architecture <paramref name="arch"/>. The <paramref name="retSize"/> is the
        /// size of the return value as placed on the stack. It will be 0 on machines
        /// which use link registers to store the return address at a function call. 
        /// </summary>
        /// <param name="target">Destination of the call.</param>
        /// <param name="retSize">Size in bytes of return address on stack</param>
        /// <param name="arch">The processor architecture to switch to.</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter CallXD(Expression target, byte retSize, IProcessorArchitecture arch)
        {
            Instructions.Add(new RtlCall(target, retSize, InstrClass.Transfer | InstrClass.Call | InstrClass.Delay, arch));
            return this;
        }


        /// <summary>
        /// Emit the RTL instruction <paramref name="instr"/> to the RTL 
        /// instruction stream.
        /// </summary>
        /// <param name="instr"></param>
        public void Emit(RtlInstruction instr)
        {
            Instructions.Add(instr); 
        }

        /// <summary>
        /// Standard goto, for architectures where there are no delay slots.
        /// </summary>
        /// <param name="target">Destination of the goto</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Goto(Expression target)
        {
            Instructions.Add(new RtlGoto(target, InstrClass.Transfer));
            return this;
        }

        /// <summary>
        /// Generates an RtlGoto instruction.
        /// </summary>
        /// <param name="target">Destination of the goto</param>
        /// <param name="rtlClass">Details of the goto instruction</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Goto(Expression target, InstrClass rtlClass)
        {
            Instructions.Add(new RtlGoto(target, rtlClass));
            return this;
        }

        /// <summary>
        /// Delayed goto (for RISC architectures with delay slots)
        /// </summary>
        /// <param name="target">Destination of the goto</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter GotoD(Expression target)
        {
            Instructions.Add(new RtlGoto(target, InstrClass.Transfer|InstrClass.Delay));
            return this;
        }

        /// <summary>
        /// Generates an invalid RTL instruction. Typically used when
        /// an instruction rewriter encounters invalid machine code.
        /// </summary>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Invalid()
        {
            Instructions.Add(new RtlInvalid());
            return this;
        }

        /// <summary>
        /// Generates a no-op instruction.
        /// </summary>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Nop()
        {
            Instructions.Add(new RtlNop { Class = InstrClass.Linear });
            return this;
        }

        /// <summary>
        /// Generates an RTL Return instruction with the specified instruction class
        /// </summary>
        /// <param name="returnAddressBytes">How large the return address is on the stack.</param>
        /// <param name="extraBytesPopped">Some architectures (like x86) pop the caller's extra
        /// bytes off the stack.</param>
        /// <param name="iclass">Instruction class of the created return statement</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Return(
            int returnAddressBytes,
            int extraBytesPopped,
            InstrClass iclass)
        {
            if ((iclass & InstrClass.Transfer) != InstrClass.Transfer)
                throw new ArgumentException(nameof(iclass), "Instruction class must be a transfer.");
            Instructions.Add(new RtlReturn(returnAddressBytes, extraBytesPopped, iclass));
            return this;
        }

        /// <summary>
        /// Generates an RTL Return instruction.
        /// </summary>
        /// <param name="returnAddressBytes">How large the return address is on the stack.</param>
        /// <param name="extraBytesPopped">Some architectures (like x86) pop the caller's extra
        /// bytes off the stack.</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter Return(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            Instructions.Add(new RtlReturn(returnAddressBytes, extraBytesPopped, InstrClass.Transfer));
            return this;
        }

        /// <summary>
        /// Generates an RTL Return instruction with a delay slot.
        /// </summary>
        /// <param name="returnAddressBytes">How large the return address is on the stack.</param>
        /// <param name="extraBytesPopped">Some architectures (like x86) pop the caller's extra
        /// bytes off the stack.</param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter ReturnD(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            var ret = new RtlReturn(returnAddressBytes, extraBytesPopped, InstrClass.Transfer | InstrClass.Delay);
            Instructions.Add(ret);
            return this;
        }

        /// <summary>
        /// Generates an RTL side effect instruction, typically for instructions which 
        /// are modeled by their effect on the processor that can't be seen in the processor
        /// registers or memory.
        /// </summary>
        /// <param name="sideEffect">Expression which when evaluated causes the side effect.</param>
        /// <param name="rtlc"></param>
        /// <returns>A reference to this RtlEmitter.</returns>
        public RtlEmitter SideEffect(Expression sideEffect, InstrClass rtlc = InstrClass.Linear)
        {
            var se = new RtlSideEffect(sideEffect);
            se.Class = rtlc;
            Instructions.Add(se);
            return this;
        }

        /// <summary>
        /// Generates a constant of type <paramref name="dataType"/> from the bit pattern
        /// <paramref name="p"/>.
        /// </summary>
        /// <param name="dataType">Data type of the constant.</param>
        /// <param name="p">Bit vector.</param>
        /// <returns>A Constant</returns>
        public Expression Const(DataType dataType, int p)
        {
            return Constant.Create(dataType, p);
        }

        [Obsolete("RtlIf is going away soon: don't use it.", false)]
        public RtlEmitter If(Expression test, RtlInstruction rtl)
        {
            Instructions.Add(new RtlIf(test, rtl));
            return this;
        }
    }
}
