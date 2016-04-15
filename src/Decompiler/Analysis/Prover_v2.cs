using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    class Prover_v2
    {
        private static TraceSwitch trace = new TraceSwitch("RegisterPreservation", "");

        private RegisterPreservation_2 rp;
        private ExpressionValueComparer cmp;

        private IDictionary<PhiAssignment, Expression> cache;
        private Dictionary<CallInstruction, Expression> called;
        private bool change;
        private PhiAssignment lastPhi;
        private ISet<PhiAssignment> lastPhis;

        private ExpressionEmitter m;
        private ExpressionSimplifier simp;

        private Expression original;
        private Expression phiInd;
        private BinaryExpression query;
        private HashSet<Instruction> refsTo;
        private bool swapped;
        private Expression _query;
        private Procedure proc;
        private SsaState ssa;

        public Prover_v2(
            RegisterPreservation_2 rp,
            Procedure proc,
            SsaState ssa,
            Expression _query,
            ISet<PhiAssignment> lastPhis,
            IDictionary<PhiAssignment, Expression> cache,
            Expression original,
            PhiAssignment lastPhi)
        {
            this.rp = rp;
            this.proc = proc;
            this.ssa = ssa;
            this.cmp = new ExpressionValueComparer();

            this._query = _query;
            this.proc = proc;
            this.lastPhis = lastPhis;
            this.cache = cache;
            this.cmp = new ExpressionValueComparer();
            this.m = new ExpressionEmitter();
            this.simp = rp.simp;

            this.original = original;
            this.lastPhi = lastPhi;
            // A map that seems to be used to detect loops in the call graph:
            this.called = new Dictionary<CallInstruction, Expression>();
            this.query = (BinaryExpression)_query;
        }

        public bool Execute()
        {
            this.phiInd = query.Right.CloneExpression();

            if (lastPhi != null && cache.ContainsKey(lastPhi) && cmp.Equals(cache[lastPhi], phiInd))
            {
                Debug.WriteLineIf(trace.TraceVerbose, "true - in the phi cache");
                return true;
            }

            this.refsTo = new HashSet<Instruction>();
            this.query = (BinaryExpression) query.CloneExpression();
            this.change = true;
            this.swapped = false;

            while (change)
            {
                change = false;
                if (query.Operator == Operator.Eq)
                {
                    // same left and right means true
                    if (cmp.Equals(query.Left, query.Right))
                    {
                        query = Constant.True();
                        change = true;
                    }

                    // move constants to the right
                    if (!change)
                    {
                        BinaryExpression plus = query.Left as BinaryExpression;
                        Expression s1s2 = plus != null ? plus.Right : null;
                        if (plus != null && s1s2 != null)
                        {
                            if (plus.Operator == Operator.IAdd && s1s2 is Constant) //.isIntConst())
                            {
                                query.Right = m.IAdd(query.Right, m.Neg(s1s2.CloneExpression()));
                                query.Left = ((BinaryExpression)plus).Left;
                                change = true;
                            }
                            if (plus.Operator == Operator.ISub && s1s2 is Constant) // .isIntConst())
                            {
                                query.Right = m.IAdd(query.Right, s1s2.CloneExpression());
                                query.Left = (((BinaryExpression)plus).Left);
                                change = true;
                            }
                        }
                    }

                    // substitute using a statement that has the same left as the query
                    if (!change && query.Left is Identifier)
                    {
                        SsaIdentifier r = ssa.Identifiers[(Identifier)query.Left];
                        Instruction s = r.DefStatement.Instruction;
                        var call = s as CallInstruction;
                        if (call != null)
                        {
                            // See if we can prove something about this register.
                            ProcedureConstant pc; // = call.Callee as ProcedureConstant;
                            //Procedure destProc = (UserProc*)call.getDestProc();
                            Expression eBase = r.Identifier;
                            if (call.Callee.As(out pc) && pc.Procedure is Procedure &&
                                rp.cycleGrp.Contains((Procedure)pc.Procedure))
                            {
                                var destProc = (Procedure)pc.Procedure;

                                // The destination procedure may not have preservation proved as yet, because it is involved
                                // in our recursion group. Use the conditional preservation logic to determine whether query is
                                // true for this procedure
                                Expression provenTo = getProven(destProc, eBase);
                                if (provenTo != null)
                                {
                                    // There is a proven preservation. Use it to bypass the call
                                    Expression queryLeft = localiseExp(call, provenTo.CloneExpression());
                                    query.Left = queryLeft;
                                    // Now try everything on the result
                                    var pp = new Prover_v2(rp, proc, ssa, query, lastPhis, cache, original, lastPhi);
                                    return pp.Execute();
                                }
                                else
                                {
                                    // Check if the required preservation is one of the premises already assumed
                                    Expression premisedTo = getPremised(destProc, eBase);
                                    if (premisedTo != null)
                                    {
                                        DebugEx.PrintIf(trace.TraceVerbose, "conditional preservation for call from {0} to {1}, allows bypassing", proc.Name, destProc.Name);
                                        Expression queryLeft = localiseExp(call, premisedTo.CloneExpression());
                                        query.Left = queryLeft;
                                        var pp = new Prover_v2(rp, proc, ssa, query, lastPhis, cache, original, lastPhi);
                                        return pp.Execute();
                                    }
                                    else
                                    {
                                        // There is no proof, and it's not one of the premises. It may yet succeed, by making
                                        // another premise! Example: try to prove esp, depends on whether ebp is preserved, so
                                        // recurse to check ebp's preservation. Won't infinitely loop because of the premise map
                                        // FIXME: what if it needs a rx = rx + K preservation?
                                        Expression newQuery = m.Eq(eBase.CloneExpression(), eBase.CloneExpression());
                                        setPremise(destProc, eBase);
                                        DebugEx.PrintIf(trace.TraceVerbose, "new required premise {0} for {1}", newQuery, destProc.Name);

                                        // Make speculative preservation conditional as true, since even if proven, this is conditional on other things
                                        var rp = new RegisterPreservation_2(destProc, getssa[destproc]);
                                        bool result = rp.prove(newQuery, true);
                                        killPremise(destProc, eBase);
                                        if (result)
                                        {
                                            DebugEx.PrintIf(trace.TraceVerbose,
                                                "conditional preservation with new premise {0} succeeds for {1}", newQuery, destProc.Name);
                                            // Use the new conditionally proven result
                                            Expression queryLeft = localiseExp(call, eBase.CloneExpression());
                                            query.Left = queryLeft;
                                            var prover = new Prover_v2(rp, proc, ssa, query, lastPhis, cache, original, lastPhi);
                                            return prover.Execute();
                                        }
                                        else
                                        {
                                            DebugEx.PrintIf(trace.TraceVerbose, "conditional preservation required premise {0}fails!", newQuery);
                                            // Do nothing else; the outer proof will likely fail
                                        }
                                    }
                                }

                            } // End call involved in this recursion group

                            // Seems reasonable that recursive procs need protection from call loops too
                            Expression right = getProven(call, r.Identifier); // getProven returns the right side of what is
                            if (right != null)                          //    proven about r (the LHS of query)
                            {
                                right = right.CloneExpression();
                                if (called.ContainsKey(call) && cmp.Equals(called[call], query))
                                {
                                    DebugEx.PrintIf(trace.TraceVerbose, "found call loop to {0} {1}", call.Callee, query);
                                    query = Constant.False();
                                    change = true;
                                }
                                else
                                {
                                    called[call] = query.CloneExpression();
                                    DebugEx.PrintIf(trace.TraceVerbose, "using proven for {0} {1} = {2}", call.Callee, r.Left, right);
                                    right = localiseExp(call, right);
                                    DebugEx.PrintIf(trace.TraceVerbose, "right with subs: {0}", right);
                                    query.Left = right; // Replace LHS of query with right
                                    change = true;
                                }
                            }
                            return false;
                        }
                        else if (s != null && s is PhiAssignment)
                        {
                            // for a phi, we have to prove the query for every statement
                            PhiAssignment pa = (PhiAssignment)s;
                            change = ProcessPhiAssignment(pa);
                            if (!change)
                                break;
                        }
                        else if (s != null && s is Assignment)
                        {
                            Assignment ass = (Assignment)s;
                            change = ProcessAssignment(ass);
                        }
                    }

                    // remove memofs from both sides if possible
                    if (!change && query.Left is MemoryAccess && query.Right is MemoryAccess)
                    {
                        query.Left = ((MemoryAccess)query.Left).EffectiveAddress;
                        query.Right = ((MemoryAccess)query.Right).EffectiveAddress;
                        change = true;
                    }

                    // find a memory def for the right if there is a memof on the left
                    // FIXME: this seems pretty much like a bad hack!
                    if (!change && query.Left is MemoryAccess)
                    {
                        foreach (var it in proc.Statements)
                        {
                            Store ass2;
                            if (it.Instruction.As(out ass2) && cmp.Equals(ass2.Src, query.Right))
                            {
                                query.Right = ass2.Dst;
                                change = true;
                                break;
                            }
                        }
                    }

                    // last chance, swap left and right if haven't swapped before
                    if (!change && !swapped)
                    {
                        Expression e = query.Left;
                        query.Left = query.Right;
                        query.Right = e;
                        change = true;
                        swapped = true;
                        refsTo.Clear();
                    }
                }
                else if (query is Constant)
                {
                    Constant c = (Constant)query;
                    query = Constant.Bool(c.ToBoolean());
                }
                _query = query.Accept(simp);
            }

            return query.getOper() == opTrue;
        }

        private bool ProcessAssignment(Assignment ass)
        {
            if (refsTo.Contains(ass))
            {
                DebugEx.PrintIf(trace.TraceVerbose, "detected ref loop {0}", ass);
                Debug.WriteLine("refsTo: ");
                Debug.WriteLine(string.Join(", ", refsTo));
                Debug.Assert(false);
                return false;
            }
            else
            {
                refsTo.Add(ass);
                query.Left = ass.Src.CloneExpression();
                return true;
            }
        }

        private bool ProcessPhiAssignment(PhiAssignment pa)
        {
            bool ok = true;
            if (lastPhis.Contains(pa) || pa == lastPhi)
            {
                Debug.WriteLineIf(trace.TraceVerbose, "phi loop detected ");
                ok = cmp.Equals(query.Right, phiInd);
                if (trace.TraceVerbose)
                {
                    if (ok) Debug.WriteLine("(set true due to induction)\n");
                    else
                        Debug.Print("(set false {0} != {1})", query.Right, phiInd);
                }
            }
            else
            {
                DebugEx.PrintIf(trace.TraceVerbose, "found {0} prove for each", pa);
                foreach (var it in pa.Src.Arguments)
                {
                    BinaryExpression e = (BinaryExpression)query.CloneExpression();
                    SsaIdentifier r1 = ssa.Identifiers[(Identifier)e.Left];
                    DebugEx.PrintIf(trace.TraceVerbose, "proving for {0}", e);
                    lastPhis.Add(lastPhi);
                    var pr = new Prover_v2(rp, proc, ssa, e, lastPhis, cache, original, pa);
                    if (pr.Execute())
                    {
                        ok = false;
                        // delete e;
                        return false;
                        //break;
                    }
                    lastPhis.Remove(lastPhi);
                    // delete e;
                }
                if (ok)
                    cache[pa] = query.Right.CloneExpression();
            }
            query = (BinaryExpression)(object)Constant.Bool(ok);
            change = true;
            return true;
        }

        private Expression localiseExp(CallInstruction call, Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression getProven(CallInstruction call, Identifier identifier)
        {
            throw new NotImplementedException();
        }

        private void killPremise(Procedure destProc, Expression eBase)
        {
            throw new NotImplementedException();
        }

        private void setPremise(Procedure destProc, Expression eBase)
        {
            throw new NotImplementedException();
        }

        private Expression getPremised(Procedure proc, Expression left)
        {
            Expression value;
            if (!rp.recurPremises[proc].TryGetValue(left, out value))
                value = null;
            return value;
        }

        private Expression getProven(Procedure destProc, Expression eBase)
        {
            throw new NotImplementedException();
        }
    }
}
