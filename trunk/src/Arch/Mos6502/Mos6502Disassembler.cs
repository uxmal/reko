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
            new OpRec(Opcode.bpl, "j")
        };
    }

    /*


<tr>
<td><b><font size="+1">1x</font></b></td>
<td><b><font size="-1">BPL<br>rel 2*</font></b></td>
<td><b><font size="-1">ORA<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SLO<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">ORA<br>zpx 4</font></b></td>
<td><b><font size="-1">ASL<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SLO<br>zpx 6</font></b></td>
<td><b><font size="-1">CLC<br>2</font></b></td>
<td><b><font size="-1">ORA<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SLO<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">ORA<br>abx 4*</font></b></td>
<td><b><font size="-1">ASL<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SLO<br>abx 7</font></b></td>
</tr>

<tr>
<td><b><font size="+1">2x</font></b></td>
<td><b><font size="-1">JSR<br>abs 6</font></b></td>
<td><b><font size="-1">AND<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>izx 8</font></b></td>
<td><b><font size="-1">BIT<br>zp 3</font></b></td>
<td><b><font size="-1">AND<br>zp 3</font></b></td>
<td><b><font size="-1">ROL<br>zp 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>zp 5</font></b></td>
<td><b><font size="-1">PLP<br>4</font></b></td>
<td><b><font size="-1">AND<br>imm 2</font></b></td>
<td><b><font size="-1">ROL<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ANC<br>imm 2</font></b></td>
<td><b><font size="-1">BIT<br>abs 4</font></b></td>
<td><b><font size="-1">AND<br>abs 4</font></b></td>
<td><b><font size="-1">ROL<br>abs 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>abs 6</font></b></td>
</tr>

<tr>
<td><b><font size="+1">3x</font></b></td>
<td><b><font size="-1">BMI<br>rel 2*</font></b></td>
<td><b><font size="-1">AND<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">AND<br>zpx 4</font></b></td>
<td><b><font size="-1">ROL<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>zpx 6</font></b></td>
<td><b><font size="-1">SEC<br>2</font></b></td>
<td><b><font size="-1">AND<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">AND<br>abx 4*</font></b></td>
<td><b><font size="-1">ROL<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RLA<br>abx 7</font></b></td>
</tr>

<tr>
<td><b><font size="+1">4x</font></b></td>
<td><b><font size="-1">RTI<br>6</font></b></td>
<td><b><font size="-1">EOR<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>izx 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zp 3</font></b></td>
<td><b><font size="-1">EOR<br>zp 3</font></b></td>
<td><b><font size="-1">LSR<br>zp 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>zp 5</font></b></td>
<td><b><font size="-1">PHA<br>3</font></b></td>
<td><b><font size="-1">EOR<br>imm 2</font></b></td>
<td><b><font size="-1">LSR<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ALR<br>imm 2</font></b></td>
<td><b><font size="-1">JMP<br>abs 3</font></b></td>
<td><b><font size="-1">EOR<br>abs 4</font></b></td>
<td><b><font size="-1">LSR<br>abs 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>abs 6</font></b></td>
</tr>

<tr>
<td><b><font size="+1">5x</font></b></td>
<td><b><font size="-1">BVC<br>rel 2*</font></b></td>
<td><b><font size="-1">EOR<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">EOR<br>zpx 4</font></b></td>
<td><b><font size="-1">LSR<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>zpx 6</font></b></td>
<td><b><font size="-1">CLI<br>2</font></b></td>
<td><b><font size="-1">EOR<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">EOR<br>abx 4*</font></b></td>
<td><b><font size="-1">LSR<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SRE<br>abx 7</font></b></td>
</tr>

<tr>
<td><b><font size="+1">6x</font></b></td>
<td><b><font size="-1">RTS<br>6</font></b></td>
<td><b><font size="-1">ADC<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>izx 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zp 3</font></b></td>
<td><b><font size="-1">ADC<br>zp 3</font></b></td>
<td><b><font size="-1">ROR<br>zp 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>zp 5</font></b></td>
<td><b><font size="-1">PLA<br>4</font></b></td>
<td><b><font size="-1">ADC<br>imm 2</font></b></td>
<td><b><font size="-1">ROR<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ARR<br>imm 2</font></b></td>
<td><b><font size="-1">JMP<br>ind 5</font></b></td>
<td><b><font size="-1">ADC<br>abs 4</font></b></td>
<td><b><font size="-1">ROR<br>abs 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>abs 6</font></b></td>
</tr>

<tr>
<td><b><font size="+1">7x</font></b></td>
<td><b><font size="-1">BVS<br>rel 2*</font></b></td>
<td><b><font size="-1">ADC<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">ADC<br>zpx 4</font></b></td>
<td><b><font size="-1">ROR<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>zpx 6</font></b></td>
<td><b><font size="-1">SEI<br>2</font></b></td>
<td><b><font size="-1">ADC<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">ADC<br>abx 4*</font></b></td>
<td><b><font size="-1">ROR<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">RRA<br>abx 7</font></b></td>
</tr>

<tr>
<td><b><font size="+1">8x</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>imm 2</font></b></td>
<td><b><font size="-1">STA<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>imm 2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SAX<br>izx 6</font></b></td>
<td><b><font size="-1">STY<br>zp 3</font></b></td>
<td><b><font size="-1">STA<br>zp 3</font></b></td>
<td><b><font size="-1">STX<br>zp 3</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SAX<br>zp 3</font></b></td>
<td><b><font size="-1">DEY<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>imm 2</font></b></td>
<td><b><font size="-1">TXA<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#FF0000" size="-1"><i>XAA<br>imm 2</i></font></b></td>
<td><b><font size="-1">STY<br>abs 4</font></b></td>
<td><b><font size="-1">STA<br>abs 4</font></b></td>
<td><b><font size="-1">STX<br>abs 4</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SAX<br>abs 4</font></b></td>
</tr>

<tr>
<td><b><font size="+1">9x</font></b></td>
<td><b><font size="-1">BCC<br>rel 2*</font></b></td>
<td><b><font size="-1">STA<br>izy 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#0000FF" size="-1"><i>AHX<br>izy 6</i></font></b></td>
<td><b><font size="-1">STY<br>zpx 4</font></b></td>
<td><b><font size="-1">STA<br>zpx 4</font></b></td>
<td><b><font size="-1">STX<br>zpy 4</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SAX<br>zpy 4</font></b></td>
<td><b><font size="-1">TYA<br>2</font></b></td>
<td><b><font size="-1">STA<br>aby 5</font></b></td>
<td><b><font size="-1">TXS<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#0000FF" size="-1"><i>TAS<br>aby 5</i></font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#0000FF" size="-1"><i>SHY<br>abx 5</i></font></b></td>
<td><b><font size="-1">STA<br>abx 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#0000FF" size="-1"><i>SHX<br>aby 5</i></font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#0000FF" size="-1"><i>AHX<br>aby 5</i></font></b></td>
</tr>

<tr>
<td><b><font size="+1">Ax</font></b></td>
<td><b><font size="-1">LDY<br>imm 2</font></b></td>
<td><b><font size="-1">LDA<br>izx 6</font></b></td>
<td><b><font size="-1">LDX<br>imm 2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>izx 6</font></b></td>
<td><b><font size="-1">LDY<br>zp 3</font></b></td>
<td><b><font size="-1">LDA<br>zp 3</font></b></td>
<td><b><font size="-1">LDX<br>zp 3</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>zp 3</font></b></td>
<td><b><font size="-1">TAY<br>2</font></b></td>
<td><b><font size="-1">LDA<br>imm 2</font></b></td>
<td><b><font size="-1">TAX<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font color="#FF0000" size="-1"><i>LAX<br>imm 2</i></font></b></td>
<td><b><font size="-1">LDY<br>abs 4</font></b></td>
<td><b><font size="-1">LDA<br>abs 4</font></b></td>
<td><b><font size="-1">LDX<br>abs 4</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>abs 4</font></b></td>
</tr>

<tr>
<td><b><font size="+1">Bx</font></b></td>
<td><b><font size="-1">BCS<br>rel 2*</font></b></td>
<td><b><font size="-1">LDA<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>izy 5*</font></b></td>
<td><b><font size="-1">LDY<br>zpx 4</font></b></td>
<td><b><font size="-1">LDA<br>zpx 4</font></b></td>
<td><b><font size="-1">LDX<br>zpy 4</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>zpy 4</font></b></td>
<td><b><font size="-1">CLV<br>2</font></b></td>
<td><b><font size="-1">LDA<br>aby 4*</font></b></td>
<td><b><font size="-1">TSX<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAS<br>aby 4*</font></b></td>
<td><b><font size="-1">LDY<br>abx 4*</font></b></td>
<td><b><font size="-1">LDA<br>abx 4*</font></b></td>
<td><b><font size="-1">LDX<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">LAX<br>aby 4*</font></b></td>
</tr>

<tr>
<td><b><font size="+1">Cx</font></b></td>
<td><b><font size="-1">CPY<br>imm 2</font></b></td>
<td><b><font size="-1">CMP<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>imm 2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>izx 8</font></b></td>
<td><b><font size="-1">CPY<br>zp 3</font></b></td>
<td><b><font size="-1">CMP<br>zp 3</font></b></td>
<td><b><font size="-1">DEC<br>zp 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>zp 5</font></b></td>
<td><b><font size="-1">INY<br>2</font></b></td>
<td><b><font size="-1">CMP<br>imm 2</font></b></td>
<td><b><font size="-1">DEX<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">AXS<br>imm 2</font></b></td>
<td><b><font size="-1">CPY<br>abs 4</font></b></td>
<td><b><font size="-1">CMP<br>abs 4</font></b></td>
<td><b><font size="-1">DEC<br>abs 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>abs 6</font></b></td>
</tr>

<tr>
<td><b><font size="+1">Dx</font></b></td>
<td><b><font size="-1">BNE<br>rel 2*</font></b></td>
<td><b><font size="-1">CMP<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">CMP<br>zpx 4</font></b></td>
<td><b><font size="-1">DEC<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>zpx 6</font></b></td>
<td><b><font size="-1">CLD<br>2</font></b></td>
<td><b><font size="-1">CMP<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">CMP<br>abx 4*</font></b></td>
<td><b><font size="-1">DEC<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">DCP<br>abx 7</font></b></td>
</tr>

<tr>
<td><b><font size="+1">Ex</font></b></td>
<td><b><font size="-1">CPX<br>imm 2</font></b></td>
<td><b><font size="-1">SBC<br>izx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>imm 2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>izx 8</font></b></td>
<td><b><font size="-1">CPX<br>zp 3</font></b></td>
<td><b><font size="-1">SBC<br>zp 3</font></b></td>
<td><b><font size="-1">INC<br>zp 5</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>zp 5</font></b></td>
<td><b><font size="-1">INX<br>2</font></b></td>
<td><b><font size="-1">SBC<br>imm 2</font></b></td>
<td><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">SBC<br>imm 2</font></b></td>
<td><b><font size="-1">CPX<br>abs 4</font></b></td>
<td><b><font size="-1">SBC<br>abs 4</font></b></td>
<td><b><font size="-1">INC<br>abs 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>abs 6</font></b></td>
</tr>

<tr>
<td><b><font size="+1">Fx</font></b></td>
<td><b><font size="-1">BEQ<br>rel 2*</font></b></td>
<td><b><font size="-1">SBC<br>izy 5*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">KIL</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>izy 8</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>zpx 4</font></b></td>
<td><b><font size="-1">SBC<br>zpx 4</font></b></td>
<td><b><font size="-1">INC<br>zpx 6</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>zpx 6</font></b></td>
<td><b><font size="-1">SED<br>2</font></b></td>
<td><b><font size="-1">SBC<br>aby 4*</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>2</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>aby 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">NOP<br>abx 4*</font></b></td>
<td><b><font size="-1">SBC<br>abx 4*</font></b></td>
<td><b><font size="-1">INC<br>abx 7</font></b></td>
<td bgcolor="#E0E0E0"><b><font size="-1">ISC<br>abx 7</font></b></td>
</tr>
</tbody>
    */
}
