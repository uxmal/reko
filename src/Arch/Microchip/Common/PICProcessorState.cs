#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;
using System;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// The state of a PIC processor. Used in the Scanning phase of the decompiler.
    /// </summary>
    public abstract class PICProcessorState : ProcessorState
    {

        protected PICArchitecture arch;
        protected Dictionary<PICRegisterStorage, uint> ValidRegsValues; // Registers values. Only for valid registers.
        protected HashSet<RegisterStorage> ValidRegs;                   // Validity of registers. Note: if not valid, there is no corresponding entry in 'regs'.

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arch">The PIC18 target architecture.</param>
        public PICProcessorState(PICArchitecture arch)
        {
            this.arch = arch;
            ValidRegsValues = new Dictionary<PICRegisterStorage, uint>();
            ValidRegs = new HashSet<RegisterStorage>();
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="st">The PIC18 state to copy.</param>
        public PICProcessorState(PICProcessorState st) : base(st)
        {
            arch = st.arch;
            HWStackItems = st.HWStackItems;
            ValidRegsValues = new Dictionary<PICRegisterStorage, uint>(st.ValidRegsValues);
            ValidRegs = new HashSet<RegisterStorage>(st.ValidRegs);
        }

        #region ProcessorState interface

        /// <summary>
        /// Gets the processor architecture.
        /// </summary>
        public override IProcessorArchitecture Architecture => arch;

        /// <summary>
        /// Query if  '<paramref name="reg"/>' is valid.
        /// </summary>
        /// <param name="reg">The PIC register.</param>
        /// <returns>
        /// True if valid, false if not.
        /// </returns>
        public bool IsValid(RegisterStorage reg)
            => ValidRegs.Contains(reg);

        //TODO: use 'access' mask of PIC registers to amend GetRegister/SetRegister results.

        /// <summary>
        /// Get the PIC register value.
        /// </summary>
        /// <param name="reg">The PIC register.</param>
        /// <returns>
        /// The register value as a constant or invalid..
        /// </returns>
        public override Constant GetRegister(RegisterStorage reg)
        {
            if ((reg is PICRegisterStorage preg) && IsValid(preg))
            {
                return Constant.Create(reg.DataType, ValidRegsValues[preg] & preg.Impl);
            }
            return Constant.Invalid;
        }

        /// <summary>
        /// Sets a PIC register value.
        /// </summary>
        /// <param name="reg">The PIC register.</param>
        /// <param name="c">A Constant to process.</param>
        public override void SetRegister(RegisterStorage reg, Constant c)
        {
            if (reg is PICRegisterStorage preg)
            {
                if (c?.IsValid ?? false)
                {
                    ValidRegs.Add(preg);
                    ValidRegsValues[preg] = (byte)(c.ToByte() & preg.Impl);
                    return;
                }
            }
            ValidRegs.Remove(reg);
        }

        /// <summary>
        /// Executes the procedure entered action.
        /// Some registers need to be updated when a procedure is entered.
        /// </summary>
        public override void OnProcedureEntered()
        {
        }

        /// <summary>
        /// Executes the procedure left action.
        /// </summary>
        /// <param name="procedureSignature">The procedure signature.</param>
        public override void OnProcedureLeft(FunctionType procedureSignature)
        {
        }

        /// <summary>
        /// Captures the processor's state before calling a procedure.
        /// </summary>
        /// <param name="sp">An identifier for the stack register of the processor.</param>
        /// <param name="returnAddressSize">The size of the return address on stack.</param>
        /// <returns>
        /// A CallSite object that abstracts the processor state right before the call.
        /// </returns>
        public override CallSite OnBeforeCall(Identifier sp, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, HWStackItems);
        }

        /// <summary>
        /// Perform any adjustments to the processor's state after returning from a procedure call with
        /// the specified signature.
        /// </summary>
        /// <param name="sigCallee">The signature of the called procedure.</param>
        public override void OnAfterCall(FunctionType sigCallee)
        {
            ShrinkHWStack(-1);
        }

        /// <summary>
        /// Grow the hardware stack.
        /// </summary>
        /// <param name="addrInstr">The address instr.</param>
        /// <exception cref="InvalidOperationException">Thrown if a stack overflow occurred.</exception>
        public void GrowHWStack(Address addrInstr)
        {
            ++HWStackItems;
            if (HWStackItems > PICRegisters.HWStackDepth)
                throw new InvalidOperationException($"PIC Hardware stack overflow at address {addrInstr}.");
        }

        /// <summary>
        /// Shrink the hardware stack.
        /// </summary>
        /// <param name="cItems">The items.</param>
        /// <exception cref="InvalidOperationException">Thrown if a stack underflow occurred.</exception>
        public void ShrinkHWStack(int cItems)
        {
            HWStackItems--;
            if (HWStackItems < 0)
                throw new InvalidOperationException("PIC Hardware stack underflow.");
        }

        #endregion

        public int HWStackItems { get; set; }

    }

}
