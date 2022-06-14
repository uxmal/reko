#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using Reko.Core.Expressions;
using Reko.Core;
using Reg = Reko.Core.RegisterStorage;

namespace Reko.Arch.SuperH
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public AddressingMode mode;
        public Reg reg;
        public int disp;

        private MemoryOperand(PrimitiveType width) : base(width)
        {
            reg = null!;
        }

        public static MemoryOperand Indirect(PrimitiveType w, Reg r)                    { return new MemoryOperand(w) { mode = AddressingMode.Indirect, reg = r }; }
        public static MemoryOperand IndirectPostIncr(PrimitiveType w, Reg r)            { return new MemoryOperand(w) { mode = AddressingMode.IndirectPostIncr, reg = r }; }
        public static MemoryOperand IndirectPreDecr(PrimitiveType w, Reg r)             { return new MemoryOperand(w) { mode = AddressingMode.IndirectPreDecr, reg = r }; }
        public static MemoryOperand IndirectDisplacement(PrimitiveType w, Reg r, int d) { return new MemoryOperand(w) { mode = AddressingMode.IndirectDisplacement, reg = r, disp = d }; }
        public static MemoryOperand IndexedIndirect(PrimitiveType w, Reg r)             { return new MemoryOperand(w) { mode = AddressingMode.IndexedIndirect, reg = r }; }
        public static MemoryOperand GbrIndirectDisplacement(PrimitiveType w, int d)     { return new MemoryOperand(w) { mode = AddressingMode.GbrIndirectDisplacement, disp = d }; }
        public static MemoryOperand GbrIndexedIndirect(PrimitiveType w)                 { return new MemoryOperand(w) { mode = AddressingMode.GbrIndexedIndirect }; }
        public static MemoryOperand PcRelativeDisplacement(PrimitiveType w, int d)      { return new MemoryOperand(w) { mode = AddressingMode.PcRelativeDisplacement, disp = d }; }
        public static MemoryOperand DoubleIndirect(PrimitiveType w, Reg r, int d)       { return new MemoryOperand(w) { mode = AddressingMode.DoubleIndirect, reg = r, disp = d }; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch(mode)
            {
            case AddressingMode.Indirect: renderer.WriteString($"@{reg.Name}"); break;
            case AddressingMode.IndirectDisplacement: renderer.WriteString($"@({disp},{reg.Name})"); break;
            case AddressingMode.IndirectPreDecr: renderer.WriteString(string.Format("@-{0}", reg.Name)); break;
            case AddressingMode.IndirectPostIncr: renderer.WriteString(string.Format("@{0}+", reg.Name)); break;
            case AddressingMode.IndexedIndirect: renderer.WriteString(string.Format("@(r0,{0})", reg.Name)); break;
            case AddressingMode.GbrIndirectDisplacement: renderer.WriteString($"@({disp},gbr)"); break;
            case AddressingMode.GbrIndexedIndirect: renderer.WriteString("@(r0,gbr)"); break;
            case AddressingMode.PcRelativeDisplacement: renderer.WriteString(string.Format("@({0:X2},pc)", disp)); break;
            case AddressingMode.DoubleIndirect: renderer.WriteString(string.Format("@@({0},{1})", disp, this.reg.Name)); break;
            default: throw new NotImplementedException(string.Format("AddressingMode.{0}", mode));
            }
        }
    }

    public enum AddressingMode
    {
        Indirect,
        IndirectPostIncr,
        IndirectPreDecr,
        IndirectDisplacement,
        IndexedIndirect,
        GbrIndirectDisplacement,
        GbrIndexedIndirect,
        PcRelativeDisplacement,
        DoubleIndirect,
    }
}
