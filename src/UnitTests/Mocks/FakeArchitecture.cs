#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Reko.Core.Serialization;
using NUnit.Framework;
using Reko.Core.Operators;

namespace Reko.UnitTests.Mocks
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
        private static RegisterStorage flags = new RegisterStorage("flags", 70, 0, PrimitiveType.Word32);
        private RtlTraceBuilder rewriters;

		internal const int RegisterCount = 64;
		private const int iStackRegister = 63;
		private const int iReturnRegister = 62;
        private bool ignoreUnknownTraces;

        public FakeArchitecture() 
		{
            this.rewriters = new RtlTraceBuilder();
            this.StackRegister = GetRegister(FakeArchitecture.iStackRegister);
            this.Name = "FakeArch";
            this.Description = "Fake Architecture for testing";
        }

		static FakeArchitecture()
		{
			registers = new RegisterStorage[RegisterCount];
			for (int i = 0; i < registers.Length; ++i)
			{
				registers[i] = new RegisterStorage("r" + i, i, 0, PrimitiveType.Word32);
			}
		}

        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<MachineInstruction> Test_DisassemblyStream { get; set; }

        public void Test_AddTrace(RtlTrace trace)
        {
            rewriters.Add(trace);
        }

        public void Test_AddTraces(IEnumerable<RtlTrace> traces)
        {
            foreach (var t in traces)
                rewriters.Add(t);
        }

        public void Test_IgnoreAllUnkownTraces()
        {
            this.ignoreUnknownTraces = true;
        }

		public static RegisterStorage GetMachineRegister(int i)
		{
			return registers[i];
		}

		#region IProcessorArchitecture Members

        public IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            var linAddr = rdr.Address.ToLinear();
            if (!rewriters.Traces.TryGetValue(rdr.Address, out RtlTrace trace))
            {
                if (ignoreUnknownTraces)
                {
                    return new RtlInstructionCluster[0];
                }
                else
                {
                    Assert.Fail(string.Format("Unexpected request for a rewriter at address {0}", rdr.Address));
                }
            }
            return trace;
        }

        public IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddrs, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            return MemoryAccess.Create(
                    binder.EnsureRegister(this.StackRegister),
                    offset,
                    dataType);
        }

        public EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotImplementedException();
        }

        public CallingConvention GetCallingConvention(string name)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(uint uGrf)
        {
            var grf = (StatusFlags) uGrf;
            var sb = new StringBuilder();
            if ((grf & StatusFlags.S) != 0) sb.Append('S');
            if ((grf & StatusFlags.C) != 0) sb.Append('C');
            if ((grf & StatusFlags.Z) != 0) sb.Append('Z');
            if ((grf & StatusFlags.O) != 0) sb.Append('O');
            if ((grf & StatusFlags.V) != 0) sb.Append('V');
            if ((grf & StatusFlags.X) != 0) sb.Append('X');
            if (sb.Length == 0)
                return null;
            return new FlagGroupStorage(flags, uGrf, sb.ToString(), PrimitiveType.Byte);
        }

		public FlagGroupStorage GetFlagGroup(string s)
		{
            StatusFlags grf = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                switch (char.ToUpper(s[i]))
                {
                case 'S': grf |= StatusFlags.S; break;
                case 'C': grf |= StatusFlags.C; break;
                case 'Z': grf |= StatusFlags.Z; break;
                case 'O': grf |= StatusFlags.O; break;
                case 'V': grf |= StatusFlags.V; break;
                case 'X': grf |= StatusFlags.X; break;
                }
            }
            if (grf != 0)
                return new FlagGroupStorage(flags, (uint)grf, s, PrimitiveType.Byte);
            return null;
		}

        public RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < registers.Length)
                return registers[i];
            return null;
        }

		public RegisterStorage GetRegister(string s)
		{
            if (s[0] == 'r')
            {
                if (int.TryParse(s.Substring(1), out int reg))
                    return GetRegister(reg);
            }
            return null;
		}

        public RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0 && width == (int)reg.BitSize)
                return reg;
            else
                return null;
        }

        public RegisterStorage[] GetRegisters()
        {
            return registers;
        }

        public bool TryGetRegister(string name, out RegisterStorage result)
        {
            result = null;
            return false;
        }

        public int InstructionBitSize { get { return 32; } }

		public IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
		{
            return new FakeDisassembler(rdr.Address, Test_DisassemblyStream.GetEnumerator());
		}

        public IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Comp(norm);
        }

        private class Comp : EqualityComparer<MachineInstruction>
        {
            private readonly Normalize norm;

            public Comp(Normalize norm)
            {
                this.norm = norm;
            }

            public override bool Equals(MachineInstruction x, MachineInstruction y)
            {
                return false;
            }

            public override int GetHashCode(MachineInstruction obj)
            {
                return 1;
            }
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

		public ProcessorState CreateProcessorState()
		{
			return new FakeProcessorState(this);
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
            get { return PrimitiveType.Ptr32; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Ptr32; }
        }

		public PrimitiveType WordWidth
		{
			get { return PrimitiveType.Word32; }
		}

        public uint CarryFlagMask { get { return (uint) StatusFlags.C; } }
        public RegisterStorage StackRegister { get; set; }

        public List<RtlInstruction> InlineCall(Address addr, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public void PostprocessProgram(Program program)
        {
        }

        public Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            if (size == 4)
            {
                if (rdr.TryReadLeUInt32(out var uaddr))
                {
                    return Address.Ptr32(uaddr);
                }
                else
                {
                    return null;
                }
            }
            throw new NotImplementedException();
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        public Address MakeSegmentedAddress(Constant seg, Constant offset)
        {
            return Address.SegPtr(seg.ToUInt16(), offset.ToUInt16());
        }

        public IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield return reg;
        }

        public RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
        {
            ulong mask = bits.Where(b => b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            return mask != 0 ? reg : null;
        }

        public void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg)
        {
            ids.Remove(reg);
        }

        public void LoadUserOptions(Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> SaveUserOptions()
        {
            throw new NotImplementedException();
        }

        public SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
        {
            return mem.TryReadLe(addr, dt, out value);
        }

        public RtlInstructionCluster InlineInstructions(AddressRange addrCaller, EndianImageReader rdrProcedureNody, IStorageBinder binder)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

	public class MockMachineRegister : RegisterStorage
	{
		public MockMachineRegister(string name, int i, PrimitiveType dt) : base(name, i, 0, dt) { }
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
            if (!regValues.TryGetValue(r, out Constant c))
            {
                c = Constant.Invalid;
            }
            return c;
		}

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(FunctionType sig)
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

        public override void OnAfterCall(FunctionType sigCallee)
        {
        }
    }

    [Flags]
    public enum StatusFlags: uint
    {
        S = 0x01,
        Z = 0x02,
        C = 0x04,
        O = 0x08,
        V = 0x10,
        X = 0x20,
    }
}
