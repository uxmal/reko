#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Qualcomm
{
#pragma warning disable IDE1006

    public class HexagonDisassembler : DisassemblerBase<HexagonPacket, Mnemonic>
    {
        private static readonly Decoder iclassDecoder;
        private static readonly RegisterStorage[] subInstrRegs;
        private static readonly (RegisterStorage, RegisterStorage)[] subInstrRegPairs;

        private readonly HexagonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<HexagonInstruction> instrs;
        private List<MachineOperand> ops;
        private Address addrPacket;
        private MachineOperand? conditionPredicate;
        private bool conditionPredicateInverted;
        private bool conditionPredicateNew;
        private DirectionHint directionHint;
        private uint? extendedConstant;

        public HexagonDisassembler(HexagonArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.instrs = new List<HexagonInstruction>();
            this.ops = new List<MachineOperand>();
        }

        public override HexagonPacket? DisassembleInstruction()
        {
            // All references to PC are relative to the start of the 
            // packet.
            this.addrPacket = rdr.Address;
            instrs.Clear();
            for (; ; )
            {
                if (!rdr.TryReadLeUInt32(out uint uInstr))
                {
                    if (instrs.Count > 0)
                    {
                        // Packet should have been properly terminated.
                        return MakeInvalidPacket(this.addrPacket);
                    }
                    else
                    {
                        return null;
                    }
                }
                var instr = DisassembleInstruction(uInstr);
                if (instr.InstructionClass == InstrClass.Invalid)
                {
                    return MakeInvalidPacket(this.addrPacket);
                }
                instrs.Add(instr);
                if (ShouldTerminatePacket(instr))
                {
                    return MakePacket(this.addrPacket);
                }
            }
        }

        public override HexagonPacket MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            throw new NotImplementedException();
        }

        private static bool ShouldTerminatePacket(HexagonInstruction instr)
        {
            var parseType = instr.ParseType;
            if (parseType == ParseType.Duplex)
                return true;
            if (parseType == ParseType.End)
                return true;
            return false;
        }

        private HexagonPacket MakePacket(Address addr)
        {
            var instrs = new HexagonInstruction[this.instrs.Count];
            var iclass = InstrClass.Linear;
            for (int i = 0; i < instrs.Length; ++i)
            {
                var instr = this.instrs[i];
                instrs[i] = instr;
                if (instr.InstructionClass != InstrClass.Linear)
                {
                    iclass = instr.InstructionClass;
                }
            }
            var packet = new HexagonPacket(instrs)
            {
                InstructionClass = iclass,
                Address = addr,
                Length = (int) (this.rdr.Address - addr)
            };
            this.instrs.Clear();
            return packet;
        }


        private HexagonPacket MakeInvalidPacket(Address addr)
        {
            var packet = MakePacket(addr);
            packet.InstructionClass = InstrClass.Invalid;
            return packet;
        }

        public override HexagonPacket CreateInvalidInstruction()
        {
            return MakeInvalidPacket(this.addrPacket);   //$BUG: should be addrPacket.
        }

        private HexagonInstruction CreateInvalidInstruction(Address addr)
        {
            return new HexagonInstruction(addr, Mnemonic.Invalid, Array.Empty<MachineOperand>())
            {
                InstructionClass = InstrClass.Invalid
            };
        }

        private HexagonInstruction DisassembleInstruction(uint uInstr)
        {
            this.Clear();
            var addr = rdr.Address;
            var instr = iclassDecoder.Decode(uInstr, this);
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        private void Clear()
        {
            this.ops.Clear();
            this.conditionPredicate = null;
            this.conditionPredicateInverted = false;
            this.conditionPredicateNew = false;
            this.directionHint = DirectionHint.None;
            this.extendedConstant = null;
        }

        public override HexagonPacket NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Hexagon_dasm", this.addrPacket, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private uint ExtendConstant(uint uValue)
        {
            if (this.extendedConstant.HasValue)
            {
                uValue = this.extendedConstant.Value | (uValue & 0x3F);
                this.extendedConstant = null;
            }
            return uValue;
        }

        private int ExtendConstant(int sValue)
        {
            if (this.extendedConstant.HasValue)
            {
                sValue = (int)this.extendedConstant.Value | (sValue & 0x3F);
                this.extendedConstant = null;
            }
            return sValue;
        }

        #region Bit fields
        private static readonly Bitfield[] bf_0L6 = Bf((0,6));
        private static readonly Bitfield[] bf_3L1_0L2 = Bf((3,1),(0,2));
        private static readonly Bitfield[] bf_5L6 = Bf((5, 6));
        private static readonly Bitfield[] bf_7L6 = Bf((7,6));
        private static readonly Bitfield[] bf_8L3 = Bf((8, 3));
        private static readonly Bitfield[] bf_20L3 = Bf((20, 3));
        private static readonly Bitfield[] bf_8L4_5L2 = Bf((8,4),(5,2));
        private static readonly Bitfield[] bf_13L1_0L5 = Bf((13, 1), (0, 5));
        private static readonly Bitfield[] bf_13L1_3L5 = Bf((13, 1), (3, 5));
        private static readonly Bitfield[] bf_13L1_6L1 = Bf((13, 1), (6, 1));
        private static readonly Bitfield[] bf_13L1_7L1 = Bf((13, 1), (7, 1));
        private static readonly Bitfield[] bf_21L3 = Bf((21, 3));
        private static readonly Bitfield[] bf_24L3 = Bf((24, 3));
        #endregion

        #region Mutators

        /// <summary>
        /// 5-bit register encoding
        /// </summary>
        private static Mutator<HexagonDisassembler> R(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = Registers.GpRegs[regEnc];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> R0 = R(0);
        private static readonly Mutator<HexagonDisassembler> R8 = R(8);
        private static readonly Mutator<HexagonDisassembler> R16 = R(16);

        /// <summary>
        /// 4-bit register encoding
        /// </summary>
        private static Mutator<HexagonDisassembler> R_4(int bitpos)
        {
            var regField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = Registers.GpRegs[regEnc];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> R0_4 = R_4(8);
        private static readonly Mutator<HexagonDisassembler> R4_4 = R_4(8);
        private static readonly Mutator<HexagonDisassembler> R8_4 = R_4(8);
        private static readonly Mutator<HexagonDisassembler> R16_4 = R_4(16);
        private static readonly Mutator<HexagonDisassembler> R20_4 = R_4(20);


        /// <summary>
        /// High or low part of register
        /// </summary>
        private static Mutator<HexagonDisassembler> Rslice(int regBitpos, int offset)
        {
            {
                var regField = new Bitfield(regBitpos, 5);
                return (u, d) =>
                {
                    var regEnc = regField.Read(u);
                    var reg = Registers.GpRegs[regEnc];
                    d.ops.Add(new DecoratorOperand(PrimitiveType.Word16, reg)
                    {
                        BitOffset = offset
                    });
                    return true;
                };
            }
        }
        private static readonly Mutator<HexagonDisassembler> R0_H = Rslice(0, 16);
        private static readonly Mutator<HexagonDisassembler> R8_L = Rslice(8, 0);
        private static readonly Mutator<HexagonDisassembler> R8_H = Rslice(8, 16);
        private static readonly Mutator<HexagonDisassembler> R16_L = Rslice(16, 0);
        private static readonly Mutator<HexagonDisassembler> R16_H = Rslice(16, 16);

        /// <summary>
        /// 4-bit register encoding in duplex sub-instruction.
        /// </summary>
        private static Mutator<HexagonDisassembler> r(int bitpos)
        {
            var regField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = subInstrRegs[regEnc];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> r0 = r(0);
        private static readonly Mutator<HexagonDisassembler> r4 = r(4);

        /// <summary>
        /// Register pair
        /// </summary>
        private static Mutator<HexagonDisassembler> RR(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                if ((regEnc & 1) == 1)
                    return false;
                var regLo = Registers.GpRegs[regEnc];
                var regHi = Registers.GpRegs[regEnc+1];
                d.ops.Add(new RegisterPairOperand(regHi, regLo));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> RR0 = RR(0);
        private static readonly Mutator<HexagonDisassembler> RR8 = RR(8);
        private static readonly Mutator<HexagonDisassembler> RR16 = RR(16);

        /// <summary>
        /// 4-bit register pair encoding in duplex sub-instruction.
        /// </summary>
        private static Mutator<HexagonDisassembler> rr(int bitpos)
        {
            var regField = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var (hi, lo) = subInstrRegPairs[regEnc];
                d.ops.Add(new RegisterPairOperand(hi, lo));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> rr0 = rr(0);


        /// <summary>
        /// Predicate registers.
        /// </summary>
        private static Mutator<HexagonDisassembler> Predicate(int bitpos, int bitwidth)
        {
            var field = new Bitfield(bitpos, bitwidth);
            return (u, d) =>
            {
                var predEnc = field.Read(u);
                var pred = Registers.PredicateRegisters[predEnc];
                d.ops.Add(pred);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> P_0L2 = Predicate(0, 2);
        private static readonly Mutator<HexagonDisassembler> P_5L2 = Predicate(5, 2);
        private static readonly Mutator<HexagonDisassembler> P_6L2 = Predicate(6, 2);
        private static readonly Mutator<HexagonDisassembler> P_8L2 = Predicate(8, 2);
        private static readonly Mutator<HexagonDisassembler> P_12L1 = Predicate(12, 1);
        private static readonly Mutator<HexagonDisassembler> P_16L2 = Predicate(16, 2);
        private static readonly Mutator<HexagonDisassembler> P_21L2 = Predicate(21, 2);
        private static readonly Mutator<HexagonDisassembler> P_23L2 = Predicate(23, 2);
        private static readonly Mutator<HexagonDisassembler> P_29L2 = Predicate(29, 2);


        /// <summary>
        /// Control register
        /// </summary>
        private static Mutator<HexagonDisassembler> C(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = Registers.ControlRegisters[regEnc];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> C0 = C(0);
        private static readonly Mutator<HexagonDisassembler> C16 = C(16);

        private static Mutator<HexagonDisassembler> CC(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                if ((regEnc & 1) == 1)
                    return false;
                var regLo = Registers.ControlRegisters[regEnc];
                var regHi = Registers.ControlRegisters[regEnc + 1];
                d.ops.Add(new RegisterPairOperand(regHi, regLo));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> CC0 = CC(0);
        private static readonly Mutator<HexagonDisassembler> CC16 = CC(16);

        /// <summary>
        /// Guest control register
        /// </summary>
        private static Mutator<HexagonDisassembler> G(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = Registers.GuestControlRegisters[regEnc];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> G0 = G(0);
        private static readonly Mutator<HexagonDisassembler> G16 = G(16);

        private static Mutator<HexagonDisassembler> GG(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                if ((regEnc & 1) == 1)
                    return false;
                var regLo = Registers.GuestControlRegisters[regEnc];
                var regHi = Registers.GuestControlRegisters[regEnc + 1];
                d.ops.Add(new RegisterPairOperand(regHi, regLo));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> GG0 = GG(0);
        private static readonly Mutator<HexagonDisassembler> GG16 = GG(16);

        /// <summary>
        /// System register
        /// </summary>
        private static Mutator<HexagonDisassembler> S(int bitpos)
        {
            var regField = new Bitfield(bitpos, 6);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                if (!Registers.SystemRegisters.TryGetValue(regEnc, out var reg))
                    return false;
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> S0 = S(0);
        private static readonly Mutator<HexagonDisassembler> S16 = S(16);


        /// <summary>
        /// System register pair
        /// </summary>
        private static Mutator<HexagonDisassembler> SS(int bitpos)
        {
            var regField = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                if ((regEnc & 1) == 1 ||
                   !Registers.SystemRegisters.TryGetValue(regEnc, out var regLo) ||
                   !Registers.SystemRegisters.TryGetValue(regEnc + 1, out var regHi))
                    return false;
                d.ops.Add(new RegisterPairOperand(regHi, regLo));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> SS0 = SS(0);
        private static readonly Mutator<HexagonDisassembler> SS16 = SS(16);

        private static Mutator<HexagonDisassembler> Reg(RegisterStorage register)
        {
            return (u, d) =>
            {
                d.ops.Add(register);
                return true;
            };
        }

        private static readonly Mutator<HexagonDisassembler> P0 = Reg(Registers.PredicateRegisters[0]);
        private static readonly Mutator<HexagonDisassembler> P1 = Reg(Registers.PredicateRegisters[1]);

        /// <summary>
        /// Reference a new value, produced by an instruction earlier in the packet.
        /// </summary>
        private static Mutator<HexagonDisassembler> NewValue(int bitoffset)
        {
            var bf = new Bitfield(bitoffset, 3);
            return (u, d) =>
            {
                var n = bf.Read(u);
                if ((n & 1) != 0 || n == 0)
                    return false;       // "Nt[0] is reserved and should always be encoded as zero. A non-zero value produces undefined results."
                var instrsBack = n >> 1;
                var bytesBack = (instrsBack + 1) * 4;
                d.rdr.Offset -= bytesBack;
                bool success = d.rdr.TryReadLeUInt32(out uint uInstr);
                d.rdr.Offset += bytesBack;
                if (!success)
                {
                    return false;
                }
                var addrInstr = d.addrPacket;
                var ops = d.ops;
                d.ops = new List<MachineOperand>();
                var instrProducer = d.DisassembleInstruction(uInstr);
                d.ops = ops;
                d.addrPacket = addrInstr;
                if (instrProducer.Mnemonic == Mnemonic.ASSIGN &&
                    instrProducer.Operands[0] is RegisterStorage reg)
                {
                    d.ops.Add(new DecoratorOperand(reg.DataType, reg) { NewValue = true });
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }
        private static readonly Mutator<HexagonDisassembler> New0 = NewValue(0);
        private static readonly Mutator<HexagonDisassembler> New8 = NewValue(8);
        private static readonly Mutator<HexagonDisassembler> New16 = NewValue(16);

        /// <summary>
        /// Unsigned immediate value.
        /// </summary>
        private static Mutator<HexagonDisassembler> uimm(PrimitiveType width, Bitfield[] fields)
        {
            return (u, d) =>
            {
                var uValue = Bitfield.ReadFields(fields, u);
                uValue = d.ExtendConstant(uValue);
                var op = Constant.Create(width, uValue);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> uh_0_22 = uimm(PrimitiveType.Word16, Bf((0, 14),(22, 2)));
        private static readonly Mutator<HexagonDisassembler> ub_5L7 = uimm(PrimitiveType.Byte, Bf((5, 7)));
        private static readonly Mutator<HexagonDisassembler> ub_5L8 = uimm(PrimitiveType.Byte, Bf((5, 8)));

        private static readonly Mutator<HexagonDisassembler> uh_5L7 = uimm(PrimitiveType.Word16, Bf((5, 7)));

        private static readonly Mutator<HexagonDisassembler> uw_0_2 = uimm(PrimitiveType.Word32, Bf((0, 2)));
        private static readonly Mutator<HexagonDisassembler> uw_0L5 = uimm(PrimitiveType.Word32, Bf((0, 5)));
        private static readonly Mutator<HexagonDisassembler> uw_3_2 = uimm(PrimitiveType.Word32, Bf((3, 2)));
        private static readonly Mutator<HexagonDisassembler> uw_4_6 = uimm(PrimitiveType.Word32, Bf((4, 6)));
        private static readonly Mutator<HexagonDisassembler> uw_5L2 = uimm(PrimitiveType.Word32, Bf((5, 2)));
        private static readonly Mutator<HexagonDisassembler> uw_5L3 = uimm(PrimitiveType.Word32, Bf((5, 3)));
        private static readonly Mutator<HexagonDisassembler> uw_5L5 = uimm(PrimitiveType.Word32, Bf((5, 5)));
        private static readonly Mutator<HexagonDisassembler> uw_5L8 = uimm(PrimitiveType.Word32, Bf((5, 8)));
        private static readonly Mutator<HexagonDisassembler> uw_5L9 = uimm(PrimitiveType.Word32, Bf((5, 9)));
        private static readonly Mutator<HexagonDisassembler> uw_7L4 = uimm(PrimitiveType.Word32, Bf((7, 4)));
        private static readonly Mutator<HexagonDisassembler> uw_7L5 = uimm(PrimitiveType.Word32, Bf((7, 5)));
        private static readonly Mutator<HexagonDisassembler> uw_7L6 = uimm(PrimitiveType.Word32, Bf((7, 6)));
        private static readonly Mutator<HexagonDisassembler> uw_8L1 = uimm(PrimitiveType.Word32, Bf((8, 1)));
        private static readonly Mutator<HexagonDisassembler> uw_8L5 = uimm(PrimitiveType.Word32, Bf((8, 5)));
        private static readonly Mutator<HexagonDisassembler> uw_8L5_2L3 = uimm(PrimitiveType.Word32, Bf((8, 5), (2,3)));
        private static readonly Mutator<HexagonDisassembler> uw_8L6 = uimm(PrimitiveType.Word32, Bf((8, 6)));
        private static readonly Mutator<HexagonDisassembler> uw_16L5_5L2_0L2 = uimm(PrimitiveType.Word32, Bf((16,5), (5,2), (0,2)));
        private static readonly Mutator<HexagonDisassembler> uw_16L5_13L1 = uimm(PrimitiveType.Word32, Bf((16,5), (13,1)));
        private static readonly Mutator<HexagonDisassembler> uw_20L6 = uimm(PrimitiveType.Word32, Bf((20,6)));
        private static readonly Mutator<HexagonDisassembler> uw_21L2_5L3 = uimm(PrimitiveType.Word32, Bf((21,2), (8,3)));
        private static readonly Mutator<HexagonDisassembler> uw_21L3_5L3 = uimm(PrimitiveType.Word32, Bf((21,3), (8,3)));
        private static readonly Mutator<HexagonDisassembler> uw_21L3_13L1_4L3_2L1 = uimm(PrimitiveType.Word32, Bf((21, 3), (13, 1), (4, 3), (2, 1)));
        private static readonly Mutator<HexagonDisassembler> uw_21L2_13L1_5L3 = uimm(PrimitiveType.Word32, Bf((21, 3), (13, 1), (5, 3))); 
        

        private static Mutator<HexagonDisassembler> uimmSh(PrimitiveType width, Bitfield[] fields, int shift)
        {
            return (u, d) =>
            {
                var uValue = Bitfield.ReadFields(fields, u) << shift;
                uValue = d.ExtendConstant(uValue);
                var op = Constant.Create(width, uValue);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> uw_0L11_3 = uimmSh(PrimitiveType.Word32, Bf((0, 11)), 3);
        private static readonly Mutator<HexagonDisassembler> uw4_5_sh3 = uimmSh(PrimitiveType.Word32, Bf((4, 5)), 3);
        private static readonly Mutator<HexagonDisassembler> uw_21L2_13L1_5L3_2 = uimmSh(PrimitiveType.Word32, Bf((21, 3), (13, 1), (5, 3)), 2);
        
        /// <summary>
        /// Sign-extended immediates.
        /// </summary>
        private static Mutator<HexagonDisassembler> simm(PrimitiveType width, Bitfield[] fields)
        {
            return (u, d) =>
            {
                var uValue = (uint) Bitfield.ReadSignedFields(fields, u);
                uValue = d.ExtendConstant(uValue);
                var op = Constant.Create(width, uValue);
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<HexagonDisassembler> sb_0L8 = simm(PrimitiveType.Byte, Bf((0, 8)));
        private static readonly Mutator<HexagonDisassembler> sb_5L8 = simm(PrimitiveType.Byte, Bf((5, 8)));

        private static readonly Mutator<HexagonDisassembler> sh_0L8 = simm(PrimitiveType.Word16, Bf((0, 8)));
        private static readonly Mutator<HexagonDisassembler> sh_5L8 = simm(PrimitiveType.Word16, Bf((5, 8)));

        private static readonly Mutator<HexagonDisassembler> sw_4L7 = simm(PrimitiveType.Word32, Bf((4, 7)));
        private static readonly Mutator<HexagonDisassembler> sw_0L8 = simm(PrimitiveType.Word32, Bf((0, 8)));
        private static readonly Mutator<HexagonDisassembler> sw_5L8 = simm(PrimitiveType.Word32, Bf((5, 8)));

        private static readonly Mutator<HexagonDisassembler> sw5_10 = simm(PrimitiveType.Word32, Bf((21, 1), (5, 9)));
        private static readonly Mutator<HexagonDisassembler> sw_7L6 = simm(PrimitiveType.Word32, Bf((7, 6)));
        private static readonly Mutator<HexagonDisassembler> sw16_13 = simm(PrimitiveType.Word32, Bf((16, 7), (13, 1)));
        private static readonly Mutator<HexagonDisassembler> sw_16L4_5L8 = simm(PrimitiveType.Word32, Bf((16, 4), (5, 8)));
        private static readonly Mutator<HexagonDisassembler> sw_16L7_13L1 = simm(PrimitiveType.Word32, Bf((16, 7), (13, 1)));
        private static readonly Mutator<HexagonDisassembler> sw5_16_22 = simm(PrimitiveType.Word32, Bf((22, 2),(16, 5), (5, 9)));
        private static readonly Mutator<HexagonDisassembler> sw_21L7_5L9 = simm(PrimitiveType.Word32, Bf((21, 7), (5, 9)));
        private static readonly Mutator<HexagonDisassembler> sw_21L1_5L9 = simm(PrimitiveType.Word32, Bf((21, 1), (5, 9)));
        private static readonly Mutator<HexagonDisassembler> sw_21L2_13L1_5L3 = simm(PrimitiveType.Word32, Bf((21, 3), (13, 1), (5, 3)));

        private static Mutator<HexagonDisassembler> Literal(Constant c)
        {
            return (u, d) =>
            {
                d.ops.Add(c);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> Imm_0 = Literal(Constant.Zero(PrimitiveType.Word32));
        private static readonly Mutator<HexagonDisassembler> Imm_1 = Literal(Constant.Int32(1));
        private static readonly Mutator<HexagonDisassembler> Imm_Minus1 = Literal(Constant.Int32(-1));
        private static readonly Mutator<HexagonDisassembler> Imm_0xFF = Literal(Constant.Word32(0xFF));


        /// <summary>
        /// Base+signed offset memory access
        /// </summary>
        private static Mutator<HexagonDisassembler> M(PrimitiveType width, int baseRegPos, Bitfield[] offsetFields)
        {
            var baseRegField = new Bitfield(baseRegPos, 5);
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(offsetFields, u);
                offset = offset * width.Size;
                offset = d.ExtendConstant(offset);
                var baseReg = Registers.GpRegs[baseRegField.Read(u)];
                var mem = new MemoryOperand(width)
                {
                    Base = baseReg,
                    Offset = offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Midx(PrimitiveType width, int baseRegPos, int indexRegPos, Bitfield[] bfsShiftAmt)
        {
            var bfBaseReg = new Bitfield(baseRegPos, 5);
            var bfIndexReg = new Bitfield(indexRegPos, 5);
            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[bfBaseReg.Read(u)];
                var indexReg = Registers.GpRegs[bfIndexReg.Read(u)];
                var shift = (int)Bitfield.ReadFields(bfsShiftAmt, u);
                var mem = new MemoryOperand(width)
                {
                    Base = baseReg,
                    Index = indexReg,
                    Shift = shift,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Midx(PrimitiveType width, int indexRegPos, Bitfield[] bfsShiftAmt, Bitfield[] bfsOffset)
        {
            var indexRegField = new Bitfield(indexRegPos, 5);
            return (u, d) =>
            {
                var offset = d.ExtendConstant(Bitfield.ReadFields(bfsOffset, u));
                var indexReg = Registers.GpRegs[indexRegField.Read(u)];
                var shift = (int)Bitfield.ReadFields(bfsShiftAmt, u);
                var mem = new MemoryOperand(width)
                {
                    Index = indexReg,
                    Shift = shift,
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Memory access with unsigned offset in duplex sub-instruction
        /// </summary>
        private static Mutator<HexagonDisassembler> m(PrimitiveType width, int offsetPos, int offsetLen)
        {
            var regField = new Bitfield(4, 4);
            var offField = new Bitfield(offsetPos, offsetLen);
            return (u, d) =>
            {
                var offset = offField.Read(u) * width.Size;
                var baseReg = regField.Read(u);
                var mem = new MemoryOperand(width)
                {
                    Base = subInstrRegs[baseReg],
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> mw = m(PrimitiveType.Word32, 8, 3);
        private static readonly Mutator<HexagonDisassembler> mub = m(PrimitiveType.Byte, 8, 3);

        /// <summary>
        /// Memory access with unsigned offset.
        /// </summary>
        private static Mutator<HexagonDisassembler> m(PrimitiveType width, int baseReg, int offsetPos, int offsetLen)
        {
            var offField = new Bitfield(offsetPos, offsetLen);
            var baseField = new Bitfield(baseReg, 5);
            return (u, d) =>
            {
                var offset = offField.Read(u) * width.Size;
                var ireg = baseField.Read(u);
                var mem = new MemoryOperand(width)
                {
                    Base = Registers.GpRegs[ireg],
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Memory access with unsigned offset, where the base is using 4-bit encoding.
        /// </summary>
        private static Mutator<HexagonDisassembler> m4(PrimitiveType width, int baseReg, Bitfield[] offField)
        {
            var baseField = new Bitfield(baseReg, 4);
            return (u, d) =>
            {
                var offset = Bitfield.ReadFields(offField, u) * width.Size;
                var ireg = baseField.Read(u);
                var mem = new MemoryOperand(width)
                {
                    Base = subInstrRegs[ireg],
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }


        /// <summary>
        /// Memory access with unsigned offset from a specific register.
        /// </summary>
        private static Mutator<HexagonDisassembler> m(PrimitiveType width, RegisterStorage baseReg, int offsetPos, int offsetLen)
        {
            var offField = new Bitfield(offsetPos, offsetLen);
            return (u, d) =>
            {
                var offset = offField.Read(u) * width.Size;
                var mem = new MemoryOperand(width)
                {
                    Base = baseReg,
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Memory access with signed offset from a specific register.
        /// </summary>
        private static Mutator<HexagonDisassembler> ms(PrimitiveType width, RegisterStorage baseReg, int offsetPos, int offsetLen)
        {
            var offField = new Bitfield(offsetPos, offsetLen);
            return (u, d) =>
            {
                var offset = offField.ReadSigned(u) * width.Size;
                var mem = new MemoryOperand(width)
                {
                    Base = baseReg,
                    Offset = offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> MabsSet(PrimitiveType width, int bitposBase, Bitfield[] imm)
        {
            var bfBase = new Bitfield(bitposBase, 5);
            return (u, d) =>
            {
                var regBase = bfBase.Read(u);
                var uAddr = Bitfield.ReadFields(imm, u);
                uAddr = d.ExtendConstant(uAddr);
                var mem = new MemoryOperand(width)
                {
                    Base = Registers.GpRegs[regBase],
                    Offset = (int) uAddr,
                    IsAbsoluteSet = true,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Mpostinc(PrimitiveType width, int bitposBase, (int pos, int len) increment, int shift)
        {
            var bfBase = new Bitfield(bitposBase, 5);
            var bfIncrement = new Bitfield(increment.pos, increment.len);
            return (u, d) =>
            {
                var regBase = bfBase.Read(u);
                var incr = bfIncrement.ReadSigned(u) << shift;
                var mem = new MemoryOperand(width)
                {
                    Base = Registers.GpRegs[regBase],
                    AutoIncrement = incr,
                };
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Post-increment using a Mu register.
        /// </summary>
        private static Mutator<HexagonDisassembler> MpostincM(PrimitiveType width, int bitposBase)
        {
            var bfBase = new Bitfield(bitposBase, 5);
            var bfMu = new Bitfield(12, 1);
            return (u, d) =>
            {
                var regBase = bfBase.Read(u);
                var incr = bfMu.Read(u);
                var mem = new MemoryOperand(width)
                {
                    Base = Registers.GpRegs[regBase],
                    AutoIncrement = Registers.ModifierRegisters[incr],
                };
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Use a specific base register in memory access.
        /// </summary>
        private static Mutator<HexagonDisassembler> Mreg(PrimitiveType width, RegisterStorage reg,  Bitfield[] offsetFields, int shift)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadFields(offsetFields, u) << shift;
                offset = d.ExtendConstant(offset);
                var mem = new MemoryOperand(width)
                {
                    Base = reg,
                    Offset = (int) offset
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> Mw_sp_4L5_2 = Mreg(PrimitiveType.Word64, Registers.sp, Bf((4, 5)), 2);
        private static readonly Mutator<HexagonDisassembler> Md_sp_3L5_3 = Mreg(PrimitiveType.Word64, Registers.sp, Bf((3, 5)), 3);
        

        private static readonly Mutator<HexagonDisassembler> Mb_gp_5_13_16_25 = Mreg(PrimitiveType.Byte, Registers.gp, Bf((25, 2), (16, 5), (13, 1), (5, 8)), 2);
        private static readonly Mutator<HexagonDisassembler> Mh_gp_5_13_16_25 = Mreg(PrimitiveType.Word16, Registers.gp, Bf((25, 2), (16, 5), (13, 1), (5, 8)), 2);
        private static readonly Mutator<HexagonDisassembler> Mw_gp_5_13_16_25 = Mreg(PrimitiveType.Word32, Registers.gp, Bf((25, 2), (16, 5), (13, 1), (5, 8)), 2);
        private static readonly Mutator<HexagonDisassembler> Md_gp_5_13_16_25 = Mreg(PrimitiveType.Word64, Registers.gp, Bf((25, 2), (16, 5), (13, 1), (5, 8)), 2);

        /// <summary>
        /// PC-Relative jump with optional extension from previous instruction.
        /// </summary>
        private static Mutator<HexagonDisassembler> PcRelExt(Bitfield[] bfOffset, int shift)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(bfOffset, u) << shift;
                var addrDst = d.addrPacket + offset;
                d.ops.Add(addrDst);
                return true;
            };
        }


//            0,1,0,1, 1,1,0, 1,j,j,0,j,j,j,j,j,P,P,j,-,0,-,u,u,j,j,j,j,j,j,j,-,"if (Pu) call #r15:2"

        private static readonly Mutator<HexagonDisassembler> PcRelExt_15_2 = PcRelExt(Bf((22, 2), (16, 5), (13, 1), (1, 7)), 2);
        private static readonly Mutator<HexagonDisassembler> PcRelExt_22_2 = PcRelExt(Bf((16, 9), (1, 13)), 2);
        private static readonly Mutator<HexagonDisassembler> PcRelExt_1L13_2 = PcRelExt(Bf((1, 13)), 2);
        private static readonly Mutator<HexagonDisassembler> PcRelExt_8L5_3L2 = PcRelExt(Bf((8, 5), (3, 2)), 2);
        private static readonly Mutator<HexagonDisassembler> PcRelExt_20L2_1L7 = PcRelExt(Bf((20, 2), (1, 7)), 2);

        private static Mutator<HexagonDisassembler> Apply(Mnemonic mnemonic, params Mutator<HexagonDisassembler> [] mutators)
        {
            return (u, d) =>
            {
                return ApplyMutators(mnemonic, mutators, null, u, d);
            };
        }

        private static Mutator<HexagonDisassembler> Apply(Mnemonic mnemonic, PrimitiveType dt, params Mutator<HexagonDisassembler>[] mutators)
        {
            return (u, d) =>
            {
                return ApplyMutators(mnemonic, mutators, dt, u, d);
            };
        }

        private static bool ApplyMutators(Mnemonic mnemonic, Mutator<HexagonDisassembler>[] mutators, PrimitiveType? dt, uint u, HexagonDisassembler d)
        {
            var opsOld = d.ops;
            d.ops = new List<MachineOperand>();
            bool success = true;
            foreach (var m in mutators)
            {
                success &= m(u, d);
                if (!success)
                    break;
            }
            if (success)
            {
                var dtReturn = dt is not null
                    ? dt
                    : d.ops.Count > 0
                        ? d.ops[0].DataType
                        : VoidType.Instance;
                var op = new ApplicationOperand(dtReturn, mnemonic, d.ops.ToArray());
                opsOld.Add(op);
            }
            d.ops = opsOld;
            return success;
        }

        private static Mutator<HexagonDisassembler> InvertIfSet(int bitPos, Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                if (Bits.IsBitSet(u, bitPos))
                {
                    int i = d.ops.Count - 1;
                    var op = d.ops[i];
                    op = new DecoratorOperand(op.DataType, op)
                    {
                        Inverted = true,
                    };
                    d.ops[i] = op;
                }
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Conditional(int bitposPredicate, int bitposNew, int bitposHint, int bitposInvert)
        {
            var bfPredicate = new Bitfield(bitposPredicate, 2);
            return (u, d) =>
            {
                d.conditionPredicate = Registers.PredicateRegisters[bfPredicate.Read(u)];
                d.conditionPredicateNew = bitposNew >= 0 && Bits.IsBitSet(u, bitposNew);
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }

        /// <summary>
        /// Conditional, but only 1 bit used for the predicate register,
        /// resulting in p0 and p1.
        /// </summary>
        private static Mutator<HexagonDisassembler> Conditional_1(int bitposPredicate, int bitposNew, int bitposHint, int bitposInvert)
        {
            var bfPredicate = new Bitfield(bitposPredicate, 1);
            return (u, d) =>
            {
                d.conditionPredicate = Registers.PredicateRegisters[bfPredicate.Read(u)];
                d.conditionPredicateNew = bitposNew >= 0 && Bits.IsBitSet(u, bitposNew);
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }


        private static Mutator<HexagonDisassembler> Conditional_p0(int bitposNew, int bitposHint, int bitposInvert)
        {
            return (u, d) =>
            {
                d.conditionPredicate = Registers.PredicateRegisters[0];
                d.conditionPredicateNew = bitposNew >= 0 && Bits.IsBitSet(u, bitposNew);
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Conditional_p0_new(int bitposHint, int bitposInvert)
        {
            return (u, d) =>
            {
                d.conditionPredicate = Registers.PredicateRegisters[0];
                d.conditionPredicateNew = true;
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Conditional_p1_new(int bitposHint, int bitposInvert)
        {
            return (u, d) =>
            {
                d.conditionPredicate = Registers.PredicateRegisters[1];
                d.conditionPredicateNew = true;
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> ConditionalApply(
            int bitposInvert,
            int bitposHint,
            Mnemonic mnemonic, 
            params Mutator<HexagonDisassembler> [] mutators)
        {
            return (u, d) =>
            {
                if (!ApplyMutators(mnemonic, mutators, null, u, d))
                    return false;
                d.conditionPredicate = d.ops[d.ops.Count - 1];
                d.ops.RemoveAt(d.ops.Count - 1);
                d.directionHint = bitposHint >= 0
                    ? Bits.IsBitSet(u, bitposHint) ? DirectionHint.Taken : DirectionHint.NotTaken
                    : DirectionHint.None;
                d.conditionPredicateInverted = bitposInvert >= 0 && Bits.IsBitSet(u, bitposInvert);
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Comp(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Complement = true,
                };
                d.ops[i] = op;
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Chop(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Chop = true,
                };
                d.ops[i] = op;
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Carry(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Carry = true,
                };
                d.ops[i] = op;
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Sat(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Sat = true,
                };
                d.ops[i] = op;
                return true;
            };
        }

        private static Mutator<HexagonDisassembler> Rnd(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Rnd = true,
                };
                d.ops[i] = op;
                return true;
            };
        }


        private static Mutator<HexagonDisassembler> Lsl16(Mutator<HexagonDisassembler> mutator)
        {
            return (u, d) =>
            {
                if (!mutator(u, d))
                    return false;
                int i = d.ops.Count - 1;
                var op = d.ops[i];
                op = new DecoratorOperand(op.DataType, op)
                {
                    Lsl16 = true,
                };
                d.ops[i] = op;
                return true;
            };
        }

        #endregion

        #region Decoders

        // We cannot reuse the standard decoders because of the awkward encoding of instructions/packets.
        public abstract class Decoder
        {
            public abstract HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Mnemonic mnemonic;
            private readonly Mutator<HexagonDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, params Mutator<HexagonDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.CreateInvalidInstruction(dasm.addrPacket);
                }
                var instr = new HexagonInstruction(dasm.addrPacket, this.mnemonic, dasm.ops.ToArray())
                {
                    InstructionClass = this.iclass,
                    ParseType = (ParseType) ((uInstr >> 14) & 3),
                    ConditionPredicate = dasm.conditionPredicate,
                    ConditionInverted = dasm. conditionPredicateInverted,
                    ConditionPredicateNew = dasm.conditionPredicateNew,
                    DirectionHint = dasm.directionHint,
                };
                return instr;
            }
        }

        public class DuplexDecoder : Decoder
        {
            private readonly Decoder slot0;
            private readonly Decoder slot1;

            public DuplexDecoder(Decoder slot0, Decoder slot1)
            {
                this.slot0 = slot0;
                this.slot1 = slot1;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                dasm.instrs.Add(slot1.Decode(uInstr >> 16, dasm));
                dasm.Clear();
                return slot0.Decode(uInstr, dasm);
            }
        }


        public class MaskDecoder : Decoder
        {
            private readonly Bitfield field;
            private readonly string tag;
            private readonly Decoder[] subdecoders;

            public MaskDecoder(Bitfield field, string tag, Decoder[] subdecoders)
            {
                if (subdecoders.Length != (1 << field.Length))
                    throw new InvalidOperationException($"Expected {1 << field.Length} decoders but received {subdecoders.Length}.");
                this.field = field;
                this.tag = tag;
                this.subdecoders = subdecoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                Core.Machine.Decoder.DumpMaskedInstruction(32, uInstr, field.Mask << field.Position, tag);
                var subfield = field.Read(uInstr);
                return subdecoders[subfield].Decode(uInstr, dasm);
            }
        }


        public class BitfieldDecoder : Decoder
        {
            private readonly Bitfield[] bitfields;
            private readonly string tag;
            private readonly Decoder[] subdecoders;

            public BitfieldDecoder(Bitfield[] fields, string tag, Decoder[] subdecoders)
            {
                this.bitfields = fields;
                this.tag = tag;
                this.subdecoders = subdecoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                Core.Machine.Decoder.DumpMaskedInstruction(32, uInstr, bitfields, tag);
                var subfield = Bitfield.ReadFields(bitfields, uInstr);
                return subdecoders[subfield].Decode(uInstr, dasm);
            }
        }

        public class SelectDecoder : Decoder
        {
            private readonly Bitfield[] fields;
            private readonly Func<uint, bool> predicate;
            private readonly Decoder decoderTrue;
            private readonly Decoder decoderFalse;

            public SelectDecoder(Bitfield[] fields, Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
            {
                this.fields = fields;
                this.predicate = predicate;
                this.decoderTrue = decoderTrue;
                this.decoderFalse = decoderFalse;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                var u = Bitfield.ReadFields(fields, uInstr);
                var decoder = predicate(u) ? decoderTrue : decoderFalse;
                return decoder.Decode(uInstr, dasm);
            }
        }

        public class IfDecoder : Decoder
        {
            private readonly Bitfield field;
            private readonly Predicate<uint> predicate;
            private readonly Decoder decoderTrue;

            public IfDecoder(Bitfield field, Predicate<uint> predicate, Decoder decoderTrue)
            {
                this.field = field;
                this.predicate = predicate;
                this.decoderTrue = decoderTrue;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                Reko.Core.Machine.Decoder.DumpMaskedInstruction(32, uInstr, field.Mask << field.Position, "  If");
                var u = field.Read(uInstr);
                if (predicate(u))
                    return decoderTrue.Decode(uInstr, dasm);
                return dasm.CreateInvalidInstruction(dasm.addrPacket);
            }
        }

        public class SequenceDecoder : Decoder
        {
            private readonly Decoder[] subdecoders;

            public SequenceDecoder(Decoder[] subdecoders)
            {
                this.subdecoders = subdecoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                for (int i = 0; i < subdecoders.Length-1; ++i)
                {
                    var instr = subdecoders[i].Decode(uInstr, dasm);
                    dasm.instrs.Add(instr);
                    dasm.Clear();
                }
                return subdecoders[^1].Decode(uInstr, dasm);
            }
        }

        public class ExtensionDecoder : Decoder
        {
            private static readonly Bitfield[] bfConstant = Bf((16, 12), (0, 14));

            private readonly Decoder[] decoders;

            public ExtensionDecoder(params Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                var addr = dasm.rdr.Address;
                if (dasm.extendedConstant.HasValue ||
                    !dasm.rdr.TryReadLeUInt32(out uint uNextInstr))
                    return dasm.CreateInvalidInstruction(dasm.addrPacket);
                dasm.extendedConstant = Bitfield.ReadFields(bfConstant, uInstr) << 6;
                dasm.instrs.Add(new HexagonInstruction(
                    addr,
                    Mnemonic.SIDEEFFECT,
                    new ApplicationOperand(
                        PrimitiveType.Word32,
                        Mnemonic.immext,
                        Constant.Word32(dasm.extendedConstant.Value)))
                {
                    InstructionClass = InstrClass.Linear
                });
                return iclassDecoder.Decode(uNextInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                dasm.NotYetImplemented(message);
                return new HexagonInstruction(dasm.addrPacket, Mnemonic.Invalid, Array.Empty<MachineOperand>())
                {
                    InstructionClass = InstrClass.Invalid,
                    Length = 4,
                };
            }
        }

        private static bool Eq0(uint u) => u == 0;

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }

        private static Decoder Assign(params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, Mnemonic.ASSIGN, mutators);
        }

        public static Decoder Assign(Mutator<HexagonDisassembler> mutatorDst, Mnemonic mnemonic, params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, Mnemonic.ASSIGN,
                mutatorDst,
                Apply(mnemonic, mutators));
        }

        public static Decoder Assign(Mutator<HexagonDisassembler> mutatorDst, Mnemonic mnemonic, PrimitiveType dtResult,  params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, Mnemonic.ASSIGN,
                mutatorDst,
                Apply(mnemonic, dtResult, mutators));
        }

        private static Decoder Duplex(Decoder slot0, Decoder slot1)
        {
            return new DuplexDecoder(slot0, slot1);
        }

        private static Decoder IfJump(params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.ConditionalTransfer, Mnemonic.jump, mutators);
        }

        private static Decoder If(int bitpos, int bitlength, Predicate<uint> predicate, Decoder decoder)
        {
            return new IfDecoder(new Bitfield(bitpos, bitlength), predicate, decoder);
        }

        private static Decoder Mask(int bitpos, int bitlength, string tag, params Decoder[] decoders)
        {
            return new MaskDecoder(new Bitfield(bitpos, bitlength), tag, decoders);
        }

        private static Decoder SideEffect(Mutator<HexagonDisassembler> mutator)
        {
            return new InstrDecoder(InstrClass.Linear, Mnemonic.SIDEEFFECT, mutator);
        }


        private static Decoder Sparse(int bitpos, int bitlength, string tag, Decoder defaultDecoder, params (int, Decoder)[] decoders)
        {
            var decs = new Decoder[1 << bitlength];
            foreach (var (code, decoder) in decoders)
            {
                Debug.Assert(decs[code] is null);
                decs[code] = decoder;
            }
            for (int i = 0; i < decs.Length;++i)
            {
                if (decs[i] is null)
                    decs[i] = defaultDecoder;
            }
            return new MaskDecoder(new Bitfield(bitpos, bitlength), tag, decs);
        }

        public static IfDecoder If(
            Predicate<uint> predicate,
            Decoder decoderTrue)
        {
            var bf = new Bitfield(0, 32, 0xFFFFFFFF);
            return new IfDecoder(bf, predicate, decoderTrue);
        }

        private static Decoder BitFields(Bitfield[] fields, string tag, params Decoder[] decoders)
        {
            return new BitfieldDecoder(fields, tag, decoders);
        }

        private static Decoder Select(Bitfield[] fields, Func<uint, bool> predicate, Decoder decoderTrue, Decoder decoderFalse)
        {
            return new SelectDecoder(fields, predicate, decoderTrue, decoderFalse);
        }

        private static Decoder Seq(params Decoder[] decoders)
        {
            return new SequenceDecoder(decoders);
        }

        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }
        #endregion

        static HexagonDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            subInstrRegs = new RegisterStorage[16]
            {
                Registers.GpRegs[0],
                Registers.GpRegs[1 ],
                Registers.GpRegs[2 ],
                Registers.GpRegs[3 ],
                Registers.GpRegs[4 ],
                Registers.GpRegs[5 ],
                Registers.GpRegs[6 ],
                Registers.GpRegs[7 ],
                Registers.GpRegs[16],
                Registers.GpRegs[17],
                Registers.GpRegs[18],
                Registers.GpRegs[19],
                Registers.GpRegs[20],
                Registers.GpRegs[21],
                Registers.GpRegs[22],
                Registers.GpRegs[23],
            };

            subInstrRegPairs = new (RegisterStorage, RegisterStorage)[8]
            {
                (Registers.GpRegs[1],  Registers.GpRegs[0]),
                (Registers.GpRegs[3],  Registers.GpRegs[2]),
                (Registers.GpRegs[5],  Registers.GpRegs[4]),
                (Registers.GpRegs[7],  Registers.GpRegs[6]),
                (Registers.GpRegs[17], Registers.GpRegs[16]),
                (Registers.GpRegs[19], Registers.GpRegs[18]),
                (Registers.GpRegs[21], Registers.GpRegs[20]),
                (Registers.GpRegs[23], Registers.GpRegs[22]),
            };

            /*
            S1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,0, 0, j,j,j,j,s,s,s,s,t,t,t,t,memw(Rs+#u4:2) = Rt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,0, 1, j,j,j,j,s,s,s,s,t,t,t,t,memb(Rs+#u4:0) = Rt
            */
            var S1 = Mask(12, 1, "  S1",
                Assign(m(PrimitiveType.Word32, 8,4), r0),
                Assign(m(PrimitiveType.Byte, 8,4), r0));

            /* S2
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 0,0, j,j,j,s,s,s,s,t,t,t,t,memh(Rs+#u3:1) = Rt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 0,1, 0,0,j,j,j,j,j,t,t,t,t,memw(r29+#u5:2) = Rt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 0,1, 0,1,j,j,j,j,j,j,t,t,t,memd(r29+#s6:3) = Rtt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 1,0, 0,0,I,s,s,s,s,j,j,j,j,memw(Rs+#u4:2) = #U1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 1,0, 0,1,I,s,s,s,s,j,j,j,j,memb(Rs+#u4) = #U1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1, 1,1, 1,0,j,j,j,j,j,-,-,-,-,allocframe(#u5:3)
            */

            var S2 = Mask(11, 2, "  S2",
                Assign(m(PrimitiveType.Word16, 11, 3), r0),
                Mask(9, 2, "  0b01",
                    Assign(m(PrimitiveType.Word32, Registers.sp, 4, 5), r0),
                    Assign(ms(PrimitiveType.Word64, Registers.sp, 3, 6), rr0),
                    invalid,
                    invalid),
                Mask(9, 2, "  0b10",
                    Assign(m(PrimitiveType.Word32, 0, 4), uw_8L1),
                    Assign(m(PrimitiveType.Byte, 0, 4), uw_8L1),
                    invalid,
                    invalid),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.allocframe, uw4_5_sh3)));


            /* L1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,0, 0,j,j,j,j,s,s,s,s,d,d,d,d,Rd = memw(Rs+#u4:2)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,0, 1,j,j,j,j,s,s,s,s,d,d,d,d,Rd = memub(Rs+#u4:0)
            */

            var L1 = Mask(12, 1, "  L1",
                    Instr(Mnemonic.ASSIGN, r0, mw),
                    Instr(Mnemonic.ASSIGN, r0, mub));
            /*
            L2
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 0,0, j,j,j,s,s,s,s,d,d,d,d,Rd = memh(Rs+#u3:1)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 0,1, j,j,j,s,s,s,s,d,d,d,d,Rd = memuh(Rs+#u3:1)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,0, j,j,j,s,s,s,s,d,d,d,d,Rd = memb(Rs+#u3:0)

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,0,j,j,j,j,j,d,d,d,d,Rd = memw(r29+#u5:2)

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 0, j,j,j,j,j,d,d,d,Rdd = memd(r29+#u5:3)

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,0,-,-,-,0,-,-,deallocframe
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,1,-,-,-,0,-,-,dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,1,-,-,-,1,0,0,if (P0) dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,1,-,-,-,1,0,1,if (!P0) dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,1,-,-,-,1,1,0,if (P0.new) dealloc_return:nt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 0,1,-,-,-,1,1,1,if (!P0.new) dealloc_return:nt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 1,1,-,-,-,0,-,-,jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 1,1,-,-,-,1,0,0,if (P0) jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 1,1,-,-,-,1,0,1,if (!P0) jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 1,1,-,-,-,1,1,0,if (P0.new) jumpr:nt R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1, 1,1, 1,1, 1, 1,1,-,-,-,1,1,1,if (!P0.new) jumpr:nt R31
            */
            var L2 = Mask(11, 2, "  L2",
                Assign(r0, m(PrimitiveType.Int16, 8, 3)),
                Assign(r0, m(PrimitiveType.Word16, 8, 3)),
                Assign(r0, m(PrimitiveType.Byte, 8, 3)),
                Mask(9, 2, "  0b11",
                    invalid,
                    invalid,
                    Assign(r0, Mw_sp_4L5_2),
                    Mask(8, 1, "  0b11",
                        Instr(Mnemonic.ASSIGN, rr0, Md_sp_3L5_3),
                        Mask(6, 2, "  0b1",
                            Instr(Mnemonic.deallocframe),
                            Mask(2, 1, "  0b11",
                                Instr(Mnemonic.dealloc_return, InstrClass.Transfer|InstrClass.Return),
                                Instr(Mnemonic.dealloc_return, InstrClass.ConditionalTransfer | InstrClass.Return,
                Conditional_p0(1, -1, 0))),
                            invalid,
                            Mask(2, 1, "  0b11",
                                Instr(Mnemonic.jumpr, InstrClass.Transfer | InstrClass.Return, Reg(Registers.lr)),
                                Instr(Mnemonic.jumpr, InstrClass.ConditionalTransfer | InstrClass.Return, Reg(Registers.lr),
                Conditional_p0(1, -1, 0)))))));

            /*
            A
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 0,0,j, j,j,j,j,j,j,x,x,x,x,"Rx = add(Rx,#s7)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 0,1,0, j,j,j,j,j,j,d,d,d,d,Rd = #u6
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 0,1,1, j,j,j,j,j,j,d,d,d,d,"Rd = add(r29,#u6:2)"
            *//*
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,0, 0,0,s,s,s,s,d,d,d,d,Rd = Rs
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,0, 0,1,s,s,s,s,d,d,d,d,"Rd = add(Rs,#1)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,0, 1,0,s,s,s,s,d,d,d,d,"Rd = and(Rs,#1)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,0, 1,1,s,s,s,s,d,d,d,d,"Rd = add(Rs,#-1)"
            */
            var A_04 = Mask(10, 2, "  A 04",
                Assign(r0, r4),
                Assign(r0, Apply(Mnemonic.add, r4, Imm_1)),
                Assign(r0, Apply(Mnemonic.and, r4, Imm_1)),
                Assign(r0, Apply(Mnemonic.add, r4, Imm_Minus1)));
            /*
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,1, 0,0,s,s,s,s,d,d,d,d,Rd = sxth(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,1, 0,1,s,s,s,s,d,d,d,d,Rd = sxtb(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,1, 1,0,s,s,s,s,d,d,d,d,Rd = zxth(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,0,1, 1,1,s,s,s,s,d,d,d,d,"Rd = and(Rs,#255)"

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 0,0,s,s,s,s,x,x,x,x,"Rx = add(Rx,Rs)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 0,1,s,s,s,s,-,-,j,j,"P0 = cmp.eq(Rs,#u2)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 1,-,-,0,-,-,d,d,d,d,Rd = #-1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 1,-,-,1,1,0,d,d,d,d,if (P0) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 1,-,-,1,1,1,d,d,d,d,if (!P0) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 1,-,-,1,0,0,d,d,d,d,if (P0.new) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,0, 1,-,-,1,0,1,d,d,d,d,if (!P0.new) Rd = #0

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,1, -,0,-,I,I,j,j,d,d,d,"Rdd = combine(#u2,#U2)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,1, -,1,s,s,s,s,0,d,d,d,"Rdd = combine(#0,Rs)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0, 1,1,1, -,1,s,s,s,s,1,d,d,d,"Rdd = combine(Rs,#0)"
            */
            var A_05 = Mask(8, 2, "  0b101",
                Assign(r0, Mnemonic.sxth, r4),
                Assign(r0, Mnemonic.sxtb, r4),
                Assign(r0, Mnemonic.zxth, r4),
                Assign(r0, Mnemonic.and, r4, Literal(Constant.Word32(0xFF))));

            var A_06 = Mask(9, 1, "  0b110",
                Mask(8, 1, "  0b0",
                    Instr(Mnemonic.ASSIGN, r0, Apply(Mnemonic.add, r0, r4)),
                    Instr(Mnemonic.ASSIGN, P0, Apply(Mnemonic.cmp__eq, r4, uw_0_2))),
                Assign(r0, Imm_Minus1));

            var A_07 = Mask(8, 1, "  0b111",
                Assign(rr0, Apply(Mnemonic.combine, uw_3_2, uw_5L2)),
                Mask(3, 1, "  1",
                    Assign(rr0, Apply(Mnemonic.combine, Imm_0, uw_5L2)),
                    Assign(rr0, Apply(Mnemonic.combine, Imm_0, uw_5L2))));

            var A = Mask(10, 3, "  A",
                Assign(r0, Apply(Mnemonic.add, r0, sw_4L7)),
                Assign(r0, Apply(Mnemonic.add, r0, sw_4L7)),
                Assign(r0, uw_4_6),
                Assign(r0, Apply(Mnemonic.add, Reg(Registers.sp), uimmSh(PrimitiveType.Word32, Bf((4,6)), 2))),

                A_04,
                A_05,
                A_06,
                A_07);

            var duplexDecoder = BitFields(Bf((29, 3), (13, 1)), "  duplex",
                Duplex(L1, L1),
                Duplex(L2, L1),
                Duplex(L2, L2),
                Duplex(A, A),
                Duplex(L1, A),
                Duplex(L2, A),
                Duplex(S1, A),
                Duplex(S2, A),
                Duplex(S1, L1),
                Duplex(S1, L2),
                Duplex(S1, S1),
                Duplex(S2, S1),
                Duplex(S2, L1),
                Duplex(S2, L2),
                Duplex(S2, S2),
                Duplex(invalid, invalid));
            /*
            0,0,0,1, 0,0,0,0, 0,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#U5); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,0, 0,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#U5); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,0, 0,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#U5); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,0, 0,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#U5); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,0, 1,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#U5); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,0, 1,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#U5); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,0, 1,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#U5); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,0, 1,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#U5); if(!p0.new) jump:t #r9:2"
            */
            var decoder_10 = Mask(23, 1, "  10",
                Seq(Assign(P0, Apply(Mnemonic.cmp__eq, R16, uw_7L5)), Instr(Mnemonic.jump, PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))),
                Seq(Assign(P0, Apply(Mnemonic.cmp__gt, R16, uw_7L5)), Instr(Mnemonic.jump, PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))));
            /*
            0,0,0,1, 0,0,0,1, 0,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,#U5); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 0,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,#U5); if(p0.new) jump:t #r9:2"
            
            0,0,0,1, 0,0,0,1, 0,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,#U5); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 0,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,#U5); if(!p0.new) jump:t #r9:2"

            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,0,0,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#-1); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,0,1,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#-1); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,1,1,j,j,j,j,j,j,j,-,"p0=tstbit(Rs,#0); if (p0.new)jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,0,0,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#-1); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,0,1,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#-1); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,1,1,j,j,j,j,j,j,j,-,"p0=tstbit(Rs,#0); if (p0.new)jump:t #r9:2"

            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,0,0,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#-1); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,0,1,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#-1); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,1,1,j,j,j,j,j,j,j,-,"p0=tstbit(Rs,#0); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,0,0,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,#-1); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,0,1,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,#-1); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,0,0,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,1,1,j,j,j,j,j,j,j,-,"p0=tstbit(Rs,#0); if(!p0.new) jump:t #r9:2"
            */
            var decoder_11 = Mask(23, 1, "  0x11",
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__gtu, R16_4, Imm_Minus1)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(14, 22))),
                Mask(7, 2, "  0b10",
                    Seq(
                        Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                        Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(14, 22))),
                    Seq(
                        Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                        Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(14, 22))),
                    Seq(
                        Assign(P0, Apply(Mnemonic.tstbit, R16_4, Imm_0)),
                        Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(14, 22))),
                    invalid));

            /*
            0,0,0,1, 0,0,1,0, 0,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#U5); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,0, 0,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#U5); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,0, 0,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#U5); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,0, 0,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#U5); if(!p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,0, 1,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#U5); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,0, 1,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#U5); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,0, 1,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#U5); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,0, 1,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#U5); if(!p1.new) jump:t #r9:2"
            */
            var decoder_12 = Mask(23, 1, "  0x12...",
                Seq(
                    Assign(P1, Mnemonic.cmp__eq, R16, uw_8L5),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p1_new(14, 22))),
                Seq(
                    Assign(P1, Mnemonic.cmp__gt, R16, uw_8L5),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p1_new(14, 22))));

            /*
            0,0,0,1, 0,0,1,1, 0,0,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,#U5); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 0,0,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,#U5); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 0,1,j,j,s,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,#U5); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 0,1,j,j,s,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,#U5); if(!p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,0,0,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#-1); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,0,1,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#-1); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,0,-,-,-,1,1,j,j,j,j,j,j,j,-,"p1=tstbit(Rs,#0); if (p1.new)jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,0,0,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#-1); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,0,1,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#-1); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 1,0,j,j,s,s,s,s,P,P,1,-,-,-,1,1,j,j,j,j,j,j,j,-,"p1=tstbit(Rs,#0); if (p1.new)jump:t #r9:2"

            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,0,0,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#-1); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,0,1,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#-1); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,0,-,-,-,1,1,j,j,j,j,j,j,j,-,"p1=tstbit(Rs,#0); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,0,0,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,#-1); if(!p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,0,1,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,#-1); if(!p1.new) jump:t #r9:2"
            0,0,0,1, 0,0,1,1, 1,1,j,j,s,s,s,s,P,P,1,-,-,-,1,1,j,j,j,j,j,j,j,-,"p1=tstbit(Rs,#0); if(!p1.new) jump:t #r9:2"
            */
            var p1 = Registers.PredicateRegisters[1];

            var decoder_13 = Mask(22, 2, "  0x13...",
                Nyi("0b00"),
                Nyi("0b01"),
                Nyi("0b10"),
                Mask(8, 2, "  0b11",
                    Nyi("0b00"),
                    Seq(
                        Assign(Reg(p1), Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                        IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(15, 22))),
                    Nyi("0b10"),
                    Nyi("0b11")));
            /*

            0,0,0,1, 0,1,0,0, 0,0,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,Rt); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 0,0,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,Rt); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 0,0,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,Rt); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 0,0,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,Rt); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 0,1,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,Rt); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 0,1,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,Rt); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 0,1,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.eq(Rs,Rt); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 0,1,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.eq(Rs,Rt); if(!p1.new) jump:t #r9:2"

            0,0,0,1, 0,1,0,0, 1,0,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,Rt); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 1,0,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,Rt); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 1,0,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,Rt); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 1,0,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,Rt); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 1,1,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,Rt); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 1,1,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,Rt); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,0, 1,1,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gt(Rs,Rt); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,0, 1,1,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gt(Rs,Rt); if(!p1.new) jump:t #r9:2"
            */
            var decoder_14 = BitFields(Bf((23, 1), (12, 1)), "  0x14...",
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))),
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))),
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))),
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))));
            /*
            0,0,0,1, 0,1,0,1, 0,0,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,Rt); if(p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,1, 0,0,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,Rt); if(p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,1, 0,0,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,Rt); if(p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,1, 0,0,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,Rt); if(p1.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,1, 0,1,j,j,s,s,s,s,P,P,0,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,Rt); if(!p0.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,1, 0,1,j,j,s,s,s,s,P,P,0,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,Rt); if(!p1.new) jump:nt #r9:2"
            0,0,0,1, 0,1,0,1, 0,1,j,j,s,s,s,s,P,P,1,0,t,t,t,t,j,j,j,j,j,j,j,-,"p0=cmp.gtu(Rs,Rt); if(!p0.new) jump:t #r9:2"
            0,0,0,1, 0,1,0,1, 0,1,j,j,s,s,s,s,P,P,1,1,t,t,t,t,j,j,j,j,j,j,j,-,"p1=cmp.gtu(Rs,Rt); if(!p1.new) jump:t #r9:2"
            */
            var decoder_15 = Mask(12, 1, "  0x15...",
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__gtu, R16_4, R8_4)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))),
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__gtu, R16_4, R8_4)),
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))));
            /*
            0,0,0,1, 0,1,1,0, -,-,j,j,d,d,d,d,P,P,I,I,I,I,I,I,j,j,j,j,j,j,j,-,"Rd=#U6 ; jump #r9:2"
            0,0,0,1, 0,1,1,1, -,-,j,j,s,s,s,s,P,P,-,-,d,d,d,d,j,j,j,j,j,j,j,-,"Rd=Rs ; jump #r9:2"
            */

            /*
                0001 - 00000 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.eq(Rs,#U5); if (p0.new) jump:nt #r9:2
                0001 - 00000 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.eq(Rs,#U5); if (p0.new) jump:t #r9:2
            */
            var decoder1_00 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0(28, 13, 22)));
            /*
                0001 - 00001 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.eq(Rs,#U5); if (!p0.new) jump:nt #r9:2
                0001 - 00001 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.eq(Rs,#U5); if (!p0.new) jump:t #r9:2
            */
            var decoder1_01 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22)));
            /*

                0001 - 00010 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.gt(Rs,#U5); if (p0.new) jump:nt #r9:2
                0001 - 00010 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.gt(Rs,#U5); if (p0.new) jump:t #r9:2
            */
            var decoder1_02 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__gt, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22)));
            /*

                0001 - 00011 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.gt(Rs,#U5); if (!p0.new) jump:nt #r9:2
                0001 - 00011 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.gt(Rs,#U5); if (!p0.new) jump:t #r9:2
            */
            var decoder1_03 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__gt, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22)));
            /*

                0001 - 00100 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.gtu(Rs,#U5); if (p0.new) jump:nt #r9:2
                0001 - 00100 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.gtu(Rs,#U5); if (p0.new) jump:t #r9:2
            */
            var decoder1_04 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__gtu, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22)));

            /*
            0001 - 00101 i i ssssPP 0 IIIIIiiiiiii- p0=cmp.gtu(Rs,#U5); if (!p0.new) jump:nt #r9:2
            0001 - 00101 i i ssssPP 1 IIIIIiiiiiii- p0=cmp.gtu(Rs,#U5); if (!p0.new) jump:t #r9:2
            */
            var decoder1_05 = Seq(
                Assign(P0, Apply(Mnemonic.cmp__gtu, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22)));

            /*
            0001 - 00110 i i ssssPP 0 --- 0 0 iiiiiii- p0=cmp.eq(Rs,#-1); if (p0.new) jump:nt #r9:2
            0001 - 00110 i i ssssPP 0 --- 0 1 iiiiiii- p0=cmp.gt(Rs,#-1); if (p0.new) jump:nt #r9:2
            0001 - 00110 i i ssssPP 0 --- 1 1 iiiiiii- p0=tstbit(Rs,#0); if (p0.new) jump:nt #r9:2
            0001 - 00110 i i ssssPP 1 --- 0 0 iiiiiii- p0=cmp.eq(Rs,#-1); if (p0.new) jump:t #r9:2
            0001 - 00110 i i ssssPP 1 --- 0 1 iiiiiii- p0=cmp.gt(Rs,#-1); if (p0.new) jump:t #r9:2
            0001 - 00110 i i ssssPP 1 --- 1 1 iiiiiii- p0=tstbit(Rs,#0); if (p0.new) jump:t #r9:2
            */
            var decoder1_06 = Mask(8, 2, "  06",
                Seq(
                    Assign(P0,Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0(23, 13, -1))),
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0(23, 13, -1))),
                invalid,
                Seq(
                    Assign(P0, Apply(Mnemonic.tstbit, R16_4, Imm_0)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))));
            /*
                0001 - 00111 i i ssssPP 1 --- 0 0 iiiiiii- p0=cmp.eq(Rs,#-1); if (!p0.new) jump:t #r9:2
                0001 - 00111 i i ssssPP 1 --- 0 1 iiiiiii- p0=cmp.gt(Rs,#-1); if (!p0.new) jump:t #r9:2
                0001 - 00111 i i ssssPP 1 --- 1 1 iiiiiii- p0=tstbit(Rs,#0); if (!p0.new) jump:t #r9:2
            */
            var decoder1_07 = Mask(8, 2, "  06",
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0(23, 13, -1))),
                Seq(
                    Assign(P0, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0(23, 13, -1))),
                Nyi("10"),
                Seq(
                    Assign(P0, Apply(Mnemonic.tstbit, R16_4, Imm_0)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p0_new(13, 22))));
            /*

                0001 - 01000 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.eq(Rs,#U5); if (p1.new) jump:nt #r9:2
                0001 - 01000 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.eq(Rs,#U5); if (p1.new) jump:t #r9:2
            */
            var decoder1_08 = Seq(
                Assign(P1, Apply(Mnemonic.cmp__eq, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, -1)));
            /*
                0001 - 01001 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.eq(Rs,#U5); if (!p1.new) jump:nt #r9:2
                0001 - 01001 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.eq(Rs,#U5); if (!p1.new) jump:t #r9:2
            */
            var decoder1_09 = Seq(
                Assign(P1, Apply(Mnemonic.cmp__eq, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22)));
            /*
                0001 - 01010 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.gt(Rs,#U5); if (p1.new) jump:nt #r9:2
                0001 - 01010 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.gt(Rs,#U5); if (p1.new) jump:t #r9:2
            */
            /*

                0001 - 01011 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.gt(Rs,#U5); if (!p1.new) jump:nt #r9:2
                0001 - 01011 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.gt(Rs,#U5); if (!p1.new) jump:t #r9:2
            */
            /*

                0001 - 01100 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.gtu(Rs,#U5); if (p1.new) jump:nt #r9:2
                0001 - 01100 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.gtu(Rs,#U5); if (p1.new) jump:t #r9:2
            */
            var decoder1_0C = Seq(
                Assign(P1, Apply(Mnemonic.cmp__gtu, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, -1)));
            /*

                0001 - 01101 i i ssssPP 0 IIIIIiiiiiii- p1=cmp.gtu(Rs,#U5); if (!p1.new) jump:nt #r9:2
                0001 - 01101 i i ssssPP 1 IIIIIiiiiiii- p1=cmp.gtu(Rs,#U5); if (!p1.new) jump:t #r9:2
            */
            var decoder1_0D = Seq(
                Assign(P1, Apply(Mnemonic.cmp__gtu, R16_4, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22)));
            /*

                0001 - 01110 i i ssssPP 0 --- 0 0 iiiiiii- p1=cmp.eq(Rs,#-1); if (p1.new) jump:nt #r9:2
                0001 - 01110 i i ssssPP 0 --- 0 1 iiiiiii- p1=cmp.gt(Rs,#-1); if (p1.new) jump:nt #r9:2
                0001 - 01110 i i ssssPP 0 --- 1 1 iiiiiii- p1=tstbit(Rs,#0); if (p1.new) jump:nt #r9:2
                0001 - 01110 i i ssssPP 1 --- 0 0 iiiiiii- p1=cmp.eq(Rs,#-1); if (p1.new) jump:t #r9:2
                0001 - 01110 i i ssssPP 1 --- 0 1 iiiiiii- p1=cmp.gt(Rs,#-1); if (p1.new) jump:t #r9:2
                0001 - 01110 i i ssssPP 1 --- 1 1 iiiiiii- p1=tstbit(Rs,#0); if (p1.new) jump:t #r9:2
            */
            var decoder1_0E = Mask(8, 2, "  0E",
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__eq, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))),
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))),
                Nyi("10"),
                Seq(
                    Assign(P1, Apply(Mnemonic.tstbit, R16_4, Imm_0)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))));
            /*

                0001 - 01111 i i ssssPP 0 --- 0 0 iiiiiii- p1=cmp.eq(Rs,#-1); if (!p1.new) jump:nt #r9:2
                0001 - 01111 i i ssssPP 0 --- 0 1 iiiiiii- p1=cmp.gt(Rs,#-1); if (!p1.new) jump:nt #r9:2
                0001 - 01111 i i ssssPP 0 --- 1 1 iiiiiii- p1=tstbit(Rs,#0); if (!p1.new) jump:nt #r9:2
                0001 - 01111 i i ssssPP 1 --- 0 0 iiiiiii- p1=cmp.eq(Rs,#-1); if (!p1.new) jump:t #r9:2
                0001 - 01111 i i ssssPP 1 --- 0 1 iiiiiii- p1=cmp.gt(Rs,#-1); if (!p1.new) jump:t #r9:2
                0001 - 01111 i i ssssPP 1 --- 1 1 iiiiiii- p1=tstbit(Rs,#0); if (!p1.new) jump:t #r9:2
            */
            var decoder1_0F = Mask(8, 2, "  0F",
                Nyi("00"),
                Seq(
                    Assign(P1, Apply(Mnemonic.cmp__gt, R16_4, Imm_Minus1)),
                    IfJump(PcRelExt_20L2_1L7, Conditional_p1_new(13, 22))),
                Nyi("10"),
                Nyi("11"));
            /*


                0001 - 10000 i i ssssPP 0 0 ttttiiiiiii- p0=cmp.eq(Rs,Rt); if (p0.new) jump:nt #r9:2
                0001 - 10000 i i ssssPP 0 1 ttttiiiiiii- p1=cmp.eq(Rs,Rt); if (p1.new) jump:nt #r9:2
                0001 - 10000 i i ssssPP 1 0 ttttiiiiiii- p0=cmp.eq(Rs,Rt); if (p0.new) jump:t #r9:2
                0001 - 10000 i i ssssPP 1 1 tttt iiiiiii- p1=cmp.eq(Rs,Rt); if (p1.new) jump:t #r9:2
            */
            var decoder1_10 = Seq(
                Assign(P_12L1, Apply(Mnemonic.cmp__eq, R16_4, R8_4)),
                IfJump(PcRelExt_20L2_1L7, Conditional_1(12, 28, 13, -1)));
            /*

                0001 - 10001 i i ssssPP 0 0 ttttiiiiiii- p0=cmp.eq(Rs,Rt); if (!p0.new) jump:nt #r9:2
                0001 - 10001 i i ssssPP 0 1 ttttiiiiiii- p1=cmp.eq(Rs,Rt); if (!p1.new) jump:nt #r9:2
                0001 - 10001 i i ssssPP 1 0 ttttiiiiiii- p0=cmp.eq(Rs,Rt); if (!p0.new) jump:t #r9:2
                0001 - 10001 i i ssssPP 1 1 ttttiiiiiii- p1=cmp.eq(Rs,Rt); if (!p1.new) jump:t #r9:2
            */
            var decoder1_11 = Seq(
                Assign(P_12L1, Apply(Mnemonic.cmp__eq, R16_4, R8_4)),
                IfJump(PcRelExt_20L2_1L7, Conditional_1(12, 28, 13, 22)));

            /*

                0001 - 10010 i i ssssPP 0 0 ttttiiiiiii- p0=cmp.gt(Rs,Rt); if (p0.new) jump:nt #r9:2
                0001 - 10010 i i ssssPP 0 1 ttttiiiiiii- p1=cmp.gt(Rs,Rt); if (p1.new) jump:nt #r9:2
                0001 - 10010 i i ssssPP 1 0 ttttiiiiiii- p0=cmp.gt(Rs,Rt); if (p0.new) jump:t #r9:2
                0001 - 10010 i i ssssPP 1 1 ttttiiiiiii- p1=cmp.gt(Rs,Rt); if (p1.new) jump:t #r9:2
            */
            var decoder1_12 = Seq(
                Assign(P_12L1, Apply(Mnemonic.cmp__gt, R16_4, R8_4)),
                IfJump(PcRelExt_20L2_1L7, Conditional_1(12, 23, 13, -1)));

            /*

                0001 - 10011 i i ssssPP 0 0 ttttiiiiiii- p0=cmp.gt(Rs,Rt); if (!p0.new) jump:nt #r9:2
                0001 - 10011 i i ssssPP 0 1 ttttiiiiiii- p1=cmp.gt(Rs,Rt); if (!p1.new) jump:nt #r9:2
                0001 - 10011 i i ssssPP 1 0 ttttiiiiiii- p0=cmp.gt(Rs,Rt); if (!p0.new) jump:t #r9:2
                0001 - 10011 i i ssssPP 1 1 ttttiiiiiii- p1=cmp.gt(Rs,Rt); if (!p1.new) jump:t #r9:2
            */
            var decoder1_13 = Seq(
                Assign(P_12L1, Apply(Mnemonic.cmp__gt, R16_4, R8_4)),
                IfJump(PcRelExt_20L2_1L7, Conditional_1(12, 23, 13, -1)));
            /*

                0001 - 10100 i i ssssPP 0 0 ttttiiiiiii- p0=cmp.gtu(Rs,Rt); if (p0.new) jump:nt #r9:2
                0001 - 10100 i i ssssPP 0 1 ttttiiiiiii- p1=cmp.gtu(Rs,Rt); if (p1.new) jump:nt #r9:2
                0001 - 10100 i i ssssPP 1 0 ttttiiiiiii- p0=cmp.gtu(Rs,Rt); if (p0.new) jump:t #r9:2
            */
            var decoder1_14 = Seq(
                Assign(P_12L1, Apply(Mnemonic.cmp__gtu, R16_4, R8_4)),
                IfJump(PcRelExt_20L2_1L7, Conditional_1(12, 23, 13, -1)));
            /*
                0001 - 110 - - i i ddddPP I I I I I I i i i i i i i - Rd=#U6 ; jump #r9:2
                0001 - 111 - - i i ssssPP - - dddd iiiiiii- Rd=Rs ; jump #r9:2
            */
            var decoder1_18 = Seq(Assign(R8_4, uw_8L6), Instr(Mnemonic.jump, PcRelExt_20L2_1L7));
            var decoder1_1C = Seq(Assign(R8_4, R16_4), Instr(Mnemonic.jump, PcRelExt_20L2_1L7));


            var decoder_1 = Mask(22, 5, "  J 2,3",
                decoder1_00,
                decoder1_01,
                decoder1_02,
                decoder1_03,

                decoder1_04,
                decoder1_05,
                decoder1_06,
                decoder1_07,

                decoder1_08,
                decoder1_09,
                Nyi("0A"),
                Nyi("0B"),

                decoder1_0C,
                decoder1_0D,
                decoder1_0E,
                decoder1_0F,

                decoder1_10,
                decoder1_11,
                decoder1_12,
                decoder1_13,

                decoder1_14,
                invalid,
                invalid,
                invalid,

                decoder1_18,
                decoder1_18,
                decoder1_18,
                decoder1_18,

                decoder1_1C,
                decoder1_1C,
                decoder1_1C,
                decoder1_1C);

            /*
            0,0,1,0, 0,0,0,0, 0,0,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,0, 0,0,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,Rt))jump:t #r9:2"
            0,0,1,0, 0,0,0,0, 0,1,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,0, 0,1,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,Rt))jump:t #r9:2"
            0,0,1,0, 0,0,0,0, 1,0,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,0, 1,0,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,Rt))jump:t #r9:2"
            0,0,1,0, 0,0,0,0, 1,1,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,0, 1,1,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,Rt))jump:t #r9:2"
            */
            var decoder_20 = Mask(23, 1, "  0x20...",
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, R8)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__gt, R8, New16)));
            /*
            0,0,1,0, 0,0,0,1, 0,0,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gtu(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,1, 0,0,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gtu(Ns.new,Rt))jump:t #r9:2"
            0,0,1,0, 0,0,0,1, 0,1,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Ns.new,Rt))jump:nt #r9:2"
            0,0,1,0, 0,0,0,1, 0,1,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Ns.new,Rt))jump:t #r9:2"
            
            0,0,1,0, 0,0,0,1, 1,0,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gt(Rt,Ns.new))jump:nt #r9:2"
            0,0,1,0, 0,0,0,1, 1,0,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gt(Rt,Ns.new))jump:t #r9:2"
            0,0,1,0, 0,0,0,1, 1,1,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gt(Rt,Ns.new))jump:nt #r9:2"
            0,0,1,0, 0,0,0,1, 1,1,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gt(Rt,Ns.new))jump:t #r9:2"
            */
            var decoder_21 = Mask(23, 1, "  0x21...",
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__gtu, New16, R8)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__gtu, R8, New16)));
            /*
            0,0,1,0, 0,0,1,0, 0,0,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gtu(Rt,Ns.new))jump:nt #r9:2"
            0,0,1,0, 0,0,1,0, 0,0,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (cmp.gtu(Rt,Ns.new))jump:t #r9:2"
            0,0,1,0, 0,0,1,0, 0,1,j,j,-,s,s,s,P,P,0,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Rt,Ns.new))jump:nt #r9:2"
            0,0,1,0, 0,0,1,0, 0,1,j,j,-,s,s,s,P,P,1,t,t,t,t,t,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Rt,Ns.new))jump:t #r9:2"
            */
            var decoder_22 = Mask(23, 1, "  0x22...",
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__gtu, R8, New16)),
                invalid);
            /*

            0,0,1,0, 0,1,0,0, 0,0,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,0, 0,0,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,#U5))jump:t #r9:2"
            0,0,1,0, 0,1,0,0, 0,1,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,0, 0,1,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,#U5))jump:t #r9:2"
            0,0,1,0, 0,1,0,0, 1,0,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,0, 1,0,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,#U5))jump:t #r9:2"
            0,0,1,0, 0,1,0,0, 1,1,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,0, 1,1,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,#U5))jump:t #r9:2"
            */
            var decoder_24 = Mask(23, 1, "  0x24...",
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, uw_7L5)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gt, New16, uw_7L5)));

            /*
            0,0,1,0, 0,1,0,1, 0,0,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.gtu(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,1, 0,0,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (cmp.gtu(Ns.new,#U5))jump:t #r9:2"
            0,0,1,0, 0,1,0,1, 0,1,j,j,-,s,s,s,P,P,0,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Ns.new,#U5))jump:nt #r9:2"
            0,0,1,0, 0,1,0,1, 0,1,j,j,-,s,s,s,P,P,1,I,I,I,I,I,j,j,j,j,j,j,j,-,"if (!cmp.gtu(Ns.new,#U5))jump:t #r9:2"
            0,0,1,0, 0,1,0,1, 1,0,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (tstbit(Ns.new,#0))jump:nt #r9:2"
            0,0,1,0, 0,1,0,1, 1,0,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (tstbit(Ns.new,#0)) jump:t#r9:2"
            0,0,1,0, 0,1,0,1, 1,1,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!tstbit(Ns.new,#0))jump:nt #r9:2"
            0,0,1,0, 0,1,0,1, 1,1,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!tstbit(Ns.new,#0)) jump:t#r9:2"
            */
            var decoder_25 = Mask(23, 1, "  0x25...",
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__gtu, New16, uw_7L5)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.tstbit, New16, Imm_0)));

            /*
            0,0,1,0, 0,1,1,0, 0,0,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,#-1))jump:nt #r9:2"
            0,0,1,0, 0,1,1,0, 0,0,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (cmp.eq(Ns.new,#-1))jump:t #r9:2"
            0,0,1,0, 0,1,1,0, 0,1,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,#-1))jump:nt #r9:2"
            0,0,1,0, 0,1,1,0, 0,1,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!cmp.eq(Ns.new,#-1))jump:t #r9:2"
            0,0,1,0, 0,1,1,0, 1,0,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,#-1))jump:nt #r9:2"
            0,0,1,0, 0,1,1,0, 1,0,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (cmp.gt(Ns.new,#-1))jump:t #r9:2"
            0,0,1,0, 0,1,1,0, 1,1,j,j,-,s,s,s,P,P,0,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,#-1))jump:nt #r9:2"
            0,0,1,0, 0,1,1,0, 1,1,j,j,-,s,s,s,P,P,1,-,-,-,-,-,j,j,j,j,j,j,j,-,"if (!cmp.gt(Ns.new,#-1))jump:t #r9:2"
             */
            var decoder_26 = Mask(23, 1, "  0x25...",
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, Imm_Minus1)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_20L2_1L7,
                    ConditionalApply(22, 13, Mnemonic.tstbit, New16, Imm_Minus1)));

            /*
0010 0 0000 0 i i - sssPP 0 tttttii iiiii- if (cmp.eq(Ns.new,Rt)) jump:nt #r9:2
0010 0 0000 0 i i - sssPP 1 tttttii iiiii- if (cmp.eq(Ns.new,Rt)) jump:t #r9:2
0010 0 0000 1 i i - sssPP 0 tttttii iiiii- if (!cmp.eq(Ns.new,Rt)) jump:nt #r9:2
0010 0 0000 1 i i - sssPP 1 tttttii iiiii- if (!cmp.eq(Ns.new,Rt)) jump:t #r9:2
0010 0 0001 0 i i - sssPP 0 tttttii iiiii- if (cmp.gt(Ns.new,Rt)) jump:nt #r9:2
0010 0 0001 0 i i - sssPP 1 tttttii iiiii- if (cmp.gt(Ns.new,Rt)) jump:t #r9:2
0010 0 0001 1 i i - sssPP 0 tttttii iiiii- if (!cmp.gt(Ns.new,Rt)) jump:nt #r9:2
0010 0 0001 1 i i - sssPP 1 tttttii iiiii- if (!cmp.gt(Ns.new,Rt)) jump:t #r9:2
0010 0 0010 0 i i - sssPP 0 tttttii iiiii- if (cmp.gtu(Ns.new,Rt)) jump:nt #r9:2
0010 0 0010 0 i i - sssPP 1 tttttii iiiii- if (cmp.gtu(Ns.new,Rt)) jump:t #r9:2
0010 0 0010 1 i i - sssPP 0 tttttii iiiii- if (!cmp.gtu(Ns.new,Rt)) jump:nt #r9:2
0010 0 0010 1 i i - sssPP 1 tttttii iiiii- if (!cmp.gtu(Ns.new,Rt)) jump:t #r9:2
0010 0 0011 0 i i - sssPP 0 tttttii iiiii- if (cmp.gt(Rt,Ns.new)) jump:nt #r9:2
0010 0 0011 0 i i - sssPP 1 tttttii iiiii- if (cmp.gt(Rt,Ns.new)) jump:t #r9:2
0010 0 0011 1 i i - sssPP 0 tttttii iiiii- if (!cmp.gt(Rt,Ns.new)) jump:nt #r9:2
0010 0 0011 1 i i - sssPP 1 tttttii iiiii- if (!cmp.gt(Rt,Ns.new)) jump:t #r9:2
0010 0 0100 0 i i - sssPP 0 tttttii iiiii- if (cmp.gtu(Rt,Ns.new)) jump:nt #r9:2
0010 0 0100 0 i i - sssPP 1 tttttii iiiii- if (cmp.gtu(Rt,Ns.new)) jump:t #r9:2
0010 0 0100 1 i i - sssPP 0 tttttii iiiii- if (!cmp.gtu(Rt,Ns.new)) jump:nt #r9:2
0010 0 0100 1 i i - sssPP 1 tttttii iiiii- if (!cmp.gtu(Rt,Ns.new)) jump:t #r9:2

0010 0 1000 0 i i - sssPP 0 IIIIIiiiiiii- if (cmp.eq(Ns.new,#U5)) jump:nt #r9:2
0010 0 1000 0 i i - sssPP 1 IIIIIiiiiiii- if (cmp.eq(Ns.new,#U5)) jump:t #r9:2
0010 0 1000 1 i i - sssPP 0 IIIIIiiiiiii- if (!cmp.eq(Ns.new,#U5)) jump:nt #r9:2
0010 0 1000 1 i i - sssPP 1 IIIIIiiiiiii- if (!cmp.eq(Ns.new,#U5)) jump:t #r9:2
0010 0 1001 0 i i - sssPP 0 IIIIIiiiiiii- if (cmp.gt(Ns.new,#U5)) jump:nt #r9:2
0010 0 1001 0 i i - sssPP 1 IIIIIiiiiiii- if (cmp.gt(Ns.new,#U5)) jump:t #r9:2
0010 0 1001 1 i i - sssPP 0 IIIIIiiiiiii- if (!cmp.gt(Ns.new,#U5)) jump:nt #r9:2
0010 0 1001 1 i i - sssPP 1 IIIIIiiiiiii- if (!cmp.gt(Ns.new,#U5)) jump:t #r9:2
0010 0 1010 0 i i - sssPP 0 IIIIIiiiiiii- if (cmp.gtu(Ns.new,#U5)) jump:nt #r9:2
0010 0 1010 0 i i - sssPP 1 IIIIIiiiiiii- if (cmp.gtu(Ns.new,#U5)) jump:t #r9:2
0010 0 1010 1 i i - sssPP 0 IIIIIiiiiiii- if (!cmp.gtu(Ns.new,#U5)) jump:nt #r9:2
0010 0 1010 1 i i - sssPP 1 IIIIIiiiiiii- if (!cmp.gtu(Ns.new,#U5)) jump:t #r9:2
0010 0 1011 0 i i - sssPP 0 -----ii iiiii- if (tstbit(Ns.new,#0)) jump:nt #r9:2
0010 0 1011 0 i i - sssPP 1 -----ii iiiii- if (tstbit(Ns.new,#0)) jump:t #r9:2
0010 0 1011 1 i i - sssPP 0 -----ii iiiii- if (!tstbit(Ns.new,#0)) jump:nt #r9:2
0010 0 1011 1 i i - sssPP 1 -----ii iiiii- if (!tstbit(Ns.new,#0)) jump:t #r9:2
0010 0 1100 0 i i - sssPP 0 -----ii iiiii- if (cmp.eq(Ns.new,#-1)) jump:nt #r9:2
0010 0 1100 0 i i - sssPP 1 -----ii iiiii- if (cmp.eq(Ns.new,#-1)) jump:t #r9:2
0010 0 1100 1 i i - sssPP 0 -----ii iiiii- if (!cmp.eq(Ns.new,#-1)) jump:nt #r9:2
0010 0 1100 1 i i - sssPP 1 -----ii iiiii- if (!cmp.eq(Ns.new,#-1)) jump:t #r9:2
0010 0 1101 0 i i - sssPP 0 -----ii iiiii- if (cmp.gt(Ns.new,#-1)) jump:nt #r9:2
0010 0 1101 0 i i - sssPP 1 -----ii iiiii- if (cmp.gt(Ns.new,#-1)) jump:t #r9:2
0010 0 1101 1 i i - sssPP 0 -----ii iiiii- if (!cmp.gt(Ns.new,#-1)) jump:nt #r9:2
0010 0 1101 1 i i - sssPP 1 -----ii iiiii- if (!cmp.gt(Ns.new,#-1)) jump:t #r9:2


0 0 1 0  0  0 0 0 0  0 i i - s s s P P 0 t t t t t i i i i i i i - if (cmp.eq(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 0 0  0 i i - s s s P P 1 t t t t t i i i i i i i - if (cmp.eq(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 0 0  1 i i - s s s P P 0 t t t t t i i i i i i i - if (!cmp.eq(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 0 0  1 i i - s s s P P 1 t t t t t i i i i i i i - if (!cmp.eq(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 0 1  0 i i - s s s P P 0 t t t t t i i i i i i i - if (cmp.gt(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 0 1  0 i i - s s s P P 1 t t t t t i i i i i i i - if (cmp.gt(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 0 1  1 i i - s s s P P 0 t t t t t i i i i i i i - if (!cmp.gt(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 0 1  1 i i - s s s P P 1 t t t t t i i i i i i i - if (!cmp.gt(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 1 0  0 i i - s s s P P 0 t t t t t i i i i i i i - if (cmp.gtu(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 1 0  0 i i - s s s P P 1 t t t t t i i i i i i i - if (cmp.gtu(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 1 0  1 i i - s s s P P 0 t t t t t i i i i i i i - if (!cmp.gtu(Ns.new,Rt)) jump:nt #r9:2
0 0 1 0  0  0 0 1 0  1 i i - s s s P P 1 t t t t t i i i i i i i - if (!cmp.gtu(Ns.new,Rt)) jump:t #r9:2
0 0 1 0  0  0 0 1 1  0 i i - s s s P P 0 t t t t t i i i i i i i - if (cmp.gt(Rt,Ns.new)) jump:nt #r9:2
0 0 1 0  0  0 0 1 1  0 i i - s s s P P 1 t t t t t i i i i i i i - if (cmp.gt(Rt,Ns.new)) jump:t #r9:2
0 0 1 0  0  0 0 1 1  1 i i - s s s P P 0 t t t t t i i i i i i i - if (!cmp.gt(Rt,Ns.new)) jump:nt #r9:2
0 0 1 0  0  0 0 1 1  1 i i - s s s P P 1 t t t t t i i i i i i i - if (!cmp.gt(Rt,Ns.new)) jump:t #r9:2
0 0 1 0  0  0 1 0 0  0 i i - s s s P P 0 t t t t t i i i i i i i - if (cmp.gtu(Rt,Ns.new)) jump:nt #r9:2
0 0 1 0  0  0 1 0 0  0 i i - s s s P P 1 t t t t t i i i i i i i - if (cmp.gtu(Rt,Ns.new)) jump:t #r9:2
0 0 1 0  0  0 1 0 0  1 i i - s s s P P 0 t t t t t i i i i i i i - if (!cmp.gtu(Rt,Ns.new)) jump:nt #r9:2
0 0 1 0  0  0 1 0 0  1 i i - s s s P P 1 t t t t t i i i i i i i - if (!cmp.gtu(Rt,Ns.new)) jump:t #r9:2

0 0 1 0  0  1 0 0 0  0 i i - s s s P P 0 I I I I I i i i i i i i - if (cmp.eq(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 0 0  0 i i - s s s P P 1 I I I I I i i i i i i i - if (cmp.eq(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 0 0  1 i i - s s s P P 0 I I I I I i i i i i i i - if (!cmp.eq(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 0 0  1 i i - s s s P P 1 I I I I I i i i i i i i - if (!cmp.eq(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 0 1  0 i i - s s s P P 0 I I I I I i i i i i i i - if (cmp.gt(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 0 1  0 i i - s s s P P 1 I I I I I i i i i i i i - if (cmp.gt(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 0 1  1 i i - s s s P P 0 I I I I I i i i i i i i - if (!cmp.gt(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 0 1  1 i i - s s s P P 1 I I I I I i i i i i i i - if (!cmp.gt(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 1 0  0 i i - s s s P P 0 I I I I I i i i i i i i - if (cmp.gtu(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 1 0  0 i i - s s s P P 1 I I I I I i i i i i i i - if (cmp.gtu(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 1 0  1 i i - s s s P P 0 I I I I I i i i i i i i - if (!cmp.gtu(Ns.new,#U5)) jump:nt #r9:2
0 0 1 0  0  1 0 1 0  1 i i - s s s P P 1 I I I I I i i i i i i i - if (!cmp.gtu(Ns.new,#U5)) jump:t #r9:2
0 0 1 0  0  1 0 1 1  0 i i - s s s P P 0 - - - - - i i i i i i i - if (tstbit(Ns.new,#0)) jump:nt #r9:2
0 0 1 0  0  1 0 1 1  0 i i - s s s P P 1 - - - - - i i i i i i i - if (tstbit(Ns.new,#0)) jump:t#r9:2
0 0 1 0  0  1 0 1 1  1 i i - s s s P P 0 - - - - - i i i i i i i - if (!tstbit(Ns.new,#0)) jump:nt #r9:2
0 0 1 0  0  1 0 1 1  1 i i - s s s P P 1 - - - - - i i i i i i i - if (!tstbit(Ns.new,#0)) jump:t#r9:2
0 0 1 0  0  1 1 0 0  0 i i - s s s P P 0 - - - - - i i i i i i i - if (cmp.eq(Ns.new,#-1)) jump:nt #r9:2
0 0 1 0  0  1 1 0 0  0 i i - s s s P P 1 - - - - - i i i i i i i - if (cmp.eq(Ns.new,#-1)) jump:t #r9:2
0 0 1 0  0  1 1 0 0  1 i i - s s s P P 0 - - - - - i i i i i i i - if (!cmp.eq(Ns.new,#-1)) jump:nt #r9:2
0 0 1 0  0  1 1 0 0  1 i i - s s s P P 1 - - - - - i i i i i i i - if (!cmp.eq(Ns.new,#-1)) jump:t #r9:2
0 0 1 0  0  1 1 0 1  0 i i - s s s P P 0 - - - - - i i i i i i i - if (cmp.gt(Ns.new,#-1)) jump:nt #r9:2
0 0 1 0  0  1 1 0 1  0 i i - s s s P P 1 - - - - - i i i i i i i - if (cmp.gt(Ns.new,#-1)) jump:t #r9:2
0 0 1 0  0  1 1 0 1  1 i i - s s s P P 0 - - - - - i i i i i i i - if (!cmp.gt(Ns.new,#-1)) jump:nt #r9:2
0 0 1 0  0  1 1 0 1  1 i i - s s s P P 1 - - - - - i i i i i i i - if (!cmp.gt(Ns.new,#-1)) jump:t #r9:2

            */

            var decoder_2_0 = Mask(23, 4, "  J 2, 3",
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, R8)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gt, New16, R8)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gtu, New16, R8)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gtu, R8, New16)),

                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gtu, R8, New16)),
                Nyi("05"),
                Nyi("06"),
                Nyi("07"),

                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gt, New16, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gtu, New16, uw_8L5)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.tstbit, New16, Imm_0)),

                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__eq, New16, Imm_Minus1)),
                IfJump(PcRelExt_20L2_1L7, ConditionalApply(22, 13, Mnemonic.cmp__gt, New16, Imm_Minus1)),
                Nyi("0E"),
                Nyi("0F"));

            /*

0010 1 IIIuuuueeeeEE 001 iiissssdddd  "Re16 = memuh ( Ru16 + #U3:1 ) ; Rd16 = memuh ( Rs16 + #u3:1 )"

0010 1 IIIuuuueeeeEE 010 iiissssdddd  "Re16 = memuh ( Ru16 + #U3:1 ) ; Rd16 = memb ( Rs16 + #u3:0 )"
            */
            var decoder_2_1_2 = Seq(
                Assign(R16_4, m4(PrimitiveType.Word16, 20, bf_20L3)),
                Assign(R0_4, m4(PrimitiveType.Byte, 4, bf_8L3)));
            /*
0010 1 IIIsssseeeeEE 011 10iiiiidddd  "Re16 = memuh ( Rs16 + #U3:1 ) ; Rd16 = memw ( Sp + #u5:2 )"
0010 1 IIIsssseeeeEE 011 110iiiiiddd  "Re16 = memuh ( Rs16 + #U3:1 ) ; Rdd8 = memd ( Sp + #u5:3 )"
0010 1 iiissssddddEE 011 11100---0--  "Rd16 = memuh ( Rs16 + #u3:1 ) ; deallocframe"
0010 1 iiissssddddEE 011 11101---0--  "Rd16 = memuh ( Rs16 + #u3:1 ) ; dealloc_return"
0010 1 iiissssddddEE 011 11101---101  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( ! p0 ) dealloc_return"
0010 1 iiissssddddEE 011 11101---111  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( ! p0 .new ) dealloc_return:nt"
0010 1 iiissssddddEE 011 11101---100  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( p0 ) dealloc_return"
0010 1 iiissssddddEE 011 11101---110  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( p0 .new ) dealloc_return:nt"
0010 1 iiissssddddEE 011 11111---101  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( ! p0 ) jumpr Lr"
0010 1 iiissssddddEE 011 11111---111  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( ! p0 .new ) jumpr:nt Lr"
0010 1 iiissssddddEE 011 11111---100  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( p0 ) jumpr Lr"
0010 1 iiissssddddEE 011 11111---110  "Rd16 = memuh ( Rs16 + #u3:1 ) ; if ( p0 .new ) jumpr:nt Lr"
0010 1 iiissssddddEE 011 11111---0--  "Rd16 = memuh ( Rs16 + #u3:1 ) ; jumpr Lr"
            */
            var decoder_2_1_3 = Mask(9, 2, "  3",
                invalid,
                invalid,
                Nyi("10"),
                Mask(8, 1, "  0b11",
                    Nyi("  0"),
                    Mask(6, 2, "  1",
                        If(2, 1, Eq0, Seq(
                            Assign(R16_4, m4(PrimitiveType.Word16, 20, bf_24L3)),
                            SideEffect(Apply(Mnemonic.deallocframe)))),
                        Nyi("01"),
                        Nyi("10"),
                        Nyi("11"))));
            /*
0010 1 0IIIIIIeeeeEE 101 0iiiiiidddd  "Re16 = #U6 ; Rd16 = #u6"
0010 1 0IIIIIIeeeeEE 101 1iiiiiidddd  "Re16 = #U6 ; Rd16 = add ( Sp , #u6:2 )"
0010 1 1IIIIIIeeeeEE 101 1iiiiiidddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = add ( Sp , #u6:2 )"

0010 1 0iiiiiieeeeEE 110 000ssssdddd  "Re16 = #u6 ; Rd16 = Rs16"
0010 1 0IIIIIIeeeeEE 110 001ssssdddd  "Re16 = #U6 ; Rd16 = add ( Rs16 , #1 )"
0010 1 0IIIIIIeeeeEE 110 010ssssdddd  "Re16 = #U6 ; Rd16 = and ( Rs16 , #1 )"
0010 1 0IIIIIIeeeeEE 110 011ssssdddd  "Re16 = #U6 ; Rd16 = add ( Rs16 , #-1 )"
0010 1 0iiiiiieeeeEE 110 100ssssdddd  "Re16 = #u6 ; Rd16 = sxth ( Rs16 )"
0010 1 0iiiiiieeeeEE 110 101ssssdddd  "Re16 = #u6 ; Rd16 = sxtb ( Rs16 )"
0010 1 0iiiiiieeeeEE 110 110ssssdddd  "Re16 = #u6 ; Rd16 = zxth ( Rs16 )"
0010 1 0IIIIIIeeeeEE 110 111ssssdddd  "Re16 = #U6 ; Rd16 = and ( Rs16 , #255 )"

0010 1 1iiiiiieeeeEE 110 000ssssdddd  "Re16 = add ( Sp , #u6:2 ) ; Rd16 = Rs16"
0010 1 1IIIIIIeeeeEE 110 001ssssdddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = add ( Rs16 , #1 )"
0010 1 1IIIIIIeeeeEE 110 010ssssdddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = and ( Rs16 , #1 )"
0010 1 1IIIIIIeeeeEE 110 011ssssdddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = add ( Rs16 , #-1 )"
0010 1 1iiiiiieeeeEE 110 100ssssdddd  "Re16 = add ( Sp , #u6:2 ) ; Rd16 = sxth ( Rs16 )"
0010 1 1iiiiiieeeeEE 110 101ssssdddd  "Re16 = add ( Sp , #u6:2 ) ; Rd16 = sxtb ( Rs16 )"
0010 1 1iiiiiieeeeEE 110 110ssssdddd  "Re16 = add ( Sp , #u6:2 ) ; Rd16 = zxth ( Rs16 )"
0010 1 1IIIIIIeeeeEE 110 111ssssdddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = and ( Rs16 , #255 )"
*/
            var decoder_2_1_6 = Mask(26, 1, "  6",
                Mask(8, 3, "  0",
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, R4_4)),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.add, R4_4, Imm_1))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.and, R4_4, Imm_1))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.add, R4_4, Imm_Minus1))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.sxth, R4_4))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.sxtb, R4_4))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.zxth, R4_4))),
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.add, R4_4, Imm_0xFF)))),
                Nyi("1"));
            /*
0010 1 0iiiiiiddddEE 111 000ssssxxxx  "Rd16 = #u6 ; Rx16 = add ( Rs16 , Rx16 )"
0010 1 0iiiiiiddddEE 111 000ssssxxxx  "Rd16 = #u6 ; Rx16 = add ( Rx16 , Rs16 )"
0010 1 0IIIIIIddddEE 111 001ssss--ii  "Rd16 = #U6 ; p0 = cmp.eq ( Rs16 , #u2 )"
0010 1 0IIIIIIeeeeEE 111 01--0--dddd  "Re16 = #U6 ; Rd16 = #-1"
0010 1 0IIIIIIeeeeEE 111 01--111dddd  "Re16 = #U6 ; if ( ! p0 ) Rd16 = #0"
0010 1 0IIIIIIeeeeEE 111 01--101dddd  "Re16 = #U6 ; if ( ! p0 .new ) Rd16 = #0"
0010 1 0IIIIIIeeeeEE 111 01--110dddd  "Re16 = #U6 ; if ( p0 ) Rd16 = #0"
0010 1 0IIIIIIeeeeEE 111 01--100dddd  "Re16 = #U6 ; if ( p0 .new ) Rd16 = #0"
0010 1 0IIIIIIeeeeEE 111 1-0-ii00ddd  "Re16 = #U6 ; Rdd8 = combine ( #0 , #u2 )"
0010 1 0IIIIIIeeeeEE 111 1-1ssss0ddd  "Re16 = #U6 ; Rdd8 = combine ( #0 , Rs16 )"
0010 1 0IIIIIIeeeeEE 111 1-0-ii01ddd  "Re16 = #U6 ; Rdd8 = combine ( #1 , #u2 )"
0010 1 0IIIIIIeeeeEE 111 1-0-ii10ddd  "Re16 = #U6 ; Rdd8 = combine ( #2 , #u2 )"
0010 1 0IIIIIIeeeeEE 111 1-0-ii11ddd  "Re16 = #U6 ; Rdd8 = combine ( #3 , #u2 )"
0010 1 0IIIIIIeeeeEE 111 1-1ssss1ddd  "Re16 = #U6 ; Rdd8 = combine ( Rs16 , #0 )"

0010 1 1iiiiiiddddEE 111 000ssssxxxx  "Rd16 = add ( Sp , #u6:2 ) ; Rx16 = add ( Rs16 , Rx16 )"
0010 1 1iiiiiiddddEE 111 000ssssxxxx  "Rd16 = add ( Sp , #u6:2 ) ; Rx16 = add ( Rx16 , Rs16 )"
0010 1 1IIIIIIddddEE 111 001ssss--ii  "Rd16 = add ( Sp , #U6:2 ) ; p0 = cmp.eq ( Rs16 , #u2 )"
0010 1 1IIIIIIeeeeEE 111 01--0--dddd  "Re16 = add ( Sp , #U6:2 ) ; Rd16 = #-1"
0010 1 1IIIIIIeeeeEE 111 01--111dddd  "Re16 = add ( Sp , #U6:2 ) ; if ( ! p0 ) Rd16 = #0"
0010 1 1IIIIIIeeeeEE 111 01--101dddd  "Re16 = add ( Sp , #U6:2 ) ; if ( ! p0 .new ) Rd16 = #0"
0010 1 1IIIIIIeeeeEE 111 01--110dddd  "Re16 = add ( Sp , #U6:2 ) ; if ( p0 ) Rd16 = #0"
0010 1 1IIIIIIeeeeEE 111 01--100dddd  "Re16 = add ( Sp , #U6:2 ) ; if ( p0 .new ) Rd16 = #0"
0010 1 1IIIIIIeeeeEE 111 1-0-ii00ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( #0 , #u2 )"
0010 1 1IIIIIIeeeeEE 111 1-0-ii01ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( #1 , #u2 )"
0010 1 1IIIIIIeeeeEE 111 1-0-ii10ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( #2 , #u2 )"
0010 1 1IIIIIIeeeeEE 111 1-0-ii11ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( #3 , #u2 )"
0010 1 1IIIIIIeeeeEE 111 1-1ssss0ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( #0 , Rs16 )"
0010 1 1IIIIIIeeeeEE 111 1-1ssss1ddd  "Re16 = add ( Sp , #U6:2 ) ; Rdd8 = combine ( Rs16 , #0 )"

            */
            var decoder_2_1_7 = Mask(26, 1, "  6",
                Mask(8, 3, "  0",
                //Rd16 = #u6 ; Rx16 = add ( Rs16 , Rx16 )"
                    Seq(Assign(R16_4, uw_20L6), Assign(R0_4, Apply(Mnemonic.add, R4_4, R0_4))),
                    Nyi("001"),
                    Nyi("010"),
                    Nyi("011"),
                    Nyi("100"),
                    Nyi("101"),
                    Nyi("110"),
                    Seq(Assign(R16_4, uw_20L6), Assign(rr0, Apply(Mnemonic.combine, R4_4, Imm_0)))),
                Nyi("1"));

            var decoder_2_1 = Mask(13, 3, "  1",
                Nyi("000"),
                Nyi("001"),
                decoder_2_1_2,
                decoder_2_1_3,
                Nyi("100"),
                Nyi("101"),
                decoder_2_1_6,
                decoder_2_1_7);

            var decoder_2 = Mask(27, 1, "  2...", decoder_2_0, decoder_2_1);

            /*
            0,1,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memb(Rs+#u6:0)=Rt"
            0,1,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memh(Rs+#u6:1)=Rt"
            0,1,0,0, 0,0,0,0, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memh(Rs+#u6:1)=Rt.H"
            0,1,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memw(Rs+#u6:2)=Rt"
            0,1,0,0, 0,0,0,0, 1,0,1,s,s,s,s,s,P,P,j,0,0,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memb(Rs+#u6:0)=Nt.new"
            0,1,0,0, 0,0,0,0, 1,0,1,s,s,s,s,s,P,P,j,0,1,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memh(Rs+#u6:1)=Nt.new"
            0,1,0,0, 0,0,0,0, 1,0,1,s,s,s,s,s,P,P,j,1,0,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memw(Rs+#u6:2)=Nt.new"
            0,1,0,0, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv)memd(Rs+#u6:3)=Rtt"
*/
            var decoder_40 = Mask(21, 3, "  0x40...",
                Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Byte, 16, bf_13L1_3L5), R8),
                invalid,
                Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8),
                Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8_H),
                Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word32, 16, bf_13L1_3L5), R8),
                Mask(12, 2, "  0b101",
                    Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Byte, 16, bf_13L1_3L5), New8),
                    Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word16, 16, bf_13L1_3L5), New8),
                    Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word32, 16, bf_13L1_3L5), New8),
                    invalid),
                Assign(Conditional(0, -1, -1, -1), M(PrimitiveType.Word64, 16, bf_13L1_3L5), RR8),
                invalid);

            /*
            0,1,0,0, 0,0,0,1, 0,0,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memb(Rs+#u6:0)"
            0,1,0,0, 0,0,0,1, 0,0,1, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memub(Rs+#u6:0)"
            0,1,0,0, 0,0,0,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memh(Rs+#u6:1)"
            0,1,0,0, 0,0,0,1, 0,1,1, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memuh(Rs+#u6:1)"
            0,1,0,0, 0,0,0,1, 1,0,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memw(Rs+#u6:2)"
            0,1,0,0, 0,0,0,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt)Rdd=memd(Rs+#u6:3)"
            */
            var decoder_41 = Mask(21, 3, "  0x41...",
                Assign(Conditional(11, -1, -1, -1), R0, M(PrimitiveType.SByte, 16, bf_5L6)),
                Assign(Conditional(11, -1, -1, -1), R0, M(PrimitiveType.Byte, 16, bf_5L6)),
                Assign(Conditional(11, -1, -1, -1), R0, M(PrimitiveType.Int16, 16, bf_5L6)),
                Assign(Conditional(11, -1, -1, -1), R0, M(PrimitiveType.Word16, 16, bf_5L6)),
                Assign(Conditional(11, -1, -1, -1), R0, M(PrimitiveType.Word32, 16, bf_5L6)),
                invalid,
                Assign(Conditional(11, -1, -1, -1), RR0, M(PrimitiveType.Word64, 16, bf_5L6)),
                invalid);
            /*
            0,1,0,0, 0,0,1,0, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memb(Rs+#u6:0)=Rt"
            0,1,0,0, 0,0,1,0, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memh(Rs+#u6:1)=Rt"
            0,1,0,0, 0,0,1,0, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memh(Rs+#u6:1)=Rt.H"
            0,1,0,0, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memw(Rs+#u6:2)=Rt"
            0,1,0,0, 0,0,1,0, 1,0,1,s,s,s,s,s,P,P,j,0,0,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memb(Rs+#u6:0)=Nt.new"
            0,1,0,0, 0,0,1,0, 1,0,1,s,s,s,s,s,P,P,j,0,1,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memh(Rs+#u6:1)=Nt.new"
            0,1,0,0, 0,0,1,0, 1,0,1,s,s,s,s,s,P,P,j,1,0,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memw(Rs+#u6:2)=Nt.new"
            0,1,0,0, 0,0,1,0, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (Pv.new)memd(Rs+#u6:3)=Rtt"
            */
            var decoder_42 = Mask(21, 3, "  0x42...",
                Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                invalid,
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8_H, Conditional(0, 25, -1, 24)),
                Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                Mask(11, 2, "  0b101",
                    Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    invalid),
                Assign(M(PrimitiveType.Word64, 16, bf_13L1_3L5), RR8, Conditional(0, 25, -1, 24)),
                invalid);

            /*
            0,1,0,0, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memb(Rs+#u6:0)"
            0,1,0,0, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memub(Rs+#u6:0)"
            0,1,0,0, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memh(Rs+#u6:1)"
            0,1,0,0, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memuh(Rs+#u6:1)"
            0,1,0,0, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memw(Rs+#u6:2)"
            0,1,0,0, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rdd=memd(Rs+#u6:3)"
            */
            var decoder_43 = Mask(21, 3, "  0x43...",
                Assign(R0, M(PrimitiveType.SByte, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                Assign(R0, M(PrimitiveType.Byte, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                Assign(R0, M(PrimitiveType.Int16, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                Assign(R0, M(PrimitiveType.Word16, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                Assign(R0, M(PrimitiveType.Word32, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                invalid,
                Assign(RR0, M(PrimitiveType.Word64, 16, bf_5L6), Conditional(11, 24, -1, 27)),
                invalid);
            /*
            0,1,0,0, 0,1,0,0, 0,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memb(Rs+#u6:0)=Rt"
            0,1,0,0, 0,1,0,0, 0,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memh(Rs+#u6:1)=Rt"
            0,1,0,0, 0,1,0,0, 0,1,1, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memh(Rs+#u6:1)=Rt.H"
            0,1,0,0, 0,1,0,0, 1,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memw(Rs+#u6:2)=Rt"
            0,1,0,0, 0,1,0,0, 1,0,1, s,s,s,s,s,P,P,j,0,0,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memb(Rs+#u6:0)=Nt.new"
            0,1,0,0, 0,1,0,0, 1,0,1, s,s,s,s,s,P,P,j,0,1,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memh(Rs+#u6:1)=Nt.new"
            0,1,0,0, 0,1,0,0, 1,0,1, s,s,s,s,s,P,P,j,1,0,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memw(Rs+#u6:2)=Nt.new"
            0,1,0,0, 0,1,0,0, 1,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv)memd(Rs+#u6:3)=Rtt"
            */
            var decoder_44 = Mask(21, 3, "  0x44...",
                Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), R8, Conditional(0, 24, -1, 26)),
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8, Conditional(0, 24, -1, 26)),
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8_H, Conditional(0, 24, -1, 26)),
                Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), R8_H, Conditional(0, 24, -1, 26)),
                Mask(11, 2, "  0b100",
                    Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), New8, Conditional(0, 24, -1, 26)),
                    Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), New8, Conditional(0, 24, -1, 26)),
                    Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), New8, Conditional(0, 24, -1, 26)),
                    invalid),
                Assign(M(PrimitiveType.Word64, 16, bf_13L1_3L5), RR8, Conditional(0, 24, -1, 26)),
                invalid,
                invalid);
            /*
            0,1,0,0, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memb(Rs+#u6:0)"
            0,1,0,0, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memub(Rs+#u6:0)"
            0,1,0,0, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memh(Rs+#u6:1)"
            0,1,0,0, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memuh(Rs+#u6:1)"
            0,1,0,0, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memw(Rs+#u6:2)"
            0,1,0,0, 0,1,0,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt)Rdd=memd(Rs+#u6:3)"
            */
            var decoder_45 = Mask(21, 3, "  0x45...",
                Assign(R0, M(PrimitiveType.SByte, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Byte, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Int16, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Word16, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Word32, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                invalid,
                Assign(RR0, M(PrimitiveType.Word64, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                invalid);
            /*
            0,1,0,0, 0,1,1,0, 0,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memb(Rs+#u6:0)=Rt"
            0,1,0,0, 0,1,1,0, 0,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memh(Rs+#u6:1)=Rt"
            0,1,0,0, 0,1,1,0, 0,1,1, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memh(Rs+#u6:1)=Rt.H"
            0,1,0,0, 0,1,1,0, 1,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memw(Rs+#u6:2)=Rt"
            0,1,0,0, 0,1,1,0, 1,0,1, s,s,s,s,s,P,P,j,0,0,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memb(Rs+#u6:0)=Nt.new"
            0,1,0,0, 0,1,1,0, 1,0,1, s,s,s,s,s,P,P,j,0,1,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memh(Rs+#u6:1)=Nt.new"
            0,1,0,0, 0,1,1,0, 1,0,1, s,s,s,s,s,P,P,j,1,0,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memw(Rs+#u6:2)=Nt.new"
            0,1,0,0, 0,1,1,0, 1,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,0,v,v,"if (!Pv.new)memd(Rs+#u6:3)=Rtt"
            */
            var decoder_46 = Mask(21, 3, "  0x46...",
                Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                invalid,
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), R8_H, Conditional(0, 25, -1, 24)),
                Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), R8, Conditional(0, 25, -1, 24)),
                Mask(11, 2, "  0b101",
                    Assign(M(PrimitiveType.Byte, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    Assign(M(PrimitiveType.Word16, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    Assign(M(PrimitiveType.Word32, 16, bf_13L1_3L5), New8, Conditional(0, 25, -1, 24)),
                    invalid),
                Assign(M(PrimitiveType.Word64, 16, bf_13L1_3L5), RR8, Conditional(0, 25, -1, 24)),
                invalid);

            /*
            0,1,0,0, 0,1,1,1, 0,0,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memb(Rs+#u6:0)"
            0,1,0,0, 0,1,1,1, 0,0,1, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memub(Rs+#u6:0)"
            0,1,0,0, 0,1,1,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memh(Rs+#u6:1)"
            0,1,0,0, 0,1,1,1, 0,1,1, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memuh(Rs+#u6:1)"
            0,1,0,0, 0,1,1,1, 1,0,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memw(Rs+#u6:2)"
            0,1,0,0, 0,1,1,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,j,j,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rdd=memd(Rs+#u6:3)"
            */
            var decoder_47 = Mask(21, 3, "  0x47...",
                Assign(R0, M(PrimitiveType.SByte, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Byte, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Int16, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Word16, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                Assign(R0, M(PrimitiveType.Word32, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                invalid,
                Assign(RR0, M(PrimitiveType.Word64, 16, bf_5L6), Conditional(11, 25, -1, 24)),
                invalid);

            /*
            0,1,0,0, 1,j,j,0, 0,0,0, j,j,j,j,j,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memb(gp+#u16:0)=Rt"
            0,1,0,0, 1,j,j,0, 0,1,0, j,j,j,j,j,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memh(gp+#u16:1)=Rt"
            0,1,0,0, 1,j,j,0, 0,1,1, j,j,j,j,j,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memh(gp+#u16:1)=Rt.H"
            0,1,0,0, 1,j,j,0, 1,0,0, j,j,j,j,j,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memw(gp+#u16:2)=Rt"
            0,1,0,0, 1,j,j,0, 1,0,1, j,j,j,j,j,P,P,j,0,0,t,t,t,j,j,j,j,j,j,j,j,"memb(gp+#u16:0)=Nt.new"
            0,1,0,0, 1,j,j,0, 1,0,1, j,j,j,j,j,P,P,j,0,1,t,t,t,j,j,j,j,j,j,j,j,"memh(gp+#u16:1)=Nt.new"
            0,1,0,0, 1,j,j,0, 1,0,1, j,j,j,j,j,P,P,j,1,0,t,t,t,j,j,j,j,j,j,j,j,"memw(gp+#u16:2)=Nt.new"
            0,1,0,0, 1,j,j,0, 1,1,0, j,j,j,j,j,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memd(gp+#u16:3)=Rtt"

            0,1,0,0, 1,j,j,1, 0,0,0, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memb(gp+#u16:0)"
            0,1,0,0, 1,j,j,1, 0,0,1, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memub(gp+#u16:0)"
            0,1,0,0, 1,j,j,1, 0,1,0, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memh(gp+#u16:1)"
            0,1,0,0, 1,j,j,1, 0,1,1, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memuh(gp+#u16:1)"
            0,1,0,0, 1,j,j,1, 1,0,0, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memw(gp+#u16:2)"
            0,1,0,0, 1,j,j,1, 1,1,0, j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=memd(gp+#u16:3)"
             */

            var decoder_4_st_gp = Mask(21, 3, "  st gp",
                Assign(Mb_gp_5_13_16_25, R8),
                invalid,
                Assign(Mh_gp_5_13_16_25, R8),
                Assign(Mh_gp_5_13_16_25, R8_H),
                Assign(Mw_gp_5_13_16_25, R8),
                Mask(11, 2, "  0b101",
                    Assign(Mb_gp_5_13_16_25, New8),
                    Assign(Mh_gp_5_13_16_25, New8),
                    Assign(Mw_gp_5_13_16_25, New8),
                    invalid),
                Assign(Md_gp_5_13_16_25, RR8),
                invalid);

            var bf25_16_5 = Bf((25, 2), (16, 5), (5, 9));
            var decoder_4_ld_gp = Mask(21, 3, "  ld gp",
                Assign(R0, Mreg(PrimitiveType.SByte, Registers.gp, bf25_16_5, 0)),
                Assign(R0, Mreg(PrimitiveType.Byte, Registers.gp, bf25_16_5, 0)),
                Assign(R0, Mreg(PrimitiveType.Int16, Registers.gp, bf25_16_5, 1)),
                Assign(R0, Mreg(PrimitiveType.Word16, Registers.gp, bf25_16_5, 1)),
                Assign(R0, Mreg(PrimitiveType.Word32, Registers.gp, bf25_16_5, 2)),
                invalid,
                Assign(RR0, Mreg(PrimitiveType.Word64, Registers.gp, bf25_16_5, 3)),
                invalid);

            var decoder_4 = Mask(24, 4, "  LD ST (conditional or GP - relative) 0, 1",
                decoder_40,
                decoder_41,
                decoder_42,
                decoder_43,
                decoder_44,
                decoder_45,
                decoder_46,
                decoder_47,
                decoder_4_st_gp,
                decoder_4_ld_gp,
                decoder_4_st_gp,
                decoder_4_ld_gp,
                decoder_4_st_gp,
                decoder_4_ld_gp,
                decoder_4_st_gp,
                decoder_4_ld_gp);

            /*
            0,1,0,1, 0,0,0, 0,1,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"callr Rs"
            0,1,0,1, 0,0,0, 1,0,0,0,s,s,s,s,s,P,P,-,-,-,-,u,u,-,-,-,-,-,-,-,-,"if (Pu) callr Rs"
            0,1,0,1, 0,0,0, 1,0,0,1,s,s,s,s,s,P,P,-,-,-,-,u,u,-,-,-,-,-,-,-,-,"if (!Pu) callr Rs"
            */
            var decoder_5_0 = Sparse(21, 4, "  5_0", invalid,
                (0b0101, Instr(Mnemonic.callr, InstrClass.Transfer|InstrClass.Call, R16)),
                (0b1000, Instr(Mnemonic.callr, InstrClass.ConditionalTransfer | InstrClass.Call, R16, Conditional(8, -1, -1, -1))),
                (0b1001, Instr(Mnemonic.callr, InstrClass.ConditionalTransfer | InstrClass.Call, R16, Conditional(8, -1, -1, 21))));

            /*
            0,1,0,1, 0,0,1, 0,1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"jumpr Rs"
            0,1,0,1, 0,0,1, 0,1,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"hintjr(Rs)"
            0,1,0,1, 0,0,1, 1,0,1,0,s,s,s,s,s,P,P,-,0,0,-,u,u,-,-,-,-,-,-,-,-,"if (Pu) jumpr:nt Rs"
            0,1,0,1, 0,0,1, 1,0,1,0,s,s,s,s,s,P,P,-,0,1,-,u,u,-,-,-,-,-,-,-,-,"if (Pu.new) jumpr:nt Rs"
            0,1,0,1, 0,0,1, 1,0,1,0,s,s,s,s,s,P,P,-,1,0,-,u,u,-,-,-,-,-,-,-,-,"if (Pu) jumpr:t Rs"
            0,1,0,1, 0,0,1, 1,0,1,0,s,s,s,s,s,P,P,-,1,1,-,u,u,-,-,-,-,-,-,-,-,"if (Pu.new) jumpr:t Rs"
            0,1,0,1, 0,0,1, 1,0,1,1,s,s,s,s,s,P,P,-,0,0,-,u,u,-,-,-,-,-,-,-,-,"if (!Pu) jumpr:nt Rs"
            0,1,0,1, 0,0,1, 1,0,1,1,s,s,s,s,s,P,P,-,0,1,-,u,u,-,-,-,-,-,-,-,-,"if (!Pu.new) jumpr:nt Rs"
            0,1,0,1, 0,0,1, 1,0,1,1,s,s,s,s,s,P,P,-,1,0,-,u,u,-,-,-,-,-,-,-,-,"if (!Pu) jumpr:t Rs"
            0,1,0,1, 0,0,1, 1,0,1,1,s,s,s,s,s,P,P,-,1,1,-,u,u,-,-,-,-,-,-,-,-,"if (!Pu.new) jumpr:t Rs"
            */
            var decoder_5_1 = Mask(21, 4, "  5_1",
                invalid,
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.jumpr, InstrClass.Transfer, R16),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.hintjr, R16)),
                invalid,
                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.jumpr, InstrClass.ConditionalTransfer, R16, Conditional(8, 11, 12, 21)),
                Instr(Mnemonic.jumpr, InstrClass.ConditionalTransfer, R16, Conditional(8, 11, 12, 21)),
                invalid,
                invalid,
                invalid,
                invalid);
            /*
            0,1,0,1, 0,1,0, 0,0,0,-,-,-,-,-,-,P,P,-,j,j,j,j,j,-,-,-,j,j,j,-,-,"trap0(#u8)"
            0,1,0,1, 0,1,0, 0,0,1,-,-,-,-,-,-,P,P,-,j,j,j,j,j,-,-,-,j,j,j,-,-,"pause(#u8)"
            0,1,0,1, 0,1,0, 0,1,0,-,-,-,-,-,-,P,P,-,j,j,j,j,j,-,-,-,j,j,j,-,-,"trap1(#u8)"
            0,1,0,1, 0,1,0, 1,1,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=icdatar(Rs)"
            0,1,0,1, 0,1,0, 1,1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"ictagw(Rs,Rt)"
            0,1,0,1, 0,1,0, 1,1,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=ictagr(Rs)"
            */
            var decoder_5_2 = Mask(22, 3, "  5_2",
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.trap0, uw_8L5_2L3)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.pause, uw_8L5_2L3)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.trap1, uw_8L5_2L3)),
                invalid,
                invalid,
                invalid,
                Mask(21, 1, "  5_2 6",
                    invalid,
                    Assign(R0, Apply(Mnemonic.icdatar, R16))),
                Mask(21, 1, "  5_2 7",
                    Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.icdtagw, R16, R0)),
                    Assign(R0, Apply(Mnemonic.icdtagr, R16))));
            /*
            0,1,0,1, 0,1,1, 0,1,1,0,s,s,s,s,s,P,P,0,0,0,-,-,-,-,-,-,-,-,-,-,-,"icinva(Rs)"
            0,1,0,1, 0,1,1, 0,1,1,0,s,s,s,s,s,P,P,0,0,1,-,-,-,-,-,-,-,-,-,-,-,"icinvidx(Rs)"
            0,1,0,1, 0,1,1, 0,1,1,0,-,-,-,-,-,P,P,0,1,0,-,-,-,-,-,-,-,-,-,-,-,"ickill"
            0,1,0,1, 0,1,1, 1,1,1,0,0,0,0,0,0,P,P,0,-,-,-,0,0,0,0,0,0,0,0,1,0,"isync"
            0,1,0,1, 0,1,1, 1,1,1,1,-,-,-,-,-,P,P,0,0,-,-,-,-,0,0,0,-,-,-,-,-,"rte"
            0,1,0,1, 0,1,1, 1,1,1,1,-,-,-,-,-,P,P,0,1,-,-,-,-,0,0,0,-,-,-,-,-,"rteunlock"
            */
            var decoder_5_3 = Sparse(21, 4, " 5_3", invalid,
                (0b0110, Sparse(11, 3, "  0b0110", invalid,
                    (0b000, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.icinva, R16))),
                    (0b001, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.icinvidx, R16))),
                    (0b010, Instr(Mnemonic.ickill)))),
                (0b1110, Instr(Mnemonic.isync)),
                (0b1111, Mask(12, 1, "  0b1111",
                    Instr(Mnemonic.rte, InstrClass.Transfer|InstrClass.Return),
                    Instr(Mnemonic.rteunlock, InstrClass.Transfer|InstrClass.Return))));
            /*
            0,1,0,1, 1,0,0, j,j,j,j,j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,j,j,j,j,-,"jump #r22:2"
            0,1,0,1, 1,0,1, j,j,j,j,j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,j,j,j,j,0,"call #r22:2"

            0,1,0,1, 1,1,0, 0,j,j,0,j,j,j,j,j,P,P,j,0,0,-,u,u,j,j,j,j,j,j,j,-,"if (Pu) jump:nt #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,0,j,j,j,j,j,P,P,j,0,1,-,u,u,j,j,j,j,j,j,j,-,"if (Pu.new) jump:nt #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,0,j,j,j,j,j,P,P,j,1,0,-,u,u,j,j,j,j,j,j,j,-,"if (Pu) jump:t #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,0,j,j,j,j,j,P,P,j,1,1,-,u,u,j,j,j,j,j,j,j,-,"if (Pu.new) jump:t #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,1,j,j,j,j,j,P,P,j,0,0,-,u,u,j,j,j,j,j,j,j,-,"if (!Pu) jump:nt #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,1,j,j,j,j,j,P,P,j,0,1,-,u,u,j,j,j,j,j,j,j,-,"if (!Pu.new) jump:nt #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,1,j,j,j,j,j,P,P,j,1,0,-,u,u,j,j,j,j,j,j,j,-,"if (!Pu) jump:t #r15:2"
            0,1,0,1, 1,1,0, 0,j,j,1,j,j,j,j,j,P,P,j,1,1,-,u,u,j,j,j,j,j,j,j,-,"if (!Pu.new) jump:t #r15:2"

            0,1,0,1, 1,1,0, 1,j,j,0,j,j,j,j,j,P,P,j,-,0,-,u,u,j,j,j,j,j,j,j,-,"if (Pu) call #r15:2"
            0,1,0,1, 1,1,0, 1,j,j,1,j,j,j,j,j,P,P,j,-,0,-,u,u,j,j,j,j,j,j,j,-,"if (!Pu) call #r15:2"


            */
            var decoder_5 = Mask(25, 3, "  J 2,3",
                decoder_5_0,
                decoder_5_1,
                decoder_5_2,
                decoder_5_3,

                Instr(Mnemonic.jump, InstrClass.Transfer, PcRelExt_22_2),
                Instr(Mnemonic.call, InstrClass.Transfer| InstrClass.Call, PcRelExt_22_2),
                Mask(24, 1, "  0b110",
                    Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, Conditional(8, 11, 12, 21), PcRelExt_15_2),
                    Instr(Mnemonic.call, InstrClass.ConditionalTransfer | InstrClass.Call, Conditional(8, -1, -1, -1), PcRelExt_15_2)),
                invalid);

            /*
            */

            /*
            0,0,1,1, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rd=memb(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,0, 0,0,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rd=memub(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rd=memh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,0, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rd=memuh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rd=memw(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv)Rdd=memd(Rs+Rt<<#u2)"

            0,0,1,1, 0,0,0,1, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rd=memb(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,1, 0,0,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rd=memub(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,1, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rd=memh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,1, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rd=memuh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,1, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rd=memw(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,0,1, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv)Rdd=memd(Rs+Rt<<#u2)"
            */
            var decoder_31 = Mask(21, 3, "  0x31...",
                Assign(R0, Midx(PrimitiveType.SByte, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Byte, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Int16, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Word16, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Word32, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                invalid,
                Assign(RR0, Midx(PrimitiveType.Word64, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                invalid);
            /*
            0,0,1,1, 0,0,1,0, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rd=memb(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,0, 0,0,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rd=memub(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,0, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rd=memh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,0, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rd=memuh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rd=memw(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,0, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (Pv.new)Rdd=memd(Rs+Rt<<#u2)"
            */
            var decoder_32 = Mask(21, 3, "  0x32...",
                Nyi("0b000"),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Assign(R0, Midx(PrimitiveType.Word32, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                invalid,
                Nyi("0b110"),
                invalid);
            /*
            0,0,1,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rd=memb(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rd=memub(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rd=memh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rd=memuh(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rd=memw(Rs+Rt<<#u2)"
            0,0,1,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,v,v,d,d,d,d,d,"if (!Pv.new)Rdd=memd(Rs+Rt<<#u2)"
            */
            var decoder_33 = Mask(21, 3, "  0x33...",
                Assign(R0, Midx(PrimitiveType.SByte, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Byte, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Int16, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Word16, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                Assign(R0, Midx(PrimitiveType.Word32, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                invalid,
                Assign(RR0, Midx(PrimitiveType.Word64, 16, 8, bf_13L1_7L1), Conditional(5, 25, -1, 24)),
                invalid);
            /*
            0,0,1,1, 0,1,0,0, 0,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv)memb(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,0, 0,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv)memh(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,0, 0,1,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv)memh(Rs+Ru<<#u2)=Rt.H"
            0,0,1,1, 0,1,0,0, 1,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv)memw(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,0,t,t,t,"if (Pv)memb(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,1,t,t,t,"if (Pv)memh(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,1,0,t,t,t,"if (Pv)memw(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,0, 1,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv)memd(Rs+Ru<<#u2)=Rtt"
            0,0,1,1, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv)memb(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv)memh(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv)memh(Rs+Ru<<#u2)=Rt.H"
            0,0,1,1, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv)memw(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,0,t,t,t,"if (!Pv)memb(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,1,t,t,t,"if (!Pv)memh(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,1,0,t,t,t,"if (!Pv)memw(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,0,1, 1,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv)memd(Rs+Ru<<#u2)=Rtt"

            0,0,1,1, 0,1,1,0, 0,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv.new)memb(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,0, 0,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv.new)memh(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,0, 0,1,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv.new)memh(Rs+Ru<<#u2)=Rt.H"
            0,0,1,1, 0,1,1,0, 1,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv.new)memw(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,0,t,t,t,"if (Pv.new)memb(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,1,t,t,t,"if (Pv.new)memh(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,0, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,1,0,t,t,t,"if (Pv.new)memw(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,0, 1,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (Pv.new)memd(Rs+Ru<<#u2)=Rtt"
            */
            var decoder_36 = Mask(21, 3, "  0x36",
                Assign(Midx(PrimitiveType.Byte, 16, 8, bf_13L1_7L1), R0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word16, 16, 8, bf_13L1_7L1), R0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word16, 16, 8, bf_13L1_7L1), R0_H, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word32, 16, 8, bf_13L1_7L1), R0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Byte, 16, 8, bf_13L1_7L1), New0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word16, 16, 8, bf_13L1_7L1), New0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word32, 16, 8, bf_13L1_7L1), New0, Conditional(5, 25, -1, -1)),
                Assign(Midx(PrimitiveType.Word64, 16, 8, bf_13L1_7L1), RR0, Conditional(5, 25, -1, -1)));
            /*
            0,0,1,1, 0,1,1,1, 0,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv.new)memb(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,1, 0,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv.new)memh(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,1, 0,1,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv.new)memh(Rs+Ru<<#u2)=Rt.H"
            0,0,1,1, 0,1,1,1, 1,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv.new)memw(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 0,1,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,0,t,t,t,"if (!Pv.new)memb(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,0,1,t,t,t,"if (!Pv.new)memh(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,1,0,t,t,t,"if (!Pv.new)memw(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,v,v,t,t,t,t,t,"if (!Pv.new)memd(Rs+Ru<<#u2)=Rtt"

            0,0,1,1, 1,0,0,0, 0,0,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv)memb(Rs+#u6:0)=#S6"
            0,0,1,1, 1,0,0,0, 0,0,1,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv)memh(Rs+#u6:1)=#S6"
            0,0,1,1, 1,0,0,0, 0,1,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv)memw(Rs+#u6:2)=#S6"
            0,0,1,1, 1,0,0,0, 1,0,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv)memb(Rs+#u6:0)=#S6"
            0,0,1,1, 1,0,0,0, 1,0,1,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv)memh(Rs+#u6:1)=#S6"
            0,0,1,1, 1,0,0,0, 1,1,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv)memw(Rs+#u6:2)=#S6"
            */
            var decoder_38 = Mask(21, 2, "  0x38...",
                Assign(M(PrimitiveType.Byte, 16, bf_7L6), simm(PrimitiveType.Byte, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                Assign(M(PrimitiveType.Word16, 16, bf_7L6), simm(PrimitiveType.Word16, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                Assign(M(PrimitiveType.Word32, 16, bf_7L6), simm(PrimitiveType.Word32, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                invalid);
            /*
            0,0,1,1, 1,0,0,1, 0,0,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv.new)memb(Rs+#u6:0)=#S6"
            0,0,1,1, 1,0,0,1, 0,0,1,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv.new)memh(Rs+#u6:1)=#S6"
            0,0,1,1, 1,0,0,1, 0,1,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (Pv.new)memw(Rs+#u6:2)=#S6"
            0,0,1,1, 1,0,0,1, 1,0,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv.new)memb(Rs+#u6:0)=#S6"
            0,0,1,1, 1,0,0,1, 1,0,1,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv.new)memh(Rs+#u6:1)=#S6"
            0,0,1,1, 1,0,0,1, 1,1,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,v,v,I,I,I,I,I,"if (!Pv.new)memw(Rs+#u6:2)=#S6"
            */
            var decoder_39 = Mask(21, 2, "  0x39...",
                Assign(M(PrimitiveType.Byte, 16, bf_7L6), simm(PrimitiveType.Byte, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                Assign(M(PrimitiveType.Word16, 16, bf_7L6), simm(PrimitiveType.Word16, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                Assign(M(PrimitiveType.Word32, 16, bf_7L6), simm(PrimitiveType.Word32, bf_13L1_0L5), Conditional(5, 24, -1, 23)),
                invalid);
            /*
            0,0,1,1, 1,0,1,0, 0,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rd=memb(Rs+Rt<<#u2)"
            0,0,1,1, 1,0,1,0, 0,0,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rd=memub(Rs+Rt<<#u2)"
            0,0,1,1, 1,0,1,0, 0,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rd=memh(Rs+Rt<<#u2)"
            0,0,1,1, 1,0,1,0, 0,1,1,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rd=memuh(Rs+Rt<<#u2)"
            0,0,1,1, 1,0,1,0, 1,0,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rd=memw(Rs+Rt<<#u2)"
            0,0,1,1, 1,0,1,0, 1,1,0,s,s,s,s,s,P,P,j,t,t,t,t,t,j,-,-,d,d,d,d,d,"Rdd=memd(Rs+Rt<<#u2)"
            */
            var decoder_3A = Mask(21, 3, "  0x3A...",
                Assign(R0, Midx(PrimitiveType.SByte, 12, 8, bf_13L1_7L1)),
                Assign(R0, Midx(PrimitiveType.Byte, 12, 8, bf_13L1_7L1)),
                Assign(R0, Midx(PrimitiveType.Int16, 12, 8, bf_13L1_7L1)),
                Assign(R0, Midx(PrimitiveType.Word16, 12, 8, bf_13L1_7L1)),
                Assign(R0, Midx(PrimitiveType.Word32, 12, 8, bf_13L1_7L1)),
                invalid,
                Assign(R0, Midx(PrimitiveType.Word64, 12, 8, bf_13L1_7L1)),
                invalid);
            /*
            0,0,1,1, 1,0,1,1, 0,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,t,t,t,t,t,"memb(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 1,0,1,1, 0,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,t,t,t,t,t,"memh(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 1,0,1,1, 0,1,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,t,t,t,t,t,"memh(Rs+Ru<<#u2)=Rt.H"
            0,0,1,1, 1,0,1,1, 1,0,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,t,t,t,t,t,"memw(Rs+Ru<<#u2)=Rt"
            0,0,1,1, 1,0,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,0,0,t,t,t,"memb(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 1,0,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,0,1,t,t,t,"memh(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 1,0,1,1, 1,0,1,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,1,0,t,t,t,"memw(Rs+Ru<<#u2)=Nt.new"
            0,0,1,1, 1,0,1,1, 1,1,0,s,s,s,s,s,P,P,j,u,u,u,u,u,j,-,-,t,t,t,t,t,"memd(Rs+Ru<<#u2)=Rtt"
            */
            var decoder_3B = Mask(21, 3, "  0x3B...",
                Assign(Midx(PrimitiveType.Byte, 12, 8, bf_13L1_7L1), R0),
                invalid,
                Assign(Midx(PrimitiveType.Word16, 12, 8, bf_13L1_7L1), R0),
                Assign(Midx(PrimitiveType.Word16, 12, 8, bf_13L1_7L1), R0_H),
                Assign(Midx(PrimitiveType.Word32, 12, 8, bf_13L1_7L1), R0),
                Mask(3, 2, "  0b101",
                    Assign(Midx(PrimitiveType.Byte, 12, 8, bf_13L1_7L1), New0),
                    Assign(Midx(PrimitiveType.Word16, 12, 8, bf_13L1_7L1), New0),
                    Assign(Midx(PrimitiveType.Word32, 12, 8, bf_13L1_7L1), New0),
                    invalid),
                Assign(Midx(PrimitiveType.Word64, 12, 8, bf_13L1_7L1), RR0),
                invalid);
            /*
            0,0,1,1, 1,1,0,-, -,0,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,I,I,I,I,I,I,I,"memb(Rs+#u6:0)=#S8"
            0,0,1,1, 1,1,0,-, -,0,1,s,s,s,s,s,P,P,I,j,j,j,j,j,j,I,I,I,I,I,I,I,"memh(Rs+#u6:1)=#S8"
            0,0,1,1, 1,1,0,-, -,1,0,s,s,s,s,s,P,P,I,j,j,j,j,j,j,I,I,I,I,I,I,I,"memw(Rs+#u6:2)=#S8"
            */
            var decoder_3C = Mask(21, 2, "  0x3C,0x3D...",
                Assign(M(PrimitiveType.Byte, 16, bf_7L6), sb_0L8),
                Assign(M(PrimitiveType.Word16, 16, bf_7L6), sh_0L8),
                Assign(M(PrimitiveType.Word32, 16, bf_7L6), sw_0L8),
                invalid);
            /*
            0,0,1,1, 1,1,1,0, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,t,t,t,t,t,"memb(Rs+#u6:0)+=Rt"
            0,0,1,1, 1,1,1,0, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,t,t,t,t,t,"memb(Rs+#u6:0)-=Rt"
            0,0,1,1, 1,1,1,0, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,t,t,t,t,t,"memb(Rs+#u6:0)&=Rt"
            0,0,1,1, 1,1,1,0, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,t,t,t,t,t,"memb(Rs+#u6:0)|=Rt"
            0,0,1,1, 1,1,1,0, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,t,t,t,t,t,"memh(Rs+#u6:1)+=Rt"
            0,0,1,1, 1,1,1,0, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,t,t,t,t,t,"memh(Rs+#u6:1)-=Rt"
            0,0,1,1, 1,1,1,0, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,t,t,t,t,t,"memh(Rs+#u6:1)&=Rt"
            0,0,1,1, 1,1,1,0, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,t,t,t,t,t,"memh(Rs+#u6:1)|=Rt"
            0,0,1,1, 1,1,1,0, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,t,t,t,t,t,"memw(Rs+#u6:2)+=Rt"
            0,0,1,1, 1,1,1,0, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,t,t,t,t,t,"memw(Rs+#u6:2)-=Rt"
            0,0,1,1, 1,1,1,0, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,t,t,t,t,t,"memw(Rs+#u6:2)&=Rt"
            0,0,1,1, 1,1,1,0, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,t,t,t,t,t,"memw(Rs+#u6:2)|=Rt"
            */
            var decoder_3E = BitFields(Bf((21, 2), (5,2)), "  3E",
                Instr(Mnemonic.ADDEQ,  M(PrimitiveType.Byte, 16, Bf((7,6))), R0),
                Instr(Mnemonic.SUBEQ, M(PrimitiveType.Byte, 16, Bf((7,6))), R0),
                Instr(Mnemonic.ANDEQ,   M(PrimitiveType.Byte, 16, Bf((7,6))), R0),
                Instr(Mnemonic.OREQ,    M(PrimitiveType.Byte, 16, Bf((7,6))), R0),
                Instr(Mnemonic.ADDEQ,  M(PrimitiveType.Word16, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.SUBEQ, M(PrimitiveType.Word16, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.ANDEQ,   M(PrimitiveType.Word16, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.OREQ,    M(PrimitiveType.Word16, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.ADDEQ,  M(PrimitiveType.Word32, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.SUBEQ, M(PrimitiveType.Word32, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.ANDEQ,   M(PrimitiveType.Word32, 16, Bf((7, 6))), R0),
                Instr(Mnemonic.OREQ,    M(PrimitiveType.Word32, 16, Bf((7, 6))), R0),
                invalid,
                invalid,
                invalid,
                invalid);
            /*
            0,0,1,1, 1,1,1,1, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,I,I,I,I,I,"memb(Rs+#u6:0)+=#U5"
            0,0,1,1, 1,1,1,1, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,I,I,I,I,I,"memb(Rs+#u6:0)-=#U5"
            0,0,1,1, 1,1,1,1, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,I,I,I,I,I,"memb(Rs+#u6:0)=clrbit(#U5)"
            0,0,1,1, 1,1,1,1, -,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,I,I,I,I,I,"memb(Rs+#u6:0)=setbit(#U5)"
            0,0,1,1, 1,1,1,1, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,I,I,I,I,I,"memh(Rs+#u6:1)+=#U5"
            0,0,1,1, 1,1,1,1, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,I,I,I,I,I,"memh(Rs+#u6:1)-=#U5"
            0,0,1,1, 1,1,1,1, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,I,I,I,I,I,"memh(Rs+#u6:1)=clrbit(#U5)"
            0,0,1,1, 1,1,1,1, -,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,I,I,I,I,I,"memh(Rs+#u6:1)=setbit(#U5)"
            0,0,1,1, 1,1,1,1, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,0,I,I,I,I,I,"memw(Rs+#u6:2)+=#U5"
            0,0,1,1, 1,1,1,1, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,0,1,I,I,I,I,I,"memw(Rs+#u6:2)-=#U5"
            0,0,1,1, 1,1,1,1, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,0,I,I,I,I,I,"memw(Rs+#u6:2)=clrbit(#U5)"
            0,0,1,1, 1,1,1,1, -,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,j,1,1,I,I,I,I,I,"memw(Rs+#u6:2)=setbit(#U5)"
            */
            var decoder_3F = BitFields(Bf((21, 2), (5, 2)), "  0x3F...",
                Nyi("0000"),
                Nyi("0001"),
                Nyi("0010"),
                Nyi("0011"),
                Nyi("0100"),
                Nyi("0101"),
                Nyi("0110"),
                Nyi("0111"),
                Instr(Mnemonic.ADDEQ, m(PrimitiveType.Word32, 16, 7, 6), uw_0L5),
                Instr(Mnemonic.SUBEQ, m(PrimitiveType.Word32, 16, 7, 6), uw_0L5),
                Nyi("1010"),
                Nyi("1011"),
                Nyi("1100"),
                Nyi("1101"),
                Nyi("1110"),
                Nyi("1111"));
            var decoder_3 = Mask(24, 4, "  LD ST 0, 1",
                Nyi("0b0000"),
                decoder_31,
                decoder_32,
                decoder_33,
                Nyi("0b0100"),
                Nyi("0b0101"),
                decoder_36,
                Nyi("0b0111"),
                decoder_38,
                decoder_39,
                decoder_3A,
                decoder_3B,
                decoder_3C,
                decoder_3C,
                decoder_3E,
                decoder_3F);

            /*
            0,1,1,0, 0,0,0, 0,0,0, 0,s,s,s,s,s,P,P,-,j,j,j,j,j,-,-,-,j,j,-,-,-,"loop0(#r7:2,Rs)"
            0,1,1,0, 0,0,0, 0,0,0, 1,s,s,s,s,s,P,P,-,j,j,j,j,j,-,-,-,j,j,-,-,-,"loop1(#r7:2,Rs)"
            0,1,1,0, 0,0,0, 0,1,0, 1,s,s,s,s,s,P,P,-,j,j,j,j,j,-,-,-,j,j,-,-,-,"p3=sp1loop0(#r7:2,Rs)"
            0,1,1,0, 0,0,0, 0,1,1, 0,s,s,s,s,s,P,P,-,j,j,j,j,j,-,-,-,j,j,-,-,-,"p3=sp2loop0(#r7:2,Rs)"
            0,1,1,0, 0,0,0, 0,1,1, 1,s,s,s,s,s,P,P,-,j,j,j,j,j,-,-,-,j,j,-,-,-,"p3=sp3loop0(#r7:2,Rs)"
            0,1,1,0, 0,0,0, 1,0,0, j,s,s,s,s,s,P,P,j,0,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs!=#0) jump:nt #r13:2"
            0,1,1,0, 0,0,0, 1,0,0, j,s,s,s,s,s,P,P,j,1,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs!=#0) jump:t #r13:2"
            0,1,1,0, 0,0,0, 1,0,1, j,s,s,s,s,s,P,P,j,0,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs>=#0) jump:nt #r13:2"
            0,1,1,0, 0,0,0, 1,0,1, j,s,s,s,s,s,P,P,j,1,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs>=#0) jump:t #r13:2"
            0,1,1,0, 0,0,0, 1,1,0, j,s,s,s,s,s,P,P,j,0,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs==#0) jump:nt #r13:2"
            0,1,1,0, 0,0,0, 1,1,0, j,s,s,s,s,s,P,P,j,1,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs==#0) jump:t #r13:2"
            0,1,1,0, 0,0,0, 1,1,1, j,s,s,s,s,s,P,P,j,0,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs<=#0) jump:nt #r13:2"
            0,1,1,0, 0,0,0, 1,1,1, j,s,s,s,s,s,P,P,j,1,j,j,j,j,j,j,j,j,j,j,j,-,"if (Rs<=#0) jump:t #r13:2"
            */
            var decoder_6_0 = Mask(22, 3, "  0x60...",
                Mask(21, 1, "  0b0",
                    Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.loop0, PcRelExt_8L5_3L2, R16)),
                    Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.loop1, PcRelExt_8L5_3L2, R16))),
                invalid,
                Assign(P_29L2, Mnemonic.sp1loop0, PcRelExt_8L5_3L2, R16),
                Mask(21, 1, "  0b011",
                    Assign(P_29L2, Mnemonic.sp2loop0, PcRelExt_8L5_3L2, R16),
                    Assign(P_29L2, Mnemonic.sp3loop0, PcRelExt_8L5_3L2, R16)),

                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_1L13_2,
                    ConditionalApply(21, 12, Mnemonic.NE, R16, Imm_0)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_1L13_2,
                    ConditionalApply(21, 12, Mnemonic.GE, R16, Imm_0)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_1L13_2,
                    ConditionalApply(21, 12, Mnemonic.EQ, R16, Imm_0)),
                Instr(Mnemonic.jump, InstrClass.ConditionalTransfer, PcRelExt_1L13_2,
                    ConditionalApply(21, 12, Mnemonic.LE, R16, Imm_0)));
            /*
            0,1,1,0, 0,0,1, 0,0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Gd=Rs"
            0,1,1,0, 0,0,1, 0,0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Cd=Rs"
            0,1,1,0, 0,0,1, 0,0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"trace(Rs)"
            0,1,1,0, 0,0,1, 1,0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Gdd=Rss"
            0,1,1,0, 0,0,1, 1,0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Cdd=Rss"
            */
            var decoder_6_1 = Sparse(21, 4, "  0x6 1...", invalid,
                (0, Assign(G0, R16)),
                (1, Assign(C0, R16)),
                (2, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.trace, R16))),
                (8, Assign(GG0, RR16)),
                (9, Assign(CC0, RR16)));
            /*
            0,1,1,0, 0,1,0, 0,0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,-,-,-,-,-,"swi(Rs)"
            0,1,1,0, 0,1,0, 0,0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,-,-,-,-,-,"cswi(Rs)"
            0,1,1,0, 0,1,0, 0,0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,-,-,-,-,-,"iassignw(Rs)"
            0,1,1,0, 0,1,0, 0,0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,1,-,-,-,-,-,"ciad(Rs)"

            0,1,1,0, 0,1,0, 0,0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,-,-,-,-,-,"wait(Rs)"
            0,1,1,0, 0,1,0, 0,0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,-,-,-,-,-,"resume(Rs)"

            0,1,1,0, 0,1,0, 0,0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,-,-,-,-,-,"stop(Rs)"
            0,1,1,0, 0,1,0, 0,0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,-,-,-,-,-,"start(Rs)"
            0,1,1,0, 0,1,0, 0,0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,-,-,-,-,-,"nmi(Rs)"

            0,1,1,0, 0,1,0, 0,1,0,0, s,s,s,s,s,P,P,-,-,-,-,t,t,0,0,0,-,-,-,-,-,"setimask(Pt,Rs)"
            0,1,1,0, 0,1,0, 0,1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,1,-,-,-,-,-,"siad(Rs)"
            
            0,1,1,0, 0,1,0, 1,0,0,0, x,x,x,x,x,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"crswap(Rx,sgp0)"
            0,1,1,0, 0,1,0, 1,0,0,1, x,x,x,x,x,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"crswap(Rx,sgp1)"
            */
            var decoder_6_2 = Mask(21, 4, "  0x6 2...",
                Sparse(5, 3, "  6 2 0", invalid,
                    (0, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.swi, R16))),
                    (1, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.cswi, R16))),
                    (2, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.iassignw, R16))),
                    (3, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.ciad, R16)))),
                invalid,
                Sparse(5, 3, "  6 2 2", invalid,
                    (0, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.wait, R16))),
                    (1, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.resume, R16)))),
                Sparse(5, 3, "  6 2 3", invalid,
                    (0, Instr(Mnemonic.SIDEEFFECT, InstrClass.Terminates, Apply(Mnemonic.stop, R16))),
                    (1, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.start, R16))),
                    (2, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.nmi, R16)))),
                
                Sparse(5, 3, "  0x6 2 4", invalid,
                    (0, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.setimask, P_8L2, R16))),
                    (3, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.siad, R16)))),
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.crswap, R16, Reg(Registers.sgp0))),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.crswap, R16, Reg(Registers.sgp1))),
                invalid,
                invalid,
                
                invalid,
                invalid,
                invalid,
                invalid);

            /*
            0,1,1,0, 0,1,1, 0,0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=getimask(Rs)"
            0,1,1,0, 0,1,1, 0,0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=iassignr(Rs)"
            0,1,1,0, 0,1,1, 1,0,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,d,d,d,d,d,d,"Sd=Rs"
            */
            var decoder_6_3 = Sparse(21, 4, "  0x6 3...", invalid,
                (0, Assign(R0, Mnemonic.getimask, R16)),
                (3, Assign(R0, Mnemonic.iassignr, R16)),
                (8, Assign(S0, R16)),
                (9, Assign(S0, R16)));
            /*
            0,1,1,0, 1,0,0, 0,0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rdd=Css"
            0,1,1,0, 1,0,0, 0,0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rdd=Gss"
            0,1,1,0, 1,0,0, 1,0,0,0,I,I,I,I,I,P,P,-,j,j,j,j,j,I,I,I,j,j,-,I,I,"loop0(#r7:2,#U10)"
            0,1,1,0, 1,0,0, 1,0,0,1,I,I,I,I,I,P,P,-,j,j,j,j,j,I,I,I,j,j,-,I,I,"loop1(#r7:2,#U10)"
            0,1,1,0, 1,0,0, 1,1,0,1,I,I,I,I,I,P,P,-,j,j,j,j,j,I,I,I,j,j,-,I,I,"p3=sp1loop0(#r7:2,#U10)"
            0,1,1,0, 1,0,0, 1,1,1,0,I,I,I,I,I,P,P,-,j,j,j,j,j,I,I,I,j,j,-,I,I,"p3=sp2loop0(#r7:2,#U10)"
            0,1,1,0, 1,0,0, 1,1,1,1,I,I,I,I,I,P,P,-,j,j,j,j,j,I,I,I,j,j,-,I,I,"p3=sp3loop0(#r7:2,#U10)"
            */
            var decoder_6_4 = Sparse(21, 4, "  0x6 4...", invalid,
                (0, Assign(CC16, RR0)),
                (1, Assign(GG16, RR0)),
                (8, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.loop0, PcRelExt_8L5_3L2, uw_16L5_5L2_0L2))),
                (9, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.loop1, PcRelExt_8L5_3L2, uw_16L5_5L2_0L2))),
                (0xD, Assign(P_29L2, Mnemonic.sp1loop0, PcRelExt_8L5_3L2, uw_16L5_5L2_0L2)),
                (0xE, Assign(P_29L2, Mnemonic.sp2loop0, PcRelExt_8L5_3L2, uw_16L5_5L2_0L2)),
                (0xF, Assign(P_29L2, Mnemonic.sp3loop0, PcRelExt_8L5_3L2, uw_16L5_5L2_0L2)));
            /*
            0,1,1,0, 1,0,1, 0,0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=Cs"
            0,1,1,0, 1,0,1, 0,0,0,1, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=Gs"

            0,1,1,0, 1,0,1, 0,0,1,0, 0,1,0,0,1,P,P,-,j,j,j,j,j,j,-,-,d,d,d,d,d,"Rd=add(pc,#u6)"

            0,1,1,0, 1,0,1, 1,0,0,0, 0,-,-,s,s,P,P,0,-,-,-,t,t,-,-,-,-,-,-,d,d,"Pd=and(Pt,Ps)"
            0,1,1,0, 1,0,1, 1,0,0,0, 0,-,-,s,s,P,P,1,-,-,-,t,t,1,-,-,1,-,-,d,d,"Pd=fastcorner9(Ps,Pt)"
            0,1,1,0, 1,0,1, 1,0,0,0, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=and(Ps,and(Pt,Pu))"
            0,1,1,0, 1,0,1, 1,0,0,0, 1,-,-,s,s,P,P,1,-,-,-,t,t,1,-,-,1,-,-,d,d,"Pd=!fastcorner9(Ps,Pt)"
            0,1,1,0, 1,0,1, 1,0,0,1, 0,-,-,s,s,P,P,0,-,-,-,t,t,-,-,-,-,-,-,d,d,"Pd=or(Pt,Ps)"
            0,1,1,0, 1,0,1, 1,0,0,1, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=and(Ps,or(Pt,Pu))"
            0,1,1,0, 1,0,1, 1,0,1,0, 0,-,-,s,s,P,P,0,-,-,-,t,t,-,-,-,-,-,-,d,d,"Pd=xor(Ps,Pt)"
            0,1,1,0, 1,0,1, 1,0,1,0, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=or(Ps,and(Pt,Pu))"
            0,1,1,0, 1,0,1, 1,0,1,1, 0,-,-,s,s,P,P,0,-,-,-,t,t,-,-,-,-,-,-,d,d,"Pd=and(Pt,!Ps)"
            0,1,1,0, 1,0,1, 1,0,1,1, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=or(Ps,or(Pt,Pu))"
            0,1,1,0, 1,0,1, 1,1,0,0, 0,-,-,s,s,P,P,0,-,-,-,-,-,-,-,-,-,-,-,d,d,"Pd=any8(Ps)"
            0,1,1,0, 1,0,1, 1,1,0,0, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=and(Ps,and(Pt,!Pu))"
            0,1,1,0, 1,0,1, 1,1,0,1, 0,-,-,s,s,P,P,0,-,-,-,-,-,-,-,-,-,-,-,d,d,"Pd=all8(Ps)"
            0,1,1,0, 1,0,1, 1,1,0,1, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=and(Ps,or(Pt,!Pu))"
            0,1,1,0, 1,0,1, 1,1,1,0, 0,-,-,s,s,P,P,0,-,-,-,-,-,-,-,-,-,-,-,d,d,"Pd=not(Ps)"
            0,1,1,0, 1,0,1, 1,1,1,0, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=or(Ps,and(Pt,!Pu))"
            0,1,1,0, 1,0,1, 1,1,1,1, 0,-,-,s,s,P,P,0,-,-,-,t,t,-,-,-,-,-,-,d,d,"Pd=or(Pt,!Ps)"
            0,1,1,0, 1,0,1, 1,1,1,1, 1,-,-,s,s,P,P,0,-,-,-,t,t,u,u,-,-,-,-,d,d,"Pd=or(Ps,or(Pt,!Pu))"
            */
            var decoder_6_5 = Mask(21, 4, "  CR 3 - 0b101",
                Assign(R0, C16),
                Assign(R0, G16),
                Assign(R0, Mnemonic.add, Reg(Registers.pc), uw_7L6),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                
                Mask(20, 1, "  0b1001",
                    Assign(P_0L2, Apply(Mnemonic.fastcorner9, P_16L2, P_8L2)),
                    Assign(P_0L2, Apply(Mnemonic.and, P_16L2, Apply(Mnemonic.and, P_8L2, P_6L2)))),
                Mask(20, 1, "  0b1001",
                    Assign(P_0L2, Apply(Mnemonic.or, P_8L2, P_16L2)),
                    Assign(P_0L2, Apply(Mnemonic.and, P_16L2, Apply(Mnemonic.or, P_16L2, P_6L2)))),
                Mask(20, 1, "  0b1010",
                    Assign(P_0L2, Apply(Mnemonic.xor, P_16L2, P_8L2)),
                    Assign(P_0L2, Apply(Mnemonic.or, P_16L2, Apply(Mnemonic.and, P_8L2, P_6L2)))),
                Mask(20, 1, "  0b1011",
                    Assign(P_0L2, Apply(Mnemonic.and, P_16L2,  InvertIfSet(20, P_8L2))),
                    Assign(P_0L2, Apply(Mnemonic.or, P_16L2, Apply(Mnemonic.or, P_8L2, P_6L2)))),
                Mask(20, 1, "  0b1100",
                    Assign(P_0L2, Mnemonic.any8, P_16L2),
                    Assign(P_0L2, Mnemonic.and, P_16L2, Apply(Mnemonic.and, P_8L2, InvertIfSet(20, P_6L2)))),
                Mask(20, 1, "  0b1101",
                    Assign(P_0L2, Mnemonic.all8, P_16L2),
                    Assign(P_0L2, Mnemonic.and, P_16L2, Apply(Mnemonic.or, P_8L2, InvertIfSet(20, P_6L2)))),
                Mask(20, 1, "  0b1110",
                    Assign(P_0L2, Apply(Mnemonic.not, P_16L2)),
                    Assign(P_0L2, Apply(Mnemonic.or, P_16L2, Apply(Mnemonic.and, P_8L2, InvertIfSet(20, P_6L2))))),
                Mask(20, 1, "  0b1111",
                    Assign(P_0L2, Mnemonic.or, P_8L2, InvertIfSet(21, P_16L2)),
                    Assign(P_0L2, Mnemonic.or, P_16L2, Apply(Mnemonic.or, P_8L2, InvertIfSet(20, P_6L2)))));

            /*
            0,1,1,0, 1,1,0, 0,0,0, 0,s,s,s,s,s,P,P,0,t,t,t,t,t,-,-,-,-,-,-,-,-,"tlbw(Rss,Rt)"
            0,1,1,0, 1,1,0, 0,0,0, 1,-,-,-,-,-,P,P,-,-,-,-,-,-,0,0,0,-,-,-,-,-,"brkpt"
            0,1,1,0, 1,1,0, 0,0,0, 1,-,-,-,-,-,P,P,-,-,-,-,-,-,0,0,1,-,-,-,-,-,"tlblock"
            0,1,1,0, 1,1,0, 0,0,0, 1,-,-,-,-,-,P,P,-,-,-,-,-,-,0,1,1,-,-,-,-,-,"k0lock"
            0,1,1,0, 1,1,0, 0,0,0, 1,-,-,-,-,-,P,P,-,-,-,-,-,-,0,1,0,-,-,-,-,-,"tlbunlock"
            0,1,1,0, 1,1,0, 0,0,0, 1,-,-,-,-,-,P,P,-,-,-,-,-,-,1,0,0,-,-,-,-,-,"k0unlock"

            0,1,1,0, 1,1,0, 0,0,1, 0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rdd=tlbr(Rs)"

            0,1,1,0, 1,1,0, 0,1,0, 0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=tlbp(Rs)"
            0,1,1,0, 1,1,0, 0,1,0, 1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"tlbinvasid(Rs)"

            0,1,1,0, 1,1,0, 0,1,1, 0,s,s,s,s,s,P,P,0,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=ctlbw(Rss,Rt)"
            0,1,1,0, 1,1,0, 0,1,1, 1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=tlboc(Rss)"
            0,1,1,0, 1,1,0, 1,0,0, -,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,d,d,d,d,d,d,"Sdd=Rss"
            0,1,1,0, 1,1,0, 1,1,0, -,x,x,x,x,x,P,P,-,-,-,-,-,-,-,-,-,0,0,0,0,0,"crswap(Rxx,sgp1:0)"
            */
            var decoder_6_6 = Mask(22, 3, "  0x6 6...",
                Mask(21, 1, "   0b000",
                    Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.tlbw, RR16, R8)),
                    Instr(Mnemonic.brkpt)),
                Assign(RR0, Mnemonic.tlbr, R16),
                Mask(21, 1, "  0b010",
                    Assign(R0, Mnemonic.tlbp, R16),
                    Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.tlbinvasid, R16))),
                Mask(21, 1, "  0b011",
                    Assign(R0, Mnemonic.ctlbw, RR16, RR8),
                    Assign(R0, Mnemonic.tlboc, RR16)),
                Assign(SS0, RR16),
                invalid,
                invalid,
                invalid);
            /*
            0,1,1,0, 1,1,1, 0,1,-,s,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=Ss"
            0,1,1,0, 1,1,1, 1,0,-,s,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rdd=Sss"
            */
            var decoder_6 = Mask(25, 3, "  CR 3",
                decoder_6_0,
                decoder_6_1,
                decoder_6_2,
                decoder_6_3,

                decoder_6_4,
                decoder_6_5,
                decoder_6_6,
                Mask(23, 2, "  CR 3 - 0b111",
                    invalid,
                    Assign(R0,S16),
                    Assign(RR0,SS16),
                    invalid));

            /*
            0,1,1,1, 0,0,0,0 ,0,0,0,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=aslh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,0,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=aslh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,0,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=aslh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,0,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=aslh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,0,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=aslh(Rs)"

            0,1,1,1, 0,0,0,0 ,0,0,1,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=asrh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,1,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=asrh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,1,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=asrh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,1,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=asrh(Rs)"
            0,1,1,1, 0,0,0,0 ,0,0,1,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=asrh(Rs)"

            0,1,1,1, 0,0,0,0 ,0,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=Rs"
            
            0,1,1,1, 0,0,0,0 ,1,0,0,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=zxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,0,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=zxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,0,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=zxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,0,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=zxtb(Rs)"

            0,1,1,1, 0,0,0,0 ,1,0,1,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=sxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,1,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=sxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,1,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=sxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,1,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=sxtb(Rs)"
            0,1,1,1, 0,0,0,0 ,1,0,1,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=sxtb(Rs)"

            0,1,1,1, 0,0,0,0 ,1,1,0,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=zxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,0,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=zxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,0,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=zxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,0,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=zxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,0,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=zxth(Rs)"

            0,1,1,1, 0,0,0,0 ,1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=sxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,1,s,s,s,s,s,P,P,1,-,0,0,u,u,-,-,-,d,d,d,d,d,"if (Pu) Rd=sxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,1,s,s,s,s,s,P,P,1,-,0,1,u,u,-,-,-,d,d,d,d,d,"if (Pu.new) Rd=sxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,1,s,s,s,s,s,P,P,1,-,1,0,u,u,-,-,-,d,d,d,d,d,"if (!Pu) Rd=sxth(Rs)"
            0,1,1,1, 0,0,0,0 ,1,1,1,s,s,s,s,s,P,P,1,-,1,1,u,u,-,-,-,d,d,d,d,d,"if (!Pu.new) Rd=sxth(Rs)"
*/
            var decoder_70 = Mask(21, 3, "  0x70...",
                Mask(13, 1, "  0b001",
                    Assign(R0, Apply(Mnemonic.aslh, R16)),
                    Assign(R0, Apply(Mnemonic.aslh, R16), Conditional(8, 10, -1, 11))),
                Mask(13, 1, "  0b001",
                    Assign(R0, Apply(Mnemonic.asrh, R16)),
                    Assign(R0, Apply(Mnemonic.asrh, R16), Conditional(8, 10, -1, 11))),
                invalid,
                Assign(R0, R16),
                Mask(13, 1, "  0b100",
                    Assign(R0, Apply(Mnemonic.zxtb, R16)),
                    Assign(R0, Apply(Mnemonic.zxtb, R16), Conditional(8, 10, -1, 11))),
                Mask(13, 1, "  0b101",
                    Assign(R0, Apply(Mnemonic.sxtb, R16)),
                    Assign(R0, Apply(Mnemonic.sxtb, R16), Conditional(8, 10, -1, 11))),
                Mask(13, 1, "  0b110",
                    Assign(R0, Apply(Mnemonic.zxth, R16)),
                    Assign(R0, Apply(Mnemonic.zxth, R16), Conditional(8, 10, -1, 11))),
                Mask(13, 1, "  0b111",
                    Assign(R0, Apply(Mnemonic.sxth, R16)),
                    Assign(R0, Apply(Mnemonic.sxth, R16), Conditional(8, 10, -1, 11))));
            /*
            0,1,1,1, 0,0,0,1 ,j,j,1,x,x,x,x,x,P,P,j,j,j,j,j,j,j,j,j,j,j,j,j,j,"Rx.L=#u16"
            */
            var decoder_71 = Mask(21, 1, "  0b0001",
                invalid,
                Instr(Mnemonic.ASSIGN, R16_L, uh_0_22));
            /*
            0,1,1,1, 0,0,1,0 ,j,j,1,x,x,x,x,x,P,P,j,j,j,j,j,j,j,j,j,j,j,j,j,j,"Rx.H=#u16"
            */
            var decoder_72 = Mask(21, 1, "  0b0010",
                invalid,
                Instr(Mnemonic.ASSIGN, R16_H, uh_0_22));

            /*
            0,1,1,1, 0,0,1,1 ,0,u,u,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=mux(Pu,Rs,#s8)"
            0,1,1,1, 0,0,1,1 ,1,u,u,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=mux(Pu,#s8,Rs)"
            0,1,1,1, 0,0,1,1 ,-,0,0,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=combine(Rs,#s8)"
            0,1,1,1, 0,0,1,1 ,-,0,1,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=combine(#s8,Rs)"
            0,1,1,1, 0,0,1,1 ,-,1,0,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=cmp.eq(Rs,#s8)"
            0,1,1,1, 0,0,1,1 ,-,1,1,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=!cmp.eq(Rs,#s8)"
            */
            var decoder_73 = Mask(13, 1, "  73",
                Mask(23, 1, "  0",
                    Assign(R0, Apply(Mnemonic.mux, P_21L2, R16, sw_5L8)),
                    Assign(R0, Apply(Mnemonic.mux, P_21L2, sw_5L8, R16))),
                Mask(21, 2, "  1",
                    Assign(RR0, Apply(Mnemonic.combine, R16, sw_5L8)),
                    Assign(RR0, Apply(Mnemonic.combine, sw_5L8, R16)),
                    Assign(R0, Apply(Mnemonic.cmp__eq, R16, sw_5L8)),
                    Assign(R0, InvertIfSet(21, Apply(Mnemonic.cmp__eq, R16, sw_5L8)))));
            /*
            0,1,1,1, 0,1,0,0 ,0,u,u,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (Pu) Rd=add(Rs,#s8)"
            0,1,1,1, 0,1,0,0 ,0,u,u,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (Pu.new) Rd=add(Rs,#s8)"
            0,1,1,1, 0,1,0,0 ,1,u,u,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (!Pu) Rd=add(Rs,#s8)"
            0,1,1,1, 0,1,0,0 ,1,u,u,s,s,s,s,s,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (!Pu.new)Rd=add(Rs,#s8)"
            */
            var decoder_74 = Assign(R0, Apply(Mnemonic.add, R16, sw_5L8, Conditional(21, 13, -1, 23)));
            /*
            0,1,1,1, 0,1,0,1 ,0,0, j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,0,0,0,d,d,"Pd=cmp.eq(Rs,#s10)"
            0,1,1,1, 0,1,0,1 ,0,0, j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,1,0,0,d,d,"Pd=!cmp.eq(Rs,#s10)"
            0,1,1,1, 0,1,0,1 ,0,1, j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,0,0,0,d,d,"Pd=cmp.gt(Rs,#s10)"
            0,1,1,1, 0,1,0,1 ,0,1, j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,1,0,0,d,d,"Pd=!cmp.gt(Rs,#s10)"
            0,1,1,1, 0,1,0,1 ,1,0, 0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,0,0,0,d,d,"Pd=cmp.gtu(Rs,#u9)"
            0,1,1,1, 0,1,0,1 ,1,0, 0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,1,0,0,d,d,"Pd=!cmp.gtu(Rs,#u9)"
            */
            var decoder_75 = Mask(22, 2, "  0b0101",
                Instr(Mnemonic.ASSIGN, P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__eq, R16, sw5_10))),
                Instr(Mnemonic.ASSIGN, P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__gt, R16, sw5_10))),
                Instr(Mnemonic.ASSIGN, P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__gtu, R16, uw_5L9))),
                invalid);

            /*
            0,1,1,1, 0,1,1,0 ,0,0,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=and(Rs,#s10)"
            0,1,1,1, 0,1,1,0 ,0,1,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=sub(#s10,Rs)"
            0,1,1,1, 0,1,1,0 ,1,0,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=or(Rs,#s10)"
            */
            var decoder_76 = Mask(22, 2, "  0b0110",
                Assign(R0, Apply(Mnemonic.and, R16, sw_21L1_5L9)),
                Assign(R0, Apply(Mnemonic.sub, sw_21L1_5L9, R16)),
                Assign(R0, Apply(Mnemonic.or, R16, sw_21L1_5L9)),
                invalid);

            /*
            0,1,1,1, 1,0,0,0 ,j,j,-,j,j,j,j,j,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=#s16"
            */
            var decoder_78 = Assign(R0, sw5_16_22);
            /*
            0,1,1,1, 1,0,1,u ,u,I,I,I,I,I,I,I,P,P,I,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=mux(Pu,#s8,#S8)"
            */
            var decoder7_mux = Assign(R0, Apply(Mnemonic.mux, P_23L2, sw_5L8, sw16_13));

            /*
            0,1,1,1, 1,1,0,0 ,0,I,I,I,I,I,I,I,P,P,I,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=combine(#s8,#S8)"
            0,1,1,1, 1,1,0,0 ,1,-,-,I,I,I,I,I,P,P,I,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=combine(#s8,#U6)"
            */
            var decoder_7C = Mask(23, 1, "  0b1100",
                Assign(RR0, Apply(Mnemonic.combine, sw_5L8, sw_16L7_13L1)),
                Assign(RR0, Apply(Mnemonic.combine, sw_5L8, uw_16L5_13L1)));

            /*
            0,1,1,1, 1,1,1,0 ,0,u,u,0,j,j,j,j,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (Pu) Rd=#s12"
            0,1,1,1, 1,1,1,0 ,0,u,u,0,j,j,j,j,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (Pu.new) Rd=#s12"
            0,1,1,1, 1,1,1,0 ,1,u,u,0,j,j,j,j,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (!Pu) Rd=#s12"
            0,1,1,1, 1,1,1,0 ,1,u,u,0,j,j,j,j,P,P,1,j,j,j,j,j,j,j,j,d,d,d,d,d,"if (!Pu.new) Rd=#s12"
            */
            var decoder_7E = Assign(R0, sw_16L4_5L8, Conditional(21, 13, -1, 23));
            /*
            0,1,1,1, 1,1,1,1 ,-,-,-,-,-,-,-,-,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"nop"
            */
            var decoder_7 = Mask(24, 4, "  ALU32 0, 1, 2, 3",
                decoder_70,
                decoder_71,
                decoder_72,
                decoder_73,

                decoder_74,
                decoder_75,
                decoder_76,
                invalid,

                decoder_78,
                invalid,
                decoder7_mux,
                decoder7_mux,
                decoder_7C,
                invalid,
                decoder_7E,
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding));

            /*
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,0,0,0,d,d,d,d,d,"Rdd=asr(Rss,#u6)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,0,0,1,d,d,d,d,d,"Rdd=lsr(Rss,#u6)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rdd=asl(Rss,#u6)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,0,1,1,d,d,d,d,d,"Rdd=rol(Rss,#u6)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rdd=vsathub(Rss)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rdd=vsatwuh(Rss)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rdd=vsatwh(Rss)"
            1,0,0,0, 0,0,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rdd=vsathb(Rss)"

            1,0,0,0, 0,0,0,0, 0,0,1,s,s,s,s,s,P,P,0,0,j,j,j,j,0,0,0,d,d,d,d,d,"Rdd=vasrh(Rss,#u4):raw"
            */
            /*
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j, 0,0,0, d,d,d,d,d,"Rdd=vasrw(Rss,#u5)"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j, 0,0,1, d,d,d,d,d,"Rdd=vlsrw(Rss,#u5)"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j, 0,1,0, d,d,d,d,d,"Rdd=vaslw(Rss,#u5)"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-, 1,0,0, d,d,d,d,d,"Rdd=vabsh(Rss)"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-, 1,0,1, d,d,d,d,d,"Rdd=vabsh(Rss):sat"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-, 1,1,0, d,d,d,d,d,"Rdd=vabsw(Rss)"
            1,0,0,0, 0,0,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-, 1,1,1, d,d,d,d,d,"Rdd=vabsw(Rss):sat"
            */
            var decoder_80_2 = Mask(5, 3, "  0x80 2...",
                Assign(RR0, Mnemonic.vasrw, RR16, uw_8L5),
                Assign(RR0, Mnemonic.vlsrw, RR16, uw_8L5),
                Assign(RR0, Mnemonic.vaslw, RR16, uw_8L5),
                invalid,
                Assign(RR0, Mnemonic.vabsh, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vabsh, RR16))),
                Assign(RR0, Mnemonic.vabsw, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vabsw, RR16))));

            /*
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,0,0,j,j,j,j,0,0,0,d,d,d,d,d,"Rdd=vasrh(Rss,#u4)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,0,0,j,j,j,j,0,0,1,d,d,d,d,d,"Rdd=vlsrh(Rss,#u4)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,0,0,j,j,j,j,0,1,0,d,d,d,d,d,"Rdd=vaslh(Rss,#u4)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rdd=not(Rss)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rdd=neg(Rss)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rdd=abs(Rss)"
            1,0,0,0, 0,0,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rdd=vconj(Rss):sat"
            */
            var decoder_80_4 = Mask(5, 3, "0b100",
                Assign(RR0, Mnemonic.vasrh, RR16, uw_7L4),
                Assign(RR0, Mnemonic.vlsrh, RR16, uw_7L4),
                Assign(RR0, Mnemonic.vaslh, RR16, uw_7L4),
                invalid,
                Assign(RR0, Mnemonic.not, RR16),
                Assign(RR0, Mnemonic.neg, RR16),
                Assign(RR0, Mnemonic.abs, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vconj, RR16))));

            /*
            1,0,0,0, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rdd=deinterleave(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rdd=interleave(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rdd=brev(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,0,s,s,s,s,s,P,P,j,j,j,j,j,j,1,1,1,d,d,d,d,d,"Rdd=asr(Rss,#u6):rnd"

            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rdd=convert_df2d(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rdd=convert_df2ud(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rdd=convert_ud2df(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,0,1,1,d,d,d,d,d,"Rdd=convert_d2df(Rss)"
            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rdd=convert_df2d(Rss):chop"
            1,0,0,0, 0,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rdd=convert_df2ud(Rss):chop"
            */
            var decoder_80 = Mask(21, 3, "  0x80...",
                Mask(5, 3, "  0b000",
                    Assign(RR0, Apply(Mnemonic.asr, RR16, uw_8L6)),
                    Assign(RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)),
                    Assign(RR0, Apply(Mnemonic.asl, RR16, uw_8L6)),
                    Assign(RR0, Apply(Mnemonic.rol, RR16, uw_8L6)),
                    Assign(RR0, Apply(Mnemonic.vsathub, RR16)),
                    Assign(RR0, Apply(Mnemonic.vsathuw, RR16)),
                    Assign(RR0, Apply(Mnemonic.vsathw, RR16)),
                    Assign(RR0, Apply(Mnemonic.vsathb, RR16))),
                Nyi("0b001"),
                decoder_80_2,
                Nyi("0b011"),
                decoder_80_4,
                Nyi("0b101"),
                Nyi("0b110"),
                Mask(5, 3, "  64-bit conversions",
                    Assign(RR0, Apply(Mnemonic.convert_df2d, RR16)),
                    Assign(RR0, Apply(Mnemonic.convert_df2ud, RR16)),
                    Assign(RR0, Apply(Mnemonic.convert_ud2df, RR16)),
                    Assign(RR0, Apply(Mnemonic.convert_d2df, RR16)),
                    invalid,
                    invalid,
                    Assign(RR0, Chop(Apply(Mnemonic.convert_df2d, RR16))),
                    Assign(RR0, Chop(Apply(Mnemonic.convert_df2ud, RR16)))));

            /*
            1,0,0,0, 0,0,0,1, I,I,I,s,s,s,s,s,P,P,j,j,j,j,j,j,I,I,I,d,d,d,d,d,"Rdd=extractu(Rss,#u6,#U6)"
            */
            var decoder_81 = Assign(RR0, Apply(Mnemonic.extractu, RR16, uw_8L6, uw_21L3_5L3));
            /*
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,0,0, x,x,x,x,x,"Rxx-=asr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,0,1, x,x,x,x,x,"Rxx-=lsr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,0, x,x,x,x,x,"Rxx-=asl(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,1, x,x,x,x,x,"Rxx-=rol(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,0,0, x,x,x,x,x,"Rxx+=asr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,0,1, x,x,x,x,x,"Rxx+=lsr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,1,0, x,x,x,x,x,"Rxx+=asl(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,1,1, x,x,x,x,x,"Rxx+=rol(Rss,#u6)"

            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,0,0, x,x,x,x,x,"Rxx&=asr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,0,1, x,x,x,x,x,"Rxx&=lsr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,0, x,x,x,x,x,"Rxx&=asl(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,1, x,x,x,x,x,"Rxx&=rol(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,0,0, x,x,x,x,x,"Rxx|=asr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,0,1, x,x,x,x,x,"Rxx|=lsr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,1,0, x,x,x,x,x,"Rxx|=asl(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 0,1,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 1,1,1, x,x,x,x,x,"Rxx|=rol(Rss,#u6)"

            1,0,0,0, 0,0,1,0, 1,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,0,1, x,x,x,x,x,"Rxx^=lsr(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 1,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,0, x,x,x,x,x,"Rxx^=asl(Rss,#u6)"
            1,0,0,0, 0,0,1,0, 1,0,-,s,s,s,s,s,P,P,j,j,j,j,j,j, 0,1,1, x,x,x,x,x,"Rxx^=rol(Rss,#u6)"
            */
            var decoder_82 = Mask(22, 2, "  0x82...",
                Mask(5, 3,  "  0b00",
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.asr, RR16, uw_8L6)), 
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)), 
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.asl, RR16, uw_8L6)), 
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.rol, RR16, uw_8L6)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.asr, RR16, uw_8L6)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.asl, RR16, uw_8L6)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.rol, RR16, uw_8L6))),
                Mask(5, 3, "  0b01",
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.asr, RR16, uw_8L6)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.asl, RR16, uw_8L6)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.rol, RR16, uw_8L6)),
                    Instr(Mnemonic.OREQ, RR0, Apply(Mnemonic.asr, RR16, uw_8L6)),
                    Instr(Mnemonic.OREQ, RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)),
                    Instr(Mnemonic.OREQ, RR0, Apply(Mnemonic.asl, RR16, uw_8L6)),
                    Instr(Mnemonic.OREQ, RR0, Apply(Mnemonic.rol, RR16, uw_8L6))),
                Mask(5, 3, "  0b10",
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.asr, RR16, uw_8L6)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.lsr, RR16, uw_8L6)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.asl, RR16, uw_8L6)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.rol, RR16, uw_8L6)),
                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid);
            /*
            1,0,0,0, 0,0,1,1, I,I,I,s,s,s,s,s,P,P,j,j,j,j,j,j,I,I,I,x,x,x,x,x,"Rxx=insert(Rss,#u6,#U6)"
               */
            var decoder_83 = Assign(RR0, Apply(Mnemonic.insert, RR16, uw_8L6, uw_21L3_5L3));
            /*
            1,0,0,0, 0,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,-,d,d,d,d,d,"Rdd=vsxtbh(Rs)"
            1,0,0,0, 0,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,-,d,d,d,d,d,"Rdd=vzxtbh(Rs)"
            1,0,0,0, 0,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,-,d,d,d,d,d,"Rdd=vsxthw(Rs)"
            1,0,0,0, 0,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,-,d,d,d,d,d,"Rdd=vzxthw(Rs)"
            1,0,0,0, 0,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,-,d,d,d,d,d,"Rdd=sxtw(Rs)"
            1,0,0,0, 0,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,-,d,d,d,d,d,"Rdd=vsplath(Rs)"
            1,0,0,0, 0,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,-,d,d,d,d,d,"Rdd=vsplatb(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rdd=convert_sf2df(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rdd=convert_uw2df(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rdd=convert_w2df(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,1,d,d,d,d,d,"Rdd=convert_sf2ud(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rdd=convert_sf2d(Rs)"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rdd=convert_sf2ud(Rs):chop"
            1,0,0,0, 0,1,0,0, 1,-,-,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rdd=convert_sf2d(Rs):chop"
            */
            var decoder_84 = Mask(23, 1, "  0x84...",
                Mask(22, 1, "  0",
                    Mask(6, 2, "  0",
                        Assign(RR0, Mnemonic.vsxtbh, R16),
                        Assign(RR0, Mnemonic.vzxtbh, R16),
                        Assign(RR0, Mnemonic.vsxthw, R16),
                        Assign(RR0, Mnemonic.vzxthw, R16)),
                    Mask(6, 2, "  1",
                        Assign(RR0, Mnemonic.sxtw, R16),
                        Assign(RR0, Mnemonic.vsplath, R16),
                        Assign(RR0, Mnemonic.vsplatb, R16),
                        invalid)),
                Mask(5, 3, "  1",
                    Assign(RR0, Mnemonic.convert_sf2df, R16),
                    Assign(RR0, Mnemonic.convert_uw2df, R16),
                    Assign(RR0, Mnemonic.convert_w2df, R16),
                    Assign(RR0, Mnemonic.convert_sf2ud, R16),
                    Assign(RR0, Mnemonic.convert_sf2d, R16),
                    Assign(RR0, Chop(Apply(Mnemonic.convert_sf2ud, R16))),
                    Assign(RR0, Chop(Apply(Mnemonic.convert_sf2d, R16))),
                    invalid));

            /*
            1,0,0,0, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,-,-,-,-,-,-,d,d,"Pd=tstbit(Rs,#u5)"
            1,0,0,0, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,0,j,j,j,j,j,-,-,-,-,-,-,d,d,"Pd=!tstbit(Rs,#u5)"
            1,0,0,0, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,d,d,"Pd=Rs"
            1,0,0,0, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,-,-,-,-,-,-,d,d,"Pd=bitsclr(Rs,#u6)"
            1,0,0,0, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,j,j,j,j,j,j,-,-,-,-,-,-,d,d,"Pd=!bitsclr(Rs,#u6)"
            1,0,0,0, 0,1,0,1, 1,1,1,s,s,s,s,s,P,P,0,j,j,j,j,j,-,-,-,-,-,-,d,d,"Pd=sfclass(Rs,#u5)"
            */
            var decoder_85 = Mask(21, 3, "  85",
                Assign(P_0L2, Apply(Mnemonic.tstbit, R16, uw_8L5)),
                Assign(P_0L2, InvertIfSet(21, Apply(Mnemonic.tstbit, R16, uw_8L5))),
                Assign(P_0L2, R16),
                invalid,
                Assign(P_0L2, Apply(Mnemonic.bitsclr, R16, uw_8L6)),
                Assign(P_0L2, InvertIfSet(21, Apply(Mnemonic.bitsclr, R16, uw_8L6))),
                invalid,
                Assign(P_0L2, Apply(Mnemonic.sfclass, R16, uw_8L5)));

            /*
            1,0,0,0, 0,1,1,0, -,-,-,-,-,-,-,-,P,P,-,-,-,-,t,t,-,-,-,d,d,d,d,d,"Rdd=mask(Pt)"
            1,0,0,0, 0,1,1,1, 0,0,j,s,s,s,s,s,P,P,I,I,I,I,I,I,j,j,j,x,x,x,x,x,"Rx=tableidxb(Rs,#u4,#S6):raw"
            1,0,0,0, 0,1,1,1, 0,1,j,s,s,s,s,s,P,P,I,I,I,I,I,I,j,j,j,x,x,x,x,x,"Rx=tableidxh(Rs,#u4,#S6):raw"
            1,0,0,0, 0,1,1,1, 1,0,j,s,s,s,s,s,P,P,I,I,I,I,I,I,j,j,j,x,x,x,x,x,"Rx=tableidxw(Rs,#u4,#S6):raw"
            1,0,0,0, 0,1,1,1, 1,1,j,s,s,s,s,s,P,P,I,I,I,I,I,I,j,j,j,x,x,x,x,x,"Rx=tableidxd(Rs,#u4,#S6):raw"

            1,0,0,0, 1,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=vsathub(Rss)"
            1,0,0,0, 1,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_df2sf(Rss)"
            1,0,0,0, 1,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rd=vsatwh(Rss)"
            1,0,0,0, 1,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=vsatwuh(Rss)"
            1,0,0,0, 1,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=vsathb(Rss)"
            */
            var decoder_88_0 = Mask(5, 3, "  0x88 0...",
                Assign(R0, Mnemonic.vsathub, RR16),
                Assign(R0, Mnemonic.convert_df2sf, RR16),
                Assign(R0, Mnemonic.vsatwh, RR16),
                invalid,
                Assign(R0, Mnemonic.vsatwuh, RR16),
                invalid,
                Assign(R0, Mnemonic.vsathb, RR16),
                invalid);



            /*
            1,0,0,0, 1,0,0,0, 0,0,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_ud2sf(Rss)"

            1,0,0,0, 1,0,0,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=clb(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_d2sf(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rd=cl0(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=cl1(Rss)"

            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=normamt(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_df2uw(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,j,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rd=add(clb(Rss),#s6)"
            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,1,d,d,d,d,d,"Rd=popcount(Rss)"
            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,0,0,j,j,j,j,1,0,0,d,d,d,d,d,"Rd=vasrhub(Rss,#u4):raw"
            1,0,0,0, 1,0,0,0, 0,1,1, s,s,s,s,s,P,P,0,0,j,j,j,j,1,0,1,d,d,d,d,d,"Rd=vasrhub(Rss,#u4):sat"
            */
            var decoder_88_3 = Mask(5, 3, "  0x88 3...",
                Nyi("0b000"),
                Nyi("0b001"),
                Assign(R0, Mnemonic.add, Apply(Mnemonic.clb, R16), sw_7L6),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,0,0,0, 1,0,0,0, 1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=vtrunohb(Rss)"
            1,0,0,0, 1,0,0,0, 1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_df2w(Rss)"
            1,0,0,0, 1,0,0,0, 1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rd=vtrunehb(Rss)"
            1,0,0,0, 1,0,0,0, 1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=vrndwh(Rss)"
            1,0,0,0, 1,0,0,0, 1,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=vrndwh(Rss):sat"
            1,0,0,0, 1,0,0,0, 1,0,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_df2uw(Rss):chop"
            1,0,0,0, 1,0,0,0, 1,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=sat(Rss)"
            1,0,0,0, 1,0,0,0, 1,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=round(Rss):sat"
            1,0,0,0, 1,0,0,0, 1,1,0, s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rd=vasrw(Rss,#u5)"
            1,0,0,0, 1,0,0,0, 1,1,0, s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,0,d,d,d,d,d,"Rdd=bitsplit(Rs,#u5)"

            1,0,0,0, 1,0,0,0, 1,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_df2w(Rss):chop"
            1,0,0,0, 1,0,0,0, 1,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,0,d,d,d,d,d,"Rd=ct0(Rss)"
            1,0,0,0, 1,0,0,0, 1,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=ct1(Rss)"
            */
            var decoder_88_7 = Sparse(5, 3, "0b111", invalid,
                (0b001, Assign(R0, Chop(Apply(Mnemonic.convert_df2w, RR16)))),
                (0b010, Assign(R0, Mnemonic.ct0, RR16)),
                (0b100, Assign(R0, Mnemonic.ct1, RR16)));

            var decoder_88 = Mask(21, 3, "  0x88...",
                decoder_88_0,
                Nyi("0b001"),
                Mask(5, 3, "  0b010",
                    Nyi("0b000"),
                    Nyi("0b001"),
                    Assign(R0, Apply(Mnemonic.cl0, RR16)),
                    Nyi("0b011"),
                    Nyi("0b100"),
                    Nyi("0b101"),
                    Nyi("0b110"),
                    Nyi("0b111")),
                decoder_88_3,
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                decoder_88_7);
            /*
            1,0,0,0, 1,0,0,1, -,0,0,-,-,-,s,s,P,P,-,-,-,-,t,t,-,-,-,d,d,d,d,d,"Rd=vitpack(Ps,Pt)"
            1,0,0,0, 1,0,0,1, -,1,-,-,-,-,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=Ps"
            */
            var decoder_89 = Mask(22, 1, "  0x89...",
                Assign(R0, Apply(Mnemonic.vitpack, P_16L2, P_8L2)),
                Assign(R0, P_16L2));
            /*
            1,0,0,0, 1,0,1,0, I,I,I,s,s,s,s,s,P,P,j,j,j,j,j,j,I,I,I,d,d,d,d,d,"Rdd=extract(Rss,#u6,#U6)"
            1,0,0,0, 1,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=convert_uw2sf(Rs)"
            1,0,0,0, 1,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=convert_w2sf(Rs)"
            1,0,0,0, 1,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=convert_sf2uw(Rs)"
            1,0,0,0, 1,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_sf2uw(Rs):chop"
            1,0,0,0, 1,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=convert_sf2w(Rs)"
            1,0,0,0, 1,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,1,d,d,d,d,d,"Rd=convert_sf2w(Rs):chop"
            1,0,0,0, 1,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,0,d,d,d,d,d,"Rd=sffixupr(Rs)"
            1,0,0,0, 1,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,0,e,e,d,d,d,d,d,"Rd,Pe=sfinvsqrta(Rs)"
            */
            var decoder_8B = Mask(21, 3, "  0x8B...",
                invalid,
                If(5, 3, Eq0, Assign(R0, Apply(Mnemonic.convert_uw2sf, R16))),
                Assign(R0, Apply(Mnemonic.convert_w2sf, R16)),
                Assign(R0, Apply(Mnemonic.convert_sf2uw, R16)),

                Sparse(5, 3, "   100",  Nyi("100"),
                    (0, Assign(R0, Apply(Mnemonic.convert_uw2sf, R16))),
                    (1, Assign(R0, Chop(Apply(Mnemonic.convert_uw2sf, R16))))),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));

            /*
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,0,d,d,d,d,d,"Rd=asr(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,1,d,d,d,d,d,"Rd=lsr(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rd=asl(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,1,d,d,d,d,d,"Rd=rol(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=clb(Rs)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rd=cl0(Rs)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=cl1(Rs)"
            1,0,0,0, 1,1,0,0, 0,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rd=normamt(Rs)"
            */
            var decoder_8C_0 = Mask(5, 3, "  8C 0",
                Assign(R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                Assign(R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                Assign(R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                Assign(R0, Apply(Mnemonic.rol, R16, uw_8L5)),
                Assign(R0, Apply(Mnemonic.clb, R16)),
                Assign(R0, Apply(Mnemonic.cl0, R16)),
                Assign(R0, Apply(Mnemonic.cl1, R16)),
                Assign(R0, Apply(Mnemonic.normamt, R16)));

            /*
            1,0,0,0, 1,1,0,0, 0,0,1,s,s,s,s,s,P,P,j,j,j,j,j,j,0,0,0,d,d,d,d,d,"Rd=add(clb(Rs),#s6)"
            *//*
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,0,d,d,d,d,d,"Rd=asr(Rs,#u5):rnd"
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rd=asl(Rs,#u5):sat"
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=ct0(Rs)"
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rd=ct1(Rs)"
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=brev(Rs)"
            1,0,0,0, 1,1,0,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rd=vsplatb(Rs)"
            */
            var decoder_8C_2 = Mask(5, 3, "  0x8C 2",
                Nyi("0b000"),
                invalid,
                Nyi("0b010"),
                invalid,
                Assign(R0, Mnemonic.ct0, R16),
                Assign(R0, Mnemonic.ct1, R16),
                Assign(R0, Mnemonic.brev, R16),
                Assign(R0, Apply(Mnemonic.vsplatb, R16)));
            /*
            1,0,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,0,-,d,d,d,d,d,"Rd=vsathb(Rs)"
            1,0,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,-,-,-,-,-,0,1,-,d,d,d,d,d,"Rd=vsathub(Rs)"
            1,0,0,0, 1,1,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=abs(Rs)"
            1,0,0,0, 1,1,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rd=abs(Rs):sat"
            1,0,0,0, 1,1,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=neg(Rs):sat"
            1,0,0,0, 1,1,0,0, 1,0,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rd=swiz(Rs)"
            */
            var vsathb = Assign(R0, Mnemonic.vsathb, R16);
            var vsathub = Assign(R0, Mnemonic.vsathub, R16);

            var decoder_8C_4 = Mask(5, 3, "  0x8C 4...",
                vsathb,
                vsathb,
                vsathub,
                vsathub,
                Assign(R0, Mnemonic.abs, R16),
                Assign(R0, Sat(Apply(Mnemonic.abs, R16))),
                Assign(R0, Sat(Apply(Mnemonic.neg, R16))),
                Assign(R0, Mnemonic.swiz, R16));

            /*
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,0,d,d,d,d,d,"Rd=setbit(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,1,d,d,d,d,d,"Rd=clrbit(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,d,d,d,d,d,"Rd=togglebit(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,0,d,d,d,d,d,"Rd=sath(Rs)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,0,1,d,d,d,d,d,"Rd=satuh(Rs)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,0,d,d,d,d,d,"Rd=satub(Rs)"
            1,0,0,0, 1,1,0,0, 1,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,1,1,1,d,d,d,d,d,"Rd=satb(Rs)"
            */
            var decoder_8C_6 = Mask(5, 3, "  0b110",
                Assign(R0, Apply(Mnemonic.setbit,R16,uw_7L5)),
                Assign(R0, Apply(Mnemonic.clrbit,R16,uw_7L5)),
                Assign(R0, Apply(Mnemonic.togglebit,R16,uw_7L5)),
                invalid,
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,0,0,0, 1,1,0,0, 1,1,1,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,-,d,d,d,d,d,"Rd=cround(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 1,1,1,s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,-,d,d,d,d,d,"Rd=round(Rs,#u5)"
            1,0,0,0, 1,1,0,0, 1,1,1,s,s,s,s,s,P,P,0,j,j,j,j,j,1,1,-,d,d,d,d,d,"Rd=round(Rs,#u5):sat"
            */
            var decoder_8C = Mask(21, 3, "  0x8C..",
                decoder_8C_0,
                Nyi("0b001"),
                decoder_8C_2,
                Nyi("0b011"),
                decoder_8C_4,
                Nyi("0b101"),
                decoder_8C_6,
                Nyi("0b111"));
            /*
            1,0,0,0, 1,1,0,1, 0,I,I,s,s,s,s,s,P,P,0,j,j,j,j,j,I,I,I,d,d,d,d,d,"Rd=extractu(Rs,#u5,#U5)"
            1,0,0,0, 1,1,0,1, 1,I,I,s,s,s,s,s,P,P,0,j,j,j,j,j,I,I,I,d,d,d,d,d,"Rd=extract(Rs,#u5,#U5)"
            */
            var decoder_8D = Mask(23, 1, "  0x8D...",
                Assign(R0, Apply(Mnemonic.extractu, R16, uw_8L5, uw_21L2_5L3)),
                Assign(R0, Apply(Mnemonic.extract, R16, uw_8L5, uw_21L2_5L3)));
            /*
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,0,x,x,x,x,x,"Rx-=asr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,1,x,x,x,x,x,"Rx-=lsr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,x,x,x,x,x,"Rx-=asl(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,1,x,x,x,x,x,"Rx-=rol(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,0,x,x,x,x,x,"Rx+=asr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,1,x,x,x,x,x,"Rx+=lsr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,1,0,x,x,x,x,x,"Rx+=asl(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,1,1,x,x,x,x,x,"Rx+=rol(Rs,#u5)"

            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,0,x,x,x,x,x,"Rx&=asr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,1,x,x,x,x,x,"Rx&=lsr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,x,x,x,x,x,"Rx&=asl(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,1,x,x,x,x,x,"Rx&=rol(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,0,x,x,x,x,x,"Rx|=asr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,0,1,x,x,x,x,x,"Rx|=lsr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,1,0,x,x,x,x,x,"Rx|=asl(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 0,1,-,s,s,s,s,s,P,P,0,j,j,j,j,j,1,1,1,x,x,x,x,x,"Rx|=rol(Rs,#u5)"

            1,0,0,0, 1,1,1,0, 1,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,0,1,x,x,x,x,x,"Rx^=lsr(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 1,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,0,x,x,x,x,x,"Rx^=asl(Rs,#u5)"
            1,0,0,0, 1,1,1,0, 1,0,-,s,s,s,s,s,P,P,0,j,j,j,j,j,0,1,1,x,x,x,x,x,"Rx^=rol(Rs,#u5)"
            */
            var decoder_8E = Mask(22, 2, "  0x8E...",
                Mask(5, 3, "  00",
                    Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                    Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                    Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                    Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.rol, R16, uw_8L5)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.rol, R16, uw_8L5))),
                Mask(5, 3, "  01",
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.rol, R16, uw_8L5)),
                    Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                    Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                    Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                    Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.rol, R16, uw_8L5))),
                Mask(5, 3, "  01",
                    Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.asr, R16, uw_8L5)),
                    Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.lsr, R16, uw_8L5)),
                    Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.asl, R16, uw_8L5)),
                    Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.rol, R16, uw_8L5)),
                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid);
            /*
            1,0,0,0, 1,1,1,1, 0,I,I,s,s,s,s,s,P,P,0,j,j,j,j,j,I,I,I,x,x,x,x,x,"Rx=insert(Rs,#u5,#U5)"
             */
            var decoder_8 = Mask(24, 4, "  XTYPE 2, 3",
                decoder_80,
                decoder_81,
                decoder_82,
                decoder_83,
                decoder_84,
                decoder_85,
                Nyi("0b0110"),
                Nyi("0b0111"),
                decoder_88,
                decoder_89,
                Nyi("0b1010"),
                decoder_8B,
                decoder_8C,
                decoder_8D,
                decoder_8E,
                Assign(R0, Mnemonic.insert, uw_8L5, uw_21L2_5L3));
            /*
            1,0,0,1, 0,0,0,0,0,0,0,1,1,1,1,0,P,P,0,-,-,-,-,-,-,-,-,1,1,1,1,0,"deallocframe",
            */
            //$TODO: check remaining bits with a (to be written) Select(bitmask, prediate)
            var decoder_90 = Instr(Mnemonic.deallocframe);
            /*
            1,0,0,1, 0,0,1,0,0,0,0,s,s,s,s,s,P,P,0,0,-,-,-,-,-,-,0,d,d,d,d,d,"Rd=memw_locked(Rs)"
            1,0,0,1, 0,0,1,0,0,0,0,s,s,s,s,s,P,P,0,1,-,-,-,-,-,-,0,d,d,d,d,d,"Rdd=memd_locked(Rs)"
            1,0,0,1, 0,0,1,0,0,0,0,s,s,s,s,s,P,P,1,t,t,t,t,t,-,-,0,d,d,d,d,d,"Rd=memw_phys(Rs,Rt)"
            */
            var decoder_92 = Mask(12, 2, "  0x92...",
                Assign(R0, Mnemonic.memw_locked, R16),
                Assign(RR0, Mnemonic.memd_locked, R16),
                Assign(R0, Mnemonic.memw_phys, R16, R8),
                Assign(R0, Mnemonic.memw_phys, R16, R8));

            /*
            1,0,0,1, 0,1,0,0,0,0,0,s,s,s,s,s,P,P,0,-,-,j,j,j,j,j,j,j,j,j,j,j,"dcfetch(Rs+#u11:3)"
            */
            var decoder_94 = Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dcfetch, R16, uw_0L11_3));
            /*
            1,0,0,1, 0,j,j,0, 0,0,1, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=membh(Rs+#s11:1)"
            1,0,0,1, 0,j,j,0, 0,1,0, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,y,y,y,y,y,"Ryy=memh_fifo(Rs+#s11:1)"
            1,0,0,1, 0,j,j,0, 0,1,1, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memubh(Rs+#s11:1)"
            1,0,0,1, 0,j,j,0, 1,0,0, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,y,y,y,y,y,"Ryy=memb_fifo(Rs+#s11:0)"
            1,0,0,1, 0,j,j,0, 1,0,1, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=memubh(Rs+#s11:2)"
            1,0,0,1, 0,j,j,0, 1,1,1, s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=membh(Rs+#s11:2)"
            */
            var decoder_9_0jj0 = Nyi("9_0jj0");
            /*
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 0,0,0,0, -,-,-,-,-,1,1,1,1,0,"dealloc_return"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 0,0,1,0, s,s,-,-,-,1,1,1,1,0,"if (Ps.new)dealloc_return:nt"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 0,1,0,0, s,s,-,-,-,1,1,1,1,0,"if (Ps) dealloc_return"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 0,1,1,0, s,s,-,-,-,1,1,1,1,0,"if (Ps.new) dealloc_return:t"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 1,0,1,0, s,s,-,-,-,1,1,1,1,0,"if (!Ps.new)dealloc_return:nt"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 1,1,0,0, s,s,-,-,-,1,1,1,1,0,"if (!Ps) dealloc_return"
            1,0,0,1, 0,1,1,0, 0,0,0, 1,1,1,1,0,P,P, 1,1,1,0, s,s,-,-,-,1,1,1,1,0,"if (!Ps.new) dealloc_return:t"
            */

            var decoder_96 = Mask(21, 3, "  96",
                Select(Bf((16,5),(0,5)), u=> u == 0b11110_11110,
                    Select(Bf((10, 4)), Eq0,
                        Instr(Mnemonic.dealloc_return, InstrClass.Transfer | InstrClass.Return),
                        Instr(Mnemonic.dealloc_return, InstrClass.ConditionalTransfer | InstrClass.Return, Conditional(8, 11, 12, 13))),
                    invalid),
                decoder_9_0jj0,
                decoder_9_0jj0,
                decoder_9_0jj0,
                decoder_9_0jj0,
                decoder_9_0jj0,
                decoder_9_0jj0,
                decoder_9_0jj0);
            /*
            1,0,0,1, 0,j,j,1, 0,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memb(Rs+#s11:0)"
            1,0,0,1, 0,j,j,1, 0,0,1,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memub(Rs+#s11:0)"
            1,0,0,1, 0,j,j,1, 0,1,0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memh(Rs+#s11:1)"
            1,0,0,1, 0,j,j,1, 0,1,1,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memuh(Rs+#s11:1)"
            1,0,0,1, 0,j,j,1, 1,0,0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=memw(Rs+#s11:2)"
            1,0,0,1, 0,j,j,1, 1,1,0,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=memd(Rs+#s11:3)"
            */
            var bf_25_5 = Bf((25, 2), (5, 9));

            var decoder9_ld = Mask(21, 3, "  ld",
                Assign(R0, M(PrimitiveType.SByte, 16, bf_25_5)),
                Assign(R0, M(PrimitiveType.Byte, 16, bf_25_5)),
                Assign(R0, M(PrimitiveType.Int16, 16, bf_25_5)),
                Assign(R0, M(PrimitiveType.Word16, 16, bf_25_5)),
                Assign(R0, M(PrimitiveType.Word32, 16, bf_25_5)),
                invalid,
                Assign(RR0, M(PrimitiveType.Word64, 16, bf_25_5)),
                invalid);
            /*
            1,0,0,1, 1,0,0,0,0,0,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=membh(Rx++#s4:1:circ(Mu))"
            1,0,0,1, 1,0,0,0,0,0,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=membh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,0,0,1,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,y,y,y,y,y,"Ryy=memh_fifo(Rx++#s4:1:circ(Mu))"
            1,0,0,1, 1,0,0,0,0,1,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,y,y,y,y,y,"Ryy=memh_fifo(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,0,0,1,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memubh(Rx++#s4:1:circ(Mu))"
            1,0,0,1, 1,0,0,0,0,1,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memubh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,0,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,y,y,y,y,y,"Ryy=memb_fifo(Rx++#s4:0:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,0,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,y,y,y,y,y,"Ryy=memb_fifo(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,0,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rdd=memubh(Rx++#s4:2:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,0,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rdd=memubh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,1,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rdd=membh(Rx++#s4:2:circ(Mu))"
            1,0,0,1, 1,0,0,0,1,1,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rdd=membh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,0,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memb(Rx++#s4:0:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,0,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memb(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,0,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memub(Rx++#s4:0:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,0,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memub(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,1,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memh(Rx++#s4:1:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,1,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,1,1,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memuh(Rx++#s4:1:circ(Mu))"
            1,0,0,1, 1,0,0,1,0,1,1,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memuh(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,1,0,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rd=memw(Rx++#s4:2:circ(Mu))"
            1,0,0,1, 1,0,0,1,1,0,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rd=memw(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,0,1,1,1,0,x,x,x,x,x,P,P,u,0,-,-,0,j,j,j,j,d,d,d,d,d,"Rdd=memd(Rx++#s4:3:circ(Mu))"
            1,0,0,1, 1,0,0,1,1,1,0,x,x,x,x,x,P,P,u,0,-,-,1,-,0,-,-,d,d,d,d,d,"Rdd=memd(Rx++I:circ(Mu))"
            1,0,0,1, 1,0,1,0,0,0,1,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=membh(Re=#U6)"
            1,0,0,1, 1,0,1,0,0,0,1,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=membh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,0,0,1,0,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,y,y,y,y,y,"Ryy=memh_fifo(Re=#U6)"
            1,0,0,1, 1,0,1,0,0,1,0,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,y,y,y,y,y,"Ryy=memh_fifo(Rx++#s4:1)"
            1,0,0,1, 1,0,1,0,0,1,1,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memubh(Re=#U6)"
            1,0,0,1, 1,0,1,0,0,1,1,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=memubh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,0,1,0,0,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,y,y,y,y,y,"Ryy=memb_fifo(Re=#U6)"
            1,0,0,1, 1,0,1,0,1,0,0,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,y,y,y,y,y,"Ryy=memb_fifo(Rx++#s4:0)"
            1,0,0,1, 1,0,1,0,1,0,1,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rdd=memubh(Re=#U6)"
            1,0,0,1, 1,0,1,0,1,0,1,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rdd=memubh(Rx++#s4:2)"
            1,0,0,1, 1,0,1,0,1,1,1,e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rdd=membh(Re=#U6)"
            1,0,0,1, 1,0,1,0,1,1,1,x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rdd=membh(Rx++#s4:2)"

            1,0,0,1, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=memb(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,0, e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memb(Re=#U6)"
            1,0,0,1, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,0,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memb(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,0,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memb(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,1,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memb(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,1,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memb(Rx++#s4:0)"
            */
            var decoder_9B_0 = Mask(12, 2, "  0x9B 1",
                Assign(R0, Mpostinc(PrimitiveType.SByte, 16, (5, 4), 0)),
                Nyi("01"),
                Assign(R0, Conditional(9, 12, -1, 11), Mpostinc(PrimitiveType.SByte, 16, (5, 4), 0)),
                Nyi("11"));
            /*
            1,0,0,1, 1,0,1,1, 0,0,1, x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=memub(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,1, e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memub(Re=#U6)"
            1,0,0,1, 1,0,1,1, 0,0,1, x,x,x,x,x,P,P,1,0,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memub(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,1, x,x,x,x,x,P,P,1,0,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memub(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,1, x,x,x,x,x,P,P,1,1,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memub(Rx++#s4:0)"
            1,0,0,1, 1,0,1,1, 0,0,1, x,x,x,x,x,P,P,1,1,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memub(Rx++#s4:0)"
            */
            var decoder_9B_1 = Mask(12, 2, "  0x9B 1",
                Assign(R0, Mpostinc(PrimitiveType.Byte, 16, (5, 4), 0)),
                Nyi("01"),
                Nyi("10"),
                Nyi("11"));
            /*
            1,0,0,1, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=memh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,0, e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memh(Re=#U6)"
            1,0,0,1, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,0,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,0,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,1,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,1,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memh(Rx++#s4:1)"

            1,0,0,1, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P, 0,0, -,-,-,j,j,j,j,d,d,d,d,d,"Rd=memuh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,1, e,e,e,e,e,P,P, 0,1, I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memuh(Re=#U6)"
            1,0,0,1, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P, 1,0, 0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memuh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P, 1,0, 1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memuh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P, 1,1, 0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memuh(Rx++#s4:1)"
            1,0,0,1, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P, 1,1, 1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memuh(Rx++#s4:1)"
            */
            var decoder_9B_3 = Mask(12, 2, "  0x9B 3",
                Assign(R0, Mpostinc(PrimitiveType.Word16, 16, (5, 4), 1)),
                Nyi("01"),
                Nyi("10"),
                Nyi("11"));
            /*
            1,0,0,1, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rd=memw(Rx++#s4:2)"
            1,0,0,1, 1,0,1,1, 1,0,0, e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rd=memw(Re=#U6)"
            1,0,0,1, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,0,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rd=memw(Rx++#s4:2)"
            1,0,0,1, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,0,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rd=memw(Rx++#s4:2)"
            1,0,0,1, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,1,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rd=memw(Rx++#s4:2)"
            1,0,0,1, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,1,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rd=memw(Rx++#s4:2)"
            */
            var decoder_9B_4 = Mask(12, 2, "  0x9B 4...",
                Assign(R0, Mpostinc(PrimitiveType.Word32, 16, (5, 4), 2)),
                Assign(R0, MabsSet( PrimitiveType.Word32, 16, bf_8L4_5L2)),
                Assign(R0, Conditional(9, 12, -1, 11), Mpostinc(PrimitiveType.Word32, 16, (5, 4), 2)),
                Nyi("11"));
            /*
            1,0,0,1, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,0,0,-,-,-,j,j,j,j,d,d,d,d,d,"Rdd=memd(Rx++#s4:3)"
            1,0,0,1, 1,0,1,1, 1,1,0, e,e,e,e,e,P,P,0,1,I,I,I,I,-,I,I,d,d,d,d,d,"Rdd=memd(Re=#U6)"
            1,0,0,1, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,0,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt)Rdd=memd(Rx++#s4:3)"
            1,0,0,1, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,0,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt)Rdd=memd(Rx++#s4:3)"
            1,0,0,1, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,1,0,t,t,j,j,j,j,d,d,d,d,d,"if (Pt.new)Rdd=memd(Rx++#s4:3)"
            1,0,0,1, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,1,1,t,t,j,j,j,j,d,d,d,d,d,"if (!Pt.new)Rdd=memd(Rx++#s4:3)"
            */

            var decoder_9B_6 = Mask(12, 2, "  0x9B 6...",
                Assign(RR0, Mpostinc(PrimitiveType.Word64, 16, (5, 4), 3)),
                Nyi("01"),
                Assign(RR0, Mpostinc(PrimitiveType.Word64, 16, (5, 4), 3), Conditional(9, 12, -1, 11)),
                Assign(RR0, Mpostinc(PrimitiveType.Word64, 16, (5, 4), 3), Conditional(9, 12, -1, 11)));

            var decoder_9B = Mask(21, 3, "  0x9B...",
                decoder_9B_0,
                decoder_9B_1,
                Nyi("0b010"),
                decoder_9B_3,
                decoder_9B_4,
                Nyi("0b101"),
                decoder_9B_6,
                invalid);
            /*
            1,0,0,1, 1,1,0,0,0,0,1,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=membh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,0,0,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=membh(Rx++Mu)"
            1,0,0,1, 1,1,0,0,0,1,0,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,y,y,y,y,y,"Ryy=memh_fifo(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,0,1,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,y,y,y,y,y,"Ryy=memh_fifo(Rx++Mu)"
            1,0,0,1, 1,1,0,0,0,1,1,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memubh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,0,1,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memubh(Rx++Mu)"
            1,0,0,1, 1,1,0,0,1,0,0,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,y,y,y,y,y,"Ryy=memb_fifo(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,1,0,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,y,y,y,y,y,"Ryy=memb_fifo(Rx++Mu)"
            1,0,0,1, 1,1,0,0,1,0,1,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rdd=memubh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,1,0,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=memubh(Rx++Mu)"
            1,0,0,1, 1,1,0,0,1,1,1,t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rdd=membh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,0,1,1,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=membh(Rx++Mu)"
            
            1,0,0,1, 1,1,0,1, 0,0,0, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memb(Rx++Mu)"
            1,0,0,1, 1,1,0,1, 0,0,1, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memub(Rx++Mu)"
            1,0,0,1, 1,1,0,1, 0,1,0, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memh(Rx++Mu)"
            1,0,0,1, 1,1,0,1, 0,1,1, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memuh(Rx++Mu)"
            1,0,0,1, 1,1,0,1, 1,0,0, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memw(Rx++Mu)"
            1,0,0,1, 1,1,0,1, 1,1,0, x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=memd(Rx++Mu)"

            1,0,0,1, 1,1,0,1, 0,0,0, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memb(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,1, 0,0,1, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memub(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,1, 0,1,0, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,1, 0,1,1, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memuh(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,1, 1,0,0, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rd=memw(Rt<<#u2+#U6)"
            1,0,0,1, 1,1,0,1, 1,1,0, t,t,t,t,t,P,P,j,1,I,I,I,I,j,I,I,d,d,d,d,d,"Rdd=memd(Rt<<#u2+#U6)"
            */
            var decoder_9D = Mask(12, 1, "  0x9D...",
                Mask(21, 3, "  0x9D 0...",
                    Nyi("000"),
                    Nyi("001"),
                    Nyi("010"),
                    Nyi("011"),
                    Nyi("100"),
                    Nyi("101"),
                    Assign(RR0, MpostincM(PrimitiveType.Word64, 16)),
                    Nyi("111")),
                Mask(21, 3, "  0x9D 1...",
                    Assign(R0, Midx(PrimitiveType.SByte, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    Assign(R0, Midx(PrimitiveType.Byte, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    Assign(R0, Midx(PrimitiveType.Int16, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    Assign(R0, Midx(PrimitiveType.Word16, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    Assign(R0, Midx(PrimitiveType.Word32, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    invalid,
                    Assign(RR0, Midx(PrimitiveType.Word64, 16, bf_13L1_7L1, bf_8L4_5L2)),
                    invalid));
            /*
        1,0,0,1, 1,1,1,0,0,0,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=membh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,0,0,1,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,y,y,y,y,y,"Ryy=memh_fifo(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,0,0,1,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memubh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,0,1,0,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,y,y,y,y,y,"Ryy=memb_fifo(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,0,1,0,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=memubh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,0,1,1,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=membh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,0,0,0,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rd=memb(#u6)"
        1,0,0,1, 1,1,1,1,0,0,0,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rd=memb(#u6)"
        1,0,0,1, 1,1,1,1,0,0,0,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rd=memb(#u6)"
        1,0,0,1, 1,1,1,1,0,0,0,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new) Rd=memb(#u6)"
        1,0,0,1, 1,1,1,1,0,0,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memb(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,0,0,1,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rd=memub(#u6)"
        1,0,0,1, 1,1,1,1,0,0,1,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rd=memub(#u6)"
        1,0,0,1, 1,1,1,1,0,0,1,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rd=memub(#u6)"
        1,0,0,1, 1,1,1,1,0,0,1,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new)Rd=memub(#u6)"
        1,0,0,1, 1,1,1,1,0,0,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memub(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,0,1,0,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rd=memh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,0,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rd=memh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,0,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rd=memh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,0,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new) Rd=memh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,0,1,1,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rd=memuh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,1,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rd=memuh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,1,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rd=memuh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,1,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new)Rd=memuh(#u6)"
        1,0,0,1, 1,1,1,1,0,1,1,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memuh(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,1,0,0,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rd=memw(#u6)"
        1,0,0,1, 1,1,1,1,1,0,0,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rd=memw(#u6)"
        1,0,0,1, 1,1,1,1,1,0,0,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rd=memw(#u6)"
        1,0,0,1, 1,1,1,1,1,0,0,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new) Rd=memw(#u6)"
        1,0,0,1, 1,1,1,1,1,0,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rd=memw(Rx++Mu:brev)"
        1,0,0,1, 1,1,1,1,1,1,0,j,j,j,j,j,P,P,1,0,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt) Rdd=memd(#u6)"
        1,0,0,1, 1,1,1,1,1,1,0,j,j,j,j,j,P,P,1,0,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt) Rdd=memd(#u6)"
        1,0,0,1, 1,1,1,1,1,1,0,j,j,j,j,j,P,P,1,1,0,t,t,j,1,-,-,d,d,d,d,d,"if (Pt.new) Rdd=memd(#u6)"
        1,0,0,1, 1,1,1,1,1,1,0,j,j,j,j,j,P,P,1,1,1,t,t,j,1,-,-,d,d,d,d,d,"if (!Pt.new)Rdd=memd(#u6)"
        1,0,0,1, 1,1,1,1,1,1,0,x,x,x,x,x,P,P,u,0,-,-,-,-,0,-,-,d,d,d,d,d,"Rdd=memd(Rx++Mu:brev)"
        */
            var decoder_9 = Mask(24, 4, "  LD 0, 1",
                decoder_90,
                decoder9_ld,
                decoder_92,
                decoder9_ld,
                decoder_94,
                decoder9_ld,
                decoder_96,
                decoder9_ld,
                Nyi("0b1000"),
                Nyi("0b1001"),
                Nyi("0b1010"),
                decoder_9B,
                Nyi("0b1100"),
                decoder_9D,
                Nyi("0b1110"),
                Nyi("0b1111"));

            /* 
            1,0,1,0, 0,0,0,0, 0,0,0, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dccleana(Rs)"
            1,0,1,0, 0,0,0,0, 0,0,1, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dcinva(Rs)"
            1,0,1,0, 0,0,0,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dccleaninva(Rs)"
            1,0,1,0, 0,0,0,0, 1,0,0, 1,1,1,0,1,P,P,0,0,0,j,j,j,j,j,j,j,j,j,j,j,"allocframe(#u11:3)"
            1,0,1,0, 0,0,0,0, 1,0,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"memw_locked(Rs,Pd)=Rt"
            1,0,1,0, 0,0,0,0, 1,1,0, s,s,s,s,s,P,P,0,-,-,-,-,-,-,-,-,-,-,-,-,-,"dczeroa(Rs)"
            1,0,1,0, 0,0,0,0, 1,1,1, s,s,s,s,s,P,P,0,t,t,t,t,t,-,-,-,-,-,-,d,d,"memd_locked(Rs,Pd)=Rtt"
            1,0,1,0, 0,0,0,0, 1,1,1, s,s,s,s,s,P,P,1,-,-,-,-,-,-,-,-,-,-,-,d,d,"Pd=l2locka(Rs)"
            */
            var decoder_A0 = Mask(21, 3, "  A0",
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dccleana, R16)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dcinva, R16)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dccleaninva, R16)),
                invalid,
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.allocframe, uimmSh(PrimitiveType.Int32, Bf((0,11)), 3))),
                Instr(Mnemonic.ASSIGN, Apply(Mnemonic.memw_locked, R16, P_0L2), R8),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dczeroa, R16)),
                Nyi("0b111"));
            /*
            1,0,1,0, 0,0,1,0, 0,0,0, -,-,-,-,-,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dckill"
            1,0,1,0, 0,0,1,0, 0,0,1, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dccleanidx(Rs)"
            1,0,1,0, 0,0,1,0, 0,1,0, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dcinvidx(Rs)"
            1,0,1,0, 0,0,1,0, 0,1,1, s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"dccleaninvidx(Rs)"
            */
            var decoder_A2 = Mask(21, 3, "  0xA2...",
                Instr(Mnemonic.dckill),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dcleanidx, R16)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dcinvdx, R16)),
                Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.dccleaninvdx, R16)),
                invalid,
                invalid,
                invalid,
                invalid);
            /*
            1,0,1,0, 0,1,0,0, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"dctagw(Rs,Rt)"
            1,0,1,0, 0,1,0,0, 0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=dctagr(Rs)"
            1,0,1,0, 0,1,0,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,-,-,-,-,-,-,-,-,"l2tagw(Rs,Rt)"
            1,0,1,0, 0,1,0,0, 0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,d,d,d,d,d,"Rd=l2tagr(Rs)"
            */
            var decoder_A4 = Mask(21, 3, "  0xA4...",
                Nyi("000"),
                Nyi("001"),
                SideEffect(Apply(Mnemonic.l2tagw, R16, R8)),
                Nyi("011"),
                Nyi("100"),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));

            /*
            1,0,1,0, 0,1,1,0, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"l2fetch(Rs,Rt)"
            1,0,1,0, 0,1,1,0, 0,0,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"l2cleanidx(Rs)"
            1,0,1,0, 0,1,1,0, 0,1,0,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"l2invidx(Rs)"
            1,0,1,0, 0,1,1,0, 0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"l2unlocka(Rs)"
            1,0,1,0, 0,1,1,0, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"l2fetch(Rs,Rtt)"
            1,0,1,0, 0,1,1,0, 1,0,1,-,-,-,-,-,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"l2gclean(Rtt)"
            1,0,1,0, 0,1,1,0, 1,1,0,-,-,-,-,-,P,P,-,t,t,t,t,t,-,-,-,-,-,-,-,-,"l2gcleaninv(Rtt)"
            */
            var decoder_A6 = Mask(21, 3, "  0xA6...",
                Nyi("000"),
                Nyi("001"),
                Nyi("010"),
                Nyi("011"),
                SideEffect(Apply(Mnemonic.l2fetch, R16, RR8)),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));
            /*
            1,0,1,0, 0,j,j,1, 0,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memb(Rs+#s11:0)=Rt"
            1,0,1,0, 0,j,j,1, 0,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memh(Rs+#s11:1)=Rt"
            1,0,1,0, 0,j,j,1, 0,1,1, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memh(Rs+#s11:1)=Rt.H"
            1,0,1,0, 0,j,j,1, 1,0,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memw(Rs+#s11:2)=Rt"
            1,0,1,0, 0,j,j,1, 1,0,1, s,s,s,s,s,P,P,j,0,0,t,t,t,j,j,j,j,j,j,j,j,"memb(Rs+#s11:0)=Nt.new"
            1,0,1,0, 0,j,j,1, 1,0,1, s,s,s,s,s,P,P,j,0,1,t,t,t,j,j,j,j,j,j,j,j,"memh(Rs+#s11:1)=Nt.new"
            1,0,1,0, 0,j,j,1, 1,0,1, s,s,s,s,s,P,P,j,1,0,t,t,t,j,j,j,j,j,j,j,j,"memw(Rs+#s11:2)=Nt.new"
            1,0,1,0, 0,j,j,1, 1,1,0, s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,j,j,j,j,j,"memd(Rs+#s11:3)=Rtt"
            */
            var bf_25_13_0 = Bf((25, 2), (13, 1), (0, 8));

            var decoder0A_st = Mask(21, 3, "  stores",
                Instr(Mnemonic.ASSIGN, M(PrimitiveType.Byte, 16, bf_25_13_0), R8),
                invalid,
                Instr(Mnemonic.ASSIGN, M(PrimitiveType.Word16, 16, bf_25_13_0), R8),
                Instr(Mnemonic.ASSIGN, M(PrimitiveType.Word16, 16, bf_25_13_0), R8_H),

                Instr(Mnemonic.ASSIGN, M(PrimitiveType.Word32, 16, bf_25_13_0), R8),
                Mask(11, 2, "  new-value operands",
                    Instr(Mnemonic.ASSIGN, M(PrimitiveType.Byte, 16, bf_25_13_0), New8),
                    Instr(Mnemonic.ASSIGN, M(PrimitiveType.Byte, 16, bf_25_13_0), New8),
                    Instr(Mnemonic.ASSIGN, M(PrimitiveType.Byte, 16, bf_25_13_0), New8),
                    invalid),
                Instr(Mnemonic.ASSIGN, M(PrimitiveType.Word64, 16, bf_25_13_0), RR8),
                invalid);

            /*
            1,0,1,0, 1,0,0,0, 0,0,0,-,-,-,-,-,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"barrier"
            1,0,1,0, 1,0,0,0, 0,0,1,-,-,-,-,-,P,P,-,0,0,0,-,-,-,-,-,-,-,-,-,-,"l2kill"
            1,0,1,0, 1,0,0,0, 0,0,1,-,-,-,-,-,P,P,-,0,1,0,-,-,-,-,-,-,-,-,-,-,"l2gunlock"
            1,0,1,0, 1,0,0,0, 0,0,1,-,-,-,-,-,P,P,-,1,0,0,-,-,-,-,-,-,-,-,-,-,"l2gclean"
            1,0,1,0, 1,0,0,0, 0,0,1,-,-,-,-,-,P,P,-,1,1,0,-,-,-,-,-,-,-,-,-,-,"l2gcleaninv"
            1,0,1,0, 1,0,0,0, 0,1,0,-,-,-,-,-,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"syncht"
            1,0,1,0, 1,0,0,0, 0,1,1,s,s,s,s,s,P,P,-,-,-,-,-,-,-,-,-,-,-,-,-,-,"l2cleaninvidx(Rs)"
            */
            var decoder_A8 = Sparse(21, 3, "  0xA8...", invalid,
                (0b000, Instr(Mnemonic.barrier)),
                (0b001, Mask(10, 3, "  0b001",
                    Instr(Mnemonic.l2kill),
                    invalid,
                    Instr(Mnemonic.l2gunlock),
                    invalid,
                    Instr(Mnemonic.l2gclean),
                    invalid,
                    Instr(Mnemonic.l2gcleaninv),
                    invalid)),
                (0b010, Instr(Mnemonic.syncht)),
                (0b011, Instr(Mnemonic.SIDEEFFECT, Apply(Mnemonic.l2cleaninvidx, R16))));
            /*
            1,0,1,0, 1,0,0,1, 0,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,1,-,"memb(Rx++I:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 0,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,j,j,j,j,-,0,-,"memb(Rx++#s4:0:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 0,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,1,-,"memh(Rx++I:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 0,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 0,1,1,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,1,-,"memh(Rx++I:circ(Mu))=Rt.H"
            1,0,1,0, 1,0,0,1, 0,1,1,x,x,x,x,x,P,P,u,t,t,t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1:circ(Mu))=Rt.H"
            1,0,1,0, 1,0,0,1, 1,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,1,-,"memw(Rx++I:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 1,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,j,j,j,j,-,0,-,"memw(Rx++#s4:2:circ(Mu))=Rt"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,0,t,t,t,0,-,-,-,-,-,1,-,"memb(Rx++I:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,0,t,t,t,0,j,j,j,j,-,0,-,"memb(Rx++#s4:0:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,1,t,t,t,0,-,-,-,-,-,1,-,"memh(Rx++I:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,1,t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,1,0,t,t,t,0,-,-,-,-,-,1,-,"memw(Rx++I:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,0,1,x,x,x,x,x,P,P,u,1,0,t,t,t,0,j,j,j,j,-,0,-,"memw(Rx++#s4:2:circ(Mu))=Nt.new"
            1,0,1,0, 1,0,0,1, 1,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,1,-,"memd(Rx++I:circ(Mu))=Rtt"
            1,0,1,0, 1,0,0,1, 1,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,j,j,j,j,-,0,-,"memd(Rx++#s4:3:circ(Mu))=Rtt"

            1,0,1,0, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,0,t,t,t,t,t,0,j,j,j,j,-,0,-,"memb(Rx++#s4:0)=Rt"
            1,0,1,0, 1,0,1,1, 0,0,0, e,e,e,e,e,P,P,0,t,t,t,t,t,1,-,I,I,I,I,I,I,"memb(Re=#U6)=Rt"
            1,0,1,0, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memb(Rx++#s4:0)=Rt"
            1,0,1,0, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memb(Rx++#s4:0)=Rt"
            1,0,1,0, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memb(Rx++#s4:0)=Rt"
            1,0,1,0, 1,0,1,1, 0,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memb(Rx++#s4:0)=Rt"
            */
            var decoder_AB_0 = Mask(13, 1, "  0xAB0...",
                Mask(7, 1, "  0",
                    Assign(Mpostinc(PrimitiveType.Byte, 16, (3, 4), 0), R8),
                    Nyi("0b00")),
                Assign(Mpostinc(PrimitiveType.Byte, 16, (3, 4), 0), R8, Conditional(0, 7, -1, 2)));
            /*
            1,0,1,0, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,0,t,t,t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1)=Rt"
            1,0,1,0, 1,0,1,1, 0,1,0, e,e,e,e,e,P,P,0,t,t,t,t,t,1,-,I,I,I,I,I,I,"memh(Re=#U6)=Rt"
            1,0,1,0, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memh(Rx++#s4:1)=Rt"
            1,0,1,0, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memh(Rx++#s4:1)=Rt"
            1,0,1,0, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memh(Rx++#s4:1)=Rt"
            1,0,1,0, 1,0,1,1, 0,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memh(Rx++#s4:1)=Rt"
            */

            var decoder_AB_2 = Mask(13, 1, "  0xAB 2...",
                Mask(7, 1, "  0",
                    Assign(Mpostinc(PrimitiveType.Word16, 16, (3, 4), 1), R8),
                    Nyi("0b00")),
                Assign(Mpostinc(PrimitiveType.Word16, 16, (3, 4), 1), R8, Conditional(0, 7, -1, 2)));

            /*
            1,0,1,0, 1,0,1,1, 0,1,1, e,e,e,e,e,P,P,0,t,t,t,t,t,1,-,I,I,I,I,I,I,"memh(Re=#U6)=Rt.H"
            1,0,1,0, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P,0,t,t,t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1)=Rt.H"
            1,0,1,0, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memh(Rx++#s4:1)=Rt.H"
            1,0,1,0, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memh(Rx++#s4:1)=Rt.H"
            1,0,1,0, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memh(Rx++#s4:1)=Rt.H"
            1,0,1,0, 1,0,1,1, 0,1,1, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memh(Rx++#s4:1)=Rt.H"

            1,0,1,0, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,0,t,t,t,t,t,0,j,j,j,j,-,0,-,"memw(Rx++#s4:2)=Rt"
            1,0,1,0, 1,0,1,1, 1,0,0, e,e,e,e,e,P,P,0,t,t,t,t,t,1,-,I,I,I,I,I,I,"memw(Re=#U6)=Rt"
            1,0,1,0, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memw(Rx++#s4:2)=Rt"
            1,0,1,0, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memw(Rx++#s4:2)=Rt"
            1,0,1,0, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memw(Rx++#s4:2)=Rt"
            1,0,1,0, 1,0,1,1, 1,0,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memw(Rx++#s4:2)=Rt"
            */
            var decoder_AB_4 = Mask(13, 1, "  0xAB 4...",
                Mask(7, 1, "  0",
                    Assign(Mpostinc(PrimitiveType.Word32, 16, (3, 4), 2), R8),
                    Nyi("0b00")),
                Assign(Mpostinc(PrimitiveType.Word16, 16, (3, 4), 2), R8, Conditional(0, 7, -1, 2)));

            /*
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 0,0,0, t,t,t,0,j,j,j,j,-,0,-,"memb(Rx++#s4:0)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, e,e,e,e,e,P,P, 0,0,0, t,t,t,1,-,I,I,I,I,I,I,"memb(Re=#U6)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 0,0,1, t,t,t,0,j,j,j,j,-,0,-,"memh(Rx++#s4:1)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, e,e,e,e,e,P,P, 0,0,1, t,t,t,1,-,I,I,I,I,I,I,"memh(Re=#U6)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 0,1,0, t,t,t,0,j,j,j,j,-,0,-,"memw(Rx++#s4:2)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, e,e,e,e,e,P,P, 0,1,0, t,t,t,1,-,I,I,I,I,I,I,"memw(Re=#U6)=Nt.new"

            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,0, t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memb(Rx++#s4:0)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,0, t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memb(Rx++#s4:0)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,0, t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memb(Rx++#s4:0)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,0, t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memb(Rx++#s4:0)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,1, t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memh(Rx++#s4:1)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,1, t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memh(Rx++#s4:1)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,1, t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memh(Rx++#s4:1)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,0,1, t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memh(Rx++#s4:1)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,1,0, t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memw(Rx++#s4:2)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,1,0, t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memw(Rx++#s4:2)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,1,0, t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memw(Rx++#s4:2)=Nt.new"
            1,0,1,0, 1,0,1,1, 1,0,1, x,x,x,x,x,P,P, 1,1,0, t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memw(Rx++#s4:2)=Nt.new"
            */
            var decoder_AB_5 = Mask(11, 3, "  0xAB 5...",
                Assign(Mpostinc(PrimitiveType.Byte, 16, (3, 4), 0), New8),
                Nyi("0b001"),
                Assign(Mpostinc(PrimitiveType.Word32, 16, (3, 4), 2), New8),
                invalid,
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,0,1,0, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,0,t,t,t,t,t,0,j,j,j,j,-,0,-,"memd(Rx++#s4:3)=Rtt"
            1,0,1,0, 1,0,1,1, 1,1,0, e,e,e,e,e,P,P,0,t,t,t,t,t,1,-,I,I,I,I,I,I,"memd(Re=#U6)=Rtt"
            1,0,1,0, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,0,v,v,"if (Pv)memd(Rx++#s4:3)=Rtt"
            1,0,1,0, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,0,j,j,j,j,1,v,v,"if (!Pv)memd(Rx++#s4:3)=Rtt"
            1,0,1,0, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memd(Rx++#s4:3)=Rtt"
            1,0,1,0, 1,0,1,1, 1,1,0, x,x,x,x,x,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memd(Rx++#s4:3)=Rtt"
            */
            var decoder_AB_6 = Mask(13, 1, "  AB6",
                Mask(7, 1, "  0b0",
                    Assign(Mpostinc(PrimitiveType.Word64, 16, (3, 4), 3), RR8),
                    Nyi("0b1")),
                Assign(Conditional(0, 7, -1, 2), Mpostinc(PrimitiveType.Word64, 16, (3, 4), 3), RR8));

            var decoder_AB = Mask(21, 3, "  AB",
                decoder_AB_0,
                Nyi("0b001"),
                decoder_AB_2,
                Nyi("0b011"),
                decoder_AB_4,
                decoder_AB_5,
                decoder_AB_6,
                invalid);
            /*
            1,0,1,0, 1,1,0,1, 0,0,0,u,u,u,u,u,P,P,j,t,t,t,t,t,1,j,I,I,I,I,I,I,"memb(Ru<<#u2+#U6)=Rt"
            1,0,1,0, 1,1,0,1, 0,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memb(Rx++Mu)=Rt"
            1,0,1,0, 1,1,0,1, 0,1,0,u,u,u,u,u,P,P,j,t,t,t,t,t,1,j,I,I,I,I,I,I,"memh(Ru<<#u2+#U6)=Rt"
            1,0,1,0, 1,1,0,1, 0,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu)=Rt"
            1,0,1,0, 1,1,0,1, 0,1,1,u,u,u,u,u,P,P,j,t,t,t,t,t,1,j,I,I,I,I,I,I,"memh(Ru<<#u2+#U6)=Rt.H"
            1,0,1,0, 1,1,0,1, 0,1,1,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu)=Rt.H"
            1,0,1,0, 1,1,0,1, 1,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memw(Rx++Mu)=Rt"
            1,0,1,0, 1,1,0,1, 1,0,0,u,u,u,u,u,P,P,j,t,t,t,t,t,1,j,I,I,I,I,I,I,"memw(Ru<<#u2+#U6)=Rt"
            1,0,1,0, 1,1,0,1, 1,0,1,u,u,u,u,u,P,P,j,0,0,t,t,t,1,j,I,I,I,I,I,I,"memb(Ru<<#u2+#U6)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,0,1,u,u,u,u,u,P,P,j,0,1,t,t,t,1,j,I,I,I,I,I,I,"memh(Ru<<#u2+#U6)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,0,1,u,u,u,u,u,P,P,j,1,0,t,t,t,1,j,I,I,I,I,I,I,"memw(Ru<<#u2+#U6)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,0,t,t,t,0,-,-,-,-,-,-,-,"memb(Rx++Mu)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,0,1,x,x,x,x,x,P,P,u,0,1,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,0,1,x,x,x,x,x,P,P,u,1,0,t,t,t,0,-,-,-,-,-,-,-,"memw(Rx++Mu)=Nt.new"
            1,0,1,0, 1,1,0,1, 1,1,0,u,u,u,u,u,P,P,j,t,t,t,t,t,1,j,I,I,I,I,I,I,"memd(Ru<<#u2+#U6)=Rtt"
            1,0,1,0, 1,1,0,1, 1,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memd(Rx++Mu)=Rtt"
            */
            var decoder_AD = Mask(21, 3, "  0xAD...",
                Nyi("0b000"),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Mask(7, 1, "  0b100",
                    Nyi("0"),
                    Assign(Midx(PrimitiveType.Word32,16,bf_13L1_6L1, bf_0L6), R8)),
                Nyi("0b101"),
                Nyi("0b110"),
                invalid);

            /*
            1,0,1,0, 1,1,1,1, 0,0,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memb(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,0,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memb(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,0,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new) memb(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,0,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new) memb(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memb(Rx++Mu:brev)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memh(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memh(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new) memh(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new) memh(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu:brev)=Rt"
            1,0,1,0, 1,1,1,1, 0,1,1,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memh(#u6)=Rt.H"
            1,0,1,0, 1,1,1,1, 0,1,1,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memh(#u6)=Rt.H"
            1,0,1,0, 1,1,1,1, 0,1,1,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memh(#u6)=Rt.H"
            1,0,1,0, 1,1,1,1, 0,1,1,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memh(#u6)=Rt.H"
            1,0,1,0, 1,1,1,1, 0,1,1,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu:brev)=Rt.H"
            1,0,1,0, 1,1,1,1, 1,0,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memw(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 1,0,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memw(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 1,0,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new) memw(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 1,0,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new) memw(#u6)=Rt"
            1,0,1,0, 1,1,1,1, 1,0,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memw(Rx++Mu:brev)=Rt"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,0,0,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memb(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,0,0,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memb(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,0,1,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memh(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,0,1,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memh(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,1,0,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memw(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,0,1,0,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memw(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,0,0,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memb(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,0,0,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memb(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,0,1,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memh(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,0,1,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memh(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,1,0,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new)memw(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,-,-,-,j,j,P,P,1,1,0,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new)memw(#u6)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,x,x,x,x,x,P,P,u,0,0,t,t,t,0,-,-,-,-,-,-,-,"memb(Rx++Mu:brev)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,x,x,x,x,x,P,P,u,0,1,t,t,t,0,-,-,-,-,-,-,-,"memh(Rx++Mu:brev)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,0,1,x,x,x,x,x,P,P,u,1,0,t,t,t,0,-,-,-,-,-,-,-,"memw(Rx++Mu:brev)=Nt.new"
            1,0,1,0, 1,1,1,1, 1,1,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv) memd(#u6)=Rtt"
            1,0,1,0, 1,1,1,1, 1,1,0,-,-,-,j,j,P,P,0,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv) memd(#u6)=Rtt"
            1,0,1,0, 1,1,1,1, 1,1,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,0,v,v,"if (Pv.new) memd(#u6)=Rtt"
            1,0,1,0, 1,1,1,1, 1,1,0,-,-,-,j,j,P,P,1,t,t,t,t,t,1,j,j,j,j,1,v,v,"if (!Pv.new) memd(#u6)=Rtt"
            1,0,1,0, 1,1,1,1, 1,1,0,x,x,x,x,x,P,P,u,t,t,t,t,t,0,-,-,-,-,-,-,-,"memd(Rx++Mu:brev)=Rtt"
            */

            var decoder_A = Mask(24, 4, "  ST 0",
                decoder_A0,
                decoder0A_st,
                decoder_A2,
                decoder0A_st,

                decoder_A4,
                decoder0A_st,
                decoder_A6,
                decoder0A_st,

                decoder_A8,
                Nyi("0b1001"),
                Nyi("0b1010"),
                decoder_AB,

                Nyi("0b1100"),
                decoder_AD,
                Nyi("0b1110"),
                Nyi("0b1111"));

            /*
            1,0,1,1,j,j,j,j,j,j,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=add(Rs,#s16)"
            */
            var decoder_B = Assign(R0, Apply(Mnemonic.add, R16, sw_21L7_5L9));
            /*
            1,1,0,0, 0,0,0,0, 0,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,j,j,j,d,d,d,d,d,"Rdd=valignb(Rtt,Rss,#u3)"
            1,1,0,0, 0,0,0,0, 1,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,j,j,j,d,d,d,d,d,"Rdd=vspliceb(Rss,Rtt,#u3)"

            1,1,0,0, 0,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=extractu(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=shuffeb(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=shuffob(Rtt,Rss)"
            1,1,0,0, 0,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=shuffeh(Rss,Rtt)"

            1,1,0,0, 0,0,0,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vxaddsubw(Rss,Rtt):sat"
            1,1,0,0, 0,0,0,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=vaddhub(Rss,Rtt):sat"
            1,1,0,0, 0,0,0,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vxsubaddw(Rss,Rtt):sat"
            1,1,0,0, 0,0,0,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vxaddsubh(Rss,Rtt):sat"
            1,1,0,0, 0,0,0,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vxsubaddh(Rss,Rtt):sat"
            
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=shuffoh(Rtt,Rss)"
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vtrunewh(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vtrunehb(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vtrunowh(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vtrunohb(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=lfs(Rss,Rtt)"
            
            1,1,0,0, 0,0,0,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=vxaddsubh(Rss,Rtt):rnd:>>1:sat"
            1,1,0,0, 0,0,0,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=vxsubaddh(Rss,Rtt):rnd:>>1:sat"
            1,1,0,0, 0,0,0,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=extract(Rss,Rtt)"
            1,1,0,0, 0,0,0,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=decbin(Rss,Rtt)"
            */
            var decoder_C1 = BitFields(Bf((22, 2), (6, 1)), "  0xC1...",
                Assign(RR0, Mnemonic.extractu, RR16, RR8),
                Assign(RR0, Mnemonic.shuffeb, RR16, RR8),
                Assign(RR0, Mnemonic.shuffob, RR16, RR8),
                Assign(RR0, Mnemonic.shuffeh, RR16, RR8),

                Nyi("  0b0100"),
                Nyi("  0b0101"),
                Nyi("  0b0100"),
                Nyi("  0b0101"),

                Nyi("  0b0100"),
                Nyi("  0b0101"),
                Nyi("  0b0110"),
                Nyi("  0b0111"),

                Nyi("  0b1000"),
                Nyi("  0b1001"),
                Assign(RR0, Mnemonic.extract, RR16, RR8),
                Assign(RR0, Mnemonic.decbin, RR16, RR8));

            /*
            1,1,0,0, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,u,u,d,d,d,d,d,"Rdd=valignb(Rtt,Rss,Pu)"
            1,1,0,0, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,u,u,d,d,d,d,d,"Rdd=vspliceb(Rss,Rtt,Pu)"
            1,1,0,0, 0,0,1,0, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,x,x,d,d,d,d,d,"Rdd=add(Rss,Rtt,Px):carry"
            1,1,0,0, 0,0,1,0, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,x,x,d,d,d,d,d,"Rdd=sub(Rss,Rtt,Px):carry"
            */
            var decoder_C2 = Mask(23, 1, "  0xC2...",
                Assign(RR0, Apply(Mnemonic.valignb, RR8, RR16, P_5L2)),
                Mask(21, 2, "  1",
                    Nyi("vspliceb"),
                    invalid,
                    Assign(RR0, Carry(Apply(Mnemonic.add, RR16, RR8, P_5L2))),
                    Assign(RR0, Carry(Apply(Mnemonic.sub, RR16, RR8, P_5L2)))));
            /*
            1,1,0,0, 0,0,1,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=vasrw(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=vlsrw(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=vaslw(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=vlslw(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=vasrh(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=vlsrh(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=vaslh(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=vlslh(Rss,Rt)"

            1,1,0,0, 0,0,1,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=asr(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=lsr(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=asl(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=lsl(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rdd=vcrotate(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rdd=vcnegh(Rss,Rt)"
            1,1,0,0, 0,0,1,1, 1,1,-,s,s,s,s,s,P,P,j,t,t,t,t,t,1,1,j,d,d,d,d,d,"Rdd=vrcrotate(Rss,Rt,#u2)"
            */
            var decoder_C3 = BitFields(Bf((22, 2), (6, 2)), "  0xC3...",
                Assign(RR0, Mnemonic.vasrw, RR16, R8),
                Assign(RR0, Mnemonic.vlsrw, RR16, R8),
                Assign(RR0, Mnemonic.vaslw, RR16, R8),
                Assign(RR0, Mnemonic.vlslw, RR16, R8),
                Assign(RR0, Mnemonic.vasrh, RR16, R8),
                Assign(RR0, Mnemonic.vlsrh, RR16, R8),
                Assign(RR0, Mnemonic.vaslh, RR16, R8),
                Assign(RR0, Mnemonic.vlslh, RR16, R8),
                Assign(RR0, Mnemonic.asr, RR16, R8),
                Assign(RR0, Mnemonic.lsr, RR16, R8),
                Assign(RR0, Mnemonic.asl, RR16, R8),
                Assign(RR0, Mnemonic.lsl, RR16, R8),
                Nyi("0b1100"),
                Nyi("0b1101"),
                invalid,
                Nyi("0b1111"));
            /*
            1,1,0,0, 0,1,0,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,j,j,j,d,d,d,d,d,"Rd=addasl(Rt,Rs,#u3)"
            */
            var decoder_C4 = Select(bf_21L3, u => u == 0,
                Assign(R0, Mnemonic.addasl, R8, R16, uw_5L3),
                invalid);
                /*
            1,1,0,0, 0,1,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=vasrw(Rss,Rt)"
            1,1,0,0, 0,1,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=cmpyiwh(Rss,Rt):<<1:rnd:sat"
            1,1,0,0, 0,1,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rd=cmpyiwh(Rss,Rt*):<<1:rnd:sat"
            1,1,0,0, 0,1,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=cmpyrwh(Rss,Rt):<<1:rnd:sat"
            1,1,0,0, 0,1,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=cmpyrwh(Rss,Rt*):<<1:rnd:sat"

            1,1,0,0, 0,1,1,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=asr(Rs,Rt):sat"
            1,1,0,0, 0,1,1,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=asl(Rs,Rt):sat"
            1,1,0,0, 0,1,1,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=asr(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rd=lsr(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=asl(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rd=lsl(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=setbit(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rd=clrbit(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=togglebit(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,0,-,j,j,j,j,j,P,P,-,t,t,t,t,t,1,1,j,d,d,d,d,d,"Rd=lsl(#s6,Rt)"
            1,1,0,0, 0,1,1,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=cround(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=round(Rs,Rt)"
            1,1,0,0, 0,1,1,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rd=round(Rs,Rt):sat"

            1,1,0,0, 0,1,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=tstbit(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=!tstbit(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=bitsset(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=!bitsset(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=bitsclr(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,-,-,-,d,d,"Pd=!bitsclr(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,-,-,-,d,d,"Pd=cmpb.gt(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,-,-,-,d,d,"Pd=cmph.eq(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,-,-,-,d,d,"Pd=cmph.gt(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,-,-,-,d,d,"Pd=cmph.gtu(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,-,-,-,d,d,"Pd=cmpb.eq(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,-,-,-,d,d,"Pd=cmpb.gtu(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,-,-,-,d,d,"Pd=sfcmp.ge(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,-,-,-,d,d,"Pd=sfcmp.uo(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,-,-,-,d,d,"Pd=sfcmp.eq(Rs,Rt)"
            1,1,0,0, 0,1,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,-,-,-,d,d,"Pd=sfcmp.gt(Rs,Rt)"
            */
            var decoder_C7 = Mask(22, 2, "  0xC7...",
                Assign(P_0L2, InvertIfSet(21, Apply(Mnemonic.tstbit, R16, R8))),
                Assign(P_0L2, InvertIfSet(21, Apply(Mnemonic.bitsset, R16, R8))),
                Assign(P_0L2, InvertIfSet(21, Apply(Mnemonic.bitsclr, R16, R8))),
                BitFields(Bf((21,1),(5,3)), "  0b11",
                    invalid,
                    invalid,
                    Assign(P_0L2, Mnemonic.cmpb__gt, R16, R8),
                    Assign(P_0L2, Mnemonic.cmph__eq, R16, R8),

                    Assign(P_0L2, Mnemonic.cmph__gt, R16, R8),
                    Assign(P_0L2, Mnemonic.cmph__gtu, R16, R8),
                    Assign(P_0L2, Mnemonic.cmpb__eq, R16, R8),
                    Assign(P_0L2, Mnemonic.cmpb__gtu, R16, R8),

                    Assign(P_0L2, Mnemonic.sfcmp__ge, R16, R8),
                    Assign(P_0L2, Mnemonic.sfcmp__uo, R16, R8),
                    invalid,
                    Assign(P_0L2, Mnemonic.sfcmp__eq, R16, R8),

                    Assign(P_0L2, Mnemonic.sfcmp__gt, R16, R8),
                    invalid,
                    invalid,
                    invalid));


            /*
            1,1,0,0, 1,0,0,0, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,x,x,x,x,x,"Rx=insert(Rs,Rtt)"
            1,1,0,0, 1,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=extractu(Rs,Rtt)"
            1,1,0,0, 1,0,0,1, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rd=extract(Rs,Rtt)"
            1,1,0,0, 1,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,-,-,-,x,x,x,x,x,"Rxx=insert(Rss,Rtt)"
            1,1,0,0, 1,0,1,0, 1,0,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx^=xor(Rss,Rtt)"

            1,1,0,0, 1,0,1,1, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rxx|=asr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rxx|=lsr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rxx|=asl(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rxx|=lsl(Rss,Rt)"

            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,0,x,x,x,x,x,0,0,1,u,u,u,u,u,"Rxx=vrmaxh(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,0,x,x,x,x,x,0,1,0,u,u,u,u,u,"Rxx=vrmaxw(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,0,x,x,x,x,x,1,0,1,u,u,u,u,u,"Rxx=vrminh(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,0,x,x,x,x,x,1,1,0,u,u,u,u,u,"Rxx=vrminw(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,1,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rxx+=vrcnegh(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,1,x,x,x,x,x,0,0,1,u,u,u,u,u,"Rxx=vrmaxuh(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,1,x,x,x,x,x,0,1,0,u,u,u,u,u,"Rxx=vrmaxuw(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,1,x,x,x,x,x,1,0,1,u,u,u,u,u,"Rxx=vrminuh(Rss,Ru)"
            1,1,0,0, 1,0,1,1, 0,0,1, s,s,s,s,s,P,P,1,x,x,x,x,x,1,1,0,u,u,u,u,u,"Rxx=vrminuw(Rss,Ru)"

            1,1,0,0, 1,0,1,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rxx&=asr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rxx&=lsr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rxx&=asl(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rxx&=lsl(Rss,Rt)"

            1,1,0,0, 1,0,1,1, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rxx^=asr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rxx^=lsr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rxx^=asl(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rxx^=lsl(Rss,Rt)"

            1,1,0,0, 1,0,1,1, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rxx-=asr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rxx-=lsr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rxx-=asl(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rxx-=lsl(Rss,Rt)"

            1,1,0,0, 1,0,1,1, 1,0,1, s,s,s,s,s,P,P,j,t,t,t,t,t,-,-,j,x,x,x,x,x,"Rxx+=vrcrotate(Rss,Rt,#u2)"

            1,1,0,0, 1,0,1,1, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rxx+=asr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rxx+=lsr(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rxx+=asl(Rss,Rt)"
            1,1,0,0, 1,0,1,1, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rxx+=lsl(Rss,Rt)"
            */
            var decoder_CB = Mask(21, 3, "  0xCB...",
                Nyi("0b000"),
                Nyi("0b001"),
                Mask(6, 2, "  0b010",
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.asr, RR16, R8)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.lsr, RR16, R8)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.asl, RR16, R8)),
                    Instr(Mnemonic.ANDEQ, RR0, Apply(Mnemonic.lsl, RR16, R8))),
                Mask(6, 2, "  0b011",
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.asr, RR16, R8)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.lsr, RR16, R8)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.asl, RR16, R8)),
                    Instr(Mnemonic.XOREQ, RR0, Apply(Mnemonic.lsl, RR16, R8))),
                Mask(6, 2, "  0b100",
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.asr, RR16, R8)),
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.lsr, RR16, R8)),
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.asl, RR16, R8)),
                    Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.lsl, RR16, R8))),
                Nyi("  vrcrotate"),
                Mask(6, 2, "  0b110",
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.asr, RR16, R8)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.lsr, RR16, R8)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.asl, RR16, R8)),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.lsl, RR16, R8))),
                invalid);
            /*
            1,1,0,0, 1,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rx|=asr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rx|=lsr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rx|=asl(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rx|=lsl(Rs,Rt)"

            1,1,0,0, 1,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rx&=asr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rx&=lsr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rx&=asl(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 0,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rx&=lsl(Rs,Rt)"

            1,1,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rx-=asr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rx-=lsr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rx-=asl(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,0,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rx-=lsl(Rs,Rt)"
            
            1,1,0,0, 1,1,0,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,x,x,x,x,x,"Rx+=asr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,x,x,x,x,x,"Rx+=lsr(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,x,x,x,x,x,"Rx+=asl(Rs,Rt)"
            1,1,0,0, 1,1,0,0, 1,1,-,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,x,x,x,x,x,"Rx+=lsl(Rs,Rt)"
             */
            var decoder_CC = BitFields(Bf((22, 2), (6, 2)), "  0xCC...",
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.asr, R16, R8)),
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.lsr, R16, R8)),
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.asl, R16, R8)),
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.lsl, R16, R8)),

                Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asr, R16, R8)),
                Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.lsr, R16, R8)),
                Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asl, R16, R8)),
                Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.lsl, R16, R8)),

                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.asr, R16, R8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.lsr, R16, R8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.asl, R16, R8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.lsl, R16, R8)),

                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.asr, R16, R8)),
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.lsr, R16, R8)),
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.asl, R16, R8)),
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.lsl, R16, R8)));

            var decoder_C = Mask(24, 4, "  XTYPE 2, 3",
                Nyi("0b0000"),
                decoder_C1,
                decoder_C2,
                decoder_C3,

                decoder_C4,
                Nyi("0b0101"),
                BitFields(Bf((22,2), (6,2)), "  0b0110",
                    Nyi("0b0000"),
                    Nyi("0b0001"),
                    Nyi("0b0010"),
                    Nyi("0b0011"),

                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asr, R16, R8)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.lsr, R16, R8)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.asl, R16, R8)),
                    Instr(Mnemonic.ANDEQ, R0, Apply(Mnemonic.lsl, R16, R8)),

                    Nyi("0b1000"),
                    Nyi("0b1001"),
                    Nyi("0b1010"),
                    Nyi("0b1011"),

                    Nyi("0b1100"),
                    Nyi("0b1101"),
                    Nyi("0b1110"),
                    Nyi("0b1111")),
                decoder_C7,

                Nyi("0b1000"),
                Nyi("0b1001"),
                Nyi("0b1010"),
                decoder_CB,

                decoder_CC,
                Nyi("0b1101"),
                Nyi("0b1110"),
                Nyi("0b1111"));

            /*
            1,1,0,1, 0,0,0,0, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=parity(Rss,Rtt)"
            1,1,0,1, 0,0,0,1, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,u,u,d,d,d,d,d,"Rdd=vmux(Pu,Rss,Rtt)"
            */
            var decoder_D0 = Assign(RR0, Mnemonic.parity, RR16, RR8);
            var decoder_D1 = Assign(RR0, Mnemonic.vmux, P_5L2, RR16, RR8);
            /*
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,-,-,-,d,d,"Pd=vcmpw.eq(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,-,-,-,d,d,"Pd=vcmpw.gt(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,-,-,-,d,d,"Pd=vcmpw.gtu(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,1,-,-,-,d,d,"Pd=vcmph.eq(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,-,-,-,d,d,"Pd=vcmph.gt(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,-,-,-,d,d,"Pd=vcmph.gtu(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,-,-,-,d,d,"Pd=vcmpb.eq(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,-,-,-,d,d,"Pd=vcmpb.gtu(Rss,Rtt)"

            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,0,0,0,-,-,-,d,d,"Pd=any8(vcmpb.eq(Rss,Rtt))"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,0,1,0,-,-,-,d,d,"Pd=vcmpb.gt(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,0,1,1,-,-,-,d,d,"Pd=tlbmatch(Rss,Rt)"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,1,0,0,-,-,-,d,d,"Pd=boundscheck(Rss,Rtt):raw:lo"
            1,1,0,1, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,1,0,1,-,-,-,d,d,"Pd=boundscheck(Rss,Rtt):raw:hi"

            1,1,0,1, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,-,-,-,d,d,"Pd=cmp.eq(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,-,-,-,d,d,"Pd=cmp.gt(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,-,-,-,d,d,"Pd=cmp.gtu(Rss,Rtt)"

            1,1,0,1, 0,0,1,0, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,-,-,-,d,d,"Pd=dfcmp.eq(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,-,-,-,d,d,"Pd=dfcmp.gt(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,-,-,-,d,d,"Pd=dfcmp.ge(Rss,Rtt)"
            1,1,0,1, 0,0,1,0, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,-,-,-,d,d,"Pd=dfcmp.uo(Rss,Rtt)"
            */
            var decoder_D2 = Mask(23, 1, "  0xD2...",
                Mask(13, 1, "  0",
                    Mask(5, 3, "  0",
                        Assign(P_0L2, Mnemonic.vcmpw__eq, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmpw__gt, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmpw__gtu, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmph__eq, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmph__gt, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmph__gtu, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmpb__eq, RR16, RR8),
                        Assign(P_0L2, Mnemonic.vcmpb__gtu, RR16, RR8)),
                    Mask(5, 3, "  1",
                        Assign(P_0L2, Mnemonic.any8, Apply(Mnemonic.vcmpb__eq, RR16, RR8)),
                        invalid,
                        Assign(P_0L2, Mnemonic.vcmpb__gt, RR16, RR8),
                        Assign(P_0L2, Mnemonic.tlbmatch, RR16, R8),
                        Nyi("boundscheck:lo"),
                        Nyi("boundscheck:hi"),
                        invalid,
                        invalid)),
                Mask(21, 2, "  1",
                    Mask(5, 3, "  00",
                        Assign(P_0L2, Mnemonic.cmp__eq, RR16, RR8),
                        invalid,
                        Assign(P_0L2, Mnemonic.cmp__gt, RR16, RR8),
                        invalid,
                        Assign(P_0L2, Mnemonic.cmp__gtu, RR16, RR8),
                        invalid,
                        invalid,
                        invalid),
                    invalid,
                    invalid,
                    Mask(5, 3, "  11",
                        Assign(P_0L2, Apply(Mnemonic.dfcmp__eq, RR16, RR8)),
                        Assign(P_0L2, Apply(Mnemonic.dfcmp__gt, RR16, RR8)),
                        Assign(P_0L2, Apply(Mnemonic.dfcmp__ge, RR16, RR8)),
                        Assign(P_0L2, Apply(Mnemonic.dfcmp__uo, RR16, RR8)),
                        invalid,
                        invalid,
                        invalid,
                        invalid)));

            /*
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vaddub(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vaddub(Rss,Rtt):sat"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vaddh(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vaddh(Rss,Rtt):sat"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vadduh(Rss,Rtt):sat"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vaddw(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vaddw(Rss,Rtt):sat"
            1,1,0,1, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=add(Rss,Rtt)"
            */
            var decoder_D3_0 = Mask(5, 3, "  0xD3 0...",
                Assign(RR0, Mnemonic.vaddub, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vaddub, RR8, RR16))),
                Assign(RR0, Mnemonic.vaddh, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vaddh, RR8, RR16))),
                Assign(RR0, Sat(Apply(Mnemonic.vadduh, RR8, RR16))),
                Assign(RR0, Mnemonic.vaddw, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vaddw, RR8, RR16))),
                Assign(RR0, Mnemonic.add, RR8, RR16));

            /*
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vsubub(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vsubub(Rtt,Rss):sat"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vsubh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vsubh(Rtt,Rss):sat"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vsubuh(Rtt,Rss):sat"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vsubw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vsubw(Rtt,Rss):sat"
            1,1,0,1, 0,0,1,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=sub(Rtt,Rss)"
            */
            var decoder_D3_1 = Mask(5, 3, "  0xD3 1...",
                Assign(RR0, Mnemonic.vsubub, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vsubub, RR8, RR16))),
                Assign(RR0, Mnemonic.vsubh, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vsubh, RR8, RR16))),
                Assign(RR0, Sat(Apply(Mnemonic.vsubuh, RR8, RR16))),
                Assign(RR0, Mnemonic.vsubw, RR8, RR16),
                Assign(RR0, Sat(Apply(Mnemonic.vsubw, RR8, RR16))),
                Assign(RR0, Mnemonic.sub, RR8, RR16));
            /*
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vavgub(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vavgub(Rss,Rtt):rnd"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vavgh(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vavgh(Rss,Rtt):rnd"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vavgh(Rss,Rtt):crnd"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vavguh(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=vavguh(Rss,Rtt):rnd"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vavgw(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vavgw(Rss,Rtt):rnd"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vavgw(Rss,Rtt):crnd"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vavguw(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vavguw(Rss,Rtt):rnd"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=add(Rss,Rtt):sat"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=add(Rss,Rtt):raw:lo"
            1,1,0,1, 0,0,1,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=add(Rss,Rtt):raw:hi"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vnavgh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vnavgh(Rtt,Rss):rnd:sat"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vnavgh(Rtt,Rss):crnd:sat"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vnavgw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rdd=vnavgw(Rtt,Rss):rnd:sat"
            1,1,0,1, 0,0,1,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rdd=vnavgw(Rtt,Rss):crnd:sat"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vminub(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vminh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vminuh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vminw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vminuw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vmaxuw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=min(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=minu(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vmaxub(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vmaxh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vmaxuh(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=vmaxw(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=max(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=maxu(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vmaxb(Rtt,Rss)"
            1,1,0,1, 0,0,1,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=vminb(Rtt,Rss)"

            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=and(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=and(Rtt,~Rss)"
            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=or(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rdd=or(Rtt,~Rss)"
            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=xor(Rss,Rtt)"
            1,1,0,1, 0,0,1,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=modwrap(Rs,Rt)"
            */
            var decoder_D3 = Mask(21, 3, "  0xD3...",
                decoder_D3_0,
                decoder_D3_1,
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Mask(5, 3, "  0b111",
                    Assign(RR0, Apply(Mnemonic.and, RR16, RR8)),
                    Assign(RR0, Apply(Mnemonic.and, RR8, Comp(RR16))),
                    Assign(RR0, Apply(Mnemonic.or, RR16, RR8)),
                    Assign(RR0, Apply(Mnemonic.or, RR8, Comp(RR16))),
                    Assign(RR0, Apply(Mnemonic.xor, RR16, RR8)),
                    invalid,
                    invalid,
                    Assign(R0, Apply(Mnemonic.modwrap, R16, R8))));

            /*
            1,1,0,1, 0,1,0,0, -,-,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rdd=bitsplit(Rs,Rt)"
            */
            var decoder_D4 = Assign(RR0, Mnemonic.bitsplit, R16, R8);
            /*
            1,1,0,1, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=add(Rt.L,Rs.L)"
            1,1,0,1, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rd=add(Rt.L,Rs.H)"
            1,1,0,1, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=add(Rt.L,Rs.L):sat"
            1,1,0,1, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rd=add(Rt.L,Rs.H):sat"

            1,1,0,1, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,-,d,d,d,d,d,"Rd=sub(Rt.L,Rs.L)"
            1,1,0,1, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,-,d,d,d,d,d,"Rd=sub(Rt.L,Rs.H)"
            1,1,0,1, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,-,d,d,d,d,d,"Rd=sub(Rt.L,Rs.L):sat"
            1,1,0,1, 0,1,0,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,-,d,d,d,d,d,"Rd=sub(Rt.L,Rs.H):sat"

            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 0,0,0, d,d,d,d,d,"Rd=add(Rt.L,Rs.L):<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 0,0,1, d,d,d,d,d,"Rd=add(Rt.L,Rs.H):<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 0,1,0, d,d,d,d,d,"Rd=add(Rt.H,Rs.L):<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 0,1,1, d,d,d,d,d,"Rd=add(Rt.H,Rs.H):<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 1,0,0, d,d,d,d,d,"Rd=add(Rt.L,Rs.L):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 1,0,1, d,d,d,d,d,"Rd=add(Rt.L,Rs.H):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 1,1,0, d,d,d,d,d,"Rd=add(Rt.H,Rs.L):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t, 1,1,1, d,d,d,d,d,"Rd=add(Rt.H,Rs.H):sat:<<16"
            */
            var decoder_D5_2 = Mask(5, 3, "  0xD5 2...",
                Assign(R0, Lsl16(Apply(Mnemonic.add, R8_L, R16_L))),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));

            /*
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=sub(Rt.L,Rs.L):<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=sub(Rt.L,Rs.H):<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=sub(Rt.H,Rs.L):<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rd=sub(Rt.H,Rs.H):<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=sub(Rt.L,Rs.L):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rd=sub(Rt.L,Rs.H):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=sub(Rt.H,Rs.L):sat:<<16"
            1,1,0,1, 0,1,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=sub(Rt.H,Rs.H):sat:<<16"

            1,1,0,1, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,-,-,d,d,d,d,d,"Rd=add(Rs,Rt):sat:deprecated"
            1,1,0,1, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,-,-,d,d,d,d,d,"Rd=sub(Rt,Rs):sat:deprecated"
            
            1,1,0,1, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,-,-,d,d,d,d,d,"Rd=min(Rt,Rs)"
            1,1,0,1, 0,1,0,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,-,-,d,d,d,d,d,"Rd=minu(Rt,Rs)"

            1,1,0,1, 0,1,0,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,-,-,d,d,d,d,d,"Rd=max(Rs,Rt)"
            1,1,0,1, 0,1,0,1, 1,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,-,-,d,d,d,d,d,"Rd=maxu(Rs,Rt)"
            
            1,1,0,1, 0,1,0,1, 1,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=parity(Rs,Rt)"
            */
            var decoder_D5 = Mask(21, 3, "  0xD5...",
                Mask(6, 2, "  0b000",
                    Assign(R0, Apply(Mnemonic.add, R8_L, R16_L)),
                    Assign(R0, Apply(Mnemonic.add, R8_L, R16_H)),
                    Nyi("0b10"),
                    Nyi("0b11")),
                Mask(6, 2, "  0b001",
                    Assign(R0, Apply(Mnemonic.sub, R8_L, R16_L)),
                    Assign(R0, Apply(Mnemonic.sub, R8_L, R16_H)),
                    Nyi("0b10"),
                    Nyi("0b11")),
                decoder_D5_2,
                Nyi("0b011"),
                Nyi("0b100"),
                Mask(7, 1, "  0b101",
                    Assign(R0, Apply(Mnemonic.min, R16, R8)),
                    Assign(R0, Apply(Mnemonic.minu, R16, R8))),
                Mask(7, 1, "  0b110",
                    Assign(R0,Apply(Mnemonic.max, R16, R8)),
                    Assign(R0,Apply(Mnemonic.maxu, R16, R8))),
                Nyi("0b111"));

            /*
            1,1,0,1, 0,1,1,0, 0,0,j,-,-,-,-,-,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=sfmake(#u10):pos"
            1,1,0,1, 0,1,1,0, 0,1,j,-,-,-,-,-,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=sfmake(#u10):neg"
            */
            // 1,1,0,1, 0,1,1,1, 0,j,j,s,s,s,s,s,P,P,j,t,t,t,t,t,j,j,j,d,d,d,d,d,"Rd=add(#u6,mpyi(Rs,Rt))"
            var decoder_D7 = Assign(R0, Mnemonic.add, uw_21L2_13L1_5L3, Apply(Mnemonic.mpyi, R16, R8));
            /*
            1,1,0,1, 1,0,0,0, I,j,j,s,s,s,s,s,P,P,j,d,d,d,d,d,j,j,j,I,I,I,I,I,"Rd=add(#u6,mpyi(Rs,#U6))"
            1,1,0,1, 1,0,0,1, 0,0,j,-,-,-,-,-,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=dfmake(#u10):pos"
            1,1,0,1, 1,0,0,1, 0,1,j,-,-,-,-,-,P,P,j,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rdd=dfmake(#u10):neg"
            *//*
            1,1,0,1, 1,0,1,0, 0,0,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx|=and(Rs,#s10)"
            1,1,0,1, 1,0,1,0, 0,1,j,x,x,x,x,x,P,P,j,j,j,j,j,j,j,j,j,u,u,u,u,u,"Rx=or(Ru,and(Rx,#s10))"
            1,1,0,1, 1,0,1,0, 1,0,j,s,s,s,s,s,P,P,j,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx|=or(Rs,#s10)"
            */
            var decoder_DA = Mask(22, 2, "  0xDA...",
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.and, R16, sw_21L1_5L9)),
                Instr(Mnemonic.ASSIGN, R16, Apply(Mnemonic.or, R0, Apply(Mnemonic.and, R16, sw_21L1_5L9))),
                Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.or, R16, sw_21L1_5L9)),
                invalid);
            /*
            1,1,0,1, 1,0,1,1, 0,j,j,s,s,s,s,s,P,P,j,d,d,d,d,d,j,j,j,u,u,u,u,u,"Rd=add(Rs,add(Ru,#s6))"
            1,1,0,1, 1,0,1,1, 1,j,j,s,s,s,s,s,P,P,j,d,d,d,d,d,j,j,j,u,u,u,u,u,"Rd=add(Rs,sub(#u6,Ru))"
            */
            var decoder_DB = Mask(23, 1, "  0xDB...",
                Assign(R8, Mnemonic.add, R16, Apply(Mnemonic.add, R0, sw_21L2_13L1_5L3)),
                Assign(R8, Mnemonic.add, R16, Apply(Mnemonic.sub, uw_21L2_13L1_5L3, R0)));
            /*
            1,1,0,1, 1,1,0,0, 0,0,0, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=vcmpb.eq(Rss,#u8)"
            1,1,0,1, 1,1,0,0, 0,0,0, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=vcmph.eq(Rss,#s8)"
            1,1,0,1, 1,1,0,0, 0,0,0, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,1,0,-,d,d,"Pd=vcmpw.eq(Rss,#s8)"
            1,1,0,1, 1,1,0,0, 0,0,1, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=vcmpb.gt(Rss,#s8)"
            1,1,0,1, 1,1,0,0, 0,0,1, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=vcmph.gt(Rss,#s8)"
            1,1,0,1, 1,1,0,0, 0,0,1, s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,1,0,-,d,d,"Pd=vcmpw.gt(Rss,#s8)"
            1,1,0,1, 1,1,0,0, 0,1,0, s,s,s,s,s,P,P,-,0,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=vcmpb.gtu(Rss,#u7)"
            1,1,0,1, 1,1,0,0, 0,1,0, s,s,s,s,s,P,P,-,0,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=vcmph.gtu(Rss,#u7)"
            1,1,0,1, 1,1,0,0, 0,1,0, s,s,s,s,s,P,P,-,0,j,j,j,j,j,j,j,1,0,-,d,d,"Pd=vcmpw.gtu(Rss,#u7)"
            1,1,0,1, 1,1,0,0, 1,0,0, s,s,s,s,s,P,P,-,0,0,0,j,j,j,j,j,1,0,-,d,d,"Pd=dfclass(Rss,#u5)"
            */
            var decoder_DC = Mask(21, 3, "  DC",
                Nyi("0b000"),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Assign(P_0L2, Apply(Mnemonic.dfclass, RR16, uw_5L5)),
                invalid,
                invalid,
                invalid);
            /*
            1,1,0,1, 1,1,0,1, -,0,0,s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=cmpb.eq(Rs,#u8)"
            1,1,0,1, 1,1,0,1, -,0,0,s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=cmph.eq(Rs,#s8)"
            1,1,0,1, 1,1,0,1, -,0,1,s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=cmpb.gt(Rs,#s8)"
            1,1,0,1, 1,1,0,1, -,0,1,s,s,s,s,s,P,P,-,j,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=cmph.gt(Rs,#s8)"
            1,1,0,1, 1,1,0,1, -,1,0,s,s,s,s,s,P,P,-,0,j,j,j,j,j,j,j,0,0,-,d,d,"Pd=cmpb.gtu(Rs,#u7)"
            1,1,0,1, 1,1,0,1, -,1,0,s,s,s,s,s,P,P,-,0,j,j,j,j,j,j,j,0,1,-,d,d,"Pd=cmph.gtu(Rs,#u7)"
            */
            var decoder_DD = BitFields(Bf((21, 2), (3, 2)), "  0xDD...",
                Assign(P_0L2, Apply(Mnemonic.cmpb__eq, R16, ub_5L8)),
                Assign(P_0L2, Apply(Mnemonic.cmph__eq, R16, sh_5L8)),
                invalid,
                invalid,
                Assign(P_0L2, Apply(Mnemonic.cmpb__gt, R16, sb_5L8)),
                Assign(P_0L2, Apply(Mnemonic.cmph__gt, R16, sh_5L8)),
                invalid,
                invalid,
                Assign(P_0L2, Apply(Mnemonic.cmpb__gtu, R16, ub_5L7)),
                Assign(P_0L2, Apply(Mnemonic.cmph__gtu, R16, uh_5L7)),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid);


            /*
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,0,j,0,0,-,"Rx=and(#u8,asl(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,0,j,0,1,-,"Rx=or(#u8,asl(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,0,j,1,0,-,"Rx=add(#u8,asl(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,0,j,1,1,-,"Rx=sub(#u8,asl(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,1,j,0,0,-,"Rx=and(#u8,lsr(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,1,j,0,1,-,"Rx=or(#u8,lsr(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,1,j,1,0,-,"Rx=add(#u8,lsr(Rx,#U5))"
            1,1,0,1, 1,1,1,0, j,j,j,x,x,x,x,x,P,P,j,I,I,I,I,I,j,j,j,1,j,1,1,-,"Rx=sub(#u8,lsr(Rx,#U5))"
            */
            var decoder_DE = BitFields(bf_3L1_0L2, "  0xDE...",
                Assign(R16, Apply(Mnemonic.and, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.asl, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.or,  uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.asl, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.add, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.asl, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.sub, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.asl, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.and, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.lsr, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.or,  uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.lsr, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.add, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.lsr, R16, uw_7L5))),
                Assign(R16, Apply(Mnemonic.sub, uw_21L3_13L1_4L3_2L1, Apply(Mnemonic.lsr, R16, uw_7L5))));
            /*
            1,1,0,1, 1,1,1,1, 0,j,j,s,s,s,s,s,P,P,j,d,d,d,d,d,j,j,j,u,u,u,u,u,"Rd=add(Ru,mpyi(#u6:2,Rs))"
            1,1,0,1, 1,1,1,1, 1,j,j,s,s,s,s,s,P,P,j,d,d,d,d,d,j,j,j,u,u,u,u,u,"Rd=add(Ru,mpyi(Rs,#u6))"
            */
            var decoder_DF = Mask(23, 1, "  0xDF..",
                Assign(R8, Apply(Mnemonic.add, R0, Apply(Mnemonic.mpyi, uw_21L2_13L1_5L3_2, R16))),
                Assign(R8, Apply(Mnemonic.add, R0, Apply(Mnemonic.mpyi, R16, uw_21L2_13L1_5L3))));

            var decoder_D = Mask(24, 4, "  XTYPE 2, 3",
                decoder_D0,
                decoder_D1,
                decoder_D2,
                decoder_D3,
                Nyi("0b0100"),
                decoder_D5,
                Nyi("0b0110"),
                decoder_D7,
                Nyi("0b1000"),
                Nyi("0b1001"),
                decoder_DA,
                decoder_DB,
                decoder_DC,
                decoder_DD,
                decoder_DE,
                decoder_DF);
            /*
            1,1,1,0, 0,0,0,0, 0,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=+mpyi(Rs,#u8)"
            1,1,1,0, 0,0,0,0, 1,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,d,d,d,d,d,"Rd=-mpyi(Rs,#u8)"
            */
            var decoder_E0 = Mask(23, 1, "  0xE0...",
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.mpyi, R16, uw_5L8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.mpyi, R16, uw_5L8)));      //$EXT
            /*
            1,1,1,0, 0,0,0,1, 0,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx+=mpyi(Rs,#u8)"
            1,1,1,0, 0,0,0,1, 1,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx-=mpyi(Rs,#u8)"
            */
            var decoder_E1 = Mask(23, 1, "  0xE1...",
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.mpyi, R16, uw_5L8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.mpyi, R16, uw_5L8)));
            /*
            1,1,1,0, 0,0,1,0, 0,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx+=add(Rs,#s8)"
            1,1,1,0, 0,0,1,0, 1,-,-,s,s,s,s,s,P,P,0,j,j,j,j,j,j,j,j,x,x,x,x,x,"Rx-=add(Rs,#s8)"
            */
            var decoder_E2 = Mask(23, 1, "  0xE2...",
                Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.add, R16, sw_5L8)),
                Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.add, R16, sw_5L8)));
            /*
            1,1,1,0, 0,0,1,1, 0,0,0,s,s,s,s,s,P,P,-,y,y,y,y,y,-,-,-,u,u,u,u,u,"Ry=add(Ru,mpyi(Ry,Rs))"
            */
            var decoder_E3 = Assign(R8, Apply(Mnemonic.add, R0, Apply(Mnemonic.mpyi, R8, R16)));
            /*
            1,1,1,0, 0,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,0,d,d,d,d,d,"Rdd=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,1,d,d,d,d,d,"Rdd=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,0,d,d,d,d,d,"Rdd=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,1,d,d,d,d,d,"Rdd=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 0,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,0,d,d,d,d,d,"Rdd=mpy(Rs.L,Rt.L)[:<<N]:rnd"
            1,1,1,0, 0,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,1,d,d,d,d,d,"Rdd=mpy(Rs.L,Rt.H)[:<<N]:rnd"
            1,1,1,0, 0,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,0,d,d,d,d,d,"Rdd=mpy(Rs.H,Rt.L)[:<<N]:rnd"
            1,1,1,0, 0,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,1,d,d,d,d,d,"Rdd=mpy(Rs.H,Rt.H)[:<<N]:rnd"
            1,1,1,0, 0,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,0,d,d,d,d,d,"Rdd=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,0,1,d,d,d,d,d,"Rdd=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,0,d,d,d,d,d,"Rdd=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,1,1,d,d,d,d,d,"Rdd=mpyu(Rs.H,Rt.H)[:<<N]"

            1,1,1,0, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, d,d,d,d,d,"Rdd=mpy(Rs,Rt)"
            1,1,1,0, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, d,d,d,d,d,"Rdd=mpyu(Rs,Rt)"

            1,1,1,0, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, d,d,d,d,d,"Rdd=cmpyi(Rs,Rt)"
            1,1,1,0, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, d,d,d,d,d,"Rdd=vmpybsu(Rs,Rt)"
            1,1,1,0, 0,1,0,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, d,d,d,d,d,"Rdd=vmpybu(Rs,Rt)"

            1,1,1,0, 0,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,1,0, d,d,d,d,d,"Rdd=cmpyr(Rs,Rt)"

            1,1,1,0, 0,1,0,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,0,1, d,d,d,d,d,"Rdd=vmpyh(Rs,Rt)[:<<N]:sat"

            1,1,1,0, 0,1,0,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,0, d,d,d,d,d,"Rdd=cmpy(Rs,Rt)[:<<N]:sat"
            1,1,1,0, 0,1,0,1, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,0, d,d,d,d,d,"Rdd=cmpy(Rs,Rt*)[:<<N]:sat"

            1,1,1,0, 0,1,0,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, d,d,d,d,d,"Rdd=pmpyw(Rs,Rt)"
            1,1,1,0, 0,1,0,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, d,d,d,d,d,"Rdd=vpmpyh(Rs,Rt)"
            1,1,1,0, 0,1,0,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, d,d,d,d,d,"Rdd=vmpyhsu(Rs,Rt)[:<<N]:sat"
            */
            var decoder_E5 = Mask(5, 3, "  0xE5...",
                Sparse(21, 3, "  0b000", invalid,
                    (0, Assign(RR0, Mnemonic.mpy, R16, R8)),
                    (2, Assign(RR0, Mnemonic.mpyu, PrimitiveType.UInt64, R16, R8))),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,1,1,0, 0,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx+=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx+=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rxx+=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx-=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx-=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx-=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rxx-=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx+=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx+=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rxx+=mpyu(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx-=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx-=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx-=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 0,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rxx-=mpyu(Rs.H,Rt.H)[:<<N]"

            1,1,1,0, 0,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, x,x,x,x,x,"Rxx+=mpy(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, x,x,x,x,x,"Rxx-=mpy(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, x,x,x,x,x,"Rxx+=mpyu(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 0,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,0, x,x,x,x,x,"Rxx-=mpyu(Rs,Rt)"
                                                  
            1,1,1,0, 0,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, x,x,x,x,x,"Rxx+=cmpyi(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, x,x,x,x,x,"Rxx+=vmpyh(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, x,x,x,x,x,"Rxx+=vmpybu(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,0,1, x,x,x,x,x,"Rxx+=vmpybsu(Rs,Rt)"
                                                  
            1,1,1,0, 0,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 0,1,0, x,x,x,x,x,"Rxx+=cmpyr(Rs,Rt)"
                                                  
            1,1,1,0, 0,1,1,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,0,1, x,x,x,x,x,"Rxx+=vmpyh(Rs,Rt)[:<<N]:sat"
            1,1,1,0, 0,1,1,1, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,0,1, x,x,x,x,x,"Rxx+=vmpyhsu(Rs,Rt)[:<<N]:sat"
                                                  
            1,1,1,0, 0,1,1,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,0, x,x,x,x,x,"Rxx+=cmpy(Rs,Rt)[:<<N]:sat"
            1,1,1,0, 0,1,1,1, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,0, x,x,x,x,x,"Rxx+=cmpy(Rs,Rt*)[:<<N]:sat"
                                                  
            1,1,1,0, 0,1,1,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, x,x,x,x,x,"Rxx^=pmpyw(Rs,Rt)"
            1,1,1,0, 0,1,1,1, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, x,x,x,x,x,"Rxx^=vpmpyh(Rs,Rt)"
            1,1,1,0, 0,1,1,1, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, x,x,x,x,x,"Rxx-=cmpy(Rs,Rt)[:<<N]:sat"
            1,1,1,0, 0,1,1,1, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t, 1,1,1, x,x,x,x,x,"Rxx-=cmpy(Rs,Rt*)[:<<N]:sat"
            */
            var decoder_E7 = Mask(5, 3, "  0xE7...",
                Sparse(21, 3, "  0b000", invalid,
                    (0b000, Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.mpy, RR16, RR8))),
                    (0b001, Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.mpy, RR16, RR8))),
                    (0b010, Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.mpyu, RR16, RR8))),
                    (0b011, Instr(Mnemonic.SUBEQ, RR0, Apply(Mnemonic.mpyu, RR16, RR8)))),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,1,1,0, 1,0,0,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vrcmpyi(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vrcmpyr(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vrmpyh(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vabsdiffw(Rtt,Rss)"
            1,1,1,0, 1,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vrcmpyi(Rss,Rtt*)"
            1,1,1,0, 1,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vraddub(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vrsadub(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 0,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vabsdiffh(Rtt,Rss)"
            1,1,1,0, 1,0,0,0, 0,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vrcmpyr(Rss,Rtt*)"
            1,1,1,0, 1,0,0,0, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vrmpybu(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vabsdiffub(Rtt,Rss)"
            1,1,1,0, 1,0,0,0, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vdmpybsu(Rss,Rtt):sat"
            1,1,1,0, 1,0,0,0, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vrcmpys(Rss,Rtt):<<1:sat:raw:hi"
            1,1,1,0, 1,0,0,0, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rdd=vrmpybsu(Rss,Rtt)"
            1,1,1,0, 1,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rdd=vabsdiffb(Rtt,Rss)"
            1,1,1,0, 1,0,0,0, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vrcmpys(Rss,Rtt):<<1:sat:raw:lo"
            1,1,1,0, 1,0,0,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vdmpy(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vmpyweh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vmpyeh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=vmpywoh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rdd=vrmpywoh(Rss,Rtt)[:<<N]"
            1,1,1,0, 1,0,0,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vmpyweh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,0,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vcmpyr(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=vmpywoh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,0,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rdd=vrmpyweh(Rss,Rtt)[:<<N]"
            1,1,1,0, 1,0,0,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vmpyweuh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rdd=vcmpyi(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=vmpywouh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,0,0, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rdd=vmpyweuh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,0,0, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rdd=vmpywouh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,0,1, 0,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,-,0,1,d,d,d,d,d,"Rd=vradduh(Rss,Rtt)"
            1,1,1,0, 1,0,0,1, 0,-,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=vraddh(Rss,Rtt)"
            1,1,1,0, 1,0,0,1, 1,-,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=vrcmpys(Rss,Rtt):<<1:rnd:sat:raw:hi"
            1,1,1,0, 1,0,0,1, 1,-,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=vrcmpys(Rss,Rtt):<<1:rnd:sat:raw:lo"
            1,1,1,0, 1,0,0,1, N,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,-,0,0,d,d,d,d,d,"Rd=vdmpy(Rss,Rtt)[:<<N]:rnd:sat"
            *//*
            1,1,1,0, 1,0,1,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx+=vrcmpyi(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vrcmpyr(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx+=vrmpyh(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vdmpybsu(Rss,Rtt):sat"
            1,1,1,0, 1,0,1,0, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx+=vmpyeh(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rxx+=vcmpyr(Rss,Rtt):sat"
            1,1,1,0, 1,0,1,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rxx+=vrcmpyi(Rss,Rtt*)"
            1,1,1,0, 1,0,1,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vraddub(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rxx+=vrsadub(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rxx+=vcmpyi(Rss,Rtt):sat"
            1,1,1,0, 1,0,1,0, 0,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vrcmpyr(Rss,Rtt*)"
            1,1,1,0, 1,0,1,0, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vrmpybu(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,e,e,x,x,x,x,x,"Rxx,Pe=vacsh(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rxx+=vrcmpys(Rss,Rtt):<<1:sat:raw:hi"
            1,1,1,0, 1,0,1,0, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rxx+=vrmpybsu(Rss,Rtt)"
            1,1,1,0, 1,0,1,0, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,e,e,d,d,d,d,d,"Rdd,Pe=vminub(Rtt,Rss)"
            1,1,1,0, 1,0,1,0, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rxx+=vrcmpys(Rss,Rtt):<<1:sat:raw:lo"
            1,1,1,0, 1,0,1,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rxx+=vdmpy(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rxx+=vmpyweh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rxx+=vmpyeh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rxx+=vmpywoh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rxx+=vmpyweh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,1,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rxx+=vrmpyweh(Rss,Rtt)[:<<N]"
            1,1,1,0, 1,0,1,0, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rxx+=vmpywoh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,1,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rxx+=vmpyweuh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rxx+=vmpywouh(Rss,Rtt)[:<<N]:sat"
            1,1,1,0, 1,0,1,0, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rxx+=vmpyweuh(Rss,Rtt)[:<<N]:rnd:sat"
            1,1,1,0, 1,0,1,0, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rxx+=vrmpywoh(Rss,Rtt)[:<<N]"
            1,1,1,0, 1,0,1,0, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rxx+=vmpywouh(Rss,Rtt)[:<<N]:rnd:sat"
            */
            var decoder_EA = Mask(21, 3, "  0xEA...",
                Nyi("000"),
                Nyi("001"),
                Mask(5, 3, "  010",
                    Nyi("000"),
                    Instr(Mnemonic.ADDEQ, RR0, Apply(Mnemonic.vraddub, RR16, RR8)),
                    Nyi("010"),
                    Nyi("011"),
                    Nyi("100"),
                    Nyi("101"),
                    Nyi("110"),
                    Nyi("111")),
                Nyi("011"),
                Nyi("100"),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));

            /*
            1,1,1,0, 1,0,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=sfadd(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=sfsub(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=sfmpy(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=sfmax(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 1,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=sfmin(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=sffixupn(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 1,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=sffixupd(Rs,Rt)"
            1,1,1,0, 1,0,1,1, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,e,e,d,d,d,d,d,"Rd,Pe=sfrecipa(Rs,Rt)"
            */
            var decoder_EB = Mask(21, 3, "  0xEB...",
                Sparse(5, 3, "000", Nyi("000"),
                    (0, Assign(R0, Apply(Mnemonic.sfadd, R16, R8))),
                    (1, Assign(R0, Apply(Mnemonic.sfsub, R16, R8)))),
                Nyi("001"),
                If(5, 3, Eq0, Assign(R0, Apply(Mnemonic.sfmpy, R16, R8))),
                Nyi("011"),
                Sparse(5, 3, "100", Nyi("100"),
                    (0, Assign(R0, Apply(Mnemonic.sfmax, R16, R8))),
                    (1, Assign(R0, Apply(Mnemonic.sfmin, R16, R8)))),
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));
            /*
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,0,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.L)[:<<N]:rnd"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.H)[:<<N]:rnd"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.L)[:<<N]:rnd"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.H)[:<<N]:rnd"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.L)[:<<N]:rnd:sat"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,d,d,d,d,d,"Rd=mpy(Rs.L,Rt.H)[:<<N]:rnd:sat"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.L)[:<<N]:rnd:sat"
            1,1,1,0, 1,1,0,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=mpy(Rs.H,Rt.H)[:<<N]:rnd:sat"
            1,1,1,0, 1,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,0,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,d,d,d,d,d,"Rd=mpyu(Rs.H,Rt.H)[:<<N]"
            */
            var mpy_L_L_shN = Assign(R0, Lsl16(Apply(Mnemonic.mpy, R16_L, R8_L)));
            var decoder_EC = Mask(21, 3, "   0xEC...",
                Mask(5, 3, "  000",
                    Assign(R0, Apply(Mnemonic.mpy, R16_L, R8_L)),
                    Nyi("001"),
                    Assign(R0, Apply(Mnemonic.mpy, R16_H, R8_L)),
                    Nyi("011"),
                    Nyi("100"),
                    Nyi("101"),
                    Nyi("110"),
                    Nyi("111")),
                Nyi("001"),
                Nyi("010"),
                Nyi("011"),
                Nyi("100"), // Same as 000, except << 1
                Nyi("101"),
                Nyi("110"),
                Nyi("111"));
            /*
            1,1,1,0, 1,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpyi(Rs,Rt)"
            1,1,1,0, 1,1,0,1, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpy(Rs,Rt.H):<<1:sat"
            1,1,1,0, 1,1,0,1, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,d,d,d,d,d,"Rd=mpy(Rs,Rt):<<1:sat"

            1,1,1,0, 1,1,0,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpy(Rs,Rt)"
            1,1,1,0, 1,1,0,1, 0,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpy(Rs,Rt):rnd"
            1,1,1,0, 1,1,0,1, 0,1,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpyu(Rs,Rt)"
            1,1,1,0, 1,1,0,1, 0,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpysu(Rs,Rt)"
            1,1,1,0, 1,1,0,1, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,d,d,d,d,d,"Rd=mpy(Rs,Rt.L):<<1:sat"
            
            1,1,1,0, 1,1,0,1, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,d,d,d,d,d,"Rd=mpy(Rs,Rt):<<1"
            
            1,1,1,0, 1,1,0,1, 1,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=mpy(Rs,Rt.H):<<1:rnd:sat"
            1,1,1,0, 1,1,0,1, 1,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,d,d,d,d,d,"Rd=mpy(Rs,Rt.L):<<1:rnd:sat"
            
            1,1,1,0, 1,1,0,1, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=cmpy(Rs,Rt)[:<<N]:rnd:sat"
            1,1,1,0, 1,1,0,1, N,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,d,d,d,d,d,"Rd=cmpy(Rs,Rt*)[:<<N]:rnd:sat"
            
            1,1,1,0, 1,1,0,1, N,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,d,d,d,d,d,"Rd=vmpyh(Rs,Rt)[:<<N]:rnd:sat"
            */
            var decoder_ED = Mask(5, 3, "  0xED...",
                Sparse(21, 3, "  0xED 0...", invalid,
                    (0b000, Assign(R0, Apply(Mnemonic.mpyi, R16, R8))),
                    (0b101, Nyi("mpy <<1:sat")),
                    (0b111, Nyi("mpy <<1:sat"))),
                Sparse(21, 3, "  0xED 1", Nyi("  0xED 1"),
                    (0b000, Assign(R0, Apply(Mnemonic.mpy, R16, R8))),
                    (0b010, Assign(R0, Apply(Mnemonic.mpyu, R16, R8)))),
                Nyi("0b010"),
                Nyi("0b011"),
                Nyi("0b100"),
                Nyi("0b101"),
                Nyi("0b110"),
                Nyi("0b111"));
            /*
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx+=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx+=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx+=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx+=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rx+=mpy(Rs.L,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rx+=mpy(Rs.L,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rx+=mpy(Rs.H,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rx+=mpy(Rs.H,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx-=mpy(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx-=mpy(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx-=mpy(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx-=mpy(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rx-=mpy(Rs.L,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rx-=mpy(Rs.L,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rx-=mpy(Rs.H,Rt.L)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rx-=mpy(Rs.H,Rt.H)[:<<N]:sat"
            1,1,1,0, 1,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx+=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx+=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx+=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx+=mpyu(Rs.H,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx-=mpyu(Rs.L,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx-=mpyu(Rs.L,Rt.H)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx-=mpyu(Rs.H,Rt.L)[:<<N]"
            1,1,1,0, 1,1,1,0, N,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx-=mpyu(Rs.H,Rt.H)[:<<N]"

            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx+=mpyi(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx+=add(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx+=sub(Rt,Rs)"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,0,x,x,x,x,x,"Rx+=sfmpy(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,0,1,x,x,x,x,x,"Rx-=sfmpy(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,0,x,x,x,x,x,"Rx+=sfmpy(Rs,Rt):lib"
            1,1,1,0, 1,1,1,1, 0,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,1,1,x,x,x,x,x,"Rx-=sfmpy(Rs,Rt):lib"

            1,1,1,0, 1,1,1,1, 0,0,1, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx|=and(Rs,~Rt)"
            1,1,1,0, 1,1,1,1, 0,0,1, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx&=and(Rs,~Rt)"
            1,1,1,0, 1,1,1,1, 0,0,1, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx^=and(Rs,~Rt)"

            1,1,1,0, 1,1,1,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx&=and(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx&=or(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx&=xor(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 0,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx|=and(Rs,Rt)"

            1,1,1,0, 1,1,1,1, 0,1,1, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx+=mpy(Rs,Rt):<<1:sat"
            1,1,1,0, 1,1,1,1, 0,1,1, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx-=mpy(Rs,Rt):<<1:sat"
            1,1,1,0, 1,1,1,1, 0,1,1, s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,x,x,x,x,x,"Rx+=sfmpy(Rs,Rt,Pu):scale"

            1,1,1,0, 1,1,1,1, 1,0,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx-=add(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 1,0,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx^=xor(Rs,Rt)"

            1,1,1,0, 1,1,1,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,0,x,x,x,x,x,"Rx|=or(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,0,1,x,x,x,x,x,"Rx|=xor(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,0,x,x,x,x,x,"Rx^=and(Rs,Rt)"
            1,1,1,0, 1,1,1,1, 1,1,0, s,s,s,s,s,P,P,0,t,t,t,t,t,0,1,1,x,x,x,x,x,"Rx^=or(Rs,Rt)"
            */
            var decoder_EF = Mask(21, 3, "  0xEF...",
                Mask(5, 3, "  0b000",
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.mpyi, R16,R8)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.add, R16,R8)),
                    invalid,
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.sub, R8, R16)),
                    Instr(Mnemonic.ADDEQ, R0, Apply(Mnemonic.sfmpy, R16,R8)),
                    Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.sfmpy, R16,R8)),
                    Nyi("Instr(Mnemonic.PLUSEQ, R0, Apply(Mnemonic.sfmpy, R16,R8)):lib"),
                    Nyi("Instr(Mnemonic.MINUSEQ, R0, Apply(Mnemonic.sfmpy, R16,R8)):lib")),
                Nyi("0b001"),
                Nyi("0b010"),
                Nyi("0b011"),
                Sparse( 5, 3, "  0b100", invalid,
                    (0b001, Instr(Mnemonic.SUBEQ, R0, Apply(Mnemonic.add, R16, R8))),
                    (0b011, Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.add, R16, R8)))),
                Nyi("0b101"),
                Sparse(5, 3, "  0b110", invalid,
                    (0b000, Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.or, R16, R8))),
                    (0b001, Instr(Mnemonic.OREQ, R0, Apply(Mnemonic.xor, R16, R8))),
                    (0b010, Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.and, R16, R8))),
                    (0b011, Instr(Mnemonic.XOREQ, R0, Apply(Mnemonic.or, R16, R8)))),
                invalid);

            var decoder_E = Mask(24, 4, "  XTYPE 2, 3",
                decoder_E0,
                decoder_E1,
                decoder_E2,
                decoder_E3,
                Nyi("0b0100"),
                decoder_E5,
                Nyi("0b0110"),
                decoder_E7,
                Nyi("0b1000"),
                Nyi("0b1001"),
                decoder_EA,
                decoder_EB,
                decoder_EC,
                decoder_ED,
                Nyi("0b1110"),
                decoder_EF);

            /*
            1,1,1,1, 0,0,0,1, 0,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=and(Rs,Rt)"
            1,1,1,1, 0,0,0,1, 0,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=or(Rs,Rt)"
            1,1,1,1, 0,0,0,1, 0,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=xor(Rs,Rt)"
            1,1,1,1, 0,0,0,1, 1,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=and(Rt,~Rs)"
            1,1,1,1, 0,0,0,1, 1,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=or(Rt,~Rs)"
            */
            var decoder_F1 = Mask(21, 3, "  0b0001",
                Assign(R0, Apply(Mnemonic.and, R16, R8)),
                Assign(R0, Apply(Mnemonic.or, R16, R8)),
                invalid,
                Assign(R0, Apply(Mnemonic.xor, R16, R8)),
                Assign(R0, Apply(Mnemonic.and, R16, Comp(R8))),
                Assign(R0, Apply(Mnemonic.or, R16, Comp(R8))),
                invalid,
                invalid);

            /*
            1,1,1,1, 0,0,1,0, -,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,0,0,0,d,d,"Pd=cmp.eq(Rs,Rt)"
            1,1,1,1, 0,0,1,0, -,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,1,0,0,d,d,"Pd=!cmp.eq(Rs,Rt)"
            1,1,1,1, 0,0,1,0, -,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,0,0,0,d,d,"Pd=cmp.gt(Rs,Rt)"
            1,1,1,1, 0,0,1,0, -,1,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,1,0,0,d,d,"Pd=!cmp.gt(Rs,Rt)"
            1,1,1,1, 0,0,1,0, -,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,0,0,0,d,d,"Pd=cmp.gtu(Rs,Rt)"
            1,1,1,1, 0,0,1,0, -,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,1,0,0,d,d,"Pd=!cmp.gtu(Rs,Rt)"
            */
            var decoder_F2 = Mask(21, 2, "  F2",
                Assign(P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__eq, R16, R8))),
                invalid,
                Assign(P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__gt, R16, R8))),
                Assign(P_0L2, InvertIfSet(4, Apply(Mnemonic.cmp__gtu, R16, R8))));

            /*
            1,1,1,1, 0,0,1,1, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=add(Rs,Rt)"
            1,1,1,1, 0,0,1,1, 0,0,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=sub(Rt,Rs)"
            1,1,1,1, 0,0,1,1, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=cmp.eq(Rs,Rt)"
            1,1,1,1, 0,0,1,1, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=!cmp.eq(Rs,Rt)"
            1,1,1,1, 0,0,1,1, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=combine(Rt.H,Rs.H)"
            1,1,1,1, 0,0,1,1, 1,0,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=combine(Rt.H,Rs.L)"
            1,1,1,1, 0,0,1,1, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=combine(Rt.L,Rs.H)"
            1,1,1,1, 0,0,1,1, 1,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=combine(Rt.L,Rs.L)"
            */
            var decoder_F3 = Mask(21, 3, "  0b0011",
                Assign(R0, Apply(Mnemonic.add, R16, R8)),
                Assign(R0, Apply(Mnemonic.sub, R8, R16)),
                Assign(R0, Apply(Mnemonic.cmp__eq, R16, R8)),
                Assign(R0, InvertIfSet(21, Apply(Mnemonic.cmp__eq, R16, R8))),

                Nyi("combine"),
                Nyi("combine"),
                Nyi("combine"),
                Assign(R0, Apply(Mnemonic.combine, R8_L, R16_L)));


            /*
            1,1,1,1, 0,1,0,0, -,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,u,u,d,d,d,d,d,"Rd=mux(Pu,Rs,Rt)"
            */
            var decoder_F4 = Assign(R0, Mnemonic.mux, P_5L2, R16, R8);

            /*
            1,1,1,1, 0,1,0,1, 0,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rdd=combine(Rs,Rt)"
            1,1,1,1, 0,1,0,1, 1,-,-,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rdd=packhl(Rs,Rt)"
            */
            var decoder_F5 = Mask(23, 1, "  0xF5...",
                Assign(RR0, Apply(Mnemonic.combine, R16, R8)),
                Assign(RR0, Apply(Mnemonic.packhl, R16, R8)));
            /*
            1,1,1,1, 0,1,1,0, 0,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vaddh(Rs,Rt)"
            1,1,1,1, 0,1,1,0, 0,0,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vaddh(Rs,Rt):sat"
            1,1,1,1, 0,1,1,0, 0,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=add(Rs,Rt):sat"
            1,1,1,1, 0,1,1,0, 0,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vadduh(Rs,Rt):sat"
            1,1,1,1, 0,1,1,0, 1,0,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vsubh(Rt,Rs)"
            1,1,1,1, 0,1,1,0, 1,0,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vsubh(Rt,Rs):sat"
            1,1,1,1, 0,1,1,0, 1,1,0, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=sub(Rt,Rs):sat"
            1,1,1,1, 0,1,1,0, 1,1,1, s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vsubuh(Rt,Rs):sat"
            */
            var decoder_F6 = Mask(21, 3, "  0xF6...",
                Assign(R0, Apply(Mnemonic.vaddh, R16, R8)),
                Assign(R0, Sat(Apply(Mnemonic.vaddh, R16, R8))),
                Assign(R0, Apply(Mnemonic.add, R16, R8)),
                Assign(R0, Sat(Apply(Mnemonic.vadduh, R16, R8))),

                Assign(R0, Apply(Mnemonic.vsubh, R8, R16)),
                Assign(R0, Sat(Apply(Mnemonic.vsubh, R8, R16))),
                Assign(R0, Apply(Mnemonic.sub, R8, R16)),
                Assign(R0, Sat(Apply(Mnemonic.vsubuh, R8, R16))));

            /*
            1,1,1,1, 0,1,1,1, -,0,0,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vavgh(Rs,Rt)"
            1,1,1,1, 0,1,1,1, -,0,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vavgh(Rs,Rt):rnd"
            1,1,1,1, 0,1,1,1, -,1,1,s,s,s,s,s,P,P,-,t,t,t,t,t,-,-,-,d,d,d,d,d,"Rd=vnavgh(Rt,Rs)"
            */
            var decoder_F7 = Mask(21, 2, "  0xF7...",
                Assign(R0, Mnemonic.vavgh, R16, R8),
                Assign(R0, Rnd(Apply(Mnemonic.vavgh, R16, R8))),
                invalid,
                Assign(R0, Mnemonic.vnavgh, R8, R16));

            /*
            1,1,1,1, 1,0,0,1, -,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rd=and(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu) Rd=and(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,0,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new) Rd=and(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,0,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new) Rd=and(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rd=or(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu) Rd=or(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,1,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new) Rd=or(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,0,1,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new) Rd=or(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rd=xor(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,1,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu) Rd=xor(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,1,1,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new) Rd=xor(Rs,Rt)"
            1,1,1,1, 1,0,0,1, -,1,1,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new) Rd=xor(Rs,Rt)"
            */
            var decoder_F9 = Mask(21, 2, "  0xF9...",
                Assign(R0, Mnemonic.and, R16, R8, Conditional(5, 13, -1, 7)),
                Assign(R0, Mnemonic.or, R16, R8, Conditional(5, 13, -1, 7)),
                invalid,
                Assign(R0, Mnemonic.xor, R16, R8, Conditional(5, 13, -1, 7)));

            /*
            1,1,1,1, 1,0,1,1, 0,-,0,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rd=add(Rs,Rt)"
            1,1,1,1, 1,0,1,1, 0,-,0,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu) Rd=add(Rs,Rt)"
            1,1,1,1, 1,0,1,1, 0,-,0,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new) Rd=add(Rs,Rt)"
            1,1,1,1, 1,0,1,1, 0,-,0,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new) Rd=add(Rs,Rt)"
            1,1,1,1, 1,0,1,1, 0,-,1,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rd=sub(Rt,Rs)"
            1,1,1,1, 1,0,1,1, 0,-,1,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu) Rd=sub(Rt,Rs)"
            1,1,1,1, 1,0,1,1, 0,-,1,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new) Rd=sub(Rt,Rs)"
            1,1,1,1, 1,0,1,1, 0,-,1,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new) Rd=sub(Rt,Rs)"
            */
            var decoder_FB = Mask(21, 1, "  0xFB...",
                Assign(R0, Apply(Mnemonic.add, R16, R8), Conditional(5, 13, -1, 7)),
                Assign(R0, Apply(Mnemonic.sub, R8, R16), Conditional(5, 13, -1, 7)));
            /*
            1,1,1,1, 1,1,0,1, -,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu) Rdd=combine(Rs,Rt)"
            1,1,1,1, 1,1,0,1, -,-,-,s,s,s,s,s,P,P,0,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu)Rdd=combine(Rs,Rt)"
            1,1,1,1, 1,1,0,1, -,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,0,u,u,d,d,d,d,d,"if (Pu.new)Rdd=combine(Rs,Rt)"
            1,1,1,1, 1,1,0,1, -,-,-,s,s,s,s,s,P,P,1,t,t,t,t,t,1,u,u,d,d,d,d,d,"if (!Pu.new)Rdd=combine(Rs,Rt)"
            */
            var decoder_FD = Assign(RR0, Apply(Mnemonic.combine, R16, R8, Conditional(5, 7, -1, 13)));

            var decoder_F = Mask(24, 4, "  ALU32 0, 1, 2, 3",
                Nyi("0b0000"),
                decoder_F1,
                decoder_F2,
                decoder_F3,

                decoder_F4,
                decoder_F5,
                decoder_F6,
                decoder_F7,

                Nyi("0b1000"),
                decoder_F9,
                Nyi("0b1010"),
                decoder_FB,

                Nyi("0b1100"),
                decoder_FD,
                invalid,
                invalid);


            var simplexDecoder = Mask(28, 4, "  simplex",
                new ExtensionDecoder(
                    invalid,
                    decoder_1,
                    decoder_2,
                    decoder_3,
                    decoder_4,
                    decoder_5,
                    decoder_6,
                    decoder_7,
                    decoder_8,
                    decoder_9,
                    decoder_A,
                    decoder_B,
                    decoder_C,
                    decoder_D,
                    decoder_E,
                    decoder_F),
                decoder_1,
                decoder_2,
                decoder_3,
                decoder_4,
                decoder_5,
                decoder_6,
                decoder_7,
                decoder_8,
                decoder_9,
                decoder_A,
                decoder_B,
                decoder_C,
                decoder_D,
                decoder_E,
                decoder_F);


            iclassDecoder = Mask(14, 2, "HexagonInstruction",
                duplexDecoder,
                simplexDecoder,
                simplexDecoder,
                simplexDecoder);
        }
    }
}
