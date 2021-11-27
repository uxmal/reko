#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Renesas.Rx
{
    using Decoder = Decoder<RxDisassembler, Mnemonic, RxInstruction>;

    public class RxDisassembler : DisassemblerBase<RxInstruction, Mnemonic>
    {
        private static Decoder rootDecoder;
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);

        private readonly RxArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType? dataWidth;

        public RxDisassembler(RxArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = [];
            this.dataWidth = default;
        }

        public override RxInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte uInstr))
                return null;
            this.ops.Clear();
            this.dataWidth = null;
            var instr = rootDecoder.Decode(uInstr, this);
            this.ops.Clear();
            if (uInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override RxInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new RxInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                DataType = this.dataWidth,
                Operands = ops.ToArray()
            };
        }

        public override RxInstruction CreateInvalidInstruction()
        {
            return new RxInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.Invalid };
        }

        public override RxInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Rl78Dis", this.addr, this.rdr, message);
            return new RxInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.nyi };
        }

        #region Mutators

        private static Mutator<RxDisassembler> GpRegister(int bitoffset)
        {
            var field = new Bitfield(bitoffset, 4);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> R0 = GpRegister(0);
        private static readonly Mutator<RxDisassembler> R4 = GpRegister(4);

        /// <summary>
        /// Unsigned immediate.
        /// </summary>
        private static Mutator<RxDisassembler> UImm(int bitpos, int bitlength, PrimitiveType dt)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var op = Constant.Create(dt, field.Read(u));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> UI4_4_32 = UImm(4, 4, PrimitiveType.Word32);


        private static Mutator<RxDisassembler> Simm(int bitpos)
        {
            var field = new Bitfield(bitpos, 2);
            return (u, d) =>
            {
                var sizeCode = field.Read(u);
                int imm;
                switch (sizeCode)
                {
                case 0b01:
                    if (!d.rdr.TryReadByte(out byte b8))
                        return false;
                    imm = (sbyte) b8;
                    break;
                case 0b10:
                    if (!d.rdr.TryReadInt16(out short w16))
                        return false;
                    imm = w16;
                    break;
                case 0b11:
                    if (!d.TryReadInt24(out imm))
                        return false;
                    break;
                default:
                    if (!d.rdr.TryReadInt32(out int w32))
                        return false;
                    imm = w32;
                    break;
                }
                var op = Constant.Create(PrimitiveType.Word32, imm);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<RxDisassembler> Simm10 = Simm(10);

        private bool TryReadInt24(out int imm)
        {
                imm = 0;
            if (arch.Endianness == EndianServices.Little)
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (sbyte)b2 << 16 | b1 << 8 | b0;
            }
            else
            {
                if (!rdr.TryReadByte(out byte b0))
                    return false;
                if (!rdr.TryReadByte(out byte b1))
                    return false;
                if (!rdr.TryReadByte(out byte b2))
                    return false;
                imm = (sbyte) b0 << 16 | b1 << 8 | b2;
            }
            return true;
        }

        private static bool Mr4(uint uInstr, RxDisassembler dasm)
        {
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, 0);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mr4_d8(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, (sbyte)b);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mr4_d16(uint uInstr, RxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out short s))
                return false;
            var iReg = bf4_4.Read(uInstr);
            var reg = Registers.GpRegisters[iReg];
            Debug.Assert(dasm.dataWidth is not null);
            var mem = new MemoryOperand(dasm.dataWidth, reg, s);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool L(uint uInstr, RxDisassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word32;
            return true;
        }

        #endregion

        #region Decoders

        private class Instr16Decoder : InstrDecoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            public Instr16Decoder(InstrClass iclass, Mnemonic mnemonic, Mutator<RxDisassembler> [] mutators)
                : base(iclass, mnemonic, mutators)
            {
            }

            public override RxInstruction Decode(uint wInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b2))
                    return dasm.CreateInvalidInstruction();
                return base.Decode((wInstr << 8) | b2, dasm);
            }
        }

        private class Next16Decoder : Decoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            private readonly Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder;

            public Next16Decoder(Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override RxInstruction Decode(uint uInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort b2))
                    return dasm.CreateInvalidInstruction();
                var wInstr = (uInstr << 16) | b2;
                return nextDecoder.Decode(wInstr, dasm);
            }
        }

        private class Next24Decoder : Decoder<RxDisassembler, Mnemonic, RxInstruction>
        {
            private readonly Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder;

            public Next24Decoder(Decoder<RxDisassembler, Mnemonic, RxInstruction> nextDecoder)
            {
                this.nextDecoder = nextDecoder;
            }

            public override RxInstruction Decode(uint uInstr, RxDisassembler dasm)
            {
                if (!dasm.rdr.TryReadBeUInt16(out ushort w))
                    return dasm.CreateInvalidInstruction();
                var wInstr = (uInstr << 16) | w;
                if (!dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                wInstr = (wInstr << 8) | b;
                return nextDecoder.Decode(wInstr, dasm);
            }
        }


        #endregion

        private static InstrDecoder<RxDisassembler, Mnemonic, RxInstruction> Instr(Mnemonic mnemonic, params Mutator<RxDisassembler>[] mutators)
            => new InstrDecoder<RxDisassembler, Mnemonic, RxInstruction>(InstrClass.Linear, mnemonic, mutators);

        private static InstrDecoder<RxDisassembler, Mnemonic, RxInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<RxDisassembler>[] mutators)
            => new InstrDecoder<RxDisassembler, Mnemonic, RxInstruction>(iclass, mnemonic, mutators);

        private static Instr16Decoder Instr16(Mnemonic mnemonic, params Mutator<RxDisassembler>[] mutators)
            => new Instr16Decoder(InstrClass.Linear, mnemonic, mutators);

        private static Instr16Decoder Instr16(Mnemonic mnemonic, InstrClass iclass, params Mutator<RxDisassembler>[] mutators)
            => new Instr16Decoder(iclass, mnemonic, mutators);


        private static Decoder Nyi(string message)
            => new NyiDecoder<RxDisassembler, Mnemonic, RxInstruction>(message);

        static RxDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var adc_Msrc_dst = Mask(16, 2, "  adc",
                Instr(Mnemonic.adc, L, Mr4, R0),
                Instr(Mnemonic.adc, L, Mr4_d8, R0),
                Instr(Mnemonic.adc, L, Mr4_d16, R0),
                invalid);

            var decode06 = new Next24Decoder(Sparse(18, 4, "  07", Nyi("06"),
                (0b1000, Sparse(8, 8, "  1000", Nyi("06 1000"),
                    (0x02, adc_Msrc_dst)
                    ))));

            var decodeFC = new Next16Decoder(Sparse(8, 8, "  FC", Nyi("FC"),
                (0x0B, Instr(Mnemonic.adc, R4, R0)),
                (0x0F, Instr(Mnemonic.abs, R4, R0))
                ));

            var decodeFD = new Next16Decoder(Sparse(12, 4, "  FD", Nyi("FD"),
                (0b0111, Sparse(4, 6, "  111", Nyi("FD 111"),
                    (0b000010, Instr(Mnemonic.adc, Simm10, R0))))));

            rootDecoder = Sparse(0, 8, "Rx", Nyi("Rx"),
                (0x06, decode06),
                (0x62, Instr16(Mnemonic.add, UI4_4_32, R0)),
                (0x7E, Instr16(Mnemonic.abs, R0)),
                (0xFC, decodeFC),
                (0xFD, decodeFD));
        }

        /*0
         FSQRT Floating-point square root
FTOU Floating point to integer conversion
UTOF Integer to floating-point conversion
Data transfer
instructions
MOVCO Storing with LI flag clear
MOVLI Loading with LI flag set
DSP instructions EMACA Extend multiply-accumulate to the accumulator
EMSBA Extended multiply-subtract to the accumulator
EMULA Extended multiply to the accumulator
MACLH Multiply-Accumulate the lower-order word and higher-order word
MSBHI Multiply-Subtract the higher-order word
MSBLH Multiply-Subtract the lower-order word and higher-order word
MSBLO Multiply-Subtract the lower-order word
MULLH Multiply the lower-order word and higher-order word
MVFACGU Move the guard longword from the accumulator
MVFACLO Move the lower-order longword from the accumulator
MVTACGU Move the guard longword to the accumulator
RACL Round the accumulator longword
RDACL Round the accumulator longword
RDACW Round the accumulator word
F*/

    }
}
