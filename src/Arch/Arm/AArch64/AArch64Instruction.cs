#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    public class AArch64Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        public Mnemonic ShiftCode { get; set; }
        public MachineOperand? ShiftAmount { get; set; }
        public VectorData VectorData { get; set; }


        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            int iOp = WriteMnemonic(renderer);
            if (Operands is null || Operands.Length == 0)
                return;
            renderer.Tab();
            RenderOperand(Operands[iOp++], renderer, options);
            for (; iOp < Operands.Length; ++iOp)
            {
                var op = Operands[iOp];
                renderer.WriteChar(',');
                RenderOperand(op, renderer, options);
            }
            if (this.ShiftCode == Mnemonic.Invalid)
                return;
            if (ShiftCode == Mnemonic.lsl && (ShiftAmount is Constant imm && imm.IsIntegerZero))
                return;
            renderer.WriteChar(',');
            renderer.WriteMnemonic(ShiftCode.ToString());
            renderer.WriteChar(' ');
            RenderOperand(ShiftAmount!, renderer, options);
        }

        private int WriteMnemonic(MachineInstructionRenderer renderer)
        {
            if (Mnemonic == Mnemonic.b && Operands[0] is ConditionOperand<ArmCondition> cop)
            {
                renderer.WriteMnemonic($"b.{cop.Condition.ToString().ToLower()}");
                return 1;
            }
            renderer.WriteMnemonic(Mnemonic.ToString());
            return 0;
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case RegisterStorage reg:
                renderer.BeginOperand();
                WriteRegister(reg, renderer);
                renderer.EndOperand();
                break;
            case Constant imm:
                renderer.BeginOperand();
                if (imm.DataType.Domain == Domain.Real)
                {
                    renderer.WriteFormat($"#{imm}");
                }
                else
                {
                    long v = imm.ToInt64();
                    if (0 <= v && v <= 9)
                        renderer.WriteFormat($"#{v}");
                    else
                        renderer.WriteFormat($"#&{imm.ToUInt64():X}");
                }
                renderer.EndOperand();
                break;
            case Address addrOp:
                renderer.BeginOperand();
                ulong linAddr = addrOp.ToLinear();
                renderer.WriteAddress($"#&{linAddr:X}", addrOp);
                renderer.EndOperand();
                break;
            default:
                op.Render(renderer, options);
                break;
            }
        }

        private void WriteRegister(RegisterStorage reg, MachineInstructionRenderer renderer)
        {
            int elemSize;
            string elemName;
            switch (VectorData)
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
                renderer.WriteString(reg.Name);
                return;
            }
            renderer.WriteFormat("v{0}.", reg.Name.Substring(1));
            int nElems = (int)reg.BitSize / elemSize;
            renderer.WriteFormat("{0}{1}", nElems, elemName);
        }
    }
}

