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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.Arch.X86
{
    public class X86RealModeEmulator : X86Emulator
    {
        public X86RealModeEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator) 
            : base(arch, segmentMap, envEmulator, X86.Registers.ip)
        {
        }

        private static ulong ToLinear(uint seg, uint off)
        {
            return (((ulong) seg) << 4) + off;
        }

        protected override ulong GetEffectiveAddress(MemoryOperand m)
        {
            var segReg = m.SegOverride;
            if (m.SegOverride == RegisterStorage.None)
                segReg = m.DefaultSegment;

            var off = GetEffectiveOffset(m);
            var seg = ReadRegister(segReg);
            return ToLinear(seg, off);
        }

        protected override void Lods(PrimitiveType dt)
        {
            var ds = ReadRegister(X86.Registers.ds);
            var si = ReadRegister(X86.Registers.si);
            var value = ReadMemory(ToLinear(ds, si), dt);
            var mask = masks[dt.Size];
            var a = ReadRegister(X86.Registers.eax);
            var aNew = (a & ~mask.value) | value;
            WriteRegister(X86.Registers.eax, aNew);
            var delta = (uint) dt.Size * (((Flags & Dmask) != 0) ? 0xFFFFu : 0x0001u);
            si += delta;
            WriteRegister(X86.Registers.si, si);
        }

        protected override void Movs(PrimitiveType dt)
        {
            var ds = ReadRegister(X86.Registers.ds);
            var es = ReadRegister(X86.Registers.es);
            var si = ReadRegister(X86.Registers.si);
            var di = ReadRegister(X86.Registers.di);
            var value = ReadMemory(ToLinear(ds, si), dt);
            WriteMemory(value, ToLinear(es, di), dt);
            var delta = (uint)dt.Size * (((Flags & Dmask) != 0) ? 0xFFFFu : 0x0001u);
            si += delta;
            di += delta;
            WriteRegister(X86.Registers.si, si);
            WriteRegister(X86.Registers.di, di);
        }

        public override void Push(ulong value)
        {
            var ss = (ushort) Registers[X86.Registers.ss.Number];
            var sp = (ushort) Registers[X86.Registers.esp.Number] - 2u;
            WriteLeUInt16(((ulong)ss << 4) + sp, (ushort) value);
            WriteRegister(X86.Registers.sp, sp);
        }
    }
}
