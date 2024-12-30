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

namespace Reko.Arch.Angstrem
{
    using Decoder = Decoder<KR1878Disassembler, Mnemonic, KR1878Instruction>;

    public class KR1878Disassembler : DisassemblerBase<KR1878Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly KR1878Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public KR1878Disassembler(KR1878Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override KR1878Instruction? DisassembleInstruction()
        {
            addr = rdr.Address;
            var offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            this.ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }


        public override KR1878Instruction CreateInvalidInstruction()
        {
            return new KR1878Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override KR1878Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new KR1878Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
            return instr;
        }

        public override KR1878Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("KR1878Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static Mutator<KR1878Disassembler> RegisterField(int bitpos, int length, RegisterStorage[] regs)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(regs[ireg]);
                return true;
            };
        }
        private static readonly Mutator<KR1878Disassembler> R0 = RegisterField(0, 5, Registers.GpRegisters);
        private static readonly Mutator<KR1878Disassembler> R0L3 = RegisterField(0, 3, Registers.GpRegisters);
        private static readonly Mutator<KR1878Disassembler> R5 = RegisterField(5, 5, Registers.GpRegisters);
        private static readonly Mutator<KR1878Disassembler> SPR0 = RegisterField(0, 3, Registers.ServiceRegisters);
        private static readonly Mutator<KR1878Disassembler> SPR5 = RegisterField(5, 3, Registers.ServiceRegisters);

        private static Mutator<KR1878Disassembler> RegisterField(int bitpos)
        {
            var field = new Bitfield(bitpos, 5);
            return (u, d) =>
            {
                var ireg = field.Read(u);
                d.ops.Add(Registers.GpRegisters[ireg]);
                return true;
            };
        }


        private static Mutator<KR1878Disassembler> Immediate(Bitfield field, DataType dt)
        {
            return (u, d) =>
            {
                var value = field.Read(u);
                var imm = ImmediateOperand.Create(Constant.Create(dt, value));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<KR1878Disassembler> I5L8 = Immediate(new Bitfield(5, 8), PrimitiveType.Byte);
        private static readonly Mutator<KR1878Disassembler> I3L8 = Immediate(new Bitfield(3, 8), PrimitiveType.Byte);
        private static readonly Mutator<KR1878Disassembler> b0 = Immediate(new Bitfield(0, 4), PrimitiveType.Byte);


        private static readonly Bitfield a10field = new Bitfield(0, 10);
        private static bool A10(uint uInstr, KR1878Disassembler dasm)
        {
            var a = a10field.Read(uInstr);
            var target = Address.Ptr16((ushort)a);
            dasm.ops.Add(target);
            return true;
        }

        private static Mutator<KR1878Disassembler> SignedImmediate(Bitfield field, DataType dt)
        {
            return (u, d) =>
            {
                var value = field.ReadSigned(u);
                var imm = ImmediateOperand.Create(Constant.Create(dt, value));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<KR1878Disassembler> S5L5 = SignedImmediate(new Bitfield(5, 5), PrimitiveType.SByte);


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder<KR1878Disassembler, Mnemonic, KR1878Instruction>(message);
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<KR1878Disassembler>[] mutators)
        {
            return new InstrDecoder<KR1878Disassembler, Mnemonic, KR1878Instruction>(
                InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<KR1878Disassembler>[] mutators)
        {
            return new InstrDecoder<KR1878Disassembler, Mnemonic, KR1878Instruction>(
                iclass, mnemonic, mutators);
        }

        static KR1878Disassembler()
        {
            var ldr = Instr(Mnemonic.ldr, SPR0, I3L8);
            var mfpr = Instr(Mnemonic.mfpr, R0, SPR5);
            var mtpr = Instr(Mnemonic.mtpr, SPR5, R0);
            var pop = Instr(Mnemonic.pop, R0L3);
            var push = Instr(Mnemonic.push, R0L3);

            var decoder_000_000_00000 = Mask(0, 5, "000_000_00000",
                Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding | InstrClass.Zero),
                Instr(Mnemonic.wait, InstrClass.Linear| InstrClass.Privileged),
                Instr(Mnemonic.reset, InstrClass.Linear | InstrClass.Privileged),
                Instr(Mnemonic.ijmp, InstrClass.Transfer),

                Instr(Mnemonic.tof),
                Instr(Mnemonic.tdc),
                Instr(Mnemonic.sksp),
                Instr(Mnemonic.ijsr, InstrClass.Transfer| InstrClass.Call),

                Instr(Mnemonic.stop, InstrClass.Linear| InstrClass.Privileged),
                Nyi("01001"),
                Nyi("01010"),
                Nyi("01011"),

                Instr(Mnemonic.rts, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.rti, InstrClass.Transfer | InstrClass.Return),
                Instr(Mnemonic.rts0, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.rts1, InstrClass.Transfer|InstrClass.Return),

                push,
                push,
                push,
                push,

                push,
                push,
                push,
                push,

                pop,
                pop,
                pop,
                pop,

                pop,
                pop,
                pop,
                pop);

            var decoder_000_000 = Mask(5, 5, "  000_000",
                decoder_000_000_00000,
                Instr(Mnemonic.swap, R0),
                Instr(Mnemonic.neg, R0),
                Instr(Mnemonic.not, R0),

                Instr(Mnemonic.shl, R0),
                Instr(Mnemonic.shr, R0),
                Instr(Mnemonic.shra, R0),
                Instr(Mnemonic.rlc, R0),

                Instr(Mnemonic.rrc, R0),
                Instr(Mnemonic.adc, R0),
                Instr(Mnemonic.sbc, R0),
                Nyi("01011"),

                Instr(Mnemonic.sst, b0),
                Nyi("01101"),
                Instr(Mnemonic.cst, b0),
                Nyi("01111"),

                mtpr,
                mtpr,
                mtpr,
                mtpr,

                mtpr,
                mtpr,
                mtpr,
                mtpr,

                mfpr,
                mfpr,
                mfpr,
                mfpr,

                mfpr,
                mfpr,
                mfpr,
                mfpr);


            rootDecoder = Mask(13, 3, "KR1878",
                Mask(10, 3, "  000",
                    decoder_000_000,
                    Instr(Mnemonic.mov, R0, R5),
                    Instr(Mnemonic.cmp, R0, R5),
                    Instr(Mnemonic.sub, R0, R5),
                    Instr(Mnemonic.xor, R0, R5),
                    Instr(Mnemonic.and, R0, R5),
                    Instr(Mnemonic.or, R0, R5),
                    Instr(Mnemonic.xor, R0, R5)),
                Mask(10, 3, "  001",
                    ldr,
                    ldr,
                    Instr(Mnemonic.bic, R0, S5L5),
                    Instr(Mnemonic.subl, R0, S5L5),
                    Instr(Mnemonic.addl, R0, S5L5),
                    Instr(Mnemonic.btt, R0, S5L5),
                    Instr(Mnemonic.bis, R0, S5L5),
                    Instr(Mnemonic.btg, R0, S5L5)),
                Instr(Mnemonic.movl, R0, I5L8),
                Instr(Mnemonic.cmpl, R0, I5L8),
                Mask(12, 1, "  100",
                    Instr(Mnemonic.jmp, InstrClass.Transfer, A10),
                    Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, A10)),
                Mask(12, 1, "  101",
                    Instr(Mnemonic.jz, InstrClass.ConditionalTransfer, A10),
                    Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, A10)),
                Mask(12, 1, "  110",
                    Instr(Mnemonic.jns, InstrClass.ConditionalTransfer, A10),
                    Instr(Mnemonic.js, InstrClass.ConditionalTransfer, A10)),
                Mask(12, 1, "  111",
                    Instr(Mnemonic.jnc, InstrClass.ConditionalTransfer, A10),
                    Instr(Mnemonic.jc, InstrClass.ConditionalTransfer, A10)));
        }
    }
}
