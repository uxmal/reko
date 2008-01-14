/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Collections;
using System;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// Supports the creation of a procedure without having to go through assembler.
	/// </summary>
	public class ProcedureMock
	{
		private Block block;
		private Hashtable blocks;
		private Procedure proc;
		private Block branchBlock;
		private Block lastBlock;
		private int numBlock;
		private ProgramMock programMock;
		private int localStackOffset;
		private ArrayList unresolvedProcedures;

		public ProcedureMock()
		{
			Init(this.GetType().Name);
		}

		public ProcedureMock(string name)
		{
			Init(name);
		}

		public Statement Emit(Instruction instr)
		{
			EnsureBlock(null);
			block.Statements.Add(instr);
			return block.Statements.Last;
		}

		/// <summary>
		/// Creates a statement that assigns the sum of left and right to sum.
		/// </summary>
		/// <param name="sum"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public Statement Add(Identifier sum, Expression left, Expression right)
		{
			return Emit(new Assignment(sum, Add(left, right)));
		}

		public Statement Add(Identifier sum, Expression left, int n)
		{
			return Emit(new Assignment(sum, Add(left, new Constant(left.DataType, n))));
		}

		
		public BinaryExpression Add(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.add, left.DataType, left, right);
		}

		public BinaryExpression Add(Expression left, int right)
		{
			return new BinaryExpression(Operator.add, left.DataType, left, new Constant(left.DataType, right));
		}

		public UnaryExpression AddrOf(Expression e)
		{
			return new UnaryExpression(UnaryOperator.addrOf, PrimitiveType.Pointer, e);
		}

		public BinaryExpression And(Expression left, int right)
		{
			return new BinaryExpression(Operator.and, left.DataType, left, new Constant(left.DataType, right));
		}

		public virtual Statement Assign(Identifier dst, Expression src)
		{
			return Emit(new Assignment(dst, src));
		}

		public virtual Statement Assign(Identifier dst, int n)
		{
			return Emit(new Assignment(dst, new Constant(dst.DataType, n)));
		}

		public virtual Statement Assign(Identifier dst, bool f)
		{
			return Assign(dst, new Constant(PrimitiveType.Bool, f ? 1 : 0));
		}

		/// <summary>
		/// Current block, into which the next statement will be added.
		/// </summary>
		public Block Block
		{
			get { return block; }
		}

		private Block BlockOf(string label)
		{
			Block b = (Block) blocks[label];
			if (b == null)
			{
				b = new Block(proc, label);
				blocks.Add(label, b);
			}
			return b;
		}

		public void Branch(ConditionCode cc, string label)
		{
			Identifier f;
			switch (cc)
			{
			case ConditionCode.EQ: case ConditionCode.NE: f = Flags("Z"); break;
			default: throw new ArgumentOutOfRangeException("Condition code: " + cc);
			}
			Emit(new Branch(cc, f));
			TerminateBlock();
			branchBlock = BlockOf(label);
		}

		public Statement BranchIf(Expression expr, string label)
		{
			Statement stm = Emit(new Branch(expr));
			TerminateBlock();
			branchBlock = BlockOf(label);
			return stm;
		}

		protected virtual void BuildBody()
		{
		}

		public Statement Call(string procedureName)
		{
			CallInstruction ci = new CallInstruction(null, 0, 0);
			unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
			return Emit(ci);
		}

		public Statement Call(Procedure callee)
		{
			CallInstruction ci = new CallInstruction(callee, 0, 0);
			return Emit(ci);
		}

		public void Compare(string flags, Expression a, Expression b)
		{
			Assign(Flags(flags), new ConditionOf(Sub(a, b)));
		}


		public Identifier Declare(DataType dt, string name)
		{
			return proc.Frame.CreateTemporary(name, dt);
		}

		public void Declare(Identifier id, Expression initial)
		{
			Emit(new Declaration(id, initial));
		}

		public void Dpb(Identifier dst, Expression src, int offset, int bitCount)
		{
			Assign(dst, new DepositBits(dst, src, offset, bitCount));
		}

		private Block EnsureBlock(string name)
		{
			if (block != null)
				return block;

			if (name == null)
			{
				name = string.Format("l{0}", ++numBlock);
			}
			block = BlockOf(name);
			if (proc.EntryBlock.Succ.Count == 0)
			{
				Block.AddEdge(proc.EntryBlock, block);
			}
			
			if (lastBlock != null)
			{
				if (branchBlock != null)
				{
					Block.AddEdge(lastBlock, block);
					Block.AddEdge(lastBlock, branchBlock);
					branchBlock = null;
				}
				else
				{
					Block.AddEdge(lastBlock, block);
				}
				lastBlock = null;
			}
			return block;
		}

		public BinaryExpression Eq(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.eq, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Eq(Expression a, int b)
		{
			return new BinaryExpression(Operator.eq, PrimitiveType.Bool, a, new Constant(a.DataType, b));
		}

		public BinaryExpression Ugt(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.ugt, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Uge(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.uge, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Uge(Expression a, int n)
		{
			return new BinaryExpression(Operator.uge, PrimitiveType.Bool, a, new Constant(a.DataType, n));
		}

		public Identifier Flags(string s)
		{
			uint grf = 0;
			for (int i = 0; i < s.Length; ++i)
			{
				switch (s[i])
				{
				case 'S': grf |= 0x01; break;
				case 'Z': grf |= 0x02; break;
				case 'C': grf |= 0x04; break;
				}
			}
			return proc.Frame.EnsureFlagGroup(grf, s, PrimitiveType.Byte);
		}

		public Frame Frame
		{
			get { return proc.Frame; }
		}

		public Application Fn(string name, params Expression [] exps)
		{
			return new Application(new ProcedureConstant(PrimitiveType.Pointer, new PseudoProcedure(name, 0)), PrimitiveType.Word32, exps);
		}

		public Application Fn(Expression e, params Expression[] exps)
		{
			return new Application(e, PrimitiveType.Word32, exps);
		}

		public Expression Ge(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.ge, PrimitiveType.Bool, a, b);
		}

		public Expression Ge(Expression a, int b)
		{
			return Ge(a, Int32(b));
		}

		public Expression Gt(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.gt, PrimitiveType.Bool, a, b);
		}

		public Expression Gt(Expression a, int b)
		{
			return Gt(a, Int32(b));
		}


		private void Init(string name)
		{
			blocks = new Hashtable();
			proc = new Procedure(name, new Frame(PrimitiveType.Word32));
			unresolvedProcedures = new ArrayList();
			BuildBody();
			proc.RenumberBlocks();
		}


		public Constant Int8(int n)
		{
			return new Constant(PrimitiveType.Byte, n);
		}

		public Constant Int16(short n)
		{
			return new Constant(PrimitiveType.Word16, n);
		}

		public Constant Int16(uint n)
		{
			return new Constant(PrimitiveType.Word16, n);
		}

		public Constant Int32(uint n)
		{
			return new Constant(PrimitiveType.Word32, n);
		}

		public Constant Int32(int n)
		{
			return Constant.Word32(n);
		}

		public void Jump(string name)
		{
			EnsureBlock(null);
			Block blockTo = BlockOf(name);
			Block.AddEdge(block, blockTo);
			block = null;
		}

		public Block Label(string name)
		{
			TerminateBlock();
			return EnsureBlock(name);
		}

		public void Load(Identifier reg, Expression ea)
		{
			Assign(reg, new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, reg.DataType));
		}

		public MemoryAccess Load(DataType dt, Expression ea)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, dt);
		}

		public MemoryAccess LoadDw(Expression ea)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word32);
		}

		public MemoryAccess LoadW(Expression ea)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word16);
		}

		public MemoryAccess LoadW(int i)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory, new Constant(i), PrimitiveType.Word16);
		}

		public BinaryExpression Lt(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.lt, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Lt(Expression a, int b)
		{
			return Lt(a, Int32(b));
		}

		public MemberPointerSelector MembPtr(Expression ptr, Expression membPtr)
		{
			return new MemberPointerSelector(ptr, membPtr);
		}

		public MkSequence Seq(Expression head, Expression tail)
		{
			return new MkSequence(
				PrimitiveType.Create(((PrimitiveType)head.DataType).Domain, head.DataType.Size + tail.DataType.Size),
				head, 
				tail);
		}

		public SegmentedAccess SegMem(DataType dt, Expression basePtr, Expression ptr)
		{
			return new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ptr, dt); 
		}

		public SegmentedAccess SegMemW(Expression basePtr, Expression ptr)
		{
			return new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ptr, PrimitiveType.Word16); 
		}

		public Procedure Procedure
		{
			get { return proc; }
		}

		public ProgramMock ProgramMock
		{
			get { return programMock; }
			set { programMock = value; }
		}

		[Obsolete("Replace with Ass(Muls(left, right))")]
		public void Muls(Identifier product, Expression left, Expression right)
		{
			Emit(new Assignment(product, Muls(left, right)));
		}

		public Expression Mul(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.mul, left.DataType, left, right);
		}

		public Expression Mul(Expression left, int c)
		{
			return new BinaryExpression(Operator.mul, left.DataType, left, new Constant(left.DataType, c));
		}

		public Expression Muls(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.muls, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, right);
		}

		public Expression Muls(Expression left, int c)
		{
			return new BinaryExpression(Operator.muls, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, new Constant(left.DataType, c));
		}

		public Expression Mulu(Expression left, int c)
		{
			return new BinaryExpression(Operator.mulu, PrimitiveType.Create(Domain.UnsignedInt, left.DataType.Size), left, new Constant(left.DataType, c));
		}

		public BinaryExpression Ne(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.ne, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Ne(Expression a, int n)
		{
			return new BinaryExpression(Operator.ne, PrimitiveType.Bool, a, new Constant(a.DataType, n));
		}

		public UnaryExpression Not(Expression exp)
		{
			return new UnaryExpression(Operator.not, PrimitiveType.Bool, exp);
		}

		public Expression Or(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.or, a.DataType, a, b);
		}

		public Statement Phi(Identifier idDst, params Identifier [] ids)
		{
			return Emit(new PhiAssignment(idDst, new PhiFunction(idDst.DataType, ids)));
		}

		public Identifier Register(int i)
		{
			return proc.Frame.EnsureRegister(ArchitectureMock.GetMachineRegister(i));
		}

		public void Return()
		{
			Return(null);
		}

		public void Return(Expression exp)
		{
			Emit(new ReturnInstruction(exp));
			Block.AddEdge(block, proc.ExitBlock);
			block = null;
		}

		public BinaryExpression Shl(Expression e, Expression sh)
		{
			return new BinaryExpression(Operator.shl, e.DataType, e, sh);
		}

		public BinaryExpression Shl(Expression e, int sh)
		{
			return new BinaryExpression(Operator.shl, e.DataType, e, new Constant(e.DataType, sh));
		}


		public BinaryExpression Shl(int c, Expression sh)
		{
			Constant cc = Constant.Word32(c);
			return new BinaryExpression(Operator.shl, cc.DataType, cc, sh);
		}

		public Statement SideEffect(Expression side)
		{
			return Emit(new SideEffect(side));
		}

		public Statement Store(Expression ea, int n)
		{
			return Store(ea, Int32(n));
		}

		public Statement Store(Expression ea, Expression expr)
		{
			Store s = new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, expr.DataType), expr);
			return Emit(s);
		}

		public Statement Store(SegmentedAccess s, Expression exp)
		{
			return Emit(new Store(s, exp));
		}

		public Statement Sub(Identifier diff, Expression left, Expression right)
		{
			return Emit(new Assignment(diff, Sub(left, right)));
		}

		public Statement Sub(Identifier diff, Expression left, int right)
		{
			return Sub(diff, left, Int32(right));
		}

		public BinaryExpression Sub(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.sub, left.DataType, left, right);
		}

		public BinaryExpression Sub(Expression left, int right)
		{
			return Sub(left, new Constant(left.DataType, right));
		}

		public void Switch(Expression e, params string [] labels)
		{
			Block [] blox = new Block[labels.Length];
			for (int i = 0; i < blox.Length; ++i)
			{
				blox[i] = BlockOf(labels[i]);
			}

			Emit(new SwitchInstruction(e, blox));
			for (int i = 0; i < blox.Length; ++i)
			{
				Block.AddEdge(this.block, blox[i]);
			}
            TerminateBlock();
		}

		public Identifier LocalBool(string name)
		{
			localStackOffset -= PrimitiveType.Word32.Size;
			return proc.Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Bool, name);
		}

		public Identifier LocalByte(string name)
		{
			localStackOffset -= PrimitiveType.Word32.Size;
			return proc.Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Byte, name);
		}

		public Identifier Local16(string name)
		{
			localStackOffset -= PrimitiveType.Word32.Size;
			return proc.Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word16, name);
		}

		public Identifier Local32(string name)
		{
			localStackOffset -= PrimitiveType.Word32.Size;
			return proc.Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word32, name);
		}


		private void TerminateBlock()
		{
			if (block != null)
			{
				lastBlock = block;
				block = null;
			}
		}

		public ICollection UnresolvedProcedures
		{
			get { return unresolvedProcedures; }
		}

		public Statement Use(Identifier id)
		{
			return Emit(new UseInstruction(id));
		}

		public Constant Word16(short n)
		{
			return new Constant(PrimitiveType.Word16, n);
		}


		public Constant Word16(uint n)
		{
			return new Constant(PrimitiveType.Word16, n);
		}
	}
}