#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class Scanner2 : IScanner
    {
        private static readonly TraceSwitch trace = new(nameof(Scanner2), "");

        private readonly IServiceProvider services;
        private readonly DecompilerEventListener listener;
        private readonly Program program;
        private readonly TypeLibrary metadata;
        private readonly IDynamicLinker dynamicLinker;

        public Scanner2(
            Program program,
            TypeLibrary metadata,
            IDynamicLinker dynamicLinker,
            IServiceProvider services)
        {
            this.program = program;
            this.metadata = metadata;
            this.dynamicLinker = dynamicLinker;
            this.services = services;
            this.listener = services.RequireService<DecompilerEventListener>();
        }

        public void ScanImage()
        {
            trace.Inform("= Loaded file ======");
            trace.Inform("{0} entry points", program.EntryPoints.Count);
            trace.Inform("{0} symbols", program.ImageSymbols.Count);
            trace.Inform("{0} executable bytes",
                program.SegmentMap.Segments.Values
                    .Where(s => s.IsExecutable)
                    .Sum(s => s.MemoryArea.Length));

            // Traverse the executable part of the image using a recursive algorithm.

            var scanner = new RecursiveScanner(program, listener);
            trace.Inform("= Recursive scan ======");
            var (cfg, recTime) = Time(scanner.ScanProgram);
            trace.Inform("Found {0} procs", cfg.Procedures.Count);
            trace.Inform("      {0} basic blocks", cfg.Blocks.Count);
            trace.Inform("      {0} bytes", cfg.Blocks.Values.Sum(b => b.Length));
            trace.Inform("      in {0} msec", (int) recTime.TotalMilliseconds);

            // There will be "islands" of unscanned bytes left over.
            // Scan these using the ShingleScanner.

            var shScanner = new ShingleScanner(program, cfg, listener);
            trace.Inform("= Shingle scan ======");
            var (cfg2, shTime) = Time(shScanner.ScanProgram);
            trace.Inform("Found {0} procs", cfg2.Procedures.Count);
            trace.Inform("      {0} basic blocks", cfg2.Blocks.Count);
            trace.Inform("      {0} bytes", cfg2.Blocks.Values.Sum(b => b.Length));
            trace.Inform("      in {0} msec", (int) shTime.TotalMilliseconds);
            cfg = null;

            // Build predecessor edges.

            trace.Inform("= Predecessor edges ======");
            var (cfg3, predTime) = Time(shScanner.RegisterPredecessors);
            trace.Inform("Graph has {0} predecessors", cfg3.Predecessors.Count);
            trace.Inform("    {0} successors", cfg3.Successors.Count);
            trace.Inform("    in {0} msec", (int) predTime.TotalMilliseconds);
            cfg2 = null;

            // From the soup of basic blocks and edges, construct
            // procedure candidates.

            var procDetector = new ProcedureDetector(cfg3, listener);
            trace.Inform("= Procedure detector ======");
            var (procs, pdTime) = Time(() => procDetector.DetectProcedures());
            trace.Inform("      Found a total of {0} procs", procs.Count);
            trace.Inform("      in {0} msec", (int) pdTime.TotalMilliseconds);

            trace.Inform("= Building procedure graph ======");
            var (final, finalTime) = Time(() => BuildProcedures(procs));
            trace.Inform("      Built a total of {0} procedures", program.Procedures.Count);
            trace.Inform("      in {0} msec", (int) finalTime.TotalMilliseconds);
            listener.Info("Found {0} procedures", program.Procedures.Count);
        }

        private static (T, TimeSpan) Time<T>(Func<T> fn)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = fn();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }

        public void ScanImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            throw new NotImplementedException();
        }

        public ProcedureBase ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state)
        {
            var scanner = new RecursiveScanner(program, listener);
            //$TODO: handle existing procedures.
            var sr = scanner.ScanProcedure(arch, addr, procedureName, state);
            var procDetector = new ProcedureDetector(sr, listener);
            var procs = procDetector.DetectProcedures();
            BuildProcedures(procs);
            return program.Procedures[addr];
        }

        private Program BuildProcedures(List<RtlProcedure> rtlProcs)
        {
            var procs = rtlProcs.ToDictionary(p => p.Entry.Address, CreateProcedure);
            foreach (var rtlProc in rtlProcs)
            {
                BuildProcedure(rtlProc, procs);
            }
            return program;
        }

        private void BuildProcedure(RtlProcedure rtlProc, Dictionary<Address, Procedure> procs)
        {
            var blocks = rtlProc.Blocks.ToDictionary(r => r, r => CreateBlock(r, procs[rtlProc.Entry.Address]));
            var visited = new HashSet<RtlBlock>();

        }

        private Block CreateBlock(RtlBlock rtlBlock, Procedure proc)
        {
            var id = program.NamingPolicy.BlockName(rtlBlock.Address);
            var block = new Block(proc, rtlBlock.Address, id);
            return block;
        }

        private Procedure CreateProcedure(RtlProcedure rtlProc)
        {
            var arch = rtlProc.Entry.Architecture;
            var name = program.NamingPolicy.ProcedureName(rtlProc.Entry.Address);
            var proc = new Procedure(arch, name, rtlProc.Entry.Address, arch.CreateFrame());
            return proc;
        }
    }
}
