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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System.Diagnostics;

namespace Reko.Arch.Avr
{
    // Opcode map: http://lyons42.com/AVR/Opcodes/AVRAllOpcodes.html
    // Opcode map: https://en.wikipedia.org/wiki/Atmel_AVR_instruction_set
    public class Avr8Disassembler : DisassemblerBase<AvrInstruction>
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

        public override AvrInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            ops.Clear();
            var instr = decoders[wInstr >> 12].Decode(this, wInstr);
            instr.Address = addr;
            var length = rdr.Address - addr;
            instr.Length = (int) length;

#if DEBUG
            if (instr.opcode == Opcode.invalid) EmitUnitTest(wInstr);
#endif
            return instr;
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
        private static readonly Mutator<Avr8Disassembler> IncX = Inc(Avr8Architecture.x);
        private static readonly Mutator<Avr8Disassembler> IncY = Inc(Avr8Architecture.y);
        private static readonly Mutator<Avr8Disassembler> IncZ = Inc(Avr8Architecture.z);

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

        private static Mutator<Avr8Disassembler> DecX = Dec(Avr8Architecture.x);
        private static Mutator<Avr8Disassembler> DecY = Dec(Avr8Architecture.y);
        private static Mutator<Avr8Disassembler> DecZ = Dec(Avr8Architecture.z);

        // I/O location
        private static bool A(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (((wInstr >> 5) & 0x30) | (wInstr & 0xF))));
            return true;
        }

        // 8-bit immediate at bits 8..11 and 0..3
        private static bool B(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (((wInstr >> 4) & 0xF0) | (wInstr & 0x0F))));
            return true;
        }

        // 4-bit field in sgis
        private static bool g(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) ((wInstr >> 3) & 0x0F)));
            return true;
        }

        // 3-bit field in sgis, indicating bit nr.
        private static bool h(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (wInstr & 7)));
            return true;
        }

        // 4-bit immediate at bit 0
        private static bool I(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (wInstr & 0x0F)));
            return true;
        }

        // 4-bit immediate at bit 4
        private static bool i(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) ((wInstr >> 4) & 0x0F)));
            return true;
        }

        // Relative jump
        private static bool J(uint wInstr, Avr8Disassembler dasm)
        {
            int offset = (short) ((wInstr & 0xFFF) << 4);
            offset = offset >> 3;
            dasm.ops.Add(AddressOperand.Create(dasm.addr + 2 + offset));
            return true;
        }

        // Destination register
        private static bool D(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(((int) wInstr >> 4) & 0x1F));
            return true;
        }

        // Destination register (r16-r31)
        private static bool d(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(0x10 | ((int) wInstr >> 4) & 0x0F));
            return true;
        }

        // source register (5 bits)
        private static bool R(uint wInstr, Avr8Disassembler dasm)
        {
            int iReg = (int) ((wInstr >> 5) & 0x10 | (wInstr) & 0x0F);
            dasm.ops.Add(dasm.Register(iReg));
            return true;
        }

        // source register (r16-r31)
        private static bool r4(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(0x10 | (int) wInstr & 0x0F));
            return true;
        }

        // source register (5 bits)
        private static bool r(uint wInstr, Avr8Disassembler dasm)
        {
            var iReg = (int) ((wInstr >> 4) & 0x10 | (wInstr >> 4) & 0x0F);
            dasm.ops.Add(dasm.Register(iReg));
            return true;
        }

        private static bool K(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) (((wInstr >> 4) & 0xF0) | (wInstr & 0xF))));
            return true;
        }

        // register pair source
        private static bool P(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register(((int) wInstr << 1) & ~1));
            return true;
        }

        // register pair destination
        private static bool p(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.Register((int) (wInstr >> 3) & ~1));
            return true;
        }

        // absolute address used by jump and call.
        private static bool Q(uint wInstr, Avr8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w2))
                return false;
            dasm.ops.Add(AddressOperand.Ptr32(
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
            dasm.ops.Add(ImmediateOperand.Byte((byte) (((wInstr >> 2) & 0x30) | (wInstr & 0xF))));
            return true;
        }

        // Trailing 16-bit absolute address
        private static bool w(uint uInstr, Avr8Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort w2))
                return false;
            dasm.ops.Add(AddressOperand.Ptr16(w2));
            return true;
        }

        // Branch offset
        private static bool o(uint wInstr, Avr8Disassembler dasm) {
            short offset;
            offset = (short) wInstr;
            offset = (short) (offset << 6);
            offset = (short) (offset >> 8);
            offset = (short) (offset & ~1);
            dasm.ops.Add(AddressOperand.Create(dasm.addr + offset + 2));
            return true;
        }

        private static bool X(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Avr8Architecture.x, 0));
            return true;
        }
        private static bool Y(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Avr8Architecture.y, 0));
            return true;
        }

        private static bool y(uint wInstr, Avr8Disassembler dasm) {
            dasm.ops.Add(dasm.MemD(Avr8Architecture.y, dasm.Displacement((ushort)wInstr)));
            return true;
        }

        private static bool Z(uint wInstr, Avr8Disassembler dasm) {
            dasm.ops.Add(dasm.MemD(Avr8Architecture.z, 0));
            return true;
        }

        private static bool z(uint wInstr, Avr8Disassembler dasm)
        {
            dasm.ops.Add(dasm.MemD(Avr8Architecture.z, dasm.Displacement((ushort) wInstr)));
            return true;
        }

        #endregion

        private static InstrDecoder Instr(Opcode opcode, params Mutator<Avr8Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, InstrClass.Linear, mutators);
        }

        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<Avr8Disassembler>[] mutators)
        {
            return new InstrDecoder(opcode, iclass, mutators);
        }


        private MachineOperand Register(int v)
        {
            return new RegisterOperand(arch.GetRegister(v & 0x1F));
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

        private void EmitUnitTest(ushort wInstr)
        {
            var instrHex = $"{wInstr:X4}";
            base.EmitUnitTest("AVR8", instrHex, "", "Avr8_dis", this.addr, w =>
            {
                w.WriteLine("            AssertCode(\"@@@\", 0x{0:X4});", wInstr);
            });
        }

        static Avr8Disassembler()
        {
            invalid = Instr(Opcode.invalid, InstrClass.Invalid);
            var decoders0 = new Decoder[16]
            {
                Instr(Opcode.invalid, InstrClass.Invalid|InstrClass.Zero),
                Instr(Opcode.movw, p,P),
                Instr(Opcode.muls, d,r4),
                Instr(Opcode.muls, d,r4),

                Instr(Opcode.cpc, D,R),
                Instr(Opcode.cpc, D,R),
                Instr(Opcode.cpc, D,R),
                Instr(Opcode.cpc, D,R),

                Instr(Opcode.sbc, D,R),
                Instr(Opcode.sbc, D,R),
                Instr(Opcode.sbc, D,R),
                Instr(Opcode.sbc, D,R),

                Instr(Opcode.add, D,R),
                Instr(Opcode.add, D,R),
                Instr(Opcode.add, D,R),
                Instr(Opcode.add, D,R),
            };

            var decoders1 = new Decoder[]
            {
                Instr(Opcode.cpse, D,R),
                Instr(Opcode.cp,   D,R),
                Instr(Opcode.sub,  D,R),
                Instr(Opcode.adc,  D,R),
            };

            var decoders2 = new Decoder[]
            {
                Instr(Opcode.and, D,R),
                Instr(Opcode.eor, D,R),
                Instr(Opcode.or, D,R),
                Instr(Opcode.mov, R,r),
            };

            var decoders80 = new Decoder[]
            {
                Instr(Opcode.ld,  D,X),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),

                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),

                Instr(Opcode.ld,  D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),

                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
            };

            var decoders_std_Z = new Decoder[]
            {
                Instr(Opcode.st, Z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),

                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),

                Instr(Opcode.st, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),

                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
            };

            var decoders_ldd = new Decoder[]
            {
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),

                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),
                Instr(Opcode.ldd, D,z),

                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),

                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
                Instr(Opcode.ldd, D,y),
            };

            var decoders_std = new Decoder[]
            {
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),

                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),
                Instr(Opcode.std, z,D),

                Instr(Opcode.st, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),

                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
                Instr(Opcode.std, y,D),
            };

            var decoders8 = new Decoder[8]
            {
                new MaskDecoder(0, 4, decoders80),
                new MaskDecoder(0, 4, decoders_std_Z),
                new MaskDecoder(0, 4, decoders_ldd),
                new MaskDecoder(0, 4, decoders_std),

                new MaskDecoder(0, 4, decoders_ldd),
                new MaskDecoder(0, 4, decoders_std),
                new MaskDecoder(0, 4, decoders_ldd),
                new MaskDecoder(0, 4, decoders_std),
            };

            var decoders94_8 = new Decoder[]
            {
                Instr(Opcode.sec),
                Instr(Opcode.sez),
                Instr(Opcode.sen),
                Instr(Opcode.sev),

                Instr(Opcode.ses),
                Instr(Opcode.seh),
                Instr(Opcode.set),
                Instr(Opcode.sei),

                Instr(Opcode.clc),
                Instr(Opcode.clz),
                Instr(Opcode.cln),
                Instr(Opcode.clv),

                Instr(Opcode.cls),
                Instr(Opcode.clh),
                Instr(Opcode.clt),
                Instr(Opcode.cli),

                Instr(Opcode.ret, InstrClass.Transfer),
                Instr(Opcode.reti, InstrClass.Transfer),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Opcode.sleep),
                Instr(Opcode.@break),
                Instr(Opcode.wdr),
                invalid,

                Instr(Opcode.lpm),
                Instr(Opcode.elpm),
                Instr(Opcode.spm),
                Instr(Opcode.spm),
            };

            var decoders95_8 = new Decoder[]
            {
                Instr(Opcode.ret, InstrClass.Transfer),
                Instr(Opcode.reti, InstrClass.Transfer),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Opcode.sleep),
                Instr(Opcode.@break),
                Instr(Opcode.wdr),
                invalid,

                Instr(Opcode.lpm),
                Instr(Opcode.elpm),
                Instr(Opcode.spm),
                Instr(Opcode.spm),
            };

            var decoders94_9 = new Dictionary<int, Decoder>
            {
                { 0, Instr(Opcode.ijmp, InstrClass.Transfer) },
                { 1, Instr(Opcode.eijmp, InstrClass.Transfer) },
                { 16, Instr(Opcode.icall, InstrClass.Transfer|InstrClass.Call) },
                { 17, Instr(Opcode.eicall, InstrClass.Transfer|InstrClass.Call) },
            };

            var decoders95_9 = new Dictionary<int, Decoder>
            {
                { 0, Instr(Opcode.icall, InstrClass.Transfer|InstrClass.Call) },
                { 1, Instr(Opcode.eicall, InstrClass.Transfer|InstrClass.Call) },
            };

            var decoders90 = new Decoder[]
            {
                Instr(Opcode.lds, D,w),
                Instr(Opcode.ld, D,IncZ),
                Instr(Opcode.ld, D,DecZ),
                invalid,

                Instr(Opcode.lpm, D,Z),
                Instr(Opcode.lpm, D,IncZ),
                Instr(Opcode.elpm, D,Z),
                Instr(Opcode.elpm, D,IncZ),

                invalid,
                Instr(Opcode.ld, D,IncY),
                Instr(Opcode.ld, D,DecY),
                invalid,

                Instr(Opcode.ld, D,X),
                Instr(Opcode.ld, D,IncX),
                Instr(Opcode.ld, D,DecX),
                Instr(Opcode.pop, D),
            };

            var decoders92 = new Decoder[]
            {
                Instr(Opcode.sts, w,D),
                Instr(Opcode.st, IncZ,D),
                Instr(Opcode.st, DecZ,D),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                Instr(Opcode.st, IncY,D),
                Instr(Opcode.st, DecY,D),
                invalid,

                Instr(Opcode.st, X,D),
                Instr(Opcode.st, IncX,D),
                Instr(Opcode.st, DecX,D),
                Instr(Opcode.push, D),
            };

            var decoders94 = new Decoder[]
            {
                Instr(Opcode.com, D),
                Instr(Opcode.neg, D),
                Instr(Opcode.swap, D),
                Instr(Opcode.inc, D),

                invalid,
                Instr(Opcode.asr, D),
                Instr(Opcode.lsr, D),
                Instr(Opcode.ror, D),

                new MaskDecoder(4, 5, decoders94_8),
                new SparseDecoder(4, 5, decoders94_9),
                Instr(Opcode.dec, D),
                Instr(Opcode.des, i),

                Instr(Opcode.jmp, InstrClass.Transfer, Q),
                Instr(Opcode.jmp, InstrClass.Transfer, Q),
                Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Q),
                Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Q),
            };

            var decoders95 = new Decoder[]
            {
                Instr(Opcode.com, D),
                Instr(Opcode.neg, D),
                Instr(Opcode.swap, D),
                Instr(Opcode.inc, D),

                invalid,
                Instr(Opcode.asr, D),
                Instr(Opcode.lsr, D),
                Instr(Opcode.ror, D),

                new MaskDecoder(4, 5, decoders95_8),
                new SparseDecoder(4, 5, decoders95_9),
                Instr(Opcode.dec, D),
                invalid,

                Instr(Opcode.jmp, InstrClass.Transfer, Q),
                Instr(Opcode.jmp, InstrClass.Transfer, Q),
                Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Q),
                Instr(Opcode.call, InstrClass.Transfer|InstrClass.Call, Q),
            };

            var decoders9 = new Decoder[]
            {
                new MaskDecoder(0, 4, decoders90),
                new MaskDecoder(0, 4, decoders90),
                new MaskDecoder(0, 4, decoders92),
                new MaskDecoder(0, 4, decoders92),

                new MaskDecoder(0, 4, decoders94),
                new MaskDecoder(0, 4, decoders94),   //$TODO: may need a decoders95 for all the invalid des
                Instr(Opcode.adiw, q,s),
                Instr(Opcode.sbiw, q,s),

                invalid,
                invalid,
                invalid,
                Instr(Opcode.sbis, g,h),

                invalid,
                invalid,
                invalid,
                invalid,
            };

            var decodersB = new Decoder[]
            {
                Instr(Opcode.@in, D,A),
                Instr(Opcode.@out, A,D),
            };

            var decoders_sbrc = new Decoder[2]
            {
                Instr(Opcode.sbrc, D,I),
                invalid,
            };

            var decoders_sbrs = new Decoder[2]
            {
                Instr(Opcode.sbrs, D,I),
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

                new MaskDecoder(3, 1, decoders_sbrc),
                new MaskDecoder(3, 1, decoders_sbrc),
                new MaskDecoder(3, 1, decoders_sbrs),
                new MaskDecoder(3, 1, decoders_sbrs),
            };

            decoders = new Decoder[]
            {
                new MaskDecoder(8, 4, decoders0),
                new MaskDecoder(10, 2, decoders1),
                new MaskDecoder(10, 2, decoders2),
                Instr(Opcode.cpi, d,B),

                Instr(Opcode.sbci, d,B),
                Instr(Opcode.subi, d,B),
                Instr(Opcode.ori, d,B),
                Instr(Opcode.andi, d,B),

                new MaskDecoder(9, 3, decoders8),
                new MaskDecoder(8, 4, decoders9),
                invalid,
                new MaskDecoder(0xB, 1, decodersB),

                Instr(Opcode.rjmp, InstrClass.Transfer, J),
                Instr(Opcode.rcall, InstrClass.Transfer|InstrClass.Call, J),
                Instr(Opcode.ldi, d,K),
                new MaskDecoder(8, 4, decodersF),
            };
        }

        public abstract class Decoder
        {
            public abstract AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr);
        }

        public class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly Mutator<Avr8Disassembler>[] mutators;

            public InstrDecoder(Opcode opcode, InstrClass iclass, params Mutator<Avr8Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return invalid.Decode(dasm, wInstr);
                }
                return new AvrInstruction
                {
                    opcode = opcode,
                    InstructionClass = iclass,
                    operands = dasm.ops.ToArray(),
                };
            }
        }

        public class MaskDecoder : Decoder
        {
            private readonly int shift;
            private readonly int mask;
            private readonly Decoder[] decoders;

            public MaskDecoder(int shift, int length, Decoder[] decoders)
            {
                this.shift = shift;
                this.mask = (1 << length) - 1;
                this.decoders = decoders;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                int slot = (wInstr >> shift) & mask;
                return decoders[slot].Decode(dasm, wInstr);
            }
        }

        public class SparseDecoder : Decoder
        {
            private readonly int shift;
            private readonly int mask;
            private readonly Dictionary<int, Decoder> decoders;

            public SparseDecoder(int shift, int length, Dictionary<int, Decoder> decoders)
            {
                this.shift = shift;
                this.mask = (1 << length) - 1;
                this.decoders = decoders;
            }

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                int slot = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(slot, out Decoder decoder))
                {
                    return invalid.Decode(dasm, wInstr);
                }
                return decoder.Decode(dasm, wInstr);
            }
        }

        public class CondDecoder : Decoder
        {
            static readonly Opcode[] branches = new Opcode[]
            {
                Opcode.brcs, Opcode.breq, Opcode.brmi, Opcode.brvs,
                Opcode.brlt, Opcode.brhs, Opcode.brts, Opcode.brie,

                Opcode.brcc, Opcode.brne, Opcode.brpl, Opcode.brvc,
                Opcode.brge, Opcode.brhc, Opcode.brtc, Opcode.brid
            };

            public override AvrInstruction Decode(Avr8Disassembler dasm, ushort wInstr)
            {
                int br = (((wInstr >> 7) & 8) | (wInstr & 7)) & 0xF;
                o(wInstr, dasm);
                return new AvrInstruction
                {
                    InstructionClass = InstrClass.ConditionalTransfer,
                    opcode = branches[br],
                    operands = dasm.ops.ToArray()
                };
            }
        }
    }
}