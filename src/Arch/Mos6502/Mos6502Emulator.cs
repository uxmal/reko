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
using Reko.Core;

namespace Reko.Arch.Mos6502
{
    public class Mos6502Emulator : IProcessorEmulator
    {
        private Mos6502ProcessorArchitecture arch;
        private SegmentMap map;
        private IPlatformEmulator envEmulator;
        private ushort[] regs;
        private IEnumerator<Instruction> dasm;

        public Mos6502Emulator(Mos6502ProcessorArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            this.arch = arch;
            this.map = segmentMap;
            this.envEmulator = envEmulator;
            this.regs = new ushort[12];
        }

        public Address InstructionPointer
        {
            get
            {
                return Address.Ptr16(regs[Registers.pc.Number]);
            }

            set
            {
                regs[Registers.pc.Number] = value.ToUInt16();
                if (!map.TryFindSegment(value, out ImageSegment segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = new Disassembler(rdr).GetEnumerator();
            }
        }

        public event EventHandler BeforeStart;
        public event EventHandler ExceptionRaised;

        public void DeleteBreakpoint(ulong linearAddress)
        {
            throw new NotImplementedException();
        }

        public ulong ReadRegister(RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public void SetBreakpoint(ulong linearAddress, Action callback)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            BeforeStart?.Invoke(this, EventArgs.Empty);
            throw new NotImplementedException();
        }

        public void StepInto(Action callback)
        {
            throw new NotImplementedException();
        }

        public void StepOver(Action callback)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void WriteRegister(RegisterStorage reg, ulong value)
        {
            throw new NotImplementedException();
        }
    }
}
