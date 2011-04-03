#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Text;
using System.Collections.Generic;

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
        private Dictionary<uint, FakeRewriter> rewriters;

		internal const int RegisterCount = 64;
		private const int iStackRegister = 63;
		private const int iReturnRegister = 62;

		public ArchitectureMock()
		{
			this.implicitRegs = new BitSet(RegisterCount);
			implicitRegs[iStackRegister]  = true;
			implicitRegs[iReturnRegister] = true;
            this.rewriters = new Dictionary<uint, FakeRewriter>();
		}

		static ArchitectureMock()
		{
			registers = new MachineRegister[RegisterCount];
			for (int i = 0; i < registers.Length; ++i)
			{
				registers[i] = new MockMachineRegister("r" + i, i, PrimitiveType.Word32);
			}
		}

        public IEnumerable<MachineInstruction> DisassemblyStream { get; set; }

        public void SetRewriterForAddress(Address address, FakeRewriter rewriter)
        {
            rewriters.Add(address.Linear, rewriter);
        }

		public static MachineRegister GetMachineRegister(int i)
		{
			return registers[i];
		}

		#region IProcessorArchitecture Members

		public RewriterOld CreateRewriterOld(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
		{
			return CreateRewriterOld(prw, proc, host);
		}

		public Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, Address addrProc, int cbReturnOnStack, IRewriterHost host)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.CreateRewriter implementation");
		}

        public Rewriter CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost2 host)
        {
            var linAddr = rdr.Address.Linear;
            FakeRewriter rewriter;
            if (!rewriters.TryGetValue(linAddr, out rewriter))
                NUnit.Framework.Assert.Fail(string.Format("Unexpected request for a rewriter at address {0}", rdr.Address));
            return rewriters[linAddr];
        }

		public Dumper CreateDumper()
		{
			return new Dumper();
		}

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            throw new NotImplementedException();
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

        public bool TryGetRegister(string name, out MachineRegister result)
        {
            result = null;
            return false;
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

		public CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
		{
			return new MockCodeWalker(addr);
		}

		public Disassembler CreateDisassembler(ImageReader rdr)
		{
            return new FakeDisassembler(rdr.Address, DisassemblyStream.GetEnumerator());
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
            var sb = new StringBuilder();
            if ((grf & (uint) StatusFlags.S) != 0) sb.Append('S');
            if ((grf & (uint) StatusFlags.Z) != 0) sb.Append('Z');
            if ((grf & (uint) StatusFlags.C) != 0) sb.Append('C');
            return sb.ToString();
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

        public uint CarryFlagMask { get { throw new NotImplementedException(); } }
        public MachineRegister StackRegister { get { return GetRegister(ArchitectureMock.iStackRegister); } }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
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
        private Dictionary<MachineRegister, Constant> regValues = new Dictionary<MachineRegister, Constant>();

		public ProcessorState Clone()
		{
			return new FakeProcessorState();
		}

		public Constant Get(MachineRegister r)
		{
            Constant c;
            if (!regValues.TryGetValue(r, out c))
            {
                c = Constant.Invalid;
            }
            return c;
		}

        public void OnProcedureEntered()
        {
        }

        public void OnProcedureLeft(ProcedureSignature sig)
        {
        }

		public void Set(MachineRegister r, Constant v)
		{
            regValues[r] = v;
		}

		public void SetInstructionPointer(Address addr)
		{
		}

        public CallSite OnBeforeCall(int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public void OnAfterCall(ProcedureSignature sigCallee)
        {
        }
	}

    [Flags]
    public enum StatusFlags: uint
    {
        S = 1,
        Z = 2,
        C = 4,
    }
}
