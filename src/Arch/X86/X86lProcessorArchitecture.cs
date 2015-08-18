#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using BitSet = Reko.Core.Lib.BitSet;
using Reko.Core.Lib;

namespace Reko.Arch.X86
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

    /// <summary>
    /// Processor architecture definition for the Intel x86 family. Currently supported processors are 8086/7,
    /// 80186/7, 80286/7, 80386/7, 80486, and Pentium. Also, beginning support for x86-64
    /// </summary>
	public class IntelArchitecture : IProcessorArchitecture
	{
		private ProcessorMode mode;
        private List<FlagGroupStorage> flagGroups;

		public IntelArchitecture(ProcessorMode mode)
		{
			this.mode = mode;
            this.flagGroups = new List<FlagGroupStorage>();
		
		}

		public Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
		{
			if (mode == ProcessorMode.Protected32)
			{
				return Address.Ptr32(offset);
			}
			else
			{
				return state.AddressFromSegOffset(seg, offset);
			}
		}

        public X86Disassembler CreateDisassembler(ImageReader imageReader)
        {
            return mode.CreateDisassembler(imageReader);
        }

        public virtual Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new X86InstructionComparer(norm);
        }

		IEnumerable<MachineInstruction> IProcessorArchitecture.CreateDisassembler(ImageReader imageReader)
		{
            return CreateDisassembler(imageReader);
		}

        public virtual BitSet CreateRegisterBitset()
		{
			return new BitSet((int) Registers.Max);
		}

		public virtual ProcessorState CreateProcessorState()
		{
			return new X86State(this);
		}

        public virtual IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new X86Rewriter(this, host, (X86State) state, rdr, frame);
        }

        public virtual IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            return mode.CreateInstructionScanner(map, rdr, knownLinAddresses, flags);
        }

        public virtual ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            return new X86ProcedureSerializer(this, typeLoader, defaultCc);
        }

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            return mode.CreateStackAccess(frame, offset, dataType);
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return mode.MakeAddressFromConstant(c);
        }

        public Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return mode.ReadCodeAddress(byteSize, rdr, state);
        }

		public FlagGroupStorage GetFlagGroup(uint grf)
		{
			foreach (FlagGroupStorage f in flagGroups)
			{
				if (f.FlagGroupBits == grf)
					return f;
			}

			PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(grf, GrfToString(grf), dt);
			flagGroups.Add(fl);
			return fl;
		}

        public FlagGroupStorage GetFlagGroup(string name)
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

		public RegisterStorage GetRegister(int i)
		{
			return Registers.GetRegister(i);
		}

        public RegisterStorage GetRegister(string name)
		{
			var r = Registers.GetRegister(name);
			if (r == RegisterStorage.None)
				throw new ArgumentException(string.Format("'{0}' is not a register name.", name));
			return r;
		}

        public bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = Registers.GetRegister(name);
            return (reg != RegisterStorage.None);
        }

        public int InstructionBitSize { get { return 8; } }

		public string GrfToString(uint grf)
		{
			StringBuilder s = new StringBuilder();
			for (int r = Registers.S.Number; grf != 0; ++r, grf >>= 1)
			{
				if ((grf & 1) != 0)
					s.Append(Registers.GetRegister(r).Name);
			}
			return s.ToString();
		}

		public ProcessorMode ProcessorMode
		{
			get { return mode; }
		}

        public PrimitiveType PointerType { get { return mode.PointerType; } }
        public PrimitiveType WordWidth { get { return mode.WordWidth; } }
        public PrimitiveType FramePointerType { get { return mode.FramePointerType; } }
        public RegisterStorage StackRegister { get { return mode.StackRegister; } }
        
        public uint CarryFlagMask { get { return (uint)FlagM.CF; } }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return mode.TryParseAddress(txtAddress, out addr);
        }
    }

    public class X86ArchitectureReal : IntelArchitecture
    {
        public X86ArchitectureReal()
            : base(ProcessorMode.Real)
        {
        }
    }

    public class X86ArchitectureFlat32 : IntelArchitecture
    {
        public X86ArchitectureFlat32()
            : base(ProcessorMode.Protected32)
        {
        }
    }

    public class X86ArchitectureFlat64 : IntelArchitecture
    {
        public X86ArchitectureFlat64()
            : base(ProcessorMode.Protected64)
        {
        }
    }
}
