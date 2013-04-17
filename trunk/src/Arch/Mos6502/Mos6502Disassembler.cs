#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mos6502
{
    public class Mos6502Disassembler : Disassembler
    {
        ImageReader rdr;

        public Mos6502Disassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public Address Address
        {
            get { return rdr.Address; }
        }

        public MachineInstruction DisassembleInstruction()
        {
            var op = rdr.ReadByte();
            var opRec = opRecs[op];
            var fmt = opRec.Format;
            Operand operand = null;
            for (int i = 0; i < fmt.Length; ++i)
            {
                switch (fmt[i++])
                {
                case 'x':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.IndexedIndirect,
                        Register = Registers.x,
                        Offset = rdr.Read(PrimitiveType.Byte)
                    };
                    break;
                case 'z':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.ZeroPage,
                        Register = null,
                        Offset = rdr.Read(PrimitiveType.Byte)
                    };
                    break;
                case 'A':
                    operand = new Operand(PrimitiveType.Byte)
                    {
                        Mode = AddressMode.Absolute,
                        Register = null,
                        Offset = rdr.Read(PrimitiveType.Word16)
                    };
                    break;
                default: throw new NotImplementedException(string.Format("Unknown format character {0}.", fmt[i - 1]));
                }
            }
            return new Instruction
            {
                Code = opRec.Code,
                Operand = operand,
            };
        }

        private class OpRec
        {
            public Opcode Code;
            public string Format;

            public OpRec(Opcode code, string fmt)
            {
                this.Code = code;
                this.Format = fmt;
            }
        }

        private static OpRec[] opRecs = new OpRec[] {
            // 00
            new OpRec(Opcode.brk, ""),
            new OpRec(Opcode.ora, "x"),
            new OpRec(Opcode.illegal, ""),  // KIL
            new OpRec(Opcode.illegal, "x"),  // SLO
            new OpRec(Opcode.illegal, "x"),   // NOP
            new OpRec(Opcode.ora, "z"),  
            new OpRec(Opcode.asl, "z"),  
            new OpRec(Opcode.illegal, "z"),  // SLO
  
            new OpRec(Opcode.php, ""),
            new OpRec(Opcode.ora, "I"),
            new OpRec(Opcode.asl, "I"),
            new OpRec(Opcode.illegal, "I"),  // ANC
            new OpRec(Opcode.illegal, "A"),  // NOP
            new OpRec(Opcode.ora, "A"),
            new OpRec(Opcode.asl, "A"),
            new OpRec(Opcode.illegal, "A"),  // SLO

            // 10
            new OpRec(Opcode.bpl, "j"),
        };
    }


    /*
imm = #$00
zp = $00
zpx = $00,X
zpy = $00,Y
izx = ($00,X)
izy = ($00),Y
abs = $0000
abx = $0000,X
aby = $0000,Y
ind = ($0000)
rel = $0000 (PC-relative) 
     */
    /*


<tr>
<td>font size="+1">1x
<td>BPL<br>rel 2*
<td>ORA<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">SLO<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>ORA<br>zpx 4
<td>ASL<br>zpx 6
<td bgcolor="#E0E0E0">SLO<br>zpx 6
<td>CLC<br>2
<td>ORA<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">SLO<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>ORA<br>abx 4*
<td>ASL<br>abx 7
<td bgcolor="#E0E0E0">SLO<br>abx 7
</tr>

<tr>
<td>font size="+1">2x
<td>JSR<br>abs 6
<td>AND<br>izx 6
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">RLA<br>izx 8
<td>BIT<br>zp 3
<td>AND<br>zp 3
<td>ROL<br>zp 5
<td bgcolor="#E0E0E0">RLA<br>zp 5
<td>PLP<br>4
<td>AND<br>imm 2
<td>ROL<br>2
<td bgcolor="#E0E0E0">ANC<br>imm 2
<td>BIT<br>abs 4
<td>AND<br>abs 4
<td>ROL<br>abs 6
<td bgcolor="#E0E0E0">RLA<br>abs 6
</tr>

<tr>
<td>font size="+1">3x
<td>BMI<br>rel 2*
<td>AND<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">RLA<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>AND<br>zpx 4
<td>ROL<br>zpx 6
<td bgcolor="#E0E0E0">RLA<br>zpx 6
<td>SEC<br>2
<td>AND<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">RLA<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>AND<br>abx 4*
<td>ROL<br>abx 7
<td bgcolor="#E0E0E0">RLA<br>abx 7
</tr>

<tr>
<td>font size="+1">4x
<td>RTI<br>6
<td>EOR<br>izx 6
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">SRE<br>izx 8
<td bgcolor="#E0E0E0">NOP<br>zp 3
<td>EOR<br>zp 3
<td>LSR<br>zp 5
<td bgcolor="#E0E0E0">SRE<br>zp 5
<td>PHA<br>3
<td>EOR<br>imm 2
<td>LSR<br>2
<td bgcolor="#E0E0E0">ALR<br>imm 2
<td>JMP<br>abs 3
<td>EOR<br>abs 4
<td>LSR<br>abs 6
<td bgcolor="#E0E0E0">SRE<br>abs 6
</tr>

<tr>
<td>font size="+1">5x
<td>BVC<br>rel 2*
<td>EOR<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">SRE<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>EOR<br>zpx 4
<td>LSR<br>zpx 6
<td bgcolor="#E0E0E0">SRE<br>zpx 6
<td>CLI<br>2
<td>EOR<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">SRE<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>EOR<br>abx 4*
<td>LSR<br>abx 7
<td bgcolor="#E0E0E0">SRE<br>abx 7
</tr>

<tr>
<td>font size="+1">6x
<td>RTS<br>6
<td>ADC<br>izx 6
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">RRA<br>izx 8
<td bgcolor="#E0E0E0">NOP<br>zp 3
<td>ADC<br>zp 3
<td>ROR<br>zp 5
<td bgcolor="#E0E0E0">RRA<br>zp 5
<td>PLA<br>4
<td>ADC<br>imm 2
<td>ROR<br>2
<td bgcolor="#E0E0E0">ARR<br>imm 2
<td>JMP<br>ind 5
<td>ADC<br>abs 4
<td>ROR<br>abs 6
<td bgcolor="#E0E0E0">RRA<br>abs 6
</tr>

<tr>
<td>font size="+1">7x
<td>BVS<br>rel 2*
<td>ADC<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">RRA<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>ADC<br>zpx 4
<td>ROR<br>zpx 6
<td bgcolor="#E0E0E0">RRA<br>zpx 6
<td>SEI<br>2
<td>ADC<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">RRA<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>ADC<br>abx 4*
<td>ROR<br>abx 7
<td bgcolor="#E0E0E0">RRA<br>abx 7
</tr>

<tr>
<td>font size="+1">8x
<td bgcolor="#E0E0E0">NOP<br>imm 2
<td>STA<br>izx 6
<td bgcolor="#E0E0E0">NOP<br>imm 2
<td bgcolor="#E0E0E0">SAX<br>izx 6
<td>STY<br>zp 3
<td>STA<br>zp 3
<td>STX<br>zp 3
<td bgcolor="#E0E0E0">SAX<br>zp 3
<td>DEY<br>2
<td bgcolor="#E0E0E0">NOP<br>imm 2
<td>TXA<br>2
<td bgcolor="#E0E0E0">font color="#FF0000" size="-1"><i>XAA<br>imm 2</i>
<td>STY<br>abs 4
<td>STA<br>abs 4
<td>STX<br>abs 4
<td bgcolor="#E0E0E0">SAX<br>abs 4
</tr>

<tr>
<td>font size="+1">9x
<td>BCC<br>rel 2*
<td>STA<br>izy 6
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">font color="#0000FF" size="-1"><i>AHX<br>izy 6</i>
<td>STY<br>zpx 4
<td>STA<br>zpx 4
<td>STX<br>zpy 4
<td bgcolor="#E0E0E0">SAX<br>zpy 4
<td>TYA<br>2
<td>STA<br>aby 5
<td>TXS<br>2
<td bgcolor="#E0E0E0">font color="#0000FF" size="-1"><i>TAS<br>aby 5</i>
<td bgcolor="#E0E0E0">font color="#0000FF" size="-1"><i>SHY<br>abx 5</i>
<td>STA<br>abx 5
<td bgcolor="#E0E0E0">font color="#0000FF" size="-1"><i>SHX<br>aby 5</i>
<td bgcolor="#E0E0E0">font color="#0000FF" size="-1"><i>AHX<br>aby 5</i>
</tr>

<tr>
<td>font size="+1">Ax
<td>LDY<br>imm 2
<td>LDA<br>izx 6
<td>LDX<br>imm 2
<td bgcolor="#E0E0E0">LAX<br>izx 6
<td>LDY<br>zp 3
<td>LDA<br>zp 3
<td>LDX<br>zp 3
<td bgcolor="#E0E0E0">LAX<br>zp 3
<td>TAY<br>2
<td>LDA<br>imm 2
<td>TAX<br>2
<td bgcolor="#E0E0E0">font color="#FF0000" size="-1"><i>LAX<br>imm 2</i>
<td>LDY<br>abs 4
<td>LDA<br>abs 4
<td>LDX<br>abs 4
<td bgcolor="#E0E0E0">LAX<br>abs 4
</tr>

<tr>
<td>font size="+1">Bx
<td>BCS<br>rel 2*
<td>LDA<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">LAX<br>izy 5*
<td>LDY<br>zpx 4
<td>LDA<br>zpx 4
<td>LDX<br>zpy 4
<td bgcolor="#E0E0E0">LAX<br>zpy 4
<td>CLV<br>2
<td>LDA<br>aby 4*
<td>TSX<br>2
<td bgcolor="#E0E0E0">LAS<br>aby 4*
<td>LDY<br>abx 4*
<td>LDA<br>abx 4*
<td>LDX<br>aby 4*
<td bgcolor="#E0E0E0">LAX<br>aby 4*
</tr>

<tr>
<td>font size="+1">Cx
<td>CPY<br>imm 2
<td>CMP<br>izx 6
<td bgcolor="#E0E0E0">NOP<br>imm 2
<td bgcolor="#E0E0E0">DCP<br>izx 8
<td>CPY<br>zp 3
<td>CMP<br>zp 3
<td>DEC<br>zp 5
<td bgcolor="#E0E0E0">DCP<br>zp 5
<td>INY<br>2
<td>CMP<br>imm 2
<td>DEX<br>2
<td bgcolor="#E0E0E0">AXS<br>imm 2
<td>CPY<br>abs 4
<td>CMP<br>abs 4
<td>DEC<br>abs 6
<td bgcolor="#E0E0E0">DCP<br>abs 6
</tr>

<tr>
<td>font size="+1">Dx
<td>BNE<br>rel 2*
<td>CMP<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">DCP<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>CMP<br>zpx 4
<td>DEC<br>zpx 6
<td bgcolor="#E0E0E0">DCP<br>zpx 6
<td>CLD<br>2
<td>CMP<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">DCP<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>CMP<br>abx 4*
<td>DEC<br>abx 7
<td bgcolor="#E0E0E0">DCP<br>abx 7
</tr>

<tr>
<td>font size="+1">Ex
<td>CPX<br>imm 2
<td>SBC<br>izx 6
<td bgcolor="#E0E0E0">NOP<br>imm 2
<td bgcolor="#E0E0E0">ISC<br>izx 8
<td>CPX<br>zp 3
<td>SBC<br>zp 3
<td>INC<br>zp 5
<td bgcolor="#E0E0E0">ISC<br>zp 5
<td>INX<br>2
<td>SBC<br>imm 2
<td>NOP<br>2
<td bgcolor="#E0E0E0">SBC<br>imm 2
<td>CPX<br>abs 4
<td>SBC<br>abs 4
<td>INC<br>abs 6
<td bgcolor="#E0E0E0">ISC<br>abs 6
</tr>

<tr>
<td>font size="+1">Fx
<td>BEQ<br>rel 2*
<td>SBC<br>izy 5*
<td bgcolor="#E0E0E0">KIL
<td bgcolor="#E0E0E0">ISC<br>izy 8
<td bgcolor="#E0E0E0">NOP<br>zpx 4
<td>SBC<br>zpx 4
<td>INC<br>zpx 6
<td bgcolor="#E0E0E0">ISC<br>zpx 6
<td>SED<br>2
<td>SBC<br>aby 4*
<td bgcolor="#E0E0E0">NOP<br>2
<td bgcolor="#E0E0E0">ISC<br>aby 7
<td bgcolor="#E0E0E0">NOP<br>abx 4*
<td>SBC<br>abx 4*
<td>INC<br>abx 7
<td bgcolor="#E0E0E0">ISC<br>abx 7
</tr>
</tbody>
    */
}
