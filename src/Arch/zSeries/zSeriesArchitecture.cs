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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006 // Naming Styles

    public class zSeriesArchitecture : ProcessorArchitecture
    {
        private readonly zSeriesIntrinsics intrinsics;

        public zSeriesArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.ByName, Registers.ByDomain)
        {
            this.Endianness = EndianServices.Big;
            this.InstructionBitSize = 16;
            SetOptionDependentProperties();
            this.intrinsics = new zSeriesIntrinsics(this);
        }

        // zSeries uses a link register
        public override int ReturnAddressOnStack => 0;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new zSeriesDisassembler(this, imageReader);
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
            return new zSeriesState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new zSeriesRewriter(this, this.intrinsics, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            //$BUG: not close to correct but it's a start.
            return Registers.CC;
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

        public RegisterStorage GetRegister(int i)
        {
            return Registers.GpRegisters[i];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegisters;
        }

        public override FlagGroupStorage[] GetFlags()
        {
            return new[] { Registers.CC };
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            yield return Registers.CC;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            //$BUG: this is clearly not correct.
            return "CC";
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            Options = options ?? new Dictionary<string, object>();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        private bool Is64Bit()
        {
            return
                !Options.TryGetValue(ProcessorOption.WordSize, out var oWordSize) ||
                int.TryParse(oWordSize.ToString(), out var wordSize) &&
                wordSize == 64;
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return Options;
        }

        private void SetOptionDependentProperties()
        {
            if (Is64Bit())
            {
                this.WordWidth = PrimitiveType.Word64;
                this.PointerType = PrimitiveType.Ptr64;
                this.FramePointerType = PrimitiveType.Ptr64;
            }
            else
            {
                this.WordWidth = PrimitiveType.Word32;
                this.PointerType = PrimitiveType.Ptr32;
                this.FramePointerType = PrimitiveType.Ptr32;
            }
            //$REVIEW: is this architectural?
            this.StackRegister = Registers.GpRegisters[15];
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(txtAddr, out addr);
        }
    }
}
