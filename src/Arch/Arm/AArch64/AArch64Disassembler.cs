#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;

namespace Reko.Arch.Arm.AArch64
{
    using Decoder = Reko.Core.Machine.Decoder<AArch64Disassembler, Mnemonic, AArch64Instruction>;

    public partial class AArch64Disassembler : DisassemblerBase<AArch64Instruction, Mnemonic>
    {
#pragma warning disable IDE1006 // Naming Styles

        private const uint RegisterMask = 0b11111;

        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;

        private readonly Arm64Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private DasmState state;

        public AArch64Disassembler(Arm64Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = null!;
            this.state = null!;
        }

        public override AArch64Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out var wInstr))
                return null;
            this.state = new DasmState();
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = addr;
            instr.Length = 4;
            instr.InstructionClass |= wInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        private class DasmState
        {
            public Mnemonic mnemonic;
            public InstrClass iclass;
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Mnemonic shiftCode = Mnemonic.Invalid;
            public MachineOperand? shiftAmount = null;
            public bool useQ;
            public VectorData vectorData;
            public uint elemsize;

            public void Clear()
            {
                this.mnemonic = Mnemonic.Invalid;
                this.iclass = InstrClass.Invalid;
                this.ops.Clear();
                this.shiftCode = Mnemonic.Invalid;
                this.shiftAmount = null;
                this.useQ = false;
                this.vectorData = VectorData.Invalid;
            }

            public void Invalid()
            {
                Clear();
                mnemonic = Mnemonic.Invalid;
            }

            public AArch64Instruction MakeInstruction()
            {
                var instr = new AArch64Instruction
                {
                    Mnemonic = mnemonic,
                    InstructionClass = iclass,
                    Operands = ops.ToArray(),
                    ShiftCode = shiftCode,
                    ShiftAmount = shiftAmount,
                    VectorData = vectorData,
                };
                return instr;
            }
        }

        private static int Bitsize(VectorData data)
        {
            switch (data)
            {
            case VectorData.I8: return 8;
            case VectorData.I16: return 16;
            case VectorData.I32: return 32;
            case VectorData.I64: return 64;
            case VectorData.F16: return 16;
            case VectorData.F32: return 32;
            case VectorData.F64: return 64;
            }
            return 0;
        }

        private ImmediateOperand DecodeSignedImmediateOperand(uint wInstr, Bitfield[] fields, DataType dt, int sh =0 )
        {
            int n = Bitfield.ReadSignedFields(fields, wInstr);
            if (sh > 0)
            {
                n <<= sh;
            }
            return new ImmediateOperand(Constant.Create(dt, n));
        }

        private ImmediateOperand DecodeUnsignedImmediateOperand(uint wInstr, Bitfield[] fields, DataType dt, int sh = 0)
        {
            uint n = Bitfield.ReadFields(fields, wInstr);
            if (sh > 0)
            {
                n <<= sh;
            }
            return new ImmediateOperand(Constant.Create(dt, n));
        }


        /// Decode a logical immediate value in the form
        /// "N:immr:imms" (where the immr and imms fields are each 6 bits) into the
        /// integer value it represents with regSize bits.
        private ulong? DecodeLogicalImmediate(uint val, int bitSize)
        {
            // Extract the N, imms, and immr fields.
            uint N = (val >> 12) & 1;
            uint immr = (val >> 6) & 0x3f;
            uint imms = val & 0x3f;

            if (bitSize != 64 && N == 1)
                return null;
            int len = 6 - Bits.CountLeadingZeros(7, (N << 6) | (~imms & 0x3f));
            if (len < 0)
                return null;
            int size = 1 << len;
            int R = (int) (immr & (size - 1));
            int S = (int) (imms & (size - 1));
            if (S == size - 1)
                return null;
            ulong pattern = (1UL << (S + 1)) -1;
            pattern = Bits.RotateR(size, pattern, R);

            // Replicate the pattern to fill the regSize.
            while (size != bitSize)
            {
                pattern |= pattern << size;
                size *= 2;
            }
            return pattern;
        }

        private static ulong DecodeSimdImmediate(uint op, uint cmode, uint w8)
        {
            ulong Replicate(ulong n, int bits, int times)
            {
                ulong result = 0;
                for (int i =0; i < times; ++i)
                {
                    result = (result << bits) | n; 
                }
                return result;
            }

            switch (cmode >> 1)
            {
            case 0:
                return Replicate(w8, 32, 2);
            case 1:
                return Replicate(w8 << 8, 32, 2);
            case 2:
                return Replicate(w8 << 16, 32, 2);
            case 3:
                return Replicate(w8 << 24, 32, 2);
            case 4:
                return Replicate(w8, 16, 4);
            case 5:
                return Replicate(w8 << 8, 16, 4);
            case 6:
                //$BUG: matches the output of objdump, but not the description of the ref manual?
                return Replicate(w8, 32, 2);
            case 7:
                if ((cmode & 1) == 0 && op == 0)
                    return Replicate(w8, 8, 8);
                if ((cmode & 1) == 0 && op == 1)
                {
                    var a = Replicate((w8 >> 7) & 1, 1, 8) << 56;
                    var b = Replicate((w8 >> 6) & 1, 1, 8) << 48;
                    var c = Replicate((w8 >> 5) & 1, 1, 8) << 40;
                    var d = Replicate((w8 >> 4) & 1, 1, 8) << 32;
                    var e = Replicate((w8 >> 3) & 1, 1, 8) << 24;
                    var f = Replicate((w8 >> 2) & 1, 1, 8) << 16;
                    var g = Replicate((w8 >> 1) & 1, 1, 8) << 8;
                    var h = Replicate((w8) & 1, 1, 8);
                    return (a | b | c | d | e | f | g | h);
                }
                goto default;
            default:
                Debug.Print("DecodeSimdImmediate: cmode={0}", cmode);
                return 0xDEADBEEFDEADBEEF;
            }
        }

        /// <summary>
        /// 32-bit general purpose register.
        /// </summary>
        private static Mutator<AArch64Disassembler> W(int pos, int size) {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs32[iReg]));
                return true;
            };
        }
        private static readonly Mutator<AArch64Disassembler> W_0 = W(0, 5);
        private static readonly Mutator<AArch64Disassembler> W_5 = W(5, 5);
        private static readonly Mutator<AArch64Disassembler> W_10 = W(10, 5);
        private static readonly Mutator<AArch64Disassembler> W_16 = W(16, 5);

        /// <summary>
        /// 32-bit GP register - but use stack register instead of w31
        /// </summary>
        private static Mutator<AArch64Disassembler> Ws(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.AddrRegs32[iReg]));
                return true;
            };
        }
        private static readonly Mutator<AArch64Disassembler> Ws_0 = Ws(0, 5);
        private static readonly Mutator<AArch64Disassembler> Ws_5 = Ws(5, 5);
        private static readonly Mutator<AArch64Disassembler> Ws_16 = Ws(16, 5);

        /// <summary>
        /// 64-bit general purpose register.
        /// </summary>
        private static Mutator<AArch64Disassembler> X(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs64[iReg]));
                return true;
            };
        }
        private static Mutator<AArch64Disassembler> X_0 = X(0, 5);
        private static Mutator<AArch64Disassembler> X_5 = X(5, 5);
        private static Mutator<AArch64Disassembler> X_10 = X(10, 5);
        private static Mutator<AArch64Disassembler> X_16 = X(16, 5);

        // Instructions that use sp rather than x31:
        //  autda
        //  autdza
        //  autdb
        //  autdzb
        //  autia
        //  autia1716
        //  autiasp
        //  autiaz
        //  autiza
        //  autib
        //  autib1716
        //  autibsp
        //  autibz
        //  autizb
        //  cas* variants
        //  ldadd* variants
        //  ldapr* variants
        //  ldar* variants
        //  ldax* variants
        //  ldclr* variants
        //  ldeor* variants
        //  ldlar* variants
        //  ldnp
        //  pac* variants
        //  prfm*
        //  swp* variants

        /// <summary>
        /// 64-bit register - but use stack register instead of x31
        /// </summary>
        private static Mutator<AArch64Disassembler> Xs(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.AddrRegs64[iReg]));
                return true;
            };
        }
        private static readonly Mutator<AArch64Disassembler> Xs_0 = Xs(0, 5);
        private static readonly Mutator<AArch64Disassembler> Xs_5 = Xs(5, 5);
        private static readonly Mutator<AArch64Disassembler> Xs_16 = Xs(16, 5);

        // 8-bit SIMD register.
        private static Mutator<AArch64Disassembler> B(int pos, int size)
        {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs8[iReg]));
                return true;
            };
        }

        // 16-bit SIMD register.
        private static Mutator<AArch64Disassembler> H(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs16[iReg]));
                return true;
            };
        }
        private readonly static Mutator<AArch64Disassembler> H_0 = H(0, 5);
        private readonly static Mutator<AArch64Disassembler> H_5 = H(5, 5);
        private readonly static Mutator<AArch64Disassembler> H_10 = H(10, 5);
        private readonly static Mutator<AArch64Disassembler> H_16 = H(16, 5);

        // 16-bit SIMD/FPU register or zero if field = 0b00000
        private static Mutator<AArch64Disassembler> Hz(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                MachineOperand op;
                if (iReg == 0)
                {
                    op = new ImmediateOperand(new ConstantReal16(PrimitiveType.Real16, 0.0));
                }
                else
                {
                    op = new RegisterOperand(Registers.SimdRegs16[iReg]);
                }
                d.state.ops.Add(op);
                return true;
            };
        }

        // 32-bit SIMD/FPU register.
        private static Mutator<AArch64Disassembler> S(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs32[iReg]));
                return true;
            };
        }
        private readonly static Mutator<AArch64Disassembler> S_0 = S(0, 5);
        private readonly static Mutator<AArch64Disassembler> S_5 = S(5, 5);
        private readonly static Mutator<AArch64Disassembler> S_10 = S(10, 5);
        private readonly static Mutator<AArch64Disassembler> S_16 = S(16, 5);

        // 32-bit SIMD/FPU register or zero if field = 0b00000
        private static Mutator<AArch64Disassembler> Sz(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                MachineOperand op;
                if (iReg == 0)
                {
                    op = new ImmediateOperand(Constant.Real32(0.0F));
                }
                else
                {
                    op = new RegisterOperand(Registers.SimdRegs32[iReg]);
                }
                d.state.ops.Add(op);
                return true;
            };
        }

        /// <summary>
        /// 64-bit SIMD register.
        /// </summary>
        private static Mutator<AArch64Disassembler> D(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs64[iReg]));
                return true;
            };
        }
        private readonly static Mutator<AArch64Disassembler> D_0 = D(0, 5);
        private readonly static Mutator<AArch64Disassembler> D_5 = D(5, 5);
        private readonly static Mutator<AArch64Disassembler> D_10 = D(10, 5);
        private readonly static Mutator<AArch64Disassembler> D_16 = D(16, 5);

        /// <summary>
        /// 64-bit SIMD/FPU register or zero if field = 0b00000
        /// </summary>
        private static Mutator<AArch64Disassembler> Dz(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = field.Read(u);
                MachineOperand op;
                if (iReg == 0)
                {
                    op = new ImmediateOperand(Constant.Real64(0.0));
                }
                else
                {
                    op = new RegisterOperand(Registers.SimdRegs64[iReg]);
                }
                d.state.ops.Add(op);
                return true;
            };
        }

        /// <summary>
        /// 128-bit SIMD register.
        /// </summary>
        private static Mutator<AArch64Disassembler> Q(int pos, int size)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = bitfield.Read(u);
                d.state.ops.Add(new RegisterOperand(Registers.SimdRegs128[iReg]));
                return true;
            };
        }
        private readonly static Mutator<AArch64Disassembler> Q_0 = Q(0, 5);
        private readonly static Mutator<AArch64Disassembler> Q_5 = Q(5, 5);
        private readonly static Mutator<AArch64Disassembler> Q_10 = Q(10, 5);
        private readonly static Mutator<AArch64Disassembler> Q_16 = Q(16, 5);

        /// <summary>
        /// Picks either a Dx or a Qx SIMD register depending on whether the
        /// 'Q' bit is set. The q() mutator must be called first for this to 
        /// work correctly.
        /// </summary>
        private static Mutator<AArch64Disassembler> V(int pos, int size)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = bitfield.Read(u);
                var regs = d.state.useQ ? Registers.SimdRegs128 : Registers.SimdRegs64;
                d.state.ops.Add(new RegisterOperand(regs[iReg]));
                return true;
            };
        }

        /// <summary>
        /// A scalar SIMD register specified by the size in <paramref name="sizePos"/>
        /// </summary>
        private static Mutator<AArch64Disassembler> Vs(int pos, int size, RegisterStorage[][] regs, int sizePos = 22)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = bitfield.Read(u);
                var iArr = (u >> sizePos) & 3;
                var reg = regs[iArr];
                if (reg is null)
                    return false;
                d.state.ops.Add(new RegisterOperand(reg[iReg]));
                return true;
            };
        }
        private static RegisterStorage[][]? Vs_BHS_ = new RegisterStorage[][]
        {
            Registers.SimdRegs8,
            Registers.SimdRegs16,
            Registers.SimdRegs32,
            null!
        };

        private static RegisterStorage[][]? Vs_BHSD = new RegisterStorage[][]
        {
            Registers.SimdRegs8,
            Registers.SimdRegs16,
            Registers.SimdRegs32,
            Registers.SimdRegs64,
        };

        private static RegisterStorage[][]? Vs_HSD_ = new RegisterStorage[][]
        {
            Registers.SimdRegs16,
            Registers.SimdRegs32,
            Registers.SimdRegs64,
            null!
        };

        private static RegisterStorage[][]? Vs__HS_ = new RegisterStorage[][]
        {
            null!,
            Registers.SimdRegs16,
            Registers.SimdRegs32,
            null!
        };

        private static RegisterStorage[][]? Vs__SD_ = new RegisterStorage[][]
{
            null!,
            Registers.SimdRegs32,
            Registers.SimdRegs64,
            null!
};


        /// <summary>
        /// Picks a V register and an element arrangement, using the 2-bit size field
        /// at location <paramref name="sizePos"/> (default 22).
        /// </summary>
        private static Mutator<AArch64Disassembler> Vr(int pos, int size, VectorData[] elementArrangement, int sizePos = 22)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = bitfield.Read(u);
                var iArr = (u >> sizePos) & 3;
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var et = elementArrangement[iArr];
                if (et == VectorData.Invalid)
                    return false;
                var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg]);
                vr.ElementType = et;
                d.state.ops.Add(vr);
                return true;
            };
        }

        /// <summary>
        /// Picks a V register and an arragement using the supplied encoding.
        /// </summary>
        private static Mutator<AArch64Disassembler> Vrc(int regBitpos, int arrBitpos, int arrBitsize, VectorData[] elementArrangement)
        {
            var regField = new Bitfield(regBitpos, 5);
            var arrField = new Bitfield(arrBitpos, arrBitsize);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var iArr = arrField.Read(u);
                var et = elementArrangement[iArr];
                if (et == VectorData.Invalid)
                    return false;
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg])
                {
                    ElementType = et
                };
                d.state.ops.Add(vr);
                return true;
            };
        }

        /// <summary>
        /// Multiple vector registers.
        /// </summary>
        private static Mutator<AArch64Disassembler> Vmr(int pos, int size, int count, VectorData[] elementArrangement, int sizePos = 22)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                int iReg = (int)bitfield.Read(u);
                var iArr = (u >> sizePos) & 3;
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var vr = new VectorMultipleRegisterOperand(dt, Registers.SimdVectorReg128, iReg, count)
                {
                    ElementType = elementArrangement[iArr]
                };
                d.state.ops.Add(vr);
                return true;
            };
        }

        /// <summary>
        /// Multiple vector registers with index
        /// </summary>
        private static Mutator<AArch64Disassembler> Vmrx(int pos, int size, int count, int bitsize)
        {
            var bitfield = new Bitfield(pos, size);
            var idxFields = new[]
            {
                new Bitfield(30,1),
                new Bitfield(10,3)
            };
            return (u, d) =>
            {
                int iReg = (int)bitfield.Read(u);
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var idx = Bitfield.ReadFields(idxFields, u);
                VectorData et;
                switch (bitsize)
                {
                case 8: et = VectorData.I8; break;
                case 16: et = VectorData.I16; idx >>= 1;  break;
                case 32: et = VectorData.I32; idx >>= 2;  break;
                case 64: et = VectorData.I64; idx >>= 3; break;
                default: return false;
                }
                var vr = new VectorMultipleRegisterOperand(dt, Registers.SimdVectorReg128, iReg, count)
                {
                    ElementType = et,
                    Index = (int)idx,
                };
                d.state.ops.Add(vr);
                return true;
            };
        }

        public static uint? LowestSetBit(uint imm5)
        {
            switch (imm5 & 0b111)
            {
            default:
                switch (imm5 >> 3)
                {
                default: return null;
                case 0b01: 
                case 0b11: return 3;
                case 0b10: return 4;
                }
            case 0b001: return 0;
            case 0b010: return 1;
            case 0b011: return 0;
            case 0b100: return 2;
            case 0b101: return 0;
            case 0b110: return 1;
            case 0b111: return 0;
            }
        }

        /// <summary>
        /// Picks a V register and an element arrangement from packed field
        /// (used in `dup` and `mov` for vectors)
        /// </summary>
        private static Mutator<AArch64Disassembler> Vrs(
            int pos, 
            int size, 
            int sizePos, 
            int sizeLength, 
            bool useIndex, 
            bool usePrevSize, 
            bool useVectorReg = true)
        {
            var bfReg = new Bitfield(pos, size);
            var bfSize = new Bitfield(sizePos, sizeLength);
            return (u, d) =>
            {
                uint iReg = bfReg.Read(u);
                uint imm5 = bfSize.Read(u);
                uint elemSize;
                int index;
                if (!usePrevSize)
                {
                    uint? elemsizeQ = LowestSetBit(imm5);
                    if (!elemsizeQ.HasValue)
                        return false;
                    elemSize = elemsizeQ.Value;
                    if (elemSize > 3)
                        return false;
                    d.state.elemsize = elemSize;
                    index = useIndex ? (int)(imm5 >> ((int)elemSize + 1)) : -1;
                }
                else
                {
                    elemSize = d.state.elemsize;
                    index = useIndex ? (int)(imm5 >> (int)elemSize) : -1;
                }
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                if (useVectorReg)
                {
                    var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg], BHSD[elemSize], index);
                    Debug.Assert(vr.ElementType != VectorData.Invalid);
                    vr.Index = index;
                    d.state.ops.Add(vr);
                }
                else
                {
                    var reg = Vs_BHSD![elemSize]![iReg]!;
                    d.state.ops.Add(new RegisterOperand(reg));
                }
                return true;
            };
        }

        /// <summary>
        /// Vector register, with explicit constant index.
        /// </summary>
        private static Mutator<AArch64Disassembler> Vri(int pos, int len, PrimitiveType dt, VectorData et, int idx)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg]);
                vr.Index = idx;
                vr.ElementType = et;
                Debug.Assert(vr.ElementType != VectorData.Invalid);
                d.state.ops.Add(vr);
                return true;
            };
        }

        /// <summary>
        /// Vector register with index, extracted using <paramref name="indexExtractor"/>.
        /// </summary>
        private static Mutator<AArch64Disassembler> Vrx(
            int pos, 
            int size,
            VectorData[] elementArrangement, 
            Bitfield[] indexExtractor,
            int sizePos = 22
            )
        {
            var regField = new Bitfield(pos, size);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var iArr = (u >> sizePos) & 3;
                var et = elementArrangement[iArr];
                if (et == VectorData.Invalid)
                    return false;
                var vreg = Registers.SimdVectorReg128[iReg];
                var index = (int) Bitfield.ReadFields(indexExtractor, u);
                var vr = new VectorRegisterOperand(dt, vreg, et, index);
                d.state.ops.Add(vr);
                return true;
            };
        }

        ///
        private static bool VrxSmovUmov(uint uInstr, AArch64Disassembler dasm)
        {
            var iReg = (uInstr >> 5) & 0x1Fu;
            var vreg = Registers.SimdVectorReg128[iReg];
            var imm5 = (uInstr >> 16) & 0x1Fu;
            VectorData et;
            uint idx;
            PrimitiveType dt;
            if (!dasm.state.useQ)
            {
                switch (imm5 & 0b11)
                {
                case 1:
                case 3:
                    et = VectorData.I8;
                    dt = PrimitiveType.Byte;
                    idx = imm5 >> 1;
                    break;
                case 2:
                    et = VectorData.I16;
                    dt = PrimitiveType.Word16;
                    idx = imm5 >> 2;
                    break;
                default:
                    return false;
                }
            }
            else
            {
                switch (imm5 & 0b111)
                {
                case 1:
                case 3:
                case 5:
                case 7:
                    et = VectorData.I8;
                    dt = PrimitiveType.Byte;
                    idx = imm5 >> 1;
                    break;
                case 2:
                case 6:
                    et = VectorData.I16;
                    dt = PrimitiveType.Word16;
                    idx = imm5 >> 2;
                    break;
                case 4:
                    et = VectorData.I32;
                    dt = PrimitiveType.Word32;
                    idx = imm5 >> 3;
                    break;
                default:
                    return false;
                }
            }
            var op = new VectorRegisterOperand(dt, vreg, et, (int) idx);
            dasm.state.ops.Add(op);
            return true;
        }

        private static Mutator<AArch64Disassembler> VrxIns(int regbitpos)
        {
            var regField = new Bitfield(regbitpos, 5);
            return (uint uInstr, AArch64Disassembler dasm) =>
            {
                var imm5 = (uInstr >> 16) & 0x1Fu;
                VectorData et;
                PrimitiveType dt;
                uint idx;
                switch (imm5)
                {
                default:
                    return false;
                case 0x01: case 0x03: case 0x05: case 0x07:
                case 0x09: case 0x0B: case 0x0D: case 0x0F:
                case 0x11: case 0x13: case 0x15: case 0x17:
                case 0x19: case 0x1B: case 0x1D: case 0x1F:
                    et = VectorData.I8;
                    dt = PrimitiveType.Byte;
                    idx = imm5 >> 1;
                    break;
                case 0x02: case 0x06: case 0x0A: case 0x0E:
                case 0x12: case 0x16: case 0x1A: case 0x1E:
                    et = VectorData.I16;
                    dt = PrimitiveType.Word16;
                    idx = imm5 >> 2;
                    break;
                case 0x04: case 0x0C: case 0x14: case 0x1C:
                    et = VectorData.I32;
                    dt = PrimitiveType.Word32;
                    idx = imm5 >> 3;
                    break;
                case 0x08: case 0x18:
                    et = VectorData.I64;
                    dt = PrimitiveType.Word64;
                    idx = imm5 >> 4;
                    break;
                }
                var vreg = Registers.SimdVectorReg128[regField.Read(uInstr)];
                var op = new VectorRegisterOperand(dt, vreg, et, (int) idx);
                dasm.state.ops.Add(op);
                return true;
            };
        }

        /// <summary>
        /// Extended register, depending on the option field.
        /// </summary>
        private static Mutator<AArch64Disassembler> Rx(int pos, int size, int optionPos, int optionSize)
        {
            var regField = new Bitfield(pos, size);
            var optionField = new Bitfield(optionPos, optionSize);
            return (u, d) =>
            {
                var iReg = regField.Read(u);
                var opt = optionField.Read(u);
                var reg = (opt == 0b011 || opt == 0b111)
                    ? Registers.GpRegs64[iReg]
                    : Registers.GpRegs32[iReg];
                d.state.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }

        // Extension to apply.
        private static Mutator<AArch64Disassembler> Ex(int posOption, int sizeOption, int posSh, int sizeSh)
        {
            var optionField = new Bitfield(posOption, sizeOption);
            var shField = new Bitfield(posSh, sizeSh);
            return (u, d) =>
            {
                var opt = optionField.Read(u);
                var sh = shField.Read(u);
                Mnemonic ext = Mnemonic.Invalid;
                switch (opt)
                {
                case 0: ext = Mnemonic.uxtb; break; 
                case 1: ext = Mnemonic.uxth; break;
                case 2: ext = Mnemonic.uxtw; break;
                case 3: if (sh != 0) ext = Mnemonic.uxtx; break;
                case 4: ext = Mnemonic.sxtb; break;
                case 5: ext = Mnemonic.sxth; break;
                case 6: ext = Mnemonic.sxtw; break;
                case 7: if (sh != 0) ext = Mnemonic.sxtx; break;
                }
                d.state.shiftCode = ext;
                d.state.shiftAmount = ImmediateOperand.Int32((int)sh);
                return true;
            };
        }

 

        // Unsigned immediate
        private static Mutator<AArch64Disassembler> U(int pos, int size, PrimitiveType dt, int sh = 0)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var i = d.DecodeUnsignedImmediateOperand(u, fields, dt, sh);
                d.state.ops.Add(i);
                return true;
            };
        }

        // Unsigned immediate (encoded for logical instructions)
        private static Mutator<AArch64Disassembler> Ul(int offset, PrimitiveType dt, int sh = 0)
        {
            return (u, d) =>
            {
                var imm = d.DecodeLogicalImmediate(u >> offset, dt.BitSize);
                if (imm == null)
                    return false;
                var op = new ImmediateOperand(Constant.Create(dt, imm.Value));
                d.state.ops.Add(op);
                return true;
            };
        }

        // Signed immediate
        private static Mutator<AArch64Disassembler> I(int pos, int size, PrimitiveType dt, int sh = 0)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var i = d.DecodeSignedImmediateOperand(u, fields, dt, sh);
                d.state.ops.Add(i);
                return true;
            };
        }

        // Shifted immediate page address
        private static Mutator<AArch64Disassembler> Ip(int pos1, int len1, int pos2, int len2, PrimitiveType dt, int sh)
        {
            var fields = new[]
            {
                new Bitfield(pos1, len1),
                new Bitfield(pos2, len2)
            };
            return (u, d) =>
            {
                long sPageOffset = Bitfield.ReadSignedFields(fields, u) << sh;
                ulong uAddrPage = d.addr.ToLinear() & ~0xFFFul;
                d.state.ops.Add(AddressOperand.Ptr64(uAddrPage + (ulong) sPageOffset));
                return true;
            };
        }

        // 16-bit Floating point immediate
        private static Mutator<AArch64Disassembler> If16(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var encodedFpNumber = bitfield.Read(u);
                var decodedFpNumber = DecodeReal16FpConstant(encodedFpNumber);
                var imm = new ImmediateOperand(decodedFpNumber);
                d.state.ops.Add(imm);
                return true;
            };
        }

        // 32-bit Floating point immediate
        private static Mutator<AArch64Disassembler> If32(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var encodedFpNumber = bitfield.Read(u);
                var decodedFpNumber = DecodeReal32FpConstant(encodedFpNumber);
                var imm = new ImmediateOperand(decodedFpNumber);
                d.state.ops.Add(imm);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> If32(int pos1, int length1, int pos2, int length2)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            return (u, d) =>
            {
                var encodedFpNumber = Bitfield.ReadFields(bitfields, u);
                var decodedFpNumber = DecodeReal32FpConstant(encodedFpNumber);
                var imm = new ImmediateOperand(decodedFpNumber);
                d.state.ops.Add(imm);
                return true;
            };
        }

        /// <summary>
        /// Create a mutator for a modified SIMD integer immediate.
        /// </summary>
        private static Mutator<AArch64Disassembler> Is64(int pos1, int length1, int pos2, int length2, int opPos, int opLength, int cmodePos, int cmodeLength)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2),
            };
            var opField = new Bitfield(opPos, opLength);
            var cmodeField = new Bitfield(cmodePos, cmodeLength);
            return (u, d) =>
            {
                var encodedNumber = Bitfield.ReadFields(bitfields, u);
                var op = opField.Read(u);
                var cmode = cmodeField.Read(u);
                var decodedNumber = DecodeSimdImmediate(op, cmode, encodedNumber);
                var imm = new ImmediateOperand(Constant.Word64(decodedNumber));
                d.state.ops.Add(imm);
                return true;
            };
        }

        // 64-bit Floating point immediate
        private static Mutator<AArch64Disassembler> If64(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var encodedFpNumber = bitfield.Read(u);
                var decodedFpNumber = DecodeReal64FpConstant(encodedFpNumber);
                var imm = new ImmediateOperand(decodedFpNumber);
                d.state.ops.Add(imm);
                return true;
            };
        }

        public static Constant DecodeReal16FpConstant(uint encodedFpNumber)
        {
            int w = (int)encodedFpNumber & 0x7F;    // strip off 'a' = sign bit.
            w = (w ^ 0x40) - 0x40;                  // sign extend bcdefgh
            w = w << 6;                             // push in 6 0's
            var hi = (int)encodedFpNumber >> 6;     // Get original high 2 bits
            hi ^= 1;                                // Toggle 'b'
            w &= 0x3FFF;                            // clear high 2 bits
            w |= hi << 14;                          // set the high 2 bits.
            return new ConstantReal16(PrimitiveType.Real16, new Float16((ushort)w));
        }

        /// <summary>
        /// Unpacks an encoded value into a 32-bit IEEE constant.
        /// </summary>
        /// <param name="encodedFpNumber"></param>
        /// <remarks>
        /// Input is an 8-bit vector:
        ///  a bcd efgh 
        /// output is 
        ///  a Bbbbbbcd efgh000....
        /// where B is (1-b).
        /// </remarks>
        public static Constant DecodeReal32FpConstant(uint encodedFpNumber)
        {
            int w = (int)encodedFpNumber & 0x7F;    // strip off 'a' = sign bit.
            w = (w ^ 0x40) - 0x40;                  // sign extend bcdefgh
            w = w << 19;                            // push in 19 0's
            var hi = (int)encodedFpNumber >> 6;     // Get original high 2 bits
            hi ^= 1;                                // Toggle 'b'
            w &= 0x3FFFFFFF;                        // clear high 2 bits
            w |= hi << 30;                          // set the high 2 bits.
            return Constant.FloatFromBitpattern(w);
        }

        public static Constant DecodeReal64FpConstant(uint encodedFpNumber)
        {
            long w = (long)encodedFpNumber & 0x7F;  // strip off 'a' = sign bit.
            w = (w ^ 0x40) - 0x40;                  // sign extend bcdefgh
            w &= 0x3FFF;                            // clear the soon to be high 2 bits
            var hi = (long)encodedFpNumber & 0xC0;  // Keep original high 2 bits 
            hi ^= 0x40;                             // Toggle 'b'
            hi <<= 8;                               // Shift to correct position
            w |= hi;                                // set the high bits.
            w = w << 48;                            // push in 48 0's
            return Constant.DoubleFromBitpattern(w);
        }

        // Fixed point scale.
        private static Mutator<AArch64Disassembler> Fxs(int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var scale = field.Read(u);
                var fbits = 64 - (int)scale;
                d.state.ops.Add(ImmediateOperand.Int32(fbits));
                return true;
            };
        }

        // Hard-coded 32-bit immediate value
        private static Mutator<AArch64Disassembler> UImm(uint imm)
        {
            return (u, d) =>
            {
                d.state.ops.Add(ImmediateOperand.Word32(imm));
                return true;
            };
        }

        /// <summary>
        /// Hard-coded 32-bit signed immediate value
        /// </summary>
        private static Mutator<AArch64Disassembler> SImm(int imm)
        {
            return (u, d) =>
            {
                d.state.ops.Add(ImmediateOperand.Int32(imm));
                return true;
            };
        }

        // PC-Relative offset
        private static Mutator<AArch64Disassembler> PcRel(int pos1, int size1, int pos2, int size2)
        {
            var fields = new[]
            {
                new Bitfield(pos1, size1),
                new Bitfield(pos2, size2)
            };
            return (u, d) =>
            {
                var displacement = Bitfield.ReadSignedFields(fields, u);
                var addr = d.addr + displacement;
                d.state.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // Jump displacement from address of current instruction
        private static Mutator<AArch64Disassembler> J(int pos, int size)
        {
            var fields = new Bitfield[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                var n = Bitfield.ReadSignedFields(fields, u);
                AddressOperand aop = AddressOperand.Create(d.addr + (n << 2));
                d.state.ops.Add(aop);
                return true;
            };
        }

        // Scaled immediate offset
        private static Mutator<AArch64Disassembler> Mo(PrimitiveType dt, int baseRegOff, int posOff, int lenOff)
        {
            var offsetField = new Bitfield(posOff, lenOff);
            int shift = ShiftFromSize(dt);
            return (u, d) =>
            {
                var iReg = (u >> baseRegOff) & 0x1F;
                var baseReg = Registers.AddrRegs64[iReg];
                var offset = offsetField.ReadSigned(u);
                offset <<= shift;
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = Constant.Int64(offset)
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static int ShiftFromSize(PrimitiveType dt)
        {
            int shift = 0;
            switch (dt.Size)
            {
            case 1: shift = 0; break;
            case 2: shift = 1; break;
            case 4: shift = 2; break;
            case 8: shift = 3; break;
            case 16: shift = 4; break;
            }

            return shift;
        }

        // Unscaled immediate offset
        private static Mutator<AArch64Disassembler> Mu(PrimitiveType dt, int baseRegOff, int posOff, int lenOff)
        {
            var offsetField = new Bitfield(posOff, lenOff);
            return (u, d) =>
            {
                var iReg = (u >> baseRegOff) & 0x1F;
                var baseReg = Registers.AddrRegs64[iReg];
                var offset = (int)Bits.SignExtend(offsetField.Read(u), offsetField.Length);
                var mem = new MemoryOperand(dt)
                {
                    Base = baseReg,
                    Offset = Constant.Int64(offset)
                };
                d.state.ops.Add(mem);
                return true;
            };
        }


        private static Mutator<AArch64Disassembler> Mpost(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 12, 9);
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PostIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> MpostPair(PrimitiveType dt)
        {
            var shift = ShiftFromSize(dt);
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 15, 7);
                offset <<= shift;
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PostIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> MvmrPpost(int elems)
        {
            return (u, d) =>
            {
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];
                mem.Offset = Constant.Int32(elems * dt.Size);
                mem.PostIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> Mpre(PrimitiveType dt)
        {
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 12, 9);
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PreIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        // Prefix form used for LDP / STP instructions.
        private static Mutator<AArch64Disassembler> MprePair(PrimitiveType dt)
        {
            var shift = ShiftFromSize(dt);
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                int offset = (int)Bits.SignExtend(u >> 15, 7);
                offset <<= shift;
                mem.Offset = offset != 0 ? Constant.Int32(offset) : null;
                mem.PreIndex = true;
                d.state.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> Mlit(PrimitiveType dt)
        {
            return (u, d) =>
            {
                int offset = (int)Bits.SignExtend(u >> 5, 19) << 2;
                var addr = d.addr + offset;
                d.state.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // [Xn,Xn] or [Xn,Wn,sxtb] indexed mode
        private static Mutator<AArch64Disassembler> Mr(PrimitiveType dt)
        {
            var sh = ShiftFromSize(dt);
            return (u, d) =>
            {
                var mem = new MemoryOperand(dt);
                var iReg = (u >> 5) & 0x1F;
                mem.Base = Registers.AddrRegs64[iReg];

                iReg = (u >> 16) & 0x1F;
                var option = (u >> 13) & 0x7;
                mem.Index = ((option & 1) == 1 ? Registers.GpRegs64 : Registers.GpRegs32)[iReg];

                switch (option)
                {
                case 2: mem.IndexExtend = Mnemonic.uxtw; break;
                case 3: mem.IndexExtend = Mnemonic.lsl; break;
                case 6: mem.IndexExtend = Mnemonic.sxtw; break;
                case 7: mem.IndexExtend = Mnemonic.sxtx; break;
                default: d.state.Invalid(); return false;
                }
                sh = (int)((u >> 12) & 1) * sh;
                mem.IndexShift = sh;
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Single base register access.
        /// </summary>
        private static Mutator<AArch64Disassembler> Mb(PrimitiveType dt, int pos, int len)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var mem = new MemoryOperand(dt)
                {
                    Base = Registers.AddrRegs64[iReg]
                };
                d.state.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Creates a mutator for a condition operand
        /// </summary>
        /// <returns></returns>
        private static Mutator<AArch64Disassembler> C(int pos, int size)
        {
            var field = new Bitfield(pos, size);
            return (u, d) =>
            {
                var cond = (ArmCondition)field.Read(u);
                d.state.ops.Add(new ConditionOperand(cond));
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> sc(int pos, int length)
        {
            var field = new Bitfield(pos, length);
            return (u, d) =>
            {
                var n = field.Read(u);
                switch (n)
                {
                case 1:
                    d.state.shiftCode = Mnemonic.lsl;
                    d.state.shiftAmount = ImmediateOperand.Int32(12);
                    break;
                }
                return true;
            };
        }


        private static Mutator<AArch64Disassembler> sh(int pos, int length)
        {
            var field = new Bitfield(pos, length);
            return (u, d) =>
            {
                int i = (int)field.Read(u);
                d.state.shiftCode = Mnemonic.lsl;
                d.state.shiftAmount = ImmediateOperand.Int32(16 * i);
                return true;
            };
        }

        /// <summary>
        /// Shift immediate.
        /// </summary>
        private static Mutator<AArch64Disassembler> si(int pos1, int len1, int pos2, int len2)
        {
            var bfShtype = new Bitfield(pos1, len1);
            var bfShamt = new Bitfield(pos2, len2);
            return (u, d) =>
            {
                var n = bfShtype.Read(u);
                switch (n)
                {
                case 0: d.state.shiftCode = Mnemonic.lsl; break;
                case 1: d.state.shiftCode = Mnemonic.lsr; break;
                case 2: d.state.shiftCode = Mnemonic.asr; break;
                case 3: d.state.shiftCode = Mnemonic.ror; break;
                }
                n = bfShamt.Read(u);
                d.state.shiftAmount = ImmediateOperand.Int32((int)n);
                return true;
            };
        }

        //        case 'i': // code + immediate 
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            switch (n)
        //            {
        //            case 0: state.shiftCode = Mnemonic.lsl; break;
        //            case 1: state.shiftCode = Mnemonic.lsr; break;
        //            case 2: state.shiftCode = Mnemonic.asr; break;
        //            case 3: state.shiftCode = Mnemonic.ror; break;
        //            }
        //            Expect(',', format, ref i);
        //            n = ReadUnsignedBitField(wInstr, format, ref i);
        //            state.shiftAmount = ImmediateOperand.Int32(n);
        //            break;
        //        default:
        //            NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode} shift", wInstr);
        //            break;
        //        }
        //        break;
        //    default:
        //        NotYetImplemented($"Unknown format character '{format[i - 1]}' in '{format}' decoding {opcode}", wInstr);
        //        return Invalid();
        //    }
        //}
        private static Mutator<AArch64Disassembler> Bm(int posS, int posR)
        {
            return (u, d) =>
            {
                var imms = (int)(u >> posS) & 0x3F;
                var immr = (int)(u >> posR) & 0x3F;
                uint n = (u >> 22) & 1;
                if ((u & 0x80000000u) == 0 && n == 1)
                {
                    return false;
                }
                d.state.ops.Add(ImmediateOperand.Int32(immr));
                d.state.ops.Add(ImmediateOperand.Int32(imms));
                return true;
            };
        }

        /// <summary>
        /// Q bit determines whether or not to use Qx or Dx registers in SIMD
        /// </summary>
        private static Mutator<AArch64Disassembler> q(int offset)
        {
            return (u, d) => { d.state.useQ = Bits.IsBitSet(u, offset); return true; };
        }
        private static readonly Mutator<AArch64Disassembler> q11 = q(11);
        private static readonly Mutator<AArch64Disassembler> q30 = q(30);

        /// <summary>
        /// Force Q bit to false
        /// </summary>
        private static bool q0(uint u, AArch64Disassembler d)
        {
            d.state.useQ = false; return true;
        }

        /// <summary>
        /// Force Q bit to true
        /// </summary>
        private static bool q1(uint u, AArch64Disassembler d)
        {
            d.state.useQ = true; return true;
        }

        /// <summary>
        ///  Arrangement specifier indicates how words are packed
        /// </summary>
        private static Mutator<AArch64Disassembler> As(int pos, int length, VectorData[] arrangements)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var uArrangement = bitfield.Read(u);
                var elementType = arrangements[uArrangement];
                d.state.vectorData = elementType;
                return elementType != VectorData.Invalid;
            };
        }

        private static readonly Bitfield barrierField = new Bitfield(8, 4);
        private static bool Barrier(uint uInstr, AArch64Disassembler dasm)
        {
            var barrierType = new BarrierOperand((BarrierOption)barrierField.Read(uInstr));
            dasm.state.ops.Add(barrierType);
            return true;
        }

        private static Mutator<AArch64Disassembler> x(string message)
        {
            return (u, d) =>
            {
                var op = d.state.mnemonic.ToString();
                string m;
                if (message == "")
                    m = op;
                else
                    m = $"{op} - {message}";
                d.NotYetImplemented(m);
                d.CreateInvalidInstruction();
                return false;
            };
        }

        // Aliases post process a decoded instruction to yield a preferred 
        // decoding for special cases.

        private static bool SbfmAliases(uint wInstr, AArch64Disassembler dasm)
        {
            var immr = ((ImmediateOperand)dasm.state.ops[2]).Value.ToUInt32();
            var imms = ((ImmediateOperand)dasm.state.ops[3]).Value.ToUInt32();
            int width = dasm.state.ops[0].Width.BitSize;
            if ((width == 32 && imms == 0x1F) ||
                (width == 64 && imms == 0x3F))
            {
                dasm.state.mnemonic = Mnemonic.asr;
                dasm.state.ops.RemoveAt(3);
                return true;
            }
            if (imms < immr)
            {
                dasm.state.mnemonic = Mnemonic.sbfiz;
                dasm.state.ops[2] = ImmediateOperand.Int32(width - (int)immr);
                dasm.state.ops[3] = ImmediateOperand.Int32((int)imms + 1);
                return true;
            }
            if (immr == 0)
            {
                var reg = ((RegisterOperand)dasm.state.ops[1]).Register;
                switch (imms)
                {
                case 0b00111:
                    dasm.state.mnemonic = Mnemonic.sxtb;
                    dasm.state.ops[1] = new RegisterOperand(Registers.GpRegs32[reg.Number]);
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                case 0b01111:
                    dasm.state.mnemonic = Mnemonic.sxth;
                    dasm.state.ops[1] = new RegisterOperand(Registers.GpRegs32[reg.Number]);
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                case 0b11111:
                    dasm.state.mnemonic = Mnemonic.sxtw;
                    dasm.state.ops[1] = new RegisterOperand(Registers.GpRegs32[reg.Number]);
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                }
            }
            return true;
        }

        private static bool UbfmAliases(uint wInstr, AArch64Disassembler dasm)
        {
            var immr = ((ImmediateOperand)dasm.state.ops[2]).Value.ToUInt32();
            var imms = ((ImmediateOperand)dasm.state.ops[3]).Value.ToUInt32();
            int width = dasm.state.ops[0].Width.BitSize;
            if (width == 32)
            {
                if (imms == 0x1F)
                {
                    dasm.state.mnemonic = Mnemonic.lsr;
                    dasm.state.ops.RemoveAt(3);
                    return true;
                } else if (imms + 1 == immr)
                {
                    dasm.state.mnemonic = Mnemonic.lsl;
                    dasm.state.ops.RemoveAt(3); dasm.state.ops[2] = ImmediateOperand.Int32(31 - (int)imms);

                    return true;
                }
            }
            if (width == 64)
            {
                if (imms == 0x3F)
                {
                    dasm.state.mnemonic = Mnemonic.lsr;
                    dasm.state.ops.RemoveAt(3);
                    return true;
                }
                else if (imms + 1 == immr)
                {
                    dasm.state.mnemonic = Mnemonic.lsl;
                    dasm.state.ops[2] = ImmediateOperand.Int32(63 - (int)imms);
                    dasm.state.ops.RemoveAt(3);
                    return true;
                }
            }
            if (immr == 0)
            {
                switch (imms)
                {
                case 0b00111:
                    dasm.state.mnemonic = Mnemonic.uxtb;
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                case 0b01111:
                    dasm.state.mnemonic = Mnemonic.uxth;
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                case 0b11111:
                    dasm.state.mnemonic = Mnemonic.uxtw;
                    dasm.state.ops.RemoveRange(2, 2);
                    return true;
                }
            }
            return true;
        }

        private static bool ShrnShift(uint wInstr, AArch64Disassembler dasm)
        {
            var shift = (int)(wInstr >> 16) & 0x7F;
            var vec = ((VectorRegisterOperand)dasm.state.ops[0]);
            var bitSize = Bitsize(vec.ElementType);
            if (bitSize == 0)
                return false;
            shift = 2 * bitSize - shift;
            dasm.state.ops.Add(ImmediateOperand.Int32(shift));
            return true;
        }

        private static bool UqshlShift(uint uInstr, AArch64Disassembler dasm)
        {
            var immh_immb = (int) ((uInstr >> 16) & 0x7F);
            switch ((uInstr>>19) & 0xF)
            {
            case 0:
                return false;
            case 1:
                immh_immb -= 8; break;
            case 2: case 3:
                immh_immb -= 16; break;
            case 4: case 5: case 6: case 7:
                immh_immb -= 32; break;
            default:
                immh_immb -= 64; break;
            }
            dasm.state.ops.Add(ImmediateOperand.Int32(immh_immb));
            return true;
        }

        // Fixed-point bits
        private static bool fbits(uint uInstr, AArch64Disassembler dasm)
        {
            var immh_immb = (int) ((uInstr >> 16) & 0x7F);
            int f;
            switch ((uInstr>>19) & 0xF)
            {
            case 0: case 1:
                return false;
            case 2: case 3:
                f = 32 - immh_immb;
                break;
            case 4: case 5: case 6: case 7:
                f = 64 - immh_immb;
                break;
            default:
                f = 128 - immh_immb;
                break;
            }
            dasm.state.ops.Add(ImmediateOperand.Int32(f));
            return true;
        }

        /// <summary>
        /// Selects an immediate shift amount from a list of <paramref name="size" /> using
        /// the selector encoded at bit position <paramref name="bitPos"/>.
        /// </summary>
        private static Mutator<AArch64Disassembler> ShiftImm(int bitPos, int bitLength, int[] sizes, Mnemonic shiftCode)
        {
            var sizeField = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var shIndex = sizeField.Read(u);
                var amt = sizes[shIndex];
                if (amt != 0)
                {
                    d.state.shiftAmount = ImmediateOperand.Int32(amt);
                    d.state.shiftCode = shiftCode;
                }
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> ShiftImm(int bitPos, int bitLength, int[] sizes)
        {
            var sizeField = new Bitfield(bitPos, bitLength);
            return (u, d) =>
            {
                var shIndex = sizeField.Read(u);
                var amt = sizes[shIndex];
                d.state.ops.Add(ImmediateOperand.Int32(amt));
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> Sysreg(params (int pos, int len)[] fields)
        {
            var bitfields = fields.Select(f => new Bitfield(f.pos, f.len)).ToArray();
            return (u, d) =>
            {
                var uSysreg = Bitfield.ReadFields(bitfields, u);
                if (!Registers.SystemRegisters.TryGetValue(uSysreg, out var sreg))
                {
                    var sregName = "sysreg" + string.Join("_", bitfields.Select(bf => bf.Read(u)));
                    Debug.Print("AArch64Dis: unknown system register {0} {1:X}", sregName, uSysreg);
                    sreg = RegisterStorage.Sysreg(sregName, (int)uSysreg, w64);
                }
                d.state.ops.Add(new RegisterOperand(sreg));
                return true;
            };
        }

        private static PrimitiveType i8 => PrimitiveType.SByte;
        private static PrimitiveType i16 => PrimitiveType.Int16;
        private static PrimitiveType i32 => PrimitiveType.Int32;
        private static PrimitiveType i64 => PrimitiveType.Int64;
        private static PrimitiveType w8 => PrimitiveType.Byte;
        private static PrimitiveType w16 => PrimitiveType.Word16;
        private static PrimitiveType w32 => PrimitiveType.Word32;
        private static PrimitiveType w64 => PrimitiveType.Word64;
        private static PrimitiveType w128 => PrimitiveType.Word128;

        // Packing arragement in SIMD vector register
        private static readonly VectorData[] B___ = new[]
        {
            VectorData.I8, VectorData.Invalid, VectorData.Invalid, VectorData.Invalid
        };
        private static readonly VectorData[] BH__ = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.Invalid, VectorData.Invalid
        };
        private static readonly VectorData[] BHS_ = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.Invalid
        };
        private static readonly VectorData[] BHSD = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.I64
        };
        private static readonly VectorData[] BHS_128 = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.Invalid
        };
        private static readonly VectorData[] HSD_ = new[]
        {
             VectorData.I16, VectorData.I32, VectorData.I64, VectorData.Invalid
        };
        private static readonly VectorData[] BBB_ = new[]
        {
             VectorData.I8, VectorData.I8, VectorData.I8, VectorData.Invalid
        };
        private static readonly VectorData[] BBBB = new[]
        {
             VectorData.I8, VectorData.I8, VectorData.I8, VectorData.I8
        };
        private static readonly VectorData[] B__D = new[]
        {
             VectorData.I8, VectorData.Invalid, VectorData.Invalid, VectorData.I64
        };

        private static readonly VectorData[] HHHH = new[]
        {
             VectorData.I16, VectorData.I16, VectorData.I16, VectorData.I16
        };
        private static readonly VectorData[] H__Q = new[]
        {
             VectorData.I16, VectorData.Invalid, VectorData.Invalid, VectorData.I128
        };
        private static readonly VectorData[] SD__ = new[]
        {
            VectorData.I32, VectorData.I64, VectorData.Invalid, VectorData.Invalid
        };
        private static readonly VectorData[] SSSS = new[]
        {
             VectorData.I32, VectorData.I32, VectorData.I32, VectorData.I32
        };
        private static readonly VectorData[] S_S_ = new[]
        {
            VectorData.I32, VectorData.Invalid, VectorData.I32, VectorData.Invalid
        };
        private static readonly VectorData[] DDDD = new[]
        {
             VectorData.I64, VectorData.I64, VectorData.I64, VectorData.I64
        };
        private static readonly VectorData[] SSDD = new[]
        {
             VectorData.I32, VectorData.I32, VectorData.I64, VectorData.I64
        };
        private static readonly VectorData[] _HS_ = new[]
        {
            VectorData.Invalid, VectorData.I16, VectorData.I32, VectorData.Invalid
        };
        private static readonly VectorData[] _SD_ = new[]
        {
            VectorData.Invalid, VectorData.I32, VectorData.I64, VectorData.Invalid
        };
        private static readonly VectorData[] ___D = new[]
        {
             VectorData.Invalid, VectorData.Invalid, VectorData.Invalid, VectorData.I64
        };

        // Frequently occurring predicates.

        private static bool IsZero(uint wInstr)
        {
            return wInstr == 0;
        }

        private static bool Is31(uint wInstr)
        {
            return wInstr == 0x1F;
        }

        private static bool Rn_Rm_Same(uint wInstr)
        {
            return ((wInstr >> 5) & 0x1F) == ((wInstr >> 16) & 0x1F);
        }










        // Factory methods for different kinds of decoders.

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<AArch64Disassembler> [] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, VectorData.Invalid, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<AArch64Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, iclass, VectorData.Invalid, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, VectorData vectorData, params Mutator<AArch64Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, vectorData, mutators);
        }

        //$TODO: version support in the instruction set.
        private static Decoder Instr_8_1(Mnemonic mnemonic, params Mutator<AArch64Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, VectorData.Invalid, mutators);
        }

        private static Decoder Instr_8_2(Mnemonic mnemonic, params Mutator<AArch64Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, VectorData.Invalid, mutators);
        }

        private static Decoder Instr_8_3(Mnemonic mnemonic, params Mutator<AArch64Disassembler>[] mutators)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, VectorData.Invalid, mutators);
        }

        private static Decoder Sparse(string tag, int pos1, int length1,int pos2, int length2, Decoder @default, params (uint, Decoder)[] decoders)
        {
            var bitfields = new[]
            {
                new Bitfield(pos1, length1),
                new Bitfield(pos2, length2)
            };
            var ds = Enumerable.Range(0, 1 << (length1 + length2))
                .Select(n => @default)
                .ToArray();
            foreach (var d in decoders)
            {
                ds[d.Item1] = d.Item2;
            }
            return new BitfieldDecoder<AArch64Disassembler, Mnemonic, AArch64Instruction>(bitfields, tag, ds);
        }

        private static NyiDecoder<AArch64Disassembler,Mnemonic, AArch64Instruction> Nyi(string str)
        {
            return new NyiDecoder<AArch64Disassembler, Mnemonic, AArch64Instruction>("Nyi \r\n" + str);
        }

        public override AArch64Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("AArch64Dis", this.addr, this.rdr, message);
            //return  CreateInvalidInstruction();
            return new AArch64Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
            };
        }

        public override AArch64Instruction CreateInvalidInstruction()
        {
            return new AArch64Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = new MachineOperand[0]
            };
        }


        static AArch64Disassembler()
        {
            invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            Decoder LdStRegUImm;
            {
                LdStRegUImm = Mask(Bf((30,2), (26,1), (22,2)), "Load/store register (unsigned immediate)",
                    Instr(Mnemonic.strb, W_0, Mo(i8,5, 10,12)),
                    Instr(Mnemonic.ldrb, W_0, Mo(i8,5, 10,12)),
                    Instr(Mnemonic.ldrsb, X_0, Mo(i8,5, 10,12)),
                    Instr(Mnemonic.ldrsb, W_0, Mo(i8,5, 10,12)),
                    // 00 1 00
                    Instr(Mnemonic.str, B(0,5), Mo(w8, 5, 10, 12)),
                    Instr(Mnemonic.ldr, B(0,5), Mo(w8, 5, 10, 12)),
                    Instr(Mnemonic.str, Q_0, Mo(w128, 5, 10, 12)),
                    Instr(Mnemonic.ldr, Q_0, Mo(w128, 5, 10, 12)),
                    // 01 0 00
                    Instr(Mnemonic.strh, W_0, Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldrh, W_0, Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldrsh, X_0, Mo(i16, 5, 10, 12)),
                    Instr(Mnemonic.ldrsh, W_0, Mo(i16, 5, 10, 12)),
                    // 01 1 00
                    Instr(Mnemonic.str, H_0, Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldr, H_0, Mo(w16, 5, 10, 12)),
                    invalid,
                    invalid,
                    // 10 0 00
                    Instr(Mnemonic.str, W_0, Mo(w32, 5, 10, 12)),
                    Instr(Mnemonic.ldr, W_0, Mo(w32, 5, 10, 12)),
                    Instr(Mnemonic.ldrsw, X_0, Mo(i16, 5, 10, 12)),
                    invalid,
                    // 10 1 00
                    Instr(Mnemonic.str, S_0, Mo(w32, 5, 10, 12)),
                    Instr(Mnemonic.ldr, S_0, Mo(w32, 5, 10, 12)),
                    invalid,
                    invalid,
                    // 11 0 00
                    Instr(Mnemonic.str, X_0, Mo(w64, 5, 10, 12)),
                    Instr(Mnemonic.ldr, X_0, Mo(w64, 5, 10, 12)),
                    Instr(Mnemonic.prfm, U(0,5, w8), Mo(w64, 5, 10, 12)),
                    invalid,
                    // 11 1 00
                    Instr(Mnemonic.str, D_0, Mo(w64, 5, 10, 12)),
                    Instr(Mnemonic.ldr, D_0, Mo(w64, 5, 10, 12)),
                    invalid,
                    invalid);
            }

            Decoder LdStRegisterRegOff;
            {
                LdStRegisterRegOff = Mask(14, 1,
                    invalid,
                    Mask(Bf((30, 2), (26, 1), (22, 2)),   // //LoadStoreRegisterRegOff sz V opc
                        Instr(Mnemonic.strb, W_0,Mr(w8)),
                        Instr(Mnemonic.ldrb, W_0,Mr(w8)),
                        Instr(Mnemonic.ldrsb, X_0,Mr(i8)),
                        Instr(Mnemonic.ldrsb, W_0,Mr(i8)),

                        // LoadStoreRegisterRegOff sz:V:opc=00 1 00
                        Instr(Mnemonic.str, B(0,5),Mr(w8)),
                        Instr(Mnemonic.ldr, B(0,5),Mr(w8)),
                        Instr(Mnemonic.str, Q_0,Mr(w128)),
                        Instr(Mnemonic.ldr, Q_0,Mr(w128)),

                        // LoadStoreRegisterRegOff sz:V:opc=01 0 00
                        Instr(Mnemonic.strh, W_0,Mr(w16)),
                        Instr(Mnemonic.ldrh, W_0,Mr(w16)),
                        Instr(Mnemonic.ldrsh, X_0,Mr(i16)),
                        Instr(Mnemonic.ldrsh, W_0,Mr(i16)),

                        // LoadStoreRegisterRegOff sz:V:opc=01 1 00
                        Instr(Mnemonic.str, H_0,Mr(w16)),
                        Instr(Mnemonic.ldr, H_0,Mr(w16)),
                        invalid,
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=10 0 00
                        Instr(Mnemonic.str, W_0,Mr(w32)),
                        Instr(Mnemonic.ldr, W_0,Mr(w32)),
                        Instr(Mnemonic.ldrsw, X_0,Mr(i32)),
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=10 1 00
                        Instr(Mnemonic.str, S_0,Mr(w32)),
                        Instr(Mnemonic.ldr, S_0,Mr(w32)),
                        invalid,
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=11 0 00
                        Instr(Mnemonic.str, X_0,Mr(w64)),
                        Instr(Mnemonic.ldr, X_0,Mr(w64)),
                        Instr(Mnemonic.prfm, U(0,5, w8), Mr(w64)),
                        invalid,

                        // LoadStoreRegisterRegOff sz:V:opc=11 1 00
                        Instr(Mnemonic.str, D_0,Mr(w64)),
                        Instr(Mnemonic.ldr, D_0,Mr(w64)),
                        invalid,
                        invalid));

            }

            Decoder LdStRegPairOffset;
            {
                LdStRegPairOffset = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0, W_10, Mo(w32,5,15,7)),
                    Instr(Mnemonic.ldp, W_0, W_10, Mo(w32,5,15,7)),
                    Instr(Mnemonic.stp, S_0, S_10, Mo(w32,5,15,7)),
                    Instr(Mnemonic.ldp, S_0, S_10, Mo(w32,5,15,7)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, Mo(w32,5,15,7)),
                    Instr(Mnemonic.stp, D_0, D_10, Mo(w64,5,15,7)),
                    Instr(Mnemonic.ldp, D_0, D_10, Mo(w64,5,15,7)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, Mo(w64,5,15,7)),
                    Instr(Mnemonic.ldp, X_0,X_10, Mo(w64,5,15,7)),
                    Instr(Mnemonic.stp, Q_0, Q_10, Mo(w128,5,15,7)),
                    Instr(Mnemonic.ldp, Q_0, Q_10, Mo(w128,5,15,7)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStRegPairPre;
            {
                LdStRegPairPre = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0, W_10, MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, W_0, W_10, MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, S_0, S_10, MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, S_0, S_10, MprePair(PrimitiveType.Word32)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, D_0, D_10, MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, D_0, D_10, MprePair(PrimitiveType.Word64)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, X_0,X_10, MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.stp, Q_0, Q_10, MprePair(PrimitiveType.Word128)),
                    Instr(Mnemonic.ldp, Q_0, Q_10, MprePair(PrimitiveType.Word128)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStRegPairPost;
            {
                LdStRegPairPost = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0, W_10, MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, W_0, W_10, MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, S_0, S_10, MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, S_0, S_10, MpostPair(PrimitiveType.Word32)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, D_0, D_10, MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, D_0, D_10, MpostPair(PrimitiveType.Word64)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, X_0,X_10, MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.stp, Q_0, Q_10, MpostPair(PrimitiveType.Word128)),
                    Instr(Mnemonic.ldp, Q_0, Q_10, MpostPair(PrimitiveType.Word128)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStNoallocatePair = Mask(Bf((30, 2), (26, 1), (22, 1)),
                Instr(Mnemonic.stnp, W_0, W_10,Mo(w32,5,15,7)),
                Instr(Mnemonic.ldnp, W_0, W_10,Mo(w32,5,15,7)),
                Instr(Mnemonic.stnp, S_0, S_10,Mo(w32,5,15,7)),
                Instr(Mnemonic.ldnp, S_0, S_10,Mo(w32,5,15,7)),

                invalid,
                invalid,
                Instr(Mnemonic.stnp, D_0, D_10, Mo(w64,5,15,7)),
                Instr(Mnemonic.ldnp, D_0, D_10, Mo(w64,5,15,7)),

                Instr(Mnemonic.stnp, X_0, X_10,Mo(w64,5,15,7)),
                Instr(Mnemonic.ldnp, X_0, X_10,Mo(w64,5,15,7)),
                Instr(Mnemonic.stnp, x("SIMD&FP 128-bit")),
                Instr(Mnemonic.ldnp, x("SIMD&FP 128-bit")),

                invalid,
                invalid,
                invalid,
                invalid);

            Decoder LoadsAndStores;
            {
                var LdStRegUnscaledImm = Mask(Bf((30, 2), (26, 1), (22, 2)),
                    Instr(Mnemonic.sturb, W_0, Mu(w8, 5, 12, 9)),
                    Instr(Mnemonic.ldurb, W_0, Mu(w8, 5, 12, 9)),
                    Instr(Mnemonic.ldursb, X_0, Mu(i8, 5, 12, 9)),
                    Instr(Mnemonic.ldursb, W_0, Mu(i8, 5, 12, 9)),

                    // LdStRegUnscaledImm size=00 V=1 opc=00
                    Instr(Mnemonic.stur, B(0,5), Mu(w8,5,12,9)),
                    Instr(Mnemonic.ldur, B(0,5), Mu(w8,5,12,9)),
                    Instr(Mnemonic.stur, Q_0, Mu(w128,5,12,9)),
                    Instr(Mnemonic.ldur, Q_0, Mu(w128,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=0 opc=00
                    Instr(Mnemonic.sturh, W_0, Mo(w16, 5, 12, 9)),
                    Instr(Mnemonic.ldurh, W_0, Mo(w16, 5, 12, 9)),
                    Instr(Mnemonic.ldursh, X_0, Mu(i16,5,12,9)),
                    Instr(Mnemonic.ldursh, W_0, Mu(i16,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=1 opc=00
                    Instr(Mnemonic.stur, H_0, Mu(w16,5,12,9)),
                    Instr(Mnemonic.ldur, H_0, Mu(w16,5,12,9)),
                    invalid,
                    invalid,

                    // LdStRegUnscaledImm size=10 V=0 opc=00
                    Instr(Mnemonic.stur, W_0, Mu(w32,5,12,9)),
                    Instr(Mnemonic.ldur, W_0, Mu(w32,5,12,9)),
                    Instr(Mnemonic.ldursw, X_0, Mu(w32,5,12,9)),
                    invalid,

                    // LdStRegUnscaledImm size=10 V=1 opc=00
                    Instr(Mnemonic.stur, S_0, Mu(w32,5,12,9)),
                    Instr(Mnemonic.ldur, S_0, Mu(w32,5,12,9)),
                    invalid,
                    invalid,

                    // LdStRegUnscaledImm size=11 V=0 opc=00
                    Instr(Mnemonic.stur, X_0, Mu(w64,5,12,9)),
                    Instr(Mnemonic.ldur, X_0, Mu(w64,5,12,9)),
                    Instr(Mnemonic.prfm, U(0,5, w8), Mu(w64, 5, 12, 9)),
                    invalid,

                    // LdStRegUnscaledImm size=11 V=0 opc=00
                    Instr(Mnemonic.stur, D_0, Mu(w64,5,12,9)),
                    Instr(Mnemonic.ldur, D_0, Mu(w64,5,12,9)),
                    invalid,
                    invalid);

                Decoder LdStRegImmPostIdx;
                {
                    LdStRegImmPostIdx = Mask(Bf((30, 2), (26, 1), (22, 2)),
                        Instr(Mnemonic.strb, W_0, Mpost(w8)),
                        Instr(Mnemonic.ldrb, W_0, Mpost(w8)),
                        Instr(Mnemonic.ldrsb, X_0, Mpost(i8)),
                        Instr(Mnemonic.ldrsb, W_0, Mpost(i8)),

                        Instr(Mnemonic.str, B(0, 5), Mpost(w8)),
                        Instr(Mnemonic.ldr, B(0, 5), Mpost(w8)),
                        Instr(Mnemonic.str, Q_0, Mpost(w128)),
                        Instr(Mnemonic.ldr, Q_0, Mpost(w128)),

                        Instr(Mnemonic.strh, W_0, Mpost(w16)),
                        Instr(Mnemonic.ldrh, W_0, Mpost(w16)),
                        Instr(Mnemonic.ldrsh, X_0, Mpost(i16)),
                        Instr(Mnemonic.ldrsh, W_0, Mpost(i16)),

                        Instr(Mnemonic.str, H_0, Mpost(w16)),
                        Instr(Mnemonic.ldr, H_0, Mpost(w16)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, W_0, Mpost(w32)),
                        Instr(Mnemonic.ldr, W_0, Mpost(w32)),
                        Instr(Mnemonic.ldrsw, X_0, Mpost(i32)),
                        invalid,

                        Instr(Mnemonic.str, S_0, Mpost(w32)),
                        Instr(Mnemonic.ldr, S_0, Mpost(w32)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, X_0, Mpost(w64)),
                        Instr(Mnemonic.ldr, X_0, Mpost(w64)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, X_0, Mpost(w64)),
                        Instr(Mnemonic.ldr, X_0, Mpost(w64)),
                        invalid,
                        invalid);
                }

                var LdStRegUnprivileged = Mask(26, 1, "  Load/store register (unprivileged)",
                        Mask(Bf((30, 2), (22, 2)), "  V=0",
                            Instr(Mnemonic.sttrb, x("")),
                            Instr(Mnemonic.ldtrb, x("")),
                            Instr(Mnemonic.ldtsrb, x("64-bit")),
                            Instr(Mnemonic.ldtsrb, x("32-bit")),

                            Instr(Mnemonic.sttrh, x("")),
                            Instr(Mnemonic.ldtrh, x("")),
                            Instr(Mnemonic.ldtsrh, x("64-bit")),
                            Instr(Mnemonic.ldtsrh, x("32-bit")),

                            Instr(Mnemonic.sttr, x("32-bit")),
                            Instr(Mnemonic.ldtr, x("32-bit")),
                            Instr(Mnemonic.ldtsrw, x("")),
                            invalid,

                            Instr(Mnemonic.sttr, x("64-bit")),
                            Instr(Mnemonic.ldtr, x("64-bit")),
                            invalid,
                            invalid),
                    invalid);


                Decoder LdStRegImmPreIdx;
                {
                    LdStRegImmPreIdx = Mask(Bf((30, 2), (26, 1), (22, 2)),
                        Instr(Mnemonic.strb, W_0, Mpre(w8)),
                        Instr(Mnemonic.ldrb, W_0, Mpre(w8)),
                        Instr(Mnemonic.ldrsb, X_0, Mpre(i8)),
                        Instr(Mnemonic.ldrsb, W_0, Mpre(i8)),

                        Instr(Mnemonic.str, B(0, 5), Mpre(w8)),
                        Instr(Mnemonic.ldr, B(0, 5), Mpre(w8)),
                        Instr(Mnemonic.str, Q_0, Mpre(w128)),
                        Instr(Mnemonic.ldr, Q_0, Mpre(w128)),

                        Instr(Mnemonic.strh, W_0, Mpre(w16)),
                        Instr(Mnemonic.ldrh, W_0, Mpre(w16)),
                        Instr(Mnemonic.ldrsh, X_0, Mpre(i16)),
                        Instr(Mnemonic.ldrsh, W_0, Mpre(i16)),

                        Instr(Mnemonic.str, H_0, Mpre(w16)),
                        Instr(Mnemonic.ldr, H_0, Mpre(w16)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, W_0, Mpre(w32)),
                        Instr(Mnemonic.ldr, W_0, Mpre(w32)),
                        Instr(Mnemonic.ldrsw, X_0, Mpre(i32)),
                        invalid,

                        Instr(Mnemonic.str, S_0, Mpre(w32)),
                        Instr(Mnemonic.ldr, S_0, Mpre(w32)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, X_0, Mpre(w64)),
                        Instr(Mnemonic.ldr, X_0, Mpre(w64)),
                        invalid,
                        invalid,

                        Instr(Mnemonic.str, D_0, Mpre(w64)),
                        Instr(Mnemonic.ldr, D_0, Mpre(w64)),
                        invalid,
                        invalid);
                }

                Decoder LoadRegLit;
                {
                    LoadRegLit = Mask(Bf((30,2),(26,1)),    // opc:V
                        Instr(Mnemonic.ldr, W_0, Mlit(w32)),
                        Instr(Mnemonic.ldr, S_0, Mlit(w32)),
                        Instr(Mnemonic.ldr, X_0, Mlit(w64)),
                        Instr(Mnemonic.ldr, D_0, Mlit(w64)),
                        Instr(Mnemonic.ldrsw, X_0, Mlit(i32)),
                        Instr(Mnemonic.ldr, Q_0, Mlit(w128)),
                        Instr(Mnemonic.prfm, U(0,5, w8),Mlit(w32)),
                        invalid);
                }

                Decoder AdvancedSimdLdStMultiple;
                {
                    AdvancedSimdLdStMultiple = Mask(Bf((22, 1), (12, 4)), "  Advanced SIMD load/store multiple structures",
                        Mask(30, 1,
                            Instr(Mnemonic.st4, Vmr(0, 5, 4, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.st4, q1, Vmr(0, 5, 4, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0011"),
                        Mask(30, 1,
                            Instr(Mnemonic.st3, q30, Vmr(0, 5, 3, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.st3, q30, Vmr(0, 5, 3, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0111"),
                        Mask(30, 1,
                            Instr(Mnemonic.st2, Vmr(0, 5, 2, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.st2, Vmr(0, 5, 2, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1011"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1100"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1111"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld4, Vmr(0, 5, 4, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.ld4, Vmr(0, 5, 4, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0011"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld3, Vmr(0, 5, 3, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.ld3, Vmr(0, 5, 3, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0110"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld1, q30, Vmr(0, 5, 1, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.ld1, q30, Vmr(0, 5, 1, BHSD, 10), Mb(w128, 5, 5))),
                        Mask(30, 1,
                            Instr(Mnemonic.ld2, Vmr(0, 5, 2, BHSD, 10), Mb(w64, 5, 5)),
                            Instr(Mnemonic.ld2, Vmr(0, 5, 2, BHSD, 10), Mb(w128, 5, 5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1011"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1100"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1111"));
                }
                Decoder AdvancedSimdLdStMultiplePostIdx;
                {
                    AdvancedSimdLdStMultiplePostIdx = Mask(Bf((22, 1), (12, 4)), "  Advanced SIMD load/store multiple structures (post-indexed)",
                        Select((16, 5), Is31, "  L:opcode=0:0000",
                            Instr(Mnemonic.st4, q30, Vmr(0, 5, 4, BHSD, 10), MvmrPpost(4)),
                            Instr(Mnemonic.st4, q30, Vmr(0, 5, 4, BHSD, 10), Mpost(w64), X_16)),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0010"),
                        invalid,

                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0100"),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0110"),
                        Select((16, 5), Is31,
                            Instr(Mnemonic.st1, q30, Vmr(0, 5, 1, BHSD), MvmrPpost(2)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0111")),

                        Select((16, 5), Is31,
                            Instr(Mnemonic.st2, q30, Vmr(0, 5, 2, BHSD), MvmrPpost(2)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1000 Rm != 11111")),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1010"),
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Select((16, 5), Is31, "  L:opcode = 1:0000",
                            Instr(Mnemonic.ld4, q30, Vmr(0, 5, 4, BHSD, 10), MvmrPpost(4)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0000 Rm!=11111")),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0010"),
                        invalid,
                        Select((16, 5), Is31, "  L:opcode=1:0100",
                            Instr(Mnemonic.ld3, q30, Vmr(0, 5, 3, BHSD, 10), MvmrPpost(3)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0100 Rm != 11111")),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0110"),
                        Select((16, 5), Is31, "  L:opcode=1:0111",
                            Instr(Mnemonic.ld1, q30, Vmr(0, 5, 1, BHSD, 10), MvmrPpost(1)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0111 Rm != 11111")),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1000"),
                        invalid,
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1010"),
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid);
                }
                Decoder AdvancedSimdLdStSingleStructure;
                { 
                    AdvancedSimdLdStSingleStructure = Mask(21,2, "  AdvancedSimdLdStSingleStructure",
                        Mask(13, 3, // L:R=0 0 opcode
                            Mask(10, 1,     // L:R=0 0 opcode=010 size=x?
                                Instr(Mnemonic.st1, Vmrx(0,5,1,8), Mb(w64, 5,5)),
                                Instr(Mnemonic.st1, Vmrx(0,5,1,8), Mb(w128, 5,5))),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=001"),
                            Mask(10, 1,     // L:R=0 0 opcode=010 size=x?
                                Mask(30, 1,
                                    Instr(Mnemonic.st1, Vmrx(0,5,1,16),Mb(w64,  5,5)),
                                    Instr(Mnemonic.st1, Vmrx(0,5,1,16),Mb(w128, 5,5))),
                                invalid),   // L:R=0 0 opcode=010 size=x1
                            Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=011"),
                            Mask(10, 2,  // L:R=0 0 opcode=100 size
                                Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=100 size=00"),
                                Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=100 size=01"),
                                Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=100 size=10"),
                                Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=100 size=11")),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=101"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=110"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=0 0 opcode=111")),
                        Nyi("AdvancedSimdLdStSingleStructure L:R=0 1"),
                        Mask(13, 3, "  L:R=1 0",
                            Mask(10, 1, "  opcode=000",
                                Instr(Mnemonic.ld1, Vmrx(0, 5, 1, 8), Mb(w64, 5,5)),
                                Instr(Mnemonic.ld1, Vmrx(0, 5, 1, 8), Mb(w128, 5,5))),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=001"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=010"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=011"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=100"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=101"),
                            Mask(12, 1, // L:R=10 opcode=110 S
                                Mask(30, 1,
                                    Instr(Mnemonic.ld1r, q30,Vmr(0,5,1,BHSD,10),Mb(w64,  5,5)),
                                    Instr(Mnemonic.ld1r, q30,Vmr(0,5,1,BHSD,10),Mb(w128, 5,5))),
                                invalid),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=111")),
                        Nyi("AdvancedSimdLdStSingleStructure L:R=1 1"));
                }

                Decoder AtomicMemoryOperations = Mask(26, 1, "  Atomic memory operations",
                    Mask(Bf((30, 2), (22, 2), (15, 1)), "  V=0",
                        Nyi("size:AR:o3 = 00:00:0"),
                        Nyi("size:AR:o3 = 00:00:1"),
                        Nyi("size:AR:o3 = 00:01:0"),
                        Nyi("size:AR:o3 = 00:01:1"),
                        Nyi("size:AR:o3 = 00:10:0"),
                        Nyi("size:AR:o3 = 00:10:1"),
                        Nyi("size:AR:o3 = 00:11:0"),
                        Nyi("size:AR:o3 = 00:11:1"),

                        Mask(12, 3, "  size:AR:o3 = 01:00:0",
                            Instr_8_1(Mnemonic.ldaddh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldclrh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldeorh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldseth, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldsmaxh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldsminh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.ldumaxh, W_16, W_0, Mb(w16, 5, 5)),
                            Instr_8_1(Mnemonic.lduminh, W_16, W_0, Mb(w16, 5, 5))),
                        Nyi("size:AR:o3 = 01:00:1"),
                        Mask(12, 3, "  size:AR:o3 = 01:01:0",
                            Instr_8_1(Mnemonic.ldaddlh, W_16,W_0,Mb(w16,5,5)),
                            Nyi("001"),
                            Nyi("010"),
                            Nyi("011"),
                            Nyi("100"),
                            Nyi("101"),
                            Nyi("110"),
                            Nyi("111")),
                        Nyi("size:AR:o3 = 01:01:1"),
                        Nyi("size:AR:o3 = 01:10:0"),
                        Nyi("size:AR:o3 = 01:10:1"),
                        Nyi("size:AR:o3 = 01:11:0"),
                        Nyi("size:AR:o3 = 01:11:1"),

                        Nyi("size:AR:o3 = 10:00:0"),
                        Nyi("size:AR:o3 = 10:00:1"),
                        Nyi("size:AR:o3 = 10:01:0"),
                        Nyi("size:AR:o3 = 10:01:1"),
                        Nyi("size:AR:o3 = 10:10:0"),
                        Nyi("size:AR:o3 = 10:10:1"),
                        Nyi("size:AR:o3 = 10:11:0"),
                        Nyi("size:AR:o3 = 10:11:1"),

                        Nyi("size:AR:o3 = 11:00:0"),
                        Nyi("size:AR:o3 = 11:00:1"),
                        Nyi("size:AR:o3 = 11:01:0"),
                        Nyi("size:AR:o3 = 11:01:1"),
                        Nyi("size:AR:o3 = 11:10:0"),
                        Nyi("size:AR:o3 = 11:10:1"),
                        Nyi("size:AR:o3 = 11:11:0"),
                        Nyi("size:AR:o3 = 11:11:1")),
                    invalid);


                Decoder LoadStoreExclusive = Mask(Bf((30, 2), (21, 3), (15, 1)),
                    Instr(Mnemonic.stxrb, W_16, W_0, Mb(w8, 5,5)),
                    Instr(Mnemonic.stlxrb, x("")),
                    Select((10, 5), Is31, Instr(Mnemonic.casp, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspl, x("32-bit")), invalid),
                    Instr(Mnemonic.ldxrb,  W_0, Mb(w8, 5,5)),
                    Instr(Mnemonic.ldaxrb, W_0, Mb(w8, 5, 5)),
                    Select((10, 5), Is31, Instr(Mnemonic.caspa, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspal, x("32-bit")), invalid),

                    Instr(Mnemonic.stllrb, W_0, Mb(w8, 5, 5)),
                    Instr(Mnemonic.stlrb, W_0, Mb(w8, 5, 5)),
                    Select((10, 5), Is31, Instr(Mnemonic.caspb, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspbl, x("32-bit")), invalid),
                    Instr(Mnemonic.ldlarb, W_0, Mb(w8, 5, 5)),
                    Instr(Mnemonic.ldarb, W_0, Mb(w8, 5, 5)),
                    Select((10, 5), Is31, Instr(Mnemonic.casab, W_16, W_0, Mb(w32, 5, 5)), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.casalb, W_16,W_0,Mb(w32, 5, 5)), invalid),

                    Instr(Mnemonic.stxrh, W_16, W_0, Mb(w16, 5, 5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010001"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010010"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010011"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010100"),
                    Instr(Mnemonic.ldaxrh, W_0,Mb(w8, 5,5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010110"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010111"),

                    Instr(Mnemonic.stllrh, W_0, Mb(w8, 5, 5)),
                    Instr(Mnemonic.stlrh, W_0, Mb(w8, 5, 5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 011010"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 011011"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 011100"),
                    Instr(Mnemonic.ldarh, W_0, Mb(w8, 5, 5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 011110"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 011111"),

                    Instr(Mnemonic.stxr, W_16, W_0, Mb(w8, 5, 5)),
                    Instr(Mnemonic.stlxr, W_16, W_0, Mb(w8, 5, 5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100010"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100011"),

                    Instr(Mnemonic.ldxr, W_0, Mb(w8, 5,5)),
                    Instr(Mnemonic.ldaxr, W_0, Mb(w8, 5,5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100110"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100111"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101000"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101001"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101010"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101011"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101100"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101101"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101110"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 101111"),

                    Instr(Mnemonic.stxr, W_16, X_0, Mb(w64, 5,5)),
                    Instr(Mnemonic.stlxr, X_0, Mb(w64, 5,5)),
                    Instr(Mnemonic.stxp, X_0,X_10, Mb(w64, 5,5)),
                    Instr(Mnemonic.stlxp, X_0,X_10, Mb(w64, 5,5)),

                    Instr(Mnemonic.ldxr, X_0,Mb(w64,5,5)),
                    Instr(Mnemonic.ldaxr, X_0, Mb(w64, 5,5)),
                    Instr(Mnemonic.ldxp, X_0,X_10, Mb(w64, 5, 5)),
                    Instr(Mnemonic.ldaxp, X_0, X_10, Mb(w64, 5, 5)),

                    Instr(Mnemonic.stllr, X_0, Mb(w64, 5, 5)),
                    Instr(Mnemonic.stlr, X_0, Mb(w64, 5, 5)),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111010"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111011"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111100"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111101"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111110"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 111111"));

                Decoder LoadStoreRegisterPAC = Select((30, 2), u => u == 3, "  Load/store register (pac)",
                    Mask(Bf((23, 1), (11, 1)), "  size=0b11",
                        Nyi("LDRAA,LDRAB - Key A offset variant"),
                        Nyi("LDRAA,LDRAB - Key A pre-indexed variant"),
                        Nyi("LDRAA,LDRAB - Key B offset variant"),
                        Nyi("LDRAA,LDRAB - Key B pre-indexed variant")),
                    invalid);

                LoadsAndStores = Mask(31, 1, "  Loads and stores",
                    Mask(28, 2, "LdSt op0 = 0",          // op0 = 0 
                        Mask(26, 1, "LdSt op0 = 0, op1:00",      // op0 = 0 op1 = 0
                            Mask(23, 2, "LdSt op0=0 op1=00 op2=0",  // op0 = 0 op1 = 00 op2 = 0
                                LoadStoreExclusive,
                                LoadStoreExclusive,
                                invalid,
                                invalid),
                            Mask(23, 2,  // op0 = 0 op1 = 00 op2 = 1
                                Select((16, 6), IsZero,
                                    AdvancedSimdLdStMultiple,
                                    invalid),
                                Select((21, 1), IsZero,
                                    AdvancedSimdLdStMultiplePostIdx,
                                    invalid),
                                Select((16, 5), IsZero,
                                    AdvancedSimdLdStSingleStructure,
                                    invalid),
                                Nyi("AdvancedSimdLdStSingleStructure"))),
                        Mask(23, 2,      // op0 = 0, op1 = 1
                            LoadRegLit,
                            LoadRegLit,
                            invalid,
                            invalid),
                        Mask(23, 2,  "  op1=10",
                            LdStNoallocatePair,
                            LdStRegPairPost,
                            LdStRegPairOffset,
                            LdStRegPairPre),
                        Mask(24, 1, // op0 = 0, op1 = 1x
                            Mask(21, 1,     // LdSt op0 = 0, op1 = 3, op3 = 0, high bit of op4
                                Mask(10, 2, 
                                    LdStRegUnscaledImm,
                                    LdStRegImmPostIdx,
                                    LdStRegUnprivileged,
                                    LdStRegImmPreIdx),
                                Mask(10, 2, // op1 = 3, op3 = 0x, op4=1xxxx
                                    AtomicMemoryOperations,
                                    LoadStoreRegisterPAC,
                                    LdStRegisterRegOff,
                                    LoadStoreRegisterPAC)),
                            LdStRegUImm)),
                    Mask(28, 2, "  op0=1 op1",
                        Mask(26, 1, "LdSt op0=1 op1=00 op2=?",
                            Mask(23, 2, "LdSt op0=1 op1=00 op2=0 op3=??",
                                LoadStoreExclusive,
                                LoadStoreExclusive,
                                invalid,
                                invalid),
                            invalid),
                        Mask(23, 2,
                            LoadRegLit,
                            LoadRegLit,
                            invalid,
                            invalid),
                        Mask(23, 2, // op1 = 2 op3
                            LdStNoallocatePair,
                            LdStRegPairPost,
                            LdStRegPairOffset,
                            LdStRegPairPre),
                        Mask(24, 1,  "  op1=11 op3=?x", 
                            Mask(21, 1,     // high bit of op4
                                Mask(10, 2, // LoadsAndStores op1 = 3, op3 = 0x, op4=0xxxx
                                    LdStRegUnscaledImm,
                                    LdStRegImmPostIdx,
                                    LdStRegUnprivileged,
                                    LdStRegImmPreIdx),
                                Mask(10, 2, // LoadsAndStores op1 = 3, op3 = 0x, op4=1xxxx
                                    Nyi("*AtomicMemoryOperations"),
                                    Nyi("*LoadStoreRegister PAC"),
                                    LdStRegisterRegOff,
                                    Nyi("*LoadStoreRegister PAC"))),
                            LdStRegUImm)));
            }

            var AddSubImmediate = Mask(23, 1,
                Mask(29, 3,
                    Instr(Mnemonic.add, Ws(0,5),Ws(5,5),U(10,12,w32),sc(22,2)),
                    Instr(Mnemonic.adds, Ws(0,5),Ws(5,5),U(10,12,w32),sc(22,2)),
                    Instr(Mnemonic.sub, Ws(0,5),Ws(5,5),U(10,12,w32),sc(22,2)),
                    Select((0, 5), n=>n == 0x1F,
                        Instr(Mnemonic.cmp, Ws(5,5),U(10,12,w32),sc(22,2)),
                        Instr(Mnemonic.subs, W_0,W_5,U(10,12,w32),sc(22,2))),
                    
                    Instr(Mnemonic.add, Xs_0,Xs(5,5),U(10,12,w64),sc(22,2)),
                    Instr(Mnemonic.adds, Xs_0,Xs(5,5),U(10,12,w64),sc(22,2)),
                    Instr(Mnemonic.sub, Xs(0,5),Xs(5,5),U(10,12,w64),sc(22,2)),
                    Select((0, 5), n=> n == 0x1F,
                        Instr(Mnemonic.cmp, Xs(5,5),U(10,12,w64),sc(22,2)),
                        Instr(Mnemonic.subs, X_0,Xs(5,5),U(10,12,w64),sc(22,2)))),
                invalid);

            var LogicalImmediate = Mask(29, 3, // size + op flag
                Mask(22, 1, // N bit
                    Instr(Mnemonic.and, W_0,W_5,Ul(10,w32)),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Mnemonic.orr, W_0,W_5,Ul(10,w32)),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Mnemonic.eor, W_0,W_5,Ul(10,w32)),
                    invalid),
                Mask(22, 1, // N bit
                    Instr(Mnemonic.ands, W_0,W_5,Ul(10,w32)),
                    invalid),

                Instr(Mnemonic.and, X_0,X_5,Ul(10,w64)),
                Instr(Mnemonic.orr, X_0,X_5,Ul(10,w64)),
                Instr(Mnemonic.eor, X_0,X_5,Ul(10,w64)),
                Instr(Mnemonic.ands, X_0,X_5,Ul(10,w64)));

            Nyi("LogicalImmediate");

            var MoveWideImmediate = Mask(29, 3,
                Mask(22, 1,
                    Instr(Mnemonic.movn, W_0,U(5,16,w32),sh(21,2)),
                    invalid),
                invalid,
                Mask(22, 1,
                    Instr(Mnemonic.movz, W_0,U(5,16,w32),sh(21,2)),
                    invalid),
                Mask(22, 1,
                    Instr(Mnemonic.movk, W_0,U(5,16,w32),sh(21,2)),
                    invalid),

                Instr(Mnemonic.movn, X_0,U(5,16,w64),sh(21,2)),
                invalid,
                Instr(Mnemonic.movz, X_0,U(5,16,w64),sh(21,2)),
                Instr(Mnemonic.movk, X_0,U(5,16,w16),sh(21,2)));


            var PcRelativeAddressing = Mask(31, 1,
                Instr(Mnemonic.adr, X_0, PcRel(5,19,29,2)),
                Instr(Mnemonic.adrp, X_0, Ip(5,19,29,2,i32,12)));

            Decoder Bitfield;
            {
                Bitfield = Mask(22, 1, "  Bitfield",
                    Mask(29, 3, "  N=0",
                        Instr(Mnemonic.sbfm, W_0,W_5,U(16,6,i32),U(10,6,i32), SbfmAliases),
                        Instr(Mnemonic.bfm, W_0,W_5,U(16,6,i32),U(10,6,i32)),
                        Instr(Mnemonic.ubfm, W_0,W_5,U(16,6,i32),U(10,6,i32), UbfmAliases),
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(29, 3, "  N=1",
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Instr(Mnemonic.sbfm, X_0,X_5,U(16,6,i32),U(10,6,i32), SbfmAliases),
                        Instr(Mnemonic.bfm, X_0,X_5,U(16,6,i32),U(10,6,i32)),
                        Instr(Mnemonic.ubfm, X_0,X_5,U(16,6,i32),U(10,6,i32), UbfmAliases), 
                        invalid));
            }
            Decoder Extract = Select((29, 2), IsZero,
                Mask(31, 1, "Extract: sf:0",
                    Select((21, 2), u => u == 0b00,
                        Select((15, 1), IsZero, Instr(Mnemonic.extr, W_0,W_5,W_16,U(10,6,i32)), invalid),
                        invalid),
                    Select((21, 2), u => u == 0b10,
                        Instr(Mnemonic.extr, X_0,X_5,X_16,U(10,6,i32)),    //$TODO: aliases: ROR is a special case of EXTR.
                        invalid)),
                invalid);

            var DataProcessingImm = Mask(23, 3, "Data Processing -- Immediate",
                PcRelativeAddressing,
                PcRelativeAddressing,
                AddSubImmediate,
                AddSubImmediate,

                LogicalImmediate,
                MoveWideImmediate,
                Bitfield,
                Extract);

            var UncondBranchImm = Mask(31, 1,
                Instr(Mnemonic.b, InstrClass.Transfer, J(0,26)),
                Instr(Mnemonic.bl, InstrClass.Transfer | InstrClass.Call, J(0,26)));

            var UncondBranchReg = Select((16,5), n => n != 0x1F,
                invalid,
                Mask(21, 4, "  UncondBranchReg",
                    Sparse(10, 6, "  0000",
                        invalid,
                        (0, Select((0, 5), n => n == 0, Instr(Mnemonic.br, InstrClass.Transfer, X_5), invalid)),
                        (2, Select((0, 5), Is31, Nyi("BRAA,BRAAZ... Key A"), invalid)),
                        (3, Select((0, 5), Is31, Nyi("BRAA,BRAAZ... Key B"), invalid))),
                    Sparse(10, 6, "  0001",
                        invalid,
                        (0, Select((0, 5), IsZero, Instr(Mnemonic.blr, InstrClass.Transfer | InstrClass.Call, X_5), invalid)),
                        (2, Select((0, 5), Is31, Nyi("BlRAA,BlRAAZ... Key A"), invalid)),
                        (3, Select((0, 5), Is31, Nyi("BlRAA,BlRAAZ... Key B"), invalid))),
                    Sparse(10, 6, "  0010",
                        invalid,
                        (0, Select((0, 5), IsZero, Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, X_5), invalid)),
                        (2, Select((0, 5), Is31, Nyi("RETAA,RETAAZ... Key A"), invalid)),
                        (3, Select((0, 5), Is31, Nyi("RETAA,RETAAZ... Key B"), invalid))),
                    invalid,

                    Select((5,5), Is31,
                        Sparse(10, 6, "  0100",
                            invalid,
                            (0, Select((0, 5), IsZero, Instr(Mnemonic.eret, InstrClass.Transfer | InstrClass.Return), invalid)),
                            (2, Select((0, 5), Is31, Nyi("ERETAA,RETAAZ... Key A"), invalid)),
                            (3, Select((0, 5), Is31, Nyi("ERETAA,RETAAZ... Key B"), invalid))),
                        invalid),
                    Select(Bf((10,6),(5,5),(0,5)), n => n == 0b000000_11111_00000,
                        Instr(Mnemonic.drps, x("*")), invalid),
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

            var CompareBranchImm = Mask(31, 1, 
                Mask(24, 1,
                    Instr(Mnemonic.cbz,  InstrClass.ConditionalTransfer, W_0,J(5,19)),
                    Instr(Mnemonic.cbnz, InstrClass.ConditionalTransfer, W_0,J(5,19))),
                Mask(24, 1,
                    Instr(Mnemonic.cbz,  InstrClass.ConditionalTransfer, X_0,J(5,19)),
                    Instr(Mnemonic.cbnz, InstrClass.ConditionalTransfer, X_0,J(5,19))));

            var TestBranchImm = Mask(24, 1,
                Mask(31, 1,
                    Instr(Mnemonic.tbz,  InstrClass.ConditionalTransfer, W_0,I(19,5,w32),J(5,14)),
                    Instr(Mnemonic.tbnz, InstrClass.ConditionalTransfer, W_0,I(19,5,w32),J(5,14))),
                Mask(31, 1,
                    Instr(Mnemonic.tbz,  InstrClass.ConditionalTransfer, W_0,I(19,5,w32),J(5,14)),
                    Instr(Mnemonic.tbnz, InstrClass.ConditionalTransfer, W_0,I(19,5,w32),J(5,14))));

            var CondBranchImm = Mask(Bf((24,1),(4,1)),
                Instr(Mnemonic.b, InstrClass.ConditionalTransfer, C(0,4),J(5,19)),
                invalid,
                invalid,
                invalid);

            var mrs_reg = Instr(Mnemonic.mrs, X_0,Sysreg((19,2),(16,3),(12,4),(8,4),(5,3)));
            var msr_reg = Instr(Mnemonic.msr, Sysreg((19,2),(16,3),(12,4),(8,4),(5,3)), X_0);
            var msr_imm = Instr(Mnemonic.msr, (u, d) =>
                {
                    d.state.ops.Add(new RegisterOperand(RegisterStorage.Sysreg("pstate", 4711, w64)));
                    return true;
                },
                U(8, 4, PrimitiveType.Byte));
            var System = Mask(19, 3,  // L:op0
                Mask(16, 3,  // System L:op0 = 0b000
                    Nyi("System L:op0 = 0b000 op1=0b000"),
                    Nyi("System L:op0 = 0b000 op1=0b001"),
                    Nyi("System L:op0 = 0b000 op1=0b010"),
                    Mask(12, 4, // System L:op0 = 0b000 op1=0b011
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0000"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0001"),
                        Mask(8, 4, // System L:op0 = 0b000 op1=0b011 crN=0010 crM
                            Mask(5, 3, // System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2
                                Select((0, 5), Is31, Instr(Mnemonic.nop), invalid),
                                Select((0, 5), Is31, Instr(Mnemonic.yield, x("*")), invalid),
                                Select((0, 5), Is31, Instr(Mnemonic.wfe, x("*")), invalid),
                                Select((0, 5), Is31, Instr(Mnemonic.wfi, x("*")), invalid),

                                Select((0, 5), Is31, Instr(Mnemonic.sev, x("*")), invalid),
                                Select((0, 5), Is31, Instr(Mnemonic.sevl, x("*")), invalid),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=110"),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=111")),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0001"),
                            Sparse(5, 3, "  crM=0010",
                                Sparse(5, 7, "  ",
                                    Instr(Mnemonic.hint, U(5, 7, PrimitiveType.Byte)),
                                    (0b0000_000, Instr(Mnemonic.nop)),
                                    (0b0000_001, Instr(Mnemonic.yield)),
                                    (0b0000_010, Instr(Mnemonic.wfe)),
                                    (0b0000_011, Instr(Mnemonic.wfi)),

                                    (0b0000_100, Instr(Mnemonic.sev)),
                                    (0b0000_101, Instr(Mnemonic.sevl))),
                                (0, Nyi("esb")),
                                (1, Nyi("psb csync"))),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0011"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0100"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0110"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0101"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0111"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1000"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1001"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1010"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1011"),

                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1100"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1101"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1110"),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=1111")),
                        Mask(5, 3, "System L:op0 = 0b000 op1=0b011 crN=0011 op2=???",
                            invalid,
                            invalid,
                            Select((0, 5), Is31, Nyi("clrex"), invalid),
                            invalid,

                            Select((0, 5), Is31, Instr(Mnemonic.dsb, Barrier), invalid), //$TODO: use barrier options
                            Select((0, 5), Is31, Instr(Mnemonic.dmb, Barrier), invalid), //$TODO: use barrier options
                            Select((0, 5), Is31, Instr(Mnemonic.isb, Barrier), invalid),//$TODO: only 0b1111 = SY barrier allowed
                            invalid),

                        Select((0, 5), Is31, msr_imm, Nyi("System L:op0 = 0b000 op1=0b011 crN=0100 Rt!=11111")),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0110"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0101"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=0111"),

                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1000"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1001"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1010"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1011"),

                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1100"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1101"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1110"),
                        Nyi("System L:op0 = 0b000 op1=0b011 crN=1111")),
                    Nyi("System L:op0 = 0b000 op1=0b100"),
                    Nyi("System L:op0 = 0b000 op1=0b101"),
                    Nyi("System L:op0 = 0b000 op1=0b110"),
                    Nyi("System L:op0 = 0b000 op1=0b111")),
                Nyi("sys"),
                msr_reg,
                msr_reg,

                invalid,
                Nyi("sysl"),
                mrs_reg,
                mrs_reg);

            var ExceptionGeneration = Sparse("ExceptionGeneration", 21, 3, 0, 5, invalid,
                (0b000_000_01, Instr(Mnemonic.svc, U(5,16, PrimitiveType.Word16))),
                (0b000_000_10, Instr(Mnemonic.hvc, U(5,16, PrimitiveType.Word16))),
                (0b000_000_11, Instr(Mnemonic.smc, U(5,16, PrimitiveType.Word16))),
                (0b001_000_00, Instr(Mnemonic.brk, InstrClass.Terminates, U(5,16, PrimitiveType.Word16))),
                (0b010_000_00, Instr(Mnemonic.hlt, InstrClass.Terminates, U(5,16, PrimitiveType.Word16))),
                (0b010_101_01, Instr(Mnemonic.dcps1, U(5,16, PrimitiveType.Word16))),
                (0b010_101_10, Instr(Mnemonic.dcps2, U(5,16, PrimitiveType.Word16))),
                (0b010_101_11, Instr(Mnemonic.dcps3, U(5, 16, PrimitiveType.Word16))));
                                                 
            var BranchesExceptionsSystem = Mask(29, 3, "BranchesExceptionsSystem",
                UncondBranchImm,
                Mask(25, 1,
                    CompareBranchImm,
                    TestBranchImm),
                Mask(25, 1,
                    CondBranchImm,
                    invalid),
                invalid,

                UncondBranchImm,
                Mask(25, 1,
                    CompareBranchImm,
                    TestBranchImm),
                Mask(22, 4,
                    ExceptionGeneration,
                    ExceptionGeneration,
                    ExceptionGeneration,
                    ExceptionGeneration,

                    System,
                    invalid,
                    invalid,
                    invalid,

                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,

                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg,
                    UncondBranchReg),
                invalid);



            Decoder LogicalShiftedRegister;
            {
                LogicalShiftedRegister = Mask(31, 1,
                    Select((15,1), n => n == 1,
                        invalid,
                        Mask(Bf((29,2),(21,1)),
                            Instr(Mnemonic.and, W_0,W_5,W_16,si(22,2,10,6)),
                            Instr(Mnemonic.bic, W_0,W_5,W_16,si(22,2,10,6)),
                            Select(Bf((22,2),(10,6),(5,5)), Is31,
                                Instr(Mnemonic.mov, W_0,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.orr, W_0,W_5,W_16,si(22,2,10,6))),
                            Select((5,5), Is31,
                                Instr(Mnemonic.mvn, W_0,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.orn, W_0,W_5,W_16,si(22,2,10,6))),

                            Instr(Mnemonic.eor, W_0,W_5,W_16,si(22,2,10,6)),
                            Instr(Mnemonic.eon, W_0,W_5,W_16,si(22,2,10,6)),
                            Select((0, 5), Is31,
                                Instr(Mnemonic.test, W_5,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.ands, W_0,W_5,W_16,si(22,2,10,6))),
                            Instr(Mnemonic.bics, W_0,W_5,W_16,si(22,2,10,6)))),
                    Mask(Bf((29,2),(21,1)),
                        Instr(Mnemonic.and, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.bic, X_0,X_5,X_16,si(22,2,10,6)),
                        Select(Bf((22,2),(10,6),(5,5)), Is31,
                            Instr(Mnemonic.mov, X_0,X_16,si(22,2,10,6)),
                            Instr(Mnemonic.orr, X_0,X_5,X_16,si(22,2,10,6))),
                        Select((5,5), Is31,
                            Instr(Mnemonic.mvn, X_0,X_16,si(22,2,10,6)),
                            Instr(Mnemonic.orn, X_0,X_5,X_16,si(22,2,10,6))),

                        Instr(Mnemonic.eor, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.eon, X_0,X_5,X_16,si(22,2,10,6)),
                        Select((0, 5), Is31,
                            Instr(Mnemonic.test, X_5,X_16,si(22,2,10,6)),
                            Instr(Mnemonic.ands, X_0,X_5,X_16,si(22,2,10,6))),
                        Instr(Mnemonic.bics, X_0,X_5,X_16,si(22,2,10,6))));
            }

            Decoder AddSubShiftedRegister;
            {
                AddSubShiftedRegister = Mask(31,1,  // size
                    Select((15,1), n => n == 1,
                        invalid,
                        Mask(29, 2,
                            Instr(Mnemonic.add, W_0,W_5,W_16,si(22,2,10,6)),
                            Instr(Mnemonic.adds, W_0,W_5,W_16,si(22,2,10,6)),
                            Instr(Mnemonic.sub, W_0,W_5,W_16,si(22,2,10,6)),
                            Select((0, 5), Is31,
                                Instr(Mnemonic.cmp, Ws(5,5),W_16,si(22,2,10,6)),
                                Instr(Mnemonic.subs, W_0,W_5,W_16,si(22,2,10,6))))),
                    Mask(29, 2,
                        Instr(Mnemonic.add,  X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.adds, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.sub,  X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.subs, X_0,X_5,X_16,si(22,2,10,6))));
            }

            var AddSubExtendedRegister = Select((22, 2), n => n != 0, "Add/subtract extended register",
                invalid,
                Mask(29, 3,
                    Instr(Mnemonic.add, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.adds, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.sub, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Select((0, 5), Is31,
                        Instr(Mnemonic.cmp, Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Mnemonic.subs, W_0,Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3))),

                    Instr(Mnemonic.add, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.adds, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.sub, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Select((0, 5), Is31,
                        Instr(Mnemonic.cmp, Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Mnemonic.subs, X_0,Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)))));

            Decoder DataProcessing3Source;
            {
                DataProcessing3Source = Mask(29, 3, "  Data processing (3 source)",
                    Mask(21, 3,
                        Mask(15, 1,
                            Select((10, 5), Is31,
                                Instr(Mnemonic.mul, W_0, W_5, W_16),
                                Instr(Mnemonic.madd, W_0,W_5,W_16, W_10)),
                            Select((10, 5), Is31,
                                Instr(Mnemonic.mneg, W_0, W_5, W_16),
                                Instr(Mnemonic.msub, W_0, W_5, W_16, W(10,5)))),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    invalid,
                    invalid,
                    invalid,

                    Mask(21, 3,
                        Mask(15, 1,
                            Select((10, 5), Is31,
                                Instr(Mnemonic.mul, X_0,X_5,X_16),
                                Instr(Mnemonic.madd, X_0,X_5,X_16,X_10)),
                            Select((10, 5), Is31,
                                Instr(Mnemonic.mneg, X_0,X_5,X_16),
                                Instr(Mnemonic.msub, X_0,X_5,X_16,X_10))),
                        Mask(15, 1,
                            Select((10, 5), Is31,
                                Instr(Mnemonic.smull, X_0,W_5,W_16),
                                Instr(Mnemonic.smaddl, X_0,W_5,W_16,X_10)),
                            Select((10, 5), Is31,
                                Instr(Mnemonic.smnegll, X_0,W_5,W_16),
                                Instr(Mnemonic.smsubl, X_0,W_5,W_16,X_10))),
                        Mask(15, 1,
                            Instr(Mnemonic.smulh, X_0,X_5,X_16),
                            invalid),
                        invalid,

                        invalid,
                        Mask(15, 1,
                            Select((10, 5), Is31,
                                Instr(Mnemonic.umull, X_0, W_5, W_16),
                                Instr(Mnemonic.umaddl, X_0, W_5, W_16, X_10)),
                            Select((10, 5), Is31,
                                Instr(Mnemonic.umnegl, X_0, W_5, W_16),
                                Instr(Mnemonic.umsubl, X_0, W_5, W_16, X_10))),
                        Mask(15, 1,
                            Instr(Mnemonic.umulh, X_0,W_5,W_16),
                            invalid),
                        invalid),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder ConditionalSelect;
            {
                ConditionalSelect = Mask(29, 3,
                    Mask(10, 2,
                        Instr(Mnemonic.csel, W_0,W_5,W_16,C(12,4)),
                        Instr(Mnemonic.csinc, W_0,W_5,W_16,C(12,4)),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 2,
                        Instr(Mnemonic.csinv, W_0,W_5,W_16,C(12,4)),
                        Instr(Mnemonic.csneg, W_0,W_5,W_16,C(12,4)),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 2,
                        Instr(Mnemonic.csel, X_0,X_5,X_16,C(12,4)),
                        Instr(Mnemonic.csinc, X_0,X_5,X_16,C(12,4)),
                        invalid,
                        invalid),
                    invalid,
                    Mask(10, 2,
                        Instr(Mnemonic.csinv, X_0,X_5,X_16,C(12,4)),
                        Instr(Mnemonic.csneg, X_0,X_5,X_16,C(12,4)),
                        invalid,
                        invalid),
                    invalid);
            }

            Decoder ConditionalCompareReg;
            {
                ConditionalCompareReg = Mask(Bf((10,1),(4,1)), "Conditional compare (register)",  // o2:o3
                    Mask(29, 3,
                        invalid,
                        Instr(Mnemonic.ccmn, W_5,W_16,U(0,4,w32),C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, W_5,W_16,U(0,4,w32),C(12,4)),

                        invalid,
                        Instr(Mnemonic.ccmn, X_5,X_16,U(0,4,w32),C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, X_5,X_16,U(0,4,w32),C(12,4))),
                    invalid,
                    invalid,
                    invalid);
            }
            Decoder ConditionalCompareImm;
            {
                ConditionalCompareImm = Select(Bf((10, 1), (4, 1)), n => n != 0, "Conditional compare (immediate)",
                    invalid,
                    Mask(29, 3,
                        invalid,
                        Instr(Mnemonic.ccmn, W_5, I(16,5,w32), U(0,4,w32), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, W_5, I(16,5,w32), U(0,4,w32), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmn, X_5, I(16,5,w64), U(0,4,w32), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, X_5, I(16,5,w64), U(0,4,w32), C(12,4))));
            }

            Decoder DataProcessing1source;
            {
                DataProcessing1source = Mask(Bf((31,1),(29,1)), // sf:S 
                    Sparse(16, 5, "  sf:S=00",
                        Nyi("DataProcessing1source sf:S=00 opcode2=?????"),
                        (0b00000, Sparse(10, 6, "  opcode2=00000",
                            Nyi("DataProcessing1source sf:S=00 opcode2=00000 opcode=??????"),
                            (0b000000, Instr(Mnemonic.rbit, W_0,W_5)),
                            (0b000001, Instr(Mnemonic.rev16, W_0,W_5)),
                            (0b000010, Instr(Mnemonic.rev, W_0, W_5)),
                            (0b000100, Instr(Mnemonic.clz, W_0, W_5)),
                            (0b000101, Instr(Mnemonic.cls, W_0, W_5))
                            ))
                        ),
                    Nyi("DataProcessing1source sf:S=01"),
                    Sparse(16, 5, "  sf:S=10",
                        Nyi("DataProcessing1source sf:S=10"),
                        (0b00000, Sparse(10, 6, "  opcode2=00000",
                            Nyi("DataProcessing1source sf:S=10 opcode2=00000 opcode=??????"),
                            (0b000000, Instr(Mnemonic.rbit, X_0, X_5)),
                            (0b000001, Instr(Mnemonic.rev16, X_0, X_5)),
                            (0b000010, Instr(Mnemonic.rev32, X_0, X_5)),
                            (0b000011, Instr(Mnemonic.rev, X_0, X_5)),
                            (0b000100, Instr(Mnemonic.clz, X_0, X_5)),
                            (0b000101, Instr(Mnemonic.cls, X_0, X_5))))),
                    Nyi("DataProcessing1source sf:S=11"));
            }

            Decoder DataProcessing2source;
            {
                DataProcessing2source = Mask(Bf((31, 1), (29, 1)),
                    Mask(12, 4,
                        Mask(10, 2, // sf:S=0:0 opcode=0000xx
                            invalid,
                            invalid,
                            Instr(Mnemonic.udiv, W_0, W_5, W_16),
                            Instr(Mnemonic.sdiv, W_0, W_5, W_16)),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0001xx"),
                        Mask(10, 2, // sf:S=0:0 opcode=0010xx
                            Instr(Mnemonic.lslv, W_0, W_5, W_16),
                            Instr(Mnemonic.lsrv, W_0, W_5, W_16),
                            Instr(Mnemonic.asrv, W_0, W_5, W_16),
                            Instr(Mnemonic.rorv, W_0, W_5, W_16)),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0011xx"),

                        Mask(10, 2, // Data Processing 2 source - sf:S=1:0 opcode=0100xx
                            Instr(Mnemonic.crc32b, W_0, W_5, W_16),
                            Instr(Mnemonic.crc32h, W_0, W_5, W_16),
                            Instr(Mnemonic.crc32w, W_0, W_5, W_16),
                            invalid),
                        Mask(10, 2, // sf:S=0:0 opcode=0101xx
                            Instr(Mnemonic.crc32cb, W_0, W_5, W_16),
                            Instr(Mnemonic.crc32ch, W_0, W_5, W_16),
                            Instr(Mnemonic.crc32cw, W_0, W_5, W_16),
                            invalid),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0110xx"),
                        Nyi("* Data Processing 2 source - sf:S=0:0 opcode=0111xx"),

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),

                    invalid,

                    Mask(12, 4,
                        Mask(10, 2, // sf:S=1:0 opcode=0000xx
                            invalid,
                            invalid,
                            Instr(Mnemonic.udiv, X_0,X_5,X_16),
                            Instr(Mnemonic.sdiv, X_0,X_5,X_16)),
                        Nyi("* Data Processing 2 source - sf:S=1:0 opcode=0001xx"),
                        Mask(10, 2, // sf:S=0:0 opcode=0010xx
                            Instr(Mnemonic.lslv, X_0,X_5,X_16),
                            Instr(Mnemonic.lsrv, X_0,X_5,X_16),
                            Instr(Mnemonic.asrv, X_0,X_5,X_16),
                            Instr(Mnemonic.rorv, X_0,X_5,X_16)),
                        Nyi("* Data Processing 2 source - sf:S=1:0 opcode=0011xx"),

                        Mask(10, 2, // sf:S=1:0 opcode=0100xx
                            Nyi("* Data Processing 2 source - sf:S=1:0 opcode=010000"),
                            Nyi("* Data Processing 2 source - sf:S=1:0 opcode=010001"),
                            Nyi("* Data Processing 2 source - sf:S=1:0 opcode=010010"),
                            Instr(Mnemonic.crc32x, W_0, W_5, X_16)),
                        Nyi("* Data Processing 2 source - sf:S=1:0 opcode=0101xx"),
                        Nyi("* Data Processing 2 source - sf:S=1:0 opcode=0110xx"),
                        Nyi("* Data Processing 2 source - sf:S=1:0 opcode=0111xx"),
                        
                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),

                    invalid);
            }

            Decoder AddSubWithCarry;
            {
                AddSubWithCarry = Select((10, 6), IsZero,
                    Mask(29, 3,
                        Instr(Mnemonic.adc, W_0, W_5, W_16),
                        Instr(Mnemonic.adcs, W_0, W_5, W_16),
                        Instr(Mnemonic.sbc, W_0, W_5, W_16),
                        Instr(Mnemonic.sbcs, W_0, W_5, W_16),

                        Instr(Mnemonic.adc, X_0,X_5,X_16),
                        Instr(Mnemonic.adcs, X_0,X_5,X_16),
                        Instr(Mnemonic.sbc, X_0,X_5,X_16),
                        Instr(Mnemonic.sbcs, X_0,X_5,X_16)),
                    invalid);
            }

            Decoder DataProcessingReg;
            {
                DataProcessingReg =  Mask(28, 1, "Data processing - register",        // op1
                    Mask(21, 4,           //op1=0 op2
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,

                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,
                        LogicalShiftedRegister,

                        AddSubShiftedRegister,
                        AddSubExtendedRegister,
                        AddSubShiftedRegister,
                        AddSubExtendedRegister,

                        AddSubShiftedRegister,
                        AddSubExtendedRegister,
                        AddSubShiftedRegister,
                        AddSubExtendedRegister),
                    Mask(21, 4,           // op1 = 1, op2
                        AddSubWithCarry,
                        invalid,
                        Mask(11, 1,         // op1 = 1, op2 = 2,
                            ConditionalCompareReg,
                            ConditionalCompareImm),
                        invalid,

                        ConditionalSelect,
                        invalid,
                        Mask(30, 1,         // op1 = 1, op2 = 6, op0
                            DataProcessing2source,
                            DataProcessing1source),
                        invalid,

                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,

                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source,
                        DataProcessing3Source));
            }

            Decoder ConversionBetweenFpAndInt;
            {
                ConversionBetweenFpAndInt = Mask(Bf((31, 1), (29, 1)), "  Conversion between floating-point and integer",
                    Mask(22, 2,      // sf:S=0b00 type
                        Sparse(16, 5, "  type=00",
                            Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=00"),
                            (0b00_010, Instr(Mnemonic.scvtf, S_0,W_5)),
                            (0b00_011, Instr(Mnemonic.ucvtf, S_0,W_5)),
                            (0b00_110, Instr(Mnemonic.fmov,  W_0, S_5)),
                            (0b00_111, Instr(Mnemonic.fmov,  S_0,W_5)),
                            (0b01_000, Instr(Mnemonic.fcvtps, W_0, S_5)),
                            (0b10_000, Instr(Mnemonic.fcvtms, W_0, S_5)),
                            (0b11_000, Instr(Mnemonic.fcvtzs, W_5,S_0)),
                            (0b11_001, Instr(Mnemonic.fcvtzu, W_5,S_0))),
                        Sparse(16, 5, "  type=01",
                            Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=01"),
                            (0b00_010, Instr(Mnemonic.scvtf, D_0,W_5)),
                            (0b11_000, Instr(Mnemonic.fcvtzs, W_0, D_5))
                            ),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=10"),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=11")),
                    invalid,
                    Mask(22, 2,      "  sf:S=0b10",
                        Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=00"),
                        Sparse(16, 5,  "  type=01",
                            Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=01"),
                            (0b00_010, Instr(Mnemonic.scvtf, D_0, X_5)),
                            (0b00_110, Instr(Mnemonic.fmov, X_0, D_5)),
                            (0b00_111, Instr(Mnemonic.fmov, D_0,X_5)),
                            (0b11_000, Instr(Mnemonic.fcvtzs, W_5, D_0))
                            ),
                        Sparse(16, 5, "  type=10",
                            Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=10"),
                            (0b01_111, Instr(Mnemonic.fmov, Vri(0,5,w128,VectorData.I64, 1),X_5)),
                            (0b01_110, Instr(Mnemonic.fmov, X_0, Vri(5, 5, w128, VectorData.I64, 1)))),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=11")),
                    invalid);
            }

            Decoder ConversionBetweenFpAndFixedPoint;
            {
                ConversionBetweenFpAndFixedPoint = Mask(Bf((31, 1), (29, 1), (22, 2)), // sf:S:type
                    Mask(16, 3, "  sf:S:type=0 0 00",
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=010"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=001"),
                        Instr(Mnemonic.scvtf, S_0,W_5,Fxs(10,6)),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=011"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=100"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=101"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=110"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=111")),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 01"),
                    invalid,
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 11"),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 00"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 01"),
                    invalid,
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 11"),
                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder AdvancedSimd3Same;
            {
                AdvancedSimd3Same = Mask(29, 1, "  Advanced SIMD three same",
                    Mask(11, 5, "  U=0",
                        Instr(Mnemonic.shadd, q30, Vr(0,5, BHS_), Vr(5,5, BHS_), Vr(16,5,BHS_)),
                        Instr(Mnemonic.sqadd, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5,BHSD)),
                        Instr(Mnemonic.srhadd, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5,BHSD)),
                        Mask(22, 2, "  opcode=00011",   // U=0 opcode=00011 size
                            Instr(Mnemonic.and, q30,Vr(0,5,BBBB),Vr(5,5,BBBB),Vr(16,5,BBBB)),
                            Instr(Mnemonic.bic, q30,Vr(0,5,BBBB),Vr(5,5,BBBB),Vr(16,5,BBBB)),
                            Select((0, 21), Rn_Rm_Same,
                                Instr(Mnemonic.mov, q30, Vr(0,5,BBB_,30), Vr(5,5,BBB_,30)),                     // U=0 opcode=00011 size=10
                                Instr(Mnemonic.orr, q30, Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30))),  // U=0 opcode=00011 size=10
                            Instr(Mnemonic.orn, q30,Vr(0,5,BBBB),Vr(5,5,BBBB),Vr(16,5,BBBB))),

                        Instr(Mnemonic.shsub, q30, Vr(0,5,BHS_), Vr(5,5, BHS_), Vr(16,5,BHS_)),
                        Instr(Mnemonic.sqsub, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.cmgt, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),
                        Instr(Mnemonic.cmge, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),

                        Instr(Mnemonic.sshl, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),
                        Instr(Mnemonic.sqshl, q30, Vr(0,5, BHSD), Vr(5,5,BHSD), Vr(16,5, BHSD)),
                        Instr(Mnemonic.srshl, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),
                        Instr(Mnemonic.sqrshl, q30, Vr(0,5, BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),

                        Instr(Mnemonic.smax, q30,Vr(0,5, BHS_), Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.smin, q30,Vr(0,5, BHS_), Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.sabd, q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.saba, q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),

                        Instr(Mnemonic.add, q30,Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                        Instr(Mnemonic.cmtst, q30,Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                        Instr(Mnemonic.mla, q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.mul, q30,Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),

                        Instr(Mnemonic.smaxp, q30,Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.sminp, q30,Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.sqdmulh, q30,Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.addp, q30,Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),

                        Mask(23, 1,
                            Nyi("FMAXNM (vector)"),
                            Nyi("FMINNM (vector)")),
                        Mask(23, 1,
                            Nyi("FMLA (vector)"),
                            Nyi("FMLS (vector)")),
                        Mask(23, 1,       // U=0 opcode=11010 size=?x
                            Instr(Mnemonic.fadd, q30,Vr(0,5,SSDD),Vr(5,5,SSDD),Vr(16,5,SSDD)),
                            Instr(Mnemonic.fsub, x("vector"))),
                        Mask(23, 1,
                            Nyi("FMULX (vector)"),
                            invalid),

                        Mask(23, 1,
                            Nyi("FCMEQ (register)"),
                            invalid),
                        Mask(23, 1,
                            Nyi("FMLAL, FMLAL2 (vector)"),
                            Nyi("FMLSL, FMLSL2 (vector)")),
                        Mask(23, 1,
                            Nyi("FMAX (vector)"),
                            Nyi("FMIN (vector)")),
                        Mask(23, 1,
                            Nyi("FRECPS"),
                            Nyi("FRSQRTS"))
                        ),
                    Mask(11, 5, "  U=1 opcode",
                        Instr(Mnemonic.uhadd , q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uqadd , q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.urhadd, q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Mask(22, 2, // U=1 opcode=00011 size
                            Instr(Mnemonic.eor, q30,Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30)),
                            Instr(Mnemonic.bsl, q30,Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30)),
                            Instr(Mnemonic.bit, q30,Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30)),
                            Instr(Mnemonic.bif, q30,Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30))),
                        
                        Instr(Mnemonic.uhsub, q1, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uqsub, q1, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.cmhi, q1, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                        Instr(Mnemonic.cmhs, q1, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                        
                        Instr(Mnemonic.ushl, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.uqshl, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.urshl, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),
                        Instr(Mnemonic.uqrshl, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), Vr(16, 5, BHSD)),

                        Instr(Mnemonic.umax, q30, Vr(0, 5, BHS_, 30), Vr(5, 5, BHS_, 30), Vr(16, 5, BHS_, 30)),
                        Instr(Mnemonic.umin, q30, Vr(0, 5, BHS_, 30), Vr(5, 5, BHS_, 30), Vr(16, 5, BHS_, 30)),
                        Instr(Mnemonic.uabd, q30, Vr(0,5, BHS_,30), Vr(5,5,BHS_,30), Vr(16,5,BHS_,30)),
                        Instr(Mnemonic.uaba, q30, Vr(0, 5, BHS_, 30), Vr(5, 5, BHS_, 30), Vr(16, 5, BHS_, 30)),

                        Instr(Mnemonic.sub, q30, Vr(0,5,BHSD), Vr(5,5, BHSD), Vr(16,5, BHSD)),
                        Instr(Mnemonic.cmeq, q1, Vr(0,5,BBBB),Vr(5,5,BBBB),Vr(16,5,BBBB)),
                        Instr(Mnemonic.mls, q30, Vr(0, 5, BHS_), Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.pmul, q30, Vr(0,5,B___),Vr(5,5,B___),Vr(16,5,B___)),

                        Instr(Mnemonic.umaxp, q30, Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.uminp, q30, Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.sqrdmulh, q30, Vr(0,5,_HS_), Vr(5,5, _HS_), Vr(16,5, _HS_)),
                        invalid,

                        Mask(23, 1,
                            Nyi("FMAXNMP (vector)"),
                            Nyi("FMINNMP (vector)")),
                        Mask(23, 1,
                            Nyi("FMLAL, FMLAL2 (vector)"),
                            Nyi("FMLSL, FMLSL2 (vector)")),
                        Mask(22, 2, // U=1 opcode=11010 size
                            Instr(Mnemonic.faddp, VectorData.F32, q30,V(0,5),V(5,5),V(16,5)),
                            Instr(Mnemonic.faddp, VectorData.F64, q30,V(0,5),V(5,5),V(16,5)),
                            Nyi("FABD"),
                            Nyi("FABD")),
                        Mask(23, 2, // U=1 opcode=00011 size=?x
                            Instr(Mnemonic.fmul, VectorData.F32, q30,Vr(0,5,SD__),Vr(5,5,SD__),Vr(16,5,SD__)),
                            Instr(Mnemonic.fmul, VectorData.F64, q30,Vr(0,5,SD__),Vr(5,5,SD__),Vr(16,5,SD__)),
                            invalid,
                            invalid),

                        Mask(23, 1,
                            Nyi("FCMGE (register)"),
                            Nyi("FCMGT (register)")),
                        Mask(23, 1,
                            Nyi("FACGE"),
                            Nyi("FACGT")),
                        Mask(23, 1,
                            Nyi("FMAXP (vector)"),
                            Nyi("FMINP (vector)")),
                        Mask(23, 2, " opcode=11111",
                            Instr(Mnemonic.fdiv, VectorData.F32, q30, V(0, 5), V(5, 5), V(16, 5)),
                            Instr(Mnemonic.fdiv, VectorData.F64, q30, V(0, 5), V(5, 5), V(16, 5)),
                            invalid,
                            invalid)));
            }

            Decoder AdvancedSimd3SameExtra;
            {
                AdvancedSimd3SameExtra = Mask(29, 1, "  Advanced SIMD three same extra",
                    Sparse(11, 4, "  U=0", invalid,
                        (0b0010, Instr_8_2(Mnemonic.sdot, x("")))),
                    Sparse(11, 4, "  U=1", invalid,
                        (0b0000, Instr_8_1(Mnemonic.sqrdmlah, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vr(16, 5, _HS_))),
                        (0b0001, Instr_8_1(Mnemonic.sqrdmlsh, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vr(16, 5, _HS_))),
                        (0b0010, Instr_8_2(Mnemonic.udot, x("(vector)"))),
                        (0b1000, Instr_8_3(Mnemonic.fcmla, x("(vector)"))),
                        (0b1001, Instr_8_3(Mnemonic.fcmla, x("(vector)"))),
                        (0b1010, Instr_8_3(Mnemonic.fcmla, x("(vector)"))),
                        (0b1011, Instr_8_3(Mnemonic.fcmla, x("(vector)"))),
                        (0b1100, Instr_8_3(Mnemonic.fcadd, x("(vector)"))),
                        (0b1110, Instr_8_3(Mnemonic.fcadd, x("(vector)")))));
            }

            Decoder AdvancedSimd3Different;
            {
                AdvancedSimd3Different = Mask(Bf((29, 1), (12, 4)), "AdvancedSimd3Different", // U:opcode
                    Mask(30, 1, "  U:opcode=0 0001",
                        Instr(Mnemonic.saddl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.saddl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  0011",
                        Instr(Mnemonic.saddw, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_)),
                        Instr(Mnemonic.saddw2, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_))),
                    Mask(30, 1, "  U:opcode=0 0001",
                        Instr(Mnemonic.ssubl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.ssubl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  0011",
                        Instr(Mnemonic.ssubw, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_)),
                        Instr(Mnemonic.ssubw2, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_))),

                    Mask(30, 1,  "0100",
                        Instr(Mnemonic.addhn, q30,Vr(0,5,BHS_), Vr(5,5,HSD_),Vr(16,5,HSD_)),
                        Instr(Mnemonic.addhn2, q30,Vr(0,5,BHS_), Vr(5,5,HSD_),Vr(16,5,HSD_))),
                    Mask(30, 1, "  0101",
                        Instr(Mnemonic.sabal, q1, Vr(0, 5, HSD_), q0, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.sabal2, q1, Vr(0, 5, HSD_), q0, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  0110",
                        Instr(Mnemonic.subhn, q0, Vr(0, 5, BHS_), q1, Vr(5, 5, HSD_), Vr(16, 5, HSD_)),
                        Instr(Mnemonic.subhn2, q1, Vr(0, 5, BHS_), Vr(5, 5, HSD_), Vr(16, 5, HSD_))),
                    Mask(30, 1, "  0111",
                        Instr(Mnemonic.sabdl, q1, Vr(0,5,HSD_), q30, Vr(5,5,BHS_), Vr(16,5,BHS_)),
                        Instr(Mnemonic.sabdl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),

                    Mask(30, 1, "  1000",
                        Instr(Mnemonic.smlal, q1, Vr(0,5,HSD_), q30,Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.smlal2, q1, Vr(0,5,HSD_), q30,Vr(5,5,BHS_),Vr(16,5,BHS_))),
                    Mask(30, 1, "  1001",
                        Instr(Mnemonic.sqdmlal, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vr(16, 5, _HS_)),
                        Instr(Mnemonic.sqdmlal2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vr(16, 5, _HS_))),
                    Mask(30, 1, "  1010",
                        Instr(Mnemonic.smlsl, q1, Vr(0,5,HSD_), q30,Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.smlsl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  1011",
                        Instr(Mnemonic.sqdmlsl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.sqdmlsl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  1100",
                        Instr(Mnemonic.smull, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.smull2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, "  1101",
                        Instr(Mnemonic.sqdmull, q1,Vr(0,5,_SD_), q30, Vr(0, 5, _HS_), Vr(16, 5, _HS_)),
                        Instr(Mnemonic.sqdmull2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vr(16, 5, _HS_))),
                    Mask(30, 1, "  1101",
                        Instr(Mnemonic.pmull, q1, Vr(0,5,H__Q),q30,Vr(5,5,B__D),Vr(16,5,B__D)),
                        Instr(Mnemonic.pmull2, q1, Vr(0,5,H__Q), q30, Vr(5,5,B__D), Vr(16,5,B__D))),
                    invalid,

                    Mask(30, 1,
                        Instr(Mnemonic.uaddl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uaddl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1,
                        Instr(Mnemonic.uaddw, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uaddw2, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_))),
                    Mask(30, 1, 
                        Instr(Mnemonic.usubl, q1,Vr(0,5,BHS_), q30, Vr(5,5,HSD_),Vr(16,5,HSD_)),
                        Instr(Mnemonic.usubl2, q1,Vr(0,5,BHS_), q30, Vr(5,5,HSD_),Vr(16,5,HSD_))),
                    Mask(30, 1,
                        Instr(Mnemonic.usubw, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_)),
                        Instr(Mnemonic.usubw2, q1, Vr(0, 5, HSD_), Vr(5, 5, HSD_), q30, Vr(16, 5, BHS_))),

                    Mask(30, 1,
                        Instr(Mnemonic.raddhn, q30, Vr(0, 5, BHS_), q1, Vr(5, 5, HSD_), Vr(16, 5, HSD_)),
                        Instr(Mnemonic.raddhn2, q30, Vr(0, 5, BHS_), q1, Vr(5, 5, HSD_), Vr(16, 5, HSD_))),
                    Mask(30, 1, 
                        Instr(Mnemonic.uabal, q1, Vr(0,5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uabal2, q1, Vr(0,5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1,
                        Instr(Mnemonic.rsubhn, q30, Vr(0,5,BHS_), q1, Vr(5,5,HSD_), Vr(16,5,HSD_)),
                        Instr(Mnemonic.rsubhn2, q30, Vr(0,5,BHS_), q1, Vr(5,5,HSD_), Vr(16,5,HSD_))),
                    Mask(30, 1,
                        Instr(Mnemonic.uabdl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.uabdl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    Mask(30, 1, 
                        Instr(Mnemonic.umlal, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.umlal2, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vr(16,5,BHS_))),
                    invalid,
                    Mask(30, 1,
                        Instr(Mnemonic.umlsl, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_)),
                        Instr(Mnemonic.umlsl2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), Vr(16, 5, BHS_))),
                    invalid,

                    Mask(30, 1, 
                        Instr(Mnemonic.umull, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vr(16,5,BHS_)),
                        Instr(Mnemonic.umull2, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vr(16,5,BHS_))),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder AdvancedSIMDscalar2RegMisc;
            {
                AdvancedSIMDscalar2RegMisc = Mask(29, 1, "Advanced SIMD scalar two-register miscellaneous",
                    Mask(12, 5, // U=0 opcode
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=00111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=01111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=10111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11100"),
                        Mask(22, 2, // U=1 opcode=11101 size
                            Instr(Mnemonic.scvtf, S_0, S_5),
                            Instr(Mnemonic.scvtf, D_0, D_5),
                            Instr(Mnemonic.scvtf, S_0, S_5),
                            Instr(Mnemonic.scvtf, D_0, D_5)),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11111")),
                    Mask(12, 5, // U=1 opcode
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=00111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=01111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10100"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10101"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=10111"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11000"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11001"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11010"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11011"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11100"),
                        Mask(22, 2, // U=1 opcode=11101 size
                            Instr(Mnemonic.ucvtf, S_0, S(5,5)),
                            Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11101 size=01"),
                            Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11101 size=10"),
                            Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11101 size=11")),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11110"),
                        Nyi("AdvancedSIMDscalar2RegMisc U=1 opcode=11111")));
            }

            Decoder AdvancedSimdScalar3Different;
            {
                AdvancedSimdScalar3Different = Mask(29, 1, "  Advanced SIMD scalar three different U",
                    Sparse(12, 4, "  U=0",
                        invalid,
                        (0b1001, Nyi("SQDMLAL, SQDMLAL2 (vector)")),
                        (0b1011, Nyi("SQDMLSL, SQDMLSL2 (vector)")),
                        (0b1101, Nyi("SQDMULL, SQDMULL2 (vector)"))),
                    invalid);
            }
            Decoder AdvancedSimdScalar3Same;
            {
                AdvancedSimdScalar3Same = Mask(Bf((29, 1), (23, 1)), "  Advanced SIMD scalar three same Q:size",
                    Nyi("Advanced SIMD scalar three same 0:0"),
                    Nyi("Advanced SIMD scalar three same 0:1"),
                    Nyi("Advanced SIMD scalar three same 1:0"),
                    Nyi("Advanced SIMD scalar three same 1:1"));
            }


            Decoder FloatingPointDataProcessing1src;
            {
                FloatingPointDataProcessing1src = Mask(Bf((31, 1), (29, 1), (22, 2)), "  FloatingPointDataProcessing1src",
                    Sparse(15, 6, "  00",
                        Nyi("FloatingPointDataProcessing1src M:S:Type=00 00"),
                        (0b000000, Instr(Mnemonic.fmov, S_0, S_5)),
                        (0b000001, Instr(Mnemonic.fabs, S_0, S_5)),
                        (0b000010, Instr(Mnemonic.fneg, S_0, S_5)),
                        (0b000011, Instr(Mnemonic.fsqrt, S_0, S_5)),
                        (0b000101, Instr(Mnemonic.fcvt, D_0, S_5)),
                        (0b000111, Instr(Mnemonic.fcvt, H_0, S_5))
                        ),
                    Sparse(15, 6, "  01",
                        Nyi("FloatingPointDataProcessing1src M:S:Type=00 01"),
                        (0b000000, Instr(Mnemonic.fmov, D_0, D_5)),
                        (0b000001, Instr(Mnemonic.fabs, D_0, D_5)),
                        (0b000010, Instr(Mnemonic.fneg, D_0, D_5)),
                        (0b000011, Instr(Mnemonic.fsqrt,D(0,5), D_5)),
                        (0b000100, Instr(Mnemonic.fcvt, S_0, D_5)),
                        (0b000111, Instr(Mnemonic.fcvt, H_0, D_5))
                        ),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=00 10"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=00 11"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=01 00"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=01 01"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=01 10"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=01 11"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=10 00"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=10 01"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=10 10"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=10 11"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=11 00"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=11 01"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=11 10"),
                    Nyi("FloatingPointDataProcessing1src M:S:Type=11 11"));
            }

            Decoder FloatingPointDataProcessing2src;
            {
                FloatingPointDataProcessing2src = Mask(Bf((31,1),(29,1),(22,2)),   // M:S:Type
                    Mask(12, 4,            // M:S:Type=0 0 00 opcode
                        Instr(Mnemonic.fmul, S_0, S_5, S_16),
                        Instr(Mnemonic.fdiv, S_0, S_5, S_16),
                        Instr(Mnemonic.fadd, S_0, S_5, S_16),
                        Instr(Mnemonic.fsub, S_0, S_5, S_16),

                        Instr(Mnemonic.fmax, S_0, S_5, S_16),
                        Instr(Mnemonic.fmin, S_0, S_5, S_16),
                        Instr(Mnemonic.fmaxnm, S_0, S_5, S_16),
                        Instr(Mnemonic.fnmul, S_0, S_5, S_16),

                        Instr(Mnemonic.fnmul, S_0, S_5, S_16),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(12, 4,            // M:S:Type=0 0 01 opcode
                        Instr(Mnemonic.fmul, D_0, D_5, D_16),
                        Instr(Mnemonic.fdiv, D_0, D_5, D_16),
                        Instr(Mnemonic.fadd, D_0, D_5, D_16),
                        Instr(Mnemonic.fsub, D_0, D_5, D_16),

                        Instr(Mnemonic.fmax, D_0, D_5, D_16),
                        Instr(Mnemonic.fmin, D_0, D_5, D_16),
                        Instr(Mnemonic.fmaxnm, D_0, D_5, D_16),
                        Instr(Mnemonic.fnmul, D_0, D_5, D_16),

                        Instr(Mnemonic.fnmul, D_0, D_5, D_16),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    invalid,
                    Mask(12, 4,            // M:S:Type=0 0 11 opcode
                        Instr(Mnemonic.fmul, H_0, H_5, H_16),
                        Instr(Mnemonic.fdiv, H_0, H_5, H_16),
                        Instr(Mnemonic.fadd, H_0, H_5, H_16),
                        Instr(Mnemonic.fsub, H_0, H_5, H_16),

                        Instr(Mnemonic.fmax, H_0, H_5, H_16),
                        Instr(Mnemonic.fmin, H_0, H_5, H_16),
                        Instr(Mnemonic.fmaxnm, H_0, H_5, H_16),
                        Instr(Mnemonic.fnmul, H_0, H_5, H_16),

                        Instr(Mnemonic.fnmul, H_0, H_5, H_16),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),

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

            Decoder FloatingPointImmediate;
            {
                FloatingPointImmediate = Mask(Bf((31,1),(29,1)),    // M:S
                    Select((5, 5), n => n == 0,   // M:S=00 imm5=00000
                        Mask(22, 2,   // M:S=00 imm5=00000
                            Instr(Mnemonic.fmov, S_0,If32(13,8)),
                            Instr(Mnemonic.fmov, D_0,If64(13,8)),
                            invalid,
                            Instr(Mnemonic.fmov, H_0,If16(13,8))),
                        invalid),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder FloatingPointCompare;
            {
                FloatingPointCompare = Mask(Bf((31, 1), (29, 1)),    // M:S
                    Select((14,2),n=>n!=0,
                        invalid,
                        Mask(Bf((22,2),(3,2)),  // M:S=00 type:opcode
                            Instr(Mnemonic.fcmp,  S_5, S_16),
                            Instr(Mnemonic.fcmp,  S_5,Sz(16,5)),
                            Instr(Mnemonic.fcmpe, S_5, S_16),
                            Instr(Mnemonic.fcmpe, S_5,Sz(16,5)),
                            Instr(Mnemonic.fcmp,  D_5, D_16),
                            Instr(Mnemonic.fcmp,  D_5,Dz(16,5)),
                            Instr(Mnemonic.fcmpe, D_5, D_16),
                            Instr(Mnemonic.fcmpe, D_5,Dz(16,5)),
                            invalid,
                            invalid,
                            invalid,
                            invalid,
                            Instr(Mnemonic.fcmp,  H_5, H_16),
                            Instr(Mnemonic.fcmp,  H_5,Hz(16,5)),
                            Instr(Mnemonic.fcmpe, H_5, H_16),
                            Instr(Mnemonic.fcmpe, H_5,Hz(16,5)))),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder FloatingPointCondSelect;
            {
                FloatingPointCondSelect = Mask(Bf((31, 1), (29, 1)),   // M:S
                    Mask(22, 2,  // M:S=00 type
                        Instr(Mnemonic.fcsel, S_0, S_5, S_16,C(12,4)),
                        Instr(Mnemonic.fcsel, D_0, D_5, D_16,C(12,4)),
                        invalid,
                        Instr(Mnemonic.fcsel, H_0, H_5, H_16,C(12,4))),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder FloatingPointCondCompare;
            {
                FloatingPointCondCompare = Mask(Bf((31, 1), (29, 1)), "  Floating point cond compare",   // M:S
                    Mask(22, 2, // M:S=00 type
                        Nyi("FloatingPointCondCompare M:S=00 type=00"),
                        Nyi("FloatingPointCondCompare M:S=00 type=01"),
                        invalid,    // M:S=00 type=00
                        Nyi("FloatingPointCondCompare M:S=00 type=11")),
                    invalid,
                    invalid,
                    invalid);
            }

            var asSxtl = new VectorData[16]
            {
                    VectorData.Invalid,
                    VectorData.I8,
                    VectorData.I16,
                    VectorData.I16,

                    VectorData.I32,
                    VectorData.I32,
                    VectorData.I32,
                    VectorData.I32,

                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,

                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
            };

            var asSxtlWide = new VectorData[16]
            {
                    VectorData.Invalid,
                    VectorData.I16,
                    VectorData.I32,
                    VectorData.I32,

                    VectorData.I64,
                    VectorData.I64,
                    VectorData.I64,
                    VectorData.I64,

                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,

                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
                    VectorData.Invalid,
            };

            var arrBHSD = new VectorData[]
            {
                VectorData.Invalid,
                VectorData.I8,
                VectorData.I16,
                VectorData.I16,

                VectorData.I32,
                VectorData.I32,
                VectorData.I32,
                VectorData.I32,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
            };

            // Arrangement used in some xxshrn instuctions
            var arrShrnHSD = new VectorData[16]
            {
                VectorData.Invalid,
                VectorData.I16,
                VectorData.I32,
                VectorData.I32,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
            };

            // Arrangement used in some xxshrn instuctions
            var arrBHS_ = new VectorData[16]
            {
                VectorData.Invalid,
                VectorData.I8,
                VectorData.I16,
                VectorData.I16,

                VectorData.I32,
                VectorData.I32,
                VectorData.I32,
                VectorData.I32,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
            };

            var arrHSD_ = new VectorData[16]
            {
                VectorData.Invalid,
                VectorData.I16,
                VectorData.I32,
                VectorData.I32,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,

                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.Invalid,
            };

            var arr_HSD = new VectorData[16]
            {
                VectorData.Invalid,
                VectorData.Invalid,
                VectorData.I16,
                VectorData.I16,

                VectorData.I32,
                VectorData.I32,
                VectorData.I32,
                VectorData.I32,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,

                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
                VectorData.I64,
};

            Decoder AdvancedSimdShiftByImm;
            {
                static bool uxtlAlias(uint u)
                {
                    var immb = (u >> 16) & 7;
                    var immh = (u >> 19) & 0xF;
                    return immb == 0 && Bits.BitCount(immh) == 1;
                }

                AdvancedSimdShiftByImm = Select((19, 4), IsZero, "  Advanced SIMD shift by immediate",
                    invalid,
                    Mask(29, 1,
                        Sparse(11,5, "  U=0",
                            invalid,
                            (0b00000, Instr(Mnemonic.sshr, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD),ShrnShift)),
                            (0b00010, Instr(Mnemonic.ssra, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b00100, Instr(Mnemonic.srshr, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b00110, Instr(Mnemonic.srsra, q30, Vrc(0,19,4, arrBHSD),Vrc(5,19,4,arrBHSD), ShrnShift)),
                            (0b01010, Instr(Mnemonic.shl, q30, Vrc(0,19,4, arrBHSD),Vrc(5,19,4,arrBHSD), UqshlShift)),
                            (0b01110, Instr(Mnemonic.sqshl, q30, Vrc(0,19,4, arrBHSD),Vrc(5,19,4,arrBHSD), UqshlShift)),
                            (0b01111, invalid),
                            (0b10000, Instr(Mnemonic.shrn, q30, Vrc(0, 19, 4, arrBHSD),q1,Vrc(5, 19, 4, arrShrnHSD),ShrnShift)),
                            (0b10001, Mask(30, 1, "  10001",
                                Instr(Mnemonic.rshrn, q30, Vrc(0,19,4, arrBHSD), q1,Vrc(5,19,4, arrShrnHSD),ShrnShift),
                                Instr(Mnemonic.rshrn2, q30, Vrc(0,19,4, arrBHSD), q1,Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10010, Mask(30, 1, "  10010",
                                Instr(Mnemonic.sqshrn, q30, Vrc(0, 19, 4, arrBHSD), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.sqshrn2, q30, Vrc(0, 19, 4, arrBHSD), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10011, Mask(30, 1, "  10011",
                                Instr(Mnemonic.sqrshrn, q30, Vrc(0,19,4, arrBHSD), q1, Vrc(5,19,4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.sqrshrn2, q30, Vrc(0,19,4, arrBHSD), q1, Vrc(5,19,4, arrShrnHSD), ShrnShift))),
                            (0b10100, Select((16, 3), n => n == 0, "  immh",
                                Instr(Mnemonic.sxtl, q1, Vrc(0, 19,4, asSxtlWide), q30,Vrc(5, 19,4, asSxtl)),
                                Mask(30, 1, "  sshll",
                                    Instr(Mnemonic.sshll, q1, Vrc(0,19,4,arrHSD_), q30, Vrc(5,19,4,arrBHS_), UqshlShift),
                                    Instr(Mnemonic.sshll2, q1, Vrc(0, 19,4,arrHSD_), q30, Vrc(5, 19, 4, arrBHS_), UqshlShift)))),
                            (0b11100, Instr(Mnemonic.scvtf, q30, Vrc(0,19,4, arr_HSD), Vrc(5,19,4, arr_HSD), fbits)),
                            (0b11111, Instr(Mnemonic.fcvts, x("(vector, fixed-point)")))
                                ),
                        Sparse(11, 5, "  U=1",
                            Nyi("AdvancedSimdShiftByImm U=1"),
                            (0b00000, Instr(Mnemonic.ushr, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD),ShrnShift)),
                            (0b00010, Instr(Mnemonic.usra, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b00100, Instr(Mnemonic.urshr, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b00110, Instr(Mnemonic.ursra, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b01000, Instr(Mnemonic.sri, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b01010, Instr(Mnemonic.sli, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), ShrnShift)),
                            (0b01100, Instr(Mnemonic.sqshlu, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), UqshlShift)),
                            (0b01110, Instr(Mnemonic.uqshl, q30, Vrc(0, 19, 4, arrBHSD), Vrc(5, 19, 4, arrBHSD), UqshlShift)),

                            (0b10000, Mask(30, 1, 
                                Instr(Mnemonic.sqrshrun, q30, Vrc(0, 19, 4, arrBHS_), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.sqrshrun, q30, Vrc(0, 19, 4, arrBHS_), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10001, Mask(30, 1,
                                Instr(Mnemonic.sqrshrun, q30, Vrc(0,19,4, arrBHS_), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.sqrshrun2, q30, Vrc(0,19,4, arrBHS_), q1, Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10010, Mask(30, 1,
                                Instr(Mnemonic.uqshrn, q0, Vrc(0, 19,4, arrBHS_), Vrc(5, 19,4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.uqshrn2, q0, Vrc(0, 19, 4, arrBHS_), Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10011, Mask(30, 1,
                                Instr(Mnemonic.uqrshrn, q0, Vrc(0, 19, 4, arrBHS_), Vrc(5, 19, 4, arrShrnHSD), ShrnShift),
                                Instr(Mnemonic.uqrshrn2, q0, Vrc(0, 19, 4, arrBHS_), Vrc(5, 19, 4, arrShrnHSD), ShrnShift))),
                            (0b10100, Select(uxtlAlias,
                                Mask(30, 1,
                                    Instr(Mnemonic.uxtl, q1,Vr(0,5,HSD_,20),q30,Vr(5,5,BHS_,20)),
                                    Instr(Mnemonic.uxtl2, q30,Vr(0,5,HSD_,20),Vr(5,5,BHS_128,20))),
                                Mask(30, 1,       // U=1 Q
                                    Instr(Mnemonic.ushll, q1, Vrc(0, 19, 4, arrShrnHSD), q30, Vrc(5, 19, 4, arrBHS_), UqshlShift),
                                    Instr(Mnemonic.ushll2, q1, Vrc(0, 19, 4, arrShrnHSD), q30, Vrc(5, 19, 4, arrBHS_), UqshlShift)))),
                            (0b11100, Instr(Mnemonic.ucvtf, q30, Vrc(0,19,4, arr_HSD), Vrc(5,19,4, arr_HSD), fbits)),
                            (0b11111, Instr(Mnemonic.fcvtzu, x("(vector, fixed-point")))
                            )));
            }

            Decoder AdvancedSimdModifiedImm;
            {
                var sShifts = new int[] { 0, 8, 16, 24 };
                var modImm64 = Is64(16, 3, 5, 5, 29, 1, 12, 4);
                var modFloat32 = If32(16, 3, 5, 5);
                var shiftImm = ShiftImm(13, 2, sShifts, Mnemonic.lsl);
                var shiftImmOnes = ShiftImm(13, 2, sShifts, Mnemonic.msl);

                var movShift32Imm = Instr(Mnemonic.movi, q30, Vr(0, 5, SSSS), modImm64, shiftImm);
                var movShift16Imm = Instr(Mnemonic.movi, q30, Vr(0, 5, HHHH), modImm64, shiftImm);
                var movi8 = Instr(Mnemonic.movi, q30, Vr(0, 5, BBBB), modImm64);
                var orrVec32Imm = Instr(Mnemonic.orr, q30, Vr(0, 5, SSSS), modImm64, shiftImm);
                var orrVec16Imm = Instr(Mnemonic.orr, q30, Vr(0, 5, HHHH), modImm64, shiftImm);
                var moviShiftOnes32Imm = Instr(Mnemonic.mvni, q30, Vr(0, 5, SSSS), modImm64, shiftImm);
                var mvniShift32Imm = Instr(Mnemonic.mvni, q30, Vr(0, 5, SSSS), modImm64, shiftImm);
                var bicVec32Imm = Instr(Mnemonic.bic, q30, Vr(0, 5, SSSS), modImm64, shiftImm);
                var mvniShift16Imm = Instr(Mnemonic.mvni, q30, Vr(0, 5, HHHH), modImm64, shiftImm);
                var bicVec16Imm = Instr(Mnemonic.bic, q30, Vr(0, 5, HHHH), modImm64, shiftImm);
                var mvniShiftOnes32Imm = Instr(Mnemonic.mvni, q30, Vr(0, 5, SSSS), modImm64, shiftImmOnes);
                var fmovHalfPrecVariant = Nyi("FMOV (vector, immediate) - Half-precision variant");
                var fmovSingleVariant = Instr(Mnemonic.fmov, q30, Vr(0, 5, SSSS, 30), modFloat32);
                var fmovDoubleVariant = Nyi("FMOV (vector, immediate) - Double-precision variant");
                AdvancedSimdModifiedImm = Mask(Bf((29, 2), (11, 1)), "  Advanced Simd Modified Immediate Q:op:op2",
                    Mask(12, 4, "  0:0:0",
                        movShift32Imm,
                        orrVec32Imm,
                        movShift32Imm,
                        orrVec32Imm,

                        movShift32Imm,
                        orrVec32Imm,
                        movShift32Imm,
                        orrVec32Imm,

                        movShift16Imm,
                        orrVec16Imm,
                        movShift16Imm,
                        orrVec16Imm,
                        
                        moviShiftOnes32Imm,
                        moviShiftOnes32Imm,
                        movi8,
                        fmovSingleVariant),

                    Mask(12, 4, "  0:0:1",
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
                        fmovHalfPrecVariant),
                    Mask(12, 4, "  0:1:0",
                        mvniShift32Imm,
                        bicVec32Imm,
                        mvniShift32Imm,
                        bicVec32Imm,

                        mvniShift32Imm,
                        bicVec32Imm,
                        mvniShift32Imm,
                        bicVec32Imm,

                        movShift16Imm,
                        bicVec16Imm,
                        movShift16Imm,
                        bicVec16Imm,

                        mvniShiftOnes32Imm,
                        mvniShiftOnes32Imm,
                        Nyi("MOVI - 64-bit scalar variant"),
                        invalid),
                    invalid,

                    Mask(12, 4, "  1:0:0",
                        movShift32Imm,
                        orrVec32Imm,
                        movShift32Imm,
                        orrVec32Imm,

                        movShift32Imm,
                        orrVec32Imm,
                        movShift32Imm,
                        orrVec32Imm,

                        movShift16Imm,
                        orrVec16Imm,
                        movShift16Imm,
                        orrVec16Imm,

                        moviShiftOnes32Imm,
                        moviShiftOnes32Imm,
                        movi8,
                        fmovSingleVariant),
                    Mask(12, 4, "  1:0:1",
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
                        fmovHalfPrecVariant),
                    Mask(12, 4, "  0:1:0",
                        mvniShift32Imm,
                        bicVec32Imm,
                        mvniShift32Imm,
                        bicVec32Imm,

                        mvniShift32Imm,
                        bicVec32Imm,
                        mvniShift32Imm,
                        bicVec32Imm,

                        mvniShift16Imm,
                        bicVec16Imm,
                        mvniShift16Imm,
                        bicVec16Imm,

                        mvniShiftOnes32Imm,
                        mvniShiftOnes32Imm,
                        Instr(Mnemonic.movi, q30, Vr(0, 5, DDDD), modImm64),
                        fmovDoubleVariant),
                    invalid);

            }

            Decoder AdvancedSimd2RegMisc;  // C4-298
            {
                var sShifts = new int[] { 8, 16, 32, 64 };
                AdvancedSimd2RegMisc = Mask(29, 1, "  Advanced SIMD two-register miscellaneous",
                    Mask(12, 5,
                        Instr(Mnemonic.rev64, q30, Vr(0,5,BHS_), Vr(5, 5, BHS_)),
                        Instr(Mnemonic.rev16, q30, Vr(0,5,B___), Vr(5,5,B___)),
                        Instr(Mnemonic.saddlp, q30, Vr(0,5,HSD_), Vr(5,5,BHS_)),
                        Instr(Mnemonic.suqadd, q30, Vr(0,5, BHSD), Vr(5, 5, BHSD)),

                        Instr(Mnemonic.cls, q30, Vr(0,5, BHS_), Vr(5,5, BHS_)),
                        Instr(Mnemonic.cnt, q30, Vr(0,5, B___), Vr(5,5, B___)),
                        Instr(Mnemonic.sadalp, q30, Vr(0,5,HSD_),Vr(5,5, BHS_)),
                        Instr(Mnemonic.sqabs, q30, Vr(0,5,BHSD), Vr(5,5,BHSD)),

                        Instr(Mnemonic.cmgt, q30, Vr(0, 5, SSSS), Vr(5, 5, SSSS)),
                        Instr(Mnemonic.cmeq, q30, Vr(0, 5, SSSS), Vr(5, 5, SSSS)),
                        Instr(Mnemonic.cmlt, q30, Vr(0, 5, SSSS), Vr(5, 5, SSSS)),
                        Instr(Mnemonic.abs, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD)),

                        Mask(23, 1, "  opcode=01100",
                            invalid,
                            Instr(Mnemonic.fcmgt, x("(zero)"))),
                        Mask(23, 1, "  opcode=01101",
                            invalid,
                            Instr(Mnemonic.fcmeq, x("(zero)"))),
                        Mask(23, 1, "  opcode=01110",
                            invalid,
                            Instr(Mnemonic.fcmlt, x("(zero)"))),
                        Mask(23, 1, "  opcode=01111",
                            invalid,
                            Instr(Mnemonic.fabs, x("(vector)"))),

                        invalid,
                        invalid,
                        Mask(30, 1, "  opcode=10011",
                            Instr(Mnemonic.xtn, q30, Vr(0, 5, BHS_), q1, Vr(5, 5, HSD_)),
                            Instr(Mnemonic.xtn2, q30, Vr(0, 5, BHS_), q1, Vr(5, 5, HSD_))),
                        invalid,

                        Mask(30, 1, "  opcode=10100",
                            Instr(Mnemonic.sqxtn, q30, Vr(0, 5, BHS_), q1, Vr(5,5,HSD_)),
                            Instr(Mnemonic.sqxtn2, q30, Vr(0, 5, BHS_), q1, Vr(5,5,HSD_))),
                        invalid,
                        Mask(23, 1, "  opcode=10110",
                            Nyi("FCVTN, FCVTN2"),
                            invalid),
                        Mask(23, 1, "  opcode=10111",
                            Nyi("FCVTL, FCVTL2"),
                            invalid),

                        Mask(23, 1, "  opcode=11000",
                            Nyi("FRINTN (vector)"),
                            Nyi("FRINTP (vector)")),
                        Mask(23, 1, "  opcode=11001",
                            Nyi("FRINTM (vector)"),
                            Nyi("FRINTZ (vector)")),
                        Mask(23, 1, "  opcode=11010",
                            Nyi("FCVTNS (vector)"),
                            Nyi("FCVTPS (vector)")),
                        Mask(22, 2,
                            Instr(Mnemonic.fcvtms, x("vector")),
                            Instr(Mnemonic.fcvtms, x("vector")),
                            Instr(Mnemonic.fcvtzs, VectorData.F32, q30, V(0, 5), V(5, 5)),
                            Instr(Mnemonic.fcvtzs, VectorData.F64, q30, V(0, 5), V(5, 5))),

                        Mask(23, 1, "  opcode=11100",
                            Nyi("FCVTAS (vector)"),
                            Instr(Mnemonic.urecpe, q30, Vr(0,5,S_S_), Vr(5,5,S_S_))),
                        Mask(22, 2,
                            Instr(Mnemonic.scvtf, VectorData.I32, q30, Vr(0,5,SD__), Vr(5,5,SD__)),
                            Instr(Mnemonic.scvtf, VectorData.I64, q30, Vr(0,5,SD__), Vr(5,5,SD__)),
                            Instr(Mnemonic.frecpe, x("")),
                            Instr(Mnemonic.frecpe, x(""))),
                        invalid,
                        invalid),
                    Mask(12, 5, "  AdvancedSimd2RegMisc U=1",
                        Instr(Mnemonic.rev32, q30, Vr(0,5,BH__), Vr(5,5,BH__)),
                        invalid,
                        Instr(Mnemonic.uaddlp, q30, Vr(0,5, HSD_), Vr(5, 5, BHS_)),
                        Instr(Mnemonic.usqadd, q30, Vr(0, 5, BHSD), Vr(5,5,BHSD)),

                        Instr(Mnemonic.clz, q30, Vr(0, 5, BHS_), Vr(0, 5, BHS_)),
                        Mask(22, 2,
                            Instr(Mnemonic.not, q30, Vr(0, 5, BBBB), Vr(5, 5, BBBB)),
                            Instr(Mnemonic.rbit, q30, Vr(0,5, BBBB), Vr(5,5,BBBB)),
                            invalid,
                            invalid),
                        Instr(Mnemonic.uadalp, q30,Vr(0,5, HSD_), Vr(5, 5, BHS_)),
                        Instr(Mnemonic.sqneg, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD)),

                        Instr(Mnemonic.cmge, q30, Vr(0,5,BHSD),Vr(5,5,BHSD), UImm(0)),
                        Instr(Mnemonic.cmle, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD), UImm(0)),
                        invalid,
                        Instr(Mnemonic.neg, q30, Vr(0, 5, BHSD), Vr(5, 5, BHSD)),

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        Mask(30, 1,
                            Instr(Mnemonic.sqxtun, q30, Vr(0,5,BHS_), q1, Vr(5,5,HSD_)),
                            Instr(Mnemonic.sqxtun2, q30, Vr(0,5,BHS_), q1, Vr(5,5,HSD_))),
                        Mask(30, 1, 
                            Instr(Mnemonic.shll, q1, Vr(0,5,HSD_), q30, Vr(5,5,BHS_), ShiftImm(22,2,sShifts)),
                            Instr(Mnemonic.shll2, q1, Vr(0, 5, HSD_), q30, Vr(5, 5, BHS_), ShiftImm(22, 2, sShifts))),

                        Mask(30, 1,
                            Instr(Mnemonic.uqxtn, q0, Vr(0,5,BHS_), Vr(5,5,HSD_)),
                            Instr(Mnemonic.uqxtn2, q0, Vr(0,5,BHS_), Vr(5,5,HSD_))),
                        invalid,
                        Mask(23, 1, "  opcode=10110",
                            Nyi("FCVTXN, FCVTXN2"),
                            invalid),
                        invalid,

                        Mask(23, 1, "  opcode=11000",
                            Instr(Mnemonic.frinta, x("(vector)")),
                            invalid),
                        Mask(23, 1, "  opcode=11001",
                            Instr(Mnemonic.frintx, x("(vector)")),
                            Instr(Mnemonic.frinti, x("(vector)"))),
                        Mask(23, 1, "  opcode=11010",
                            Instr(Mnemonic.fcvtnu, x("(vector)")),
                            Instr(Mnemonic.fcvtpu, x("(vector)"))),
                        Mask(23, 1, "  opcode=11011",
                            Instr(Mnemonic.fcvtmu, x("(vector)")),
                            Instr(Mnemonic.fcvtzu, x("(vector,integer)"))),

                        Mask(23, 1, "  opcode=11100",
                            Instr(Mnemonic.fcvtau, x("(vector)")),
                            Instr(Mnemonic.ursqrte, q30,Vr(0,5,SSSS),Vr(5,5,SSSS))),
                        Mask(23, 1, "  opcode=11101",
                            Instr(Mnemonic.ucvtf, VectorData.I32, q30, V(0, 5), V(5, 5)),
                            Instr(Mnemonic.frsqrte, x("(vector,integer)"))),
                        invalid,
                        Mask(23, 1, "  opcode=11111",
                            invalid,
                            Instr(Mnemonic.fsqrt, x("(vector)")))));
            }

            Decoder AdvancedSimdAcrossLanes;
            {
                AdvancedSimdAcrossLanes = Mask(12, 5, "  Advanced SIMD across lanes",
                    invalid,
                    invalid,
                    invalid,
                    Mask(29, 1, "  00011",
                        Instr(Mnemonic.saddlv, Vs(0,5,Vs_HSD_),q30,Vr(5,5,BHS_)),
                        Instr(Mnemonic.uaddlv, Vs(0, 5, Vs_HSD_), q30, Vr(5, 5, BHS_))),

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Mask(29, 1,    // opcode=01010 U=0 size
                        Instr(Mnemonic.smaxv, q30, Vs(0, 5, Vs_BHS_), Vr(5, 5, BHS_)),
                        Instr(Mnemonic.umaxv, q30, Vs(0, 5, Vs_BHS_), Vr(5, 5, BHS_))),
                    invalid,

                    Mask(29, 1, "  01100",
                        Mask(22, 2, "  U=0",
                            Instr_8_2(Mnemonic.fmaxnmv, x("FMAXNMV - Half-precision variant ")),
                            invalid,
                            Instr_8_2(Mnemonic.fminnmv, x("FMINNMV - Half-precision variant ")),
                            invalid),
                        Mask(23, 1, "  U=1",
                            Instr(Mnemonic.fmaxnmv, x("FMAXNMV - Single-precision and double-precision variant")),
                            Instr(Mnemonic.fminnmv, x("FMINNMV - Single-precision and double-precision variant")))),
                    invalid,
                    invalid,
                    Mask(29, 1, "  01111",
                        Mask(22, 2, "  U=0",
                            Instr(Mnemonic.fmaxv, x("FMAXV - Half-precision variant ")),
                            invalid,
                            Instr(Mnemonic.fminv, x("FMINV - Half-precision variant ")),
                            invalid),
                        Mask(23, 1, "  U=1",
                            Instr(Mnemonic.fmaxv, x("FMAXV - Single-precision and double-precision variant")),
                            Instr(Mnemonic.fminv, x("FMINV - Single-precision and double-precision variant")))),

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
                    Mask(29, 1, "  11010",
                        Instr(Mnemonic.sminv, q30, Vs(0, 5, Vs_BHS_), Vr(5, 5, BHS_)),
                        Instr(Mnemonic.uminv, q30, Vs(0, 5, Vs_BHS_), Vr(5, 5, BHS_))),
                    Mask(29, 1, "  11011",
                        Mask(22, 2,   // opcode=11011 U=0 size
                            Instr(Mnemonic.addv, q30, B(0, 5), Vr(5, 5, BHS_)),
                            Instr(Mnemonic.addv, q30, H(0, 5), Vr(5, 5, BHS_)),
                            Instr(Mnemonic.addv, q30, S_0, Vr(5, 5, BHS_)),
                            invalid),
                        invalid),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            var dupVectorElement = Instr(Mnemonic.dup, q30, Vrs(0, 5, 16, 5, false, false, true),  Vrs(5, 5, 16, 5, true, false));
            var dupScalarElement = Instr(Mnemonic.mov, q30, Vrs(0, 5, 16, 5, false, false, false), Vrs(5, 5, 16, 5, true, false));

            Decoder AdvancedSimdCopy;
            {
                bool isMovAlias(uint u) =>
                    (u & 0b01111) == 0b01000 ||
                    (u & 0b00111) == 0b00100;

                AdvancedSimdCopy = Select((16, 4), IsZero, "  Advanced SIMD Copy",
                    invalid,
                    Mask(29, 2,    // Q:op
                        Sparse(11, 4,  "  op=00", 
                            Nyi("AdvancedSIMDcopy Q:op=00"),
                            (0b0000, dupVectorElement),
                            (0b0101, Instr(Mnemonic.smov, q30, W_0, VrxSmovUmov)),
                            (0b0111, Select((16, 5), isMovAlias,
                                Instr(Mnemonic.mov, q30, W_0,Vrs(5,5,16,5,true, false)),
                                Instr(Mnemonic.umov, q30, W_0,Vrs(5,5,16,5,true, false))))),
                        Nyi("AdvancedSIMDcopy Q:op=01"),
                        Sparse(11, 4, "  op=10",
                            Nyi("AdvancedSIMDcopy Q:op=10"),
                            (0b0000, dupVectorElement),
                            (0b0001, Select((16, 4), n => n == 0b1000,
                                Instr(Mnemonic.dup, q30,Vrs(0,5,16,5,false,false),X_5),
                                Instr(Mnemonic.dup, q30,Vrs(0,5,16,5,false,false),W_5))),
                            (0b0011, Select((16, 4), n => n == 0b1000,
                                Instr(Mnemonic.mov, VrxIns(0), X_5),
                                Instr(Mnemonic.mov, VrxIns(0), W_5))),
                            (0b0101, Instr(Mnemonic.smov, q30, X_0, VrxSmovUmov)),
                            (0b0111, Select((16, 5), isMovAlias, "  imm4=0111",
                                Instr(Mnemonic.mov, q30, X_0, Vrs(5, 5, 16, 5, true, false)),
                                Instr(Mnemonic.umov, x(""))))
                            ),
                        Instr(Mnemonic.mov, q30,Vrs(0,5,16,5,true,false),Vrs(5,5,11,4,true,true))));
            }

            Decoder AdvancedSimdExtract = Select((22, 2), IsZero,
                Instr(Mnemonic.ext, q30, Vr(0, 5, BBBB), Vr(5, 5, BBBB), Vr(16, 5, BBBB), U(11, 4, PrimitiveType.Byte)),
                invalid);


            Decoder AdvancedSimdTableLookup;
            {
                AdvancedSimdTableLookup = Select((22, 2), IsZero, "  Advanced SIMD table lookup",
                    Mask(12, 3, // len:op
                        Instr(Mnemonic.tbl, q30,Vr(0,5,BBBB),q1,Vmr(5,5,1,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbx, q30,Vr(0,5,BBBB),q1,Vmr(5,5,1,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbl, q30,Vr(0,5,BBBB),q1,Vmr(5,5,2,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbx, q30,Vr(0,5,BBBB),q1,Vmr(5,5,2,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbl, q30,Vr(0,5,BBBB),q1,Vmr(5,5,3,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbx, q30,Vr(0,5,BBBB),q1,Vmr(5,5,3,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbl, q30,Vr(0,5,BBBB),q1,Vmr(5,5,4,BBBB), q30, Vr(16,5,BBBB)),
                        Instr(Mnemonic.tbx, q30,Vr(0,5,BBBB),q1,Vmr(5,5,4,BBBB), q30, Vr(16,5,BBBB))),
                    invalid);
            }

            Decoder AdvancedSimdScalar_x_IdxElem;
            {
                AdvancedSimdScalar_x_IdxElem = Mask(12, 4, "  Advanced SIMD scalar x indexed element",
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0001"),
                    invalid,
                    Mask(29, 1, "  opcode=0011",
                        Mask(30, 1, "  U=0",
                            Mask(22, 2, "  Q=0",
                                invalid,
                                Instr(Mnemonic.fnmadd, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.fnmadd, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.sqdmlal, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlal, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),
                        invalid),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0101"),
                    Mask(29, 1, "  opcode=0110",
                        Mask(30, 1, 
                            Instr(Mnemonic.smlsl, x("")),
                            Instr(Mnemonic.smlsl2, x(""))),
                        Mask(30, 1,
                            Instr(Mnemonic.umlsl, x("")),
                            Instr(Mnemonic.umlsl2, x("")))),
                    Mask(29, 1, "  opcode=0111",
                        Mask(30, 1,  "  U=0",
                            Mask(22, 2,  "  Q=0",
                                invalid,
                                Instr(Mnemonic.sqdmlsl, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlsl, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.sqdmlsl2, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlsl2, Vs(0, 5, Vs__SD_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),
                        invalid),

                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1001"),
                    invalid,
                    Mask(29, 1, "  opcode=1011",
                        Mask(22, 2, "  Q=0",
                            invalid,
                            Instr(Mnemonic.sqdmull, Vs(0,5,Vs__SD_), Vs(5,5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.sqdmull, Vs(0,5,Vs__SD_), Vs(5,5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid),
                        invalid),

                    Mask(29, 1, "  opcode=1100",
                        Mask(22, 2, "  Q=0",
                            invalid,
                            Instr(Mnemonic.sqdmulh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.sqdmulh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid),
                        invalid),
                    Mask(29, 1, "  opcode=1101",
                        Mask(22, 2, "  Q=0",
                            invalid,
                            Instr(Mnemonic.sqrdmulh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.sqrdmulh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid),
                        Mask(22, 2, "  Q=1",
                            invalid,
                            Instr(Mnemonic.sqrdmlah, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.sqrdmlah, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid)),
                    invalid,
                    Mask(29, 1, "  opcode=1111",
                        invalid,
                        Mask(22, 2,
                            invalid,
                            Instr_8_1(Mnemonic.sqrdmlsh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 4, _HS_, Bf((11,1),(20,2)))),
                            Instr_8_1(Mnemonic.sqrdmlsh, Vs(0, 5, Vs__HS_), Vs(5, 5, Vs__HS_), Vrx(16, 5, _HS_, Bf((11,1), (21,1)))),
                            invalid)));
            }

            Decoder DataProcessingScalarFpAdvancedSimd;
            {
                Decoder FloatingPointDecoders = Mask(10, 2, "  Floating point decoders",
                    Mask(12, 2,                              // op3=xxxxx??00
                        Mask(14, 2,                          // op3=xxx??0000
                            ConversionBetweenFpAndInt,          // op3=xxx000000
                            FloatingPointDataProcessing1src,    // op3=xxx010000
                            invalid,                            // op3=xxx100000
                            FloatingPointDataProcessing1src),   // op3=xxx110000
                        FloatingPointImmediate,                 // op3=xxxxx0100
                        FloatingPointCompare,                   // op3=xxxxx1000
                        FloatingPointImmediate),                // op3=xxxxx1100
                    FloatingPointCondCompare,                   // op3=xxxxxxx01
                    FloatingPointDataProcessing2src,            // op3=xxxxxxx10
                    FloatingPointCondSelect);                   // op3=xxxxxxx11

                Decoder FloatingPointDataProcessing3src = Mask(Bf((31,1), (29,1), (22,2)),  // M:S:type 
                    Mask(Bf((21,1),(15,1)),
                        Instr(Mnemonic.fmadd, S_0, S_5, S_16, S_10),
                        Instr(Mnemonic.fmsub, S_0, S_5, S_16, S_10),
                        Instr(Mnemonic.fnmadd, S_0, S_5, S_16, S_10),
                        Instr(Mnemonic.fnmsub, S_0, S_5, S_16, S_10)),
                    Mask(Bf((21,1),(15,1)),
                        Instr(Mnemonic.fmadd, D_0, D_5, D_16, D_10),
                        Instr(Mnemonic.fmsub, D_0, D_5, D_16, D_10),
                        Instr(Mnemonic.fnmadd, D_0, D_5, D_16, D_10),
                        Instr(Mnemonic.fnmsub, D_0, D_5, D_16, D_10)),
                    invalid,
                    Mask(Bf((21, 1), (15, 1)),
                        Instr(Mnemonic.fmadd, H_0, H_5, H_16,H(10,5)),
                        Instr(Mnemonic.fmsub, H_0, H_5, H_16,H(10,5)),
                        Instr(Mnemonic.fnmadd, H_0, H_5, H_16,H(10,5)),
                        Instr(Mnemonic.fnmsub, H_0, H_5, H_16,H(10,5))),
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

                Decoder CryptographicAES = Select((22, 2), IsZero,
                    Sparse(12, 5, "  CryptographicAES", invalid,
                        (0b00100, Instr(Mnemonic.aese, q1,Vr(0, 5, BBBB), Vr(5,5,BBBB))),
                        (0b00101, Instr(Mnemonic.aesd, q1, Vr(0,5, BBBB), Vr(5,5,BBBB))),
                        (0b00110, Instr(Mnemonic.aesmc, q1, Vr(0, 5, BBBB), Vr(5, 5, BBBB))),
                        (0b00111, Instr(Mnemonic.aesimc, q1, Vr(0,5, BBBB), Vr(5,5,BBBB)))),
                    invalid);

                Decoder Cryptographic2regSHA = Select((22, 2), IsZero,
                    Sparse(12, 5, "  Cryptographic2regSHA", invalid,
                        (0b00000, Instr(Mnemonic.sha1h, S_0, S_5)),
                        (0b00001, Instr(Mnemonic.sha1su1, Vr(0, 5, SSSS), Vr(5, 5, SSSS))),
                        (0b00010, Instr(Mnemonic.sha256su0, Vr(0, 5, SSSS), Vr(5, 5, SSSS)))),
                    invalid);

                Decoder Cryptographic3regSHA = Select((22, 2), IsZero,
                    Sparse(12, 3, "  Cryptographic3regSHA", invalid,
                        (0b000, Instr(Mnemonic.sha1c, q1, Q_0, S_5, Vr(16,5, SSSS))),
                        (0b001, Instr(Mnemonic.sha1p, x(""))),
                        (0b010, Instr(Mnemonic.sha1m, x(""))),
                        (0b011, Instr(Mnemonic.sha1su0, x(""))),
                        (0b100, Instr(Mnemonic.sha256h, x(""))),
                        (0b101, Instr(Mnemonic.sha256h2, x(""))),
                        (0b110, Instr(Mnemonic.sha256su1, q1, Vr(0, 5, SSSS), Vr(5, 5, SSSS), Vr(16, 5, SSSS)))),
                    invalid);

                Decoder AdvancedSimdScalarShiftByImmediate = Select((19, 4), IsZero,
                    invalid,
                    Mask(29, 1, 11, 5, "  Advanced SIMD scalar shift by immediate",
                        Nyi("sshr"),
                        invalid,
                        Nyi("ssra"),
                        invalid,

                        Nyi("srshr"),
                        invalid,
                        Instr(Mnemonic.srsra, x("by immediate")),
                        invalid,

                        invalid,
                        invalid,
                        Nyi("shl"),
                        invalid,

                        invalid,
                        invalid,
                        Nyi("sqshl (immediate)"),
                        invalid,

                        invalid,
                        invalid,
                        Nyi("sqshrn, sqshrn2"),
                        Nyi("sqrshrn, sqrshrn2"),

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Nyi("scvtf (vector, fixed-point)"),
                        invalid,
                        invalid,
                        Nyi("fcvtzs (vector, fixed-point)"),

                        // U:1:00000
                        Instr(Mnemonic.ushr, x("ushr - scalar")),
                        invalid,
                        Nyi("usra"),
                        invalid,

                        Instr(Mnemonic.urshr, Vrc(0, 19, 4, arrBHSD),Vrc(5, 19, 4, arrBHSD)),
                        invalid,
                        Nyi("ursra"),
                        invalid,

                        Nyi("sri"),
                        invalid,
                        Nyi("sli"),
                        invalid,

                        Nyi("sqshlu (immediate)"),
                        invalid,
                        Nyi("uqshl (immediate)"),
                        invalid,

                        Nyi("sqshrun, sqshrun2"),
                        Nyi("sqrshrun, sqrshrun2"),
                        Nyi("uqshrn, uqshrn2"),
                        Nyi("uqrshrn, uqrshrn2"),

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid,

                        Instr(Mnemonic.ucvtf, x("(vector, fixed-point)")),
                        invalid,
                        invalid,
                        Mask(21, 2, "  fcvtzu",
                            Instr(Mnemonic.fcvtzu, H_0, H_5),
                            Instr(Mnemonic.fcvtzu, S_0, S_5),
                            Instr(Mnemonic.fcvtzu, D_0, D_5),
                            Instr(Mnemonic.fcvtzu, D_0, D_5))));

                var AdvancedSimdVector_x_indexedElement = Mask(29, 1, "  Advanced SIMD vector x indexed element",
                    Mask(12, 4, "  U=0 opcode",
                        Mask(22, 2, "  opcode=0000 size",
                            invalid,
                            invalid,
                            Nyi("FMLAL,FMLAL2 by element"),
                            Nyi("FMLAL,FMLAL2 by element")),
                        Mask(22, 2, "  opcode=0001 size",
                            Nyi("FMLA (by element) - vector, half precision"),
                            invalid,
                            Nyi("FMLA (by element) - Vector, single-precision and double-precision variant"),
                            Nyi("FMLA (by element) - Vector, single - precision and double - precision variant")),

                        Mask(30, 1, "  opcode=0002",
                            Mask(22, 2, "  Q=0",
                                invalid,
                                Instr(Mnemonic.smlal, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                                Instr(Mnemonic.smlal, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,5, _HS_, Bf((11,1),(21,1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.smlal2, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                                Instr(Mnemonic.smlal2, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,5, _HS_, Bf((11,1),(21,1)))),
                                invalid)),
                        Mask(30, 1,
                            Mask(22, 2,
                                invalid,
                                Instr(Mnemonic.sqdmlal, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                                Instr(Mnemonic.sqdmlal, q1, Vr(0,5,_SD_), q30, Vr(5,5,_HS_), Vrx(16,5, _HS_, Bf((11,1),(21,1)))),
                                invalid),
                            Mask(22, 2,
                                invalid,
                                Instr(Mnemonic.sqdmlal2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlal2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),

                        Mask(23, 1, "  opcode=0100 size",
                            invalid,
                            Nyi("FMLSL,FMLS2 by element")),
                        Mask(22, 2, "  opcode=0101 size",
                            Instr(Mnemonic.fmls, x("(by element) - vector, half precision")),
                            invalid,
                            Instr(Mnemonic.fmls, x("(by element) - Vector, single-precision and double-precision variant")),
                            Instr(Mnemonic.fmls, x("(by element) - Vector, single - precision and double - precision variant"))),
                        Mask(30, 1, "  opcode=0110",
                            Mask(22, 2, "  Q=0",
                                invalid,
                                Instr(Mnemonic.smlsl, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.smlsl, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.smlsl2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.smlsl2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),
                        Mask(30, 1, "  sqdmlsl",
                            Mask(22, 2,
                                invalid,
                                Instr(Mnemonic.sqdmlsl, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlsl, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2,
                                invalid,
                                Instr(Mnemonic.sqdmlsl2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmlsl2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),

                        Instr(Mnemonic.mul, q30, Vr(0,5,_HS_), Vr(5,5,_HS_), Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                        Mask(22, 2, " opcode=1001 size",
                            Instr(Mnemonic.fmul, x("by element - vector, half precision")),
                            invalid,
                            Instr(Mnemonic.fmul, x("(by element) - Vector, single-precision and double-precision variant")),
                            Instr(Mnemonic.fmul, x("(by element) - Vector, single - precision and double - precision variant"))),
                        Mask(30, 1, "  opcode=0110",
                            Mask(22, 2, "  Q=0",
                                invalid,
                                Instr(Mnemonic.smull, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.smull, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.smull2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.smull2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),
                        Mask(30, 1, "  opcode=1011",
                            Mask(22, 2, "  Q=0",
                                invalid,
                                Instr(Mnemonic.sqdmull, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmull, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  Q=1",
                                invalid,
                                Instr(Mnemonic.sqdmull2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.sqdmull2, q1, Vr(0, 5, _SD_), q30, Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),

                        Mask(22, 2, "  sqdmulh by element",
                            invalid,
                            Instr(Mnemonic.sqdmulh, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.sqdmulh, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid),
                        Mask(22, 2, "  sqrdmulh by element",
                            invalid,
                            Instr(Mnemonic.sqrdmulh, q30, Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,4,_HS_,Bf((11,1),(20,2)))),
                            Instr(Mnemonic.sqrdmulh, q30, Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,5,_HS_,Bf((11,1),(21,1)))),
                            invalid),
                        Nyi("SDOT by element"),
                        invalid),
                    Mask(12, 4, "  U=1 opcode",
                        Mask(22, 2,  "  MLA by element, size",
                            invalid,
                            Instr(Mnemonic.mla, q30, Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,4,_HS_,Bf((11,1),(20,2)))),
                            Instr(Mnemonic.mla, q30, Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,5,_HS_,Bf((11,1),(21,1)))),
                            invalid),
                        Mask(22, 2, "  0001",
                            invalid,
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            invalid),
                        Mask(30, 1, "  0010",
                            Mask(22, 2, "  umlal by element",
                                invalid,
                                Instr(Mnemonic.umlal, q1, Vr(0,5, _SD_), q30, Vr(5,5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.umlal, q1, Vr(0,5, _SD_), q30, Vr(5,5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (21, 1)))),
                                invalid),
                            Mask(22, 2, "  umlal by element",
                                invalid,
                                Instr(Mnemonic.umlal2, q1, Vr(0,5, _SD_), q30, Vr(5,5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                                Instr(Mnemonic.umlal2, q1, Vr(0,5, _SD_), q30, Vr(5,5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (21, 1)))),
                                invalid)),
                        Mask(22, 2, "  0011",
                            invalid,
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            invalid),

                        Mask(22, 2, "  MLS by element, size",
                            invalid,
                            Instr(Mnemonic.mls, q1, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr(Mnemonic.mls, q1, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid),
                        Mask(22, 2, "  0101",
                            invalid,
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            invalid),
                        Mask(30, 1, "  0110",
                            Mask(22, 2, "  umlsl by element",
                                invalid,
                                Instr(Mnemonic.umlsl, q11, Vr(0,5,_SD_),Vr(5,5,_HS_),Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                                Instr(Mnemonic.umlsl, q11, Vr(0,5,_SD_),Vr(5,5,_HS_),Vrx(16,4, _HS_, Bf((11,1),(20,2)))),
                                invalid),
                            Mask(22, 2, "  umlsl by element",
                                invalid,
                                Instr(Mnemonic.umlsl2, q11, Vr(0,5,_SD_), Vr(5,5,_HS_), Vrx(16, 4, _HS_, Bf((11,1), (20,2)))),
                                Instr(Mnemonic.umlsl2, q11, Vr(0,5,_SD_), Vr(5,5,_HS_), Vrx(16, 4, _HS_, Bf((11,1), (20,2)))),
                                invalid)),
                        Mask(22, 2,  "  opc=0111",
                            invalid,
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            Instr_8_3(Mnemonic.fcmla, x("")),
                            invalid),

                        Mask(23,1,  "  opcode=1000",
                            invalid,
                            Nyi("FMLAL, FMLAL2 (by element) - FMLAL2 variant")),
                        Mask(22,2,  "  opcode=1001",
                            Instr_8_2(Mnemonic.fmulx, x("FMULX (by element) - Vector, half-precision variant")),
                            invalid,
                            Instr(Mnemonic.fmulx, x("FMULX (by element) - Vector, single-precision and double-precision variant")),
                            Instr(Mnemonic.fmulx, x("FMULX (by element) - Vector, single-precision and double-precision variant"))),
                        Mask(30, 1, "  opcode=1010",
                            Instr(Mnemonic.umull, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vrx(16,4,BHS_,Bf((11,1),(20,2)))),
                            Instr(Mnemonic.umull2, q1,Vr(0,5,HSD_),q30,Vr(5,5,BHS_),Vrx(16,4,BHS_,Bf((11,1),(20,2))))),
                        invalid,

                        Mask(23, 1, "  opcode=1100",
                            invalid,
                            Nyi("FMLSL, FMLSL2 (by element) - FMLSL2 variant")),
                        Mask(22, 2, "  opcode=1101",
                            invalid,
                            Instr_8_1(Mnemonic.sqrdmlah, q30,Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,4,_HS_,Bf((11,1),(20,2)))),
                            Instr_8_1(Mnemonic.sqrdmlah, q30,Vr(0,5,_HS_),Vr(5,5,_HS_),Vrx(16,5,_HS_,Bf((11,1),(21,1)))),
                            invalid),
                        Nyi("UDOT (by element)"),
                        Mask(22, 2, "  opcode=1111",
                            invalid,
                            Instr_8_1(Mnemonic.sqrdmlsh, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 4, _HS_, Bf((11, 1), (20, 2)))),
                            Instr_8_1(Mnemonic.sqrdmlsh, q30, Vr(0, 5, _HS_), Vr(5, 5, _HS_), Vrx(16, 5, _HS_, Bf((11, 1), (21, 1)))),
                            invalid)

                        ));

                var AdvancedSimd2RegMisc_FP16 = Mask(12, 5, "  Advanced SIMD two-register miscellaneous (FP16)",
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

                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=01100"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=01101"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=01110"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=01111"),

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11000"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11001"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11010"),
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11011"),

                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11100"),
                    Mask(29,1,23,1, "  opcode=11101 U:a",
                        Instr_8_2(Mnemonic.scvtf, q30, Vr(0,5,HHHH),Vr(5,5,HHHH)),
                        Instr_8_2(Mnemonic.frecpe, x("")),
                        Instr_8_2(Mnemonic.ucvtf, q30, Vr(0,5,HHHH),Vr(5,5,HHHH)),
                        Instr_8_2(Mnemonic.frsqrte, x("SCVTF (vector, integer)"))),
                    invalid,
                    Nyi("AdvancedSimd2RegMisc_FP16 opcode=11111")
                    );

                var AdvancedSimdPermute = Mask(12, 3, "  Advanced SIMD permute",
                    invalid,
                    Instr(Mnemonic.uzp1, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                    Instr(Mnemonic.trn1, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                    Instr(Mnemonic.zip1, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                    invalid,
                    Instr(Mnemonic.uzp2, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                    Instr(Mnemonic.trn2, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)),
                    Instr(Mnemonic.zip2, q30, Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD)));

                var AdvancedSimdScalarCopy = Mask(29, 1, "Advanced SIMD scalar copy",
                    Select((11, 4), IsZero,
                        dupScalarElement,
                        invalid),
                    invalid);

                var AdvancedSimdScalar3SameExtra = Mask(29, 1, "  Advanced SIMD scalar three same extra",
                    invalid,
                    Sparse(11, 4, "  U=1", invalid,
                        (0b0000, Instr_8_1(Mnemonic.sqrdmlah, x("(vector)"))),
                        (0b0001, Instr_8_1(Mnemonic.sqrdmlsh, x("(vector)")))));

                var AdvancedSimdScalarPairwise = Mask(12, 5, "  Advanced SIMD scalar pairwise",
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

                    Mask(29, 1, "  opcode=01100",
                        Mask(22, 2, "  U=0",
                            Instr_8_2(Mnemonic.fmaxnmp, x("(scalar) - Half - precision variant")),
                            invalid,
                            Instr_8_2(Mnemonic.fminnmp, x("(scalar) - Half - precision variant")),
                            invalid),
                        Mask(23, 1, "  U=1",
                            Instr(Mnemonic.fmaxnmp, x("(scalar) - Single-precision and double-precision variant")),
                            Instr(Mnemonic.fminnmp, x("(scalar) - Single-precision and double-precision variant")))),
                    Mask(29, 1, "  opcode=01101",
                        Mask(22, 2, "  U=0",
                            Instr_8_2(Mnemonic.faddp, x("(scalar) - Half - precision variant")),
                            invalid,
                            invalid,
                            invalid),
                        Mask(23, 1, "  U=1",
                            Instr(Mnemonic.faddp, x("(scalar) - Single-precision and double-precision variant")),
                            invalid)),
                    invalid,
                    Mask(29, 1, "  opcode=01111",
                        Mask(22, 2, "  U=0",
                            Instr_8_2(Mnemonic.fmaxp, x("(scalar) - Half - precision variant")),
                            invalid,
                            Instr_8_2(Mnemonic.fminp, x("(scalar) - Half - precision variant")),
                            invalid),
                        Mask(23, 1, "  U=1",
                            Instr(Mnemonic.fmaxp, x("(scalar) - Single-precision and double-precision variant")),
                            Instr(Mnemonic.fminp, x("(scalar) - Single-precision and double-precision variant")))),
                    // 10000
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
                    Mask(29, 1, "  opcode=11011",
                        Instr(Mnemonic.addp, q1, D_0, Vr(5,5,___D)),
                        invalid),

                    invalid,
                    invalid,
                    invalid,
                    invalid);

            var AdvancedSimd3Same_FP16 = Mask(Bf((29, 1), (23, 1), (11, 3)), "  Advanced SIMD three same (FP16)",
                Nyi("AdvancedSimd3Same_FP16 0:0:000"),
                Nyi("AdvancedSimd3Same_FP16 0:0:001"),
                Nyi("AdvancedSimd3Same_FP16 0:0:010"),
                Nyi("AdvancedSimd3Same_FP16 0:0:011"),
                Nyi("AdvancedSimd3Same_FP16 0:0:100"),
                invalid,
                Nyi("AdvancedSimd3Same_FP16 0:0:110"),
                Nyi("AdvancedSimd3Same_FP16 0:0:111"),

                Nyi("AdvancedSimd3Same_FP16 0:1:000"),
                Nyi("AdvancedSimd3Same_FP16 0:1:001"),
                Nyi("AdvancedSimd3Same_FP16 0:1:010"),
                invalid,
                invalid,
                invalid,
                Nyi("AdvancedSimd3Same_FP16 0:1:110"),
                Nyi("AdvancedSimd3Same_FP16 0:1:111"),

                Nyi("AdvancedSimd3Same_FP16 1:0:000"),
                invalid,
                Nyi("AdvancedSimd3Same_FP16 1:0:010"),
                Nyi("AdvancedSimd3Same_FP16 1:0:011"),
                Nyi("AdvancedSimd3Same_FP16 1:0:100"),
                Nyi("AdvancedSimd3Same_FP16 1:0:101"),
                Nyi("AdvancedSimd3Same_FP16 1:0:110"),
                Nyi("AdvancedSimd3Same_FP16 1:0:111"),

                Nyi("AdvancedSimd3Same_FP16 1:1:000"),
                invalid,
                Nyi("AdvancedSimd3Same_FP16 1:1:010"),
                invalid,
                Nyi("AdvancedSimd3Same_FP16 1:1:100"),
                Nyi("AdvancedSimd3Same_FP16 1:1:101"),
                Nyi("AdvancedSimd3Same_FP16 1:1:110"),
                invalid);

            var DataProcessingScalarFpAdvancedSimd_0_0 = 
                Mask(19, 4,        // op0=0000 op1=00 o
                    Mask(10, 2, "  op2=0000",
                        Mask(15, 1, "op3=xxxxxxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "  xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0000 op3=xxxxxxx01")),
                        Mask(15, 1, "  xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Mask(15, 1, "  xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0000 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0001",
                        Mask(15, 1, "  xxx?xxx00",
                            AdvancedSimdTableLookup,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxx1xxx00")),
                        Mask(15, 1, "  xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxx1xxx01")),
                        Mask(15, 1, "  xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxx1xxx10")),
                        Mask(15, 1, "  xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0010",
                        Mask(15, 1, "  xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "  xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0010 op3=xxx1xxx01")),
                        Mask(15, 1, "  xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0010 op3=xxx1xxx10")),
                        Mask(15, 1, "  xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0010 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0011",
                        Mask(15, 1, "  xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "  xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0011 op3=xxx1xxx01")),
                        Mask(15, 1, "  xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0011 op3=xxx1xxx10")),
                        Mask(15, 1, "  xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0011 op3=xxx1xxx11"))),

                    Mask(10, 2, "  op2=0100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0111"),
                        AdvancedSimd3Same),

                    Mask(10, 2, "  op2=1000",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1000 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1000 op3=xxxxxxx10")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1000 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1001",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1001 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1001 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1010",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1010 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1010 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1011",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1011 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1011 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, // op0=0000 op1=00 op2=1100 op3=??xxxxx10
                            AdvancedSimd2RegMisc,
                            invalid,    // op0=0000 op1=00 op2=1100 op3=01xxxxx10
                            invalid,    // op0=0000 op1=00 op2=1100 op3=10xxxxx10
                            invalid),   // op0=0000 op1=00 op2=1100 op3=11xxxxx10
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimd2RegMisc_FP16,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1111 op3=??xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1111 op3=??xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1111 op3=??xxxxx10")),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_0_1 =
                Mask(19, 4, "  op1=01",
                    Mask(10, 2,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0000 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0000 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0000 op3=xxxxxxx11")),
                    Mask(10, 2,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0001 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0001 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0001 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=0010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0010 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0010 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=0011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0011 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0011 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0011 op3=xxxxxxx11")),

                    Mask(10, 2, "  op2=0100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0101 op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0110"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0111"),
                        AdvancedSimd3Same),

                    Mask(10, 2, "  op2=1000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1000 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1000 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1000 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1001 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1001 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1001 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1010 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1010 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1011 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1011 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1011 op3=xxxxxxx11")),

                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1100"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1110"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1111 op3=xxxxxxx10"),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_0 = Mask(23, 2, // op0 = 0000
                DataProcessingScalarFpAdvancedSimd_0_0,
                DataProcessingScalarFpAdvancedSimd_0_1,
                Mask(10, 1, "  op1=10",
                    AdvancedSimdVector_x_indexedElement,
                    Select((19, 4), IsZero,
                        AdvancedSimdModifiedImm,
                        AdvancedSimdShiftByImm)),

                Mask(10, 1, "  op1=11",
                    AdvancedSimdVector_x_indexedElement,
                    invalid)
                    );


            var DataProcessingScalarFpAdvancedSimd_1 = Mask(23, 2, //op0 = 1 op1
                Mask(19, 4, // op0=1 op1=0b00 op2"),
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    FloatingPointDecoders,
                    FloatingPointDecoders,
                    FloatingPointDecoders,
                    FloatingPointDecoders,
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    ConversionBetweenFpAndFixedPoint,
                    FloatingPointDecoders,
                    FloatingPointDecoders,
                    FloatingPointDecoders,
                    FloatingPointDecoders),
                Nyi("DataProcessingScalarFpAdvancedSimd - op0=1 op1=0b01"),
                FloatingPointDataProcessing3src,
                FloatingPointDataProcessing3src);

            var DataProcessingScalarFpAdvancedSimd_2_0 = Mask(19, 4,    // op0=2 op1=00
                    Mask(10, 1, "  op2=0000",
                        Mask(15, 1, "  op3=xxxxxxxx0",
                            AdvancedSimdExtract,
                            AdvancedSimdCopy),
                        Nyi("  op3=xxxxxxxx0")),
                    Mask(10, 2, "  op2=0001",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "  op3=xxxxxxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0001 op3=xxx1xxx01")),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdExtract,
                            invalid),
                        Mask(15, 1, "  op3=xxxxxxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0001 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0010 op3=xxx1xxx00"),
                        Mask(15, 1, "  op3=xxxxxxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0010 op3=xxx1xxx01")),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdExtract,
                            invalid),
                        Mask(15, 1, "  op3=xxxxxxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0010 op3=xxx1xxx01"))),
                    Mask(10, 2, "  op2=0011",
                        Mask(15, 1, // op0=2 op1=00 op2=0100 op3=xxx?xxx00
                            AdvancedSimdExtract,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0011 op3=xxx1xxx00")),
                        Mask(15, 1, "  op3=xxxxxxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0010 op3=xxx1xxx01")),
                        Mask(15, 1, "  op3=xxxxxxx10",
                            AdvancedSimdExtract,
                            AdvancedSimdCopy),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            AdvancedSimd3SameExtra)),

                    Mask(10, 2,  // op0=2 op1=00 op2=0100 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        invalid,
                        AdvancedSimd3Same),

                    Mask(10, 2, "  op2=1000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1000 op3=??xxxxx00"),
                        Mask(14, 2, "  op3=xxx??xx01",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1000 op3=??xxxxx10"),
                        Mask(14, 2, "  op3=xxx??xx11",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=1001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1001 op3=??xxxxx00"),
                        Mask(14, 2, "  op3=xxx??xx01",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1001 op3=??xxxxx10"),
                        Mask(14, 2, "  op3=xxx??xx11",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=1010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1001 op3=??xxxxx00"),
                        Mask(14, 2, "  op3=xxx??xx01",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1001 op3=??xxxxx10"),
                        Mask(14, 2, "  op3=xxx??xx11",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=1011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1011 op3=??xxxxx00"),
                        Mask(14, 2, "  op3=xxx??xx01",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1011 op3=??xxxxx10"),
                        Mask(14, 2, "  op3=xxx??xx11",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),

                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        invalid,
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimd2RegMisc_FP16,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1111 op3=01xxxxx10"),
                            invalid,
                            invalid),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_2_1 =
                Mask(19, 4, "  op1=01",
                    Mask(10, 1, "  op2=0000",
                        Mask(15, 1, "  op3=xxxxxxxxx0",
                            AdvancedSimdExtract,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0000 op3=xxxx1xxxx0")),
                        Mask(15, 1, "  op3=xxx?xxxx1",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=0001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0001 op3=xxxxxxx00"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            invalid,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0001 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=0010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0010 op3=xxxxxxx01"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0010 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=0011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0011 op3=xxxxxxx00"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            invalid,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0011 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=0100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=0101 op3=xxxxxxxxx1"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        invalid,
                        AdvancedSimd3Same),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1000"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1001"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1010"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1011"),
                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1100"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                         AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01 op2=1111"),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_2 = Mask(23, 2, "  op0=2",
                DataProcessingScalarFpAdvancedSimd_2_0,
                DataProcessingScalarFpAdvancedSimd_2_1,

                Mask(10, 1, "  op1=10",
                    AdvancedSimdVector_x_indexedElement,
                    Select((19, 4), IsZero, "  op3=xxxxxxxx1",
                        AdvancedSimdModifiedImm,
                        AdvancedSimdShiftByImm)),
                Mask(10, 1, "  op1=11",
                    AdvancedSimdVector_x_indexedElement,
                    invalid));

                var DataProcessingScalarFpAdvancedSimd_3 = Mask(23, 2, "  op0=3",
                        Mask(19, 4, "  op1=00",
                            ConversionBetweenFpAndFixedPoint,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0011"),

                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=0111"),

                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1011"),

                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=00 op2=1101"),
                            FloatingPointDecoders,
                            FloatingPointDecoders),

                        Mask(19, 4, "  op1=01",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0101"),
                            FloatingPointDecoders,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=3 op1=01 op2=1111")),
                        FloatingPointDataProcessing3src,
                        FloatingPointDataProcessing3src);

                var DataProcessingScalarFpAdvancedSimd_4_0 = Mask(19, 4, "  op1=00",
                    Mask(10, 2,  // op0=4 op1=00 op2=0000 op3=xxxxxxx??
                        Mask(15, 1, // op0=4 op1=0b00 op2=0b0000 op3=xxx?xxx01"
                            AdvancedSimdTableLookup,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxx1xxx00")),
                        Mask(15, 1, // op0=4 op1=0b00 op2=0b0000 op3=xxx?xxx01"
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxx1xxx01")),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Mask(15, 1, // op0=4 op1=0b00 op2=0b0000 op3=xxx?xxx11"
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0001",
                        Mask(15, 1, "op3=xxxxxxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "op3=xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001 op3=xxx1xxx01")),
                        Mask(15, 1, "op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001 op3=xxxxxxx10")),
                        Mask(15, 1, "op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001 op3=xxxxxxx11"))),
                    Mask(10, 2, "  op2=0010",
                        Mask(15, 1, "  op3=xxxxxxx00",
                            AdvancedSimdTableLookup,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0010 op3=xxx1xxx00")),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0010 op3=xxxxxxx01")),
                        Mask(15, 1, "op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001 op3=xxxxxxx10")),
                        Mask(15, 1, "op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0010 op3=xxxxxxx11"))),
                    Mask(10, 2, "  op2=0011",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0011 op3=xxx1xxx00")),
                        Mask(15, 1, "  op3=xxxxxxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0011 op3=xxxxxxx01")),
                        Mask(15, 1, "op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001 op3=xxxxxxx10")),
                        Mask(15, 1, "  op3=xxxxxxx11",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0011 op3=xxx1xxx11"))),
                    Mask(10, 2, "  op2=0100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2,  // op0=4 op1=0b00 op2=0b0100 op3=??xxxxx10
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2,  // op0=4 op1=0b00 op2=0b0101 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, // op0=4 op1=0b00 op2=0b0101 op3=??xxxxx10
                            CryptographicAES,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, // op0=4 op1=00 op2=0110
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op0=4 op1=00 op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0111 op3=xxxxxxx10"),
                        AdvancedSimd3Same),

                    Mask(10, 2, "  op2=1000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1000 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1000 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1000 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1001 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1001 op3=xxxxxxx01"),
                        Mask(15, 1,
                            AdvancedSimdPermute,
                            invalid),
                        Mask(15, 1,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1001 op3=xxx00xx11"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1001 op3=xxx11xx11"))),
                    Mask(10, 2, "  op2=1010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1010 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1010 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=0b1011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011 op3=xxxxxxx01"),
                        Mask(15, 2, "   op3=xx??xxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011 op3=xx01xxx10"),
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011 op3=xx11xxx10")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=0b1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "op3=...10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0b1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1101 op3=00xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1101 op3=01xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1101 op3=10xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1101 op3=11xxxxx10")),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0b1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0b1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=10",
                            AdvancedSimd2RegMisc_FP16,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1111 op3=01xxxxx10"),
                            Mask(17, 2, "  op3=10xxxx10",
                                AdvancedSimd2RegMisc,
                                invalid,
                                invalid,
                                invalid),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1111 op3=11xxxx10")),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_4 = Mask(23, 2, "  op0 = 4",
                DataProcessingScalarFpAdvancedSimd_4_0,
                Mask(19, 4, "  op0=4 op1=0b01 op2",
                    Mask(10, 2, "  op2=0000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0000 op3=xxxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0000 op3=xxxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0000 op3=xxxxxxxx11")),
                    Mask(10, 2, "  op2=0001",
                        Mask(15, 2, "  op3=xx??xxx00",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxx0xxxx00"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxx0xxxx01"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxx0xxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxx1xxxx11")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxxx10",
                            AdvancedSimdPermute,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxx1xxxx10")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001 op3=xxxxxxxx11")),
                    Mask(10, 2, "  op2=0010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0010 op3=xxxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0010 op3=xxxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0010 op3=xxxxxxxx11")),
                    Mask(10, 2, "  op2=0011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0011 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0011 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0011 op3=xxxxxxx11")),
                    Mask(10, 2,   // op0=4 op1=0b01 op2=0b0100 op3
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, // op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx10
                            AdvancedSimd2RegMisc,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=01xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=10xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=11xxxxx10")),
                        AdvancedSimd3Same),
                    Mask(10, 2, // op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2,   // op0=4 op1=0b01 op2=0b0110 op3
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, // op0=4 op1=0b01 op2=0b0110 op3=xxxxxxx10
                            AdvancedSimdAcrossLanes,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=01xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=10xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=11xxxxx10")),
                        AdvancedSimd3Same),
                    Mask(10, 2,      // op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1000 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1000 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1000 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1001 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1001 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1001 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1010 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1010 op3=xxxxxxx11")),
                    Mask(10, 2, "  op2=1011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1011 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1011 op3=xxxxxxx01"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            AdvancedSimdPermute,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1011 op3=xxxxxxx11")),
                    Mask(10, 2,      // op0=4 op1=0b01 op2=0b1100 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2,
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",     // op0=4 op1=0b01 op2=0b1101 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1101 op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1111"),
                        AdvancedSimd3Same)),
                Select((19, 4), IsZero, "  op1=10",
                    Mask(10, 1, // op0=4 op1=0b10 op2=0000 op3=xxxxxxxx?
                        AdvancedSimdVector_x_indexedElement,
                        AdvancedSimdModifiedImm),
                    Mask(10, 1, // op0=4 op1=0b10 op2=0000 op3=xxxxxxxx?
                        AdvancedSimdVector_x_indexedElement,
                        AdvancedSimdShiftByImm)),
                Mask(10, 1, "  op1=11",
                    AdvancedSimdVector_x_indexedElement,
                    invalid));

                var DataProcessingScalarFpAdvancedSimd_5 = Mask(23, 2, "  op0=5", // op0=5 op1
                        Sparse(19, 4, "  op1=0b00",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=???"),
                            (0b0000, Mask(10, 2,     // op0=5 op1=0b00 op2=0000")),
                                Mask(15, 1,             // op0=5 op1=0b00 op2=0000 op3=xxxxxxx00"),
                                    Cryptographic3regSHA,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxx1xxx00")),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx10"),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    AdvancedSimdScalar3SameExtra))),
                            (0b0001, Mask(10, 2,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx00"),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx10"),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0001 op3=xxx1xxx11")))),
                            (0b0010, Mask(10, 2,     // op0=5 op1=0b00 op2=0100 op3
                                Mask(15, 1,             // op0=5 op1=0b00 op2=0010 op3=xxx?xxx00"),
                                    Cryptographic3regSHA,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxx1xxx00")),
                                Mask(15, 1, 
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxxxxxx10"),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxx1xxx11")))),
                            (0b0011, Mask(10, 2, "  op2=0011",
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0011 op3=xxxxxxx00"),
                                Mask(15, 1, "  op3=xxxxxxx01",
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0011 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0011 op3=xxxxxxx10"),
                                Mask(15, 1,
                                    AdvancedSimdScalarCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0011 op3=xxx1xxx11")))),

                            (0b0100, Mask(10, 2,     // op0=5 op1=0b00 op2=0100 op3
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=xxxxxxx01"),
                                Mask(17, 2,          // op0=5 op1=0b00 op2=0100 op3=??xxxxxx10"),
                                    AdvancedSIMDscalar2RegMisc,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=01xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=10xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=11xxxxxx10")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0100 op3=xxxxxxx11"))),
                            (0b0101, Mask(10, 2, "  op2=5",
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=xxxxxxx01"),
                                Mask(17, 2,          // op0=5 op1=0b00 op2=0101 op3=??xxxxxx10"),
                                    Cryptographic2regSHA,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=01xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=10xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=11xxxxx10")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=xxxxxxx11"))),
                            (0b1100, Mask(10, 2, "  op2=C",
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=xxxxxxx01"),
                                Mask(16, 2, "  op3=xxxxxxx10",
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=00xxxxx10"),
                                    AdvancedSIMDscalar2RegMisc,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=10xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=11xxxxx10")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=1100 op3=xxxxxxx11")))
                            ),
                        Sparse(19, 4, "  op1=01",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b01 op2=????"),
                            (0b1110, Mask(10, 2, "  op2=1110",
                                AdvancedSimdScalar3Different,
                                AdvancedSimdScalar3Same,
                                Mask(17, 2, "  op3=xxxxxxx10",
                                    AdvancedSimdScalarPairwise,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b01 op2=1110 op3=01xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b01 op2=1110 op3=10xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b01 op2=1110 op3=11xxxxxx10")),
                                AdvancedSimdScalar3Same))
                                ),
                        Mask(10, 1, "  op0=5 op1=0b10 ", // op0=5 op1=10 op2
                            AdvancedSimdScalar_x_IdxElem,
                            AdvancedSimdScalarShiftByImmediate),
                        Mask(10, 1, "  op0=5 op1=0b10 ", // op0=5 op1=10 op2
                            AdvancedSimdScalar_x_IdxElem,
                            invalid));

            var DataProcessingScalarFpAdvancedSimd_6_0 = Mask(19, 4, "  op1=00",     // op0=6 op1=00 op2
                    Mask(10, 2,             // op0=6 op1=00 op2=0000 op3=xxxxxxx??
                        Mask(15, 1, "  op3=xxxxxxx00",      // op0=6 op1=00 op2=0000 op3=xxx?xxx00
                            AdvancedSimdExtract,
                            invalid),
                        Mask(15, 1,     // op0=6 op1=00 op2=0000 op3=xxx?xxx01
                            AdvancedSimdCopy,   // op0=6 op1=00 op2=0000 op3=xxx0xxx01
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxx1xxx01")),
                        Mask(15, 1,     // op0=6 op1=00 op2=0000 op3=xxx?xxx10
                            AdvancedSimdExtract,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxx1xxx10")),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op=0001",
                        Mask(15, 1,     // op0=6 op1=00 op2=0001 op3=xxx?xxx01
                            AdvancedSimdExtract,
                            invalid),
                        Mask(15, 1,     // op0=6 op1=00 op2=0001 op3=xxx?xxx01
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001 op3=xxx1xxx01")),
                        Mask(15, 1, "  op3=xxx?xxx10",
                            AdvancedSimdExtract,
                            AdvancedSimdCopy),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op=0010",
                        Mask(15, 1,     // op0=6 op1=00 op2=0010 op3=xxx?xxx00
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1,
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxx1xxx01")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=0011",
                        Mask(15, 1, "  op3=xxx?xxx00",
                            AdvancedSimdTableLookup,
                            invalid),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            AdvancedSimdCopy,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0011 op3=xxx1xxx01")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0011 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            AdvancedSimdCopy,
                            AdvancedSimd3SameExtra)),

                    Mask(10, 2, "  op2=0100",     // op0=6 op1=00 op2=0100 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2,  // op0=6 op1=00 op2=0100 op3=??xxxxxx10
                            AdvancedSimd2RegMisc,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=01xxxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=10xxxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=11xxxxxx10")),
                        AdvancedSimd3Same),
                    Mask(10, 2,      // op0=6 op1=00 op2=0101 op3=xxxxxxx??
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0101 op3=000000010"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0111  op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxxxxxx00"),
                        Mask(14, 2, "   op3=xxx??xx11",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxx00xx01"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxx01xx01"),
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxxxxxx10"),
                        Mask(14, 2, "   op3=xxxxxxx11",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxx00xx11"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000 op3=xxx01xx11"),
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 1, "  op2=1001",
                        Mask(15, 1, "  op3=xxxxxxxx0",
                            AdvancedSimdExtract,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1001 op3=xxx1xxxx0")),
                        Mask(14, 2, "  op3=xxx??xxx1",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1001 op3=xxx00xxx1"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1001 op3=xxx01xxx1"),
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 1, "  op2=1010",
                        Mask(15, 1, "  op3=xxxxxxxx0",
                            AdvancedSimdExtract,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1010 op3=xxx1xxxx0")),
                        Mask(14, 2, "  op3=xxx??xxx1",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=1011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1011 op3=xxxxxxx00"),
                        Mask(14, 2, "  op3=xxxxxxx01",
                            AdvancedSimd3Same_FP16,
                            invalid,
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1011 op3=xxxxxxx10"),
                        Mask(14, 2, "  op3=xxx??xx11",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1011 op3=xxx00xx11"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1011 op3=xxx01xx11"),
                            AdvancedSimd3SameExtra,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        invalid,
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimd2RegMisc_FP16,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_6_1 =
                Mask(19, 4, "  op1=01",
                    Mask(10, 2, " op2=0000",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0000 op3=xxxxxxx00"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            invalid,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0000 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, " op2=0001",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0001 op3=xxxxxxx00"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            invalid,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0001 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, " op2=0010",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0010 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0010 op3=xxxxxxx01"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0010 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),
                    Mask(10, 2, " op2=0011",
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0011 op3=xxxxxxx00"),
                        Mask(15, 1, "  op3=xxx?xxx01",
                            invalid,
                            AdvancedSimd3SameExtra),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0011 op3=xxxxxxx10"),
                        Mask(15, 1, "  op3=xxx?xxx11",
                            invalid,
                            AdvancedSimd3SameExtra)),

                    Mask(10, 2, " op2=0100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxxx10",
                            AdvancedSimd2RegMisc,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0100 op3=01xxxxxx10"),
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, " op2=0101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0101"),
                        AdvancedSimd3Same),
                    Mask(10, 2, " op2=0110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, " op2=0111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=0111"),
                        AdvancedSimd3Same),

                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1000"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1001"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1010"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1011"),

                    Mask(10, 2, "  op2=1100",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=xxxxxxx10",
                            AdvancedSimd2RegMisc,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1101",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1101 op3=xxxxxxx10"),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1110",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Mask(17, 2, "  op3=??xxxxx10",
                            AdvancedSimdAcrossLanes,
                            invalid,
                            invalid,
                            invalid),
                        AdvancedSimd3Same),
                    Mask(10, 2, "  op2=1111",
                        AdvancedSimd3Different,
                        AdvancedSimd3Same,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01 op2=1111 op3=xxxxxxx10"),
                        AdvancedSimd3Same));

            var DataProcessingScalarFpAdvancedSimd_6 = Mask(23, 2, "  op0=6",          // DataProcessingScalarFpAdvancedSimd - op0=6
                DataProcessingScalarFpAdvancedSimd_6_0,
                DataProcessingScalarFpAdvancedSimd_6_1,
                Mask(10, 1, "  op1=10",
                    AdvancedSimdVector_x_indexedElement,
                    Select((19, 4), IsZero,
                        AdvancedSimdModifiedImm,
                        AdvancedSimdShiftByImm)),
                Mask(10, 1, "  op1=11",
                    AdvancedSimdVector_x_indexedElement,
                    invalid));

            var DataProcessingScalarFpAdvancedSimd_7 = Mask(23, 2, "  op0=7", // op0=7 op1
                Mask(19, 4,    // op0=7 op1=00
                    Mask(10, 1,
                        Mask(15, 1,
                            invalid,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0000 op3=xxx1xxxx0")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0000 op3=xxxxxxxx1")),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0001"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0010"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0011"),
                    Mask(10, 2,  // op0=7 op1=00 op2=0100 op3=xxxxxxx??
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=xxxxxxx01"),
                        Mask(17, 2,  // op0=7 op1=00 op2=0100 op3=??xxxxx10
                            AdvancedSIMDscalar2RegMisc,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=01xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=10xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=11xxxxx10")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0100 op3=xxxxxxx11")),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0101"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0110"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0111"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1000"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1001"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1010"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1111"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1100"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1101"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1110"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=1111")),
                Sparse(19, 4, "  op1=01",
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01"),
                    (0b0100, Mask(10, 2,
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01 op2=0100 op3=xxxxxxx00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01 op2=0100 op3=xxxxxxx01"),
                        Mask(17, 2,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01 op2=0100 op3=00xxxxx10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01 op2=0100 op3=01xxxxx10"),
                            invalid,
                            invalid),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=01 op2=0100 op3=xxxxxxx11")))),
                Mask(10, 1, "  op1=10",
                    AdvancedSimdVector_x_indexedElement,
                    AdvancedSimdScalarShiftByImmediate),
                Mask(10, 1, "  op1=11",
                    AdvancedSimdScalar_x_IdxElem,
                    invalid));

            var DataProcessingScalarFpAdvancedSimd_8 = invalid;

            var DataProcessingScalarFpAdvancedSimd_9 = Mask(23, 2, "  op0=9",
                        Mask(19, 4, "op1=00",
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1011"),
                            Mask(10, 2, // op0=9 op1=00 op2=1100 op3
                                FloatingPointDecoders,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1100 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1100 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1100 op3=xxxxxxx11")),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1110"),
                            Mask(10, 2, "  op2=1111",
                                FloatingPointDecoders,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1111 01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1111 10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1111 11"))),
                        Mask(19, 4,        // op0=9 op1=01 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0100"),
                            FloatingPointDecoders,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=01 op2=1111")),
                        FloatingPointDataProcessing3src,    // op0=9 op1=10
                        FloatingPointDataProcessing3src);   // op0=9 op1=11

                var DataProcessingScalarFpAdvancedSimd_A = invalid;
                var DataProcessingScalarFpAdvancedSimd_B = invalid;

                DataProcessingScalarFpAdvancedSimd = Mask(28, 4, "  DataProcessingScalarFpAdvancedSimd",
                    DataProcessingScalarFpAdvancedSimd_0,
                    DataProcessingScalarFpAdvancedSimd_1,
                    DataProcessingScalarFpAdvancedSimd_2,
                    DataProcessingScalarFpAdvancedSimd_3,

                    DataProcessingScalarFpAdvancedSimd_4,
                    DataProcessingScalarFpAdvancedSimd_5,
                    DataProcessingScalarFpAdvancedSimd_6,
                    DataProcessingScalarFpAdvancedSimd_7,

                    DataProcessingScalarFpAdvancedSimd_8,
                    DataProcessingScalarFpAdvancedSimd_9,
                    DataProcessingScalarFpAdvancedSimd_A,
                    DataProcessingScalarFpAdvancedSimd_B,

                    Mask(23, 2,              // op0=C op1
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=C op1=00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=C op1=01"),
                        invalid,
                        invalid),
                    invalid,
                    invalid,
                    invalid);
            }

            rootDecoder = Mask(25, 4, "AArch64",
                invalid,
                invalid,
                invalid,
                invalid,

                LoadsAndStores,
                DataProcessingReg,
                LoadsAndStores,
                DataProcessingScalarFpAdvancedSimd,
                
                DataProcessingImm,
                DataProcessingImm,
                BranchesExceptionsSystem,
                BranchesExceptionsSystem,
                
                LoadsAndStores,
                DataProcessingReg,
                LoadsAndStores,
                DataProcessingScalarFpAdvancedSimd);
        }
    }
}
