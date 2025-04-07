#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Loading;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Emulation
{
    /// <summary>
    /// A default implementation of the <see cref="IPlatformEmulator"/> interface 
    /// that does nothing.
    /// </summary>
    public class DefaultPlatformEmulator : IPlatformEmulator
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DefaultPlatformEmulator"/> class.
        /// </summary>
        public DefaultPlatformEmulator()
        {
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>();
        }

        /// <summary>
        /// A dictionary of intercepted calls. No calls are intercepted by this class.
        /// </summary>
        public Dictionary<Address, ExternalProcedure> InterceptedCalls { get; }

        /// <summary>
        /// Dummy implementation of <see cref="IPlatformEmulator.InterceptCall(IProcessorEmulator, uint)"/>.
        /// No calls are actually intercepted.
        /// </summary>
        /// <param name="emulator">Ignored.</param>
        /// <param name="calledAddress">Ignored.</param>
        /// <returns>Always false.</returns>
        public bool InterceptCall(IProcessorEmulator emulator, ulong calledAddress)
        {
            //$TODO: this is not implemented yet.
            return false;
        }

        /// <summary>
        /// No system calls are emulated by this class.
        /// </summary>
        /// <param name="emulator">Ignored.</param>
        /// <param name="operands">Ignored.</param>
        /// <returns>Always false.</returns>
        public bool EmulateSystemCall(IProcessorEmulator emulator, params MachineOperand[] operands)
        {
            return false;
        }

        /// <summary>
        /// The stack is not affected by this class.
        /// </summary>
        /// <param name="emulator">Ignored.</param>
        /// <param name="state">Ignored.</param>
        /// <returns>Nothing.</returns>
        public ImageSegment? InitializeStack(IProcessorEmulator emulator, ProcessorState state)
        {
            return null;
        }

        /// <summary>
        /// This class has a dummy implementation of this method.
        /// </summary>
        /// <param name="stackSeg">Ignored.</param>
        public void TearDownStack(ImageSegment? stackSeg)
        {
        }

        
    }
}
