#region License
/* 
 * Copyright (C) 2017-2019 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2019 John Källén.
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
                instrCur = opcodesTable[uInstr.Extract(12, 2)].Decode(uInstr, this);
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

        private static Decoder[] opcodesTable = new Decoder[4]
        {
            new SubDecoder(10, 2, new Decoder[4] {                  // 00 ??.. .... ....
                new SubDecoder(8, 2, new Decoder[4] {               // 00 00?? .... ....
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0000 ?... ....
                        new WrongDecoder(),                         // 00 0000 0... ....
                        new MemoryByteOpRec(Opcode.MOVWF)           // 00 0000 1... ....
                    }),
                    new SubDecoder(7, 1, new Decoder[2] {           // 00 0001 ?... ....
                        new WrongDecoder(),                         // 00 0001 0... ....
                        new MemoryByteOpRec(Opcode.CLRF)            // 00 0001 1... ....
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
                new TargetAbs11OpRec(Opcode.CALL),                  // 10 0... .... ....
                new TargetAbs11OpRec(Opcode.GOTO)                   // 10 1... .... ....
            }),
            new WrongDecoder()                                      // 11 .... .... ....

        };

        /// <summary>
        /// Instruction with no operand.
        /// </summary>
        protected class NoOperandOpRec : Decoder
        {
            private Opcode opcode;

            public NoOperandOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return new PICInstructionNoOpnd(opcode);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-..bb-bfff-ffff'</code>  (BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        protected class MemoryBitOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryBitOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var bitno = (byte)uInstr.Extract(7, 3);
                var off = uInstr.Extract(0, 7);
                return new PICInstructionMemFB(opcode, off, bitno);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-....-.fff-ffff'</code>  (MOVWF)
        /// </summary>
        protected class MemoryByteOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryByteOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var fff = uInstr.Extract(0, 7);
                return new PICInstructionMemF(opcode, fff);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-....-dfff-ffff'</code>  (ADDWF, LSLF, IORWF, INCF, SWAPF, ...)
        /// </summary>
        protected class MemoryByteWDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryByteWDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var fff = uInstr.Extract(0, 7);
                var dest = uInstr.Extract(7, 1);
                return new PICInstructionMemFD(opcode, fff, dest);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-...k-kkkk'</code> (MOVLB, ...)
        /// </summary>
        protected class Immed5OpRec : Decoder
        {
            private Opcode opcode;

            public Immed5OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm5 = uInstr.Extract(0, 5);
                return new PICInstructionImmedByte(opcode, imm5);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.kkk-kkkk'</code> (MOVLP, ...)
        /// </summary>
        protected class Immed7OpRec : Decoder
        {
            private Opcode opcode;

            public Immed7OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm7 = uInstr.Extract(0, 7);
                return new PICInstructionImmedByte(opcode, imm7);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-kkkk-kkkk'</code> (ADDLW, MOVLW, RETLW, PUSHL, ...)
        /// </summary>
        protected class Immed8OpRec : Decoder
        {
            private Opcode opcode;

            public Immed8OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var imm8 = uInstr.Extract(0, 8);
                return new PICInstructionImmedByte(opcode, imm8);
            }
        }

        /// <summary>
        /// Relative branch (BRA) decoder.
        /// </summary>
        protected class TargetRel9OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel9OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var reloff = uInstr.ExtractSignExtend(0, 9);
                return new PICInstructionProgTarget(opcode, reloff, dasm.addrCur);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (ADDFSR)
        /// </summary>
        protected class FSRArithOpRec : Decoder
        {
            private Opcode opcode;

            public FSRArithOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PICInstructionFSRIArith(opcode, fsrnum, lit);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (MOVIW k[n], MOVWI k[n]) with -32 <= k <= 31
        /// </summary>
        protected class FSRIndexedOpRec : Decoder
        {
            private Opcode opcode;

            public FSRIndexedOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PICInstructionWithFSR(opcode, fsrnum, lit, FSRIndexedMode.INDEXED);
            }
        }

        /// <summary>
        /// Instruction MOVIW/MOVWI inc/dec decoder.
        /// </summary>
        protected class MoviIncDecOpRec : Decoder
        {
            private static readonly FSRIndexedMode[] code2FSRIdx = new FSRIndexedMode[4]
                { FSRIndexedMode.PREINC, FSRIndexedMode.PREDEC, FSRIndexedMode.POSTINC, FSRIndexedMode.POSTDEC };

            private Opcode opcode;

            public MoviIncDecOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(2, 1);
                byte modecode = (byte)uInstr.Extract(0, 2);
                return new PICInstructionWithFSR(opcode, fsrnum, 0, code2FSRIdx[modecode]);
            }
        }

        /// <summary>
        /// Instruction GOTO/CALL decoder.
        /// </summary>
        protected class TargetAbs11OpRec : Decoder
        {
            private Opcode opcode;

            public TargetAbs11OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var progAdr = uInstr.Extract(0, 11);
                return new PICInstructionProgTarget(opcode, progAdr);
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
            instrCur = new PICInstructionPseudo(Opcode.DE, new PICOperandDEEPROM(bl.ToArray()))
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
            instrCur = new PICInstructionPseudo(Opcode.DA, new PICOperandDASCII(uDAByte))
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
            instrCur = new PICInstructionPseudo(Opcode.DB, new PICOperandDByte(uDBByte))
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
            instrCur = new PICInstructionPseudo(Opcode.DW, new PICOperandDWord(uDWWord))
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
            instrCur = new PICInstructionPseudo(Opcode.__IDLOCS, new PICOperandIDLocs(addrCur, uConfigWord))
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
            instrCur = new PICInstructionPseudo(Opcode.__CONFIG, new PICOperandConfigBits(arch, cfgAddr, uConfigWord))
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
