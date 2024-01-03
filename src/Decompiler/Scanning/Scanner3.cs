#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    public class Scanner3 : IScanner
    {
        private static readonly TraceSwitch trace = new(nameof(Scanner3), "");

        private readonly IServiceProvider services;
        private readonly IDecompilerEventListener listener;
        private readonly Program program;
        private readonly TypeLibrary metadata;
        private readonly IDynamicLinker dynamicLinker;

        public Scanner3(
            Program program,
            TypeLibrary metadata,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
        {
            this.program = program;
            this.metadata = metadata;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.listener = services.RequireService<IDecompilerEventListener>();
        }

        public void ScanImage()
        {
            // Strategy: find every possible basic block, all of which end in a CTI.
            // This "block soup" will be an over-estimate of the actual basic blocks
            // of the program.
            // Blocks may end in:
            // - direct jumps, branches: remember the targets of the jumps. 
            // - an indirect jump: use backwalking to determine the targets of the indirect jump
            // - direct calls: count the number of times a particular target is called; 
            //   assume fall-through
            // - indirect calls: assume fall-through
            var shscan = new ShingleScanner(this.program, new ScanResultsV2(), dynamicLinker, listener, services);
            var (blockSoup, soupTime) = Time(shscan.ScanProgram);
            trace.Inform("= Block soup scanner ======");
            trace.Inform("Found {0} callees", blockSoup.Procedures.Count);
            trace.Inform("      {0} basic blocks", blockSoup.Blocks.Count);
            trace.Inform("      {0} successors", blockSoup.Successors.Count);
            trace.Inform("      {0} predecessors", blockSoup.Predecessors.Count);
            trace.Inform("      {0} bytes", blockSoup.Blocks.Values.Sum(b => b.Length));
            trace.Inform("      in {0} msec", (int) soupTime.TotalMilliseconds);

#if NYI
            // Given a block soup, perform a recursive scan, starting at the "roots".
            // Roots are:
            // - user-specified procedures
            // - the image entry point
            // - image symbols
            // We promote all callees reached during the recursive scan to "procedures".
            var rsc = new RecursiveScanner2(this.program, blockSoup);
            var (bs2, recTime) = Time(rsc.Scan);
            trace.Inform("= Block soup scanner ======");
            trace.Inform("Found {0} procedures", bs2.Procedures.Count);
            trace.Inform("");

            // There may still be blocks in the block soup that weren't reached by the recursive scan.
            // The procedure detector will build procedures out of them. It will also reject
            // blocks that end in invalid instructions.
            var procDetector = new ProcedureDetector(cfg3, listener);
            trace.Inform("= Procedure detector ======");
            var (procs, pdTime) = Time(() => procDetector.DetectProcedures());
            trace.Inform("      Found a total of {0} procs", procs.Count);
            trace.Inform("      in {0} msec", (int) pdTime.TotalMilliseconds);

            trace.Inform("= Building procedure graph ======");
            var (final, finalTime) = Time(() => BuildProcedures(cfg3, procs));
            trace.Inform("      Built a total of {0} procedures", program.Procedures.Count);
            trace.Inform("      in {0} msec", (int) finalTime.TotalMilliseconds);
            listener.Info("Built {0} procedures", program.Procedures.Count);
#endif
        }


        public void ScanImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            throw new NotImplementedException();
        }

        public ProcedureBase ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state)
        {
            var scanner = new RecursiveScanner(program, ProvenanceType.Scanning, dynamicLinker, listener, services);
            //$TODO: handle existing procedures.
            var sr = scanner.ScanProcedure(arch, addr, procedureName, state);
            var procDetector = new ProcedureDetector(sr, listener);
            var procs = procDetector.DetectProcedures();
            BuildProcedures(sr, procs);
            return program.Procedures[addr];
        }

        private Program BuildProcedures(ScanResultsV2 sr, List<RtlProcedure> procs)
        {
            var pgb = new ProcedureGraphBuilder(sr, program);
            pgb.Build(procs);
            return program;
        }

        private static (T, TimeSpan) Time<T>(Func<T> fn)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = fn();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }
    }
}
