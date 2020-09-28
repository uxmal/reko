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
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.M6800.M6809
{
    public class MultipleRegisterOperand : MachineOperand
    {
        private readonly RegisterStorage[] regs;
        private readonly byte b;

        public MultipleRegisterOperand(RegisterStorage[] regs, byte b) : base(PrimitiveType.Byte)
        {
            this.regs = regs;
            this.b = b;
        }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            uint m = 0x80;
            int iReg = 0;
            while (m != 0)
            {
                if ((b & m) != 0)
                {
                    yield return regs[iReg];
                }
                m >>= 1;
                ++iReg;
            }
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string sep = "";
            foreach (var reg in GetRegisters())
            {
                writer.WriteString(sep);
                sep = ",";
                writer.WriteString(reg.Name);
            }
        }
    }
}