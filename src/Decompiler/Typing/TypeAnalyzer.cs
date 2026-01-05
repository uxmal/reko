#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Typing
{
    /// <summary>
    /// Gathers type information, infers structure, union, and array types,
    /// then rewrites the program as appropriate to incorporate the inferred
    /// types. Much of the type inference code in this namespace was inspired
    /// by the master's thesis "Entwicklung eines Typanalysesystem für einen
    /// Decompiler", 2004, by Raimar Falke.
    /// </summary>
    public class TypeAnalyzer
	{
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(TypeAnalyzer), "Traces the progress of the type analysis") { Level = TraceLevel.Verbose };

        private readonly IEventListener eventListener;

		private TypeFactory? factory;
		private TypeStore? store;
		private ExpressionNormalizer? aen;
		private EquivalenceClassBuilder? eqb;
        private TypeCollector? tyco;
        //private DerivedPointerAnalysis dpa;
        private TypeVariableReplacer? tvr;
		private TypeTransformer? trans;
		private ComplexTypeNamer? ctn;
		private TypedExpressionRewriter? ter;

        /// <summary>
        /// Constucts an instance of the <see cref="TypeAnalyzer"/> class.
        /// </summary>
        /// <param name="eventListener"><see cref="IEventListener"/> to which 
        /// diagnostic messages are reported.
        /// </param>
		public TypeAnalyzer(IEventListener eventListener)
		{
			this.eventListener = eventListener;
		}

		/// <summary>
		/// Performs type analysis and rewrites program based on the inferred
        /// information.
		/// </summary>
		/// <remarks>
		/// For instance, all MemoryAccesses will be converted to structure
        /// field accesses or array accesses as appropriate.
		/// </remarks>
		/// <param name="program"></param>
		public void RewriteProgram(Program program)
		{
            factory = program.TypeFactory;
            store = program.TypeStore;
            var timer = new Stopwatch();

            aen = new ExpressionNormalizer(program.Platform.PointerType);
            eqb = new EquivalenceClassBuilder(factory, store, eventListener);
            tyco = new TypeCollector(
                program.TypeFactory, store, program,
                eventListener);
            //dpa = new DerivedPointerAnalysis(factory, store, program.Architecture);
            tvr = new TypeVariableReplacer(store);
            trans = new TypeTransformer(factory, store,program, eventListener);
            ctn = new ComplexTypeNamer();
            ter = new TypedExpressionRewriter(program, store, eventListener);
            
            RestrictProcedures(program, 0, 0, false);
            Time("Normalizing expressions", () => aen.Transform(program));
            Time("Building equivalence classes", () => eqb.Build(program));
            Time("Collecting data types", tyco.CollectTypes);
            Time("Build eq. class data types", () => store.BuildEquivalenceClassDataTypes(factory, eventListener));
            //dpa.FollowConstantPointers(program);
            Time("Replacing type variables", tvr.ReplaceTypeVariables);

            eventListener.Progress.ShowStatus("Transforming datatypes.");
            Time("Replace primitive types", () =>
            {
                var ppr = new PtrPrimitiveReplacer(factory, store, program, eventListener);
                ppr.ReplaceAll();
            });

			Time("Transforming data types", trans.Transform);
			Time("Renaming data types", () => ctn.RenameAllTypes(store));
            Time("Rewriting Program with type information", () => ter.RewriteProgram(program));
		}

        private void Time(string message, Action action)
        {
            var timer = new Stopwatch();
            trace.Inform("== {0}: {1} ======", nameof(TypeAnalyzer), message);
            timer.Start();
            action();
            timer.Stop();
            trace.Inform("   {0} msec", timer.Elapsed);
        }

        /// <summary>
        /// $DEBUG: for debugging only, only performs type analysis on the 
        /// procedures starting at index <paramref name="start" /> and ending at 
        /// index <paramref name="end" />.
        /// </summary>
        [Conditional("DEBUG")]
        private void RestrictProcedures(Program program, int start, int end, bool dumpProcedures)
        {
            if (program.DebugProcedureRange.Item1 != 0 || program.DebugProcedureRange.Item2 != 0)
            {
                (start, end) = program.DebugProcedureRange;
            }
            if (start == 0 && end == 0)
                return;

            end = end != 0 ? end : program.Procedures.Count;
            end = Math.Min(program.Procedures.Count, end);
            int count = Math.Max(end - start, 0);
            eventListener.Info(new NullCodeLocation("TypeAnalysis"), "Filtering procedures to {0}:{1}", start, end);
            var procs = program.Procedures.Values.Skip(start).Take(count).ToArray();
            // Use the below commented-out code to get the names of the procedures that 
            // are being selected for type analysis.
            //using (var tw = File.CreateText("d:/tmp/tmp.txt"))
            //{
            //    for (int i = start; i < end; ++i)
            //    {
            //        var procedure = procs[i];
            //        tw.WriteLine("{0}:{1} {2}", i, procedure.EntryAddress, procedure.Name);
            //    }
            //}
            program.Procedures.Clear();
            for (int i = 0; i < procs.Length; ++i)
            {
                program.Procedures[procs[i].EntryAddress] = procs[i];
                if (dumpProcedures)
                {
                    procs[i].Dump(true);
                }
                else
                {
                    Debug.WriteLine(procs[i].Name);
                }
            }
        }

        /// <inheritdoc/>
		public void WriteTypes(string filename, TextWriter output)
		{
		}
	}
}
