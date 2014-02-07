#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Types;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Arm
{
    public class ArmProcessorArchitecture : IProcessorArchitecture
    {
        public ArmProcessorArchitecture()
        {
        }

        #region IProcessorArchitecture Members

        public IEnumerator<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            return new ArmDisassembler2(this, imageReader);
        }

        public ProcessorState CreateProcessorState()
        {
            return new ArmProcessorState(this);
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new ArmRewriter(this, rdr, (ArmProcessorState)state, frame);
        }

        public IEnumerable<uint> CreateCallInstructionScanner(ImageReader rdr, HashSet<uint> knownLinAddresses)
        {
            while (rdr.IsValid)
            {
                uint linAddrCall = rdr.Address.Linear;
                var opcode = rdr.ReadLeUInt32();
                if ((opcode & 0x0F000000) == 0x0B000000)         // BL
                {
                    int offset = ((int) opcode << 8) >> 6;
                    uint target = (uint) (linAddrCall + 8 + offset);
                    if (knownLinAddresses.Contains(target))
                        yield return linAddrCall;
                }
            }
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType);
        }

        public RegisterStorage GetRegister(int i)
        {
            return A32Registers.GpRegs[i];
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

        public Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public int InstructionBitSize { get { return 32; } }

        public BitSet ImplicitArgumentRegisters
        {
            get { throw new NotImplementedException(); }
        }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public PrimitiveType FramePointerType
        {
            get { return StackRegister.DataType; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Word32; }
        }

        public PrimitiveType WordWidth
        {
            get { return PrimitiveType.Word32; }
        }

        public RegisterStorage StackRegister
        {
            get { return ArmRegisters.r13; }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public static class ArmRegisters
    {
        public static readonly RegisterStorage r0 = new RegisterStorage("r0", 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r1 = new RegisterStorage("r1", 1, PrimitiveType.Word32);
        public static readonly RegisterStorage r2 = new RegisterStorage("r2", 2, PrimitiveType.Word32);
        public static readonly RegisterStorage r3 = new RegisterStorage("r3", 3, PrimitiveType.Word32);

        public static readonly RegisterStorage r4 = new RegisterStorage("r4", 4, PrimitiveType.Word32);
        public static readonly RegisterStorage r5 = new RegisterStorage("r5", 5, PrimitiveType.Word32);
        public static readonly RegisterStorage r6 = new RegisterStorage("r6", 6, PrimitiveType.Word32);
        public static readonly RegisterStorage r7 = new RegisterStorage("r7", 7, PrimitiveType.Word32);

        public static readonly RegisterStorage r8 = new RegisterStorage("r8", 8, PrimitiveType.Word32);
        public static readonly RegisterStorage r9 = new RegisterStorage("r9", 9, PrimitiveType.Word32);
        public static readonly RegisterStorage r10 = new RegisterStorage("r10", 10, PrimitiveType.Word32);
        public static readonly RegisterStorage r11 = new RegisterStorage("r11", 11, PrimitiveType.Word32);

        public static readonly RegisterStorage r12 = new RegisterStorage("r12", 12, PrimitiveType.Word32);
        public static readonly RegisterStorage r13 = new RegisterStorage("r13", 13, PrimitiveType.Word32);
        public static readonly RegisterStorage r14 = new RegisterStorage("r14", 14, PrimitiveType.Word32);
        public static readonly RegisterStorage r15 = new RegisterStorage("r15", 15, PrimitiveType.Word32);

        public static readonly RegisterStorage[] Registers;

        static ArmRegisters()
        {
            Registers = new RegisterStorage[] { r0, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r14, r15 };
        }
    }
}