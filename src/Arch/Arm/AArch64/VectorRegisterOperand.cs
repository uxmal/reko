#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Arm.AArch64
{
    public class VectorRegisterOperand : AbstractMachineOperand
    {
        public VectorRegisterOperand(PrimitiveType dt, RegisterStorage reg) : base(dt)
        {
            this.VectorRegister = reg;
            this.Index = -1;
        }

        public VectorRegisterOperand(
            PrimitiveType dt, 
            RegisterStorage reg,
            VectorData elementType) : this(dt, reg, elementType, -1)
        {
            this.VectorRegister = reg;
            this.ElementType = elementType;
            this.Index = -1;
        }

        public VectorRegisterOperand(
            PrimitiveType dt,
            RegisterStorage reg,
            VectorData elementType,
            int index) : base(dt)
        {
            this.VectorRegister = reg;
            this.ElementType = elementType;
            this.Index = index;
        }

        public RegisterStorage VectorRegister { get; }
        public VectorData ElementType { get; set; }
        public int Index { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            WriteName(Width.BitSize, VectorRegister, ElementType, Index, renderer);
            if (Index >= 0)
            {
                renderer.WriteFormat("[{0}]", Index);
            }
        }

        public static void WriteName(int bitSize, RegisterStorage VectorRegister, VectorData ElementType, int Index, MachineInstructionRenderer renderer)
        {
            renderer.WriteString(VectorRegister.Name);
            renderer.WriteChar('.');
            switch (ElementType)
            {
            case VectorData.I8:
                if (Index >= 0)
                    renderer.WriteString("b");
                else if (bitSize == 64)
                    renderer.WriteString("8b");
                else
                    renderer.WriteString("16b");
                break;
            case VectorData.I16:
                if (Index >= 0)
                    renderer.WriteString("h");
                else if (bitSize == 64)
                    renderer.WriteString("4h");
                else
                    renderer.WriteString("8h");
                break;
            case VectorData.I32:
                if (Index >= 0)
                    renderer.WriteString("s");
                else if (bitSize == 64)
                    renderer.WriteString("2s");
                else
                    renderer.WriteString("4s");
                break;
            case VectorData.I64:
                if (Index >= 0)
                    renderer.WriteString("d");
                else if (bitSize == 64)
                    renderer.WriteString("1d");
                else
                    renderer.WriteString("2d");
                break;
            case VectorData.I128:
                if (Index >= 0)
                    renderer.WriteString("q");
                else if (bitSize == 64)
                    renderer.WriteString("1q??");
                else
                    renderer.WriteString("1q");
                break;
            default:
                throw new NotImplementedException($"{ElementType}");
            }
        }
    }
}
