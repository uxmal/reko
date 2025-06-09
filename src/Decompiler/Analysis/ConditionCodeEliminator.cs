#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis;

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
public class ConditionCodeEliminator : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new TraceSwitch("CcodeEliminator", "Traces the progress of the condition code eliminator")
    {
        Level = TraceLevel.Warning,
    };

    private readonly AnalysisContext context;

    public ConditionCodeEliminator(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "cce";

    public string Description => "Elimination of condition codes";



    public (SsaState, bool) Transform(SsaState ssa)
    {
        var w = CreateWorker(ssa);
        w.Transform();
        return (ssa, w.Changed);
    }

    public Worker CreateWorker(SsaState ssa)
    {
        return new Worker(context.Program, ssa, context.EventListener);
    }

    public class Worker : InstructionTransformer
    {
        private readonly SsaState ssa;
        private readonly SsaMutator mutator;
        private readonly SsaIdentifierCollection ssaIds;
        private readonly IReadOnlyProgram program;
        private readonly IEventListener listener;
        private readonly ExpressionEmitter m;
        private readonly HashSet<Identifier> aliases;
        private readonly Dictionary<(Identifier, ConditionCode), SsaIdentifier> generatedIds;
        private SsaIdentifier sidGrf = default!;
        private Statement useStm = default!;

        public Worker(IReadOnlyProgram program, SsaState ssa, IEventListener listener)
        {
            this.ssa = ssa;
            this.mutator = new SsaMutator(ssa);
            this.ssaIds = ssa.Identifiers;
            this.program = program;
            this.listener = listener;
            this.m = new ExpressionEmitter();
            this.aliases = new HashSet<Identifier>();
            this.generatedIds = new Dictionary<(Identifier, ConditionCode), SsaIdentifier>();
        }

        public bool Changed { get; private set; }

        public void Transform()
        {
            foreach (var s in ssaIds.ToList())
            {
                sidGrf = s;
                if (!IsLocallyDefinedFlagGroup(sidGrf))
                    continue;
                if (sidGrf.DefStatement.Instruction is AliasAssignment)
                    continue;
                var uses = new HashSet<Statement>();
                this.aliases.Clear();
                ClosureOfUsingStatements(sidGrf, uses, aliases);
                trace.Inform("CCE: Tracing {0}", sidGrf.DefStatement.Instruction);
                foreach (var u in uses)
                {
                    try
                    {
                        useStm = u;
                        trace.Inform("CCE:   used {0}", useStm.Instruction);
                        useStm.Instruction.Accept(this);
                        trace.Inform("CCE:    now {0}", useStm.Instruction);
                    }
                    catch (Exception ex)
                    {
                        var loc = listener.CreateStatementNavigator(program, u);
                        listener.Error(loc, ex, "An error occurred while eliminating condition codes in procedure {0}.", ssa.Procedure.Name);
                    }
                }
            }
        }

        public HashSet<SsaIdentifier> ClosureOfReachingDefinitions(SsaIdentifier sidUse)
        {
            var defs = new HashSet<SsaIdentifier>();
            var wl = new WorkList<SsaIdentifier>();
            var visited = new HashSet<Statement>();
            wl.Add(sidUse);
            while (wl.TryGetWorkItem(out var sid))
            {
                var def = sid.DefStatement;
                if (!visited.Add(def))
                    continue;
                switch (def.Instruction)
                {
                case Assignment ass:
                    switch (ass.Src)
                    {
                    case Identifier idSrc:
                        wl.Add(ssaIds[idSrc]);
                        break;
                    case BinaryExpression bin:
                        var idSliced = FindSlicedFlagRegister(bin);
                        if (idSliced is not null)
                        {
                            wl.Add(ssaIds[idSliced]);
                            break;
                        }
                        goto default;
                    case UnaryExpression unary:
                        idSliced = FindSlicedFlagRegister(unary.Expression);
                        if (idSliced is not null && unary.Operator == BinaryOperator.Not)
                        {
                            wl.Add(ssaIds[idSliced]);
                            break;
                        }
                        goto default;
                    default:
                        defs.Add(ssaIds[ass.Dst]);
                        break;
                    }
                    break;
                case PhiAssignment phi:
                    wl.AddRange(
                    phi.Src.Arguments.Select(a => ssaIds[(Identifier) a.Value]));
                    break;
                }
            }
            return defs;
        }

        /// <summary>
        /// Computes the closure of a web of using statements. The <paramref name="uses"/> hash set 
        /// will contain all non-trivial uses of the expression.
        /// </summary>
        /// <param name="sid">The SSA identifier whose use-closure we're calculating</param>
        /// <param name="uses">Uses we've seen so far.</param>
        /// <param name="aliases">Aliases of sid we've seen so far.</param>
        /// <returns>
        /// A set of all statements which directly or indirectly use the variable <paramref name="sid"/>.
        /// /returns>
        public HashSet<Statement> ClosureOfUsingStatements(
            SsaIdentifier sid,
            HashSet<Statement> uses,
            HashSet<Identifier> aliases)
        {
            foreach (var use in sid.Uses)
            {
                if (uses.Contains(use))
                    continue;
                uses.Add(use);
                if (IsCopyWithOptionalCast(sid.Identifier, use))
                {
                    // Bypass copies (C_4 = C_3) and slices
                    // (C_4 = SZC_3 & 1)
                    var ass = (Assignment) use.Instruction;
                    var sidAlias = ssaIds[ass.Dst];
                    aliases.Add(sidAlias.Identifier);
                    ClosureOfUsingStatements(sidAlias, uses, aliases);
                }
                if (use.Instruction is PhiAssignment phiAss)
                {
                    // Bypass PHI nodes.
                    var sidPhi = ssaIds[phiAss.Dst];
                    aliases.Add(sidPhi.Identifier);
                    ClosureOfUsingStatements(sidPhi, uses, aliases);
                }
            }
            return uses;
        }

        private static bool IsLocallyDefinedFlagGroup(SsaIdentifier sid)
        {
            if (sid.DefStatement is null)
                return false;
            var stg = sid.OriginalIdentifier.Storage;
            if (stg is FlagGroupStorage)
                return true;
            return (sid.GetDefiningExpression() is ConditionOf);
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
        private static bool IsCopyWithOptionalCast(Identifier grf, Statement stm)
        {
            if (stm.Instruction is not Assignment ass)
                return false;
            return ass.Src switch
            {
                Identifier id => grf == id,
                Conversion conv => grf == conv.Expression,
                Cast cast => grf == cast.Expression,
                Slice s => grf == s.Expression,
                BinaryExpression bin when bin.Operator.Type == OperatorType.And => bin.Left == grf || bin.Right == grf,
                BinaryExpression bin when bin.Operator.Type == OperatorType.Or => bin.Left == grf || bin.Right == grf,
                _ => false,
            };
        }

        private BinaryExpression CmpExpressionToZero(Expression e)
        {
            return m.ISub(e, 0);
        }

        public Expression UseGrfConditionally(
            SsaIdentifier sid,
            ConditionCode cc,
            FlagGroupStorage? testedFlags,
            bool forceIdentifier)
        {
            var gf = new GrfDefinitionFinder(ssaIds);
            gf.FindDefiningExpression(sid);

            Expression? e = gf.DefiningExpression;
            if (e is null)
            {
                return sid.Identifier;
            }

            while (e is not null)
            {
                switch (e)
                {
                case BinaryExpression bin:
                    // The following hack for Or deals with expressions like
                    // (SCZ|O) where O is always false.
                    if (bin.Operator.Type == OperatorType.Or)
                    {
                        if (IsZero(bin.Left) && bin.Right is Identifier idRight)
                        {
                            e = ssa.Identifiers[idRight].GetDefiningExpression();
                            continue;
                        }
                        else if (IsZero(bin.Right) && bin.Left is Identifier idLeft)
                        {
                            e = ssa.Identifiers[idLeft].GetDefiningExpression();
                            continue;
                        }
                    }
                    if (sid.Identifier.Storage.Equals(ssa.Procedure.Architecture.CarryFlag))
                    {
                        if (cc == ConditionCode.UGE)
                            e = e.Invert();
                    }
                    if (gf.IsNegated)
                        e = e.Invert();

                    if (forceIdentifier)
                    {
                        e = InsertNewAssignment(e, cc, sid);
                    }
                    this.Changed = true;
                    return e;
                case ConditionOf cof:
                    if (cof.Expression is not BinaryExpression condBinDef)
                        condBinDef = CmpExpressionToZero(cof.Expression);
                    Expression newCond = ComparisonFromConditionCode(cc, condBinDef, gf.IsNegated);
                    if (forceIdentifier)
                    {
                        newCond = InsertNewAssignment(newCond, cc, sid);
                    }
                    this.Changed = true;
                    return newCond;
                case Application _:
                    this.Changed = true;
                    return UseDirectly(sid, cc, testedFlags, forceIdentifier, gf.IsNegated);
                case MemoryAccess _:
                    this.Changed = true;
                    return UseDirectly(sid, cc, testedFlags, forceIdentifier, gf.IsNegated);
                case PhiFunction phi:
                    this.Changed = true;
                    return InsertNewPhi(sid, cc, phi);
                case Identifier _:
                    this.Changed = true;
                    return UseDirectly(sid, cc, testedFlags, forceIdentifier, gf.IsNegated);
                default:
                    throw new NotImplementedException("NYI: e: " + e.ToString());
                }
            }
            return sid.Identifier;
        }

        private Expression UseDirectly(
            SsaIdentifier sid,
            ConditionCode cc,
            FlagGroupStorage? testedFlags,
            bool forceIdentifier,
            bool isNegated)
        {
            Expression e = sid.Identifier;
            if (testedFlags is not null && !Bits.IsSingleBitSet(testedFlags.FlagGroupBits))
            {
                e = m.And(e, testedFlags.FlagGroupBits);
            }
            if (isNegated)
            {
                e = m.Not(e);
            }
            if (forceIdentifier)
            {
                e = InsertNewAssignment(e, cc, sid);
            }
            return e;
        }

        private bool IsZero(Expression expr)
        {
            return (expr is Identifier id &&
                ssa.Identifiers[id].DefStatement?.Instruction is Assignment ass &&
                ass.Src is Constant c &&
                c.IsZero);
        }

        private Identifier InsertNewPhi(SsaIdentifier sidDef, ConditionCode cc, PhiFunction phi)
        {
            if (this.generatedIds.TryGetValue((sidDef.Identifier, cc), out var existingId))
                return existingId.Identifier;

            // Create a dummy PHI node to stop recursion.
            var idNew = ssa.Procedure.Frame.CreateTemporary(new UnknownType());
            var tmpPhi = new PhiAssignment(idNew);
            var stmPhi = mutator.InsertStatementAfter(tmpPhi, sidDef.DefStatement);
            var sidPhi = ssaIds.Add(idNew, stmPhi, false);
            tmpPhi.Dst = sidPhi.Identifier;
            generatedIds.Add((sidDef.Identifier, cc), sidPhi);

            // Chase all the arguments of the PHI function to obtain their
            // respective variables.
            var newArgs = new List<PhiArgument>();
            foreach (var arg in phi.Arguments)
            {
                var sidArg = ssaIds[(Identifier) arg.Value];
                var id = (Identifier) UseGrfConditionally(sidArg, cc, null, true);
                newArgs.Add(new PhiArgument(arg.Block, id));
            }

            var phiNew = new PhiAssignment(sidPhi.Identifier, newArgs.ToArray());
            stmPhi.Instruction = phiNew;
            sidPhi.Identifier.DataType = newArgs[0].Value.DataType;
            Use(phiNew.Src, stmPhi);
            return sidPhi.Identifier;
        }

        private Expression InsertNewAssignment(Expression newCond, ConditionCode cc, SsaIdentifier sid)
        {
            if (this.generatedIds.TryGetValue((sid.Identifier, cc), out var existingId))
                return existingId.Identifier;
            var idNew = ssa.Procedure.Frame.CreateTemporary(newCond.DataType);
            var ass = new Assignment(idNew, newCond);
            var stm = mutator.InsertStatementAfter(ass, sid.DefStatement);
            var sidNew = ssaIds.Add(idNew, stm, false);
            ass.Dst = sidNew.Identifier;
            Use(newCond, stm);
            generatedIds.Add((sid.Identifier, cc), sidNew);
            return sidNew.Identifier;
        }

        public override Instruction TransformAssignment(Assignment a)
        {
            a.Src = a.Src.Accept(this);
            switch (a.Src)
            {
            case BinaryExpression binUse when binUse.Operator.Type.IsAddOrSub():
                return TransformAddOrSub(a, binUse);
            case Application app when app.Procedure is ProcedureConstant pc:
                if (pc.Procedure is IntrinsicProcedure pseudo)
                {
                    if (pseudo.Name == CommonOps.RorC.Name)
                    {
                        return TransformRorC(app, a);
                    }
                    else if (pseudo.Name == CommonOps.RolC.Name)
                    {
                        return TransformRolC(app, a);
                    }
                }
                return a;
            case Conversion conv when conv.Expression == this.sidGrf.Identifier:
                if (conv.SourceDataType == PrimitiveType.Bool)
                {
                    if (sidGrf.GetDefiningExpression() is not ConditionOf cof)
                        return a;
                    if (cof.Expression is not Identifier id)
                        return a;
                    if (ssaIds[id].GetDefiningExpression() is BinaryExpression bin &&
                        bin.Operator.Type == OperatorType.Shr &&
                        bin.Right is Constant shift &&
                        shift.ToInt32() == 1)
                    {
                        ssa.RemoveUses(useStm);
                        a.Src = m.Conditional(
                            conv.DataType,
                            m.Ne0(m.And(bin.Left, 1)),
                            Constant.Create(conv.DataType, 1),
                            Constant.Create(conv.DataType, 0));
                        Use(a.Src, useStm);
                    }
                }
                return a;
            }
            return a;
        }

        private Instruction TransformAddOrSub(Assignment a, BinaryExpression addSub)
        {
            Expression u = addSub.Right;
            Slice? slice = null;
            BinaryExpression? binMask = null;
            if (u != sidGrf.Identifier && !this.aliases.Contains(u))
            {
                slice = addSub.Right as Slice;
                if (slice is not null)
                    u = slice.Expression;
                else
                {
                    binMask = addSub.Right as BinaryExpression;
                    if (binMask is not null &&
                        binMask.Operator.Type == OperatorType.And &&
                        binMask.Right is Constant)
                    {
                        u = binMask.Left;
                    }
                }
            }
            if (u != sidGrf.Identifier && !this.aliases.Contains(u))
                return a;

            var oldSid = ssaIds[(Identifier) u];
            u = UseGrfConditionally(sidGrf, ConditionCode.ULT, null, false);
            if (slice is not null)
            {
                addSub.Right = new Conversion(u, u.DataType, slice.DataType);
            }
            else if (addSub.Left.DataType.BitSize > u.DataType.BitSize)
            {
                var conv = new Conversion(u, u.DataType, PrimitiveType.CreateWord(addSub.Left.DataType.BitSize));
                addSub.Right = conv;
            }
            else
                addSub.Right = u;
            oldSid.Uses.Remove(useStm!);
            Use(u, useStm!);
            return a;
        }

        /// <summary>
        /// Transform a (Shl a,1; Rolc b,1) pair to shl SEQ(b,a),1
        /// </summary>
        private Instruction TransformRolC(Application rolc, Assignment assRolc)
        {
            // 1. a_2 = a_1 << 1
            // 2. C_2 = cond(a_2)
            // 3. b_2 = b_1 rolc 1, C_2
            // 4. flags_3 = cond(b_2)

            // *.  tmp_3 = (b_1:a_1) << 1
            // 1'. a_2 = slice(tmp3,16)
            // 2.  C_2 = cond(a_2)
            // 2'. b_2 = slice(tmp3,0)
            // 4.  flags_3 = cond(b_2)

            var sidOrigHi = ssaIds[assRolc.Dst];

            // Go 'backwards' through aliasing slices until instruction
            // defining the carry flag is found.
            var carryIn = rolc.Arguments[2];
            if (carryIn is not Identifier idCarry)
            {
                var idCarrySliced = FindSlicedFlagRegister(carryIn);
                if (idCarrySliced is null)
                    return assRolc;
                idCarry = idCarrySliced;
            }

            var sidCarryUsedInRolc = ssaIds[idCarry];
            var defStatements = ClosureOfReachingDefinitions(sidCarryUsedInRolc);
            if (defStatements.Count != 1)
                return assRolc;
            var sidCarry = defStatements.First();
            if (sidCarry.GetDefiningExpression() is not ConditionOf cond)
                return assRolc;
            if (cond.Expression is not Identifier condId)
                return assRolc;
            var sidOrigLo = ssaIds[condId];
            if (sidOrigLo.GetDefiningExpression() is not BinaryExpression shift || shift.Operator.Type != OperatorType.Shl)
                return assRolc;

            var expShlSrc = shift.Left;
            var expRorSrc = rolc.Arguments[0];

            // Discard the use of the carry that tied the SHL and ROLC together.
            sidCarryUsedInRolc.Uses.Remove(sidOrigHi.DefStatement);

            var tmpLo = ssa.Procedure.Frame.CreateTemporary(expShlSrc.DataType);
            var sidTmpLo = mutator.InsertAssignmentBefore(tmpLo, expShlSrc, sidOrigLo.DefStatement);

            var tmpHi = ssa.Procedure.Frame.CreateTemporary(rolc.Arguments[0].DataType);
            var sidTmpHi = mutator.InsertAssignmentAfter(tmpHi, rolc.Arguments[0], sidTmpLo.DefStatement);

            var dt = PrimitiveType.Create(Domain.Integer, expShlSrc.DataType.BitSize + expRorSrc.DataType.BitSize);
            var tmp = ssa.Procedure.Frame.CreateTemporary(dt);
            var expMkLongword = m.Shl(m.Seq(sidTmpHi.Identifier, sidTmpLo.Identifier), 1);
            var sidTmp = mutator.InsertAssignmentAfter(tmp, expMkLongword, sidTmpHi.DefStatement);

            ssa.RemoveUses(sidOrigLo.DefStatement);
            var expNewLo = m.Slice(
                sidTmp.Identifier,
                PrimitiveType.CreateWord(tmpHi.DataType.BitSize));
            sidOrigLo.DefStatement!.Instruction = new Assignment(sidOrigLo.Identifier, expNewLo);
            sidTmp.Uses.Add(sidOrigLo.DefStatement!);

            ssa.RemoveUses(sidOrigHi.DefStatement);
            var expNewHi = m.Slice(
                sidTmp.Identifier,
                PrimitiveType.CreateWord(tmpLo.DataType.BitSize),
                tmpHi.DataType.BitSize);
            sidOrigHi.DefStatement.Instruction = new Assignment(sidOrigHi.Identifier, expNewHi);
            sidTmp.Uses.Add(sidOrigHi.DefStatement);
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
        // 2'. b_2 = slice(tmp3,0)
        // 4.  flags_3 = cond(tmp_3)

        private Instruction TransformRorC(Application rorc, Assignment a)
        {
            var sidOrigLo = ssaIds[a.Dst];

            // Go 'backwards' through aliasing slices until instruction
            // defining the carry flag is found.
            var carryIn = rorc.Arguments[2];
            if (carryIn is not Identifier idCarry)
            {
                var idCarrySliced  = FindSlicedFlagRegister(carryIn);
                if (idCarrySliced is null)
                    return a;
                idCarry = idCarrySliced;
            }
            var sidCarry = ssaIds[idCarry];
            var sidsToKill = new HashSet<SsaIdentifier> { sidCarry };
            while (sidCarry.GetDefiningExpression() is BinaryExpression bin)
            {
                var idSliced = FindSlicedFlagRegister(bin);
                if (idSliced is null)
                {
                    return a;
                }
                sidCarry = ssaIds[idSliced];
                if (sidCarry.Uses.Count < 2)
                {
                    sidsToKill.Add(sidCarry);
                }
            }
            if (sidCarry.GetDefiningExpression() is not ConditionOf cond)
            {
                if (sidCarry.GetDefiningExpression() is not Identifier idTmp)
                    return a;
                var sidT = ssaIds[idTmp];
                if (sidT.GetDefiningExpression() is not ConditionOf cond2)
                    return a;
                cond = cond2;
            }
            if (cond.Expression is not Identifier condId)
                return a;
            var sidOrigHi = ssaIds[condId];
            if (sidOrigHi.GetDefiningExpression() is Slice slice2)
            {
                if (slice2.Expression is not Identifier id)
                    return a;
                sidOrigHi = ssaIds[id];
            }
            if (sidOrigHi.GetDefiningExpression() is not BinaryExpression shift)
                return a;
            Domain domain;
            if (shift.Operator.Type == OperatorType.Shr)
                domain = Domain.UnsignedInt;
            else if (shift.Operator.Type == OperatorType.Sar)
                domain = Domain.SignedInt;
            else
                return a;

            var expShrSrc = shift.Left;
            var expRorSrc = rorc.Arguments[0];

            // inject a 'tmp = SEQ(hi, lo) >> 1' statement
            var ssam = new SsaMutator(ssa);
            var dt = PrimitiveType.Create(domain, expShrSrc.DataType.BitSize + expRorSrc.DataType.BitSize);
            var tmp = ssa.Procedure.Frame.CreateTemporary(dt);
            var sidTmp = ssam.InsertAssignmentBefore(
                tmp,
                m.Shr(m.Seq(expShrSrc, expRorSrc), shift.Right),
                sidOrigHi.DefStatement);

            // Replace the 'hi = SHR(...)' with 'hi = SLICE(...)'
            var expHi = m.Slice(sidTmp.Identifier, sidOrigHi.Identifier.DataType, expRorSrc.DataType.BitSize);
            ssam.ReplaceAssigment(sidOrigHi, new Assignment(sidOrigHi.Identifier, expHi));

            // Replace the 'lo = SHR(...)' with 'lo = SLICE(...)'
            var expLo = m.Slice(sidTmp.Identifier, sidOrigLo.Identifier.DataType, 0);
            ssam.ReplaceAssigment(sidOrigLo, new Assignment(sidOrigLo.Identifier, expLo));
            return sidOrigLo.DefStatement.Instruction;
        }

        public override Expression VisitConversion(Conversion conversion)
        {
            var c = conversion.Expression.Accept(this);
            if (c is Identifier id && id.Storage is FlagGroupStorage grf)
            {
                if (conversion.SourceDataType == PrimitiveType.Bool)
                {
                    var e = m.Ne0(c);
                    return e;
                }
            }
            return m.Convert(c, conversion.SourceDataType, conversion.DataType);
        }

        public override Expression VisitTestCondition(TestCondition tc)
        {
            var id = (Identifier) tc.Expression;
            SsaIdentifier sid = ssaIds[id];
            sid.Uses.Remove(useStm!);
            var grf = id.Storage as FlagGroupStorage;
            Expression c = UseGrfConditionally(sid, tc.ConditionCode, grf, false);
            if (c.DataType.Domain != Domain.Boolean)
            {
                if (grf is null || !Bits.IsSingleBitSet(grf.FlagGroupBits))
                    c = m.Ne0(c);
            }
            Use(c, useStm!);
            return c;
        }

		public Expression ComparisonFromConditionCode(ConditionCode cc, BinaryExpression bin, bool isNegated)
		{
            BinaryOperator? cmpOp;
			if (isNegated)
			{
				cc = cc.Invert();
			}
            bool isReal = bin.DataType.Domain == Domain.Real;
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
            case ConditionCode.IS_NAN:
                return OrderedComparison(bin,false);
            case ConditionCode.NOT_NAN:
                return OrderedComparison(bin, true);

            case ConditionCode.PE:
                return ComparisonFromParity(bin, isNegated);
            case ConditionCode.PO:
                return ComparisonFromParity(bin, !isNegated);
            default: throw new NotImplementedException(string.Format("Case {0} not handled.", cc));
			}

			Expression e;
            if (bin.Operator.Type == OperatorType.ISub || bin.Operator.Type == OperatorType.FSub)
            {
                e = m.Bin(cmpOp, PrimitiveType.Bool, bin.Left, bin.Right);
            }
            else
            {
                var dt = bin.Left.DataType;
                var ptr = dt.ResolveAs<Pointer>();
                Expression zero;
                if (ptr is not null)
                {
                    //$REVIEW: assumes a null pointer has bit pattern 0000...00.
                    zero = Address.Create(ptr, 0);
                }
                else
                {
                    var pt = bin.Left.DataType.ResolveAs<PrimitiveType>();
                    zero = Constant.Zero(pt!);
                }
                e = m.Bin(cmpOp, PrimitiveType.Bool, bin, zero);
            }
			return e;
		}

		public Expression ComparisonFromOverflow(BinaryExpression bin, bool isNegated)
		{
            var intrinsic = CommonOps.Overflow.MakeInstance(bin.DataType);
            Expression e = new Application(
                new ProcedureConstant(program.Platform.PointerType, intrinsic),
                PrimitiveType.Bool,
                bin);
			if (isNegated)
			{
				e = m.Not(e);
			}
			return e;
		}

        public Expression ComparisonFromParity(BinaryExpression bin, bool isNegated)
        {
            var sig = new FunctionType(
                [ new Identifier("", bin.DataType, null!) ],
                [ new Identifier("", PrimitiveType.Bool, null!) ]);
            Expression e = new Application(
                new ProcedureConstant(
                    program.Platform.PointerType,
                    new IntrinsicProcedure("PARITY_EVEN", false, sig)),
                PrimitiveType.Bool,
                bin);
            if (isNegated)
            {
                e = m.Not(e);
            }
            return e;
        }

        /// <summary>
        /// Generate a comparison using the standard C
        /// "isunordered" function. 
        /// </summary>
        public Expression OrderedComparison(BinaryExpression bin, bool isNegated)
		{
            var sig = new FunctionType(
                [
                    new Identifier("x", bin.DataType, null!),
                    new Identifier("y", bin.DataType, null!)
                ],
                [
                    new Identifier("", PrimitiveType.Bool, null!)
                ]);
            Expression e = m.Fn(
                new ProcedureConstant(
                    program.Platform.PointerType,
                    new IntrinsicProcedure("isunordered", false, sig)),
                bin.Left,
                bin.Right);
            if (isNegated)
			{
                e = m.Not(e);
			}
            return e;
		}

        private void Use(Expression expr, Statement stm)
        {
            var eua = new InstructionUseAdder(stm, ssaIds);
            expr.Accept(eua);
        }
    }

    /// <summary>
    /// Find a "sliced" flag group register in the given expression.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    /// <remarks>
    /// After SSA analysis, accesses to the flag registers are often
    /// aliased by instructions of the type:<code>
    /// single_bit = flag_group & constant_mask.
    /// </code>
    /// This method detects such "slices".
    /// </remarks>
    public static Identifier? FindSlicedFlagRegister(BinaryExpression bin)
    {
        if (bin.Operator == Operator.And &&
            bin.Left is Identifier { Storage: FlagGroupStorage } id &&
            bin.Right is Constant)
        {
            return id;
        }
        return null;
    }

    /// <summary>
    /// Find a "sliced" flag group register in the given expression.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    /// <remarks>
    /// After SSA analysis, accesses to the flag registers are often
    /// aliased by instructions of the type:<code>
    /// single_bit = flag_group & constant_mask.
    /// </code>
    /// This method detects such "slices".
    /// </remarks>
    public static Identifier? FindSlicedFlagRegister(Expression? e)
    {
        if (e is BinaryExpression bin)
            return FindSlicedFlagRegister(bin);
        return null;
    }
}
