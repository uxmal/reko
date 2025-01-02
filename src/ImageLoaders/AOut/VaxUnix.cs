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
using Reko.Core.Hll.C;
using Reko.Core.Machine;
using Reko.Core.Memory;
using System;
using System.Diagnostics;

namespace Reko.ImageLoaders.AOut
{
    public class VaxUnix : Platform
    {
        public VaxUnix(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "vax-unix")
        {
        }

        public override string DefaultCallingConvention => "";

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            foreach (var module in metadata.Modules.Values)
            {
                if (module.ServicesByVector.TryGetValue(vector, out var svc) &&
                    svc.Count == 1)
                {
                    return svc[0];
                }
            }
            Debug.WriteLine("*** Unknown vector {0}", vector);
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            case CBasicType.WChar_t: return 2;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ICallingConvention? GetCallingConvention(string? ccName)
        {
            return new VaxUnixCallingConvention(this.Architecture);
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
