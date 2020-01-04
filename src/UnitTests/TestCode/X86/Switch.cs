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

namespace Reko.UnitTests.TestCode
{
    public partial class X86 
    {
        public static void Switch(X86Assembler m)
        {
            m.Proc("foo");
            m.Push(m.cs);
            m.Pop(m.ds);
            m.Mov(m.bl, m.MemB(Registers.si, 0));
            m.Cmp(m.bl, 0x02);
            m.Ja("default");

            m.Label("test");
            m.Xor(m.bh, m.bh);
            m.Add(m.bx, m.bx);
            m.Jmp(m.MemW(Registers.bx, "jmptable"));

            m.Label("jmptable");
            m.Dw("one");
            m.Dw("two");
            m.Dw("three");

            m.Label("one");
            m.Mov(m.ax, 1);
            m.Ret();

            m.Label("two");
            m.Mov(m.ax, 2);
            m.Ret();

            m.Label("three");
            m.Mov(m.ax, 3);
            m.Ret();

            m.Label("default");
            m.Mov(m.ax, 0);
            m.Ret();
        }

        public static void Switch32(X86Assembler m)
        {
            m.Proc("foo");
            m.Mov(m.eax, m.MemDw(Registers.esp, 4));
            m.Cmp(m.eax, 3);
            m.Ja("default");

            m.Xor(m.edx, m.edx);
            m.Mov(m.dl, m.MemB(Registers.eax, "bytes"));
            m.Jmp(m.MemDw(Registers.edx, 4, "jumps"));

            m.Label("bytes").Db(1, 0, 1, 2);
            m.Label("jumps").Dd("jump0", "jump1", "jump2");

            m.Label("jump0");
            m.Mov(m.eax, 0);
            m.Jmp("done");

            m.Label("jump1");
            m.Mov(m.eax, 1);
            m.Jmp("done");

            m.Label("jump2");
            m.Mov(m.eax, 2);
            m.Jmp("done");

            m.Label("default");
            m.Mov(m.eax, -1);

            m.Label("done");
            m.Mov(m.MemDw("dummy"), m.eax);
            m.Ret();

            m.Label("dummy").Dd(0);

        }
    }
}
