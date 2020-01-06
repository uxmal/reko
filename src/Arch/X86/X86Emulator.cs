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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
{
    using TWord = System.UInt32;

    /// <summary>
    /// Simple emulator of X86 instructions. No attempt is made to be high-performance
    /// as long as correctness is maintained.
    /// </summary>
    public class X86Emulator : IProcessorEmulator
    {
        public event EventHandler BeforeStart;
        public event EventHandler ExceptionRaised;

        public const uint Cmask = 1u << 0;
        public const uint Zmask = 1u << 6;
        public const uint Omask = 1u << 11;

        private IntelArchitecture arch;
        private SegmentMap map;
        private IPlatformEmulator envEmulator;
        private IEnumerator<X86Instruction> dasm;
        private bool running;
        private Dictionary<uint, Action> bpExecute = new Dictionary<uint, Action>();

        public readonly ulong[] Registers;
        public readonly bool[] Valid;
        public TWord Flags;
        private Address ip;
        private Action stepAction;
        private bool stepInto;
        private TWord stepOverAddress;
        private bool ignoreRep;

        public X86Emulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            this.arch = arch;
            this.map = segmentMap;
            this.Registers = new ulong[40];
            this.Valid = new bool[40];
            this.envEmulator = envEmulator;
        }

        /// <summary>
        /// The current instruction pointer of the emulator.
        /// </summary>
        public Address InstructionPointer
        {
            get { return ip; }
            set
            {
                ip = value;
                if (!map.TryFindSegment(ip, out ImageSegment segment))
                    throw new AccessViolationException();
                var rdr = arch.CreateImageReader(segment.MemoryArea, value);
                dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
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
            stepOverAddress = (TWord)((long)dasm.Current.Address.ToLinear() + dasm.Current.Length);
            stepAction = callback;
        }

        public void StepInto(Action callback)
        {
            stepInto = true;
            stepAction = callback;
        }

        public void Start()
        {
            running = true;
            CreateStack();
            BeforeStart.Fire(this);
            Run();
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

        private void Run()
        {
            int counter = 0;
            try
            {
                while (running && dasm.MoveNext())
                {
                    //                Debug.Print("emu: {0} {1,-15} {2}", dasm.Current.Address, dasm.Current, DumpRegs());
                    TWord eip = (uint) dasm.Current.Address.ToLinear();
                    if (bpExecute.TryGetValue(eip, out Action bpAction))
                    {
                        ++counter;
                        stepOverAddress = 0;
                        stepInto = false;
                        bpAction();
                        if (!running)
                            break;
                    }
                    else if (stepInto)
                    {
                        stepInto = false;
                        var s = stepAction;
                        stepAction = null;
                        s();
                        if (!running)
                            break;
                    }
                    else if (stepOverAddress == eip)
                    {
                        stepOverAddress = 0;
                        var s = stepAction;
                        stepAction = null;
                        s();
                        if (!running)
                            break;
                    }
                    Execute(dasm.Current);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Emulator exception when executing {0}. {1}\r\n{2}", dasm.Current, ex.Message, ex.StackTrace);
                ExceptionRaised.Fire(this);
            }
        }

        public void Stop()
        {
            running = false;
        }

        public void CreateStack()
        {

        }

        public void Execute(X86Instruction instr)
        {
            if (instr.repPrefix == 2 && !ignoreRep)
            {
                // repne
                switch (instr.code)
                {
                case Mnemonic.scasb: Repne(); return;
                }
                throw new NotImplementedException();
            }
            switch (instr.code)
            {
            default:
                throw new NotImplementedException(string.Format("Instruction emulation for {0} not implemented yet.", instr));
            case Mnemonic.adc: Adc(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.add: Add(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.and: And(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.call: Call(instr.Operands[0]); return;
            case Mnemonic.cmp: Cmp(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.dec: Dec(instr.Operands[0]); return;
            case Mnemonic.hlt: running = false; return;
            case Mnemonic.inc: Inc(instr.Operands[0]); return;
            case Mnemonic.ja: if ((Flags & (Cmask | Zmask)) == 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.jbe: if ((Flags & (Cmask | Zmask)) != 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.jc: if ((Flags & Cmask) != 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.jmp: Jump(instr.Operands[0]); return;
            case Mnemonic.jnc: if ((Flags & Cmask) == 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.jnz: if ((Flags & Zmask) == 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.jz: if ((Flags & Zmask) != 0) InstructionPointer = ((AddressOperand)instr.Operands[0]).Address; return;
            case Mnemonic.lea: Write(instr.Operands[0], GetEffectiveAddress((MemoryOperand)instr.Operands[1])); break;
            case Mnemonic.loop: Loop(instr.Operands[0]); break;
            case Mnemonic.mov: Write(instr.Operands[0], Read(instr.Operands[1])); break;
            case Mnemonic.or: Or(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.pop: Write(instr.Operands[0], Pop()); return;
            case Mnemonic.popa: Popa(); return;
            case Mnemonic.push: Push(Read(instr.Operands[0])); return;
            case Mnemonic.pusha: Pusha(); return;
            case Mnemonic.rol: Rol(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.scasb: Scasb(); return;
            case Mnemonic.shl: Shl(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.shr: Shr(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.sub: Sub(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xor: Xor(instr.Operands[0], instr.Operands[1]); return;
            case Mnemonic.xchg: Xchg(instr.Operands[0], instr.Operands[1]); return;
            }
        }

        private void Adc(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            TWord sum = l + r + (Flags & 1);
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
                r = (TWord)(sbyte)r;
            TWord sum = l + r;
            Write(dst, sum);
            uint ov = ((~(l ^ r) & (l ^ sum)) & 0x80000000u) >> 20;
            Flags =
                (r > sum ? 1u : 0u) |     // Carry
                (sum == 0 ? 1u << 6: 0u)  | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Repne()
        {
            var strInstr = dasm.Current;
            this.ignoreRep = true;
            uint ecx = ReadRegister(X86.Registers.ecx);
            while (ecx != 0)
            {
                Execute(strInstr);
                --ecx;
                if ((Flags & Zmask) != 0)
                    break;
            }
            WriteRegister(X86.Registers.ecx, ecx);
            this.ignoreRep = false;
        }

        private void Rol(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = (l << sh) | (l >> (32 - sh));
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Scasb()
        {
            //$TODO repne
            byte al = (byte) ReadRegister(X86.Registers.al);
            TWord edi = ReadRegister(X86.Registers.edi);
            var addr = Address.Ptr32(edi);
            if (!map.TryFindSegment(addr, out ImageSegment seg))
                throw new AccessViolationException();
            byte mem = (byte)(al - seg.MemoryArea.Bytes[edi - (uint)seg.MemoryArea.BaseAddress.ToLinear()]);
            WriteRegister(X86.Registers.edi, edi + 1);      //$BUG: Direction flag not respected
            Flags =
                (mem == 0 ? Zmask : 0u);
        }

        private void Shl(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = l << sh;
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Shr(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = l >> sh;
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Call(MachineOperand op)
        {
            Push(InstructionPointer.ToLinear() + (uint)dasm.Current.Length);   // Push return value on stack
      
            TWord l = Read(op);
            if (envEmulator.InterceptCall(this, l))
                return;
             InstructionPointer = Address.Ptr32(l);
        }

        private void Jump(MachineOperand op)
        {
            TWord l = Read(op);
            InstructionPointer = Address.Ptr32(l);
        }

        private void Cmp(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            r = ~r + 1u;
            TWord diff = l + r;
            uint ov = ((~(l ^ r) & (l ^ diff)) & 0x80000000u) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
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
            TWord diff = l + r;
            Write(dst, diff);
            uint ov = ((~(l ^ r) & (l ^ diff)) & 0x80000000u) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }


        private void And(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var and = l & r;
            Write(dst, and);
            Flags =
                0 |                         // Clear Carry
                (and == 0 ? Zmask : 0u) |    // Zero
                0;                          // Clear Overflow
        }

        private void Or(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var or = l | r;
            Write(dst, or);
            Flags =
                0 |                         // Clear Carry
                (or == 0 ? Zmask : 0u) |    // Zero
                0;                          // Clear Overflow
        }


        private void Dec(MachineOperand op)
        {
            TWord old = Read(op);
            TWord gnu = old - 1;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & ~gnu & 0x80000000u) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ov;                          //$BUG:
        }
        private void Inc(MachineOperand op)
        {
            TWord old = Read(op);
            TWord gnu = old + 1;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & gnu & 0x80000000u) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ov;                          //$BUG:
        }

        private void Xor(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var xor = l ^ r;
            Write(dst, xor);
            Flags =
                0 |                         // Carry
                (xor == 0 ? Zmask : 0u) |   // Zero
                0;                          // Overflow
        }

        private TWord Read(MachineOperand op)
        {
            if (op is RegisterOperand r)
            {
                return ReadRegister(r.Register);
            }
            if (op is ImmediateOperand i)
                return i.Value.ToUInt32();
            if (op is AddressOperand a)
                return a.Address.ToUInt32();
            if (op is MemoryOperand m)
            {
                TWord ea = GetEffectiveAddress(m);
                byte b;
                switch (op.Width.Size)
                {
                case 1: if (!TryReadByte(Address.Ptr32(ea), out b)) throw new IndexOutOfRangeException(); else return b;
                case 4: return ReadLeUInt32(Address.Ptr32(ea));
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private TWord GetEffectiveAddress(MemoryOperand m)
        {
            TWord ea = 0;
            if (m.Offset.IsValid)
                ea += m.Offset.ToUInt32();
            if (m.Index != RegisterStorage.None)
                ea += ReadRegister(m.Index) * m.Scale;
            if (m.Base != null && m.Base != RegisterStorage.None)
            {
                ea += ReadRegister(m.Base);
            }
            return ea;
        }

        public TWord ReadRegister(RegisterStorage r)
        {
            return (TWord)(Registers[r.Number] & r.BitMask) >> (int)r.BitAddress;
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
                switch (op.Width.Size)
                {
                case 1: WriteByte(Address.Ptr32(ea), (byte) w); return;
                case 4: WriteLeUInt32(Address.Ptr32(ea), (UInt32) w); return;
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public void WriteRegister(RegisterStorage r, TWord value)
        {
            Registers[r.Number] = (Registers[r.Number] & ~r.BitMask) | (value << (int)r.BitAddress);
        }

        public void Loop(MachineOperand op)
        {
            var c = ReadRegister(X86.Registers.ecx)  -1u;
            WriteRegister(X86.Registers.ecx, c);
            if (c != 0)
                InstructionPointer = ((AddressOperand)op).Address;
        }

        public void Popa()
        {
            Registers[X86.Registers.edi.Number] = Pop();
            Registers[X86.Registers.esi.Number] = Pop();
            Registers[X86.Registers.ebp.Number] = Pop();
            Pop();
            Registers[X86.Registers.ebx.Number] = Pop();
            Registers[X86.Registers.edx.Number] = Pop();
            Registers[X86.Registers.ecx.Number] = Pop();
            Registers[X86.Registers.eax.Number] = Pop();
        }

        public void Pusha()
        {
            var temp = Registers[X86.Registers.esp.Number];
            Push(Registers[X86.Registers.eax.Number]);
            Push(Registers[X86.Registers.ecx.Number]);
            Push(Registers[X86.Registers.edx.Number]);
            Push(Registers[X86.Registers.ebx.Number]);
            Push(temp);
            Push(Registers[X86.Registers.ebp.Number]);
            Push(Registers[X86.Registers.esi.Number]);
            Push(Registers[X86.Registers.edi.Number]);
        }

        public TWord Pop()
        {
            var esp = Registers[X86.Registers.esp.Number];
            var word = ReadLeUInt32((uint)esp);
            esp += 4;
            WriteRegister(X86.Registers.esp, (uint)esp);
            return word;
        }

        public void Push(ulong word)
        {
            var esp = (uint)Registers[X86.Registers.esp.Number] - 4;
            WriteLeUInt32(Address.Ptr32(esp), (uint) word);
            WriteRegister(X86.Registers.esp, (uint) esp);
        }

        private void Xchg(MachineOperand op1, MachineOperand op2)
        {
            var tmp = Read(op1);
            Write(op1, Read(op2));
            Write(op2, tmp);
        }

        private bool TryReadByte(Address ea, out byte b)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            if (!map.TryFindSegment(ea, out ImageSegment segment))
                throw new AccessViolationException();
            return segment.MemoryArea.TryReadByte(ea, out b);
        }

        private uint ReadLeUInt32(uint ea)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            var addr = Address.Ptr32(ea);
            ImageSegment segment;
            if (!map.TryFindSegment(addr, out segment))
                throw new AccessViolationException();
            return segment.MemoryArea.ReadLeUInt32(addr);
        }

        private uint ReadLeUInt32(Address ea)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            ImageSegment segment;
            if (!map.TryFindSegment(ea, out segment))
                throw new AccessViolationException();
            return segment.MemoryArea.ReadLeUInt32(ea);
        }

        private void WriteLeUInt32(Address ea, uint value)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            ImageSegment segment;
            if (!map.TryFindSegment(ea, out segment))
                throw new AccessViolationException();
            segment.MemoryArea.WriteLeUInt32(ea, value);
        }

        private void WriteLeUInt32(uint ea, uint value)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            var addr = Address.Ptr32(ea);
            ImageSegment segment;
            if (!map.TryFindSegment(addr, out segment))
                throw new AccessViolationException();
            segment.MemoryArea.WriteLeUInt32(addr, value);
        }

        private void WriteByte(Address ea, byte value)
        {
            //$PERF: wow this is inefficient; an allocation
            // per memory fetch. TryFindSegment needs an overload
            // that accepts ulongs / linear addresses.
            ImageSegment segment;
            if (!map.TryFindSegment(ea, out segment))
                throw new AccessViolationException();
            segment.MemoryArea.WriteByte(ea, value);
        }

        public void SetBreakpoint(uint address, Action callback)
        {
            bpExecute.Add(address, callback);
        }

        public void DeleteBreakpoint(uint address)
        {
            bpExecute.Remove(address);
        }
    }
}