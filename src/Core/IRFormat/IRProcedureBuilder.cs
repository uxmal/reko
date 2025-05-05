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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.IRFormat
{
    /// <summary>
    /// Builds an intermediate representation of a procedure.
    /// </summary>
    public class IRProcedureBuilder : ExpressionEmitter
    {
        private IProcessorArchitecture arch;
        private IRBlock? block;
        private Address addrCur;
        private string name;

        /// <summary>
        /// Constructs an <see cref="IRProcedureBuilder"/> with the given name, architecture, and address.
        /// </summary>
        /// <param name="name">Name of the procedure being built.</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> of the procedure.</param>
        /// <param name="address">Address of the procedure.</param>
        public IRProcedureBuilder(string name, IProcessorArchitecture arch, Address address)
        {
            this.name = name;
            this.arch = arch;
            this.addrCur = address;
            this.BlocksByName = new Dictionary<string, IRBlock>();
        }

        /// <summary>
        /// Size of instruction.
        /// </summary>
        public uint InstructionSize { get; set; }

        /// <summary>
        /// The blocks of the procedure, indexed by their name.
        /// </summary>
        public Dictionary<string, IRBlock> BlocksByName { get; }

        /// <summary>
        /// Creates an assignment instruction.
        /// </summary>
        /// <param name="id">Destination of the assignment.</param>
        /// <param name="e">Source of the assignment.</param>
        /// <returns>An <see cref="Assignment"/> statement.</returns>
        public Instruction Assign(Identifier id, Expression e)
        {
            var ass = new Assignment(id, e);
            this.Emit(ass);
            return ass;
        }

        /// <summary>
        /// Creates a new basic block, with the given label.
        /// </summary>
        /// <param name="name">Name of the label.</param>
        public void Label(string name)
        {
            var oldBlock = this.block;
            this.block = null;
            var newBlock = EnsureBlock(name);
            BlocksByName.Add(name, newBlock);
            if (oldBlock is not null)
            {
                AddEdge(oldBlock, this.block);
            }
        }

        private void AddEdge(IRBlock oldBlock, IRBlock? block)
        {
            //$TODO
        }

        private IRBlock EnsureBlock(string? name)
        {
            var id = NamingPolicy.Instance.BlockName(addrCur);
            if (this.block is null)
                this.block = new IRBlock(addrCur, id, name);
            return this.block;
        }

        private void Emit(Instruction instr)
        {
            EnsureBlock(null).AddStatement(addrCur, instr);
            addrCur += InstructionSize;
        }

        /// <summary>
        /// Emits a store instruction.
        /// </summary>
        /// <param name="memid">Memory identifier.</param>
        /// <param name="dt">Data type of the store.</param>
        /// <param name="ea">Effective address of the store.</param>
        /// <param name="src">Source of the store.</param>
        /// <returns>A <see cref="Store"/> instruction.</returns>
        public Instruction Store(Identifier memid, DataType dt, Expression ea, Expression src)
        {
            var mem = Mem(memid, dt, ea);
            var store = new Store(mem, src);
            Emit(store);
            return store;
        }
    }
}