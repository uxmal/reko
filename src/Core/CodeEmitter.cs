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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Provides an interface for simple generation of intermediate code.
    /// </summary>
    public abstract class CodeEmitter : ExpressionEmitter
    {
        private int localStackOffset;

        public abstract Statement Emit(Instruction instr);
        public abstract Frame Frame { get; }

        public virtual AliasAssignment Alias(Identifier dst, Expression src)
        {
            var ass = new AliasAssignment(dst, src);
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, Expression src)
        {
            var ass = new Assignment(dst, src);
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, int n)
        {
            return Assign(dst, Constant.Create((PrimitiveType)dst.DataType, n));
        }

        public virtual Assignment Assign(Identifier dst, bool f)
        {
            return Assign(dst, f ? Constant.True() : Constant.False());
        }

        public void Comment(string comment)
        {
            Emit(new CodeComment(comment));
        }

        public void Def(Identifier id)
        {
            Emit(new DefInstruction(id));
        }

        public GotoInstruction Goto(Expression dest)
        {
            var gi = new GotoInstruction(dest);
            Emit(gi);
            return gi;
        }

        public virtual GotoInstruction Goto(uint linearAddress)
        {
            var gi = new GotoInstruction(Address.Ptr32(linearAddress));
            Emit(gi);
            return gi;
        }

        public void LoadId(Identifier reg, Expression ea)
        {
            Assign(reg, new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, reg.DataType));
        }

        public virtual void Return()
        {
            Emit(new ReturnInstruction());
        }

        public virtual void Return(Expression exp)
        {
            Emit(new ReturnInstruction(exp));
        }

        public Statement SideEffect(Expression side)
        {
            return Emit(new SideEffect(side));
        }

        /// <summary>
        /// Generates a`Store` instruction. Callers are expected to provide an
        /// assignable L-Value; constants are not allowed.
        /// </summary>
        /// <param name="dst">L-value destination of the store</param>
        /// <param name="src">Source r-value.</param>
        /// <returns></returns>
        public Statement Store(Expression dst, Expression src)
        {
            if (dst is Identifier)
                throw new ArgumentException("Use the 'Assign' method for identifiers.", nameof(dst));
            if (dst is MemoryAccess ||
                dst is SegmentedAccess ||
                dst is FieldAccess ||
                dst is ArrayAccess ||
                dst is MemberPointerSelector ||
                dst is Dereference)
            {
                return Emit(new Store(dst, src));
            }
            throw new ArgumentException(
                $"An expression of the type {dst.GetType().Name} is not an L-value.", nameof(dst));
        }

        /// <summary>
        /// Takes the effective address <paramref name="ea"/>
        /// and wraps it in a memory access expression. The resulting expression
        /// is used to generate the l-value of a `Store` instruction.
        /// </summary>
        /// <param name="ea">Effective address to be wrapped.</param>
        /// <param name="src">R-Value.</param>
        public Statement MStore(Expression ea, Expression src)
        {
            Store s = new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, src.DataType), src);
            return Emit(s);
        }

        public Statement MStore(MemoryIdentifier mem, Expression ea, Expression src)
        {
            Store s = new Store(new MemoryAccess(mem, ea, src.DataType), src);
            return Emit(s);
        }

        /// <summary>
        /// Convenience method that takes the base pointer <paramref name="basePtr"/>>
        /// and the effective address <paramref name="ea"/>
        /// and wraps them in a segmented memory access expression. The resulting expression
        /// is used to generate the l-value of a `Store` instruction.
        /// </summary>
        /// <param name="ea">Effective address to be wrapped.</param>
        /// <param name="src">R-Value.</param>
        public Statement SStore(Expression basePtr, Expression ea, Expression src)
        {
            Store s = new Store(new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ea, src.DataType), src);
            return Emit(s);
        }

        /// <summary>
        /// Allocate a stack-based identifier.
        /// </summary>
        /// <param name="primitiveType">Data type of the identifier</param>
        /// <param name="name">name of the identifier.</param>
        public Identifier Local(PrimitiveType primitiveType, string name)
        {
            localStackOffset -= primitiveType.Size;
            return Frame.EnsureStackLocal(localStackOffset, primitiveType, name);
        }

        /// <summary>
        /// Convenience method that allocates a stack-based boolean, aligned to 32 bits.
        /// </summary>
        public Identifier LocalBool(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Bool, name);
        }

        /// <summary>
        /// Convenience method that allocates a stack-based byte, aligned to 32 bits.
        /// </summary>
        public Identifier LocalByte(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Byte, name);
        }

        /// <summary>
        /// Convenience method that allocates a stack-based 16-bit word, aligned to 32 bits.
        /// </summary>
        public Identifier Local16(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Local16(name, localStackOffset);
        }

        /// <summary>
        /// Allocates a stack-based 16-bit variable  named <paramref name="name"/> at the 
        /// given <paramref name="offset"/>.
        /// </summary>
        public virtual Identifier Local16(string name, int offset)
        {
            Debug.Assert(offset < 0);
            return Frame.EnsureStackLocal(offset, PrimitiveType.Word16, name);
        }

        /// <summary>
        /// Convenience method that allocates a stack-based 32-bit word.
        /// </summary>
        public Identifier Local32(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word32, name);
        }

        /// <summary>
        /// Allocates a stack-based 32-bit variable  named <paramref name="name"/> at the 
        /// given <paramref name="offset"/>.
        /// </summary>
        public virtual Identifier Local32(string name, int offset)
        {
            Debug.Assert(offset < 0);
            return Frame.EnsureStackLocal(offset, PrimitiveType.Word32, name);
        }

        /// <summary>
        /// Generate a temporary identifier named <paramref name="name"/> with the data type <paramref name="type"/>.
        /// </summary>
        public virtual Identifier Temp(DataType type, string name)
        {
            return Frame.CreateTemporary(name, type);
        }

        public Statement Use(Identifier id)
        {
            return Emit(new UseInstruction(id));
        }
    }
}
