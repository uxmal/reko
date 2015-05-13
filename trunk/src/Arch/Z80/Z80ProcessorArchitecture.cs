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

namespace Decompiler.Arch.Z80
{
    public class Z80ProcessorArchitecture : IProcessorArchitecture
    {
        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new Z80Disassembler(imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            return new Z80ProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Z80Rewriter(this, rdr, state, frame, host);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
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

        public Core.Expressions.Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
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
			StringBuilder s = new StringBuilder();
			for (int r = Registers.S.Number; grf != 0; ++r, grf >>= 1)
			{
				if ((grf & 1) != 0)
					s.Append(Registers.GetRegister(r).Name);
			}
			return s.ToString();
		}

        public PrimitiveType FramePointerType
        {
            get { return PrimitiveType.Ptr16; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Ptr16; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Word16; }
        }

        public RegisterStorage StackRegister
        {
            get { return Registers.sp; }
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

    public abstract class Z80Register : RegisterStorage
    {
        public readonly int FileSlot;
        
        public Z80Register(string name, int id, int fileSlot, PrimitiveType dataType) :
            base(name, id, dataType)
        {
            this.FileSlot = fileSlot;
        }
    }

    public class LowByteRegister : Z80Register
    {
        public LowByteRegister(string name, int id, int fileSlot) :
            base(name, id, fileSlot,PrimitiveType.Byte)
        {
        }
    }

    public class HighByteRegister : Z80Register
    {
        public HighByteRegister(string name, int id, int fileSlot) :
            base(name, id, fileSlot, PrimitiveType.Byte)
        {
        }
    }

    public class WordRegister : Z80Register
    {
        public WordRegister(string name, int id, int fileSlot) : 
            base(name, id, fileSlot, PrimitiveType.Word32)
        {
        }
    }

    public static class Registers
    {
        public static readonly RegisterStorage b = new RegisterStorage("b", 0, PrimitiveType.Byte);
        public static readonly RegisterStorage c = new RegisterStorage("c", 1, PrimitiveType.Byte);
        public static readonly RegisterStorage d = new RegisterStorage("d", 2, PrimitiveType.Byte);
        public static readonly RegisterStorage e = new RegisterStorage("e", 3, PrimitiveType.Byte);
        public static readonly RegisterStorage h = new RegisterStorage("h", 4, PrimitiveType.Byte);
        public static readonly RegisterStorage l = new RegisterStorage("l", 5, PrimitiveType.Byte);
        public static readonly RegisterStorage a = new RegisterStorage("a", 7, PrimitiveType.Byte);

        public static readonly RegisterStorage bc = new RegisterStorage("bc", 8, PrimitiveType.Word16);
        public static readonly RegisterStorage de = new RegisterStorage("de", 9, PrimitiveType.Word16);
        public static readonly RegisterStorage hl = new RegisterStorage("hl", 10, PrimitiveType.Word16);
        public static readonly RegisterStorage sp = new RegisterStorage("sp", 11, PrimitiveType.Word16);
        public static readonly RegisterStorage ix = new RegisterStorage("ix", 12, PrimitiveType.Word16);
        public static readonly RegisterStorage iy = new RegisterStorage("iy", 13, PrimitiveType.Word16);
        public static readonly RegisterStorage af = new RegisterStorage("af", 14, PrimitiveType.Word16);

        public static readonly RegisterStorage i = new RegisterStorage("i", 16, PrimitiveType.Byte);
        public static readonly RegisterStorage r = new RegisterStorage("r", 17, PrimitiveType.Byte);

        public static readonly RegisterStorage S = new RegisterStorage("S", 20, PrimitiveType.Bool);
        public static readonly RegisterStorage Z = new RegisterStorage("Z", 21, PrimitiveType.Bool);
        public static readonly RegisterStorage P = new RegisterStorage("P", 22, PrimitiveType.Bool);
        public static readonly RegisterStorage C = new RegisterStorage("C", 23, PrimitiveType.Bool);

        private static RegisterStorage[] regs;

        static Registers()
        {
            regs = new RegisterStorage[] {
             b ,
             c ,
             d ,
             e ,

             h ,
             l ,
             a ,
             null,
               
             bc,
             de,
             hl,
             sp,
             ix,
             iy,
             af,
             null,

             i ,
             r ,
             null,
             null,

             S ,
             Z ,
             P ,
             C ,
            };
        }

        internal static RegisterStorage GetRegister(int r)
        {
            return regs[r];
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        SF = 1,             // sign
        ZF = 2,             // zero
        PF = 4,             // overflow / parity
        CF = 8,             // carry
    }

    public enum CondCode
    {
        nz,
        z,
        nc,
        c,
        po,
        pe,
        p,
        m,
    }
}