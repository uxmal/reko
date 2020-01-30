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
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Machine;

namespace Reko.Arch.Mos6502
{
    // http://www.obelisk.me.uk/6502/
    public class Mos6502Emulator : EmulatorBase
    {
        public const byte Cmask = 1;
        public const byte Zmask = 2;
        public const byte Imask = 4;
        public const byte Dmask = 8;
        public const byte Vmask = 0x40;
        public const byte Nmask = 0x80;

        private static readonly TraceSwitch trace = new TraceSwitch(nameof(Mos6502Emulator), "Trace execution of 6502 Emulator") { Level = TraceLevel.Verbose };
        private static readonly RegisterStorage[] dumpRegs = new[]
        {
            Registers.a, Registers.x, Registers.y, Registers.s, Registers.p
        };

        private Mos6502Architecture arch;
        private SegmentMap map;
        private IPlatformEmulator envEmulator;
        private ushort[] regs;
        private IEnumerator<Instruction> dasm;

        public Mos6502Emulator(Mos6502Architecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
            : base(segmentMap)
        {
            this.arch = arch;
            this.map = segmentMap;
            this.envEmulator = envEmulator;
            this.regs = new ushort[12];
        }

        public override MachineInstruction CurrentInstruction => dasm.Current;
        
        public override Address InstructionPointer
        {
            get
            {
                return Address.Ptr16(regs[Registers.pc.Number]);
            }

            set
            {
                UpdatePc(value);
                if (!map.TryFindSegment(value, out ImageSegment segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = new Disassembler(rdr).GetEnumerator();
            }
        }

        private void UpdatePc(Address value)
        {
            regs[Registers.pc.Number] = value.ToUInt16();
        }

        protected override void Run()
        {
            while (IsRunning && dasm.MoveNext())
            {
                TraceCurrentInstruction();
                var pc = dasm.Current.Address;
                UpdatePc(pc);
                ulong linPc = pc.ToLinear();
                if (!TestForBreakpoint(linPc))
                    break;
                Execute(dasm.Current);
            }
        } 

        public override ulong ReadRegister(RegisterStorage reg)
        {
            return regs[reg.Number];
        }

        public override ulong WriteRegister(RegisterStorage reg, ulong value)
        {
            var v = (ushort) value;
            this.regs[reg.Number] = v;
            return v;
        }

        [Conditional("DEBUG")]
        private void TraceCurrentInstruction()
        {
            if (trace.Level != TraceLevel.Verbose)
                return;
            Debug.Print("emu: {0} {1,-15} {2}", dasm.Current.Address, dasm.Current, DumpRegs());
        }

        private string DumpRegs()
        {
            return string.Join("", dumpRegs
                .Select(r => $" {r.Name} {regs[r.Number]:X2}"));
        }

        private void Execute(Instruction instr)
        {
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException(string.Format("Instruction emulation for {0} not implemented yet.", instr));
            case Mnemonic.bne: if ((regs[Registers.p.Number] & Zmask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.dex: NZ(regs[Registers.x.Number] = (byte)(ReadRegister(Registers.x)-1)); return;
            case Mnemonic.dey: NZ(regs[Registers.y.Number] = (byte)(ReadRegister(Registers.y)-1)); return;
            case Mnemonic.inx: NZ(regs[Registers.x.Number] = (byte)(ReadRegister(Registers.x)+1)); return;
            case Mnemonic.iny: NZ(regs[Registers.y.Number] = (byte)(ReadRegister(Registers.y)+1)); return;
            case Mnemonic.jmp: Jump(instr.Operands[0]); return;
            case Mnemonic.lda: NZ(regs[Registers.a.Number] = Read(instr.Operands[0])); return;
            case Mnemonic.ldx: NZ(regs[Registers.x.Number] = Read(instr.Operands[0])); return;
            case Mnemonic.ldy: NZ(regs[Registers.y.Number] = Read(instr.Operands[0])); return;
            case Mnemonic.nop: return;
            case Mnemonic.sta: Write(instr.Operands[0], regs[Registers.a.Number]); return;
            }
        }

        protected void Jump(MachineOperand op)
        {
            InstructionPointer = Address.Ptr16(((Operand) op).Offset.ToUInt16());
        }

        private ushort Read(MachineOperand mop)
        {
            var op = (Operand) mop;
            ushort ea;
            switch (op.Mode)
            {
            default:
                throw new NotImplementedException($"Addressing mode {op.Mode} not implemented yet.");
            case AddressMode.Immediate:
                return op.Offset.ToUInt16();
            case AddressMode.ZeroPage:
                ea = op.Offset.ToByte();
                break;
            case AddressMode.AbsoluteY:
                // Treat y as unsigned.
                ea = (ushort) (regs[Registers.y.Number] + op.Offset.ToUInt16());
                break;
            }
            return ReadMemory(op, ea);
        }

        private void Write(MachineOperand mop, ushort value)
        {
            var op = (Operand) mop;
            ushort ea;
            switch (op.Mode)
            {
            default:
                throw new NotImplementedException($"Addressing mode {op.Mode} not implemented yet.");
            case AddressMode.ZeroPage:
                ea = op.Offset.ToByte();
                break;
            case AddressMode.ZeroPageX:
            case AddressMode.ZeroPageY:
                ea = (ushort)(regs[op.Register.Number] + op.Offset.ToByte());
                break;
            case AddressMode.AbsoluteY:
                // Treat y as unsigned.
                ea = (ushort) (regs[Registers.y.Number] + op.Offset.ToUInt16());
                break;
            }
            WriteMemory(op, ea, value);
        }

        private ushort ReadMemory(Operand op, ushort ea)
        {
            if (op.Width.Size == 2)
            {
                return ReadLeUInt16(ea);
            }
            else
            {
                if (!TryReadByte(ea, out var b))
                    throw new AccessViolationException();
                return b;
            }
        }

        private void WriteMemory(Operand op, ushort ea, ushort value)
        {
            if (op.Width.Size == 2)
            {
                WriteLeUInt16(ea, value);
            }
            else
            {
                WriteByte(ea, (byte)value);
            }
        }

        private void NZ(ulong value)
        {
            var p = regs[Registers.p.Number];
            p &= unchecked((ushort)~(Nmask | Zmask));
            if (value == 0)
                p |= Zmask;
            if ((value & 0x80) != 0)
                p |= Nmask;
            regs[Registers.p.Number] = p;
        }
    }
}
