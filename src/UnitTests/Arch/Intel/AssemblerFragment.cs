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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
    public abstract class AssemblerFragment
    {
        protected ParsedOperand eax = new ParsedOperand(new RegisterOperand(Registers.eax));
        protected ParsedOperand ecx = new ParsedOperand(new RegisterOperand(Registers.ecx));
        protected ParsedOperand edx = new ParsedOperand(new RegisterOperand(Registers.edx));
        protected ParsedOperand ebx = new ParsedOperand(new RegisterOperand(Registers.ebx));
        protected ParsedOperand ebp = new ParsedOperand(new RegisterOperand(Registers.ebp));
        protected ParsedOperand esp = new ParsedOperand(new RegisterOperand(Registers.esp));
        protected ParsedOperand esi = new ParsedOperand(new RegisterOperand(Registers.esi));
        protected ParsedOperand edi = new ParsedOperand(new RegisterOperand(Registers.edi));
        protected ParsedOperand ax = new ParsedOperand(new RegisterOperand(Registers.ax));
        protected ParsedOperand cx = new ParsedOperand(new RegisterOperand(Registers.cx));
        protected ParsedOperand dx = new ParsedOperand(new RegisterOperand(Registers.dx));
        protected ParsedOperand bx = new ParsedOperand(new RegisterOperand(Registers.bx));
        protected ParsedOperand bp = new ParsedOperand(new RegisterOperand(Registers.bp));
        protected ParsedOperand sp = new ParsedOperand(new RegisterOperand(Registers.sp));
        protected ParsedOperand si = new ParsedOperand(new RegisterOperand(Registers.si));
        protected ParsedOperand di = new ParsedOperand(new RegisterOperand(Registers.di));

        public abstract void Build(X86Assembler m);
    }
}
