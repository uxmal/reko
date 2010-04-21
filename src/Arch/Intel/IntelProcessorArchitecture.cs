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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
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

namespace Decompiler.Arch.Intel
{
	// The registers of an Intel ia32 machine.

	public class Registers
	{
		public static readonly Intel32AccRegister eax;
		public static readonly Intel32AccRegister ecx;
		public static readonly Intel32AccRegister edx;
		public static readonly Intel32AccRegister ebx;
		public static readonly Intel32Register esp;
		public static readonly Intel32Register ebp;
		public static readonly Intel32Register esi;
		public static readonly Intel32Register edi;
		public static readonly Intel16AccRegister ax;
		public static readonly Intel16AccRegister cx;
		public static readonly Intel16AccRegister dx;
		public static readonly Intel16AccRegister bx;
		public static readonly Intel16Register sp;
		public static readonly Intel16Register bp;
		public static readonly Intel16Register si;
		public static readonly Intel16Register di;
		public static readonly IntelLoByteRegister al;
		public static readonly IntelLoByteRegister cl;
		public static readonly IntelLoByteRegister dl;
		public static readonly IntelLoByteRegister bl;
		public static readonly IntelHiByteRegister ah;
		public static readonly IntelHiByteRegister ch;
		public static readonly IntelHiByteRegister dh;
		public static readonly IntelHiByteRegister bh;
		public static readonly SegmentRegister es;
		public static readonly SegmentRegister cs;
		public static readonly SegmentRegister ss;
		public static readonly SegmentRegister ds;
		public static readonly SegmentRegister fs;
		public static readonly SegmentRegister gs;

		public static readonly FlagRegister S;
		public static readonly FlagRegister C;
		public static readonly FlagRegister Z;
		public static readonly FlagRegister D;
		public static readonly FlagRegister O;
        public static readonly FlagRegister P;

        public static readonly MachineRegister FPUF;

		private static readonly MachineRegister[] regs;

		static Registers()
		{
			eax = new Intel32AccRegister("eax", 0,  8, 16, 20);
			ecx = new Intel32AccRegister("ecx", 1,  9, 17, 21);
			edx = new Intel32AccRegister("edx", 2, 10, 18, 22);
			ebx = new Intel32AccRegister("ebx", 3, 11, 19, 23);
			esp = new Intel32Register("esp", 4, 12, -1, -1);
			ebp = new Intel32Register("ebp", 5, 13, -1, -1);
			esi = new Intel32Register("esi", 6, 14, -1, -1);
			edi = new Intel32Register("edi", 7, 15, -1, -1);
			ax = new Intel16AccRegister("ax",  8, 0, 16, 20);
			cx = new Intel16AccRegister("cx",  9, 1, 17, 21);
			dx = new Intel16AccRegister("dx", 10, 2, 18, 22);
			bx = new Intel16AccRegister("bx", 11, 3, 19, 23);
			sp = new Intel16Register("sp", 12,  4, -1, -1);
			bp = new Intel16Register("bp", 13,  5, -1, -1);
			si = new Intel16Register("si", 14,  6, -1, -1);
			di = new Intel16Register("di", 15,  7, -1, -1);
			al = new IntelLoByteRegister("al", 16,  0,   8, 16, 20);
			cl = new IntelLoByteRegister("cl", 17,  1,  9, 17, 21);
			dl = new IntelLoByteRegister("dl", 18,  2, 10, 18, 22);
			bl = new IntelLoByteRegister("bl", 19,  3, 11, 19, 23);
			ah = new IntelHiByteRegister("ah", 20,  0,  8, 16, 20);
			ch = new IntelHiByteRegister("ch", 21,  1,  9, 17, 21);
			dh = new IntelHiByteRegister("dh", 22,  2, 10, 18, 22);
			bh = new IntelHiByteRegister("bh", 23,  3, 11, 19, 23);
			es = new SegmentRegister("es", 24);
			cs = new SegmentRegister("cs", 25);
			ss = new SegmentRegister("ss", 26);
			ds = new SegmentRegister("ds", 27);
			fs = new SegmentRegister("fs", 28);
			gs = new SegmentRegister("gs", 29);
			S = new FlagRegister("S", 32);
			C = new FlagRegister("C", 33);
			Z = new FlagRegister("Z", 34);
			D = new FlagRegister("D", 35);
            O = new FlagRegister("O", 36);
            P = new FlagRegister("P", 37);
            FPUF = new MachineRegister("FPUF", 38, PrimitiveType.Byte);

			regs = new MachineRegister[] {
				eax,
				ecx,
				edx,
				ebx, 
				esp,
				ebp,
				esi,
				edi,

				ax ,
				cx ,
				dx ,
				bx ,
				sp ,
				bp ,
				si ,
				di ,

				al ,
				cl ,
				dl ,
				bl ,
				ah ,
				ch ,
				dh ,
				bh ,

				es ,
				cs ,
				ss ,
				ds ,
				fs ,
				gs ,
				null,
				null,

				S ,
				C ,
				Z ,
				D ,

				O ,
                P,
                FPUF,
			};
		}

		public static MachineRegister GetRegister(int i)
		{
			return (0 <= i && i < regs.Length)
				? regs[i] 
				: null;
		}

		public static MachineRegister GetRegister(string name)
		{
			for (int i = 0; i < regs.Length; ++i)
			{
				if (regs[i] != null && String.Compare(regs[i].Name, name, true) == 0)
					return regs[i];
			}
			return MachineRegister.None;
		}

		public static int Max 
		{
			get { return regs.Length; }
		}
	}

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

		public Address AddressFromSegOffset(IntelState state, MachineRegister seg, uint offset)
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

		public BackWalker CreateBackWalker(ProgramImage img)
		{
			return new IntelBackWalker(this, img);
		}

		public virtual CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
		{
			return new IntelCodeWalker(this, platform, new IntelDisassembler(img.CreateReader(addr), this.WordWidth), (IntelState) st);
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
			return new IntelState();
		}

		public virtual Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
		{
			return new IntelRewriter(prw, proc, host, this, new IntelRewriterState(proc.Frame));
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

        public PrimitiveType PointerType
        {
            get { return mode.PointerType; }
        }

		public ProcessorMode ProcessorMode
		{
			get { return mode; }
		}

		public PrimitiveType WordWidth
		{
			get { return mode.WordWidth; }
		}

        public PrimitiveType FramePointerType
        {
            get { return mode.FramePointerType; }
        }

    }
}
