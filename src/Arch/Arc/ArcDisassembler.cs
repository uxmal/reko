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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Arc
{
    using Decoder = Decoder<ArcDisassembler, Mnemonic, ArcInstruction>;
    using NyiDecoder = NyiDecoder<ArcDisassembler, Mnemonic, ArcInstruction>;

    public class ArcDisassembler : DisassemblerBase<ArcInstruction>
    {
        private static readonly Decoder rootDecoder;

        private readonly ARCompactArchitecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private ArcInstruction instr;   // Instruction being decoded.
        private List<MachineOperand> ops;

        public ArcDisassembler(ARCompactArchitecture arch, EndianImageReader rdr)
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
            this.instr = new ArcInstruction();
            var instr = rootDecoder.Decode(hInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        protected override ArcInstruction CreateInvalidInstruction()
        {
            return new ArcInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = new MachineOperand[0]
            };
        }

        public override ArcInstruction NotYetImplemented(uint wInstr, string message)
        {
            var len = rdr.Address - addr;
            var hex = (len == 4)
                ? $"{wInstr:X8}"
                : $"{wInstr:X4}";
            EmitUnitTest("ARCompact", hex, message, "ARCompactDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"{0}\");", hex);
            });
            return CreateInvalidInstruction();
        }

        #region Mutators

        // Register encoding
        private static Mutator<ArcDisassembler> R(int bitpos, int length)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static Mutator<ArcDisassembler> R(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                var reg = Registers.GpRegs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> A = R(0, 6);
        private static readonly Mutator<ArcDisassembler> B = R(Bf((12, 3),(24, 3)));
        private static readonly Mutator<ArcDisassembler> C = R(6, 6);
        private static readonly Mutator<ArcDisassembler> h = R(Bf((0, 3), (5, 3)));

        private static Mutator<ArcDisassembler> R_or_limm(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                if (iReg == 62)
                {
                    if (!d.rdr.TryReadUInt32(out uint limm))
                        return false;
                    d.ops.Add(ImmediateOperand.Word32(limm));
                }
                else
                {
                    var reg = Registers.GpRegs[iReg];
                    d.ops.Add(new RegisterOperand(reg));
                }
                return true;

            };
        }
        private static readonly Mutator<ArcDisassembler> h_limm = R_or_limm(Bf((0, 3), (5, 3)));


        // Compact register encoding
        private static readonly RegisterStorage[] bRegs = new[]
        {
            Registers.GpRegs[0],
            Registers.GpRegs[1],
            Registers.GpRegs[2],
            Registers.GpRegs[3],

            Registers.GpRegs[12],
            Registers.GpRegs[13],
            Registers.GpRegs[14],
            Registers.GpRegs[15],
        };
        private static Mutator<ArcDisassembler> BReg(int bitpos, int length)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var reg = bRegs[field.Read(u)];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> b = BReg(8, 3);
        private static readonly Mutator<ArcDisassembler> c = BReg(5, 3);

        // Specific register
        private static Mutator<ArcDisassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static readonly Mutator<ArcDisassembler> sp = Reg(Registers.Sp);
        private static readonly Mutator<ArcDisassembler> blink = Reg(Registers.Blink);

        // Signed immediate
        private static Mutator<ArcDisassembler> I(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(ImmediateOperand.Int32((int) imm));
                return true;
            };
        }

        // Unsigned immediate with shift
        private static Mutator<ArcDisassembler> U(int bitPos, int length, int shift)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                //$REVIEW: could be made more efficient by avoiding the extra shift.
                var imm = field.Read(u) << shift;
                d.ops.Add(ImmediateOperand.Word32((int)imm));
                return true;
            };
        }

        // Memory access with offset
        private static Mutator<ArcDisassembler> Mo(
            PrimitiveType dt,
            Bitfield[] baseRegFields,
            Bitfield[] offsetFields)
        {
            return (u, d) =>
            {
                var iBaseReg = Bitfield.ReadFields(baseRegFields, u);
                var offset = Bitfield.ReadSignedFields(offsetFields, u);
                var baseReg = Registers.GpRegs[iBaseReg];
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        // Compact memory access with offset
        private static Mutator<ArcDisassembler> Mo_s(PrimitiveType dt)
        {
            var baseRegField = new Bitfield(8, 3);
            var offsetField = new Bitfield(0, 5);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var offset = offsetField.Read(u) * dt.Size;
                var baseReg = bRegs[iBaseReg];
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = (int)offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static bool Mblink(uint uInstr, ArcDisassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(PrimitiveType.Word32)
            {
                Base = Registers.Blink,
            });
            return true;
        }
        private static Mutator<ArcDisassembler> AddressWriteBack(int bitPos)
        {
            return (u, d) =>
            {
                d.instr.Writeback = (AddressWritebackMode) ((u >> bitPos) & 3);
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> AA3 = AddressWriteBack(3);
        private static readonly Mutator<ArcDisassembler> AA9 = AddressWriteBack(9);

        private static Mutator<ArcDisassembler> Direct(int bitPos)
        {
            return (u, d) =>
            {
                d.instr.DirectWrite = Bits.IsBitSet(u, bitPos);
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> Di5 = Direct(5);
        private static readonly Mutator<ArcDisassembler> Di11 = Direct(11);

        private static bool X(uint uInstr, ArcDisassembler dasm)
        {
            dasm.instr.SignExtend = Bits.IsBitSet(uInstr, 6);
            return true;
        }


        private static Mutator<ArcDisassembler> PcRel(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << 2;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var addr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32((uint)addr));
                return true;
            };
        }

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
                dasm.instr.InstructionClass = iclass;
                dasm.instr.Mnemonic = mnemonic;
                dasm.instr.Operands = dasm.ops.ToArray();
                return dasm.instr;
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
                Instr(Mnemonic.j_s, InstrClass.Transfer, Mblink),   // delay slot doesn't execute.
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

            var SpBasedInstructions = Mask(5, 3, "  Sp-based instructions 16-bit",
                Nyi("LD_S"),
                Nyi("LDB_S"),
                Nyi("ST_S"),
                Nyi("STB_S"),
                Nyi("ADD_S"),
                Sparse(8, 3, "  add_s/sub_s", invalid,
                    (0x0, Instr(Mnemonic.add_s, sp,sp,U(0,5,2))),
                    (0x1, Instr(Mnemonic.sub_s, sp,sp,U(0,5,2)))),
                Sparse(0, 5, "  pop_s", invalid,
                    (0x01, Instr(Mnemonic.pop_s, b)),
                    (0x11, Instr(Mnemonic.pop_s, blink))),
                Sparse(0, 5, "  push_s", invalid,
                    (0x01, Instr(Mnemonic.push_s, b)),
                    (0x11, Instr(Mnemonic.push_s, blink))));

            rootDecoder = Mask(11, 5, "ARC instruction",
                new W32Decoder(Mask(16, 1, "Bcc Branch  32-bit",
                    Nyi("Branch Conditionally"),
                    Nyi("Branch Unconditional Far"))),
                new W32Decoder(Mask(16,1, "BLcc, BRcc Branch and link conditional Compare-branch conditional 32-bit",
                    Mask(17, 1,  "    BLcc 0?",
                        Nyi("    BLcc 00"),
                        Instr(Mnemonic.bl, PcRel(Bf((0,4),(6,10),(18,9))))),
                    Mask(4, 1, "    BLcc 1?",
                        Nyi("    BLcc 10"),
                        Nyi("    BLcc 11")))),
                new W32Decoder(Mask(7, 2, "LD register + offset Delayed load 32-bit",
                    Mask(6, 1, "  X - sign extension",
                        Instr(Mnemonic.ld, A, AA9, Di11, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                        invalid),
                    Instr(Mnemonic.ldb, A, AA9, Di11, X, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                    Instr(Mnemonic.ldw, A, AA9, Di11, X, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                    invalid)),
                new W32Decoder(Mask(1, 2, "ST register + offset Buffered store 32-bit",
                    Instr(Mnemonic.st, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                    Instr(Mnemonic.stb, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                    Instr(Mnemonic.stw, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12,3),(24,3)),Bf((15,1),(16,7)))),
                    invalid)),
                new W32Decoder(Mask(22, 2, "04 op  a,b,c ARC 32-bit basecase instructions 32-bit",
                    Sparse(16, 6, "REG_REG", Nyi("REG_REG"),
                        (0x0A, Instr(Mnemonic.mov, B,C))),
                    Nyi("REG_U6IMM"),
                    Sparse(16, 6, "REG_S12IMM", Nyi("REG_S12IMM"),
                        (0x06, Instr(Mnemonic.bic, B,B,I(Bf((0,6),(6,6)))))
                        ),
                    Nyi("COND_REG"))),
                new W32Decoder(Nyi("05 op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("06 op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("07 op  a,b,c User 32-bit extension instructions 32-bit")),
                
                new W32Decoder(Nyi("08 op  a,b,c User 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("09 op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("0A op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("0B op  <market specific> ARC market-specific extension instructions 32-bit")),

                Nyi("LD_S / LDB_S / LDW_S / ADD_S   a,b,c Load/add register-register 16-bit"),
                Nyi("ADD_S / SUB_S / ASL_S /  LSR_S  c,b,u3 Add/sub/shift immediate 16-bit"),
                Mask(3, 2, "MOV_S / CMP_S / ADD_S   b,h / b,b,h One dest/source can be any of r0-r63 16-bit",
                    Instr(Mnemonic.add_s, b, b, h_limm),
                    Instr(Mnemonic.mov_s, b, h_limm),
                    Instr(Mnemonic.mov_s, b, h_limm),
                    Instr(Mnemonic.mov_s, h, b)),

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
                Instr(Mnemonic.ldw_s, c, Mo_s(PrimitiveType.Word16)),
                Nyi("LDW_S.X c,[b,u6] Delayed load (16-bit aligned offset) 16-bit"),
                
                Instr(Mnemonic.st_s, c,b,Mo_s(PrimitiveType.Word32)),
                Nyi("STB_S c,[b,u5] Buffered store (  8-bit aligned offset) 16-bit"),
                Instr(Mnemonic.stw_s, c, Mo_s(PrimitiveType.Word16)),
                Nyi("OP_S b,b,u5 Shift/subtract/bit ops 16-bit"),
                
                SpBasedInstructions,
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