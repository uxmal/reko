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
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.C64
{
    public class C64Basic : IProcessorArchitecture
    {
        private SortedList<ushort, C64BasicInstruction> prog;
        private RegisterStorage stackRegister = new RegisterStorage("sp", 1, PrimitiveType.Ptr16);

        public C64Basic(SortedList<ushort, C64BasicInstruction> prog)
        {
            this.prog = prog;
        }

        public IEnumerable<MachineInstruction> CreateDisassembler(ImageReader imageReader)
        {
            int i = prog.IndexOfKey((ushort)imageReader.Address.Linear);
            if (i < 0)
                yield break;
            for (; i < prog.Count; ++i)
            {
                yield return prog.Values[i];
            }
        }

        public ProcessorState CreateProcessorState()
        {
            return new C64BasicState(this);
        }

        public Core.Lib.BitSet CreateRegisterBitset()
        {
            return new Core.Lib.BitSet(0x10);
        }

        public IEnumerable<RtlInstructionCluster> CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            return new C64BasicRewriter(this, rdr.Address, prog, host);
        }

        public IEnumerable<Address> CreatePointerScanner(ImageMap map, ImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public Frame CreateFrame()
        {
            return new Frame(PrimitiveType.Ptr16);
        }

        public ImageReader CreateImageReader(LoadedImage img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public ImageReader CreateImageReader(LoadedImage img, uint off)
        {
            throw new NotImplementedException();
        }

        public ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultCc)
        {
            throw new NotSupportedException();  //$Makes no sense in C64 Basic!
        }

        public ProcedureBase GetTrampolineDestination(ImageReader rdr, IRewriterHost host)
        {
            //$NYI
            return null;
        }


        public RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
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

        public Core.Expressions.Expression CreateStackAccess(Frame frame, int cbOffset, Core.Types.DataType dataType)
        {
            throw new NotImplementedException();
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public Core.Lib.BitSet ImplicitArgumentRegisters
        {
            get { throw new NotImplementedException(); }
        }

        public string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public Core.Types.PrimitiveType FramePointerType
        {
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Ptr16; }
        }

        public Core.Types.PrimitiveType WordWidth
        {
            get { throw new NotImplementedException(); }
        }

        public int InstructionBitSize
        {
            get { return 8; }
        }

        public RegisterStorage StackRegister
        {
            get { return stackRegister; }
        }

        public uint CarryFlagMask
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }


        public class C64BasicState : ProcessorState
        {
            private C64Basic arch;

            public C64BasicState(C64Basic arch)
            {
                this.arch = arch;
            }

            public override IProcessorArchitecture Architecture { get { return arch; } }

            public override ProcessorState Clone()
            {
                return new C64BasicState(arch);
            }

            public override Core.Expressions.Constant GetRegister(RegisterStorage r)
            {
                throw new NotImplementedException();
            }

            public override void SetRegister(RegisterStorage r, Core.Expressions.Constant v)
            {
            }

            public override void SetInstructionPointer(Address addr)
            {
            }

            public override void OnProcedureEntered()
            {
            }

            public override void OnProcedureLeft(ProcedureSignature procedureSignature)
            {
            }

            public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
            {
                return new CallSite(2, 0);
            }

            public override void OnAfterCall(Core.Expressions.Identifier stackReg, ProcedureSignature sigCallee, Core.Expressions.ExpressionVisitor<Core.Expressions.Expression> eval)
            {
            }
        }
    }
}
