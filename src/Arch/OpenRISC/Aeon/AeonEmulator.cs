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
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Emulation;
using Reko.Core.Loading;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class AeonEmulator : EmulatorBase
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(AeonEmulator), "Traces the running of the Aeon emulator")
        {
            Level = TraceLevel.Verbose,
        };

        private readonly AeonArchitecture arch;
        private readonly SegmentMap segmentMap;
        private readonly IPlatformEmulator envEmulator;
        private readonly uint[] gpregBank;
        private IEnumerator<AeonInstruction> dasm;
        private uint uInstrPtr;


        public AeonEmulator(AeonArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
            : base(segmentMap)
        {
            this.arch = arch;
            this.segmentMap = segmentMap;
            this.envEmulator = envEmulator;
            this.gpregBank = new uint[32];
            this.dasm = default!;
        }

        public override Address InstructionPointer
        {
            get => Address.Ptr32(uInstrPtr);
            set
            {
                this.uInstrPtr = value.ToUInt32();
                if (!segmentMap.TryFindSegment(value, out ImageSegment? segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = new AeonDisassembler(arch, rdr).GetEnumerator();
            }
        }

        public override MachineInstruction CurrentInstruction => dasm.Current;

        public override sealed ulong ReadRegister(RegisterStorage reg)
        {
            int n = reg.Number;
            if (n >= 0 && n < Registers.GpRegisters.Length)
            {
                return gpregBank[n];
            }
            throw new NotImplementedException($"Reading from special registers not supported yet.");
        }

        public override sealed ulong WriteRegister(RegisterStorage reg, ulong value)
        {
            int n = reg.Number;
            if (n == 0)
                return 0;
            if (n > 0 && n < Registers.GpRegisters.Length)
            {
                var ui = (uint) value;
                gpregBank[n] = ui;
                return ui;
            }
            throw new NotImplementedException($"Writing to special registers not supported yet.");
        }

        protected override void Run()
        {
            if (dasm is null)
                throw new InvalidOperationException($"The {nameof(InstructionPointer)} property needs to be set before running the emulator.");
            while (IsRunning && dasm.MoveNext())
            {
                TraceCurrentInstruction();
                var pc = dasm.Current.Address;
                uint linPc = pc.ToUInt32();
                this.uInstrPtr = linPc;
                if (!TestForBreakpoint(linPc))
                    break;
                Execute(dasm.Current);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Execute(AeonInstruction instr)
        {
            uint op1;
            uint op2;
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException(
                    $"Emulation of Aeon instruction {instr} is not implemented yet.");
            case Mnemonic.bg_addi:
            case Mnemonic.bt_addi__:
            case Mnemonic.bn_addi:
                op1 = ReadReg(instr, 1);
                op2 = ReadImm(instr, 2);
                WriteReg(instr, 0, op1 + op2);
                break;
            case Mnemonic.bg_beq__:
                op1 = ReadReg(instr, 0);
                op2 = ReadReg(instr, 1);
                if (op1 == op2) Jump(instr, 2);
                break;
            }
        }

        private void Jump(AeonInstruction instr, int iop)
        {
            Address addr = (Address)instr.Operands[iop];
            InstructionPointer = addr;
        }

        private uint ReadImm(AeonInstruction instr, int iop)
        {
            var imm = (ImmediateOperand) instr.Operands[iop];
            return imm.Value.ToUInt32();
        }

        private uint ReadReg(AeonInstruction instr, int iop)
        {
            var reg = (RegisterStorage) instr.Operands[iop];
            return (uint) ReadRegister(reg);
        }

        private void WriteReg(AeonInstruction instr, int iop, uint value)
        {
            var reg = (RegisterStorage) instr.Operands[iop];
            WriteRegister(reg, value);
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
