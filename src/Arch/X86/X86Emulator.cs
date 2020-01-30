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

using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reko.Arch.X86
{
    using TWord = System.UInt32;

    /// <summary>
    /// Simple emulator of X86 instructions. No attempt is made to be high-performance
    /// as long as correctness is maintained.
    /// </summary>
    public abstract class X86Emulator : EmulatorBase
    {
        public const uint Cmask = 1u << 0;
        public const uint Zmask = 1u << 6;
        public const uint Smask = 1u << 7;
        public const uint Dmask = 1u << 10;
        public const uint Omask = 1u << 11;

        protected static readonly TraceSwitch trace = new TraceSwitch(nameof(X86Emulator), "Trace execution of X86 Emulator") { Level = TraceLevel.Warning };
        public static readonly (uint value, uint hibit)[] masks = new(uint, uint)[]{
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
            this.map = segmentMap;
            this.ipReg = ipReg;
            this.cxReg = cxReg;
            this.Registers = new ulong[40];
            this.iFlags = X86.Registers.eflags.Number;
            this.envEmulator = envEmulator;
        }

        public override MachineInstruction CurrentInstruction => dasm.Current;

        public ulong Flags
        {
            get { return this.Registers[iFlags]; }
            set { this.Registers[iFlags] = value; }
        }

        /// <summary>
        /// The current instruction pointer of the emulator.
        /// </summary>
        public override Address InstructionPointer
        {
            get { return ip; }
            set
            {
                UpdateIp(value);
                if (!map.TryFindSegment(ip, out ImageSegment segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateIp(Address value)
        {
            this.ip = value;
            WriteRegister(this.ipReg, (TWord)value.Offset);
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
                sb.AppendFormat(" {0} {1:X8}", arch.GetRegister(i + StorageDomain.Register, new BitRange(0, 64)).Name, Registers[i]);
            }
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
            Debug.Print("emu: {0} {1,-15} {2}", dasm.Current.Address, dasm.Current, DumpRegs());
        }

        public void Execute(X86Instruction instr)
        {
            if (!ignoreRep)
            {
                if (instr.repPrefix == 2)
                {
                // repne
                switch (instr.Mnemonic)
                {
                case Mnemonic.scasb: Repne(); return;
                }
                throw new NotImplementedException();
            }
                else if (instr.repPrefix == 3)
                {
                    // rep / repe
            switch (instr.Mnemonic)
            {
                    case Mnemonic.lods: Rep(); return;
                    case Mnemonic.lodsb: Rep(); return;
                    case Mnemonic.movs: Rep(); return;
                    case Mnemonic.movsb: Rep(); return;
                    case Mnemonic.stosb: Rep(); return;
                    }
                    throw new NotImplementedException();
                }
            }
            switch (instr.Mnemonic)
            {
            default:
                throw new NotImplementedException(string.Format("Instruction emulation for {0} not implemented yet.", instr));
            case Mnemonic.adc: Adc(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.add: Add(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.and: And(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.call: Call(instr.Operands[0]); return;
            case Mnemonic.cld: Cld(); return;
            case Mnemonic.cmp: Cmp(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.dec: Dec(instr.Operands[0]); return;
            case Mnemonic.hlt: Stop(); return;
            case Mnemonic.inc: Inc(instr.Operands[0]); return;
            case Mnemonic.ja: if ((Flags & (Cmask | Zmask)) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jbe: if ((Flags & (Cmask | Zmask)) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jc: if ((Flags & Cmask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jmp: Jump(instr.Operands[0]); return;
            case Mnemonic.jnc: if ((Flags & Cmask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jns: if ((Flags & Smask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jnz: if ((Flags & Zmask) == 0) Jump(instr.Operands[0]); return;
            case Mnemonic.js: if ((Flags & Smask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.jz: if ((Flags & Zmask) != 0) Jump(instr.Operands[0]); return;
            case Mnemonic.lea: Write(instr.Operands[0], GetEffectiveOffset((MemoryOperand) instr.Operands[1])); break;
            case Mnemonic.lodsb: Lods(PrimitiveType.Byte); break;
            case Mnemonic.lods: Lods(instr.dataWidth); break;
            case Mnemonic.loop: Loop(instr.Operands[0]); break;
            case Mnemonic.mov: Write(instr.Operands[0], Read(instr.Operands[1])); break;
            case Mnemonic.movs: Movs(instr.dataWidth); break;
            case Mnemonic.movsb: Movs(PrimitiveType.Byte); break;
            case Mnemonic.or: Or(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.pop: Write(instr.Operands[0], Pop(instr.Operands[0].Width)); return;
            case Mnemonic.popa: Popa(); return;
            case Mnemonic.push: Push(instr.Operands[0]); return;
            case Mnemonic.pusha: Pusha(); return;
            case Mnemonic.ret: Ret(); return;
            case Mnemonic.retf: Retf(); return;
            case Mnemonic.rcl: Rcl(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.rol: Rol(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.sar: Sar(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.scasb: Scasb(); return;
            case Mnemonic.shl: Shl(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.shr: Shr(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.stc: Flags |= Cmask; break;
            case Mnemonic.stosb: Stos(PrimitiveType.Byte); break;
            case Mnemonic.sub: Sub(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xor: Xor(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xchg: Xchg(instr.Operands[0], instr.Operands[1]); return;
            }
        }

        protected abstract ulong GetEffectiveAddress(MemoryOperand m);

        protected TWord GetEffectiveOffset(MemoryOperand m)
        {
            TWord ea = 0;
            if (m.Offset.IsValid)
                ea += m.Offset.ToUInt32();
            if (m.Index != RegisterStorage.None)
                ea += (TWord)ReadRegister(m.Index) * m.Scale;
            if (m.Base != null && m.Base != RegisterStorage.None)
            {
                ea += (TWord) ReadRegister(m.Base);
            }
            return ea;
        }

        private TWord Read(MachineOperand op)
        {
            if (op is RegisterOperand r)
            {
                return (TWord) ReadRegister(r.Register);
            }
            if (op is ImmediateOperand i)
                return i.Value.ToUInt32();
            if (op is AddressOperand a)
                return a.Address.ToUInt32();
            if (op is MemoryOperand m)
            {
                ulong ea = GetEffectiveAddress(m);
                return ReadMemory(ea, m.Width);
            }
            throw new NotImplementedException();
        }

        public override ulong ReadRegister(RegisterStorage r)
        {
            return (Registers[r.Number] & r.BitMask) >> (int) r.BitAddress;
        }

        public TWord ReadMemory(ulong ea, PrimitiveType dt)
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
            if (op is RegisterOperand r)
            {
                WriteRegister(r.Register, w);
                return;
            }
            if (op is MemoryOperand m)
            {
                var ea = GetEffectiveAddress(m);
                WriteMemory(w, ea, op.Width);
                return;
            }
            throw new NotImplementedException();
        }

        public override ulong WriteRegister(RegisterStorage r, ulong value)
        {
            Registers[r.Number] = (Registers[r.Number] & ~r.BitMask) | (value << (int) r.BitAddress);
            return value;
        }

        public void WriteMemory(TWord w, ulong ea, PrimitiveType dt)
        {
            switch (dt.Size)
            {
            case 1: WriteByte(ea, (byte) w); return;
            case 2: WriteLeUInt16(ea, (ushort) w); return;
            case 4: WriteLeUInt32(ea, (uint) w); return;
            case 8: throw new NotImplementedException();
            }
            throw new InvalidOperationException();
        }

        protected Address XferTarget(MachineOperand op)
        {
            if (op is AddressOperand a)
            {
                return a.Address;
            }
            else if (op is ImmediateOperand immediate)
            {
                var addrNew = InstructionPointer.NewOffset(immediate.Value.ToUInt64());
                return addrNew;
            }
            else
            {
                //$BUG: not correct for x86-real
                TWord l = Read(op);
                return Address.Ptr32(l);
            }
        }

        private void Adc(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            var mask = masks[dst.Width.Size];
            TWord sum = (l + r + ((uint)Flags & 1)) & mask.value;
            Write(dst, sum);
            var newCy =
                ((l & r) | ((l | r) & (~(sum)))) >> 31;

            uint ov = ((~(l ^ r) & (l ^ sum)) & 0x80000000u) >> 20;
            Flags =
                (newCy) |       // Carry
                (sum == 0 ? 1u << 6 : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Add(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.Width.Size];
            TWord sum = (l + r) & mask.value;
            Write(dst, sum);
            uint ov = ((~(l ^ r) & (l ^ sum)) & mask.hibit) >> 20;
            Flags =
                (r > sum ? 1u : 0u) |     // Carry
                (sum == 0 ? 1u << 6 : 0u) |                 // Zero
                ((sum & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                (ov)                        // Overflow
                ;
        }

        private void Rep()
        {
            var strInstr = dasm.Current;
            this.ignoreRep = true;
            var c = ReadRegister(cxReg);
            while (c != 0)
            {
                Execute(strInstr);
                --c;
                WriteRegister(cxReg, c);
            }
            this.ignoreRep = false;
        }

        private void Repne()
        {
            var strInstr = dasm.Current;
            this.ignoreRep = true;
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
            this.ignoreRep = false;
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
            TWord r = (l << sh) | (l >> (dst.Width.BitSize + 1 - sh));
            var mask = masks[dst.Width.Size];
            Write(dst, (r >> 1) & mask.value);
            Flags =
                ((r & ~1) == 0 ? Zmask : 0u) |  // Zero
                ((r & 1) != 0 ? Cmask : 0u);    // Carry
        }

        private void Rol(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            TWord r = (l << sh) | (l >> (32 - sh));
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        protected abstract void Lods(PrimitiveType dt);
        protected abstract void Movs(PrimitiveType dt);
        protected abstract void Stos(PrimitiveType dt);

        private void Scasb()
        {
            //$TODO repne
            byte al = (byte) ReadRegister(X86.Registers.al);
            var edi = ReadRegister(X86.Registers.edi);
            var addr = Address.Ptr32((TWord)edi);
            if (!map.TryFindSegment(addr, out ImageSegment seg))
                throw new AccessViolationException();
            byte mem = (byte) (al - seg.MemoryArea.Bytes[edi - (uint) seg.MemoryArea.BaseAddress.ToLinear()]);
            WriteRegister(X86.Registers.edi, edi + 1);      //$BUG: Direction flag not respected
            Flags =
                (mem == 0 ? Zmask : 0u);
        }

        private void Sar(MachineOperand dst, MachineOperand src)
        {
            ulong n = Read(dst);
            long l = (long) Bits.SignExtend(n, dst.Width.BitSize);
            byte sh = (byte) Read(src);
            var mask = masks[dst.Width.Size];
            TWord r = (TWord)((l >> sh) & mask.value);
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & mask.hibit) != 0 ? Smask : 0u);   // Sign
        }

        private void Shl(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            var mask = masks[dst.Width.Size];
            TWord r = (l << sh) & mask.value;
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & mask.hibit) != 0 ? Smask : 0u);   // Sign
        }

        private void Shr(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte) Read(src);
            var mask = masks[dst.Width.Size];
            TWord r = (l >> sh) & mask.value;
            Write(dst, r);
            Flags =
                ((l >> (sh-1)) & 1) |                   // Carry
                (r == 0 ? Zmask : 0u) |                 // Zero
                ((r & mask.hibit) != 0 ? Smask : 0u);   // Sign
        }

        protected abstract void Call(MachineOperand op);

        private void Cld()
        {
            Flags &= ~Dmask;
        }

        protected void Jump(MachineOperand op)
        {
            InstructionPointer = XferTarget(op);
        }

        private void Cmp(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            r = ~r + 1u;
            var mask = masks[dst.Width.Size];
            TWord diff = (l + r) & mask.value;
            uint ov = ((~(l ^ r) & (l ^ diff)) & mask.hibit) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                ((diff & mask.hibit) != 0 ? Smask : 0u) |   // Sign
                (ov)                        // Overflow
                ;
        }

        private void Sub(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            r = ~r + 1u;        // Two's complement subtraction.
            var mask = masks[dst.Width.Size];
            TWord diff = (l + r) & mask.value;
            Write(dst, diff);
            uint ov = ((~(l ^ r) & (l ^ diff)) & mask.hibit) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                ((diff & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                (ov)                        // Overflow
                ;
        }

        private void And(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var mask = masks[dst.Width.Size];
            var and = (l & r) & mask.value;
            Write(dst, and);
            Flags =
                0 |                         // Clear Carry
                (and == 0 ? Zmask : 0u) |    // Zero
                ((and & mask.hibit) != 0 ? Smask : 0u) | // Sign
                0;                          // Clear Overflow
        }

        private void Or(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var mask = masks[dst.Width.Size];
            var or = (l | r) & mask.value;
            Write(dst, or);
            Flags =
                0 |                         // Clear Carry
                (or == 0 ? Zmask : 0u) |    // Zero
                ((or & mask.hibit) != 0 ? Smask : 0u) | // Sign
                0;                          // Clear Overflow
        }

        private void Dec(MachineOperand op)
        {
            TWord old = Read(op);
            var mask = masks[op.Width.Size];
            TWord gnu = (old - 1) & mask.value;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & ~gnu & mask.hibit) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ((gnu & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                ov;                          //$BUG:
        }

        private void Inc(MachineOperand op)
        {
            TWord old = Read(op);
            var mask = masks[op.Width.Size];
            TWord gnu = (old + 1) & mask.value;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & gnu & mask.hibit) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
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
            var temp = (uint)Registers[X86.Registers.esp.Number];
            Push((uint)Registers[X86.Registers.eax.Number], dt);
            Push((uint)Registers[X86.Registers.ecx.Number], dt);
            Push((uint)Registers[X86.Registers.edx.Number], dt);
            Push((uint)Registers[X86.Registers.ebx.Number], dt);
            Push(temp, dt);
            Push((uint)Registers[X86.Registers.ebp.Number], dt);
            Push((uint)Registers[X86.Registers.esi.Number], dt);
            Push((uint)Registers[X86.Registers.edi.Number], dt);
        }

        protected abstract TWord Pop(PrimitiveType dt);

        public void Push(MachineOperand op)
        {
            var value = Read(op);
            Push(value, op.Width);
        }

        protected abstract void Push(ulong dw, PrimitiveType dt);

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
            if (src.Width.Size < dst.Width.Size)
                r = (TWord) (sbyte) r;
            var mask = masks[dst.Width.Size];
            var xor = (l ^ r) & mask.value;
            Write(dst, xor);
            Flags =
                0 |                         // Carry
                (xor == 0 ? Zmask : 0u) |   // Zero
                ((xor & mask.hibit) != 0 ? Smask : 0u) |    // Sign
                0;                          // Overflow
        }
        }
}