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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.NativeInterface;
using Reko.Core.Types;
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

        public override int OpcodeAsInteger
        {
            get { return info.Opcode; }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            nInstr.Render(writer, options);
        }
    }

    public class AArch64Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic;
        public Mnemonic shiftCode;
        public MachineOperand shiftAmount;
        public VectorData vectorData;

        public override int OpcodeAsInteger => (int)Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            int iOp = WriteMnemonic(writer);
            if (Operands == null || Operands.Length == 0)
                return;
            writer.Tab();
            RenderOperand(Operands[iOp++], writer, options);
            for (; iOp < Operands.Length; ++iOp)
            {
                var op = Operands[iOp];
                writer.WriteChar(',');
                RenderOperand(op, writer, options);
            }
            if (this.shiftCode == Mnemonic.Invalid)
                return;
            if (shiftCode == Mnemonic.lsl && (shiftAmount is ImmediateOperand imm && imm.Value.IsIntegerZero))
                return;
            writer.WriteChar(',');
            writer.WriteOpcode(shiftCode.ToString());
            writer.WriteChar(' ');
            RenderOperand(shiftAmount, writer, options);
        }

        private int WriteMnemonic(MachineInstructionWriter writer)
        {
            if (Mnemonic == Mnemonic.b && Operands[0] is ConditionOperand cop)
            {
                writer.WriteOpcode($"b.{cop.Condition.ToString().ToLower()}");
                return 1;
            }
            writer.WriteOpcode(Mnemonic.ToString());
            return 0;
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case RegisterOperand reg:
                WriteRegister(reg.Register, writer);
                break;
            case ImmediateOperand imm:
                if (imm.Width.Domain == Domain.Real)
                {
                    writer.WriteFormat($"#{imm.Value}");
                }
                else
                {
                    int v = imm.Value.ToInt32();
                    if (0 <= v && v <= 9)
                        writer.WriteFormat($"#{imm.Value.ToInt32()}");
                    else
                        writer.WriteFormat($"#&{imm.Value.ToUInt32():X}");
                }
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

        private void WriteRegister(RegisterStorage reg, MachineInstructionWriter writer)
        {
            int elemSize;
            string elemName;
            switch (vectorData)
            {
            case VectorData.F32:
            case VectorData.I32:
                elemSize = 32;
                elemName = "s";
                break;
            case VectorData.F16:
            case VectorData.I16:
                elemSize = 16;
                elemName = "h";
                break;
            default:
                writer.WriteString(reg.Name);
                return;
            }
            writer.WriteFormat("v{0}.", reg.Name.Substring(1));
            int nElems = (int)reg.BitSize / elemSize;
            writer.WriteFormat("{0}{1}", nElems, elemName);
        }
    }
}

