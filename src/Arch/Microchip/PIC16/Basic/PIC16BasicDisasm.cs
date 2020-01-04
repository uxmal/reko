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
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// A Basic-Range PIC16 disassembler.
    /// </summary>
    public class PIC16BasicDisasm : PIC16DisassemblerBase
    {
        /// <summary>
        /// Instantiates a new Basic-Range PIC16 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory segment reader.</param>
        protected PIC16BasicDisasm(PICArchitecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        public static PICDisassemblerBase Create(PICArchitecture arch, EndianImageReader rdr)
        {
            return new PIC16BasicDisasm(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                rdr ?? throw new ArgumentNullException(nameof(rdr))
                );
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public override InstructionSetID InstructionSetID => InstructionSetID.PIC16;


        /// <summary>
        /// Disassembles a single Basic PIC16 instruction.
        /// First try for any instruction-set specific decoding. If fail, fall to common decoder.
        /// </summary>
        /// <returns>
        /// A <see cref="PICInstruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            var offset = rdr.Offset;
            try
            {
                var bits = uInstr.Extract(12, 2);
                instrCur = opcodesTable[bits].Decode(uInstr, this);
                if (instrCur is null)
                    return base.DecodePICInstruction(uInstr, addr); // Fall to common PIC16 instruction decoder
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

        private static Decoder[] opcodesTable = new Decoder[4]
        {
            new SubDecoder(10, 2, new Decoder[4] {                  // 00 ??.. .... ....
                new SubDecoder(8, 2, new Decoder[4] {               // 00 00?? .... ....
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0000 ?... ....
                        new B00_0000_0_Decoder(),                     // 00 0000 0... ....
                        new UseBaseDecode()                         // 00 0000 1... ....
                    }),
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0001 ?... ....
                        new NoOperandDecoder(Mnemonic.CLRW),            // 00 0001 0... ....
                        new UseBaseDecode()                         // 00 0001 1... ....
                    }),
                    new UseBaseDecode(),                            // 00 0010 .... ....
                    new UseBaseDecode()                             // 00 0011 .... ....
                }),
                new UseBaseDecode(),                                // 00 01.. .... ....
                new UseBaseDecode(),                                // 00 10.. .... ....
                new UseBaseDecode()                                 // 00 11.. .... ....
            }),
            new UseBaseDecode(),                                    // 01 .... .... ....
            new UseBaseDecode(),                                    // 10 .... .... ....
            new SubDecoder(10, 2, new Decoder[4]{                   // 11 ??.. .... ....
                new Immed8Decoder(Mnemonic.MOVLW),                      // 11 00.. .... ....
                new Immed8Decoder(Mnemonic.RETLW),                      // 11 01.. .... ....
                new SubDecoder(8, 2, new Decoder[4]{                // 11 10?? .... ....
                    new Immed8Decoder(Mnemonic.IORLW),                  // 11 1000 .... ....
                    new Immed8Decoder(Mnemonic.ANDLW),                  // 11 1001 .... ....
                    new Immed8Decoder(Mnemonic.XORLW),                  // 11 1010 .... ....
                    new Immed8Decoder(Mnemonic.XORLW)                   // 11 1011 .... ....
                }),
                new SubDecoder(9, 1, new Decoder[2]{                // 11 11?. .... ....
                    new Immed8Decoder(Mnemonic.SUBLW),                  // 11 110. .... ....
                    new Immed8Decoder(Mnemonic.ADDLW)                   // 11 111. .... ....
                })
            })
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
        /// A mix of instructions. Avoid huge decoder tables.
        /// </summary>
        private class B00_0000_0_Decoder : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                switch (uInstr)
                {
                    case 0b00_0000_0000_1000:
                        return new PICInstructionNoOpnd(Mnemonic.RETURN);

                    case 0b00_0000_0000_1001:
                        return new PICInstructionNoOpnd(Mnemonic.RETFIE);

                    case 0b00_0000_0000_0000:
                    case 0b00_0000_0010_0000:
                    case 0b00_0000_0100_0000:
                    case 0b00_0000_0110_0000:
                        return new PICInstructionNoOpnd(Mnemonic.NOP);

                    case 0b00_0000_0110_0011:
                        return new PICInstructionNoOpnd(Mnemonic.SLEEP);

                    case 0b00_0000_0110_0100:
                        return new PICInstructionNoOpnd(Mnemonic.CLRWDT);

                    default:
                        return new PICInstructionNoOpnd(Mnemonic.invalid);
                }
            }
        }

    }

}
