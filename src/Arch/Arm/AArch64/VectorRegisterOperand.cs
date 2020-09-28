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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public class VectorRegisterOperand : MachineOperand
    {
        public VectorRegisterOperand(PrimitiveType dt, RegisterStorage reg) : base(dt)
        {
            this.VectorRegister = reg;
            this.Index = -1;
        }

        public RegisterStorage VectorRegister { get; }
        public VectorData ElementType { get; set; }
        public int Index { get; set; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            WriteName(Width.BitSize, VectorRegister, ElementType, Index, writer);
            if (Index >= 0)
            {
                writer.WriteFormat("[{0}]", Index);
            }
        }

        public static void WriteName(int bitSize, RegisterStorage VectorRegister, VectorData ElementType, int Index, MachineInstructionWriter writer)
        {
            writer.WriteString(VectorRegister.Name);
            writer.WriteChar('.');
            switch (ElementType)
            {
            case VectorData.I8:
                if (Index >= 0)
                    writer.WriteString("b");
                else if (bitSize == 64)
                    writer.WriteString("8b");
                else
                    writer.WriteString("16b");
                break;
            case VectorData.I16:
                if (Index >= 0)
                    writer.WriteString("h");
                else if (bitSize == 64)
                    writer.WriteString("4h");
                else
                    writer.WriteString("8h");
                break;
            case VectorData.I32:
                if (Index >= 0)
                    writer.WriteString("s");
                else if (bitSize == 64)
                    writer.WriteString("2s");
                else
                    writer.WriteString("4s");
                break;
            case VectorData.I64:
                if (Index >= 0)
                    writer.WriteString("d");
                else if (bitSize == 64)
                    writer.WriteString("1d");
                else
                    writer.WriteString("2d");
                break;
            default:
                throw new NotImplementedException($"{ElementType}");
            }
        }
    }
}
