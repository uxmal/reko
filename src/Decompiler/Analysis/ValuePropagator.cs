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

using Decompiler.Analysis.Simplification;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Performs propagation by replacing occurences of expressions with simpler expressions if these are beneficial. 
	/// Constants are folded, and so on.
	/// </summary>
    /// <remarks>
    /// This is a useful transform that doesn't cause too many problems for later transforms. Calling it will flush out
    /// lots of dead expressions that can be removed with DeadCode.Eliminate()
    /// </remarks>
    public class ValuePropagator : InstructionVisitor<Instruction>
    {
        private SsaIdentifierCollection ssaIds;
        private ExpressionValuePropagator eval;
        private SsaEvaluationContext evalCtx;
        private Procedure proc;
        private Statement stm;
        private bool changed;

        private static TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

        public ValuePropagator(SsaIdentifierCollection ssaIds, Procedure proc)
        {
            this.ssaIds = ssaIds;
            this.proc = proc;
            this.evalCtx = new SsaEvaluationContext(ssaIds);
            this.eval = new ExpressionValuePropagator(evalCtx);
        }

        public bool Changed { get { return eval.Changed; } set { eval.Changed = value; } }

        public void Transform()
        {
            do
            {
                Changed = false;
                foreach (Block block in proc.RpoBlocks)
                {
                    for (int i = 0; i < block.Statements.Count; ++i)
                    {
                        Transform(block.Statements[i]);
                    }
                }
            } while (Changed);
        }

        public void Transform(Statement stm)
        {
            evalCtx.Statement = stm;
            if (trace.TraceVerbose) Debug.WriteLine(string.Format("From: {0}", stm.Instruction.ToString()));
            stm.Instruction = stm.Instruction.Accept(this);
            if (trace.TraceVerbose) Debug.WriteLine(string.Format("  To: {0}", stm.Instruction.ToString()));
        }


        #region InstructionVisitor<Instruction> Members

        public Instruction VisitAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(eval);
            return a;
        }

        public Instruction VisitBranch(Branch b)
        {
            b.Condition = b.Condition.Accept(eval);
            return b;
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            return ci;
        }

        public Instruction VisitDeclaration(Declaration decl)
        {
            if (decl.Expression != null)
                decl.Expression = decl.Expression.Accept(eval);
            return decl;
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            return def;
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            return gotoInstruction;
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            return phi;
        }

        public Instruction VisitIndirectCall(IndirectCall ic)
        {
            ic.Callee = ic.Callee.Accept(eval);
            return ic;
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression = ret.Expression.Accept(eval);
            return ret;
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            side.Expression = side.Expression.Accept(eval);
            return side;
        }

        public Instruction VisitStore(Store store)
        {
            store.Src = store.Src.Accept(eval);
            store.Dst = store.Dst.Accept(eval);
            return store;
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression = si.Expression.Accept(eval);
            return si;
        }

        public Instruction VisitUseInstruction(UseInstruction u)
        {
            u.Expression = u.Expression.Accept(eval);
            return u;
        }

        #endregion
    }



    public class ExpressionValuePropagator : ExpressionVisitor<Expression>
    {
        private EvaluationContext ctx;

        private AddTwoIdsRule add2ids;
        private Add_e_c_cRule addEcc;
        private Add_mul_id_c_id_Rule addMici;
        private ConstConstBin_Rule constConstBin;
        private DpbConstantRule dpbConstantRule;
        private IdConstant idConst;
        private IdCopyPropagationRule idCopyPropagation;
        private IdBinIdc_Rule idBinIdc;
        private SliceConstant_Rule sliceConst;
        private SliceMem_Rule sliceMem;
        private Shl_mul_e_Rule shMul;
        private ShiftShift_c_c_Rule shiftShift;
        private SliceShift sliceShift;
        private NegSub_Rule negSub;
        private Mps_Constant_Rule mpsRule;

        public ExpressionValuePropagator(EvaluationContext ctx)
        {
            this.ctx = ctx;

            this.add2ids = new AddTwoIdsRule(ctx);
            this.addEcc = new Add_e_c_cRule(ctx);
            this.addMici = new Add_mul_id_c_id_Rule(ctx);
            this.dpbConstantRule = new DpbConstantRule();
            this.idConst = new IdConstant(ctx, new Decompiler.Typing.Unifier(null));
            this.idCopyPropagation = new IdCopyPropagationRule(ctx);
            this.idBinIdc = new IdBinIdc_Rule(ctx);
            this.sliceConst = new SliceConstant_Rule();
            this.sliceMem = new SliceMem_Rule();
            this.negSub = new NegSub_Rule();
            this.constConstBin = new ConstConstBin_Rule();
            this.shMul = new Shl_mul_e_Rule(ctx);
            this.shiftShift = new ShiftShift_c_c_Rule(ctx);
            this.mpsRule = new Mps_Constant_Rule(ctx);
            this.sliceShift = new SliceShift(ctx);
        }

        public bool Changed { get; set; }

        private bool IsAddOrSub(Operator op)
        {
            return op == Operator.Add || op == Operator.Sub;
        }

        private bool IsComparison(Operator op)
        {
            return op == Operator.Eq || op == Operator.Ne ||
                   op == Operator.Ge || op == Operator.Gt ||
                   op == Operator.Le || op == Operator.Lt ||
                   op == Operator.Uge || op == Operator.Ugt ||
                   op == Operator.Ule || op == Operator.Ult;
        }

        public Expression VisitAddress(Address addr)
        {
            return addr;
        }

        public Expression VisitApplication(Application appl)
        {
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i] = appl.Arguments[i].Accept(this);
            }
            appl.Procedure = appl.Procedure.Accept(this);
            return appl;
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

		public Expression VisitBinaryExpression(BinaryExpression binExp)
		{
			// (+ id1 id1) ==> (* id1 2)

			if (add2ids.Match(binExp))
			{
				Changed = true;
				return add2ids.Transform().Accept(this);
			}

			binExp.Left = binExp.Left.Accept(this);
			binExp.Right = binExp.Right.Accept(this);

			if (constConstBin.Match(binExp))
			{
				Changed = true;
				return constConstBin.Transform();
			}
			Constant cLeft = binExp.Left as Constant; 
			Constant cRight = binExp.Right as Constant;

			if (cLeft != null && BinaryExpression.Commutes(binExp.op))
			{
				cRight = cLeft; binExp.Left = binExp.Right; binExp.Right = cLeft;
			}

			// (- X 0) ==> X
			// (+ X 0) ==> X

			if (cRight != null && cRight.IsIntegerZero && IsAddOrSub(binExp.op))
			{
				Changed = true;
				return binExp.Left;
			}

			Identifier idLeft = binExp.Left as Identifier;
			Identifier idRight = binExp.Right as Identifier;

			// (rel? id1 c) should just pass.

			if (IsComparison(binExp.op) && cRight != null && idLeft != null)
				return binExp;

			// Replace identifier with its definition if possible.

			Expression left = ctx.DefiningExpression(idLeft);
			if (left == null)
				left = binExp.Left;
			BinaryExpression binLeft = left as BinaryExpression;
			Constant cLeftRight = (binLeft != null) ? binLeft.Right as Constant : null;

			// (+ (+ e c1) c2) ==> (+ e (+ c1 c2))
			// (+ (- e c1) c2) ==> (+ e (- c2 c1))
			// (- (+ e c1) c2) ==> (- e (- c2 c1))
			// (- (- e c1) c2) ==> (- e (+ c1 c2))

			if (binLeft != null && cLeftRight != null && cRight != null && 
				IsAddOrSub(binExp.op) && IsAddOrSub(binLeft.op) && 
				!cLeftRight.IsReal && !cRight.IsReal)
			{
                ctx.RemoveIdentifierUse(idLeft);
                ctx.UseExpression(left);
				Constant c;
				if (binLeft.op == binExp.op)
				{
					c = Operator.Add.ApplyConstants(cLeftRight, cRight);
				}
				else
				{
					c = Operator.Sub.ApplyConstants(cRight, cLeftRight);
				}
				return new BinaryExpression(binExp.op, binExp.DataType, binLeft.Left, c);
			}

			// (== (- e c1) c2) => (== e c1+c2)

			if (binLeft != null && cLeftRight != null && cRight != null &&
				IsComparison(binExp.op) && IsAddOrSub(binLeft.op) &&
				!cLeftRight.IsReal && !cRight.IsReal)
			{
				ctx.RemoveIdentifierUse(idLeft);
				BinaryOperator op = binLeft.op == Operator.Add ? Operator.Sub : Operator.Add;
				Constant c = ExpressionSimplifier.SimplifyTwoConstants(op, cLeftRight, cRight);
				return new BinaryExpression(binExp.op, PrimitiveType.Bool, binLeft.Left, c);
			}

			if (addMici.Match(binExp))
			{
				Changed = true;
				return addMici.Transform();
			}

			if (shMul.Match(binExp))
			{
				Changed = true;
				return shMul.Transform();
			}

			if (shiftShift.Match(binExp))
			{
				Changed = true;
				return shiftShift.Transform();
			}

			// No change, just return as is.

			return binExp;
		}

        public Expression VisitCast(Cast cast)
        {
            cast.Expression = cast.Expression.Accept(this);
            Constant c = cast.Expression as Constant;
            if (c != null)
            {
                PrimitiveType p = c.DataType as PrimitiveType;
                if (p != null && p.IsIntegral)
                {
                    //$REVIEW: this is fixed to 32 bits; need a general solution to it.
                    return new Constant(cast.DataType, c.ToUInt64());
                }
            }
            return cast;
        }

        public Expression VisitConditionOf(ConditionOf c)
        {
            c.Expression = c.Expression.Accept(this);
            return c;
        }

        public Expression VisitConstant(Constant c)
        {
            return c;
        }

		public Expression VisitDepositBits(DepositBits d)
		{
			d.Source = d.Source.Accept(this);
			d.InsertedBits = d.InsertedBits.Accept(this);
			if (dpbConstantRule.Match(d))
			{
				Changed = true;
				return dpbConstantRule.Transform();
			}
			return d;
		}

        public Expression VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

		public Expression VisitIdentifier(Identifier id)
		{
			if (idConst.Match(id))
			{
				Changed = true;
				return idConst.Transform();
			}
			if (idCopyPropagation.Match(id))
			{
				Changed = true;
				return idCopyPropagation.Transform();
			}
			if (idBinIdc.Match(id))
			{
				Changed = true;
				return idBinIdc.Transform();
			}
			return id;
		}

		public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			if (mpsRule.Match(mps))
			{
				Changed = true;
				return mpsRule.Transform();
			}
			return mps;
		}

		public Expression VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			return access;
		}

		public Expression VisitMkSequence(MkSequence seq)
		{
			seq.Head = seq.Head.Accept(this);
			seq.Tail = seq.Tail.Accept(this);
			Constant c1 = seq.Head as Constant;
			Constant c2 = seq.Tail as Constant;
			if (c1 != null && c2 != null)
			{
				PrimitiveType tHead = (PrimitiveType) c1.DataType;
				PrimitiveType tTail = (PrimitiveType) c2.DataType;
				PrimitiveType t;
				Changed = true;
                if (tHead.Domain == Domain.Selector)			//$REVIEW: seems to require Address, SegmentedAddress?
                {

                    t = PrimitiveType.Create(Domain.Pointer, tHead.Size + tTail.Size);
                    return new Address(c1.ToUInt16(), c2.ToUInt16());
                }
                else
                {
                    t = PrimitiveType.Create(tHead.Domain, tHead.Size + tTail.Size);
                    return new Constant(t, c1.ToInt32() << tHead.BitSize | c2.ToInt32());
                }
			}
			return seq;
		}

        public Expression VisitPhiFunction(PhiFunction pc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitPointerAddition(PointerAddition pc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitProcedureConstant(ProcedureConstant pc)
        {
            return pc;
        }

        public Expression VisitScopeResolution(ScopeResolution sc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSegmentedAccess(SegmentedAccess segMem)
        {
            throw new NotImplementedException();
        }

		public Expression VisitSlice(Slice slice)
		{
			slice.Expression = slice.Expression.Accept(this);
			if (sliceConst.Match(slice))
			{
				Changed = true;
				return sliceConst.Transform();
			}
			if (sliceMem.Match(slice))
			{
				Changed = true;
				return sliceMem.Transform();
			}

            // (slice (shl e n) n) ==> e
            if (sliceShift.Match(slice))
            {
                Changed = true;
                return sliceShift.Transform();
            }
			return slice;
		}

        public Expression VisitTestCondition(TestCondition tc)
        {
            tc.Expression = tc.Expression.Accept(this);
            return tc;
        }

		public Expression VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression = unary.Expression.Accept(this);
			if (negSub.Match(unary))
			{
				Changed = true;
				return negSub.Transform();
			}
			return unary;
		}
	}
}
