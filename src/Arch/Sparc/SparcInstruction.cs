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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public class SparcInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        public bool Annul => (InstructionClass & InstrClass.Annul) != 0;

        public Prediction Prediction { get; set; }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RenderMnemonic(writer);
            if (Mnemonic == Mnemonic.@return)
            {
                writer.Tab();
                RenderOperand(Operands[0], writer, options);
                RenderOperand(Operands[1], writer, options);
            }
            else
            {
                RenderOperands(writer, options);
            }
        }

        private void RenderMnemonic(MachineInstructionWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append(Mnemonic.ToString());
            if (Annul)
                sb.Append(",a");
            if (Prediction == Prediction.NotTaken)
                sb.Append(",pn");
            if (Prediction == Prediction.Taken)
                sb.Append(",pt");
            writer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case RegisterOperand reg:
                writer.WriteFormat("%{0}", reg.Register.Name);
                return;
            default:
                op.Write(writer, options);
                return;
            }
        }
    }

    [Flags]
    public enum Prediction
    {
        None = 0,
        NotTaken = 1,
        Taken = 2,
    }
}
