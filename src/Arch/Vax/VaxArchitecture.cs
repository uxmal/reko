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
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System.Globalization;

namespace Reko.Arch.Vax
{
    public class VaxArchitecture : ProcessorArchitecture
    {
        private static RegisterStorage[] regs = new[]
        {
            new RegisterStorage("r0", 0, 0, PrimitiveType.Word32),
            new RegisterStorage("r1", 1, 0, PrimitiveType.Word32),
            new RegisterStorage("r2", 2, 0, PrimitiveType.Word32),
            new RegisterStorage("r3", 3, 0, PrimitiveType.Word32),

            new RegisterStorage("r4", 4, 0, PrimitiveType.Word32),
            new RegisterStorage("r5", 5, 0, PrimitiveType.Word32),
            new RegisterStorage("r6", 6, 0, PrimitiveType.Word32),
            new RegisterStorage("r7", 7, 0, PrimitiveType.Word32),

            new RegisterStorage("r8", 8, 0, PrimitiveType.Word32),
            new RegisterStorage("r9", 9, 0, PrimitiveType.Word32),
            new RegisterStorage("r10", 10, 0, PrimitiveType.Word32),
            new RegisterStorage("r11", 11, 0, PrimitiveType.Word32),

            new RegisterStorage("ap", 12, 0, PrimitiveType.Word32),
            new RegisterStorage("fp", 13, 0, PrimitiveType.Word32),
            new RegisterStorage("sp", 14, 0, PrimitiveType.Word32),
            new RegisterStorage("pc", 15, 0, PrimitiveType.Word32),

        };

        public VaxArchitecture()
        {
            InstructionBitSize = 8;

        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new VaxDisassembler(this, imageReader);
        }

        public override ImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            return new LeImageReader(img, off);
        }

        public override ImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public override ImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(img, addrBegin, addrEnd);
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

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
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
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            uint uAddr;
            if (!uint.TryParse(txtAddr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uAddr))
            {
                addr = null;
                return false;
            }
            else
            {
                addr = Address.Ptr32(uAddr);
                return true;
            }
        }
    }
}
