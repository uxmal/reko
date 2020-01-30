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

using Reko.Libraries.Microchip;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// A Microchip PIC16 *partial* disassembler.
    /// Valid for most of program memory regions (code, eeprom, config, ...).
    /// For PIC16 instructions, only the instructions common to all PIC16 families are decoded here.
    /// For each PIC16 family (basic, enhanced, full-featured) a specific decoder must take care of
    /// decoding instructions that are specific to the said-target family and uses this inherited disassembler
    /// only for the balance of instructions.
    /// This 2-stage decoding permits to ease the maintenance, the tests and decreases the total size of the decoder tables
    /// </summary>
    public class PIC16DisassemblerBase : PICDisassemblerBase
    {
        /// <summary>
        /// Instantiates a base PIC16 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory image reader.</param>
        protected PIC16DisassemblerBase(PICArchitecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier. Must be overriden.
        /// </summary>
        public override InstructionSetID InstructionSetID => InstructionSetID.UNDEFINED;

        /// <summary>
        /// Disassembles a single PIC16 instruction.
        /// Only instructions whose binary values are common to all PIC16 families are decoded here.
        /// This method should be called by derived classes in case the binary instruction they deal
        /// with can't be decoded for a specific PIC16 instruction-set.
        /// </summary>
        /// <param name="addr">The program address of the instruction.</param>
        /// <param name="uInstr">The instruction binary 16-bit word.</param>
        /// <returns>
        /// A <see cref="PICInstruction"/> instance or null.
        /// </returns>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            try
            {
                instrCur = decoderTable[uInstr.Extract(12, 2)].Decode(uInstr, this);
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            // All legal PIC16 instructions are one word long.
            if (instrCur != null)
            {
                instrCur.Address = addrCur;
                instrCur.Length = 2;
            }
            return instrCur;
        }

        #region Instruction Decoder helpers

        private static readonly Decoder[] decoderTable = new Decoder[4]
        {
            new SubDecoder(10, 2, new Decoder[4] {                  // 00 ??.. .... ....
                new SubDecoder(8, 2, new Decoder[4] {               // 00 00?? .... ....
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0000 ?... ....
                        new WrongDecoder(),                         // 00 0000 0... ....
                        new MemoryByteDecoder(Mnemonic.MOVWF)           // 00 0000 1... ....
                    }),
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0001 ?... ....
                        new WrongDecoder(),                         // 00 0001 0... ....
                        new MemoryByteDecoder(Mnemonic.CLRF)            // 00 0001 1... ....
                    }),
                    new MemoryByteWDestDecoder(Mnemonic.SUBWF),         // 00 0010 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.DECF)           // 00 0011 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 01?? .... ....
                    new MemoryByteWDestDecoder(Mnemonic.IORWF),         // 00 0100 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.ANDWF),         // 00 0101 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.XORWF),         // 00 0110 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.ADDWF)          // 00 0111 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 10?? .... ....
                    new MemoryByteWDestDecoder(Mnemonic.MOVF),          // 00 1000 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.COMF),          // 00 1001 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.INCF),          // 00 1010 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.DECFSZ)         // 00 1011 .... ....
                }),
                new SubDecoder(8, 2, new Decoder[4] {               // 00 11?? .... ....
                    new MemoryByteWDestDecoder(Mnemonic.RRF),           // 00 1100 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.RLF),           // 00 1101 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.SWAPF),         // 00 1110 .... ....
                    new MemoryByteWDestDecoder(Mnemonic.INCFSZ)         // 00 1101 .... ....
                })
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 01 ??.. .... ....
                new MemoryBitDecoder(Mnemonic.BCF),                     // 01 00.. .... ....
                new MemoryBitDecoder(Mnemonic.BSF),                     // 01 01.. .... ....
                new MemoryBitDecoder(Mnemonic.BTFSC),                   // 01 10.. .... ....
                new MemoryBitDecoder(Mnemonic.BTFSS)                    // 01 11.. .... ....
            }),
            new SubDecoder(11, 1, new Decoder[2] {                  // 10 ?... .... ....
                new TargetAbs11Decoder(Mnemonic.CALL),                  // 10 0... .... ....
                new TargetAbs11Decoder(Mnemonic.GOTO)                   // 10 1... .... ....
            }),
            new WrongDecoder()                                      // 11 .... .... ....

        };

        /// <summary>
        /// Instruction with no operand.
        /// </summary>
        protected class NoOperandDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public NoOperandDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return new PICInstructionNoOpnd(mnemonic);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-..bb-bfff-ffff'</code>  (BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        protected class MemoryBitDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public MemoryBitDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var bitno = (byte)uInstr.Extract(7, 3);
                var off = uInstr.Extract(0, 7);
                return new PICInstructionMemFB(mnemonic, off, bitno);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-....-.fff-ffff'</code>  (MOVWF)
        /// </summary>
        protected class MemoryByteDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public MemoryByteDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var fff = uInstr.Extract(0, 7);
                return new PICInstructionMemF(mnemonic, fff);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-....-dfff-ffff'</code>  (ADDWF, LSLF, IORWF, INCF, SWAPF, ...)
        /// </summary>
        protected class MemoryByteWDestDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public MemoryByteWDestDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var fff = uInstr.Extract(0, 7);
                var dest = uInstr.Extract(7, 1);
                return new PICInstructionMemFD(mnemonic, fff, dest);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-...k-kkkk'</code> (MOVLB, ...)
        /// </summary>
        protected class Immed5Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public Immed5Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm5 = uInstr.Extract(0, 5);
                return new PICInstructionImmedByte(mnemonic, imm5);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.kkk-kkkk'</code> (MOVLP, ...)
        /// </summary>
        protected class Immed7Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public Immed7Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm7 = uInstr.Extract(0, 7);
                return new PICInstructionImmedByte(mnemonic, imm7);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-kkkk-kkkk'</code> (ADDLW, MOVLW, RETLW, PUSHL, ...)
        /// </summary>
        protected class Immed8Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public Immed8Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm8 = uInstr.Extract(0, 8);
                return new PICInstructionImmedByte(mnemonic, imm8);
            }
        }

        /// <summary>
        /// Relative branch (BRA) decoder.
        /// </summary>
        protected class TargetRel9Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public TargetRel9Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var reloff = uInstr.ExtractSignExtend(0, 9);
                return new PICInstructionProgTarget(mnemonic, reloff, dasm.addrCur);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (ADDFSR)
        /// </summary>
        protected class FSRArithDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public FSRArithDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PICInstructionFSRIArith(mnemonic, fsrnum, lit);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (MOVIW k[n], MOVWI k[n]) with -32 <= k <= 31
        /// </summary>
        protected class FSRIndexedDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public FSRIndexedDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PICInstructionWithFSR(mnemonic, fsrnum, lit, FSRIndexedMode.INDEXED);
            }
        }

        /// <summary>
        /// Instruction MOVIW/MOVWI inc/dec decoder.
        /// </summary>
        protected class MoviIncDecDecoder : Decoder
        {
            private static readonly FSRIndexedMode[] code2FSRIdx = new FSRIndexedMode[4]
                { FSRIndexedMode.PREINC, FSRIndexedMode.PREDEC, FSRIndexedMode.POSTINC, FSRIndexedMode.POSTDEC };

            private readonly Mnemonic mnemonic;

            public MoviIncDecDecoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(2, 1);
                byte modecode = (byte)uInstr.Extract(0, 2);
                return new PICInstructionWithFSR(mnemonic, fsrnum, 0, code2FSRIdx[modecode]);
            }
        }

        /// <summary>
        /// Instruction GOTO/CALL decoder.
        /// </summary>
        protected class TargetAbs11Decoder : Decoder
        {
            private readonly Mnemonic mnemonic;

            public TargetAbs11Decoder(Mnemonic mnemonic)
            {
                this.mnemonic = mnemonic;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var progAdr = uInstr.Extract(0, 11);
                return new PICInstructionProgTarget(mnemonic, progAdr);
            }
        }

        #endregion

        #region Pseudo-instructions disassemblers for PIC16

        protected override PICInstruction DecodeEEPROMInstruction()
        {
            if (!rdr.TryReadByte(out byte uEEByte))
                return null;
            var bl = new List<byte>() { uEEByte };
            for (int i = 0; i < 7; i++)
            {
                if (!lastusedregion.Contains(rdr.Address))
                    break;
                if (!rdr.TryReadByte(out uEEByte))
                    break;
                bl.Add(uEEByte);
            }
            instrCur = new PICInstructionPseudo(Mnemonic.DE, new PICOperandDEEPROM(bl.ToArray()))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected override PICInstruction DecodeDAInstruction()
        {
            if (!rdr.TryReadByte(out byte uDAByte))
                return null;
            instrCur = new PICInstructionPseudo(Mnemonic.DA, new PICOperandDASCII(uDAByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        protected override PICInstruction DecodeDBInstruction()
        {
            if (!rdr.TryReadByte(out byte uDBByte))
                return null;
            instrCur = new PICInstructionPseudo(Mnemonic.DB, new PICOperandDByte(uDBByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        protected override PICInstruction DecodeDWInstruction()
        {
            if (!rdr.TryReadUInt16(out ushort uDWWord))
                return null;
            instrCur = new PICInstructionPseudo(Mnemonic.DW, new PICOperandDWord(uDWWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        protected override PICInstruction DecodeUserIDInstruction()
        {
            if (!rdr.TryReadUInt16(out ushort uConfigWord))
                return null;
            instrCur = new PICInstructionPseudo(Mnemonic.__IDLOCS, new PICOperandIDLocs(addrCur, uConfigWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        protected override PICInstruction DecodeConfigInstruction()
        {
            if (!rdr.TryReadUInt16(out ushort uConfigWord))
                return null;
            var cfgAddr = PICProgAddress.Ptr((uint)(addrCur.ToLinear() >> 1));
            instrCur = new PICInstructionPseudo(Mnemonic.__CONFIG, new PICOperandConfigBits(arch, cfgAddr, uConfigWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        protected override PICInstruction DecodeDCIInstruction()
            => throw new NotImplementedException("Disassembly of DCI memory region is not yet implemented.");

        protected override PICInstruction DecodeDIAInstruction()
            => throw new NotImplementedException("Disassembly of DIA memory region is not yet implemented.");

        protected override PICInstruction DecodeRevisionIDInstruction()
            => throw new NotImplementedException("Disassembly of Revision ID memory region is not yet implemented.");

        protected override PICInstruction DecodeDTInstruction()
            => throw new NotImplementedException("Disassembly of DT memory region is not yet implemented.");

        protected override PICInstruction DecodeDTMInstruction()
            => throw new NotImplementedException("Disassembly of DTM memory region is not yet implemented.");

        #endregion

    }

}
