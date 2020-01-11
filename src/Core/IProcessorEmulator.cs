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

using Reko.Core.Emulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Interface for processor emulation.
    /// </summary>
    public interface IProcessorEmulator
    {
        /// <summary>
        /// This event gets fired when the <see cref="Start"/> method is called.
        /// </summary>
        /// <remarks>
        /// Listeners get a chance to modify the state of the emulator before
        /// execution.
        /// </remarks>
        event EventHandler BeforeStart;

        /// <summary>
        /// This events get fired if the emulator ran into an exception while executing.
        /// </summary>
        event EventHandler<EmulatorExceptionEventArgs> ExceptionRaised;

        /// <summary>
        /// The current instruction address.
        /// </summary>
        Address InstructionPointer { get; set; }

        /// <summary>
        /// Reads a register's current value in the emulator state.
        /// </summary>
        /// <param name="reg">The register whose value is read.</param>
        /// <returns>The value of that register.
        /// </returns>
        ulong ReadRegister(RegisterStorage reg);

        /// <summary>
        /// Writes a register value to the emulator state.
        /// </summary>
        /// <param name="reg">Registers whose value is to be written.</param>
        /// <param name="value">The new value of that register.</param>
        /// <returns>The written value.</returns>
        ulong WriteRegister(RegisterStorage reg, ulong value);

        /// <summary>
        /// Starts emulating instructions at the address <see cref="InstructionPointer"/>.
        /// Execution will continue until <see cref="Stop"/> is called or an unrecoverable exception
        /// occurs.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the emulator.
        /// </summary>
        void Stop();

        /// <summary>
        /// Asks the emulator to execute a single instruction, then perform a callback.
        /// </summary>
        /// <param name="callback">Callback to invoke.</param>
        void StepInto(Action callback);

        /// <summary>
        /// Asks the emulator to execute until the instruction pointer reaches the next
        /// instruction. 
        /// </summary>
        /// <param name="callback">Callback to invoke.</param>
        void StepOver(Action callback);

        /// <summary>
        /// Sets a breakpoint that will be invoked when the instruction pointer 
        /// contains the linear address <paramref name="linearAddress"/>, at which point the 
        /// <paramref name="callback"/> is invoked.
        /// </summary>
        /// <param name="linearAddress">Address of the breakpoint.</param>
        /// <param name="callback">Callback to invoke when the breakpoint is hit.</param>
        void SetBreakpoint(ulong linearAddress, Action callback);

        /// <summary>
        /// Deletes any breakpoint at the given <paramref name="linearAddress"/>.
        /// </summary>
        /// <param name="linearAddress">Breakpoint to delete.</param>
        void DeleteBreakpoint(ulong linearAddress);
    }
}
