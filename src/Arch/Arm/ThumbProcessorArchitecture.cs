#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Arm
{
    public class ThumbArchitecture : ProcessorArchitecture
    {
#if NATIVE
        private INativeArchitecture native;
#endif
        private readonly Dictionary<ulong, FlagGroupStorage> flagGroups;

        public ThumbArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.All)
        {
            this.Endianness = EndianServices.Little;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.InstructionBitSize = 16;

            this.flagGroups = [];
#if NATIVE
            var unk = CreateNativeArchitecture("arm-thumb");
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);
            GetRegistersFromNative();
#endif
            StackRegister = Registers.All.GetRegister("sp");
        }

#if NATIVE
        private void GetRegistersFromNative()
        {
            int cRegs;
            IntPtr aRegs;
            native.GetAllRegisters(0, out cRegs, out aRegs);
            if (aRegs is null)
                throw new OutOfMemoryException();
            this.regsByName = new Dictionary<string, RegisterStorage>();
            this.regsByNumber = new Dictionary<int, RegisterStorage>();
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
                    regsByNumber.Add(reg.Number, reg);
        }
                aRegs += cb;
                --cRegs;
            }
        }
#endif

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new T32Disassembler(this, rdr);
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
                        yield return new Arm32Instruction(nInstr);
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

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new ThumbRewriter(this, rdr, host, binder);
            //return new ThumbRewriterRetired(regsByNumber, this.native, rdr, (ArmProcessorState)state, binder, host);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Arm32InstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new AArch32ProcessorState(this);
        }
 

        public override SortedList<string, int> GetMnemonicNames()
        {
            return new SortedList<string, int>();
        }

        public override int? GetMnemonicNumber(string name)
        {
                return null;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, ulong grf)
        {
            if (flagGroups.TryGetValue(grf, out FlagGroupStorage? f))
            {
                return f;
            }

            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf));
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            ulong grf = flags.FlagGroupBits;
            if ((grf & (ulong) FlagM.NF) != 0) yield return GetFlagGroup(flags.FlagRegister, (ulong) FlagM.NF);
            if ((grf & (ulong) FlagM.ZF) != 0) yield return GetFlagGroup(flags.FlagRegister, (ulong) FlagM.ZF);
            if ((grf & (ulong) FlagM.CF) != 0) yield return GetFlagGroup(flags.FlagRegister, (ulong) FlagM.CF);
            if ((grf & (ulong) FlagM.VF) != 0) yield return GetFlagGroup(flags.FlagRegister, (ulong) FlagM.VF);
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, ulong grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (ulong) FlagM.NF) != 0) s.Append('N');
            if ((grf & (ulong) FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (ulong) FlagM.CF) != 0) s.Append('C');
            if ((grf & (ulong) FlagM.VF) != 0) s.Append('V');
            return s.ToString();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture(
            [MarshalAs(UnmanagedType.LPStr)] string archName);
    }
}
