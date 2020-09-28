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
using Reko.Core.CLanguage;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Analysis
{
    /// <summary>
    /// Builds ProcedureSignatures from user-supplied signatures.
    /// </summary>
    public class UserSignatureBuilder
    {
        private readonly Program program;

        public UserSignatureBuilder(Program program)
        {
            this.program = program;
        }

        /// <summary>
        /// For each procedure, either use a user-supplied signature, 
        /// or the predefined one.
        /// </summary>
        public void BuildSignatures(DecompilerEventListener listener)
        {
            foreach (var de in program.Procedures)
            {
                if (listener.IsCanceled())
                    break;
                BuildSignature(de.Key, de.Value);
            }
        }

        public void BuildSignature(Address addr, Procedure proc)
        {
            if (program.User.Procedures.TryGetValue(addr, out Procedure_v1 userProc))
            {
                var sProc = DeserializeSignature(userProc, proc);
                if (sProc != null)
                {
                    var ser = program.CreateProcedureSerializer();
                    var sig = ser.Deserialize(sProc.Signature, proc.Frame);
                    if (sig != null)
                    {
                        proc.Name = sProc.Name;
                        proc.Signature = sig;
                        return;
                    }
                }
            }
        }

        public ProcedureBase_v1 DeserializeSignature(Procedure_v1 userProc, Procedure proc)
        {
            if (!string.IsNullOrEmpty(userProc.CSignature))
            {
                return ParseFunctionDeclaration(userProc.CSignature);
            }
            return null;
        }

        /// <summary>
        /// Given a decompiled procedure `proc` and a function signature `sig`, introduce
        /// copy instructions that copy the formal parameters into the body of the procedure.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="sig"></param>
        /// <param name="proc"></param>
        [Obsolete("This now happens inside of SSA Transformation.")]
        public void ApplySignatureToProcedure(Address addr, FunctionType sig, Procedure proc)
        {
            proc.Signature = sig;
            // The copy instructions have to happen before any of the 
            // "real" code, so they have to live in the entry block.
            var stmts = proc.EntryBlock.Statements;
            int i = stmts.Count;

            var linAddr = addr.ToLinear();
            var m = new ExpressionEmitter();
            foreach (var param in sig.Parameters)
            {
                if (param.Storage is StackArgumentStorage starg)
                {
                    proc.Frame.EnsureStackArgument(
                        starg.StackOffset,
                        param.DataType,
                        param.Name);
                    var fp = proc.Frame.FramePointer;
                    stmts.Insert(i, linAddr, new Store(
                        m.Mem(param.DataType, m.IAdd(fp, starg.StackOffset)),
                        param));
                }
                else
                {
                    var paramId = proc.Frame.EnsureIdentifier(param.Storage);

                    // Need to take an extra step with parameters being passed
                    // in a register. It's perfectly possible for a user to 
                    // create a variable which they want to call 'r2' but which
                    // the calling convention of the machine wants to call 'r1'.
                    // To avoid this, we create a temporary identifier for 
                    // the formal parameter, and inject an copy statement in the
                    // entry block that moves the parameter value into the 
                    // register.
                    stmts.Insert(i, linAddr, new Assignment(param, paramId));
                }
                ++i;
            }
        }

        public ProcedureBase_v1 ParseFunctionDeclaration(string fnDecl)
        {
            try {
                var lexer = new CLexer(new StringReader(fnDecl + ";"));
                var symbols = program.CreateSymbolTable();
                var oldProcs = symbols.Procedures.Count;
                var cstate = new ParserState(symbols);
                var cParser = new CParser(cstate, lexer);
                var decl = cParser.Parse_ExternalDecl();
                if (decl == null)
                    return null;

                //$HACK: Relying on a side effect here to
                // get both the procedure name and the signature. Ew.
                symbols.AddDeclaration(decl);
                if (symbols.Procedures.Count == oldProcs)
                    return null;
                return symbols.Procedures.Last();
            }
            catch (Exception ex)
            {
                //$TODO: if user has supplied a signature that can't parse,
                // we must notify them in the diagnostics window with a 
                // WARNING.
                Debug.Print("{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public GlobalDataItem_v2 ParseGlobalDeclaration(string txtGlobal)
        {
            try
            {
                var lexer = new CLexer(new StringReader(txtGlobal + ";"));
                var symbols = program.CreateSymbolTable();
                var oldVars = symbols.Variables.Count;
                var cstate = new ParserState(symbols);
                var cParser = new CParser(cstate, lexer);
                var decl = cParser.Parse_ExternalDecl();
                if (decl == null)
                    return null;

                //$HACK: Relying on a side effect here to
                // get both the global type. Ew.
                symbols.AddDeclaration(decl);
                if (symbols.Variables.Count == oldVars)
                    return null;
                return symbols.Variables.Last();
            }
            catch (Exception ex)
            {
                Debug.Print("{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        //$REFACTOR: this is language and platform dependent.
        public static bool IsValidCIdentifier(string id)
        {
            return Regex.IsMatch(id, "^[_a-zA-Z][_a-zA-Z0-9]*$");
        }
    }
}
