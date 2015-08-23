#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System.Text;

namespace Reko.Arch.Sparc
{
    public class SparcArchitecture : ProcessorArchitecture
    {
        public SparcArchitecture(PrimitiveType wordWidth)
        {
            this.WordWidth = wordWidth;
            this.PointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);
            this.FramePointerType = PointerType;
            this.InstructionBitSize = 32;
        }

        #region IProcessorArchitecture Members

        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new SparcDisassembler(this, imageReader);
        }

        public override ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new SparcProcessorState(this);
        }

        public override BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new SparcRewriter(this, rdr, (SparcProcessorState)state, frame, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
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

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
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

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }


        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & Registers.N.FlagGroupBits) != 0) s.Append(Registers.N.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) s.Append(Registers.Z.Name);
            if ((grf & Registers.V.FlagGroupBits) != 0) s.Append(Registers.V.Name);
            if ((grf & Registers.C.FlagGroupBits) != 0) s.Append(Registers.C.Name);

            if ((grf & Registers.E.FlagGroupBits) != 0) s.Append(Registers.E.Name);
            if ((grf & Registers.L.FlagGroupBits) != 0) s.Append(Registers.L.Name);
            if ((grf & Registers.G.FlagGroupBits) != 0) s.Append(Registers.G.Name);
            if ((grf & Registers.U.FlagGroupBits) != 0) s.Append(Registers.U.Name);

            return s.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
        #endregion
    }
}