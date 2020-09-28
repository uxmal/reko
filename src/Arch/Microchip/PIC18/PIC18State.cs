#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core;
using Reko.Core.Expressions;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// The state of a PIC18 processor. Used in the Scanning phase of the decompiler.
    /// </summary>
    public class PIC18State : PICProcessorState
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arch">The PIC18 target architecture.</param>
        public PIC18State(PICArchitecture arch)
            : base(arch)
        {
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="st">The PIC18 state to copy.</param>
        public PIC18State(PIC18State st) : base(st)
        {
        }

        /// <summary>
        /// Makes a deep copy of this <see cref="PIC18State"/> instance.
        /// </summary>
        public override ProcessorState Clone() => new PIC18State(this);

        /// <summary>
        /// Sets the instruction pointer (PC - Program Counter).
        /// </summary>
        /// <param name="addr">The address to assign to the PC.</param>
        public override void SetInstructionPointer(Address addr)
        {
            uint off = addr.ToUInt32();
            SetRegister(PIC18Registers.PCLAT, Constant.Word32(off));
        }

    }

}
