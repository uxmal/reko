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

using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
	/// <summary>
	/// Walks code backwards to find "dominating" comparisons against constants,
	/// which may provide vector table limits.
	/// </summary>
	public abstract class BackWalker
	{
		private ProgramImage img;

		public BackWalker(ProgramImage img)
		{
			this.img = img;
		}

		protected ProgramImage Image
		{
			get { return img; }
		}

		public abstract List<BackwalkOperation> BackWalk(Address addrFrom, IBackWalkHost host);

		public static bool IsEvenPowerOfTwo(int n)
		{
			return n != 0 && (n & (n - 1)) == 0;
		}

		public abstract Address MakeAddress(PrimitiveType size, ImageReader rdr, ushort segBase);

		/// <summary>
		/// The register used to perform a table-dispatch switch.
		/// </summary>
		public abstract MachineRegister IndexRegister { get; }
	}
}
