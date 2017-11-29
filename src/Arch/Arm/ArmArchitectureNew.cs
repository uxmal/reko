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

namespace Reko.Arch.Arm
{
    public class Arm32ArchitectureNew : ProcessorArchitecture
    {
        private INativeArchitecture native;
        private Dictionary<string, RegisterStorage> regsByName;
        private RegisterStorage[] regsByNumber;

        public Arm32ArchitectureNew()
        {
            var unk = CreateNativeArchitecture();
            this.native = (INativeArchitecture)Marshal.GetObjectForIUnknown(unk);
            Marshal.Release(unk);

            GetRegistersFromNative();
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
                    Debug.Print($"Name: {n}, number: {i}, bit size: {b}");
                    var reg = new RegisterStorage(n, i, 0, PrimitiveType.CreateWord(b/8));
                    regsByName.Add(reg.Name, reg);
                    regsByNumber.Add(reg);
                }
                aRegs += cb;
                --cRegs;
            }
            this.regsByNumber = regsByNumber.ToArray();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            throw new NotImplementedException();
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            throw new NotImplementedException();
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            throw new NotImplementedException();
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter()
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter(MemoryArea img, Address addr)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new ArmRewriterNew(this, rdr, (ArmProcessorState) state, binder, host);
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return regsByName.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            throw new NotImplementedException();
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            throw new NotImplementedException();
        }

        [DllImport("ArmNative", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "CreateNativeArchitecture")]
        public static extern IntPtr CreateNativeArchitecture();
    }
}
