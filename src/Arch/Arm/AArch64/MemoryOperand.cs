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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public Constant Offset;
        public RegisterStorage Index;
        public Mnemonic IndexExtend;
        public int IndexShift;
        public bool PreIndex;
        public bool PostIndex;

        public MemoryOperand(PrimitiveType dt)  : base(dt)
        {
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteChar('[');
            writer.WriteString(Base.Name);
            if (Offset != null && !Offset.IsIntegerZero)
            {
                if (PostIndex)
                {
                    writer.WriteChar(']');
                }
                writer.WriteChar(',');
                var off = Offset.ToInt32();
                if (off < 0)
                {
                    writer.WriteFormat("#-&{0:X}", -off);
                }
                else
                {
                    writer.WriteFormat("#&{0:X}", off);
                }
            }
            else if (Index != null)
            {
                writer.WriteChar(',');
                writer.WriteString(Index.Name);
                if (IndexExtend != Mnemonic.Invalid && (IndexExtend != Mnemonic.lsl || IndexShift != 0))
                {
                    writer.WriteChar(',');
                    writer.WriteOpcode(IndexExtend.ToString());
                    if (IndexShift != 0)
                    {
                        writer.WriteString($" #{IndexShift}");
                    }
                }
            }
            if (!PostIndex)
            {
                writer.WriteChar(']');
            }
            if (PreIndex)
            {
                writer.WriteChar('!');
            }
        }
    }
}
