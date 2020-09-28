#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Arch.PowerPC;

namespace Reko.Environments.Wii
{
	public class WiiPlatform : Platform {

		public WiiPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "wii")
        {
		}

		public override string DefaultCallingConvention { get { return ""; } }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
			//$TODO: find out what registers are always preserved
			return new HashSet<RegisterStorage>();
		}

        public override CallingConvention GetCallingConvention(string ccName)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters() {
			//TODO: find out what registers are always trashed
			return new HashSet<RegisterStorage>();
		}

		public override SystemService FindService(int vector, ProcessorState state) {
			//$TODO: implement some services;
			return null;
		}

		public override int GetByteSizeFromCBasicType(CBasicType cb) {
			throw new NotImplementedException();
		}

		public override ExternalProcedure LookupProcedureByName(string moduleName, string procName) {
			throw new NotImplementedException();
		}
	}
}
