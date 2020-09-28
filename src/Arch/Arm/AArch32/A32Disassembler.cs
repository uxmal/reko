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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{
    using Decoder = Reko.Core.Machine.Decoder<A32Disassembler, Mnemonic, AArch32Instruction>;

    public partial class A32Disassembler : DisassemblerBase<AArch32Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;
        private static readonly Dictionary<uint, RegisterStorage> bankedRegisters;

        private readonly Arm32Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private DasmState state;

        public A32Disassembler(Arm32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AArch32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
                return null;
            this.state = new DasmState();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = this.addr;
            instr.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            instr.InstructionClass |= instr.condition != ArmCondition.AL ? InstrClass.Conditional : 0;
            instr.Length = 4;
            return instr;
        }

        public class DasmState
        {
            public InstrClass iclass;
            public Mnemonic mnemonic;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public bool updateFlags = false;
            public bool writeback = false;
            public Mnemonic shiftOp = Mnemonic.Invalid;
            public MachineOperand shiftValue = null;
            public bool useQ = false;
            public bool userStmLdm = false;
            public ArmVectorData vectorData;

            public void Clear()
            {
                ops.Clear();
                iclass = InstrClass.Invalid;
                updateFlags = false;
                writeback = false;
                shiftOp = Mnemonic.Invalid;
                shiftValue = null;
                useQ = false;
                userStmLdm = false;
                vectorData = ArmVectorData.INVALID;
            }

            public void Invalid()
            {
                Clear();
                mnemonic = Mnemonic.Invalid;
            }

            public AArch32Instruction MakeInstruction()
            {
                var instr = new A32Instruction
                {
                    InstructionClass = iclass,
                    Mnemonic = mnemonic,
                    Operands = ops.ToArray(),
                    ShiftType = shiftOp,
                    ShiftValue = shiftValue,
                    SetFlags = updateFlags,
                    Writeback = writeback,
                    UserStmLdm = userStmLdm,
                    vector_data = vectorData,
                };
                return instr;
            }
        }

        private ArmVectorData VectorElementInteger(int bitSize)
        {
            switch (bitSize)
            {
            default: throw new ArgumentException(nameof(bitSize), "Bit size must be 8, 16, or 32.");
            case 8: return ArmVectorData.I8;
            case 16: return ArmVectorData.I16;
            case 32: return ArmVectorData.I32;
            case 64: return ArmVectorData.I64;
            }
        }

        private ArmVectorData VectorElementFloat(int bitSize)
        {
            switch (bitSize)
            {
            default: throw new ArgumentException(nameof(bitSize), "Bit size must be 8, 16, or 32.");
            case 16: return ArmVectorData.F16;
            case 32: return ArmVectorData.F32;
            case 64: return ArmVectorData.F64;
            }
        }

        private ArmVectorData VectorElementUntypedReverse(uint imm)
        {
            switch (imm)
            {
            case 0: return ArmVectorData.I32;
            case 1: return ArmVectorData.I16;
            case 2: return ArmVectorData.I8;
            default: return ArmVectorData.INVALID;
            }
        }

        private RegisterOperand Coprocessor(uint wInstr, int bitPos)
        {
            var cp = Registers.Coprocessors[SBitfield(wInstr, bitPos, 4)];
            return new RegisterOperand(cp);
        }

        private RegisterOperand CoprocessorRegister(uint wInstr, int bitPos)
        {
            var cr = Registers.CoprocessorRegisters[SBitfield(wInstr, bitPos, 4)];
            return new RegisterOperand(cr);
        }

        private int SBitfield(uint wInstr, int bitPos, int size)
        {
            return (int) ((wInstr >> bitPos) & ((1u << size) - 1));
        }

        public static ulong SimdExpandImm(uint op, uint cmode, uint imm)
        {
            ulong imm64 = imm;
            switch (cmode)
            {
            case 0:
            case 1:
                imm64 |= imm64 << 32;
                break;
            case 2:
            case 3:
                imm64 = imm64 << 8;
                imm64 |= imm64 << 32;
                break;
            case 4:
            case 5:
                imm64 = imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 6:
            case 7:
                imm64 = imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 8:
            case 9:
                imm64 |= imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 10:
            case 11:
                imm64 = imm64 << 8;
                imm64 |= imm64 << 16;
                imm64 |= imm64 << 32;
                break;
            case 12:
                imm64 = (imm64 << 8) | 0xFF;
                imm64 |= imm64 << 32;
                break;
            case 13:
                imm64 = (imm64 << 16) | 0xFFFF;
                imm64 |= imm64 << 32;
                break;
            case 14:
                if (op == 0)
                {
                    imm64 |= imm64 << 8;
                    imm64 |= imm64 << 16;
                    imm64 |= imm64 << 32;
                }
                else
                {
                    ulong MakeByte(uint n, int bit)
                    {
                        return ((n >> bit) & 1u) == 1u
                            ? (ulong) 0xFF
                            : (ulong) 0;
                    }
                    var imm8a = MakeByte(imm, 7); var imm8b = MakeByte(imm, 6);
                    var imm8c = MakeByte(imm, 5); var imm8d = MakeByte(imm, 4);
                    var imm8e = MakeByte(imm, 3); var imm8f = MakeByte(imm, 2);
                    var imm8g = MakeByte(imm, 1); var imm8h = MakeByte(imm, 0);
                    imm64 =
                        (imm8a << 56) |
                        (imm8b << 48) |
                        (imm8c << 40) |
                        (imm8d << 32) |
                        (imm8e << 24) |
                        (imm8f << 16) |
                        (imm8g << 8) |
                        (imm8h);
                }
                break;
            case 15:
                if (op == 0)
                {
                    uint imm32 = VfpExpandImm32(imm);
                    imm64 = ((ulong) imm32 << 32) | imm32;
                }
                else
                {
                    imm64 = VfpExpandImm64(imm);
                }
                break;
            default:
                throw new NotImplementedException();
            }
            return imm64;
        }

        private static ulong VfpExpandImm64(ulong imm)
        {
            ulong imm64 = (imm & 0xC0) << 56;
            imm64 ^= 0x40000000_00000000u;
            imm64 |= Bits.Replicate64(imm >> 6, 1, 8) << 54;
            imm64 |= (imm & 0x3F) << 48;
            return imm64;
        }

        private static uint VfpExpandImm32(uint imm)
        {
            uint imm32 = (imm & 0xC0) << 24;
            imm32 ^= 0x40000000u;
            imm32 |= (uint) Bits.Replicate64(imm >> 6, 1, 5) << 25;
            imm32 |= (imm & 0x3F) << 19;
            return imm32;
        }

        private AArch32Instruction NotYetImplemented(string message, uint wInstr)
        {
            var hexBytes = wInstr.ToString("X8");
            base.EmitUnitTest("A32", hexBytes, message, "ArmDasm", this.addr,
                w =>
            {
                w.WriteLine($"    Disassemble32(0x{hexBytes});");
                w.WriteLine($"    Expect_Code(\"@@@\");");
            });
            return CreateInvalidInstruction();
        }

        public override AArch32Instruction CreateInvalidInstruction()
        {
            return new A32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = new MachineOperand[0]
            };
        }

        private (Mnemonic, int) DecodeImmShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            int shift_n = (int) bitmask(wInstr, 7, 0x1F);
            Mnemonic shift_t;
            switch (type)
            {
            case 0:
                shift_t = shift_n > 0 ? Mnemonic.lsl : Mnemonic.Invalid;
                break;
            case 1:
                shift_t = Mnemonic.lsr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 2:
                shift_t = Mnemonic.asr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 3:
                shift_t = shift_n > 0 ? Mnemonic.ror : Mnemonic.rrx;
                shift_n = shift_n == 0 ? 1 : shift_n;
                break;
            default:
                throw new InvalidOperationException("impossiburu");
            }
            return (shift_t != Mnemonic.Invalid)
                ? (shift_t, shift_n)
                : (shift_t, 0);
        }

        private (Mnemonic, MachineOperand) DecodeRegShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            var shift_n = Registers.GpRegs[(int) bitmask(wInstr, 8, 0xF)];
            Mnemonic shift_t;
            switch (type)
            {
            case 0:
                shift_t = Mnemonic.lsl;
                break;
            case 1:
                shift_t = Mnemonic.lsr;
                break;
            case 2:
                shift_t = Mnemonic.asr;
                break;
            case 3:
                shift_t = Mnemonic.ror;
                break;
            default:
                throw new InvalidOperationException("impossiburu");
            }
            return (shift_t, new RegisterOperand(shift_n));
        }

        private ImmediateOperand DecodeImm12(uint wInstr)
        {
            var unrotated_value = wInstr & 0xFF;
            var n = Bits.RotateR32(unrotated_value, 2 * (int) bitmask(wInstr, 8, 0xF));
            return ImmediateOperand.Word32(n);
        }

        private static Mutator<A32Disassembler> vu(int bitpos, ArmVectorData v0, ArmVectorData v1)
        {
            return (u, d) => {
                d.state.vectorData = (Bits.IsBitSet(u, bitpos)) ? v1 : v0;
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> u23_I8 = vu(23, S8, U8);
        private static readonly Mutator<A32Disassembler> u23_I16 = vu(23, S16, U16);

        private static Mutator<A32Disassembler> vW(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.vectorData = d.VectorElementUntypedReverse(imm);
                return d.state.vectorData != ArmVectorData.INVALID;
            };
        }

        private static Mutator<A32Disassembler> viu()
        {
            var sizes = new ArmVectorData[][]
            {
                new ArmVectorData[]
                {
                    ArmVectorData.S8,
                    ArmVectorData.S16,
                    ArmVectorData.S32,
                    ArmVectorData.INVALID
                },
                new ArmVectorData[]
                {
                    ArmVectorData.U8,
                    ArmVectorData.U16,
                    ArmVectorData.U32,
                    ArmVectorData.INVALID
                },
            };
            var unsignedField = new Bitfield(24, 1);
            var sizeField = new Bitfield(20, 2);
            return (u, d) =>
            {
                var uf = unsignedField.Read(u);
                var sf = sizeField.Read(u);
                d.state.vectorData = sizes[uf][sf];
                return d.state.vectorData != ArmVectorData.INVALID;
            };
        }

        private static Mutator<A32Disassembler> vi(int offset, int[] sizes)
        {
            return (u, d) => {
                var size = sizes[bitmask(u, offset, 3)];
                if (size == 0)
                    return false;
                d.state.vectorData = d.VectorElementInteger(size);
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> vi_ld1 = vi(10, new[] { 8, 16, 32, 0 });
        private static readonly Mutator<A32Disassembler> vi_ld3 = vi(6, new[] { 8, 16, 32, 0 });
        private static readonly Mutator<A32Disassembler> vi_ld4 = vi(6, new[] { 8, 16, 32, 0 });

        private static Mutator<A32Disassembler> vf(int offset, int length, params ArmVectorData[] sizes)
        {
            var field = new Bitfield(offset, length);
            return (u, d) => {
                var iSize = field.Read(u);
                var size = sizes[iSize];
                if (size == ArmVectorData.INVALID)
                    return false;
                d.state.vectorData = size;
                return true;
            };
        }

        private static Mutator<A32Disassembler> vf20_SD = vf(20, 1, F32, F16);

        /// <summary>
        /// Vector element size specified by bitfields and array of possible values.
        /// </summary>
        private static Mutator<A32Disassembler> ves(Bitfield[] fields, params ArmVectorData[] values)
        {
            return (u, d) =>
            {
                var iValue = Bitfield.ReadFields(fields, u);
                var elemType = values[iValue];
                d.state.vectorData = elemType;
                return elemType != ArmVectorData.INVALID;
            };
        }
        private static readonly Mutator<A32Disassembler> vi_HW_f_HS_ = ves(Bf((8, 1), (20, 2)), INVALID, I16, I32, INVALID,   INVALID, F16, F32, INVALID);

        private static readonly Mutator<A32Disassembler> vis_HW_ = ves(Bf((20, 2)), INVALID, S16, S32, INVALID);
        private static readonly Mutator<A32Disassembler> visBHW_ = ves(Bf((20, 2)), S8, S16, S32, INVALID);
        private static readonly Mutator<A32Disassembler> visBHWD = ves(Bf((20, 2)), S8, S16, S32, S64);
        private static readonly Mutator<A32Disassembler> viHWD_ = ves(Bf((20, 2)), I16, I32, I64, INVALID);
        private static readonly Mutator<A32Disassembler> viBHW_ = ves(Bf((20, 2)), I8, I16, I32, INVALID);
        private static readonly Mutator<A32Disassembler> viBHWD = ves(Bf((20, 2)), I8, I16, I32, I64);
        private static readonly Mutator<A32Disassembler> viBHW_BHW_ = ves(Bf((24, 1), (20, 2)), S8, S16, S32, INVALID, U8, U16, U32, INVALID);
        private static readonly Mutator<A32Disassembler> vi_HW__HW_ = ves(Bf((24, 1), (20, 2)), INVALID, S16, S32, INVALID, INVALID, U16, U32, INVALID);
        /// <summary>
        /// Bit which determines whether or not to use Qx or Dx registers in SIMD
        /// </summary>
        private static Mutator<A32Disassembler> q(int offset)
        {
            return (u, d) => { d.state.useQ = Bits.IsBitSet(u, offset); return true; };
        }
        private static Mutator<A32Disassembler> q6 = q(6);

        /// <summary>
        /// Sets the writeback Bits.IsBitSet.
        /// </summary>
        private static Mutator<A32Disassembler> w(int offset)
        {
            return (u, d) => { d.state.writeback = Bits.IsBitSet(u, offset); return true; };
        }

        // sets user bit (LDM user / STM user)
        private static Mutator<A32Disassembler> u => (u, d) =>
            { d.state.userStmLdm = true; return true; };

        /// <summary>
        /// 12-Bits.IsBitSet encoded immediate at offset 0
        /// </summary>
        private static Mutator<A32Disassembler> I =>
            (u, d) => { d.state.ops.Add(d.DecodeImm12(u)); return true; };

        // 24-bits at offset 0.
        private static Mutator<A32Disassembler> J =>
            (u, d) =>
            {
                var offset = 8 + (((int) u << 8) >> 6);
                d.state.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };

        // 24-bits at offset 0.
        private static Mutator<A32Disassembler> V =>
            (u, d) =>
            {
                var imm = u & 0x00FFFFFF;
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };

        // 24-bits + extra H bit
        private static Mutator<A32Disassembler> X =>
            (u, d) =>
            {
                var offset = 8 + (((int) u << 8) >> 6);
                offset |= ((int) u >> 23) & 2;
                d.state.ops.Add(AddressOperand.Create(d.addr + offset));
                return true;
            };

        // immediate low 12 bits + extra 4 bits
        private static Mutator<A32Disassembler> Y =>
            (u, d) =>
            {
                var imm = (u & 0xFFF) | ((u >> 4) & 0xF000);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };

        // immediate low 12 bits + extra 4 bits
        private static Mutator<A32Disassembler> Yh =>
            (u, d) =>
            {
                var imm = (u & 0xFFF) | ((u >> 4) & 0xF000);
                d.state.ops.Add(ImmediateOperand.Word16((ushort) imm));
                return true;
            };

        // register at a 4-bit multiple offset
        private static Mutator<A32Disassembler> r(int offset)
        {
            offset *= 4;
            return (u, d) => {
                var imm = bitmask(u, offset, 0xF);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm]));
                return true;
            };
        }

        private static readonly Mutator<A32Disassembler> R0 = r(0);
        private static readonly Mutator<A32Disassembler> R12 = r(3);
        private static readonly Mutator<A32Disassembler> R16 = r(4);

        // GP register except PC.
        private static Mutator<A32Disassembler> Rnp(int offset)
        {
            var bf = new Bitfield(offset, 4);
            return (u, d) =>
            {
                var iReg = bf.Read(u);
                if (iReg == 0x0F)
                    return false;
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> Rnp0 = Rnp(0);
        private static readonly Mutator<A32Disassembler> Rnp8 = Rnp(8);
        private static readonly Mutator<A32Disassembler> Rnp12 = Rnp(12);
        private static readonly Mutator<A32Disassembler> Rnp16 = Rnp(16);

        /// <summary>
        /// rp - Register pair
        /// </summary>
        private static Mutator<A32Disassembler> rp(int offset)
        {
            return (u, d) =>
            {
                var imm = bitmask(u, offset, 0xF);
                if ((imm & 1) != 0)
                {
                    d.state.Invalid();
                    return false;
                }
                else
                {
                    d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm]));
                    d.state.ops.Add(new RegisterOperand(Registers.GpRegs[imm + 1]));
                    return true;
                }
            };
        }
        private static Mutator<A32Disassembler> Rp_0 = rp(0);
        private static Mutator<A32Disassembler> Rp_12 = rp(12);

        // Banked register
        private static Mutator<A32Disassembler> rb(int pos1, int size1, int pos2, int size2, int pos3, int size3)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (bankedRegisters.TryGetValue(imm, out var reg))
                {
                    d.state.ops.Add(new RegisterOperand(reg));
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }

        /// <summary>
        /// Vector register, whose size is set by q(<bitpos>)
        /// </summary>
        private static Mutator<A32Disassembler> W(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (d.state.useQ)
                {
                    if ((imm & 1) == 1)
                    {
                        d.state.Invalid();
                        return false;
                    }
                    else
                    {
                        d.state.ops.Add(new RegisterOperand(Registers.QRegs[imm >> 1]));
                    }
                }
                else
                {
                    d.state.ops.Add(new RegisterOperand(Registers.DRegs[imm]));
                }
                return true;
            };
        }

        private readonly static Mutator<A32Disassembler> W5_0 = W(5, 1, 0, 4);
        private readonly static Mutator<A32Disassembler> W7_16 = W(7, 1, 16, 4);
        private readonly static Mutator<A32Disassembler> W22_12 = W(22, 1, 12, 4);

        /// <summary>
        /// Set the SIMD vector index of the most recently added operand.
        /// </summary>
        private static Mutator<A32Disassembler> Ix(params (int pos, int size)[] fieldSpecs)
        {
            var fields = Bf(fieldSpecs);
            return (u, d) =>
            {
                var imm = (int) Bitfield.ReadFields(fields, u);
                int iLastOp = d.state.ops.Count - 1;
                var rLast = (RegisterOperand) d.state.ops[iLastOp];
                var dtElem = Arm32Architecture.VectorElementDataType(d.state.vectorData);
                var ixOp = new IndexedOperand(dtElem, rLast.Register, imm);
                d.state.ops[iLastOp] = ixOp;
                return true;
            };
        }
        private static Mutator<A32Disassembler> Ix(int pos, int size) { return Ix((pos, size)); }


        // Memory accesses //////

        private (MemoryOperand, bool) MakeMemoryOperand(
            uint wInstr,
            RegisterStorage n,
            RegisterStorage m,
            Constant offset,
            Mnemonic shiftType,
            int shiftAmt,
            PrimitiveType dt)
        {
            bool add = Bits.IsBitSet(wInstr, 23);
            bool preIndex = Bits.IsBitSet(wInstr, 24);
            bool wback = Bits.IsBitSet(wInstr, 21);
            bool writeback = !preIndex | wback;
            var mem = new MemoryOperand(dt)
            {
                BaseRegister = n,
                Offset = offset,
                Index = m,
                Add = add,
                PreIndex = preIndex,
                ShiftType = shiftType,
                Shift = shiftAmt,
            };
            return (mem, writeback);
        }

        /// <summary>
        /// Generate simple [R] memory access.
        /// </summary>
        private static Mutator<A32Disassembler> M(int offset, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iReg = bitmask(u, offset * 4, 0xF);
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                // Writeback makes no sense for this addressing mode.
                var (mem, ignoreWriteback) = d.MakeMemoryOperand(u, n, null, null, Mnemonic.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Memory access with 12-bit offset.
        /// </summary>
        private static Mutator<A32Disassembler> Mo(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int) bitmask(u, 0, 0xFFF));
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Mnemonic.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<A32Disassembler> M_(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var m = Registers.GpRegs[bitmask(u, 0, 0x0F)];
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, m, null, Mnemonic.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        // offset split in hi-lo nybbles.
        private static Mutator<A32Disassembler> Mh(PrimitiveType dt, bool allowWriteback = true)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32(
                    (int) (((u >> 4) & 0xF0) | (u & 0x0F)));
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Mnemonic.Invalid, 0, dt);
                if (!allowWriteback && d.state.writeback)
                    return false;
                d.state.ops.Add(mem);
                return true;
            };
        }

        // Memory access with register offset
        private static Mutator<A32Disassembler> Mx(PrimitiveType dt)
        {
            return (wInstr, d) =>
            {
                var n = Registers.GpRegs[bitmask(wInstr, 16, 0xF)];
                var m = Registers.GpRegs[bitmask(wInstr, 0, 0x0F)];
                int shiftAmt;
                Mnemonic shiftType = Mnemonic.Invalid;
                (shiftType, shiftAmt) = d.DecodeImmShift(wInstr);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(wInstr, n, m, null, shiftType, shiftAmt, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        // Memory access with 8-bit immediate offset (possibly shifted)
        private static Mutator<A32Disassembler> Mi(int offsetBits, int shift, PrimitiveType dt)
        {
            var fieldOffset = new Bitfield(0, offsetBits);
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int) fieldOffset.Read(u) << shift);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Mnemonic.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }
        private static Mutator<A32Disassembler> Mi8(int shift, PrimitiveType dt)
        {
            return Mi(8, shift, dt);
        }
        private static Mutator<A32Disassembler> Mi12(int shift, PrimitiveType dt)
        {
            return Mi(12, shift, dt);
        }
        private static PrimitiveType w1 => PrimitiveType.Byte;
        private static PrimitiveType w2 => PrimitiveType.Word16;
        private static PrimitiveType w4 => PrimitiveType.Word32;
        private static PrimitiveType w8 => PrimitiveType.Word64;
        private static PrimitiveType s1 => PrimitiveType.SByte;
        private static PrimitiveType s2 => PrimitiveType.Int16;
        private static PrimitiveType s4 => PrimitiveType.Int32;
        private static PrimitiveType s8 => PrimitiveType.Int64;

        //case '[':
        //    {
        //        int shift = 0;
        //        ++i;
        //        var memType = format[i];
        //        ++i;
        //        if (PeekAndDiscard('<', format, ref i))
        //        {
        //            shift = ReadDecimal(format, ref i);
        //        }
        //        Expect(':', format, ref i);
        //        var dom = format[i];
        //        ++i;
        //        var size = format[i] - '0';
        //        ++i;
        //        var dt = GetDataType(dom, size);
        //        (op, writeback) = DecodeMemoryAccess(u, memType, shift, dt);
        //    }
        //    break;


        // Multiple registers
        private static Mutator<A32Disassembler> Mr(int pos, int size) {
            var bitfields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                d.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, (ushort) imm));
                return true;
            };
        }


        private static Mutator<A32Disassembler> Vel(int count, int step)
        {
            Bitfield[] velFields = new[]
            {
                new Bitfield(22,1),
                new Bitfield(12,4),
            };
            return (u, d) =>
            {
                var baseReg = (int)Bitfield.ReadFields(velFields, u);
                var regs = new RegisterStorage[count];
                uint bitmask = 0;
                for (int i = 0; i < count; ++i)
                {
                    bitmask |= (1u << (baseReg + i * step));
                }
                d.state.ops.Add(new MultiRegisterOperand(
                    Registers.DRegs, 
                    Registers.DRegs[0].DataType, 
                    bitmask)); 
                return true;
            };
        }

        /// <summary>
        /// Multiple double-precision floats
        /// </summary>
        private static Mutator<A32Disassembler> Md(int pos, int size)
        {
            var bitfields = new[]
            {
                new Bitfield(pos, size)
            };
            var baseRegFields = new[]
            {
                new Bitfield(22, 1), new Bitfield(12, 4)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                var baseReg = (int)Bitfield.ReadFields(baseRegFields, u);
                var regs = d.SBitfield(u, 1, 7);
                if(regs == 0 || regs > 16 || (baseReg + regs) > 32)
                {
                    return false;
                }
                var bitmask = (((1u << regs) - 1u) << baseReg);
                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, bitmask));
                return true;
            };
        }


        /// <summary>
        /// Multiple single-precision floats
        /// </summary>
        private static Mutator<A32Disassembler> Ms(int pos, int size)
        {
            var bitfields = new[]
            {
                new Bitfield(pos, size)
            };
            var baseRegFields = new[]
            {
                new Bitfield(12, 4), new Bitfield(22, 1),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                var baseReg = (int) Bitfield.ReadFields(baseRegFields, u);
                var regs = d.SBitfield(u, 0, 8);
                if (regs == 0 || (baseReg + regs) > 32)
                {
                    return false;
                }
                var bitmask = (((1u << regs) - 1u) << baseReg);
                d.state.ops.Add(new MultiRegisterOperand(Registers.SRegs, PrimitiveType.Word32, bitmask));
                return true;
            };
        }

        // Vector element access with alignment.
        private static readonly int []mveAlignments = { 0, 64, 128, 256 };

        private static bool Mve(uint uInstr, A32Disassembler dasm)
        {
            var regBase = Registers.GpRegs[bitmask(uInstr, 16, 0xF)];
            var uIdx = bitmask(uInstr, 0, 0xF);
            var align = mveAlignments[bitmask(uInstr, 4, 0x3)];
            var dt = Registers.DRegs[0].DataType;
            MemoryOperand mop;
            switch (uIdx)
            {
            case 13:
                dasm.state.writeback = true;
                mop = new MemoryOperand(dt)
                {
                    BaseRegister = regBase,
                    Alignment = align,
                };
                break;
            case 15:
                mop = new MemoryOperand(dt)
                {
                    BaseRegister = regBase,
                    Alignment = align
                };
                break;
            default:
                dasm.state.writeback = true;
                mop = new MemoryOperand(dt)
                {
                    BaseRegister = regBase,
                    Index = Registers.GpRegs[uIdx],
                    Alignment = align,
                    Add = true,
                };
                break;
            }
            dasm.state.ops.Add(mop);
            return true;
        }

        private static Mutator<A32Disassembler> SR =>
            (u, d) =>
            {
                var sr = Bits.IsBitSet(u, 22) ? Registers.spsr : Registers.cpsr;
                d.state.ops.Add(new RegisterOperand(sr));
                return true;
            };

        // Single precision register
        private static Mutator<A32Disassembler> S(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SRegs[iReg]));
                return true;
            };
        }
        
        private static readonly Mutator<A32Disassembler> S0_5 = S(0,4,5,1);
        private static readonly Mutator<A32Disassembler> S16_7 = S(16,4,7,1);
        private static readonly Mutator<A32Disassembler> S12_22 = S(12,4,22,1);

        private static Mutator<A32Disassembler> S_pair(int pos1, int pos2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, 4),
                new Bitfield(pos2, 1),
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                if (iReg >= 31)
                    return false;
                d.state.ops.Add(new RegisterOperand(Registers.SRegs[iReg]));
                d.state.ops.Add(new RegisterOperand(Registers.SRegs[iReg + 1]));
                return true;
            };
        }

        private static Mutator<A32Disassembler> D(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.DRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> D5_0 = D(5, 1, 0, 4);
        private static readonly Mutator<A32Disassembler> D7_16 = D(7, 1, 16, 4);
        private static readonly Mutator<A32Disassembler> D22_12 = D(22, 1, 12, 4);

        private static Bitfield[] d7_16_fields = Bf((7, 1), (16, 4));

        private static bool DRegList(uint wInstr, A32Disassembler dasm)
        {
            var len = bitmask(wInstr, 8, 3) + 1;
            var n = (int) Bitfield.ReadFields(d7_16_fields, wInstr);
            var bits = ((1u << (int)len) - 1) << n;
            dasm.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, bits));
            return true;
        }

        private static Mutator<A32Disassembler> Q(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u) >> 1;
                d.state.ops.Add(new RegisterOperand(Registers.QRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> Q5_0 = Q(5, 1, 0, 4);
        private static readonly Mutator<A32Disassembler> Q7_16 = Q(7, 1, 16, 4);
        private static readonly Mutator<A32Disassembler> Q22_12 = Q(22, 1, 12, 4);

        //if (PeekAndDiscard('[', format, ref i))
        //{
        //    // D13[3] - index into sub-element
        //    vector_index = (int)ReadBitfields(u, format, ref i);
        //    Expect(']', format, ref i);
        //}

        // Endianness
        private static Mutator<A32Disassembler> E(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new EndiannessOperand(imm != 0));
                return true;
            };
        }

        /// <summary>
        /// Immediate value
        /// </summary>
        private static Mutator<A32Disassembler> i(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<A32Disassembler> i(int pos, int size, int lshift)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u) << lshift;
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<A32Disassembler> i(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        // Add 1 to the immediate at pos:size.
        private static Mutator<A32Disassembler> i_p1(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm+1));
                return true;
            };
        }

        private static Mutator<A32Disassembler> ih(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(ImmediateOperand.Word16((ushort)imm));
                return true;
            };
        }

        /// <summary>
        /// Generate an immediate whose value is computed by subtracting the value found 
        /// at the field defined by (pos, size) from the unsigned value <paramref name="nFrom"/>.
        /// </summary>
        private static Mutator<A32Disassembler> iFrom(int pos, int size, uint nFrom)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var imm = field.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(nFrom - imm));
                return true;
            };
        }

        /// <summary>
        /// Modified SIMD immediate
        /// </summary>
        private static Mutator<A32Disassembler> Is(int pos1, int size1, int pos2, int size2, int pos3, int size3)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2),
                new Bitfield(pos3, size3),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                var cmode = (u >> 8) & 0xF;
                var op = (u >> 5) & 1;
                d.state.ops.Add(ImmediateOperand.Word64(SimdExpandImm(op, cmode, (uint)imm)));
                return true;
            };
        }

        private static Mutator<A32Disassembler> vfpImm32(int posH, int lenH, int posL, int lenL)
        {
            var fields = new[]
            {
                new Bitfield(posH, lenH),
                new Bitfield(posL, lenL),
            };
            return (u, d) =>
            {
                var imm8 = Bitfield.ReadFields(fields, u);
                var uFloat = VfpExpandImm32(imm8);
                var c = Constant.FloatFromBitpattern(uFloat);
                d.state.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        private static Mutator<A32Disassembler> vfpImm64(int posH, int lenH, int posL, int lenL)
        {
            var fields = new[]
            {
                new Bitfield(posH, lenH),
                new Bitfield(posL, lenL),
            };
            return (u, d) =>
            {
                var imm8 = Bitfield.ReadFields(fields, u);
                var uFloat = (long) VfpExpandImm64(imm8);
                var c = Constant.DoubleFromBitpattern(uFloat);
                d.state.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }

        private static (ArmVectorData, uint)[] vsh_BHWD_table = new[]
        {
           (ArmVectorData.INVALID, 0u),
           (ArmVectorData.I8,  8u),

           (ArmVectorData.I16, 16u),
           (ArmVectorData.I16, 16u),

           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),
           (ArmVectorData.I32, 32u),

           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
           (ArmVectorData.I64, 0u),
        };

        private static (ArmVectorData, uint)[] vsh_HWDD_table = new[]
        {
           (ArmVectorData.INVALID, 0u),
           (ArmVectorData.I8, 16u),

           (ArmVectorData.I16, 32u),
           (ArmVectorData.I16, 32u),

           (ArmVectorData.I32, 64u),
           (ArmVectorData.I32, 64u),
           (ArmVectorData.I32, 64u),
           (ArmVectorData.I32, 64u),

           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
           (ArmVectorData.I64, 64u),
        };

        /// <summary>
        /// Compute size of vector elements from shift amount.
        /// </summary>
        private static Mutator<A32Disassembler> VshImmSize((ArmVectorData,uint)[] vectorShiftImm)
        {
            return (u, d) =>
            {
                var immL_6 = ((u >> 1) & 0x40) | (u >> 16) & 0b111111;
                d.state.vectorData = vectorShiftImm[immL_6 >> 3].Item1;
                return d.state.vectorData != INVALID;
            };
        }
        private static Mutator<A32Disassembler> vsh_BHWD_size = VshImmSize(vsh_BHWD_table);
        private static Mutator<A32Disassembler> vsh_HWDD_size = VshImmSize(vsh_HWDD_table);

        /// <summary>
        /// Compute SIMD shift amount.
        /// </summary>
        private static Mutator<A32Disassembler> VshImm((ArmVectorData, uint)[] vectorShiftImm)
        {
            return (u, d) =>
            {
                var imm6 = (u >> 16) & 0b111111;
                var immL_6 = ((u >> 1) & 0x40) | imm6;
                var imm = imm6 - vectorShiftImm[immL_6 >> 3].Item2;
                d.state.ops.Add(ImmediateOperand.Int32((int) imm));
                return true;
            };
        }
        private static Mutator<A32Disassembler> vsh_BHWD = VshImm(vsh_BHWD_table);

        private static Mutator<A32Disassembler> VshImmRev((ArmVectorData, uint)[] vectorShiftImm)
        {
            return (u, d) =>
            {
                var imm6 = (u >> 16) & 0b111111;
                var immL_6 = ((u >> 1) & 0x40) | imm6;
                var imm = vectorShiftImm[immL_6 >> 3].Item2 - imm6;
                d.state.ops.Add(ImmediateOperand.Int32((int) imm));
                return true;
            };
        }
        private static Mutator<A32Disassembler> vsh_HWDD_rev = VshImmRev(vsh_HWDD_table);

        /// <summary>
        /// Compute vector element type from cmode(0:2)
        /// </summary>
        private static Mutator<A32Disassembler> DtFromCmode(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var cmode0_2 = field.Read(u);
                d.state.vectorData = dtFromCmode[cmode0_2];
                return true;
            };
        }

        private static Mutator<A32Disassembler> Imm(Constant c)
        {
            return (u, d) =>
            {
                d.state.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }
        private static readonly Mutator<A32Disassembler> Imm0_r32 = Imm(Constant.Real32(0));
        private static readonly Mutator<A32Disassembler> Imm0_r64 = Imm(Constant.Real64(0));

        private static ArmVectorData[] dtFromCmode =
        {
            // See p. F6-4271
            ArmVectorData.I32,
            ArmVectorData.I32,
            ArmVectorData.I8,
            ArmVectorData.F32,
        };

        // use bit 20 to determine if sets flags
        private static Mutator<A32Disassembler> s =>
            (u, d) => {
                d.state.updateFlags = ((u >> 20) & 1) != 0;
                return true;
            };

        // Coprocessor #
        private static Mutator<A32Disassembler> CP(int n)
        {
            return (u, d) =>
            {
                d.state.ops.Add(d.Coprocessor(u, n));
                return true;
            };
        }

        // Coprocessor register
        private static Mutator<A32Disassembler> CR(int offset)
        {
            return (u, d) => {
                d.state.ops.Add(d.CoprocessorRegister(u, offset));
                return true;
            };
        }

        // '>i' immediate shift
        private static Mutator<A32Disassembler> Shi =>
            (u, d) =>
            {
                int sh;
                (d.state.shiftOp, sh) = d.DecodeImmShift(u);
                if (d.state.shiftOp != Mnemonic.Invalid)
                {
                    d.state.shiftValue = ImmediateOperand.Int32(sh);
                }
                return true;
            };

        // >R:  rotation as encoded in uxtb / stxb and  friends
        private static Mutator<A32Disassembler> ShR(int pos, int size)
        {
            var bitfields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var offset = (int)Bitfield.ReadFields(bitfields, u);
                if (offset == 0)
                {
                    d.state.shiftOp = Mnemonic.Invalid;
                }
                else
                {
                    d.state.shiftOp = Mnemonic.ror;
                    d.state.shiftValue = ImmediateOperand.Int32(offset << 3);
                }
                return true;
            };
        }

        /// <summary>
        /// Not yet implemented decoder.
        /// </summary>
        private static Mutator<A32Disassembler> x(string message)
        {
            return (u, d) =>
            {
                var op = d.state.mnemonic.ToString();
                string m;
                if (message == "")
                    m = op;
                else
                    m = $"{op} - {message}";
                d.NotYetImplemented(m, u);
                d.CreateInvalidInstruction();
                return false;
            };
        }

        // >r : register shift
        private static Mutator<A32Disassembler> Shr =>
            (u, d) => {
                (d.state.shiftOp, d.state.shiftValue) = d.DecodeRegShift(u);
                return true;
            };


        // Ba => barrier
        private static Mutator<A32Disassembler> Ba(int pos, int size)
        {
            var bitfield = new[] { new Bitfield(pos, size) };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfield, u);
                d.state.ops.Add(new BarrierOperand((BarrierOption)imm));
                return true;
            };
        }

        // BFI / BFC bit field pair. It's encoded as lsb,msb but needs to be 
        // decoded as lsb,width
        private static Mutator<A32Disassembler> B(int lsbPos, int lsbSize, int msbPos, int msbSize)
        {
            var lsbField = new[]
            {
                new Bitfield(lsbPos, lsbSize),
            };
            var msbField = new[]
            {
                new Bitfield(msbPos, msbSize)
            };
            return (u, d) =>
            {
                var lsb = Bitfield.ReadFields(lsbField, u);
                var msb = Bitfield.ReadFields(msbField, u);
                d.state.ops.Add(ImmediateOperand.Int32((int)lsb));
                d.state.ops.Add(ImmediateOperand.Int32((int)(msb - lsb + 1)));
                return true;
            };
        }

        // Alias mutators that morph the disassembled instruction
        // if special cases are present
        private static bool MovToShift(uint wInstr, A32Disassembler dasm)
        {
            if (dasm.state.shiftOp != Mnemonic.Invalid)
            {
                dasm.state.mnemonic = dasm.state.shiftOp;
                dasm.state.ops.Add(dasm.state.shiftValue);
                dasm.state.shiftValue = null;
                dasm.state.shiftOp = Mnemonic.Invalid;
            }
            return true;
        }














        private static Decoder Instr(Mnemonic opcode, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static Decoder Instr(Mnemonic opcode, InstrClass iclass, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, ArmVectorData.INVALID, mutators);
        }

        private static Decoder Instr(Mnemonic opcode, ArmVectorData vec, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, vec, mutators);
        }

        private static Decoder Select(string tag, int shift, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder(tag, shift, mask, predicate, trueDecoder, falseDecoder);
        }

        private static Decoder Select(int shift, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder("", shift, mask, predicate, trueDecoder, falseDecoder);
        }

        protected static NyiDecoder<A32Disassembler, Mnemonic, AArch32Instruction> nyi(string str)
        {
            return new NyiDecoder<A32Disassembler, Mnemonic, AArch32Instruction>(str);
        }

        static A32Disassembler()
        {
            invalid = new InstrDecoder(Mnemonic.Invalid, InstrClass.Invalid, ArmVectorData.INVALID);
            bankedRegisters = new Dictionary<uint, RegisterStorage>
            {
                { 0b000000, Registers.r8_usr },
                { 0b000001, Registers.r9_usr },
                { 0b000010, Registers.r10_usr },
                { 0b000011, Registers.r11_usr },
                { 0b000100, Registers.r12_usr },
                { 0b000101, Registers.sp_usr },
                { 0b000110, Registers.lr_usr },
                { 0b001000, Registers.r8_fiq },
                { 0b001001, Registers.r9_fiq },
                { 0b001010, Registers.r10_fiq },
                { 0b001011, Registers.r11_fiq },
                { 0b001100, Registers.r12_fiq },
                { 0b001101, Registers.sp_fiq },
                { 0b001110, Registers.lr_fiq },
                { 0b010000, Registers.lr_irq },
                { 0b010001, Registers.sp_irq },
                { 0b010010, Registers.lr_svc },
                { 0b010011, Registers.sp_svc },
                { 0b010100, Registers.lr_abt },
                { 0b010101, Registers.sp_abt },
                { 0b010110, Registers.lr_und },
                { 0b010111, Registers.sp_und },
                { 0b011100, Registers.lr_mon },
                { 0b011101, Registers.sp_mon },
                { 0b011110, Registers.elr_hyp },
                { 0b011111, Registers.sp_hyp },
                { 0b101110, Registers.spsr_fiq },
                { 0b110000, Registers.spsr_irq },
                { 0b110010, Registers.spsr_svc },
                { 0b110100, Registers.spsr_abt },
                { 0b110110, Registers.spsr_und },
                { 0b111100, Registers.spsr_mon },
                { 0b111110, Registers.spsr_hyp },
            };

            var LoadStoreExclusive = nyi("LoadStoreExclusive");

            var Stl = Instr(Mnemonic.stl, Rnp0, M(16, w4));
            var Stlex = Instr(Mnemonic.stlex, Rnp12, Rnp0, M(16, w4));
            var Strex = Instr(Mnemonic.strex, Rnp12,Rnp0, M(16, w4));
            var Lda = Instr(Mnemonic.lda, Rnp12, M(16, w4));
            var Ldaex = Instr(Mnemonic.ldaex, Rnp12, M(16, w4));
            var Ldrex = Instr(Mnemonic.ldrex, Rnp12, M(16, w4));

            var Stlexd = Instr(Mnemonic.stlexd, Rnp12, Rp_0, M(16, w8));
            var Strexd = Instr(Mnemonic.strexd, Rnp12, Rp_0, M(16, w8));
            var Ldaexd = Instr(Mnemonic.ldaexd, Rp_12, M(16, w8));
            var Ldrexd = Instr(Mnemonic.ldrexd, Rp_12, M(16, w8));

            var Stlb = Instr(Mnemonic.stlb, Rnp0, M(16, w1));
            var Stlexb = Instr(Mnemonic.stlexb, Rnp12, Rnp0, M(16, w2));
            var Strexb = Instr(Mnemonic.strexb, Rnp12, Rnp0, M(16, w2));
            var Ldab = Instr(Mnemonic.ldab, Rnp12, M(16, w1));
            var Ldaexb = Instr(Mnemonic.ldrexb, Rnp12, M(16, w1));
            var Ldrexb = Instr(Mnemonic.ldaexb, Rnp12, M(16, w1));

            var Stlh = Instr(Mnemonic.stlh, Rnp0, M(16, w2));
            var Stlexh = Instr(Mnemonic.stlexh, Rnp12, Rnp0, M(16, w2));
            var Strexh = Instr(Mnemonic.strexh, Rnp12, Rnp0, M(16, w2));
            var Ldah = Instr(Mnemonic.ldah, Rnp12, M(16, w2));
            var Ldaexh = Instr(Mnemonic.ldrexh, Rnp12, M(16, w2));
            var Ldrexh = Instr(Mnemonic.ldaexh, Rnp12, M(16, w2));

            var SynchronizationPrimitives = Mask(23, 1, "Synchronization primitives",
                Mask(22, 1,
                    Instr(Mnemonic.swp, r(3), r(0), M(4,w4)),     //$TODO: deprecated in ARMv6 and later.
                    Instr(Mnemonic.swpb, r(3), r(0), M(4,w1))),   //$TODO: deprecated in ARMv6 and later.
                Mask(20, 3, 8, 2, "  type:L:ex:ord",
                    Stl,
                    invalid,
                    Stlex,
                    Strex,

                    Lda,
                    invalid,
                    Ldaex,
                    Ldrex,

                    invalid,
                    invalid,
                    Stlexd,
                    Strexd,

                    invalid,
                    invalid,
                    Ldaexd,
                    Ldrexd,

                    Stlb,
                    invalid,
                    Stlexb,
                    Strexb,

                    Ldab,
                    invalid,
                    Ldaexb,
                    Ldrexb,
                    
                    Stlh,
                    invalid,
                    Stlexh,
                    Strexh,

                    Ldah,
                    invalid,
                    Ldaexh,
                    Ldrexh));

            var Mul = Instr(Mnemonic.mul, s,r(4),r(0),r(2));
            var Mla = Instr(Mnemonic.mla, s,r(4),r(0),r(2),r(3));
            var Mls = Instr(Mnemonic.mls, s,r(4),r(0),r(2),r(3));
            var Umaal = Instr(Mnemonic.umaal, s,r(3),r(4),r(0),r(2));
            var Umull = Instr(Mnemonic.umull, s,r(3),r(4),r(0),r(2));
            var Umlal = Instr(Mnemonic.umlal, s,r(3),r(4),r(0),r(2));
            var Smull = Instr(Mnemonic.smull, s,r(3),r(4),r(0),r(2));
            var Smlal = Instr(Mnemonic.smlal, s,r(3),r(4),r(0),r(2));

            var MultiplyAndAccumulate = Mask(20, 4,
               Mul,
               Mul,
               Mla,
               Mla,

               Umaal,
               invalid,
               Mls,
               invalid,

               Umull,
               Umull,
               Umlal,
               Umlal,

               Smull,
               Smull,
               Smlal,
               Smlal);

            // --
            var LdrdRegister = Instr(Mnemonic.ldrd, Rp_12,M_(w8));
            var LdrhRegister = Instr(Mnemonic.ldrh, r(3),M_(w2));
            var LdrsbRegister = Instr(Mnemonic.ldrsb, r(3),M_(s1));
            var LdrshRegister = Instr(Mnemonic.ldrsh, r(3),M_(s2));
            var Ldrht = Instr(Mnemonic.ldrht, r(3),Mh(w2));
            var Ldrsbt = Instr(Mnemonic.ldrsbt, r(3),Mh(s1));
            var Ldrsht = Instr(Mnemonic.ldrsht, r(3),Mh(s2));
            var StrdRegister = Instr(Mnemonic.strd, Rp_12,Mx(w8));
            var StrhRegister = Instr(Mnemonic.strh, r(3),M_(w2));
            var Strht = Instr(Mnemonic.strht, r(3),Mh(w2));

            var LoadStoreDualHalfSbyteRegister = Mask(24, 1,
                Mask(20, 2,
                   Mask(5, 2,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    Mask(5, 2,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister),
                    Mask(5, 2,
                        invalid,
                        Strht,
                        invalid,
                        invalid),
                    Mask(5, 2,
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht)),
                Mask(20, 1,
                    Mask(5, 2,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    Mask(5, 2,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister)));

            var LdrdLiteral = Instr(Mnemonic.ldrd, Rp_12, r(3), Mh(w8, false));
            var LdrhLiteral = Instr(Mnemonic.ldrh, r(3), Mh(w2, false));
            var LdrsbLiteral = Instr(Mnemonic.ldrsb, r(3),Mh(s1, false));
            var LdrshLiteral = Instr(Mnemonic.ldrsh, r(3),Mh(s2, false));
            var StrhImmediate = Instr(Mnemonic.strh, r(3),Mh(w2));
            var LdrdImmediate = Instr(Mnemonic.ldrd, Rp_12,Mh(w8));
            var StrdImmediate = Instr(Mnemonic.strd, Rp_12,Mh(w8));
            var LdrhImmediate = Instr(Mnemonic.ldrh, r(3),Mh(w2));
            var LdrsbImmediate = Instr(Mnemonic.ldrsb, r(3),Mh(s1));
            var LdrshImmediate = Instr(Mnemonic.ldrsh, r(3),Mh(s2));

            var LoadStoreDualHalfSbyteImmediate = Mask(Bf((24, 1), (20, 2)), // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1"LoadStoreDualHalfSbyteImmediate",
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=000 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=001 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=010 op2
                        invalid,
                        Strht,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        invalid),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=011 op2
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=100 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=101 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=110 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=111 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder("  0b111", 16, LdrshImmediate, LdrshLiteral)));

            var LoadStoreDualHalfSbyteImmediate2 = new CustomDecoder((wInstr, dasm) =>
            {
                var rn = bitmask(wInstr, 16, 0xF);
                var pw = bitmask(wInstr, 23, 2) | bitmask(wInstr, 21, 1);
                var o1 = bitmask(wInstr, 20, 1);
                var op2 = bitmask(wInstr, 5, 3);
                if (rn == 0xF)
                {
                    if (o1 == 0)
                    {
                        if (op2 == 2)
                            return LdrdLiteral;
                    }
                    else
                    {
                        if (pw != 1)
                        {
                            Mask(5, 2,
                                invalid,
                                LdrhLiteral,
                                LdrsbLiteral,
                                LdrshLiteral);
                        }
                        else
                        {
                            return invalid;
                        }
                    }
                }
                switch ((pw << 1) | o1)
                {
                case 0:
                    return Mask(5, 2,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 1:
                    return Mask(5, 2,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 2:
                    return Mask(5, 2,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 3:
                    return Mask(5, 2,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 4:
                    return Mask(5, 2,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 5:
                    return Mask(5, 2,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 6:
                    return Mask(5, 2,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 7:
                    return Mask(5, 2,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                }
                throw new InvalidOperationException("Impossible");
            });

            var ExtraLoadStore = Mask(22, 1, "ExtraLoadStore",
                LoadStoreDualHalfSbyteRegister,
                LoadStoreDualHalfSbyteImmediate);

            var Mrs = Instr(Mnemonic.mrs, r(3),SR);
            var Msr = Instr(Mnemonic.msr, SR,r(0));
            var MrsBanked = Instr(Mnemonic.mrs, rb(22,1,8,1,16,4), r(0));
            var MsrBanked = Instr(Mnemonic.msr, rb(22,1,8,1,16,4), r(0));
            var MoveSpecialRegister = Mask(21, 1,
                Mask(9, 1,
                    Mrs,
                    MrsBanked),
                Mask(9, 1,
                    Msr,
                    MsrBanked));

            var CyclicRedundancyCheck = Mask(21, 2,
                Mask(9, 1,
                    Instr(Mnemonic.crc32b, Rnp12, Rnp16, Rnp0),
                    Instr(Mnemonic.crc32cb, Rnp12, Rnp16, Rnp0)),
                Mask(9, 1,
                    Instr(Mnemonic.crc32h, Rnp12, Rnp16, Rnp0),
                    Instr(Mnemonic.crc32ch, Rnp12, Rnp16, Rnp0)),
                Mask(9, 1,
                    Instr(Mnemonic.crc32w, Rnp12, Rnp16, Rnp0),
                    Instr(Mnemonic.crc32cw, Rnp12, Rnp16, Rnp0)),
                invalid);

            var Qadd = Instr(Mnemonic.qadd, r(3),r(0),r(4));
            var Qsub = Instr(Mnemonic.qsub, r(3),r(0),r(4));
            var Qdadd = Instr(Mnemonic.qdadd, r(3),r(0),r(4));
            var Qdsub = Instr(Mnemonic.qdsub, r(3),r(0),r(4));
            var IntegerSaturatingArithmetic = Mask(21, 2,
                Qadd,
                Qsub,
                Qdadd,
                Qdsub);

            var Hlt = Instr(Mnemonic.hlt, i(8, 12, 0, 4));
            var Bkpt = Instr(Mnemonic.bkpt, i(8,12,0,4));
            var Hvc = Instr(Mnemonic.hvc, i(8,12,0,4));
            var Smc = Instr(Mnemonic.smc, i(0,4));
            var ExceptionGeneration = Mask(21, 2,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = Instr(Mnemonic.bx, InstrClass.Transfer, r(0));
            var Bxj = Instr(Mnemonic.bxj, InstrClass.Transfer, r(0));
            var Blx = Instr(Mnemonic.blx, J);
            var Clz = Instr(Mnemonic.clz, r(3),r(0));
            var Eret = Instr(Mnemonic.eret, InstrClass.Transfer);

            var ChangeProcessState = Mask(16, 1, "Change Process State", 
                Mask(17, 3, "  imod:M",
                    invalid,
                    Instr(Mnemonic.cps, i(0, 5)),
                    invalid,
                    invalid,

                    invalid, 
                    Instr(Mnemonic.cpsie, i(0, 5)),
                    Instr(Mnemonic.cpsid, i(0, 5)),
                    Instr(Mnemonic.cpsid, i(0, 5))),
                Select(4, 1, n => n == 0, Instr(Mnemonic.setend, E(9,1)), invalid));

            var UncMiscellaneous = Mask(22, 3,   // op0
                invalid,
                invalid,
                invalid,
                invalid,

                Mask(20, 2,
                    Select(5, 1, n => n == 0, ChangeProcessState, invalid),
                    Select(4, 0xF, n => n == 0, Instr(Mnemonic.setpan, x("")), invalid),
                    invalid,
                    invalid),
                invalid,
                invalid,
                invalid);

        var Miscellaneous = Mask(21, 2,   // op0
            Mask(4, 3, // op1
                MoveSpecialRegister,
                invalid,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            Mask(4, 3, // op1
                MoveSpecialRegister,
                Bx,
                Bxj,
                Instr(Mnemonic.blx, r(0)),

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            Mask(4, 3, // op1
                MoveSpecialRegister,
                invalid,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            Mask(4, 3, // op1
                MoveSpecialRegister,
                Clz,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                Eret,
                ExceptionGeneration));

            var HalfwordMultiplyAndAccumulate = Mask(21, 2,
                Mask(5, 2,      // M:N
                    Instr(Mnemonic.smlabb, r(4),r(0),r(2),r(3)),
                    Instr(Mnemonic.smlatb, r(4),r(0),r(2),r(3)),
                    Instr(Mnemonic.smlabt, r(4),r(0),r(2),r(3)),
                    Instr(Mnemonic.smlatt, r(4),r(0),r(2),r(3))),
                Mask(5, 2,
                    Instr(Mnemonic.smlawb, r(4),r(0),r(2),r(3)),
                    Instr(Mnemonic.smulwb, r(4),r(0),r(2)),
                    Instr(Mnemonic.smlawt, r(4),r(0),r(2),r(3)),
                    Instr(Mnemonic.smulwt, r(4),r(0),r(2))),
                Mask(5, 2,
                    Instr(Mnemonic.smlalbb, r(3),r(4),r(0),r(2)),
                    Instr(Mnemonic.smlaltb, r(3),r(4),r(0),r(2)),
                    Instr(Mnemonic.smlalbt, r(3),r(4),r(0),r(2)),
                    Instr(Mnemonic.smlaltt, r(3),r(4),r(0),r(2))),
                Mask(5, 2,
                    Instr(Mnemonic.smulbb, r(4),r(0),r(2)),
                    Instr(Mnemonic.smultb, r(4),r(0),r(2)),
                    Instr(Mnemonic.smulbt, r(4),r(0),r(2)),
                    Instr(Mnemonic.smultt, r(4),r(0),r(2))));

            var IntegerDataProcessingImmShift = Mask(21, 3,
                Instr(Mnemonic.and, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.eor, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.sub, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.rsb, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.add, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.adc, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.sbc, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.rsc, s,r(3),r(4),r(0),Shi));

            var IntegerTestAndCompareImmShift = Mask(21, 2,
                Instr(Mnemonic.tst, r(4),r(0),Shi),
                Instr(Mnemonic.teq, r(4),r(0),Shi),
                Instr(Mnemonic.cmp, r(4),r(0),Shi),
                Instr(Mnemonic.cmn, r(4),r(0),Shi));

            var LogicalArithmeticImmShift = Mask(21, 2,
                Instr(Mnemonic.orr, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.mov, s,r(3),r(0),Shi,MovToShift),
                Instr(Mnemonic.bic, s,r(3),r(4),r(0),Shi),
                Instr(Mnemonic.mvn, s,r(3),r(0),Shi));

            var DataProcessingImmediateShift = Mask(23, 2,
                IntegerDataProcessingImmShift, // 3 reg, imm shift
                IntegerDataProcessingImmShift,
                IntegerTestAndCompareImmShift,
                LogicalArithmeticImmShift);

            var IntegerDataProcessingRegShift = Mask(21, 3,
               Instr(Mnemonic.and, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.eor, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.sub, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.rsb, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.add, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.adc, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.sbc, s,r(3),r(4),r(0),Shr),
               Instr(Mnemonic.rsc, s,r(3),r(4),r(0),Shr));

            var IntegerTestAndCompareRegShift = Mask(21, 2,
                Instr(Mnemonic.tst, r(4),r(0),Shr),
                Instr(Mnemonic.teq, r(4),r(0),Shr),
                Instr(Mnemonic.cmp, r(4),r(0),Shr),
                Instr(Mnemonic.cmn, r(4),r(0),Shr));

            var LogicalArithmeticRegShift = Mask(21, 2,
                Instr(Mnemonic.orr, s,r(3),r(4),r(0),Shr),
                Instr(Mnemonic.mov, s,r(4),r(0),Shr),
                Instr(Mnemonic.bic, s,r(3),r(4),r(0),Shr),
                Instr(Mnemonic.mvn, s,r(4),r(0),Shr));

            var DataProcessingRegisterShift = Mask(23, 2,
                IntegerDataProcessingRegShift,
                IntegerDataProcessingRegShift,
                IntegerTestAndCompareRegShift,
                LogicalArithmeticRegShift);

            var IntegerDataProcessingTwoRegImm = Mask(21, 3,
                Instr(Mnemonic.and, s,r(3),r(4),I),
                Instr(Mnemonic.eor, s,r(3),r(4),I),
                Instr(Mnemonic.sub, s,r(3),r(4),I),
                Instr(Mnemonic.rsb, s,r(3),r(4),I),
                Instr(Mnemonic.add, s,r(3),r(4),I),
                Instr(Mnemonic.adc, s,r(3),r(4),I),
                Instr(Mnemonic.sbc, s,r(3),r(4),I),
                Instr(Mnemonic.rsc, s,r(3),r(4),I));

            var LogicalArithmeticTwoRegImm = Mask(21, 2,
                Instr(Mnemonic.orr, s,r(3),r(4),I),
                Instr(Mnemonic.mov, s,r(3),I),
                Instr(Mnemonic.bic, s,r(3),r(4),I),
                Instr(Mnemonic.mvn, s,r(3),I));

            var MoveHalfwordImm = Mask(22, 1,
               Instr(Mnemonic.mov, r(3),Y),
               Instr(Mnemonic.movt, r(3),Yh));

            var IntegerTestAndCompareOneRegImm = Mask(21, 2,
                Instr(Mnemonic.tst, r(4),I),
                Instr(Mnemonic.teq, r(4),I),
                Instr(Mnemonic.cmp, r(4),I),
                Instr(Mnemonic.cmn, r(4),I));

            var MsrImmediate = Instr(Mnemonic.msr, SR,i(0,12));
            var Nop = Instr(Mnemonic.nop);
            var Yield = Instr(Mnemonic.yield);
            var Wfe = Instr(Mnemonic.wfe);
            var Wfi = Instr(Mnemonic.wfi);
            var Sev = Instr(Mnemonic.sev);
            var Sevl = Instr(Mnemonic.sevl);
            var ReservedNop = Instr(Mnemonic.nop);
            var Esb = Instr(Mnemonic.esb, x(""));
            var Dbg = Instr(Mnemonic.dbg, x(""));

            var MoveSpecialRegisterAndHints = new CustomDecoder((wInstr, dasm) =>
            {
                var imm12 = bitmask(wInstr, 0, 0xFF);
                var imm4 = bitmask(wInstr, 16, 0xF);
                var r_iim4 = (bitmask(wInstr, 22, 1) << 4) | imm4;
                if (r_iim4 != 0)
                    return MsrImmediate;
                switch (imm12 >> 4)
                {
                case 0:
                    switch (imm12 & 0xF)
                    {
                    case 0: return Nop;
                    case 1: return Yield;
                    case 2: return Wfe;
                    case 3: return Wfi;
                    case 4: return Sev;
                    case 5: return Sevl;
                    default: return ReservedNop;
                    }
                case 1:
                    switch (imm12 & 0x0F)
                    {
                    case 0: return Esb;
                    default: return ReservedNop;
                    }
                case 0xF: return Dbg;
                default: return ReservedNop;
                }
            });

            var DataProcessingImmediate = Mask(23, 2, "Data processing immediate",
                IntegerDataProcessingTwoRegImm,
                IntegerDataProcessingTwoRegImm,
                Mask(20, 2,
                    MoveHalfwordImm,
                    IntegerTestAndCompareOneRegImm,
                    MoveSpecialRegisterAndHints,
                    IntegerTestAndCompareOneRegImm),
                LogicalArithmeticTwoRegImm);

            var DataProcessingAndMisc = Mask(25, 1, "Data-processing and miscellaneous instructions op0",
                Mask(7, 1, 4, 1, "  op0=0 op2:op4",
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingImmediateShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingRegisterShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        HalfwordMultiplyAndAccumulate,
                        DataProcessingImmediateShift),
                    Mask(5, 2, "  op2:op4=11 op3",
                        Mask(24, 1, // DataProcessingAndMisc op0=0 op2=1 op4=1 op3=0b00 op1
                            MultiplyAndAccumulate,
                            SynchronizationPrimitives),
                        ExtraLoadStore,
                        ExtraLoadStore,
                        ExtraLoadStore)),
                DataProcessingImmediate);

            var LdrLiteral = Instr(Mnemonic.ldr, r(3),Mo(w4));
            var LdrbLiteral = Instr(Mnemonic.ldrb, r(3),Mo(w1));
            var StrImm = Instr(Mnemonic.str, r(3),Mo(w4));
            var LdrImm = Instr(Mnemonic.ldr, r(3),Mo(w4));
            var StrbImm = Instr(Mnemonic.strb, r(3),Mo(w1));
            var LdrbImm = Instr(Mnemonic.ldrb, r(3),Mo(w1));
            
            var LoadStoreWordUnsignedByteImmLit = Mask(Bf((24, 1),(21, 1), (22, 1), (20, 1)),
                // PW=0b00 00
                Instr(Mnemonic.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldr, r(3),Mo(w4)),
                    Instr(Mnemonic.ldr, r(3),Mo(w4))),
                Instr(Mnemonic.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldrb, r(3),Mo(w1)),
                    Instr(Mnemonic.ldrb, r(3),Mo(w1))),

                Instr(Mnemonic.strt, r(3),Mo(w4)),
                Instr(Mnemonic.ldrt, r(3),Mo(w4)),
                Instr(Mnemonic.strbt, r(3),Mo(w1)),
                Instr(Mnemonic.ldrbt, r(3),Mo(w1)),

                Instr(Mnemonic.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldr, r(3),Mo(w4)),
                    Instr(Mnemonic.ldr, r(3),Mo(w4))),
                Instr(Mnemonic.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldrb, r(3),Mo(w1)),
                    Instr(Mnemonic.ldrb, r(3),Mo(w1))),

                Instr(Mnemonic.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldr, r(3),Mo(w4)),
                    Instr(Mnemonic.ldr, r(3),Mo(w4))),
                Instr(Mnemonic.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Mnemonic.ldrb, r(3),Mo(w1)),
                    Instr(Mnemonic.ldrb, r(3),Mo(w1))));

            var StrReg = Instr(Mnemonic.str, r(3),Mx(w4));
            var LdrReg = Instr(Mnemonic.ldr, r(3),Mx(w4));
            var StrbReg = Instr(Mnemonic.strb, r(3),Mx(w1));
            var LdrbReg = Instr(Mnemonic.ldrb, r(3),Mx(w1));
            var StrtReg = Instr(Mnemonic.strt, r(3),Mx(w4));
            var LdrtReg = Instr(Mnemonic.strt, r(3),Mx(w4));
            var StrbtReg = Instr(Mnemonic.strbt, r(3),Mx(w1));
            var LdrbtReg = Instr(Mnemonic.strbt, r(3),Mx(w1));
            var LoadStoreWordUnsignedByteRegister = Mask(Bf((24, 1), (20, 3)),  // P:o2:W:o1
                 StrReg,
                 LdrReg,
                 StrtReg,
                 LdrtReg,

                 StrbReg,
                 LdrbReg,
                 StrbtReg,
                 LdrbtReg,

                 StrReg,
                 LdrReg,
                 StrReg,
                 LdrReg,

                 StrbReg,
                 LdrbReg,
                 StrbReg,
                 LdrbReg);

            var Sadd16 = Instr(Mnemonic.sadd16, Rnp12, Rnp16, Rnp0);
            var Sasx = Instr(Mnemonic.sasx, Rnp12,Rnp16,Rnp0);
            var Ssax = Instr(Mnemonic.ssax, Rnp12, Rnp16, Rnp0);
            var Ssub16 = Instr(Mnemonic.ssub16, Rnp12, Rnp16, Rnp0);
            var Sadd8 = Instr(Mnemonic.sadd8, Rnp12, Rnp16, Rnp0);
            var Ssub8 = Instr(Mnemonic.ssub8, Rnp12, Rnp16, Rnp0);
            var Qadd16 = Instr(Mnemonic.qadd16, Rnp12, Rnp16, Rnp0);
            var Qadd8 = Instr(Mnemonic.qadd8, Rnp12, Rnp16, Rnp0);
            var Qasx = Instr(Mnemonic.qasx, Rnp12, Rnp16, Rnp0);
            var Qsax = Instr(Mnemonic.qsax, Rnp12, Rnp16, Rnp0);
            var Qsub16 = Instr(Mnemonic.qsub16, Rnp12, Rnp16, Rnp0);
            var QSub8 = Instr(Mnemonic.qsub8, Rnp12, Rnp16, Rnp0);
            var Shadd16 = Instr(Mnemonic.shadd16, Rnp12, Rnp16, Rnp0);
            var Shasx = Instr(Mnemonic.shasx, Rnp12, Rnp16, Rnp0);
            var Shsax = Instr(Mnemonic.shsax, Rnp12, Rnp16, Rnp0);
            var Shsub16 = Instr(Mnemonic.shsub16, Rnp12,Rnp16,Rnp0);
            var Shadd8 = Instr(Mnemonic.shadd8, Rnp12, Rnp16, Rnp0);
            var Shsub8 = Instr(Mnemonic.shsub8, Rnp12, Rnp16, Rnp0);
            var Uadd16 = Instr(Mnemonic.uadd16, Rnp12, Rnp16, Rnp0);
            var Uasx = Instr(Mnemonic.uasx, Rnp12, Rnp16, Rnp0);
            var Usax = Instr(Mnemonic.usax, Rnp12, Rnp16, Rnp0);
            var Usub16 = Instr(Mnemonic.usub16, Rnp12, Rnp16, Rnp0);
            var Uadd8 = Instr(Mnemonic.uadd8, Rnp12, Rnp16, Rnp0);
            var Usub8 = Instr(Mnemonic.usub8, Rnp12, Rnp16, Rnp0);
            var Uqadd16 = Instr(Mnemonic.uqadd16, Rnp12, Rnp16, Rnp0);
            var Uqasx = Instr(Mnemonic.uqasx, Rnp12, Rnp16, Rnp0);
            var Uqsax = Instr(Mnemonic.uqsax, Rnp12, Rnp16, Rnp0);
            var Uqsub16 = Instr(Mnemonic.uqsub16, Rnp12, Rnp16, Rnp0);
            var Uqadd8 = Instr(Mnemonic.uqadd8, Rnp12, Rnp16, Rnp0);
            var Uqsub8 = Instr(Mnemonic.uqsub8, Rnp12, Rnp16, Rnp0);
            var Uhadd16 = Instr(Mnemonic.uhadd16, Rnp12, Rnp16, Rnp0);
            var Uhasx = Instr(Mnemonic.uhasx, Rnp12, Rnp16, Rnp0);
            var Uhsax = Instr(Mnemonic.uhsax, Rnp12, Rnp16, Rnp0);
            var Uhsub16 = Instr(Mnemonic.uhsub16, Rnp12, Rnp16, Rnp0);
            var Uhadd8 = Instr(Mnemonic.uhadd8, Rnp12, Rnp16, Rnp0);
            var Uhsub8 = Instr(Mnemonic.uhsub8, Rnp12, Rnp16, Rnp0);

            var ParallelArithmetic = Mask(20, 3, "Parallel arithmetic",
                invalid,
                Mask(5, 3, "  001",
                    Sadd16,
                    Sasx,
                    Ssax,
                    Ssub16,

                    Sadd8,
                    invalid,
                    invalid,
                    Ssub8),
                Mask(5, 3, "  010",
                    Qadd16,
                    Qasx,
                    Qsax,
                    Qsub16,

                    Qadd8,
                    invalid,
                    invalid,
                    QSub8),
                Mask(5, 3, "  011",
                    Shadd16,
                    Shasx,
                    Shsax,
                    Shsub16,

                    Shadd8,
                    invalid,
                    invalid,
                    Shsub8),
                invalid,
                Mask(5, 3, "  101",
                    Uadd16,
                    Uasx,
                    Usax,
                    Usub16,

                    Uadd8,
                    invalid,
                    invalid,
                    Usub8),
                Mask(5, 3, "  110",
                    Uqadd16,
                    Uqasx,
                    Uqsax,
                    Uqsub16,

                    Uqadd8,
                    invalid,
                    invalid,
                    Uqsub8),
                Mask(5, 3, "  111",
                    Uhadd16,
                    Uhasx,
                    Uhsax,
                    Uhsub16,

                    Uhadd8,
                    invalid,
                    invalid,
                    Uhsub8));

            var BitfieldInsert = Select(0, 0xF, n => n != 0xF,
                Instr(Mnemonic.bfi, r(3),r(0),B(7,5,16,5)),
                Instr(Mnemonic.bfc, r(3),B(7,5,16,5)));

            var BitfieldExtract = Mask(22, 1,
                Instr(Mnemonic.sbfx, r(3),r(0),i(7,5),i_p1(16,5)),
                Instr(Mnemonic.ubfx, r(3),r(0),i(7,5),i_p1(16,5)));

            var Saturate16Bit = Mask(22, 1, "Saturate 16-bit",
                Instr(Mnemonic.ssat16, Rnp12, i(16, 4), Rnp0),
                Instr(Mnemonic.usat16, Rnp12, i(16, 4), Rnp0));

            var Saturate32Bit = Mask(22, 1, "Saturate 32-bit",
                Instr(Mnemonic.ssat, Rnp12, i(16, 5), Rnp0, Shi),
                Instr(Mnemonic.usat, Rnp12, i(16, 5), Rnp0, Shi));

            var ExtendAndAdd = Mask(20, 3, "Extend and add",
                Select(16, 0xF, n => n != 0xF, 
                    Instr(Mnemonic.sxtab16, Rnp12,R16,Rnp0,ShR(10,2)), 
                    Instr(Mnemonic.sxtb16, Rnp12, R16, Rnp0, ShR(10, 2))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Mnemonic.sxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Mnemonic.sxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Mnemonic.sxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Mnemonic.sxth, r(3),r(0),ShR(10,2))),
                
                Select(16, 0xF, n => n != 0xF, Instr(Mnemonic.uxtab16, Rnp12, Rnp16, Rnp0, ShR(10, 2)), Instr(Mnemonic.uxtb16, Rnp12, Rnp0, ShR(10, 2))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Mnemonic.uxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Mnemonic.uxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Mnemonic.uxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Mnemonic.uxth, r(3),r(0),ShR(10,2))));
            var ReverseBitByte = Mask(22, 1,
                Mask(7, 1,
                    Instr(Mnemonic.rev, r(3),r(0)),
                    Instr(Mnemonic.rev16, r(3),r(0))),
                Mask(7, 1,
                    Instr(Mnemonic.rbit, r(3),r(0)),
                    Instr(Mnemonic.revsh, r(3),r(0))));

            var PermanentlyUndefined = Select(0, 4, n => n == 0b1110,
                Instr(Mnemonic.udf),
                invalid);

            var SignedMultiplyDivide = Mask(20, 3, "Signed multiply, Divide",
                new PcDecoder(12, 
                    Mask(5, 3,
                        Instr(Mnemonic.smlad, Rnp16,Rnp0,Rnp8,Rnp12),
                        Instr(Mnemonic.smladx, Rnp16, Rnp0, Rnp8, Rnp12),
                        Instr(Mnemonic.smlsd, Rnp16, Rnp0, Rnp8, Rnp12),
                        Instr(Mnemonic.smlsdx, Rnp16, Rnp0, Rnp8, Rnp12),

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3,
                        Instr(Mnemonic.smuad, Rnp16, Rnp0, Rnp8),
                        Instr(Mnemonic.smuadx, Rnp16, Rnp0, Rnp8),
                        Instr(Mnemonic.smusd, Rnp16, Rnp0, Rnp8),
                        Instr(Mnemonic.smusdx, Rnp16, Rnp0, Rnp8),

                        invalid,
                        invalid,
                        invalid,
                        invalid)),
                Select(5, 0x7, n => n != 0, invalid, Instr(Mnemonic.sdiv, Rnp16,Rnp0,Rnp8)),
                invalid,
                Select(5, 0x7, n => n != 0, invalid, Instr(Mnemonic.udiv, Rnp16, Rnp0, Rnp8)),

                Mask(5, 3,
                    Instr(Mnemonic.smlald, Rnp16, Rnp0, Rnp8, Rnp12),
                    Instr(Mnemonic.smlaldx, Rnp16, Rnp0, Rnp8, Rnp12),
                    Instr(Mnemonic.smlsld, Rnp16, Rnp0, Rnp8, Rnp12),
                    Instr(Mnemonic.smlsldx, Rnp16, Rnp0, Rnp8, Rnp12),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(5, 3,
                    new PcDecoder(12, 
                        Instr(Mnemonic.smmla, Rnp16, Rnp0, Rnp8, Rnp12), 
                        Instr(Mnemonic.smmul, Rnp16, Rnp0, Rnp8)),
                    new PcDecoder(12,
                        Instr(Mnemonic.smmlar, Rnp16,Rnp0, Rnp8,Rnp12),
                        Instr(Mnemonic.smmulr, Rnp16, Rnp0, Rnp8)),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.smmls, Rnp16, Rnp0, Rnp8, Rnp12),
                    Instr(Mnemonic.smmlsr, Rnp16, Rnp0, Rnp8, Rnp12)),
                invalid,
                invalid);

            var PkhbtPkhtb = Mask(6, 1,
                Instr(Mnemonic.pkhbt, Rnp12,Rnp16,Rnp0,Shi),
                Instr(Mnemonic.pkhtb, Rnp12, Rnp16, Rnp0, Shi));

            var UnsignedSumAbsoluteDifferences = new PcDecoder("Unsigned Sum of Absolute Differences", 12,
                Instr(Mnemonic.usada8, Rnp16, Rnp0, Rnp8, Rnp12),
                Instr(Mnemonic.usad8, Rnp16,Rnp0,Rnp8));

            var Media = Mask(23, 2, "Media",
                ParallelArithmetic,
                Mask(20, 3, "  op0=01",
                    Mask(5, 3,  // op0=0b01_000
                        PkhbtPkhtb,
                        invalid,
                        PkhbtPkhtb,
                        ExtendAndAdd,

                        PkhbtPkhtb,
                        Instr(Mnemonic.sel),
                        PkhbtPkhtb,
                        invalid),
                    Mask(5, 3, "  0b01001",
                        invalid,
                        invalid,
                        invalid,
                        ExtendAndAdd,
                        
                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3,  // op0=0b01_010
                        Saturate32Bit,
                        Saturate16Bit,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        invalid,
                        Saturate32Bit,
                        invalid),
                    Mask(5, 3,  // media op0=0b01011
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid),

                    Mask(5, 3, "  op0=0b01100",
                        Saturate32Bit,
                        Saturate16Bit,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        invalid,
                        Saturate32Bit,
                        invalid),

                    Mask(5, 3, "media - 0b01101",
                        invalid,
                        invalid, 
                        invalid,
                        ExtendAndAdd,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3, "media - 0b01110",
                        Saturate32Bit,
                        Saturate16Bit,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        invalid,
                        Saturate32Bit,
                        invalid),
                    Mask(5, 3,
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid)),
                SignedMultiplyDivide,
                Mask(20, 3, "  0b11???",
                    Mask(5, 3, "media - 0b11000",
                        UnsignedSumAbsoluteDifferences,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3, "  0b11001",
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3, "  0b11010",
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid),
                    Mask(5, 3, "media - 0b11011",
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid),
                    Mask(5, 3, "media - 0b11100",
                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid,

                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3, "media - 0b11101",
                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid,

                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 3,
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid),
                    Mask(5, 3, // media - 0b11111
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        PermanentlyUndefined)));

            var StmdaStmed = Instr(Mnemonic.stmda, w(21), r(4),Mr(0,16));
            var LdmdaLdmfa = Instr(Mnemonic.ldmda, w(21), r(4),Mr(0,16));
            var Stm =        Instr(Mnemonic.stm, w(21), r(4),Mr(0,16));
            var Ldm =        Instr(Mnemonic.ldm, w(21), r(4),Mr(0,16));
            var StmStmia =   Instr(Mnemonic.stm, w(21), r(4),Mr(0,16));
            var LdmLdmia =   Instr(Mnemonic.ldm, w(21), r(4),Mr(0,16));
            var StmdbStmfd = Instr(Mnemonic.stmdb, w(21), r(4),Mr(0,16));
            var LdmdbLDmea = Instr(Mnemonic.ldmdb, w(21), r(4),Mr(0,16));
            var StmibStmfa = Instr(Mnemonic.stmib, w(21), r(4),Mr(0,16));
            var LdmibLdmed = Instr(Mnemonic.ldmib, w(21), r(4),Mr(0,16));
            var StmUser = Mask(23, 2,
                Instr(Mnemonic.stmda, r(4), Mr(0, 16), u),
                Instr(Mnemonic.stmdb, r(4), Mr(0, 16), u),
                Instr(Mnemonic.stm, r(4), Mr(0, 16), u),
                Instr(Mnemonic.stmib, r(4), Mr(0, 16), u));
            var LdmUser = Mask(23, 2,
                Instr(Mnemonic.ldmda, r(4), Mr(0, 16), u),
                Instr(Mnemonic.ldmdb, r(4), Mr(0, 16), u),
                Instr(Mnemonic.ldm, r(4), Mr(0, 16), u),
                Instr(Mnemonic.ldmib, r(4), Mr(0, 16), u));
            var LoadStoreMultiple = Mask(Bf((22, 3),(20, 1)), // P U op L
                    StmdaStmed,
                    LdmdaLdmfa,
                    Stm,
                    Ldm,
                    StmStmia,
                    LdmLdmia,
                    StmUser,
                    LdmUser,

                    StmdbStmfd,
                    LdmdbLDmea,
                    StmUser,
                    LdmUser,
                    
                    StmibStmfa,
                    LdmibLdmed,
                    StmUser,
                    LdmUser);

            var RfeRfeda = nyi("RfeRefda");
            var SrcSrsda = Select(
                "SrcSrsda",
                5, 15,
                n => n == 0b1101_0000_0101_000,
                nyi("srs"),
                invalid);

            bool validRfe(uint n) => n == 0x0A00;

            var ExceptionSaveRestore = Mask(22, 3, // PUS"Exception Save/Restore",
                Mask(20, 1, // L
                    invalid,
                    Select(0, 0xFFFF, validRfe, Instr(Mnemonic.rfeda, w(21), Rnp16), invalid)),
                Mask(20, 1, // L
                    SrcSrsda,
                    invalid),
                Mask(20, 1, // L
                    invalid,
                    Select(0, 0xFFFF, validRfe, Instr(Mnemonic.rfeia, w(21), Rnp16), invalid)),
                Mask(20, 1, // L
                    SrcSrsda,
                    invalid),

                Mask(20, 1, // L
                    invalid,
                    Select(0, 0xFFFF, validRfe, Instr(Mnemonic.rfedb, w(21), Rnp16), invalid)),
                Mask(20, 1, // L
                    SrcSrsda,
                    invalid),
                Mask(20, 1, // L
                    invalid,
                    Select(0, 0xFFFF, validRfe, Instr(Mnemonic.rfeib, w(21), Rnp16), invalid)),
                Mask(20, 1, // L
                    SrcSrsda,
                    invalid));

            var BranchImmediate = new PcDecoder(28,
                Mask(24, 1,
                    Instr(Mnemonic.b, J),
                    Instr(Mnemonic.bl, J)),
                Instr(Mnemonic.blx, X));

            var Branch_BranchLink_BlockDataTransfer = Mask(25, 1, "Branch_BranchLink_BlockDataTransfer",
                new PcDecoder(28,
                    LoadStoreMultiple,
                    ExceptionSaveRestore),
                BranchImmediate);

            var SystemRegister_64bitMove = new PcDecoder(28, 
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Mnemonic.mcrr, CP(8),i(4,4),r(3),r(4),CR(0)),
                        Instr(Mnemonic.mrrc, CP(8),i(4,4),r(3),r(4),CR(0)))),
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Mnemonic.mcrr2, CP(8), i(4, 4), r(3), r(4), CR(0)),
                        Instr(Mnemonic.mrrc2, CP(8), i(4, 4), r(3), r(4), CR(0)))));


            var SystemRegister_LdSt = Select("SystemRegister_LdSt", 12, 0xF, n => n != 5, 
                invalid,
                Mask(20, 1,         // L (load)
                    Mask(23, 2, 21, 1, "SystemRegister_LdSt puw",
                        invalid,
                        Instr(Mnemonic.stc, CP(8),CR(12),Mi8(2, w4)),
                        Instr(Mnemonic.stc, CP(8),CR(12),Mi8(2,w4)),
                        Mask(22, 1, "SystemRegister_LdSt puw=011 d",
                            nyi("SystemRegister_LdSt puw=011 d=0"),
                            nyi("SystemRegister_LdSt puw=011 d=1")),

                        Instr(Mnemonic.stc, CP(8),CR(12),Mi8(2,w4)),
                        Instr(Mnemonic.stc, CP(8),CR(12),Mi8(2,w4)),
                        nyi("SystemRegister_LdSt puw=110"),
                        nyi("SystemRegister_LdSt puw=111")),
                    Mask(Bf((23, 2), (21, 1)),
                        invalid,
                        nyi("SystemRegister_LdSt puw=001"),
                        Instr(Mnemonic.ldc, CP(8),CR(12),Mi8(2,w4)),
                        nyi("SystemRegister_LdSt puw=011"),

                        Instr(Mnemonic.ldc, CP(8),CR(12),Mi8(2,w4)),
                        Instr(Mnemonic.ldc, CP(8),CR(12),Mi8(2,w4)),
                        nyi("SystemRegister_LdSt puw=110"),
                        nyi("SystemRegister_LdSt puw=111"))));

            var SystemRegister_LdSt_64bitMove = Select(21, 0b1101, n => n == 0,
                SystemRegister_64bitMove,
                SystemRegister_LdSt);

            var FloatingPointConvertToFixed =
                Mask(16, 1, "1 10x signed",
                    Mask(7, 2, "unsigned size:rounding",
                        Instr(Mnemonic.vcvt, U32F32, S12_22, S0_5),
                        Instr(Mnemonic.vcvtr, U32F32, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, U32F64, S12_22, D5_0),
                        Instr(Mnemonic.vcvtr, U32F64, S12_22, D5_0)),
                    Mask(7, 2, "signed size:rounding",
                        Instr(Mnemonic.vcvt, S32F32, S12_22, S0_5),
                        Instr(Mnemonic.vcvtr, S32F32, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, S32F64, S12_22, D5_0),
                        Instr(Mnemonic.vcvtr, S32F64, S12_22, D5_0)));

            var FloatingPointDataProcessing2regs = Mask(16, 4, "Floating point data processing (2 registers)",
                Mask(7, 3,  // size:o3
                    invalid,
                    invalid,
                    invalid,
                    Instr(Mnemonic.vabs, F16, S12_22, S0_5),

                    Instr(Mnemonic.vmov, F32, S12_22, S0_5),
                    Instr(Mnemonic.vabs, F32, S12_22, S0_5),
                    Instr(Mnemonic.vmov, F64, D22_12, D5_0),
                    Instr(Mnemonic.vabs, F64, D22_12, D5_0)),
                Mask(7, 1,
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vneg, F16, S12_22, S0_5),
                        Instr(Mnemonic.vneg, F32, S12_22, S0_5),
                        Instr(Mnemonic.vneg, F64, D22_12, D5_0)),
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vsqrt, F16, S12_22, S0_5),
                        Instr(Mnemonic.vsqrt, F32, S12_22, S0_5),
                        Instr(Mnemonic.vsqrt, F64, D22_12, D5_0))),

                Mask(7, 2, "  sz:o3",
                    Instr(Mnemonic.vcvtb, F32F16, S12_22, S0_5),
                    Instr(Mnemonic.vcvtt, F32F16, S12_22, S0_5),
                    Instr(Mnemonic.vcvtb, F64F16, D22_12, S0_5),
                    Instr(Mnemonic.vcvtt, F64F16, D22_12, S0_5)),

                Mask(7, 2, "  sz:o3",
                    Instr(Mnemonic.vcvtb, F16F32, S12_22, S0_5),
                    Instr(Mnemonic.vcvtt, F16F32, S12_22, S0_5),
                    Instr(Mnemonic.vcvtb, F16F64, D22_12, S0_5),
                    Instr(Mnemonic.vcvtt, F16F64, D22_12, S0_5)),

                Mask(7, 1,
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vcmp, F16, S12_22, S0_5),
                        Instr(Mnemonic.vcmp, F32, S12_22, S0_5),
                        Instr(Mnemonic.vcmp, F64, D22_12, D5_0)),
                    Mask(8, 2, 
                        invalid,
                        Instr(Mnemonic.vcmpe, F16, S12_22, S0_5),
                        Instr(Mnemonic.vcmpe, F32, S12_22, S0_5),
                        Instr(Mnemonic.vcmpe, F64, D22_12, D5_0))),
                Select("vcmpe #0", 0, 0xF, u => u == 0,
                    Mask(7, 2, "0101",
                        Instr(Mnemonic.vcmp, F32, S12_22, Imm0_r32),
                        Instr(Mnemonic.vcmpe, F32, S12_22, Imm0_r32),
                        Instr(Mnemonic.vcmp, F64, D22_12, Imm0_r64),
                        Instr(Mnemonic.vcmpe, F64, D22_12, Imm0_r64)),
                    invalid),
                nyi("Floating-point data-procesing (two registers) 0 110"),
                Mask(6, 2, "Floating-point data-procesing (two registers) 0 111 - op3",
                    invalid,
                    Mask(8, 2, 
                        invalid,
                        Instr(Mnemonic.vrintx, F16, S12_22, S0_5),
                        Instr(Mnemonic.vrintx, F32, S12_22, S0_5),
                        Instr(Mnemonic.vrintx, F64, D22_12, D5_0)),
                    invalid,
                    Mask(8, 1, "sz",
                        Instr(Mnemonic.vcvt, F64F32, D22_12, S0_5),
                        Instr(Mnemonic.vcvt, F32F64, S12_22, D5_0))),

                Mask(7, 1,
                    Mask(8, 2, 
                        invalid,
                        Instr(Mnemonic.vcvt, F16U16, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, F32U32, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, F64U32, D22_12, S0_5)),
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vcvt, F16S16, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, F32S32, S12_22, S0_5),
                        Instr(Mnemonic.vcvt, F64S32, D22_12, S0_5))),
                nyi("Floating-point data-procesing (two registers) 1 001"),
                nyi("Floating-point data-procesing (two registers) 1 010"),
                nyi("Floating-point data-procesing (two registers) 1 011"),

                FloatingPointConvertToFixed,
                FloatingPointConvertToFixed,
                nyi("Floating-point data-procesing (two registers) 1 110"),
                nyi("Floating-point data-procesing (two registers) 1 111"));

            var FloatingPointDataProcessing3regs = Mask(Bf((23, 1), (20, 2), (6, 1)), "Floating-point data-processing (three registers)",
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vmla, F16, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vmla, F32, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vmla, F64, D22_12,D7_16,D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vmls, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vmls, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vmls, F64, D22_12,D7_16,D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vnmls, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmls, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmls, F64, D22_12,D7_16,D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vnmla, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmla, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmla, F64, D22_12, D7_16, D5_0)),

                Mask(8, 2, 
                    invalid,
                    Instr(Mnemonic.vmul, F16, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vmul, F32, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vmul, F64, D22_12, D7_16, D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vnmul, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmul, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vnmul, F64, D22_12, D7_16, D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vadd, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vadd, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vadd, F64, D22_12, D7_16, D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vsub, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vsub, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vsub, F64, D22_12, D7_16, D5_0)),

                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vdiv, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vdiv, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vdiv, F64, D22_12,D7_16,D5_0)),
                invalid,
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vfnms, F16, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfnms, F32, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfnms, F64, D22_12, D7_16, D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vfnma, F16, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vfnma, F32, S12_22, S16_7,S0_5),
                    Instr(Mnemonic.vfnma, F64, D22_12, D7_16, D5_0)),

                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vfma, F16, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfma, F32, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfma, F64, D22_12, D7_16, D5_0)),
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vfms, F16, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfms, F32, S12_22, S16_7, S0_5),
                    Instr(Mnemonic.vfms, F64, D22_12, D7_16, D5_0)),
                invalid,
                invalid);

            var FloatingPointDataProcessing = new PcDecoder("Floating point data processing", 28,
                Select(20, 0b1011, n => n != 0b1011,
                    FloatingPointDataProcessing3regs,
                    Mask(6, 1,
                        Mask(8, 1,  // FloatingPointMoveImmediate sz
                            Instr(Mnemonic.vmov, F32, S12_22, vfpImm32(16,4,0,4)),
                            Instr(Mnemonic.vmov, F64, D22_12, vfpImm64(16,4,0,4))),
                        FloatingPointDataProcessing2regs)),
                Select(8, 0b11, n => n == 0,
                    invalid,
                    Select(23, 1, n => n == 0,  // op0 = 0b0xxx
                        Mask(6, 1,
                            invalid,
                            nyi("floating point conditional select")),
                        Select(20, 3, n => n == 0,  // op0 = 0b1x00
                            nyi("floating point minNum/maxNum"),
                            nyi("floating point data processing")))));

            var vmov_scalar_to_gp_reg = Mask(22, 2, 5, 2, "VMOV (scalar to general-purpose register) LC=11 U:opc1:opc2=??xxx",
                    Instr(Mnemonic.vmov, I32, Rnp12, D7_16, Ix((21, 1))),
                    Instr(Mnemonic.vmov, u23_I16, Rnp12, D7_16, Ix((21, 1),(6,1))),
                    invalid,
                    Instr(Mnemonic.vmov, u23_I16, Rnp12, D7_16, Ix((21, 1),(6,1))),

                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),

                    invalid,
                    Instr(Mnemonic.vmov, u23_I16, Rnp12, D7_16, Ix((21, 1),(6,1))),
                    invalid,
                    Instr(Mnemonic.vmov, u23_I16, Rnp12, D7_16, Ix((21, 1),(6,1))),

                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))),
                    Instr(Mnemonic.vmov, u23_I8, Rnp12, D7_16, Ix((21, 1), (5,2))));

            var vmov_gp_reg_to_scalar = Mask(23, 1, 5, 2, "VMOV (general-purpose register to scalar) LC=11 opc1:opc2=?x??",
                    Instr(Mnemonic.vmov, I32, D7_16, Ix((21, 1)), Rnp12),
                    Instr(Mnemonic.vmov, I16, D7_16, Ix((21, 1), (6, 1)), Rnp12),
                    invalid,
                    Instr(Mnemonic.vmov, I16, D7_16, Ix((21, 1), (6, 1)), Rnp12),

                    Instr(Mnemonic.vmov, I8, D7_16, Ix((21, 1), (6, 2)), Rnp12),
                    Instr(Mnemonic.vmov, I8, D7_16, Ix((21, 1), (6, 2)), Rnp12),
                    Instr(Mnemonic.vmov, I8, D7_16, Ix((21, 1), (6, 2)), Rnp12),
                    Instr(Mnemonic.vmov, I8, D7_16, Ix((21, 1), (6, 2)), Rnp12));

            var AdvancedSimd_32bitTransfer = Mask(20, 1, 6, 1, "Advanced SIMD 8/16/32-bit element move/duplicate",
                Mask(23, 1, "AdvancedSimd_32bitTransfer LC=00 A=?xx",
                    vmov_gp_reg_to_scalar,
                    Instr(Mnemonic.vdup, vW(22, 1, 5, 1), q(21), W7_16, R12)),
                Mask(23, 1, "AdvancedSimd_32bitTransfer LC=01 A=?xx",
                    Instr(Mnemonic.vmov, vW(22, 1, 5, 1), D7_16, r(3)),
                    Mask(6, 1,
                        Instr(Mnemonic.vdup, vW(22, 1, 5, 1), q(21), W7_16, R12),
                        invalid)),
                vmov_scalar_to_gp_reg,
                vmov_scalar_to_gp_reg);

            //var AdvancedSIMDElementMovDuplicate = Mask(20, 1, "AdvancedSIMDElementMovDuplicate",
            //    Mask(21,2,5,2,
            //        Instr(Opcode.vmov, I32, r(3), D7_16, Ix(21,1)),
            //        Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 01)")),
            //        Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 10)")),
            //        Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b00 11)")),

            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 00)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 01)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 10)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b01 11)")),

            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 00)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 01)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 10)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b10 11)")),

            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 00)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 01)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 10)")),
            //Instr(Opcode.vmov, r(3), x("*Scalar to GP op1:op2=0b11 11)))"))));

            var FloatingPointMoveSpecialReg = Mask(20, 1,
                Instr(Mnemonic.vmsr, i(16,4), r(3)),
                Instr(Mnemonic.vmrs, r(3), i(16,4)));

            var AdvancedSIMDandFloatingPoint32bitMove = Mask(8, 1, "Advanced SIMD and floating-point 32-bit move",
                    Mask(21, 3,
                        Instr(Mnemonic.vmov, S16_7,r(3)),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        FloatingPointMoveSpecialReg),
                    AdvancedSimd_32bitTransfer);

            var FldmdbxFldmiax = nyi("FLDMDBX FLDMIAX");

            var AdvancedSimd_and_floatingpoint_LdSt = Mask(23, 2, 20, 2, "Advanced SIMD and floating-point load/store",
                invalid,
                invalid,
                invalid,
                invalid,

                Mask(8, 2, "PUWL: 0b0100",
                    invalid,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0100 size: 0b01"),
                    Instr(Mnemonic.vstmia, w(21), r(4), Ms(0, 16)),
                    Instr(Mnemonic.vstmia, w(21), r(4), Md(0, 16))),
                Mask(8, 2, "PUWL: 0b0101",
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b01"),
                    Instr(Mnemonic.vldmia, w(21), r(4), Ms(0,16)),
                    Instr(Mnemonic.vldmia, w(21), r(4), Md(0,16))),
                Mask(8, 2, "PUWL: 0b0110",
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b01"),
                    Instr(Mnemonic.vstmia, w(21), r(4), Ms(0,16)),
                    Instr(Mnemonic.vstmia, w(21), r(4), Md(0,16))),
                Mask(8, 2, "PUWL: 0b0111",
                    invalid,
                    invalid,
                    Instr(Mnemonic.vldmia, w(21), r(4), Ms(0,16)),
                    Mask(0, 1,
                        Instr(Mnemonic.vldmia, w(21), r(4), Md(0,16)),
                        FldmdbxFldmiax)),

                Mask(8, 2, // size
                    invalid,
                    Instr(Mnemonic.vstr, I16, S12_22, Mi8(1, w2)),
                    Instr(Mnemonic.vstr, S12_22, Mi8(2, w4)),
                    Instr(Mnemonic.vstr, D22_12, Mi8(2, w8))),
                Mask(8, 2, // size"AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1001",
                    invalid,
                    Instr(Mnemonic.vldr, I16, S12_22, Mi8(1, w2)),
                    Instr(Mnemonic.vldr, S12_22, Mi8(2, w4)),
                    Instr(Mnemonic.vldr, D22_12, Mi8(2, w8))),
                Mask(8, 2, "   PUWL: 0b1010",
                    invalid,
                    invalid,
                    Instr(Mnemonic.vstmdb, w(21), r(4), Ms(0,16)),
                    Mask(0, 1,
                        Instr(Mnemonic.vstmdb, w(21), r(4), Md(0,16)),
                        FldmdbxFldmiax)),
                Mask(8, 2, // AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011 size
                    invalid,
                    invalid,
                    Instr(Mnemonic.vstmdb, w(21), r(4), Ms(0, 16)),
                    Mask(0, 1,
                        Instr(Mnemonic.vldmdb, w(21), r(4), Md(0, 16)),
                        FldmdbxFldmiax)),

                Mask(8, 2, // size
                    invalid,    
                    Instr(Mnemonic.vstr, I16, S12_22, Mi8(1,w2)),
                    Instr(Mnemonic.vstr, S12_22, Mi8(2,w4)),
                    Instr(Mnemonic.vstr, D22_12,Mi8(2,w8))),
                Mask(8, 2, // size
                    invalid,
                    Instr(Mnemonic.vldr, I16, S12_22, Mi8(1,w2)),
                    Instr(Mnemonic.vldr, S12_22, Mi8(2,w4)),
                    Instr(Mnemonic.vldr, D22_12,Mi8(2,w8))),
                invalid,
                invalid);

            var AdvancedSimd_and_floatingpoint64bitmove = Mask(22, 1, "Advanced SIMD and floating-point 64-bit move",
                invalid,
                Mask(4, 1, "  o3",
                    invalid,
                    Select("  opc2", 6, 0x3, n => n != 0,
                        invalid,
                        Mask(20, 1, "  op",
                            Mask(8, 2, "  size",
                                invalid,
                                invalid,
                                Instr(Mnemonic.vmov, S_pair(0, 5), Rnp12, Rnp16),
                                Instr(Mnemonic.vmov, D5_0, Rnp12, Rnp16)),
                            Mask(8, 2, "  size",
                                invalid,
                                invalid,
                                Instr(Mnemonic.vmov, Rnp12, Rnp16, S_pair(0,5)),
                                Instr(Mnemonic.vmov, Rnp12, Rnp16, D5_0))))));

            var AdvancedSimd_LdSt_64bitmove = Select("AdvancedSimd_LdSt_64bitmove", 21, 0b1101, n => n == 0,
                AdvancedSimd_and_floatingpoint64bitmove,
                AdvancedSimd_and_floatingpoint_LdSt);

            var SystemRegister32BitMove = new PcDecoder(28,
                Mask(20, 1,
                    Instr(Mnemonic.mcr, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3)),
                    Instr(Mnemonic.mrc, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3))),
                invalid);

            var VpmaxInteger = Instr(Mnemonic.vpmax, viu(), D22_12, D7_16, D5_0);
            var VpminInteger = Instr(Mnemonic.vpmin, viu(), D22_12, D7_16, D5_0);
            var Vfma = Instr(Mnemonic.vfma, q6, vf20_SD, W22_12, W7_16, W5_0);
            var Vfms = Instr(Mnemonic.vfms, q6, vf20_SD, W22_12, W7_16, W5_0);
            var Vabd = Instr(Mnemonic.vabd, viBHW_BHW_, q6, W22_12, W7_16, W5_0);
            var Vaba = Instr(Mnemonic.vaba, viBHW_BHW_, q6, W22_12, W7_16, W5_0);

            var AdvancedSimd_ThreeRegisters = Mask(24, 1, "Advanced SIMD three registers of the same length",
                Mask(8, 4, "  U = 0",
                    Mask(4, 1, "opc=0b0000 o1",
                        Instr(Mnemonic.vhadd, viBHW_BHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vqadd, viBHW_BHW_, q6, W22_12, W7_16, W5_0)),
                    Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=00 o1
                            Instr(Mnemonic.vrhadd, q6,viBHW_BHW_, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vand, q6, W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=01"),
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1=0"),
                            Instr(Mnemonic.vorr, q6, W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=11")),
                    Mask(4, 1, "  opc=0b0010",
                        Instr(Mnemonic.vhsub, viBHW_BHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vqsub, viBHW_BHW_, q6, W22_12, W7_16, W5_0)),
                    Mask(4, 1, "AdvancedSimd_ThreeRegisters - U=0, opc=0b0011",
                        Instr(Mnemonic.vcgt, visBHWD, q6, W22_12, W5_0, W7_16),
                        Instr(Mnemonic.vcge, visBHWD, q6, W22_12, W5_0, W7_16)),

                    Mask(4, 1, "  opc=0b0100",
                        Instr(Mnemonic.vshl, visBHWD, q6, W22_12, W5_0, W7_16),
                        Instr(Mnemonic.vqshl, visBHWD, q6, W22_12, W5_0, W7_16)),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0101"),
                    Mask(4, 1, "  opc=0b0110",
                        Instr(Mnemonic.vmax, visBHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vmin, visBHW_, q6, W22_12, W7_16, W5_0)),

                    Mask(4, 1, "  opc=0b0111",
                        Vabd,
                        Vaba),

                    Mask(4, 1, "  U = 0, opc=0b1000",
                        Mask(20, 2,
                            Instr(Mnemonic.vadd, I8, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vadd, I16, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vadd, I32, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vadd, I64, q6, W22_12, W7_16, W5_0)),
                        Instr(Mnemonic.vtst, viBHW_, q6, W22_12, W7_16, W5_0)),

                    Mask(4, 1, "  U = 0, opc=0b1001",
                        Instr(Mnemonic.vmla, viBHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vmul, viBHW_, q6, W22_12, W7_16, W5_0)),

                    Mask(6, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 
                            Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=0
                                Instr(Mnemonic.vpmax, S8, D22_12,D7_16,D5_0),
                                Instr(Mnemonic.vpmax, S16, D22_12,D7_16,D5_0),
                                Instr(Mnemonic.vpmax, S32, D22_12,D7_16,D5_0),
                                invalid),
                            Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=1
                                Instr(Mnemonic.vpmin, S8, D22_12,D7_16,D5_0),
                                Instr(Mnemonic.vpmin, S16, D22_12,D7_16,D5_0),
                                Instr(Mnemonic.vpmin, S32, D22_12,D7_16,D5_0),
                                invalid)),
                        invalid),
                    Mask(4, 1,
                        Instr(Mnemonic.vqdmulh, q6, vis_HW_, W22_12,W7_16,W5_0),
                        Instr(Mnemonic.vpadd, I8, viBHW_, D22_12,D7_16,D5_0)),

                    Mask(20, 2, "AdvancedSimd_ThreeRegisters - U = 0, opc=0b1100",
                        Mask(4, 1, "  size=0x",
                            Instr(Mnemonic.sha1c, x("*")),
                            Vfma),
                        Mask(4, 1, "  size=0x",
                            Instr(Mnemonic.sha1p, x("*")),
                            Vfma),
                        Mask(4, 1, "  size=0x",
                            Instr(Mnemonic.sha1m, x("*")),
                            Vfms),
                        Mask(4, 1, "  size=0x",
                            Instr(Mnemonic.sha1su0, x("*")),
                            Vfms)),

                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1101"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1110"),
                    Mask(21, 1, "  opc=0b1111",
                        Mask(4, 1, "  size=0x", 
                            Instr(Mnemonic.vmax, vf20_SD, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vrecps, vf20_SD, q6, W22_12, W7_16, W5_0)),
                        Mask(4, 1, "  size=0x",
                            nyi("VMIN (floating point)"),
                            nyi("VRSQRTS")))),

                Mask(8, 4, "  U = 1",
                    Mask(4, 1, "  opc=0b0000 o1",
                        Instr(Mnemonic.vhadd, viBHW_BHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vqadd, viBHW_BHW_, q6, W22_12, W7_16, W5_0)),
                    Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=00 o1
                            Instr(Mnemonic.vrhadd, q6, viu(), W22_12, W7_16, W5_0),
                            Instr(Mnemonic.veor, q6, W22_12 ,W7_16,W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=01"),
                        Instr(Mnemonic.vbit, q6, W22_12, W7_16, W5_0),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=11")),
                    Mask(4, 1, "  opc=0b0010",
                        Instr(Mnemonic.vhsub, viBHW_BHW_, q6, W22_12, W7_16, W5_0),
                        Instr(Mnemonic.vqsub, viBHW_BHW_, q6, W22_12, W7_16, W5_0)),
                    Mask(4, 1, "  opc=0b0011",
                        Instr(Mnemonic.vcgt, viBHW_BHW_, q6,W22_12,W7_16,W5_0),
                        Instr(Mnemonic.vcge, viBHW_BHW_, q6,W22_12,W7_16,W5_0)),

                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0100
                        Mask(20, 2, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=0
                            Instr(Mnemonic.vshl, U8, q6, W22_12 ,W5_0,W7_16),
                            Instr(Mnemonic.vshl, U16, q6, W22_12 ,W5_0,W7_16),
                            Instr(Mnemonic.vshl, U32, q6, W22_12 ,W5_0,W7_16),
                            Instr(Mnemonic.vshl, U64, q6, W22_12 ,W5_0,W7_16)),
                        Mask(20, 2, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=1
                            Instr(Mnemonic.vqshl, U8, q6, W22_12 ,W7_16,W5_0),
                            Instr(Mnemonic.vqshl, U16, q6, W22_12 ,W7_16,W5_0),
                            Instr(Mnemonic.vqshl, U32, q6, W22_12 ,W7_16,W5_0),
                            Instr(Mnemonic.vqshl, U64, q6, W22_12 ,W7_16,W5_0))),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0101"),
                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110
                        Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110 max
                            Instr(Mnemonic.vmax, U8, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vmax, U16, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vmax, U32, q6, W22_12, W7_16, W5_0),
                            invalid),
                        Mask(20, 2, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110 min
                            Instr(Mnemonic.vmin, U8, q6, W22_12 ,W7_16,W5_0),
                            Instr(Mnemonic.vmin, U16, q6, W22_12 ,W7_16,W5_0),
                            Instr(Mnemonic.vmin, U32, q6, W22_12 ,W7_16,W5_0),
                            invalid)),
                    Mask(4, 1, "  opc=0b0111",
                        Vabd,
                        Vaba),

                    Mask(4, 1, "AdvancedSimd_ThreeRegisters - U = 1, opc=0b1000",
                        Mask(20, 2,
                            Instr(Mnemonic.vsub, I8, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vsub, I16, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vsub, I32, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vsub, I64, q6, W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc = 0b1000 op=1")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1001"),
                    Mask(6, 1, "  opc=1010",
                        Mask(4, 1, "  Q=0",
                            VpmaxInteger,
                            VpminInteger),
                        invalid),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1011"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1100"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1101"),
                    Mask(21, 1, 4, 1, "opc=0b1110 size:o1",
                        nyi("VCGE (register) - A2"),
                        Instr(Mnemonic.vacge, vf20_SD, q6, W22_12, W7_16, W5_0),
                        nyi("VCGT (register) - A2"),
                        Instr(Mnemonic.vacgt, vf20_SD, q6, W22_12, W7_16, W5_0)),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1111")));

            var AdvancedSimd_TwoRegistersScalarExtension = Mask(23, 1, "AdvancedSimd_TwoRegistersScalarExtension",
                nyi("AdvancedSimd_TwoRegistersScalarExtension op1=0"),
                Mask(10, 1, 8, 1, "AdvancedSimd_TwoRegistersScalarExtension op1=1",
                    Mask(4, 1, "AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:0",
                        nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:0 U=0"),
                        invalid),

                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:1"),
                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=1:0"),
                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=1:1")));

            var CoprocessorDataOperations = new PcDecoder(28,
                Instr(Mnemonic.cdp, CP(8), i(20,4), CR(12), CR(16), CR(0), i(5, 3)),
                Instr(Mnemonic.cdp2, CP(8), i(20,4), CR(12), CR(16), CR(0), i(5, 3)));

            var CoprocessorInstructionsAndSupervisorCall = Select("CoprocessorInstructionsAndSupervisorCall coproc=???x", 9, 0b111,
                u => u == 5,
                Mask(24, 2, "coproc=101x op1=??xxxx",
                    Select("op1=00?x?x", 21, 0b101, u => u == 0,
                        AdvancedSimd_LdSt_64bitmove,
                        AdvancedSimd_and_floatingpoint_LdSt),
                    AdvancedSimd_and_floatingpoint_LdSt,
                    Mask(4, 1, "10xxxx op=?",
                        FloatingPointDataProcessing,
                        AdvancedSIMDandFloatingPoint32bitMove),
                    Instr(Mnemonic.svc, InstrClass.Transfer | InstrClass.Call, i(0,24))),
                Mask(24, 2, "coproc!=101x ??xxxx",
                    Select("op1=00?x?x", 21, 0b101, u => u == 0,
                        SystemRegister_64bitMove,
                        Mask(20, 1,
                            Instr(Mnemonic.stc, CP(8), CR(12), Mi8(2, w4)),
                            Instr(Mnemonic.ldc, CP(8), CR(12), Mi8(2, w4)))),
                    Select("op1=01?x?x", 21, 0b101, u => u == 0,
                        SystemRegister_64bitMove,
                        Mask(20, 1,
                            Instr(Mnemonic.stc, CP(8), CR(12), Mi8(2, w4)),
                            Instr(Mnemonic.ldc, CP(8), CR(12), Mi8(2, w4)))),
                    Mask(4, 1, "10xxxx op=?",
                        CoprocessorDataOperations,
                        Mask(20, 1, "10xxx? op=0",
                            Instr(Mnemonic.mcr, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)),
                            Instr(Mnemonic.mrc, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)))),
                    Instr(Mnemonic.svc, InstrClass.Transfer | InstrClass.Call, i(0,24))));

            Decoder SystemRegister_AdvancedSimd_FloatingPoint = Mask(24, 2, "SystemRegister_AdvancedSimd_FloatingPoint",
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28, 
                        Select(10, 3, n => n == 2, 
                            AdvancedSimd_LdSt_64bitmove,
                            SystemRegister_LdSt_64bitMove),
                        AdvancedSimd_ThreeRegisters)),
                Select(9, 7, n => n == 7,
                    SystemRegister_LdSt_64bitMove,
                    new PcDecoder(28,
                        Select(10, 3, n => n == 2,
                            AdvancedSimd_LdSt_64bitmove,
                            SystemRegister_LdSt_64bitMove),
                        AdvancedSimd_ThreeRegisters)),
                Select(9, 7, n => n == 7,
                    Mask(4, 1, invalid, SystemRegister32BitMove),
                    new PcDecoder(28,
                        Select(10, 3, n => n == 2, 
                            Mask(4, 1,
                                FloatingPointDataProcessing,
                                AdvancedSIMDandFloatingPoint32bitMove),
                            invalid),
                        Select(10, 3, n => n == 2, AdvancedSimd_TwoRegistersScalarExtension, invalid))),
                    Instr(Mnemonic.svc, InstrClass.Transfer | InstrClass.Call, i(0,24)));

            var ConditionalDecoder = new CondMaskDecoder(25, 3, "",
                DataProcessingAndMisc,
                DataProcessingAndMisc,
                LoadStoreWordUnsignedByteImmLit,
                Mask(4, 1,
                    LoadStoreWordUnsignedByteRegister,
                    Media),
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                CoprocessorInstructionsAndSupervisorCall,
                CoprocessorInstructionsAndSupervisorCall);

            var VmullIntegerPolynomial = nyi("VmullIntegerPolynomial");

            var AdvanceSimd_ThreeRegistersDifferentLength = Mask(8, 4, "AdvanceSimd_ThreeRegistersDifferentLength",
                    Instr(Mnemonic.vaddl, viBHW_BHW_, Q22_12, D7_16, D5_0),
                    Instr(Mnemonic.vaddw, viBHW_BHW_, Q22_12, D7_16, D5_0),
                    Instr(Mnemonic.vsubl, viBHW_BHW_, Q22_12, D7_16, D5_0),
                    Instr(Mnemonic.vsubw, viBHW_BHW_, Q22_12, D7_16, D5_0),

                    Mask(24, 1,
                        Instr(Mnemonic.vaddhn, viHWD_, D22_12, Q7_16, Q5_0),
                        Instr(Mnemonic.vraddhn, viHWD_, D22_12, Q7_16, Q5_0)),
                    Instr(Mnemonic.vabal, viBHW_BHW_, Q22_12,D7_16,D5_0),
                    Mask(24, 1, "  opc=0110",
                        Instr(Mnemonic.vsubhn, viHWD_, D22_12, Q7_16, Q5_0),
                        Instr(Mnemonic.vrsubhn, viHWD_, D22_12, Q7_16, Q5_0)),
                    Instr(Mnemonic.vabdl, viBHW_BHW_, Q22_12, D7_16, D5_0),

                    Instr(Mnemonic.vmlal, viHWD_, Q22_12,D7_16,Q5_0),
                    Mask(24, 1,
                        Instr(Mnemonic.vqdmlal, x("*")),
                        invalid),
                    Instr(Mnemonic.vmlsl, viHWD_, Q22_12,D7_16,Q5_0),
                    Mask(24, 1,
                        Instr(Mnemonic.vqdmlsl, x("*")),
                        invalid),

                    VmullIntegerPolynomial,
                    Mask(24, 1,
                        Instr(Mnemonic.vqdmull, x("*")),
                        invalid),
                    VmullIntegerPolynomial,
                    invalid);

            var VmlaByScalar = Instr(Mnemonic.vmla, q(24),W22_12, vi_HW_f_HS_, W22_12,W7_16,W5_0,Ix(5,1));
            var VmlsByScalar = Instr(Mnemonic.vmls, q(24),W22_12, vi_HW_f_HS_, W22_12,W7_16,W5_0,Ix(5,1));
            var VmlalByScalar = Mask(20, 2, "vmlsl",
                invalid,
                Instr(Mnemonic.vmlal, vi_HW__HW_, Q22_12, D7_16, D(11, 1, 0, 3), Ix((5, 1), (3, 1))),
                Instr(Mnemonic.vmlal, vi_HW__HW_, Q22_12, D7_16, D(11, 1, 0, 4), Ix((5, 1))),
                invalid);
            var VmlslByScalar = Mask(20, 2, "vmlsl",
                invalid,
                Instr(Mnemonic.vmlsl, vi_HW__HW_, Q22_12, D7_16, D(11, 1, 0, 3), Ix((5, 1), (3, 1))),
                Instr(Mnemonic.vmlsl, vi_HW__HW_, Q22_12, D7_16, D(11, 1, 0, 4), Ix((5, 1))),
                invalid);
            var Vqdmlal = nyi("vqdmlal");
            var Vqdmlsl = nyi("vqdmlsl");
            var VmulByScalar = nyi("vmul");
            var VmullByScalar = nyi("vmull");
            var Vqdmull = nyi("vqdmull");
            var Vqdmulh = nyi("vqdmulh");
            var Vqrdmulh = nyi("vqrdmulh");
            var Vqrdmlah = nyi("vqrdmlah");
            var Vqrdmlsh = nyi("vqrdmlsh");

            var AdvanceSimd_TwoRegistersScalar = Mask(8, 4, "Advanced SIMD two registers and a scalar",
                VmlaByScalar,
                VmlaByScalar,
                VmlalByScalar,
                Mask(24, 1, "  0011",
                    Vqdmlal,
                    invalid),

                VmlsByScalar,
                VmlsByScalar,
                VmlslByScalar,
                Mask(24, 1, "  0111",
                    Vqdmlsl,
                    invalid),

                VmulByScalar,
                VmulByScalar,
                VmullByScalar,
                Mask(24, 1, "  1011",
                    Vqdmull,
                    invalid),

                Vqdmulh,
                Vqrdmulh,
                Vqrdmlah,
                Vqrdmlsh);

            var Vpaddl = Instr(Mnemonic.vpaddl, viBHW_BHW_, q6, W22_12, W7_16);
            var Vpadal = Instr(Mnemonic.vpadal, viBHW_BHW_, q6, W22_12, W7_16);
            var VqmovnVqmovun = nyi("VqmovnVqmovun");
            var Vcvta = nyi("Vcvta");
            var Vcvtn = nyi("Vcvtn");
            var Vcvtp = nyi("Vcvtp");
            var Vcvtm = nyi("Vcvtm");
            var Vrecpe = nyi("Vrecpe");
            var Vrsqrtpe = nyi("Vrsqrtpe");
            var Vcvt_between_fp_and_int = nyi("VCVT(between floating - point and integer, Advanced SIMD)");


            var AdvancedSimd_TwoRegistersMisc = Mask(16, 2, "Advanced SIMD two registers misc",
                Mask(7, 4, "  opc1=00",
                    Instr(Mnemonic.vrev64, x("*")),
                    Instr(Mnemonic.vrev32, x("*")),
                    Instr(Mnemonic.vrev16, viBHW_, q6, W22_12,W5_0),
                    invalid,

                    Vpaddl,
                    Vpaddl,
                    Mask(6, 1, "  opc2=0110",
                        Instr(Mnemonic.aese, x("*")),
                        Instr(Mnemonic.aesd, x("*"))),
                    Mask(6, 1, "  opc2=0111",
                        Instr(Mnemonic.aesmc, x("*")),
                        Instr(Mnemonic.aesimc, x("*"))),

                    Instr(Mnemonic.vcls, x("*")),
                    Instr(Mnemonic.vclz, x("*")),
                    Instr(Mnemonic.vcnt, x("*")),
                    Instr(Mnemonic.vmvn, x("(register)")),

                    Vpadal,
                    Vpadal,
                    Instr(Mnemonic.vqabs, x("*")),
                    Instr(Mnemonic.vqneg, x("*"))),
                Mask(7, 3, "  opc1=01",
                    Instr(Mnemonic.vcgt, x("(immediate 0)")),
                    Instr(Mnemonic.vcge, x("(immediate 0)")),
                    Instr(Mnemonic.vceq, x("(immediate 0)")),
                    Instr(Mnemonic.vcle, x("(immediate 0)")),

                    Instr(Mnemonic.vclt, x("(immediate 0)")),
                    Mask(10, 1, "  opc2=x101",
                        Mask(6, 1,
                            invalid,
                            Instr(Mnemonic.sha1h, x("*"))),
                        invalid),
                    Instr(Mnemonic.vabs, x("*")),
                    Instr(Mnemonic.vneg, x("*"))),

                Mask(7, 4, "  opc1=10",
                    Select("  size", 18, 0b11, n => n == 0,
                        Instr(Mnemonic.vswp, x("*")),
                        invalid),
                    Instr(Mnemonic.vtrn, x("*")),
                    Instr(Mnemonic.vuzp, x("*")),
                    Instr(Mnemonic.vzip, x("*")),

                    Mask(6, 1, " Q",
                        Instr(Mnemonic.vmovn, x("*")),
                        VqmovnVqmovun),
                    VqmovnVqmovun,
                    Mask(6, 1, " Q",
                        Instr(Mnemonic.vshll, x("*")),
                        invalid),
                    Mask(6, 1, " Q",
                        Instr(Mnemonic.sha1su1, x("*")),
                        Instr(Mnemonic.sha256su0, x("*"))),

                    Instr(Mnemonic.vrintn, x("*")),
                    Instr(Mnemonic.vrintx, x("*")),
                    Instr(Mnemonic.vrinta, x("*")),
                    Instr(Mnemonic.vrintz, x("*")),

                    Mask(6, 1, " Q",
                        nyi("VCVT (between half-precision and single-precision, Advanced SIMD) - Single-precision to half-precision variant"),
                        invalid),
                    Instr(Mnemonic.vrintm, x("*")),
                    Mask(6, 1, " Q",
                        nyi("VCVT (between half-precision and single-precision, Advanced SIMD) - Half-precision to single-precision variant"),
                        invalid),
                    Instr(Mnemonic.vrintp, x("*"))),

                Mask(7, 4, "  opc1=11",
                    Vcvta,
                    Vcvta,
                    Vcvtn,
                    Vcvtn,

                    Vcvtp,
                    Vcvtp,
                    Vcvtm,
                    Vcvtm,

                    Vrecpe,
                    Vrsqrtpe,
                    Vrecpe,
                    Vrsqrtpe,

                    Vcvt_between_fp_and_int,
                    Vcvt_between_fp_and_int,
                    Vcvt_between_fp_and_int,
                    Vcvt_between_fp_and_int));

            var AdvancedSimd_Duplicate_scalar = nyi("Advanced SIMD duplicate (scalar)");

            var Vtbl_vtbx = Mask(6, 1, "VTBL,VTBX",
                Instr(Mnemonic.vtbl, I8, D22_12,DRegList,D5_0),
                Instr(Mnemonic.vtbx, I8, D22_12,DRegList,D5_0));

            var AdvancedSimd_TwoRegisterOrThreeRegisters = Select("Advanced SIMD two registers, or three registers of different lengths",
                20, 0x3,
                n => n == 0b11,
                Mask(24, 1, "  op1=0b11",
                    Instr(Mnemonic.vext, U8, q6, W22_12, W7_16, W5_0, i(8, 4)),
                    Mask(10, 2, "  op0=1",
                        AdvancedSimd_TwoRegistersMisc,
                        AdvancedSimd_TwoRegistersMisc,
                        Vtbl_vtbx,
                        AdvancedSimd_Duplicate_scalar)),
                Mask(6, 1, "  op1!=0b11 op=0",
                    AdvanceSimd_ThreeRegistersDifferentLength,
                    AdvanceSimd_TwoRegistersScalar));

            var vmov_A1 = Instr(Mnemonic.vmov, I32, q6, W22_12, Is(24,1,16,3,0,4));
            var vmov_A3 = Instr(Mnemonic.vmov, I16, q6, W22_12, Is(24,1,16,3,0,4));
            var vmov_A4 = Instr(Mnemonic.vmov, DtFromCmode(8,2), q6, W22_12, Is(24,1,16,3,0,4));

            var VbicImm_A1 = Instr(Mnemonic.vbic, I32, q6, W22_12, Is(24, 1, 16, 3, 0, 4));
            var VbicImm_A2 = Instr(Mnemonic.vbic, I16, q6, W22_12, Is(24, 1, 16, 3, 0, 4));

            var AdvancedSimd_OneRegisterModifiedImmediate = Mask(8,4,5,1, "AdvancedSimd_OneRegisterModifiedImmediate",
                vmov_A1,
                Instr(Mnemonic.vmvn, I32, q6, W22_12, Is(24,1,16,3,0,4)),
                Instr(Mnemonic.vorr, x("immediate - A1")),
                VbicImm_A1,

                vmov_A1,
                Instr(Mnemonic.vmvn, I32, q6, W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Mnemonic.vorr, x("immediate - A1")),
                VbicImm_A1,

                vmov_A1,
                Instr(Mnemonic.vmvn, I32, q6, W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Mnemonic.vorr, x("immediate - A1")),
                VbicImm_A1,

                vmov_A1,
                Instr(Mnemonic.vmvn, I32, W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Mnemonic.vorr, x("immediate - A1")),
                VbicImm_A1,

                vmov_A3,
                Instr(Mnemonic.vmvn, x("immediate - A2")),
                Instr(Mnemonic.vorr, x("immediate - A2")),
                VbicImm_A2,

                vmov_A3,
                Instr(Mnemonic.vmvn, x("immediate - A2")),
                Instr(Mnemonic.vorr, x("immediate - A2")),
                VbicImm_A2,

                vmov_A4,
                Instr(Mnemonic.vmvn, x("immediate - A3")),
                vmov_A4,
                Instr(Mnemonic.vmvn, x("immediate - A3")),

                vmov_A4,
                Instr(Mnemonic.vmov, I64, q6, W22_12, Is(24,1, 16,3, 0,4)),
                vmov_A4,
                invalid);

            var fbits = iFrom(16, 6, 64);

            var VqshlVqshlu = Mask(24, 1, "vqshl, vqshlu",
                Instr(Mnemonic.vqshl, q6, vsh_BHWD_size, W22_12, W5_0, vsh_BHWD),
                Instr(Mnemonic.vqshlu, q6, vsh_BHWD_size, W22_12, W5_0, vsh_BHWD));

            var vcvt_between_fp_and_fixed_point = Mask(7, 1, "VCVT (between floating-point and fixed-point",
                Mask(8, 2, 24, 1, "vcvt (vector) op:U",
                    Instr(Mnemonic.vcvt, F16S16, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, F16U16, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, S16F16, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, U16F16, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, F32S32, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, F32U32, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, S32F32, q6, W22_12, W5_0, fbits),
                    Instr(Mnemonic.vcvt, U32F32, q6, W22_12, W5_0, fbits)),
                invalid);

            var AdvancedSimd_TwoRegisterShiftAmount = Mask(8, 4, "Advanced SIMD two registers and shift amount",
                Instr(Mnemonic.vshr, q6, vsh_HWDD_size, W22_12, W5_0, vsh_HWDD_rev),
                Instr(Mnemonic.vsra, q6, vsh_HWDD_size, W22_12, W5_0, vsh_HWDD_rev),
                Mask(6, 1, "  Q",
                    Instr(Mnemonic.vrshr, x("")),
                    invalid),
                Instr(Mnemonic.vrsra, q6, vsh_HWDD_size, W22_12, W5_0, vsh_HWDD_rev),

                Mask(24, 1, "  U",
                    invalid,
                    Instr(Mnemonic.vsri, x(""))),
                Mask(24, 1, "  U",
                    Instr(Mnemonic.vshl, x("(immediate)")),
                    Instr(Mnemonic.vsli, q6, vsh_BHWD_size, W22_12, W5_0, vsh_BHWD)),
                Mask(24, 1, "  U",
                    invalid,
                    VqshlVqshlu),
                VqshlVqshlu,

                Mask(6, 2, 24, 1, "  LQ:U",
                    Instr(Mnemonic.vshrn, x("*")),
                    Instr(Mnemonic.vqshrun, x("*")),
                    Instr(Mnemonic.vrshrn, x("*")),
                    Instr(Mnemonic.vqrshrun, x("*")),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                nyi(""),
                Select(6, 3, n => n != 0,
                    Select(16, 7, n => n != 0,
                        Instr(Mnemonic.vshl, x("")),
                        Instr(Mnemonic.vmov, x(""))),
                    invalid),
                invalid,

                vcvt_between_fp_and_fixed_point,
                vcvt_between_fp_and_fixed_point,
                vcvt_between_fp_and_fixed_point,
                vcvt_between_fp_and_fixed_point);


            var AdvancedSimd_ShiftsAndImmediate = Select(7, 0b111000000000001, n => n == 0,
                AdvancedSimd_OneRegisterModifiedImmediate,
                AdvancedSimd_TwoRegisterShiftAmount);

            var AdvancedSimd = Mask(23, 1,
                AdvancedSimd_ThreeRegisters,
                Mask(4, 1,
                    AdvancedSimd_TwoRegisterOrThreeRegisters,
                    AdvancedSimd_ShiftsAndImmediate));

            var AdvancedSimdLdStMultipleStructures = Mask(21, 1, "Advanced SIMD load/ store multiple structures",
                Mask(8, 4, "  L=0 type",
                    nyi("VST4(multiple 4 - element structures)"),
                    nyi("VST4(multiple 4 - element structures)"),
                    nyi("VST1(multiple single elements)"),
                    nyi("VST2(multiple 2 - element structures)"),

                    Instr(Mnemonic.vst3, vi_ld3, Vel(4, 2), Mve),
                    Instr(Mnemonic.vst3, vi_ld3, Vel(4, 2), Mve),
                    nyi("VST1(multiple single elements)"),
                    nyi("VST1(multiple single elements)"),

                    nyi("VST2(multiple 2 - element structures)"),
                    nyi("VST2(multiple 2 - element structures)"),
                    nyi("VST1(multiple single elements)"),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(8, 4, "  L=1 type",
                    Instr(Mnemonic.vld4, vi_ld4, Vel(4,1), Mve),
                    Instr(Mnemonic.vld4, vi_ld4, Vel(4,1), Mve),
                    nyi("VLD1(multiple single elements)"),
                    Instr(Mnemonic.vld2, vi_ld1, Vel(4, 1), Mve),

                    nyi("VLD3(multiple 3 - element structures)"),
                    nyi("VLD3(multiple 3 - element structures)"),
                    nyi("VLD1(multiple single elements)"),
                    nyi("VLD1(multiple single elements)"),

                    nyi("VLD2(multiple 2 - element structures)"),
                    nyi("VLD2(multiple 2 - element structures)"),
                    nyi("VLD1(multiple single elements)"),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid));

            var AdvancedSimdLdStStoreSingleOneLane = Mask(21, 1, "Advanced SIMD load / store single structure to one lane",
                Mask(8, 2, "  L=0",
                    Mask(10, 2, "  N=00 size",
                        Instr(Mnemonic.vst1, x("single element - A1")),
                        Instr(Mnemonic.vst1, x("single element - A2")),
                        Instr(Mnemonic.vst1, x("single element - A3")),
                        invalid),
                    Mask(10, 2, "  N=01 size",
                        nyi("VST2 (single 2-element structure from one lane) - A1"),
                        nyi("VST2 (single 2-element structure from one lane) - A2"),
                        nyi("VST2 (single 2-element structure from one lane) - A3"),
                        invalid),
                    Mask(10, 2, "  N=10 size",
                        nyi("VST3 (single 3-element structure from one lane) - A1"),
                        nyi("VST3 (single 3-element structure from one lane) - A2"),
                        nyi("VST3 (single 3-element structure from one lane) - A3"),
                        invalid),
                    Mask(10, 2, "  N=11 size",
                        nyi("VST4 (single 4-element structure from one lane) - A1"),
                        nyi("VST4 (single 4-element structure from one lane) - A2"),
                        nyi("VST4 (single 4-element structure from one lane) - A3"),
                        invalid)),
                Mask(8, 2, "  L=1",
                    Mask(10, 2, "  N=00 size",
                        Instr(Mnemonic.vst1, x("single element - A1")),
                        Instr(Mnemonic.vst1, x("single element - A2")),
                        Instr(Mnemonic.vst1, x("single element - A3")),
                        invalid),
                    Mask(10, 2, "  N=01 size",
                        nyi("VLD2 (single 2-element structure from one lane) - A1"),
                        nyi("VLD2 (single 2-element structure from one lane) - A2"),
                        nyi("VLD2 (single 2-element structure from one lane) - A3"),
                        invalid),
                    Mask(10, 2, "  N=10 size",
                        nyi("VLD3 (single 3-element structure from one lane) - A1"),
                        nyi("VLD3 (single 3-element structure from one lane) - A2"),
                        nyi("VLD3 (single 3-element structure from one lane) - A3"),
                        invalid),
                    Mask(10, 2, "  N=11 size",
                        nyi("VLD4 (single 4-element structure from one lane) - A1"),
                        nyi("VLD4 (single 4-element structure from one lane) - A2"),
                        nyi("VLD4 (single 4-element structure from one lane) - A3"),
                        invalid)));

            var AdvancedSimdLdSingleStructureToAllLanes = Mask(21, 1, "Advanced SIMD load single structure to all lanes",
                invalid,
                Mask(8, 2, "  L=1 N",
                    nyi("VLD1(single element to all lanes)"),
                    nyi("VLD2(single 2 - element structure to all lanes)"),
                    Mask(4, 1, "  N=10 a",
                        nyi("VLD3(single 3 - element structure to all lanes)"),
                        invalid),
                    nyi("VLD4(single 4 - element structure to all lanes)")));

            var AdvancedSimdElementLoadStore = Mask(23, 1, "Advanced SIMD element or structure load/store",
                AdvancedSimdLdStMultipleStructures,
                Mask(10, 2, "  op1",
                    AdvancedSimdLdStStoreSingleOneLane,
                    AdvancedSimdLdStStoreSingleOneLane,
                    AdvancedSimdLdStStoreSingleOneLane,
                    AdvancedSimdLdSingleStructureToAllLanes));

            var AdvancedSimdElementLoadStoreDead = Mask(23, 1, "AdvancedSimdElementLoadStore A=?",
                Mask(8, 4, "AdvancedSimdElementLoadStore A=0 B=????",
                    Mask(21, 1,
                        Instr(Mnemonic.vst4, vi_ld4, Vel(4,1), Mve),
                        Instr(Mnemonic.vld4, vi_ld4, Vel(4,1), Mve)),
                    Mask(21, 1,
                        Instr(Mnemonic.vst4, vi_ld4, Vel(4,2), Mve),
                        Instr(Mnemonic.vld4, vi_ld4, Vel(4,2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0010"),
                    Mask(21, 1,
                        Instr(Mnemonic.vst2, vi_ld4, Vel(4, 1), Mve),
                        Instr(Mnemonic.vld2, vi_ld4, Vel(4, 1), Mve)),

                    Mask(21, 1,
                        Instr(Mnemonic.vst3, vi_ld3, Vel(4, 1), Mve),
                        Instr(Mnemonic.vld3, vi_ld3, Vel(4, 1), Mve)),
                    Mask(21, 1,
                        Instr(Mnemonic.vst3, vi_ld3, Vel(4, 2), Mve),
                        Instr(Mnemonic.vld3, vi_ld3, Vel(4, 2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0110"),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0111"),

                    Mask(21, 1,
                        Instr(Mnemonic.vst2, vi_ld1, Vel(4, 1), Mve),
                        Instr(Mnemonic.vld2, vi_ld1, Vel(4, 1), Mve)),
                    Mask(21, 1,
                        Instr(Mnemonic.vst2, vi_ld1, Vel(2, 2), Mve),
                        Instr(Mnemonic.vld2, vi_ld1, Vel(2, 2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=1010"),
                    nyi("AdvancedSimdElementLoadStore A=0 B=1011"),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(8, 4, "AdvancedSimdElementLoadStore A=1 B=????",
                    Mask(21, 1,
                        Instr(Mnemonic.vst1, vi_ld1, Vel(1, 1), Mve),
                        Instr(Mnemonic.vld1, vi_ld1, Vel(1, 1), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0001"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0010"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0011"),

                    nyi("AdvancedSimdElementLoadStore A=1 B=0100"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0101"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0110"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=0111"),

                    nyi("AdvancedSimdElementLoadStore A=1 B=1000"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=1001"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=1010"),
                    nyi("AdvancedSimdElementLoadStore A=1 B=1011"),

                    invalid,
                    invalid,
                    invalid,
                    invalid));

            var Barriers = Mask(4, 4,
                invalid,
                Select(0, 0xFFFFFu, n => n == 0xFF01Fu, Instr(Mnemonic.clrex), invalid),
                invalid,
                invalid,

                Instr(Mnemonic.dsb, Ba(0,4)),
                Instr(Mnemonic.dmb, Ba(0,4)),
                Instr(Mnemonic.isb, Ba(0,4)),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var PreloadImmediate = Mask(24, 1, "Preload (immediate) D",
                Mask(22, 1, "  R",
                    Instr(Mnemonic.nop, InstrClass.Padding | InstrClass.Linear),
                    nyi("PLI (immediate, literal)")),
                new PcDecoder("  Rn", 16,
                    Mask(22, 1, "  R",
                        Instr(Mnemonic.pldw, Mo(w4)),
                        Instr(Mnemonic.pld, Mo(w4))),
                    Instr(Mnemonic.pld, Mo(w4))));

            var MemoryHintsAndBarriers = Mask(25, 1, "Memory hints and barriers",
                Mask(21, 1,
                    PreloadImmediate,
                    Mask(22, 3,
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        Barriers,
                        invalid,
                        invalid)),
                Mask(4, 1,
                    Mask(21, 1,
                        nyi("Preload (register)"),
                        invalid),
                    invalid));

            var vext = Instr(Mnemonic.vext, U8, q6, W22_12, W7_16, W5_0, i(8, 4));

            var AdvancedSimdDataProcessing = Mask(23, 1, "Advanced SIMD data-processing",
                AdvancedSimd_ThreeRegisters,
                Mask(4, 1, " op0=1",
                    AdvancedSimd_TwoRegisterOrThreeRegisters,
                    AdvancedSimd_ShiftsAndImmediate));

            var MemoryHintsAdvancedSimdMiscellaneous = Mask(24, 3, "Memory hints, Advanced SIMD instructions, and miscellaneous instructions",
                invalid,
                Select("op1=001????", 20, 0b1111, u => u == 0,
                    Mask(16, 1, "Rn=xxx?",
                        Mask(5, 1, "op2=xx?x",
                            ChangeProcessState,
                            invalid),
                        Select("op2=0000", 4, 0b1111, u => u == 0,
                            Instr(Mnemonic.setend, E(9, 1)), invalid)),
                    invalid),
                AdvancedSimdDataProcessing,
                AdvancedSimdDataProcessing,

                Mask(20, 3, "op1=100x???",
                    AdvancedSimdElementLoadStore,
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),    // Unallocated memory hint (treat as NOP)
                    AdvancedSimdElementLoadStore,
                    invalid,

                    AdvancedSimdElementLoadStore,
                    nyi("Preload instruction"),
                    AdvancedSimdElementLoadStore,
                    invalid),
                Mask(20, 1, "  op1=101xxx?",
                    invalid,
                    MemoryHintsAndBarriers),
                invalid,
                Mask(24, 4, "  op1=111xxxx",
                    nyi("op1=1110000"),
                    nyi("op1=1110001"),
                    nyi("op1=1110010"),
                    invalid,

                    nyi("op1=1110100"),
                    nyi("Preload (PLD,PLDW)"),
                    nyi("op1=1110110"),
                    invalid,

                    nyi("op1=1111000"),
                    nyi("op1=1111001"),
                    nyi("op1=1111010"),
                    invalid,

                    nyi("op1=1111100"),
                    nyi("Preload (PLD,PLDW)"),
                    nyi("op1=1111110"),
                    invalid));

            var SystemRegisterAccessAdvancedSimd = Mask(24, 2, "System register access, Advanced SIMD, floating-point, and Supervisor call",
                Select("  op0=0b00", 9, 0b111, n => n == 0b111,
                    SystemRegister_LdSt_64bitMove,
                    invalid),
                Select("  op0=0b01", 9, 0b111, n => n == 0b111,
                    SystemRegister_LdSt_64bitMove,
                    invalid),
                nyi("  op0=0b10"),
                Instr(Mnemonic.svc, InstrClass.Transfer | InstrClass.Call, i(0, 24)));

            // Note: this decoder is ARM7. ARM8 looks different.
            var UnconditionalDecoder = Mask(25, 3, "Unconditional instructions",
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,

                ExceptionSaveRestore,
                Branch_BranchLink_BlockDataTransfer,
                new PcDecoder(28,
                    Mask(20, 3, "Unconditional op1=0b110x?x??",
                        Instr(Mnemonic.stc, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.stc, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc, CP(8), CR(12), Mi8(2, w4)),

                        Instr(Mnemonic.stcl, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldcl, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.stcl, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldcl, CP(8), CR(12), Mi8(2, w4))),
                    Mask(20, 3, "Unconditional op1=0b110x?x??",
                        Instr(Mnemonic.stc2, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc2, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.stc2, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc2, CP(8), CR(12), Mi8(2, w4)),

                        Instr(Mnemonic.stc2l, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc2l, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.stc2l, CP(8), CR(12), Mi8(2, w4)),
                        Instr(Mnemonic.ldc2l, CP(8), CR(12), Mi8(2, w4)))),

                Mask(24, 1, "Unconditional op1=0b111....",
                    Mask(4, 1, "op",
                        CoprocessorDataOperations,
                        Mask(20, 1, "Unconditional op1=0b111....x",
                            Instr(Mnemonic.mcr2, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)),
                            Instr(Mnemonic.mrc2, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)))),
                    invalid));

            rootDecoder = Mask(28, 4,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,

                ConditionalDecoder,
                ConditionalDecoder,
                ConditionalDecoder,
                UnconditionalDecoder);
        }
    }
}
