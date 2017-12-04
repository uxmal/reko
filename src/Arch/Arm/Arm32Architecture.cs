#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System.Runtime.InteropServices;
using Reko.Core.NativeInterface;
using System.Diagnostics;
using System.Collections;
using Reko.Core.Lib;
using Reko.Core.Operators;

namespace Reko.Arch.Arm
{
    public class Arm32Architecture : ProcessorArchitecture
    {
        private INativeArchitecture native;
        private Dictionary<string, RegisterStorage> regsByName;
        private RegisterStorage[] regsByNumber;
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Arm32Architecture()
        {
            InstructionBitSize = 32;
            FramePointerType = PrimitiveType.Pointer32;
            PointerType = PrimitiveType.Pointer32;
            WordWidth = PrimitiveType.Word32;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();

            var unk = CreateNativeArchitecture("arm");
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);

            GetRegistersFromNative();
            StackRegister = regsByName["sp"];
        }

        private void GetRegistersFromNative()
        {
            int cRegs;
            IntPtr aRegs;
            native.GetAllRegisters(out cRegs, out aRegs);
            if (aRegs == null)
                throw new OutOfMemoryException();
            this.regsByName = new Dictionary<string, RegisterStorage>();
            var regsByNumber = new List<RegisterStorage> { null };
            NativeRegister nReg = new NativeRegister();
            int cb = Marshal.SizeOf(nReg);
            while (cRegs > 0)
            {
                nReg = (NativeRegister) Marshal.PtrToStructure(aRegs, typeof(NativeRegister));
                if (nReg.Name != null)
                {
                    var n = nReg.Name;
                    var i = nReg.Number;
                    var b = nReg.BitSize;
                    var reg = new RegisterStorage(n, i, 0, PrimitiveType.CreateWord(b/8));
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
                        yield return new Arm32Instruction(nInstr);
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

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(img, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            return new LeImageReader(img, off);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea img, Address addr)
        {
            return new LeImageWriter(img, addr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return null;
        }

        public override ProcessorState CreateProcessorState()
        {
            return new ArmProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            if (flags != PointerScannerFlags.Calls)
                throw new NotImplementedException(string.Format("Haven't implemented support for scanning for {0} yet.", flags));
            while (rdr.IsValid)
            {
                uint linAddrCall = rdr.Address.ToUInt32();
                var opcode = rdr.ReadLeUInt32();
                if ((opcode & 0x0F000000) == 0x0B000000)         // BL
                {
                    int offset = ((int)opcode << 8) >> 6;
                    uint target = (uint)(linAddrCall + 8 + offset);
                    if (knownLinAddresses.Contains(target))
                        yield return Address.Ptr32(linAddrCall);
                }
            }
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new ArmRewriterNew(this, rdr, (ArmProcessorState) state, binder, host);
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int cbOffset, DataType dataType)
        {
            return new MemoryAccess(new BinaryExpression(
                                    Operator.IAdd, FramePointerType,
                                    frame.EnsureRegister(StackRegister), Constant.Word32(cbOffset)),
                                    dataType);
        }

        public override RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < regsByNumber.Length)
                return regsByNumber[i];
            else
                return null;
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
            // First element is "Invalid".
            return regsByNumber.Skip(1).ToArray();
        }

        public override int? GetOpcodeNumber(string name)
        {
            //$TOD: write a dictionary mapping ARM instructions to ARM_INS_xxx.
            return null; 
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            //$TOD: write a dictionary mapping ARM instructions to ARM_INS_xxx.
            return new SortedList<string, int>();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return regsByName.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            FlagGroupStorage f;
            if (flagGroups.TryGetValue(grf, out f))
            {
                return f;
            }

            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var flagregister = (FlagRegister)this.regsByName["cpsr"];
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            if ((grf & (uint)FlagM.VF) != 0) s.Append('V');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture(
            [MarshalAs(UnmanagedType.LPStr)] string archName);
    }
}
