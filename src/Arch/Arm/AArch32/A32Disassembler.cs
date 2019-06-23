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
using static Reko.Arch.Arm.AArch32.A32Disassembler.Decoder;
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{
    public partial class A32Disassembler : DisassemblerBase<AArch32Instruction>
    {
        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;
        private static readonly Dictionary<uint, RegisterStorage> bankedRegisters;

        private Arm32Architecture arch;
        private EndianImageReader rdr;
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
            public Opcode opcode;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public bool updateFlags = false;
            public bool writeback = false;
            public Opcode shiftOp = Opcode.Invalid;
            public MachineOperand shiftValue = null;
            public bool useQ = false;
            public bool userStmLdm = false;
            public int? vector_index = null;
            public ArmVectorData vectorData;

            public void Clear()
            {
                ops.Clear();
                iclass = InstrClass.Invalid;
                updateFlags = false;
                writeback = false;
                shiftOp = Opcode.Invalid;
                shiftValue = null;
                useQ = false;
                userStmLdm = false;
                vector_index = null;
                vectorData = ArmVectorData.INVALID;
            }

            public void Invalid()
            {
                Clear();
                opcode = Opcode.Invalid;
            }

            public AArch32Instruction MakeInstruction()
            {
                var instr = new A32Instruction
                {
                    InstructionClass = iclass,
                    opcode = opcode,
                    ops = ops.ToArray(),
                    ShiftType = shiftOp,
                    ShiftValue = shiftValue,
                    SetFlags = updateFlags,
                    Writeback = writeback,
                    UserStmLdm = userStmLdm,
                    vector_data = vectorData,
                    vector_index = vector_index,
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
            return Invalid();
        }

        private AArch32Instruction Invalid()
        {
            return new A32Instruction
            {
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }

        private (Opcode, int) DecodeImmShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            int shift_n = (int) bitmask(wInstr, 7, 0x1F);
            Opcode shift_t;
            switch (type)
            {
            case 0:
                shift_t = shift_n > 0 ? Opcode.lsl : Opcode.Invalid;
                break;
            case 1:
                shift_t = Opcode.lsr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 2:
                shift_t = Opcode.asr;
                shift_n = shift_n == 0 ? 32 : shift_n;
                break;
            case 3:
                shift_t = shift_n > 0 ? Opcode.ror : Opcode.rrx;
                shift_n = shift_n == 0 ? 1 : shift_n;
                break;
            default:
                throw new InvalidOperationException("impossiburu");
            }
            return (shift_t != Opcode.Invalid)
                ? (shift_t, shift_n)
                : (shift_t, 0);
        }

        private (Opcode, MachineOperand) DecodeRegShift(uint wInstr)
        {
            uint type = bitmask(wInstr, 5, 0x3);
            var shift_n = Registers.GpRegs[(int) bitmask(wInstr, 8, 0xF)];
            Opcode shift_t;
            switch (type)
            {
            case 0:
                shift_t = Opcode.lsl;
                break;
            case 1:
                shift_t = Opcode.lsr;
                break;
            case 2:
                shift_t = Opcode.asr;
                break;
            case 3:
                shift_t = Opcode.ror;
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
                return true;
            };
        }

        private static Mutator<A32Disassembler> vi(int offset, int [] sizes)
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

        private static Mutator<A32Disassembler> vf(int offset)
        {
            return (u, d) => { d.state.vectorData = d.VectorElementFloat(offset); return true; };
        }

        // bit which determines whether or not to use Qx or Dx registers in SIMD
        private static Mutator<A32Disassembler> q(int offset)
        {
            return (u, d) => { d.state.useQ = Bits.IsBitSet(u, offset); return true; };
        }

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

        /// <summary>
        /// rp - Register pair
        /// </summary>
        private static Mutator<A32Disassembler> rp(int offset)
        {
            offset *= 4;
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

        // Vector register
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
        /// Set the SIMD vector index
        /// </summary>
        private static Mutator<A32Disassembler> Ix(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                d.state.vector_index = (int) imm;
                return true;
            };
        }
        private (MemoryOperand, bool) MakeMemoryOperand(
            uint wInstr,
            RegisterStorage n,
            RegisterStorage m,
            Constant offset,
            Opcode shiftType,
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

        // Simple base register access.
        private static Mutator<A32Disassembler> M(int offset, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var iReg = bitmask(u, offset * 4, 0xF);
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, null, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }


        // 12-bit offset
        private static Mutator<A32Disassembler> Mo(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int) bitmask(u, 0, 0xFFF));
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
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
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, m, null, Opcode.Invalid, 0, dt);
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
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
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
                Opcode shiftType = Opcode.Invalid;
                (shiftType, shiftAmt) = d.DecodeImmShift(wInstr);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(wInstr, n, m, null, shiftType, shiftAmt, dt);
                d.state.ops.Add(mem);
                return true;
            };
        }

        // Memory access with 8-bit immediate offset (possibly shifted)
        private static Mutator<A32Disassembler> Mi(int shift, PrimitiveType dt)
        {
            return (u, d) =>
            {
                var n = Registers.GpRegs[bitmask(u, 16, 0xF)];
                var offset = Constant.Int32((int) bitmask(u, 0, 0xFF) << shift);
                MemoryOperand mem;
                (mem, d.state.writeback) = d.MakeMemoryOperand(u, n, null, offset, Opcode.Invalid, 0, dt);
                d.state.ops.Add(mem);
                return true;
            };
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
                if (d.state.shiftOp != Opcode.Invalid)
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
                    d.state.shiftOp = Opcode.Invalid;
                }
                else
                {
                    d.state.shiftOp = Opcode.ror;
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
                var op = d.state.opcode.ToString();
                string m;
                if (message == "")
                    m = op;
                else
                    m = $"{op} - {message}";
                d.NotYetImplemented(m, u);
                d.Invalid();
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
            if (dasm.state.shiftOp != Opcode.Invalid)
            {
                dasm.state.opcode = dasm.state.shiftOp;
                dasm.state.ops.Add(dasm.state.shiftValue);
                dasm.state.shiftValue = null;
                dasm.state.shiftOp = Opcode.Invalid;
            }
            return true;
        }














        private static Decoder Instr(Opcode opcode, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static Decoder Instr(Opcode opcode, InstrClass iclass, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, ArmVectorData.INVALID, mutators);
        }

        private static Decoder Instr(Opcode opcode, ArmVectorData vec, params Mutator<A32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, vec, mutators);
        }

        private static NyiDecoder nyi(string str)
        {
            return new NyiDecoder(str);
        }

        private static Decoder Mask(int sh, uint mask, params Decoder []decoders)
        { 
            return new MaskDecoder("", sh, mask, decoders);
        }

        private static Decoder Mask(string tag, int sh, uint mask, params Decoder[] decoders)
        {
            return new MaskDecoder(tag, sh, mask, decoders);
        }

        /// <summary>
        /// Create a decoder for 2 bitfields.
        /// </summary>
        private static Decoder Mask(
            string tag,
            int sh1, int len1,
            int sh2, int len2,
            params Decoder[] decoders)
        {
            return new BitfieldDecoder(
                tag,
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                }, decoders);
        }

        private static Decoder Mask(
            int sh1, int len1,
            int sh2, int len2,
            params Decoder[] decoders)
        {
            return new BitfieldDecoder(
                "",
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                }, decoders);
        }

        /// <summary>
        /// Create a decoder for 3 bitfields.
        /// </summary>
        private static Decoder Mask(
            int sh1, int len1,
            int sh2, int len2,
            int sh3, int len3,
            params Decoder[] decoders)
        {
            return new BitfieldDecoder(
                "",
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                    new Bitfield(sh3, len3),
                }, decoders);
        }

        /// <summary>
        /// Create a decoder for 4 bitfields.
        /// </summary>
        private static Decoder Mask(
            int sh1, int len1, 
            int sh2, int len2,
            int sh3, int len3,
            int sh4, int len4,
            params Decoder [] decoders)
        {
            return new BitfieldDecoder(
                "",
                new Bitfield[] {
                    new Bitfield(sh1, len1),
                    new Bitfield(sh2, len2),
                    new Bitfield(sh3, len3),
                    new Bitfield(sh4, len4)
                }, decoders);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders)
        {
            return new SparseMaskDecoder(shift, mask, decoders, invalid);
        }

        private static Decoder SparseMask(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
        {
            return new SparseMaskDecoder(shift, mask, decoders, @default);
        }

        private static Decoder Select(string tag, int shift, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder(tag, shift, mask, predicate, trueDecoder, falseDecoder);
        }

        private static Decoder Select(int shift, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
        {
            return new SelectDecoder("", shift, mask, predicate, trueDecoder, falseDecoder);
        }

        static A32Disassembler()
        {
            invalid = new InstrDecoder(Opcode.Invalid, InstrClass.Invalid, ArmVectorData.INVALID);
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

            var Stl = Instr(Opcode.stl, x(""));
            var Stlex = Instr(Opcode.stlex, x(""));
            var Strex = Instr(Opcode.strex, x(""));
            var Lda = Instr(Opcode.lda, x(""));
            var Ldaex = Instr(Opcode.ldaex, x(""));
            var Ldrex = Instr(Opcode.ldrex, x(""));
            var Stlexd = Instr(Opcode.stlexd, x(""));
            var Strexd = Instr(Opcode.strexd, x(""));
            var Ldaexd = Instr(Opcode.ldaexd, x(""));
            var Ldrexd = Instr(Opcode.ldrexd, x(""));
            var Stlb = Instr(Opcode.stlb, x(""));
            var Stlexb = Instr(Opcode.stlexb, x(""));
            var Strexb = Instr(Opcode.strexb, x(""));
            var Ldab = Instr(Opcode.ldab, x(""));
            var Ldaexb = Instr(Opcode.ldrexb, x(""));
            var Ldrexb = Instr(Opcode.ldaexb, x(""));
            var Stlh = Instr(Opcode.stlh, x(""));
            var Stlexh = Instr(Opcode.stlexh, x(""));
            var Strexh = Instr(Opcode.strexh, x(""));
            var Ldah = Instr(Opcode.ldah, x(""));
            var Ldaexh = Instr(Opcode.ldrexh, x(""));
            var Ldrexh = Instr(Opcode.ldaexh, x(""));

            var SynchronizationPrimitives = new MaskDecoder(23, 1,
                Mask(22, 1,
                    Instr(Opcode.swp, r(3), r(0), M(4,w4)),     //$TODO: deprecated in ARMv6 and later.
                    Instr(Opcode.swpb, r(3), r(0), M(4,w1))),   //$TODO: deprecated in ARMv6 and later.
                new MaskDecoder(20, 7,  // type || L
                    new MaskDecoder(8, 3,   // ex ord
                        Stl,
                        invalid,
                        Stlex,
                        Strex),
                    new MaskDecoder(8, 3,   // ex ord
                        Lda,
                        invalid,
                        Ldaex,
                        Ldrex),
                    new MaskDecoder(8, 3,   // ex ord
                        invalid,
                        invalid,
                        Stlexd,
                        Strexd),
                    new MaskDecoder(8, 3,   // ex ord
                        invalid,
                        invalid,
                        Ldaexd,
                        Ldrexd),

                    new MaskDecoder(8, 3,   // ex ord
                        Stlb,
                        invalid,
                        Stlexb,
                        Strexb),
                    new MaskDecoder(8, 3,   // ex ord
                        Ldab,
                        invalid,
                        Ldaexb,
                        Ldrexb),
                    new MaskDecoder(8, 3,   // ex ord
                        Stlh,
                        invalid,
                        Stlexh,
                        Strexh),
                    new MaskDecoder(8, 3,   // ex ord
                        Ldah,
                        invalid,
                        Ldaexh,
                        Ldrexh)));

            var Mul = Instr(Opcode.mul, s,r(4),r(0),r(2));
            var Mla = Instr(Opcode.mla, s,r(4),r(0),r(2),r(3));
            var Mls = Instr(Opcode.mls, s,r(4),r(0),r(2),r(3));
            var Umaal = Instr(Opcode.umaal, s,r(3),r(4),r(0),r(2));
            var Umull = Instr(Opcode.umull, s,r(3),r(4),r(0),r(2));
            var Umlal = Instr(Opcode.umlal, s,r(3),r(4),r(0),r(2));
            var Smull = Instr(Opcode.smull, s,r(3),r(4),r(0),r(2));
            var Smlal = Instr(Opcode.smlal, s,r(3),r(4),r(0),r(2));

            var MultiplyAndAccumulate = new MaskDecoder(20, 0xF,
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
            var LdrdRegister = Instr(Opcode.ldrd, rp(3),M_(w8));
            var LdrhRegister = Instr(Opcode.ldrh, r(3),M_(w2));
            var LdrsbRegister = Instr(Opcode.ldrsb, r(3),M_(s1));
            var LdrshRegister = Instr(Opcode.ldrsh, r(3),M_(s2));
            var Ldrht = Instr(Opcode.ldrht, r(3),Mh(w2));
            var Ldrsbt = Instr(Opcode.ldrsbt, r(3),Mh(s1));
            var Ldrsht = Instr(Opcode.ldrsht, r(3),Mh(s2));
            var StrdRegister = Instr(Opcode.strd, rp(3),Mx(w8));
            var StrhRegister = Instr(Opcode.strh, r(3),M_(w2));
            var Strht = Instr(Opcode.strht, r(3),Mh(w2));

            var LoadStoreDualHalfSbyteRegister = new MaskDecoder(24, 1,
                new MaskDecoder(20, 0x3,
                   new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid),
                    new MaskDecoder(5, 3,
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht)),
                new MaskDecoder(20, 1,
                    new MaskDecoder(5, 3,
                        invalid,
                        StrhRegister,
                        LdrdRegister,
                        StrdRegister),
                    new MaskDecoder(5, 3,
                        invalid,
                        LdrhRegister,
                        LdrsbRegister,
                        LdrshRegister)));

            var LdrdLiteral = Instr(Opcode.ldrd, rp(3), r(3), Mh(w8, false));
            var LdrhLiteral = Instr(Opcode.ldrh, r(3), Mh(w2, false));
            var LdrsbLiteral = Instr(Opcode.ldrsb, r(3),Mh(s1, false));
            var LdrshLiteral = Instr(Opcode.ldrsh, r(3),Mh(s2, false));
            var StrhImmediate = Instr(Opcode.strh, r(3),Mh(w2));
            var LdrdImmediate = Instr(Opcode.ldrd, rp(3),Mh(w8));
            var StrdImmediate = Instr(Opcode.strd, rp(3),Mh(w8));
            var LdrhImmediate = Instr(Opcode.ldrh, r(3),Mh(w2));
            var LdrsbImmediate = Instr(Opcode.ldrsb, r(3),Mh(s1));
            var LdrshImmediate = Instr(Opcode.ldrsh, r(3),Mh(s2));

            var LoadStoreDualHalfSbyteImmediate = Mask("LoadStoreDualHalfSbyteImmediate", 24, 1, 20, 2, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=000 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=001 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=010 op2
                        invalid,
                        Strht,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        invalid),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=011 op2
                        invalid,
                        Ldrht,
                        Ldrsbt,
                        Ldrsht),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=100 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        StrdImmediate),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=101 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=110 op2
                        invalid,
                        StrhImmediate,
                        new PcDecoder(16, LdrdImmediate, LdrdLiteral),
                        new PcDecoder(16, LdrshImmediate, LdrshLiteral)),
                    Mask(5, 3, // LoadStoreDualHalfSbyteImmediate Rn != pc P:W:op1=111 op2
                        invalid,
                        new PcDecoder(16, LdrhImmediate, LdrhLiteral),
                        new PcDecoder(16, LdrsbImmediate, LdrsbLiteral),
                        new PcDecoder(" 0b111", 16, LdrshImmediate, LdrshLiteral)));

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
                            new MaskDecoder(5, 3,
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
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 1:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 2:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 3:
                    return new MaskDecoder(5, 3,
                        invalid,
                        Strht,
                        invalid,
                        invalid);
                case 4:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 5:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                case 6:
                    return new MaskDecoder(5, 3,
                        invalid,
                        StrhImmediate,
                        LdrdImmediate,
                        StrdImmediate);
                case 7:
                    return new MaskDecoder(5, 3,
                        invalid,
                        LdrhImmediate,
                        LdrsbImmediate,
                        LdrshImmediate);
                }
                throw new InvalidOperationException("Impossible");
            });

            var ExtraLoadStore = new MaskDecoder("ExtraLoadStore", 22, 1,
                LoadStoreDualHalfSbyteRegister,
                LoadStoreDualHalfSbyteImmediate);

            var Mrs = Instr(Opcode.mrs, r(3),SR);
            var Msr = Instr(Opcode.msr, SR,r(0));
            var MrsBanked = Instr(Opcode.mrs, rb(22,1,8,1,16,4), r(0));
            var MsrBanked = Instr(Opcode.msr, rb(22,1,8,1,16,4), r(0));
            var MoveSpecialRegister = new MaskDecoder(21, 1,
                new MaskDecoder(9, 1,
                    Mrs,
                    MrsBanked),
                new MaskDecoder(9, 1,
                    Msr,
                    MsrBanked));

            var CyclicRedundancyCheck = new MaskDecoder(21, 3,
                new MaskDecoder(9, 1,
                    Instr(Opcode.crc32b, r(3), r(4), r(0)),
                    Instr(Opcode.crc32cb, r(3), r(4), r(0))),
                new MaskDecoder(9, 1,
                    Instr(Opcode.crc32h, r(3), r(4), r(0)),
                    Instr(Opcode.crc32ch, r(3), r(4), r(0))),
                new MaskDecoder(9, 1,
                    Instr(Opcode.crc32w, r(3), r(4), r(0)),
                    Instr(Opcode.crc32cw, r(3), r(4), r(0))),
                invalid);

            var Qadd = Instr(Opcode.qadd, r(3),r(0),r(4));
            var Qsub = Instr(Opcode.qsub, r(3),r(0),r(4));
            var Qdadd = Instr(Opcode.qdadd, r(3),r(0),r(4));
            var Qdsub = Instr(Opcode.qdsub, r(3),r(0),r(4));
            var IntegerSaturatingArithmetic = new MaskDecoder(21, 3,
                Qadd,
                Qsub,
                Qdadd,
                Qdsub);

            var Hlt = Instr(Opcode.hlt, i(8, 12, 0, 4));
            var Bkpt = Instr(Opcode.bkpt, i(8,12,0,4));
            var Hvc = Instr(Opcode.hvc, i(8,12,0,4));
            var Smc = Instr(Opcode.smc, i(0,4));
            var ExceptionGeneration = new MaskDecoder(21, 3,
                Hlt,
                Bkpt,
                Hvc,
                Smc);

            var Bx = Instr(Opcode.bx, InstrClass.Transfer, r(0));
            var Bxj = Instr(Opcode.bxj, InstrClass.Transfer, r(0));
            var Blx = Instr(Opcode.blx, J);
            var Clz = Instr(Opcode.clz, r(3),r(0));
            var Eret = Instr(Opcode.eret, InstrClass.Transfer);

            var ChangeProcessState = new MaskDecoder(16, 1, // op
                Mask(18, 0x3,
                    Instr(Opcode.cps, i(0, 5)),
                    nyi("CPS,CPSID,CPSIE mask=0b01"),
                    nyi("CPS,CPSID,CPSIE mask=0b10"),
                    Mask(17, 1,  // mask=0b11 M
                        Instr(Opcode.cps),
                        nyi("CPS,CPSID,CPSIE mask=0b11 m=1"))),
                Select(4, 1, n => n == 0, Instr(Opcode.setend, E(9,1)), invalid));

            var UncMiscellaneous = new MaskDecoder(22, 7,   // op0
                invalid,
                invalid,
                invalid,
                invalid,

                new MaskDecoder(20, 3,
                    Select(5, 1, n => n == 0, ChangeProcessState, invalid),
                    Select(4, 0xF, n => n == 0, Instr(Opcode.setpan, x("")), invalid),
                    invalid,
                    invalid),
                invalid,
                invalid,
                invalid);

        var Miscellaneous = new MaskDecoder(21, 3,   // op0
            new MaskDecoder(4, 7, // op1
                MoveSpecialRegister,
                invalid,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            new MaskDecoder(4, 7, // op1
                MoveSpecialRegister,
                Bx,
                Bxj,
                Instr(Opcode.blx, r(0)),

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            new MaskDecoder(4, 7, // op1
                MoveSpecialRegister,
                invalid,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                invalid,
                ExceptionGeneration),
            new MaskDecoder(4, 7, // op1
                MoveSpecialRegister,
                Clz,
                invalid,
                invalid,

                CyclicRedundancyCheck,
                IntegerSaturatingArithmetic,
                Eret,
                ExceptionGeneration));

            var HalfwordMultiplyAndAccumulate = new MaskDecoder(21, 0x3,
                Mask(5, 3,      // M:N
                    Instr(Opcode.smlabb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlatb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlabt, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlatt, r(4),r(0),r(2),r(3))),
                Mask(5, 3,
                    Instr(Opcode.smlawb, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smulwb, r(4),r(0),r(2)),
                    Instr(Opcode.smlawt, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smulwt, r(4),r(0),r(2))),
                Mask(5, 3,
                    Instr(Opcode.smlalbb, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlaltb, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlalbt, r(3),r(4),r(0),r(2)),
                    Instr(Opcode.smlaltt, r(3),r(4),r(0),r(2))),
                Mask(5, 3,
                    Instr(Opcode.smulbb, r(4),r(0),r(2)),
                    Instr(Opcode.smultb, r(4),r(0),r(2)),
                    Instr(Opcode.smulbt, r(4),r(0),r(2)),
                    Instr(Opcode.smultt, r(4),r(0),r(2))));

            var IntegerDataProcessingImmShift = new MaskDecoder(21, 7,
                Instr(Opcode.and, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.eor, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.sub, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.rsb, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.add, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.adc, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.sbc, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.rsc, s,r(3),r(4),r(0),Shi));

            var IntegerTestAndCompareImmShift = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),r(0),Shi),
                Instr(Opcode.teq, r(4),r(0),Shi),
                Instr(Opcode.cmp, r(4),r(0),Shi),
                Instr(Opcode.cmn, r(4),r(0),Shi));

            var LogicalArithmeticImmShift = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.mov, s,r(3),r(0),Shi,MovToShift),
                Instr(Opcode.bic, s,r(3),r(4),r(0),Shi),
                Instr(Opcode.mvn, s,r(3),r(0),Shi));

            var DataProcessingImmediateShift = new MaskDecoder(23, 3,
                IntegerDataProcessingImmShift, // 3 reg, imm shift
                IntegerDataProcessingImmShift,
                IntegerTestAndCompareImmShift,
                LogicalArithmeticImmShift);

            var IntegerDataProcessingRegShift = new MaskDecoder(21, 7,
               Instr(Opcode.and, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.eor, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.sub, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.rsb, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.add, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.adc, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.sbc, s,r(3),r(4),r(0),Shr),
               Instr(Opcode.rsc, s,r(3),r(4),r(0),Shr));

            var IntegerTestAndCompareRegShift = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),r(0),Shr),
                Instr(Opcode.teq, r(4),r(0),Shr),
                Instr(Opcode.cmp, r(4),r(0),Shr),
                Instr(Opcode.cmn, r(4),r(0),Shr));

            var LogicalArithmeticRegShift = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),r(0),Shr),
                Instr(Opcode.mov, s,r(4),r(0),Shr),
                Instr(Opcode.bic, s,r(3),r(4),r(0),Shr),
                Instr(Opcode.mvn, s,r(4),r(0),Shr));

            var DataProcessingRegisterShift = new MaskDecoder(23, 3,
                IntegerDataProcessingRegShift,
                IntegerDataProcessingRegShift,
                IntegerTestAndCompareRegShift,
                LogicalArithmeticRegShift);

            var IntegerDataProcessingTwoRegImm = new MaskDecoder(21, 7,
                Instr(Opcode.and, s,r(3),r(4),I),
                Instr(Opcode.eor, s,r(3),r(4),I),
                Instr(Opcode.sub, s,r(3),r(4),I),
                Instr(Opcode.rsb, s,r(3),r(4),I),
                Instr(Opcode.add, s,r(3),r(4),I),
                Instr(Opcode.adc, s,r(3),r(4),I),
                Instr(Opcode.sbc, s,r(3),r(4),I),
                Instr(Opcode.rsc, s,r(3),r(4),I));

            var LogicalArithmeticTwoRegImm = new MaskDecoder(21, 3,
                Instr(Opcode.orr, s,r(3),r(4),I),
                Instr(Opcode.mov, s,r(3),I),
                Instr(Opcode.bic, s,r(3),r(4),I),
                Instr(Opcode.mvn, s,r(3),I));

            var MoveHalfwordImm = new MaskDecoder(22, 1,
               Instr(Opcode.mov, r(3),Y),
               Instr(Opcode.movt, r(3),Yh));

            var IntegerTestAndCompareOneRegImm = new MaskDecoder(21, 3,
                Instr(Opcode.tst, r(4),I),
                Instr(Opcode.teq, r(4),I),
                Instr(Opcode.cmp, r(4),I),
                Instr(Opcode.cmn, r(4),I));

            var MsrImmediate = Instr(Opcode.msr, SR,i(0,12));
            var Nop = Instr(Opcode.nop);
            var Yield = Instr(Opcode.yield, x(""));
            var Wfe = Instr(Opcode.wfe);
            var Wfi = Instr(Opcode.wfi);
            var Sev = Instr(Opcode.sevl, x(""));
            var Sevl = Instr(Opcode.sevl, x(""));
            var ReservedNop = Instr(Opcode.nop);
            var Esb = Instr(Opcode.esb, x(""));
            var Dbg = Instr(Opcode.dbg, x(""));

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

            var DataProcessingImmediate = new MaskDecoder(23, 3,
                IntegerDataProcessingTwoRegImm,
                IntegerDataProcessingTwoRegImm,
                new MaskDecoder(20, 3,
                    MoveHalfwordImm,
                    IntegerTestAndCompareOneRegImm,
                    MoveSpecialRegisterAndHints,
                    IntegerTestAndCompareOneRegImm),
                LogicalArithmeticTwoRegImm);

            var DataProcessingAndMisc = Mask(25, 1,
                Mask(7, 1, 4, 1, // DataProcessingAndMisc op0=0 op2:op4
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingImmediateShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        Miscellaneous,
                        DataProcessingRegisterShift),
                    Select(20, 0b11001, n => n == 0b10000,
                        HalfwordMultiplyAndAccumulate,
                        DataProcessingImmediateShift),
                    Mask(5, 3, // DataProcessingAndMisc op0=0 op2:op4=11 op3
                        Mask(24, 1, // DataProcessingAndMisc op0=0 op2=1 op4=1 op3=0b00 op1
                            MultiplyAndAccumulate,
                            SynchronizationPrimitives),
                        ExtraLoadStore,
                        ExtraLoadStore,
                        ExtraLoadStore)),
                DataProcessingImmediate);

            var LdrLiteral = Instr(Opcode.ldr, r(3),Mo(w4));
            var LdrbLiteral = Instr(Opcode.ldrb, r(3),Mo(w1));
            var StrImm = Instr(Opcode.str, r(3),Mo(w4));
            var LdrImm = Instr(Opcode.ldr, r(3),Mo(w4));
            var StrbImm = Instr(Opcode.strb, r(3),Mo(w1));
            var LdrbImm = Instr(Opcode.ldrb, r(3),Mo(w1));
            
            var LoadStoreWordUnsignedByteImmLit = Mask(24, 1, 21, 1, 22, 1, 20, 1,
                // PW=0b00 00
                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))),

                Instr(Opcode.strt, r(3),Mo(w4)),
                Instr(Opcode.ldrt, r(3),Mo(w4)),
                Instr(Opcode.strbt, r(3),Mo(w1)),
                Instr(Opcode.ldrbt, r(3),Mo(w1)),

                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))),

                Instr(Opcode.str, r(3),Mo(w4)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldr, r(3),Mo(w4)),
                    Instr(Opcode.ldr, r(3),Mo(w4))),
                Instr(Opcode.strb, r(3),Mo(w1)),
                Select(16, 0xF, n => n != 0xF,
                    Instr(Opcode.ldrb, r(3),Mo(w1)),
                    Instr(Opcode.ldrb, r(3),Mo(w1))));

            var StrReg = Instr(Opcode.str, r(3),Mx(w4));
            var LdrReg = Instr(Opcode.ldr, r(3),Mx(w4));
            var StrbReg = Instr(Opcode.strb, r(3),Mx(w1));
            var LdrbReg = Instr(Opcode.ldrb, r(3),Mx(w1));
            var StrtReg = Instr(Opcode.strt, r(3),Mx(w4));
            var LdrtReg = Instr(Opcode.strt, r(3),Mx(w4));
            var StrbtReg = Instr(Opcode.strbt, r(3),Mx(w1));
            var LdrbtReg = Instr(Opcode.strbt, r(3),Mx(w1));
            var LoadStoreWordUnsignedByteRegister = Mask(24, 1, 20, 3,  // P:o2:W:o1
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

            var Sadd16 = Instr(Opcode.sadd16, x(""));
            var Sasx = Instr(Opcode.sasx, x(""));
            var Ssax = Instr(Opcode.ssax, x(""));
            var Ssub16 = Instr(Opcode.ssub16, x(""));
            var Sadd8 = Instr(Opcode.sadd8, r(3), r(4), r(0));
            var Ssub8 = Instr(Opcode.ssub8, r(3), r(4), r(0));
            var Qadd16 = Instr(Opcode.qadd16, r(3),r(4),r(0));
            var Qadd8 = Instr(Opcode.qadd8, r(3),r(4),r(0));
            var Qasx = Instr(Opcode.qasx, r(3),r(4),r(0));
            var Qsax = Instr(Opcode.qsax, r(3),r(4),r(0));
            var Qsub16 = Instr(Opcode.qsub16, r(3),r(4),r(0));
            var QSub8 = Instr(Opcode.qsub8, r(3),r(4),r(0));
            var Shadd16 = Instr(Opcode.shadd16, r(3),r(4),r(0));
            var Shasx = Instr(Opcode.shasx, x(""));
            var Shsax = Instr(Opcode.shsax, x(""));
            var Shsub16 = Instr(Opcode.shsub16, x(""));
            var Shadd8 = Instr(Opcode.shadd8, x(""));
            var Shsub8 = Instr(Opcode.shsub8, r(3),r(4),r(0));
            var Uadd16 = Instr(Opcode.uadd16, x(""));
            var Uasx = Instr(Opcode.uasx, r(3), r(4), r(0));
            var Usax = Instr(Opcode.usax, r(3),r(4),r(0));
            var Usub16 = Instr(Opcode.usub16, r(3),r(4),r(0));
            var Uadd8 = Instr(Opcode.uadd8, x(""));
            var Usub8 = Instr(Opcode.usub8, x(""));
            var Uqadd16 = Instr(Opcode.uqadd16, r(3), r(4), r(0));
            var Uqasx = Instr(Opcode.uqasx, r(3), r(4), r(0));
            var Uqsax = Instr(Opcode.uqsax, r(3), r(4), r(0));
            var Uqsub16 = Instr(Opcode.uqsub16, r(3),r(4),r(0));
            var Uqadd8 = Instr(Opcode.uqadd8, r(3), r(4), r(0));
            var Uqsub8 = Instr(Opcode.uqsub8, r(3),r(4),r(0));
            var Uhadd16 = Instr(Opcode.uhadd16, r(3),r(4),r(0));
            var Uhasx = Instr(Opcode.uhasx, x(""));
            var Uhsax = Instr(Opcode.uhsax, x(""));
            var Uhsub16 = Instr(Opcode.uhsub16, x(""));
            var Uhadd8 = Instr(Opcode.uhadd8, r(3),r(4),r(0));
            var Uhsub8 = Instr(Opcode.uhsub8, r(3),r(4),r(0));

            var ParallelArithmetic = new MaskDecoder(20, 7,
                invalid,
                new MaskDecoder(5, 7,
                    Sadd16,
                    Sasx,
                    Ssax,
                    Ssub16,

                    Sadd8,
                    invalid,
                    invalid,
                    Ssub8),
                new MaskDecoder(5, 7,
                    Qadd16,
                    Qasx,
                    Qsax,
                    Qsub16,

                    Qadd8,
                    invalid,
                    invalid,
                    QSub8),
                new MaskDecoder(5, 7,
                    Shadd16,
                    Shasx,
                    Shsax,
                    Shsub16,

                    Shadd8,
                    invalid,
                    invalid,
                    Shsub8),
                invalid,
                new MaskDecoder(5, 7,
                    Uadd16,
                    Uasx,
                    Usax,
                    Usub16,

                    Uadd8,
                    invalid,
                    invalid,
                    Usub8),
                new MaskDecoder(5, 7,
                    Uqadd16,
                    Uqasx,
                    Uqsax,
                    Uqsub16,

                    Uqadd8,
                    invalid,
                    invalid,
                    Uqsub8),
                new MaskDecoder(5, 7,
                    Uhadd16,
                    Uhasx,
                    Uhsax,
                    Uhsub16,

                    Uhadd8,
                    invalid,
                    invalid,
                    Uhsub8));

            var BitfieldInsert = Select(0, 0xF, n => n != 0xF,
                Instr(Opcode.bfi, r(3),r(0),B(7,5,16,5)),
                Instr(Opcode.bfc, r(3),B(7,5,16,5)));

            var BitfieldExtract = Mask(22, 1,
                Instr(Opcode.sbfx, r(3),r(0),i(7,5),i_p1(16,5)),
                Instr(Opcode.ubfx, r(3),r(0),i(7,5),i_p1(16,5)));

            var Saturate16Bit = nyi("Saturate16Bit");
            var Saturate32Bit = Mask("Saturate32Bit", 22, 1,
                Instr(Opcode.ssat, x("ssat")),
                Instr(Opcode.usat, r(3), i(16,5), r(0), Shi));
            var ExtendAndAdd = Mask(20, 7,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab16, x("*")), Instr(Opcode.sxtb16, x("*"))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.sxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.sxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.sxth, r(3),r(0),ShR(10,2))),
                
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab16, x("*")), Instr(Opcode.uxtb16, x("*"))),
                invalid,
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtab, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.uxtb, r(3),r(0),ShR(10,2))),
                Select(16, 0xF, n => n != 0xF, Instr(Opcode.uxtah, r(3),r(4),r(0),ShR(10,2)), Instr(Opcode.uxth, r(3),r(0),ShR(10,2))));
            var ReverseBitByte = Mask(22, 1,
                Mask(7, 1,
                    Instr(Opcode.rev, r(3),r(0)),
                    Instr(Opcode.rev16, r(3),r(0))),
                Mask(7, 1,
                    Instr(Opcode.rbit, r(3),r(0)),
                    Instr(Opcode.revsh, r(3),r(0))));

            var PermanentlyUndefined = Select(0, 4, n => n == 0b1110,
                Instr(Opcode.udf),
                invalid);

            var SignedMultiplyDivide = Mask("SignedMultiplyDivide", 20, 0x7, 
                new PcDecoder(12, 
                    Mask(5, 0x7,
                        Instr(Opcode.smlad, x("smlad")),
                        Instr(Opcode.smladx, x("smladx")),
                        Instr(Opcode.smlsd, x("smlsd")),
                        Instr(Opcode.smlsdx, x("smlsdx")),

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 0x7,
                        Instr(Opcode.smuad, x("smuad")),
                        Instr(Opcode.smuadx, x("smuadx")),
                        Instr(Opcode.smusd, x("smusd")),
                        Instr(Opcode.smusdx, x("smusdx")),

                        invalid,
                        invalid,
                        invalid,
                        invalid)),
                Select(5, 0x7, n => n != 0, invalid, Instr(Opcode.sdiv, x("sdiv"))),
                invalid,
                Select(5, 0x7, n => n != 0, invalid, Instr(Opcode.udiv, x("udiv"))),

                Mask(5, 0x7,
                    Instr(Opcode.smlald, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlaldx, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlsld, r(4),r(0),r(2),r(3)),
                    Instr(Opcode.smlsldx, r(4),r(0),r(2),r(3)),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(5, 0x7,
                    new PcDecoder(12, Instr(Opcode.smmla, x("smmla")), Instr(Opcode.smmul, x("smmul"))),
                    new PcDecoder(12, Instr(Opcode.smmlar, x("smmlar")), Instr(Opcode.smmulr, x("smmulr"))),
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Opcode.smmls, x("smmls")),
                    Instr(Opcode.smmlsr, x("smmlsr"))),
                invalid,
                invalid);

            var Media = Mask("Media", 23, 3,
                ParallelArithmetic,
                Mask(20, 7,
                    Mask(5, 7,  // op0=0b01_000
                        nyi("media - op0=0b01_000 op1=000"),
                        nyi("media - op0=0b01_000 op1=001"),
                        nyi("media - op0=0b01_000 op1=010"),
                        nyi("media - op0=0b01_000 op1=011"),

                        nyi("media - op0=0b01_000 op1=100"),
                        nyi("media - op0=0b01_000 op1=101"),
                        nyi("media - op0=0b01_000 op1=110"),
                        invalid),
                    nyi("media1 - 0b01001"),
                    Mask(5, 7,  // op0=0b01_010
                        Saturate32Bit,
                        Saturate16Bit,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        invalid,
                        Saturate32Bit,
                        invalid),
                    Mask(5, 7,  // media op0=0b01011
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid),

                    nyi("media1 - 0b01110"),
                    Mask("media - 0b01101", 5, 7,
                        invalid,
                        nyi("media - 0b01101 op1=001"),
                        invalid,
                        nyi("media - 0b01101 op1=011"),

                        invalid,
                        nyi("media - 0b01101 op1=101"),
                        invalid,
                        nyi("media - 0b01101 op1=111")),
                    Mask("media - 0b01110", 5, 7,      // media - 0b01110
                        Saturate32Bit,
                        nyi("media1 - 0b01110 - 001"),
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        nyi("media1 - 0b01110 - 101"),
                        Saturate32Bit,
                        invalid),
                    Mask(5, 7,
                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        ExtendAndAdd,

                        Saturate32Bit,
                        ReverseBitByte,
                        Saturate32Bit,
                        invalid)),
                SignedMultiplyDivide,
                Mask(20, 7,
                    Mask("media - 0b11000", 5, 7,
                        nyi("media - 0b11000 - 000"),
                        nyi("media - 0b11000 - 001"),
                        nyi("media - 0b11000 - 010"),
                        nyi("media - 0b11000 - 011"),

                        nyi("media - 0b11000 - 100"),
                        nyi("media - 0b11000 - 101"),
                        nyi("media - 0b11000 - 110"),
                        invalid),
                    nyi("media - 0b11001"),
                    Mask(5, 7, 
                        nyi("media - 0b11010 - 000"),
                        nyi("media - 0b11010 - 001"),
                        BitfieldExtract,
                        nyi("media - 0b11010 - 011"),

                        nyi("media - 0b11010 - 100"),
                        nyi("media - 0b11010 - 101"),
                        BitfieldExtract,
                        nyi("media - 0b11010 - 111")),
                    Mask("media - 0b11011", 5, 7,
                        nyi("media - 0b11011 - 000"),
                        invalid,
                        nyi("media - 0b11011 - 010"),
                        nyi("media - 0b11011 - 011"),

                        nyi("media - 0b11011 - 100"),
                        invalid,
                        nyi("media - 0b11011 - 110"),
                        nyi("media - 0b11011 - 111")),
                    Mask(5, 7, 
                        BitfieldInsert,
                        invalid,
                        nyi("media - 0b11100 - 010"),
                        nyi("media - 0b11100 - 011"),

                        BitfieldInsert,
                        invalid,
                        nyi("media - 0b11100 - 110"),
                        invalid),
                    Mask(5, 7, // media - 0b11101
                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid,

                        BitfieldInsert,
                        invalid,
                        invalid,
                        invalid),
                    Mask(5, 7,
                        nyi("media - 0b11110 - 000"),
                        invalid,
                        BitfieldExtract,
                        nyi("media - 0b11110 - 011"),

                        nyi("media - 0b11110 - 100"),
                        invalid,
                        BitfieldExtract,
                        invalid),
                    Mask(5, 7, // media - 0b11111
                        invalid,
                        invalid,
                        BitfieldExtract,
                        invalid,

                        invalid,
                        invalid,
                        BitfieldExtract,
                        PermanentlyUndefined)));

            var StmdaStmed = Instr(Opcode.stmda, w(21), r(4),Mr(0,16));
            var LdmdaLdmfa = Instr(Opcode.ldmda, w(21), r(4),Mr(0,16));
            var Stm =        Instr(Opcode.stm, w(21), r(4),Mr(0,16));
            var Ldm =        Instr(Opcode.ldm, w(21), r(4),Mr(0,16));
            var StmStmia =   Instr(Opcode.stm, w(21), r(4),Mr(0,16));
            var LdmLdmia =   Instr(Opcode.ldm, w(21), r(4),Mr(0,16));
            var StmdbStmfd = Instr(Opcode.stmdb, w(21), r(4),Mr(0,16));
            var LdmdbLDmea = Instr(Opcode.ldmdb, w(21), r(4),Mr(0,16));
            var StmibStmfa = Instr(Opcode.stmib, w(21), r(4),Mr(0,16));
            var LdmibLdmed = Instr(Opcode.ldmib, w(21), r(4),Mr(0,16));
            var StmUser = Mask(23, 0b11,
                Instr(Opcode.stmda, r(4), Mr(0, 16), u),
                Instr(Opcode.stmdb, r(4), Mr(0, 16), u),
                Instr(Opcode.stm, r(4), Mr(0, 16), u),
                Instr(Opcode.stmib, r(4), Mr(0, 16), u));
            var LdmUser = Mask(23, 0b11,
                Instr(Opcode.ldmda, r(4), Mr(0, 16), u),
                Instr(Opcode.ldmdb, r(4), Mr(0, 16), u),
                Instr(Opcode.ldm, r(4), Mr(0, 16), u),
                Instr(Opcode.ldmib, r(4), Mr(0, 16), u));
            var LoadStoreMultiple = Mask(22, 3, 20, 1, // P U op L
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

            var ExceptionSaveRestore = Mask("ExceptionSaveRestore", 22, 7, // PUS
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),

                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid),
                new MaskDecoder(20, 1, // L
                    invalid,
                    RfeRfeda),
                new MaskDecoder(20, 1, // L
                    SrcSrsda,
                    invalid));

            var BranchImmediate = new PcDecoder(28,
                new MaskDecoder(24, 1,
                    Instr(Opcode.b, J),
                    Instr(Opcode.bl, J)),
                Instr(Opcode.blx, X));

            var Branch_BranchLink_BlockDataTransfer = new MaskDecoder("Branch_BranchLink_BlockDataTransfer",
                25, 1,
                new PcDecoder(28,
                    LoadStoreMultiple,
                    ExceptionSaveRestore),
                BranchImmediate);

            var SystemRegister_64bitMove = new PcDecoder(28, 
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Opcode.mcrr, CP(8),i(4,4),r(3),r(4),CR(0)),
                        Instr(Opcode.mrrc, CP(8),i(4,4),r(3),r(4),CR(0)))),
                Mask(22, 1,
                    invalid,
                    Mask(20, 1,
                        Instr(Opcode.mcrr2, CP(8), i(4, 4), r(3), r(4), CR(0)),
                        Instr(Opcode.mrrc2, CP(8), i(4, 4), r(3), r(4), CR(0)))));


            var SystemRegister_LdSt = Select("SystemRegister_LdSt", 12, 0xF, n => n != 5, 
                invalid,
                Mask(20, 1,         // L (load)
                    Mask("SystemRegister_LdSt puw", 23, 2, 21, 1,
                        invalid,
                        Instr(Opcode.stc, CP(8),CR(12),Mi(2,w4)),
                        Instr(Opcode.stc, CP(8),CR(12),Mi(2,w4)),
                        Mask("SystemRegister_LdSt puw=011 d", 22, 1,
                            nyi("SystemRegister_LdSt puw=011 d=0"),
                            nyi("SystemRegister_LdSt puw=011 d=1")),

                        Instr(Opcode.stc, CP(8),CR(12),Mi(2,w4)),
                        Instr(Opcode.stc, CP(8),CR(12),Mi(2,w4)),
                        nyi("SystemRegister_LdSt puw=110"),
                        nyi("SystemRegister_LdSt puw=111")),
                    Mask(23, 2, 21, 1,
                        invalid,
                        nyi("SystemRegister_LdSt puw=001"),
                        Instr(Opcode.ldc, CP(8),CR(12),Mi(2,w4)),
                        nyi("SystemRegister_LdSt puw=011"),

                        Instr(Opcode.ldc, CP(8),CR(12),Mi(2,w4)),
                        Instr(Opcode.ldc, CP(8),CR(12),Mi(2,w4)),
                        nyi("SystemRegister_LdSt puw=110"),
                        nyi("SystemRegister_LdSt puw=111"))));

            var SystemRegister_LdSt_64bitMove = Select(21, 0b1101, n => n == 0,
                SystemRegister_64bitMove,
                SystemRegister_LdSt);

            var FloatingPointConvertToFixed =
                Mask("1 10x signed", 16, 1,
                    Mask("unsigned size:rounding", 7, 3,
                        Instr(Opcode.vcvt, U32F32, S12_22, S0_5),
                        Instr(Opcode.vcvtr, U32F32, S12_22, S0_5),
                        Instr(Opcode.vcvt, U32F64, S12_22, D5_0),
                        Instr(Opcode.vcvtr, U32F64, S12_22, D5_0)),
                    Mask("signed size:rounding", 7, 3,
                        Instr(Opcode.vcvt, S32F32, S12_22, S0_5),
                        Instr(Opcode.vcvtr, S32F32, S12_22, S0_5),
                        Instr(Opcode.vcvt, S32F64, S12_22, D5_0),
                        Instr(Opcode.vcvtr, S32F64, S12_22, D5_0)));

            var FloatingPointDataProcessing2regs = Mask(19, 1, 16, 3,
                Mask(7, 0b111,  // size:o3
                    invalid,
                    invalid,
                    invalid,
                    Instr(Opcode.vabs, F16, S12_22, S0_5),

                    Instr(Opcode.vmov, F32, S12_22, S0_5),
                    Instr(Opcode.vabs, F32, S12_22, S0_5),
                    Instr(Opcode.vmov, F64, D22_12, D5_0),
                    Instr(Opcode.vabs, F64, D22_12, D5_0)),
                Mask(7, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vneg, F16, S12_22, S0_5),
                        Instr(Opcode.vneg, F32, S12_22, S0_5),
                        Instr(Opcode.vneg, F64, D22_12, D5_0)),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vsqrt, F16, S12_22, S0_5),
                        Instr(Opcode.vsqrt, F32, S12_22, S0_5),
                        Instr(Opcode.vsqrt, F64, D22_12, D5_0))),
                nyi("Floating-point data-procesing (two registers) 0 010"),
                nyi("Floating-point data-procesing (two registers) 0 011"),

                Mask(7, 1,
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vcmp, F16, S12_22, S0_5),
                        Instr(Opcode.vcmp, F32, S12_22, S0_5),
                        Instr(Opcode.vcmp, F64, D22_12, D5_0)),
                    Mask(8, 3, 
                        invalid,
                        Instr(Opcode.vcmpe, F16, S12_22, S0_5),
                        Instr(Opcode.vcmpe, F32, S12_22, S0_5),
                        Instr(Opcode.vcmpe, F64, D22_12, D5_0))),
                Select("vcmpe #0", 0, 0xF, u => u == 0,
                    Mask("0101", 7, 3,
                        Instr(Opcode.vcmp, F32, S12_22, Imm0_r32),
                        Instr(Opcode.vcmpe, F32, S12_22, Imm0_r32),
                        Instr(Opcode.vcmp, F64, D22_12, Imm0_r64),
                        Instr(Opcode.vcmpe, F64, D22_12, Imm0_r64)),
                    invalid),
                nyi("Floating-point data-procesing (two registers) 0 110"),
                Mask("Floating-point data-procesing (two registers) 0 111 - op3", 6, 3, 
                    invalid,
                    nyi("vrintx (floating-point)"),
                    invalid,
                    Mask("sz", 8, 1,
                        Instr(Opcode.vcvt, F64F32, D22_12, S0_5),
                        Instr(Opcode.vcvt, F32F64, S12_22, D5_0))),

                Mask(7, 1,
                    Mask(8, 3, 
                        invalid,
                        Instr(Opcode.vcvt, F16U16, S12_22, S0_5),
                        Instr(Opcode.vcvt, F32U32, S12_22, S0_5),
                        Instr(Opcode.vcvt, F64U32, D22_12, S0_5)),
                    Mask(8, 3,
                        invalid,
                        Instr(Opcode.vcvt, F16S16, S12_22, S0_5),
                        Instr(Opcode.vcvt, F32S32, S12_22, S0_5),
                        Instr(Opcode.vcvt, F64S32, D22_12, S0_5))),
                nyi("Floating-point data-procesing (two registers) 1 001"),
                nyi("Floating-point data-procesing (two registers) 1 010"),
                nyi("Floating-point data-procesing (two registers) 1 011"),

                FloatingPointConvertToFixed,
                FloatingPointConvertToFixed,
                nyi("Floating-point data-procesing (two registers) 1 110"),
                nyi("Floating-point data-procesing (two registers) 1 111"));

            var FloatingPointDataProcessing3regs = Mask(23, 1, 20, 2, 6, 1,
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vmla, F16, S12_22, S16_7, S0_5),
                    Instr(Opcode.vmla, F32, S12_22, S16_7, S0_5),
                    Instr(Opcode.vmla, F64, D22_12,D7_16,D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vmls, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vmls, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vmls, F64, D22_12,D7_16,D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmls, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmls, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmls, F64, D22_12,D7_16,D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmla, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmla, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmla, F64, D22_12, D7_16, D5_0)),

                Mask(8, 0x3, 
                    invalid,
                    Instr(Opcode.vmul, F16, S12_22, S16_7, S0_5),
                    Instr(Opcode.vmul, F32, S12_22, S16_7, S0_5),
                    Instr(Opcode.vmul, F64, D22_12, D7_16, D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vnmul, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmul, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vnmul, F64, D22_12, D7_16, D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vadd, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vadd, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vadd, F64, D22_12, D7_16, D5_0)),
                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vsub, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vsub, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vsub, F64, D22_12, D7_16, D5_0)),

                Mask(8, 0x3,
                    invalid,
                    Instr(Opcode.vdiv, F16, S12_22, S16_7,S0_5),
                    Instr(Opcode.vdiv, F32, S12_22, S16_7,S0_5),
                    Instr(Opcode.vdiv, F64, D22_12,D7_16,D5_0)),
                invalid,
                Instr(Opcode.vfnms, x(" vfnms")),
                Instr(Opcode.vfnma, x(" vfnma")),

                Instr(Opcode.vfma, x("")),
                Instr(Opcode.vfms, x("")),
                invalid,
                invalid);

            var FloatingPointDataProcessing = new PcDecoder(28,
                Select(20, 0b1011, n => n != 0b1011,
                    FloatingPointDataProcessing3regs,
                    Mask(6, 1,
                        Mask(8, 1,  // FloatingPointMoveImmediate sz
                            Instr(Opcode.vmov, F32, S12_22, vfpImm32(16,4,0,4)),
                            Instr(Opcode.vmov, F64, D22_12, vfpImm64(16,4,0,4))),
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

            var AdvancedSimd_32bitTransfer = Mask("AdvancedSimd_32bitTransfer LC=??", 20, 1, 8, 1,
                nyi("AdvancedSimd_32bitTransfer LC=00"),
                Mask("AdvancedSimd_32bitTransfer LC=01 A=?xx", 23, 1,
                    Instr(Opcode.vmov, vW(22, 1, 5, 1), D7_16, r(3)),
                    Mask(6, 1,
                        Instr(Opcode.vdup, vW(22, 1, 5, 1), q(21), W7_16, r(3)),
                        invalid)),
                nyi("AdvancedSimd_32bitTransfer LC=10"),
                Mask("AdvancedSimd_32bitTransfer LC=11 U:opc1:opc2=??xxx", 22, 0b11,
                    Mask("U:opc1:opc2=00x?? = 0", 5, 0b11,
                        Instr(Opcode.vmov, I32, r(3), D7_16, Ix(21,1)),
                        Instr(Opcode.vmov, I16, r(3), D7_16, Ix(21,1)),
                        invalid,
                        Instr(Opcode.vmov, I16, r(3), D7_16, Ix(21,1))),
                    Instr(Opcode.vmov, I8, r(3), D7_16, Ix(21,1)),
                    nyi("AdvancedSimd_32bitTransfer LC=11 U:opc1:opc2=10xxx"),
                    Instr(Opcode.vmov, I8, r(3), D7_16, Ix(21,1))));

            //var AdvancedSIMDElementMovDuplicate = Mask("AdvancedSIMDElementMovDuplicate", 20, 1,
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
                Instr(Opcode.vmsr, i(16,4), r(3)),
                Instr(Opcode.vmrs, r(3), i(16,4)));

            var AdvancedSIMDandFloatingPoint32bitMove = Mask(8, 1,
                    Mask(21, 7,
                        Instr(Opcode.vmov, S16_7,r(3)),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        FloatingPointMoveSpecialReg),
                    AdvancedSimd_32bitTransfer);

            var AdvancedSimd_and_floatingpoint_LdSt = Mask("AdvancedSimd_and_floatingpoint_LdSt", 23, 2, 20, 2,
                invalid,
                invalid,
                invalid,
                invalid,

                Mask("PUWL: 0b0100", 8, 3,
                    invalid,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0100 size: 0b01"),
                    Instr(Opcode.vstmia, w(21), r(4), Ms(0, 16)),
                    Instr(Opcode.vstmia, w(21), r(4), Md(0, 16))),
                Mask("PUWL: 0b0101", 8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0101 size: 0b01"),
                    Instr(Opcode.vldmia, w(21), r(4), Ms(0,16)),
                    Instr(Opcode.vldmia, w(21), r(4), Md(0,16))),
                Mask("PUWL: 0b0110", 8, 3,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b00"),
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0110 size: 0b01"),
                    Instr(Opcode.vstmia, w(21), r(4), Ms(0,16)),
                    Instr(Opcode.vstmia, w(21), r(4), Md(0,16))),
                Mask("PUWL: 0b0111", 8, 3,
                    invalid,
                    invalid,
                    nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b10"),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, w(21), r(4), Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b0111 size: 0b11 xxxxxx1"))),

                Mask(8, 3, // size
                    invalid,
                    Instr(Opcode.vstr, I16, S12_22, Mi(1, w2)),
                    Instr(Opcode.vstr, S12_22, Mi(2, w4)),
                    Instr(Opcode.vstr, D22_12, Mi(2, w8))),
                Mask("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1001", 8, 3, // size
                    invalid,
                    Instr(Opcode.vldr, I16, S12_22, Mi(1, w2)),
                    Instr(Opcode.vldr, S12_22, Mi(2, w4)),
                    Instr(Opcode.vldr, D22_12, Mi(2, w8))),
                Mask(8, 3,
                    invalid,
                    invalid,
                    Instr(Opcode.vstmdb, w(21), r(4), Ms(0,16)),
                    Mask(0, 1,
                        Instr(Opcode.vstmdb, w(21), r(4), Md(0,16)),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1010 size: 0b11 xxxxxx1"))),
                Mask(8, 3, // AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011 size
                    invalid,
                    invalid,
                    Instr(Opcode.vstmdb, w(21), r(4), Ms(0, 16)),
                    Mask(0, 1,
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011 size: 0b11 xxxxxx0"),
                        nyi("AdvancedSimd_and_floatingpoint_LdSt - PUWL: 0b1011 size: 0b11 xxxxxx1"))),

                Mask(8, 3, // size
                    invalid,    
                    Instr(Opcode.vstr, I16, S12_22, Mi(1,w2)),
                    Instr(Opcode.vstr, S12_22, Mi(2,w4)),
                    Instr(Opcode.vstr, D22_12,Mi(2,w8))),
                Mask(8, 3, // size
                    invalid,
                    Instr(Opcode.vldr, I16, S12_22, Mi(1,w2)),
                    Instr(Opcode.vldr, S12_22, Mi(2,w4)),
                    Instr(Opcode.vldr, D22_12,Mi(2,w8))),
                invalid,
                invalid);

            var AdvancedSimd_and_floatingpoint64bitmove = Mask("AdvancedSimd_and_floatingpoint64bitmove", 22, 1,
                invalid,
                Mask(4, 1, // o3
                    invalid,
                    Select(6, 0x3, n => n != 0, // opc2
                        invalid,
                        Mask(20, 1, // op
                            Mask(8, 3, // size
                                invalid,
                                invalid,
                                nyi("vmov from 2 gp regs to 2 single floats"),
                                Instr(Opcode.vmov, D5_0, r(3),r(4))),
                            Mask(8, 3, // size
                                invalid,
                                invalid,
                                nyi("vmov to 2 gp regs from 2 single floats"),
                                Instr(Opcode.vmov, r(3), r(4), D5_0))))));

            var AdvancedSimd_LdSt_64bitmove = Select("AdvancedSimd_LdSt_64bitmove", 21, 0b1101, n => n == 0,
                AdvancedSimd_and_floatingpoint64bitmove,
                AdvancedSimd_and_floatingpoint_LdSt);

            var SystemRegister32BitMove = new PcDecoder(28,
                Mask(20, 1,
                    Instr(Opcode.mcr, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3)),
                    Instr(Opcode.mrc, CP(8),i(21,3),r(3),CR(16),CR(0),i(5,3))),
                invalid);

            var AdvancedSimd_ThreeRegisters = Mask("AdvancedSimd_ThreeRegisters", 24, 1,
                Mask(8, 0xF, // AdvancedSimd_ThreeRegisters - U = 0
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=00
                            invalid,
                            Instr(Opcode.vand, x(""))),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=01"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=10"),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0000 size=11")),
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=00 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=00 o1=0"),
                            Instr(Opcode.vand, q(6), W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=01"),
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0001 size=10 o1=0"),
                            Instr(Opcode.vorr, q(6), W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=11")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0010"),
                    Mask("AdvancedSimd_ThreeRegisters - U=0, opc=0b0011", 4, 1,
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=0
                            Instr(Opcode.vcgt, S8, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcgt, S16, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcgt, S32, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcgt, S64, q(6), W22_12, W5_0, W7_16)),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=0
                            Instr(Opcode.vcge, S8, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcge, S16, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcge, S32, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vcge, S64, q(6), W22_12, W5_0, W7_16))),

                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=0
                            Instr(Opcode.vshl, S8, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vshl, S16, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vshl, S32, q(6), W22_12, W5_0, W7_16),
                            Instr(Opcode.vshl, S64, q(6), W22_12, W5_0, W7_16)),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=0, opc=0b0100 o1=1
                            Instr(Opcode.vqshl, S8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vqshl, S16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vqshl, S32, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vqshl, S64, q(6), W22_12, W7_16, W5_0))),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0101"),
                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b0110 u=0  
                        Mask(20, 3,
                            Instr(Opcode.vmax, S8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, S16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, S32, q(6), W22_12, W7_16, W5_0),
                            invalid),
                        Mask(20, 3,
                            Instr(Opcode.vmin, S8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmin, S16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmin, S32, q(6), W22_12, W7_16, W5_0),
                            invalid)),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b0111"),

                    Mask("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1000", 4, 1,
                        Mask(20, 3,
                            Instr(Opcode.vadd, I8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vadd, I16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vadd, I32, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vadd, I64, q(6), W22_12, W7_16, W5_0)),
                        Instr(Opcode.vtst, x("*"))),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1001"),
                    Mask(6, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 
                            Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=0
                                Instr(Opcode.vpmax, S8, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmax, S16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmax, S32, D22_12,D7_16,D5_0),
                                invalid),
                            Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 0, opc=0b1010 Q=0 o1=1
                                Instr(Opcode.vpmin, S8, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, S16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, S32, D22_12,D7_16,D5_0),
                                invalid)),
                        invalid),
                    Mask(4, 1,
                        nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1011 o1=0"),
                        Mask(20, 3,
                            Instr(Opcode.vpadd, I8, D22_12,D7_16,D5_0),
                            Instr(Opcode.vpadd, I16, D22_12,D7_16,D5_0),
                            Instr(Opcode.vpadd, I32, D22_12,D7_16,D5_0),
                            invalid)),

                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1100"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1101"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1110"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 0, opc=0b1111")),

                Mask(8, 0xF, // AdvancedSimd_ThreeRegisters - U = 1
                    Mask("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0000 B=?", 4, 1, 9, 1,
                        Mask(20, 3,
                            Instr(Opcode.vhadd, U8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhadd, U16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhadd, U32, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhadd, U64, q(6), W22_12, W7_16, W5_0)),
                        Mask(20, 3,
                            Instr(Opcode.vhsub, U8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhsub, U16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhsub, U32, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vhsub, U64, q(6), W22_12, W7_16, W5_0)),
                        Instr(Opcode.vqadd, x("*")),
                        Instr(Opcode.vqsub, x("*"))),
                    Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001
                        Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=00 o1
                            nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=00 o1=0"),
                            Instr(Opcode.veor, q(6), W22_12 ,W7_16,W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=01"),
                        Instr(Opcode.vbit, q(6), W22_12, W7_16, W5_0),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0001 size=11")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0010"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0011"),

                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0100
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=0
                            Instr(Opcode.vshl, U8, q(6), W22_12 ,W5_0,W7_16),
                            Instr(Opcode.vshl, U16, q(6), W22_12 ,W5_0,W7_16),
                            Instr(Opcode.vshl, U32, q(6), W22_12 ,W5_0,W7_16),
                            Instr(Opcode.vshl, U64, q(6), W22_12 ,W5_0,W7_16)),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U=1, opc=0b0100 o1=1
                            Instr(Opcode.vqshl, U8, q(6), W22_12 ,W7_16,W5_0),
                            Instr(Opcode.vqshl, U16, q(6), W22_12 ,W7_16,W5_0),
                            Instr(Opcode.vqshl, U32, q(6), W22_12 ,W7_16,W5_0),
                            Instr(Opcode.vqshl, U64, q(6), W22_12 ,W7_16,W5_0))),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0101"),
                    Mask(4, 1, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110 max
                            Instr(Opcode.vmax, U8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, U16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, U32, q(6), W22_12, W7_16, W5_0),
                            invalid),
                        Mask(20, 3, // AdvancedSimd_ThreeRegisters - U = 1, opc=0b0110 min
                            Instr(Opcode.vmin, U8, q(6), W22_12 ,W7_16,W5_0),
                            Instr(Opcode.vmin, U16, q(6), W22_12 ,W7_16,W5_0),
                            Instr(Opcode.vmin, U32, q(6), W22_12 ,W7_16,W5_0),
                            invalid)),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b0111"),

                    Mask("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1000", 4, 1,
                        Mask(20, 3,
                            Instr(Opcode.vsub, I8, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vsub, I16, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vsub, I32, q(6), W22_12, W7_16, W5_0),
                            Instr(Opcode.vsub, I64, q(6), W22_12, W7_16, W5_0)),
                        nyi("AdvancedSimd_ThreeRegisters - U = 1, opc = 0b1000 op=1")),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1001"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1010"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1011"),

                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1100"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1101"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1110"),
                    nyi("AdvancedSimd_ThreeRegisters - U = 1, opc=0b1111")));

            var AdvancedSimd_TwoRegistersScalarExtension = Mask("AdvancedSimd_TwoRegistersScalarExtension", 23, 1,
                nyi("AdvancedSimd_TwoRegistersScalarExtension op1=0"),
                Mask("AdvancedSimd_TwoRegistersScalarExtension op1=1", 10, 1, 8, 1,
                    Mask("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:0", 4, 1,
                        nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:0 U=0"),
                        invalid),

                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=0:1"),
                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=1:0"),
                    nyi("AdvancedSimd_TwoRegistersScalarExtension op1=1 op3:op4=1:1")));

            var CoprocessorDataOperations = new PcDecoder(28,
                Instr(Opcode.cdp, CP(8), i(20,4), CR(12), CR(16), CR(0), i(5, 3)),
                Instr(Opcode.cdp2, CP(8), i(20,4), CR(12), CR(16), CR(0), i(5, 3)));

            var CoprocessorInstructionsAndSupervisorCall = Select("CoprocessorInstructionsAndSupervisorCall coproc=???x", 9, 0b111,
                u => u == 5,
                Mask("coproc=101x op1=??xxxx", 24, 0b11,
                    Select("op1=00?x?x", 21, 0b101, u => u == 0,
                        AdvancedSimd_LdSt_64bitmove,
                        AdvancedSimd_and_floatingpoint_LdSt),
                    AdvancedSimd_and_floatingpoint_LdSt,
                    Mask("10xxxx op=?", 4, 1,
                        FloatingPointDataProcessing,
                        AdvancedSIMDandFloatingPoint32bitMove),
                    Instr(Opcode.svc, InstrClass.Transfer | InstrClass.Call, i(0,24))),
                Mask("coproc!=101x ??xxxx", 24, 0b11,
                    Select("op1=00?x?x", 21, 0b101, u => u == 0,
                        SystemRegister_64bitMove,
                        Mask(20, 1,
                            Instr(Opcode.stc, CP(8), CR(12), Mi(2, w4)),
                            Instr(Opcode.ldc, CP(8), CR(12), Mi(2, w4)))),
                    Select("op1=00?x?x", 21, 0b101, u => u == 0,
                        SystemRegister_64bitMove,
                        Mask(20, 1,
                            Instr(Opcode.stc, CP(8), CR(12), Mi(2, w4)),
                            Instr(Opcode.ldc, CP(8), CR(12), Mi(2, w4)))),
                    Mask("10xxxx op=?", 4, 1,
                        CoprocessorDataOperations,
                        Mask("10xxx? op=0", 20, 1,
                            Instr(Opcode.mcr, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)),
                            Instr(Opcode.mrc, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)))),
                    Instr(Opcode.svc, InstrClass.Transfer | InstrClass.Call, i(0,24))));

            Decoder SystemRegister_AdvancedSimd_FloatingPoint = Mask("SystemRegister_AdvancedSimd_FloatingPoint", 24, 3,
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
                    Instr(Opcode.svc, InstrClass.Transfer | InstrClass.Call, i(0,24)));

            var ConditionalDecoder = new CondMaskDecoder(25, 0x7,
                DataProcessingAndMisc,
                DataProcessingAndMisc,
                LoadStoreWordUnsignedByteImmLit,
                new MaskDecoder(4, 1,
                    LoadStoreWordUnsignedByteRegister,
                    Media),
                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                CoprocessorInstructionsAndSupervisorCall,
                CoprocessorInstructionsAndSupervisorCall);

            var VmullIntegerPolynomial = nyi("VmullIntegerPolynomial");

            var AdvanceSimd_ThreeRegistersDifferentLength = Mask("AdvanceSimd_ThreeRegistersDifferentLength",
                    8, 0xF,
                    Mask(20, 3, 
                        Instr(Opcode.vaddl, U8, Q(22,1,12,4), D7_16, D5_0),
                        Instr(Opcode.vaddl, U16, Q(22, 1, 12, 4), D7_16, D5_0),
                        Instr(Opcode.vaddl, U32, Q(22, 1, 12, 4), D7_16, D5_0),
                        invalid),
                    Instr(Opcode.vaddw, x("*")),
                    Instr(Opcode.vsubl, x("*")),
                    Instr(Opcode.vsubw, x("*")),

                    Mask(24, 1,
                        Instr(Opcode.vaddhn, x("*")),
                        Instr(Opcode.vraddhn, x("*"))),
                    Instr(Opcode.vabal, x("*")),
                    Mask(24, 1,
                        Instr(Opcode.vsubhn, x("*")),
                        Instr(Opcode.vsubhn, x("*"))),
                    Instr(Opcode.vabdl, x("vabdl - integer")),

                    Instr(Opcode.vmlal, x("*")),
                    Mask(24, 1,
                        Instr(Opcode.vqdmlal, x("*")),
                        invalid),
                    Instr(Opcode.vmlsl, x("*")),
                    Mask(24, 1,
                        Instr(Opcode.vqdmlsl, x("*")),
                        invalid),

                    VmullIntegerPolynomial,
                    Mask(24, 1,
                        Instr(Opcode.vqdmull, x("*")),
                        invalid),
                    VmullIntegerPolynomial,
                    invalid);

            var AdvancedSimd_TwoRegisterOrThreeRegisters = Select(
                "AdvancedSimd_TwoRegisterOrThreeRegisters",
                20, 0x3,
                n => n == 0b11,
                Mask(24, 1,
                    Instr(Opcode.vext, U8, q(6), W22_12, W7_16, W5_0, i(8, 4, 3)),
                    nyi("AdvancedSimd_TwoRegisterOrThreeRegisters op1==0b11 op0=1")),
                Mask("AdvancedSimd_TwoRegisterOrThreeRegisters op1!=0b11 op=0",
                    6, 1,
                    AdvanceSimd_ThreeRegistersDifferentLength,
                    nyi("AdvanceSimd_TwoRegistersScalar")));

            var vmov_A1 = Instr(Opcode.vmov, I32, q(6), W22_12, Is(24,1,16,3,0,4));
            var vmov_A3 = Instr(Opcode.vmov, I16, q(6), W22_12, Is(24,1,16,3,0,4));
            var vmov_A4 = Instr(Opcode.vmov, DtFromCmode(8,2), q(6), W22_12, Is(24,1,16,3,0,4));

            var AdvancedSimd_OneRegisterModifiedImmediate = Mask("AdvancedSimd_OneRegisterModifiedImmediate", 8,4,5,1,
                vmov_A1,
                Instr(Opcode.vmvn, I32, q(6), W22_12, Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                vmov_A1,
                Instr(Opcode.vmvn, I32, q(6), W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                vmov_A1,
                Instr(Opcode.vmvn, I32, q(6), W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                vmov_A1,
                Instr(Opcode.vmvn, I32, W22_12 ,Is(24,1,16,3,0,4)),
                Instr(Opcode.vorr, x("immediate - A1")),
                Instr(Opcode.vbic, x("immediate - A1")),

                vmov_A3,
                Instr(Opcode.vmvn, x("immediate - A2")),
                Instr(Opcode.vorr, x("immediate - A2")),
                Instr(Opcode.vbic, x("immediate - A2")),

                vmov_A3,
                Instr(Opcode.vmvn, x("immediate - A2")),
                Instr(Opcode.vorr, x("immediate - A2")),
                Instr(Opcode.vbic, x("immediate - A2")),

                vmov_A4,
                Instr(Opcode.vmvn, x("immediate - A3")),
                vmov_A4,
                Instr(Opcode.vmvn, x("immediate - A3")),

                vmov_A4,
                Instr(Opcode.vmov, I64, q(6), W(22,1, 12,4), Is(24,1, 16,3, 0,4)),
                vmov_A4,
                invalid);



            var AdvancedSimd_TwoRegisterShiftAmount = nyi("AdvancedSimd_TwoRegisterShiftAmount");

            var AdvancedSimd_ShiftsAndImmediate = Select(7, 0b111000000000001, n => n == 0,
                AdvancedSimd_OneRegisterModifiedImmediate,
                AdvancedSimd_TwoRegisterShiftAmount);

            var AdvancedSimd = Mask(23, 1,
                AdvancedSimd_ThreeRegisters,
                Mask(4, 1,
                    AdvancedSimd_TwoRegisterOrThreeRegisters,
                    AdvancedSimd_ShiftsAndImmediate));

            var AdvancedSimdElementLoadStore = Mask("AdvancedSimdElementLoadStore A=?", 23, 1,
                Mask("AdvancedSimdElementLoadStore A=0 B=????", 8, 0b1111,
                    Mask(21, 1,
                        Instr(Opcode.vst4, vi_ld4, Vel(4,1), Mve),
                        Instr(Opcode.vld4, vi_ld4, Vel(4,1), Mve)),
                    Mask(21, 1,
                        Instr(Opcode.vst4, vi_ld4, Vel(4,2), Mve),
                        Instr(Opcode.vld4, vi_ld4, Vel(4,2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0010"),
                    Mask(21, 1,
                        Instr(Opcode.vst2, vi_ld4, Vel(4, 1), Mve),
                        Instr(Opcode.vld2, vi_ld4, Vel(4, 1), Mve)),

                    Mask(21, 1,
                        Instr(Opcode.vst3, vi_ld3, Vel(4, 1), Mve),
                        Instr(Opcode.vld3, vi_ld3, Vel(4, 1), Mve)),
                    Mask(21, 1,
                        Instr(Opcode.vst3, vi_ld3, Vel(4, 2), Mve),
                        Instr(Opcode.vld3, vi_ld3, Vel(4, 2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0110"),
                    nyi("AdvancedSimdElementLoadStore A=0 B=0111"),

                    Mask(21, 1,
                        Instr(Opcode.vst2, vi_ld1, Vel(4, 1), Mve),
                        Instr(Opcode.vld2, vi_ld1, Vel(4, 1), Mve)),
                    Mask(21, 1,
                        Instr(Opcode.vst2, vi_ld1, Vel(2, 2), Mve),
                        Instr(Opcode.vld2, vi_ld1, Vel(2, 2), Mve)),
                    nyi("AdvancedSimdElementLoadStore A=0 B=1010"),
                    nyi("AdvancedSimdElementLoadStore A=0 B=1011"),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask("AdvancedSimdElementLoadStore A=1 B=????", 8, 0b1111,
                    Mask(21, 1,
                        Instr(Opcode.vst1, vi_ld1, Vel(1, 1), Mve),
                        Instr(Opcode.vld1, vi_ld1, Vel(1, 1), Mve)),
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

            var Barriers = Mask(4, 0xF,
                invalid,
                Instr(Opcode.clrex, x("")),
                invalid,
                invalid,

                Instr(Opcode.dsb, Ba(0,4)),
                Instr(Opcode.dmb, Ba(0,4)),
                Instr(Opcode.isb, Ba(0,4)),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var MemoryHintsAndBarriers = Mask(25, 1,
                Mask(21, 1,
                    nyi("Preload (immediate)"),
                    Mask(22, 7,
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

            var vext = Instr(Opcode.vext, U8, q(6), W22_12, W7_16, W5_0, i(8, 4));
            
            var AdvancedSimdDataProcessing = Mask("AdvancedSimdDataProcessing", 23, 1,
                AdvancedSimd_ThreeRegisters,
                Mask("AdvancedSimdDataProcessing A=1x??x", 20, 0b11,
                    Mask("AdvancedSimdDataProcessing A=1x00?", 19, 1,
                        Mask("AdvancedSimdDataProcessing A=1x000 C=????", 4, 0b1111,
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_OneRegisterModifiedImmediate,
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_OneRegisterModifiedImmediate,

                            nyi("AdvancedSimdDataProcessing A=1x000 C=0100"),
                            AdvancedSimd_OneRegisterModifiedImmediate,
                            nyi("AdvancedSimdDataProcessing A=1x000 C=0110"),
                            AdvancedSimd_OneRegisterModifiedImmediate,

                            AdvanceSimd_ThreeRegistersDifferentLength,
                            nyi("AdvancedSimdDataProcessing A=1x000 C=1001"),
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            nyi("AdvancedSimdDataProcessing A=1x000 C=1011"),

                            nyi("AdvancedSimdDataProcessing A=1x000 C=1100"),
                            nyi("AdvancedSimdDataProcessing A=1x000 C=1101"),
                            nyi("AdvancedSimdDataProcessing A=1x000 C=1110"),
                            nyi("AdvancedSimdDataProcessing A=1x000 C=1111")),
                        Mask("AdvancedSimdDataProcessing A=1x001 C=????", 4, 0b1111,
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_TwoRegisterShiftAmount,
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_TwoRegisterShiftAmount,

                            nyi("AdvancedSimdDataProcessing A=1x001 C=0100"),
                            AdvancedSimd_TwoRegisterShiftAmount,
                            nyi("AdvancedSimdDataProcessing A=1x001 C=0110"),
                            AdvancedSimd_TwoRegisterShiftAmount,

                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_TwoRegisterShiftAmount,
                            AdvanceSimd_ThreeRegistersDifferentLength,
                            AdvancedSimd_TwoRegisterShiftAmount,

                            nyi("AdvancedSimdDataProcessing A=1x001 C=1100"),
                            AdvancedSimd_TwoRegisterShiftAmount,
                            nyi("AdvancedSimdDataProcessing A=1x001 C=1110"),
                            AdvancedSimd_TwoRegisterShiftAmount)),
                    Mask("AdvancedSimdDataProcessing A=1x01x C=????", 4, 0b1111,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,

                        nyi("AdvancedSimdDataProcessing A=1x01x C=0100"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x01x C=0110"),
                        AdvancedSimd_TwoRegisterShiftAmount,

                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        
                        nyi("AdvancedSimdDataProcessing A=1x01x C=1100"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x01x C=1110"),
                        AdvancedSimd_TwoRegisterShiftAmount),
                    Mask("AdvancedSimdDataProcessing A=1x10x C=????", 4, 0b1111,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,

                        nyi("AdvancedSimdDataProcessing A=1x10x C=0100"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x10x C=0110"),
                        AdvancedSimd_TwoRegisterShiftAmount,

                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        AdvanceSimd_ThreeRegistersDifferentLength,
                        AdvancedSimd_TwoRegisterShiftAmount,

                        nyi("AdvancedSimdDataProcessing A=1x10x C=1100"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x10x C=1110"),
                        AdvancedSimd_TwoRegisterShiftAmount),
                    Mask("AdvancedSimdDataProcessing A=1x11x C=????", 4, 0b1111,
                        nyi("AdvancedSimdDataProcessing A=1x11x C=0000"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x11x C=0010"),
                        AdvancedSimd_TwoRegisterShiftAmount,

                        nyi("AdvancedSimdDataProcessing A=1x11x C=0100"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x11x C=0110"),
                        AdvancedSimd_TwoRegisterShiftAmount,

                        nyi("AdvancedSimdDataProcessing A=1x11x C=1000"),
                        AdvancedSimd_TwoRegisterShiftAmount,
                        nyi("AdvancedSimdDataProcessing A=1x11x C=1010"),
                        AdvancedSimd_TwoRegisterShiftAmount,

                        vext,
                        AdvancedSimd_TwoRegisterShiftAmount,
                        Mask(4, 1, 
                            vext,
                            nyi("AdvancedSimdDataProcessing A=1x11x C=1110 U=1")),
                        AdvancedSimd_TwoRegisterShiftAmount)));

        var MemoryHintsAdvancedSimdMiscellaneous = Mask("MemoryHintsAdvancedSimdMiscellaneous", 24, 0b111,
                invalid,
                Select("op1=001????", 20, 0b1111, u => u == 0,
                    Mask("Rn=xxx?", 16, 1,
                        Mask("op2=xx?x", 5, 1,
                            ChangeProcessState,
                            invalid),
                        Select("op2=0000", 4, 0b1111, u => u == 0,
                            Instr(Opcode.setend, E(9, 1)), invalid)),
                    invalid),
                AdvancedSimdDataProcessing,
                AdvancedSimdDataProcessing,

                Mask("op1=100x???", 20, 0b111,
                    AdvancedSimdElementLoadStore,
                    nyi("Unallocated memory hint (treat as NOP)"),
                    AdvancedSimdElementLoadStore,
                    invalid,

                    AdvancedSimdElementLoadStore,
                    nyi("Preload instruction"),
                    AdvancedSimdElementLoadStore,
                    invalid),
                Mask("op1=101xxx?", 20, 1,
                    invalid,
                    Mask("op1=101???1", 21, 0b111,
                        nyi("op1=1010001"),
                        nyi("op1=1010011"),
                        Instr(Opcode.pldw, x("*")),
                        Barriers,
                        
                        nyi("op1=1011001"),
                        nyi("op1=1011011"),
                        Instr(Opcode.pld, Mo(w4)),
                        nyi("op1=1011111"))),
                nyi("op1=110xxxx"),
                nyi("op1=111xxxx"));

            var UnconditionalDecoder = Mask("Unconditional", 25, 0b111,
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,
                MemoryHintsAdvancedSimdMiscellaneous,

                nyi("Unconditional op1=0b100xxxxx"),
                Branch_BranchLink_BlockDataTransfer,
                new PcDecoder(28, 
                    Mask("Unconditional op1=0b110x?x??", 20, 7,
                        Instr(Opcode.stc, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.stc, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc, CP(8), CR(12), Mi(2, w4)),

                        Instr(Opcode.stcl, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldcl, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.stcl, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldcl, CP(8), CR(12), Mi(2, w4))),
                    Mask("Unconditional op1=0b110x?x??", 20, 7,
                        Instr(Opcode.stc2, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc2, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.stc2, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc2, CP(8), CR(12), Mi(2, w4)),

                        Instr(Opcode.stc2l, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc2l, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.stc2l, CP(8), CR(12), Mi(2, w4)),
                        Instr(Opcode.ldc2l, CP(8), CR(12), Mi(2, w4)))),

                Mask("Unconditional op1=0b111....", 24, 1,
                    Mask("op", 4, 1,
                        CoprocessorDataOperations,
                        Mask("Unconditional op1=0b111....x", 20, 1,
                            Instr(Opcode.mcr2, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)),
                            Instr(Opcode.mrc2, CP(8), i(21,3), r(3), CR(16), CR(0), i(5,3)))),
                    invalid));

            var unconditional = Mask("Unconditional", 25, 7,
                UncMiscellaneous,
                AdvancedSimd,
                new MaskDecoder(20, 1,
                    AdvancedSimdElementLoadStore,
                    MemoryHintsAndBarriers),
                new MaskDecoder(20, 1,
                    MemoryHintsAndBarriers,
                    invalid),

                Branch_BranchLink_BlockDataTransfer,
                Branch_BranchLink_BlockDataTransfer,
                SystemRegister_AdvancedSimd_FloatingPoint,
                SystemRegister_AdvancedSimd_FloatingPoint);


            rootDecoder = Mask(28, 0x0F,
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
