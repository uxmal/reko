#region License
/* 
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Arc
{
    using Decoder = Decoder<ArcDisassembler, Mnemonic, ArcInstruction>;
    using NyiDecoder = NyiDecoder<ArcDisassembler, Mnemonic, ArcInstruction>;

    public class ArcDisassembler : DisassemblerBase<ArcInstruction, Mnemonic>
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
        private bool readLimm;

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
            this.readLimm = false;
            this.instr = new ArcInstruction();
            var instr = rootDecoder.Decode(hInstr, this);
            if (hInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override ArcInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            this.instr.InstructionClass = iclass;
            this.instr.Mnemonic = mnemonic;
            this.instr.Operands = this.ops.ToArray();
            return this.instr;
        }

        public override ArcInstruction CreateInvalidInstruction()
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
            imm = 0;
            if (this.readLimm)
                return false;
            if (!rdr.TryReadUInt16(out ushort immHi))
                return false;
            if (!rdr.TryReadUInt16(out ushort immLo))
                return false;
            this.readLimm = true;
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
                var reg = Registers.CoreRegisters[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static Mutator<ArcDisassembler> R(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                var reg = Registers.CoreRegisters[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> A = R(0, 6);
        private static readonly Mutator<ArcDisassembler> B = R(Bf((12, 3), (24, 3)));
        private static readonly Mutator<ArcDisassembler> C = R(6, 6);
        private static readonly Mutator<ArcDisassembler> h = R(Bf((0, 3), (5, 3)));

        private static Mutator<ArcDisassembler> R_or_limm(Bitfield[] bitfields)
        {
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(bitfields, u);
                if (iReg == ArcDisassembler.LongImmediateDataIndicator)
                {
                    if (!d.TryReadLongImmediate(out uint limm))
                        return false;
                    d.ops.Add(ImmediateOperand.Word32(limm));
                }
                else
                {
                    var reg = Registers.CoreRegisters[iReg];
                    d.ops.Add(new RegisterOperand(reg));
                }
                return true;
            };
        }

        private static readonly Mutator<ArcDisassembler> h_limm = R_or_limm(Bf((0, 3), (5, 3)));
        private static readonly Mutator<ArcDisassembler> B_limm = R_or_limm(Bf((12, 3), (24, 3)));
        private static readonly Mutator<ArcDisassembler> C_limm = R_or_limm(Bf((6, 6)));


        // Compact register encoding
        private static readonly RegisterStorage[] bRegs = new[]
        {
            Registers.CoreRegisters[0],
            Registers.CoreRegisters[1],
            Registers.CoreRegisters[2],
            Registers.CoreRegisters[3],

            Registers.CoreRegisters[12],
            Registers.CoreRegisters[13],
            Registers.CoreRegisters[14],
            Registers.CoreRegisters[15],
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
        private static readonly Mutator<ArcDisassembler> a = BReg(0, 3);
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

        private static readonly Mutator<ArcDisassembler> r0 = Reg(Registers.CoreRegisters[0]);
        private static readonly Mutator<ArcDisassembler> sp = Reg(Registers.Sp);
        private static readonly Mutator<ArcDisassembler> blink = Reg(Registers.Blink);

        // Signed immediate
        private static Mutator<ArcDisassembler> I(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(ImmediateOperand.Int32(imm));
                return true;
            };
        }

        // Signed immediate with shift
        private static Mutator<ArcDisassembler> Is(int bitpos, int bitlen, int shift)
        {
            var field = new Bitfield(bitpos, bitlen);
            return (u, d) =>
            {
                var imm = field.ReadSigned(u) << shift;
                d.ops.Add(ImmediateOperand.Int32(imm));
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
                d.ops.Add(ImmediateOperand.Word32((int) imm));
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
                var baseReg = Registers.CoreRegisters[iBaseReg];
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
                var baseReg = Registers.CoreRegisters[iBaseReg];
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
                    var indexReg = Registers.CoreRegisters[iIndexReg];
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
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        // Compact memory access with given register, offset
        private static Mutator<ArcDisassembler> Mo_reg_s(PrimitiveType dt, RegisterStorage baseReg, int offsetSize)
        {
            var offsetField = new Bitfield(0, offsetSize);
            return (u, d) =>
            {
                var offset = offsetField.Read(u) * dt.Size;
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        // Compact stack memory access with offset.
        private static Mutator<ArcDisassembler> Mo_sp_s(PrimitiveType dt)
        {
            var offsetField = new Bitfield(0, 5);
            return (u, d) =>
            {
                // Manual: "offsets are always 32-bit aligned."
                var offset = offsetField.Read(u) * 4;
                var mem = new MemoryOperand(dt)
                {
                    Base = Registers.Sp,
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        // Indirect register access
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
                    mem.Offset = (int) offset;
                }
                else
                {
                    mem.Base = Registers.CoreRegisters[iBaseReg];
                }
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> Mr_C = Mr(PrimitiveType.Word32, 6, 6);

        private static readonly Bitfield asField = new Bitfield(0, 3);
        private static readonly Bitfield bsField = new Bitfield(8, 3);
        private static readonly Bitfield csField = new Bitfield(5, 3);

        // Compact Indirect register access
        private static Mutator<ArcDisassembler> Mr_s(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var breg = bRegs[bsField.Read(u)];
                var mem = new MemoryOperand(dt)
                {
                    Base = breg
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<ArcDisassembler> Midx_s(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var breg = bRegs[bsField.Read(u)];
                var creg = bRegs[csField.Read(u)];
                d.ops.Add(new MemoryOperand(dt)
                {
                    Base = breg,
                    Index = creg,
                });
                return true;
            };
        }

        // External register access
        private static Mutator<ArcDisassembler> Mext(PrimitiveType dt, int bitpos, int bitlen)
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
                    mem.Offset = (int) iBaseReg;
                    //mem.Base = bRegs[iBaseReg];
                }
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<ArcDisassembler> Mext_C = Mext(PrimitiveType.Word32, 6, 6);

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

        private static Mutator<ArcDisassembler> N(bool delay)
        {
            return (u, d) =>
            {
                d.instr.Delay = true;
                return true;
            };
        }

        private static bool F15(uint uInstr, ArcDisassembler dasm)
        {
            dasm.instr.SetFlags = Bits.IsBitSet(uInstr, 15);
            return true;
        }

        private static Mutator<ArcDisassembler> PcRel2(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << 1;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var uAddr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32((uint)uAddr));
                return true;
            };
        }

        // PC relative unsigned offset (always forward jump)
        private static Mutator<ArcDisassembler> PcRelU2(int bitpos, int bitlength)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var offset = field.Read(u) << 1;
                // According to manual, PC-relative calculations 
                // always mask the low 2 bits of program counter....
                var uAddr = (d.addr.ToUInt32() & ~3u) + offset;
                d.ops.Add(AddressOperand.Ptr32(uAddr));
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
        private static readonly Bitfield genopCondField = new Bitfield(0, 5);
        private static readonly Bitfield[] genopSimm12 = Bf((0, 6), (6, 6));

        private static Mutator<ArcDisassembler> GeneralOp(bool use3ops)
        {
            return (uInstr, dasm) =>
            {
                var format = genopFormatField.Read(uInstr);
                switch (format)
                {
                case 0: // REG_REG
                    if (use3ops && !A(uInstr, dasm))
                        return false;
                    if (!B_limm(uInstr, dasm))
                        return false;
                    if (!C_limm(uInstr, dasm))
                        return false;
                    return true;
                case 1: // REG_U6IMM
                    if (use3ops && !A(uInstr, dasm))
                        return false;
                    if (!B(uInstr, dasm))
                        return false;
                    var uimm6 = genopUimm6.Read(uInstr);
                    dasm.ops.Add(ImmediateOperand.Word32(uimm6));
                    return true;
                case 2: // REG_S12IMM
                    if (use3ops && !B(uInstr, dasm))
                        return false;
                    if (!B(uInstr, dasm))
                        return false;
                    var simm12 = Bitfield.ReadSignedFields(genopSimm12, uInstr);
                    dasm.ops.Add(ImmediateOperand.Int32(simm12));
                    return true;
                default: // COND_REG
                    if (use3ops && !B(uInstr, dasm))
                        return false;
                    if (!B_limm(uInstr, dasm))
                        return false;
                    var cond = (ArcCondition) genopCondField.Read(uInstr);
                    if (cond >= ArcCondition.Max)
                        return false;
                    dasm.instr.Condition = cond;
                    if (Bits.IsBitSet(uInstr, 5))
                    {
                        uimm6 = genopUimm6.Read(uInstr);
                        dasm.ops.Add(ImmediateOperand.Word32(uimm6));
                    }
                    else
                    {
                        if (!C_limm(uInstr, dasm))
                            return false;
                    }
                    return true;
                }
            };
        }
        private static readonly Mutator<ArcDisassembler> GeneralOp_BC = GeneralOp(false);
        private static readonly Mutator<ArcDisassembler> GeneralOp_ABC = GeneralOp(true);

        #endregion

        #region Decoders

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

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<ArcDisassembler> [] mutators)
        {
            return new InstrDecoder<ArcDisassembler,Mnemonic,ArcInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<ArcDisassembler>[] mutators)
        {
            return new InstrDecoder<ArcDisassembler, Mnemonic, ArcInstruction>(iclass, mnemonic, mutators);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<ArcDisassembler, Mnemonic, ArcInstruction>(message);
        }

        #endregion

        static ArcDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var reserved = Nyi("Reserved");

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
                Instr(Mnemonic.j_s, T, Mr_s(PrimitiveType.Word32)),
                Instr(Mnemonic.j_s, TD, N(true), Mr_s(PrimitiveType.Word32)),
                Instr(Mnemonic.jl_s, InstrClass.Call | T, Mr_s(PrimitiveType.Word32)),
                Instr(Mnemonic.jl_s, InstrClass.Call | TD, N(true), Mr_s(PrimitiveType.Word32)),

                invalid,
                invalid,
                Nyi("SUB_S.NE"),
                ZOPs);

            Decoder BLcc(bool delay)
            {
                var iclass = delay ? (TD | InstrClass.Call) : (T | InstrClass.Call);
                var offset = PcRel4(Bf((6, 10), (18, 9)));
                return Mask(0, 5, " BLcc delay=" + delay,
                    Instr(Mnemonic.blal, iclass, N5, offset),
                    Instr(Mnemonic.bleq, iclass, N5, offset),
                    Instr(Mnemonic.blne, iclass, N5, offset),
                    Instr(Mnemonic.blpl, iclass, N5, offset),

                    Instr(Mnemonic.blmi, iclass, N5, offset),
                    Instr(Mnemonic.blcs, iclass, N5, offset),
                    Instr(Mnemonic.blcc, iclass, N5, offset),
                    Instr(Mnemonic.blvs, iclass, N5, offset),

                    Instr(Mnemonic.blvc, iclass, N5, offset),
                    Instr(Mnemonic.blgt, iclass, N5, offset),
                    Instr(Mnemonic.blge, iclass, N5, offset),
                    Instr(Mnemonic.bllt, iclass, N5, offset),

                    Instr(Mnemonic.blle, iclass, N5, offset),
                    Instr(Mnemonic.blhi, iclass, N5, offset),
                    Instr(Mnemonic.blls, iclass, N5, offset),
                    Instr(Mnemonic.blpnz, iclass, N5, offset),

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
                    invalid,
                    invalid,
                    invalid);
            }

            var breq = Mask(0, 4, "  BRcc",
                Instr(Mnemonic.breq, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                Instr(Mnemonic.brne, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                Instr(Mnemonic.brlt, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                Instr(Mnemonic.brge, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),

                Instr(Mnemonic.brlo, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                Instr(Mnemonic.brhs, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                Instr(Mnemonic.bbit0, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))),
                Instr(Mnemonic.bbit1, CT, B, C, PcRel2(Bf((15, 1), (17, 7)))));

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

            bool C_or_u6(uint uInstr, ArcDisassembler dasm)
            {
                if (Bits.IsBitSet(uInstr, 5))
                {
                    dasm.NotYetImplemented(uInstr, "Jcc u6");
                    return false;
                }
                var ireg = (int) Bits.ZeroExtend(uInstr >> 6, 6);
                var mem = new MemoryOperand(PrimitiveType.Word32);
                if (ireg == LongImmediateDataIndicator)
                {
                    if (!dasm.TryReadLongImmediate(out uint imm))
                        return false;
                    mem.Offset = (int) imm;
                }
                else
                {
                    mem.Base = Registers.CoreRegisters[ireg];
                }
                dasm.ops.Add(mem);
                return true;
            }

            Decoder Jcc(bool delay)
            {
                var iclass = delay ? T : TD;
                var ndelay = N(delay);
                return Mask(22, 2, "  Jcc",
                    Instr(Mnemonic.j, iclass, N(delay), Mr_C),
                    Nyi("  Jcc - 01"),
                    Nyi("  Jcc - 10"),
                    Mask(0, 5, "  Jcc - 11",
                        Instr(Mnemonic.j, iclass,   N(delay), C_or_u6),
                        Instr(Mnemonic.jeq, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jne, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jpl, iclass, N(delay), C_or_u6),

                        Instr(Mnemonic.jmi, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jcs, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jcc, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jvs, iclass, N(delay), C_or_u6),

                        Instr(Mnemonic.jvc, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jgt, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jge, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jlt, iclass, N(delay), C_or_u6),

                        Instr(Mnemonic.jle, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jhi, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jls, iclass, N(delay), C_or_u6),
                        Instr(Mnemonic.jpnz, iclass, C_or_u6),

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
                        invalid,
                        invalid,
                        invalid));
            }

            Decoder JLcc(bool delay)
            {
                var iclass = delay ? (InstrClass.Call | T) : (InstrClass.Call | TD);
                return Mask(22, 2, "  JLcc",
                    Instr(Mnemonic.jl, iclass, N(delay), Mr_C),
                    Nyi("  JLcc - 01"),
                    Nyi("  JLcc - 10"),
                    Nyi("  JLcc - 11"));
            }

            var pcrelU_66 = PcRelU2(6, 6);
            var LPcc = Mask(22, 2, "  LPcc",
                    invalid,
                    invalid,
                    Instr(Mnemonic.lp, PcRel2(Bf((0,6),(6,6)))),
                    Mask(5, 1, "  LPcc bit5",
                        invalid,
                        Mask(0, 5, "  LPcc condition",
                        Instr(Mnemonic.lp,   pcrelU_66),
                        Instr(Mnemonic.lpeq, pcrelU_66),
                        Instr(Mnemonic.lpne, pcrelU_66),
                        Instr(Mnemonic.lppl, pcrelU_66),

                        Instr(Mnemonic.lpmi, pcrelU_66),
                        Instr(Mnemonic.lpcs, pcrelU_66),
                        Instr(Mnemonic.lpcc, pcrelU_66),
                        Instr(Mnemonic.lpvs, pcrelU_66),

                        Instr(Mnemonic.lpvc, pcrelU_66),
                        Instr(Mnemonic.lpgt, pcrelU_66),
                        Instr(Mnemonic.lpge, pcrelU_66),
                        Instr(Mnemonic.lplt, pcrelU_66),

                        Instr(Mnemonic.lple, pcrelU_66),
                        Instr(Mnemonic.lphi, pcrelU_66),
                        Instr(Mnemonic.lpls, pcrelU_66),
                        Instr(Mnemonic.lppnz, pcrelU_66),

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
                        invalid,
                        invalid,
                        invalid)));

            var ZOP = Sparse(24, 3, "  ZOP", invalid,
                (0x01, Nyi("sleep")),
                (0x02, Instr(Mnemonic.trap0, T | InstrClass.Call)),
                (0x03, Nyi("sync")),
                (0x04, Nyi("rtie")),
                (0x05, Instr(Mnemonic.brk, InstrClass.Terminates)));

            var unaries_04 = Sparse(0, 6, "  04 unaries", invalid,
                (0x00, Instr(Mnemonic.asl, F15, B,C)),
                (0x01, Instr(Mnemonic.asr, F15, B,C)),
                (0x02, Instr(Mnemonic.lsr, F15, B,C)),
                (0x03, Instr(Mnemonic.ror, F15, B,C)),
                (0x04, Instr(Mnemonic.rrc, F15, B,C)),
                (0x05, Instr(Mnemonic.sexb, F15, B,C)),
                (0x06, Instr(Mnemonic.sexw, F15, B,C)),
                (0x07, Instr(Mnemonic.extb, F15, B,C)),
                (0x08, Instr(Mnemonic.extw, F15, B,C)),
                (0x09, Instr(Mnemonic.abs, F15, B,C)),
                (0x0A, Instr(Mnemonic.not, F15, B,C)),
                (0x0B, Instr(Mnemonic.rlc, F15, B,C)),
                (0x0C, Instr(Mnemonic.ex, F15, B,C)),
                (0x3F, ZOP));

            var unaries_05 = Sparse(0, 6, "  05 unaries", invalid,
                (0x00, Instr(Mnemonic.swap, B, C)),
                (0x01, Instr(Mnemonic.norm, B, C)),
                (0x02, Instr(Mnemonic.sat16, B, C)),
                (0x03, Instr(Mnemonic.rnd16, B, C)),
                (0x04, Instr(Mnemonic.abssw, B, C)),
                (0x05, Instr(Mnemonic.abss, B, C)),
                (0x06, Instr(Mnemonic.negsw, B, C)),
                (0x07, Instr(Mnemonic.negs, B, C)),
                (0x08, Instr(Mnemonic.normw, B, C)),
                (0x3F, ZOP));

            var majorOpc_04 = new W32Decoder(Sparse(16, 6, "  04 General operation", Nyi("04 General operation"),
                (0x00, Instr(Mnemonic.add, F15, GeneralOp_ABC)),
                (0x01, Instr(Mnemonic.adc, F15, GeneralOp_ABC)),
                (0x02, Instr(Mnemonic.sub, F15, GeneralOp_ABC)),
                (0x03, Instr(Mnemonic.sbc, F15, GeneralOp_ABC)),

                (0x04, Instr(Mnemonic.and, F15, GeneralOp_ABC)),
                (0x05, Instr(Mnemonic.or, F15, GeneralOp_ABC)),
                (0x06, Instr(Mnemonic.bic, F15, GeneralOp_ABC)),
                (0x07, Instr(Mnemonic.xor, F15, GeneralOp_ABC)),
                
                (0x08, Instr(Mnemonic.max, F15, GeneralOp_ABC)),
                (0x09, Instr(Mnemonic.min, F15, GeneralOp_ABC)),
                (0x0A, Instr(Mnemonic.mov, F15, GeneralOp_BC)),
                (0x0B, Instr(Mnemonic.tst, F15, GeneralOp_BC)),
               
                (0x0C, Instr(Mnemonic.cmp, GeneralOp_BC)),
                (0x0D, Instr(Mnemonic.rcmp, F15, GeneralOp_BC)),
                (0x0E, Instr(Mnemonic.rsub, F15, GeneralOp_ABC)),
                (0x0F, Instr(Mnemonic.bset, F15, GeneralOp_ABC)),
                
                (0x10, Instr(Mnemonic.bclr, F15, GeneralOp_ABC)),
                (0x11, Instr(Mnemonic.btst, F15, GeneralOp_BC)),
                (0x12, Instr(Mnemonic.bxor, F15, GeneralOp_ABC)),
                (0x13, Instr(Mnemonic.bmsk, F15, GeneralOp_ABC)),
                
                (0x14, Instr(Mnemonic.add1, F15, GeneralOp_ABC)),
                (0x15, Instr(Mnemonic.add2, F15, GeneralOp_ABC)),
                (0x16, Instr(Mnemonic.add3, F15, GeneralOp_ABC)),
                (0x17, Instr(Mnemonic.sub1, F15, GeneralOp_ABC)),
                
                (0x18, Instr(Mnemonic.sub2, F15, GeneralOp_ABC)),
                (0x19, Instr(Mnemonic.sub3, F15, GeneralOp_ABC)),
                
                (0x20, Jcc(false)),
                (0x21, Jcc(true)),
                (0x22, JLcc(false)),
                (0x23, JLcc(true)),
                (0x28, LPcc),
                (0x29, Instr(Mnemonic.flag, C)),
                (0x2A, Instr(Mnemonic.lr, B, Mext_C)),
                (0x2B, Instr(Mnemonic.sr, B, Mext_C)),
                (0x2F, unaries_04),
                (0x30, Instr(Mnemonic.ld, A, AA22, Di15, Mrr(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((6, 6)))))));

            var majorOpc_05 = new W32Decoder(Sparse(16, 6, "  05 Extension instruction", Nyi("05 Extension instruction"),
                (0x00, Instr(Mnemonic.asl, A, B_limm, C_limm)),
                (0x01, Instr(Mnemonic.lsr, A, B_limm, C_limm)),
                (0x02, Instr(Mnemonic.asr, A, B_limm, C_limm)),
                (0x03, Instr(Mnemonic.ror, A, B_limm, C_limm)),
                (0x04, Instr(Mnemonic.mul64, B_limm, C_limm)),
                (0x05, Instr(Mnemonic.mulu64, B_limm, C_limm)),
                (0x06, Instr(Mnemonic.adds, A, B_limm, C_limm)),
                (0x07, Instr(Mnemonic.subs, A, B_limm, C_limm)),
                (0x08, Instr(Mnemonic.divaw, A, B, C)),
                (0x0A, Instr(Mnemonic.asls, A, B, C)),
                (0x0B, Instr(Mnemonic.asrs, A, B, C)),

                (0x28, Instr(Mnemonic.addsdw, A, B, C)),
                (0x29, Instr(Mnemonic.subsdw, A, B, C)),

                (0x2F, Instr(Mnemonic.subsdw, A, B, C))));

            var SpBasedInstructions = Mask(5, 3, "  18 Sp-based instructions 16-bit",
                Instr(Mnemonic.ld_s, b, Mo_sp_s(PrimitiveType.Word32)),
                Instr(Mnemonic.ldb_s, b, Mo_sp_s(PrimitiveType.Byte)),
                Instr(Mnemonic.st_s, b, Mo_sp_s(PrimitiveType.Word32)),
                Instr(Mnemonic.stb_s, b, Mo_sp_s(PrimitiveType.Byte)),

                Instr(Mnemonic.add_s, b, sp, U(0,5,2)),
                Sparse(8, 3, "  add_s/sub_s", invalid,
                    (0x0, Instr(Mnemonic.add_s, sp,sp,U(0,5,2))),
                    (0x1, Instr(Mnemonic.sub_s, sp,sp,U(0,5,2)))),
                Sparse(0, 5, "  pop_s", invalid,
                    (0x01, Instr(Mnemonic.pop_s, b)),
                    (0x11, Instr(Mnemonic.pop_s, blink))),
                Sparse(0, 5, "  push_s", invalid,
                    (0x01, Instr(Mnemonic.push_s, b)),
                    (0x11, Instr(Mnemonic.push_s, blink))));

            var GpBasedInstructions = Mask(9, 2, "  19 LD_S / LDW_S / LDB_S / ADD_S  Gp-based ld/add (data aligned offset) 16-bit",
                Instr(Mnemonic.ld_s, r0, Mo_reg_s(PrimitiveType.Word32, Registers.Gp, 9)),
                Instr(Mnemonic.ldb_s, r0, Mo_reg_s(PrimitiveType.Byte, Registers.Gp, 9)),
                Instr(Mnemonic.ldw_s, r0, Mo_reg_s(PrimitiveType.Word16, Registers.Gp, 9)),
                Instr(Mnemonic.add_s, r0, Reg(Registers.Gp), Is(0, 8, 2)));


            rootDecoder = Mask(11, 5, "ARC instruction",
                majorOpc_00,
                new W32Decoder(Mask(16, 1, "BLcc, BRcc Branch and link conditional Compare-branch conditional 32-bit",
                    Mask(17, 1, "  BLcc 0?",
                        Mask(5, 1, "  BLcc 00",
                            BLcc(false),
                            BLcc(true)),
                        Mask(5, 1, "  Branch and link unconditionally",
                            Instr(Mnemonic.bl, T | InstrClass.Call, PcRel4(Bf((0, 4), (6, 10), (18, 9)))),
                            Instr(Mnemonic.bl, TD | InstrClass.Call, PcRel4(Bf((0, 4), (6, 10), (18, 9)))))),
                    Mask(4, 1, "    BLcc 1?",
                        breq,
                        Nyi("    BLcc 11")))),
                new W32Decoder(Mask(7, 2, "  02 LD register + offset Delayed load 32-bit",
                    Mask(6, 1, "  X - sign extension",
                        Instr(Mnemonic.ld, A, AA9, Di11, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                        invalid),
                    Instr(Mnemonic.ldb, A, AA9, Di11, X, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                    Instr(Mnemonic.ldw, A, AA9, Di11, X, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                    invalid)),
                new W32Decoder(Mask(1, 2, "  03 ST register + offset Buffered store 32-bit",
                    Instr(Mnemonic.st, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                    Instr(Mnemonic.stb, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                    Instr(Mnemonic.stw, C, AA3, Di5, Mo(PrimitiveType.Word32, Bf((12, 3), (24, 3)), Bf((15, 1), (16, 7)))),
                    invalid)),
                majorOpc_04,
                majorOpc_05, // 05 op  a,b,c ARC 32-bit extension instructions 32-bit
                new W32Decoder(reserved), // 06 op  a,b,c ARC 32-bit extension instructions 32-bit
                new W32Decoder(reserved), // 07 op  a,b,c User 32-bit extension instructions 32-bit
                new W32Decoder(reserved), // 08 op  a,b,c User 32-bit extension instructions 32-bit,
                new W32Decoder(reserved), // 09 op  <market specific> ARC market-specific extension instructions 32-bit
                new W32Decoder(reserved), // 0A op  <market specific> ARC market-specific extension instructions 32-bit
                new W32Decoder(reserved), // 0B op  <market specific> ARC market-specific extension instructions 32-bit

                Mask(3, 2, "  0C LD_S / LDB_S / LDW_S / ADD_S   a,b,c Load/add register-register 16-bit",
                    Instr(Mnemonic.ld_s, a, Midx_s(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldb_s, a, Midx_s(PrimitiveType.Byte)),
                    Instr(Mnemonic.ldw_s, a, Midx_s(PrimitiveType.Word16)),
                    Instr(Mnemonic.add_s, a, b, c)),
                Mask(3, 2, "  0D ADD_S / SUB_S / ASL_S /  LSR_S  c,b,u3 Add/sub/shift immediate 16-bit",
                    Instr(Mnemonic.add_s, c, b, U(0, 3)),
                    Instr(Mnemonic.sub_s, c, b, U(0, 3)),
                    Instr(Mnemonic.asl_s, c, b, U(0, 3)),
                    Instr(Mnemonic.asr_s, c, b, U(0, 3))),
                Mask(3, 2, "  0E MOV_S / CMP_S / ADD_S   b,h / b,b,h One dest/source can be any of r0-r63 16-bit",
                    Instr(Mnemonic.add_s, b, b, h_limm),
                    Instr(Mnemonic.mov_s, b, h_limm),
                    Instr(Mnemonic.cmp_s, b, h_limm),
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
                    Instr(Mnemonic.trap_s, T| InstrClass.Call),
                    Instr(Mnemonic.brk_s, InstrClass.Terminates)),

                // 0x10
                Instr(Mnemonic.ld_s, c, Mo_s(PrimitiveType.Word16)),
                Instr(Mnemonic.ldb_s, c, Mo_s(PrimitiveType.Word16)),
                Instr(Mnemonic.ldw_s, c, Mo_s(PrimitiveType.Word16)),
                Nyi("  13 LDW_S.X c,[b,u6] Delayed load (16-bit aligned offset) 16-bit"),
                
                Instr(Mnemonic.st_s, c, Mo_s(PrimitiveType.Word32)),
                Instr(Mnemonic.stb_s, c, Mo_s(PrimitiveType.Word32)),
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
                GpBasedInstructions,
                Instr(Mnemonic.ld_s, b, Mo_reg_s(PrimitiveType.Word32, Registers.Pcl, 8)),
                Instr(Mnemonic.mov_s, b, U(0,8)),
                
                Mask(7, 1, "  1C ADD_S / CMP_S b,u7 Add/compare immediate 16-bit",
                    Instr(Mnemonic.add_s, b,b,U(0,7)),
                    Instr(Mnemonic.cmp_s, b,U(0,7))),
                Mask(7, 1, "  1D BRcc_S b,0,s8 Branch conditionally on reg z/nz 16-bit",
                    Instr(Mnemonic.breq_s, CT, b,n(0),PcRel2(Bf((0,7)))),
                    Instr(Mnemonic.brne_s, CT, b,n(0),PcRel2(Bf((0,7))))),
                Mask(9, 2, "  1E Bcc_S s10/s7 Branch conditionally 16-bit",
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
                Instr(Mnemonic.bl_s, InstrClass.Call | T,  PcRel4(Bf((0, 11)))));
        }
    }
}