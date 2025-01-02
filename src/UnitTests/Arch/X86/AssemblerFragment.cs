#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.X86.Assembler;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.X86
{
    public abstract class AssemblerFragment
    {
        protected ParsedOperand eax = new ParsedOperand(Registers.eax);
        protected ParsedOperand ecx = new ParsedOperand(Registers.ecx);
        protected ParsedOperand edx = new ParsedOperand(Registers.edx);
        protected ParsedOperand ebx = new ParsedOperand(Registers.ebx);
        protected ParsedOperand ebp = new ParsedOperand(Registers.ebp);
        protected ParsedOperand esp = new ParsedOperand(Registers.esp);
        protected ParsedOperand esi = new ParsedOperand(Registers.esi);
        protected ParsedOperand edi = new ParsedOperand(Registers.edi);
        protected ParsedOperand ax = new ParsedOperand(Registers.ax);
        protected ParsedOperand cx = new ParsedOperand(Registers.cx);
        protected ParsedOperand dx = new ParsedOperand(Registers.dx);
        protected ParsedOperand bx = new ParsedOperand(Registers.bx);
        protected ParsedOperand bp = new ParsedOperand(Registers.bp);
        protected ParsedOperand sp = new ParsedOperand(Registers.sp);
        protected ParsedOperand si = new ParsedOperand(Registers.si);
        protected ParsedOperand di = new ParsedOperand(Registers.di);

        public abstract void Build(X86Assembler m);
    }
}
