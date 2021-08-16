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
using System;
using System.Collections.Generic;

namespace Reko.Arch.Tms320C28
{
    using Decoder = Decoder<Tms320C28Disassembler, Mnemonic, Tms320C28Instruction>;

    public class Tms320C28Disassembler : DisassemblerBase<Tms320C28Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly Tms320C28Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public Tms320C28Disassembler(Tms320C28Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override Tms320C28Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            ops.Clear();
            if (uInstr == 0)
                instr.InstructionClass |= InstrClass.Zero;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Tms320C28Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Tms320C28Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Address = addr,
                Operands = ops.ToArray()
            };
        }

        public override Tms320C28Instruction CreateInvalidInstruction()
        {
            return new Tms320C28Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        private static Mutator<Tms320C28Disassembler> Reg(RegisterStorage reg)
        {
            var op = new RegisterOperand(reg);
            return (u, d) =>
            {
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<Tms320C28Disassembler> acc = Reg(Registers.acc);

        private static Mutator<Tms320C28Disassembler> UImmediate(PrimitiveType dt, int bitpos, int bitlength)
        {
            var field = new Bitfield(bitpos, bitlength);
            return (u, d) =>
            {
                var imm = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, imm)));
                return true;
            };
        }
        private static readonly Mutator<Tms320C28Disassembler> Imm0_8 = UImmediate(PrimitiveType.Byte, 0, 8);

        public override Tms320C28Instruction NotYetImplemented(string message)
        {
            var svc = arch.Services.GetService<ITestGenerationService>();
            svc?.ReportMissingDecoder("Tms320C28", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Tms320C28Disassembler>[] mutators) =>
            Instr(mnemonic, InstrClass.Linear, mutators);

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Tms320C28Disassembler>[] mutators) =>
            new InstrDecoder<Tms320C28Disassembler, Mnemonic, Tms320C28Instruction>(iclass, mnemonic, mutators);

        private static Decoder Nyi(string message) =>
            new NyiDecoder<Tms320C28Disassembler, Mnemonic, Tms320C28Instruction>(message);


        static Tms320C28Disassembler()
        {
            var op00 = Sparse(0, 8, "  op00", Nyi("00"),
                (0x01, Instr(Mnemonic.aborti)));

            rootDecoder = Sparse(8, 8, "TMS320C28", Nyi(""),
                (0x00, op00),
                (0x19, Instr(Mnemonic.subb, acc, Imm0_8)));
        }
    }
}