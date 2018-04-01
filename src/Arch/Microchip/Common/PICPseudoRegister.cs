#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
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
using Reko.Libraries.Microchip;

namespace Reko.Arch.Microchip.Common
{
    public class PICPseudoRegister
    {

        SFRDef sfr = new SFRDef();

        private PICPseudoRegister()
        {
        }

        public static PICRegisterStorage CreatePseudoRegister(string regName, int nmmrID, params PICRegisterBitFieldStorage[] fields)
        {
            var ps = new PICPseudoRegister();
            ps.sfr = new SFRDef()
            {
                CName = regName,
                Desc = $"Pseudo-register {regName}",
                NMMRID = nmmrID.ToString(),
                AddrFormatted = "0",
                MCLR = "00000000",
                POR = "00000000",
                Access = "nnnnnnnn",
                NzWidthFormatted = "8"
            };
            var reg = new PICRegisterStorage(ps.sfr, 0);
            return reg;
        }

    }

}
