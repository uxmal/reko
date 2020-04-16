#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.i8051
{
    using Decoder = Decoder<i8051Disassembler, Mnemonic, i8051Instruction>;

    // http://www.keil.com/support/man/docs/is51/is51_instructions.htm
    public class i8051Disassembler : DisassemblerBase<i8051Instruction, Mnemonic>
    {
        private readonly i8051Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public i8051Disassembler(i8051Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override i8051Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out var b))
                return null;
            ops.Clear();
            var instr = decoders[b].Decode(b, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - this.addr);
            return instr;
        }

        public override i8051Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new i8051Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override i8051Instruction CreateInvalidInstruction()
        {
            return new i8051Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        #region Mutators
        // An 11-bit address destination. This argument is used by ACALL and AJMP instructions. The target of the CALL or JMP must lie within the same 2K page as the first byte of the following instruction.
        private static bool j(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(AddressOperand.Ptr16(
                (ushort) (
                    (dasm.rdr.Address.ToLinear() & ~0x7Ful) |
                    (uInstr & 0xE0u) << 3 |
                    b)));
            return true;
        }

        // A 16-bit address destination. This argument is used by LCALL and LJMP instructions.
        private static bool J(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out var uAddr)) // Yes, big endian!
                return false;
            dasm.ops.Add(AddressOperand.Ptr16(uAddr));
            return true;
        }

        // A signed (two's complement) 8-bit offset (-128 to 127) relative to the first byte of the following instruction.
        private static bool o(uint uInstr, i8051Disassembler dasm) {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + (sbyte) b));
            return true;
        }

        // The accumulator.
        private static bool A(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.A));
            return true;
        }

        // An internal data RAM location (0-127) or SFR (128-255).
        private static bool d(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(MemoryOperand.Direct(Address.Ptr16(b)));
            return true;
        }

        // Register r0-r7
        private static bool r(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(dasm.Reg((int)uInstr & 7));
            return true;
        }

        // A direct addressed bit in internal data RAM or SFR memory.
        private static bool b(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(dasm.BitReg(b, false));
            return true;
        }

        // A direct addressed bit in internal data RAM or SFR memory.
        private static bool B(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(dasm.BitReg(b, true));
            return true;
        }

        // C flag of PSW
        private static bool C(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(new FlagGroupOperand(Registers.CFlag));
            return true;
        }

        // @DPTR 
        private static bool D(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Indirect(Registers.DPTR));
            return true;
        }

        // DPTR register pair
        private static bool p(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(new SequenceOperand(Registers.DPTR));
            return true;
        }

        // A constant included in the instruction encoding.
        private static bool i(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte b))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(b));
            return true;
        }

        // A constant included in the instruction encoding.
        private static bool I(uint uInstr, i8051Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out var w))
                return false;

            dasm.ops.Add(ImmediateOperand.Word16(w));
            return true;
        }

        // @Ri	An internal data RAM location (0-255) addressed indirectly through R0 or R1.
        private static bool Ind(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Indirect(Registers.GetRegister((int) uInstr & 1)));
            return true;
        }

        // case '+': // @A + DPTR:
        private static bool Adptr(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Indexed(Registers.DPTR, Registers.A));
            return true;
        }

        // @A + PC:
        private static bool P(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.Indexed(Registers.PC, Registers.A));
            return true;
        }

        // AB register pair
        private static bool AB(uint uInstr, i8051Disassembler dasm)
        {
            dasm.ops.Add(new SequenceOperand(Registers.AB));
            return true;
        }
        
        #endregion

        private RegisterOperand Reg(int r)
        {
            return new RegisterOperand(Registers.GetRegister(r));
        }

        private BitOperand BitReg(int b, bool neg)
        {
            var reg = Registers.GetRegister(b & 0xF0);
            return new BitOperand(reg, b & 0x7, neg);
        }

        private void EmitUnitTest(Mnemonic mnemonic, byte uInstr)
        {
            Debug.Print(
$@"    [Test]
        public void I8051_dis_{mnemonic}()
        {{
            var instr = DisassembleBytes(0x{uInstr:X2});
            Assert.AreEqual(""@@@"", instr.ToString());
        }}
");
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<i8051Disassembler>[] mutators)
        {
            return new InstrDecoder<i8051Disassembler, Mnemonic, i8051Instruction>(InstrClass.Linear, mnemonic, mutators);
            }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<i8051Disassembler>[] mutators)
            {
            return new InstrDecoder<i8051Disassembler, Mnemonic, i8051Instruction>(iclass, mnemonic, mutators);
                    }

        private static readonly Decoder[] decoders = new Decoder[256] {
        /*
/*00	1	*/ Instr(Mnemonic.nop), 
/*01	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j), // 	addr11
/*02	3	*/ Instr(Mnemonic.ljmp, InstrClass.Transfer, J), // 	addr16
/*03	1	*/ Instr(Mnemonic.rr, A), // 	A
/*04	1	*/ Instr(Mnemonic.inc, A), // 	A
/*05	2	*/ Instr(Mnemonic.inc, d),
/*06	1	*/ Instr(Mnemonic.inc, Ind), // 	@R0
/*07	1	*/ Instr(Mnemonic.inc, Ind), // 	@R1
/*08	1	*/ Instr(Mnemonic.inc, r), // 	R0
/*09	1	*/ Instr(Mnemonic.inc, r), // 	R1
/*0A	1	*/ Instr(Mnemonic.inc, r), // 	R2
/*0B	1	*/ Instr(Mnemonic.inc, r), // 	R3
/*0C	1	*/ Instr(Mnemonic.inc, r), // 	R4
/*0D	1	*/ Instr(Mnemonic.inc, r), // 	R5
/*0E	1	*/ Instr(Mnemonic.inc, r), // 	R6
/*0F	1	*/ Instr(Mnemonic.inc, r), // 	R7
/*10	3	*/ Instr(Mnemonic.jbc, InstrClass.ConditionalTransfer, b,o), // 	bit, offset
/*11	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j), // 	addr11
/*12	3	*/ Instr(Mnemonic.lcall, InstrClass.Transfer|InstrClass.Call, J), // 	addr16
/*13	1	*/ Instr(Mnemonic.rrc, A), // 	A
/*14	1	*/ Instr(Mnemonic.dec, A),
/*15	2	*/ Instr(Mnemonic.dec, d),
/*16	1	*/ Instr(Mnemonic.dec, Ind),
/*17	1	*/ Instr(Mnemonic.dec, Ind),
/*18	1	*/ Instr(Mnemonic.dec, r),
/*19	1	*/ Instr(Mnemonic.dec, r),
/*1A	1	*/ Instr(Mnemonic.dec, r),
/*1B	1	*/ Instr(Mnemonic.dec, r),
/*1C	1	*/ Instr(Mnemonic.dec, r),
/*1D	1	*/ Instr(Mnemonic.dec, r),
/*1E	1	*/ Instr(Mnemonic.dec, r),
/*1F	1	*/ Instr(Mnemonic.dec, r),

/*20	3	*/ Instr(Mnemonic.jb, InstrClass.ConditionalTransfer, b,o),
/*21	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j),
/*22	1	*/ Instr(Mnemonic.ret, InstrClass.Transfer),
/*23	1	*/ Instr(Mnemonic.rl, A),
/*24	2	*/ Instr(Mnemonic.add, A,i),
/*25	2	*/ Instr(Mnemonic.add, A,d),
/*26	1	*/ Instr(Mnemonic.add, A,Ind),
/*27	1	*/ Instr(Mnemonic.add, A,Ind),
/*28	1	*/ Instr(Mnemonic.add, A,r),
/*29	1	*/ Instr(Mnemonic.add, A,r),
/*2A	1	*/ Instr(Mnemonic.add, A,r),
/*2B	1	*/ Instr(Mnemonic.add, A,r),
/*2C	1	*/ Instr(Mnemonic.add, A,r),
/*2D	1	*/ Instr(Mnemonic.add, A,r),
/*2E	1	*/ Instr(Mnemonic.add, A,r),
/*2F	1	*/ Instr(Mnemonic.add, A,r),

/*30	3	*/ Instr(Mnemonic.jnb, InstrClass.ConditionalTransfer, b,o),
/*31	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j),
/*32	1	*/ Instr(Mnemonic.reti, InstrClass.Transfer),
/*33	1	*/ Instr(Mnemonic.rlc, A),
/*34	2	*/ Instr(Mnemonic.addc, A,i),
/*35	2	*/ Instr(Mnemonic.addc, A,d),
/*36	1	*/ Instr(Mnemonic.addc, A,Ind),
/*37	1	*/ Instr(Mnemonic.addc, A,Ind),
/*38	1	*/ Instr(Mnemonic.addc, A,r),
/*39	1	*/ Instr(Mnemonic.addc, A,r),
/*3A	1	*/ Instr(Mnemonic.addc, A,r),
/*3B	1	*/ Instr(Mnemonic.addc, A,r),
/*3C	1	*/ Instr(Mnemonic.addc, A,r),
/*3D	1	*/ Instr(Mnemonic.addc, A,r),
/*3E	1	*/ Instr(Mnemonic.addc, A,r),
/*3F	1	*/ Instr(Mnemonic.addc, A,r),

/*40	2	*/ Instr(Mnemonic.jc, InstrClass.ConditionalTransfer, o),
/*41	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j),
/*42	2	*/ Instr(Mnemonic.orl, d,A),
/*43	3	*/ Instr(Mnemonic.orl, d,i),
/*44	2	*/ Instr(Mnemonic.orl, A,i),
/*45	2	*/ Instr(Mnemonic.orl, A,d),
/*46	1	*/ Instr(Mnemonic.orl, A,Ind),
/*47	1	*/ Instr(Mnemonic.orl, A,Ind),
/*48	1	*/ Instr(Mnemonic.orl, A,r),
/*49	1	*/ Instr(Mnemonic.orl, A,r),
/*4A	1	*/ Instr(Mnemonic.orl, A,r),
/*4B	1	*/ Instr(Mnemonic.orl, A,r),
/*4C	1	*/ Instr(Mnemonic.orl, A,r),
/*4D	1	*/ Instr(Mnemonic.orl, A,r),
/*4E	1	*/ Instr(Mnemonic.orl, A,r),
/*4F	1	*/ Instr(Mnemonic.orl, A,r),

/*50	2	*/ Instr(Mnemonic.jnc, InstrClass.ConditionalTransfer, o),
/*51	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j),
/*52	2	*/ Instr(Mnemonic.anl, d,A),
/*53	3	*/ Instr(Mnemonic.anl, d,i),
/*54	2	*/ Instr(Mnemonic.anl, A,i),
/*55	2	*/ Instr(Mnemonic.anl, A,d),
/*56	1	*/ Instr(Mnemonic.anl, A,Ind),
/*57	1	*/ Instr(Mnemonic.anl, A,Ind),
/*58	1	*/ Instr(Mnemonic.anl, A,r),
/*59	1	*/ Instr(Mnemonic.anl, A,r),
/*5A	1	*/ Instr(Mnemonic.anl, A,r),
/*5B	1	*/ Instr(Mnemonic.anl, A,r),
/*5C	1	*/ Instr(Mnemonic.anl, A,r),
/*5D	1	*/ Instr(Mnemonic.anl, A,r),
/*5E	1	*/ Instr(Mnemonic.anl, A,r),
/*5F	1	*/ Instr(Mnemonic.anl, A,r),

/*60	2	*/ Instr(Mnemonic.jz, InstrClass.ConditionalTransfer,  o),
/*61	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j),
/*62	2	*/ Instr(Mnemonic.xrl, d,A),
/*63	3	*/ Instr(Mnemonic.xrl, d,i),
/*64	2	*/ Instr(Mnemonic.xrl, A,i),
/*65	2	*/ Instr(Mnemonic.xrl, A,d),
/*66	1	*/ Instr(Mnemonic.xrl, A,Ind),
/*67	1	*/ Instr(Mnemonic.xrl, A,Ind),
/*68	1	*/ Instr(Mnemonic.xrl, A,r),
/*69	1	*/ Instr(Mnemonic.xrl, A,r),
/*6A	1	*/ Instr(Mnemonic.xrl, A,r),
/*6B	1	*/ Instr(Mnemonic.xrl, A,r),
/*6C	1	*/ Instr(Mnemonic.xrl, A,r),
/*6D	1	*/ Instr(Mnemonic.xrl, A,r),
/*6E	1	*/ Instr(Mnemonic.xrl, A,r),
/*6F	1	*/ Instr(Mnemonic.xrl, A,r),

/*70	2	*/ Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, o),
/*71	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j),
/*72	2	*/ Instr(Mnemonic.orl, C,b),
/*73	1	*/ Instr(Mnemonic.jmp, InstrClass.Transfer, Adptr), // 	@A+DPTR
/*74	2	*/ Instr(Mnemonic.mov, A,i), // 	A, #immed
/*75	3	*/ Instr(Mnemonic.mov, d,i), // 	direct, #immed
/*76	2	*/ Instr(Mnemonic.mov, Ind,i), // 	@R0, #immed
/*77	2	*/ Instr(Mnemonic.mov, Ind,i), // 	@R1, #immed
/*78	2	*/ Instr(Mnemonic.mov, r,i), // 	R0, #immed
/*79	2	*/ Instr(Mnemonic.mov, r,i), // 	R1, #immed
/*7A	2	*/ Instr(Mnemonic.mov, r,i), // 	R2, #immed
/*7B	2	*/ Instr(Mnemonic.mov, r,i), // 	R3, #immed
/*7C	2	*/ Instr(Mnemonic.mov, r,i), // 	R4, #immed
/*7D	2	*/ Instr(Mnemonic.mov, r,i), // 	R5, #immed
/*7E	2	*/ Instr(Mnemonic.mov, r,i), // 	R6, #immed
/*7F	2	*/ Instr(Mnemonic.mov, r,i), // 	R7, #immed	 	

/*80	2	*/ Instr(Mnemonic.sjmp, InstrClass.Transfer, o), // 	offset
/*81	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j), // 	addr11
/*82	2	*/ Instr(Mnemonic.anl, C,b), // 	C, bit
/*83	1	*/ Instr(Mnemonic.movc, P), // 	A, IndA+PC
/*84	1	*/ Instr(Mnemonic.div, AB), // 	AB
/*85	3	*/ Instr(Mnemonic.mov, d,d), // 	direct, direct
/*86	2	*/ Instr(Mnemonic.mov, d,Ind), // 	direct, @R0
/*87	2	*/ Instr(Mnemonic.mov, d,Ind), // 	direct, @R1
/*88	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R0
/*89	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R1
/*8A	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R2
/*8B	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R3
/*8C	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R4
/*8D	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R5
/*8E	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R6
/*8F	2	*/ Instr(Mnemonic.mov, d,r), // 	direct, R7

/*90	3	*/ Instr(Mnemonic.mov, p,I), // 	DPTR, #immed
/*91	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j), // 	addr11
/*92	2	*/ Instr(Mnemonic.mov, b,C), // 	bit, C
/*93	1	*/ Instr(Mnemonic.movc, Adptr), // 	A, IndA+DPTR
/*94	2	*/ Instr(Mnemonic.subb, A,i), // 	A, #immed
/*95	2	*/ Instr(Mnemonic.subb, A,d), // 	A, direct
/*96	1	*/ Instr(Mnemonic.subb, A,Ind), // 	A, @R0
/*97	1	*/ Instr(Mnemonic.subb, A,Ind), // 	A, @R1
/*98	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R0
/*99	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R1
/*9A	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R2
/*9B	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R3
/*9C	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R4
/*9D	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R5
/*9E	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R6
/*9F	1	*/ Instr(Mnemonic.subb, A,r), // 	A, R7

/*A0	2	*/ Instr(Mnemonic.orl, C,B), // 	C, /bit
/*A1	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j), // 	addr11
/*A2	2	*/ Instr(Mnemonic.mov, C,b), // 	C, bit
/*A3	1	*/ Instr(Mnemonic.inc, p), // 	DPTR
/*A4	1	*/ Instr(Mnemonic.mul, AB), // 	AB
/*A5	 	*/ Instr(Mnemonic.reserved), // 	 
/*A6	2	*/ Instr(Mnemonic.mov, Ind,d), // 	@R0, direct
/*A7	2	*/ Instr(Mnemonic.mov, Ind,d), // 	@R1, direct
/*A8	2	*/ Instr(Mnemonic.mov, r,d), // 	R0, direct
/*A9	2	*/ Instr(Mnemonic.mov, r,d), // 	R1, direct
/*AA	2	*/ Instr(Mnemonic.mov, r,d), // 	R2, direct
/*AB	2	*/ Instr(Mnemonic.mov, r,d), // 	R3, direct
/*AC	2	*/ Instr(Mnemonic.mov, r,d), // 	R4, direct
/*AD	2	*/ Instr(Mnemonic.mov, r,d), // 	R5, direct
/*AE	2	*/ Instr(Mnemonic.mov, r,d), // 	R6, direct
/*AF	2	*/ Instr(Mnemonic.mov, r,d), // 	R7, direct

/*B0	2	*/ Instr(Mnemonic.anl, C,B), // 	C, /bit
/*B1	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j), // 	addr11
/*B2	2	*/ Instr(Mnemonic.cpl, B), // 	bit
/*B3	1	*/ Instr(Mnemonic.cpl, C), // 	C
/*B4	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, A,i,o), // 	A, #immed, offset
/*B5	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, A,i,o), // 	A, direct, offset
/*B6	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, Ind,i,o), // 	@R0, #immed, offset
/*B7	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, Ind,i,o), // 	@R1, #immed, offset
/*B8	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R0, #immed, offset
/*B9	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R1, #immed, offset
/*BA	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R2, #immed, offset
/*BB	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R3, #immed, offset
/*BC	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R4, #immed, offset
/*BD	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R5, #immed, offset
/*BE	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R6, #immed, offset
/*BF	3	*/ Instr(Mnemonic.cjne, InstrClass.ConditionalTransfer, r,i,o), // 	R7, #immed, offset

/*C0	2	*/ Instr(Mnemonic.push, d), // 	direct
/*C1	2	*/ Instr(Mnemonic.ajmp, InstrClass.Transfer, j), // 	addr11
/*C2	2	*/ Instr(Mnemonic.clr, b), // 	bit
/*C3	1	*/ Instr(Mnemonic.clr, C), // 	C
/*C4	1	*/ Instr(Mnemonic.swap, A), // 	A
/*C5	2	*/ Instr(Mnemonic.xch, A,d), // 	A, direct
/*C6	1	*/ Instr(Mnemonic.xch, A,Ind), // 	A, @R0
/*C7	1	*/ Instr(Mnemonic.xch, A,Ind), // 	A, @R1
/*C8	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R0
/*C9	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R1
/*CA	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R2
/*CB	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R3
/*CC	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R4
/*CD	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R5
/*CE	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R6
/*CF	1	*/ Instr(Mnemonic.xch, A,r), // 	A, R7

/*D0	2	*/ Instr(Mnemonic.pop, d), // 	direct
/*D1	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j), // 	addr11
/*D2	2	*/ Instr(Mnemonic.setb, b), // 	bit
/*D3	1	*/ Instr(Mnemonic.setb, C), // 	C
/*D4	1	*/ Instr(Mnemonic.da, A), // 	A
/*D5	3	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, d,o), // 	direct, offset
/*D6	1	*/ Instr(Mnemonic.xchd, A,Ind), // 	A, @R0
/*D7	1	*/ Instr(Mnemonic.xchd, A,Ind), // 	A, @R1
/*D8	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R0, offset
/*D9	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R1, offset
/*DA	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R2, offset
/*DB	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R3, offset
/*DC	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R4, offset
/*DD	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R5, offset
/*DE	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R6, offset
/*DF	2	*/ Instr(Mnemonic.djnz, InstrClass.ConditionalTransfer, r,o), // 	R7, offset

/*E0	1	*/ Instr(Mnemonic.movx, A,D), // 	A, @DPTR
/*E1	2	*/ Instr(Mnemonic.ajmp,  InstrClass.Transfer, j), // 	addr11
/*E2	1	*/ Instr(Mnemonic.movx, A,Ind), // 	A, @R0
/*E3	1	*/ Instr(Mnemonic.movx, A,Ind), // 	A, @R1
/*E4	1	*/ Instr(Mnemonic.clr, A), // 	A
/*E5	2	*/ Instr(Mnemonic.mov, A,d), // 	A, direct
/*E6	1	*/ Instr(Mnemonic.mov, A,Ind), // 	A, @R0
/*E7	1	*/ Instr(Mnemonic.mov, A,Ind), // 	A, @R1
/*E8	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R0
/*E9	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R1
/*EA	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R2
/*EB	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R3
/*EC	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R4
/*ED	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R5
/*EE	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R6
/*EF	1	*/ Instr(Mnemonic.mov, A,r), // 	A, R7

/*F0	1	*/ Instr(Mnemonic.movx, D,A), // 	@DPTR, A
/*F1	2	*/ Instr(Mnemonic.acall, InstrClass.Transfer|InstrClass.Call, j), // 	addr11
/*F2	1	*/ Instr(Mnemonic.movx, Ind,A), // 	@R0, A
/*F3	1	*/ Instr(Mnemonic.movx, Ind,A), // 	@R1, A
/*F4	1	*/ Instr(Mnemonic.cpl, A), // 	A
/*F5	2	*/ Instr(Mnemonic.mov, d,A), // 	direct, A
/*F6	1	*/ Instr(Mnemonic.mov, Ind,A), // 	@R0, A
/*F7	1	*/ Instr(Mnemonic.mov, Ind,A), // 	@R1, A
/*F8	1	*/ Instr(Mnemonic.mov, r,A), // 	R0, A
/*F9	1	*/ Instr(Mnemonic.mov, r,A), // 	R1, A
/*FA	1	*/ Instr(Mnemonic.mov, r,A), // 	R2, A
/*FB	1	*/ Instr(Mnemonic.mov, r,A), // 	R3, A
/*FC	1	*/ Instr(Mnemonic.mov, r,A), // 	R4, A
/*FD	1	*/ Instr(Mnemonic.mov, r,A), // 	R5, A
/*FE	1	*/ Instr(Mnemonic.mov, r,A), // 	R6, A
/*FF	1	*/ Instr(Mnemonic.mov, r,A), // 	R7, A

        };
    }
}