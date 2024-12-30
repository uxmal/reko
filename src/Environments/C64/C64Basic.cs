#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Oh why the hell not. C64 Basic can be interpreted as a machine
    /// language of sorts.
    /// </summary>
    public class C64Basic : ProcessorArchitecture
    {
        private static readonly RegisterStorage stackRegister = new RegisterStorage("sp", 1, 0, PrimitiveType.Ptr16);

        private readonly SortedList<ushort, C64BasicInstruction> program;
        private readonly BTreeDictionary<Address, C64BasicInstruction> mpAddrToInstr;

        public C64Basic(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.Description = "Commodore 64 Basic";
            this.Endianness = EndianServices.Little;
            this.PointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 8;
            this.StackRegister = stackRegister;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.WordWidth = PrimitiveType.Word16;
            program = new SortedList<ushort, C64BasicInstruction>();
            mpAddrToInstr = new BTreeDictionary<Address, C64BasicInstruction>();
        }

        public C64Basic(IServiceProvider services, SortedList<ushort, C64BasicInstruction> program) : this(services, "c64Basic", new Dictionary<string, object>())
        {
            this.program = program;
            foreach (var instr in program.Values)
            {
                mpAddrToInstr[instr.Address] = instr;
            }
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            if (!mpAddrToInstr.TryGetLowerBound(imageReader.Address, out var instr))
                yield break;
            int i = program.IndexOfKey(instr.LineNumber);
            if (i < 0)
                yield break;
            for (; i < program.Count; ++i)
            {
                yield return program.Values[i];
            }
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
            return new C64BasicRewriter(this, rdr.Address, program, this.mpAddrToInstr, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Array.Empty<RegisterStorage>();
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

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enumerable.Range(0, C64BasicInstruction.TokenMax - C64BasicInstruction.TokenMin)
                .ToSortedList(
                    v => C64BasicInstruction.TokenStrs[v],
                    v => v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            int i = Array.IndexOf(C64BasicInstruction.TokenStrs, name);
            if (i < 0)
                return null;
            else
                return i + C64BasicInstruction.TokenMin;
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            Services.RequireService<IEventListener>().Warn(
                "Basic doesn't have the notion of a parameter stack.");
            var stg = new TemporaryStorage("sp" + cbOffset, 0, dataType);
            return new Identifier("sp" + cbOffset, dataType, stg);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
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
                return InvalidConstant.Create(r.DataType);
            }

            public override void SetRegister(RegisterStorage r, Core.Expressions.Constant v)
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

            public override void OnAfterCall(FunctionType? sigCallee)
            {
            }
        }
    }
}