#region License
/* 
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

using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.Arc
{
    using Decoder = Decoder<ArcDisassembler, Mnemonic, ArcInstruction>;
    using NyiDecoder = NyiDecoder<ArcDisassembler, Mnemonic, ArcInstruction>;

    public class ArcDisassembler : DisassemblerBase<ArcInstruction>
    {
        private static readonly Decoder rootDecoder;

        private readonly ArcArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private List<MachineOperand> ops;

        public ArcDisassembler(ArcArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override ArcInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort hInstr))
                return null;
            this.ops.Clear();
            var instr = rootDecoder.Decode(hInstr, this);
            instr.Address = rdr.Address;
            return instr;
        }

        protected override ArcInstruction CreateInvalidInstruction()
        {
            return new ArcInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        #region Mutators
        #endregion

        #region Decoders
        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<ArcDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, Mutator<ArcDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override ArcInstruction Decode(uint wInstr, ArcDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var instr = new ArcInstruction
                {
                    InstructionClass = iclass,
                    Mnemonic = mnemonic,
                    Operands = dasm.ops.ToArray()
                };
                return instr;
            }
        }

        private class W32Decoder : Decoder
        {
            private readonly Decoder decoder;

            public W32Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override ArcInstruction Decode(uint hInstr, ArcDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort loword))
                    return dasm.CreateInvalidInstruction();
                var uInstr = ((uint) hInstr) << 16;
                uInstr |= loword;
                return decoder.Decode(uInstr, dasm);
            }
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<ArcDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<ArcDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        #endregion



        static ArcDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var ZOPs = Mask(8, 3, "  Zero operand Instructions",
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                Instr(Mnemonic.unimp_s, InstrClass.Invalid),
                invalid,
                invalid,

                Nyi("jeq_s [blink]"),
                Nyi("jne_s [blink]"),
                Nyi("j_s [blink]"),
                Nyi("j_s.d [blink]"));
            var SOPs = Mask(5, 3, "  Single operand, Jumps and Special Format Instructions",
                Nyi("J_S"),
                Nyi("J_S.D"),
                Nyi("JL_S"),
                Nyi("JL_S.D"),

                invalid,
                invalid,
                Nyi("SUB_S.NE"),
                ZOPs);

            rootDecoder = Mask(11, 5, "ARC instruction",
                new W32Decoder(Nyi("Bcc Branch  32-bit")),
                new W32Decoder(Nyi("BLcc, BRcc Branch and link conditional Compare-branch conditional 32-bit")),
                new W32Decoder(Nyi("LD register + offset Delayed load 32-bit")),
                new W32Decoder(Nyi("ST register + offset Buffered store 32-bit")),
                
                new W32Decoder(Nyi("op  a,b,c ARC 32-bit basecase instructions 32-bit")),
                new W32Decoder(Nyi("op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("op  a,b,c User 32-bit extension instructions 32-bit")),
                
                new W32Decoder(Nyi("op  a,b,c User 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("op  <market specific> ARC market-specific extension instructions 32-bit")),

                Nyi("LD_S / LDB_S / LDW_S / ADD_S   a,b,c Load/add register-register 16-bit"),
                Nyi("ADD_S / SUB_S / ASL_S /  LSR_S  c,b,u3 Add/sub/shift immediate 16-bit"),
                Nyi("MOV_S / CMP_S / ADD_S   b,h / b,b,h One dest/source can be any of r0-r63 16-bit"),
                Mask(0, 5, "  op_S b,b,c General ops/ single ops 16-bit",
                    SOPs,
                    Nyi("  1"),
                    Nyi("  2"),
                    Nyi("  3"),

                    Nyi("  4"),
                    Nyi("  5"),
                    Nyi("  6"),
                    Nyi("  7"),

                    Nyi("  8"),
                    Nyi("  9"),
                    Nyi("  A"),
                    Nyi("  B"),

                    Nyi("  C"),
                    Nyi("  D"),
                    Nyi("  E"),
                    Nyi("  F"),

                    Nyi(" 10"),
                    Nyi(" 11"),
                    Nyi(" 12"),
                    Nyi(" 13"),

                    Nyi(" 14"),
                    Nyi(" 15"),
                    Nyi(" 16"),
                    Nyi(" 17"),

                    Nyi(" 18"),
                    Nyi(" 19"),
                    Nyi(" 1A"),
                    Nyi(" 1B"),

                    Nyi(" 1C"),
                    Nyi(" 1D"),
                    Nyi(" 1E"),
                    Nyi(" 1F")),

                Nyi("LD_S c,[b,u7] Delayed load (32-bit aligned offset) 16-bit"),
                Nyi("LDB_S c,[b,u5] Delayed load (  8-bit aligned offset) 16-bit"),
                Nyi("LDW_S c,[b,u6] Delayed load (16-bit aligned offset) 16-bit"),
                Nyi("LDW_S.X c,[b,u6] Delayed load (16-bit aligned offset) 16-bit"),
                
                Nyi("ST_S c,[b,u7] Buffered store (32-bit aligned offset) 16-bit"),
                Nyi("STB_S c,[b,u5] Buffered store (  8-bit aligned offset) 16-bit"),
                Nyi("STW_S c,[b,u6] Buffered store (16-bit aligned offset) 16-bit"),
                Nyi("OP_S b,b,u5 Shift/subtract/bit ops 16-bit"),
                
                Nyi("LD_S / LDB_S / ST_S / STB_S / ADD_S / PUSH_S / POP_S Sp-based instructions 16-bit"),
                Nyi("LD_S / LDW_S / LDB_S / ADD_S  Gp-based ld/add (data aligned offset) 16-bit"),
                Nyi("LD_S b,[PCL,u10] Pcl-based ld (32-bit aligned offset) 16-bit"),
                Nyi("MOV_S b,u8 Move immediate 16-bit"),
                
                Nyi("ADD_S / CMP_S b,u7 Add/compare immediate 16-bit"),
                Nyi("BRcc_S b,0,s8 Branch conditionally on reg z/nz 16-bit"),
                Nyi("Bcc_S s10/s7 Branch conditionally 16-bit"),
                Nyi("BL_S s13 Branch and link unconditionally 16-bit"));
        }
    }
}