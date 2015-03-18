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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcArchitecture : IProcessorArchitecture
    {
        private PrimitiveType wordWidth;
        private PrimitiveType ptrType;
        private ReadOnlyCollection<RegisterStorage> regs;
        private ReadOnlyCollection<RegisterStorage> fpregs;
        private ReadOnlyCollection<RegisterStorage> cregs;

        public RegisterStorage lr { get; private set; }
        public RegisterStorage cr { get; private set; }
        public RegisterStorage ctr { get; private set; }
        public RegisterStorage xer { get; private set; }

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(PrimitiveType wordWidth)
        {
            this.wordWidth = wordWidth;
            this.ptrType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);

            this.lr = new RegisterStorage("lr", 0x48,   wordWidth);
            this.cr = new RegisterStorage("cr", 0x49,   wordWidth);
            this.ctr = new RegisterStorage("ctr", 0x4A, wordWidth);
            this.xer = new RegisterStorage("xer", 0x4B, wordWidth);

            regs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("r" + n, n, wordWidth))
                .Concat(Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("f" + n, n + 0x20, PrimitiveType.Word64)))
                .Concat(Enumerable.Range(0, 8)
                    .Select(n => new RegisterStorage("cr" + n, n + 0x40, PrimitiveType.Byte)))
                .Concat(new[] { lr, cr, ctr, xer })
                .ToList());

            fpregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x20).Take(0x20).ToList());

            cregs = new ReadOnlyCollection<RegisterStorage>(
                regs.Skip(0x40).Take(0x8).ToList());
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

        public IEnumerable<uint> CreatePointerScanner(ImageReader rdr, HashSet<uint> knownLinAddresses, PointerScannerFlags flags)
        {
            return new PowerPcPointerScanner(rdr, knownLinAddresses, flags);
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

        public ImageReader CreateImageReader(LoadedImage image, uint offset)
        {
            //$TODO: PowerPC is bi-endian.
            return new BeImageReader(image, offset);
        }

        public ProcessorState CreateProcessorState()
        {
            return new PowerPcState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet(0x50);
        }

        public RegisterStorage GetRegister(int i)
        {
            return regs[i];
        }

        public RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
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

        public BitSet ImplicitArgumentRegisters
        {
            get {
                return CreateRegisterBitset();
            }
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

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public uint GetAddressOffset(Address addr)
        {
            return addr.Linear;
        }

        #endregion
    }

    public class PowerPcArchitecture32 : PowerPcArchitecture
    {
        public PowerPcArchitecture32()
            : base(PrimitiveType.Word32)
        { }
    }

    public class PowerPcArchitecture64 : PowerPcArchitecture
    {
        public PowerPcArchitecture64()
            : base(PrimitiveType.Word64)
        { }
    }
}
