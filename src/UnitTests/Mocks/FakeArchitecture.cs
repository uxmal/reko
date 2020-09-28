#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// A fake architecture.
    /// </summary>
    /// <remarks>
    /// Our fake architecture has 64 registers. r63 is the stack register, r62 is the return address register.
    /// </remarks>
    public class FakeArchitecture : ProcessorArchitecture
	{
		private static RegisterStorage [] registers;
        private static RegisterStorage FlagRegister = new RegisterStorage("flags", 70, 0, PrimitiveType.Word32);
        private static MemoryIdentifier ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));
        private static RegisterStorage Top = new RegisterStorage("Top", 76, 0, PrimitiveType.Byte);

        private RtlTraceBuilder rewriters;

		internal const int RegisterCount = 64;
		private const int iStackRegister = 63;
		private const int iReturnRegister = 62;
        private bool ignoreUnknownTraces;

        public FakeArchitecture() : base("fake")
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 32;
            this.rewriters = new RtlTraceBuilder();
            this.StackRegister = GetRegister(FakeArchitecture.iStackRegister);
            this.Description = "Fake Architecture for testing";
            this.FramePointerType = PrimitiveType.Ptr32;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.FpuStackBase = ST;
            this.FpuStackRegister = Top;

            this.CarryFlagMask = (uint)StatusFlags.C; 
        }

        static FakeArchitecture()
		{
			registers = new RegisterStorage[RegisterCount];
			for (int i = 0; i < registers.Length; ++i)
			{
				registers[i] = new RegisterStorage("r" + i, i, 0, PrimitiveType.Word32);
			}
		}

        public EndianServices Test_Endianness
        {
            get { return this.Endianness; }
            set { this.Endianness = value; }
        }

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

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
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

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddrs, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            var sp = binder.EnsureRegister(StackRegister);
            return MemoryAccess.Create(sp, offset, dataType);
        }

        public override CallingConvention GetCallingConvention(string name)
        {
            throw new NotImplementedException();
        }


        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var sb = new StringBuilder();
            if (((uint)grf & 0x01) != 0) sb.Append('S');
            if (((uint)grf & 0x02) != 0) sb.Append('Z');
            if (((uint)grf & 0x04) != 0) sb.Append('C');
            if (((uint)grf & 0x08) != 0) sb.Append('V');
            if (((uint)grf & 0x10) != 0) sb.Append('X');
            if (sb.Length == 0)
                return null;
            return new FlagGroupStorage(flagRegister, grf, sb.ToString(), PrimitiveType.Byte);
        }

		public override FlagGroupStorage GetFlagGroup(string s)
		{
            uint grf = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                switch (char.ToUpper(s[i]))
                {
                case 'S': grf |= 0x01; break;
                case 'Z': grf |= 0x02; break;
                case 'C': grf |= 0x04; break;
                case 'V': grf |= 0x08; break;
                case 'X': grf |= 0x10; break;
                }
            }
            if (grf != 0)
                return new FlagGroupStorage(FlagRegister, grf, s,
                    Bits.IsSingleBitSet(grf)
                        ? PrimitiveType.Bool
                        : PrimitiveType.Byte);
            return null;
		}

        public RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < registers.Length)
                return registers[i];
            return null;
        }

		public override RegisterStorage GetRegister(string s)
		{
            if (s[0] == 'r')
            {
                if (int.TryParse(s.Substring(1), out int reg))
                    return GetRegister(reg);
            }
            return null;
		}

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return GetRegister(domain - StorageDomain.Register);
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            for (uint bitMask = 1; bitMask <= flags.FlagGroupBits; bitMask <<= 1)
            {
                if ((flags.FlagGroupBits & bitMask) != 0)
                {
                    yield return GetFlagGroup(FakeArchitecture.FlagRegister, bitMask);
                }
            }
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            return reg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return registers;
        }

        public override bool TryGetRegister(string name, out RegisterStorage result)
        {
            result = null;
            return false;
        }

		public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
		{
            return new FakeDisassembler(rdr.Address, Test_DisassemblyStream.GetEnumerator());
		}

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
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

		public override ProcessorState CreateProcessorState()
		{
			return new FakeProcessorState(this);
		}

		public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
		{
            var sb = new StringBuilder();
            if ((grf & (uint) StatusFlags.S) != 0) sb.Append('S');
            if ((grf & (uint) StatusFlags.Z) != 0) sb.Append('Z');
            if ((grf & (uint) StatusFlags.C) != 0) sb.Append('C');
            return sb.ToString();
		}


        public MemoryIdentifier FpuStackBase { get; set; }

        public override List<RtlInstruction> InlineCall(Address addr, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override void PostprocessProgram(Program program)
        {
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
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

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
        {
            return Address.SegPtr(seg.ToUInt16(), offset.ToUInt16());
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield return reg;
        }

        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
        {
            ulong mask = bits.Where(b => b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            return mask != 0 ? reg : null;
        }

        public override void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg)
        {
            ids.Remove(reg);
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public Func<
            IProcessorArchitecture, IStorageBinder, CallSite, Expression,
            FrameApplicationBuilder>
                Test_CreateFrameApplicationBuilder =
                    (arch, binder, site, callee) =>
                    new FrameApplicationBuilder(arch, binder, site, callee, false);
        public override FrameApplicationBuilder CreateFrameApplicationBuilder(IStorageBinder binder, CallSite site, Expression callee)
        {
            return Test_CreateFrameApplicationBuilder(this, binder, site, callee);
        }

        public override Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            Expression e = binder.EnsureRegister(FpuStackRegister);
            if (offset != 0)
            {
                BinaryOperator op;
                if (offset < 0)
                {
                    offset = -offset;
                    op = Operator.ISub;
                }
                else
                {
                    op = Operator.IAdd;
                }
                e = new BinaryExpression(op, e.DataType, e, Constant.Create(e.DataType, offset));
            }
            return new MemoryAccess(FpuStackBase, e, PrimitiveType.Real64);
        }


        public RtlInstructionCluster InlineInstructions(AddressRange addrCaller, EndianImageReader rdrProcedureNody, IStorageBinder binder)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class FakeArchitecture64 : ProcessorArchitecture
    {
        public FakeArchitecture64() : base("fakeArch64")
        {
            Endianness = EndianServices.Little;
            FramePointerType = PrimitiveType.Ptr64;
            PointerType = PrimitiveType.Ptr64;
            WordWidth = PrimitiveType.Word64;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            throw new NotImplementedException();
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr64(c.ToUInt64());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            throw new NotImplementedException();
        }
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
        S = 1,
        Z = 2,
        C = 4,
    }
}
