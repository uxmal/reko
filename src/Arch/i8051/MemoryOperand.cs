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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.i8051
{
    public class MemoryOperand : MachineOperand
    {
        private MemoryOperand() : base(PrimitiveType.Byte)
        {

        }

        public static MemoryOperand Direct(Expression ea)
        {
            return new MemoryOperand { DirectAddress = ea };
        }

        public static MemoryOperand Indirect(Storage reg)
        {
            return new MemoryOperand { Register = reg };
        }

        public static MemoryOperand Indexed(Expression @base, RegisterStorage idx)
        {
            return new MemoryOperand { DirectAddress = @base, Index = idx };
        }

        public static MemoryOperand Indexed(Storage @base, RegisterStorage idx)
        {
            return new MemoryOperand { Register = @base, Index = idx };
        }

        public Expression DirectAddress { get; set; }
        public Storage Register { get; set; }
        public RegisterStorage Index { get; set; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Index != null)
            {
                writer.WriteString("@");
                writer.WriteString(Register.Name);
                writer.WriteString("+");
                writer.WriteString(Index.Name);
            }
            else
            {
                if (DirectAddress != null)
                    writer.WriteString($"[{DirectAddress}]");
                else
                    writer.WriteString($"@{Register.Name}");
            }
        }
    }
}