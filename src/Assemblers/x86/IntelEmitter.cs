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

using Decompiler.Arch.Intel;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Assemblers.x86
{
    public class IntelEmitter : Emitter
    {
        private MachineRegister segOverride;
        private PrimitiveType dataWidthSeg;	// Default data width for this segment.
        private PrimitiveType addrWidthSeg;	// Default address width for this segment.
        private PrimitiveType addrWidth;	// width of the address of this opcode.

        public IntelEmitter()
		{
			segOverride = MachineRegister.None;
		}

        public PrimitiveType AddressWidth
        {
            get { return addrWidth; }
            set { addrWidth = value; }
        }


		public void EmitOpcode(int b, PrimitiveType dataWidth)
		{
			if (SegmentOverride != MachineRegister.None)
			{
				byte bOv;
				if (SegmentOverride == Registers.es) bOv = 0x26; else
				if (SegmentOverride == Registers.cs) bOv = 0x2E; else
				if (SegmentOverride == Registers.ss) bOv = 0x36; else
				if (SegmentOverride == Registers.ds) bOv = 0x3E; else
				if (SegmentOverride == Registers.fs) bOv = 0x64; else
				if (SegmentOverride == Registers.gs) bOv = 0x65; else
				throw new ArgumentOutOfRangeException("Invalid segment register: " + SegmentOverride);
				EmitByte(bOv);
				SegmentOverride = MachineRegister.None;
			}
			if (IsDataWidthOverridden(dataWidth))
			{
				EmitByte(0x66);
			}
			if (AddressWidth != null && AddressWidth != SegmentAddressWidth)
			{
				EmitByte(0x67);
			}
			EmitByte(b);
		}

        private bool IsDataWidthOverridden(PrimitiveType dataWidth)
        {
            return dataWidth != null &&
                dataWidth.Domain != Domain.Real &&
                dataWidth.Size != 1 &&
                dataWidth != SegmentDataWidth;
        }


        public PrimitiveType SegmentAddressWidth
        {
            get { return addrWidthSeg; }
            set { addrWidthSeg = value; }
        }

        public PrimitiveType SegmentDataWidth
        {
            get { return dataWidthSeg; }
            set { dataWidthSeg = value; }
        }

        public MachineRegister SegmentOverride
        {
            get { return segOverride; }
            set { segOverride = value; }
        }

    }
}
