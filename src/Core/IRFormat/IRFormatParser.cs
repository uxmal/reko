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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.IRFormat
{
    public class IRFormatParser
    {
        private IRFormatLexer lex;
        private Program program;

        public IRFormatParser(TextReader rdr)
        {
            this.lex = new IRFormatLexer(rdr);
        }

        public Program Parse()
        {
            this.program = new Program();

            //ParseTarget();

            while (!lex.Peek(IRTokenType.EOF))
            {
                ParseProgramItem();
            }
            return program;
        }

        public void ParseProgramItem()
        {
            switch (lex.Get().Type)
            {
            case IRTokenType.Define:
                ParseProcedureDefinition();
                break;
            }
        }

        public void ParseProcedureDefinition()
        {
            //var name = Expect(IRTokenType.ID).Value;
            //var m = new IRProcedureBuilder(name, program.Platform);

            //if (PeekAndDiscard(IRTokenType.Addr))
            //{
            //    lex.SkipSpaces();
            //    var sAddr = lex.GetSpaceDelimitedString();
            //    Address addr;
            //    if (!program.Architecture.TryParseAddress(sAddr, out addr))
            //        throw new FormatException("Invalid address line", lex.LineNumber);
            //    program.Procedures.Add(addr, )
            //}
        }

        private Token Expect(IRTokenType type)
        {
            throw new NotImplementedException();
        }

        // target x86-Win32

        // void foo()
        // define foo

        // Frame
        // def eax:word32 reg(eax 0)
        // id = <expr>
        // call/ret/sideffect/phi/if
        // label
        // backpatch labels.

    }
}
