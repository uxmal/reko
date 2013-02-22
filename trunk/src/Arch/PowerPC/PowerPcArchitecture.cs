#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcArchitecture : IProcessorArchitecture
    {
        private PrimitiveType wordWidth;
        private ReadOnlyCollection<RegisterStorage> regs;

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(PrimitiveType wordWidth)
        {
            if (wordWidth != PrimitiveType.Word32)
                throw new ArgumentException("Only 32-bit mode of the architecture is currently supported.");
            this.wordWidth = wordWidth;

            regs = new ReadOnlyCollection<RegisterStorage>(new RegisterStorage[] {
                new RegisterStorage("r0", 0, wordWidth),
                new RegisterStorage("r1", 1, wordWidth),
                new RegisterStorage("r2", 2, wordWidth),
                new RegisterStorage("r3", 3, wordWidth),
                new RegisterStorage("r4", 4, wordWidth),
                new RegisterStorage("r5", 5, wordWidth),
                new RegisterStorage("r6", 6, wordWidth),
                new RegisterStorage("r7", 7, wordWidth),
                new RegisterStorage("r8", 8, wordWidth),
                new RegisterStorage("r9", 9, wordWidth),

                new RegisterStorage("r10", 10, wordWidth),
                new RegisterStorage("r11", 11, wordWidth),
                new RegisterStorage("r12", 12, wordWidth),
                new RegisterStorage("r13", 13, wordWidth),
                new RegisterStorage("r14", 14, wordWidth),
                new RegisterStorage("r15", 15, wordWidth),
                new RegisterStorage("r16", 16, wordWidth),
                new RegisterStorage("r17", 17, wordWidth),
                new RegisterStorage("r18", 18, wordWidth),
                new RegisterStorage("r19", 19, wordWidth),

                new RegisterStorage("r20", 20, wordWidth),
                new RegisterStorage("r21", 21, wordWidth),
                new RegisterStorage("r22", 22, wordWidth),
                new RegisterStorage("r23", 23, wordWidth),
                new RegisterStorage("r24", 24, wordWidth),
                new RegisterStorage("r25", 25, wordWidth),
                new RegisterStorage("r26", 26, wordWidth),
                new RegisterStorage("r27", 27, wordWidth),
                new RegisterStorage("r28", 28, wordWidth),
                new RegisterStorage("r29", 29, wordWidth),

                new RegisterStorage("r30", 30, wordWidth),
                new RegisterStorage("r31", 31, wordWidth),
            });
        }

        public ReadOnlyCollection<RegisterStorage> Registers
        {
            get { return regs; }
        }

        #region IProcessorArchitecture Members

        public Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType); 
        }

        public ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public BitSet CreateRegisterBitset()
        {
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType PointerType
        {
            get { throw new NotImplementedException(); }
        }

        public PrimitiveType WordWidth
        {
            get { return this.wordWidth; } 
        }

        public uint CarryFlagMask { get { throw new NotImplementedException(); } }
        public RegisterStorage StackRegister { get { return regs[1]; } }



        public Expression CreateStackAccess(Frame frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public Rewriter CreateRewriter(ImageReader rdr, ProcessorState state, Frame frame, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public Address ReadCodeAddress(int size, ImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
