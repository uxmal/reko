#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    using Mutator = Func<byte, M6812Disassembler, bool>;

    public class M6812Disassembler : DisassemblerBase<M6812Instruction>
    {
        private readonly static Decoder[] decoders;
        private readonly static Decoder[] decodersSecondByte;
        private readonly static RegisterStorage[] IndexRegisters;
        private readonly static HashSet<byte> seen = new HashSet<byte>();

        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> operands;

        public M6812Disassembler(EndianImageReader rdr)
        {
            this.rdr = rdr;
            this.operands = new List<MachineOperand>();
        }

        public override M6812Instruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadByte(out var bInstr))
                return null;
            operands.Clear();
            M6812Instruction instr = decoders[bInstr].Decode(bInstr, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        private M6812Instruction Invalid()
        {
            return new M6812Instruction
            {
                Opcode = Opcode.invalid,
                Operands = new MachineOperand[0]
            };
        }

        public abstract class Decoder
        {
            public abstract M6812Instruction Decode(byte bInstr, M6812Disassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly Mutator[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator[] mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override M6812Instruction Decode(byte bInstr, M6812Disassembler dasm)
            {
                foreach (var mutator in mutators)
                {
                    if (!mutator(bInstr, dasm))
                        return dasm.Invalid();
                }
                return new M6812Instruction
                {
                    Opcode = this.opcode,
                    Operands = dasm.operands.ToArray()
                };
            }
        }

        public class NextByteDecoder : Decoder
        {
            public override M6812Instruction Decode(byte bInstr, M6812Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out bInstr))
                    return dasm.Invalid();
                return decodersSecondByte[bInstr].Decode(bInstr, dasm);
            }
        }


        private static Decoder Instr(Opcode opcode, params Mutator [] mutators)
        {
            return new InstrDecoder(opcode, mutators);
        }


        private static Decoder Nyi(string message)
        {
            return new InstrDecoder(Opcode.invalid, NotYetImplemented);
        }

        private static bool I(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm))
                return false;
            dasm.operands.Add(ImmediateOperand.Byte(imm));
            return true;
        }

        private static bool JK(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var imm))
                return false;
            dasm.operands.Add(ImmediateOperand.Word16(imm));
            return true;
        }

        private static bool D(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm))
                return false;
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Offset = imm
            };
            return true;
        }


        private static bool HL(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeInt16(out var imm))
                return false;
            var mem = new MemoryOperand(PrimitiveType.Byte)
            {
                Offset = imm
            };
            dasm.operands.Add(mem);
            return true;
        }

        public static bool PostByte(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out bInstr))
                return false;
            switch ((bInstr >> 5) & 7)
            {
            case 0:
            case 2:
            case 4:
            case 6:
                var idxReg = IndexRegisters[(bInstr >> 6) & 3];
                var offset = (short)Bits.SignExtend(bInstr, 5);
                var mem = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = idxReg,
                    Offset = offset
                };
                dasm.operands.Add(mem);
                return true;
            case 1:
            case 3:
            case 5:
            default:
                Debug.Assert(false, "not implemented yet!");
                return false;
            }
        }

        private static bool R(byte bInstr, M6812Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var rel))
                return false;
            var addrDst = dasm.rdr.Address + (sbyte)rel;
            dasm.operands.Add(AddressOperand.Create(addrDst));
            return true;
        }

        private static bool NotYetImplemented(byte bInstr, M6812Disassembler dasm)
        {
            if (!seen.Contains(bInstr))
            {
                seen.Add(bInstr);
                Debug.Print("// An M6812 decoder for instruction {0:X2} has not been implemented.", bInstr);
                Debug.Print("[Test]");
                Debug.Print("public void M6812Dis_{0:X2}()", bInstr);
                Debug.Print("{");
                Debug.Print("    Given_Code(\"{0:X2}\");", bInstr);
                Debug.Print("    Expect_Instruction(\"@@@\");");
                Debug.Print("}");
                Debug.WriteLine("");
            }
            return true;
        }

        static M6812Disassembler()
        {
            IndexRegisters = new RegisterStorage[4]
            {
                Registers.x,
                Registers.y,
                Registers.sp,
                Registers.pc
            };

            var invalid = Instr(Opcode.invalid);

            decoders = new Decoder[256]
            {
                // 00
                Instr(Opcode.bgnd),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.bclr, HL,PostByte),
                Nyi(""),
                Nyi(""),
                // 10
                Instr(Opcode.andcc, I),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.jsr, PostByte),
                Instr(Opcode.jsr, HL),
                Instr(Opcode.jsr, D),

                new NextByteDecoder(),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.bclr, HL,I),
                Nyi(""),
                Instr(Opcode.brclr, HL, I, R),
                // 20
                Nyi(""),
                Nyi(""),
                Instr(Opcode.bhi, R),
                Nyi(""),

                Instr(Opcode.bcc, R),
                Instr(Opcode.bcs, R),
                Instr(Opcode.bne, R),
                Instr(Opcode.beq, R),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Instr(Opcode.bge, R),
                Nyi(""),
                Instr(Opcode.bgt, R),
                Nyi(""),
                // 30
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.pshd),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 40
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.asra),

                Instr(Opcode.asla),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.bclr, D,I),
                Nyi(""),
                Nyi(""),
                // 50
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.asrb),

                Instr(Opcode.aslb),
                Instr(Opcode.asld),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 60
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.asr, PostByte),

                Instr(Opcode.asl, PostByte),
                Instr(Opcode.clr, PostByte),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 70
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.asr, HL),

                Instr(Opcode.asl, HL),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 80
                Instr(Opcode.suba, I),
                Instr(Opcode.cmpa, I),
                Nyi(""),
                Nyi(""),

                Instr(Opcode.anda, I),
                Instr(Opcode.bita, I),
                Instr(Opcode.ldaa, I),
                Instr(Opcode.clra),

                Nyi(""),
                Instr(Opcode.adca, I),
                Nyi(""),
                Instr(Opcode.adda, I),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 90
                Instr(Opcode.suba, D),
                Instr(Opcode.cmpa, D),
                Instr(Opcode.sbca, D),
                Instr(Opcode.subd, D),

                Instr(Opcode.anda, D),
                Instr(Opcode.bita, D),
                Instr(Opcode.ldaa, D),
                Nyi(""),

                Instr(Opcode.eora, D),
                Instr(Opcode.adca, D),
                Instr(Opcode.oraa, D),
                Instr(Opcode.adda, D),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // A0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Instr(Opcode.anda, PostByte),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.adca, PostByte),
                Nyi(""),
                Instr(Opcode.adda, PostByte),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // B0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.anda, HL),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.adca, HL),
                Nyi(""),
                Instr(Opcode.adda, HL),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // C0
                Instr(Opcode.subb, I),
                Instr(Opcode.cmpb, I),
                Instr(Opcode.sbcb, I),
                Instr(Opcode.addd, JK),

                Instr(Opcode.andb, I),
                Nyi(""),
                Instr(Opcode.ldab, I),
                Instr(Opcode.clrb),

                Instr(Opcode.eorb, I),
                Instr(Opcode.adcb, I),
                Instr(Opcode.orab, I),
                Instr(Opcode.addb, I),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // D0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.addd, D),

                Instr(Opcode.andb, D),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.adcb, D),
                Nyi(""),
                Instr(Opcode.addb, D),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // E0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.addb, PostByte),

                Instr(Opcode.andb, PostByte),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.adcb, PostByte),
                Nyi(""),
                Instr(Opcode.addb, PostByte),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // F0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Instr(Opcode.addd, HL),

                Instr(Opcode.andb, HL),
                Nyi(""),
                Instr(Opcode.ldab, HL),
                Nyi(""),

                Nyi(""),
                Instr(Opcode.adcb, HL),
                Nyi(""),
                Instr(Opcode.addb, HL),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),


            };

            decodersSecondByte = new Decoder[256]
            {
                // 00
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Instr(Opcode.aba),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 10
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 20
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 30
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 40
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 50
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 60
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 70
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 80
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // 90
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // A0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // B0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // C0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // D0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // E0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),
                // F0
                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),

                Nyi(""),
                Nyi(""),
                Nyi(""),
                Nyi(""),


            };

        }

        private static Decoder Instr(object brclr, Mutator hL, Mutator i, Mutator r)
        {
            throw new NotImplementedException();
        }
    }
}
