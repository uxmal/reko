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
using System.Linq;

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

        public abstract IEnumerable<Address> CreateInstructionScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);

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

        public abstract bool TryParseAddress(string txtAddress, out Address addr);
    }

    internal class RealMode : ProcessorMode
    {
        public RealMode()
            : base(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32)
        {
        }

        public override IEnumerable<Address> CreateInstructionScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (uint)a.ToLinear()).ToHashSet();
            return new X86RealModePointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return ReadSegmentedCodeAddress(byteSize, rdr, state);
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            if (txtAddress != null)
            {
                int c = txtAddress.IndexOf(':');
                if (c > 0)
                {
                    try
                    {
                        addr = Address.SegPtr(
                            Convert.ToUInt16(txtAddress.Substring(0, c), 16),
                            Convert.ToUInt32(txtAddress.Substring(c + 1), 16));
                        return true;
                    }
                    catch { }
                }
            }
            addr = null;
            return false;
		}        
    }

    internal class SegmentedMode : ProcessorMode
    {
        public SegmentedMode()
            : base(PrimitiveType.Word16, PrimitiveType.Ptr16, PrimitiveType.Pointer32)
        {
        }

        public override X86Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new X86Disassembler(rdr, PrimitiveType.Word16, PrimitiveType.Word16, false);
        }

        public override IEnumerable<Address> CreateInstructionScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int byteSize, ImageReader rdr, ProcessorState state)
        {
            return ReadSegmentedCodeAddress(byteSize, rdr, state);
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            throw new NotImplementedException();
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
            return Address.Ptr32(offset);
        }

        public override IEnumerable<Address> CreateInstructionScanner(
            ImageMap map,
            ImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinaddresses = knownAddresses.Select(a => (uint)a.ToLinear()).ToHashSet();
            return new X86PointerScanner32(rdr, knownLinaddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
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
            return Address.Ptr32(rdr.ReadLeUInt32());
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
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
            return Address.Ptr64(offset);
        }

        public override IEnumerable<Address> CreateInstructionScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (ulong)a.ToLinear()).ToHashSet();
            return new X86PointerScanner64(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
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

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }
    }
}