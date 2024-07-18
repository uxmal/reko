#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using System;
using Reko.Core.Code;
using Reko.Core.Expressions;

namespace Reko.Core.IRFormat
{
    public class IRProcedureBuilder : ExpressionEmitter
    {
        private IProcessorArchitecture arch;
        private IRBlock? block;
        private Address addrCur;
        private string name;

        public IRProcedureBuilder(string name, IProcessorArchitecture arch, Address address)
        {
            this.name = name;
            this.arch = arch;
            this.addrCur = address;
        }

        public uint InstructionSize { get; set; }

        public Instruction Assign(Identifier id, Expression e)
        {
            var ass = new Assignment(id, e);
            this.Emit(ass);
            return ass;
        }

        private IRBlock EnsureBlock(string? name)
        {
            name ??= NamingPolicy.Instance.BlockName(addrCur);
            if (this.block is null)
                this.block = new IRBlock(addrCur, name);
            return this.block;
        }

        private void Emit(Instruction instr)
        {
            EnsureBlock(null).AddStatement(addrCur, instr);
            addrCur += InstructionSize;
        }
    }
}