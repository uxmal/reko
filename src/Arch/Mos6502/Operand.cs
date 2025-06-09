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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mos6502
{
    public class Operand : AbstractMachineOperand
    {
        public AddressMode Mode;
        public RegisterStorage? Register;
        public Constant? Offset;

        public Operand(PrimitiveType size)
            : base(size)
        {
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            int o = Offset is not null ? (int)Offset.ToUInt32() : 0;
            string fmt;
            switch (Mode)
            {
            case AddressMode.Accumulator: return;        // Implicit, never displayed
            case AddressMode.Immediate: fmt = "#${0:X2}"; break;
            case AddressMode.ZeroPage: fmt = "${0:X2}"; break;
            case AddressMode.ZeroPageX: 
            case AddressMode.ZeroPageY: fmt = "${0:X2},{1}"; break;
            case AddressMode.Relative:
            case AddressMode.Absolute: fmt = "${0:X4}"; break;
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY: fmt = "${0:X4},{1}"; break;
            case AddressMode.Indirect: fmt = "(${0:X4})"; break;
            case AddressMode.IndexedIndirect: fmt = "(${0:X2},{1})"; break;
            case AddressMode.IndirectIndexed: fmt = "(${0:X2}),{1}"; break;

            case AddressMode.AbsoluteLong: fmt = "${0:X6}"; break;
            case AddressMode.AbsoluteLongX: fmt = "${0:X6},{1}"; break;
            case AddressMode.AbsoluteIndexedIndirect: fmt = "(${0:X4},{1})"; break;
            case AddressMode.DirectPage: fmt = "${0:X2}"; break;
            case AddressMode.DirectPageX:
            case AddressMode.DirectPageY: fmt = "${0:X2},{1}"; break;
            case AddressMode.DirectPageIndexedIndirectX: fmt = "(${0:X2},x)"; break;
            case AddressMode.DirectPageIndirect: fmt = "(${0:X2})"; break;
            case AddressMode.DirectPageIndirectIndexedY: fmt = "(${0:X2}),y"; break;
            case AddressMode.DirectPageIndirectLong: fmt = "[${0:X2}]"; break;
            case AddressMode.DirectPageIndirectLongIndexedY: fmt = "[${0:X6}],y"; break;
            case AddressMode.StackRelative: fmt = "${0:X2},s"; break;
            case AddressMode.StackRelativeIndirectIndexedY: fmt = "(${0:X2},s),y"; break;
            default: throw new NotSupportedException($"Mode {Mode} not supported.");
            }
            renderer.WriteString(string.Format(fmt, o, Register));
        }
    }

    public enum AddressMode
    {
        Immediate,          // #$AA
        ZeroPage,           // $AA
        ZeroPageX,          // $AA,x
        ZeroPageY,          // $AA,y
        Relative,           // $AABB
        Absolute,           // $AABB
        AbsoluteX,          // $AABB,x
        AbsoluteY,          // $AABB,y
        Indirect,           // ($AABB)
        IndexedIndirect,    // $(AA,x)
        IndirectIndexed,    // $(AA),y
        Accumulator,        // a

        // New 65816 modes
        DirectPage,                     // $AA
        DirectPageX,                    // $AA,X
        DirectPageY,                    // $AA,Y
        DirectPageIndirect,             // ($AA)
        DirectPageIndexedIndirectX,     // ($AA,X)
        DirectPageIndirectIndexedY,     // ($AA),Y
        DirectPageIndirectLong,         // [$AA]
        DirectPageIndirectLongIndexedY, // [$AA],Y
        AbsoluteLong,                   // $AABBCC
        AbsoluteLongX,                  // $AABBCC,X
        AbsoluteIndexedIndirect,        // $(AABB,X)
        AbsoluteIndirectLong,           // [$AABBCC]
        StackRelative,                  // $AA,S
        StackRelativeIndirectIndexedY,  // ($AA,S),Y
    }
}
