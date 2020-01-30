#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// A Legacy PIC18 disassembler.
    /// </summary>
    public class PIC18LegacyDisasm : PIC18DisassemblerBase
    {
        /// <summary>
        /// Instantiates a new Legacy PIC18 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory segment reader.</param>
        protected PIC18LegacyDisasm(PICArchitecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        public static PICDisassemblerBase Create(PICArchitecture arch, EndianImageReader rdr)
        {
            return new PIC18LegacyDisasm(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                rdr ?? throw new ArgumentNullException(nameof(rdr))
                );
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public override InstructionSetID InstructionSetID => InstructionSetID.PIC18;

        /// <summary>
        /// Gets the decoder table corresponding to this PIC family.
        /// </summary>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            var offset = rdr.Offset;
            try
            {
                var bits = uInstr.Extract(12, 4);
                instrCur = legacyOpcodesTable[bits].Decode(uInstr, this);
                if (instrCur is null)
                    return base.DecodePICInstruction(uInstr, addr); // Fall to common PIC18 instruction decoder
                if (!instrCur.IsValid)
                {
                    rdr.Offset = offset;
                }
                instrCur.Address = addrCur;
                instrCur.Length = (int)(rdr.Address - addrCur);
            }
            catch (AddressCorrelatedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }
            return instrCur;
        }

        /// <summary>
        /// The PIC18 Legacy opcodes decoder table.
        /// </summary>
        private static readonly Decoder[] legacyOpcodesTable = new Decoder[16]
        {
            new SubDecoder(8, 4, new Decoder[16] {                  // 0000 ???? .... ....
                new SubDecoder(4, 4, new Decoder[16] {              // 0000 0000 ???? ....
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0000 ??..
                        new SubDecoder(0, 2, new Decoder[4] {       // 0000 0000 0000 00??
                            new NoOperandDecoder(Mnemonic.NOP),         // 0000 0000 0000 0000
                            new InvalidDecoder(),                     // 0000 0000 0000 0001
                            new InvalidDecoder(),                     // 0000 0000 0000 0010
                            new UseBaseDecode(),                    // 0000 0000 0000 0011
                        }),
                        new UseBaseDecode(),                        // 0000 0000 0000 01..
                        new UseBaseDecode(),                        // 0000 0000 0000 10..
                        new UseBaseDecode(),                        // 0000 0000 0000 11..
                    }),
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0001 ??..
                        new UseBaseDecode(),                        // 0000 0000 0001 00..
                        new InvalidDecoder(),                         // 0000 0000 0001 01..
                        new InvalidDecoder(),                         // 0000 0000 0001 10..
                        new InvalidDecoder(),                         // 0000 0000 0001 11..
                    }),
                    new InvalidDecoder(),                             // 0000 0000 0010 ....
                    new InvalidDecoder(),                             // 0000 0000 0011 ....
                    new InvalidDecoder(),                             // 0000 0000 0100 ....
                    new InvalidDecoder(),                             // 0000 0000 0101 ....
                    new InvalidDecoder(),                             // 0000 0000 0110 ....
                    new InvalidDecoder(),                             // 0000 0000 0111 ....
                    new InvalidDecoder(),                             // 0000 0000 1000 ....
                    new InvalidDecoder(),                             // 0000 0000 1001 ....
                    new InvalidDecoder(),                             // 0000 0000 1010 ....
                    new InvalidDecoder(),                             // 0000 0000 1011 ....
                    new InvalidDecoder(),                             // 0000 0000 1100 ....
                    new InvalidDecoder(),                             // 0000 0000 1101 ....
                    new InvalidDecoder(),                             // 0000 0000 1110 ....
                    new ResetDecoder(Mnemonic.RESET),                   // 0000 0000 1111 1111 
                }),
                new MovlbImmDecoder(Mnemonic.MOVLB),                    // 0000 0001 ..kk kkkk
                new UseBaseDecode(),                                // 0000 0010 .... ....
                new UseBaseDecode(),                                // 0000 0011 .... ....
                new UseBaseDecode(),                                // 0000 0100 .... ....
                new UseBaseDecode(),                                // 0000 0101 .... ....
                new UseBaseDecode(),                                // 0000 0110 .... ....
                new UseBaseDecode(),                                // 0000 0111 .... ....
                new UseBaseDecode(),                                // 0000 1000 .... ....
                new UseBaseDecode(),                                // 0000 1001 .... ....
                new UseBaseDecode(),                                // 0000 1010 .... ....
                new UseBaseDecode(),                                // 0000 1011 .... ....
                new UseBaseDecode(),                                // 0000 1100 .... ....
                new UseBaseDecode(),                                // 0000 1101 .... ....
                new UseBaseDecode(),                                // 0000 1110 .... ....
                new UseBaseDecode(),                                // 0000 1111 .... ....
            }),
            new UseBaseDecode(),                                    // 0001 .... .... ....
            new UseBaseDecode(),                                    // 0010 .... .... ....
            new UseBaseDecode(),                                    // 0011 .... .... ....
            new UseBaseDecode(),                                    // 0100 .... .... ....
            new UseBaseDecode(),                                    // 0101 .... .... ....
            new UseBaseDecode(),                                    // 0110 .... .... ....
            new UseBaseDecode(),                                    // 0111 .... .... ....
            new UseBaseDecode(),                                    // 1000 .... .... ....
            new UseBaseDecode(),                                    // 1001 .... .... ....
            new UseBaseDecode(),                                    // 1010 .... .... ....
            new UseBaseDecode(),                                    // 1011 .... .... ....
            new UseBaseDecode(),                                    // 1100 .... .... ....
            new UseBaseDecode(),                                    // 1101 .... .... ....
            new SubDecoder(11, 1, new Decoder[2] {                  // 1110 ?... .... ....
                new UseBaseDecode(),                                // 1110 0... .... ....
                new SubDecoder(8, 3, new Decoder[8] {               // 1110 1??? .... ....
                    new InvalidDecoder(),                             // 1110 1000 .... ....
                    new InvalidDecoder(),                             // 1110 1001 .... ....
                    new InvalidDecoder(),                             // 1110 1010 .... ....
                    new InvalidDecoder(),                             // 1110 1011 .... ....
                    new UseBaseDecode(),                            // 1110 1100 .... ....
                    new UseBaseDecode(),                            // 1110 1101 .... ....
                    new LfsrDecoder(Mnemonic.LFSR),                     // 1110 1110 ffkk kkkk + 1111 0000 kkkk kkkk
                    new UseBaseDecode(),                            // 1110 1111 .... ....
                }),
            }),
            new NoOperandDecoder(Mnemonic.NOP),                         // 1111 .... .... ....
        };

        /// <summary>
        /// Return <code>null</code> to indicate further decoding is required.
        /// </summary>
        protected class UseBaseDecode : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return null;
            }
        }

        /// <summary>
        /// Instruction MOVLB with <code>'....-....-0000-kkkk'</code> or <code>'....-....-00kk-kkkk'</code> immediate value.
        /// </summary>
        private class MovlbImmDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public MovlbImmDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte bsrval = (byte)uInstr.Extract(0, 8);
                if (bsrval >= 16)
                    return new PICInstructionNoOpnd(Mnemonic.invalid);
                return new PICInstructionImmedByte(mnemonic, bsrval);
            }
        }

        /// <summary>
        /// Instruction LFSR decoder.
        /// </summary>
        private class LfsrDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public LfsrDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(4, 4);
                if (fsrnum >= 3)
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);
                if (word2 > 0xFF) // Second word must be 'xxxx-0000-kkkk-kkkk'
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var imm12 = (ushort)((uInstr.Extract(0, 4) << 8) | word2);
                return new PICInstructionLFSRLoad(mnemonic, fsrnum, imm12);

            }
        }

    }

}
