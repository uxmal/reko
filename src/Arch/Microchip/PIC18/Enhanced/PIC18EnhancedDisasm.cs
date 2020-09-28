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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;


namespace Reko.Arch.MicrochipPIC.PIC18
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
        /// Gets the opcodes table corresponding to this PIC family.
        /// </summary>
        protected override PICInstruction DecodePICInstruction(ushort uInstr, PICProgAddress addr)
        {
            var offset = rdr.Offset;
            try
            {
                var bits = uInstr.Extract(12, 4);
                instrCur = enhancedOpcodesTable[bits].Decode(uInstr, this);
                if (instrCur is null)
                    return base.DecodePICInstruction(uInstr, addr); // Fall to common PIC18 instruction decoder
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

        /// <summary>
        /// The PIC18 Enhanced opcodes decoder table.
        /// </summary>
        private static Decoder[] enhancedOpcodesTable = new Decoder[16]
        {
            new SubDecoder(8, 4, new Decoder[16] {                  // 0000 ???? .... ....
                new SubDecoder(4, 4, new Decoder[16] {              // 0000 0000 ???? ....
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0000 ??..
                        new SubDecoder(0, 2, new Decoder[4] {       // 0000 0000 0000 00??
                        new NoOperandDecoder(Mnemonic.NOP),             // 0000 0000 0000 0000
                        new InvalidDecoder(),                         // 0000 0000 0000 0001
                        new MovsflDecoder(Mnemonic.MOVSFL),             // 0000 0000 0000 0010
                        new UseBaseDecode(),                        // 0000 0000 0000 0011
                        }),
                        new UseBaseDecode(),                        // 0000 0000 0000 01..
                        new UseBaseDecode(),                        // 0000 0000 0000 10..
                        new UseBaseDecode(),                        // 0000 0000 0000 11..
                    }),
                    new SubDecoder(2, 2, new Decoder[4] {           // 0000 0000 0001 ??..
                        new UseBaseDecode(),                        // 0000 0000 0001 00..
                        new SubDecoder(0, 2, new Decoder[4] {       // 0000 0000 0001 01??
                            new NoOperandDecoder(Mnemonic.CALLW),       // 0000 0000 0001 0100
                            new InvalidDecoder(),                     // 0000 0000 0001 0101
                            new InvalidDecoder(),                     // 0000 0000 0001 0110
                            new InvalidDecoder(),                     // 0000 0000 0001 0111
                        }),
                        new InvalidDecoder(),                         // 0000 0000 0001 10..
                        new InvalidDecoder(),                         // 0000 0000 0001 11..
                    }),
                    new InvalidDecoder(),                             // 0000 0000 0010 ....
                    new InvalidDecoder(),                             // 0000 0000 0011 ....
                    new InvalidDecoder(),                             // 0000 0000 0100 ....
                    new InvalidDecoder(),                             // 0000 0000 0101 ....
                    new MovfflDecoder(Mnemonic.MOVFFL),                 // 0000 0000 0110 ffff + 1111 ffff ffff ffgg + 1111 gggg gggg gggg
                    new InvalidDecoder(),                             // 0000 0000 0111 ....
                    new InvalidDecoder(),                             // 0000 0000 1000 ....
                    new InvalidDecoder(),                             // 0000 0000 1001 ....
                    new InvalidDecoder(),                             // 0000 0000 1010 ....
                    new InvalidDecoder(),                             // 0000 0000 1011 ....
                    new InvalidDecoder(),                             // 0000 0000 1100 ....
                    new InvalidDecoder(),                             // 0000 0000 1101 ....
                    new InvalidDecoder(),                             // 0000 0000 1110 ....
                    new UseBaseDecode(),                            // 0000 0000 1111 1111 
                }),
                new MovlbImmDecoder(Mnemonic.MOVLB),                    // 0000 0001 .... ....
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
                            new FsrArithDecoder(Mnemonic.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrArithDecoder(Mnemonic.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrArithDecoder(Mnemonic.ADDFSR),       // 1110 1000 ffkk kkkk
                            new FsrULinkDecoder(Mnemonic.ADDULNK),      // 1110 1000 11kk kkkk
                    }),
                    new SubDecoder(6, 2, new Decoder[4] {           // 1110 1001 ??.. ....
                            new FsrArithDecoder(Mnemonic.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrArithDecoder(Mnemonic.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrArithDecoder(Mnemonic.SUBFSR),       // 1110 1001 ffkk kkkk
                            new FsrULinkDecoder(Mnemonic.SUBULNK),      // 1110 1001 11kk kkkk
                        }),
                    new PushlDecoder(Mnemonic.PUSHL),                   // 1110 1010 kkkk kkkk
                    new SubDecoder(7, 1, new Decoder[2] {           // 1110 1011 ?... ....
                        new MovsfDecoder(Mnemonic.MOVSF),               // 1110 1011 0zzz zzzz + 1111 ffff ffff ffff
                        new MovssDecoder(Mnemonic.MOVSS),               // 1110 1011 1zzz zzzz + 1111 .... .zzz zzzz
                    }),
                    new UseBaseDecode(),                            // 1110 110s .... ....
                    new UseBaseDecode(),                            // 1110 110s .... ....
                    new LfsrDecoder(Mnemonic.LFSR),                     // 1110 1110 ffkk kkkk + 1111 0000 kkkk kkkk
                    new UseBaseDecode(),                            // 1110 1111 .... ....
                }),
            }),
            new NoOperandDecoder(Mnemonic.NOP),                         // 1111 .... .... ....
        };

        /// <summary>
        /// Instruction MOVLB with <code>'....-....-0000-kkkk'</code> or <code>'....-....-00kk-kkkk'</code> immediate value.
        /// </summary>
        private class MovlbImmDecoder : Decoder
        {
            private Mnemonic opcode;

            public MovlbImmDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte bsrval = (byte)uInstr.Extract(0, 8);
                if (bsrval >= 64)
                    return new PICInstructionNoOpnd(Mnemonic.invalid);
                return new PICInstructionImmedByte(opcode, bsrval);
            }
        }

        /// <summary>
        /// Instruction MOVFFL decoder. (Enhanced PIC)
        /// </summary>
        private class MovfflDecoder : Decoder
        {
            private Mnemonic opcode;

            public MovfflDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);
                ushort srcaddr = (ushort)((uInstr.Extract(0, 4) << 10) | word2.Extract(2, 10));
                ushort dstaddr = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PICRegisters.NotAllowedDest(dstaddr))
                    return null;

                return new PICInstructionMem2Mem(opcode, srcaddr, dstaddr);
            }
        }

        /// <summary>
        /// Instructions ADDFSR, SUBFSR (PIC18 extended or later).
        /// </summary>
        private class FsrArithDecoder : Decoder
        {
            private Mnemonic opcode;

            public FsrArithDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                byte fsrnum = (byte)uInstr.Extract(6, 2);
                if (fsrnum >= 3)
                    return null;
                var imm6 = (byte)uInstr.Extract(0, 6);
                return new PICInstructionFSRUArith(opcode, fsrnum, imm6);
            }
        }

        /// <summary>
        /// Instructions ADDULNK, SUBULNK (PIC18 extended or later, extended execution mode).
        /// </summary>
        private class FsrULinkDecoder : Decoder
        {
            private Mnemonic opcode;

            public FsrULinkDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (PICMemoryDescriptor.ExecMode != PICExecMode.Extended) // Only supported by PIC18 running in Extended Execution mode.
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var imm6 = uInstr.Extract(0, 6);
                return new PICInstructionImmedByte(opcode, imm6);
            }
        }

        /// <summary>
        /// Instruction MOVSF decoder. (Extended mode)
        /// </summary>
        private class MovsfDecoder : Decoder
        {
            private Mnemonic opcode;

            public MovsfDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (PICMemoryDescriptor.ExecMode != PICExecMode.Extended) // PIC running in Extended Execution mode?
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort fd))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PICRegisters.NotAllowedDest(fd))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var operzs = (byte)uInstr.Extract(0, 7);
                return new PICInstructionMem2Mem(opcode, operzs, fd);
            }
        }

        /// <summary>
        /// Instruction MOVSFL decoder. (Extended mode, Enhanced PIC)
        /// </summary>
        private class MovsflDecoder : Decoder
        {
            private Mnemonic opcode;

            public MovsflDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (PICMemoryDescriptor.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                // This is a 3-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2) || !GetAddlInstrWord(dasm.rdr, out ushort word3))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                ushort fd = (ushort)(word3.Extract(0, 12) | (word2.Extract(0, 2) << 12));

                // PCL, TOSL, TOSH, TOSU are invalid destinations.
                if (PICRegisters.NotAllowedDest(fd))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var operzs = (byte)word2.Extract(2, 7);
                return new PICInstructionMem2Mem(opcode, operzs, fd);
            }
        }

        /// <summary>
        /// Instruction MOVSS decoder. (Extended mode)
        /// </summary>
        private class MovssDecoder : Decoder
        {
            private Mnemonic opcode;

            public MovssDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (PICMemoryDescriptor.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                // This is a 2-word instruction.
                if (!GetAddlInstrWord(dasm.rdr, out ushort word2))
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var operzs = (byte)uInstr.Extract(0, 7);
                var operzd = (byte)word2.Extract(0, 7);

                return new PICInstructionMem2Mem(opcode, operzs, operzd);
            }
        }

        /// <summary>
        /// Instruction PUSHL decoder. (Extended mode)
        /// </summary>
        private class PushlDecoder : Decoder
        {
            private Mnemonic opcode;

            public PushlDecoder(Mnemonic opc)
            {
                opcode = opc;
            }

            public override PICInstruction Decode(ushort uInstr, PICDisassemblerBase dasm)
            {
                if (PICMemoryDescriptor.ExecMode != PICExecMode.Extended) // Is PIC running in Extended Execution mode...
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var imm8 = (byte)uInstr.Extract(0, 8);
                return new PICInstructionImmedByte(opcode, imm8);
            }
        }

        /// <summary>
        /// Instruction LFSR decoder.
        /// </summary>
        private class LfsrDecoder : Decoder
        {
            private Mnemonic opcode;

            public LfsrDecoder(Mnemonic opc)
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
                    return new PICInstructionNoOpnd(Mnemonic.invalid);
                if (word2 > 0x3FF) // Second word must be 'xxxx-00kk-kkkk-kkkk'
                    return new PICInstructionNoOpnd(Mnemonic.invalid);

                var imm14 = ((ushort)((uInstr.Extract(0, 4) << 10) | word2));
                return new PICInstructionLFSRLoad(opcode, fsrnum, imm14);
            }
        }

    }

}
