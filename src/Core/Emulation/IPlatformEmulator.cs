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

namespace Reko.Core.Emulation

{
    /// <summary>
    /// Provides services for CPU emulators for specific environments
    /// </summary>
    public interface IPlatformEmulator
    {
        /// <summary>
        /// A dictionary of platform services that were called by the emulated
        /// program, organized by the address of the calling instruction.
        /// </summary>
        Dictionary<Address, ExternalProcedure> InterceptedCalls { get; }

        /// <summary>
        /// Processor emulators call this method to give the platform emulator 
        /// a shot at intercepting the call to simulate an OS service. If the call
        /// is intercepted, the platform emulator is responsible for emulating the
        /// call, including updating any register state that may have been modified
        /// by appropriate calls to IProcessorEmulator.WriteRegister.
        /// </summary>
        /// <param name="emulator">Emulator requesting the call</param>
        /// <param name="calledAddress">The address called</param>
        /// <returns>True if the platform emulator intercepted the call and simulated it.
        /// False otherwise.</returns>
        bool InterceptCall(IProcessorEmulator emulator, ulong calledAddress);

        /// <summary>
        /// When called, emulates a system call.
        /// </summary>
        /// <param name="emulator">The emulator that is calling this method.</param>
        /// <param name="operands"></param>
        /// <returns>True if the platform emulator emulated the system call.
        /// False otherwise.</returns>
        bool EmulateSystemCall(IProcessorEmulator emulator, params MachineOperand[] operands);

        /// <summary>
        /// Requests the platform emulator to create and initialize the stack segment.
        /// </summary>
        /// <param name="emulator">Processor emulator instance.</param>
        /// <param name="state">Processor register state.</param>
        /// <returns>The newly create stack segment if applicable.</returns>
        ImageSegment? InitializeStack(IProcessorEmulator emulator, ProcessorState state);

        /// <summary>
        /// Destroys the stack segment created by the platform emulator.
        /// </summary>
        /// <param name="stackSeg">A stack segment previously created by the emulator.</param>
        void TearDownStack(ImageSegment? stackSeg);
    }
}
