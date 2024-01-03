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
    public class Ia64Relocator : ElfRelocator64
    {
        public Ia64Relocator(ElfLoader64 elfLoader, SortedList<Address, ImageSymbol> symbols)
            : base(elfLoader, symbols)
        {
        }

        public override (Address?, ElfSymbol?) RelocateEntry(Program program, ElfSymbol symbol, ElfSection? referringSection, ElfRelocation rela)
        {
            //$TODO: relocate!
            return (null, null);
        }

        public override string RelocationTypeToString(uint type)
        {
            return ((Ia64Rt) type).ToString();
        }

        public enum Ia64Rt
        {
            R_IA_64_NONE = 0x00, /* None                 None            */

            /* unused:  0x01 .. 0x20                                            */

            R_IA_64_IMM14 = 0x21, /* instr: immediate14   S+A             */
            R_IA_64_IMM22 = 0x22, /* instr: immediate22   S+A             */
            R_IA_64_IMM64 = 0x23, /* instr: immediate64   S+A             */
            R_IA_64_DIR32MSB = 0x24, /* word32 MSB           S+A             */
            R_IA_64_DIR32LSB = 0x25, /* word32 LSB           S+A             */
            R_IA_64_DIR64MSB = 0x26, /* word64 MSB           S+A             */
            R_IA_64_DIR64LSB = 0x27, /* word64 LSB           S+A             */

            /* unused:  0x28 .. 0x29                                            */

            R_IA_64_GPREL22 = 0x2a, /* instr: immediate22   @gprel(S+A)     */
            R_IA_64_GPREL64I = 0x2b, /* instr: immediate64   @gprel(S+A)     */

            /* unused:  0x2c .. 0x2d                                            */

            R_IA_64_GPREL64MSB = 0x2e, /* word64 MSB           @gprel(S+A)     */
            R_IA_64_GPREL64LSB = 0x2f, /* word64 LSB           @gprel(S+A)     */

            /* unused:  0x30 .. 0x31                                            */

            R_IA_64_LTOFF22 = 0x32, /* instr: immediate22   @ltoff(S+A)     */
            R_IA_64_LTOFF64I = 0x33, /* instr: immediate64   @ltoff(S+A)     */

            /* unused:  0x34 .. 0x39                                            */

            R_IA_64_PLTOFF22 = 0x3a, /* instr: immediate22   @pltoff(S+A)    */
            R_IA_64_PLTOFF64I = 0x3b, /* instr: immediate64   @pltoff(S+A)    */

            /* unused:  0x3c .. 0x3d                                            */

            R_IA_64_PLTOFF64MSB = 0x3e, /* word64 MSB           @pltoff(S+A)    */
            R_IA_64_PLTOFF64LSB = 0x3f, /* wordL4 MSB           @pltoff(S+A)    */

            /* unused:  0x40 .. 0x42                                            */

            R_IA_64_FPTR64I = 0x43, /* instr: immediate64   @fptr(S+A)      */
            R_IA_64_FPTR32MSB = 0x44, /* word32 MSB           @fptr(S+A)      */
            R_IA_64_FPTR32LSB = 0x45, /* word32 LSB           @fptr(S+A)      */
            R_IA_64_FPTR64MSB = 0x46, /* word64 MSB           @fptr(S+A)      */
            R_IA_64_FPTR64LSB = 0x47, /* word64 LSB           @fptr(S+A)      */

            /* unused:  0x48 .. 0x48                                            */

            R_IA_64_PCREL21B = 0x49, /* instr: imm21 (form1) S+A-P           */
            R_IA_64_PCREL21M = 0x4a, /* instr: imm21 (form2) S+A-P           */
            R_IA_64_PCREL21F = 0x4b, /* instr: imm21 (form3) S+A-P           */
            R_IA_64_PCREL32MSB = 0x4c, /* word32 MSB           S+A-P           */
            R_IA_64_PCREL32LSB = 0x4d, /* word32 LSB           S+A-P           */
            R_IA_64_PCREL64MSB = 0x4e, /* word64 MSB           S+A-P           */
            R_IA_64_PCREL64LSB = 0x4f, /* word64 LSB           S+A-P           */

            /* unused:  0x50 .. 0x51                                            */

            R_IA_64_LTOFF_FPTR22 = 0x52,/* instr: immediate22   @ltoff(@fptr(S+A)) */
            R_IA_64_LTOFF_FPTR64I = 0x53,/* instr: immediate64   @ltoff(@fptr(S+A)) */

            /* unused:  0x54 .. 0x5b                                            */

            R_IA_64_SEGREL32MSB = 0x5c, /* word32 MSB           @segrel(S+A)    */
            R_IA_64_SEGREL32LSB = 0x5d, /* word32 LSB           @segrel(S+A)    */
            R_IA_64_SEGREL64MSB = 0x5e, /* word64 MSB           @segrel(S+A)    */
            R_IA_64_SEGREL64LSB = 0x5f, /* word64 LSB           @segrel(S+A)    */

            /* unused:  0x60 .. 0x63                                            */

            R_IA_64_SECREL32MSB = 0x64, /* word32 MSB           @secrel(S+A)    */
            R_IA_64_SECREL32LSB = 0x65, /* word32 LSB           @secrel(S+A)    */
            R_IA_64_SECREL64MSB = 0x66, /* word64 MSB           @secrel(S+A)    */
            R_IA_64_SECREL64LSB = 0x67, /* word64 LSB           @secrel(S+A)    */

            /* unused:  0x68 .. 0x6b                                            */

            R_IA_64_REL32MSB = 0x6c, /* word32 MSB           BD+C            */
            R_IA_64_REL32LSB = 0x6d, /* word32 LSB           BD+C            */
            R_IA_64_REL64MSB = 0x6e, /* word64 MSB           BD+C            */
            R_IA_64_REL64LSB = 0x6f, /* word64 LSB           BD+C            */
            R_IA_64_LTV32MSB = 0x70, /* word32 MSB           S+A [note 2]    */
            R_IA_64_LTV32LSB = 0x71, /* word32 LSB           S+A [note 2]    */
            R_IA_64_LTV64MSB = 0x72, /* word64 MSB           S+A [note 2]    */
            R_IA_64_LTV64LSB = 0x73, /* word64 LSB           S+A [note 2]    */

            /* unused:  0x74 .. 0x7f                                            */

            R_IA_64_IPLTMSB = 0x80, /* func desc MSB        [note 3]        */
            R_IA_64_IPLTLSB = 0x81, /* func desc LSB        [note 3]        */

            /* unused:  0x82 .. 0xff                                            */

            R_IA_64_END_ = 0x82  /* R_IA_64_END_ is not a relocation type.
                                  * It marks the end of the list of types.
                                  */
        }
    }
}