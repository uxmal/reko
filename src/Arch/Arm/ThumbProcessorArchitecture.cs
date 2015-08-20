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

namespace Reko.Arch.Arm
{
    public class ThumbProcessorArchitecture : IProcessorArchitecture
    {
        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new ThumbDisassembler(imageReader);
        }

        public Frame CreateFrame()
        {
            return new Frame(PrimitiveType.Pointer32);
        }

        public ImageReader CreateImageReader(LoadedImage img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public ImageReader CreateImageReader(LoadedImage img, ulong off)
        {
            throw new NotImplementedException();
        }

        public IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            throw new NotImplementedException();
        }

        public ProcessorState CreateProcessorState()
        {
            return new ArmProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet(0x30);
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new ThumbRewriter(this, rdr, (ArmProcessorState) state, frame, host);
        }

        public RegisterStorage GetRegister(int i)
        {
            return A32Registers.GpRegs[i];
        }

        public RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage[] GetRegisters()
        {
            return A32Registers.GpRegs.ToArray();
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

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType
        {
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Word32; }
        }

        public int InstructionBitSize
        {
            get { return 16; }
        }

        public RegisterStorage StackRegister
        {
            get { return A32Registers.sp; }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryParseAddress(string txtAddr, out Address addr)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            throw new NotImplementedException();
        }
    }
}
