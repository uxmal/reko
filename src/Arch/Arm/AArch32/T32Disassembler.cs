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
using System.Linq;
using System.Numerics;
using static Reko.Arch.Arm.AArch32.ArmVectorData;

namespace Reko.Arch.Arm.AArch32
{
    using Decoder = Decoder<T32Disassembler, Mnemonic, AArch32Instruction>;


    /// <summary>
    /// Disassembles machine code in the ARM T32 encoding into 
    /// ARM32 instructions.
    /// </summary>
    public partial class T32Disassembler : DisassemblerBase<AArch32Instruction, Mnemonic>
    {
#pragma warning disable IDE1006 // Naming Styles

        private const uint ArmRegPC = 0xFu;

        private static readonly Decoder[] decoders;
        private static readonly Decoder invalid;

        private readonly EndianImageReader rdr;
        private readonly ThumbArchitecture arch;
        private Address addr;
        private int itState;
        private ArmCondition itCondition;
        private DasmState state;

        public T32Disassembler(ThumbArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.itState = 0;
            this.itCondition = ArmCondition.AL;
            this.state = null!;
        }

        public override AArch32Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out var wInstr))
                return null;
            this.state = new DasmState();
            var instr = decoders[wInstr >> 13].Decode(wInstr, this);
            instr.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            instr.InstructionClass |= instr.Condition != ArmCondition.AL ? InstrClass.Conditional : 0;
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            if (IsLastItInstruction())
            {
                // No more IT bits, reset condition back to normal.
                itCondition = ArmCondition.AL;
                itState = 0;
            }
            else if (itState != 0 && instr.Mnemonic != Mnemonic.it)
            {
                // We're still under the influence of the IT instruction.
                var bit = ((itState >> 4) ^ ((int) this.itCondition)) & 1;
                instr.Condition = (ArmCondition) ((int) this.itCondition ^ bit);
                itState <<= 1;
            }
            return instr;
        }

        public override AArch32Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return state.MakeInstruction();
        }

        private bool IsLastItInstruction()
        {
            return (itState & 0x1F) == 0x10;
        }

        private class DasmState
        {
            public Mnemonic mnemonic;
            public InstrClass iclass;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public ArmCondition cc = ArmCondition.AL;
            public bool updateFlags = false;
            public bool wide = false;
            public bool writeback = false;
            public Mnemonic shiftType = Mnemonic.Invalid;
            public MachineOperand? shiftValue = null;
            public ArmVectorData vectorData = ArmVectorData.INVALID;
            public bool useQ = false;
            public uint vectorShiftAmt = 0;

            public AArch32Instruction MakeInstruction()
            {
                return new T32Instruction
                {
                    Mnemonic = mnemonic,
                    InstructionClass = iclass,
                    Operands = ops.ToArray(),
                    Condition = cc,
                    SetFlags = updateFlags,
                    Wide = wide,
                    Writeback = writeback,
                    ShiftType = shiftType,
                    ShiftValue = shiftValue,
                    vector_data = vectorData,
                };
            }
        }

        private (Mnemonic, MachineOperand?) DecodeImmShift(uint wInstr, Bitfield bfType, Bitfield[] bfImm)
        {
            var type = bfType.Read(wInstr);
            var imm = Bitfield.ReadFields(bfImm, wInstr);
            switch (type)
            {
            case 0:
                if (imm != 0)
                    return (Mnemonic.lsl, Constant.UInt32(imm));
                else
                    return (Mnemonic.Invalid, null); 
            case 1: return (Mnemonic.lsr, Constant.UInt32(imm == 0 ? 32 : imm));
            case 2: return (Mnemonic.asr, Constant.UInt32(imm == 0 ? 32 : imm));
            case 3:
                if (imm == 0)
                    return (Mnemonic.rrx, Constant.UInt32(1));
                else
                    return (Mnemonic.ror, Constant.UInt32(imm));
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
                int op = SBitfield(wInstr, 5, 1);
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
            return Constant.Word64(imm64);
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
                d.state.ops.Add(c);
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
                d.state.ops.Add(c);
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

        private static MachineOperand? MakeBarrierOperand(uint n)
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

        public override AArch32Instruction CreateInvalidInstruction()
        {
            return new T32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        public override AArch32Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("ThumbDis", this.addr, this.rdr, message);
            return new T32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
                Operands = Array.Empty<MachineOperand>()
            };
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

        private RegisterStorage Coprocessor(uint wInstr, int bitPos)
        {
            var cp = Registers.Coprocessors[SBitfield(wInstr, bitPos, 4)];
            return cp;
        }

        private RegisterStorage CoprocessorRegister(uint wInstr, int bitPos)
        {
            var cr = Registers.CoprocessorRegisters[SBitfield(wInstr, bitPos, 4)];
            return cr;
        }

        private static int SBitfield(uint word, int offset, int size)
        {
            return ((int) word >> offset) & ((1 << size) - 1);
        }

        private static Decoder DecodeBfcBfi(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators)
        {
            return new BfcBfiDecoder(mnemonic, mutators);
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
        private static readonly Mutator<T32Disassembler> q21 = q(21);
        private static readonly Mutator<T32Disassembler> q28 = q(28);

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

        private static readonly Bitfield[] vifFields = {
            new Bitfield(10,1), new Bitfield(18, 2)
        };

        /// <summary>
        /// Set element size based on 3 bits (see <see cref="vifFields"/>.)
        /// </summary>
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
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                d.state.vectorData = sizes[field.Read(u)];
                return d.state.vectorData != INVALID;
            };
        }
        private static Mutator<T32Disassembler> vi(Bitfield[] bitfields, params ArmVectorData[] sizes)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadFields(bitfields, u);
                d.state.vectorData = sizes[n];
                return d.state.vectorData != INVALID;
            };
        }

        private static readonly Mutator<T32Disassembler> viBHW_ = vi(20, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> viHWD_ = vi(20, 2, I16, I32, I64, INVALID);
        private static readonly Mutator<T32Disassembler> vi20_HW_ = vi(20, 2, INVALID, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> viBHWD = vi(20, 2, I8, I16, I32, I64);
        private static readonly Mutator<T32Disassembler> viWH__ = vi(20, 2, I32, I16, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vi_BHW_chk = vi(20, 2, I8, I16, I32, INVALID);  //$REVIEW: not all of these are correct!

        private static readonly Mutator<T32Disassembler> vi18B___ = vi(18, 2, I8, INVALID, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vi18BH__ = vi(18, 2, I8, I16, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vi18BHW_ = vi(18, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> vi18BHWD = vi(18, 2, I8, I16, I32, I64);
        private static readonly Mutator<T32Disassembler> vu18B___ = vi(18, 2, U8, INVALID, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vu18BH__ = vi(18, 2, U8, U16, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vu18BHW_ = vi(18, 2, U8, U16, U32, INVALID);
        private static readonly Mutator<T32Disassembler> vu18BHWD = vi(18, 2, U8, U16, U32, U64);

        private static readonly Mutator<T32Disassembler> vf8_HSD = vi(8, 2, INVALID, F16, F32, F64);
        private static readonly Mutator<T32Disassembler> vf18_HS_ = vi(18, 2, INVALID, F16, F32, INVALID);
        private static readonly Mutator<T32Disassembler> vf20_HS_ = vi(20, 2, INVALID, F16, F32, INVALID);
        private static readonly Mutator<T32Disassembler> vf20SH__ = vi(20, 2, F32, F16, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vpB___ = vi(20, 2, P8, INVALID, INVALID, INVALID);
        private static readonly Mutator<T32Disassembler> vpB_D_ = vi(20, 2, P8, INVALID, P64, INVALID);

        private static readonly Mutator<T32Disassembler> vi6BHW_ = vi(6, 2, I8, I16, I32, INVALID);
        private static readonly Mutator<T32Disassembler> vi6BHWD = vi(6, 2, I8, I16, I32, I64);

        private static readonly Mutator<T32Disassembler> vi10BHW_ = vi(10, 2, I8, I16, I32, INVALID);
        
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
        private static readonly Mutator<T32Disassembler> viuBHW_BHW_ = viu(7, 18, S8, S16, S32, INVALID, I8, I16, I32, INVALID);

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

        private static readonly ArmVectorData[] signed_bhw_ = new[]
        {
            ArmVectorData.S8,
            ArmVectorData.S16,
            ArmVectorData.S32,
            ArmVectorData.INVALID,
        };

        private static readonly ArmVectorData[] unsigned_bhw_ = new[]
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

        private static readonly ArmVectorData[] signed_bhwd = new[]
        {
            ArmVectorData.S8,
            ArmVectorData.S16,
            ArmVectorData.S32,
            ArmVectorData.S64,
        };

        private static readonly ArmVectorData[] unsigned_bhwd = new[]
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
            dasm.state.ops.Add(Constant.Int32((int) dasm.state.vectorShiftAmt));
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

        private static readonly ArmVectorData[] _hw_ = new[]
        {
            INVALID,
            I16,
            I32,
            INVALID
        };

        private static readonly ArmVectorData[] whb_ = new[]
        {
            I32,
            I16,
            I8,
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
                d.state.ops.Add(Registers.GpRegs[iReg]);
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
                d.state.ops.Add(Registers.GpRegs[iReg]);
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
                d.state.ops.Add(Registers.GpRegs[iReg]);
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
            dasm.state.ops.Add(Registers.GpRegs[tReg]);
            return true;
        }

        private static Mutator<T32Disassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.state.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> sp = Reg(Registers.sp);
        private static readonly Mutator<T32Disassembler> cpsr = Reg(Registers.cpsr);
        private static readonly Mutator<T32Disassembler> spsr = Reg(Registers.spsr);

        /// <summary>
        /// Banked register
        /// </summary>
        private static Mutator<T32Disassembler> rb(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (A32Disassembler.bankedRegisters.TryGetValue(imm, out var reg))
                {
                    d.state.ops.Add(reg);
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }

        /// SIMD / FP system registers
        private static Mutator<T32Disassembler> SIMDSysReg(int bitoffset)
        {
            var field = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = simdSysRegisters[iReg];
                if (reg is null)
                    return false;
                d.state.ops.Add(reg);
                return true;
            };
        }

        private static readonly RegisterStorage?[] simdSysRegisters = new[]
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
                    d.state.ops.Add(Registers.GpRegs[imm]);
                    d.state.ops.Add(Registers.GpRegs[imm + 1]);
                    return true;
                }
            };
        }
        private static readonly Mutator<T32Disassembler> Rp_0 = rp(0);
        private static readonly Mutator<T32Disassembler> Rp_12 = rp(12);

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
                d.state.ops.Add(Registers.SRegs[iReg]);
                d.state.ops.Add(Registers.SRegs[iReg + 1]);
                return true;
            };
        }

        private static Mutator<T32Disassembler> D(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(Registers.DRegs[iReg]);
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
                regMask <<= iStartReg;
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
            dasm.state.ops.Add(Registers.SRegs[d]);
            return true;
        }

        private static bool D22_12(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(Registers.DRegs[d]);
            return true;
        }

        private static bool Q22_12(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 18) & 0x10) | ((wInstr >> 12) & 0xF);
            dasm.state.ops.Add(Registers.QRegs[q >> 1]);
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
                d.state.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> FP0 = FP(5, 0);
        private static readonly Mutator<T32Disassembler> FP12 = FP(22, 12);
        private static readonly Mutator<T32Disassembler> FP16 = FP(7, 16);


        /// <summary>
        /// Vector register, whose size is set by a previous "q"  mutator.
        /// </summary>
        private static Mutator<T32Disassembler> W(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.state.ops.Add(Registers.DRegs[iReg]);
                return true;
            };
        }

        /// <summary>
        /// Vector register, whose size is set by a previous "q" mutator
        /// to either 'S' or 'D'.
        /// </summary>
        private static Mutator<T32Disassembler> V(int pos1, int size1, int pos2, int size2)
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
                    if ((imm & 1) == 1) // Odd-numbered Qx regsiters are illegal.
                    {
                        return false;
                    }
                    else
                    {
                        d.state.ops.Add(Registers.QRegs[imm >> 1]);
                    }
                }
                else
                {
                    d.state.ops.Add(Registers.DRegs[imm]);
                }
                return true;
            };
        }
        private readonly static Mutator<T32Disassembler> V5_0 = V(5, 1, 0, 4);
        private readonly static Mutator<T32Disassembler> V7_16 = V(7, 1, 16, 4);
        private readonly static Mutator<T32Disassembler> V22_12 = V(22, 1, 12, 4);


        /// <summary>
        /// Vector register, whose size is set by a previous "q" mutator.
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
                    if ((imm & 1) == 1) // Odd-numbered Qx regsiters are illegal.
                    {
                        return false;
                    }
                    else
                    {
                        d.state.ops.Add(Registers.QRegs[imm >> 1]);
                    }
                }
                else
                {
                    d.state.ops.Add(Registers.DRegs[imm]);
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
            dasm.state.ops.Add(Registers.QRegs[q >> 1]);
            return true;
        }


        private static bool F16_7(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr >> 15) & 0x1E) | ((wInstr >> 7) & 0x1);
            dasm.state.ops.Add(Registers.SRegs[s]);
            return true;
        }

        private static bool D7_16(uint wInstr, T32Disassembler dasm)
        {
            var d = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(Registers.DRegs[d]);
            return true;
        }

        private static bool Q7_16(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 3) & 0x10) | ((wInstr >> 16) & 0xF);
            dasm.state.ops.Add(Registers.QRegs[q >> 1]);
            return true;
        }


        private static bool F0_5(uint wInstr, T32Disassembler dasm)
        {
            var s = ((wInstr & 0xF) << 1) | ((wInstr >> 0x5) & 1);
            dasm.state.ops.Add(Registers.SRegs[s]);
            return true;
        }

        private static bool D5_0(uint wInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(Registers.DRegs[
                ((wInstr >> 1) & 0x10) | (wInstr & 0xF)]);
            return true;
        }

        private static bool Q5_0(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(Registers.QRegs[q >> 1]);
            return true;
        }

        private static bool Q5_0_times2(uint wInstr, T32Disassembler dasm)
        {
            var q = ((wInstr >> 1) & 0x10) | (wInstr & 0xF);
            dasm.state.ops.Add(Registers.QRegs[q >> 1]);
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
                //    op = new RegisterStorage(cp);
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
                d.state.ops.Add(cp);
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
                d.state.ops.Add(Constant.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Simm(int pos, int length, int shift = 0)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var imm = bitfield.ReadSigned(u) << shift;
                d.state.ops.Add(Constant.Int32(imm));
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
                d.state.ops.Add(Constant.Word32(imm));
                return true;
            };
        }

        private static Mutator<T32Disassembler> Imm(PrimitiveType? dt = null, uint minuend = 0, params Bitfield[] fields)
        {
            var dataType = dt ?? PrimitiveType.Word32;
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                if (minuend != 0)
                {
                    imm = minuend - imm;
                }
                d.state.ops.Add(Constant.Create(dataType, imm));
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> Imm26_12_0 = Imm(fields: Bf((26, 1), (12, 3), (0, 8)));

        private static Mutator<T32Disassembler> Imm(Constant c)
        {
            return (u, d) =>
            {
                d.state.ops.Add(c);
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
                d.state.ops.Add(Constant.Word32(n + 1));
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
            dasm.state.ops.Add(zero);
            return true;
        }

        private static Mutator<T32Disassembler> Ibit(int pos)
        {
            return (u, d) =>
            {
                var value = Bits.IsBitSet(u, pos);
                d.state.ops.Add(Constant.Bool(value));
                return true;
            };
        }

        /// <summary>
        /// Signed integer
        /// </summary>
        private static Mutator<T32Disassembler> S(int pos, int len)
        {
            var bf = new Bitfield(pos, len);
            return (u, d) =>
            {
                d.state.ops.Add(Constant.Int32((int)bf.Read(u)));
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
                d.state.shiftType = n != 0 ? Mnemonic.ror : Mnemonic.Invalid;
                d.state.shiftValue = Constant.Int32(n * 8);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> SrBy8_4_2 = SrBy8(4, 2);

        private static Mutator<T32Disassembler> ShiftImm(Mnemonic opc, int pos1, int length1, int pos2, int length2)
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
                    d.state.shiftValue = Constant.Int32((int) imm);
                }
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> LslImm = ShiftImm(Mnemonic.lsl, 12, 3, 6, 2);
        private static readonly Mutator<T32Disassembler> AsrImm = ShiftImm(Mnemonic.asr, 12, 3, 6, 2);

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
                op = Constant.Word32(abcdefgh);
                break;
            case 2:
            case 3:
                op = Constant.Word32((abcdefgh << 16) | abcdefgh);
                break;
            case 4:
            case 5:
                op = Constant.Word32((abcdefgh << 24) | (abcdefgh << 8));
                break;
            case 6:
            case 7:
                op = Constant.Word32(
                    (abcdefgh << 24) |
                    (abcdefgh << 16) |
                    (abcdefgh << 8) |
                    (abcdefgh));
                break;
            default:
                abcdefgh |= 0x80;
                op = Constant.Word32(abcdefgh << (int) (0x20 - i_imm3_a));
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
                d.state.ops.Add(Constant.Word64(A32Disassembler.SimdExpandImm(op, cmode, (uint) imm)));
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
                var op = d.addr.Align(4) + offset;
                d.state.ops.Add(op);
                return true;
            };
        }

        private static Mutator<T32Disassembler> PcRelative(int shift = 0, params Bitfield[] fields)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) << shift;
                var op = d.addr + (offset + 4);
                d.state.ops.Add(op);
                return true;
            };
        }

        // Jump displacement in bits 9:3..7, shifted left by 1.
        private static bool x(uint wInstr, T32Disassembler dasm)
        {
            var offset = (SBitfield(wInstr, 9, 1) << 6) |
                         (SBitfield(wInstr, 3, 5) << 1);
            dasm.state.ops.Add(dasm.addr + (offset + 4));
            return true;
        }

        /// <summary>
        /// Vector immediate quantity.
        /// </summary>
        private static bool IW0(uint uInstr, T32Disassembler dasm)
        {
            dasm.state.ops.Add(dasm.state.useQ
                ? new BigConstant(PrimitiveType.Word128, BigInteger.Zero)
                : Constant.Word64(0));
            return true;
        }

        private static readonly (ArmVectorData, uint)[] vectorImmediateShiftSize = new[]
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
            dasm.state.ops.Add(Constant.Int32((int) imm));
            return true;
        }

        private static bool VshImmRev(uint wInstr, T32Disassembler dasm)
        {
            var imm6 = (wInstr >> 16) & 0b111111;
            var immL_6 = ((wInstr >> 1) & 0x40) | imm6;
            var imm = vectorRevImmediateShiftSize[immL_6 >> 3].Item2 - imm6;
            dasm.state.ops.Add(Constant.Int32((int) imm));
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
                var rLast = (RegisterStorage) d.state.ops[iLastOp];
                var dtElem = Arm32Architecture.VectorElementDataType(d.state.vectorData);
                var ixOp = new IndexedOperand(dtElem, rLast, imm);
                d.state.ops[iLastOp] = ixOp;
                return true;
            };
        }
        private static Mutator<T32Disassembler> Ix(int pos, int size) { return Ix((pos, size)); }


        // Memory access mutators

        private static Mutator<T32Disassembler> MemOff(
            PrimitiveType dt,
            int baseRegBitoffset = 0,
            RegisterStorage? baseReg = null,
            int offsetShift = 0,
            IndexSpec? indexSpec = null,
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
                if (indexSpec is not null)
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
            RegisterStorage? baseReg = null,
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
                Mnemonic shiftType = Mnemonic.Invalid;
                if (field.HasValue)
                {
                    shiftAmt = (int)field.Value.Read(u);
                    shiftType = shiftAmt != 0 ? Mnemonic.lsl : Mnemonic.Invalid;
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
            var mop = new MemoryOperand(Arm32Architecture.VectorElementDataType(dasm.state.vectorData));
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

        private static readonly Bitfield[] B_T4_fields = new Bitfield[]
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
            var op = dasm.addr + (offset + 4);
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
                var b = MakeBarrierOperand(n);
                if (b is null)
                    return false;
                d.state.ops.Add(b);
                return true;
            };
        }
        private static readonly Mutator<T32Disassembler> B0_4 = B(0);

        /// <summary>
        /// If the current operand is the LR register, make the instruction
        /// a return instruction.
        /// </summary>
        private static bool useLr(uint uInstr, T32Disassembler dasm)
        {
            var reg = (RegisterStorage) dasm.state.ops[^1];
            if (reg == Registers.lr)
                dasm.state.iclass = InstrClass.Transfer | InstrClass.Return;
            return true;
        }

        /// <summary>
        /// Fail if the current instruction is inside an IT block.
        /// </summary>
        private static bool noIt(uint uInstr, T32Disassembler dasm)
        {
            return dasm.itState == 0;
        }

        /// <summary>
        /// Fail if the current instruction is inside an IT block,
        /// and isn't the last instruction.
        /// </summary>
        private static bool noItUnlessLast(uint uInstr, T32Disassembler dasm)
        {
            return dasm.itState == 0 || dasm.IsLastItInstruction();
        }

        private static Mutator<T32Disassembler> nyi(string message)
        {
            return (u, d) =>
            {
                d.NotYetImplemented($"Unimplemented '{message}' when decoding {u:X4}");
                return false;
            };
        }
        #endregion


        // Factory methods

        private static InstrDecoder Instr(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, iclass, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, ArmVectorData vec, params Mutator<T32Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, vec, mutators);
        }

        private static InstrDecoder Instr_v8_1(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators)
        {
            //$TODO: implement model-specific behavior
            return new InstrDecoder(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr_v8_2(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators)
        {
            //$TODO: implement model-specific behavior
            return new InstrDecoder(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr_v8_2(Mnemonic mnemonic, ArmVectorData vecdata, params Mutator<T32Disassembler>[] mutators)
        {
            //$TODO: implement model-specific behavior
            return new InstrDecoder(mnemonic, InstrClass.Linear, vecdata, mutators);
        }

        private static InstrDecoder Instr_v8_3(Mnemonic mnemonic, params Mutator<T32Disassembler>[] mutators)
        {
            //$TODO: implement model-specific behavior
            return new InstrDecoder(mnemonic, InstrClass.Linear, ArmVectorData.INVALID, mutators);
        }

        private static InstrDecoder Instr_v8_3(Mnemonic mnemonic, ArmVectorData vecdata, params Mutator<T32Disassembler>[] mutators)
        {
            //$TODO: implement model-specific behavior
            return new InstrDecoder(mnemonic, InstrClass.Linear, vecdata, mutators);
        }

        /// <summary>
        /// Select decoding depending on whether the 4 bit field value is 0xF or not.
        /// </summary>
        private static ConditionalDecoder<T32Disassembler, Mnemonic, AArch32Instruction> Select_ne15(
            int bitPos, 
            string tag, 
            Decoder<T32Disassembler, Mnemonic, AArch32Instruction> decoderNot15,
            Decoder<T32Disassembler, Mnemonic, AArch32Instruction> decoder15)
        {
            var fields = new[]
            {
                new Bitfield(bitPos, 4)
            };
            return new ConditionalDecoder<T32Disassembler, Mnemonic, AArch32Instruction>(fields, n => n != 15, tag, decoderNot15, decoder15);
        }

        private static ConditionalDecoder<T32Disassembler, Mnemonic, AArch32Instruction> Select_ne15(
           int bitPos,
           Decoder<T32Disassembler, Mnemonic, AArch32Instruction> decoderNot15,
           Decoder<T32Disassembler, Mnemonic, AArch32Instruction> decoder15)
        {
            return Select_ne15(bitPos, "", decoderNot15, decoder15);
        }

        private static NyiDecoder<T32Disassembler, Mnemonic, AArch32Instruction> Nyi(string msg)
        {
            return new NyiDecoder<T32Disassembler, Mnemonic, AArch32Instruction>(msg);
        }

        static T32Disassembler()
        {
            invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

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
                    Instr(Mnemonic.b, noItUnlessLast, PcRelative(1, Bf((0, 11)))),
                    dec32bit,
                    dec32bit,
                    dec32bit)
            };
        }

        private static MaskDecoder<T32Disassembler, Mnemonic, AArch32Instruction> Create16bitDecoders()
        {
            var AddSpRegisterT1 = Instr(Mnemonic.add, uf,T,sp);
            var AddSpRegisterT2 = Instr(Mnemonic.add, sp,T);
            var decAlu = CreateAluDecoder();
            var decDataLowRegisters = CreateDataLowRegisters();
            var decDataHiRegisters = Mask(8, 2, "Add, subtract, compare, move (two high registers)",
                Select(Bf((7,1),(0,3)), n => n != 13, 
                    Select((3,4), n => n != 13,
                        Instr(Mnemonic.add, uf,T,R3),
                        AddSpRegisterT1),
                    Select((3,4), n => n != 13,
                        AddSpRegisterT2, 
                        AddSpRegisterT1)),
                Instr(Mnemonic.cmp, uf,T,R3),
                Instr(Mnemonic.mov, T,R3), // mov,movs
                invalid);

            var LdrLiteral = Instr(Mnemonic.ldr,r8,MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetShift:2, offsetFields:(0,8)));

            var LdStRegOffset = Mask(9, 3, "LdStRegOffset",
                Instr(Mnemonic.str, r0,MemIdx_r(PrimitiveType.Word32,3,6)),
                Instr(Mnemonic.strh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Mnemonic.strb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Mnemonic.ldrsb, r0, MemIdx_r(PrimitiveType.SByte, 3, 6)),

                Instr(Mnemonic.ldr, r0, MemIdx_r(PrimitiveType.Word32, 3, 6)),
                Instr(Mnemonic.ldrh, r0, MemIdx_r(PrimitiveType.Word16, 3, 6)),
                Instr(Mnemonic.ldrb, r0, MemIdx_r(PrimitiveType.Byte, 3, 6)),
                Instr(Mnemonic.ldrsh, r0, MemIdx_r(PrimitiveType.Int16, 3, 6)));

            var decLdStWB = Nyi("LdStWB");
            var decLdStHalfword = Nyi("LdStHalfWord");
            var decLdStSpRelative = Nyi("LdStSpRelative");
            var decAddPcSp = Mask(11, 1,
                Instr(Mnemonic.adr, r8,P(0,8)),
                Instr(Mnemonic.add, r8,sp,Simm(0, 8, 2)));
            var decMisc16Bit = CreateMisc16bitDecoder();
            var decLdmStm = new LdmStmDecoder16();
            var decCondBranch = Mask(8, 4, "CondBranch",
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),

                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.b, noIt, c8, PcRelative(1, Bf((0, 8)))),
                Instr(Mnemonic.udf, Imm(0,8)),
                Instr(Mnemonic.svc, InstrClass.Transfer | InstrClass.Call, noIt, Imm(0, 8)));

            return Mask(13, 3, "  16-bit",
                decAlu,
                decAlu,
                Mask(10, 3,
                    decDataLowRegisters,
                    Mask(8, 2, // Special data and branch exchange 
                        decDataHiRegisters,
                        decDataHiRegisters,
                        decDataHiRegisters,
                        Mask(7,1,
                            Instr(Mnemonic.bx, InstrClass.Transfer, noItUnlessLast, R3, useLr),
                            Instr(Mnemonic.blx, InstrClass.Transfer|InstrClass.Call, noItUnlessLast, R3))),
                    LdrLiteral,
                    LdrLiteral,

                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset,
                    LdStRegOffset),
                Mask(11, 2,   // decLdStWB,
                    Instr(Mnemonic.str, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Mnemonic.ldr, r0, MemOff_r(PrimitiveType.Word32, 3, shift:2, fields: (6,5))),
                    Instr(Mnemonic.strb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5))),
                    Instr(Mnemonic.ldrb, r0, MemOff_r(PrimitiveType.Byte, 3, fields: (6,5)))),

                Mask(12, 0x01,
                    Mask(11, 0x01,
                        Instr(Mnemonic.strh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5))),
                        Instr(Mnemonic.ldrh, r0, MemOff_r(PrimitiveType.Word16, 3, shift:1, fields: (6,5)))),
                    Mask(11, 0x01,   // load store SP-relative
                        Instr(Mnemonic.str, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))),
                        Instr(Mnemonic.ldr, r8, MemOff_r(PrimitiveType.Word32, baseReg:Registers.sp, shift:2, fields: (0,8))))),
                Mask(12, 0x01,
                    decAddPcSp,
                    decMisc16Bit),
                Mask(12, 0x01,
                    decLdmStm,
                    decCondBranch),
                invalid);
        }

        private static Decoder CreateAluDecoder()
        {
            var decAddSub3 = Nyi("addsub3");
            var decAddSub3Imm = Nyi("AddSub3Imm");
            var decMovMovs = Mask(11, 2,
                Select((6,5), n => n != 0,
                    new MovMovsDecoder(Mnemonic.lsl, uf,r0,r3,S(6,5)),
                    Instr(Mnemonic.mov, r0,r3)),
                new MovMovsDecoder(Mnemonic.lsr, uf,r0,r3,S(6,5)),
                Instr(Mnemonic.asrs, uf,r0,r3,S(6,5)),
                invalid);
            var decAddSub = Mask(11, 2,
                Instr(Mnemonic.mov, r8,Imm(0,8)),
                Instr(Mnemonic.cmp, uf,r8,Imm(0,8)),
                Instr(Mnemonic.add, uf,r8,Imm(0,8)),
                Instr(Mnemonic.sub, uf,r8,Imm(0,8)));
            return Mask(10, 4,
                decMovMovs,
                decMovMovs,
                decMovMovs,
                decMovMovs,

                decMovMovs,
                decMovMovs,
                Mask(9, 1,
                    Instr(Mnemonic.add, r0,r3,r6),
                    Instr(Mnemonic.sub, r0,r3,r6)),
                Mask(9, 1,
                    Instr(Mnemonic.add, r0,r3,Imm(6,3)),
                    Instr(Mnemonic.sub, r0,r3,Imm(6,3))),
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
            return Mask(6, 4, "  Data processing (two low registers)",
                Instr(Mnemonic.and, ufit, r0, r3),
                Instr(Mnemonic.eor, ufit, r0, r3),
                Instr(Mnemonic.lsl, ufit, r0, r3),
                Instr(Mnemonic.lsr, ufit, r0, r3),

                Instr(Mnemonic.asr, ufit, r0, r3),
                Instr(Mnemonic.adc, ufit, r0, r3),
                Instr(Mnemonic.sbc, ufit, r0, r3),
                Instr(Mnemonic.ror, ufit, r0, r3),

                Instr(Mnemonic.tst, r0, r3),
                Instr(Mnemonic.rsb, ufit, r0, r3),
                Instr(Mnemonic.cmp, uf, r0, r3),
                Instr(Mnemonic.cmn, uf, r0, r3),

                Instr(Mnemonic.orr, ufit, r0, r3),
                Instr(Mnemonic.mul, ufit, r0, r3),
                Instr(Mnemonic.bic, ufit, r0, r3),
                Instr(Mnemonic.mvn, ufit, r0, r3));
        }

        private static Decoder CreateMisc16bitDecoder()
        {
            var pushAndPop = Mask(11, 1,
                Instr(Mnemonic.push, mw),
                Instr(Mnemonic.pop, mr));

            var cbnzCbz = Mask(11, 1,
                Instr(Mnemonic.cbz, r0,x),
                Instr(Mnemonic.cbnz, r0,x));

            return Mask(8, 4,
                Mask(7, 1,  // Adjust SP
                    Instr(Mnemonic.add, sp,Simm(0,7, 2)),
                    Instr(Mnemonic.sub, sp,Simm(0,7, 2))),
                cbnzCbz,
                Mask(6, 2,
                    Instr(Mnemonic.sxth, r0,r3),
                    Instr(Mnemonic.sxtb, r0,r3),
                    Instr(Mnemonic.uxth, r0,r3),
                    Instr(Mnemonic.uxtb, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Mask(5, 3,
                    Instr(Mnemonic.setpan, Ibit(3)),
                    invalid,
                    Instr(Mnemonic.setend, E(3, 1)),
                    Instr(Mnemonic.cps, Imm(3, 1)),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                cbnzCbz,
                Mask(6, 2,
                    Instr(Mnemonic.rev, r0,r3),
                    Instr(Mnemonic.rev, r0,r3),
                    Instr(Mnemonic.hlt, InstrClass.Terminates),
                    Instr(Mnemonic.rev, r0,r3)),
                cbnzCbz,

                pushAndPop,
                pushAndPop,
                Instr(Mnemonic.bkpt),
                Select((0, 4), n => n == 0,
                    Mask(4, 4, // Hints
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.yield),
                        Instr(Mnemonic.wfe),
                        Instr(Mnemonic.wfi),

                        Instr(Mnemonic.sev),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hints, behaves as NOP.
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),

                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                        Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear)),
                    new ItDecoder()));
        }

        private static LongDecoder CreateLongDecoder()
        {
            var branchesMiscControl = CreateBranchesMiscControl();
            var loadStoreMultipleTableBranch = CreateLoadStoreDualMultipleBranchDecoder();

            var LdStMultiple = Mask(7 + 16, 2,
                Mask(4 + 16, 1,
                    Instr(Mnemonic.srsdb, w21,sp,Imm(0,5)),
                    Instr(Mnemonic.rfedb, InstrClass.Transfer|InstrClass.Return, w21,R16)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Mnemonic.stm),
                    new LdmStmDecoder32(Mnemonic.ldm)),
                Mask(4 + 16, 1,
                    new LdmStmDecoder32(Mnemonic.stmdb),
                    new LdmStmDecoder32(Mnemonic.ldmdb)),
                Mask(4 + 16, 1,
                    Instr(Mnemonic.srsia, w21,sp,Imm(0,5)),
                    Instr(Mnemonic.rfeia, InstrClass.Transfer|InstrClass.Return, w21,R16)));

            var DataProcessingModifiedImmediate = Mask(4 + 16, 5,
                Instr(Mnemonic.and, R8,R16,M),
                Select_ne15(8,
                    Instr(Mnemonic.and, uf,R8,R16,M),
                    Instr(Mnemonic.tst, R16,M)),
                Instr(Mnemonic.bic, R8,R16,M),
                Instr(Mnemonic.bic, uf,R8,R16,M),
                // 4
                Select_ne15(16, 
                    Instr(Mnemonic.orr, R8,R16,M),
                    Instr(Mnemonic.mov, R8,M)),
                Select_ne15(16, 
                    Instr(Mnemonic.orr, uf,R8,R16,M),
                    Instr(Mnemonic.mov, uf,R8,M)),
                Select_ne15(16,
                    Instr(Mnemonic.orn, R8,R16,M),
                    Instr(Mnemonic.mvn, R8,M)),
                Select_ne15(16,
                    Instr(Mnemonic.orn, uf,R8,R16,M),
                    Instr(Mnemonic.mvn, uf,R8,M)),
                // 8
                Instr(Mnemonic.eor, R8,R16,M),
                Select_ne15(8,
                    Instr(Mnemonic.eor, uf,R8,R16,M),
                    Instr(Mnemonic.teq, uf,R8,M)),
                invalid,
                invalid,
                // C
                invalid,
                invalid,
                invalid,
                invalid,
                // 10
                Select((16, 4), n => n != 0xD,
                    Instr(Mnemonic.add, R8,R16,M),
                    Instr(Mnemonic.add, R9,R16,M)), //$REVIEW: check this
                Select_ne15(8, 
                    Select((16, 4), n => n != 0xD,
                        Instr(Mnemonic.add, uf,R8,R16,M),
                        Instr(Mnemonic.add, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Mnemonic.cmn, R16,M)),
                invalid,
                invalid,
                // 14
                Instr(Mnemonic.adc, R8,R16,M),
                Instr(Mnemonic.adc, uf,R9,R16,M),
                Instr(Mnemonic.sbc, R8,R16,M),
                Instr(Mnemonic.sbc, uf,R9,R16,M),
                // 18
                invalid,
                invalid,
                Select((16, 4), n => n != 0xD,
                    Instr(Mnemonic.sub, R8,R16,M),
                    Instr(Mnemonic.sub, R9,R16,M)), //$REVIEW: check this
                Select_ne15(8, 
                    Select((16, 4), n => n != 0xD,
                        Instr(Mnemonic.sub, uf,R8,R16,M),
                        Instr(Mnemonic.sub, uf,R9,R16,M)), //$REVIEW: check this
                    Instr(Mnemonic.cmp, R16,M)),
                // 1C
                Instr(Mnemonic.rsb, R8,R16,M),
                Instr(Mnemonic.rsb, uf,R9,R16,M),
                invalid,
                invalid);

            var DataProcessingSimpleImm = Mask(7 + 16, 1, "Data-processing (simple immediate)",
                Mask(5 + 16, 1,
                    Select((16, 4), w => (w & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Mnemonic.add, R8, R16, Imm26_12_0),
                            Instr(Mnemonic.add, uf, R8, R16, Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Mnemonic.add, R8, R16, Imm26_12_0),
                            Instr(Mnemonic.add, R8, R16, Imm26_12_0))),
                    invalid),
                Mask(5 + 16, 1,
                    invalid,
                    Select((16, 4), w => (w & 0xD) != 0xD,
                        Mask(10 + 16, 1,
                            Instr(Mnemonic.sub, R8, R16, Imm26_12_0),
                            Instr(Mnemonic.sub, uf, R8, R16, Imm26_12_0)),
                        Mask(17, 1,
                            Instr(Mnemonic.sub, R8, R16, Imm26_12_0),
                            Instr(Mnemonic.sub, R8, R16, Imm26_12_0)))));

            var SaturateBitfield = Mask(5 + 16, 3, "Saturate, Bitfield",
                Instr(Mnemonic.ssat, Rnp8, ImmM1(0,5), Rnp16, LslImm),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Instr(Mnemonic.ssat, Rnp8, Imm(0, 5), Rnp16, AsrImm),
                    Instr(Mnemonic.ssat16, R8, Imm(0, 4), Rnp16)),
                Instr(Mnemonic.sbfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                Select_ne15(16,
                    DecodeBfcBfi(Mnemonic.bfi, R8,R16,Imm(12,3,6,2),Imm(0,5)),
                    DecodeBfcBfi(Mnemonic.bfc, R8, Imm(12, 3, 6, 2), Imm(0, 5))),
                // 4
                Instr(Mnemonic.usat, R8, ImmM1(0,5), R16, LslImm),
                Select(w => SBitfield(w, 12, 3) != 0 || SBitfield(w, 6, 2) != 0,
                    Instr(Mnemonic.ssat, Rnp8, Imm(0, 5), Rnp16, AsrImm),
                    Instr(Mnemonic.usat16, R8, Imm(0, 4), R16)),
                Instr(Mnemonic.ubfx, R8, R16,Imm(12,3,6,2), ImmM1(0, 5)),
                invalid);

            var MoveWide16BitImm = Mask(7 + 16, 1,
                Instr(Mnemonic.mov, R8,Imm(PrimitiveType.Word32, fields: Bf((16,4),(26,1),(12,3),(0,8)))),
                Instr(Mnemonic.movt, Rnp8,Imm(PrimitiveType.Word16, fields: Bf((16,4),(26,1),(12,3),(0,8)))));

            var DataProcessingPlainImm = Mask(8 + 16, 1, "Data processing (plain binary immediate)",
                Mask(5 + 16, 2,
                    DataProcessingSimpleImm,
                    DataProcessingSimpleImm,
                    MoveWide16BitImm,
                    invalid),
                SaturateBitfield);

            var LoadStoreSignedPositiveImm = Select_ne15(12,
                Mask(5 + 16, 2,
                    Instr(Mnemonic.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,12))),
                    Instr(Mnemonic.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0, 12))),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Instr(Mnemonic.pli, MemOff(PrimitiveType.SByte, 16, indexSpec: idx10, offsetFields: (0, 12))),
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));   // reserved hint

            var LoadStoreSignedImmediatePostIndexed = Mask(5 + 16, 2,
                Instr(Mnemonic.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, indexSpec:idx10, offsetFields:(0,8))),
                Instr(Mnemonic.ldrsh, R12,MemOff(PrimitiveType.Int16, 16, indexSpec:idx10, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadStoreSignedImmediatePreIndexed = Mask(5 + 16, 2,
                Instr(Mnemonic.ldrsb, wide, R12, MemOff(PrimitiveType.SByte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrsh, wide, R12, MemOff(PrimitiveType.Int16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreSignedUnprivileged = Mask(5 + 16, 2,
                Instr(Mnemonic.ldrsbt, R12, MemOff(PrimitiveType.SByte, 16, indexSpec: idx10, offsetFields: [(8, 4), (0, 4)])),
                Instr(Mnemonic.ldrsht, R12, MemOff(PrimitiveType.Int16, 16, indexSpec: idx10, offsetFields: [(8, 4), (0, 4)])),
                invalid,
                invalid);

            var LoadStoreSignedNegativeImm = Mask(5 + 16, 2,
                Select((12, 4), w=> w != 0xF,
                    Instr(Mnemonic.ldrsb, R12,MemOff(PrimitiveType.SByte, 16, offsetFields:(0,8))),
                    Instr(Mnemonic.pli, nyi("*"))),
                Select((12, 4), w=> w != 0xF,
                    Instr(Mnemonic.ldrsh, R12, MemOff(PrimitiveType.Int16, 16, offsetFields: (0,8))),
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear)),        // Reserved hint
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePostIndexed = Mask(4 + 16, 3,
                Instr(Mnemonic.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldr, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedPositiveImm = Mask(4 + 16, 3, "LoadStoreUnsignedPositiveImm",
                Instr(Mnemonic.strb, R12,MemOff(PrimitiveType.Byte, 16, offsetFields: (0,12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Mnemonic.ldrb, R12, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12))),
                    Instr(Mnemonic.pld, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12)))),
                Instr(Mnemonic.strh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                Select(w => SBitfield(w, 12, 4) != 0xF,
                    Instr(Mnemonic.ldrh, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                    Instr(Mnemonic.pldw, MemOff(PrimitiveType.Byte, 16, offsetFields: (0, 12)))),
                // 4
                Instr(Mnemonic.str, R12, MemOff(PrimitiveType.Word32, 16, offsetFields: (0, 12))),
                Instr(Mnemonic.ldr, R12, MemOff(PrimitiveType.Word16, 16, offsetFields: (0, 12))),
                invalid,
                invalid);

            var LoadStoreUnsignedImmediatePreIndexed = Mask(4 + 16, 3,
                Instr(Mnemonic.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.str, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedRegisterOffset = Mask(4 + 16, 3, "Load/store, unsigned (register Offset)",
                Instr(Mnemonic.strb, R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Mnemonic.ldrb, wide,R12,MemIdx(PrimitiveType.Byte,16,0,(4,2))),
                    Instr(Mnemonic.pld, nyi("*"))),
                Instr(Mnemonic.strh, R12, MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                Select((16, 4), n => n != 0xF,
                    Instr(Mnemonic.ldrh, wide, R12,MemIdx(PrimitiveType.Word16, 16, 0, (4, 2))),
                    Instr(Mnemonic.pld, nyi("*"))),
                Instr(Mnemonic.str, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                Instr(Mnemonic.ldr, wide, R12,MemIdx(PrimitiveType.Word32, 16, 0, (4, 2))),
                invalid,
                invalid);

            var LoadStoreUnsignedNegativeImm = Mask(4 + 16, 3,
                Instr(Mnemonic.strb, R12, MemOff(PrimitiveType.Byte, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Mnemonic.ldrb, wide,R12,MemOff(PrimitiveType.Byte,16,indexSpec:idx10, offsetFields:(0, 8))),
                    Instr(Mnemonic.pld, nyi("*"))),
                Instr(Mnemonic.strh, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Select((16, 4), n => n != 0xF,
                    Instr(Mnemonic.ldrh, wide, R12, MemOff(PrimitiveType.Word16, 16, indexSpec: idx10, offsetFields: (0, 8))),
                    Instr(Mnemonic.pld, nyi("*"))),
                Instr(Mnemonic.str, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                Instr(Mnemonic.ldr, wide, R12, MemOff(PrimitiveType.Word32, 16, indexSpec: idx10, offsetFields: (0, 8))),
                invalid,
                invalid);

            var LoadStoreUnsignedUnprivileged = Mask(4 + 16, 3,
                Instr(Mnemonic.strbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Mnemonic.ldrbt, R12, MemOff(PrimitiveType.Byte, 16, offsetFields:(0,8))),
                Instr(Mnemonic.strht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Mnemonic.ldrht, R12, MemOff(PrimitiveType.Word16, 16, offsetFields:(0,8))),
                Instr(Mnemonic.strt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                Instr(Mnemonic.ldrt, R12, MemOff(PrimitiveType.Word32, 16, offsetFields:(0,8))),
                invalid,
                invalid);

            var LoadUnsignedLiteral = Select_ne15(12, "Load unsigned (literal)",
                Mask(4 + 16, 3,
                    invalid,
                    Instr(Mnemonic.ldrb, R12, MemOff(PrimitiveType.Byte, baseReg:Registers.pc, offsetFields:(0,12))),
                    invalid,
                    Instr(Mnemonic.ldrh, R12, MemOff(PrimitiveType.Word16, baseReg: Registers.pc, offsetFields: (0, 12))),
                    
                    invalid,
                    Instr(Mnemonic.ldr, R12, MemOff(PrimitiveType.Word32, baseReg:Registers.pc, offsetFields:(0,12))),
                    invalid,
                    invalid),
                Mask(4 + 16, 3,
                    invalid,
                    Instr(Mnemonic.pld, MemOff(PrimitiveType.Word32, baseReg: Registers.pc, offsetFields: (0, 12))),
                    invalid,
                    Instr(Mnemonic.pld, MemOff(PrimitiveType.Word32, baseReg: Registers.pc, offsetFields: (0, 12))),
                    
                    invalid,
                    invalid,
                    invalid,
                    invalid));

            var LoadSignedLiteral = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 2,
                    Instr(Mnemonic.ldrsb, R12, MemOff(PrimitiveType.SByte, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    Instr(Mnemonic.ldrsh, R12, MemOff(PrimitiveType.Int16, baseReg: Registers.pc, offsetFields: new[] { (8, 4), (0, 4) })),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Instr(Mnemonic.pli, nyi("* literal")),
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
                    invalid,
                    invalid));

            var LoadStoreSignedRegisterOffset = Select((12,4), n => n != 0xF,
                Mask(5 + 16, 2, "Load/store, signed (register Offset)",
                    Instr(Mnemonic.ldrsb, wide, R12, MemIdx(PrimitiveType.SByte, 16, 0, (4, 2))),
                    Instr(Mnemonic.ldrsh, wide, R12, MemIdx(PrimitiveType.Int16, 16, 0, (4, 2))),
                    invalid,
                    invalid),
                Mask(5 + 16, 2,
                    Instr(Mnemonic.pli, nyi("*register")),
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear),
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
                            LoadStoreSignedImmediatePreIndexed,
                            LoadStoreSignedUnprivileged,
                            Nyi("LoadStoreSignedImmediatePreIndexed"))),
                    LoadSignedLiteral),
                Select_ne15(16, 
                    LoadStoreSignedPositiveImm,
                    LoadSignedLiteral));

            var ldc_literal = Nyi("LDC (literal)");
            var ldc_preindexed = Instr(Mnemonic.ldc, CPn14, CR12, MemOff(PrimitiveType.Word32, 16, offsetFields: (0, 8), indexSpec: idx24));
            var stc_preindexed = Instr(Mnemonic.stc, CPn14, CR12, MemOff(PrimitiveType.Word32, 16, offsetFields: (0, 8), indexSpec: idx24));
            
            var SystemRegisterLdSt = Select((8,1), n => n != 0, "SystemRegisterLdSt",
                invalid,
                Select((12,4), n => n != 5,
                    invalid,
                    Select((22,1), n => n != 0,
                        invalid,
                        Mask(Bf((23,2),(20,2)), "  PU-WL",
                            invalid,
                            invalid,
                            Instr(Mnemonic.stc, nyi("*post-indexed")),
                            Select((16,4), n => n != 15,
                                Instr(Mnemonic.ldc, nyi("*imm")),
                                ldc_literal),

                            Instr(Mnemonic.stc, nyi("*unindexed variant")),
                            Select((16,4), n => n != 15,
                                Instr(Mnemonic.ldc, nyi("*immediate - unindexed variant")),
                                ldc_literal),
                            invalid,
                            invalid, 

                            Instr(Mnemonic.stc, nyi("*Offset variant")),
                            Select((16,4), n => n != 15,
                                Instr(Mnemonic.ldc, nyi("*Offset variant")),
                                ldc_literal),
                            stc_preindexed,
                            Select((16,4), n => n != 15,
                                ldc_preindexed,
                                ldc_literal),

                            Instr(Mnemonic.stc, CPn14,CR12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                            Select((16,4), n => n != 15,
                                Instr(Mnemonic.ldc, nyi("*Offset variant")),
                                ldc_literal),
                            stc_preindexed,
                            Select((16,4), n => n != 15,
                                ldc_preindexed,
                                ldc_literal)))));


            var StoreCoprocessor = Mask(12 + 16, 1, "  store-nonPC",
                Instr(Mnemonic.stc, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Mnemonic.stc2, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));

            var LoadCoprocessor = Select_ne15(16, "",
                Mask(12 + 16, 1, "  load-nonPC",
                    Instr(Mnemonic.ldc, nyi("*")),
                    Mask(6 + 16, 1, "  ldc2{l}",
                        Instr(Mnemonic.ldc2, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                        Instr(Mnemonic.ldc2l, CP8, CR12, MemOff(PrimitiveType.Word32, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))))),
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
                    Instr(Mnemonic.mcrr, CP8, Imm(4, 4), Rnp12, Rnp16, CR0),
                    Instr(Mnemonic.mrrc, CP8, Imm(4, 4), Rnp12,Rnp16,CR0)),
                Coproc,
                Coproc);

            var SystemRegisterLdStAnd64bitMove = Select(Bf((23,2),(21,1)), n => (n & 0xD) == 0,
                SystemRegister64bitMove,
                SystemRegisterLdSt);

            var vstmia = Mask(8, 2, // size
                    invalid,
                    invalid,
                    Instr(Mnemonic.vstmia, nyi("*")),
                    Mask(0, 1,
                        Instr(Mnemonic.vstmia, nyi("*")),
                        Instr(Mnemonic.fstmiax, nyi("*"))));

            var vldmia = Mask(8, 2, "VLDMIA",
                    invalid,
                    invalid,
                    Instr(Mnemonic.vldmia, w(21), R16, mrsimdS((0, 8))),
                    Mask(0, 1,
                        Instr(Mnemonic.vldmia, w(21), R16, mrsimdD((1, 7))),
                        Instr(Mnemonic.fldmiax, nyi("*"))));
            var vstr = Mask(8, 2,  // size
                invalid,
                Instr(Mnemonic.vstr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Mnemonic.vstr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Mnemonic.vstr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var vldr = Select_ne15(16, "",
                Mask(8, 2,
                    invalid,
                    Instr(Mnemonic.vldr, F12_22,MemOff(PrimitiveType.Real16, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Mnemonic.vldr, F12_22,MemOff(PrimitiveType.Real32, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                    Instr(Mnemonic.vldr, D22_12,MemOff(PrimitiveType.Real64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8)))),
                Mask(8, 2, "  (literal)",
                    invalid,
                    Instr(Mnemonic.vldr, F12_22, MemOff(PrimitiveType.Real16, 16, offsetShift:1, offsetFields:(0,8))),
                    Instr(Mnemonic.vldr, F12_22, MemOff(PrimitiveType.Real16, 16, offsetShift:2, offsetFields:(0,8))),
                    Instr(Mnemonic.vldr, D22_12, MemOff(PrimitiveType.Real16, 16, offsetShift:2, offsetFields:(0,8)))));

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

                        Instr(Mnemonic.vmov, S_pair(0, 5), Rnp12, Rnp16),
                        Instr(Mnemonic.vmov, D5_0, Rnp12, Rnp16),

                        invalid,
                        invalid,
                        Instr(Mnemonic.vmov, Rnp12, Rnp16, S_pair(0, 5)),
                        Instr(Mnemonic.vmov, Rnp12, Rnp16, D5_0))));

            var AvancedSimdLdStAnd64bitMove = Select((5 + 16, 4), w => (w & 0b1101) == 0, "Advanced SIMD load/store and 64-bit move",
                AdvancedSimdAndFp64bitMove,
                AdvancedSimdAndFpLdSt);

            var FloatingPointDataProcessing2Regs = Mask(16, 4, "Floating-point data-processing (two registers)",
                    Mask(7, 1, " opc1:opc2=0000 o3",
                        Mask(8, 2, "  o3=0",
                            invalid,
                            invalid,
                            Instr(Mnemonic.vmov, F32, F12_22, F0_5),
                            Instr(Mnemonic.vmov, F64, D22_12, D5_0)),
                        Instr(Mnemonic.vabs, nyi("*"))),
                    Mask(7, 1, "  op1:opc2=0001 o3",
                        Instr(Mnemonic.vneg, vf8_HSD, FP12, FP0),
                        Instr(Mnemonic.vsqrt, vf8_HSD, FP12, FP0)),
                    Nyi("0010 - _HSD"),
                    Nyi("0011 - _HSD"),

                    Mask(7, 1, "  op1:opc2=0100 o3",
                        Instr(Mnemonic.vcmp, vf8_HSD, FP12, FP0),
                        Instr(Mnemonic.vcmpe, vf8_HSD, FP12, FP0)),
                    Mask(7, 1, "  op1:opc2=0101 o3",
                        Mask(8, 2,
                            invalid,
                            Instr(Mnemonic.vcmp, F16, FP12, Imm0_r32),
                            Instr(Mnemonic.vcmp, F32, FP12, Imm0_r32),
                            Instr(Mnemonic.vcmp, F64, FP12, Imm0_r64)),
                        Mask(8, 2,
                            invalid,
                            Instr(Mnemonic.vcmpe, F16, FP12, Imm0_r32),
                            Instr(Mnemonic.vcmpe, F32, FP12, Imm0_r32),
                            Instr(Mnemonic.vcmpe, F64, FP12, Imm0_r64))),
                    Mask(7, 1, "  op1:opc2=0110 o3",
                        Mask(8, 2, "   o3=0",
                            invalid,
                            Instr(Mnemonic.vrintr, F16, F12_22, F0_5),
                            Instr(Mnemonic.vrintr, F32, F12_22, F0_5),
                            Instr(Mnemonic.vrintr, F64, D22_12, D5_0)),
                        Mask(8, 2, "   o3=0",
                            invalid,
                            Instr(Mnemonic.vrintz, F16, F12_22, F0_5),
                            Instr(Mnemonic.vrintz, F32, F12_22, F0_5),
                            Instr(Mnemonic.vrintz, F64, D22_12, D5_0))),
                    Mask(7, 1, "  op1:opc2=0111 o3",
                        Mask(8, 2, "   o3=0",
                            invalid,
                            Instr(Mnemonic.vrintx, F16, F12_22, F0_5),
                            Instr(Mnemonic.vrintx, F32, F12_22, F0_5),
                            Instr(Mnemonic.vrintx, F64, D22_12, D5_0)),
                        Mask(8, 2,
                            invalid,
                            invalid,
                            Instr(Mnemonic.vcvt, F64F32, D22_12, F0_5),
                            Instr(Mnemonic.vcvt, F32F64, F12_22, D5_0))),

                    Mask(7, 3, "  op1:opc2=1000",
                        invalid,
                        invalid,
                        Instr(Mnemonic.vcvt, F16U32, F12_22, F0_5),
                        Instr(Mnemonic.vcvt, F16S32, F12_22, F0_5),

                        Instr(Mnemonic.vcvt, F32U32, F12_22, F0_5),
                        Instr(Mnemonic.vcvt, F32S32, F12_22, F0_5),
                        Instr(Mnemonic.vcvt, F64U32, D22_12, F0_5),
                        Instr(Mnemonic.vcvt, F64S32, D22_12, F0_5)),

                    Nyi("1001 - _HSD"),
                    Nyi("1010 - _HSD"),
                    Nyi("1011 - _HSD"),

                    Mask(7, 1, "  op1:opc2=1100",
                        Instr(Mnemonic.vcvtr, nyi("*")),
                        Mask(8, 2, 
                            invalid,
                            Instr(Mnemonic.vcvt, U32F16, F12_22, F0_5),
                            Instr(Mnemonic.vcvt, U32F32, F12_22, F0_5),
                            Instr(Mnemonic.vcvt, U32F64, F12_22, D5_0))),
                    Mask(7, 1, "  op1:opc2=1101",
                        Instr(Mnemonic.vcvtr, nyi("*")),
                        Mask(8, 2,
                            invalid,
                            Instr(Mnemonic.vcvt, S32F16, F12_22, F0_5),
                            Instr(Mnemonic.vcvt, S32F32, F12_22, F0_5),
                            Instr(Mnemonic.vcvt, S32F64, F12_22, D5_0))),
                    Nyi("1110 - _HSD"),
                    Nyi("1111 - _HSD")
                );

            var FloatingPointDataProcessing3Regs = Mask(Bf((7 + 16, 1), (4 + 16, 2), (6, 1)), "Floating-point data-processing (three registers)",
                    Instr(Mnemonic.vmla, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vmls, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vmls, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vmla, vf8_HSD, FP12, FP16, FP0),

                    Instr(Mnemonic.vmul, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vnmul, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vadd, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vsub, vf8_HSD, FP12, FP16, FP0),

                    Instr(Mnemonic.vdiv, vf8_HSD, FP12, FP16, FP0),
                    invalid,
                    Instr(Mnemonic.vfnms, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vfnma, vf8_HSD, FP12, FP16, FP0),

                    Instr(Mnemonic.vfma, vf8_HSD, FP12, FP16, FP0),
                    Instr(Mnemonic.vfms, vf8_HSD, FP12, FP16, FP0),
                    invalid,
                    invalid);

            var FloatingPointMoveImm = Mask(8, 2, "Floating-point move immediate on page F3-3152",
                invalid,
                Instr(Mnemonic.vmov, F16, FP12, vfpImm32(16, 4, 0, 4)),
                Instr(Mnemonic.vmov, F32, FP12, vfpImm32(16, 4, 0, 4)),
                Instr(Mnemonic.vmov, F64, FP12, vfpImm64(16, 4, 0, 4)));

            var FloatingPointConditionalSelect = Mask(20, 2, "Floating-point conditional select",
                Instr(Mnemonic.vseleq, vf8_HSD, FP12, FP16, FP0),
                Instr(Mnemonic.vselvs, vf8_HSD, FP12, FP16, FP0),
                Instr(Mnemonic.vselge, vf8_HSD, FP12, FP16, FP0),
                Instr(Mnemonic.vselgt, vf8_HSD, FP12, FP16, FP0));

            var FloatingPointMinNumMaxNum =
                Mask(6, 1,
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vmaxnm, F16, F12_22,F16_7,F0_5),
                        Instr(Mnemonic.vmaxnm, F32, F12_22,F16_7,F0_5),
                        Instr(Mnemonic.vmaxnm, F64, D22_12,D7_16,D5_0)),
                    Mask(8, 2,
                        invalid,
                        Instr(Mnemonic.vminnm, F16, F12_22,F16_7,F0_5),
                        Instr(Mnemonic.vminnm, F32, F12_22,F16_7,F0_5),
                        Instr(Mnemonic.vminnm, F64, D22_12,D7_16,D5_0)));

            var FloatingPointExtIns = Nyi("FloatingPointExtIns");
            var FloatingPointDirectedCvt2Int = Mask(16, 3, "Floating-point directed convert to integer",
                Instr(Mnemonic.vrinta, vf8_HSD, FP12, FP0),
                Instr(Mnemonic.vrintn, vf8_HSD, FP12, FP0),
                Instr(Mnemonic.vrintp, vf8_HSD, FP12, FP0),
                Instr(Mnemonic.vrintm, vf8_HSD, FP12, FP0),
                Instr(Mnemonic.vcvta, vif8_HSD, FP12, FP0),
                Instr(Mnemonic.vcvtn, vif8_HSD, FP12, FP0),
                Instr(Mnemonic.vcvtp, vif8_HSD, FP12, FP0),
                Instr(Mnemonic.vcvtm, vif8_HSD, FP12, FP0));


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
                Instr(Mnemonic.vst1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Mnemonic.vst2, nyi("single 2-element structure from one lane - T1")),
                Instr(Mnemonic.vst3, nyi("single 3-element structure from one lane - T1")),
                Instr(Mnemonic.vst4, vi10BHW_, DlistIdx4_2, MsingleElem),

                Instr(Mnemonic.vst1, nyi("single element from one lane - T2")),
                Instr(Mnemonic.vst2, vi10BHW_, DlistIdx2_5_1, MsingleElem),
                Instr(Mnemonic.vst3, vi10BHW_, DlistIdx3_5_1, MsingleElem),
                Instr(Mnemonic.vst4, nyi("single 4-element structure from one lane - T2")),

                Instr(Mnemonic.vst1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Mnemonic.vst2, vi10BHW_, DlistIdx2_5_1, MsingleElem),
                Instr(Mnemonic.vst3, vi10BHW_, DlistIdx3_4_1, MsingleElem),
                Instr(Mnemonic.vst4, vi10BHW_, DlistIdx4_2, MsingleElem),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.vld1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Mnemonic.vld2, nyi("single 2-element structure from one lane - T1")),
                Instr(Mnemonic.vld3, nyi("single 3-element structure from one lane - T1")),
                Instr(Mnemonic.vld4, vi10BHW_, DlistIdx4_2, MsingleElem),

                Instr(Mnemonic.vld1, nyi("single element from one lane - T2")),
                Instr(Mnemonic.vld2, nyi("single 2-element structure from one lane - T2")),
                Instr(Mnemonic.vld3, vi10BHW_, DlistIdx3_5_1, MsingleElem),
                Instr(Mnemonic.vld4, nyi("single 4-element structure from one lane - T2")),

                Instr(Mnemonic.vld1, vi10BHW_, DlistIdx1_7_1, MsingleElem),
                Instr(Mnemonic.vld2, nyi("single 2-element structure from one lane - T3")),
                Instr(Mnemonic.vld3, nyi("single 3-element structure from one lane - T3")),
                Instr(Mnemonic.vld4, vi10BHW_, DlistIdx4_2, MsingleElem),

                invalid,
                invalid,
                invalid,
                invalid);

            var vmov_gpr_to_scalar_8 = Instr(Mnemonic.vmov, I8, D7_16, Ix((5 + 16, 1), (5, 2)), Rnp12);
            var vmov_gpr_to_scalar_16 = Instr(Mnemonic.vmov, I16, D7_16, Ix((5 + 16, 1), (6, 1)), Rnp12);
            var vmov_gpr_to_scalar_32 = Instr(Mnemonic.vmov, I32, D7_16, Ix(5 + 16, 1), Rnp12);

            var vmov_gpr_to_scalar = Mask(5 + 16, 2, 5, 2, "  vmov (general-purpose register to scalar)",
                vmov_gpr_to_scalar_32,
                vmov_gpr_to_scalar_16,
                invalid,
                vmov_gpr_to_scalar_16,

                vmov_gpr_to_scalar_32,
                vmov_gpr_to_scalar_16,
                invalid,
                vmov_gpr_to_scalar_16,

                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8,

                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8,
                vmov_gpr_to_scalar_8);


            var vmov_scalar_to_gpr_8 = Instr(Mnemonic.vmov, I8, Rnp12, D7_16, Ix((5 + 16, 1), (5, 2)));
            var vmov_scalar_to_gpr_16 = Instr(Mnemonic.vmov, I16, Rnp12, D7_16, Ix((5 + 16, 1), (6, 1)));
            var vmov_scalar_to_gpr_32 = Instr(Mnemonic.vmov, I32, Rnp12, D7_16, Ix(5 + 16, 1));

            var vmov_scalar_to_gpr = Mask(5 + 16, 2, 5, 2, "  vmov (scalar to general-purpose register)",
                 vmov_scalar_to_gpr_32,
                 vmov_scalar_to_gpr_16,
                 invalid,
                 vmov_scalar_to_gpr_16,

                 vmov_scalar_to_gpr_32,
                 vmov_scalar_to_gpr_16,
                 invalid,
                 vmov_scalar_to_gpr_16,

                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8,

                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8,
                 vmov_scalar_to_gpr_8);

            var AdvancedSimd8_16_32_bitElementMove = Mask(7 + 16, 1, "  Advanced SIMD 8/16/32-bit element move/duplicate",
                Mask(4 + 16, 1, "  opc1=0xx",
                    vmov_gpr_to_scalar,
                    vmov_scalar_to_gpr),
                Mask(4 + 16, 1, 6, 1, "  opc1=1xx",
                    Instr(Mnemonic.vdup, q21, vi(Bf((22,1),(5,1)), whb_), W7_16, Rnp12),
                    invalid,
                    invalid,
                    invalid));

            var AdvancedSimdAndFloatingPoint32bitMove = Mask(8, 1, "Advanced SIMD and floating-point 32-bit move",
                Select((21,3), n => n == 0,
                    Mask(20, 1,
                        Instr(Mnemonic.vmov, F16_7, Rnp12),
                        Instr(Mnemonic.vmov, Rnp12, F16_7)),
                    Select((21,3), n => n == 7,
                        Mask(20, 1,
                            Instr(Mnemonic.vmsr, nyi("*")),
                            Select_ne15(12, "",
                                Instr(Mnemonic.vmrs, R12, SIMDSysReg(16)),
                                Instr(Mnemonic.vmrs, cpsr, SIMDSysReg(16)))), //$REVIEW: should be apsr
                        invalid)),
                AdvancedSimd8_16_32_bitElementMove);

            var AdvancedSimdLdStMultipleStructures = Mask(21, 1, "AdvancedSimdLdStMultipleStructures",
                Mask(8, 4, "  st?",
                    Instr(Mnemonic.vst4, vi6BHW_, Dlist4, Melem16Align),
                    Instr(Mnemonic.vst4, vi6BHW_, Dlist4_2, Melem16Align),
                    Instr(Mnemonic.vst1, nyi("*multiple single elements - T4")),
                    Instr(Mnemonic.vst2, vi6BHW_, Dlist4, Melem16Align),

                    Instr(Mnemonic.vst3, vi6BHW_, Dlist3, Melem16Align),
                    Instr(Mnemonic.vst3, vi6BHW_, Dlist3_2, Melem16Align),
                    Instr(Mnemonic.vst1, vi6BHWD, Dlist3, Melem16Align),
                    Instr(Mnemonic.vst1, vi6BHWD, Dlist1, Melem16Align),

                    Instr(Mnemonic.vst2, vi6BHW_, Dlist2, Melem16Align),
                    Instr(Mnemonic.vst2, vi6BHW_, Dlist2_2, Melem16Align),
                    Instr(Mnemonic.vst1, vi6BHWD, Dlist2, Melem16Align),
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                Mask(8, 4, "  ld?",
                    Instr(Mnemonic.vld4, vi6BHW_, Dlist4, Melem16Align),
                    Instr(Mnemonic.vld4, vi6BHW_, Dlist4_2, Melem16Align),
                    Instr(Mnemonic.vld1, vi6BHWD, Dlist4, Melem16Align),
                    Instr(Mnemonic.vld2, vi6BHW_, Dlist4, Melem16Align),

                    Instr(Mnemonic.vld3, vi6BHW_, Dlist3, Melem16Align),
                    Instr(Mnemonic.vld3, vi6BHW_, Dlist3_2, Melem16Align),
                    Instr(Mnemonic.vld1, vi6BHWD, Dlist3, Melem16Align),
                    Instr(Mnemonic.vld1, vi6BHWD, Dlist1, Melem16Align),

                    Instr(Mnemonic.vld2, vi6BHW_, Dlist2, Melem16Align),
                    Instr(Mnemonic.vld2, vi6BHW_, Dlist2_2, Melem16Align),
                    Instr(Mnemonic.vld1, vi6BHWD, Dlist2, Melem16Align),
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
                    Instr(Mnemonic.mcr, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3)),
                    Instr(Mnemonic.mrc, CP8,Imm(21,3),R12,CR16,CR0,Imm(5,3))),
                invalid);

            var AdvancedSimd3RegistersSameLength_opc3 = Mask(4, 1, "  opc=0011",
                Instr(Mnemonic.vcgt, viuBHWD, q6, W22_12, W7_16, W5_0),
                Instr(Mnemonic.vcge, viuBHWD, q6, W22_12, W7_16, W5_0));

            var VmulIntegerAndPolynomial = Mask(12 + 16, 1, "  vmul (integer and polynomial)",
                Mask(6, 1, // poly=0
                    Instr(Mnemonic.vmul, viBHW_, D22_12, D7_16, D5_0),
                    Instr(Mnemonic.vmul, viBHW_, Q22_12, Q7_16, Q5_0)),
                Mask(6, 1, // Q //$TODO: even regs only
                    Instr(Mnemonic.vmul, vpB___, D22_12, D7_16, D5_0),
                    Instr(Mnemonic.vmul, vpB___, Q22_12, Q7_16, Q5_0)));

            var AdvancedSimd3RegistersSameLength_opcB = Mask(12 + 16, 1, 4, 1, "  opc=1011",
                Instr(Mnemonic.vqdmulh, q6, vi20_HW_, W22_12, W7_16, W5_0),
                Instr(Mnemonic.vpadd, viBHW_, D22_12, D7_16, D5_0),
                Instr(Mnemonic.vqrdmulh, q6, vi20_HW_, W22_12, W7_16, W5_0),
                Instr_v8_1(Mnemonic.vqrdmlah, q6, vi20_HW_, W22_12, W7_16, W5_0));

            var AdvancedSimd3RegistersSameLength_opcC = Mask(4, 1, "  opc=1100",
                Mask(12 + 16, 1, 4 + 16, 2, "  op1=0",
                    Instr(Mnemonic.sha1c, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha1p, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha1m, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha1su0, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha256h, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha256h2, I32, Q22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.sha256su1, I32, Q22_12, Q7_16, Q5_0),
                    Nyi("*")),
                Mask(12 + 16, 1, "  op1=1",
                    Mask(5 + 16, 1, "  U=0",
                        Instr(Mnemonic.vfma, nyi("*")),
                        Instr(Mnemonic.vfms, nyi("*"))),
                    Instr_v8_1(Mnemonic.vqrdmlsh, q6, vi20_HW_, W22_12, W7_16, W5_0)));
            
            var AdvancedSimd3RegistersSameLength_opcE = Mask(12 + 16, 1, "  opc=1110",  // U
                Mask(21, 1, "  U=0",  // high-bit of size
                    Mask(4, 1, "  Sz=0x",
                        Instr(Mnemonic.vceq, q6, vf20SH__, W22_12, W7_16, W5_0),
                        Nyi("AdvancedSimd3RegistersSameLength_opcE U=0 Sz=0x op1=0")),
                    invalid),
                Mask(21, 1,  // high-bit of size
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Instr(Mnemonic.vcge, F32, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vcge, F16, Q22_12, Q7_16, Q5_0)),
                        Instr(Mnemonic.vacge, viWH__, q6, W22_12, W7_16, W5_0)),
                    Mask(4, 1, // op1
                        Mask(6, 1,    // Q
                            Instr(Mnemonic.vcgt, F32, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vcgt, F16, Q22_12, Q7_16, Q5_0)),
                        Nyi("AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1"))));

            var AdvancedSimd3RegistersSameLength = Mask(8, 4, "  Advanced SIMD three registers of the same length",
                Mask(4, 1, // o1
                    Mask(6, 1,
                        Instr(Mnemonic.vhadd, viuBHW_, D22_12, D7_16, D5_0),
                        Instr(Mnemonic.vhadd, viuBHW_, Q22_12, Q7_16, Q5_0)),
                    Mask(6, 1,
                        Instr(Mnemonic.vqadd, viuBHWD, D22_12, D7_16, D5_0),
                        Instr(Mnemonic.vqadd, viuBHWD, Q22_12, Q7_16, Q5_0))),
                Mask(12 + 16, 1,  // U
                    Mask(4, 1,      // o1
                        Instr(Mnemonic.vrhadd, viuBHW_, q6, W22_12, W7_16, W5_0),
                        Mask(4 + 16, 2, // size
                            Instr(Mnemonic.vand, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vbic, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vorr, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vorn, q6, W22_12, W7_16, W5_0))),
                    Mask(4, 1,      // o1),
                        Instr(Mnemonic.vrhadd, viuBHW_, q6, W22_12, W7_16, W5_0),
                        Mask(4 + 16, 2, // size
                            Instr(Mnemonic.veor, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vbsl, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vbit, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vbif, q6, W22_12, W7_16, W5_0)))),
                Mask(4, 1, // o1
                    Instr(Mnemonic.vhsub, viuBHW_, q6, W22_12, W7_16, W5_0),
                    Instr(Mnemonic.vqsub, viuBHWD, q6, W22_12, W7_16, W5_0)),

                AdvancedSimd3RegistersSameLength_opc3,

                Mask(4, 1, "  opc=4",
                    Instr(Mnemonic.vshl,  viuBHWD, q6, W22_12, W7_16, W5_0),
                    Instr(Mnemonic.vqshl, viuBHWD, q6, W22_12, W7_16, W5_0)),

                Mask(4, 1,
                    Mask(6, 1, // Q
                        Instr(Mnemonic.vrshl, vu_bhwd(20), D22_12, D5_0, D7_16),
                        Instr(Mnemonic.vrshl, vu_bhwd(20), Q22_12, Q5_0, Q7_16)),
                    Mask(6, 1, // Q
                        Instr(Mnemonic.vqrshl, vu_bhwd(20), D22_12, D7_16, D5_0),
                        Instr(Mnemonic.vqrshl, vu_bhwd(20), Q22_12, Q7_16, Q5_0))),
                Mask(4, 1,
                    Instr(Mnemonic.vmax, viuBHW_, q6, W22_12, W7_16, W5_0),
                    Instr(Mnemonic.vmin, viuBHW_, q6, W22_12, W7_16, W5_0)),
                Mask(4, 1,
                    Instr(Mnemonic.vabd, viuBHW_, q6, W22_12, W7_16, W5_0),
                    Instr(Mnemonic.vaba, viuBHW_, q6, W22_12, W7_16, W5_0)),

                Mask(12 + 16, 1,  // U
                    Mask(4, 1, // op1
                        Mask(6, 1, // Q
                            Instr(Mnemonic.vadd, viBHWD, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vadd, viBHWD, Q22_12, Q7_16, Q5_0)),
                        Mask(6, 1, // Q
                            Instr(Mnemonic.vtst, viBHWD, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vtst, viBHWD, Q22_12, Q7_16, Q5_0))),
                    Mask(4, 1, "opc=8 U=1 op1",
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Mnemonic.vsub, viBHWD, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vsub, viBHWD, Q22_12, Q7_16, Q5_0)),
                        Mask(6, 1, "opc=8 U=1 op1=0 Q",
                            Instr(Mnemonic.vceq, viBHWD, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vceq, viBHWD, Q22_12, Q7_16, Q5_0)))),
                // opc9
                Mask(12 + 16, 1, "  opc=1001", // U
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Mnemonic.vmla, viBHW_, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vmla, viBHW_, Q22_12, Q7_16, Q5_0)),
                        VmulIntegerAndPolynomial),
                    Mask(4, 1,      // op1
                        Mask(6, 1, // Q
                            Instr(Mnemonic.vmls, viu_HW__HW_, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vmls, viu_HW__HW_, Q22_12, Q7_16, Q5_0)),
                        VmulIntegerAndPolynomial)),
                Mask(6, 1, // Q
                    Mask(4, 1, // op1
                        Instr(Mnemonic.vpmax, viuBHW_, D22_12, D7_16, D5_0),
                        Instr(Mnemonic.vpmin, viuBHW_, D22_12, D7_16, D5_0)),
                    invalid),

                AdvancedSimd3RegistersSameLength_opcB,

                AdvancedSimd3RegistersSameLength_opcC,
                // opcD
                Mask(12 + 16, 1, "  opc=1101", // U
                    Mask(4, 1,      // op1
                        Mask(6, 1,      // Q
                            Mask(20, 2,  // size
                                Instr(Mnemonic.vadd, F32, D22_12, D7_16, D5_0),
                                Instr(Mnemonic.vadd, F16, D22_12, D7_16, D5_0),
                                Instr(Mnemonic.vsub, F32, D22_12, D7_16, D5_0),
                                Instr(Mnemonic.vsub, F16, D22_12, D7_16, D5_0)),
                            Mask(20, 2,  // size
                                Instr(Mnemonic.vadd, F32, Q22_12, Q7_16, Q5_0),
                                Instr(Mnemonic.vadd, F16, Q22_12, Q7_16, Q5_0),
                                Instr(Mnemonic.vsub, F32, Q22_12, Q7_16, Q5_0),
                                Instr(Mnemonic.vsub, F16, Q22_12, Q7_16, Q5_0))),
                        Mask(20, 2,  // high-bit of size
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmla (floating point)"),
                            Nyi("*vmls (floating point)"),
                            Nyi("*vmls (floating point)"))),
                    Mask(4, 1,      // op1
                        Mask(20, 2,  // size
                            Instr(Mnemonic.vpadd, F32, D22_12, D7_16, D5_0),
                            Instr(Mnemonic.vpadd, F16, D22_12, D7_16, D5_0),
                            Nyi("*vabd (floating point)"),
                            Nyi("*vabd (floating point)")),
                        Mask(21, 1,  // high-bit of size
                            Mask(6, 1,      // Q
                                Instr(Mnemonic.vmul, F32, D22_12, D7_16, D5_0),
                                Instr(Mnemonic.vmul, F16, Q22_12, Q7_16, Q5_0)),
                            invalid))),

                // opc = E
                AdvancedSimd3RegistersSameLength_opcE,
                // opc = F
                Mask(12 + 16, 1, "  opc=0b1111 U",
                    Mask(5 + 16, 1, 4, 1, "  size<1>:o1",
                        Mask(4 + 16, 1, "  sz",
                            Instr(Mnemonic.vmax, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vmax, F32, q6, W22_12, W7_16, W5_0)),
                        Instr(Mnemonic.vrecps, nyi("*")),
                        Mask(4 + 16, 1, "  sz",
                            Instr(Mnemonic.vmax, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vmax, F32, q6, W22_12, W7_16, W5_0)),
                        Mask(4 + 16, 1, "  sz",
                            Instr(Mnemonic.vrsqrts, F32, q6, W22_12, W7_16, W5_0),
                            Instr(Mnemonic.vrsqrts, F32, q6, W22_12, W7_16, W5_0))),
                    Mask(Bf((4 + 16, 2), (6, 1), (4, 1)), "  U=1",     // Q:op1
                         Instr(Mnemonic.vpmax, F32, D22_12, D7_16, D5_0),
                         Instr(Mnemonic.vpmax, F64, D22_12, D7_16, D5_0),
                         Instr(Mnemonic.vmaxnm, nyi("*")),
                         Instr(Mnemonic.vmaxnm, nyi("*")),

                         invalid,
                         invalid,
                         Instr(Mnemonic.vmaxnm, nyi("*")),
                         Instr(Mnemonic.vmaxnm, nyi("*")),

                         Instr(Mnemonic.vpmin, F32, D22_12, D7_16, D5_0),
                         Instr(Mnemonic.vpmin, F64, D22_12, D7_16, D5_0),
                         Instr(Mnemonic.vmaxnm, nyi("*")),
                         Instr(Mnemonic.vmaxnm, nyi("*")),

                         invalid,
                         invalid,
                         Instr(Mnemonic.vmaxnm, nyi("*")),
                         Instr(Mnemonic.vmaxnm, nyi("*")))));

            var vclt_imm0 = Instr(Mnemonic.vclt, q6, vif, W22_12, W5_0, IW0);

            var vcmla = Mask(16+4, 1, 
                Instr_v8_3(Mnemonic.vcmla, F16, q6, W22_12, W7_16, W5_0),
                Instr_v8_3(Mnemonic.vcmla, F32, q6, W22_12, W7_16, W5_0));

            var op3_op4_Q_U = Bf((10, 1), (8, 1), (6, 1), (4, 1));
            var op3_op4_U = Bf((10, 1), (8, 1), (4, 1));
            var op3_op4 = Bf((10, 1), (8, 1));
            var Q_U = Bf((6, 1), (4, 1));

            var AdvancedSimd3RegistersSameLengthExt = Mask(7 + 16, 2, 4 + 16, 2, "Advanced SIMD three registers of the same length extension",
                invalid,
                invalid,
                Mask(op3_op4, "  op1:op2=0b0010",
                    Mask(4, 1, "  op3:op4=00",
                        vcmla,
                        Instr_v8_2(Mnemonic.vfmlal, nyi("vector"))),
                    invalid,
                    invalid,
                    Mask(Q_U, "  op3:op4=11",
                        Instr_v8_2(Mnemonic.vsdot, nyi("vector - 64 SIMD")),
                        Instr_v8_2(Mnemonic.vudot, nyi("vector - 64 SIMD")),
                        Instr_v8_2(Mnemonic.vsdot, nyi("vector - 128 SIMD")),
                        Instr_v8_2(Mnemonic.vudot, nyi("vector - 128 SIMD")))),
                Select(op3_op4_U, n => n == 0, "op1:op2=0b0011",
                    vcmla, 
                    invalid),

                Nyi("op1:op2=0b0100"),
                Select(op3_op4_U, n => n == 0, "op1:op2=0b0101",
                    vcmla,
                    invalid),
                Select(op3_op4_U, n => n == 0b001, "op1:op2=0b0110",
                    Instr_v8_2(Mnemonic.vfmsl, F16, q6, W22_12,V7_16,V5_0),
                    invalid),
                Nyi("op1:op2=0b0111"),

                invalid,
                invalid,
                Select(op3_op4_U, n => n == 0, "  op1:op2=0b1011",
                    vcmla,
                    invalid),
                Select(op3_op4_U, n => n == 0, "  op1:op2=0b1011",
                    vcmla,
                    Coproc),

                Nyi("op1:op2=0b1100"),
                Nyi("op1:op2=0b1101"),
                Nyi("op1:op2=0b1110"),
                Select(op3_op4_U, u => u == 0, "  op1:op2=0b1111",
                    vcmla,
                    invalid));

            var AdvancedSimd2RegsMisc = Mask(16, 2, "Advanced SIMD two registers misc",
                Mask(7, 4,
                    Instr(Mnemonic.vrev64, vu18BHW_, q6, W22_12, W5_0),
                    Instr(Mnemonic.vrev32, vu18BH__, q6, W22_12, W5_0),
                    Instr(Mnemonic.vrev16, vu18B___, q6, W22_12, W5_0),
                    invalid,

                    Instr(Mnemonic.vpaddl, nyi("*")),
                    Instr(Mnemonic.vpaddl, nyi("*")),
                    Mask(6, 1,
                        Instr(Mnemonic.aese, nyi("*")),
                        Instr(Mnemonic.aesd, nyi("*"))),
                    Mask(6, 1,
                        Instr(Mnemonic.aesmc, nyi("*")),
                        Instr(Mnemonic.aesimc, nyi("*"))),

                    invalid, //$REVIEW VSWP looks odd.
                    Instr(Mnemonic.vclz, nyi("*")),
                    Instr(Mnemonic.vcnt, nyi("*")),
                    Instr(Mnemonic.vmvn, nyi("*reg")),

                    Instr(Mnemonic.vpadal, viuBHW_BHW_, D22_12, D5_0),
                    Instr(Mnemonic.vpadal, viuBHW_BHW_, D22_12, D5_0),
                    Instr(Mnemonic.vqabs, nyi("*")),
                    Instr(Mnemonic.vqneg, nyi("*"))),
                Mask(7, 4,
                    Instr(Mnemonic.vcgt, vsfBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Mnemonic.vcge, vsfBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Mnemonic.vceq, vifBHW__HS_, D22_12, D5_0, ImmV0),
                    Instr(Mnemonic.vcle, vsfBHW__HS_, D22_12, D5_0, ImmV0),

                    vclt_imm0,
                    Mask(6, 1,
                        invalid,
                        Instr(Mnemonic.sha1h, I32, Q22_12, Q5_0)),
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Mnemonic.vabs, vi18BHWD, D22_12,D5_0),
                            Instr(Mnemonic.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Mnemonic.vabs, vi18BHWD, Q22_12,Q5_0),
                            Instr(Mnemonic.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Mnemonic.vneg, nyi("*")),

                    Instr(Mnemonic.vcgt, nyi("*imm0")),
                    Instr(Mnemonic.vcge, nyi("*imm0")),
                    Instr(Mnemonic.vceq, nyi("*imm0")),
                    Instr(Mnemonic.vcle, nyi("*imm0")),

                    vclt_imm0,
                    invalid,
                    Mask(6, 1,
                        Mask(10, 1,
                            Instr(Mnemonic.vabs, vi18BHWD, D22_12,D5_0),
                            Instr(Mnemonic.vabs, vr(18),D22_12,D5_0)),
                        Mask(10, 1,
                            Instr(Mnemonic.vabs, vi18BHWD, Q22_12,Q5_0),
                            Instr(Mnemonic.vabs, vr(18),Q22_12,Q5_0))),
                    Instr(Mnemonic.vqneg, nyi("*"))),
                Mask(7, 4,
                    invalid,
                    Instr(Mnemonic.vtrn, vi18BHW_, q6,W22_12, W5_0),
                    Instr(Mnemonic.vuzp, nyi("*")),
                    Instr(Mnemonic.vzip, nyi("*")),

                    Mask(6, 1,
                        Instr(Mnemonic.vmovn, nyi("*")),
                        Instr(Mnemonic.vqmovn, nyi("*unsigned"))),
                    Instr(Mnemonic.vqmovn, nyi("*signed")),
                    Mask(6, 1,
                        Instr(Mnemonic.vshll, nyi("*")),
                        invalid),
                    Mask(6, 1,
                        Instr(Mnemonic.sha1su1, nyi("*")),
                        Instr(Mnemonic.sha256su0, nyi("*"))),

                    Instr(Mnemonic.vrintn, nyi("*")),
                    Instr(Mnemonic.vrintx, nyi("*")),
                    Instr(Mnemonic.vrinta, nyi("*")),
                    Instr(Mnemonic.vrintz, nyi("*")),

                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,D22_12,D5_0),
                        invalid),
                    Instr(Mnemonic.vrintm, nyi("*")),
                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,Q22_12,Q5_0),
                        invalid),
                    Instr(Mnemonic.vrintp, q6, vf18_HS_, W22_12,W5_0)),
                Mask(4 + 16, 4,
                    Instr(Mnemonic.vcvta, nyi("*")),
                    Instr(Mnemonic.vcvta, nyi("*")),
                    Instr(Mnemonic.vcvtn, nyi("*")),
                    Instr(Mnemonic.vcvtn, nyi("*")),

                    Instr(Mnemonic.vcvtp, nyi("*")),
                    Instr(Mnemonic.vcvtp, nyi("*")),
                    Instr(Mnemonic.vcvtm, nyi("*")),
                    Instr(Mnemonic.vcvtm, nyi("*")),

                    Instr(Mnemonic.vrecpe, nyi("*")),
                    Instr(Mnemonic.vrsqrte, nyi("*")),
                    Instr(Mnemonic.vrecpe, nyi("*")),
                    Instr(Mnemonic.vrsqrte, nyi("*")),

                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,D22_12,D5_0),
                        Instr(Mnemonic.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,D22_12,D5_0),
                        Instr(Mnemonic.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,D22_12,D5_0),
                        Instr(Mnemonic.vcvt, vc,Q22_12,Q5_0)),
                    Mask(6, 1,
                        Instr(Mnemonic.vcvt, vc,D22_12,D5_0),
                        Instr(Mnemonic.vcvt, vc,Q22_12,Q5_0))));

            var AdvancedSimd3DiffLength = Mask(8, 4, "Advanced SIMD three registers of different lengths",
                Instr(Mnemonic.vaddl, viuBHW_, Q22_12, D7_16, D5_0),
                Instr(Mnemonic.vaddw, viuBHW_, Q22_12, Q7_16, D5_0),
                Instr(Mnemonic.vsubl, viuBHW_, Q22_12, D7_16, D5_0),
                Instr(Mnemonic.vsubw, viuBHW_, Q22_12, Q7_16, D5_0),

                Mask(12 + 16, 1,
                    Instr(Mnemonic.vaddhn, viHWD_, D22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.vraddhn, viHWD_, D22_12, Q7_16, Q5_0)),
                Instr(Mnemonic.vabal, viuBHW_, Q22_12, D7_16, D5_0),
                Mask(12 + 16, 1,
                    Instr(Mnemonic.vsubhn, viHWD_, D22_12, Q7_16, Q5_0),
                    Instr(Mnemonic.vrsubhn, viHWD_, D22_12, Q7_16, Q5_0)),
                Instr(Mnemonic.vabdl, viuBHW_, Q22_12,D7_16,D5_0),

                Instr(Mnemonic.vmlal, viuBHW_, Q22_12,D7_16,D5_0),
                Mask(12 + 16, 1,
                    Instr(Mnemonic.vqdmlal, vi20_HW_, Q22_12, D7_16, D5_0),
                    invalid),
                Instr(Mnemonic.vmlsl, viuBHW_, Q22_12, D7_16, D5_0),
                Mask(12 + 16, 1,
                    Instr(Mnemonic.vqdmlsl, vi20_HW_, Q22_12, D7_16, D5_0),
                    invalid),

                Instr(Mnemonic.vmull, viuBHW_, Q22_12, D7_16, D5_0),   //$TODO: polynomial?
                Mask(12 + 16, 1,
                    Instr(Mnemonic.vqdmull, vi20_HW_,  Q22_12, D7_16, D5_0),
                    invalid),
                Instr(Mnemonic.vmull, vpB_D_, Q22_12, D7_16, D5_0),   //$TODO: polynomial?
                invalid);

            var AdvancedSimd2RegsScalar = Mask(8, 4, "Advanced SIMD two registers and a scalar",
                Instr(Mnemonic.vmla, vi20_HW_, q28, W22_12,W7_16,W5_0),
                Instr(Mnemonic.vmla, vf20_HS_, q28, W22_12,W7_16,W5_0),
                Instr(Mnemonic.vmlal, nyi("*scalar")),
                Mask(12 + 16, 1, // Q
                    Instr(Mnemonic.vqdmlal, nyi("*")),
                    invalid),

                Mask(20, 2, "  vmls (scalar)",
                    invalid,
                    Instr(Mnemonic.vmls, vi20_HW_, q28, W22_12, W7_16, W(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Mnemonic.vmls, vi20_HW_, q28, W22_12, W7_16, W(0, 4), Ix(5, 1)),
                    invalid),
                Mask(20, 2, "  vmls (scalar)",
                    invalid,
                    Instr(Mnemonic.vmls, vi20_HW_, q28, W22_12, W7_16, W(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Mnemonic.vmls, vi20_HW_, q28, W22_12, W7_16, W(0, 4), Ix(5, 1)),
                    invalid),
                Mask(20, 2, "  vmlsl (scalar)", 
                    invalid,
                    Instr(Mnemonic.vmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Mnemonic.vmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 4), Ix(5, 1)),
                    invalid),
                Mask(12 + 16, 1, // Q
                    Mask(20, 2, "  vqdmlsl (scalar)",
                        invalid,
                        Instr(Mnemonic.vqdmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 3), Ix((5, 1), (3, 1))),
                        Instr(Mnemonic.vqdmlsl, viu_HW__HW_, Q22_12, D7_16, D(0, 4), Ix(5, 1)),
                        invalid),
                    invalid),

                Instr(Mnemonic.vmul, nyi("*scalar")),
                Instr(Mnemonic.vmul, nyi("*scalar")),
                Mask(20, 2, "  vmull (scalar)",
                    invalid,
                    Instr(Mnemonic.vmull, viu_HW__HW_, Q22_12, D7_16, D(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Mnemonic.vmull, viu_HW__HW_, Q22_12, D7_16, D(0, 4), Ix(5, 1)),
                    invalid),
                Mask(12 + 16, 1, // Q
                    Instr(Mnemonic.vqdmull, nyi("*")),
                    invalid),

                Mask(20, 2, "  vqdmulh (scalar)",
                    invalid,
                    Instr(Mnemonic.vqdmulh, S16, q28, W22_12, W7_16, D(0, 3), Ix((5, 1), (3, 1))),
                    Instr(Mnemonic.vqdmulh, S32, q28, W22_12, W7_16, D(0, 3), Ix(5, 1)),
                    invalid),

                Instr(Mnemonic.vqrdmlah, nyi("*")),
                Instr(Mnemonic.vqrdmlah, nyi("*")),
                Instr(Mnemonic.vqrdmlsh, nyi("*")));

            var AdvancedSimdDuplicateScalar = Mask(7, 3, "Advanced SIMD duplicate (scalar)",
                Mask(16, 3, "VDUP (scalar)",
                    invalid,
                    Instr(Mnemonic.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),
                    Instr(Mnemonic.vdup, I16, q6, W22_12, D5_0, Ix(18, 4)),
                    Instr(Mnemonic.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),

                    Instr(Mnemonic.vdup, I32, q6, W22_12, D5_0, Ix(19, 1)),
                    Instr(Mnemonic.vdup, I8, q6, W22_12, D5_0, Ix(17, 3)),
                    Instr(Mnemonic.vdup, I16, q6, W22_12, D5_0, Ix(18, 4)),
                    Instr(Mnemonic.vdup, I8, q6, W22_12, D5_0, Ix(17, 3))),
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
                    Instr(Mnemonic.vext, I8, q6, W22_12, W7_16, W5_0, Imm(8, 4))), 
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
                            Instr(Mnemonic.vtbl, I8, D22_12, mrsimdD_1((7, 16), (8, 2)), D5_0),
                            Instr(Mnemonic.vtbx, I8, D22_12, mrsimdD_1((7, 16), (8, 2)), D5_0)),
                        AdvancedSimdDuplicateScalar)));

            var reserved = invalid;
            var vcmla_by_element_f16 = Instr_v8_2(Mnemonic.vcmla, F16, q6, W22_12, W7_16, W5_0, Ix(5, 1));
            var vcmla_by_element_f32 = Instr_v8_2(Mnemonic.vcmla, F32, q6, W22_12, W7_16, W5_0, Ix(10, 1)); // Hack to force a 0 index
            var vfmal_by_scalar = Instr_v8_2(Mnemonic.vfmal, F16, q6, W22_12, V7_16, V5_0);
            var vfmsl_by_scalar = Instr_v8_2(Mnemonic.vfmsl, F16, q6, W22_12, V7_16, V5_0);
            var vsdot_by_element = Nyi("vsdot by element");
            var vudot_by_element = Nyi("vudot by element");

            var AdvancedSimdTwoScalarsAndExtension = Mask(7 + 16, 1, "  Advanced SIMD two registers and a scalar extension",
                Mask(Bf((20, 2), (10, 1), (8, 1), (4, 1)), "  op1=0",
                    vcmla_by_element_f16,
                    vfmal_by_scalar,
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=00010"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=00011"),
                    reserved,
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=00101"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=00110"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=00111"),

                    vcmla_by_element_f16,
                    vfmal_by_scalar,
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01010"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01011"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01100"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01101"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01110"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=01111"),

                    vcmla_by_element_f16,
                    reserved,
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10010"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10011"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10100"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10101"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10110"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=10111"),

                    vcmla_by_element_f16,
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11001"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11010"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11011"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11100"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11101"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11110"),
                    Nyi("  AdvancedSimdTwoScalarsAndExtension op1=0 op2:op3:op4:U=11111")),
                Mask(Bf((10, 1), (8, 1), (6, 1), (4, 1)), "  op1=1",
                    vcmla_by_element_f32,
                    reserved,
                    vcmla_by_element_f32,
                    reserved,

                    reserved,
                    reserved,
                    reserved,
                    reserved,

                    reserved,
                    reserved,
                    reserved,
                    reserved,

                    reserved,
                    reserved,
                    reserved,
                    reserved));

            var vmov_t1 = Instr(Mnemonic.vmov, I32, q6, W22_12, MS_28_16_0);
            var vmvn_t1 = Instr(Mnemonic.vmvn, I32, q6, W22_12, MS_28_16_0);
            var vorr_v1 = Instr(Mnemonic.vorr, I32, q6, W22_12, W22_12, MS_28_16_0);
            var vbic_t1 = Instr(Mnemonic.vbic, I32, q6, W22_12, W22_12, MS_28_16_0);

            var AdvancedSimdOneRegisterAndModifiedImmediate = Mask(8, 4, "Advanced SIMD one register and modified immediate",
                Mask(5, 1, vmov_t1, vmvn_t1),
                Mask(5, 1, vorr_v1, vbic_t1),
                Mask(5, 1, vmov_t1, vmvn_t1),
                Mask(5, 1, vorr_v1, vbic_t1),

                Mask(5, 1, vmov_t1, vmvn_t1),
                Mask(5, 1, vorr_v1, vbic_t1),
                Mask(5, 1, vmov_t1, vmvn_t1),
                Mask(5, 1, vorr_v1, vbic_t1),

                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, nyi("*immediate - T3")),
                    Instr(Mnemonic.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vorr, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Mnemonic.vbic, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Mnemonic.vmvn, nyi("*immediate - T2"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vorr, I16, q6, W22_12, Is(28,1,16,3,0,4)),
                    Instr(Mnemonic.vbic, nyi("*immediate - T2"))),

                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, nyi("*immediate - T4")),
                    Instr(Mnemonic.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, I32, q6, W22_12, Is(28, 1, 16, 3, 0, 4)),
                    Instr(Mnemonic.vmvn, nyi("*immediate - T3"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, nyi("*immediate - T4")),
                    Instr(Mnemonic.vmov, nyi("*immediate - T5"))),
                Mask(5, 1,  // op
                    Instr(Mnemonic.vmov, nyi("*immediate - T4")),
                    invalid));

            var AdvancedSimdTwoRegistersAndShiftAmount = Mask(8, 4, "Advanced SIMD two registers and shift amount",
                Instr(Mnemonic.vshr, q6, VshImmSize, W22_12, W5_0, VshImm),
                Instr(Mnemonic.vsra, q6, VshImmSize, W22_12, W5_0, VshImm),
                Instr(Mnemonic.vrshr, q6, VshImmSizeSU7_16, W22_12, W5_0, VshImmRev),
                Instr(Mnemonic.vrsra, q6, VshImmSizeSU7_16, W22_12, W5_0, VshImmRev),

                Mask(12 + 16, 1, "  U", 
                    Nyi("U=0"),
                    Instr(Mnemonic.vsri, VshImmSize, q6, W22_12, W5_0, VshImmRev)), 
                Mask(12 + 16, 1,   // U
                    Instr(Mnemonic.vshl, VshImmSize, q6, W22_12, W5_0, VshImm),
                    Instr(Mnemonic.vsli, VshImmSize, q6, W22_12, W5_0, VshImm)),
                Mask(12 + 16, 1, "  u", 
                    invalid,
                    Mask(6, 1, // Q
                        Instr(Mnemonic.vqshlu, VshImmSizeSU7_16, D22_12, D5_0, VshImm),
                        Instr(Mnemonic.vqshlu, VshImmSizeSU7_16, Q22_12_times2, Q5_0_times2, VshImm))),
                Mask(6, 1, // Q
                    Instr(Mnemonic.vqshl, VshImmSize, D22_12, D5_0, VshImm),
                    Instr(Mnemonic.vqshl, VshImmSize, Q22_12_times2, Q5_0_times2, VshImm)),

                Mask(12 + 16, 1, "  opc=8",    // U
                    Mask(6, 2,     // L:Q
                        Instr(Mnemonic.vshrn, VshImmSizeSU7_16, D22_12, Q5_0_times2, nyi("*AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00")),
                        Instr(Mnemonic.vrshrn, nyi("*AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=01")),
                        invalid,
                        invalid),
                    Mask(6, 2, "  U=1", // L:Q
                        Instr(Mnemonic.vqshrun, VshImmSizeSU7_16, D22_12, Q5_0_times2, VshImm),
                        Instr(Mnemonic.vqrshrun, VshImmSizeSU7_16, D22_12, Q5_0_times2, VshImm),
                        invalid,
                        invalid)),
                Mask(7, 1, // opc= 9 L
                    Mask(6, 1, //  L= 0 Q
                        Instr(Mnemonic.vqshrn, VshImmSizeSU16_half, D22_12,Q5_0, VshImmRev),
                        Instr(Mnemonic.vqrshrn, VshImmSizeSU16_half, D22_12, Q5_0, VshImmRev)),
                    invalid),
                Mask(7, 1, "  opc=1010 L",
                    Mask(6, 1, "  L=0 Q",
                        Instr(Mnemonic.vshll, calcVectorShiftAmount(16, 6), Q22_12, D5_0, readVectorShiftAmount),
                        invalid),
                    invalid),
                invalid,

                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcC"),
                Nyi("AdvancedSimdTwoRegistersAndShiftAmount_opcD"),
                Mask(7, 1, // L
                    Instr(Mnemonic.vcvt, vC, q6, W22_12,W5_0,Imm(minuend:64, fields:Bf((16,6)))),
                    invalid),
                Mask(7, 1, // L
                    Instr(Mnemonic.vcvt, vC,W22_12,W5_0,Imm(minuend:64, fields:Bf((16,6)))),
                    invalid));


            var AdvancedSimdShiftImm = Select(Bf((19,3),(7,1)), n => n == 0, "Advanced SIMD shifts and immediate generation",
                AdvancedSimdOneRegisterAndModifiedImmediate,
                AdvancedSimdTwoRegistersAndShiftAmount);

            var AdvancedSimdDataProcessing = Mask(7 + 16, 1, "Advanced SIMD data-processing",
                AdvancedSimd3RegistersSameLength,
                Mask(4, 1,
                    AdvancedSimd2RegsOr3RegsDiffLength,
                    AdvancedSimdShiftImm));

            var AdvancedSimd2RegsScalarExt = Mask(7 + 16, 1, "  Advanced SIMD two registers and a scalar extension",
                Nyi("op1=0"),
                Select(Bf((10,1),(8,1),(4,1)), u => u == 0, "  op1=1",
                    Nyi("VCMLA - v8.3"),
                    invalid));

            var SystemRegisterAccessAdvSimdFpu = Mask(12 + 16, 1, "  System register access, Advanced SIMD, and floating-point",
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
                    Instr(Mnemonic.qadd, R8, R0, R16),
                    Instr(Mnemonic.qdadd, R8,R0,R16),
                    Instr(Mnemonic.qsub, R8, R0, R16),
                    Instr(Mnemonic.qdsub, R8,R0,R16)),
                Mask(4, 2,
                    Instr(Mnemonic.rev, wide, Rnp8, Rnp0),
                    Instr(Mnemonic.rev16, wide,Rnp8,Rnp0),
                    Instr(Mnemonic.rbit, Rnp8, Rnp0),
                    Instr(Mnemonic.revsh, Rnp8, Rnp0)),
                Mask(4, 2,
                    Instr(Mnemonic.sel, nyi("*")),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 2,
                    Instr(Mnemonic.clz, R8,R0),
                    invalid,
                    invalid,
                    invalid),
                Mask(4, 2,
                    Instr(Mnemonic.crc32b, Rnp8, Rnp16,Rnp0),
                    Instr(Mnemonic.crc32h, Rnp8, Rnp16,Rnp0),
                    Instr(Mnemonic.crc32w, Rnp8, Rnp16,Rnp0),
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
                    Instr(Mnemonic.sxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.sxth, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Mnemonic.uxtah, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.uxth, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Mnemonic.sxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.sxtb16, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Mnemonic.uxtab16, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.uxtb16, R8,R0,SrBy8_4_2)),

                Select_ne15(16,
                    Instr(Mnemonic.sxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.sxtb, R8,R0,SrBy8_4_2)),
                Select_ne15(16,
                    Instr(Mnemonic.uxtab, R8,R16,R0,SrBy8_4_2),
                    Instr(Mnemonic.uxtb, R8,R0,SrBy8_4_2)),
                invalid,
                invalid);

            var ParallelAddSub = Mask(4 + 16, 3,
                Mask(4, 3,
                    Instr(Mnemonic.sadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shadd8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.uadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqadd8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhadd8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Mnemonic.sadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shadd16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.uadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqadd16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhadd16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Mnemonic.sasx, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qasx, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shasx, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.uasx, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqasx, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhasx, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid,

                Mask(4, 3,
                    Instr(Mnemonic.ssub8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shsub8, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.usub8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqsub8, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhsub8, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Mnemonic.ssub16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shsub16, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.usub16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqsub16, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhsub16, Rnp8,Rnp16,Rnp0),
                    invalid),
                Mask(4, 3,
                    Instr(Mnemonic.ssax, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.qsax, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.shsax, Rnp8,Rnp16,Rnp0),
                    invalid,
                    Instr(Mnemonic.usax, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uqsax, Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.uhsax, Rnp8,Rnp16,Rnp0),
                    invalid),
                invalid);

            var MovMovsRegisterShiftedRegister = Mask(20, 1,
                Mask(5 + 16, 2,
                    Instr(Mnemonic.lsl, R8,R16,R0),
                    Instr(Mnemonic.lsr, R8,R16,R0),
                    Instr(Mnemonic.asr, R8,R16,R0),
                    Instr(Mnemonic.ror, R8,R16,R0)),
                Mask(5 + 16, 2,
                    Instr(Mnemonic.lsl, uf,R8,R16,R0),
                    Instr(Mnemonic.lsr, uf,R8,R16,R0),
                    Instr(Mnemonic.asr, uf,R8,R16,R0),
                    Instr(Mnemonic.ror, uf,R8,R16,R0)));

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
                        Instr(Mnemonic.mla, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.mul, Rnp8,Rnp16,Rnp0)),
                    Instr(Mnemonic.mls, Rnp8,Rnp16,Rnp0,Rnp12),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b001
                    Select_ne15(12,
                        Instr(Mnemonic.smlabb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smulbb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smlabt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smulbt, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smlatb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smultb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smlatt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smultt, Rnp8,Rnp16,Rnp0))),
                Mask(4, 2,      // op1 = 0b010
                    Select_ne15(12,
                        Instr(Mnemonic.smlad, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smuad, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smladx, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smuadx, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b011
                    Select_ne15(12,
                        Instr(Mnemonic.smlawb, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smulwb, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smlawt, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smulwt, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),

                Mask(4, 2, "op1 = 0b100",
                    Select_ne15(12,
                        Instr(Mnemonic.smlsd, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smusd, Rnp8,Rnp16,Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smlsdx, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smusdx, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b101
                    Select_ne15(12,
                        Instr(Mnemonic.smmla, Rnp8,Rnp16,Rnp0,Rnp12),
                        Instr(Mnemonic.smmul, Rnp8, Rnp16, Rnp0)),
                    Select_ne15(12,
                        Instr(Mnemonic.smmlar, Rnp8,Rnp16,Rnp0, Rnp12),
                        Instr(Mnemonic.smmulr, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b110
                    Instr(Mnemonic.smmls, Rnp8,Rnp16,Rnp0, Rnp12),
                    Instr(Mnemonic.smmlsr, Rnp8,Rnp16,Rnp0, Rnp12),
                    invalid,
                    invalid),
                Mask(4, 2,      // op1 = 0b111
                    Select_ne15(12,
                        Instr(Mnemonic.usada8, Rnp8,Rnp16,Rnp0, Rnp12),
                        Instr(Mnemonic.usad8, Rnp8,Rnp16,Rnp0)),
                    invalid,
                    invalid,
                    invalid));

            var MultiplyRegister = Select((6,2), n => n == 0,
                MultiplyAbsDifference,
                invalid);

            var LongMultiplyDivide = Mask(4 + 16, 3, "LongMultiplyDivide",
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Mnemonic.smull, Rnp12,Rnp8,Rnp16,Rnp0)),
                Select_ne15(4,
                    invalid,
                    Instr(Mnemonic.sdiv, Rnp8,Rnp16,Rnp0)),
                Select(w => SBitfield(w, 4, 4) != 0,
                    invalid,
                    Instr(Mnemonic.umull, Rnp12,Rnp8,Rnp16,Rnp0)),
                Select_ne15(4,
                    invalid,
                    Instr(Mnemonic.udiv, Rnp8,Rnp16,Rnp0)),
                // 4
                Mask(4, 4,
                    Instr(Mnemonic.smlal, Rnp12, Rnp8, Rnp16, Rnp0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.smlalbb, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Mnemonic.smlalbt, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Mnemonic.smlaltb, Rnp12, Rnp8, Rnp16, Rnp0),
                    Instr(Mnemonic.smlaltt, Rnp12, Rnp8, Rnp16, Rnp0),

                    Instr(Mnemonic.smlald, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.smlaldx, Rnp12, Rnp8, Rnp16, Rnp0),
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

                    Instr(Mnemonic.smlsld, Rnp12,Rnp8,Rnp16,Rnp0),
                    Instr(Mnemonic.smlsldx, Rnp12,Rnp8,Rnp16,Rnp0),
                    invalid,
                    invalid),
                Mask(4, 4,   // op1 = 0b110
                    Instr(Mnemonic.umlal, Rnp12, Rnp8, Rnp16, Rnp0),
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.umaal, Rnp12,Rnp8,Rnp16,Rnp0),
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
                    Instr(Mnemonic.and, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Select(Bf((12,3),(4,4)), n => n != 0b0011,
                        Select_ne15(8,
                            Instr(Mnemonic.and, uf,wide,Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Mnemonic.tst, wide,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2))))),
                        Select_ne15(8,
                            Nyi("ANDS, rotate right with extend variant on"),
                            Nyi("TST")))),
                Mask(20, 1,
                    Instr(Mnemonic.bic, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Mnemonic.bic, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                Mask(20, 1,
                    Select_ne15(16,
                        Instr(Mnemonic.orr, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.mov, wide,R8,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select_ne15(16,
                        Instr(Mnemonic.orr, uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.mov, uf,wide,R8,R0,Si((4,2),Bf((12,3),(6,2)))))),
                Mask(20, 1,
                    Select_ne15(16,
                        Instr(Mnemonic.orn, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.mvn, R8,wide,R8,R0,Si((4, 2), Bf((12,3), (6, 2))))),
                    Select_ne15(16,
                        Instr(Mnemonic.orn, uf,Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.mvn, uf,wide,Rnp8,Rnp0,Si((4,2),Bf((12,3),(6,2)))))),

                Mask(20, 1,
                    Instr(Mnemonic.eor, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Select(Bf((12,3),(4,4)), n => n != 0b0011,
                        Select_ne15(8, "",
                            Instr(Mnemonic.eor, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Mnemonic.teq, uf,wide,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Select_ne15(8, "",
                            Instr(Mnemonic.eor, uf,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Mnemonic.teq, nyi("rrx"))))),
                invalid,
                Mask(20, 1,
                    Mask(4, 2,
                        Instr(Mnemonic.pkhbt, Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        invalid,
                        Instr(Mnemonic.pkhtb, Rnp8,Rnp16,Rnp0,Si((4,2),Bf((12,3),(6,2)))),
                        invalid),
                    invalid),
                invalid,

                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Mnemonic.add, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.add, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select_ne15(8,
                        Select((16,4), n => n != 13,
                            Instr(Mnemonic.add, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Mnemonic.add, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Instr(Mnemonic.cmn,wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))))),
                invalid,
                Mask(20, 1,
                    Instr(Mnemonic.adc, wide,R8,R16,R0, Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Mnemonic.adc, wide,uf,R8,R16,R0, Si((4,2), Bf((12,3), (6,2))))),
                Mask(20, 1,
                    Instr(Mnemonic.sbc, R8,R16,R0, Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Mnemonic.sbc, uf,R8,R16,R0, Si((4,2), Bf((12,3), (6,2))))),

                invalid,
                Mask(20, 1,
                    Select((16, 4), n => n != 13,
                        Instr(Mnemonic.sub, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                        Instr(Mnemonic.sub, wide,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                    Select((8,4), n => n != 15,
                        Select((16,4), n => n != 13,
                            Instr(Mnemonic.sub, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                            Instr(Mnemonic.sub, wide,uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
                        Instr(Mnemonic.cmp, wide,R16,R0,Si((4,2),Bf((12,3),(6,2)))))),
                Mask(20, 1,
                    Instr(Mnemonic.rsb, R8,R16,R0,Si((4,2),Bf((12,3),(6,2)))),
                    Instr(Mnemonic.rsb, uf,R8,R16,R0,Si((4,2),Bf((12,3),(6,2))))),
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

        private static MaskDecoder<T32Disassembler, Mnemonic, AArch32Instruction> CreateLoadStoreDualMultipleBranchDecoder()
        {
            var ldrd = Instr(Mnemonic.ldrd, R12, R8, MemOff(PrimitiveType.Word64, baseReg: Registers.pc, offsetShift:2, offsetFields: (0, 8)));

            var LoadAcquireStoreRelease = Mask(20, 1,
                Mask(4, 3,
                    Instr(Mnemonic.stlb, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Mnemonic.stlh, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Mnemonic.stl, Rnp12, MemOff(PrimitiveType.Word32, 16)),
                    invalid,

                    Instr(Mnemonic.stlexb, R12, R0, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Mnemonic.stlexh, R12, R0, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Mnemonic.stlex, R12, R0, MemOff(PrimitiveType.Word32, 16)),
                    Instr(Mnemonic.stlexd, R12, Rp_0, MemOff(PrimitiveType.Word64, 16))),
                Mask(4, 3,
                    Instr(Mnemonic.ldab, R12, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Mnemonic.ldah, R12, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Mnemonic.lda, R12, MemOff(PrimitiveType.Word32, 16)),
                    invalid,

                    Instr(Mnemonic.ldaexb, R12, MemOff(PrimitiveType.Byte, 16)),
                    Instr(Mnemonic.ldaexh, R12, MemOff(PrimitiveType.Word16, 16)),
                    Instr(Mnemonic.ldaex, R12, MemOff(PrimitiveType.Word32, 16)),
                    Instr(Mnemonic.ldaexd, Rp_12, MemOff(PrimitiveType.Word32, 16))));

            var ldStExclusive = Mask(20, 1,
                Instr(Mnemonic.strex, R8,R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))),
                Instr(Mnemonic.ldrex, R12,MemOff(PrimitiveType.Word32, 16, offsetShift:2, offsetFields:(0,8))));

            var ldStDual = Mask(20, 1,
                Instr(Mnemonic.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))),
                Instr(Mnemonic.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift:2, indexSpec:idx24, offsetFields:(0,8))));
            var ldStDualImm = Mask(4 + 16, 1,
                Instr(Mnemonic.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));
            var ldStDualPre = Mask(20, 1,
                Instr(Mnemonic.strd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))),
                Instr(Mnemonic.ldrd, R12,R8, MemOff(PrimitiveType.Word64, 16, offsetShift: 2, indexSpec: idx24, offsetFields: (0, 8))));

            var LdStExBHD = Mask(4 + 16, 1, 4, 2, "load/store exclusive byte/half/dual",
                Instr(Mnemonic.strexb, Rnp0, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                Instr(Mnemonic.strexh, Rnp0, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                invalid,
                Instr(Mnemonic.strexd, Rnp0, Rnp12, Rnp8, MemOff(PrimitiveType.Word64, 16)),

                Instr(Mnemonic.ldrexb, Rnp12, MemOff(PrimitiveType.Byte, 16)),
                Instr(Mnemonic.ldrexh, Rnp12, MemOff(PrimitiveType.Word16, 16)),
                invalid,
                Instr(Mnemonic.ldrexd, Rnp12, Rnp8, MemOff(PrimitiveType.Word64, 16)));

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
                            Instr(Mnemonic.tbb, MemIdx(PrimitiveType.Byte, 16, 0)),
                            Instr(Mnemonic.tbh, MemIdx(PrimitiveType.Word16, 16, 0)))),
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
            var branch_T3_variant = Instr(Mnemonic.b, noItUnlessLast, PcRelative(1, Bf((26,1),(11,1),(13,1),(16,6),(0,11))));
            var branch_T4_variant = Instr(Mnemonic.b, noItUnlessLast, B_T4);
            var branch = Nyi("Branch");

            var MiscellaneousSystem = Mask(4, 4,
                invalid,
                invalid,
                Instr(Mnemonic.clrex),
                invalid,

                Instr(Mnemonic.dsb, B0_4),
                Instr(Mnemonic.dmb, B0_4),
                Instr(Mnemonic.isb, B0_4),
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
                    Instr(Mnemonic.nop, wide),
                    Instr(Mnemonic.yield, nyi("*")),
                    Instr(Mnemonic.wfe, nyi("*")),
                    Instr(Mnemonic.wfi, nyi("*")),

                    Instr(Mnemonic.sev, nyi("*")),
                    Instr(Mnemonic.sevl, nyi("*")),
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear)), // Reserved hint
                Select((0, 4), n => n != 0, 
                    Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                    Instr(Mnemonic.esb, nyi("*"))),
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint

                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Linear), // Reserved hint
                Instr(Mnemonic.dbg, nyi("*")));

            var ExceptionGeneration = Mask(4 + 16, 1, 13, 1, "Exception generation",
                Instr(Mnemonic.hvc, nyi("*")),
                invalid,
                Instr(Mnemonic.smc, Imm(16, 4)),
                Instr(Mnemonic.udf, wide, Imm(16, 4, 0, 12)));

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
                                Instr(Mnemonic.msr, cpsr, R16),
                                Instr(Mnemonic.msr, spsr, R16)),
                            Instr(Mnemonic.msr, nyi("*banked register"))),
                        Mask(5, 1,  // op5
                            Instr(Mnemonic.msr, nyi("*register")),
                            Instr(Mnemonic.msr, nyi("*banked register"))),
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
                        Instr(Mnemonic.bxj, nyi("*")),
                        Nyi("ExceptionReturn"),
                        Mask(5, 1,  // op5
                            Mask(20, 1, // read spsr
                                Instr(Mnemonic.mrs, R8, cpsr),
                                Instr(Mnemonic.mrs, R8, spsr)),
                            Instr(Mnemonic.mrs, R8, rb(Bf((20, 1), (4, 1), (16, 4))))),
                        Mask(5, 1,  // op5
                            Instr(Mnemonic.mrs, nyi("*register")),
                            Instr(Mnemonic.mrs, R8, rb(Bf((20,1),(4,1),(16,4)))))),
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
