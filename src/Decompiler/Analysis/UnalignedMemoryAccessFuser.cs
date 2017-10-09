#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// Fuses pairs of unaligned loads or stores.
    /// </summary>
    /// <remarks>
    /// This transform must run before value propagation, because 
    /// it depends on the availability of the source/target registers
    /// of the LWR/LWL and SWL/SWR instructions. Value propagation
    /// can wipe these out.
    /// </remarks>
    public class UnalignedMemoryAccessFuser : InstructionVisitorBase
    {
        private SsaState ssa;

        public UnalignedMemoryAccessFuser(SsaState ssa)
        {
            this.ssa = ssa;
        }

        public void Transform()
        {
            // Find all unaligned reds and writs and group them by
            // register + innstruction.
            var x = FindAllUnalignedInstructions(ssa.Procedure.Statements);

            // Find pairs of instructions that are (left/right) and
            // whose offsets differ by 3 bytes.
            var y = GroupUnalignedAccesses(x);

            // Fuse the pairs.
            FuseUnalignedPairs(y);
        }

        private void FuseUnalignedPairs(List<Tuple<UnalignedAccess, UnalignedAccess>> pairs)
        {
            foreach (var pair in pairs)
            {
                if (pair.Item1.isWrite)
                {
                    FuseStorePair(pair);
                }
                else
                {
                    FuseLoadPair(pair);
                }
            }
        }

        private void FuseStorePair(Tuple<UnalignedAccess, UnalignedAccess> pair)
        {
            Statement stmL, stmR;
            if (pair.Item1.isLeft)
            {
                stmL = pair.Item1.stm;
                stmR = pair.Item2.stm;
            }
            else
            {
                stmL = pair.Item2.stm;
                stmR = pair.Item1.stm;
            }

            ssa.RemoveUses(stmL);
            ssa.RemoveUses(stmR);
            stmR.Instruction = new Store(
                new MemoryAccess(
                new BinaryExpression(
                    Operator.IAdd,
                    PrimitiveType.Word32,
                    pair.Item1.reg,
                    Constant.Int32(pair.Item1.offset)),
                PrimitiveType.Word32),
                pair.Item1.value);
            stmL.Block.Statements.Remove(stmL);
            ssa.AddUses(stmR);
        }

        private void FuseLoadPair(Tuple<UnalignedAccess, UnalignedAccess> pair)
        {
            throw new NotImplementedException();
        }

        private List<Tuple<UnalignedAccess, UnalignedAccess>> GroupUnalignedAccesses(Dictionary<Identifier, List<UnalignedAccess>> x)
        {
            var pairs = new List<Tuple<UnalignedAccess, UnalignedAccess>>();
            foreach (var de in x)
            {
                var sorted = de.Value.ToSortedList(k => k.offset);
                foreach (var se in sorted)
                {
                    UnalignedAccess other;
                    if (!sorted.TryGetValue(se.Key + 3, out other))
                        continue;
                    if (se.Value.isWrite != other.isWrite)
                        continue;
                    if (se.Value.isLeft == other.isLeft)
                        continue;

                    var pair = Tuple.Create(se.Value, other);
                    pairs.Add(pair);
                }
            }
            return pairs;
        }

        private readonly string[] unalignedIntrinsics =
        {
            PseudoProcedure.LwL,
            PseudoProcedure.LwR,
            PseudoProcedure.SwL,
            PseudoProcedure.SwR,
        };

        public class UnalignedAccess
        {
            public Statement stm;
            public Identifier reg;
            public int offset;
            public bool isWrite;
            public bool isLeft;
            public Expression value;
        }

        private Dictionary<Identifier, List<UnalignedAccess>> FindAllUnalignedInstructions(IEnumerable<Statement> stms)
        {
            var dict = new Dictionary<Identifier, List<UnalignedAccess>>();
            foreach (var stm in stms)
            {
                var side = stm.Instruction as SideEffect;
                if (side == null)
                    continue;
                var tup = MatchIntrinsicApplication(side.Expression, unalignedIntrinsics);
                if (tup == null)
                    continue;
                var appName = tup.Item1;
                var app = tup.Item2;
                var reg = GetRegisterOf(app.Arguments[0]);
                var offset = GetOffsetOf(app.Arguments[0]);
                var ua = new UnalignedAccess
                {
                    stm = stm,
                    reg = reg,
                    offset = offset,
                    isWrite = appName[2] == 's',
                    isLeft = appName.EndsWith("l"),
                    value = app.Arguments[1],
                };
                List<UnalignedAccess> accesses;
                if (!dict.TryGetValue(reg, out accesses))
                {
                    accesses = new List<UnalignedAccess>();
                    dict.Add(reg, accesses);
                }
                accesses.Add(ua);
            }
            return dict;
        }

        // On MIPS-LE the sequence
        //   lwl rx,K+3(ry)
        //   lwr rx,K(ry)
        // is an unaligned read. On MIPS-BE it instead is:
        //   lwl rx,K(ry)
        //   lwr rx,K+3(ry)
        //public void FuseUnalignedLoads(Assignment assR)
        //{
        //    var appR = MatchIntrinsicApplication(assR.Src, PseudoProcedure.LwR);
        //    if (appR == null)
        //        return;

        //    var regR = assR.Dst;
        //    var stmR = ssa.Identifiers[regR].DefStatement;

        //    var memR = appR.Arguments[1];
        //    var offR = GetOffsetOf(memR);

        //    var appL = appR.Arguments[0] as Application;
        //    Statement stmL = null;
        //    Assignment assL = null;
        //    if (appL == null)
        //    {
        //        var regL = (Identifier)appR.Arguments[0];
        //        stmL = ssa.Identifiers[regL].DefStatement;
        //        if (stmL == null)
        //            return;
        //        assL = stmL.Instruction as Assignment;
        //        if (assL == null)
        //            return;

        //        appL = assL.Src as Application;
        //    }
        //    appL = MatchIntrinsicApplication(appL, PseudoProcedure.LwL);
        //    if (appL == null)
        //        return;

        //    var memL = appL.Arguments[1];
        //    var offL = GetOffsetOf(memL);

        //    Expression mem;
        //    if (offR + 3 == offL)
        //    {
        //        // Little endian use
        //        mem = memR;
        //    }
        //    else if (offL + 3 == offR)
        //    {
        //        // Big endian use
        //        mem = memL;
        //    }
        //    else
        //        return;

        //    ssa.RemoveUses(stmL);
        //    ssa.RemoveUses(stmR);
        //    if (assL != null)
        //    {
        //        assL.Src = appL.Arguments[0];
        //        ssa.AddUses(stmL);
        //    }
        //    assR.Src = mem;
        //    ssa.AddUses(stmR);
        //}


        //// On MIPS-LE the sequence
        ////   swl rx,K+3(ry)
        ////   swr rx,K(ry)
        //// is an unaligned store.
        //public Instruction FuseUnalignedStores(SideEffect se)
        //{
        //    var appR = MatchIntrinsicApplication(se.Expression, PseudoProcedure.SwR);
        //    if (appR == null)
        //        return se;
   
        //    var sidR = ssa.Identifiers[idR];
        //    Application appL = null;
        //    Statement stmL = null;
        //    Statement stmR = null;
        //    foreach (var use in sidR.Uses)
        //    {
        //        var s = use.Instruction as SideEffect;
        //        if (s == null)
        //            continue;
        //        var app = MatchIntrinsicApplication(s.Expression, PseudoProcedure.SwR);
        //        if (app != null)
        //        {
        //            appR = app;
        //            stmR = use;
        //        }
        //        app = MatchIntrinsicApplication(s.Expression, PseudoProcedure.SwL);
        //        if (app != null)
        //        {
        //            appL = app;
        //            stmL = use;
        //        }
        //    }
        //    if (stmL == null || stmR == null)
        //        return se;

        //    var memL = appL.Arguments[0];
        //    var offL = GetOffsetOf(memL);

        //    var memR = appR.Arguments[0];
        //    var offR = GetOffsetOf(memR);

        //    Expression mem;
        //    if (offR + 3 == offL)
        //    {
        //        // Little endian use
        //        mem = memR;
        //    }
        //    else if (offL + 3 == offR)
        //    {
        //        // Big endian use
        //        mem = memL;
        //    }
        //    else
        //        return se;

        //    ssa.RemoveUses(stmL);
        //    ssa.RemoveUses(stmR);
        //    stmR.Instruction = mem is Identifier
        //        ? (Instruction)new Assignment((Identifier)mem, sidR.Identifier)
        //        : (Instruction)new Store(mem, sidR.Identifier);
        //    stmL.Block.Statements.Remove(stmL);
        //    ssa.AddUses(stmR);
        //    return stmR.Instruction;
        //}


        private Identifier GetRegisterOf(Expression e)
        {
            MemoryAccess mem = e as MemoryAccess;
            if (e != null)
            {
                return (Identifier)((BinaryExpression)mem.EffectiveAddress).Left;
            }
            else
            {
                return (Identifier)e;
            }
        }

        private int GetOffsetOf(Expression e)
        {
            var id = e as Identifier;
            if (id != null)
            {
                var mem = id.Storage as StackStorage;
                return mem.StackOffset;
            }
            else
            {
                var mem = (MemoryAccess)e;
                var binL = mem.EffectiveAddress as BinaryExpression;
                if (binL != null)
                {
                    return ((Constant)binL.Right).ToInt32();
                }
                else
                {
                    return 0;
                }
            }
        }

        private Tuple<string, Application> MatchIntrinsicApplication(Expression e, string [] names)
        {
            var app = e as Application;
            if (app == null)
                return null;
            var pc = app.Procedure as ProcedureConstant;
            if (pc == null)
                return null;
            var ppp = pc.Procedure as PseudoProcedure;
            if (ppp == null)
                return null;
            if (!names.Contains(ppp.Name))
                return null;
            return Tuple.Create(ppp.Name, app);
        }
    }
}
