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
using Reko.Core.Machine;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        public partial class InstructionSet
        {
            /// <summary>
            /// Overrides entries in the 0F-prefixed decoder table with NEC V20/V30 
            /// specific instructions.  These opcodes conflict with later Intel SSE 
            /// encodings which are not present on the V20/V30.
            /// </summary>
            /// <remarks>
            /// Bit-manipulation group (0F 10..1F):
            ///   0F 10 /r  TEST1 r/m8,  CL
            ///   0F 11 /r  TEST1 r/m16, CL
            ///   0F 12 /r  CLR1  r/m8,  CL
            ///   0F 13 /r  CLR1  r/m16, CL
            ///   0F 14 /r  SET1  r/m8,  CL
            ///   0F 15 /r  SET1  r/m16, CL
            ///   0F 16 /r  NOT1  r/m8,  CL
            ///   0F 17 /r  NOT1  r/m16, CL
            ///   0F 18 /r ib  TEST1 r/m8,  imm8
            ///   0F 19 /r ib  TEST1 r/m16, imm8
            ///   0F 1A /r ib  CLR1  r/m8,  imm8
            ///   0F 1B /r ib  CLR1  r/m16, imm8
            ///   0F 1C /r ib  SET1  r/m8,  imm8
            ///   0F 1D /r ib  SET1  r/m16, imm8
            ///   0F 1E /r ib  NOT1  r/m8,  imm8
            ///   0F 1F /r ib  NOT1  r/m16, imm8
            ///
            /// Nybble-string group (0F 20..2F):
            ///   0F 20        ADD4S  (DS:SI += ES:DI, BCD nybble strings, length in CX)
            ///   0F 22        SUB4S  (DS:SI -= ES:DI, BCD nybble strings, length in CX)
            ///   0F 26        CMP4S  (compare BCD nybble strings)
            ///   0F 28 /r     ROL4   r/m8  (rotate AL:r/m8 nibble pair left)
            ///   0F 2A /r     ROL4   r/m16 (same, word variant -- not in all V20 docs)
            ///   0F 2C /r     ROR4   r/m8  (rotate AL:r/m8 nibble pair right)
            ///   0F 2E /r     ROR4   r/m16
            ///
            /// Emulator break (0F FF):
            ///   0F FF ib     BRKEM  imm8  (break to emulator)
            /// </remarks>
            private void CreateV20TwobyteDecoders(Decoder[] d)
            {
                // 0F 10..17: bit ops with CL as bit-index
                d[0x10] = Instr(Mnemonic.test1, Eb, c);
                d[0x11] = Instr(Mnemonic.test1, Ew, c);
                d[0x12] = Instr(Mnemonic.clr1,  Eb, c);
                d[0x13] = Instr(Mnemonic.clr1,  Ew, c);
                d[0x14] = Instr(Mnemonic.set1,  Eb, c);
                d[0x15] = Instr(Mnemonic.set1,  Ew, c);
                d[0x16] = Instr(Mnemonic.not1,  Eb, c);
                d[0x17] = Instr(Mnemonic.not1,  Ew, c);

                // 0F 18..1F: bit ops with immediate bit-index
                d[0x18] = Instr(Mnemonic.test1, Eb, Ib);
                d[0x19] = Instr(Mnemonic.test1, Ew, Ib);
                d[0x1A] = Instr(Mnemonic.clr1,  Eb, Ib);
                d[0x1B] = Instr(Mnemonic.clr1,  Ew, Ib);
                d[0x1C] = Instr(Mnemonic.set1,  Eb, Ib);
                d[0x1D] = Instr(Mnemonic.set1,  Ew, Ib);
                d[0x1E] = Instr(Mnemonic.not1,  Eb, Ib);
                d[0x1F] = Instr(Mnemonic.not1,  Ew, Ib);

                // 0F 20, 22, 26: nybble BCD string operations (no explicit operands)
                d[0x20] = Instr(Mnemonic.add4s, InstrClass.Linear);
                d[0x22] = Instr(Mnemonic.sub4s, InstrClass.Linear);
                d[0x26] = Instr(Mnemonic.cmp4s, InstrClass.Linear);

                // 0F 28, 2C: nybble rotate (byte operand)
                d[0x28] = Instr(Mnemonic.rol4, Eb);
                d[0x2C] = Instr(Mnemonic.ror4, Eb);

                // 0F FF ib: break to emulator
                d[0xFF] = Instr(Mnemonic.brkem, InstrClass.Transfer | InstrClass.Call, Ib);
            }
        }
    }
}
