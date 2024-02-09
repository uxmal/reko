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

// Gameboy info: https://gbdev.io/pandocs/

namespace Reko.Environments.Gameboy
{
    /// <summary>
    /// Provides support for the Nintendo Gameboy platform.
    /// </summary>
    public class GameboyPlatform : Platform
    {
        public GameboyPlatform(IServiceProvider services, IProcessorArchitecture arch) 
            : base(services, arch, "gameboy")
        {
            this.StructureMemberAlignment = 1;
        }

        public override string DefaultCallingConvention
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
