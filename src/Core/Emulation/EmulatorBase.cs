#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Diagnostics;

namespace Reko.Core.Emulation
{
    /// <summary>
    /// Common boilerplate implementation of a the <see cref="IProcessorEmulator"/>
    /// interface.
    /// </summary>
    public abstract class EmulatorBase : IProcessorEmulator
    {
        /// <summary>
        /// This event is fired before the emulator starts executing instructions.
        /// It is a good place to initialize debuggers, set breakpoints, etc.
        /// </summary>
        public event EventHandler? BeforeStart;

        /// <summary>
        /// This event is fired if the emulator ran into an exception while executing.
        /// </summary>
        public event EventHandler<EmulatorExceptionEventArgs>? ExceptionRaised;

        private readonly SegmentMap map;
        private readonly Dictionary<ulong, Action> bpExecute;
        private Action? stepAction;
        private bool stepInto;
        private ulong stepOverAddress;

        /// <summary>
        /// Initializes the emulator with the given <paramref name="map" />.
        /// </summary>
        /// <param name="map">A <see cref="SegmentMap"/> instance describing the 
        /// memory of the emulated process.</param>
        public EmulatorBase(SegmentMap map)
        {
            this.map = map;
            this.bpExecute = new Dictionary<ulong, Action>();
        }

        /// <summary>
        /// The current value of the instruction pointer.
        /// </summary>
        public abstract Address InstructionPointer { get; set; }

        /// <summary>
        /// The current instruction being emulated.
        /// </summary>
        public abstract MachineInstruction CurrentInstruction { get; }

        /// <summary>
        /// Returns true if the emulator is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts the emulator.
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            BeforeStart?.Invoke(this, EventArgs.Empty);
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Debug.Print("Emulator exception when executing {0}. {1}\r\n{2}", CurrentInstruction, ex.Message, ex.StackTrace);
                ExceptionRaised?.Invoke(this, new EmulatorExceptionEventArgs(ex));
            }
        }


        /// <summary>
        /// Requests the emulator to place itself in step-over mode. This means
        /// it should execute the next instruction then call the provided 
        /// <paramref name="callback" />. If the instruction
        /// is a CALL or a REP[NZ] the call will be taken and the REP will be 
        /// carried out before resuming.
        /// </summary>
        public void StepOver(Action callback)
        {
            var instr = CurrentInstruction;
            stepOverAddress = instr.Address.ToLinear() + (uint)instr.Length;
            stepAction = callback;
        }

        /// <summary>
        /// Requests the emulator to place itself in step-into mode. This means
        /// it should execute the next instruction then call the provided
        /// <paramref name="callback"/>. If the instruction is a CALL-type 
        /// instruction, the emulator will step into the called function.
        /// For a REP-style instruction, only one iteration of the repeated
        /// instruction is emulated.
        /// </summary>
        /// <param name="callback"></param>
        public void StepInto(Action callback)
        {
            stepInto = true;
            stepAction = callback;
        }

        /// <summary>
        /// Stops the emulator.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// The inner loop of the emulator, which emulates instructions until
        /// the <see cref="IsRunning"/> property is false.
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// Sets a breakpoint at the given <paramref name="address"/>.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="callback"></param>
        public void SetBreakpoint(ulong address, Action callback)
        {
            bpExecute.Add(address, callback);
        }

        /// <summary>
        /// Deletes any breakpoints at the given <paramref name="address"/>.
        /// </summary>
        /// <param name="address"></param>
        public void DeleteBreakpoint(ulong address)
        {
            bpExecute.Remove(address);
        }

        /// <summary>
        /// Reads a register's current value from the emulator state.
        /// </summary>
        /// <param name="reg">Register whose value is to be retrieved.</param>
        /// <returns>Register value as a bit vector.</returns>
        public abstract ulong ReadRegister(RegisterStorage reg);

        /// <summary>
        /// Writes a register value to the emulator state.
        /// </summary>
        /// <param name="reg">Register whose value is to be written.</param>
        /// <param name="value">Register value as a bit vector.</param>
        /// <returns>The new value in the register.</returns>
        public abstract ulong WriteRegister(RegisterStorage reg, ulong value);

        /// <summary>
        /// Test if a breakpoint has been hit.
        /// </summary>
        /// <param name="linAddrInstr"></param>
        /// <returns></returns>
        protected bool TestForBreakpoint(ulong linAddrInstr)
        {
            if (bpExecute.TryGetValue(linAddrInstr, out Action? bpAction))
            {
                stepOverAddress = 0;
                stepInto = false;
                bpAction();
            }
            else if (stepInto)
            {
                stepInto = false;
                var s = stepAction;
                stepAction = null;
                s?.Invoke();
            }
            else if (stepOverAddress == linAddrInstr)
            {
                stepOverAddress = 0;
                var s = stepAction;
                stepAction = null;
                s?.Invoke();
            }
            return IsRunning;
        }

        /// <summary>
        /// Reads a byte from the address <paramref name="address"/>.
        /// </summary>
        /// <param name="address">Address to read from.</param>
        /// <param name="b">Byte read.</param>
        /// <returns>True if successful; othewise an <see cref="AccessViolationException"/> is thrown</returns>
        public bool TryReadByte(ulong address, out byte b)
        {
            //$REVIEW: consider using an "EmulatorException" instead.
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = address - mem.BaseAddress.ToLinear();
            return segment.MemoryArea.TryReadByte((long) off, out b);
        }

        /// <summary>
        /// Attempt to read a 16-bit unsigned integer at the given address.
        /// </summary>
        /// <param name="address">Address from which to read the 16-bit unsigned integer.</param>
        /// <returns>The read value if the address was readable.</returns>
        public ushort ReadLeUInt16(ulong address)
        {
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = address - mem.BaseAddress.ToLinear();
            if (!mem.TryReadLeUInt16((uint) off, out ushort retvalue))
                throw new AccessViolationException();
            return retvalue;
        }

        /// <summary>
        /// Attempt to read a 32-bit unsigned integer at the given address.
        /// </summary>
        /// <param name="address">Address from which to read the 32-bit unsigned integer.</param>
        /// <returns>The read value if the address was readable.</returns>
        public uint ReadLeUInt32(ulong address)
        {
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = address - mem.BaseAddress.ToLinear();
            if (!mem.TryReadLeUInt32((uint) off, out var retvalue))
                throw new AccessViolationException();
            return retvalue;
        }

        /// <summary>
        /// Writes a byte to the given address.
        /// </summary>
        /// <param name="address">Addres at which to write a byte.</param>
        /// <param name="value">Byte to write.</param>
        /// <exception cref="AccessViolationException"></exception>

        public void WriteByte(ulong address, byte value)
        {
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            mem.WriteByte((long) (address - mem.BaseAddress.ToLinear()), value);
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer to the given address.
        /// </summary>
        /// <param name="address">Address at which to write the value.</param>
        /// <param name="value">16-bit unsigned integer to write.</param>
        /// <exception cref="AccessViolationException"></exception>
        public void WriteLeUInt16(ulong address, ushort value)
        {
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = address - mem.BaseAddress.ToLinear();
            segment.MemoryArea.WriteLeUInt16((uint) off, value);
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer to the given address.
        /// </summary>
        /// <param name="address">Address at which to write the value.</param>
        /// <param name="value">32-bit unsigned integer to write.</param>
        /// <exception cref="AccessViolationException"></exception>
        public void WriteLeUInt32(ulong address, uint value)
        {
            if (!map.TryFindSegment(address, out ImageSegment? segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = address - mem.BaseAddress.ToLinear();
            segment.MemoryArea.WriteLeUInt32((long) off, value);
        }
    }
}
