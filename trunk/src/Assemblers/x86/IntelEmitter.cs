#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
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
        public IntelEmitter()
		{
			SegmentOverride = RegisterStorage.None;
		}

        // width of the address of this opcode.
        public PrimitiveType AddressWidth { get; set; }
        
        // Default address width for this segment.
        public PrimitiveType SegmentAddressWidth { get; set; }

        // Default data width for this segment.
        public PrimitiveType SegmentDataWidth { get; set; }

        public RegisterStorage SegmentOverride { get; set; }


		public void EmitOpcode(int b, PrimitiveType dataWidth)
		{
			if (SegmentOverride != RegisterStorage.None)
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
				SegmentOverride = RegisterStorage.None;
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
	}
}
