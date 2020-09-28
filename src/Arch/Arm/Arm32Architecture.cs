#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.Arm.AArch32;

namespace Reko.Arch.Arm
{
    // https://wiki.ubuntu.com/ARM/Thumb2PortingHowto
    public class Arm32Architecture : ProcessorArchitecture
    {
#if NATIVE
        private INativeArchitecture native;
        private Dictionary<string, RegisterStorage> regsByName;
        private Dictionary<int, RegisterStorage> regsByNumber;
#endif
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Arm32Architecture(string archId) : base(archId)
        {
            Endianness = EndianServices.Little;
            InstructionBitSize = 32;
            FramePointerType = PrimitiveType.Ptr32;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Word32;
            StackRegister = Registers.sp;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
#if NATIVE

            var unk = CreateNativeArchitecture("arm");
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);

            GetRegistersFromNative();
            StackRegister = regsByName["sp"];
#endif
        }

#if NATIVE
        private void GetRegistersFromNative()
        {
            this.regsByName = new Dictionary<string, RegisterStorage>();
            this.regsByNumber = new Dictionary<int, RegisterStorage>();
            GetRegisterOfType(0);
            GetRegisterOfType(1);
        }

        private void GetRegisterOfType(int registerKind)
        {
            native.GetAllRegisters(registerKind, out int cRegs, out IntPtr aRegs);
            if (aRegs == null)
                throw new OutOfMemoryException();
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
                    var a = (uint)nReg.BitOffset;
                    var reg = new RegisterStorage(n, i, a, PrimitiveType.CreateWord(b));
                    regsByName.Add(reg.Name, reg);
                    regsByNumber.Add(reg.Number, reg);
                }
                aRegs += cb;
                --cRegs;
            }
        }
#endif

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new A32Disassembler(this, rdr);
#if NATIVE
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
#endif
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return null;
        }

        public override ProcessorState CreateProcessorState()
        {
            return new AArch32ProcessorState(this);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            if (flags != PointerScannerFlags.Calls)
                throw new NotImplementedException(string.Format("Haven't implemented support for scanning for {0} yet.", flags));
            while (rdr.IsValid)
            {
                uint linAddrCall =  rdr.Address.ToUInt32();
                var wInstr = rdr.ReadLeUInt32();
                if ((wInstr & 0x0F000000) == 0x0B000000)         // BL
                {
                    int offset = ((int)wInstr << 8) >> 6;
                    uint target = (uint)(linAddrCall + 8 + offset);
                    if (knownLinAddresses.Contains(target))
                        yield return Address.Ptr32(linAddrCall);
                }
            }
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new ArmRewriter(this, rdr, host, binder);
#if NATIVE
            return new ArmRewriterRetired(regsByNumber, rdr, (ArmProcessorState) state, binder, host);
#endif
        }

        public override Core.CallingConvention GetCallingConvention(string name)
        {
            // At this point, we're falling back onto the architecture-defined
            // calling convention.
            return new Arm32CallingConvention();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            if (Registers.RegistersByDomain.TryGetValue(domain, out var reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage GetRegister(string name)
        {
            if (Registers.RegistersByName.TryGetValue(name, out var reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
#if NATIVE
            // First element is "Invalid".
            return regsByNumber.Values.OrderBy(r => r.Number).ToArray();
#else
            return Registers.GpRegs;
#endif
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.NF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.NF);
            if ((grf & (uint) FlagM.ZF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.ZF);
            if ((grf & (uint) FlagM.CF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.CF);
            if ((grf & (uint) FlagM.VF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.VF);
        }

        public override int? GetOpcodeNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            //$TOD: write a dictionary mapping ARM instructions to ARM_INS_xxx.
            return new SortedList<string, int>();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
#if NATIVE
            return regsByName.TryGetValue(name, out reg);'
#else
            return Registers.RegistersByName.TryGetValue(name, out reg);
#endif
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var f))
            {
                return f;
            }

            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var flagregister =
#if NATIVE
                this.regsByName["cpsr"];
#else
                Registers.cpsr;
#endif
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            if ((grf & (uint)FlagM.VF) != 0) s.Append('V');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        public static PrimitiveType VectorElementDataType(ArmVectorData elemType)
        {
            switch (elemType)
            {
            case ArmVectorData.I8: return PrimitiveType.SByte;
            case ArmVectorData.S8: return PrimitiveType.SByte;
            case ArmVectorData.U8: return PrimitiveType.Byte;
            case ArmVectorData.F16: return PrimitiveType.Real16;
            case ArmVectorData.I16: return PrimitiveType.Int16;
            case ArmVectorData.S16: return PrimitiveType.Int16;
            case ArmVectorData.U16: return PrimitiveType.UInt16;
            case ArmVectorData.F32: return PrimitiveType.Real32;
            case ArmVectorData.I32: return PrimitiveType.Int32;
            case ArmVectorData.S32: return PrimitiveType.Int32;
            case ArmVectorData.U32: return PrimitiveType.UInt32;
            case ArmVectorData.F64: return PrimitiveType.Real64;
            case ArmVectorData.I64: return PrimitiveType.Int64;
            case ArmVectorData.S64: return PrimitiveType.Int64;
            case ArmVectorData.U64: return PrimitiveType.UInt64;
            default: throw new ArgumentException(nameof(elemType));
            }
        }



        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture(
            [MarshalAs(UnmanagedType.LPStr)] string archName);
    }
}
