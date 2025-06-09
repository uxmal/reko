#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Arm.AArch64;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Registers64 = Reko.Arch.Arm.AArch64.Registers;

namespace Reko.Arch.Arm
{
    public class Arm64Architecture : ProcessorArchitecture
    {
#if NATIVE
        private INativeArchitecture native;
        private Dictionary<string, RegisterStorage> regsByName;
        private RegisterStorage[] regsByNumber;
#endif

        public Arm64Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers64.ByName, Registers.ByDomain)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 32;
            this.FramePointerType = PrimitiveType.Ptr64;
            this.PointerType = PrimitiveType.Ptr64;
            this.WordWidth = PrimitiveType.Word64;
            this.CarryFlag = null;
#if NATIVE
            var unk = CreateNativeArchitecture("arm-64");
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);
            GetRegistersFromNative();
#endif
            StackRegister = Registers.sp;
        }

        public override int ReturnAddressOnStack => 0;

#if NATIVE
        private void GetRegistersFromNative()
        {
            int cRegs;
            IntPtr aRegs;
            native.GetAllRegisters(0, out cRegs, out aRegs);
            if (aRegs is null)
                throw new OutOfMemoryException();
            this.regsByName = new Dictionary<string, RegisterStorage>();
            var regsByNumber = new List<RegisterStorage> { null };
            NativeRegister nReg = new NativeRegister();
            int cb = Marshal.SizeOf(nReg);
            while (cRegs > 0)
            {
                nReg = (NativeRegister)Marshal.PtrToStructure(aRegs, typeof(NativeRegister));
                if (nReg.Name is not null)
                {
                    var n = nReg.Name;
                    var i = nReg.Number;
                    var b = nReg.BitSize;
                    var reg = new RegisterStorage(n, i, 0, PrimitiveType.CreateWord(b));
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
            return new AArch64Disassembler(this, rdr);
            /*
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
                    if (nInstr is null)
                        yield break;
                    else
                        yield return new Arm64Instruction(nInstr);
                }
            }
            finally
            {
                if (ndasm is not null)
                {
                    ndasm = null;
                }
                if (hBytes is not null && hBytes.IsAllocated)
                {
                    hBytes.Free();
                }
            }*/
        }
#endif
        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new AArch64Disassembler(this, rdr);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownLinAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            return null;
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Arm64State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new A64Rewriter(this, rdr, state, binder, host);
        }

        public override Core.Machine.ICallingConvention? GetCallingConvention(string? name)
        {
            return new AArch64.AArch64CallingConvention(this);
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return new SortedList<string, int>();
        }

        public override int? GetMnemonicNumber(string name)
        {
            return null;
        }

        public override RegisterStorage? GetRegister(StorageDomain dom, BitRange range)
        {
            var i = (int) dom;
            if (0 <= i && i < Registers.SubRegisters.Length)
            {
                var subregs = Registers.SubRegisters[i];
                var reg = subregs[0];
                for (int j = 0; j < subregs.Length; ++j)
                {
                    var subreg = subregs[j];
                    if ((short) subreg.BitSize < range.Msb)
                        break;
                    reg = subreg;
                }
                return reg;
            }
            return null;
        }


        public override RegisterStorage[] GetRegisters()
        {
#if NATIVE
            return regsByNumber.ToArray();
#else
            return Registers.GpRegs64;
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

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            if ((grf & (uint)FlagM.VF) != 0) s.Append('V');
            return s.ToString();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var flagregister = Registers.pstate;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf));
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt64();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr64(uAddr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }

        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture(
           [MarshalAs(UnmanagedType.LPStr)] string archName);
    }
}

