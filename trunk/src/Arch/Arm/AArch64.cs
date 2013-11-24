#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class AArch64 : IProcessorArchitecture
    {
        public IDisassembler CreateDisassembler(ImageReader imageReader)
        {
            return new AArch64Disassembler(imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Core.Rtl.RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
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

        public Core.Expressions.Expression CreateStackAccess(Frame frame, int cbOffset, Core.Types.DataType dataType)
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

        public Core.Types.PrimitiveType FramePointerType
        {
            get { throw new NotImplementedException(); }
        }

        public Core.Types.PrimitiveType PointerType
        {
            get { throw new NotImplementedException(); }
        }

        public Core.Types.PrimitiveType WordWidth
        {
            get { throw new NotImplementedException(); }
        }

        public RegisterStorage StackRegister
        {
            get { throw new NotImplementedException(); }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }
    }
}
