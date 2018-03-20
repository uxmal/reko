#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    /// <summary>
    /// A Microchip PIC18 *partial* disassembler.
    /// Valid for most of program memory regions (code, eeprom, config, ...).
    /// For PIC18 instructions, only the instructions common to all PIC18 families are decoded here.
    /// For each PIC18 family (legacy, egg/extended, enhanced) a specific decoder must take care of
    /// decoding instructions that are specific to the said-target family and uses this inherited disassembler
    /// only for the balance of instructions.
    /// This 2-stage decoding permits to ease the maintenance, the tests and decreases the total size of the decoder tables
    /// </summary>
    public class PIC18DisassemblerBase : PICDisassemblerBase
    {
        protected PIC18DisassemblerBase(PICArchitecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        /// <summary>
        /// Gets the PIC18 execution mode this PIC18 disassembler is configured to.
        /// </summary>
        public override PICExecMode ExecMode => arch?.ExecMode ?? PICExecMode.Traditional;

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public override InstructionSetID InstructionSetID => arch?.PICDescriptor?.GetInstructionSetID ?? InstructionSetID.PIC18_ENHANCED;


        /// <summary>
        /// Disassembles a single PIC18 instruction.
        /// Only instructions whose binary values are common to all PIC18 families are decoded here.
        /// This method should be called by derived classes in case the binary instruction they deal
        /// with can't be decoded for a specific PIC18 instruction-set.
        /// </summary>
        /// <returns>
        /// A <see cref="PICInstruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            try
            {
                instrCur = opcodesTable[uInstr.Extract(12, 4)].Decode(uInstr, this);
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            if (instrCur != null)
            {
                instrCur.Address = addrCur;
                instrCur.Length = (int)(rdr.Address - addrCur);
                instrCur.ExecMode = ExecMode;
            }
            return instrCur;
        }

        /// <summary>
        /// Gets an additional instruction's word. Used for 2-word or 3-word instructions.
        /// Check for consistency (NOP-alike format) and provides the 12 least-significant bits.
        /// </summary>
        /// <param name="rdr">The memory reader.</param>
        /// <param name="w">[out] an 16-bit integer to fill in.</param>
        /// <returns>
        /// True if it succeeds, false if it fails. Reached end of memory or mis-formed binary word.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown <paramref name="rdr"/> argument is null.</exception>
        protected static bool GetAddlInstrWord(EndianImageReader rdr, out ushort w)
        {
            if (rdr is null)
                throw new ArgumentNullException(nameof(rdr));
            if (!rdr.TryReadUInt16(out w))
                return false;
            if ((w & 0xF000U) != 0xF000U)
                return false;
            w &= (ushort)0xFFFU;
            return true;
        }

        #region Instruction Decoder Helpers

        /// <summary>
        /// The common opcodes decoder table.
        /// </summary>
        private static Decoder[] opcodesTable = new Decoder[16]
        {
            new SubDecoder(8, 4, new Decoder[16] {                  // 0000 ???? .... ....
                new SubDecoder(4, 4, new Decoder[16] {              // 0000 0000 ???? ....
                    new SubDecoder(0, 4, new Decoder[16] {          // 0000 0000 0000 ????
                        new NoOperandOpRec(Opcode.NOP),             // 0000 0000 0000 0000
                        new InvalidOpRec(),                         // 0000 0000 0000 0001
                        new WrongDecoder(),                         // 0000 0000 0000 0010
                        new NoOperandOpRec(Opcode.SLEEP),           // 0000 0000 0000 0011
                        new NoOperandOpRec(Opcode.CLRWDT),          // 0000 0000 0000 0100
                        new NoOperandOpRec(Opcode.PUSH),            // 0000 0000 0000 0101
                        new NoOperandOpRec(Opcode.POP),             // 0000 0000 0000 0110
                        new NoOperandOpRec(Opcode.DAW),             // 0000 0000 0000 0111
                        new TblOpRec(Opcode.TBLRD),                 // 0000 0000 0000 1000
                        new TblOpRec(Opcode.TBLRD),                 // 0000 0000 0000 1001
                        new TblOpRec(Opcode.TBLRD),                 // 0000 0000 0000 1010
                        new TblOpRec(Opcode.TBLRD),                 // 0000 0000 0000 1011
                        new TblOpRec(Opcode.TBLWT),                 // 0000 0000 0000 1100
                        new TblOpRec(Opcode.TBLWT),                 // 0000 0000 0000 1101
                        new TblOpRec(Opcode.TBLWT),                 // 0000 0000 0000 1110
                        new TblOpRec(Opcode.TBLWT)                  // 0000 0000 0000 1111
                    }),
                    new SubDecoder(0, 4, new Decoder[16] {          // 0000 0000 0001 ????
                        new ShadowOpRec(Opcode.RETFIE),             // 0000 0000 0001 0000
                        new ShadowOpRec(Opcode.RETFIE),             // 0000 0000 0001 0001
                        new ShadowOpRec(Opcode.RETURN),             // 0000 0000 0001 0010
                        new ShadowOpRec(Opcode.RETURN),             // 0000 0000 0001 0011
                        new WrongDecoder(),                         // 0000 0000 0001 0100
                        new InvalidOpRec(),                         // 0000 0000 0001 0101
                        new InvalidOpRec(),                         // 0000 0000 0001 0110
                        new InvalidOpRec(),                         // 0000 0000 0001 0111
                        new InvalidOpRec(),                         // 0000 0000 0001 1000
                        new InvalidOpRec(),                         // 0000 0000 0001 1001
                        new InvalidOpRec(),                         // 0000 0000 0001 1010
                        new InvalidOpRec(),                         // 0000 0000 0001 1011
                        new InvalidOpRec(),                         // 0000 0000 0001 1100
                        new InvalidOpRec(),                         // 0000 0000 0001 1101
                        new InvalidOpRec(),                         // 0000 0000 0001 1110
                        new InvalidOpRec(),                         // 0000 0000 0001 1111
                    }),
                    new InvalidOpRec(),                             // 0000 0000 0010 ....
                    new InvalidOpRec(),                             // 0000 0000 0011 ....
                    new InvalidOpRec(),                             // 0000 0000 0100 ....
                    new InvalidOpRec(),                             // 0000 0000 0101 ....
                    new WrongDecoder(),                             // 0000 0000 0110 ffff + 1111 ffff ffff ffgg + 1111 gggg gggg gggg
                    new InvalidOpRec(),                             // 0000 0000 0111 ....
                    new InvalidOpRec(),                             // 0000 0000 1000 ....
                    new InvalidOpRec(),                             // 0000 0000 1001 ....
                    new InvalidOpRec(),                             // 0000 0000 1010 ....
                    new InvalidOpRec(),                             // 0000 0000 1011 ....
                    new InvalidOpRec(),                             // 0000 0000 1100 ....
                    new InvalidOpRec(),                             // 0000 0000 1101 ....
                    new InvalidOpRec(),                             // 0000 0000 1110 ....
                    new ResetOpRec(Opcode.RESET),                   // 0000 0000 1111 1111 
                }),
                new WrongDecoder(),                                 // 0000 0001 .... ....
                new MemoryAccessOpRec(Opcode.MULWF),                // 0000 001a ffff ffff
                new MemoryAccessOpRec(Opcode.MULWF),                // 0000 001a ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DECF),         // 0000 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DECF),         // 0000 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DECF),         // 0000 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DECF),         // 0000 01da ffff ffff
                new Immed8OpRec(Opcode.SUBLW),                      // 0000 1000 kkkk kkkk
                new Immed8OpRec(Opcode.IORLW),                      // 0000 1001 kkkk kkkk
                new Immed8OpRec(Opcode.XORLW),                      // 0000 1010 kkkk kkkk
                new Immed8OpRec(Opcode.ANDLW),                      // 0000 1011 kkkk kkkk
                new Immed8OpRec(Opcode.RETLW),                      // 0000 1100 kkkk kkkk
                new Immed8OpRec(Opcode.MULLW),                      // 0000 1101 kkkk kkkk
                new Immed8OpRec(Opcode.MOVLW),                      // 0000 1110 kkkk kkkk
                new Immed8OpRec(Opcode.ADDLW),                      // 0000 1111 kkkk kkkk
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 0001 ??.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.IORWF),        // 0001 00da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.ANDWF),        // 0001 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.XORWF),        // 0001 10da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.COMF),         // 0001 11da ffff ffff
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 0010 ??.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.ADDWFC),       // 0010 00da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.ADDWF),        // 0010 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.INCF),         // 0010 10da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DECFSZ),       // 0010 11da ffff ffff
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 0011 ??.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RRCF),         // 0011 00da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.RLCF),         // 0011 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.SWAPF),        // 0011 10da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.INCFSZ),       // 0011 11da ffff ffff
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 0100 ??.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RRNCF),        // 0100 00da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.RLNCF),        // 0100 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.INFSNZ),       // 0100 10da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.DCFSNZ),       // 0100 11da ffff ffff
            }),
            new SubDecoder(10, 2, new Decoder[4] {                  // 0101 ??.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.MOVF),         // 0101 00da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.SUBFWB),       // 0101 01da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.SUBWFB),       // 0101 10da ffff ffff
                new MemoryAccessWithDestOpRec(Opcode.SUBWF),        // 0101 11da ffff ffff
            }),
            new SubDecoder(9, 3, new Decoder[8] {                   // 0110 ???. .... ....
                new MemoryAccessOpRec(Opcode.CPFSLT),               // 0110 000. .... ....
                new MemoryAccessOpRec(Opcode.CPFSEQ),               // 0110 001. .... ....
                new MemoryAccessOpRec(Opcode.CPFSGT),               // 0110 010. .... ....
                new MemoryAccessOpRec(Opcode.TSTFSZ),               // 0110 011. .... ....
                new MemoryAccessOpRec(Opcode.SETF),                 // 0110 100. .... ....
                new MemoryAccessOpRec(Opcode.CLRF),                 // 0110 101. .... ....
                new MemoryAccessOpRec(Opcode.NEGF),                 // 0110 110 .... ....
                new MemoryAccessOpRec(Opcode.MOVWF),                // 0110 111. .... ....
            }),
            new MemoryBitAccessWithDestOpRec(Opcode.BTG),           // 0111 bbba ffff ffff
            new MemoryBitAccessWithDestOpRec(Opcode.BSF),           // 1000 bbba ffff ffff
            new MemoryBitAccessWithDestOpRec(Opcode.BCF),           // 1001 bbba ffff ffff
            new MemoryBitAccessWithDestOpRec(Opcode.BTFSS),         // 1010 bbba ffff ffff
            new MemoryBitAccessWithDestOpRec(Opcode.BTFSC),         // 1011 bbba ffff ffff
            new MovffOpRec(Opcode.MOVFF),                           // 1100 ffff ffff ffff + 1111 ffff ffff ffff
            new SubDecoder(11, 1 , new Decoder[2] {                 // 1101 ?... .... ....
                new TargetRel11OpRec(Opcode.BRA),                   // 1101 0nnn nnnn nnnn
                new TargetRel11OpRec(Opcode.RCALL),                 // 1101 1nnn nnnn nnnn
            }),
            new SubDecoder(11, 1, new Decoder[2] {                  // 1110 ?... .... ....
                new SubDecoder(8, 3, new Decoder[8] {               // 1110 0??? .... ....
                    new TargetRel8OpRec(Opcode.BZ),                 // 1110 0000 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BNZ),                // 1110 0001 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BC),                 // 1110 0010 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BNC),                // 1110 0011 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BOV),                // 1110 0100 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BNOV),               // 1110 0101 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BN),                 // 1110 0110 nnnn nnnn
                    new TargetRel8OpRec(Opcode.BNN),                // 1110 0111 nnnn nnnn
                }),
                new SubDecoder(8, 3, new Decoder[8] {               // 1110 1??? .... ....
                    new WrongDecoder(),                             // 1110 1000 .... ....
                    new WrongDecoder(),                             // 1110 1001 .... ....
                    new WrongDecoder(),                             // 1110 1010 .... ....
                    new WrongDecoder(),                             // 1110 1011 .... ....
                    new CallOpRec(Opcode.CALL),                     // 1110 110s kkkk kkkk + 1111 kkkk kkkk kkkk
                    new CallOpRec(Opcode.CALL),                     // 1110 110s kkkk kkkk + 1111 kkkk kkkk kkkk
                    new WrongDecoder(),                             // 1110 1110 .... ....
                    new TargetAbs20OpRec(Opcode.GOTO),              // 1110 1111 kkkk kkkk + 1111 kkkk kkkk kkkk
                }),
            }),
            new NoOperandOpRec(Opcode.NOP),                         // 1111 .... .... ....
        };

        protected abstract class Decoder
        {
            public abstract PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm);
        }

        protected class SubDecoder : Decoder
        {
            private int bitpos;
            private int width;
            private Decoder[] decoders;

            public SubDecoder(int bitpos, int width, Decoder[] decoders)
            {
                this.decoders = decoders ?? throw new ArgumentNullException(nameof(decoders));
                this.bitpos = (bitpos < 0 ? 0 : bitpos);
                this.width = (width <= 0 ? 1 : width);
                if (decoders.Length != (1 << width))
                    throw new ArgumentOutOfRangeException(nameof(width), "Wrong decoder table size.");
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return decoders[uInstr.Extract(bitpos, width)].Decode(uInstr, dasm);
            }
        }

        /// <summary>
        /// Forgot to define a valid entry in the decoder table. Should not occur...
        /// </summary>
        protected class WrongDecoder : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                throw new InvalidOperationException($"BUG! Missing decoder entry for PIC18 instruction 0x{uInstr:X4}.");
            }
        }

        /// <summary>
        /// Invalid instruction. Return <code>null</code> to indicate an invalid instruction.
        /// </summary>
        protected class InvalidOpRec : Decoder
        {
            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return new PICInstruction(Opcode.invalid);
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                return new PICInstruction(opcode) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction RESET.
        /// </summary>
        protected class ResetOpRec : Decoder
        {
            private Opcode opcode;

            public ResetOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (uInstr == 0x00FFU)
                    return new PICInstruction(opcode) { ExecMode = dasm.ExecMode };
                return null;
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-bbba-ffff-ffff'</code>  (BTG, BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        protected class MemoryBitAccessWithDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryBitAccessWithDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var adr = (byte)uInstr.Extract(0, 8);
                var acc = uInstr.Extract(8, 1);
                var bit = (byte)uInstr.Extract(9, 3);
                var operd = new PIC18DataBitAccessOperand(dasm.ExecMode, adr, acc, bit);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-...a-ffff-ffff'</code> (MULWF, CPFSLT, CPFSEQ, CPFSGT, SETF, CLRF, NEGF, MOVWF)
        /// </summary>
        protected class MemoryAccessOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryAccessOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var adr = (byte)uInstr.Extract(0, 8);
                var acc = uInstr.Extract(8, 1);
                var operd = new PIC18BankedAccessOperand(dasm.ExecMode, adr, acc);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-..da-ffff-ffff'</code>
        /// (DECF, IORWF, ANDWF, XORWF, COMF, ADDWFC, ADDWF, INCF, DECFSZ, RRCF, RLCF, SWAPF, INCFSZ, RRNCF, RLNCF, INFSNZ, DCFSNZ, MOVF, SUBFWB, SUBWFB, SUBWF, TSTFSZ)
        /// </summary>
        protected class MemoryAccessWithDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryAccessWithDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var adr = (byte)uInstr.Extract(0, 8);
                var acc = uInstr.Extract(8, 1);
                var dst = uInstr.Extract(9, 1);
                var operd = new PIC18DataByteAccessWithDestOperand(dasm.ExecMode, adr, acc, dst);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
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
                var imm = (byte)uInstr.Extract(0, 8);
                var operd = new PIC18Immed8Operand(imm);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-kkkk-kkkk, 1111-0000-kkkk-kkkk'</code>.
        /// </summary>
        protected class Immed12OpRec : Decoder
        {
            private Opcode opcode;

            public Immed12OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (!GetAddlInstrWord(dasm.rdr, out ushort lsw))
                    return null;
                if (lsw >= 256)  // second word must be <xxxx 0000 kkkk kkkk>
                    return null;

                var msw = uInstr.Extract(0, 4);
                var operd = new PIC18Data12bitAbsAddrOperand((ushort)((msw << 8) | lsw));
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction with Shadow flag (RETFIE, RETURN)
        /// </summary>
        protected class ShadowOpRec : Decoder
        {
            private Opcode opcode;

            public ShadowOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var operd = new PIC18ShadowOperand(uInstr.Extract(0, 1));
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Short relative branch (BC, BN, BZ, ...) decoder.
        /// </summary>
        protected class TargetRel8OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel8OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var operd = new PIC18ProgRel8AddrOperand((sbyte)(uInstr.ExtractSignExtend(0, 8)), dasm.addrCur);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Long relative branch (BRA, RCALL) decoder.
        /// </summary>
        protected class TargetRel11OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel11OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var operd = new PIC18ProgRel11AddrOperand(uInstr.ExtractSignExtend(0, 11), dasm.addrCur);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction TBLRD, TBLWT decoder
        /// </summary>
        protected class TblOpRec : Decoder
        {
            private Opcode opcode;

            public TblOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                var operd = new PIC18TableReadWriteOperand(uInstr.Extract(0, 2));
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction MOVFF decoder.
        /// </summary>
        protected class MovffOpRec : Decoder
        {
            private Opcode opcode;

            public MovffOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                // This a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;

                var opers = new PIC18Data12bitAbsAddrOperand(uInstr.Extract(0, 12));
                var operd = new PIC18Data12bitAbsAddrOperand(word2);
                return new PICInstruction(opcode, opers, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction GOTO decoder.
        /// </summary>
        protected class TargetAbs20OpRec : Decoder
        {
            private Opcode opcode;

            public TargetAbs20OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                // This a 2-words instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;
                uint dstaddr = (uint)(uInstr.Extract(0, 8) | (word2 << 8));
                var operd = new PIC18ProgAbsAddrOperand(dstaddr);
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
            }
        }

        /// <summary>
        /// Instruction CALL decoder.
        /// </summary>
        protected class CallOpRec : Decoder
        {
            private Opcode opcode;

            public CallOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;
                uint dstaddr = (uint)(uInstr.Extract(0, 8) | (word2 << 8));
                var operd = new PIC18ProgAbsAddrOperand(dstaddr);
                var shad = new PIC18ShadowOperand(uInstr.Extract(8, 1));
                return new PICInstruction(opcode, operd, shad) { ExecMode = dasm.ExecMode };
            }
        }

        #endregion

        #region Pseudo-instructions decoders

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
            instrCur = new PICInstruction(Opcode.DE, new PIC18DataEEPROMOperand(bl.ToArray()))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };

            return instrCur;
        }

        protected override PICInstruction DecodeDAInstruction()
        {

            if (!rdr.TryReadByte(out byte uDAByte))
                return null;
            instrCur = new PICInstruction(Opcode.DA, new PIC18DataASCIIOperand(uDAByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };

            return instrCur;
        }

        protected override PICInstruction DecodeDBInstruction()
        {

            if (!rdr.TryReadByte(out byte uDBByte))
                return null;
            instrCur = new PICInstruction(Opcode.DB, new PIC18DataByteOperand(uDBByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };

            return instrCur;
        }

        protected override PICInstruction DecodeDWInstruction()
        {

            if (!rdr.TryReadUInt16(out ushort uDWWord))
                return null;
            instrCur = new PICInstruction(Opcode.DW, new PIC18DataWordOperand(uDWWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };

            return instrCur;
        }

        protected override PICInstruction DecodeUserIDInstruction()
        {

            if (!rdr.TryReadByte(out byte uIDByte))
                return null;
            instrCur = new PICInstruction(Opcode.__IDLOCS, new PIC18IDLocsOperand(addrCur, uIDByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };

            return instrCur;
        }

        protected override PICInstruction DecodeConfigInstruction()
        {

            if (!rdr.TryReadByte(out byte uConfigByte))
                return null;
            instrCur = new PICInstruction(Opcode.CONFIG, new PIC18ConfigOperand(arch, addrCur, uConfigByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur),
                ExecMode = ExecMode
            };
            return instrCur;
        }

        protected override PICInstruction DecodeDCIInstruction() => throw new NotImplementedException();

        protected override PICInstruction DecodeDIAInstruction() => throw new NotImplementedException();

        protected override PICInstruction DecodeRevisionIDInstruction() => throw new NotImplementedException();

        protected override PICInstruction DecodeDTInstruction() => throw new NotImplementedException();

        protected override PICInstruction DecodeDTMInstruction() => throw new NotImplementedException();

        #endregion

    }

}
