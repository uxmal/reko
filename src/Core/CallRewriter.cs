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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public class CallRewriter
	{
		private ApplicationBuilder ab;

		public CallRewriter()
		{
		}

		public virtual ProcedureSignature GetProcedureSignature(Procedure proc)
		{
			return proc.Signature;
		}

		/// <summary>
		/// Rewrites CALL instructions to function applications.
		/// </summary>
		/// <remarks>
		/// Converts an opcode:
		/// <code>
		///   call procExpr 
		/// </code>
		/// to one of:
		/// <code>
		///	 ax = procExpr(bindings);
		///  procEexpr(bindings);
		/// </code>
		/// </remarks>
		/// <param name="proc">Procedure in which the CALL instruction exists</param>
		/// <param name="stm">The particular statement of the call instruction</param>
		/// <param name="call">The actuall CALL instruction.</param>
		/// <returns>True if the conversion was possible, false if the procedure didn't have
		/// a signature yet.</returns>
		public bool RewriteCall(Procedure proc, Statement stm, CallInstruction call)
		{
			Procedure procCallee = call.Callee;
			ProcedureSignature sigCallee = GetProcedureSignature(procCallee);
			ProcedureConstant fn = new ProcedureConstant(PrimitiveType.Pointer, procCallee);
			Instruction instr = ab.BuildApplication(call.CallSite, fn, sigCallee);
			if (instr != null)
			{
				stm.Instruction = instr;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		// Statements of the form:
		//		call	<proc-operand>
		// become redefined to 
		//		ret = <proc-operand>(bindings)
		// where ret is the return register (if any) and the
		// bindings are the bindings of the procedure.
		/// </summary>
		/// <param name="proc"></param>
		/// <returns>The number of calls that couldn't be converted</returns>
		public int RewriteCalls(Procedure proc)
		{
			ab = new ApplicationBuilder(proc.Frame);
			int unConverted = 0;
			foreach (Block b in proc.RpoBlocks)
			{
				foreach (Statement stm in b.Statements)
				{
					CallInstruction ci = stm.Instruction as CallInstruction;
					if (ci != null)
					{
						if (!RewriteCall(proc, stm, ci))
							++unConverted;
					}
				}
			}
			return unConverted;
		}
	}
}
