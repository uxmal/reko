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

using Reko.Core;
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class Sparc64Relocator : ElfRelocator64
    {
        private Dictionary<Address, ImportReference> importReferences;

        public Sparc64Relocator(ElfLoader64 elfLoader, SortedList<Address, ImageSymbol> symbols) : 
            base(elfLoader, symbols)
        {
            importReferences = null!;
        }

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            this.importReferences = program.ImportReferences;
            base.Relocate(program, addrBase, pltEntries);
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = ctx.CreateAddress(ctx.P);
            var rt = (SparcRt) (rela.Info & 0xFF);
            int A;
            int sh = 0;
            ulong P;
            ulong S = 0;
            ulong B = 0;
            uint mask = ~0u;
            switch (rt)
            {
            case SparcRt.R_SPARC_GLOB_DAT:
            case SparcRt.R_SPARC_JMP_SLOT:
                if (ctx.S == 0)
                {
                    ctx.AddImportReference(symbol, addr);
                }
                return (addr, null);
            case SparcRt.R_SPARC_NONE:
                return default;
            case SparcRt.R_SPARC_HI22:
                A = (int) ctx.A;
                sh = 10;
                P = 0;
                mask = 0x3FFFFF;
                return Relocate32(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_LM22:
                A = (int) ctx.A;
                S = ctx.S;
                sh = 10;
                P = 0;
                mask = 0x3FFFFF;
                return Relocate32(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_LO10:
                A = (int) rela.Addend!.Value;
                S = ctx.S;
                mask = 0x3FF;
                P = 0;
                return Relocate32(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_32:
                A = (int) rela.Addend!.Value;
                S = ctx.S;
                mask = 0xFFFFFFFF;
                P = 0;
                return Relocate32(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_WDISP30:
                A = (int) rela.Addend!.Value;
                P = ~ctx.P + 1;
                sh = 2;
                return Relocate32(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_RELATIVE:
                A = (int) rela.Addend!.Value;
                B = ctx.B;
                P = 0;
                return Relocate64(ctx, addr, S, A, sh, mask, P, B);
            case SparcRt.R_SPARC_COPY:
                return (addr, null);
            default:
                ctx.Warn(addr, $"SPARC ELF relocation type {rt} not implemented yet.");
                return (addr, null);
            }
        }

        private (Address?, ElfSymbol?) Relocate32(RelocationContext ctx, Address addr, ulong S, int A, int sh, uint mask, ulong P, ulong B)
        {
            if (!ctx.TryReadUInt32(addr, out uint w))
                return default;
            var wOld = w;
            w += ((uint) (B + S + (uint) A + P) >> sh) & mask;
            ctx.WriteUInt32(addr, w);
            return (addr, null);
        }

        private (Address?, ElfSymbol?) Relocate64(RelocationContext ctx, Address addr, ulong S, int A, int sh, uint mask, ulong P, ulong B)
        {
            if (!ctx.TryReadUInt64(addr, out ulong w))
                return default;
            var wOld = w;
            w += ((ulong) (B + S + (ulong) (long) A + P) >> sh) & mask;
            ctx.WriteUInt64(addr, w);
            return (addr, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return type.ToString();
        }
    }
}
