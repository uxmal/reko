#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class AeonInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer);
            if (Operands.Length > 0)
            {
                base.RenderOperands(renderer, options);
            }
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer)
        {
            var sMnemonic = MnemonicAsString
                .Replace("__", "?")
                .Replace("_", ".");
            renderer.WriteMnemonic(sMnemonic);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case ImmediateOperand imm:
                if (this.Mnemonic == Mnemonic.Nyi && operand == Operands[0])
                {
                    RenderOpcodeToBinary(imm.Value.ToUInt32(), renderer);
                    return;
                }
                if (imm.Value.DataType.Domain == Domain.SignedInt)
                {
                    var value = imm.Value.ToInt32();
                    if (value < 0)
                    {
                        renderer.WriteChar('-');
                        value = -value;
                    }
                    renderer.WriteFormat("0x{0:X}", value);
                }
                else
                {
                    renderer.WriteFormat("0x{0:X}", imm.Value.ToUInt32());
                }
                if ((this.Mnemonic == Mnemonic.bn_ori ||
                    this.Mnemonic == Mnemonic.bg_ori ||
                    this.Mnemonic == Mnemonic.bt_addi__ ||
                    this.Mnemonic == Mnemonic.bn_addi ||
                    this.Mnemonic == Mnemonic.bg_addi) && 
                    imm.Width.BitSize == 32)
                {
                    renderer.WriteString("@lo");
                }
                break;
            case AddressOperand addr:
                if (this.Mnemonic == Mnemonic.bg_movhi ||
                    this.Mnemonic == Mnemonic.bn_movhi__ ||
                    this.Mnemonic == Mnemonic.bt_movhi__)
                {
                    var uAddr = addr.Address.ToUInt32();
                    renderer.WriteAddress($"0x{uAddr:X}@hi", addr.Address);
                    return;
                }
                goto default;
            default:
                base.RenderOperand(operand, renderer, options);
                break;
            }
        }

        private static void RenderOpcodeToBinary(uint opcode, MachineInstructionRenderer renderer)
        {
            for (uint mask = 0b10_0000; mask != 0; mask >>= 1)
            {
                char ch = (opcode & mask) != 0 ? '1' : '0';
                renderer.WriteChar(ch);
            }
        }
    }

}