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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Emulation
{
    /// <summary>
    /// Common boilerplate implementation of a the <see cref="IProcessorEmulator"/>
    /// interface.
    /// </summary>
    public abstract class EmulatorBase : IProcessorEmulator
    {
        public event EventHandler BeforeStart;
        public event EventHandler<EmulatorExceptionEventArgs>  ExceptionRaised;

        private readonly SegmentMap map;
        private readonly Dictionary<ulong, Action> bpExecute;
        private Action stepAction;
        private bool stepInto;
        private ulong stepOverAddress;

        public EmulatorBase(SegmentMap map)
        {
            this.map = map;
            this.bpExecute = new Dictionary<ulong, Action>();
        }

        public abstract Address InstructionPointer { get; set; }
        public abstract MachineInstruction CurrentInstruction { get; }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            IsRunning = true;
            BeforeStart.Fire(this);
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

        public void StepInto(Action callback)
        {
            stepInto = true;
            stepAction = callback;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// The inner loop of the emulator, which emulates instructions until
        /// the <see cref="IsRunning"/> property is true.
        /// </summary>
        protected abstract void Run();

        public void SetBreakpoint(ulong address, Action callback)
        {
            bpExecute.Add(address, callback);
        }

        public void DeleteBreakpoint(ulong address)
        {
            bpExecute.Remove(address);
        }

        public abstract ulong ReadRegister(RegisterStorage reg);

        public abstract ulong WriteRegister(RegisterStorage reg, ulong value);

        /// <summary>
        /// Test if a breakpoint has been hit.
        /// </summary>
        /// <param name="linAddrInstr"></param>
        /// <returns></returns>
        protected bool TestForBreakpoint(ulong linAddrInstr)
        {
            if (bpExecute.TryGetValue(linAddrInstr, out Action bpAction))
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
                s();
            }
            else if (stepOverAddress == linAddrInstr)
            {
                stepOverAddress = 0;
                var s = stepAction;
                stepAction = null;
                s();
            }
            return IsRunning;
        }

        public bool TryReadByte(ulong ea, out byte b)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = ea - mem.BaseAddress.ToLinear();
            return segment.MemoryArea.TryReadByte((long) off, out b);
        }

        public ushort ReadLeUInt16(ulong ea)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = ea - mem.BaseAddress.ToLinear();
            return mem.ReadLeUInt16((uint) off);
        }

        public uint ReadLeUInt32(ulong ea)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = ea - mem.BaseAddress.ToLinear();
            return mem.ReadLeUInt32((uint) off);
        }

        public void WriteByte(ulong ea, byte value)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            mem.WriteByte((long) (ea - mem.BaseAddress.ToLinear()), value);
        }

        public void WriteLeUInt16(ulong ea, ushort value)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = ea - mem.BaseAddress.ToLinear();
            segment.MemoryArea.WriteLeUInt16((uint) off, value);
        }

        public void WriteLeUInt32(ulong ea, uint value)
        {
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            var mem = segment.MemoryArea;
            var off = ea - mem.BaseAddress.ToLinear();
            segment.MemoryArea.WriteLeUInt32((long) off, value);
        }
    }
}
