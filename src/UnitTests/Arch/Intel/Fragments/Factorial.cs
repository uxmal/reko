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

using Reko.Arch.X86;
using Reko.Assemblers.x86;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.Intel.Fragments
{
    public class Factorial : AssemblerFragment
    {
        public override void Build(X86Assembler m)
        {
            m.i86();
            // A straight-forward factorial function + a driver program to ensure the return value 
            // is USE'd.

            m.Mov(cx, 0x100);
            m.Push(cx);
            m.Call("factorial");
            m.Add(Registers.sp, 2);
            m.Mov(m.WordPtr(0x0100), ax);
            m.Ret();

            m.Proc("factorial");
            m.Push(bp);
            m.Mov(bp, sp);

            m.Mov(ax, m.WordPtr(bp, 4));
            m.Dec(ax);
            m.Jz("base_case");

            m.Push(ax);
            m.Call("factorial");
            m.Inc(sp);
            m.Inc(sp);
            m.Mov(dx, m.WordPtr(bp, 4));
            m.Imul(dx);
            m.Jmp("done");

            m.Label("base_case");
            m.Mov(ax, 1);

            m.Label("done");
            m.Pop(bp);
            m.Ret();
            m.Endp("factorial");
        }
    }
}
