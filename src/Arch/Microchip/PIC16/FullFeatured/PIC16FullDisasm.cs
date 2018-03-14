#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// A full-featured family PIC16 disassembler.
    /// </summary>
    public class PIC16FullDisasm : PIC16BasicDisasm
    {
        /// <summary>
        /// Instantiates a new Enhanced PIC16 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory segment reader.</param>
        public PIC16FullDisasm(PIC16Architecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public override InstructionSetID InstructionSetID => InstructionSetID.PIC16_FULLFEATURED;

        /// <summary>
        /// Disassembles a single Full-featured PIC16 instruction.
        /// </summary>
        /// <returns>
        /// A <see cref="PIC16Instruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        protected override PIC16Instruction DecodePIC16Instruction(ushort uInstr, PICProgAddress addr)
        {
            try
            {
                instrCur = opcodesTable[uInstr.Extract(12, 2)].Decode(uInstr, addr);
                if (instrCur is null)
                    instrCur = base.DecodePIC16Instruction(uInstr, addr);
            }
            catch (AddressCorrelatedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            if (instrCur is null)
            {
                instrCur = new PIC16Instruction(Opcode.invalid);
            }
            instrCur.Address = addrCur;
            instrCur.Length = 2;
            return instrCur;
        }


        private static Decoder[] opcodesTable = new Decoder[4]
        {
            new SubDecoder(10, 2, new Decoder[4] {                  // 00 ??.. .... ....
                new SubDecoder(8, 2, new Decoder[4] {               // 00 00?? .... ....
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0000 ?... ....
                        new SubDecoder(4, 3, new Decoder[8]{        // 00 0000 0??? ....
                            new B00_0000_0000_OpRec(),              // 00 0000 0000 ....
                            new SubDecoder(3, 1, new Decoder[2]{    // 00 0000 0001 ?...
                                new MoviIncDecOpRec(Opcode.MOVIW),  // 00 0000 0001 0...
                                new MoviIncDecOpRec(Opcode.MOVWI)   // 00 0000 0001 1...
                            }),
                            new InvalidOpRec(),                     // 00 0000 0010 ....
                            new InvalidOpRec(),                     // 00 0000 0011 ....
                            new InvalidOpRec(),                     // 00 0000 0100 ....
                            new InvalidOpRec(),                     // 00 0000 0101 ....
                            new B00_0000_0110_OpRec(),              // 00 0000 0110 ....
                            new InvalidOpRec()                      // 00 0000 0111 ....
                        }),
                        new MemoryByteOpRec(Opcode.MOVWF)           // 00 0000 1... ....
                    }),
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0001 ?... ....
                        new B00_0001_0_OpRec(),                     // 00 0001 0... ....
                        new MemoryByteWDestOpRec(Opcode.CLRF)       // 00 0001 1... ....
                    }),
                    new MemoryByteWDestOpRec(Opcode.SUBWF),         // 00 0010 .... ....
                    new MemoryByteWDestOpRec(Opcode.DECF)           // 00 0011 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 01?? .... ....
                    new MemoryByteWDestOpRec(Opcode.IORWF),         // 00 0100 .... ....
                    new MemoryByteWDestOpRec(Opcode.ANDWF),         // 00 0101 .... ....
                    new MemoryByteWDestOpRec(Opcode.XORWF),         // 00 0110 .... ....
                    new MemoryByteWDestOpRec(Opcode.ADDWF)          // 00 0111 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 10?? .... ....
                    new MemoryByteWDestOpRec(Opcode.MOVF),          // 00 1000 .... ....
                    new MemoryByteWDestOpRec(Opcode.COMF),          // 00 1001 .... ....
                    new MemoryByteWDestOpRec(Opcode.INCF),          // 00 1010 .... ....
                    new MemoryByteWDestOpRec(Opcode.DECFSZ)         // 00 1011 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 11?? .... ....
                    new MemoryByteWDestOpRec(Opcode.RRF),           // 00 1100 .... ....
                    new MemoryByteWDestOpRec(Opcode.RLF),           // 00 1101 .... ....
                    new MemoryByteWDestOpRec(Opcode.SWAPF),         // 00 1110 .... ....
                    new MemoryByteWDestOpRec(Opcode.INCFSZ)         // 00 1101 .... ....
                })
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 01 ??.. .... ....
                new MemoryBitOpRec(Opcode.BCF),                     // 01 00.. .... ....
                new MemoryBitOpRec(Opcode.BSF),                     // 01 01.. .... ....
                new MemoryBitOpRec(Opcode.BTFSC),                   // 01 10.. .... ....
                new MemoryBitOpRec(Opcode.BTFSS)                    // 01 11.. .... ....
            }),
            new SubDecoder(11, 1, new Decoder[2] {                  // 10 ?... .... ....
                new TargetAbs11OpRec(Opcode.GOTO),                  // 10 0... .... ....
                new TargetAbs11OpRec(Opcode.CALL)                   // 10 1... .... ....
            }),
            new SubDecoder(8, 4, new Decoder[16]{                   // 11 ???? .... ....
                new Immed8OpRec(Opcode.MOVLW),                      // 11 0000 .... ....
                new SubDecoder(7, 1, new Decoder[2]{                // 11 0001 ?... ....
                    new FSRArithOpRec(Opcode.ADDFSR),               // 11 0001 0... ....
                    new Immed7OpRec(Opcode.MOVLP)                   // 11 0001 1... ....
                }),
                new TargetRel9OpRec(Opcode.BRA),                    // 11 0010 .... ....
                new TargetRel9OpRec(Opcode.BRA),                    // 11 0011 .... ....
                new Immed8OpRec(Opcode.RETLW),                      // 11 0100 .... ....
                new MemoryByteWDestOpRec(Opcode.LSLF),              // 11 0101 .... ....
                new MemoryByteWDestOpRec(Opcode.LSRF),              // 11 0110 .... ....
                new MemoryByteWDestOpRec(Opcode.ASRF),              // 11 0111 .... ....
                new Immed8OpRec(Opcode.IORLW),                      // 11 1000 .... ....
                new Immed8OpRec(Opcode.ANDLW),                      // 11 1001 .... ....
                new Immed8OpRec(Opcode.XORLW),                      // 11 1010 .... ....
                new MemoryByteWDestOpRec(Opcode.SUBWFB),            // 11 1011 .... ....
                new Immed8OpRec(Opcode.SUBLW),                      // 11 1100 .... ....
                new MemoryByteWDestOpRec(Opcode.ADDWFC),            // 11 1101 .... ....
                new Immed8OpRec(Opcode.ADDLW),                      // 11 1110 .... ....
                new SubDecoder(7, 1, new Decoder[2]{                // 11 1111 ?... ....
                    new FSRIndexedOpRec(Opcode.MOVIW),              // 11 1111 0... ....
                    new FSRIndexedOpRec(Opcode.MOVWI),              // 11 1111 1... ....
                })
            })
        };

        /// <summary>
        /// A mix of instructions. Avoid huge decoder tables.
        /// </summary>
        private class B00_0000_0000_OpRec : Decoder
        {
            public B00_0000_0000_OpRec()
            {
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                switch (uInstr)
                {
                    case 0b00_0000_0000_0000:
                        return new PIC16Instruction(Opcode.NOP);

                    case 0b00_0000_0000_0001:
                        return new PIC16Instruction(Opcode.RESET);

                    case 0b00_0000_0000_1000:
                        return new PIC16Instruction(Opcode.RETURN);

                    case 0b00_0000_0000_1001:
                        return new PIC16Instruction(Opcode.RETFIE);

                    case 0b00_0000_0000_1010:
                        return new PIC16Instruction(Opcode.CALLW);

                    case 0b00_0000_0000_1011:
                        return new PIC16Instruction(Opcode.BRW);

                    default:
                        return new PIC16Instruction(Opcode.invalid);
                }
            }
        }

        /// <summary>
        /// An other mix of instructions. Avoid huge decoder tables.
        /// </summary>
        private class B00_0000_0110_OpRec : Decoder
        {
            public B00_0000_0110_OpRec()
            {
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                switch (uInstr)
                {
                    case 0b00_0000_0110_0011:
                        return new PIC16Instruction(Opcode.SLEEP);

                    case 0b00_0000_0110_0100:
                        return new PIC16Instruction(Opcode.CLRWDT);

                    default:
                        return new PIC16Instruction(Opcode.invalid);
                }
            }
        }

        /// <summary>
        /// An other mix of instructions. Avoid huge decoder tables.
        /// </summary>
        private class B00_0001_0_OpRec : Decoder
        {
            public B00_0001_0_OpRec()
            {
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                if ((uInstr & 0b11_1111_1111_1100) == 0b00_0001_0000_0000)
                    return new PIC16Instruction(Opcode.CLRW);
                if ((uInstr & 0b11_1111_1100_0000) == 0b00_0001_0100_0000)
                    return new PIC16Instruction(Opcode.MOVLB);
                return new PIC16Instruction(Opcode.invalid);
            }
        }

    }

}
