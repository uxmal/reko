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
        private const InstrClass T = InstrClass.Transfer;
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass CT = InstrClass.ConditionalTransfer;
        private const InstrClass CTD = InstrClass.ConditionalTransfer | InstrClass.Delay;
        private const int LongImmediateDataIndicator = 62;

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


        private bool TryReadLongImmediate(out uint imm)
        {
            if (!rdr.TryReadUInt16(out ushort immHi))
            {
                imm = 0;
                return false;
            }
            if (!rdr.TryReadUInt16(out ushort immLo))
            {
                imm = 0;
                return false;
            }
            imm = immLo | ((uint) immHi << 16);
            return true;
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
                if (iReg ==  ArcDisassembler.LongImmediateDataIndicator)
                {
                    if (!d.TryReadLongImmediate(out uint limm))
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
        private static readonly Mutator<ArcDisassembler> C_limm = R_or_limm(Bf((6, 6)));


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

        // Unsigned immediate
        private static Mutator<ArcDisassembler> U(int bitPos, int length)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = field.Read(u);
                d.ops.Add(ImmediateOperand.Word32((int) imm));
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

        // Literal number
        private static Mutator<ArcDisassembler> n(int n)
        {
            return (u, d) =>
            {
                var imm = ImmediateOperand.Int32(n);
                d.ops.Add(imm);
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

        // Memory access with index register
        private static Mutator<ArcDisassembler> Mrr(
            PrimitiveType dt,
            Bitfield[] baseRegFields,
            Bitfield[] indexFields)
        {
            return (u, d) =>
            {
                var iBaseReg = Bitfield.ReadFields(baseRegFields, u);
                if (iBaseReg == LongImmediateDataIndicator)
                    return false;
                var baseReg = Registers.GpRegs[iBaseReg];
                var iIndexReg = Bitfield.ReadSignedFields(indexFields, u);
                MemoryOperand mem;
                if (iIndexReg == LongImmediateDataIndicator)
                {
                    if (!d.TryReadLongImmediate(out uint imm))
                        return false;
                    mem = new MemoryOperand(dt)
                    {
                        Base = baseReg,
                        Offset = (int) iIndexReg,
                    };
                }
                else
                {
                    var indexReg = Registers.GpRegs[iIndexReg];
                    mem = new MemoryOperand(dt)
                    {
                        Base = baseReg,
                        Index = indexReg,
                    };
                }
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

        // Indirect access
        private static Mutator<ArcDisassembler> Mr(PrimitiveType dt, int bitpos, int bitlen)
        {
            var baseRegField = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var mem = new MemoryOperand(dt);
                if (iBaseReg == LongImmediateDataIndicator)
                {
                    if (!d.TryReadLongImmediate(out uint offset))
                        return false;
                    mem.Offset = (int)offset;
                }
                else
                {
                    mem.Base = bRegs[iBaseReg];
                }
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> Mr_C = Mr(PrimitiveType.Word32, 6, 6);

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
        private static readonly Mutator<ArcDisassembler> AA22 = AddressWriteBack(22);

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
        private static readonly Mutator<ArcDisassembler> Di15 = Direct(11);

        private static bool X(uint uInstr, ArcDisassembler dasm)
        {
            dasm.instr.SignExtend = Bits.IsBitSet(uInstr, 6);
            return true;
        }

        private static bool N5(uint uInstr, ArcDisassembler dasm)
        {
            dasm.instr.Delay = Bits.IsBitSet(uInstr, 5);
            return true;
        }

        private static Mutator<ArcDisassembler> PcRel2(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << 1;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var addr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32((uint)addr));
                return true;
            };
        }

        private static Mutator<ArcDisassembler> PcRel4(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << 2;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var addr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32((uint) addr));
                return true;
            };
        }

        private static Mutator<ArcDisassembler> PcRel_s(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << 1;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var addr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32((uint) addr));
                return true;
            };
        }

        private static readonly Bitfield genopFormatField = new Bitfield(22, 2);
        private static readonly Bitfield genopUimm6 = new Bitfield(6, 6);
        private static bool GeneralOp(uint uInstr, ArcDisassembler dasm)
        {
            var format = genopFormatField.Read(uInstr);
            switch (format)
            {
            case 0: // REG_REG
                if (!B(uInstr, dasm))
                    return false;
                if (!C(uInstr, dasm))
                    return false;
                return true;
            case 1: // REG_U6IMM
                if (!B(uInstr, dasm))
                    return false;
                var uimm6 = genopUimm6.Read(uInstr);
                dasm.ops.Add(ImmediateOperand.Word32(uimm6));
                return true;
            case 2: // REG_S12IMM
                Nyi("General op, REG_S12IMM");
                return false;
            default: // COND_REG
                if (Bits.IsBitSet(uInstr, 5))
                {
                    Nyi("General op, COND_REG_UIMM6");
                    return false;
                }
                else
                {
                    Nyi("General op, COND_REG");
                    return false;
                }
            }
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

            var majorOpc_00 = new W32Decoder(Mask(16, 1, "  00 Bcc Branch  32-bit",
                Mask(5, 1, "  Branch Conditionally",
                    Mask(0, 5, "  N=0",
                        Instr(Mnemonic.b, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.beq, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bne, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bpl, CT, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bmi, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bcs, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bcc, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bvs, CT, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bvc, T, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bgt, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bge, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.blt, CT, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.ble, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bhi, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bls, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bpnz, CT, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bss, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bsc, CT, PcRel2(Bf((6, 10), (17, 10)))),
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(0, 5, "  N=1",
                        Instr(Mnemonic.b, TD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.beq, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bne, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bpl, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bmi, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bcs, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bcc, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bvs, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bvc, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bgt, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bge, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.blt, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.ble, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bhi, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bls, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bpnz, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),

                        Instr(Mnemonic.bss, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        Instr(Mnemonic.bsc, CTD, N5, PcRel2(Bf((6, 10), (17, 10)))),
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid)),
                Mask(5, 1, "  Branch far unconditionally",
                    Instr(Mnemonic.b, T, N5, PcRel2(Bf((0, 4), (6, 10), (17, 10)))),
                    Instr(Mnemonic.b, TD, N5, PcRel2(Bf((0, 4), (6, 10), (17, 10)))))));

            var majorOpc_04 = new W32Decoder(Mask(22, 2, "  04 op  a,b,c ARC 32-bit basecase instructions 32-bit",
                Sparse(16, 6, "REG_REG", Nyi("REG_REG"),
                    (0x00, Instr(Mnemonic.add, A, B, C)),
                    (0x02, Instr(Mnemonic.sub, A, B, C)),
                    (0x06, Instr(Mnemonic.bic, A, B, C)),
                    (0x0A, Instr(Mnemonic.mov, B, C)),
                    (0x2A, Instr(Mnemonic.lr, B, Mr_C)),
                    (0x2B, Instr(Mnemonic.sr, B, Mr_C))),
                Sparse(16, 6, "REG_U6IMM", Nyi("REG_U6IMM"),
                    (0x04, Instr(Mnemonic.and, A, GeneralOp)),
                    (0x13, Instr(Mnemonic.bmsk, A, B, U(6,6))),
                    (0x30, Instr(Mnemonic.ld, A, AA22, Di15, Mrr(PrimitiveType.Word32,Bf((12,3),(24,3)), Bf((6,6)))))),
                Sparse(16, 6, "REG_S12IMM", Nyi("REG_S12IMM"),
                    (0x06, Instr(Mnemonic.bic, B, B, I(Bf((0, 6), (6, 6)))))
                    ),
                Sparse(16, 6, "COND_REG", Nyi("COND_REG"))));

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
                majorOpc_00,
                new W32Decoder(Mask(16,1, "BLcc, BRcc Branch and link conditional Compare-branch conditional 32-bit",
                    Mask(17, 1,  "    BLcc 0?",
                        Nyi("    BLcc 00"),
                        Mask(5, 1, "  Branch and link unconditionally",
                            Instr(Mnemonic.bl, T |InstrClass.Call, PcRel4(Bf((0,4),(6,10),(18,9)))),
                            Instr(Mnemonic.bl, TD|InstrClass.Call, PcRel4(Bf((0,4),(6,10),(18,9)))))),
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
                majorOpc_04,
                new W32Decoder(Nyi("  05 op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("  06 op  a,b,c ARC 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("  07 op  a,b,c User 32-bit extension instructions 32-bit")),
                
                new W32Decoder(Nyi("  08 op  a,b,c User 32-bit extension instructions 32-bit")),
                new W32Decoder(Nyi("  09 op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("  0A op  <market specific> ARC market-specific extension instructions 32-bit")),
                new W32Decoder(Nyi("  0B op  <market specific> ARC market-specific extension instructions 32-bit")),

                Nyi("LD_S / LDB_S / LDW_S / ADD_S   a,b,c Load/add register-register 16-bit"),
                Mask(3, 2, "  0D ADD_S / SUB_S / ASL_S /  LSR_S  c,b,u3 Add/sub/shift immediate 16-bit",
                    Instr(Mnemonic.add_s, c, b, U(0, 3)),
                    Instr(Mnemonic.sub_s, c, b, U(0, 3)),
                    Instr(Mnemonic.asl_s, c, b, U(0, 3)),
                    Instr(Mnemonic.asr_s, c, b, U(0, 3))),
                Mask(3, 2, "  0E MOV_S / CMP_S / ADD_S   b,h / b,b,h One dest/source can be any of r0-r63 16-bit",
                    Instr(Mnemonic.add_s, b, b, h_limm),
                    Instr(Mnemonic.mov_s, b, h_limm),
                    Instr(Mnemonic.mov_s, b, h_limm),
                    Instr(Mnemonic.mov_s, h, b)),

                Mask(0, 5, "  0F op_S b,b,c General ops/ single ops 16-bit",
                    SOPs,
                    invalid,
                    Instr(Mnemonic.sub_s, b,b, c),
                    invalid,

                    Instr(Mnemonic.and_s, b,b, c),
                    Instr(Mnemonic.or_s, b, b, c),
                    Instr(Mnemonic.bic_s, b, b, c),
                    Instr(Mnemonic.xor_s, b, b, c),

                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.tst_s, b, c),

                    Instr(Mnemonic.mul64_s, b, c),
                    Instr(Mnemonic.sexb_s, b,c),
                    Instr(Mnemonic.sexw_s, b,c),
                    Instr(Mnemonic.extb_s, b,c),

                    Instr(Mnemonic.extw_s, b,c),
                    Instr(Mnemonic.abs_s, b,c),
                    Instr(Mnemonic.not_s, b,c),
                    Instr(Mnemonic.neg_s, b,c),

                    Instr(Mnemonic.add1_s, b,b,c),
                    Instr(Mnemonic.add2_s, b,b,c),
                    Instr(Mnemonic.add3_s, b,b,c),
                    invalid,

                    Instr(Mnemonic.asl_s, b,b,c),
                    Instr(Mnemonic.lsr_s, b, b, c),
                    Instr(Mnemonic.asr_s, b,c,n(1)),
                    Instr(Mnemonic.asl_s, b,c,n(1)),

                    Instr(Mnemonic.asr_s, b,c,n(1)),
                    Instr(Mnemonic.lsr_s, b,c,n(1)),
                    Instr(Mnemonic.trap_s),
                    Instr(Mnemonic.brk_s)),

                // 0x10
                Instr(Mnemonic.ld_s, c, Mo_s(PrimitiveType.Word16)),
                Instr(Mnemonic.ldb_s, c, Mo_s(PrimitiveType.Word16)),
                Instr(Mnemonic.ldw_s, c, Mo_s(PrimitiveType.Word16)),
                Nyi("LDW_S.X c,[b,u6] Delayed load (16-bit aligned offset) 16-bit"),
                
                Instr(Mnemonic.st_s, c,b,Mo_s(PrimitiveType.Word32)),
                Instr(Mnemonic.stb_s, c,b,Mo_s(PrimitiveType.Word32)),
                Instr(Mnemonic.stw_s, c, Mo_s(PrimitiveType.Word16)),
                Mask(5, 3, "  17 OP_S b,b,u5 Shift/subtract/bit ops 16-bit",
                    Instr(Mnemonic.asl_s, b, b, U(0, 5)),
                    Instr(Mnemonic.lsr_s, b, b, U(0, 5)),
                    Instr(Mnemonic.asr_s, b, b, U(0, 5)),
                    Instr(Mnemonic.sub_s, b, b, U(0, 5)),

                    Instr(Mnemonic.bset_s, b, b, U(0, 5)),
                    Instr(Mnemonic.bclr_s, b, b, U(0, 5)),
                    Instr(Mnemonic.bmsk_s, b, b, U(0, 5)),
                    Instr(Mnemonic.btst_s, b, U(0, 5))),
                // 0x18
                SpBasedInstructions,
                Nyi("LD_S / LDW_S / LDB_S / ADD_S  Gp-based ld/add (data aligned offset) 16-bit"),
                Nyi("LD_S b,[PCL,u10] Pcl-based ld (32-bit aligned offset) 16-bit"),
                Instr(Mnemonic.mov_s, b, U(0,8)),
                
                Mask(7, 1, "  ADD_S / CMP_S b,u7 Add/compare immediate 16-bit",
                    Instr(Mnemonic.add_s, b,b,U(0,7)),
                    Instr(Mnemonic.cmp_s, b,b,U(0,7))),
                Nyi("BRcc_S b,0,s8 Branch conditionally on reg z/nz 16-bit"),
                Mask(9, 2, "  Bcc_S s10/s7 Branch conditionally 16-bit",
                    Instr(Mnemonic.b_s, T, PcRel_s(Bf((0,9)))),
                    Instr(Mnemonic.beq_s, CT, PcRel_s(Bf((0,9)))),
                    Instr(Mnemonic.bne_s, CT, PcRel_s(Bf((0,9)))),
                    Mask(6, 3, "  Bcc_S",
                        Instr(Mnemonic.bgt_s, CT, PcRel_s(Bf((0,6)))),
                        Instr(Mnemonic.bge_s, CT, PcRel_s(Bf((0, 6)))),
                        Instr(Mnemonic.blt_s, CT, PcRel_s(Bf((0, 6)))),
                        Instr(Mnemonic.ble_s, CT, PcRel_s(Bf((0, 6)))),

                        Instr(Mnemonic.bhi_s, CT, PcRel_s(Bf((0, 6)))),
                        Instr(Mnemonic.bhs_s, CT, PcRel_s(Bf((0, 6)))),
                        Instr(Mnemonic.blo_s, CT, PcRel_s(Bf((0, 6)))),
                        Instr(Mnemonic.bls_s, CT, PcRel_s(Bf((0, 6)))))),
                Nyi("BL_S s13 Branch and link unconditionally 16-bit"));
        }
    }
}