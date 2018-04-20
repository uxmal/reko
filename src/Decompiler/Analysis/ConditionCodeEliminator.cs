#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
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
    ///	For best performance, preprocess the intermediate code with the ValuePropagator transformer.
    ///	</para>
	/// </remarks>
	public class ConditionCodeEliminator : InstructionTransformer
	{
        private SsaState ssa;
		private SsaIdentifierCollection ssaIds;
		private SsaIdentifier sidGrf;
        private HashSet<SsaIdentifier> aliases;     // aliases of sidGrf
        private Statement useStm;
        private IPlatform platform;
        private ExpressionEmitter m;

        private static TraceSwitch trace = new TraceSwitch("CcodeEliminator", "Traces the progress of the condition code eliminator");

        public ConditionCodeEliminator(SsaState ssa, IPlatform arch)
		{
            this.ssa=ssa;
			this.ssaIds = ssa.Identifiers;
            this.platform = arch;
            this.m = new ExpressionEmitter();
		}

        public void Transform()
        {
            foreach (var s in ssaIds.ToList())
            {
                sidGrf = s;
                if (!IsLocallyDefinedFlagGroup(sidGrf))
                    continue;

                var uses = new HashSet<Statement>();
                aliases = new HashSet<SsaIdentifier>();
                ClosureOfUsingStatements(sidGrf, sidGrf.DefExpression, uses, aliases);

                if (trace.TraceInfo) Debug.WriteLine(string.Format("Tracing {0}", sidGrf.DefStatement.Instruction));

                foreach (var u in uses)
                {
                    useStm = u;

                    if (trace.TraceInfo) Debug.WriteLine(string.Format("   used {0}", useStm.Instruction));
                    useStm.Instruction.Accept(this);
                    if (trace.TraceInfo) Debug.WriteLine(string.Format("    now {0}", useStm.Instruction));
                }
            }
        }

        /// <summary>
        /// Computes the close of a web of using statements. The <paramref name="uses"/> hash set 
        /// will contain all non-trivial uses of the expression.
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="expr"></param>
        /// <param name="uses"></param>
        public HashSet<Statement> ClosureOfUsingStatements(
            SsaIdentifier sid,
            Expression expr,
            HashSet<Statement> uses,
            HashSet<SsaIdentifier> aliases)
        {
            foreach (var use in sid.Uses)
            {
                if (uses.Contains(use))
                    continue;
                uses.Add(use);
                if (IsCopyWithOptionalCast(sid.Identifier, use))
                {
                    var ass = (Assignment)use.Instruction;
                    var sidAlias = ssaIds[ass.Dst];
                    aliases.Add(sidAlias);
                    ClosureOfUsingStatements(sidAlias, expr, uses, aliases);
                }
                var phiAss = use.Instruction as PhiAssignment;
                if (phiAss != null)
                {
                    var sidPhi = ssaIds[phiAss.Dst];
                    aliases.Add(sidPhi);
                    ClosureOfUsingStatements(sidPhi, expr, uses, aliases);
                }
            }
            return uses;
        }

        private bool IsLocallyDefinedFlagGroup(SsaIdentifier sid)
        {
            if (sid.DefStatement == null)
                return false;
            var stg = sid.OriginalIdentifier.Storage;
            if (stg is FlagGroupStorage)
                return true;
            return (sid.DefExpression is ConditionOf);
		}

        /// <summary>
        /// Returns true if the instruction in the statement is an assignment of the form
        ///     grf = src
        /// or
        ///     grf = (foo) src 
        /// or 
        ///     grf = SLICE(src, x, y)
        /// </summary>
        /// <param name="grf"></param>
        /// <param name="stm"></param>
        /// <returns></returns>
        private bool IsCopyWithOptionalCast(Identifier grf, Statement stm)
        {
            Assignment ass = stm.Instruction as Assignment;
            if (ass == null)
                return false;
            Expression e = ass.Src;
            if (e is Cast cast)
                return grf == cast.Expression;
            else if (e is Slice s)
                return grf == s.Expression;
            else if (e is BinaryExpression bin && bin.Operator == Operator.Or)
            {
                return bin.Left == grf || bin.Right == grf;
            }
            return false;
        }

		private BinaryExpression CmpExpressionToZero(Expression e)
		{
            return m.ISub(e, 0);
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
            switch (e)
			{
            case BinaryExpression binDef:
				if (gf.IsNegated)
					e = e.Invert();
				return e;
            case ConditionOf cof:
				var condBinDef = cof.Expression as BinaryExpression;
				if (condBinDef == null)
                    condBinDef = CmpExpressionToZero(cof.Expression);
				return ComparisonFromConditionCode(cc, condBinDef, gf.IsNegated);
            case Application app:
				return sid.Identifier;
            case PhiFunction phi:
				return sid.Identifier;
            default:
			throw new NotImplementedException("NYI: e: " + e.ToString());
		}
		}

		public override Instruction TransformAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(this);
            if (a.Src is BinaryExpression binUse && IsAddOrSub(binUse.Operator))
                return TransformAddOrSub(a, binUse);
            if (a.Src is Application app && app.Procedure is ProcedureConstant pc)
            {
                if (pc.Procedure is PseudoProcedure pseudo)
                {
                    if (pseudo.Name == PseudoProcedure.RorC)
                    {
                        return TransformRorC(app, a);
                    }
                    else if (pseudo.Name == PseudoProcedure.RolC)
                    {
                        return TransformRolC(app, a);
                    }
                }
            }
            return a;
        }

        private Instruction TransformAddOrSub(Assignment a, BinaryExpression binUse)
        {
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

            //var law = new LongAddRewriter(arch, useStm.Block.Procedure.Frame);
            //if (law.Match(u, a))
            //{
            //    InsertLongAdd(sidGrf, u, a);
            //}
            u = UseGrfConditionally(sidGrf, ConditionCode.ULT);
            if (c != null)
                c.Expression = u;
            else
                binUse.Right = u;
            sidGrf.Uses.Remove(useStm);
            Use(u, useStm);
            return a;
        }

        private Instruction TransformRolC(Application rolc, Assignment a)
        {
            var sidOrigHi = ssaIds[a.Dst];
            var sidCarry = ssaIds[(Identifier)rolc.Arguments[2]];
            var cond = sidCarry.DefExpression as ConditionOf;
            if (cond == null)
                return a;
            var condId = cond.Expression as Identifier;
            if (condId == null)
                return a;
            var sidOrigLo = ssaIds[condId];
            var shift = sidOrigLo.DefExpression as BinaryExpression;
            if (shift == null || shift.Operator != Operator.Shl)
                return a;

            var block = sidOrigHi.DefStatement.Block;
            var sidShift = sidOrigLo.DefStatement;
            var expShrSrc = shift.Left;
            var expRorSrc = rolc.Arguments[0];

            var stmGrf = sidGrf.DefStatement;
            block.Statements.Remove(stmGrf);

            // xx = lo
            var tmpLo = ssa.Procedure.Frame.CreateTemporary(expShrSrc.DataType);
            var sidTmpLo = ssaIds.Add(tmpLo, sidOrigLo.DefStatement, expShrSrc, false);
            sidOrigLo.DefStatement.Instruction = new Assignment(sidTmpLo.Identifier, expShrSrc);

            var tmpHi = ssa.Procedure.Frame.CreateTemporary(rolc.Arguments[0].DataType);
            var sidTmpHi = ssaIds.Add(tmpHi, sidOrigHi.DefStatement, rolc.Arguments[0], false);
            sidOrigHi.DefStatement.Instruction = new Assignment(sidTmpHi.Identifier, rolc.Arguments[0]);

            var iRolc = block.Statements.IndexOf(sidOrigHi.DefStatement);
            var dt = PrimitiveType.Create(Domain.Integer, expShrSrc.DataType.Size + expRorSrc.DataType.Size);
            var tmp = ssa.Procedure.Frame.CreateTemporary(dt);
            var expMkLongword = m.Shl(m.Seq(sidTmpHi.Identifier, sidTmpLo.Identifier), 1);
            var sidTmp = ssaIds.Add(tmp, sidGrf.DefStatement, expMkLongword, false);
            var stmTmp = block.Statements.Insert(iRolc + 1, sidOrigHi.DefStatement.LinearAddress,
                new Assignment(sidTmp.Identifier, expMkLongword));
            sidTmp.DefStatement = stmTmp;
            sidTmpLo.Uses.Add(stmTmp);
            sidTmpHi.Uses.Add(stmTmp);

            ssa.RemoveUses(sidCarry.DefStatement);
            block.Statements.Remove(sidCarry.DefStatement);
            ssaIds.Remove(sidCarry);

            var expNewLo = m.Cast(
                PrimitiveType.CreateWord(tmpHi.DataType.Size),
                sidTmp.Identifier);
            var stmNewLo = block.Statements.Insert(
                iRolc + 2,
                sidOrigLo.DefStatement.LinearAddress,
                new Assignment(sidOrigLo.Identifier, expNewLo));
            sidTmp.Uses.Add(stmNewLo);
            sidOrigLo.DefStatement = stmNewLo;
            sidOrigLo.DefExpression = expNewLo;

            var expNewHi = m.Slice(
                PrimitiveType.CreateWord(tmpLo.DataType.Size),
                sidTmp.Identifier,
                tmpHi.DataType.BitSize);
            var stmNewHi = block.Statements.Insert(
                iRolc + 3,
                sidOrigHi.DefStatement.LinearAddress,
                new Assignment(sidOrigHi.Identifier, expNewHi));
            sidTmp.Uses.Add(stmNewHi);
            sidOrigHi.DefStatement = stmNewHi;
            sidOrigHi.DefStatement = stmNewHi;

            sidGrf.DefExpression = m.Cond(sidTmp.Identifier);
            sidGrf.DefStatement.Instruction = new Assignment(
                sidGrf.Identifier, sidGrf.DefExpression);
            sidTmp.Uses.Add(sidGrf.DefStatement);

            return sidOrigHi.DefStatement.Instruction;
        }

        // 1. a_2 = a_1 >> 1
        // 2. C_2 = cond(a_2)
        // 3. b_2 = b_1 rorc 1, C
        // 4. flags_3 = cond(b_2)

        // 1.  tmp_1 = a_1
        // 3.  tmp_2 = b_2
        // *.  tmp_3 = (tmp1:tmp2) >> 1
        // 1'. a_2 = slice(tmp3,16)
        // 2'. b_2 = (cast) tmp3
        // 4.  flags_3 = cond(b_2)
        private Instruction TransformRorC(Application rorc, Assignment a)
        {
            var sidOrigLo = ssaIds[a.Dst];
            var sidCarry = ssaIds[(Identifier)rorc.Arguments[2]];
            var cond = sidCarry.DefExpression as ConditionOf;
            if (cond == null)
                return a;
            var condId = cond.Expression as Identifier;
            if (condId == null)
                return a;
            var sidOrigHi = ssaIds[condId];
            if (!(sidOrigHi.DefExpression is BinaryExpression shift &&
                  shift.Operator == Operator.Shr))
                return a;

            var block = sidOrigLo.DefStatement.Block;
            var sidShift = sidOrigHi.DefStatement;
            var expShrSrc = shift.Left;
            var expRorSrc = rorc.Arguments[0];

            var stmGrf = sidGrf.DefStatement;
            block.Statements.Remove(stmGrf);

            var tmpHi = ssa.Procedure.Frame.CreateTemporary(expShrSrc.DataType);
            var sidTmpHi = ssaIds.Add(tmpHi, sidOrigHi.DefStatement, expShrSrc, false);
            sidOrigHi.DefStatement.Instruction = new Assignment(sidTmpHi.Identifier, expShrSrc);

            var tmpLo = ssa.Procedure.Frame.CreateTemporary(rorc.Arguments[0].DataType);
            var sidTmpLo = ssaIds.Add(tmpLo, sidOrigLo.DefStatement, rorc.Arguments[0], false);
            sidOrigLo.DefStatement.Instruction = new Assignment(sidTmpLo.Identifier, rorc.Arguments[0]);

            var iRorc = block.Statements.IndexOf(sidOrigLo.DefStatement);
            var dt = PrimitiveType.Create(Domain.UnsignedInt, expShrSrc.DataType.Size + expRorSrc.DataType.Size);
            var tmp = ssa.Procedure.Frame.CreateTemporary(dt);
            var expMkLongword = m.Shr(m.Seq(sidTmpHi.Identifier, sidTmpLo.Identifier), 1);
            var sidTmp = ssaIds.Add(tmp, sidGrf.DefStatement, expMkLongword, false);
            var stmTmp = block.Statements.Insert(iRorc + 1, sidOrigLo.DefStatement.LinearAddress,
                new Assignment(sidTmp.Identifier, expMkLongword));
            sidTmp.DefStatement = stmTmp;
            sidTmpHi.Uses.Add(stmTmp);
            sidTmpLo.Uses.Add(stmTmp);

            ssa.RemoveUses(sidCarry.DefStatement);
            block.Statements.Remove(sidCarry.DefStatement);
            ssaIds.Remove(sidCarry);

            var expNewHi = m.Slice(
                PrimitiveType.CreateWord(tmpHi.DataType.Size),
                sidTmp.Identifier,
                tmpLo.DataType.BitSize);
            var stmNewHi = block.Statements.Insert(
                iRorc + 2,
                sidOrigHi.DefStatement.LinearAddress,
                new Assignment(sidOrigHi.Identifier, expNewHi));
            sidTmp.Uses.Add(stmNewHi);
            sidOrigHi.DefStatement = stmNewHi;
            sidOrigHi.DefExpression = expNewHi;

            var expNewLo = m.Cast(
                PrimitiveType.CreateWord(tmpLo.DataType.Size),
                sidTmp.Identifier);
            var stmNewLo = block.Statements.Insert(
                iRorc + 3,
                sidOrigLo.DefStatement.LinearAddress,
                new Assignment(sidOrigLo.Identifier, expNewLo));
            sidTmp.Uses.Add(stmNewLo);
            sidOrigLo.DefStatement = stmNewLo;
            sidOrigLo.DefStatement = stmNewLo;

            sidGrf.DefExpression = m.Cond(sidTmp.Identifier);
            sidGrf.DefStatement.Instruction = new Assignment(
                sidGrf.Identifier, sidGrf.DefExpression);
            sidTmp.Uses.Add(sidGrf.DefStatement);
             
            return sidOrigLo.DefStatement.Instruction;
        }

        public override Expression VisitTestCondition(TestCondition tc)
		{
			SsaIdentifier sid = ssaIds[(Identifier) tc.Expression];
			sid.Uses.Remove(useStm);
			Expression c = UseGrfConditionally(sid, tc.ConditionCode);
			Use(c, useStm);
			return c;
		}

		private static bool IsAddOrSub(Operator op)
		{
			return op == Operator.IAdd || op == Operator.ISub;
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
			case ConditionCode.UGT: cmpOp = Operator.Ugt; break;
			case ConditionCode.UGE: cmpOp = Operator.Uge; break;
			case ConditionCode.ULE: cmpOp = Operator.Ule; break;
			case ConditionCode.ULT: cmpOp = Operator.Ult; break;
			case ConditionCode.GT:  cmpOp = isReal ? Operator.Fgt : Operator.Gt; break;
			case ConditionCode.GE:  cmpOp = isReal ? Operator.Fge : Operator.Ge; break;
			case ConditionCode.LE:  cmpOp = isReal ? Operator.Fle : Operator.Le; break;
			case ConditionCode.LT:  cmpOp = isReal ? Operator.Flt : Operator.Lt; break;
			case ConditionCode.NE:  cmpOp = Operator.Ne; break;
			case ConditionCode.EQ:  cmpOp = Operator.Eq; break;
			case ConditionCode.SG:  cmpOp = Operator.Lt; break;
			case ConditionCode.NS:  cmpOp = Operator.Ge; break;
			case ConditionCode.OV:  
				return ComparisonFromOverflow(bin, isNegated);
			case ConditionCode.NO:
				return ComparisonFromOverflow(bin, !isNegated);
			default: throw new NotImplementedException(string.Format("Case {0} not handled.", cc));
			}

			Expression e;
            if (bin.Operator == Operator.ISub || bin.Operator == Operator.FSub)
            {
                e = new BinaryExpression(cmpOp, PrimitiveType.Bool, bin.Left, bin.Right);
            }
            else
            {
                var dt = bin.Left.DataType;
                if (dt is QualifiedType qt)
                {
                    dt = qt.DataType;
                }
                var ptr = dt.ResolveAs<Pointer>();
                Expression zero;
                if (ptr != null)
                {
                    //$REVIEW: assumes a null pointer has bit pattern 0000...00.
                    zero = Address.Create(ptr, 0);
                }
                else
                {
                    var pt = bin.Left.DataType.ResolveAs<PrimitiveType>();
                    zero = Constant.Zero(pt);
                }
                e = new BinaryExpression(cmpOp, PrimitiveType.Bool, bin, zero);
            }
			return e;
		}

		public Expression ComparisonFromOverflow(BinaryExpression bin, bool isNegated)
		{
			Expression e = new Application(new ProcedureConstant(platform.PointerType, new PseudoProcedure("OVERFLOW", PrimitiveType.Bool, 1)),
				PrimitiveType.Bool, bin);
			if (isNegated)
			{
				e = m.Not(e);
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
			var eua = new InstructionUseAdder(stm, ssaIds);
			expr.Accept(eua);
		}
	}
}
