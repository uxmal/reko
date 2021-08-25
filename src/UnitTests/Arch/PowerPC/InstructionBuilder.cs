#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
            this.r0 = arch.Registers[0];
            this.r1 = arch.Registers[1];
            this.r2 = arch.Registers[2];
            this.r3 = arch.Registers[3];
            this.r4 = arch.Registers[4];
            this.r5 = arch.Registers[5];
            this.r6 = arch.Registers[6];
            this.r7 = arch.Registers[7];
            this.r8 = arch.Registers[8];
            this.r9 = arch.Registers[9];
            this.r10 = arch.Registers[10];
            this.r11 = arch.Registers[11];
            this.r12 = arch.Registers[12];
            this.r13 = arch.Registers[13];
            this.r14 = arch.Registers[14];
            this.r15 = arch.Registers[15];
            this.r16 = arch.Registers[16];
            this.r17 = arch.Registers[17];
            this.r18 = arch.Registers[18];
            this.r19 = arch.Registers[19];
            this.r20 = arch.Registers[20];
            this.r21 = arch.Registers[21];
            this.r22 = arch.Registers[22];
            this.r23 = arch.Registers[23];
            this.r24 = arch.Registers[24];
            this.r25 = arch.Registers[25];
            this.r26 = arch.Registers[26];
            this.r27 = arch.Registers[27];
            this.r28 = arch.Registers[28];
            this.r29 = arch.Registers[29];
            this.r30 = arch.Registers[30];
            this.r31 = arch.Registers[31];

            this.addr = start;
        }

        public RegisterStorage r0 { get; set; }
        public RegisterStorage r1 { get; set; }
        public RegisterStorage r2 { get; set; }
        public RegisterStorage r3 { get; set; }
        public RegisterStorage r4 { get; set; }
        public RegisterStorage r5 { get; set; }
        public RegisterStorage r6 { get; set; }
        public RegisterStorage r7 { get; set; }
        public RegisterStorage r8 { get; set; }
        public RegisterStorage r9 { get; set; }
        public RegisterStorage r10 { get; set; }
        public RegisterStorage r11 { get; set; }
        public RegisterStorage r12 { get; set; }
        public RegisterStorage r13 { get; set; }
        public RegisterStorage r14 { get; set; }
        public RegisterStorage r15 { get; set; }
        public RegisterStorage r16 { get; set; }
        public RegisterStorage r17 { get; set; }
        public RegisterStorage r18 { get; set; }
        public RegisterStorage r19 { get; set; }
        public RegisterStorage r20 { get; set; }
        public RegisterStorage r21 { get; set; }
        public RegisterStorage r22 { get; set; }
        public RegisterStorage r23 { get; set; }
        public RegisterStorage r24 { get; set; }
        public RegisterStorage r25 { get; set; }
        public RegisterStorage r26 { get; set; }
        public RegisterStorage r27 { get; set; }
        public RegisterStorage r28 { get; set; }
        public RegisterStorage r29 { get; set; }
        public RegisterStorage r30 { get; set; }
        public RegisterStorage r31 { get; set; }

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

        public void Oris(RegisterStorage rA, RegisterStorage rS, ushort val)
        {
            Add(new PowerPcInstruction(Mnemonic.oris, rA, rS, new ImmediateOperand(Constant.Word16(val)), false));
        }

        public void Add(RegisterStorage rT, RegisterStorage rA, RegisterStorage rB)
        {
            Add(new PowerPcInstruction(Mnemonic.add, rT, rA, rB, false));
        }

        public void Add_(RegisterStorage rT, RegisterStorage rA, RegisterStorage rB)
        {
            Add(new PowerPcInstruction(Mnemonic.add, rT, rA, rB, true));
        }

        internal void Lbzu(RegisterStorage rD, short offset, RegisterStorage rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lbzu, rD, new MemoryOperand(rD.DataType, rA, offset), null, false));
        }

        public void Lis(RegisterStorage r, ushort uimm)
        {
            Add(new PowerPcInstruction(Mnemonic.oris, r, r, new ImmediateOperand(Constant.Word16(uimm)), false));
        }

        public void Lwzu(RegisterStorage rD, short offset, RegisterStorage rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lwzu, rD, new MemoryOperand(rD.DataType, rA, offset), null, false));
        }

        public void Lwz(RegisterStorage rD, short offset, RegisterStorage rA)
        {
            Add(new PowerPcInstruction(Mnemonic.lwz, rD, new MemoryOperand(rD.DataType, rA, offset), null, false));
        }

        public void Mtctr(RegisterStorage r)
        {
            Add(new PowerPcInstruction(Mnemonic.mtctr, r, null, null, false));
        }

        public void Stbux(RegisterStorage rS, RegisterStorage rA, RegisterStorage rB)
        {
            Add(new PowerPcInstruction(Mnemonic.stbux, rS, rA, rB, false));
        }

        public void Stbu(RegisterStorage rS, short offset, RegisterStorage rA)
        {
            Add(new PowerPcInstruction(Mnemonic.stbu, rS, Mem(rA, offset), null, false));
        }

        private MemoryOperand Mem(RegisterStorage baseReg, short offset)
        {
            return new MemoryOperand(baseReg.DataType, baseReg, offset);
        }
    }
}
