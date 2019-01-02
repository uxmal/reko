#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
	public class SsaState
	{
		private SsaIdentifierCollection ids;

		public SsaState(Procedure proc, DominatorGraph<Block> domGraph)
		{
			this.Procedure = proc;
            this.DomGraph = domGraph;
			this.ids = new SsaIdentifierCollection();
		}

        public Procedure Procedure { get; private set; }
        public DominatorGraph<Block> DomGraph { get; private set; }

        /// <summary>
        /// Inserts the instr d of the identifier v at statement S.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="S"></param>
        public void Insert(Instruction d, Identifier v, Statement S)
        {
            // Insert new phi-functions.
            foreach (var dfFode in DomGraph.DominatorFrontier(S.Block))
            {
                // If there is no phi-function for v
                //    create new phi-function for v. (which is an insert, so call self recursively)
                // All input operands of the new phi-finctions are initually assumed to be
                // uses of r.

                // Update uses sets for all uses dominated by S, or the new phi statements.
                // This is done by walking down the dominator tree from each def and find uses
                // that along wit the def match property 1.

                // Update each use that is a parameter of a newly created phi-function, according
                // to property 2.
            }
        }
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
        /// Check if there are uses which is not in the procedure statements
        /// list
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
        /// Compare uses stored in SsaState with actual ones
        /// </summary>
        public void ValidateUses(Action<string> error)
        {
            ValidateDeadUses(error);
            var uc = new InstructionUseCollector();
            foreach (var stm in Procedure.Statements)
            {
                var idMapStored = GetStatementIdentifiers(stm);
                var idMapActual = uc.CollectUses(stm);
                foreach (var id in idMapStored.Keys)
                {
                    if (!idMapActual.ContainsKey(id) || idMapActual[id] < idMapStored[id])
                        error(
                            string.Format(
                                "{0}: incorrect {1} id in {2} uses",
                                Procedure.Name,
                                id,
                                stm));
                }
                foreach (var id in idMapActual.Keys)
                {
                    if (!idMapStored.ContainsKey(id) || idMapStored[id] < idMapActual[id])
                        error(
                            string.Format(
                                "{0}: there is no {1} id in {2} uses",
                                Procedure.Name,
                                id,
                                stm));
                }
            }
        }

        /// <summary>
        /// Collect all the statements that are used in statement
        /// stm, _according to the SsaState_. 
        /// </summary>
        /// <param name="stm"></param>
        /// <returns></returns>
        private IDictionary<Identifier, int> GetStatementIdentifiers(Statement stm)
        {
            var idMap =
               (from sid in Identifiers
                from use in sid.Uses
                where use == stm
                group new { sid.Identifier, use } by sid.Identifier into g
                select new { Key = g.Key, Value = g.Count() })
                .ToDictionary(de => de.Key, de => de.Value);
            return idMap;
        }

        /// <summary>
        /// Compare definitions stored in SsaState with actual ones
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
        /// <param name="pstm"></param>
        public void DeleteStatement(Statement stm)
		{
			// Remove all definitions and uses.
			ReplaceDefinitions(stm, null);

			// For each used identifier remove this statement from its uses.
			RemoveUses(stm);

			// Remove the statement itself.
			stm.Block.Statements.Remove(stm);
		}

        public int RpoNumber(Block b)
        {
            return DomGraph.ReversePostOrder[b];
        }

		/// <summary>
		/// Writes all SSA identifiers, showing the original variable,
		/// the defining statement, and the using statements.
		/// </summary>
		/// <param name="writer"></param>
		public void Write(TextWriter writer)
		{
			foreach (SsaIdentifier id in ids)
			{
				id.Write(writer);
				writer.WriteLine();
			}
		}

		public SsaIdentifierCollection Identifiers { get { return ids; } }

		public void ReplaceDefinitions(Statement stmOld, Statement stmNew)
		{
			foreach (var sid in Identifiers)
			{
				if (sid.DefStatement == stmOld)
					sid.DefStatement = stmNew;
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
        /// Add all identifiers used in <paramref name="stm"/>.
        /// </summary>
        /// <param name="stm"></param>
        public void AddUses(Statement stm)
        {
            var iua = new InstructionUseAdder(stm, Identifiers);
            stm.Instruction.Accept(iua);
        }

        /// <summary>
        /// Undoes the SSA renaming by replacing each ssa identifier with its original identifier.
        /// </summary>
        private class UnSSA : InstructionTransformer
		{
			private SsaState ssa;

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
