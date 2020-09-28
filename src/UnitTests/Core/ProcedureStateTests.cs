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

using Reko.Scanning;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core.Serialization;
using System.Linq;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class ProcedureStateTests
    {
        private RegisterStorage sp;
        private FakeArchitecture arch;
        private Identifier idSp;
        private ExpressionEmitter m;
        private SegmentMap map;

        [SetUp]
        public void Setup()
        {
            arch = new FakeArchitecture();
            sp = new RegisterStorage("sp", 42, 0, PrimitiveType.Ptr32);
            arch.StackRegister = sp;

            idSp = new Identifier(sp.Name, sp.DataType, sp);
            m = new ExpressionEmitter();
        }

        private void Given_32bit_SegmentMap()
        {
            this.map = new SegmentMap(
                Address.Ptr32(0x00100000),
                new ImageSegment(".text", new MemoryArea(Address.Ptr32(0x00100000), new byte[0x100]), AccessMode.ReadExecute),
                new ImageSegment(".data", new MemoryArea(Address.Ptr32(0x00101000), new byte[0x100]), AccessMode.ReadWriteExecute));
        }

        [Test]
        public void ProcState_SetValue()
        {
            var sce = new TestProcessorState(arch);

            sce.SetValue(idSp, m.ISub(idSp, 4));

            Assert.AreEqual("sp - 0x00000004", sce.GetValue(idSp).ToString());
        }

        [Test]
        public void ProcState_PushValueOnstack()
        {
            var sce = new TestProcessorState(arch);

            sce.SetValue(idSp, m.ISub(idSp, 4));
            sce.SetValueEa(idSp, Constant.Word32(0x12345678));

            Assert.AreEqual("0x12345678", sce.GetValue(m.Mem32(idSp), map).ToString());
        }

        [Test]
        public void ProcState_ReadConstantFromReadOnlyMemory()
        {
            Given_32bit_SegmentMap();
            var text = map.Segments.Values.Single(s => s.Name == ".text").MemoryArea;
            text.WriteLeUInt32(0, 0x01234567);

            var sce = new TestProcessorState(arch);
            var access = new MemoryAccess(Constant.Word32(0x00100000), PrimitiveType.Word32);
            var c = sce.GetValue(access, map);

            Assert.AreEqual("0x01234567", c.ToString());
        }

        public class FakeArchitecture : ProcessorArchitecture
        {
            public FakeArchitecture() : base("fake")
            {
                this.Endianness = EndianServices.Little;
                this.InstructionBitSize = 32;
            }

        #region IProcessorArchitecture Members

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

            public override ProcessorState CreateProcessorState()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddrs, PointerScannerFlags flags)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
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

            public override bool TryGetRegister(string name, out RegisterStorage reg)
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

            public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
            {
                throw new NotImplementedException();
            }

            public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
            {
                throw new NotImplementedException();
            }

            public override List<RtlInstruction> InlineCall(Address addr, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
            {
                throw new NotImplementedException();
            }

            public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
            {
                return Address.Ptr32(c.ToUInt32());
            }

            public override void PostprocessProgram(Program program)
            {
                throw new NotImplementedException();
            }

            public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
            {
                throw new NotImplementedException();
            }


            public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
            {
                throw new NotImplementedException();
            }

            public override bool TryParseAddress(string txtAddress, out Address addr)
            {
                return Address.TryParse32(txtAddress, out addr);
            }

            public override Address MakeSegmentedAddress(Constant seg, Constant offset)
            {
                throw new NotImplementedException();
            }

            public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
            {
                throw new NotImplementedException();
            }

            public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> bits)
            {
                throw new NotImplementedException();
            }

            public override void RemoveAliases(ISet<RegisterStorage> ids, RegisterStorage reg)
            {
                throw new NotImplementedException();
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

            public override FrameApplicationBuilder CreateFrameApplicationBuilder(IStorageBinder binder, CallSite site, Expression callee)
            {
                throw new NotImplementedException();
            }

            public override Expression CreateFpuStackAccess(IStorageBinder binder, int offset, DataType dataType)
            {
                throw new NotImplementedException();
            }

            public RtlInstructionCluster InlineInstructions(AddressRange addrCaller, EndianImageReader rdrProcedureNody, IStorageBinder binder)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class TestProcessorState : ProcessorState
        {
            private readonly IProcessorArchitecture arch;
            private readonly Dictionary<RegisterStorage, Constant> regs = new Dictionary<RegisterStorage, Constant>();
            private readonly SortedList<int, Constant> stack = new SortedList<int, Constant>();

            public TestProcessorState(IProcessorArchitecture arch)
            {
                this.arch = arch;
            }

            public override IProcessorArchitecture Architecture { get { return arch; } }

            #region ProcessorState Members

            public override ProcessorState Clone()
            {
                throw new NotImplementedException();
            }

            public override Constant GetRegister(RegisterStorage r)
            {
                if (!regs.TryGetValue(r, out Constant c))
                    c = Constant.Invalid;
                return c;
            }

            public override void SetRegister(RegisterStorage r, Constant v)
            {
                regs[r] = v;
            }

            public override void SetInstructionPointer(Address addr)
            {
                throw new NotImplementedException();
            }

            public override void OnProcedureEntered()
            {
                throw new NotImplementedException();
            }

            public override void OnProcedureLeft(FunctionType procedureSignature)
            {
                throw new NotImplementedException();
            }

            public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
            {
                throw new NotImplementedException();
            }

            public override void OnAfterCall(FunctionType sigCallee)
            {
            }

            #endregion
        }
    }
}
