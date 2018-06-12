﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Oh why the hell not. C64 Basic can be interpreted as a machine
    /// language of sorts.
    /// </summary>
    public class C64Basic : ProcessorArchitecture
    {
        private SortedList<ushort, C64BasicInstruction> program;
        private RegisterStorage stackRegister = new RegisterStorage("sp", 1, 0, PrimitiveType.Ptr16);

        public C64Basic(SortedList<ushort, C64BasicInstruction> program) : base("c64Basic")
        {
            this.program = program;
            this.PointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 8;
            this.StackRegister = stackRegister;
            this.FramePointerType = PrimitiveType.Ptr16;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            int i = program.IndexOfKey(imageReader.Address.ToUInt16());
            if (i < 0)
                yield break;
            for (; i < program.Count; ++i)
            {
                yield return program.Values[i];
            }
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea mem, ulong off)
        {
            return new LeImageReader(mem, off);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new C64BasicState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new C64BasicRewriter(this, rdr.Address, program, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return new RegisterStorage[0];
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enumerable.Range(0, C64BasicInstruction.TokenMax - C64BasicInstruction.TokenMin)
                .ToSortedList(
                    v => C64BasicInstruction.TokenStrs[v],
                    v => v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            int i = Array.IndexOf(C64BasicInstruction.TokenStrs, name);
            if (i < 0)
                return null;
            else
                return i + C64BasicInstruction.TokenMin;
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }

        public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
        {
            return mem.TryReadLe(addr, dt, out value);
        }

        public class C64BasicState : ProcessorState
        {
            private C64Basic arch;

            public C64BasicState(C64Basic arch)
            {
                this.arch = arch;
            }

            public C64BasicState(C64BasicState that) : base(that)
            {
                this.arch = that.arch;
            }

            public override IProcessorArchitecture Architecture { get { return arch; } }

            public override ProcessorState Clone()
            {
                return new C64BasicState(this);
            }

            public override Core.Expressions.Constant GetRegister(RegisterStorage r)
            {
                throw new NotImplementedException();
            }

            public override void SetRegister(RegisterStorage r, Core.Expressions.Constant v)
            {
            }

            public override void SetInstructionPointer(Address addr)
            {
            }

            public override void OnProcedureEntered()
            {
            }

            public override void OnProcedureLeft(FunctionType procedureSignature)
            {
            }

            public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
            {
                return new CallSite(2, 0);
            }

            public override void OnAfterCall(FunctionType sigCallee)
            {
            }
        }
    }
}