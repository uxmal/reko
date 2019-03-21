#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using System;
using System.Text;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.PaRisc
{
    public class PaRiscInstruction : MachineInstruction
    {
        public InstrClass IClass { get; set; }
        public Opcode Opcode { get; set; }
        // Completers
        public int Coprocessor { get; set; }
        public bool Zero { get; set; }
        public SignExtension Sign { get; set; }
        public BaseRegMod BaseReg { get; set; }
        public bool Annul { get; set; }
        public FpFormat FpFmt { get; set; }

        public ConditionOperand Condition { get; set; }

        public MachineOperand[] Operands { get; set; }

        public override InstrClass InstructionClass => IClass;

        public override int OpcodeAsInteger => (int) Opcode;


        public override MachineOperand GetOperand(int i)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            WriteMnemonic(writer);
            if (Operands.Length == 0)
                return;
            writer.Tab();
            Operands[0].Write(writer, options);
            if (Operands.Length == 1)
                return;
            writer.WriteChar(',');
            Operands[1].Write(writer, options);
            if (Operands.Length == 2)
                return;
            writer.WriteChar(',');
            Operands[2].Write(writer, options);
            if (Operands.Length == 3)
                return;
            writer.WriteChar(',');
            Operands[3].Write(writer, options);
        }

        private void WriteMnemonic(MachineInstructionWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append(Opcode.ToString().Replace('_',','));
            if (FpFmt != FpFormat.None)
                sb.AppendFormat(",{0}", FpFmt);
            if (Zero)
                sb.Append(",z");
            if (Sign != SignExtension.None)
                sb.AppendFormat(",{0}", Sign);
            if (Condition != null)
                sb.AppendFormat(",{0}", Condition.Display);
            if (BaseReg != BaseRegMod.None)
                sb.AppendFormat(",{0}", BaseReg);
            if (Annul)
                sb.Append(",n");
            writer.WriteOpcode(sb.ToString());
        }
    }

    public enum SignExtension
    {
        None, s, u
    }

    public enum BaseRegMod
    {
        None, ma, mb, o
    }

    public enum FpFormat
    {
        None, sgl,dbl,quad 
    }
}