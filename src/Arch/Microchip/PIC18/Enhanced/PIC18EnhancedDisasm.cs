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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;


namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    /// <summary>
    /// A Enhanced PIC18 family disassembler.
    /// </summary>
    public class PIC18EnhancedDisasm : PIC18LegacyDisasm
    {
        /// <summary>
        /// Instantiates a new Enhanced PIC18 disassembler.
        /// </summary>
        /// <param name="arch">The PIC architecture.</param>
        /// <param name="rdr">The memory segment reader.</param>
        private PIC18EnhancedDisasm(PICArchitecture arch, EndianImageReader rdr)
            : base(arch, rdr)
        {
        }

        public new static PICDisassemblerBase Create(PICArchitecture arch, EndianImageReader rdr)
        {
            return new PIC18EnhancedDisasm(
                arch ?? throw new ArgumentNullException(nameof(arch)),
                rdr ?? throw new ArgumentNullException(nameof(rdr))
                );
        }

        /// <summary>
        /// Gets the PIC instruction-set identifier.
        /// </summary>
        public override InstructionSetID InstructionSetID => InstructionSetID.PIC18_ENHANCED;

        /// <summary>
        /// Gets the PIC18 execution mode this PIC18 disassembler is configured to.
        /// </summary>
        public override PICExecMode ExecMode => arch.ExecMode;


        /// <summary>
        /// Gets the opcodes table corresponding to this PIC family.
        /// </summary>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            var offset = rdr.Offset;
            try
            {
                instrCur = enhancedOpcodesTable[uInstr.Extract(12, 4)].Decode(uInstr, this);
                if (instrCur is null)
                    instrCur = base.DecodePICInstruction(uInstr, addr); // Fall to common PIC18 instruction decoder
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
                instrCur = new PICInstruction(Opcode.invalid);
            }
            if (!instrCur.IsValid)
            {
                rdr.Offset = offset;
            }
            instrCur.Address = addrCur;
            instrCur.Length = (int)(rdr.Address - addrCur);
            instrCur.ExecMode = ExecMode;
            return instrCur;
        }

        /// <summary>
        /// The PIC18 Enhanced opcodes decoder table.
        /// </summary>
        private static Decoder[] enhancedOpcodesTable = new Decoder[16]
        {
            new SubDecoder(8, 4, new Decoder[16] {                  // 0000 ???? .... ....
                new SubDecoder(4, 4, new Decoder[16] {              // 0000 0000 ???? ....
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0000 ??..
                        new SubDecoder(0, 2, new Decoder[4] {       // 0000 0000 0000 00??
                        new NoOperandOpRec(Opcode.NOP),             // 0000 0000 0000 0000
                        new InvalidOpRec(),                         // 0000 0000 0000 0001
                        new MovsflOpRec(Opcode.MOVSFL),             // 0000 0000 0000 0010
                        new UseBaseDecode(),                        // 0000 0000 0000 0011
                        }),
                        new UseBaseDecode(),                        // 0000 0000 0000 01..
                        new UseBaseDecode(),                        // 0000 0000 0000 10..
                        new UseBaseDecode(),                        // 0000 0000 0000 11..
                    }),
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0001 ??..
                        new UseBaseDecode(),                        // 0000 0000 0001 00..
                        new SubDecoder(0, 2, new Decoder[4] {       // 0000 0000 0001 01??
                            new NoOperandOpRec(Opcode.CALLW),       // 0000 0000 0001 0100
                            new InvalidOpRec(),                     // 0000 0000 0001 0101
                            new InvalidOpRec(),                     // 0000 0000 0001 0110
                            new InvalidOpRec(),                     // 0000 0000 0001 0111
                        }),
                        new InvalidOpRec(),                         // 0000 0000 0001 10..
                        new InvalidOpRec(),                         // 0000 0000 0001 11..
                    }),
                    new InvalidOpRec(),                             // 0000 0000 0010 ....
                    new InvalidOpRec(),                             // 0000 0000 0011 ....
                    new InvalidOpRec(),                             // 0000 0000 0100 ....
                    new InvalidOpRec(),                             // 0000 0000 0101 ....
                    new MovfflOpRec(Opcode.MOVFFL),                 // 0000 0000 0110 ffff + 1111 ffff ffff ffgg + 1111 gggg gggg gggg
                    new InvalidOpRec(),                             // 0000 0000 0111 ....
                    new InvalidOpRec(),                             // 0000 0000 1000 ....
                    new InvalidOpRec(),                             // 0000 0000 1001 ....
                    new InvalidOpRec(),                             // 0000 0000 1010 ....
                    new InvalidOpRec(),                             // 0000 0000 1011 ....
                    new InvalidOpRec(),                             // 0000 0000 1100 ....
                    new InvalidOpRec(),                             // 0000 0000 1101 ....
                    new InvalidOpRec(),                             // 0000 0000 1110 ....
                    new UseBaseDecode(),                            // 0000 0000 1111 1111 
                }),
                new MovlbImmOpRec(Opcode.MOVLB),                    // 0000 0001 .... ....
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
                    new SubDecoder(6, 2, new Decoder[4] {           // 1110 1000 ??.. ....
                            new FsrArithOpRec(Opcode.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrArithOpRec(Opcode.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrArithOpRec(Opcode.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrULinkOpRec(Opcode.ADDULNK),      // 1110 1000 11kk kkkk
                    }),
                    new SubDecoder(6, 2, new Decoder[4] {           // 1110 1001 ??.. ....
                            new FsrArithOpRec(Opcode.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrArithOpRec(Opcode.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrArithOpRec(Opcode.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrULinkOpRec(Opcode.SUBULNK),      // 1110 1001 11kk kkkk
                        }),
                    new PushlOpRec(Opcode.PUSHL),                   // 1110 1010 kkkk kkkk
                    new SubDecoder(7, 1, new Decoder[2] {           // 1110 1011 ?... ....
                        new MovsfOpRec(Opcode.MOVSF),               // 1110 1011 0zzz zzzz + 1111 ffff ffff ffff
                        new MovssOpRec(Opcode.MOVSS),               // 1110 1011 1zzz zzzz + 1111 .... .zzz zzzz
                    }),
                    new UseBaseDecode(),                            // 1110 110s .... ....
                    new UseBaseDecode(),                            // 1110 110s .... ....
                    new LfsrOpRec(Opcode.LFSR),                     // 1110 1110 ffkk kkkk + 1111 0000 kkkk kkkk
                    new UseBaseDecode(),                            // 1110 1111 .... ....
                }),
            }),
            new NoOperandOpRec(Opcode.NOP),                         // 1111 .... .... ....
        };

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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte bsrval = (byte)uInstr.Extract(0, 8);
                if (bsrval >= 64)
                    return new PICInstruction(Opcode.invalid);
                var operd6 = new PIC18Immed6Operand(bsrval);
                return new PICInstruction(opcode, operd6) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return new PICInstruction(Opcode.invalid);
                ushort srcaddr = (ushort)((uInstr.Extract(0, 4) << 10) | word2.Extract(2, 10));
                ushort dstaddr = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18EnhancedRegisters.NotAllowedMovlDest(dstaddr))
                    return null;

                var opers = new PIC18Data14bitAbsAddrOperand(srcaddr);
                var operd = new PIC18Data14bitAbsAddrOperand(dstaddr);
                return new PICInstruction(opcode, opers, operd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 2);
                if (fsrnum >= 3)
                    return null;
                var fsrn = new PIC18FSROperand(fsrnum);
                var imm6 = new PIC18Immed6Operand((byte)uInstr.Extract(0, 6));
                return new PICInstruction(opcode, fsrn, imm6) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Only supported by PIC18 running in Extended Execution mode.
                    return new PICInstruction(Opcode.invalid);

                var operd = new PIC18Immed6Operand((byte)uInstr.Extract(0, 6));
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // PIC running in Extended Execution mode?
                    return new PICInstruction(Opcode.invalid);

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return new PICInstruction(Opcode.invalid);

                var zs = (byte)uInstr.Extract(0, 7);
                var fd = word2;

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18EnhancedRegisters.NotAllowedMovlDest(fd))
                    return new PICInstruction(Opcode.invalid);

                var operzs = new PIC18FSR2IdxOperand(zs);
                var operfd = new PIC18Data12bitAbsAddrOperand(fd);
                return new PICInstruction(opcode, operzs, operfd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstruction(Opcode.invalid);

                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return new PICInstruction(Opcode.invalid);
                byte zs = (byte)word2.Extract(2, 7);
                ushort fd = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PIC18EnhancedRegisters.NotAllowedMovlDest(fd))
                    return new PICInstruction(Opcode.invalid);

                var operzs = new PIC18FSR2IdxOperand(zs);
                var operfd = new PIC18Data14bitAbsAddrOperand(fd);
                return new PICInstruction(opcode, operzs, operfd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstruction(Opcode.invalid);

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return new PICInstruction(Opcode.invalid);

                var operzs = new PIC18FSR2IdxOperand((byte)uInstr.Extract(0, 7));
                var operzd = new PIC18FSR2IdxOperand((byte)word2.Extract(0, 7));

                return new PICInstruction(opcode, operzs, operzd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (dasm.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstruction(Opcode.invalid);

                var operd = new PIC18Immed8Operand((byte)uInstr.Extract(0, 8));
                return new PICInstruction(opcode, operd) { ExecMode = dasm.ExecMode };
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

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(4, 4);
                if (fsrnum >= 3)
                    return null;

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return new PICInstruction(Opcode.invalid);

                var fsrn = new PIC18FSROperand(fsrnum);
                if (word2 > 0x3FF) // Second word must be 'xxxx-00kk-kkkk-kkkk'
                    return new PICInstruction(Opcode.invalid);
                var imm14 = new PIC18Immed14Operand((ushort)((uInstr.Extract(0, 4) << 10) | word2));
                return new PICInstruction(opcode, fsrn, imm14) { ExecMode = dasm.ExecMode };
            }
        }

    }

}
