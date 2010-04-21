/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Scanning
{
	/// <summary>
	/// If a procedure has an escaping frame, rewrite any local stack variable accesses
	/// to memory accesses.
	/// </summary>
	public class EscapedAccessRewriter : InstructionTransformer
	{
		private Procedure proc;

		public EscapedAccessRewriter(Procedure proc)
		{
			this.proc = proc;
		}


		private Instruction Def(Identifier id, Expression src)
		{
			if (IsLocal(id))
				return new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, EffectiveAddress(id), id.DataType), src);
			else 
				return new Assignment(id, src);
		}

		private Expression EffectiveAddress(Identifier id)
		{
			Identifier fp = proc.Frame.FramePointer;
			StackLocalStorage local = proc.Frame.Identifiers[id.Number].Storage as StackLocalStorage;

			int offset = local.StackOffset + proc.Frame.FrameOffset;
			BinaryOperator op;
			if (offset < 0)
			{
				offset = -offset;
				op = Operator.Sub;
			}
			else
			{
				op = Operator.Add;
			}
			PrimitiveType p = (PrimitiveType) fp.DataType;
			Expression ea = new BinaryExpression(op, fp.DataType,
				fp, new Constant(PrimitiveType.Create(Domain.SignedInt, p.Size), offset));
			return ea;
		}

		/// <summary>
		/// Inserts a frame pointer assignment at the beginning of the procedure
		/// </summary>
		/// <remarks>
		/// The frame pointer assignment needs a block of its own, as placing it in the 
		/// "entryBlock" wreaks havoc with SSA assignments &c.
		/// </remarks>
		public void InsertFramePointerAssignment(IProcessorArchitecture arch)
		{
			Block b = proc.AddBlock(proc.Name + "_frame_asgn");
			Block s = proc.EntryBlock.Succ[0];
			proc.RemoveEdge(proc.EntryBlock, s);
            proc.AddEdge(proc.EntryBlock, b);
			proc.AddEdge(b, s);
			StructureType st = new StructureType(proc.Name + "_frame_t", 0);
			Identifier frame = proc.Frame.CreateTemporary(proc.Name + "_frame", st);
			b.Statements.Add(
				new Assignment(
				proc.Frame.FramePointer,
				new UnaryExpression(Operator.AddrOf, arch.FramePointerType, frame)));
		}

		private bool IsLocal(Identifier id)
		{
			return id.Storage is StackLocalStorage;
		}

		public void Transform()
		{
			foreach (Block block in proc.RpoBlocks)
			{
				foreach (Statement stm in block.Statements)
				{
					stm.Instruction = stm.Instruction.Accept(this);
				}
			}
		}

		public override Instruction TransformAssignment(Assignment a)
		{
			return Def(a.Dst, a.Src.Accept(this));
		}

		public override Instruction TransformStore(Store store)
		{
			store.Src = store.Src.Accept(this);
			store.Dst = store.Dst.Accept(this);
			return store;
		}


		public override Expression TransformIdentifier(Identifier id)
		{
			if (IsLocal(id))
				return new MemoryAccess(MemoryIdentifier.GlobalMemory, EffectiveAddress(id), id.DataType);
			else
				return id;
		}
	}
}
