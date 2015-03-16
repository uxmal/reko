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
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Arch.X86
{
    public abstract class ProcessorMode
    {
        public static readonly ProcessorMode Real = new RealMode();
        public static readonly ProcessorMode ProtectedSegmented = new SegmentedMode();
        public static readonly ProcessorMode Protected32 = new FlatMode32();
        public static readonly ProcessorMode Protected64 = new FlatMode64();

        protected ProcessorMode(PrimitiveType wordSize, PrimitiveType framePointerType, PrimitiveType pointerType)
        {
            this.WordWidth = wordSize;
            this.FramePointerType = framePointerType;
            this.PointerType = pointerType;
        }

        public virtual Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return state.AddressFromSegOffset(seg, offset);
        }

        public PrimitiveType FramePointerType { get; private set ; }

        public PrimitiveType PointerType { get; private set; }

        public PrimitiveType WordWidth { get; private set; }

        public virtual RegisterStorage StackRegister
        {
            get { return Registers.sp; }
        }

        public abstract IEnumerable<uint> CreateInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags);

        public abstract X86Disassembler CreateDisassembler(ImageReader rdr);
        
        public virtual Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            var sp = frame.EnsureRegister(Registers.sp);
            var ss = frame.EnsureRegister(Registers.ss);
            return SegmentedAccess.Create(ss, sp, offset, dataType);
        }

        public abstract Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state);

        protected Address ReadSegmentedCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            if (byteSize == PrimitiveType.Word16.Size)
            {
                return Address.SegPtr(state.GetRegister(Registers.cs).ToUInt16(), rdr.ReadLeUInt16());
            }
            else
            {
                ushort off = rdr.ReadLeUInt16();
                ushort seg = rdr.ReadLeUInt16();
                return Address.SegPtr(seg, off);
            }
        }

        public virtual uint GetAddressOffset(Address addr) { return addr.Linear; }
    }

    internal class RealMode : ProcessorMode
    {
        public RealMode()
            : base(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32)
        {
        }

        public override IEnumerable<uint> CreateInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            return new X86RealModePointerScanner(rdr, knownLinAddresses, flags);
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return ReadSegmentedCodeAddress(byteSize, rdr, state);
        }

        public override uint GetAddressOffset(Address addr)
        {
            return addr.Offset;
        }
    }

    internal class SegmentedMode : ProcessorMode
    {
        public SegmentedMode()
            : base(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32)
        {
        }

        public override IEnumerable<uint> CreateInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return ReadSegmentedCodeAddress(byteSize, rdr, state);
        }
    }

    internal class FlatMode32 : ProcessorMode
    {
        internal FlatMode32()
            : base(PrimitiveType.Word32, PrimitiveType.Pointer32, PrimitiveType.Pointer32)
        {
        }

        public override RegisterStorage StackRegister
        {
            get { return Registers.esp; }
        }

        public override Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return new Address(offset);
        }

        public override IEnumerable<uint> CreateInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            return new X86PointerScanner(rdr, knownLinAddresses, flags);
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
        }

        public override Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            var esp = frame.EnsureRegister(Registers.esp);
            return MemoryAccess.Create(esp, offset, dataType);
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return new Address(rdr.ReadLeUInt32());
        }
    }

    internal class FlatMode64 : ProcessorMode
    {
        internal FlatMode64()
            : base(PrimitiveType.Word64, PrimitiveType.Pointer64, PrimitiveType.Pointer64)
        {
        }

        public override RegisterStorage StackRegister
        {
            get { return Registers.rsp; }
        }

        public override Address AddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return new Address(offset);
        }

        public override IEnumerable<uint> CreateInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word64, true);
        }

        public override Expression CreateStackAccess(Frame frame, int offset, DataType dataType)
        {
            var rsp = frame.EnsureRegister(Registers.rsp);
            return MemoryAccess.Create(rsp, offset, dataType);
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return Address.Ptr64(rdr.ReadLeUInt64());
        }
    }
}