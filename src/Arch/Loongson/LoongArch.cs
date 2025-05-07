#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Loongson
{
    // https://loongson.github.io/LoongArch-Documentation/LoongArch-Vol1-EN.html
    public class LoongArch : ProcessorArchitecture
    {
        public LoongArch(IServiceProvider services, string name, Dictionary<string, object> options)
            : base(services, name, options, MakeRegisterBank(options))
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 32;
            this.MemoryGranularity = 8;
            this.SignedWord = null!;        // Will be set by LoadUserOptions.
            LoadUserOptions(options);
            this.GpRegisters = base.RegisterBank.GetRegistersByDomain(0, 32).ToArray();
            this.FpRegisters = base.RegisterBank.GetRegistersByDomain(32, 64).ToArray();
            this.CfRegisters = base.RegisterBank.GetRegistersByDomain(64, 96).ToArray();
            this.StackRegister = GpRegisters[3];
        }

        public RegisterStorage[] GpRegisters { get; }
        public RegisterStorage[] FpRegisters { get; }
        public RegisterStorage[] CfRegisters { get; }

        public PrimitiveType SignedWord { get; private set; }

        private static RegisterBank MakeRegisterBank(Dictionary<string, object> options)
        {
            var w = GetWordWidthFromOptions(options);
            var factory = new StorageFactory();
            factory.RangeOfReg(32, n => $"r{n}", w);
            factory.RangeOfReg(32, n => $"f{n}", PrimitiveType.Word64);
            factory.RangeOfReg(32, n => $"c{n}", PrimitiveType.Word32);
            return new RegisterBank(factory.NamesToRegisters.Values);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new LoongArchDisassembler(this, imageReader);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            return new LoongArchInstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new DefaultProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new LoongArchRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, ulong grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, ulong grf)
        {
            throw new NotImplementedException();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            PrimitiveType w = GetWordWidthFromOptions(options);
            this.WordWidth = w;
            long bitsize = w.BitSize;
            this.FramePointerType = PrimitiveType.Create(Domain.Pointer, bitsize);
            this.PointerType = this.FramePointerType;
            this.SignedWord = PrimitiveType.Create(Domain.SignedInt, bitsize);
        }

        private static PrimitiveType GetWordWidthFromOptions(Dictionary<string, object>? options)
        {
            int bitsize;
            if (options is not null && options.TryGetValue(ProcessorOption.WordSize, out var oSize))
            {
                bitsize = oSize switch
                {
                    int b => b,
                    long l => (int) l,
                    string s => Convert.ToInt32(s),
                    _ => throw new ArgumentException(ProcessorOption.WordSize)
                };
            }
            else
            {
                bitsize = 32;
            }
            return PrimitiveType.CreateWord(bitsize);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            if (this.WordWidth.BitSize == 32)
                return Address.TryParse32(txtAddr, out addr);
            else
                return Address.TryParse64(txtAddr, out addr);
        }
    }
}
