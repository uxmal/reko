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
using System.Collections.Generic;

namespace Reko.Core.Hll.C
{
    public class CharacteristicsParser
    {
        private readonly List<CToken>? tokens;
        private int iToken;

        public CharacteristicsParser(CAttribute attrCharacteristics)
        {
            this.tokens = attrCharacteristics.Tokens;
        }

        public ProcedureCharacteristics Parse()
        {
            if (!PeekAndDiscard(CTokenType.LBrace))
                return DefaultProcedureCharacteristics.Instance;
            if (PeekAndDiscard(CTokenType.RBrace))
                return DefaultProcedureCharacteristics.Instance;
            bool terminates = false;
            bool alloca = false;
            string? sVarargs = null;
            do
            {
                var charName = Expect<string>(CTokenType.Id, "identifier");
                Expect(CTokenType.Colon, ":");
                switch (charName)
                {
                case "terminates":
                    var sTerminates = Expect<string>(CTokenType.Id, "true or false");
                    terminates = (sTerminates == "true");
                    break;
                case "alloca":
                    var sAlloca = Expect<string>(CTokenType.Id, "true or false");
                    alloca = (sAlloca != "false" && sAlloca != "0");
                    break;
                case "varargs":
                    sVarargs = Expect<string>(CTokenType.StringLiteral, "string literal");
                    break;
                default:
                    throw new CParserException($"Unexpected procedure characteristic '{charName}'.");
                }
            } while (PeekAndDiscard(CTokenType.Comma));
            Expect(CTokenType.RBrace, "}");
            return new ProcedureCharacteristics
            {
                Terminates = terminates,
                IsAlloca = alloca,
                VarargsParserClass = sVarargs,
            };
        }

        private bool PeekAndDiscard(CTokenType type)
        {
            if (tokens is { } && iToken < tokens.Count && tokens[iToken].Type == type)
            {
                ++iToken;
                return true;
            }
            else
            {
                return false;
            }

        }
        private void Expect(CTokenType type, string errorMsg)
        {
            if (tokens is null || iToken >= tokens.Count || tokens[iToken++].Type != type)
                throw new CParserException($"Expected '{errorMsg}'.");
        }

        private T Expect<T>(CTokenType type, string errorMsg)
        {
            if (tokens is null || iToken >= tokens.Count || tokens[iToken].Type != type)
                throw new CParserException($"Expected '{errorMsg}'.");
            return (T) tokens[iToken++].Value!;
        }
    }
}