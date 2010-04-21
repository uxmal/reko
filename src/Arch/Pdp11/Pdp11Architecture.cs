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
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Registers
    {
        public static MachineRegister r0 = new MachineRegister("r0", 0, PrimitiveType.Word16);
        public static MachineRegister r1 = new MachineRegister("r1", 1, PrimitiveType.Word16);
        public static MachineRegister r2 = new MachineRegister("r2", 2, PrimitiveType.Word16);
        public static MachineRegister r3 = new MachineRegister("r3", 3, PrimitiveType.Word16);
        public static MachineRegister r4 = new MachineRegister("r4", 4, PrimitiveType.Word16);
        public static MachineRegister r5 = new MachineRegister("r5", 5, PrimitiveType.Word16);
        public static MachineRegister r6 = new MachineRegister("r6", 6, PrimitiveType.Word16);
        public static MachineRegister sp = new MachineRegister("sp", 7, PrimitiveType.Word16);
    }

    public class Pdp11Architecture : IProcessorArchitecture
    {
        private MachineRegister[] regs;

        public Pdp11Architecture()
        {
            regs = new MachineRegister[] { 
                Registers.r0, Registers.r1, Registers.r2, Registers.r3, 
                Registers.r4, Registers.r5, Registers.r6, Registers.sp, };
        }
        #region IProcessorArchitecture Members

        public Disassembler CreateDisassembler(ImageReader rdr)
        {
            return new Pdp11Disassembler(rdr);
        }

        public Dumper CreateDumper()
        {
            return new Pdp11Dumper();
        }

        public Frame CreateFrame()
        {
            return new Frame(PrimitiveType.Word16);
        }

        public ProcessorState CreateProcessorState()
        {
            return new Pdp11ProcessorState();
        }

        public BackWalker CreateBackWalker(ProgramImage img)
        {
            return new Pdp11BackWalker(img);
        }

        public CodeWalker CreateCodeWalker(ProgramImage img, Platform platform, Address addr, ProcessorState st)
        {
            return new Pdp11CodeWalker();
        }

        public BitSet CreateRegisterBitset()
        {
            return new BitSet(16);
        }

        public Rewriter CreateRewriter(IProcedureRewriter prw, Procedure proc, IRewriterHost host)
        {
            return new Pdp11Rewriter(this, prw);
        }

        public MachineRegister GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public MachineRegister GetRegister(string name)
        {
            foreach (MachineRegister reg in regs)
            {
                if (string.Compare(reg.Name, name, StringComparison.InvariantCultureIgnoreCase) != 0)
                    return reg;
            }
            return null;
        }

        public MachineFlags GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public MachineFlags GetFlagGroup(string name)
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
            get { return PrimitiveType.Word16; }
        }

        public PrimitiveType PointerType
        {
            get { return PrimitiveType.Word16; }
        }

        public PrimitiveType WordWidth
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
