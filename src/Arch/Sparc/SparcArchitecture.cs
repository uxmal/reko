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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcArchitecture : IProcessorArchitecture
    {
        private PrimitiveType wordWidth;
        private PrimitiveType pointerType;

        public SparcArchitecture(PrimitiveType wordWidth)
        {
            this.wordWidth = wordWidth;
            this.pointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);
        }

        #region IProcessorArchitecture Members

        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new SparcDisassembler(this, imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            return new SparcProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new SparcRewriter(this, rdr, (SparcProcessorState) state, frame, host);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            return new Frame(pointerType);
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public int InstructionBitSize { get { return 32; } }

        public string GrfToString(uint grf)
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

        public PrimitiveType FramePointerType
        {
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType PointerType
        {
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType WordWidth { get { return wordWidth; } }

        public RegisterStorage StackRegister
        {
            get { throw new NotImplementedException(); }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }


        #endregion
    }
}
