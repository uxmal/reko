using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    public class RegisterPreservation_2
    {
        private static TraceSwitch trace = new TraceSwitch("RegisterPreservation", "");

        private Procedure proc;
        private Dictionary<Expression, Expression> provenTrue;
        private SsaState ssa;
        internal ExpressionSimplifier simp;
        internal ISet<Procedure> cycleGrp;
        internal Dictionary<Procedure, Dictionary<Expression, Expression>> recurPremises;
        private ExpressionValueComparer cmp;
        private bool result;
        private HashSet<PhiAssignment> lastPhis;
        private Dictionary<PhiAssignment, Expression> cache;

        public RegisterPreservation_2(Procedure proc, SsaState state, ISet<Procedure> cycle)
        {
            this.proc = proc;
            this.ssa = state;
            this.cmp = new ExpressionValueComparer();
            this.recurPremises = new Dictionary<Procedure, Dictionary<Expression, Expression>>();
            this.lastPhis = new HashSet<PhiAssignment>();
            this.cache = new Dictionary<PhiAssignment, Expression>();

            this.cycleGrp = cycle;
        }

        public void findPreserveds()
        {
            var removes = new HashSet<Expression>();

            DebugEx.PrintIf(trace.TraceInfo, "Finding preserved registers for {0}", proc.Name);

            // prove preservation for all uses in the exit block
            var uses = proc.ExitBlock.Statements.Select(s => (UseInstruction)s.Instruction);
            foreach (var use in uses)
            {
                var lhs = (Identifier)use.Expression;
                Expression equation = new BinaryExpression(Operator.Eq, PrimitiveType.Bool, lhs, lhs);
                DebugEx.PrintIf(trace.TraceInfo, "attempting to prove " + equation + " is preserved by " + proc.Name);
                if (prove(equation, false))
                {
                    removes.Add(equation);
                }
            }

            DumpPreservedRegisters();

            // Remove the preserved locations from the modifieds and the returns
            foreach (var pp in provenTrue)
            {
                Expression lhs = pp.Key;
                Expression rhs = pp.Value;
                // Has to be of the form loc = loc, not say loc+4, otherwise the bypass logic won't see the add of 4
                if (!cmp.Equals(lhs, rhs))
                    continue;

                //theReturnStatement.removeModified(lhs);
            }
        }

        private void DumpPreservedRegisters()
        {
            if (trace.TraceVerbose)
            {
                Debug.Print("### proven true for procedure {0}:", proc.Name);
                foreach (var elem in provenTrue)
                    Debug.Print("{0} = {1}", elem.Key, elem.Value);
                Debug.Print("### end proven true for procedure {0} ", proc.Name);
                Debug.WriteLine("");
            }
        }

        // this function was non-reentrant, but now reentrancy is frequently used
        /// prove any arbitary property of this procedure. If conditional is true, do not save the result, as it may
        /// be conditional on premises stored in other procedures
        public bool prove(Expression query, bool conditional /* = false */)
        {
            var bQuery = (BinaryExpression)query;
            Debug.Assert(bQuery.Operator == Operator.Eq);

            Expression queryLeft = bQuery.Left;
            Expression queryRight = bQuery.Right;
            if (provenTrue.ContainsKey(queryLeft) && cmp.Equals(provenTrue[queryLeft], queryRight))
            {
                DebugEx.PrintIf(trace.TraceVerbose, "found true in provenTrue cache {0} in {1}", query, proc.Name);
                return true;
            }
            Expression original = query.CloneExpression();
            Expression origLeft = ((BinaryExpression)original).Left;
            Expression origRight = ((BinaryExpression)original).Right;

            // subscript locs on the right with {-} (nullptr reference)
            //LocationSet locs;
            //query.Right.addUsedLocs(locs);
            //LocationSet::iterator xx;
            //for (xx = locs.begin(); xx != locs.end(); xx++)
            //{
            //    query.setSubExp2(query.Right.expSubscriptValNull(*xx));
            //}

            if (bQuery.Left is Identifier)
            {
                bool gotDef = false;
                // replace expression from return set with expression in the collector of the return
                Expression def = ssatheReturnStatement.findDefFor(bQuery.Left);
                if (def != null)
                {
                    bQuery.Left = def;
                    gotDef = true;
                }
                if (!gotDef)
                {
                    // OK, the thing I'm looking for isn't in the return collector, but perhaps there is an entry for <all>
                    // If this is proved, then it is safe to say that x == x for any x with no definition reaching the exit
                    Expression right = origRight.CloneExpression().Accept(simp); // In case it's sp+0
                    if (cmp.Equals(origLeft, right) &&                   // x == x
                        origLeft is opDefineAll &&    // Beware infinite recursion
                        prove(allEqAll))
                    {                      // Recurse in case <all> not proven yet
                        DebugEx.PrintIf(trace.TraceVerbose, "Using all=all for {0}", bQuery.Left);
                        DebugEx.PrintIf(trace.TraceVerbose, "prove returns true");
                        provenTrue[origLeft.CloneExpression()] = right;
                        return true;
                    }
                    DebugEx.PrintIf(trace.TraceVerbose, "not in return collector: {0}", bQuery.Left);
                    DebugEx.PrintIf(trace.TraceVerbose, "prove returns false");
                    return false;
                }
            }

            if (cycleGrp.Count > 1) // If in involved in a recursion cycle
                                    //    then save the original query as a premise for bypassing calls
                recurPremises[proc][origLeft] = origRight;

            bool result = prover(query, lastPhis, cache, original, null);
            if (cycleGrp.Count > 1)
                recurPremises[proc].Remove(origLeft); // Remove the premise, regardless of result
            DebugEx.PrintIf(trace.TraceVerbose, "prove returns {0} for {1} in {2}", result, query, proc.Name);

            if (!conditional)
            {
                if (result)
                    provenTrue[origLeft] = origRight; // Save the now proven equation
            }
            return result;
        }
        /// helper function, should be private
        bool prover(
            Expression _query,
            ISet<PhiAssignment> lastPhis,
            IDictionary<PhiAssignment, Expression> cache,
            Expression original,
            PhiAssignment lastPhi)
        {
            var pp = new Prover_v2(this, _query, lastPhis, cache, original, lastPhi);
            return pp.Execute();
        }
    }

    public static class DebugEx
    {
        [Conditional("DEBUG")]
        public static void PrintIf(bool condition, string format, params object[] args)
        {
            Debug.WriteLineIf(condition, string.Format(format, args));
        }
    }
}