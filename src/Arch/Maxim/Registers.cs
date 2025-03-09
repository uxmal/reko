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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Maxim;

public static class Registers
{
    public static Dictionary<string, RegisterStorage>? RegsByName { get; internal set; }
    public static Dictionary<StorageDomain, RegisterStorage>? RegsByDomain { get; internal set; }
 
    public static RegisterStorage[] Accumulators { get; internal set; }
    public static RegisterStorage[] DataPointers { get; internal set; }
    public static RegisterStorage[] Prefixes { get; internal set; }
    public static RegisterStorage AP { get; }
    public static RegisterStorage APC { get; internal set; }
    public static RegisterStorage GRL { get; internal set; }
    public static RegisterStorage IC { get; internal set; }
    public static RegisterStorage IIR { get; internal set; }
    public static RegisterStorage IMR { get; internal set; }
    public static RegisterStorage MDP { get; internal set; }
    public static RegisterStorage PSF { get; internal set; }
    public static RegisterStorage SC { get; internal set; }
    public static RegisterStorage CKCN { get; internal set; }
    public static RegisterStorage WDCN { get; internal set; }
    public static RegisterStorage Acc { get; internal set; }
    public static RegisterStorage SP { get; internal set; }
    public static RegisterStorage IV { get; internal set; }
    public static RegisterStorage OFFS { get; internal set; }
    public static RegisterStorage IP { get; internal set; }
    public static RegisterStorage BP { get; internal set; }
    public static RegisterStorage DPC { get; internal set; }
    public static RegisterStorage FP { get; internal set; }
    public static RegisterStorage GR { get; internal set; }
    public static RegisterStorage GRH { get; internal set; }
    public static RegisterStorage GRXL { get; internal set; }
    public static RegisterStorage GRS { get; internal set; }

    static Registers()
    {
        var factory = new StorageFactory();
        Accumulators = factory.RangeOfReg(16, n => $"a{n}", PrimitiveType.Word16);
        DataPointers = factory.RangeOfReg(2, n => $"dp{n}", PrimitiveType.Word16);
        Prefixes = factory.RangeOfReg(8, n => $"pfx{n}", PrimitiveType.Word16);

           AP = factory.Reg16("AP");
   APC = factory.Reg16("APC");
   GRL = factory.Reg16("GRL");
   IC = factory.Reg16("IC");
   IIR = factory.Reg16("IIR");
   IMR = factory.Reg16("IMR");
   MDP = factory.Reg16("MDP");
   PSF = factory.Reg16("PSF");
   SC = factory.Reg16("SC");
   CKCN = factory.Reg16("CKCN");
   WDCN = factory.Reg16("WDCN");
   Acc = factory.Reg16("Acc");
   SP = factory.Reg16("SP");
   IV = factory.Reg16("IV");
   OFFS = factory.Reg16("OFFS");
   IP = factory.Reg16("IP");
   BP = factory.Reg16("BP");
   DPC = factory.Reg16("DPC");
   FP = factory.Reg16("FP");
   GR = factory.Reg16("GR");
   GRH = factory.Reg16("GRH");
   GRXL = factory.Reg16("GRXL");
   GRS = factory.Reg16("GRS");
}
}
