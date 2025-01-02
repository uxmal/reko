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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Sanyo
{
    using Decoder = Decoder<LC8670Disassembler, Mnemonic, LC8670Instruction>;

    public class LC8670Disassembler : DisassemblerBase<LC8670Instruction, Mnemonic>
    {
        private static readonly Decoder[] rootDecoders;

        private readonly IProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public LC8670Disassembler(LC8670Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override LC8670Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte opcode))
                return null;
            var instr = rootDecoders[opcode].Decode(opcode, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override LC8670Instruction NotYetImplemented(string message)
        {
            var testSvc = arch.Services.GetService<ITestGenerationService>();
            testSvc?.ReportMissingDecoder("LC86104Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        public override LC8670Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new LC8670Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = ops.ToArray()
            };
        }

        public override LC8670Instruction CreateInvalidInstruction()
        {
            return new LC8670Instruction
            {
                Mnemonic = Mnemonic.invalid,
                InstructionClass = InstrClass.Invalid,
            };
        }



        /// <summary>
        /// Absolute 12-bit address
        /// </summary>
        private static bool a12(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var uAddr = (((dasm.addr.ToLinear() + 2) & 0xF000)               // get first part from PC
                        | ((uInstr & 0x07u) << 8)        // get some bits
                        | ((uInstr & 0x10u) != 0 ? 0x800u : 0)  // out of place bit
                        | b);                   // lower 8 bits
            dasm.ops.Add(Address.Ptr16((ushort) uAddr));
            return true;
        }

        private static bool a16(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort uAddr))
                return false;
            dasm.ops.Add(Address.Ptr16(uAddr));
            return true;
        }


        private static bool r8(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            short offset = (sbyte) b;
            var uAddr = (0xFFFF & (dasm.addr.ToLinear() + 2 + (ushort) offset));
            dasm.ops.Add(Address.Ptr16((ushort) uAddr));
            return true;
        }

        private static bool r16(uint uInstr, LC8670Disassembler dasm)
        {
            // warning: byte order is reversed of A16!
            if (!dasm.rdr.TryReadInt16(out short offset))
                return false;
            var uAddr = (0xFFFF & (dasm.addr.ToLinear() + 3 - 1 + (ushort) offset));
            dasm.ops.Add(Address.Ptr16((ushort) uAddr));
            return true;
        }

        private static bool d9(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;

            var n = ((uInstr & 1) << 8) | b;
            dasm.ops.Add(DReg(n));
            return true;
        }

        private static bool d9bit(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var n = ((uInstr & 0x10) << 4 | b);
            dasm.ops.Add(DReg(n));
            
            return true;
        }

        private static MachineOperand DReg(uint n)
        {
            if (Registers.SFR.TryGetValue((int) n, out var reg))
                return reg;
            else
                return new MemoryOperand(PrimitiveType.Byte, (ushort) n);
        }

        private static bool Ri(uint uInstr, LC8670Disassembler dasm)
        {
            var n = uInstr & 0x3;
            var mem = new MemoryOperand(PrimitiveType.Byte, Registers.Reg(n), true);
            dasm.ops.Add(mem);
            return true;
        }

        private static bool reg(uint uInstr, LC8670Disassembler dasm)
        {
            var n = uInstr & 0x3;
            var reg = Registers.Reg(n);
            dasm.ops.Add(reg);
            return true;
        }

        private static bool i8(uint uInstr, LC8670Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            var imm = ImmediateOperand.Byte(b);
            dasm.ops.Add(imm);
            return true;
        }

        private static bool i8_d9(uint uInstr, LC8670Disassembler dasm)
        {
            // ^=#i8,d9 immediate
            if (!d9(uInstr, dasm) || !i8(uInstr, dasm))
                return false;

            var ops = dasm.ops;
            var tmp = ops[0];
            ops[0] = ops[1];
            ops[1] = tmp;
            return true;
        }

        private static bool i8_Ri(uint uInstr, LC8670Disassembler dasm)
        {
            // %=#i8,@Ri
            if (!dasm.rdr.TryReadByte(out var b))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(b));
            return reg(uInstr, dasm);
        }

        private static bool c(uint uInstr, LC8670Disassembler dasm)
        {
            return Ri(uInstr, dasm) && r8(uInstr, dasm);
        }

        private static bool v(uint uInstr, LC8670Disassembler dasm)
        {
            return Ri(uInstr, dasm) && reg(uInstr, dasm) && r8(uInstr, dasm);
        }

        private static bool b(uint uInstr, LC8670Disassembler dasm)
        {
            if (!d9bit(uInstr, dasm))
                return false;
            var bit = (sbyte) (uInstr & 7);
            dasm.ops.Add(ImmediateOperand.SByte(bit));
            return true;
        }

        private static bool r(uint uInstr, LC8670Disassembler dasm)
        {
            if (!d9(uInstr, dasm))
                return false;
            dasm.ops.Add(ImmediateOperand.Int32((int) uInstr & 7));
            return r8(uInstr, dasm);
        }

        private static bool x(uint uInstr, LC8670Disassembler dasm)
        {
            return d9(uInstr, dasm) && r8(uInstr, dasm);
        }

        private static bool z(uint uInstr, LC8670Disassembler dasm)
        {
            return i8(uInstr, dasm) && r8(uInstr, dasm);
        }


        private static Decoder Instr(Mnemonic mnemonic, params Mutator<LC8670Disassembler>[] mutators)
        {
            return new InstrDecoder<LC8670Disassembler, Mnemonic, LC8670Instruction>(InstrClass.Linear, mnemonic, mutators);
        }


        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<LC8670Disassembler>[] mutators)
        {
            return new InstrDecoder<LC8670Disassembler, Mnemonic, LC8670Instruction>(iclass, mnemonic, mutators);
        }



        static LC8670Disassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            rootDecoders = new Decoder[]
            {
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
                Instr(Mnemonic.br, InstrClass.Transfer, r8),
                Instr(Mnemonic.ld, d9),
                Instr(Mnemonic.ld, d9),
                Instr(Mnemonic.ld, Ri),
                Instr(Mnemonic.ld, Ri),
                Instr(Mnemonic.ld, Ri),
                Instr(Mnemonic.ld, Ri),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),

                Instr(Mnemonic.callr, InstrClass.Transfer|InstrClass.Call, r16),
                Instr(Mnemonic.brf, InstrClass.Transfer, r16),
                Instr(Mnemonic.st, d9),
                Instr(Mnemonic.st, d9),
                Instr(Mnemonic.st, Ri),
                Instr(Mnemonic.st, Ri),
                Instr(Mnemonic.st, Ri),
                Instr(Mnemonic.st, Ri),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, a12),

                Instr(Mnemonic.callf, InstrClass.Transfer|InstrClass.Call, a16),
                Instr(Mnemonic.jmpf, InstrClass.Transfer, a16),
                Instr(Mnemonic.mov, i8_d9),
                Instr(Mnemonic.mov, i8_d9),
                Instr(Mnemonic.mov, i8_Ri),
                Instr(Mnemonic.mov, i8_Ri),
                Instr(Mnemonic.mov, i8_Ri),
                Instr(Mnemonic.mov, i8_Ri),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),

                Instr(Mnemonic.mul),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, z),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.be, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),
                Instr(Mnemonic.jmp, InstrClass.Transfer, a12),

                Instr(Mnemonic.div),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, z),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, v),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer, v),
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
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, x),
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, c),
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, c),
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, c),
                Instr(Mnemonic.dbnz, InstrClass.ConditionalTransfer, c),
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.push, d9),
                Instr(Mnemonic.push, d9),
                Instr(Mnemonic.inc, d9),
                Instr(Mnemonic.inc, d9),
                Instr(Mnemonic.inc, Ri),
                Instr(Mnemonic.inc, Ri),
                Instr(Mnemonic.inc, Ri),
                Instr(Mnemonic.inc, Ri),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),

                Instr(Mnemonic.pop, d9),
                Instr(Mnemonic.pop, d9),
                Instr(Mnemonic.dec, d9),
                Instr(Mnemonic.dec, d9),
                Instr(Mnemonic.dec, Ri),
                Instr(Mnemonic.dec, Ri),
                Instr(Mnemonic.dec, Ri),
                Instr(Mnemonic.dec, Ri),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bp, InstrClass.ConditionalTransfer, r),

                Instr(Mnemonic.bz, InstrClass.ConditionalTransfer, r8),
                Instr(Mnemonic.add, i8),
                Instr(Mnemonic.add, d9),
                Instr(Mnemonic.add, d9),
                Instr(Mnemonic.add, Ri),
                Instr(Mnemonic.add, Ri),
                Instr(Mnemonic.add, Ri),
                Instr(Mnemonic.add, Ri),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),

                Instr(Mnemonic.bnz, InstrClass.ConditionalTransfer, r8),
                Instr(Mnemonic.addc, i8),
                Instr(Mnemonic.addc, d9),
                Instr(Mnemonic.addc, d9),
                Instr(Mnemonic.addc, Ri),
                Instr(Mnemonic.addc, Ri),
                Instr(Mnemonic.addc, Ri),
                Instr(Mnemonic.addc, Ri),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),
                Instr(Mnemonic.bn, InstrClass.ConditionalTransfer, r),

                Instr(Mnemonic.ret, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.sub  , i8),
                Instr(Mnemonic.sub  , d9),
                Instr(Mnemonic.sub  , d9),
                Instr(Mnemonic.sub  , Ri),
                Instr(Mnemonic.sub  , Ri),
                Instr(Mnemonic.sub  , Ri),
                Instr(Mnemonic.sub  , Ri),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),
                Instr(Mnemonic.not1 , b),

                Instr(Mnemonic.reti, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.subc, i8),
                Instr(Mnemonic.subc, d9),
                Instr(Mnemonic.subc, d9),
                Instr(Mnemonic.subc, Ri),
                Instr(Mnemonic.subc, Ri),
                Instr(Mnemonic.subc, Ri),
                Instr(Mnemonic.subc, Ri),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),
                Instr(Mnemonic.not1, b),

                Instr(Mnemonic.ror),
                Instr(Mnemonic.ldc),
                Instr(Mnemonic.xch  , d9),
                Instr(Mnemonic.xch  , d9),
                Instr(Mnemonic.xch  , Ri),
                Instr(Mnemonic.xch  , Ri),
                Instr(Mnemonic.xch  , Ri),
                Instr(Mnemonic.xch  , Ri),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),

                Instr(Mnemonic.rorc),
                Instr(Mnemonic.or   , i8),
                Instr(Mnemonic.or   , d9),
                Instr(Mnemonic.or   , d9),
                Instr(Mnemonic.or   , Ri),
                Instr(Mnemonic.or   , Ri),
                Instr(Mnemonic.or   , Ri),
                Instr(Mnemonic.or   , Ri),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),
                Instr(Mnemonic.clr1 , b),

                Instr(Mnemonic.rol),
                Instr(Mnemonic.and, i8),
                Instr(Mnemonic.and, d9),
                Instr(Mnemonic.and, d9),
                Instr(Mnemonic.and, Ri),
                Instr(Mnemonic.and, Ri),
                Instr(Mnemonic.and, Ri),
                Instr(Mnemonic.and, Ri),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),

                Instr(Mnemonic.rolc),
                Instr(Mnemonic.xor, i8),
                Instr(Mnemonic.xor, d9),
                Instr(Mnemonic.xor, d9),
                Instr(Mnemonic.xor, Ri),
                Instr(Mnemonic.xor, Ri),
                Instr(Mnemonic.xor, Ri),
                Instr(Mnemonic.xor, Ri),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
                Instr(Mnemonic.set1, b),
            };

            // This list need not be ordered because i'm lazy. And the VMU doesn't produce that
            // much code that a decent computer can't cut through it all quick enough.
            //
            // This portion provided by Alexander Villagran -- thanks!

            /*

            */
        }
    }
}