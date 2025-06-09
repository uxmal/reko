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

using System;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.M6800.M6809
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public Mode AccessMode;
        public int Offset;
        public RegisterStorage? Base;
        public RegisterStorage? Index;
        public bool Indirect;

        public MemoryOperand(PrimitiveType dt) : base(dt)
        {
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            string fmt;
            int offset;
            string regName = "";
            if (Indirect)
                renderer.WriteChar('[');
            switch (AccessMode)
            {
            case Mode.Direct:
                renderer.WriteFormat(">${0:X2}", Offset);
                break;
            case Mode.ConstantOffset:
                offset = Offset; 
                if (Base is null)
                {
                    fmt = "${0:X4}";
                }
                else if (offset < 0)
                {
                    offset = -offset;
                    fmt = "-${0:X2},{1}";
                    regName = Base.Name;
                }
                else if (Offset == 0)
                {
                    fmt = ",{1}";
                    regName = Base.Name;
                }
                else
                {
                    if (offset < 0x0100)
                        fmt = "${0:X2},{1}";
                    else
                        fmt = "${0:X4},{1}";
                    regName = Base.Name;
                }
                renderer.WriteFormat(fmt, offset, regName);
                break;
            case Mode.AccumulatorOffset:
                renderer.WriteFormat("{0},{1}", Index!.Name, Base!.Name);
                break;
            case Mode.PostInc1:
                renderer.WriteFormat(",{0}+", Base!.Name);
                break;
            case Mode.PostInc2:
                renderer.WriteFormat(",{0}++", Base!.Name);
                break;
            case Mode.PreDec1:
                renderer.WriteFormat(",-{0}", Base!.Name);
                break;
            case Mode.PreDec2:
                renderer.WriteFormat(",--{0}", Base!.Name);
                break;
            default:
               throw new NotImplementedException($"Unimplemented address mode {AccessMode}.");
            }
            if (Indirect)
                renderer.WriteChar(']');
        }

        public enum Mode
        {
            Invalid,
            Direct,
            ConstantOffset,
            AccumulatorOffset,
            PostInc1,
            PostInc2,
            PreDec1,
            PreDec2,
        }
    }
}