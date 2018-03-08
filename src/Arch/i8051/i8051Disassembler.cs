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

        private i8051Instruction Decode(Opcode opcode, byte op)
        {
            return new i8051Instruction
            {
                Opcode = opcode,
                Address = this.addr,
                Length = (int)(rdr.Address - this.addr),
            };
        }

        private abstract class Decoder
        {
            public abstract i8051Instruction Decode(byte op, i8051Disassembler dasm);
        }


        private class ByteDecoder : Decoder
        {
            private Opcode opcode;

            public ByteDecoder(Opcode opcode)
            {
                this.opcode = opcode;
            }

            public override i8051Instruction Decode(byte op, i8051Disassembler dasm)
            {
                return dasm.Decode(opcode, op);
            }
        }

        private static readonly Decoder[] decoders = new[] {
        /*
/*00	1	*/ new ByteDecoder(Opcode.nop), 
/*01	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*02	3	*/ new ByteDecoder(Opcode.ljmp), // 	addr16
/*03	1	*/ new ByteDecoder(Opcode.rr), // 	A
/*04	1	*/ new ByteDecoder(Opcode.inc), // 	A
/*05	2	*/ new ByteDecoder(Opcode.inc), // 	direct
/*06	1	*/ new ByteDecoder(Opcode.inc), // 	@R0
/*07	1	*/ new ByteDecoder(Opcode.inc), // 	@R1
/*08	1	*/ new ByteDecoder(Opcode.inc), // 	R0
/*09	1	*/ new ByteDecoder(Opcode.inc), // 	R1
/*0A	1	*/ new ByteDecoder(Opcode.inc), // 	R2
/*0B	1	*/ new ByteDecoder(Opcode.inc), // 	R3
/*0C	1	*/ new ByteDecoder(Opcode.inc), // 	R4
/*0D	1	*/ new ByteDecoder(Opcode.inc), // 	R5
/*0E	1	*/ new ByteDecoder(Opcode.inc), // 	R6
/*0F	1	*/ new ByteDecoder(Opcode.inc), // 	R7
/*10	3	*/ new ByteDecoder(Opcode.jbc), // 	bit, offset
/*11	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*12	3	*/ new ByteDecoder(Opcode.lcall), // 	addr16
/*13	1	*/ new ByteDecoder(Opcode.rrc), // 	A
/*14	1	*/ new ByteDecoder(Opcode.dec), // 	A
/*15	2	*/ new ByteDecoder(Opcode.dec), // 	direct
/*16	1	*/ new ByteDecoder(Opcode.dec), // 	@R0
/*17	1	*/ new ByteDecoder(Opcode.dec), // 	@R1
/*18	1	*/ new ByteDecoder(Opcode.dec), // 	R0
/*19	1	*/ new ByteDecoder(Opcode.dec), // 	R1
/*1A	1	*/ new ByteDecoder(Opcode.dec), // 	R2
/*1B	1	*/ new ByteDecoder(Opcode.dec), // 	R3
/*1C	1	*/ new ByteDecoder(Opcode.dec), // 	R4
/*1D	1	*/ new ByteDecoder(Opcode.dec), // 	R5
/*1E	1	*/ new ByteDecoder(Opcode.dec), // 	R6
/*1F	1	*/ new ByteDecoder(Opcode.dec), // 	R7
/*20	3	*/ new ByteDecoder(Opcode.jb), // 	bit, offset
/*21	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*22	1	*/ new ByteDecoder(Opcode.ret), // 	 
/*23	1	*/ new ByteDecoder(Opcode.rl), // 	A
/*24	2	*/ new ByteDecoder(Opcode.add), // 	A, #immed
/*25	2	*/ new ByteDecoder(Opcode.add), // 	A, direct
/*26	1	*/ new ByteDecoder(Opcode.add), // 	A, @R0
/*27	1	*/ new ByteDecoder(Opcode.add), // 	A, @R1
/*28	1	*/ new ByteDecoder(Opcode.add), // 	A, R0
/*29	1	*/ new ByteDecoder(Opcode.add), // 	A, R1
/*2A	1	*/ new ByteDecoder(Opcode.add), // 	A, R2
/*2B	1	*/ new ByteDecoder(Opcode.add), // 	A, R3
/*2C	1	*/ new ByteDecoder(Opcode.add), // 	A, R4
/*2D	1	*/ new ByteDecoder(Opcode.add), // 	A, R5
/*2E	1	*/ new ByteDecoder(Opcode.add), // 	A, R6
/*2F	1	*/ new ByteDecoder(Opcode.add), // 	A, R7
/*30	3	*/ new ByteDecoder(Opcode.jnb), // 	bit, offset
/*31	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*32	1	*/ new ByteDecoder(Opcode.reti), // 	 
/*33	1	*/ new ByteDecoder(Opcode.rlc), // 	A
/*34	2	*/ new ByteDecoder(Opcode.addc), // 	A, #immed
/*35	2	*/ new ByteDecoder(Opcode.addc), // 	A, direct
/*36	1	*/ new ByteDecoder(Opcode.addc), // 	A, @R0
/*37	1	*/ new ByteDecoder(Opcode.addc), // 	A, @R1
/*38	1	*/ new ByteDecoder(Opcode.addc), // 	A, R0
/*39	1	*/ new ByteDecoder(Opcode.addc), // 	A, R1
/*3A	1	*/ new ByteDecoder(Opcode.addc), // 	A, R2
/*3B	1	*/ new ByteDecoder(Opcode.addc), // 	A, R3
/*3C	1	*/ new ByteDecoder(Opcode.addc), // 	A, R4
/*3D	1	*/ new ByteDecoder(Opcode.addc), // 	A, R5
/*3E	1	*/ new ByteDecoder(Opcode.addc), // 	A, R6
/*3F	1	*/ new ByteDecoder(Opcode.addc), // 	A, R7
/*40	2	*/ new ByteDecoder(Opcode.jc), // 	offset
/*41	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*42	2	*/ new ByteDecoder(Opcode.orl), // 	direct, A
/*43	3	*/ new ByteDecoder(Opcode.orl), // 	direct, #immed
/*44	2	*/ new ByteDecoder(Opcode.orl), // 	A, #immed
/*45	2	*/ new ByteDecoder(Opcode.orl), // 	A, direct
/*46	1	*/ new ByteDecoder(Opcode.orl), // 	A, @R0
/*47	1	*/ new ByteDecoder(Opcode.orl), // 	A, @R1
/*48	1	*/ new ByteDecoder(Opcode.orl), // 	A, R0
/*49	1	*/ new ByteDecoder(Opcode.orl), // 	A, R1
/*4A	1	*/ new ByteDecoder(Opcode.orl), // 	A, R2
/*4B	1	*/ new ByteDecoder(Opcode.orl), // 	A, R3
/*4C	1	*/ new ByteDecoder(Opcode.orl), // 	A, R4
/*4D	1	*/ new ByteDecoder(Opcode.orl), // 	A, R5
/*4E	1	*/ new ByteDecoder(Opcode.orl), // 	A, R6
/*4F	1	*/ new ByteDecoder(Opcode.orl), // 	A, R7
/*50	2	*/ new ByteDecoder(Opcode.jnc), // 	offset
/*51	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*52	2	*/ new ByteDecoder(Opcode.anl), // 	direct, A
/*53	3	*/ new ByteDecoder(Opcode.anl), // 	direct, #immed
/*54	2	*/ new ByteDecoder(Opcode.anl), // 	A, #immed
/*55	2	*/ new ByteDecoder(Opcode.anl), // 	A, direct
/*56	1	*/ new ByteDecoder(Opcode.anl), // 	A, @R0
/*57	1	*/ new ByteDecoder(Opcode.anl), // 	A, @R1
/*58	1	*/ new ByteDecoder(Opcode.anl), // 	A, R0
/*59	1	*/ new ByteDecoder(Opcode.anl), // 	A, R1
/*5A	1	*/ new ByteDecoder(Opcode.anl), // 	A, R2
/*5B	1	*/ new ByteDecoder(Opcode.anl), // 	A, R3
/*5C	1	*/ new ByteDecoder(Opcode.anl), // 	A, R4
/*5D	1	*/ new ByteDecoder(Opcode.anl), // 	A, R5
/*5E	1	*/ new ByteDecoder(Opcode.anl), // 	A, R6
/*5F	1	*/ new ByteDecoder(Opcode.anl), // 	A, R7
/*60	2	*/ new ByteDecoder(Opcode.jz), // 	offset
/*61	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*62	2	*/ new ByteDecoder(Opcode.xrl), // 	direct, A
/*63	3	*/ new ByteDecoder(Opcode.xrl), // 	direct, #immed
/*64	2	*/ new ByteDecoder(Opcode.xrl), // 	A, #immed
/*65	2	*/ new ByteDecoder(Opcode.xrl), // 	A, direct
/*66	1	*/ new ByteDecoder(Opcode.xrl), // 	A, @R0
/*67	1	*/ new ByteDecoder(Opcode.xrl), // 	A, @R1
/*68	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R0
/*69	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R1
/*6A	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R2
/*6B	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R3
/*6C	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R4
/*6D	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R5
/*6E	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R6
/*6F	1	*/ new ByteDecoder(Opcode.xrl), // 	A, R7
/*70	2	*/ new ByteDecoder(Opcode.jnz), // 	offset
/*71	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*72	2	*/ new ByteDecoder(Opcode.orl), // 	C, bit
/*73	1	*/ new ByteDecoder(Opcode.jmp), // 	@A+DPTR
/*74	2	*/ new ByteDecoder(Opcode.mov), // 	A, #immed
/*75	3	*/ new ByteDecoder(Opcode.mov), // 	direct, #immed
/*76	2	*/ new ByteDecoder(Opcode.mov), // 	@R0, #immed
/*77	2	*/ new ByteDecoder(Opcode.mov), // 	@R1, #immed
/*78	2	*/ new ByteDecoder(Opcode.mov), // 	R0, #immed
/*79	2	*/ new ByteDecoder(Opcode.mov), // 	R1, #immed
/*7A	2	*/ new ByteDecoder(Opcode.mov), // 	R2, #immed
/*7B	2	*/ new ByteDecoder(Opcode.mov), // 	R3, #immed
/*7C	2	*/ new ByteDecoder(Opcode.mov), // 	R4, #immed
/*7D	2	*/ new ByteDecoder(Opcode.mov), // 	R5, #immed
/*7E	2	*/ new ByteDecoder(Opcode.mov), // 	R6, #immed
/*7F	2	*/ new ByteDecoder(Opcode.mov), // 	R7, #immed	 	

/*80	2	*/ new ByteDecoder(Opcode.sjmp), // 	offset
/*81	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*82	2	*/ new ByteDecoder(Opcode.anl), // 	C, bit
/*83	1	*/ new ByteDecoder(Opcode.movc), // 	A, @A+PC
/*84	1	*/ new ByteDecoder(Opcode.div), // 	AB
/*85	3	*/ new ByteDecoder(Opcode.mov), // 	direct, direct
/*86	2	*/ new ByteDecoder(Opcode.mov), // 	direct, @R0
/*87	2	*/ new ByteDecoder(Opcode.mov), // 	direct, @R1
/*88	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R0
/*89	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R1
/*8A	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R2
/*8B	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R3
/*8C	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R4
/*8D	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R5
/*8E	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R6
/*8F	2	*/ new ByteDecoder(Opcode.mov), // 	direct, R7
/*90	3	*/ new ByteDecoder(Opcode.mov), // 	DPTR, #immed
/*91	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*92	2	*/ new ByteDecoder(Opcode.mov), // 	bit, C
/*93	1	*/ new ByteDecoder(Opcode.movc), // 	A, @A+DPTR
/*94	2	*/ new ByteDecoder(Opcode.subb), // 	A, #immed
/*95	2	*/ new ByteDecoder(Opcode.subb), // 	A, direct
/*96	1	*/ new ByteDecoder(Opcode.subb), // 	A, @R0
/*97	1	*/ new ByteDecoder(Opcode.subb), // 	A, @R1
/*98	1	*/ new ByteDecoder(Opcode.subb), // 	A, R0
/*99	1	*/ new ByteDecoder(Opcode.subb), // 	A, R1
/*9A	1	*/ new ByteDecoder(Opcode.subb), // 	A, R2
/*9B	1	*/ new ByteDecoder(Opcode.subb), // 	A, R3
/*9C	1	*/ new ByteDecoder(Opcode.subb), // 	A, R4
/*9D	1	*/ new ByteDecoder(Opcode.subb), // 	A, R5
/*9E	1	*/ new ByteDecoder(Opcode.subb), // 	A, R6
/*9F	1	*/ new ByteDecoder(Opcode.subb), // 	A, R7
/*A0	2	*/ new ByteDecoder(Opcode.orl), // 	C, /bit
/*A1	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*A2	2	*/ new ByteDecoder(Opcode.mov), // 	C, bit
/*A3	1	*/ new ByteDecoder(Opcode.inc), // 	DPTR
/*A4	1	*/ new ByteDecoder(Opcode.mul), // 	AB
/*A5	 	*/ new ByteDecoder(Opcode.reserved), // 	 
/*A6	2	*/ new ByteDecoder(Opcode.mov), // 	@R0, direct
/*A7	2	*/ new ByteDecoder(Opcode.mov), // 	@R1, direct
/*A8	2	*/ new ByteDecoder(Opcode.mov), // 	R0, direct
/*A9	2	*/ new ByteDecoder(Opcode.mov), // 	R1, direct
/*AA	2	*/ new ByteDecoder(Opcode.mov), // 	R2, direct
/*AB	2	*/ new ByteDecoder(Opcode.mov), // 	R3, direct
/*AC	2	*/ new ByteDecoder(Opcode.mov), // 	R4, direct
/*AD	2	*/ new ByteDecoder(Opcode.mov), // 	R5, direct
/*AE	2	*/ new ByteDecoder(Opcode.mov), // 	R6, direct
/*AF	2	*/ new ByteDecoder(Opcode.mov), // 	R7, direct
/*B0	2	*/ new ByteDecoder(Opcode.anl), // 	C, /bit
/*B1	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*B2	2	*/ new ByteDecoder(Opcode.cpl), // 	bit
/*B3	1	*/ new ByteDecoder(Opcode.cpl), // 	C
/*B4	3	*/ new ByteDecoder(Opcode.cjne), // 	A, #immed, offset
/*B5	3	*/ new ByteDecoder(Opcode.cjne), // 	A, direct, offset
/*B6	3	*/ new ByteDecoder(Opcode.cjne), // 	@R0, #immed, offset
/*B7	3	*/ new ByteDecoder(Opcode.cjne), // 	@R1, #immed, offset
/*B8	3	*/ new ByteDecoder(Opcode.cjne), // 	R0, #immed, offset
/*B9	3	*/ new ByteDecoder(Opcode.cjne), // 	R1, #immed, offset
/*BA	3	*/ new ByteDecoder(Opcode.cjne), // 	R2, #immed, offset
/*BB	3	*/ new ByteDecoder(Opcode.cjne), // 	R3, #immed, offset
/*BC	3	*/ new ByteDecoder(Opcode.cjne), // 	R4, #immed, offset
/*BD	3	*/ new ByteDecoder(Opcode.cjne), // 	R5, #immed, offset
/*BE	3	*/ new ByteDecoder(Opcode.cjne), // 	R6, #immed, offset
/*BF	3	*/ new ByteDecoder(Opcode.cjne), // 	R7, #immed, offset
/*C0	2	*/ new ByteDecoder(Opcode.push), // 	direct
/*C1	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*C2	2	*/ new ByteDecoder(Opcode.clr), // 	bit
/*C3	1	*/ new ByteDecoder(Opcode.clr), // 	C
/*C4	1	*/ new ByteDecoder(Opcode.swap), // 	A
/*C5	2	*/ new ByteDecoder(Opcode.xch), // 	A, direct
/*C6	1	*/ new ByteDecoder(Opcode.xch), // 	A, @R0
/*C7	1	*/ new ByteDecoder(Opcode.xch), // 	A, @R1
/*C8	1	*/ new ByteDecoder(Opcode.xch), // 	A, R0
/*C9	1	*/ new ByteDecoder(Opcode.xch), // 	A, R1
/*CA	1	*/ new ByteDecoder(Opcode.xch), // 	A, R2
/*CB	1	*/ new ByteDecoder(Opcode.xch), // 	A, R3
/*CC	1	*/ new ByteDecoder(Opcode.xch), // 	A, R4
/*CD	1	*/ new ByteDecoder(Opcode.xch), // 	A, R5
/*CE	1	*/ new ByteDecoder(Opcode.xch), // 	A, R6
/*CF	1	*/ new ByteDecoder(Opcode.xch), // 	A, R7
/*D0	2	*/ new ByteDecoder(Opcode.pop), // 	direct
/*D1	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*D2	2	*/ new ByteDecoder(Opcode.setb), // 	bit
/*D3	1	*/ new ByteDecoder(Opcode.setb), // 	C
/*D4	1	*/ new ByteDecoder(Opcode.da), // 	A
/*D5	3	*/ new ByteDecoder(Opcode.djnz), // 	direct, offset
/*D6	1	*/ new ByteDecoder(Opcode.xchd), // 	A, @R0
/*D7	1	*/ new ByteDecoder(Opcode.xchd), // 	A, @R1
/*D8	2	*/ new ByteDecoder(Opcode.djnz), // 	R0, offset
/*D9	2	*/ new ByteDecoder(Opcode.djnz), // 	R1, offset
/*DA	2	*/ new ByteDecoder(Opcode.djnz), // 	R2, offset
/*DB	2	*/ new ByteDecoder(Opcode.djnz), // 	R3, offset
/*DC	2	*/ new ByteDecoder(Opcode.djnz), // 	R4, offset
/*DD	2	*/ new ByteDecoder(Opcode.djnz), // 	R5, offset
/*DE	2	*/ new ByteDecoder(Opcode.djnz), // 	R6, offset
/*DF	2	*/ new ByteDecoder(Opcode.djnz), // 	R7, offset
/*E0	1	*/ new ByteDecoder(Opcode.movx), // 	A, @DPTR
/*E1	2	*/ new ByteDecoder(Opcode.ajmp), // 	addr11
/*E2	1	*/ new ByteDecoder(Opcode.movx), // 	A, @R0
/*E3	1	*/ new ByteDecoder(Opcode.movx), // 	A, @R1
/*E4	1	*/ new ByteDecoder(Opcode.clr), // 	A
/*E5	2	*/ new ByteDecoder(Opcode.mov), // 	A, direct
/*E6	1	*/ new ByteDecoder(Opcode.mov), // 	A, @R0
/*E7	1	*/ new ByteDecoder(Opcode.mov), // 	A, @R1
/*E8	1	*/ new ByteDecoder(Opcode.mov), // 	A, R0
/*E9	1	*/ new ByteDecoder(Opcode.mov), // 	A, R1
/*EA	1	*/ new ByteDecoder(Opcode.mov), // 	A, R2
/*EB	1	*/ new ByteDecoder(Opcode.mov), // 	A, R3
/*EC	1	*/ new ByteDecoder(Opcode.mov), // 	A, R4
/*ED	1	*/ new ByteDecoder(Opcode.mov), // 	A, R5
/*EE	1	*/ new ByteDecoder(Opcode.mov), // 	A, R6
/*EF	1	*/ new ByteDecoder(Opcode.mov), // 	A, R7
/*F0	1	*/ new ByteDecoder(Opcode.movx), // 	@DPTR, A
/*F1	2	*/ new ByteDecoder(Opcode.acall), // 	addr11
/*F2	1	*/ new ByteDecoder(Opcode.movx), // 	@R0, A
/*F3	1	*/ new ByteDecoder(Opcode.movx), // 	@R1, A
/*F4	1	*/ new ByteDecoder(Opcode.cpl), // 	A
/*F5	2	*/ new ByteDecoder(Opcode.mov), // 	direct, A
/*F6	1	*/ new ByteDecoder(Opcode.mov), // 	@R0, A
/*F7	1	*/ new ByteDecoder(Opcode.mov), // 	@R1, A
/*F8	1	*/ new ByteDecoder(Opcode.mov), // 	R0, A
/*F9	1	*/ new ByteDecoder(Opcode.mov), // 	R1, A
/*FA	1	*/ new ByteDecoder(Opcode.mov), // 	R2, A
/*FB	1	*/ new ByteDecoder(Opcode.mov), // 	R3, A
/*FC	1	*/ new ByteDecoder(Opcode.mov), // 	R4, A
/*FD	1	*/ new ByteDecoder(Opcode.mov), // 	R5, A
/*FE	1	*/ new ByteDecoder(Opcode.mov), // 	R6, A
/*FF	1	*/ new ByteDecoder(Opcode.mov), // 	R7, A

        };
    }
}