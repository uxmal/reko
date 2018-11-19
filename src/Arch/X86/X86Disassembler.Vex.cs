#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Dictionary<Opcode, Opcode> CreateVexMapping()
        {
            return new Dictionary<Opcode, Opcode>
            {
                { Opcode.addsd, Opcode.vaddsd },
                { Opcode.addpd, Opcode.vaddpd },
                { Opcode.cvtsi2sd, Opcode.vcvtsi2sd },
                { Opcode.cvtsi2ss, Opcode.vcvtsi2ss },
                { Opcode.movapd, Opcode.vmovapd },
                { Opcode.movaps, Opcode.vmovaps },
                { Opcode.movsd, Opcode.vmovsd },
                { Opcode.movss, Opcode.vmovss },
                { Opcode.xorpd, Opcode.vxorpd },
                { Opcode.xorps, Opcode.vxorps },
				{ Opcode.movlps, Opcode.vmovlps },
				{ Opcode.movlpd, Opcode.vmovlpd },
				{ Opcode.addsubpd, Opcode.vaddsubpd }
            };
        }
    }
}
