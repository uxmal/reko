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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public class PaRiscArchitecture : ProcessorArchitecture
    {
        public PaRiscArchitecture(string archId) : base(archId)
        {
            InstructionBitSize = 32;
            StackRegister = Registers.GpRegs[30];
            Endianness = EndianServices.Big;
            Options = new Dictionary<string, object>();
            SetOptionDependentProperties();
        }

        public Dictionary<string, object> Options { get; }


        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new PaRiscDisassembler(this, rdr);
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

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegs;
        }

        public override string GrfToString(RegisterStorage reg, string str, uint grf)
        {
            return "";
        }

        public bool Is64Bit()
        {
            return
                Options.TryGetValue("WordSize", out var oWordSize) &&
                int.TryParse(oWordSize.ToString(), out var wordSize) &&
                wordSize == 64;
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            if (options == null)
                return;
            foreach (var option in options)
            {
                this.Options[option.Key] = option.Value;
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

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
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
        }


        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
