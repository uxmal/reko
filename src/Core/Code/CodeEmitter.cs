#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Core.Code
{
    /// <summary>
    /// Provides an interface for simple generation of intermediate code.
    /// </summary>
    public abstract class CodeEmitter : ExpressionEmitter
    {
        private int localStackOffset;

        /// <summary>
        /// Current <see cref="Frame"/> instance.
        /// </summary>
        public abstract Frame Frame { get; }

        /// <summary>
        /// Emit a specific <see cref="Instruction"/>.
        /// </summary>
        /// <param name="instr">Instruction to emit.</param>
        /// <returns>The resulting <see cref="Statement"/>.</returns>
        public abstract Statement Emit(Instruction instr);

        /// <summary>
        /// Emits an <see cref="AliasAssignment"/>.
        /// </summary>
        /// <param name="dst">Destination of the assignment.</param>
        /// <param name="src">Source of the assignment.</param>
        /// <returns>The created alias assignment.</returns>
        public virtual AliasAssignment Alias(Identifier dst, Expression src)
        {
            var ass = new AliasAssignment(dst, src);
            Emit(ass);
            return ass;
        }

        /// <summary>
        /// Emits an <see cref="Assignment"/>.
        /// </summary>
        /// <param name="dst">Destination of the assignment.</param>
        /// <param name="src">Source of the assignment.</param>
        /// <returns>The created assignment.</returns>
        public virtual Assignment Assign(Identifier dst, Expression src)
        {
            var ass = new Assignment(dst, src);
            Emit(ass);
            return ass;
        }

        /// <summary>
        /// Emits an <see cref="Assignment"/>. This overload creates 
        /// a constant assignment, where the constant is the same size
        /// as the <paramref name="dst"/> identifier.
        /// </summary>
        /// <param name="dst">Destination of the assignment.</param>
        /// <param name="n">Constant source of the assignment.</param>
        /// <returns>The created assignment.</returns>
        public virtual Assignment Assign(Identifier dst, int n)
        {
            return Assign(dst, Const(dst.DataType, n));
        }

        /// <summary>
        /// Emits an <see cref="Assignment"/>. This overload creates 
        /// a constant boolean assignment, where the constant is the same size
        /// as the <paramref name="dst"/> identifier.
        /// </summary>
        /// <param name="dst">Destination of the assignment.</param>
        /// <param name="f">Constant boolean source of the assignment.</param>
        /// <returns>The created assignment.</returns>
        public virtual Assignment Assign(Identifier dst, bool f)
        {
            return Assign(dst, f ? Constant.True() : Constant.False());
        }

        /// <summary>
        /// Emits a <see cref="CodeComment"/>.
        /// </summary>
        /// <param name="comment">The text of the comment.</param>
        public void Comment(string comment)
        {
            Emit(new CodeComment(comment));
        }

        /// <summary>
        /// Emits a <see cref="DefInstruction"/>.
        /// </summary>
        /// <param name="id">Identifier defined by the <see cref="DefInstruction"/>.</param>
        public void Def(Identifier id)
        {
            Emit(new DefInstruction(id));
        }

        /// <summary>
        /// Emits a <see cref="GotoInstruction"/> to the given <paramref name="target"/>
        /// </summary>
        /// <param name="target">Target of the goto instruction.</param>
        /// <returns>The created goto instruction.</returns>
        public GotoInstruction Goto(Expression target)
        {
            var gi = new GotoInstruction(target);
            Emit(gi);
            return gi;
        }

        /// <summary>
        /// Emits a <c>reg = Mem[ea]</c> assignment. The assignment's size 
        /// is determined by the destination, <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">Assignment destination.</param>
        /// <param name="ea">Effective address to be dereferenced.</param>
        public void LoadId(Identifier reg, Expression ea)
        {
            Assign(reg, new MemoryAccess(MemoryStorage.GlobalMemory, ea, reg.DataType));
        }

        /// <summary>
        /// Generates a <see cref="ReturnInstruction"/> that returns nothing.
        /// </summary>
        public virtual void Return()
        {
            Emit(new ReturnInstruction());
        }

        /// <summary>
        /// Generates a <see cref="ReturnInstruction"/> that returns an expression.
        /// </summary>
        /// <param name="exp">Expression to be returned.</param>
        public virtual void Return(Expression exp)
        {
            Emit(new ReturnInstruction(exp));
        }

        /// <summary>
        /// Generates a <see cref="Code.SideEffect"/>.
        /// </summary>
        /// <param name="side">Expression evaluated for its side effect.</param>
        /// <returns>The resulting <see cref="Statement"/>.</returns>
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
        /// is used to generate the l-value of a <see cref="Store" /> instruction.
        /// </summary>
        /// <param name="ea">Effective address to be wrapped.</param>
        /// <param name="src">R-Value.</param>
        public Statement MStore(Expression ea, Expression src)
        {
            var s = new Store(new MemoryAccess(MemoryStorage.GlobalMemory, ea, src.DataType), src);
            return Emit(s);
        }

        /// <summary>
        /// Takes the effective address <paramref name="ea"/>
        /// and wraps it in a memory access expression. The resulting expression
        /// is used to generate the l-value of a <see cref="Store" /> instruction.
        /// </summary>
        /// <param name="mem">Identifier for the memory space to use.</param>
        /// <param name="ea">Effective address to be wrapped.</param>
        /// <param name="src">R-Value.</param>
        public Statement MStore(Identifier mem, Expression ea, Expression src)
        {
            var s = new Store(new MemoryAccess(mem, ea, src.DataType), src);
            return Emit(s);
        }

        /// <summary>
        /// Convenience method that takes the base pointer <paramref name="basePtr"/>>
        /// and the effective address <paramref name="offset"/>
        /// and wraps them in a segmented memory access expression. The resulting expression
        /// is used to generate the l-value of a `Store` instruction.
        /// </summary>
        /// <param name="basePtr">Base pointer.</param>
        /// <param name="offset">Effective address to be wrapped.</param>
        /// <param name="src">R-Value.</param>
        public Statement SStore(Expression basePtr, Expression offset, Expression src)
        {
            var s = new Store(SegMem(src.DataType, basePtr, offset), src);
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
            return Frame.EnsureStackVariable(offset, PrimitiveType.Word16, name);
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


        /// <summary>
        /// Generate a <see cref="UseInstruction"/> using the given identifier.
        /// </summary>
        /// <param name="id">Identifier to mark as used.</param>
        /// <returns></returns>
        public Statement Use(Identifier id)
        {
            return Emit(new UseInstruction(id));
        }
    }
}
