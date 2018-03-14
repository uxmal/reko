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

using Reko.Libraries.Microchip;
using Reko.Core;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// A Microchip PIC16 *partial* disassembler.
    /// Valid for most of program memory regions (code, eeprom, config, ...).
    /// For PIC16 instructions, only the instructions common to all PIC16 families are decoded here.
    /// Further decoding is required by each PIC family's disassembler.
    /// This 2-stage decoding permits to ease the maintenance, the tests and decreases the total size of the decoder tables
    /// </summary>
    public class PIC16DisassemblerBase : DisassemblerBase<PIC16Instruction>
    {

        public readonly EndianImageReader rdr;
        public readonly PIC16Architecture arch;
        public readonly PIC pic;

        protected PIC16Instruction instrCur;
        public Address addrCur;

        private static IMemoryRegion lastusedregion = null;


        /// <summary>
        /// Instantiates a base PIC16 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory reader.</param>
        public PIC16DisassemblerBase(PIC16Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            pic = this.arch.PICDescriptor;
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier. Must be overriden.
        /// </summary>
        public virtual InstructionSetID InstructionSetID => InstructionSetID.UNDEFINED;


        /// <summary>
        /// Disassemble a single instruction. Return null if the end of the reader has been reached.
        /// </summary>
        /// <returns>
        /// A <seealso cref="PIC18Instruction"/> instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        public override PIC16Instruction DisassembleInstruction()
        {
            IMemoryRegion GetProgRegion()
            {
                if (lastusedregion != null && lastusedregion.Contains(addrCur))
                    return lastusedregion;
                return lastusedregion = arch.MemoryDescriptor.MemoryMap.GetProgramRegion(addrCur);
            }

            if (!rdr.IsValid)
                return null;
            addrCur = rdr.Address;
            IMemoryRegion regn = GetProgRegion();
            if (regn is null)
                throw new InvalidOperationException($"Unable to retrieve program memory region for address {addrCur.ToString()}.");
            if ((addrCur.Offset % (regn.Trait?.LocSize ?? 1)) != 0)
            {
                instrCur = new PIC16Instruction(Opcode.unaligned) { Address = addrCur, Length = 1 };
                rdr.Offset += 1; // Consume only the first byte of the binary instruction.
                return instrCur;
            }

            switch (regn.SubtypeOfMemory)
            {
                case MemorySubDomain.Code:
                case MemorySubDomain.ExtCode:
                case MemorySubDomain.Debugger:
                    if (!rdr.TryReadUInt16(out ushort uInstr))
                        return null;
                    return DecodePIC16Instruction(uInstr, PICProgAddress.Ptr(addrCur));

                case MemorySubDomain.EEData:
                    return DisasmEEPROMInstruction();

                case MemorySubDomain.UserID:
                    return DisasmUserIDInstruction();

                case MemorySubDomain.DeviceConfig:
                    return DisasmConfigInstruction();

                case MemorySubDomain.DeviceID:
                    return DisasmDWInstruction();

                case MemorySubDomain.DeviceConfigInfo:  //TODO: Decode DCI
                case MemorySubDomain.DeviceInfoAry:     //TODO: Decode DIA 
                case MemorySubDomain.RevisionID:        //TODO: Decode Revision ID
                case MemorySubDomain.Test:
                case MemorySubDomain.Other:
                default:
                    throw new NotImplementedException($"Disassembly of '{regn.SubtypeOfMemory}' memory region is not yet implemented.");
            }

        }

        /// <summary>
        /// Disassembles PIC16 instructions common to all PIC16 families.
        /// </summary>
        /// <param name="addr">The program address of the instruction.</param>
        /// <param name="uInstr">The instruction binary 16-bit word.</param>
        /// <returns>
        /// A <see cref="PIC16Instruction"/> instance or null.
        /// </returns>
        protected virtual PIC16Instruction DecodePIC16Instruction(ushort uInstr, PICProgAddress addr)
        {
            try
            {
                instrCur = opcodesTable[uInstr.Extract(12, 2)].Decode(uInstr, addr);
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            // If there is a legal instruction, consume only one word.
            if (instrCur != null)
            {
                instrCur.Address = addrCur;
                instrCur.Length = 2;
            }
            return instrCur;
        }

        #region Instruction Decoder helpers

        protected abstract class Decoder
        {
            public abstract PIC16Instruction Decode(ushort uInstr, PICProgAddress addr);
        }

        protected class SubDecoder : Decoder
        {
            private int bitpos;
            private int width;
            private Decoder[] decoders;

            public SubDecoder(int bitpos, int width, Decoder[] decoders)
            {
                this.bitpos = (bitpos < 0 ? 0 : bitpos);
                this.width = (width <= 0 ? 1 : width);
                this.decoders = decoders;
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return decoders[uInstr.Extract(bitpos, width)].Decode(uInstr, addr);
            }
        }

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
            new WrongDecoder()                                      // 11 .... .... ....

        };

        /// <summary>
        /// Return <code>null</code> to indicate further decoding is required.
        /// </summary>
        protected class UseBaseDecode : Decoder
        {
            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return null;
            }
        }

        /// <summary>
        /// Forgot to define a valid entry in the decoder table. Should not occur...
        /// </summary>
        protected class WrongDecoder : Decoder
        {
            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                throw new InvalidOperationException($"BUG! Missing decoder entry for PIC16 instruction 0x{uInstr:X4} at {addr}");
            }
        }

        /// <summary>
        /// Invalid instruction. Not legal for any PIC16.
        /// </summary>
        protected class InvalidOpRec : Decoder
        {
            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(Opcode.invalid);
            }
        }

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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode);
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16DataBitOperand((byte)uInstr.Extract(0, 7),
                                                                    (byte)uInstr.Extract(7, 3)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode, new PIC16BankedOperand((byte)uInstr.Extract(0, 7)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16DataByteWithDestOperand((byte)uInstr.Extract(0, 7),
                                                                             (byte)uInstr.Extract(7, 1)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16Immed5Operand((byte)uInstr.Extract(0, 5)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16Immed7Operand((byte)uInstr.Extract(0, 7)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16Immed8Operand((byte)uInstr.Extract(0, 8)));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16ProgRel9AddrOperand(uInstr.ExtractSignExtend(0, 9),
                                                                         addr));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PIC16Instruction(opcode,
                                            new PIC16FSRArithOperand(fsrnum, lit));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (MOVIW k[n], MOVWI k[n])
        /// </summary>
        protected class FSRIndexedOpRec : Decoder
        {
            private Opcode opcode;

            public FSRIndexedOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                sbyte lit = (sbyte)uInstr.ExtractSignExtend(0, 6);
                return new PIC16Instruction(opcode,
                                            new PIC16FSRIndexedOperand(fsrnum, lit));
            }
        }

        /// <summary>
        /// Instruction MOVIW/MOVWI inc/dec decoder.
        /// </summary>
        protected class MoviIncDecOpRec : Decoder
        {
            private Opcode opcode;

            public MoviIncDecOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                byte fsrnum = (byte)uInstr.Extract(2, 1);
                byte modecode = (byte)uInstr.Extract(0, 2);
                return new PIC16Instruction(opcode, new PIC16FSRIncDecOperand(fsrnum, modecode));
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

            public override PIC16Instruction Decode(ushort uInstr, PICProgAddress addr)
            {
                uint dstaddr = uInstr.Extract(0, 11);
                return new PIC16Instruction(opcode, new PIC16ProgAbsAddrOperand(dstaddr));
            }
        }

        #endregion

        #region Pseudo-instructions disassemblers

        protected virtual PIC16Instruction DisasmEEPROMInstruction()
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
            instrCur = new PIC16Instruction(Opcode.DE, 
                                            new PIC16DataEEPROMOperand(bl.ToArray()))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected virtual PIC16Instruction DisasmDAInstruction()
        {

            if (!rdr.TryReadByte(out byte uDAByte))
                return null;
            instrCur = new PIC16Instruction(Opcode.DA,
                                            new PIC16DataASCIIOperand(uDAByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected virtual PIC16Instruction DisasmDBInstruction()
        {

            if (!rdr.TryReadByte(out byte uDBByte))
                return null;
            instrCur = new PIC16Instruction(Opcode.DB, 
                                            new PIC16DataByteOperand(uDBByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected virtual PIC16Instruction DisasmDWInstruction()
        {

            if (!rdr.TryReadUInt16(out ushort uDWWord))
                return null;
            instrCur = new PIC16Instruction(Opcode.DW, 
                                            new PIC16DataWordOperand(uDWWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected virtual PIC16Instruction DisasmUserIDInstruction()
        {

            if (!rdr.TryReadUInt16(out ushort uConfigWord))
                return null;
            instrCur = new PIC16Instruction(Opcode.__IDLOCS, 
                                            new PIC16IDLocsOperand(addrCur, uConfigWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        protected virtual PIC16Instruction DisasmConfigInstruction()
        {

            if (!rdr.TryReadUInt16(out ushort uConfigWord))
                return null;
            instrCur = new PIC16Instruction(Opcode.__CONFIG, 
                                            new PIC16ConfigOperand(arch, addrCur, uConfigWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        #endregion

    }

}
