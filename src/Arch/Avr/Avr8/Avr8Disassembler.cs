#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Core.Services;
using Reko.Core.Expressions;

#pragma warning disable IDE1006

namespace Reko.Arch.Avr.Avr8
{
    using Decoder = Decoder<Avr8Disassembler, Mnemonic, Avr8Instruction>;

    // Opcode map: http://lyons42.com/AVR/Opcodes/AVRAllOpcodes.html
    // Opcode map: https://en.wikipedia.org/wiki/Atmel_AVR_instruction_set
    public class Avr8Disassembler : DisassemblerBase<Avr8Instruction, Mnemonic>
    {
        private readonly static Decoder[] decoders;
        private readonly static Decoder invalid;

        private readonly Avr8Architecture arch;
        private readonly EndianImageReader rdr;
        private Address addr;
        private readonly List<MachineOperand> ops;

        public Avr8Disassembler(Avr8Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override Avr8Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            ops.Clear();
            var instr = decoders[wInstr >> 12].Decode(wInstr, this);
            instr.Address = addr;
            var length = rdr.Address - addr;
            instr.Length = (int) length;
            return instr;
        }

        public override Avr8Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Avr8Instruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Operands = this.ops.ToArray(),
            };
        }

        public override Avr8Instruction CreateInvalidInstruction()
        {
            return new Avr8Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        #region Mutators

        private static Mutator<Avr8Disassembler> Inc(RegisterStorage reg)
        {
            return (u, d) => {
                d.ops.Add(new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = reg,
                    PostIncrement = true,
                    PreDecrement = false,
                });
                return true;
            };
        }
        private static readonly Mutator<Avr8Disassembler> IncX = Inc(Registers.x);
        private static readonly Mutator<Avr8Disassembler> IncY = Inc(Registers.y);
        private static readonly Mutator<Avr8Disassembler> IncZ = Inc(Registers.z);

        private static Mutator<Avr8Disassembler> Dec(RegisterStorage reg)
        {
            return (u, d) => {
                d.ops.Add(new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = reg,
                    PostIncrement = false,
                    PreDecrement = true,
                });
                return true;
            };
        }

        private static readonly Mutator<Avr8Disassembler> DecX = Dec(Registers.x);
        private static readonly Mutator<Avr8Disassembler> DecY = Dec(Registers.y);
        private static readonly Mutator<Avr8Disassembler> DecZ = Dec(Registers.z);

        // I/O location
        private static bool A(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (((wInstr >> 5) & 0x30) | (wInstr & 0xF))));
            return true;
        }

        // 8-bit immediate at bits 8..11 and 0..3
        private static bool B(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (((wInstr >> 4) & 0xF0) | (wInstr & 0x0F))));
            return true;
        }

        // 4-bit field in sgis
        private static bool g(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) ((wInstr >> 3) & 0x0F)));
            return true;
        }

        // 3-bit field in sgis, indicating bit number.
        private static bool h(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (wInstr & 7)));
            return true;
        }

        /// <summary>
        /// 4-bit immediate at bit 0
        /// </summary>
        private static bool I(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (wInstr & 0x0F)));
            return true;
        }

        /// <summary>
        /// 4-bit immediate at bit 4
        /// </summary>
        private static bool i(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) ((wInstr >> 4) & 0x0F)));
            return true;
        }

        /// <summary>
        /// Relative jump
        /// </summary>
        private static bool J(uint wInstr, Avr8Disassembler dasm)
        {
            int offset = (short) ((wInstr & 0xFFF) << 4);
            offset = offset >> 3;
            dasm.ops.Add(dasm.addr + 2 + offset);
            return true;
        }

        /// <summary>
        /// Destination register
        /// </summary>
        private static bool D(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(((int) wInstr >> 4) & 0x1F));
            return true;
        }

        /// <summary>
        /// Destination register (r16-r31)
        /// </summary>
        private static bool d(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(0x10 | ((int) wInstr >> 4) & 0x0F));
            return true;
        }

        /// <summary>
        /// Source register (5 bits)
        /// </summary>
        private static bool R(uint wInstr, Avr8Disassembler dasm)
        {
            int iReg = (int) ((wInstr >> 5) & 0x10 | (wInstr) & 0x0F);
            dasm.ops.Add(dasm.Register(iReg));
            return true;
        }

        /// <summary>
        /// Source register (r16-r31)
        /// </summary>
        private static bool r4(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(0x10 | (int) wInstr & 0x0F));
            return true;
        }

        /// <summary>
        /// Source register (5 bits)
        /// </summary>
        private static bool r(uint wInstr, Avr8Disassembler dasm)
        {
            var iReg = (int) ((wInstr >> 4) & 0x10 | (wInstr >> 4) & 0x0F);
            dasm.ops.Add(dasm.Register(iReg));
            return true;
        }

        private static bool K(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (((wInstr >> 4) & 0xF0) | (wInstr & 0xF))));
            return true;
        }

        /// <summary>
        /// Register pair source
        /// </summary>
        private static bool P(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(((int) wInstr << 1) & ~1));
            return true;
        }

        /// <summary>
        /// Register pair destination
        /// </summary>
        private static bool p(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register((int) (wInstr >> 3) & ~1));
            return true;
        }

        /// <summary>
        /// Absolute address used by jump and call.
        /// </summary>
        private static bool Q(uint wInstr, Avr8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w2))
                return false;
            dasm.ops.Add(Address.Ptr32(
                (uint) (((wInstr >> 4) & 0x1F) << 18) |
                (uint) ((wInstr & 1) << 17) |
                (uint) (w2 << 1)));
            return true;
        }

        // register pair used by adiw
        private static bool q(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(24 + ((int) (wInstr >> 3) & 6)));
            return true;
        }

        // immediate used by adiw/sbiw
        private static bool s(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(Constant.Byte((byte) (((wInstr >> 2) & 0x30) | (wInstr & 0xF))));
            return true;
        }

        // Trailing 16-bit absolute address
        private static bool w(uint uInstr, Avr8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w2))
                return false;
            dasm.ops.Add(Address.Ptr16(w2));
            return true;
        }

        // Branch offset
        private static bool o(uint wInstr, Avr8Disassembler dasm) {
            short offset;
            offset = (short) wInstr;
            offset = (short) (offset << 6);
            offset = (short) (offset >> 8);
            offset = (short) (offset & ~1);
            dasm.ops.Add(dasm.addr + offset + 2);
            return true;
        }

        private static bool X(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Registers.x, 0));
            return true;
        }
        private static bool Y(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Registers.y, 0));
            return true;
        }

        private static bool y(uint wInstr, Avr8Disassembler dasm) {
            dasm.ops.Add(dasm.MemD(Registers.y, dasm.Displacement((ushort)wInstr)));
            return true;
        }

        private static bool Z(uint wInstr, Avr8Disassembler dasm) {
            dasm.ops.Add(dasm.MemD(Registers.z, 0));
            return true;
        }

        private static bool z(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Registers.z, dasm.Displacement((ushort) wInstr)));
            return true;
        }

        #endregion

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Avr8Disassembler>[] mutators)
        {
            return new InstrDecoder<Avr8Disassembler, Mnemonic, Avr8Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Avr8Disassembler>[] mutators)
        {
            return new InstrDecoder<Avr8Disassembler, Mnemonic, Avr8Instruction>(iclass, mnemonic, mutators);
        }

        private MachineOperand Register(int v)
        {
            return arch.GetRegister(v & 0x1F);
        }

        private MachineOperand MemD(RegisterStorage baseReg, short displacement)
        {
            return new MemoryOperand(PrimitiveType.Byte)
            {
                Base = baseReg,
                Displacement = displacement
            };
        }

        private short Displacement(ushort wInstr)
        {
            var d = 
                ((wInstr >> 8) & 0x20)
                | ((wInstr >> 7) & 0x18)
                | (wInstr & 7);
            return (short)d;
        }

        public override Avr8Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Avr8_dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        static Avr8Disassembler()
        {
            invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);
            var decoders0 = new Decoder[16]
            {
                Instr(Mnemonic.invalid, InstrClass.Invalid|InstrClass.Zero),
                Instr(Mnemonic.movw, p,P),
                Instr(Mnemonic.muls, d,r4),
                Instr(Mnemonic.muls, d,r4),

                Instr(Mnemonic.cpc, D,R),
                Instr(Mnemonic.cpc, D,R),
                Instr(Mnemonic.cpc, D,R),
                Instr(Mnemonic.cpc, D,R),

                Instr(Mnemonic.sbc, D,R),
                Instr(Mnemonic.sbc, D,R),
                Instr(Mnemonic.sbc, D,R),
                Instr(Mnemonic.sbc, D,R),

                Instr(Mnemonic.add, D,R),
                Instr(Mnemonic.add, D,R),
                Instr(Mnemonic.add, D,R),
                Instr(Mnemonic.add, D,R),
            };

            var decoders1 = new Decoder[]
            {
                Instr(Mnemonic.cpse, D,R),
                Instr(Mnemonic.cp,   D,R),
                Instr(Mnemonic.sub,  D,R),
                Instr(Mnemonic.adc,  D,R),
            };

            var decoders2 = new Decoder[]
            {
                Instr(Mnemonic.and, D,R),
                Instr(Mnemonic.eor, D,R),
                Instr(Mnemonic.or, D,R),
                Instr(Mnemonic.mov, R,r),
            };

            var decoders80 = new Decoder[]
            {
                Instr(Mnemonic.ld,  D,X),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),

                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),

                Instr(Mnemonic.ld,  D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),

                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
            };

            var decoders_std_Z = new Decoder[]
            {
                Instr(Mnemonic.st, Z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),

                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),

                Instr(Mnemonic.st, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),

                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
            };

            var decoders_ldd = new Decoder[]
            {
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),

                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),
                Instr(Mnemonic.ldd, D,z),

                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),

                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
                Instr(Mnemonic.ldd, D,y),
            };

            var decoders_std = new Decoder[]
            {
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),

                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),
                Instr(Mnemonic.std, z,D),

                Instr(Mnemonic.st, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),

                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
                Instr(Mnemonic.std, y,D),
            };

            var decoders8 = new Decoder[8]
            {
                Mask(0, 4, decoders80),
                Mask(0, 4, decoders_std_Z),
                Mask(0, 4, decoders_ldd),
                Mask(0, 4, decoders_std),

                Mask(0, 4, decoders_ldd),
                Mask(0, 4, decoders_std),
                Mask(0, 4, decoders_ldd),
                Mask(0, 4, decoders_std),
            };

            var decoders94_8 = new Decoder[]
            {
                Instr(Mnemonic.sec),
                Instr(Mnemonic.sez),
                Instr(Mnemonic.sen),
                Instr(Mnemonic.sev),

                Instr(Mnemonic.ses),
                Instr(Mnemonic.seh),
                Instr(Mnemonic.set),
                Instr(Mnemonic.sei),

                Instr(Mnemonic.clc),
                Instr(Mnemonic.clz),
                Instr(Mnemonic.cln),
                Instr(Mnemonic.clv),

                Instr(Mnemonic.cls),
                Instr(Mnemonic.clh),
                Instr(Mnemonic.clt),
                Instr(Mnemonic.cli),

                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),
                Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.sleep),
                Instr(Mnemonic.@break),
                Instr(Mnemonic.wdr),
                invalid,

                Instr(Mnemonic.lpm),
                Instr(Mnemonic.elpm),
                Instr(Mnemonic.spm),
                Instr(Mnemonic.spm),
            };

            var decoders95_8 = new Decoder[]
            {
                Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),
                Instr(Mnemonic.reti, InstrClass.Transfer | InstrClass.Return),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.sleep),
                Instr(Mnemonic.@break),
                Instr(Mnemonic.wdr),
                invalid,

                Instr(Mnemonic.lpm),
                Instr(Mnemonic.elpm),
                Instr(Mnemonic.spm),
                Instr(Mnemonic.spm),
            };

            var decoders94_9 = new (uint, Decoder)[]
            {
                ( 0, Instr(Mnemonic.ijmp, InstrClass.Transfer) ),
                ( 1, Instr(Mnemonic.eijmp, InstrClass.Transfer) ),
                ( 16, Instr(Mnemonic.icall, InstrClass.Transfer|InstrClass.Call) ),
                ( 17, Instr(Mnemonic.eicall, InstrClass.Transfer|InstrClass.Call) ),
            };

            var decoders95_9 = new (uint, Decoder)[] 
            {
                ( 0, Instr(Mnemonic.icall, InstrClass.Transfer|InstrClass.Call)),
                ( 1, Instr(Mnemonic.eicall, InstrClass.Transfer|InstrClass.Call)),
            };

            var decoders90 = new Decoder[]
            {
                Instr(Mnemonic.lds, D,w),
                Instr(Mnemonic.ld, D,IncZ),
                Instr(Mnemonic.ld, D,DecZ),
                invalid,

                Instr(Mnemonic.lpm, D,Z),
                Instr(Mnemonic.lpm, D,IncZ),
                Instr(Mnemonic.elpm, D,Z),
                Instr(Mnemonic.elpm, D,IncZ),

                invalid,
                Instr(Mnemonic.ld, D,IncY),
                Instr(Mnemonic.ld, D,DecY),
                invalid,

                Instr(Mnemonic.ld, D,X),
                Instr(Mnemonic.ld, D,IncX),
                Instr(Mnemonic.ld, D,DecX),
                Instr(Mnemonic.pop, D),
            };

            var decoders92 = new Decoder[]
            {
                Instr(Mnemonic.sts, w,D),
                Instr(Mnemonic.st, IncZ,D),
                Instr(Mnemonic.st, DecZ,D),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                Instr(Mnemonic.st, IncY,D),
                Instr(Mnemonic.st, DecY,D),
                invalid,

                Instr(Mnemonic.st, X,D),
                Instr(Mnemonic.st, IncX,D),
                Instr(Mnemonic.st, DecX,D),
                Instr(Mnemonic.push, D),
            };

            var decoders94 = new Decoder[]
            {
                Instr(Mnemonic.com, D),
                Instr(Mnemonic.neg, D),
                Instr(Mnemonic.swap, D),
                Instr(Mnemonic.inc, D),

                invalid,
                Instr(Mnemonic.asr, D),
                Instr(Mnemonic.lsr, D),
                Instr(Mnemonic.ror, D),

                Mask(4, 5, decoders94_8),
                Sparse(4, 5, invalid, decoders94_9),
                Instr(Mnemonic.dec, D),
                Instr(Mnemonic.des, i),

                Instr(Mnemonic.jmp, InstrClass.Transfer, Q),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Q),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Q),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Q),
            };

            var decoders95 = new Decoder[]
            {
                Instr(Mnemonic.com, D),
                Instr(Mnemonic.neg, D),
                Instr(Mnemonic.swap, D),
                Instr(Mnemonic.inc, D),

                invalid,
                Instr(Mnemonic.asr, D),
                Instr(Mnemonic.lsr, D),
                Instr(Mnemonic.ror, D),

                Mask(4, 4, decoders95_8),
                Sparse(4, 5, invalid, decoders95_9),
                Instr(Mnemonic.dec, D),
                invalid,

                Instr(Mnemonic.jmp, InstrClass.Transfer, Q),
                Instr(Mnemonic.jmp, InstrClass.Transfer, Q),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Q),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Q),
            };

            var decoders9 = new Decoder[]
            {
                Mask(0, 4, decoders90),
                Mask(0, 4, decoders90),
                Mask(0, 4, decoders92),
                Mask(0, 4, decoders92),

                Mask(0, 4, decoders94),
                Mask(0, 4, decoders94),   //$TODO: may need a decoders95 for all the invalid des
                Instr(Mnemonic.adiw, q,s),
                Instr(Mnemonic.sbiw, q,s),

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.sbis, g,h),

                invalid,
                invalid,
                invalid,
                invalid,
            };

            var decodersB = new Decoder[]
            {
                Instr(Mnemonic.@in, D,A),
                Instr(Mnemonic.@out, A,D),
            };

            var decoders_sbrc = new Decoder[2]
            {
                Instr(Mnemonic.sbrc, D,I),
                invalid,
            };

            var decoders_sbrs = new Decoder[2]
            {
                Instr(Mnemonic.sbrs, D,I),
                invalid,
            };

            var decodersF = new Decoder[]
            {
                new CondDecoder(),
                new CondDecoder(),
                new CondDecoder(),
                new CondDecoder(),

                new CondDecoder(),
                new CondDecoder(),
                new CondDecoder(),
                new CondDecoder(),

                invalid,
                invalid,
                invalid,
                invalid,

                Mask(3, 1, decoders_sbrc),
                Mask(3, 1, decoders_sbrc),
                Mask(3, 1, decoders_sbrs),
                Mask(3, 1, decoders_sbrs),
            };

            decoders = new Decoder[]
            {
                Mask(8, 4, decoders0),
                Mask(10, 2, decoders1),
                Mask(10, 2, decoders2),
                Instr(Mnemonic.cpi, d,B),

                Instr(Mnemonic.sbci, d,B),
                Instr(Mnemonic.subi, d,B),
                Instr(Mnemonic.ori, d,B),
                Instr(Mnemonic.andi, d,B),

                Mask(9, 3, decoders8),
                Mask(8, 4, decoders9),
                invalid,
                Mask(0xB, 1, decodersB),

                Instr(Mnemonic.rjmp, InstrClass.Transfer, J),
                Instr(Mnemonic.rcall, InstrClass.Transfer|InstrClass.Call, J),
                Instr(Mnemonic.ldi, d,K),
                Mask(8, 4, decodersF),
            };
        }

        public class CondDecoder : Decoder
        {
            static readonly Mnemonic[] branches = new Mnemonic[]
            {
                Mnemonic.brcs, Mnemonic.breq, Mnemonic.brmi, Mnemonic.brvs,
                Mnemonic.brlt, Mnemonic.brhs, Mnemonic.brts, Mnemonic.brie,

                Mnemonic.brcc, Mnemonic.brne, Mnemonic.brpl, Mnemonic.brvc,
                Mnemonic.brge, Mnemonic.brhc, Mnemonic.brtc, Mnemonic.brid
            };

            public override Avr8Instruction Decode(uint uInstr, Avr8Disassembler dasm)
            {
                ushort wInstr = (ushort) uInstr;
                int br = (((wInstr >> 7) & 8) | (wInstr & 7)) & 0xF;
                o(wInstr, dasm);
                return new Avr8Instruction
                {
                    InstructionClass = InstrClass.ConditionalTransfer,
                    Mnemonic = branches[br],
                    Operands = dasm.ops.ToArray()
                };
            }
        }
    }
}