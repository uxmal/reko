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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Emulation;
using Reko.Core.Loading;
using Reko.Core.Machine;

namespace Reko.Arch.Mos6502
{
    // http://www.obelisk.me.uk/6502/
    // https://www.masswerk.at/6502/6502_instruction_set.html
    public class Mos6502Emulator : EmulatorBase
    {
        public const byte Cmask = 1;
        public const byte Zmask = 2;
        public const byte Imask = 4;
        public const byte Dmask = 8;
        public const byte Vmask = 0x40;
        public const byte Nmask = 0x80;

        private static readonly TraceSwitch trace = new TraceSwitch(nameof(Mos6502Emulator), "Trace execution of 6502 Emulator") { Level = TraceLevel.Error };

        private static readonly RegisterStorage[] dumpRegs = new[]
        {
            Registers.a, Registers.x, Registers.y, Registers.s, Registers.p
        };

        private readonly Mos6502Architecture arch;
        private readonly SegmentMap map;
        private readonly IPlatformEmulator envEmulator;
        private readonly ushort[] regs;
        private IEnumerator<Instruction>? dasm;

        public Mos6502Emulator(Mos6502Architecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
            : base(segmentMap)
        {
            this.arch = arch;
            this.map = segmentMap;
            this.envEmulator = envEmulator;
            this.regs = new ushort[12];
            this.regs[Registers.s.Number] = 0xFF;
        }

        public override MachineInstruction CurrentInstruction => dasm!.Current;
        
        public override Address InstructionPointer
        {
            get
            {
                return Address.Ptr16(regs[Registers.pc.Number]);
            }

            set
            {
                UpdatePc(value);
                if (!map.TryFindSegment(value, out ImageSegment? segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = new Disassembler(arch, rdr).GetEnumerator();
            }
        }

        private void UpdatePc(Address value)
        {
            regs[Registers.pc.Number] = value.ToUInt16();
        }

        protected override void Run()
        {
            while (IsRunning)
            {
                Instruction? instr = null;
                while (IsRunning && dasm!.MoveNext())
                {
                    TraceCurrentInstruction();
                    instr = dasm.Current;
                    var pc = instr.Address;
                    UpdatePc(pc);
                    ulong linPc = pc.ToLinear();
                    if (!TestForBreakpoint(linPc))
                        break;
                    Execute(instr);
                }
                if (instr is null)
                    break;
                var addr = instr.Address + instr.Length;
                if (!map.TryFindSegment(addr, out _))
                    return;
                this.InstructionPointer = addr;
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
            if (reg == Registers.pc)
            {
                if (!map.TryFindSegment(value, out ImageSegment? segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, Address.Ptr16(v));
                dasm = new Disassembler(arch, rdr).GetEnumerator();
            }
            return v;
        }

        [Conditional("DEBUG")]
        private void TraceCurrentInstruction()
        {
            if (trace.Level != TraceLevel.Verbose)
                return;
            var instr = dasm!.Current;
            trace.Verbose("emu: {0} {1,-15} {2}", instr.Address, instr, DumpRegs(instr));
        }

        private string DumpRegs(Instruction instr)
        {
            var s = string.Join("", dumpRegs
                .Select(r => $" {r.Name} {regs[r.Number]:X2}"));
            if (instr.Operands.Length == 0)
                return s;
            var op = (Operand) instr.Operands[0];
            ushort ea;
            switch (op.Mode)
            {
            default:
                return s;
            case AddressMode.ZeroPage:
                ea = op.Offset!.ToByte();
                break;
            case AddressMode.Absolute:
                ea = op.Offset!.ToUInt16();
                break;
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY:
                // Treat y as unsigned.
                ea = (ushort) (regs[op.Register!.Number] + op.Offset!.ToUInt16());
                break;
            case AddressMode.IndirectIndexed:
                ea = ReadLeUInt16(op.Offset!.ToUInt16());
                ea += regs[op.Register!.Number];
                break;
            }
            
            try { 
            return $"{s} [{ea:X4}] = {(byte)ReadMemory(op, ea):X2}";
            } catch
            {
                return $"{s} [{ea:X4}] = ?? (probably a bug)";
            }
        }

        private void Execute(Instruction instr)
        {
            byte b;
            MachineOperand op = instr.Operands.Length > 0
                ? instr.Operands[0]
                : null!;
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException($"Instruction emulation for {instr} not implemented yet.");
            case Mnemonic.adc:
                Adc(op);
                return;
            case Mnemonic.asl:
                b = (byte) Read(op);
                SetFlag((b & 0x80) != 0, Cmask);
                Write(op, (byte)(b << 1));
                NZ(b);
                return;
            case Mnemonic.bcc: if ((regs[Registers.p.Number] & Cmask) == 0) Jump(op); return;
            case Mnemonic.bcs: if ((regs[Registers.p.Number] & Cmask) != 0) Jump(op); return;
            case Mnemonic.beq: if ((regs[Registers.p.Number] & Zmask) != 0) Jump(op); return;
            case Mnemonic.bne: if ((regs[Registers.p.Number] & Zmask) == 0) Jump(op); return;
            case Mnemonic.bpl: if ((regs[Registers.p.Number] & Nmask) == 0) Jump(op); return;
            case Mnemonic.clc: regs[Registers.p.Number] &= unchecked((ushort)~Cmask); return;
            case Mnemonic.cli: regs[Registers.p.Number] &= unchecked((ushort)~Imask); return;
            case Mnemonic.cmp: Cmp(Registers.a, op);return;
            case Mnemonic.cpy: Cmp(Registers.y, op);return;
            case Mnemonic.dec:
                b = (byte) (Read(op) - 1);
                Write(op, b);
                NZ(b);
                return;
            case Mnemonic.dex: NZ(regs[Registers.x.Number] = (byte)(ReadRegister(Registers.x)-1)); return;
            case Mnemonic.dey: NZ(regs[Registers.y.Number] = (byte)(ReadRegister(Registers.y)-1)); return;
            case Mnemonic.inc:
                b = (byte) (Read(op) + 1);
                Write(op, b);
                NZ(b);
                return;
            case Mnemonic.inx: NZ(regs[Registers.x.Number] = (byte)(ReadRegister(Registers.x)+1)); return;
            case Mnemonic.iny: NZ(regs[Registers.y.Number] = (byte)(ReadRegister(Registers.y)+1)); return;
            case Mnemonic.jmp: Jump(op); return;
            case Mnemonic.jsr: Jsr(op); return;
            case Mnemonic.lda: NZ(regs[Registers.a.Number] = Read(op)); return;
            case Mnemonic.ldx: NZ(regs[Registers.x.Number] = Read(op)); return;
            case Mnemonic.ldy: NZ(regs[Registers.y.Number] = Read(op)); return;
            case Mnemonic.nop: return;
            case Mnemonic.pha: Push((byte)regs[Registers.a.Number]); return;
            case Mnemonic.pla: Pop(Registers.a); return;
            case Mnemonic.rol: Rol(op); return;
            case Mnemonic.rts: Rts(); return;
            case Mnemonic.sbc: Sbc(op); return;
            case Mnemonic.sec: regs[Registers.p.Number] |= Cmask; return;
            case Mnemonic.sei: regs[Registers.p.Number] |= Imask; return;
            case Mnemonic.sta: Write(op, regs[Registers.a.Number]); return;
            case Mnemonic.stx: Write(op, regs[Registers.x.Number]); return;
            case Mnemonic.sty: Write(op, regs[Registers.y.Number]); return;
            case Mnemonic.tax: TransferNZ(Registers.a, Registers.x); return;
            case Mnemonic.tay: TransferNZ(Registers.a, Registers.y); return;
            case Mnemonic.txa: TransferNZ(Registers.x, Registers.a); return;
            case Mnemonic.tya: TransferNZ(Registers.y, Registers.a); return;
            }
        }

        private void Adc(MachineOperand op)
        {
            var a = (byte) regs[Registers.a.Number];
            var p = (byte) regs[Registers.p.Number];
            var v = (byte) Read(op);
            var s = (byte) (a + v + (p&Cmask));
            regs[Registers.a.Number] = s;
            p = NZ(s, p);
            p &= unchecked((byte) ~(Cmask|Vmask));
            p |= (byte) (s < a ? Cmask : 0);
            p |= (byte) (((a ^ s) & (a ^ s) & 0x80) != 0 ? Vmask : 0);
            regs[Registers.p.Number] = p;
        }

        private void Sbc(MachineOperand op)
        {
            var a = (byte) regs[Registers.a.Number];
            var v = (byte) Read(op);
            var p = (byte) regs[Registers.p.Number];
            var ss = (a + ~v + (p&Cmask));
            var s = (byte) ss;
            regs[Registers.a.Number] = s;
            p = NZ(s, p);
            p &= unchecked((byte) ~(Cmask | Vmask));
            p |= (byte) (ss > 0xFF ? Cmask : 0);
            p |= (byte) (((a ^ s) & (a ^ s) & 0x80) != 0 ? Vmask : 0);
            regs[Registers.p.Number] = p;
        }

        private void Cmp(RegisterStorage reg, MachineOperand op)
        {
            var a = (byte)regs[reg.Number];
            var b = Read(op);
            var r = (byte)(a - b);
            var p = (byte) regs[Registers.p.Number];
            p = NZ(r, p);
            p &= unchecked((byte)~Cmask);
            p |= (byte)(r > a ? Cmask : 0);
            regs[Registers.p.Number] = p;
        }

        private void Jump(MachineOperand op)
        {
            InstructionPointer = Address.Ptr16(((Operand) op).Offset!.ToUInt16());
        }

        private void Jsr(MachineOperand op)
        {
            var pcNext = this.InstructionPointer.ToUInt16() + 2;
            Push((byte) (pcNext >> 8));
            Push((byte) pcNext);
            InstructionPointer = Address.Ptr16(((Operand) op).Offset!.ToUInt16());
        }

        private void Rol(MachineOperand op)
        {
            var p = regs[Registers.p.Number];
            var oldC = p & Cmask;
            byte b = (byte) Read(op);
            p = SetFlag((b & 0x80) != 0, Cmask, p);
            Write(op, (byte) ((b << 1) | oldC));
            p = NZ(b, p);
            regs[Registers.p.Number] = p;
            return;
        }
        private void Rts()
        {
            var lsb = Pop();
            var msb = Pop();
            var pcNext = (ushort) (((msb << 8) | lsb) + 1);
            InstructionPointer = Address.Ptr16(pcNext);
        }

        private byte Pop()
        {
            var s = this.regs[Registers.s.Number] + 1;
            this.regs[Registers.s.Number] = (byte) s;
            var sp = 0x100u | (byte)s;
            if (!TryReadByte(sp, out byte b))
                throw new AccessViolationException();
            return b;
        }

        private void Pop(RegisterStorage reg)
        {
            var b = Pop();
            regs[reg.Number] = b;
            NZ(b);
        }

        private void Push(byte value)
        {
            var s = this.regs[Registers.s.Number];
            var sp = 0x100u | s;
            WriteByte(sp, value);
            this.regs[Registers.s.Number] = (byte) (s - 1);
        }

        private void TransferNZ(RegisterStorage src, RegisterStorage dst)
        {
            var v = this.regs[src.Number];
            regs[dst.Number] = v;
            NZ(v);
        }

        private ushort Read(MachineOperand mop)
        {
            var op = (Operand) mop;
            ushort ea;
            switch (op.Mode)
            {
            default:
                throw new NotImplementedException($"Addressing mode {op.Mode} not implemented yet.");
            case AddressMode.Accumulator:
                return regs[Registers.a.Number];
            case AddressMode.Immediate:
                return op.Offset!.ToUInt16();
            case AddressMode.ZeroPage:
                ea = op.Offset!.ToByte();
                break;
            case AddressMode.Absolute:
                ea = op.Offset!.ToUInt16();
                break;
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY:
                // Treat y as unsigned.
                ea = (ushort) (regs[op.Register!.Number] + op.Offset!.ToUInt16());
                break;
            case AddressMode.IndirectIndexed:
                ea = ReadLeUInt16(op.Offset!.ToUInt16());
                ea += regs[op.Register!.Number];
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
            case AddressMode.Accumulator:
                regs[Registers.a.Number] = (byte) value;
                return;
            case AddressMode.ZeroPage:
                ea = op.Offset!.ToByte();
                break;
            case AddressMode.ZeroPageX:
            case AddressMode.ZeroPageY:
                ea = (ushort)(regs[op.Register!.Number] + op.Offset!.ToByte());
                break;
            case AddressMode.Absolute:
                ea = op.Offset!.ToUInt16();
                break;
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY:
                // Treat x or y as unsigned.
                ea = (ushort) (regs[op.Register!.Number] + op.Offset!.ToUInt16());
                break;
            case AddressMode.IndirectIndexed:
                ea = ReadLeUInt16(op.Offset!.ToUInt16());
                ea += regs[op.Register!.Number];
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
            p = NZ(value, p);
            regs[Registers.p.Number] = p;
        }

        private static byte NZ(ulong value, ushort p)
        {
            p &= unchecked((ushort) ~(Nmask | Zmask));
            if (value == 0)
                p |= Zmask;
            if ((value & 0x80) != 0)
                p |= Nmask;
            return (byte) p;
        }

        private void SetFlag(bool condition, ushort mask)
        {
            var i = Registers.p.Number;
            var p = regs[i];
            p = SetFlag(condition, mask, p);
            regs[i] = p;
        }

        private static ushort SetFlag(bool condition, ushort mask, ushort p)
        {
            ushort value = condition ? mask : (ushort) 0;
            p = (ushort) ((p & ~mask) | value);
            return p;
        }
    }
}
