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
using Reko.Core.Memory;
using System;

namespace Reko.Environments.Hpux
{
    public class HpuxPlatform : Platform
    {
        private readonly RegisterStorage r27;

        public HpuxPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "hpux")
        {
            this.r27 = Architecture.GetRegister("r27") ?? throw new InvalidOperationException("Expected architecture to have r27.");
            this.StructureMemberAlignment = arch.WordWidth.Size;
        }

        public override string DefaultCallingConvention => "";

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return reg.Number == Architecture.StackRegister.Number ||
                reg.Number == r27.Number;
        }

        public override SystemService FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
