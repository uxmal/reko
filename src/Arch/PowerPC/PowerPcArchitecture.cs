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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public abstract class PowerPcArchitecture : ProcessorArchitecture
    {
        private ReadOnlyCollection<RegisterStorage> regs;
        private ReadOnlyCollection<RegisterStorage> fpregs;
        private ReadOnlyCollection<RegisterStorage> vregs;
        private ReadOnlyCollection<RegisterStorage> cregs;
        private Dictionary<int, RegisterStorage> spregs;

        public RegisterStorage lr { get; private set; }
        public RegisterStorage ctr { get; private set; }
        public RegisterStorage xer { get; private set; }
        public RegisterStorage fpscr { get; private set; }

        public FlagRegister cr { get; private set; }

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(PrimitiveType wordWidth)
        {
            WordWidth = wordWidth;
            PointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);
            FramePointerType = PointerType;
            InstructionBitSize = 32;

            this.lr = new RegisterStorage("lr", 0x68, 0, wordWidth);
            this.ctr = new RegisterStorage("ctr", 0x6A, 0, wordWidth);
            this.xer = new RegisterStorage("xer", 0x6B, 0, wordWidth);
            this.fpscr = new RegisterStorage("fpscr", 0x6C, 0, wordWidth);

            this.cr = new FlagRegister("cr", 0x80, wordWidth);

            regs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("r" + n, n, 0, wordWidth))
                .Concat(Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("f" + n, n + 0x20, 0, PrimitiveType.Word64)))
                .Concat(Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("v" + n, n + 0x40, 0, PrimitiveType.Word128)))
                .Concat(Enumerable.Range(0, 8)
                    .Select(n => new RegisterStorage("cr" + n, n + 0x60, 0, PrimitiveType.Byte)))
                .Concat(new[] { lr, ctr, xer })
                .ToList());

            fpregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x20).Take(0x20).ToList());

            vregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x40).Take(0x20).ToList());

            cregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x60).Take(0x8).ToList());

            spregs = new Dictionary<int, RegisterStorage>
            {
                { 26, new RegisterStorage("srr0", 0x0100 + 26, 0, PrimitiveType.Word32) },
                { 27, new RegisterStorage("srr1", 0x0100 + 27, 0, PrimitiveType.Word32) },
            };

            //$REVIEW: using R1 as the stack register is a _convention_. It 
            // should be platform-specific at the very least.
            StackRegister = regs[1];
        }

        public ReadOnlyCollection<RegisterStorage> Registers
        {
            get { return regs; }
        }

        public ReadOnlyCollection<RegisterStorage> FpRegisters
        {
            get { return fpregs; }
        }

        public ReadOnlyCollection<RegisterStorage> VecRegisters
        {
            get { return vregs; }
        }
        public ReadOnlyCollection<RegisterStorage> CrRegisters
        {
            get { return cregs; }
        }
        public Dictionary<int, RegisterStorage> SpRegisters
        {
            get { return spregs; }
        }

        #region IProcessorArchitecture Members

        public PowerPcDisassembler CreateDisassemblerImpl(EndianImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override abstract IEnumerable<Address> CreatePointerScanner(
            SegmentMap map, 
            EndianImageReader rdr,
            IEnumerable<Address> addrs, 
            PointerScannerFlags flags);

        //public override ProcedureBase GetTrampolineDestination(EndianImageReader rdr, IRewriterHost host)
        //{
        //    var dasm = new PowerPcDisassembler(this, rdr, WordWidth);
        //    return GetTrampolineDestination(dasm, host);
        //}

        /// <summary>
        /// Detects the presence of a PowerPC trampoline and returns the imported function 
        /// that is actually being requested.
        /// </summary>
        /// <remarks>
        /// A PowerPC trampoline looks like this:
        ///     addis  rX,r0,XXXX (or oris rx,r0,XXXX)
        ///     lwz    rY,YYYY(rX)
        ///     mtctr  rY
        ///     bctr   rY
        /// When loading the ELF binary, we discovered the memory locations
        /// that will contain pointers to imported functions. If the address
        /// XXXXYYYY matches one of those memory locations, we have found a
        /// trampoline.
        /// </remarks>
        /// <param name="rdr"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public ProcedureBase GetTrampolineDestination(IEnumerable<PowerPcInstruction> rdr, IRewriterHost host)
        {
            var e = rdr.GetEnumerator();

            if (!e.MoveNext() || (e.Current.Opcode != Opcode.addis && e.Current.Opcode != Opcode.oris))
                return null;
            var addrInstr = e.Current.Address;
            var reg = ((RegisterOperand)e.Current.op1).Register;
            var uAddr = ((ImmediateOperand)e.Current.op3).Value.ToUInt32() << 16;

            if (!e.MoveNext() || e.Current.Opcode != Opcode.lwz)
                return null;
            var mem = e.Current.op2 as MemoryOperand;
            if (mem == null)
                return null;
            if (mem.BaseRegister != reg)
                return null;
            uAddr = (uint)((int)uAddr + mem.Offset.ToInt32());
            reg = ((RegisterOperand)e.Current.op1).Register;

            if (!e.MoveNext() || e.Current.Opcode != Opcode.mtctr)
                return null;
            if (((RegisterOperand)e.Current.op1).Register != reg)
                return null;

            if (!e.MoveNext() || e.Current.Opcode != Opcode.bcctr)
                return null;

            // We saw a thunk! now try to resolve it.

            var addr = Address.Ptr32(uAddr);
            var ep = host.GetImportedProcedure(addr, addrInstr);
            if (ep != null)
                return ep;
            return host.GetInterceptedCall(addr);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new PowerPcState(this);
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(v => Enum.GetName(typeof(Opcode), v), v => (int)v);
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
            if (0 <= i && i < regs.Count)
                return regs[i];
            else
                return null;
        }

        public override RegisterStorage GetRegister(string name)
        {
            return this.regs.Where(r => r.Name == name).SingleOrDefault();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs.ToArray();
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

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0)
                return reg;
            else
                return null;
        }

        public override string GrfToString(uint grf)
        {
            //$BUG: this needs to be better conceved. There are 
            // 32 (!) condition codes in the PowerPC architecture
            return "crX";
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            return new PowerPcRewriter(this, rdr, frame, host);
        }

        public override abstract Address MakeAddressFromConstant(Constant c);

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        #endregion
    }

    public class PowerPcBe32Architecture : PowerPcArchitecture
    {
        public PowerPcBe32Architecture()
            : base(PrimitiveType.Word32)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            SegmentMap map, 
            EndianImageReader rdr, 
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToUInt32())
                .ToHashSet();
            return new PowerPcPointerScanner32(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr32(u));
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new BeImageWriter(mem, addr);
        }
    }

    public class PowerPcLe32Architecture : PowerPcArchitecture
    {
        public PowerPcLe32Architecture() : base(PrimitiveType.Word32)
        {

        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addrs, PointerScannerFlags flags)
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

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }

    public class PowerPcBe64Architecture : PowerPcArchitecture
    {
        public PowerPcBe64Architecture()
            : base(PrimitiveType.Word64)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            SegmentMap map,
            EndianImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToLinear())
                .ToHashSet();
            return new PowerPcPointerScanner64(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr64(u));
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr64(c.ToUInt64());
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, offset);
        }

        public override ImageWriter CreateImageWriter()
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageWriter(mem, addr);
        }
    }
}
