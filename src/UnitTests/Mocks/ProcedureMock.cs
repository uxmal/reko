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
using System.Collections.Generic;
using System;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// Supports the creation of a procedure without having to go through assembler.
	/// </summary>
	public class ProcedureMock
	{
		private Block block;
		private Dictionary<string,Block> blocks;
		private Procedure proc;
		private Block branchBlock;
		private Block lastBlock;
		private int numBlock;
		private ProgramMock programMock;
		private int localStackOffset;
		private List<ProcUpdater> unresolvedProcedures;

		public ProcedureMock()
		{
			Init(this.GetType().Name);
		}

		public ProcedureMock(string name)
		{
			Init(name);
		}

        private void Init(string name)
        {
            blocks = new Dictionary<string, Block>();
            proc = new Procedure(name, new Frame(PrimitiveType.Word32));
            unresolvedProcedures = new List<ProcUpdater>();
            BuildBody();
            proc.RenumberBlocks();
        }



        public ArrayAccess Array(DataType elemType, Expression arrayPtr, Expression index)
        {
            return new ArrayAccess(elemType, arrayPtr, index);
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
			return new BinaryExpression(Operator.Add, left.DataType, left, right);
		}

		public BinaryExpression Add(Expression left, int right)
		{
			return new BinaryExpression(Operator.Add, left.DataType, left, new Constant(left.DataType, right));
		}

		public UnaryExpression AddrOf(Expression e)
		{
			return new UnaryExpression(UnaryOperator.AddrOf, PrimitiveType.Pointer32, e);
		}

		public BinaryExpression And(Expression left, int right)
		{
			return And(left, new Constant(left.DataType, right));
		}

        public BinaryExpression And(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.And, left.DataType, left, right);
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
			Block b;
            if (!blocks.TryGetValue(label, out b))
			{
				b = proc.AddBlock(label);
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
			branchBlock = BlockOf(label);
            Emit(new Branch(new TestCondition(cc, f), branchBlock));
			TerminateBlock();
		}

		public Statement BranchIf(Expression expr, string label)
		{
            Block b = EnsureBlock(null);
			branchBlock = BlockOf(label);
            TerminateBlock();

            Statement stm = new Statement(new Branch(expr, branchBlock), b);
            b.Statements.Add(stm);
            return stm;
		}

		protected virtual void BuildBody()
		{
		}

		public Statement Call(string procedureName)
		{
			CallInstruction ci = new CallInstruction(null, new CallSite(0, 0));
			unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
			return Emit(ci);
		}

		public Statement Call(Procedure callee)
		{
            ProcedureConstant c = new ProcedureConstant(PrimitiveType.Pointer32, callee);
			CallInstruction ci = new CallInstruction(c, new CallSite(0, 0));
			return Emit(ci);
		}

        public Expression Cand(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cand, PrimitiveType.Bool, a, b);
        }

        public Expression Cor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cor, PrimitiveType.Bool, a, b);
        }



		public void Compare(string flags, Expression a, Expression b)
		{
			Assign(Flags(flags), new ConditionOf(Sub(a, b)));
		}

        public Block CurrentBlock
        {
            get { return this.block; }
        }

		public Identifier Declare(DataType dt, string name)
		{
			return proc.Frame.CreateTemporary(name, dt);
		}

		public Identifier Declare(DataType dt, string name, Expression expr)
		{
			Identifier id = proc.Frame.CreateTemporary(name, dt);
            Emit(new Declaration(id, expr));
            return id;
		}


		public Statement Declare(Identifier id, Expression initial)
		{
			return Emit(new Declaration(id, initial));
		}

        public DepositBits Dpb(Expression dst, Expression src, int offset, int bitCount)
        {
            return new DepositBits(dst, src, offset, bitCount);
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
				proc.AddEdge(proc.EntryBlock, block);
			}
			
			if (lastBlock != null)
			{
				if (branchBlock != null)
				{
					proc.AddEdge(lastBlock, block);
					proc.AddEdge(lastBlock, branchBlock);
					branchBlock = null;
				}
				else
				{
					proc.AddEdge(lastBlock, block);
				}
				lastBlock = null;
			}
			return block;
		}

		public BinaryExpression Eq(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Eq(Expression a, int b)
		{
			return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, new Constant(a.DataType, b));
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
			Application appl = new Application(
                new ProcedureConstant(PrimitiveType.Pointer32, new PseudoProcedure(name, PrimitiveType.Void, 0)),
                PrimitiveType.Word32, exps);
			unresolvedProcedures.Add(new ApplicationUpdater(name, appl));
			return appl;
		}

		public Application Fn(Expression e, params Expression[] exps)
		{
			return new Application(e, PrimitiveType.Word32, exps);
		}

		public Expression Ge(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Ge, PrimitiveType.Bool, a, b);
		}

		public Expression Ge(Expression a, int b)
		{
			return Ge(a, new Constant(a.DataType, b));
		}

		public Expression Gt(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Gt, PrimitiveType.Bool, a, b);
		}

		public Expression Gt(Expression a, int b)
		{
			return Gt(a, Int32(b));
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
			proc.AddEdge(block, blockTo);
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

        public BinaryExpression Le(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Le, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Le(Expression a, int b)
        {
            return Le(a, new Constant(a.DataType, b));
        }

		public BinaryExpression Lt(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Lt, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Lt(Expression a, int b)
		{
			return Lt(a, new Constant(a.DataType, b));
		}


		public MemberPointerSelector MembPtrW(Expression ptr, Expression membPtr)
		{
			return new MemberPointerSelector(PrimitiveType.Word16, new Dereference(PrimitiveType.Pointer32, ptr), membPtr);
		}

		public MkSequence Seq(Expression head, Expression tail)
		{
            int totalSize = head.DataType.Size + tail.DataType.Size;
            Domain dom = (head.DataType == PrimitiveType.SegmentSelector)
                ? Domain.Pointer
                : ((PrimitiveType) head.DataType).Domain;
			return new MkSequence(PrimitiveType.Create(dom, totalSize), head, tail);
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

		public Expression Mul(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.Mul, left.DataType, left, right);
		}

		public Expression Mul(Expression left, int c)
		{
			return new BinaryExpression(Operator.Mul, left.DataType, left, new Constant(left.DataType, c));
		}

		public Expression Muls(Expression left, Expression right)
		{
			return new BinaryExpression(Operator.Muls, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, right);
		}

		public Expression Muls(Expression left, int c)
		{
			return new BinaryExpression(Operator.Muls, PrimitiveType.Create(Domain.SignedInt, left.DataType.Size), left, new Constant(left.DataType, c));
		}

        public Expression Mulu(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.Mulu, left.DataType, left, right);
        }

        public Expression Mulu(Expression left, int c)
		{
			return new BinaryExpression(Operator.Mulu, PrimitiveType.Create(Domain.UnsignedInt, left.DataType.Size), left, new Constant(left.DataType, c));
		}

		public BinaryExpression Ne(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, b);
		}

		public BinaryExpression Ne(Expression a, int n)
		{
			return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, new Constant(a.DataType, n));
		}

		public UnaryExpression Not(Expression exp)
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, exp);
		}

		public Expression Or(Expression a, Expression b)
		{
			return new BinaryExpression(Operator.Or, a.DataType, a, b);
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
			proc.AddEdge(block, proc.ExitBlock);
			block = null;
		}

		public BinaryExpression Shl(Expression e, Expression sh)
		{
			return new BinaryExpression(Operator.Shl, e.DataType, e, sh);
		}

		public BinaryExpression Shl(Expression e, int sh)
		{
			return new BinaryExpression(Operator.Shl, e.DataType, e, new Constant(e.DataType, sh));
		}


		public BinaryExpression Shl(int c, Expression sh)
		{
			Constant cc = Constant.Word32(c);
			return new BinaryExpression(Operator.Shl, cc.DataType, cc, sh);
		}

        public BinaryExpression Shr(Expression bx, byte c)
        {
            Constant cc = Constant.Byte(c);
            return new BinaryExpression(Operator.Shr, bx.DataType, bx, cc);
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

        public Statement SegStoreW(Expression basePtr, Expression ea, Expression src)
        {
            Store s = new Store(new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ea, PrimitiveType.Word16), src);
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
			return new BinaryExpression(Operator.Sub, left.DataType, left, right);
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
				proc.AddEdge(this.block, blox[i]);
			}
            lastBlock = null;
            block = null;
		}

		public Identifier Local(PrimitiveType primitiveType, string name)
		{
			localStackOffset -= primitiveType.Size;
			return proc.Frame.EnsureStackLocal(localStackOffset, primitiveType, name);
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


        public Slice Slice(PrimitiveType primitiveType, Identifier value, uint bitOffset)
        {
            return new Slice(primitiveType, value, bitOffset);
        }


		private void TerminateBlock()
		{
			if (block != null)
			{
				lastBlock = block;
				block = null;
			}
		}

		public ICollection<ProcUpdater> UnresolvedProcedures
		{
			get { return unresolvedProcedures; }
		}


        public BinaryExpression Ugt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ugt, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Uge(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Uge(Expression a, int n)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, new Constant(a.DataType, n));
        }

        public BinaryExpression Ule(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ule, PrimitiveType.Bool, a, b);
        }

        public Expression Ult(Expression a, Expression b)
        {
			return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, a, b);
        }

        public Expression Ult(Expression a, int b)
        {
            return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, a, new Constant(PrimitiveType.CreateWord(a.DataType.Size), b));
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

		public Constant Word32(int n)
		{
			return new Constant(PrimitiveType.Word32, n);
		}

        public Expression Xor(Identifier a, Expression b)
        {
            return new BinaryExpression(Operator.Xor, a.DataType, a, b);
        }
    }
}