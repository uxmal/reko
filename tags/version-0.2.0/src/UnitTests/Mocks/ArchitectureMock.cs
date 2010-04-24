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
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Core.Lib;
using System;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// A fake architecture.
	/// </summary>
	/// <remarks>
	/// Our fake architecture has 64 registers. r63 is the stack register, r62 is the return address register.
	/// </remarks>
	public class ArchitectureMock : IProcessorArchitecture
	{
		private static MachineRegister [] registers;
		private BitSet implicitRegs;

		internal const int RegisterCount = 64;
		private const int StackRegister = 63;
		private const int ReturnRegister = 62;

		public ArchitectureMock()
		{
			this.implicitRegs = new BitSet(RegisterCount);
			implicitRegs[StackRegister]  = true;
			implicitRegs[ReturnRegister] = true;
		}

		static ArchitectureMock()
		{
			registers = new MachineRegister[RegisterCount];
			for (int i = 0; i < registers.Length; ++i)
			{
				registers[i] = new MockMachineRegister("r" + i, i, PrimitiveType.Word32);
			}
		}

		public static MachineRegister GetMachineRegister(int i)
		{
			return registers[i];
		}

		#region IProcessorArchitecture Members

		public Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
		{
			return CreateRewriter(prw, proc, host);
		}

		public Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, Address addrProc, int cbReturnOnStack, IRewriterHost host)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.CreateRewriter implementation");
		}

		public Dumper CreateDumper()
		{
			return new Dumper();
		}

		public MachineFlags GetFlagGroup(uint grf)
		{
			return null;
		}

		public MachineFlags GetFlagGroup(string name)
		{
			return null;
		}

		public MachineRegister GetRegister(int i)
		{
			if (0 <= i && i < registers.Length)
				return registers[i];
			return null;
		}

		public MachineRegister GetRegister(string s)
		{
			return null;
		}

		public BitSet ImplicitArgumentRegisters
		{
			get { return implicitRegs; }
		}

		public string RegisterToString(int reg)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.RegisterToString implementation");
		}

		public BackWalker CreateBackWalker(ProgramImage img)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.CreateBackWalker implementation");
		}

		public CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
		{
			return new MockCodeWalker(addr);
		}

		public Disassembler CreateDisassembler(ImageReader rdr)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.CreateDisassembler implementation");
		}

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

		public ProcessorState CreateProcessorState()
		{
			return new FakeProcessorState();
		}

		public BitSet CreateRegisterBitset()
		{
			return new BitSet(RegisterCount);
		}

		public string GrfToString(uint grf)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.GrfToString implementation");
		}

        public PrimitiveType FramePointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

		public PrimitiveType WordWidth
		{
			get { return PrimitiveType.Word32; }
		}

		#endregion

	}

	public class MockMachineRegister : MachineRegister
	{
		public MockMachineRegister(string name, int i, PrimitiveType dt) : base(name, i, dt) { }

		public override MachineRegister GetSubregister(int offset, int size)
		{
			return this;
		}

		public override MachineRegister GetWidestSubregister(BitSet bits)
		{
			return (bits[Number])
				? this
				: null;
		}
	}

	public class FakeProcessorState : ProcessorState
	{
		public ProcessorState Clone()
		{
			return new FakeProcessorState();
		}

		public Constant Get(MachineRegister r)
		{
			return Constant.Invalid;
		}

		public void Set(MachineRegister r, Decompiler.Core.Code.Constant v)
		{
			throw new NotImplementedException();
		}


		public void SetInstructionPointer(Address addr)
		{
		}
	}
}
