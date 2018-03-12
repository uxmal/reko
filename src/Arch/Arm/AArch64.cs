#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Arm
{
    public class Arm64Architecture : ProcessorArchitecture
    {

        private INativeArchitecture native;
        private Dictionary<string, RegisterStorage> regsByName;
        private RegisterStorage[] regsByNumber;
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Arm64Architecture(string archId) : base(archId)
        {
            this.InstructionBitSize = 32;
            this.FramePointerType = PrimitiveType.Ptr64;
            this.PointerType = PrimitiveType.Ptr64;
            this.WordWidth = PrimitiveType.Word64;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
            this.CarryFlagMask = 0;
            var unk = CreateNativeArchitecture("arm-64");
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);

            GetRegistersFromNative();
            StackRegister = regsByName["sp"];
        }

        private void GetRegistersFromNative()
        {
            int cRegs;
            IntPtr aRegs;
            native.GetAllRegisters(0, out cRegs, out aRegs);
            if (aRegs == null)
                throw new OutOfMemoryException();
            this.regsByName = new Dictionary<string, RegisterStorage>();
            var regsByNumber = new List<RegisterStorage> { null };
            NativeRegister nReg = new NativeRegister();
            int cb = Marshal.SizeOf(nReg);
            while (cRegs > 0)
            {
                nReg = (NativeRegister)Marshal.PtrToStructure(aRegs, typeof(NativeRegister));
                if (nReg.Name != null)
                {
                    var n = nReg.Name;
                    var i = nReg.Number;
                    var b = nReg.BitSize;
                    var reg = new RegisterStorage(n, i, 0, PrimitiveType.CreateWord(b / 8));
                    regsByName.Add(reg.Name, reg);
                    regsByNumber.Add(reg);
        }
                aRegs += cb;
                --cRegs;
            }
            this.regsByNumber = regsByNumber.ToArray();
        }


        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            var bytes = rdr.Bytes;
            ulong uAddr = rdr.Address.ToLinear();
            var hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            INativeDisassembler ndasm = null;

            try
            {
                ndasm = native.CreateDisassembler(hBytes.AddrOfPinnedObject(), bytes.Length, (int)rdr.Offset, uAddr);
                for (;;)
                {
                    INativeInstruction nInstr = ndasm.NextInstruction();
                    if (nInstr == null)
                        yield break;
                    else
                        yield return new Arm64Instruction(nInstr);
                }
            }
            finally
            {
                if (ndasm != null)
                {
                    ndasm = null;
                }
                if (hBytes != null && hBytes.IsAllocated)
                {
                    hBytes.Free();
                }
            }
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
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

        public override ProcessorState CreateProcessorState(SegmentMap map)
        {
            return new Arm64State(this, map);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return new SortedList<string, int>();
        }

        public override int? GetOpcodeNumber(string name)
        {
                return null;
        }

        public override RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            RegisterStorage reg;
            if (regsByName.TryGetValue(name, out reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regsByNumber.ToArray();
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            throw new NotSupportedException();
        }

        public override string GrfToString(uint grf)
        {
            throw new NotImplementedException();
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

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }

        public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
        {
            throw new NotImplementedException("Endianness is BE or LE");
        }

        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture(
           [MarshalAs(UnmanagedType.LPStr)] string archName);
    }
}

