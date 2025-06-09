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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reko.Arch.X86.Emulator
{
    using TWord = UInt32;

    /// <summary>
    /// Simple emulator of X86 instructions. No attempt is made to be high-performance
    /// as long as correctness is maintained.
    /// </summary>
    public abstract class X86Emulator : EmulatorBase
    {
        public const uint Cmask = 1u << 0;
        public const uint Zmask = 1u << 6;
        public const uint Smask = 1u << 7;
        public const uint Imask = 1u << 9;
        public const uint Dmask = 1u << 10;
        public const uint Omask = 1u << 11;

        protected static readonly TraceSwitch trace = new TraceSwitch(nameof(X86Emulator), "Trace execution of X86 Emulator")
        {
            Level = TraceLevel.Warning
        };

        public static readonly (uint value, uint hibit)[] masks = new (uint, uint)[]{
                (0, 0),
                (0x0000_00FFu,  0x0000_0080),
                (0x0000_FFFFu, 0x0000_8000),
                (0, 0),

                (0xFFFF_FFFFu, 0x8000_0000),
                (0, 0),
                (0, 0),
                (0, 0),

                (0, 0),  //$TODO: 64-bit implementation.
            };

        protected readonly IntelArchitecture arch;
        protected readonly SegmentMap map;
        protected readonly IPlatformEmulator envEmulator;
        protected IEnumerator<X86Instruction> dasm;
        private readonly RegisterStorage ipReg;
        private readonly RegisterStorage cxReg;

        public readonly ulong[] Registers;
        private readonly int iFlags;
        private Address ip;
        private bool ignoreRep;

        public X86Emulator(
            IntelArchitecture arch,
            SegmentMap segmentMap,
            IPlatformEmulator envEmulator,
            RegisterStorage ipReg,
            RegisterStorage cxReg)
            : base(segmentMap)
        {
            this.arch = arch;
            map = segmentMap;
            this.ipReg = ipReg;
            this.cxReg = cxReg;
            Registers = new ulong[56];
            iFlags = X86.Registers.eflags.Number;
            this.envEmulator = envEmulator;
            dasm = default!;
            ip = default!;
        }

        public override MachineInstruction CurrentInstruction => dasm.Current;

        public ulong Flags
        {
            get { return Registers[iFlags]; }
            set { Registers[iFlags] = value; }
        }

        public abstract Address AddressFromWord(ulong word);

        /// <summary>
        /// The current instruction pointer of the emulator.
        /// </summary>
        public override Address InstructionPointer
        {
            get { return ip; }
            set
            {
                UpdateIp(value);
                if (!map.TryFindSegment(ip, out ImageSegment? segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateIp(Address value)
        {
            ip = value;
            WriteRegister(ipReg, (TWord) value.Offset);
            if (value.Selector.HasValue)
            {
                WriteRegister(X86.Registers.cs, value.Selector.Value);
            }
        }

        private StringBuilder DumpRegs()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 8; ++i)
            {
                sb.AppendFormat(" {0} {1:X8}", arch.GetRegister(i + StorageDomain.Register, new BitRange(0, 64))!.Name, Registers[i]);
            }
            sb.Append(' ');
            sb.Append((Flags & Cmask) != 0 ? 'C' : 'c');
            sb.Append((Flags & Zmask) != 0 ? 'Z' : 'z');
            sb.Append((Flags & Smask) != 0 ? 'S' : 's');
            sb.Append((Flags & Imask) != 0 ? 'I' : 'i');
            sb.Append((Flags & Dmask) != 0 ? 'D' : 'd');
            sb.Append((Flags & Omask) != 0 ? 'O' : 'o');
            return sb;
        }

        protected override void Run()
        {
            while (IsRunning && dasm.MoveNext())
            {
                TraceCurrentInstruction();
                UpdateIp(dasm.Current.Address);
                ulong eip = ip.ToLinear();
                if (!TestForBreakpoint(eip))
                    break;
                Execute(dasm.Current);
            }
        }

        [Conditional("DEBUG")]
        private void TraceCurrentInstruction()
        {
            if (trace.Level != TraceLevel.Verbose)
                return;
            TraceState(dasm.Current);
        }

        protected virtual void TraceState(X86Instruction current)
        {
            trace.Verbose("emu: {0} {1,-15} {2}", dasm.Current.Address, dasm.Current, DumpRegs());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(X86Instruction instr)
        {
            if (!ignoreRep)
            {
                if (instr.RepPrefix == 2)
                {
                    // repne
                    switch (instr.Mnemonic)
                    {
                    case Mnemonic.cmpsb: Repne(); return;
                    case Mnemonic.scasb: Repne(); return;
                    }
                    throw new NotImplementedException();
                }
                else if (instr.RepPrefix == 3)
                {
                    // rep / repe
                    switch (instr.Mnemonic)
                    {
                    case Mnemonic.cmpsb: Repe(); return;
                    case Mnemonic.lods: Rep(); return;
                    case Mnemonic.lodsb: Rep(); return;
                    case Mnemonic.movs: Rep(); return;
                    case Mnemonic.movsb: Rep(); return;
                    case Mnemonic.scasb: Repe(); return;
                    case Mnemonic.stosb: Rep(); return;
                    }
                    throw new NotImplementedException();
                }
            }
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException($"Instruction emulation for {instr} not implemented yet.");
            case Mnemonic.adc: Adc(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.add: Add(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.and: And(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.call: Call(instr.Operands[0]); return;
            case Mnemonic.clc: Clc(); return;
            case Mnemonic.cld: Cld(); return;
            case Mnemonic.cli: Flags &= ~Imask; return;
            case Mnemonic.std: Std(); return;
            case Mnemonic.cmp: Cmp(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.dec: Dec(instr.Operands[0]); return;
            case Mnemonic.hlt: Stop(); return;
            case Mnemonic.inc: Inc(instr.Operands[0]); return;
            case Mnemonic.ja: if ((Flags & (Cmask | Zmask)) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jbe: if ((Flags & (Cmask | Zmask)) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jc: if ((Flags & Cmask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jcxz: Jcxz(instr.Operands[0]); return;
            case Mnemonic.jmp: Jump(instr.Operands[0]); return;
            case Mnemonic.jnc: if ((Flags & Cmask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jns: if ((Flags & Smask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jnz: if ((Flags & Zmask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.js: if ((Flags & Smask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jz: if ((Flags & Zmask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.lea: Write(instr.Operands[0], GetEffectiveOffset((MemoryOperand) instr.Operands[1])); return;
            case Mnemonic.lodsb: Lods(instr); return;
            case Mnemonic.lods: Lods(instr); return;
            case Mnemonic.loop: Loop(instr.Operands[0]); return;
            case Mnemonic.mov: Write(instr.Operands[0], Read(instr.Operands[1])); return;
            case Mnemonic.movs: Movs(instr); return;
            case Mnemonic.movsb: Movs(instr); return;
            case Mnemonic.nop: return;
            case Mnemonic.not: Not(instr.Operands[0]); return;
            case Mnemonic.or: Or(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.pop: Write(instr.Operands[0], Pop(instr.Operands[0].DataType)); return;
            case Mnemonic.popa: Popa(); return;
            case Mnemonic.push: Push(instr.Operands[0]); return;
            case Mnemonic.pusha: Pusha(); return;
            case Mnemonic.ret: Ret(); return;
            case Mnemonic.retf: Retf(); return;
            case Mnemonic.rcl: Rcl(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.rol: Rol(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.sar: Sar(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.scasb: Scas(instr); return;
            case Mnemonic.shl: Shl(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.shr: Shr(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.stc: Flags |= Cmask; break;
            case Mnemonic.sti: Flags |= Imask; break;
            case Mnemonic.stosb: Stos(instr); break;
            case Mnemonic.sub: Sub(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.test: Test(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xor: Xor(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xchg: Xchg(instr.Operands[0], instr.Operands[1]); return;
            }
        }

        protected abstract ulong GetEffectiveAddress(MemoryOperand m);

        protected TWord GetEffectiveOffset(MemoryOperand m)
        {
            TWord ea = 0;
            if (m.Offset is not null)
                ea += m.Offset.ToUInt32();
            if (m.Index != RegisterStorage.None)
                ea += (TWord) ReadRegister(m.Index) * m.Scale;
            if (m.Base is not null && m.Base != RegisterStorage.None)
            {
                ea += (TWord) ReadRegister(m.Base);
            }
            return ea;
        }

        protected TWord Read(MachineOperand op)
        {
            if (op is RegisterStorage r)
            {
                return (TWord) ReadRegister(r);
            }
            if (op is Constant i)
                return i.ToUInt32();
            if (op is Address a)
                return a.ToUInt32();
            if (op is MemoryOperand m)
            {
                ulong ea = GetEffectiveAddress(m);
                return ReadMemory(ea, m.DataType);
            }
            throw new NotImplementedException();
        }

        public override sealed ulong ReadRegister(RegisterStorage r)
        {
            return (Registers[r.Number] & r.BitMask) >> (int) r.BitAddress;
        }

        public TWord ReadMemory(ulong ea, DataType dt)
        {
            switch (dt.Size)
            {
            case 1: if (!TryReadByte(ea, out byte b)) throw new IndexOutOfRangeException(); else return b;
            case 2: return ReadLeUInt16(ea);
            case 4: return ReadLeUInt32(ea);
            case 8: throw new NotImplementedException();
            }
            throw new InvalidOperationException();
        }

        private void Write(MachineOperand op, TWord w)
        {
            if (op is RegisterStorage r)
            {
                WriteRegister(r, w);
                return;
            }
            if (op is MemoryOperand m)
            {
                var ea = GetEffectiveAddress(m);
                WriteMemory(w, ea, op.DataType);
                return;
            }
            throw new NotImplementedException();
        }

        public sealed override ulong WriteRegister(RegisterStorage r, ulong value)
        {
            Registers[r.Number] = Registers[r.Number] & ~r.BitMask | value << (int) r.BitAddress;
            return value;
        }

        public void WriteMemory(TWord w, ulong ea, DataType dt)
        {
            switch (dt.Size)
            {
            case 1: WriteByte(ea, (byte) w); return;
            case 2: WriteLeUInt16(ea, (ushort) w); return;
            case 4: WriteLeUInt32(ea,  w); return;
            case 8: throw new NotImplementedException();
            }
            throw new InvalidOperationException();
        }

        protected Address XferTarget(MachineOperand op)
        {
            if (op is Address addr)
            {
                return addr;
            }
            else if (op is Constant immediate)
            {
                var addrNew = InstructionPointer.NewOffset(immediate.ToUInt64());
                return addrNew;
            }
            else
            {
                TWord l = Read(op);
                return AddressFromWord(l);
            }
        }

        private void Adc(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            var mask = masks[dst.DataType.Size];
            TWord sum = l + r + ((uint) Flags & 1) & mask.value;
            Write(dst, sum);
            var newCy =
                (l & r | (l | r) & ~sum) >> 31;

            uint ov = (~(l ^ r) & (l ^ sum) & 0x80000000u) >> 20;
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                newCy |       // Carry
                (sum == 0 ? 1u << 6 : 0u) | // Zero
                ((sum & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov                        // Overflow
                ;
        }

        private void Add(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.DataType.Size];
            TWord sum = l + r & mask.value;
            Write(dst, sum);
            uint ov = (~(l ^ r) & (l ^ sum) & mask.hibit) >> 20;
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                (r > sum ? 1u : 0u) |     // Carry
                (sum == 0 ? 1u << 6 : 0u) |                 // Zero
                ((sum & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov                        // Overflow
                ;
        }

        private void Rep()
        {
            var strInstr = dasm.Current;
            ignoreRep = true;
            var c = ReadRegister(cxReg);
            while (c != 0)
            {
                Execute(strInstr);
                --c;
                WriteRegister(cxReg, c);
            }
            ignoreRep = false;
        }

        // Repeat while Z flag is set.
        private void Repe()
        {
            var strInstr = dasm.Current;
            ignoreRep = true;
            var c = ReadRegister(cxReg);
            while (c != 0)
            {
                // A faithful simulation would handle 
                // pending interrupts.
                Execute(strInstr);
                --c;
                WriteRegister(cxReg, c);
                if ((Flags & Zmask) == 0)
                    break;
            }
            ignoreRep = false;
        }

        private void Repne()
        {
            var strInstr = dasm.Current;
            ignoreRep = true;
            var c = ReadRegister(cxReg);
            while (c != 0)
            {
                // Note: a more faithful simulation would 
                // check for pending interrupts.
                Execute(strInstr);
                --c;
                WriteRegister(cxReg, c);
                if ((Flags & Zmask) != 0)
                    break;
            }
            ignoreRep = false;
        }

        protected abstract void Ret();

        protected abstract void Retf();

        private void Rcl(MachineOperand dst, MachineOperand src)
        {
            //$TODO: 64-bit will be harder.
            TWord l = Read(dst) << 1; // Make space for inbound carry bit.
            if ((Flags & Cmask) != 0)
                l |= 1;
            byte sh = (byte) Read(src);
            TWord r = l << sh | l >> dst.DataType.BitSize + 1 - sh;
            var mask = masks[dst.DataType.Size];
            Write(dst, r >> 1 & mask.value);
            Flags &= ~(Cmask | Zmask);
            Flags |=
                ((r & ~1) == 0 ? Zmask : 0u) |  // Zero
                ((r & 1) != 0 ? Cmask : 0u);    // Carry
        }

        private void Rol(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            TWord r = l << sh | l >> 32 - sh;
            Write(dst, r);
            Flags &= ~Zmask;
            Flags |=
                r == 0 ? Zmask : 0u;      // Zero
        }

        protected abstract void Lods(X86Instruction instr);
        protected abstract void Movs(X86Instruction instr);

        protected abstract void Scas(X86Instruction instr);
        protected abstract void Stos(X86Instruction instr);

        private void Sar(MachineOperand dst, MachineOperand src)
        {
            ulong n = Read(dst);
            long l = (long) Bits.SignExtend(n, dst.DataType.BitSize);
            byte sh = (byte) Read(src);
            var mask = masks[dst.DataType.Size];
            TWord r = (TWord) (l >> sh & mask.value);
            Write(dst, r);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & mask.hibit) != 0 ? Smask : 0u);   // Sign
        }

        private void Shl(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            var (value, hibit) = masks[dst.DataType.Size];
            TWord r = l << sh & value;
            Write(dst, r);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & hibit) != 0 ? Smask : 0u);   // Sign
        }

        private void Shr(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            var mask = masks[dst.DataType.Size];
            TWord r = l >> sh & mask.value;
            Write(dst, r);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                l >> sh - 1 & 1 |                   // Carry
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & mask.hibit) != 0 ? Smask : 0u);   // Sign
        }

        protected abstract void Call(MachineOperand op);

        private void Clc()
        {
            Flags &= ~Cmask;
        }

        private void Cld()
        {
            Flags &= ~Dmask;
        }

        protected void Jcxz(MachineOperand op)
        {
            TWord cx = (TWord) ReadRegister(X86.Registers.cx);
            if (cx == 0)
            {
                InstructionPointer = XferTarget(op);
            }
        }

        protected void Jump(MachineOperand op)
        {
            InstructionPointer = XferTarget(op);
        }

        private void Cmp(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            r = ~r + 1u;
            var mask = masks[dst.DataType.Size];
            TWord diff = l + r & mask.value;
            uint ov = (~(l ^ r) & (l ^ diff) & mask.hibit) >> 20;
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                ((diff & mask.hibit) != 0 ? Smask : 0u) |   // Sign
                ov                        // Overflow
                ;
        }

        private void Sub(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            r = ~r + 1u;        // Two's complement subtraction.
            var mask = masks[dst.DataType.Size];
            TWord diff = l + r & mask.value;
            Write(dst, diff);
            uint ov = (~(l ^ r) & (l ^ diff) & mask.hibit) >> 20;
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                ((diff & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov                        // Overflow
                ;
        }

        private void And(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.DataType.Size];
            var and = l & r & mask.value;
            Write(dst, and);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                0 |                         // Clear Carry
                (and == 0 ? Zmask : 0u) |    // Zero
                ((and & mask.hibit) != 0 ? Smask : 0u) | // Sign
                0;                          // Clear Overflow
        }

        private void Not(MachineOperand op)
        {
            TWord v = Read(op);
            var mask = masks[op.DataType.Size];
            var not = ~v & mask.value;
            Write(op, not);
            // Flags are not affected according to Intel docs.
        }

        private void Or(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.DataType.Size];
            var or = (l | r) & mask.value;
            Write(dst, or);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                0 |                         // Clear Carry
                (or == 0 ? Zmask : 0u) |    // Zero
                ((or & mask.hibit) != 0 ? Smask : 0u) | // Sign
                0;                          // Clear Overflow
        }

        private void Dec(MachineOperand op)
        {
            TWord old = Read(op);
            var mask = masks[op.DataType.Size];
            TWord gnu = old - 1 & mask.value;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & ~gnu & mask.hibit) >> 20;
            Flags &= ~(Zmask | Smask | Omask);
            Flags |=
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ((gnu & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov;                          //$BUG:
        }

        private void Inc(MachineOperand op)
        {
            TWord old = Read(op);
            var mask = masks[op.DataType.Size];
            TWord gnu = old + 1 & mask.value;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & gnu & mask.hibit) >> 20;
            Flags &= ~(Zmask | Smask | Omask);
            Flags |=
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ((gnu & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov;                          //$BUG:
        }

        public void Loop(MachineOperand op)
        {
            var c = ReadRegister(cxReg) - 1u;
            WriteRegister(cxReg, c);
            if (c != 0)
            {
                InstructionPointer = XferTarget(op);
            }
        }

        public void Popa()
        {
            var dt = arch.WordWidth;
            Registers[X86.Registers.edi.Number] = Pop(dt);
            Registers[X86.Registers.esi.Number] = Pop(dt);
            Registers[X86.Registers.ebp.Number] = Pop(dt);
            Pop(dt);
            Registers[X86.Registers.ebx.Number] = Pop(dt);
            Registers[X86.Registers.edx.Number] = Pop(dt);
            Registers[X86.Registers.ecx.Number] = Pop(dt);
            Registers[X86.Registers.eax.Number] = Pop(dt);
        }

        public void Pusha()
        {
            var dt = PrimitiveType.Word32;
            var temp = (uint) Registers[X86.Registers.esp.Number];
            Push((uint) Registers[X86.Registers.eax.Number], dt);
            Push((uint) Registers[X86.Registers.ecx.Number], dt);
            Push((uint) Registers[X86.Registers.edx.Number], dt);
            Push((uint) Registers[X86.Registers.ebx.Number], dt);
            Push(temp, dt);
            Push((uint) Registers[X86.Registers.ebp.Number], dt);
            Push((uint) Registers[X86.Registers.esi.Number], dt);
            Push((uint) Registers[X86.Registers.edi.Number], dt);
        }

        protected abstract TWord Pop(DataType dt);

        public void Push(MachineOperand op)
        {
            var value = Read(op);
            Push(value, op.DataType);
        }

        protected abstract void Push(ulong dw, DataType dt);

        private void Stc()
        {
            Flags |= Cmask;
        }

        private void Std()
        {
            Flags |= Dmask;
        }

        private void Test(MachineOperand op1, MachineOperand op2)
        {
            TWord l = Read(op1);
            TWord r = Read(op2);
            if (op2.DataType.Size < op1.DataType.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[op1.DataType.Size];
            var test = l & r & mask.value;
            Flags &= ~(Cmask | Zmask | Smask | Omask);      //$TODO: parity.
            Flags |=
                0 |                             // Clear carry
                (test == 0 ? Zmask : 0u) |      // Zero
                ((test & mask.hibit) != 0 ? Smask : 0u) | // Sign
                0;                              // Clear overflow
        }

        private void Xchg(MachineOperand op1, MachineOperand op2)
        {
            var tmp = Read(op1);
            Write(op1, Read(op2));
            Write(op2, tmp);
        }

        private void Xor(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.DataType.Size < dst.DataType.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.DataType.Size];
            var xor = (l ^ r) & mask.value;
            Write(dst, xor);
            Flags &= ~(Cmask | Zmask | Smask | Omask);
            Flags |=
                0 |                         // Carry
                (xor == 0 ? Zmask : 0u) |   // Zero
                ((xor & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                0;                          // Overflow
        }
    }
}