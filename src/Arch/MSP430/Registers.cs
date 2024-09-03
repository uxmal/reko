#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Msp430
{
    public class Registers
    {
        public RegisterStorage[] GpRegisters { get; }
        public Dictionary<string, RegisterStorage> ByName { get; }

        public RegisterStorage pc { get; }
        public RegisterStorage sp { get; }

        /// <summary>
        /// Processor Status Register.
        /// </summary>
        public RegisterStorage sr { get; }

        public FlagGroupStorage C { get; }
        public FlagGroupStorage NZC { get; }
        public FlagGroupStorage V { get; }
        public FlagGroupStorage VNZC { get; }
        
        public Registers(PrimitiveType dtGpReg)
        {
            pc = new RegisterStorage("pc", 0, 0, dtGpReg);
            sp = new RegisterStorage("sp", 1, 0, dtGpReg);
            sr = new RegisterStorage("sr", 2, 0, dtGpReg);
            GpRegisters = new RegisterStorage[]
            {
                pc,
                sp,
                sr,
                new RegisterStorage("cg2", 3, 0, dtGpReg),
            }.Concat(
                Enumerable.Range(4, 12)
                .Select(i => new RegisterStorage(
                    string.Format("r{0}", i), i, 0, dtGpReg)))

                .ToArray();
            C = new FlagGroupStorage(sr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            NZC = new FlagGroupStorage(sr, (uint) (FlagM.NF | FlagM.ZF | FlagM.CF), "NZC", sr.DataType);
            V = new FlagGroupStorage(sr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            VNZC = new FlagGroupStorage(sr, (uint) (FlagM.VF | FlagM.NF | FlagM.ZF | FlagM.CF), "VNZC", sr.DataType);

            ByName = GpRegisters.ToDictionary(r => r.Name);
        }
    }

    [Flags]
    public enum FlagM
    {
        VF = 0x100,
        NF = 0x004,
        ZF = 0x002,
        CF = 0x001
    }
}