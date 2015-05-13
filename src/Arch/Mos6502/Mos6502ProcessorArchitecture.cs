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
using Decompiler.Core.Rtl;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompiler.Core.Serialization;

namespace Decompiler.Arch.Mos6502
{
    public class Mos6502ProcessorArchitecture : IProcessorArchitecture
    {
        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new Disassembler(imageReader.Clone());
        }

        public ProcessorState CreateProcessorState()
        {
            return new Mos6502ProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Rewriter(this, rdr.Clone(), state, frame, host);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            //$HM. The stack pointer is 8 bits on this processor.
            return new Frame(PrimitiveType.Ptr16);
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
            return Address.Ptr16(c.ToUInt16());
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public int InstructionBitSize { get { return 8; } }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType
        {
            get { return PrimitiveType.Byte; }      // Yup, stack pointer is a byte register (!)
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Ptr16; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Byte; }      // 8-bit, baby!
        }

        public RegisterStorage StackRegister
        {
            get { return Registers.s; }
        }

        public uint CarryFlagMask
        {
            get { return (uint) FlagM.CF; }
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }

    }

    public static class Registers
    {
        public static readonly RegisterStorage a = new RegisterStorage("a", 0, PrimitiveType.Byte);
        public static readonly RegisterStorage x = new RegisterStorage("x", 1, PrimitiveType.Byte);
        public static readonly RegisterStorage y = new RegisterStorage("y", 2, PrimitiveType.Byte);
        public static readonly RegisterStorage s = new RegisterStorage("s", 3, PrimitiveType.Byte);

        public static readonly RegisterStorage N = new RegisterStorage("N", 4, PrimitiveType.Byte);
        public static readonly RegisterStorage V = new RegisterStorage("V", 5, PrimitiveType.Byte);
        public static readonly RegisterStorage C = new RegisterStorage("C", 6, PrimitiveType.Byte);
        public static readonly RegisterStorage Z = new RegisterStorage("Z", 7, PrimitiveType.Byte);
        public static readonly RegisterStorage I = new RegisterStorage("I", 8, PrimitiveType.Byte);
        public static readonly RegisterStorage D = new RegisterStorage("D", 9, PrimitiveType.Byte);
        
        private static RegisterStorage[] regs;

        public static RegisterStorage GetRegister(int reg)
        {
            return regs[reg];
        }

        static Registers()
        {
            regs = new RegisterStorage[]
            {
                a,
                x,
                y,
                s,

                N,
                V,
                C,
                Z,
                I,
                D,
            };
        }
    }
}
