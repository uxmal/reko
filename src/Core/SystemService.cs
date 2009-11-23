/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using System;

namespace Decompiler.Core
{
	public class SystemService
	{
		public string Name;
		public SyscallInfo SyscallInfo;
		public ProcedureSignature Signature;
		public ProcedureCharacteristics Characteristics;

		public ExternalProcedure CreateExternalProcedure(IProcessorArchitecture arch)
		{
			return new ExternalProcedure(Name, Signature);
		}
	}

	public class RegValue
	{
		public MachineRegister Register;
		public int Value;
	}

	public struct SyscallInfo
	{
		public int Vector;
		public RegValue [] RegisterValues;

		public bool Matches(int vector, ProcessorState state)
		{
			if (Vector != vector)
				return false;
			for (int i = 0; i < RegisterValues.Length; ++i)
			{
				Constant v = state.Get(RegisterValues[i].Register);
				if (v == null || v == Constant.Invalid)
					return false;
				if (v.ToUInt32() != RegisterValues[i].Value)
					return false;
			}
			return true;
		}
	}	
}
