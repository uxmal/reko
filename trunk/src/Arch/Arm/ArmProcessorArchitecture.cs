using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Core.Machine;
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

        public Disassembler CreateDisassembler(ImageReader imageReader)
        {
            throw new NotImplementedException();
        }

        public Dumper CreateDumper()
        {
            throw new NotImplementedException();
        }

        public ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }


        public Rewriter CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            throw new NotImplementedException();
        }

        public MachineRegister GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public MachineRegister GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRegister(string name, out MachineRegister reg)
        {
            throw new NotImplementedException();
        }

        public MachineFlags GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public MachineFlags GetFlagGroup(string name)
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

        public MachineRegister StackRegister
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
        public static readonly MachineRegister r0 = new MachineRegister("r0", 0, PrimitiveType.Word32);
        public static readonly MachineRegister r1 = new MachineRegister("r1", 1, PrimitiveType.Word32);
        public static readonly MachineRegister r2 = new MachineRegister("r2", 2, PrimitiveType.Word32);
        public static readonly MachineRegister r3 = new MachineRegister("r3", 3, PrimitiveType.Word32);

        public static readonly MachineRegister r4 = new MachineRegister("r4", 4, PrimitiveType.Word32);
        public static readonly MachineRegister r5 = new MachineRegister("r5", 5, PrimitiveType.Word32);
        public static readonly MachineRegister r6 = new MachineRegister("r6", 6, PrimitiveType.Word32);
        public static readonly MachineRegister r7 = new MachineRegister("r7", 7, PrimitiveType.Word32);

        public static readonly MachineRegister r8 = new MachineRegister("r8", 8, PrimitiveType.Word32);
        public static readonly MachineRegister r9 = new MachineRegister("r9", 9, PrimitiveType.Word32);
        public static readonly MachineRegister r10 = new MachineRegister("r10", 10, PrimitiveType.Word32);
        public static readonly MachineRegister r11 = new MachineRegister("r11", 11, PrimitiveType.Word32);

        public static readonly MachineRegister r12 = new MachineRegister("r12", 12, PrimitiveType.Word32);
        public static readonly MachineRegister r13 = new MachineRegister("r13", 13, PrimitiveType.Word32);
        public static readonly MachineRegister r14 = new MachineRegister("r14", 14, PrimitiveType.Word32);
        public static readonly MachineRegister r15 = new MachineRegister("r15", 15, PrimitiveType.Word32);

        public static readonly MachineRegister[] Registers;

        static ArmRegisters()
        {
            Registers = new MachineRegister[] { r0, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r14, r15 };
        }
    }
}