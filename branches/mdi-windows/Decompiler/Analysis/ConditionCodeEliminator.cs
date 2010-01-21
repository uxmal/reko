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
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Removes any uses and definitions of condition codes.
	/// </summary>
	/// <remarks>
	/// Removal of condition codes becomes exciting in situations like the following (x86 code):
	///		add ax,bx
	///		mov [si],ax
	///		jnz foo
	///	or
	///	    cmp ax,0
	///	    jl less
	///	    jg greater
    ///	<para>
    ///	For best performance, preprocess the intermediate code with the ValuePropgator transformer.
    ///	</para>
	/// </remarks>
	public class ConditionCodeEliminator : InstructionTransformer
	{
		private SsaIdentifierCollection ssaIds;
		private SsaIdentifier sidGrf;
		private Statement useStm;
        private IProcessorArchitecture arch;

		private static TraceSwitch trace = new TraceSwitch("CcodeEliminator", "Traces the progress of the condition code eliminator");

		public ConditionCodeEliminator(SsaIdentifierCollection ssaIds, IProcessorArchitecture arch)
		{
			this.ssaIds = ssaIds;
            this.arch = arch;
		}

		public void Transform()
		{
			for (int i = 0; i < ssaIds.Count; ++i)
			{	
				sidGrf = ssaIds[i];
				if (sidGrf.OriginalIdentifier.Storage is FlagGroupStorage && sidGrf.DefStatement != null)
				{
					if (trace.TraceInfo) Debug.WriteLine(string.Format("Tracing {0}", sidGrf.DefStatement.Instruction));
					for (int u = sidGrf.Uses.Count - 1; u >= 0; --u)
					{
						TraverseCopyChain(sidGrf, sidGrf, sidGrf.Uses[u]);
                    }

                    Statement[] uses = sidGrf.Uses.ToArray();
                    foreach (var u in uses)
                    {
                        useStm = u;
						if (trace.TraceInfo) Debug.WriteLine(string.Format("   used {0}", useStm.Instruction));
						useStm.Instruction.Accept(this);
						if (trace.TraceInfo) Debug.WriteLine(string.Format("    now {0}", useStm.Instruction));
					}
				}
			}
		}

        private void TraverseCopyChain(SsaIdentifier sidGrfOrig, SsaIdentifier sidGrf, Statement stm)
        {
            if (!IsCopyWithOptionalCast(sidGrf.Identifier, stm))
            {
		        IdentifierReplacer ir = new IdentifierReplacer(sidGrf.Identifier, sidGrfOrig.Identifier);
		        stm.Instruction.Accept(ir);
            }
            else
            {
                Assignment ass = (Assignment)stm.Instruction;
                foreach (Statement use in ssaIds[ass.Dst].Uses)
                {
                    TraverseCopyChain(sidGrfOrig, ssaIds[ass.Dst], use);
                }
            }
        }

        private bool IsCopyWithOptionalCast(Identifier grf, Statement stm)
        {
            Assignment ass = stm.Instruction as Assignment;
            if (ass == null)
                return false;
            Expression e = ass.Src;
            Cast cast = ass.Src as Cast;
            if (cast != null)
                e = cast.Expression;
            return (e == grf);
        }

		private BinaryExpression CmpExpressionToZero(Expression e)
		{
			return new BinaryExpression(Operator.sub, e.DataType, e, new Constant(e.DataType, 0));
		}

		public Expression UseGrfConditionally(SsaIdentifier sid, ConditionCode cc)
		{
			GrfDefinitionFinder gf = new GrfDefinitionFinder(ssaIds);
			gf.FindDefiningExpression(sid);
			
			Expression e = gf.DefiningExpression;
			if (e == null)
			{
				return sid.Identifier;
			}
			BinaryExpression binDef = e as BinaryExpression;
			if (binDef != null)
			{
				if (gf.IsNegated)
					e = new UnaryExpression(Operator.not, PrimitiveType.Bool, e);
				return e;
			}
			ConditionOf cof = e as ConditionOf;
			if (cof != null)
			{
				binDef = cof.Expression as BinaryExpression;
				if (binDef != null)
					return ComparisonFromConditionCode(cc, binDef, gf.IsNegated);
				else
					return ComparisonFromConditionCode(cc, CmpExpressionToZero(cof.Expression), gf.IsNegated);
			}
			Application app = e as Application;
			if (app != null)
			{
				return sid.Identifier;
			}
			PhiFunction phi = e as PhiFunction;
			if (phi != null)
			{
				return sid.Identifier;
			}
			throw new NotImplementedException("NYI: e: " + e.ToString());
		}

		public override Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			BinaryExpression binUse = a.Src as BinaryExpression;
			if (binUse == null)
				return a;
			if (!IsAddOrSub(binUse.op))
				return a;
			Expression u = binUse.Right;
			Cast c = null;
			if (u != sidGrf.Identifier)
			{
				c = binUse.Right as Cast;
				if (c != null)
					u = c.Expression;
			}
			if (u != sidGrf.Identifier)
				return a;

			u = UseGrfConditionally(sidGrf, ConditionCode.ULT);
			if (c != null)
				c.Expression = u;
			else
				binUse.Right = u;
			sidGrf.Uses.Remove(useStm);
			Use(u, useStm);
			return a;
		}

		public override Expression TransformTestCondition(TestCondition tc)
		{
			SsaIdentifier sid = ssaIds[(Identifier) tc.Expression];
		
			sid.Uses.Remove(useStm);
			Expression c = UseGrfConditionally(sid, tc.ConditionCode);
			Use(c, useStm);
			return c;
		}

		private static bool IsAddOrSub(BinaryOperator op)
		{
			return op == Operator.add || op == Operator.sub;
		}

		public Expression ComparisonFromConditionCode(ConditionCode cc, BinaryExpression bin, bool isNegated)
		{
			BinaryOperator cmpOp = null;
			if (isNegated)
			{
				cc = Negate(cc);
			}
			PrimitiveType p = bin.DataType as PrimitiveType;
			bool isReal = (p != null && p.Domain == Domain.Real);
			switch (cc)
			{
			case ConditionCode.UGT: cmpOp = Operator.ugt; break;
			case ConditionCode.UGE: cmpOp = Operator.uge; break;
			case ConditionCode.ULE: cmpOp = Operator.ule; break;
			case ConditionCode.ULT: cmpOp = Operator.ult; break;
			case ConditionCode.GT:  cmpOp = isReal ? Operator.rgt : Operator.gt; break;
			case ConditionCode.GE:  cmpOp = isReal ? Operator.rge : Operator.ge; break;
			case ConditionCode.LE:  cmpOp = isReal ? Operator.rle : Operator.le; break;
			case ConditionCode.LT:  cmpOp = isReal ? Operator.rlt : Operator.lt; break;
			case ConditionCode.NE:  cmpOp = Operator.ne; break;
			case ConditionCode.EQ:  cmpOp = Operator.eq; break;
			case ConditionCode.SG:  cmpOp = Operator.lt; break;
			case ConditionCode.NS:  cmpOp = Operator.ge; break;
			case ConditionCode.OV:  
				return ComparisonFromOverflow(bin, isNegated);
			case ConditionCode.NO:
				return ComparisonFromOverflow(bin, !isNegated);
			default: throw new NotImplementedException(string.Format("Case {0} not handled.", cc));
			}

			Expression e;
			if (bin.op == Operator.sub)
			{
				e = new BinaryExpression(cmpOp, PrimitiveType.Bool, bin.Left, bin.Right);
			}
			else
			{
				e = new BinaryExpression(cmpOp, PrimitiveType.Bool, bin, Constant.Zero(bin.Left.DataType));
			}		
			return e;
		}

		public Expression ComparisonFromOverflow(BinaryExpression bin, bool isNegated)
		{
			Expression e = new Application(new ProcedureConstant(arch.PointerType, new PseudoProcedure("OVERFLOW", PrimitiveType.Bool, 1)),
				PrimitiveType.Bool, bin);
			if (isNegated)
			{
				e = new UnaryExpression(Operator.not, PrimitiveType.Bool, e);
			}
			return e;
		}
		
		public static ConditionCode Negate(ConditionCode cc)
		{
			switch (cc)
			{
			case ConditionCode.UGT: return ConditionCode.ULE;
			case ConditionCode.ULE: return ConditionCode.UGT;
			case ConditionCode.ULT: return ConditionCode.UGE;
			case ConditionCode.GT:  return ConditionCode.LE; 
			case ConditionCode.GE:  return ConditionCode.LT; 
			case ConditionCode.LT:  return ConditionCode.GE; 
			case ConditionCode.LE:  return ConditionCode.GT; 
			case ConditionCode.UGE: return ConditionCode.ULT;
			case ConditionCode.NE:  return ConditionCode.EQ; 
			case ConditionCode.EQ:  return ConditionCode.NE; 
			case ConditionCode.SG:  return ConditionCode.GE; 
			case ConditionCode.NS:  return ConditionCode.LT; 
			default: throw new ArgumentException(string.Format("Don't know how to negate ConditionCode.{0}.",  cc), "cc");
			}

		}

		private void Use(Expression expr, Statement stm)
		{
			ExpressionUseAdder eua = new ExpressionUseAdder(stm, ssaIds);
			expr.Accept(eua);
		}
	}
}
