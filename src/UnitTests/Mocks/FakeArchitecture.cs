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
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Decompiler.Core.Serialization;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// A fake architecture.
	/// </summary>
	/// <remarks>
	/// Our fake architecture has 64 registers. r63 is the stack register, r62 is the return address register.
	/// </remarks>
	public class FakeArchitecture : IProcessorArchitecture
	{
		private static RegisterStorage [] registers;
        private RtlTraceBuilder rewriters;

		internal const int RegisterCount = 64;
		private const int iStackRegister = 63;
		private const int iReturnRegister = 62;

		public FakeArchitecture()
		{
            this.rewriters = new RtlTraceBuilder();
		}

		static FakeArchitecture()
		{
			registers = new RegisterStorage[RegisterCount];
			for (int i = 0; i < registers.Length; ++i)
			{
				registers[i] = new MockMachineRegister("r" + i, i, PrimitiveType.Word32);
			}
		}

        public IEnumerable<MachineInstruction> DisassemblyStream { get; set; }

        public void Test_AddTrace(RtlTrace trace)
        {
            rewriters.Add(trace);
        }

        public void Test_AddTraces(IEnumerable<RtlTrace> traces)
        {
            foreach (var t in traces)
                rewriters.Add(t);
        }

		public static RegisterStorage GetMachineRegister(int i)
		{
			return registers[i];
		}

		#region IProcessorArchitecture Members

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            var linAddr = rdr.Address.ToLinear();
            RtlTrace trace;
            if (!rewriters.Traces.TryGetValue(rdr.Address, out trace))
                NUnit.Framework.Assert.Fail(string.Format("Unexpected request for a rewriter at address {0}", rdr.Address));
            return trace;
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownLinAddrs, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(uint grf)
		{
			return null;
		}

		public FlagGroupStorage GetFlagGroup(string name)
		{
			return null;
		}

        public RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < registers.Length)
                return registers[i];
            return null;
        }

        public bool TryGetRegister(string name, out RegisterStorage result)
        {
            result = null;
            return false;
        }

		public RegisterStorage GetRegister(string s)
		{
            if (s[0] == 'r')
            {
                int reg;
                if (int.TryParse(s.Substring(1), out reg))
                    return GetRegister(reg);
            }
            return null;
		}

        public int InstructionBitSize { get { return 32; } }

		public string RegisterToString(int reg)
		{
			throw new NotImplementedException("// TODO:  Add ArchitectureMock.RegisterToString implementation");
		}

		public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader rdr)
		{
            return new FakeDisassembler(rdr.Address, DisassemblyStream.GetEnumerator());
		}

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

		public ProcessorState CreateProcessorState()
		{
			return new FakeProcessorState(this);
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

        public uint CarryFlagMask { get { return (uint) StatusFlags.C; } }
        public RegisterStorage StackRegister { get { return GetRegister(FakeArchitecture.iStackRegister); } }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        #endregion
    }

	public class MockMachineRegister : RegisterStorage
	{
		public MockMachineRegister(string name, int i, PrimitiveType dt) : base(name, i, dt) { }

        public override RegisterStorage GetSubregister(int offset, int size)
		{
			return this;
		}

		public override RegisterStorage GetWidestSubregister(BitSet bits)
		{
			return (bits[Number])
				? this
				: null;
		}
	}

	public class FakeProcessorState : ProcessorState
	{
        private IProcessorArchitecture arch;
        private Dictionary<RegisterStorage, Constant> regValues;
        private SortedList<int, Constant> stackValues;

        public FakeProcessorState(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regValues = new Dictionary<RegisterStorage, Constant>();
            this.stackValues = new SortedList<int, Constant>();
        }

        public FakeProcessorState(FakeProcessorState orig)
            : base(orig)
        {
            this.arch = orig.arch;
            this.regValues = new Dictionary<RegisterStorage, Constant>(orig.regValues);
            this.stackValues = new SortedList<int, Constant>(orig.stackValues);
        }

        public override IProcessorArchitecture Architecture { get { return arch; }  }
		
        public override ProcessorState Clone()
		{
			return new FakeProcessorState(this);
		}

		public override Constant GetRegister(RegisterStorage r)
		{
            Constant c;
            if (!regValues.TryGetValue(r, out c))
            {
                c = Constant.Invalid;
            }
            return c;
		}

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(ProcedureSignature sig)
        {
        }

        public override void SetRegister(RegisterStorage r, Constant v)
		{
            regValues[r] = v;
		}

        public override void SetInstructionPointer(Address addr)
		{
		}

        public override CallSite OnBeforeCall(Identifier sp, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, 0);
        }

        public override void OnAfterCall(Identifier sp, ProcedureSignature sigCallee, ExpressionVisitor<Expression> eval)
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
