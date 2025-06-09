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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.PaRisc
{
    public class PaRiscArchitecture : ProcessorArchitecture
    {
        public PaRiscArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null!, null!)
        {
            LoadUserOptions(options);
            InstructionBitSize = 32;
            StackRegister = this.Registers!.GpRegs[30];
            Endianness = EndianServices.Big;
        }

        public Registers Registers { get; private set; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new PaRiscDisassembler(this, rdr: rdr);
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
            return new PaRiscState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PaRiscRewriter(this, rdr, state, binder, host);
        }

        // PA-RISC uses a link register
        public override int ReturnAddressOnStack => 0;

        public override FlagGroupStorage GetFlagGroup(RegisterStorage reg, uint grf)
        {
            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var f = new FlagGroupStorage(reg, grf, GrfToString(reg, "", grf));
            return f;
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

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegs;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            return new FlagGroupStorage[] { flags };
        }

        public override string GrfToString(RegisterStorage reg, string str, uint grf)
        {
            return "C";
        }

        public bool Is64Bit()
        {
            return
                Options.TryGetValue(ProcessorOption.WordSize, out var oWordSize) &&
                int.TryParse(oWordSize.ToString(), out var wordSize) &&
                wordSize == 64;
        }

        public override bool IsStackArgumentOffset(long frameOffset)
        {
            // On PA-RISC, the stack grows toward higher memory accesses. Items
            // on the stack previous to a call with have
            // non-positive offsets with respect to the stack/frame pointer.
            // http://www.3kranger.com/HP3000/mpeix/en-hard/09740-90015/ch02s02.html
            return frameOffset <= 0;
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options is not null)
            {
                foreach (var option in options.ToList())
                {
                    this.Options[option.Key] = option.Value;
                }
            }
            SetOptionDependentProperties();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            if (Is64Bit())
            {
                var uAddr = c.ToUInt64();
                if (codeAlign)
                    uAddr &= ~3u;
                return Address.Ptr64(uAddr);
            }
            else
            {
                var uAddr = c.ToUInt32();
                if (codeAlign)
                    uAddr &= ~3u;
                return Address.Ptr32(uAddr);

            }
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return Options;
        }

        private void SetOptionDependentProperties()
        {
            if (Is64Bit())
            {
                WordWidth = PrimitiveType.Word64;
                PointerType = PrimitiveType.Ptr64;
                FramePointerType = PrimitiveType.Ptr64;
            }
            else
            {
                WordWidth = PrimitiveType.Word32;
                PointerType = PrimitiveType.Ptr32;
                FramePointerType = PrimitiveType.Ptr32;
            }
            this.Registers = new Registers(WordWidth);
            base.regsByName = Registers.ByName;
            base.regsByDomain = Registers.ByDomain;
        }


        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
