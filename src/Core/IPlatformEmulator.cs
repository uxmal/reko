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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    using TWord = System.UInt32;
    /// <summary>
    /// Provides services for CPU emulators for specific environments
    /// </summary>
    public interface IPlatformEmulator
    {
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
        bool InterceptCall(IProcessorEmulator emulator, TWord calledAddress);

        ImageSegment InitializeStack(IProcessorEmulator emulator, ProcessorState state);

        void TearDownStack(ImageSegment stackSeg);
    }
}
