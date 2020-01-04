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

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    public class PIC16State: PICProcessorState
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arch">The PIC16 target architecture.</param>
        public PIC16State(PICArchitecture pArch) 
            : base(pArch)
        {
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="st">The PIC16 state to copy.</param>
        public PIC16State(PIC16State st) : base(st)
        {
        }

        /// <summary>
        /// Makes a deep copy of this <see cref="PIC16State"/> instance.
        /// </summary>
        public override ProcessorState Clone() => new PIC16State(this);

        /// <summary>
        /// Sets the instruction pointer (PC - Program Counter).
        /// </summary>
        /// <param name="addr">The address to assign to the PC.</param>
        public override void SetInstructionPointer(Address addr)
        {
            uint off = addr.ToUInt32();
            SetRegister(PICRegisters.PCL, Constant.Byte((byte)(off)));
            SetRegister(PICRegisters.PCLATH, Constant.Byte((byte)(off>>8)));
        }

    }

}
