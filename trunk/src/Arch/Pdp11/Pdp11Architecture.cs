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
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Registers
    {
        public static RegisterStorage r0 = new RegisterStorage("r0", 0, PrimitiveType.Word16);
        public static RegisterStorage r1 = new RegisterStorage("r1", 1, PrimitiveType.Word16);
        public static RegisterStorage r2 = new RegisterStorage("r2", 2, PrimitiveType.Word16);
        public static RegisterStorage r3 = new RegisterStorage("r3", 3, PrimitiveType.Word16);
        public static RegisterStorage r4 = new RegisterStorage("r4", 4, PrimitiveType.Word16);
        public static RegisterStorage r5 = new RegisterStorage("r5", 5, PrimitiveType.Word16);
        public static RegisterStorage sp = new RegisterStorage("sp", 6, PrimitiveType.Word16);
        public static RegisterStorage pc = new RegisterStorage("pc", 7, PrimitiveType.Word16);
    }

    [Flags]
    public enum FlagM
    {
        NF = 1,
        ZF = 2,
        VF = 4,
        CF = 8,
    }

    public class Pdp11Architecture : IProcessorArchitecture
    {
        private RegisterStorage[] regs;

        public Pdp11Architecture()
        {
            regs = new RegisterStorage[] { 
                Registers.r0, Registers.r1, Registers.r2, Registers.r3, 
                Registers.r4, Registers.r5, Registers.sp, Registers.pc, };
        }

        #region IProcessorArchitecture Members

        public RegisterStorage StackRegister { get { return Registers.sp; } }
        public uint CarryFlagMask { get { throw new NotImplementedException(); } }

        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader rdr)
        {
            return new Pdp11Disassembler(rdr, this);
        }

        public Frame CreateFrame()
        {
            return new Frame(PrimitiveType.Word16);
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public ProcessorState CreateProcessorState()
        {
            return new Pdp11ProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet(16);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public RegisterStorage GetRegister(string name)
        {
            foreach (RegisterStorage reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return reg;
            }
            return null;
        }

        public bool TryGetRegister(string name, out RegisterStorage result)
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

        public FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public int InstructionBitSize { get { return 16; } }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType
        {
            get { return PrimitiveType.Word16; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Word16; }
        }

        public PrimitiveType WordWidth
        {
            get { throw new NotImplementedException(); }
        }

        public Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new Pdp11Rewriter(this, new Pdp11Disassembler(rdr, this), frame);
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }

        #endregion
    }
}
