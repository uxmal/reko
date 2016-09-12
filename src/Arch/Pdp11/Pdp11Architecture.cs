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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class Registers
    {
        public static RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word16);
        public static RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word16);
        public static RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word16);
        public static RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word16);
        public static RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word16);
        public static RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word16);
        public static RegisterStorage sp = new RegisterStorage("sp", 6, 0, PrimitiveType.Word16);
        public static RegisterStorage pc = new RegisterStorage("pc", 7, 0, PrimitiveType.Word16);

        public static RegisterStorage N = new RegisterStorage("N", 8, 0, PrimitiveType.Bool);
        public static RegisterStorage Z = new RegisterStorage("Z", 9, 0, PrimitiveType.Bool);
        public static RegisterStorage V = new RegisterStorage("V", 10, 0, PrimitiveType.Bool);
        public static RegisterStorage C = new RegisterStorage("C", 11, 0, PrimitiveType.Bool);

        public static FlagRegister psw = new FlagRegister("psw", PrimitiveType.Word16);
    }

    [Flags]
    public enum FlagM
    {
        NF = 1,
        ZF = 2,
        VF = 4,
        CF = 8,
    }

    public class Pdp11Architecture : ProcessorArchitecture
    {
        private RegisterStorage[] regs;
        private RegisterStorage[] flagRegs;
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Pdp11Architecture()
        {
            regs = new RegisterStorage[] { 
                Registers.r0, Registers.r1, Registers.r2, Registers.r3, 
                Registers.r4, Registers.r5, Registers.sp, Registers.pc, };
            flagRegs = new RegisterStorage[] 
            {
                Registers.N, Registers.Z, Registers.V, Registers.C
            };
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();

            InstructionBitSize = 16;
            StackRegister = Registers.sp;
            //CarryFlagMask  = { get { throw new NotImplementedException(); } }
            FramePointerType = PrimitiveType.Ptr16;
            PointerType = PrimitiveType.Ptr16;
            WordWidth = PrimitiveType.Word16;
        }

        #region IProcessorArchitecture Members


        public override IEnumerable<MachineInstruction> CreateDisassembler(ImageReader rdr)
        {
            return new Pdp11Disassembler(rdr, this);
        }

        public override ImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override ImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
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

        public override ProcessorState CreateProcessorState()
        {
            return new Pdp11ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public override RegisterStorage GetRegister(string name)
        {
            foreach (RegisterStorage reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return reg;
            }
            return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs;
        }

        public override bool TryGetRegister(string name, out RegisterStorage result)
        {
            result = null;
            foreach (RegisterStorage reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    result = reg;
                    return true;
                }
            }
            return false;
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            return reg;
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
		{
            FlagGroupStorage f;
            if (flagGroups.TryGetValue(grf, out f))
                return f;

			PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.psw, grf, GrfToString(grf), dt);
			flagGroups.Add(grf, fl);
			return fl;
		}

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }


        public override string GrfToString(uint grf)
        {
			var s = new StringBuilder();
			foreach (var r in flagRegs)
			{
                if (grf == 0)
                    break;
				if ((grf & 1) != 0)
					s.Append(r.Name);
                grf >>= 1;
			}
			return s.ToString();
		}

        public override Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Pdp11Rewriter(this, new Pdp11Disassembler(rdr, this), frame, host);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }

        #endregion
    }
}
