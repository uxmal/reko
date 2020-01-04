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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        private static Dictionary<Mnemonic, Mnemonic> CreateVexMapping()
        {
            return new Dictionary<Mnemonic, Mnemonic>
            {
                { Mnemonic.illegal, Mnemonic.illegal },
                { Mnemonic.addsd, Mnemonic.vaddsd },
                { Mnemonic.addpd, Mnemonic.vaddpd },
                { Mnemonic.addsubpd, Mnemonic.vaddsubpd },
                { Mnemonic.addsubps, Mnemonic.vaddsubps },
                { Mnemonic.cvtsi2sd, Mnemonic.vcvtsi2sd },
                { Mnemonic.cvtsi2ss, Mnemonic.vcvtsi2ss },
                { Mnemonic.cvttpd2dq, Mnemonic.vcvttpd2dq },
                { Mnemonic.cvtdq2pd, Mnemonic.vcvtdq2pd },
                { Mnemonic.cvtpd2dq, Mnemonic.vcvtpd2dq },
                { Mnemonic.movapd, Mnemonic.vmovapd },
                { Mnemonic.movaps, Mnemonic.vmovaps },
                { Mnemonic.movlps, Mnemonic.vmovlps },
                { Mnemonic.movlpd, Mnemonic.vmovlpd },
                { Mnemonic.movsd, Mnemonic.vmovsd },
                { Mnemonic.movss, Mnemonic.vmovss },
                { Mnemonic.xorpd, Mnemonic.vxorpd },
                { Mnemonic.xorps, Mnemonic.vxorps },
                { Mnemonic.unpckhpd, Mnemonic.vunpckhpd },
                { Mnemonic.unpckhps, Mnemonic.vunpckhps },

                //$TODO: should be in the decoder.
                { Mnemonic.vhaddpd, Mnemonic.vhaddpd },
                { Mnemonic.vhaddps, Mnemonic.vhaddps },
                { Mnemonic.vhsubpd, Mnemonic.vhsubpd },
                { Mnemonic.vhsubps, Mnemonic.vhsubps },
                { Mnemonic.vlddqu, Mnemonic.vlddqu }, 
                { Mnemonic.vmovlps, Mnemonic.vmovlps},
                { Mnemonic.vpacksswb, Mnemonic.vpacksswb },
                { Mnemonic.vpslld, Mnemonic.vpslld },
                { Mnemonic.vpunpckhqdq, Mnemonic.vpunpckhqdq },
                { Mnemonic.vpunpcklqdq, Mnemonic.vpunpcklqdq },
            };
        }
    }
}
