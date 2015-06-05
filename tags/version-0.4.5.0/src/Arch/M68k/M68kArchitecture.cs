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
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kArchitecture : IProcessorArchitecture
    {
        public M68kArchitecture()
        {
        }

        public M68kDisassembler CreateDisassembler(ImageReader rdr)
        {
            return M68kDisassembler.Create68020(rdr);
        }

        IEnumerable<MachineInstruction> IProcessorArchitecture.CreateDisassembler(ImageReader rdr)
        {
            return CreateDisassembler(rdr);
        }

        public ProcessorState CreateProcessorState()
        {
            return new M68kState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet((int) Registers.Max);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new M68kPointerScanner(rdr, knownLinAddresses, flags).Select(li => Address.Ptr32(li));
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
            return new M68kProcedureSerializer(this, typeLoader, defaultCc);
        }

        public RegisterStorage GetRegister(int i)
        {
            return Registers.GetRegister(i);
        }

        public RegisterStorage GetRegister(string name)
        {
            var r = Registers.GetRegister(name);
            if (r == RegisterStorage.None)
                throw new ArgumentException(string.Format("'{0}' is not a register name.", name));
            return r;
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

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Rewriter(this, rdr, (M68kState) state, frame, host);
        }

        public Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            return new MemoryAccess(new BinaryExpression(
                Operator.IAdd, FramePointerType,
                frame.EnsureRegister(StackRegister), Constant.Word32(offset)),
                dataType);
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public Address CreateSegmentedAddress(int size, ImageReader rdr, ushort segBase)
        {
            throw new NotSupportedException("M68k architecture doesn't support segmented pointers.");
        }

        public int InstructionBitSize { get { return 16; } }

        private static RegisterStorage[] flagRegisters = {
            new RegisterStorage("C", 0, PrimitiveType.Bool),
            new RegisterStorage("V", 0, PrimitiveType.Bool),
            new RegisterStorage("Z", 0, PrimitiveType.Bool),
            new RegisterStorage("N", 0, PrimitiveType.Bool),
            new RegisterStorage("X", 0, PrimitiveType.Bool),
        };

        public string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            for (int r = 0; grf != 0; ++r, grf >>= 1)
            {
                if ((grf & 1) != 0)
                    s.Append(flagRegisters[r].Name);
            }
            return s.ToString();
        }

        public PrimitiveType FramePointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Pointer32; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Word32; }
        }

        public uint CarryFlagMask { get { return (uint) FlagM.CF; } }

        public RegisterStorage StackRegister { get { return Registers.a7; } }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }
}
