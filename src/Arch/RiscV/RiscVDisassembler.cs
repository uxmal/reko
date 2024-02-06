#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

#pragma warning disable IDE1006

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

namespace Reko.Arch.RiscV
{
    using Decoder = Reko.Core.Machine.Decoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;
    using MaskDecoder = Reko.Core.Machine.MaskDecoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;

    public partial class RiscVDisassembler : DisassemblerBase<RiscVInstruction, Mnemonic>
    {
        private static readonly Bitfield bf_r1 = new Bitfield(15, 5);
        private static readonly Bitfield bf_r2 = new Bitfield(20, 5);
        private static readonly Bitfield bf20_12 = new Bitfield(20, 12);
        private static readonly Bitfield bf_funct3 = new Bitfield(12, 3);

        private static readonly int[] compressedRegs = new int[8]
        {
            8, 9, 10, 11, 12, 13, 14, 15,
        };
        private static readonly Bitfield[] bfBranch = Bf((31,1), (7,1), (25,6), (8, 4));
        private static readonly (uint sign, uint biasedExp, uint mantissa)[] encodedFpImms = new (uint,uint,uint)[32]
        {
            (1, 0b01111111, 0b000), // \(-1.0\)
            (0, 0b00000001, 0b000), // Minimum positive normal
            (0, 0b01101111, 0b000), // \(1.0 \times 2^{-16}\)
            (0, 0b01110000, 0b000), // \(1.0 \times 2^{-15}\)
            (0, 0b01110111, 0b000), // \(1.0 \times 2^{-8}\)
            (0, 0b01111000, 0b000), // \(1.0 \times 2^{-7}\)
            (0, 0b01111011, 0b000), // 0.0625 (\(2^{-4}\))
            (0, 0b01111100, 0b000), // 0.125 (\(2^{-3}\))

            (0, 0b01111101, 0b000), // 0.25
            (0, 0b01111101, 0b010), // 0.3125
            (0, 0b01111101, 0b100), // 0.375
            (0, 0b01111101, 0b110), // 0.4375
            (0, 0b01111110, 0b000), // 0.5
            (0, 0b01111110, 0b010), // 0.625
            (0, 0b01111110, 0b100), // 0.75
            (0, 0b01111110, 0b110), // 0.875

            (0, 0b01111111, 0b000), // 1.0
            (0, 0b01111111, 0b010), // 1.25
            (0, 0b01111111, 0b100), // 1.5
            (0, 0b01111111, 0b110), // 1.75
            (0, 0b10000000, 0b000), // 2.0
            (0, 0b10000000, 0b010), // 2.5
            (0, 0b10000000, 0b100), // 3
            (0, 0b10000001, 0b000), // 4

            (0, 0b10000010, 0b000), // 8
            (0, 0b10000011, 0b000), // 16
            (0, 0b10000110, 0b000), // 128 (\(2^7\))
            (0, 0b10000111, 0b000), // 256 (\(2^8\))
            (0, 0b10001110, 0b000), // \(2^{15}\)
            (0, 0b10001111, 0b000), // \(2^{16}\)
            (0, 0b11111111, 0b000), // \(+\infty\)
            (0, 0b11111111, 0b100), // Canonical NaN
        };

        private static readonly float[] encodedFpImms_s = new float[32]
        {
            -1.0F,
            float.Epsilon, //$BUG: Minimum positive normal
            1.0F / 65536.0F,
            1.0F / 32768.0F,
            1.0F / 256.0F,
            1.0F / 128.0F,
            0.0625F,
            0.125F,

            0.25F,
            0.3125F,
            0.375F,
            0.4375F,
            0.5F,
            0.625F,
            0.75F,
            0.875F,

            1.0F,
            1.25F,
            1.5F,
            1.75F,
            2.0F,
            2.5F,
            3.0F,
            4.0F,

            8.0F,
            16.0F,
            128.0F,
            256.0F,
            32769.0F,
            65536.0F,
            float.PositiveInfinity,
            float.NaN
        };

        private static readonly double[] encodedFpImms_d = new double[32]
        {
            -1.0,
            Double.Epsilon, //$BUG: Minimum positive normal
            1.0 / 65536.0,
            1.0 / 32768.0,
            1.0 / 256.0,
            1.0 / 128.0,
            0.0625,
            0.125,

            0.25,
            0.3125,
            0.375,
            0.4375,
            0.5,
            0.625,
            0.75,
            0.875,

            1.0,
            1.25,
            1.5,
            1.75,
            2.0,
            2.5,
            3.0,
            4.0,

            8.0,
            16.0,
            128.0,
            256.0,
            32769.0,
            65536.0,
            Double.PositiveInfinity,
            Double.NaN
        };

        private readonly RiscVArchitecture arch;
        private readonly Decoder[] decoders;
        private readonly EndianImageReader rdr;
        private Address addrInstr;
        private State state;

        public RiscVDisassembler(RiscVArchitecture arch, Decoder[] decoders, EndianImageReader rdr)
        {
            this.arch = arch;
            this.decoders = decoders;
            this.rdr = rdr;
            this.addrInstr = rdr.Address;
            this.state = new State();
        }

        public override RiscVInstruction? DisassembleInstruction()
        {
            if (!rdr.TryReadLeUInt16(out ushort hInstr))
            {
                return null;
            }
            state.ops.Clear();
            state.instr = new RiscVInstruction();
            var instr = decoders[hInstr & 0x3].Decode(hInstr, this);
            instr.Address = addrInstr;
            instr.Length = (int) (rdr.Address - addrInstr);
            instr.InstructionClass |= hInstr == 0 ? InstrClass.Zero : 0;
            this.addrInstr = rdr.Address;
            return instr;
        }

        public override RiscVInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var i = state.instr;
            i.InstructionClass = state.iclass != InstrClass.None
                ? state.iclass
                : iclass;
            i.Mnemonic = mnemonic;
            i.Address = this.addrInstr;
            i.Operands = this.state.ops.ToArray();
            state.instr = new RiscVInstruction();
            return i;
        }

        public override RiscVInstruction CreateInvalidInstruction()
        {
            return new RiscVInstruction
            {
                Address = addrInstr,
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        /// <summary>
        /// Register encoded at bit position 15
        /// </summary>
        private static bool r1(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetRegister(wInstr, 15);
            dasm.state.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Register encoded at bit position 20
        /// </summary>
        private static bool r2(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetRegister(wInstr, 20);
            dasm.state.ops.Add(op);
            return true;
        }

        private static bool rd(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetRegister(wInstr, 7);
            dasm.state.ops.Add(op);
            return true;
        }

        private static bool i(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetImmediate(wInstr, 20, 's');
            dasm.state.ops.Add(op);
            return true;
        }

        private static bool B(uint wInstr, RiscVDisassembler dasm)
        { 
            var op = dasm.GetBranchTarget(wInstr);
            dasm.state.ops.Add(op);
            return true;
        }

        private static Mutator<RiscVDisassembler> Ff(int bitPos)
        {
            return (u, d) =>
            {
                var op = d.GetFpuRegister(u, bitPos);
                d.state.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RiscVDisassembler> F1 = Ff(15);
        private static readonly Mutator<RiscVDisassembler> F2 = Ff(20);
        private static readonly Mutator<RiscVDisassembler> F3 = Ff(27);
        private static readonly Mutator<RiscVDisassembler> Fd = Ff(7);

        /// <summary>
        /// Acquire/release bits
        /// </summary>
        private static bool aq_rl(uint uInstr, RiscVDisassembler dasm)
        {
            dasm.state.instr.Acquire = Bits.IsBitSet(uInstr, 26);
            dasm.state.instr.Release = Bits.IsBitSet(uInstr, 25);
            return true;
        }

        private static bool Csr20(uint uInstr, RiscVDisassembler dasm)
        {
            var iCsr = bf20_12.Read(uInstr);
            if (dasm.arch.Csrs.TryGetValue(iCsr, out var csr))
            {
                dasm.state.ops.Add(csr);
            }
            else
            {
                dasm.state.ops.Add(ImmediateOperand.UInt32(iCsr));
            }
            return true;
        }

        private static bool J(uint u, RiscVDisassembler d)
        {
            var offset = Bitfield.ReadSignedFields(j_bitfields, u) << 1;
            d.state.ops.Add(AddressOperand.Create(d.addrInstr + offset));
            return true;
        }

        private static bool Jc(uint u, RiscVDisassembler d)
        {
            var offset = Bitfield.ReadSignedFields(c_j_bitfields, u) << 1;
            d.state.ops.Add(AddressOperand.Create(d.addrInstr + offset));
            return true;
        }

        private static bool Iu(uint wInstr, RiscVDisassembler dasm)
        {
            uint u = wInstr >> 12;
            var op = ImmediateOperand.Word32(u);
            dasm.state.ops.Add(op);
            return true;
        }

        private static bool fpImm_s(uint wInstr, RiscVDisassembler dasm)
        {
            var code = bf_r1.Read(wInstr);
            var fp = encodedFpImms_s[code];
            var imm = ImmediateOperand.Create(Constant.Real32(fp));
            dasm.state.ops.Add(imm);
            return true;
        }

        private static bool fpImm_d(uint wInstr, RiscVDisassembler dasm)
        {
            var code = bf_r1.Read(wInstr);
            var fp = encodedFpImms_s[code];
            var imm = ImmediateOperand.Create(Constant.Real32(fp));
            dasm.state.ops.Add(imm);
            return true;
        }

        private static readonly Mutator<RiscVDisassembler> Ss = MemSignedOffset(PrimitiveType.Word32, 15, (25,7),(7,5));

        /// <summary>
        /// Predecessor or successor field in a <code>fence</code>
        /// instruction.
        /// </summary>
        private static Mutator<RiscVDisassembler> PredSucc(int bitpos)
        {
            var field = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var ps = field.Read(u);
                d.state.ops.Add(d.arch.PredSuccRegs[ps]);
                return true;
            };
        }
        private static readonly Mutator<RiscVDisassembler> ps_20 = PredSucc(20);
        private static readonly Mutator<RiscVDisassembler> ps_24 = PredSucc(24);

        // signed offset used in loads
        private static readonly Mutator<RiscVDisassembler> Ls = MemSignedOffset(PrimitiveType.Word32, 15, (20,12));

        private static bool z(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetShiftAmount(wInstr, 5);
            dasm.state.ops.Add(op);
            return true;
        }

        private static bool Z(uint wInstr, RiscVDisassembler dasm)
        {
            var op = dasm.GetShiftAmount(wInstr, 6);
            dasm.state.ops.Add(op);
            return true;
        }

        private RegisterStorage GetRegister(uint wInstr, int bitPos)
        {
            var reg = arch.GetRegister((int)(wInstr >> bitPos) & 0x1F)!;
            return reg;
        }

        private RegisterStorage GetFpuRegister(uint wInstr, int bitPos)
        {
            var reg = arch.GetRegister(32 + ((int)(wInstr >> bitPos) & 0x1F))!;
            return reg;
        }

        private ImmediateOperand GetImmediate(uint wInstr, int bitPos, char sign)
        {
            Constant c;
            if (sign == 's')
            {
                int s = ((int)wInstr) >> bitPos;
                c = Constant.Create(arch.NaturalSignedInteger, s);
            }
            else
            {
                uint u = wInstr >> bitPos;
                c = Constant.Create(arch.WordWidth, u);
            }
            return new ImmediateOperand(c);
        }

        private ImmediateOperand GetShiftAmount(uint wInstr, int length)
        {
            return ImmediateOperand.UInt32(extract32(wInstr, 20, length));
        }

        private static bool bit(uint wInstr, int bitNo)
        {
            return (wInstr & (1u << bitNo)) != 0;
        }

        private static uint extract32(uint wInstr, int start, int length)
        {
            uint n = (wInstr >> start) & (~0U >> (32 - length));
            return n;
        }

        private AddressOperand GetBranchTarget(uint wInstr)
        {
            long offset = Bitfield.ReadSignedFields(bfBranch, wInstr) << 1;
            return AddressOperand.Create(addrInstr + offset);
        }

        private ImmediateOperand GetSImmediate(uint wInstr)
        {
            var offset = (int)
                   (extract32(wInstr, 7, 5)
                 | (extract32(wInstr, 25, 7) << 5));
            return ImmediateOperand.Int32(offset);
        }

        public override RiscVInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("RiscV_dasm", this.addrInstr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        internal class State
        {
            public RiscVInstruction instr = new RiscVInstruction();
            public List<MachineOperand> ops = new List<MachineOperand>();
            public InstrClass iclass;
        }

        #region Decoders

        public class NyiDecoder : Decoder
        {
            private readonly string message; 

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override RiscVInstruction Decode(uint hInstr, RiscVDisassembler dasm)
            {
                return dasm.NotYetImplemented(message);
            }
        }

        public class W32Decoder : Decoder
        {
            private readonly MaskDecoder<RiscVDisassembler, Mnemonic, RiscVInstruction> subDecoders;

            public W32Decoder(MaskDecoder subDecoders)
            {
                this.subDecoders = subDecoders;
            }

            public override RiscVInstruction Decode(uint hInstr, RiscVDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort hiword))
                {
                    return dasm.CreateInvalidInstruction();
                }
                uint wInstr = (uint)hiword << 16;
                wInstr |= hInstr;
                return subDecoders.Decode(wInstr, dasm);
            }
        }

        public class ShiftDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public ShiftDecoder(params Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override RiscVInstruction Decode(uint wInstr, RiscVDisassembler dasm)
            {
                var decoder = decoders[bit(wInstr, 30) ? 1 : 0];
                return decoder.Decode(wInstr, dasm);
            }
        }

        /// <summary>
        /// Handle instructions that are encoded differently depending on the word size
        /// of the CPU architecture.
        /// </summary>
        internal class WordSizeDecoder : Decoder
        {
            private readonly Decoder rv32;
            private readonly Decoder rv64;
            private readonly Decoder rv128;

            public WordSizeDecoder(
                Decoder rv32,
                Decoder rv64,
                Decoder rv128)
            {
                this.rv32 = rv32;
                this.rv64 = rv64;
                this.rv128 = rv128;
            }

            public override RiscVInstruction Decode(uint wInstr, RiscVDisassembler dasm)
            {
                switch (dasm.arch.WordWidth.Size)
                {
                case 4: return rv32.Decode(wInstr, dasm);
                case 8: return rv64.Decode(wInstr, dasm);
                case 16: return rv128.Decode(wInstr, dasm);
                }
                throw new NotSupportedException($"{dasm.arch.WordWidth.Size}-bit Risc-V instructions not supported.");
            }
        }

        #endregion

        #region Mutators

        // The Risc-V manual specifies 5 immediate formats

        // I-immediate
        private static readonly Bitfield iI = new Bitfield(20, 12);
        // S-immediate
        private static readonly Bitfield[] iS = Bf((25, 7), (7, 5));
        // B-immediate
        private static readonly Bitfield[] iB = Bf((31, 1), (7, 1), (25, 6), (8, 4));
        // U-immediate
        private static readonly Bitfield iU = new Bitfield(12, 20);
        // J-immediate
        private static readonly Bitfield[] iJ = Bf((31, 1), (12, 8), (20, 1), (21, 10));

        // Integer register
        private static Mutator<RiscVDisassembler> R(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                var reg = d.arch.GetRegister(iReg)!;
                d.state.ops.Add(reg);
                return true;
            };
        }

        // use SP / StackRegister register
        private static bool Rsp(uint instr, RiscVDisassembler dasm)
        {
            dasm.state.ops.Add(dasm.arch.StackRegister);
            return true;
        }

        /// <summary>
        /// If the LR register is used, mark the instruction as 'return'.
        /// </summary>
        private static Mutator<RiscVDisassembler> useLr(int bitpos)
        {
            var regMask = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                if (iReg == d.arch.LinkRegister.Number)
                    d.state.iclass = InstrClass.Transfer | InstrClass.Return;
                return true;
            };

        }

        /// <summary>
        /// Integer register encoded at <paramref name="bitPos"/>, but 0 is invalid.
        /// </summary>
        private static Mutator<RiscVDisassembler> R_nz(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                if (iReg == 0)
                    return false;
                var reg = d.arch.GetRegister(iReg)!;
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Registers specified at fixed positions.

        private static readonly Mutator<RiscVDisassembler> Rd = R(7);
        private static readonly Mutator<RiscVDisassembler> R1 = R(15);
        private static readonly Mutator<RiscVDisassembler> R2 = R(20);

        // Floating point register
        private static Mutator<RiscVDisassembler> F(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 5);
            return (u, d) =>
            {
                var iReg = (int) regMask.Read(u);
                var reg = d.arch.FpRegs[iReg];
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Compressed format register (r')
        private static Mutator<RiscVDisassembler> Rc(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 3);
            return (u, d) =>
            {
                var iReg = compressedRegs[regMask.Read(u)];
                var reg = d.arch.GetRegister(iReg)!;
                d.state.ops.Add(reg);
                return true;
            };
        }

        // Compressed format floating point register (fr')
        private static Mutator<RiscVDisassembler> Fc(int bitPos)
        {
            var regMask = new Bitfield(bitPos, 3);
            return (u, d) =>
            {
                var iReg = compressedRegs[regMask.Read(u)];
                var reg = d.arch.FpRegs[iReg];
                d.state.ops.Add(reg);
                return true;
            };
        }

        /// <summary>
        /// Rounding mode
        /// </summary>
        private static Mutator<RiscVDisassembler> rm(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var rm = d.arch.RoundingModes[field.Read(u)];
                if (rm is null)
                    return false;
                d.state.ops.Add(rm);
                return true;
            };
        }
        private static readonly Mutator<RiscVDisassembler> rm12 = rm(12);

        private static Mutator<RiscVDisassembler> ImmSigned(int bitPos, int length)
        {
            var field = new Bitfield(bitPos, length);
            return (u, d) =>
            {
                var imm = Constant.Create(d.arch.NaturalSignedInteger, field.ReadSigned(u));
                d.state.ops.Add(new ImmediateOperand(imm));
                return true;
            };
        }

        private static Mutator<RiscVDisassembler> ImmSigned(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var n = Bitfield.ReadSignedFields(fields, u);
                var imm = Constant.Create(d.arch.NaturalSignedInteger, n);
                d.state.ops.Add(new ImmediateOperand(imm));
                return true;
            };
        }

        // Unsigned immediate
        private static Mutator<RiscVDisassembler> Imm(int bitPos1, int length1)
        {
            var mask = new Bitfield(bitPos1, length1);
            return (u, d) =>
            {
                var imm = Constant.Create(d.arch.WordWidth, mask.Read(u));
                d.state.ops.Add(new ImmediateOperand(imm));
                return true;
            };
        }

        private static Mutator<RiscVDisassembler> Imm(params (int pos, int len) [] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return Imm(masks);
        }

        private static Mutator<RiscVDisassembler> Imm(Bitfield[] masks)
        {
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, uImm)));
                return true;
            };
        }

        // Immediate with a shift
        private static Mutator<RiscVDisassembler> ImmSh(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return ImmSh(sh, masks);
        }

        private static Mutator<RiscVDisassembler> ImmSh(int sh, Bitfield[] masks)
        {
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u) << sh;
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.WordWidth, uImm)));
                return true;
            };
        }

        private static Mutator<RiscVDisassembler> ImmB(params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return ImmB(masks);
        }

        private static Mutator<RiscVDisassembler> ImmB(Bitfield[] masks)
        {
            return (u, d) =>
            {
                var uImm = Bitfield.ReadFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(PrimitiveType.Byte, uImm)));
                return true;
            };
        }

        private static Mutator<RiscVDisassembler> ImmS(params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return ImmS(masks);
        }

        private static Mutator<RiscVDisassembler> ImmS(Bitfield[] masks)
        {
            return (u, d) =>
            {
                var sImm = Bitfield.ReadSignedFields(masks, u);
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.NaturalSignedInteger, sImm)));
                return true;
            };
        }

        // Signed immediate with a shift
        private static Mutator<RiscVDisassembler> ImmShS(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return ImmShS(sh, masks);
        }

        private static Mutator<RiscVDisassembler> ImmShS(int sh, Bitfield[] masks)
        {
            return (u, d) =>
            {
                var uImm = Bitfield.ReadSignedFields(masks, u) << sh;
                d.state.ops.Add(new ImmediateOperand(
                    Constant.Create(d.arch.NaturalSignedInteger, uImm)));
                return true;
            };
        }

        // Common immediate fields
        private static readonly Mutator<RiscVDisassembler> I20s = ImmS((20, 12));
        private static readonly Mutator<RiscVDisassembler> I12 = Imm(12, 20);

        // PC-relative displacement with shift.
        private static Mutator<RiscVDisassembler> PcRel(int sh, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return PcRel(sh, masks);
        }

        private static Mutator<RiscVDisassembler> PcRel(int sh, Bitfield[] masks)
        {
            return (u, d) =>
            {
                var sImm = Bitfield.ReadSignedFields(masks, u) << sh;
                var addr = d.addrInstr + sImm;
                d.state.ops.Add(AddressOperand.Create(addr));
                return true;
            };
        }

        // Memory operand format, where offset is not scaled
        private static Mutator<RiscVDisassembler> Mem(PrimitiveType dt, int regOffset, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return Mem(dt, regOffset, masks);
        }

        private static Mutator<RiscVDisassembler> Mem(PrimitiveType dt, int regOffset, Bitfield[] masks)
        {
            var baseRegMask = new Bitfield(regOffset, 5);

            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadFields(masks, u);
                var iBase = (int) baseRegMask.Read(u);

                d.state.ops.Add(new MemoryOperand(
                    dt,
                    d.arch.GetRegister(iBase)!,
                    uOffset));
                return true;
            };
        }


        // Memory operand format, where the _signed_ offset is not scaled
        private static Mutator<RiscVDisassembler> MemSignedOffset(PrimitiveType dt, int regOffset, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return MemSignedOffset(dt, regOffset, masks);
        }

        private static Mutator<RiscVDisassembler> MemSignedOffset(PrimitiveType dt, int regOffset, Bitfield[] masks)
        {
            var baseRegMask = new Bitfield(regOffset, 5);

            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadSignedFields(masks, u);
                var iBase = (int) baseRegMask.Read(u);

                d.state.ops.Add(new MemoryOperand(
                    dt,
                    d.arch.GetRegister(iBase)!,
                    uOffset));
                return true;
            };
        }


        // Memory operand format, where offset is scaled by the register size
        private static Mutator<RiscVDisassembler> MemScaledOffset(PrimitiveType dt, int regOffset, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return MemScaledOffset(dt, regOffset, masks);
        }

        private static Mutator<RiscVDisassembler> MemScaledOffset(PrimitiveType dt, int regOffset, Bitfield[] masks)
        {
            var baseRegField = new Bitfield(regOffset, 5);

            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadFields(masks, u) * dt.Size;
                var iBase = (int)baseRegField.Read(u);

                d.state.ops.Add(new MemoryOperand(
                    dt,
                    d.arch.GetRegister(iBase)!,
                    uOffset));
                return true;
            };
        }

        // Memory operand format used for compressed instructions. Offset is unsigned.
        private static Mutator<RiscVDisassembler> Memc(PrimitiveType dt, int regOffset, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return Memc(dt, regOffset, masks);
        }

        private static Mutator<RiscVDisassembler> Memc(PrimitiveType dt, int regOffset, Bitfield[] masks)
        {
            var baseRegMask = new Bitfield(regOffset, 3);
            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadFields(masks, u) * dt.Size;
                var iBase = compressedRegs[baseRegMask.Read(u)];

                d.state.ops.Add(new MemoryOperand(
                    dt,
                    d.arch.GetRegister(iBase)!,
                    uOffset));
                return true;
            };
        }

        private static Mutator<RiscVDisassembler> MemcSpRel(PrimitiveType dt, params (int pos, int len)[] fields)
        {
            var masks = fields
                .Select(field => new Bitfield(field.pos, field.len))
                .ToArray();

            return MemcSpRel(dt, masks);
        }

        private static Mutator<RiscVDisassembler> MemcSpRel(PrimitiveType dt, Bitfield[] masks)
        {
            return (u, d) =>
            {
                var uOffset = (int) Bitfield.ReadFields(masks, u) * dt.Size;

                d.state.ops.Add(new MemoryOperand(
                    dt,
                    d.arch.StackRegister,
                    uOffset));
                return true;
            };
        }

        private static readonly Bitfield[] j_bitfields = new[]
        {
            new Bitfield(31, 1),
            new Bitfield(12, 8),
            new Bitfield(20, 1),
            new Bitfield(21, 10)
        };

        private static readonly Bitfield[] c_j_bitfields =
            Bf((12, 1), (8, 1), (9, 2), (6, 1), (7, 1), (2,1), (11,1), (3,3));
            //Bf((11, 1), (4, 1), (8, 2), (10, 1), (6, 1), (7, 1), (1, 3), (5, 1));

        private static bool Eq0(uint u) => u == 0;

        private static bool Ne0(uint u) => u != 0;

        private static bool R1EqR2(uint u)
        {
            var ireg1 = bf_r1.Read(u);
            var ireg2 = bf_r2.Read(u);
            return ireg1 == ireg2;
        }

        #endregion

    }
}
