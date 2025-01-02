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
using Reko.Core.Memory;
using System;
using System.Collections.Generic;

namespace Reko.Environments.Wii
{
    public class WiiPlatform : Platform {

		public WiiPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "wii")
        {
            this.StructureMemberAlignment = 4;
            //$TODO: find out what registers are always trashed
        }

        public override string DefaultCallingConvention => "";

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory) {
			//$TODO: implement some services;
			return null;
		}

		public override int GetBitSizeFromCBasicType(CBasicType cb) {
			throw new NotImplementedException();
		}

		public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName) {
			throw new NotImplementedException();
		}
	}
}
