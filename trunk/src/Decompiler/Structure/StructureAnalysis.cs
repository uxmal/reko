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

using Decompiler.Core;
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
	public class StructureAnalysis
	{
		private Procedure proc;
		private DominatorGraph domGraph;
		private IntervalFinder itvf;

		private static TraceSwitch trace = new TraceSwitch("CodeStructure", "Traces the flow of code structuring");

		public StructureAnalysis(Procedure proc)
		{
			int cBlocks = proc.RpoBlocks.Count;
			this.proc = proc;
			domGraph = new DominatorGraph(proc);
			itvf = new IntervalFinder(proc);
		}

		private BitSet AllBlocks(Procedure proc)
		{
			BitSet b = proc.CreateBlocksBitset();
			b.SetAll(true);
#if BOGUS
			for (int i = 0; i < proc.RpoBlocks.Count; ++i)
			{
				Block block = proc.RpoBlocks[i];
				if (block.Statements.Count > 0)
					b[i] = true;
			}
#endif
			return b;
		}

		private void FindLoops()
		{
			DominatorGraph dom = new DominatorGraph(proc);
			int intCount = -1;
			IntervalFinder intf = new IntervalFinder(proc);
			while (intf.Intervals.Count != intCount)
			{
				intCount = intf.Intervals.Count;
				foreach (Interval I in intf.Intervals)
				{
					LoopFinder lrw = new LoopFinder(proc, dom);
					Loop loop = lrw.FindLoop(I);
					if (loop != null)
						lrw.BuildLoop(loop);
				}
				proc.Dump(trace.TraceVerbose, false);
				dom = new DominatorGraph(proc);
				intf = new IntervalFinder(proc);
			} 
		}
		
		public void FindStructures()
		{
			if (proc.Name.EndsWith("6_2F6F"))
				proc.Name.ToCharArray();
			
			CompoundConditionCoalescer ccc = new CompoundConditionCoalescer(proc);
			ccc.Transform();
			Debug.WriteLineIf(trace.TraceWarning, "Finding structure in " + proc.Name);
			FindLoops();
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			lin.ProcedureExit = proc.ExitBlock;
			lin.Linearize(AllBlocks(proc), false).CopyTo(proc.Body);
		}
	}

}