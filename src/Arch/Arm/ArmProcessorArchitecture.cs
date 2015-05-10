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
using Decompiler.Core.Types;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompiler.Core.Serialization;
using System.Globalization;

namespace Decompiler.Arch.Arm
{
    public class ArmProcessorArchitecture : IProcessorArchitecture
    {
        public ArmProcessorArchitecture()
        {
        }

        #region IProcessorArchitecture Members

        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new ArmDisassembler(this, imageReader);
            //return new ArmDisassembler2(this, imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            return new ArmProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new ArmRewriter(this, rdr, (ArmProcessorState)state, frame, host);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            if (flags != PointerScannerFlags.Calls)
                throw new NotImplementedException(string.Format("Haven't implemented support for scanning for {0} yet.", flags));
            while (rdr.IsValid)
            {
                uint linAddrCall =  rdr.Address.ToUInt32();
                var opcode = rdr.ReadLeUInt32();
                if ((opcode & 0x0F000000) == 0x0B000000)         // BL
                {
                    int offset = ((int)opcode << 8) >> 6;
                    uint target = (uint)(linAddrCall + 8 + offset);
                    if (knownLinAddresses.Contains(target))
                        yield return Address.Ptr32(linAddrCall);
                }
            }
        }


        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetRegister(int i)
        {
            return A32Registers.GpRegs[i];
        }

        public RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
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

        public int InstructionBitSize { get { return 32; } }

        public BitSet ImplicitArgumentRegisters
        {
            get { throw new NotImplementedException(); }
        }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType
        {
            get { return StackRegister.DataType; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Word32; }
        }

        public RegisterStorage StackRegister
        {
            get { return A32Registers.sp; }
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