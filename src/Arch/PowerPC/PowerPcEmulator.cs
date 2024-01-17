#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Emulation;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Reko.Arch.PowerPC
{
    public class PowerPcEmulator : EmulatorBase
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(PowerPcEmulator), "Trace execution of PowerPC Emulator") { Level = TraceLevel.Verbose };

        private readonly PowerPcArchitecture arch;
        private readonly SegmentMap map;
        private readonly IPlatformEmulator envEmulator;
        private ulong uInstrPointer;    //$REVIEW: is this exposed as a PPC special register?
        private IEnumerator<PowerPcInstruction> dasm;
        private readonly ulong[] registerFile;

        public PowerPcEmulator(PowerPcArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
            : base(segmentMap)
        {
            this.arch = arch;
            this.map = segmentMap;
            this.envEmulator = envEmulator;
            this.registerFile = new ulong[32];
            this.dasm = null!;
        }

        public override MachineInstruction CurrentInstruction => dasm.Current;

        public override Address InstructionPointer
        {
            get
            {
                return Address.Ptr64(uInstrPointer);
            }

            set
            {
                this.uInstrPointer = value.ToLinear();
                if (!map.TryFindSegment(value, out ImageSegment? segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
            }
        }

        protected override void Run()
        {
            while (IsRunning && dasm.MoveNext())
            {
                TraceCurrentInstruction();
                var pc = dasm.Current.Address;
                ulong linPc = pc.ToLinear();
                this.uInstrPointer = linPc;
                if (!TestForBreakpoint(linPc))
                    break;
                Execute(dasm.Current);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Execute(PowerPcInstruction instr)
        {
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException($"Instruction emulation for PowerPC instruction {instr} not implemented yet.");
            case Mnemonic.rldicl:
                {
                    var rs = Read(instr.Operands[1]);
                    var n = (int) Read(instr.Operands[2]);
                    var mb = 64 - (int) Read(instr.Operands[3]); // convert to little-endian bit numbering.
                    var mask = (1ul << mb) - 1ul;
                    var rd = Bits.RotateL64(rs, n) & mask;
                    Write(instr.Operands[0], rd);
                    break;
                }
            case Mnemonic.rldicr:
                {
                    var rs = Read(instr.Operands[1]);
                    var n = (int) Read(instr.Operands[2]);
                    var me = 63 - (int) Read(instr.Operands[3]); // convert to little-endian bit numbering.
                    var mask = me == 0 ?  ~0ul : Bits.Mask(me, 64 - me);
                    var rd = Bits.RotateL64(rs, n) & mask;
                    Write(instr.Operands[0], rd);
                    break;
                }
            }
        }

        private ulong Read(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage r:
                return ReadRegister(r);
            case ImmediateOperand i:
                return i.Value.ToUInt64();
            default:
               throw new NotImplementedException();
            }
        }

        private void Write(MachineOperand op, ulong value)
        {
            if (op is RegisterStorage reg)
            {
                WriteRegister(reg, value);
                return;
            }
            throw new NotImplementedException();
        }

        public sealed override ulong ReadRegister(RegisterStorage reg)
        {
            if (reg.Number == 0)
                return 0;
            return registerFile[reg.Number] & reg.BitMask; 
        }

        public sealed override ulong WriteRegister(RegisterStorage reg, ulong value)
        {
            if (reg.Number == 0)
                return 0;
            var oldValue = registerFile[reg.Number];
            var newValue = (oldValue & ~reg.BitMask) | (value & reg.BitMask);
            registerFile[reg.Number] = newValue;
            return newValue;
        }

        [Conditional("DEBUG")]
        private void TraceCurrentInstruction()
        {
            if (trace.Level != TraceLevel.Verbose)
                return;
            trace.Verbose("emu: {0} {1,-15}", dasm.Current.Address, dasm.Current);
        }
    }
}