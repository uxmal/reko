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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.i8051
{
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
            MachineOperand op = null;
            foreach (var ch in fmt)
            {
                switch (ch)
                {
                case 'a': // An 11-bit address destination. This argument is used by ACALL and AJMP instructions. The target of the CALL or JMP must lie within the same 2K page as the first byte of the following instruction.
                    if (!rdr.TryReadByte(out b))
                        return null;
                    op = AddressOperand.Ptr16(
                        (ushort)(
                            (rdr.Address.ToLinear() & ~0x7Ful) |
                            (uInstr & 0xE0u) << 3 |
                            b));
                    break;
                case 'A': // A 16-bit address destination. This argument is used by LCALL and LJMP instructions.
                    if (!rdr.TryReadBeUInt16(out var uAddr)) // Yes, big endian!
                        return null;
                    op = AddressOperand.Ptr16(uAddr);
                    break;
                case 'b': // A direct addressed bit in internal data RAM or SFR memory.
                case 'd': // An internal data RAM location (0-127) or SFR (128-255).
                case 'i': // A constant included in the instruction encoding.
                case 'o': // A signed (two's complement) 8-bit offset (-128 to 127) relative to the first byte of the following instruction.
                case '@': // @Ri	An internal data RAM location (0-255) addressed indirectly through R0 or R1.
                case 'r': // Regi
                    throw new NotSupportedException();
                }
            }

            return new i8051Instruction
            {
                Opcode = opcode,
                Address = this.addr,
                Length = (int)(rdr.Address - this.addr),
                Operand = op,
            };
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
/*00	1	*/ new ByteDecoder(Opcode.nop, "X"), 
/*01	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*02	3	*/ new ByteDecoder(Opcode.ljmp, "A"), // 	addr16
/*03	1	*/ new ByteDecoder(Opcode.rr, "X"), // 	A
/*04	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	A
/*05	2	*/ new ByteDecoder(Opcode.inc, "X"), // 	direct
/*06	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	@R0
/*07	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	@R1
/*08	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R0
/*09	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R1
/*0A	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R2
/*0B	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R3
/*0C	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R4
/*0D	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R5
/*0E	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R6
/*0F	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	R7
/*10	3	*/ new ByteDecoder(Opcode.jbc, "X"), // 	bit, offset
/*11	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*12	3	*/ new ByteDecoder(Opcode.lcall, "X"), // 	addr16
/*13	1	*/ new ByteDecoder(Opcode.rrc, "X"), // 	A
/*14	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	A
/*15	2	*/ new ByteDecoder(Opcode.dec, "X"), // 	direct
/*16	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	@R0
/*17	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	@R1
/*18	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R0
/*19	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R1
/*1A	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R2
/*1B	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R3
/*1C	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R4
/*1D	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R5
/*1E	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R6
/*1F	1	*/ new ByteDecoder(Opcode.dec, "X"), // 	R7
/*20	3	*/ new ByteDecoder(Opcode.jb, "X"), // 	bit, offset
/*21	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*22	1	*/ new ByteDecoder(Opcode.ret, "X"), // 	 
/*23	1	*/ new ByteDecoder(Opcode.rl, "X"), // 	A
/*24	2	*/ new ByteDecoder(Opcode.add, "X"), // 	A, #immed
/*25	2	*/ new ByteDecoder(Opcode.add, "X"), // 	A, direct
/*26	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, @R0
/*27	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, @R1
/*28	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R0
/*29	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R1
/*2A	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R2
/*2B	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R3
/*2C	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R4
/*2D	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R5
/*2E	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R6
/*2F	1	*/ new ByteDecoder(Opcode.add, "X"), // 	A, R7
/*30	3	*/ new ByteDecoder(Opcode.jnb, "X"), // 	bit, offset
/*31	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*32	1	*/ new ByteDecoder(Opcode.reti, "X"), // 	 
/*33	1	*/ new ByteDecoder(Opcode.rlc, "X"), // 	A
/*34	2	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, #immed
/*35	2	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, direct
/*36	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, @R0
/*37	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, @R1
/*38	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R0
/*39	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R1
/*3A	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R2
/*3B	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R3
/*3C	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R4
/*3D	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R5
/*3E	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R6
/*3F	1	*/ new ByteDecoder(Opcode.addc, "X"), // 	A, R7
/*40	2	*/ new ByteDecoder(Opcode.jc, "X"), // 	offset
/*41	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*42	2	*/ new ByteDecoder(Opcode.orl, "X"), // 	direct, A
/*43	3	*/ new ByteDecoder(Opcode.orl, "X"), // 	direct, #immed
/*44	2	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, #immed
/*45	2	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, direct
/*46	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, @R0
/*47	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, @R1
/*48	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R0
/*49	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R1
/*4A	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R2
/*4B	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R3
/*4C	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R4
/*4D	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R5
/*4E	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R6
/*4F	1	*/ new ByteDecoder(Opcode.orl, "X"), // 	A, R7
/*50	2	*/ new ByteDecoder(Opcode.jnc, "X"), // 	offset
/*51	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*52	2	*/ new ByteDecoder(Opcode.anl, "X"), // 	direct, A
/*53	3	*/ new ByteDecoder(Opcode.anl, "X"), // 	direct, #immed
/*54	2	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, #immed
/*55	2	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, direct
/*56	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, @R0
/*57	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, @R1
/*58	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R0
/*59	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R1
/*5A	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R2
/*5B	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R3
/*5C	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R4
/*5D	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R5
/*5E	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R6
/*5F	1	*/ new ByteDecoder(Opcode.anl, "X"), // 	A, R7
/*60	2	*/ new ByteDecoder(Opcode.jz, "X"), // 	offset
/*61	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*62	2	*/ new ByteDecoder(Opcode.xrl, "X"), // 	direct, A
/*63	3	*/ new ByteDecoder(Opcode.xrl, "X"), // 	direct, #immed
/*64	2	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, #immed
/*65	2	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, direct
/*66	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, @R0
/*67	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, @R1
/*68	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R0
/*69	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R1
/*6A	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R2
/*6B	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R3
/*6C	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R4
/*6D	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R5
/*6E	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R6
/*6F	1	*/ new ByteDecoder(Opcode.xrl, "X"), // 	A, R7
/*70	2	*/ new ByteDecoder(Opcode.jnz, "X"), // 	offset
/*71	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*72	2	*/ new ByteDecoder(Opcode.orl, "X"), // 	C, bit
/*73	1	*/ new ByteDecoder(Opcode.jmp, "X"), // 	@A+DPTR
/*74	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, #immed
/*75	3	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, #immed
/*76	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R0, #immed
/*77	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R1, #immed
/*78	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R0, #immed
/*79	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R1, #immed
/*7A	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R2, #immed
/*7B	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R3, #immed
/*7C	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R4, #immed
/*7D	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R5, #immed
/*7E	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R6, #immed
/*7F	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R7, #immed	 	

/*80	2	*/ new ByteDecoder(Opcode.sjmp, "X"), // 	offset
/*81	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*82	2	*/ new ByteDecoder(Opcode.anl, "X"), // 	C, bit
/*83	1	*/ new ByteDecoder(Opcode.movc, "X"), // 	A, @A+PC
/*84	1	*/ new ByteDecoder(Opcode.div, "X"), // 	AB
/*85	3	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, direct
/*86	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, @R0
/*87	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, @R1
/*88	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R0
/*89	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R1
/*8A	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R2
/*8B	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R3
/*8C	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R4
/*8D	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R5
/*8E	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R6
/*8F	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, R7
/*90	3	*/ new ByteDecoder(Opcode.mov, "X"), // 	DPTR, #immed
/*91	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*92	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	bit, C
/*93	1	*/ new ByteDecoder(Opcode.movc, "X"), // 	A, @A+DPTR
/*94	2	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, #immed
/*95	2	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, direct
/*96	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, @R0
/*97	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, @R1
/*98	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R0
/*99	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R1
/*9A	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R2
/*9B	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R3
/*9C	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R4
/*9D	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R5
/*9E	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R6
/*9F	1	*/ new ByteDecoder(Opcode.subb, "X"), // 	A, R7
/*A0	2	*/ new ByteDecoder(Opcode.orl, "X"), // 	C, /bit
/*A1	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*A2	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	C, bit
/*A3	1	*/ new ByteDecoder(Opcode.inc, "X"), // 	DPTR
/*A4	1	*/ new ByteDecoder(Opcode.mul, "X"), // 	AB
/*A5	 	*/ new ByteDecoder(Opcode.reserved, "X"), // 	 
/*A6	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R0, direct
/*A7	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R1, direct
/*A8	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R0, direct
/*A9	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R1, direct
/*AA	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R2, direct
/*AB	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R3, direct
/*AC	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R4, direct
/*AD	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R5, direct
/*AE	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R6, direct
/*AF	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	R7, direct
/*B0	2	*/ new ByteDecoder(Opcode.anl, "X"), // 	C, /bit
/*B1	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*B2	2	*/ new ByteDecoder(Opcode.cpl, "X"), // 	bit
/*B3	1	*/ new ByteDecoder(Opcode.cpl, "X"), // 	C
/*B4	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	A, #immed, offset
/*B5	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	A, direct, offset
/*B6	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	@R0, #immed, offset
/*B7	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	@R1, #immed, offset
/*B8	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R0, #immed, offset
/*B9	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R1, #immed, offset
/*BA	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R2, #immed, offset
/*BB	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R3, #immed, offset
/*BC	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R4, #immed, offset
/*BD	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R5, #immed, offset
/*BE	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R6, #immed, offset
/*BF	3	*/ new ByteDecoder(Opcode.cjne, "X"), // 	R7, #immed, offset
/*C0	2	*/ new ByteDecoder(Opcode.push, "X"), // 	direct
/*C1	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*C2	2	*/ new ByteDecoder(Opcode.clr, "X"), // 	bit
/*C3	1	*/ new ByteDecoder(Opcode.clr, "X"), // 	C
/*C4	1	*/ new ByteDecoder(Opcode.swap, "X"), // 	A
/*C5	2	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, direct
/*C6	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, @R0
/*C7	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, @R1
/*C8	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R0
/*C9	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R1
/*CA	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R2
/*CB	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R3
/*CC	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R4
/*CD	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R5
/*CE	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R6
/*CF	1	*/ new ByteDecoder(Opcode.xch, "X"), // 	A, R7
/*D0	2	*/ new ByteDecoder(Opcode.pop, "X"), // 	direct
/*D1	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*D2	2	*/ new ByteDecoder(Opcode.setb, "X"), // 	bit
/*D3	1	*/ new ByteDecoder(Opcode.setb, "X"), // 	C
/*D4	1	*/ new ByteDecoder(Opcode.da, "X"), // 	A
/*D5	3	*/ new ByteDecoder(Opcode.djnz, "X"), // 	direct, offset
/*D6	1	*/ new ByteDecoder(Opcode.xchd, "X"), // 	A, @R0
/*D7	1	*/ new ByteDecoder(Opcode.xchd, "X"), // 	A, @R1
/*D8	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R0, offset
/*D9	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R1, offset
/*DA	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R2, offset
/*DB	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R3, offset
/*DC	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R4, offset
/*DD	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R5, offset
/*DE	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R6, offset
/*DF	2	*/ new ByteDecoder(Opcode.djnz, "X"), // 	R7, offset
/*E0	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	A, @DPTR
/*E1	2	*/ new ByteDecoder(Opcode.ajmp, "a"), // 	addr11
/*E2	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	A, @R0
/*E3	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	A, @R1
/*E4	1	*/ new ByteDecoder(Opcode.clr, "X"), // 	A
/*E5	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, direct
/*E6	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, @R0
/*E7	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, @R1
/*E8	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R0
/*E9	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R1
/*EA	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R2
/*EB	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R3
/*EC	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R4
/*ED	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R5
/*EE	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R6
/*EF	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	A, R7
/*F0	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	@DPTR, A
/*F1	2	*/ new ByteDecoder(Opcode.acall, "X"), // 	addr11
/*F2	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	@R0, A
/*F3	1	*/ new ByteDecoder(Opcode.movx, "X"), // 	@R1, A
/*F4	1	*/ new ByteDecoder(Opcode.cpl, "X"), // 	A
/*F5	2	*/ new ByteDecoder(Opcode.mov, "X"), // 	direct, A
/*F6	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R0, A
/*F7	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	@R1, A
/*F8	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R0, A
/*F9	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R1, A
/*FA	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R2, A
/*FB	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R3, A
/*FC	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R4, A
/*FD	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R5, A
/*FE	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R6, A
/*FF	1	*/ new ByteDecoder(Opcode.mov, "X"), // 	R7, A

        };
    }
}