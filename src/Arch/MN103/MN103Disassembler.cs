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

using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System.Collections.Generic;

namespace Reko.Arch.MN103
{
    public class MN103Disassembler : DisassemblerBase<MN103Instruction, Mnemonic>
    {
        private static readonly Decoder<MN103Disassembler, Mnemonic, MN103Instruction> rootDecoder;
        private static readonly Bitfield bf0l2 = Bf(0, 2);
        private static readonly Bitfield bf2l2 = Bf(2, 2);
        private static readonly Bitfield bf4l2 = Bf(4, 2);

        private readonly MN103Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public MN103Disassembler(MN103Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }


        public override MN103Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            var offset = rdr.Offset;
            if (!rdr.TryReadByte(out var opcode))
                return null;
            var instr = rootDecoder.Decode(opcode, this);
            ops.Clear();
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }

        public override MN103Instruction CreateInvalidInstruction()
        {
            return new MN103Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        public override MN103Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MN103Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override MN103Instruction NotYetImplemented(string message)
        {
            return new MN103Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.nyi,
            };
        }

        private static Mutator<MN103Disassembler> Reg(Bitfield bitfield, RegisterStorage[] regs)
        {
            return (u, d) =>
            {
                var ireg = bitfield.Read(u);
                var reg = regs[ireg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> dn = Reg(Bf(0, 2), Registers.DRegs);
        private static readonly Mutator<MN103Disassembler> dm = Reg(Bf(2, 2), Registers.DRegs);
        private static readonly Mutator<MN103Disassembler> an = Reg(Bf(0, 2), Registers.ARegs);
        private static readonly Mutator<MN103Disassembler> am = Reg(Bf(2, 2), Registers.ARegs);


        private static Mutator<MN103Disassembler> Reg(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> mdr = Reg(Registers.mdr);
        private static readonly Mutator<MN103Disassembler> psw = Reg(Registers.psw);
        private static readonly Mutator<MN103Disassembler> sp = Reg(Registers.sp);

        private static bool regs(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var bRegs))
                return false;
            var regs = new MultipleRegistersOperand(bRegs);
            dasm.ops.Add(regs);
            return true;
        }
        private static bool imm8(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var imm = ImmediateOperand.SByte((sbyte)b);
            dasm.ops.Add(imm);
            return true;
        }

        private static bool imm16(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short u))
                return false;
            var imm = ImmediateOperand.Int16(u);
            dasm.ops.Add(imm);
            return true;
        }

        private static bool imm32(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out uint u))
                return false;
            var imm = ImmediateOperand.Int32((int)u);
            dasm.ops.Add(imm);
            return true;
        }


        private static bool abs16(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort uAbs))
                return false;
            var mem = MemoryOperand.Absolute(uAbs);
            dasm.ops.Add(mem);
            return true;
        }


        private static bool abs32(uint uInstr, MN103Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out uint uAbs))
                return false;
            var addr = Address.Ptr32(uAbs);
            dasm.ops.Add(addr);
            return true;
        }

        private static Mutator<MN103Disassembler> Indirect(Bitfield field)
        {
            return (u, d) =>
            {
                var ireg = field.Read(u);
                var areg = Registers.ARegs[ireg];
                var mem = MemoryOperand.Relative(areg, 0);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> indan = Indirect(bf0l2);
        private static readonly Mutator<MN103Disassembler> indam = Indirect(bf2l2);

        private static bool indsp(uint _, MN103Disassembler dasm)
        {
            var mem = MemoryOperand.Relative(Registers.sp, 0);
            dasm.ops.Add(mem);
            return true;
        }

        private static Mutator<MN103Disassembler> reg_relative_u8(RegisterStorage reg)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out var b))
                    return false;
                var mem = MemoryOperand.Relative(reg, b);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d8_sp = reg_relative_u8(Registers.sp);

        private static Mutator<MN103Disassembler> reg_relative_u16(RegisterStorage reg)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadUInt16(out ushort b))
                    return false;
                var mem = MemoryOperand.Relative(reg, b);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d16_sp = reg_relative_u16(Registers.sp);

        private static Mutator<MN103Disassembler> reg_relative_u32(RegisterStorage reg)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadUInt32(out uint b))
                    return false;
                var mem = MemoryOperand.Relative(reg, (int) b);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d32_sp = reg_relative_u16(Registers.sp);


        private static bool d8_pc(uint uInstr, MN103Disassembler dasm)
        {
            //$REVIEW: is it pc-relative at the start of the instruction or end?
            if (!dasm.rdr.TryReadByte(out var b))
                return false;
            var disp8 = (sbyte) b;
            var addr = dasm.addr + disp8;
            dasm.ops.Add(addr);
            return true;
        }

        private static bool d16_pc(uint uInstr, MN103Disassembler dasm)
        {
            //$REVIEW: is it pc-relative at the start of the instruction or end?
            if (!dasm.rdr.TryReadLeInt16(out var disp16))
                return false;
            var addr = dasm.addr + disp16;
            dasm.ops.Add(addr);
            return true;
        }

        private static bool d32_pc(uint uInstr, MN103Disassembler dasm)
        {
            //$REVIEW: is it pc-relative at the start of the instruction or end?
            if (!dasm.rdr.TryReadLeUInt32(out var udisp32))
                return false;
            var addr = dasm.addr + (int) udisp32;
            dasm.ops.Add(addr);
            return true;
        }

        private static Mutator<MN103Disassembler> Relative_areg_s8(Bitfield field)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out var b))
                    return false;
                var disp8 = (sbyte) b;
                var ireg = field.Read(u);
                var areg = Registers.ARegs[ireg];
                var mem = MemoryOperand.Relative(areg, disp8);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d8_am = Relative_areg_s8(bf2l2);
        private static readonly Mutator<MN103Disassembler> d8_an = Relative_areg_s8(bf0l2);

        private static Mutator<MN103Disassembler> Relative_areg_s16(Bitfield field)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadInt16(out short disp16))
                    return false;
                var ireg = field.Read(u);
                var areg = Registers.ARegs[ireg];
                var mem = MemoryOperand.Relative(areg, disp16);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d16_am = Relative_areg_s16(bf2l2);
        private static readonly Mutator<MN103Disassembler> d16_an = Relative_areg_s16(bf0l2);

        private static Mutator<MN103Disassembler> Relative_areg_s32(Bitfield field)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadInt32(out int disp32))
                    return false;
                var ireg = field.Read(u);
                var areg = Registers.ARegs[ireg];
                var mem = MemoryOperand.Relative(areg, disp32);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> d32_am = Relative_areg_s32(bf2l2);
        private static readonly Mutator<MN103Disassembler> d32_an = Relative_areg_s32(bf0l2);



        private static Mutator<MN103Disassembler> Indexed(Bitfield field)
        {
            return (u, d) =>
            {
                var ireg = field.Read(u);
                var areg = Registers.ARegs[ireg];
                var ixreg = bf4l2.Read(u);
                var idxreg = Registers.DRegs[ireg];
                var mem = MemoryOperand.Indexed(areg, idxreg);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<MN103Disassembler> idx_di_am = Indexed(bf2l2);
        private static readonly Mutator<MN103Disassembler> idx_di_an = Indexed(bf0l2);


        private class ExtensionDecoder : Decoder<MN103Disassembler, Mnemonic, MN103Instruction>
        {
            private readonly Decoder<MN103Disassembler, Mnemonic, MN103Instruction> subdecoder;

            public ExtensionDecoder(Decoder<MN103Disassembler, Mnemonic, MN103Instruction> subdecoder)
            {
                this.subdecoder = subdecoder;
            }
            public override MN103Instruction Decode(uint wInstr, MN103Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out var b))
                    return dasm.CreateInvalidInstruction();
                return subdecoder.Decode(b, dasm);
            }
        }

        private static Decoder<MN103Disassembler, Mnemonic, MN103Instruction> Instr(Mnemonic mnemonic, params Mutator<MN103Disassembler>[] mutators)
        {
            return new InstrDecoder<MN103Disassembler, Mnemonic, MN103Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder<MN103Disassembler, Mnemonic, MN103Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MN103Disassembler>[] mutators)
        {
            return new InstrDecoder<MN103Disassembler, Mnemonic, MN103Instruction>(iclass, mnemonic, mutators);
        }

        private static Decoder<MN103Disassembler, Mnemonic, MN103Instruction> ext(
            Decoder<MN103Disassembler, Mnemonic, MN103Instruction> subdecoder)
        {
            return new ExtensionDecoder(subdecoder);
        }

        static MN103Disassembler()
        {
            var nyi = new NyiDecoder<MN103Disassembler, Mnemonic, MN103Instruction>("nyi");

            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var calls_indan = Instr(Mnemonic.calls, indan);
            var jmp_indan = Instr(Mnemonic.jmp, indan);

            var f0decoders = Mask(4, 4, "  F0",
                Instr(Mnemonic.mov, indam, an),
                Instr(Mnemonic.mov, am, indan),
                invalid,
                invalid,

                Instr(Mnemonic.movbu, indam, dn),
                Instr(Mnemonic.movbu, dm, indan),
                Instr(Mnemonic.movhu, indam, dn),
                Instr(Mnemonic.movhu, dm, indan),

                Instr(Mnemonic.bset, dm, indan),
                Instr(Mnemonic.bclr, dm, indan),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Mask(0, 4, "  F0 F?",
                    calls_indan,
                    calls_indan,
                    calls_indan,
                    calls_indan,

                    jmp_indan,
                    jmp_indan,
                    jmp_indan,
                    jmp_indan,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    Instr(Mnemonic.rets, InstrClass.Transfer|InstrClass.Return),
                    Instr(Mnemonic.rti, InstrClass.Transfer|InstrClass.Return),
                    Instr(Mnemonic.trap, InstrClass.Transfer|InstrClass.Call),
                    invalid));
            var f1decoders = Mask(4, 4, "  F1",
                Instr(Mnemonic.sub, dm, dn),
                Instr(Mnemonic.sub, am, dn),
                Instr(Mnemonic.sub, dm, an),
                Instr(Mnemonic.sub, am, an),

                Instr(Mnemonic.addc, dm, dn),
                Instr(Mnemonic.add, am, dn),
                Instr(Mnemonic.add, dm, an),
                Instr(Mnemonic.add, am, an),

                Instr(Mnemonic.subc, dm, dn),
                Instr(Mnemonic.cmp, am, dn),
                Instr(Mnemonic.cmp, dm, an),
                invalid,

                invalid,
                Instr(Mnemonic.mov, am, dn),
                Instr(Mnemonic.mov, dm, an),
                invalid);


            var f2decoders = Mask(4, 4, "  F2",
                Instr(Mnemonic.and, dm, dn),
                Instr(Mnemonic.or, dm, dn),
                Instr(Mnemonic.xor, dm, dn),
                Mask(2, 2, "  F2 3?",
                    Instr(Mnemonic.not, dn),
                    invalid,
                    invalid,
                    invalid),

                Instr(Mnemonic.mul, dm, dn),
                Instr(Mnemonic.mulu, dm, dn),
                Instr(Mnemonic.div, dm, dn),
                Instr(Mnemonic.divu, dm, dn),

                Mask(2, 2, "  F2 8?",
                    Instr(Mnemonic.rol, dn),
                    Instr(Mnemonic.ror, dn),
                    invalid,
                    invalid),
                Instr(Mnemonic.asl, dm, dn),
                Instr(Mnemonic.lsr, dm, dn),
                Instr(Mnemonic.asr, dm, dn),

                invalid,
                Mask(2, 2, "  F2 D?",
                    Instr(Mnemonic.ext, dn),
                    invalid,
                    invalid,
                    invalid),
                Mask(2, 2, "  F2 E?",
                    Instr(Mnemonic.mov, mdr, dn),
                    Instr(Mnemonic.mov, psw, dn),
                    invalid,
                    invalid),
                Mask(0, 2, "  F2 F?",
                    Instr(Mnemonic.mov, am, sp),
                    invalid,
                    Instr(Mnemonic.mov, dm, mdr),
                    Instr(Mnemonic.mov, dm, psw)));


            var f3decoders = Mask(6, 2, "  F3",
                Instr(Mnemonic.mov, idx_di_am, dn),
                Instr(Mnemonic.mov, dm, idx_di_an),
                Instr(Mnemonic.mov, idx_di_am, an),
                Instr(Mnemonic.mov, am, idx_di_an));

            var f4decoders = Mask(6, 2, "  F4",
                Instr(Mnemonic.movbu, idx_di_am, dn),
                Instr(Mnemonic.movbu, dm, idx_di_an),
                Instr(Mnemonic.movhu, idx_di_am, dn),
                Instr(Mnemonic.movhu, dm, idx_di_an));

            var f5decoders = invalid;
            var f6decoders = invalid;
            var f7decoders = invalid;

            var and_imm8_dn = Instr(Mnemonic.and, imm8, dn);
            var btst_imm8_dn = Instr(Mnemonic.btst, imm8, dn);
            var mov_d8_an_sp = Instr(Mnemonic.mov, d8_an, sp);
            var mov_sp_d8_an = Instr(Mnemonic.mov, d8_an, sp);
            var or_imm8_dn = Instr(Mnemonic.or, imm8, dn);
            var f8decoders = Mask(4, 4, "  F8",
                Instr(Mnemonic.mov, d8_am, dn),
                Instr(Mnemonic.mov, dm, d8_an),
                Instr(Mnemonic.mov, d8_am, an),
                Instr(Mnemonic.mov, am, d8_an),

                Instr(Mnemonic.movbu, d8_am, dn),
                Instr(Mnemonic.movbu, dm, d8_an),
                Instr(Mnemonic.movhu, d8_am, an),
                Instr(Mnemonic.movhu, am, d8_an),

                invalid,
                Mask(0, 2, "  F8 9?",
                    invalid,
                    invalid,
                    Instr(Mnemonic.movbu, dm, d8_sp),
                    Instr(Mnemonic.movhu, dm, d8_sp)),
                invalid,
                Mask(2, 2, "  F8 B?",
                    invalid,
                    invalid,
                    Instr(Mnemonic.movbu, d8_sp, dn),
                    Instr(Mnemonic.movhu, d8_sp, dn)),

                Mask(2, 2, "  F8 C?",
                    Instr(Mnemonic.asl, imm8, dn),
                    Instr(Mnemonic.lsr, imm8, dn),
                    Instr(Mnemonic.asr, imm8, dn),
                    invalid),
                invalid,
                Mask(0, 4, "  F8 E?",
                    and_imm8_dn,
                    and_imm8_dn,
                    and_imm8_dn,
                    and_imm8_dn,

                    or_imm8_dn,
                    or_imm8_dn,
                    or_imm8_dn,
                    or_imm8_dn,

                    Instr(Mnemonic.bvc, d8_pc),
                    Instr(Mnemonic.bvs, d8_pc),
                    Instr(Mnemonic.bnc, d8_pc),
                    Instr(Mnemonic.bns, d8_pc),

                    btst_imm8_dn,
                    btst_imm8_dn,
                    btst_imm8_dn,
                    btst_imm8_dn),
                Mask(0, 4, "  F8 F?",
                    mov_d8_an_sp,
                    mov_d8_an_sp,
                    mov_d8_an_sp,
                    mov_d8_an_sp,

                    mov_sp_d8_an,
                    mov_sp_d8_an,
                    mov_sp_d8_an,
                    mov_sp_d8_an,

                    invalid,
                    invalid,
                    invalid,
                    invalid,

                    invalid,
                    invalid,
                    Instr(Mnemonic.add, imm8, sp),
                    invalid));

            var f9decoders = invalid;
            var fadecoders = Mask(4, 4, "  FA",
                Instr(Mnemonic.mov, d16_am, dn),
                Instr(Mnemonic.mov, dm, d16_an),
                Instr(Mnemonic.mov, d16_am, an),
                Instr(Mnemonic.mov, am, d16_an),

                Instr(Mnemonic.movbu, d16_am, dn),
                Instr(Mnemonic.movbu, dm, d16_an),
                Instr(Mnemonic.movhu, d16_am, dn),
                Instr(Mnemonic.movhu, dm, d16_an),

                Mask(0, 2, "  FA 8?",
                    Instr(Mnemonic.mov, am, abs16),
                    invalid,
                    invalid,
                    invalid),
                Mask(0, 2, "  FA 9?",
                    Instr(Mnemonic.mov, am, d16_sp),
                    Instr(Mnemonic.mov, dm, d16_sp),
                    Instr(Mnemonic.movbu, dm, d16_sp),
                    Instr(Mnemonic.movhu, dm, d16_sp)),
                Mask(2, 2, "  FA A?",
                    Instr(Mnemonic.mov, abs16, an),
                    invalid,
                    invalid,
                    invalid),
                Mask(2, 2, "  FA B?",
                    Instr(Mnemonic.mov, d16_sp, an),
                    Instr(Mnemonic.mov, d16_sp, dn),
                    Instr(Mnemonic.movbu, d16_sp, dn),
                    Instr(Mnemonic.movhu, d16_sp, dn)),

                Mask(2, 2, "  FA C?",
                    Instr(Mnemonic.add, imm16, dn),
                    invalid,
                    Instr(Mnemonic.cmp, imm16, dn),
                    invalid),
                Mask(2, 2, "  FA D?",
                    Instr(Mnemonic.add, imm16, an),
                    invalid,
                    Instr(Mnemonic.cmp, imm16, an),
                    invalid),
                Mask(2, 2, "  FA E?",
                    Instr(Mnemonic.and, imm16, an),
                    Instr(Mnemonic.or, imm16, an),
                    Instr(Mnemonic.xor, imm16, an),
                    Instr(Mnemonic.btst, imm16, an)),
                Mask(2, 2, "  FA F?",
                    Instr(Mnemonic.bset, imm8, d8_an),
                    Instr(Mnemonic.bclr, imm8, d8_an),
                    Instr(Mnemonic.btst, imm8, d8_an),
                    Mask(0, 2, "  FA FC?",
                        Instr(Mnemonic.and, imm16, psw),
                        Instr(Mnemonic.or, imm16, psw),
                        Instr(Mnemonic.add, imm16, sp),
                        Instr(Mnemonic.calls, d16_pc))));
            var fbdecoders = invalid;
            var fcdecoders = Mask(4, 4, "  FC",
                Instr(Mnemonic.mov, d32_am, dn),
                Instr(Mnemonic.mov, dm, d32_an),
                Instr(Mnemonic.mov, d32_am, an),
                Instr(Mnemonic.mov, am, d32_an),

                Instr(Mnemonic.movbu, d32_am, dn),
                Instr(Mnemonic.movbu, dm, d32_an),
                Instr(Mnemonic.movhu, d32_am, dn),
                Instr(Mnemonic.movhu, dm, d32_an),

                Mask(0, 2, "  FC 8?",
                    Instr(Mnemonic.mov, am, abs32),
                    Instr(Mnemonic.mov, dm, abs32),
                    Instr(Mnemonic.movbu, dm, abs32),
                    Instr(Mnemonic.movhu, dm, abs32)),
                Mask(0, 2, "  FC 9?",
                    Instr(Mnemonic.mov, am, d32_sp),
                    Instr(Mnemonic.mov, dm, d32_sp),
                    Instr(Mnemonic.movbu, dm, d32_sp),
                    Instr(Mnemonic.movhu, dm, d32_sp)),
                Mask(2, 2, "  FC A?",
                    Instr(Mnemonic.mov, abs32, an),
                    Instr(Mnemonic.mov, abs32, dn),
                    Instr(Mnemonic.movbu, abs32, dn),
                    Instr(Mnemonic.movhu, abs32, dn)),
                Mask(2, 2, "  FC B?",
                    Instr(Mnemonic.mov, d32_sp, an),
                    Instr(Mnemonic.mov, d32_sp, dn),
                    Instr(Mnemonic.movbu, d32_sp, dn),
                    Instr(Mnemonic.movhu, d32_sp, dn)),
                Mask(2, 2, "  FC C?",
                    Instr(Mnemonic.add, imm32, dn),
                    Instr(Mnemonic.sub, imm32, dn),
                    Instr(Mnemonic.cmp, imm32, dn),
                    Instr(Mnemonic.mov, imm32, dn)),
                Mask(2, 2, "  FC D?",
                    Instr(Mnemonic.add, imm32, an),
                    Instr(Mnemonic.sub, imm32, an),
                    Instr(Mnemonic.cmp, imm32, an),
                    Instr(Mnemonic.mov, imm32, an)),
                Mask(2, 2, "  FC E?",
                    Instr(Mnemonic.and, imm32, dn),
                    Instr(Mnemonic.or, imm32, dn),
                    Instr(Mnemonic.xor, imm32, dn),
                    Instr(Mnemonic.btst, imm32, dn)),
                Mask(0, 4, "  FC F?",
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
                    Instr(Mnemonic.add, imm32, sp),
                    Instr(Mnemonic.calls, d32_sp)));
            var fddecoders = invalid;
            var fedecoders = Sparse(0, 8, "  FE", invalid,
                (0x00, Instr(Mnemonic.bset, imm8, abs32)),
                (0x01, Instr(Mnemonic.bclr, imm8, abs32)),
                (0x02, Instr(Mnemonic.btst, imm8, abs32)),

                (0x80, Instr(Mnemonic.bset, imm8, abs16)),
                (0x81, Instr(Mnemonic.bclr, imm8, abs16)),
                (0x82, Instr(Mnemonic.btst, imm8, abs16)));



            var add_dm_dn = Instr(Mnemonic.add, dm, dn);
            var add_imm8_an = Instr(Mnemonic.add, imm8, an);
            var add_imm8_dn = Instr(Mnemonic.add, imm8, dn);
            var asl2_dn = Instr(Mnemonic.asl2, dn);

            var clr_dm = Instr(Mnemonic.clr, dm);
            var cmp_imm8_an = Instr(Mnemonic.cmp, imm8, an);
            var cmp_imm8_dn = Instr(Mnemonic.cmp, imm8, dn);
            var cmp_am_an = Instr(Mnemonic.cmp, am, an);
            var cmp_dm_dn = Instr(Mnemonic.cmp, dm, dn);

            var extb_dn = Instr(Mnemonic.extb, dn);
            var extbu_dn = Instr(Mnemonic.extbu, dn);
            var exth_dn = Instr(Mnemonic.exth, dn);
            var exthu_dn = Instr(Mnemonic.exthu, dn);
            var inc4_an = Instr(Mnemonic.inc4, an);
            var mov_abs16_dn = Instr(Mnemonic.mov, abs16, dn);
            var mov_am_an = Instr(Mnemonic.mov, am, an);
            var mov_d8_sp_an = Instr(Mnemonic.mov, d8_sp, an);
            var mov_d8_sp_dn = Instr(Mnemonic.mov, d8_sp, dn);
            var mov_dm_abs16 = Instr(Mnemonic.mov, dm, abs16);
            var mov_dm_dn = Instr(Mnemonic.mov, dm, dn);
            var mov_dm_indan = Instr(Mnemonic.mov, dm, indan);
            var mov_imm8_an = Instr(Mnemonic.mov, imm8, an);
            var mov_imm8_dn = Instr(Mnemonic.mov, imm8, dn);
            var mov_imm16_an = Instr(Mnemonic.mov, imm16, an);
            var mov_imm16_dn = Instr(Mnemonic.mov, imm16, dn);
            var mov_indam_dn = Instr(Mnemonic.mov, indam, dn);
            var mov_sp_an = Instr(Mnemonic.mov, sp, an);
            var movbu_abs16_dn = Instr(Mnemonic.movbu, abs16, dn);
            var movbu_dm_abs16 = Instr(Mnemonic.movbu, dm, abs16);
            var movhu_abs16_dn = Instr(Mnemonic.movhu, abs16, dn);
            var movhu_dm_abs16 = Instr(Mnemonic.movhu, dm, abs16);

            rootDecoder = new MaskDecoder<MN103Disassembler, Mnemonic, MN103Instruction>(new Bitfield(0, 8), "MN103",
                // 0x00
                clr_dm,
                mov_dm_abs16,
                movbu_dm_abs16,
                movhu_dm_abs16,

                clr_dm,
                mov_dm_abs16,
                movbu_dm_abs16,
                movhu_dm_abs16,

                clr_dm,
                mov_dm_abs16,
                movbu_dm_abs16,
                movhu_dm_abs16,

                clr_dm,
                mov_dm_abs16,
                movbu_dm_abs16,
                movhu_dm_abs16,

                // 0x10
                extb_dn,
                extb_dn,
                extb_dn,
                extb_dn,

                extbu_dn,
                extbu_dn,
                extbu_dn,
                extbu_dn,

                exth_dn,
                exth_dn,
                exth_dn,
                exth_dn,

                exthu_dn,
                exthu_dn,
                exthu_dn,
                exthu_dn,

                // 0x20
                add_imm8_an,
                add_imm8_an,
                add_imm8_an,
                add_imm8_an,

                mov_imm16_an,
                mov_imm16_an,
                mov_imm16_an,
                mov_imm16_an,

                add_imm8_dn,
                add_imm8_dn,
                add_imm8_dn,
                add_imm8_dn,

                mov_imm16_dn,
                mov_imm16_dn,
                mov_imm16_dn,
                mov_imm16_dn,

                // 0x30
                mov_abs16_dn,
                mov_abs16_dn,
                mov_abs16_dn,
                mov_abs16_dn,

                movbu_abs16_dn,
                movbu_abs16_dn,
                movbu_abs16_dn,
                movbu_abs16_dn,

                movhu_abs16_dn,
                movhu_abs16_dn,
                movhu_abs16_dn,
                movhu_abs16_dn,

                mov_sp_an,
                mov_sp_an,
                mov_sp_an,
                mov_sp_an,

                // 0x40
                Instr(Mnemonic.inc, dm),
                Instr(Mnemonic.inc, am),
                Instr(Mnemonic.mov, dm, d8_sp),
                Instr(Mnemonic.mov, am, d8_sp),

                Instr(Mnemonic.inc, dm),
                Instr(Mnemonic.inc, am),
                Instr(Mnemonic.mov, dm, d8_sp),
                Instr(Mnemonic.mov, am, d8_sp),

                Instr(Mnemonic.inc, dm),
                Instr(Mnemonic.inc, am),
                Instr(Mnemonic.mov, dm, d8_sp),
                Instr(Mnemonic.mov, am, d8_sp),

                Instr(Mnemonic.inc, dm),
                Instr(Mnemonic.inc, am),
                Instr(Mnemonic.mov, dm, d8_sp),
                Instr(Mnemonic.mov, am, d8_sp),

                // 0x50
                inc4_an,
                inc4_an,
                inc4_an,
                inc4_an,

                asl2_dn,
                asl2_dn,
                asl2_dn,
                asl2_dn,

                mov_d8_sp_dn,
                mov_d8_sp_dn,
                mov_d8_sp_dn,
                mov_d8_sp_dn,

                mov_d8_sp_an,
                mov_d8_sp_an,
                mov_d8_sp_an,
                mov_d8_sp_an,

                // 0x60
                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,

                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,

                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,

                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,
                mov_dm_indan,

                // 0x70
                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,

                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,

                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,

                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,
                mov_indam_dn,

                // 0x80
                mov_imm8_dn,
                mov_dm_dn,
                mov_dm_dn,
                mov_dm_dn,

                mov_dm_dn,
                mov_imm8_dn,
                mov_dm_dn,
                mov_dm_dn,

                mov_dm_dn,
                mov_dm_dn,
                mov_imm8_dn,
                mov_dm_dn,

                mov_dm_dn,
                mov_dm_dn,
                mov_dm_dn,
                mov_imm8_dn,

                // 0x90
                mov_imm8_an,
                mov_am_an,
                mov_am_an,
                mov_am_an,

                mov_am_an,
                mov_imm8_an,
                mov_am_an,
                mov_am_an,

                mov_am_an,
                mov_am_an,
                mov_imm8_an,
                mov_am_an,

                mov_am_an,
                mov_am_an,
                mov_am_an,
                mov_imm8_an,

                // 0xA0
                cmp_imm8_dn,
                cmp_dm_dn,
                cmp_dm_dn,
                cmp_dm_dn,

                cmp_dm_dn,
                cmp_imm8_dn,
                cmp_dm_dn,
                cmp_dm_dn,

                cmp_dm_dn,
                cmp_dm_dn,
                cmp_imm8_dn,
                cmp_dm_dn,

                cmp_dm_dn,
                cmp_dm_dn,
                cmp_dm_dn,
                cmp_imm8_dn,

                // 0xB0
                cmp_imm8_an,
                cmp_am_an,
                cmp_am_an,
                cmp_am_an,

                cmp_am_an,
                cmp_imm8_an,
                cmp_am_an,
                cmp_am_an,

                cmp_am_an,
                cmp_am_an,
                cmp_imm8_an,
                cmp_am_an,

                cmp_am_an,
                cmp_am_an,
                cmp_am_an,
                cmp_imm8_an,

                // 0xC0
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, d8_pc),

                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, d8_pc),

                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, d8_pc),
                Instr(Mnemonic.bra, InstrClass.Transfer, d8_pc),
                Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding),

                Instr(Mnemonic.jmp, InstrClass.Transfer, d16_pc),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, d16_pc, regs, imm8),
                Instr(Mnemonic.movm, indsp, regs),
                Instr(Mnemonic.movm, regs, indsp),

                // 0xD0
                Instr(Mnemonic.llt),
                Instr(Mnemonic.lgt),
                Instr(Mnemonic.lge),
                Instr(Mnemonic.lle),

                Instr(Mnemonic.lcs),
                Instr(Mnemonic.lhi),
                Instr(Mnemonic.lcc),
                Instr(Mnemonic.lls),

                Instr(Mnemonic.leq),
                Instr(Mnemonic.lne),
                Instr(Mnemonic.lra),
                Instr(Mnemonic.setlb),

                Instr(Mnemonic.jmp, InstrClass.Transfer, d32_pc),
                Instr(Mnemonic.call, InstrClass.Transfer | InstrClass.Call, d32_pc, regs, imm8),
                Instr(Mnemonic.retf, InstrClass.Transfer | InstrClass.Return, regs, imm8),
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, regs, imm8),

                // 0xE0
                add_dm_dn,
                add_dm_dn,
                add_dm_dn,
                add_dm_dn,

                add_dm_dn,
                add_dm_dn,
                add_dm_dn,
                add_dm_dn,

                add_dm_dn,
                add_dm_dn,
                add_dm_dn,
                add_dm_dn,

                add_dm_dn,
                add_dm_dn,
                add_dm_dn,
                add_dm_dn,

                // 0xF0
                ext(f0decoders),
                ext(f1decoders),
                ext(f2decoders),
                ext(f3decoders),

                ext(f4decoders),
                ext(f5decoders),
                ext(f6decoders),
                invalid,

                ext(f8decoders),
                ext(f9decoders),
                ext(fadecoders),
                ext(fbdecoders),

                ext(fcdecoders),
                ext(fddecoders),
                ext(fedecoders),
                invalid);

        }
    }
}