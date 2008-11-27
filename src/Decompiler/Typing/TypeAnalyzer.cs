/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Typing
{
	/// <summary>
	/// Gathers type information, infers structure, union, and array types,
	/// then rewrites the program as appropriate to incorporate the inferred types.
	/// Much of the type inference code in this namespace was inspired by the master's thesis
	/// "Entwicklung eines Typanalysesystem für einen Decompiler", 2004, by Raimar Falke.
	/// </summary>
	public class TypeAnalyzer
	{
		private Program prog;
		private InductionVariableCollection ivs;
		private DecompilerHost host;

		private TypeFactory factory;
		private TypeStore store;
		private ExpressionNormalizer aen;
		private EquivalenceClassBuilder eqb;
		private TraitCollector trco;
		private DataTypeBuilder dtb;
		private DerivedPointerAnalysis dpa;
		private TypeVariableReplacer tvr;
		private TypeTransformer trans;
		private ComplexTypeNamer ctn;
		private TypedExpressionRewriter ter;

		public TypeAnalyzer(Program prog, InductionVariableCollection ivs, DecompilerHost host)
		{
			this.prog = prog;
			this.ivs = ivs;
			this.host = host;

			factory = prog.TypeFactory;
			store = prog.TypeStore;

			aen = new ExpressionNormalizer();
			eqb = new EquivalenceClassBuilder(factory, store);
			dtb = new DataTypeBuilder(factory, store);
			trco = new TraitCollector(factory, store, dtb, prog.Globals, ivs);
			dpa = new DerivedPointerAnalysis(factory, store, dtb);
			tvr = new TypeVariableReplacer(store);
			trans = new TypeTransformer(factory, store, host);
			ctn = new ComplexTypeNamer();
			ter = new TypedExpressionRewriter(store, prog.Globals);
		}

		/// <summary>
		/// Performs type analysis and rewrites program based on the inferred information.
		/// </summary>
		/// <remarks>
		/// For instance, all MemoryAccesses will be converted to structure field
		/// accesses or array accesses as appropriate.
		/// </remarks>
		/// <param name="prog"></param>
		public void RewriteProgram()
		{
            RestrictProcedures(0, 4);
			aen.Transform(prog);
			eqb.Build(prog);
			Debug.WriteLine("= Collecting traits ========================================");
			trco.CollectProgramTraits(prog);
			Debug.WriteLine("= Building equivalence classes =============================");
			dtb.BuildEquivalenceClassDataTypes();
			dpa.FollowConstantPointers(prog);
			tvr.ReplaceTypeVariables();
			Debug.WriteLine("= replaced type variables ==================================");
			PtrPrimitiveReplacer ppr = new PtrPrimitiveReplacer(factory, store);
			ppr.ReplaceAll();
			Debug.WriteLine("= Transforming types =======================================");
			trans.Transform();
			ctn.RenameAllTypes(store);
			Debug.WriteLine("= transformed types ========================================");
			Debug.WriteLine("= Rewriting expressions ====================================");
			store.Dump();
			ter.RewriteProgram(prog);
		}

        /// <summary>
        /// $DEBUG: for debugging only, only performs type analysis on the count procedures starting at
        /// procedure start.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        private void RestrictProcedures(int start, int count)
        {
            Procedure[] procs = new Procedure[count];
            for (int i = 0; i < count; ++i)
            {
                procs[i] = prog.Procedures.Values[i + start];
            }
            prog.Procedures.Clear();
            for (uint i = 0; i <procs.Length; ++i)
            {
                prog.Procedures[new Address(i)] = procs[i];
            }
        }

		public void WriteTypes(TextWriter output)
		{
		}
	}
}
