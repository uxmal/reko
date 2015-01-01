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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using Decompiler.Core.Rtl;
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

    /// <summary>
    /// Processor architecture definition for the Intel x86 family. Currently supported processors are 8086/7,
    /// 80186/7, 80286/7, 80386/7, 80486, and Pentium. Also, beginning support for x86-64
    /// </summary>
	public class IntelArchitecture : IProcessorArchitecture
	{
		private BitSet implicitRegs;
		private ProcessorMode mode;
        private List<FlagGroupStorage> flagGroups;

		public IntelArchitecture(ProcessorMode mode)
		{
			this.mode = mode;
            this.flagGroups = new List<FlagGroupStorage>();
		
			implicitRegs = CreateRegisterBitset();
			implicitRegs[Registers.cs.Number] = true;
			implicitRegs[Registers.ss.Number] = true;
			implicitRegs[Registers.sp.Number] = true;
			implicitRegs[Registers.esp.Number] = true;
		}

		public Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
		{
			if (mode == ProcessorMode.Protected32)
			{
				return new Address(offset);
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

		IEnumerable<MachineInstruction> IProcessorArchitecture.CreateDisassembler(ImageReader imageReader)
		{
            return CreateDisassembler(imageReader);
		}

        public virtual Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, uint offset)
        {
            return new LeImageReader(image, offset);
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

        public virtual IEnumerable<uint> CreatePointerScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            return mode.CreateInstructionScanner(rdr, knownLinAddresses, flags);
        }

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            return mode.CreateStackAccess(frame, offset, dataType);
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

			PrimitiveType dt = IsSingleBit(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
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

		public BitSet ImplicitArgumentRegisters
		{
			get { return implicitRegs; }
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

		public static bool IsSingleBit(uint u)
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
        public RegisterStorage StackRegister { get { return mode.StackRegister; } }
        
        public uint CarryFlagMask { get { return (uint)FlagM.CF; } }
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
}
