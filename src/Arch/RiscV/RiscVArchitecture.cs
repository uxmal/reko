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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Core.Operators;

namespace Reko.Arch.RiscV
{
    public class RiscVArchitecture : ProcessorArchitecture
    {
        static string [] regnames = {
            "zero", "ra", "sp", "gp", "tp", "t0", "t1", "t2",
            "s0",   "s1", "a0", "a1", "a2", "a3", "a4", "a5",
            "a6",   "a7", "s2", "s3", "s4", "s5", "s6", "s7",
            "s8",   "s9", "s10","s11","t3", "t4", "t5", "t6"
        };

        static string[]  fpuregnames = {
          "ft0", "ft1", "ft2",  "ft3",  "ft4", "ft5", "ft6",  "ft7",
          "fs0", "fs1", "fa0",  "fa1",  "fa2", "fa3", "fa4",  "fa5",
          "fa6", "fa7", "fs2",  "fs3",  "fs4", "fs5", "fs6",  "fs7",
          "fs8", "fs9", "fs10", "fs11", "ft8", "ft9", "ft10", "ft11"
        };

        private RegisterStorage[] regs;
        internal readonly RegisterStorage[] FpRegs;
        internal readonly RegisterStorage LinkRegister;
        internal readonly PrimitiveType NaturalSignedInteger;
        private Dictionary<string, RegisterStorage> regsByName;

        public RiscVArchitecture(string archId) : base(archId)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 16;
            //$TODO: what about 32-bit version of arch?
            this.PointerType = PrimitiveType.Ptr64;
            this.WordWidth = PrimitiveType.Word64;
            this.FramePointerType = PrimitiveType.Ptr64;
            this.NaturalSignedInteger = PrimitiveType.Int64;

            this.FpRegs = fpuregnames
                .Select((n, i) => new RegisterStorage(
                    n,
                    i + 32,
                    0,
                    PrimitiveType.Word64))
                .ToArray();
            this.regs = regnames
                .Select((n, i) => new RegisterStorage(
                    n,
                    i,
                    0,
                    PrimitiveType.Word64)) //$TODO: setting!
                .Concat(FpRegs)
                .ToArray();
            this.regsByName = regs.ToDictionary(r => r.Name);
            this.LinkRegister = regs[1];        // ra
            this.StackRegister = regs[2];       // sp
        }

        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        /// <remarks>
        /// Return address is not pushed directly on a stack in memory on
        /// RISC-V.
        /// </remarks>
        public override int ReturnAddressOnStack => 0;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new RiscVDisassembler(this, imageReader);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new RiscVInstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new RiscVState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new RiscVRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
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
            if (regsByName.TryGetValue(name, out RegisterStorage reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return regs[domain - StorageDomain.Register];
        }

        public RegisterStorage GetRegister(int i)
        {
            return regs[i];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            return "";
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            //$TODO: what about 32-bit? 
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
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
            //$TODO: what if 32-bit?
            return Address.TryParse64(txtAddr, out addr);
        }
    }
}
