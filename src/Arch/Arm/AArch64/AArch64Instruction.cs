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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Arm.AArch64
{
    public class Arm64Instruction : MachineInstruction
    {
        private INativeInstruction nInstr;
        private NativeInstructionInfo info;

        public Arm64Instruction(INativeInstruction nInstr)
        {
            this.nInstr = nInstr;
            nInstr.GetInfo(out info);
            this.Address = Address.Ptr32((uint)info.LinearAddress);
            this.Length = (int)info.Length;
        }

        //$REVIEW: is this really needed? nInstr is a ComInstance object,
        // provided by the CLR, and probably has its own finalizer.
        ~Arm64Instruction()
        {
            Marshal.ReleaseComObject(nInstr);
            nInstr = null;
        }

        public override InstructionClass InstructionClass
        {
            get { return (InstructionClass)info.InstructionClass; }
        }

        public override bool IsValid
        {
            get { return this.InstructionClass != InstructionClass.Invalid; }
        }

        public override int OpcodeAsInteger
        {
            get { return info.Opcode; }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotSupportedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            nInstr.Render(writer, options);
        }
    }

    public class AArch64Instruction : MachineInstruction
    {
        public Opcode opcode;
        public MachineOperand[] ops;
        public Opcode shiftCode;
        public MachineOperand shiftAmount;

        public override InstructionClass InstructionClass { get; }

        public override bool IsValid { get; }

        public override int OpcodeAsInteger => (int)opcode;

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(opcode.ToString());
            if (ops == null || ops.Length == 0)
                return;
            writer.Tab();
            RenderOperand(ops[0], writer, options);
            foreach (var op in ops.Skip(1))
            {
                writer.WriteString(",");
                RenderOperand(op, writer, options);
            }
        }

        private void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case ImmediateOperand imm:
                int v = imm.Value.ToInt32();
                if (0 <= v && v <= 9)
                    writer.WriteFormat($"#{imm.Value.ToInt32()}");
                else
                    writer.WriteFormat($"#&{imm.Value.ToUInt32():X}");
                break;
            case AddressOperand addrOp:
                ulong linAddr = addrOp.Address.ToLinear();
                writer.WriteAddress($"#&{linAddr:X}", addrOp.Address);
                break;
            default:
                op.Write(writer, options);
                break;
            }
        }
    }
}

