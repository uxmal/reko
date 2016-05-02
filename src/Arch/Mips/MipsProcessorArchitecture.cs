#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Mips
{
    public abstract class MipsProcessorArchitecture : ProcessorArchitecture
    {
        public MipsProcessorArchitecture()
        {
            this.WordWidth = PrimitiveType.Word32;
            this.PointerType = PrimitiveType.Word32;
            this.FramePointerType = PrimitiveType.Word32;
            this.InstructionBitSize = 32;
            this.StackRegister = Registers.sp;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new MipsDisassembler(this, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new MipsProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new MipsRewriter(
                this,
                new MipsDisassembler(this, rdr),
                frame,
                host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (uint)a.ToLinear()).ToHashSet();
            return new MipsPointerScanner32(rdr, knownLinAddresses, flags).Select(l => Address.Ptr32(l));
        }

        public override RegisterStorage GetRegister(int i)
        {
            if (i >= Registers.generalRegs.Length)
                return null;
            return Registers.generalRegs[i];
        }

        public override RegisterStorage GetRegister(string name)
        {
            return Registers.mpNameToReg[name];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.generalRegs;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.mpNameToReg.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(uint grf)
        {
            if (grf != 0)   // MIPS has no traditional status register.
                throw new NotSupportedException();
            return "";
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }

    public class MipsBe32Architecture : MipsProcessorArchitecture
    {
        public override ImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override ImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override ImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new BeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }

    public class MipsLe32Architecture : MipsProcessorArchitecture
    {
        public override ImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override ImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override ImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }
}