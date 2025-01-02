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
using Reko.Core.Diagnostics;
using Reko.Core.Loading;
using System.Collections.Generic;

namespace Reko.ImageLoaders.Elf.Relocators
{
    public class Msp430Relocator : ElfRelocator32
    {
        public Msp430Relocator(ElfLoader32 loader, SortedList<Address, ImageSymbol> imageSymbols) : base(loader, imageSymbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(RelocationContext ctx, ElfRelocation rela, ElfSymbol symbol)
        {
            var rt = (Msp430Rt) (rela.Info & 0xFF);
            ElfImageLoader.trace.Verbose("MSP430 rela: {0} {1}", rt, rela);
            return default;
        }

        public override string RelocationTypeToString(uint type)
        {
            return type.ToString();
        }
    }

    public enum Msp430Rt
    { 
        /*
R_MSP430_NONE                0
R_MSP430_ABS32               1  S + A
R_MSP430_ABS16               2  S + A
R_MSP430_ABS8                3  S + A
R_MSP430_PCR16               4  S + A - PC
R_MSP430X_PCR20_EXT_SRC      5  S + A - PC
R_MSP430X_PCR20_EXT_DST      6  S + A - PC
R_MSP430X_PCR20_EXT_ODST     7  S + A - PC
R_MSP430X_ABS20_EXT_SRC      8  S + A
R_MSP430X_ABS20_EXT_DST      9  S + A
R_MSP430X_ABS20_EXT_ODST    10  S + A
R_MSP430X_ABS20_ADR_SRC     11  S + A
R_MSP430X_ABS20_ADR_DST     12  S + A
R_MSP430X_PCR16             13  S + A - PC
R_MSP430X_PCR20_CALL        14  S + A - PC
R_MSP430X_ABS16             15  S + A
R_MSP430_ABS_HI16           16  S + A Rela only
R_MSP430_PREL31             17  S + A - PC 

* F    The relocatable field. The field is specified using the tuple [CS, O, FS], where CS is the container size, O is the starting
offset from the LSB of the container to the LSB of the field, and FS is the size of the field. All values are in bits. The notation
[x,y]+[z,w] indicates that relocation occupies discontiguous bit ranges, which should be concatenated to form the field. When
"F" is used in the addend column, it indicates that the field is already of the exact size of the address space.
* R     The arithmetic result of the relocation operation
* EV    The encoded value to be stored back into the relocation field
* SE(x) Sign-extended value of x. Sign-extension is conceptually performed to the width of the address space.
* ZE(x) Zero-extended value of x. Zero-extension is conceptually performed to the width of the address space.
* r_addend The addend must be stored in a RELA field, and may not be stored in the relocation container.
For relocation types for which overflow checking is enabled, an overflow occurs if the encoded value (including
its sign, if any) cannot be encoded into the relocatable field. That is:
• A signed relocation overflows if the encoded value falls outside the half-open interval [ -2FS-1... 2FS-1).
• An unsigned relocation overflows if the encoded value falls outside the half-open interval [ 0 … 2FS).
• A relocation whose signedness is indicated as either overflows if the encoded value falls outside the halfopen interval [ -2FS-1… 2FS).



Relocation Name Signedness Container Size (CS)
Field [O, FS] (F) Addend (A) Result (R) Overflow
Check
Encoded
Value (EV)
R_MSP430_NONE               None     32 [0,32]        None      None      No  None
R_MSP430_ABS32              Either   32 [0,32]        F         S + A     No  R
R_MSP430_ABS16              Either   16 [0,16]        SE(F)     S + A     No  R
R_MSP430_ABS8               Either    8 [0,8]         SE(F)     S + A     Yes R
R_MSP430_PCR16              Signed   16 [0,16]        SE(F)     S + A - P No  R
R_MSP430X_PCR20_EXT_SRC     Signed   48 [7,4]+[32,16] SE(F)     S + A - P Yes R
R_MSP430X_PCR20_EXT_DST     Signed   48 [0,4]+[32,16] SE(F)     S + A - P Yes R
R_MSP430X_PCR20_EXT_ODST    Signed   64 [0,4]+[48,16] SE(F)     S + A - P Yes R
R_MSP430X_ABS20_EXT_SRC     Unsigned 48 [7,4]+[32,16] ZE(F)     S + A     Yes R
R_MSP430X_ABS20_EXT_DST     Unsigned 48 [0,4]+[32,16] ZE(F)     S + A     Yes R
R_MSP430X_ABS20_EXT_ODST    Unsigned 64 [0,4]+[48,16] ZE(F)     S + A     Yes R
R_MSP430X_ABS20_ADR_SRC     Unsigned 32 [8,4]+[16,16] ZE(F)     S + A     Yes R
R_MSP430X_ABS20_ADR_DST     Unsigned 32 [0,4]+[16,16] ZE(F)     S + A     Yes R
R_MSP430X_PCR16             Signed   16 [0,16]        SE(F)     S + A - P Yes R
R_MSP430X_PCR20_CALL        Signed   32 [0,4]+[16,16] SE(F)     S + A - P Yes R
R_MSP430X_ABS16             Unsigned 16 [0,16]        SE(F)     S + A     Yes R
R_MSP430_ABS_HI16           None     16 [0,16]        r_addend  S + A     No  R >> 16
R_MSP430_PREL31             Signed   32 [0,31]        SE(F)     S + A - P No  R >> 1
         */
    }
}
