#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using Decompiler.Core.Rtl;
using Decompiler.Core.Machine;
using System;

namespace Decompiler.Core
{
    /// <summary>
    /// ProcessorState simulates the state of the processor and a part of the stack during scanning.
    /// </summary>
	public abstract class ProcessorState
	{
        /// <summary>
        /// Method to call if an error occurs within the processor state object (such as stack over/underflows).
        /// </summary>
        public Action<string> ErrorListener { get; set; }
	    public ProcessorState Clone()
        {
            var clone = this.CloneInternal();
            clone.ErrorListener = this.ErrorListener;
            return clone;
        }
        protected abstract ProcessorState CloneInternal();

        public abstract Constant GetRegister(RegisterStorage r);
        public abstract void SetRegister(RegisterStorage r, Constant v);
        public abstract void SetInstructionPointer(Address addr);

        public abstract void OnProcedureEntered();                 // Some registers need to be updated when a procedure is entered.
        public abstract void OnProcedureLeft(ProcedureSignature procedureSignature);
        
        /// <summary>
        /// Captures the the processor's state before calling a procedure.
        /// </summary>
        /// <returns>A CallSite object that abstracts the processor state right before the call.</returns>
        public abstract CallSite OnBeforeCall(int returnAddressSize);
        /// <summary>
        /// Perform any adjustments to the processor's state after returning from a procedure call with the
        /// specified signature.
        /// </summary>
        /// <param name="sigCallee">The signature of the called procedure.</param>
        public abstract void OnAfterCall(ProcedureSignature sigCallee);

    }
}
