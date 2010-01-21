/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Machine
{
	/// <summary>
	/// Flyweight representation of processor condition codes/flags
	/// </summary>
	public class MachineFlags : MachineRegisterBase
	{
		private uint grf;

		public MachineFlags(string name, uint grfBits, PrimitiveType dt) : base(name, dt)
		{
			this.grf = grfBits;
		}

		public uint FlagGroupBits
		{
			get { return grf; }
		}


		public override int SubregisterOffset(MachineRegisterBase subReg)
		{
			MachineFlags subFlags = subReg as MachineFlags;
			if (subFlags != null)
			{
				if ((grf & subFlags.grf) != 0)
					return 0;
			}
			return -1;
		}

	}
}
