#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    /// <summary>
    /// This class maintains the SSA transformation of a given procedure. Specifically
    /// it keeps tracks of all SSA Identifiers, which implicitly form a graph of 
    /// identifiers connected by use->def and def->use edges.
    /// </summary>
    [DebuggerDisplay("{Procedure.Name}")]
	public class SsaState
	{
		public SsaState(Procedure proc)
		{
			this.Procedure = proc;
			this.Identifiers = new SsaIdentifierCollection();
		}

        public SsaIdentifierCollection Identifiers { get; }

        public Procedure Procedure { get; }

        /// <summary>
        /// Given a procedure in SSA form, converts it back to "normal" form.
        /// </summary>
        /// <param name="renameVariables"></param>
        public void ConvertBack(bool renameVariables)
		{
			UnSSA unssa = new UnSSA(this);
			foreach (Block block in Procedure.ControlGraph.Blocks)
			{
				for (int st = 0; st < block.Statements.Count; ++st)
				{
					Instruction instr = block.Statements[st].Instruction;
					if (instr is PhiAssignment || instr is DefInstruction)
					{
						block.Statements.RemoveAt(st);
						--st;
					}
					else if (renameVariables)
					{
						instr.Accept(unssa);
					}
				}
			}

			// Remove any instructions in the return block, used only 
			// for computation of reaching definitions.

			Procedure.ExitBlock.Statements.Clear();
		}

        /// <summary>
        /// Returns the index of the last <see cref="DefInstruction"/> in the 
        /// <paramref name="stmts"/> statement list.
        /// </summary>
        /// <param name="stmts"></param>
        /// <returns></returns>
        private int LastDefPosition(StatementList stmts)
        {
            for (int i = stmts.Count - 1; i >= 0; --i)
            {
                if (stmts[i].Instruction is DefInstruction)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// If there is no defined SSA identifier for <paramref name="id"/>
        /// identifier, then create DefInstruction in the <paramref name="b"/>
        /// block. Return existing SSA identifier otherwise
        /// </summary>
        /// <param name="id"></param>
        /// <param name="b"></param>
        /// <returns>
        /// New or existing SSA identifier for <paramref name="id"/>
        /// </returns>
        public SsaIdentifier EnsureDefInstruction(Identifier id, Block b)
        {
            if (Identifiers.TryGetValue(id, out var sid))
                return sid;
            sid = Identifiers.Add(id, null, null, false);
            var stm = new Statement(
                b.Address.ToLinear(),
                new DefInstruction(id),
                b);
            sid.DefStatement = stm;

            if (b == b.Procedure.EntryBlock)
            {
                int defPos = LastDefPosition(b.Statements) + 1;
                b.Statements.Insert(defPos, stm);
            }
            else
            {
                b.Statements.Add(stm);
            }
            return sid;
        }

        [Conditional("DEBUG")]
		public void Dump(bool trace)
		{
			if (trace)
			{
				StringWriter sb = new StringWriter();
				Write(sb);
				Procedure.Write(false, sb);
				Debug.WriteLineIf(trace, sb.ToString());
			}
		}

        public void Validate(Action<string> error)
        {
            ValidateUses(error);
            ValidateDefinitions(error);
        }

        [Conditional("DEBUG")]
        public void ValidateUses()
        {
            ValidateUses(s => Debug.WriteLine(s));
        }

        /// <summary>
        /// Check if there are uses which aren't in the procedure statement
        /// list.
        /// </summary>
        private void ValidateDeadUses(Action<string> error)
        {
            var deadUses = Identifiers
                .SelectMany(
                    sid => sid.Uses
                    .Except(Procedure.Statements)
                    .Select(u => (sid, u)));
            foreach (var (sid, use) in deadUses)
            {
                error(
                    $"{Procedure.Name}: there is no {sid.Identifier} use " +
                    $"({use}) in procedure statements list");
            }
        }

        /// <summary>
        /// Compare uses stored in SsaState with actual ones.
        /// </summary>
        public void ValidateUses(Action<string> error)
        {
            ValidateDeadUses(error);
            var uc = new InstructionUseCollector();
            foreach (var stm in Procedure.Statements)
            {
                var idMapStored = GetStatementIdentifierCounts(stm);
                var idMapActual = uc.CollectUses(stm);
                foreach (var id in idMapStored.Keys)
                {
                    if (!idMapActual.ContainsKey(id) || idMapActual[id] < idMapStored[id])
                        error(
                            string.Format(
                                "{0}: incorrect {1} id in {2}:{3} uses",
                                Procedure.Name,
                                id,
                                stm.Block,
                                stm));
                }
                foreach (var id in idMapActual.Keys)
                {
                    if (!idMapStored.ContainsKey(id) || idMapStored[id] < idMapActual[id])
                        error(
                            string.Format(
                                "{0}: there is no {1} id in {2}:{3} uses",
                                Procedure.Name,
                                id,
                                stm.Block,
                                stm));
                }
            }
        }

        /// <summary>
        /// Counts the number of uses of each identifier referenced in the 
        /// statement <paramref name="stm"/> by consulting the SSA state.
        /// </summary>
        /// <param name="stm"></param>
        /// <returns>A dictionary mapping identifiers to use counts.</returns>
        private IDictionary<Identifier, int> GetStatementIdentifierCounts(Statement stm)
        {
            var idMap =
               (from sid in Identifiers
                from use in sid.Uses
                where use == stm
                group new { sid.Identifier, use } by sid.Identifier into g
                select new { g.Key, Value = g.Count() })
                .ToDictionary(de => de.Key, de => de.Value);
            return idMap;
        }

        /// <summary>
        /// Compare definitions stored in SsaState with actual ones.
        /// </summary>
        public void ValidateDefinitions(Action<string> error)
        {
            var dc = new SsaDefinitionsCollector();
            var actualDefs = new Dictionary<Identifier, Statement>();
            foreach (var stm in Procedure.Statements)
            {
                var definitions = dc.CollectDefinitions(stm);
                foreach (var defId in definitions)
                {
                    if (actualDefs.TryGetValue(defId, out var def))
                    {
                        error(string.Format(
                            "{0}: multiple definitions for {1} ({2} and {3})",
                            Procedure.Name,
                            defId,
                            stm,
                            def));
                    }
                    else
                    {
                        actualDefs.Add(defId, stm);
                    }
                }
            }
            foreach (var sid in Identifiers)
            {
                if (!actualDefs.TryGetValue(sid.Identifier, out var actualDef))
                {
                    actualDef = null;
                }
                if (sid.DefStatement != actualDef)
                {
                    error(string.Format(
                        "{0}: incorrect definition for {1}({2}). Should be {3}",
                        Procedure.Name,
                        sid.Identifier,
                        sid.DefStatement?.ToString() ?? "<null>",
                        actualDef?.ToString() ?? "<null>"));
                }
            }
            foreach (var defId in actualDefs.Keys)
            {
                if (!Identifiers.Contains(defId) &&
                    actualDefs.ContainsKey(defId))
                {
                    error(string.Format(
                        "{0}: there is no {1}({2}) in the ssa identifiers",
                        Procedure.Name,
                        defId,
                        actualDefs[defId]?.ToString() ?? "<null>"));
                }
            }
        }

        /// <summary>
        /// Deletes a statement by removing all the ids it references 
        /// from SSA state, then removes the statement itself from code.
        /// </summary>
        /// <param name="stm"></param>
        public void DeleteStatement(Statement stm)
		{
			// Remove all definitions and uses.
			ReplaceDefinitions(stm, null);

			// For each used identifier remove this statement from its uses.
			RemoveUses(stm);

			// Remove the statement itself.
			stm.Block.Statements.Remove(stm);
		}

		/// <summary>
		/// Writes all SSA identifiers, showing the original variable,
		/// the defining statement, and the using statements.
		/// </summary>
		/// <param name="writer"></param>
		public void Write(TextWriter writer)
		{
			foreach (SsaIdentifier id in Identifiers)
			{
				id.Write(writer);
				writer.WriteLine();
			}
		}

        /// <summary>
        /// For each <see cref="SsaIdentifier"/>, if its defining statement is
        /// <paramref name="stmOld"/>, replace it with <paramref name="stmNew"/>.
        /// </summary>
        /// <param name="stmOld"></param>
        /// <param name="stmNew"></param>
		public void ReplaceDefinitions(Statement stmOld, Statement stmNew)
		{
			foreach (var sid in Identifiers)
			{
				if (sid.DefStatement == stmOld)
					sid.DefStatement = stmNew;
			}
		}

        public void AddDefinitions(Statement stm)
        {
            var dc = new SsaDefinitionsCollector();
            var definitions = dc.CollectDefinitions(stm);
            foreach (var id in definitions)
            {
                if (Identifiers.TryGetValue(id, out var sid))
                {
                    sid.DefExpression = null;
                    sid.DefStatement = stm;
                }
            }
        }

        /// <summary>
        /// Remove all uses <paramref name="stm"/> makes.
        /// </summary>
        /// <param name="stm"></param>
		public void RemoveUses(Statement stm)
		{
			foreach (var sid in Identifiers)
			{
                sid.Uses.RemoveAll(u => u == stm);
			}
		}

        /// <summary>
        /// Remove all uses <paramref name="e"/> expression makes
        /// at <paramref name="stm"/> statement.
        /// </summary>
        /// <param name="stm">Statement</param>
        /// <param name="e">Statement</param>
        public void RemoveUses(Statement stm, Expression e)
        {
            var xu = new ExpressionUseRemover(stm, Identifiers);
            e.Accept(xu);
        }

        /// <summary>
        /// Add all identifiers used in <paramref name="stm"/>.
        /// </summary>
        /// <param name="stm"></param>
        public void AddUses(Statement stm)
        {
            var iua = new InstructionUseAdder(stm, Identifiers);
            stm.Instruction.Accept(iua);
        }

        /// <summary>
        /// Finds all phi statements of a block, and creates a transposition of
        /// their arguments.
        /// </summary>
        /// <remarks>
        /// The phi statements typically cluster at the top of a basic block, and
        /// we often want to see how values pass from predecessors to the block.
        /// We want to "rip" vertical stripes of the phi statements, one 
        /// stripe for each predecessor;
        /// <code>
        /// r_2 = f(r_0, r_1)
        /// r_5 = f(r_3, r_4)
        /// ...
        /// r_z = f(r_x, r_y)
        /// </code>
        /// should result in the following "arg lists" (where pred0 and pred1)
        /// are the block's predecessors.
        /// <code>
        /// pred0: (r_0, r_3, ..., r_x)
        /// pred1: (r_1, r_4, ..., r_y)
        /// </code>
        /// </remarks>
        /// <param name="block"></param>
        /// <returns></returns>
        public Dictionary<Block, CallBinding[]> PredecessorPhiIdentifiers(Block block)
        {
            var dict = new Dictionary<Block, CallBinding[]>();
            if (block.Pred.Count > 1)
            {
                var phis = block.Statements
                    .Select(s => s.Instruction)
                    .OfType<PhiAssignment>()
                    .Select(phi => phi.Src.Arguments
                        .Select(a => new CallBinding(
                            phi.Dst.Storage,
                            a.Value)).ToArray()).ToArray();
                var arrs = Reko.Core.EnumerableEx.ZipMany(
                    phis,
                    ids => ids.ToArray()).ToArray();
                if (arrs.Length == 0)
                    return dict;
                for (int p = 0; p < block.Pred.Count; ++p)
                {
                    dict.Add(block.Pred[p], arrs[p]);
                }
            }
            else if (block.Pred.Count == 1)
            {
                var uses = block.Statements
                    .Select(s => s.Instruction)
                    .OfType<UseInstruction>()
                    .Select(u => 
                        new CallBinding(
                            ((Identifier)u.Expression).Storage,
                            u.Expression));
                dict.Add(block.Pred[0], uses.ToArray());
            }
            return dict;
        }

		/// <summary>
		/// Undoes the SSA renaming by replacing each ssa identifier with its original identifier.
		/// </summary>
		private class UnSSA : InstructionTransformer
		{
			private readonly SsaState ssa;

			public UnSSA(SsaState ssa)
			{
				this.ssa = ssa;
			}

			public override Expression VisitIdentifier(Identifier id)
			{
				return ssa.Identifiers[id].OriginalIdentifier;
			}
		}
	}
}
