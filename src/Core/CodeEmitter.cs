#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

        public Statement Phi(Identifier idDst, params Expression[] exprs)
        {
            return Emit(new PhiAssignment(idDst, new PhiFunction(idDst.DataType, exprs)));
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

        public Statement Store32(Expression ea, int n)
        {
            return Store(ea, Word32(n));
        }

        public Statement Store(Expression ea, Expression src)
        {
            Store s = new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, src.DataType), src);
            return Emit(s);
        }


        public Statement Store(MemoryIdentifier mid, Expression ea, Expression src)
        {
            Store s = new Store(new MemoryAccess(mid, ea, src.DataType), src);
            return Emit(s);
        }

        public Statement SegStore(Expression basePtr, Expression ea, Expression src)
        {
            Store s = new Store(new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ea, src.DataType), src);
            return Emit(s);
        }

        public Statement Store(SegmentedAccess s, Expression exp)
        {
            return Emit(new Store(s, exp));
        }

        public Identifier Local(PrimitiveType primitiveType, string name)
        {
            localStackOffset -= primitiveType.Size;
            return Frame.EnsureStackLocal(localStackOffset, primitiveType, name);
        }

        public Identifier LocalBool(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Bool, name);
        }

        public Identifier LocalByte(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Byte, name);
        }

        public Identifier Local16(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word16, name);
        }

        public Identifier Local32(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word32, name);
        }

        public virtual Identifier Local32(string name, int offset)
        {
            Debug.Assert(offset < 0);
            return Frame.EnsureStackLocal(offset, PrimitiveType.Word32, name);
        }

        public Identifier Temp(DataType type, string name)
        {
            return Frame.CreateTemporary(name, type);
        }

        public Statement Use(Identifier id)
        {
            return Emit(new UseInstruction(id));
        }
    }
}
