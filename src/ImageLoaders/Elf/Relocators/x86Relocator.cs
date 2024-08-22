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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class x86Relocator : ElfRelocator32
    {
        public x86Relocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override void Relocate(Program program, Address addrBase, Dictionary<ElfSymbol, Address> pltEntries)
        {
            base.Relocate(program, addrBase, pltEntries);
            LoadImportReferencesFromRelPlt(program.ImportReferences);
        }

        private void LoadImportReferencesFromRelPlt(Dictionary<Address,ImportReference> importReferences)
        {
            var rel_plt = loader.GetSectionInfoByName(".rel.plt");
            if (rel_plt is null)
                return;
            var symtab = rel_plt.LinkedSection!;
            var relRdr = loader.CreateReader(rel_plt.FileOffset);

            uint entries = rel_plt.EntryCount();
            for (uint i = 0; i < entries; ++i)
            {
                if (!relRdr.TryReadUInt32(out uint offset))
                    return;
                if (!relRdr.TryReadUInt32(out uint info))
                    return;

                uint sym = info >> 8;
                string symStr = loader.GetSymbolName(symtab, sym);

                //$REFACTOR: use common code here. 
                var addr = Address.Ptr32(offset);
                importReferences[addr] = new NamedImportReference(
                    addr, "", symStr, SymbolType.ExternalProcedure);
            }
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var addr = Address.Ptr32((uint) ctx.P);
            var rt = (i386Rt)(rela.Info & 0xFF);
            switch (rt)
            {
            case i386Rt.R_386_NONE:
            case i386Rt.R_386_COPY:
                break;
            case i386Rt.R_386_RELATIVE: // B + A
                ctx.WriteUInt32(addr, ctx.B + ctx.A);
                break;
            case i386Rt.R_386_JMP_SLOT: // S
                if (symbol.Value == 0)
                {
                    // Broken GCC compilers generate relocations referring to symbols 
                    // whose value is 0 instead of the expected address of the PLT stub.
                    if (!ctx.TryReadUInt32(addr, out var gotEntry))
                        return default;
                    var symNew = CreatePltStubSymbolFromRelocation(symbol, gotEntry, 6);
                    return (addr, symNew);
                }
                ctx.WriteUInt32(addr, ctx.S);
                break;
            case i386Rt.R_386_32:   // S + A
                ctx.WriteUInt32(addr, ctx.S + ctx.A);
                break;
            case i386Rt.R_386_PC32: // S + A - P
                if (symbol.Value == 0)
                {
                    // This means that the symbol doesn't exist in this module, and is not accessed
                    // through the PLT, i.e. it will be statically linked, e.g. strcmp. We have the
                    // name of the symbol right here in the symbol table entry, but the only way
                    // to communicate with the loader is through the target address of the call.
                    // So we use some very improbable addresses (e.g. -1, -2, etc) and give them entries
                    // in the symbol table
                    //S = nextFakeLibAddr--; // Allocate a new fake address
                    //loader.AddSymbol(S, sym.Name);
                    //}
                }
                ctx.WriteUInt32(addr, ctx.S + ctx.A - ctx.P);
                break;
            case i386Rt.R_386_GLOB_DAT: // S
                // This relocation type is used to set a global offset table entry to the address of the
                // specified symbol. The special relocation type allows one to determine the
                // correspondence between symbols and global offset table entries.
                ctx.WriteUInt32(addr, ctx.S);
                break;
            default:
                Debug.Print($"i386 ELF relocation type {rt} not implemented yet.");
                return (null, null);
            }
            return (addr, symbol);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((i386Rt)type).ToString();
        }
    }

    public enum i386Rt
    {
        R_386_NONE,             // just ignore (common)
        R_386_32 = 1,           // S + A
        R_386_PC32 = 2,         // S + A - P
        R_386_COPY = 5,         // seems to do nothing according to ELF spec.
        R_386_GLOB_DAT = 6,
        R_386_JMP_SLOT = 7,     // S
        R_386_RELATIVE = 8
    }
}
