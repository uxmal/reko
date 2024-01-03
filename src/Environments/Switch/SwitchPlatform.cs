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
using Reko.Core.Hll.C;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Reko.Core.Loading;

namespace Reko.Environments.Switch
{
    public class SwitchPlatform : Platform
    {
        public SwitchPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "switch")
        {
            this.StructureMemberAlignment = 4;
            //$TODO: find out what registers are always trashed
        }

        public override string DefaultCallingConvention => "";

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            // Right now we are not aware of any way to locate main
            // on Switch binaries.
            return null;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            return null;
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(sAddress, out addr);
        }
    }
}
