#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Libraries.Microchip
{
    public static class PICRemapMemDomains
    {
        private readonly static Hashtable subdom2dom = new Hashtable(20)
        {
            { MemorySubDomain.SFR,      MemoryDomain.Data },
            { MemorySubDomain.GPR,      MemoryDomain.Data },
            { MemorySubDomain.DPR,      MemoryDomain.Data },
            { MemorySubDomain.NNMR,     MemoryDomain.Data },
            { MemorySubDomain.Emulator, MemoryDomain.Data },
            { MemorySubDomain.Linear,   MemoryDomain.Data },
            { MemorySubDomain.DMA,      MemoryDomain.Data },
            { MemorySubDomain.Code,             MemoryDomain.Prog },
            { MemorySubDomain.ExtCode,          MemoryDomain.Prog },
            { MemorySubDomain.EEData,           MemoryDomain.Prog },
            { MemorySubDomain.DeviceConfig,     MemoryDomain.Prog },
            { MemorySubDomain.DeviceConfigInfo, MemoryDomain.Prog },
            { MemorySubDomain.DeviceInfoAry,    MemoryDomain.Prog },
            { MemorySubDomain.UserID,           MemoryDomain.Prog },
            { MemorySubDomain.DeviceID,         MemoryDomain.Prog },
            { MemorySubDomain.RevisionID,       MemoryDomain.Prog },
            { MemorySubDomain.Debugger,         MemoryDomain.Prog },
            { MemorySubDomain.Calib,            MemoryDomain.Prog },
            { MemorySubDomain.Test,             MemoryDomain.Prog },
        };

        public static MemoryDomain GetDomain(this MemorySubDomain subdom)
            => (MemoryDomain)(subdom2dom[subdom] ?? MemoryDomain.Unknown);

    }

}
