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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryOperand = Reko.Arch.PowerPC.MemoryOperand;

namespace Reko.UnitTests.Arch.PowerPC
{
    class InstructionBuilder
    {
        private PowerPcArchitecture arch;
        private Address addr;

        public InstructionBuilder(PowerPcArchitecture arch, Address start)
        {
            this.arch = arch;
            this.Instructions = new List<PowerPcInstruction>();
            this.r0 = new RegisterOperand(arch.Registers[0]);
            this.r1 = new RegisterOperand(arch.Registers[1]);
            this.r2 = new RegisterOperand(arch.Registers[2]);
            this.r3 = new RegisterOperand(arch.Registers[3]);
            this.r4 = new RegisterOperand(arch.Registers[4]);
            this.r5 = new RegisterOperand(arch.Registers[5]);
            this.r6 = new RegisterOperand(arch.Registers[6]);
            this.r7 = new RegisterOperand(arch.Registers[7]);
            this.r8 = new RegisterOperand(arch.Registers[8]);
            this.r9 = new RegisterOperand(arch.Registers[9]);
            this.r10 = new RegisterOperand(arch.Registers[10]);
            this.r11 = new RegisterOperand(arch.Registers[11]);
            this.r12 = new RegisterOperand(arch.Registers[12]);
            this.r13 = new RegisterOperand(arch.Registers[13]);
            this.r14 = new RegisterOperand(arch.Registers[14]);
            this.r15 = new RegisterOperand(arch.Registers[15]);
            this.r16 = new RegisterOperand(arch.Registers[16]);
            this.r17 = new RegisterOperand(arch.Registers[17]);
            this.r18 = new RegisterOperand(arch.Registers[18]);
            this.r19 = new RegisterOperand(arch.Registers[19]);
            this.r20 = new RegisterOperand(arch.Registers[20]);
            this.r21 = new RegisterOperand(arch.Registers[21]);
            this.r22 = new RegisterOperand(arch.Registers[22]);
            this.r23 = new RegisterOperand(arch.Registers[23]);
            this.r24 = new RegisterOperand(arch.Registers[24]);
            this.r25 = new RegisterOperand(arch.Registers[25]);
            this.r26 = new RegisterOperand(arch.Registers[26]);
            this.r27 = new RegisterOperand(arch.Registers[27]);
            this.r28 = new RegisterOperand(arch.Registers[28]);
            this.r29 = new RegisterOperand(arch.Registers[29]);
            this.r30 = new RegisterOperand(arch.Registers[30]);
            this.r31 = new RegisterOperand(arch.Registers[31]);

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

        public void Bctr()
        {
            Add(new PowerPcInstruction(Mnemonic.bcctr, new ImmediateOperand(Constant.Byte(0x20)), null, null, false));
        }

        public void Oris(RegisterOperand rA, RegisterOperand rS, ushort val)
        {
            Add(new PowerPcInstruction(Mnemonic.oris, rA, rS, new ImmediateOperand(Constant.Word16(val)), false));
        }

        public void Add(RegisterOperand rT, RegisterOperand rA, RegisterOperand rB)
        {
            Add(new PowerPcInstruction(Mnemonic.add, rT, rA, rB, false));
        }

        public void Add_(RegisterOperand rT, RegisterOperand rA, RegisterOperand rB)
        {
            Add(new PowerPcInstruction(Mnemonic.add, rT, rA, rB, true));
        }

        internal void Lbzu(RegisterOperand rD, short offset, RegisterOperand rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lbzu, rD, new MemoryOperand(rD.Register.DataType, rA.Register, Constant.Int16(offset)), null, false));
        }

        public void Lis(RegisterOperand r, ushort uimm)
        {
            Add(new PowerPcInstruction(Mnemonic.oris, r, r, new ImmediateOperand(Constant.Word16(uimm)), false));
        }

        public void Lwzu(RegisterOperand rD, short offset, RegisterOperand rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lwzu, rD, new MemoryOperand(rD.Register.DataType, rA.Register, Constant.Int16(offset)), null, false));
        }

        public void Lwz(RegisterOperand rD, short offset, RegisterOperand rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lwz, rD, new MemoryOperand(rD.Register.DataType, rA.Register, Constant.Int16(offset)), null, false));
        }

        public void Mtctr(RegisterOperand r)
        {
            Add(new PowerPcInstruction(Mnemonic.mtctr, r, null, null, false));
        }

        public void Stbux(RegisterOperand rS, RegisterOperand rA, RegisterOperand rB)
        {
            Add(new PowerPcInstruction(Mnemonic.stbux, rS, rA, rB, false));
        }

        public void Stbu(RegisterOperand rS, short offset, RegisterOperand rA)
        {
            Add(new PowerPcInstruction(Mnemonic.stbu, rS, Mem(rA, offset), null, false));
        }

        private MemoryOperand Mem(RegisterOperand baseReg, short offset)
        {
            return new MemoryOperand(baseReg.Register.DataType, baseReg.Register, Constant.Int16(offset));
        }
    }
}
