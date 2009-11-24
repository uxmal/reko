/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kCodeWalker : CodeWalker
    {
        ProgramImage img;
        Platform platform;
        ProcessorState st;
        M68kDisassembler dasm;

        public M68kCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
        {
            this.img = img;
            this.dasm = new M68kDisassembler(img.CreateReader(addr));
            this.st = st;
        }

        public override Address Address
        {
            get { return dasm.Address; }
        }

        public override MachineInstruction WalkInstruction(ICodeWalkerListener listener)
        {
            Address addrInstr = dasm.Address;
            M68kInstruction instr = dasm.Disassemble();
            switch (instr.code)
            {
            default: throw new NotImplementedException(string.Format("Walking {0} not implemented yet.", instr.code));
            case Opcode.adda:
            case Opcode.addi:
            case Opcode.addq:
            case Opcode.bchg:
            case Opcode.lea:
            case Opcode.lsl:
            case Opcode.move:
            case Opcode.movea:
            case Opcode.movem:
            case Opcode.moveq:
            case Opcode.ori:
                break;
            case Opcode.bra:
                listener.OnJump(st, addrInstr, dasm.Address, ((AddressOperand)instr.op1).Address);
                break;
            }
            return instr;
        }
    }
}
