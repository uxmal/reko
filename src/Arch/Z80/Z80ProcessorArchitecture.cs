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

namespace Reko.Arch.Z80
{
    public class Z80ProcessorArchitecture : ProcessorArchitecture
    {
        public Z80ProcessorArchitecture()
        {
            this.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.PointerType = PrimitiveType.Ptr16;
            this.WordWidth = PrimitiveType.Word16;
            this.StackRegister = Registers.sp;
            this.CarryFlagMask = (uint)FlagM.CF;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new Z80Disassembler(imageReader);
        }

        public override ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Z80ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Z80Rewriter(this, rdr, state, frame, host);
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
            return Registers.All;
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

        public override Core.Expressions.Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }


		public override string GrfToString(uint grf)
		{
			StringBuilder s = new StringBuilder();
			for (int r = Registers.S.Number; grf != 0; ++r, grf >>= 1)
			{
				if ((grf & 1) != 0)
					s.Append(Registers.GetRegister(r).Name);
			}
			return s.ToString();
		}

        public override bool TryParseAddress(string txtAddress, out Address addr)
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
            base(name, id, fileSlot, PrimitiveType.Word16)
        {
        }
    }

    public static class Registers
    {
        public static readonly Z80Register b = new HighByteRegister("b", 0, 1);
        public static readonly Z80Register c = new LowByteRegister("c", 1, 1);
        public static readonly Z80Register d = new HighByteRegister("d", 2, 2);
        public static readonly Z80Register e = new LowByteRegister("e", 3, 2);
        public static readonly Z80Register h = new HighByteRegister("h", 4, 3);
        public static readonly Z80Register l = new LowByteRegister("l", 5, 3);
        public static readonly Z80Register a = new LowByteRegister("a", 7, 0);

        public static readonly Z80Register bc = new WordRegister("bc", 8,  1);
        public static readonly Z80Register de = new WordRegister("de", 9,  2);
        public static readonly Z80Register hl = new WordRegister("hl", 10, 3);
        public static readonly Z80Register sp = new WordRegister("sp", 11, 4);
        public static readonly Z80Register ix = new WordRegister("ix", 12, 5);
        public static readonly Z80Register iy = new WordRegister("iy", 13, 6);
        public static readonly Z80Register af = new WordRegister("af", 14, 0);

        public static readonly RegisterStorage i = new LowByteRegister("i", 16, 7);
        public static readonly RegisterStorage r = new LowByteRegister("r", 17, 8);

        public static readonly RegisterStorage S = new RegisterStorage("S", 20, PrimitiveType.Bool);
        public static readonly RegisterStorage Z = new RegisterStorage("Z", 21, PrimitiveType.Bool);
        public static readonly RegisterStorage P = new RegisterStorage("P", 22, PrimitiveType.Bool);
        public static readonly RegisterStorage C = new RegisterStorage("C", 23, PrimitiveType.Bool);

        internal static RegisterStorage[] All;

        static Registers()
        {
            All = new RegisterStorage[] {
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
            return All[r];
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