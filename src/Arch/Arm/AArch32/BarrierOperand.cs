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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch32
{
    public class BarrierOperand : MachineOperand
    {
        public BarrierOperand(BarrierOption barrierOption) : base(PrimitiveType.Byte)
        {
            this.Option = barrierOption;
        }

        public BarrierOption Option { get; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString(Option.ToString().ToLower());
        }
    }

    public enum BarrierOption
    {
        OSHLD = 0b0001,
        OSHST = 0b0010,
        OSH = 0b0011,
        NSHLD = 0b0101,
        NSHST = 0b0110,
        NSH = 0b0111,
        ISHLD = 0b1001,
        ISHST = 0b1010,
        ISH = 0b1011,
        LD = 0b1101,
        ST = 0b1110,
        SY = 0b1111,
    }
}
