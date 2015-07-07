#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kInstruction : MachineInstruction
    {
        public Opcode code;
        public PrimitiveType dataWidth;
        public MachineOperand op1;
        public MachineOperand op2;
        public MachineOperand op3;

        public override int OpcodeAsInteger { get { return (int)code; } }

        public override void Render(MachineInstructionWriter writer)
        {
            if (code == Opcode.illegal && op1 != null && writer.Platform != null)
            {
                var imm = op1 as M68kImmediateOperand;
                // MacOS uses invalid opcodes to invoke Macintosh Toolbox services. 
                // We may have to generalize the Platform API to allow specifying 
                // the opcode of the invoking instruction, to disambiguate from 
                // "legitimate" TRAP calls.
                var svc = writer.Platform.FindService((int)imm.Constant.ToUInt32(), null);
                if (svc != null)
                {
                    writer.Write(svc.Name);
                    return;
                }
            }
            if (dataWidth != null)
            {
                writer.WriteOpcode(string.Format("{0}{1}", code, DataSizeSuffix(dataWidth)));
            }
            else
            {
                writer.WriteOpcode(code.ToString());
            }
            writer.Tab();
            if (op1 != null)
            {
                op1.Write(false, writer);
                if (op2 != null)
                {
                    writer.Write(',');
                    op2.Write(false, writer);
                }
            }
        }

        private string DataSizeSuffix(PrimitiveType dataWidth)
        {
            switch (dataWidth.BitSize)
            {
            case 8: return ".b";
            case 16: return ".w";
            case 32: return ".l";
            default: throw new InvalidOperationException(string.Format("Unsupported data width {0}.", dataWidth.BitSize));
            }
        }
    }
}
