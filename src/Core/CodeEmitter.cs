#region License 
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
#endregion

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	/// <summary>
	/// Emits code sequences into basic blocks.
	/// </summary>
	public class CodeEmitter : CodeEmitter2
	{
        private IProcessorArchitecture arch;
		private IRewriterHost host;
		private Procedure proc;

		private Block blockCur;

		public CodeEmitter(IProcessorArchitecture arch, IRewriterHost host, Procedure proc, Block block)
		{
            this.arch = arch;
			this.host = host;
			this.proc = proc;
            this.blockCur = block;
		}

		public Block Block
		{
			get { return blockCur; }
		}

        public override Frame Frame
        {
            get { return proc.Frame; } 
        }

        public override Identifier Register(int i)
        {
            return Frame.EnsureRegister(arch.GetRegister(i));
        }

		/// <summary>
		/// Appends the instruction to the end of the list of intstructions of the current block.
		/// </summary>
		/// <param name="instr"></param>
		public override Statement Emit(Instruction instr)
		{
			blockCur.Statements.Add(instr);
            return blockCur.Statements.Last;
		}

        public void Call(Procedure procCallee, CallSite site)
        {
            ProcedureConstant pc = new ProcedureConstant(arch.PointerType, procCallee);
            Emit(new CallInstruction(pc, site));
        }



        public UnaryExpression Neg(Expression expr)
        {
            return new UnaryExpression(Operator.Neg, expr.DataType, expr);
        }


		public Expression PseudoProc(PseudoProcedure ppp, PrimitiveType retType, params Expression [] args)
		{
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));
            
			return new Application(
                new ProcedureConstant(arch.PointerType, ppp),
                retType, 
                args);
		}

		public Expression PseudoProc(string name, PrimitiveType retType, params Expression [] args)
		{
			PseudoProcedure ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
			return PseudoProc(ppp, retType, args);
		}



        /// <summary>
        /// Emits a call to a pseudoprocedure as a side effect, implying the pseudoprocedure is a void function.
        /// </summary>
        /// <param name="ppp"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public SideEffect SideEffect(PseudoProcedure ppp, params Expression[] args)
        {
            SideEffect s = new SideEffect(PseudoProc(ppp, null, args));
            Emit(s);
            return s;
        }

        /// <summary>
        /// Emits a call to a named pseudoprocedure as a side effect, implying the pseudoprocedure is a void function.
        /// </summary>
        /// <param name="ppp"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public SideEffect SideEffect(string name, params Expression[] args)
        {
            PseudoProcedure ppp = host.EnsurePseudoProcedure(name, PrimitiveType.Void, args.Length);
            return SideEffect(ppp, args);
        }

        public SideEffect SideEffect(Application appl)
        {
            SideEffect s = new SideEffect(appl);
            Emit(s);
            return s;
        }

		public Store Store(MemoryAccess dst, Expression src)
		{
			Store s = new Store(dst, src);
			Emit(s);
			return s;
		}



        public void Switch(Expression expr, Block[] jumps)
        {
            Emit(new SwitchInstruction(expr, jumps));
        }

        public IndirectCall IndirectCall(Expression e, CallSite callSite)
        {
            IndirectCall i = new IndirectCall(e, callSite);
            Emit(i);
            return i;
        }


    }

    public abstract class CodeEmitter2
    {
        private int localStackOffset;

        public ArrayAccess Array(DataType elemType, Expression arrayPtr, Expression index)
        {
            return new ArrayAccess(elemType, arrayPtr, index);
        }

        public abstract Statement Emit(Instruction instr);

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

        public virtual Assignment Assign(Identifier dst, Expression src)
        {
            var ass = new Assignment(dst, src);
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, int n)
        {
            var ass = new Assignment(dst, new Constant(dst.DataType, n));
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, bool f)
        {
            return Assign(dst, new Constant(PrimitiveType.Bool, f ? 1 : 0));
        }

        public Branch Branch(Expression condition, Block target)
        {
            Branch b = new Branch(condition, target);
            Emit(b);
            return b;
        }

        public void Call(Expression target)
        {
            Emit(new IndirectCall(target, null));
        }

        public Expression Cand(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cand, PrimitiveType.Bool, a, b);
        }

        public Cast Cast(DataType dataType, Expression expr)
        {
            return new Cast(dataType, expr);
        }

        public Constant Const(DataType dataType, int n)
        {
            return new Constant(dataType, n);
        }

        public Expression Cor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cor, PrimitiveType.Bool, a, b);
        }

        public DepositBits Dpb(Expression dst, Expression src, int offset, int bitCount)
        {
            return new DepositBits(dst, src, offset, bitCount);
        }


        public BinaryExpression Eq(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, b);
        }

        public BinaryExpression Eq(Expression a, int b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, new Constant(a.DataType, b));
        }


        public BinaryExpression Eq0(Expression exp)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, exp, new Constant(exp.DataType, 0));
        }

        public Identifier Flags(uint grf, string name)
        {
            return Frame.EnsureFlagGroup(grf, name, PrimitiveType.Byte);
        }

        public abstract Frame Frame { get; }

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

        public GotoInstruction Goto(Expression dest)
        {
            var gi = new GotoInstruction(dest);
            Emit(gi);
            return gi;
        }

        public GotoInstruction Goto(uint linearAddress)
        {
            var gi = new GotoInstruction(new Address(linearAddress));
            Emit(gi);
            return gi;
        }

        public GotoInstruction IfGoto(Expression condition, Address addr)
        {
            var gi = new GotoInstruction(condition, addr);
            Emit(gi);
            return gi;
        }

        public GotoInstruction IfGoto(Expression condition, uint linearAddress)
        {
            var gi = new GotoInstruction(condition,
                new Constant(PrimitiveType.Word32, linearAddress));
            Emit(gi);
            return gi;
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
            return new Constant(PrimitiveType.Word32, n);
        }

        public void Load(Identifier reg, Expression ea)
        {
            Assign(reg, new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, reg.DataType));
        }

        public MemoryAccess Load(DataType dt, Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, dt);
        }

        public Expression LoadB(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Byte);
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

        public BinaryExpression Ne0(Expression expr)
        {
            return new BinaryExpression(
                Operator.Ne, PrimitiveType.Bool, expr, new Constant(expr.DataType, 0));
        }

        public MkSequence Seq(Expression head, Expression tail)
        {
            int totalSize = head.DataType.Size + tail.DataType.Size;
            Domain dom = (head.DataType == PrimitiveType.SegmentSelector)
                ? Domain.Pointer
                : ((PrimitiveType)head.DataType).Domain;
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

        public Statement Phi(Identifier idDst, params Identifier[] ids)
        {
            return Emit(new PhiAssignment(idDst, new PhiFunction(idDst.DataType, ids)));
        }

        public abstract Identifier Register(int i);

        public void Return()
        {
            Return(null);
        }

        //public virtual void Return(Expression exp)
        //{
        //    Emit(new ReturnInstruction(exp));
        //}

        public virtual void Return(Expression rv)
        {
            Emit(new ReturnInstruction(rv));
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
            Constant cc = new Constant(PrimitiveType.Word32, c);
            return new BinaryExpression(Operator.Shl, cc.DataType, cc, sh);
        }

        public BinaryExpression Shr(Expression bx, byte c)
        {
            Constant cc = new Constant(PrimitiveType.Byte, c);
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


        public Slice Slice(PrimitiveType primitiveType, Identifier value, uint bitOffset)
        {
            return new Slice(primitiveType, value, bitOffset);
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

        public Constant Byte(byte b)
        {
            return new Constant(PrimitiveType.Byte, b);
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
