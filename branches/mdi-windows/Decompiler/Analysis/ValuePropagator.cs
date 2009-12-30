/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Analysis.Simplification;
using Decompiler.Core;
using Decompiler.Core.Code;
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
	public class ValuePropagator : InstructionTransformer
	{
		private SsaIdentifierCollection ssaIds;
		private Procedure proc;
		private Statement stm;
		private bool changed;
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
		private ShiftShift_c_c_Rule  shiftShift;
        private SliceShift sliceShift;
		private NegSub_Rule negSub;
		private Mps_Constant_Rule mpsRule;

		private static TraceSwitch trace = new TraceSwitch("ValuePropagation", "Traces value propagation");

		public ValuePropagator(SsaIdentifierCollection ssaIds, Procedure proc)
		{
			this.ssaIds = ssaIds;
			this.proc = proc;

			this.add2ids = new AddTwoIdsRule(ssaIds);
			this.addEcc = new Add_e_c_cRule(ssaIds);
			this.addMici = new Add_mul_id_c_id_Rule(ssaIds);
			this.dpbConstantRule = new DpbConstantRule();
			this.idConst = new IdConstant(ssaIds, new Decompiler.Typing.Unifier(null));
			this.idCopyPropagation = new IdCopyPropagationRule(ssaIds);
			this.idBinIdc = new IdBinIdc_Rule(ssaIds);
			this.sliceConst = new SliceConstant_Rule();
			this.sliceMem = new SliceMem_Rule();
			this.negSub = new NegSub_Rule();
			this.constConstBin = new ConstConstBin_Rule();
			this.shMul = new Shl_mul_e_Rule(ssaIds);
			this.shiftShift = new ShiftShift_c_c_Rule(ssaIds);
			this.mpsRule = new Mps_Constant_Rule(ssaIds);
            this.sliceShift = new SliceShift(ssaIds);
		}


		private void AddUses(Expression expr)
		{
			ExpressionUseAdder uf = new ExpressionUseAdder(stm, ssaIds);
			expr.Accept(uf);
		}

		public bool Changed
		{
			get { return changed; }
			set { changed = value; }
		}


		private Expression DefiningExpression(Identifier id)
		{
			if (id == null)
				return null;
			SsaIdentifier ssaId = ssaIds[id];
			if (ssaId.DefStatement == null)
				return null;
			Assignment ass = ssaId.DefStatement.Instruction as Assignment;
			if (ass != null && ass.Dst == ssaId.Identifier)
			{
				return ass.Src;
			}
			return null;
		}

		private bool IsAddOrSub(Operator op)
		{
			return op == Operator.add || op == Operator.sub; 
		}

		private bool IsComparison(Operator op)
		{
			return op == Operator.eq || op == Operator.ne ||
				   op == Operator.ge || op == Operator.gt ||
				   op == Operator.le || op == Operator.lt ||
				   op == Operator.uge || op == Operator.ugt ||
				   op == Operator.ule || op == Operator.ult;
		}

		private void RemoveUse(Identifier id)
		{
			if (id != null)
				ssaIds[id].Uses.Remove(stm);
		}

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
			this.stm = stm;
			if (trace.TraceVerbose) Debug.WriteLine(string.Format("From: {0}", stm.Instruction.ToString()));
			stm.Instruction = stm.Instruction.Accept(this);
			if (trace.TraceVerbose) Debug.WriteLine(string.Format("  To: {0}", stm.Instruction.ToString()));
		}

		public override Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			return a;
		}

		public override Expression TransformCast(Cast cast)
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


		public override Instruction TransformStore(Store store)
		{
			store.Src = store.Src.Accept(this);
			store.Dst = store.Dst.Accept(this);
			return store;
		}

		public override Expression TransformBinaryExpression(BinaryExpression binExp)
		{
			// (+ id1 id1) ==> (* id1 2)

			if (add2ids.Match(binExp))
			{
				Changed = true;
				return add2ids.Transform(stm).Accept(this);
			}

			binExp.Left = binExp.Left.Accept(this);
			binExp.Right = binExp.Right.Accept(this);

			if (constConstBin.Match(binExp))
			{
				Changed = true;
				return constConstBin.Transform(stm);
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

			Expression left = DefiningExpression(idLeft);
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
				RemoveUse(idLeft);
				AddUses(left);
				Constant c;
				if (binLeft.op == binExp.op)
				{
					c = Operator.add.ApplyConstants(cLeftRight, cRight);
				}
				else
				{
					c = Operator.sub.ApplyConstants(cRight, cLeftRight);
				}
				return new BinaryExpression(binExp.op, binExp.DataType, binLeft.Left, c);
			}

			// (== (- e c1) c2) => (== e c1+c2)

			if (binLeft != null && cLeftRight != null && cRight != null &&
				IsComparison(binExp.op) && IsAddOrSub(binLeft.op) &&
				!cLeftRight.IsReal && !cRight.IsReal)
			{
				RemoveUse(idLeft);
				BinaryOperator op = binLeft.op == Operator.add ? Operator.sub : Operator.add;
				Constant c = ExpressionSimplifier.SimplifyTwoConstants(op, cLeftRight, cRight);
				return new BinaryExpression(binExp.op, PrimitiveType.Bool, binLeft.Left, c);
			}

			if (addMici.Match(binExp))
			{
				Changed = true;
				return addMici.Transform(this.stm);
			}

			if (shMul.Match(binExp))
			{
				Changed = true;
				return shMul.Transform(stm);
			}

			if (shiftShift.Match(binExp))
			{
				Changed = true;
				return shiftShift.Transform(stm);
			}

			// No change, just return as is.

			return binExp;
		}

		public override Expression TransformDepositBits(DepositBits d)
		{
			d.Source = d.Source.Accept(this);
			d.InsertedBits = d.InsertedBits.Accept(this);
			if (dpbConstantRule.Match(d))
			{
				Changed = true;
				return dpbConstantRule.Transform(stm);
			}
			return d;
		}


		public override Expression TransformIdentifier(Identifier id)
		{
			if (idConst.Match(id))
			{
				Changed = true;
				return idConst.Transform(stm);
			}
			if (idCopyPropagation.Match(id))
			{
				Changed = true;
				return idCopyPropagation.Transform(stm);
			}
			if (idBinIdc.Match(id))
			{
				Changed = true;
				return idBinIdc.Transform(stm);
			}
			return id;
		}

		public override Expression TransformMemberPointerSelector(MemberPointerSelector mps)
		{
			if (mpsRule.Match(mps))
			{
				Changed = true;
				return mpsRule.Transform(stm);
			}
			return mps;
		}

		public override Expression TransformMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			return access;
		}

		public override Expression TransformMkSequence(MkSequence seq)
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
				if (tHead.Domain == Domain.Selector)			//$REVIEW: seems to require Address, SegmentedAddress?
					t = PrimitiveType.Create(Domain.Pointer, tHead.Size + tTail.Size);
				else
					t = PrimitiveType.Create(tHead.Domain, tHead.Size + tTail.Size);
				Changed = true;
				return new Constant(t, c1.ToInt32() << tHead.BitSize | c2.ToInt32());
			}
			return seq;
		}


		public override Instruction TransformPhiAssignment(PhiAssignment phi)
		{
			return phi;
		}

		public override Expression TransformSlice(Slice slice)
		{
			slice.Expression = slice.Expression.Accept(this);
			if (sliceConst.Match(slice))
			{
				Changed = true;
				return sliceConst.Transform(stm);
			}
			if (sliceMem.Match(slice))
			{
				Changed = true;
				return sliceMem.Transform(stm);
			}

            // (slice (shl e n) n) ==> e
            if (sliceShift.Match(slice))
            {
                Changed = true;
                return sliceShift.Transform(stm);
            }
			return slice;
		}


		public override Expression TransformUnaryExpression(UnaryExpression unary)
		{
			unary.Expression = unary.Expression.Accept(this);
			if (negSub.Match(unary))
			{
				Changed = true;
				return negSub.Transform(stm);
			}
			return unary;
		}

		public override Instruction TransformUseInstruction(UseInstruction use)
		{
			return use;
		}

	}
}
