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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using BitSet = Decompiler.Core.Lib.BitSet;

namespace Decompiler.Arch.X86
{

	// X86 flag masks.

	[Flags]
	public enum FlagM : byte
	{
		SF = 1,             // sign
		CF = 2,             // carry
		ZF = 4,             // zero
		DF = 8,             // direction
		
        OF = 16,            // overflow
        PF = 32,            // parity
        FPUF = 64,          // FPU flags
	}


	public class IntelArchitecture : IProcessorArchitecture
	{
		private BitSet implicitRegs;
		private ProcessorMode mode;
		private List<MachineFlags> flagGroups;

		public IntelArchitecture(ProcessorMode mode)
		{
			this.mode = mode;
			this.flagGroups = new List<MachineFlags>();
		
			implicitRegs = CreateRegisterBitset();
			implicitRegs[Registers.cs.Number] = true;
			implicitRegs[Registers.ss.Number] = true;
			implicitRegs[Registers.sp.Number] = true;
			implicitRegs[Registers.esp.Number] = true;
		}

		public Address AddressFromSegOffset(X86State state, MachineRegister seg, uint offset)
		{
			if (mode == ProcessorMode.ProtectedFlat)
			{
				return new Address(offset);
			}
			else
			{
				return state.AddressFromSegOffset(seg, offset);
			}
		}

		public virtual Disassembler CreateDisassembler(ImageReader imageReader)
		{
			return new IntelDisassembler(imageReader, WordWidth);
		}

		public virtual Dumper CreateDumper()
		{
			return new IntelDumper(this);
		}

        public virtual Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

		public virtual BitSet CreateRegisterBitset()
		{
			return new BitSet((int) Registers.Max);
		}

		public virtual ProcessorState CreateProcessorState()
		{
			return new X86State();
		}

        public virtual Rewriter CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new X86Rewriter(this, host, (X86State) state, rdr, frame);
        }

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            return mode.CreateStackAccess(frame, offset, dataType);
        }

        //$REFACTOR: this into the different processor modes.
        public Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            var st = (X86State)state;
            if (WordWidth == PrimitiveType.Word16)
            {
                if (byteSize == PrimitiveType.Word16.Size)
                {
                    return new Address(state.GetRegister(Registers.cs).ToUInt16(), rdr.ReadLeUInt16());
                }
                else
                {
                    ushort off = rdr.ReadLeUInt16();
                    ushort seg = rdr.ReadLeUInt16();
                    return new Address(seg, off);
                }
            }
            else if (WordWidth == PrimitiveType.Word32)
            {
                return new Address(rdr.ReadLeUInt32());
            }
            else
                throw new ApplicationException("Unexpected word width: " + byteSize);
        }

		public MachineFlags GetFlagGroup(uint grf)
		{
			foreach (MachineFlags f in flagGroups)
			{
				if (f.FlagGroupBits == grf)
					return f;
			}

			PrimitiveType dt = IsSingleBit(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            MachineFlags fl = new MachineFlags(GrfToString(grf), grf, dt);
			flagGroups.Add(fl);
			return fl;
		}

		public MachineFlags GetFlagGroup(string name)
		{
			FlagM grf = 0;
			for (int i = 0; i < name.Length; ++i)
			{
				switch (name[i])
				{
				case 'S': grf |= FlagM.SF; break;
				case 'C': grf |= FlagM.CF; break;
				case 'Z': grf |= FlagM.ZF; break;
				case 'D': grf |= FlagM.DF; break;
				case 'O': grf |= FlagM.OF; break;
				default: return null;
				}
			}
			return GetFlagGroup((uint) grf);
		}

		public MachineRegister GetRegister(int i)
		{
			return Registers.GetRegister(i);
		}

		public MachineRegister GetRegister(string name)
		{
			MachineRegister r = Registers.GetRegister(name);
			if (r == MachineRegister.None)
				throw new ArgumentException(string.Format("'{0}' is not a register name.", name));
			return r;
		}

        public bool TryGetRegister(string name, out MachineRegister reg)
        {
            reg = Registers.GetRegister(name);
            return (reg != MachineRegister.None);
        }


		public BitSet ImplicitArgumentRegisters
		{
			get { return implicitRegs; }
		}

		public virtual string GrfToString(uint grf)
		{
			StringBuilder s = new StringBuilder();
			for (int r = Registers.S.Number; grf != 0; ++r, grf >>= 1)
			{
				if ((grf & 1) != 0)
					s.Append(Registers.GetRegister(r).Name);
			}
			return s.ToString();
		}

		public bool IsSingleBit(uint u)
		{
			if (u == 0)
				return false;
			return (u & (u - 1)) == 0;
		}


		public ProcessorMode ProcessorMode
		{
			get { return mode; }
		}

        public PrimitiveType PointerType { get { return mode.PointerType; } }
        public PrimitiveType WordWidth { get { return mode.WordWidth; } }
        public PrimitiveType FramePointerType { get { return mode.FramePointerType; } }
        public MachineRegister StackRegister { get { return mode.StackRegister; } }
        
        public uint CarryFlagMask { get { return (uint)FlagM.CF; } }
    }
}
