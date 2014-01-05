#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mos6502
{
    public class Mos6502ProcessorArchitecture : IProcessorArchitecture
    {
        public IDisassembler CreateDisassembler(ImageReader imageReader)
        {
            return new Disassembler(imageReader.CreateLeReader());
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
            return new Rewriter(this, rdr.CreateLeReader(), state, frame);
        }

        public IEnumerable<uint> CreateCallInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
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

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

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
    }

    public static class Registers
    {
        public static readonly RegisterStorage a = new RegisterStorage("a", 0, PrimitiveType.Byte);
        public static readonly RegisterStorage x = new RegisterStorage("x", 1, PrimitiveType.Byte);
        public static readonly RegisterStorage y = new RegisterStorage("y", 2, PrimitiveType.Byte);
        public static readonly RegisterStorage s = new RegisterStorage("s", 3, PrimitiveType.Byte);
    }
}
