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
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public abstract class PowerPcArchitecture : IProcessorArchitecture
    {
        private PrimitiveType wordWidth;
        private PrimitiveType ptrType;
        private ReadOnlyCollection<RegisterStorage> regs;
        private ReadOnlyCollection<RegisterStorage> fpregs;
        private ReadOnlyCollection<RegisterStorage> vregs;
        private ReadOnlyCollection<RegisterStorage> cregs;

        public RegisterStorage lr { get; private set; }
        public RegisterStorage cr { get; private set; }
        public RegisterStorage ctr { get; private set; }
        public RegisterStorage xer { get; private set; }
        public RegisterStorage fpscr { get; private set; }

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(PrimitiveType wordWidth)
        {
            this.wordWidth = wordWidth;
            this.ptrType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);

            this.lr = new RegisterStorage("lr", 0x68,   wordWidth);
            this.cr = new RegisterStorage("cr", 0x69,   wordWidth);
            this.ctr = new RegisterStorage("ctr", 0x6A, wordWidth);
            this.xer = new RegisterStorage("xer", 0x6B, wordWidth);
            this.fpscr = new RegisterStorage("fpscr", 0x6C, wordWidth);

            regs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("r" + n, n, wordWidth))
                .Concat(Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("f" + n, n + 0x20, PrimitiveType.Word64)))
                .Concat(Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("v" + n, n + 0x40, PrimitiveType.Word128)))
                .Concat(Enumerable.Range(0, 8)
                    .Select(n => new RegisterStorage("cr" + n, n + 0x60, PrimitiveType.Byte)))
                .Concat(new[] { lr, cr, ctr, xer })
                .ToList());

            fpregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x20).Take(0x20).ToList());

            vregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x40).Take(0x20).ToList());

            cregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x60).Take(0x8).ToList());

        }

        public uint CarryFlagMask { get { throw new NotImplementedException(); } }

        //$REVIEW: using R1 as the stack register is a _convention_. It 
        // should be platform-specific at the very least.
        public RegisterStorage StackRegister { get { return regs[1]; } }

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

        #region IProcessorArchitecture Members

        public PowerPcDisassembler CreateDisassembler(ImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        IEnumerable<MachineInstruction> IProcessorArchitecture.CreateDisassembler(ImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType); 
        }

        public ImageReader CreateImageReader(LoadedImage image, Address addr)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, addr);
        }

        public ImageReader CreateImageReader(LoadedImage image, ulong offset)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, offset);
        }

        public abstract IEnumerable<Address> CreatePointerScanner(
            ImageMap map, 
            ImageReader rdr,
            IEnumerable<Address> addrs, 
            PointerScannerFlags flags);

        public ProcedureBase GetTrampolineDestination(ImageReader rdr, IRewriterHost host)
        {
            var dasm = new PowerPcDisassembler(this, rdr, WordWidth);
            return GetTrampolineDestination(dasm, host);
        }

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

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            return new PowerPcProcedureSerializer(this, typeLoader, defaultCc);
        }

        public ProcessorState CreateProcessorState()
        {
            return new PowerPcState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet(0x80);
        }

        public RegisterStorage GetRegister(int i)
        {
            if (0 <= i && i < regs.Count)
                return regs[i];
            else
                return null;
        }

        public RegisterStorage GetRegister(string name)
        {
            return this.regs.Where(r => r.Name == name).SingleOrDefault();
        }

        public bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public int InstructionBitSize { get { return 32; } }

        public string GrfToString(uint grf)
        {
            //$BUG: this needs to be better conceved. There are 
            // 32 (!) condition codes in the PowerPC architecture
            return "crX";
        }

        public PrimitiveType FramePointerType
        {
            get { return ptrType; }
        }

        public PrimitiveType PointerType
        {
            get { return ptrType; }
        }

        public PrimitiveType WordWidth
        {
            get { return this.wordWidth; } 
        }

        public Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new PowerPcRewriter(this, rdr, frame, host);
        }

        public abstract Address MakeAddressFromConstant(Constant c);

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        #endregion
    }

    public class PowerPcArchitecture32 : PowerPcArchitecture
    {
        public PowerPcArchitecture32()
            : base(PrimitiveType.Word32)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            ImageMap map, 
            ImageReader rdr, 
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToUInt32())
                .ToHashSet();
            return new PowerPcPointerScanner32(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr32(u));
        }

        public override  Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }
    }

    public class PowerPcArchitecture64 : PowerPcArchitecture
    {
        public PowerPcArchitecture64()
            : base(PrimitiveType.Word64)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            ImageMap map,
            ImageReader rdr,
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
    }
}
