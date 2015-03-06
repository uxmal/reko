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

using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void RewriteB()
        {
            var dst = RewriteOperand(instr.op1);
            emitter.Goto(dst);
        }

        private void RewriteBcctr(bool linkRegister)
        {
            var bo = ((Constant)RewriteOperand(instr.op1)).ToByte();
            var ctr = frame.EnsureRegister(Registers.ctr);

            switch (bo)
            {
            case 0x00: 
            case 0x01: throw new NotImplementedException("dec ctr");
            case 0x02: 
            case 0x03: throw new NotImplementedException("dec ctr");
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07: throw new NotImplementedException("condition false");
            case 0x08:
            case 0x09: throw new NotImplementedException("dec ctr; condition false");
            case 0x0A:
            case 0x0B: throw new NotImplementedException("dec ctr; condition false");
            case 0x0C:
            case 0x0D:
            case 0x0E:
            case 0x0F: throw new NotImplementedException("condition true");
            case 0x10:
            case 0x11:
            case 0x18:
            case 0x19: throw new NotImplementedException("condition true");
            case 0x12:
            case 0x13:
            case 0x1A:
            case 0x1B: throw new NotImplementedException("condition true");
            default:
                if (linkRegister)
                    emitter.Call(ctr, 0);
                else
                    emitter.Goto(ctr);
                return;
            }
        }


        private void RewriteBl()
        {
            var dst = RewriteOperand(instr.op1);
            emitter.Call(dst, 0);
        }
    }
}
