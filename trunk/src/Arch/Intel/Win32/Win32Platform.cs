/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using System;

namespace Decompiler.Arch.Intel.Win32
{
	public class Win32Platform : Platform
	{
		private IProcessorArchitecture arch;
		private SystemService int3svc;

		public Win32Platform(IProcessorArchitecture arch)
		{
			this.arch = arch;
			int3svc = new SystemService();
			int3svc.SyscallInfo.Vector = 3;
			int3svc.SyscallInfo.RegisterValues = new RegValue[0];
			int3svc.Name = "int3";
			int3svc.Signature = new ProcedureSignature(null, new Identifier[0]);
			int3svc.Characteristics = new ProcedureCharacteristics();
		}

		public override SystemService FindService(int vector, ProcessorState state)
		{
			if (int3svc.SyscallInfo.Matches(vector, state))
				return int3svc;
			throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
		}
	}
}
