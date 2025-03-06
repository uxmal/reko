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
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Telink
{
    public class TC32Disassembler : DisassemblerBase<TC32Instruction, Mnemonic>
    {
        private static readonly Decoder<TC32Disassembler, Mnemonic, TC32Instruction> rootDecoder;

        private readonly TC32Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr = default!;

        public TC32Disassembler(TC32Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override TC32Instruction CreateInvalidInstruction()
        {
            return new TC32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override TC32Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            var offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }

        public override TC32Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new TC32Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override TC32Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("TC32Dasm", this.addr, this.rdr, message);
            return new TC32Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Nyi,
            };
        }

        private static readonly RegisterStorage[] reg3encodings = Registers.GpRegisters.Take(8).ToArray();
        private static readonly (Mnemonic, InstrClass)[] branches = new (Mnemonic, InstrClass)[16]
        {
            (Mnemonic.tjeq, InstrClass.ConditionalTransfer),
            (Mnemonic.tjne, InstrClass.ConditionalTransfer),
            (Mnemonic.tjcs, InstrClass.ConditionalTransfer),
            (Mnemonic.tjcc, InstrClass.ConditionalTransfer),
            (Mnemonic.tjmi, InstrClass.ConditionalTransfer),
            (Mnemonic.tjpl, InstrClass.ConditionalTransfer),
            (Mnemonic.tjvs, InstrClass.ConditionalTransfer),
            (Mnemonic.tjvc, InstrClass.ConditionalTransfer),
            (Mnemonic.tjhi, InstrClass.ConditionalTransfer),
            (Mnemonic.tjls, InstrClass.ConditionalTransfer),
            (Mnemonic.tjge, InstrClass.ConditionalTransfer),
            (Mnemonic.tjlt, InstrClass.ConditionalTransfer),
            (Mnemonic.tjgt, InstrClass.ConditionalTransfer),
            (Mnemonic.tjle, InstrClass.ConditionalTransfer),
            (Mnemonic.tjal, InstrClass.Transfer),
            (Mnemonic.Invalid, InstrClass.Invalid)
        };

        private static Mutator<TC32Disassembler> Register3(int bitpos)
        {
            var field = new Bitfield(bitpos, 3);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                var reg = reg3encodings[ireg];
                d.ops.Add(reg);
                return true;
            };
        }

        private static readonly Mutator<TC32Disassembler> r0_3 = Register3(0);
        private static readonly Mutator<TC32Disassembler> r8_3 = Register3(8);

        private static Mutator<TC32Disassembler> Register(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<TC32Disassembler> reg_LR = Register(Registers.lr);


        private static Mutator<TC32Disassembler> immediate(int bitpos, int length)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var value = field.Read(u);
                var imm = Constant.Word32(value);
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<TC32Disassembler> imm_0_8 = immediate(0, 8);


        private static bool disp_0_8(uint uInstr, TC32Disassembler dasm)
        {
            var displacement = bf0_8.ReadSigned(uInstr) << 1;
            var addrTarget = dasm.addr + displacement;
            dasm.ops.Add(addrTarget);
            return true;
        }

        private static readonly Bitfield bf0_8 = new Bitfield(0, 8);
        private static readonly Bitfield bf3_3 = new Bitfield(3, 3);
        private static readonly Bitfield bf6_5 = new Bitfield(6, 5);

        private static bool M3_3_u6_5(uint uInstr, TC32Disassembler dasm)
        {
            var ireg = bf3_3.Read(uInstr);
            var baseReg = reg3encodings[ireg];
            var offset = bf6_5.Read(uInstr);
            var mem = MemoryOperand.Create(PrimitiveType.Word32, baseReg, (int) offset);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool Mpc_u0_8_x4(uint uInstr, TC32Disassembler dasm)
        {
            var offset = bf0_8.Read(uInstr) * 4;
            var mem = MemoryOperand.Create(PrimitiveType.Word32, Registers.pc, (int) offset);
            dasm.ops.Add(mem);
            return true;
        }

        private static Decoder<TC32Disassembler, Mnemonic, TC32Instruction> Instr(
            (Mnemonic, InstrClass)[] values,
            int bitsize, int length,
            params Mutator<TC32Disassembler>[] mutators)
        {
            var field = new Bitfield(bitsize, length);
            var decoders = values
                .Select(mi => new InstrDecoder<TC32Disassembler, Mnemonic, TC32Instruction>(mi.Item2, mi.Item1, mutators))
                .ToArray();
            return Mask(bitsize, length, decoders);
        }

        static TC32Disassembler()
        {
            var nyi = new NyiDecoder<TC32Disassembler, Mnemonic, TC32Instruction>("nyi");

            var decode11_5 = Sparse(11, 5, "TC32", nyi,
                (0b00001, Instr(Mnemonic.tloadr, r8_3, Mpc_u0_8_x4)),
                (0b01001, Instr(Mnemonic.tloadrb, r0_3, M3_3_u6_5)),
                (0b01100, Sparse(9, 7, "  01100", nyi,
                    (0b0110010, Instr(Mnemonic.tpush, reg_LR)))),
                (0b10101, Instr(Mnemonic.tcmp, r8_3, imm_0_8)));

            rootDecoder = Sparse(12, 4, "TC32", decode11_5,
                (0b1100, Instr(branches, 8, 4, disp_0_8)));

        }
    }
}