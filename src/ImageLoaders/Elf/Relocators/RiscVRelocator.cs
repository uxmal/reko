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
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class RiscVRelocator32 : ElfRelocator32
    {
        // https://github.com/riscv/riscv-elf-psabi-doc/blob/master/riscv-elf.md
        public RiscVRelocator32(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = loader.CreateAddress(ctx.P);
            var rt = (RiscVRt) (rela.Info & 0xFF);
            switch (rt)
            {
            case RiscVRt.R_RISCV_COPY:
                return (addr, null);
            case RiscVRt.R_RISCV_RELATIVE: // B + A
                ctx.WriteUInt32(addr, ctx.B + ctx.A);
                return (addr, null);
            case RiscVRt.R_RISCV_64: // S + A
                ctx.WriteUInt32(addr, ctx.S + ctx.A);
                return (addr, null);
            case RiscVRt.R_RISCV_JUMP_SLOT: // S
                if (ctx.S == 0)
                {
                    if (!ctx.TryReadUInt32(addr, out var gotEntry))
                        return default;
                    var newSym = CreatePltStubSymbolFromRelocation(symbol, gotEntry, 0x0);
                    return (addr, newSym);
                }
                return (addr, null);
            default:
                Debug.Print("ELF RiscV: unhandled 32-bit relocation {0}: {1}", rt, rela);
                return default;
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((RiscVRt) type).ToString();
        }
    }

    public class RiscVRelocator64 : ElfRelocator64
    {
        public RiscVRelocator64(ElfLoader64 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = ctx.CreateAddress(ctx.P);
            var rt = (RiscVRt)(rela.Info & 0xFF);

            switch (rt)
            {
            case RiscVRt.R_RISCV_COPY:
                return (addr, null);
            case RiscVRt.R_RISCV_RELATIVE: // B + A
                ctx.WriteUInt64(addr, ctx.B + ctx.A);
                return (addr, null);
            case RiscVRt.R_RISCV_64: // S + A
                ctx.WriteUInt64(addr, ctx.S + ctx.A);
                return (addr, null);
            case RiscVRt.R_RISCV_JUMP_SLOT: // S
                if (ctx.S == 0)
                {
                    if (!ctx.TryReadUInt64(addr, out ulong gotEntry))
                        return default;
                    var newSym = CreatePltStubSymbolFromRelocation(symbol, gotEntry, 0x0);
                    return (addr, newSym);
                }
                return (addr, null);
            default:
                ctx.Warn(addr, $"ELF RiscV: unhandled 64-bit relocation {rt}: {rela}.");
                return (addr, null);
            }
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((RiscVRt)type).ToString();
        }
    }

    public enum RiscVRt
    {
        R_RISCV_NONE = 0,
        R_RISCV_32 = 1,
        R_RISCV_64 = 2,
        R_RISCV_RELATIVE = 3,
        R_RISCV_COPY = 4,
        R_RISCV_JUMP_SLOT = 5,
        R_RISCV_TLS_DTPMOD32 = 6,
        R_RISCV_TLS_DTPMOD64 = 7,
        R_RISCV_TLS_DTPREL32 = 8,
        R_RISCV_TLS_DTPREL64 = 9,
        R_RISCV_TLS_TPREL32 = 1,
        R_RISCV_TLS_TPREL64 = 1,
    }

    [Flags]
    public enum RiscVFlags
    {
        EF_RISCV_RVC = 0x1,
        EF_RISCV_FLOAT_ABI_MASK = 0x6,
        EF_RISCV_FLOAT_ABI_SOFT = 0x6,
        EF_RISCV_FLOAT_ABI_SINGLE = 0x2,
        EF_RISCV_FLOAT_ABI_DOUBLE = 0x4,
        EF_RISCV_FLOAT_ABI_QUAD = 0x6,

    }

    public static class RiscVElf
    {
        public static void SetOptions(RiscVFlags riscVFlags, Dictionary<string, object> options)
        {
            options["Compact"] = (riscVFlags & RiscVFlags.EF_RISCV_RVC) != 0;
            options["FloatAbi"] = (riscVFlags & RiscVFlags.EF_RISCV_FLOAT_ABI_MASK) switch
            {
                RiscVFlags.EF_RISCV_FLOAT_ABI_QUAD => 128,
                RiscVFlags.EF_RISCV_FLOAT_ABI_DOUBLE => 64,
                RiscVFlags.EF_RISCV_FLOAT_ABI_SINGLE => 32,
                _ => 0    // soft floats only.
            };
        }
    }
}
