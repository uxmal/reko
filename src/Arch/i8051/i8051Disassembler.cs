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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;

namespace Reko.Arch.i8051
{
    // http://www.keil.com/support/man/docs/is51/is51_instructions.htm
    public class i8051Disassembler : Core.DisassemblerBase<i8051Instruction>
    {
        private i8051Architecture arch;
        private EndianImageReader rdr;
        private Address addr;

        public i8051Disassembler(i8051Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override i8051Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out var b))
                return null;
            return decoders[b].Decode(b, this);
        }

        private i8051Instruction Decode(Opcode opcode, byte uInstr, string fmt)
        {
            byte b;
            var ops = new List<MachineOperand>();
            foreach (var ch in fmt)
            {
                switch (ch)
                {
                case ',':
                    continue;
                case 'j': // An 11-bit address destination. This argument is used by ACALL and AJMP instructions. The target of the CALL or JMP must lie within the same 2K page as the first byte of the following instruction.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(AddressOperand.Ptr16(
                        (ushort)(
                            (rdr.Address.ToLinear() & ~0x7Ful) |
                            (uInstr & 0xE0u) << 3 |
                            b)));
                    break;
                case 'J': // A 16-bit address destination. This argument is used by LCALL and LJMP instructions.
                    if (!rdr.TryReadBeUInt16(out var uAddr)) // Yes, big endian!
                        return null;
                    ops.Add(AddressOperand.Ptr16(uAddr));
                    break;
                case 'o': // A signed (two's complement) 8-bit offset (-128 to 127) relative to the first byte of the following instruction.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(AddressOperand.Create(rdr.Address + (sbyte)b));
                    break;
                case 'A': // The accumulator.
                    ops.Add(new RegisterOperand(Registers.A));
                    break;
                case 'd': // An internal data RAM location (0-127) or SFR (128-255).
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(MemoryOperand.Direct(Address.Ptr16(b)));
                    break;
                case 'r': // Register r0-r7
                    ops.Add(Reg(uInstr&7));
                    break;
                case 'b': // A direct addressed bit in internal data RAM or SFR memory.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(BitReg(b, false));
                    break;
                case 'B': // A direct addressed bit in internal data RAM or SFR memory.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(BitReg(b, true));
                    break;
                case 'C':   // C flag of PSW
                    ops.Add(new FlagGroupOperand(arch.GetFlagGroup(Registers.PSW, (uint)FlagM.C)));
                    break;
                case 'D':   // @DPTR 
                    ops.Add(MemoryOperand.Indirect(Registers.DPTR));
                    break;
                case 'p':   // DPTR register pair
                    ops.Add(new SequenceOperand(Registers.DPTR));
                    break;
                case 'i': // A constant included in the instruction encoding.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    ops.Add(ImmediateOperand.Byte(b));
                    break;
                case 'I': // A constant included in the instruction encoding.
                    if (!rdr.TryReadUInt16(out var w))
                        return null;
                    ops.Add(ImmediateOperand.Word16(w));
                    break;
                case '@': // @Ri	An internal data RAM location (0-255) addressed indirectly through R0 or R1.
                    ops.Add(MemoryOperand.Indirect(Registers.GetRegister(uInstr & 1)));
                    break;
                case '+': // @A + DPTR:
                    ops.Add(MemoryOperand.Indexed(Registers.DPTR, Registers.A));
                    break;
                case 'P': // @A + PC:
                    ops.Add(MemoryOperand.Indexed(Registers.PC, Registers.A));
                    break;

                case '*': // AB register pair
                    ops.Add(new SequenceOperand(Registers.AB));
                    break;
                default:
                    EmitUnitTest(opcode, uInstr);
                    break;
                }
            }

            return new i8051Instruction
            {
                Opcode = opcode,
                Address = this.addr,
                Length = (int)(rdr.Address - this.addr),
                Operand1 = ops.Count >= 1 ? ops[0] : null,
                Operand2 = ops.Count >= 2 ? ops[1] : null,
                Operand3 = ops.Count >= 3 ? ops[2] : null,
            };
        }

        private RegisterOperand Reg(int r)
        {
            return new RegisterOperand(Registers.GetRegister(r));
        }

        private BitOperand BitReg(int b, bool neg)
        {
            var reg = Registers.GetRegister(b & 0xF0);
            return new BitOperand(reg, b & 0x7, neg);
        }

        private void EmitUnitTest(Opcode opcode, byte uInstr)
        {
            Debug.Print(
$@"    [Test]
        public void I8051_dis_{opcode}()
        {{
            var instr = DisassembleBytes(0x{uInstr:X2});
            Assert.AreEqual(""@@@"", instr.ToString());
        }}
");
        }

        private abstract class Decoder
        {
            public abstract i8051Instruction Decode(byte op, i8051Disassembler dasm);
        }


        private class ByteDecoder : Decoder
        {
            private Opcode opcode;
            private string fmt;

            public ByteDecoder(Opcode opcode, string fmt)
            {
                this.opcode = opcode;
                this.fmt = fmt;
            }

            public override i8051Instruction Decode(byte op, i8051Disassembler dasm)
            {
                return dasm.Decode(opcode, op, fmt);
            }
        }

        private static readonly Decoder[] decoders = new Decoder[256] {
        /*
/*00	1	*/ new ByteDecoder(Opcode.nop, ""), 
/*01	2	*/ new ByteDecoder(Opcode.ajmp, "j"), // 	addr11
/*02	3	*/ new ByteDecoder(Opcode.ljmp, "J"), // 	addr16
/*03	1	*/ new ByteDecoder(Opcode.rr, "A"), // 	A
/*04	1	*/ new ByteDecoder(Opcode.inc, "A"), // 	A
/*05	2	*/ new ByteDecoder(Opcode.inc, "d"),
/*06	1	*/ new ByteDecoder(Opcode.inc, "@"), // 	@R0
/*07	1	*/ new ByteDecoder(Opcode.inc, "@"), // 	@R1
/*08	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R0
/*09	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R1
/*0A	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R2
/*0B	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R3
/*0C	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R4
/*0D	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R5
/*0E	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R6
/*0F	1	*/ new ByteDecoder(Opcode.inc, "r"), // 	R7
/*10	3	*/ new ByteDecoder(Opcode.jbc, "b,o"), // 	bit, offset
/*11	2	*/ new ByteDecoder(Opcode.acall, "j"), // 	addr11
/*12	3	*/ new ByteDecoder(Opcode.lcall, "J"), // 	addr16
/*13	1	*/ new ByteDecoder(Opcode.rrc, "A"), // 	A
/*14	1	*/ new ByteDecoder(Opcode.dec, "A"),
/*15	2	*/ new ByteDecoder(Opcode.dec, "d"),
/*16	1	*/ new ByteDecoder(Opcode.dec, "@"),
/*17	1	*/ new ByteDecoder(Opcode.dec, "@"),
/*18	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*19	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1A	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1B	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1C	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1D	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1E	1	*/ new ByteDecoder(Opcode.dec, "r"),
/*1F	1	*/ new ByteDecoder(Opcode.dec, "r"), 
/*20	3	*/ new ByteDecoder(Opcode.jb, "b,o"),
/*21	2	*/ new ByteDecoder(Opcode.ajmp, "j"),
/*22	1	*/ new ByteDecoder(Opcode.ret, ""),
/*23	1	*/ new ByteDecoder(Opcode.rl, "A"),
/*24	2	*/ new ByteDecoder(Opcode.add, "A,i"),
/*25	2	*/ new ByteDecoder(Opcode.add, "A,d"),
/*26	1	*/ new ByteDecoder(Opcode.add, "A,@"),
/*27	1	*/ new ByteDecoder(Opcode.add, "A,@"),
/*28	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*29	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2A	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2B	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2C	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2D	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2E	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*2F	1	*/ new ByteDecoder(Opcode.add, "A,r"),
/*30	3	*/ new ByteDecoder(Opcode.jnb, "b,o"),
/*31	2	*/ new ByteDecoder(Opcode.acall, "j"),
/*32	1	*/ new ByteDecoder(Opcode.reti, ""),
/*33	1	*/ new ByteDecoder(Opcode.rlc, "A"),
/*34	2	*/ new ByteDecoder(Opcode.addc, "A,i"),
/*35	2	*/ new ByteDecoder(Opcode.addc, "A,d"),
/*36	1	*/ new ByteDecoder(Opcode.addc, "A,@"),
/*37	1	*/ new ByteDecoder(Opcode.addc, "A.@"),
/*38	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*39	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3A	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3B	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3C	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3D	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3E	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*3F	1	*/ new ByteDecoder(Opcode.addc, "A,r"),
/*40	2	*/ new ByteDecoder(Opcode.jc, "o"),
/*41	2	*/ new ByteDecoder(Opcode.ajmp, "j"),
/*42	2	*/ new ByteDecoder(Opcode.orl, "d,A"),
/*43	3	*/ new ByteDecoder(Opcode.orl, "d,i"),
/*44	2	*/ new ByteDecoder(Opcode.orl, "A,i"),
/*45	2	*/ new ByteDecoder(Opcode.orl, "A,d"),
/*46	1	*/ new ByteDecoder(Opcode.orl, "A,@"),
/*47	1	*/ new ByteDecoder(Opcode.orl, "A,@"),
/*48	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*49	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4A	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4B	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4C	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4D	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4E	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*4F	1	*/ new ByteDecoder(Opcode.orl, "A,r"),
/*50	2	*/ new ByteDecoder(Opcode.jnc, "o"),
/*51	2	*/ new ByteDecoder(Opcode.acall, "j"),
/*52	2	*/ new ByteDecoder(Opcode.anl, "d,A"),
/*53	3	*/ new ByteDecoder(Opcode.anl, "d,i"),
/*54	2	*/ new ByteDecoder(Opcode.anl, "A,i"),
/*55	2	*/ new ByteDecoder(Opcode.anl, "A,d"),
/*56	1	*/ new ByteDecoder(Opcode.anl, "A,@"),
/*57	1	*/ new ByteDecoder(Opcode.anl, "A,@"),
/*58	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*59	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5A	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5B	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5C	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5D	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5E	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*5F	1	*/ new ByteDecoder(Opcode.anl, "A,r"),
/*60	2	*/ new ByteDecoder(Opcode.jz, "o"),
/*61	2	*/ new ByteDecoder(Opcode.ajmp, "j"),
/*62	2	*/ new ByteDecoder(Opcode.xrl, "d,A"),
/*63	3	*/ new ByteDecoder(Opcode.xrl, "d,i"),
/*64	2	*/ new ByteDecoder(Opcode.xrl, "A,i"),
/*65	2	*/ new ByteDecoder(Opcode.xrl, "A,d"),
/*66	1	*/ new ByteDecoder(Opcode.xrl, "A,@"),
/*67	1	*/ new ByteDecoder(Opcode.xrl, "A,@"),
/*68	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*69	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6A	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6B	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6C	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6D	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6E	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*6F	1	*/ new ByteDecoder(Opcode.xrl, "A,r"),
/*70	2	*/ new ByteDecoder(Opcode.jnz, "o"),
/*71	2	*/ new ByteDecoder(Opcode.acall, "j"),
/*72	2	*/ new ByteDecoder(Opcode.orl, "C,b"),
/*73	1	*/ new ByteDecoder(Opcode.jmp, "+"), // 	@A+DPTR
/*74	2	*/ new ByteDecoder(Opcode.mov, "A,i"), // 	A, #immed
/*75	3	*/ new ByteDecoder(Opcode.mov, "d,i"), // 	direct, #immed
/*76	2	*/ new ByteDecoder(Opcode.mov, "@,i"), // 	@R0, #immed
/*77	2	*/ new ByteDecoder(Opcode.mov, "@,i"), // 	@R1, #immed
/*78	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R0, #immed
/*79	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R1, #immed
/*7A	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R2, #immed
/*7B	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R3, #immed
/*7C	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R4, #immed
/*7D	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R5, #immed
/*7E	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R6, #immed
/*7F	2	*/ new ByteDecoder(Opcode.mov, "r,i"), // 	R7, #immed	 	

/*80	2	*/ new ByteDecoder(Opcode.sjmp, "o"), // 	offset
/*81	2	*/ new ByteDecoder(Opcode.ajmp, "j"), // 	addr11
/*82	2	*/ new ByteDecoder(Opcode.anl, "C,b"), // 	C, bit
/*83	1	*/ new ByteDecoder(Opcode.movc, "P"), // 	A, @A+PC
/*84	1	*/ new ByteDecoder(Opcode.div, "X"), // 	AB
/*85	3	*/ new ByteDecoder(Opcode.mov, "d,d"), // 	direct, direct
/*86	2	*/ new ByteDecoder(Opcode.mov, "d,@"), // 	direct, @R0
/*87	2	*/ new ByteDecoder(Opcode.mov, "d,@"), // 	direct, @R1
/*88	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R0
/*89	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R1
/*8A	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R2
/*8B	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R3
/*8C	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R4
/*8D	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R5
/*8E	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R6
/*8F	2	*/ new ByteDecoder(Opcode.mov, "d,r"), // 	direct, R7
/*90	3	*/ new ByteDecoder(Opcode.mov, "p,I"), // 	DPTR, #immed
/*91	2	*/ new ByteDecoder(Opcode.acall, "j"), // 	addr11
/*92	2	*/ new ByteDecoder(Opcode.mov, "b,C"), // 	bit, C
/*93	1	*/ new ByteDecoder(Opcode.movc, "+"), // 	A, @A+DPTR
/*94	2	*/ new ByteDecoder(Opcode.subb, "A,i"), // 	A, #immed
/*95	2	*/ new ByteDecoder(Opcode.subb, "A,d"), // 	A, direct
/*96	1	*/ new ByteDecoder(Opcode.subb, "A,@"), // 	A, @R0
/*97	1	*/ new ByteDecoder(Opcode.subb, "A,@"), // 	A, @R1
/*98	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R0
/*99	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R1
/*9A	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R2
/*9B	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R3
/*9C	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R4
/*9D	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R5
/*9E	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R6
/*9F	1	*/ new ByteDecoder(Opcode.subb, "A,r"), // 	A, R7
/*A0	2	*/ new ByteDecoder(Opcode.orl, "C,B"), // 	C, /bit
/*A1	2	*/ new ByteDecoder(Opcode.ajmp, "j"), // 	addr11
/*A2	2	*/ new ByteDecoder(Opcode.mov, "C,b"), // 	C, bit
/*A3	1	*/ new ByteDecoder(Opcode.inc, "p"), // 	DPTR
/*A4	1	*/ new ByteDecoder(Opcode.mul, "*"), // 	AB
/*A5	 	*/ new ByteDecoder(Opcode.reserved, ""), // 	 
/*A6	2	*/ new ByteDecoder(Opcode.mov, "@,d"), // 	@R0, direct
/*A7	2	*/ new ByteDecoder(Opcode.mov, "@,d"), // 	@R1, direct
/*A8	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R0, direct
/*A9	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R1, direct
/*AA	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R2, direct
/*AB	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R3, direct
/*AC	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R4, direct
/*AD	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R5, direct
/*AE	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R6, direct
/*AF	2	*/ new ByteDecoder(Opcode.mov, "r,d"), // 	R7, direct
/*B0	2	*/ new ByteDecoder(Opcode.anl, "C,B"), // 	C, /bit
/*B1	2	*/ new ByteDecoder(Opcode.acall, "j"), // 	addr11
/*B2	2	*/ new ByteDecoder(Opcode.cpl, "X"), // 	bit
/*B3	1	*/ new ByteDecoder(Opcode.cpl, "X"), // 	C
/*B4	3	*/ new ByteDecoder(Opcode.cjne, "A,i,o"), // 	A, #immed, offset
/*B5	3	*/ new ByteDecoder(Opcode.cjne, "A,i,o"), // 	A, direct, offset
/*B6	3	*/ new ByteDecoder(Opcode.cjne, "@,i,o"), // 	@R0, #immed, offset
/*B7	3	*/ new ByteDecoder(Opcode.cjne, "@,i,o"), // 	@R1, #immed, offset
/*B8	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R0, #immed, offset
/*B9	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R1, #immed, offset
/*BA	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R2, #immed, offset
/*BB	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R3, #immed, offset
/*BC	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R4, #immed, offset
/*BD	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R5, #immed, offset
/*BE	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R6, #immed, offset
/*BF	3	*/ new ByteDecoder(Opcode.cjne, "r,i,o"), // 	R7, #immed, offset
/*C0	2	*/ new ByteDecoder(Opcode.push, "d"), // 	direct
/*C1	2	*/ new ByteDecoder(Opcode.ajmp, "j"), // 	addr11
/*C2	2	*/ new ByteDecoder(Opcode.clr, "b"), // 	bit
/*C3	1	*/ new ByteDecoder(Opcode.clr, "C"), // 	C
/*C4	1	*/ new ByteDecoder(Opcode.swap, "A"), // 	A
/*C5	2	*/ new ByteDecoder(Opcode.xch, "A,d"), // 	A, direct
/*C6	1	*/ new ByteDecoder(Opcode.xch, "A,@"), // 	A, @R0
/*C7	1	*/ new ByteDecoder(Opcode.xch, "A,@"), // 	A, @R1
/*C8	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R0
/*C9	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R1
/*CA	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R2
/*CB	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R3
/*CC	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R4
/*CD	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R5
/*CE	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R6
/*CF	1	*/ new ByteDecoder(Opcode.xch, "A,r"), // 	A, R7
/*D0	2	*/ new ByteDecoder(Opcode.pop, "d"), // 	direct
/*D1	2	*/ new ByteDecoder(Opcode.acall, "j"), // 	addr11
/*D2	2	*/ new ByteDecoder(Opcode.setb, "b"), // 	bit
/*D3	1	*/ new ByteDecoder(Opcode.setb, "C"), // 	C
/*D4	1	*/ new ByteDecoder(Opcode.da, "A"), // 	A
/*D5	3	*/ new ByteDecoder(Opcode.djnz, "d,o"), // 	direct, offset
/*D6	1	*/ new ByteDecoder(Opcode.xchd, "A,@"), // 	A, @R0
/*D7	1	*/ new ByteDecoder(Opcode.xchd, "A,@"), // 	A, @R1
/*D8	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R0, offset
/*D9	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R1, offset
/*DA	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R2, offset
/*DB	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R3, offset
/*DC	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R4, offset
/*DD	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R5, offset
/*DE	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R6, offset
/*DF	2	*/ new ByteDecoder(Opcode.djnz, "r,o"), // 	R7, offset
/*E0	1	*/ new ByteDecoder(Opcode.movx, "A,D"), // 	A, @DPTR
/*E1	2	*/ new ByteDecoder(Opcode.ajmp, "j"), // 	addr11
/*E2	1	*/ new ByteDecoder(Opcode.movx, "A,@"), // 	A, @R0
/*E3	1	*/ new ByteDecoder(Opcode.movx, "A,@"), // 	A, @R1
/*E4	1	*/ new ByteDecoder(Opcode.clr, "A"), // 	A
/*E5	2	*/ new ByteDecoder(Opcode.mov, "A,d"), // 	A, direct
/*E6	1	*/ new ByteDecoder(Opcode.mov, "A,@"), // 	A, @R0
/*E7	1	*/ new ByteDecoder(Opcode.mov, "A,@"), // 	A, @R1
/*E8	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R0
/*E9	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R1
/*EA	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R2
/*EB	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R3
/*EC	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R4
/*ED	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R5
/*EE	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R6
/*EF	1	*/ new ByteDecoder(Opcode.mov, "A,r"), // 	A, R7
/*F0	1	*/ new ByteDecoder(Opcode.movx, "D,A"), // 	@DPTR, A
/*F1	2	*/ new ByteDecoder(Opcode.acall, "j"), // 	addr11
/*F2	1	*/ new ByteDecoder(Opcode.movx, "@,A"), // 	@R0, A
/*F3	1	*/ new ByteDecoder(Opcode.movx, "@,A"), // 	@R1, A
/*F4	1	*/ new ByteDecoder(Opcode.cpl, "A"), // 	A
/*F5	2	*/ new ByteDecoder(Opcode.mov, "d,A"), // 	direct, A
/*F6	1	*/ new ByteDecoder(Opcode.mov, "@,A"), // 	@R0, A
/*F7	1	*/ new ByteDecoder(Opcode.mov, "@,A"), // 	@R1, A
/*F8	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R0, A
/*F9	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R1, A
/*FA	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R2, A
/*FB	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R3, A
/*FC	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R4, A
/*FD	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R5, A
/*FE	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R6, A
/*FF	1	*/ new ByteDecoder(Opcode.mov, "r,A"), // 	R7, A

        };
    }
}