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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Msp430
{
    public class Msp430Instruction : MachineInstruction
    {
        public Mnemonics Mnemonic;
        public PrimitiveType dataWidth;
        public int repeatImm;
        public RegisterStorage repeatReg;

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RenderMnemonic(writer);
            RenderOperands(writer, options);
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            if (repeatReg != null)
            {
                writer.WriteMnemonic("rpt");
                writer.WriteString(" ");
                writer.WriteString(repeatReg.Name);
                writer.WriteString(" ");
            }
            else if (repeatImm > 1)
            {
                writer.WriteMnemonic("rpt");
                writer.WriteString(" #");
                writer.WriteString(repeatImm.ToString());
                writer.WriteString(" ");
            }
            var sb = new StringBuilder(Mnemonic.ToString());
            if (dataWidth != null)
            {
                sb.AppendFormat(".{0}", dataWidth.BitSize == 8
                    ? "b"
                    : dataWidth.BitSize == 16
                        ? "w"
                        : "a");
            }
            writer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (op is AddressOperand && (base.InstructionClass & InstrClass.Transfer) == 0)
            {
                writer.WriteString("&");
            }
            if (op is ImmediateOperand && Mnemonic != Mnemonics.call)
            {
                writer.WriteString("#");
            }
            op.Write(writer, options);
        }
    }
}