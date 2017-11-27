#region License
/* 
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

using Microchip.Crownking;
using Microchip.Utils;
using Reko.Core;
using System;

namespace Reko.Arch.Microchip.PIC18
{
    public class PIC18Disassembler : DisassemblerBase<PIC18Instruction>
    {
        #region Locals

        private EndianImageReader rdr;
        private PIC18Architecture arch;
        private PIC18Instruction instrCur;
        private Address addrCur;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the PIC18 execution mode.
        /// </summary>
        public PICExecMode ExecMode { get; set; } = PICExecMode.Traditional;

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public InstructionSetID InstructionSetID { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rdr">The memory reader.</param>
        /// <param name="instrsetID">Identifier for the PIC instruction set.</param>
        public PIC18Disassembler(PIC18Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.InstructionSetID = arch.PICDescriptor?.GetInstructionSetID ?? InstructionSetID.PIC18_ENHANCED;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disassemble a single instruction. Return null if the end of the reader has been reached.
        /// </summary>
        /// <returns>
        /// A PIC18Instruction.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        public override PIC18Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            addrCur = rdr.Address;
            if ((addrCur.Offset & 1) != 0)
                throw new AddressCorrelatedException(addrCur, $"Attempt to disassemble at odd address : {addrCur.ToString()}.");
            ushort uInstr;
            if (!rdr.TryReadUInt16(out uInstr))
                return null;
            try
            {
                var op = uInstr >> 12;
                instrCur = opcodesTable[op].Decode(uInstr, this);
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} code.");
            }
            if (instrCur == null)
            {
                return new PIC18Instruction(Opcode.invalid, ExecMode) { Address = addrCur };
            }
            instrCur.Address = addrCur;
            instrCur.Length = (int)(rdr.Address - addrCur);
            return instrCur;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets additional instruction's word. Check for consistency (like a NOP instruction) and provides the 12 least-significant bits.
        /// </summary>
        /// <param name="rdr">The memory reader.</param>
        /// <param name="w">[out] an ushort to fill in.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown <paramref name="rdr"/> argument is null.</exception>
        private static bool _getAddlWord(EndianImageReader rdr, out ushort w)
        {
            if (rdr == null) throw new ArgumentNullException(nameof(rdr));
            if (!rdr.TryReadBeUInt16(out w)) return false;
            if ((w & 0xF000U) != 0xF000U) return false;
            w &= (ushort)0xFFFU;
            return true;
        }

        /// <summary>
        /// Gets an invalid instruction.
        /// </summary>
        /// <returns>
        /// A PIC18Instruction.
        /// </returns>
        private PIC18Instruction Invalid()
        {
            return new PIC18Instruction(Opcode.invalid, ExecMode);
        }

        private abstract class Decoder
        {
            public abstract PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm);
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

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return decoders[uInstr.Extract(bitpos, width)].Decode(uInstr, dasm);
            }
        }

        /// <summary>
        /// Invalid instruction.
        /// </summary>
        private class InvalidOpRec : Decoder
        {
            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return dasm.Invalid();
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

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode);
            }
        }

        /// <summary>
        /// Instruction RESET.
        /// </summary>
        private class ResetOpRec : Decoder
        {
            private Opcode opcode;

            public ResetOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (uInstr == 0x00FFU)
                    return new PIC18Instruction(opcode, dasm.ExecMode);
                return dasm.Invalid();
            }
        }

        /// <summary>
        /// Instruction with no operand (PIC18 extended or later).
        /// </summary>
        private class ExtdNoOperandOpRec : Decoder
        {
            private Opcode opcode;

            public ExtdNoOperandOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED)
                    return dasm.Invalid();
                return new PIC18Instruction(opcode, dasm.ExecMode);
            }
        }

        /// <summary>
        /// Instructions ADDFSR, SUBFSR (PIC18 extended or later).
        /// </summary>
        private class FsrArithOpRec : Decoder
        {
            private Opcode opcode;

            public FsrArithOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED)
                    return dasm.Invalid();
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18FSRNumOperand((byte)uInstr.Extract(6, 2)),
                    op2 = new PIC18Immed6Operand((byte)uInstr.Extract(0, 6))
                };
            }
        }

        /// <summary>
        /// Instruction in the form '....-bbba-ffff-ffff' (BTG, BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        private class MemBitOpRec : Decoder
        {
            private Opcode opcode;

            public MemBitOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18BankMemoryOperand((byte)uInstr.Extract(0, 8)),
                    op2 = new PIC18BitNumberOperand((byte)uInstr.Extract(9, 3)),
                    op3 = new PIC18AccessRAMOperand(uInstr.Extract(8, 1))
                };
            }
        }

        /// <summary>
        /// Instruction in the form '....-...a-ffff-ffff' (MULWF, CPFSLT, CPFSEQ, CPFSGT, SETF, CLRF, NEGF, MOVWF)
        /// </summary>
        private class MemBankedOpRec : Decoder
        {
            private Opcode opcode;

            public MemBankedOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18BankMemoryOperand((byte)uInstr.Extract(0, 8)),
                    op2 = new PIC18AccessRAMOperand(uInstr.Extract(8, 1))
                };
            }
        }

        /// <summary>
        /// Instruction in the form '....-..da-ffff-ffff' (DECF, IORWF, ANDWF, XORWF, COMF, ADDWFC, ADDWF, INCF, DECFSZ, RRCF, RLCF, SWAPF, INCFSZ, RRNCF, RLNCF, INFSNZ, DCFSNZ, MOVF, SUBFWB, SUBWFB, SUBWF, TSTFSZ)
        /// </summary>
        private class MemBankedWregOpRec : Decoder
        {
            private Opcode opcode;

            public MemBankedWregOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18BankMemoryOperand((byte)uInstr.Extract(0, 8)),
                    op2 = new PIC18WdestOperand(uInstr.Extract(9, 1)),
                    op3 = new PIC18AccessRAMOperand(uInstr.Extract(8, 1))
                };
            }
        }

        /// <summary>
        /// Instruction in the form '....-....-kkkk-kkkk' ()
        /// </summary>
        private class Immed8OpRec : Decoder
        {
            private Opcode opcode;

            public Immed8OpRec(Opcode opc)
            {
                opcode = opc;
            }
            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Immed8Operand((byte)uInstr.Extract(0, 8))
                };
            }
        }

        /// <summary>
        /// Instruction MOVLB with '....-....-0000-kkkk' or '....-....-00kk-kkkk' immediate value.
        /// </summary>
        private class MovlbImmOpRec : Decoder
        {
            private Opcode opcode;

            public MovlbImmOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                byte bsrval = (byte)uInstr.Extract(0, 8);

                // Check for valid range on this literal for BSR register. Depends on PIC18 instruction set version.
                switch (dasm.InstructionSetID)
                {
                    case InstructionSetID.PIC18:
                    case InstructionSetID.PIC18_EXTENDED:
                        if (bsrval >= 16)
                            return dasm.Invalid();
                        return new PIC18Instruction(opcode, dasm.ExecMode)
                        {
                            op1 = new PIC18Immed4Operand(bsrval)
                        };
                    case InstructionSetID.PIC18_ENHANCED:
                        if (bsrval >= 64)
                            return dasm.Invalid();
                        return new PIC18Instruction(opcode, dasm.ExecMode)
                        {
                            op1 = new PIC18Immed6Operand(bsrval)
                        };
                }
                return dasm.Invalid();
            }
        }

        /// <summary>
        /// Instruction in the form '....-....-kkkk-kkkk, 1111-0000-kkkk-kkkk' ()
        /// </summary>
        private class Immed12OpRec : Decoder
        {
            private Opcode opcode;

            public Immed12OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                ushort lsw;
                if (!_getAddlWord(dasm.rdr, out lsw))
                    return dasm.Invalid();
                if (lsw >= 256)  // second word must be <xxxx 0000 kkkk kkkk>
                    return dasm.Invalid();

                var msw = uInstr.Extract(0, 4);
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Memory12bitAddrOperand((ushort)((msw << 8) | lsw))
                };
            }
        }

        /// <summary>
        /// Instruction with Shadow flag (RETFIE, RETURN)
        /// </summary>
        private class ShadowOpRec : Decoder
        {
            private Opcode opcode;

            public ShadowOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18ShadowOperand(uInstr.Extract(0, 1))
                };
            }
        }

        /// <summary>
        /// Short relative branch (BC, BN, BZ, ...) decoder.
        /// </summary>
        private class TargetRel8OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel8OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Rel8Operand((sbyte)(uInstr.ExtractSignExtend(0, 8)), dasm.addrCur)
                };
            }
        }

        /// <summary>
        /// Long relative branch (BRA, RCALL) decoder.
        /// </summary>
        private class TargetRel11OpRec : Decoder
        {
            private Opcode opcode;

            public TargetRel11OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Rel11Operand(uInstr.ExtractSignExtend(0, 11), dasm.addrCur)
                };
            }
        }

        /// <summary>
        /// Instruction TBLRD, TBLWT decoder
        /// </summary>
        private class TblOpRec : Decoder
        {
            private Opcode opcode;

            public TblOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18TBLOperand(uInstr.Extract(0, 2))
                };
            }
        }

        /// <summary>
        /// Instruction MOVFF decoder.
        /// </summary>
        private class MovffOpRec : Decoder
        {
            private Opcode opcode;

            public MovffOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                ushort word2;  // This a 2-words instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Memory12bitAddrOperand(uInstr.Extract(0, 12)),
                    op2 = new PIC18Memory12bitAddrOperand(word2.Extract(0, 12)),
                };
            }
        }

        /// <summary>
        /// Instruction GOTO decoder.
        /// </summary>
        private class TargetAbs20OpRec : Decoder
        {
            private Opcode opcode;

            public TargetAbs20OpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                ushort word2;  // This a 2-words instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();
                uint dstaddr = (uint)((uInstr.Extract(0, 8) << 12) | word2.Extract(0, 12));

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18CodeAddr20Operand(dstaddr),
                };
            }
        }

        /// <summary>
        /// Instruction CALL decoder.
        /// </summary>
        private class CallOpRec : Decoder
        {
            private Opcode opcode;

            public CallOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                ushort word2;  // This is a 2-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();
                uint dstaddr = (uint)((uInstr.Extract(0, 8) << 12) | word2.Extract(0, 12));

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18CodeAddr20Operand(dstaddr),
                    op2 = new PIC18ShadowOperand(uInstr.Extract(8, 1)),
                };
            }
        }

        /// <summary>
        /// Instruction MOVFFL decoder. (Enhanced PIC)
        /// </summary>
        private class MovfflOpRec : Decoder
        {
            private Opcode opcode;

            public MovfflOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.InstructionSetID < InstructionSetID.PIC18_ENHANCED) // Only supported by PIC18 Enhanced.
                    return dasm.Invalid();
                ushort word2, word3;  // This is a 3-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2) || !_getAddlWord(dasm.rdr, out word3))
                    return dasm.Invalid();
                ushort srcaddr = (ushort)((uInstr.Extract(0, 4) << 10) | word2.Extract(2, 10));
                ushort dstaddr = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Memory14bitAddrOperand(srcaddr),
                    op2 = new PIC18Memory14bitAddrOperand(dstaddr),
                };
            }
        }

        /// <summary>
        /// Instruction LFSR decoder.
        /// </summary>
        private class LfsrOpRec : Decoder
        {
            private Opcode opcode;

            public LfsrOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                ushort word2;  // This is a 2-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();

                switch (dasm.InstructionSetID)
                {
                    case InstructionSetID.PIC18:
                    case InstructionSetID.PIC18_EXTENDED:
                        if (word2 >= 256) // Second word must be 'xxxx-0000-kkkk-kkkk'
                            return dasm.Invalid();
                        return new PIC18Instruction(opcode, dasm.ExecMode)
                        {
                            op1 = new PIC18FSRNumOperand((byte)uInstr.Extract(4, 2)),
                            op2 = new PIC18Immed12Operand((ushort)((uInstr.Extract(0, 4) << 8) | word2))
                        };
                    case InstructionSetID.PIC18_ENHANCED:
                        if (word2 >= 1024) // Second word must be 'xxxx-00kk-kkkk-kkkk'
                            return dasm.Invalid();
                        return new PIC18Instruction(opcode, dasm.ExecMode)
                        {
                            op1 = new PIC18FSRNumOperand((byte)uInstr.Extract(4, 2)),
                            op2 = new PIC18Immed14Operand((ushort)((uInstr.Extract(0, 4) << 8) | word2))
                        };
                    default:
                        throw new InvalidOperationException($"Unknown PIC18 instruction set: {dasm.InstructionSetID}");
                }

            }
        }

        #region PIC18 Extended Execution mode only

        /// <summary>
        /// Instruction ADDULNK, SUBULNK decoder. (Extended Execution mode)
        /// </summary>
        private class LnkOpRec : Decoder
        {
            private Opcode opcode;

            public LnkOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Only supported by PIC18 running in Extended Execution mode.
                    return dasm.Invalid();

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Immed6Operand((byte)uInstr.Extract(0, 6))
                };
            }
        }

        /// <summary>
        /// Instruction MOVSF decoder. (Extended mode)
        /// </summary>
        private class MovsfOpRec : Decoder
        {
            private Opcode opcode;

            public MovsfOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // PIC running in Extended Execution mode?
                    return dasm.Invalid();
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // Only supported by PIC18 Extended and later.
                    return dasm.Invalid();

                ushort word2;  // This is a 2-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18FSRIdxOperand((byte)uInstr.Extract(0, 7)),
                    op2 = new PIC18Memory12bitAddrOperand(word2)
                };
            }
        }

        /// <summary>
        /// Instruction MOVSFL decoder. (Extended mode, Enhanced PIC)
        /// </summary>
        private class MovsflOpRec : Decoder
        {
            private Opcode opcode;

            public MovsflOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return dasm.Invalid();
                if (dasm.InstructionSetID < InstructionSetID.PIC18_ENHANCED) // ... and being a PIC18 Enhanced.
                    return dasm.Invalid();

                ushort word2, word3;  // This is a 3-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2) || !_getAddlWord(dasm.rdr, out word3))
                    return dasm.Invalid();
                byte zssource = (byte)word2.Extract(2, 7);
                ushort fsdest = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18FSRIdxOperand(zssource),
                    op2 = new PIC18Memory14bitAddrOperand(fsdest)
                };
            }
        }

        /// <summary>
        /// Instruction MOVSS decoder. (Extended mode)
        /// </summary>
        private class MovssOpRec : Decoder
        {
            private Opcode opcode;

            public MovssOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return dasm.Invalid();
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // ... and being a PIC18 Extended and later?
                    return dasm.Invalid();

                ushort word2;  // This is a 2-word instruction.
                if (!_getAddlWord(dasm.rdr, out word2))
                    return dasm.Invalid();

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18FSRIdxOperand((byte)uInstr.Extract(0, 7)),
                    op2 = new PIC18FSRIdxOperand((byte)word2.Extract(0, 7))
                };
            }
        }

        /// <summary>
        /// Instruction PUSHL decoder. (Extended mode)
        /// </summary>
        private class PushlOpRec : Decoder
        {
            private Opcode opcode;

            public PushlOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return dasm.Invalid();
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // ... and being a PIC18 Extended and later.
                    return dasm.Invalid();

                return new PIC18Instruction(opcode, dasm.ExecMode)
                {
                    op1 = new PIC18Immed8Operand((byte)uInstr.Extract(0, 8)),
                };
            }
        }

        #endregion

        /// <summary>
        /// The opcodes decoder table.
        /// </summary>
        private static Decoder[] opcodesTable = new Decoder[16]
        {
            // 0000 ???? .... ....
            new SubDecoder(8, 4, new Decoder[16]
            {
                // 0000 0000 ???? ....
                new SubDecoder(4, 4, new Decoder[16]
                {
                    // 0000 0000 0000 ????
                    new SubDecoder(0, 4, new Decoder[16]
                    {
                        // 0000 0000 0000 0000
                        new NoOperandOpRec(Opcode.nop),
                        // 0000 0000 0000 0001
                        new InvalidOpRec(),
                        // 0000 0000 0000 0010
                        new MovsflOpRec(Opcode.movsfl),
                        // 0000 0000 0000 0011
                        new NoOperandOpRec(Opcode.sleep),
                        // 0000 0000 0000 0100
                        new NoOperandOpRec(Opcode.clrwdt),
                        // 0000 0000 0000 0101
                        new NoOperandOpRec(Opcode.push),
                        // 0000 0000 0000 0110
                        new NoOperandOpRec(Opcode.pop),
                        // 0000 0000 0000 0111
                        new NoOperandOpRec(Opcode.daw),
                        // 0000 0000 0000 1000
                        new TblOpRec(Opcode.tblrd),
                        // 0000 0000 0000 1001
                        new TblOpRec(Opcode.tblrd),
                        // 0000 0000 0000 1010
                        new TblOpRec(Opcode.tblrd),
                        // 0000 0000 0000 1011
                        new TblOpRec(Opcode.tblrd),
                        // 0000 0000 0000 1100
                        new TblOpRec(Opcode.tblwt),
                        // 0000 0000 0000 1101
                        new TblOpRec(Opcode.tblwt),
                        // 0000 0000 0000 1110
                        new TblOpRec(Opcode.tblwt),
                        // 0000 0000 0000 1111
                        new TblOpRec(Opcode.tblwt)
                    } ),
                    // 0000 0000 0001 ????
                    new SubDecoder(0, 4, new Decoder[16]
                    {
                        // 0000 0000 0001 0000
                        new ShadowOpRec(Opcode.retfie),
                        // 0000 0000 0001 0001
                        new ShadowOpRec(Opcode.retfie),
                        // 0000 0000 0001 0010
                        new ShadowOpRec(Opcode.@return),
                        // 0000 0000 0001 0011
                        new ShadowOpRec(Opcode.@return),
                        // 0000 0000 0001 0100
                        new ExtdNoOperandOpRec(Opcode.callw),
                        // 0000 0000 0001 0101
                        new InvalidOpRec(),
                        // 0000 0000 0001 0110
                        new InvalidOpRec(),
                        // 0000 0000 0001 0111
                        new InvalidOpRec(),
                        // 0000 0000 0001 1000
                        new InvalidOpRec(),
                        // 0000 0000 0001 1001
                        new InvalidOpRec(),
                        // 0000 0000 0001 1010
                        new InvalidOpRec(),
                        // 0000 0000 0001 1011
                        new InvalidOpRec(),
                        // 0000 0000 0001 1100
                        new InvalidOpRec(),
                        // 0000 0000 0001 1101
                        new InvalidOpRec(),
                        // 0000 0000 0001 1110
                        new InvalidOpRec(),
                        // 0000 0000 0001 1111
                        new InvalidOpRec(),
                    } ),
                    // 0000 0000 0010 ....
                    new InvalidOpRec(),
                    // 0000 0000 0011 ....
                    new InvalidOpRec(),
                    // 0000 0000 0100 ....
                    new InvalidOpRec(),
                    // 0000 0000 0101 ....
                    new InvalidOpRec(),
                    // 0000 0000 0110 ....
                    new MovfflOpRec(Opcode.movffl),
                    // 0000 0000 0111 ....
                    new InvalidOpRec(),
                    // 0000 0000 1000
                    new InvalidOpRec(),
                    // 0000 0000 1001
                    new InvalidOpRec(),
                    // 0000 0000 1010
                    new InvalidOpRec(),
                    // 0000 0000 1011
                    new InvalidOpRec(),
                    // 0000 0000 1100
                    new InvalidOpRec(),
                    // 0000 0000 1101
                    new InvalidOpRec(),
                    // 0000 0000 1110
                    new InvalidOpRec(),
                    // 0000 0000 1111
                    new ResetOpRec(Opcode.reset),
                } ),
                // 0000 0001
                new MovlbImmOpRec(Opcode.movlb),
                // 0000 0010 .... ....
                new MemBankedWregOpRec(Opcode.mulwf),
                // 0000 0011 .... ....
                new MemBankedWregOpRec(Opcode.mulwf),
                // 0000 0100 .... ....
                new MemBankedWregOpRec(Opcode.decf),
                // 0000 0101 .... ....
                new MemBankedWregOpRec(Opcode.decf),
                // 0000 0110 .... ....
                new MemBankedWregOpRec(Opcode.decf),
                // 0000 0111 .... ....
                new MemBankedWregOpRec(Opcode.decf),
                // 0000 1000 .... ....
                new Immed8OpRec(Opcode.sublw),
                // 0000 1001 .... ....
                new Immed8OpRec(Opcode.iorlw),
                // 0000 1010 .... ....
                new Immed8OpRec(Opcode.xorlw),
                // 0000 1011 .... ....
                new Immed8OpRec(Opcode.andlw),
                // 0000 1100 .... ....
                new Immed8OpRec(Opcode.retlw),
                // 0000 1101 .... ....
                new Immed8OpRec(Opcode.mullw),
                // 0000 1110 .... ....
                new Immed8OpRec(Opcode.movlw),
                // 0000 1111 .... ....
                new Immed8OpRec(Opcode.addlw),
            } ),
            // 0001 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0001 00.. .... ....
                new MemBankedWregOpRec(Opcode.iorwf),
                // 0001 01.. .... ....
                new MemBankedWregOpRec(Opcode.andwf),
                // 0001 10.. .... ....
                new MemBankedWregOpRec(Opcode.xorwf),
                // 0001 11.. .... ....
                new MemBankedWregOpRec(Opcode.comf),
            } ),
            // 0010 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0010 00.. .... ....
                new MemBankedWregOpRec(Opcode.addwfc),
                // 0010 01.. .... ....
                new MemBankedWregOpRec(Opcode.addwf),
                // 0010 10.. .... ....
                new MemBankedWregOpRec(Opcode.incf),
                // 0010 11.. .... ....
                new MemBankedWregOpRec(Opcode.decfsz),
            } ),
            // 0011 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0011 00.. .... ....
                new MemBankedWregOpRec(Opcode.rrcf),
                // 0011 01.. .... ....
                new MemBankedWregOpRec(Opcode.rlcf),
                // 0011 10.. .... ....
                new MemBankedWregOpRec(Opcode.swapf),
                // 0011 11.. .... ....
                new MemBankedWregOpRec(Opcode.incfsz),
            } ),
            // 0100 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0100 00.. .... ....
                new MemBankedWregOpRec(Opcode.rrncf),
                // 0100 01.. .... ....
                new MemBankedWregOpRec(Opcode.rlncf),
                // 0100 10.. .... ....
                new MemBankedWregOpRec(Opcode.infsnz),
                // 0100 11.. .... ....
                new MemBankedWregOpRec(Opcode.dcfsnz),
            } ),
            // 0101 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0101 00.. .... ....
                new MemBankedWregOpRec(Opcode.movf),
                // 0101 01.. .... ....
                new MemBankedWregOpRec(Opcode.subfwb),
                // 0101 10.. .... ....
                new MemBankedWregOpRec(Opcode.subwfb),
                // 0101 11.. .... ....
                new MemBankedWregOpRec(Opcode.subwf),
            } ),
            // 0110 ???. .... ....
            new SubDecoder(9, 3, new Decoder[8]
            {
                // 0110 000. .... ....
                new MemBankedOpRec(Opcode.cpfslt),
                // 0110 001. .... ....
                new MemBankedOpRec(Opcode.cpfseq),
                // 0110 010. .... ....
                new MemBankedOpRec(Opcode.cpfsgt),
                // 0110 011. .... ....
                new MemBankedOpRec(Opcode.tstfsz),
                // 0110 100. .... ....
                new MemBankedOpRec(Opcode.setf),
                // 0110 101. .... ....
                new MemBankedOpRec(Opcode.clrf),
                // 0110 110
                new MemBankedOpRec(Opcode.negf),
                // 0110 111. .... ....
                new MemBankedOpRec(Opcode.movwf),
            } ),
            // 0111 .... .... ....
            new MemBitOpRec(Opcode.btg),
            // 1000 .... .... ....
            new MemBitOpRec(Opcode.bsf),
            // 1001 .... .... ....
            new MemBitOpRec(Opcode.bcf),
            // 1010 .... .... ....
            new MemBitOpRec(Opcode.btfss),
            // 1011 .... .... ....
            new MemBitOpRec(Opcode.btfsc),
            // 1100 .... .... ....
            new MovffOpRec(Opcode.movff),
            // 1101 ?... .... ....
            new SubDecoder(11, 1 , new Decoder[2]
                {
                // 1101 0... .... ....
                new TargetRel11OpRec(Opcode.bra),
                // 1101 1... .... ....
                new TargetRel11OpRec(Opcode.rcall),
                } ),
            // 1110 ?... .... ....
            new SubDecoder(11, 1, new Decoder[2]
            {
                // 1110 0??? .... ....
                new SubDecoder(8, 3, new Decoder[8]
                {
                    // 1110 0000 .... ....
                    new TargetRel8OpRec(Opcode.bz),
                    // 1110 0001 .... ....
                    new TargetRel8OpRec(Opcode.bnz),
                    // 1110 0010 .... ....
                    new TargetRel8OpRec(Opcode.bc),
                    // 1110 0011 .... ....
                    new TargetRel8OpRec(Opcode.bnc),
                    // 1110 0100 .... ....
                    new TargetRel8OpRec(Opcode.bov),
                    // 1110 0101 .... ....
                    new TargetRel8OpRec(Opcode.bnov),
                    // 1110 0110 .... ....
                    new TargetRel8OpRec(Opcode.bn),
                    // 1110 0111 .... ....
                    new TargetRel8OpRec(Opcode.bnn),
                } ),
                // 1110 1??? .... ....
                new SubDecoder(8, 3, new Decoder[8]
                {
                    // 1110 1000 ??.. ....
                    new SubDecoder(6, 2, new Decoder[4]
                    {
                        // 1110 1000 00.. ....
                        new LnkOpRec(Opcode.addulnk),
                        // 1110 1000 01.. ....
                        new FsrArithOpRec(Opcode.addfsr),
                        // 1110 1000 10.. ....
                        new FsrArithOpRec(Opcode.addfsr),
                        // 1110 1000 11.. ....
                        new FsrArithOpRec(Opcode.addfsr),
                    } ),
                    // 1110 1001 ??.. ....
                    new SubDecoder(6, 2, new Decoder[4]
                    {
                        // 1110 1001 00.. ....
                        new LnkOpRec(Opcode.subulnk),
                        // 1110 1001 01.. ....
                        new FsrArithOpRec(Opcode.subfsr),
                        // 1110 1001 10.. ....
                        new FsrArithOpRec(Opcode.subfsr),
                        // 1110 1001 11.. ....
                        new FsrArithOpRec(Opcode.subfsr),
                    } ),
                    // 1110 1010 .... ....
                    new PushlOpRec(Opcode.pushl),
                    // 1110 1011 ?... ....
                    new SubDecoder(7, 1, new Decoder[2]
                    {
                        // 1110 1011 0... ....
                        new MovsfOpRec(Opcode.movsf),
                        // 1110 1011 1... ....
                        new MovssOpRec(Opcode.movss),
                    } ),
                    // 1110 1100 .... ....
                    new CallOpRec(Opcode.call),
                    // 1110 1101 .... ....
                    new CallOpRec(Opcode.call),
                    // 1110 1110 .... ....
                    new LfsrOpRec(Opcode.lfsr),
                    // 1110 1111 .... ....
                    new TargetAbs20OpRec(Opcode.@goto),
                } ),
            } ),
            // 1111 .... .... ....
            new NoOperandOpRec(Opcode.nop),
        };

        #endregion

    }

}
