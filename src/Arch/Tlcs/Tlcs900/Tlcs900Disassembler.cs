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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnemonic = Reko.Arch.Tlcs.Tlcs900.Tlcs900Mnemonic;

namespace Reko.Arch.Tlcs.Tlcs900
{
    using Decoder = Decoder<Tlcs900Disassembler, Mnemonic, Tlcs900Instruction>;
#pragma warning disable IDE1006

    /// <summary>
    /// Disassembler for the 32-bit Toshiba TLCS-900 processor.
    /// </summary>
    public partial class Tlcs900Disassembler : DisassemblerBase<Tlcs900Instruction, Mnemonic>
    {
        private readonly Tlcs900Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType? opSize;

        public Tlcs900Disassembler(Tlcs900Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Tlcs900Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
                return null;
            this.opSize = null;
            this.ops.Clear();
            var instr = rootDecoders[b].Decode(b, this);
            if (instr is null)
            {
                instr = CreateInvalidInstruction();
            }
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - instr.Address);
            return instr;
        }

        public override Tlcs900Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Tlcs900Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override Tlcs900Instruction CreateInvalidInstruction()
        {
            return new Tlcs900Instruction {
                Address = this.addr,
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        public override Tlcs900Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("RiscV_dasm", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators 

        private static bool BW(uint b, Tlcs900Disassembler dasm)
        {
            // Opsize must be set to byte or word16.
            return dasm.opSize!.Size == 1 || dasm.opSize.Size == 2;
        }

        private static bool clr(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Clear();
            return true;
        }

        private static bool C(uint b, Tlcs900Disassembler dasm) {
            // condition code
            var cc = (CondCode) (b & 0xF);
            if (cc != CondCode.T)
                dasm.ops.Add(ConditionOperand.Create(cc));
            return true;
        }

        // A register
        private static bool A(uint b, Tlcs900Disassembler dasm) {

            dasm.ops.Add(Tlcs900Registers.a);
            return true;
        }

        // Immediate encoded in low 3 bits
        private static Mutator<Tlcs900Disassembler> i3(PrimitiveType? size)
        {
            return (b, dasm) => 
            {
                var s = dasm.Size(size);
                var c = Constant.Create(s, imm3Const[b & 7]);
                dasm.opSize = s;
                dasm.ops.Add(c);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> i3b = i3(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> i3z = i3(null);

        // immediate
        private static bool Ib(uint u, Tlcs900Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(Constant.Byte(b));
            return true;
        }

        private static bool Iw(uint u, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w))
                return false;
            dasm.ops.Add(Constant.Word16(w));
            return true;
        }

        private static bool Ix(uint u, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out u))
                return false;
            dasm.ops.Add(Constant.Word32(u));
            return true;
        }

        private static bool Iz(uint u, Tlcs900Disassembler dasm)
        {
            if (dasm.opSize is null)
                return false;
            switch (dasm.opSize.Size)
            {
            case 1: return Ib(u, dasm);
            case 2: return Iw(u, dasm);
            case 4: return Ix(u, dasm);
            }
            return false;
        }


        // Relative jump
        private static bool jb(uint b, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte o8))
                return false;
            dasm.ops.Add(dasm.rdr.Address + (sbyte) o8);
            return true;
        }

        private static bool jw(uint b, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short o16))
                return false;
            dasm.ops.Add(dasm.rdr.Address + o16);
            return true;
        }

        // Absolute jump

        private static bool Jw(uint b, Tlcs900Disassembler dasm)
        {
            var dst = dasm.AbsoluteDestination(2);
            if (dst is null)
                return false;
            dasm.ops.Add(dst);
            return true;
        }

        private static bool Jl(uint b, Tlcs900Disassembler dasm)
        {
            var dst = dasm.AbsoluteDestination(3);
            if (dst is null)
                return false;
            dasm.ops.Add(dst);
            return true;
        }

        private static Mutator<Tlcs900Disassembler> R(PrimitiveType? size)
        {
            return (b, dasm) => {
                dasm.ops.Add(dasm.Reg(size, (int)b & 0x7));
                dasm.SetSize(size);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Rb = R(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Rw = R(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Rx = R(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> Rz = R(null);

        // status/flag register
        private static bool Sb(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Add(Tlcs900Registers.f);
            return true;
        }

        private static bool Sw(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Add(Tlcs900Registers.sr);
            return true;
        }

        #endregion

        private static Mutator<Tlcs900Disassembler> Post(PrimitiveType? size)
        {
            return (b, dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out byte r))
                    return false;
                var incCode = r & 3;
                if (incCode >= incDecSize.Length)
                    return false;
                dasm.ops.Add(MemoryOperand.PostIncrement(dasm.Size(size), incDecSize[r & 3], dasm.Reg(PrimitiveType.Word32, (r >> 2) & 0x3F)));
                dasm.SetSize(size);
                return true;
            };
        }

        // Predecrement
        private static Mutator<Tlcs900Disassembler> Pre(PrimitiveType? size)
        {
            return (b, dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out byte r))
                    return false;
                var incCode = r & 3;
                if (incCode >= incDecSize.Length)
                    return false;
                dasm.ops.Add(MemoryOperand.PreDecrement(dasm.Size(size), incDecSize[r & 3], dasm.Reg(PrimitiveType.Word32, (r >> 2) & 0x3F)));
                dasm.SetSize(size);
                return true;
            };
        }

        // Immediate encoded in low 3 bits, with 8 encoded as 0
        private static Mutator<Tlcs900Disassembler> q3(PrimitiveType? size)
        {
            return (b, dasm) => {
                var c = Constant.Create(dasm.Size(size), imm3Const8[b & 7]);
                dasm.SetSize(size);
                dasm.ops.Add(c);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> qz = q3(null);


        //           private static bool I(uint b, Tlcs900Disassembler dasm) {
        //// immediate
        //               dasm.ops.Add(Immediate(fmt[1]));
        //               return op;
        //           }

        //           private static bool jb(uint b, Tlcs900Disassembler dasm) {
        //// Relative jump
        //               switch (fmt[1])
        //               {
        //               case 'b':
        //                   if (!rdr.TryReadByte(out o8))
        //                       return  null;
        //                   else
        //                       return AddressOperand.Create(rdr.Address + (sbyte)o8);
        //               case 'w':
        //                   short o16;
        //                   if (!rdr.TryReadLeInt16(out o16))
        //                       return null;
        //                   else
        //                       return AddressOperand.Create(rdr.Address + o16);
        //               }
        //               return null;
        //}

        private static Mutator<Tlcs900Disassembler> r(PrimitiveType size)
        {
            //$TODO: 'r' may encode other registers. manual is dense
            return (b, dasm) =>
            {
                dasm.ops.Add(dasm.Reg(size, (int) b & 0x7));
                dasm.SetSize(size);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> rb = r(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> rw = r(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> rx = r(PrimitiveType.Word32);

        private static Mutator<Tlcs900Disassembler> M(PrimitiveType? size)
        {
            return (b, dasm) =>
            {
                // Register indirect
                dasm.ops.Add(MemoryOperand.Indirect(dasm.Size(size), dasm.Reg(PrimitiveType.Word32, (int)b & 7)));
                dasm.SetSize(size);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Mb = M(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Mw = M(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Mx = M(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> M_ = M(null);

        // indexed (8-bit offset)
        private static Mutator<Tlcs900Disassembler> N(PrimitiveType? size)
        {
            return (b, dasm) => {
                if (!dasm.rdr.TryReadByte(out byte o8))
                    return false;
                dasm.ops.Add(MemoryOperand.Indexed8(dasm.Size(size), dasm.Reg(PrimitiveType.Word32, (int)b & 7), (sbyte) o8));
                dasm.SetSize(size);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Nb = N(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Nw = N(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Nx = N(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> N_ = N(null);

        // various mem formats
        private static Mutator<Tlcs900Disassembler> m(PrimitiveType? size)
        {
            return (b, dasm) =>
            {
                if (!dasm.rdr.TryReadByte(out byte m))
                    return false;
                switch (m & 3)
                {
                case 0: // Register indirect
                    dasm.ops.Add(MemoryOperand.Indirect(dasm.Size(size), dasm.Reg(PrimitiveType.Word32, (m >> 2) & 0x3F)));
                    dasm.SetSize(size);
                    return true;
                case 1: // indexed (16-bit offset)
                    if (!dasm.rdr.TryReadLeInt16(out short o16))
                        return false;
                    dasm.ops.Add(MemoryOperand.Indexed16(dasm.Size(size), dasm.Reg(PrimitiveType.Word32, (m >> 2) & 0x3F), o16));
                    dasm.SetSize(size);
                    return true;
                case 3:
                    if (m != 3 && m != 7)
                        return false;
                    if (!dasm.rdr.TryReadByte(out byte rBase))
                        return false;
                    if (!dasm.rdr.TryReadByte(out byte rIdx))
                        return false;
                    var regBase = dasm.Reg(PrimitiveType.Word32, rBase);
                    var regIdx = dasm.Reg(m == 3 ? PrimitiveType.Byte : PrimitiveType.Word16, rIdx);
                    dasm.ops.Add(MemoryOperand.RegisterIndexed(dasm.Size(size), regBase, regIdx));
                    dasm.SetSize(size);
                    return true;
                default:
                    return false;
                }
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> mb = m(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> mw = m(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> mx = m(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> m_ = m(null);


        // Override the size of opSrc
        private static Mutator<Tlcs900Disassembler> Z(PrimitiveType size)
        {
            return (b, dasm) =>
            {
                dasm.ops[0].DataType = size;
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Zb = Z(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Zw = Z(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Zx = Z(PrimitiveType.Word32);


        private static Mutator<Tlcs900Disassembler> O(PrimitiveType? size) {
            return (b, dasm) => {
                var abs = dasm.Absolute(1, size);
                if (abs is null)
                    return false;
                dasm.ops.Add(abs);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Ob = O(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Ow = O(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Ox = O(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> O_ = O(null);

        private static Mutator<Tlcs900Disassembler> P(PrimitiveType? size)
        {
            return (b, dasm) => 
            {
                var abs = dasm.Absolute(2, size);
                if (abs is null)
                    return false;
                dasm.ops.Add(abs);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Pb = P(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Pw = P(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Px = P(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> P_ = P(null);

        private static Mutator<Tlcs900Disassembler> Q(PrimitiveType? size)
        {
            return (b, dasm) => 
            {
                var abs = dasm.Absolute(3, size);
                if (abs is null)
                    return false;
                dasm.ops.Add(abs);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Qb = Q(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Qw = Q(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Qx = Q(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> Q_ = Q(null);

        private RegisterStorage Reg(PrimitiveType? size, int regNum)
        {
            int r = regNum & 7;
            if (size is null)
            {
                size = this.opSize!;
            }
            switch (size.Size)
            {
            case 4: return Tlcs900Registers.regs[r];
            case 2: return Tlcs900Registers.regs[8 + r];
            case 1: return Tlcs900Registers.regs[16 + r];
            default: throw new FormatException();
            }
        }

        private PrimitiveType Size(PrimitiveType? size)
        {
            return size ?? this.opSize!;
        }

        private void SetSize(PrimitiveType? size)
        {
            if (size is not null)
                this.opSize = size;
        }

        private MachineOperand? Absolute(int addrBytes, PrimitiveType? size)
        {
            uint uAddr = 0;
            int sh = 0;
            while (--addrBytes >= 0)
            {
                if (!rdr.TryReadByte(out byte b))
                    return null;
                uAddr |= (uint)b << sh;
                sh += 8;
            }
            SetSize(size);
            return MemoryOperand.Absolute(size!, uAddr);
        }

        private MachineOperand? AbsoluteDestination(int addrBytes)
        {
            uint uAddr = 0;
            int sh = 0;
            while (--addrBytes >= 0)
            {
                if (!rdr.TryReadByte(out byte b))
                    return null;
                uAddr |= (uint)b << sh;
                sh += 8;
            }
            return Address.Ptr32(uAddr);
        }

        private RegisterStorage? ExtraRegister(byte b)
        {
            switch (b)
            {
            case 0x31: return Tlcs900Registers.w;
            case 0xE6: return Tlcs900Registers.bc;
            }
            return null;
        }

        private class RegDecoder : Decoder
        {
            private readonly Mutator<Tlcs900Disassembler> mutator;

            public RegDecoder(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(uint bPrev, Tlcs900Disassembler dasm)
            {
                if (!mutator(bPrev, dasm) || !dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                return regDecoders[b].Decode(b, dasm);
            }
        }

        private class ExtraRegDecoder :Decoder
        {
            private readonly PrimitiveType width;

            public ExtraRegDecoder(PrimitiveType width)
            {
                this.width = width;
            }

            public override Tlcs900Instruction Decode(uint bPrev, Tlcs900Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                dasm.opSize = width;
                var op = dasm.ExtraRegister(b);
                if (op is null)
                    return dasm.CreateInvalidInstruction();
                if (!dasm.rdr.TryReadByte(out b))
                    return dasm.CreateInvalidInstruction();
                dasm.ops.Add(op);
                return regDecoders[b].Decode(b, dasm);
            }
        }

        private class MemDecoder : Decoder
        {
            private readonly Mutator<Tlcs900Disassembler> mutator;

            public MemDecoder(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(uint bPrev, Tlcs900Disassembler dasm)
            {
                if (!mutator(bPrev, dasm) || !dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                return memDecoders[b].Decode(b, dasm);
            }
        }

        private class DstDecoder : Decoder
        {
            private readonly Mutator<Tlcs900Disassembler> mutator;

            public DstDecoder(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(uint bPrev, Tlcs900Disassembler dasm)
            {
                if (!mutator(bPrev, dasm) || !dasm.rdr.TryReadByte(out byte b))
                    return dasm.CreateInvalidInstruction();
                var instr = dstDecoders[b].Decode(b, dasm);
                if (instr.Operands.Length >= 2 && !(instr.Operands[0] is RegisterStorage))
                {
                    instr.Operands[0].DataType = instr.Operands[1].DataType;
                }
                if (instr.Operands.Length >= 2 && instr.Operands[1].DataType is null)
                {
                    //$HACK to get conditional calls/jumps to work
                    instr.Operands[1].DataType = PrimitiveType.Word32;
                }
                return instr;
            }
        }

        private class SecondDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly Mutator<Tlcs900Disassembler>? mutator;

            public SecondDecoder(Mnemonic mnemonic, Mutator<Tlcs900Disassembler>? mutator = null)
            {
                this.mnemonic = mnemonic;
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(uint bPrev, Tlcs900Disassembler dasm)
            {
                if (this.mutator is null)
                {
                    return new Tlcs900Instruction
                    {
                        Mnemonic = this.mnemonic,
                        Address = dasm.addr,
                        Operands = dasm.ops.ToArray()
                    };
                }

                if (!mutator(bPrev, dasm))
                    return dasm.CreateInvalidInstruction();
                dasm.ops.Reverse();
                return new Tlcs900Instruction
                {
                    Mnemonic = this.mnemonic,
                    Address = dasm.addr,
                    Operands = dasm.ops.ToArray()
                };
            }
        }

        // Inverts the order of the decoded operands
        private class InvDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly Mutator<Tlcs900Disassembler>[] mutators;

            public InvDecoder(Mnemonic mnemonic, params Mutator<Tlcs900Disassembler>[]mutators)
            {
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override Tlcs900Instruction Decode(uint b, Tlcs900Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                dasm.ops.Reverse();
                return new Tlcs900Instruction
                {
                    Mnemonic = this.mnemonic,
                    Address = dasm.addr,
                    Operands = dasm.ops.ToArray()
                };
            }
        }

        private class LdirDecoder : Decoder
        {
            public override Tlcs900Instruction Decode(uint b, Tlcs900Disassembler dasm)
            {
                return new Tlcs900Instruction
                {
                    Mnemonic = dasm.opSize!.Size == 2 ? Mnemonic.ldirw : Mnemonic.ldir,
                    InstructionClass = InstrClass.Linear,
                    Operands = Array.Empty<MachineOperand>(),
                    Address = dasm.addr
                };
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Tlcs900Disassembler>[] mutators)
        {
            return new InstrDecoder<Tlcs900Disassembler, Mnemonic, Tlcs900Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Tlcs900Disassembler>[] mutators)
        {
            return new InstrDecoder<Tlcs900Disassembler, Mnemonic, Tlcs900Instruction>(iclass, mnemonic, mutators);
        }

        private static readonly int[] imm3Const = new int[8]
        {
            0, 1, 2, 3, 4, 5, 6, 7,
        };

        private static readonly int[] imm3Const8 = new int[8]
        {
            8, 1, 2, 3, 4, 5, 6, 7,
        };

        private static readonly int[] incDecSize = new int[3]
        {
            1, 2, 4
        };

        private static readonly Decoder invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

        private static readonly Decoder[] rootDecoders = {
            // 00
            Instr(Mnemonic.nop, InstrClass.Padding|InstrClass.Zero),
            invalid,
            Instr(Mnemonic.push, Sw),
            Instr(Mnemonic.pop, Sw),

            invalid,
            Instr(Mnemonic.halt),
            Instr(Mnemonic.ei, Ib),
            Instr(Mnemonic.reti, InstrClass.Transfer|InstrClass.Return),

            invalid,
            Instr(Mnemonic.push, Ib),
            invalid,
            Instr(Mnemonic.push, Iw),

            Instr(Mnemonic.incf),
            Instr(Mnemonic.decf),
            Instr(Mnemonic.ret, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.retd, InstrClass.Transfer|InstrClass.Return, Iw),
            // 10
            Instr(Mnemonic.rcf),
            Instr(Mnemonic.scf),
            Instr(Mnemonic.ccf),
            Instr(Mnemonic.zcf),

            Instr(Mnemonic.push, A),
            Instr(Mnemonic.pop, A),
            invalid,
            Instr(Mnemonic.ldf, Ib),

            invalid,
            invalid,
            Instr(Mnemonic.jp, Jw),
            Instr(Mnemonic.jp, Jl),

            Instr(Mnemonic.call, Jw),
            Instr(Mnemonic.call, Jl),
            Instr(Mnemonic.calr, jw),
            invalid,
            // 20
            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),

            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),
            Instr(Mnemonic.ld, Rb,Ib),

            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
                                        
            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
            Instr(Mnemonic.push, Rw),
            // 30
            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),

            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),
            Instr(Mnemonic.ld, Rw,Iw),

            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),

            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),
            Instr(Mnemonic.push, Rx),
            // 40
            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),

            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),
            Instr(Mnemonic.ld, Rx,Ix),

            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),

            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),
            Instr(Mnemonic.pop, Rw),
            // 50
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),

            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),
            Instr(Mnemonic.pop, Rx),
            // 60
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),

            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),

            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),

            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            Instr(Mnemonic.jr, C,jb),
            // 70
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),

            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),

            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),

            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            Instr(Mnemonic.jr, C,jw),
            // 80
            new MemDecoder(Mb),
            new MemDecoder(Mb),
            new MemDecoder(Mb),
            new MemDecoder(Mb),

            new MemDecoder(Mb),
            new MemDecoder(Mb),
            new MemDecoder(Mb),
            new MemDecoder(Mb),

            new MemDecoder(Nb),
            new MemDecoder(Nb),
            new MemDecoder(Nb),
            new MemDecoder(Nb),

            new MemDecoder(Nb),
            new MemDecoder(Nb),
            new MemDecoder(Nb),
            new MemDecoder(Nb),
            // 90
            new MemDecoder(Mw),
            new MemDecoder(Mw),
            new MemDecoder(Mw),
            new MemDecoder(Mw),

            new MemDecoder(Mw),
            new MemDecoder(Mw),
            new MemDecoder(Mw),
            new MemDecoder(Mw),

            new MemDecoder(Nw),
            new MemDecoder(Nw),
            new MemDecoder(Nw),
            new MemDecoder(Nw),

            new MemDecoder(Nw),
            new MemDecoder(Nw),
            new MemDecoder(Nw),
            new MemDecoder(Nw),
            // A0
            new MemDecoder(Mx),
            new MemDecoder(Mx),
            new MemDecoder(Mx),
            new MemDecoder(Mx),

            new MemDecoder(Mx),
            new MemDecoder(Mx),
            new MemDecoder(Mx),
            new MemDecoder(Mx),

            new MemDecoder(Nx),
            new MemDecoder(Nx),
            new MemDecoder(Nx),
            new MemDecoder(Nx),

            new MemDecoder(Nx),
            new MemDecoder(Nx),
            new MemDecoder(Nx),
            new MemDecoder(Nx),
            // B0
            new DstDecoder(M_),
            new DstDecoder(M_),
            new DstDecoder(M_),
            new DstDecoder(M_),

            new DstDecoder(M_),
            new DstDecoder(M_),
            new DstDecoder(M_),
            new DstDecoder(M_),

            new DstDecoder(N_),
            new DstDecoder(N_),
            new DstDecoder(N_),
            new DstDecoder(N_),

            new DstDecoder(N_),
            new DstDecoder(N_),
            new DstDecoder(N_),
            new DstDecoder(N_),
            // C0
            new MemDecoder(Ob),
            new MemDecoder(Pb),
            new MemDecoder(Qb),
            new MemDecoder(mb),

            new MemDecoder(Pre(PrimitiveType.Byte)),
            new MemDecoder(Post(PrimitiveType.Byte)),
            invalid,
            new ExtraRegDecoder(PrimitiveType.Byte),

            new RegDecoder(rb),
            new RegDecoder(rb),
            new RegDecoder(rb),
            new RegDecoder(rb),

            new RegDecoder(rb),
            new RegDecoder(rb),
            new RegDecoder(rb),
            new RegDecoder(rb),
            // D0
            new MemDecoder(Ow),
            new MemDecoder(Pw),
            new MemDecoder(Qw),
            new MemDecoder(mw),

            new MemDecoder(Pre(PrimitiveType.Word16)),
            new MemDecoder(Post(PrimitiveType.Word16)),
            invalid,
            new ExtraRegDecoder(PrimitiveType.Word16),

            new RegDecoder(rw),
            new RegDecoder(rw),
            new RegDecoder(rw),
            new RegDecoder(rw),

            new RegDecoder(rw),
            new RegDecoder(rw),
            new RegDecoder(rw),
            new RegDecoder(rw),
            // E0
            new MemDecoder(Ox),
            new MemDecoder(Px),
            new MemDecoder(Qx),
            new MemDecoder(mx),

            new MemDecoder(Pre(PrimitiveType.Word32)),
            new MemDecoder(Post(PrimitiveType.Word32)),
            invalid,
            new ExtraRegDecoder(PrimitiveType.Word32),

            new RegDecoder(rx),
            new RegDecoder(rx),
            new RegDecoder(rx),
            new RegDecoder(rx),

            new RegDecoder(rx),
            new RegDecoder(rx),
            new RegDecoder(rx),
            new RegDecoder(rx),

            // F0
            new DstDecoder(O_),
            new DstDecoder(P_),
            new DstDecoder(Q_),
            new DstDecoder(m_),

            new DstDecoder(Pre(null)),
            new DstDecoder(Post(null)),
            invalid,
            invalid,

            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),

            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),
            Instr(Mnemonic.swi, i3b),
        };
    }
}
