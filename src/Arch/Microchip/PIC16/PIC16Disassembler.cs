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
    /// A Microchip PIC16 disassembler. Valid for most of code program memory regions.
    /// </summary>
    public class PIC16Disassembler : DisassemblerBase<PIC16Instruction>
    {
        #region Locals

        private EndianImageReader rdr;
        private PIC16Architecture arch;
        private PIC16Instruction instrCur;
        private PIC pic;
        private Address addrCur;
        private static IMemoryRegion lastusedregion = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rdr">The memory reader.</param>
        /// <param name="instrsetID">Identifier for the PIC instruction set.</param>
        public PIC16Disassembler(PIC16Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            pic = this.arch.PICDescriptor;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public InstructionSetID InstructionSetID => arch?.PICDescriptor?.GetInstructionSetID ?? InstructionSetID.PIC16_ENHANCED_V1;

        private bool PIC16Basic => InstructionSetID == InstructionSetID.PIC16;

        #endregion

        #region Public Methods

        /// <summary>
        /// Disassemble a single instruction. Return null if the end of the reader has been reached.
        /// </summary>
        /// <returns>
        /// A <seealso cref="PIC18Instruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
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
                    addrCur = PICProgAddress.Ptr(rdr.Address);
                    return DisasmPIC16Instruction();

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
        /// Disassembles a "true" PIC16 instruction.
        /// </summary>
        /// <returns>
        /// A <see cref="PIC18Instruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        private PIC16Instruction DisasmPIC16Instruction()
        {
            // A PIC16 instruction is 1 word long.

            var offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            try
            {
                if (PIC16Basic)
                {
                    instrCur = opcodesTable1[uInstr.Extract(12, 2)].Decode(uInstr, this);
                }
                else
                {
                    instrCur = opcodesTable2[uInstr.Extract(12, 2)].Decode(uInstr, this);
                }
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            // If there is no legal instruction, consume only one word and return an Illegal pseudo-instruction.
            if (instrCur is null)
            {
                instrCur = new PIC16Instruction(Opcode.invalid) { Address = addrCur };
                rdr.Offset = offset + 2; // Consume only the first word of the binary instruction.
            }
            instrCur.Address = addrCur;
            instrCur.Length = (int)(rdr.Address - addrCur);
            return instrCur;
        }

        #endregion

        #region Instruction Decoder

        private abstract class Decoder
        {
            public abstract PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm);
        }

        private class SubDecoder : Decoder
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

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return decoders[uInstr.Extract(bitpos, width)].Decode(uInstr, dasm);
            }
        }

        /// <summary>
        /// Invalid instruction. Return <code>null</code> to indicate an invalid instruction.
        /// </summary>
        private class InvalidOpRec : Decoder
        {
            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return null;
            }
        }

        /// <summary>
        /// Instruction with no operand.
        /// </summary>
        private class NoOperandOpRec : Decoder
        {
            private Opcode opcode;

            public NoOperandOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-..bb-bfff-ffff'</code>  (BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        private class MemoryBitOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryBitOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16DataBitOperand((byte)uInstr.Extract(0, 7),
                                                                    (byte)uInstr.Extract(7, 3)));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'..-....-dfff-ffff'</code>  (ADDWF, LSLF, IORWF, INCF, SWAPF, ...)
        /// </summary>
        private class MemoryByteWDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryByteWDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16DataByteWithDestOperand((byte)uInstr.Extract(0, 7),
                                                                             (byte)uInstr.Extract(7, 1)));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.kkk-kkkk'</code> (MOVLP, ...)
        /// </summary>
        private class Immed7OpRec : Decoder
        {
            private Opcode opcode;

            public Immed7OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16Immed7Operand((byte)uInstr.Extract(0, 7)));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-kkkk-kkkk'</code> (ADDLW, MOVLW, RETLW, PUSHL, ...)
        /// </summary>
        private class Immed8OpRec : Decoder
        {
            private Opcode opcode;

            public Immed8OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16Immed8Operand((byte)uInstr.Extract(0, 8)));
            }
        }

        /// <summary>
        /// Relative branch (BRA) decoder.
        /// </summary>
        private class TargetRel9OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel9OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                return new PIC16Instruction(opcode,
                                            new PIC16ProgRel9AddrOperand(uInstr.ExtractSignExtend(0, 9),
                                                                         dasm.addrCur));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-.nkk-kkkk'</code> (ADDFSR, MOVIW, MOVWI)
        /// </summary>
        private class Signed6WithFSROpRec : Decoder
        {
            private Opcode opcode;

            public Signed6WithFSROpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 1);
                short lit = uInstr.ExtractSignExtend(0, 6);
                return new PIC16Instruction(opcode,
                                            new PIC16FSROperand(fsrnum),
                                            new PIC16Signed6Operand(lit));
            }
        }

        /// <summary>
        /// Instruction GOTO/CALL decoder.
        /// </summary>
        private class TargetAbs11OpRec : Decoder
        {
            private Opcode opcode;

            public TargetAbs11OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                uint dstaddr = uInstr.Extract(0, 11);

                return new PIC16Instruction(opcode,
                                            new PIC16ProgAbsAddrOperand(dstaddr));
            }
        }

        /// <summary>
        /// Instruction GOTO/CALL decoder.
        /// </summary>
        private class MovIdx1OpRec : Decoder
        {
            private Opcode opcode;

            public MovIdx1OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC16Instruction Decode(ushort uInstr, PIC16Disassembler dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(2, 1);
                byte idxcode = (byte)uInstr.Extract(0, 2);

                return new PIC16Instruction(opcode,
                                            new PIC16FSROperand(fsrnum),
                                            new PIC16FSRIdxOperand(idxcode));
            }
        }

        // PIC16 Basic decoder
        private static Decoder[] opcodesTable1 = new Decoder[4]
        {
            // 00 ???? .... ....
            new SubDecoder(8, 4, new Decoder[16]
            {
            // 00 0000 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 00 0000 0??? ....
                    new SubDecoder(4, 3, null),
            // 00 0000 1... ....
                    new MemoryByteWDestOpRec(Opcode.MOVWF),
                }),
            // 00 0001 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 00 0001 0... ....
                    new NoOperandOpRec(Opcode.CLRW),
            // 00 0001 1... ....
                    new MemoryByteWDestOpRec(Opcode.CLRF),
                }),
            // 00 0010 .... ....
               new MemoryByteWDestOpRec(Opcode.SUBWF),
            // 00 0011 .... ....
               new MemoryByteWDestOpRec(Opcode.DECF),
            // 00 0100 .... ....
               new MemoryByteWDestOpRec(Opcode.IORWF),
            // 00 0101 .... ....
               new MemoryByteWDestOpRec(Opcode.ANDWF),
            // 00 0110 .... ....
               new MemoryByteWDestOpRec(Opcode.XORWF),
            // 00 0111 .... ....
               new MemoryByteWDestOpRec(Opcode.ADDWF),
            // 00 1000 .... ....
               new MemoryByteWDestOpRec(Opcode.MOVF),
            // 00 1001 .... ....
               new MemoryByteWDestOpRec(Opcode.COMF),
            // 00 1010 .... ....
               new MemoryByteWDestOpRec(Opcode.INCF),
            // 00 1011 .... ....
               new MemoryByteWDestOpRec(Opcode.DECFSZ),
            // 00 1100 .... ....
               new MemoryByteWDestOpRec(Opcode.RRF),
            // 00 1101 .... ....
               new MemoryByteWDestOpRec(Opcode.RLF),
            // 00 1110 .... ....
               new MemoryByteWDestOpRec(Opcode.SWAPF),
            // 00 1111 .... ....
               new MemoryByteWDestOpRec(Opcode.INCFSZ)
            }),
            // 01 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
            // 01 00.. .... ....
                new MemoryBitOpRec(Opcode.BCF),
            // 01 01.. .... ....
                new MemoryBitOpRec(Opcode.BSF),
            // 01 10.. .... ....
                new MemoryBitOpRec(Opcode.BTFSC),
            // 01 11.. .... ....
                new MemoryBitOpRec(Opcode.BTFSS)
            }),
            // 10 ?... .... ....
            new SubDecoder(11, 1, new Decoder[2]
            {
            // 10 0... .... ....
                new TargetAbs11OpRec(Opcode.GOTO),
            // 10 1... .... ....
                new TargetAbs11OpRec(Opcode.CALL)
            }),
            // 11 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
            // 11 00.. .... ....
                new Immed8OpRec(Opcode.MOVLW),
            // 11 01.. .... ....
                new Immed8OpRec(Opcode.RETLW),
            // 11 10?? .... ....
                new SubDecoder(8, 2, new Decoder[4]
                {
            // 11 1000 .... ....
                    new Immed8OpRec(Opcode.IORLW),
            // 11 1001 .... ....
                    new Immed8OpRec(Opcode.ANDLW),
            // 11 1010 .... ....
                    new Immed8OpRec(Opcode.XORLW),
            // 11 1011 .... ....
                    new Immed8OpRec(Opcode.XORLW)
                }),
            // 11 11?. .... ....
                new SubDecoder(9, 1, new Decoder[2]
                {
            // 11 110. .... ....
                    new Immed8OpRec(Opcode.SUBLW),
            // 11 111. .... ....
                    new Immed8OpRec(Opcode.ADDLW)
                })
            })
        };

        // PIC16 Enhanced decoder
        private static Decoder[] opcodesTable2 = new Decoder[4]
        {
            // 00 ???? .... ....
            new SubDecoder(8, 4, new Decoder[16]
            {
            // 00 0000 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 00 0000 0??? ....
                    new SubDecoder(4, 3, new Decoder[8]
                    {
            // 00 0000 0000 ....
            // 00 0000 0001 ?...
                        new SubDecoder(3, 1, new Decoder[2]
                        {
            // 00 0000 0001 0...
                            new MovIdx1OpRec(Opcode.MOVIW),
            // 00 0000 0001 1...
                            new MovIdx1OpRec(Opcode.MOVWI)
                        }),
            // 00 0000 0010 ....
            // 00 0000 0011 ....
            // 00 0000 0100 ....
                        new InvalidOpRec()
            // 00 0000 0101 ....
                        new InvalidOpRec()
            // 00 0000 0110 ....
            // 00 0000 0111 ....
                        new InvalidOpRec()
                    }),
            // 00 0000 1... ....
                    new MemoryByteWDestOpRec(Opcode.MOVWF),
                }),
            // 00 0001 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 00 0001 0... ....
                    new NoOperandOpRec(Opcode.CLRW),
            // 00 0001 1... ....
                    new MemoryByteWDestOpRec(Opcode.CLRF),
                }),
            // 00 0010 .... ....
               new MemoryByteWDestOpRec(Opcode.SUBWF),
            // 00 0011 .... ....
               new MemoryByteWDestOpRec(Opcode.DECF),
            // 00 0100 .... ....
               new MemoryByteWDestOpRec(Opcode.IORWF),
            // 00 0101 .... ....
               new MemoryByteWDestOpRec(Opcode.ANDWF),
            // 00 0110 .... ....
               new MemoryByteWDestOpRec(Opcode.XORWF),
            // 00 0111 .... ....
               new MemoryByteWDestOpRec(Opcode.ADDWF),
            // 00 1000 .... ....
               new MemoryByteWDestOpRec(Opcode.MOVF),
            // 00 1001 .... ....
               new MemoryByteWDestOpRec(Opcode.COMF),
            // 00 1010 .... ....
               new MemoryByteWDestOpRec(Opcode.INCF),
            // 00 1011 .... ....
               new MemoryByteWDestOpRec(Opcode.DECFSZ),
            // 00 1100 .... ....
               new MemoryByteWDestOpRec(Opcode.RRF),
            // 00 1101 .... ....
               new MemoryByteWDestOpRec(Opcode.RLF),
            // 00 1110 .... ....
               new MemoryByteWDestOpRec(Opcode.SWAPF),
            // 00 1111 .... ....
               new MemoryByteWDestOpRec(Opcode.INCFSZ)
            }),
            // 01 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
            // 01 00.. .... ....
                new MemoryBitOpRec(Opcode.BCF),
            // 01 01.. .... ....
                new MemoryBitOpRec(Opcode.BSF),
            // 01 10.. .... ....
                new MemoryBitOpRec(Opcode.BTFSC),
            // 01 11.. .... ....
                new MemoryBitOpRec(Opcode.BTFSS)
            }),
            // 10 ?... .... ....
            new SubDecoder(11, 1, new Decoder[2]
            {
                new TargetAbs11OpRec(Opcode.GOTO),
                new TargetAbs11OpRec(Opcode.CALL)
            }),
            // 11 ???? .... ....
            new SubDecoder(8, 4, new Decoder[16]
            {
            // 11 0000 .... ....
                new Immed8OpRec(Opcode.MOVLW),
            // 11 0001 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 11 0001 0... ....
                    new Signed6WithFSROpRec(Opcode.ADDFSR),
            // 11 0001 1... ....
                    new Immed7OpRec(Opcode.MOVLP)
                }),
            // 11 0010 .... ....
                new TargetRel9OpRec(Opcode.BRA),
            // 11 0011 .... ....
                new TargetRel9OpRec(Opcode.BRA),
            // 11 0100 .... ....
                new Immed8OpRec(Opcode.RETLW),
            // 11 0101 .... ....
                new MemoryByteWDestOpRec(Opcode.LSLF),
            // 11 0110 .... ....
                new MemoryByteWDestOpRec(Opcode.LSRF),
            // 11 0111 .... ....
                new MemoryByteWDestOpRec(Opcode.ASRF),
            // 11 1000 .... ....
                new Immed8OpRec(Opcode.IORLW),
            // 11 1001 .... ....
                new Immed8OpRec(Opcode.ANDLW),
            // 11 1010 .... ....
                new Immed8OpRec(Opcode.XORLW),
            // 11 1011 .... ....
                new MemoryByteWDestOpRec(Opcode.SUBWFB),
            // 11 1100 .... ....
                new Immed8OpRec(Opcode.SUBLW),
            // 11 1101 .... ....
                new MemoryByteWDestOpRec(Opcode.ADDWFC),
            // 11 1110 .... ....
                new Immed8OpRec(Opcode.ADDLW),
            // 11 1111 ?... ....
                new SubDecoder(7, 1, new Decoder[2]
                {
            // 11 1111 0... ....
                    new Signed6WithFSROpRec(Opcode.MOVIW),
            // 11 1111 1... ....
                    new Signed6WithFSROpRec(Opcode.MOVWI),
                }),
            })
        };

        #endregion

        #region Pseudo-instructions decoders

        private PIC16Instruction DisasmEEPROMInstruction()
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

        private PIC16Instruction DisasmDAInstruction()
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

        private PIC16Instruction DisasmDBInstruction()
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

        private PIC16Instruction DisasmDWInstruction()
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

        private PIC16Instruction DisasmUserIDInstruction()
        {

            if (!rdr.TryReadByte(out byte uIDByte))
                return null;
            instrCur = new PIC16Instruction(Opcode.__IDLOCS, 
                                            new PIC16IDLocsOperand(addrCur, uIDByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC16Instruction DisasmConfigInstruction()
        {

            if (!rdr.TryReadByte(out byte uConfigByte))
                return null;
            instrCur = new PIC16Instruction(Opcode.__CONFIG, 
                                            new PIC16ConfigOperand(arch, addrCur, uConfigByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        #endregion

    }

}
