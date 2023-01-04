#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Services;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class Arm64Relocator : ElfRelocator64
    {
        public Arm64Relocator(ElfLoader64 loader, SortedList<Address, ImageSymbol> symbols) : base(loader, symbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol symbol, ElfSection? referringSection, ElfRelocation  rela)
        {
            var addr = Address.Ptr64(rela.Offset);
            var rt = (Aarch64Rt) (rela.Info & 0xFFFF);
            ulong A = (ulong) rela.Addend!;
            ulong S = symbol.Value;

            switch (rt)
            {
            case Aarch64Rt.R_AARCH64_RELATIVE:
                A = S = 0;
                break;
            case Aarch64Rt.R_AARCH64_JUMP_SLOT: // A + S
            case Aarch64Rt.R_AARCH64_GLOB_DAT:  // A + S
                break;
            default:
                var listener = this.loader.Services.RequireService<DecompilerEventListener>();
                var loc = listener.CreateAddressNavigator(program, addr);
                listener.Warn(loc, $"Unimplemented PowerPC64 relocation type {rt}.");
                return (addr, null);
            }
            var arch = program.Architecture;
            var relR = program.CreateImageReader(arch, addr);
            var relW = program.CreateImageWriter(arch, addr);

            var w = relR.ReadUInt64();
            w += (S + A);
            relW.WriteUInt64(w);

            return (addr, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Aarch64Rt) type).ToString();
        }
    }

    public enum Aarch64Rt
    {
        R_AARCH64_COPY = 1024, //  	See note below.
        R_AARCH64_GLOB_DAT = 1025, // S + A	See note below
        R_AARCH64_JUMP_SLOT = 1026, // S + A	See note below
        R_AARCH64_RELATIVE = 1027, // Delta(S) + A	See note below
        R_AARCH64_TLS_IMPDEF1 = 1028, //  	See note below
        R_AARCH64_TLS_IMPDEF2 = 1029, //  	See note below
                                      //R_AARCH64_TLS_DTPREL	DTPREL(S+A)	See note below
                                      //R_AARCH64_TLS_DTPMOD	LDM(S)	See note below
        R_AARCH64_TLS_TPREL = 1030, // TPREL(S+A)	 
        R_AARCH64_TLSDESC = 1031, // TLSDESC(S+A)	Identifies a TLS descriptor to be filled
        R_AARCH64_IRELATIVE = 1032, // Indirect(Delta(S) + A)	See note below.
    }
}
