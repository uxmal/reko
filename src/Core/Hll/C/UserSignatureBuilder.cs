#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Builds ProcedureSignatures from user-supplied signatures.
    /// </summary>
    public class UserSignatureBuilder
    {
        private readonly Program program;

        /// <summary>
        /// Constructs a new <see cref="UserSignatureBuilder"/> instance.
        /// </summary>
        /// <param name="program">Program context.</param>
        public UserSignatureBuilder(Program program)
        {
            this.program = program;
        }

        /// <summary>
        /// For each procedure, either use a user-supplied signature, 
        /// or the predefined one.
        /// </summary>
        public void BuildSignatures(IEventListener listener)
        {
            foreach (var de in program.Procedures)
            {
                if (listener.IsCanceled())
                    break;
                BuildSignature(de.Key, de.Value);
            }
        }

        /// <summary>
        /// Builds a signature for the specified procedure, if there is 
        /// user data for it.
        /// </summary>
        /// <param name="addr">Address of the procedure.</param>
        /// <param name="proc">Procedure whose signature is to be built.</param>
        public void BuildSignature(Address addr, Procedure proc)
        {
            if (program.User.Procedures.TryGetValue(addr, out var userProc))
            {
                var sProc = DeserializeSignature(userProc, proc);
                if (sProc is not null)
                {
                    var ser = program.CreateProcedureSerializer();
                    var sig = ser.Deserialize(sProc.Signature, proc.Frame);
                    if (sig is not null)
                    {
                        proc.Name = sProc.Name ?? program.NamingPolicy.ProcedureName(proc.EntryAddress);
                        proc.Signature = sig;
                        return;
                    }
                }
                proc.Name = userProc.Name ?? proc.Name;
            }
        }

        /// <summary>
        /// Given a user procedure, deserialize the signature.
        /// </summary>
        /// <param name="userProc">User procedure.</param>
        /// <param name="proc">Procedure corresponding </param>
        /// <returns></returns>
        public ProcedureBase_v1? DeserializeSignature(UserProcedure userProc, Procedure proc)
        {
            if (!string.IsNullOrEmpty(userProc.CSignature))
            {
                return ParseFunctionDeclaration(userProc.CSignature);
            }
            return null;
        }

        /// <summary>
        /// Parses a C function declaration and returns the signature.
        /// </summary>
        /// <param name="fnDecl"></param>
        /// <returns></returns>
        public ProcedureBase_v1? ParseFunctionDeclaration(string? fnDecl)
        {
            if (string.IsNullOrEmpty(fnDecl))
                return null;
            try {
                var rdr = new StringReader(fnDecl + ";");
                var symbols = program.CreateSymbolTable();
                var oldProcs = symbols.Procedures.Count;
                var cstate = new ParserState(symbols);
                var cParser = program.Platform.CreateCParser(rdr, cstate);
                var decl = cParser.Parse_ExternalDecl();
                if (decl is null
                    )
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

        /// <summary>
        /// Parses a user's global variable declaration.
        /// </summary>
        /// <param name="txtGlobal">Global variable declaration.</param>
        /// <returns>A global data item.</returns>
        public GlobalDataItem_v2? ParseGlobalDeclaration(string txtGlobal)
        {
            try
            {
                var lexer = new CLexer(new StringReader(txtGlobal + ";"), CLexer.GccKeywords);  //$REVIEW: what's the right thing?
                var symbols = program.CreateSymbolTable();
                var oldVars = symbols.Variables.Count;
                var cstate = new ParserState(symbols);
                var cParser = new CParser(cstate, lexer);
                var decl = cParser.Parse_ExternalDecl();
                if (decl is null)
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

        private static readonly Regex regexCIdentifier = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");

        /// <summary>
        /// Determines if the given identifier is a valid C identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsValidCIdentifier(string id)
        {
            //$REFACTOR: this is language and platform dependent.
            return regexCIdentifier.IsMatch(id);
        }
    }
}
