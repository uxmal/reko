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
using Reko.Arch.Microchip.Common;
using Reko.Core;
using System;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// A Microchip PIC18 disassembler. Valid only for code program memory regions.
    /// </summary>
    public class PIC18Disassembler : DisassemblerBase<PIC18Instruction>
    {

        #region Locals

        private EndianImageReader rdr;
        private PIC18Architecture arch;
        private PIC18Instruction instrCur;
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
        public PIC18Disassembler(PIC18Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            pic = this.arch.PICDescriptor;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the PIC18 execution mode this disassembler is configured to.
        /// </summary>
        public PICExecMode ExecMode => arch?.ExecMode ?? PICExecMode.Traditional;

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public InstructionSetID InstructionSetID => arch?.PICDescriptor?.GetInstructionSetID ?? InstructionSetID.PIC18_ENHANCED;

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
        public override PIC18Instruction DisassembleInstruction()
        {
            IMemoryRegion GetProgRegion()
            {
                if (lastusedregion != null && lastusedregion.Contains(addrCur))
                    return lastusedregion;
                return lastusedregion = arch.MemoryMapper.PICMemoryMap.GetProgramRegion(addrCur);
            }

            if (!rdr.IsValid)
                return null;
            addrCur = rdr.Address;
            IMemoryRegion regn = GetProgRegion();
            if (regn is null)
                throw new InvalidOperationException($"Unable to retrieve program memory region for address {addrCur.ToString()}.");
            if ((addrCur.Offset % (regn.Trait?.LocSize??1)) != 0)
                throw new AddressCorrelatedException(addrCur, $"Attempt to disassemble at unaligned address {addrCur.ToString()} in region '{regn.RegionName}'.");

            switch (regn.SubtypeOfMemory)
            {
                case MemorySubDomain.Code:
                case MemorySubDomain.ExtCode:
                case MemorySubDomain.Debugger:
                    addrCur = PICProgAddress.Ptr(rdr.Address);
                    return DisasmPIC18Instruction();

                case MemorySubDomain.EEData:
                    return DisasmEEPROMInstruction();

                case MemorySubDomain.UserID:
                    return DisasmUserIDInstruction();

                case MemorySubDomain.DeviceConfig:
                    return DisasmConfigInstruction();

                case MemorySubDomain.DeviceID:
                    return DisasmDWInstruction();

                case MemorySubDomain.DeviceConfigInfo:
                case MemorySubDomain.DeviceInfoAry:
                case MemorySubDomain.RevisionID:
                case MemorySubDomain.Test:
                case MemorySubDomain.Other:
                default:
                    throw new NotImplementedException($"Disassembly of '{regn.SubtypeOfMemory}' memory region is not yet implemented.");
            }

        }

        /// <summary>
        /// Disassembles a "true" PIC18 instruction.
        /// </summary>
        /// <returns>
        /// A <see cref="PIC18Instruction"/> instance.
        /// </returns>
        /// <exception cref="AddressCorrelatedException">Thrown when the Address Correlated error
        ///                                              condition occurs.</exception>
        private PIC18Instruction DisasmPIC18Instruction()
        {
            // A PIC18 instruction can be 1, 2 or 3 words long. The 1st word (opcode) determines the actual length of the instruction.

            var offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            try
            {
                instrCur = opcodesTable[uInstr.Extract(12, 4)].Decode(uInstr, this);
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addrCur, ex, $"An exception occurred when disassembling {InstructionSetID.ToString()} binary code 0x{uInstr:X4}.");
            }

            // If there is no legal instruction, consume only one word and return an Illegal pseudo-instruction.
            if (instrCur is null)
            {
                instrCur = new PIC18Instruction(Opcode.invalid, ExecMode) { Address = addrCur };
                rdr.Offset = offset + 2; // Consume only the first word of the binary instruction.
            }
            instrCur.Address = addrCur;
            instrCur.Length = (int)(rdr.Address - addrCur);
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
        private static bool GetAddlInstrWord(EndianImageReader rdr, out ushort w)
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

        #endregion

        #region Instruction Decoder

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
        /// Invalid instruction. Return <code>null</code> to indicate an invalid instruction.
        /// </summary>
        private class InvalidOpRec : Decoder
        {
            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
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
                return null;
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
                    return null;
                return new PIC18Instruction(opcode, dasm.ExecMode);
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-bbba-ffff-ffff'</code>  (BTG, BSF, BCF, BTFSS, BTFSC)
        /// </summary>
        private class MemoryBitAccessWithDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryBitAccessWithDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18DataBitAccessOperand(dasm.ExecMode,
                                                                          (byte)uInstr.Extract(0, 8),
                                                                          uInstr.Extract(8, 1),
                                                                          (byte)uInstr.Extract(9, 3)));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-...a-ffff-ffff'</code> (MULWF, CPFSLT, CPFSEQ, CPFSGT, SETF, CLRF, NEGF, MOVWF)
        /// </summary>
        private class MemoryAccessOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryAccessOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18BankedAccessOperand(dasm.ExecMode,
                                                                         (byte)uInstr.Extract(0, 8),
                                                                         uInstr.Extract(8, 1)));
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-..da-ffff-ffff'</code>
        /// (DECF, IORWF, ANDWF, XORWF, COMF, ADDWFC, ADDWF, INCF, DECFSZ, RRCF, RLCF, SWAPF, INCFSZ, RRNCF, RLNCF, INFSNZ, DCFSNZ, MOVF, SUBFWB, SUBWFB, SUBWF, TSTFSZ)
        /// </summary>
        private class MemoryAccessWithDestOpRec : Decoder
        {
            private Opcode opcode;

            public MemoryAccessWithDestOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18DataByteAccessWithDestOperand(dasm.ExecMode,
                                                                                   (byte)uInstr.Extract(0, 8),
                                                                                   uInstr.Extract(8, 1),
                                                                                   uInstr.Extract(9, 1)));
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

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18Immed8Operand((byte)uInstr.Extract(0, 8)));
            }
        }

        /// <summary>
        /// Instruction MOVLB with <code>'....-....-0000-kkkk'</code> or <code>'....-....-00kk-kkkk'</code> immediate value.
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
                            return null;
                        return new PIC18Instruction(opcode,
                                                    dasm.ExecMode,
                                                    new PIC18Immed4Operand(bsrval));

                    case InstructionSetID.PIC18_ENHANCED:
                        if (bsrval >= 64)
                            return null;
                        return new PIC18Instruction(opcode,
                                                    dasm.ExecMode,
                                                    new PIC18Immed6Operand(bsrval));

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Instruction in the form <code>'....-....-kkkk-kkkk, 1111-0000-kkkk-kkkk'</code>.
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
                if (!GetAddlInstrWord(dasm.rdr, out ushort lsw))
                    return null;
                if (lsw >= 256)  // second word must be <xxxx 0000 kkkk kkkk>
                    return null;

                var msw = uInstr.Extract(0, 4);
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18Data12bitAbsAddrOperand((ushort)((msw << 8) | lsw)));
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
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18ShadowOperand(uInstr.Extract(0, 1)));
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

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18ProgRel8AddrOperand((sbyte)(uInstr.ExtractSignExtend(0, 8)),
                                                                          dasm.addrCur));
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
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18ProgRel11AddrOperand(uInstr.ExtractSignExtend(0, 11),
                                                                          dasm.addrCur));
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
                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18TableReadWriteOperand(uInstr.Extract(0, 2)));
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
                // This a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;

                return new PIC18Instruction(opcode,dasm.ExecMode,
                                            new PIC18Data12bitAbsAddrOperand(uInstr.Extract(0, 12)),
                                            new PIC18Data12bitAbsAddrOperand(word2));
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
                // This a 2-words instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;
                uint dstaddr = (uint)(uInstr.Extract(0, 8) | (word2 << 8));

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18ProgAbsAddrOperand(dstaddr));
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
                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;
                uint dstaddr = (uint)(uInstr.Extract(0, 8) | (word2 << 8));

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18ProgAbsAddrOperand(dstaddr),
                                            new PIC18ShadowOperand(uInstr.Extract(8, 1)));
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
                    return null;
                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return null;
                ushort srcaddr = (ushort)((uInstr.Extract(0, 4) << 10) | word2.Extract(2, 10));
                ushort dstaddr = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18Registers.NotAllowedMovlDest(dstaddr))
                    return null;

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18Data14bitAbsAddrOperand(srcaddr),
                                            new PIC18Data14bitAbsAddrOperand(dstaddr));
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
                byte fsrnum = (byte)uInstr.Extract(4, 4);
                if (fsrnum >= 3)
                    return null;

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;

                switch (dasm.InstructionSetID)
                {
                    case InstructionSetID.PIC18:
                    case InstructionSetID.PIC18_EXTENDED:
                        if (word2 > 0xFF) // Second word must be 'xxxx-0000-kkkk-kkkk'
                            return null;
                        return new PIC18Instruction(opcode,dasm.ExecMode,
                                                    new PIC18FSROperand(fsrnum),
                                                    new PIC18Immed12Operand((ushort)((uInstr.Extract(0, 4) << 8) | word2)));

                    case InstructionSetID.PIC18_ENHANCED:
                        if (word2 > 0x3FF) // Second word must be 'xxxx-00kk-kkkk-kkkk'
                            return null;
                        return new PIC18Instruction(opcode, dasm.ExecMode,
                                                    new PIC18FSROperand(fsrnum),
                                                    new PIC18Immed14Operand((ushort)((uInstr.Extract(0, 4) << 10) | word2)));

                    default:
                        throw new InvalidOperationException($"Unknown PIC18 instruction set: {dasm.InstructionSetID}");
                }

            }
        }

        #region PIC18 Extended Execution mode only

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
                byte fsrnum = (byte)uInstr.Extract(6, 2);
                if (fsrnum >= 3)
                    return null;
                switch (dasm.InstructionSetID)
                {
                    case InstructionSetID.PIC18:
                        return null;

                    case InstructionSetID.PIC18_EXTENDED:
                        if (dasm.ExecMode != PICExecMode.Extended)
                            return null;
                        break;

                    case InstructionSetID.PIC18_ENHANCED:
                        if (dasm.ExecMode != PICExecMode.Extended && (opcode != Opcode.ADDFSR && opcode != Opcode.SUBFSR))
                            return null;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18FSROperand(fsrnum),
                                            new PIC18Immed6Operand((byte)uInstr.Extract(0, 6)));
            }
        }

        /// <summary>
        /// Instructions ADDULNK, SUBULNK (PIC18 extended or later, extended execution mode).
        /// </summary>
        private class FsrULinkOpRec : Decoder
        {
            private Opcode opcode;

            public FsrULinkOpRec(Opcode opc)
            {
                opcode = opc;
            }

            public override PIC18Instruction Decode(ushort uInstr, PIC18Disassembler dasm)
            {
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED)
                    return null;
                if (dasm.ExecMode != PICExecMode.Extended) // Only supported by PIC18 running in Extended Execution mode.
                    return null;

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18Immed6Operand((byte)uInstr.Extract(0, 6)));
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
                    return null;
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // Only supported by PIC18 Extended and later.
                    return null;

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;

                var zs = (byte)uInstr.Extract(0, 7);
                var fd = word2;

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18Registers.NotAllowedMovlDest(fd))
                    return null;

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18FSR2IdxOperand(zs),
                                            new PIC18Data12bitAbsAddrOperand(fd));
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
                    return null;
                if (dasm.InstructionSetID < InstructionSetID.PIC18_ENHANCED) // ... and being a PIC18 Enhanced.
                    return null;

                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return null;
                byte zs = (byte)word2.Extract(2, 7);
                ushort fd = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18Registers.NotAllowedMovlDest(fd))
                    return null;

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18FSR2IdxOperand(zs),
                                            new PIC18Data14bitAbsAddrOperand(fd));
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
                    return null;
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // ... and being a PIC18 Extended and later?
                    return null;

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return null;

                var zs = (byte)uInstr.Extract(0, 7);
                var zd = (byte)word2.Extract(0, 7);

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18FSR2IdxOperand(zs),
                                            new PIC18FSR2IdxOperand(zd));
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
                    return null;
                if (dasm.InstructionSetID < InstructionSetID.PIC18_EXTENDED) // ... and being a PIC18 Extended and later.
                    return null;

                return new PIC18Instruction(opcode, dasm.ExecMode,
                                            new PIC18Immed8Operand((byte)uInstr.Extract(0, 8)));
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
                        new NoOperandOpRec(Opcode.NOP),
                        // 0000 0000 0000 0001
                        new InvalidOpRec(),
                        // 0000 0000 0000 0010
                        new MovsflOpRec(Opcode.MOVSFL),
                        // 0000 0000 0000 0011
                        new NoOperandOpRec(Opcode.SLEEP),
                        // 0000 0000 0000 0100
                        new NoOperandOpRec(Opcode.CLRWDT),
                        // 0000 0000 0000 0101
                        new NoOperandOpRec(Opcode.PUSH),
                        // 0000 0000 0000 0110
                        new NoOperandOpRec(Opcode.POP),
                        // 0000 0000 0000 0111
                        new NoOperandOpRec(Opcode.DAW),
                        // 0000 0000 0000 1000
                        new TblOpRec(Opcode.TBLRD),
                        // 0000 0000 0000 1001
                        new TblOpRec(Opcode.TBLRD),
                        // 0000 0000 0000 1010
                        new TblOpRec(Opcode.TBLRD),
                        // 0000 0000 0000 1011
                        new TblOpRec(Opcode.TBLRD),
                        // 0000 0000 0000 1100
                        new TblOpRec(Opcode.TBLWT),
                        // 0000 0000 0000 1101
                        new TblOpRec(Opcode.TBLWT),
                        // 0000 0000 0000 1110
                        new TblOpRec(Opcode.TBLWT),
                        // 0000 0000 0000 1111
                        new TblOpRec(Opcode.TBLWT)
                    } ),
                    // 0000 0000 0001 ????
                    new SubDecoder(0, 4, new Decoder[16]
                    {
                        // 0000 0000 0001 0000
                        new ShadowOpRec(Opcode.RETFIE),
                        // 0000 0000 0001 0001
                        new ShadowOpRec(Opcode.RETFIE),
                        // 0000 0000 0001 0010
                        new ShadowOpRec(Opcode.RETURN),
                        // 0000 0000 0001 0011
                        new ShadowOpRec(Opcode.RETURN),
                        // 0000 0000 0001 0100
                        new ExtdNoOperandOpRec(Opcode.CALLW),
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
                    new MovfflOpRec(Opcode.MOVFFL),
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
                    new ResetOpRec(Opcode.RESET),
                } ),
                // 0000 0001
                new MovlbImmOpRec(Opcode.MOVLB),
                // 0000 0010 .... ....
                new MemoryAccessOpRec(Opcode.MULWF),
                // 0000 0011 .... ....
                new MemoryAccessOpRec(Opcode.MULWF),
                // 0000 0100 .... ....
                new MemoryAccessWithDestOpRec(Opcode.DECF),
                // 0000 0101 .... ....
                new MemoryAccessWithDestOpRec(Opcode.DECF),
                // 0000 0110 .... ....
                new MemoryAccessWithDestOpRec(Opcode.DECF),
                // 0000 0111 .... ....
                new MemoryAccessWithDestOpRec(Opcode.DECF),
                // 0000 1000 .... ....
                new Immed8OpRec(Opcode.SUBLW),
                // 0000 1001 .... ....
                new Immed8OpRec(Opcode.IORLW),
                // 0000 1010 .... ....
                new Immed8OpRec(Opcode.XORLW),
                // 0000 1011 .... ....
                new Immed8OpRec(Opcode.ANDLW),
                // 0000 1100 .... ....
                new Immed8OpRec(Opcode.RETLW),
                // 0000 1101 .... ....
                new Immed8OpRec(Opcode.MULLW),
                // 0000 1110 .... ....
                new Immed8OpRec(Opcode.MOVLW),
                // 0000 1111 .... ....
                new Immed8OpRec(Opcode.ADDLW),
            } ),
            // 0001 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0001 00.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.IORWF),
                // 0001 01.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.ANDWF),
                // 0001 10.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.XORWF),
                // 0001 11.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.COMF),
            } ),
            // 0010 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0010 00.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.ADDWFC),
                // 0010 01.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.ADDWF),
                // 0010 10.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.INCF),
                // 0010 11.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.DECFSZ),
            } ),
            // 0011 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0011 00.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RRCF),
                // 0011 01.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RLCF),
                // 0011 10.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.SWAPF),
                // 0011 11.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.INCFSZ),
            } ),
            // 0100 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0100 00.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RRNCF),
                // 0100 01.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.RLNCF),
                // 0100 10.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.INFSNZ),
                // 0100 11.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.DCFSNZ),
            } ),
            // 0101 ??.. .... ....
            new SubDecoder(10, 2, new Decoder[4]
            {
                // 0101 00.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.MOVF),
                // 0101 01.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.SUBFWB),
                // 0101 10.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.SUBWFB),
                // 0101 11.. .... ....
                new MemoryAccessWithDestOpRec(Opcode.SUBWF),
            } ),
            // 0110 ???. .... ....
            new SubDecoder(9, 3, new Decoder[8]
            {
                // 0110 000. .... ....
                new MemoryAccessOpRec(Opcode.CPFSLT),
                // 0110 001. .... ....
                new MemoryAccessOpRec(Opcode.CPFSEQ),
                // 0110 010. .... ....
                new MemoryAccessOpRec(Opcode.CPFSGT),
                // 0110 011. .... ....
                new MemoryAccessOpRec(Opcode.TSTFSZ),
                // 0110 100. .... ....
                new MemoryAccessOpRec(Opcode.SETF),
                // 0110 101. .... ....
                new MemoryAccessOpRec(Opcode.CLRF),
                // 0110 110
                new MemoryAccessOpRec(Opcode.NEGF),
                // 0110 111. .... ....
                new MemoryAccessOpRec(Opcode.MOVWF),
            } ),
            // 0111 .... .... ....
            new MemoryBitAccessWithDestOpRec(Opcode.BTG),
            // 1000 .... .... ....
            new MemoryBitAccessWithDestOpRec(Opcode.BSF),
            // 1001 .... .... ....
            new MemoryBitAccessWithDestOpRec(Opcode.BCF),
            // 1010 .... .... ....
            new MemoryBitAccessWithDestOpRec(Opcode.BTFSS),
            // 1011 .... .... ....
            new MemoryBitAccessWithDestOpRec(Opcode.BTFSC),
            // 1100 .... .... ....
            new MovffOpRec(Opcode.MOVFF),
            // 1101 ?... .... ....
            new SubDecoder(11, 1 , new Decoder[2]
                {
                // 1101 0... .... ....
                new TargetRel11OpRec(Opcode.BRA),
                // 1101 1... .... ....
                new TargetRel11OpRec(Opcode.RCALL),
                } ),
            // 1110 ?... .... ....
            new SubDecoder(11, 1, new Decoder[2]
            {
                // 1110 0??? .... ....
                new SubDecoder(8, 3, new Decoder[8]
                {
                    // 1110 0000 .... ....
                    new TargetRel8OpRec(Opcode.BZ),
                    // 1110 0001 .... ....
                    new TargetRel8OpRec(Opcode.BNZ),
                    // 1110 0010 .... ....
                    new TargetRel8OpRec(Opcode.BC),
                    // 1110 0011 .... ....
                    new TargetRel8OpRec(Opcode.BNC),
                    // 1110 0100 .... ....
                    new TargetRel8OpRec(Opcode.BOV),
                    // 1110 0101 .... ....
                    new TargetRel8OpRec(Opcode.BNOV),
                    // 1110 0110 .... ....
                    new TargetRel8OpRec(Opcode.BN),
                    // 1110 0111 .... ....
                    new TargetRel8OpRec(Opcode.BNN),
                } ),
                // 1110 1??? .... ....
                new SubDecoder(8, 3, new Decoder[8]
                {
                    // 1110 1000 ??.. ....
                    new SubDecoder(6, 2, new Decoder[4]
                        {
                            new FsrArithOpRec(Opcode.ADDFSR),
                            new FsrArithOpRec(Opcode.ADDFSR),
                            new FsrArithOpRec(Opcode.ADDFSR),
                            new FsrULinkOpRec(Opcode.ADDULNK),
                        }),
                    // 1110 1001 ??.. ....
                    new SubDecoder(6, 2, new Decoder[4]
                        {
                            new FsrArithOpRec(Opcode.SUBFSR),
                            new FsrArithOpRec(Opcode.SUBFSR),
                            new FsrArithOpRec(Opcode.SUBFSR),
                            new FsrULinkOpRec(Opcode.SUBULNK),
                        }),
                    // 1110 1010 .... ....
                    new PushlOpRec(Opcode.PUSHL),
                    // 1110 1011 ?... ....
                    new SubDecoder(7, 1, new Decoder[2]
                    {
                        // 1110 1011 0... ....
                        new MovsfOpRec(Opcode.MOVSF),
                        // 1110 1011 1... ....
                        new MovssOpRec(Opcode.MOVSS),
                    } ),
                    // 1110 1100 .... ....
                    new CallOpRec(Opcode.CALL),
                    // 1110 1101 .... ....
                    new CallOpRec(Opcode.CALL),
                    // 1110 1110 .... ....
                    new LfsrOpRec(Opcode.LFSR),
                    // 1110 1111 .... ....
                    new TargetAbs20OpRec(Opcode.GOTO),
                } ),
            } ),
            // 1111 .... .... ....
            new NoOperandOpRec(Opcode.NOP),
        };

        #endregion

        #region Pseudo-instructions decoders

        private PIC18Instruction DisasmEEPROMInstruction()
        {

            if (!rdr.TryReadByte(out byte uEEByte))
                return null;
            instrCur = new PIC18Instruction(Opcode.DE, ExecMode,
                                            new PIC18DataEEPROMOperand(uEEByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC18Instruction DisasmDAInstruction()
        {

            if (!rdr.TryReadByte(out byte uDAByte))
                return null;
            instrCur = new PIC18Instruction(Opcode.DA, ExecMode,
                                            new PIC18DataASCIIOperand(uDAByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC18Instruction DisasmDBInstruction()
        {

            if (!rdr.TryReadByte(out byte uDBByte))
                return null;
            instrCur = new PIC18Instruction(Opcode.DB, ExecMode,
                                            new PIC18DataByteOperand(uDBByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC18Instruction DisasmDWInstruction()
        {

            if (!rdr.TryReadUInt16(out ushort uDWWord))
                return null;
            instrCur = new PIC18Instruction(Opcode.DW, ExecMode,
                                            new PIC18DataWordOperand(uDWWord))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC18Instruction DisasmUserIDInstruction()
        {

            if (!rdr.TryReadByte(out byte uIDByte))
                return null;
            instrCur = new PIC18Instruction(Opcode.IDLOCS, ExecMode,
                                            new PIC18DataByteOperand(uIDByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };

            return instrCur;
        }

        private PIC18Instruction DisasmConfigInstruction()
        {

            if (!rdr.TryReadByte(out byte uConfigByte))
                return null;
            instrCur = new PIC18Instruction(Opcode.CONFIG, ExecMode,
                                            new PIC18ConfigOperand(arch, addrCur, uConfigByte))
            {
                Address = addrCur,
                Length = (int)(rdr.Address - addrCur)
            };
            return instrCur;
        }

        #endregion

    }

}
