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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Mips
{
    /// <summary>
    /// MIPS processor architecture
    /// </summary>
    /// <remarks>
    /// R4000 = MIPS III instruction set
    /// MIPS16 = compact 16-bit encoding
    /// </remarks>
    public abstract class MipsProcessorArchitecture : ProcessorArchitecture
    {
        public Dictionary<uint, RegisterStorage> fpuCtrlRegs;
        public RegisterStorage[] GeneralRegs;
        public RegisterStorage[] fpuRegs;
        public RegisterStorage[] ccRegs;
        public RegisterStorage LinkRegister;
        public RegisterStorage hi;
        public RegisterStorage lo;
        private Dictionary<string, RegisterStorage> mpNameToReg;

        public MipsProcessorArchitecture(PrimitiveType wordSize, PrimitiveType ptrSize)
        {
            this.WordWidth = wordSize;
            this.PointerType = ptrSize;
            this.FramePointerType = ptrSize;
            this.InstructionBitSize = 32;
            this.GeneralRegs = CreateGeneralRegisters().ToArray();
            this.StackRegister = GeneralRegs[29];
            this.LinkRegister = GeneralRegs[31];

            this.hi = new RegisterStorage("hi", 32, 0, wordSize);
            this.lo = new RegisterStorage("lo", 33, 0, wordSize);
            this.fpuRegs = CreateFpuRegisters();
            this.FCSR = RegisterStorage.Reg32("FCSR", 0x201F);
            this.ccRegs = CreateCcRegs();
            this.fpuCtrlRegs = new Dictionary<uint, RegisterStorage>
            {
                { 0x1F, FCSR }
            };

            this.mpNameToReg = GeneralRegs
                .Concat(fpuRegs)
                .Concat(fpuCtrlRegs.Values)
                .Concat(ccRegs)
                .Concat(new[] { hi, lo })
                .ToDictionary(k => k.Name);

    }

    /// <summary>
    /// If the architecture name contains "v6" we are a MIPS v6, which
    /// has different instruction encodings.
    /// </summary>
    private bool IsVersion6OrLater
        {
            get { return this.Name.Contains("v6"); }
        }

        public RegisterStorage FCSR { get; private set; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new MipsDisassembler(this, imageReader, this.IsVersion6OrLater);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new MipsInstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new MipsProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new MipsRewriter(
                this,
                new MipsDisassembler(this, rdr, IsVersion6OrLater),
                binder,
                host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (uint)a.ToLinear()).ToHashSet();
            return new MipsPointerScanner32(rdr, knownLinAddresses, flags).Select(l => Address.Ptr32(l));
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            Opcode result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(int i)
        {
            if (i >= GeneralRegs.Length)
                return null;
            return GeneralRegs[i];
        }

        public override RegisterStorage GetRegister(string name)
        {
            return mpNameToReg[name];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return GeneralRegs;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return mpNameToReg.TryGetValue(name, out reg);
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int cbOffset, DataType dataType)
        {
            var esp = frame.EnsureRegister(this.StackRegister);
            return MemoryAccess.Create(esp, cbOffset, dataType);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(uint grf)
        {
            if (grf != 0)   // MIPS has no traditional status register.
                throw new NotSupportedException();
            return "";
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        private IEnumerable<RegisterStorage> CreateGeneralRegisters()
        {
            return from i in Enumerable.Range(0, 32)
                join name in new[] {
                    new { id = 29, n = "sp" },
                    new { id = 31, n = "ra" }
                } on i equals name.id into names
                from name in names.DefaultIfEmpty()
                select new RegisterStorage(
                    name != null 
                        ? name.n 
                        : string.Format("r{0}", i),
                    i,
                    0,
                    WordWidth);
        }

        private RegisterStorage[] CreateFpuRegisters()
        {
            return Enumerable.Range(0, 32)
                .Select(i => new RegisterStorage(
                    string.Format("f{0}", i),
                    i + 64,
                    0,
                    PrimitiveType.Word64))
                .ToArray();
        }

        private RegisterStorage[] CreateCcRegs()
        {
            return Enumerable.Range(0, 8)
                .Select(i => new RegisterStorage(
                    string.Format("cc{0}", i),
                    0x3000,
                    0,
                    PrimitiveType.Bool))
                .ToArray();
        }
    }

    public class MipsBe32Architecture : MipsProcessorArchitecture
    {
        public MipsBe32Architecture() : base(PrimitiveType.Word32, PrimitiveType.Ptr32) { }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new BeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }

    public class MipsLe32Architecture : MipsProcessorArchitecture
    {
        public MipsLe32Architecture() : base(PrimitiveType.Word32, PrimitiveType.Ptr32) { }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }

    public class MipsBe64Architecture : MipsProcessorArchitecture
    {
        public MipsBe64Architecture() : base(PrimitiveType.Word64, PrimitiveType.Ptr64)
        { }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new BeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr64(c.ToUInt64());
        }
    }

    public class MipsLe64Architecture : MipsProcessorArchitecture
    {
        public MipsLe64Architecture() : base(PrimitiveType.Word64, PrimitiveType.Ptr64)
        {
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr64(c.ToUInt64());
        }
    }
}