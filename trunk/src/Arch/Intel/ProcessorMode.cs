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
using Decompiler.Core.Types;
using System;

namespace Decompiler.Arch.Intel
{
	public class ProcessorMode 
	{
		private PrimitiveType width;

		public static readonly ProcessorMode None = new ProcessorMode(null);
		public static readonly ProcessorMode Real = new ProcessorMode(PrimitiveType.Word16);
		public static readonly ProcessorMode ProtectedSegmented = new ProcessorMode(PrimitiveType.Word16);
		public static readonly ProcessorMode ProtectedFlat = new FlatMode();

		protected ProcessorMode(PrimitiveType w)
		{
			this.width = w;
		}

		public PrimitiveType WordWidth
		{
			get { return width; }
		}

		public virtual Address AddressFromSegOffset(IntelState state, MachineRegister seg, uint offset)
		{
			return state.AddressFromSegOffset(seg, offset);
		}
	}

	internal class FlatMode : ProcessorMode
	{
		internal FlatMode() : base(PrimitiveType.Word32)
		{
		}

		public override Address AddressFromSegOffset(IntelState state, MachineRegister seg, uint offset)
		{
			return new Address(offset);
		}
	}
}
