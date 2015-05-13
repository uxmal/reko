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

namespace Decompiler.Arch.Mips
{
    public class MipsProcessorArchitecture : IProcessorArchitecture
    {
        public MipsProcessorArchitecture()
        {
            this.WordWidth = PrimitiveType.Word32;
            this.PointerType = PrimitiveType.Word32;
            this.FramePointerType = PrimitiveType.Word32;
        }

        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new MipsDisassembler(this, imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
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
            return Registers.generalRegs[i];
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
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType { get; private set; }
        public PrimitiveType PointerType { get; private set; }
        public PrimitiveType WordWidth { get; private set ; }

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
            return Address.TryParse16(txtAddress, out addr);
        }

    }
}
