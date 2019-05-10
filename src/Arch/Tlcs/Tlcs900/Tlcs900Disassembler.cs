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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Opcode;

namespace Reko.Arch.Tlcs.Tlcs900
{
    /// <summary>
    /// Disassembler for the 32-bit Toshiba TLCS-900 processor.
    /// </summary>
    public partial class Tlcs900Disassembler : DisassemblerBase<Tlcs900Instruction>
    {
        private readonly Tlcs900Architecture arch;
        private readonly ImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType opSize;

        public Tlcs900Disassembler(Tlcs900Architecture arch, ImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Tlcs900Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
                return null;
            this.opSize = null;
            this.ops.Clear();
            if (addr.ToLinear() == 0x200116) //$DEBUG
                addr.ToLinear();
            var instr = opRecs[b].Decode(b, this);
            if (instr == null)
            {
                instr = Invalid();
            }
            instr.Length = (int) (rdr.Address - instr.Address);
            return instr;
        }

        private Tlcs900Instruction Invalid()
        {
            return new Tlcs900Instruction {
                Address = this.addr,
                InstructionClass = InstrClass.Invalid,
                Opcode = Opcode.invalid };

        }
        #region Mutators 

        private static bool clr(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Clear();
            return true;
        }

        private static bool C(uint b, Tlcs900Disassembler dasm) {
            // condition code
            var cc = (CondCode) (b & 0xF);
            if (cc != CondCode.T)
                dasm.ops.Add(new ConditionOperand(cc));
            return true;
        }

        // A register
        private static bool A(uint b, Tlcs900Disassembler dasm) {

            dasm.ops.Add(new RegisterOperand(Tlcs900Registers.a));
            return true;
        }

        // Immediate encoded in low 3 bits
        private static Mutator<Tlcs900Disassembler> i3(PrimitiveType size)
        {
            return (b, dasm) => 
            {
                var s = dasm.Size(size);
                var c = Constant.Create(s, imm3Const[b & 7]);
                dasm.opSize = s;
                dasm.ops.Add(new ImmediateOperand(c));
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> i3b = i3(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> i3z = i3(null);

        // immediate
        private static bool Ib(uint u, Tlcs900Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(b));
            return true;
        }

        private static bool Iw(uint u, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(w));
            return true;
        }

        private static bool Ix(uint u, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out u))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32(u));
            return true;
        }

        private static bool Iz(uint u, Tlcs900Disassembler dasm)
        {
            if (dasm.opSize == null)
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
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + (sbyte) o8));
            return true;
        }

        private static bool jw(uint b, Tlcs900Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short o16))
                return false;
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + o16));
            return true;
        }

        // Absolute jump

        private static bool Jw(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Add(dasm.AbsoluteDestination(2));
            return true;
        }

        private static bool Jl(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Add(dasm.AbsoluteDestination(3));
            return true;
        }

        private static Mutator<Tlcs900Disassembler> R(PrimitiveType size)
        {
            return (b, dasm) => {
                dasm.ops.Add(new RegisterOperand(dasm.Reg(size, (int)b & 0x7)));
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
            dasm.ops.Add(new RegisterOperand(Tlcs900Registers.f));
            return true;
        }

        private static bool Sw(uint b, Tlcs900Disassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Tlcs900Registers.sr));
            return true;
        }

        #endregion

        private static Mutator<Tlcs900Disassembler> Post(PrimitiveType size)
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
        private static Mutator<Tlcs900Disassembler> Pre(PrimitiveType size)
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
        private static Mutator<Tlcs900Disassembler> q3(PrimitiveType size)
        {
            return (b, dasm) => {
                var c = Constant.Create(dasm.Size(size), imm3Const8[b & 7]);
                dasm.SetSize(size);
                dasm.ops.Add(new ImmediateOperand(c));
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
                dasm.ops.Add(new RegisterOperand(dasm.Reg(size, (int) b & 0x7)));
                dasm.SetSize(size);
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> rb = r(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> rw = r(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> rx = r(PrimitiveType.Word32);

        private static Mutator<Tlcs900Disassembler> M(PrimitiveType size)
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
        private static Mutator<Tlcs900Disassembler> N(PrimitiveType size)
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
        private static Mutator<Tlcs900Disassembler> m(PrimitiveType size)
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
                dasm.ops[0].Width = size;
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Zb = Z(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Zw = Z(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Zx = Z(PrimitiveType.Word32);


        private static Mutator<Tlcs900Disassembler> O(PrimitiveType size) {
            return (b, dasm) => {
                dasm.ops.Add(dasm.Absolute(1, size));
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Ob = O(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Ow = O(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Ox = O(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> O_ = O(null);

        private static Mutator<Tlcs900Disassembler> P(PrimitiveType size)
        {
            return (b, dasm) => {
                dasm.ops.Add(dasm.Absolute(2, size));
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Pb = P(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Pw = P(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Px = P(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> P_ = P(null);

        private static Mutator<Tlcs900Disassembler> Q(PrimitiveType size)
        {
            return (b, dasm) => {
                dasm.ops.Add(dasm.Absolute(3, size));
                return true;
            };
        }
        private static readonly Mutator<Tlcs900Disassembler> Qb = Q(PrimitiveType.Byte);
        private static readonly Mutator<Tlcs900Disassembler> Qw = Q(PrimitiveType.Word16);
        private static readonly Mutator<Tlcs900Disassembler> Qx = Q(PrimitiveType.Word32);
        private static readonly Mutator<Tlcs900Disassembler> Q_ = Q(null);

        private RegisterStorage Reg(PrimitiveType size, int regNum)
        {
            int r = regNum & 7;
            if (size == null)
            {
                size = this.opSize;
            }
            switch (size.Size)
            {
            case 4: return Tlcs900Registers.regs[r];
            case 2: return Tlcs900Registers.regs[8 + r];
            case 1: return Tlcs900Registers.regs[16 + r];
            default: throw new FormatException();
            }
        }

        private PrimitiveType Size(PrimitiveType size)
        {
            return size ?? this.opSize;
        }

        private void SetSize(PrimitiveType size)
        {
            if (size != null)
                this.opSize = size;
        }

        private MachineOperand Absolute(int addrBytes, PrimitiveType size)
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
            return MemoryOperand.Absolute(size, uAddr);
        }

        private MachineOperand AbsoluteDestination(int addrBytes)
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
            return AddressOperand.Ptr32(uAddr);
        }

        private MachineOperand StatusRegister(char size)
        {
            switch (size)
            {
            case 'b': return new RegisterOperand( Tlcs900Registers.f);
            case 'w': return new RegisterOperand(Tlcs900Registers.sr);
            default: return null;
            }
        }

        private RegisterOperand ExtraRegister(byte b)
        {
            switch (b)
            {
            case 0x31: return new RegisterOperand(Tlcs900Registers.w);
            case 0xE6: return new RegisterOperand(Tlcs900Registers.bc);
            }
            return null;
        }


        public abstract class OpRecBase
        {
            public abstract Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm);
        }

        private class OpRec : OpRecBase
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly Mutator<Tlcs900Disassembler>[] mutators;

            public OpRec(Opcode opcode, InstrClass iclass, Mutator<Tlcs900Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return new Tlcs900Instruction
                        {
                            Opcode = Opcode.invalid,
                            InstructionClass = iclass,
                            Address = dasm.addr
                        };
                }
                var instr = new Tlcs900Instruction
                {
                    Opcode = opcode,
                    InstructionClass = iclass,
                    Address = dasm.addr,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
                };
                return instr;
            }
        }

        private class RegOpRec : OpRecBase
        {
            private readonly Mutator<Tlcs900Disassembler> mutator;

            public RegOpRec(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (!mutator(b, dasm) || !dasm.rdr.TryReadByte(out b))
                    return dasm.Invalid();
                return regOpRecs[b].Decode(b, dasm);
            }
        }

        private class ExtraRegOprec :OpRecBase
        {
            private readonly PrimitiveType width;

            public ExtraRegOprec(PrimitiveType width)
            {
                this.width = width;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out b))
                    return null;
                dasm.opSize = width;
                var op = dasm.ExtraRegister(b);
                if (op == null)
                    return dasm.Invalid();
                if (!dasm.rdr.TryReadByte(out b))
                    return dasm.Invalid();
                dasm.ops.Add(op);
                return regOpRecs[b].Decode(b, dasm);
            }
        }

        private class MemOpRec : OpRecBase
        {
            private Mutator<Tlcs900Disassembler> mutator;

            public MemOpRec(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (!mutator(b, dasm) || !dasm.rdr.TryReadByte(out b))
                    return dasm.Invalid();
                return memOpRecs[b].Decode(b, dasm);
            }
        }

        private class DstOpRec : OpRecBase
        {
            private Mutator<Tlcs900Disassembler> mutator;

            public DstOpRec(Mutator<Tlcs900Disassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (!mutator(b, dasm) || !dasm.rdr.TryReadByte(out b))
                    return dasm.Invalid();
                var instr = dstOpRecs[b].Decode(b, dasm);
                if (instr.op1 != null && instr.op2 != null)
                {
                    instr.op1.Width = instr.op2.Width;
                }
                if (instr.op2 != null && instr.op2.Width == null)
                {
                    //$HACK to get conditional calls/jumps to work
                    instr.op2.Width = PrimitiveType.Word32;
                }
                return instr;
            }
        }

        private class SecondOpRec : OpRecBase
        {
            private Opcode opcode;
            private Mutator<Tlcs900Disassembler> mutator;

            public SecondOpRec(Opcode opcode, Mutator<Tlcs900Disassembler> mutator = null)
            {
                this.opcode = opcode;
                this.mutator = mutator;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                if (this.mutator == null)
                {
                    return new Tlcs900Instruction
                    {
                        Opcode = this.opcode,
                        Address = dasm.addr,
                        op1 = dasm.ops[0]
                    };
                }

                if (!mutator(b, dasm))
                    return dasm.Invalid();
                return new Tlcs900Instruction
                {
                    Opcode = this.opcode,
                    Address = dasm.addr,
                    op1 = dasm.ops.Count > 1 ? dasm.ops[1] : dasm.ops[0],
                    op2 = dasm.ops.Count > 1 ? dasm.ops[0] : null,
                };
            }
        }

        // Inverts the order of the decoded operands
        private class InvOpRec : OpRecBase
        {
            private Opcode opcode;
            private Mutator<Tlcs900Disassembler>[] mutators;

            public InvOpRec(Opcode opcode, params Mutator<Tlcs900Disassembler>[]mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(b, dasm))
                        return dasm.Invalid();
                }
                bool swap = dasm.ops.Count == 2;
                return new Tlcs900Instruction
                {
                    Opcode = this.opcode,
                    Address = dasm.addr,
                    op1 = swap ? dasm.ops[1] : dasm.ops[0],
                    op2 = swap ? dasm.ops[0] : null
                };
            }
        }

        private class LdirOpRec : OpRecBase
        {
            public override Tlcs900Instruction Decode(byte b, Tlcs900Disassembler dasm)
            {
                return new Tlcs900Instruction
                {
                    Opcode = dasm.opSize.Size == 2 ? Opcode.ldirw : Opcode.ldir,
                    InstructionClass = InstrClass.Linear,
                    Address = dasm.addr
                };
            }
        }

        private static OpRec Instr(Opcode opcode, params Mutator<Tlcs900Disassembler>[] mutators)
        {
            return new OpRec(opcode, InstrClass.Linear, mutators);
        }

        private static OpRec Instr(Opcode opcode, InstrClass iclass, params Mutator<Tlcs900Disassembler>[] mutators)
        {
            return new OpRec(opcode, iclass, mutators);
        }

        private static int[] imm3Const = new int[8]
        {
            0, 1, 2, 3, 4, 5, 6, 7,
        };

        private static int[] imm3Const8 = new int[8]
        {
            8, 1, 2, 3, 4, 5, 6, 7,
        };

        private static int[] incDecSize = new int[3]
        {
            1, 2, 4
        };

        private static OpRecBase[] opRecs = {
            // 00
            Instr(Opcode.nop, InstrClass.Padding|InstrClass.Zero),
            Instr(Opcode.invalid),
            Instr(Opcode.push, Sw),
            Instr(Opcode.pop, Sw),

            Instr(Opcode.invalid),
            Instr(Opcode.halt),
            Instr(Opcode.ei, Ib),
            Instr(Opcode.reti),

            Instr(Opcode.invalid),
            Instr(Opcode.push, Ib),
            Instr(Opcode.invalid),
            Instr(Opcode.push, Iw),

            Instr(Opcode.incf),
            Instr(Opcode.decf),
            Instr(Opcode.ret),
            Instr(Opcode.retd, Iw),
            // 10
            Instr(Opcode.rcf),
            Instr(Opcode.scf),
            Instr(Opcode.ccf),
            Instr(Opcode.zcf),

            Instr(Opcode.push, A),
            Instr(Opcode.pop, A),
            Instr(Opcode.invalid),
            Instr(Opcode.ldf, Ib),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.jp, Jw),
            Instr(Opcode.jp, Jl),

            Instr(Opcode.call, Jw),
            Instr(Opcode.call, Jl),
            Instr(Opcode.calr, jw),
            Instr(Opcode.invalid),
            // 20
            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),

            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),
            Instr(Opcode.ld, Rb,Ib),

            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
                                        
            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
            Instr(Opcode.push, Rw),
            // 30
            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),

            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),
            Instr(Opcode.ld, Rw,Iw),

            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),

            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),
            Instr(Opcode.push, Rx),
            // 40
            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),

            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),
            Instr(Opcode.ld, Rx,Ix),

            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),

            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),
            Instr(Opcode.pop, Rw),
            // 50
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),

            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),
            Instr(Opcode.pop, Rx),
            // 60
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),

            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),

            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),

            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            Instr(Opcode.jr, C,jb),
            // 70
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),

            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),

            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),

            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            Instr(Opcode.jr, C,jw),
            // 80
            new MemOpRec(Mb),
            new MemOpRec(Mb),
            new MemOpRec(Mb),
            new MemOpRec(Mb),

            new MemOpRec(Mb),
            new MemOpRec(Mb),
            new MemOpRec(Mb),
            new MemOpRec(Mb),

            new MemOpRec(Nb),
            new MemOpRec(Nb),
            new MemOpRec(Nb),
            new MemOpRec(Nb),

            new MemOpRec(Nb),
            new MemOpRec(Nb),
            new MemOpRec(Nb),
            new MemOpRec(Nb),
            // 90
            new MemOpRec(Mw),
            new MemOpRec(Mw),
            new MemOpRec(Mw),
            new MemOpRec(Mw),

            new MemOpRec(Mw),
            new MemOpRec(Mw),
            new MemOpRec(Mw),
            new MemOpRec(Mw),

            new MemOpRec(Nw),
            new MemOpRec(Nw),
            new MemOpRec(Nw),
            new MemOpRec(Nw),

            new MemOpRec(Nw),
            new MemOpRec(Nw),
            new MemOpRec(Nw),
            new MemOpRec(Nw),
            // A0
            new MemOpRec(Mx),
            new MemOpRec(Mx),
            new MemOpRec(Mx),
            new MemOpRec(Mx),

            new MemOpRec(Mx),
            new MemOpRec(Mx),
            new MemOpRec(Mx),
            new MemOpRec(Mx),

            new MemOpRec(Nx),
            new MemOpRec(Nx),
            new MemOpRec(Nx),
            new MemOpRec(Nx),

            new MemOpRec(Nx),
            new MemOpRec(Nx),
            new MemOpRec(Nx),
            new MemOpRec(Nx),
            // B0
            new DstOpRec(M_),
            new DstOpRec(M_),
            new DstOpRec(M_),
            new DstOpRec(M_),

            new DstOpRec(M_),
            new DstOpRec(M_),
            new DstOpRec(M_),
            new DstOpRec(M_),

            new DstOpRec(N_),
            new DstOpRec(N_),
            new DstOpRec(N_),
            new DstOpRec(N_),

            new DstOpRec(N_),
            new DstOpRec(N_),
            new DstOpRec(N_),
            new DstOpRec(N_),
            // C0
            new MemOpRec(Ob),
            new MemOpRec(Pb),
            new MemOpRec(Qb),
            new MemOpRec(mb),

            new MemOpRec(Pre(PrimitiveType.Byte)),
            new MemOpRec(Post(PrimitiveType.Byte)),
            Instr(Opcode.invalid),
            new ExtraRegOprec(PrimitiveType.Byte),

            new RegOpRec(rb),
            new RegOpRec(rb),
            new RegOpRec(rb),
            new RegOpRec(rb),

            new RegOpRec(rb),
            new RegOpRec(rb),
            new RegOpRec(rb),
            new RegOpRec(rb),
            // D0
            new MemOpRec(Ow),
            new MemOpRec(Pw),
            new MemOpRec(Qw),
            new MemOpRec(mw),

            new MemOpRec(Pre(PrimitiveType.Word16)),
            new MemOpRec(Post(PrimitiveType.Word16)),
            Instr(Opcode.invalid),
            new ExtraRegOprec(PrimitiveType.Word16),

            new RegOpRec(rw),
            new RegOpRec(rw),
            new RegOpRec(rw),
            new RegOpRec(rw),

            new RegOpRec(rw),
            new RegOpRec(rw),
            new RegOpRec(rw),
            new RegOpRec(rw),
            // E0
            new MemOpRec(Ox),
            new MemOpRec(Px),
            new MemOpRec(Qx),
            new MemOpRec(mx),

            new MemOpRec(Pre(PrimitiveType.Word32)),
            new MemOpRec(Post(PrimitiveType.Word32)),
            Instr(Opcode.invalid),
            new ExtraRegOprec(PrimitiveType.Word32),

            new RegOpRec(rx),
            new RegOpRec(rx),
            new RegOpRec(rx),
            new RegOpRec(rx),

            new RegOpRec(rx),
            new RegOpRec(rx),
            new RegOpRec(rx),
            new RegOpRec(rx),

            // F0
            new DstOpRec(O_),
            new DstOpRec(P_),
            new DstOpRec(Q_),
            new DstOpRec(m_),

            new DstOpRec(Pre(null)),
            new DstOpRec(Post(null)),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),

            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),
            Instr(Opcode.swi, i3b),
        };
    }
}
