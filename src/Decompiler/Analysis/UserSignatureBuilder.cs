#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    public class UserSignatureBuilder
    {
        private Program program;
        private SymbolTable symbolTable;

        public UserSignatureBuilder(Program program)
        {
            this.program = program;
            this.symbolTable = new SymbolTable();
        }

        public void BuildSignatures()
        {
            foreach (var de in program.UserProcedures
                .Where(d => !string.IsNullOrEmpty(d.Value.CSignature)))
            {
                Procedure proc;
                if (!program.Procedures.TryGetValue(de.Key, out proc))
                    continue;
                var sig = BuildSignature(de.Value.CSignature, proc.Frame);
                if (sig == null)
                    continue;
                proc.Signature = sig;
            }
        }

        public ProcedureSignature BuildSignature(string str, Frame frame)
        {
            try {
                var lexer = new CLexer(new StringReader(str + ";"));
                var cstate = new ParserState();
                var cParser = new CParser(cstate, lexer);
                var decl = cParser.Parse_ExternalDecl();
                var sSig = symbolTable.AddDeclaration(decl)
                    .OfType<SerializedSignature>()
                    .FirstOrDefault();
                if (sSig == null)
                    return null;
                var ser = program.Platform.CreateProcedureSerializer();
                return ser.Deserialize(sSig, frame);
            }
            catch (Exception ex)
            {
                Debug.Print("{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }
    }
}
