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
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Qualcomm
{
    public class MemoryOperand : AbstractMachineOperand
    {

        public MemoryOperand(PrimitiveType width) : base(width)
        {
        }

        public RegisterStorage? Base { get; set; }
        public int Offset { get; set; }
        public RegisterStorage? Index { get; set; }
        public int Shift { get; set; }

        /// <summary>
        /// AutoIncrement can either be a constant or a register.
        /// </summary>
        public object? AutoIncrement { get; set; }

        /// <summary>
        /// True if this is an absolute-set operation.
        /// </summary>
        public bool IsAbsoluteSet { get; set; }

        protected override void DoRender(MachineInstructionRenderer writer, MachineInstructionRendererOptions options)
        {
            writer.WriteString("mem");
            if (DataType.Size == 1)
                writer.WriteString("b");
            else if (DataType == PrimitiveType.Int16)
                writer.WriteString("h");
            else if (DataType.Size == 2)
                writer.WriteString("uh");
            else if (DataType.Size == 4)
                writer.WriteString("w");
            else if (DataType.Size == 8)
                writer.WriteString("d");
            else
                throw new System.NotImplementedException($"Unimplemented size {DataType.Size}");
            writer.WriteChar('(');
            if (Base is not null)
            {
                writer.WriteString(Base.Name);
                if (this.IsAbsoluteSet)
                {
                    writer.WriteFormat("={0:X8})", (uint) this.Offset);
                    return;
                }
                if (AutoIncrement is not null)
                {
                    writer.WriteString("++");
                    if (AutoIncrement is int uIncr)
                    {
                        writer.WriteFormat("#{0}", uIncr);
                    }
                    else
                    {
                        writer.WriteString(AutoIncrement.ToString());
                    }
                }
            }
            if (Index is not null)
            {
                if (Base is not null)
                {
                    writer.WriteString("+");
                }
                writer.WriteString(Index.Name);
                if (Shift > 0)
                {
                    writer.WriteFormat("<<#{0}", Shift);
                }
            }
            if (Base is not null)
            {
                if (Offset < 0)
                {
                    writer.WriteFormat("-{0}", -Offset);
                }
                else if (Offset > 0)
                {
                    writer.WriteFormat("+{0}", Offset);
                }
            }
            else
            {
                if (Index is not null)
                {
                    writer.WriteChar('+');
                }
                writer.WriteFormat("{0:X8}", (uint) Offset);
            }
            writer.WriteChar(')');
        }
    }
}