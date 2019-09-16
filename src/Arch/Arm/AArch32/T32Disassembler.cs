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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{
    using Decoder = Reko.Core.Machine.Decoder<T32Disassembler, Opcode, AArch32Instruction>;


    /// <summary>
    /// Disassembles machine code in the ARM T32 encoding into 
    /// ARM32 instructions.
    /// </summary>
    public partial class T32Disassembler : DisassemblerBase<AArch32Instruction>
    {
        private const uint ArmRegPC = 0xFu;

        private static readonly Decoder[] decoders;
        private static readonly Decoder invalid;

        private readonly ImageReader rdr;
        private readonly ThumbArchitecture arch;
        private Address addr;
        private int itState;
        private ArmCondition itCondition;
        private DasmState state;

        public T32Disassembler(ThumbArchitecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.itState = 0;
            this.itCondition = ArmCondition.AL;
        }

        public override AArch32Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out var wInstr))
                return null;
            this.state = new DasmState();
            var instr = decoders[wInstr >> 13].Decode(wInstr, this);
            instr.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            instr.InstructionClass |= instr.condition != ArmCondition.AL ? InstrClass.Conditional : 0;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            if ((itState & 0x1F) == 0x10)
            {
                // No more IT bits, reset condition back to normal.
                itCondition = ArmCondition.AL;
                itState = 0;
            }
            else if (itState != 0 && instr.opcode != Opcode.it)
            {
                // We're still under the influence of the IT instruction.
                var bit = ((itState >> 4) ^ ((int) this.itCondition)) & 1;
                instr.condition = (ArmCondition) ((int) this.itCondition ^ bit);
                itState <<= 1;
            }
            return instr;
        }

        private class DasmState
        {
            public Opcode opcode;
            public InstrClass iclass;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public ArmCondition cc = ArmCondition.AL;
            public bool updateFlags = false;
            public bool wide = false;
            public bool writeback = false;
            public Opcode shiftType = Opcode.Invalid;
            public MachineOperand shiftValue = null;
            public ArmVectorData vectorData = ArmVectorData.INVALID;
            public bool useQ = false;
            public uint vectorShiftAmt = 0;

            internal void Invalid()
            {
                throw new NotImplementedException();
            }

            public AArch32Instruction MakeInstruction()
            {
                return new T32Instruction
                {
                    opcode = opcode,
                    InstructionClass = iclass,
                    ops = ops.ToArray(),
                    condition = cc,
                    SetFlags = updateFlags,
                    Wide = wide,
                    Writeback = writeback,
                    ShiftType = shiftType,
                    ShiftValue = shiftValue,
                    vector_data = vectorData,
                };
            }
        }

        private (Opcode, MachineOperand) DecodeImmShift(uint wInstr, Bitfield bfType, Bitfield[] bfImm)
        {
            var type = bfType.Read(wInstr);
            var imm = Bitfield.ReadFields(bfImm, wInstr);
            switch (type)
            {
            case 0:
                if (imm != 0)
                    return (Opcode.lsl, ImmediateOperand.UInt32(imm));
                else
                    return (Opcode.Invalid, null); 
            case 1: return (Opcode.lsr, ImmediateOperand.UInt32(imm == 0 ? 32 : imm));
            case 2: return (Opcode.asr, ImmediateOperand.UInt32(imm == 0 ? 32 : imm));
            case 3:
                if (imm == 0)
                    return (Opcode.rrx, ImmediateOperand.UInt32(1));
                else
                    return (Opcode.ror, ImmediateOperand.UInt32(imm));
            }
            throw new InvalidOperationException("Type must be [0..3].");
        }

        private MachineOperand ModifiedSimdImmediate(uint wInstr, uint imm8)
        {
            ulong Replicate2(uint value)
            {
                return (((ulong) value) << 32) | value;
            }

            ulong Replicate4(uint value)
            {
                var v = (ulong) (ushort) value;
                return (v << 48) | (v << 32) | (v << 16) | v;
            }

            int op = SBitfield(wInstr, 5, 1);
            int cmode = SBitfield(wInstr, 8, 4);
            ulong imm64 = 0;
            switch (cmode >> 1)
            {
            case 0:
                imm64 = Replicate2(imm8); break;
            case 1:
                imm64 = Replicate2(imm8 << 8); break;
            case 2:
                imm64 = Replicate2(imm8 << 16); break;
            case 3:
                imm64 = Replicate2(imm8 << 24); break;
            case 4:
                imm64 = Replicate4(imm8); break;
            case 5:
                imm64 = Replicate4(imm8 << 8); break;
            case 6:
                if ((cmode & 1) == 0) {
                    imm64 = Replicate2((imm8 << 8) | 0xFF);
                } else {
                    imm64 = Replicate2((imm8 << 16) | 0xFFFF);
                }
                break;
            case 7:
                throw new NotImplementedException();
                /*
                if (cmode < 0 > == '0' && op == '0') {
                    imm64 = Replicate(imm8, 8);
                }
                if (cmode < 0 > == '0' && op == '1') {
                    imm8a = Replicate(imm8 < 7 >, 8); imm8b = Replicate(imm8 < 6 >, 8);
                    imm8c = Replicate(imm8 < 5 >, 8); imm8d = Replicate(imm8 < 4 >, 8);
                    imm8e = Replicate(imm8 < 3 >, 8); imm8f = Replicate(imm8 < 2 >, 8);
                    imm8g = Replicate(imm8 < 1 >, 8); imm8h = Replicate(imm8 < 0 >, 8);
                    imm64 = imm8a:imm8b: imm8c: imm8d: imm8e: imm8f: imm8g: imm8h;
                }
                if (cmode < 0 > == '1' && op == '0') {
                    imm32 = imm8 < 7 >:NOT(imm8 < 6 >):Replicate(imm8 < 6 >, 5):imm8 < 5:0 >:Zeros(19);
                    imm64 = Replicate(imm32, 2);
                }
                if (cmode < 0 > == '1' && op == '1') {
                    if UsingAArch32() then ReservedEncoding();
                    imm64 = imm8 < 7 >:NOT(imm8 < 6 >):Replicate(imm8 < 6 >, 8):imm8 < 5:0 >:Zeros(48);
                }
                break;
                */
            }
            return ImmediateOperand.Word64(imm64);
        }
        private static Mutator<T32Disassembler> vfpImm32(int posH, int lenH, int posL, int lenL)
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

        private static Mutator<T32Disassembler> vfpImm64(int posH, int lenH, int posL, int lenL)
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

        private static MachineOperand MakeBarrierOperand(uint n)
        {
            var bo = (BarrierOption) n;
            switch (bo)
            {
            case BarrierOption.OSHLD:
            case BarrierOption.OSHST:
            case BarrierOption.OSH:
            case BarrierOption.NSHLD:
            case BarrierOption.NSHST:
            case BarrierOption.NSH:
            case BarrierOption.ISHLD:
            case BarrierOption.ISHST:
            case BarrierOption.ISH:
            case BarrierOption.LD:
            case BarrierOption.ST:
            case BarrierOption.SY:
                return new BarrierOperand(bo);

            }
            return null;
        }

        protected override AArch32Instruction CreateInvalidInstruction()
        {
            return new T32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                opcode = Opcode.Invalid,
                ops = new MachineOperand[0]
            };
        }

        private AArch32Instruction NotYetImplemented(string message, uint wInstr)
        {
            string instrHexBytes;
            if (wInstr > 0xFFFF)
            {
                instrHexBytes = $"{wInstr >> 16:X4}{ wInstr & 0xFFFF:X4}";
            }
            else
            {
                instrHexBytes = $"{wInstr:X4}";
            }
            var rev = $"{Bits.Reverse(wInstr):X8}";
            message = (string.IsNullOrEmpty(message))
                ? rev
                : $"{rev} - {message}";
            base.EmitUnitTest("T32", instrHexBytes, message, "ThumbDis", this.addr, Console =>
            {
                if (wInstr > 0xFFFF)
                {
                    Console.WriteLine($"    Given_Instructions(0x{wInstr >> 16:X4}, 0x{wInstr & 0xFFFF:X4});");
                }
                else
                {
                    Console.WriteLine($"    Given_Instructions(0x{wInstr:X4});");
                }
                Console.WriteLine("    Expect_Code(\"@@@\");");
            });
            return CreateInvalidInstruction();
        }


        private ArmVectorData VectorFloatElementData(uint n)
        {
            switch (n)
            {
            case 1: return ArmVectorData.F16;
            case 2: return ArmVectorData.F32;
            default: return ArmVectorData.INVALID;
            }
        }


        private ArmVectorData VectorConvertData(uint wInstr)
        {
            var op = SBitfield(wInstr, 7, 2);
            switch (SBitfield(wInstr, 18, 2))
            {
            case 1:
                switch (op)
                {
                case 0: return ArmVectorData.F16S16;
                case 1: return ArmVectorData.F16U16;
                case 2: return ArmVectorData.S16F16;
                case 3: return ArmVectorData.U16F16;
                }
                break;
            case 2:
                switch (op)
                {
                case 0: return ArmVectorData.F32S32;
                case 1: return ArmVectorData.F32U32;
                case 2: return ArmVectorData.S32F32;
                case 3: return ArmVectorData.U32F32;
                }
                break;
            }
            return ArmVectorData.INVALID;
        }

        private ArmVectorData VectorConvertData2(uint wInstr)
        {
            var op = SBitfield(wInstr, 8, 2);
            var u = SBitfield(wInstr, 28, 1);
            switch (op)
            {
            case 0:
                return u == 0 ? ArmVectorData.F16S16 : ArmVectorData.F16U16;
            case 1:
                return u == 0 ? ArmVectorData.S16F16 : ArmVectorData.U16F16;
            case 2:
                return u == 0 ? ArmVectorData.F32S32 : ArmVectorData.F32U32;
            case 3:
                return u == 0 ? ArmVectorData.S32F32 : ArmVectorData.U32F32;
            }
            return ArmVectorData.INVALID;
        }

        /// <summary>
        /// Concatenate the value in 1 or more bit fields and then optionally
        /// shift it to the left by a given amount.
        /// </summary>
        /// <param name="wInstr"></param>
        /// <param name="format"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private uint ReadBitfields(uint wInstr, string format, ref int i)
        {
            uint n = 0u;
            int bits = 0;
            bool signExtend = PeekAndDiscard('+', format, ref i);
            do
            {
                var offset = ReadDecimal(format, ref i);
                Expect(':', format, ref i);
                var size = ReadDecimal(format, ref i);
                n = (n << size) | ((wInstr >> offset) & ((1u << size) - 1));
                bits += size;
            } while (PeekAndDiscard(':', format, ref i));
            if (PeekAndDiscard('<', format, ref i))
            {
                var shift = ReadDecimal(format, ref i);
                n <<= shift;
                bits += shift;
            }
            if (signExtend)
            {
                n = (uint) Bits.SignExtend(n, bits);
            }
            return n;
        }

        private static ImmediateOperand ModifiedImmediate(uint wInstr)
        {
            var i_imm3_a = (SBitfield(wInstr, 10 + 16, 1) << 4) |
                (SBitfield(wInstr, 12, 3) << 1) |
                (SBitfield(wInstr, 7, 1));
            var abcdefgh = wInstr & 0xFF;
            switch (i_imm3_a)
            {
            case 0:
            case 1:
                return ImmediateOperand.Word32(abcdefgh);
            case 2:
            case 3:
                return ImmediateOperand.Word32((abcdefgh << 16) | abcdefgh);
            case 4:
            case 5:
                return ImmediateOperand.Word32((abcdefgh << 24) | (abcdefgh << 8));
            case 6:
            case 7:
                return ImmediateOperand.Word32(
                    (abcdefgh << 24) |
                    (abcdefgh << 16) |
                    (abcdefgh << 8) |
                    (abcdefgh));
            default:
                abcdefgh |= 0x80;
                return ImmediateOperand.Word32(abcdefgh << (0x20 - i_imm3_a));
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

        private static int SBitfield(uint word, int offset, int size)
        {
            return ((int) word >> offset) & ((1 << size) - 1);
        }

        private static bool PeekAndDiscard(char c, string format, ref int i)
        {
            if (i >= format.Length)
                return false;
            if (format[i] != c)
                return false;
            ++i;
            return true;
        }

        private static void Expect(char c, string format, ref int i)
        {
            Debug.Assert(format[i] == c);
            ++i;
        }

        private static int ReadDecimal(string format, ref int i)
        {
            int n = 0;
            while (i < format.Length)
            {
                char c = format[i];
                if (!char.IsDigit(c))
                    break;
                ++i;
                n = n * 10 + (c - '0');
            }
            return n;
        }

        private PrimitiveType DataType(string format, ref int i)
        {
            switch (format[i++])
            {
            case 'd': return PrimitiveType.Word64;
            case 'w': return PrimitiveType.Word32;
            case 'h': return PrimitiveType.Word16;
            case 'H': return PrimitiveType.Int16;
            case 'b': return PrimitiveType.Byte;
            case 'B': return PrimitiveType.SByte;
            case 'r':
                var n = ReadDecimal(format, ref i);
                return PrimitiveType.Create(Domain.Real, n);
            default: throw new InvalidOperationException($"{format[i - 1]}");
            }
        }

        private static Decoder DecodeBfcBfi(Opcode opcode, params Mutator<T32Disassembler>[] mutators)
        {
            return new BfcBfiDecoder(opcode, mutators);
        }

        #region Mutators

        /// <summary>
        /// If present sets the updateflags bit of the instruction.
        /// </summary>
        private static bool uf(uint u, T32Disassembler dasm)
        {
            dasm.state.updateFlags = true;
            return true;
        }

        /// <summary>
        /// This instructions sets the flags if it's outside an IT block.
        /// </summary>
        private static bool ufit(uint u, T32Disassembler dasm)
        {
            dasm.state.updateFlags = dasm.itCondition == ArmCondition.AL;
            return true;
        }



        /// <summary>
        /// This is the wide form of an ARM Thumb instruction.
        /// </summary>
        private static bool wide(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.wide = true;
            return true;
        }
    
        /// <summary>
        /// Conditional instruction encoded at bit position <paramref name="bitPos" />
        /// </summary>
        private static Mutator<T32Disassembler> ConditionCode(int bitPos)
        {
            var field = new Bitfield(bitPos, 4);
            return (u, d) =>
            {
                d.state.cc = (ArmCondition) field.Read(u);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> c8 = ConditionCode(8);

        private static Mutator<T32Disassembler> q(int bitPos)
        {
            return (u, d) =>
            {
                d.state.useQ = Bits.IsBitSet(u, bitPos);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> q6 = q(6);

        /// <summary>
        /// Writeback bit.
        /// </summary>
        private static Mutator<T32Disassembler> w(int bitPos)
        {
            return (u, d) =>
            {
                d.state.writeback = Bits.IsBitSet(u, bitPos);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> w21 = w(21);

        private static Bitfield[] vifFields = {
            new Bitfield(10,1), new Bitfield(18, 2)
        };

        private static bool vif(uint uInstr, T32Disassembler dasm)
        {
            var code = Bitfield.ReadFields(vifFields, uInstr);
            switch (code)
            {
            case 0b0_00: dasm.state.vectorData = ArmVectorData.S8; return true;
            case 0b0_01: dasm.state.vectorData = ArmVectorData.S16; return true;
            case 0b0_10: dasm.state.vectorData = ArmVectorData.S32; return true;
            case 0b1_01: dasm.state.vectorData = ArmVectorData.F16; return true;
            case 0b1_10: dasm.state.vectorData = ArmVectorData.F32; return true;
            }
            return false;
        }

        /// <summary>
        /// Set vector element size to a signed integer.
        /// </summary>
        private static Mutator<T32Disassembler> vi(int bitpos, int length, params ArmVectorData[] sizes)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                d.state.vectorData = sizes[field.Read(u)];
                return d.state.vectorData != INVALID;
            };
        }
        private static readonly Mutator<T32Disassembler> viBHW_ = vi(20, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> viHWD_ = vi(20, 2, I16, I32, I64, INVALID);
        private static readonly Mutator<T32Disassembler> viBHWD = vi(20, 2, I8, I16, I32, I64);
        private static readonly Mutator<T32Disassembler> vi18BHW_ = vi(18, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> vi18BHWD = vi(18, 2, I8, I16, I32, I64);
        private static readonly Mutator<T32Disassembler> vf8_HSD = vi(8, 2, INVALID, F16, F32, F64);
        private static readonly Mutator<T32Disassembler> vi6BHW_ = vi(6, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> vi6BHWD = vi(6, 2, I8, I16, I32, I64);
        private static readonly Mutator<T32Disassembler> vi10BHW_ = vi(10, 2, I8, I16, I32, INVALID);

        private static readonly Mutator<T32Disassembler> vi_BHW_chk = vi(20, 2, I8, I16, I32, INVALID);  //$REVIEW: not all of these are correct!

        private static Mutator<T32Disassembler> viu(int bitposU, int bitposSize, params ArmVectorData[] sizes)
        {
            var fields = new[]
            {
                new Bitfield(bitposU, 1),
                new Bitfield(bitposSize, 2),
            };

            return (u, d) =>
            {
                var sel = Bitfield.ReadFields(fields, u);
                d.state.vectorData = sizes[sel];
                return d.state.vectorData != INVALID;
            };
        }
        private static readonly Mutator<T32Disassembler> viuBHW_ = viu(28, 20, S8, S16, S32, INVALID, U8, U16, U32, INVALID);
        private static readonly Mutator<T32Disassembler> viuBHWD = viu(28, 20, S8, S16, S32, S64, U8, U16, U32, U64);
        private static readonly Mutator<T32Disassembler> viu_HW__HW_ = viu(28, 20, INVALID, S16, S32, INVALID, INVALID, U16, U32, INVALID);
        private static readonly Mutator<T32Disassembler> vi_HW_HS_ = viu(8, 20, INVALID, I16, I32, INVALID, INVALID, F16, F32, INVALID);
        private static readonly Mutator<T32Disassembler> vifBHW__HS_ = viu(10, 18, I8, I16, I32, INVALID, INVALID, F16, F32, INVALID);
        private static readonly Mutator<T32Disassembler> vsfBHW__HS_ = viu(10, 18, S8, S16, S32, INVALID, INVALID, F16, F32, INVALID);
        private static readonly Mutator<T32Disassembler> vif8_HSD = viu(7, 8, INVALID, U32F16, U32F32, U32F64, INVALID, S32F16, S32F32, S32F64);

        /// <summary>
        /// Vector elements are signed or unsigned integers
        /// </summary>
        private static Mutator<T32Disassembler> vu(int bitpos, ArmVectorData[] signed, ArmVectorData [] unsigned)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                uint nn = field.Read(u);
                if (Bits.IsBitSet(u, 28))
                    d.state.vectorData = unsigned[nn];
                else
                    d.state.vectorData = signed[nn];
                return d.state.vectorData != INVALID;
            };
        }

        private static ArmVectorData[] signed_bhw_ = new[]
        {
            ArmVectorData.S8,
            ArmVectorData.S16,
            ArmVectorData.S32,
            ArmVectorData.INVALID,
        };
        private static ArmVectorData[] unsigned_bhw_ = new[]
        {
            ArmVectorData.U8,
            ArmVectorData.U16,
            ArmVectorData.U32,
            ArmVectorData.INVALID,
        };

        private static Mutator<T32Disassembler> vu_bhw_(int bitpos)
        {
            return vu(bitpos, signed_bhw_, unsigned_bhw_);
        }

        private static ArmVectorData[] signed_bhwd = new[]
{
            ArmVectorData.S8,
            ArmVectorData.S16,
            ArmVectorData.S32,
            ArmVectorData.S64,
        };
        private static ArmVectorData[] unsigned_bhwd = new[]
 {
            ArmVectorData.U8,
            ArmVectorData.U16,
            ArmVectorData.U32,
            ArmVectorData.U64,
        };

        private static Mutator<T32Disassembler> vu_bhwd(int bitpos)
        {
            return vu(bitpos, signed_bhw_, unsigned_bhwd);
        }

        private static Mutator<T32Disassembler> vr(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                uint nn = field.Read(u);
                throw new NotImplementedException();
                //d.state.vectorData = d.VectorIntUIntData(u, nn);
                //return true;
            };
        }

        /// <summary>
        /// Shift amount depends on the bit pattern encouded in the field
        /// </summary>
        private static Mutator<T32Disassembler> calcVectorShiftAmount(int bitpos, int length)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var imm6 = field.Read(u);
                var unsigned = Bits.IsBitSet(u, 28);
                switch (imm6 >> 3)
                {
                case 0: return false;
                case 1:
                    d.state.vectorData = unsigned ? ArmVectorData.U8 : ArmVectorData.I8;
                    d.state.vectorShiftAmt = imm6 - 8;
                    break;
                case 2:
                case 3:
                    d.state.vectorData = unsigned ? ArmVectorData.U16 : ArmVectorData.I16;
                    d.state.vectorShiftAmt = imm6 - 16;
                    break;
                default:
                    d.state.vectorData = unsigned ? ArmVectorData.U32 : ArmVectorData.I32;
                    d.state.vectorShiftAmt = imm6 - 32;
                    break;
                }
                return true;
            };
        }

        private static bool readVectorShiftAmount(uint uInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(ImmediateOperand.Int32((int) dasm.state.vectorShiftAmt));
            return true;
        }

        // conversion 
        private static bool vc(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.vectorData = dasm.VectorConvertData(wInstr);
            return dasm.state.vectorData != INVALID;
        }

        // conversion2 
        private static bool vC(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.vectorData = dasm.VectorConvertData2(wInstr);
            return dasm.state.vectorData != INVALID;
        }

        // floating point elements specified by a bitfield
        private static Mutator<T32Disassembler> vF(int bitPos)
        {
            var bf = new Bitfield(bitPos, 2);
            return (u, d) =>
            {
                uint n = bf.Read(u);
                d.state.vectorData = d.VectorFloatElementData(n);
                return d.state.vectorData != ArmVectorData.INVALID;
            };
        }

        private static ArmVectorData[] _hw_ = new[]
        {
            INVALID,
            I16,
            I32,
            INVALID
        };

        private static Mutator<T32Disassembler> v_hw_(int bitPos)
        {
            var bf = new Bitfield(bitPos, 2);
            return (u, d) =>
            {
                uint n = bf.Read(u);
                d.state.vectorData = _hw_[n];
                return d.state.vectorData != INVALID;
            };
        }



        /// <summary>
        /// Register bitfield
        /// </summary>
        private static Mutator<T32Disassembler> R(int bitOffset)
        {
            var field = new Bitfield(bitOffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> R0 = R(0);
        private static readonly Mutator<T32Disassembler> R3 = R(3);
        private static readonly Mutator<T32Disassembler> R8 = R(8);
        private static readonly Mutator<T32Disassembler> R9 = R(9);
        private static readonly Mutator<T32Disassembler> R12 = R(12);
        private static readonly Mutator<T32Disassembler> R16 = R(16);

        /// <summary>
        /// Register bitfield, but don't allow PC
        /// </summary>
        private static Mutator<T32Disassembler> Rnp(int bitOffset)
        {
            var field = new Bitfield(bitOffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                if (iReg == ArmRegPC)
                    return false;
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }

        private static readonly Mutator<T32Disassembler> Rnp0 = Rnp(0);
        private static readonly Mutator<T32Disassembler> Rnp8 = Rnp(8);
        private static readonly Mutator<T32Disassembler> Rnp12 = Rnp(12);
        private static readonly Mutator<T32Disassembler> Rnp16 = Rnp(16);


        /// <summary>
        /// GP register specified by 3 bits (r0..r7)
        /// </summary>
        private static Mutator<T32Disassembler> r(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> r0 = r(0);
        private static readonly Mutator<T32Disassembler> r3 = r(3);
        private static readonly Mutator<T32Disassembler> r6 = r(6);
        private static readonly Mutator<T32Disassembler> r8 = r(8);

        /// <summary>
        /// GP register, specified by bits 7 || 2..0
        /// </summary>
        private static bool T(uint wInstr, T32Disassembler dasm)
        {
            var tReg = ((wInstr & 0x80) >> 4) | (wInstr & 7);
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegs[tReg]));
            return true;
        }

        private static Mutator<T32Disassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static Mutator<T32Disassembler> sp = Reg(Registers.sp);
        private static Mutator<T32Disassembler> cpsr = Reg(Registers.cpsr);
        private static Mutator<T32Disassembler> spsr = Reg(Registers.spsr);

        /// SIMD / FP system registers
        private static Mutator<T32Disassembler> SIMDSysReg(int bitoffset)
        {
            var field = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = simdSysRegisters[iReg];
                if (reg == null)
                    return false;
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static readonly RegisterStorage[] simdSysRegisters = new[]
        {
            Registers.fpsid,
            Registers.fpscr,
            null,
            null,

            null,
            Registers.mvfr2,
            Registers.mvfr1,
            Registers.mvfr0,

            Registers.fpexc,
            null,
            null,
            null,

            null,
            null,
            null,
            null,
        };


        // Multiple regs

        /// <summary>
        /// rp - Register pair
        /// </summary>
        private static Mutator<T32Disassembler> rp(int offset)
        {
            var field = new Bitfield(offset, 4);
            return (u, d) =>
            {
                var imm = field.Read(u);
                if ((imm & 1) != 0)
                {
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
        private static Mutator<T32Disassembler> Rp_0 = rp(0);

        // 'mw': 16-bit instruction register mask used by push
        private static bool mw(uint wInstr, T32Disassembler dasm)
        {
            uint regmask = wInstr & 0xFF;
            regmask |= (wInstr & 0x100) << 6;
            dasm.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, regmask));
            return true;
        }

        // 'mr': 16-bit instruction register mask used by pop
        private static bool mr(uint wInstr, T32Disassembler dasm)
        {
            uint regmask = wInstr & 0xFF;
            regmask |= (wInstr & 0x100) << 7;
            dasm.state.ops.Add(new MultiRegisterOperand(Registers.GpRegs, PrimitiveType.Word16, regmask));
            return true;
        }

        /// <summary>
        /// Multiple SIMD S-registers (for VSTM* and VLD*)
        /// </summary>
        private static Mutator<T32Disassembler> mrsimdS((int pos, int length) regCount)
        {
            var fldRegCount = new Bitfield(regCount.pos, regCount.length);
            var fldsRegStart = new[] { new Bitfield(22, 1), new Bitfield(12, 4) };
            return (u, d) =>
            {
                var regs = (int) fldRegCount.Read(u);
                var startReg = (int) Bitfield.ReadFields(fldsRegStart, u);
                if (regs + startReg > 32) return false;
                uint regmask = ((1u << regs) - 1) << startReg;
                d.state.ops.Add(new MultiRegisterOperand(Registers.SRegs, PrimitiveType.Word32, regmask));
                return true;
            };
        }

        /// <summary>
        /// Multiple SIMD D-registers (for VSTM* and VLD*)
        /// </summary>
        private static Mutator<T32Disassembler> mrsimdD((int pos, int length) regCount)
        {
            var fldRegCount = new Bitfield(regCount.pos, regCount.length);
            var fldsRegStart = new[] { new Bitfield(22, 1), new Bitfield(12, 4) };
            return (u, d) =>
            {
                var regs = (int)fldRegCount.Read(u);
                var startReg = (int) Bitfield.ReadFields(fldsRegStart, u);
                if (regs + startReg > 32) return false;
                uint regmask = ((1u << regs) - 1) << startReg;
                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, regmask));
                return true;
            };
        }

        private static Mutator<T32Disassembler> mrsimdD_1((int bit, int bit4) regStart, (int pos, int length) regCount)
        {
            var fldRegCount = new Bitfield(regCount.pos, regCount.length);
            var fldsRegStart = new[] { new Bitfield(regStart.bit, 1), new Bitfield(regStart.bit4, 4) };
            return (u, d) =>
            {
                var regs = (int) fldRegCount.Read(u) + 1;
                var startReg = (int) Bitfield.ReadFields(fldsRegStart, u);
                if (regs + startReg > 32) return false;
                uint regmask = ((1u << regs) - 1) << startReg;
                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, regmask));
                return true;
            };
        }

        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>

        private static Mutator<T32Disassembler> S_pair(int pos1, int pos2)
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


        private static Mutator<T32Disassembler> D(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.DRegs[iReg]));
                return true;
            };
        }



        private static Mutator<T32Disassembler> Dlist(int nRegs, int incr)
        {
            var fields = new[] {
                new Bitfield(22, 1),
                new Bitfield(12, 4)
            };
            return (u, d) =>
            {
                var iStartReg = (int)Bitfield.ReadFields(fields, u);
                if (iStartReg + (nRegs * incr) > 32) return false;
                uint regMask = 0;
                for (int i = 0; i < nRegs; ++i)
                {
                    regMask = (regMask << incr) | 1u;
                }
                regMask = regMask << iStartReg;

                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, regMask));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> Dlist1 = Dlist(1, 1);
        private static readonly Mutator<T32Disassembler> Dlist2 = Dlist(2, 1);
        private static readonly Mutator<T32Disassembler> Dlist3 = Dlist(3, 1);
        private static readonly Mutator<T32Disassembler> Dlist4 = Dlist(4, 1);
        private static readonly Mutator<T32Disassembler> Dlist2_2 = Dlist(2, 2);
        private static readonly Mutator<T32Disassembler> Dlist3_2 = Dlist(3, 2);
        private static readonly Mutator<T32Disassembler> Dlist4_2 = Dlist(4, 2);


        private static Mutator<T32Disassembler> DlistIdx(int nRegs, int bitposStep, int lenStep)
        {
            var fields = new[] {
                new Bitfield(22, 1),
                new Bitfield(12, 4)
            };
            var incrFld = new Bitfield(bitposStep, lenStep);
            var sizeFld = new Bitfield(10, 2);
            var indexFields = new[]
            {
                new Bitfield(5, 3),
                new Bitfield(6, 2),
                new Bitfield(7, 1),
            };
            
            return (u, d) =>
            {
                var iStartReg = (int) Bitfield.ReadFields(fields, u);
                var incr = (int) incrFld.Read(u) + 1;
                if (iStartReg + (nRegs * incr) > 32) return false;
                uint regMask = 0;
                for (int i = 0; i < nRegs; ++i)
                {
                    regMask = (regMask << incr) | 1u;
                }
                regMask = regMask << iStartReg;
                var size = sizeFld.Read(u);
                int index = (int) indexFields[size].Read(u);
                d.state.ops.Add(new MultiRegisterOperand(Registers.DRegs, PrimitiveType.Word64, regMask, index));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> DlistIdx1_7_1 = DlistIdx(1, 7, 1);
        private static readonly Mutator<T32Disassembler> DlistIdx2_5_1 = DlistIdx(2, 5, 1);
        private static readonly Mutator<T32Disassembler> DlistIdx3_4_1 = DlistIdx(3, 4, 1);
        private static readonly Mutator<T32Disassembler> DlistIdx3_5_1 = DlistIdx(3, 5, 1);
        private static readonly Mutator<T32Disassembler> DlistIdx3_7_1 = DlistIdx(3, 7, 1);
        private static readonly Mutator<T32Disassembler> DlistIdx4_2 = DlistIdx(4, 6, 2);



        private static bool F12_22(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 11) & 0x1E) | ((wInstr >> 22) & 1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[d]));
            return true;
        }

        private static bool D22_12(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[d]));
            return true;
        }

        private static bool Q22_12(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        /// SIMD / FP register, whose size is determined by the vectordata size.
        private static Mutator<T32Disassembler> FP(int posBit, int pos4bit)
        {
            var sFields = new[] { new Bitfield(pos4bit, 4), new Bitfield(posBit, 1) };
            var dFields = new[] { new Bitfield(posBit, 1), new Bitfield(pos4bit, 4) };
            return (u, d) =>
            {
                Bitfield[] fields;
                RegisterStorage[] regs;
                switch (d.state.vectorData)
                {
                case F16: case F32:
                case U32F16: case U32F32:
                case S32F16: case S32F32:
                    fields = sFields; regs = Registers.SRegs; break;
                case F64:
                case U32F64:
                case S32F64:
                    fields = dFields; regs = Registers.DRegs; break;
                default: return false;
                }
                var iReg = Bitfield.ReadFields(fields, u);
                var reg = regs[iReg];
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> FP0 = FP(5, 0);
        private static readonly Mutator<T32Disassembler> FP12 = FP(22, 12);
        private static readonly Mutator<T32Disassembler> FP16 = FP(7, 16);


        /// <summary>
        /// Vector register, whose size is set by q(<bitpos>)
        /// </summary>
        private static Mutator<T32Disassembler> W(int pos1, int size1, int pos2, int size2)
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
        private readonly static Mutator<T32Disassembler> W5_0 = W(5, 1, 0, 4);
        private readonly static Mutator<T32Disassembler> W7_16 = W(7, 1, 16, 4);
        private readonly static Mutator<T32Disassembler> W22_12 = W(22, 1, 12, 4);

        private static bool Q22_12_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }


        private static bool F16_7(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr >> 15) & 0x1E) | ((wInstr >> 7) & 0x1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[s]));
            return true;
        }

        private static bool D7_16(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[d]));
            return true;
        }

        private static bool Q7_16(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }


        private static bool F0_5(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr & 0xF) << 1) | ((wInstr >> 0x5) & 1);
            dasm.state.ops.Add(new RegisterOperand(Registers.SRegs[s]));
            return true;
        }

        private static bool D5_0(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.DRegs[
                ((wInstr >> 1) & 0x10) | (wInstr & 0xF)]));
            return true;
        }

        private static bool Q5_0(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        /// <summary>
        /// Floating-point register specifier.
        /// </summary>
        /// <remarks>
        /// FP registers need 5-bit numbers to identify them. The 5 bits
        /// are broken up into a single bit and a four bit field. Annoyingly
        /// the encoding for single-precision instructions is nnnn:m while
        /// double-precision instructions is m:nnnn.
        /// </remarks>
        private static Mutator<T32Disassembler> Fp(int bitpos, int fourBitPos)
        {
            var singleFields = new[] {
                new Bitfield(fourBitPos, 4),
                new Bitfield(bitpos, 1)
            };
            var doubleFields = new[]
            {
                singleFields[1],
                singleFields[0]
            };
            return (u, d) =>
            {
                RegisterStorage reg;
                if (d.state.vectorData == ArmVectorData.F64)
                {
                    var iReg = Bitfield.ReadFields(doubleFields, u);
                    reg = Registers.DRegs[iReg];
                }
                else
                {
                    var iReg = Bitfield.ReadFields(singleFields, u);
                    reg = Registers.DRegs[iReg];
                }
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        private static bool Q5_0_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(new RegisterOperand(Registers.QRegs[q >> 1]));
            return true;
        }

        // Coprocessor registers

        private static Mutator<T32Disassembler> CP(int n)
        {
            return (u, d) =>
            {
                //if (PeekAndDiscard('#', format, ref i))   // Literal
                //{
                //    offset = ReadDecimal(format, ref i);
                //    var cp = Registers.Coprocessors[offset];
                //    op = new RegisterOperand(cp);
                //}
                //else
                var op = d.Coprocessor(u, n);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CP8 = CP(8);

        private static Mutator<T32Disassembler> CPn(int n)
        {
            return (u, d) =>
            {
                var cp = Registers.Coprocessors[n];
                var op = new RegisterOperand(cp);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CPn14 = CPn(14);


        private static Mutator<T32Disassembler> CR(int n)
        {
            return (u, d) =>
            {
                var op = d.CoprocessorRegister(u, n);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> CR0 = CR(0);
        private static readonly Mutator<T32Disassembler> CR12 = CR(12);
        private static readonly Mutator<T32Disassembler> CR16 = CR(16);


        // Immediate mutators

        private static Mutator<T32Disassembler> Imm(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var imm = bitfield.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Simm(int pos, int length, int shift = 0)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u) << shift;
                d.state.ops.Add(ImmediateOperand.Int32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Imm(int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Imm(PrimitiveType dt = null, uint minuend = 0, params Bitfield[] fields)
        {
            var dataType = dt ?? PrimitiveType.Word32;
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (minuend != 0)
                {
                    imm = minuend - imm;
                }
                d.state.ops.Add(new ImmediateOperand(Constant.Create(dataType, imm)));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> Imm26_12_0 = Imm(fields: Bf((26, 1), (12, 3), (0, 8)));

        private static Mutator<T32Disassembler> Imm(Constant c)
        {
            return (u, d) =>
            {
                d.state.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> Imm0_r32 = Imm(Constant.Real32(0));
        private static readonly Mutator<T32Disassembler> Imm0_r64 = Imm(Constant.Real64(0));


        private static Mutator<T32Disassembler> ImmM1(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var n = bitfield.Read(u);
                d.state.ops.Add(ImmediateOperand.Word32(n + 1));
                return true;
            };
        }

        /// <summary>
        /// Generate an immediate 0 based on vector data
        /// </summary>
        private static bool ImmV0(uint wInstr, T32Disassembler dasm)
        {
            if (dasm.state.vectorData == INVALID)
                return false;
            var dt = Arm32Architecture.VectorElementDataType(dasm.state.vectorData);
            var zero = Constant.Zero(dt);
            dasm.state.ops.Add(new ImmediateOperand(zero));
            return true;
        }

        /// <summary>
        /// Signed integer
        /// </summary>
        private static Mutator<T32Disassembler> S(int pos, int len)
        {
            var bf = new Bitfield(pos, len);
            return (u, d) =>
            {
                d.state.ops.Add(ImmediateOperand.Int32((int)bf.Read(u)));
                return true;
            };
        }

        // 'Si' = shift immediate
        private static Mutator<T32Disassembler> Si((int pos, int len) bfType, Bitfield[] bfCount)
        {
            var fType = new Bitfield(bfType.pos, bfType.len);
            return (u, d) =>
            {
                (d.state.shiftType, d.state.shiftValue) = d.DecodeImmShift(u, fType, bfCount);
                return true;
            };
        }

        // Sr = rotate
        private static Mutator<T32Disassembler> SrBy8(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                int n = (int) field.Read(u);
                d.state.shiftType = n != 0 ? Opcode.ror : Opcode.Invalid;
                d.state.shiftValue = ImmediateOperand.Int32(n * 8);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> SrBy8_4_2 = SrBy8(4, 2);

        private static Mutator<T32Disassembler> ShiftImm(Opcode opc, int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(bitfields, u);
                if (imm != 0)
                {
                    d.state.shiftType = opc;
                    d.state.shiftValue = ImmediateOperand.Int32((int) imm);
                }
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> LslImm = ShiftImm(Opcode.lsl, 12, 3, 6, 2);
        private static readonly Mutator<T32Disassembler> AsrImm = ShiftImm(Opcode.asr, 12, 3, 6, 2);

        private static readonly Bitfield[] modifiedImmediateFields = new[]
        {
            new Bitfield(10 + 16, 1),
            new Bitfield(12, 3),
            new Bitfield(7, 1)
        };
        private static bool M(uint wInstr, T32Disassembler dasm)
        {
            var i_imm3_a = Bitfield.ReadFields(modifiedImmediateFields, wInstr);
            var abcdefgh = wInstr & 0xFF;
            MachineOperand op;
            switch (i_imm3_a)
            {
            case 0:
            case 1:
                op = ImmediateOperand.Word32(abcdefgh);
                break;
            case 2:
            case 3:
                op = ImmediateOperand.Word32((abcdefgh << 16) | abcdefgh);
                break;
            case 4:
            case 5:
                op = ImmediateOperand.Word32((abcdefgh << 24) | (abcdefgh << 8));
                break;
            case 6:
            case 7:
                op = ImmediateOperand.Word32(
                    (abcdefgh << 24) |
                    (abcdefgh << 16) |
                    (abcdefgh << 8) |
                    (abcdefgh));
                break;
            default:
                abcdefgh |= 0x80;
                op = ImmediateOperand.Word32(abcdefgh << (int) (0x20 - i_imm3_a));
                break;
            }
            dasm.state.ops.Add(op);
            return true;
        }

        private static Mutator<T32Disassembler> MS(params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(d.ModifiedSimdImmediate(u, n));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> MS_28_16_0 = MS(Bf((28, 1), (16, 3), (0, 4)));


        /// <summary>
        /// Modified SIMD immediate
        /// </summary>
        private static Mutator<T32Disassembler> Is(int pos1, int size1, int pos2, int size2, int pos3, int size3)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2),
                new Bitfield(pos3, size3),
            };
            var op0size = new[,]
            {
                {
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I8, ArmVectorData.F32,
                },
            {
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I32,
                 ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16, ArmVectorData.I16,
                 ArmVectorData.I32, ArmVectorData.I32, ArmVectorData.I64, ArmVectorData.INVALID,
            } };
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                var cmode = (u >> 8) & 0xF;
                var op = (u >> 5) & 1;
                d.state.vectorData = op0size[op, cmode];
                d.state.ops.Add(ImmediateOperand.Word64(A32Disassembler.SimdExpandImm(op, cmode, (uint) imm)));
                return d.state.vectorData != INVALID;
            };
        }

        // Endianness
        private static Mutator<T32Disassembler> E(int pos, int size)
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
        /// PC-relative offset, aligned by 4 bytes
        /// </summary>
        private static Mutator<T32Disassembler> P(int bitOffset, int length)
        {
            var field = new Bitfield(bitOffset, length);
            return (u, d) =>
            {
                var offset = field.ReadSigned(u) << 2;
                var op = AddressOperand.Create(d.addr.Align(4) + offset);
                d.state.ops.Add(op);
                return true;
            };
        }

        private static Mutator<T32Disassembler> PcRelative(int shift = 0, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << shift;
                var op = AddressOperand.Create(d.addr + (offset + 4));
                d.state.ops.Add(op);
                return true;
            };
        }

        // Jump displacement in bits 9:3..7, shifted left by 1.
        private static bool x(uint wInstr, T32Disassembler dasm)
        {
            var offset = (SBitfield(wInstr, 9, 1) << 6) |
                         (SBitfield(wInstr, 3, 5) << 1);
            dasm.state.ops.Add(AddressOperand.Create(dasm.addr + (offset + 4)));
            return true;
        }

        /// <summary>
        /// Vector immediate quantity.
        /// </summary>
        private static bool IW0(uint uInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(dasm.state.useQ
                ? ImmediateOperand.Word128(0)
                : ImmediateOperand.Word64(0));
            return true;
        }

        private static (ArmVectorData, uint)[] vectorImmediateShiftSize = new[]
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

        private static readonly (ArmVectorData, uint)[] vectorRevImmediateShiftSize =
        {
            (ArmVectorData.INVALID, 0u),
            (ArmVectorData.I8,  16u),
            
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

        private static readonly ArmVectorData [] vectorImmediateShiftSizeSU =
        {
           ArmVectorData.INVALID,
           ArmVectorData.S8,

           ArmVectorData.S16,
           ArmVectorData.S16,

           ArmVectorData.S32,
           ArmVectorData.S32,
           ArmVectorData.S32,
           ArmVectorData.S32,

           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,
           ArmVectorData.S64,

           ArmVectorData.INVALID,
           ArmVectorData.U8,

           ArmVectorData.U16,
           ArmVectorData.U16,

           ArmVectorData.U32,
           ArmVectorData.U32,
           ArmVectorData.U32,
           ArmVectorData.U32,

           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
           ArmVectorData.U64,
        };

        private static readonly ArmVectorData[] vectorImmediateShiftSizeSU_half =
        {
            ArmVectorData.INVALID,
            ArmVectorData.S16,

            ArmVectorData.S32,
            ArmVectorData.S32,

            ArmVectorData.S64,
            ArmVectorData.S64,
            ArmVectorData.S64,
            ArmVectorData.S64,

            ArmVectorData.INVALID,
            ArmVectorData.U16,

            ArmVectorData.U32,
            ArmVectorData.U32,

            ArmVectorData.U64,
            ArmVectorData.U64,
            ArmVectorData.U64,
            ArmVectorData.U64,
        };

        private static bool VshImmSize(uint wInstr, T32Disassembler dasm)
        {
            var immL_6 = ((wInstr >> 1) & 0x40) | (wInstr >> 16) & 0b111111;
            dasm.state.vectorData = vectorImmediateShiftSize[immL_6 >> 3].Item1;
            return dasm.state.vectorData != INVALID;
        }

        private static Mutator<T32Disassembler> VshImmSizeSU(Bitfield[] immL_6Fields, ArmVectorData[] sizes)
        {
            return (u, d) =>
            {
                var immL_6 = Bitfield.ReadFields(immL_6Fields, u);
                var i = (immL_6 >> 3);
                d.state.vectorData = sizes[i];
                return d.state.vectorData != INVALID;
            };
        }
        private static readonly Mutator<T32Disassembler> VshImmSizeSU16 = VshImmSizeSU(Bf((24,1),(16, 6)), vectorImmediateShiftSizeSU);
        private static readonly Mutator<T32Disassembler> VshImmSizeSU16_half = VshImmSizeSU(Bf((24,1),(16, 6)), vectorImmediateShiftSizeSU_half);
        private static readonly Mutator<T32Disassembler> VshImmSizeSU7_16 = VshImmSizeSU(Bf((24,1),(7,1), (16, 6)), vectorImmediateShiftSizeSU);

        private static bool VshImm(uint wInstr, T32Disassembler dasm)
        {
            var imm6 = (wInstr >> 16) & 0b111111;
            var immL_6 = ((wInstr >> 1) & 0x40) | imm6; 
            var imm = imm6 - vectorImmediateShiftSize[immL_6 >> 3].Item2;
            dasm.state.ops.Add(ImmediateOperand.Int32((int) imm));
            return true;
        }

        private static bool VshImmRev(uint wInstr, T32Disassembler dasm)
        {
            var imm6 = (wInstr >> 16) & 0b111111;
            var immL_6 = ((wInstr >> 1) & 0x40) | imm6;
            var imm = vectorRevImmediateShiftSize[immL_6 >> 3].Item2 - imm6;
            dasm.state.ops.Add(ImmediateOperand.Int32((int) imm));
            return true;
        }


        /// <summary>
        /// Set the SIMD vector index of the most recently added operand.
        /// </summary>
        private static Mutator<T32Disassembler> Ix(params (int pos, int size)[] fieldSpecs)
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
        private static Mutator<T32Disassembler> Ix(int pos, int size) { return Ix((pos, size)); }


        // Memory access mutators

        private static Mutator<T32Disassembler> MemOff(
            PrimitiveType dt,
            int baseRegBitoffset = 0,
            RegisterStorage baseReg = null,
            int offsetShift = 0,
            IndexSpec indexSpec = null,
            params (int bitOffset, int length)[] offsetFields)
        {
            var brf = new Bitfield(baseRegBitoffset, 4);
            var bfs = offsetFields.Select(f => new Bitfield(f.bitOffset, f.length)).ToArray();
            return (u, d) =>
            {
                var b = baseReg ?? Registers.GpRegs[brf.Read(u)];
                var offset = bfs.Length > 0
                    ? (int) Bitfield.ReadFields(bfs, u)
                    : 0;
                bool preIndex = false;
                bool add = true;
                if (indexSpec != null)
                {
                    preIndex = indexSpec.preIndex.Read(u) != 0;
                    add = indexSpec.add.Read(u) != 0;
                    d.state.writeback = indexSpec.writeback.Read(u) != 0;
                }

                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = b,
                    Offset = Constant.Int32(offset << offsetShift),
                    Add = add,
                    PreIndex = preIndex,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Indexed addressing using 3-bit fields for registers
        /// </summary>
        private static Mutator<T32Disassembler> MemOff_r(
            PrimitiveType dt,
            int baseRegBitoffset = 0,
            RegisterStorage baseReg = null,
            int shift = 0,
            params (int bitOffset, int length)[] fields)
        {
            var brf = new Bitfield(baseRegBitoffset, 3);
            var bfs = fields.Select(f => new Bitfield(f.bitOffset, f.length)).ToArray();
            return (u, d) =>
            {
                var b = baseReg ?? Registers.GpRegs[brf.Read(u)];
                var offset = Bitfield.ReadFields(bfs, u);
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = b,
                    Offset = Constant.Int32((int)offset << shift),
                    Add = true,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<T32Disassembler> MemIdx(PrimitiveType dt, int posBaseReg, int posIdxReg, (int,int)? sh = null)
        {
            Bitfield? field = sh.HasValue
                ? new Bitfield(sh.Value.Item1, sh.Value.Item2)
                : (Bitfield?)null;

            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[(u >> posBaseReg) & 0xF];
                var idxReg = Registers.GpRegs[(u >> posIdxReg) & 0xF];

                int shiftAmt = 0;
                Opcode shiftType = Opcode.Invalid;
                if (field.HasValue)
                {
                    shiftAmt = (int)field.Value.Read(u);
                    shiftType = shiftAmt != 0 ? Opcode.lsl : Opcode.Invalid;
                }
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = baseReg,
                    Index = idxReg,
                    ShiftType = shiftType,
                    Shift = shiftAmt,
                    Add = true,
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Indexed addressing using 3-bit fields for registers
        /// </summary>
        private static Mutator<T32Disassembler> MemIdx_r(PrimitiveType dt, int posBaseReg, int posIdxReg)
        {
            return (u, d) =>
            {
                var baseReg = Registers.GpRegs[(u >> posBaseReg) & 0x7];
                var idxReg = Registers.GpRegs[(u >> posIdxReg) & 0x7];
                var mem = new MemoryOperand(dt)
                {
                    BaseRegister = baseReg,
                    Index = idxReg,
                    Add = true
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private class IndexSpec
        {
            public Bitfield preIndex;
            public Bitfield add;
            public Bitfield writeback;
        }

        // Indexing bits in P=10, W=8
        // Negative bit in U=9
        private static readonly IndexSpec idx10 = new IndexSpec
        {
            preIndex = new Bitfield(10, 1),
            add = new Bitfield(9, 1),
            writeback = new Bitfield(8, 1)
        };

        private static readonly IndexSpec idx24 = new IndexSpec
        {
            preIndex = new Bitfield(24, 1),
            add = new Bitfield(23, 1),
            writeback = new Bitfield(21, 1)
        };

        /// <summary>
        /// Operand type used by single element load/store instructions
        /// </summary>
        private static bool MsingleElem(uint wInstr, T32Disassembler dasm)
        {
            var rm = wInstr & 0b1111;
            var rn = (wInstr >> 16) & 0b1111;
            var baseReg = Registers.GpRegs[rn];
            MemoryOperand mop = new MemoryOperand(Arm32Architecture.VectorElementDataType(dasm.state.vectorData));
            mop.BaseRegister = baseReg;
            if (rm == 0b1101)
            {
                dasm.state.writeback = true;
            }
            else if (rm != 0b1111)
            {
                mop.Index = Registers.GpRegs[rm];
                dasm.state.writeback = true;
                mop.Add = true;
            }
            //$TODO: alignment
            dasm.state.ops.Add(mop);
            return true;
        }

        private static bool Melem16Align(uint wInstr, T32Disassembler dasm)
        {
            var rm = wInstr & 0b1111;
            var rn = (wInstr >> 16) & 0b1111;
            var baseReg = Registers.GpRegs[rn];
            MemoryOperand mop = new MemoryOperand(Arm32Architecture.VectorElementDataType(dasm.state.vectorData));
            mop.BaseRegister = baseReg;
            if (rm == 0b1101)
            {
                dasm.state.writeback = true;
            } 
            else if (rm != 0b1111)
            {
                mop.Index = Registers.GpRegs[rm];
                dasm.state.writeback = true;
                mop.Add = true;
            }
            var align = (int)(wInstr >> 4) & 0b11;
            if (align > 0)
                mop.Alignment = 4 << (align + 3);
            dasm.state.ops.Add(mop);
            return true;
        }


        // Branch targets

        private static Bitfield[] B_T4_fields = new Bitfield[]
        {
            new Bitfield(26, 1),
            new Bitfield(13, 1),
            new Bitfield(11, 1),
            new Bitfield(16, 10),
            new Bitfield(0, 11)
        };
        private static bool B_T4(uint wInstr, T32Disassembler dasm)
        {
            // The T4 encoding of the 'b' instruction is incredibly
            // hairy....
            var mask = 5u << 11;
            var ssss = Bits.SignExtend(wInstr >> 26, 1) & mask;
            wInstr = (wInstr & ~mask) | (~(wInstr ^ ssss) & mask);
            int offset = Bitfield.ReadSignedFields(B_T4_fields, wInstr) << 1;
            var op = AddressOperand.Create(dasm.addr + (offset + 4));
            dasm.state.ops.Add(op);
            return true;
        }

        // Miscellaneous

        private static Mutator<T32Disassembler> B(int pos)
        {
            var field = new Bitfield(pos, 4);
            return (u, d) =>
            {
                uint n = field.Read(u);
                d.state.ops.Add(MakeBarrierOperand(n));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> B0_4 = B(0);

        private static Mutator<T32Disassembler> nyi(string message)
        {
            return (u, d) =>
            {
                d.NotYetImplemented($"Unimplemented '{message}' when decoding {u:X4}", u);
                return false;
            };
        }
        #endregion


        // Factory methods

        private static InstrDecoder Instr(Opcode opcode, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, ArmVectorData.INVALID, mutators);
        }


        private static InstrDecoder Instr(Opcode opcode, ArmVectorData vec, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, vec, mutators);
        }


        /// <summary>
        /// Select decoding depending on whether the 4 bit field value is 0xF or not.
        /// </summary>
        private static ConditionalDecoder<T32Disassembler, Opcode, AArch32Instruction> Select_ne15(
            int bitPos, 
            string tag, 
            Decoder<T32Disassembler, Opcode, AArch32Instruction> decoderNot15,
            Decoder<T32Disassembler, Opcode, AArch32Instruction> decoder15)
        {
            var fields = new[]
            {
                new Bitfield(bitPos, 4)
            };
            return new ConditionalDecoder<T32Disassembler, Opcode, AArch32Instruction>(fields, n => n != 15, tag, decoderNot15, decoder15);
        }

        private static ConditionalDecoder<T32Disassembler, Opcode, AArch32Instruction> Select_ne15(
           int bitPos,
           Decoder<T32Disassembler, Opcode, AArch32Instruction> decoderNot15,
           Decoder<T32Disassembler, Opcode, AArch32Instruction> decoder15)
        {
            return Select_ne15(bitPos, "", decoderNot15, decoder15);
        }

        private static NyiDecoder<T32Disassembler, Opcode, AArch32Instruction> Nyi(string msg)
        {
            return new NyiDecoder<T32Disassembler, Opcode, AArch32Instruction>(msg);
        }


        static T32Disassembler()
        {
            invalid = Instr(Opcode.Invalid);

            // Build the decoder decision tree.
            var dec16bit = Create16bitDecoders();
            var dec32bit = CreateLongDecoder();
            decoders = new Decoder[8] {
                dec16bit,
                dec16bit,
                dec16bit,
                dec16bit,

                dec16bit,
                dec16bit,
                dec16bit,
                Mask(11, 2,
                    Instr(Opcode.b, PcRelative(1, Bf((0, 11)))),
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder<T32Disassembler, Opcode, AArch32Instruction> Create16bitDecoders()
        {
            var AddSpRegisterT1 = Instr(Opcode.add, uf,T,sp);
            var AddSpRegisterT2 = Instr(Opcode.add, sp,T);
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = CreateDataLowRegisters();
            var decDataHiRegisters = Mask(8, 2, "Add, subtract, compare, move (two high registers)",
                Select(Bf((7,1),(0,3)), n => n != 13, 
                    Select((3,4), n => n != 13,
                        Instr(Opcode.add, uf,T,R3),
                        AddSpRegisterT1),
                    Select((3,4), n => n != 13,
                        AddSpRegisterT2, 
                        AddSpRegisterT1)),
                Instr(Opcode.cmp, uf,T,R3),
                Instr(Opcode.mov, T,R3), // mov,movs
                invalid);

            var LdrLiteral = Instr(Opcode.ldr,r8,MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetShift:2, offsetFields:(0,8)));

            var LdStRegOffset = Mask(9, 3, "LdStRegOffset",
                Instr(Opcode.str, r0,MemIdx_r(PrimitiveType.Word32,3,6)),
                Instr(Opcode.strh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Opcode.strb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Opcode.ldrsb, r0, MemIdx_r(PrimitiveType.SByte, 3, 6)),

                Instr(Opcode.ldr, r0, MemIdx_r(PrimitiveType.Word32, 3, 6)),
                Instr(Opcode.ldrh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Opcode.ldrb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Opcode.ldrsh, r0, MemIdx_r(PrimitiveType.Int16, 3, 6)));

            var decLdStWB = Nyi("LdStWB");
            var decLdStHalfword = Nyi("LdStHalfWord");
            var decLdStSpRelative = Nyi("LdStSpRelative");
            var decAddPcSp = Mask(11, 1,
                Instr(Opcode.adr, r8,P(0,8)),
                Instr(Opcode.add, r8,sp,Simm(0, 8, 2)));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new LdmStmDecoder16();
            var decCondBranch = Mask(8, 4, "CondBranch",
                Instr(Opcode.b, c8,PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.b, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Opcode.udf, Imm(0,8)),
                Instr(Opcode.svc, InstrClass.Transfer | InstrClass.Call, Imm(0, 8)));

            return Mask(13, 3,
                decAlu,
                decAlu,
                Mask(10, 3,
                    decDataLowRegisters,
                    Mask(8, 2, // Special data and branch exchange 
                        decDataHiRegisters,
                        decDataHiRegisters,
                        decDataHiRegisters,
                        Mask(7,1,
                            Instr(Opcode.bx, R3),
                            Instr(Opcode.blx, R3))),
                    LdrLiteral,
                    LdrLiteral,

                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset),
                Mask(11, 2,   // decLdStWB,
                    Instr(Opcode.str, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Opcode.ldr, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Opcode.strb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5))),
                    Instr(Opcode.ldrb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5)))),

                Mask(12, 0x01,
                    Mask(11, 0x01,
                        Instr(Opcode.strh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5))),
                        Instr(Opcode.ldrh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5)))),
                    Mask(11, 0x01,   // load store SP-relative
                        Instr(Opcode.str, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))),
                        Instr(Opcode.ldr, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))))),
                Mask(12, 0x01,
                    decAddPcSp,
                    decMisc16Bit),
                Mask(12, 0x01,
                    decLdmStm,
                    decCondBranch),
                Instr(Opcode.Invalid));
        }

        private static Decoder CreateAluDecoder()
        {
            var decAddSub3 = Nyi("addsub3");
            var decAddSub3Imm = Nyi("AddSub3Imm");
            var decMovMovs = Mask(11, 2,
                Select((6,5), n => n != 0,
                    new MovMovsDecoder(Opcode.lsl, uf,r0,r3,S(6,5)),
                    Instr(Opcode.mov, r0,r3)),
                new MovMovsDecoder(Opcode.lsr, uf,r0,r3,S(6,5)),
                Instr(Opcode.asrs, uf,r0,r3,S(6,5)),
                invalid);
            var decAddSub = Mask(11, 2,
                Instr(Opcode.mov, r8,Imm(0,8)),
                Instr(Opcode.cmp, uf,r8,Imm(0,8)),
                Instr(Opcode.add, uf,r8,Imm(0,8)),
                Instr(Opcode.sub, uf,r8,Imm(0,8)));
            return Mask(10, 4,
                decMovMovs,
                decMovMovs,
                decMovMovs,
                decMovMovs,

                decMovMovs,
                decMovMovs,
                Mask(9, 1,
                    Instr(Opcode.add, r0,r3,r6),
                    Instr(Opcode.sub, r0,r3,r6)),
                Mask(9, 1,
                    Instr(Opcode.add, r0,r3,Imm(6,3)),
                    Instr(Opcode.sub, r0,r3,Imm(6,3))),
                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub,

                decAddSub,
                decAddSub,
                decAddSub,
                decAddSub);
        }

        private static Decoder CreateDataLowRegisters()
        {
            return Mask(6, 4,
                Instr(Opcode.and, ufit, r0, r3),
                Instr(Opcode.eor, ufit, r0, r3),
                Instr(Opcode.lsl, ufit, r0, r3),
                Instr(Opcode.lsr, ufit, r0, r3),

                Instr(Opcode.asr, ufit, r0, r3),
                Instr(Opcode.adc, ufit, r0, r3),
                Instr(Opcode.sbc, ufit, r0, r3),
                Instr(Opcode.ror, ufit, r0, r3),

                Instr(Opcode.adc, ufit, r0, r3),
                Instr(Opcode.rsb, ufit, r0, r3),
                Instr(Opcode.cmp, uf, r0, r3),
                Instr(Opcode.cmn, uf, r0, r3),

                Instr(Opcode.orr, ufit, r0, r3),
                Instr(Opcode.mul, ufit, r0, r3),
                Instr(Opcode.bic, ufit, r0, r3),
                Instr(Opcode.mvn, ufit, r0, r3));
        }

        private static Decoder CreateMisc16bitDecoder()
        {
            var pushAndPop = Mask(11, 1,
                Instr(Opcode.push, mw),
                Instr(Opcode.pop, mr));

            var cbnzCbz = Mask(11, 1,
                Instr(Opcode.cbz, r0,x),
                Instr(Opcode.cbnz, r0,x));

            return Mask(8, 4,
                Mask(7, 1,  // Adjust SP
                    Instr(Opcode.add, sp,Simm(0,7, 2)),
                    Instr(Opcode.sub, sp,Simm(0,7, 2))),
                cbnzCbz,
                Mask(6, 2,
                    Instr(Opcode.sxth, r0,r3),
                    Instr(Opcode.sxtb, r0,r3),
                    Instr(Opcode.uxth, r0,r3),
                    Instr(Opcode.uxtb, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Mask(5, 3,
                    Instr(Opcode.setpan, Imm(3,1)),
                    invalid,
                    Instr(Opcode.setend, E(3, 1)),
                    Instr(Opcode.cps, Imm(3, 1)),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                cbnzCbz,
                Mask(6, 2,
                    Instr(Opcode.rev, r0,r3),
                    Instr(Opcode.rev, r0,r3),
                    Instr(Opcode.hlt, InstrClass.Terminates),
                    Instr(Opcode.rev, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Instr(Opcode.bkpt),
                Select((0, 4), n => n == 0,
                    Mask(4, 4, // Hints
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.yield),
                        Instr(Opcode.wfe),
                        Instr(Opcode.wfi),

                        Instr(Opcode.sev),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)),
                    new ItDecoder()));
        }

        private static LongDecoder CreateLongDecoder()
        {
            var branchesMiscControl = CreateBranchesMiscControl();
            var loadStoreMultipleTableBranch = CreateLoadStoreDualMultipleBranchDecoder();

            var LdStMultiple = Mask(7 + 16, 2,
                Mask(4 + 16, 1,
                    Instr(Opcode.srsdb, w21,sp,Imm(0,5)),
                    Instr(Opcode.rfedb, w21,R16)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stm),
                    new LdmStmDecoder32(Opcode.ldm)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Opcode.stmdb),
                    new LdmStmDecoder32(Opcode.ldmdb)),
                Mask(4 + 16, 1,
                    Instr(Opcode.srsia, w21,sp,Imm(0,5)),
                    Instr(Opcode.rfeia, w21,R16)));

            var DataProcessingModifiedImmediate = Mask(4 + 16, 5,
                Instr(Opcode.and, R8,R16,M),
                Select_ne15(8,
                    Instr(Opcode.and, uf,R8,R16,M),
                    Instr(Opcode.tst, R16,M)),
                Instr(Opcode.bic, R8,R16,M),
                Instr(Opcode.bic, uf,R8,R16,M),
                // 4
                Select_ne15(16, 
                    Instr(Opcode.orr, R8,R16,M),
                    Instr(Opcode.mov, R8,M)),
                Select_ne15(16, 
                    Instr(Opcode.orr, uf,R8,R16,M),
                    Instr(Opcode.mov, uf,R8,M)),
                Select_ne15(16,
                    Instr(Opcode.orn, R8,R16,M),
                    Instr(Opcode.mvn, R8,M)),
                Select_ne15(16,
                    Instr(Opcode.orn, uf,R8,R16,M),
                    Instr(Opcode.mvn, uf,R8,M)),
                // 8
                Instr(Opcode.eor, R8,R16,M),
                Select_ne15(8,
                    Instr(Opcode.eor, uf,R8,R16,M),
                    Instr(Opcode.teq, uf,R8,M)),
                invalid,
                invalid,
                // C
                invalid,
                invalid,
                invalid,
                invalid,
                // 10
                Select((16, 4), n => n != 0xD,
                    Instr(Opcode.add, R8,R16,M),
                    Instr(Opcode.add, R9,R16,M)), //$REVIEW: check this
                Select_ne15(8, 
                    Select((16, 4), n => n != 0xD,
                        Instr(Opcode.add, uf,R8,R16,M),
                        Instr(Opcode.add, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Opcode.cmn, R16,M)),
                invalid,
                invalid,
                // 14
                Instr(Opcode.adc, R8,R16,M),
                Instr(Opcode.adc, uf,R9,R16,M),
                Instr(Opcode.sbc, R8,R16,M),
                Instr(Opcode.sbc, uf,R9,R16,M),
                // 18
                invalid,
                invalid,
                Select((16, 4), n => n != 0xD,
                    Instr(Opcode.sub, R8,R16,M),
                    Instr(Opcode.sub, R9,R16,M)), //$REVIEW: check this
                Select_ne15(8, 
                    Select((16, 4), n => n != 0xD,
                        Instr(Opcode.sub, uf,R8,R16,M),
                        Instr(Opcode.sub, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Opcode.cmp, R16,M)),
                // 1C
                Instr(Opcode.rsb, R8,R16,M),
                Instr(Opcode.rsb, uf,R9,R16,M),
                invalid,
                invalid);

            var DataProcessingSimpleImm = Mask(7 + 16, 1, "Data-processing (simple immediate)",
                Mask(5 + 16, 1,
                    Select((16, 4), w => (w & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Opcode.add, R8, R16, Imm26_12_0),
                            Instr(Opcode.add, uf, R8, R16, Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Opcode.add, R8, R16, Imm26_12_0),
                            Instr(Opcode.add, R8, R16, Imm26_12_0))),
                    invalid),
                Mask(5 + 16, 1,
                    invalid,
                    Select((16, 4), w => (w & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Opcode.sub, R8, R16, Imm26_12_0),
                            Instr(Opcode.sub, uf, R8, R16, Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Opcode.sub, R8, R16, Imm26_12_0),
                            Instr(Opcode.sub, R8, R16, Imm26_12_0)))));

            var SaturateBitfield = Mask(5 + 16, 3, "Saturate, Bitfield",
                Instr(Opcode.ssat, Rnp8, ImmM1(0,5), Rnp16, LslImm),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Instr(Opcode.ssat, Rnp8, Imm(0, 5), Rnp16, AsrImm),
                    Instr(Opcode.ssat16, R8, Imm(0, 4), Rnp16)),
                Instr(Opcode.sbfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                Select_ne15(16,
                    DecodeBfcBfi(Opcode.bfi, R8,R16,Imm(12,3,6,2),Imm(0,5)),
                    DecodeBfcBfi(Opcode.bfc, R8, Imm(12, 3, 6, 2), Imm(0, 5))),
                // 4
                Instr(Opcode.usat, R8, ImmM1(0,5), R16, LslImm),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Instr(Opcode.ssat, Rnp8, Imm(0, 5), Rnp16, AsrImm),
                    Instr(Opcode.usat16, R8, Imm(0, 4), R16)),
                Instr(Opcode.ubfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                invalid);

            var MoveWide16BitImm = Mask(7 + 16, 1,
                Instr(Opcode.mov, R8,Imm(PrimitiveType.Word32, fields: Bf((16,4),(26,1),(12,3),(0,8)))),
                Instr(Opcode.movt, Rnp8,Imm(PrimitiveType.Word16, fields: Bf((16,4),(26,1),(12,3),(0,8)))));

            var DataProcessingPlainImm = Mask(8 + 16, 1, "Data processing (plain binary immediate)",
                Mask(5 + 16, 2,
                    DataProcessingSimpleImm,
                    DataProcessingSimpleImm,
                    MoveWide16BitImm,
                    invalid),
                SaturateBitfield);

            var LoadStoreSignedPositiveImm = Select_ne15(12,
                Mask(5 + 16, 2,
                    Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,12))),
                    Instr(Opcode.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0, 12))),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Nyi("PLI"),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSignedImmediatePostIndexed = Mask(5 + 16, 2,
                Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,8))),
                Instr(Opcode.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadStoreSignedNegativeImm = Mask(5 + 16, 2,
                Select((12, 4), w=> w != 0xF,
                    Instr(Opcode.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, offsetFields:(0,8))),
                    Instr(Opcode.pli, nyi("*"))),
                Select((12, 4), w=> w != 0xF,
                    Instr(Opcode.ldrsh, R12, MemOff(PrimitiveType.Int16, 16, offsetFields: (0,8))),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)),        // Reserved hint
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePostIndexed = Mask(4 + 16, 3,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedPositiveImm = Mask(4 + 16, 3, "LoadStoreUnsignedPositiveImm",
                Instr(Opcode.strb, R12,MemOff(PrimitiveType.Byte, 16, offsetFields: (0,12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12))),
                    Instr(Opcode.pld, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12)))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                    Instr(Opcode.pldw, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12)))),
                // 4
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, offsetFields: (0, 12))),
                Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePreIndexed = Mask(4 + 16, 3,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedRegisterOffset = Mask(4 + 16, 3, "Load/store, unsigned (register offset)",
                Instr(Opcode.strb, R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, wide,R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.strh, R12, MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, wide, R12,MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.str, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                Instr(Opcode.ldr, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                invalid,
                invalid);

            var LoadStoreUnsignedNegativeImm = Mask(4 + 16, 3,
                Instr(Opcode.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrb, wide,R12,MemOff(PrimitiveType.Byte,16,indexSpec:idx10, offsetFields:(0, 8))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Opcode.ldrh, wide, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                    Instr(Opcode.pld, nyi("*"))),
                Instr(Opcode.str, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Opcode.ldr, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedUnprivileged = Mask(4 + 16, 3,
                Instr(Opcode.strbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Opcode.strht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Opcode.strt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                Instr(Opcode.ldrt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadUnsignedLiteral = Select_ne15(12, "Load unsigned (literal)",
                Mask(4 + 16, 3,
                    invalid,
                    Instr(Opcode.ldrb, R12, MemOff(PrimitiveType.Byte, baseReg:Registers.pc, offsetFields:(0,12))),
                    invalid,
                    Instr(Opcode.ldrh, R12, MemOff(PrimitiveType.Word16, baseReg: Registers.pc, offsetFields: (0, 12))),
                    
                    invalid,
                    Instr(Opcode.ldr, R12, MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetFields:(0,12))),
                    invalid,
                    invalid),
                Mask(4 + 16, 3,
                    invalid,
                    Instr(Opcode.pld, MemOff(PrimitiveType.Word32, baseReg: Registers.pc, offsetFields: (0, 12))),
                    invalid,
                    Instr(Opcode.pld, MemOff(PrimitiveType.Word32, baseReg: Registers.pc, offsetFields: (0, 12))),
                    
                    invalid,
                    invalid,
                    invalid,
                    invalid));

            var LoadSignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 2,
                    Instr(Opcode.ldrsb, R12, MemOff(PrimitiveType.SByte, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    Instr(Opcode.ldrsh, R12, MemOff(PrimitiveType.Int16, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Instr(Opcode.pli, nyi("* literal")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));

            var LoadStoreSignedRegisterOffset = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 2, "Load/store, signed (register offset)",
                    Instr(Opcode.ldrsb, wide, R12, MemIdx(PrimitiveType.SByte, 16, 0, (4, 2))),
                    Instr(Opcode.ldrsh, wide, R12, MemIdx(PrimitiveType.Int16, 16, 0, (4, 2))),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Instr(Opcode.pli, nyi("*register")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));

            var LoadStoreSingle = Mask(7 + 16, 2, "Load/store single",
                Select_ne15(16, "",
                    Mask(10, 2, "  op0=0b00 op3",
                        Select((6, 6), n => n == 0,
                            LoadStoreUnsignedRegisterOffset,
                            invalid),
                        invalid,
                        Select((8, 1), n => n == 0,
                            invalid,
                            LoadStoreUnsignedImmediatePostIndexed),
                        Mask(8, 2,
                            LoadStoreUnsignedNegativeImm,
                            LoadStoreUnsignedImmediatePreIndexed,
                            LoadStoreUnsignedUnprivileged,
                            LoadStoreUnsignedImmediatePreIndexed)),
                    LoadUnsignedLiteral),
                Select((16,4), n => n != 0xF,
                    LoadStoreUnsignedPositiveImm,
                    LoadUnsignedLiteral),
                Select((16, 4), w  => w != 0xF,
                    Mask(10, 2, " op0=0b10 op3",
                        Select((6, 6), n => n == 0,
                            LoadStoreSignedRegisterOffset,
                            invalid),
                        invalid,
                        Select((8, 1), n => n == 0,
                            invalid,
                            LoadStoreSignedImmediatePostIndexed),
                        Mask(8, 2,
                            LoadStoreSignedNegativeImm,
                            Nyi("LoadStoreSignedImmediatePreIndexed"),
                            Nyi("LoadStoreSignedUnprivileged"),
                            Nyi("LoadStoreSignedImmediatePreIndexed"))),
                    LoadSignedLiteral),
                Select_ne15(16, 
                    LoadStoreSignedPositiveImm,
                    LoadSignedLiteral));

            var ldc_literal = Nyi("LDC (literal)");

            var SystemRegisterLdSt = Select((8,1), n => n != 0, "SystemRegisterLdSt",
                invalid,
                Select((12,4), n => n != 5,
                    invalid,
                    Select((22,1), n => n != 0,
                        invalid,
                        Mask(Bf((23,2),(20,2)), "  PU-WL",
                            invalid,
                            invalid,
                            Instr(Opcode.stc, nyi("*post-indexed")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*imm")),
                                ldc_literal),

                            Instr(Opcode.stc, nyi("*unindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*immediate - unindexed variant")),
                                ldc_literal),
                            invalid,
                            invalid, 

                            Instr(Opcode.stc, nyi("*offset variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*offset variant")),
                                ldc_literal),
                            Instr(Opcode.stc, nyi("*preindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*preindexed variant")),
                                ldc_literal),

                            Instr(Opcode.stc, CPn14,CR12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*offset variant")),
                                ldc_literal),
                            Instr(Opcode.stc, nyi("*preindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Opcode.ldc, nyi("*preindexed variant")),
                                ldc_literal)))));


            var StoreCoprocessor = Mask(12 + 16, 1, "  store-nonPC",
                Instr(Opcode.stc, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Opcode.stc2, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));

            var LoadCoprocessor = Select_ne15(16, "",
                Mask(12 + 16, 1, "  load-nonPC",
                    Instr(Opcode.ldc, nyi("*")),
                    Mask(6 + 16, 1, "  ldc2{l}",
                        Instr(Opcode.ldc2, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                        Instr(Opcode.ldc2l, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))))),
                Nyi("load PC"));


            //$REVIEW: This decoder tree was taken from the old ARMv7 manual. It seems
            // many coprocessor instructions are no longer supported in ARMv8?
            var Coproc = Mask(Bf((8 + 16, 2), (4 + 16, 1)), "Coprocessor",
                StoreCoprocessor,
                LoadCoprocessor,
                StoreCoprocessor,
                LoadCoprocessor,

                Nyi("10xxx0"),
                Nyi("10xxx1"),
                Nyi("11xxx0"),
                Nyi("11xxx1"));

            var SystemRegister64bitMove = Mask(12 + 16, 1, 6 + 16, 1, "System register 64-bit move",
                Coproc,
                Mask(4 + 16, 1, "  o0:D=0b01",
                    Instr(Opcode.mcrr, CP8, Imm(4, 4), Rnp12, Rnp16, CR0),
                    Instr(Opcode.mrrc, CP8, Imm(4, 4), Rnp12,Rnp16,CR0)),
                Coproc,
                Coproc);

            var SystemRegisterLdStAnd64bitMove = Select(Bf((23,2),(21,1)), n => (n & 0xD) == 0,
                SystemRegister64bitMove,
                SystemRegisterLdSt);

            var vstmia = Mask(8, 2, // size
                    invalid,
                    invalid,
                    Instr(Opcode.vstmia, nyi("*")),
                    Mask(0, 1,
                        Instr(Opcode.vstmia, nyi("*")),
                        Instr(Opcode.fstmiax, nyi("*"))));

            var vldmia = Mask(8, 2, "VLDMIA", 
                    invalid,
                    invalid,
                    Instr(Opcode.vldmia, w(21), R16, mrsimdS((0, 8))),
                    Mask(0, 1,
                        Instr(Opcode.vldmia, w(21), R16, mrsimdD((1, 7))),
                        Instr(Opcode.fldmiax, nyi("*"))));
            var vstr = Mask(8, 2,  // size
                invalid,
                Instr(Opcode.vstr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.vstr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.vstr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var vldr = Select_ne15(16, "",
                Mask(8, 2,
                    invalid,
                    Instr(Opcode.vldr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Opcode.vldr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Opcode.vldr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8)))),
                Mask(8, 2, "  (literal)",
                    invalid,
                    Instr(Opcode.vldr, F12_22, MemOff(PrimitiveType.Real16, 16, offsetShift:1, offsetFields:(0,8))),
                    Instr(Opcode.vldr, F12_22, MemOff(PrimitiveType.Real16, 16, offsetShift:2, offsetFields:(0,8))),
                    Instr(Opcode.vldr, D22_12, MemOff(PrimitiveType.Real16, 16, offsetShift:2, offsetFields:(0,8)))));

            var AdvancedSimdAndFpLdSt = Mask(4 + 16, 5, "Advanced SIMD and floating-point load/store",
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                vstmia,
                vldmia,
                vstmia,
                vldmia,

                vstmia,
                vldmia,
                vstmia,
                vldmia,
                // 0x10
                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid,

                vstr,
                vldr,
                invalid,
                invalid);

            var AdvancedSimdAndFp64bitMove = Mask(6 + 16, 1, 4, 1, "Advanced SIMD and floating-point 64-bit move",
                invalid,
                invalid,
                invalid,

                Select((6, 2), n => n != 0,
                    invalid,
                    Mask(4 + 16, 1, 8, 2, "  opc2=0b00",
                        invalid,
                        invalid,

                        Instr(Opcode.vmov, S_pair(0, 5), Rnp12, Rnp16),
                        Instr(Opcode.vmov, D5_0, Rnp12, Rnp16),

                        invalid,
                        invalid,
                        Instr(Opcode.vmov, Rnp12, Rnp16, S_pair(0, 5)),
                        Instr(Opcode.vmov, Rnp12, Rnp16, D5_0))));

            var AvancedSimdLdStAnd64bitMove = Select((5 + 16, 4), w => (w & 0b1101) == 0, "Advanced SIMD load/store and 64-bit move",
                AdvancedSimdAndFp64bitMove,
                AdvancedSimdAndFpLdSt);

            var FloatingPointDataProcessing2Regs = Mask(16, 4, "Floating-point data-processing (two registers)",
                    Mask(7, 1, " opc1:opc2=0000 o3",
                        Mask(8, 2, "  o3=0",
                            invalid,
                            invalid,
                            Instr(Opcode.vmov, F32, F12_22, F0_5),
                            Instr(Opcode.vmov, F64, D22_12, D5_0)),
                        Instr(Opcode.vabs, nyi("*"))),
                    Mask(7, 1, "  op1:opc2=0001 o3",
                        Instr(Opcode.vneg, vf8_HSD, FP12, FP0),
                        Instr(Opcode.vsqrt, vf8_HSD, FP12, FP0)),
                    Nyi("0010 - _HSD"),
                    Nyi("0011 - _HSD"),

                    Mask(7, 1, "  op1:opc2=0100 o3",
                        Instr(Opcode.vcmp, vf8_HSD, FP12, FP0),
                        Instr(Opcode.vcmpe, vf8_HSD, FP12, FP0)),
                    Mask(7, 1, "  op1:opc2=0101 o3",
                        Mask(8, 2,
                            invalid,
                            Instr(Opcode.vcmp, F16, FP12, Imm0_r32),
                            Instr(Opcode.vcmp, F32, FP12, Imm0_r32),
                            Instr(Opcode.vcmp, F64, FP12, Imm0_r64)),
                        Mask(8, 2,
                            invalid,
                            Instr(Opcode.vcmpe, F16, FP12, Imm0_r32),
                            Instr(Opcode.vcmpe, F32, FP12, Imm0_r32),
                            Instr(Opcode.vcmpe, F64, FP12, Imm0_r64))),
                    Mask(7, 1, "  op1:opc2=0101 o3",
                        Instr(Opcode.vrintr, nyi("*")),
                        Instr(Opcode.vrintz, nyi("*"))),
                    Mask(7, 1, "  op1:opc2=0101 o3",
                        Instr(Opcode.vrintx, nyi("*")),
                        Mask(8, 2,
                            invalid,
                            invalid,
                            Instr(Opcode.vcvt, F64F32, D22_12, F0_5),
                            Instr(Opcode.vcvt, F32F64, F12_22, D5_0))),

                    Mask(7, 3, "  op1:opc2=1000",
                        invalid,
                        invalid,
                        Instr(Opcode.vcvt, F16U32, F12_22, F0_5),
                        Instr(Opcode.vcvt, F16S32, F12_22, F0_5),

                        Instr(Opcode.vcvt, F32U32, F12_22, F0_5),
                        Instr(Opcode.vcvt, F32S32, F12_22, F0_5),
                        Instr(Opcode.vcvt, F64U32, D22_12, F0_5),
                        Instr(Opcode.vcvt, F64S32, D22_12, F0_5)),

                    Nyi("1001 - _HSD"),
                    Nyi("1010 - _HSD"),
                    Nyi("1011 - _HSD"),

                    Mask(7, 1, "  op1:opc2=1100",
                        Instr(Opcode.vcvtr, nyi("*")),
                        Mask(8, 2, 
                            invalid,
                            Instr(Opcode.vcvt, U32F16, F12_22, F0_5),
                            Instr(Opcode.vcvt, U32F32, F12_22, F0_5),
                            Instr(Opcode.vcvt, U32F64, F12_22, D5_0))),
                    Mask(7, 1, "  op1:opc2=1101",
                        Instr(Opcode.vcvtr, nyi("*")),
                        Mask(8, 2,
                            invalid,
                            Instr(Opcode.vcvt, S32F16, F12_22, F0_5),
                            Instr(Opcode.vcvt, S32F32, F12_22, F0_5),
                            Instr(Opcode.vcvt, S32F64, F12_22, D5_0))),
                    Nyi("1110 - _HSD"),
                    Nyi("1111 - _HSD")
                );

            var FloatingPointDataProcessing3Regs = Mask(Bf((7 + 16, 1), (4 + 16, 2), (6, 1)), "Floating-point data-processing (three registers)",
                    Instr(Opcode.vmla, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vmls, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vmls, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vmla, vf8_HSD, FP12, FP16, FP0),

                    Instr(Opcode.vmul, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vnmul, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vadd, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vsub, vf8_HSD, FP12, FP16, FP0),

                    Instr(Opcode.vdiv, vf8_HSD, FP12, FP16, FP0),
                    invalid,
                    Instr(Opcode.vfnms, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vfnma, vf8_HSD, FP12, FP16, FP0),

                    Instr(Opcode.vfma, vf8_HSD, FP12, FP16, FP0),
                    Instr(Opcode.vfms, vf8_HSD, FP12, FP16, FP0),
                    invalid,
                    invalid);

            var FloatingPointMoveImm = Mask(8, 2, "Floating-point move immediate on page F3-3152",
                invalid,
                Instr(Opcode.vmov, F16, FP12, vfpImm32(16, 4, 0, 4)),
                Instr(Opcode.vmov, F32, FP12, vfpImm32(16, 4, 0, 4)),
                Instr(Opcode.vmov, F64, FP12, vfpImm64(16, 4, 0, 4)));

            var FloatingPointConditionalSelect = Mask(20, 2, "Floating-point conditional select",
                Instr(Opcode.vseleq, vf8_HSD, FP12, FP16, FP0),
                Instr(Opcode.vselvs, vf8_HSD, FP12, FP16, FP0),
                Instr(Opcode.vselge, vf8_HSD, FP12, FP16, FP0),
                Instr(Opcode.vselgt, vf8_HSD, FP12, FP16, FP0));

            var FloatingPointMinNumMaxNum =
                Mask(6, 1,
                    Mask(8, 2,
                        invalid,
                        Instr(Opcode.vmaxnm, F16, F12_22,F16_7,F0_5),
                        Instr(Opcode.vmaxnm, F32, F12_22,F16_7,F0_5),
                        Instr(Opcode.vmaxnm, F64, D22_12,D7_16,D5_0)),
                    Mask(8, 2,
                        invalid,
                        Instr(Opcode.vminnm, F16, F12_22,F16_7,F0_5),
                        Instr(Opcode.vminnm, F32, F12_22,F16_7,F0_5),
                        Instr(Opcode.vminnm, F64, D22_12,D7_16,D5_0)));

            var FloatingPointExtIns = Nyi("FloatingPointExtIns");
            var FloatingPointDirectedCvt2Int = Mask(16, 3, "Floating-point directed convert to integer",
                Instr(Opcode.vrinta, vf8_HSD, FP12, FP0),
                Instr(Opcode.vrintn, vf8_HSD, FP12, FP0),
                Instr(Opcode.vrintp, vf8_HSD, FP12, FP0),
                Instr(Opcode.vrintm, vf8_HSD, FP12, FP0),
                Instr(Opcode.vcvta, vif8_HSD, FP12, FP0),
                Instr(Opcode.vcvtn, vif8_HSD, FP12, FP0),
                Instr(Opcode.vcvtp, vif8_HSD, FP12, FP0),
                Instr(Opcode.vcvtm, vif8_HSD, FP12, FP0));


            var FloatingPointDataProcessing = Mask(12 + 16, 1, "Floating-point data-processing",
                Mask(4 + 16, 4, // op1
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    Mask(6, 1,
                        FloatingPointMoveImm,
                        FloatingPointDataProcessing2Regs),

                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    FloatingPointDataProcessing3Regs,
                    Mask(6, 1,
                        FloatingPointMoveImm,
                        FloatingPointDataProcessing2Regs)),
                Select((8,2), n => n != 0,
                    Mask(4 + 16, 4, // op1
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,

                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,
                        FloatingPointConditionalSelect,

                        FloatingPointMinNumMaxNum,
                        invalid,
                        invalid,
                        Mask(6, 1,
                            invalid,
                            Select((16,4), n => n == 0,
                                FloatingPointExtIns,
                                Mask(19, 1,
                                    invalid,
                                    FloatingPointDirectedCvt2Int))),

                        FloatingPointMinNumMaxNum,
                        invalid,
                        invalid,
                        Mask(6, 1,
                            invalid,
                            Select((16,4), n => n == 0,
                                FloatingPointExtIns,
                                Mask(19, 1,
                                    invalid,
                                    FloatingPointDirectedCvt2Int)))),
                    invalid));

            var AdvancedSimdLdStSingleStructureOneLane = Mask(Bf((5 + 16, 1), (10, 2), (8, 2)), "Advanced SIMD load/store single structure to one lane",
                Instr(Opcode.vst1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Opcode.vst2, nyi("single 2-element structure from one lane - T1")),
                Instr(Opcode.vst3, nyi("single 3-element structure from one lane - T1")),
                Instr(Opcode.vst4, nyi("single 4-element structure from one lane - T1")),

                Instr(Opcode.vst1, nyi("single element from one lane - T2")),
                Instr(Opcode.vst2, vi10BHW_, DlistIdx2_5_1, MsingleElem),
                Instr(Opcode.vst3, vi10BHW_, DlistIdx3_5_1, MsingleElem),
                Instr(Opcode.vst4, nyi("single 4-element structure from one lane - T2")),

                Instr(Opcode.vst1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Opcode.vst2, nyi("single 2-element structure from one lane - T3")),
                Instr(Opcode.vst3, vi10BHW_, DlistIdx3_4_1, MsingleElem),
                Instr(Opcode.vst4, nyi("single 4-element structure from one lane - T3")),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Opcode.vld1, nyi("single element from one lane - T1")),
                Instr(Opcode.vld2, nyi("single 2-element structure from one lane - T1")),
                Instr(Opcode.vld3, nyi("single 3-element structure from one lane - T1")),
                Instr(Opcode.vld4, vi10BHW_, DlistIdx4_2, MsingleElem),

                Instr(Opcode.vld1, nyi("single element from one lane - T2")),
                Instr(Opcode.vld2, nyi("single 2-element structure from one lane - T2")),
                Instr(Opcode.vld3, vi10BHW_, DlistIdx3_7_1, MsingleElem),
                Instr(Opcode.vld4, nyi("single 4-element structure from one lane - T2")),

                Instr(Opcode.vld1, nyi("single element from one lane - T3")),
                Instr(Opcode.vld2, nyi("single 2-element structure from one lane - T3")),
                Instr(Opcode.vld3, nyi("single 3-element structure from one lane - T3")),
                Instr(Opcode.vld4, vi10BHW_, DlistIdx4_2, MsingleElem),

                invalid,
                invalid,
                invalid,
                invalid);


            var AdvancedSimdAndFloatingPoint32bitMove = Mask(8, 1, "Advanced SIMD and floating-point 32-bit move",
                Select((21,3), n => n == 0,
                    Mask(20, 1,
                        Instr(Opcode.vmov, F16_7, Rnp12),
                        Instr(Opcode.vmov, Rnp12, F16_7)),
                    Select((21,3), n => n == 7,
                        Mask(20, 1,
                            Instr(Opcode.vmsr, nyi("*")),
                            Select_ne15(12, "",
                                Instr(Opcode.vmrs, R12, SIMDSysReg(16)),
                                Instr(Opcode.vmrs, cpsr, SIMDSysReg(16)))), //$REVIEW: should be apsr
                        invalid)),
                Nyi("AdvancedSimd8_16_32_bitElementMove"));

            var AdvancedSimdLdStMultipleStructures = Mask(21, 1, "AdvancedSimdLdStMultipleStructures",
                Mask(8, 4,
                    Instr(Opcode.vst4, vi6BHW_, Dlist4, Melem16Align),
                    Instr(Opcode.vst4, vi6BHW_, Dlist4_2, Melem16Align),
                    Instr(Opcode.vst1, nyi("*multiple single elements - T4")),
                    Instr(Opcode.vst2, vi6BHW_, Dlist4, Melem16Align),

                    Instr(Opcode.vst3, vi6BHW_, Dlist3, Melem16Align),
                    Instr(Opcode.vst3, vi6BHW_, Dlist3_2, Melem16Align),
                    Instr(Opcode.vst1, vi6BHWD, Dlist3, Melem16Align),
                    Instr(Opcode.vst1, vi6BHWD, Dlist1, Melem16Align),

                    Instr(Opcode.vst2, vi6BHW_, Dlist2, Melem16Align),
                    Instr(Opcode.vst2, vi6BHW_, Dlist2_2, Melem16Align),
                    Instr(Opcode.vst1, vi6BHWD, Dlist2, Melem16Align),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(8, 4, "  L",
                    Instr(Opcode.vld4, vi6BHW_, Dlist4, Melem16Align),
                    Instr(Opcode.vld4, vi6BHW_, Dlist4_2, Melem16Align),
                    Instr(Opcode.vld1, vi6BHWD, Dlist4, Melem16Align),
                    Instr(Opcode.vld2, vi6BHW_, Dlist4, Melem16Align),

                    Instr(Opcode.vld3, vi6BHW_, Dlist3, Melem16Align),
                    Instr(Opcode.vld3, vi6BHW_, Dlist3_2, Melem16Align),
                    Instr(Opcode.vld1, vi6BHWD, Dlist3, Melem16Align),
                    Instr(Opcode.vld1, vi6BHWD, Dlist1, Melem16Align),

                    Instr(Opcode.vld2, vi6BHW_, Dlist2, Melem16Align),
                    Instr(Opcode.vld2, vi6BHW_, Dlist2_2, Melem16Align),
                    Instr(Opcode.vld1, vi6BHWD, Dlist2, Melem16Align),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid));

            var AdvancedSimdElementOrStructureLdSt = Mask(7 + 16, 1, "Advanced SIMD element or structure load/store",
                AdvancedSimdLdStMultipleStructures,
                Mask(10, 2,
                    AdvancedSimdLdStSingleStructureOneLane,
                    AdvancedSimdLdStSingleStructureOneLane,
                    AdvancedSimdLdStSingleStructureOneLane,
                    Nyi("AdvancedSimdLdSingleStructureToAllLanes")));

            var SystemRegister32bitMove = Mask(12 + 16, 1, 
                Mask(4 + 16, 1,
                    Instr(Opcode.mcr, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3)),
                    Instr(Opcode.mrc, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3))),
                invalid);

            var AdvancedSimd3RegistersSameLength = Mask(8, 4, "Advanced SIMD three registers of the same length",
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhadd, viuBHW_, D22_12,D7_16,D5_0),
                        Instr(Opcode.vhadd, viuBHW_, Q22_12,Q7_16,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vqadd, viuBHWD, D22_12,D7_16,D5_0),
                        Instr(Opcode.vqadd, viuBHWD, Q22_12,Q7_16,Q5_0))),
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // o1
                        Instr(Opcode.vrhadd, nyi("*")),
                        Mask(4 + 16, 2, // size
                            Instr(Opcode.vand, nyi("*register")),
                            Instr(Opcode.vbic, nyi("*register")),
                            Instr(Opcode.vorr, nyi("*register")),
                            Instr(Opcode.vorn, nyi("*register")))),
                    Mask(4, 1,      // o1),
                        Instr(Opcode.vrhadd, nyi("*")),
                        Mask(4 + 16, 2, // size
                            Mask(6, 1, // Q
                                Instr(Opcode.veor, D22_12,D7_16,D5_0),
                                Instr(Opcode.veor, Q22_12,Q7_16,Q5_0)),
                            Instr(Opcode.vbsl, nyi("*register")),
                            Instr(Opcode.vbit, q6, W22_12, W7_16, W5_0),
                            Instr(Opcode.vbif, q6, W22_12, W7_16, W5_0)))),
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Opcode.vhsub, viuBHW_, D22_12,D7_16,D5_0),
                        Instr(Opcode.vhsub, viuBHW_, Q22_12,Q7_16,Q5_0)),
                    Instr(Opcode.vqsub, nyi("*"))),
                Nyi("AdvancedSimd3RegistersSameLength_opc3"),

                Nyi("AdvancedSimd3RegistersSameLength_opc4"),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vrshl, vu_bhwd(20),D22_12,D5_0,D7_16),
                        Instr(Opcode.vrshl, vu_bhwd(20),Q22_12,Q5_0,Q7_16)),
                    Mask(6, 1, // Q
                        Instr(Opcode.vqrshl, vu_bhwd(20),D22_12,D7_16,D5_0),
                        Instr(Opcode.vqrshl, vu_bhwd(20),Q22_12,Q7_16,Q5_0))),
                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Opcode.vmax, viuBHW_,D22_12,D7_16,D5_0),
                        Instr(Opcode.vmax, viuBHW_,Q22_12,Q7_16,Q5_0)),
                    Mask(6, 1, // Q
                        Instr(Opcode.vmin, viuBHW_, D22_12,D7_16,D5_0),
                        Instr(Opcode.vmin, viuBHW_, Q22_12,Q7_16,Q5_0))),
                Mask(4, 1,
                    Instr(Opcode.vabd, viuBHW_, q6, W22_12, W7_16, W5_0),
                    Instr(Opcode.vaba, viuBHW_, q6, W22_12, W7_16, W5_0)),

                Mask(12 + 16, 1,  // U
                    Mask(4, 1, // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vadd, viBHWD,D22_12,D7_16,D5_0),
                            Instr(Opcode.vadd, viBHWD,Q22_12,Q7_16,Q5_0)),
                        Mask(6, 1, // Q
                            Instr(Opcode.vtst, viBHWD, D22_12,D7_16,D5_0),
                            Instr(Opcode.vtst, viBHWD, Q22_12,Q7_16,Q5_0))),
                    Mask(4, 1, "opc=8 U=1 op1",
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Opcode.vsub, viBHWD,D22_12,D7_16,D5_0),
                            Instr(Opcode.vsub, viBHWD,Q22_12,Q7_16,Q5_0)),
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Opcode.vceq, viBHWD, D22_12,D7_16,D5_0),
                            Instr(Opcode.vceq, viBHWD, Q22_12,Q7_16,Q5_0)))),
                // opc9
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmla, viBHW_, D22_12,D7_16,D5_0),
                            Instr(Opcode.vmla, viBHW_, Q22_12,Q7_16,Q5_0)),
                        Nyi("*vmul (integer and polynomial")),
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Opcode.vmls, viu_HW__HW_, D22_12,D7_16,D5_0),
                            Instr(Opcode.vmls, viu_HW__HW_, Q22_12,Q7_16,Q5_0)),
                        Nyi("*vmul (integer and polynomial"))),
                Mask(6, 1, // Q
                    Mask(4, 1, // op1
                        Instr(Opcode.vpmax, viuBHW_, D22_12,D7_16,D5_0),
                        Instr(Opcode.vpmin, viuBHW_, D22_12,D7_16,D5_0)),
                    invalid),
                Nyi("AdvancedSimd3RegistersSameLength_opcB"),

                Nyi("AdvancedSimd3RegistersSameLength_opcC"),
                // opcD
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 2,  // size
                                Instr(Opcode.vadd, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vadd, F16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vsub, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vsub, F16, D22_12,D7_16,D5_0)),
                            Mask(20, 2,  // size
                                Instr(Opcode.vadd, F32, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vadd, F16, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vsub, F32, Q22_12,Q7_16,Q5_0),
                                Instr(Opcode.vsub, F16, Q22_12,Q7_16,Q5_0))),
                        Mask(20, 2,  // high-bit of size
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmls (floating point)"),
                            Nyi("*vmls (floating point)"))),
                    Mask(4, 1,      // op1
                        Mask(20, 2,  // size
                            Instr(Opcode.vpadd, F32, D22_12,D7_16,D5_0),
                            Instr(Opcode.vpadd, F16, D22_12,D7_16,D5_0),
                            Nyi("*vabd (floating point)"),
                            Nyi("*vabd (floating point)")),
                        Mask(21, 1,  // high-bit of size
                            Mask(6, 1,      // Q
                                Instr(Opcode.vmul, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vmul, F16, Q22_12,Q7_16,Q5_0)),
                            invalid))),

                // opc = E
                Mask(12 + 16, 1,  // U
                    Nyi("AdvancedSimd3RegistersSameLength_opcE U=0"),
                    Mask(21, 1,  // high-bit of size
                        Mask(4, 1,      // op1
                            Mask(6, 1,      // Q
                                Instr(Opcode.vcge, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vcge, F16, Q22_12,Q7_16,Q5_0)),
                            Instr(Opcode.vacge, nyi("*"))),
                        Mask(4, 1, // op1
                            Mask(6, 1,    // Q
                                Instr(Opcode.vcgt, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vcgt, F16, Q22_12,Q7_16,Q5_0)),
                            Nyi("AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1")))),
                // opc = F
                Mask(12 + 16, 1,  "  opc=0b1111 U",
                    Mask(5+16, 1, 4, 1, "  size<1>:o1",
                        Mask(4+16, 1, "  sz",
                            Instr(Opcode.vmax, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, F32, q6, W22_12, W7_16, W5_0)),
                        Instr(Opcode.vrecps, nyi("*")),
                        Mask(4 + 16, 1, "  sz",
                            Instr(Opcode.vmax, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Opcode.vmax, F32, q6, W22_12, W7_16, W5_0)),
                        Mask(4 + 16, 1, "  sz",
                            Instr(Opcode.vrsqrts, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Opcode.vrsqrts, F32, q6, W22_12, W7_16, W5_0))),
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 2,  // size
                                Instr(Opcode.vpmax, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmax, F16, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, F32, D22_12,D7_16,D5_0),
                                Instr(Opcode.vpmin, F16, D22_12,D7_16,D5_0)),
                            Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1")),
                        Nyi("AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1"))));

            var vclt_imm0 = Instr(Opcode.vclt, q6, vif, W22_12, W5_0, IW0);



            var AdvancedSimd3RegistersSameLengthExt = Mask(7 + 16, 2, 4 + 16, 2, "Advanced SIMD three registers of the same length extension",
                    Nyi("op1:op2=0b0000"),
                    Nyi("op1:op2=0b0001"),
                    Nyi("op1:op2=0b0010"),
                    Nyi("op1:op2=0b0011"),

                    Nyi("op1:op2=0b0100"),
                    Nyi("op1:op2=0b0101"),
                    Nyi("op1:op2=0b0110"),
                    Nyi("op1:op2=0b0111"),

                    Nyi("op1:op2=0b1000"),
                    Nyi("op1:op2=0b1001"),
                    Nyi("op1:op2=0b1010"),
                    Select(Bf((10, 1), (8, 1), (4, 1)), n => n == 0, "op1:op2=0b1011",
                        Nyi("VCMLA"),
                        Coproc),

                    Nyi("op1:op2=0b1100"),
                    Nyi("op1:op2=0b1101"),
                    Nyi("op1:op2=0b1110"),
                    Nyi("op1:op2=0b1111"));

            var AdvancedSimd2RegsMisc = Mask(16, 2, "Advanced SIMD two registers misc",
                Mask(7, 4,
                    Instr(Opcode.vrev64, nyi("*")),
                    Instr(Opcode.vrev32, nyi("*")),
                    Instr(Opcode.vrev16, nyi("*")),
                    invalid,

                    Instr(Opcode.vpaddl, nyi("*")),
                    Instr(Opcode.vpaddl, nyi("*")),
                    Mask(6, 1,
                        Instr(Opcode.aese, nyi("*")),
                        Instr(Opcode.aesd, nyi("*"))),
                    Mask(6, 1,
                        Instr(Opcode.aesmc, nyi("*")),
                        Instr(Opcode.aesimc, nyi("*"))),

                    invalid, //$REVIEW VSWP looks odd.
                    Instr(Opcode.vclz, nyi("*")),
                    Instr(Opcode.vcnt, nyi("*")),
                    Instr(Opcode.vmvn, nyi("*reg")),

                    Instr(Opcode.vpadal, nyi("*")),
                    Instr(Opcode.vpadal, nyi("*")),
                    Instr(Opcode.vqabs, nyi("*")),
                    Instr(Opcode.vqneg, nyi("*"))),
                Mask(7, 4,
                    Instr(Opcode.vcgt, vsfBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Opcode.vcge, vsfBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Opcode.vceq, vifBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Opcode.vcle, vsfBHW__HS_, D22_12, D5_0, ImmV0),

                    vclt_imm0,
                    Mask(6, 1,
                        invalid,
                        Instr(Opcode.sha1h, nyi("*"))),
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi18BHWD, D22_12,D5_0),
                            Instr(Opcode.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi18BHWD, Q22_12,Q5_0),
                            Instr(Opcode.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Opcode.vneg, nyi("*")),

                    Instr(Opcode.vcgt, nyi("*imm0")),
                    Instr(Opcode.vcge, nyi("*imm0")),
                    Instr(Opcode.vceq, nyi("*imm0")),
                    Instr(Opcode.vcle, nyi("*imm0")),

                    vclt_imm0,
                    invalid,
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi18BHWD, D22_12,D5_0),
                            Instr(Opcode.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Opcode.vabs, vi18BHWD, Q22_12,Q5_0),
                            Instr(Opcode.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Opcode.vqneg, nyi("*"))),
                Mask(7, 4,
                    invalid,
                    Instr(Opcode.vtrn, vi18BHW_, q6,W22_12, W5_0),
                    Instr(Opcode.vuzp, nyi("*")),
                    Instr(Opcode.vzip, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vmovn, nyi("*")),
                        Instr(Opcode.vqmovn, nyi("*unsigned"))),
                    Instr(Opcode.vqmovn, nyi("*signed")),
                    Mask(6, 1,
                        Instr(Opcode.vshll, nyi("*")),
                        invalid),
                    Mask(6, 1,
                        Instr(Opcode.sha1su1, nyi("*")),
                        Instr(Opcode.sha256su0, nyi("*"))),

                    Instr(Opcode.vrintn, nyi("*")),
                    Instr(Opcode.vrintx, nyi("*")),
                    Instr(Opcode.vrinta, nyi("*")),
                    Instr(Opcode.vrintz, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        invalid),
                    Instr(Opcode.vrintm, nyi("*")),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0),
                        invalid),
                    Instr(Opcode.vrintp, nyi("*"))),
                Mask(4 + 16, 4,
                    Instr(Opcode.vcvta, nyi("*")),
                    Instr(Opcode.vcvta, nyi("*")),
                    Instr(Opcode.vcvtn, nyi("*")),
                    Instr(Opcode.vcvtn, nyi("*")),

                    Instr(Opcode.vcvtp, nyi("*")),
                    Instr(Opcode.vcvtp, nyi("*")),
                    Instr(Opcode.vcvtm, nyi("*")),
                    Instr(Opcode.vcvtm, nyi("*")),

                    Instr(Opcode.vrecpe, nyi("*")),
                    Instr(Opcode.vrsqrte, nyi("*")),
                    Instr(Opcode.vrecpe, nyi("*")),
                    Instr(Opcode.vrsqrte, nyi("*")),

                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Opcode.vcvt, vc,D22_12,D5_0),
                        Instr(Opcode.vcvt, vc,Q22_12,Q5_0))));

            var AdvancedSimd3DiffLength = Mask(8, 4, "Advanced SIMD three registers of different lengths",
                Instr(Opcode.vaddl, viuBHW_, Q22_12, D7_16, D5_0),
                Instr(Opcode.vaddw, viuBHW_, Q22_12, Q7_16, D5_0),
                Instr(Opcode.vsubl, viuBHW_, Q22_12, D7_16, D5_0),
                Instr(Opcode.vsubw, viuBHW_, Q22_12, Q7_16, D5_0),

                Mask(12 + 16, 1,
                    Instr(Opcode.vaddhn, viHWD_, D22_12, Q7_16, Q5_0),
                    Instr(Opcode.vraddhn, viHWD_, D22_12, Q7_16, Q5_0)),
                Instr(Opcode.vabal, viuBHW_, Q22_12, D7_16, D5_0),
                Mask(12 + 16, 1,
                    Instr(Opcode.vsubhn, viHWD_, D22_12, Q7_16, Q5_0),
                    Instr(Opcode.vrsubhn, viHWD_, D22_12, Q7_16, Q5_0)),
                Instr(Opcode.vabdl, viuBHW_, Q22_12,D7_16,D5_0),

                Instr(Opcode.vmlal, viuBHW_, Q22_12,D7_16,D5_0),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlal, nyi("*integer")),
                    invalid),
                Instr(Opcode.vmlsl, viuBHW_, Q22_12, D7_16, D5_0),
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmlsl, nyi("*integer")),
                    invalid),

                Instr(Opcode.vmull, viuBHW_, Q22_12, D7_16, D5_0),   //$TODO: polynomial?
                Mask(12 + 16, 1,
                    Instr(Opcode.vqdmull, nyi("*integer")),
                    invalid),
                invalid,
                invalid);

            var AdvancedSimd2RegsScalar = Mask(8, 4, "Advanced SIMD two registers and a scalar",
                Mask(12 +16, 1,
                    Instr(Opcode.vmla, v_hw_(20), D22_12,D7_16,D5_0),
                    Instr(Opcode.vmla, v_hw_(20), Q22_12,Q7_16,Q5_0)),
                Mask(12 + 16, 1,
                    Instr(Opcode.vmla, vF(20), D22_12,D7_16,D5_0),
                    Instr(Opcode.vmla, vF(20), Q22_12,Q7_16,Q5_0)),
                Instr(Opcode.vmlal, nyi("*scalar")),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlal, nyi("*")),
                    invalid),

                Instr(Opcode.vmls, nyi("*scalar")),
                Instr(Opcode.vmls, nyi("*scalar")),
                Mask(20, 2, "VMLSL (scalar)", 
                    invalid,
                    Instr(Opcode.vmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Opcode.vmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 4), Ix(5, 1)),
                    invalid),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmlsl, nyi("*")),
                    invalid),

                Instr(Opcode.vmul, nyi("*scalar")),
                Instr(Opcode.vmul, nyi("*scalar")),
                Instr(Opcode.vmull, nyi("*")),
                Mask(12 + 16, 1, // Q
                    Instr(Opcode.vqdmull, nyi("*")),
                    invalid),

                Instr(Opcode.vqdmulh, nyi("*")),
                Instr(Opcode.vqrdmlah, nyi("*")),
                Instr(Opcode.vqrdmlah, nyi("*")),
                Instr(Opcode.vqrdmlsh, nyi("*")));

            var AdvancedSimdDuplicateScalar = Mask(7, 3, "Advanced SIMD duplicate (scalar)",
                Mask(16, 3, "VDUP (scalar)",
                    invalid,
                    Instr(Opcode.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),
                    Instr(Opcode.vdup, I16, q6, W22_12, D5_0, Ix(18, 4)),
                    Instr(Opcode.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),

                    Instr(Opcode.vdup, I32, q6, W22_12, D5_0, Ix(19, 1)),
                    Instr(Opcode.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),
                    Instr(Opcode.vdup, I16, q6, W22_12, D5_0, Ix(18, 4)),
                    Instr(Opcode.vdup, I8, q6, W22_12, D5_0, Ix(17, 3))),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var AdvancedSimd2RegsOr3RegsDiffLength = Mask(12 + 16, 1, "Advanced SIMD two registers, or three registers of different lengths",
                Mask(4 + 16, 2,
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Instr(Opcode.vext, I8, q6, W22_12, W7_16, W5_0, Imm(8, 4))), 
                Mask(4 + 16, 2,
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(6, 1,
                        AdvancedSimd3DiffLength,
                        AdvancedSimd2RegsScalar),
                    Mask(10, 2,
                        AdvancedSimd2RegsMisc,
                        AdvancedSimd2RegsMisc,
                        Mask(6, 1,
                            Instr(Opcode.vtbl, I8, D22_12, mrsimdD_1((7, 16), (8, 2)), D5_0),
                            Instr(Opcode.vtbx, I8, D22_12, mrsimdD_1((7, 16), (8, 2)), D5_0)),
                        AdvancedSimdDuplicateScalar)));

            var AdvancedSimdTwoScalarsAndExtension = Nyi("AdvancedSimdTwoScalarsAndExtension");

            var vmov_t1_d = Instr(Opcode.vmov, I32, D22_12, MS_28_16_0);
            var vmov_t1_q = Instr(Opcode.vmov, I32, Q22_12, MS_28_16_0);
            var vmvn_t1_d = Instr(Opcode.vmvn, I32, D22_12, MS_28_16_0);
            var vmvn_t1_q = Instr(Opcode.vmvn, I32, Q22_12, MS_28_16_0);

            var AdvancedSimdOneRegisterAndModifiedImmediate = Mask(8, 4, "AdvancedSimdOneRegisterAndModifiedImmediate",
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),

                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),
                Mask(6, 1, // Q
                    Mask(5, 1, vmov_t1_d, vmvn_t1_d),
                    Mask(5, 1, vmov_t1_q, vmvn_t1_q)),

                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T3")),
                    Instr(Opcode.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Opcode.vbic, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Opcode.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vorr, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Opcode.vbic, nyi("*immediate - T2"))),

                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    Instr(Opcode.vmov, nyi("*immediate - T5"))),
                Mask(5, 1,  // op
                    Instr(Opcode.vmov, nyi("*immediate - T4")),
                    invalid));

            var AdvancedSimdTwoRegistersAndShiftAmount = Mask(8, 4, "Advanced SIMD two registers and shift amount",
                Instr(Opcode.vshr, q6, VshImmSize, W22_12, W5_0, VshImm),
                Instr(Opcode.vsra, q6, VshImmSize, W22_12, W5_0, VshImm),
                Instr(Opcode.vrshr, q6, VshImmSizeSU7_16, W22_12, W5_0, VshImmRev),
                Instr(Opcode.vrsra, q6, VshImmSizeSU7_16, W22_12, W5_0, VshImmRev),

                Mask(12 + 16, 1, "  U", 
                    Nyi("U=0"),
                    Instr(Opcode.vsri, VshImmSize, q6, W22_12, W5_0, VshImmRev)), 
                Mask(12 + 16, 1,   // U
                    Instr(Opcode.vshl, VshImmSize, q6, W22_12, W5_0, VshImm),
                    Instr(Opcode.vsli, VshImmSize, q6, W22_12, W5_0, VshImm)),
                Mask(12 + 16, 1, "  u", 
                    invalid,
                    Mask(6, 1, // Q
                        Instr(Opcode.vqshlu, VshImmSizeSU7_16, D22_12, D5_0, VshImm),
                        Instr(Opcode.vqshlu, VshImmSizeSU7_16, Q22_12_times2, Q5_0_times2, VshImm))),
                Mask(6, 1, // Q
                    Instr(Opcode.vqshl, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Opcode.vqshl, VshImmSize, Q22_12_times2, Q5_0_times2, VshImm)),

                Mask(12 + 16, 1,     // U
                    Mask(12 + 16, 2,     // L:Q
                        Instr(Opcode.vshrn, nyi("*AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00"),
                        Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                    Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1")),
                Mask(7, 1, // opc= 9 L
                    Mask(6, 1, //  L= 0 Q
                        Instr(Opcode.vqshrn, VshImmSizeSU16_half, D22_12,Q5_0, VshImmRev),
                        Instr(Opcode.vqrshrn, nyi("D22_12,Q5_0,*signed result variant"))),   //$TODO hairy encoding.
                    invalid),
                Mask(7, 1, "  opc=1010 L",
                    Mask(6, 1, "  L=0 Q",
                        Instr(Opcode.vshll, calcVectorShiftAmount(16, 6), Q22_12, D5_0, readVectorShiftAmount),
                        invalid),
                    invalid), Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcB"),

                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcC"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcD"),
                Mask(7, 1, // L
                    Mask(6, 1, // Q
                        Instr(Opcode.vcvt, vC,D22_12,D5_0,Imm(minuend:64, fields:Bf((16,6)))),
                        Instr(Opcode.vcvt, vC,Q22_12,Q5_0,Imm(minuend:64, fields:Bf((16,6))))),
                    invalid),
                Mask(7, 1, // L
                    Mask(6, 1, // Q
                        Instr(Opcode.vcvt, vC,D22_12,D5_0,Imm(minuend:64, fields:Bf((16,6)))),
                        Instr(Opcode.vcvt, vC,Q22_12,Q5_0,Imm(minuend:64, fields:Bf((16,6))))),
                    invalid));


            var AdvancedSimdShiftImm = Select(Bf((19,3),(7,1)), n => n == 0, "Advanced SIMD shifts and immediate generation",
                AdvancedSimdOneRegisterAndModifiedImmediate,
                AdvancedSimdTwoRegistersAndShiftAmount);

            var AdvancedSimdDataProcessing = Mask(7 + 16, 1, "Advanced SIMD data-processing",
                AdvancedSimd3RegistersSameLength,
                Mask(4, 1,
                    AdvancedSimd2RegsOr3RegsDiffLength,
                    AdvancedSimdShiftImm));

            var AdvancedSimd2RegsScalarExt = Nyi("Advanced SIMD two registers and a scalar extension");

            var SystemRegisterAccessAdvSimdFpu = Mask(12 + 16, 1, "System register access, Advanced SIMD, and floating-point",
                Mask(8 + 16, 2, // op0 = 0
                    Mask(9, 3,  // op1 = 0b00
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AvancedSimdLdStAnd64bitMove,
                        AvancedSimdLdStAnd64bitMove,
                        invalid,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 3,  "  op1 = 0b01",
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AvancedSimdLdStAnd64bitMove,
                        AvancedSimdLdStAnd64bitMove,
                        invalid,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 3, "  op1 = 0b10",
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdAndFloatingPoint32bitMove),
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdAndFloatingPoint32bitMove),
                        invalid,
                        Mask(4, 1,
                            invalid,
                            SystemRegister32bitMove)),
                    AdvancedSimdDataProcessing), // op1 = 0b11
                Mask(8 + 16, 2, "  op0 = 1",
                    Mask(9, 3,  // op1 = 0b00
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AdvancedSimd3RegistersSameLengthExt,
                        invalid,
                        AdvancedSimd3RegistersSameLengthExt,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 3, "  op1 = 0b01",
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        AdvancedSimd3RegistersSameLengthExt,
                        invalid,
                        AdvancedSimd3RegistersSameLengthExt,
                        SystemRegisterLdStAnd64bitMove),
                    Mask(9, 3,  "  op1 = 0b10",
                        invalid,
                        invalid,
                        invalid,
                        invalid,
                        // 4
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            AdvancedSimdTwoScalarsAndExtension),
                        Mask(4, 1,
                            FloatingPointDataProcessing,
                            invalid),
                        AdvancedSimdTwoScalarsAndExtension,
                        Mask(4, 1,
                            invalid,
                            SystemRegister32bitMove)),
                    AdvancedSimdDataProcessing) // op1 = 0b11
                );

            var DataProcessing2srcRegs = Mask(4 + 16, 3, "Data-processing (two source registers)",
                Mask(4, 2,
                    Instr(Opcode.qadd, nyi("*")),
                    Instr(Opcode.qdadd, R8,R0,R16),
                    Instr(Opcode.qsub, nyi("*")),
                    Instr(Opcode.qdsub, R8,R0,R16)),
                Mask(4, 2,
                    Instr(Opcode.rev, wide, Rnp8, Rnp0),
                    Instr(Opcode.rev16, wide,Rnp8,Rnp0),
                    Instr(Opcode.rbit, nyi("*")),
                    Instr(Opcode.revsh, nyi("*"))),
                Mask(4, 2,
                    Instr(Opcode.sel, nyi("*")),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 2,
                    Instr(Opcode.clz, R8,R0),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 2,
                    Nyi("crc32-crc32b"),
                    Nyi("crc32-crc32h"),
                    Nyi("crc32-crc32w"),
                    invalid),
                Mask(4, 2,
                    Nyi("crc32c-crc32cb"),
                    Nyi("crc32c-crc32ch"),
                    Nyi("crc32c-crc32cw"),
                    invalid),
                invalid,
                invalid);

            var RegisterExtends = Mask(4 + 16, 3,
                Select_ne15(16,
                    Instr(Opcode.sxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxth, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Opcode.uxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxth, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Opcode.sxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxtb16, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Opcode.uxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxtb16, R8,R0,SrBy8_4_2)),

                Select_ne15(16,
                    Instr(Opcode.sxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.sxtb, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Opcode.uxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Opcode.uxtb, R8,R0,SrBy8_4_2)),
                invalid,
                invalid);

            var ParallelAddSub = Mask(4 + 16, 3,
                Mask(4, 3,
                    Instr(Opcode.sadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shadd8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhadd8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.sadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shadd16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhadd16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.sasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shasx, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.uasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqasx, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhasx, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid,

                Mask(4, 3,
                    Instr(Opcode.ssub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsub8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsub8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.ssub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsub16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsub16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Opcode.ssax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.qsax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.shsax, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Opcode.usax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uqsax, Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.uhsax, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid);

            var MovMovsRegisterShiftedRegister = Mask(20, 1,
                Mask(5 + 16, 2,
                    Instr(Opcode.lsl, R8,R16,R0),
                    Instr(Opcode.lsr, R8,R16,R0),
                    Instr(Opcode.asr, R8,R16,R0),
                    Instr(Opcode.ror, R8,R16,R0)),
                Mask(5 + 16, 2,
                    Instr(Opcode.lsl, uf,R8,R16,R0),
                    Instr(Opcode.lsr, uf,R8,R16,R0),
                    Instr(Opcode.asr, uf,R8,R16,R0),
                    Instr(Opcode.ror, uf,R8,R16,R0)));

            var DataProcessingRegister = Mask(7 + 16, 1, "Data-processing (register)",
                Mask(7, 1,  "  op1",
                    Select((4, 4), n => n == 0,
                        MovMovsRegisterShiftedRegister,
                        invalid),
                    RegisterExtends),
                Mask(6, 2,
                    ParallelAddSub,
                    ParallelAddSub,
                    DataProcessing2srcRegs,
                    invalid));

            var MultiplyAbsDifference = Mask(4 + 16, 3, "MultiplyAbsDifference",
                Mask(4, 2,
                    Select_ne15(12,
                        Instr(Opcode.mla, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.mul, Rnp8,Rnp16,Rnp0)),
                    Instr(Opcode.mls, Rnp8,Rnp16,Rnp0,Rnp12),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b001
                    Select_ne15(12,
                        Instr(Opcode.smlabb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smulbb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smlabt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smulbt, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smlatb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smultb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smlatt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smultt, Rnp8,Rnp16,Rnp0))),
                Mask(4, 2,      // op1 = 0b010
                    Select_ne15(12,
                        Instr(Opcode.smlad, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smuad, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smladx, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smuadx, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b011
                    Select_ne15(12,
                        Instr(Opcode.smlawb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smulwb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smlawt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smulwt, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),

                Mask(4, 2, "op1 = 0b100",
                    Select_ne15(12,
                        Instr(Opcode.smlsd, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smusd, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smlsdx, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smusdx, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b101
                    Select_ne15(12,
                        Instr(Opcode.smmla, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Opcode.smmul, Rnp8, Rnp16, Rnp0)),
                    Select_ne15(12,
                        Instr(Opcode.smmlar, Rnp8,Rnp16,Rnp0, Rnp12),
                        Instr(Opcode.smmulr, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b110
                    Instr(Opcode.smmls, Rnp8,Rnp16,Rnp0, Rnp12),
                    Instr(Opcode.smmlsr, Rnp8,Rnp16,Rnp0, Rnp12),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b111
                    Select_ne15(12,
                        Instr(Opcode.usada8, Rnp8,Rnp16,Rnp0, Rnp12),
                        Instr(Opcode.usad8, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid,
                    invalid));

            var MultiplyRegister = Select((6,2), n => n == 0,
                MultiplyAbsDifference,
                invalid);

            var LongMultiplyDivide = Mask(4 + 16, 3, "LongMultiplyDivide",
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.smull, Rnp12,Rnp8,Rnp16,Rnp0)),
                Select_ne15(4,
                    invalid,
                    Instr(Opcode.sdiv, Rnp8,Rnp16,Rnp0)),
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Opcode.umull, Rnp12,Rnp8,Rnp16,Rnp0)),
                Select_ne15(4,
                    invalid,
                    Instr(Opcode.udiv, Rnp8,Rnp16,Rnp0)),
                // 4
                Mask(4, 4,
                    Instr(Opcode.smlal, Rnp12, Rnp8, Rnp16, Rnp0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Opcode.smlalbb, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Opcode.smlalbt, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Opcode.smlaltb, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Opcode.smlaltt, Rnp12, Rnp8, Rnp16, Rnp0),

                    Instr(Opcode.smlald, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.smlaldx, Rnp12, Rnp8, Rnp16, Rnp0),
                    invalid,
                    invalid),
                Mask(4, 4, "LongMultiplyDivide op=5",
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

                    Instr(Opcode.smlsld, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Opcode.smlsldx, Rnp12,Rnp8,Rnp16,Rnp0),
                    invalid,
                    invalid),
                Mask(4, 4,   // op1 = 0b110
                    Instr(Opcode.umlal, Rnp12, Rnp8, Rnp16, Rnp0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Opcode.umaal, Rnp12,Rnp8,Rnp16,Rnp0),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid);   // op1 = 0b111

            var DataProcessingShiftedRegister = Mask(21, 4, "Data-processing (shifted register)",
                Mask(20, 1,
                    Instr(Opcode.and, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Select(Bf((12,3),(4,4)), n => n != 0b0011,
                        Select_ne15(8,
                            Instr(Opcode.and, uf,wide,Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Opcode.tst, wide,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2))))),
                        Select_ne15(8,
                            Nyi("ANDS, rotate right with extend variant on"),
                            Nyi("TST")))),
                Mask(20, 1,
                    Instr(Opcode.bic, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.bic, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                Mask(20, 1,
                    Select_ne15(16,
                        Instr(Opcode.orr, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mov, wide,R8,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select_ne15(16,
                        Instr(Opcode.orr, uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mov, uf,wide,R8,R0,Si((4,2),Bf((12,3),(6,2)))))),
                Mask(20, 1,
                    Select_ne15(16,
                        Instr(Opcode.orn, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mvn, R8,wide,R8,R0,Si((4, 2), Bf((12,3), (6, 2))))),
                    Select_ne15(16,
                        Instr(Opcode.orn, uf,Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.mvn, uf,wide,Rnp8,Rnp0,Si((4,2),Bf((12,3),(6,2)))))),

                Mask(20, 1,
                    Instr(Opcode.eor, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Select(Bf((12,3),(4,4)), n => n != 0b0011,
                        Select_ne15(8, "",
                            Instr(Opcode.eor, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Opcode.teq, uf,wide,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Select_ne15(8, "",
                            Instr(Opcode.eor, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Opcode.teq, nyi("rrx"))))),
                invalid,
                Mask(20, 1,
                    Mask(4, 2,
                        Instr(Opcode.pkhbt, Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        invalid,
                        Instr(Opcode.pkhtb, Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        invalid),
                    invalid),
                invalid,

                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Opcode.add, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.add, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select_ne15(8,
                        Select((16,4), n => n != 13,
                            Instr(Opcode.add, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Opcode.add, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Instr(Opcode.cmn, nyi("*register")))),
                invalid,
                Mask(20, 1,
                    Instr(Opcode.adc, wide,R8,R16,R0, Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.adc, wide,uf,R8,R16,R0, Si((4,2), Bf((12,3), (6,2))))),
                Mask(20, 1,
                    Instr(Opcode.sbc, R8,R16,R0, Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.sbc, uf,R8,R16,R0, Si((4,2), Bf((12,3), (6,2))))),

                invalid,
                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Opcode.sub, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Opcode.sub, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select((8,4), n => n != 15,
                        Select((16,4), n => n != 13,
                            Instr(Opcode.sub, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Opcode.sub, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Instr(Opcode.cmp, wide,R16,R0,Si((4,2),Bf((12,3),(6,2)))))),
                Mask(20, 1,
                    Instr(Opcode.rsb, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Opcode.rsb, uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                invalid);

            return new LongDecoder(new Decoder[16]
            {
                invalid,
                invalid,
                invalid,
                invalid,

                Mask(6+16, 1,
                    LdStMultiple,
                    loadStoreMultipleTableBranch),
                DataProcessingShiftedRegister,
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu,

                Mask(15, 1,
                    DataProcessingModifiedImmediate,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingPlainImm,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingModifiedImmediate,
                    branchesMiscControl),
                Mask(15, 1,
                    DataProcessingPlainImm,
                    branchesMiscControl),

                Select(Bf((24,1),(20,1)), n => n != 2,
                    LoadStoreSingle,
                    AdvancedSimdElementOrStructureLdSt),
                Mask(7 + 16, 2, "LongDecoder 23:2",
                    DataProcessingRegister,
                    DataProcessingRegister,
                    MultiplyRegister,
                    LongMultiplyDivide),
                SystemRegisterAccessAdvSimdFpu,
                SystemRegisterAccessAdvSimdFpu
            });
        }

        private static MaskDecoder<T32Disassembler, Opcode, AArch32Instruction> CreateLoadStoreDualMultipleBranchDecoder()
        {
            var ldrd = Instr(Opcode.ldrd, R12, R8, MemOff(PrimitiveType.Word64, baseReg: Registers.pc, offsetShift:2, offsetFields: (0, 8)));

            var LoadAcquireStoreRelease = Mask(20, 1,
                Mask(4, 3,
                    Instr(Opcode.stlb, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Opcode.stlh, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Opcode.stl, Rnp12, MemOff(PrimitiveType.Word32, 16)),
                    invalid,

                    Instr(Opcode.stlexb, R0,R12,MemOff(PrimitiveType.Byte,16)),
                    Instr(Opcode.stlexh, R0,R12,MemOff(PrimitiveType.Word16,16)),
                    Instr(Opcode.stlex, R0,R12,MemOff(PrimitiveType.Word32,16)),
                    Instr(Opcode.stlexd, R0,R12,MemOff(PrimitiveType.Word64,16))),
                Mask(4, 3,
                    Instr(Opcode.ldab, R0, R12, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Opcode.ldah, R0, R12, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Opcode.lda, R0, R12, MemOff(PrimitiveType.Word32, 16)),
                    invalid,

                    Instr(Opcode.ldaexb, nyi("*")),
                    Instr(Opcode.ldaexh, nyi("*")),
                    Instr(Opcode.ldaex, R12,MemOff(PrimitiveType.Word32,16)),
                    Instr(Opcode.ldaexd, nyi("*"))));

            var ldStExclusive = Mask(20, 1,
                Instr(Opcode.strex, R8,R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                Instr(Opcode.ldrex, R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))));

            var ldStDual = Mask(20, 1,
                Instr(Opcode.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Opcode.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var ldStDualImm = Mask(4 + 16, 1,
                Instr(Opcode.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Opcode.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));
            var ldStDualPre = Mask(20, 1,
                Instr(Opcode.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Opcode.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));

            var LdStExBHD = Mask(4 + 16, 1, 4, 2, "load/store exclusive byte/half/dual",
                Instr(Opcode.strexb, Rnp0, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                Instr(Opcode.strexh, Rnp0, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                invalid,
                Instr(Opcode.strexd, Rnp0, Rnp12, Rnp8, MemOff(PrimitiveType.Word64, 16)),

                Instr(Opcode.ldrexb, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                Instr(Opcode.ldrexh, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                invalid,
                Instr(Opcode.ldrexd, Rnp12, Rnp8, MemOff(PrimitiveType.Word64, 16)));

            return Mask(5 + 16, 4, "Load/store (multiple, dual, exclusive) table branch",
                invalid,
                invalid,
                ldStExclusive,
                Select_ne15(16, ldStDual, ldrd),

                invalid,
                invalid,
                Mask(5, 3, // op0 = 0b0110, op3 
                    Mask(20, 1,
                        invalid,
                        Mask(4, 1,
                            Instr(Opcode.tbb, MemIdx(PrimitiveType.Byte, 16, 0)),
                            Instr(Opcode.tbh, MemIdx(PrimitiveType.Word16, 16, 0)))),
                    invalid,
                    LdStExBHD,
                    LdStExBHD,
                    
                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease,
                    LoadAcquireStoreRelease),
                Select((16,4), n => n != 15, ldStDual,ldrd),

                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, ldStDualImm, ldrd),
                Select((16,4), n => n != 15, ldStDualPre, ldrd),

                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, invalid, ldrd),
                Select((16,4), n => n != 15, ldStDualImm, ldrd),
                Select((16,4), n => n != 15, ldStDualPre, ldrd));
        }

        private static Decoder CreateBranchesMiscControl()
        {
            var branch_T3_variant = Instr(Opcode.b, PcRelative(1, Bf((26,1),(11,1),(13,1),(16,6),(0,11))));
            var branch_T4_variant = Instr(Opcode.b, B_T4);
            var branch = Nyi("Branch");

            var MiscellaneousSystem = Mask(4, 4,
                invalid,
                invalid,
                Instr(Opcode.clrex, nyi("*")),
                invalid,

                Instr(Opcode.dsb, B0_4),
                Instr(Opcode.dmb, B0_4),
                Instr(Opcode.isb, B0_4),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);

            var Hints = Mask(4, 4,
                Mask(0, 4,
                    Instr(Opcode.nop, wide),
                    Instr(Opcode.yield, nyi("*")),
                    Instr(Opcode.wfe, nyi("*")),
                    Instr(Opcode.wfi, nyi("*")),

                    Instr(Opcode.sev, nyi("*")),
                    Instr(Opcode.sevl, nyi("*")),
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear)), // Reserved hint
                Select((0, 4), n => n != 0, 
                    Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Opcode.esb, nyi("*"))),
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Opcode.dbg, nyi("*")));

            var ExceptionGeneration = Mask(4 + 16, 1, 13, 1, "Exception generation",
                Instr(Opcode.hvc, nyi("*")),
                invalid,
                Instr(Opcode.smc, nyi("*")),
                Instr(Opcode.udf, wide, Imm(16, 4, 0, 12)));

            var mixedDecoders = Mask(6 + 16, 4,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,
                branch_T3_variant,

                branch_T3_variant,
                branch_T3_variant,
                Mask(26, 1,     // op0
                    Mask(20, 2,     // op2
                        Mask(5, 1,  // op5
                            Mask(20, 1, // write spsr
                                Instr(Opcode.msr, cpsr, R16),
                                Instr(Opcode.msr, spsr, R16)),
                            Instr(Opcode.msr, nyi("*banked register"))),
                        Mask(5, 1,  // op5
                            Instr(Opcode.msr, nyi("*register")),
                            Instr(Opcode.msr, nyi("*banked register"))),
                        Select((8, 3), n => n == 0,
                            Hints,
                            Nyi("ChangeProcessorState")),
                        MiscellaneousSystem),
                    Mask(20, 2,     // op2
                        Select((12, 7), n => n == 0,
                            Nyi("Dcps"),
                            invalid),
                        invalid,
                        invalid,
                        invalid)),
                Mask(26, 1,         // op0
                    Mask(20, 2,     // op2
                        Instr(Opcode.bxj, nyi("*")),
                        Nyi("ExceptionReturn"),
                        Mask(5, 1,  // op5
                            Mask(20, 1, // read spsr
                                Instr(Opcode.mrs, R8, cpsr),
                                Instr(Opcode.mrs, R8, spsr)),
                            Instr(Opcode.mrs, nyi("*banked register"))),
                        Mask(5, 1,  // op5
                            Instr(Opcode.mrs, nyi("*register")),
                            Instr(Opcode.mrs, nyi("*banked register")))),
                    Mask(21, 1,
                        invalid,
                        ExceptionGeneration)));

            var bl = new BlDecoder();
            return Mask(12, 3, "Branches and miscellaneous control",
                mixedDecoders,
                branch_T4_variant,
                mixedDecoders,
                branch_T4_variant,

                invalid,
                bl,
                invalid,
                bl);
        }
    }
}
