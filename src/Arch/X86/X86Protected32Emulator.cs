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
using Reko.Core.Types;
using System;

namespace Reko.Arch.X86
{
    public class X86Protected32Emulator : X86Emulator
    {
        public X86Protected32Emulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator) 
            : base(arch, segmentMap, envEmulator, X86.Registers.eip, X86.Registers.ecx)
        {
        }

        protected override void Call(MachineOperand op)
        {
            Push((uint) InstructionPointer.ToLinear() + (uint) dasm.Current.Length, PrimitiveType.Word32);   // Push return value on stack

            var dest = XferTarget(op);
            if (envEmulator.InterceptCall(this, (uint) dest.ToLinear()))
                return;
            InstructionPointer = dest;
        }

        //$TODO: fs:[...] and gs:[...]
        protected override ulong GetEffectiveAddress(MemoryOperand m)
        {
            return GetEffectiveOffset(m);
        }

        protected override void Lods(PrimitiveType dt)
        {
            throw new NotImplementedException();
        }

        protected override void Movs(PrimitiveType dt)
        {
            throw new NotImplementedException();
        }

        protected override void Stos(PrimitiveType dt)
        {
            throw new NotImplementedException();
        }


        protected override uint Pop(PrimitiveType dt)
        {
            var esp = ReadRegister(X86.Registers.esp);
            var word = ReadLeUInt32(esp);
            WriteRegister(X86.Registers.esp, esp + 4);
            return word;
        }

        protected override void Push(ulong word, PrimitiveType dt)
        {
            var esp = (uint) Registers[X86.Registers.esp.Number] - 4;
            WriteLeUInt32(esp, (uint) word);
            WriteRegister(X86.Registers.esp, (uint) esp);
        }

        protected override void Ret()
        {
            var dst = Pop(PrimitiveType.Word32);
            InstructionPointer = Address.Ptr32(dst);
        }

        protected override void Retf()
        {
            // RETF on x86 is rare. Implement when needed.
            throw new NotImplementedException();
        }
    }
}
