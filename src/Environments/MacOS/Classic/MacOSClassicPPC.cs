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

using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Reko.Environments.MacOS.Classic
{
    public class MacOSClassicPPC : Platform
    {
        public MacOSClassicPPC(IServiceProvider services, IProcessorArchitecture arch) :
            base(services, arch, "macOsPpc")
        {
        }

        public override string DefaultCallingConvention => "";

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            var arch = (PowerPcArchitecture)Architecture;

            var trashed = new HashSet<RegisterStorage>
            {
                arch.Registers[0],
                arch.Registers[1],
                arch.FpRegisters[0],
                arch.lr,
                arch.ctr,
                arch.xer,
                arch.CrRegisters[0],
                arch.CrRegisters[1],
                arch.CrRegisters[5],
                arch.CrRegisters[7]
            };

            //GPR2 - GPR12
            foreach (var reg in RuntimeHelpers.GetSubArray(arch.Registers.ToArray(), new Range(2, 13)))
            {
                trashed.Add(reg);
            }

            //FPR0 - FPR13
            foreach(var reg in RuntimeHelpers.GetSubArray(arch.FpRegisters.ToArray(), new Range(1, 14)))
            {
                trashed.Add(reg);
            }

            return trashed;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override CallingConvention? GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
