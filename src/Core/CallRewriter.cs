#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using System;

namespace Reko.Core
{
    /// <summary>
    /// Rewrite call statements to Applications.
    /// </summary>
    /// <remarks>
    /// Call statements are rewritten by consulting the ProcedureSignature
    /// of the called function (callee). 
    /// </remarks>
	public class CallRewriter
	{
		public CallRewriter(Program program)
		{
            this.Program = program;
        }

        public static void Rewrite(Program program, DecompilerEventListener listener)
        {
            var crw = new CallRewriter(program);
            foreach (Procedure proc in program.Procedures.Values)
            {
                if (listener.IsCanceled())
                    break;
                crw.RewriteCalls(proc);
                crw.RewriteReturns(proc);
            }
        }

        public Program Program { get; private set; }

		public virtual FunctionType GetProcedureSignature(ProcedureBase proc)
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
            var callee = call.Callee as ProcedureConstant;
            if (callee == null)
                return false;          //$REVIEW: what happens with indirect calls?
			var procCallee = callee.Procedure;
			var sigCallee = GetProcedureSignature(procCallee);
			var fn = new ProcedureConstant(Program.Platform.PointerType, procCallee);
            if (sigCallee == null || !sigCallee.ParametersValid)
                return false;

            var ab = new ApplicationBuilder(Program.Architecture, proc.Frame, call.CallSite, fn, sigCallee, true);
			stm.Instruction = ab.CreateInstruction();
            return true;
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
            int unConverted = 0;
            foreach (Statement stm in proc.Statements)
            {
                CallInstruction ci = stm.Instruction as CallInstruction;
                if (ci != null)
                {
                    if (!RewriteCall(proc, stm, ci))
                        ++unConverted;
                }
            }
            return unConverted;
        }

        /// <summary>
        /// Having identified the return variable -- if any, rewrite all 
        /// return statements to return that variable.
        /// </summary>
        /// <param name="proc"></param>
        public void RewriteReturns(Procedure proc)
        {
            Identifier idRet = proc.Signature.ReturnValue;
            if (idRet == null || idRet.DataType is VoidType)
                return;
            foreach (Statement stm in proc.Statements)
            {
                var ret = stm.Instruction as ReturnInstruction;
                if (ret != null)
                {
                    ret.Expression = proc.Frame.EnsureIdentifier(idRet.Storage);
                }
            }
        }
    }
}
