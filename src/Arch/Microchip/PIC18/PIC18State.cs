#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// The state of an PIC18 processor. Used in the Scanning phase of the decompiler.
    /// </summary>
    public class PIC18State : ProcessorState
    {
        //TODO: use 'access' mask and 'impl' mask of PIC registers to amend GetRegister/SetRegister results.
        //TODO: see stack evolution (push, pop, under/overflow)

        #region Locals

        private byte[] regs;              // register values - all are 8-bit wide.
        private bool[] isValid;           // validity of registers values
        private PIC18Architecture arch;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arch">The PIC18 target architecture.</param>
        public PIC18State(PIC18Architecture arch)
        {
            this.arch = arch;
            regs = new byte[PIC18Registers.Max];
            isValid = new bool[PIC18Registers.Max];
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="st">The PIC18 state to copy.</param>
        public PIC18State(PIC18State st) : base(st)
        {
            arch = st.arch;
            regs = (byte[])st.regs.Clone();
            isValid = (bool[])st.isValid.Clone();
        }

        #endregion

        #region ProcessorState interface

        public override IProcessorArchitecture Architecture => arch;

        public override ProcessorState Clone() => new PIC18State(this);

        public bool IsValid(RegisterStorage reg) => isValid[reg.Number];

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (reg != null && isValid[reg.Number])
                return Constant.Create(reg.DataType, regs[reg.Number]);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage reg, Constant c)
        {
            if (reg != null && c != null && c.IsValid)
            {
                isValid[reg.Number] = true;
                regs[reg.Number] = c.ToByte();
            }
            else
            {
                isValid[reg.Number] = false;
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            ulong off = addr.ToUInt32();
            SetRegister(PIC18Registers.PCL, Constant.Byte((byte)(off & 0xFF)));
            SetRegister(PIC18Registers.PCLATH, Constant.Byte((byte)((off >> 8) & 0xFF)));
            SetRegister(PIC18Registers.PCLATU, Constant.Byte((byte)((off >> 16) & 0xFF)));
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
            return new CallSite(returnAddressSize, 0);
        }

        /// <summary>
        /// Perform any adjustments to the processor's state after returning from a procedure call with
        /// the specified signature.
        /// </summary>
        /// <param name="sigCallee">The signature of the called procedure.</param>
        public override void OnAfterCall(FunctionType sigCallee)
        {
        }

        #endregion

    }

}
