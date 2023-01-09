#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Arch.Cray.Ymp;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.Cray
{
    /// <summary>
    /// Architecture support for CrayYMP.
    /// </summary>
    /// <remarks>
    /// What's odd about this architecture? Well:
    /// - Memory is not byte-addressable, but word-addressable (where words are 64 bits)
    /// - But program memory is addressable by 16-bit "parcels"; 
    /// This presents a special challenge when decompiling to a byte-addressable 
    /// abstract machine.
    /// </remarks>
    public class CrayYmpArchitecture : ProcessorArchitecture
    {
        private InstructionSet instructionSet;
        private Decoder<YmpDisassembler, Mnemonic, CrayInstruction> rootDecoder;

        public CrayYmpArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, new(), new())
        {
            this.DefaultBase = 8;
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 16;
            this.MemoryGranularity = 64;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;          // Fake register, YMP has no architecture defined SP.
            this.WordWidth = PrimitiveType.Word64;
            this.instructionSet = CreateInstructionSet();
            this.rootDecoder = instructionSet.CreateDecoder();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Ymp.YmpDisassembler(this, rootDecoder, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        private InstructionSet CreateInstructionSet()
        {
            if (Options.TryGetValue("Model", out var oModel))
            {
                switch (((string) oModel).ToLower())
                {
                case "c90": return new C90InstructionSet();
                }
            }
            return new YmpInstructionSet();
        }

        public override MemoryArea CreateCodeMemoryArea(Address addr, byte[] bytes)
        {
            // NOTE: assumes the bytes are provided in big-endian form.
            var words = new ulong[bytes.Length / 8];
            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = ByteMemoryArea.ReadBeUInt64(bytes, i * 8);
            }
            return new Word64MemoryArea(addr, words);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new CrayProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new YmpRewriter(this, rootDecoder, rdr, state, binder, host);
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
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
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

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            this.Options = options ?? Options;
            this.instructionSet = CreateInstructionSet();
            this.rootDecoder = instructionSet.CreateDecoder();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override string RenderInstructionOpcode(MachineInstruction instr, EndianImageReader rdr)
        {
            var bitSize = this.InstructionBitSize;
            var instrSize = PrimitiveType.CreateWord(bitSize);
            var sb = new StringBuilder();
            var numBase = this.DefaultBase;
            int digits = 6;
            for (int i = 0; i < instr.Length; ++i)
            {
                if (rdr.TryRead(instrSize, out var v))
                {
                    sb.Append(Convert.ToString((long) v.ToUInt64(), numBase)
                        .PadLeft(digits, '0'));
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
