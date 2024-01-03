#region License
/* 
 * Copyright (C) 2018-2024 Stefano Moioli <smxdev4@gmail.com>.
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Xbox
{
    /// <summary>
    /// Platform implementation for original Xbox console.
    /// </summary>
    public class XboxPlatform : Platform
    {
        public XboxPlatform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "xbox")
        {
            EnsureTypeLibraries(this.PlatformIdentifier);
            this.StructureMemberAlignment = 8;
        }

        public override string DefaultCallingConvention => throw new NotImplementedException();


        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Storage? PossibleReturnValue(IEnumerable<Storage> storages)
        {
            var retStorage = storages
                .OfType<RegisterStorage>()
                .FirstOrDefault(r => r.Name == "eax");
            return retStorage;
        }
    }
}
