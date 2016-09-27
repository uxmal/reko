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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.Xtensa
{
    public class XtensaArchitecture : ProcessorArchitecture
    {
        public XtensaArchitecture() 
        {
            this.InstructionBitSize = 8;        // Instruction alignment, really.
            this.FramePointerType = PrimitiveType.Pointer32;
            this.PointerType = PrimitiveType.Pointer32;
            this.WordWidth = PrimitiveType.Word32;
        }

        private static RegisterStorage[] regs = new[]
        {
            Registers.a0 ,
            Registers.a1 ,
            Registers.a2 ,
            Registers.a3 ,
            Registers.a4 ,
            Registers.a5 ,
            Registers.a6 ,
            Registers.a7 ,
            Registers.a8 ,
            Registers.a9 ,
            Registers.a10,
            Registers.a11,
            Registers.a12,
            Registers.a13,
            Registers.a14,
            Registers.a15,
        };

        private static Dictionary<int, RegisterStorage> sregs = new Dictionary<int, RegisterStorage>
        {
            { 0xE7, new RegisterStorage("VECBASE", 0x1E7, 0, PrimitiveType.Pointer32) }
        };

        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader rdr)
        {
            return new XtensaDisassembler(this, rdr);
        }

        public override ImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            //$TODO: Xtensa is bi-endian, but we're assuming little-endian here.
            // Fix this if encountering a big-endian binary.
            return new LeImageReader(img, off);
        }

        public override ImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            //$TODO: Xtensa is bi-endian, but we're assuming little-endian here.
            // Fix this if encountering a big-endian binary.
            return new LeImageReader(img, addr);
        }

        public override ImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter()
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter(MemoryArea img, Address addr)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new XtensaProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetAluRegister(int i)
        {
            return regs[i];
        }

        public RegisterStorage GetSpecialRegister(int sr)
        {
            return sregs[sr];
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
        {
            return regs[i];
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }
}
