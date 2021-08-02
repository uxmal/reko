#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    public class AArch64Instruction : MachineInstruction
    {
        public Mnemonic shiftCode;
        public MachineOperand? shiftAmount;
        public VectorData vectorData;

        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            int iOp = WriteMnemonic(renderer);
            if (Operands == null || Operands.Length == 0)
                return;
            renderer.Tab();
            RenderOperand(Operands[iOp++], renderer, options);
            for (; iOp < Operands.Length; ++iOp)
            {
                var op = Operands[iOp];
                renderer.WriteChar(',');
                RenderOperand(op, renderer, options);
            }
            if (this.shiftCode == Mnemonic.Invalid)
                return;
            if (shiftCode == Mnemonic.lsl && (shiftAmount is ImmediateOperand imm && imm.Value.IsIntegerZero))
                return;
            renderer.WriteChar(',');
            renderer.WriteMnemonic(shiftCode.ToString());
            renderer.WriteChar(' ');
            RenderOperand(shiftAmount!, renderer, options);
        }

        private int WriteMnemonic(MachineInstructionRenderer renderer)
        {
            if (Mnemonic == Mnemonic.b && Operands[0] is ConditionOperand cop)
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
            case RegisterOperand reg:
                renderer.BeginOperand();
                WriteRegister(reg.Register, renderer);
                renderer.EndOperand();
                break;
            case ImmediateOperand imm:
                renderer.BeginOperand();
                if (imm.Width.Domain == Domain.Real)
                {
                    renderer.WriteFormat($"#{imm.Value}");
                }
                else
                {
                    int v = imm.Value.ToInt32();
                    if (0 <= v && v <= 9)
                        renderer.WriteFormat($"#{imm.Value.ToInt32()}");
                    else
                        renderer.WriteFormat($"#&{imm.Value.ToUInt32():X}");
                }
                renderer.EndOperand();
                break;
            case AddressOperand addrOp:
                renderer.BeginOperand();
                ulong linAddr = addrOp.Address.ToLinear();
                renderer.WriteAddress($"#&{linAddr:X}", addrOp.Address);
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
                renderer.WriteString(reg.Name);
                return;
            }
            renderer.WriteFormat("v{0}.", reg.Name.Substring(1));
            int nElems = (int)reg.BitSize / elemSize;
            renderer.WriteFormat("{0}{1}", nElems, elemName);
        }
    }
}

