#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    class InstructionBuilder
    {
        private Address addr;

        public InstructionBuilder(Address start)
        {
            this.Instructions = new List<PowerPcInstruction>();
            this.r0 = new RegisterOperand(Registers.r0);
            this.r1 = new RegisterOperand(Registers.r1);
            this.r2 = new RegisterOperand(Registers.r2);
            this.r3 = new RegisterOperand(Registers.r3);
            this.r4 = new RegisterOperand(Registers.r4);
            this.r5 = new RegisterOperand(Registers.r5);
            this.r6 = new RegisterOperand(Registers.r6);
            this.r7 = new RegisterOperand(Registers.r7);
            this.r8 = new RegisterOperand(Registers.r8);
            this.r9 = new RegisterOperand(Registers.r9);
            this.r10 = new RegisterOperand(Registers.r10);
            this.r11 = new RegisterOperand(Registers.r11);
            this.r12 = new RegisterOperand(Registers.r12);
            this.r13 = new RegisterOperand(Registers.r13);
            this.r14 = new RegisterOperand(Registers.r14);
            this.r15 = new RegisterOperand(Registers.r15);
            this.r16 = new RegisterOperand(Registers.r16);
            this.r17 = new RegisterOperand(Registers.r17);
            this.r18 = new RegisterOperand(Registers.r18);
            this.r19 = new RegisterOperand(Registers.r19);
            this.r20 = new RegisterOperand(Registers.r20);
            this.r21 = new RegisterOperand(Registers.r21);
            this.r22 = new RegisterOperand(Registers.r22);
            this.r23 = new RegisterOperand(Registers.r23);
            this.r24 = new RegisterOperand(Registers.r24);
            this.r25 = new RegisterOperand(Registers.r25);
            this.r26 = new RegisterOperand(Registers.r26);
            this.r27 = new RegisterOperand(Registers.r27);
            this.r28 = new RegisterOperand(Registers.r28);
            this.r29 = new RegisterOperand(Registers.r29);
            this.r30 = new RegisterOperand(Registers.r30);
            this.r31 = new RegisterOperand(Registers.r31);

            this.addr = start;
        }

        public RegisterOperand r0 { get; set; }
        public RegisterOperand r1 { get; set; }
        public RegisterOperand r2 { get; set; }
        public RegisterOperand r3 { get; set; }
        public RegisterOperand r4 { get; set; }
        public RegisterOperand r5 { get; set; }
        public RegisterOperand r6 { get; set; }
        public RegisterOperand r7 { get; set; }
        public RegisterOperand r8 { get; set; }
        public RegisterOperand r9 { get; set; }
        public RegisterOperand r10 { get; set; }
        public RegisterOperand r11 { get; set; }
        public RegisterOperand r12 { get; set; }
        public RegisterOperand r13 { get; set; }
        public RegisterOperand r14 { get; set; }
        public RegisterOperand r15 { get; set; }
        public RegisterOperand r16 { get; set; }
        public RegisterOperand r17 { get; set; }
        public RegisterOperand r18 { get; set; }
        public RegisterOperand r19 { get; set; }
        public RegisterOperand r20 { get; set; }
        public RegisterOperand r21 { get; set; }
        public RegisterOperand r22 { get; set; }
        public RegisterOperand r23 { get; set; }
        public RegisterOperand r24 { get; set; }
        public RegisterOperand r25 { get; set; }
        public RegisterOperand r26 { get; set; }
        public RegisterOperand r27 { get; set; }
        public RegisterOperand r28 { get; set; }
        public RegisterOperand r29 { get; set; }
        public RegisterOperand r30 { get; set; }
        public RegisterOperand r31 { get; set; }

        public List<PowerPcInstruction> Instructions { get; private set; }

        private void Add(PowerPcInstruction instr)
        {
            instr.Address = addr;
            addr += 4;
            Instructions.Add(instr);
        }

        public void Oris(RegisterOperand rA, RegisterOperand rS, ushort val)
        {
            Add(new PowerPcInstruction(Opcode.oris, rA, rS, new ImmediateOperand(Constant.Word16(val)), false));
        }

        public void Add(RegisterOperand rT, RegisterOperand rA, RegisterOperand rB)
        {
            Add(new PowerPcInstruction(Opcode.add, rT, rA, rB, false));
        }

        public void Add_(RegisterOperand rT, RegisterOperand rA, RegisterOperand rB)
        {
            Add(new PowerPcInstruction(Opcode.add, rT, rA, rB, true));
        }
    }
}
