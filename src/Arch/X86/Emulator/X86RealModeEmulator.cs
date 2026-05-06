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

using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Reko.Arch.X86.Emulator
{
    public class X86RealModeEmulator : X86Emulator
    {
        public X86RealModeEmulator(IntelArchitecture arch, ByteProgramMemory memory, IPlatformEmulator envEmulator)
            : base(arch, memory, envEmulator, arch.Registers.ip, arch.Registers.cx)
        {
        }

        public override Address AddressFromWord(ulong word)
        {
            var off = (ushort) word;
            var seg = (ushort) (word >> 16);
            return Address.SegPtr(seg, off);
        }

        protected override void Call(MachineOperand op)
        {
            if (op.DataType.Size == 4)
            {
                Push(InstructionPointer.Selector!.Value, PrimitiveType.Word16);
            }
            var nextIp = InstructionPointer.Offset + (uint) dasm.Current.Length;   // Push return value on stack
            Push(nextIp, PrimitiveType.Word16);
            var dest = XferTarget(op);
            if (envEmulator.InterceptCall(this, (uint) dest.ToLinear()))
                return;
            InstructionPointer = dest;
        }

        protected override ulong GetEffectiveAddress(MemoryOperand m)
        {
            var segReg = m.SegOverride;
            if (m.SegOverride == RegisterStorage.None)
                segReg = m.DefaultSegment(arch);

            var off = GetEffectiveOffset(m) & 0xFFFF;
            var seg = (ushort) ReadRegister(segReg);
            return ToLinear(seg, off);
        }

        protected override void Lods(X86Instruction instr)
        {
            var ds = ReadSegmentRegister(instr.Operands[1], arch.Registers.ds);
            var si = (uint) ReadRegister(arch.Registers.si);
            var dt = instr.DataWidth;
            var value = ReadMemory(ToLinear(ds, si), dt);
            var mask = masks[dt.Size];
            var a = ReadRegister(X86.Registers.eax);
            var aNew = a & ~mask.value | value;
            WriteRegister(X86.Registers.eax, aNew);
            var delta = (uint) dt.Size * ((Flags & Dmask) != 0 ? 0xFFFFu : 0x0001u);
            si += delta;
            WriteRegister(arch.Registers.si, si);
        }

        protected override void Movs(X86Instruction instr)
        {
            var ds = ReadSegmentRegister(instr.Operands[1], arch.Registers.ds);
            var es = ReadSegmentRegister(instr.Operands[0], arch.Registers.es);
            var si = (uint) ReadRegister(arch.Registers.si);
            var di = (uint) ReadRegister(arch.Registers.di);
            var dt = instr.DataWidth;
            var value = ReadMemory(ToLinear(ds, si), dt);
            WriteMemory(value, ToLinear(es, di), dt);
            var delta = (uint) dt.Size * ((Flags & Dmask) != 0 ? 0xFFFFu : 0x0001u);
            si += delta;
            di += delta;
            WriteRegister(arch.Registers.si, si);
            WriteRegister(arch.Registers.di, di);
        }

        private ushort ReadSegmentRegister(MachineOperand op, RegisterStorage defaultRegister)
        {
            var mem = (MemoryOperand) op;
            var seg = mem.SegOverride is not null && mem.SegOverride != RegisterStorage.None
                ? mem.SegOverride
                : defaultRegister;
            return (ushort) ReadRegister(seg);
        }

        protected override void Scas(X86Instruction instr)
        {
            var dt = instr.DataWidth;
            var mask = masks[dt.Size];
            var a = ReadRegister(X86.Registers.eax) & mask.value;
            var es = ReadSegmentRegister(instr.Operands[1], arch.Registers.es);
            var di = (uint) ReadRegister(arch.Registers.di);
            var value = ReadMemory(ToLinear(es, di), dt) & mask.value;
            var delta = dt.Size * ((Flags & Dmask) != 0 ? -1 : 1);
            di += (uint) delta;
            WriteRegister(arch.Registers.di, di);
            Flags &= ~Zmask;
            Flags |= a == value ? Zmask : 0u;
        }

        protected override void Stos(X86Instruction instr)
        {
            var dt = instr.DataWidth;
            var es = ReadSegmentRegister(instr.Operands[0], arch.Registers.es);
            var di = (uint) ReadRegister(arch.Registers.di);
            var value = (uint) (Registers[X86.Registers.rax.Number] & Bits.Mask(0, dt.BitSize));
            WriteMemory(value, ToLinear(es, di), dt);
            var delta = (uint) dt.Size * ((Flags & Dmask) != 0 ? 0xFFFFu : 0x0001u);
            di += delta;
            WriteRegister(arch.Registers.di, di);
        }


        protected override uint Pop(DataType dt)
        {
            var spReg = arch.Registers.sp;
            var ss = (ushort) ReadRegister(arch.Registers.ss);
            var sp = (ushort) ReadRegister(spReg);
            var value = ReadLeUInt16(ToLinear(ss, sp));
            WriteRegister(spReg, sp + (uint) dt.Size);
            return value;
        }

        protected override void Push(ulong value, DataType dt)
        {
            var spReg = arch.Registers.sp;
            var ss = (ushort) ReadRegister(arch.Registers.ss);
            var sp = (ushort) ReadRegister(spReg) - (uint) dt.Size;
            WriteLeUInt16(ToLinear(ss, sp), (ushort) value);
            WriteRegister(spReg, sp);
        }

        protected override void Ret()
        {
            var dst = (ushort) Pop(PrimitiveType.Word16);
            InstructionPointer = InstructionPointer.NewOffset(dst);
        }

        protected override void Retf()
        {
            var ip = (ushort) Pop(PrimitiveType.Word16);
            var cs = (ushort) Pop(PrimitiveType.Word16);
            InstructionPointer = Address.SegPtr(cs, ip);
        }

        protected override void TraceState(X86Instruction instr)
        {
            Debug.Print("{0}  ", string.Join("  ",
                new[] { "AX", "BX", "CX", "DX", "SP", "BP", "SI", "DI" }
                .Select(DumpReg)));
            Debug.Print("{0}   {1}", string.Join("  ",
                new[] { "DS", "ES", "SS", "CS", "IP" }
                .Select(DumpReg)),
                DumpFlags());
            Debug.Print("{0}", DumpInstr(instr));
            Debug.Print("-t");
            Debug.Print("");
            Debug.Print("");
        }

        private string DumpReg(string regName)
        {
            var regValue = ReadRegister(arch.GetRegister(regName.ToLower())!);
            return $"{regName}={regValue:X4}";
        }

        private string DumpFlags()
        {
            string D(uint mask, string t, string f)
            {
                return (Flags & mask) != 0 ? t : f;
            }
            return string.Join(" ",
                D(Omask, "OV", "NV"),
                D(Dmask, "DN", "UP"),
                D(Imask, "EI", "EI"),
                D(Smask, "PL", "NG"),
                D(Zmask, "ZR", "NZ"),
                D(Cmask, "--", "--"),
                D(Cmask, "--", "--"),
                D(Cmask, "CY", "NC"));
        }

        private string DumpInstr(X86Instruction instr)
        {
            return $"{instr.Address} XX {instr.ToString().ToUpper()}";
        }

        private string DumpMem(X86Instruction instr)
        {
            string sValue = "???";
            if (instr.Mnemonic == Mnemonic.lodsb || instr.Mnemonic == Mnemonic.movsb)
            {
                var ds = (uint) ReadRegister(arch.Registers.ds);
                var si = (uint) ReadRegister(arch.Registers.si);
                if (TryReadByte(ToLinear(ds, si), out var b))
                    sValue = b.ToString("X2");
                return $"DS:[{si:X4}]={sValue}";
            }
            if (instr.Mnemonic == Mnemonic.lods || instr.Mnemonic == Mnemonic.movs)
            {
                var ds = (uint) ReadRegister(arch.Registers.ds);
                var si = (uint) ReadRegister(arch.Registers.si);
                var w = ReadLeUInt16(ToLinear(ds, si));
                sValue = w.ToString("X4");
                return $"DS:[{si:X4}]={sValue}";
            }
            foreach (var op in instr.Operands)
            {
                if (op is MemoryOperand mem)
                {
                    try
                    {
                        var value = Read(mem);
                        if (mem.DataType.Size == 1)
                            sValue = ((byte) value).ToString("X2");
                        else
                            sValue = ((ushort) value).ToString("X4");
                    }
                    catch
                    {
                    }
                    return $"{mem.ToString()}={sValue}";
                }
            }
            return "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ToLinear(uint seg, uint off)
        {
            return ((ulong) seg << 4) + off;
        }
    }
}
