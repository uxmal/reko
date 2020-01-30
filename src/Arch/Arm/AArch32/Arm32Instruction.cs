#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Reko.Arch.Arm.AArch32
{
    public class Arm32Instruction : MachineInstruction
    {
        private INativeInstruction nInstr;
        private NativeInstructionInfo info;

        public Arm32Instruction(INativeInstruction nInstr)
        {
            this.nInstr = nInstr;
            nInstr.GetInfo(out info);
            this.Address = Address.Ptr32((uint)info.LinearAddress);
            this.Length = (int) info.Length;
        }

        //$REVIEW: is this really needed? nInstr is a ComInstance object,
        // provided by the CLR, and probably has its own finalizer.
        ~Arm32Instruction()
        {
            Marshal.ReleaseComObject(nInstr);
            nInstr = null;
        }

        public override int MnemonicAsInteger
        {
            get { return info.Mnemonic; }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            nInstr.Render(writer, options);
        }
    }
}
