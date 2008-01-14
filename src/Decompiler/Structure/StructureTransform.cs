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
using System.Diagnostics;

namespace Decompiler.Structure
{
	public abstract class StructureTransform
	{
		protected static TraceSwitch trace = new TraceSwitch("StructureTransforms", "Traces the flow of code structuring");

		protected StructureTransform(Procedure proc)
		{
		}

		protected AbsynStatement ConvertInstruction(Instruction instr)
		{
			AbsynStatement stm = instr as AbsynStatement;
			if (stm != null)
				return stm;
			Assignment ass = instr as Assignment;
			if (ass != null)
				return new AbsynAssignment(ass.Dst, ass.Src);
			ReturnInstruction ret = instr as ReturnInstruction;
			if (ret != null)
				return new AbsynReturn(ret.Value);
			throw new NotImplementedException("NYI");
		}

#if DEBUG
		protected void DumpBlockSetIf(bool enabled, string caption, BitSet s)
		{
			if (enabled)
			{
				Debug.Write(caption);
				Debug.Write(" [ ");
				foreach (int i in s)
				{
					Debug.Write(string.Format("{0} ", i));
				}
				Debug.WriteLine("]");
			}
		}
#else
		protected void DumpBlockSetIf(bool enabled, string caption, BitSet s)
		{
		}
#endif

		protected Branch GetBranch(Block block)
		{
			if (block.Statements.Count == 0)
				return null;
			return block.Statements.Last.Instruction as Branch;
		}

		public virtual void Transform()
		{
		}
	}

}
