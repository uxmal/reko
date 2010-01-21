/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
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
        private ReadOnlyCollection<MachineRegister> regs;

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(PrimitiveType wordWidth)
        {
            if (wordWidth != PrimitiveType.Word32)
                throw new ArgumentException("Only 32-bit mode of the architecture is supported.");
            this.wordWidth = wordWidth;

            regs = new ReadOnlyCollection<MachineRegister>(new MachineRegister[] {
                new MachineRegister("r0", 0, wordWidth),
                new MachineRegister("r1", 1, wordWidth),
                new MachineRegister("r2", 2, wordWidth),
                new MachineRegister("r3", 3, wordWidth),
                new MachineRegister("r4", 4, wordWidth),
                new MachineRegister("r5", 5, wordWidth),
                new MachineRegister("r6", 6, wordWidth),
                new MachineRegister("r7", 7, wordWidth),
                new MachineRegister("r8", 8, wordWidth),
                new MachineRegister("r9", 9, wordWidth),

                new MachineRegister("r10", 10, wordWidth),
                new MachineRegister("r11", 11, wordWidth),
                new MachineRegister("r12", 12, wordWidth),
                new MachineRegister("r13", 13, wordWidth),
                new MachineRegister("r14", 14, wordWidth),
                new MachineRegister("r15", 15, wordWidth),
                new MachineRegister("r16", 16, wordWidth),
                new MachineRegister("r17", 17, wordWidth),
                new MachineRegister("r18", 18, wordWidth),
                new MachineRegister("r19", 19, wordWidth),

                new MachineRegister("r20", 20, wordWidth),
                new MachineRegister("r21", 21, wordWidth),
                new MachineRegister("r22", 22, wordWidth),
                new MachineRegister("r23", 23, wordWidth),
                new MachineRegister("r24", 24, wordWidth),
                new MachineRegister("r25", 25, wordWidth),
                new MachineRegister("r26", 26, wordWidth),
                new MachineRegister("r27", 27, wordWidth),
                new MachineRegister("r28", 28, wordWidth),
                new MachineRegister("r29", 29, wordWidth),

                new MachineRegister("r30", 30, wordWidth),
                new MachineRegister("r31", 31, wordWidth),
            });
        }

        public ReadOnlyCollection<MachineRegister> Registers
        {
            get { return regs; }
        }

        #region IProcessorArchitecture Members

        public Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new PowerPcDisassembler(this, rdr, WordWidth);
        }

        public Dumper CreateDumper()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Frame CreateFrame()
        {
            return new Frame(FramePointerType); 
        }

        public ProcessorState CreateProcessorState()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public BackWalker CreateBackWalker(ProgramImage img)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public BitSet CreateRegisterBitset()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public MachineRegister GetRegister(int i)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public MachineRegister GetRegister(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public MachineFlags GetFlagGroup(uint grf)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public MachineFlags GetFlagGroup(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Decompiler.Core.Lib.BitSet ImplicitArgumentRegisters
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string GrfToString(uint grf)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Decompiler.Core.Types.PrimitiveType FramePointerType
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public Decompiler.Core.Types.PrimitiveType PointerType
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public PrimitiveType WordWidth
        {
            get { return this.wordWidth; } 
        }

        #endregion
    }
}
