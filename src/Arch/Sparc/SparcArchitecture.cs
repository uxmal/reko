using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public class SparcArchitecture : IProcessorArchitecture
    {
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

        public CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Lib.BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
        }

        public RewriterOld CreateRewriterOld(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public Rewriter CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost2 host)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Machine.MachineRegister GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Machine.MachineRegister GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRegister(string name, out Decompiler.Core.Machine.MachineRegister reg)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Machine.MachineFlags GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Machine.MachineFlags GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Expressions.Expression CreateStackAccess(Frame frame, int cbOffset, Decompiler.Core.Types.DataType dataType)
        {
            throw new NotImplementedException();
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Lib.BitSet ImplicitArgumentRegisters
        {
            get { throw new NotImplementedException(); }
        }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public Decompiler.Core.Types.PrimitiveType FramePointerType
        {
            get { throw new NotImplementedException(); }
        }

        public Decompiler.Core.Types.PrimitiveType PointerType
        {
            get { throw new NotImplementedException(); }
        }

        public Decompiler.Core.Types.PrimitiveType WordWidth
        {
            get { throw new NotImplementedException(); }
        }

        public Decompiler.Core.Machine.MachineRegister StackRegister
        {
            get { throw new NotImplementedException(); }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
