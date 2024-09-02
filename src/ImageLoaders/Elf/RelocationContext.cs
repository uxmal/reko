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
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.Elf
{
    /// <summary>
    /// Resources used when processing a particular segment containing
    /// relocations.
    /// </summary>
    public class RelocationContext
    {
        private readonly ElfLoader loader;
        private readonly Dictionary<ElfSymbol, Address> pltLocations;
        private readonly Program program;
        private readonly IWriteableMemory memory;
        private readonly bool isBigendian;
        private readonly bool isRelocatableFile;

        public RelocationContext(
            ElfLoader loader,
            Program program,
            Address addrBase,
            ElfSection? referringSection,
            Dictionary<ElfSymbol, Address> pltLocations)
        {
            this.loader = loader;
            this.ReferringSection = referringSection;
            this.pltLocations = pltLocations;
            this.program = program;
            this.memory = (IWriteableMemory)program.Memory;
            this.isBigendian = loader.IsBigendian;
            this.isRelocatableFile = loader.IsRelocatableFile;
            this.B = addrBase.ToLinear();
        }

        public bool Update(ElfRelocation reloc, ElfSymbol sym)
        {
            if (this.isRelocatableFile)
            {
                if (sym.SectionIndex >= loader.Sections.Count)
                    return false;
                this.P = reloc.Offset + (ReferringSection?.Address.ToLinear() ?? 0);
                this.S = loader.Sections[(int) sym.SectionIndex].Address.Offset + sym.Value;
                this.L = pltLocations.TryGetValue(sym, out var pltAddr)
                    ? pltAddr.Offset
                    : 0;
            }
            else
            {
                this.P = reloc.Offset;
                this.S = sym.Value;
                this.L = 0;
            }
            this.A = (ulong)(reloc.Addend ?? 0);
            return true;
        }

        public ulong A { get; private set; }
        public ulong B { get; }
        public ulong L { get; private set; }
        public ulong P { get; private set; }
        public ulong S { get; private set; }
        public ElfSection? ReferringSection { get; }
        public PrimitiveType PointerType => program.Architecture.PointerType;

        public void WriteUInt16(Address addr, ulong value)
        {
            var val = (ushort) value;
            if (isBigendian)
                memory.WriteBeUInt16(addr, val);
            else
                memory.WriteLeUInt16(addr, val);
        }

        public void WriteUInt32(Address addr, ulong value)
        {
            var val = (uint) value;
            if (isBigendian)
                memory.WriteBeUInt32(addr, val);
            else
                memory.WriteLeUInt32(addr, val);
        }

        public void WriteUInt64(Address addr, ulong value)
        {
            if (isBigendian)
                memory.WriteBeUInt64(addr, value);
            else
                memory.WriteLeUInt64(addr, value);
        }

        public bool TryReadUInt16(Address addr, out ushort value)
        {
            if (isBigendian)
                return memory.TryReadBeUInt16(addr, out value);
            else
                return memory.TryReadLeUInt16(addr, out value);
        }

        public bool TryReadUInt32(Address addr, out uint value)
        {
            if (isBigendian)
            {
                return memory.TryReadBeUInt32(addr, out value);
            }
            else
            {
                return memory.TryReadLeUInt32(addr, out value);
            }
        }

        public bool TryReadUInt64(Address addr, out ulong value)
        {
            if (isBigendian)
            {
                return memory.TryReadBeUInt64(addr, out value);
            }
            else
            {
                return memory.TryReadLeUInt64(addr, out value);
            }
        }

        public Address CreateAddress(ulong uAddr) => loader.CreateAddress(uAddr);

        public bool IsExecutableAddress(Address address) => memory.IsExecutableAddress(address);

        public void AddImportReference(ElfSymbol symbol, Address addrPfn)
        {
            Debug.Print("Import reference {0} - {1}", addrPfn, symbol.Name);
            var st = ElfLoader.GetSymbolType(symbol);
            if (st.HasValue)
            {
                program.ImportReferences[addrPfn] = new NamedImportReference(addrPfn, null, symbol.Name, st.Value);
            }
        }

        public void AddGotSymbol(ElfSymbol symbol, Address addrPfn)
        {
            var gotSymbol = loader.CreateGotSymbol(program.Architecture, addrPfn, symbol.Name);
            program.ImageSymbols[addrPfn] = gotSymbol;
        }

        public void Warn(Address addr, string message)
        {
            var listener = loader.Services.RequireService<IEventListener>();
            var loc = listener.CreateAddressNavigator(program, addr);
            listener.Warn(loc, message);
        }
    }
}
