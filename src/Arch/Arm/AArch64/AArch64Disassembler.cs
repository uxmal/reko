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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
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
        private const uint RegisterMask = 0b11111;

        private static readonly Decoder rootDecoder;
        private static readonly Decoder invalid;

        private Arm64Architecture arch;
        private EndianImageReader rdr;
        private Address addr;
        private DasmState state;

        public AArch64Disassembler(Arm64Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override AArch64Instruction DisassembleInstruction()
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
            public MachineOperand shiftAmount = null;
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
                    shiftCode = shiftCode,
                    shiftAmount = shiftAmount,
                    vectorData = vectorData,
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
            case 4:
                return Replicate(w8, 16, 4);
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





        // 32-bit register.
        private static Mutator<AArch64Disassembler> W(int pos, int size) {
            var fields = new[]
            {
                new Bitfield(pos, size)
            };
            return (u, d) =>
            {
                uint iReg = Bitfield.ReadFields(fields, u);
                d.state.ops.Add(new RegisterOperand(Registers.GpRegs32[iReg]));
                return true;
            };
        }
        private static Mutator<AArch64Disassembler> W_0 = W(0, 5);
        private static Mutator<AArch64Disassembler> W_5 = W(5, 5);
        private static Mutator<AArch64Disassembler> W_10 = W(10, 5);
        private static Mutator<AArch64Disassembler> W_16 = W(16, 5);

        // 32-bit register - but use stack register instead of w31

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
        private static Mutator<AArch64Disassembler> Ws_0 = Ws(0, 5);
        private static Mutator<AArch64Disassembler> Ws_5 = Ws(5, 5);
        private static Mutator<AArch64Disassembler> Ws_16 = Ws(16, 5);


        // 64-bit register.
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

        // 64-bit register - but use stack register instead of x31

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
        private static Mutator<AArch64Disassembler> Xs_0 = Xs(0, 5);
        private static Mutator<AArch64Disassembler> Xs_5 = Xs(5, 5);
        private static Mutator<AArch64Disassembler> Xs_16 = Xs(16, 5);

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
        private static Mutator<AArch64Disassembler> S_0 = S(0, 5);
        private static Mutator<AArch64Disassembler> S_5 = S(5, 5);
        private static Mutator<AArch64Disassembler> S_16 = S(16, 5);

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



        // 64-bit SIMD register.
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

        // 64-bit SIMD/FPU register or zero if field = 0b00000
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


        // 128-bit SIMD register.
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

        // Picks either a Dx or a Qx SIMD register depending on whether the
        // 'Q' bit is set. The q() mutator must be called first for this to 
        // work correctly.
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

        // Picks a V register and an element arrangement
        private static Mutator<AArch64Disassembler> Vr(int pos, int size, VectorData[] elementArrangement, int sizePos = 22)
        {
            var bitfield = new Bitfield(pos, size);
            return (u, d) =>
            {
                uint iReg = bitfield.Read(u);
                var iArr = (u >> sizePos) & 3;
                var dt = d.state.useQ ? PrimitiveType.Word128 : PrimitiveType.Word64;
                var et= elementArrangement[iArr];
                if (et == VectorData.Invalid)
                    return false;
                var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg]);
                vr.ElementType = et;
                d.state.ops.Add(vr);
                return true;
            };
        }

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

        // Picks a V register and an element arrangement from packed field
        // (used in `dup` and `mov` for vectors)
        private static Mutator<AArch64Disassembler> Vrs(int pos, int size, int sizePos, int sizeLength, bool useIndex, bool usePrevSize)
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
                var vr = new VectorRegisterOperand(dt, Registers.SimdVectorReg128[iReg]);
                vr.ElementType = BHSD[elemSize];
                Debug.Assert(vr.ElementType != VectorData.Invalid);
                vr.Index = index;
                d.state.ops.Add(vr);
                return true;
            };
        }

        private static Mutator<AArch64Disassembler> Vri(int pos, int len, PrimitiveType dt, VectorData et, int idx)
        {
            var field = new Bitfield(pos, len);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var vr = new VectorRegisterOperand(dt, Registers.SimdRegs128[iReg]);
                vr.Index = idx;
                vr.ElementType = et;
                Debug.Assert(vr.ElementType != VectorData.Invalid);
                d.state.ops.Add(vr);
                return true;
            };
        }

        // Extended register, depending on the option field.
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
                var dt = Bits.IsBitSet(u, 30) ? PrimitiveType.Word128 : PrimitiveType.Word64;
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

        // Single base register access.
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


        // bit which determines whether or not to use Qx or Dx registers in SIMD
        private static Mutator<AArch64Disassembler> q(int offset)
        {
            return (u, d) => { d.state.useQ = Bits.IsBitSet(u, offset); return true; };
        }

        // Force Q bit to true
        private static bool q1(uint u, AArch64Disassembler d)
        {
            d.state.useQ = true; return true;
        }

        // Arrangement specifier tells us how words are packed
        private static Mutator<AArch64Disassembler> As(int pos, int length)
        {
            var bitfield = new Bitfield(pos, length);
            return (u, d) =>
            {
                var arrangement = bitfield.Read(u);
                switch (arrangement)
                {
                case 1:
                    d.state.vectorData = VectorData.I8; break;
                case 2:
                case 3:
                    d.state.vectorData = VectorData.I16; break;
                case 4:
                case 5:
                case 6:
                case 7:
                    d.state.vectorData = VectorData.I32; break;
                }
                return true;
            };
        }

        private static Bitfield barrierField = new Bitfield(8, 4);
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
                d.NotYetImplemented(m, u);
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
        private static VectorData[] BHS_ = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.Invalid
        };
        // Packing arragement in SIMD vector register
        private static VectorData[] BHSD = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.I64
        };
        private static VectorData[] BHS_128 = new[]
        {
            VectorData.I8, VectorData.I16, VectorData.I32, VectorData.Invalid
        };
        private static VectorData[] HSD_ = new[]
        {
             VectorData.I16, VectorData.I32, VectorData.I64, VectorData.Invalid
        };
        private static VectorData[]BBB_ = new[]
        {
             VectorData.I8, VectorData.I8, VectorData.I8, VectorData.Invalid
        };
        private static VectorData[] BBBB = new[]
        {
             VectorData.I8, VectorData.I8, VectorData.I8, VectorData.I8
        };
        private static VectorData[] HHHH = new[]
        {
             VectorData.I16, VectorData.I16, VectorData.I16, VectorData.I16
        };
        private static VectorData[] SSSS = new[]
        {
             VectorData.I32, VectorData.I32, VectorData.I32, VectorData.I32
        };
        private static VectorData[] DDDD = new[]
        {
             VectorData.I64, VectorData.I64, VectorData.I64, VectorData.I64
        };
        private static VectorData[] SSDD = new[]
        {
             VectorData.I32, VectorData.I32, VectorData.I64, VectorData.I64
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

        private static Decoder Sparse(int pos, uint mask, Decoder @default, params (uint, Decoder)[] decoders)
        {
            return new SparseMaskDecoder("", pos, mask, decoders.ToDictionary(k => k.Item1, v => v.Item2), @default);
        }


        private static Decoder Sparse(string tag, int pos, uint mask, Decoder @default, params (uint, Decoder)[] decoders)
        {
            return new SparseMaskDecoder(tag, pos, mask, decoders.ToDictionary(k => k.Item1, v => v.Item2), @default);
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
            return new NyiDecoder<AArch64Disassembler, Mnemonic, AArch64Instruction>(str);
        }

        private AArch64Instruction NotYetImplemented(string message, uint wInstr)
        {
            var instrHex = $"{wInstr:X8}";
            base.EmitUnitTest("AArch64", instrHex, message, "AArch64Dis", this.addr, Console =>
            {
                Console.WriteLine($"    Given_Instruction(0x{wInstr:X8});");
                Console.WriteLine($"    Expect_Code(\"@@@\");");
                Console.WriteLine();
            });
            return CreateInvalidInstruction();
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
                    Instr(Mnemonic.str, Q(0,5), Mo(w128, 5, 10, 12)),
                    Instr(Mnemonic.ldr, Q(0,5), Mo(w128, 5, 10, 12)),
                    // 01 0 00
                    Instr(Mnemonic.strh, W_0, Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldrh, W_0, Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldrsh, X_0, Mo(i16, 5, 10, 12)),
                    Instr(Mnemonic.ldrsh, W_0, Mo(i16, 5, 10, 12)),
                    // 01 1 00
                    Instr(Mnemonic.str, H(0,5), Mo(w16, 5, 10, 12)),
                    Instr(Mnemonic.ldr, H(0,5), Mo(w16, 5, 10, 12)),
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
                    Instr(Mnemonic.str, D(0,5), Mo(w64, 5, 10, 12)),
                    Instr(Mnemonic.ldr, D(0,5), Mo(w64, 5, 10, 12)),
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
                        Instr(Mnemonic.str, Q(0,5),Mr(w128)),
                        Instr(Mnemonic.ldr, Q(0,5),Mr(w128)),

                        // LoadStoreRegisterRegOff sz:V:opc=01 0 00
                        Instr(Mnemonic.strh, W_0,Mr(w16)),
                        Instr(Mnemonic.ldrh, W_0,Mr(w16)),
                        Instr(Mnemonic.ldrsh, X_0,Mr(i16)),
                        Instr(Mnemonic.ldrsh, W_0,Mr(i16)),

                        // LoadStoreRegisterRegOff sz:V:opc=01 1 00
                        Instr(Mnemonic.str, H(0,5),Mr(w16)),
                        Instr(Mnemonic.ldr, H(0,5),Mr(w16)),
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
                        Instr(Mnemonic.str, D(0,5),Mr(w64)),
                        Instr(Mnemonic.ldr, D(0,5),Mr(w64)),
                        invalid,
                        invalid));

            }

            Decoder LdStRegPairOffset;
            {
                LdStRegPairOffset = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0,W(10,5), Mo(w32,5,15,7)),
                    Instr(Mnemonic.ldp, W_0,W(10,5), Mo(w32,5,15,7)),
                    Instr(Mnemonic.stp, S_0,S(10,5), Mo(w32,5,15,7)),
                    Instr(Mnemonic.ldp, S_0,S(10,5), Mo(w32,5,15,7)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, Mo(w32,5,15,7)),
                    Instr(Mnemonic.stp, D(0,5),D(10,5), Mo(w64,5,15,7)),
                    Instr(Mnemonic.ldp, D(0,5),D(10,5), Mo(w64,5,15,7)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, Mo(w64,5,15,7)),
                    Instr(Mnemonic.ldp, X_0,X_10, Mo(w64,5,15,7)),
                    Instr(Mnemonic.stp, Q(0,5),Q(10,5), Mo(w128,5,15,7)),
                    Instr(Mnemonic.ldp, Q(0,5),Q(10,5), Mo(w128,5,15,7)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStRegPairPre;
            {
                LdStRegPairPre = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0,W(10,5), MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, W_0,W(10,5), MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, S_0,S(10,5), MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, S_0,S(10,5), MprePair(PrimitiveType.Word32)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, MprePair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, D(0,5),D(10,5), MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, D(0,5),D(10,5), MprePair(PrimitiveType.Word64)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, X_0,X_10, MprePair(PrimitiveType.Word64)),
                    Instr(Mnemonic.stp, Q(0,5),Q(10,5), MprePair(PrimitiveType.Word128)),
                    Instr(Mnemonic.ldp, Q(0,5),Q(10,5), MprePair(PrimitiveType.Word128)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStRegPairPost;
            {
                LdStRegPairPost = Mask(Bf((30,2), (26,1), (22,1)), // opc:V:L
                    Instr(Mnemonic.stp, W_0,W(10,5), MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, W_0,W(10,5), MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, S_0,S(10,5), MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.ldp, S_0,S(10,5), MpostPair(PrimitiveType.Word32)),

                    invalid,
                    Instr(Mnemonic.ldpsw, X_0,X_10, MpostPair(PrimitiveType.Word32)),
                    Instr(Mnemonic.stp, D(0,5),D(10,5), MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, D(0,5),D(10,5), MpostPair(PrimitiveType.Word64)),
                    
                    Instr(Mnemonic.stp, X_0,X_10, MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.ldp, X_0,X_10, MpostPair(PrimitiveType.Word64)),
                    Instr(Mnemonic.stp, Q(0,5),Q(10,5), MpostPair(PrimitiveType.Word128)),
                    Instr(Mnemonic.ldp, Q(0,5),Q(10,5), MpostPair(PrimitiveType.Word128)),

                    invalid,
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder LdStNoallocatePair = Mask(Bf((30, 2), (26, 1), (22, 1)),
                Instr(Mnemonic.stnp, W_0, W_10,Mo(w32,5,15,7)),
                Instr(Mnemonic.ldnp, W_0, W_10,Mo(w32,5,15,7)),
                Instr(Mnemonic.stnp, x("SIMD&FP 32-bit")),
                Instr(Mnemonic.ldnp, x("SIMD&FP 32-bit")),

                invalid,
                invalid,
                Instr(Mnemonic.stnp, x("SIMD&FP 64-bit")),
                Instr(Mnemonic.ldnp, x("SIMD&FP 64-bit")),

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
                    Instr(Mnemonic.stur, Q(0,5), Mu(w128,5,12,9)),
                    Instr(Mnemonic.ldur, Q(0,5), Mu(w128,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=0 opc=00
                    Instr(Mnemonic.sturh, W_0, Mo(w16, 5, 12, 9)),
                    Instr(Mnemonic.ldurh, W_0, Mo(w16, 5, 12, 9)),
                    Instr(Mnemonic.ldursh, X_0, Mu(i16,5,12,9)),
                    Instr(Mnemonic.ldursh, W_0, Mu(i16,5,12,9)),

                    // LdStRegUnscaledImm size=01 V=1 opc=00
                    Instr(Mnemonic.stur, H(0,5), Mu(w16,5,12,9)),
                    Instr(Mnemonic.ldur, H(0,5), Mu(w16,5,12,9)),
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
                    Instr(Mnemonic.stur, D(0,5), Mu(w64,5,12,9)),
                    Instr(Mnemonic.ldur, D(0,5), Mu(w64,5,12,9)),
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
                        Instr(Mnemonic.str, Q(0, 5), Mpost(w128)),
                        Instr(Mnemonic.ldr, Q(0, 5), Mpost(w128)),

                        Instr(Mnemonic.strh, W_0, Mpost(w16)),
                        Instr(Mnemonic.ldrh, W_0, Mpost(w16)),
                        Instr(Mnemonic.ldrsh, X_0, Mpost(i16)),
                        Instr(Mnemonic.ldrsh, W_0, Mpost(i16)),

                        Instr(Mnemonic.str, H(0, 5), Mpost(w16)),
                        Instr(Mnemonic.ldr, H(0, 5), Mpost(w16)),
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

            var LdStRegUnprivileged = Nyi("LdStRegUnprivileged");

                Decoder LdStRegImmPreIdx;
                {
                    LdStRegImmPreIdx = Mask(Bf((30, 2), (26, 1), (22, 2)),
                        Instr(Mnemonic.strb, W_0, Mpre(w8)),
                        Instr(Mnemonic.ldrb, W_0, Mpre(w8)),
                        Instr(Mnemonic.ldrsb, X_0, Mpre(i8)),
                        Instr(Mnemonic.ldrsb, W_0, Mpre(i8)),

                        Instr(Mnemonic.str, B(0, 5), Mpre(w8)),
                        Instr(Mnemonic.ldr, B(0, 5), Mpre(w8)),
                        Instr(Mnemonic.str, Q(0, 5), Mpre(w128)),
                        Instr(Mnemonic.ldr, Q(0, 5), Mpre(w128)),

                        Instr(Mnemonic.strh, W_0, Mpre(w16)),
                        Instr(Mnemonic.ldrh, W_0, Mpre(w16)),
                        Instr(Mnemonic.ldrsh, X_0, Mpre(i16)),
                        Instr(Mnemonic.ldrsh, W_0, Mpre(i16)),

                        Instr(Mnemonic.str, H(0, 5), Mpre(w16)),
                        Instr(Mnemonic.ldr, H(0, 5), Mpre(w16)),
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

                        Instr(Mnemonic.str, D(0,5), Mpre(w64)),
                        Instr(Mnemonic.ldr, D(0,5), Mpre(w64)),
                        invalid,
                        invalid);
                }

                Decoder LoadRegLit;
                {
                    LoadRegLit = Mask(Bf((30,2),(26,1)),    // opc:V
                        Instr(Mnemonic.ldr, W_0, Mlit(w32)),
                        Instr(Mnemonic.ldr, S_0, Mlit(w32)),
                        Instr(Mnemonic.ldr, X_0, Mlit(w64)),
                        Instr(Mnemonic.ldr, D(0,5), Mlit(w64)),
                        Instr(Mnemonic.ldrsw, X_0, Mlit(i32)),
                        Instr(Mnemonic.ldr, Q(0,5), Mlit(w128)),
                        Instr(Mnemonic.prfm, U(0,5, w8),Mlit(w32)),
                        invalid);
                }

                Decoder AdvancedSimdLdStMultiple;
                Decoder AdvancedSimdLdStMultiplePostIdx;
                Decoder AdvancedSimdLdStSingleStructure;
                {
                    AdvancedSimdLdStMultiple = Mask(Bf((22, 1), (12, 4)), // L:opcode
                        Mask(30,1,
                            Instr(Mnemonic.st4, Vmr(0,5,4,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.st4, q1,Vmr(0,5,4,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0011"),
                        Mask(30,1,
                            Instr(Mnemonic.st3, q(30),Vmr(0,5,3,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.st3, q(30),Vmr(0,5,3,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:0111"),
                        Mask(30,1,
                            Instr(Mnemonic.st2, Vmr(0,5,2,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.st2, Vmr(0,5,2,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1011"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1100"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=0:1111"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld4, Vmr(0,5,4,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.ld4, Vmr(0,5,4,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0011"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld3, Vmr(0,5,3,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.ld3, Vmr(0,5,3,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:0111"),
                        Mask(30, 1,
                            Instr(Mnemonic.ld2, Vmr(0,5,2,BHSD,10), Mb(w64,5,5)),
                            Instr(Mnemonic.ld2, Vmr(0,5,2,BHSD,10), Mb(w128,5,5))),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1001"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1010"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1011"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1100"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1101"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1110"),
                        Nyi("AdvancedSimdLdStMultiple L:opcode=1:1111"));

                    AdvancedSimdLdStMultiplePostIdx = Mask(Bf((22, 1), (12, 4)), // L:opcode
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0000"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0001"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0010"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0011"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0100"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0101"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0110"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:0111"),
                        Select((16,5), Is31,
                            Instr(Mnemonic.st2, q(30),Vmr(0,5,2,BBBB),MvmrPpost(2)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1000 Rm != 11111")),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1001"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1010"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1011"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1100"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1101"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1110"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=0:1111"),
                        Select((16,5), Is31, 
                            Instr(Mnemonic.ld4, q(30),Vmr(0,5,4,BBBB),MvmrPpost(4)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0000 Rm!=11111")),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0001"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0010"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0011"),
                        Select((16,5), Is31,
                            Instr(Mnemonic.ld3, q(30),Vmr(0,5,3,BBBB),MvmrPpost(3)),
                            Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0100 Rm != 11111")),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0101"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0110"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0111"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1000"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1001"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1010"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1011"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1100"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1101"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:1110"),
                        Nyi("AdvancedSimdLdStMultiplePostIdx L:opcode=1:0111"));

                    AdvancedSimdLdStSingleStructure = Mask(21,2,
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
                        Mask(13, 3, // L:R=1 0 opcode
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=000"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=001"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=010"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=011"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=100"),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=101"),
                            Mask(12, 1, // L:R=10 opcode=110 S
                                Mask(30, 1,
                                    Instr(Mnemonic.ld1r, q(30),Vmr(0,5,1,BHSD,10),Mb(w64,  5,5)),
                                    Instr(Mnemonic.ld1r, q(30),Vmr(0,5,1,BHSD,10),Mb(w128, 5,5))),
                                invalid),
                            Nyi("AdvancedSimdLdStSingleStructure L:R=1 0 opcode=111")),
                        Nyi("AdvancedSimdLdStSingleStructure L:R=1 1"));
                }

                Decoder LoadStoreExclusive = Mask(Bf((30, 2), (21, 3), (15, 1)),
                    Instr(Mnemonic.stxrb, W_16, W_0, Mb(w8, 5,5)),
                    Instr(Mnemonic.stlxrb, x("")),
                    Select((10, 5), Is31, Instr(Mnemonic.casp, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspl, x("32-bit")), invalid),
                    Instr(Mnemonic.ldxrb,  W_0, Mb(w8, 5,5)),
                    Instr(Mnemonic.ldaxrb, W_0, Mb(w8, 5, 5)),
                    Select((10, 5), Is31, Instr(Mnemonic.caspa, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspal, x("32-bit")), invalid),

                    Instr(Mnemonic.stllrb, x("")),
                    Instr(Mnemonic.stlrb, x("")),
                    Select((10, 5), Is31, Instr(Mnemonic.caspb, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.caspbl, x("32-bit")), invalid),
                    Instr(Mnemonic.ldlarb, x("")),
                    Instr(Mnemonic.ldarb, x("")),
                    Select((10, 5), Is31, Instr(Mnemonic.casab, x("32-bit")), invalid),
                    Select((10, 5), Is31, Instr(Mnemonic.casalb, x("32-bit")), invalid),

                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 010000"),
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
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100100"),
                    Nyi("LoadStoreExclusive size:o2:L:o1:o0 100101"),
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

                    Instr(Mnemonic.stxr, X_0, Mb(w64, 5,5)),
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

                LoadsAndStores = Mask(31, 1, "Loads and stores",
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
                                Select((21, 1), IsZero,
                                    AdvancedSimdLdStSingleStructure,
                                    invalid),
                                Nyi("AdvancedSimdLdStSingleStructure"))),
                        Mask(23, 2,      // op0 = 0, op1 = 1
                            LoadRegLit,
                            LoadRegLit,
                            invalid,
                            invalid),
                        Mask(23, 2,      // op0 = 0, op1 = 2
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
                                    Nyi("*AtomicMemoryOperations"),
                                    Nyi("*LoadStoreRegister PAC"),
                                    LdStRegisterRegOff,
                                    Nyi("*LoadStoreRegister PAC"))),
                            LdStRegUImm)),
                    Mask(28, 2, "  op0=1 op1",
                        Mask(26, 1, "LdSt op0=1 op1=00 op2=?",
                            Mask(23, 2, "LdSt op0=1 op1=00 op2=0 op3=??",
                                LoadStoreExclusive,
                                LoadStoreExclusive,
                                Nyi("LdSt op0=1 op1=00 op2=0 op3=10"),
                                Nyi("LdSt op0=1 op1=00 op2=0 op3=11")),
                            Nyi("LdSt op0=1 op1=00 op2=1")),
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
                    
                    Instr(Mnemonic.add, Xs(0,5),Xs(5,5),U(10,12,w64),sc(22,2)),
                    Instr(Mnemonic.adds, Xs(0,5),Xs(5,5),U(10,12,w64),sc(22,2)),
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
                Bitfield = Mask(22, 1,
                    Mask(29, 3,
                        Instr(Mnemonic.sbfm, W_0,W_5,U(16,6,i32),U(10,6,i32), SbfmAliases),
                        Instr(Mnemonic.bfm, W_0,W_5,U(16,6,i32),U(10,6,i32)),
                        Instr(Mnemonic.ubfm, W_0,W_5,U(16,6,i32),U(10,6,i32), UbfmAliases),
                        invalid,

                        invalid,
                        Instr(Mnemonic.Invalid, x("*BOGOTRON")),
                        invalid,
                        invalid),
                    Mask(29, 3,
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

            var DataProcessingImm = Mask(23, 3,
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
                Mask(21, 4,
                    Sparse(10, 6,
                        invalid,
                        (0, Select((0, 5), n => n == 0, Instr(Mnemonic.br, InstrClass.Transfer, X_5), invalid)),
                        (2, Select((0, 5), n => n == 0x1F, Nyi("BRAA,BRAAZ... Key A"), invalid)),
                        (3, Select((0, 5), n => n == 0x1F, Nyi("BRAA,BRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select((0, 5), n => n == 0, Instr(Mnemonic.blr, InstrClass.Transfer | InstrClass.Call, X_5), invalid)),
                        (2, Select((0, 5), n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key A"), invalid)),
                        (3, Select((0, 5), n => n == 0x1F, Nyi("BlRAA,BlRAAZ... Key B"), invalid))),
                    Sparse(10, 6,
                        invalid,
                        (0, Select((0, 5), n => n == 0, Instr(Mnemonic.ret, InstrClass.Transfer, X_5), invalid)),
                        (2, Select((0, 5), n => n == 0x1F, Nyi("RETAA,RETAAZ... Key A"), invalid)),
                        (3, Select((0, 5), n => n == 0x1F, Nyi("RETAA,RETAAZ... Key B"), invalid))),
                    invalid,

                    Select((5,5), n => n == 0x1F,
                        Sparse(10, 6,
                            invalid,
                            (0, Select((0, 5), n => n == 0, Instr(Mnemonic.eret), invalid)),
                            (2, Select((0, 5), n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key A"), invalid)),
                            (3, Select((0, 5), n => n == 0x1F, Nyi("ERETAA,RETAAZ... Key B"), invalid))),
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
                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.nop), invalid),
                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.yield, x("*")), invalid),
                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.wfe, x("*")), invalid),
                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.wfi, x("*")), invalid),

                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.sev, x("*")), invalid),
                                Select((0, 5), n => n == 0x1F, Instr(Mnemonic.sevl, x("*")), invalid),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=110"),
                                Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0000 op2=111")),
                            Nyi("System L:op0 = 0b000 op1=0b011 crN=0010 crM=0001"),
                            Sparse(5, 7, // System L:op0 = 0b000 op1=0b011 crN=0010 crM=0010 op2=???
                                Sparse(5, 0x7F,
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
                (0b001_000_00, Instr(Mnemonic.brk, U(5,16, PrimitiveType.Word16))),
                (0b010_000_00, Instr(Mnemonic.hlt, U(5,16, PrimitiveType.Word16))),
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
                            Select(Bf((22,2),(10,6),(5,5)), n => n == 0x1F,
                                Instr(Mnemonic.mov, W_0,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.orr, W_0,W_5,W_16,si(22,2,10,6))),
                            Select((5,5), n => n == 0x1F,
                                Instr(Mnemonic.mvn, W_0,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.orn, W_0,W_5,W_16,si(22,2,10,6))),

                            Instr(Mnemonic.eor, W_0,W_5,W_16,si(22,2,10,6)),
                            Instr(Mnemonic.eon, x("*shifted register, 32-bit")),
                            Select((0, 5), n => n == 0x1F,
                                Instr(Mnemonic.test, W_5,W_16,si(22,2,10,6)),
                                Instr(Mnemonic.ands, W_0,W_5,W_16,si(22,2,10,6))),
                            Instr(Mnemonic.bics, W_0,W_5,W_16,si(22,2,10,6)))),
                    Mask(Bf((29,2),(21,1)),
                        Instr(Mnemonic.and, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.bic, X_0,X_5,X_16,si(22,2,10,6)),
                        Select(Bf((22,2),(10,6),(5,5)), n => n == 0x1F,
                            Instr(Mnemonic.mov, X_0,X_16,si(22,2,10,6)),
                            Instr(Mnemonic.orr, X_0,X_5,X_16,si(22,2,10,6))),
                        Select((5,5), n => n == 0x1F,
                            Instr(Mnemonic.mvn, X_0,X_16,si(22,2,10,6)),
                            Instr(Mnemonic.orn, X_0,X_5,X_16,si(22,2,10,6))),

                        Instr(Mnemonic.eor, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.eon, X_0,X_5,X_16,si(22,2,10,6)),
                        Select((0, 5), n => n == 0x1F,
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
                            Select((0, 5), n => n == 0x1F,
                                Instr(Mnemonic.cmp, Ws(5,5),W_16,si(22,2,10,6)),
                                Instr(Mnemonic.subs, W_0,W_5,W_16,si(22,2,10,6))))),
                    Mask(29, 2,
                        Instr(Mnemonic.add,  X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.adds, X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.sub,  X_0,X_5,X_16,si(22,2,10,6)),
                        Instr(Mnemonic.subs, X_0,X_5,X_16,si(22,2,10,6))));
            }

            var AddSubExtendedRegister = Select((22, 2), n => n != 0,
                invalid,
                Mask(29, 3,
                    Instr(Mnemonic.add, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.adds, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.sub, Ws(0,5),Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Select((0, 5), n => n == 0x1F,
                        Instr(Mnemonic.cmp, Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Mnemonic.subs, W_0,Ws(5,5),Rx(16,5,13,3),Ex(13,3,10,3))),

                    Instr(Mnemonic.add, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.adds, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Instr(Mnemonic.sub, Xs(0,5),Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                    Select((0, 5), n => n == 0x1F,
                        Instr(Mnemonic.cmp, Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)),
                        Instr(Mnemonic.subs, X_0,Xs(5,5),Rx(16,5,13,3),Ex(13,3,10,3)))));

            Decoder DataProcessing3Source;
            {
                DataProcessing3Source = Mask(29, 3,
                    Mask(21, 3,
                        Mask(15, 1,
                            Select((10, 5), n => n == 0x1F,
                                Instr(Mnemonic.mul, W_0, W_5, W_16),
                                Instr(Mnemonic.madd, W_0,W_5,W_16,W(10,5))),
                            Select((10, 5), n => n == 0x1F,
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
                            Select((10, 5), n => n == 0x1F,
                                Instr(Mnemonic.mul, X_0,X_5,X_16),
                                Instr(Mnemonic.madd, X_0,X_5,X_16,X_10)),
                            Select((10, 5), n => n == 0x1F,
                                Instr(Mnemonic.mneg, X_0,X_5,X_16),
                                Instr(Mnemonic.msub, X_0,X_5,X_16,X_10))),
                        Mask(15, 1,
                            Select((10, 5), n => n == 0x1F,
                                Instr(Mnemonic.smull, X_0,W_5,W_16),
                                Instr(Mnemonic.smaddl, X_0,W_5,W_16,X_10)),
                            Select((10, 5), n => n == 0x1F,
                                Instr(Mnemonic.smnegll, X_0,W_5,W_16),
                                Instr(Mnemonic.smsubl, X_0,W_5,W_16,X_10))),
                        Mask(15, 1,
                            Instr(Mnemonic.smulh, X_0,W_5,W_16),
                            invalid),
                        invalid,

                        invalid,
                        Mask(15, 1,
                            Instr(Mnemonic.umaddl, X_0,W_5,W_16,X_10),
                            Instr(Mnemonic.umsubl, X_0,W_5,W_16,X_10)),
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
                        Instr(Mnemonic.ccmn, W_5,W_16,U(0,4,w8),C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, W_5,W_16,U(0,4,w8),C(12,4)),

                        invalid,
                        Instr(Mnemonic.ccmn, X_5,X_16,U(0,4,w8),C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, X_5,X_16,U(0,4,w8),C(12,4))),
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
                        Instr(Mnemonic.ccmn, W_5, I(16,5,w32), U(0,4,w8), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, W_5, I(16,5,w32), U(0,4,w8), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmn, X_5, I(16,5,w64), U(0,4,w8), C(12,4)),
                        invalid,
                        Instr(Mnemonic.ccmp, X_5, I(16,5,w64), U(0,4,w8), C(12,4))));
            }

            Decoder DataProcessing1source;
            {
                DataProcessing1source = Mask(Bf((31,1),(29,1)), // sf:S 
                    Sparse(16, 0b11111, // sf:S=00
                        Nyi("DataProcessing1source sf:S=00 opcode2=?????"),
                        (0b00000, Sparse(10, 0x3F,      // sf:S=00 opcode2=00000 opcode
                            Nyi("DataProcessing1source sf:S=00 opcode2=00000 opcode=??????"),
                            (0b000000, Instr(Mnemonic.rbit, W_0,W_5)),
                            (0b000001, Instr(Mnemonic.rev16, W_0,W_5)),
                            (0b000010, Instr(Mnemonic.rev, W_0, W_5)),
                            (0b000100, Instr(Mnemonic.clz, W_0, W_5)),
                            (0b000101, Instr(Mnemonic.cls, W_0, W_5))
                            ))
                        ),
                    Nyi("DataProcessing1source sf:S=01"),
                    Sparse(16, 0b11111, // sf:S=00
                        Nyi("DataProcessing1source sf:S=10"),
                        (0b00000, Sparse(10, 0x3F,      // sf:S=10 opcode2=00000 opcode
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
                            Instr(Mnemonic.crc32x, W_0, W_5, X(16, 5))),
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
                ConversionBetweenFpAndInt = Mask(Bf((31, 1), (29, 1)),
                    Mask(22, 2,      // sf:S=0b00 type
                        Sparse(16, 0b11111,  // sf:S=0b00 type=00 rmode:opcode
                            Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=00"),
                            (0b00_010, Instr(Mnemonic.scvtf, S_0,W_5)),
                            (0b00_011, Instr(Mnemonic.ucvtf, S_0,W_5)),
                            (0b00_110, Instr(Mnemonic.fmov,  W_0,S(5,5))),
                            (0b00_111, Instr(Mnemonic.fmov,  S_0,W_5)),
                            (0b01_000, Instr(Mnemonic.fcvtps, W_0,S(5,5))),
                            (0b10_000, Instr(Mnemonic.fcvtms, W_0,S(5,5))),
                            (0b11_000, Instr(Mnemonic.fcvtzs, W_5,S_0)),
                            (0b11_001, Instr(Mnemonic.fcvtzu, W_5,S_0))),
                        Sparse(16, 0b11111,  // sf:S=0b00 type=01 rmode:opcode
                            Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=01"),
                            (0b00_010, Instr(Mnemonic.scvtf, D(0,5),W_5))
                            ),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=10"),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b00 type=11")),
                    invalid,
                    Mask(22, 2,      // sf:S=0b00 type
                        Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=00"),
                        Sparse(16, 0b11111,  // sf:S=0b10 type=01
                            Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=01"),
                            (0b00_111, Instr(Mnemonic.fmov, D(0,5),X_5))
                            ),
                        Sparse(16, 0b11111, // sf:S=0b10 type=10
                            Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=10"),
                            (0b01_111, Instr(Mnemonic.fmov, Vri(0,5,w128,VectorData.I64, 1),X_5))),
                        Nyi("ConversionBetweenFpAndInt sf:S=0b10 type=11")),
                    invalid);
            }

            Decoder ConversionBetweenFpAndFixedPoint;
            {
                ConversionBetweenFpAndFixedPoint = Mask(Bf((31, 1), (29, 1), (22, 2)), // sf:S:type
                    Mask(16, 3, // sf:S:type=0 0 00 opcode 
                        Instr(Mnemonic.scvtf, S_0,W_5,Fxs(10,6)),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=001"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=010"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=011"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=100"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=101"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=110"),
                        Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 00 opcode=111")),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 01"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 10"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 0 11"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 1 00"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 1 01"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 1 10"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=0 1 11"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 00"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 01"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 10"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 0 11"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 1 00"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 1 01"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 1 10"),
                    Nyi("ConversionBetweenFpAndFixedPoint sf:S:type=1 1 11"));
            }

            Decoder AdvancedSimd3Same;
            {
                AdvancedSimd3Same = Mask(29, 1,
                    Sparse(11, 0b11111,
                        Nyi("AdvancedSimd3Same U=0"),
                        (0b00011, Mask(22, 2,    // U=0 opcode=00011 size
                            Nyi("AdvancedSimd3Same U=0 opcode=00011 size=00"),
                            Nyi("AdvancedSimd3Same U=0 opcode=00011 size=01"),
                            Select((0, 21), Rn_Rm_Same,
                                Instr(Mnemonic.mov, q(30), Vr(0,5,BBB_,30), Vr(5,5,BBB_,30)),                     // U=0 opcode=00011 size=10
                                Instr(Mnemonic.orr, q(30), Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30))),  // U=0 opcode=00011 size=10
                            Nyi("AdvancedSimd3Same U=0 opcode=00011 size=11"))),
                        (0b01100, Instr(Mnemonic.smax, q(30),Vr(0,5,BHS_), Vr(5,5,BHS_),Vr(16,5,BHS_))),
                        (0b10000, Instr(Mnemonic.add, q(30),Vr(0,5,BHSD),Vr(5,5,BHSD),Vr(16,5,BHSD))),
                        (0b10011, Instr(Mnemonic.mul, q(30),Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_))),
                        (0b10110, Instr(Mnemonic.sqdmulh, q(30),Vr(0,5,BHS_),Vr(5,5,BHS_),Vr(16,5,BHS_))),
                        (0b11010, Mask(23, 1,       // U=0 opcode=11010 size=?x
                            Instr(Mnemonic.fadd, q(30),Vr(0,5,SSDD),Vr(5,5,SSDD),Vr(16,5,SSDD)),
                            Instr(Mnemonic.fsub, x("vector"))))),
                    Mask(11, 5, // U=1 opcode
                        Nyi("AdvancedSimd3Same U=1 opcode=00000"),
                        Nyi("AdvancedSimd3Same U=1 opcode=00001"),
                        Nyi("AdvancedSimd3Same U=1 opcode=00010"),
                        Mask(22, 2, // U=1 opcode=00011 size
                            Instr(Mnemonic.eor, q(30),Vr(0,5,BBB_,30), Vr(5,5,BBB_,30), Vr(16,5,BBB_,30)),
                            Nyi("AdvancedSimd3Same U=1 opcode=00011 size=01"),
                            Nyi("AdvancedSimd3Same U=1 opcode=00011 size=10"),
                            Nyi("AdvancedSimd3Same U=1 opcode=00011 size=11")),
                        Nyi("AdvancedSimd3Same U=1 opcode=00100"),
                        Nyi("AdvancedSimd3Same U=1 opcode=00101"),
                        Nyi("AdvancedSimd3Same U=1 opcode=00110"),
                        Nyi("AdvancedSimd3Same U=1 opcode=00111"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01000"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01001"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01010"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01011"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01100"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01101"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01110"),
                        Nyi("AdvancedSimd3Same U=1 opcode=01111"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10000"),
                        Instr(Mnemonic.cmeq, q1, Vr(0,5,BBBB),Vr(5,5,BBBB),Vr(16,5,BBBB)),
                        Nyi("AdvancedSimd3Same U=1 opcode=10010"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10011"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10100"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10101"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10110"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10111"),
                        Nyi("AdvancedSimd3Same U=1 opcode=10000"),
                        Nyi("AdvancedSimd3Same U=1 opcode=11001"),
                        Mask(22, 2, // U=1 opcode=00011 size
                            Instr(Mnemonic.fadd, VectorData.F32, q(30),V(0,5),V(5,5),V(16,5)),
                            Instr(Mnemonic.fadd, VectorData.F64, q(30),V(0,5),V(5,5),V(16,5)),
                            Nyi("AdvancedSimd3Same U=1 opcode=11010 size=10"),
                            Nyi("AdvancedSimd3Same U=1 opcode=11010 size=11")),
                        Mask(23, 2, // U=1 opcode=00011 size=?x
                            Instr(Mnemonic.fmul, VectorData.F32, q(30),V(0,5),V(5,5),V(16,5)),
                            Instr(Mnemonic.fmul, VectorData.F64, q(30),V(0,5),V(5,5),V(16,5)),
                            invalid,
                            invalid),
                        Nyi("AdvancedSimd3Same U=1 opcode=11100"),
                        Nyi("AdvancedSimd3Same U=1 opcode=11101"),
                        Nyi("AdvancedSimd3Same U=1 opcode=11110"),
                        Nyi("AdvancedSimd3Same U=1 opcode=11111")));
            }

            Decoder AdvancedSimd3Different;
            {
                AdvancedSimd3Different = Mask(Bf((29, 1), (12, 4)), // U:opcode
                    Nyi("AdvancedSimd3Different U:opcode=0 0000"),
                    Nyi("AdvancedSimd3Different U:opcode=0 0001"),
                    Nyi("AdvancedSimd3Different U:opcode=0 0010"),
                    Nyi("AdvancedSimd3Different U:opcode=0 0011"),
                    Mask(30, 1,
                        Instr(Mnemonic.addhn, q(30),Vr(0,5,BHS_),Vr(5,5,HSD_),Vr(16,5,HSD_)),
                        Instr(Mnemonic.addhn2, q(30),Vr(0,5,BHS_),Vr(5,5,HSD_),Vr(16,5,HSD_))),
                    Nyi("AdvancedSimd3Different U:opcode=0 0101"),
                    Nyi("AdvancedSimd3Different U:opcode=0 0110"),
                    Nyi("AdvancedSimd3Different U:opcode=0 0111"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1000"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1001"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1010"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1011"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1100"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1101"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1110"),
                    Nyi("AdvancedSimd3Different U:opcode=0 1111"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0000"),
                    Instr(Mnemonic.uaddw, q(30), Vr(0, 5, HSD_), Vr(5, 5, HSD_), Vr(16, 5, BHS_)),
                    Nyi("AdvancedSimd3Different U:opcode=1 0010"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0011"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0100"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0101"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0110"),
                    Nyi("AdvancedSimd3Different U:opcode=1 0111"),
                    Instr(Mnemonic.umlal, q1,Vr(0,5,HSD_),q(30),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                    Nyi("AdvancedSimd3Different U:opcode=1 1001"),
                    Nyi("AdvancedSimd3Different U:opcode=1 1010"),
                    Nyi("AdvancedSimd3Different U:opcode=1 1011"),
                    Instr(Mnemonic.umull, q1,Vr(0,5,HSD_),q(30),Vr(5,5,BHS_),Vr(16,5,BHS_)),
                    Nyi("AdvancedSimd3Different U:opcode=1 1101"),
                    Nyi("AdvancedSimd3Different U:opcode=1 1110"),
                    Nyi("AdvancedSimd3Different U:opcode=1 1111"));
            }

            Decoder AdvancedSIMDscalar2RegMisc;
            {
                AdvancedSIMDscalar2RegMisc = Mask(29, 1,
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
                            Instr(Mnemonic.scvtf, S_0,S(5,5)),
                            Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11101 size=01"),
                            Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11101 size=10"),
                            Nyi("AdvancedSIMDscalar2RegMisc U=0 opcode=11101 size=11")),
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

            Decoder FloatingPointDataProcessing1src;
            {
                FloatingPointDataProcessing1src = Mask(Bf((31, 1), (29, 1), (22, 2)),  // M:S:Type
                    Sparse(15, 0x3F,    // M:S:Type=00 00
                        Nyi("FloatingPointDataProcessing1src M:S:Type=00 00"),
                        (0b000000, Instr(Mnemonic.fmov, S_0,S(5,5))),
                        (0b000001, Instr(Mnemonic.fabs, S_0,S(5,5))),
                        (0b000010, Instr(Mnemonic.fneg, S_0,S(5,5))),
                        (0b000011, Instr(Mnemonic.fsqrt, S_0,S(5,5))),
                        (0b000101, Instr(Mnemonic.fcvt, D(0,5),S(5,5))),
                        (0b000111, Instr(Mnemonic.fcvt, H(0,5),S(5,5)))
                        ),
                    Sparse(15, 0x3F,    // M:S:Type=00 01
                        Nyi("FloatingPointDataProcessing1src M:S:Type=00 01"),
                        (0b000000, Instr(Mnemonic.fmov, D(0,5),D(5,5))),
                        (0b000001, Instr(Mnemonic.fabs, D(0,5),D(5,5))),
                        (0b000010, Instr(Mnemonic.fneg, D(0,5),D(5,5))),
                        (0b000011, Instr(Mnemonic.fsqrt,D(0,5),D(5,5))),
                        (0b000100, Instr(Mnemonic.fcvt, S_0,D(5,5))),
                        (0b000111, Instr(Mnemonic.fcvt, H(0,5),D(5,5)))
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
                        Instr(Mnemonic.fmul, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fdiv, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fadd, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fsub, S_0,S(5,5),S(16,5)),

                        Instr(Mnemonic.fmax, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fmin, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fmaxnm, S_0,S(5,5),S(16,5)),
                        Instr(Mnemonic.fnmul, S_0,S(5,5),S(16,5)),

                        Instr(Mnemonic.fnmul, S_0,S(5,5),S(16,5)),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Mask(12, 4,            // M:S:Type=0 0 01 opcode
                        Instr(Mnemonic.fmul, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fdiv, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fadd, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fsub, D(0,5),D(5,5),D(16,5)),

                        Instr(Mnemonic.fmax, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fmin, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fmaxnm, D(0,5),D(5,5),D(16,5)),
                        Instr(Mnemonic.fnmul, D(0,5),D(5,5),D(16,5)),

                        Instr(Mnemonic.fnmul, D(0,5),D(5,5),D(16,5)),
                        invalid,
                        invalid,
                        invalid,

                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    invalid,
                    Mask(12, 4,            // M:S:Type=0 0 11 opcode
                        Instr(Mnemonic.fmul, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fdiv, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fadd, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fsub, H(0,5),H(5,5),H(16,5)),

                        Instr(Mnemonic.fmax, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fmin, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fmaxnm, H(0,5),H(5,5),H(16,5)),
                        Instr(Mnemonic.fnmul, H(0,5),H(5,5),H(16,5)),

                        Instr(Mnemonic.fnmul, H(0,5),H(5,5),H(16,5)),
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
                            Instr(Mnemonic.fmov, D(0,5),If64(13,8)),
                            invalid,
                            Instr(Mnemonic.fmov, H(0,5),If16(13,8))),
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
                            Instr(Mnemonic.fcmp,  S(5,5),S(16,5)),
                            Instr(Mnemonic.fcmp,  S(5,5),Sz(16,5)),
                            Instr(Mnemonic.fcmpe, S(5,5),S(16,5)),
                            Instr(Mnemonic.fcmpe, S(5,5),Sz(16,5)),
                            Instr(Mnemonic.fcmp,  D(5,5),D(16,5)),
                            Instr(Mnemonic.fcmp,  D(5,5),Dz(16,5)),
                            Instr(Mnemonic.fcmpe, D(5,5),D(16,5)),
                            Instr(Mnemonic.fcmpe, D(5,5),Dz(16,5)),
                            invalid,
                            invalid,
                            invalid,
                            invalid,
                            Instr(Mnemonic.fcmp,  H(5,5),H(16,5)),
                            Instr(Mnemonic.fcmp,  H(5,5),Hz(16,5)),
                            Instr(Mnemonic.fcmpe, H(5,5),H(16,5)),
                            Instr(Mnemonic.fcmpe, H(5,5),Hz(16,5)))),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder FloatingPointCondSelect;
            {
                FloatingPointCondSelect = Mask(Bf((31, 1), (29, 1)),   // M:S
                    Mask(22, 2,  // M:S=00 type
                        Instr(Mnemonic.fcsel, S_0,S(5,5),S(16,5),C(12,4)),
                        Instr(Mnemonic.fcsel, D(0,5),D(5,5),D(16,5),C(12,4)),
                        invalid,
                        Instr(Mnemonic.fcsel, H(0,5),H(5,5),H(16,5),C(12,4))),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder FloatingPointCondCompare;
            {
                FloatingPointCondCompare = Mask(Bf((31, 1), (29, 1)),   // M:S
                    Mask(22, 2, // M:S=00 type
                        Nyi("FloatingPointCondCompare M:S=00 type=00"),
                        Nyi("FloatingPointCondCompare M:S=00 type=01"),
                        invalid,    // M:S=00 type=00
                        Nyi("FloatingPointCondCompare M:S=00 type=11")),
                    invalid,
                    invalid,
                    invalid);
            }

            Decoder AdvancedSimdShiftByImm;
            {
                AdvancedSimdShiftByImm = Select((19, 4), IsZero,
                    invalid,
                    Mask(29, 1,
                        Sparse(11,0b11111, // U=0
                            Nyi("AdvancedSimdShiftByImm U=0"),
                            (0b00000, Instr(Mnemonic.sshr, q(30),Vr(0,5,BHS_,20),q1,Vr(5,5,BHS_,20),ShrnShift)),
                            (0b10000, Instr(Mnemonic.shrn, q(30),Vr(0,5,BHS_,20),q1,Vr(5,5,HSD_,20),ShrnShift)),
                        (0b10100, Select((16, 3), n => n == 0, 
                            Instr(Mnemonic.sxtl, q1,As(19,4),V(0,5), q(30),V(5,5)),
                            Instr(Mnemonic.sshll, x("sshll"))))
                            ),
                        Sparse(11, 0b11111, // U=1
                            Nyi("AdvancedSimdShiftByImm U=1"),
                            (0b10100, Select((16, 3), n => n == 0,
                                Mask(30, 1,
                                    Instr(Mnemonic.uxtl, q1,Vr(0,5,HSD_,20),q(30),Vr(5,5,BHS_,20)),
                                    Instr(Mnemonic.uxtl2, q(30),Vr(0,5,HSD_,20),Vr(5,5,BHS_128,20))),
                                Mask(30, 1,       // U=1 Q
                                    Instr(Mnemonic.ushll, x("")),
                                    Instr(Mnemonic.ushll2, x(""))))))));
            }

            Decoder AdvancedSimdModifiedImm;
            {
                AdvancedSimdModifiedImm = Sparse("AdvancedSimdModifiedImm", 12, 0b1111,
                    Nyi("AdvancedSimdModifiedImm cmode"),
                    (0b0000, Mask(Bf((29, 2), (11, 1)),  // cmode=0b1110 Q:op:op2
                        Instr(Mnemonic.movi, q(30),Vr(0,5,SSSS),Is64(16,3,5,5,29,1,12,4)),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=001"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=010"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=011"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=100"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=101"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=110"),
                        Nyi("AdvancedSimdModifiedImm cmode=0000 Q:op:op2=111"))),
                    (0b1000, Mask(Bf((29, 2), (11, 1)),  // cmode=0b1110 Q:op:op2
                        Instr(Mnemonic.movi, q(30),Vr(0,5,HHHH),Is64(16,3,5,5,29,1,12,4)),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=001"),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=010"),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=011"),
                        Instr(Mnemonic.movi, q(30),Vr(0,5,HHHH),Is64(16,3,5,5,29,1,12,4)),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=101"),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=110"),
                        Nyi("AdvancedSimdModifiedImm cmode=1000 Q:op:op2=111"))),
                    (0b1110, Mask(Bf((29, 2), (11, 1)),  // cmode=0b1110 Q:op:op2
                        Instr(Mnemonic.movi, q(30),Vr(0,5,BBBB),Is64(16,3,5,5,29,1,12,4)),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=001"),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=010"),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=011"),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=100"),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=101"),
                        Instr(Mnemonic.movi, q(30),Vr(0,5,DDDD),Is64(16,3,5,5,29,1,12,4)),
                        Nyi("AdvancedSimdModifiedImm cmode=1110 Q:op:op2=111"))),

                    (0b1111, Mask(Bf((29, 2), (11, 1)),  // cmode=0b1111 Q:op:op2
                        Instr(Mnemonic.fmov, q(30),Vr(0,5,SSSS,30), If32(16,3,5,5)),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=001"),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=010"),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=011"),
                        Instr(Mnemonic.fmov, q(30),Vr(0,5,SSSS,30), If32(16,3,5,5)),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=101"),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=110"),
                        Nyi("AdvancedSimdModifiedImm cmode=1111 Q:op:op2=111"))));
            }

            Decoder AdvancedSimd2RegMisc;  // C4-298
            {
                AdvancedSimd2RegMisc = Mask(29, 1,
                    Mask(12, 5,
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00000"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00001"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00010"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00011"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00100"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00101"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00110"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=00111"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01000"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01001"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01010"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01011"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01100"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01101"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01110"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=01111"),
                        invalid,
                        invalid,
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=10010"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=10011"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=10100"),
                        invalid,
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=10110"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=10111"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=11000"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=11001"),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=11010"),
                        Mask(22, 2,
                            Instr(Mnemonic.fcvtms, x("vector")),
                            Instr(Mnemonic.fcvtms, x("vector")),
                            Instr(Mnemonic.fcvtzs, VectorData.F32, q(30),V(0,5),V(5,5)),
                            Instr(Mnemonic.fcvtzs, VectorData.F64, q(30),V(0,5),V(5,5))),
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=11100"),
                        Mask(22, 2,
                            Instr(Mnemonic.scvtf, VectorData.I32, q(30),V(0,5),V(5,5)),
                            Nyi("AdvancedSimd2RegMisc U=0 opcode=11101 size=01"),
                            Nyi("AdvancedSimd2RegMisc U=0 opcode=11101 size=10"),
                            Nyi("AdvancedSimd2RegMisc U=0 opcode=11101 size=11")),
                        invalid,
                        Nyi("AdvancedSimd2RegMisc U=0 opcode=11111")),
                    Sparse(12, 0b11111,
                        Nyi("AdvancedSimd2RegMisc U=1"),
                        (0b00000, Instr(Mnemonic.rev32, x("AdvancedSimd2RegMisc U=1 opcode=00000"))),
                        (0b00101, Mask(22, 2,
                            Instr(Mnemonic.not, q(30),Vr(0,5,BBBB),Vr(5,5,BBBB)),
                            Nyi("AdvancedSimd2RegMisc U=1 opcode=00101 size=01"),
                            Nyi("AdvancedSimd2RegMisc U=1 opcode=00101 size=10"),
                            Nyi("AdvancedSimd2RegMisc U=1 opcode=00101 size=11")))));
            }

            Decoder AdvancedSimdAcrossLanes;
            {
                AdvancedSimdAcrossLanes = Sparse(12, 0b11111, // opcode
                    Nyi("AdvancedSimdAcrossLanes opcode"),
                    (0b01010, Mask(29, 1,    // opcode=01010 U=0 size
                        Mask(22, 2,       // opcode=01010 U=0 size
                            Instr(Mnemonic.smaxv, q(30), B(0, 5), Vr(5, 5, BHS_)),
                            Instr(Mnemonic.smaxv, q(30), H(0, 5), Vr(5, 5, BHS_)),
                            Instr(Mnemonic.smaxv, q(30), S_0, Vr(5, 5, BHS_)),
                            invalid),
                        Nyi("AdvancedSimdAcrossLanes opcode=01010 U=1"))),
                    (0b11011, Mask(29, 1,
                        Mask(22,2,   // opcode=11011 U=0 size
                            Instr(Mnemonic.addv, q(30),B(0,5),Vr(5,5,BHS_)),
                            Instr(Mnemonic.addv, q(30),H(0,5),Vr(5,5,BHS_)),
                            Instr(Mnemonic.addv, q(30),S_0,Vr(5,5,BHS_)),
                            invalid),
                        Nyi("AdvancedSimdAcrossLanes opcode=11011 U=1"))));
            }

            Decoder AdvancedSimdCopy;
            {
                AdvancedSimdCopy = Select((16, 4), IsZero,
                    invalid,
                    Mask(29, 2,    // Q:op
                        Nyi("AdvancedSIMDcopy Q:op=00"),
                        Nyi("AdvancedSIMDcopy Q:op=01"),
                        Sparse(11, 0b1111,
                            invalid,
                            (0b0000, Instr(Mnemonic.dup, q(30),Vrs(0,5,16,5,false,false),Vrs(5,5,16,5,true,false))),
                            (0b0001, Select((16, 4), n => n == 0b1000,
                                Instr(Mnemonic.dup, q(30),Vrs(0,5,16,5,false,false),X_5),
                                Instr(Mnemonic.dup, q(30),Vrs(0,5,16,5,false,false),W_5)))),
                        Instr(Mnemonic.mov, q(30),Vrs(0,5,16,5,true,false),Vrs(5,5,11,4,true,true))));
            }

            Decoder AdvancedSimdExtract = Select((22, 2), IsZero,
                Instr(Mnemonic.ext, q(30), Vr(0, 5, BBBB), Vr(5, 5, BBBB), Vr(16, 5, BBBB), U(11, 4, PrimitiveType.Byte)),
                invalid);

            Decoder AdvancedSIMD2RegMisc;
            {
                AdvancedSIMD2RegMisc = Mask(29, 1,
                    Sparse(12, 0b11111,
                        Nyi("AdvancedSIMD2RegMisc U=0"),
                        (0b10010, Mask(30, 1,
                            Instr(Mnemonic.xtn, q(30),Vr(0,5,BHS_),q1,Vr(5,5,HSD_)),
                            Instr(Mnemonic.xtn2, x(""))))),
                    Nyi("AdvancedSIMD2RegMisc U=1"));
            }

            Decoder AdvancedSimdTableLookup;
            {
                AdvancedSimdTableLookup = Select((22, 2), IsZero,
                    Mask(12, 3, // len:op
                        Instr(Mnemonic.tbl, x("1 register variant")),
                        Instr(Mnemonic.tbx, x("1 register variant")),
                        Instr(Mnemonic.tbl, x("2 register variant")),
                        Instr(Mnemonic.tbx, x("2 register variant")),
                        Instr(Mnemonic.tbl, q(30),Vr(0,5,BBBB),Vmr(5,5,3,BBBB), Vr(0,5,BBBB)),
                        Instr(Mnemonic.tbx, x("3 register variant")),
                        Instr(Mnemonic.tbl, x("4 register variant")),
                        Instr(Mnemonic.tbx, x("4 register variant"))),
                    invalid);
            }

            Decoder AdvancedSimdScalar_x_IdxElem;
            {
                AdvancedSimdScalar_x_IdxElem = Mask(12, 4, // opcode
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0001"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0011"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0101"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=0111"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1001"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1011"),
                    Mask(29, 1,
                        Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1100 U=0"),
                        invalid),
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1101"),
                    invalid,
                    Nyi("AdvancedSimdScalar_x_IdxElem - opcode=1111"));
            }

            Decoder DataProcessingScalarFpAdvancedSimd;
            {
                Decoder FloatingPointDecoders = Mask(10, 2,  // op3=xxxxxxx??
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
                        Instr(Mnemonic.fmadd, S_0,S(5,5),S(16,5),S(10,5)),
                        Instr(Mnemonic.fmsub, S_0,S(5,5),S(16,5),S(10,5)),
                        Instr(Mnemonic.fnmadd, S_0,S(5,5),S(16,5),S(10,5)),
                        Instr(Mnemonic.fnmsub, S_0,S(5,5),S(16,5),S(10,5))),
                    Mask(Bf((21,1),(15,1)),
                        Instr(Mnemonic.fmadd, D(0,5),D(5,5),D(16,5),D(10,5)),
                        Instr(Mnemonic.fmsub, D(0,5),D(5,5),D(16,5),D(10,5)),
                        Instr(Mnemonic.fnmadd, D(0,5),D(5,5),D(16,5),D(10,5)),
                        Instr(Mnemonic.fnmsub, D(0,5),D(5,5),D(16,5),D(10,5))),
                    Nyi("FloatingPointDataProcessing3src - M:S:type=0010"),
                    Mask(Bf((21, 1), (15, 1)),
                        Instr(Mnemonic.fmadd, H(0,5),H(5,5),H(16,5),H(10,5)),
                        Instr(Mnemonic.fmsub, H(0,5),H(5,5),H(16,5),H(10,5)),
                        Instr(Mnemonic.fnmadd, H(0,5),H(5,5),H(16,5),H(10,5)),
                        Instr(Mnemonic.fnmsub, H(0,5),H(5,5),H(16,5),H(10,5))),
                    Nyi("FloatingPointDataProcessing3src - M:S:type=0100"),
                    Nyi("FloatingPointDataProcessing3src - M:S:type=0101"),
                    Nyi("FloatingPointDataProcessing3src - M:S:type=0110"),
                    Nyi("FloatingPointDataProcessing3src - M:S:type=0111"),
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid,
                    invalid);

                Decoder CryptographicAES = Select((22, 2), IsZero,
                    Sparse(12, 0x1F, invalid,
                        (0b00100, Instr(Mnemonic.aese, q1,Vr(0, 5, BBBB), Vr(5,5,BBBB))),
                        (0b00101, Instr(Mnemonic.aesd, q1, Vr(0,5, BBBB), Vr(5,5,BBBB))),
                        (0b00110, Instr(Mnemonic.aesmc, q1, Vr(0, 5, BBBB), Vr(5, 5, BBBB))),
                        (0b00111, Instr(Mnemonic.aesimc, q1, Vr(0,5, BBBB), Vr(5,5,BBBB)))),
                    invalid);

                Decoder Cryptographic2regSHA = Select((22, 2), IsZero,
                    Sparse(12, 0x1F, invalid,
                        (0b00000, Instr(Mnemonic.sha1h, S_0, S_5)),
                        (0b00001, Instr(Mnemonic.sha1su1, Vr(0, 5, SSSS), Vr(5, 5, SSSS))),
                        (0b00010, Instr(Mnemonic.sha256su0, Vr(0, 5, SSSS), Vr(5, 5, SSSS)))),
                    invalid);

                Decoder Cryptographic3regSHA = Select((22, 2), IsZero,
                    Sparse(12, 0x7, invalid,
                        (0b000, Instr(Mnemonic.sha1c, q1, Q(0,5), S_5, Vr(16,5, SSSS))),
                        (0b001, Instr(Mnemonic.sha1p, x(""))),
                        (0b010, Instr(Mnemonic.sha1m, x(""))),
                        (0b011, Instr(Mnemonic.sha1su0, x(""))),
                        (0b100, Instr(Mnemonic.sha256h, x(""))),
                        (0b101, Instr(Mnemonic.sha256h2, x(""))),
                        (0b110, Instr(Mnemonic.sha256su1, q1, Vr(0, 5, SSSS), Vr(5, 5, SSSS), Vr(16, 5, SSSS)))),
                    invalid);

                DataProcessingScalarFpAdvancedSimd = Mask(28, 4, "DataProcessingScalarFpAdvancedSimd",
                    Mask(23, 2, // op0 = 0000
                        Mask(19, 4,        // op0=0000 op1=00 op
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0000"),
                            Mask(10, 2,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0001 op3=xxxxxxx11")),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0101"),
                            Mask(10, 2,      // op0=0000 op1=00 op2=0110 op3=xxxxxxx??
                                AdvancedSimd3Different,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0110 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0110 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0110 op3=xxxxxxx11")),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1000"), 
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1011"),
                            Mask(10, 2, 
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1100 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Mask(17, 2, // op0=0000 op1=00 op2=1100 op3=??xxxxx10
                                    AdvancedSIMD2RegMisc,
                                    invalid,    // op0=0000 op1=00 op2=1100 op3=01xxxxx10
                                    invalid,    // op0=0000 op1=00 op2=1100 op3=10xxxxx10
                                    invalid),   // op0=0000 op1=00 op2=1100 op3=11xxxxx10
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=00 op2=1111")),
                        Sparse(19, 0b1111,        // op0=0000 op1=01 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01"),
                            (0b0101, Mask(10, 2,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0101 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0101 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=0101 op3=xxxxxxx11"))),
                            (0b1111, Mask(10, 2,
                                AdvancedSimd3Different,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1111 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1111 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=01 op2=1111 op3=xxxxxxx11")))),
                        Mask(19, 4,        // op0=0000 op1=10 op2
                            Mask(10, 2,      // op0=0000 op1=10 op2=0000 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0000 op3=xxxxxxx00"),
                                AdvancedSimdModifiedImm,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0000 op3=xxxxxxx10"),
                                AdvancedSimdModifiedImm),
                            Mask(10, 2,      // op0=0000 op1=10 op2=0000 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0001 op3=xxxxxxx00"),
                                AdvancedSimdShiftByImm,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0001 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0001 op3=xxxxxxx11")),
                            Mask(10, 1,         // op0=0000 op1=10 op2=0010 op3
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0010 op3=xxxxxxxx0"),
                                AdvancedSimdShiftByImm),               // op0=0000 op1=10 op2=0010 op3=xxxxxxxx1
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=10 op2=1111")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=0000 op1=11")),
                    Mask(23, 2, //op0 = 1 op1
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
                        FloatingPointDataProcessing3src),
                    Mask(23, 2, // op0=2,
                        Mask(19, 4,    // op0=2 op1=00
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0010"),
                            Mask(10, 2, 
                                Mask(15, 1, // op0=2 op1=00 op2=0100 op3=xxx?xxx00
                                    AdvancedSimdExtract,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0011 op3=xxx1xxx00")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0011 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0011 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0011 op3=xxxxxxx11")),
                            Mask(10, 2,  // op0=2 op1=00 op2=0100 op3=xxxxxxx??
                                AdvancedSimd3Different,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0100 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0100 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=00 op2=1111")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=01"),
                        Mask(19, 4, //op0=2 op1=10"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0000"),
                            Mask(10, 1, // op=2 op1=10 op2=0001 op3=xxxxxxxx?
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0001 op3=xxxxxxxx0"),
                                AdvancedSimdShiftByImm),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=10 op2=1111")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=2 op1=11")),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=3"),

                    Mask(23, 2, //op0 = 4 op1
                        Mask(19, 4,    // op0=4 op1=00 op2
                            Mask(10, 2,  // op0=4 op1=00 op2=0000 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxxxxxx00"),
                                Mask(15, 1, // op0=4 op1=0b00 op2=0b0000 op3=xxx?xxx01"
                                    AdvancedSimdCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxxxxxx10"),
                                Mask(15, 1, // op0=4 op1=0b00 op2=0b0000 op3=xxx?xxx11"
                                    AdvancedSimdCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0000 op3=xxx1xxx11"))),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0011"),
                            Mask(10, 2, 
                                AdvancedSimd3Different,
                                AdvancedSimd3Same,
                                Mask(17, 2,  // op0=4 op1=0b00 op2=0b0100 op3=??xxxxx10
                                    AdvancedSimd2RegMisc,
                                    invalid,
                                    invalid,
                                    invalid),
                                AdvancedSimd3Same),
                            Mask(10, 2,  // op0=4 op1=0b00 op2=0b0101 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=xxxxxxx01"),
                                Mask(17, 2, // op0=4 op1=0b00 op2=0b0101 op3=??xxxxx10
                                    CryptographicAES,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=01xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=10xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=11xxxxxx10")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0101 op3=xxxxxxx11")),
                            Mask(10, 2, // op0=4 op1=00 op2=0110
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0110 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0110 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1011"),
                            Mask(10, 2, // op0=4 op1=0b00 op2=0b1100 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1100 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1100 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1111")),
                        Mask(19, 4, // op0=4 op1=0b01 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0011"),
                            Mask(10, 2,   // op0=4 op1=0b01 op2=0b0100 op3
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Mask(17, 2, // op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx10
                                    AdvancedSimd2RegMisc,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=01xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=10xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=11xxxxx10")),
                                AdvancedSimd3Same),
                            Mask(10, 2, // op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0101 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Mask(10, 2,   // op0=4 op1=0b01 op2=0b0110 op3
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Mask(17, 2, // op0=4 op1=0b01 op2=0b0110 op3=xxxxxxx10
                                    AdvancedSimdAcrossLanes,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=01xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=10xxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110 op3=11xxxxx10")),
                                AdvancedSimd3Same),
                            Mask(10, 2,      // op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0111 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1011"),
                            Mask(10, 2,      // op0=4 op1=0b01 op2=0b11001 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1100 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1100 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b1111")),
                        Select((19, 4), IsZero, // op0=4 op1=0b10 op2
                            Mask(10, 1, // op0=4 op1=0b10 op2=0000 op3=xxxxxxxx?
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b10 op2=0b0000"),
                                AdvancedSimdModifiedImm),
                            Mask(10, 1, // op0=4 op1=0b10 op2=0000 op3=xxxxxxxx?
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b10 op2=0b0001"),
                                AdvancedSimdShiftByImm)),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b11")),
                    Mask(23, 2, "op0=5", // op0=5 op1
                        Sparse(19, 0b1111, // op0=5 op1=0b00 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=???"),
                            (0b0000, Mask(10, 2,     // op0=5 op1=0b00 op2=0000")),
                                Mask(15, 1,             // op0=5 op1=0b00 op2=0000 op3=xxxxxxx00"),
                                    Cryptographic3regSHA,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxx1xxx00")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0000 op3=xxxxxxx11"))),
                            (0b0010, Mask(10, 2,     // op0=5 op1=0b00 op2=0100 op3
                                Mask(15, 1,             // op0=5 op1=0b00 op2=0010 op3=xxx?xxx00"),
                                    Cryptographic3regSHA,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxx1xxx00")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxxxxxx01"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0010 op3=xxxxxxx11"))),
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
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=0101 op3=xxxxxxx11")))),
                        Sparse(19, 0b1111,  // op0=5 op1=01 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b01 op2=????"),
                            (0b0101, FloatingPointDecoders)),
                        Sparse("  op0=5 op1=0b10 ", 19, 0b1111,  // op0=5 op1=10 op2
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b10"),
                            (0b0000, Mask(10, 2, // op0=5 op1=0b10 op2=0000 op3=xxxxxxx??
                                AdvancedSimdScalar_x_IdxElem,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b11 op2=0000 op3=xxxxxxx01"),
                                AdvancedSimdScalar_x_IdxElem,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b11 op2=0000 op3=xxxxxxx11")))),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b11")),

                    Mask(23, 2, // DataProcessingScalarFpAdvancedSimd - op0=6
                        Mask(19, 4,            // op0=6 op1=00 op2
                            Mask(10, 2,          // op0=6 op1=00 op2=0000 op3=xxxxxxx??
                                Mask(15, 1,         // op0=6 op1=00 op2=0000 op3=xxx?xxx00
                                    AdvancedSimdTableLookup,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxx1xxx00")),
                                Mask(15, 1,     // op0=6 op1=00 op2=0000 op3=xxx?xxx01
                                    AdvancedSimdCopy,   // op0=6 op1=00 op2=0000 op3=xxx0xxx01
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxx1xxx01")),
                                Mask(15, 1,     // op0=6 op1=00 op2=0000 op3=xxx?xxx10
                                    AdvancedSimdExtract,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxx1xxx10")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0000 op3=xxxxxxx11")),
                            Mask(10, 2,      // op0=6 op1=00 op2=0001 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001 op3=xxxxxxx00"),
                                Mask(15, 1,     // op0=6 op1=00 op2=0001 op3=xxx?xxx01
                                    AdvancedSimdCopy,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001 op3=xxx1xxx01")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001 op3=xxxxxxx10"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001 op3=xxxxxxx11")),
                            Mask(10, 2,      // op0=6 op1=00 op2=0010 op3=xxxxxxx??
                                Mask(15, 1,     // op0=6 op1=00 op2=0010 op3=xxx?xxx00
                                    AdvancedSimdTableLookup,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxx1xxx00")),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxxxxxx00"),
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0010 op3=xxxxxxx00")),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0011"),
                            Mask(10, 2,      // op0=6 op1=00 op2=0100 op3=xxxxxxx??
                                AdvancedSimd3Different,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=xxxxxxx01"),
                                Mask(17, 2,  // op0=6 op1=00 op2=0100 op3=??xxxxxx10
                                    AdvancedSimd2RegMisc,
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=01xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=10xxxxxx10"),
                                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0100 op3=11xxxxxx10")),
                                AdvancedSimd3Same),
                            Mask(10, 2,      // op0=6 op1=00 op2=0101 op3=xxxxxxx??
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0101 op3=000000000"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0101 op3=000000010"),
                                AdvancedSimd3Same),
                            Mask(10, 2,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110 op3=xxxxxxx00"),
                                AdvancedSimd3Same,
                                Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110 op3=xxxxxxx10"),
                                AdvancedSimd3Same),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0111"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1000"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1001"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1010"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1011"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1100"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1101"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1110"),
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=1111")),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=01"),
                        Mask(10, 1, 
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=10 op2=0110 op3=xxxxxxxx0"),
                            Select((19, 4), IsZero,
                                AdvancedSimdModifiedImm,
                                AdvancedSimdShiftByImm)),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=6 op1=3")),
                    Mask(23, 2, // op0=7 op1
                        Mask(19, 4,    // op0=7 op1=00
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=00 op2=0000"),
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
                        Sparse(19, 0b1111,
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
                        Sparse(19, 0b1111,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=10")),
                        Sparse(19, 0b1111,
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=7 op1=11"),
                            (0b1010, Mask(10, 1,
                                AdvancedSimdScalar_x_IdxElem,
                                invalid)))),

                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=8"),
                    Mask(23, 2, // op0=9 op1
                        Mask(19, 4,    // op0=9 op1=00 op2
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
                            Nyi("DataProcessingScalarFpAdvancedSimd - op0=9 op1=00 op2=1111")),
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
                        FloatingPointDataProcessing3src),   // op0=9 op1=11
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=A"),
                    Nyi("DataProcessingScalarFpAdvancedSimd - op0=B"),

                    Mask(23, 2,              // op0=C op1
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=C op1=00"),
                        Nyi("DataProcessingScalarFpAdvancedSimd - op0=C op1=01"),
                        invalid,
                        invalid),
                    invalid,
                    invalid,
                    invalid);
            }

            rootDecoder = Mask(25, 4,
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
