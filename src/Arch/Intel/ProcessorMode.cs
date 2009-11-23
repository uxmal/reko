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

using Decompiler.Core;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Arch.Intel
{
	public class ProcessorMode 
	{
		private PrimitiveType wordSize;
        private PrimitiveType framePtrSize;
        private PrimitiveType pointerType;

		public static readonly ProcessorMode None = new ProcessorMode(null, null, null);
        public static readonly ProcessorMode Real = new ProcessorMode(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32);
        public static readonly ProcessorMode ProtectedSegmented = new ProcessorMode(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32);
		public static readonly ProcessorMode ProtectedFlat = new FlatMode();

		protected ProcessorMode(PrimitiveType wordSize, PrimitiveType framePointerType, PrimitiveType pointerType)
		{
			this.wordSize = wordSize;
            this.framePtrSize = framePointerType;
            this.pointerType = pointerType;
		}

        public virtual Address AddressFromSegOffset(IntelState state, MachineRegister seg, uint offset)
        {
            return state.AddressFromSegOffset(seg, offset);
        }

        public PrimitiveType FramePointerType
        {
            get { return framePtrSize; }
        }

        public PrimitiveType PointerType
        {
            get { return pointerType; }
        }

		public PrimitiveType WordWidth
		{
			get { return wordSize; }
		}


        public virtual bool IsPointerRegister(MachineRegister machineRegister)
        {
            return machineRegister == Registers.bx ||
                machineRegister == Registers.sp ||
                machineRegister == Registers.bp ||
                machineRegister == Registers.si ||
                machineRegister == Registers.di;
        }
    }

	internal class FlatMode : ProcessorMode
	{
		internal FlatMode() : base(PrimitiveType.Word32, PrimitiveType.Pointer32, PrimitiveType.Pointer32)
		{
		}

		public override Address AddressFromSegOffset(IntelState state, MachineRegister seg, uint offset)
		{
			return new Address(offset);
		}

        public override bool IsPointerRegister(MachineRegister machineRegister)
        {
            return machineRegister.DataType.BitSize == PointerType.BitSize;
        }
	}
}
