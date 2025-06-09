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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public RegisterStorage? Base;
        public Constant? Offset;
        public RegisterStorage? Index;
        public Mnemonic IndexExtend;
        public int IndexShift;
        public bool PreIndex;
        public bool PostIndex;

        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('[');
            renderer.WriteString(Base!.Name);
            if (Offset is not null && !Offset.IsIntegerZero)
            {
                if (PostIndex)
                {
                    renderer.WriteChar(']');
                }
                renderer.WriteChar(',');
                var off = Offset.ToInt32();
                if (off < 0)
                {
                    renderer.WriteFormat("#-&{0:X}", -off);
                }
                else
                {
                    renderer.WriteFormat("#&{0:X}", off);
                }
            }
            else if (Index is not null)
            {
                if (PostIndex)
                {
                    renderer.WriteChar(']');
                }
                renderer.WriteChar(',');
                renderer.WriteString(Index.Name);
                if (IndexExtend != Mnemonic.Invalid && (IndexExtend != Mnemonic.lsl || IndexShift != 0))
                {
                    renderer.WriteChar(',');
                    renderer.WriteMnemonic(IndexExtend.ToString());
                    if (IndexShift != 0)
                    {
                        renderer.WriteString($" #{IndexShift}");
                    }
                }
            }
            if (!PostIndex)
            {
                renderer.WriteChar(']');
            }
            if (PreIndex)
            {
                renderer.WriteChar('!');
            }
        }
    }
}
